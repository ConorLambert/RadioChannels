namespace RadioChannels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FavouriteChannelId : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Favourite", "ChannelId", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Favourite", "ChannelId");
        }
    }
}
