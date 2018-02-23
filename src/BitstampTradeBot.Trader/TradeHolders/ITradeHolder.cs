using System.Threading.Tasks;
using BitstampTradeBot.Trader.TradeRules;

namespace BitstampTradeBot.Trader.TradeHolders
{
    public interface ITradeHolder
    {
        Task<bool> ExecuteAsync(TradeRuleBase tradeRule);
    }
}