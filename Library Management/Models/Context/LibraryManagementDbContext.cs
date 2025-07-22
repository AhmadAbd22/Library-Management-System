using Library_Management.Controllers;
using Library_Management.Helping_Classes;
using Library_Management.HelpingClasses;
using Microsoft.EntityFrameworkCore;

namespace Library_Management.Models.Context
{
    public class LibraryManagementDbContext : DbContext
    {
        public LibraryManagementDbContext(DbContextOptions<LibraryManagementDbContext> options)
        : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasData(
                new User()
                {
                    Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    firstName = "Admin",
                    lastName = "Bhai",
                    phoneNumber = "12341234567",
                    email = "admin1@gmail.com",
                    password = PasswordHelper.HashPassword("123"),
                    role = (int)enumRole.Admin,
                    isActive = (int)enumActiveStatus.Active,
                    createdAt = new DateTime(2024, 01, 01),
                }
                
            );
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<BorrowedBooks> BorrowedBooks { get; set; }
    }
}
