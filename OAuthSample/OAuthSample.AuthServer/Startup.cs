using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using Microsoft.Owin;
using OAuthSample.AuthServer.MiddleWare;
using OAuthSample.Service.Implements;
using OAuthSample.Service.Interfaces;
using Owin;

[assembly: OwinStartup(typeof(OAuthSample.AuthServer.Startup))]

namespace OAuthSample.AuthServer
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var builder = new ContainerBuilder();

            // 註冊MVC要用的
            builder.RegisterType<UserService>().AsImplementedInterfaces().InstancePerRequest();
            builder.RegisterControllers(typeof(Global).Assembly).InstancePerRequest();

            // 註冊OWIN要用的
            // OAuth的Provider建立一次，但是在method使用服務，所以可以使用InstancePerRequest
            builder.RegisterType<ProviderService>().AsImplementedInterfaces().InstancePerRequest();

            // MiddleWare的註冊順序 先註冊MiddleWare要用的Service 在註冊MiddleWare
            builder.RegisterType<ExcetpionMiddleWareService>().AsImplementedInterfaces().InstancePerRequest();
            builder.RegisterType<ExceptionMiddleWare>().InstancePerRequest();

            var container = builder.Build();

            // 先設給MVC的DependencyResolver用
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));

            // 再設給OWIN用 (這邊順序不能錯，要先UseAutofacMiddleware在UseAutofacMvc)
            app.UseAutofacMiddleware(container);
            app.UseAutofacMvc();

            // 設定Auth的
            ConfigureAuth(app);
        }
    }
}