using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BitstampTradeBot.Exchange.Helpers;
using BitstampTradeBot.Models;

namespace BitstampTradeBot.Exchange
{
    public class MockExchange : IExchange
    {
        private readonly List<KeyValuePair<string, Ticker>> _tickers = new List<KeyValuePair<string, Ticker>>();
        private readonly List<ExchangeOrder> _openOrders = new List<ExchangeOrder>();
        private readonly List<Transaction> _transactions = new List<Transaction>();
        private readonly AccountBalance _accountBalance = new AccountBalance();

        private readonly IdGenerator _openOrdersIds = new IdGenerator();

        public MockExchange()
        {
            InitializeTickers();
        }

        public Task<Ticker> GetTickerAsync(string pairCode)
        {
            return Task.Run(() => _tickers.First(t => t.Key == pairCode).Value);
        }

        public Task<AccountBalance> GetAccountBalanceAsync()
        {
            return Task.Run(() => _accountBalance);
        }

        public Task<List<ExchangeOrder>> GetOpenOrdersAsync()
        {
            return Task.Run(() => _openOrders);
        }

        public Task<List<Transaction>> GetTransactionsAsync()
        {
            return Task.Run(() => _transactions);
        }

        public Task<List<TradingPairInfo>> GetPairsInfoAsync()
        {
            var pairInfo = new List<TradingPairInfo>
            {
                new TradingPairInfo{PairCode = "btcusd", BaseDecimals = 8, CounterDecimals = 2},
                new TradingPairInfo{PairCode = "btceur", BaseDecimals = 8, CounterDecimals = 2},
                new TradingPairInfo{PairCode = "eurusd", BaseDecimals = 5, CounterDecimals = 5},
                new TradingPairInfo{PairCode = "xrpusd", BaseDecimals = 8, CounterDecimals = 5},
                new TradingPairInfo{PairCode = "xrpeur", BaseDecimals = 8, CounterDecimals = 5},
                new TradingPairInfo{PairCode = "xrpbtc", BaseDecimals = 8, CounterDecimals = 8},
                new TradingPairInfo{PairCode = "ltcusd", BaseDecimals = 8, CounterDecimals = 2},
                new TradingPairInfo{PairCode = "ltceur", BaseDecimals = 8, CounterDecimals = 2},
                new TradingPairInfo{PairCode = "ltcbtc", BaseDecimals = 8, CounterDecimals = 8},
                new TradingPairInfo{PairCode = "ethusd", BaseDecimals = 8, CounterDecimals = 2},
                new TradingPairInfo{PairCode = "etheur", BaseDecimals = 8, CounterDecimals = 2},
                new TradingPairInfo{PairCode = "ethbtc", BaseDecimals = 8, CounterDecimals = 8},
                new TradingPairInfo{PairCode = "bchusd", BaseDecimals = 8, CounterDecimals = 2},
                new TradingPairInfo{PairCode = "bcheur", BaseDecimals = 8, CounterDecimals = 2},
                new TradingPairInfo{PairCode = "bchbtc", BaseDecimals = 8, CounterDecimals = 8}
            };

            return Task.Run(() => pairInfo);
        }

        public Task<ExchangeOrder> BuyLimitOrderAsync(string pairCode, decimal amount, decimal price)
        {
            var newOrder = new ExchangeOrder { Id = _openOrdersIds.GetNextId(), Timestamp = DateTime.Now, PairCode = pairCode, Amount = amount, Price = price, Type = BitstampOrderType.Buy };
            _openOrders.Add(newOrder);

            return Task.Run(() => newOrder);
        }

        public Task<ExchangeOrder> SellLimitOrderAsync(string pairCode, decimal amount, decimal price)
        {
            throw new NotImplementedException();
        }

        private void InitializeTickers()
        {
            _tickers.Add(new KeyValuePair<string, Ticker>("btcusd", new Ticker { Last = 10050, Timestamp = DateTime.Now }));
            _tickers.Add(new KeyValuePair<string, Ticker>("btceur", new Ticker { Last = 8150, Timestamp = DateTime.Now }));
            _tickers.Add(new KeyValuePair<string, Ticker>("eurusd", new Ticker { Last = 1.20M, Timestamp = DateTime.Now }));
            _tickers.Add(new KeyValuePair<string, Ticker>("xrpusd", new Ticker { Last = 0.90M, Timestamp = DateTime.Now }));
            _tickers.Add(new KeyValuePair<string, Ticker>("xrpeur", new Ticker { Last = 0.75M, Timestamp = DateTime.Now }));
            _tickers.Add(new KeyValuePair<string, Ticker>("xrpbtc", new Ticker { Last = 0.000095M, Timestamp = DateTime.Now }));
            _tickers.Add(new KeyValuePair<string, Ticker>("ltcusd", new Ticker { Last = 205, Timestamp = DateTime.Now }));
            _tickers.Add(new KeyValuePair<string, Ticker>("ltceur", new Ticker { Last = 165, Timestamp = DateTime.Now }));
            _tickers.Add(new KeyValuePair<string, Ticker>("ltcbtc", new Ticker { Last = 0.02M, Timestamp = DateTime.Now }));
            _tickers.Add(new KeyValuePair<string, Ticker>("ethusd", new Ticker { Last = 840, Timestamp = DateTime.Now }));
            _tickers.Add(new KeyValuePair<string, Ticker>("etheur", new Ticker { Last = 680, Timestamp = DateTime.Now }));
            _tickers.Add(new KeyValuePair<string, Ticker>("ethbtc", new Ticker { Last = 0.085M, Timestamp = DateTime.Now }));
            _tickers.Add(new KeyValuePair<string, Ticker>("bchusd", new Ticker { Last = 1220, Timestamp = DateTime.Now }));
            _tickers.Add(new KeyValuePair<string, Ticker>("bcheur", new Ticker { Last = 985, Timestamp = DateTime.Now }));
            _tickers.Add(new KeyValuePair<string, Ticker>("bchbtc", new Ticker { Last = 0.125M, Timestamp = DateTime.Now }));
        }
    }
}
