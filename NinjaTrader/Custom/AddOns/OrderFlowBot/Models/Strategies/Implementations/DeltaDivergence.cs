using NinjaTrader.Custom.AddOns.OrderFlowBot.Configs;
using NinjaTrader.Custom.AddOns.OrderFlowBot.Containers;

namespace NinjaTrader.Custom.AddOns.OrderFlowBot.Models.Strategies.Implementations
{
    public class DeltaDivergence : StrategyBase
    {
        private readonly int _validBarCount;
        private readonly int _validVolume;
        private readonly long _validDelta;
        private readonly int _lookBackBars;

        public DeltaDivergence(EventsContainer eventsContainer) : base(eventsContainer)
        {
            StrategyData.Name = "Delta Divergence";
            _validBarCount = 2;
            _validVolume = 500;
            _validDelta = 1; // Minimum delta threshold
            _lookBackBars = 5; // Number of bars to look back for price comparison
        }

        public override bool CheckLong()
        {
            return CheckDirection(isLong: true);
        }

        public override bool CheckShort()
        {
            return CheckDirection(isLong: false);
        }

        private bool CheckDirection(bool isLong)
        {
            if (!IsValidBarCount() ||
                !IsValidVolume() ||
                !IsValidTriggerStrikePrice())
            {
                return false;
            }

            if (dataBars.Count < _lookBackBars)
            {
                return false;
            }

            return isLong ? 
                IsBullishBar() && DeltaIsPositive() && IsValidLastBarsForBullishDivergence() :
                IsBearishBar() && DeltaIsNegative() && IsValidLastBarsForBearishDivergence();
        }

        private bool IsValidBarCount()
        {
            return dataBars.Count > _validBarCount;
        }

        private bool IsValidVolume()
        {
            return currentDataBar.Volumes.Volume > _validVolume;
        }

        private bool IsBullishBar()
        {
            return currentDataBar.BarType == BarType.Bullish;
        }

        private bool IsBearishBar()
        {
            return currentDataBar.BarType == BarType.Bearish;
        }

        private bool DeltaIsPositive()
        {
            return currentDataBar.Deltas.Delta > _validDelta;
        }

        private bool DeltaIsNegative()
        {
            return currentDataBar.Deltas.Delta < -_validDelta;
        }

        // Price makes a new low based on the last x bars, delta is positive and the bar is bullish
        private bool IsValidLastBarsForBullishDivergence()
        {
            double currentBarLowPrice = currentDataBar.Prices.Low;

            // Check if the current bar's low price is lower than each of the preceding bars
            for (int i = dataBars.Count - 1; i > dataBars.Count - 1 - _lookBackBars; i--)
            {
                if (i < 0 || currentBarLowPrice >= dataBars[i].Prices.Low)
                {
                    return false;
                }
            }

            return true;
        }

        // Price makes a new high based on the last x bars, delta is negative and the bar is bearish
        private bool IsValidLastBarsForBearishDivergence()
        {
            double currentBarHighPrice = currentDataBar.Prices.High;

            // Check if the current bar's high price is higher than each of the preceding bars
            for (int i = dataBars.Count - 1; i > dataBars.Count - 1 - _lookBackBars; i--)
            {
                if (i < 0 || currentBarHighPrice <= dataBars[i].Prices.High)
                {
                    return false;
                }
            }

            return true;
        }
    }
}