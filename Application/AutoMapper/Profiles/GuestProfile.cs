using Application.DTOS.Guest;
using Application.ViewModel.Guest;
using AutoMapper;
using Domain.Entities.Guest;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.AutoMapper.Profiles
{
    public class GuestProfile : Profile
    {
        public GuestProfile()
        {
            CreateMap<AddGuestDto, Guest>().ReverseMap();
            CreateMap<AddGuestDto, AddGuestVM>().ReverseMap();

            CreateMap<UpdateGuestDto, Guest>().ReverseMap();
            CreateMap<UpdateGuestDto, UpdateGuestVM>().ReverseMap();
        }
    }
}
