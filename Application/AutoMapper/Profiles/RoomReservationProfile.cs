using Application.DTOS.Facility;
using Application.DTOS.Reservation;
using Application.DTOS.RoomReservation;
using Application.ViewModel.ReservationRoom;
using AutoMapper;
using Domain.Entities;
using Domain.Entities.ReservationManagement;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.AutoMapper.Profiles
{
    public class RoomReservationProfile : Profile
    {
        public RoomReservationProfile()
        {
            CreateMap<AddReservationRoomDto, ReservationRoom>().ReverseMap();
            CreateMap<AddReservationRoomDto, AddReservationRoomVM>().ReverseMap();
        }
    }
}
