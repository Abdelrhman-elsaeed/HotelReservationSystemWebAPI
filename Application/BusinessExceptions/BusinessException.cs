using Application.Enum;

namespace HotelReservationSystem.API.Helper.BusinessExceptions
{
    public class BusinessException : Exception
    {
        public ErrorCode ErrorCode { get; }

        public BusinessException(ErrorCode errorCode, string message) : base(message)
        {
            ErrorCode = errorCode;
        }
    }
}
