using Application.DTOS.Offer;
using Application.ViewModel.Offer;
using AutoMapper;
using Domain.Entities;

namespace Application.AutoMapper.Profiles
{
    public class OfferProfile:Profile
    {
        public OfferProfile()
        {
            CreateMap<AddOfferDto, Offer>().ReverseMap();
            CreateMap<AddOfferDto, AddOfferVM>().ReverseMap();
        }
    }
}
