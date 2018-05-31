using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using OAuthSample.Service.Interfaces;

namespace OAuthSample.AuthServer.Controllers
{
    public class OAuthController : Controller
    {
        private readonly IUserService _userService;

        public OAuthController(IUserService userService)
        {
            this._userService = userService;
        }

        [HttpGet]
        public ActionResult Authorize()
        {
            var result = this._userService.IsValid();
            if (Response.StatusCode != 200)
            {
                return this.View("AuthorizeError");
            }

            var authentication = HttpContext.GetOwinContext().Authentication;
            var ticket = authentication.AuthenticateAsync("Application").Result;
            var identity = ticket != null ? ticket.Identity : null;
            if (identity == null)
            {
                authentication.Challenge("Application");
                return new HttpUnauthorizedResult();
            }

            return this.View();
        }

        [HttpPost]
        public ActionResult Authorize(string scope)
        {
            /*
             * 這裡是授權，scope會由
             */

            var result = this._userService.IsValid();
            if (Response.StatusCode != 200)
            {
                return View("AuthorizeError");
            }

            var authentication = HttpContext.GetOwinContext().Authentication;
            var ticket = authentication.AuthenticateAsync("Application").Result;
            var identity = ticket != null ? ticket.Identity : null;
            if (identity == null)
            {
                authentication.Challenge("Application");
                return new HttpUnauthorizedResult();
            }

            var scopes = scope.Split(' ');

            if (Request.HttpMethod == "POST")
            {
                if (!string.IsNullOrEmpty(Request.Form.Get("submit.Grant")))
                {
                    identity = new ClaimsIdentity(identity.Claims, "Bearer", identity.NameClaimType, identity.RoleClaimType);
                    foreach (var arg in scopes)
                    {
                        identity.AddClaim(new Claim("urn:oauth:scope", arg));
                    }
                    authentication.SignIn(identity);
                }
                if (!string.IsNullOrEmpty(Request.Form.Get("submit.Login")))
                {
                    authentication.SignOut("Application");
                    authentication.Challenge("Application");
                    return new HttpUnauthorizedResult();
                }
            }

            return View();
        }
    }
}