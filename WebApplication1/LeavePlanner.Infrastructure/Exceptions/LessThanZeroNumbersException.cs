namespace LeavePlanner.Infrastructure.Exceptions
{
    public class LessThanZeroNumbersException(string errorMessage) : Exception(errorMessage);
}
