using System.Configuration;

namespace ToDoManager.Model
{
    public static class AppConfig
    {
        public static string TasksDbConnectionString =>
            ConfigurationManager.ConnectionStrings["TasksDbConnectionString"].ConnectionString;
    }
}