namespace StressTestApp.Models;

public class ResponseInfo
{
    public ResponseInfo()
    {
    }
    public ResponseInfo(double elapsedMs)
    {
        ElapsedMs = elapsedMs;
    }

    public ResponseInfo(HttpResponseMessage responseMessage, double elapsedMs, int threadNumber, int requestNumber, int page=0)
    {
        ThreadNumber = threadNumber;
        RequestNumber = requestNumber;
        ElapsedMs = elapsedMs;
        Url = responseMessage?.RequestMessage?.RequestUri?.ToString();
        Page = page;
        StatusCode = responseMessage?.StatusCode.ToString();
        IsSuccessStatusCode = responseMessage?.IsSuccessStatusCode??false;
    }
    
    public int ThreadNumber { get; set; }
    public int RequestNumber { get; set; }
    public double ElapsedMs { get; set; }
    public string? Url { get; set; }
    public int? Page { get; set; }
    public string? StatusCode { get; set; }
    public bool IsSuccessStatusCode { get; set; }

    public override string ToString()
    {
        return
            $"Thread:{ThreadNumber}\tRequest:{RequestNumber}\tPage:{Page}\t\tCode:{StatusCode}\t\tTime:{ElapsedMs}\tUrl:{Url}";
    }
}