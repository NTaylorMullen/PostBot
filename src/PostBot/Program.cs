using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using PostBot.Configuration;
using PostBot.Monitors.GitHub;
using PostBot.Posters;

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
            var slackClient = new SlackClient(configuration);

            var monitor = new GitHubMonitor(slackClient, configuration.GitHub);

            Console.ReadLine();
        }
    }
}
