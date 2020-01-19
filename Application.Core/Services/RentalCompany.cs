using System;
using Application.Core.Contracts;
using Application.Core.Enums;
using Application.Core.Validation;

namespace Application.Core.Services
{
    public class RentalCompany : IRentalCompany
    {
        private readonly IRentalLogService _rentalLogService;
        private readonly IScooterService _scooterService;

        public RentalCompany(IScooterService scooterService, IRentalLogService rentalLogService)
        {
            _scooterService = scooterService;
            _rentalLogService = rentalLogService;
        }

        /// <summary>
        ///     Name of company
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Start the rent of the scooter.
        /// </summary>
        /// <param name="id">ID of the scooter</param>
        public void StartRent(string id)
        {
            var scooter = _scooterService.GetScooterById(id);
            Guard.Against.Null(scooter);

            if (scooter.IsRented)
                throw new RentalCompanyException($"This scooter : {id} is currently rented.You can not rent for now!");

            scooter.IsRented = true;

            _rentalLogService.AddRentalLog(scooter.Id, scooter.PricePerMinute, RentalOperationType.StartRent);
        }

        /// <summary>
        ///     End the rent of the scooter.
        /// </summary>
        /// <param name="id">ID of the scooter</param>
        /// <returns>
        ///     The total price of rental. It has to calculated taking into account for how long time scooter was rented.
        ///     If total amount per day reaches 20 EUR than timer has to be stopped and restarted at beginning of next day at 0:00
        ///     am.
        /// </returns>
        public decimal EndRent(string id)
        {
            var scooter = _scooterService.GetScooterById(id);
            Guard.Against.Null(scooter);

            if (!scooter.IsRented)
                throw new RentalCompanyException(
                    $"This scooter : {id} is currently is not rented.You can not perform operation!");

            scooter.IsRented = false;

            _rentalLogService.AddRentalLog(scooter.Id, scooter.PricePerMinute, RentalOperationType.EndRent);

            return _rentalLogService.CalculateIncome(scooter.Id);
        }

        /// <summary>
        ///     Income report.
        /// </summary>
        /// <param name="year">Year of the report. Sum all years if value is not set.</param>
        /// <param name="includeNotCompletedRentals">
        ///     Include income from the scooters that are rented out (rental has not ended yet) and calculate rental
        ///     price as if the rental would end at the time when this report was requested.
        /// </param>
        /// ///
        /// <returns>The total price of all rentals filtered by year if given.</returns>
        public decimal CalculateIncome(int? year, bool includeNotCompletedRentals)
        {
            year ??= DateTime.Now.Year;

            Guard.Against.OutOfRange(year.GetValueOrDefault(), 0, DateTime.Now.Year);

            return _rentalLogService.CalculateIncomeYearly(year.Value, includeNotCompletedRentals);
        }
    }
}