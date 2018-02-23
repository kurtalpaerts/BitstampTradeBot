using System;
using System.Globalization;
using BitstampTradeBot.Trader;
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
                _trader = new BitstampTrader(TimeSpan.FromSeconds(5));
                
                var tradeSettings = new TradeSettings
                {
                    PairCode = "btceur",
                    BuyUnderPriceMargin = 2,
                    CounterAmount = 10,
                    BaseAmountSavingsRate = 3,
                    SellPriceRate = 15
                };
                var tradeRule = new BuyPeriodicTradeRule(_trader,tradeSettings,
                        //new WaitPeriodAfterStartHolder(TimeSpan.FromSeconds(0)),
                        new WaitPeriodAfterBuyOrderHolder(TimeSpan.FromHours(1)),
                        new MaxNumberOfBuyOrdersHolder(1));

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
            System.Console.WriteLine($"Sell order placed for {e.Order.Amount} {e.Order.PairCode} @{e.Order.Price} {e.Order.PairCode} ({e.Order.Amount * e.Order.Price} {e.Order.PairCode})");
        }

        private static void BuyLimitOrderExecuted(object sender, BitstampOrderEventArgs order)
        {
            System.Console.WriteLine($"Buy order executed for {order.Order.Amount} {order.Order.PairCode.Substring(0, 3).ToUpper()} @{order.Order.Price} {order.Order.PairCode.Substring(3, 3).ToUpper()}");
        }

        private static void BuyLimitOrderPlaced(object sender, BitstampOrderEventArgs e)
        {
            System.Console.WriteLine($"Buy order placed for {e.Order.Amount} {e.Order.PairCode} @{e.Order.Price} {e.Order.PairCode} ({e.Order.Amount * e.Order.Price} {e.Order.PairCode})");
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
