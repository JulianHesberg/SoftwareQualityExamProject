using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using SoftwareQualityExamProject.Api;
using SoftwareQualityExamProject.Application.Services.Coupon.Interfaces;
using SoftwareQualityExamProject.Application.Services.Pricing.Interfaces;

namespace SoftwareQualityExamProject.BddTests.Support;

public class TestApiFactory : WebApplicationFactory<Program>
{
    private readonly Mock<IPricingService> _pricingMock;
    private readonly Mock<ICouponService> _couponMock;

    public TestApiFactory(Mock<IPricingService> pricingMock, Mock<ICouponService> couponMock)
    {
        _pricingMock = pricingMock;
        _couponMock = couponMock;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");

        builder.ConfigureServices(services =>
        {
            // Replace real services with mocks
            services.RemoveAll<IPricingService>();
            services.RemoveAll<ICouponService>();

            services.AddSingleton(_pricingMock.Object);
            services.AddSingleton(_couponMock.Object);
        });
    }
}
