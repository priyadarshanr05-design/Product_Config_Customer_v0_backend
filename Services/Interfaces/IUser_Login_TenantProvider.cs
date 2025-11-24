public interface IUser_Login_TenantProvider
{
    string TenantKey { get; }
    void SetTenant(string tenantKey);
}