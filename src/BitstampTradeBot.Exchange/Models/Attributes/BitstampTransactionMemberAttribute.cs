using System;

namespace BitstampTradeBot.Exchange.Models.Attributes
{
    internal class BitstampTransactionMemberAttribute : Attribute
    {
        public readonly string MemberType;

        public BitstampTransactionMemberAttribute(string memberType)
        {
            MemberType = memberType;
        }
    }
}
