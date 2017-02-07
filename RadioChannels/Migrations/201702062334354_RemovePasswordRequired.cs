namespace RadioChannels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemovePasswordRequired : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.AspNetUsers", "PasswordHash", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.AspNetUsers", "PasswordHash", c => c.String(nullable: false));
        }
    }
}
