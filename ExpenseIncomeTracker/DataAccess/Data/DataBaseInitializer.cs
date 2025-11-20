using DbUp;

namespace DataAccess.Data;

public class DataBaseInitializer
{
    private readonly string _connectionString;

    public DataBaseInitializer(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task InitializeAsync()
    {
        EnsureDatabase.For.PostgresqlDatabase(_connectionString);

        var upgrader = DeployChanges.To.PostgresqlDatabase(_connectionString).
            WithScriptsEmbeddedInAssembly(typeof(DataBaseInitializer).Assembly).
            LogToConsole().
            Build();
        
        if (upgrader.IsUpgradeRequired())
        {
            var result = upgrader.PerformUpgrade();
        }
    }
   
}