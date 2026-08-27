using BookCatalog.Application.Common.Repositories;
using BookCatalog.Application.Features.Books;
using BookCatalog.Application.Common.Exceptions;
using BookCatalog.Domain.Entities;
using BookCatalog.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using BookCatalog.Application.Features.Books.CreateBooks;
using BookCatalog.Application.Features.Books.GetBooks;

namespace BookCatalog.Infrastructure.Repositories
{
    public class BookRepository(AppDbContext appDbContext) : IBooksRepository
    {
        public async Task<GetBooksResult> GetBooksAsync(GetBooksFilterQuery filter, int reviewPage = 1, int reviewPageSize = 20, CancellationToken cancellationToken = default)
        {
            var query = appDbContext.Books.AsQueryable();

            if (filter.Rating.HasValue)
            {
                query = query.Where(x => x.Rating >= filter.Rating.Value);
            }

            if (!string.IsNullOrWhiteSpace(filter.Genre))
            {
                query = query.Where(x => x.Genre == filter.Genre);
            }

            var count = await query.CountAsync(cancellationToken);

             var books = await query
                 .OrderBy(x => x.Id)
                 .Skip((reviewPage - 1) * reviewPageSize)
                 .Take(reviewPageSize)
                 .Select(x => new BooksResponseDto
                 {
                     BookId = x.Id,
                     Title = x.Title,
                     Genre = x.Genre,
                     Author = new AuthorDto
                     {
                         Id = x.Author.Id,
                         Name = x.Author.Name,
                         LastName = x.Author.LastName,
                         Biography = x.Author.Biography,
                         BirthDate = x.Author.BirthDate,
                     },
                     Description = x.Description,
                 })
                 .ToListAsync(cancellationToken);

            return new GetBooksResult(count, books);
        }


        public async Task<BooksResponseDto> GetBookByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await appDbContext.Books
                .Where(book => book.Id == id)
                .Select(book => new BooksResponseDto
                {
                    BookId = book.Id,
                    Title = book.Title,
                    Genre = book.Genre,
                    Author = new AuthorDto
                    {
                        Id = book.Author.Id,
                        Name = book.Author.Name,
                        LastName = book.Author.LastName,
                        Biography = book.Author.Biography,
                        BirthDate = book.Author.BirthDate
                    },
                    Description = book.Description,
                })
                .FirstOrDefaultAsync(cancellationToken)
                ?? throw new NotFoundException(nameof(Book), id);
        }

        public async Task<bool> UpdateBook(BooksResponseDto booksResponseDto, CancellationToken cancellationToken = default)
        {
            var book = await appDbContext.Books.FindAsync([booksResponseDto.BookId], cancellationToken)
                ?? throw new NotFoundException(nameof(Book), booksResponseDto.BookId);

            book.Title = booksResponseDto.Title;
            book.Description = booksResponseDto.Description;
            book.Genre = booksResponseDto.Genre;
            book.AuthorId = booksResponseDto.Author.Id;

            await appDbContext.SaveChangesAsync(cancellationToken);

            return true;
        }

        public async Task<CreateBooksResult> CreateBookAsync(CreateBooksCommand createBooksCommand, CancellationToken cancellationToken = default)
        {
            var book = new Book 
            {
                Id = createBooksCommand.Id,
                Title = createBooksCommand.Title,
                Description = createBooksCommand.Description,
                Genre = createBooksCommand.Genre,
                Author = new Author {
                    Id =    createBooksCommand.Author.Id,
                    Name = createBooksCommand.Author.Name,
                    LastName = createBooksCommand.Author.LastName,
                    Biography = createBooksCommand.Author.Biography,
                    BirthDate = createBooksCommand.Author.BirthDate,
                }
            };

            await appDbContext.Books.AddAsync(book, cancellationToken);
            await appDbContext.SaveChangesAsync(cancellationToken);

            var createBookResult = new CreateBooksResult
            {
                Id = book.Id,
            };

            return createBookResult;
        }

        public async Task<bool> DeleteBookAsync(int id, CancellationToken cancellationToken = default)
        {
            var book = await appDbContext.Books.FindAsync([id], cancellationToken)
                ?? throw new NotFoundException(nameof(Book), id);

            appDbContext.Books.Remove(book);

            await appDbContext.SaveChangesAsync( cancellationToken);

            return true;
        }
    }
}
