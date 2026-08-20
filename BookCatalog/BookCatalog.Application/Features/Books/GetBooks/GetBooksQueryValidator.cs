using FluentValidation;

namespace BookCatalog.Application.Features.Books.GetBooks;

public sealed class GetBooksQueryValidator : AbstractValidator<GetBooksQuery>
{
    public GetBooksQueryValidator()
    {
        RuleFor(query => query.ReviewPage)
            .GreaterThan(0);

        RuleFor(query => query.ReviewPageSize)
            .InclusiveBetween(1, 100);
    }
}
