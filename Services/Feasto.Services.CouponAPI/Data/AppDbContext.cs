using Feasto.Services.CouponAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace Feasto.Services.CouponAPI.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    
    public DbSet<Coupon> Coupons { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Coupon>().HasData(new Coupon
        {
            CouponId = 1,
            CouponCode = "10OFF",
            DiscountAmount = 10,
            MinAmount = 200
        });
        modelBuilder.Entity<Coupon>().HasData(new Coupon
        {
            CouponId = 2,
            CouponCode = "20OFF",
            DiscountAmount = 20,
            MinAmount = 600
        });
        modelBuilder.Entity<Coupon>().HasData(new Coupon
        {
            CouponId = 3,
            CouponCode = "10OFF",
            DiscountAmount = 30,
            MinAmount = 900
        });
    }
}