    public class StressTestParams
    {
        public int ThreadCount { get; set; }
        public int RequestPerThread { get; set; }
        public string BaseUrlWithPathToPage { get; set; }
        public string? AuthToken { get; set; }
    }
