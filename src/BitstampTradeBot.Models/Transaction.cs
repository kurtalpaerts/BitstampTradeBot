using System;

namespace BitstampTradeBot.Models
{
    public class Transaction
    {
        public long Id { get; set; }
        public DateTime Timestamp { get; set; }
        public decimal Price { get; set; }
    }
}
