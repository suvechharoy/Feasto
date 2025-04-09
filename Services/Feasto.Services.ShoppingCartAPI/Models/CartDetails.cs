using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Feasto.Services.ShoppingCartAPI.Models.DTO;

namespace Feasto.Services.ShoppingCartAPI.Models;

public class CartDetails
{
    [Key]
    public int CartDetailsId { get; set; }

    public int CartHeaderId { get; set; } // Foreign Key

    [ForeignKey("CartHeaderId")] // Applied to the foreign key property, referencing the navigation property
    public CartHeader CartHeader { get; set; } // Navigation Property

    public int ProductId { get; set; }

    [NotMapped]
    public ProductDTO Product { get; set; }

    public int Count { get; set; }
}