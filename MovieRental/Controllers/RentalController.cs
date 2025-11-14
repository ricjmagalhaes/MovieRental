using Microsoft.AspNetCore.Mvc;
using MovieRental.Movie;
using MovieRental.Rental;

namespace MovieRental.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RentalController : ControllerBase
    {

        private readonly IRentalFeatures _features;

        public RentalController(IRentalFeatures features)
        {
            _features = features;
        }
         
        /// <summary>
        /// alugar umm filme.
        /// </summary>
        /// <param name="rental">Objeto Rental a ser criado</param>
        /// <returns>O filme alugado</returns>
        /// <response code="200">Retorna OK com sucesso</response>
        /// <response code="400">Erro na validação dos dados</response> 
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Post([FromBody] Rental.Rental rental)
        {
            try
            {
                var result = await _features.Save(rental);
                return CreatedAtAction(nameof(rental.Id), new { id = result.Id }, result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Filmes alugados por Nome de cliente.
        /// </summary>
        /// <param name="customerName">Nome da Pesquisa</param>
        /// <returns>O filme alugado</returns>
        /// <response code="200">Retorna OK com sucesso</response>
        /// <response code="400">Erro na validação dos dados</response> 
        [HttpGet("by-customer")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetRentalsByCustomerName([FromQuery] string customerName)
        {
            try
            {
                var rentals = await _features.GetRentalsByCustomerName(customerName);
                if (!rentals.Any())
                    return NotFound($"No rentals found for customer '{customerName}'.");

                return Ok(rentals);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = ex.Message });
            }
        }

    }
}
