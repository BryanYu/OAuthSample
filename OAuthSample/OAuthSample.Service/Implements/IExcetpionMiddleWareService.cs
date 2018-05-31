using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OAuthSample.Service.Interfaces;

namespace OAuthSample.Service.Implements
{
    public class ExcetpionMiddleWareService : IExcetpionMiddleWareService
    {
        public string GetExceptionMessage(Exception ex)
        {
            return ex.Message;
        }
    }
}