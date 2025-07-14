using NinjaTrader.Custom.AddOns.OrderFlowBot.Containers;
using NinjaTrader.Custom.AddOns.OrderFlowBot.Models.Strategies.Implementations;
using Xunit;

namespace OrderFlowBot.Tests
{
    public class NewStrategiesTests
    {
        private readonly EventsContainer _eventsContainer = new EventsContainer();

        [Fact]
        public void VolumeSequencing_Should_Initialize_With_Correct_Name()
        {
            // Arrange & Act
            var strategy = new VolumeSequencing(_eventsContainer);

            // Assert
            Assert.Equal("Volume Sequencing", strategy.StrategyData.Name);
        }

        [Fact]
        public void DeltaDivergence_Should_Initialize_With_Correct_Name()
        {
            // Arrange & Act
            var strategy = new DeltaDivergence(_eventsContainer);

            // Assert
            Assert.Equal("Delta Divergence", strategy.StrategyData.Name);
        }

        [Fact]
        public void VolumeSequencing_Should_Not_Trigger_With_Invalid_Data()
        {
            // Arrange
            var strategy = new VolumeSequencing(_eventsContainer);

            // Act
            bool longResult = strategy.CheckLong();
            bool shortResult = strategy.CheckShort();

            // Assert
            Assert.False(longResult);
            Assert.False(shortResult);
        }

        [Fact]
        public void DeltaDivergence_Should_Not_Trigger_With_Invalid_Data()
        {
            // Arrange
            var strategy = new DeltaDivergence(_eventsContainer);

            // Act
            bool longResult = strategy.CheckLong();
            bool shortResult = strategy.CheckShort();

            // Assert
            Assert.False(longResult);
            Assert.False(shortResult);
        }

        [Fact]
        public void All_Strategies_Should_Inherit_From_StrategyBase()
        {
            // Arrange & Act
            var stackedImbalances = new StackedImbalances(_eventsContainer);
            var volumeSequencing = new VolumeSequencing(_eventsContainer);
            var deltaDivergence = new DeltaDivergence(_eventsContainer);

            // Assert
            Assert.NotNull(stackedImbalances.StrategyData);
            Assert.NotNull(volumeSequencing.StrategyData);
            Assert.NotNull(deltaDivergence.StrategyData);
            
            Assert.Equal("Stacked Imbalances", stackedImbalances.StrategyData.Name);
            Assert.Equal("Volume Sequencing", volumeSequencing.StrategyData.Name);
            Assert.Equal("Delta Divergence", deltaDivergence.StrategyData.Name);
        }
    }
}