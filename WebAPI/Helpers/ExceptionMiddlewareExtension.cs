﻿using Data;
using Entities.Models;
using Microsoft.AspNetCore.Diagnostics;
using System.Net;
using WebAPI.Models;

namespace WebAPI.Helpers
{
    public static class ExceptionMiddlewareExtension
    {
        public static void ConfigureExceptionHandler(this IApplicationBuilder app, ICustomeLogger<string> logger)
        {
            app.UseExceptionHandler(appError =>
            {
                appError.Run(async context =>
                {
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    context.Response.ContentType = "application/json";

                    var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                    if (contextFeature != null)
                    {
                        logger.LogError(contextFeature.Error,$"Something went wrong");

                        await context.Response.WriteAsync(new ErrorDetails
                        {
                            StatusCode = context.Response.StatusCode,
                            Message = "Internal Server LogError. LogError generated by NLog!"
                        }.ToString());
                    }
                });
            });
        }
    }
}