namespace Shared.Results;

public static class ExceptionExtensions
{
    public static ExceptionData AsExceptionData(this Exception exc)
        => new(exc.GetType()!.FullName!,
               exc.Message,
               exc.StackTrace ?? string.Empty,
               exc.TargetSite?.Name ?? string.Empty);
}

public class ExceptionData
{
    public string ExceptionType { get; set; }
    public string Message { get; set; }
    public string? StackTrace { get; set; }
    public string? TargetSite { get; set; }

    public ExceptionData(string exceptionType, string message, string stackTrace, string targetSite)
    {
        ExceptionType = exceptionType;
        Message = message;
        StackTrace = stackTrace;
        TargetSite = targetSite;
    }
}