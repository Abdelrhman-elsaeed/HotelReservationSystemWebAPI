using Application.AutoMapper.Profiles;
using Application.DTOS;
using Application.DTOS.RoomType;
using Domain.Repositories.Interfaces;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.CQRS.RoomType.Command
{
        public sealed record UpdateRoomTypeCommand(UpdateRoomTypeDto model) : IRequest<ResponseViewModel<UpdateRoomTypeDto>>;

        public class UpdateRoomTypeCommandHandler : IRequestHandler<UpdateRoomTypeCommand, ResponseViewModel<UpdateRoomTypeDto>>
        {
            private readonly IRepository<Domain.Entities.RoomManagement.RoomType> _repository;

            public UpdateRoomTypeCommandHandler(IRepository<Domain.Entities.RoomManagement.RoomType> repository)
            {
                _repository = repository;
            }

            public async Task<ResponseViewModel<UpdateRoomTypeDto>> Handle(UpdateRoomTypeCommand request, CancellationToken cancellationToken)
            {
                // Check if room type exists
                var isExist = await _repository.CheckExistsByIDAsync(request.model.ID, cancellationToken);

                if (!isExist)
                {
                    return ResponseViewModel<UpdateRoomTypeDto>.Failure(Enum.ErrorCode.RoomTypeNotExist, message: "Room type not found");
                }

                var roomTypeEntity = request.model.Map<Domain.Entities.RoomManagement.RoomType>();

                _repository.UpdateInclude(roomTypeEntity, 
                    nameof(roomTypeEntity.Name), 
                    nameof(roomTypeEntity.Price));

                var isSaved = await _repository.SaveChangesAsync(cancellationToken);

                if (!isSaved)
                {
                    return ResponseViewModel<UpdateRoomTypeDto>.Failure(Enum.ErrorCode.UpdateRoomTypeFail, message: "Update room type failed");
                }

                return ResponseViewModel<UpdateRoomTypeDto>.Success(roomTypeEntity.Map<UpdateRoomTypeDto>(), message: "Room type updated successfully");
            }
        }
}
