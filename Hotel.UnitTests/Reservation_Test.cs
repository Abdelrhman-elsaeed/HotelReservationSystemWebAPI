using Application.AutoMapper.Profiles;
using Application.CQRS.Reservation.Command;
using Application.CQRS.Reservation.Queries;
using Application.DTOS.Reservation;
using AutoMapper;
using Domain.Entities.ReservationManagement;
using Domain.Enum;
using Domain.Repositories.Interfaces;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace Hotel.UnitTests
{
    [TestFixture]
    public class Reservation_Test
    {
        private Mock<IRepository<Reservation>> _reservationRepositoryMock = null!;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            var mockMapper = new Mock<IMapper>();

            mockMapper.Setup(m => m.Map<GetReservationDetailsDto>(It.IsAny<Reservation>()))
                .Returns((Reservation src) => new GetReservationDetailsDto
                {
                    ReservationId = src.ID,
                    GuestId = src.GuestId,
                    Status = src.Status.ToString(),
                    SpecialRequest = src.SpecialRequest,
                    TotalAmount = src.TotalAmount
                });

            AutoMapperHelper.Mapper = mockMapper.Object;
        }

        [SetUp]
        public void SetUp()
        {
            _reservationRepositoryMock = new Mock<IRepository<Reservation>>();
        }

        #region CancelReservationCommand

        [Test]
        [Category("Happy")]
        public async Task CancelReservationCommand_Success_ReturnsSuccessResponse()
        {
            // Arrange
            int reservationId = 1;
            var command = new CancelReservationCommand(reservationId);
            var handler = new CancelReservationCommandHandler(_reservationRepositoryMock.Object);

            var reservationEntity = new Reservation
            {
                ID = reservationId,
                GuestId = 10,
                Status = ReservationStatus.Pending,
                TotalAmount = 1500
            };

            _reservationRepositoryMock.Setup(r => r.GetByIDAsync(reservationId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(reservationEntity);

            _reservationRepositoryMock.Setup(r => r.UpdateInclude(It.IsAny<Reservation>(), nameof(Reservation.Status)));

            _reservationRepositoryMock.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Message.Should().Be("Reservation cancelled successfully.");
            reservationEntity.Status.Should().Be(ReservationStatus.Cancelled);

            _reservationRepositoryMock.Verify(x => x.GetByIDAsync(reservationId, It.IsAny<CancellationToken>()), Times.Once);
            _reservationRepositoryMock.Verify(x => x.UpdateInclude(It.IsAny<Reservation>(), nameof(Reservation.Status)), Times.Once);
            _reservationRepositoryMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        [Category("Business")]
        public async Task CancelReservationCommand_NotFound_ReturnsFailureResponse()
        {
            // Arrange
            int reservationId = 99;
            var command = new CancelReservationCommand(reservationId);
            var handler = new CancelReservationCommandHandler(_reservationRepositoryMock.Object);

            _reservationRepositoryMock.Setup(r => r.GetByIDAsync(reservationId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Reservation?)null);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Message.Should().Be("Reservation not found.");

            _reservationRepositoryMock.Verify(x => x.GetByIDAsync(reservationId, It.IsAny<CancellationToken>()), Times.Once);
            _reservationRepositoryMock.Verify(x => x.UpdateInclude(It.IsAny<Reservation>(), It.IsAny<string[]>()), Times.Never);
            _reservationRepositoryMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test]
        [Category("Business")]
        public async Task CancelReservationCommand_AlreadyCancelled_ReturnsFailureResponse()
        {
            // Arrange
            int reservationId = 1;
            var command = new CancelReservationCommand(reservationId);
            var handler = new CancelReservationCommandHandler(_reservationRepositoryMock.Object);

            var reservationEntity = new Reservation
            {
                ID = reservationId,
                GuestId = 10,
                Status = ReservationStatus.Cancelled,
                TotalAmount = 1500
            };

            _reservationRepositoryMock.Setup(r => r.GetByIDAsync(reservationId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(reservationEntity);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Message.Should().Be("Reservation is already cancelled.");

            _reservationRepositoryMock.Verify(x => x.GetByIDAsync(reservationId, It.IsAny<CancellationToken>()), Times.Once);
            _reservationRepositoryMock.Verify(x => x.UpdateInclude(It.IsAny<Reservation>(), It.IsAny<string[]>()), Times.Never);
            _reservationRepositoryMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test]
        [Category("Business")]
        public async Task CancelReservationCommand_FailToSave_ReturnsFailureResponse()
        {
            // Arrange
            int reservationId = 1;
            var command = new CancelReservationCommand(reservationId);
            var handler = new CancelReservationCommandHandler(_reservationRepositoryMock.Object);

            var reservationEntity = new Reservation
            {
                ID = reservationId,
                GuestId = 10,
                Status = ReservationStatus.Pending,
                TotalAmount = 1500
            };

            _reservationRepositoryMock.Setup(r => r.GetByIDAsync(reservationId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(reservationEntity);

            _reservationRepositoryMock.Setup(r => r.UpdateInclude(It.IsAny<Reservation>(), nameof(Reservation.Status)));

            _reservationRepositoryMock.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Message.Should().Be("Failed to cancel the reservation.");

            _reservationRepositoryMock.Verify(x => x.GetByIDAsync(reservationId, It.IsAny<CancellationToken>()), Times.Once);
            _reservationRepositoryMock.Verify(x => x.UpdateInclude(It.IsAny<Reservation>(), nameof(Reservation.Status)), Times.Once);
            _reservationRepositoryMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        #endregion

        #region UpdateReservationDetailsCommand

        [Test]
        [Category("Happy")]
        public async Task UpdateReservationDetailsCommand_Success_ReturnsSuccessResponse()
        {
            // Arrange
            int reservationId = 1;
            string specialRequest = "Sea view room please";
            var command = new UpdateReservationDetailsCommand(reservationId, specialRequest);
            var handler = new UpdateReservationDetailsCommandHandler(_reservationRepositoryMock.Object);

            var reservationEntity = new Reservation
            {
                ID = reservationId,
                GuestId = 10,
                Status = ReservationStatus.Pending,
                SpecialRequest = "Old request",
                TotalAmount = 1500
            };

            _reservationRepositoryMock.Setup(r => r.GetByIDAsync(reservationId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(reservationEntity);

            _reservationRepositoryMock.Setup(r => r.UpdateInclude(It.IsAny<Reservation>(), nameof(Reservation.SpecialRequest)));

            _reservationRepositoryMock.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Message.Should().Be("Reservation details updated successfully");
            reservationEntity.SpecialRequest.Should().Be(specialRequest);

            _reservationRepositoryMock.Verify(x => x.GetByIDAsync(reservationId, It.IsAny<CancellationToken>()), Times.Once);
            _reservationRepositoryMock.Verify(x => x.UpdateInclude(It.IsAny<Reservation>(), nameof(Reservation.SpecialRequest)), Times.Once);
            _reservationRepositoryMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        [Category("Business")]
        public async Task UpdateReservationDetailsCommand_NotFound_ReturnsFailureResponse()
        {
            // Arrange
            int reservationId = 99;
            var command = new UpdateReservationDetailsCommand(reservationId, "Some request");
            var handler = new UpdateReservationDetailsCommandHandler(_reservationRepositoryMock.Object);

            _reservationRepositoryMock.Setup(r => r.GetByIDAsync(reservationId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Reservation?)null);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Message.Should().Be("Reservation not found");

            _reservationRepositoryMock.Verify(x => x.GetByIDAsync(reservationId, It.IsAny<CancellationToken>()), Times.Once);
            _reservationRepositoryMock.Verify(x => x.UpdateInclude(It.IsAny<Reservation>(), It.IsAny<string[]>()), Times.Never);
            _reservationRepositoryMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test]
        [Category("Business")]
        public async Task UpdateReservationDetailsCommand_FailToSave_ReturnsFailureResponse()
        {
            // Arrange
            int reservationId = 1;
            var command = new UpdateReservationDetailsCommand(reservationId, "Sea view room please");
            var handler = new UpdateReservationDetailsCommandHandler(_reservationRepositoryMock.Object);

            var reservationEntity = new Reservation
            {
                ID = reservationId,
                GuestId = 10,
                Status = ReservationStatus.Pending,
                TotalAmount = 1500
            };

            _reservationRepositoryMock.Setup(r => r.GetByIDAsync(reservationId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(reservationEntity);

            _reservationRepositoryMock.Setup(r => r.UpdateInclude(It.IsAny<Reservation>(), nameof(Reservation.SpecialRequest)));

            _reservationRepositoryMock.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Message.Should().Be("Failed to update reservation details");

            _reservationRepositoryMock.Verify(x => x.GetByIDAsync(reservationId, It.IsAny<CancellationToken>()), Times.Once);
            _reservationRepositoryMock.Verify(x => x.UpdateInclude(It.IsAny<Reservation>(), nameof(Reservation.SpecialRequest)), Times.Once);
            _reservationRepositoryMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        #endregion

        #region GetReservationByIdQuery

        [Test]
        [Category("Happy")]
        public async Task GetReservationByIdQuery_Success_ReturnsSuccessResponse()
        {
            // Arrange
            int reservationId = 1;
            var query = new GetReservationByIdQuery(reservationId);
            var handler = new GetReservationByIdQueryHandler(_reservationRepositoryMock.Object);

            var reservationEntity = new Reservation
            {
                ID = reservationId,
                GuestId = 10,
                Status = ReservationStatus.Pending,
                SpecialRequest = "Late checkout",
                TotalAmount = 2000
            };

            _reservationRepositoryMock.Setup(r => r.GetByIDAsync(reservationId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(reservationEntity);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Message.Should().Be("Reservation retrieved successfully");
            result.Data.Should().NotBeNull();
            result.Data!.ReservationId.Should().Be(reservationId);
            result.Data.GuestId.Should().Be(10);
            result.Data.Status.Should().Be(ReservationStatus.Pending.ToString());

            _reservationRepositoryMock.Verify(x => x.GetByIDAsync(reservationId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        [Category("Business")]
        public async Task GetReservationByIdQuery_NotFound_ReturnsFailureResponse()
        {
            // Arrange
            int reservationId = 99;
            var query = new GetReservationByIdQuery(reservationId);
            var handler = new GetReservationByIdQueryHandler(_reservationRepositoryMock.Object);

            _reservationRepositoryMock.Setup(r => r.GetByIDAsync(reservationId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Reservation?)null);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Message.Should().Be("Reservation not found");

            _reservationRepositoryMock.Verify(x => x.GetByIDAsync(reservationId, It.IsAny<CancellationToken>()), Times.Once);
        }

        #endregion
    }
}
