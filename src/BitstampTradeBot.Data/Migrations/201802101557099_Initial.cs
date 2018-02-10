namespace BitstampTradeBot.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MinMaxLogs",
                c => new
                    {
                        Day = c.DateTime(nullable: false),
                        CurrencyPairId = c.Long(nullable: false),
                        Minimum = c.Decimal(nullable: false, precision: 18, scale: 8),
                        Maximum = c.Decimal(nullable: false, precision: 18, scale: 8),
                    })
                .PrimaryKey(t => new { t.Day, t.CurrencyPairId })
                .ForeignKey("dbo.CurrencyPairs", t => t.CurrencyPairId)
                .Index(t => t.CurrencyPairId);
            
            CreateTable(
                "dbo.CurrencyPairs",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        PairCode = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Orders",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        BuyAmount = c.Decimal(nullable: false, precision: 18, scale: 8),
                        BuyTimestamp = c.DateTime(nullable: false),
                        BuyPrice = c.Decimal(nullable: false, precision: 18, scale: 8),
                        BuyFee = c.Decimal(nullable: false, precision: 8, scale: 2),
                        BuyId = c.Long(nullable: false),
                        SellAmount = c.Decimal(nullable: false, precision: 18, scale: 8),
                        SellTimestamp = c.DateTime(nullable: false),
                        SellPrice = c.Decimal(nullable: false, precision: 18, scale: 8),
                        SellFee = c.Decimal(nullable: false, precision: 8, scale: 2),
                        SellId = c.Long(nullable: false),
                        CurrencyPair_Id = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CurrencyPairs", t => t.CurrencyPair_Id)
                .Index(t => t.CurrencyPair_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MinMaxLogs", "CurrencyPairId", "dbo.CurrencyPairs");
            DropForeignKey("dbo.Orders", "CurrencyPair_Id", "dbo.CurrencyPairs");
            DropIndex("dbo.Orders", new[] { "CurrencyPair_Id" });
            DropIndex("dbo.MinMaxLogs", new[] { "CurrencyPairId" });
            DropTable("dbo.Orders");
            DropTable("dbo.CurrencyPairs");
            DropTable("dbo.MinMaxLogs");
        }
    }
}
