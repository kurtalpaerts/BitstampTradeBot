using System.Linq;
using BitstampTradeBot.Models;
using BitstampTradeBot.Trader.Models;

namespace BitstampTradeBot.Trader.TradeHolders
{
    public class MaxNumberOfSellOrdersHolder : ITradeHolder
    {
        private readonly int _maxNumberOfSellOrders;

        public MaxNumberOfSellOrdersHolder(int maxNumberOfSellOrders)
        {
            _maxNumberOfSellOrders = maxNumberOfSellOrders;
        }

        public bool Execute(TradeSession tradeSession)
        {
            var openOrders = tradeSession.OpenOrders.Where(o => o.PairCode == tradeSession.PairCode).ToList();

            return openOrders.Count(o => o.Type == BitstampOrderType.Sell) >= _maxNumberOfSellOrders;
        }
    }
}
