using FluentValidation;
using Microsoft.EntityFrameworkCore;
using UrlShortener.Application;
using UrlShortener.Infrastructure;

namespace UrlShortener.Application.Validators
{
    public class CreateShortUrlRequestValidator : AbstractValidator<CreateShortUrlRequest>
    {
        private readonly ApplicationDbContext _dbContext;

        public CreateShortUrlRequestValidator(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;

            RuleFor(x => x.LongUrl)
                .NotEmpty().WithMessage("URL must not be empty.")
                .Must(BeAValidUrl).WithMessage("A valid URL is required.");

            RuleFor(x => x.CustomAlias)
                .Must(alias => alias.Length = 5).When(x => !string.IsNullOrEmpty(x.CustomAlias))
                     .WithMessage("Custom alias must be 5 characters.")
                 .Matches("^[a-zA-Z0-9_-]*$").When(x => !string.IsNullOrEmpty(x.CustomAlias))
                    .WithMessage("Alias can only contain letters, numbers, dashes, and underscores.")
                .MustAsync(BeUniqueAlias).When(x => !string.IsNullOrEmpty(x.CustomAlias))
                    .WithMessage("This custom alias is already taken. Please choose another.");
        }

        private bool BeAValidUrl(string longUrl)
        {
            return Uri.IsWellFormedUriString(longUrl, UriKind.Absolute);
        }

        private async Task<bool> BeUniqueAlias(string? alias, CancellationToken token)
        {
            if (string.IsNullOrEmpty(alias))
                return true;

            return !await _dbContext.ShortenedUrls.AnyAsync(s => s.ShortCode == alias, token);
        }
    }
}