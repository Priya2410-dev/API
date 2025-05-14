using Calyx_Solutions.Model;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Data;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Authorization;
using gudusoft.gsqlparser;
using System.Net;
using System.Security.Claims;
using System.Text.Json;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Calyx_Solutions.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddressController : ControllerBase
    {
        // POST api/<AddressInfoController>
        [HttpPost]
        [Authorize]
        [Route("addressinfo")]
        public IActionResult AddressInfo([FromBody] JsonObject obj)
        {
            Model.response.WebResponse response = null;
            string json = System.Text.Json.JsonSerializer.Serialize(obj);
            dynamic data = JObject.Parse(json);
            JsonElement jsonElement = System.Text.Json.JsonSerializer.SerializeToElement(obj);//sanket // Converted JsonObject to JsonElement
            var claimsIdentity = User.Identity as ClaimsIdentity;//sanket
            var authHeader = "";
            try { authHeader = HttpContext.Request.Headers["Authorization"].ToString(); } catch (Exception egx) { authHeader = ""; }

            bool auth = AuthController.checkAuth(claimsIdentity, jsonElement, Convert.ToString(authHeader));
            if (!auth)
            {
                var errorResponse = new { response = false, responseCode = "403", data = "Access Forbidden" };
                return new JsonResult(errorResponse) { StatusCode = (int)HttpStatusCode.Forbidden };
            }
            CompanyInfo.InsertrequestLogTracker("addressinfo full request body: " + JObject.Parse(json), 0, 0, 0, 0, "AddressInfo", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
            var validateJsonData = (dynamic)null;
            Customer Obj = JsonConvert.DeserializeObject<Customer>(json);
            try
            {
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
                if (data.customerID == "" || data.customerID == null)
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "Invalid customer ID ." };
                    return new JsonResult(validateJsonData);
                }
                Obj.Client_ID = data.clientID;
                Obj.Branch_ID = data.branchID;
                Obj.Id = data.customerID;

                if (!SqlInjectionProtector.ValidateObjectForSqlInjection(Obj) || !SqlInjectionProtector.ValidateObjectForScriptSqlInjection(Obj))
                {
                    var errorResponse = new { response = false, responseCode = "02", data = "Invalid input detected." };
                    return new JsonResult(errorResponse) { StatusCode = 400 };
                }

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
                int Customer_ID = Convert.ToInt32(CompanyInfo.Decrypt(Convert.ToString(Obj.Id), true));
                List<Model.Customer> _lst = new List<Model.Customer>();
                DataTable dt = new DataTable();
                MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("customer_details_by_param");
                _cmd.CommandType = CommandType.StoredProcedure;
                string _whereclause = " and cr.Client_ID=" + Obj.Client_ID;
                _whereclause = " and cr.Customer_ID=" + Customer_ID;

                _cmd.Parameters.AddWithValue("_whereclause", _whereclause);
                _cmd.Parameters.AddWithValue("_SecurityKey", CompanyInfo.SecurityKey());
                dt = db_connection.ExecuteQueryDataTableProcedure(_cmd);
                if(dt.Rows.Count > 0) 
                {
                    List<Dictionary<string, object>> addresses = new List<Dictionary<string, object>>();

                    foreach (DataRow dr in dt.Rows)
                    {
                        Dictionary<string, object> address = new Dictionary<string, object>();
                        address["postCode"] = dr["Post_Code"];
                        address["houseNumber"] = dr["House_Number"];
                        address["countryName"] = dr["Country_Name"];
                        address["cityName"] = dr["City_Name"];
                        address["street"] = dr["Street"];
                        address["referenceNo"] = dr["WireTransfer_ReferanceNo"];
                        address["customerAddress"] = dr["Addressline_2"];
                        addresses.Add(address);
                    }
                    validateJsonData = new { response = true, responseCode = "00", data = addresses };
                    return new JsonResult(validateJsonData);
                }
                validateJsonData = new { response = false, responseCode = "02", data = "Invalid Request" };
                return new JsonResult(validateJsonData);

            }
            catch (Exception ex)
            {
                validateJsonData = new { response = false, responseCode = "01", data = "Invalid Request" };
                CompanyInfo.InsertErrorLogTracker(ex.ToString(), 0, 0, 0, 0, "UpdateAddressInfo", Convert.ToInt32(Obj.Branch_ID), Convert.ToInt32(Obj.Client_ID), "", HttpContext);
                return new JsonResult(validateJsonData);
            }
        }
    }
}
