using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Data;
using System.Text.Json.Nodes;
using Calyx_Solutions.Model;
using Microsoft.AspNetCore.Authorization;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Calyx_Solutions.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeliveryTypeController : ControllerBase
    {

        [HttpPost]         
        [Route("getdeliverytype")]
        public IActionResult Getdeliverytype([FromBody] JsonObject obj)
        {
            Model.response.WebResponse response = null;
            string json = System.Text.Json.JsonSerializer.Serialize(obj);
            dynamic data = JObject.Parse(json);
            CompanyInfo.InsertrequestLogTracker("getdeliverytype full request body: " + JObject.Parse(json), 0, 0, 0, 0, "Getdeliverytype", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
            var validateJsonData = (dynamic)null;
            Currency Obj = JsonConvert.DeserializeObject<Currency>(json);
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
                if (data.countryID == "" || data.countryID == null)
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "Invalid country ID ." };
                    return new JsonResult(validateJsonData);
                }
                Obj.Client_ID = data.clientID;
                Obj.Branch_ID = data.branchID;
                Obj.Country_ID = data.countryID;
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

                MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("GetDeliveryTypes");
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.Parameters.AddWithValue("_Client_ID", Obj.Client_ID);
                _cmd.Parameters.AddWithValue("_Country_ID", Obj.Country_ID);

                DataTable dt = db_connection.ExecuteQueryDataTableProcedure(_cmd);
                if (dt.Rows.Count > 0)
                {
                    List<Dictionary<string, object>> deleveries = new List<Dictionary<string, object>>();

                    dt.Columns["Delivery_Type"].ColumnName = "deliveryType";
                    dt.Columns["DeliveryType_Id"].ColumnName = "deliveryTypeID";
                    dt.Columns["DType_Description"].ColumnName = "deliveryTypeDescription";

                    var resultData = dt.Rows.OfType<DataRow>()
                .Select(row => dt.Columns.OfType<DataColumn>()
                    .ToDictionary(col => col.ColumnName, c => row[c].ToString()));

                    /*foreach (DataRow dr in dt.Rows)
                    {
                        Dictionary<string, object> delevery = new Dictionary<string, object>();
                        delevery["deliveryType"] = dr["Delivery_Type"];
                        delevery["deliveryTypeID"] = dr["DeliveryType_Id"];
                        delevery["deliveryTypeDescription"] = dr["DType_Description"];
                        deleveries.Add(delevery);
                    } 
                    validateJsonData = new { response = true, responseCode = "00", data = deleveries };*/
                    validateJsonData = new { response = true, responseCode = "00", data = resultData };
                    return new JsonResult(validateJsonData);
                }
                validateJsonData = new { response = false, responseCode = "02", data = "Record not found" };
                return new JsonResult(validateJsonData);
            }
            catch (Exception ex)
            {
                validateJsonData = new { response = false, responseCode = "01", data = "Invalid Request" };
                CompanyInfo.InsertErrorLogTracker(ex.ToString(), 0, 0, 0, 0, "Getdeliverytype", Convert.ToInt32(Obj.Branch_ID), Convert.ToInt32(Obj.Client_ID), "", HttpContext);
                return new JsonResult(validateJsonData);
            }


        }
    }
}
