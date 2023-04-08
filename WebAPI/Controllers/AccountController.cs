using Entities.Enums;
using Entities.Models;
using Infrastructure.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Identity;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using System.Net;
using WebAPI.Models;
using WebAPI.Helpers;
using Data;
using System.ComponentModel.DataAnnotations;
using AppUtility.Helper;
using System.IdentityModel.Tokens.Jwt;
using Entities;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("/api/")]
    public class AccountController : ControllerBase
    {
        #region Variables
        private readonly ApplicationUserManager _userManager;
        private readonly ILogger<AccountController> _logger;
        private readonly ITokenService _tokenService;
        private readonly INotifyService _notify;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly SignInManager<ApplicationUser> _signInManager;

        #endregion

        public AccountController(ApplicationUserManager userManager, ITokenService tokenService, ILogger<AccountController> logger, IHttpContextAccessor httpContextAccessor, INotifyService notify)
        {
            _logger = logger;
            _userManager = userManager;
            _notify = notify;
            _tokenService = tokenService;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _signInManager = _httpContextAccessor.HttpContext.RequestServices.GetService<SignInManager<ApplicationUser>>();
        }

        [ProducesResponseType(typeof(LoginViewModel), (int)HttpStatusCode.OK)]
        [HttpPost(nameof(Login))]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            bool isReqFromApp = false;
            OSInfo oSInfo = new OSInfo(_httpContextAccessor);
            if (string.IsNullOrEmpty(oSInfo.FullInfo))
            {
                isReqFromApp = true;
            }
            var res = new Response<AuthenticateResponse>
            {
                StatusCode = ResponseStatus.Failed,
                ResponseText = "Invalid Credentials"
            };
            try
            {
                var result = await _signInManager.PasswordSignInAsync(model.MobileNo, model.Password, model.RememberMe, lockoutOnFailure: true);
                if (result.RequiresTwoFactor)
                {
                    return RedirectToAction(nameof(LoginTwoStep), new
                    {
                        model.MobileNo,
                        model.RememberMe
                    });
                }
                else if (result.Succeeded)
                {
                    res = await GenerateAccessToken(model.MobileNo);
                    if (res.Result.Role.ToUpper()=="ADMIN" && isReqFromApp)
                    {
                        return Unauthorized("This device is not authorized");
                    }
                }
            }
            catch (Exception ex)
            {
                res.ResponseText = "Something went wrong.Please try after some time.";
                _logger.LogError(ex, ex.Message);
            }
            return Ok(res);
        }

        [HttpPost(nameof(SendLoginOTP))]
        public async Task<IActionResult> SendLoginOTP([Required][RegularExpression(StringFormats.MobileRegex)] string mobileNo)
        {
            var res = new Response<AuthenticateResponse>
            {
                StatusCode = ResponseStatus.Failed,
                ResponseText = "Invalid OTP"
            };
            try
            {
                var password = Utility.O.GenrateRandom(6, true);
                /* Send SMS here */
                var result = await _notify.SaveSMSEmailWhatsappNotification(new SMSEmailWhatsappNotification
                {
                    FormatID = MessageFormat.OTP,
                    IsSms = true,
                    IsWhatsapp = true,
                    PhoneNumber = mobileNo,
                    OTP = password,
                    Action = "LOGINOTP",
                    IsOTP= true
                }, 0);
                /* End SMS */
                return Ok(result);
            }
            catch (Exception ex)
            {
                res.ResponseText = "Something went wrong.Please try after some time.";
                _logger.LogError(ex, ex.Message);
            }
            return Ok(res);
        }


        [AllowAnonymous]
        [HttpPost(nameof(LoginWithOTP))]
        public async Task<IActionResult> LoginWithOTP([Required][RegularExpression(StringFormats.MobileRegex)] string mobileNo, [Required][RegularExpression(StringFormats.NumberOnly)] string otp)
        {
            bool isReqFromApp = false;
            OSInfo oSInfo = new OSInfo(_httpContextAccessor);
            if (string.IsNullOrEmpty(oSInfo.FullInfo))
            {
                isReqFromApp = true;
            }
            var res = new Response<AuthenticateResponse>
            {
                StatusCode = ResponseStatus.Failed,
                ResponseText = "Invalid OTP"
            };
            try
            {
                var result = await _userManager.SigninWithOTP(mobileNo, otp, lockoutOnFailure: true);
                if (result.Succeeded)
                {
                    res = await GenerateAccessToken(mobileNo);
                    if (res.Result.Role.ToUpper()=="ADMIN" && isReqFromApp)
                    {
                        return Unauthorized("This device is not authorized");
                    }
                }
            }
            catch (Exception ex)
            {
                res.ResponseText = "Something went wrong.Please try after some time.";
                _logger.LogError(ex, ex.Message);
            }
            return Ok(res);
        }

        private async Task<Response<AuthenticateResponse>> GenerateAccessToken(string mobileNo)
        {
            var res = new Response<AuthenticateResponse>
            {
                StatusCode = ResponseStatus.Failed,
                ResponseText = "Invalid Credentials"
            };
            var user = await _userManager.FindByMobileNoAsync(mobileNo);
            if (user.Id>0)
            {
                var claims = new[] {
                        new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(ClaimTypesExtension.Id, user.Id.ToString()),
                        new Claim(ClaimTypesExtension.Role, user.Role ?? "2" ),
                        new Claim(ClaimTypes.Role, user.Role ?? "2" ),
                        new Claim(ClaimTypesExtension.UserName, user.UserName),
                        new Claim(ClaimTypesExtension.BussinessType, user.BuisnessTypes ?? "")
                    };
                var token = _tokenService.GenerateAccessToken(claims);
                var authResponse = new AuthenticateResponse(user, token);
                res.StatusCode = ResponseStatus.Success;
                res.ResponseText = "Login Succussful";
                res.Result = authResponse;
            }
            return res;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet(nameof(LoginTwoStep))]
        public async Task<IActionResult> LoginTwoStep(string email, bool rememberMe, string returnUrl = null)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return BadRequest("Invalid attempt");
            }
            var providers = await _userManager.GetValidTwoFactorProvidersAsync(user);
            if (!providers.Contains("Email"))
            {
                return BadRequest("Invalid attempt");
            }
            var token = await _userManager.GenerateTwoFactorTokenAsync(user, "Email");

            //var message = new Message(new string[] { email }, "Authentication token", token, null);

            //await _emailSender.SendEmailAsync(message);

            return Ok(new { StatusCode = -3, ResponseText = "Two factor verification needed" });
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost(nameof(LoginTwoStep))]
        public async Task<IActionResult> LoginTwoStep(TwoStepModel twoStepModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(twoStepModel);
            }
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                //return RedirectToAction(nameof(LogError));
                return BadRequest("LogError");
            }
            var result = await _signInManager.TwoFactorSignInAsync("Email", twoStepModel.TwoFactorCode, twoStepModel.RememberMe, rememberClient: false);
            if (result.Succeeded)
            {
                //return RedirectToLocal(returnUrl);
                return Ok("Success");
            }
            else if (result.IsLockedOut)
            {
                //Same logic as in the Login action
                ModelState.AddModelError("", "The account is locked out");
                return BadRequest("The account is locked out");
            }
            else
            {
                ModelState.AddModelError("", "Invalid Login Attempt");
                return BadRequest("Invalid Login Attempt");
            }
        }

        [HttpPost("/token/refresh")]
        public async Task<IActionResult> RefreshAsync(TokenApiModel tokenApiModel)
        {
            var response = new Response<AuthenticatedResponse>
            {
                StatusCode = ResponseStatus.Failed,
                ResponseText = "Invalid client request",
            };
            if (tokenApiModel is null)
                return Ok(response); //return BadRequest("Invalid client request");
            string accessToken = tokenApiModel.AccessToken;
            string refreshToken = tokenApiModel.RefreshToken;
            var principal = _tokenService.GetPrincipalFromExpiredToken(accessToken);
            var id = principal.FindFirstValue("Id");
            var user = await _userManager.FindByIdAsync(id);
            if (user is null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
            {
                response.StatusCode = ResponseStatus.TokenExpired;
                response.ResponseText = ResponseStatus.TokenExpired.ToString();
                return Ok(response);//return BadRequest("Invalid client request");
            }
            var newAccessToken = _tokenService.GenerateAccessToken(principal.Claims);
            var newRefreshToken = _tokenService.GenerateRefreshToken();
            user.RefreshToken = newRefreshToken.RefreshToken;
            await _userManager.UpdateAsync(user);
            return Ok(new Response<AuthenticatedResponse>
            {
                StatusCode = ResponseStatus.Success,
                ResponseText = ResponseStatus.Success.ToString(),
                Result = new AuthenticatedResponse()
                {
                    Token = newAccessToken,
                    RefreshToken = newRefreshToken.RefreshToken
                }
            });
        }

        [HttpPost, Authorize]
        [Route(nameof(Revoke))]
        public IActionResult Revoke()
        {
            var username = User.Identity.Name;
            return NoContent();
        }

        [HttpPost(nameof(SendRegistrationOTP))]
        public async Task<IActionResult> SendRegistrationOTP([Required][RegularExpression(StringFormats.MobileRegex)] string mobileNo)
        {
            var res = new Response<AuthenticateResponse>
            {
                StatusCode = ResponseStatus.Failed,
                ResponseText = "Invalid OTP"
            };
            try
            {
                var password = Utility.O.GenrateRandom(6, true);
                /* Send SMS here */
                var result = await _notify.SaveSMSEmailWhatsappNotification(new SMSEmailWhatsappNotification
                {
                    FormatID = MessageFormat.OTP,
                    IsSms = true,
                    IsWhatsapp = true,
                    PhoneNumber = mobileNo,
                    OTP = password,
                    Action="REGISTRATION",
                    IsOTP = true
                }, 0);
                /* End SMS */
                return Ok(result);
            }
            catch (Exception ex)
            {
                res.ResponseText = "Something went wrong.Please try after some time.";
                _logger.LogError(ex, ex.Message);
            }
            return Ok(res);
        }

        /// <summary>
        /// Creates a customer.
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /api/register
        ///     {
        ///         "id": 0,
        ///         "name": "Amit",
        ///         "phoneNumber": "9936301548",
        ///         "password": "123456",
        ///         "confirmPassword": "123456"
        ///     }
        /// </remarks>
        /// <param name="employee"></param>
        /// <returns>A newly created customer</returns>
        /// <response code="200">Returns on success</response>
        /// <response code="400">If the item is null</response> 
        [HttpPost(nameof(Register))]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.BadRequest)]
        [Produces("application/json")]
        public async Task<IActionResult> Register(RegisterViewModels model)
        {
            var userExists = await _userManager.FindByMobileNoAsync(model.PhoneNumber);
            if (userExists?.Id > 0 && userExists.ToString() != "{}")
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { StatusCode = ResponseStatus.Failed, ResponseText = "User already exists!" });
            ApplicationUser user = new ApplicationUser()
            {
                SecurityStamp = Guid.NewGuid().ToString(),
                UserId = Guid.NewGuid().ToString(),
                UserName = model.PhoneNumber.Trim(),
                Email = model.Email ?? $"{model.PhoneNumber}@kisaantreat.com",
                Role = model.UserRole.ToString(),//UserRoles.User.ToString(),
                Name = model.Name,
                PhoneNumber = model.PhoneNumber.Trim(),
                RefreshToken = Guid.NewGuid().ToString().Replace("-", ""),
                RefreshTokenExpiryTime = DateTime.Now.AddDays(30),
                Gender = model.Gender,
            };
            string password = Utility.O.GenrateRandom(8);
            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                string responseText = "User creation failed! Please check user details and try again.";
                if (result.Errors!=null && result.Errors.Count() > 0)
                {
                    responseText = result.Errors.FirstOrDefault()?.Description;
                }
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { StatusCode = ResponseStatus.Failed, ResponseText = responseText });

            }

            #region send Notification

            _notify.SaveSMSEmailWhatsappNotification(new SMSEmailWhatsappNotification()
            {
                FormatID = MessageFormat.Registration,
                Password=password,
                IsSms = true,
                IsWhatsapp= true,
            }, User.GetLoggedInUserId<int>());
            #endregion
            return Ok(new Response { StatusCode = ResponseStatus.Success, ResponseText = "User created successfully!" });
        }
    }
}
