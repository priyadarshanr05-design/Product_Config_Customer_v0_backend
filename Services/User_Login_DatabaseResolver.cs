using Microsoft.EntityFrameworkCore;
using Product_Config_Customer_v0.Data;

public class User_Login_DatabaseResolver : IUser_Login_DatabaseResolver
{
    private readonly Dictionary<string, string> _domainToConnection;

    public User_Login_DatabaseResolver(IConfiguration config)
    {
        _domainToConnection = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "VisualAllies", config.GetConnectionString("CustDb_VisualAllies") },
            { "Jvl",          config.GetConnectionString("CustDb_Jvl") },
            { "CubicM3",      config.GetConnectionString("CustDb_CubicM3") },
            { "Dynaflow",     config.GetConnectionString("CustDb_Dynaflow") },
            { "Motor",        config.GetConnectionString("CustDb_Motor") }
        };
    }

    public bool TryGetConnectionString(string domain, out string connection)
    {
        connection = null!; 

        if (string.IsNullOrWhiteSpace(domain))
            return false; 

        return _domainToConnection.TryGetValue(domain, out connection!);
    }

}
