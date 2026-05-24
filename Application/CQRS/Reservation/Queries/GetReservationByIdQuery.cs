using Application.AutoMapper.Profiles;
using Application.DTOS;
using Application.DTOS.Reservation;
using Application.Enum;
using Domain.Repositories.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.CQRS.Reservation.Queries
{
    public sealed record GetReservationByIdQuery(int ReservationId) : IRequest<ResponseViewModel<GetReservationDetailsDto>>;
    public class GetReservationByIdQueryHandler : IRequestHandler<GetReservationByIdQuery, ResponseViewModel<GetReservationDetailsDto>>
    {
        private readonly IRepository<Domain.Entities.ReservationManagement.Reservation> _reservationRepository;
        public GetReservationByIdQueryHandler(IRepository<Domain.Entities.ReservationManagement.Reservation> reservationRepository)
        {
            _reservationRepository = reservationRepository;
        }
        public async Task<ResponseViewModel<GetReservationDetailsDto>> Handle(GetReservationByIdQuery request, CancellationToken cancellationToken)
        {
            var reservation = await _reservationRepository.GetByIDAsync(request.ReservationId, cancellationToken);
            if (reservation == null)
                return ResponseViewModel<GetReservationDetailsDto>.Failure(ErrorCode.ReservationNotFound, message: "Reservation not found");


            var dto = reservation.Map<GetReservationDetailsDto>();
            return ResponseViewModel<GetReservationDetailsDto>.Success(dto,message:"Reservation retrieved successfully");
        }
    }
}
