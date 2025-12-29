using Reqnroll;

namespace SoftwareQualityExamProject.BddTests.Support;

[Binding]
public class Hooks
{
    private readonly PricingTestContext _ctx;

    public Hooks(PricingTestContext ctx)
    {
        _ctx = ctx;
    }

    [BeforeScenario]
    public void BeforeScenario()
    {
        _ctx.Reset();
    }

    [AfterScenario]
    public void AfterScenario()
    {
        _ctx.Dispose();
    }
}
