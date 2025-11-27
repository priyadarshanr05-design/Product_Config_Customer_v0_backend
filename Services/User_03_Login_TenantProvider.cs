using Microsoft.AspNetCore.Http;

public class User_03_Login_TenantProvider : IUser_03_Login_TenantProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private const string TenantKeyItem = "TenantKey";

    public User_03_Login_TenantProvider(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string TenantKey =>
        _httpContextAccessor.HttpContext?.Items[TenantKeyItem]?.ToString();

    public void SetTenant(string tenantKey)
    {
        if (_httpContextAccessor.HttpContext != null)
            _httpContextAccessor.HttpContext.Items[TenantKeyItem] = tenantKey;
    }
}
