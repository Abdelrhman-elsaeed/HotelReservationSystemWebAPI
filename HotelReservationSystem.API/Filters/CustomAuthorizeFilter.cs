using Application.CQRS.RoleFeature.Queries;
using Application.DTOS;
using Application.Enum;
using Domain.Enum;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace HotelReservationSystem.API.Filters
{
    public class CustomAuthorizeFilter : IAsyncActionFilter
    {
        private readonly IMediator _mediator;
        private readonly Feature _requiredFeature;
        public CustomAuthorizeFilter(Feature requiredFeature, IMediator mediator)
        {
            _requiredFeature = requiredFeature;
            _mediator = mediator;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var roleClaim = context.HttpContext.User.FindFirst(ClaimTypes.Role);

            if (roleClaim is null)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            if (!Enum.TryParse<Role>(roleClaim.Value, out Role userRole))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var query = new HasAccessQuery(userRole, _requiredFeature);
            var result = await _mediator.Send(query);

            if (!result.IsSuccess)
            {
                context.Result = new ObjectResult(ResponseViewModel<string>.Failure(result.ErrorCode, result.Message))
                {
                    StatusCode = StatusCodes.Status403Forbidden
                };
                return;
            }

            await next();
        }
    }
}
