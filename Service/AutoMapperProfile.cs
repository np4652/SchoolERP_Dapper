using AutoMapper;
using Entities.Models;
using Entities;
using Microsoft.Win32;
using Service.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<ApplicationUser, RegisterViewModel>();
            CreateMap<ApplicationUser, UserUpdateRequest>();          
            CreateMap<ApplicationUserProcModel, ApplicationUser>().ReverseMap();
            CreateMap<PaymentGatewayModel, StatusCheckRequest>().ReverseMap();          
        }
    }
}
