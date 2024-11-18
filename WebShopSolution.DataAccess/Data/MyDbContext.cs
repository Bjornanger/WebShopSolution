using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebShopSolution.DataAccess.Entities;

namespace WebShopSolution.DataAccess.Data
{
    public class MyDbContext : DbContext
    {

        DbSet<Product> Products { get; set; }
        DbSet<Order> Orders { get; set; }
        DbSet<Customer> Customers { get; set; }


      public MyDbContext(DbContextOptions<MyDbContext> options) : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>().HasKey(p => p.Id);
            modelBuilder.Entity<Order>().HasKey(o => o.Id);
            modelBuilder.Entity<Customer>().HasKey(c => c.Id);


            modelBuilder.Entity<Order>().HasOne(o => o.Customer).WithMany(c => c.Orders).HasForeignKey(o => o.CustomerId);

            modelBuilder.Entity<OrderItem>().HasKey(op => new { op.OrderId, op.ProductId });

            modelBuilder.Entity<OrderItem>().HasOne(op => op.Order).WithMany(o => o.OrderProducts).HasForeignKey(op => op.OrderId);

            modelBuilder.Entity<OrderItem>().HasOne(op => op.Product).WithMany(p => p.OrderProducts).HasForeignKey(op => op.ProductId);


            base.OnModelCreating(modelBuilder);

        }
     



    }
}
