namespace OnlineStoreAPI
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<LoggingMiddleware> _logger;

        public LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            _logger.LogInformation("Incoming Request: {Method} {Path} - Headers: {Headers} - Query: {Query}",
                context.Request.Method, context.Request.Path, context.Request.Headers, context.Request.QueryString);

            await _next(context);
        }
    }
}
