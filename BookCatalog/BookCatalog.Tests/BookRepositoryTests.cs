using BookCatalog.Application.Common.Exceptions;
using BookCatalog.Application.Features.Books;
using BookCatalog.Application.Features.Books.CreateBooks;
using BookCatalog.Application.Features.Books.GetBooks;
using BookCatalog.Domain.Entities;
using BookCatalog.Infrastructure.Persistence;
using BookCatalog.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace BookCatalog.Tests;

public sealed class BookRepositoryTests
{
    [Fact]
    public async Task GetBooksAsync_AppliesBothFiltersAndReturnsFilteredCount()
    {
        await using var context = CreateContext();
        await SeedBooksAsync(context);
        var repository = new BookRepository(context);

        var result = await repository.GetBooksAsync(new GetBooksFilterQuery(4m, "Mystery"));

        Assert.Equal(1, result.Count);
        var book = Assert.Single(result.Books);
        Assert.Equal("The Silent Patient", book.Title);
    }

    [Fact]
    public async Task GetBooksAsync_PaginatesFilteredBooks()
    {
        await using var context = CreateContext();
        await SeedBooksAsync(context);
        var repository = new BookRepository(context);

        var result = await repository.GetBooksAsync(new GetBooksFilterQuery(null, "Mystery"), reviewPage: 2, reviewPageSize: 1);

        Assert.Equal(2, result.Count);
        var book = Assert.Single(result.Books);
        Assert.Equal("Gone Girl", book.Title);
    }

    [Fact]
    public async Task CreateBookAsync_PersistsBookAndAuthor()
    {
        await using var context = CreateContext();
        var repository = new BookRepository(context);
        var command = new CreateBooksCommand
        {
            Id = 12, Title = "Dune", Description = "Science fiction", Genre = "Sci-Fi",
            Author = new AuthorDto { Id = 6, Name = "Frank", LastName = "Herbert" }
        };

        var result = await repository.CreateBookAsync(command);
        var stored = await context.Books.Include(book => book.Author).SingleAsync();

        Assert.Equal(12, result.Id);
        Assert.Equal(command.Title, stored.Title);
        Assert.Equal("Frank", stored.Author.Name);
    }

    [Fact]
    public async Task GetBookByIdAsync_ReturnsBookWithCompleteAuthorData()
    {
        await using var context = CreateContext();
        await SeedBooksAsync(context);
        var repository = new BookRepository(context);

        var result = await repository.GetBookByIdAsync(1);

        Assert.Equal(1, result.BookId);
        Assert.Equal(1, result.Author.Id);
        Assert.Equal("Alex", result.Author.Name);
        Assert.Equal("Writer", result.Author.LastName);
    }

    [Fact]
    public async Task UpdateBook_UpdatesStoredValues()
    {
        await using var context = CreateContext();
        await SeedBooksAsync(context);
        var repository = new BookRepository(context);
        var update = new BooksResponseDto
        {
            BookId = 1, Title = "Updated title", Description = "Updated description", Genre = "Thriller",
            Author = new AuthorDto { Id = 2, Name = "Jamie" }
        };

        var updated = await repository.UpdateBook(update);
        var stored = await context.Books.FindAsync(1);

        Assert.True(updated);
        Assert.NotNull(stored);
        Assert.Equal(update.Title, stored.Title);
        Assert.Equal(2, stored.AuthorId);
    }

    [Fact]
    public async Task DeleteBookAsync_RemovesBook()
    {
        await using var context = CreateContext();
        await SeedBooksAsync(context);
        var repository = new BookRepository(context);

        var deleted = await repository.DeleteBookAsync(1);

        Assert.True(deleted);
        Assert.Null(await context.Books.FindAsync(1));
    }

    [Fact]
    public async Task MissingBookOperations_ThrowNotFoundException()
    {
        await using var context = CreateContext();
        var repository = new BookRepository(context);

        await Assert.ThrowsAsync<NotFoundException>(() => repository.GetBookByIdAsync(999));
        await Assert.ThrowsAsync<NotFoundException>(() => repository.UpdateBook(new BooksResponseDto { BookId = 999, Author = new AuthorDto() }));
        await Assert.ThrowsAsync<NotFoundException>(() => repository.DeleteBookAsync(999));
    }

    private static AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    private static async Task SeedBooksAsync(AppDbContext context)
    {
        var firstAuthor = new Author { Id = 1, Name = "Alex", LastName = "Writer" };
        var secondAuthor = new Author { Id = 2, Name = "Jamie", LastName = "Author" };
        context.Books.AddRange(
            new Book { Id = 1, Title = "The Silent Patient", Description = "A mystery", Genre = "Mystery", Rating = 4.5m, Author = firstAuthor },
            new Book { Id = 2, Title = "Gone Girl", Description = "A mystery", Genre = "Mystery", Rating = 3.5m, Author = secondAuthor },
            new Book { Id = 3, Title = "Dune", Description = "A novel", Genre = "Sci-Fi", Rating = 4.8m, Author = secondAuthor });
        await context.SaveChangesAsync();
    }
}
