using MediatR;

namespace BookCatalog.Application.Features.Books.GetBooks;

public sealed record GetBookByIdQuery(int Id) : IRequest<BooksResponseDto>;
