using System.Data.Entity;

namespace MyStore.Models
{
    public partial class MyStoreEntities : DbContext
    {
        public MyStoreEntities() : base("name=MyStoreEntities")
        {
            System.Data.Entity.Database.SetInitializer<MyStoreEntities>(null);
            this.Configuration.LazyLoadingEnabled = true;
            this.Configuration.ProxyCreationEnabled = true;
        }

        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<OrderDetail> OrderDetails { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Order>().ToTable("Order");
            modelBuilder.Entity<User>().ToTable("User");

            modelBuilder.Entity<User>().HasKey(u => u.Username);
            modelBuilder.Entity<User>().Property(u => u.Username).HasMaxLength(255);
            modelBuilder.Entity<User>().Property(u => u.Password).HasMaxLength(50).IsFixedLength();
            modelBuilder.Entity<User>().Property(u => u.UserRole).HasMaxLength(1).IsFixedLength();

            modelBuilder.Entity<Customer>().HasKey(c => c.CustomerID);
            modelBuilder.Entity<Customer>().Property(c => c.Username).HasMaxLength(255);
            modelBuilder.Entity<Customer>().Property(c => c.CustomerPhone).HasMaxLength(15);

            modelBuilder.Entity<Customer>()
                .HasRequired(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.Username);

            modelBuilder.Entity<Product>().Property(p => p.ProductDescription)
                .HasColumnName("ProductDecription");

            modelBuilder.Entity<Product>()
                .HasRequired(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryID);

            modelBuilder.Entity<Order>()
                .HasRequired(o => o.Customer)
                .WithMany(c => c.Orders)
                .HasForeignKey(o => o.CustomerID);

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
