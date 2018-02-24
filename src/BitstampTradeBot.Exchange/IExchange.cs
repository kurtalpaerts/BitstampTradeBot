using System.Collections.Generic;
using System.Threading.Tasks;
using BitstampTradeBot.Models;

namespace BitstampTradeBot.Exchange
{
    public interface IExchange
    {
        Task<Ticker> GetTickerAsync(string pairCode);
        Task<AccountBalance> GetAccountBalanceAsync();
        Task<List<ExchangeOrder>> GetOpenOrdersAsync();
        Task<List<Transaction>> GetTransactionsAsync();
        Task<List<TradingPairInfo>> GetPairsInfoAsync();
        Task<ExchangeOrder> BuyLimitOrderAsync(string pairCode, decimal amount, decimal price);
        Task<ExchangeOrder> SellLimitOrderAsync(string pairCode, decimal amount, decimal price);
    }
}