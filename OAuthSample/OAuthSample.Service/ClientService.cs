using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OAuthSample.Service.Model;

namespace OAuthSample.Service
{
    public class ClientService
    {
        private static ConcurrentDictionary<string, ClientInfo>
            _client = new ConcurrentDictionary<string, ClientInfo>(StringComparer.Ordinal);

        private static ConcurrentDictionary<string, string> _authenicationCode =
            new ConcurrentDictionary<string, string>(StringComparer.Ordinal);

        public ClientService()
        {
            // AuthorizationCodeGrantFlow
            _client.TryAdd("OAuthSample.AuthorizationCodeGrantFlow", new ClientInfo()
            {
                AuthType = AuthType.AuthorizationCodeGrant,
                ClientId = "OAuthSample.AuthorizationCodeGrantFlow",
                ClientSecret = "OAuthSample.AuthorizationCodeGrantFlow.Secret",
                RedirectUrI = "http://localhost:13082/Home/Redirect"
            });
        }

        public ClientInfo GetClient(string clientId)
        {
            _client.TryGetValue(clientId, out ClientInfo clientInfo);
            return clientInfo;
        }

        public void AddAuthenicationCode(string code, string token)
        {
            _authenicationCode.TryAdd(code, token);
        }

        public bool IsAuthenicationCodeExist(string code, out string token)
        {
            return _authenicationCode.TryRemove(code, out token);
        }
    }
}