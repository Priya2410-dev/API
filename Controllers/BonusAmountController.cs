using Calyx_Solutions.Model;
using Calyx_Solutions.Service;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data;
using System.Globalization;
using System.Net;
using System.Security.Claims;
using System.Text.Json;

using static Google.Apis.Requests.BatchRequest;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Calyx_Solutions.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BonusAmountController : ControllerBase
    {
        // POST api/<BonusAmountController>
        [HttpPost]
        [Authorize]
        [Route("rewardamount")]
        public IActionResult GetRewardAmountCustomerprofile([FromBody] JsonElement obj1)
        {
            HttpContext context = HttpContext;
            string json = System.Text.Json.JsonSerializer.Serialize(obj1);
            dynamic data = JObject.Parse(json); string Status = string.Empty;
            var claimsIdentity = User.Identity as ClaimsIdentity;//sanket
            var authHeader = "";
            try { authHeader = HttpContext.Request.Headers["Authorization"].ToString(); } catch (Exception egx) { authHeader = ""; }

            bool auth = AuthController.checkAuth(claimsIdentity, obj1, Convert.ToString(authHeader));
            if (!auth)
            {
                var errorResponse = new { response = false, responseCode = "403", data = "Access Forbidden" };
                return new JsonResult(errorResponse) { StatusCode = (int)HttpStatusCode.Forbidden };
            }
            CompanyInfo.InsertrequestLogTracker("rewardamount full request body: " + JObject.Parse(json), 0, 0, 0, 0, "GetRewardAmountCustomerprofile", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
            Model.response.WebResponse response = null;
            Customer obj = new Customer();

            var validateJsonData = (dynamic)null;

            // data used for validation purpose
            try
            {
                if (data.clientID == "" || data.clientID == null)
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "Invalid Client ID." };
                    return new JsonResult(validateJsonData);
                }

                if (data.customerID == "" || data.customerID == null)
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "Invalid Customer ID." };
                    return new JsonResult(validateJsonData);
                }

                if (!SqlInjectionProtector.ReadJsonElementValues(obj1) || !SqlInjectionProtector.ReadJsonElementValuesScript(obj1))
                {
                    var errorResponse = new { response = false, responseCode = "02", data = "Invalid input detected." };
                    return new JsonResult(errorResponse) { StatusCode = 400 };
                }


                obj.Client_ID = data.clientID;
            obj.Customer_ID = data.customerID;
            obj.Branch_ID = data.branchID;

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
            int Customer_ID = Convert.ToInt32(CompanyInfo.Decrypt(Convert.ToString(obj.Customer_ID), true));
            List<Model.Customer> _lst = new List<Model.Customer>();
            MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("Show_reward_customerprofile");
            _cmd.CommandType = CommandType.StoredProcedure;
            _cmd.Parameters.AddWithValue("_Customer_ID", Customer_ID);
            _cmd.Parameters.AddWithValue("_client_id", obj.Client_ID);
            DataTable dt = db_connection.ExecuteQueryDataTableProcedure(_cmd);


                if (dt != null && dt.Rows.Count > 0)
                {
                    List<Dictionary<string, object>> Wallet = new List<Dictionary<string, object>>();
                   
                    foreach (DataRow dr in dt.Rows)
                    {
                        Dictionary<string, object> bonus_amount = new Dictionary<string, object>();
                        bonus_amount["result"] = dr["result"];

                        if( DBNull.Value.Equals(bonus_amount["result"] ) ) { }
                        else { Wallet.Add(bonus_amount); }                        
                    }

                    validateJsonData = new { response = true, responseCode = "00", data = Wallet };
                    CompanyInfo.InsertActivityLogDetails("rewardamount", 0, 0, 0, 0, "getrewardamountcustomerprofile", Convert.ToInt32(obj.Client_ID), 0, "", HttpContext);
                    return new JsonResult(validateJsonData);
                }
                else
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "No Records Found." };
                    return new JsonResult(validateJsonData);
                }
        }
            catch (Exception ex)
            {
                
                string Activity = "Api rewardamount: " + ex.ToString() + " ";
                

                validateJsonData = new { response = false, responseCode = "01", data = "Invalid Request" };
                CompanyInfo.InsertErrorLogTracker(Activity.ToString(), 0, 0, 0, 0, "getrewardamountcustomerprofile", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "", context);
                return new JsonResult(validateJsonData);
            }

        }


        [HttpPost]
        [Authorize]
        [Route("totalsendamount")]
        public IActionResult TotalSendAmount([FromBody] JsonElement obj)
        {
            HttpContext context = HttpContext;
            string json = System.Text.Json.JsonSerializer.Serialize(obj);
            var validateJsonData = (dynamic)null;
            dynamic data = JObject.Parse(json);
            var claimsIdentity = User.Identity as ClaimsIdentity;//sanket
            var authHeader = "";
            try { authHeader = HttpContext.Request.Headers["Authorization"].ToString(); } catch (Exception egx) { authHeader = ""; }

            bool auth = AuthController.checkAuth(claimsIdentity, obj, Convert.ToString(authHeader));
            if (!auth)
            {
                var errorResponse = new { response = false, responseCode = "403", data = "Access Forbidden" };
                return new JsonResult(errorResponse) { StatusCode = (int)HttpStatusCode.Forbidden };
            }
            Customer obj1 = new Customer();
            Model.response.WebResponse response = null;
            try
            {
                if (!SqlInjectionProtector.ReadJsonElementValues(obj) || !SqlInjectionProtector.ReadJsonElementValuesScript(obj))
                {
                    var errorResponse = new { response = false, responseCode = "02", data = "Invalid input detected." };
                    return new JsonResult(errorResponse) { StatusCode = 400 };
                }

                if (data.clientID == "" || data.clientID == null)
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "Invalid Client ID." };
                    return new JsonResult(validateJsonData);
                }
                if (data.branchID == "" || data.branchID == null)
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "Invalid Client Id." };
                    return new JsonResult(validateJsonData);
                }

                if (data.customerID == "" || data.customerID == null)
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "Invalid Customer ID." };
                    return new JsonResult(validateJsonData);
                }

                
                obj1.Client_ID = data.clientID;
                obj1.Customer_ID = data.customerID;
                obj1.Branch_ID = data.branchID;
                //obj1.RecordDate = CompanyInfo.gettime(data.clientId, context);

                int Customer_ID = Convert.ToInt32(CompanyInfo.Decrypt(Convert.ToString(obj1.Customer_ID), true));
               

                DataTable dt0 = new DataTable();
                // DataTable[] dt = new DataTable[2];

                List<Model.Customer> _lst = new List<Model.Customer>();
                //var context = System.Web.HttpContext.Current;
                //string Username = Convert.ToString(context.Request.Form["Branch_ID"]);
                MySqlConnector.MySqlCommand _cmd0 = new MySqlConnector.MySqlCommand("Dashboard_Details");//"select    cast(AES_DECRYPT(UNHEX(u.Password), '" + obj.SecurityKey.Trim() + "' ) as  char(500)) as Password,u.First_Name,u.Last_Name,u.Customer_ID,U.WireTransfer_ReferanceNo,u.Email_ID,u.Branch_ID,u.Client_ID from customer_registration u where    Email_ID = '" + obj.Name.Trim() + "' and Password=HEX(AES_ENCRYPT('" + obj.Password.Trim() + "','" + obj.SecurityKey.Trim() + "'))  and u.delete_status=0 ;");
                _cmd0.CommandType = CommandType.StoredProcedure;
                _cmd0.Parameters.AddWithValue("_Customer_ID", Customer_ID);
                _cmd0.Parameters.AddWithValue("_Client_ID", obj1.Client_ID);
                dt0 = db_connection.ExecuteQueryDataTableProcedure(_cmd0);
                // dt[0] = dt0; dt[0].TableName = "DashInfo";
                if (dt0 != null && dt0.Rows.Count > 0)
                {
                    List<Dictionary<string, object>> Amount = new List<Dictionary<string, object>>();

                    foreach (DataRow dr in dt0.Rows)
                    {
                        Dictionary<string, object> Total_Amount = new Dictionary<string, object>();
                        Total_Amount["sendingAmount"] = dr["Sending_Amount"];
                        Total_Amount["transactionCount"] = dr["Transaction_count"];
                        Total_Amount["benfCount"] = dr["Benf_count"];
                        Amount.Add(Total_Amount);
                    }


                    validateJsonData = new { response = true, responseCode = "00", data = Amount };
                    CompanyInfo.InsertActivityLogDetails("TotalSendAmount", 0, 0, 0, 0, "TotalSendAmount", Convert.ToInt32(obj1.Client_ID), 0, "", HttpContext);
                    return new JsonResult(validateJsonData);
                }
                else
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "No Records Foound." };
                    return new JsonResult(validateJsonData);
                }
            }
            catch(Exception ex)
            {
                validateJsonData = new { response = false, responseCode = "01", data = "Invalid Request" };
                CompanyInfo.InsertErrorLogTracker(ex.ToString(), 0, 0, 0, 0, "TotalSendAmount", Convert.ToInt32(obj1.Branch_ID), Convert.ToInt32(obj1.Client_ID),  "", context);
                return new JsonResult(validateJsonData);

            }


        }


       


    } 
}
