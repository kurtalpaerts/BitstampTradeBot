using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BitstampTradeBot.Trader.Data.Helpers;
using BitstampTradeBot.Trader.Data.Models;
using BitstampTradeBot.Trader.Data.Repositories;
using BitstampTradeBot.Trader.Helpers;
using BitstampTradeBot.Trader.Models;
using BitstampTradeBot.Trader.Models.Exchange;
using BitstampTradeBot.Trader.TradeHolders;

namespace BitstampTradeBot.Trader.TradeRules
{
    public class BuyPeriodicTradeRule : TradeRuleBase
    {
        private readonly BitstampPairCode _pairCode;

        public BuyPeriodicTradeRule(BitstampPairCode pairCode, TradeSettings tradeSettings, params ITradeHolder[] tradeHolders) : base(tradeSettings, tradeHolders)
        {
            _pairCode = pairCode;
        }

        internal override async Task ExecuteAsync(BitstampTrader bitstampTrader)
        {
            if (ExecuteTradeHolders()) return;

            // get ticker
            var ticker = await bitstampTrader.GetTickerAsync(_pairCode);

            // get pair info
            var pairInfo = CacheHelper.GetFromCache<List<BitstampTradingPairInfo>>("TradingPairInfo").First(i => i.UrlSymbol == ticker.PairCode.ToLower());

            // get the pair code id from cache
            var pairCodeId = CacheHelper.GetFromCache<List<CurrencyPair>>("TradingPairsDb").First(c => c.PairCode == _pairCode.ToString()).Id;

            // buy currency on Bitstamp exchange
            var orderResult = await bitstampTrader.BuyLimitOrderAsync(_pairCode, TradeSettings.GetBaseAmount(ticker, pairInfo), TradeSettings.GetBasePrice(ticker, pairInfo));

            // update database
            var ordersRepo = new SqlRepository<Order>(new AppDbContext());
            ordersRepo.Add(new Order
            {
                BuyId = orderResult.Id,
                CurrencyPairId = pairCodeId,
                BuyAmount = orderResult.Amount,
                BuyPrice = orderResult.Price
            });
            ordersRepo.Save();

            LastBuyTimestamp = DateTime.Now;
        }
    }
}
