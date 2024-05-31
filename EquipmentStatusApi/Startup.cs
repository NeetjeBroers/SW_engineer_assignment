using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

// Assembly attribute to specify the startup class for the Azure Functions
[assembly: FunctionsStartup(typeof(EquipmentStatusApi.Startup))]

namespace EquipmentStatusApi
{
    /// <summary>
    /// The Startup class for configuring services and dependencies in the Azure Functions.
    /// </summary>
    public class Startup : FunctionsStartup
    {
        /// <summary>
        /// Configures the services for the Azure Functions app.
        /// </summary>
        /// <param name="builder">The IFunctionsHostBuilder to configure services.</param>
        public override void Configure(IFunctionsHostBuilder builder)
        {
            // Adds Swagger generation services to the dependency injection container
            builder.Services.AddSwaggerGen(item =>
            {
                // Defines the Swagger document information
                item.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "EquipmentStatus API",
                    Description = "Get and Set EquipmentStatus"
                });
            });
        }
    }
}
