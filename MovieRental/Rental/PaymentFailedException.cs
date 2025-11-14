namespace MovieRental.Rental
{
    public class PaymentFailedException: Exception
    {
        public PaymentFailedException() { }
        public PaymentFailedException(string message) : base(message) { }
        public PaymentFailedException(string message, Exception innerException) : base(message, innerException) { }
    }
}
