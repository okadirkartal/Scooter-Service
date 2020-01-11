using System;

namespace Application.Core
{
    public class ScooterException : Exception
    {
        public ScooterException()
        {
        }

        public ScooterException(string message) : base(message)
        {
        }
    }
}