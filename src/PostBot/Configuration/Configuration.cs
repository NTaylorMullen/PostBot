namespace PostBot.Configuration
{
    public class Configuration
    {
        public GithubConfiguration GitHub { get; set; }

        public TeamCityConfiguration TeamCity { get; set; }

        public StackOverflowConfiguration StackOverflow { get; set; }

        public AspNetForumConfiguration AspNetForums { get; set; }
    }
}
