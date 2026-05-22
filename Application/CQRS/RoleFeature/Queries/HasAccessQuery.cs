using Application.DTOS;
using Domain.Enum;
using Domain.Repositories.Interfaces;
using MediatR;

namespace Application.CQRS.RoleFeature.Queries
{
    public sealed record HasAccessQuery(Role role, Feature feature) : IRequest<ResponseViewModel<bool>>;

    public class HasAccessQueryHandler : IRequestHandler<HasAccessQuery, ResponseViewModel<bool>>
    {
        private readonly IRepository<Domain.Entities.User.RoleFeature> _Repository;
        public HasAccessQueryHandler(IRepository<Domain.Entities.User.RoleFeature> Repository)
        {
            _Repository = Repository;
        }

        public async Task<ResponseViewModel<bool>> Handle(HasAccessQuery request, CancellationToken cancellationToken)
        {
            var HasAccess = await _Repository
                .CheckExistsByConditionAsync(rf => rf.Role == request.role 
                && rf.Feature == request.feature
                && !rf.Deleted,cancellationToken);

            if (!HasAccess)
                return ResponseViewModel<bool>.Failure(Enum.ErrorCode.HasAccessFail, message: "Role do not have access to this feature");

            return ResponseViewModel<bool>.Success(HasAccess, message: "Role Can access this feature");
        }
    }
}
