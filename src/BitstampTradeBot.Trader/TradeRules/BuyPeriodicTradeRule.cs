using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BitstampTradeBot.Models;
using BitstampTradeBot.Models.Helpers;
using BitstampTradeBot.Trader.Helpers;

namespace BitstampTradeBot.Trader.TradeRules
{
    public class BuyPeriodicTradeRule : ITradeRule
    {
        private readonly BitstampPairCode _pairCode;
        private readonly TimeSpan _period;
        private DateTime _lastBuyTimestamp;

        public BuyPeriodicTradeRule(BitstampPairCode pairCode, TimeSpan period)
        {
            _pairCode = pairCode;
            _period = period;
        }

        public async Task ExecuteAsync(BitstampTrader bitstampTrader)
        {
            if (_lastBuyTimestamp == DateTime.MinValue)
            {
                _lastBuyTimestamp = DateTime.Now;
            }

            if (DateTime.Now > _lastBuyTimestamp.Add(_period))
            {
                // get ticker
                var ticker = await bitstampTrader.GetTickerAsync(_pairCode);

                // buy
                var tradingPairInfo = CacheHelper.GetFromCache<List<BitstampTradingPairInfo>>("TradingPairInfo").First(i => i.UrlSymbol == _pairCode.ToLower());

                var price = Math.Round(ticker.Last * 0.9M, tradingPairInfo.CounterDecimals);
                var amount = Math.Round(CurrencyPairCalculator.AmountBase(price, 10), tradingPairInfo.BaseDecimals);

                var orderResult = await bitstampTrader.BuyLimitOrderAsync(_pairCode, amount, price);

                _lastBuyTimestamp = DateTime.Now;
            }
        }
    }
}
