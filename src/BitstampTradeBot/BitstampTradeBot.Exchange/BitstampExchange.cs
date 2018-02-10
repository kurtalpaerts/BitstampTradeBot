using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BitstampTradeBot.Exchange.Models;
using BitstampTradeBot.Exchange.Services;
using Newtonsoft.Json;

namespace BitstampTradeBot.Exchange
{
    public class BitstampExchange
    {
        public enum BitstampPairCode
        {
            BtcUsd, BtcEur, EurUsd, XrpUsd, XrpEur, XrpBtc, LtcUsd, LtcEur, LtcBtc, EthUsd, EthEur, EthBtc, BchUsd, BchEur, BchBtc
        }

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
                new KeyValuePair<string, string>("key", SettingsService.BitstampApiKey),
                new KeyValuePair<string, string>("signature", GetSignature(_nonce, SettingsService.BitstampApiKey, SettingsService.BitstampApiSecret, SettingsService.BitstampCustomerId)),
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
                using (var response = await client.GetAsync(SettingsService.ApiBaseUrl + "ticker/" + pairCode.ToString().ToLower()))
                using (var content = response.Content)
                {
                    var result = await content.ReadAsStringAsync();
                    Ticker = JsonConvert.DeserializeObject<BitstampTicker>(result);

                    return Ticker;
                }
            }
            catch (Exception e)
            {
                throw new Exception("BitstampExchange.GetTickerAsync() : " + e);
            }
        }

        public async Task<BitstampAccountBalance> GetAccountBalanceAsync()
        {
            try
            {
                using (var client = new HttpClient())
                using (var response = await client.PostAsync(SettingsService.ApiBaseUrl + "balance/", new FormUrlEncodedContent(GetAuthenticationPostData())))
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
                using (var response = await client.PostAsync(SettingsService.ApiBaseUrl + "open_orders/all/", new FormUrlEncodedContent(GetAuthenticationPostData())))
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
                using (var response = await client.PostAsync(SettingsService.ApiBaseUrl + "user_transactions/", new FormUrlEncodedContent(GetAuthenticationPostData())))
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
    }
}
