using AgileLabs.ContentManager.Models;
using Microsoft.AspNetCore.Mvc;

namespace AgileLabs.ContentManager.Areas.Admin.Controllers
{
    [Route("api")]
    public class SubmitApiController : Controller
    {
        [HttpPost("freetrial/trialapply")]
        public string TrialApply(TrialApplyModel model)
        {
            return "Ok";
        }
    }
}
