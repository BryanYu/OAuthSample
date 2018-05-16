using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace OAuthSample.AuthServer.Controllers
{
    public class OAuthController : Controller
    {
        [HttpGet]
        public ActionResult Authorize()
        {
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
            var authentication = HttpContext.GetOwinContext().Authentication;
            var ticket = authentication.AuthenticateAsync("Application").Result;
            var identity = ticket != null ? ticket.Identity : null;

            identity = new ClaimsIdentity(identity.Claims, "Bearer", identity.NameClaimType, identity.RoleClaimType);
            var scopesList = scope.Split(' ').ToList();
            foreach (var arg in scopesList)
            {
                identity.AddClaim(new Claim("urn:oauth:scope", arg));
            }

            authentication.SignIn(identity);
            return this.View();
        }

        [HttpPost]
        public ActionResult SignInDifferentUser()
        {
            var authentication = HttpContext.GetOwinContext().Authentication;
            authentication.SignOut("Application");
            authentication.Challenge("Application");
            return new HttpUnauthorizedResult();
        }
    }
}