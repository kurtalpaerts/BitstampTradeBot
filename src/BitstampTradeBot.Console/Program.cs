using System;
using System.Globalization;
using BitstampTradeBot.Exchange.Models;
using BitstampTradeBot.Trader;
using BitstampTradeBot.Trader.Data.Helpers;
using BitstampTradeBot.Trader.Helpers;
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
                var tradeRule = new BuyPeriodicTradeRule(new TradeSettings { PairCode = BitstampPairCode.BtcEur, BuyUnderPriceMargin = 1, CounterAmount = 2, BaseAmountSavingsRate = 3, SellPriceRate = 15 },
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

        private static void SellLimitOrderPlaced(object sender, BitstampOrderEventArgs e)
        {
            System.Console.WriteLine($"Sell e placed for {e.Order.Amount} {e.Order.PairCode.BaseCodeUpper()} @{e.Order.Price} {e.Order.PairCode.CounterCodeUpper()} ({e.Order.Amount * e.Order.Price} {e.Order.PairCode.CounterCodeUpper()})");
        }

        private static void BuyLimitOrderExecuted(object sender, BitstampOrderEventArgs order)
        {
            System.Console.WriteLine($"Buy e executed for {order.Order.Amount} {order.Order.PairCode.ToString().Substring(0,3).ToUpper()} @{order.Order.Price} {order.Order.PairCode.ToString().Substring(3,3).ToUpper()}");
        }

        private static void BuyLimitOrderPlaced(object sender, BitstampOrderEventArgs e)
        {
            System.Console.WriteLine($"Buy e placed for {e.Order.Amount} {e.Order.PairCode.BaseCodeUpper()} @{e.Order.Price} {e.Order.PairCode.CounterCodeUpper()} ({e.Order.Amount * e.Order.Price} {e.Order.PairCode.CounterCodeUpper()})");
        }

        private static void TickerRetrieved(object sender, BitstampTickerEventArgs e)
        {
            System.Console.WriteLine($"{e.PairCode} : {e.Ticker.Last.ToString("N8", new NumberFormatInfo { CurrencyDecimalDigits = 8 }) }  ");
        }

        private static void ErrorOccured(object sender, Exception e)
        {
            System.Console.WriteLine($"ERROR: {e}");
        }
    }
}
