using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Owin.Security.OAuth;

namespace OAuthSample.AuthServer.Provider
{
    public class CustomerOAuthAuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        //OnValidateClientRedirectUri = OnValidateClientRedirectUri,
        //OnValidateClientAuthentication = OnValidateClientAuthentication,
        //OnGrantResourceOwnerCredentials = OnGrantResourceOwnerCredentials,
        //OnGrantClientCredentials = OnGrantClientCredentials,
    }
}