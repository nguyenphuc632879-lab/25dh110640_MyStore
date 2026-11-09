using System;
using System.Collections.Generic;
using MyStore.Models;
using PagedList;

namespace MyStore.Models.ViewModel
{
    public class HomeProductVM
    {
        public string SearchTerm { get; set; }

        public List<Product> FeaturedProducts { get; set; }

        public IPagedList<Product> NewProducts { get; set; }

        public HomeProductVM()
        {
            FeaturedProducts = new List<Product>();
        }
    }
}
