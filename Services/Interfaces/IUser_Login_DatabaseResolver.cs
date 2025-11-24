using Product_Config_Customer_v0.Data;

public interface IUser_Login_DatabaseResolver
{
    bool TryGetConnectionString(string domain, out string connection);
}
