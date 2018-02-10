using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BitstampTradeBot.Data.Models
{
    public class CurrencyPair
    {
        [Key]
        public long Id { get; set; }
        public string PairCode { get; set; }

        public virtual ICollection<Order> Orders { get; set; }
    }
}
