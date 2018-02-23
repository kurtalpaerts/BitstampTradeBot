using System;
using System.Collections.Generic;
using BitstampTradeBot.Models;
using BitstampTradeBot.Trader.Models;
using BitstampTradeBot.Trader.TradeHolders;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BitstampTradeBot.Test
{
    [TestClass]
    public class TraderTests
    {
        [TestMethod]
        public void BaseAmountAndPrice()
        {
            // arrange  
            var tradeSettings = new TradeSettings { CounterAmount = 10, BuyUnderPriceMargin = 1, BaseAmountSavingsRate = 5, SellPriceRate = 10 };
            var ticker = new Ticker { Last = 9500 };
            var pairInfo = new TradingPairInfo { BaseDecimals = 8, CounterDecimals = 2 };

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

        [TestMethod]
        public void WaitPeriodAfterStartHolder()
        {
            // arrange
            var tradeSession1 = new TradeSession { Timestamp = DateTime.Now };
            var tradeSession2 = new TradeSession { Timestamp = DateTime.Now.AddSeconds(30) };
            var tradeSession3 = new TradeSession { Timestamp = DateTime.Now.AddSeconds(61) };
            var tradeHolder = new WaitPeriodAfterStartHolder(TimeSpan.FromMinutes(1));

            // act
            var result1 = tradeHolder.Execute(tradeSession1);
            var result2 = tradeHolder.Execute(tradeSession2);
            var result3 = tradeHolder.Execute(tradeSession3);

            // assert
            Assert.AreEqual(result1, true);
            Assert.AreEqual(result2, true);
            Assert.AreEqual(result3, false);
        }

        [TestMethod]
        public void WaitPeriodAfterBuyOrderHolder()
        {
            // arrange
            var tradeSession1 = new TradeSession { Timestamp = DateTime.Now, LastBuyTimestamp = DateTime.Now };
            var tradeSession2 = new TradeSession { Timestamp = DateTime.Now, LastBuyTimestamp = DateTime.Now.AddSeconds(-30) };
            var tradeSession3 = new TradeSession { Timestamp = DateTime.Now, LastBuyTimestamp = DateTime.Now.AddSeconds(-61) };
            var tradeHolder = new WaitPeriodAfterBuyOrderHolder(TimeSpan.FromMinutes(1));

            // act
            var result1 = tradeHolder.Execute(tradeSession1);
            var result2 = tradeHolder.Execute(tradeSession2);
            var result3 = tradeHolder.Execute(tradeSession3);

            // assert
            Assert.AreEqual(result1, true);
            Assert.AreEqual(result2, true);
            Assert.AreEqual(result3, false);
        }

        [TestMethod]
        public void MaxNumberOfBuyOrdersHolder()
        {
            // arrange
            var sessionWithZeroOrders = new TradeSession { OpenOrders = new List<ExchangeOrder> () };
            var sessionWithOneOrder = new TradeSession { OpenOrders = new List<ExchangeOrder> { new ExchangeOrder { Type = BitstampOrderType.Buy } } };
            var sessionWithTwoOrders = new TradeSession
            {
                OpenOrders = new List<ExchangeOrder> {
                    new ExchangeOrder{ Type = BitstampOrderType.Buy},
                    new ExchangeOrder{Type = BitstampOrderType.Buy}}
            };
            var tradeHolder = new MaxNumberOfBuyOrdersHolder(2);

            // act
            var resultWithZeroOrders = tradeHolder.Execute(sessionWithZeroOrders);
            var resultWithOneOrder = tradeHolder.Execute(sessionWithOneOrder);
            var resultWithTwoOrders = tradeHolder.Execute(sessionWithTwoOrders);

            // assert
            Assert.AreEqual(resultWithZeroOrders, false);
            Assert.AreEqual(resultWithOneOrder, false);
            Assert.AreEqual(resultWithTwoOrders, true);
        }
    }
}