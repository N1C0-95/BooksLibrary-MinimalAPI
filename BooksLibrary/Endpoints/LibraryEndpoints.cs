using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using MinimalApi.Auth;
using MinimalApi.Models;
using MinimalApi.Services;

namespace BooksLibrary.Endpoints
{
    public static class LibraryEndpoints
    {
        public static void AddLibraryEndpoint(this IServiceCollection services)
        {
            services.AddScoped<IBookService, BookService>();
        }
        public static void UseLibraryEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapPost("books", CreateBook).WithName("CreateBook").Accepts<Book>("application/json").Produces<Book>(201).Produces<IEnumerable<ValidationFailure>>(400).WithTags("Books");

            app.MapGet("books", GetAllBooks).WithName("GetBooks").Produces<IEnumerable<Book>>(200).WithTags("Books");
            app.MapGet("books/{isbn}", GetBooksByIsbn).WithName("GetBookByIsbn").Produces<Book>(200).Produces(404).WithTags("Books");

            app.MapPut("books/{isbn}", UpdateBook).WithName("UpdateName").Accepts<Book>("application/json").Produces<Book>(201).Produces<IEnumerable<ValidationFailure>>(400).WithTags("Books");

            app.MapDelete("books/{isbn}", DeleteBook).WithName("DeleteBook").Produces(204).Produces(404).WithTags("Books");
        }

        [Authorize(AuthenticationSchemes = ApiKeySchemeConstant.SchemeName)]
        private static async Task<IResult> CreateBook(Book book, IBookService bookService, IValidator<Book> validator)
        {
            var validationResult = await validator.ValidateAsync(book);
            if (!validationResult.IsValid)
            {
                return Results.BadRequest(validationResult.Errors);
            }
            var created = await bookService.CreateAsync(book);
            if (!created)
            {
                return Results.BadRequest("Error during creation");
            }
            return Results.Created($"/books/{book.Isbn}", book);
        }
        private static async Task<IResult> GetAllBooks(IBookService bookService, string? searchTerm)
        {
            if(searchTerm is not null && !string.IsNullOrWhiteSpace(searchTerm))
            {
                var matchedBook = await bookService.SearchByTitleAsync(searchTerm);
                return Results.Ok(matchedBook);
            }
            var result = await bookService.GetAllBookAsync();
            return Results.Ok(result);
        }
        private static async Task<IResult> GetBooksByIsbn(string isbn, IBookService bookService)
        {
            var book = await bookService.GetByIsbnAsync(isbn);
            return book is not null ? Results.Ok(book) : Results.NotFound();
        }
        [Authorize(AuthenticationSchemes = ApiKeySchemeConstant.SchemeName)]
        private static async Task<IResult> UpdateBook(string isbn, Book book, IBookService bookService, IValidator<Book> validator)
        {
            book.Isbn = isbn;
            var validationResult = await validator.ValidateAsync(book);

            if (!validationResult.IsValid)
            {
                return Results.BadRequest(validationResult.Errors);
            }

            var updated = await bookService.UpadateAsync(book);
            return updated ? Results.Ok(book) : Results.NotFound();
        }
        [Authorize(AuthenticationSchemes = ApiKeySchemeConstant.SchemeName)]
        private static async Task<IResult> DeleteBook(string isbn, IBookService bookService)
        {
            var deleted = await bookService.DeleteAsync(isbn);
            return deleted ? Results.NoContent() : Results.NotFound();
        }
    }
}
