using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Feasto.Services.OrderAPI.Models.DTO;

namespace Feasto.Services.OrderAPI.Models;

public class OrderDetails
{
    [Key]
    public int OrderDetailId { get; set; }
    public int OrderHeaderId { get; set; }
    [ForeignKey("OrderHeaderId")]
    public OrderHeader? OrderHeader { get; set; } // Navigation Property
    public int ProductId { get; set; }
    [NotMapped]
    public ProductDTO? Product { get; set; }
    public int Count { get; set; }
    public string ProductName { get; set; }
    public double Price { get; set; }
}