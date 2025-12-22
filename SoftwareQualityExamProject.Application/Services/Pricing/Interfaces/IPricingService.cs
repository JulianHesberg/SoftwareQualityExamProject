namespace SoftwareQualityExamProject.Application.Services.Pricing.Interfaces;

public interface IPricingService
{
    public double CalculatePrice(Models.Coupon couponCode, double basePrice, bool isPremium);
}