using BookApi.DTOs;
using BookApi.Models;

namespace BookApi.Services;

public interface IBookService
{
    List<Book> GetAll(string? author = null);
    Book? GetById(int id);
    Book Create(BookCreateDto dto);
    bool Update(int id, BookUpdateDto dto);
    bool Delete(int id);
}