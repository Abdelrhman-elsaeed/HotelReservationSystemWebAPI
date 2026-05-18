using Application.DTOS.Facility;
using Application.DTOS.Receipt;
using Application.ViewModel.Receipt;
using AutoMapper;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.AutoMapper.Profiles
{
    public class ReceiptProfile : Profile
    {
        public ReceiptProfile()
        {
            CreateMap<RoomReceiptDto, RoomReceiptVM>().ReverseMap();
            CreateMap<ReservationReceiptDto, ReservationReceiptVM>().ReverseMap();
        }
    }
}
