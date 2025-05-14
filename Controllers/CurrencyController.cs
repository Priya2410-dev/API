using Calyx_Solutions.Model;
using iTextSharp.text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Nodes;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Calyx_Solutions.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CurrencyController : ControllerBase
    {
        [HttpPost]
        [Route("currencylist")]
        public IActionResult CheckListCurrency([FromBody] JsonObject obj)
        {
            Model.response.WebResponse response = null;
            string json = System.Text.Json.JsonSerializer.Serialize(obj);
            dynamic data = JObject.Parse(json);
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
                if (data.countryID == "" || data.countryID == null)
            {
                validateJsonData = new { response = false, responseCode = "02", data = "Invalid countryID field." };
                return new JsonResult(validateJsonData);
            }
            Obj.Client_ID = data.clientID;
            Obj.Country_Id = data.countryID;
            object o = JsonConvert.DeserializeObject(json);
            bool checkinjection = CompanyInfo.checksqlinjectiondata(o);
            if (!checkinjection)
            {
                response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_TECHNICAL_ERROR);
                response.ResponseMessage = "Invalid Field Values.";
                response.ResponseCode = 6;
                validateJsonData = new { response = false, responseCode = "02", data = response.ResponseMessage };
                return new JsonResult(validateJsonData);
            }
         
                DataTable dt = new DataTable();
                MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("getMultiCurrency");
                string whereclause = "";
                if (Obj.Country_Id > 0)
                {
                    whereclause = " and c.Country_ID=" + Obj.Country_Id + "";
                }
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.Parameters.AddWithValue("_whereclause", whereclause);
                _cmd.Parameters.AddWithValue("_Client_ID", Obj.Client_ID);
                dt = db_connection.ExecuteQueryDataTableProcedure(_cmd);

                List<Dictionary<string, object>> currencies = new List<Dictionary<string, object>>();

                foreach (DataRow dr in dt.Rows)
                {
                    Dictionary<string, object> currency = new Dictionary<string, object>();
                    currency["currencyID"] = dr["Currency_ID"];
                    currency["currencyCode"] = dr["Currency_Code"];
                    currency["countryName"] = dr["Country_Name"];
                    currency["countryID"] = dr["Country_ID"];
                    currency["currencyFlag"] = dr["flag"];
                    currencies.Add(currency);
                }

                validateJsonData = new { response = true, responseCode = "00", data = currencies };
                return new JsonResult(validateJsonData);

            }
            catch (Exception ex)
            {
                validateJsonData = new { response = false, responseCode = "01", data = "Invalid Request" };
                CompanyInfo.InsertErrorLogTracker(ex.ToString(), 0, 0, 0, 0, "CheckListCurrency", Convert.ToInt32(Obj.Country_Id), Convert.ToInt32(Obj.Client_ID), "", HttpContext);
                return new JsonResult(validateJsonData);
            }

        }
    }
}
