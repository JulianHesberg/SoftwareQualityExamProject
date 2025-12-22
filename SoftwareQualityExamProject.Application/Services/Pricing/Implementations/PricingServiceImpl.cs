using SoftwareQualityExamProject.Application.Exceptions;
using SoftwareQualityExamProject.Application.Services.Pricing.Interfaces;

namespace SoftwareQualityExamProject.Application.Services.Pricing.Implementations;

public class PricingServiceImpl : IPricingService
{
    public double CalculatePrice(Models.Coupon coupon, double basePrice, bool isPremium)
    {
        if (DateTime.Now > coupon.ExpiresAt)
            throw new ExpiredCouponException("Coupon is expired");

        var couponDiscount = GetCouponDiscount(basePrice, coupon.DiscountPercent);
        var premiumDiscount = GetPremiumDiscount(basePrice, isPremium);
        
        return basePrice - (couponDiscount + premiumDiscount);
    }

    private double GetCouponDiscount(double basePrice, double discountPercent)
    {
        var multiplier = discountPercent / 100;
        return  basePrice * multiplier;
    }
    
    private double GetPremiumDiscount(double basePrice, bool isPremium)
    {
        if (!isPremium)
            return 0;

        return basePrice * 0.1;
    }
}