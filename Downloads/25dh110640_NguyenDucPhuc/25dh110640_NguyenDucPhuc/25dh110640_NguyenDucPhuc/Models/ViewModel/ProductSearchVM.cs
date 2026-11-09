using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MyStore.Models;
using PagedList;

namespace MyStore.Models.ViewModel
{
    public class ProductSearchVM
    {
        [Display(Name = "Từ khoá")]
        public string SearchTerm { get; set; }

        [Display(Name = "Giá từ")]
        [DisplayFormat(DataFormatString = "{0:#,##0}")]
        public decimal? MinPrice { get; set; }

        [Display(Name = "Giá đến")]
        [DisplayFormat(DataFormatString = "{0:#,##0}")]
        public decimal? MaxPrice { get; set; }

        [Display(Name = "Sắp xếp")]
        public string SortOrder { get; set; }

        [Display(Name = "Mã danh mục")]
        public int? CategoryID { get; set; }

        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public IPagedList<Product> Products { get; set; }

        public ProductSearchVM()
        {
            PageNumber = 1;
            PageSize = 6;
        }
    }
}
