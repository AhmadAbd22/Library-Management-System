using Library_Management.Models.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Library_Management.Controllers
{
    public class BookController : Controller
    {
        public class HomeController : Controller
        {
            private readonly IBookRepo _bookRepo;
            private readonly IBorrowRepo _borrowRepo;

            public HomeController(IBookRepo bookRepo, IBorrowRepo borrowRepo)
            {
                _bookRepo = bookRepo;
                _borrowRepo = borrowRepo;
            }

            [HttpGet]
            public IActionResult Index()
            {
                return View();
            }

            // GET: Show all active books
            public async Task<IActionResult> ShowBooks()
            {
                var books = await _bookRepo.GetActiveBookList();
                return View(books);
            }

            // GET: Search active books
            [HttpGet]
            public async Task<IActionResult> Search(string search)
            {
                var filteredBooks = await _bookRepo.GetActiveBookListBySearch(search);
                return View("Index", filteredBooks);
            }

            // POST: Borrow a book
            [HttpPost]
            public async Task<IActionResult> Borrow(Guid bookId)
            {
                var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var borrowedBooks = await _borrowRepo.GetUserBorrows(userId);
                var alreadyBorrowed = borrowedBooks.Any(b => b.bookId == bookId && !b.IsReturned);

                if (alreadyBorrowed)
                {
                    TempData["Error"] = "You have already borrowed this book and not returned it.";
                    return RedirectToAction("Index");
                }

                var success = await _borrowRepo.BorrowedBooks(userId, bookId);

                if (success)
                {
                    TempData["Message"] = "Book borrowed successfully!";
                }
                else
                {
                    TempData["Error"] = "Failed to borrow the book. Please try again.";
                }

                return RedirectToAction("UserHome", "UserHome");
            }

            // GET: My Borrowed Books
            public async Task<IActionResult> MyBorrows()
            {
                var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var borrows = await _borrowRepo.GetUserBorrows(userId);
                return View(borrows);
            }


        }
    }

}

    