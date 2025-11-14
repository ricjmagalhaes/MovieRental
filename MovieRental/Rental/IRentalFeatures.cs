using System.Threading.Tasks;

namespace MovieRental.Rental;

public interface IRentalFeatures
{ 
    Task<Rental> Save(Rental rental);

    Task<Rental> ProcessPayment(Rental rental);
     
    Task<IEnumerable<Rental>> GetRentalsByCustomerName(string customerName);
    Task<IEnumerable<Rental>> GetAllRentals();

}