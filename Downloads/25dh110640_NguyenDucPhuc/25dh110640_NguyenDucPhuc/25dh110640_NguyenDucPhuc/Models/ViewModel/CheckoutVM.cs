using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MyStore.Models.ViewModel;

namespace MyStore.Models.ViewModel
{
    public class CheckoutVM
    {
        [Required(ErrorMessage = "Địa chỉ giao hàng không được để trống")]
        [Display(Name = "Địa chỉ giao hàng")]
        public string ShippingAddress { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn hình thức giao hàng")]
        [Display(Name = "Hình thức giao hàng")]
        public string DeliveryMethod { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn hình thức thanh toán")]
        [Display(Name = "Hình thức thanh toán")]
        public string PaymentMethod { get; set; }

        [Display(Name = "Trạng thái thanh toán")]
        public string PaymentStatus { get; set; }

        [Display(Name = "Ngày đặt hàng")]
        public DateTime OrderDate { get; set; }

        [Display(Name = "Tổng tiền")]
        public decimal TotalAmount { get; set; }

        [Display(Name = "Họ tên")]
        public string CustomerName { get; set; }

        [Display(Name = "Số điện thoại")]
        public string CustomerPhone { get; set; }

        public List<CartItem> CartItems { get; set; }

        public List<int> SelectedIds { get; set; }

        public CheckoutVM()
        {
            CartItems = new List<CartItem>();
            SelectedIds = new List<int>();
            OrderDate = DateTime.Now;
            DeliveryMethod = "Giao hàng tiêu chuẩn";
            PaymentMethod = "Thanh toán khi nhận hàng";
        }
    }
}
