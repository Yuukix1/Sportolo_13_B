using MySqlConnector;

namespace SportoloEredmenyApi.Data;

/// <summary>
/// Egyszerű gyár, amely a konfigurációban megadott connection string alapján
/// hoz létre új MySqlConnection példányokat. A kapcsolatot a hívó nyitja/zárja.
/// </summary>
public class DbConnectionFactory
{
    private readonly string _connectionString;

    public DbConnectionFactory(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Hiányzik a 'DefaultConnection' connection string az appsettings.json-ból.");
    }

    public MySqlConnection CreateConnection() => new(_connectionString);
}
