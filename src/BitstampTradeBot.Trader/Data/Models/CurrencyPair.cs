using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BitstampTradeBot.Trader.Data.Models
{
    public sealed class CurrencyPair
    {
        [Key]
        public long Id { get; set; }
        public string PairCode { get; set; }

        //public ICollection<Order> Orders { get; set; }
        //public ICollection<CurrencyPair> MinMaxLogs { get; set; }
    }
}
