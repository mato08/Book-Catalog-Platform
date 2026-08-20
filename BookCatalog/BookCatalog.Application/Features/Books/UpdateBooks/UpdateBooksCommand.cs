using MediatR;

namespace BookCatalog.Application.Features.Books.UpdateBooks;

public sealed class UpdateBooksCommand : IRequest
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Genre { get; set; } = string.Empty;

    public AuthorDto Author { get; set; } = null!;
}
