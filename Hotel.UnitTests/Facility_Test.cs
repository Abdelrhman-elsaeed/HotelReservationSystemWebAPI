using Application.AutoMapper.Profiles;
using Application.CQRS.Facility.Command;
using Application.CQRS.Facility.Queries;
using Application.DTOS.Facility;
using AutoMapper;
using Domain.Entities;
using Domain.Repositories.Interfaces;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;

namespace Hotel.UnitTests
{
    [TestFixture]
    public class Facility_Test
    {

        private Mock<IRepository<Facility>> _facilityRepositoryMock = null!;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            var mockMapper = new Mock<IMapper>();

            mockMapper.Setup(m => m.Map<Facility>(It.IsAny<object>()))
                .Returns((object src) =>
                {
                    if (src is AddFacilityDto addDto) return new Facility { Name = addDto.Name, Price = addDto.Price };
                    if (src is UpdateFacilityDto updateDto) return new Facility { ID = updateDto.ID, Name = updateDto.Name, Price = updateDto.Price };
                    if (src is DeleteFacilityDto deleteDto) return new Facility { ID = deleteDto.ID, Name = deleteDto.Name, Price = deleteDto.Price };
                    return new Facility();
                });

            mockMapper.Setup(m => m.Map<GetFacilityDto>(It.IsAny<Facility>()))
                .Returns((Facility src) => new GetFacilityDto { ID = src.ID, Name = src.Name, Price = src.Price });

            mockMapper.Setup(m => m.Map<AddFacilityDto>(It.IsAny<Facility>()))
                .Returns((Facility src) => new AddFacilityDto { ID = src.ID, Name = src.Name, Price = src.Price });

            mockMapper.Setup(m => m.Map<UpdateFacilityDto>(It.IsAny<Facility>()))
                .Returns((Facility src) => new UpdateFacilityDto { ID = src.ID, Name = src.Name, Price = src.Price });

            AutoMapperHelper.Mapper = mockMapper.Object;
        }

        [SetUp]
        public void SetUp()
        {
            _facilityRepositoryMock = new Mock<IRepository<Facility>>();
        }

        #region AddFacilityCommand

        [Test]
        [Category("Happy")]
        public async Task AddFacilityCommand_Success_ReturnsSuccessResponse()
        {
            // Arrange
            var addFacilityDto = new AddFacilityDto { Name = "Pool", Price = 50 };
            var command = new AddFacilityCommand(addFacilityDto);
            var handler = new AddFacilityCommandHandler(_facilityRepositoryMock.Object);

            var facilityEntity = new Facility { ID = 1, Name = "Pool", Price = 50 };

            _facilityRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Facility>(), It.IsAny<CancellationToken>())).ReturnsAsync(facilityEntity);

            _facilityRepositoryMock.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(true);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Message.Should().Be("facility added successfully");

            _facilityRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Facility>(), It.IsAny<CancellationToken>()), Times.Once);
            _facilityRepositoryMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        [Category("Business")]
        public async Task AddFacilityCommand_FailToSave_ReturnsFailureResponse()
        {
            // Arrange
            var addFacilityDto = new AddFacilityDto { Name = "Pool", Price = 50 };
            var command = new AddFacilityCommand(addFacilityDto);
            var handler = new AddFacilityCommandHandler(_facilityRepositoryMock.Object);

            var facilityEntity = new Facility { ID = 1, Name = "Pool", Price = 50 };

            _facilityRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Facility>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(facilityEntity);

            _facilityRepositoryMock.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(false);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Message.Should().Be("Fail to add facility!");

            _facilityRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Facility>(), It.IsAny<CancellationToken>()), Times.Once);
            _facilityRepositoryMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        #endregion

        #region UpdateFacilityCommand

        [Test]
        [Category("Happy")]
        public async Task UpdateFacilityCommand_Success_ReturnsSuccessResponse()
        {
            // Arrange
            var updateFacilityDto = new UpdateFacilityDto { ID = 1, Name = "Gym", Price = 100 };
            var command = new UpdateFacilityCommand(updateFacilityDto);
            var handler = new UpdateFacilityCommandHandler(_facilityRepositoryMock.Object);

            _facilityRepositoryMock.Setup(r => r.CheckExistsByIDAsync(updateFacilityDto.ID, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            _facilityRepositoryMock.Setup(r => r.UpdateInclude(It.IsAny<Facility>(), nameof(Facility.Name), nameof(Facility.Price)));

            _facilityRepositoryMock.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Message.Should().Be("Facility updated successfully");

            _facilityRepositoryMock.Verify(x => x.CheckExistsByIDAsync(updateFacilityDto.ID, It.IsAny<CancellationToken>()), Times.Once);
            _facilityRepositoryMock.Verify(x => x.UpdateInclude(It.IsAny<Facility>(), nameof(Facility.Name), nameof(Facility.Price)), Times.Once);
            _facilityRepositoryMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        [Category("Business")]
        public async Task UpdateFacilityCommand_NotExist_ReturnsFailureResponse()
        {
            // Arrange
            var updateFacilityDto = new UpdateFacilityDto { ID = 1, Name = "Gym", Price = 100 };
            var command = new UpdateFacilityCommand(updateFacilityDto);
            var handler = new UpdateFacilityCommandHandler(_facilityRepositoryMock.Object);

            _facilityRepositoryMock.Setup(r => r.CheckExistsByIDAsync(updateFacilityDto.ID, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Message.Should().Be("Facility not Found");

            _facilityRepositoryMock.Verify(x => x.CheckExistsByIDAsync(updateFacilityDto.ID, It.IsAny<CancellationToken>()), Times.Once);
            _facilityRepositoryMock.Verify(x => x.UpdateInclude(It.IsAny<Facility>(), It.IsAny<string[]>()), Times.Never);
            _facilityRepositoryMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test]
        [Category("Business")]
        public async Task UpdateFacilityCommand_FailToSave_ReturnsFailureResponse()
        {
            // Arrange
            var updateFacilityDto = new UpdateFacilityDto { ID = 1, Name = "Gym", Price = 100 };
            var command = new UpdateFacilityCommand(updateFacilityDto);
            var handler = new UpdateFacilityCommandHandler(_facilityRepositoryMock.Object);

            _facilityRepositoryMock.Setup(r => r.CheckExistsByIDAsync(updateFacilityDto.ID, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            _facilityRepositoryMock.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Message.Should().Be("Facility fail to update");

            _facilityRepositoryMock.Verify(x => x.CheckExistsByIDAsync(updateFacilityDto.ID, It.IsAny<CancellationToken>()), Times.Once);
            _facilityRepositoryMock.Verify(x => x.UpdateInclude(It.IsAny<Facility>(), nameof(Facility.Name), nameof(Facility.Price)), Times.Once);
            _facilityRepositoryMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        #endregion

        #region DeleteFacilityCommand

        [Test]
        [Category("Happy")]
        public async Task DeleteFacilityCommand_Success_ReturnsSuccessResponse()
        {
            // Arrange
            var deleteFacilityDto = new DeleteFacilityDto { ID = 1 };
            var command = new DeleteFacilityCommand(deleteFacilityDto);
            var handler = new DeleteFacilityCommandHandler(_facilityRepositoryMock.Object);

            _facilityRepositoryMock.Setup(r => r.CheckExistsByIDAsync(deleteFacilityDto.ID, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            _facilityRepositoryMock.Setup(r => r.SoftDelete(It.IsAny<Facility>()));

            _facilityRepositoryMock.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Message.Should().Be("facility deleted successfully");

            _facilityRepositoryMock.Verify(x => x.CheckExistsByIDAsync(deleteFacilityDto.ID, It.IsAny<CancellationToken>()), Times.Once);
            _facilityRepositoryMock.Verify(x => x.SoftDelete(It.IsAny<Facility>()), Times.Once);
            _facilityRepositoryMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        [Category("Business")]
        public async Task DeleteFacilityCommand_NotExist_ReturnsFailureResponse()
        {
            // Arrange
            var deleteFacilityDto = new DeleteFacilityDto { ID = 1 };
            var command = new DeleteFacilityCommand(deleteFacilityDto);
            var handler = new DeleteFacilityCommandHandler(_facilityRepositoryMock.Object);

            _facilityRepositoryMock.Setup(r => r.CheckExistsByIDAsync(deleteFacilityDto.ID, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Message.Should().Be("Facility not Found");

            _facilityRepositoryMock.Verify(x => x.CheckExistsByIDAsync(deleteFacilityDto.ID, It.IsAny<CancellationToken>()), Times.Once);
            _facilityRepositoryMock.Verify(x => x.SoftDelete(It.IsAny<Facility>()), Times.Never);
            _facilityRepositoryMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test]
        [Category("Business")]
        public async Task DeleteFacilityCommand_FailToSave_ReturnsFailureResponse()
        {
            // Arrange
            var deleteFacilityDto = new DeleteFacilityDto { ID = 1 };
            var command = new DeleteFacilityCommand(deleteFacilityDto);
            var handler = new DeleteFacilityCommandHandler(_facilityRepositoryMock.Object);

            _facilityRepositoryMock.Setup(r => r.CheckExistsByIDAsync(deleteFacilityDto.ID, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            _facilityRepositoryMock.Setup(r => r.SoftDelete(It.IsAny<Facility>()));

            _facilityRepositoryMock.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Message.Should().Be("delete facility fail");

            _facilityRepositoryMock.Verify(x => x.CheckExistsByIDAsync(deleteFacilityDto.ID, It.IsAny<CancellationToken>()), Times.Once);
            _facilityRepositoryMock.Verify(x => x.SoftDelete(It.IsAny<Facility>()), Times.Once);
            _facilityRepositoryMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        #endregion

        #region GetFacilityQuery

        [Test]
        [Category("Happy")]
        public async Task GetFacilityQuery_Success_ReturnsSuccessResponse()
        {
            // Arrange
            int facilityId = 1;
            var command = new GetFacilityQuery(facilityId);
            var handler = new GetFacilityQueryHandler(_facilityRepositoryMock.Object);
            var facilityEntity = new Facility { ID = facilityId, Name = "Spa", Price = 200 };

            _facilityRepositoryMock.Setup(r => r.GetByIDAsync(facilityId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(facilityEntity);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Message.Should().Be("Facility retrieved successfully");

            result.Data.Should().NotBeNull();
            result.Data!.ID.Should().Be(facilityId);
            result.Data.Name.Should().Be("Spa");

            _facilityRepositoryMock.Verify(x => x.GetByIDAsync(facilityId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        [Category("Business")]
        public async Task GetFacilityQuery_NotExist_ReturnsFailureResponse()
        {
            // Arrange
            int facilityId = 1;
            var command = new GetFacilityQuery(facilityId);
            var handler = new GetFacilityQueryHandler(_facilityRepositoryMock.Object);

            _facilityRepositoryMock.Setup(r => r.GetByIDAsync(facilityId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Facility?)null);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Message.Should().Be("Facility not Found");

            _facilityRepositoryMock.Verify(x => x.GetByIDAsync(facilityId, It.IsAny<CancellationToken>()), Times.Once);
        }

        #endregion


    }
}
