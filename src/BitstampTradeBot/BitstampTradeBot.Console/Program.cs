using System;
using BitstampTradeBot.Trader;

namespace BitstampTradeBot.Console
{
    static class Program
    {
        private static BitstampTrader _trader;

        static void Main()
        {
            try
            {
                _trader = new BitstampTrader(5000, 5000);
                _trader.ErrorOccured += ErrorOccured;

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
