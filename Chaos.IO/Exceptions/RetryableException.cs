namespace Chaos.IO.Exceptions;

/// <summary>
///     An exception that can be retried
/// </summary>
public class RetryableException : Exception
{
    public RetryableException(string message, Exception? innerException = null)
        : base(message, innerException)
    {
    }
}