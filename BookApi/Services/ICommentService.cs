using BookApi.Models;

namespace BookApi.Services;

public interface ICommentService
{
    Task<List<CommentResponse>> GetCommentsAsync();
}