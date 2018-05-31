using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OAuthSample.Service.Interfaces
{
    public interface IExcetpionMiddleWareService
    {
        string GetExceptionMessage(Exception ex);
    }
}