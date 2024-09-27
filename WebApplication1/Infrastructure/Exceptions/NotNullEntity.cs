namespace Infrastructure.Exceptions
{
    public class NotNullEntity : Exception
    {
        public NotNullEntity() { }
        public NotNullEntity(string errorMessage) : base(errorMessage) { }
        public NotNullEntity(string errorMessage, Exception innerException) : base(errorMessage, innerException) { }
    }
}
