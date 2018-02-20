using System;
using BitstampTradeBot.Models;

namespace BitstampTradeBot.Trader.Models.Exchange
{
    public class BitstampOrderEventArgs : EventArgs
    {
        public ExchangeOrder Order { get; set; }

        public BitstampOrderEventArgs(ExchangeOrder order)
        {
            Order = order;
        }
    }
}
