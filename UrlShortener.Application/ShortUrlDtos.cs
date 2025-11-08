namespace UrlShortener.Application
{
    public record CreateShortUrlRequest(string LongUrl, string? CustomAlias);
    public record CreateShortUrlResponse(string ShortUrl, string QrCodeBase64);
}