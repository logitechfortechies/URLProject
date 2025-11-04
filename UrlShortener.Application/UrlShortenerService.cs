using Microsoft.EntityFrameworkCore;
using UrlShortener.Domain;
using UrlShortener.Infrastructure;

namespace UrlShortener.Application
{
    // 1. ADD THE NEW METHOD TO THE INTERFACE
    public interface IUrlShortenerService
    {
        Task<string> CreateShortUrlAsync(string longUrl);
        Task<string?> GetLongUrlAsync(string shortCode); // <-- NEW
    }

    public class UrlShortenerService : IUrlShortenerService
    {
        private readonly ApplicationDbContext _dbContext;

        public UrlShortenerService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // --- THIS IS YOUR EXISTING "CREATE" METHOD ---
        public async Task<string> CreateShortUrlAsync(string longUrl)
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

            return shortCode;
        }

        // --- 2. THIS IS THE NEW "GET" METHOD ---
        public async Task<string?> GetLongUrlAsync(string shortCode)
        {
            // Find the short code in the database
            var shortenedUrl = await _dbContext.ShortenedUrls
                .FirstOrDefaultAsync(s => s.ShortCode == shortCode);

            // Return the LongUrl if found, or null if not
            return shortenedUrl?.LongUrl;
        }

        // --- THIS IS YOUR EXISTING "GENERATE" METHOD ---
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