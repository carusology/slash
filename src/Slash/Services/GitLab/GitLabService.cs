using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace Slash.Services.GitLab {

    /// <summary>
    ///   The "real" implementation of the <see cref="IGitLabService" /> that
    ///   makes calls using <see cref="HttpClient" /> to the relevant
    ///   installation of GitLab, be it self-hosted or GitLab.com.
    /// </summary>
    public sealed class GitLabService : IGitLabService {

        private HttpClient client { get; }

        public GitLabService(
            HttpClient client,
            IOptions<GitLabConfiguration> configuration) {

            if (client == null) {
                throw new ArgumentNullException(nameof(client));
            } else if (configuration == null) {
                throw new ArgumentNullException(nameof(configuration));
            }

            this.client = client;
            this.client.BaseAddress = configuration.Value.BaseUrl;
        }

        /// <summary>
        ///   Performs a GET HTTP request to the location configured via the
        ///   <see cref="GitLabConfiguration.BaseUrl" /> property to determine
        ///   if is reachable.
        /// </summary>
        /// <remarks>
        ///   The response from the request must be a 2XX HTTP status code in order
        ///   for the ping to be considered "successful."
        /// </remarks>
        /// <param name="cancellationToken">
        ///   Optional. Pass to provide the ability to cancel the asynchronous request
        ///   after it has been initiated.
        /// </param>
        /// <returns>
        ///   A <see cref="PingResult" /> that contains data on whether or not
        ///   the ping was considered successful.
        /// </returns>
        public async Task<PingResult> PingAsync(
            CancellationToken cancellationToken = default(CancellationToken)) {

            var response = await
                this.client
                    .GetAsync("", HttpCompletionOption.ResponseHeadersRead, cancellationToken)
                    .ConfigureAwait(false);

            return new PingResult(isSuccessful: response.IsSuccessStatusCode);
        }

    }

}