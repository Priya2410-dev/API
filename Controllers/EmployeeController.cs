using Borland.Delphi;
using Calyx_Solutions.Model;
using Calyx_Solutions.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Data;
using System.Text.Json.Nodes;
using static Google.Apis.Requests.BatchRequest;
using System.Web.Helpers;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Calyx_Solutions.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        [HttpPost]
        [Route("employeestatus")]
        public IActionResult EmployeeStatus([FromBody] JsonObject obj)
        {
            var validateJsonData = (dynamic)null;
            
            Model.response.WebResponse response = null;
            string json = System.Text.Json.JsonSerializer.Serialize(obj);
            Customer Obj = JsonConvert.DeserializeObject<Customer>(json);
            dynamic data = JObject.Parse(json);
            CompanyInfo.InsertrequestLogTracker("employeestatus full request body: " + JObject.Parse(json), 0, 0, 0, 0, "EmployeeStatus", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
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

            Obj.Client_ID = data.clientID;

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
                MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("sp_select_employementstatus");
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.Parameters.AddWithValue("_ClientId", Obj.Client_ID);

                dt = db_connection.ExecuteQueryDataTableProcedure(_cmd);
                List<Dictionary<string, object>> statuses = new List<Dictionary<string, object>>();

                foreach (DataRow dr in dt.Rows)
                {
                    Dictionary<string, object> status = new Dictionary<string, object>();
                    status["employeeStatusId"] = dr["ID"];
                    status["employeeStatusName"] = dr["Employement"];
                    statuses.Add(status);
                }
                validateJsonData = new { response = true, responseCode = "00", data = statuses };
                return new JsonResult(validateJsonData);
            }
            catch (Exception ex)
            {
                validateJsonData = new { response = false, responseCode = "01", data = "Invalid Request" };
                CompanyInfo.InsertErrorLogTracker(ex.ToString(), 0, 0, 0, 0, "EmployeeStatus", 0, Convert.ToInt32(Obj.Client_ID), "", HttpContext);
                return new JsonResult(validateJsonData);
            }
        }

    }
}
