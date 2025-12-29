using System.Net.Http;
using Moq;
using SoftwareQualityExamProject.Application.Models;
using SoftwareQualityExamProject.Application.Services.Coupon.Interfaces;
using SoftwareQualityExamProject.Application.Services.Pricing.Interfaces;

namespace SoftwareQualityExamProject.BddTests.Support;

public class PricingTestContext : IDisposable
{
    public Mock<IPricingService> PricingServiceMock { get; private set; } = null!;
    public Mock<ICouponService> CouponServiceMock { get; private set; } = null!;

    public TestApiFactory Factory { get; private set; } = null!;
    public HttpClient? Client { get; private set; }

    // Neutral naming: whatever the user types as "coupon" in the scenario
    public string? CouponInput { get; set; } = "";

    public double BasePrice { get; set; }
    public bool IsPremium { get; set; }

    public Coupon? Coupon { get; set; }

    public HttpResponseMessage? Response { get; set; }
    public string? ResponseBody { get; set; }

    public void Reset()
    {
        PricingServiceMock = new Mock<IPricingService>(MockBehavior.Loose);
        CouponServiceMock = new Mock<ICouponService>(MockBehavior.Loose);

        Factory = new TestApiFactory(PricingServiceMock, CouponServiceMock);

        Client?.Dispose();
        Client = Factory.CreateClient();

        CouponInput = "";
        BasePrice = 0;
        IsPremium = false;
        Coupon = null;

        Response = null;
        ResponseBody = null;
    }

    public void Dispose()
    {
        Client?.Dispose();
        Factory?.Dispose();
    }
}
