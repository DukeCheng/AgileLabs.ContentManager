using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AgileLabs.ContentManager.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AccountController : Controller
    {
        [HttpGet, AllowAnonymous]
        public async Task<IActionResult> Login()
        {
            var context = this.HttpContext;
            if (!context.User.Identities.Any(identity => identity.IsAuthenticated))
            {
                // Make a large identity
                var claims = new List<Claim>(1001);
                claims.Add(new Claim(ClaimTypes.Name, "bob"));
                for (int i = 0; i < 1000; i++)
                {
                    claims.Add(new Claim(ClaimTypes.Role, "SomeRandomGroup" + i, ClaimValueTypes.String, "IssuedByBob", "OriginalIssuerJoe"));
                }

                var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme));
                await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                //context.Response.ContentType = "text/plain";
            }
            return RedirectToAction(nameof(PagesController.Index), "Pages", new { area = "Admin" });
        }
    }
}
