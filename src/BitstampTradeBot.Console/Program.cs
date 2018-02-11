using System;
using BitstampTradeBot.Data.Models;
using BitstampTradeBot.Data.Repositories;
using BitstampTradeBot.Exchange;
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
                _trader = new BitstampTrader(5000, 5000, 
                    new BuyPeriodicTradeRule(BitstampPairCode.BtcUsd, TimeSpan.FromSeconds(20)),
                    new BuyPeriodicTradeRule(BitstampPairCode.XrpUsd, TimeSpan.FromSeconds(20))
                 );


                _trader.ErrorOccured += ErrorOccured;
                _trader.Start();

                System.Console.ReadLine();
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e);
            }
        }

        private static void ErrorOccured(object sender, Exception e)
        {
            System.Console.WriteLine($"ERROR: {e}");
        }
    }
}
