using Application.AutoMapper.Profiles;
using Application.DTOS;
using Application.DTOS.RoleFeature;
using Application.Helper.Caching;
using Domain.Repositories.Interfaces;
using MediatR;
using Microsoft.Extensions.Caching.Memory;


namespace Application.CQRS.RoleFeature.Command
{
    public sealed record AssignFeatureToRoleCommand(AssignFeatureToRoleDto model) : IRequest<ResponseViewModel<AssignFeatureToRoleDto>>;

    public class AssignFeatureToRoleCommandHandler:IRequestHandler<AssignFeatureToRoleCommand, ResponseViewModel<AssignFeatureToRoleDto>>
    {
        private readonly IRepository<Domain.Entities.User.RoleFeature> _Repository;
        private readonly IMemoryCache _memoryCache;
        public AssignFeatureToRoleCommandHandler(IRepository<Domain.Entities.User.RoleFeature> Repository, IMemoryCache memoryCache)
        {
            _Repository = Repository;
            _memoryCache = memoryCache;
        }

        public async Task<ResponseViewModel<AssignFeatureToRoleDto>> Handle(AssignFeatureToRoleCommand request, CancellationToken cancellationToken)
        {
            // validate Feature if assigned before
            var IsFeatureAssignedBefore = await _Repository.CheckExistsByConditionAsync(rf => rf.Feature == request.model.Feature && rf.Role == request.model.Role);

            if (IsFeatureAssignedBefore)
                return ResponseViewModel<AssignFeatureToRoleDto>.Failure(Enum.ErrorCode.FeatureAssignedBefore, message: "This feature assigned before to this role");

            var Entity = request.model.Map<Domain.Entities.User.RoleFeature>();
            var AddedEntity = await _Repository.AddAsync(Entity, cancellationToken);

            var IsSaved = await _Repository.SaveChangesAsync(cancellationToken);

            if (!IsSaved)
                return ResponseViewModel<AssignFeatureToRoleDto>.Failure(Enum.ErrorCode.AssignFeatureToRoleFail, message: "Fail to assign feature to this role");


            _memoryCache.Remove(RoleFeatureCacheKeys.RoleFeaturesDictionary);


            return ResponseViewModel<AssignFeatureToRoleDto>.Success(AddedEntity.Map<AssignFeatureToRoleDto>(), message: "Feature assigned successfully");
        }
    }
}
