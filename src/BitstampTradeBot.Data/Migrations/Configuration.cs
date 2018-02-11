using System;
using BitstampTradeBot.Data.Helpers;
using BitstampTradeBot.Data.Models;
using System.Data.Entity.Migrations;

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
            var pairCodeEnums = Enum.GetValues(typeof(BitstampPairCode));
            foreach (var value in pairCodeEnums)
            {
                context.CurrencyPairs.AddIfNotExists(new CurrencyPair { PairCode = value.ToString() }, c => c.PairCode == value.ToString());
            }
        }
    }
}
