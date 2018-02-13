namespace BitstampTradeBot.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Nullable_Columns_In_Order_Model : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Orders", "BuyTimestamp", c => c.DateTime());
            AlterColumn("dbo.Orders", "BuyFee", c => c.Decimal(precision: 8, scale: 2));
            AlterColumn("dbo.Orders", "SellAmount", c => c.Decimal(precision: 18, scale: 8));
            AlterColumn("dbo.Orders", "SellTimestamp", c => c.DateTime());
            AlterColumn("dbo.Orders", "SellPrice", c => c.Decimal(precision: 18, scale: 8));
            AlterColumn("dbo.Orders", "SellFee", c => c.Decimal(precision: 8, scale: 2));
            AlterColumn("dbo.Orders", "SellId", c => c.Long());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Orders", "SellId", c => c.Long(nullable: false));
            AlterColumn("dbo.Orders", "SellFee", c => c.Decimal(nullable: false, precision: 8, scale: 2));
            AlterColumn("dbo.Orders", "SellPrice", c => c.Decimal(nullable: false, precision: 18, scale: 8));
            AlterColumn("dbo.Orders", "SellTimestamp", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Orders", "SellAmount", c => c.Decimal(nullable: false, precision: 18, scale: 8));
            AlterColumn("dbo.Orders", "BuyFee", c => c.Decimal(nullable: false, precision: 8, scale: 2));
            AlterColumn("dbo.Orders", "BuyTimestamp", c => c.DateTime(nullable: false));
        }
    }
}
