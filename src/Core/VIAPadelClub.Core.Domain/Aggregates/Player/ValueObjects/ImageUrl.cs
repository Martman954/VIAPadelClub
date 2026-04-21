using System;
using VIAPadelClub.Core.Tools.OperationResult.Results;
using VIAPadelClub.Core.Tools.OperationResult.Results.Errors;

namespace VIAPadelClub.Core.Domain.Aggregates.Player.ValueObjects;

public class ImageUrl
{
    public string Value { get; }

    private ImageUrl(string value)
    {
        Value = value;
    }

    public static Result<ImageUrl> CreateImageUrl(string value)
        =>
            Result.Combine(
                UrlMustBeInCorrectFormat(value)
            ).WithSuccessPayload(
                new ImageUrl(value)
            );

    private static Result<None> UrlMustBeInCorrectFormat(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result.Failure("Image URL is empty", ErrorType.Validation);

        if (!Uri.TryCreate(value, UriKind.Absolute, out var uri))
            return Result.Failure("Invalid URL format", ErrorType.Validation);

        if (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps)
            return Result.Failure("URL must be HTTP or HTTPS", ErrorType.Validation);

        if (!HasValidImageExtension(uri.AbsolutePath))
            return Result.Failure("URL must point to an image", ErrorType.Validation);

        return Result.Success();
    }

    private static bool HasValidImageExtension(string path)
    {
        return path.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
               path.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase) ||
               path.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ||
               path.EndsWith(".gif", StringComparison.OrdinalIgnoreCase) ||
               path.EndsWith(".webp", StringComparison.OrdinalIgnoreCase);
    }
}