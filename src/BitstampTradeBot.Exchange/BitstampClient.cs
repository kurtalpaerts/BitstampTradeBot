using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BitstampTradeBot.Exchange.Models;
using Newtonsoft.Json;

namespace BitstampTradeBot.Exchange
{
    public class BitstampClient
    {
        public const string ApiBaseUrl = "https://www.bitstamp.net/api/v2/";

        private readonly string _apiKey;
        private readonly string _apiSecret;
        private readonly string _customerId;
        
        public BitstampClient(string apiKey, string apiSecret, string customerId)
        {
            _apiKey = apiKey;
            _apiSecret = apiSecret;
            _customerId = customerId;
        }

        #region public methods

        public async Task<BitstampTicker> GetTickerAsync(string pairCode)
        {
            return await ApiCallGet<BitstampTicker>("ticker/" + pairCode.ToLower());
        }

        public async Task<List<BitstampTradingPairInfo>> GetPairsInfoAsync()
        {
            return await ApiCallGet<List<BitstampTradingPairInfo>>("trading-pairs-info");
        }

        public async Task<BitstampAccountBalance> GetAccountBalanceAsync()
        {
            return await ApiCallPost<BitstampAccountBalance>("balance");
        }

        public async Task<List<BitstampOrder>> GetOpenOrdersAsync()
        {
            return await ApiCallPost<List<BitstampOrder>>("open_orders/all");
        }

        public async Task<List<BitstampTransaction>> GetTransactions()
        {
            return await ApiCallPost<List<BitstampTransaction>>("user_transactions");
        }

        public async Task<BitstampOrder> CancelOrderAsync(string id)
        {
            return await ApiCallPost<BitstampOrder>("cancel_order",
                    new KeyValuePair<string, string>("id", id)
            );
        }

        public async Task<BitstampOrder> BuyLimitOrderAsync(string pairCode, decimal amount, decimal price)
        {
            return await ApiCallPost<BitstampOrder>("buy/" + pairCode.ToLower(),
                    new KeyValuePair<string, string>("amount", amount.ToString(CultureInfo.InvariantCulture)),
                    new KeyValuePair<string, string>("price", price.ToString(CultureInfo.InvariantCulture))
            );
        }

        public async Task<BitstampOrder> SellLimitOrderAsync(string pairCode, decimal amount, decimal price)
        {
            return await ApiCallPost<BitstampOrder>("sell/" + pairCode.ToLower(),
                    new KeyValuePair<string, string>("amount", amount.ToString(CultureInfo.InvariantCulture)),
                    new KeyValuePair<string, string>("price", price.ToString(CultureInfo.InvariantCulture))
            );
        }

        #endregion public methods

        #region private methods

        private static async Task<T> ApiCallGet<T>(string endPoint)
        {
            using (var client = new HttpClient())
            using (var response = await client.GetAsync($"{ApiBaseUrl}{endPoint}/"))
            using (var content = response.Content)
            {
                var result = await content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(result);
            }
        }

        private async Task<T> ApiCallPost<T>(string endPoint, params KeyValuePair<string, string>[] postData)
        {
            var authPostData = GetAuthenticationPostData();
            if (postData != null)
            {
                authPostData.AddRange(postData);
            }

            using (var client = new HttpClient())
            using (var response = await client.PostAsync($"{ApiBaseUrl}{endPoint}/", new FormUrlEncodedContent(authPostData)))
            using (var content = response.Content)
            {
                var result = await content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(result);
            }
        }

        #endregion private methods

        #region  Api authentication

        private long _nonce = DateTime.Now.Ticks;

        private List<KeyValuePair<string, string>> GetAuthenticationPostData()
        {
            Interlocked.Increment(ref _nonce);

            return new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("key", _apiKey),
                new KeyValuePair<string, string>("signature", GetSignature(_nonce, _apiKey, _apiSecret, _customerId)),
                new KeyValuePair<string, string>("nonce", _nonce.ToString())
            };
        }

        private string GetSignature(long nonce, string key, string secret, string customerId)
        {
            var msg = $"{nonce}{customerId}{key}";

            return ByteArrayToString(ComputeHash(secret, StringToByteArray(msg))).ToUpper();
        }

        private static string ByteArrayToString(byte[] hash)
        {
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }

        private static byte[] StringToByteArray(string str)
        {
            return Encoding.UTF8.GetBytes(str);
        }

        private static byte[] ComputeHash(string key, byte[] data)
        {
            var hashMaker = new HMACSHA256(Encoding.UTF8.GetBytes(key));
            return hashMaker.ComputeHash(data);
        }

        #endregion Api authentication
    }
}