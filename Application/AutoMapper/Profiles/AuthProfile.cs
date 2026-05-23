using Application.DTOS.User;
using Application.ViewModel.Auth;
using AutoMapper;
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

        }
    }
}
