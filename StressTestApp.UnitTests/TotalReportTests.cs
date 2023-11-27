using StressTestApp.Models;
using TestStack.BDDfy;
using TestStack.BDDfy.Xunit;

namespace StressTestApp.UnitTests;

public class TotalReportTests : ResponseReportTests
{
    [BddfyFact]
    public void AvgCalculation_HappyPath()
    {
        this.Given(x => x.GivenResponse(new ResponseInfo(100)))
            .Given(x => x.GivenResponse(new ResponseInfo(200)))
            .When(x => x.WhenGenerateReport())
            .Then(x => x.ThenParamShouldBe(TotalReport.AverageResponseTime, 150))
            .BDDfy();
    }

    [BddfyFact]
    public void AvgCalculation_NoResponses()
    {
        this.When(x => x.WhenGenerateReport())
            .Then(x => x.ThenParamShouldBe(TotalReport.AverageResponseTime, 0))
            .BDDfy();
    }

    [BddfyFact]
    public void MaxCalculation_HappyPath()
    {
        this.Given(x => x.GivenResponse(new ResponseInfo(100)))
            .Given(x => x.GivenResponse(new ResponseInfo(200)))
            .When(x => x.WhenGenerateReport())
            .Then(x => x.ThenParamShouldBe(TotalReport.MaxResponseTime, 200))
            .BDDfy();
    }
    
    [BddfyFact]
    public void MinCalculation_HappyPath()
    {
        this.Given(x => x.GivenResponse(new ResponseInfo(100)))
            .Given(x => x.GivenResponse(new ResponseInfo(200)))
            .When(x => x.WhenGenerateReport())
            .Then(x => x.ThenParamShouldBe(TotalReport.MinResponseTime, 100))
            .BDDfy();
    }
    
    [BddfyFact]
    public void Perc50_2Requests_HappyPath()
    {
        this.Given(x => x.GivenResponse(new ResponseInfo(100)))
            .Given(x => x.GivenResponse(new ResponseInfo(200)))
            .When(x => x.WhenGenerateReport())
            .Then(x => x.ThenParamShouldBe(TotalReport.Percentile50, 150))
            .BDDfy();
    }
    
    [BddfyFact]
    public void Perc50_3Requests_HappyPath()
    {
        this.Given(x => x.GivenResponse(new ResponseInfo(100)))
            .Given(x => x.GivenResponse(new ResponseInfo(200)))
            .Given(x => x.GivenResponse(new ResponseInfo(300)))
            .When(x => x.WhenGenerateReport())
            .Then(x => x.ThenParamShouldBe(TotalReport.Percentile50, 200))
            .BDDfy();
    }
    
    [BddfyFact]
    public void AllPercentiles_2Requests_HappyPath()
    {
        this.Given(x => x.GivenResponse(new ResponseInfo(100)))
            .Given(x => x.GivenResponse(new ResponseInfo(200)))
            .Given(x => x.GivenResponse(new ResponseInfo(300)))
            .When(x => x.WhenGenerateReport())
            .Then(x => x.ThenParamShouldBe(TotalReport.Percentile50, 200))
            .Then(x => x.ThenParamShouldBe(TotalReport.Percentile90, 280.0))
            .Then(x => x.ThenParamShouldBe(TotalReport.Percentile95, 290.0))
            .Then(x => x.ThenParamShouldBe(TotalReport.Percentile99, 298.0))
            .BDDfy();
    }
}