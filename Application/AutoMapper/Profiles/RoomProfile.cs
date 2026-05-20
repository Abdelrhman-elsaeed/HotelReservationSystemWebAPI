using Application.DTOS.Room;
using Application.DTOS.RoomType;
using Application.ViewModel.Room;
using Application.ViewModel.RoomType;
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
            //Room Type
            CreateMap<AddRoomTypeDto, RoomType>().ReverseMap();
            CreateMap<AddRoomTypeDto, AddRoomTypeVM>().ReverseMap();

            CreateMap<GetRoomTypeDto, RoomType>().ReverseMap();
            CreateMap<GetRoomTypeVM, GetRoomTypeDto>().ReverseMap();

            CreateMap<AddRoomDetailsDto, Room>().ReverseMap();
            CreateMap<AddRoomDetailsDto, AddRoomDetailsVM>().ReverseMap();

            CreateMap<UpdateRoomDetailsDto, Room>().ReverseMap();
            CreateMap<UpdateRoomDetailsDto, UpdateRoomDetailsVM>().ReverseMap();

            CreateMap<GetRoomDto, Room>().ReverseMap();
            CreateMap<GetRoomDto, GetRoomVM>().ReverseMap();

        }
   
    }
}
