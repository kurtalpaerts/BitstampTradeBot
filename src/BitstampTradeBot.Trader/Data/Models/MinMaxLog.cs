using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BitstampTradeBot.Trader.Data.Models
{
    public class MinMaxLog
    {
        [Key]
        [Column(Order = 0)]
        public DateTime Day { get; set; }

        [Key]
        [Column(Order = 1)]
        public long CurrencyPairId { get; set; }

        public decimal Minimum { get; set; }

        public decimal Maximum { get; set; }

        public virtual CurrencyPair CurrencyPair { get; set; }
    }
}
