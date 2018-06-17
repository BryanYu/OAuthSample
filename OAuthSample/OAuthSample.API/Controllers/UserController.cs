using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Security;

namespace OAuthSample.API.Controllers
{
    public class UserController : ApiController
    {
        [HttpGet]
        public string GetUsers()
        {
            var user = User.Identity;
            return "From OAuthSample.API";
        }
    }
}