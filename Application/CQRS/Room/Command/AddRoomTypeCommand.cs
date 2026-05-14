using Application.AutoMapper.Profiles;
using Application.DTOS;
using Application.DTOS.Room;
using Domain.Entities.RoomManagement;
using Domain.Repositories.Interfaces;
using MediatR;

namespace Application.CQRS.Room.Command
{
    // Request
    public sealed record AddRoomTypeCommand(AddRoomTypeDto model) : IRequest<ResponseViewModel<AddRoomTypeDto>>;

    // Request Handler
    public class AddRoomTypeCommandHandler : IRequestHandler<AddRoomTypeCommand, ResponseViewModel<AddRoomTypeDto>>
    {
        private readonly IRepository<RoomType> _repository;
        public AddRoomTypeCommandHandler(IRepository<RoomType> repository)
        {
            _repository = repository;
        }

        public async Task<ResponseViewModel<AddRoomTypeDto>> Handle(AddRoomTypeCommand request, CancellationToken cancellationToken)
        {

            //validate is this type exist before 
            var IsExistBefore = await _repository.CheckExistsByConditionAsync(x => x.Name == request.model.Name, cancellationToken);
            if (IsExistBefore)
            {
                return ResponseViewModel<AddRoomTypeDto>.Failure(Enum.ErrorCode.RoomTypeIsExist, $"Room Type '{request.model.Name}' already exists.");
            }

            var entity = request.model.Map<RoomType>();

            var addedEntity = await _repository.AddAsync(entity, cancellationToken);
            var isSaved = await _repository.SaveChangesAsync(cancellationToken);

            if (!isSaved)
            {
                return ResponseViewModel<AddRoomTypeDto>.Failure(Enum.ErrorCode.AddRoomTypeFail,"Failed to save the Room Type to the database.");
            }

            var responseDto = addedEntity.Map<AddRoomTypeDto>();

            return ResponseViewModel<AddRoomTypeDto>.Success(responseDto, message: "Room Type Added Successfully");
        }
    }
}
