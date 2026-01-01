using System.Text.Json;
using Moq;
using SoftwareQualityExamProject.Application.Exceptions;
using SoftwareQualityExamProject.Application.Models;
using SoftwareQualityExamProject.Application.Services.Coupon.Implementations;
using SoftwareQualityExamProject.Application.Services.FileReader;

namespace SoftwareQualityExamProject.Tests.WhiteBox.UnitTests;

public class CouponServiceUnitTests
{
    private readonly Mock<IFileReader> _fileReaderMock;
    private readonly CouponServiceImpl _service;

    public CouponServiceUnitTests()
    {
        _fileReaderMock = new Mock<IFileReader>();
        _service = new CouponServiceImpl(_fileReaderMock.Object);
    }

    [Fact]
    public void IsCouponValid_ReturnsTrue_WhenExpiresAtIsInFuture()
    {
        // Arrange
        var coupon = new Coupon
        {
            Id = "XYZ",
            ExpiresAt = DateTime.Now.AddDays(1),
            DiscountPercent = 0.2
        };

        // Act
        var result = _service.IsCouponValid(coupon, DateTime.Now);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsCouponValid_ReturnsFalse_WhenExpiresAtIsInPast()
    {
        // Arrange
        var coupon = new Coupon
        {
            Id = new Guid().ToString(),
            ExpiresAt = DateTime.Now.AddDays(-1),
            DiscountPercent = 0.2
        };

        // Act
        var result = _service.IsCouponValid(coupon, DateTime.Now);

        // Assert
        Assert.False(result);
    }
    
    [Fact]
    public void GetCouponByCouponCode_ThrowsJsonErrorException_WhenFileReadFails()
    {
        // Arrange
        _fileReaderMock
            .Setup(fr => fr.ReadAllText(It.IsAny<string>()))
            .Throws(new Exception("File error"));

        // Act & Assert
        Assert.Throws<JsonErrorException>(() => _service.GetCouponByCouponCode("ABC"));
    }
    
    [Fact]
    public void GetCouponByCouponCode_ThrowsApplicationException_WhenCouponsListIsEmpty()
    {
        // Arrange
        var json = "[]";
        _fileReaderMock.Setup(fr => fr.ReadAllText(It.IsAny<string>())).Returns(json);

        // Act & Assert
        Assert.Throws<System.ApplicationException>(() => _service.GetCouponByCouponCode("ABC"));
    }
    
    [Fact]
    public void GetCouponByCouponCode_ThrowsApplicationException_WhenCouponsListIsNull()
    {
        // Arrange
        string json = "null";
        _fileReaderMock.Setup(fr => fr.ReadAllText(It.IsAny<string>())).Returns(json);

        // Act & Assert
        Assert.Throws<System.ApplicationException>(() => _service.GetCouponByCouponCode("ABC"));
    }
    
    [Fact]
    public void GetCouponByCouponCode_ThrowsNotFoundException_WhenCouponDoesNotExist()
    {
        // Arrange
        var coupons = new List<Coupon>
        {
            new Coupon
            {
                Id = "XYZ",
                ExpiresAt = DateTime.Now.AddDays(5),
                DiscountPercent = 0.2
            }
        };

        var json = JsonSerializer.Serialize(coupons);
        _fileReaderMock.Setup(fr => fr.ReadAllText(It.IsAny<string>())).Returns(json);

        // Act & Assert
        Assert.Throws<NotFoundException>(() => _service.GetCouponByCouponCode("ABC"));
    }
    
    [Fact]
    public void GetCouponByCouponCode_ReturnsCoupon_WhenCouponExists()
    {
        // Arrange
        var expected = new Coupon
        {
            Id = "ABC",
            ExpiresAt = DateTime.Now.AddDays(5),
            DiscountPercent = 0.2
        };

        var coupons = new List<Coupon> { expected };
        var json = JsonSerializer.Serialize(coupons);

        _fileReaderMock.Setup(fr => fr.ReadAllText(It.IsAny<string>())).Returns(json);

        // Act
        var result = _service.GetCouponByCouponCode("ABC");

        // Assert
        Assert.Equal(expected.Id, result.Id);
        Assert.Equal(expected.ExpiresAt, result.ExpiresAt);
    }

}