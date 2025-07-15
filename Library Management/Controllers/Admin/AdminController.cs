using Library_Management.Controllers.Auth;
using Library_Management.Models;
using Library_Management.Models.Context;
using Library_Management.Models.Dtos;
using Library_Management.Models.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Library_Management.Controllers
{
    public class AdminController : Controller
    {
        private readonly IBookRepo _bookRepo;

        public AdminController(IBookRepo bookRepo)
        {
            _bookRepo = bookRepo;
        }
        public IActionResult AddBook()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddBook(BookDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var book = new Book
            {
                Id = Guid.NewGuid(),
                title = dto.title,
                author = dto.author,
                quantity = dto.totalQuantity,
                ISBN = dto.ISBN,
                isActive = (int)enumActiveStatus.Active,
                createdAt = DateTime.Now
            };

            var result = await _bookRepo.AddBook(book);

            if (result)
                return RedirectToAction("BookList");
            else
                return View(dto);
        }

        // List Books
        public async Task<IActionResult> BookList()
        {
            var books = await _bookRepo.GetActiveBooks();
            return View(books);
        }
    }
}
