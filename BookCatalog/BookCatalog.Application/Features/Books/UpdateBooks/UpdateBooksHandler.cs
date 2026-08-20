using BookCatalog.Application.Common.Repositories;
using MediatR;

namespace BookCatalog.Application.Features.Books.UpdateBooks;

public sealed class UpdateBooksHandler(IBooksRepository repository)
    : IRequestHandler<UpdateBooksCommand>
{
    public async Task Handle(UpdateBooksCommand request, CancellationToken cancellationToken)
    {
        await repository.UpdateBook(new BooksResponseDto
        {
            BookId = request.Id,
            Title = request.Title,
            Description = request.Description,
            Genre = request.Genre,
            Author = request.Author
        }, cancellationToken);
    }
}
