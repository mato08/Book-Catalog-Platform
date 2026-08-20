using BookCatalog.Application.Common.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace BookCatalog.Application.Features.Books.GetBooks
{
    public class GetBooksQueryHandler : IRequestHandler<GetBooksQuery, IReadOnlyList<BooksResponseDto>>
    {
        private readonly IBooksRepository repository;

        public GetBooksQueryHandler(IBooksRepository repository)
        {
            this.repository = repository;
        }

        public async Task<IReadOnlyList<BooksResponseDto>> Handle(GetBooksQuery request, CancellationToken cancellationToken)
        {
            return await repository.GetBooksAsync(request.ReviewPage, request.ReviewPageSize,cancellationToken);
        }
    }
}
