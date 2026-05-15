using Application.DTOS.Room;
using Application.DTOS.RoomPicture;
using Application.ViewModel.Room;
using Application.ViewModel.RoomPicture;
using AutoMapper;
using Domain.Entities.RoomManagement;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.AutoMapper.Profiles
{
    public class RoomPictureProfile : Profile
    {
        public RoomPictureProfile()
        {
            //Room Type
            CreateMap<FileUploadDto, FileUploadVM>().ReverseMap();

            //Room
        }
   
    }
}
