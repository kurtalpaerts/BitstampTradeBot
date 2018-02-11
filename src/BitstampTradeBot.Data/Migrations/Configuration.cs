using System;
using BitstampTradeBot.Data.Helpers;
using BitstampTradeBot.Models;
using System.Data.Entity.Migrations;
using BitstampTradeBot.Data.Models;

namespace BitstampTradeBot.Data.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<BitstampTradeBot.Data.Models.AppDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(BitstampTradeBot.Data.Models.AppDbContext context)
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
