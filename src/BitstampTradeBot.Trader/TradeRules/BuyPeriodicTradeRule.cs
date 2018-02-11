using System;
using System.Threading.Tasks;
using BitstampTradeBot.Data.Models;
using BitstampTradeBot.Data.Repositories;
using BitstampTradeBot.Exchange;
using BitstampTradeBot.Trader.TraderRules;

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

        public async Task ExecuteAsync(BitstampExchange bitstampExchange, IRepository<MinMaxLog> minMaxLogRepository, IRepository<Order> orderRepository)
        {
            if (_lastBuyTimestamp == DateTime.MinValue)
            {
                _lastBuyTimestamp = DateTime.Now;
            }

            if (DateTime.Now > _lastBuyTimestamp.Add(_period))
            {
                // get ticker
                var ticker = await bitstampExchange.GetTickerAsync(_pairCode);

                // buy

                _lastBuyTimestamp = DateTime.Now;
            }
        }
    }
}
