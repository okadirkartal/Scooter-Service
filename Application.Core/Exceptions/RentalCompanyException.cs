using System;

namespace Application.Core
{
    public class RentalCompanyException : Exception
    {
        public RentalCompanyException()
        {
        }

        public RentalCompanyException(string message) : base(message)
        {
        }
    }
}