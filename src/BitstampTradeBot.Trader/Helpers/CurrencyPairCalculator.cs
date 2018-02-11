using BitstampTradeBot.Models;

namespace BitstampTradeBot.Trader.Helpers
{
    class CurrencyPairCalculator
    {
        public static decimal AmountCredit(BitstampTicker ticker, decimal amountDebet)
        {
            return amountDebet / ticker.Last;
        }

        public static decimal AmountDebet(BitstampTicker ticker, decimal amountCredit)
        {
            return amountCredit * ticker.Last;
        }
    }
}
