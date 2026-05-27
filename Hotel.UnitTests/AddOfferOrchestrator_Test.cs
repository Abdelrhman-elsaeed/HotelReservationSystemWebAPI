using Application.AutoMapper.Profiles;
using Application.CQRS.Offer.Command;
using Application.CQRS.Room.Queries;
using Application.CQRS.RoomOffer.Command;
using Application.CQRS.RoomOffer.Orchestrators;
using Application.DTOS;
using Application.DTOS.Offer;
using Application.DTOS.RoomOffer;
using Application.ViewModel.Offer;
using AutoMapper;
using FluentAssertions;
using HotelReservationSystem.API.Helper.BusinessExceptions;
using MediatR;
using Moq;
using NUnit.Framework;

namespace Hotel.UnitTests
{
    [TestFixture]
    public class AddOfferOrchestrator_Test
    {
        private Mock<IMediator> _mediatorMock = null!;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            var mockMapper = new Mock<IMapper>();

            mockMapper.Setup(m => m.Map<AddOfferVM>(It.IsAny<AddOfferDto>()))
                .Returns((AddOfferDto src) => new AddOfferVM
                {
                    ID = src.ID,
                    DiscountPercentage = src.DiscountPercentage,
                    StartDate = src.StartDate,
                    EndDate = src.EndDate
                });

            AutoMapperHelper.Mapper = mockMapper.Object;
        }

        [SetUp]
        public void SetUp()
        {
            _mediatorMock = new Mock<IMediator>();
        }

        #region AddOfferOrchestrator

        [Test]
        [Category("Happy")]
        public async Task AddOfferOrchestrator_AllStepsSucceed_ReturnsSuccessResponse()
        {
            // Arrange
            int roomId = 5;
            var addOfferDto = new AddOfferDto { DiscountPercentage = 20, StartDate = DateTime.Today, EndDate = DateTime.Today.AddDays(30) };
            var command = new AddOfferOrchestrator(addOfferDto, roomId);
            var handler = new AddOfferOrchestratorHandler(_mediatorMock.Object);

            var savedOfferDto = new AddOfferDto { ID = 100, DiscountPercentage = 20, StartDate = addOfferDto.StartDate, EndDate = addOfferDto.EndDate };

            _mediatorMock.Setup(m => m.Send(It.IsAny<IsRoomExistQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            _mediatorMock.Setup(m => m.Send(It.IsAny<AddOfferCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(ResponseViewModel<AddOfferDto>.Success(savedOfferDto, "Offer added successfully."));

            _mediatorMock.Setup(m => m.Send(It.IsAny<AssigneOfferCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(ResponseViewModel<AssigneOfferDto>.Success(new AssigneOfferDto { RoomId = roomId, OfferId = 100 }, "Offer assigned to room successfully."));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Message.Should().Be("Offer added and assigned to room successfully.");
            result.Data.Should().NotBeNull();
            result.Data!.DiscountPercentage.Should().Be(20);

            _mediatorMock.Verify(x => x.Send(It.IsAny<IsRoomExistQuery>(), It.IsAny<CancellationToken>()), Times.Once);
            _mediatorMock.Verify(x => x.Send(It.IsAny<AddOfferCommand>(), It.IsAny<CancellationToken>()), Times.Once);
            _mediatorMock.Verify(x => x.Send(It.IsAny<AssigneOfferCommand>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        [Category("Business")]
        public async Task AddOfferOrchestrator_RoomNotFound_ThrowsBusinessException()
        {
            // Arrange
            int roomId = 99;
            var addOfferDto = new AddOfferDto { DiscountPercentage = 20, StartDate = DateTime.Today, EndDate = DateTime.Today.AddDays(30) };
            var command = new AddOfferOrchestrator(addOfferDto, roomId);
            var handler = new AddOfferOrchestratorHandler(_mediatorMock.Object);

            _mediatorMock.Setup(m => m.Send(It.IsAny<IsRoomExistQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            var act = async () => await handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<BusinessException>()
                .WithMessage("Room not found.");

            _mediatorMock.Verify(x => x.Send(It.IsAny<AddOfferCommand>(), It.IsAny<CancellationToken>()), Times.Never);
            _mediatorMock.Verify(x => x.Send(It.IsAny<AssigneOfferCommand>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test]
        [Category("Business")]
        public async Task AddOfferOrchestrator_AddOfferFails_ThrowsBusinessException()
        {
            // Arrange
            int roomId = 5;
            var addOfferDto = new AddOfferDto { DiscountPercentage = 20, StartDate = DateTime.Today, EndDate = DateTime.Today.AddDays(30) };
            var command = new AddOfferOrchestrator(addOfferDto, roomId);
            var handler = new AddOfferOrchestratorHandler(_mediatorMock.Object);

            _mediatorMock.Setup(m => m.Send(It.IsAny<IsRoomExistQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            _mediatorMock.Setup(m => m.Send(It.IsAny<AddOfferCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(ResponseViewModel<AddOfferDto>.Failure(Application.Enum.ErrorCode.AddOfferFail, "Failed to add the new offer."));

            // Act
            var act = async () => await handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<BusinessException>()
                .WithMessage("Failed to add the new offer.");

            _mediatorMock.Verify(x => x.Send(It.IsAny<IsRoomExistQuery>(), It.IsAny<CancellationToken>()), Times.Once);
            _mediatorMock.Verify(x => x.Send(It.IsAny<AddOfferCommand>(), It.IsAny<CancellationToken>()), Times.Once);
            _mediatorMock.Verify(x => x.Send(It.IsAny<AssigneOfferCommand>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test]
        [Category("Business")]
        public async Task AddOfferOrchestrator_AssignOfferFails_ThrowsBusinessException()
        {
            // Arrange
            int roomId = 5;
            var addOfferDto = new AddOfferDto { DiscountPercentage = 20, StartDate = DateTime.Today, EndDate = DateTime.Today.AddDays(30) };
            var command = new AddOfferOrchestrator(addOfferDto, roomId);
            var handler = new AddOfferOrchestratorHandler(_mediatorMock.Object);

            var savedOfferDto = new AddOfferDto { ID = 100, DiscountPercentage = 20, StartDate = addOfferDto.StartDate, EndDate = addOfferDto.EndDate };

            _mediatorMock.Setup(m => m.Send(It.IsAny<IsRoomExistQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            _mediatorMock.Setup(m => m.Send(It.IsAny<AddOfferCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(ResponseViewModel<AddOfferDto>.Success(savedOfferDto, "Offer added successfully."));

            _mediatorMock.Setup(m => m.Send(It.IsAny<AssigneOfferCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(ResponseViewModel<AssigneOfferDto>.Failure(Application.Enum.ErrorCode.AssigneOfferFail, "Failed to assign offer to the room."));

            // Act
            var act = async () => await handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<BusinessException>()
                .WithMessage("Failed to assign offer to the room.");

            _mediatorMock.Verify(x => x.Send(It.IsAny<IsRoomExistQuery>(), It.IsAny<CancellationToken>()), Times.Once);
            _mediatorMock.Verify(x => x.Send(It.IsAny<AddOfferCommand>(), It.IsAny<CancellationToken>()), Times.Once);
            _mediatorMock.Verify(x => x.Send(It.IsAny<AssigneOfferCommand>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        [Category("Business")]
        public async Task AddOfferOrchestrator_OfferIdPropagatedToAssignStep()
        {
            // Arrange — verifies data flow: the offer ID from AddOffer must be passed to AssigneOfferCommand
            int roomId = 5;
            var addOfferDto = new AddOfferDto { DiscountPercentage = 20, StartDate = DateTime.Today, EndDate = DateTime.Today.AddDays(30) };
            var command = new AddOfferOrchestrator(addOfferDto, roomId);
            var handler = new AddOfferOrchestratorHandler(_mediatorMock.Object);

            int capturedOfferId = -1;
            int capturedRoomId = -1;

            _mediatorMock.Setup(m => m.Send(It.IsAny<IsRoomExistQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            _mediatorMock.Setup(m => m.Send(It.IsAny<AddOfferCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(ResponseViewModel<AddOfferDto>.Success(
                    new AddOfferDto { ID = 77, DiscountPercentage = 20, StartDate = addOfferDto.StartDate, EndDate = addOfferDto.EndDate },
                    "Offer added successfully."));

            _mediatorMock.Setup(m => m.Send(It.IsAny<AssigneOfferCommand>(), It.IsAny<CancellationToken>()))
                .Callback<IRequest<ResponseViewModel<AssigneOfferDto>>, CancellationToken>((req, _) =>
                {
                    capturedOfferId = ((AssigneOfferCommand)req).model.OfferId;
                    capturedRoomId = ((AssigneOfferCommand)req).model.RoomId;
                })
                .ReturnsAsync(ResponseViewModel<AssigneOfferDto>.Success(new AssigneOfferDto { RoomId = roomId, OfferId = 77 }, "Offer assigned to room successfully."));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            capturedOfferId.Should().Be(77, "the orchestrator must pass the created offer's ID to the assign step");
            capturedRoomId.Should().Be(roomId, "the orchestrator must pass the original room ID to the assign step");
        }

        #endregion
    }
}
