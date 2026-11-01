using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MyStore.Models;
using PagedList;

namespace MyStore.Models.ViewModel
{
    /// <summary>
    /// ViewModel chứa các tiêu chí tìm kiếm + kết quả tìm kiếm sản phẩm (BE#4)
    /// Admin: Products/Index   |   Customer: Home/Index, Home/ProductList
    /// </summary>
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

        // Kết quả tìm kiếm đã phân trang (PagedList.Mvc)
        public IPagedList<Product> Products { get; set; }

        public ProductSearchVM()
        {
            PageNumber = 1;
            PageSize = 6;
        }
    }
}
