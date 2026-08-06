using System.Data;
using MySqlConnector;

namespace EmployeeManagementApi.Data;

public class EmployeeContext(IConfiguration configuration)
{
    public IDbConnection GetConnection()
    {
        var connstring=configuration.GetConnectionString("DefaultConnection");
        return new MySqlConnection(connstring);
    }
}