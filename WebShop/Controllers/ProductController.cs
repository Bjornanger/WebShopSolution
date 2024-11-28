using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc;
using WebShopSolution.DataAccess.Entities;
using WebShopSolution.DataAccess.Strategy;
using WebShopSolution.DataAccess.Strategy.DateTimeHelper;
using WebShopSolution.DataAccess.UnitOfWork;
using WebShopSolution.Shared.Interfaces;

namespace WebShop.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly DiscountContext _discountContext;
        private readonly DiscountStrategyFactory _discountStrategyFactory;
        private readonly IDateTimeProvider _dateTimeProvider;
        public ProductController(IUnitOfWork unitOfWork, DiscountContext discountContext, DiscountStrategyFactory discountStrategyFactory, IDateTimeProvider dateTimeProvider)
        {
            _unitOfWork = unitOfWork;
            _discountContext = discountContext;
            _discountStrategyFactory = discountStrategyFactory;
            _dateTimeProvider = dateTimeProvider;
        }


        [HttpPost]
        public async Task<ActionResult> AddProduct([FromBody] Product product)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            if (product is null)
                return BadRequest();

            try
            {
                var productRepository = _unitOfWork.Repository<Product>();

                await productRepository.AddAsync(product);

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
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetAllProducts()
        {


            try
            {
                var productRepository = _unitOfWork.Repository<Product>();

                if (productRepository is null)
                    return NotFound();
                var productList = await productRepository.GetAllAsync();

                if (productList is null)
                    return NotFound();

                List<Product> updatedProductListWithDiscount = new List<Product>();
                foreach (var product in productList)
                {

                    var currentDate = _dateTimeProvider.Now;
                    var discountStrategy = _discountStrategyFactory.GetDiscountStrategy(currentDate);
                    _discountContext.SetDiscountStrategy(discountStrategy);
                    product.Price = _discountContext.GetDiscountPeriod(product);
                    updatedProductListWithDiscount.Add(product);
                }

                if (updatedProductListWithDiscount is null || !updatedProductListWithDiscount.Any())
                {
                    return NotFound();
                }

                return Ok(updatedProductListWithDiscount);

            }
            catch (Exception e)
            {
                _unitOfWork.Dispose();
                Console.WriteLine(e.Message);
                return StatusCode(500, "Internal server error");
            }

        }
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProductById(int id)
        {


            try
            {
                var productRepository = _unitOfWork.Repository<Product>();

                var product = await productRepository.GetByIdAsync(id);

                if (product is null)
                {
                    return NotFound();
                }

                var currentDate = _dateTimeProvider.Now;

                var discountStrategy = _discountStrategyFactory.GetDiscountStrategy(currentDate);
                _discountContext.SetDiscountStrategy(discountStrategy);
                product.Price = _discountContext.GetDiscountPeriod(product);

                return Ok(product);
            }
            catch (Exception e)
            {
                _unitOfWork.Dispose();
                Console.WriteLine(e);
                return StatusCode(500, "Internal server error");
            }


        }
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateProduct(int id, [FromBody] Product product)
        {

            if (!ModelState.IsValid)
                return BadRequest();

            try
            {
                var productRepository = _unitOfWork.Repository<Product>();
                if (productRepository is null)
                    return NotFound();

                var productToUpdate = await productRepository.GetByIdAsync(id);

                if (productToUpdate is null)
                {
                    return NotFound();
                }

                productToUpdate.Name = product.Name;
                productToUpdate.Price = product.Price;
                productToUpdate.Stock = product.Stock;

                await productRepository.UpdateAsync(productToUpdate);

                await _unitOfWork.CompleteAsync();

                return Ok(productToUpdate);
            }
            catch (Exception e)
            {
                _unitOfWork.Dispose();
                Console.WriteLine(e);
                return StatusCode(500, "Internal server error");
            }


        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteProduct(int id)
        {

            try
            {
                var productRepository = _unitOfWork.Repository<Product>();
                var product = await productRepository.GetByIdAsync(id);

                if (product == null)
                {
                    return NotFound();
                }

                await productRepository.RemoveAsync(id);

                await _unitOfWork.CompleteAsync();

                return StatusCode(200, "Ok");
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

