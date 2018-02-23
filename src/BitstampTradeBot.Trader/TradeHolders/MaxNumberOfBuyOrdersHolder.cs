using System.Linq;
using System.Threading.Tasks;
using BitstampTradeBot.Models;
using BitstampTradeBot.Trader.TradeRules;

namespace BitstampTradeBot.Trader.TradeHolders
{
    public class MaxNumberOfBuyOrdersHolder : ITradeHolder
    {
        private readonly int _maxNumberOfBuyOrders;

        public MaxNumberOfBuyOrdersHolder(int maxNumberOfBuyOrders)
        {
            _maxNumberOfBuyOrders = maxNumberOfBuyOrders;
        }
        public async Task<bool> ExecuteAsync(TradeRuleBase tradeRule)
        {
            var openOrders = await tradeRule.BitstampTrader.OpenOrdersAsync(tradeRule.TradeSettings.PairCode);

            return openOrders.Count(o => o.Type == BitstampOrderType.Buy) > _maxNumberOfBuyOrders;
        }
    }
}