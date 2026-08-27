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
    public class GetBooksQueryHandler : IRequestHandler<GetBooksQuery, GetBooksResult>
    {
        private readonly IBooksRepository repository;

        public GetBooksQueryHandler(IBooksRepository repository)
        {
            this.repository = repository;
        }

        public async Task<GetBooksResult> Handle(GetBooksQuery request, CancellationToken cancellationToken)
        {
            return await repository.GetBooksAsync(request.Filter, request.ReviewPage, request.ReviewPageSize,cancellationToken);
        }
    }
}
