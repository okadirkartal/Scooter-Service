using System;

namespace Application.Core.Entities
{
    public class RentalLogs
    {
        public string ScooterId { get; set; }

        public decimal PricePerMinute { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }
    }
}