using AutoMapper;
using Entities.Models;

namespace Services.Identity
{
    public class Mappings : Profile
    {
        public Mappings()
        {
            CreateMap<UserStore, ApplicationUser>();
            CreateMap<ApplicationUser, UserStore>();
        }
    }
}
