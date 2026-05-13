using Application.Enum;

namespace Application.DTOS
{
    public record ResponseViewModel<T>(T? Data,bool IsSuccess,ErrorCode ErrorCode,string? Message = "")
    {
        public static ResponseViewModel<T> Success(T data, string? message = "")
            => new(data, true, ErrorCode.None, message);

        public static ResponseViewModel<T> Failure(ErrorCode errorCode, string? message = null)
            => new(default, false, errorCode, message );
    }
}
