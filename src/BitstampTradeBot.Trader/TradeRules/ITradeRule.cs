using System.Threading.Tasks;

namespace BitstampTradeBot.Trader.TradeRules
{
    public interface ITradeRule
    {
        Task ExecuteAsync(BitstampTrader bitstampTrader);
    }
}
