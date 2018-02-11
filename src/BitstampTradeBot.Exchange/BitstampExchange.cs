using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BitstampTradeBot.Data.Models;
using BitstampTradeBot.Data.Repositories;
using BitstampTradeBot.Exchange.Models;
using BitstampTradeBot.Exchange.Services;
using Newtonsoft.Json;

namespace BitstampTradeBot.Exchange
{
    public class BitstampExchange
    {
        private readonly IRepository<MinMaxLog> _minMaxLogRepository;
        private readonly IRepository<Order> _orderRepository;
        private readonly IRepository<CurrencyPair> _currencyPair;
        public BitstampTicker Ticker;
        public BitstampAccountBalance AccountBalance;
        public List<BitstampOrder> OpenOrders;
        public List<BitstampTransaction> Transactions;


        public BitstampExchange(IRepository<MinMaxLog> minMaxLogRepository, IRepository<Order> orderRepository, IRepository<CurrencyPair> currencyPair)
        {
            _minMaxLogRepository = minMaxLogRepository;
            _orderRepository = orderRepository;
            _currencyPair = currencyPair;
        }

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
                using (var response = await client.GetAsync(SettingsService.ApiBaseUrl + "ticker/" + pairCode.ToString().ToLower()))
                using (var content = response.Content)
                {
                    var result = await content.ReadAsStringAsync();
                    Ticker = JsonConvert.DeserializeObject<BitstampTicker>(result);

                    UpdateMinMaxLog(pairCode, Ticker);

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

        public async Task<BitstampOrder> BuyLimitOrderAsync(BitstampPairCode pairCode, decimal amount, decimal price)
        {
            try
            {
                // prepare post data
                var postData = GetAuthenticationPostData();
                postData.Add(new KeyValuePair<string, string>("amount", amount.ToString(CultureInfo.InvariantCulture)));
                postData.Add(new KeyValuePair<string, string>("price", price.ToString(CultureInfo.InvariantCulture)));

                using (var client = new HttpClient())
                using (var response = await client.PostAsync(SettingsService.ApiBaseUrl + "buy/" + pairCode.ToString().ToLower() + "/", new FormUrlEncodedContent(postData)))
                using (var content = response.Content)
                {
                    var result = await content.ReadAsStringAsync();
                    var executedOrder = JsonConvert.DeserializeObject<BitstampOrder>(result);

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
                // prepare post data
                var postData = GetAuthenticationPostData();
                postData.Add(new KeyValuePair<string, string>("amount", amount.ToString(CultureInfo.InvariantCulture)));
                postData.Add(new KeyValuePair<string, string>("price", price.ToString(CultureInfo.InvariantCulture)));

                using (var client = new HttpClient())
                using (var response = await client.PostAsync(SettingsService.ApiBaseUrl + "sell/" + pairCode.ToString().ToLower() + "/", new FormUrlEncodedContent(postData)))
                using (var content = response.Content)
                {
                    var result = await content.ReadAsStringAsync();
                    var executedOrder = JsonConvert.DeserializeObject<BitstampOrder>(result);

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
                // prepare post data
                var postData = GetAuthenticationPostData();
                postData.Add(new KeyValuePair<string, string>("id", id));

                using (var client = new HttpClient())
                using (var response = await client.PostAsync(SettingsService.ApiBaseUrl + "cancel_order/", new FormUrlEncodedContent(postData)))
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

        private void UpdateMinMaxLog(BitstampPairCode pairCode, BitstampTicker ticker)
        {
            var minMaxLogRepo = _minMaxLogRepository;
            var tickerCodeStr = pairCode.ToString();

            // get the record of the current day
            var currentDay = DateTime.Now.Date;
            var dateDb = minMaxLogRepo.ToList().FirstOrDefault(l => l.Day == currentDay && l.CurrencyPair.PairCode == tickerCodeStr);

            // if the day record do not exist then add, otherwise update the min and max values if necessary
            if (dateDb == null)
            {
                var currencyPairId = _currencyPair.ToList().First(p => p.PairCode == pairCode.ToString());

                minMaxLogRepo.Add(new MinMaxLog { Day = currentDay, CurrencyPairId = currencyPairId.Id, Minimum = ticker.Last, Maximum = ticker.Last });
            }
            else
            {
                if (dateDb.Minimum > ticker.Last) dateDb.Minimum = ticker.Last;
                if (dateDb.Maximum < ticker.Last) dateDb.Maximum = ticker.Last;
            }

            // save changes to database
            minMaxLogRepo.Save();
        }
    }
}
