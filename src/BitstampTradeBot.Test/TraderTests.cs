using BitstampTradeBot.Models;
using BitstampTradeBot.Trader.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BitstampTradeBot.Test
{
    [TestClass]
    public class TraderTests
    {
        [TestMethod]
        public void BaseAmountAndPriceTestMethod()
        {
            // arrange  
            var tradeSettings = new TradeSettings { CounterAmount = 10, BuyUnderPriceMargin = 1, BaseAmountSavingsRate = 5, SellPriceRate = 10};
            var ticker = new Ticker { Last = 9500 };
            var pairInfo = new TradingPairInfo { BaseDecimals = 8, CounterDecimals = 2};

            // act  
            var baseAmount = tradeSettings.GetBuyBaseAmount(ticker, pairInfo);
            var basePrice = tradeSettings.GetBuyBasePrice(ticker, pairInfo);
            var sellBaseAmount = tradeSettings.GetSellBaseAmount(ticker, pairInfo);
            var sellBasePrice = tradeSettings.GetSellBasePrice(ticker, pairInfo);

            // assert  
            Assert.AreEqual(baseAmount, 0.00106326M);
            Assert.AreEqual(basePrice, 9405M);
            Assert.AreEqual(sellBaseAmount, 0.001010097M);
            Assert.AreEqual(sellBasePrice, 10345.5M);
        }
    }
}
