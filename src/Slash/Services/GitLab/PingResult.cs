using System;
using Invio.Immutable;

namespace Slash.Services.GitLab {

    public sealed class PingResult : ImmutableBase<PingResult> {

        /// <summary>
        ///   Whether or not the request to "ping" the instance of GitLab
        ///   was successful.
        /// </summary>
        public bool IsSuccessful { get; }

        public PingResult(bool isSuccessful = false) {
            this.IsSuccessful = isSuccessful;
        }

    }

}