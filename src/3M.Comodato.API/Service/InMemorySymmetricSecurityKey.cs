//using Microsoft.IdentityModel.Tokens;
//using Microsoft.Owin.Security;
namespace _3M.Comodato.API.Service
{
    internal class InMemorySymmetricSecurityKey
    {
        private byte[] _jwtTokenSignKey;

        public InMemorySymmetricSecurityKey(byte[] jwtTokenSignKey)
        {
            _jwtTokenSignKey = jwtTokenSignKey;
        }
    }
}