using Application.DTOS;
using Application.Helper.Caching;
using Domain.Enum;
using Domain.Repositories.Interfaces;
using MediatR;
using Microsoft.Extensions.Caching.Memory;

namespace Application.CQRS.RoleFeature.Queries
{
    public sealed record HasAccessQuery(Role role, Feature feature)
        : IRequest<ResponseViewModel<bool>>;

    public class HasAccessQueryHandler : IRequestHandler<HasAccessQuery, ResponseViewModel<bool>>
    {

        private readonly IRepository<Domain.Entities.User.RoleFeature> _repository;
        private readonly IMemoryCache _memoryCache;

        public HasAccessQueryHandler(IRepository<Domain.Entities.User.RoleFeature> repository,IMemoryCache memoryCache)
        {
            _repository = repository;
            _memoryCache = memoryCache;
        }

        public async Task<ResponseViewModel<bool>> Handle(HasAccessQuery request,CancellationToken cancellationToken)
        {
            var roleFeaturesDictionary = await GetRoleFeaturesDictionaryAsync(cancellationToken);

            var hasAccess =roleFeaturesDictionary.TryGetValue(request.role, out var features) && features.Contains(request.feature);

            return ResponseViewModel<bool>.Success(hasAccess,hasAccess? "Role can access this feature" : "Role does not have access to this feature");
        }

        private async Task<Dictionary<Role, HashSet<Feature>>> GetRoleFeaturesDictionaryAsync(CancellationToken cancellationToken)
        {
            if (_memoryCache.TryGetValue(RoleFeatureCacheKeys.RoleFeaturesDictionary, out Dictionary<Role, HashSet<Feature>>? cachedDictionary) && cachedDictionary is not null)
            {
                return cachedDictionary;
            }

            var roleFeatures = await _repository.GetAllByConditionAsync(x=>!x.Deleted,cancellationToken);

            var dictionary = roleFeatures.GroupBy(rf => rf.Role)
                .ToDictionary(group => group.Key,group => group.Select(rf => rf.Feature).ToHashSet());

            var cacheOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(15)).SetAbsoluteExpiration(TimeSpan.FromHours(1));

            _memoryCache.Set(RoleFeatureCacheKeys.RoleFeaturesDictionary, dictionary, cacheOptions);

            return dictionary;
        }
    }
}