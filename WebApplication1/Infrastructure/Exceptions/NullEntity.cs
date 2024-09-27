namespace Infrastructure.Exceptions
{
    public class NullEntity : Exception
    {
        public NullEntity() { }
        public NullEntity(string errorMessage) : base(errorMessage) { }
        public NullEntity(string errorMessage, Exception innerException) : base(errorMessage, innerException) { }
        public NullEntity(long entityId, string entityName) : base(FormattableString.Invariant($"'{entityName}' with id '{entityId}' was not found.")) { }
    }
}
