using Application.DTOS.Room;
using AutoMapper;
using Domain.Entities.RoomManagement;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.AutoMapper.Profiles
{
    public class RoomProfile : Profile
    {
        public RoomProfile()
        {
            CreateMap<AddRoomTypeDto, RoomType>().ReverseMap();
        }
   
    }
}
