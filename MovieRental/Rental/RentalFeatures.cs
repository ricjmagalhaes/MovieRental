using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MovieRental.Data;
using MovieRental.PaymentProviders;

namespace MovieRental.Rental
{
	public class RentalFeatures : IRentalFeatures
	{
		private readonly MovieRentalDbContext _movieRentalDb;
        private readonly IEnumerable<IPaymentProvider> _paymentProviders;
        private readonly ILogger<RentalFeatures> _logger;
        public RentalFeatures(MovieRentalDbContext movieRentalDb, IEnumerable<IPaymentProvider> paymentProviders, ILogger<RentalFeatures> logger)
		{
			_movieRentalDb = movieRentalDb;
            _paymentProviders = paymentProviders;
            _logger = logger;
        }

        // made async: returns Task<Rental>
        public async Task<Rental> Save(Rental rental)
        {
			_movieRentalDb.Rentals.Add(rental);
			await _movieRentalDb.SaveChangesAsync();
			return rental;
		}

        /// <summary>
        /// Processes the payment for a rental and records the rental if the payment is successful.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if the specified payment provider is not supported or if the payment fails.</exception>
        public async Task<Rental> ProcessPayment(Rental rental)
        {

            // Ensure we have customer info (either navigation or load from DB)
            var customer = rental.Customer ?? await _movieRentalDb.Customers.FindAsync(rental.CustomerId);
            var customerName = customer?.Name ?? "Unknown";

            var provider = _paymentProviders
                .FirstOrDefault(p =>
                    p.Name.Equals(rental.PaymentMethod, StringComparison.OrdinalIgnoreCase));

             
            if (provider == null)
            {
                _logger.LogWarning($"Payment provider '{rental.PaymentMethod}' not found for customer {rental?.Customer?.Name}");
                throw new InvalidOperationException($"Payment provider '{rental?.PaymentMethod}' not supported.");
            }

            _logger.LogInformation($"Processing rental for {rental?.Customer?.Name} using {rental?.PaymentMethod}");

            // Process the payment before saving
            try
            {
                var success = await provider.Pay(rental.Price);

                if (!success)
                {
                    _logger.LogWarning($"Payment failed for customer {rental?.Customer?.Name} (amount: {rental?.Price})");

                    throw new InvalidOperationException("Payment failed. Rental not created.");
                }
            } 
            catch (PaymentFailedException) 
            {
                // Re-throw known payment exceptions without wrapping
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Payment provider {provider.Name} call failed for customer {customerName}");
                throw new PaymentFailedException("Payment provider error occurred.", ex);
            }

            //Only persist if payment succeeded
            _movieRentalDb.Rentals.Add(rental);
            await _movieRentalDb.SaveChangesAsync();

            _logger.LogInformation($"Rental saved successfully for customer {rental?.Customer?.Name}");
            return rental;
        }


        // returns rentals including customer navigation and filters by customer name
        public async Task<IEnumerable<Rental>> GetRentalsByCustomerName(string customerName)
		{
            if (string.IsNullOrWhiteSpace(customerName))
                throw new ArgumentException("Customer name cannot be empty.", nameof(customerName));

            return await _movieRentalDb.Rentals
               .Include(r => r.Customer)
               .Where(r => r.Customer != null && EF.Functions.Like(r.Customer.Name, $"%{customerName}%"))
               .ToListAsync();
        }

	}
}
