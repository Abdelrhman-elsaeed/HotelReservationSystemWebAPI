using Application.DTOS;
using Application.DTOS.Reservation;
using Domain.Repositories.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.CQRS.ReservationRoom.Queries
{
    //public sealed record GetReservationQuery(int ReservationId) : IRequest<ResponseViewModel<GetReservationDto>>;

    //public class GetReservationQueryHandler : IRequestHandler<GetReservationQuery, ResponseViewModel<GetReservationDto>>
    //{
    //    private readonly IRepository<Domain.Entities.ReservationManagement.ReservationRoom> _Repository;

    //    public GetReservationQueryHandler(IRepository<Domain.Entities.ReservationManagement.ReservationRoom> Repository)
    //    {
    //        _Repository = Repository;
    //    }

    //    public Task<ResponseViewModel<GetReservationDto>> Handle(GetReservationQuery request, CancellationToken cancellationToken)
    //    {
            
    //    }
    //}
}
