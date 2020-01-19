using System;
using System.Threading;
using Application.Core;
using Application.Core.Contracts;
using Application.Core.Entities;
using Application.Core.Services;
using NSubstitute;
using Xunit;

namespace Application.Tests
{
    public class RentalCompanyTests
    {
        public RentalCompanyTests()
        {
            _scooterService = Substitute.For<IScooterService>();
            IRentalLogService rentalLogService = new RentalLogService();
            _rentalCompany = new RentalCompany(_scooterService, rentalLogService);
        }

        private readonly IScooterService _scooterService;
        private readonly IRentalCompany _rentalCompany;

        [Fact]
        [Trait("RentalCompany.Category", "CalculateIncome")]
        public void GetYearlyIncome_WhenIncludedNotCompletedRentals_ReturnsTrue()
        {
            var scooter = new Scooter("Scooter1", 25);
            _scooterService.GetScooterById(scooter.Id).Returns(new Scooter(scooter.Id, scooter.PricePerMinute));
            _rentalCompany.StartRent(scooter.Id);
            _rentalCompany.EndRent(scooter.Id);

            Thread.Sleep(1000);

            _rentalCompany.StartRent(scooter.Id);

            var result = _rentalCompany.CalculateIncome(DateTime.Now.Year, true);
            Assert.True(result > 0);
        }

        [Fact]
        [Trait("RentalCompany.Category", "StartRent")]
        public void StartRent_ScooterIsRented_ThrowsArgumentOutOfRangeException()
        {
            var scooter = new Scooter("Scooter1", 25);
            scooter.SetRented(true);
            _scooterService.GetScooterById(scooter.Id).Returns(scooter);
            Assert.Throws<RentalCompanyException>(() => _rentalCompany.StartRent(scooter.Id));
        }

        [Fact]
        [Trait("RentalCompany.Category", "StartRent")]
        public void StartRent_WhenScooterIdIsEmpty_ThrowsArgumentException()
        {
            //Scooter is not found
            _scooterService.GetScooterById("Scooter1").Returns(new Scooter("Scooter2", 4));
            Assert.Throws<ArgumentNullException>(() => _rentalCompany.StartRent(string.Empty));
        }

        [Fact]
        [Trait("RentalCompany.Category", "StartRent")]
        public void StartRent_WhenScooterIdIsNull_ThrowsArgumentNullException()
        {
            //Scooter is not found
            _scooterService.GetScooterById("Scooter1").Returns(new Scooter("Scooter1", 5));
            Assert.Throws<ArgumentNullException>(() => _rentalCompany.StartRent(null));
        }

        [Fact]
        [Trait("RentalCompany.Category", "StartRent")]
        public void StartRent_WhenScooterIdIsWhiteSpace_ThrowsArgumentException()
        {
            //Scooter is not found
            _scooterService.GetScooterById("Scooter1").Returns(new Scooter("Scooter1", 10));
            Assert.Throws<ArgumentNullException>(() => _rentalCompany.StartRent(" "));
        }

        [Fact]
        public void StartRent_WhenScooterRented_IncomeShouldBeGreaterThanZero()
        {
            var scooter = new Scooter("Scooter1", 25);
            _scooterService.GetScooterById(scooter.Id).Returns(scooter);
            _rentalCompany.StartRent(scooter.Id);
            var income = _rentalCompany.EndRent(scooter.Id);
            Assert.True(income > 0);
        }


        [Fact]
        [Trait("RentalCompany.Category", "RentedCheck")]
        public void StartRent_WhenScooterStillRented_ThrowsRentalCompanyException()
        {
            var scooter = new Scooter("Scooter1", 25);
            _scooterService.GetScooterById("Scooter1").Returns(scooter);
            _rentalCompany.StartRent(scooter.Id);
            Assert.Throws<RentalCompanyException>(() => _rentalCompany.StartRent(scooter.Id));
        }
    }
}