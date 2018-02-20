using System;
using BitstampTradeBot.Models;

namespace BitstampTradeBot.Trader.Models.Exchange
{
    public class BitstampTickerEventArgs : EventArgs
    {
        public Ticker Ticker { get;  }
        public string PairCode { get; }

        public BitstampTickerEventArgs(Ticker ticker, string pairCode)
        {
            Ticker = ticker;
            PairCode = pairCode;
        }
    }
}
