using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata;
using WebShopSolution.DataAccess.Entities;
using WebShopSolution.DataAccess.UnitOfWork;

namespace WebShop.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;


        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        [HttpPost]
        public async Task<ActionResult> AddOrder([FromBody]  Order order)
        {
            if (!ModelState.IsValid)
                return BadRequest();



            try
            {

                var customer = await _unitOfWork.Repository<Customer>().GetByIdAsync(order.CustomerId);
                if (customer is null)
                {
                    return NotFound("No Customer with that ID found.");
                }

                var productsList = new List<OrderItem>();

                foreach (var orderItem in order.OrderProducts)
                {
                    var product = await _unitOfWork.Repository<Product>().GetByIdAsync(orderItem.ProductId);

                    if (product is null)
                    {
                        return NotFound("No Products found");
                    }

                    productsList.Add(new OrderItem
                    {
                        ProductId = product.Id,
                        Product = product,
                        Quantity = orderItem.Quantity

                    });
                }

                order.Customer = customer;
                order.OrderProducts = productsList;




                var orderRepository = _unitOfWork.Repository<Order>();

                orderRepository.AddAsync(order);

                await _unitOfWork.CompleteAsync();
                return Ok(order);

            }
            catch (Exception e)
            {
                _unitOfWork.Dispose();
                Console.WriteLine(e);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetAllOrders()
        {
            try
            {
                var orderRepository = _unitOfWork.Repository<Order>();
                var ordersList = await orderRepository.GetAllAsync();
                if (ordersList is null || !ordersList.Any())
                {
                    return NotFound();
                }
                return Ok(ordersList);
            }
            catch (Exception e)
            {
                _unitOfWork.Dispose();
                Console.WriteLine(e);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrderById(int id)
        {
            try
            {
                var orderRepository = _unitOfWork.Repository<Order>();
                var order = await orderRepository.GetByIdAsync(id);
                if (order is null)
                {
                    return NotFound();
                }
                return Ok(order);
            }
            catch (Exception e)
            {
                _unitOfWork.Dispose();
                Console.WriteLine(e);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut]
        public async Task<ActionResult> UpdateOrder([FromBody] Order order)
        {
            if (!ModelState.IsValid)
                return BadRequest();


            if (order is null)
                return BadRequest();


            try
            {
                var orderRepository = _unitOfWork.Repository<Order>();
                orderRepository.UpdateAsync(order);
                await _unitOfWork.CompleteAsync();
                return Ok();
            }
            catch (Exception e)
            {
                _unitOfWork.Dispose();
                Console.WriteLine(e);
                return StatusCode(500, "Internal server error");
            }
        }


        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteOrder(int id)
        {
            try
            {
                var orderRepository = _unitOfWork.Repository<Order>();
                var order = await orderRepository.GetByIdAsync(id);
                if (order is null)
                {
                    return NotFound();
                }
                orderRepository.RemoveAsync(id);
                await _unitOfWork.CompleteAsync();
                return Ok();
            }
            catch (Exception e)
            {
                _unitOfWork.Dispose();
                Console.WriteLine(e);
                return StatusCode(500, "Internal server error");
            }
        }





    }
}
