using Application.DTOS.Auth;
using Application.DTOS.User;
using Application.ViewModel.Auth;
using AutoMapper;
using Domain.Entities.User;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.AutoMapper.Profiles
{
    public class AuthProfile : Profile
    {
        public AuthProfile()
        {
            CreateMap<LoginRequestDto, LoginRequestVM>().ReverseMap();
            CreateMap<LoginResponseDto,LoginResponseVM >().ReverseMap();

            CreateMap<RegisterDto, User>().ForMember(dest => dest.PasswordHash, opt => opt.Ignore());

        }
    }
}
