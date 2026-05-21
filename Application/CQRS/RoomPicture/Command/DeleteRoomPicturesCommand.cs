using Application.DTOS;
using Application.Enum;
using Domain.Helper.Services;
using Domain.Repositories.Interfaces;
using MediatR;

namespace Application.CQRS.RoomPicture.Command
{
    public sealed record DeleteRoomPicturesCommand(int RoomId) : IRequest<ResponseViewModel<bool>>;

    public class DeleteRoomPicturesCommandHandler : IRequestHandler<DeleteRoomPicturesCommand, ResponseViewModel<bool>>
    {
        private readonly IRepository<Domain.Entities.RoomManagement.RoomPicture> _repository;
        private readonly IFileHandlingService _fileService;

        public DeleteRoomPicturesCommandHandler(
            IRepository<Domain.Entities.RoomManagement.RoomPicture> repository,
            IFileHandlingService fileService)
        {
            _repository = repository;
            _fileService = fileService;
        }

        public async Task<ResponseViewModel<bool>> Handle(DeleteRoomPicturesCommand request, CancellationToken cancellationToken)
        {
            var roomPictures = await _repository
                .GetAllByConditionAsync(x => x.RoomId == request.RoomId, cancellationToken);

            if (!roomPictures.Any())
                return ResponseViewModel<bool>.Failure(ErrorCode.RoomPictureNotExist, "No images found for this room.");

            _repository.DeleteRange(roomPictures);

            var isSaved = await _repository.SaveChangesAsync(cancellationToken);
            if (!isSaved)
                return ResponseViewModel<bool>.Failure(ErrorCode.DeleteRoomPicturesFail, "Failed to delete room pictures.");

            foreach (var picture in roomPictures)
            {
                _fileService.DeleteFile(picture.PictureUrl, "RoomImages");
            }

            return ResponseViewModel<bool>.Success(true, "Room pictures deleted successfully.");
        }
    }
}
