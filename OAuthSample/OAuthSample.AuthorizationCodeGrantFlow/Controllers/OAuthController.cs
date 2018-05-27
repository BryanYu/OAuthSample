using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DotNetOpenAuth.OAuth2;
using RestSharp;

namespace OAuthSample.AuthorizationCodeGrantFlow.Controllers
{
    public class OAuthController : Controller
    {
        private readonly string _clientId = "OAuthSample.AuthorizationCodeGrantFlow";

        private readonly string _redirectUrl = "http://localhost:13082/OAuth/Redirect";

        public ActionResult Index()
        {
            return this.View();
        }

        [HttpGet]
        public ActionResult Redirect(string code,
                                     string scope)
        {
            ViewBag.Code = code;
            ViewBag.Scope = scope;
            return this.View("Index");
        }

        [HttpPost]
        public ActionResult Authorize()
        {
            var authorizationServerUri = new Uri("http://localhost:57585");
            var authorizationServer = new AuthorizationServerDescription
            {
                AuthorizationEndpoint = new Uri(authorizationServerUri, "/OAuth/Authorize"),
                TokenEndpoint = new Uri(authorizationServerUri, "/OAuth/Token")
            };
            var webServerClient = new WebServerClient(authorizationServer, this._clientId);
            var scope = new List<string>() { "GeneralUser", "Admin" };
            var client = webServerClient.PrepareRequestUserAuthorization(scope, new Uri("http://localhost:13082/OAuth/Redirect"));

            client.Send(HttpContext);
            Response.End();
            return this.View("Index");
        }
    }
}