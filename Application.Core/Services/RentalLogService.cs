using System;
using System.Collections.Generic;
using System.Linq;
using Application.Core.Contracts;
using Application.Core.Entities;
using Application.Core.Enums;
using Application.Core.Validation;

namespace Application.Core.Services
{
    public class RentalLogService : IRentalLogService
    {
        private readonly List<RentalLogs> _rentalLogs = new List<RentalLogs>();

        public void AddRentalLog(string scooterId, decimal pricePerMinute, RentalOperationType operationType)
        {
            if (operationType == RentalOperationType.StartRent)
                AddRentalLog(scooterId, pricePerMinute, DateTime.Now);
            else
                UpdateRentalLog(scooterId, pricePerMinute);
        }

        //Calculates rental information for active scooters
        public decimal CalculateIncome(string scooterId)
        {
            var rentalLog = _rentalLogs
                .OrderByDescending(x => x.EndDate).FirstOrDefault(x => x.ScooterId == scooterId);

            Guard.Against.Null(rentalLog);

            return GetIncome(new List<RentalLogs> {rentalLog}, x => x.ScooterId == scooterId, x => x.EndDate);
        }

        public decimal CalculateIncomeYearly(int? year, bool includeNotCompletedRentals)
        {
            var incomeResult = 0m;

            Func<RentalLogs, bool> func = null;
            if (year.HasValue)
              func=  x => x.StartDate.Year <= year && x.EndDate.HasValue;
            
            var completedRentalIncomes = GetIncome(_rentalLogs,  func);


            if (includeNotCompletedRentals)
            {
                if (year.HasValue)
                    func =x=> x.StartDate.Year <= DateTime.Now.Year &&
                           x.EndDate == null;
                
                var incompletedRentalLogs = _rentalLogs.Where(func).ToList();

                var currentDate = DateTime.Now;

                incompletedRentalLogs.ForEach(x => x.EndDate = currentDate);

                incomeResult += GetIncome(incompletedRentalLogs);
            }

            return incomeResult + completedRentalIncomes;
        }

        #region private methods

        private void AddRentalLog(string scooterId, decimal pricePerMinute, DateTime startDate)
        {
            var rentalLogItem = _rentalLogs.SingleOrDefault(x => x.ScooterId == scooterId
                                                                 && x.EndDate.HasValue == false);

            if (rentalLogItem != null)
                throw new RentalCompanyException("You cannot rent a scooter in use again");

            _rentalLogs.Add(new RentalLogs
            {
                ScooterId = scooterId, PricePerMinute = pricePerMinute, StartDate = startDate
            });
        }

        private void UpdateRentalLog(string scooterId, decimal pricePerMinute)
        {
            var rentalLogItem = _rentalLogs.SingleOrDefault(x => x.ScooterId == scooterId
                                                                 && x.EndDate.HasValue == false);

            if (rentalLogItem == null)
                throw new RentalCompanyException("This scooter is not currently in use.");

            var amount = GetIncome(_rentalLogs, x => x.ScooterId == scooterId &&
                                                     x.EndDate != null, x => x.EndDate);

            var endDate = amount == Constants.Constants.MaxUsageLimit
                ? new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day + 1, 0, 0, 0)
                : DateTime.Now;

            rentalLogItem.EndDate = endDate;
        }


        private decimal GetIncome(IEnumerable<RentalLogs> rentalLogs, Func<RentalLogs, bool> whereCondition = null,
            Func<RentalLogs, DateTime?> orderCondition = null)
        {
            if (!rentalLogs.Any()) return 0m;
            
            if (whereCondition != null) rentalLogs = rentalLogs.Where(whereCondition);
            if (orderCondition != null) rentalLogs = rentalLogs.OrderByDescending(orderCondition);

            return (decimal) rentalLogs.Sum(x =>
                (x.EndDate - x.StartDate).Value.TotalMinutes * (double) x.PricePerMinute);
        }

        #endregion
    }
}