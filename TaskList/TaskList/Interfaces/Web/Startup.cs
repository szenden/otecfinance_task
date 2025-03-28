using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TaskList.Core.Repositories;
using TaskList.Core.UnitOfWork;
using TaskList.Core.Validation;
using FluentValidation.AspNetCore;

namespace TaskList.Interfaces.Web
{
    /// <summary>
    /// Startup class that configures services and the application's request pipeline
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Initializes a new instance of the Startup class
        /// </summary>
        /// <param name="configuration">Application configuration</param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// Gets the application configuration
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// Configures the application services
        /// </summary>
        /// <param name="services">The service collection to configure</param>
        public void ConfigureServices(IServiceCollection services)
        {
            // Add MVC controllers with FluentValidation
            services.AddControllers()
                .AddFluentValidation(fv =>
                {
                    fv.RegisterValidatorsFromAssemblyContaining<ProjectValidator>();
                });

            // Register application services
            services.AddScoped<ITaskListRepository, TaskListRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Configure CORS policies
            services.AddCors(options =>
            {
                options.AddPolicy("DefaultPolicy", builder =>
                {
                    builder.WithOrigins(Configuration.GetSection("AllowedOrigins").Get<string[]>())
                           .WithMethods("GET", "POST", "PUT", "DELETE", "OPTIONS")
                           .WithHeaders("Authorization", "Content-Type")
                           .AllowCredentials();
                });
            });
        }

        /// <summary>
        /// Configures the application request pipeline
        /// </summary>
        /// <param name="app">The application builder</param>
        /// <param name="env">The web hosting environment</param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Configure error handling based on environment
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            // Enable HTTPS redirection and static files
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            // Add security headers middleware
            app.Use(async (context, next) =>
            {
                // Configure various security headers
                context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
                context.Response.Headers.Add("X-Frame-Options", "DENY");
                context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
                context.Response.Headers.Add("Referrer-Policy", "strict-origin-when-cross-origin");
                context.Response.Headers.Add("Content-Security-Policy",
                    "default-src 'self'; " +
                    "script-src 'self' 'unsafe-inline' 'unsafe-eval'; " +
                    "style-src 'self' 'unsafe-inline'; " +
                    "img-src 'self' data: https:; " +
                    "font-src 'self'; " +
                    "connect-src 'self'");

                await next();
            });

            // Configure the HTTP request pipeline
            app.UseRouting();
            app.UseCors("DefaultPolicy");
            app.UseAuthorization();

            // Configure endpoints
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}