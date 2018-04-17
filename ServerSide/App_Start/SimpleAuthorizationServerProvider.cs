using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Threading.Tasks;
using ServerSide.Models;

namespace ServerSide.App_Start
{
    public class SimpleAuthorizationServerProvider:OAuthAuthorizationServerProvider
    {
        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated();
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });            
            uPayEntities db = new uPayEntities();
            User appUser = db.Users.FirstOrDefault(x => x.UserName == context.UserName && x.Password == context.Password);

            if (context.UserName == appUser.UserName && context.Password == appUser.Password)
            {
                var identity = new ClaimsIdentity(context.Options.AuthenticationType);
                identity.AddClaim(new Claim("name", context.UserName));
                identity.AddClaim(new Claim("role", "user"));
                context.Validated(identity);
            }
            else
            {
                context.SetError("invalid_grant", "Username or password is invalid");
            }
        }

        public override async Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            if (context.TokenIssued)
            {
                var accessExpiration = DateTimeOffset.Now.AddSeconds(60);
                context.Properties.ExpiresUtc = accessExpiration;
            }
        }
    }
}