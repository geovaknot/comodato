using Microsoft.Owin.Security.Jwt;
using Microsoft.Owin.Security;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Owin;
using Owin;
using System.Web.Http;
using System.Configuration;
using _3M.Comodato.API.Service;
using System;
using Microsoft.Owin.Security.OAuth;
using Microsoft.Owin.Security.DataHandler.Encoder;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.DependencyInjection;
using System.Web.Mvc;
using System.Linq;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security.Authorization.Infrastructure;
using Microsoft.Owin.Cors;

//[assembly: OwinStartupAttribute(typeof(_3M.Comodato.API.Startup_JWT))]

namespace _3M.Comodato.API
{
    public class Startup_JWT
    {
        public void Configuration(IAppBuilder app)
        {

            ConfigureOAuth(app);

            ConfigureOAuthTokenConsumption(app);
            
            //app.UseAuthorization();

            HttpConfiguration config = new HttpConfiguration();
            WebApiConfig.Register(config);
            app.UseCors(CorsOptions.AllowAll);
            app.UseWebApi(config);
        }

        private void ConfigureOAuth(IAppBuilder app)
        {
            var key = Encoding.ASCII.GetBytes(Settings.Secret);
            var secret = Encoding.UTF8.GetBytes("12341234123412341234");
            var jwtFormatter = new CustomJwtFormat("Any", "local", secret);

            // This part issues tokens
            OAuthAuthorizationServerOptions OAuthServerOptions = new OAuthAuthorizationServerOptions()
            {
                AllowInsecureHttp = true,
                TokenEndpointPath = new PathString("/token"),
                AccessTokenExpireTimeSpan = TimeSpan.FromHours(4),
                Provider = new SimpleAuthorizationServerProvider(),
                AccessTokenFormat = jwtFormatter,
                RefreshTokenFormat = jwtFormatter
            };

            app.UseOAuthAuthorizationServer(OAuthServerOptions);
            app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());
        }
        private void ConfigureOAuthTokenConsumption(IAppBuilder app)
        {

            var secret = Encoding.UTF8.GetBytes("12341234123412341234");

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            JwtSecurityTokenHandler.DefaultOutboundClaimTypeMap.Clear();

            // This part checks the tokens
            app.UseJwtBearerAuthentication(new JwtBearerAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ExternalBearer,
                AuthenticationMode = AuthenticationMode.Active, // Block requests
                AllowedAudiences = new[] { "Any" },
                TokenValidationParameters = new TokenValidationParameters
                {
                    IssuerSigningKey = new SymmetricSecurityKey(secret),
                    ValidAudience = "Any",
                    ValidIssuer = "local"
                }
            });



        }

        //public void ConfigureOAuth(IAppBuilder app)
        //{



        //    OAuthAuthorizationServerOptions OAuthServerOptions = new OAuthAuthorizationServerOptions()
        //    {
        //        //For Dev enviroment only (on production should be AllowInsecureHttp = false)
        //        AllowInsecureHttp = true,
        //        TokenEndpointPath = new PathString("/token"),
        //        AccessTokenExpireTimeSpan = TimeSpan.FromHours(24),
        //        Provider = new SimpleAuthorizationServerProvider(),

        //    };



        //    // OAuth 2.0 Bearer Access Token Generation
        //    // app.UseOAuthAuthorizationServer(OAuthServerOptions);
        //    ;app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());


        //    //app.UseOAuthBearerTokens(OAuthServerOptions);

        //}

        //public void ConfigureServices(IServiceCollection services)
        //{
        //    services.AddControllersAsServices(typeof(Startup).Assembly.GetExportedTypes()
        //       .Where(t => !t.IsAbstract && !t.IsGenericTypeDefinition)
        //       .Where(t => typeof(IController).IsAssignableFrom(t)
        //          || t.Name.EndsWith("Controller", StringComparison.OrdinalIgnoreCase)));

        //}

    }
}
