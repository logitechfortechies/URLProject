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
        // 1. THIS IS THE CORRECT INTERFACE
        // It now takes the full request object
        Task<CreateShortUrlResponse> CreateShortUrlAsync(CreateShortUrlRequest request, string requestScheme, string requestHost);
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

        // 2. THIS IS THE CORRECT METHOD SIGNATURE
        public async Task<CreateShortUrlResponse> CreateShortUrlAsync(CreateShortUrlRequest request, string requestScheme, string requestHost)
        {
            // 3. THIS IS THE NEW ALIAS LOGIC
            string shortCode;
            if (string.IsNullOrEmpty(request.CustomAlias))
            {
                // No custom alias, so generate a random one
                shortCode = await GenerateUniqueShortCodeAsync();
            }
            else
            {
                // Use the custom alias (it was already validated by FluentValidation)
                shortCode = request.CustomAlias;
            }
            // --- END OF NEW ALIAS LOGIC ---

            var shortenedUrl = new ShortenedUrl
            {
                LongUrl = request.LongUrl, // Use request.LongUrl
                ShortCode = shortCode,
                CreatedOnUtc = DateTime.UtcNow
            };

            await _dbContext.ShortenedUrls.AddAsync(shortenedUrl);
            await _dbContext.SaveChangesAsync();

            var cacheOptions = new DistributedCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromHours(1));

            // Cache the correct data
            await _cache.SetStringAsync(shortCode, request.LongUrl, cacheOptions);

            // --- YOUR QR CODE LOGIC (STAYS THE SAME) ---
            var shortUrl = $"{requestScheme}://{requestHost}/{shortCode}";
            var qrCodeBase64 = GenerateQrCode(shortUrl);

            // Return the new object (which is defined in ShortUrlDtos.cs)
            return new CreateShortUrlResponse(shortUrl, qrCodeBase64);
        }

        public async Task<string?> GetLongUrlAsync(string shortCode)
        {
            // Your GetLongUrlAsync logic is correct and stays the same
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

        // YOUR QR CODE METHOD (STAYS THE SAME)
        private string GenerateQrCode(string url)
        {
            using (var qrGenerator = new QRCodeGenerator())
            using (var qrCodeData = qrGenerator.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q))
            using (var qrCode = new PngByteQRCode(qrCodeData))
            {
                byte[] qrCodeImageBytes = qrCode.GetGraphic(20);
                return $"data:image/png;base64,{Convert.ToBase64String(qrCodeImageBytes)}";
            }
        }

        // YOUR RANDOM CODE GENERATOR (STAYS THE SAME)
        private async Task<string> GenerateUniqueShortCodeAsync()
        {
            const string chars = "abcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            var codeLength = 7;

            while (true)
                Async(s => s.ShortCode == code))
                {
                return code;
            }
        }
    }
}
}