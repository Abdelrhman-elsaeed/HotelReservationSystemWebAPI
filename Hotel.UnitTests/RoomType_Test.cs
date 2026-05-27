using Application.AutoMapper.Profiles;
using Application.CQRS.RoomType.Command;
using Application.CQRS.RoomType.Queries;
using Application.DTOS.RoomType;
using AutoMapper;
using Domain.Entities.RoomManagement;
using Domain.Repositories.Interfaces;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using System;
using System.Linq.Expressions;

namespace Hotel.UnitTests
{
    [TestFixture]
    public class RoomType_Test
    {
        private Mock<IRepository<RoomType>> _roomTypeRepositoryMock = null!;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            var mockMapper = new Mock<IMapper>();

            mockMapper.Setup(m => m.Map<RoomType>(It.IsAny<object>()))
                .Returns((object src) =>
                {
                    if (src is AddRoomTypeDto addDto) return new RoomType { Name = addDto.Name, Price = addDto.Price };
                    if (src is UpdateRoomTypeDto updateDto) return new RoomType { ID = updateDto.ID, Name = updateDto.Name, Price = updateDto.Price };
                    return new RoomType();
                });

            mockMapper.Setup(m => m.Map<AddRoomTypeDto>(It.IsAny<RoomType>()))
                .Returns((RoomType src) => new AddRoomTypeDto { ID = src.ID, Name = src.Name, Price = src.Price });

            mockMapper.Setup(m => m.Map<UpdateRoomTypeDto>(It.IsAny<RoomType>()))
                .Returns((RoomType src) => new UpdateRoomTypeDto { ID = src.ID, Name = src.Name, Price = src.Price });

            mockMapper.Setup(m => m.Map<GetRoomTypeDto>(It.IsAny<RoomType>()))
                .Returns((RoomType src) => new GetRoomTypeDto { ID = src.ID, Name = src.Name, Price = src.Price });

            AutoMapperHelper.Mapper = mockMapper.Object;
        }

        [SetUp]
        public void SetUp()
        {
            _roomTypeRepositoryMock = new Mock<IRepository<RoomType>>();
        }

        #region AddRoomTypeCommand

        [Test]
        [Category("Happy")]
        public async Task AddRoomTypeCommand_Success_ReturnsSuccessResponse()
        {
            // Arrange
            var addDto = new AddRoomTypeDto { Name = "Suite", Price = 500 };
            var command = new AddRoomTypeCommand(addDto);
            var handler = new AddRoomTypeCommandHandler(_roomTypeRepositoryMock.Object);

            var roomTypeEntity = new RoomType { ID = 1, Name = "Suite", Price = 500 };

            _roomTypeRepositoryMock.Setup(r => r.CheckExistsByConditionAsync(It.IsAny<Expression<Func<RoomType, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            _roomTypeRepositoryMock.Setup(r => r.AddAsync(It.IsAny<RoomType>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(roomTypeEntity);

            _roomTypeRepositoryMock.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Message.Should().Be("Room Type Added Successfully");
            result.Data.Should().NotBeNull();
            result.Data!.Name.Should().Be("Suite");

            _roomTypeRepositoryMock.Verify(x => x.CheckExistsByConditionAsync(It.IsAny<Expression<Func<RoomType, bool>>>(), It.IsAny<CancellationToken>()), Times.Once);
            _roomTypeRepositoryMock.Verify(x => x.AddAsync(It.IsAny<RoomType>(), It.IsAny<CancellationToken>()), Times.Once);
            _roomTypeRepositoryMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        [Category("Business")]
        public async Task AddRoomTypeCommand_AlreadyExists_ReturnsFailureResponse()
        {
            // Arrange
            var addDto = new AddRoomTypeDto { Name = "Suite", Price = 500 };
            var command = new AddRoomTypeCommand(addDto);
            var handler = new AddRoomTypeCommandHandler(_roomTypeRepositoryMock.Object);

            _roomTypeRepositoryMock.Setup(r => r.CheckExistsByConditionAsync(It.IsAny<Expression<Func<RoomType, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Message.Should().Be($"Room Type '{addDto.Name}' already exists.");

            _roomTypeRepositoryMock.Verify(x => x.CheckExistsByConditionAsync(It.IsAny<Expression<Func<RoomType, bool>>>(), It.IsAny<CancellationToken>()), Times.Once);
            _roomTypeRepositoryMock.Verify(x => x.AddAsync(It.IsAny<RoomType>(), It.IsAny<CancellationToken>()), Times.Never);
            _roomTypeRepositoryMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test]
        [Category("Business")]
        public async Task AddRoomTypeCommand_FailToSave_ReturnsFailureResponse()
        {
            // Arrange
            var addDto = new AddRoomTypeDto { Name = "Suite", Price = 500 };
            var command = new AddRoomTypeCommand(addDto);
            var handler = new AddRoomTypeCommandHandler(_roomTypeRepositoryMock.Object);

            var roomTypeEntity = new RoomType { ID = 1, Name = "Suite", Price = 500 };

            _roomTypeRepositoryMock.Setup(r => r.CheckExistsByConditionAsync(It.IsAny<Expression<Func<RoomType, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            _roomTypeRepositoryMock.Setup(r => r.AddAsync(It.IsAny<RoomType>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(roomTypeEntity);

            _roomTypeRepositoryMock.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Message.Should().Be("Failed to save the Room Type to the database.");

            _roomTypeRepositoryMock.Verify(x => x.AddAsync(It.IsAny<RoomType>(), It.IsAny<CancellationToken>()), Times.Once);
            _roomTypeRepositoryMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        #endregion

        #region UpdateRoomTypeCommand

        [Test]
        [Category("Happy")]
        public async Task UpdateRoomTypeCommand_Success_ReturnsSuccessResponse()
        {
            // Arrange
            var updateDto = new UpdateRoomTypeDto { ID = 1, Name = "Deluxe", Price = 750 };
            var command = new UpdateRoomTypeCommand(updateDto);
            var handler = new UpdateRoomTypeCommandHandler(_roomTypeRepositoryMock.Object);

            _roomTypeRepositoryMock.Setup(r => r.CheckExistsByIDAsync(updateDto.ID, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            _roomTypeRepositoryMock.Setup(r => r.UpdateInclude(It.IsAny<RoomType>(), nameof(RoomType.Name), nameof(RoomType.Price)));

            _roomTypeRepositoryMock.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Message.Should().Be("Room type updated successfully");

            _roomTypeRepositoryMock.Verify(x => x.CheckExistsByIDAsync(updateDto.ID, It.IsAny<CancellationToken>()), Times.Once);
            _roomTypeRepositoryMock.Verify(x => x.UpdateInclude(It.IsAny<RoomType>(), nameof(RoomType.Name), nameof(RoomType.Price)), Times.Once);
            _roomTypeRepositoryMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        [Category("Business")]
        public async Task UpdateRoomTypeCommand_NotExist_ReturnsFailureResponse()
        {
            // Arrange
            var updateDto = new UpdateRoomTypeDto { ID = 99, Name = "Deluxe", Price = 750 };
            var command = new UpdateRoomTypeCommand(updateDto);
            var handler = new UpdateRoomTypeCommandHandler(_roomTypeRepositoryMock.Object);

            _roomTypeRepositoryMock.Setup(r => r.CheckExistsByIDAsync(updateDto.ID, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Message.Should().Be("Room type not found");

            _roomTypeRepositoryMock.Verify(x => x.CheckExistsByIDAsync(updateDto.ID, It.IsAny<CancellationToken>()), Times.Once);
            _roomTypeRepositoryMock.Verify(x => x.UpdateInclude(It.IsAny<RoomType>(), It.IsAny<string[]>()), Times.Never);
            _roomTypeRepositoryMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test]
        [Category("Business")]
        public async Task UpdateRoomTypeCommand_FailToSave_ReturnsFailureResponse()
        {
            // Arrange
            var updateDto = new UpdateRoomTypeDto { ID = 1, Name = "Deluxe", Price = 750 };
            var command = new UpdateRoomTypeCommand(updateDto);
            var handler = new UpdateRoomTypeCommandHandler(_roomTypeRepositoryMock.Object);

            _roomTypeRepositoryMock.Setup(r => r.CheckExistsByIDAsync(updateDto.ID, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            _roomTypeRepositoryMock.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Message.Should().Be("Update room type failed");

            _roomTypeRepositoryMock.Verify(x => x.CheckExistsByIDAsync(updateDto.ID, It.IsAny<CancellationToken>()), Times.Once);
            _roomTypeRepositoryMock.Verify(x => x.UpdateInclude(It.IsAny<RoomType>(), nameof(RoomType.Name), nameof(RoomType.Price)), Times.Once);
            _roomTypeRepositoryMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        #endregion

        #region CheckRoomTypeExistQuery

        [Test]
        [Category("Happy")]
        public async Task CheckRoomTypeExistQuery_Exists_ReturnsSuccessResponse()
        {
            // Arrange
            int roomTypeId = 1;
            var query = new CheckRoomTypeExistQuery(roomTypeId, CancellationToken.None);
            var handler = new CheckRoomTypeExistQueryHandler(_roomTypeRepositoryMock.Object);

            _roomTypeRepositoryMock.Setup(r => r.CheckExistsByIDAsync(roomTypeId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Message.Should().Be("Room Type Is Exist");

            _roomTypeRepositoryMock.Verify(x => x.CheckExistsByIDAsync(roomTypeId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        [Category("Business")]
        public async Task CheckRoomTypeExistQuery_NotExist_ReturnsFailureResponse()
        {
            // Arrange
            int roomTypeId = 99;
            var query = new CheckRoomTypeExistQuery(roomTypeId, CancellationToken.None);
            var handler = new CheckRoomTypeExistQueryHandler(_roomTypeRepositoryMock.Object);

            _roomTypeRepositoryMock.Setup(r => r.CheckExistsByIDAsync(roomTypeId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Message.Should().Be("Room type not exist!");

            _roomTypeRepositoryMock.Verify(x => x.CheckExistsByIDAsync(roomTypeId, It.IsAny<CancellationToken>()), Times.Once);
        }

        #endregion
    }
}
