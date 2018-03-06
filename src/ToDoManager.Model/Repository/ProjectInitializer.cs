using System;
using System.Data.Entity;
using ToDoManager.Model.Migrations;

namespace ToDoManager.Model.Repository
{
    internal class ProjectInitializer : MigrateDatabaseToLatestVersion<ToDoManagerContext, Configuration>
    {
        public override void InitializeDatabase(ToDoManagerContext context)
        {
            context.Database.Log = Console.Write;
            base.InitializeDatabase(context);
        }
    }
}