using Library.Domain;
using Library.MVC.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Library.MVC.Controllers
{
    public class BooksController(ApplicationDbContext context) : Controller
    {
        public async Task<IActionResult> Index(string searchString, string category, string availability, string sortOrder)
        {
            var query = context.Books.AsQueryable();

            // 1. Search by Title or Author
            if (!string.IsNullOrWhiteSpace(searchString))
            {
                query = query.Where(b => b.Title.Contains(searchString) || b.Author.Contains(searchString));
            }

            // 2. Filter by Category
            if (!string.IsNullOrWhiteSpace(category))
            {
                query = query.Where(b => b.Category == category);
            }

            // 3. Filter by Availability
            if (!string.IsNullOrWhiteSpace(availability))
            {
                if (availability == "Available")
                    query = query.Where(b => b.IsAvailable);
                else if (availability == "OnLoan")
                    query = query.Where(b => !b.IsAvailable);
            }

            // 4. Sorting (optional)
            ViewData["TitleSortParm"] = string.IsNullOrEmpty(sortOrder) ? "title_desc" : "";
            switch (sortOrder)
            {
                case "title_desc":
                    query = query.OrderByDescending(b => b.Title);
                    break;
                default:
                    query = query.OrderBy(b => b.Title);
                    break;
            }

            // Prepare filter data
            var categories = await context.Books.Select(b => b.Category).Distinct().ToListAsync();
            ViewData["CategoryList"] = new SelectList(categories);
            ViewData["CurrentSearch"] = searchString;
            ViewData["CurrentCategory"] = category;
            ViewData["CurrentAvailability"] = availability;

            return View(await query.ToListAsync());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Book book)
        {
            if (ModelState.IsValid)
            {
                context.Books.Add(book);
                await context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(book);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var book = await context.Books.FindAsync(id);
            if (book == null) return NotFound();
            return View(book);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Book book)
        {
            if (id != book.Id) return NotFound();
            if (ModelState.IsValid)
            {
                context.Books.Update(book);
                await context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(book);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var book = await context.Books.FindAsync(id);
            if (book == null) return NotFound();
            return View(book);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var book = await context.Books.FindAsync(id);
            if (book != null)
            {
                context.Books.Remove(book);
                await context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
