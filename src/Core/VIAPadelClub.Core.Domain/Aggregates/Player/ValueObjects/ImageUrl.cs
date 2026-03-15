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
    
    public static Result<ImageUrl> CreateImageUrl(
        string value)
        =>
            Result.Combine(
                UrlMustBeInCorrectFormat(value)
            ).WithSuccessPayload(new ImageUrl(value));
        
    
        
    

    
    //  Can add more validation
    private static Result<None> UrlMustBeInCorrectFormat(string value) =>
        (string.IsNullOrWhiteSpace(value))
            ? Result.Failure("Image URL not in correct format", ErrorType.Validation)
            : Result.Success();

}