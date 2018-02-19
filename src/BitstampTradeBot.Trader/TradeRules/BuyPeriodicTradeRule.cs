using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BitstampTradeBot.Trader.Data.Helpers;
using BitstampTradeBot.Trader.Data.Models;
using BitstampTradeBot.Trader.Data.Repositories;
using BitstampTradeBot.Trader.Helpers;
using BitstampTradeBot.Trader.Models.Exchange;
using BitstampTradeBot.Trader.TradeHolders;

namespace BitstampTradeBot.Trader.TradeRules
{
    public class BuyPeriodicTradeRule : TradeRuleBase
    {
        public BuyPeriodicTradeRule(TradeSettings tradeSettings, params ITradeHolder[] tradeHolders) : base(tradeSettings, tradeHolders)
        {
        }

        internal override async Task ExecuteAsync(BitstampTrader bitstampTrader)
        {
            if (ExecuteTradeHolders()) return;
            
            // get ticker
            var ticker = await bitstampTrader.GetTickerAsync(TradeSettings.PairCode);

            // get pair info
            var pairInfo = CacheHelper.GetFromCache<List<BitstampTradingPairInfo>>("TradingPairInfo").First(i => i.UrlSymbol == TradeSettings.PairCode.ToLower());

            // get the pair code id from cache
            var pairCodeId = CacheHelper.GetFromCache<List<CurrencyPair>>("TradingPairsDb").First(c => c.PairCode == TradeSettings.PairCode.ToString()).Id;

            // buy currency on Bitstamp exchange
            var orderResult = await bitstampTrader.BuyLimitOrderAsync(TradeSettings.PairCode, TradeSettings.GetBuyBaseAmount(ticker, pairInfo), TradeSettings.GetBuyBasePrice(ticker, pairInfo));

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

            LastBuyTimestamp = DateTime.Now;
        }
    }
}
