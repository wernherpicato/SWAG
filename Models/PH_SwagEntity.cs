using Microsoft.AspNetCore.Identity.UI.Services;
using PH_Swag.Controllers;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;

using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.EntityFrameworkCore;




namespace PH_Swag.Models
{
    public partial class PH_SwagEntity : DbContext
    {
        //public PH_SwagEntity() : base("name=PH_SwagEntity")
        //{
        //}
        //public PH_SwagEntity()
        //{
            
        //}
        public PH_SwagEntity(DbContextOptions<PH_SwagEntity> options) : base(options)
        {

        }

        public DbSet<Product> Products { get; set; }
        public DbSet<UserAccess> UserAccesses { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Image> Images { get; set; }

        public DbSet<Size> Sizes { get; set; }

        public DbSet<Cart> Cart { set; get; }
        public DbSet<CartItem> CartItem { set; get; }


        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseSqlServer(System.Configuration.ConfigurationManager.GetConnectionString("DevConnection"));
        //}





        //private readonly PH_Swag.Models.PH_SwagEntity db = new PH_Swag.Models.PH_SwagEntity();
        //public PH_Swag.Models.UserAccess usr;


    }
}
