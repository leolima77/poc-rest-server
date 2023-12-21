using RestServer.Domain.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Builder;
using RestServer.Application.Interfaces;
using RestServer.Infrastructure.Services;
using Microsoft.OpenApi.Models;

namespace RestServer
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers(opt =>
            {
                //opt.AddLogging();
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
            });

            services.Configure<Config>(Configuration.GetSection("Program"));

            services.AddTransient<ICustomerValidation, CustomerValidation>();
            services.AddTransient<ICustomerData, CustomerData>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
