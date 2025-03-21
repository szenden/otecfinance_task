using Microsoft.Extensions.DependencyInjection;
using TaskList;
using TaskList.Core.Models;
using TaskList.Core.Processing;
using TaskList.Core.Services;
using TaskList.Interfaces.Console;

partial class Program
{
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

    static async Task RunConsoleAppAsync(IServiceProvider serviceProvider)
    {
        var consoleTaskList = serviceProvider.GetRequiredService<ConsoleTaskList>();
        await consoleTaskList.RunAsync();
    }

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
