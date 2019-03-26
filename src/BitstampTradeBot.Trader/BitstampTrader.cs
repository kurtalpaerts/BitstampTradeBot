using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using BitstampTradeBot.Exchange;
using BitstampTradeBot.Models;
using BitstampTradeBot.Trader.Data.Helpers;
using BitstampTradeBot.Trader.Data.Models;
using BitstampTradeBot.Trader.Data.Repositories;
using BitstampTradeBot.Trader.Models;
using BitstampTradeBot.Trader.Models.Exchange;
using BitstampTradeBot.Trader.TradeRules;
using Ninject;

namespace BitstampTradeBot.Trader
{
    public class BitstampTrader : IDisposable
    {
        public event EventHandler<Exception> ErrorOccured;
        public event EventHandler<BitstampTickerEventArgs> TickerRetrieved;
        public event EventHandler<BitstampOrderEventArgs> BuyLimitOrderPlaced;
        public event EventHandler<BitstampOrderEventArgs> BuyLimitOrderExecuted;
        public event EventHandler<BitstampOrderEventArgs> SellLimitOrderPlaced;
        public event EventHandler<BitstampOrderEventArgs> SellLimitOrderExecuted;

        internal List<ExchangeOrder> OpenOrders;

        private static int _counter;
        private Timer _mainTimer;
        internal readonly TimeSpan Interval;
        private readonly List<TradeRuleBase> _traderRules = new List<TradeRuleBase>();
        private readonly IExchange _exchange;
        private readonly IRepository<MinMaxLog> _minMaxLogRepository;
        private readonly IRepository<Order> _orderRepository;
        private readonly IRepository<CurrencyPair> _currencyPairRepository;

        public BitstampTrader(TimeSpan interval)
        {
            // Ninject
            var kernel = new StandardKernel();
            kernel.Load(Assembly.GetExecutingAssembly());
            _minMaxLogRepository = kernel.Get<IRepository<MinMaxLog>>();
            _orderRepository = kernel.Get<IRepository<Order>>();
            _currencyPairRepository = kernel.Get<IRepository<CurrencyPair>>();
            _exchange = kernel.Get<IExchange>();

            if (_currencyPairRepository.GetType() == typeof(MockRepository<CurrencyPair>))
            {
                GenerateMockData();
            }

            Interval = interval;
        }

        public void AddTradeRule(TradeRuleBase tradeRule)
        {
            _traderRules.Add(tradeRule);
        }

        public void Start()
        {
            _mainTimer = new Timer(TimerCallback, null, 0, Timeout.Infinite);
        }

        public void Stop()
        {
            _mainTimer.Dispose();
        }

        private async void TimerCallback(object state)
        {
            try
            {
                await UpdateTradingPairsInfo();
                await CheckCurrencyBoughtAsync();
                await CheckCurrencySoldAsync();
                await Trade();
            }
            catch (Exception e)
            {
                ErrorOccured?.Invoke(this, e);
            }

            _mainTimer.Change(Interval.Seconds * 1000, Timeout.Infinite);
        }

        private async Task Trade()
        {
            await _traderRules[_counter].ExecuteAsync();

            _counter++;

            if (_counter >= _traderRules.Count)
            {
                _counter = 0;
            }
        }

        #region proxy methods

        internal async Task<Ticker> GetTickerAsync(string pairCode)
        {
            var ticker = await _exchange.GetTickerAsync(pairCode);
            TickerRetrieved?.Invoke(this, new BitstampTickerEventArgs(ticker, pairCode));

            UpdateMinMaxLog(pairCode, ticker);

            return ticker;
        }

        internal async Task<ExchangeOrder> BuyLimitOrderAsync(TradeSession tradeSession, decimal amount, decimal price)
        {
            var executedOrder = await _exchange.BuyLimitOrderAsync(tradeSession.PairCode, amount, price);

            if (executedOrder.Id == 0) throw new Exception("Buy order not executed (order id = 0)");

            BuyLimitOrderPlaced?.Invoke(this, new BitstampOrderEventArgs(executedOrder));
            tradeSession.LastBuyTimestamp = DateTime.Now;

            return executedOrder;
        }

        internal long GetCurrencyPairId(string pairCode)
        {
            return _currencyPairRepository.First(p => p.PairCode == pairCode).Id;
        }

        internal void AddNewOrderToDb(ExchangeOrder order, Ticker ticker, TradeSettings tradeSettings, TradingPairInfo pairInfo)
        {
            _orderRepository.Add(new Order
            {
                BuyId = order.Id,
                CurrencyPairId = GetCurrencyPairId(order.PairCode),
                //CurrencyPair = _currencyPairRepository.First(p => p.PairCode == order.PairCode), //todo: check with concrete repository
                BuyAmount = order.Amount,
                BuyPrice = order.Price,
                SellAmount = tradeSettings.GetSellBaseAmount(ticker, pairInfo),
                SellPrice = tradeSettings.GetSellBasePrice(ticker, pairInfo)
            });
            _orderRepository.Save();
        }

        internal List<Order> GetOpenOrdersDb()
        {
            return _orderRepository.Where(o => o.SellTimestamp == null).ToList();
        }

        #endregion proxy methods

        #region helpers

        private void UpdateMinMaxLog(string pairCode, Ticker ticker)
        {
            // get the record of the current day
            var currentDay = DateTime.Now.Date;

            // get the currencypair
            var currencyPair = _currencyPairRepository.First(p => p.PairCode == pairCode);

            // get minMaxLog from database
            var minMaxLog = _minMaxLogRepository.ToList().FirstOrDefault(l => l.Day == currentDay && l.CurrencyPairId == currencyPair.Id);

            // if the day record do not exist then add, otherwise update the min and max values if necessary
            if (minMaxLog == null)
            {
                _minMaxLogRepository.Add(new MinMaxLog { Day = currentDay, CurrencyPairId = currencyPair.Id, Minimum = ticker.Last, Maximum = ticker.Last });
            }
            else
            {
                if (minMaxLog.Minimum > ticker.Last) minMaxLog.Minimum = ticker.Last;
                if (minMaxLog.Maximum < ticker.Last) minMaxLog.Maximum = ticker.Last;
            }

            // save changes to database
            _minMaxLogRepository.Save();
        }

        private async Task CheckCurrencyBoughtAsync()
        {
            OpenOrders = await _exchange.GetOpenOrdersAsync();
            
            // loop through all open buy orders in db
            foreach (var order in _orderRepository.Where(o => o.BuyTimestamp == null).ToList())
            {
                // can the order be found in the exchange orders?
                if (OpenOrders.Any(o => o.Id == order.BuyId)) continue;

                // order not found in the exchange orders, so the buy order has been executed --> sell the bought currency

                // update buy price
                var transactions = await _exchange.GetTransactionsAsync();
                var transaction = transactions.First(t => t.OrderId == order.BuyId);
                if (order.BuyPrice != transaction.Price)
                {
                    order.BuyPrice = transaction.Price;
                }
                order.BuyTimestamp = transaction.Timestamp;

                var paircode = _currencyPairRepository.First(p=> p.Id == order.CurrencyPairId).PairCode;

                BuyLimitOrderExecuted?.Invoke(this, new BitstampOrderEventArgs(new ExchangeOrder
                {
                    Amount = order.BuyAmount,
                    Price = order.BuyPrice,
                    Id = order.BuyId,
                    PairCode = paircode,
                    Timestamp = transaction.Timestamp
                }));

                // get pair info from cache
                var pairInfo = CacheHelper.GetFromCache<List<TradingPairInfo>>("TradingPairInfo").First(i => i.PairCode == paircode);

                // sell currency on Bitstamp exchange
                var orderResult = await _exchange.SellLimitOrderAsync(paircode, Math.Round(order.SellAmount, pairInfo.BaseDecimals), Math.Round(order.SellPrice, pairInfo.CounterDecimals));

                if (orderResult.Id == 0) throw new Exception("Sell order not executed (order id = 0)");

                orderResult.PairCode = paircode;
                SellLimitOrderPlaced?.Invoke(this, new BitstampOrderEventArgs(orderResult));

                // update order in database
                order.SellId = orderResult.Id;
                _orderRepository.Update(order);
                _orderRepository.Save();
            }
        }

        private async Task CheckCurrencySoldAsync()
        {
            OpenOrders = await _exchange.GetOpenOrdersAsync();

            // loop through the open sell orders in database
            foreach (var order in _orderRepository.Where(o => o.BuyTimestamp != null && o.SellTimestamp == null).ToList())
            {
                // can the order be found in the exchange?
                if (OpenOrders.All(o => o.Id != order.SellId))
                {
                    // sell order not found, so it has executed

                    // todo find the according transaction

                    var paircode = _currencyPairRepository.First(p => p.Id == order.CurrencyPairId).PairCode;

                    SellLimitOrderExecuted?.Invoke(this, new BitstampOrderEventArgs(new ExchangeOrder
                    {
                        Amount = order.SellAmount, PairCode = paircode, Price = order.SellPrice
                    }));

                    // update database
                    order.SellTimestamp = DateTime.Now; // todo: get timestamp and fee from transaction
                    _orderRepository.Update(order);
                    _orderRepository.Save();
                }
            }
        }

        private async Task UpdateTradingPairsInfo()
        {
            if (!CacheHelper.IsIncache("TradingPairInfo"))
            {
                var pairsInfo = await _exchange.GetPairsInfoAsync();

                // tradingpair info isn't likely to change
                // cache the pairsinfo for 4 hours to reduce the number of api calls (Bitstamp allows only 600 calls per 10 minutes)
                CacheHelper.SaveTocache("TradingPairInfo", pairsInfo, DateTime.Now.AddHours(4));
            }
        }

        private void GenerateMockData()
        {
            _currencyPairRepository.Add(new CurrencyPair { Id = 1, PairCode = "btcusd" });
            _currencyPairRepository.Add(new CurrencyPair { Id = 2, PairCode = "btceur" });
            _currencyPairRepository.Add(new CurrencyPair { Id = 3, PairCode = "eurusd" });
            _currencyPairRepository.Add(new CurrencyPair { Id = 4, PairCode = "xrpusd" });
            _currencyPairRepository.Add(new CurrencyPair { Id = 5, PairCode = "xrpeur" });
            _currencyPairRepository.Add(new CurrencyPair { Id = 6, PairCode = "xrpbtc" });
            _currencyPairRepository.Add(new CurrencyPair { Id = 7, PairCode = "ltcusd" });
            _currencyPairRepository.Add(new CurrencyPair { Id = 8, PairCode = "ltceur" });
            _currencyPairRepository.Add(new CurrencyPair { Id = 9, PairCode = "ltcbtc" });
            _currencyPairRepository.Add(new CurrencyPair { Id = 10, PairCode = "ethusd" });
            _currencyPairRepository.Add(new CurrencyPair { Id = 11, PairCode = "etheur" });
            _currencyPairRepository.Add(new CurrencyPair { Id = 12, PairCode = "ethbtc" });
            _currencyPairRepository.Add(new CurrencyPair { Id = 13, PairCode = "bchusd" });
            _currencyPairRepository.Add(new CurrencyPair { Id = 14, PairCode = "bcheur" });
            _currencyPairRepository.Add(new CurrencyPair { Id = 15, PairCode = "bchbtc" });
        }

        #endregion helpers

        public void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool managedResources)
        {
            if (managedResources)
            {
                _mainTimer.Dispose();
            }
        }
    }
}