using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Owin;

namespace OAuthSample.API
{
    public partial class Startup
    {
        public void ConfigureAuth(IAppBuilder app)
        {
            app.UseOAuthBearerAuthentication(new Microsoft.Owin.Security.OAuth.OAuthBearerAuthenticationOptions());
        }
    }
}