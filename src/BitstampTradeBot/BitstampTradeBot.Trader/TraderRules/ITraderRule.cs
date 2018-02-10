using System.Threading.Tasks;

namespace BitstampTradeBot.Trader.TraderRules
{
    public interface ITraderRule
    {
        Task ExecuteAsync(BitstampTrader trader);
    }
}
