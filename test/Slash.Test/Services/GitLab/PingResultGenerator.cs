using System;
using Peddler;

namespace Slash.Services.GitLab {

    public sealed class PingResultGenerator : IGenerator<PingResult> {

        private static IGenerator<bool> isSuccessfulGenerator { get; }

        static PingResultGenerator() {
            isSuccessfulGenerator = new BooleanGenerator();
        }

        public PingResult Next() {
            return new PingResult(
                isSuccessful: isSuccessfulGenerator.Next()
            );
        }

    }

}