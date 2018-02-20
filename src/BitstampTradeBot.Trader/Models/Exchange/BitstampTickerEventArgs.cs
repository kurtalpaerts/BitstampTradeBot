using System;
using BitstampTradeBot.Exchange.Models;

namespace BitstampTradeBot.Trader.Models.Exchange
{
    public class BitstampTickerEventArgs : EventArgs
    {
        public BitstampTicker Ticker { get;  }
        public BitstampPairCode PairCode { get; }

        public BitstampTickerEventArgs(BitstampTicker ticker, BitstampPairCode pairCode)
        {
            Ticker = ticker;
            PairCode = pairCode;
        }
    }
}
