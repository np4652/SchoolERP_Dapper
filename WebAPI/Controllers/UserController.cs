using AppUtility.Helper;
using AutoMapper;
using Data;
using Entities;
using Entities.Enums;
using Entities.Models;
using Infrastructure.Interface;
using Microsoft.AspNetCore.Mvc;
using Service.Identity;
using Service.PaymentGateway;
using System.ComponentModel.DataAnnotations;
using System.Net;
using WebAPI.Helpers;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    [ApiController]
    [JWTAuthorize]
    [Route("api/user/")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _users;
        private readonly IMapper _mapper;
        private readonly ApplicationUserManager _userManager;
        private readonly ApplicationUser User;
        private readonly INotifyService _notify;
        private readonly IPaymentGatewayService _pgService;
        private readonly IPGDetailService _pgDetail;
        
        public UserController(IHttpContextAccessor httpContext, IUserService users, IMapper mapper, ApplicationUserManager userManager, INotifyService notify, IPaymentGatewayService pgService,IPGDetailService pgDetail)
        {
            _users = users;
            _mapper = mapper;
            _userManager = userManager;
            _notify = notify;
            _pgService = pgService;
            _pgDetail = pgDetail;
            if (httpContext != null && httpContext.HttpContext != null)
            {
                LoginResponse loginResponse = (LoginResponse)httpContext?.HttpContext.Items["User"];
                if (loginResponse != null)
                {
                    User = new ApplicationUser
                    {
                        Id= loginResponse.Id,
                        Role = loginResponse.Role
                    };
                }
            }
            //_package=package;
            //var role = httpContext.GetLoggedInUserRole(jwtConfig.Value?.Secretkey);
        }
        [JWTAuthorize("Admin,User")]
        [HttpPost(nameof(List))]
        public async Task<IActionResult> List()
        {
            if (User.Role==UserRoles.Admin.ToString())
            {
                var res = await _users.GetAllAsync(User.Id, new ApplicationUser());
                var users = new List<ApplicationUserResponse>();
                foreach (var item in res)
                {
                    users.Add(new ApplicationUserResponse
                    {
                        Id = item.Id,
                        name= item.Name,
                        UserName= item.UserName,
                        Email = item.Email,
                        PhoneNumber = item.PhoneNumber,
                        Balance = item.Balance,
                        EmailConfirmed= item.EmailConfirmed,
                        IsActive= item.IsActive,
                        LockoutEnabled= item.LockoutEnabled,
                        Role=item.Role,
                        TwoFactorEnabled= item.TwoFactorEnabled,
                        RoleId = item.RoleId,
                        BussinessType = item.BuisnessTypes
                    });
                    //users.Add(_mapper.Map<ApplicationUserResponse>(item));
                }
                return Ok(users ?? new List<ApplicationUserResponse>());
            }
            return Unauthorized();
        }

        [JWTAuthorize("Admin,User")]
        [HttpPost(nameof(GetById))]
        public async Task<ApplicationUser> GetById(int id = -1)
        {
            id = id==-1 ? User.Id : id;
            var users = await _users.GetByIdAsync(id);
            return users.Result ?? new ApplicationUser();
        }

        [JWTAuthorize("Admin,User")]
        [HttpPost(nameof(GetUserDetail))]
        public async Task<UserDetail> GetUserDetail()
        {
            var users = await _users.GetUserDetailAsync(User.Id);
            return users ?? new UserDetail();
        }


        [JWTAuthorize("Admin,User")]
        [HttpPost(nameof(Update))]
        public async Task<IResponse> Update(UserDetailRequest model)
        {
            IResponse response = new Response()
            {
                StatusCode = ResponseStatus.Failed,
                ResponseText = "Updation Failed"
            };
            if (model.Id > 0)
            {
                var user = new ApplicationUser
                {
                    Id = model.Id,
                    Name = model.Name,
                    PhoneNumber = model.PhoneNumber,
                    Email = model.Email,
                    PasswordHash = model.PasswordHash,
                    TwoFactorEnabled = model.TwoFactorEnabled,
                    RefreshToken = model.RefreshToken,
                    RefreshTokenExpiryTime  = model.RefreshTokenExpiryTime
                };
                response = await _users.AddAsync(user);
            }
            return response;
        }

        /// <summary>
        /// Creates a DeliveryPerson.
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /api/user/register
        ///     {
        ///         "id": 0,
        ///         "name": "Amit",
        ///         "phoneNumber": "9936301548"
        ///     }
        /// </remarks>
        /// <param name="employee"></param>
        /// <returns>A newly created DeliveryPerson</returns>
        /// <response code="200">Returns on success</response>
        /// <response code="400">If the item is null</response> 
        [JWTAuthorize("Admin")]
        [HttpPost(nameof(CreateDeliveryPerson))]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(Response), (int)HttpStatusCode.BadRequest)]
        [Produces("application/json")]
        public async Task<IActionResult> CreateDeliveryPerson(RegisterViewModels model)
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
                Role = UserRoles.DeliveryPerson.ToString(),
                Name = model.Name,
                PhoneNumber = model.PhoneNumber.Trim(),
                RefreshToken = Guid.NewGuid().ToString().Replace("-", ""),
                RefreshTokenExpiryTime = DateTime.Now.AddDays(30)
            };

            string password = "Welcome@1";//Utility.O.GenrateRandom(8);
            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { StatusCode = ResponseStatus.Failed, ResponseText = "User creation failed! Please check user details and try again." });
            #region send Notification

            _notify.SaveSMSEmailWhatsappNotification(new SMSEmailWhatsappNotification()
            {
                FormatID = MessageFormat.Registration,
                Password= password,
                IsWhatsapp = true,
                IsSms = true
            }, User.Id);
            #endregion
            return Ok(new Response { StatusCode = ResponseStatus.Success, ResponseText = "User created successfully!" });
        }
        
    }
}