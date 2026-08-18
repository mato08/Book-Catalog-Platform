using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookCatalog.Application.Features.Books;
using BookCatalog.Domain.Entities;

namespace BookCatalog.Application.Common.Repositories
{
    public interface IBooksRepository
    {
        Task<IReadOnlyList<BooksResponseDto>> GetBooksAsync(int reviewPage = 1, int reviewPageSize = 20);

        Task<BooksResponseDto> GetBookByIdAsync(int id);

        Task<bool> UpdateBook(BooksResponseDto booksResponseDto);

        Task<int> CreateBookAsync(BooksResponseDto bookResponseDto);

        Task<bool> DeleteBookAsync(int id);
    }
}
