using Calyx_Solutions.Model;
using Calyx_Solutions.Service;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Data;
using System.Text.Json.Nodes;
using static Google.Apis.Requests.BatchRequest;
using System.Web.Helpers;
using System.Net.Http;

using System;
using System.Net.Http;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Calyx_Solutions.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TitleController : ControllerBase
    {

        private readonly ILogger<TitleController> _logger;

        public TitleController(ILogger<TitleController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
         
        [Route("title")]
        public IActionResult Select_Title([FromBody] JsonObject obj)
        {
            Model.response.WebResponse response = null;
            string json = System.Text.Json.JsonSerializer.Serialize(obj);
            dynamic data = JObject.Parse(json);
            var validateJsonData = (dynamic)null;
            CompanyInfo.InsertrequestLogTracker("title full request body: " + JObject.Parse(json), 0, 0, 0, 0, "Select_Title", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
            Customer Obj = JsonConvert.DeserializeObject<Customer>(json);
            try
            {
                if (!SqlInjectionProtector.ReadJsonObjectValues(obj) || !SqlInjectionProtector.ReadJsonObjectValuesScript(obj))
                {
                    var errorResponse = new { response = false, responseCode = "02", data = "Invalid input detected." };
                    return new JsonResult(errorResponse) { StatusCode = 400 };
                }

                int number;
                if (!int.TryParse(Convert.ToString(data.clientID), out number) || data.clientID == "" || data.clientID == null)
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "Invalid Client ID." };
                    return new JsonResult(validateJsonData);
                }
                if (!int.TryParse(Convert.ToString(data.branchID), out number) || data.branchID == "" || data.branchID == null)
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "Invalid branch ID ." };
                    return new JsonResult(validateJsonData);
                }

                Obj.Client_ID = data.clientID;
            Obj.Branch_ID = data.branchID;
            object o = JsonConvert.DeserializeObject(json);
            bool checkinjection = CompanyInfo.checksqlinjectiondata(o);
            if (!checkinjection)
            {
                response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_TECHNICAL_ERROR);
                response.ResponseMessage = "Invalid Field Values.";
                response.ResponseCode = 6;
                validateJsonData = new { response = false, responseCode = "01", data = response.ResponseMessage };
                return new JsonResult(validateJsonData);
            }
            
                DataTable dt = new DataTable();
                MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("sp_select_title_master");
                _cmd.CommandType = CommandType.StoredProcedure;

                dt = db_connection.ExecuteQueryDataTableProcedure(_cmd);
                List<Dictionary<string, object>> Titles = new List<Dictionary<string, object>>();
                foreach (DataRow dr in dt.Rows)
                {
                    Dictionary<string, object> Title = new Dictionary<string, object>();
                    Title["employeeTitleId"] = dr["Title_ID"];
                    Title["employeeTitle"] = dr["Title"];
                    Titles.Add(Title);
                }
                validateJsonData = new { response = true, responseCode = "00", data = Titles };
                return new JsonResult(validateJsonData);
            }
            catch (Exception ex)
            {
                validateJsonData = new { response = false, responseCode = "01", data = "Invalid Request" };
                CompanyInfo.InsertErrorLogTracker(ex.ToString(), 0, 0, 0, 0, "Select_Title", 0, Convert.ToInt32(Obj.Client_ID), "", HttpContext);
                return new JsonResult(validateJsonData);
            }
        }

        [HttpGet]
        [Route("Gender")]
        public IActionResult Select_Gender()
        {
            try
            {
                string[] genders = new string[] { "Male", "Female", "Other" };

                var jsonData = new { response = true, responseCode = "00", data = genders };
                return new JsonResult(jsonData);
            }
            catch (Exception ex)
            {
                
                var jsonData = new { response = false, responseCode = "01", data = "Invalid Request" };
                return new JsonResult(jsonData);
            }
        }


       

    }
}
