// Copyright (c) Daniel Valadas. All rights reserved.

namespace PantryPad.Middlewares
{
    /// <summary>
    /// Logs web requests.
    /// </summary>
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestLoggingMiddleware"/> class.
        /// </summary>
        /// <param name="next">The next request delegate.</param>
        /// <param name="logger">The logger to use.</param>
        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            this.next = next;
            this.logger = logger;
        }

        /// <summary>
        /// Handles the request.
        /// </summary>
        /// <param name="context">The http context of the request.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task Invoke(HttpContext context)
        {
            // Log the request URL
            this.logger.LogInformation($"Request URL: {context.Request.Method} {context.Request.Path}");

            // Log the headers
            foreach (var header in context.Request.Headers)
            {
                this.logger.LogInformation($"{header.Key}: {header.Value}");
            }

            await this.next(context);
        }
    }
}