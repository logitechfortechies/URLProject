using FluentValidation;
using UrlShortener.Application;

namespace UrlShortener.Application.Validators
{
    // This validator is for your 'CreateShortUrlRequest' record
    public class CreateShortUrlRequestValidator : AbstractValidator<CreateShortUrlRequest>
    {
        public CreateShortUrlRequestValidator()
        {
            // This is the validation rule
            RuleFor(x => x.LongUrl)
                .NotEmpty().WithMessage("URL must not be empty.")
                .Must(BeAValidUrl).WithMessage("A valid URL is required.");
        }

        // This is a custom function to check if the URL is valid
        private bool BeAValidUrl(string longUrl)
        {
            return Uri.IsWellFormedUriString(longUrl, UriKind.Absolute);
        }
    }
}