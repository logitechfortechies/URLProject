using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using UrlShortener.Domain;
using UrlShortener.Infrastructure;
using QRCoder; 
using System.Drawing; 
using System.IO; 

namespace UrlShortener.Application
{
    public interface IUrlShortenerService
    {
        // 4. UPDATE THE INTERFACE to return the new Response object
        Task<CreateShortUrlResponse> CreateShortUrlAsync(string longUrl, string requestScheme, string requestHost);
        Task<string?> GetLongUrlAsync(string shortCode);
    }

    public class UrlShortenerService : IUrlShortenerService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IDistributedCache _cache;

        public UrlShortenerService(ApplicationDbContext dbContext, IDistributedCache cache)
        {
            _dbContext = dbContext;
            _cache = cache;
        }

        // 5. UPDATE THE METHOD SIGNATURE
        public async Task<CreateShortUrlResponse> CreateShortUrlAsync(string longUrl, string requestScheme, string requestHost)
        {
            var shortCode = await GenerateUniqueShortCodeAsync();

            var shortenedUrl = new ShortenedUrl
            {
                LongUrl = longUrl,
                ShortCode = shortCode,
                CreatedOnUtc = DateTime.UtcNow
            };

            await _dbContext.ShortenedUrls.AddAsync(shortenedUrl);
            await _dbContext.SaveChangesAsync();

            var cacheOptions = new DistributedCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromHours(1));
            await _cache.SetStringAsync(shortCode, longUrl, cacheOptions);

            // 6. --- GENERATE QR CODE AND FULL URL ---
            var shortUrl = $"{requestScheme}://{requestHost}/{shortCode}";
            var qrCodeBase64 = GenerateQrCode(shortUrl);

            // 7. Return the new object
            return new CreateShortUrlResponse(shortUrl, qrCodeBase64);
        }

        public async Task<string?> GetLongUrlAsync(string shortCode)
        {
            string? longUrl = await _cache.GetStringAsync(shortCode);

            if (string.IsNullOrEmpty(longUrl))
            {
                var shortenedUrl = await _dbContext.ShortenedUrls
                    .FirstOrDefaultAsync(s => s.ShortCode == shortCode);

                if (shortenedUrl is null)
                {
                    return null;
                }

                longUrl = shortenedUrl.LongUrl;

                var cacheOptions = new DistributedCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromHours(1));
                await _cache.SetStringAsync(shortCode, longUrl, cacheOptions);
            }

            return longUrl;
        }

        // 8. --- NEW QR CODE GENERATOR METHOD ---
        private string GenerateQrCode(string url)
        {
            using (var qrGenerator = new QRCodeGenerator())
            using (var qrCodeData = qrGenerator.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q))
            using (var qrCode = new PngByteQRCode(qrCodeData))
            {
                byte[] qrCodeImageBytes = qrCode.GetGraphic(20);
                // Convert to a Base64 string that can be sent as JSON
                return $"data:image/png;base64,{Convert.ToBase64String(qrCodeImageBytes)}";
            }
        }

        private async Task<string> GenerateUniqueShortCodeAsync()
        {
            const string chars = "abcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            var codeLength = 7;

            while (true)
            {
                var code = new string(Enumerable.Repeat(chars, codeLength)
                    .Select(s => s[random.Next(s.Length)]).ToArray());

                if (!await _dbContext.ShortenedUrls.AnyAsync(s => s.ShortCode == code))
                {
                    return code;
                }
            }
        }
    }
}