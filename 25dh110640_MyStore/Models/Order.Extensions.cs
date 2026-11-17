using System.ComponentModel.DataAnnotations.Schema;

namespace _25dh110640_MyStore.Models
{
    // Các thuộc tính bổ sung chỉ phục vụ giao diện/luồng xử lý.
    // NotMapped bảo đảm Entity Framework không yêu cầu cột tương ứng trong database.
    public partial class Order
    {
        [NotMapped]
        public string PaymentMethod { get; set; }

        [NotMapped]
        public string ShippingMethod { get; set; }

        [NotMapped]
        public string ShippingAddress
        {
            get { return AddressDelivery; }
            set { AddressDelivery = value; }
        }
    }

    public partial class OrderDetail
    {
        [NotMapped]
        public decimal TotalPrice
        {
            get { return UnitPrice * Quantity; }
        }
    }
}
