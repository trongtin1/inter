using test.Project.Domain.Entity;

namespace test.Project.Application.DTOs.Response.Mails
{
    public class MailRes
    {
        public IEnumerable<Mail> Items { get; set; } = new List<Mail>();
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
}
