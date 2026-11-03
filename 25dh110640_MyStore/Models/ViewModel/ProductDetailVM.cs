using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MyStore.Models;
using PagedList;

namespace MyStore.Models.ViewModel
{
    public class ProductDetailVM
    {
        public Product product { get; set; }

        public List<Product> RelatedProducts { get; set; }
        
        public IPagedList<Product> TopProducts { get; set; }

 
        public int SoldCount { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0")]
        [Display(Name = "Số lượng")]
        public int quantity { get; set; }

        public ProductDetailVM()
        {
            RelatedProducts = new List<Product>();
            quantity = 1;
        }
    }
}
