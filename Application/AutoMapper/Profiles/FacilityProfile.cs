using Application.DTOS.Facility;
using Application.DTOS.Room;
using Application.ViewModel.Facility;
using AutoMapper;
using Domain.Entities;
using Domain.Entities.RoomManagement;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.AutoMapper.Profiles
{
    public class FacilityProfile : Profile
    {
        public FacilityProfile()
        {
            CreateMap<AddFacilityDto, Facility>().ReverseMap();
            CreateMap<AddFacilityDto, AddFacilityVM>().ReverseMap();

            CreateMap<UpdateFacilityDto, Facility>().ReverseMap();
            CreateMap<UpdateFacilityDto, UpdateFacilityVM>().ReverseMap();
            
            CreateMap<DeleteFacilityDto, Facility>().ReverseMap();
            CreateMap<DeleteFacilityDto, DeleteFacilityVM>().ReverseMap();

            CreateMap<GetFacilityDto, Facility>().ReverseMap();
            CreateMap<GetFacilityDto, GetFacilityVM>().ReverseMap();
        }
    }
}
