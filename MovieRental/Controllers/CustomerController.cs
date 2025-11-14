using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieRental.Customer;
using MovieRental.Data;

namespace MovieRental.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerFeatures _features;
        private readonly ILogger<CustomerController> _logger;

        public CustomerController(ICustomerFeatures features, ILogger<CustomerController> logger)
        {
            _features = features;
            _logger = logger;
        }

        /// <summary>
        /// List all customers.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Get()
        {
            _logger.LogInformation("Listing all customers");
            var customers = await _features.GetAllAsync();
            return Ok(customers);
        }

        /// <summary>
        /// Get a customer by id.
        /// </summary>
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var customers = await _features.GetAllAsync();
            var customer = customers.FirstOrDefault(c => c.Id == id);
            if (customer == null)
                return NotFound();
            return Ok(customer);
        }

        /// <summary>
        /// Create a new customer.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Post([FromBody] Customer.Customer customer)
        {
            if (customer == null)
                return BadRequest(new { error = "Customer body is required." });

            if (string.IsNullOrWhiteSpace(customer.Name))
                return BadRequest(new { error = "Customer.Name is required." });

            try
            {
                _logger.LogDebug($"Try Create CustomerName: {customer.Name}");
                var created = await _features.SaveAsync(customer);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving customer");
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = "Could not save customer." });
            }
        }
    }
} 
