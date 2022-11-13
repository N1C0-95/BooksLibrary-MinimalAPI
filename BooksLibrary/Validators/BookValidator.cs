using FluentValidation;
using MinimalApi.Models;

namespace MinimalApi.Validators
{
    public class BookValidator : AbstractValidator<Book>
    {
        public BookValidator()
        {
            //inserisco qui la mia logica di validazione
            RuleFor(book => book.Isbn)
                .Matches(@"^(?=(?:\D*\d){10}(?:(?:\D*\d){3})?$)[\d-]+$")
                .WithMessage("Value was not valid ISBN-13");
            RuleFor(book => book.Title).NotEmpty();
            RuleFor(book => book.ShortDescription).NotEmpty();
            RuleFor(book => book.PageCounter).GreaterThan(0);
            RuleFor(book => book.Author).NotEmpty();    
        }
    }
}
