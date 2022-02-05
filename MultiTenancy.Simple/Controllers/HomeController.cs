using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MultiTenancy.Data.Auth;
using MultiTenancy.Simple.Models;
using MultiTenancy.Simple.Models.Home;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MultiTenancy.Simple.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        private readonly AuthContext _context;

        public HomeController(AuthContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View(new LoginModel());
        }

        [HttpPost]
        public async Task<IActionResult> Index(LoginModel model)
        {
            string activeTenant = "";
            if (!string.IsNullOrWhiteSpace(model.Username))
            {
                var user = _context.Users
                    .Include(x => x.Tenants)
                        .ThenInclude(x => x.Tenant)
                    .SingleOrDefault(x => x.Username.ToLower() == model.Username.ToLower());
                if (user == null)
                {
                    ModelState.AddModelError(nameof(LoginModel.Username), "User not found");
                }
                else
                {
                    var claims = new List<Claim>();
                    claims.Add(new Claim(System.Security.Claims.ClaimTypes.Name, user.Username));
                    foreach (var tenant in user.Tenants)
                    {
                        claims.Add(new Claim(Models.Home.ClaimTypes.Tenant, tenant.Tenant.Name.ToUpper()));
                        claims.Add(new Claim(Models.Home.ClaimTypes.TenantFriendly, tenant.Tenant.Name));
                    }
                    activeTenant = user.Tenants.First().Tenant.Name;
                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(identity),
                        new AuthenticationProperties());
                }
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            return RedirectToAction("Index", "Dashboard", new { tenant = activeTenant });
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
