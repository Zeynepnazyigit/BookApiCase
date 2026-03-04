namespace BookApi.DTOs
{
    public class BookCreateDto
    {
        public string Title { get; set; } = string.Empty;
        public string Author {  get; set; } = string.Empty;
        public int PublishedYear { get; set; }
        public int PageCount { get; set; }
    }
}
