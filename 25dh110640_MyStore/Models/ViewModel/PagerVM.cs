using System;

namespace _25dh110640_MyStore.Models.ViewModel
{
    public class PagerVM
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }

        public int TotalPages
        {
            get
            {
                if (PageSize <= 0)
                {
                    return 0;
                }
                return (int)Math.Ceiling((double)TotalItems / PageSize);
            }
        }
    }
}
