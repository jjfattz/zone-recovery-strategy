﻿using System;

namespace ZoneRecoveryAlgorithm
{           
    public class Session: IActiveTurn
    {               

        public RecoveryTurn ActivePosition { get; private set; }
        public ZoneLevels ZoneLevels { get; }

        public double UnrealizedGrossProfit { get { return CalculateUnrealizedGrossProfit(); } }
        public double UnrealizedNetProfit { get { return CalculateUnrealizedNetProfit(); } }

        public double TotalLotSize { get { return CalculateTotalLotSize(); } }

        public double RecoveryTurns {  get { return ActivePosition.TurnIndex; } }

        public Session(MarketPosition initPosition, double entryBidPrice, double entryAskPrice, double initLotSize, double spread, double commission, double slippage, int tradeZoneSize, int zoneRecoverySize)
        {
            ZoneLevels = new ZoneLevels((entryBidPrice + entryAskPrice)/2d, tradeZoneSize, zoneRecoverySize);

            ActivePosition = new RecoveryTurn(this, null, ZoneLevels, initPosition, initPosition, entryBidPrice, entryAskPrice, initLotSize, spread, commission, slippage);

        }

        public Tuple<PriceActionResult, RecoveryTurn> PriceAction(double bid, double ask)
        {
            return ActivePosition.PriceAction(bid, ask);
        }

        private double CalculateUnrealizedNetProfit()
        {
            var turn = ActivePosition;
            double totalNetReturns = 0;
            while (turn != null)
            {
                totalNetReturns += turn.UnrealizedNetProfit;
                turn = turn.PreviousTurn;
            }

            return totalNetReturns;
        }

        private double CalculateUnrealizedGrossProfit()
        {
            var turn = ActivePosition;
            double totalNetReturns = 0;
            while (turn != null)
            {
                totalNetReturns += turn.UnrealizedGrossProfit;
                turn = turn.PreviousTurn;
            }

            return totalNetReturns;
        }

        private double CalculateTotalLotSize()
        {
            var turn = ActivePosition;
            double totalLotSize = 0;
            while (turn != null)
            {
                totalLotSize += turn.LotSize;
                turn = turn.PreviousTurn;
            }

            return totalLotSize;
        }

        public void Update(RecoveryTurn activeTurn)
        {
            ActivePosition = activeTurn;
        }
    }    
}