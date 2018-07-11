using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace dotnet_core_app
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IGreeter, Greeter>();
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, 
                              IHostingEnvironment env,
                              IGreeter greeter,
                              ILogger<Startup> logger)
        {
            if (env.IsDevelopment())
            {
                // Stack trace for developers when exceptions are thrown in server
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();

            app.UseMvcWithDefaultRoute();

            app.Use(next =>
            {
                // The middleware invoked once per HTTP request
                return async context =>
                {
                    logger.LogInformation("Request incoming");
                    if (context.Request.Path.StartsWithSegments("/mym"))
                    {
                        await context.Response.WriteAsync("Hit!");
                        logger.LogInformation("Request handled");
                    }
                    else
                    {
                        // Going backwards in the pipeline
                        await next(context);
                        logger.LogInformation("Response outgoing");
                    }

                };
            });

            app.UseWelcomePage(new WelcomePageOptions
            {
                Path="/wp"
            });

            app.Run(async (context) =>
            {
                var greeting = greeter.GetMessageOfTheDay();
                await context.Response.WriteAsync($"{greeting} : {env.EnvironmentName}");
            });
        }
    }
}
