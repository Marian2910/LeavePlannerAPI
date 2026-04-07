namespace Common.DTOs
{
    public class PagedResultDto<T>
    {
        public required IEnumerable<T> Items { get; set; } = Enumerable.Empty<T>();

        public required int TotalCount { get; set; }
    }
}