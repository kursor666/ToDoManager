namespace ToDoManager.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TaskGroupEntities",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.TaskEntities",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        CreatedUtc = c.DateTime(),
                        CompletedUtc = c.DateTime(),
                        Note = c.String(),
                        GroupId = c.Guid(),
                        Name = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TaskGroupEntities", t => t.GroupId)
                .Index(t => t.GroupId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TaskEntities", "GroupId", "dbo.TaskGroupEntities");
            DropIndex("dbo.TaskEntities", new[] { "GroupId" });
            DropTable("dbo.TaskEntities");
            DropTable("dbo.TaskGroupEntities");
        }
    }
}
