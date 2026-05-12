using AspNetCoreGeneratedDocument;
using CarService.Areas.Identity.Data;
using CarService.Data;
using CarService.Models;
using CarService.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarService.Controllers
{
    public class OrdersController : Controller
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly CarserviceContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _dbcontext;
        public OrdersController(UserManager<ApplicationUser> userManager, CarserviceContext context, RoleManager<IdentityRole> roleManager, ApplicationDbContext dbcontext)
        {
            _userManager = userManager;
            _context = context;
            _roleManager = roleManager;
            _dbcontext = dbcontext;
        }

        // GET: Orders
        [Authorize(Roles = "Admin,Master")]
        public async Task<IActionResult> Index()
        {
            IQueryable<Order> carserviceContext = null;
            var userId = _userManager.GetUserId(User);
            var user = await _userManager.FindByIdAsync(userId);
            var role = await _userManager.GetRolesAsync(user);
            var users = await _userManager.Users.ToListAsync();

            Dictionary<string, string> o = new Dictionary<string, string>();
            for (int i = 0; i < users.Count(); i++)
            {
                var userRole = await _userManager.GetRolesAsync(users[i]);
                if (userRole[0] == "Master")
                {
                    o.Add(users[i].Id, users[i].Email);
                }
            }

            if (role[0] == "Master")
            {
                carserviceContext = from p in _context.Orders.Include(o => o.Client).Include(o => o.Product) where p.MasterId == userId select p;
                var masterModel = new OrdersViewModel
                {
                    orders = await carserviceContext.ToListAsync(),
                    masters = o
                };
                return View(masterModel);
            }
            carserviceContext = _context.Orders.Include(o => o.Client).Include(o => o.Product);

            var model = new OrdersViewModel
            {
                orders = await carserviceContext.ToListAsync(),
                masters = o
            };

            //return View(await carserviceContext.ToListAsync());
            return View(model);
        }

        // GET: Orders/Details/5
        [Authorize(Roles = "Admin,Master")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Client)
                .Include(o => o.Product)
                .FirstOrDefaultAsync(m => m.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // GET: Orders/Create
        [Authorize(Roles = "Admin,Master")]
        public async Task<IActionResult> Create()
        {
            ViewData["ClientId"] = new SelectList(_context.Clients, "ClientId", "Name");
            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "Model");
            var userId = _userManager.GetUserId(User);
            var user = await _userManager.FindByIdAsync(userId);
            var role = await _userManager.GetRolesAsync(user);
            if (role[0] == "Master")
            {
                ViewData["MasterId"] = userId;
            }
            else
            {
                var roles = await _userManager.GetUsersInRoleAsync("Master");
                ViewData["MasterId"] = new SelectList(roles, "Id", "UserName");
            }
            return View();
        }

        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Master")]
        public async Task<IActionResult> Create([Bind("OrderId,ProductId,MasterId,ClientId,StartDate,EndDate,Price")] Order order)
        {
            //order.Client = await _context.Clients.SingleOrDefaultAsync(m => m.ClientId == order.ClientId);
            ModelState.Remove("Client");
            if (ModelState.IsValid)
            {
                _context.Add(order);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ClientId"] = new SelectList(_context.Clients, "ClientId", "Name", order.ClientId);
            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "Model", order.ProductId);
            var userId = _userManager.GetUserId(User);
            var user = await _userManager.FindByIdAsync(userId);
            var role = await _userManager.GetRolesAsync(user);
            if (role[0] == "Master")
            {
                ViewData["MasterId"] = userId;
            }
            else
            {
                var roles = await _userManager.GetUsersInRoleAsync("Master");
                ViewData["MasterId"] = new SelectList(roles, "Id", "UserName", order.MasterId);
            }
            return View(order);
        }

        // GET: Orders/Edit/5
        [Authorize(Roles = "Admin,Master")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            ViewData["ClientId"] = new SelectList(_context.Clients, "ClientId", "Name", order.ClientId);
            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "Model", order.ProductId);
            var userId = _userManager.GetUserId(User);
            var user = await _userManager.FindByIdAsync(userId);
            var role = await _userManager.GetRolesAsync(user);
            if (role[0] == "Master")
            {
                if(order.MasterId != userId)
                {
                    return Forbid();
                }

                //ViewData["MasterId"] = userId;
                ViewData["MasterId"] = new SelectList(new[] { new { Id = userId, Name = user.Email } }, "Id", "Name", userId);
            }
            else
            {
                var roles = await _userManager.GetUsersInRoleAsync("Master");
                ViewData["MasterId"] = new SelectList(roles, "Id", "UserName", order.MasterId);
            }
            return View(order);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Master")]
        public async Task<IActionResult> Edit(int id, [Bind("OrderId,ProductId,MasterId,ClientId,StartDate,EndDate,Price")] Order order)
        {
            if (id != order.OrderId)
            {
                return NotFound();
            }


            ModelState.Remove("Client");
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.OrderId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ClientId"] = new SelectList(_context.Clients, "ClientId", "Name", order.ClientId);
            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "Model", order.ProductId);
            var userId = _userManager.GetUserId(User);
            var user = await _userManager.FindByIdAsync(userId);
            var role = await _userManager.GetRolesAsync(user);
            if (role[0] == "Master")
            {
                if (order.MasterId != userId)
                {
                    return Forbid();
                }

                //ViewData["MasterId"] = userId;
                ViewData["MasterId"] = new SelectList(new[] { new { Id = userId, Name = user.Email } }, "Id", "Name", userId);
            }
            else
            {
                var roles = await _userManager.GetUsersInRoleAsync("Master");
                ViewData["MasterId"] = new SelectList(roles, "Id", "UserName", order.MasterId);
            }
            return View(order);
        }

        // GET: Orders/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Client)
                .Include(o => o.Product)
                .FirstOrDefaultAsync(m => m.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                _context.Orders.Remove(order);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.OrderId == id);
        }

        [HttpPost]
        public async Task<IActionResult> SearchOrder(string word)
        {
            IQueryable<Order> carserviceContext = null;
            var userId = _userManager.GetUserId(User);
            var user = await _userManager.FindByIdAsync(userId);
            var role = await _userManager.GetRolesAsync(user);
            var users = await _userManager.Users.ToListAsync();
            Dictionary<string, string> o = new Dictionary<string, string>();
            for (int i = 0; i < users.Count(); i++)
            {
                var userRole = await _userManager.GetRolesAsync(users[i]);
                if (userRole[0] == "Master")
                {
                    o.Add(users[i].Id, users[i].Email);
                }
            }

            if (role[0] == "Master")
            {
                carserviceContext = from p in _context.Orders.Include(o => o.Client).Include(o => o.Product) where p.MasterId == userId select p;
                var masterModel = new OrdersViewModel
                {
                    orders = await carserviceContext.ToListAsync(),
                    masters = o
                };
                return View(masterModel);
            }
            carserviceContext = _context.Orders.Include(o => o.Client).Include(o => o.Product);

            if (!word.IsNullOrEmpty())
            {
                word = word.Trim();
                decimal d = 0;
                DateOnly date = new DateOnly();
                if(decimal.TryParse(word, out d))
                {
                    carserviceContext = carserviceContext.Where(u => u.Price == d);
                    
                }
                else if(DateOnly.TryParse(word, out date)){
                    carserviceContext = carserviceContext.Where(u => (u.StartDate == date) || (u.EndDate == date));
                }
                else
                {
                    List<string>? masterId = await _dbcontext.Users.Where(u => u.Email == word).Select(u => u.Id).ToListAsync();
                    List<int>? clientId = await _context.Clients.Where(c => c.Name.Contains(word)).Select(c => c.ClientId).ToListAsync();
                    List<int>? productId = await _context.Products.Where(p => p.Model == word).Select(p => p.ProductId).ToListAsync();

                    carserviceContext = carserviceContext.Where(u => (masterId.Count > 0 && masterId.Contains(u.MasterId)) || (clientId.Count > 0 && clientId.Contains(u.ClientId)) || (productId.Count > 0 && productId.Contains((int)u.ProductId)));
                }
            }

            var model = new OrdersViewModel
            {
                orders = await carserviceContext.ToListAsync(),
                masters = o
            };

            return PartialView(model);
        }
    }
}
