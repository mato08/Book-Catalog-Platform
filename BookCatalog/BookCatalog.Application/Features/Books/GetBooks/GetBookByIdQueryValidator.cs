using FluentValidation;

namespace BookCatalog.Application.Features.Books.GetBooks;

public sealed class GetBookByIdQueryValidator : AbstractValidator<GetBookByIdQuery>
{
    public GetBookByIdQueryValidator()
    {
        RuleFor(query => query.Id)
            .GreaterThan(0);
    }
}
