using System;
using System.Collections.Generic;
using System.Linq;
using BitstampTradeBot.Models;
using BitstampTradeBot.Models.Helpers;
using BitstampTradeBot.Trader.Models.Exchange;

namespace BitstampTradeBot.Trader.Helpers
{
    public class TradeSettings
    {
        public decimal BuyUnderPriceMargin { get; set; }
        public decimal CounterAmount { get; set; }

        public decimal GetBaseAmount(BitstampTicker ticker)
        {
            return Math.Round(CounterAmount / GetBasePrice(ticker), GetPairInfo(ticker).BaseDecimals);
        }

        public decimal GetBasePrice(BitstampTicker ticker)
        {
            return Math.Round(ticker.Last * (1 - BuyUnderPriceMargin / 100), GetPairInfo(ticker).CounterDecimals);
        }

        private BitstampTradingPairInfo GetPairInfo(BitstampTicker ticker)
        {
            return CacheHelper.GetFromCache<List<BitstampTradingPairInfo>>("TradingPairInfo").First(i => i.UrlSymbol == ticker.PairCode.ToLower());
        }
    }
}
