namespace BitstampTradeBot.Exchange.Helpers
{
    public class IdGenerator
    {
        private long _id;

        public long GetNextId()
        {
            return ++_id;
        }
    }
}
