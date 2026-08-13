using System.Data;
using Microsoft.Data.SqlClient;

namespace JobManagementApi.Data;

public class DapperContext(IConfiguration configuration)
{
    public IDbConnection GetConnection()
    {
        string? connectionString=configuration.GetConnectionString("DefaultConnection");
        return new SqlConnection(connectionString);
    }
}