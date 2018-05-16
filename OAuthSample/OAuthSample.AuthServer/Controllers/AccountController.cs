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
        [HttpGet]
        public ActionResult Login()
        {
            return this.View();
        }

        [HttpPost]
        public ActionResult Login(string userName,
                                  string password,
                                  bool? isPersistent = false)
        {
            var authentication = HttpContext.GetOwinContext().Authentication;

            var authenticationProperties =
                new AuthenticationProperties() { IsPersistent = isPersistent.GetValueOrDefault() };
            var claim = new Claim(ClaimsIdentity.DefaultNameClaimType, userName);
            var claimsIdentity = new ClaimsIdentity(new[] { claim }, "Application");
            authentication.SignIn(authenticationProperties, claimsIdentity);
            return View();
        }

        public ActionResult Logout()
        {
            return this.View();
        }
    }
}