using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Owin;
using Microsoft.Owin.Security.Infrastructure;
using Microsoft.Owin.Security.OAuth;
using Owin;

namespace OAuthSample.AuthServer
{
    public partial class Startup
    {
        public void ConfigureAuth(IAppBuilder app)
        {
            app.UseOAuthAuthorizationServer(
                new OAuthAuthorizationServerOptions()
                {
                    AuthorizeEndpointPath = new PathString("/Authorize"),
                    TokenEndpointPath = new PathString("/Token"),
                    ApplicationCanDisplayErrors = true,
                    AllowInsecureHttp = true,
                    Provider = new OAuthAuthorizationServerProvider()
                    {
                        OnValidateClientRedirectUri = OnValidateClientRedirectUri,
                        OnValidateClientAuthentication = OnValidateClientAuthentication,
                        OnGrantResourceOwnerCredentials = OnGrantResourceOwnerCredentials,
                        OnGrantClientCredentials = OnGrantClientCredentials,
                    },
                    AuthorizationCodeProvider = new AuthenticationTokenProvider
                    {
                        OnCreate = CreateAuthenticationCode,
                        OnReceive = ReceiveAuthenticationCode
                    },
                    RefreshTokenProvider = new AuthenticationTokenProvider
                    {
                        OnCreate = CreateRefreshToken,
                        OnReceive = ReceiveRefreshToken
                    }
                });
        }

        public Task OnValidateClientRedirectUri(OAuthValidateClientRedirectUriContext context)
        {
            return null;
        }

        public Task OnValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            return null;
        }

        public Task OnGrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            return null;
        }

        public Task OnGrantClientCredentials(OAuthGrantClientCredentialsContext context)
        {
            return null;
        }

        public void CreateAuthenticationCode(AuthenticationTokenCreateContext context)
        {
        }

        public void ReceiveAuthenticationCode(AuthenticationTokenReceiveContext context)
        {
        }

        public void CreateRefreshToken(AuthenticationTokenCreateContext context)
        {
        }

        public void ReceiveRefreshToken(AuthenticationTokenReceiveContext context)
        {
        }
    }
}