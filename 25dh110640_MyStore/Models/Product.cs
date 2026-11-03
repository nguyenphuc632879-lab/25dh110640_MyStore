using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web;

namespace MyStore.Models
{
    [Table("Product")]
    public partial class Product
    {
        public Product()
        {

            ProductImage = "~/Content/images/default.png";
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Mã sản phẩm")]
        public int ProductID { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn danh mục")]
        [Display(Name = "Danh mục")]
        public int CategoryID { get; set; }

        [Required(ErrorMessage = "Tên sản phẩm không được để trống")]
        [Display(Name = "Tên sản phẩm")]
        public string ProductName { get; set; }


        [Required(ErrorMessage = "Mô tả sản phẩm không được để trống")]
        [Column("ProductDecription")]
        [Display(Name = "Mô tả")]
        [DataType(DataType.MultilineText)]
        public string ProductDescription { get; set; }

        [Required(ErrorMessage = "Giá sản phẩm không được để trống")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá sản phẩm phải lớn hơn hoặc bằng 0")]
        [Display(Name = "Giá sản phẩm")]
        [DisplayFormat(DataFormatString = "{0:#,##0}")]
        public decimal ProductPrice { get; set; }

        [Display(Name = "Hình ảnh")]
        public string ProductImage { get; set; }

    
        [NotMapped]
        [Display(Name = "Chọn ảnh")]
        public HttpPostedFileBase UploadImg { get; set; }

    
        [ForeignKey("CategoryID")]
        public virtual Category Category { get; set; }
    }
}
