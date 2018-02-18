using System;

namespace BitstampTradeBot.Trader.Models.Exchange
{
    public class BitstampTickerEventArgs : EventArgs
    {
        public BitstampTicker Ticker { get; set; }

        public BitstampTickerEventArgs(BitstampTicker ticker)
        {
            Ticker = ticker;
        }
    }
}
