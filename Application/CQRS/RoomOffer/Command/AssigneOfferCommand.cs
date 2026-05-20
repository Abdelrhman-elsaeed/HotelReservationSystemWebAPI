using Application.AutoMapper.Profiles;
using Application.DTOS;
using Application.DTOS.RoomOffer;
using Domain.Repositories.Interfaces;
using MediatR;

namespace Application.CQRS.RoomOffer.Command
{
    public sealed record AssigneOfferCommand(AssigneOfferDto model) : IRequest<ResponseViewModel<AssigneOfferDto>>;

    public class AssigneOfferCommandHandler : IRequestHandler<AssigneOfferCommand, ResponseViewModel<AssigneOfferDto>>
    {
        private readonly IRepository<Domain.Entities.RoomManagement.RoomOffer> _repository;

        public AssigneOfferCommandHandler(IRepository<Domain.Entities.RoomManagement.RoomOffer> repository)
        {
            _repository = repository;
        }

        public async Task<ResponseViewModel<AssigneOfferDto>> Handle(AssigneOfferCommand request, CancellationToken cancellationToken)
        {

            var roomOffer = request.model.Map<Domain.Entities.RoomManagement.RoomOffer>();

            var addedEntity = await _repository.AddAsync(roomOffer, cancellationToken);
            
            var isSaved = await _repository.SaveChangesAsync(cancellationToken);

            if (!isSaved)
            {
                return ResponseViewModel<AssigneOfferDto>.Failure(Enum.ErrorCode.AssigneOfferFail, "Failed to assign offer to the room.");
            }

            return ResponseViewModel<AssigneOfferDto>.Success(addedEntity.Map<AssigneOfferDto>(), "Offer assigned to room successfully.");
        }
    }
}
