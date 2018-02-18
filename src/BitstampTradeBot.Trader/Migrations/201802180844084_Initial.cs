namespace BitstampTradeBot.Trader.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
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
                        CurrencyPairId = c.Long(nullable: false),
                        BuyAmount = c.Decimal(nullable: false, precision: 18, scale: 8),
                        BuyTimestamp = c.DateTime(),
                        BuyPrice = c.Decimal(nullable: false, precision: 18, scale: 8),
                        BuyFee = c.Decimal(precision: 8, scale: 2),
                        BuyId = c.Long(nullable: false),
                        SellAmount = c.Decimal(nullable: false, precision: 18, scale: 8),
                        SellTimestamp = c.DateTime(),
                        SellPrice = c.Decimal(nullable: false, precision: 18, scale: 8),
                        SellFee = c.Decimal(precision: 8, scale: 2),
                        SellId = c.Long(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CurrencyPairs", t => t.CurrencyPairId)
                .Index(t => t.CurrencyPairId);
            
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
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MinMaxLogs", "CurrencyPairId", "dbo.CurrencyPairs");
            DropForeignKey("dbo.Orders", "CurrencyPairId", "dbo.CurrencyPairs");
            DropIndex("dbo.MinMaxLogs", new[] { "CurrencyPairId" });
            DropIndex("dbo.Orders", new[] { "CurrencyPairId" });
            DropTable("dbo.MinMaxLogs");
            DropTable("dbo.Orders");
            DropTable("dbo.CurrencyPairs");
        }
    }
}
