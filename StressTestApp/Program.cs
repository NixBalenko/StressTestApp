using System.Collections.Concurrent;
using System.Diagnostics;
using StressTestApp.Models;

class Program
{
    static readonly HttpClient httpClient = new HttpClient();
    static readonly int threads = 2;
    static readonly int requestPerThread = 3;

    static async Task Main(string[] args)
    {
        string apiGWDeal = "https://3ickvifyzf.execute-api.us-east-2.amazonaws.com/epam-igm-qa/v1/deals?per=1&page=";
        var baseUrl = apiGWDeal;
        httpClient.DefaultRequestHeaders.Add("Authorization", "Token token=aea04f79bd8548644c46f448f630b807, company_key=074e3915-dd7a-40a0-84cd-3bebe93447a8");
        httpClient.DefaultRequestHeaders.Add("x-api-key", "Token token=aea04f79bd8548644c46f448f630b807, company_key=074e3915-dd7a-40a0-84cd-3bebe93447a8");

        var allTasks = new List<Task>();
        var times = new List<long>();
        var requestStatistics = new ConcurrentBag<ResponseInfo>();
        for (var threadNumber = 1; threadNumber <= threads; threadNumber++)
        {
            for (int i = 1; i <= requestPerThread; i++)
            {
                allTasks.Add(ProcessUrlAsync(baseUrl, threadNumber, i, times,requestStatistics));
            }
        }

        await Task.WhenAll(allTasks);

        double avg = times.Count > 0 ? (double)times.Sum() / times.Count : 0;
        Console.WriteLine($"Average Time = {avg} ms");
        Console.WriteLine("----------Finish!----------");
        Console.WriteLine($"Number of threads: {threads}");
        Console.WriteLine("----------Logs!----------");
        foreach (var requests in requestStatistics)
        {
            Console.WriteLine(requests.ToString());
        }
    }

    static async Task ProcessUrlAsync(string baseUrl, int threadNumber, int iteration, List<long> times,
        ConcurrentBag<ResponseInfo> requestStatistics)
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

            lock (times)
            {
                times.Add(time);
            }
            requestStatistics.Add(new ResponseInfo(response, time, threadNumber, iteration, pageNum));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"{threadNumber} {iteration} {url} failed: {ex.Message}");
        }
    }
}
