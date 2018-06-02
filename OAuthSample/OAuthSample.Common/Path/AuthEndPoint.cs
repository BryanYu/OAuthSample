using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SqlServer.Server;

namespace OAuthSample.Common.Path
{
    public enum AuthType
    {
        AuthorizationCodeGrant,
    }

    public class AuthEndPoint
    {
        public const string Server = "http://localhost:57585";

        public const string LoginPath = "/Account/Login";

        public const string LogoutPath = "/Account/Logout";

        public const string AuthorizeEndpoint = "/OAuth/Authorize";

        public const string TokenEndpointPath = "/OAuth/Token";

        public static Dictionary<AuthType, Client> Clients =
            new Dictionary<AuthType, Client>()
            {
                {
                    AuthType.AuthorizationCodeGrant,
                    new Client
                    {
                        ClientId =
                            "OAuthSample.AuthorizationCodeGrantFlow",
                        RedirectUrl =
                            "http://localhost:13082/Home/Redirect",
                        Secret = "OAuthSample.API.Secret"
                    }
                }
            };
    }

    public class Client
    {
        public string ClientId { get; set; }

        public string RedirectUrl { get; set; }

        public string Secret { get; set; }
    }
}