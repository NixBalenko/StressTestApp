using System.Collections.Concurrent;
using System.Diagnostics;
using StressTestApp.Models;

partial class Program
{
    static readonly HttpClient httpClient = new HttpClient();

    static async Task Main(string[] args)
    {
        var stressTestParams = new StressTestParams
        {
            ThreadCount = 5,
            RequestPerThread = 10,
            BaseUrlWithPathToPage = "http://epam-igm-qa-orch-service-alb-1240739889.us-east-2.elb.amazonaws.com/api/v1/stories?per=1&page=",
            AuthToken = "Token token=aa601ab42270edc4433f878cac861e50, company_key=8bc9f1b2-b862-48f3-90a6-7b0391899ec6"
        };
  
        var datetimeStart = DateTime.Now;
        var requestStatistics = await RunStressTest(stressTestParams);
        var datetimeEnd = DateTime.Now;
        
        var totalReport = new TotalReport(requestStatistics.ToList());
        PrintTotalReport(requestStatistics, datetimeStart, datetimeEnd, totalReport, stressTestParams);
    }

    private static async Task<ConcurrentBag<ResponseInfo>> RunStressTest(StressTestParams stressTestParams)
    {
        httpClient.DefaultRequestHeaders.Add("Authorization", stressTestParams.AuthToken);
        var allTasks = new List<Task>();
        var requestStatistics = new ConcurrentBag<ResponseInfo>();
        for (var threadNumber = 1; threadNumber <= stressTestParams.ThreadCount; threadNumber++)
        {
            for (var i = 1; i <= stressTestParams.RequestPerThread; i++)
            {
                allTasks.Add(ProcessUrlAsync(stressTestParams.BaseUrlWithPathToPage, threadNumber, i, requestStatistics, stressTestParams.ThreadCount));
            }
        }
        await Task.WhenAll(allTasks);

        return requestStatistics;
    }

    private static void PrintTotalReport(ConcurrentBag<ResponseInfo> requestStatistics, DateTime datetimeStart, DateTime datetimeEnd,
        TotalReport totalReport, StressTestParams stressTestParams)
    {
        Console.WriteLine("----------Finish!----------");
        Console.WriteLine("----------Logs!----------");
        foreach (var requests in requestStatistics.Where(x=>!x.IsSuccessStatusCode))
        {
            Console.WriteLine(requests.ToString());
        }
        Console.WriteLine("----------Statistic!----------");
        Console.WriteLine("# of threads:\t\t" + stressTestParams.ThreadCount);
        Console.WriteLine("# in one thread:\t" + stressTestParams.RequestPerThread);
        Console.WriteLine("Start:\t" + datetimeStart);
        Console.WriteLine("End:\t" + datetimeEnd);
        Console.WriteLine(totalReport);
    }

    static async Task ProcessUrlAsync(string baseUrl, int threadNumber, int iteration, 
        ConcurrentBag<ResponseInfo> requestStatistics, int threads = 10)
    {
        int pageNum = (threadNumber - 1) * threads * 10 + iteration;
        string url = baseUrl + pageNum;
        Console.WriteLine($"{threadNumber} {iteration} {url} - started");
        
        Stopwatch stopwatch = Stopwatch.StartNew();

        try
        {
            var response = await httpClient.GetAsync(url);
            stopwatch.Stop();
            var time = stopwatch.ElapsedMilliseconds;

            Console.WriteLine(response.IsSuccessStatusCode
                ? $"{threadNumber} {iteration} {url} - finished: {response.StatusCode} in {time} ms"
                : $"{threadNumber} {iteration} {url} - failed with status code: {response.StatusCode} in {time} ms");

            requestStatistics.Add(new ResponseInfo(response, time, threadNumber, iteration, pageNum));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"{threadNumber} {iteration} {url} failed: {ex.Message}");
        }
    }
}
