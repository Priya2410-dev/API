using System.Net.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net;
using RestSharp;
using RestSharp;
using System.Net;
using Newtonsoft.Json.Linq;
using Microsoft.Ajax.Utilities;

namespace Calyx_Solutions.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GeoController : ControllerBase
    {
        private readonly HttpClient _httpClient;

        public GeoController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        static async Task Mainf()
        {
            HttpContext context = null;
            try
            {
               
 
            }
            catch (HttpRequestException e)
            {
                CompanyInfo.InsertErrorLogTracker("GetGeoInfo Mainf error: " + e.ToString(), 0, 0, 0, 0, "Mainf", Convert.ToInt32(0), Convert.ToInt32(0), "", context);
                // Handle request errors
                 
            }
        }

        [HttpGet]
        [Route("getgeoinfo")]
        public async Task<IActionResult> GetGeoInfo()
        {
             
           
            try
            {
                var client = new RestClient("https://tools.keycdn.com/geo.json?host=203.192.231.68");
                client.Timeout = -1;
                var request = new RestRequest(Method.GET);
                client.UserAgent = "keycdn-tools:https://aremkopay.com/";
                IRestResponse response = client.Execute(request);
                Console.WriteLine(response.Content);
                CompanyInfo.InsertErrorLogTracker("GetGeoInfo Mainf  : " + response.Content, 0, 0, 0, 0, "Mainf", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);

                return Ok( );
            }
            catch (HttpRequestException e)
            {
                
                return StatusCode(500, $"Request error: {e.Message}");
            }
        }
    }
}
