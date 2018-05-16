using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using Microsoft.Owin;
using Microsoft.Owin.Security.Infrastructure;
using Microsoft.Owin.Security.OAuth;
using Microsoft.Owin.Security.Cookies;
using Owin;
using System.Threading.Tasks;
using Microsoft.Owin.Security;

namespace OAuthSample.AuthServer
{
    public partial class Startup
    {
        private readonly ConcurrentDictionary<string, string> _authenticationCodes =
            new ConcurrentDictionary<string, string>(StringComparer.Ordinal);

        public void ConfigureAuth(IAppBuilder app)
        {
            app.UseCookieAuthentication(
                new CookieAuthenticationOptions()
                {
                    AuthenticationType = "Application",
                    AuthenticationMode = AuthenticationMode.Passive,
                    LoginPath = new PathString("/Account/Login"),
                    LogoutPath = new PathString("/Account/Logout"),
                });

            app.UseOAuthAuthorizationServer(
                new OAuthAuthorizationServerOptions()
                {
                    AuthorizeEndpointPath = new PathString("/OAuth/Authorize"),
                    TokenEndpointPath = new PathString("/OAuth/Token"),
                    ApplicationCanDisplayErrors = true,
                    AllowInsecureHttp = true,
                    Provider = new OAuthAuthorizationServerProvider()
                    {
                        OnValidateClientRedirectUri = ValidateClientRedirectUri,
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

        public Task ValidateClientRedirectUri(OAuthValidateClientRedirectUriContext context)
        {
            // 驗證Client Application預先註冊的回呼Url(用ClientId驗)

            if (context.ClientId == Properties.Settings.Default.Client)
            {
                context.Validated(Properties.Settings.Default.RedirectUrl);
            }

            Debug.WriteLine("OnValidateClientRedirectUri");
            return Task.FromResult(0);
        }

        public Task OnValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            // 驗證Client預先註冊的資訊(ClientId,Secret)

            string clientId = string.Empty;
            string clientSecret = string.Empty;

            if (context.TryGetBasicCredentials(out clientId, out clientSecret)
                || context.TryGetFormCredentials(out clientId, out clientSecret))
            {
                if (clientId == Properties.Settings.Default.Client
                    && clientSecret == Properties.Settings.Default.Secret)
                {
                    context.Validated();
                }
            }

            Debug.WriteLine("OnValidateClientAuthentication");
            return Task.FromResult(0);
        }

        public Task OnGrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            Debug.WriteLine("OnGrantResourceOwnerCredentials");
            return null;
        }

        public Task OnGrantClientCredentials(OAuthGrantClientCredentialsContext context)
        {
            Debug.WriteLine("OnGrantClientCredentials");
            return null;
        }

        public void CreateAuthenticationCode(AuthenticationTokenCreateContext context)
        {
            // 產出驗證碼
            context.SetToken(Guid.NewGuid().ToString("n") + Guid.NewGuid().ToString("n"));
            this._authenticationCodes[context.Token] = context.SerializeTicket();

            Debug.WriteLine("CreateAuthenticationCode");
        }

        public void ReceiveAuthenticationCode(AuthenticationTokenReceiveContext context)
        {
            // 收到驗證碼時的處理
            string value = string.Empty;
            if (this._authenticationCodes.TryRemove(context.Token, out value))
            {
                context.DeserializeTicket(value);
            }

            Debug.WriteLine("ReceiveAuthenticationCode");
        }

        public void CreateRefreshToken(AuthenticationTokenCreateContext context)
        {
            Debug.WriteLine("CreateRefreshToken");
        }

        public void ReceiveRefreshToken(AuthenticationTokenReceiveContext context)
        {
            Debug.WriteLine("ReceiveRefreshToken");
        }
    }
}