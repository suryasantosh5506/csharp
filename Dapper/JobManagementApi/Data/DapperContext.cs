using System.Data;
using MySqlConnector;

namespace JobManagementApi.Data;

public class DapperContext(IConfiguration configuration)
{
    public IDbConnection GetConnection()
    {
        string? connectionString=configuration.GetConnectionString("DefaultConnection");
        return new MySqlConnection(connectionString);
    }
}