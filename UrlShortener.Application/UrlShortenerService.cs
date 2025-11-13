using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using UrlShortener.Domain;
using UrlShortener.Infrastructure;
using QRCoder;
using System.Drawing;
using System.IO;
using System.Security.Claims;

namespace UrlShortener.Application
{
    public interface IUrlShortenerService
    {

        Task<CreateShortUrlResponse> CreateShortUrlAsync(CreateShortUrlRequest request, string requestScheme, string requestHost, ClaimsPrincipal user);
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

        // Create a shortened URL with optional custom alias
        public async Task<CreateShortUrlResponse> CreateShortUrlAsync(CreateShortUrlRequest request, string requestScheme, string requestHost, ClaimsPrincipal user)
        {
            string shortCode;
            if (string.IsNullOrEmpty(request.CustomAlias))
            {
                shortCode = await GenerateUniqueShortCodeAsync();
            }
            else
            {
                shortCode = request.CustomAlias + "amd";
            }

            //  Use 'ClaimTypes.NameIdentifier' to get the User ID
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);

            var shortenedUrl = new ShortenedUrl
            {
                LongUrl = request.LongUrl,
                ShortCode = shortCode,
                CreatedOnUtc = DateTime.UtcNow,
                UserId = userId
            };

            await _dbContext.ShortenedUrls.AddAsync(shortenedUrl);
            await _dbContext.SaveChangesAsync();

            var cacheOptions = new DistributedCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromHours(1));

            await _cache.SetStringAsync(shortCode, request.LongUrl, cacheOptions);

            var shortUrl = $"{requestScheme}://{requestHost}/{shortCode}";
            var qrCodeBase64 = GenerateQrCode(shortUrl);

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