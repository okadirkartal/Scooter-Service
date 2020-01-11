using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Application.Core.Contracts;
using Application.Core.Entities;
using Application.Core.Validation;

namespace Application.Core.Services
{
    public class ScooterService : IScooterService
    {
        private readonly ConcurrentDictionary<string, Scooter> _scooterList =
            new ConcurrentDictionary<string, Scooter>();

        public void AddScooter(string id, decimal pricePerMinute)
        {
            Guard.Against.Null(id);
            Guard.Against.Empty(id);
            Guard.Against.NullOrWhiteSpace(id);
            Guard.Against.OutOfRange(pricePerMinute, 1.0m, decimal.MaxValue);
            Guard.Against.Zero(pricePerMinute);

            if (_scooterList.ContainsKey(id)) throw new ScooterException($"This scooter id({nameof(id)}) is exists");

            if (!_scooterList.TryAdd(id, new Scooter(id, pricePerMinute)))
                throw new ScooterException("An error occured when Scooter was add.");
        }

        public void RemoveScooter(string id)
        {
            var scooter = GetScooterById(id);
            Guard.Against.Null(scooter);

            if (scooter.IsRented) throw new ScooterException("This scooter is rented,You can not remove");

            if (!_scooterList.TryRemove(id, out scooter))
                throw new ScooterException("An error occured when Scooter was remove");
        }

        public IList<Scooter> GetScooters()
        {
            return _scooterList.Values.ToList();
        }

        public Scooter GetScooterById(string scooterId)
        {
            Guard.Against.NullOrEmpty(scooterId);
            Guard.Against.NullOrWhiteSpace(scooterId);

            return _scooterList.TryGetValue(scooterId, out var scooter) ? scooter : null;
        }
    }
}