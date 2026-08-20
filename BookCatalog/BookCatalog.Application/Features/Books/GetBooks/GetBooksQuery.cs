using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace BookCatalog.Application.Features.Books.GetBooks
{
    public record GetBooksQuery(int ReviewPage, int ReviewPageSize) : IRequest<IReadOnlyList<BooksResponseDto>>;
}
