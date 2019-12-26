using System;
using System.Threading;
using System.Threading.Tasks;

namespace Slash.Services.GitLab {

    /// <summary>
    ///   An abstraction for interactions with an instance of GitLab.
    /// </summary>
    /// <remarks>
    ///   This is a glorified wrapper around a <see cref="HttpClient" /> that
    ///     (1) is configured to interact with GitLab
    ///     (2) is efficiently cached within the <see cref="IServiceProvider" />
    ///     (3) provides of a clean abstraction layer for well known endpoints.
    /// </remarks>
    public interface IGitLabService {

        /// <summary>
        ///   Asynchronously checks to see if the instance of the GitLab that Slash
        ///   has been configured to interact with is actually there.
        /// </summary>
        /// <param name="cancellationToken">
        ///   Optional. Pass to provide the ability to cancel the asynchronous request
        ///   after it has been initiated.
        /// </param>
        /// <returns>
        ///   A <see cref="PingResult" /> which describes the result of the ping request.
        /// </returns>
        Task<PingResult> PingAsync(CancellationToken cancellationToken = default(CancellationToken));

    }

}