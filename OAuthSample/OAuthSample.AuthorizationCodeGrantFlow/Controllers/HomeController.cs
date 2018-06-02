using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OAuthSample.Common.Path;
using RestSharp;

namespace OAuthSample.AuthorizationCodeGrantFlow.Controllers
{
    public class HomeController : Controller
    {
        private readonly Client ClientInfo = AuthEndPoint.Clients[AuthType.AuthorizationCodeGrant];

        public ActionResult Index()
        {
            var url = $"{AuthEndPoint.Server + AuthEndPoint.AuthorizeEndpoint}?"
                      + $"response_type=code&client_id={ClientInfo.ClientId}&state=123456" + $"&scope=Admin,GeneralUser"
                      + $"&redirect_uri={Url.Encode(ClientInfo.RedirectUrl)}";

            ViewBag.AuthUrl = url;
            return this.View();
        }

        [HttpGet]
        public ActionResult Redirect(string code, string state)
        {
            ViewBag.Code = code;
            ViewBag.State = state;
            return this.View("Index");
        }

        [HttpPost]
        public ActionResult Token(string code)
        {
            var restClient = new RestClient(new Uri(AuthEndPoint.Server));
            var request = new RestRequest(AuthEndPoint.TokenEndpointPath, Method.POST);

            var bytes = Encoding.UTF8.GetBytes(this.ClientInfo.ClientId + ":" + this.ClientInfo.Secret);

            var clientAuth = Convert.ToBase64String(bytes);

            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddHeader("Authorization", "Basic " + clientAuth);
            request.AddHeader("Accept", "application/json");
            request.AddParameter("grant_type", "authorization_code");
            request.AddParameter("code", code);
            request.AddParameter("redirect_uri", Url.Encode(ClientInfo.RedirectUrl));
            request.AddParameter("client_id", this.ClientInfo.ClientId);

            var response = restClient.Execute(request);

            return this.View("Index");
        }
    }
}