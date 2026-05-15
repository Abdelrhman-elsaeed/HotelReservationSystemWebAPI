using Application.DTOS;
using Application.DTOS.RoomFacility;
using Domain.Repositories.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.CQRS.RoomFacility.Queries
{
    //request
    public sealed record GetRoomFacilitiesQuery() : IRequest<ResponseViewModel<IEnumerable<GetRoomFacilitiesDto>>>;

    //request handler
    //public class GetRoomFacilitiesQueryHandler : IRequestHandler<GetRoomFacilitiesQuery, ResponseViewModel<IEnumerable<GetRoomFacilitiesDto>>>
    //{
    //    private readonly IRepository<Domain.Entities.RoomManagement.RoomFacility> _Repository;

    //    public async Task<ResponseViewModel<IEnumerable<GetRoomFacilitiesDto>>> Handle(GetRoomFacilitiesQuery request, CancellationToken cancellationToken)
    //    {
    //        var roomFacilities = await _Repository.GetAll().ToListAsync(cancellationToken);
    //    }
    //}
}
