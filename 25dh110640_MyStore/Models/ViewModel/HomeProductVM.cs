using System;
using System.Collections.Generic;
using MyStore.Models;
using PagedList;

namespace MyStore.Models.ViewModel
{
    /// <summary>
    /// ViewModel trang Home của Customer (BE#6):
    /// - FeaturedProducts: 10 sản phẩm bán chạy nhất
    /// - NewProducts     : 20 sản phẩm mới (ít người mua), phân trang 6 sản phẩm / trang
    /// </summary>
    public class HomeProductVM
    {
        public string SearchTerm { get; set; }

        // Sản phẩm nổi bật
        public List<Product> FeaturedProducts { get; set; }

        // Danh sách sản phẩm (đã phân trang)
        public IPagedList<Product> NewProducts { get; set; }

        public HomeProductVM()
        {
            FeaturedProducts = new List<Product>();
        }
    }
}
