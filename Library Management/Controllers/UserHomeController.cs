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

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (Guid.TryParse(userIdClaim, out Guid userId))
            {
                var borrows = await _borrowRepo.GetUserBorrows(userId);
                var borrowedIds = borrows.Where(b => !b.IsReturned).Select(b => b.bookId).ToList();
                ViewBag.BorrowedBookIds = borrowedIds;
            }

            return View(books);
        }

        [HttpPost]
        public async Task<IActionResult> BorrowBook(Guid bookId, DateTime returnDate)
        {
            if (returnDate > DateTime.Now.AddDays(20))
            {
                TempData["Error"] = "Return date must be within 20 days.";
                return RedirectToAction("UserHome"); 
            }

            if (returnDate <= DateTime.Now)
            {
                TempData["Error"] = "Return date must be in the future.";
                return RedirectToAction("UserHome");
            }

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdClaim, out Guid userId))
            {
                TempData["Error"] = "User not found.";
                return RedirectToAction("UserHome");
            }

            var success = await _borrowRepo.BorrowedBooks(userId, bookId, returnDate);

            if (success)
            {
                TempData["Success"] = "Book borrowed successfully!"; 
            }
            else
            {
                TempData["Error"] = "Could not borrow book. It might be out of stock or already borrowed.";
            }

            return RedirectToAction("UserHome");
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
            var result = await _borrowRepo.ReturnBook(borrowId);

            if (!result)
            {
                TempData["Error"] = "Failed to return the book.";
            }
            else
            {
                TempData["Success"] = "Book returned successfully!";
            }

            return RedirectToAction("BorrowHistory", "UserHome");
        }

        [HttpGet]
        public async Task<IActionResult> BorrowHistory()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdClaim, out Guid userId))
            {
                return RedirectToAction("UserHome", "UserHome");
            }

            var history = await _borrowRepo.GetUserBorrowHistory(userId);
            return View(history); 
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Logout", "Login", new { msg = "Logged out successfully." });
        }
    }
}