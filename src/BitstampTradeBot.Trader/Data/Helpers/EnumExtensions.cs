using System;

namespace BitstampTradeBot.Trader.Data.Helpers
{
    public static class EnumExtensions
    {
        public static string BaseCodeUpper(this Enum value)
        {
            var val = Enum.GetName(value.GetType(), value);

            return val?.Substring(0, 3).ToUpper();
        }

        public static string CounterCodeUpper(this Enum value)
        {
            var val = Enum.GetName(value.GetType(), value);

            return val?.Substring(3, 3).ToUpper();
        }

        public static string ToLower(this Enum value)
        {
            var val = Enum.GetName(value.GetType(), value);

            return val?.ToLower();
        }
    
    }
}
