using _3M.Comodato.Entity;
using Microsoft.AspNet.Identity;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Web;

namespace _3M.Comodato.API.Service
{
    public static class TokenService
    {
        public static string GenerateToken(UsuarioPerfilEntity user)
        {
            var secret = Encoding.UTF8.GetBytes("12341234123412341234");

            var signingCredentials = new SigningCredentials
            (
                new SymmetricSecurityKey(secret),
                "http://www.w3.org/2001/04/xmldsig-more#hmac-sha256",
                "http://www.w3.org/2001/04/xmlenc#sha256"
            );

            var identity = new ClaimsIdentity(DefaultAuthenticationTypes.ExternalBearer);
            identity.AddClaim(new Claim(ClaimTypes.PrimarySid, "1"));
            identity.AddClaim(new Claim(ClaimTypes.Name, user.usuario.cnmNome));

            return new JwtSecurityTokenHandler().WriteToken(new JwtSecurityToken(
                "local",
                "Any",
                identity.Claims,
                DateTime.UtcNow, DateTime.UtcNow.AddHours(4),
                signingCredentials
            ));

            //var tokenHandler = new JwtSecurityTokenHandler();
            //var key = Encoding.UTF8.GetBytes("12341234123412341234");
            //var issuer = "local";
            //var tokenDescriptor = new SecurityTokenDescriptor
            //{
            //    Subject = new ClaimsIdentity(new Claim[]
            //    {
            //        new Claim(ClaimTypes.PrimarySid, "1"),
            //        new Claim(ClaimTypes.Name, user.usuario.cnmNome.ToString())
            //    }),
            //    Issuer = issuer,
            //    Expires = DateTime.UtcNow.AddHours(4),
            //    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            //};
            //var token = tokenHandler.CreateToken(tokenDescriptor);
            //return tokenHandler.WriteToken(token);
        }
    }
}