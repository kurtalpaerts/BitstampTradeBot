﻿namespace BitstampTradeBot.Models
{
    public class ExchangeOrder
    {
        public long Id { get; set; }
        public string PairCode { get; set; }
        public decimal Amount { get; set; }
        public decimal Price { get; set; }
    }
}