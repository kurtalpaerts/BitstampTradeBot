using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BitstampTradeBot.Models;
using BitstampTradeBot.Models.Helpers;
using BitstampTradeBot.Trader.Models;
using BitstampTradeBot.Trader.Models.Exchange;
using Newtonsoft.Json;

namespace BitstampTradeBot.Trader
{
    public class BitstampExchange
    {
        public BitstampTicker Ticker;
        public BitstampAccountBalance AccountBalance;
        public List<BitstampOrder> OpenOrders;
        public List<BitstampTransaction> Transactions;


        #region  Api authentication

        private long _nonce = DateTime.Now.Ticks;

        private List<KeyValuePair<string, string>> GetAuthenticationPostData()
        {
            Interlocked.Increment(ref _nonce);

            return new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("key", ApiKeys.BitstampApiKey),
                new KeyValuePair<string, string>("signature", GetSignature(_nonce, ApiKeys.BitstampApiKey, ApiKeys.BitstampApiSecret, ApiKeys.BitstampCustomerId)),
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

        public async Task<BitstampTicker> GetTickerAsync(BitstampPairCode pairCode)
        {
            try
            {
                using (var client = new HttpClient())
                using (var response = await client.GetAsync(Settings.ApiBaseUrl + "ticker/" + pairCode.ToLower()))
                using (var content = response.Content)
                {
                    var result = await content.ReadAsStringAsync();
                    Ticker = JsonConvert.DeserializeObject<BitstampTicker>(result);
                    Ticker.PairCode = pairCode;

                    return Ticker;
                }
            }
            catch (Exception e)
            {
                throw new Exception("BitstampExchange.GetTickerAsync() : " + e);
            }
        }

        public async Task<List<BitstampTradingPairInfo>> GetPairsInfoAsync()
        {
            try
            {
                using (var client = new HttpClient())
                using (var response = await client.GetAsync(Settings.ApiBaseUrl + "trading-pairs-info/"))
                using (var content = response.Content)
                {
                    var result = await content.ReadAsStringAsync();
                    var pairsInfo = JsonConvert.DeserializeObject<List<BitstampTradingPairInfo>>(result);

                    return pairsInfo;
                }
            }
            catch (Exception e)
            {
                throw new Exception("BitstampExchange.GetPairsInfoAsync() : " + e);
            }
        }

        public async Task<BitstampAccountBalance> GetAccountBalanceAsync()
        {
            try
            {
                using (var client = new HttpClient())
                using (var response = await client.PostAsync(Settings.ApiBaseUrl + "balance/", new FormUrlEncodedContent(GetAuthenticationPostData())))
                using (var content = response.Content)
                {
                    var result = await content.ReadAsStringAsync();
                    AccountBalance = JsonConvert.DeserializeObject<BitstampAccountBalance>(result);

                    return AccountBalance;
                }
            }
            catch (Exception e)
            {
                throw new Exception("BitstampExchange.GetAccountBalanceAsync() : " + e);
            }
        }
        public async Task<List<BitstampOrder>> GetOpenOrdersAsync()
        {
            try
            {
                using (var client = new HttpClient())
                using (var response = await client.PostAsync(Settings.ApiBaseUrl + "open_orders/all/", new FormUrlEncodedContent(GetAuthenticationPostData())))
                using (var content = response.Content)
                {
                    var result = await content.ReadAsStringAsync();
                    OpenOrders = JsonConvert.DeserializeObject<List<BitstampOrder>>(result);

                    return OpenOrders;
                }
            }
            catch (Exception e)
            {
                throw new Exception("BitstampExchange.GetOpenOrdersAsync() : " + e);
            }
        }

        public async Task<List<BitstampTransaction>> GetTransactions()
        {
            try
            {
                using (var client = new HttpClient())
                using (var response = await client.PostAsync(Settings.ApiBaseUrl + "user_transactions/", new FormUrlEncodedContent(GetAuthenticationPostData())))
                using (var content = response.Content)
                {
                    var result = await content.ReadAsStringAsync();
                    Transactions = JsonConvert.DeserializeObject<List<BitstampTransaction>>(result);

                    return Transactions;
                }
            }
            catch (Exception e)
            {
                throw new Exception("BitstampExchange.GetTransactions() : " + e);
            }
        }

        public async Task<BitstampOrder> BuyLimitOrderAsync(BitstampPairCode pairCode, decimal amount, decimal price)
        {
            try
            {
                var postData = GetAuthenticationPostData();
                postData.Add(new KeyValuePair<string, string>("amount", amount.ToString(CultureInfo.InvariantCulture)));
                postData.Add(new KeyValuePair<string, string>("price", price.ToString(CultureInfo.InvariantCulture)));

                using (var client = new HttpClient())
                using (var response = await client.PostAsync(Settings.ApiBaseUrl + "buy/" + pairCode.ToLower() + "/", new FormUrlEncodedContent(postData)))
                using (var content = response.Content)
                {
                    var result = await content.ReadAsStringAsync();
                    var executedOrder = JsonConvert.DeserializeObject<BitstampOrder>(result);

                    if (executedOrder.Id == 0) throw new Exception("Executed buy order id == 0!");

                    executedOrder.PairCode = pairCode;

                    return executedOrder;
                }
            }
            catch (Exception e)
            {
                throw new Exception("BitstampExchange.BuyLimitOrderAsync() : " + e);
            }
        }

        public async Task<BitstampOrder> SellLimitOrderAsync(BitstampPairCode pairCode, decimal amount, decimal price)
        {
            try
            {
                var postData = GetAuthenticationPostData();
                postData.Add(new KeyValuePair<string, string>("amount", amount.ToString(CultureInfo.InvariantCulture)));
                postData.Add(new KeyValuePair<string, string>("price", price.ToString(CultureInfo.InvariantCulture)));

                using (var client = new HttpClient())
                using (var response = await client.PostAsync(Settings.ApiBaseUrl + "sell/" + pairCode.ToLower() + "/", new FormUrlEncodedContent(postData)))
                using (var content = response.Content)
                {
                    var result = await content.ReadAsStringAsync();
                    var executedOrder = JsonConvert.DeserializeObject<BitstampOrder>(result);

                    if (executedOrder.Id == 0) throw new Exception("Executed sell order id == 0!");

                    return executedOrder;
                }
            }
            catch (Exception e)
            {
                throw new Exception("BitstampExchange.SellLimitOrderAsync() : " + e);
            }
        }

        public async Task<BitstampOrder> CancelOrderAsync(string id)
        {
            try
            {
                var postData = GetAuthenticationPostData();
                postData.Add(new KeyValuePair<string, string>("id", id));

                using (var client = new HttpClient())
                using (var response = await client.PostAsync(Settings.ApiBaseUrl + "cancel_order/", new FormUrlEncodedContent(postData)))
                using (var content = response.Content)
                {
                    var result = await content.ReadAsStringAsync();
                    var canceledOrder = JsonConvert.DeserializeObject<BitstampOrder>(result);

                    return canceledOrder;
                }
            }
            catch (Exception e)
            {
                throw new Exception("BitstampExchange.SellLimitOrderAsync() : " + e);
            }
        }
    }
}
