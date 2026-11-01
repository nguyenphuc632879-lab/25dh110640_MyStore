using System.Data.Entity;

namespace MyStore.Models
{
    /// <summary>
    /// Lớp DbContext (tương đương MyStoreEntities sinh ra từ EDMX trong DB First).
    /// Ánh xạ đúng 6 bảng trong file Database/database2024.sql:
    /// User, Customer, Category, Product, Order, OrderDetail
    /// </summary>
    public partial class MyStoreEntities : DbContext
    {
        public MyStoreEntities() : base("name=MyStoreEntities")
        {
            // Đã có sẵn DB (DB First) -> không cho EF tự tạo/migrate
            System.Data.Entity.Database.SetInitializer<MyStoreEntities>(null);
            this.Configuration.LazyLoadingEnabled = true;
            this.Configuration.ProxyCreationEnabled = false;
        }

        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<OrderDetail> OrderDetails { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Bảng Order / User là từ khoá của SQL Server -> ép tên bảng
            modelBuilder.Entity<Order>().ToTable("Order");
            modelBuilder.Entity<User>().ToTable("User");

            // User.Username là khoá chính (NVARCHAR(255))
            modelBuilder.Entity<User>().HasKey(u => u.Username);
            modelBuilder.Entity<User>().Property(u => u.Username).HasMaxLength(255);
            // Password là NCHAR(50)
            modelBuilder.Entity<User>().Property(u => u.Password).HasMaxLength(50).IsFixedLength();
            // UserRole là NCHAR(1)
            modelBuilder.Entity<User>().Property(u => u.UserRole).HasMaxLength(1).IsFixedLength();

            // Khoá chính của Customer là CustomerID (identity)
            modelBuilder.Entity<Customer>().HasKey(c => c.CustomerID);
            modelBuilder.Entity<Customer>().Property(c => c.Username).HasMaxLength(255);
            modelBuilder.Entity<Customer>().Property(c => c.CustomerPhone).HasMaxLength(15);

            // Quan hệ User 1 - n Customer thông qua Username
            modelBuilder.Entity<Customer>()
                .HasRequired(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.Username);

            // Cột Product.ProductDecription (tên cột trong DB)
            modelBuilder.Entity<Product>().Property(p => p.ProductDescription)
                .HasColumnName("ProductDecription");

            // Quan hệ Category 1 - n Product
            modelBuilder.Entity<Product>()
                .HasRequired(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryID);

            // Quan hệ Customer 1 - n Order
            modelBuilder.Entity<Order>()
                .HasRequired(o => o.Customer)
                .WithMany(c => c.Orders)
                .HasForeignKey(o => o.CustomerID);

            // Quan hệ Order 1 - n OrderDetail
            modelBuilder.Entity<OrderDetail>()
                .HasRequired(d => d.Order)
                .WithMany(o => o.OrderDetails)
                .HasForeignKey(d => d.OrderID);

            modelBuilder.Entity<OrderDetail>()
                .HasRequired(d => d.Product)
                .WithMany()
                .HasForeignKey(d => d.ProductID);

            base.OnModelCreating(modelBuilder);
        }
    }
}
