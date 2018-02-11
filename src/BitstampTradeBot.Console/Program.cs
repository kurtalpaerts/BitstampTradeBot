using System;
using System.Globalization;
using BitstampTradeBot.Models;
using BitstampTradeBot.Trader;
using BitstampTradeBot.Trader.TradeRules;

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
                _trader = new BitstampTrader(5000, 5000,
                    new BuyPeriodicTradeRule(BitstampPairCode.BtcUsd, TimeSpan.FromSeconds(5))
                 );
                _trader.ErrorOccured += ErrorOccured;
                _trader.TickerRetrieved += TickerRetrieved;
                _trader.BuyLimitOrderPlaced += BuyLimitOrderPlaced;

                // start trader
                _trader.Start();

                System.Console.ReadLine();
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e);
            }
        }

        private static void BuyLimitOrderPlaced(object sender, BitstampOrder order)
        {
            System.Console.WriteLine($"Buy order placed for {order.Amount} {order.PairCode.ToString().Substring(0,3).ToUpper()} @{order.Price} {order.PairCode.ToString().Substring(3,3).ToUpper()} ({order.Amount * order.Price} {order.PairCode.ToString().Substring(3,3).ToUpper()})");
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
