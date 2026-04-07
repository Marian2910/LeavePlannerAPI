namespace LeavePlanner.Infrastructure.Exceptions
{
    public abstract class NotNullEntity(string errorMessage, Exception innerException)
        : Exception(errorMessage, innerException);
}
