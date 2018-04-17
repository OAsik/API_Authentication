using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
using Microsoft.Owin.Security.OAuth;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Security.Claims;
using Microsoft.Owin.Cors;

[assembly: OwinStartup(typeof(ServerSide.App_Start.Startup))]

namespace ServerSide.App_Start
{
    public class Startup:OAuthAuthorizationServerProvider
    {
        public void Configuration(IAppBuilder app)
        {
            HttpConfiguration httpConfig = new HttpConfiguration();
            ConfigureOAuth(app);
            WebApiConfig.Register(httpConfig);
            //app.UseCors(CorsOptions.AllowAll);
            app.UseWebApi(httpConfig);
        }

        private void ConfigureOAuth(IAppBuilder appBuilder)
        {
            OAuthAuthorizationServerOptions opts = new OAuthAuthorizationServerOptions()
            {
                TokenEndpointPath = new Microsoft.Owin.PathString("/token"),
                AccessTokenExpireTimeSpan = TimeSpan.FromMinutes(2),
                AllowInsecureHttp = true,
                Provider = new SimpleAuthorizationServerProvider()
            };

            //appBuilder.UseCors(CorsOptions.AllowAll);
            appBuilder.UseOAuthAuthorizationServer(opts);
            appBuilder.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());
        }        
    }
}
