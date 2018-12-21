using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace NepaliWeatherApi.Controllers
{
    [Route("api/[controller]")]
    public class NepalMeteorologicalDataController : Controller
    {
        private readonly WeatherService _wetherServ;
        public NepalMeteorologicalDataController(WeatherService weatherService)
        {
            _wetherServ = weatherService;
        }
        [HttpGet]
        public object WeatherApi(string city=null)
        {
            if (city != null)
            {
              return  _wetherServ.GetWeatherByDetail(city);
            }
            else
            {
                return _wetherServ.GetAllWeather();
            }
        }
        

    }
}
