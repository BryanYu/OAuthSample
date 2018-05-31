using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OAuthSample.Service.Interfaces;

namespace OAuthSample.Service.Implements
{
    public class UserService : IUserService
    {
        public bool IsValid()
        {
            return true;
        }
    }
}