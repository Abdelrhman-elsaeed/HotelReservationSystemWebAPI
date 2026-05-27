using Application.CQRS.Reservation.Command;
using Application.CQRS.Reservation.Queries;
using Application.CQRS.ReservationRoom.Command;
using Application.CQRS.ReservationRoom.Orchestrators;
using Application.DTOS;
using Application.DTOS.Receipt;
using Application.DTOS.Reservation;
using Application.DTOS.RoomReservation;
using Domain.Enum;
using FluentAssertions;
using HotelReservationSystem.API.Helper.BusinessExceptions;
using MediatR;
using Moq;
using NUnit.Framework;

namespace Hotel.UnitTests
{
    [TestFixture]
    public class UpdateReservationOrchestrator_Test
    {
        private Mock<IMediator> _mediatorMock = null!;

        private UpdateReservationDto _updateDto = null!;

        [SetUp]
        public void SetUp()
        {
            _mediatorMock = new Mock<IMediator>();

            _updateDto = new UpdateReservationDto
            {
                ID = 1,
                SpecialRequest = "Early check-in please",
                Rooms = new List<AddReservationRoomDto>
                {
                    new AddReservationRoomDto
                    {
                        RoomId = 10,
                        CheckInDate = DateTime.Today.AddDays(1),
                        CheckOutDate = DateTime.Today.AddDays(3),
                        RoomGuestIds = new List<int> { 5 }
                    }
                }
            };
        }

        #region UpdateReservationOrchestratorCommand

        [Test]
        [Category("Happy")]
        public async Task UpdateReservationOrchestratorCommand_AllStepsSucceed_ReturnsSuccessResponse()
        {
            // Arrange
            var command = new UpdateReservationOrchestratorCommand(_updateDto);
            var handler = new UpdateReservationOrchestratorCommandHandler(_mediatorMock.Object);

            var reservationDetailsDto = new GetReservationDetailsDto
            {
                ReservationId = 1,
                GuestId = 5,
                Status = ReservationStatus.Pending.ToString(),
                SpecialRequest = "Old request",
                TotalAmount = 1000,
                CreatedAt = DateTime.Today.AddDays(-5)
            };

            var updatedRoomsReceipt = new UpdatedRoomsReceiptDto
            {
                NewTotalAmount = 1500,
                Rooms = new List<RoomReceiptDto>
                {
                    new RoomReceiptDto { RoomId = 10, CheckInDate = DateTime.Today.AddDays(1), CheckOutDate = DateTime.Today.AddDays(3), TotalNights = 2, PricePerNight = 750, RoomTotal = 1500 }
                }
            };

            _mediatorMock.Setup(m => m.Send(It.IsAny<GetReservationByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(ResponseViewModel<GetReservationDetailsDto>.Success(reservationDetailsDto, "Reservation retrieved successfully"));

            _mediatorMock.Setup(m => m.Send(It.IsAny<UpdateReservationDetailsCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(ResponseViewModel<bool>.Success(true, "Reservation details updated successfully"));

            _mediatorMock.Setup(m => m.Send(It.IsAny<UpdateReservationRoomsCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(ResponseViewModel<UpdatedRoomsReceiptDto>.Success(updatedRoomsReceipt, "Rooms updated successfully"));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Message.Should().Be("Reservation updated successfully");
            result.Data.Should().NotBeNull();
            result.Data!.TotalAmount.Should().Be(1500);
            result.Data.SpecialRequest.Should().Be("Early check-in please");

            _mediatorMock.Verify(x => x.Send(It.IsAny<GetReservationByIdQuery>(), It.IsAny<CancellationToken>()), Times.Once);
            _mediatorMock.Verify(x => x.Send(It.IsAny<UpdateReservationDetailsCommand>(), It.IsAny<CancellationToken>()), Times.Once);
            _mediatorMock.Verify(x => x.Send(It.IsAny<UpdateReservationRoomsCommand>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        [Category("Business")]
        public async Task UpdateReservationOrchestratorCommand_ReservationNotFound_ThrowsBusinessException()
        {
            // Arrange
            var command = new UpdateReservationOrchestratorCommand(_updateDto);
            var handler = new UpdateReservationOrchestratorCommandHandler(_mediatorMock.Object);

            _mediatorMock.Setup(m => m.Send(It.IsAny<GetReservationByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(ResponseViewModel<GetReservationDetailsDto>.Failure(Application.Enum.ErrorCode.ReservationNotFound, "Reservation not found"));

            // Act
            var act = async () => await handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<BusinessException>()
                .WithMessage("Reservation not found");

            _mediatorMock.Verify(x => x.Send(It.IsAny<UpdateReservationDetailsCommand>(), It.IsAny<CancellationToken>()), Times.Never);
            _mediatorMock.Verify(x => x.Send(It.IsAny<UpdateReservationRoomsCommand>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test]
        [Category("Business")]
        public async Task UpdateReservationOrchestratorCommand_CancelledReservation_ThrowsBusinessException()
        {
            // Arrange
            var command = new UpdateReservationOrchestratorCommand(_updateDto);
            var handler = new UpdateReservationOrchestratorCommandHandler(_mediatorMock.Object);

            var cancelledReservationDto = new GetReservationDetailsDto
            {
                ReservationId = 1,
                GuestId = 5,
                Status = ReservationStatus.Cancelled.ToString(),
                TotalAmount = 1000
            };

            _mediatorMock.Setup(m => m.Send(It.IsAny<GetReservationByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(ResponseViewModel<GetReservationDetailsDto>.Success(cancelledReservationDto, "Reservation retrieved successfully"));

            // Act
            var act = async () => await handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<BusinessException>()
                .WithMessage("Cannot update a cancelled or rejected reservation");

            _mediatorMock.Verify(x => x.Send(It.IsAny<UpdateReservationDetailsCommand>(), It.IsAny<CancellationToken>()), Times.Never);
            _mediatorMock.Verify(x => x.Send(It.IsAny<UpdateReservationRoomsCommand>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test]
        [Category("Business")]
        public async Task UpdateReservationOrchestratorCommand_RejectedReservation_ThrowsBusinessException()
        {
            // Arrange
            var command = new UpdateReservationOrchestratorCommand(_updateDto);
            var handler = new UpdateReservationOrchestratorCommandHandler(_mediatorMock.Object);

            var rejectedReservationDto = new GetReservationDetailsDto
            {
                ReservationId = 1,
                GuestId = 5,
                Status = ReservationStatus.Rejected.ToString(),
                TotalAmount = 1000
            };

            _mediatorMock.Setup(m => m.Send(It.IsAny<GetReservationByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(ResponseViewModel<GetReservationDetailsDto>.Success(rejectedReservationDto, "Reservation retrieved successfully"));

            // Act
            var act = async () => await handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<BusinessException>()
                .WithMessage("Cannot update a cancelled or rejected reservation");

            _mediatorMock.Verify(x => x.Send(It.IsAny<UpdateReservationDetailsCommand>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test]
        [Category("Business")]
        public async Task UpdateReservationOrchestratorCommand_UpdateDetailsFails_ThrowsBusinessException()
        {
            // Arrange
            var command = new UpdateReservationOrchestratorCommand(_updateDto);
            var handler = new UpdateReservationOrchestratorCommandHandler(_mediatorMock.Object);

            var reservationDetailsDto = new GetReservationDetailsDto
            {
                ReservationId = 1,
                GuestId = 5,
                Status = ReservationStatus.Pending.ToString(),
                TotalAmount = 1000
            };

            _mediatorMock.Setup(m => m.Send(It.IsAny<GetReservationByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(ResponseViewModel<GetReservationDetailsDto>.Success(reservationDetailsDto, "Reservation retrieved successfully"));

            _mediatorMock.Setup(m => m.Send(It.IsAny<UpdateReservationDetailsCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(ResponseViewModel<bool>.Failure(Application.Enum.ErrorCode.UpdateReservationFail, "Failed to update reservation details"));

            // Act
            var act = async () => await handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<BusinessException>()
                .WithMessage("Failed to update reservation details");

            _mediatorMock.Verify(x => x.Send(It.IsAny<GetReservationByIdQuery>(), It.IsAny<CancellationToken>()), Times.Once);
            _mediatorMock.Verify(x => x.Send(It.IsAny<UpdateReservationDetailsCommand>(), It.IsAny<CancellationToken>()), Times.Once);
            _mediatorMock.Verify(x => x.Send(It.IsAny<UpdateReservationRoomsCommand>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test]
        [Category("Business")]
        public async Task UpdateReservationOrchestratorCommand_UpdateRoomsFails_ThrowsBusinessException()
        {
            // Arrange
            var command = new UpdateReservationOrchestratorCommand(_updateDto);
            var handler = new UpdateReservationOrchestratorCommandHandler(_mediatorMock.Object);

            var reservationDetailsDto = new GetReservationDetailsDto
            {
                ReservationId = 1,
                GuestId = 5,
                Status = ReservationStatus.Pending.ToString(),
                TotalAmount = 1000
            };

            _mediatorMock.Setup(m => m.Send(It.IsAny<GetReservationByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(ResponseViewModel<GetReservationDetailsDto>.Success(reservationDetailsDto, "Reservation retrieved successfully"));

            _mediatorMock.Setup(m => m.Send(It.IsAny<UpdateReservationDetailsCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(ResponseViewModel<bool>.Success(true, "Reservation details updated successfully"));

            _mediatorMock.Setup(m => m.Send(It.IsAny<UpdateReservationRoomsCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(ResponseViewModel<UpdatedRoomsReceiptDto>.Failure(Application.Enum.ErrorCode.UpdateReservationFail, "Failed to update reservation rooms"));

            // Act
            var act = async () => await handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<BusinessException>()
                .WithMessage("Failed to update reservation rooms");

            _mediatorMock.Verify(x => x.Send(It.IsAny<GetReservationByIdQuery>(), It.IsAny<CancellationToken>()), Times.Once);
            _mediatorMock.Verify(x => x.Send(It.IsAny<UpdateReservationDetailsCommand>(), It.IsAny<CancellationToken>()), Times.Once);
            _mediatorMock.Verify(x => x.Send(It.IsAny<UpdateReservationRoomsCommand>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        #endregion
    }
}
