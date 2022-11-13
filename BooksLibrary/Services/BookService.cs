using MinimalApi.Data;
using MinimalApi.Models;
using Dapper;

namespace MinimalApi.Services
{
    public class BookService : IBookService
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public BookService(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }
        public async Task<bool> CreateAsync(Book book)
        {
            var existingBook = await GetByIsbnAsync(book.Isbn);
            if (existingBook is not null)
            {
                return false;
            }
            using var connection = await _connectionFactory.CreateConnectionAsync();
            var result = await connection.ExecuteAsync(@"
                INSERT INTO Books(Isbn, Title, Author, ShortDescription, PageCounter, ReleasedDate )
                VALUES (@Isbn, @Title, @Author, @ShortDescription, @PageCounter, @ReleasedDate )        
            ",book);
            return result > 0;
        }
        public async Task<bool> DeleteAsync(string isbn)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            var result = await connection.ExecuteAsync(@"DELETE FROM Books WHERE Isbn = @Isbn", new { Isbn = isbn });
            return result > 0;
        }
        public async Task<IEnumerable<Book>> GetAllBookAsync()
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            var result = await connection.QueryAsync<Book>(@"SELECT * FROM Books");
            return result;
        }
        public async Task<Book?> GetByIsbnAsync(string isbn)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            var result = await connection.QueryFirstOrDefaultAsync<Book>(@"SELECT * FROM Books WHERE Isbn = @Isbn LIMIT 1",new {Isbn = isbn});
            return result;
         
        }
        public async Task<IEnumerable<Book>> SearchByTitleAsync(string searchTerm)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            return await connection.QueryAsync<Book>("SELECT * FROM Books WHERE Title LIKE '%' || @SearchTerm || '%'", new { SearchTerm = searchTerm });
        }
        public async Task<bool> UpadateAsync(Book book)
        {
            
            var existingBook = await GetByIsbnAsync(book.Isbn); 
            if(existingBook is null)
            {
                return false;   
            }
            using var connection = await _connectionFactory.CreateConnectionAsync();
            var result = await connection.ExecuteAsync(@"
                UPDATE Books SET Title=@Title, Author=@Author, ShortDescription = @ShortDescription, PageCounter=@PageCounter, ReleasedDate = @ReleasedDate WHERE Isbn = @Isbn", book);

            return result > 0;
        }
    }
}
