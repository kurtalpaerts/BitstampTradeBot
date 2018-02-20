using System;
using System.Data.Entity.Migrations;
using BitstampTradeBot.Exchange.Models;
using BitstampTradeBot.Trader.Data.Helpers;
using BitstampTradeBot.Trader.Data.Models;

namespace BitstampTradeBot.Trader.Data.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<AppDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(AppDbContext context)
        {
            // loop through all currency pair codes and add to database
            var pairCodeEnums = Enum.GetValues(typeof(BitstampPairCode));
            foreach (var value in pairCodeEnums)
            {
                context.CurrencyPairs.AddIfNotExists(new CurrencyPair { PairCode = value.ToString() }, c => c.PairCode == value.ToString());
            }
        }
    }
}
