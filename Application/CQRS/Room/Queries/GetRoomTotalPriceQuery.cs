using Application.DTOS;
using Domain.Repositories.Interfaces;
using HotelReservationSystem.API.Helper.BusinessExceptions;
using MediatR;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Application.CQRS.Room.Queries
{
    public sealed record GetRoomTotalPriceQuery(int roomId) : IRequest<ResponseViewModel<decimal?>>;

    public class GetRoomTotalPriceQueryHandler : IRequestHandler<GetRoomTotalPriceQuery, ResponseViewModel<decimal?>>
    {
        private readonly IRoomRepository _RoomRepository;

        public GetRoomTotalPriceQueryHandler(IRoomRepository RoomRepository)
        {
            _RoomRepository = RoomRepository;

        }
        public async Task<ResponseViewModel<decimal?>> Handle(GetRoomTotalPriceQuery request, CancellationToken cancellationToken)
        {

            var IsExist = await _RoomRepository.CheckExistsByIDAsync(request.roomId, cancellationToken);
            if (!IsExist)
                return ResponseViewModel<decimal?>.Failure(Enum.ErrorCode.RoomNotFound, message: "Room not found!");


            var TotalPrice = await _RoomRepository.GetRoomTotalPriceAsync(request.roomId, cancellationToken);
            
            if(TotalPrice is null)
                return ResponseViewModel<decimal?>.Failure(Enum.ErrorCode.GetRoomTotalPriceFail, message: "Fail to get room total price!");

            return ResponseViewModel<decimal?>.Success(TotalPrice, message: "Total price retrieved successfully");

        }
    }
}
