using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BitstampTradeBot.Data.Models
{
    public sealed class CurrencyPair
    {
        [Key]
        public long Id { get; set; }
        public string PairCode { get; set; }

        public ICollection<Order> Orders { get; set; }
    }
}
