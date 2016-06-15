// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Startup.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the Startup type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(OAuth.WebApi.Startup))]

namespace OAuth.WebApi
{
    using System.Web.Http;

    using Microsoft.Owin.Cors;
    using Microsoft.Owin.Security.OAuth;

    public class Startup
    {
        public static OAuthBearerAuthenticationOptions OAuthBearerOptions { get; private set; }
        public void Configuration(IAppBuilder app)
        {
            // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=316888
            var httpConfiguration = new HttpConfiguration();
            this.ConfigureOAuth(app);
            WebApiConfig.Register(httpConfiguration);
            app.UseCors(CorsOptions.AllowAll);

            app.UseWebApi(httpConfiguration);
            AuthRepo.Instance.AddDefaultClient();
        }

        private void ConfigureOAuth(IAppBuilder app)
        {
            OAuthBearerOptions = new OAuthBearerAuthenticationOptions();
            OAuthAuthorizationServerOptions OAuthServerOptions = new OAuthAuthorizationServerOptions()
            {

                AllowInsecureHttp = true,
                TokenEndpointPath = new PathString("/token"),
                AccessTokenExpireTimeSpan = TimeSpan.FromMinutes(5),
                Provider = new SimpleAuthorizationServerProvider(),
                RefreshTokenProvider = new SimpleRefreshTokenProvider()
            };

            // Token Generation
            app.UseOAuthAuthorizationServer(OAuthServerOptions);
            app.UseOAuthBearerAuthentication(OAuthBearerOptions);
        }
    }
}
