namespace Application.Enum
{
    public enum ErrorCode
    {
        None=0,
        UnExpectedError=1,



        AddRoomTypeFail=100,
        RoomTypeIsExist=101,
        GetRoomTypeFail=102,
        AddRoomDetailsFail=103,
        GetRoomTotalPriceFail=104,
        RoomNotFound=105,
        InvalidDate=106,
        RoomNotAvailable=107,
        RoomTypeNotExist=108,
        UpdateRoomDetailsFail=109,


        AddFacilityFail =201,
        UpdateFacilityFail=202,
        FacilityNotExist=203,
        DeleteFacilityFail = 204,
        RoomFacilityNotExist=205,
        UpdateRoomFacilityFail=206,

        FacilityAssignedBefore=300,
        AssignFacilityToRoomFail=301,


        NoImageUploaded=401,
        UploadImagesFail=402,


        GuestNotFound=500,

        AddReservationFail=600,
    }
}
