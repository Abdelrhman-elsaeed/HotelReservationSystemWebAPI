using Application.DTOS.Facility;
using Application.DTOS.Reservation;
using Application.ViewModel.Reservation;
using AutoMapper;
using Domain.Entities;
using Domain.Entities.ReservationManagement;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.AutoMapper.Profiles
{
    public class ReservationProfile : Profile
    {
        public ReservationProfile()
        {
            CreateMap<AddReservationDto, Reservation>().ReverseMap();
            CreateMap<AddReservationDto, AddReservationVM>().ReverseMap();

            CreateMap<Reservation, GetReservationDetailsDto>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedDate));
        }
    }
}
