using System.Text.RegularExpressions;

namespace StressTestApp.Models;

public class TotalReport
{
    private readonly List<ResponseInfo> _responseInfos;

    public TotalReport(List<ResponseInfo> responseInfos)
    {
        _responseInfos = responseInfos;
    }

    public double AverageResponseTime => _responseInfos.Any() ? _responseInfos.Average(x => x.ElapsedMs) : 0;
    public double MinResponseTime => _responseInfos.Any() ? _responseInfos.Min(x => x.ElapsedMs) : 0;
    public double MaxResponseTime => _responseInfos.Any() ? _responseInfos.Max(x => x.ElapsedMs) : 0;
    public double Percentile50 => CalculatePercentile(50);
    public double Percentile90 => CalculatePercentile(90);
    public double Percentile95 => CalculatePercentile(95);
    public double Percentile99 => CalculatePercentile(99);

    public double SuccessRate => CountOfRequests == 0 ? 0 :
        CountOfFailedRequests == 0 ? 1 : 1 - (double)CountOfFailedRequests / CountOfRequests;

    public int CountOfRequests => _responseInfos.Count;
    public int CountOfFailedRequests => _responseInfos.Count(x => !x.IsSuccessStatusCode);

    public Dictionary<string, int> ErrorsByStatusCode => CalculateErrorsByStatusCode();

    private Dictionary<string, int> CalculateErrorsByStatusCode()
    {
        var errorRequests = _responseInfos.Where(x => !x.IsSuccessStatusCode).GroupBy(x => x.StatusCode)
            .Select(x => new { StatusCode = x.Key, Count = x.Count() }).ToList();

        return errorRequests.ToDictionary(errorRequest => errorRequest.StatusCode, errorRequest => errorRequest.Count);
    }

    private double CalculatePercentile(double percentile)
    {
        if (!_responseInfos.Any())
        {
            return 0;
        }

        var requestsDurationSorted = this._responseInfos.Select(x => x.ElapsedMs).OrderBy(x => x).ToArray();
        var countOfRequests = requestsDurationSorted.Length;
        double n = (countOfRequests - 1) * (percentile / 100) + 1;
        if (n == 1d) return requestsDurationSorted[0];
        if (n == countOfRequests) return requestsDurationSorted[countOfRequests - 1];
        int k = (int)n;
        double d = n - k;
        return requestsDurationSorted[k - 1] + d * (requestsDurationSorted[k] - requestsDurationSorted[k - 1]);
    }

    public override string ToString()
    {
        return $"# of requests:\t\t{CountOfRequests}\n" +
               $"# of failed requests:\t{CountOfFailedRequests}\n" +
               $"Success rate:\t\t{Math.Round(SuccessRate)}\n" +
               $"AVG response time:\t{Math.Round(AverageResponseTime)}\n" +
               $"MIN response time:\t{MinResponseTime}\n" +
               $"MAX response time:\t{MaxResponseTime}\n" +
               $"50 percentile:\t\t{Math.Round(Percentile50)}\n" +
               $"90 percentile:\t\t{Math.Round(Percentile90)}\n" +
               $"95 percentile:\t\t{Math.Round(Percentile95)}\n" +
               $"99 percentile:\t\t{Math.Round(Percentile99)}\n" +
               $"Errors by status code:\n {string.Join(", ", ErrorsByStatusCode.Select(x => $"{x.Key} - {x.Value}"))}";
    }
}