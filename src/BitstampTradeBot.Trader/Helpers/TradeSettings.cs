using System;
using BitstampTradeBot.Exchange.Models;
using BitstampTradeBot.Trader.Models;
using BitstampTradeBot.Trader.Models.Exchange;

namespace BitstampTradeBot.Trader.Helpers
{
    public class TradeSettings
    {
        public BitstampPairCode PairCode { get; set; }
        public decimal BuyUnderPriceMargin { get; set; }
        public decimal CounterAmount { get; set; }
        public decimal BaseAmountSavingsRate { get; set; }
        public decimal SellPriceRate { get; set; }

        public decimal GetBuyBaseAmount(BitstampTicker ticker, BitstampTradingPairInfo pairInfo)
        {
            return Math.Round(CounterAmount / GetBuyBasePrice(ticker, pairInfo), pairInfo.BaseDecimals);
        }

        public decimal GetBuyBasePrice(BitstampTicker ticker, BitstampTradingPairInfo pairInfo)
        {
            return Math.Round(ticker.Last * (1 - BuyUnderPriceMargin / 100), pairInfo.CounterDecimals);
        }

        public decimal GetSellBaseAmount(BitstampTicker ticker, BitstampTradingPairInfo pairInfo)
        {
            var buyAmount = GetBuyBaseAmount(ticker, pairInfo);

            return buyAmount - buyAmount * (BaseAmountSavingsRate / 100);
        }

        public decimal GetSellBasePrice(BitstampTicker ticker, BitstampTradingPairInfo pairInfo)
        {
            var buyPrice = GetBuyBasePrice(ticker, pairInfo);

            return Math.Round(buyPrice * (1 + SellPriceRate / 100), pairInfo.CounterDecimals);
        }
    }
}
