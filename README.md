 📚 Community Library Desk System

  This project is an internal management application for a small community library, developed for the Modern Programming
  Principles and Practice (Assessment #1).

  🚀 Core Features

  The application manages three main entities: Books, Members, and Loans, with a focus on business rules and data
  integrity.

  1. Books Management
   * Full CRUD: List, create, edit, and delete books.
   * Advanced Search & Filtering:
       * Search by Title or Author (contains logic).
       * Filter by Category (dynamic dropdown).
       * Filter by Availability (All / Available / On Loan).
       * Implementation: All filtering is performed server-side using EF Core query composition (IQueryable) to ensure
         performance.

  2. Members Management
   * Full CRUD: Manage library members' information (Full Name, Email, and Phone).

  3. Loans Workflow & Business Rules
   * Lending Process:
       * Choose a member and select from a list of only available books.
       * Automated LoanDate and DueDate (14-day period).
   * Validation Rules:
       * Prevent Duplicate Loans: The system prevents lending a book that is already on an active loan (where
         ReturnedDate is null).
       * Mark as Returned: Updates the ReturnedDate and automatically marks the book as Available again.
   * Visual Indicators: Color-coded badges for "Active", "Returned", and "Overdue" statuses.

  4. Admin & Role Management
   * Secure Access: The /Admin/Roles page is restricted to users with the "Admin" role using server-side authorization.
   * Role UI: List existing roles, create new ones via a dedicated form, and delete non-system roles.

  ---

  🛠️ Technical Stack

   * Framework: ASP.NET Core 10.0 (MVC)
   * Database: Entity Framework Core with SQL Server (LocalDB)
   * Identity: ASP.NET Core Identity for authentication and role-based access.
   * Fake Data: Bogus library for automated database seeding.
   * Testing: xUnit for unit testing business logic.
   * CI/CD: GitHub Actions workflow (Build & Test on push/PR).

  ---

  ⚙️ Setup and Installation

  Prerequisites
   * Visual Studio 2022 (latest version recommended)
   * .NET 10.0 SDK
   * SQL Server Express / LocalDB (included with Visual Studio)

  Getting Started
   1. Open the solution file (epita-s1-1-mvc-83303.slnx or the root folder) in Visual Studio.
   2. Open the Package Manager Console and run the following command to create the database:
   1     Update-Database
   3. Press F5 to run the application.

  Default Admin Credentials
  The database is automatically seeded with an admin account during the first run:
   * Email: admin@library.com
   * Password: Admin123!

  ---

  🧪 Testing & Data Quality

  Automated Seeding (Bogus)
  Upon the first run, the database is populated with realistic data:
   * 20 Books with various categories.
   * 10 Members.
   * 15 Loans (including active, returned, and overdue scenarios).

  Unit Tests
  The project includes 6 xUnit tests covering critical scenarios:
   * Book availability logic after return.
   * Detection of overdue loans.
   * Search result validation.
   * Business rules for active loans.

  To run tests: Test > Run All Tests in Visual Studio.

  ---

  📈 Marking Criteria Alignment (Checklist)


  ┌─────────────────────┬────────┬────────────────────────────────────────────────────────────────────────────────┐
  │ Criterion           │ Status │ Description                                                                    │
  ├─────────────────────┼────────┼────────────────────────────────────────────────────────────────────────────────┤
  │ EF Core Model       │ ✅     │ 3 entities with proper 1-* relationships and navigation properties.            │
  │ Fake Data Seeding   │ ✅     │ Used Bogus to generate 20 books, 10 members, and 15 loans.                     │
  │ Books Search/Filter │ ✅     │ Implemented Title/Author search and Category/Availability filters on one page. │
  │ Loan Workflow       │ ✅     │ Rules enforced: prevent duplicate active loans, return functionality.          │
  │ Role Management     │ ✅     │ Admin-only page created for Role listing and creation.                         │
  │ CI Workflow         │ ✅     │ .github/workflows/ci.yml correctly configured for GitHub Actions.              │
  └─────────────────────┴────────┴────────────────────────────────────────────────────────────────────────────────┘


  ---
  Developed for Modern Programming Principles and Practice - Assessment #1 (EPITA / Dorset College).
