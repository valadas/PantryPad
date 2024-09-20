// Copyright (c) Daniel Valadas. All rights reserved.

namespace PantryPad.Middlewares
{
    using System.Text.RegularExpressions;

    /// <summary>
    /// Middleware to handle the ingress path.
    /// </summary>
    public class IngressPathMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="IngressPathMiddleware"/> class.
        /// </summary>
        /// <param name="next">The next request delegate.</param>
        /// <param name="logger">The logger to use.</param>
        public IngressPathMiddleware(RequestDelegate next, ILogger<IngressPathMiddleware> logger)
        {
            this.next = next;
            this.logger = logger;
        }

        /// <summary>
        /// Handles the request.
        /// </summary>
        /// <param name="context">The http context of the request.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task InvokeAsync(HttpContext context)
        {
            this.logger.LogInformation($"Request URL: {context.Request.Method} {context.Request.Path}");

            // Handle requests to "/" or "/index.html" only.
            var path = context.Request.Path.Value ?? string.Empty;
            if (path != "/" && !path.Contains("index.html"))
            {
                await this.next(context);
                return;
            }

            var ingressPath = context.Request.Headers["X-Ingress-Path"];
            this.logger.LogInformation($"Ingress path: {ingressPath}");

            // We only want to modify if we have an ingress path.
            if (string.IsNullOrEmpty(ingressPath))
            {
                await this.next(context);
                return;
            }

            // We only want to modify the response for successful requests.
            if (context.Response.StatusCode != 200)
            {
                await this.next(context);
                return;
            }

            // Modify response to inject ingress path.
            context.Response.OnStarting(async () =>
            {
                var responseBody = context.Response.Body;
                using (var buffer = new MemoryStream())
                {
                    context.Response.Body = buffer;
                    await this.next(context);

                    // Remove the content-lenght, it's wont match and will be recomputed.
                    context.Response.Headers.Remove("Content-Length");

                    context.Response.Body = responseBody;
                    buffer.Seek(0, SeekOrigin.Begin);
                    var html = await new StreamReader(buffer).ReadToEndAsync();

                    // Replace the existing data-resources-url with the ingress path.
                    string pattern = @"data-resources-url=""\/build\/""";
                    string replacement = $"data-resources-url=\"{ingressPath}\"";
                    html = Regex.Replace(html, pattern, replacement);

                    await context.Response.WriteAsync(html);
                }
            });
        }
    }
}