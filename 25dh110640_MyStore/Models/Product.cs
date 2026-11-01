using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web;

namespace MyStore.Models
{
    /// <summary>
    /// Lớp ánh xạ bảng [Product] theo đúng file database2024.sql:
    ///     ProductID          INT IDENTITY(1,1)   - khoá chính
    ///     CategoryID         INT            NOT NULL - FK -> Category
    ///     ProductName        NVARCHAR(MAX)  NOT NULL
    ///     ProductDecription  NVARCHAR(MAX)  NOT NULL  (tên cột trong DB viết đúng như file SQL)
    ///     ProductPrice       DECIMAL(18,2)  NOT NULL
    ///     ProductImage       NVARCHAR(MAX)  NULL
    /// </summary>
    [Table("Product")]
    public partial class Product
    {
        public Product()
        {
            // Ảnh mặc định nếu không có ảnh
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

        // DB đặt tên cột là ProductDecription (theo file database2024.sql)
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

        // Thuộc tính phục vụ upload ảnh: không lưu vào DB
        [NotMapped]
        [Display(Name = "Chọn ảnh")]
        public HttpPostedFileBase UploadImg { get; set; }

        // Quan hệ n - 1 với Category
        [ForeignKey("CategoryID")]
        public virtual Category Category { get; set; }
    }
}
