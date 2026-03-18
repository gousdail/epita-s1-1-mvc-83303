using Library.Domain;
using Library.MVC.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Library.MVC.Controllers
{
    public class LoansController(ApplicationDbContext context) : Controller
    {
        public async Task<IActionResult> Index()
        {
            var loans = await context.Loans
                .Include(l => l.Book)
                .Include(l => l.Member)
                .OrderByDescending(l => l.LoanDate)
                .ToListAsync();
            return View(loans);
        }

        public async Task<IActionResult> Create()
        {
            var availableBooks = await context.Books.Where(b => b.IsAvailable).ToListAsync();
            var members = await context.Members.ToListAsync();

            ViewData["BookId"] = new SelectList(availableBooks, "Id", "Title");
            ViewData["MemberId"] = new SelectList(members, "Id", "FullName");
            
            var loan = new Loan
            {
                LoanDate = DateTime.Now,
                DueDate = DateTime.Now.AddDays(14)
            };

            return View(loan);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Loan loan)
        {
            // Check if book is still available
            var book = await context.Books.FindAsync(loan.BookId);
            if (book == null || !book.IsAvailable)
            {
                ModelState.AddModelError("", "Book is not available for loan.");
            }

            if (ModelState.IsValid)
            {
                book!.IsAvailable = false;
                context.Loans.Add(loan);
                await context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            var availableBooks = await context.Books.Where(b => b.IsAvailable).ToListAsync();
            var members = await context.Members.ToListAsync();
            ViewData["BookId"] = new SelectList(availableBooks, "Id", "Title", loan.BookId);
            ViewData["MemberId"] = new SelectList(members, "Id", "FullName", loan.MemberId);
            return View(loan);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkReturned(int id)
        {
            var loan = await context.Loans.Include(l => l.Book).FirstOrDefaultAsync(l => l.Id == id);
            if (loan != null && loan.ReturnedDate == null)
            {
                loan.ReturnedDate = DateTime.Now;
                if (loan.Book != null)
                {
                    loan.Book.IsAvailable = true;
                }
                await context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
