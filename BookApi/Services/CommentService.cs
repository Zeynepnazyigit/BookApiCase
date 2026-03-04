using System.Net;
using System.Text.Json;
using BookApi.Models;

namespace BookApi.Services;

public class CommentService : ICommentService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<CommentService> _logger;

    public CommentService(HttpClient httpClient, ILogger<CommentService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<List<CommentResponse>> GetCommentsAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("https://jsonplaceholder.typicode.com/posts/1/comments");

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Dış servis hata döndü. StatusCode: {StatusCode}", response.StatusCode);
                throw new HttpRequestException("Dış servise erişilemedi.", null, HttpStatusCode.BadGateway);
            }

            var json = await response.Content.ReadAsStringAsync();

            var comments = JsonSerializer.Deserialize<List<Comment>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new List<Comment>();

            var result = comments.Select(c => new CommentResponse
            {
                PostId = c.PostId,
                Id = c.Id,
                Name = c.Name.ToUpperInvariant(),
                Email = MaskEmail(c.Email),
                Body = ShortenBody(c.Body)
            }).ToList();

            _logger.LogInformation("Comments verisi başarıyla alındı.");
            return result;
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.BadGateway)
        {
            _logger.LogError(ex, "Dış servis erişim hatası.");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Comments alınırken hata oluştu.");
            throw new HttpRequestException("Dış servise erişilemedi.", null, HttpStatusCode.BadGateway);
        }
    }

    private string MaskEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
            return email;

        var parts = email.Split('@');
        var namePart = parts[0];
        var domainPart = parts[1];

        if (namePart.Length <= 1)
            return $"{namePart[0]}***@{domainPart}";

        return $"{namePart[0]}***@{domainPart}";
    }

    private string ShortenBody(string body)
    {
        if (string.IsNullOrWhiteSpace(body))
            return string.Empty;

        return body.Length <= 50 ? body : body.Substring(0, 50) + "...";
    }
}