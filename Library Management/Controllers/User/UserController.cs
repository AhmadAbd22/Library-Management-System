using Library_Management.Models.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Library_Management.Controllers.User
{
    public class BorrowController : Controller
    {
        private readonly IBorrowRepo _borrowRepo;

        public BorrowController(IBorrowRepo borrowRepo)
        {
            _borrowRepo = borrowRepo;
        }

        // POST: Borrow a Book
        [HttpPost]
        public async Task<IActionResult> Borrow(Guid bookId, Guid userId)
        {
            var success = await _borrowRepo.BorrowedBooks(userId, bookId);

            if (success)
                return RedirectToAction("MyBorrows", new { userId });
            else
                return RedirectToAction("Error", "Home", new { msg = "Borrowing failed." });
        }

        // GET: List of books borrowed by user
        [HttpGet]
        public async Task<IActionResult> MyBorrows(Guid userId)
        {
            var borrows = await _borrowRepo.GetUserBorrows(userId);
            return View(borrows);
        }

        // POST: Return a Book
        [HttpPost]
        public async Task<IActionResult> Return(Guid borrowId, Guid userId)
        {
            var result = await _borrowRepo.ReturnBook(borrowId);

            if (result)
                return RedirectToAction("MyBorrows", new { userId });
            else
                return RedirectToAction("Error", "Home", new { msg = "Return failed." });
        }
    }
}