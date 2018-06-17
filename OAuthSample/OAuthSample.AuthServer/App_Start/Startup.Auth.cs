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
using Microsoft.Owin.Security;
using OAuthSample.Service;

namespace OAuthSample.AuthServer
{
    public partial class Startup
    {
        private readonly ClientService _clientService = new ClientService();

        public void ConfigureAuth(IAppBuilder app)
        {
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
                        OnReceive = ReceiveAuthenticationCode,
                        OnCreate = CreateAuthenticationCode,
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
            // Authorize Code Grnat Step1

            // 驗證Client Application預先註冊的ClientId，如果驗證通過就使用預先註冊的RedirectUrI

            var clientInfo = _clientService.GetClient(context.ClientId);
            if (clientInfo != null)
            {
                context.Validated(clientInfo.RedirectUrI);
            }

            return Task.FromResult(0);
        }

        public Task OnValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            // 驗證Client預先註冊的資訊(ClientId,Secret)
            // Client透過 Authorization Header 填入 Basic Base64(Client:Secret)
            // 在這邊使用context.TryGetBasicCredentials就會解出Base64中的資訊

            if (context.TryGetBasicCredentials(out string clientId, out string clientSecret))
            {
                var clientInfo = this._clientService.GetClient(clientId);
                if (clientInfo != null && clientInfo.ClientSecret == clientSecret)
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
            var authenticationCode = Guid.NewGuid().ToString("n") + Guid.NewGuid().ToString("n");
            context.SetToken(authenticationCode);
            this._clientService.AddAuthenicationCode(authenticationCode, context.SerializeTicket());
        }

        public void ReceiveAuthenticationCode(AuthenticationTokenReceiveContext context)
        {
            // 收到驗證碼時的處理，收到的code是存在context.Token變數中
            // 為了不讓語意造成混淆，另外寫新的變數存放
            var authenticationCode = context.Token;
            if (this._clientService.IsAuthenicationCodeExist(authenticationCode, out string token))
            {
                context.DeserializeTicket(token);
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