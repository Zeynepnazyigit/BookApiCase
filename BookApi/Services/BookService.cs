using System.Text.Json;
using BookApi.DTOs;
using BookApi.Models;

namespace BookApi.Services;

public class BookService : IBookService
{
    private readonly string _filePath;
    private readonly ILogger<BookService> _logger;

    public BookService(IWebHostEnvironment env, ILogger<BookService> logger)
    {
        _filePath = Path.Combine(env.ContentRootPath, "Data", "books.json");
        _logger = logger;
    }

    public List<Book> GetAll(string? author = null)
    {
        try
        {
            var books = ReadFromFile();

            if (!string.IsNullOrWhiteSpace(author))
            {
                books = books
                    .Where(b => b.Author.Contains(author, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            return books;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Tüm kitaplar alınırken hata oluştu.");
            throw;
        }
    }

    public Book? GetById(int id)
    {
        try
        {
            var books = ReadFromFile();
            return books.FirstOrDefault(x => x.Id == id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Kitap getirilirken hata oluştu. Id: {Id}", id);
            throw;
        }
    }

    public Book Create(BookCreateDto dto)
    {
        try
        {
            Validate(dto.Title, dto.Author, dto.PublishedYear, dto.PageCount);

            var books = ReadFromFile();
            int newId = books.Count == 0 ? 1 : books.Max(x => x.Id) + 1;

            var book = new Book
            {
                Id = newId,
                Title = dto.Title.Trim(),
                Author = dto.Author.Trim(),
                PublishedYear = dto.PublishedYear,
                PageCount = dto.PageCount
            };

            books.Add(book);
            WriteToFile(books);

            _logger.LogInformation("Yeni kitap eklendi. Id: {Id}", book.Id);

            return book;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Kitap eklenirken hata oluştu.");
            throw;
        }
    }

    public bool Update(int id, BookUpdateDto dto)
    {
        try
        {
            Validate(dto.Title, dto.Author, dto.PublishedYear, dto.PageCount);

            var books = ReadFromFile();
            var book = books.FirstOrDefault(x => x.Id == id);

            if (book is null)
            {
                _logger.LogWarning("Güncellenecek kitap bulunamadı. Id: {Id}", id);
                return false;
            }

            book.Title = dto.Title.Trim();
            book.Author = dto.Author.Trim();
            book.PublishedYear = dto.PublishedYear;
            book.PageCount = dto.PageCount;

            WriteToFile(books);

            _logger.LogInformation("Kitap güncellendi. Id: {Id}", id);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Kitap güncellenirken hata oluştu. Id: {Id}", id);
            throw;
        }
    }

    public bool Delete(int id)
    {
        try
        {
            var books = ReadFromFile();
            var book = books.FirstOrDefault(x => x.Id == id);

            if (book is null)
            {
                _logger.LogWarning("Silinecek kitap bulunamadı. Id: {Id}", id);
                return false;
            }

            books.Remove(book);
            WriteToFile(books);

            _logger.LogInformation("Kitap silindi. Id: {Id}", id);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Kitap silinirken hata oluştu. Id: {Id}", id);
            throw;
        }
    }

    private List<Book> ReadFromFile()
    {
        if (!File.Exists(_filePath))
            return new List<Book>();

        var json = File.ReadAllText(_filePath);

        if (string.IsNullOrWhiteSpace(json))
            return new List<Book>();

        return JsonSerializer.Deserialize<List<Book>>(json) ?? new List<Book>();
    }

    private void WriteToFile(List<Book> books)
    {
        var json = JsonSerializer.Serialize(books, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        File.WriteAllText(_filePath, json);
    }

    private void Validate(string title, string author, int publishedYear, int pageCount)
    {
        int currentYear = DateTime.Now.Year;

        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title alanı zorunludur.");

        if (string.IsNullOrWhiteSpace(author))
            throw new ArgumentException("Author alanı zorunludur.");

        if (publishedYear < 1000 || publishedYear > currentYear)
            throw new ArgumentException($"PublishedYear 1000 ile {currentYear} arasında olmalıdır.");

        if (pageCount <= 1)
            throw new ArgumentException("PageCount 1'den büyük olmalıdır.");
    }
}