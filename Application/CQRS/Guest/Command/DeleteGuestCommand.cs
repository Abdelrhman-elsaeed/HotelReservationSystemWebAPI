using Application.DTOS;
using Domain.Repositories.Interfaces;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.CQRS.Guest.Command
{
    public sealed record DeleteGuestCommand(int id) : IRequest<ResponseViewModel<bool>>;

    public class DeleteGuestCommandHandler : IRequestHandler<DeleteGuestCommand, ResponseViewModel<bool>>
    {
        private readonly IRepository<Domain.Entities.Guest.Guest> _repository;

        public DeleteGuestCommandHandler(IRepository<Domain.Entities.Guest.Guest> repository)
        {
            _repository = repository;
        }

        public async Task<ResponseViewModel<bool>> Handle(DeleteGuestCommand request, CancellationToken cancellationToken)
        {
            var guestEntity = await _repository.GetByIDAsync(request.id, cancellationToken);

            if (guestEntity == null)
            {
                return ResponseViewModel<bool>.Failure(Enum.ErrorCode.GuestNotFound, message: "Guest not found");
            }

            _repository.SoftDelete(guestEntity);

            var isSaved = await _repository.SaveChangesAsync(cancellationToken);

            if (!isSaved)
            {
                return ResponseViewModel<bool>.Failure(Enum.ErrorCode.DeleteGuestFail, message: "Delete guest failed");
            }

            return ResponseViewModel<bool>.Success(true, message: "Guest deleted successfully");
        }
    }
}
