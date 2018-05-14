using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using Microsoft.Owin.Security;

namespace OAuthSample.AuthServer.Controllers
{
    public class AccountController : Controller
    {
        public ActionResult Login()
        {
            return this.View();
        }

        [HttpPost]
        public ActionResult Login(string userName, string password, bool isPersistent = false)
        {
            var authentication = HttpContext.GetOwinContext().Authentication;
            var claims = new[] { new Claim(ClaimsIdentity.DefaultNameClaimType, userName) };
            var claimsIdentity = new ClaimsIdentity(claims, "Application");
            var properties = new AuthenticationProperties() { IsPersistent = isPersistent };
            authentication.SignIn(properties, claimsIdentity);
            return RedirectToAction("Authorize", "OAuth");
        }

        public ActionResult Logout()
        {
            return this.View();
        }
    }
}