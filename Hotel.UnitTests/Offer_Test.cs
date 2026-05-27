using Application.AutoMapper.Profiles;
using Application.CQRS.Offer.Command;
using Application.DTOS.Offer;
using AutoMapper;
using Domain.Entities;
using Domain.Repositories.Interfaces;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace Hotel.UnitTests
{
    [TestFixture]
    public class Offer_Test
    {
        private Mock<IRepository<Offer>> _offerRepositoryMock = null!;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            var mockMapper = new Mock<IMapper>();

            mockMapper.Setup(m => m.Map<Offer>(It.IsAny<object>()))
                .Returns((object src) =>
                {
                    if (src is AddOfferDto addDto)
                        return new Offer { DiscountPercentage = addDto.DiscountPercentage, StartDate = addDto.StartDate, EndDate = addDto.EndDate };
                    return new Offer();
                });

            mockMapper.Setup(m => m.Map<AddOfferDto>(It.IsAny<Offer>()))
                .Returns((Offer src) => new AddOfferDto { ID = src.ID, DiscountPercentage = src.DiscountPercentage, StartDate = src.StartDate, EndDate = src.EndDate });

            AutoMapperHelper.Mapper = mockMapper.Object;
        }

        [SetUp]
        public void SetUp()
        {
            _offerRepositoryMock = new Mock<IRepository<Offer>>();
        }

        #region AddOfferCommand

        [Test]
        [Category("Happy")]
        public async Task AddOfferCommand_Success_ReturnsSuccessResponse()
        {
            // Arrange
            var addDto = new AddOfferDto
            {
                DiscountPercentage = 15,
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddDays(30)
            };
            var command = new AddOfferCommand(addDto);
            var handler = new AddOfferCommandHandler(_offerRepositoryMock.Object);

            var offerEntity = new Offer { ID = 1, DiscountPercentage = 15, StartDate = addDto.StartDate, EndDate = addDto.EndDate };

            _offerRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Offer>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(offerEntity);

            _offerRepositoryMock.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Message.Should().Be("Offer added successfully.");
            result.Data.Should().NotBeNull();
            result.Data!.DiscountPercentage.Should().Be(15);

            _offerRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Offer>(), It.IsAny<CancellationToken>()), Times.Once);
            _offerRepositoryMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        [Category("Business")]
        public async Task AddOfferCommand_FailToSave_ReturnsFailureResponse()
        {
            // Arrange
            var addDto = new AddOfferDto
            {
                DiscountPercentage = 15,
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddDays(30)
            };
            var command = new AddOfferCommand(addDto);
            var handler = new AddOfferCommandHandler(_offerRepositoryMock.Object);

            var offerEntity = new Offer { ID = 1, DiscountPercentage = 15, StartDate = addDto.StartDate, EndDate = addDto.EndDate };

            _offerRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Offer>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(offerEntity);

            _offerRepositoryMock.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Message.Should().Be("Failed to add the new offer.");

            _offerRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Offer>(), It.IsAny<CancellationToken>()), Times.Once);
            _offerRepositoryMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        #endregion
    }
}
