using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Net;
using System.Threading.Tasks;
using MVCWebApp.Models;

namespace MVCWebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly IMongoCollection<VisitorEntry> _visitorCollection;

        public HomeController(IConfiguration config)
        {
            var client = new MongoClient(config["MongoDB:ConnectionString"]);
            var database = client.GetDatabase(config["MongoDB:DatabaseName"]);
            _visitorCollection = database.GetCollection<VisitorEntry>(config["MongoDB:CollectionName"]);
        }

       

        public async Task<IActionResult> Index()
{
    var remoteIp = HttpContext.Connection.RemoteIpAddress;

    string formattedIp = "Unknown";

    if (remoteIp != null)
    {
        if (remoteIp.IsIPv4MappedToIPv6) 
        {
            // Convert IPv6-mapped IPv4 to a standard IPv4 address
            formattedIp = remoteIp.MapToIPv4().ToString(); 
        }
        else if (remoteIp.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork) // IPv4
        {
            formattedIp = remoteIp.ToString(); 
        }
        else if (remoteIp.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6) // IPv6
        {
            if (remoteIp.ToString() == "::1")  
            {
                formattedIp = "127.0.0.1";  // Convert localhost IPv6 (::1) to IPv4
            }
            else
            {
                formattedIp = remoteIp.ToString(); 
            }
        }
    }

    var visitor = new VisitorEntry
    {
        IpAddress = formattedIp, 
        VisitTime = DateTime.UtcNow
    };

    await _visitorCollection.InsertOneAsync(visitor);

    return View();
}


    }
}
