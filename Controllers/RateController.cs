using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Data;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;

using System;
using System.Threading.Tasks;
using MySqlConnector;
using Calyx_Solutions.Model;
using Calyx_Solutions.Service;

using static Google.Apis.Requests.BatchRequest;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using RestSharp;
using Microsoft.Net.Http.Headers;
using Auth0.ManagementApi.Models;
using System.Net;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNetCore.Http.Features;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http.Extensions;
using Borland.Vcl;
using System.Linq;
using System.Web.Http.Results;
using System.Web.Helpers;
using System.Web.Script.Serialization;
using System.Text.Json.Nodes;
using System.Web.DynamicData;
using System.Globalization;
using ListToDataTable;
using System.Security.Claims;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Calyx_Solutions.Controllers
{
    [Route("api/[controller]")]    
    [ApiController]

    public class RateController : ControllerBase
    {
        // GET: api/<RateController>
        [HttpGet]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        [HttpPost]
        [Authorize]
        [Route("dashboardrates")]
        public JsonResult dashboardrates([FromBody] JsonElement objdata)
        {
            var returnJsonData = (dynamic)null;
            Model.response.WebResponse response = null;
            try
            {
                string json = System.Text.Json.JsonSerializer.Serialize(objdata);
                dynamic data = JObject.Parse(json);
                CompanyInfo.InsertrequestLogTracker("dashboardrates full request body: " + JObject.Parse(json ), 0, 0, 0, 0, "dashboardrates", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
                var claimsIdentity = User.Identity as ClaimsIdentity;//sanket
                var authHeader = "";
                try { authHeader = HttpContext.Request.Headers["Authorization"].ToString(); } catch (Exception egx) { authHeader = ""; }

                bool auth = AuthController.checkAuth(claimsIdentity, objdata, Convert.ToString(authHeader));
                if (!auth)
                {
                    var errorResponse = new { response = false, responseCode = "403", data = "Access Forbidden" };
                    return new JsonResult(errorResponse) { StatusCode = (int)HttpStatusCode.Forbidden };
                }
                if (!SqlInjectionProtector.ReadJsonElementValues(objdata) || !SqlInjectionProtector.ReadJsonElementValuesScript(objdata))
                {
                    var errorResponse = new { response = false, responseCode = "02", data = "Invalid input detected." };
                    return new JsonResult(errorResponse) { StatusCode = 400 };
                }

                #region validateData
                try
                {
                    if (data.clientID == "" || data.clientID == null)
                    {
                        returnJsonData = new { response = false, responseCode = "02", data = "Invalid Client ID." };
                        return new JsonResult(returnJsonData);
                    }
                    else if (data.branchID == "" || data.branchID == null)
                    {
                        returnJsonData = new { response = false, responseCode = "02", data = "Invalid Branch ID." };
                        return new JsonResult(returnJsonData);
                    }
                    else if (data.countryID == "" || data.countryID == null)
                    {
                        returnJsonData = new { response = false, responseCode = "02", data = "Invalid Country ID." };
                        return new JsonResult(returnJsonData);
                    }                    
                    else if (data.fromCurrencyCode == "" || data.fromCurrencyCode == null)
                    {
                        returnJsonData = new { response = false, responseCode = "02", data = "Invalid Currency Code." };
                        return new JsonResult(returnJsonData);
                    }
                }
                catch (Exception ex)
                {
                    Model.ActivityLogTracker _objActivityLog = new Model.ActivityLogTracker();
                    _objActivityLog.Activity = " Exception Found Rate Error: " + ex.ToString() + " ";
                    _objActivityLog.FunctionName = "dashboardrates";
                    _objActivityLog.Transaction_ID = 0;
                    _objActivityLog.WhoAcessed = 1;
                    _objActivityLog.Branch_ID = 0;
                    _objActivityLog.Client_ID = 0;

                    Service.srvActivityLogTracker srvActivityLog = new Service.srvActivityLogTracker();
                    int _i = srvActivityLog.Create(_objActivityLog, HttpContext);

                    returnJsonData = new { response = false, responseCode = "01", data = "Field is missing." };
                    return new JsonResult(returnJsonData);
                }
                #endregion

                // Initialization objects here
                Transaction obj = JsonConvert.DeserializeObject<Transaction>(json);
                obj.Client_ID = data.clientID;
                obj.Branch_ID = data.branchID;
                obj.FromCurrency_Code = data.fromCurrencyCode;
                obj.Customer_ID = data.customerID;
                obj.Country_ID = data.countryID;
                obj.CB_ID = data.branchID;
                if (data.newAgentBranch == "" || data.newAgentBranch == null)
                { obj.new_agent_branch = 0; data.newAgentBranch = 0; }
                obj.new_agent_branch = data.newAgentBranch;
                obj.new_agent_id = data.newAgentID;

                if (data.newAgentID == "" || data.newAgentID == null)
                { obj.new_agent_id = null; }

                obj.Transaction_From_Flag = data.commentFlag;

                Service.srvRates srv = new Service.srvRates(HttpContext);
                //DataTable li1 = srv.GetRates(obj);
                DataTable li1 = srv.Dashboard_Rates(obj);
                if (li1 != null && li1.Rows.Count > 0)
                {
                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_SUCCESS);
                    response.data = "success";
                    response.ObjData = li1;
                    response.ResponseCode = 0;
                    
                    foreach (DataColumn column in li1.Columns)
                    {
                        // Replace "OldColumnName" with the desired new name
                        column.ColumnName = column.ColumnName.Replace("Currency_Code", "currencyCode");
                        column.ColumnName = column.ColumnName.Replace("Currency_Name", "currencyName");
                        column.ColumnName = column.ColumnName.Replace("Min_Amount", "minAmount" );
                        column.ColumnName = column.ColumnName.Replace("Max_Amount", "maxAmount");
                        column.ColumnName = column.ColumnName.Replace("Rate", "rate");
                        column.ColumnName = column.ColumnName.Replace("ImageName", "imageName");
                        column.ColumnName = column.ColumnName.Replace("Country_Flag", "countryFlag");
                        column.ColumnName = column.ColumnName.Replace("ID", "ID");
                        column.ColumnName = column.ColumnName.Replace("Country_ID", "countryID");
                        column.ColumnName = column.ColumnName.Replace("Currency_ID", "currencyID");
                        column.ColumnName = column.ColumnName.Replace("Transfer_Fees", "transferFees");
                    }

                    var rateData = li1.Rows.OfType<DataRow>()
                .Select(row => li1.Columns.OfType<DataColumn>()
                    .ToDictionary(col => col.ColumnName, c => row[c].ToString()));

                    returnJsonData = new { response = true, responseCode = "00", data = rateData };
                    return new JsonResult(returnJsonData);
                }
                else
                {
                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_TECHNICAL_ERROR);
                    response.ResponseMessage = "No Records Found.";
                    response.ResponseCode = 0;
                    returnJsonData = new { response = false, responseCode = "02", data = response.ResponseMessage };
                    return new JsonResult(returnJsonData);
                }


            }
            catch (Exception ex)
            {
                string json = System.Text.Json.JsonSerializer.Serialize(objdata);
                Login obj = JsonConvert.DeserializeObject<Login>(json);

                response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_TECHNICAL_ERROR);
                response.sErrorExceptionText = ex.ToString();
                response.ResponseMessage = "technical error";
                response.ResponseCode = 3;
                //Error Log Handled
                Model.ErrorLog objError = new Model.ErrorLog();
                objError.User = new Model.User();
                objError.Error = "Api : Dashboard Rates : Currency --" + ex.ToString();
                objError.Date = DateTime.Now;
                objError.User_ID = 1;
                objError.Client_ID = obj.Client_ID;
                objError.Function_Name = "dashboardrates";
                Service.srvErrorLog srvError = new Service.srvErrorLog();
                srvError.Create(objError, HttpContext);

                returnJsonData = new { response = false, responseCode = "01", data = "Invalid request." };
                return new JsonResult(returnJsonData);
            }
            return new JsonResult(returnJsonData);
        }



        // GET api/<RateController>/5


        // POST api/<RateController>

        [HttpPost]
        [Authorize]
        [Route("getpurposeslist")]
        public IActionResult GetPurposes([FromBody] JsonElement obj)
        {
            HttpContext context = HttpContext;
            Model.response.WebResponse response = null;
            string json1 = System.Text.Json.JsonSerializer.Serialize(obj);
            dynamic data = JObject.Parse(json1);
            CompanyInfo.InsertrequestLogTracker("getpurposeslist full request body: " + JObject.Parse(json1), 0, 0, 0, 0, "GetPurposes", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
            var validateJsonData = (dynamic)null;
            var claimsIdentity = User.Identity as ClaimsIdentity;//sanket
            var authHeader = "";
            try { authHeader = HttpContext.Request.Headers["Authorization"].ToString(); } catch (Exception egx) { authHeader = ""; }

            bool auth = AuthController.checkAuth(claimsIdentity, obj, Convert.ToString(authHeader));
            if (!auth)
            {
                var errorResponse = new { response = false, responseCode = "403", data = "Access Forbidden" };
                return new JsonResult(errorResponse) { StatusCode = (int)HttpStatusCode.Forbidden };
            }
            if (!SqlInjectionProtector.ReadJsonElementValues(obj) || !SqlInjectionProtector.ReadJsonElementValuesScript(obj))
            {
                var errorResponse = new { response = false, responseCode = "02", data = "Invalid input detected." };
                return new JsonResult(errorResponse) { StatusCode = 400 };
            }

            int number;
            if (!int.TryParse(Convert.ToString(data.clientID), out number))
            {
                validateJsonData = new { response = false, responseCode = "02", data = "Invalid Client ID." };
                return new JsonResult(validateJsonData);
            }
            if (data.customerID == "" || data.customerID == null)
            {
                validateJsonData = new { response = false, responseCode = "02", data = "Invalid customer ID ." };
                return new JsonResult(validateJsonData);
            }
            

            Transaction obj1 = new Transaction();
            obj1.Client_ID = data.clientID;
            obj1.Customer_ID = data.customerID;
           
            DataTable[] d = new DataTable[6];
            try
            {
                Service.srvPurpose srv = new Service.srvPurpose();
                DataTable li1 = srv.GetPurposes(obj1);
                d[0] = li1; d[0].TableName = "Purpose";

                var listData = li1.Rows.OfType<DataRow>()
               .Select(row => li1.Columns.OfType<DataColumn>()
                   .ToDictionary(col => col.ColumnName, c => row[c]));

                string[] ColumnsToBeDeleted = { "Client_ID" };
                foreach (string ColName in ColumnsToBeDeleted)
                {
                    if (li1.Columns.Contains(ColName))
                        li1.Columns.Remove(ColName);
                }
                int recordFound = 0;
                foreach (DataColumn column in li1.Columns)
                {
                    recordFound = 1;
                    // Replace "OldColumnName" with the desired new name
                    column.ColumnName = column.ColumnName.Replace("Purpose_ID", "purposeID");
                    column.ColumnName = column.ColumnName.Replace("Purpose", "purpose");
                    column.ColumnName = column.ColumnName.Replace("Delete_Status", "activeStatus");
                }
                var returnJsonData = (dynamic)null;
                if (recordFound == 0)
                {
                    returnJsonData = new { response = true, responseCode = "00", data = listData };
                    return new JsonResult(returnJsonData);
                }

                returnJsonData = new { response = true, responseCode = "00", data = listData };
                return new JsonResult(returnJsonData);



            }
            catch (Exception ex)
            {
                validateJsonData = new { response = false, responseCode = "01", data = "Invalid Request" };
                string Acitivy = "Api: GetPurposes " + ex.ToString();
                CompanyInfo.InsertErrorLogTracker(Acitivy.ToString(), 0, 0, 0, 0, "GetPurposes", Convert.ToInt32(obj1.Branch_ID), Convert.ToInt32(obj1.Client_ID), "", context);
                return new JsonResult(validateJsonData);
            }
        }

        [HttpPost]
        [Authorize]
        [Route("getcardlist")]
        public IActionResult GetCardList([FromBody] JsonElement obj)
        {
            HttpContext context = HttpContext;
            Model.response.WebResponse response = null;
            string json1 = System.Text.Json.JsonSerializer.Serialize(obj);
            dynamic data = JObject.Parse(json1);
            CompanyInfo.InsertrequestLogTracker("getcardlist full request body: " + JObject.Parse(json1), 0, 0, 0, 0, "GetCardList", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
            var validateJsonData = (dynamic)null;
            var claimsIdentity = User.Identity as ClaimsIdentity;//sanket
            var authHeader = "";
            try { authHeader = HttpContext.Request.Headers["Authorization"].ToString(); } catch (Exception egx) { authHeader = ""; }

            bool auth = AuthController.checkAuth(claimsIdentity, obj, Convert.ToString(authHeader));
            if (!auth)
            {
                var errorResponse = new { response = false, responseCode = "403", data = "Access Forbidden" };
                return new JsonResult(errorResponse) { StatusCode = (int)HttpStatusCode.Forbidden };
            }
            if (!SqlInjectionProtector.ReadJsonElementValues(obj) || !SqlInjectionProtector.ReadJsonElementValuesScript(obj))
            {
                var errorResponse = new { response = false, responseCode = "02", data = "Invalid input detected." };
                return new JsonResult(errorResponse) { StatusCode = 400 };
            }

            int number;
            if (!int.TryParse(Convert.ToString(data.clientID), out number))
            {
                validateJsonData = new { response = false, responseCode = "02", data = "Invalid Client ID." };
                return new JsonResult(validateJsonData);
            }
            if (data.customerID == "" || data.customerID == null)
            {
                validateJsonData = new { response = false, responseCode = "02", data = "Invalid customer ID ." };
                return new JsonResult(validateJsonData);
            }
            if (data.currencyCode == "" || data.currencyCode == null)
            {
                validateJsonData = new { response = false, responseCode = "02", data = "Invalid Currency Code ." };
                return new JsonResult(validateJsonData);
            }


            Transaction obj1 = new Transaction();
            obj1.Client_ID = data.clientID;
            obj1.Customer_ID = data.customerID;
            obj1.Currency_Code = data.currencyCode;
            DataTable[] d = new DataTable[6];
            try
            {
                Service.srvPaymentType srv1 = new Service.srvPaymentType();
                DataTable li1 = srv1.GetPaybyCardTypes(obj1);
                d[0] = li1; d[0].TableName = "Cardtype";

                var listData = li1.Rows.OfType<DataRow>()
               .Select(row => li1.Columns.OfType<DataColumn>()
                   .ToDictionary(col => col.ColumnName, c => row[c]));

                string[] ColumnsToBeDeleted = { "Client_ID" };
                foreach (string ColName in ColumnsToBeDeleted)
                {
                    if (li1.Columns.Contains(ColName))
                        li1.Columns.Remove(ColName);
                }
                int recordFound = 0;
                foreach (DataColumn column in li1.Columns)
                {
                    recordFound = 1;
                    // Replace "OldColumnName" with the desired new name
                    column.ColumnName = column.ColumnName.Replace("ID", "ID");
                    column.ColumnName = column.ColumnName.Replace("PayByCard_TypeName", "payByCardTypeName");
                    column.ColumnName = column.ColumnName.Replace("Admin_Status", "adminStatus");
                    column.ColumnName = column.ColumnName.Replace("Customer_Status", "customerStatus");
                    column.ColumnName = column.ColumnName.Replace("Delete_Status", "activeStatus");
                }
                var returnJsonData = (dynamic)null;
                if (recordFound == 0)
                {
                    returnJsonData = new { response = true, responseCode = "00", data = listData };
                    return new JsonResult(returnJsonData);
                }

                returnJsonData = new { response = true, responseCode = "00", data = listData };
                return new JsonResult(returnJsonData);



            }
            catch (Exception ex)
            {
                validateJsonData = new { response = false, responseCode = "02", data = "Invalid Request" };
                string Acitivy = "Api: getcardlist " + ex.ToString();
                CompanyInfo.InsertErrorLogTracker(Acitivy.ToString(), 0, 0, 0, 0, "getcardlist", Convert.ToInt32(obj1.Branch_ID), Convert.ToInt32(obj1.Client_ID), "", context);
                return new JsonResult(validateJsonData);
            }
        }


        [HttpPost]
        [Authorize]
        [Route("paygetwaylist")]
        public IActionResult PayGatewayList([FromBody] JsonElement obj)
        {
            HttpContext context = HttpContext;
            Model.response.WebResponse response = null;
            string json1 = System.Text.Json.JsonSerializer.Serialize(obj);
            dynamic data = JObject.Parse(json1);
            CompanyInfo.InsertrequestLogTracker("paygetwaylist full request body: " + JObject.Parse(json1), 0, 0, 0, 0, "PayGatewayList", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
            var claimsIdentity = User.Identity as ClaimsIdentity;//sanket
            var authHeader = "";
            try { authHeader = HttpContext.Request.Headers["Authorization"].ToString(); } catch (Exception egx) { authHeader = ""; }

            bool auth = AuthController.checkAuth(claimsIdentity, obj, Convert.ToString(authHeader));
            if (!auth)
            {
                var errorResponse = new { response = false, responseCode = "403", data = "Access Forbidden" };
                return new JsonResult(errorResponse) { StatusCode = (int)HttpStatusCode.Forbidden };
            }
            var validateJsonData = (dynamic)null;

            if (!SqlInjectionProtector.ReadJsonElementValues(obj) || !SqlInjectionProtector.ReadJsonElementValuesScript(obj))
            {
                var errorResponse = new { response = false, responseCode = "02", data = "Invalid input detected." };
                return new JsonResult(errorResponse) { StatusCode = 400 };
            }

            int number;
            if (!int.TryParse(Convert.ToString(data.clientID), out number))
            {
                validateJsonData = new { response = false, responseCode = "02", data = "Invalid Client ID." };
                return new JsonResult(validateJsonData);
            }
            if (data.customerID == "" || data.customerID == null)
            {
                validateJsonData = new { response = false, responseCode = "02", data = "Invalid customer ID ." };
                return new JsonResult(validateJsonData);
            }
            if (data.currencyCode == "" || data.currencyCode == null)
            {
                validateJsonData = new { response = false, responseCode = "02", data = "Invalid Currency Code ." };
                return new JsonResult(validateJsonData);
            }

            Transaction obj1 = new Transaction();
            obj1.Client_ID = data.clientID;
            obj1.Customer_ID = data.customerID;
            obj1.Currency_Code = data.currencyCode;
            DataTable[] d = new DataTable[6];
            try
            {
                Service.srvPaymentType srv3 = new Service.srvPaymentType();
                DataTable li1 = srv3.GetPayGateway(obj1);
                d[0] = li1; d[0].TableName = "PayGatewayList";

                var listData = li1.Rows.OfType<DataRow>()
               .Select(row => li1.Columns.OfType<DataColumn>()
                   .ToDictionary(col => col.ColumnName, c => row[c]));

                string[] ColumnsToBeDeleted = { "Client_ID" };
                foreach (string ColName in ColumnsToBeDeleted)
                {
                    if (li1.Columns.Contains(ColName))
                        li1.Columns.Remove(ColName);
                }
                int recordFound = 0;
                foreach (DataColumn column in li1.Columns)
                {
                    recordFound = 1;
                    // Replace "OldColumnName" with the desired new name
                    column.ColumnName = column.ColumnName.Replace("ID", "ID");
                    column.ColumnName = column.ColumnName.Replace("PaymentGateway_Name", "paymentGatewayName");
                    column.ColumnName = column.ColumnName.Replace("Admin_Status", "adminStatus");
                    column.ColumnName = column.ColumnName.Replace("Customer_Status", "customerStatus");
                    column.ColumnName = column.ColumnName.Replace("Delete_Status", "activeStatus");
                    column.ColumnName = column.ColumnName.Replace("API_Url", "apiUrl");
                    column.ColumnName = column.ColumnName.Replace("APIUnique_Codes", "apiUniqueCodes");
                    column.ColumnName = column.ColumnName.Replace("Preference_No", "preferenceNo");
                }
                var returnJsonData = (dynamic)null;

                CompanyInfo.InsertrequestLogTracker("paygetwaylist Length of dttable: " + li1.Rows.Count, 0, 0, 0, 0, "PayGatewayList", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
                if ( li1.Rows.Count == 0)
                {
                    returnJsonData = new { response = false, responseCode = "02", data = "Currently we don't have service for selected payment type. Please use another payment type and try again." };
                    return new JsonResult(returnJsonData);
                }


                if (recordFound == 0)
                {
                    /*returnJsonData = new { response = true, responseCode = "00", data = listData };
                    return new JsonResult(returnJsonData);*/

                    returnJsonData = new { response = false, responseCode = "02", data = "Currently we don't have service for selected payment type. Please use another payment type and try again." };
                    return new JsonResult(returnJsonData);
                }

                returnJsonData = new { response = true, responseCode = "00", data = listData };
                return new JsonResult(returnJsonData);

            }
            catch (Exception ex)
            {
                validateJsonData = new { response = false, responseCode = "01", data = "Invalid Request" };
                string Acitivy = "Api: paygetwaylist " + ex.ToString();
                CompanyInfo.InsertErrorLogTracker(Acitivy.ToString(), 0, 0, 0, 0, "paygetwaylist", Convert.ToInt32(obj1.Branch_ID), Convert.ToInt32(obj1.Client_ID), "", context);
                return new JsonResult(validateJsonData);
            }
        }


        [HttpPost]
        [Authorize]
        [Route("getwallet")]
        public IActionResult Getwallet([FromBody] JsonElement obj)
        {
            HttpContext context = HttpContext;
            Model.response.WebResponse response = null;
            string json1 = System.Text.Json.JsonSerializer.Serialize(obj);
            dynamic data = JObject.Parse(json1);

            CompanyInfo.InsertrequestLogTracker("getwallet full request body: " + JObject.Parse(json1), 0, 0, 0, 0, "Getwallet", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
            var claimsIdentity = User.Identity as ClaimsIdentity;//sanket
            var authHeader = "";
            try { authHeader = HttpContext.Request.Headers["Authorization"].ToString(); } catch (Exception egx) { authHeader = ""; }

            bool auth = AuthController.checkAuth(claimsIdentity, obj, Convert.ToString(authHeader));
            if (!auth)
            {
                var errorResponse = new { response = false, responseCode = "403", data = "Access Forbidden" };
                return new JsonResult(errorResponse) { StatusCode = (int)HttpStatusCode.Forbidden };
            }
            var validateJsonData = (dynamic)null;

            if (!SqlInjectionProtector.ReadJsonElementValues(obj) || !SqlInjectionProtector.ReadJsonElementValuesScript(obj))
            {
                var errorResponse = new { response = false, responseCode = "02", data = "Invalid input detected." };
                return new JsonResult(errorResponse) { StatusCode = 400 };
            }

            int number;
            if (!int.TryParse(Convert.ToString(data.clientID), out number))
            {
                validateJsonData = new { response = false, responseCode = "02", data = "Invalid Client ID." };
                return new JsonResult(validateJsonData);
            }
            if (data.customerID == "" || data.customerID == null)
            {
                validateJsonData = new { response = false, responseCode = "02", data = "Invalid customer ID ." };
                return new JsonResult(validateJsonData);
            }
           

            Transaction obj1 = new Transaction();
            obj1.Client_ID = data.clientID;
            obj1.Customer_ID = data.customerID;
            obj1.Currency_Code = data.currencyCode;
            DataTable[] d = new DataTable[6];
            try
            {
                Service.srvWallet srv4 = new Service.srvWallet();
                DataTable li1 = srv4.Get_Wallets(obj1);
                d[0] = li1; d[0].TableName = "Get_Wallets";

                try
                {
                    foreach (DataRow row in li1.Rows)
                    {
                        if (row["Wallet_balance"] != DBNull.Value) // Check for NULL values
                        {
                            row["Wallet_balance"] = Math.Round(Convert.ToDecimal(row["Wallet_balance"]), 2);
                        }
                    }
                }
                catch { }

                // Update last column values to two digits
                foreach (DataRow row in li1.Rows)
                {
                    string lastColumnValue = row[li1.Columns.Count - 1].ToString();
                    if (int.TryParse(lastColumnValue, out int numericValue))
                    {                                                
                        row[li1.Columns.Count - 1] = numericValue.ToString("F2");
                    }
                }
                foreach (DataRow row in li1.Rows)
                {
                    Console.WriteLine(string.Join(", ", row.ItemArray));
                }

                var listData = li1.Rows.OfType<DataRow>()
               .Select(row => li1.Columns.OfType<DataColumn>()
                   .ToDictionary(col => col.ColumnName, c => row[c]));

                string[] ColumnsToBeDeleted = { "Client_ID" , "Security_Question_ID", "Security_Question_Answer", "Profile_Image",  "House_Number",
                "Street", "Gender" , "Addressline_2" , "Nationality" , "Profession" ,  "company_name" ,  "Heard_from_Id", "Heard_from_event",
                 "Sourse_of_Registration" , "Comment" ,  "Comment_UserId" , "Remind_Me_Flag" , "Remind_Date" , "Title_ID",  "Heard_from" , "Blacklisted_flag",
                 "Reason_for_blacklist","block_login_flag","Reason_for_blocklogin" ,  "ThridParty_Check" , "Profession_ID" , "Suspicious_Flag", "Place_Of_Birth" ,
                  "Suspicious_Reason" ,  "Nationality_ID" ,  "Suspended_Flag" ,"Suspend_Reason","Middle_Name" ,"File_Reference" ,
                   "Employement_ID",  "Is_Merged",  "Merged_With" , "ProbableMatch_Flag", "Matching_ID" ,"DateOf_Birth","FaceMatch_Score", "Customer_API_ID","Mobile_Verification_flag",
                   "Mobile_Retry_Flag","Email_Retry_Flag", "Retry_Date","Mobile_OTP", "Security_Flag","Security_Date",
                    "Inactivate_Date",  "Verification_flag",  "Cust_Limit" ,"check_rules","Mobile_number_code","Phone_number_code","device_reg_id",
"User_ID",  "Watchlist_flag" , "Reason_for_watchlist", "promotional_flag" ,"provience_id" ,"NameMatchFlag",  "unsubscribe_zoho" , "passcode", "Referred_by_agent"
, "Password" , "WireTransfer_ReferanceNo"  ,"Agent_MappingID","Record_Insert_DateTime", "Delete_Status"
                ,"Send_Money_flag" ,"Exceeded_Amount" ,"Branch_ID" , "RegularcustomerID"
                };

                foreach (string ColName in ColumnsToBeDeleted)
                {
                    if (li1.Columns.Contains(ColName))
                        li1.Columns.Remove(ColName);
                }
                int recordFound = 0;
                foreach (DataColumn column in li1.Columns)
                {
                    recordFound = 1;
                    // Replace "OldColumnName" with the desired new name
                    column.ColumnName = column.ColumnName.Replace("Cust_Reference", "customerReference");
                    column.ColumnName = column.ColumnName.Replace("Wallet_Reference", "walletReference");
                    column.ColumnName = column.ColumnName.Replace("Customer_ID", "customerID");
                    column.ColumnName = column.ColumnName.Replace("First_Name", "firstName");
                    column.ColumnName = column.ColumnName.Replace("Last_Name", "lastName");
                    column.ColumnName = column.ColumnName.Replace("City_ID", "cityID");
                    column.ColumnName = column.ColumnName.Replace("Country_ID", "countryID");
                    column.ColumnName = column.ColumnName.Replace("Post_Code", "postCode");
                    column.ColumnName = column.ColumnName.Replace("Phone_Number", "phoneNumber");
                    column.ColumnName = column.ColumnName.Replace("Mobile_Number", "mobileNumber");
                    column.ColumnName = column.ColumnName.Replace("Email_ID", "emailID");
                    column.ColumnName = column.ColumnName.Replace("Wallet_ID", "walletID");
                    column.ColumnName = column.ColumnName.Replace("Currency_ID", "currencyID");
                    column.ColumnName = column.ColumnName.Replace("Currency_Code", "currencyCode");
                    column.ColumnName = column.ColumnName.Replace("Currency_Image", "currencyImage");
                    column.ColumnName = column.ColumnName.Replace("Wallet_balance", "walletBalance");
                    column.ColumnName = column.ColumnName.Replace("ReKyc_Eligibility", "reKycEligibility");                    
                }
                var returnJsonData = (dynamic)null;
                if (recordFound == 0)
                {
                    returnJsonData = new { response = true, responseCode = "00", data = listData };
                    return new JsonResult(returnJsonData);
                }

                returnJsonData = new { response = true, responseCode = "00", data = listData };
                return new JsonResult(returnJsonData);



            }
            catch (Exception ex)
            {
                validateJsonData = new { response = false, responseCode = "01", data = "Invalid Request" };
                string Acitivy = "Api: getwallet " + ex.ToString();
                CompanyInfo.InsertErrorLogTracker(Acitivy.ToString(), 0, 0, 0, 0, "getwallet", Convert.ToInt32(obj1.Branch_ID), Convert.ToInt32(obj1.Client_ID), "", context);
                return new JsonResult(validateJsonData);
            }
        }


        [HttpPost]      
        [Route("getpermission")]
        public IActionResult GetPerm([FromBody] JsonElement obj)
        {
            HttpContext context = HttpContext;
            Model.response.WebResponse response = null;
            string json1 = System.Text.Json.JsonSerializer.Serialize(obj);
            dynamic data = JObject.Parse(json1);
            CompanyInfo.InsertrequestLogTracker("getpermission full request body: " + JObject.Parse(json1), 0, 0, 0, 0, "GetPerm", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
            var validateJsonData = (dynamic)null;

            if (!SqlInjectionProtector.ReadJsonElementValues(obj) || !SqlInjectionProtector.ReadJsonElementValuesScript(obj))
            {
                var errorResponse = new { response = false, responseCode = "02", data = "Invalid input detected." };
                return new JsonResult(errorResponse) { StatusCode = 400 };
            }

            int number;
            if (!int.TryParse(Convert.ToString(data.clientID), out number))
            {
                validateJsonData = new { response = false, responseCode = "02", data = "Invalid Client ID." };
                return new JsonResult(validateJsonData);
            }

            Transaction obj1 = new Transaction();
            obj1.Client_ID = data.clientID;

            DataTable[] d = new DataTable[6];
            try
            {
                DataTable li1 = (DataTable)CompanyInfo.getEmailPermission(obj1.Client_ID, 0);
                d[4] = li1; d[4].TableName = "Get_Perm";

                var listData = li1.Rows.OfType<DataRow>()
               .Select(row => li1.Columns.OfType<DataColumn>()
                   .ToDictionary(col => col.ColumnName, c => row[c].ToString()));

                string[] ColumnsToBeDeleted = { "Percentage" };
                foreach (string ColName in ColumnsToBeDeleted)
                {
                    if (li1.Columns.Contains(ColName))
                        li1.Columns.Remove(ColName);
                }
                int recordFound = 0;
                foreach (DataColumn column in li1.Columns)
                {
                    recordFound = 1;
                    // Replace "OldColumnName" with the desired new name
                    column.ColumnName = column.ColumnName.Replace("PID", "permissionID");
                    column.ColumnName = column.ColumnName.Replace("Permission_Name", "permissionName");
                    column.ColumnName = column.ColumnName.Replace("Status", "statusForBckofc");
                    column.ColumnName = column.ColumnName.Replace("Status_ForCustomer", "statusForCustomer");
                    column.ColumnName = column.ColumnName.Replace("Status_ForAgent", "statusForAgent");
                    column.ColumnName = column.ColumnName.Replace("Delete_Status", "deleteStatus");
                    column.ColumnName = column.ColumnName.Replace("RecInserted_Date", "recInsertedDate");
                    column.ColumnName = column.ColumnName.Replace("RecUpdated_Date", "recUpdatedDate");
                    column.ColumnName = column.ColumnName.Replace("Branch_ID", "branchID");
                    column.ColumnName = column.ColumnName.Replace("Alert_Msg", "alertMsg");
                    column.ColumnName = column.ColumnName.Replace("Permission_Description", "permissionDescription");
                    column.ColumnName = column.ColumnName.Replace("RatePermission_flag", "ratePermissionflag");
                    column.ColumnName = column.ColumnName.Replace("AdminPermission_flag", "adminPermissionflag");

                }
                var returnJsonData = (dynamic)null;
                if (recordFound == 0)
                {
                    returnJsonData = new { response = true, responseCode = "00", data = listData };
                    return new JsonResult(returnJsonData);
                }

                returnJsonData = new { response = true, responseCode = "00", data = listData };
                return new JsonResult(returnJsonData);



            }
            catch (Exception ex)
            {
                validateJsonData = new { response = false, responseCode = "01", data = "Invalid Request" };
                string Acitivy = "Api: getpermission " + ex.ToString();
                CompanyInfo.InsertErrorLogTracker(Acitivy.ToString(), 0, 0, 0, 0, "getpermission", Convert.ToInt32(obj1.Branch_ID), Convert.ToInt32(obj1.Client_ID), "", context);
                return new JsonResult(validateJsonData);
            }
        }

        [HttpPost]
        [Authorize]
        [Route("getinstantbankapi")]
        public IActionResult Get_instantBankAPI([FromBody] JsonElement obj)
        {
            HttpContext context = HttpContext;
            Model.response.WebResponse response = null;
            string json1 = System.Text.Json.JsonSerializer.Serialize(obj);
            dynamic data = JObject.Parse(json1);
            CompanyInfo.InsertrequestLogTracker("getinstantbankapi full request body: " + JObject.Parse(json1), 0, 0, 0, 0, "Get_instantBankAPI", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
            var validateJsonData = (dynamic)null;
            var claimsIdentity = User.Identity as ClaimsIdentity;//sanket
            var authHeader = "";
            try { authHeader = HttpContext.Request.Headers["Authorization"].ToString(); } catch (Exception egx) { authHeader = ""; }

            bool auth = AuthController.checkAuth(claimsIdentity, obj, Convert.ToString(authHeader));
            if (!auth)
            {
                var errorResponse = new { response = false, responseCode = "403", data = "Access Forbidden" };
                return new JsonResult(errorResponse) { StatusCode = (int)HttpStatusCode.Forbidden };
            }
            if (!SqlInjectionProtector.ReadJsonElementValues(obj) || !SqlInjectionProtector.ReadJsonElementValuesScript(obj))
            {
                var errorResponse = new { response = false, responseCode = "02", data = "Invalid input detected." };
                return new JsonResult(errorResponse) { StatusCode = 400 };
            }

            int number;
            if (!int.TryParse(Convert.ToString(data.clientID), out number))
            {
                validateJsonData = new { response = false, responseCode = "02", data = "Invalid Client ID." };
                return new JsonResult(validateJsonData);
            }
            if (data.customerID == "" || data.customerID == null)
            {
                validateJsonData = new { response = false, responseCode = "02", data = "Invalid customer ID ." };
                return new JsonResult(validateJsonData);
            }
            if (data.currencyCode == "" || data.currencyCode == null)
            {
                validateJsonData = new { response = false, responseCode = "02", data = "Invalid Currency Code ." };
                return new JsonResult(validateJsonData);
            }

            Transaction obj1 = new Transaction();
            obj1.Client_ID = data.clientID;
            obj1.Customer_ID = data.customerID;
            obj1.Currency_Code = data.currencyCode;
            DataTable[] d = new DataTable[6];
            try
            {
                Service.srvPaymentType srv3 = new Service.srvPaymentType();
                DataTable li1 = srv3.GetinstantBankAPIDetails(obj1);
                //DataTable li1 = new DataTable();
                d[5] = li1; d[5].TableName = "Get_instantBankAPI";

                var listData = li1.Rows.OfType<DataRow>()
               .Select(row => li1.Columns.OfType<DataColumn>()
                   .ToDictionary(col => col.ColumnName, c => row[c]));

                string[] ColumnsToBeDeleted = { "Client_ID" };
                foreach (string ColName in ColumnsToBeDeleted)
                {
                    if (li1.Columns.Contains(ColName))
                        li1.Columns.Remove(ColName);
                }
                int recordFound = 0;
                foreach (DataColumn column in li1.Columns)
                {
                    recordFound = 1;
                    // Replace "OldColumnName" with the desired new name
                    column.ColumnName = column.ColumnName.Replace("bank_api_id", "bankApiID");
                    column.ColumnName = column.ColumnName.Replace("Company_Name", "companyName");
                    column.ColumnName = column.ColumnName.Replace("API_URL", "apiUrl");
                    column.ColumnName = column.ColumnName.Replace("UserName", "userName");
                    column.ColumnName = column.ColumnName.Replace("Password", "password");
                    column.ColumnName = column.ColumnName.Replace("API_Status", "apiStatus");
                    column.ColumnName = column.ColumnName.Replace("Delete_Status", "deleteStatus");
                    column.ColumnName = column.ColumnName.Replace("APIUnique_Codes", "apiUniqueCodes");
                    column.ColumnName = column.ColumnName.Replace("Preference_No", "preferenceNo");
                    column.ColumnName = column.ColumnName.Replace("Admin_Status", "adminStatus");
                    column.ColumnName = column.ColumnName.Replace("Customer_Status", "customerStatus");
                }
                var returnJsonData = (dynamic)null;

                if (li1.Rows.Count == 0)
                {
                    returnJsonData = new { response = false, responseCode = "02", data = "Currently we don't have service for selected payment type. Please use another payment type and try again." };
                    return new JsonResult(returnJsonData);
                }

                if (recordFound == 0)
                {
                    /*returnJsonData = new { response = true, responseCode = "00", data = listData };
                    return new JsonResult(returnJsonData);*/
                    returnJsonData = new { response = false, responseCode = "02", data = "Currently we don't have service for selected payment type. Please use another payment type and try again." };
                    return new JsonResult(returnJsonData);
                }

                returnJsonData = new { response = true, responseCode = "00", data = listData };
                return new JsonResult(returnJsonData);

            }
            catch (Exception ex)
            {
                validateJsonData = new { response = false, responseCode = "01", data = "Invalid Request" };
                string Acitivy = "Api: getinstantbankapi " + ex.ToString();
                CompanyInfo.InsertErrorLogTracker(Acitivy.ToString(), 0, 0, 0, 0, "getinstantbankapi", Convert.ToInt32(obj1.Branch_ID), Convert.ToInt32(obj1.Client_ID), "", context);
                return new JsonResult(validateJsonData);
            }
        }



        [HttpPost]
        [Authorize]
        [Route("apicollectionlist")]
        public IActionResult Get_BKLBranchList([FromBody] JsonElement Obj)
        {
            HttpContext context = HttpContext;
            Model.response.WebResponse response = null;
            string json1 = System.Text.Json.JsonSerializer.Serialize(Obj);
            dynamic data = JObject.Parse(json1);
            CompanyInfo.InsertrequestLogTracker("apicollectionlist full request body: " + JObject.Parse(json1), 0, 0, 0, 0, "Get_BKLBranchList", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
            var validateJsonData = (dynamic)null;
            Transaction obj1 = new Transaction();


            try
            {

                if (!SqlInjectionProtector.ReadJsonElementValues(Obj) || !SqlInjectionProtector.ReadJsonElementValuesScript(Obj))
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
                    validateJsonData = new { response = false, responseCode = "02", data = "Invalid Branch ID." };
                    return new JsonResult(validateJsonData);
                }
                if (data.countryID == "" || data.countryID == null)
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "Invalid Country ID." };
                    return new JsonResult(validateJsonData);
                }

                if (data.status == "" || data.status == null)
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "Invalid Status ID." };
                    return new JsonResult(validateJsonData);
                }
                if (data.cityID == "" || data.cityID == null)
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "Invalid City ID." };
                    return new JsonResult(validateJsonData);
                }
                if (data.paymentDepositTypeID == "" || data.paymentDepositTypeID == null)
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "Invalid Payment Deposite ID." };
                    return new JsonResult(validateJsonData);
                }


                obj1.Client_ID = data.clientID;
                obj1.Branch_ID = data.branchID;
                obj1.Country_ID = data.countryID;
                obj1.CollectionPoint = data.collectionPoint;
                obj1.Currency_Code = data.currencyCode;
                obj1.Delete_Status = data.status;
                obj1.City_ID = data.cityID;
                obj1.PaymentDepositType_ID = data.paymentDepositTypeID;


                Service.srvRates srv = new Service.srvRates(context);
                DataTable li1 = srv.Get_BKLBranchList(obj1);
                if (li1 != null && li1.Rows.Count > 0)
                {
                    List<Dictionary<string, object>> collectionpoint = new List<Dictionary<string, object>>();

                    foreach (DataRow dr in li1.Rows)
                    {
                        Dictionary<string, object> collection_point = new Dictionary<string, object>();
                        collection_point["branchCode"] = dr["Branchcode"];
                        collection_point["city"] = dr["City"];
                        collection_point["country"] = dr["Country"];
                        collection_point["address"] = dr["Address"];
                        collection_point["apiID"] = dr["API_ID"];

                        collectionpoint.Add(collection_point);
                    }

                    validateJsonData = new { response = true, responseCode = "00", data = collectionpoint };
                    return new JsonResult(validateJsonData);
                }
                else
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "No Records Foound." };
                    return new JsonResult(validateJsonData);

                }
            }
            catch (Exception ex)
            {


                validateJsonData = new { response = false, responseCode = "01", data = "Invalid Request" };
                string Acitivy = "Api: CollectionPoints " + ex.ToString();
                CompanyInfo.InsertErrorLogTracker(Acitivy.ToString(), 0, 0, 0, 0, "CollectionPoints", Convert.ToInt32(obj1.Branch_ID), Convert.ToInt32(obj1.Client_ID), "", context);
                return new JsonResult(validateJsonData);


            }



        }


        // GET api/<RateController>/5


        // POST api/<RateController>
        [HttpPost]
        [ApiExplorerSettings(IgnoreApi = true)]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<RateController>/5
        [HttpPut("{id}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<RateController>/5
        [HttpDelete("{id}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public void Delete(int id)
        {
        }


        [HttpPost]
        [Authorize]
        [Route("collectionpointlist")]

        public IActionResult CollectionPoints([FromBody] JsonElement Obj)
        {
            HttpContext context = HttpContext;
            Model.response.WebResponse response = null;
            string json1 = System.Text.Json.JsonSerializer.Serialize(Obj);
            dynamic data = JObject.Parse(json1);
            var validateJsonData = (dynamic)null;
            Transaction obj1 = new Transaction();
            CompanyInfo.InsertrequestLogTracker("collectionpointlist full request body: " + JObject.Parse(json1), 0, 0, 0, 0, "CollectionPoints", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
            try
            {
                if (!SqlInjectionProtector.ReadJsonElementValues(Obj) || !SqlInjectionProtector.ReadJsonElementValuesScript(Obj))
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
                    validateJsonData = new { response = false, responseCode = "02", data = "Invalid Branch ID." };
                    return new JsonResult(validateJsonData);
                }
                if (data.countryID == "" || data.countryID == null)
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "Invalid Country ID." };
                    return new JsonResult(validateJsonData);
                }

                if (data.status == "" || data.status == null)
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "Invalid Status ID." };
                    return new JsonResult(validateJsonData);
                }
                if (data.cityID == "" || data.cityID == null)
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "Invalid City ID." };
                    return new JsonResult(validateJsonData);
                }
                if (data.paymentDepositTypeID == "" || data.paymentDepositTypeID == null)
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "Invalid Payment Deposite ID." };
                    return new JsonResult(validateJsonData);
                }


                obj1.Client_ID = data.clientID;
                obj1.Branch_ID = data.branchID;
                obj1.Country_ID = data.countryID;
                obj1.CollectionPoint = data.collectionPoint;
                obj1.Currency_Code = data.currencyCode;
                obj1.Delete_Status = data.status;
                obj1.City_ID = data.cityID;
                obj1.PaymentDepositType_ID = data.paymentDepositTypeID;

                if (!SqlInjectionProtector.ValidateObjectForSqlInjection(obj1) || !SqlInjectionProtector.ValidateObjectForScriptSqlInjection(obj1))
                {
                    return new JsonResult(validateJsonData) { StatusCode = StatusCodes.Status400BadRequest, Value = "Invalid input detected." };
                }

                Service.srvRates srv = new Service.srvRates(context);
                DataTable li1 = new DataTable(); DataTable dt_new = new DataTable();
                dt_new.Columns.Add("ID", typeof(int));
                dt_new.Columns.Add("LocationName", typeof(string));
                dt_new.Columns.Add("APIid", typeof(string));

                



                DataSet dt = new DataSet();



                DataTable li14 = srv.CollectionPoints(obj1); 

                DataTable li15 = srv.Get_BKLBranchList(obj1);

                if (li15.Rows.Count > 0)
                {
                    for (var i = 0; i < li15.Rows.Count; i++)
                    {
                        var ID = ""; string LocationName = ""; string API_ID = "";

                        ID = Convert.ToString(li15.Rows[i]["API_ID"]);
                        LocationName = Convert.ToString(li15.Rows[i]["Branchcode"]) + " - " + Convert.ToString(li15.Rows[i]["Address"]) + Convert.ToString(li15.Rows[i]["City"]) + " - " + Convert.ToString(li15.Rows[i]["Country"]);
                        API_ID = "API" + Convert.ToString(li15.Rows[i]["API_ID"]);
                        dt_new.Rows.Add(ID, LocationName, API_ID);
                    }
                }

                if (li14.Rows.Count > 0)
                {
                    for (var i = 0; i < li14.Rows.Count; i++)
                    {
                        var ID = ""; string LocationName = "";

                        ID = Convert.ToString(li14.Rows[i]["ID"]);
                        LocationName = Convert.ToString(li14.Rows[i]["Location_Name"]) + " - " + Convert.ToString(li14.Rows[i]["Address"]) + " " + Convert.ToString(li14.Rows[i]["Country_Name"]);

                        dt_new.Rows.Add(ID, LocationName, "");
                    }
                }




                if (dt_new != null && dt_new.Rows.Count > 0)
                {
                    List<Dictionary<string, object>> collectionpoint = new List<Dictionary<string, object>>();

                    foreach (DataRow dr in dt_new.Rows)
                    {
                        Dictionary<string, object> collection_point = new Dictionary<string, object>();
                        collection_point["collectionPointID"] = dr["ID"];
                        collection_point["locationName"] = dr["LocationName"];
                        collection_point["apiID"] = dr["APIid"];

                        collectionpoint.Add(collection_point);
                    }

                    validateJsonData = new { response = true, responseCode = "00", data = collectionpoint };
                    return new JsonResult(validateJsonData);
                }
                else
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "No Records Foound." };
                    return new JsonResult(validateJsonData);
                }
            }
            catch (Exception ex)
            {
                validateJsonData = new { response = false, responseCode = "01", data = "Invalid Request" };
                string Acitivy = "Api: CollectionPoints " + ex.ToString();
                CompanyInfo.InsertErrorLogTracker(Acitivy.ToString(), 0, 0, 0, 0, "CollectionPoints", Convert.ToInt32(obj1.Branch_ID), Convert.ToInt32(obj1.Client_ID), "", context);
                return new JsonResult(validateJsonData);
            }



        }


        [HttpPost]
        [Authorize]
        [Route("sendmoneyratecalculator")]

        public IActionResult GetData([FromBody] JsonElement Obj)
        {
            HttpContext context = HttpContext;
            Model.response.WebResponse response = null;
            string json1 = System.Text.Json.JsonSerializer.Serialize(Obj);
            dynamic data = JObject.Parse(json1);
            _ = Task.Run(() => CompanyInfo.InsertrequestLogTracker("sendmoneyratecalculator full request body: " + JObject.Parse(json1), 0, 0, 0, 0, "GetData", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext));
            var validateJsonData = (dynamic)null;
            Transaction obj1 = new Transaction();

            try
            {
                if (!SqlInjectionProtector.ReadJsonElementValues(Obj) || !SqlInjectionProtector.ReadJsonElementValuesScript(Obj))
                {
                    var errorResponse = new { response = false, responseCode = "02", data = "Invalid input detected." };
                    return new JsonResult(errorResponse) { StatusCode = 400 };
                }

                /*if (data.clientID == "" || data.clientID == null)
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "Invalid Client ID." };
                    return new JsonResult(validateJsonData);
                }
                if (data.branchID == "" || data.branchID == null)
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "Invalid Branch ID." };
                    return new JsonResult(validateJsonData);
                }
                if (data.countryID == "" || data.countryID == null)
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "Invalid Country ID." };
                    return new JsonResult(validateJsonData);
                }
                if (data.paymentDepositTypeID == "" || data.paymentDepositTypeID == null)
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "Invalid Payment Deposite ID." };
                    return new JsonResult(validateJsonData);
                }*/

                obj1.Country_ID = data.countryID;
                obj1.Currency_ID = data.currencyID;
                obj1.new_agent_id = data.newAgentID;
                obj1.new_agent_branch = data.newAgentBranch;
                obj1.CB_ID = data.branchID;
                obj1.PType_ID = data.pTypeID;
                obj1.PaymentDepositType_ID = data.paymentDepositTypeID;
                obj1.DeliveryType_Id = data.deliveryTypeID;
                //obj1.AmountInGBP = data.TransferAmount;
                //obj1.AmountInPKR = data.TransferForeignAmount;
                obj1.FromCurrency_Code = data.fromCurrencyCode;
                obj1.Client_ID = data.clientID;


                try { obj1.Customer_ID = data.Customer_ID; } catch (Exception ex) { obj1.Customer_ID = ""; }
                try { obj1.offer_rate_flag = data.offer_rate_flag; } catch(Exception ex) { obj1.offer_rate_flag = 1; }
                try { obj1.improved_rate_flag = data.improved_rate_flag; } catch (Exception ex) { obj1.improved_rate_flag = 1; }

                if (!SqlInjectionProtector.ValidateObjectForSqlInjection(obj1) || !SqlInjectionProtector.ValidateObjectForScriptSqlInjection(obj1))
                {
                    return new JsonResult(validateJsonData) { StatusCode = StatusCodes.Status400BadRequest, Value = "Invalid input detected." };
                }


                Service.srvRates srv = new Service.srvRates(context);
                DataTable li1 = srv.GetRates(obj1);
                List<Dictionary<string, object>> rateList = new List<Dictionary<string, object>>();
                if (li1 != null && li1.Rows.Count > 0)
                {
                    /*var dtData = li1.Rows.OfType<DataRow>()
               .Select(row => li1.Columns.OfType<DataColumn>()
                   .ToDictionary(col => col.ColumnName, c => row[c].ToString()));
                    validateJsonData = new { response = true, responseCode = "00", data = dtData };
                    return new JsonResult(validateJsonData);*/

                    /*foreach (DataRow dr in li1.Rows)
                    {
                        Dictionary<string, object> rate_list = new Dictionary<string, object>();
                        rate_list["ID"] = dr["ID"];
                        rate_list["countryName"] = dr["Country_Name"];
                        rate_list["countryFlag"] = dr["Country_Flag"];
                        rate_list["currencyName"] = dr["Currency_Name"];
                        rate_list["currencyCode"] = dr["Currency_Code"];
                        rate_list["imageName"] = dr["ImageName"];
                        rate_list["branch"] = dr["Branch"];
                        rate_list["paymentType"] = dr["PType"];
                        rate_list["collectionType"] = dr["Collection_Type"];
                        rate_list["deliveryType"] = dr["Delivery_Type"];
                        rate_list["ID1"] = dr["ID1"];
                        rate_list["baseCurrencyID"] = dr["BaseCurrency_ID"];
                        rate_list["countryID"] = dr["Country_ID"];
                        rate_list["currencyID"] = dr["Currency_ID"];
                        rate_list["branchID"] = dr["CB_ID"];
                        rate_list["paymentTypeID"] = dr["PType_ID"];
                        rate_list["payDepositeTypeID"] = dr["PayDepositType_ID"];
                        rate_list["deliveryTypeID"] = dr["DeliveryType_ID"];
                        rate_list["minAmount"] = dr["Min_Amount"];
                        rate_list["maxAmount"] = dr["Max_Amount"];
                        rate_list["foreginCurrencyMinAmount"] = dr["Foreign_Currency_Min_Amount"];
                        rate_list["foreginCurrencyMaxAmount"] = dr["Foreign_Currency_Max_Amount"];
                        rate_list["transferFees"] = dr["Transfer_Fees"];
                        rate_list["feeType"] = dr["Fee_Type"];
                        rate_list["Rate"] = dr["Rate"];
                        rate_list["marginPercent"] = dr["Margin_Percent"];
                        rate_list["comparePercent"] = dr["Compare_Percent"];
                        rate_list["spotRate"] = dr["Spot_Rate"];
                        rate_list["compareRate"] = dr["Compare_Rate"];
                        rate_list["userID"] = dr["User_ID"];
                        rate_list["deleteStatus"] = dr["Delete_Status"];
                        rate_list["clientID"] = dr["Client_ID"];

                        rateList.Add(rate_list);
                    }
                    validateJsonData = new { response = true, responseCode = "00", data = rateList };
                    return new JsonResult(validateJsonData);*/

                    if (li1 != null && li1.Rows.Count > 0)
                    {
                        li1.Columns["ID"].ColumnName = "ID";
                        li1.Columns["Country_Name"].ColumnName = "countryName";
                        li1.Columns["Country_Flag"].ColumnName = "countryFlag";
                        li1.Columns["Currency_Name"].ColumnName = "currencyName";
                        li1.Columns["Currency_Code"].ColumnName = "currencyCode";
                        li1.Columns["ImageName"].ColumnName = "imageName";
                        li1.Columns["Branch"].ColumnName = "branch";
                        li1.Columns["PType"].ColumnName = "paymentType";
                        li1.Columns["Collection_Type"].ColumnName = "collectionType";
                        li1.Columns["Delivery_Type"].ColumnName = "deliveryType";
                        li1.Columns["ID1"].ColumnName = "ID1";
                        li1.Columns["BaseCurrency_ID"].ColumnName = "baseCurrencyID";
                        li1.Columns["Country_ID"].ColumnName = "countryID";
                        li1.Columns["Currency_ID"].ColumnName = "currencyID";
                        li1.Columns["CB_ID"].ColumnName = "branchID";
                        li1.Columns["PType_ID"].ColumnName = "paymentTypeID";
                        li1.Columns["PayDepositType_ID"].ColumnName = "payDepositeTypeID";
                        li1.Columns["DeliveryType_ID"].ColumnName = "deliveryTypeID";
                        li1.Columns["Min_Amount"].ColumnName = "minAmount";
                        li1.Columns["Max_Amount"].ColumnName = "maxAmount";
                        li1.Columns["Foreign_Currency_Min_Amount"].ColumnName = "foreginCurrencyMinAmount";
                        li1.Columns["Foreign_Currency_Max_Amount"].ColumnName = "foreginCurrencyMaxAmount";
                        li1.Columns["Transfer_Fees"].ColumnName = "transferFees";
                        li1.Columns["Fee_Type"].ColumnName = "feeType";
                        li1.Columns["Rate"].ColumnName = "Rate";
                        li1.Columns["Margin_Percent"].ColumnName = "marginPercent";
                        li1.Columns["Compare_Percent"].ColumnName = "comparePercent";
                        li1.Columns["Spot_Rate"].ColumnName = "spotRate";
                        li1.Columns["Compare_Rate"].ColumnName = "compareRate";
                        li1.Columns["User_ID"].ColumnName = "userID";
                        li1.Columns["Delete_Status"].ColumnName = "deleteStatus";
                        li1.Columns["Client_ID"].ColumnName = "clientID";

                        
                    }
                    var relationshipData = li1.Rows.OfType<DataRow>()
               .Select(row => li1.Columns.OfType<DataColumn>()
                   .ToDictionary(col => col.ColumnName, c => row[c].ToString()));
                    validateJsonData = new { response = true, responseCode = "00", data = relationshipData };
                    return new JsonResult(validateJsonData);

                }
                else
                {
                    validateJsonData = new { response = true, responseCode = "00", data = rateList };
                    return new JsonResult(validateJsonData);

                }
            }
            catch (Exception ex)
            {
                validateJsonData = new { response = false, responseCode = "01", data = "Invalid Request" };
                string Acitivy = "Api: sendmoneyratecalculator " + ex.ToString();
                CompanyInfo.InsertErrorLogTracker(Acitivy.ToString(), 0, 0, 0, 0, "sendmoneyratecalculator", Convert.ToInt32(obj1.Branch_ID), Convert.ToInt32(obj1.Client_ID), "", context);
                return new JsonResult(validateJsonData);
            }
        }

        // Convert DataTable to List<Dictionary<string, object>> for easier JSON serialization
        static List<Dictionary<string, object>> ConvertDataTableToList(DataTable dt)
        {
            var result = new List<Dictionary<string, object>>();

            foreach (DataRow row in dt.Rows)
            {
                var dict = new Dictionary<string, object>();
                foreach (DataColumn col in dt.Columns)
                {
                    dict[col.ColumnName] = row[col].ToString(); ; // Collect only column data, not metadata
                }
                result.Add(dict);
            }

            return result;
        }

        

        [HttpPost]
        [Authorize]
        [Route("getbasedata")]
        /* public IActionResult GetBaseData([FromBody] JsonElement ObjV)
         {
             HttpContext context = HttpContext;
             Model.response.WebResponse response = null;
             string json1 = System.Text.Json.JsonSerializer.Serialize(ObjV);
             dynamic data = JObject.Parse(json1);
             _ = Task.Run(() => CompanyInfo.InsertrequestLogTracker("getbasedata full request body: " + JObject.Parse(json1), 0, 0, 0, 0, "GetBaseData", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext));
             var validateJsonData = (dynamic)null;
             Transaction obj  = new Transaction();
             try
             {
                 if (!SqlInjectionProtector.ReadJsonElementValues(ObjV) || !SqlInjectionProtector.ReadJsonElementValuesScript(ObjV))
                 {
                     var errorResponse = new { response = false, responseCode = "02", data = "Invalid input detected." };
                     return new JsonResult(errorResponse) { StatusCode = 400 };
                 }

                 obj.Customer_ID = data.Customer_ID;
                 obj.UserName = data.UserName;
                 obj.Is_App = data.Is_App;
                 obj.Country_ID= data.Country_ID;
                 obj.Basecurr_Country_ID=data.Basecurr_Country_ID;
                 obj.PaymentDepositType_ID=data.PaymentDepositType_ID;
                 obj.Client_ID = data.Client_ID;
                 obj.Branch_ID = data.Branch_ID;
                 obj.TransferTypeFlag = data.TransferTypeFlag;

                 Service.srvDeliveryType srv = new Service.srvDeliveryType();
                 DataTable li1 = srv.getDeliveryTypes(obj);
                 if (li1 != null && li1.Rows.Count > 0)
                 {
                      DataTable[] d = new DataTable[5];//**** Set Length here

                     d[0] = li1; d[0].TableName = "DType";
                     Service.srvPaymentType srv1 = new Service.srvPaymentType();
                     DataTable li2 = srv1.GetPaymentTypes(obj);
                     d[1] = li2; d[1].TableName = "PType";
                     Service.srvPaymentDepositType srv3 = new Service.srvPaymentDepositType();
                     DataTable li3 = srv3.GetPayDepositTypes(obj);
                     d[2] = li3; d[2].TableName = "CType";
                     Service.srvRates srv4 = new Service.srvRates(context);
                     DataTable li4 = srv4.GetActiveCountries(obj);
                     d[3] = li4; d[3].TableName = "dtCountry";
                     DataTable li5 = srv.GetDeliveryMapping(obj);
                     d[4] = li5; d[4].TableName = "DMType";
                     int Customer_ID = Convert.ToInt32(CompanyInfo.Decrypt(obj.Customer_ID, true));

                     _ = Task.Run(() => CompanyInfo.InsertActivityLogDetails("App - Customer " + obj.UserName + " is on send money step 2. All transaction related details are shown. Customer ID: " + Customer_ID + "", obj.User_ID, obj.Transaction_ID, obj.User_ID, Customer_ID, "Send-Get Base Data", obj.CB_ID, obj.Client_ID, "Send Money Step 2",context));


                     var dt1List = ConvertDataTableToList(li1);
                     var dt2List = ConvertDataTableToList(li2);
                     var dt3List = ConvertDataTableToList(li3);
                     var dt4List = ConvertDataTableToList(li4);
                     var dt5List = ConvertDataTableToList(li5);

                     // Prepare the data list
                     var datad = new
                     {
                         DType = dt1List,
                         PType = dt2List,
                         CType = dt3List,
                         dtCountry = dt4List,
                         DMType = dt5List
                     };

                     // Create a structured response
                     validateJsonData = new
                     {
                         response = true,
                         responseCode = "00",
                         data = datad
                     };

                     return Ok(validateJsonData);                                      
                 }
                 else
                 {
                     response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_TECHNICAL_ERROR);
                     response.ResponseMessage = "No Records Found.";
                     response.ResponseCode = 0;
                     validateJsonData = new { response = false, responseCode = "02", data = response.ResponseMessage };
                 }                              
             }
             catch (Exception ex)
             {
                 response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_TECHNICAL_ERROR);
                 response.sErrorExceptionText = ex.ToString();
                 response.ResponseMessage = "technical error";
                 response.ResponseCode = 3;
                 //Error Log Handled
                 Model.ErrorLog objError = new Model.ErrorLog();
                 objError.User = new Model.User();
                 objError.Error = "Api : Rate : Delivery Types --" + ex.ToString();
                 objError.Date = DateTime.Now;
                 objError.User_ID = 1;
                 objError.Client_ID = data.Client_ID;
                 objError.Function_Name = "GetBaseData";
                 Service.srvErrorLog srvError = new Service.srvErrorLog();
                 srvError.Create(objError, context);
                 validateJsonData = new { response = false, responseCode = "01", data = "Invalid request" };

             }
             return new JsonResult(validateJsonData);
         }
         */

        public async Task<IActionResult> GetBaseData([FromBody] JsonElement ObjV)
        {
            HttpContext context = HttpContext;
            Model.response.WebResponse response = null;

            string json1 = System.Text.Json.JsonSerializer.Serialize(ObjV);
            dynamic data = JObject.Parse(json1);
            _ = Task.Run(() => CompanyInfo.InsertrequestLogTracker("getbasedata full request body: " + JObject.Parse(json1), 0, 0, 0, 0, "GetBaseData", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext));


            // Deserialize JSON more efficiently
            //var requestData = JsonSerializer.Deserialize<RequestModel>(ObjV.ToString());
            var requestData = JsonConvert.DeserializeObject<Transaction>(ObjV.ToString());
            var claimsIdentity = User.Identity as ClaimsIdentity;//sanket
            var authHeader = "";
            try { authHeader = HttpContext.Request.Headers["Authorization"].ToString(); } catch (Exception egx) { authHeader = ""; }

            bool auth = AuthController.checkAuth(claimsIdentity, ObjV, Convert.ToString(authHeader));
            if (!auth)
            {
                var errorResponse = new { response = false, responseCode = "403", data = "Access Forbidden" };
                return new JsonResult(errorResponse) { StatusCode = (int)HttpStatusCode.Forbidden };
            }
            if (requestData == null ||
                !SqlInjectionProtector.ReadJsonElementValues(ObjV) ||
                !SqlInjectionProtector.ReadJsonElementValuesScript(ObjV))
            {
                return BadRequest(new { response = false, responseCode = "02", data = "Invalid input detected." });
            }

            int Benf_BankDetails_ID = 0;
            try
            {
                Benf_BankDetails_ID= requestData.Benf_BankDetails_ID;
            }
            catch(Exception ex) { Benf_BankDetails_ID = 0; }

            Transaction obj = null; 
            if (Benf_BankDetails_ID > 0)
            {
                 obj = new Transaction
                {
                    Customer_ID = requestData.Customer_ID,
                    UserName = requestData.UserName,
                    Is_App = requestData.Is_App,
                    Country_ID = requestData.Country_ID,
                    Basecurr_Country_ID = requestData.Basecurr_Country_ID,
                    PaymentDepositType_ID = requestData.PaymentDepositType_ID,
                    Client_ID = requestData.Client_ID,
                    Branch_ID = requestData.Branch_ID,
                    TransferTypeFlag = requestData.TransferTypeFlag,
                    Benf_BankDetails_ID = requestData.Benf_BankDetails_ID //New anushka240325               
                };
            }
            else
            {
                 obj = new Transaction
                {
                    Customer_ID = requestData.Customer_ID,
                    UserName = requestData.UserName,
                    Is_App = requestData.Is_App,
                    Country_ID = requestData.Country_ID,
                    Basecurr_Country_ID = requestData.Basecurr_Country_ID,
                    PaymentDepositType_ID = requestData.PaymentDepositType_ID,
                    Client_ID = requestData.Client_ID,
                    Branch_ID = requestData.Branch_ID,
                    TransferTypeFlag = requestData.TransferTypeFlag             
                };
            }


            try
            {
                var srv = new Service.srvDeliveryType();
                var srv1 = new Service.srvPaymentType();
                var srv3 = new Service.srvPaymentDepositType();
                var srv4 = new Service.srvRates(context);

                // Fetch data in parallel
                var task1 = Task.Run(() => srv.getDeliveryTypes(obj));
                var task2 = Task.Run(() => srv1.GetPaymentTypes(obj));
                var task3 = Task.Run(() => srv3.GetPayDepositTypes(obj));
                var task4 = Task.Run(() => srv4.GetActiveCountries(obj));
                var task5 = Task.Run(() => srv.GetDeliveryMapping(obj));

                await Task.WhenAll(task1, task2, task3, task4, task5);

                if (task1.Result == null || task1.Result.Rows.Count == 0)
                {
                    return NotFound(new { response = false, responseCode = "02", data = "No Records Found."});
                }

                // Convert DataTables to Lists in Parallel
                var dt1List = ConvertDataTableToList(task1.Result);
                var dt2List = ConvertDataTableToList(task2.Result);
                var dt3List = ConvertDataTableToList(task3.Result);
                var dt4List = ConvertDataTableToList(task4.Result);
                var dt5List = ConvertDataTableToList(task5.Result);

                int Customer_ID = Convert.ToInt32(CompanyInfo.Decrypt(obj.Customer_ID, true));

                // Logging should be async and backgrounded
                _ = Task.Run(() => CompanyInfo.InsertActivityLogDetails(
                    $"App - Customer {obj.UserName} is on send money step 2. Customer ID: {Customer_ID}",
                    obj.User_ID, obj.Transaction_ID, obj.User_ID, Customer_ID,
                    "Send-Get Base Data", obj.CB_ID, obj.Client_ID, "Send Money Step 2", context));

                // Response payload
                return Ok(new
                {
                    response = true,
                    responseCode = "00",
                    data = new
                    {
                        DType = dt1List,
                        PType = dt2List,
                        CType = dt3List,
                        dtCountry = dt4List,
                        DMType = dt5List
                    }
                });
            }
            catch (Exception ex)
            {
                response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_TECHNICAL_ERROR)
                {
                    sErrorExceptionText = ex.ToString(),
                    ResponseMessage = "Technical error",
                    ResponseCode = 3
                };

                // Log exception
                _ = Task.Run(() => new Service.srvErrorLog().Create(new Model.ErrorLog
                {
                    Error = $"Api : Rate : Delivery Types -- {ex}",
                    Date = DateTime.Now,
                    User_ID = 1,
                    Client_ID = requestData.Client_ID,
                    Function_Name = "GetBaseData"
                }, context));

                return BadRequest(new { response = false, responseCode = "01", data = "Invalid request" });
            }
        }


        [HttpPost]// Get purpose, paybycard details, wallets
        [Authorize]
        [Route("getadditionaldata")]
        public async Task<IActionResult> GetAdditionalData([FromBody] JsonElement Objdata)
        {
            var returnJsonData = (dynamic)null;
            Model.response.WebResponse response = null;
            HttpContext context = HttpContext;
            var requestData = JsonConvert.DeserializeObject<Transaction>(Objdata.ToString());
            string jsonData = System.Text.Json.JsonSerializer.Serialize(Objdata);
            dynamic data = JObject.Parse(jsonData);

            if (requestData == null ||
                !SqlInjectionProtector.ReadJsonElementValues(Objdata) ||
                !SqlInjectionProtector.ReadJsonElementValuesScript(Objdata))
            {
                return BadRequest(new { response = false, responseCode = "02", data = "Invalid input detected." });
            }
            try
            {
                Transaction obj = JsonConvert.DeserializeObject<Transaction>(jsonData);
                //HttpContext.Current.Session["RewardToken"] = "";
                Service.srvPurpose srv = new Service.srvPurpose();
                DataTable li1 = srv.GetPurposes(obj);

                DataTable[] d = new DataTable[6];//**** Set Length here
                d[0] = li1; d[0].TableName = "Purpose";
                Service.srvPaymentType srv1 = new Service.srvPaymentType();
                DataTable li2 = srv1.GetPaybyCardTypes(obj);
                d[1] = li2; d[1].TableName = "CardType";
                Service.srvPaymentType srv3 = new Service.srvPaymentType();
                DataTable li3 = srv3.GetPayGateway(obj);
                d[2] = li3; d[2].TableName = "PayGateway";
                Service.srvWallet srv4 = new Service.srvWallet();
                DataTable li4 = srv4.Get_Wallets(obj);
                d[3] = li4; d[3].TableName = "Get_Wallets";
                DataTable li5 = (DataTable)CompanyInfo.getEmailPermission(obj.Client_ID, 0);
                d[4] = li5; d[4].TableName = "Get_Perm";

                DataTable li6 = srv3.GetinstantBankAPIDetails(obj);
                d[5] = li6; d[5].TableName = "Get_instantBankAPI";


                int Customer_ID = Convert.ToInt32(CompanyInfo.Decrypt(obj.Customer_ID, true));
                _ = Task.Run(() => CompanyInfo.InsertActivityLogDetails("App - Customer " + obj.UserName + " is on send money step 3. Fetched additional data and transaction summary. Customer ID: " + Customer_ID + "", obj.User_ID, obj.Transaction_ID, obj.User_ID, Convert.ToInt32(Customer_ID), "Send-Get Base Data", obj.CB_ID, obj.Client_ID, "Send Money Step 3", context));
                response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_SUCCESS);
                response.data = "success";
                response.ObjData = d.ToDictionary(
                    dt => dt?.TableName,  // Use TableName as the key
                    dt => dt?.AsEnumerable().Select(row => dt.Columns.Cast<DataColumn>()
                                                .ToDictionary(col => col.ColumnName,
                                                                col => row[col] == DBNull.Value ? "" : row[col].ToString())));

                response.ResponseCode = 0;
                returnJsonData = new { response = true, responseCode = "00", data = response.ObjData };
            }
            catch (Exception ex)
            {
                response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_TECHNICAL_ERROR);
                response.sErrorExceptionText = ex.ToString();
                response.ResponseMessage = "technical error";
                response.ResponseCode = 3;
                //Error Log Handled
                Model.ErrorLog objError = new Model.ErrorLog();
                objError.User = new Model.User();
                objError.Error = "Api : Rate : Get Additional Data --" + ex.ToString();
                objError.Date = DateTime.Now;
                objError.User_ID = 1;
                objError.Client_ID = 1;
                objError.Function_Name = "GetAdditionalData";
                Service.srvErrorLog srvError = new Service.srvErrorLog();
                srvError.Create(objError, context);

                returnJsonData = new { response = false, responseCode = "02", data = response.ResponseMessage };
            }
            return new JsonResult(returnJsonData);
        }

    }
}
