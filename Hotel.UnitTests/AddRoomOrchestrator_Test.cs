using Application.CQRS.Facility.Command;
using Application.CQRS.Room.Command;
using Application.CQRS.Room.Orchestrators;
using Application.CQRS.RoomFacility.Command;
using Application.CQRS.RoomPicture.Command;
using Application.CQRS.RoomType.Command;
using Application.DTOS;
using Application.DTOS.Facility;
using Application.DTOS.Room;
using Application.DTOS.RoomFacility;
using Application.DTOS.RoomPicture;
using Application.DTOS.RoomType;
using FluentAssertions;
using HotelReservationSystem.API.Helper.BusinessExceptions;
using MediatR;
using Moq;
using NUnit.Framework;

namespace Hotel.UnitTests
{
    [TestFixture]
    public class AddRoomOrchestrator_Test
    {
        private Mock<IMediator> _mediatorMock = null!;

        private AddRoomTypeDto _typeDto = null!;
        private AddRoomDetailsDto _detailsDto = null!;
        private AddFacilityDto _facilityDto = null!;
        private List<FileUploadDto> _pictures = null!;

        [SetUp]
        public void SetUp()
        {
            _mediatorMock = new Mock<IMediator>();

            _typeDto = new AddRoomTypeDto { Name = "Suite", Price = 500 };
            _detailsDto = new AddRoomDetailsDto { RoomNumber = "101", Description = "Sea view", RoomTypeId = 0 };
            _facilityDto = new AddFacilityDto { Name = "Pool", Price = 50 };
            _pictures = new List<FileUploadDto>
            {
                new FileUploadDto { Content = new MemoryStream(new byte[] { 1, 2, 3 }), FileName = "room.jpg", ContentType = "image/jpeg" }
            };
        }

        #region AddRoomOrchestrator

        [Test]
        [Category("Happy")]
        public async Task AddRoomOrchestrator_AllStepsSucceed_ReturnsSuccessResponse()
        {
            // Arrange
            var command = new AddRoomOrchestrator(_typeDto, _detailsDto, _facilityDto, _pictures);
            var handler = new AddRoomOrchestratorHandler(_mediatorMock.Object);

            var roomTypeResult = ResponseViewModel<AddRoomTypeDto>.Success(new AddRoomTypeDto { ID = 10, Name = "Suite", Price = 500 }, "Room Type Added Successfully");
            var roomDetailsResult = ResponseViewModel<AddRoomDetailsDto>.Success(new AddRoomDetailsDto { ID = 20, RoomNumber = "101", Description = "Sea view", RoomTypeId = 10 }, "Room details add successfully");
            var facilityResult = ResponseViewModel<AddFacilityDto>.Success(new AddFacilityDto { ID = 30, Name = "Pool", Price = 50 }, "facility added successfully");
            var assignFacilityResult = ResponseViewModel<AssignFacilityToRoomDto>.Success(new AssignFacilityToRoomDto { RoomId = 20, FacilityId = 30 }, "Facility assigned to room successfully");
            var uploadPicturesResult = ResponseViewModel<bool>.Success(true, "Pictures uploaded successfully");

            _mediatorMock.Setup(m => m.Send(It.IsAny<AddRoomTypeCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(roomTypeResult);

            _mediatorMock.Setup(m => m.Send(It.IsAny<AddRoomDetailsCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(roomDetailsResult);

            _mediatorMock.Setup(m => m.Send(It.IsAny<AddFacilityCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(facilityResult);

            _mediatorMock.Setup(m => m.Send(It.IsAny<AssignFacilityToRoomCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(assignFacilityResult);

            _mediatorMock.Setup(m => m.Send(It.IsAny<UploadRoomPicturesCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(uploadPicturesResult);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Message.Should().Be("Room and all related details added successfully.");

            _mediatorMock.Verify(x => x.Send(It.IsAny<AddRoomTypeCommand>(), It.IsAny<CancellationToken>()), Times.Once);
            _mediatorMock.Verify(x => x.Send(It.IsAny<AddRoomDetailsCommand>(), It.IsAny<CancellationToken>()), Times.Once);
            _mediatorMock.Verify(x => x.Send(It.IsAny<AddFacilityCommand>(), It.IsAny<CancellationToken>()), Times.Once);
            _mediatorMock.Verify(x => x.Send(It.IsAny<AssignFacilityToRoomCommand>(), It.IsAny<CancellationToken>()), Times.Once);
            _mediatorMock.Verify(x => x.Send(It.IsAny<UploadRoomPicturesCommand>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        [Category("Happy")]
        public async Task AddRoomOrchestrator_NoPictures_SkipsPictureUpload_ReturnsSuccessResponse()
        {
            // Arrange
            var command = new AddRoomOrchestrator(_typeDto, _detailsDto, _facilityDto, new List<FileUploadDto>());
            var handler = new AddRoomOrchestratorHandler(_mediatorMock.Object);

            _mediatorMock.Setup(m => m.Send(It.IsAny<AddRoomTypeCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(ResponseViewModel<AddRoomTypeDto>.Success(new AddRoomTypeDto { ID = 10, Name = "Suite", Price = 500 }, "Room Type Added Successfully"));

            _mediatorMock.Setup(m => m.Send(It.IsAny<AddRoomDetailsCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(ResponseViewModel<AddRoomDetailsDto>.Success(new AddRoomDetailsDto { ID = 20, RoomNumber = "101", RoomTypeId = 10 }, "Room details add successfully"));

            _mediatorMock.Setup(m => m.Send(It.IsAny<AddFacilityCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(ResponseViewModel<AddFacilityDto>.Success(new AddFacilityDto { ID = 30, Name = "Pool" }, "facility added successfully"));

            _mediatorMock.Setup(m => m.Send(It.IsAny<AssignFacilityToRoomCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(ResponseViewModel<AssignFacilityToRoomDto>.Success(new AssignFacilityToRoomDto(), "Facility assigned to room successfully"));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Message.Should().Be("Room and all related details added successfully.");

            // Picture upload should be skipped when list is empty
            _mediatorMock.Verify(x => x.Send(It.IsAny<UploadRoomPicturesCommand>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test]
        [Category("Business")]
        public async Task AddRoomOrchestrator_AddRoomTypeFails_ThrowsBusinessException()
        {
            // Arrange
            var command = new AddRoomOrchestrator(_typeDto, _detailsDto, _facilityDto, _pictures);
            var handler = new AddRoomOrchestratorHandler(_mediatorMock.Object);

            _mediatorMock.Setup(m => m.Send(It.IsAny<AddRoomTypeCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(ResponseViewModel<AddRoomTypeDto>.Failure(Application.Enum.ErrorCode.AddRoomTypeFail, "Failed to save the Room Type to the database."));

            // Act
            var act = async () => await handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<BusinessException>()
                .WithMessage("Failed to save the Room Type to the database.");

            // All subsequent steps must not be called
            _mediatorMock.Verify(x => x.Send(It.IsAny<AddRoomDetailsCommand>(), It.IsAny<CancellationToken>()), Times.Never);
            _mediatorMock.Verify(x => x.Send(It.IsAny<AddFacilityCommand>(), It.IsAny<CancellationToken>()), Times.Never);
            _mediatorMock.Verify(x => x.Send(It.IsAny<AssignFacilityToRoomCommand>(), It.IsAny<CancellationToken>()), Times.Never);
            _mediatorMock.Verify(x => x.Send(It.IsAny<UploadRoomPicturesCommand>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test]
        [Category("Business")]
        public async Task AddRoomOrchestrator_AddRoomDetailsFails_ThrowsBusinessException()
        {
            // Arrange
            var command = new AddRoomOrchestrator(_typeDto, _detailsDto, _facilityDto, _pictures);
            var handler = new AddRoomOrchestratorHandler(_mediatorMock.Object);

            _mediatorMock.Setup(m => m.Send(It.IsAny<AddRoomTypeCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(ResponseViewModel<AddRoomTypeDto>.Success(new AddRoomTypeDto { ID = 10, Name = "Suite", Price = 500 }, "Room Type Added Successfully"));

            _mediatorMock.Setup(m => m.Send(It.IsAny<AddRoomDetailsCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(ResponseViewModel<AddRoomDetailsDto>.Failure(Application.Enum.ErrorCode.AddRoomDetailsFail, "Fail to add room details!"));

            // Act
            var act = async () => await handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<BusinessException>()
                .WithMessage("Fail to add room details!");

            _mediatorMock.Verify(x => x.Send(It.IsAny<AddRoomTypeCommand>(), It.IsAny<CancellationToken>()), Times.Once);
            _mediatorMock.Verify(x => x.Send(It.IsAny<AddRoomDetailsCommand>(), It.IsAny<CancellationToken>()), Times.Once);
            _mediatorMock.Verify(x => x.Send(It.IsAny<AddFacilityCommand>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test]
        [Category("Business")]
        public async Task AddRoomOrchestrator_AddFacilityFails_ThrowsBusinessException()
        {
            // Arrange
            var command = new AddRoomOrchestrator(_typeDto, _detailsDto, _facilityDto, _pictures);
            var handler = new AddRoomOrchestratorHandler(_mediatorMock.Object);

            _mediatorMock.Setup(m => m.Send(It.IsAny<AddRoomTypeCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(ResponseViewModel<AddRoomTypeDto>.Success(new AddRoomTypeDto { ID = 10 }, "Room Type Added Successfully"));

            _mediatorMock.Setup(m => m.Send(It.IsAny<AddRoomDetailsCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(ResponseViewModel<AddRoomDetailsDto>.Success(new AddRoomDetailsDto { ID = 20 }, "Room details add successfully"));

            _mediatorMock.Setup(m => m.Send(It.IsAny<AddFacilityCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(ResponseViewModel<AddFacilityDto>.Failure(Application.Enum.ErrorCode.UnExpectedError, "Fail to add facility!"));

            // Act
            var act = async () => await handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<BusinessException>()
                .WithMessage("Fail to add facility!");

            _mediatorMock.Verify(x => x.Send(It.IsAny<AssignFacilityToRoomCommand>(), It.IsAny<CancellationToken>()), Times.Never);
            _mediatorMock.Verify(x => x.Send(It.IsAny<UploadRoomPicturesCommand>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test]
        [Category("Business")]
        public async Task AddRoomOrchestrator_AssignFacilityFails_ThrowsBusinessException()
        {
            // Arrange
            var command = new AddRoomOrchestrator(_typeDto, _detailsDto, _facilityDto, _pictures);
            var handler = new AddRoomOrchestratorHandler(_mediatorMock.Object);

            _mediatorMock.Setup(m => m.Send(It.IsAny<AddRoomTypeCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(ResponseViewModel<AddRoomTypeDto>.Success(new AddRoomTypeDto { ID = 10 }, "Room Type Added Successfully"));

            _mediatorMock.Setup(m => m.Send(It.IsAny<AddRoomDetailsCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(ResponseViewModel<AddRoomDetailsDto>.Success(new AddRoomDetailsDto { ID = 20 }, "Room details add successfully"));

            _mediatorMock.Setup(m => m.Send(It.IsAny<AddFacilityCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(ResponseViewModel<AddFacilityDto>.Success(new AddFacilityDto { ID = 30 }, "facility added successfully"));

            _mediatorMock.Setup(m => m.Send(It.IsAny<AssignFacilityToRoomCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(ResponseViewModel<AssignFacilityToRoomDto>.Failure(Application.Enum.ErrorCode.AssignFacilityToRoomFail, "Fail to assign facility to this room"));

            // Act
            var act = async () => await handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<BusinessException>()
                .WithMessage("Fail to assign facility to this room");

            _mediatorMock.Verify(x => x.Send(It.IsAny<UploadRoomPicturesCommand>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test]
        [Category("Business")]
        public async Task AddRoomOrchestrator_UploadPicturesFails_ThrowsBusinessException()
        {
            // Arrange
            var command = new AddRoomOrchestrator(_typeDto, _detailsDto, _facilityDto, _pictures);
            var handler = new AddRoomOrchestratorHandler(_mediatorMock.Object);

            _mediatorMock.Setup(m => m.Send(It.IsAny<AddRoomTypeCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(ResponseViewModel<AddRoomTypeDto>.Success(new AddRoomTypeDto { ID = 10 }, "Room Type Added Successfully"));

            _mediatorMock.Setup(m => m.Send(It.IsAny<AddRoomDetailsCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(ResponseViewModel<AddRoomDetailsDto>.Success(new AddRoomDetailsDto { ID = 20 }, "Room details add successfully"));

            _mediatorMock.Setup(m => m.Send(It.IsAny<AddFacilityCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(ResponseViewModel<AddFacilityDto>.Success(new AddFacilityDto { ID = 30 }, "facility added successfully"));

            _mediatorMock.Setup(m => m.Send(It.IsAny<AssignFacilityToRoomCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(ResponseViewModel<AssignFacilityToRoomDto>.Success(new AssignFacilityToRoomDto(), "Facility assigned to room successfully"));

            _mediatorMock.Setup(m => m.Send(It.IsAny<UploadRoomPicturesCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(ResponseViewModel<bool>.Failure(Application.Enum.ErrorCode.UnExpectedError, "Failed to upload pictures."));

            // Act
            var act = async () => await handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<BusinessException>()
                .WithMessage("Failed to upload pictures.");
        }

        [Test]
        [Category("Business")]
        public async Task AddRoomOrchestrator_RoomTypeIdPropagatedToRoomDetails()
        {
            // Arrange — verifies data flow: roomTypeResult.Data.ID must be written to detailsDto.RoomTypeId
            var command = new AddRoomOrchestrator(_typeDto, _detailsDto, _facilityDto, new List<FileUploadDto>());
            var handler = new AddRoomOrchestratorHandler(_mediatorMock.Object);

            int capturedRoomTypeId = -1;

            _mediatorMock.Setup(m => m.Send(It.IsAny<AddRoomTypeCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(ResponseViewModel<AddRoomTypeDto>.Success(new AddRoomTypeDto { ID = 42, Name = "Suite", Price = 500 }, "Room Type Added Successfully"));

            _mediatorMock.Setup(m => m.Send(It.IsAny<AddRoomDetailsCommand>(), It.IsAny<CancellationToken>()))
                .Callback<IRequest<ResponseViewModel<AddRoomDetailsDto>>, CancellationToken>((req, _) =>
                {
                    capturedRoomTypeId = ((AddRoomDetailsCommand)req).model.RoomTypeId;
                })
                .ReturnsAsync(ResponseViewModel<AddRoomDetailsDto>.Success(new AddRoomDetailsDto { ID = 20 }, "Room details add successfully"));

            _mediatorMock.Setup(m => m.Send(It.IsAny<AddFacilityCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(ResponseViewModel<AddFacilityDto>.Success(new AddFacilityDto { ID = 30 }, "facility added successfully"));

            _mediatorMock.Setup(m => m.Send(It.IsAny<AssignFacilityToRoomCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(ResponseViewModel<AssignFacilityToRoomDto>.Success(new AssignFacilityToRoomDto(), "assigned"));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            capturedRoomTypeId.Should().Be(42, "the orchestrator must propagate the new RoomType ID to the room details step");
        }

        #endregion
    }
}
