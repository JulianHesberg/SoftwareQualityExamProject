using System.Globalization;
using System.Text.Json;
using FluentAssertions;
using Moq;
using Reqnroll;
using SoftwareQualityExamProject.Application.Exceptions;
using SoftwareQualityExamProject.Application.Models;
using SoftwareQualityExamProject.Application.Services.Coupon.Interfaces;
using SoftwareQualityExamProject.Application.Services.Pricing.Interfaces;
using SoftwareQualityExamProject.BddTests.Support;

namespace SoftwareQualityExamProject.BddTests.Steps;

[Binding]
public class PricingSteps
{
    private readonly PricingTestContext _ctx;

    // Your controller currently expects the query param name "couponCode".
    // We keep that only as a technical detail; tests avoid using "couponCode" as a concept.
    private const string CouponQueryParamName = "couponCode";

    public PricingSteps(PricingTestContext ctx)
    {
        _ctx = ctx;
    }

    [Given(@"coupon is ""(.*)""")]
    public void GivenCouponIs(string couponCode)
    {
        _ctx.CouponInput = couponCode;
    }

    [Given(@"basePrice is (.*)")]
    public void GivenBasePriceIs(double basePrice)
    {
        _ctx.BasePrice = basePrice;
    }

    [Given(@"premium is (.*)")]
    public void GivenPremiumIs(string premium)
    {
        _ctx.IsPremium = bool.Parse(premium);
    }

    [Given("coupon lookup returns a coupon")]
    public void GivenCouponLookupReturnsACoupon()
    {
        // Coupon model has no CouponCode property; only set required members
        var coupon = new Coupon
        {
            Id = "TEST-1",
            DiscountPercent = 10,
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        };

        _ctx.Coupon = coupon;

        _ctx.CouponServiceMock
            .Setup(s => s.GetCouponByCouponCode(_ctx.CouponInput!))
            .Returns(coupon);
    }

    [Given(@"coupon lookup throws NotFoundException with message ""(.*)""")]
    public void GivenCouponLookupThrowsNotFound(string message)
    {
        _ctx.CouponServiceMock
            .Setup(s => s.GetCouponByCouponCode(_ctx.CouponInput!))
            .Throws(new NotFoundException(message));
    }

    [Given(@"coupon is valid is (.*)")]
    public void GivenCouponIsValidIs(string expected)
    {
        var isValid = bool.Parse(expected);

        _ctx.CouponServiceMock
            .Setup(s => s.IsCouponValid(It.IsAny<Coupon>(), It.IsAny<DateTime>()))
            .Returns(isValid);
    }

    [Given(@"coupon validation throws ExpiredCouponException with message ""(.*)""")]
    public void GivenCouponValidationThrowsExpired(string message)
    {
        _ctx.CouponServiceMock
            .Setup(s => s.IsCouponValid(It.IsAny<Coupon>(), It.IsAny<DateTime>()))
            .Throws(new ExpiredCouponException(message));
    }

    [Given(@"pricing service returns (.*)")]
    public void GivenPricingServiceReturns(double finalPrice)
    {
        _ctx.PricingServiceMock
            .Setup(s => s.CalculatePrice(It.IsAny<Coupon>(), It.IsAny<double>(), It.IsAny<bool>()))
            .Returns(finalPrice);
    }

    [Given("pricing service throws an unexpected exception")]
    public void GivenPricingServiceThrowsUnexpected()
    {
        _ctx.PricingServiceMock
            .Setup(s => s.CalculatePrice(It.IsAny<Coupon>(), It.IsAny<double>(), It.IsAny<bool>()))
            .Throws(new Exception("Boom"));
    }

    [Given(@"pricing service throws JsonErrorException with message ""(.*)""")]
    public void GivenPricingServiceThrowsJsonError(string message)
    {
        _ctx.PricingServiceMock
            .Setup(s => s.CalculatePrice(It.IsAny<Coupon>(), It.IsAny<double>(), It.IsAny<bool>()))
            .Throws(new JsonErrorException(message));
    }

    [When(@"I call GET {string}")]
    public async Task WhenICallGet(string path)
    {
        _ctx.Client.Should().NotBeNull();

        // Always include the coupon query parameter (even if empty/whitespace),
        // otherwise binding can fail and produce 400.
        var couponInput = _ctx.CouponInput ?? "";

        var url =
            path +
            $"?{CouponQueryParamName}={Uri.EscapeDataString(couponInput)}" +
            $"&basePrice={_ctx.BasePrice.ToString(CultureInfo.InvariantCulture)}" +
            $"&isPremium={_ctx.IsPremium.ToString().ToLowerInvariant()}";

        _ctx.Response = await _ctx.Client!.GetAsync(url);
        _ctx.ResponseBody = await _ctx.Response.Content.ReadAsStringAsync();
    }

    [Then(@"the response status should be (\d+)")]
    public void ThenTheResponseStatusShouldBe(int expectedStatus)
    {
        _ctx.Response.Should().NotBeNull();

        var actual = (int)_ctx.Response!.StatusCode;
        if (actual != expectedStatus)
        {
            throw new Exception(
                $"Expected status {expectedStatus} but got {actual}.\nResponse body:\n{_ctx.ResponseBody}"
            );
        }
    }

    [Then(@"the response body should contain ""(.*)""")]
    public void ThenResponseBodyShouldContain(string expected)
    {
        (_ctx.ResponseBody ?? "").Should().Contain(expected);
    }

    [Then(@"json bool ""(.*)"" should be (.*)")]
    public void ThenJsonBoolShouldBe(string property, string expected)
    {
        using var doc = JsonDocument.Parse(_ctx.ResponseBody!);
        var el = GetPropertyCaseInsensitive(doc.RootElement, property);

        el.ValueKind.Should().BeOneOf(JsonValueKind.True, JsonValueKind.False);

        var actual = el.GetBoolean();
        actual.Should().Be(bool.Parse(expected));
    }


    [Then(@"json number ""(.*)"" should be (-?\d+(\.\d+)?)")]
    public void ThenJsonNumberShouldBe(string property, double expected)
    {
        using var doc = JsonDocument.Parse(_ctx.ResponseBody!);
        var el = GetPropertyCaseInsensitive(doc.RootElement, property);
        el.ValueKind.Should().Be(JsonValueKind.Number);

        var actual = el.GetDouble();
        actual.Should().BeApproximately(expected, 0.0001);
    }

    [Then("coupon service should not be called")]
    public void ThenCouponServiceShouldNotBeCalled()
    {
        _ctx.CouponServiceMock.Verify(
            s => s.GetCouponByCouponCode(It.IsAny<string>()),
            Times.Never);

        _ctx.CouponServiceMock.Verify(
            s => s.IsCouponValid(It.IsAny<Coupon>(), It.IsAny<DateTime>()),
            Times.Never);
    }

    [Then("pricing service should not be called")]
    public void ThenPricingServiceShouldNotBeCalled()
    {
        _ctx.PricingServiceMock.Verify(
            s => s.CalculatePrice(It.IsAny<Coupon>(), It.IsAny<double>(), It.IsAny<bool>()),
            Times.Never);
    }

    [Then("pricing service should be called once")]
    public void ThenPricingServiceShouldBeCalledOnce()
    {
        _ctx.PricingServiceMock.Verify(
            s => s.CalculatePrice(It.IsAny<Coupon>(), It.IsAny<double>(), It.IsAny<bool>()),
            Times.Once);
    }

    [Then("coupon service lookup should be called")]
    public void ThenCouponServiceLookupShouldBeCalled()
    {
        _ctx.CouponServiceMock.Verify(
            s => s.GetCouponByCouponCode(It.Is<string>(c => c == (_ctx.CouponInput ?? ""))),
            Times.Once);
    }

    [Then("coupon validation should be called")]
    public void ThenCouponValidationShouldBeCalled()
    {
        _ctx.CouponServiceMock.Verify(
            s => s.IsCouponValid(It.IsAny<Coupon>(), It.IsAny<DateTime>()),
            Times.Once);
    }

    private static JsonElement GetPropertyCaseInsensitive(JsonElement root, string name)
    {
        if (root.TryGetProperty(name, out var exact))
            return exact;

        if (!string.IsNullOrEmpty(name))
        {
            var camel = char.ToLowerInvariant(name[0]) + name[1..];
            if (root.TryGetProperty(camel, out var camelProp))
                return camelProp;
        }

        foreach (var prop in root.EnumerateObject())
        {
            if (string.Equals(prop.Name, name, StringComparison.OrdinalIgnoreCase))
                return prop.Value;
        }

        throw new KeyNotFoundException($"Property '{name}' not found in JSON.");
    }
}
