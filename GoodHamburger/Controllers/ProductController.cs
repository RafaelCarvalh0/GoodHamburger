using GoodHamburger.Models.Product;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GoodHamburger.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly ILogger<ProductController> _logger;
        private readonly ApplicationDbContext _context;

        public ProductController(ILogger<ProductController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        // GET: api/Product/GetProducts
        /// <summary>
        /// Retrieves all products.
        /// </summary>
        /// <param></param>
        /// <returns>Returns a list of all sandwiches and extras.</returns>
        /// <response code="200">Ok</response>
        /// <response code="204">No Content</response>
        /// <response code="500">Internal Server Error</response>
        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<Products>> GetProducts()
        {
            try
            {
                Products products = new Products();

                products.sandwiches = _context.Sandwich.ToList();
                products.extras = _context.Extra.ToList();           

                if(products is not null)
                {
                    return Ok(products);
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while fetching products: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
            }
        }

        // GET: api/Product/GetSandwich
        /// <summary>
        /// Retrieves all sandwiches.
        /// </summary>
        /// <param></param>
        /// <returns>Returns a list of sandwiches.</returns>
        /// <response code="200">Ok</response>
        /// <response code="204">No Content</response>
        /// <response code="500">Internal Server Error</response>
        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<IEnumerable<Sandwich>>> GetSandwich()
        {
            try
            {
                var sandwich = _context.Sandwich.ToList();

                if (sandwich is not null)
                {
                    return Ok(sandwich);
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while fetching sandwiches: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
            }
        }

        // GET: api/Product/GetExtra
        /// <summary>
        /// Retrieves all extras.
        /// </summary>
        /// <param></param>
        /// <returns>Returns a list of extras.</returns>
        /// <response code="200">Ok</response>
        /// <response code="204">No Content</response>
        /// <response code="500">Internal Server Error</response>
        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<IEnumerable<Extra>>> GetExtra()
        {
            try
            {
                var extra = _context.Extra.ToList();

                if (extra is not null)
                {
                    return Ok(extra);
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while fetching extras: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
            }
        }
    }
}
