using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc;
using WebShopSolution.DataAccess.Entities;
using WebShopSolution.DataAccess.Repositories.Products;
using WebShopSolution.DataAccess.UnitOfWork;

namespace WebShop.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {

        private readonly IUnitOfWork _unitOfWork;

        public ProductController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        [HttpPost]
        public async Task<ActionResult> AddProduct([FromBody] Product product)
        {
            // Lägger till produkten via repository

            var productRepository = _unitOfWork.Repository<Product>();


            if (product is null)
            {
                return BadRequest();
            }

            
            await productRepository.AddAsync(product);


            // Sparar förändringar
            await _unitOfWork.CompleteAsync();

            // Notifierar observatörer om att en ny produkt har lagts till


            //_unitOfWork.NotifyProductAdded(newProduct);

            return Ok();
        }



        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetAllProducts()
        {
            
            var productRepository = _unitOfWork.Repository<Product>();

            if (productRepository is null)
                return null;

            try
            {
              var productList = await productRepository.GetAllAsync();

              if (productList is null)
              {
                  return null;
              }
              else
              {
                  return Ok(productList);
              }


            }
            catch (Exception e)
            {
                _unitOfWork.Dispose();
                Console.WriteLine(e.Message);
            }

            return Ok();

        }


        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProductById(int id)
        {
            var productRepository = _unitOfWork.Repository<Product>();
            if (productRepository is null)
                return null;

            try
            {
                var product = await productRepository.GetByIdAsync(id);

                if (product is null)
                {
                    return NotFound();
                }
            }
            catch (Exception e)
            {
                _unitOfWork.Dispose();
                Console.WriteLine(e);
                
            }
            
            return Ok();
        }


        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateProduct(int id, [FromBody] Product product)
        {

            var productRepository = _unitOfWork.Repository<Product>();


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

            return Ok();
        }


        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteProduct(int id)
        {

            var productRepository = _unitOfWork.Repository<Product>();

            var product = await productRepository.GetByIdAsync(id);

            if (product is null)
            {
                return NotFound();
            }

            await productRepository.RemoveAsync(id);

            await _unitOfWork.CompleteAsync();

            return Ok();
        }

    }
}

