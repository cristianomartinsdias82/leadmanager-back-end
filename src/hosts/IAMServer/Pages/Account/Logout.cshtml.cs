// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using IAMServer.Entities;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IAMServer.Identity.Pages.Account
{
    public class LogoutModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<LogoutModel> _logger;
        private readonly IIdentityServerInteractionService _identityServerService;

        public LogoutModel(
            SignInManager<ApplicationUser> signInManager,
            ILogger<LogoutModel> logger,
            IIdentityServerInteractionService identityServerService)
        {
            _signInManager = signInManager;
            _logger = logger;
            _identityServerService = identityServerService;
        }

        public async Task<IActionResult> OnGet(string logoutId)
        {
            var ctx = await _identityServerService.GetLogoutContextAsync(logoutId);
            if (ctx is not null && !ctx.ShowSignoutPrompt)
                return await OnPost(ctx.PostLogoutRedirectUri);

            return Page();
        }

        public async Task<IActionResult> OnPost(string returnUrl = null)
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");

            if (returnUrl != null)
                return Redirect(returnUrl);
         
            // This needs to be a redirect so that the browser performs a new
            // request and the identity for the user gets updated.
            return RedirectToPage();
        }
    }
}
