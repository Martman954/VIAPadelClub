namespace VIAPadelClub.Core.Tools.OperationResult.OperationResult.Errors;

public record Error(string Code, string Message, ErrorType Type = ErrorType.Failure)
{
    public static readonly Error None = new(string.Empty, string.Empty, ErrorType.None);
}