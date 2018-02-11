using System.Threading.Tasks;
using BitstampTradeBot.Data.Models;
using BitstampTradeBot.Data.Repositories;
using BitstampTradeBot.Exchange;
using BitstampTradeBot.Trader.TraderRules;

namespace BitstampTradeBot.Trader.TraderRules
{
    public interface ITradeRule
    {
        Task ExecuteAsync(BitstampExchange bitstampExchange, IRepository<MinMaxLog> minMaxLogRepository, IRepository<Order> orderRepository);
    }
}
