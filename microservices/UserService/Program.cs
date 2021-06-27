using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using DatabaseLayer;

namespace UserService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            MongoDBLayer.InitializeDefaultDatabase();
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseConfiguration(new ConfigurationBuilder().AddCommandLine(args).Build()) // Support command line args
                .Build();
        }
    }
}
