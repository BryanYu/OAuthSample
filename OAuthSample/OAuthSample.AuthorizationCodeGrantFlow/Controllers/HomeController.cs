using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using OAuthSample.AuthorizationCodeGrantFlow.Properties;
using RestSharp;

namespace OAuthSample.AuthorizationCodeGrantFlow.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var state = Guid.NewGuid().ToString("n");
            var url = $"{Settings.Default.AuthServer + Settings.Default.AuthUri}?"
                      + $"response_type=code&client_id={Settings.Default.ClientId}" + $"&scope={Settings.Default.Roles}&state={state}"
                      + $"&redirect_uri={Url.Encode(Settings.Default.RedirectUri)}";
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
            var restClient = new RestClient(Settings.Default.AuthServer);
            var request = new RestRequest(Settings.Default.TokenUri, Method.POST);
            var bytes = Encoding.UTF8.GetBytes(Settings.Default.ClientId + ":" + Settings.Default.Secret);
            var clientAuth = Convert.ToBase64String(bytes);
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddHeader("Authorization", "Basic " + clientAuth);
            request.AddHeader("Accept", "application/json");
            request.AddParameter("grant_type", "authorization_code");
            request.AddParameter("code", code);
            request.AddParameter("redirect_uri", Settings.Default.RedirectUri);

            var response = restClient.Execute(request);
            ViewBag.TokenResponse = response.Content;

            return this.View("Index");
        }
    }
}