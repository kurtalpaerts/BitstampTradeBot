using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using BitstampTradeBot.Exchange.Helpers;
using BitstampTradeBot.Exchange.Models;
using BitstampTradeBot.Models;
using Newtonsoft.Json;

namespace BitstampTradeBot.Exchange
{
    public class BitstampClient
    {
        private const string ApiBaseUrl = "https://www.bitstamp.net/api/v2/";

        private readonly string _apiKey;
        private readonly string _apiSecret;
        private readonly string _customerId;
        private readonly ApiCallCounter _apiCallCounter = new ApiCallCounter();

        public BitstampClient(string apiKey, string apiSecret, string customerId)
        {
            _apiKey = apiKey;
            _apiSecret = apiSecret;
            _customerId = customerId;

            InitializeMappings();
        }

        #region public methods

        public async Task<Ticker> GetTickerAsync(string pairCode)
        {
            var bitstampTicker = await ApiCallGet<BitstampTicker>("ticker/" + pairCode);
            return Mapper.Map<BitstampTicker, Ticker>(bitstampTicker);
        }

        public async Task<List<TradingPairInfo>> GetPairsInfoAsync()
        {
            var bitstampTradingPairInfos = await ApiCallGet<List<BitstampTradingPairInfo>>("trading-pairs-info");
            return Mapper.Map<List<BitstampTradingPairInfo>, List<TradingPairInfo>>(bitstampTradingPairInfos);
        }

        public async Task<AccountBalance> GetAccountBalanceAsync()
        {
            var bitstampAccountBalance = await ApiCallPost<BitstampAccountBalance>("balance");
            return Mapper.Map<BitstampAccountBalance, AccountBalance>(bitstampAccountBalance);
        }

        public async Task<List<ExchangeOrder>> GetOpenOrdersAsync()
        {
            var bitstampOrders = await ApiCallPost<List<BitstampOrder>>("open_orders/all");
            return Mapper.Map<List<BitstampOrder>, List<ExchangeOrder>>(bitstampOrders);
        }

        public async Task<List<ExchangeOrder>> GetOpenOrdersAsync(string pairCode)
        {
            var bitstampOrders = await ApiCallPost<List<BitstampOrder>>($"open_orders/{pairCode}");
            return Mapper.Map<List<BitstampOrder>, List<ExchangeOrder>>(bitstampOrders);
        }

        public async Task<List<Transaction>> GetTransactionsAsync()
        {
            var bitstampTransactions = await ApiCallPost<List<BitstampTransaction>>("user_transactions");
            return Mapper.Map<List<BitstampTransaction>, List<Transaction>>(bitstampTransactions);
        }

        public async Task<ExchangeOrder> CancelOrderAsync(string id)
        {
            var bitstampOrder = await ApiCallPost<BitstampOrder>("cancel_order", new KeyValuePair<string, string>("id", id));
            return Mapper.Map<BitstampOrder, ExchangeOrder>(bitstampOrder);
        }

        public async Task<ExchangeOrder> BuyLimitOrderAsync(string pairCode, decimal amount, decimal price)
        {
            var bitstampOrder = await ApiCallPost<BitstampOrder>("buy/" + pairCode,
                    new KeyValuePair<string, string>("amount", amount.ToString(CultureInfo.InvariantCulture)),
                    new KeyValuePair<string, string>("price", price.ToString(CultureInfo.InvariantCulture))
            );

            var order = Mapper.Map<BitstampOrder, ExchangeOrder>(bitstampOrder);
            order.PairCode = pairCode;

            return order;
        }

        public async Task<ExchangeOrder> SellLimitOrderAsync(string pairCode, decimal amount, decimal price)
        {
            var bitstampOrder = await ApiCallPost<BitstampOrder>("sell/" + pairCode,
                    new KeyValuePair<string, string>("amount", amount.ToString(CultureInfo.InvariantCulture)),
                    new KeyValuePair<string, string>("price", price.ToString(CultureInfo.InvariantCulture))
            );
            return Mapper.Map<BitstampOrder, ExchangeOrder>(bitstampOrder);
        }

        #endregion public methods

        #region private methods

        private async Task<T> ApiCallGet<T>(string endPoint)
        {
            _apiCallCounter.CheckIfMaximumReached();

            using (var client = new HttpClient())
            using (var response = await client.GetAsync($"{ApiBaseUrl}{endPoint}/"))
            using (var content = response.Content)
            {
                _apiCallCounter.AddCall();
                var result = await content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(result);
            }
        }

        private async Task<T> ApiCallPost<T>(string endPoint, params KeyValuePair<string, string>[] postData)
        {
            _apiCallCounter.CheckIfMaximumReached();

            var authPostData = GetAuthenticationPostData();
            if (postData != null)
            {
                authPostData.AddRange(postData);
            }

            using (var client = new HttpClient())
            using (var response = await client.PostAsync($"{ApiBaseUrl}{endPoint}/", new FormUrlEncodedContent(authPostData)))
            using (var content = response.Content)
            {
                _apiCallCounter.AddCall();
                var result = await content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(result);
            }
        }

        private static void InitializeMappings()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<BitstampTicker, Ticker>();
                cfg.CreateMap<BitstampAccountBalance, AccountBalance>();
                cfg.CreateMap<BitstampTradingPairInfo, TradingPairInfo>().ForMember(dest => dest.PairCode, opts => opts.MapFrom(src => src.UrlSymbol));
                cfg.CreateMap<BitstampOrder, ExchangeOrder>()
                        .ForMember(dest => dest.PairCode, opts => opts.MapFrom(src => src.CurrencyPair.Replace("/","").ToLower()));
                cfg.CreateMap<BitstampTransaction, Transaction>()
                        .ForMember(dest => dest.Price, opts => opts.MapFrom(src => src.Price))
                        .ForMember(dest => dest.Id, opts => opts.MapFrom(src => src.OrderId));
            });
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