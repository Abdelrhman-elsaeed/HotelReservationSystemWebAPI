using Application.AutoMapper.Profiles;
using Application.CQRS.Room.Command;
using Application.CQRS.Room.Queries;
using Application.CQRS.RoomType.Queries;
using Application.DTOS;
using Application.DTOS.Room;
using Application.DTOS.RoomType;
using AutoMapper;
using Domain.Entities.RoomManagement;
using Domain.Repositories.Interfaces;
using FluentAssertions;
using MediatR;
using Moq;
using NUnit.Framework;

namespace Hotel.UnitTests
{
    [TestFixture]
    public class Room_Test
    {
        private Mock<IRepository<Domain.Entities.RoomManagement.Room>> _roomRepositoryMock = null!;
        private Mock<IRepository<RoomType>> _roomTypeRepositoryMock = null!;
        private Mock<IRoomRepository> _extendedRoomRepositoryMock = null!;
        private Mock<IMediator> _mediatorMock = null!;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            var mockMapper = new Mock<IMapper>();

            mockMapper.Setup(m => m.Map<Domain.Entities.RoomManagement.Room>(It.IsAny<object>()))
                .Returns((object src) =>
                {
                    if (src is AddRoomDetailsDto addDto)
                        return new Domain.Entities.RoomManagement.Room { RoomNumber = addDto.RoomNumber, Description = addDto.Description, RoomTypeId = addDto.RoomTypeId };
                    if (src is UpdateRoomDetailsDto updateDto)
                        return new Domain.Entities.RoomManagement.Room { ID = updateDto.ID, RoomNumber = updateDto.RoomNumber, Description = updateDto.Description, RoomTypeId = updateDto.RoomTypeId };
                    return new Domain.Entities.RoomManagement.Room();
                });

            mockMapper.Setup(m => m.Map<AddRoomDetailsDto>(It.IsAny<Domain.Entities.RoomManagement.Room>()))
                .Returns((Domain.Entities.RoomManagement.Room src) => new AddRoomDetailsDto { ID = src.ID, RoomNumber = src.RoomNumber, Description = src.Description, RoomTypeId = src.RoomTypeId });

            mockMapper.Setup(m => m.Map<GetRoomDto>(It.IsAny<Domain.Entities.RoomManagement.Room>()))
                .Returns((Domain.Entities.RoomManagement.Room src) => new GetRoomDto { ID = src.ID, RoomNumber = src.RoomNumber, Description = src.Description, RoomTypeId = src.RoomTypeId });

            mockMapper.Setup(m => m.Map<GetRoomTypeDto>(It.IsAny<RoomType>()))
                .Returns((RoomType src) => new GetRoomTypeDto { ID = src.ID, Name = src.Name, Price = src.Price });

            AutoMapperHelper.Mapper = mockMapper.Object;
        }

        [SetUp]
        public void SetUp()
        {
            _roomRepositoryMock = new Mock<IRepository<Domain.Entities.RoomManagement.Room>>();
            _roomTypeRepositoryMock = new Mock<IRepository<RoomType>>();
            _extendedRoomRepositoryMock = new Mock<IRoomRepository>();
            _mediatorMock = new Mock<IMediator>();
        }

        #region AddRoomDetailsCommand

        [Test]
        [Category("Happy")]
        public async Task AddRoomDetailsCommand_Success_ReturnsSuccessResponse()
        {
            // Arrange
            var addDto = new AddRoomDetailsDto { RoomNumber = "101", Description = "Ocean view", RoomTypeId = 1 };
            var command = new AddRoomDetailsCommand(addDto);
            var handler = new AddRoomDetailsCommandHandler(_roomRepositoryMock.Object);

            var roomEntity = new Domain.Entities.RoomManagement.Room { ID = 1, RoomNumber = "101", Description = "Ocean view", RoomTypeId = 1 };

            _roomRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Domain.Entities.RoomManagement.Room>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(roomEntity);

            _roomRepositoryMock.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Message.Should().Be("Room details add successfully");
            result.Data.Should().NotBeNull();

            _roomRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Domain.Entities.RoomManagement.Room>(), It.IsAny<CancellationToken>()), Times.Once);
            _roomRepositoryMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        [Category("Business")]
        public async Task AddRoomDetailsCommand_NullModel_ReturnsFailureResponse()
        {
            // Arrange
            var command = new AddRoomDetailsCommand(null!);
            var handler = new AddRoomDetailsCommandHandler(_roomRepositoryMock.Object);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Message.Should().Be("Room Details cannot be Null.");

            _roomRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Domain.Entities.RoomManagement.Room>(), It.IsAny<CancellationToken>()), Times.Never);
            _roomRepositoryMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test]
        [Category("Business")]
        public async Task AddRoomDetailsCommand_FailToSave_ReturnsFailureResponse()
        {
            // Arrange
            var addDto = new AddRoomDetailsDto { RoomNumber = "101", Description = "Ocean view", RoomTypeId = 1 };
            var command = new AddRoomDetailsCommand(addDto);
            var handler = new AddRoomDetailsCommandHandler(_roomRepositoryMock.Object);

            var roomEntity = new Domain.Entities.RoomManagement.Room { ID = 1, RoomNumber = "101", Description = "Ocean view", RoomTypeId = 1 };

            _roomRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Domain.Entities.RoomManagement.Room>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(roomEntity);

            _roomRepositoryMock.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Message.Should().Be("Fail to add room details!");

            _roomRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Domain.Entities.RoomManagement.Room>(), It.IsAny<CancellationToken>()), Times.Once);
            _roomRepositoryMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        #endregion

        #region UpdateRoomDetailsCommand

        [Test]
        [Category("Happy")]
        public async Task UpdateRoomDetailsCommand_Success_ReturnsSuccessResponse()
        {
            // Arrange
            var updateDto = new UpdateRoomDetailsDto { ID = 1, RoomNumber = "102", Description = "Mountain view", RoomTypeId = 2 };
            var command = new UpdateRoomDetailsCommand(updateDto);
            var handler = new UpdateRoomDetailsCommandHandler(_roomRepositoryMock.Object, _mediatorMock.Object);

            _roomRepositoryMock.Setup(r => r.CheckExistsByIDAsync(updateDto.ID, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            _mediatorMock.Setup(m => m.Send(It.IsAny<CheckRoomTypeExistQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(ResponseViewModel<bool>.Success(true, "Room Type Is Exist"));

            _roomRepositoryMock.Setup(r => r.UpdateInclude(It.IsAny<Domain.Entities.RoomManagement.Room>(),
                nameof(Domain.Entities.RoomManagement.Room.RoomNumber),
                nameof(Domain.Entities.RoomManagement.Room.Description),
                nameof(Domain.Entities.RoomManagement.Room.RoomTypeId)));

            _roomRepositoryMock.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Message.Should().Be("Room details updated successfully");

            _roomRepositoryMock.Verify(x => x.CheckExistsByIDAsync(updateDto.ID, It.IsAny<CancellationToken>()), Times.Once);
            _mediatorMock.Verify(x => x.Send(It.IsAny<CheckRoomTypeExistQuery>(), It.IsAny<CancellationToken>()), Times.Once);
            _roomRepositoryMock.Verify(x => x.UpdateInclude(It.IsAny<Domain.Entities.RoomManagement.Room>(),
                nameof(Domain.Entities.RoomManagement.Room.RoomNumber),
                nameof(Domain.Entities.RoomManagement.Room.Description),
                nameof(Domain.Entities.RoomManagement.Room.RoomTypeId)), Times.Once);
            _roomRepositoryMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        [Category("Business")]
        public async Task UpdateRoomDetailsCommand_RoomNotFound_ReturnsFailureResponse()
        {
            // Arrange
            var updateDto = new UpdateRoomDetailsDto { ID = 99, RoomNumber = "102", Description = "Mountain view", RoomTypeId = 2 };
            var command = new UpdateRoomDetailsCommand(updateDto);
            var handler = new UpdateRoomDetailsCommandHandler(_roomRepositoryMock.Object, _mediatorMock.Object);

            _roomRepositoryMock.Setup(r => r.CheckExistsByIDAsync(updateDto.ID, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Message.Should().Be("Room not found");

            _roomRepositoryMock.Verify(x => x.CheckExistsByIDAsync(updateDto.ID, It.IsAny<CancellationToken>()), Times.Once);
            _mediatorMock.Verify(x => x.Send(It.IsAny<CheckRoomTypeExistQuery>(), It.IsAny<CancellationToken>()), Times.Never);
            _roomRepositoryMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test]
        [Category("Business")]
        public async Task UpdateRoomDetailsCommand_RoomTypeNotFound_ReturnsFailureResponse()
        {
            // Arrange
            var updateDto = new UpdateRoomDetailsDto { ID = 1, RoomNumber = "102", Description = "Mountain view", RoomTypeId = 99 };
            var command = new UpdateRoomDetailsCommand(updateDto);
            var handler = new UpdateRoomDetailsCommandHandler(_roomRepositoryMock.Object, _mediatorMock.Object);

            _roomRepositoryMock.Setup(r => r.CheckExistsByIDAsync(updateDto.ID, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            _mediatorMock.Setup(m => m.Send(It.IsAny<CheckRoomTypeExistQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(ResponseViewModel<bool>.Failure(Application.Enum.ErrorCode.RoomTypeNotExist, "Room type not exist!"));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Message.Should().Be("Room type not exist!");

            _mediatorMock.Verify(x => x.Send(It.IsAny<CheckRoomTypeExistQuery>(), It.IsAny<CancellationToken>()), Times.Once);
            _roomRepositoryMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test]
        [Category("Business")]
        public async Task UpdateRoomDetailsCommand_FailToSave_ReturnsFailureResponse()
        {
            // Arrange
            var updateDto = new UpdateRoomDetailsDto { ID = 1, RoomNumber = "102", Description = "Mountain view", RoomTypeId = 2 };
            var command = new UpdateRoomDetailsCommand(updateDto);
            var handler = new UpdateRoomDetailsCommandHandler(_roomRepositoryMock.Object, _mediatorMock.Object);

            _roomRepositoryMock.Setup(r => r.CheckExistsByIDAsync(updateDto.ID, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            _mediatorMock.Setup(m => m.Send(It.IsAny<CheckRoomTypeExistQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(ResponseViewModel<bool>.Success(true, "Room Type Is Exist"));

            _roomRepositoryMock.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Message.Should().Be("Room details fail to update!");

            _roomRepositoryMock.Verify(x => x.UpdateInclude(It.IsAny<Domain.Entities.RoomManagement.Room>(),
                nameof(Domain.Entities.RoomManagement.Room.RoomNumber),
                nameof(Domain.Entities.RoomManagement.Room.Description),
                nameof(Domain.Entities.RoomManagement.Room.RoomTypeId)), Times.Once);
            _roomRepositoryMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        #endregion

        #region DeleteRoomDetailsCommand

        [Test]
        [Category("Happy")]
        public async Task DeleteRoomDetailsCommand_Success_ReturnsSuccessResponse()
        {
            // Arrange
            int roomId = 1;
            var command = new DeleteRoomDetailsCommand(roomId);
            var handler = new DeleteRoomDetailsCommandHandler(_extendedRoomRepositoryMock.Object);

            var roomEntity = new Domain.Entities.RoomManagement.Room { ID = roomId, RoomNumber = "101", Description = "Ocean view", RoomTypeId = 1 };

            _extendedRoomRepositoryMock.Setup(r => r.GetByIDAsync(roomId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(roomEntity);

            _extendedRoomRepositoryMock.Setup(r => r.SoftDelete(It.IsAny<Domain.Entities.RoomManagement.Room>()));

            _extendedRoomRepositoryMock.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Message.Should().Be("Room details deleted successfully");

            _extendedRoomRepositoryMock.Verify(x => x.GetByIDAsync(roomId, It.IsAny<CancellationToken>()), Times.Once);
            _extendedRoomRepositoryMock.Verify(x => x.SoftDelete(It.IsAny<Domain.Entities.RoomManagement.Room>()), Times.Once);
            _extendedRoomRepositoryMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        [Category("Business")]
        public async Task DeleteRoomDetailsCommand_NotFound_ReturnsFailureResponse()
        {
            // Arrange
            int roomId = 99;
            var command = new DeleteRoomDetailsCommand(roomId);
            var handler = new DeleteRoomDetailsCommandHandler(_extendedRoomRepositoryMock.Object);

            _extendedRoomRepositoryMock.Setup(r => r.GetByIDAsync(roomId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Domain.Entities.RoomManagement.Room?)null);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Message.Should().Be("Room not found!");

            _extendedRoomRepositoryMock.Verify(x => x.GetByIDAsync(roomId, It.IsAny<CancellationToken>()), Times.Once);
            _extendedRoomRepositoryMock.Verify(x => x.SoftDelete(It.IsAny<Domain.Entities.RoomManagement.Room>()), Times.Never);
            _extendedRoomRepositoryMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test]
        [Category("Business")]
        public async Task DeleteRoomDetailsCommand_FailToSave_ReturnsFailureResponse()
        {
            // Arrange
            int roomId = 1;
            var command = new DeleteRoomDetailsCommand(roomId);
            var handler = new DeleteRoomDetailsCommandHandler(_extendedRoomRepositoryMock.Object);

            var roomEntity = new Domain.Entities.RoomManagement.Room { ID = roomId, RoomNumber = "101", Description = "Ocean view", RoomTypeId = 1 };

            _extendedRoomRepositoryMock.Setup(r => r.GetByIDAsync(roomId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(roomEntity);

            _extendedRoomRepositoryMock.Setup(r => r.SoftDelete(It.IsAny<Domain.Entities.RoomManagement.Room>()));

            _extendedRoomRepositoryMock.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Message.Should().Be("Fail to delete room details");

            _extendedRoomRepositoryMock.Verify(x => x.GetByIDAsync(roomId, It.IsAny<CancellationToken>()), Times.Once);
            _extendedRoomRepositoryMock.Verify(x => x.SoftDelete(It.IsAny<Domain.Entities.RoomManagement.Room>()), Times.Once);
            _extendedRoomRepositoryMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        #endregion

        #region GetRoomTypeQuery

        [Test]
        [Category("Happy")]
        public async Task GetRoomTypeQuery_Success_ReturnsSuccessResponse()
        {
            // Arrange
            int roomTypeId = 1;
            var query = new GetRoomTypeQuery(roomTypeId);
            var handler = new GetRoomTypeQueryHandler(_roomTypeRepositoryMock.Object);

            var roomTypeEntity = new RoomType { ID = roomTypeId, Name = "Suite", Price = 500 };

            _roomTypeRepositoryMock.Setup(r => r.GetByIDAsync(roomTypeId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(roomTypeEntity);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Message.Should().Be("Room type retrieved successfully");
            result.Data.Should().NotBeNull();
            result.Data!.ID.Should().Be(roomTypeId);
            result.Data.Name.Should().Be("Suite");

            _roomTypeRepositoryMock.Verify(x => x.GetByIDAsync(roomTypeId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        [Category("Business")]
        public async Task GetRoomTypeQuery_NotFound_ReturnsFailureResponse()
        {
            // Arrange
            int roomTypeId = 99;
            var query = new GetRoomTypeQuery(roomTypeId);
            var handler = new GetRoomTypeQueryHandler(_roomTypeRepositoryMock.Object);

            _roomTypeRepositoryMock.Setup(r => r.GetByIDAsync(roomTypeId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((RoomType?)null);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Message.Should().Be("Room Type not found!");

            _roomTypeRepositoryMock.Verify(x => x.GetByIDAsync(roomTypeId, It.IsAny<CancellationToken>()), Times.Once);
        }

        #endregion

        #region IsRoomExistQuery

        [Test]
        [Category("Happy")]
        public async Task IsRoomExistQuery_Exists_ReturnsTrue()
        {
            // Arrange
            int roomId = 1;
            var query = new IsRoomExistQuery(roomId);
            var handler = new IsRoomExistQueryHandler(_roomRepositoryMock.Object);

            _roomRepositoryMock.Setup(r => r.CheckExistsByIDAsync(roomId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().BeTrue();

            _roomRepositoryMock.Verify(x => x.CheckExistsByIDAsync(roomId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        [Category("Business")]
        public async Task IsRoomExistQuery_NotExist_ReturnsFalse()
        {
            // Arrange
            int roomId = 99;
            var query = new IsRoomExistQuery(roomId);
            var handler = new IsRoomExistQueryHandler(_roomRepositoryMock.Object);

            _roomRepositoryMock.Setup(r => r.CheckExistsByIDAsync(roomId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().BeFalse();

            _roomRepositoryMock.Verify(x => x.CheckExistsByIDAsync(roomId, It.IsAny<CancellationToken>()), Times.Once);
        }

        #endregion

        #region GetRoomTotalPriceQuery

        [Test]
        [Category("Happy")]
        public async Task GetRoomTotalPriceQuery_Success_ReturnsSuccessResponse()
        {
            // Arrange
            int roomId = 1;
            var query = new GetRoomTotalPriceQuery(roomId);
            var handler = new GetRoomTotalPriceQueryHandler(_extendedRoomRepositoryMock.Object);

            _extendedRoomRepositoryMock.Setup(r => r.CheckExistsByIDAsync(roomId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            _extendedRoomRepositoryMock.Setup(r => r.GetRoomTotalPriceAsync(roomId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(750.00m);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Message.Should().Be("Total price retrieved successfully");
            result.Data.Should().Be(750.00m);

            _extendedRoomRepositoryMock.Verify(x => x.CheckExistsByIDAsync(roomId, It.IsAny<CancellationToken>()), Times.Once);
            _extendedRoomRepositoryMock.Verify(x => x.GetRoomTotalPriceAsync(roomId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        [Category("Business")]
        public async Task GetRoomTotalPriceQuery_RoomNotFound_ReturnsFailureResponse()
        {
            // Arrange
            int roomId = 99;
            var query = new GetRoomTotalPriceQuery(roomId);
            var handler = new GetRoomTotalPriceQueryHandler(_extendedRoomRepositoryMock.Object);

            _extendedRoomRepositoryMock.Setup(r => r.CheckExistsByIDAsync(roomId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Message.Should().Be("Room not found!");

            _extendedRoomRepositoryMock.Verify(x => x.CheckExistsByIDAsync(roomId, It.IsAny<CancellationToken>()), Times.Once);
            _extendedRoomRepositoryMock.Verify(x => x.GetRoomTotalPriceAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test]
        [Category("Business")]
        public async Task GetRoomTotalPriceQuery_PriceNull_ReturnsFailureResponse()
        {
            // Arrange
            int roomId = 1;
            var query = new GetRoomTotalPriceQuery(roomId);
            var handler = new GetRoomTotalPriceQueryHandler(_extendedRoomRepositoryMock.Object);

            _extendedRoomRepositoryMock.Setup(r => r.CheckExistsByIDAsync(roomId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            _extendedRoomRepositoryMock.Setup(r => r.GetRoomTotalPriceAsync(roomId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((decimal?)null);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Message.Should().Be("Fail to get room total price!");

            _extendedRoomRepositoryMock.Verify(x => x.CheckExistsByIDAsync(roomId, It.IsAny<CancellationToken>()), Times.Once);
            _extendedRoomRepositoryMock.Verify(x => x.GetRoomTotalPriceAsync(roomId, It.IsAny<CancellationToken>()), Times.Once);
        }

        #endregion
    }
}
