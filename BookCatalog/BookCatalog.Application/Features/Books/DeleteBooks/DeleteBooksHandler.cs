using BookCatalog.Application.Common.Repositories;
using MediatR;

namespace BookCatalog.Application.Features.Books.DeleteBooks;

public sealed class DeleteBooksHandler(IBooksRepository repository)
    : IRequestHandler<DeleteBooksCommand>
{
    public async Task Handle(DeleteBooksCommand request, CancellationToken cancellationToken)
    {
        await repository.DeleteBookAsync(request.Id, cancellationToken);
    }
}
