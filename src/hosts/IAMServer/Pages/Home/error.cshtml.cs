using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IAMServer.Pages.Home
{
    public class ErrorModel : PageModel
    {
        public void OnGet()
        {
        }

        public string ErrorId
        {
            get => HttpContext.Request.Query["errorId"].ToString();
        }
    }
}
