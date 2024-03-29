using System.ComponentModel.DataAnnotations;

namespace GoodHamburger.Models.Product
{
    public class Extra
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
    }
}
