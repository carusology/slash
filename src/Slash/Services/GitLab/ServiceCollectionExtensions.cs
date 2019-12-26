using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

namespace Slash.Services.GitLab {

    /// <summary>
    ///   When the GitLab proxy is enabled, this configures how the slash
    ///   commands from Slack will behave when proxied through this web service.
    /// </summary>
    public static class ServiceCollectionExtensions {

        /// <summary>
        ///   Enables the web service to proxy slash commands from Slack
        ///   to an instance of GitLab.
        /// </summary>
        /// <param name="services">
        ///   The <see cref="IServiceCollection" /> object being used to define
        ///   how services will be hydrated for this web service.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///   Thrown when <paramref name="services" /> is null.
        /// </exception>
        public static IServiceCollection AddGitLabProxy(this IServiceCollection services) {
            return AddGitLabProxy(services, (GitLabConfiguration config) => {});
        }

        /// <summary>
        ///   Enables the web service to proxy slash commands from Slack
        ///   to an instance of GitLab.
        /// </summary>
        /// <param name="services">
        ///   The <see cref="IServiceCollection" /> object being used to define
        ///   how services will be hydrated for this web service.
        /// </param>
        /// <param name="configureOptions">
        ///   An action used to configure how the GitLab proxy should
        ///   behave as it services request from Slack.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///   Thrown when <paramref name="services" /> or <see cref="configure" /> is null.
        /// </exception>
        public static IServiceCollection AddGitLabProxy(
            this IServiceCollection services,
            Action<GitLabConfiguration> configure) {

            if (services == null) {
                throw new ArgumentNullException(nameof(services));
            } else if (configure == null) {
                throw new ArgumentNullException(nameof(configure));
            }

            services.Configure<GitLabConfiguration>(configure);

            services.AddSingleton<
                IPostConfigureOptions<GitLabConfiguration>,
                GitLabConfigurationVerifier>();

            services.AddHttpClient<GitLabService>();
            services.AddTransient<IGitLabService>(
                provider => provider.GetRequiredService<GitLabService>()
            );

            services.Configure<HealthCheckServiceOptions>(
                options => options.Registrations.Add(
                    new HealthCheckRegistration(
                        name: "GitLab",
                        factory: provider =>
                            new GitLabHealthCheck(provider.GetRequiredService<IGitLabService>()),
                        failureStatus: null,
                        tags: null
                    )
                )
            );

            return services;
        }

    }

}