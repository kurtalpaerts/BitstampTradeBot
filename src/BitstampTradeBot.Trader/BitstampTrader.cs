using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using BitstampTradeBot.Models;
using BitstampTradeBot.Trader.Data.Models;
using BitstampTradeBot.Trader.Data.Repositories;
using BitstampTradeBot.Trader.Helpers;
using BitstampTradeBot.Trader.Models;
using BitstampTradeBot.Trader.Models.Exchange;
using BitstampTradeBot.Trader.TradeRules;
using Ninject;

namespace BitstampTradeBot.Trader
{
    public class BitstampTrader
    {
        public event EventHandler<Exception> ErrorOccured;
        public event EventHandler<BitstampTicker> TickerRetrieved;
        public event EventHandler<BitstampOrder> BuyLimitOrderPlaced;

        private static int _counter;
        private Timer _mainTimer;
        private readonly int _dueTime;
        private readonly List<TradeRuleBase> _traderRules;
        private readonly BitstampExchange BitstampExchange;
        private readonly IRepository<MinMaxLog> _minMaxLogRepository;
        private readonly IRepository<Order> _orderRepository;
        private readonly IRepository<CurrencyPair> _currencyPairRepository;

        public BitstampTrader(int dueTime, params TradeRuleBase[] tradeRules)
        {
            // Ninject
            var kernel = new StandardKernel();
            kernel.Load(Assembly.GetExecutingAssembly());
            _minMaxLogRepository = kernel.Get<IRepository<MinMaxLog>>();
            _orderRepository = kernel.Get<IRepository<Order>>();
            _currencyPairRepository = kernel.Get<IRepository<CurrencyPair>>();

            _dueTime = dueTime;
            _traderRules = tradeRules.ToList();

            BitstampExchange = new BitstampExchange();
            CacheHelper.SaveTocache("TradingPairsDb", _currencyPairRepository.ToList(), DateTime.MaxValue);
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
                await UpdateTradingPairsInfo();
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

        internal async Task<BitstampTicker> GetTickerAsync(BitstampPairCode pairCode)
        {
            var ticker = await BitstampExchange.GetTickerAsync(pairCode);
            TickerRetrieved?.Invoke(this, ticker);

            UpdateMinMaxLog(pairCode, ticker);

            return ticker;
        }

        internal async Task<BitstampOrder> BuyLimitOrderAsync(BitstampPairCode pairCode, decimal amount, decimal price)
        {
            var executedOrder = await BitstampExchange.BuyLimitOrderAsync(pairCode, amount, price);
            BuyLimitOrderPlaced?.Invoke(this, executedOrder);

            return executedOrder;
        }

        #endregion

        #region helpers

        private void UpdateMinMaxLog(BitstampPairCode pairCode, BitstampTicker ticker)
        {
            // get the record of the current day
            var currentDay = DateTime.Now.Date;

            // get the pair code id from cache
            var pairCodeId = CacheHelper.GetFromCache<List<CurrencyPair>>("TradingPairsDb").First(c => c.PairCode == pairCode.ToString()).Id;

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

        private async Task UpdateTradingPairsInfo()
        {
            if (!CacheHelper.IsIncache("TradingPairInfo"))
            {
                var pairsInfo = await BitstampExchange.GetPairsInfoAsync();

                // cache the pairsinfo for 4 hours to reduce the number of api calls, Bitstamp allows only 600 calls per 10 minutes
                CacheHelper.SaveTocache("TradingPairInfo", pairsInfo, DateTime.Now.AddHours(4));
            }
        }

        #endregion helpers
    }
}
