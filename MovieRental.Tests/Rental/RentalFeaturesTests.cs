using Microsoft.EntityFrameworkCore;
using Moq;
using MovieRental.Data;
using MovieRental.PaymentProviders;
using MovieRental.Rental;
using System;
using System.Collections.Generic;
using System.Text; 

namespace MovieRental.Tests.Rental
{
    public class RentalFeaturesTests
    {
        [Fact]
        public async Task ProcessRental_ShouldNotSave_WhenPaymentFails()
        {
            // Arrange
            var mockProvider = new Mock<IPaymentProvider>();
            mockProvider.Setup(p => p.Name).Returns("MbPay");
            mockProvider.Setup(p => p.Pay(It.IsAny<double>())).ReturnsAsync(false);

            var providers = new List<IPaymentProvider> { mockProvider.Object };

            using var _movieRentalDb = new MovieRentalDbContext();
            var features = new RentalFeatures(_movieRentalDb, providers, );

            var customer = new Customer.Customer { Id = 1, Name = "Ricardo Magalhaes" };

            var rental = new Rental.Rental
            {
                Id = 1, CustomerId = 1,
                PaymentMethod = "MbPay",
                Amount = 200,
                Customer = customer
            };

            // Act
            var result = await features.ProcessPayment(rental);

            // Assert
            Assert.Null(result);
            Assert.Empty(_movieRentalDb.Rentals);
        }

        [Fact]
        public async Task ProcessRental_ShouldSave_WhenPaymentSucceeds()
        {
            // Arrange
            var mockProvider = new Mock<IPaymentProvider>();
            mockProvider.Setup(p => p.Name).Returns("MbPay");
            mockProvider.Setup(p => p.Pay(It.IsAny<double>())).ReturnsAsync(true);

            var providers = new List<IPaymentProvider> { mockProvider.Object };

            using var _movieRentalDb = new MovieRentalDbContext();
            var features = new RentalFeatures(_movieRentalDb, providers);

            var rental = new Rental.Rental { PaymentMethod = "MbPay", Amount = 100m };

            // Act
            var result = await features.ProcessPayment(rental);

            // Assert
            Assert.NotNull(result);
            Assert.Single(_movieRentalDb.Rentals);
        }
    }
}
