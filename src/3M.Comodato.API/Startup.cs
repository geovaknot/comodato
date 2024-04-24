using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web.Http;
using _3M.Comodato.API.Service;
using Microsoft.AspNet.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Owin;
using Microsoft.Owin.Security.Jwt;
using Microsoft.Owin.Security.OAuth;
using Owin;

[assembly: OwinStartup(typeof(_3M.Comodato.API.Startup))]

namespace _3M.Comodato.API
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=316888
            //ConfigureOAuth(app);
            ConfigureOAuth(app);

            HttpConfiguration config = new HttpConfiguration();

            WebApiConfig.Register(config);
            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
            config.EnableCors();
            app.UseWebApi(config);
        }
        public void ConfigureOAuth(IAppBuilder app)
        {
            var sMinutos = System.Configuration.ConfigurationManager.AppSettings["MinutosExpirarToken"];
            double dMinutos;
            if (!double.TryParse(sMinutos, out dMinutos))
                dMinutos = 60;

            OAuthAuthorizationServerOptions OAuthServerOptions = new OAuthAuthorizationServerOptions()
            {
                AllowInsecureHttp = true,
                TokenEndpointPath = new PathString("/api/UsuarioAPI/GetToken"),
                AccessTokenExpireTimeSpan = TimeSpan.FromMinutes(dMinutos),
                Provider = new SimpleAuthorizationServerProvider()
            };

            // Token Generation
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
                AuthenticationMode = Microsoft.Owin.Security.AuthenticationMode.Active, // Block requests
                AllowedAudiences = new[] { "Any" },
                TokenValidationParameters = new TokenValidationParameters
                {
                    IssuerSigningKey = new SymmetricSecurityKey(secret),
                    ValidAudience = "Any",
                    ValidIssuer = "local"
                }
            });



        }
    }
}
