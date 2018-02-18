using System;
using System.Globalization;
using BitstampTradeBot.Trader;
using BitstampTradeBot.Trader.Data.Helpers;
using BitstampTradeBot.Trader.Data.Models;
using BitstampTradeBot.Trader.Helpers;
using BitstampTradeBot.Trader.Models;
using BitstampTradeBot.Trader.TradeRules;
using BitstampTradeBot.Trader.Models.Exchange;
using BitstampTradeBot.Trader.TradeHolders;

namespace BitstampTradeBot.Console
{
    static class Program
    {
        private static BitstampTrader _trader;

        static void Main()
        {
            try
            {
                // initialize trader
                _trader = new BitstampTrader(10000);
                var tradeRule = new BuyPeriodicTradeRule(new TradeSettings { PairCode = BitstampPairCode.BtcEur, BuyUnderPriceMargin = 1, CounterAmount = 10, BaseAmountSavingsRate = 3, SellPriceRate = 15 },
                                                                new WaitPeriodAfterStartHolder(TimeSpan.FromSeconds(0)),
                                                                new WaitPeriodAfterBuyOrderHolder(TimeSpan.FromSeconds(3600)));
                _trader.AddTradeRule(tradeRule);


                _trader.ErrorOccured += ErrorOccured;
                _trader.TickerRetrieved += TickerRetrieved;
                _trader.BuyLimitOrderPlaced += BuyLimitOrderPlaced;
                _trader.BuyLimitOrderExecuted += BuyLimitOrderExecuted;
                _trader.SellLimitOrderPlaced += SellLimitOrderPlaced;

                // start trader
                _trader.Start();

                System.Console.ReadLine();
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e);
            }
        }

        private static void SellLimitOrderPlaced(object sender, BitstampOrder order)
        {
            System.Console.WriteLine($"Sell order placed for {order.Amount} {order.PairCode.BaseCodeUpper()} @{order.Price} {order.PairCode.CounterCodeUpper()} ({order.Amount * order.Price} {order.PairCode.CounterCodeUpper()})");
        }

        private static void BuyLimitOrderExecuted(object sender, Order order)
        {
            System.Console.WriteLine($"Buy order executed for {order.BuyAmount} {order.CurrencyPair.PairCode.Substring(0,3).ToUpper()} @{order.BuyPrice} {order.CurrencyPair.PairCode.Substring(3,3).ToUpper()}");
        }

        private static void BuyLimitOrderPlaced(object sender, BitstampOrder order)
        {
            System.Console.WriteLine($"Buy order placed for {order.Amount} {order.PairCode.BaseCodeUpper()} @{order.Price} {order.PairCode.CounterCodeUpper()} ({order.Amount * order.Price} {order.PairCode.CounterCodeUpper()})");
        }

        private static void TickerRetrieved(object sender, BitstampTicker ticker)
        {
            System.Console.WriteLine($"{ticker.PairCode} : {ticker.Last.ToString("N8", new NumberFormatInfo { CurrencyDecimalDigits = 8 }) }  ");
        }

        private static void ErrorOccured(object sender, Exception e)
        {
            System.Console.WriteLine($"ERROR: {e}");
        }
    }
}
