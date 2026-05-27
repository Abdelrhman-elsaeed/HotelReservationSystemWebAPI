using Application.AutoMapper.Profiles;
using Application.CQRS.Guest.Command;
using Application.CQRS.Guest.Queries;
using Application.DTOS.Guest;
using AutoMapper;
using Domain.Repositories.Interfaces;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace Hotel.UnitTests
{
    [TestFixture]
    public class Guest_Test
    {
        private Mock<IRepository<Domain.Entities.Guest.Guest>> _guestRepositoryMock = null!;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            var mockMapper = new Mock<IMapper>();

            mockMapper.Setup(m => m.Map<Domain.Entities.Guest.Guest>(It.IsAny<object>()))
                .Returns((object src) =>
                {
                    if (src is AddGuestDto addDto)
                        return new Domain.Entities.Guest.Guest { FullName = addDto.FullName, NationalId = addDto.NationalId, MobileNumber = addDto.MobileNumber };
                    if (src is UpdateGuestDto updateDto)
                        return new Domain.Entities.Guest.Guest { ID = updateDto.ID, FullName = updateDto.FullName, NationalId = updateDto.NationalId, MobileNumber = updateDto.MobileNumber };
                    return new Domain.Entities.Guest.Guest();
                });

            mockMapper.Setup(m => m.Map<AddGuestDto>(It.IsAny<Domain.Entities.Guest.Guest>()))
                .Returns((Domain.Entities.Guest.Guest src) => new AddGuestDto { ID = src.ID, FullName = src.FullName, NationalId = src.NationalId, MobileNumber = src.MobileNumber });

            mockMapper.Setup(m => m.Map<UpdateGuestDto>(It.IsAny<Domain.Entities.Guest.Guest>()))
                .Returns((Domain.Entities.Guest.Guest src) => new UpdateGuestDto { ID = src.ID, FullName = src.FullName, NationalId = src.NationalId, MobileNumber = src.MobileNumber });

            mockMapper.Setup(m => m.Map<GetGuestDto>(It.IsAny<Domain.Entities.Guest.Guest>()))
                .Returns((Domain.Entities.Guest.Guest src) => new GetGuestDto { ID = src.ID, FullName = src.FullName, NationalId = src.NationalId, MobileNumber = src.MobileNumber });

            AutoMapperHelper.Mapper = mockMapper.Object;
        }

        [SetUp]
        public void SetUp()
        {
            _guestRepositoryMock = new Mock<IRepository<Domain.Entities.Guest.Guest>>();
        }

        #region AddGuestCommand

        [Test]
        [Category("Happy")]
        public async Task AddGuestCommand_Success_ReturnsSuccessResponse()
        {
            // Arrange
            var addDto = new AddGuestDto { FullName = "John Doe", NationalId = "123456789", MobileNumber = "0501234567" };
            var command = new AddGuestCommand(addDto);
            var handler = new AddGuestCommandHandler(_guestRepositoryMock.Object);

            var guestEntity = new Domain.Entities.Guest.Guest { ID = 1, FullName = "John Doe", NationalId = "123456789", MobileNumber = "0501234567" };

            _guestRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Domain.Entities.Guest.Guest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(guestEntity);

            _guestRepositoryMock.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Message.Should().Be("Guest add successfully");
            result.Data.Should().NotBeNull();
            result.Data!.FullName.Should().Be("John Doe");

            _guestRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Domain.Entities.Guest.Guest>(), It.IsAny<CancellationToken>()), Times.Once);
            _guestRepositoryMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        [Category("Business")]
        public async Task AddGuestCommand_FailToSave_ReturnsFailureResponse()
        {
            // Arrange
            var addDto = new AddGuestDto { FullName = "John Doe", NationalId = "123456789", MobileNumber = "0501234567" };
            var command = new AddGuestCommand(addDto);
            var handler = new AddGuestCommandHandler(_guestRepositoryMock.Object);

            var guestEntity = new Domain.Entities.Guest.Guest { ID = 1, FullName = "John Doe", NationalId = "123456789", MobileNumber = "0501234567" };

            _guestRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Domain.Entities.Guest.Guest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(guestEntity);

            _guestRepositoryMock.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Message.Should().Be("Add guest fail");

            _guestRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Domain.Entities.Guest.Guest>(), It.IsAny<CancellationToken>()), Times.Once);
            _guestRepositoryMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        #endregion

        #region UpdateGuestCommand

        [Test]
        [Category("Happy")]
        public async Task UpdateGuestCommand_Success_ReturnsSuccessResponse()
        {
            // Arrange
            var updateDto = new UpdateGuestDto { ID = 1, FullName = "Jane Doe", NationalId = "987654321", MobileNumber = "0509876543" };
            var command = new UpdateGuestCommand(updateDto);
            var handler = new UpdateGuestCommandHandler(_guestRepositoryMock.Object);

            _guestRepositoryMock.Setup(r => r.CheckExistsByIDAsync(updateDto.ID, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            _guestRepositoryMock.Setup(r => r.UpdateInclude(It.IsAny<Domain.Entities.Guest.Guest>(),
                nameof(Domain.Entities.Guest.Guest.FullName),
                nameof(Domain.Entities.Guest.Guest.NationalId),
                nameof(Domain.Entities.Guest.Guest.MobileNumber)));

            _guestRepositoryMock.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Message.Should().Be("Guest updated successfully");

            _guestRepositoryMock.Verify(x => x.CheckExistsByIDAsync(updateDto.ID, It.IsAny<CancellationToken>()), Times.Once);
            _guestRepositoryMock.Verify(x => x.UpdateInclude(It.IsAny<Domain.Entities.Guest.Guest>(),
                nameof(Domain.Entities.Guest.Guest.FullName),
                nameof(Domain.Entities.Guest.Guest.NationalId),
                nameof(Domain.Entities.Guest.Guest.MobileNumber)), Times.Once);
            _guestRepositoryMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        [Category("Business")]
        public async Task UpdateGuestCommand_NotFound_ReturnsFailureResponse()
        {
            // Arrange
            var updateDto = new UpdateGuestDto { ID = 99, FullName = "Jane Doe", NationalId = "987654321", MobileNumber = "0509876543" };
            var command = new UpdateGuestCommand(updateDto);
            var handler = new UpdateGuestCommandHandler(_guestRepositoryMock.Object);

            _guestRepositoryMock.Setup(r => r.CheckExistsByIDAsync(updateDto.ID, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Message.Should().Be("Guest not found");

            _guestRepositoryMock.Verify(x => x.CheckExistsByIDAsync(updateDto.ID, It.IsAny<CancellationToken>()), Times.Once);
            _guestRepositoryMock.Verify(x => x.UpdateInclude(It.IsAny<Domain.Entities.Guest.Guest>(), It.IsAny<string[]>()), Times.Never);
            _guestRepositoryMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test]
        [Category("Business")]
        public async Task UpdateGuestCommand_FailToSave_ReturnsFailureResponse()
        {
            // Arrange
            var updateDto = new UpdateGuestDto { ID = 1, FullName = "Jane Doe", NationalId = "987654321", MobileNumber = "0509876543" };
            var command = new UpdateGuestCommand(updateDto);
            var handler = new UpdateGuestCommandHandler(_guestRepositoryMock.Object);

            _guestRepositoryMock.Setup(r => r.CheckExistsByIDAsync(updateDto.ID, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            _guestRepositoryMock.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Message.Should().Be("Update guest failed");

            _guestRepositoryMock.Verify(x => x.CheckExistsByIDAsync(updateDto.ID, It.IsAny<CancellationToken>()), Times.Once);
            _guestRepositoryMock.Verify(x => x.UpdateInclude(It.IsAny<Domain.Entities.Guest.Guest>(),
                nameof(Domain.Entities.Guest.Guest.FullName),
                nameof(Domain.Entities.Guest.Guest.NationalId),
                nameof(Domain.Entities.Guest.Guest.MobileNumber)), Times.Once);
            _guestRepositoryMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        #endregion

        #region DeleteGuestCommand

        [Test]
        [Category("Happy")]
        public async Task DeleteGuestCommand_Success_ReturnsSuccessResponse()
        {
            // Arrange
            int guestId = 1;
            var command = new DeleteGuestCommand(guestId);
            var handler = new DeleteGuestCommandHandler(_guestRepositoryMock.Object);

            var guestEntity = new Domain.Entities.Guest.Guest { ID = guestId, FullName = "John Doe", NationalId = "123456789", MobileNumber = "0501234567" };

            _guestRepositoryMock.Setup(r => r.GetByIDAsync(guestId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(guestEntity);

            _guestRepositoryMock.Setup(r => r.SoftDelete(It.IsAny<Domain.Entities.Guest.Guest>()));

            _guestRepositoryMock.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Message.Should().Be("Guest deleted successfully");

            _guestRepositoryMock.Verify(x => x.GetByIDAsync(guestId, It.IsAny<CancellationToken>()), Times.Once);
            _guestRepositoryMock.Verify(x => x.SoftDelete(It.IsAny<Domain.Entities.Guest.Guest>()), Times.Once);
            _guestRepositoryMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        [Category("Business")]
        public async Task DeleteGuestCommand_NotFound_ReturnsFailureResponse()
        {
            // Arrange
            int guestId = 99;
            var command = new DeleteGuestCommand(guestId);
            var handler = new DeleteGuestCommandHandler(_guestRepositoryMock.Object);

            _guestRepositoryMock.Setup(r => r.GetByIDAsync(guestId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Domain.Entities.Guest.Guest?)null);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Message.Should().Be("Guest not found");

            _guestRepositoryMock.Verify(x => x.GetByIDAsync(guestId, It.IsAny<CancellationToken>()), Times.Once);
            _guestRepositoryMock.Verify(x => x.SoftDelete(It.IsAny<Domain.Entities.Guest.Guest>()), Times.Never);
            _guestRepositoryMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test]
        [Category("Business")]
        public async Task DeleteGuestCommand_FailToSave_ReturnsFailureResponse()
        {
            // Arrange
            int guestId = 1;
            var command = new DeleteGuestCommand(guestId);
            var handler = new DeleteGuestCommandHandler(_guestRepositoryMock.Object);

            var guestEntity = new Domain.Entities.Guest.Guest { ID = guestId, FullName = "John Doe", NationalId = "123456789", MobileNumber = "0501234567" };

            _guestRepositoryMock.Setup(r => r.GetByIDAsync(guestId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(guestEntity);

            _guestRepositoryMock.Setup(r => r.SoftDelete(It.IsAny<Domain.Entities.Guest.Guest>()));

            _guestRepositoryMock.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Message.Should().Be("Delete guest failed");

            _guestRepositoryMock.Verify(x => x.GetByIDAsync(guestId, It.IsAny<CancellationToken>()), Times.Once);
            _guestRepositoryMock.Verify(x => x.SoftDelete(It.IsAny<Domain.Entities.Guest.Guest>()), Times.Once);
            _guestRepositoryMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        #endregion

        #region GetGuestQuery

        [Test]
        [Category("Happy")]
        public async Task GetGuestQuery_Success_ReturnsSuccessResponse()
        {
            // Arrange
            int guestId = 1;
            var query = new GetGuestQuery(guestId);
            var handler = new GetGuestQueryHandler(_guestRepositoryMock.Object);

            var guestEntity = new Domain.Entities.Guest.Guest { ID = guestId, FullName = "John Doe", NationalId = "123456789", MobileNumber = "0501234567" };

            _guestRepositoryMock.Setup(r => r.GetByIDAsync(guestId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(guestEntity);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Message.Should().Be("Guest retrieved successfully");
            result.Data.Should().NotBeNull();
            result.Data!.ID.Should().Be(guestId);
            result.Data.FullName.Should().Be("John Doe");

            _guestRepositoryMock.Verify(x => x.GetByIDAsync(guestId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        [Category("Business")]
        public async Task GetGuestQuery_NotFound_ReturnsFailureResponse()
        {
            // Arrange
            int guestId = 99;
            var query = new GetGuestQuery(guestId);
            var handler = new GetGuestQueryHandler(_guestRepositoryMock.Object);

            _guestRepositoryMock.Setup(r => r.GetByIDAsync(guestId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Domain.Entities.Guest.Guest?)null);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Message.Should().Be("Guest not found");

            _guestRepositoryMock.Verify(x => x.GetByIDAsync(guestId, It.IsAny<CancellationToken>()), Times.Once);
        }

        #endregion

        #region IsGuestExistQuery

        [Test]
        [Category("Happy")]
        public async Task IsGuestExistQuery_Exists_ReturnsSuccessResponse()
        {
            // Arrange
            int guestId = 1;
            var query = new IsGuestExistQuery(guestId);
            var handler = new IsGuestExistQueryHandler(_guestRepositoryMock.Object);

            _guestRepositoryMock.Setup(r => r.CheckExistsByIDAsync(guestId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Message.Should().Be("Guest is exist");

            _guestRepositoryMock.Verify(x => x.CheckExistsByIDAsync(guestId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        [Category("Business")]
        public async Task IsGuestExistQuery_NotExist_ReturnsFailureResponse()
        {
            // Arrange
            int guestId = 99;
            var query = new IsGuestExistQuery(guestId);
            var handler = new IsGuestExistQueryHandler(_guestRepositoryMock.Object);

            _guestRepositoryMock.Setup(r => r.CheckExistsByIDAsync(guestId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Message.Should().Be("Guest not found on the system");

            _guestRepositoryMock.Verify(x => x.CheckExistsByIDAsync(guestId, It.IsAny<CancellationToken>()), Times.Once);
        }

        #endregion
    }
}
