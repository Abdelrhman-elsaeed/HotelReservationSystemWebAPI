using Application.AutoMapper.Profiles;
using Application.DTOS;
using Application.DTOS.Facility;
using Domain.Repositories.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.CQRS.Facility.Queries
{
    public sealed record GetFacilityQuery(int id) : IRequest<ResponseViewModel<GetFacilityDto>>;

    public class GetFacilityQueryHandler : IRequestHandler<GetFacilityQuery, ResponseViewModel<GetFacilityDto>>
    {

        private readonly IRepository<Domain.Entities.Facility> _Repository;
        public GetFacilityQueryHandler(IRepository<Domain.Entities.Facility> Repository)
        {
            _Repository = Repository;
        }

        public async Task<ResponseViewModel<GetFacilityDto>> Handle(GetFacilityQuery request, CancellationToken cancellationToken)
        {
            var FacilityEntity = await _Repository.GetByIDAsync(request.id, cancellationToken);

            if (FacilityEntity is null)
                return ResponseViewModel<GetFacilityDto>.Failure(Enum.ErrorCode.FacilityNotExist, message: "Facility not Found");

            var FacilityDto = FacilityEntity.Map<GetFacilityDto>();

            return ResponseViewModel<GetFacilityDto>.Success(FacilityDto, message: "Facility retrieved successfully");
        }
    }
}
