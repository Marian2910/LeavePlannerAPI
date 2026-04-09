namespace LeavePlanner.Infrastructure.Exceptions
{
    public abstract class NotNullEntityException(string errorMessage, Exception innerException)
        : Exception(errorMessage, innerException);
}
