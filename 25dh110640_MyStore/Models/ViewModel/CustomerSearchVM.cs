using System;
using MyStore.Models;
using PagedList;

namespace MyStore.Models.ViewModel
{
    public class CustomerSearchVM
    {
        public string SearchTerm { get; set; }

        public string SortOrder { get; set; }

        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public IPagedList<Customer> Customers { get; set; }

        public CustomerSearchVM()
        {
            PageNumber = 1;
            PageSize = 10;
        }
    }
}
