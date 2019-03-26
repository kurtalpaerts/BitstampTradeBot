using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BitstampTradeBot.Models;
using BitstampTradeBot.Trader.Data.Helpers;
using BitstampTradeBot.Trader.Data.Models;
using BitstampTradeBot.Trader.Models;
using BitstampTradeBot.Trader.TradeHolders;

namespace BitstampTradeBot.Trader.TradeRules
{
    public class LinearSpreadTradeRule : TradeRuleBase
    {
        private decimal _spreadPct;

        public LinearSpreadTradeRule(BitstampTrader bitstampTrader, TradeSettings tradeSettings, decimal spreadPct, params ITradeHolder[] tradeHolders) : base(bitstampTrader, tradeSettings, tradeHolders)
        {
            _spreadPct = spreadPct;
        }

        internal override async Task ExecuteAsync()
        {
            // get ticker
            var ticker = await BitstampTrader.GetTickerAsync(TradeSession.PairCode);

            if (await SpreadIsOpen(ticker))
            {
                // get pair info
                var pairInfo = CacheHelper.GetFromCache<List<TradingPairInfo>>("TradingPairInfo").First(i => i.PairCode == TradeSession.PairCode.ToLower());

                // buy currency on Bitstamp exchange
                var orderResult = await BitstampTrader.BuyLimitOrderAsync(TradeSession, TradeSettings.GetBuyBaseAmount(ticker, pairInfo), TradeSettings.GetBuyBasePrice(ticker, pairInfo));

                // update database
                BitstampTrader.AddNewOrderToDb(orderResult, ticker, TradeSettings, pairInfo);
            }
        }

        private async Task<bool> SpreadIsOpen(Ticker ticker)
        {
            // get pair info
            var pairInfo = CacheHelper.GetFromCache<List<TradingPairInfo>>("TradingPairInfo").First(i => i.PairCode == TradeSession.PairCode.ToLower());

            // get open orders from database
            var openOrdersDb = BitstampTrader.GetOpenOrdersDb().FindAll(o => o.CurrencyPairId == BitstampTrader.GetCurrencyPairId(pairInfo.PairCode));

            var tickerMax = ticker.Last * (1 + _spreadPct / 100);
            var tickerMin = ticker.Last * (1 - _spreadPct / 100);

            var openOrdersInRange = openOrdersDb.FindAll(o => o.BuyPrice > tickerMin && o.BuyPrice < tickerMax);

            if (openOrdersInRange.Count == 0)
            {
                return true;
            }

            return false;
        }
    }
}
