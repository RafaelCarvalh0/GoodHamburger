using GoodHamburger.Models.Product;
using System.ComponentModel.DataAnnotations;

namespace GoodHamburger.Models.Order
{
    public class Order
    {
        [Key]
        public int Id { get; set; }
    }

    public class OrderItem
    {
        [Key]
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int SandwichId { get; set; }
        public int ExtraId { get; set; }
    }
}
