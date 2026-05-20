using Application.AutoMapper.Profiles;
using Application.DTOS;
using Application.DTOS.Offer;
using Domain.Repositories.Interfaces;
using MediatR;

namespace Application.CQRS.Offer.Command
{
    public sealed record AddOfferCommand(AddOfferDto model) : IRequest<ResponseViewModel<AddOfferDto>>;

    public class AddOfferCommandHandler : IRequestHandler<AddOfferCommand, ResponseViewModel<AddOfferDto>>
    {
        private readonly IRepository<Domain.Entities.Offer> _Repository;

        public AddOfferCommandHandler(IRepository<Domain.Entities.Offer> Repository)
        {
            _Repository = Repository;
        }

        public async Task<ResponseViewModel<AddOfferDto>> Handle(AddOfferCommand request, CancellationToken cancellationToken)
        {
            var OfferEntity = request.model.Map<Domain.Entities.Offer>();

            var SavedOffer = await _Repository.AddAsync(OfferEntity, cancellationToken);
            
            var isSaved = await _Repository.SaveChangesAsync(cancellationToken);

            if (!isSaved)
            {
                return ResponseViewModel<AddOfferDto>.Failure(Enum.ErrorCode.AddOfferFail,"Failed to add the new offer.");
            }
            return ResponseViewModel<AddOfferDto>.Success(SavedOffer.Map<AddOfferDto>(), "Offer added successfully.");
        }
    }
}
