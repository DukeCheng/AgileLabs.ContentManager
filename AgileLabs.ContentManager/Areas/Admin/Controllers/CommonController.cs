using AgileLabs.ContentManager.Entities;
using AgileLabs.ContentManager.Models;
using AgileLabs.ContentManager.Repositories;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AgileLabs.ContentManager.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CommonController : Controller
    {
        private MongoDbBaseRepository<Page> _pageRepository;
        private MongoDbBaseRepository<Template> _templateRepository;

        public CommonController(MongoDbBaseRepository<Page> pageRepository, MongoDbBaseRepository<Template> templateRepository)
        {
            this._pageRepository = pageRepository;
            this._templateRepository = templateRepository;
        }
    }
}
