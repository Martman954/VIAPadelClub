namespace VIAPadelClub.Core.Tools.OperationResult.Results.Errors;

public sealed record ResultError(string Message, ErrorType errorType)
{
    public static readonly ResultError None = new( string.Empty, ErrorType.None);
}