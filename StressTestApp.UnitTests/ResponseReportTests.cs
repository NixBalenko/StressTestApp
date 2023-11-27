using System.Collections.Generic;
using FluentAssertions;
using StressTestApp.Models;

namespace StressTestApp.UnitTests;

public abstract class ResponseReportTests
{
    protected TotalReport TotalReport;
    protected List<ResponseInfo> Responses = new();

    protected void ThenErrorsShouldBe(Dictionary<string, int> expectedValue)
    {
        TotalReport.ErrorsByStatusCode.Should().BeEquivalentTo(expectedValue);
    }

    protected void ThenParamShouldBe(double param, double expectedValue)
    {
        param.Should().Be(expectedValue);
    }
    protected void GivenResponse(ResponseInfo response)
    {
        Responses.Add(response);
    }
    
    protected void GivenSuccessResponse()
    {
        Responses.Add(new ResponseInfo()
        {
            StatusCode = "200",
            IsSuccessStatusCode = true,
            ElapsedMs = 100
        });
    }
    
    protected void GivenErrorResponse(string errorCode)
    {
        Responses.Add(new ResponseInfo()
        {
            StatusCode = errorCode,
            IsSuccessStatusCode = false,
            ElapsedMs = 100
        });
    }
    
    protected void WhenGenerateReport()
    {
        TotalReport = new TotalReport(Responses);
    }
}