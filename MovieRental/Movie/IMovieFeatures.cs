namespace MovieRental.Movie;

public interface IMovieFeatures
{
	Movie Save(Movie movie);
	List<Movie> GetAll();

    // async variants
    Task<Movie> SaveAsync(Movie movie);
    Task<List<Movie>> GetAllAsync();
}