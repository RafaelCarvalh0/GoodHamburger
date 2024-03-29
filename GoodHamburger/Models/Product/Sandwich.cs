using System.ComponentModel.DataAnnotations;

namespace GoodHamburger.Models.Product
{
    public class Sandwich
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
    }
}
