using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Autofac;
using Autofac.Integration.Owin;
using Microsoft.Owin;
using OAuthSample.Service.Interfaces;

namespace OAuthSample.AuthServer.MiddleWare
{
    public class ExceptionMiddleWare : OwinMiddleware
    {
        private IExcetpionMiddleWareService _exceptionMiddleWareService;

        public ExceptionMiddleWare(OwinMiddleware next,
                                   IExcetpionMiddleWareService exceptionMiddleWareService)
            : base(next)
        {
            this._exceptionMiddleWareService = exceptionMiddleWareService;
        }

        public override Task Invoke(IOwinContext context)
        {
            try
            {
                return Next.Invoke(context);
            }
            catch (Exception e)
            {
                this._exceptionMiddleWareService.GetExceptionMessage(e);
            }

            return Task.FromResult(0);
        }
    }
}