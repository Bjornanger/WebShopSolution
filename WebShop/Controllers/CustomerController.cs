using Microsoft.AspNetCore.Mvc;
using WebShopSolution.DataAccess.Entities;
using WebShopSolution.DataAccess.UnitOfWork;
using WebShopSolution.Shared.Interfaces;

namespace WebShop.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        public CustomerController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        
        [HttpPost]
        public async Task<ActionResult> AddCustomer([FromBody] Customer customer)   
        {
            if (!ModelState.IsValid)
                return BadRequest();
            if (customer is null)
                return BadRequest();
            try
            {
                var customerRepository = _unitOfWork.Repository<Customer>();

                await customerRepository.AddAsync(customer);
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
        public async Task<ActionResult<IEnumerable<Customer>>> GetAllCustomers()
        {

            try
            {
                var customerRepository = _unitOfWork.Repository<Customer>();
                var customersList = await customerRepository.GetAllAsync();

                if (customersList is null || !customersList.Any())
                {
                    return NotFound();
                }

                return Ok(customersList);
            }
            catch (Exception e)
            {
                _unitOfWork.Dispose();
                Console.WriteLine(e.Message);
                return StatusCode(500, "Internal server error");
            }
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<Customer>> GetCustomerById(int id)
        {

            try
            {
                var customerRepository = _unitOfWork.Repository<Customer>();

                var customer = await customerRepository.GetByIdAsync(id);
                if (customer is null)
                {
                    return NotFound();
                }

                return Ok(customer);
            }
            catch (Exception e)
            {
                _unitOfWork.Dispose();
                Console.WriteLine(e);
                return StatusCode(500, "Internal server error");
            }
        }
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateCustomer(int id, [FromBody] Customer customer)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            if (customer is null)
                return BadRequest();
            try
            {
                var customerRepository = _unitOfWork.Repository<Customer>();
                var customerToUpdate = await customerRepository.GetByIdAsync(id);
                if (customerToUpdate is null)
                {
                    return NotFound();
                }
                customerToUpdate.FirstName = customer.FirstName;
                customerToUpdate.LastName = customer.LastName;
                customerToUpdate.Email = customer.Email;
                customerToUpdate.Password = customer.Password;
                
                customerRepository.UpdateAsync(customerToUpdate);
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
        public async Task<ActionResult> DeleteCustomer(int id)
        {
            try
            {
                var customerRepository = _unitOfWork.Repository<Customer>();
                var customerToDelete = await customerRepository.GetByIdAsync(id);
                if (customerToDelete is null)
                {
                    return NotFound();
                }

               await customerRepository.RemoveAsync(customerToDelete.Id);
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
