using Microsoft.EntityFrameworkCore;
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
		
		public async Task<Movie> SaveAsync(Movie movie)
		{
			await _movieRentalDb.Movies.AddAsync(movie);
			_movieRentalDb.SaveChanges();
			return movie;
		}

        // TODO: tell us what is wrong in this method? Forget about the async, what other concerns do you have?
        //Too much abstraction in the method, too generic, it doesn't make it clear what the method does
        //It should be something like GetAllMovies or GetAllMovieTitles etc.
        //This returns entities that are still tied to the EF context.
        //This can lead to performance and memory issues like lazy loading.
        public List<Movie> GetAll()
		{
            //return _movieRentalDb.Movies.ToList();
            return _movieRentalDb.Movies
				.Select(m => new Movie { Id = m.Id, Title = m.Title})
				.ToList();
        }

        public async Task<List<Movie>> GetAllAsync()
        { 
            return await _movieRentalDb.Movies
                .Select(m => new Movie { Id = m.Id, Title = m.Title })
                .ToListAsync();
        }

    }
}
