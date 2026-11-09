using System;
using MyStore.Models;
using PagedList;

namespace MyStore.Models.ViewModel
{
    public class AdminUserSearchVM
    {
        public string SearchTerm { get; set; }

        public string SortOrder { get; set; }

        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public IPagedList<User> AdminUsers { get; set; }

        public AdminUserSearchVM()
        {
            PageNumber = 1;
            PageSize = 10;
        }
    }
}
