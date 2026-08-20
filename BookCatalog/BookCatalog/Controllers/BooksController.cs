using BookCatalog.Application.Common.Repositories;
using BookCatalog.Application.Features.Books;
using BookCatalog.Application.Features.Books.CreateBooks;
using BookCatalog.Application.Features.Books.DeleteBooks;
using BookCatalog.Application.Features.Books.UpdateBooks;
using BookCatalog.Application.Features.Books.GetBooks;
using Microsoft.AspNetCore.Mvc;
using MediatR;



namespace BookCatalog.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly ISender sender;

        public BooksController(ISender sender)
        {
            this.sender = sender;
        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<BooksResponseDto>>> GetBooksAsync(int reviewPage = 1, int reviewPageSize = 20, CancellationToken cancellationToken = default)
        {
            var query = new GetBooksQuery(reviewPage, reviewPageSize);

            var books = await sender.Send(query, cancellationToken);


            return Ok(books);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<BooksResponseDto>> GetBookByIdAsync(int id, CancellationToken cancellationToken)
        {
            var book = await sender.Send(new GetBookByIdQuery(id), cancellationToken);

            return Ok(book);
        }

        [HttpPost]
        public async Task<ActionResult<CreateBooksResult>> CreateBookAsync(
            [FromBody] CreateBooksCommand command,
            CancellationToken cancellationToken)
        {
            var result = await sender.Send(command, cancellationToken);

            return CreatedAtAction(nameof(GetBookByIdAsync), new { id = result.Id }, result);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateBookAsync(
            int id,
            [FromBody] UpdateBooksCommand command,
            CancellationToken cancellationToken)
        {
            command.Id = id;
            await sender.Send(command, cancellationToken);

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteBookAsync(int id, CancellationToken cancellationToken)
        {
            await sender.Send(new DeleteBooksCommand(id), cancellationToken);

            return NoContent();
        }


    }
}
