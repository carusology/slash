using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Slash {

    public sealed class Program {

        private static IDictionary<string, string> defaults { get; }

        static Program() {
            defaults =
                ImmutableDictionary<string, string>
                    .Empty
                    .Add(WebHostDefaults.EnvironmentKey, "local");
        }

        public static void Main(string[] args) {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureHostConfiguration(
                    (IConfigurationBuilder builder) => {
                        builder.AddInMemoryCollection(defaults);
                    }
                )
                .ConfigureAppConfiguration(
                    (HostBuilderContext context, IConfigurationBuilder builder) => {
                        var environment = context.HostingEnvironment.EnvironmentName;

                        builder.AddInMemoryCollection(defaults);
                        builder.AddJsonFile($"config/appsettings.json", optional: false, reloadOnChange: true);
                        builder.AddJsonFile($"config/{environment}/appsettings.json", optional: true, reloadOnChange: true);
                    }
                )
                .ConfigureWebHostDefaults(
                    (IWebHostBuilder builder) => {
                        builder.UseStartup<Startup>();
                    }
                );

    }

}
