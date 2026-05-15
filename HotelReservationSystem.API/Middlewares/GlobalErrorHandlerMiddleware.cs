using Application.DTOS;
using Application.Enum;
using HotelReservationSystem.API.Helper.BusinessExceptions;

namespace HotelReservationSystem.API.Middlewares
{
    public class GlobalErrorHandlerMiddleware : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (BusinessException ex)
            {
                var response = ResponseViewModel<bool>.Failure(ex.ErrorCode, ex.Message);

                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(response);
            }
            catch (Exception ex)
            {
                var response = ResponseViewModel<bool>.Failure(ErrorCode.UnExpectedError,"An unexpected error occurred. Please try again later."
                );
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(response);
            }
        }
    }
}
