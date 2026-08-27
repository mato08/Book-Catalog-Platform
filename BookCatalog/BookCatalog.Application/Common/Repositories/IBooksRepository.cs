using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookCatalog.Application.Features.Books;
using BookCatalog.Application.Features.Books.CreateBooks;
using BookCatalog.Application.Features.Books.GetBooks;
using BookCatalog.Domain.Entities;

namespace BookCatalog.Application.Common.Repositories
{
    public interface IBooksRepository
    {
        Task<GetBooksResult> GetBooksAsync(GetBooksFilterQuery filter, int reviewPage = 1, int reviewPageSize = 20, CancellationToken  cancellationToken = default);

        Task<BooksResponseDto> GetBookByIdAsync(int id, CancellationToken cancellationToken = default);

        Task<bool> UpdateBook(BooksResponseDto booksResponseDto, CancellationToken cancellationToken = default);

        Task<CreateBooksResult> CreateBookAsync(CreateBooksCommand createBooksCommand, CancellationToken cancellationToken = default);

        Task<bool> DeleteBookAsync(int id, CancellationToken cancellationToken = default);
    }
}
