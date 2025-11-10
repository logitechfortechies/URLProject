namespace UrlShortener.Domain
{
    public class ShortenedUrl
    {
        public int Id { get; set; }
        public string LongUrl { get; set; } = string.Empty;
        public string ShortCode { get; set; } = string.Empty;
        public DateTime CreatedOnUtc { get; set; }
        public string? UserId { get; set; }
    }
}