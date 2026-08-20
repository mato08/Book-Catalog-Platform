using BookCatalog.Application.Common.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookCatalog.Application.Features.Books.CreateBooks
{
    public class CreateBooksHandler : IRequestHandler<CreateBooksCommand, CreateBooksResult>
    {
        private readonly IBooksRepository booksRepository;

        public CreateBooksHandler(IBooksRepository booksRepository)
        {
            this.booksRepository = booksRepository;
        }

        public async Task<CreateBooksResult> Handle(CreateBooksCommand request, CancellationToken cancellationToken)
        {
            return await booksRepository.CreateBookAsync(request, cancellationToken);
        }
    }
}
