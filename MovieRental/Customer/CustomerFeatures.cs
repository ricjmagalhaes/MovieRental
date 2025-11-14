using Microsoft.EntityFrameworkCore;
using MovieRental.Data;

namespace MovieRental.Customer
{
    public class CustomerFeatures : ICustomerFeatures
    {
        private readonly MovieRentalDbContext _movieRentalDb;

        public CustomerFeatures(MovieRentalDbContext movieRentalDb)
        {
            _movieRentalDb = movieRentalDb;
        }

        public Customer Save(Customer customer)
        {
            _movieRentalDb.Customers.Add(customer);
            _movieRentalDb.SaveChanges();
            return customer;
        }

        // Async version
        public async Task<Customer> SaveAsync(Customer customer)
        {
            _movieRentalDb.Customers.Add(customer);
            await _movieRentalDb.SaveChangesAsync();
            return customer;
        }

        public List<Customer> GetAll()
        {
            return _movieRentalDb.Customers
                .Select(c => new Customer { Id = c.Id, Name = c.Name })
                .ToList();
        }

        public async Task<List<Customer>> GetAllAsync()
        {
            return await _movieRentalDb.Customers
                .AsNoTracking()
                .Select(c => new Customer { Id = c.Id, Name = c.Name })
                .ToListAsync();
        }
    }
}
