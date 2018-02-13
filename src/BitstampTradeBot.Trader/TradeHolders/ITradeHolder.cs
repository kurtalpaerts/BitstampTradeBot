namespace BitstampTradeBot.Trader.TradeHolders
{
    public interface ITradeHolder
    {
        bool Execute(BitstampTrader bitstampTrader);
    }
}