namespace TestWebJob.Utils
{
    public class AppSettings
    {
        public Filters Filters { get; set; }
    }

    public class Filters
    {
        public int TakenDays { get; set; }
    }
}