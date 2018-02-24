using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BitstampTradeBot.Models;
using BitstampTradeBot.Trader.Data.Helpers;
using BitstampTradeBot.Trader.Models;
using BitstampTradeBot.Trader.TradeHolders;

namespace BitstampTradeBot.Trader.TradeRules
{
    public class BuyPeriodicTradeRule : TradeRuleBase
    {
        public BuyPeriodicTradeRule(BitstampTrader bitstampTrader, TradeSettings tradeSettings, params ITradeHolder[] tradeHolders) 
            : base(bitstampTrader, tradeSettings, tradeHolders)
        {
        }

        internal override async Task ExecuteAsync()
        {
            // get ticker
            var ticker = await BitstampTrader.GetTickerAsync(TradeSession.PairCode);

            if (ExecuteTradeHolders()) return;
            
            // get pair info
            var pairInfo = CacheHelper.GetFromCache<List<TradingPairInfo>>("TradingPairInfo").First(i => i.PairCode == TradeSession.PairCode.ToLower());
            
            // buy currency on Bitstamp exchange
            var orderResult = await BitstampTrader.BuyLimitOrderAsync(TradeSession, TradeSettings.GetBuyBaseAmount(ticker, pairInfo), TradeSettings.GetBuyBasePrice(ticker, pairInfo));
            
            // update database
            BitstampTrader.AddNewOrderToDb(orderResult, ticker, TradeSettings, pairInfo);
        }
    }
}