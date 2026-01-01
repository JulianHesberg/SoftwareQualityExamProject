using SoftwareQualityExamProject.Application.Exceptions;
using SoftwareQualityExamProject.Application.Models;
using SoftwareQualityExamProject.Application.Services.Pricing.Implementations;

namespace SoftwareQualityExamProject.Tests.WhiteBox.UnitTests;

public class PricingServiceUnitTests
{
    private readonly PricingServiceImpl _service;

    public PricingServiceUnitTests()
    {
        _service = new PricingServiceImpl();
    }

    [Fact]
    public void CalculatePrice_ThrowsExpiredCouponException_WhenCouponIsExpired()
    {
        // Arrange
        var coupon = new Coupon
        {
            Id = "XYZ",
            ExpiresAt = DateTime.Now.AddDays(-1),
            DiscountPercent = 10
        };

        // Act & Assert
        Assert.Throws<ExpiredCouponException>(() =>
            _service.CalculatePrice(coupon, 100, false));
    }

    [Fact]
    public void CalculatePrice_ReturnsCorrectPrice_WhenValidCouponAndPremiumUser()
    {
        // Arrange
        var coupon = new Coupon
        {
            Id = "XYZ",
            ExpiresAt = DateTime.Now.AddDays(1),
            DiscountPercent = 20 // 20% of 100 = 20
        };

        double basePrice = 100;
        bool isPremium = true; // premium discount = 10% = 10

        // Act
        var result = _service.CalculatePrice(coupon, basePrice, isPremium);

        // Assert
        Assert.Equal(70, result); // 100 - (20 + 10)
    }

    [Fact]
    public void CalculatePrice_ReturnsCorrectPrice_WhenValidCouponAndNotPremium()
    {
        // Arrange
        var coupon = new Coupon
        {
            Id = "XYZ",
            ExpiresAt = DateTime.Now.AddDays(1),
            DiscountPercent = 25 // 25% of 200 = 50
        };

        double basePrice = 200;
        bool isPremium = false;

        // Act
        var result = _service.CalculatePrice(coupon, basePrice, isPremium);

        // Assert
        Assert.Equal(150, result); // 200 - 50
    }

    [Fact]
    public void CalculatePrice_UsesCorrectCouponDiscount()
    {
        // Arrange
        var coupon = new Coupon
        {
            Id = "XYZ",
            ExpiresAt = DateTime.Now.AddDays(1),
            DiscountPercent = 10 // 10% of 100 = 10
        };

        double basePrice = 100;

        // Act
        var result = _service.CalculatePrice(coupon, basePrice, false);

        // Assert
        Assert.Equal(90, result);
    }
    
    [Fact]
    public void CalculatePrice_UsesPremiumDiscount_WhenPremium()
    {
        // Arrange
        var coupon = new Coupon
        {
            Id = "XYZ",
            ExpiresAt = DateTime.Now.AddDays(1),
            DiscountPercent = 0
        };

        double basePrice = 100;

        // Act
        var result = _service.CalculatePrice(coupon, basePrice, true);

        // Assert
        Assert.Equal(90, result); // 10% premium discount
    }

    [Fact]
    public void CalculatePrice_NoPremiumDiscount_WhenNotPremium()
    {
        // Arrange
        var coupon = new Coupon
        {
            Id = "XYZ",
            ExpiresAt = DateTime.Now.AddDays(1),
            DiscountPercent = 0
        };

        double basePrice = 100;

        // Act
        var result = _service.CalculatePrice(coupon, basePrice, false);

        // Assert
        Assert.Equal(100, result);
    }

}