using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MultiTenancy.Web.Data.Tenant;
using System.Linq;
using System.Threading.Tasks;

namespace MultiTenancy.Web.Controllers
{
    public class DashboardController : Controller
    {
        private readonly TenantContext _context;

        public DashboardController(TenantContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var model = await _context.Names.Select(x => x.Forename).ToListAsync();
            return View(model);
        }

        public IActionResult Other()
        {
            return View();
        }

    }
}
