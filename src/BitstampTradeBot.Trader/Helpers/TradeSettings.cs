using System;
using System.Collections.Generic;
using System.Linq;
using BitstampTradeBot.Trader.Data.Helpers;
using BitstampTradeBot.Trader.Models.Exchange;

namespace BitstampTradeBot.Trader.Helpers
{
    public class TradeSettings
    {
        public decimal BuyUnderPriceMargin { get; set; }
        public decimal CounterAmount { get; set; }

        public decimal GetBaseAmount(BitstampTicker ticker, BitstampTradingPairInfo pairInfo)
        {
            return Math.Round(CounterAmount / GetBasePrice(ticker, pairInfo), pairInfo.BaseDecimals);
        }

        public decimal GetBasePrice(BitstampTicker ticker, BitstampTradingPairInfo pairInfo)
        {
            return Math.Round(ticker.Last * (1 - BuyUnderPriceMargin / 100), pairInfo.CounterDecimals);
        }

        //private BitstampTradingPairInfo GetPairInfo(BitstampTicker ticker)
        //{
        //    return CacheHelper.GetFromCache<List<BitstampTradingPairInfo>>("TradingPairInfo").First(i => i.UrlSymbol == ticker.PairCode.ToLower());
        //}
    }
}
