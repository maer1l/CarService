using CarService.Data;
using CarService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace CarService.Controllers
{
    public class ProductsController : Controller
    {
        private readonly CarserviceContext _context;

        public ProductsController(CarserviceContext context)
        {
            _context = context;
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {
            var carserviceContext = _context.Products.Include(p => p.Category);
            return View(await carserviceContext.ToListAsync());
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName"); // второй параметр это отображаемое значение
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductId,CategoryId,SerialNumber,Price,ReleaseYear,Brand,Model")] Product product)
        {
            if (ModelState.IsValid)
            {
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", product.CategoryId);
            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", product.CategoryId);
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductId,CategoryId,SerialNumber,Price,ReleaseYear,Brand,Model")] Product product)
        {
            decimal price = 0;
            if (decimal.TryParse(product.Price.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out price))
            {
                product.Price = price;
            }

            if (id != product.ProductId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ProductId))
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
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryId", product.CategoryId);
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }

        [HttpPost]
        public ActionResult ValidateProduct(Product product)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    return Json(new { success = true}); // ajax в require
                }
                else
                {
                    Dictionary<string, string> errorsDictionary = new Dictionary<string, string>();

                    foreach (var errors in ModelState)
                    {
                        errorsDictionary.Add(errors.Key, errors.Value.Errors[0].ErrorMessage);
                    }

                    return Json(new { success = false, errors = errorsDictionary });
                }
            }
            catch
            {
                return Json(new { success = false, errors = new Dictionary<string, string>() });
            }
        }

        [HttpPost]
        public async Task<ActionResult> SearchProduct(string word)
        {
            IEnumerable<Product> filteredProducts = null;
            if (!word.IsNullOrEmpty())
            {
                word = word.Trim();
                var products = await _context.Products.Include(p => p.Category).ToListAsync();
                decimal searchword = 0;
                if (decimal.TryParse(word, NumberStyles.Any, CultureInfo.InvariantCulture, out searchword))
                {
                    filteredProducts = from p in products where p.SerialNumber == decimal.ToInt32(searchword) select p;
                    filteredProducts = filteredProducts.Union(from p in products where p.Price == searchword select p);
                    filteredProducts = filteredProducts.Union(from p in products where p.ReleaseYear == decimal.ToInt32(searchword) select p);
                    filteredProducts = filteredProducts.Union(from p in products where p.Brand == searchword.ToString() select p);
                    filteredProducts = filteredProducts.Union(from p in products where p.Model == searchword.ToString() select p);
                }
                else
                {
                    filteredProducts = from p in products where p.Brand == word select p;
                    filteredProducts = filteredProducts.Union(from p in products where p.Model == word select p);
                    var catId = await _context.Categories.SingleOrDefaultAsync(m => m.CategoryName == word);
                    if (catId != null)
                    {
                        filteredProducts = filteredProducts.Union(from p in products where p.CategoryId == catId.CategoryId select p);
                    }
                    filteredProducts = filteredProducts.Union(from p in products where p.Model == word select p);
                }
            }

            return PartialView(filteredProducts);
        }
    }
}
