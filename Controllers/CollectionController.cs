using Calyx_Solutions.Model;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Data;
using System.Text.Json.Nodes;


namespace Calyx_Solutions.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CollectionController : ControllerBase
    {
        // POST api/<CollectionController>
        [HttpPost]      
        [Route("getcollectiontype")]
        public IActionResult Getcollectiontype([FromBody] JsonObject obj)
        {
            Model.response.WebResponse response = null;
            string json = System.Text.Json.JsonSerializer.Serialize(obj);
            dynamic data = JObject.Parse(json);
            CompanyInfo.InsertrequestLogTracker("getcollectiontype full request body: " + JObject.Parse(json), 0, 0, 0, 0, "Getcollectiontype", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
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
                if (!int.TryParse(Convert.ToString(data.clientID), out number)  )
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "Invalid Client ID." };
                    return new JsonResult(validateJsonData);
                }
                if (!int.TryParse(Convert.ToString(data.branchID), out number))
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "Invalid Branch ID." };
                    return new JsonResult(validateJsonData);
                }
                if (data.countryID == "" || data.countryID == null)
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "Invalid Country ID." };
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

                MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("GetPayDepositTypes");
                _cmd.CommandType = CommandType.StoredProcedure;
                string where = "";
                if (Obj.PaymentDepositType_ID > 0)
                {
                    where = " and PaymentDepositType_ID=" + Obj.PaymentDepositType_ID;
                }
                _cmd.Parameters.AddWithValue("_where", " and ShowOnCustSide=0 and Country_ID=" + Obj.Country_ID + "" + where);
                _cmd.Parameters.AddWithValue("_Client_ID", Obj.Client_ID);
                _cmd.Parameters.AddWithValue("_TransferTypeFlag", 0);
                DataTable dt = db_connection.ExecuteQueryDataTableProcedure(_cmd);
                if (dt.Rows.Count > 0)
                {
                    List<Dictionary<string, object>> collections = new List<Dictionary<string, object>>();

                    foreach (DataRow dr in dt.Rows)
                    {
                        Dictionary<string, object> collection = new Dictionary<string, object>();
                        collection["paymentCollectionTypeID"] = dr["PaymentDepositType_ID"];
                        collection["typeName"] = dr["Type_Name"];
                        collections.Add(collection);
                    }

                    validateJsonData = new { response = true, responseCode = "00", data = collections };
                    return new JsonResult(validateJsonData);
                }
                List<Dictionary<string, object>> collectionsData = new List<Dictionary<string, object>>();
                validateJsonData = new { response = true, responseCode = "00", data = collectionsData };
                return new JsonResult(validateJsonData);
            }
            catch (Exception ex)
            {
                validateJsonData = new { response = false, responseCode = "01", data = "Invalid Request" };
                CompanyInfo.InsertErrorLogTracker(ex.ToString(), 0, 0, 0, 0, "Getcollectiontype", Convert.ToInt32(Obj.Branch_ID), Convert.ToInt32(Obj.Client_ID), "", HttpContext);
                return new JsonResult(validateJsonData);
            }

        }
    
    }
}
