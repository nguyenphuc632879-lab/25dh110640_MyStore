using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace _25dh110640_MyStore.Models.ViewModel
{
    public class ProductSearchVM : PagerVM
    {
        public string SearchTerm { get; set; }

        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }

        public string SortOrder { get; set; }

        public List<Product> Products { get; set; }
    }
}
