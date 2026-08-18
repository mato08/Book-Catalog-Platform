using BookCatalog.Application.Common.Repositories;
using BookCatalog.Application.Features.Books;
using BookCatalog.Domain.Entities;
using BookCatalog.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace BookCatalog.Infrastructure.Repositories
{
    public class BookRepository(AppDbContext appDbContext) : IBooksRepository
    {
        public async Task<IReadOnlyList<BooksResponseDto>> GetBooksAsync(int reviewPage = 1, int reviewPageSize = 20)
        {
            return await appDbContext.Books
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
                 .ToListAsync();
        }


        public async Task<BooksResponseDto> GetBookByIdAsync(int id)
        {
            var book = await appDbContext.Books.FindAsync(id);

            var result = new BooksResponseDto
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
                    BirthDate = book.Author.BirthDate,
                },
                Description = book.Description,
            };

            return result;
        }

        public async Task<bool> UpdateBook(BooksResponseDto booksResponseDto)
        {
            var book = await appDbContext.Books.FindAsync(booksResponseDto.BookId);

            book.Title = booksResponseDto.Title;
            book.Description = booksResponseDto.Description;
            book.Genre = booksResponseDto.Genre;
            book.AuthorId = booksResponseDto.Author.Id;

            await appDbContext.SaveChangesAsync();

            return true;
        }

        public async Task<int> CreateBookAsync(BooksResponseDto booksResponseDto)
        {
            var book = new Book 
            {
                Id = booksResponseDto.BookId,
                Title = booksResponseDto.Title,
                Description = booksResponseDto.Description,
                Genre = booksResponseDto.Genre,
                Author = new Author {
                    Id = booksResponseDto.Author.Id,
                    Name = booksResponseDto.Author.Name,
                    LastName = booksResponseDto.Author.LastName,
                    Biography = booksResponseDto.Author.Biography,
                    BirthDate = booksResponseDto.Author.BirthDate,
                }
            };

            await appDbContext.Books.AddAsync(book);
            await appDbContext.SaveChangesAsync();

            return book.Id;
        }

        public async Task<bool> DeleteBookAsync(int id)
        {
            var book = await appDbContext.Books.FindAsync(id);

            if (book == null)
            {
                return false;
            }

            appDbContext.Books.Remove(book);

            await appDbContext.SaveChangesAsync();

            return true;

        }
    }
}
