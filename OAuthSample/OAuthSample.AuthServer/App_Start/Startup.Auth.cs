using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Owin;
using Microsoft.Owin.Security.Infrastructure;
using Microsoft.Owin.Security.OAuth;
using Microsoft.Owin.Security.Cookies;
using Owin;
using System.Threading.Tasks;
using Autofac;
using Autofac.Integration.Owin;
using Microsoft.Owin.Security;
using OAuthSample.Service.Implements;
using OAuthSample.Service.Interfaces;

namespace OAuthSample.AuthServer
{
    public partial class Startup
    {
        private readonly ConcurrentDictionary<string, string> _authenticationCodes =
                    new ConcurrentDictionary<string, string>(StringComparer.Ordinal);

        private readonly ConcurrentDictionary<string, string> _client =
            new ConcurrentDictionary<string, string>(StringComparer.Ordinal);

        public void ConfigureAuth(IAppBuilder app)
        {
            this._client.TryAdd("OAuthSample.AuthorizationCodeGrantFlow", "http://localhost:13082/OAuth/Redirect");
            this._client.TryAdd("OAuthSample.API", "http://localhost:57653/swagger/ui/o2c-html");

            /*
             * 在Authorization Code Grant Flow與Implicit Grant Flow的流程中，
             * 要重導向讓使用者填入帳號密碼進行驗證，所以在這邊需要設定使用者登入的頁面路徑
             * LoginPath = new PathString("/Account/Login"),
             * LogoutPath = new PathString("/Account/Logout"),
             * 要對應至MVC的Controller 與 Action
             * View的部分(填帳號密碼的畫面)要自行實作，OWIN沒有提供
            */
            app.UseCookieAuthentication(
                new CookieAuthenticationOptions()
                {
                    AuthenticationType = "Application",
                    AuthenticationMode = AuthenticationMode.Passive,
                    LoginPath = new PathString("/Account/Login"),
                    LogoutPath = new PathString("/Account/Logout"),
                });
            /*
             * 在Authorization Code Grant Flow的流程中，
             * 會導向AuthorizeEndpointPath(也就是底下設定的/OAuth/Authorize)，
             * 判斷沒有權限會導回上面設定的LoginPath(也就是/Account/Login)進行帳號密碼登入
             * TokenEndpointPath是Implicit Grant Flow流程可以直接使用帳號密碼取得Token
             * Provider、AuthorizationCodeProvider、RefreshTokenProvider這三個屬性
             * 是提供OAuth四個驗證流程的相關事件
             */
            app.UseOAuthAuthorizationServer(
                new OAuthAuthorizationServerOptions()
                {
                    AuthorizeEndpointPath = new PathString("/OAuth/Authorize"),
                    TokenEndpointPath = new PathString("/OAuth/Token"),
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
            var scope = context.OwinContext.GetAutofacLifetimeScope();
            var providerService = scope.Resolve<IProviderService>();

            var message = providerService.GetSampleMessage();
            // 驗證Client Application預先註冊的回呼Url(用ClientId驗)
            if (this._client.ContainsKey(context.ClientId))
            {
                context.Validated(this._client[context.ClientId]);
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

            return Task.FromResult(0);
        }

        public Task OnGrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            return Task.FromResult(0); ;
        }

        public Task OnGrantClientCredentials(OAuthGrantClientCredentialsContext context)
        {
            return null;
        }

        public void CreateAuthenticationCode(AuthenticationTokenCreateContext context)
        {
            // 產出驗證碼
            context.SetToken(Guid.NewGuid().ToString("n") + Guid.NewGuid().ToString("n"));
            this._authenticationCodes[context.Token] = context.SerializeTicket();
        }

        public void ReceiveAuthenticationCode(AuthenticationTokenReceiveContext context)
        {
            // 收到驗證碼時的處理
            string value = string.Empty;
            if (this._authenticationCodes.TryRemove(context.Token, out value))
            {
                context.DeserializeTicket(value);
            }
        }

        public void CreateRefreshToken(AuthenticationTokenCreateContext context)
        {
        }

        public void ReceiveRefreshToken(AuthenticationTokenReceiveContext context)
        {
        }
    }
}