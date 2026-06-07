using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Enum
{
    public enum Feature
    {
        //Facility Feature
        AddFacility = 0,
        UpdateFacility = 1,
        DeleteFacility = 2,
        GetFacility = 3,

        //Room Features
        AddEntireRoom = 4,
        AddRoomType = 5,
        GetAllRooms = 6,
        AddRoomDetails = 7,
        UpdateRoomDetails = 8,
        UpdateRoomType = 9,
        GetRoomType = 10,
        DeleteEntireRoom = 11,

        //Reservation Features
        AddReservation = 12,
        UpdateReservation = 13,
        CancelReservation = 14,
        GetReservation = 15,

        //Guest Features
        AddGuest = 16,
        UpdateGuest = 17,
        DeleteGuest = 18,
        GetGuest = 19,

        //User & Auth Features
        AddUser = 20,
        Logout = 21,
        Login = 22,

        //RoomPicture Features
        UploadImages = 23

    }
}
