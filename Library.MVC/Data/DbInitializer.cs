using Bogus;
using Library.Domain;
using Library.MVC.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Library.MVC.Data
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

            // 1. Ensure DB is created/migrated
            // Note: If you have issues with migrations, you can use context.Database.EnsureCreatedAsync() instead
            await context.Database.MigrateAsync();

            // 2. Seed Roles
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            // 3. Seed Admin User
            var adminEmail = "admin@library.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new IdentityUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(adminUser, "Admin123!");
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }

            // 4. Seed Books
            if (!context.Books.Any())
            {
                var bookFaker = new Faker<Book>()
                    .RuleFor(b => b.Title, f => f.Commerce.ProductName())
                    .RuleFor(b => b.Author, f => f.Person.FullName)
                    .RuleFor(b => b.Isbn, f => f.Random.Replace("###-##########"))
                    .RuleFor(b => b.Category, f => f.PickRandom(new[] { "Fiction", "Science", "History", "Biography", "Fantasy" }))
                    .RuleFor(b => b.IsAvailable, f => true);

                var books = bookFaker.Generate(20);
                context.Books.AddRange(books);
                await context.SaveChangesAsync();
            }

            // 5. Seed Members
            if (!context.Members.Any())
            {
                var memberFaker = new Faker<Member>()
                    .RuleFor(m => m.FullName, f => f.Person.FullName)
                    .RuleFor(m => m.Email, f => f.Person.Email)
                    .RuleFor(m => m.Phone, f => f.Person.Phone);

                var members = memberFaker.Generate(10);
                context.Members.AddRange(members);
                await context.SaveChangesAsync();
            }

            // 6. Seed Loans
            if (!context.Loans.Any())
            {
                var books = await context.Books.ToListAsync();
                var members = await context.Members.ToListAsync();
                var faker = new Faker();

                for (int i = 0; i < 15; i++)
                {
                    var book = faker.PickRandom(books);
                    var member = faker.PickRandom(members);
                    var loanDate = faker.Date.Past(1);
                    var dueDate = loanDate.AddDays(14);
                    var returnedDate = faker.Random.Bool(0.7f) ? (DateTime?)faker.Date.Between(loanDate, DateTime.Now) : null;

                    var loan = new Loan
                    {
                        BookId = book.Id,
                        MemberId = member.Id,
                        LoanDate = loanDate,
                        DueDate = dueDate,
                        ReturnedDate = returnedDate
                    };

                    // Update book availability if loan is active
                    if (returnedDate == null)
                    {
                        book.IsAvailable = false;
                    }

                    context.Loans.Add(loan);
                }
                await context.SaveChangesAsync();
            }
        }
    }
}
