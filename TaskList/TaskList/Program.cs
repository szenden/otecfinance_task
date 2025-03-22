using Microsoft.Extensions.DependencyInjection;
using TaskList;
using TaskList.Core.Models;
using TaskList.Core.Processing;
using TaskList.Core.Services;
using TaskList.Interfaces.Console;

/// <summary>
/// Main program class for the TaskList application.
/// Provides entry points for both console and web interfaces.
/// </summary>
partial class Program
{
    /// <summary>
    /// The main entry point for the application.
    /// Determines whether to run as a console application or web application based on command line arguments.
    /// </summary>
    /// <param name="args">Command line arguments. Use "--cli" to run in console mode.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    static async Task Main(string[] args)
    {
        var serviceProvider = ConfigureServices();

        // Check if running as console or web
        if (args.Length > 0 && args[0] == "--cli")
        {
            await RunConsoleAppAsync(serviceProvider);
        }
        else
        {
            RunWebApp(args);
        }
    }

    /// <summary>
    /// Configures the dependency injection container with required services.
    /// </summary>
    /// <returns>A service provider containing all registered services.</returns>
    static IServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();

        // Register services
        services.AddSingleton<ITaskService, TaskService>();
        services.AddSingleton<ICommandProcessor, CommandProcessor>();
        services.AddSingleton<IConsole, RealConsole>();
        services.AddSingleton<ConsoleTaskList>();

        return services.BuildServiceProvider();
    }

    /// <summary>
    /// Runs the application in console mode.
    /// </summary>
    /// <param name="serviceProvider">The service provider containing required services.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    static async Task RunConsoleAppAsync(IServiceProvider serviceProvider)
    {
        var consoleTaskList = serviceProvider.GetRequiredService<ConsoleTaskList>();
        await consoleTaskList.RunAsync();
    }

    /// <summary>
    /// Runs the application in web mode.
    /// Configures and starts the ASP.NET Core web application with Swagger documentation.
    /// </summary>
    /// <param name="args">Command line arguments for the web application.</param>
    static void RunWebApp(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddControllers();
        builder.Services.AddSingleton<ITaskService, TaskService>();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();

        app.Run();
    }
}
