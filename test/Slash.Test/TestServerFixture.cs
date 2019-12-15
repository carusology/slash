using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Slash {

    public sealed class TestServerFixture : WebApplicationFactory<Startup> {

        protected override IHostBuilder CreateHostBuilder() =>
            Program
                .CreateHostBuilder(new string[0])
                .ConfigureLogging(builder => builder.ClearProviders());

    }

}