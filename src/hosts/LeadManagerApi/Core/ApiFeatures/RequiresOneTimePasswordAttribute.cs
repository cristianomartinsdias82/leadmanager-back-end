using Application.Security.OneTimePassword.Commands.HandleOneTimePassword;
using CrossCutting.Security.IAM;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;

namespace CrossCutting.Security.Authorization;

public sealed class RequiresOneTimePasswordAttribute : ActionFilterAttribute
{
    private const int Status461OtpChallenge = 461;
    private const int Status462OtpInvalidCode = 462;
    private const int Status463OtpExpiredCode = 463;
    private const string HeaderOtpResource = "resource";
    private const string HeaderOtpCode = "otp";

    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var serviceProvider = context.HttpContext.RequestServices;
        var mediator = serviceProvider.GetRequiredService<ISender>();
        var userService = serviceProvider.GetRequiredService<IUserService>();
        var userId = userService.GetUserId();
        StringValues otpCode;

        if (!context.HttpContext.Request.Headers.TryGetValue(HeaderOtpResource, out var resource) || string.IsNullOrWhiteSpace(resource))
        {
            context.Result = new BadRequestResult();
            return;
        }

        context.HttpContext.Request.Headers.TryGetValue(HeaderOtpCode, out otpCode);

        var optHandlingResult = await mediator.Send(new HandleOneTimePasswordCommandRequest
        {
            Resource = resource!,
            UserId = userId!.Value,
            Code = otpCode
        });

        if (optHandlingResult.Data.Result != OneTimePasswordHandlingOperationResult.ValidCode)
            context.Result = optHandlingResult.Data.Result switch
            {
                OneTimePasswordHandlingOperationResult.Error => new StatusCodeResult(StatusCodes.Status500InternalServerError),
                OneTimePasswordHandlingOperationResult.CodeGeneratedSuccessfully => new StatusCodeResult(Status461OtpChallenge),
                OneTimePasswordHandlingOperationResult.ExpiredCode => new StatusCodeResult(Status463OtpExpiredCode),
                OneTimePasswordHandlingOperationResult.InvalidCode => new StatusCodeResult(Status462OtpInvalidCode),
                _ => new OkResult()
            };

        await base.OnActionExecutionAsync(context, next);
    }
}
