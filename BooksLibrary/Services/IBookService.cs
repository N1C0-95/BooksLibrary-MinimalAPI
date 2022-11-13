using MinimalApi.Models;

namespace MinimalApi.Services
{
    public interface IBookService
    {
        public Task<bool> CreateAsync(Book book);
        public Task<Book?> GetByIsbnAsync(string isbn);
        public Task<IEnumerable<Book>> GetAllBookAsync();
        public Task<IEnumerable<Book>> SearchByTitleAsync(string searchTerm);
        public Task<bool> UpadateAsync(Book book);
        public Task<bool> DeleteAsync(string isbn);
    }
}
