using System.Linq;
using BitstampTradeBot.Models;
using BitstampTradeBot.Trader.Models;

namespace BitstampTradeBot.Trader.TradeHolders
{
    public class MaxNumberOfBuyOrdersHolder : ITradeHolder
    {
        private readonly int _maxNumberOfBuyOrders;

        public MaxNumberOfBuyOrdersHolder(int maxNumberOfBuyOrders)
        {
            _maxNumberOfBuyOrders = maxNumberOfBuyOrders;
        }
        
        public bool Execute(TradeSession tradeSession)
        {
            var openOrders = tradeSession.OpenOrders.Where(o=> o.PairCode == tradeSession.PairCode);

            return openOrders.Count(o => o.Type == BitstampOrderType.Buy) >= _maxNumberOfBuyOrders;
        }
    }
}