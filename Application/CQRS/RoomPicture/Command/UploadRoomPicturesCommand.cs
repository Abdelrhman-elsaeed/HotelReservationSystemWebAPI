using Application.DTOS;
using Application.DTOS.RoomPicture;
using Domain.Entities.RoomManagement;
using Domain.Helper.Services;
using Domain.Repositories.Interfaces;
using HotelReservationSystem.API.Helper.BusinessExceptions;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.CQRS.RoomPicture.Command
{
    public sealed record UploadRoomPicturesCommand(int RoomId, List<FileUploadDto> Pictures) : IRequest<ResponseViewModel<bool>>;

    public class UploadRoomPicturesCommandHandler : IRequestHandler<UploadRoomPicturesCommand, ResponseViewModel<bool>>
    {
        private readonly IRepository<Domain.Entities.RoomManagement.RoomPicture> _Repository;
        private readonly IFileHandlingService _FileService;
        public UploadRoomPicturesCommandHandler(IRepository<Domain.Entities.RoomManagement.RoomPicture> Repository, IFileHandlingService FileService)
        {
            _Repository = Repository;
            _FileService = FileService;
        }

        public async Task<ResponseViewModel<bool>> Handle(UploadRoomPicturesCommand request, CancellationToken cancellationToken)
        {
            if (request.Pictures == null || !request.Pictures.Any())
            {
                return ResponseViewModel<bool>.Failure(Enum.ErrorCode.NoImageUploaded, "You have to upload 1 image at least!");
            }

            // have to check if the room exist

            var uploadedFilesTracking = new List<string>();

            try
            {
                foreach (var fileDto in request.Pictures)
                {
                    var fileName = await _FileService.UploadFileAsync(fileDto.Content, fileDto.FileName, "RoomImages");

                    if (!string.IsNullOrEmpty(fileName))
                    {
                        uploadedFilesTracking.Add(fileName); 

                        var roomPictureEntity = new Domain.Entities.RoomManagement.RoomPicture()
                        {
                            RoomId = request.RoomId,
                            PictureUrl = fileName 
                        };

                        await _Repository.AddAsync(roomPictureEntity, cancellationToken);
                    }
                }

                var isSaved = await _Repository.SaveChangesAsync(cancellationToken);

                // Scenario 1: The save operation returns false (no rows impacted)
                if (!isSaved)
                {
                    RollbackPhysicalFiles(uploadedFilesTracking);
                    throw new BusinessException(Enum.ErrorCode.UnExpectedError, "Failed to save images in the database!"); 
                }

                return ResponseViewModel<bool>.Success(true, message: "Images uploaded successfully");
            }
            catch (Exception ex)
            {
                // Scenario 2: Any database errors or system-level exceptions
                RollbackPhysicalFiles(uploadedFilesTracking); 
                throw; 
            }
        }

        private void RollbackPhysicalFiles(List<string> fileNames)
        {
            foreach (var fileName in fileNames)
            {
                _FileService.DeleteFile(fileName, "RoomImages");
            }
        }
    }
}
    