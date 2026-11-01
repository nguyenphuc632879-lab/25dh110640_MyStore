using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyStore.Models
{
    /// <summary>
    /// Lớp ánh xạ bảng [User] theo đúng file database2024.sql:
    ///     Username  NVARCHAR(255) NOT NULL  - khoá chính
    ///     Password  NCHAR(50)     NOT NULL
    ///     UserRole  NCHAR(1)      NOT NULL  - 'A': admin, 'C': customer
    /// </summary>
    [Table("User")]
    public partial class User
    {
        [Key]
        [StringLength(255)]
        [Display(Name = "Tên đăng nhập")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        [StringLength(50, MinimumLength = 6, ErrorMessage = "Mật khẩu từ 6 đến 50 ký tự")]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn quyền")]
        [StringLength(1)]
        [Display(Name = "Quyền")]
        public string UserRole { get; set; }

        // Lưu ý: khoá ngoại Customer.Username -> User.Username được cấu hình
        // một chiều trong MyStoreEntities.OnModelCreating (EF6 không hỗ trợ
        // khoá ngoại không phải khoá chính trong quan hệ 1-1),
        // vì vậy không khai báo navigation property Customer ở đây.
    }
}
