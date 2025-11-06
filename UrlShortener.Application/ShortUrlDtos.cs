namespace UrlShortener.Application
{
	
	public record CreateShortUrlRequest(string LongUrl);
	public record CreateShortUrlResponse(string ShortUrl);
}