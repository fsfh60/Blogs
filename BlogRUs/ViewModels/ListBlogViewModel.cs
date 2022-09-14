using System.Collections.Generic;

namespace BlogRUs.ViewModels
{
    public class ListBlogViewModel
    {
        public int? PageNumber { get; set; }
        public int? PageSize { get; set; }

        public int? CurrentPage { get; set; }

        public int TotalPages { get; set; }
        public string Searching { get; set; }



        public IEnumerable<DisplayBlogViewModel> Records { get; set; }
        public int RecordsCount { get; internal set; }
    }
}
