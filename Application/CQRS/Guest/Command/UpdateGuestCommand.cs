using Application.AutoMapper.Profiles;
using Application.DTOS;
using Application.DTOS.Guest;
using Domain.Repositories.Interfaces;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.CQRS.Guest.Command
{
    public sealed record UpdateGuestCommand(UpdateGuestDto model) : IRequest<ResponseViewModel<UpdateGuestDto>>;

    public class UpdateGuestCommandHandler : IRequestHandler<UpdateGuestCommand, ResponseViewModel<UpdateGuestDto>>
    {
        private readonly IRepository<Domain.Entities.Guest.Guest> _repository;

        public UpdateGuestCommandHandler(IRepository<Domain.Entities.Guest.Guest> repository)
        {
            _repository = repository;
        }

        public async Task<ResponseViewModel<UpdateGuestDto>> Handle(UpdateGuestCommand request, CancellationToken cancellationToken)
        {
            var isExist = await _repository.CheckExistsByIDAsync(request.model.ID, cancellationToken);

            if (!isExist)
            {
                return ResponseViewModel<UpdateGuestDto>.Failure(Enum.ErrorCode.UpdateGuestFail, message: "Guest not found");
            }

            var guestEntity = request.model.Map<Domain.Entities.Guest.Guest>();

            _repository.UpdateInclude(guestEntity, 
                nameof(guestEntity.FullName), 
                nameof(guestEntity.NationalId), 
                nameof(guestEntity.MobileNumber));

            var isSaved = await _repository.SaveChangesAsync(cancellationToken);

            if (!isSaved)
            {
                return ResponseViewModel<UpdateGuestDto>.Failure(Enum.ErrorCode.UpdateGuestFail, message: "Update guest failed");
            }

            return ResponseViewModel<UpdateGuestDto>.Success(guestEntity.Map<UpdateGuestDto>(), message: "Guest updated successfully");
        }
    }
}
