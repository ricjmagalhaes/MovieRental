using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using MovieRental.Customer;
using MovieRental.Data;
using MovieRental.Movie;
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
            var loggerFactory = LoggerFactory.Create(builder => { });
            var log = new Logger<RentalFeatures>(loggerFactory);
            var features = new RentalFeatures(_movieRentalDb, providers, log);

            var customer = new Customer.Customer { Id = 1, Name = "Ricardo Magalhaes" };
            var movies = new Movie.Movie { Id = 1, Title = "Matrix" };

            var rental = new MovieRental.Rental.Rental 
            {
                Id = 1,
                DaysRented = 1,
                Movie = movies,
                MovieId = 1,
                PaymentMethod = "MbPay",
                Price = 200,
                CustomerId = 1,
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
             
            var loggerFactory = LoggerFactory.Create(builder => { });
            var log = new Logger<RentalFeatures>(loggerFactory);
            var features = new RentalFeatures(_movieRentalDb, providers, log);

            var customer = new Customer.Customer { Id = 1, Name = "Ricardo Magalhaes" };
            var movies = new Movie.Movie { Id = 1, Title = "Matrix" };
            var rental = new MovieRental.Rental.Rental
            {
                Id = 1,
                DaysRented = 1,
                Movie = movies,
                MovieId = 1,
                PaymentMethod = "MbPay",
                Price = 200,
                CustomerId = 1,
                Customer = customer
            };

            // Act
            var result = await features.ProcessPayment(rental);

            // Assert
            Assert.NotNull(result);
            Assert.Single(_movieRentalDb.Rentals);
        }
    }
}
