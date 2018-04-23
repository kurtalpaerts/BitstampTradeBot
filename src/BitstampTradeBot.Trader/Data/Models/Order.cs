using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BitstampTradeBot.Trader.Data.Models
{
    public class Order
    {
        public long Id { get; set; }
        
        [Required]
        public long CurrencyPairId { get; set; }

        [Required]
        public decimal BuyAmount { get; set; }

        public DateTime? BuyTimestamp { get; set; }

        [Required]
        public decimal BuyPrice { get; set; }

        public decimal? BuyFee { get; set; }

        [Required]
        public long BuyId { get; set; }

        public decimal SellAmount { get; set; }

        public DateTime? SellTimestamp { get; set; }

        public decimal SellPrice { get; set; }

        public decimal? SellFee { get; set; }

        public long? SellId { get; set; }
        

        public virtual CurrencyPair CurrencyPair { get; set; }

    }
}
