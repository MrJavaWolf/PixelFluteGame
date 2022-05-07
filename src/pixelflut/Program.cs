﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PixelFlut.Core;
using PixelFlut.Pong;
using Serilog;
namespace PixelFlut;

public class Program
{
    public static async Task Main(string[] args)
    {
        // Configuration
        IConfiguration Configuration = new ConfigurationBuilder()
                        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                        .AddEnvironmentVariables()
                        .AddCommandLine(args)
                        .Build();
        PixelFlutScreenRendererConfiguration rendererConfig = Configuration.GetSection("Renderer").Get<PixelFlutScreenRendererConfiguration>();
        PixelFlutGamepadConfiguration gamepadConfig = Configuration.GetSection("Gamepad").Get<PixelFlutGamepadConfiguration>();
        GameLoopConfiguration gameloopConfig = Configuration.GetSection("GameLoop").Get<GameLoopConfiguration>();
        PongConfiguration pongConfig = Configuration.GetSection("Pong").Get<PongConfiguration>();

        // Dependency injection
        var services = new ServiceCollection();
        services.AddSingleton(Configuration);
        services.AddSingleton(rendererConfig);
        services.AddSingleton(gamepadConfig);
        services.AddSingleton(gameloopConfig);
        services.AddSingleton(pongConfig);

        services.AddSingleton<IPixelFlutScreenProtocol, PixelFlutScreenProtocol1>();
        services.AddSingleton<PixelFlutGamepad>();
        services.AddSingleton<IPixelFlutInput>(s => s.GetRequiredService<PixelFlutGamepad>());
        services.AddSingleton<PongGame>();
        services.AddSingleton<GameLoop>();
        services.AddSingleton<TestFraneGenerator>();
        services.AddLogging(logging => logging.AddSerilog(new LoggerConfiguration().ReadFrom.Configuration(Configuration).CreateLogger()));
        services.AddTransient<PixelFlutScreenRenderer>();
        ServiceProvider serviceProvider = services.BuildServiceProvider();

        // Create pixel game loop
        ILogger<Program> logger = serviceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogInformation($"- - - - - Starting pixelflut game - - - - - ");
        PixelFlutGamepad gamepad = serviceProvider.GetRequiredService<PixelFlutGamepad>();
        GameLoop gameLoop = serviceProvider.GetRequiredService<GameLoop>();

        // Setup gracefull shutdown
        CancellationTokenSource tokenSource = new();
        Console.CancelKeyPress += delegate (object? sender, ConsoleCancelEventArgs e)
        {
            e.Cancel = true;
            tokenSource.Cancel();
        };


        // Run
        Task t1 = Task.Run(async () => await gamepad.RunAsync(tokenSource.Token));
        Thread gameLoopThread = new(() => gameLoop.Run(tokenSource.Token));
        gameLoopThread.Start();
        gameLoopThread.Join();
        await Task.WhenAll(t1);

        logger.LogInformation($"- - - - -  Shutdown pixelflut game - - - - - ");
    }


}


