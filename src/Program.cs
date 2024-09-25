// Copyright (c) Daniel Valadas. All rights reserved.

namespace PantryPad
{
    /// <summary>
    /// Our application.
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// <param name="args">The command line arguments.</param>
        internal static void Main(string[] args)
        {
            try
            {
                SetupApp(args).Run();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unhandled exceptions: {ex}");
            }
        }

        private static WebApplication SetupApp(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            Startup.ConfigureServices(builder.Services);

            var app = builder.Build();

            Startup.Configure(app, app.Environment);

            return app;
        }
    }
}
