namespace MovieRental.PaymentProviders
{
    public interface IPaymentProvider
    {
        string Name { get; }   // Ex: "MbWay", "PayPal"
        Task<bool> Pay(double price); 
    }
}
