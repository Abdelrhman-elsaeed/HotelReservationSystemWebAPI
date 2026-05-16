using Application.DTOS.Facility;
using Application.DTOS.RoomFacility;
using Application.ViewModel.Facility;
using Application.ViewModel.RoomFacility;
using AutoMapper;
using Domain.Entities;
using Domain.Entities.RoomManagement;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.AutoMapper.Profiles
{
    public class RoomFacilityProfile : Profile
    {
        public RoomFacilityProfile()
        {
            CreateMap<AssignFacilityToRoomDto, RoomFacility>().ReverseMap();
            CreateMap<AssignFacilityToRoomDto, AssignFacilityToRoomVM>().ReverseMap();

            CreateMap<GetRoomFacilitiesDto, RoomFacility>().ReverseMap();
            CreateMap<GetRoomFacilitiesDto, GetRoomFacilitiesVM>().ReverseMap();
        }
    }
}
        