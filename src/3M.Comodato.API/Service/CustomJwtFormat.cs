//using Microsoft.IdentityModel.Tokens;
//using Microsoft.Owin.Security;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
//using System.IdentityModel.Tokens.Jwt;
//using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Web;

namespace _3M.Comodato.API.Service
{
    public class CustomJwtFormat : ISecureDataFormat<AuthenticationTicket>
    {
        private readonly string _allowedAudience;
        private readonly string _issuer;
        private readonly byte[] _jwtTokenSignKey;

        public CustomJwtFormat(string allowedAudience, string issuer, byte[] jwtTokenSignKey)
        {
            _allowedAudience = allowedAudience;
            _issuer = issuer;
            _jwtTokenSignKey = jwtTokenSignKey;
        }

        public string Protect(AuthenticationTicket data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));

            var signingCredentials = new SigningCredentials
            (
                new SymmetricSecurityKey(_jwtTokenSignKey),
                "http://www.w3.org/2001/04/xmldsig-more#hmac-sha256",
                "http://www.w3.org/2001/04/xmlenc#sha256"
            );

            return new JwtSecurityTokenHandler().WriteToken(new JwtSecurityToken(
                _issuer,
                _allowedAudience,
                data.Identity.Claims,
                DateTime.UtcNow, DateTime.UtcNow.AddHours(4),
                signingCredentials
            ));

        }

        public AuthenticationTicket Unprotect(string protectedText)
        {
            throw new NotImplementedException();
        }
    }
}