using Newtonsoft.Json;

namespace BitstampTradeBot.Trader.Models.Exchange
{
    public class BitstampAccountBalance
    {
        // USD balance
        [JsonProperty(PropertyName = "usd_balance")]
        public decimal BalanceUsd { get; set; }

        // BTC balance
        [JsonProperty(PropertyName = "btc_balance")]
        public decimal BalanceBtc { get; set; }

        // EUR balance
        [JsonProperty(PropertyName = "eur_balance")]
        public decimal BalanceEur { get; set; }

        // XRP balance
        [JsonProperty(PropertyName = "xrp_balance")]
        public decimal BalanceXrp { get; set; }

        // LTC balance
        [JsonProperty(PropertyName = "ltc_balance")]
        public decimal BalanceLtc { get; set; }

        // ETH balance
        [JsonProperty(PropertyName = "eth_balance")]
        public decimal BalanceEth { get; set; }

        // BCH balance
        [JsonProperty(PropertyName = "bch_balance")]
        public decimal BalanceBch { get; set; }

        // USD reserved
        [JsonProperty(PropertyName = "usd_reserved")]
        public decimal ReservedUsd { get; set; }

        // BTC reserved
        [JsonProperty(PropertyName = "btc_reserved")]
        public decimal ReservedBtc { get; set; }

        // EUR reserved
        [JsonProperty(PropertyName = "eur_reserved")]
        public decimal ReservedEur { get; set; }

        // XRP reserved
        [JsonProperty(PropertyName = "xrp_reserved")]
        public decimal ReservedXrp { get; set; }

        // LTC reserved
        [JsonProperty(PropertyName = "ltc_reserved")]
        public decimal ReservedLtc { get; set; }

        // ETH reserved
        [JsonProperty(PropertyName = "eth_reserved")]
        public decimal ReservedEth { get; set; }

        // BCH reserved
        [JsonProperty(PropertyName = "bch_reserved")]
        public decimal ReservedBch { get; set; }

        // USD available for trading
        [JsonProperty(PropertyName = "usd_available")]
        public decimal AvailableUsd { get; set; }

        // BTC available for trading
        [JsonProperty(PropertyName = "btc_available")]
        public decimal AvailableBtc { get; set; }

        // EUR available for trading
        [JsonProperty(PropertyName = "eur_available")]
        public decimal AvailableEur { get; set; }

        // XRP available for trading
        [JsonProperty(PropertyName = "xrp_available")]
        public decimal AvailableXrp { get; set; }

        // LTC available for trading
        [JsonProperty(PropertyName = "ltc_available")]
        public decimal AvailableLtc { get; set; }

        // ETH available for trading
        [JsonProperty(PropertyName = "eth_available")]
        public decimal AvailableEth { get; set; }

        // BCH available for trading
        [JsonProperty(PropertyName = "bch_available")]
        public decimal AvailableBch { get; set; }

        // Customer BTC/USD trading fee
        [JsonProperty(PropertyName = "btcusd_fee")]
        public decimal FeeBtcUsd { get; set; }

        // Customer BTC/EUR trading fee
        [JsonProperty(PropertyName = "btceur_fee")]
        public decimal FeeBtcEur { get; set; }

        // Customer EUR/USD trading fee
        [JsonProperty(PropertyName = "eurusd_fee")]
        public decimal FeeEurUsd { get; set; }

        // Customer XRP/USD trading fee
        [JsonProperty(PropertyName = "xrpusd_fee")]
        public decimal FeeXrpUsd { get; set; }

        // Customer XRP/EUR trading fee
        [JsonProperty(PropertyName = "xrpeur_fee")]
        public decimal FeeXrpEur { get; set; }

        // Customer XRP/BTC trading fee
        [JsonProperty(PropertyName = "xrpbtc_fee")]
        public decimal FeeXrpBtc { get; set; }

        // Customer LTC/USD trading fee
        [JsonProperty(PropertyName = "ltcusd_fee")]
        public decimal FeeLtcUsd { get; set; }

        // Customer LTC/EUR trading fee
        [JsonProperty(PropertyName = "ltceur_fee")]
        public decimal FeeLtcEur { get; set; }

        // Customer LTC/BTC trading fee
        [JsonProperty(PropertyName = "ltcbtc_fee")]
        public decimal FeeLtcBtc { get; set; }

        // Customer ETH/USD trading fee
        [JsonProperty(PropertyName = "ethusd_fee")]
        public decimal FeeEthUsd { get; set; }

        // Customer ETH/EUR trading fee
        [JsonProperty(PropertyName = "etheur_fee")]
        public decimal FeeEthEur { get; set; }

        // Customer ETH/BTC trading fee
        [JsonProperty(PropertyName = "ethbtc_fee")]
        public decimal FeeEthBtc { get; set; }

        // Customer BCH/USD trading fee
        [JsonProperty(PropertyName = "bchusd_fee")]
        public decimal FeeBchUsd { get; set; }

        // Customer BCH/EUR trading fee
        [JsonProperty(PropertyName = "bcheur_fee")]
        public decimal FeeBchEur { get; set; }

        // Customer BCH/BTC trading fee
        [JsonProperty(PropertyName = "bchbtc_fee")]
        public decimal FeeBchBtc { get; set; }

        // Customer trading fee
        [JsonProperty(PropertyName = "fee")]
        public decimal Fee { get; set; }
    }
}
