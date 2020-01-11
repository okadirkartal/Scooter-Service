using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Core.Contracts;
using Application.Core.Services;
using Xunit;

namespace Application.Tests
{
    public class ScooterServiceTests
    {
        public ScooterServiceTests()
        {
            _scooterService = new ScooterService();
        }

        private readonly IScooterService _scooterService;


        [Theory]
        [InlineData("Scooter1")]
        [InlineData("Scooter 2")]
        [Trait("ScooterService.Category", "AddScooter")]
        public void AddScooter_WhenSameItemExists_ThrowsException(string scooterId)
        {
            _scooterService.AddScooter(scooterId, 80);
            Assert.Throws<ScooterException>(() => _scooterService.AddScooter(scooterId, 80));
        }

        [Theory]
        [InlineData("Scooter1")]
        [Trait("ScooterService.Category", "GetScooter")]
        public void GetScooterById_IfExists_ScooterNameShouldExists(string scooterId)
        {
            _scooterService.AddScooter(scooterId, 80);
            var scooter = _scooterService.GetScooterById(scooterId);
            Assert.Equal(scooterId, scooter.Id);
        }

        private void AddTemporaryItemsToScooterService(Action action, int totalCount = 10)
        {
            Task.Factory.StartNew(() =>
            {
                for (var index = 0; index < totalCount; index++)
                {
                    _scooterService.AddScooter($"Scooter{index}", index * 10);
                    Thread.Sleep(2000);
                }
            }).GetAwaiter().OnCompleted(() => { action(); });
        }

        [Fact]
        [Trait("ScooterService.Category", "AddScooter")]
        public void AddScooter_WhenAItemsAreAdded_CompareItemCount()
        {
            var totalCount = 10;
            AddTemporaryItemsToScooterService(() => { Assert.Equal(totalCount, _scooterService.GetScooters().Count); },
                totalCount);
        }


        [Fact]
        [Trait("ScooterService.Category", "AddScooter")]
        public void AddScooter_WhenIdIsEmpty_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => _scooterService.AddScooter(string.Empty, 80));
        }

        [Fact]
        [Trait("ScooterService.Category", "AddScooter")]
        public void AddScooter_WhenIdIsNull_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _scooterService.AddScooter(null, 80));
        }

        [Fact]
        [Trait("ScooterService.Category", "AddScooter")]
        public void AddScooter_WhenIdIsWhiteSpace_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => _scooterService.AddScooter(" ", 80));
        }

        [Fact]
        [Trait("ScooterService.Category", "AddScooter")]
        public void AddScooter_WhenPricePerMinuteIsMinus_ThrowsArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => _scooterService.AddScooter("Scooter1", -5));
        }

        [Fact]
        [Trait("ScooterService.Category", "AddScooter")]
        public void AddScooter_WhenPricePerMinuteIsZero_ThrowsArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => _scooterService.AddScooter("Scooter1", 0));
        }


        [Fact]
        [Trait("ScooterService.Category", "GetScooter")]
        public void GetScooterById_IfThereIsNoRecord_ReturnsNull()
        {
            var scooter = _scooterService.GetScooterById("Scooter1");
            Assert.Null(scooter);

            _scooterService.AddScooter("Scooter2", 55);
            Assert.Null(_scooterService.GetScooterById("Scooter1"));
        }

        [Fact]
        [Trait("ScooterService.Category", "RemoveScooter")]
        public void RemoveItemFromScooterList_WhenIdIsNull_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentNullException>(() => _scooterService.RemoveScooter(null));
        }

        [Fact]
        [Trait("ScooterService.Category", "RemoveScooter")]
        public void RemoveItemFromScooterList_WhenIdIsWhiteEmpty_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => _scooterService.RemoveScooter(string.Empty));
        }

        [Fact]
        [Trait("ScooterService.Category", "RemoveScooter")]
        public void RemoveItemFromScooterList_WhenItemIIsRented_ThrowsScooterException()
        {
            _scooterService.AddScooter("Scooter1", 10);
            _scooterService.GetScooterById("Scooter1").IsRented = true;
            Assert.Throws<ScooterException>(() => _scooterService.RemoveScooter("Scooter1"));
        }

        [Fact]
        [Trait("ScooterService.Category", "RemoveScooter")]
        public void RemoveItemFromScooterList_WhenItemIsNotExist_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _scooterService.RemoveScooter("SampleScooter"));
        }

        [Fact]
        [Trait("ScooterService.Category", "RemoveScooter")]
        public void RemoveItemFromScooterList_WhenItemsAreExists_ShouldBeRemoveFromList()
        {
            AddTemporaryItemsToScooterService(() =>
            {
                _scooterService.RemoveScooter("Scooter1");
                _scooterService.RemoveScooter("Scooter2");
                _scooterService.RemoveScooter("NonExistingName");
                Assert.Equal(8, _scooterService.GetScooters().Count);
            });
        }

        [Fact]
        [Trait("ScooterService.Category", "RemoveScooter")]
        public void RemoveScooter_TryToRemoveSameItemTwiceTime_ThrowsArgumentException()
        {
            _scooterService.AddScooter("Scooter1", 10);
            _scooterService.RemoveScooter("Scooter1");
            Assert.Throws<ArgumentNullException>(() => _scooterService.RemoveScooter("Scooter1"));
        }

        [Fact]
        [Trait("ScooterService.Category", "RemoveScooter")]
        public void RemoveScooter_WhenIdIsWhiteSpace_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => _scooterService.RemoveScooter(" "));
        }
    }
}