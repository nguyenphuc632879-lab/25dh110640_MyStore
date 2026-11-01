using System;
using System.Collections.Generic;
using MyStore.Models;
using PagedList;

namespace MyStore.Models.ViewModel
{
    /// <summary>
    /// ViewModel trang "Đơn hàng của tôi" (BE#7 - Tra cứu lịch sử mua hàng)
    /// </summary>
    public class MyOrderVM
    {
        // Tìm theo mã đơn hàng hoặc tên sản phẩm
        public string SearchTerm { get; set; }

        // Trạng thái đang chọn: Tất cả | Chờ thanh toán | Đang xử lý | Đang vận chuyển | Đã giao | Đã huỷ
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
