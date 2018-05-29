using Microsoft.EntityFrameworkCore;
using SpyStore.Models.Entities;

namespace SpyStore.DAL.EF
{
    public class StoreContext : DbContext
    {
        public StoreContext()
        {
        }

        public StoreContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ShoppingCartRecord> ShoppingCartRecords { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured) optionsBuilder.UseSqlite("Filename=SpyStoreDAL.sqlite");
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            const string defDateFunction = "date('now')";
            const string dateTimeType = "datetime";


            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasIndex(e => e.EmailAddress).HasName("IX_Customer").IsUnique();
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.Property(e => e.OrderDate).HasColumnType(dateTimeType).HasDefaultValueSql(defDateFunction);
                entity.Property(e => e.ShipDate).HasColumnType(dateTimeType).HasDefaultValueSql(defDateFunction);
            });

            modelBuilder.Entity<ShoppingCartRecord>(entity =>
            {
                entity.HasIndex(e => new {ShoppingCartRecordId = e.Id, e.ProductId, e.CustomerId})
                    .HasName("IX_ShoppingCart")
                    .IsUnique();
                entity.Property(e => e.DateCreated)
                    .HasColumnType(dateTimeType)
                    .HasDefaultValueSql(defDateFunction);
                entity.Property(e => e.Quantity).HasDefaultValue(1);
            });
        }
    }
}