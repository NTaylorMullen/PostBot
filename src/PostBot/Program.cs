using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using PostBot.Configuration;
using PostBot.Monitors.GitHub;
using PostBot.Slack;

namespace ConsoleApplication
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var configuration = new ApplicationConfiguration();
            new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddUserSecrets()
                .Build()
                .Bind(configuration);
            var slackClient = new SlackMessageClient(configuration.Slack);

            using (var monitor = new GitHubMonitor(slackClient, configuration.GitHub))
            {
                Console.WriteLine("Press any key to exit...");
                Console.ReadLine();
            }
        }
    }
}
