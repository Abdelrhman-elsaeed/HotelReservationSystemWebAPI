using Application.AutoMapper.Profiles;
using Application.DTOS;
using Application.DTOS.Facility;
using Domain.Entities;
using Domain.Repositories.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.CQRS.Facility.Command
{
    public sealed record AddFacilityCommand(AddFacilityDto model) : IRequest<ResponseViewModel<AddFacilityDto>>;

    public class AddFacilityCommandHandler : IRequestHandler<AddFacilityCommand, ResponseViewModel<AddFacilityDto>>
    {

        private readonly IRepository<Domain.Entities.Facility> _Repository;
        public AddFacilityCommandHandler(IRepository<Domain.Entities.Facility> Repository)
        {
            _Repository = Repository;
        }
        public async Task<ResponseViewModel<AddFacilityDto>> Handle(AddFacilityCommand request, CancellationToken cancellationToken)
        {
            var FacilityEntity = request.model.Map<Domain.Entities.Facility>();
            var result  = await _Repository.AddAsync(FacilityEntity, cancellationToken);

            var IsSaved  = await _Repository.SaveChangesAsync(cancellationToken);

            if (!IsSaved)
                return ResponseViewModel<AddFacilityDto>.Failure(Enum.ErrorCode.AddFacilityFail, message: "Fail to add facility!");

            return ResponseViewModel<AddFacilityDto>.Success(result.Map<AddFacilityDto>(), message: "facility added successfully");
        }
    }
}
