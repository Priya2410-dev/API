using Calyx_Solutions.Model;
using Calyx_Solutions.Service;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Data;
using System.Transactions;
using System.Text.Json.Nodes;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Calyx_Solutions.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NationalityController : ControllerBase
    {
        [HttpPost]
        [Route("nationalitylist")]
        public IActionResult Nationality([FromBody] JsonObject obj)
        {
            Model.response.WebResponse response = null;
            string json = System.Text.Json.JsonSerializer.Serialize(obj);
            dynamic data = JObject.Parse(json);
            CompanyInfo.InsertrequestLogTracker("nationalitylist full request body: " + JObject.Parse(json), 0, 0, 0, 0, "Nationality", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
            Customer Obj = JsonConvert.DeserializeObject<Customer>(json);

            var validateJsonData = (dynamic)null;
            try {
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
                    validateJsonData = new { response = false, responseCode = "02", data = "Invalid branchID field." };
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
                MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("SP_Select_All_Countries");
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.Parameters.AddWithValue("_Client_ID", Obj.Client_ID);

                dt = db_connection.ExecuteQueryDataTableProcedure(_cmd);

                List<Dictionary<string, object>> nationalities = new List<Dictionary<string, object>>();
                foreach (DataRow dr in dt.Rows)
                {
                    Dictionary<string, object> nationality = new Dictionary<string, object>();
                    nationality["nationalityCountryName"] = dr["Country_Name"];
                    nationality["nationalityCountryId"] = dr["Country_ID"];
                    nationality["nationalityCountryCode"] = dr["Country_Code"];
                    nationality["nationalityCountryCurrency"] = dr["Country_Currency"];
                    nationality["ISO_Code"] = Convert.ToString( dr["ISO_Code"]);
                    nationality["Delete_Status"] = Convert.ToString(dr["Delete_Status"]);
                    nationality["Country_Flag"] = Convert.ToString(dr["Country_Flag"]);
                    nationality["Sending_Flag"] = Convert.ToString(dr["Sending_Flag"]);
                    nationality["Currency_Sign"] = Convert.ToString(dr["Currency_Sign"]);

                    nationality["Timezone"] = Convert.ToString(dr["Timezone"]);
                    nationality["Risk_Level"] = Convert.ToString(dr["Risk_Level"]);
                    nationality["preferred_flag"] = Convert.ToString(dr["preferred_flag"]);
                    nationality["Popular_country"] = Convert.ToString(dr["Popular_country"]);

                    nationality["nationality"] = Convert.ToString(dr["nationality"]);
                    nationality["basecountry_status"] = Convert.ToString(dr["basecountry_status"]);
                    nationality["chkpostcode"] = Convert.ToString(dr["chkpostcode"]);
                    nationality["chkmobile"] = Convert.ToString(dr["chkmobile"]);
                    nationality["Sending_Flag_backoffice"] = Convert.ToString(dr["Sending_Flag_backoffice"]);
                    nationality["show_on_Registration"] = Convert.ToString(dr["show_on_Registration"]);
                    nationalities.Add(nationality);
                }
                validateJsonData = new { response = true, responseCode = "00", data = nationalities };
                return new JsonResult(validateJsonData);
            }
            catch (Exception ex)
            {
                validateJsonData = new { response = false, responseCode = "01", data = "Invalid Request" };
                CompanyInfo.InsertErrorLogTracker(ex.ToString(), 0, 0, 0, 0, "Nationality", Convert.ToInt32(Obj.Branch_ID), Convert.ToInt32(Obj.Client_ID), "", HttpContext);
                return new JsonResult(validateJsonData);
            }
        }

    }
}
