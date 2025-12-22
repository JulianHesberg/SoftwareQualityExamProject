using System.Text.Json;
using SoftwareQualityExamProject.Application.Exceptions;
using SoftwareQualityExamProject.Application.Services.Coupon.Interfaces;
using ApplicationException = System.ApplicationException;

namespace SoftwareQualityExamProject.Application.Services.Coupon.Implementations;

public class CouponServiceImpl : ICouponService
{
    public bool IsCouponValid(Models.Coupon coupon, DateTime currentDate)
    {
        var now = DateTime.Now;
        var expiresAt = coupon.ExpiresAt;

        return now < expiresAt;
    }

    public Models.Coupon GetCouponByCouponCode(string couponCode)
    {
        var coupons = new List<Models.Coupon>();
        
        try
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var json = File.ReadAllText("coupons.json");
            coupons = System.Text.Json.JsonSerializer.Deserialize<List<Models.Coupon>>(json, options);
        }
        catch (Exception e)
        {
            throw new JsonErrorException("Could not read coupons from json", e);
        }
        
        if (coupons == null || coupons.Count == 0)
            throw new ApplicationException("Something went wrong while fetching coupons");
        
        var coupon = coupons.FirstOrDefault(c => c.Id == couponCode);
        
        if (coupon == null)
            throw new NotFoundException("Coupon not found");
        
        return coupon;
    }
    
}