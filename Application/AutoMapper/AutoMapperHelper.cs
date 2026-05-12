
using AutoMapper;
using AutoMapper.QueryableExtensions;

namespace Application.AutoMapper.Profiles
{
    public static class AutoMapperHelper
    {
        public static IMapper Mapper { get; set; }

        public static T Map<T>(this object source)
        {
            return Mapper.Map<T>(source);
        }

        public static IQueryable<T> Project<T>(this IQueryable source)
        {
            return source.ProjectTo<T>(Mapper.ConfigurationProvider);
        }
    }
}
