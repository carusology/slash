using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Slash.Services.GitLab;

namespace Slash {

    public sealed class Startup {

        private IConfiguration globalConfiguration { get; }

        public Startup(IConfiguration globalConfiguration) {
            this.globalConfiguration = globalConfiguration;
        }

        public void ConfigureServices(IServiceCollection services) {
            services.AddHealthChecks();
            services.AddGitLabProxy(ConfigureGitLabProxy);
        }

        private void ConfigureGitLabProxy(GitLabConfiguration gitLabConfiguration) {
            this.globalConfiguration
                .GetSection("GitLab")
                .Bind(gitLabConfiguration);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/health");
            });
        }

    }

}
