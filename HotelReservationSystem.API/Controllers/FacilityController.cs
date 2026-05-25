using Application.AutoMapper.Profiles;
using Application.CQRS.Auth.Command;
using Application.CQRS.Reservation.Command;
using Application.DTOS;
using Application.DTOS.Auth;
using Application.DTOS.Reservation;
using Application.DTOS.User;
using Application.Enum;
using Application.ViewModel.Auth;
using Application.ViewModel.Receipt;
using Application.ViewModel.Reservation;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace HotelReservationSystem.API.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class FacilityController : ControllerBase
    {
        private readonly IMediator _Mediator;

        public FacilityController(IMediator mediator)
        {
            _Mediator = mediator;
        }


    }
}
