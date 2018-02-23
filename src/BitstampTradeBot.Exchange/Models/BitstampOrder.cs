using System;
using BitstampTradeBot.Models;
using Newtonsoft.Json;

namespace BitstampTradeBot.Exchange.Models
{
    internal class BitstampOrder
    {
        // transaction ID
        [JsonProperty(PropertyName = "id")]
        public long Id { get; set; }

        // Currency Pair
        [JsonProperty(PropertyName = "currency_pair")]
        public string CurrencyPair { get; set; }

        // type: 0 - buy; 1 - sell
        [JsonProperty(PropertyName = "type")]
        public BitstampOrderType Type { get; set; }

        // date and time
        [JsonProperty(PropertyName = "datetime")]
        public DateTime Timestamp { get; set; }

        // price
        [JsonProperty(PropertyName = "price")]
        public decimal Price { get; set; }

        // amount
        [JsonProperty(PropertyName = "amount")]
        public decimal Amount { get; set; }
    }
}
