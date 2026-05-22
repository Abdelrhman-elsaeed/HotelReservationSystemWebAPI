using Application.DTOS.RoleFeature;
using Application.ViewModel.RoleFeature;
using AutoMapper;
using Domain.Entities.User;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.AutoMapper.Profiles
{
    public class RoleFeatureProfile : Profile
    {
        public RoleFeatureProfile()
        {
            CreateMap<AssignFeatureToRoleDto, RoleFeature>().ReverseMap();
            CreateMap<AssignFeatureToRoleDto, AssignFeatureToRoleVM>().ReverseMap();

        }
    }
}
