using FluentValidation;

namespace BookCatalog.Application.Features.Books.DeleteBooks;

public sealed class DeleteBooksCommandValidator : AbstractValidator<DeleteBooksCommand>
{
    public DeleteBooksCommandValidator()
    {
        RuleFor(command => command.Id)
            .GreaterThan(0);
    }
}
