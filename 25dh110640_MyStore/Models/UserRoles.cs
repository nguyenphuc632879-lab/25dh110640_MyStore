namespace MyStore.Models
{
    /// <summary>
    /// Giá trị của cột User.UserRole theo file database2024.sql:
    ///     UserRole NCHAR(1) NOT NULL  -- 'A': admin, 'C': customer
    /// Dùng class này để tránh gõ nhầm chuỗi trong code.
    /// </summary>
    public static class UserRoles
    {
        public const string Admin = "A";
        public const string Customer = "C";

        /// <summary>Đổi mã 'A'/'C' thành tên hiển thị</summary>
        public static string GetDisplayName(string role)
        {
            if (role == null) return "";
            role = role.Trim();
            if (role == Admin) return "Admin";
            if (role == Customer) return "Customer";
            return role;
        }
    }
}
