using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MyStore.Models;
using PagedList;

namespace MyStore.Models.ViewModel
{
    /// <summary>
    /// ViewModel trang Chi tiết sản phẩm (BE#6)
    /// </summary>
    public class ProductDetailVM
    {
        public Product product { get; set; }

        // 8 sản phẩm cùng danh mục
        public List<Product> RelatedProducts { get; set; }

        // 8 sản phẩm bán chạy nhất cùng danh mục (phân trang 3 sản phẩm / trang)
        public IPagedList<Product> TopProducts { get; set; }

        // Số lượng đã bán của sản phẩm đang xem
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
