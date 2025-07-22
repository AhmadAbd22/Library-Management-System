using Library_Management.Models;
using Library_Management.Models.Context;
using Library_Management.Models.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Library_Management.Models.Repositories
{
    public interface IUserRepo
    {
        Task<User?> GetUserByLogin(string email);
        Task<User?> GetUserById(Guid id);
        Task<bool> IsUserValidate(Guid id);
        Task<int> GetActiveUserCount();
        Task<(int ActiveUserCount, int NonActiveUserCount)> GetActive_NonActiveUserCount();
        Task<IEnumerable<User>> GetActiveUserList();
        Task<IEnumerable<User>> GetActiveUserListBySearch(string searchValue);
        Task<bool> AddUser(User user);
        Task<bool> UpdateUser(User user);
        Task<bool> DeleteUser(Guid id);
        Task<bool> ValidateEmail(string email, Guid id = default(Guid));
        Task<User?> GetUserByEmail(string email);
    }

    public interface IBookRepo
    {
        Task<IEnumerable<Book>> GetActiveBookList();
        Task<IEnumerable<Book>> GetActiveBookListBySearch(string searchValue);
        Task<bool> AddBook(Book book);
        Task<Book?> GetBookById(Guid id);
        Task<bool> UpdateBook(Book book);
        Task<bool> DeleteBook(Guid id);
        Task<bool> IsBookBorrowed(Guid id); // Changed to async Task<bool>
    }

    public interface IBorrowRepo
    {
        Task<bool> BorrowedBooks(Guid userId, Guid bookId, DateTime returnDate);
        Task<bool> ReturnBook(Guid borrowId);
        Task<IEnumerable<BorrowedBooks>> GetUserBorrows(Guid userId);
        Task<IEnumerable<BorrowedBooks>> GetAllBorrows();
        Task<int> GetTotalBorrowedCount();
        Task<IEnumerable<BorrowedBooks>> GetUserBorrowHistory(Guid userId);
        Task<bool> IsBookBorrowed(Guid id);
    }

    //USER
    public class UserRepo : IUserRepo
    {
        private readonly LibraryManagementDbContext _context;
        public UserRepo(LibraryManagementDbContext context)
        {
            _context = context;
        }
        public async Task<bool> AddUser(User user)
        {
            try
            {
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding user: {ex.Message}");
                throw;
            }
        }

        public Task<bool> DeleteUser(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<int> GetActiveUserCount()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<User>> GetActiveUserList()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<User>> GetActiveUserListBySearch(string searchValue)
        {
            throw new NotImplementedException();
        }

        public Task<(int ActiveUserCount, int NonActiveUserCount)> GetActive_NonActiveUserCount()
        {
            throw new NotImplementedException();
        }

        public async Task<User?> GetUserByEmail(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(
                x => x.email!.ToLower() == email.Trim().ToLower()
                  && x.isActive == (int)enumActiveStatus.Active);
        }

        public async Task<User?> GetUserById(Guid id)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.Id == id && x.isActive == (int)enumActiveStatus.Active);
        }

        public async Task<User?> GetUserByLogin(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.email == email && u.isActive == (int)enumActiveStatus.Active);
        }

        public Task<bool> IsUserValidate(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> UpdateUser(User user)
        {
            if (user == null || user.Id == Guid.Empty)
                return false;

            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == user.Id && u.isActive == (int)enumActiveStatus.Active);
            if (existingUser == null)
                return false;

            existingUser.firstName = user.firstName;
            existingUser.lastName = user.lastName;
            existingUser.email = user.email;
            existingUser.password = user.password;
            existingUser.phoneNumber = user.phoneNumber;
            existingUser.city = user.city;
            existingUser.address = user.address;
            existingUser.role = user.role;
            existingUser.updatedAt = DateTime.UtcNow;

            try
            {
                _context.Users.Update(existingUser);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public Task<bool> ValidateEmail(string email, Guid id = default)
        {
            throw new NotImplementedException();
        }
    }

    //BORROW
    public class BorrowRepo : IBorrowRepo
    {
        private readonly LibraryManagementDbContext _context;

        public BorrowRepo(LibraryManagementDbContext context)
        {
            _context = context;
        }

        public async Task<bool> BorrowedBooks(Guid userId, Guid bookId, DateTime returnDate)
        {
            // Use a transaction to ensure data consistency
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var book = await _context.Books.FindAsync(bookId);
                if (book == null || book.quantity <= 0)
                    return false;

                bool alreadyBorrowed = await _context.BorrowedBooks
                    .AnyAsync(x => x.userId == userId && x.bookId == bookId && x.IsReturned == false);

                if (alreadyBorrowed)
                    return false;

                var borrow = new BorrowedBooks
                {
                    Id = Guid.NewGuid(),
                    userId = userId,
                    bookId = bookId,
                    rentDate = DateTime.Now,
                    returnDate = returnDate,
                    IsReturned = false,
                    isActive = (int)enumActiveStatus.Active,
                    createdAt = DateTime.UtcNow
                };

                book.quantity--;
                book.updatedAt = DateTime.UtcNow;

                _context.BorrowedBooks.Add(borrow);
                _context.Books.Update(book); // Explicitly update the book

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine($"Error in BorrowedBooks: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> ReturnBook(Guid borrowId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var borrowRecord = await _context.BorrowedBooks.FindAsync(borrowId);
                if (borrowRecord == null || borrowRecord.IsReturned)
                    return false;

                borrowRecord.IsReturned = true;
                borrowRecord.returnDate = DateTime.Now;
                borrowRecord.updatedAt = DateTime.UtcNow;

                var book = await _context.Books.FindAsync(borrowRecord.bookId);
                if (book != null)
                {
                    book.quantity += 1;
                    book.updatedAt = DateTime.UtcNow;
                    _context.Books.Update(book);
                }

                _context.BorrowedBooks.Update(borrowRecord);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine($"Error in ReturnBook: {ex.Message}");
                return false;
            }
        }

        public async Task<IEnumerable<BorrowedBooks>> GetUserBorrows(Guid userId)
        {
            return await _context.BorrowedBooks
                .Include(x => x.book)
                .Where(x => x.userId == userId && x.isActive == (int)enumActiveStatus.Active && !x.IsReturned)
                .OrderByDescending(x => x.createdAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<BorrowedBooks>> GetAllBorrows()
        {
            return await _context.BorrowedBooks
                .Include(x => x.book)
                .Where(x => x.isActive == (int)enumActiveStatus.Active)
                .OrderByDescending(x => x.createdAt)
                .ToListAsync();
        }

        public async Task<int> GetTotalBorrowedCount()
        {
            return await _context.BorrowedBooks
                .Where(x => x.isActive == (int)enumActiveStatus.Active && !x.IsReturned)
                .CountAsync();
        }

        public async Task<IEnumerable<BorrowedBooks>> GetUserBorrowHistory(Guid userId)
        {
            return await _context.BorrowedBooks
                .Include(x => x.book)
                .Where(x => x.userId == userId)
                .OrderByDescending(x => x.createdAt)
                .ToListAsync();
        }

        public async Task<bool> IsBookBorrowed(Guid id)
        {
            return await _context.BorrowedBooks
                .AnyAsync(bb => bb.bookId == id && bb.isActive == (int)enumActiveStatus.Active && !bb.IsReturned);
        }
    }

    //BOOK
    public class BookRepo : IBookRepo
    {
        private readonly LibraryManagementDbContext _context;

        public BookRepo(LibraryManagementDbContext context)
        {
            _context = context;
        }

        public async Task<bool> AddBook(Book book)
        {
            try
            {
                if (book == null)
                    return false;

                book.Id = Guid.NewGuid();
                book.isActive = (int)enumActiveStatus.Active;
                book.createdAt = DateTime.UtcNow;

                _context.Books.Add(book);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding book: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteBook(Guid id)
        {
            try
            {
                // Check if book is currently borrowed
                if (await IsBookBorrowed(id))
                    return false;

                var book = await _context.Books.FirstOrDefaultAsync(b => b.Id == id && b.isActive == (int)enumActiveStatus.Active);
                if (book == null)
                    return false;

                book.isActive = (int)enumActiveStatus.Deleted;
                book.updatedAt = DateTime.UtcNow;

                _context.Books.Update(book);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting book: {ex.Message}");
                return false;
            }
        }

        public async Task<IEnumerable<Book>> GetActiveBookList()
        {
            return await _context.Books
                .Where(b => b.isActive == (int)enumActiveStatus.Active)
                .OrderByDescending(b => b.createdAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Book>> GetActiveBookListBySearch(string searchValue)
        {
            if (string.IsNullOrWhiteSpace(searchValue))
                return await GetActiveBookList();

            searchValue = searchValue.Trim().ToLower();
            return await _context.Books
                .Where(b => b.isActive == (int)enumActiveStatus.Active &&
                           (b.title.ToLower().Contains(searchValue) ||
                            b.author.ToLower().Contains(searchValue)))
                .OrderByDescending(b => b.createdAt)
                .ToListAsync();
        }

        public async Task<Book?> GetBookById(Guid id)
        {
            try
            {
                return await _context.Books
                    .FirstOrDefaultAsync(b => b.Id == id && b.isActive == (int)enumActiveStatus.Active);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting book by id: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> UpdateBook(Book book)
        {
            if (book == null || book.Id == Guid.Empty)
                return false;

            try
            {
                var existingBook = await _context.Books
                    .FirstOrDefaultAsync(b => b.Id == book.Id && b.isActive == (int)enumActiveStatus.Active);

                if (existingBook == null)
                    return false;

                existingBook.title = book.title;
                existingBook.author = book.author;
                existingBook.ISBN = book.ISBN;
                existingBook.quantity = book.quantity;
                existingBook.rentPrice = book.rentPrice;
                existingBook.updatedAt = DateTime.UtcNow;

                _context.Books.Update(existingBook);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating book: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> IsBookBorrowed(Guid id)
        {
            return await _context.BorrowedBooks
                .AnyAsync(bb => bb.bookId == id &&
                               bb.isActive == (int)enumActiveStatus.Active &&
                               !bb.IsReturned);
        }
    }
}