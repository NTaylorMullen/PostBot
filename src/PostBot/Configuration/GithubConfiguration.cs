namespace PostBot.Configuration
{
    public class GithubConfiguration : MonitorConfiguration
    {
        public string Endpoint { get; set; }
        public string Token { get; set; }
        public string Organization { get; set; }
    }
}
