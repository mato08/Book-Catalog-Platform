using FluentValidation;

namespace BookCatalog.Application.Features.Books.UpdateBooks;

public sealed class UpdateBooksCommandValidator : AbstractValidator<UpdateBooksCommand>
{
    public UpdateBooksCommandValidator()
    {
        RuleFor(command => command.Id)
            .GreaterThan(0);

        RuleFor(command => command.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200);

        RuleFor(command => command.Description)
            .NotEmpty().WithMessage("Description is required.")
            .MaximumLength(4_000);

        RuleFor(command => command.Genre)
            .NotEmpty().WithMessage("Genre is required.")
            .MaximumLength(100);

        RuleFor(command => command.Author)
            .NotNull().WithMessage("Author is required.");

        When(command => command.Author is not null, () =>
        {
            RuleFor(command => command.Author.Id)
                .NotEmpty().WithMessage("Author ID is required.");
        });
    }
}
