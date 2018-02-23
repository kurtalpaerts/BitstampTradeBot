using System;
using BitstampTradeBot.Models;

namespace BitstampTradeBot.Trader.Models
{
    public class TradeSettings
    {
        public string PairCode { get; set; }
        public decimal BuyUnderPriceMargin { private get; set; }
        public decimal CounterAmount { private get; set; }
        public decimal BaseAmountSavingsRate { private get; set; }
        public decimal SellPriceRate { private get; set; }

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
