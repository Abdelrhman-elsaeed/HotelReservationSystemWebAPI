namespace Application.Enum
{
    public enum ErrorCode
    {
        None=0,
        UnExpectedError=1,



        AddRoomTypeFail=100,
        RoomTypeIsExist=101,
        GetRoomTypeFail=102,


        AddFacilityFail=201,
        UpdateFacilityFail=202,
        FacilityNotExist=203,
        DeleteFacilityFail = 204,

        FacilityAssignedBefore=300,
        AssignFacilityToRoomFail=301,


        NoImageUploaded=401,
        UploadImagesFail=402
    }
}
