using BitstampTradeBot.Trader.Data.Models;
using BitstampTradeBot.Trader.Data.Repositories;
using Ninject.Modules;

namespace BitstampTradeBot.Trader.Helpers
{
    public class NinjectBindings : NinjectModule
    {
        public override void Load()
        {
            Bind<IRepository<MinMaxLog>>().To<SqlRepository<MinMaxLog>>().WithConstructorArgument("context", new AppDbContext());
            Bind<IRepository<Order>>().To<SqlRepository<Order>>().WithConstructorArgument("context", new AppDbContext());
            Bind<IRepository<CurrencyPair>>().To<SqlRepository<CurrencyPair>>().WithConstructorArgument("context", new AppDbContext());
        }
    }
}
