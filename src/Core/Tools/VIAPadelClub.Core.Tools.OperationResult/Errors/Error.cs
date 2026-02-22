using VIAPadelClub.Core.Tools.OperationResult.Errors;

namespace VIAPadelClub.Core.Tools.OperationResult.OperationResult.Errors;

/// <summary>
/// Represents a specific failure that occurred during an operation.
/// Fulfills the requirement for errors to have a Code/ID and a Message.
/// </summary>
/// <param name="Code">A unique string identifier for the error (e.g., "Registration.EmailTaken").</param>
/// <param name="Message">A human-readable description of what went wrong.</param>
/// <param name="Type">The category of error, used to determine the severity or response type.</param>
public record Error(string Code, string Message, ErrorType Type = ErrorType.Failure)
{
    public static readonly Error None = new(string.Empty, string.Empty, ErrorType.None);
}