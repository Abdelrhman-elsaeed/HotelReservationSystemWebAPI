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
        }
    }
}
