using System;
using System.Collections.Generic;
using BitstampTradeBot.Models;

namespace BitstampTradeBot.Trader.Models
{
    public class TradeSession
    {
        public DateTime Timestamp { get; set; }
        public string PairCode { get; set; }
        public DateTime LastBuyTimestamp { get; set; }
        public List<ExchangeOrder> OpenOrders { get; set; }
    }
}
