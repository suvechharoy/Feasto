using System.ComponentModel.DataAnnotations;

namespace Feasto.Services.CouponAPI.Models;

public class Coupon
{
    [Key]
    public int CouponId { get; set; }
    [Required]
    public string CouponCode { get; set; }
    [Required]
    public double DiscountAmount { get; set; } //percentage discount
    public int MinAmount { get; set; } //min purchase amount after which the coupon should be applied   
}