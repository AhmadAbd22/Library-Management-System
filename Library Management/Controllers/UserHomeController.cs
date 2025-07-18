using Library_Management.Models;
using Library_Management.Models.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Library_Management.Controllers
{
    [Authorize(Roles = "Customer")]
    public class UserHomeController : Controller
    {
        private readonly IUserRepo _userRepo;
        private readonly IBorrowRepo _borrowRepo;
        private readonly IBookRepo _bookRepo;

        public UserHomeController(IUserRepo userRepo, IBorrowRepo borrowRepo, IBookRepo bookRepo)
        {
            _userRepo = userRepo;
            _borrowRepo = borrowRepo;
            _bookRepo = bookRepo;
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

            var result = await _borrowRepo.BorrowedBooks(userId, bookId);
            TempData[result ? "Success" : "Error"] = result ? "Book borrowed successfully." : "Failed to borrow book.";

            return RedirectToAction("UserHome");
        }

        [HttpGet]
        public async Task<IActionResult> BorrowedBooks()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdClaim, out Guid userId))
            {
                return RedirectToAction("Login", "Login");
            }

            var borrows = await _borrowRepo.GetUserBorrows(userId);
            return View(borrows);
        }



        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Login", "Login", new { msg = "Logged out successfully." });
        }
    }
}
