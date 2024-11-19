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

            Product newProduct = new Product
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                Stock = product.Stock,
                OrderProducts = []
            };


            await productRepository.AddAsync(newProduct);


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

            var productList = await productRepository.GetAllAsync();

            return Ok(productList);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProductById(int id)
        {
            var productRepository = _unitOfWork.Repository<Product>();


            var product = await productRepository.GetByIdAsync(id);

            if (product is null)
            {
                return NotFound();
            }

            return Ok(product);
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

