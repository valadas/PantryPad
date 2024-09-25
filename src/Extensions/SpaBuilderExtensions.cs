// Copyright (c) Daniel Valadas. All rights reserved.

namespace PantryPad.Extensions
{
    using System.Diagnostics;
    using Microsoft.AspNetCore.SpaServices;
    using PantryPad.Middlewares;

    /// <summary>
    /// Extension methods for enabling Stencil development server middleware support.
    /// </summary>
    public static class SpaBuilderExtensions
    {
        private static bool isStencilDevServerStarted = false;

        /// <summary>
        /// Configures the SPA to use the Stencil development server.
        /// </summary>
        /// <param name="spa">The <see cref="ISpaBuilder"/> to extend.</param>
        /// <param name="npmScript">The name of the npm script to run.</param>
        /// <param name="app">The <see cref="IApplicationBuilder"/> to use.</param>
        public static void UseStencilDevelopmentServer(this ISpaBuilder spa, string npmScript, IApplicationBuilder app)
        {
            var env = spa.ApplicationBuilder.ApplicationServices.GetRequiredService<IWebHostEnvironment>();
            var lifetime = app.ApplicationServices.GetRequiredService<IHostApplicationLifetime>();

            if (env.IsDevelopment() && !isStencilDevServerStarted)
            {
                // Add middleware to intercept and modify the HTML if needed.
                app.UseMiddleware<StencilHtmlResponseMiddleware>();

                // Run npm script to start Stencil dev server and wait for build to finish.
                spa.UseProxyToSpaDevelopmentServer(async () =>
                {
                    if (isStencilDevServerStarted)
                    {
                        return new Uri("http://localhost:3333");
                    }

                    var processInfo = new ProcessStartInfo("npm", $"run {npmScript}")
                    {
                        WorkingDirectory = spa.Options.SourcePath,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true,
                    };

                    var process = Process.Start(processInfo);
                    if (process == null)
                    {
                        throw new InvalidOperationException("Failed to start Stencil development server.");
                    }

                    // Hook into application shutdown event to stop the dev-server process.
                    lifetime.ApplicationStopping.Register(() =>
                    {
                        if (!process.HasExited)
                        {
                            Console.WriteLine("Stopping the Stencil dev server...");
                            process.Kill();  // Stop the dev-server process when dotnet watch stops.
                        }
                    });

                    // Capture the output and wait for the "build finished" signal.
                    var tcs = new TaskCompletionSource<bool>();
                    process.OutputDataReceived += (sender, args) =>
                    {
                        if (args.Data != null)
                        {
                            Console.WriteLine(args.Data);

                            if (args.Data.Contains("build finished"))
                            {
                                isStencilDevServerStarted = true;

                                if (!tcs.Task.IsCompleted)
                                {
                                    tcs.SetResult(true);
                                }
                            }
                        }
                    };

                    process.BeginOutputReadLine();

                    // Wait for "build finished" before returning
                    await tcs.Task;

                     // Dev server URL
                    return new Uri("http://localhost:3333");
                });
            }
        }
    }
}