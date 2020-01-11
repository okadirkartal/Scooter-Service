using Application.Core.Enums;

namespace Application.Core.Contracts
{
    public interface IRentalLogService
    {
        /// <summary>
        ///     Add Scooter Rental Information to the Log.
        /// </summary>
        /// <param name="scooterId">Scooter's Id.</param>
        /// <param name="pricePerMinute">Scooter's rent price per minute</param>
        void AddRentalLog(string scooterId, decimal pricePerMinute, RentalOperationType operationType);

        /// <summary>
        ///     Returns active scooter's usage fee
        /// </summary>
        /// <param name="scooterId">Scooter's Id.</param>
        decimal CalculateIncome(string scooterId);

        /// <summary>
        ///     Calculate rental company income from all rentals and provide yearly report if requested.
        /// </summary>
        /// <param name="year">Scooter's Id.</param>
        /// <param name="includeNotCompletedRentals">Included non-completed rentals for scooters.</param>
        decimal CalculateIncomeYearly(int year, bool includeNotCompletedRentals);
    }
}