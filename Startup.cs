using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

using TodoApi.Models;

namespace TodoApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddLogging(builder =>
               {
                   builder.AddConsole(); // Add console logger
               });
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<TenantSpecificDbContextFactory>(provider =>
{
    var options = provider.GetRequiredService<DbContextOptions<TodoContext>>();
    var httpContextAccessor = provider.GetRequiredService<IHttpContextAccessor>();
    var logger = provider.GetRequiredService<ILogger<Startup>>();
    var tenantHeader = httpContextAccessor.HttpContext.Request.Headers["Tenant"].ToString();
    //logger.LogInformation("This is a tenantHeader - " + tenantHeader);
    return new TenantSpecificDbContextFactory(options, tenantHeader);
});

            services.AddScoped<TodoContext>(provider =>
            {
                var factory = provider.GetRequiredService<TenantSpecificDbContextFactory>();
                var logger = provider.GetRequiredService<ILogger<Startup>>();
                logger.LogInformation("This is a schema - " + factory._schema);
                return factory.Create();
            });

            services.AddDbContext<TodoContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("MyConnectionString")));




            services.AddControllers();


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            // Add the TenantMiddleware before authorization and endpoint mapping
            app.UseTenantMiddleware();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
