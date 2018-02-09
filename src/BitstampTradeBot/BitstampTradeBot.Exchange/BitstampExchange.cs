using System;
using System.Net.Http;
using System.Threading.Tasks;
using BitstampTradeBot.Exchange.Models;
using Newtonsoft.Json;

namespace BitstampTradeBot.Exchange
{
    public class BitstampExchange
    {
        public enum BitstampPairCode
        {
            BtcUsd, BtcEur, EurUsd, XrpUsd, XrpEur, XrpBtc, LtcUsd, LtcEur, LtcBtc, EthUsd, EthEur, EthBtc, BchUsd, BchEur, BchBtc
        }

        private const string ApiBaseUrl = "https://www.bitstamp.net/api/v2/";
        private BitstampTicker _ticker = new BitstampTicker();

        public async Task<BitstampTicker> GetTickerAsync(BitstampPairCode pairCode)
        {
            try
            {
                using (var client = new HttpClient())
                using (var response = await client.GetAsync(ApiBaseUrl + "ticker/" + pairCode.ToString().ToLower()))
                using (var content = response.Content)
                {
                    var result = await content.ReadAsStringAsync();
                    _ticker = JsonConvert.DeserializeObject<BitstampTicker>(result);

                    return _ticker;
                }
            }
            catch (Exception e)
            {
                throw new Exception("BitstampExchange.GetTickerAsync() : " + e.StackTrace);
            }
        }
    }
}
