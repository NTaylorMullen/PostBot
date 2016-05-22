using System;
using Microsoft.Extensions.Configuration;
using PostBot.Configuration;

namespace ConsoleApplication
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var configuration = new Configuration();
            new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build()
                .Bind(configuration);
        }
    }
}
