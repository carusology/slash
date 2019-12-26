using System;
using System.Collections.Immutable;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Moq;
using Peddler;
using RichardSzalay.MockHttp;
using Xunit;

namespace Slash.Services.GitLab {

    public sealed class GitLabHealthCheckTests {

        private static IGenerator<HealthStatus?> failureStatusGenerator { get; }

        static GitLabHealthCheckTests() {
            failureStatusGenerator = new MaybeDefaultDistinctGenerator<HealthStatus?>(
                new NullableDistinctGenerator<HealthStatus>(
                    new EnumGenerator<HealthStatus>(
                        ImmutableHashSet.Create(
                            HealthStatus.Degraded,
                            HealthStatus.Unhealthy
                        )
                    )
                )
            );
        }

        [Fact]
        public void Constructor_NullGitLabService_ThrowsArgumentNullException() {

            // Act

            var exception = Record.Exception(
                () => new GitLabHealthCheck(null)
            );

            // Assert

            Assert.IsType<ArgumentNullException>(exception);
        }

        [Fact]
        public async Task CheckHealthAsync_SuccessfulPingIsConsideredHealthy() {

            // Arrange

            var service = MockService(pingSuccessful: true);
            var healthCheck = new GitLabHealthCheck(service);

            var context = NextHealthCheckContext();

            // Act

            var result = await healthCheck.CheckHealthAsync(context, default(CancellationToken));

            // Assert

            Assert.Equal($"{context.Registration.Name} is accessible.", result.Description);
            Assert.Null(result.Exception);
            Assert.Equal(HealthStatus.Healthy, result.Status);
        }

        [Theory]
        [InlineData(HealthStatus.Unhealthy, HealthStatus.Unhealthy)]
        [InlineData(HealthStatus.Degraded, HealthStatus.Degraded)]
        [InlineData(null, HealthStatus.Unhealthy)]
        public async Task CheckHealthAsync_UnsuccessfulPingIsConsideredUnhealthy(
            HealthStatus? failureStatus,
            HealthStatus expectedStatus) {

            // Arrange

            var service = MockService(pingSuccessful: false);
            var healthCheck = new GitLabHealthCheck(service);

            var context = NextHealthCheckContext(failureStatus);

            // Act

            var result = await healthCheck.CheckHealthAsync(context);

            // Assert

            Assert.Equal($"{context.Registration.Name} is inaccessible.", result.Description);
            Assert.Null(result.Exception);
            Assert.Equal(expectedStatus, result.Status);
        }

        [Theory]
        [InlineData(HealthStatus.Unhealthy, HealthStatus.Unhealthy)]
        [InlineData(HealthStatus.Degraded, HealthStatus.Degraded)]
        [InlineData(null, HealthStatus.Unhealthy)]
        public async Task CheckHealthAsync_ExceptionFromPingIsConsideredUnhealthy(
            HealthStatus? failureStatus,
            HealthStatus expectedStatus) {

            // Arrange

            var service = new Mock<IGitLabService>();
            var expectedException = new Exception("OBJECTION!");

            service
                .Setup(m => m.PingAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(expectedException);

            var healthCheck = new GitLabHealthCheck(service.Object);

            var context = NextHealthCheckContext(failureStatus);

            // Act

            var result = await healthCheck.CheckHealthAsync(context, default(CancellationToken));

            // Assert

            Assert.Equal(
                $"Exception thrown when attempting to access {context.Registration.Name}.",
                result.Description
            );

            Assert.Equal(expectedException, result.Exception);
            Assert.Equal(expectedStatus, result.Status);
        }

        private static IGitLabService MockService(bool pingSuccessful) {
            var mock = new Mock<IGitLabService>();

            mock.Setup(m => m.PingAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new PingResult(isSuccessful: pingSuccessful)));

            return mock.Object;
        }

        private static HealthCheckContext NextHealthCheckContext() {
            return NextHealthCheckContext(failureStatusGenerator.Next());
        }

        private static HealthCheckContext NextHealthCheckContext(HealthStatus? failureStatus) {
            return new HealthCheckContext {
                Registration = new HealthCheckRegistration(
                    name: "Fake GitLab Registration",
                    instance: Mock.Of<IHealthCheck>(),
                    failureStatus: failureStatus,
                    tags: null
                )
            };
        }

    }

}