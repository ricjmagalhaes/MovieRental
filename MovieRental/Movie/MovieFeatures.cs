using MovieRental.Data;

namespace MovieRental.Movie
{
	public class MovieFeatures : IMovieFeatures
	{
		private readonly MovieRentalDbContext _movieRentalDb;
		public MovieFeatures(MovieRentalDbContext movieRentalDb)
		{
			_movieRentalDb = movieRentalDb;
		}
		
		public Movie Save(Movie movie)
		{
			_movieRentalDb.Movies.Add(movie);
			_movieRentalDb.SaveChanges();
			return movie;
		}

        // TODO: tell us what is wrong in this method? Forget about the async, what other concerns do you have?
        //Muita abstracao no metodo, muito generico, nao deixa claro o que o metodo faz
        //Deveria ser algo como GetAllMovies ou GetAllMovieTitles etc
        //Isto devolve entidades que ainda estão ligadas ao contexto do EF
        //Pode levar a problemas de performance e de memoria tipo Lazy Loading
        public List<Movie> GetAll()
		{
            //return _movieRentalDb.Movies.ToList();
            return _movieRentalDb.Movies
				.Select(m => new Movie { Id = m.Id, Title = m.Title})
				.ToList();
        }


	}
}
