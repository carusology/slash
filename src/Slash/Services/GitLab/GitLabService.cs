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