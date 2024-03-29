using GoodHamburger.Models.Product;

namespace GoodHamburger.ViewModels.Order
{
    public class OrderViewModel
    {
        public int OrderId { get; set; }
        public List<Sandwich> sandwich { get; set; }
        public List<Extra> extra { get; set; }
    }
}
