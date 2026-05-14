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
    public sealed record UpdateFacilityCommand(UpdateFacilityDto model) : IRequest<ResponseViewModel<UpdateFacilityDto>>;

    public class UpdateFacilityCommandHandler : IRequestHandler<UpdateFacilityCommand, ResponseViewModel<UpdateFacilityDto>>
    {

        private readonly IRepository<Domain.Entities.Facility> _Repository;
        public UpdateFacilityCommandHandler(IRepository<Domain.Entities.Facility> Repository)
        {
            _Repository = Repository;
        }

        public async Task<ResponseViewModel<UpdateFacilityDto>> Handle(UpdateFacilityCommand request, CancellationToken cancellationToken)
        {

            var IsExist = await _Repository.CheckExistsByIDAsync(request.model.ID, cancellationToken);

            if(!IsExist)
                return ResponseViewModel<UpdateFacilityDto>.Failure(Enum.ErrorCode.FacilityNotExist, message: "Facility not Found");

            var FacilityEntity = request.model.Map<Domain.Entities.Facility>();

            _Repository.UpdateInclude(FacilityEntity, nameof(Domain.Entities.Facility.Name), nameof(Domain.Entities.Facility.Price));

            var IsSaved = await _Repository.SaveChangesAsync(cancellationToken);

            if (!IsSaved)
                return ResponseViewModel<UpdateFacilityDto>.Failure(Enum.ErrorCode.UpdateFacilityFail,message:"Facility fail to update");

            return ResponseViewModel<UpdateFacilityDto>.Success(FacilityEntity.Map<UpdateFacilityDto>(), message: "Facility updated successfully");
        }
    }
}
