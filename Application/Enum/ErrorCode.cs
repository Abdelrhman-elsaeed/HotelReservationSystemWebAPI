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
        UpdateRoomTypeFail=110,
        GetAllRoomsFail=111,
        DeleteRoomDetailsFail=112,


        AddFacilityFail =201,
        UpdateFacilityFail=202,
        FacilityNotExist=203,
        DeleteFacilityFail = 204,
        RoomFacilityNotExist=205,
        UpdateRoomFacilityFail=206,


        FacilityAssignedBefore=300,
        AssignFacilityToRoomFail=301,
        DeleteAllFacilitiesOfRoomFail=302,


        NoImageUploaded=401,
        UploadImagesFail=402,
        RoomPictureNotExist=403,
        DeleteRoomPicturesFail=404,


        GuestNotFound=500,
        AddGuestFail=501,
        DeleteGuestFail=502,
        UpdateGuestFail=503,
        GetGuestFail=504,

        AddReservationFail =600,

        AddOfferFail=700,
        AssigneOfferFail=701,

        PaginationFail=800,

        AssignFeatureToRoleFail=900,
        FeatureAssignedBefore=901,
        HasAccessFail=902,

        UserNotFound=1000,
        InvalidPassword=1001
    }
}
