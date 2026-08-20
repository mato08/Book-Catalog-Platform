using MediatR;

namespace BookCatalog.Application.Features.Books.DeleteBooks;

public sealed record DeleteBooksCommand(int Id) : IRequest;
