// Copyright (c) Daniel Valadas. All rights reserved.

namespace PantryPad
{
    using Microsoft.Extensions.FileProviders;
    using Microsoft.OpenApi.Models;

    /// <summary>
    /// Initial startup configuration for the application.
    /// </summary>
    public static class Startup
    {
        /// <summary>
        /// Configures the services that the app might need.
        /// </summary>
        /// <param name="services">The current service collection.</param>
        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddHttpClient();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "1.0",
                    Title = "PantryPad API v1",
                    Description = "An API for PantryPad (v1)",
                });
            });
            services.AddCors(options =>
            {
                options.AddPolicy("AllowStencilDevServer", policy =>
                {
                    policy.WithOrigins("http://localhost:3333")
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });
        }

        /// <summary>
        /// Configures the app.
        /// </summary>
        /// <param name="app">The application builder.</param>
        /// <param name="env">The current environment.</param>
        public static void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseCors("AllowStencilDevServer");
            }

            app.UseDefaultFiles(new DefaultFilesOptions
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(env.ContentRootPath, "wwwroot", "www")),
                DefaultFileNames = new List<string> { "index.html" },
            });

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                Path.Combine(env.ContentRootPath, "wwwroot", "www")),
                RequestPath = string.Empty,
            });
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "PantryPad API v1");
                c.RoutePrefix = "swagger";
            });

            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
