using BookCatalog.Application.Common.Repositories;
using MediatR;

namespace BookCatalog.Application.Features.Books.GetBooks;

public sealed class GetBookByIdQueryHandler(IBooksRepository repository)
    : IRequestHandler<GetBookByIdQuery, BooksResponseDto>
{
    public Task<BooksResponseDto> Handle(GetBookByIdQuery request, CancellationToken cancellationToken)
        => repository.GetBookByIdAsync(request.Id, cancellationToken);
}
