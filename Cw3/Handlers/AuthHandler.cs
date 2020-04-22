using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Cw3.Handlers
{
    public class AuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {

        public AuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
        {
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
           if(!Request.Headers.ContainsKey("Authorization"))
               return AuthenticateResult.Fail("Missing authorization header");

           var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
           var credentialBytes = Convert.FromBase64String(authHeader.Parameter);

           var credentials = Encoding.UTF8.GetString(credentialBytes).Split(":");

           if(credentials.Length != 2)
               return AuthenticateResult.Fail("Incorrect authorization Header");

           var claims = new[]
           {
               new Claim(ClaimTypes.NameIdentifier, "1"),
               new Claim(ClaimTypes.Name, "jan123"),
               new Claim(ClaimTypes.Role, "admin"),
               new Claim(ClaimTypes.Role, "student")

           };

           var identity = new ClaimsIdentity(claims, Scheme.Name);

           var principal = new ClaimsPrincipal(identity);
           var ticket = new AuthenticationTicket(principal,Scheme.Name);

           return AuthenticateResult.Success(ticket);
        }
    }
}
