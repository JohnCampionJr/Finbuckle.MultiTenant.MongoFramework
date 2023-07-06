using MongoFramework;
using MongoFramework.AspNetCore.Identity;

namespace AuthenticationSample.Data
{
    public class MongoTenantIdentityUser : MongoIdentityUser, IHaveTenantId
    {
        public string TenantId { get; set; }
    }
}
