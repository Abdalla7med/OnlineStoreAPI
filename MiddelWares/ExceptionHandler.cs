namespace OnlineStoreAPI
{
    public class ExceptionHandler
    {

        private RequestDelegate _next;
        public ExceptionHandler(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next.Invoke(context);

            }
            catch (Exception ex)
            {

                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/json";
                var result = new
                {
                    errorCode = ex.Source,
                    errorText = ex.Message
                };
                await context.Response.WriteAsJsonAsync(result);

            }

        }
    }
}
