using Microsoft.AspNetCore.Mvc;
using SoftwareQualityExamProject.Application.Exceptions;
using SoftwareQualityExamProject.Application.Models;
using SoftwareQualityExamProject.Application.Services.Coupon.Interfaces;
using SoftwareQualityExamProject.Application.Services.Pricing.Interfaces;

namespace SoftwareQualityExamProject.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PricingController : ControllerBase
{
    private readonly IPricingService _pricingService;
    private readonly ICouponService _couponService;

    public PricingController(IPricingService pricingService, ICouponService couponService)
    {
        _pricingService = pricingService;
        _couponService = couponService;
    }

    [HttpGet("calculate")]
    public IActionResult CalculatePrice([FromQuery] string? couponCode,
        [FromQuery] double basePrice,
        [FromQuery] bool isPremium)
    {
        try
        {
            Coupon? coupon = null;

            if (!string.IsNullOrWhiteSpace(couponCode))
            {
                coupon = _couponService.GetCouponByCouponCode(couponCode);

                if (!_couponService.IsCouponValid(coupon, DateTime.Now))
                    return BadRequest("Coupon is expired");
            }

            double finalPrice;

            if (coupon != null)
                finalPrice = _pricingService.CalculatePrice(coupon, basePrice, isPremium);
            else
                finalPrice = isPremium ? basePrice * 0.9 : basePrice;

            return Ok(new
            {
                BasePrice = basePrice,
                FinalPrice = finalPrice,
                CouponApplied = coupon != null,
                PremiumApplied = isPremium
            });
        }
        catch (NotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (ExpiredCouponException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (JsonErrorException ex)
        {
            return StatusCode(500, ex.Message);
        }
        catch (Exception)
        {
            return StatusCode(500, "Unexpected error occurred");
        }
    }

    
}