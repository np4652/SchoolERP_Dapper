using AutoMapper;
using Entities.Models;
using Service.EmailConfig;

namespace Service.Identity
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<ApplicationUser, RegisterViewModel>().ReverseMap();
            CreateMap<ApplicationUser, UserUpdateRequest>().ReverseMap();
            CreateMap<ApplicationUser, RegisterModel>().ReverseMap(); 
            CreateMap<ApplicationUserProcModel, ApplicationUser>().ReverseMap();
            CreateMap<ApplicationUser, ApplicationUserResponse>().ReverseMap();
            CreateMap<List<ApplicationUser>, List<ApplicationUserResponse>>().ReverseMap();
        }
    }
}
