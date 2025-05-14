using Calyx_Solutions.Model;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Data;
using System.Net;
using System.Reflection;
using System.Security.Claims;
using System.Text.Json;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Calyx_Solutions.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardWalletBalanceController : ControllerBase
    {
        

        // POST api/<DashboardWalletBalanceController>
        [HttpPost]
        [Route("DashboardWalletBlnce")]
        public IActionResult DashboardWalletBlnce([FromBody] JsonElement obj1) // dashboard get wallet data
        {
            string json = System.Text.Json.JsonSerializer.Serialize(obj1);
            dynamic data = JObject.Parse(json);
            var claimsIdentity = User.Identity as ClaimsIdentity;//sanket
            var authHeader = "";
            try { authHeader = HttpContext.Request.Headers["Authorization"].ToString(); } catch (Exception egx) { authHeader = ""; }

            bool auth = AuthController.checkAuth(claimsIdentity, obj1, Convert.ToString(authHeader));
            if (!auth)
            {
                var errorResponse = new { response = false, responseCode = "403", data = "Access Forbidden" };
                return new JsonResult(errorResponse) { StatusCode = (int)HttpStatusCode.Forbidden };
            }
            var validateJsonData = (dynamic)null;

            if (!SqlInjectionProtector.ReadJsonElementValues(obj1) || !SqlInjectionProtector.ReadJsonElementValuesScript(obj1))
            {
                var errorResponse = new { response = false, responseCode = "02", data = "Invalid input detected." };
                return new JsonResult(errorResponse) { StatusCode = 400 };
            }


            // data used for validation purpose

            if (data.clientId == "" || data.clientId == null)
            {
                validateJsonData = new { response = false, responseCode = "02", data = "Invalid Client Id." };
                return new JsonResult(validateJsonData);
            }
            if (data.branchId == "" || data.branchId == null)
            {
                validateJsonData = new { response = false, responseCode = "02", data = "Invalid Branch Id." };
                return new JsonResult(validateJsonData);
            }
            if (data.currencyCode == "" || data.currencyCode == null)
            {
                validateJsonData = new { response = false, responseCode = "02", data = "Invalid data.Currency Code." };
                return new JsonResult(validateJsonData);
            }
            if (data.customerID == "" || data.customerID == null)
            {
                validateJsonData = new { response = false, responseCode = "02", data = "Invalid Customer ID." };
                return new JsonResult(validateJsonData);
            }

            Model.response.WebResponse response = null;
            Customer obj = new Customer();

            obj.Client_ID = data.clientId;
            obj.Currency_Code = data.currencyCode;
            obj.Customer_ID = data.customerID;

            try { 

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

            var jsonData = new { response = true, responseCode = "00", data = Wallet };
            return new JsonResult(jsonData);
            }
            catch(Exception ex)
            {
                var jsonData = new { response = false, responseCode = "01", data = "Invalid Request" };
                return new JsonResult(jsonData);
            }
        }


        [HttpPost]
        [Route("dashbaordpermissions")]
        public IActionResult dashbaordpermissions([FromBody] JsonElement obj1)
        {
            string json = System.Text.Json.JsonSerializer.Serialize(obj1);
            dynamic data = JObject.Parse(json);
            CompanyInfo.InsertrequestLogTracker("dashbaordpermissions full request body: " + JObject.Parse(json), 0, 0, 0, 0, "dashbaordpermissions", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
            var validateJsonData = (dynamic)null;
            if (data.clientID == "" || data.clientID == null)
            {
                validateJsonData = new { response = false, responseCode = "02", data = "Invalid Client Id." };
                return new JsonResult(validateJsonData);
            }
            else if (data.branchID == "" || data.branchID == null)
            {
                validateJsonData = new { response = false, responseCode = "02", data = "Invalid Branch Id." };
                return new JsonResult(validateJsonData);
            }

            Model.response.WebResponse response = null;
            Customer obj = new Customer();

            obj.Client_ID = data.clientID;
            obj.Branch_ID = data.branchID;
            try
            {
                if (!SqlInjectionProtector.ReadJsonElementValues(obj1) || !SqlInjectionProtector.ReadJsonElementValuesScript(obj1))
                {
                    var errorResponse = new { response = false, responseCode = "02", data = "Invalid input detected." };
                    return new JsonResult(errorResponse) { StatusCode = 400 };
                }


                MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("dashboardpermissions");
                _cmd.CommandType = CommandType.StoredProcedure;
                //_cmd.Parameters.AddWithValue("_Client_ID", obj.Client_ID);              
                string where = " and Client_ID="+ obj.Client_ID;
                _cmd.Parameters.AddWithValue("whereclause", " and 1 = 1  " + where);
                DataTable dt = db_connection.ExecuteQueryDataTableProcedure(_cmd);
                var returndata = dt.Rows.OfType<DataRow>()
                    .Select(row => dt.Columns.OfType<DataColumn>()
                        .ToDictionary(col => col.ColumnName, c => row[c].ToString()));

                var jsonData = new { response = true, responseCode = "00", data = returndata };
                return new JsonResult(jsonData);
            }
            catch (Exception ex)
            {
                CompanyInfo.InsertErrorLogTracker("Exception dashbaordpermissions error: " + ex.ToString(), 0, 0, 0, 0, "dashbaordpermissions", Convert.ToInt32(data.Branch_ID), Convert.ToInt32(data.Client_ID), "", HttpContext);
                var jsonData = new { response = false, responseCode = "01", data = "Invalid Request" };
                return new JsonResult(jsonData);
            }
        }

        [HttpPost]
        [Route("dashbaordbanner")]
        public IActionResult dashbaordbanner([FromBody] JsonElement obj1)
        {
            string json = System.Text.Json.JsonSerializer.Serialize(obj1);
            dynamic data = JObject.Parse(json);
            CompanyInfo.InsertrequestLogTracker("dashbaordbanner full request body: " + JObject.Parse(json), 0, 0, 0, 0, "dashbaordbanner", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
            var validateJsonData = (dynamic)null;
            if (data.clientID == "" || data.clientID == null)
            {
                validateJsonData = new { response = false, responseCode = "02", data = "Invalid Client Id." };
                return new JsonResult(validateJsonData);
            }
            else if (data.branchID == "" || data.branchID == null)
            {
                validateJsonData = new { response = false, responseCode = "02", data = "Invalid Branch Id." };
                return new JsonResult(validateJsonData);
            }

            Model.response.WebResponse response = null;
            Customer obj = new Customer();

            obj.Client_ID = data.clientID;
            obj.Branch_ID = data.branchID;
            try
            {
                if (!SqlInjectionProtector.ReadJsonElementValues(obj1) || !SqlInjectionProtector.ReadJsonElementValuesScript(obj1))
                {
                    var errorResponse = new { response = false, responseCode = "02", data = "Invalid input detected." };
                    return new JsonResult(errorResponse) { StatusCode = 400 };
                }


              

                DateTime now = DateTime.Now;
                string dateToCheck = now.ToString("yyyy-MM-dd");

                MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("dashboardbanner");
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                string where = "";
                _cmd.Parameters.AddWithValue("_whereclause", " and 1 = 1 " + where);
                DataTable dt = db_connection.ExecuteQueryDataTableProcedure(_cmd);
                List<Dictionary<string, object>> banner = new List<Dictionary<string, object>>();

                string _SecurityKey = Calyx_Solutions.Service.srvCommon.SecurityKey();
                MySqlConnector.MySqlCommand cmd = new MySqlConnector.MySqlCommand("sp_select_company_details_by_param");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("_securityKey", _SecurityKey);
                cmd.Parameters.AddWithValue("_clientId", obj.Client_ID);
                DataTable dt_companyurl = db_connection.ExecuteQueryDataTableProcedure(cmd);
                string company_url = Convert.ToString(dt_companyurl.Rows[0]["Company_URL_Admin"]);

                foreach (DataRow dr in dt.Rows)
                {
                    Dictionary<string, object> bannerdata = new Dictionary<string, object>();
                    bannerdata["id"] = dr["id"];
                    bannerdata["banner_title"] = dr["banner_title"];
                    bannerdata["banner_image"] = dr["banner_image"];
                 
                    bannerdata["status_forcustomer"] = dr["status_forcustomer"];
                
                    bannerdata["banner_activefrom"] = dr["banner_activefrom"];
                    bannerdata["banner_activetill"] = dr["banner_activetill"];

                    if(bannerdata["banner_activefrom"]  != null && bannerdata["banner_activetill"] != null) {

                        string startDate = Convert.ToDateTime(bannerdata["banner_activefrom"]).ToString("yyyy-MM-dd");
                        string endDate = Convert.ToDateTime(bannerdata["banner_activetill"]).ToString("yyyy-MM-dd");

                        DateTime targetDate = DateTime.ParseExact(dateToCheck, "yyyy-MM-dd", null);
                        DateTime startDateTime = DateTime.ParseExact(startDate, "yyyy-MM-dd", null);
                        DateTime endDateTime = DateTime.ParseExact(endDate, "yyyy-MM-dd", null);

                        bool isBetween = targetDate >= startDateTime && targetDate <= endDateTime;
                        if (!isBetween)
                        {
                            continue;
                        }                        
                    }

                    string image_link = company_url + bannerdata["banner_image"].ToString();

                    string base64str = CompanyInfo.ConvertImageLinkToBase64(image_link);
                    bannerdata["banner_image_base64str"] = base64str;

                    banner.Add(bannerdata);
                }

                var jsonData = new { response = true, responseCode = "00", data = banner };
                return new JsonResult(jsonData);
            }
            catch (Exception ex)
            {
                CompanyInfo.InsertErrorLogTracker("Exception dashbaordbanner error: " + ex.ToString(), 0, 0, 0, 0, "dashbaordbanner", Convert.ToInt32(data.Branch_ID), Convert.ToInt32(data.Client_ID), "", HttpContext);
                var jsonData = new { response = false, responseCode = "01", data = "Invalid Request" };
                return new JsonResult(jsonData);
            }
        }

        }
}
