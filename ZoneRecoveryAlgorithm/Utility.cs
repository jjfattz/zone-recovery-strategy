﻿using System.Collections.Generic;

namespace ZoneRecoveryAlgorithm
{
    public class Utility
    {
        public static (MarketPosition, double)[] GenerateLotSizes(double maxTurns, MarketPosition initPosition, double entryBidPrice, double entryAskPrice, double initLotSize, double spread, double pipFactor, double commission, double profitMargin, double slippage, double tradeZoneSize, double zoneRecoverySize)
        {
            var zoneRecovery = new ZoneRecovery(initLotSize, pipFactor, commission, profitMargin, slippage);

            var lotSizes = new List<(MarketPosition, double)>() { (initPosition, initLotSize) };

            var session = zoneRecovery.CreateSession(initPosition, entryBidPrice, entryAskPrice, tradeZoneSize, zoneRecoverySize);

            var position = initPosition;            

            for (int index=0; index<maxTurns; index++)
            {
                double bid = session.ActivePosition.ZoneLevels.LossRecoveryLevel;
                double ask = bid + spread;                                  

                var (result, turn)  = session.PriceAction(bid, ask);

                if (result == PriceActionResult.RecoveryLevelHit)
                {
                    lotSizes.Add((turn.Position, turn.LotSize));
                }
                else
                {
                    throw new System.Exception("Unexpected price action result!");
                }

                position = position.Reverse();                
            }

            return lotSizes.ToArray();
        }
    }
}
