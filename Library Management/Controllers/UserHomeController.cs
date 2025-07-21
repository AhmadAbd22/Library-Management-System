using Library_Management.Models;
using Library_Management.Models.Context;
using Library_Management.Models.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Library_Management.Controllers
{
    [Authorize(Roles = "Customer")]
    public class UserHomeController : Controller
    {
        private readonly IUserRepo _userRepo;
        private readonly IBorrowRepo _borrowRepo;
        private readonly IBookRepo _bookRepo;
        private readonly LibraryManagementDbContext _context;

        public UserHomeController(IUserRepo userRepo, IBorrowRepo borrowRepo, IBookRepo bookRepo, LibraryManagementDbContext context)
        {
            _userRepo = userRepo;
            _borrowRepo = borrowRepo;
            _bookRepo = bookRepo;
            _context = context;
        }
        public async Task<IActionResult> UserHome(string search)
        {
            List<Book> books;

            if (!string.IsNullOrWhiteSpace(search))
            {
                books = (await _bookRepo.GetActiveBookListBySearch(search)).ToList();
            }
            else
            {
                books = (await _bookRepo.GetActiveBookList()).ToList();
            }

            return View(books); 
        }

        [HttpPost]
        public async Task<IActionResult> BorrowBook(Guid bookId)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdClaim, out Guid userId))
            {
                TempData["Error"] = "User is not logged in.";
                return RedirectToAction("Login", "Login");
            }
            var alreadyBorrowed = _borrowRepo.GetUserBorrows(userId);
            {
                if (userId == null)
                {
                    var borrows = await _borrowRepo.GetUserBorrows(userId);
                    return View(borrows);
                }
            }
            TempData["BorrowedBefore"] = "Book is already borrowed by the user";
            return RedirectToAction("UserHome", "UserHome");
        }

        [HttpGet]
        public async Task<IActionResult> BorrowedBooks()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdClaim, out Guid userId))
            {
                return RedirectToAction("UserHome", "UserHome");
            }

            var borrows = await _borrowRepo.GetUserBorrows(userId);
            return View(borrows);
        }

        [HttpPost]
        public async Task<IActionResult> ReturnBook(Guid borrowId)
        {
            var borrow = await _context.BorrowedBooks.FindAsync(borrowId);

            if (borrow == null)
            {
                TempData["Error"] = "Borrow record not found.";
                return RedirectToAction("BorrowedBooks");
            }

            if (borrow.IsReturned)
            {
                TempData["Error"] = "This book has already been returned.";
                return RedirectToAction("BorrowedBooks");
            }

            borrow.IsReturned = true;
            borrow.returnDate = DateTime.UtcNow;

            _context.BorrowedBooks.Update(borrow);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Book returned successfully!";
            return View(borrow);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Logout", "Login", new { msg = "Logged out successfully." });
        }
    }
}
