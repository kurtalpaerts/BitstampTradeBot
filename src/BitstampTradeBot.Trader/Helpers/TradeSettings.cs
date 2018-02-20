using System;
using BitstampTradeBot.Models;

namespace BitstampTradeBot.Trader.Helpers
{
    public class TradeSettings
    {
        public string PairCode { get; set; }
        public decimal BuyUnderPriceMargin { get; set; }
        public decimal CounterAmount { get; set; }
        public decimal BaseAmountSavingsRate { get; set; }
        public decimal SellPriceRate { get; set; }

        public decimal GetBuyBaseAmount(Ticker ticker, TradingPairInfo pairInfo)
        {
            return Math.Round(CounterAmount / GetBuyBasePrice(ticker, pairInfo), pairInfo.BaseDecimals);
        }

        public decimal GetBuyBasePrice(Ticker ticker, TradingPairInfo pairInfo)
        {
            return Math.Round(ticker.Last * (1 - BuyUnderPriceMargin / 100), pairInfo.CounterDecimals);
        }

        public decimal GetSellBaseAmount(Ticker ticker, TradingPairInfo pairInfo)
        {
            var buyAmount = GetBuyBaseAmount(ticker, pairInfo);

            return buyAmount - buyAmount * (BaseAmountSavingsRate / 100);
        }

        public decimal GetSellBasePrice(Ticker ticker, TradingPairInfo pairInfo)
        {
            var buyPrice = GetBuyBasePrice(ticker, pairInfo);

            return Math.Round(buyPrice * (1 + SellPriceRate / 100), pairInfo.CounterDecimals);
        }
    }
}
