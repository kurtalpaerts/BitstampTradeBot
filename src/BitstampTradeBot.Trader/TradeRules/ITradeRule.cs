using System.Threading.Tasks;
using BitstampTradeBot.Exchange;

namespace BitstampTradeBot.Trader.TradeRules
{
    public interface ITradeRule
    {
        Task ExecuteAsync(BitstampExchange bitstampExchange);
    }
}
