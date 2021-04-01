using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using iBook.tj.Areas.Identity.Data;
using iBook.tj.Models;

namespace iBook.tj.Db
{
    public class DatabaseContext : IdentityDbContext
    {
        public DatabaseContext(DbContextOptions options) : base(options)
        {

        }
        public DbSet<Book> Books { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<iBooktjUser> iBooktjUsers { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<CartItemDetail> CartItemDetails { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Category>()
                .HasData(
                    new Category() { Id = 1 , CategoryName = "Фантастика"},
                    new Category() { Id = 2, CategoryName = "Бизнес"},
                    new Category() { Id = 3, CategoryName = "Психология"}
                );
        }
    }
}
