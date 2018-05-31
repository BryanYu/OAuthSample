using Microsoft.Owin.Security;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using OAuthSample.Service.Interfaces;

namespace OAuthSample.AuthServer.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserService _userService;

        public AccountController(IUserService userService)
        {
            this._userService = userService;
        }

        [HttpGet]
        public ActionResult Login()
        {
            var result = this._userService.IsValid();
            return this.View();
        }

        [HttpPost]
        public ActionResult Login(string userName,
                                  string password,
                                  bool? isPersistent = false)
        {
            var authentication = HttpContext.GetOwinContext().Authentication;

            /*
             * 在這邊實作使用者驗證機制，驗證通過後才能產生Claim與ClaimsIdentity
             */

            var result = this._userService.IsValid();

            var authenticationProperties =
                new AuthenticationProperties() { IsPersistent = isPersistent.GetValueOrDefault() };
            var claim = new Claim(ClaimsIdentity.DefaultNameClaimType, userName);
            var claimsIdentity = new ClaimsIdentity(new[] { claim }, "Application");
            authentication.SignIn(authenticationProperties, claimsIdentity);
            return View();
        }

        public ActionResult Logout()
        {
            var result = this._userService.IsValid();
            return this.View();
        }
    }
}