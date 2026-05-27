using Application.CQRS.Room.Command;
using Application.CQRS.Room.Orchestrators;
using Application.CQRS.RoomFacility.Command;
using Application.CQRS.RoomPicture.Command;
using Application.DTOS;
using FluentAssertions;
using HotelReservationSystem.API.Helper.BusinessExceptions;
using MediatR;
using Moq;
using NUnit.Framework;

namespace Hotel.UnitTests
{
    [TestFixture]
    public class DeleteRoomOrchestrator_Test
    {
        private Mock<IMediator> _mediatorMock = null!;

        [SetUp]
        public void SetUp()
        {
            _mediatorMock = new Mock<IMediator>();
        }

        #region DeleteRoomOrchestrator

        [Test]
        [Category("Happy")]
        public async Task DeleteRoomOrchestrator_AllStepsSucceed_ReturnsSuccessResponse()
        {
            // Arrange
            int roomId = 1;
            var command = new DeleteRoomOrchestrator(roomId);
            var handler = new DeleteRoomOrchestratorHandler(_mediatorMock.Object);

            _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteRoomPicturesCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(ResponseViewModel<bool>.Success(true, "Pictures deleted successfully"));

            _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteAllFacilitiesOfRoomCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(ResponseViewModel<bool>.Success(true, "Room facilities deleted successfully."));

            _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteRoomDetailsCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(ResponseViewModel<bool>.Success(true, "Room details deleted successfully"));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Message.Should().Be("Room and all related details deleted successfully.");

            _mediatorMock.Verify(x => x.Send(It.IsAny<DeleteRoomPicturesCommand>(), It.IsAny<CancellationToken>()), Times.Once);
            _mediatorMock.Verify(x => x.Send(It.IsAny<DeleteAllFacilitiesOfRoomCommand>(), It.IsAny<CancellationToken>()), Times.Once);
            _mediatorMock.Verify(x => x.Send(It.IsAny<DeleteRoomDetailsCommand>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        [Category("Business")]
        public async Task DeleteRoomOrchestrator_DeletePicturesFails_ThrowsBusinessException()
        {
            // Arrange
            int roomId = 1;
            var command = new DeleteRoomOrchestrator(roomId);
            var handler = new DeleteRoomOrchestratorHandler(_mediatorMock.Object);

            _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteRoomPicturesCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(ResponseViewModel<bool>.Failure(Application.Enum.ErrorCode.DeleteRoomPicturesFail, "Failed to delete room pictures."));

            // Act
            var act = async () => await handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<BusinessException>()
                .WithMessage("Failed to delete room pictures.");

            // Subsequent steps must not run
            _mediatorMock.Verify(x => x.Send(It.IsAny<DeleteAllFacilitiesOfRoomCommand>(), It.IsAny<CancellationToken>()), Times.Never);
            _mediatorMock.Verify(x => x.Send(It.IsAny<DeleteRoomDetailsCommand>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test]
        [Category("Business")]
        public async Task DeleteRoomOrchestrator_DeleteFacilitiesFails_ThrowsBusinessException()
        {
            // Arrange
            int roomId = 1;
            var command = new DeleteRoomOrchestrator(roomId);
            var handler = new DeleteRoomOrchestratorHandler(_mediatorMock.Object);

            _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteRoomPicturesCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(ResponseViewModel<bool>.Success(true, "Pictures deleted successfully"));

            _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteAllFacilitiesOfRoomCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(ResponseViewModel<bool>.Failure(Application.Enum.ErrorCode.DeleteAllFacilitiesOfRoomFail, "Failed to delete room facilities."));

            // Act
            var act = async () => await handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<BusinessException>()
                .WithMessage("Failed to delete room facilities.");

            _mediatorMock.Verify(x => x.Send(It.IsAny<DeleteRoomPicturesCommand>(), It.IsAny<CancellationToken>()), Times.Once);
            _mediatorMock.Verify(x => x.Send(It.IsAny<DeleteAllFacilitiesOfRoomCommand>(), It.IsAny<CancellationToken>()), Times.Once);
            _mediatorMock.Verify(x => x.Send(It.IsAny<DeleteRoomDetailsCommand>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test]
        [Category("Business")]
        public async Task DeleteRoomOrchestrator_DeleteRoomDetailsFails_ThrowsBusinessException()
        {
            // Arrange
            int roomId = 1;
            var command = new DeleteRoomOrchestrator(roomId);
            var handler = new DeleteRoomOrchestratorHandler(_mediatorMock.Object);

            _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteRoomPicturesCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(ResponseViewModel<bool>.Success(true, "Pictures deleted successfully"));

            _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteAllFacilitiesOfRoomCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(ResponseViewModel<bool>.Success(true, "Room facilities deleted successfully."));

            _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteRoomDetailsCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(ResponseViewModel<bool>.Failure(Application.Enum.ErrorCode.DeleteRoomDetailsFail, "Fail to delete room details"));

            // Act
            var act = async () => await handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<BusinessException>()
                .WithMessage("Fail to delete room details");

            _mediatorMock.Verify(x => x.Send(It.IsAny<DeleteRoomPicturesCommand>(), It.IsAny<CancellationToken>()), Times.Once);
            _mediatorMock.Verify(x => x.Send(It.IsAny<DeleteAllFacilitiesOfRoomCommand>(), It.IsAny<CancellationToken>()), Times.Once);
            _mediatorMock.Verify(x => x.Send(It.IsAny<DeleteRoomDetailsCommand>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        #endregion
    }
}
