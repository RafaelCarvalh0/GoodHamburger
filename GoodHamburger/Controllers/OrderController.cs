using GoodHamburger.Models.Order;
using GoodHamburger.Models.Product;
using GoodHamburger.ViewModels.Order;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GoodHamburger.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly ILogger<OrderController> _logger;
        private readonly ApplicationDbContext _context;

        public OrderController(ILogger<OrderController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        // POST: api/Order/SendOrder
        /// <summary>
        /// Creates an order.
        /// </summary>
        /// <param></param>
        /// <returns>Return the amount that will be charged to the customer</returns>
        /// <response code="200">Ok</response>
        /// <response code="400">Bad Request</response>
        /// <response code="500">Internal Server Error</response>
        [HttpPost]
        [Route("[action]")]
        public async Task<ActionResult> SendOrder(Products products)
        {
            try
            {
                bool IsValidOrder = await Helper.OrderValidator(_context, products);

                if (IsValidOrder)
                {
                    var sandwich = await _context.Sandwich.FindAsync(products.sandwiches.First().Id);
                    if (sandwich is null)
                        throw new Exception("Invalid sandwich");

                    var extras = await _context.Extra.Where(e => products.extras.Select(extra => extra.Id).Contains(e.Id)).ToListAsync();
                    if (extras.Count == 0)
                        throw new Exception("Invalid extra");

                    decimal SelectedSandwich = sandwich.Price;
                    decimal SelectedExtra = extras.Sum(extra => extra.Price);

                    decimal total;
                    if (extras.Count == 2)
                        total = (SelectedSandwich + SelectedExtra) * 0.8M;

                    else if (extras.First().Id == 1)
                        total = (SelectedSandwich + SelectedExtra) * 0.9M;

                    else
                        total = (SelectedSandwich + SelectedExtra) * 0.85M;


                    var order = new Order();
                    _context.Order.Add(order);
                    await _context.SaveChangesAsync();

                    int orderId = order.Id;

                    foreach (var extra in products.extras)
                    {
                        var orderItem = new OrderItem
                        {
                            OrderId = orderId,
                            SandwichId = products.sandwiches.First().Id,
                            ExtraId = extra.Id,
                        };
                        _context.OrderItem.Add(orderItem);
                    }
                    await _context.SaveChangesAsync();

                    return Ok(total);
                }

                return BadRequest();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while creating the order: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET: api/Order/GetOrder
        /// <summary>
        /// Retrieves all orders.
        /// </summary>
        /// <param></param>
        /// <returns>Returns a list of orders.</returns>
        /// <response code="200">Ok</response>
        /// <response code="500">Internal Server Error</response>
        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult> GetOrder()
        {
            try
            {
                var orderIds = await _context.Order.Select(o => o.Id).ToListAsync();

                List<OrderViewModel> orderList = new List<OrderViewModel>();

                foreach (var id in orderIds)
                {
                    var sandwichId = _context.OrderItem.Where(oi => oi.OrderId == id).Select(oi => oi.SandwichId).First();
                    var extraIds = await _context.OrderItem.Where(oi => oi.OrderId == id).Select(oi => oi.ExtraId).ToListAsync();
                    var sandwich = await _context.Sandwich.Where(s => s.Id == sandwichId).Distinct().ToListAsync();
                    var extra = await _context.Extra.Where(e => extraIds.Contains(e.Id)).ToListAsync();

                    OrderViewModel order = new OrderViewModel()
                    {
                        OrderId = id,
                        sandwich = sandwich,
                        extra = extra
                    };

                    orderList.Add(order);
                }

                return Ok(orderList);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while fetching orders: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
            }
        }

        // PUT: api/Order/UpdateOrder/OrderId
        /// <summary>
        /// Updates an order.
        /// </summary>
        /// <param name="OrderId"></param>
        /// <returns>Returns true if the order was successfully updated; otherwise, false.</returns>
        /// <response code="200">Ok</response>
        /// <response code="400">Bad Request</response>
        /// <response code="404">Not Found</response>
        /// <response code="500">Internal Server Error</response>
        [HttpPut]
        [Route("[action]/{OrderId}")]
        public async Task<ActionResult> UpdateOrder(int OrderId, Products products)
        {
            try
            {
                var Id = await _context.Order.FindAsync(OrderId);
                if (Id is null)
                    return NotFound("Order not found");

                bool IsValidOrder = await Helper.OrderValidator(_context, products);

                if (IsValidOrder)
                {
                    var sandwich = await _context.Sandwich.FindAsync(products.sandwiches.First().Id);
                    if (sandwich is null)
                        return BadRequest("Invalid sandwich");//throw new Exception("Invalid sandwich");

                    var extras = await _context.Extra.Where(e => products.extras.Select(extra => extra.Id).Contains(e.Id)).ToListAsync();
                    if (extras.Count == 0)
                        return BadRequest("Invalid extra"); //throw new Exception("Invalid extra");


                    var existingOrderItems = await _context.OrderItem.Where(oi => oi.OrderId == OrderId).ToListAsync();
                    _context.OrderItem.RemoveRange(existingOrderItems);

                    foreach (var extra in products.extras)
                    {
                        var orderItem = new OrderItem
                        {
                            OrderId = OrderId,
                            SandwichId = products.sandwiches.First().Id,
                            ExtraId = extra.Id,
                        };
                        _context.OrderItem.Add(orderItem);
                    }

                    await _context.SaveChangesAsync();


                    return Ok("Order updated successfully");
                }

                return BadRequest();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while updating the order: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
            }
        }

        // DELETE: api/Order/RemoveOrder/OrderId
        /// <summary>
        /// Removes an order.
        /// </summary>
        /// <param name="OrderId">The ID of the order to be removed.</param>
        /// <returns>Returns true if the order was successfully removed; otherwise, false.</returns>
        /// <response code="200">Ok</response>
        /// <response code="404">Not Found</response>
        /// <response code="500">Internal Server Error</response>
        [HttpDelete]
        [Route("[action]/{OrderId}")]
        public async Task<ActionResult> RemoveOrder(int OrderId)
        {
            try
            {
                var existingOrder = await _context.Order.FindAsync(OrderId);
                if (existingOrder == null)
                    return NotFound("Order not found");

                _context.Order.Remove(existingOrder);

                await _context.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while removing the order: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
            }
        }
    }
}
