using System;
using System.Diagnostics;
using System.IO;
using System.ServiceProcess;
using System.Threading;
using Microsoft.Extensions.Configuration;
using PostBot.Configuration;
using PostBot.Monitors.GitHub;
using PostBot.Monitors.Internal;
using PostBot.Slack;

namespace ConsoleApplication
{
    public class Program : ServiceBase
    {
        private GitHubMonitor _gitHubMonitor;

        public Program()
        {
            CanShutdown = true;
            CanStop = true;
            CanPauseAndContinue = true;
        }

        public static void Main()
        {
            var runAsConsole = Environment.GetEnvironmentVariable("RUN_AS_CONSOLE");

            if (runAsConsole == null ||
                runAsConsole == "0" ||
                string.IsNullOrWhiteSpace(runAsConsole))
            {
                Run(new Program());
            }
            else
            {
                using (var program = new Program())
                {
                    program.OnStart(null);

                    Console.ReadLine();
                    program.Dispose();
                }
            }
        }

        protected override void OnStart(string[] args)
        {
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
            var configuration = new ApplicationConfiguration();
            new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddUserSecrets()
                .Build()
                .Bind(configuration);

            var slackClient = new SlackClient(configuration.Slack);
            _gitHubMonitor = new GitHubMonitor(slackClient, configuration.GitHub);
        }

        protected override void OnStop()
        {
            _gitHubMonitor?.Dispose();
        }

        protected override void OnPause()
        {
            OnStop();
        }

        protected override void OnContinue()
        {
            OnStart(null);
        }

        protected override void OnShutdown()
        {
            OnStop();
        }

        protected override void Dispose(bool disposing)
        {
            OnStop();

            base.Dispose(disposing);
        }
    }
}
