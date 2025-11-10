using FluentValidation;
using Microsoft.EntityFrameworkCore;
using UrlShortener.Application;
using UrlShortener.Infrastructure; 

namespace UrlShortener.Application.Validators
{
    public class CreateShortUrlRequestValidator : AbstractValidator<CreateShortUrlRequest>
    {
        
        private readonly ApplicationDbContext _dbContext;

        //  UPDATE THE CONSTRUCTOR TO RECEIVE THE DATABASE
        public CreateShortUrlRequestValidator(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext; // 4. ASSIGN THE DATABASE

           
            RuleFor(x => x.LongUrl)
                .NotEmpty().WithMessage("URL must not be empty.")
                .Must(BeAValidUrl).WithMessage("A valid URL is required.");

            //  CUSTOM ALIAS ---
            RuleFor(x => x.CustomAlias)
                .MinimumLength(5).When(x => !string.IsNullOrEmpty(x.CustomAlias))
                    .WithMessage("Custom alias must be at least 5 characters.")
                .MaximumLength(30).When(x => !string.IsNullOrEmpty(x.CustomAlias))
                    .WithMessage("Custom alias must be 30 characters or less.")
                .Matches("^[a-zA-Z0-9_-]*$").When(x => !string.IsNullOrEmpty(x.CustomAlias)) // Allows letters, numbers, dash, underscore
                    .WithMessage("Alias can only contain letters, numbers, dashes, and underscores.")
                .MustAsync(BeUniqueAlias).When(x => !string.IsNullOrEmpty(x.CustomAlias))
                    .WithMessage("This custom alias is already taken. Please choose another.");
        }

        private bool BeAValidUrl(string longUrl)
        {
            return Uri.IsWellFormedUriString(longUrl, UriKind.Absolute);
        }

        