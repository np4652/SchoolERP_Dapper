using System.Security.Claims;
using System;
using Microsoft.AspNetCore.Http;
using Entities.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Net.Http;

namespace WebAPI.Helpers
{
    public static class ClaimsExtension
    {
        public static T GetLoggedInUserId<T>(this ClaimsPrincipal principal)
        {
            var loggedInUserId = principal.FindFirstValue("id");
            if (typeof(T) == typeof(string))
            {
                return (T)Convert.ChangeType(loggedInUserId, typeof(T));
            }
            else if (typeof(T) == typeof(int) || typeof(T) == typeof(long))
            {
                return loggedInUserId != null ? (T)Convert.ChangeType(loggedInUserId, typeof(T)) : (T)Convert.ChangeType(0, typeof(T));
            }
            else
            {
                throw new Exception("Invalid type provided");
            }
        }

        public static string GetLoggedInUserName(this ClaimsPrincipal principal)
        {
            if (principal == null)
                throw new ArgumentNullException(nameof(principal));

            return principal.FindFirstValue("UserName");
        }
        public static string GetLoggedInUserEmail(this ClaimsPrincipal principal)
        {
            if (principal == null)
                throw new ArgumentNullException(nameof(principal));

            return principal.FindFirstValue("UserName");
        }

        public static string GetLoggedInUserRoles(this ClaimsPrincipal principal)
        {
            if (principal == null)
                throw new ArgumentNullException(nameof(principal));

            return principal.FindFirstValue(ClaimTypes.Role);

        }

        public static object GetLoggedInUserRolesList(this ClaimsPrincipal principal)
        {
            if (principal == null)
                throw new ArgumentNullException(nameof(principal));

            var roles = principal.FindAll(ClaimTypes.Role);
            return roles;
        }
        public static List<string> GetLoggedInBussinessTypes(this ClaimsPrincipal principal)
        {
            if (principal == null)
                throw new ArgumentNullException(nameof(principal));
            if (string.IsNullOrEmpty(principal.FindFirstValue("BussinessType")))
                return new List<string>();
            return principal.FindFirstValue("BussinessType").Split(",").ToList();
        }

        #region JWT

        private static IEnumerable<Claim> GetClaims(this IHttpContextAccessor httpContext, string secretkey)
        {
            var token = httpContext.HttpContext.Request.Headers["Authorization"];
            if(string.IsNullOrEmpty(token))
            {
                throw new ArgumentNullException(token);
            }
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secretkey);
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                //ClockSkew = TimeSpan.Zero // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            var claims = jwtToken.Claims;
            return claims;
        }

        public static T GetLoggedInUserId<T>(this IHttpContextAccessor httpContext,string secretkey)
        {
            var claims = GetClaims(httpContext,secretkey);
            var loggedInUserId = claims.First(x => x.Type.ToUpper() == "ID").Value;
            if (typeof(T) == typeof(string))
            {
                return (T)Convert.ChangeType(loggedInUserId, typeof(T));
            }
            else if (typeof(T) == typeof(int) || typeof(T) == typeof(long))
            {
                return loggedInUserId != null ? (T)Convert.ChangeType(loggedInUserId, typeof(T)) : (T)Convert.ChangeType(0, typeof(T));
            }
            else
            {
                throw new Exception("Invalid type provided");
            }
        }
        public static string GetLoggedInUserEmail(this IHttpContextAccessor httpContext, string secretkey)
        {
            var claims = GetClaims(httpContext, secretkey);
            if (claims == null)
                throw new ArgumentNullException(nameof(claims));
            return Convert.ToString(claims.First(x => x.Type.ToUpper() == "EMAIL").Value);
        }

        public static string GetLoggedInUserRole(this IHttpContextAccessor httpContext, string secretkey)
        {
            var claims = GetClaims(httpContext, secretkey);
            if (claims == null)
                throw new ArgumentNullException(nameof(claims));

            return Convert.ToString(claims.First(x => x.Type.ToUpper() == "ROLE").Value);

        }
        #endregion
    }
}
