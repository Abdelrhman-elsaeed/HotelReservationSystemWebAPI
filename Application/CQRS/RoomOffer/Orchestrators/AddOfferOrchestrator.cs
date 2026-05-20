using Application.AutoMapper.Profiles;
using Application.CQRS.Offer.Command;
using Application.CQRS.Room.Queries;
using Application.CQRS.RoomOffer.Command;
using Application.DTOS;
using Application.DTOS.Offer;
using Application.DTOS.RoomOffer;
using Application.ViewModel.Offer;
using HotelReservationSystem.API.Helper.BusinessExceptions;
using MediatR;

namespace Application.CQRS.RoomOffer.Orchestrators
{
    public sealed record AddOfferOrchestrator(AddOfferDto model, int RoomId) : IRequest<ResponseViewModel<AddOfferVM>>;

    public class AddOfferOrchestratorHandler : IRequestHandler<AddOfferOrchestrator, ResponseViewModel<AddOfferVM>>
    {
        private readonly IMediator _mediator;

        public AddOfferOrchestratorHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<ResponseViewModel<AddOfferVM>> Handle(AddOfferOrchestrator request, CancellationToken cancellationToken)
        {
            // 1. check room is exist
            var isRoomExistQuery = new IsRoomExistQuery(request.RoomId);
            var isRoomExist = await _mediator.Send(isRoomExistQuery, cancellationToken);
            
            if (!isRoomExist)
                throw new BusinessException(Enum.ErrorCode.RoomNotFound, "Room not found.");

            // 2. add offer
            var addOfferCommand = new AddOfferCommand(request.model);
            var offerResult = await _mediator.Send(addOfferCommand, cancellationToken);

            if (!offerResult.IsSuccess)
                throw new BusinessException(Enum.ErrorCode.AddOfferFail, offerResult.Message);

            // 3. assign offer
            var assignDto = new AssigneOfferDto
            {
                RoomId = request.RoomId,
                OfferId = offerResult.Data.ID
            };

            var assignCommand = new AssigneOfferCommand(assignDto);
            var assignResult = await _mediator.Send(assignCommand, cancellationToken);

            if (!assignResult.IsSuccess)
                throw new BusinessException(Enum.ErrorCode.AssigneOfferFail, assignResult.Message);

            // 4. Return success response mapping the added offer to the View Model
            var mappedOfferVM = offerResult.Data.Map<AddOfferVM>();
            
            return ResponseViewModel<AddOfferVM>.Success(mappedOfferVM, "Offer added and assigned to room successfully.");
        }
    }
}
