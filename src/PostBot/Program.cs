using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using PostBot.Configuration;
using PostBot.Monitors.GitHub;

namespace ConsoleApplication
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var configuration = new Configuration();
            new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddUserSecrets()
                .Build()
                .Bind(configuration);

            var monitor = new GitHubMonitor(configuration.GitHub);

            Console.ReadLine();
        }
    }
}
