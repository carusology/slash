using System;
using Invio.Immutable;

namespace Slash.Services.GitLab {

    public sealed class PingResult : ImmutableBase<PingResult> {

        public bool IsSuccessful { get; }

        public PingResult(bool isSuccessful = false) {
            this.IsSuccessful = isSuccessful;
        }

    }

}