using System.Web;
using System.Web.Mvc;

namespace OAuthSample.AuthorizationCodeGrantFlow
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
