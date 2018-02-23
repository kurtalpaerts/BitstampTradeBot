using System;
using System.Collections.Generic;

namespace BitstampTradeBot.Exchange.Helpers
{
    internal class ApiCallCounter
    {
        private readonly List<DateTime> _apiCalls = new List<DateTime>();

        internal void AddCall()
        {
            _apiCalls.Add(DateTime.Now);
        }

        internal void CheckIfMaximumReached()
        {
            // Bitstamp has a limit of 600 requests per 10 minutes

            _apiCalls.RemoveAll(c => c < DateTime.Now.AddMinutes(-10));

            if (_apiCalls.Count > 600) throw new Exception("Maximum number of api calls reached");
        }
    }
}
