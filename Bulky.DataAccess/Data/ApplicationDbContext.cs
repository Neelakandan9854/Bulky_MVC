using Bulky.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Bulky.DataAccess.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) 
        {

        }
        public DbSet<Category> Categories{ get; set; }
        public DbSet<Product> Products { get; set; }

        public DbSet<Company> Companies { get; set; }

       public DbSet<Carts> Cart { get; set; }
       public DbSet<OrderHead> OrderHeader { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }

        public DbSet<ApplicationUser>ApplicationUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Category>().HasData(
                new Category {Id=1,Name="Bike",DisplayOrder=1},
                new Category { Id = 2, Name = "Car", DisplayOrder = 3 }

                );
            modelBuilder.Entity<Product>().HasData(
                new Product { 
                    id = 1,
                    Tittle = "Bike",
                    Author = "welcome",
                    Description="new arrival",
                    ISBN="12345678",
                    Listprice=100,
                    price=20,
                    price50=60,
                    price100=100,
                    CategoryId=8,
                    ImageUrl=""
                },
                new Product
                {
                    id = 2,
                    Tittle = "Bike1",
                    Author = "welcome2",
                    Description = "old arrival",
                    ISBN = "12345678",
                    Listprice = 100,
                    price = 20,
                    price50 = 60,
                    price100 = 100,
                    CategoryId=9,
                    ImageUrl = ""
                }


                );
            modelBuilder.Entity<Company>().HasData(
               new Company
               {
                   Id = 1,
                   City = "Chennai",
                   StreetAdress = "Chennai",
                   State = "Tamilnadu",
                   Name = "Kandan",
                   PhoneNumber = "9677445054",
                   PostelCode = "30270",
                  
               },
               new Company
               {
                   Id = 2,
                   City = "Chennai",
                   StreetAdress = "Chennai",
                   State = "Tamilnadu",
                   Name = "Kandan123",
                   PhoneNumber = "9677445054",
                   PostelCode = "30270",
               }


               );




        }
    }
}
