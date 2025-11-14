namespace MovieRental.Movie
{
    public class MovieFailedException : Exception
    {
        public MovieFailedException() { }

        public MovieFailedException(string message) : base(message) { }

        public MovieFailedException(string message, Exception innerException) : base(message, innerException) { }
    }
}
