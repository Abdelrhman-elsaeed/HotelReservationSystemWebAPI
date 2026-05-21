using Application.AutoMapper.Profiles;
using Application.DTOS;
using Application.DTOS.Room;
using Application.Enum;
using Domain.Repositories.Interfaces;
using LinqKit;
using MediatR;
using System.Linq.Expressions;

namespace Application.CQRS.Room.Queries
{
    public sealed record GetAllRoomsQuery(
         DateTime? StartDate
        , DateTime? EndDate
        , int? RoomTypeId
        , int PageNumber = 1
        , int PageSize = 10) : IRequest<ResponseViewModel<IEnumerable<GetRoomDto>>>;


    public class GetAllRoomsQueryHandler : IRequestHandler<GetAllRoomsQuery, ResponseViewModel<IEnumerable<GetRoomDto>>>
    {
        private readonly IRoomRepository _Repository;

        public GetAllRoomsQueryHandler(IRoomRepository Repository)
        {
            _Repository = Repository;
        }

        public async Task<ResponseViewModel<IEnumerable<GetRoomDto>>> Handle(GetAllRoomsQuery request, CancellationToken cancellationToken)
        {
            if (request.PageNumber <= 0 || request.PageSize <= 0)
            {
                return ResponseViewModel<IEnumerable<GetRoomDto>>.Failure(ErrorCode.PaginationFail,message: "PageNumber and PageSize must be greater than zero.");
            }

            var predicate = BuildPredicate(request);
            var rooms = await _Repository.GetRoomsByPredicatePagedAsync(predicate,request.PageNumber,request.PageSize,cancellationToken);

            if (!rooms.Any())
                return ResponseViewModel<IEnumerable<GetRoomDto>>.Failure(ErrorCode.GetAllRoomsFail, message: "There are no rooms found!");

            var RoomsResultDto = rooms.Map<IEnumerable<GetRoomDto>>();

            return ResponseViewModel<IEnumerable<GetRoomDto>>.Success(RoomsResultDto, message: "Rooms retrived successfully");
        }

        private static Expression<Func<Domain.Entities.RoomManagement.Room, bool>>? BuildPredicate(GetAllRoomsQuery request)
        {
            // If no filters are provided, just return null early
            if (!request.RoomTypeId.HasValue && (!request.StartDate.HasValue || !request.EndDate.HasValue))
            {
                return null;
            }

            var predicate = PredicateBuilder.New<Domain.Entities.RoomManagement.Room>(true);

            // Filter by RoomTypeId if provided
            if (request.RoomTypeId.HasValue)
            {
                predicate = predicate.And(r => r.RoomTypeId == request.RoomTypeId);
            }

            // Filter by availability dates
            if (request.StartDate.HasValue && request.EndDate.HasValue)
            {
                predicate = predicate.And(r => !r.ReservationRooms.Any(rr =>
                    rr.Deleted == false &&
                    rr.CheckInDate < request.EndDate &&
                    rr.CheckOutDate > request.StartDate));
            }

            return predicate;
        }
    }

}
