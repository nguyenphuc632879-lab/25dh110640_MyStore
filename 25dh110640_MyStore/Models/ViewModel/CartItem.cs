using System;
using System.ComponentModel.DataAnnotations;

namespace MyStore.Models.ViewModel
{
    public class CartItem
    {
        public int ProductID { get; set; }

        [Display(Name = "Tên sản phẩm")]
        public string ProductName { get; set; }

        [Display(Name = "Hình ảnh")]
        public string ProductImage { get; set; }

        [Display(Name = "Đơn giá")]
        public decimal ProductPrice { get; set; }

        [Display(Name = "Số lượng")]
        public int Quantity { get; set; }

        // Giỏ hàng nâng cao: lưu tên danh mục để gom nhóm sản phẩm trong giỏ
        public string Category { get; set; }

        [Display(Name = "Thành tiền")]
        public decimal TotalPrice
        {
            get { return ProductPrice * Quantity; }
        }
    }
}
