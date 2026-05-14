using Application.AutoMapper.Profiles;
using Application.DTOS;
using Application.DTOS.Facility;
using Domain.Repositories.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.CQRS.Facility.Command
{
    public sealed record DeleteFacilityCommand(DeleteFacilityDto model) : IRequest<ResponseViewModel<bool>>;

    public class DeleteFacilityCommandHandler : IRequestHandler<DeleteFacilityCommand, ResponseViewModel<bool>>
    {

        private readonly IRepository<Domain.Entities.Facility> _Repository;
        public DeleteFacilityCommandHandler(IRepository<Domain.Entities.Facility> Repository)
        {
            _Repository = Repository;
        }

        public async Task<ResponseViewModel<bool>> Handle(DeleteFacilityCommand request, CancellationToken cancellationToken)
        {
            var IsExist = await _Repository.CheckExistsByIDAsync(request.model.ID, cancellationToken);

            if (!IsExist)
                return ResponseViewModel<bool>.Failure(Enum.ErrorCode.FacilityNotExist, message: "Facility not Found");

            var FacilityEntity = request.model.Map<Domain.Entities.Facility>();

            _Repository.SoftDelete(FacilityEntity);

            var IsSaved = await _Repository.SaveChangesAsync(cancellationToken);

            if (!IsSaved)
                return ResponseViewModel<bool>.Failure(Enum.ErrorCode.DeleteFacilityFail, message: "delete facility fail");

            return ResponseViewModel<bool>.Success(true, message: "facility deleted successfully");
        }
    }
}
