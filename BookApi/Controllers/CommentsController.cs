using BookApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CommentsController : ControllerBase
{
    private readonly ICommentService _commentService;
    private readonly ILogger<CommentsController> _logger;

    public CommentsController(ICommentService commentService, ILogger<CommentsController> logger)
    {
        _commentService = commentService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetComments()
    {
        try
        {
            var comments = await _commentService.GetCommentsAsync();
            return Ok(comments);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Dış servise erişilemedi.");
            return StatusCode(502, new { message = "Dış servise erişilemedi." });
        }
    }
}