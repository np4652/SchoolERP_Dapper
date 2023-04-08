using Entities.Enums;
using Entities.Models;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using WebAPI.Models;

namespace WebAPI.Helpers
{
    [AttributeUsage(AttributeTargets.All)]
    public class JWTAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        private readonly string[] allowedroles;
        public JWTAuthorizeAttribute(string Roles="")
        {
            if (!string.IsNullOrEmpty(Roles))
            {
                this.allowedroles = Roles.Split(",");
            }
        }
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            bool isValid = false;

            IServiceProvider services = context.HttpContext.RequestServices;

            JWTConfig _jwtConfig = services.GetService<JWTConfig>();
            string XAuth = context.HttpContext.Request.Headers["Authorization"];
            XAuth = XAuth ?? string.Empty;
            if (XAuth.StartsWith("."))
            {
                XAuth = XAuth.Remove(0, 1);
            }
            if (!string.IsNullOrEmpty(XAuth))
            {
                XAuth = XAuth.Replace("Bearer ", "");
                isValid = attachUserToContext(context.HttpContext, XAuth, _jwtConfig);
            }
            if (!isValid)
            {
                // not logged in
                context.Result = new JsonResult(new { message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };
            }
        }

        private bool attachUserToContext(HttpContext context, string token, JWTConfig jwtConfig)
        {
            bool isValid = false;
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(jwtConfig.Secretkey);
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var claims = jwtToken.Claims.ToList();
                var loginResponse = new LoginResponse
                {
                    StatusCode = ResponseStatus.Success,
                    ResponseText = nameof(ResponseStatus.Success),
                    Token = token,
                    Id = int.Parse(claims.First(x => x.Type == ClaimTypesExtension.Id).Value),
                    Role = Convert.ToString(claims.First(x => x.Type == ClaimTypesExtension.Role).Value),
                };
                if(allowedroles!=null && allowedroles.Count()>0)
                {
                    int count = 0;
                    if (!string.IsNullOrEmpty(loginResponse.Role))
                    {
                        count = allowedroles.Where(x => x == loginResponse.Role).Count();
                        if (count < 1)
                        {
                            return isValid;
                        }
                    }
                }
                context.Items["User"] = loginResponse;
                isValid = true;
            }
            catch
            {
                // do nothing if jwt validation fails
                // user is not attached to context so request won't have access to secure routes
            }

            return isValid;
        }
    }
}
