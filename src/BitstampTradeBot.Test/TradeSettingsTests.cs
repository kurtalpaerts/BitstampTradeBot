using BitstampTradeBot.Trader.Helpers;
using BitstampTradeBot.Trader.Models.Exchange;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BitstampTradeBot.Test
{
    [TestClass]
    public class TradeSettingsTests
    {
        [TestMethod]
        public void BaseAmountAndPriceTestMethod()
        {
            // arrange  
            var tradeSettings = new TradeSettings { CounterAmount = 10, BuyUnderPriceMargin = 1 };
            var ticker = new BitstampTicker { Last = 9500 };
            var pairInfo = new BitstampTradingPairInfo { BaseDecimals = 8 };

            // act  
            var baseAmount = tradeSettings.GetBaseAmount(ticker, pairInfo);
            var basePrice = tradeSettings.GetBasePrice(ticker, pairInfo);

            // assert  
            Assert.AreEqual(baseAmount, 0.00106326M);
            Assert.AreEqual(basePrice, 9405M);
        }
    }
}
