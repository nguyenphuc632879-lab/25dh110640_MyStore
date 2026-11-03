using System;
using System.Collections.Generic;
using MyStore.Models;
using PagedList;

namespace MyStore.Models.ViewModel
{
    public class MyOrderVM
    {
        public string SearchTerm { get; set; }
        
        public string Status { get; set; }

        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public IPagedList<Order> Orders { get; set; }

        public MyOrderVM()
        {
            PageNumber = 1;
            PageSize = 5;
        }
    }
}
