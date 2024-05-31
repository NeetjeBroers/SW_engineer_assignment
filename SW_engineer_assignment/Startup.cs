using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System;

[assembly: FunctionsStartup(typeof(SW_engineer_assignment.Startup))]

namespace SW_engineer_assignment
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddSwaggerGen(item =>
            {
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
