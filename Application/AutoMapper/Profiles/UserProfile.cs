using Application.DTOS.User;
using Application.ViewModel.User;
using AutoMapper;
using Domain.Entities.User;

namespace Application.AutoMapper.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<AddUserDto, User>().ForMember(dest => dest.PasswordHash, opt => opt.Ignore());
            CreateMap<AddUserDto, AddUserVM>().ReverseMap();


            CreateMap<AddUserResponseDto, AddUserResponseVM>().ReverseMap();

        }
    }
}
