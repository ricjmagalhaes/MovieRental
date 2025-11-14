using Microsoft.AspNetCore.Mvc;
using MovieRental.Movie;

namespace MovieRental.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MovieController : ControllerBase
    {

        private readonly IMovieFeatures _features;
        private readonly ILogger<MovieController> _logger;

        public MovieController(IMovieFeatures features, ILogger<MovieController> logger)
        {
            _features = features;
            _logger = logger;
        }

        /// <summary>
        /// Obtém todos os filmes.
        /// </summary>
        /// <remarks>
        /// Exemplo de request:
        /// 
        ///     GET /api/Movies
        /// 
        /// </remarks>
        /// <returns>Lista de filmes</returns>
        /// <response code="200">Retorna a lista com sucesso</response>
        /// <response code="500">Erro interno no servidor</response>
        //[HttpGet]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //public IActionResult Get()
        //{
        //    _logger.LogInformation($"Getting all movies at {DateTime.Now}");
        //    return Ok(_features.GetAll());
        //}


        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAsync()
        {
            _logger.LogInformation($"Getting all movies at {DateTime.Now}");
            var movies = await _features.GetAllAsync();

            return Ok(movies);
        }


        ///// <summary>
        ///// Cria um novo filme.
        ///// </summary>
        ///// <param name="movie">Objeto FILME a ser criado</param>
        ///// <returns>O filme criado</returns>
        ///// <response code="200">Retorna OK com sucesso</response>
        ///// <response code="400">Erro na validação dos dados</response> 
        //[HttpPost]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)] 
        //public IActionResult Post([FromBody] Movie.Movie movie)
        //{
        //    _logger.LogInformation($"Save new movies {movie.Title}");
        //    return Ok(_features.Save(movie));
        //}

        /// <summary>
        /// Cria um novo filme.
        /// </summary>
        /// <param name="movie">Objeto FILME a ser criado</param>
        /// <returns>O filme criado</returns>
        /// <response code="200">Retorna OK com sucesso</response>
        /// <response code="400">Erro na validação dos dados</response> 
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PostAsync([FromBody] Movie.Movie movie)
        {
            _logger.LogInformation($"SaveAsync new movies {movie.Title}");
            return Ok(_features.SaveAsync(movie));
        }
    }
}
