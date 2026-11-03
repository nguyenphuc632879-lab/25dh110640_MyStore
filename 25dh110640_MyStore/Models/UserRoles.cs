namespace MyStore.Models
{
    public static class UserRoles
    {
        public const string Admin = "A";
        public const string Customer = "C";
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
