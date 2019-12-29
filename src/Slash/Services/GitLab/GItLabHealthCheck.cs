using System;
using System.Collections.Immutable;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Slash.Services.GitLab {

    /// <summary>
    ///   Basic health check to ensure that the endpoint specified
    ///   for the GitLab instance being proxied can be reached.
    /// </summary>
    public sealed class GitLabHealthCheck : IHealthCheck {

        private IGitLabService service { get; }

        public GitLabHealthCheck(IGitLabService service) {
            if (service == null) {
                throw new ArgumentNullException(nameof(service));
            }

            this.service = service;
        }

        /// <summary>
        ///   Asynchronously checks to see if the GitLab is currently accessible.
        /// </summary>
        /// <param name="context">
        ///   General information on how this health check was registered.
        /// </param>
        /// <param name="cancellationToken">
        ///   Optional. Pass to provide the ability to cancel the asynchronous request
        ///   after it has been initiated.
        /// </param>
        /// <returns>
        ///   Whether or not GitLab is reachable in
        ///   the form of a <see cref="HealthCheckResult" />.
        /// </returns>
        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default(CancellationToken)) {

            try {
                var result = await this.service.PingAsync(cancellationToken).ConfigureAwait(false);

                if (result.IsSuccessful) {
                    return HealthCheckResult.Healthy(
                        $"{context.Registration.Name} is accessible."
                    );
                }

                return new HealthCheckResult(
                    context.Registration.FailureStatus,
                    $"{context.Registration.Name} is inaccessible."
                );
            } catch (Exception exception) {
                return new HealthCheckResult(
                    context.Registration.FailureStatus,
                    $"Exception thrown when attempting to access {context.Registration.Name}.",
                    exception
                );
            }
        }

    }

}