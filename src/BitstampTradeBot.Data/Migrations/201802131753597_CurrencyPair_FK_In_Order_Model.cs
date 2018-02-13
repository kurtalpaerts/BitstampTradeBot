namespace BitstampTradeBot.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CurrencyPair_FK_In_Order_Model : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.Orders", name: "CurrencyPair_Id", newName: "CurrencyPairId");
            RenameIndex(table: "dbo.Orders", name: "IX_CurrencyPair_Id", newName: "IX_CurrencyPairId");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.Orders", name: "IX_CurrencyPairId", newName: "IX_CurrencyPair_Id");
            RenameColumn(table: "dbo.Orders", name: "CurrencyPairId", newName: "CurrencyPair_Id");
        }
    }
}
