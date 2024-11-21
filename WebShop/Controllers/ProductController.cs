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
            if (!ModelState.IsValid)
                return BadRequest();

            if (product is null)
                return BadRequest();
           
            try
            {
                var productRepository = _unitOfWork.Repository<Product>();
                //if (productRepository is null)
                //    return NotFound();


                productRepository.AddAsync(product);//Endast denna ska testas
                

                await _unitOfWork.CompleteAsync();
                return Ok();
                // Notifierar observat�rer om att en ny produkt har lagts till
                //_unitOfWork.NotifyProductAdded(newProduct);


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

            var productRepository = _unitOfWork.Repository<Product>();

            if (productRepository is null)
                return NotFound();


            try
            {
                var productList = await productRepository.GetAllAsync();

                if (productList is null || !productList.Any())
                {
                    return NotFound();
                }

                return Ok(productList);

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

