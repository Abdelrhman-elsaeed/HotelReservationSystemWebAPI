using Application.DTOS.RoomType;
using Application.ViewModel.RoomType;
using AutoMapper;
using Domain.Entities.RoomManagement;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.AutoMapper.Profiles
{
    public class RoomTypeProfile : Profile
    {
        public RoomTypeProfile()
        {
            CreateMap<UpdateRoomTypeDto, RoomType>().ReverseMap();
            CreateMap<UpdateRoomTypeDto, UpdateRoomTypeVM>().ReverseMap();
        }
    }
}
