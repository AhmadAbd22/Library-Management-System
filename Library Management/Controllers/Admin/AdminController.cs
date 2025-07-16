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

        // GET: Add Book Form
        public IActionResult AddBook()
        {
            return View();
        }

        // POST: Add Book
        [HttpPost]
        public async Task<IActionResult> AddBook(BookDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var book = new Book
            {
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

        // GET: List Active Books
        public async Task<IActionResult> BookList()
        {
            var books = await _bookRepo.GetActiveBookList();
            return View(books);
        }

        // GET: Edit Book by Id
        [HttpGet]
        public async Task<IActionResult> EditBook(Guid id)
        {
            var book = await _bookRepo.GetBookById(id);
            if (book == null)
                return NotFound();

            var dto = new BookDto
            {
                title = book.title,
                author = book.author,
                ISBN = book.ISBN,
                totalQuantity = book.quantity
            };

            ViewBag.BookId = id; // For sending id to the view
            return View(dto);
        }

        // POST: Edit Book
        [HttpPost]
        public async Task<IActionResult> EditBook(Guid id, BookDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var book = await _bookRepo.GetBookById(id);
            if (book == null)
                return NotFound();

            book.title = dto.title;
            book.author = dto.author;
            book.quantity = dto.totalQuantity;
            book.ISBN = dto.ISBN;

            var result = await _bookRepo.UpdateBook(book);
            if (result)
                return RedirectToAction("BookList");
            else
                return View(dto);
        }

        // POST: Delete Book
        [HttpPost]
        public async Task<IActionResult> DeleteBook(Guid id)
        {
            var result = await _bookRepo.DeleteBook(id);

            if (result)
                return RedirectToAction("BookList");

            TempData["Error"] = "Could not delete book.";
            return RedirectToAction("BookList");
        }

        // GET: Filter Book List
        [HttpGet]
        public async Task<IActionResult> FilterBooks(string search)
        {
            var filteredBooks = await _bookRepo.GetActiveBookListBySearch(search);
            return View("BookList", filteredBooks);
        }
    }
}
