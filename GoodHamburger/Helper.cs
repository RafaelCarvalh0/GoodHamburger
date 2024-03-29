using GoodHamburger.Models.Order;
using GoodHamburger.Models.Product;
using Microsoft.EntityFrameworkCore;

namespace GoodHamburger
{
    public static class Helper
    {
        public async static Task<bool> OrderValidator(ApplicationDbContext _context, Products products)
        {
            try
            {
                if (products.sandwiches.Count > 1)
                    throw new Exception("You should choose only one sandwich!");

                else if (products.extras.Count > 2)
                    throw new Exception("You should choose only two extra!");

                else if (products.extras.GroupBy(e => e.Id).Any(e => e.Count() > 1))
                    throw new Exception("You cannot select more than one soda or one fries!");
              
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            
        }
    }
}
