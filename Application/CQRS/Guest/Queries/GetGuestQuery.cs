using Application.AutoMapper.Profiles;
using Application.DTOS;
using Application.DTOS.Guest;
using Domain.Repositories.Interfaces;
using MediatR;

namespace Application.CQRS.Guest.Queries
{
    public sealed record GetGuestQuery(int id) : IRequest<ResponseViewModel<GetGuestDto>>;

    public class GetGuestQueryHandler : IRequestHandler<GetGuestQuery, ResponseViewModel<GetGuestDto>>
    {
        private readonly IRepository<Domain.Entities.Guest.Guest> _repository;

        public GetGuestQueryHandler(IRepository<Domain.Entities.Guest.Guest> repository)
        {
            _repository = repository;
        }

        public async Task<ResponseViewModel<GetGuestDto>> Handle(GetGuestQuery request, CancellationToken cancellationToken)
        {
            var guestEntity = await _repository.GetByIDAsync(request.id, cancellationToken);

            if (guestEntity == null)
            {
                return ResponseViewModel<GetGuestDto>.Failure(Enum.ErrorCode.GetGuestFail, message: "Guest not found");
            }

            var getGuestDto = guestEntity.Map<GetGuestDto>();

            return ResponseViewModel<GetGuestDto>.Success(getGuestDto, message: "Guest retrieved successfully");
        }
    }
}
