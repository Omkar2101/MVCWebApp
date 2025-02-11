using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using MVCWebApp.Models;
using System.Threading.Tasks;

namespace MVCWebApp.Controllers
{
    public class PrivacyController : Controller
    {
        private readonly IMongoCollection<VisitorEntry> _visitorCollection;

        public PrivacyController(IConfiguration config)
        {
            var client = new MongoClient(config["MongoDB:ConnectionString"]);
            var database = client.GetDatabase(config["MongoDB:DatabaseName"]);
            _visitorCollection = database.GetCollection<VisitorEntry>(config["MongoDB:CollectionName"]);
        }

        public async Task<IActionResult> Index()
        {
            var latestVisitor = await _visitorCollection
                .Find(_ => true)
                .SortByDescending(v => v.VisitTime)
                .Limit(1)
                .FirstOrDefaultAsync();

            return View(latestVisitor);
        }
    }
}
