using Application.Enum;

namespace Application.DTOS
{
    public record ResponseViewModel<T>(T? Data,bool IsSuccess,string? Message = "",ErrorCode? ErrorCode = null)
    {
        public static ResponseViewModel<T> Success(T data, string? message = "")
            => new(data, true, message, null);

        public static ResponseViewModel<T> Failure(ErrorCode errorCode, string? message = null)
            => new(default, false, message, errorCode);
    }
}
