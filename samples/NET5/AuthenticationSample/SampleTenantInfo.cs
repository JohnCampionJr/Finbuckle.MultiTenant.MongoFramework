using System.ComponentModel;
using Finbuckle.MultiTenant;
using MongoDB.Bson.Serialization.Attributes;

namespace AuthenticationSample
{
    //adds field for per tenant cookie login
    public class SampleTenantInfo : MongoTenantInfo
    {
        //this is not actually used by the 6.0.0 release of Finbuckle, prefering claims over cookies
        public string CookiePath { get; set; }

        public string CookieLoginPath { get; set; } = "/__tenant__/Identity/Account/Login";
        public string CookieLogoutPath { get; set; } = "/__tenant__/Identity/Account/Logout";
        public string CookieAccessDeniedPath { get; set; } = "/__tenant__/Identity/Account/AccessDenied";
    }
}
