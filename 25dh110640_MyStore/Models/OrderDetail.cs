using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyStore.Models
{
    [Table("OrderDetail")]
    public partial class OrderDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Mã chi tiết đơn hàng")]
        public int ID { get; set; }

        [Display(Name = "Mã đơn hàng")]
        public int OrderID { get; set; }

        [Display(Name = "Mã sản phẩm")]
        public int ProductID { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0")]
        [Display(Name = "Số lượng")]
        public int Quantity { get; set; }

        [Display(Name = "Đơn giá")]
        [DisplayFormat(DataFormatString = "{0:#,##0}")]
        public decimal UnitPrice { get; set; }


        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [Display(Name = "Thành tiền")]
        [DisplayFormat(DataFormatString = "{0:#,##0}")]
        public decimal TotalPrice { get; set; }

        [ForeignKey("OrderID")]
        public virtual Order Order { get; set; }

        [ForeignKey("ProductID")]
        public virtual Product Product { get; set; }
    }
}
