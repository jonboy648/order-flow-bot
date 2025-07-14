using NinjaTrader.Custom.AddOns.OrderFlowBot.Configs;
using NinjaTrader.Custom.AddOns.OrderFlowBot.Containers;
using NinjaTrader.Custom.AddOns.OrderFlowBot.Models.DataBars.Base;
using System.Linq;

namespace NinjaTrader.Custom.AddOns.OrderFlowBot.Models.Strategies.Implementations
{
    public class VolumeSequencing : StrategyBase
    {
        private readonly int _validBarCount;
        private readonly int _validVolume;
        private readonly int _minSequentialLevels;

        public VolumeSequencing(EventsContainer eventsContainer) : base(eventsContainer)
        {
            StrategyData.Name = "Volume Sequencing";
            _validBarCount = 2;
            _validVolume = 500;
            _minSequentialLevels = 3; // Minimum levels required for volume sequencing
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

            return isLong ? 
                IsBullishBar() && HasAskVolumeSequencing() :
                IsBearishBar() && HasBidVolumeSequencing();
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

        private bool HasAskVolumeSequencing()
        {
            return CheckVolumeSequencing(true);
        }

        private bool HasBidVolumeSequencing()
        {
            return CheckVolumeSequencing(false);
        }

        private bool CheckVolumeSequencing(bool checkAsk)
        {
            var bidAskVolumes = currentDataBar.Volumes.BidAskVolumes;
            if (bidAskVolumes == null || bidAskVolumes.Count < _minSequentialLevels)
            {
                return false;
            }

            // Sort by price to ensure we're checking in sequence
            var sortedVolumes = bidAskVolumes.OrderBy(v => v.Price).ToList();
            
            int consecutiveCount = 0;
            int maxConsecutiveCount = 0;

            for (int i = 0; i < sortedVolumes.Count; i++)
            {
                long targetVolume = checkAsk ? sortedVolumes[i].AskVolume : sortedVolumes[i].BidVolume;
                
                // Check if this level has significant volume
                if (targetVolume > 0)
                {
                    consecutiveCount++;
                    maxConsecutiveCount = System.Math.Max(maxConsecutiveCount, consecutiveCount);
                }
                else
                {
                    consecutiveCount = 0;
                }
            }

            return maxConsecutiveCount >= _minSequentialLevels;
        }
    }
}