namespace VIAPadelClub.Core.Tools.OperationResult.Errors;

/// <summary>
/// Categorizes errors to define how the application should handle or display them.
/// </summary>
public enum ErrorType
{
    None = 0,
    Validation = 1,
    NotFound = 2,
    Conflict = 3,
    Failure = 4,
    Unauthorized = 5
}