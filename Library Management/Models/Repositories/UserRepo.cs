using Library_Management.Models;
using Library_Management.Models.Context;
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
        Task<bool> DeleteBook
            (Guid id);
        Task<string?> GetActiveBooks();
    }

    public interface IBorrowRepo
    {
        Task<bool> BorrowedBooks(Guid userId, Guid bookId);
        Task<bool> ReturnBook(Guid borrowId);
        Task<IEnumerable<BorrowedBooks>> GetUserBorrows(Guid userId);
        Task<IEnumerable<BorrowedBooks>> GetAllBorrows(); // for admin
        Task<int> GetTotalBorrowedCount(); // for dashboard
    }


    public class UserRepo : IUserRepo
    {
        private readonly LibraryManagementDbContext context;

        public async Task<bool> AddUser(User user)
        {
            try
            {
                context.Users.Add(user);
                await context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
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
            return await context.Books
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
            return await context.Users.FirstOrDefaultAsync(x => x.email!.ToLower().Equals(email.Trim(), StringComparison.CurrentCultureIgnoreCase) && x.isActive == (int)enumActiveStatus.Active);
        }

        public Task<User?> GetUserById(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<User?> GetUserByLogin(string email)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsUserValidate(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateUser(User user)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ValidateEmail(string email, Guid id = default)
        {
            throw new NotImplementedException();
        }
    }


    //BOOK

    public class BookRepo : IBookRepo
    {
        private readonly LibraryManagementDbContext context;

        public Task<bool> AddBook(Book book)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> DeleteBook(string title)
        {
            try
            {
                var book = await context.Books
                    .FirstOrDefaultAsync(b => b.title.ToLower().Trim() == title.ToLower().Trim()
                                           && b.isActive == (int)enumActiveStatus.Active);

                if (book == null)
                    return false;

                book.isActive = (int)enumActiveStatus.Deleted;
                book.updatedAt = DateTime.UtcNow;

                context.Books.Update(book);
                await context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public Task<bool> DeleteBook(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Book>> GetActiveBookList()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Book>> GetActiveBookListBySearch(string searchValue)
        {
            throw new NotImplementedException();
        }

        public Task<string?> GetActiveBooks()
        {
            throw new NotImplementedException();
        }

        public Task<Book?> GetBookById(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateBook(Book book)
        {
            throw new NotImplementedException();
        }
    }
}