using Microsoft.AspNetCore.Http;
using speedCode.Core.interfaces;

namespace speedCode.Core.attribute;

using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System;

[AttributeUsage(AttributeTargets.Method)]
public class InjectUserIdAttribute : Attribute, IAsyncActionFilter
{
    private readonly string _propertyName;

    public InjectUserIdAttribute(string propertyName = "profileId")
    {
        _propertyName = propertyName;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var jwtService = context.HttpContext.RequestServices.GetService(typeof(IJwtService)) as IJwtService;
        if (jwtService == null)
        {
            context.Result = new ObjectResult("JWT service not available")
            {
                StatusCode = StatusCodes.Status500InternalServerError
            };
            return;
        }

        var authHeader = context.HttpContext.Request.Headers["Authorization"].ToString();
        if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
        {
            context.Result = new UnauthorizedObjectResult("Missing or invalid Authorization header");
            return;
        }

        var token = authHeader.Substring("Bearer ".Length).Trim();

        string? clientId;
        try
        {
            clientId = jwtService.GetClientIdFromToken(token);
        }
        catch (Exception ex)
        {
            context.Result = new UnauthorizedObjectResult($"Invalid token: {ex.Message}");
            return;
        }

        if (string.IsNullOrEmpty(clientId))
        {
            context.Result = new UnauthorizedObjectResult("Unable to extract clientId from token");
            return;
        }

        foreach (var arg in context.ActionArguments.Values)
        {
            var prop = arg?.GetType().GetProperty(_propertyName);
            if (prop != null && prop.CanWrite)
            {
                prop.SetValue(arg, Guid.Parse(clientId));
            }
        }

        await next();
    }
}