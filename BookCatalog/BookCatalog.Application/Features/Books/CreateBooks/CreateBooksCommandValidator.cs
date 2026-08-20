using FluentValidation;

namespace BookCatalog.Application.Features.Books.CreateBooks;

public sealed class CreateBooksCommandValidator : AbstractValidator<CreateBooksCommand>
{
    public CreateBooksCommandValidator()
    {
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

            RuleFor(command => command.Author.Name)
                .NotEmpty().WithMessage("Author name is required.")
                .MaximumLength(100);
        });
    }
}
