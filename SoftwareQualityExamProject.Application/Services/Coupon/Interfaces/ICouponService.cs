namespace SoftwareQualityExamProject.Application.Services.Coupon.Interfaces;

public interface ICouponService
{
    /// <summary>
    /// Reads coupon and checks expiration date against current date.
    /// </summary>
    /// <param name="couponCode"></param>
    /// <param name="currentDate"></param>
    /// <returns>true if valid date, otherwise false</returns>
    public bool IsCouponValid(Models.Coupon couponCode, DateTime currentDate);
    
    public Models.Coupon GetCouponByCouponCode(string couponCode);
}