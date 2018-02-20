using System;
using BitstampTradeBot.Exchange.Models;

namespace BitstampTradeBot.Trader.Models.Exchange
{
    public class BitstampOrderEventArgs : EventArgs
    {
        public BitstampOrder Order { get; set; }

        public BitstampOrderEventArgs(BitstampOrder order)
        {
            Order = order;
        }
    }
}
