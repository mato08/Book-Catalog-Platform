using BookCatalog.Application.Common.Repositories;
using BookCatalog.Application.Features.Books;
using BookCatalog.Application.Features.Books.CreateBooks;
using BookCatalog.Application.Features.Books.DeleteBooks;
using BookCatalog.Application.Features.Books.GetBooks;
using BookCatalog.Application.Features.Books.UpdateBooks;
using Moq;
using Xunit;

namespace BookCatalog.Tests;

public sealed class BookCommandValidatorTests
{
    [Fact]
    public void CreateValidator_AcceptsValidCommand()
    {
        var result = new CreateBooksCommandValidator().Validate(CreateCommand());

        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData("Title")]
    [InlineData("Description")]
    [InlineData("Genre")]
    public void CreateValidator_RejectsMissingRequiredText(string propertyName)
    {
        var command = CreateCommand();
        typeof(CreateBooksCommand).GetProperty(propertyName)!.SetValue(command, string.Empty);

        var result = new CreateBooksCommandValidator().Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == propertyName);
    }

    [Fact]
    public void CreateValidator_RejectsMissingAuthorDetails()
    {
        var command = CreateCommand();
        command.Author = new AuthorDto { Id = 0, Name = string.Empty };

        var result = new CreateBooksCommandValidator().Validate(command);

        Assert.Contains(result.Errors, error => error.PropertyName == "Author.Id");
        Assert.Contains(result.Errors, error => error.PropertyName == "Author.Name");
    }

    [Fact]
    public void UpdateValidator_RequiresPositiveIdAndAuthorId()
    {
        var command = new UpdateBooksCommand
        {
            Id = 0,
            Title = "Clean Code",
            Description = "A software craftsmanship book.",
            Genre = "Technology",
            Author = new AuthorDto { Id = 0, Name = "Robert" }
        };

        var result = new UpdateBooksCommandValidator().Validate(command);

        Assert.Contains(result.Errors, error => error.PropertyName == "Id");
        Assert.Contains(result.Errors, error => error.PropertyName == "Author.Id");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void IdValidators_RejectNonPositiveIds(int id)
    {
        Assert.False(new DeleteBooksCommandValidator().Validate(new DeleteBooksCommand(id)).IsValid);
        Assert.False(new GetBookByIdQueryValidator().Validate(new GetBookByIdQuery(id)).IsValid);
    }

    [Theory]
    [InlineData(0, 20)]
    [InlineData(1, 0)]
    [InlineData(1, 101)]
    public void GetBooksValidator_RejectsInvalidPaging(int page, int pageSize)
    {
        var query = new GetBooksQuery(page, pageSize, new GetBooksFilterQuery(null, null));

        Assert.False(new GetBooksQueryValidator().Validate(query).IsValid);
    }

    private static CreateBooksCommand CreateCommand() => new()
    {
        Id = 7,
        Title = "Clean Code",
        Description = "A software craftsmanship book.",
        Genre = "Technology",
        Author = new AuthorDto { Id = 3, Name = "Robert", LastName = "Martin" }
    };
}

public sealed class BookHandlerTests
{
    private readonly Mock<IBooksRepository> repository = new();

    [Fact]
    public async Task CreateHandler_ReturnsRepositoryResult()
    {
        var command = CreateCommand();
        var expected = new CreateBooksResult { Id = 42 };
        repository.Setup(x => x.CreateBookAsync(command, It.IsAny<CancellationToken>())).ReturnsAsync(expected);

        var result = await new CreateBooksHandler(repository.Object).Handle(command, CancellationToken.None);

        Assert.Same(expected, result);
        repository.Verify(x => x.CreateBookAsync(command, CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task UpdateHandler_MapsCommandToRepositoryDto()
    {
        var command = new UpdateBooksCommand
        {
            Id = 8, Title = "Dune", Description = "Science fiction", Genre = "Sci-Fi",
            Author = new AuthorDto { Id = 2, Name = "Frank" }
        };
        BooksResponseDto? captured = null;
        repository.Setup(x => x.UpdateBook(It.IsAny<BooksResponseDto>(), It.IsAny<CancellationToken>()))
            .Callback<BooksResponseDto, CancellationToken>((dto, _) => captured = dto)
            .ReturnsAsync(true);

        await new UpdateBooksHandler(repository.Object).Handle(command, CancellationToken.None);

        Assert.NotNull(captured);
        Assert.Equal(command.Id, captured.BookId);
        Assert.Equal(command.Title, captured.Title);
        Assert.Equal(command.Description, captured.Description);
        Assert.Equal(command.Genre, captured.Genre);
        Assert.Same(command.Author, captured.Author);
    }

    [Fact]
    public async Task DeleteHandler_DelegatesIdAndCancellationToken()
    {
        repository.Setup(x => x.DeleteBookAsync(11, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        using var cancellation = new CancellationTokenSource();

        await new DeleteBooksHandler(repository.Object).Handle(new DeleteBooksCommand(11), cancellation.Token);

        repository.Verify(x => x.DeleteBookAsync(11, cancellation.Token), Times.Once);
    }

    [Fact]
    public async Task GetBooksHandler_ForwardsFilterAndPaging()
    {
        var filter = new GetBooksFilterQuery(4m, "Mystery");
        var query = new GetBooksQuery(2, 10, filter);
        var expected = new GetBooksResult(1, []);
        repository.Setup(x => x.GetBooksAsync(filter, 2, 10, It.IsAny<CancellationToken>())).ReturnsAsync(expected);

        var result = await new GetBooksQueryHandler(repository.Object).Handle(query, CancellationToken.None);

        Assert.Same(expected, result);
        repository.Verify(x => x.GetBooksAsync(filter, 2, 10, CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task GetBookByIdHandler_ReturnsRepositoryBook()
    {
        var expected = new BooksResponseDto { BookId = 9, Title = "The Hobbit" };
        repository.Setup(x => x.GetBookByIdAsync(9, It.IsAny<CancellationToken>())).ReturnsAsync(expected);

        var result = await new GetBookByIdQueryHandler(repository.Object).Handle(new GetBookByIdQuery(9), CancellationToken.None);

        Assert.Same(expected, result);
        repository.Verify(x => x.GetBookByIdAsync(9, CancellationToken.None), Times.Once);
    }

    private static CreateBooksCommand CreateCommand() => new()
    {
        Id = 1, Title = "Dune", Description = "Science fiction", Genre = "Sci-Fi",
        Author = new AuthorDto { Id = 2, Name = "Frank" }
    };
}
