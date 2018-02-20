﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
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
    public class BitstampTrader
    {
        public event EventHandler<Exception> ErrorOccured;
        public event EventHandler<BitstampTickerEventArgs> TickerRetrieved;
        public event EventHandler<BitstampOrderEventArgs> BuyLimitOrderPlaced;
        public event EventHandler<BitstampOrderEventArgs> BuyLimitOrderExecuted;
        public event EventHandler<BitstampOrderEventArgs> SellLimitOrderPlaced;

        private static int _counter;
        private Timer _mainTimer;
        private readonly int _dueTime;
        private readonly List<TradeRuleBase> _traderRules = new List<TradeRuleBase>();
        private readonly BitstampClient _bitstampClient;
        private readonly IRepository<MinMaxLog> _minMaxLogRepository;
        private readonly IRepository<Order> _orderRepository;
        private readonly IRepository<CurrencyPair> _currencyPairRepository;

        public BitstampTrader(int dueTime)
        {
            // Ninject
            var kernel = new StandardKernel();
            kernel.Load(Assembly.GetExecutingAssembly());
            _minMaxLogRepository = kernel.Get<IRepository<MinMaxLog>>();
            _orderRepository = kernel.Get<IRepository<Order>>();
            _currencyPairRepository = kernel.Get<IRepository<CurrencyPair>>();

            _dueTime = dueTime;

            _bitstampClient = new BitstampClient(ApiKeys.BitstampApiKey, ApiKeys.BitstampApiSecret, ApiKeys.BitstampCustomerId );
            CacheHelper.SaveTocache("TradingPairsDb", _currencyPairRepository.ToList(), DateTime.MaxValue);
        }

        public void AddTradeRule(TradeRuleBase tradeRule)
        {
            _traderRules.Add(tradeRule);
        }

        public void Start()
        {
            _mainTimer = new Timer(TimerCallback, null, _dueTime, Timeout.Infinite);
        }

        public void Stop()
        {
            _mainTimer.Dispose();
        }

        private async void TimerCallback(object state)
        {
            try
            {
                var transactions = await _bitstampClient.GetTransactions();

                await UpdateTradingPairsInfo();
                await CheckCurrencyBoughtAsync();
                await Trade();
            }
            catch (Exception e)
            {
                ErrorOccured?.Invoke(this, e);
            }

            _mainTimer.Change(_dueTime, Timeout.Infinite);
        }

        private async Task Trade()
        {
            await _traderRules[_counter].ExecuteAsync(this);

            _counter++;

            if (_counter >= _traderRules.Count)
            {
                _counter = 0;
            }
        }

        #region proxy methods

        internal async Task<Ticker> GetTickerAsync(string pairCode)
        {
            var ticker = await _bitstampClient.GetTickerAsync(pairCode);
            TickerRetrieved?.Invoke(this, new BitstampTickerEventArgs(ticker, pairCode));

            UpdateMinMaxLog(pairCode, ticker);

            return ticker;
        }

        internal async Task<ExchangeOrder> BuyLimitOrderAsync(string pairCode, decimal amount, decimal price)
        {
            var executedOrder = await _bitstampClient.BuyLimitOrderAsync(pairCode.ToLower(), amount, price);

            if (executedOrder.Id == 0) throw new Exception("Buy order not executed (order id = 0)");

            BuyLimitOrderPlaced?.Invoke(this, new BitstampOrderEventArgs(executedOrder));

            return executedOrder; 
        }

        #endregion proxy methods

        #region helpers

        private void UpdateMinMaxLog(string pairCode, Ticker ticker)
        {
            // get the record of the current day
            var currentDay = DateTime.Now.Date;

            // get the pair code id from cache
            var pairCodeId = CacheHelper.GetFromCache<List<CurrencyPair>>("TradingPairsDb").First(c => c.PairCode == pairCode).Id;

            // get minMaxLog from database
            var minMaxLog = _minMaxLogRepository.ToList().FirstOrDefault(l => l.Day == currentDay && l.CurrencyPairId == pairCodeId);

            // if the day record do not exist then add, otherwise update the min and max values if necessary
            if (minMaxLog == null)
            {
                _minMaxLogRepository.Add(new MinMaxLog { Day = currentDay, CurrencyPairId = pairCodeId, Minimum = ticker.Last, Maximum = ticker.Last });
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
            var openOrders = await _bitstampClient.GetOpenOrdersAsync();

            // loop through all open buy orders in db
            foreach (var order in _orderRepository.Where(o => o.BuyTimestamp == null).ToList())
            {
                // can the order be found in the exchange orders?
                if (openOrders.All(o => o.Id != order.BuyId))
                {
                    // order not found in the exchange orders, so the buy order has been executed --> sell the bought currency

                    // update buyprice
                    var transactions = await _bitstampClient.GetTransactions();
                    var transaction = transactions.First(t => t.Id == order.BuyId);
                    if (order.BuyPrice != transaction.Price)
                    {
                        order.BuyPrice = transaction.Price;
                    }
                    order.BuyTimestamp = transaction.Timestamp;

                    // todo
                    //BuyLimitOrderExecuted?.Invoke(this, new BitstampOrderEventArgs(new BitstampOrder
                    //{
                    //    Amount = order.BuyAmount,
                    //    Price = order.BuyPrice,
                    //    Id = order.BuyId,
                    //    PairCode = (BitstampPairCode)Enum.Parse(typeof(BitstampPairCode), order.CurrencyPair.PairCode),
                    //    Timestamp = transaction.Timestamp
                    //}));

                    // get pair info
                    var pairInfo = CacheHelper.GetFromCache<List<TradingPairInfo>>("TradingPairInfo").First(i => i.PairCode == order.CurrencyPair.PairCode.ToLower());

                    // sell currency on Bitstamp exchange
                    var pairCode = (BitstampPairCode)Enum.Parse(typeof(BitstampPairCode), order.CurrencyPair.PairCode);
                    var orderResult = await _bitstampClient.SellLimitOrderAsync(pairCode.ToString(), Math.Round(order.SellAmount, pairInfo.BaseDecimals), Math.Round(order.SellPrice, pairInfo.CounterDecimals));

                    if (orderResult.Id == 0) throw new Exception("Sell order not executed (order id = 0)");

                    SellLimitOrderPlaced?.Invoke(this, new BitstampOrderEventArgs(orderResult));

                    // update order in database
                    order.SellId = orderResult.Id;
                    _orderRepository.Update(order);
                    _orderRepository.Save();
                }
            }
        }

        private async Task UpdateTradingPairsInfo()
        {
            if (!CacheHelper.IsIncache("TradingPairInfo"))
            {
                var pairsInfo = await _bitstampClient.GetPairsInfoAsync();

                // cache the pairsinfo for 4 hours to reduce the number of api calls, Bitstamp allows only 600 calls per 10 minutes
                CacheHelper.SaveTocache("TradingPairInfo", pairsInfo, DateTime.Now.AddHours(4));
            }
        }

        #endregion helpers
    }
}
