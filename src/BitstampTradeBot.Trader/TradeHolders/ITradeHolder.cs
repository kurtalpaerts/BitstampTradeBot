using BitstampTradeBot.Trader.Models;

namespace BitstampTradeBot.Trader.TradeHolders
{
    public interface ITradeHolder
    {
        bool Execute(TradeSession tradeSession);
    }
}