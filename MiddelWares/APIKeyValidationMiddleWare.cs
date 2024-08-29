namespace OnlineStoreAPI
{

    public class ApiKeyValidationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ApiKeyValidationMiddleware> _logger;
        private readonly string _apiKey;

        public ApiKeyValidationMiddleware(RequestDelegate next, IConfiguration configuration, ILogger<ApiKeyValidationMiddleware> logger)
        {
            _next = next;
            _logger = logger;
            _apiKey = configuration.GetValue<string>("ApiKey");
        }

        public async Task InvokeAsync(HttpContext context)
        {
            _logger.LogInformation("Validating API key...");

            if (!context.Request.Headers.TryGetValue("X-Api-Key", out var extractedApiKey) ||
                !_apiKey.Equals(extractedApiKey))
            {
                context.Response.StatusCode = 401; // Unauthorized
                await context.Response.WriteAsync("Unauthorized client.");
                return;
            }

            await _next(context);
        }
    }
}