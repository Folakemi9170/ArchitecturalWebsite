using Microsoft.AspNetCore.Diagnostics;
using Serilog;

namespace ArchitecturalWebsite.Infrastructure.Extensions
{
    public static class ExceptionHandlerExtensions
    {
        public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseExceptionHandler(appError =>
            {
                appError.Run(async context =>
                {
                    context.Response.StatusCode = 500;
                    context.Response.ContentType = "application/json";

                    var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;

                    if (exception != null)
                    {
                        Log.Error(exception, "Unhandled exception at {Path}", context.Request.Path);

                        var response = new
                        {
                            Success = false,
                            Message = env.IsDevelopment() ? exception.Message : "An error occurred."
                        };

                        await context.Response.WriteAsJsonAsync(response);
                    }
                });
            });

            return app;
        }
    }
}
