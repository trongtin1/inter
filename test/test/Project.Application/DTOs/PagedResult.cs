public class PagedResult<T>
{
    public IEnumerable<T> Items { get; set; }
    public int PageIndex { get; set; }
    public int TotalItems { get; set; }
    public int PageSize { get; set; }
    public int TotalPages 
    { 
        get 
        {
            if (PageSize == 0) return 0;
            return (int)Math.Ceiling(TotalItems / (double)PageSize);
        }
    }
    public bool HasPreviousPage => PageIndex > 1;
    public bool HasNextPage => PageIndex < TotalPages;
} 