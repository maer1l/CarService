using CarService.Areas.Identity.Data;
using CarService.Data;
using CarService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
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
        public OrdersController(UserManager<ApplicationUser> userManager, CarserviceContext context, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _context = context;
            _roleManager = roleManager;
        }

        // GET: Orders
        [Authorize(Roles = "Admin,Master")]
        public async Task<IActionResult> Index()
        {
            IQueryable<Order> carserviceContext = null;
            var userId = _userManager.GetUserId(User);
            var user = await _userManager.FindByIdAsync(userId);
            var role = await _userManager.GetRolesAsync(user);
            if (role[0] == "Master")
            {
                carserviceContext = from p in _context.Orders.Include(o => o.Client).Include(o => o.Product) where p.MasterId == userId select p;
                return View(await carserviceContext.ToListAsync());
            }
            carserviceContext = _context.Orders.Include(o => o.Client).Include(o => o.Product);
            return View(await carserviceContext.ToListAsync());
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
                ViewData["MasterId"] = userId;
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
                ViewData["MasterId"] = userId;
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
    }
}
