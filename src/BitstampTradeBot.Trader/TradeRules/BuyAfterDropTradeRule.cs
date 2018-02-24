using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BitstampTradeBot.Models;
using BitstampTradeBot.Trader.Data.Helpers;
using BitstampTradeBot.Trader.Data.Models;
using BitstampTradeBot.Trader.Data.Repositories;
using BitstampTradeBot.Trader.Models;
using BitstampTradeBot.Trader.TradeHolders;

namespace BitstampTradeBot.Trader.TradeRules
{
    public class BuyAfterDropTradeRule : TradeRuleBase
    {
        private readonly decimal _dropRate;
        private readonly TimeSpan _dropPeriod;
        private readonly List<Ticker> _tickers = new List<Ticker>();

        public BuyAfterDropTradeRule(BitstampTrader bitstampTrader, TradeSettings tradeSettings, decimal dropRate, TimeSpan dropPeriod, params ITradeHolder[] tradeHolders)
            : base(bitstampTrader, tradeSettings, tradeHolders)
        {
            _dropRate = dropRate;
            _dropPeriod = dropPeriod;
        }

        internal override async Task ExecuteAsync()
        {
            // get ticker
            var ticker = await BitstampTrader.GetTickerAsync(TradeSession.PairCode);
            _tickers.Add(ticker);

            if (ExecuteTradeHolders()) return;

            if (!PriceDropped()) return;

            // get pair info
            var pairInfo = CacheHelper.GetFromCache<List<TradingPairInfo>>("TradingPairInfo").First(i => i.PairCode == TradeSession.PairCode.ToLower());

            // get the pair code id from cache
            var pairCodeId = CacheHelper.GetFromCache<List<CurrencyPair>>("TradingPairsDb").First(c => c.PairCode == TradeSession.PairCode.ToString()).Id;

            // buy currency on Bitstamp exchange
            var orderResult = await BitstampTrader.BuyLimitOrderAsync(TradeSession, TradeSettings.GetBuyBaseAmount(ticker, pairInfo), TradeSettings.GetBuyBasePrice(ticker, pairInfo));

            // update database
            var ordersRepo = new SqlRepository<Order>(new AppDbContext());
            ordersRepo.Add(new Order
            {
                BuyId = orderResult.Id,
                CurrencyPairId = pairCodeId,
                BuyAmount = orderResult.Amount,
                BuyPrice = orderResult.Price,
                SellAmount = TradeSettings.GetSellBaseAmount(ticker, pairInfo),
                SellPrice = TradeSettings.GetSellBasePrice(ticker, pairInfo)
            });
            ordersRepo.Save();
        }

        private bool PriceDropped()
        {
            // are there enough tickers?
            if (_tickers.OrderBy(t => t.Timestamp).First().Timestamp > DateTime.Now.Add(-_dropPeriod))
            {
                return false;
            }

            // get the tickers average
            _tickers.RemoveAll(t => t.Timestamp < DateTime.Now.Add(-_dropPeriod));
            var tickerAverage = _tickers.Average(t => t.Last);

            // did the price drop?
            var lastTicker = _tickers.OrderByDescending(t => t.Timestamp).First();
            return lastTicker.Last < tickerAverage * (1 - _dropRate/100);
        }
    }
}
