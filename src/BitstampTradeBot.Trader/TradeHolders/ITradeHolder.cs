using BitstampTradeBot.Trader.TradeRules;

namespace BitstampTradeBot.Trader.TradeHolders
{
    public interface ITradeHolder
    {
        bool Execute(TradeRuleBase tradeRule);
    }
}