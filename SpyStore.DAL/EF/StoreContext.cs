using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SpyStore.Models.Entities;

namespace SpyStore.DAL.EF
{
    public class StoreContext : DbContext
    {

        private void init()
        {
            var builder = new ConfigurationBuilder();
            builder.AddJsonFile("app.json", false)
                .Build();


            Configuration = builder.Build();
        }

        public StoreContext()
        {
            init();
        }

        public StoreContext(DbContextOptions options) : base(options) 
        {
            init();
        }

        public static IConfiguration Configuration { get; set; }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ShoppingCartRecord> ShoppingCartRecords { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var useDbType = Configuration["use_db_type"];

            switch (useDbType)
            {
                case "pg":
                    if (!optionsBuilder.IsConfigured)
                    {
                        Console.WriteLine("*****PG - construct*****");
                        var connString =
                            $"Host = {Configuration["pg_host"]}; Port =  {Configuration["pg_port"]}; Database =  {Configuration["pg_db"]}; Username =  {Configuration["pg_user"]}; Password =  {Configuration["pg_pass"]}";
                        optionsBuilder.UseNpgsql(connString  );
                    }
                    Console.WriteLine("*****PG*****");
                    break;

                case "sql_lite":
                    if (!optionsBuilder.IsConfigured) optionsBuilder.UseSqlite("Filename=SpyStoreDAL.sqlite");
                    break;
                default:
                    throw new ArgumentException();
            }
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            const string defDateFunction = "now()";
            const string dateTimeType = "Date";


            
            

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