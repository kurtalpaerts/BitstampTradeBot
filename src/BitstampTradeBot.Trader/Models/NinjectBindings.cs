using BitstampTradeBot.Exchange;
using BitstampTradeBot.Trader.Data.Models;
using BitstampTradeBot.Trader.Data.Repositories;
using Ninject.Modules;

namespace BitstampTradeBot.Trader.Models
{
    public class NinjectBindings : NinjectModule
    {
        public override void Load()
        {
            Bind<IRepository<MinMaxLog>>().To<SqlRepository<MinMaxLog>>().WithConstructorArgument("context", new AppDbContext());
            Bind<IRepository<Order>>().To<SqlRepository<Order>>().WithConstructorArgument("context", new AppDbContext());
            Bind<IRepository<CurrencyPair>>().To<SqlRepository<CurrencyPair>>().WithConstructorArgument("context", new AppDbContext());
            Bind<IExchange>().To<BitstampExchange>()
                .WithConstructorArgument("apiKey", ApiKeys.BitstampApiKey)
                .WithConstructorArgument("apiSecret", ApiKeys.BitstampApiSecret)
                .WithConstructorArgument("customerId", ApiKeys.BitstampCustomerId);

            //Bind<IRepository<MinMaxLog>>().To<MockRepository<MinMaxLog>>();
            //Bind<IRepository<Order>>().To<MockRepository<Order>>();
            //Bind<IRepository<CurrencyPair>>().To<MockRepository<CurrencyPair>>();
            //Bind<IExchange>().To<MockExchange>();
        }
    }
}
