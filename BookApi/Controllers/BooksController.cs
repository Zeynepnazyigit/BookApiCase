using BookApi.DTOs;
using BookApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BooksController : ControllerBase
{
    private readonly IBookService _bookService;
    private readonly ILogger<BooksController> _logger;

    public BooksController(IBookService bookService, ILogger<BooksController> logger)
    {
        _bookService = bookService;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult GetAll([FromQuery] string? author)
    {
        var books = _bookService.GetAll(author);
        return Ok(books);
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var book = _bookService.GetById(id);

        if (book is null)
        {
            _logger.LogWarning("Kitap bulunamadı. Id: {Id}", id);
            return NotFound(new { message = "Kitap bulunamadı." });
        }

        return Ok(book);
    }

    [HttpPost]
    public IActionResult Create([FromBody] BookCreateDto dto)
    {
        var createdBook = _bookService.Create(dto);
        return CreatedAtAction(nameof(GetById), new { id = createdBook.Id }, createdBook);
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, [FromBody] BookUpdateDto dto)
    {
        var updated = _bookService.Update(id, dto);

        if (!updated)
        {
            _logger.LogWarning("Güncellenecek kitap bulunamadı. Id: {Id}", id);
            return NotFound(new { message = "Kitap bulunamadı." });
        }

        return Ok(new { message = "Kitap güncellendi." });
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var deleted = _bookService.Delete(id);

        if (!deleted)
        {
            _logger.LogWarning("Silinecek kitap bulunamadı. Id: {Id}", id);
            return NotFound(new { message = "Kitap bulunamadı." });
        }

        return NoContent();
    }
}