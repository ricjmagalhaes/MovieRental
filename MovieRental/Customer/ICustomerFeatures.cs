namespace MovieRental.Customer
{
    public interface ICustomerFeatures
    {
        Customer Save(Customer customer);
        List<Customer> GetAll();

        // async variants
        Task<Customer> SaveAsync(Customer customer);
        Task<List<Customer>> GetAllAsync();
    }
}
