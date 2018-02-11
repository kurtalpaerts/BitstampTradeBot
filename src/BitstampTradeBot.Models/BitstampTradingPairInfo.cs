using Newtonsoft.Json;

namespace BitstampTradeBot.Models
{
    public class BitstampTradingPairInfo
    {
        // Trading pair
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        // URL symbol of trading pair
        [JsonProperty(PropertyName = "url_symbol")]
        public string UrlSymbol { get; set; }

        // Decimal precision for base currency (BTC/USD - base: BTC)
        [JsonProperty(PropertyName = "base_decimals")]
        public int BaseDecimals { get; set; }

        // Decimal precision for counter currency (BTC/USD - counter: USD)
        [JsonProperty(PropertyName = "counter_decimals")]
        public int CounterDecimals { get; set; }

        // Minimum order size
        [JsonProperty(PropertyName = "minimum_order")]
        public string MinimumOrder { get; set; }

        // Trading engine status (Enabled/Disabled)
        [JsonProperty(PropertyName = "trading")]
        public string TradingEngineStatus { get; set; }

        // Trading pair description
        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }
    }
}
