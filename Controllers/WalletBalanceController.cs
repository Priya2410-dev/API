using Calyx_Solutions.Model;
using Calyx_Solutions.Service;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data;
using System.Globalization;
using System.Text.Json;
using static Google.Apis.Requests.BatchRequest;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Calyx_Solutions.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WalletBalanceController : ControllerBase
    {
        

        // POST api/<WalletBalanceController>
        [HttpPost]
        [Authorize]
        [Route("walletdetails")]
        public IActionResult GetWalletDetails([FromBody] JsonElement obj1) // dashboard get wallet data
        {
            HttpContext context = HttpContext;
            Model.response.WebResponse response = null;
            string json = System.Text.Json.JsonSerializer.Serialize(obj1);
            dynamic data = JObject.Parse(json);
            CompanyInfo.InsertrequestLogTracker("walletdetails full request body: " + JObject.Parse(json), 0, 0, 0, 0, "GetWalletDetails", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
            var validateJsonData = (dynamic)null;
            Customer obj = new Customer();

            // data used for validation purpose
            try
            {

                if (!SqlInjectionProtector.ReadJsonElementValues(obj1) || !SqlInjectionProtector.ReadJsonElementValuesScript(obj1))
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
                validateJsonData = new { response = false, responseCode = "02", data = "Invalid Client ID." };
                return new JsonResult(validateJsonData);
            }
            
            if (data.customerID == "" || data.customerID == null)
            {
                validateJsonData = new { response = false, responseCode = "02", data = "Invalid Customer ID." };
                return new JsonResult(validateJsonData);
            }

            
           

            obj.Client_ID = data.clientID;
            obj.Currency_Code = data.currencyCode;
            obj.Customer_ID = data.customerID;
            obj.Branch_ID = data.branchID;
                if (!SqlInjectionProtector.ValidateObjectForSqlInjection(obj) || !SqlInjectionProtector.ValidateObjectForScriptSqlInjection(obj))
                {
                    var errorResponse = new { response = false, responseCode = "02", data = "Invalid input detected." };
                    return new JsonResult(errorResponse) { StatusCode = 400 };
                }


                //Check Sql injection
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



                // Excecution part is start 

                int Customer_ID = Convert.ToInt32(CompanyInfo.Decrypt(Convert.ToString(obj.Customer_ID), true));
            List<Model.Customer> _lst = new List<Model.Customer>();
            MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("SP_GetWalletDetails");
            _cmd.CommandType = CommandType.StoredProcedure;
            _cmd.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
            _cmd.Parameters.AddWithValue("_Customer_ID", Customer_ID);
            string where = "";
            if (obj.Currency_Code != null && obj.Currency_Code != "")
            {
                where = " and currency_master.Currency_Code like '%" + obj.Currency_Code + "%'";
            }
            _cmd.Parameters.AddWithValue("_where", " and wallet_table.AgentFlag=1" + where);
            DataTable dt = db_connection.ExecuteQueryDataTableProcedure(_cmd);
                if (dt != null && dt.Rows.Count > 0)
                {
                    List<Dictionary<string, object>> Wallet = new List<Dictionary<string, object>>();

                    foreach (DataRow dr in dt.Rows)
                    {
                        Dictionary<string, object> wallet_balnce = new Dictionary<string, object>();
                        wallet_balnce["walletID"] = dr["Wallet_ID"];
                        wallet_balnce["walletReference"] = dr["Wallet_Reference"];
                        wallet_balnce["currencyID"] = dr["Currency_ID"];
                        wallet_balnce["walletBalance"] = dr["Wallet_balance"];
                        wallet_balnce["currencyCode"] = dr["Currency_Code"];
                        wallet_balnce["currencyName"] = dr["Currency_Name"];
                        wallet_balnce["imageName"] = dr["ImageName"];
                        Wallet.Add(wallet_balnce);
                    }

                    validateJsonData = new { response = true, responseCode = "00", data = Wallet };
                    CompanyInfo.InsertActivityLogDetails("getwalletdetails", 0, 0, 0, 0, "getwalletdetails", Convert.ToInt32(obj.Client_ID), 0, "", HttpContext);
                    return new JsonResult(validateJsonData);
                }
                else
                {
                    validateJsonData = new { response = true, responseCode = "00", data = "No Record Found." };
                    return new JsonResult(validateJsonData);
                }
            }
            catch(Exception ex)
            {

                validateJsonData = new { response = false, responseCode = "01", data = "Invalid Request" };
                string Acitivy = "Api: getwalletdetails " + ex.ToString();
                CompanyInfo.InsertErrorLogTracker(Acitivy.ToString(), 0, 0, 0, 0, "getwalletdetails", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "", context);
                return new JsonResult(validateJsonData);
            }
        }

       
    }
}
