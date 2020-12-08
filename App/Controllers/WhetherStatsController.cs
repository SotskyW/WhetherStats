using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Whether.Common;
using Whether.Repository;
using Whether.ServiceModels;

namespace Whether.Controllers
{
    [Route("api/[controller]")]
    public class WhetherStatsController : Controller
    {
        IRepository _repository;
        IWhetherLogger _logger;
        public WhetherStatsController(IRepository repository, IWhetherLogger logger)
        {
            _repository = repository;
            _logger = logger;
        }

        [HttpGet("")]
        public async Task<WhetherStatsServiceModel> Index(string date, string city, int daysInPast = 2, int daysInFuture = 15)
        {
            var whetherStatsDto = await new Manager(_repository, _logger).GetWhetherStats(DateTime.Parse(date), city, daysInPast, daysInFuture);
            return new WhetherStatsServiceModel(whetherStatsDto);
        }
    }
}
