using Application.AutoMapper.Profiles;
using Application.CQRS.Auth.Command;
using Application.CQRS.Facility.Command;
using Application.CQRS.Facility.Queries;
using Application.CQRS.Reservation.Command;
using Application.DTOS;
using Application.DTOS.Auth;
using Application.DTOS.Facility;
using Application.DTOS.Reservation;
using Application.DTOS.User;
using Application.Enum;
using Application.ViewModel.Auth;
using Application.ViewModel.Facility;
using Application.ViewModel.Receipt;
using Application.ViewModel.Reservation;
using Domain.Enum;
using HotelReservationSystem.API.Filters;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace HotelReservationSystem.API.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class FacilityController : ControllerBase
    {
        private readonly IMediator _Mediator;

        public FacilityController(IMediator mediator)
        {
            _Mediator = mediator;
        }

        [HttpPost]
        [TypeFilter(typeof(CustomAuthorizeFilter), Arguments = new object[] { Feature.AddFacility})]
        public async Task<IActionResult> AddFacility([FromBody] AddFacilityVM model, CancellationToken cancellationToken)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var FacilityDto = model.Map<AddFacilityDto>();
            var result = await _Mediator.Send(new AddFacilityCommand(FacilityDto), cancellationToken);

            if (result.IsSuccess)
                return Ok(ResponseViewModel<AddFacilityVM>.Success(result.Data.Map<AddFacilityVM>(), result.Message));

            return BadRequest(ResponseViewModel<AddFacilityVM>.Failure(result.ErrorCode, result.Message));
        }

        [HttpPut]
        [TypeFilter(typeof(CustomAuthorizeFilter), Arguments = new object[] { Feature.UpdateFacility })]
        public async Task<IActionResult> UpdateFacility([FromBody] UpdateFacilityVM model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var FacilityDto = model.Map<UpdateFacilityDto>();
            var result = await _Mediator.Send(new UpdateFacilityCommand(FacilityDto), cancellationToken);

            if (result.IsSuccess)
                return Ok(ResponseViewModel<UpdateFacilityVM>.Success(result.Data.Map<UpdateFacilityVM>(), result.Message));

            if (result.ErrorCode == Application.Enum.ErrorCode.FacilityNotExist)
                return NotFound(ResponseViewModel<UpdateFacilityVM>.Failure(result.ErrorCode, result.Message));

            return BadRequest(ResponseViewModel<UpdateFacilityVM>.Failure(result.ErrorCode, result.Message));
        }

        [HttpDelete]
        [TypeFilter(typeof(CustomAuthorizeFilter), Arguments = new object[] { Feature.DeleteFacility })]
        public async Task<IActionResult> DeleteFacility([FromBody] DeleteFacilityVM model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var FacilityDto = model.Map<DeleteFacilityDto>();
            var result = await _Mediator.Send(new DeleteFacilityCommand(FacilityDto), cancellationToken);

            if (result.IsSuccess)
                return Ok(ResponseViewModel<bool>.Success(result.Data, result.Message));

            if (result.ErrorCode == Application.Enum.ErrorCode.FacilityNotExist)
                return NotFound(ResponseViewModel<bool>.Failure(result.ErrorCode, result.Message));

            return BadRequest(ResponseViewModel<bool>.Failure(result.ErrorCode, result.Message));
        }

        [HttpGet("{id}")]
        [TypeFilter(typeof(CustomAuthorizeFilter), Arguments = new object[] { Feature.GetFacility })]
        public async Task<IActionResult> GetFacility(int id, CancellationToken cancellationToken)
        {
            var result = await _Mediator.Send(new GetFacilityQuery(id), cancellationToken);

            if (result.IsSuccess)
                return Ok(ResponseViewModel<GetFacilityVM>.Success(result.Data.Map<GetFacilityVM>(), result.Message));

            if (result.ErrorCode == Application.Enum.ErrorCode.FacilityNotExist)
                return NotFound(ResponseViewModel<GetFacilityVM>.Failure(result.ErrorCode, result.Message));

            return BadRequest(ResponseViewModel<GetFacilityVM>.Failure(result.ErrorCode, result.Message));
        }
    }
}
