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
        Task<bool> DeleteBook (Guid id);
    }

    public interface IBorrowRepo
    {
        Task<bool> BorrowedBooks(Guid userId, Guid bookId);
        Task<bool> ReturnBook(Guid borrowId);
        Task<IEnumerable<BorrowedBooks>> GetUserBorrows(Guid userId);
        Task<IEnumerable<BorrowedBooks>> GetAllBorrows();
        Task<int> GetTotalBorrowedCount();

    }

    public class BorrowRepo : IBorrowRepo
    {
        private readonly LibraryManagementDbContext _context;

        public BorrowRepo(LibraryManagementDbContext context)
        {
            _context = context;
        }

        public async Task<bool> BorrowedBooks(Guid userId, Guid bookId)
        {
            try
            {
                var book = await _context.Books.FirstOrDefaultAsync(x => x.Id == bookId && x.isActive == (int)enumActiveStatus.Active);
                if (book == null || book.quantity <= 0)
                    return false;

                var borrow = new BorrowedBooks
                {
                    Id = Guid.NewGuid(),
                    bookId = bookId,
                    userId = userId,
                    rentDate = DateTime.UtcNow,
                    rentPrice = 0, 
                    isActive = (int)enumActiveStatus.Active,
                    createdAt = DateTime.UtcNow
                };

                book.quantity--; 

                _context.BorrowedBooks.Add(borrow);
                _context.Books.Update(book);
                await _context.SaveChangesAsync();
                await _context.SaveChangesAsync();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> ReturnBook(Guid borrowId)
        {
            try
            {
                var borrow = await _context.BorrowedBooks.FirstOrDefaultAsync(x => x.Id == borrowId);
                if (borrow == null)
                    return false;

                borrow.returnDate = DateTime.UtcNow;
                borrow.isActive = (int)enumActiveStatus.Pending;

                var book = await _context.Books.FirstOrDefaultAsync(x => x.Id == borrow.bookId);
                if (book != null)
                {
                    book.quantity++;
                    _context.Books.Update(book);
                }

                _context.BorrowedBooks.Update(borrow);
                await _context.SaveChangesAsync();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<IEnumerable<BorrowedBooks>> GetUserBorrows(Guid userId)
        {
            return await _context.BorrowedBooks
                .Include(x => x.book)
                .Where(x => x.userId == userId && x.isActive == (int)enumActiveStatus.Active)
                .ToListAsync();
        }

        public async Task<IEnumerable<BorrowedBooks>> GetAllBorrows()
        {
            return await _context.BorrowedBooks
                .Include(x => x.book)
                .Where(x => x.isActive == (int)enumActiveStatus.Active)
                .ToListAsync();
        }

        public async Task<int> GetTotalBorrowedCount()
        {
            return await _context.BorrowedBooks
                .Where(x => x.isActive == (int)enumActiveStatus.Active)
                .CountAsync();
        }
    }
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

        public async Task<IEnumerable<Book>> GetActiveBooks()
        {
            return await _context.Books
                .Where(b => b.isActive == (int)enumActiveStatus.Active)
                .OrderByDescending(b => b.createdAt)
                .ToListAsync();
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
                    return await Task.FromResult(false);
                book.Id = Guid.NewGuid();
                book.isActive = (int)enumActiveStatus.Active;
                book.createdAt = DateTime.UtcNow;
                _context.Books.Add(book);
                _context.SaveChanges();
                return await Task.FromResult(true);
            }
            catch
            {
                return await Task.FromResult(false);
            }
        }

        public async Task<bool> DeleteBook(string title)
        {
            try
            {
                var book = await _context.Books
                    .FirstOrDefaultAsync(b => b.title.ToLower().Trim() == title.ToLower().Trim()
                                           && b.isActive == (int)enumActiveStatus.Active);

                if (book == null)
                    return false;

                book.isActive = (int)enumActiveStatus.Deleted;
                book.updatedAt = DateTime.UtcNow;

                _context.Books.Update(book);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public Task<bool> DeleteBook(Guid id)
        {
            try
            {
                var book = _context.Books.FirstOrDefault(b => b.Id == id && b.isActive == (int)enumActiveStatus.Active);
                if (book == null)
                    return Task.FromResult(false);
                book.isActive = (int)enumActiveStatus.Deleted;
                book.updatedAt = DateTime.UtcNow;
                _context.Books.Update(book);
                _context.SaveChanges();
                return Task.FromResult(true);
            }
            catch
            {
                return Task.FromResult(false);
            }
        }

    public async Task<IEnumerable<Book>> GetActiveBookList()
    {
        return await _context.Books.Where(b => b.isActive == (int)enumActiveStatus.Active).ToListAsync();
    }

    public async Task<IEnumerable<Book>> GetActiveBookListBySearch(string searchValue)
    {
        searchValue = searchValue.Trim().ToLower();
        var result = await _context.Books.Where(b => b.isActive == (int)enumActiveStatus.Active && b.title.ToLower().Contains(searchValue)).ToListAsync();
        return result;
    }

        public Task<string?> GetActiveBooks()
        {
        return (Task<string?>)_context.Books.Where(b => b.isActive == (int)enumActiveStatus.Active);
        }

        public async Task<Book?> GetBookById(Guid id)
        {
            try
            {
                return await _context.Books.FirstOrDefaultAsync(b => b.Id == id && b.isActive == (int)enumActiveStatus.Active);
            }
            catch
            {
                return await Task.FromResult<Book?>(null);
            }
        }

    public async Task<bool> UpdateBook(Book book)
    {
        if (book == null || book.Id == Guid.Empty)
            return false;

        var existingBook = await _context.Books.FirstOrDefaultAsync(b => b.Id == book.Id && b.isActive == (int)enumActiveStatus.Active);
        if (existingBook == null)
            return false;

        existingBook.title = book.title;
        existingBook.author = book.author;
        existingBook.ISBN = book.ISBN;
        existingBook.quantity = book.quantity;
        existingBook.rentPrice = book.rentPrice;
        existingBook.updatedAt = DateTime.UtcNow;

        try
        {
            _context.Books.Update(existingBook);
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

}

