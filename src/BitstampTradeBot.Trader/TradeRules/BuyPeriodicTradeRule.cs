using System;
using System.Threading.Tasks;
using BitstampTradeBot.Models;
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
                var amount = CurrencyPairCalculator.AmountCredit(ticker, 10);
                // var orderResult = await bitstampTrader.BitstampExchange.BuyLimitOrderAsync(_pairCode, amount, ticker.Last * 0.9M);

                _lastBuyTimestamp = DateTime.Now;
            }
        }
    }
}
