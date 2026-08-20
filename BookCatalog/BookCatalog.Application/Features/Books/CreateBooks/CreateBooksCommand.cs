using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookCatalog.Application.Features.Books.CreateBooks
{
    public sealed class CreateBooksCommand : IRequest<CreateBooksResult>
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string Genre { get; set; } = string.Empty;

        public AuthorDto Author { get; set; } = null!;
    }
}
