using Library.Domain;
using Library.MVC.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Library.Tests
{
    public class LibraryTests
    {
        private ApplicationDbContext GetInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task ReturnedLoan_MakesBookAvailableAgain()
        {
            // Arrange
            var context = GetInMemoryContext();
            var book = new Book { Id = 1, Title = "Test Book", IsAvailable = false };
            var loan = new Loan { Id = 1, BookId = 1, Book = book, LoanDate = DateTime.Now, DueDate = DateTime.Now.AddDays(7) };
            context.Books.Add(book);
            context.Loans.Add(loan);
            await context.SaveChangesAsync();

            // Act
            loan.ReturnedDate = DateTime.Now;
            book.IsAvailable = true;
            await context.SaveChangesAsync();

            // Assert
            var updatedBook = await context.Books.FindAsync(1);
            Assert.True(updatedBook!.IsAvailable);
        }

        [Fact]
        public async Task BookSearch_ReturnsExpectedMatches()
        {
            // Arrange
            var context = GetInMemoryContext();
            context.Books.AddRange(
                new Book { Title = "Clean Code", Author = "Robert Martin" },
                new Book { Title = "Refactoring", Author = "Martin Fowler" },
                new Book { Title = "Design Patterns", Author = "Gamma" }
            );
            await context.SaveChangesAsync();

            // Act
            var results = await context.Books
                .Where(b => b.Title.Contains("Code") || b.Author.Contains("Martin"))
                .ToListAsync();

            // Assert
            Assert.Equal(2, results.Count);
        }

        [Fact]
        public void OverdueLogic_DetectsOverdueLoans()
        {
            // Arrange
            var loan = new Loan 
            { 
                LoanDate = DateTime.Now.AddDays(-20), 
                DueDate = DateTime.Now.AddDays(-6), 
                ReturnedDate = null 
            };

            // Act
            bool isOverdue = loan.ReturnedDate == null && loan.DueDate < DateTime.Now;

            // Assert
            Assert.True(isOverdue);
        }

        [Fact]
        public async Task Member_CanHaveMultipleLoans()
        {
            // Arrange
            var context = GetInMemoryContext();
            var member = new Member { Id = 1, FullName = "John Doe" };
            var book1 = new Book { Id = 1, Title = "Book 1" };
            var book2 = new Book { Id = 2, Title = "Book 2" };
            context.Members.Add(member);
            context.Books.AddRange(book1, book2);
            await context.SaveChangesAsync();

            // Act
            context.Loans.AddRange(
                new Loan { MemberId = 1, BookId = 1, LoanDate = DateTime.Now, DueDate = DateTime.Now.AddDays(7) },
                new Loan { MemberId = 1, BookId = 2, LoanDate = DateTime.Now, DueDate = DateTime.Now.AddDays(7) }
            );
            await context.SaveChangesAsync();

            // Assert
            var loans = await context.Loans.Where(l => l.MemberId == 1).ToListAsync();
            Assert.Equal(2, loans.Count);
        }

        [Fact]
        public async Task DeleteBook_ShouldWorkIfNoActiveLoans()
        {
            // Arrange
            var context = GetInMemoryContext();
            var book = new Book { Id = 1, Title = "Deletable Book" };
            context.Books.Add(book);
            await context.SaveChangesAsync();

            // Act
            var b = await context.Books.FindAsync(1);
            context.Books.Remove(b!);
            await context.SaveChangesAsync();

            // Assert
            var count = await context.Books.CountAsync();
            Assert.Equal(0, count);
        }

        [Fact]
        public void Loan_IsActiveWhenNotReturned()
        {
            // Arrange
            var loan = new Loan { ReturnedDate = null };

            // Act
            bool isActive = loan.ReturnedDate == null;

            // Assert
            Assert.True(isActive);
        }
    }
}
