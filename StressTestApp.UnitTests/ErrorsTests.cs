using System.Collections.Generic;
using StressTestApp.Models;
using TestStack.BDDfy;
using TestStack.BDDfy.Xunit;

namespace StressTestApp.UnitTests;

public class ErrorsTests : ResponseReportTests
{
    [BddfyFact]
    public void SuccessRate_NoRequests()
    {
        this.When(x => x.WhenGenerateReport())
            .Then(x => x.ThenParamShouldBe(TotalReport.SuccessRate, 0))
            .Then(x => x.ThenParamShouldBe(TotalReport.CountOfRequests, 0))
            .Then(x => x.ThenParamShouldBe(TotalReport.CountOfFailedRequests, 0))
            .BDDfy();
    }

    [BddfyFact]
    public void SuccessRate_OneRequest_NoError()
    {
        this.Given(x => x.GivenSuccessResponse())
            .When(x => x.WhenGenerateReport())
            .Then(x => x.ThenParamShouldBe(TotalReport.SuccessRate, 1))
            .Then(x => x.ThenParamShouldBe(TotalReport.CountOfRequests, 1))
            .Then(x => x.ThenParamShouldBe(TotalReport.CountOfFailedRequests, 0))
            .Then(x => x.ThenErrorsShouldBe(new Dictionary<string, int>()))
            .BDDfy();
    }

    [BddfyFact]
    public void SuccessRate_OneRequest_OneError()
    {
        this.Given(x => x.GivenErrorResponse("500"))
            .When(x => x.WhenGenerateReport())
            .Then(x => x.ThenParamShouldBe(TotalReport.SuccessRate, 0))
            .Then(x => x.ThenParamShouldBe(TotalReport.CountOfRequests, 1))
            .Then(x => x.ThenParamShouldBe(TotalReport.CountOfFailedRequests, 1))
            .Then(x => x.ThenErrorsShouldBe(new Dictionary<string, int>{{ "500", 1 }
            }))
            .BDDfy();
    }

    [BddfyFact]
    public void SuccessRate_MultipleRequests_MultipleError()
    {
        this.Given(x => x.GivenSuccessResponse())
            .Given(x => x.GivenErrorResponse("500"))
            .Given(x => x.GivenErrorResponse("500"))
            .Given(x => x.GivenErrorResponse("429"))
            .When(x => x.WhenGenerateReport())
            .Then(x => x.ThenParamShouldBe(TotalReport.SuccessRate, 0.25))
            .Then(x => x.ThenParamShouldBe(TotalReport.CountOfRequests, 4))
            .Then(x => x.ThenParamShouldBe(TotalReport.CountOfFailedRequests, 3))
            .Then(x => x.ThenErrorsShouldBe(new Dictionary<string, int>()
            {
                { "500",2 },
                { "429",1 }
            }))
            .BDDfy();
    }
}