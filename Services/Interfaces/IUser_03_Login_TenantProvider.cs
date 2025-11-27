public interface IUser_03_Login_TenantProvider
{
    string TenantKey { get; }
    void SetTenant(string tenantKey);
}