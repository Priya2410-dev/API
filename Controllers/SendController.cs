using Calyx_Solutions.Model;
using Calyx_Solutions.Service;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Data;
using System.Globalization;
using System.Web.Helpers;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Authorization;
using System.Text.RegularExpressions;
using System.Text.Json;
using static Google.Apis.Requests.BatchRequest;
using System.Web;
using Microsoft.Net.Http.Headers;
using System.Web.DynamicData;
using Microsoft.Ajax.Utilities;
using System.Linq;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Calyx_Solutions.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SendController : ControllerBase
    {
        [HttpPost]
        [Route("getvalues")]
        public IActionResult GetValues([FromBody] JsonElement Obj)
        {
            Model.response.WebResponse response = null;
            var returnJsonData = (dynamic)null;
            HttpContext context = null;
            string json = System.Text.Json.JsonSerializer.Serialize(Obj);
            dynamic data = JObject.Parse(json);

            Customer obj = JsonConvert.DeserializeObject<Customer>(json);
            _ = Task.Run(() => CompanyInfo.InsertrequestLogTracker("getvalues full request body: " + JObject.Parse(json), 0, 0, 0, 0, "GetValues", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext));
            try
            {
                Model.Customer _ObjCustomer = new Model.Customer();
                Service.srvCustomer srvCustomer = new Service.srvCustomer();
                _ObjCustomer.Comment = CompanyInfo.Decrypt(obj.Comment, true);

                returnJsonData = new { response = true, responseCode = "00", data = _ObjCustomer.Comment };
            }
            catch (Exception ex)
            {
                response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_TECHNICAL_ERROR);
                response.sErrorExceptionText = ex.ToString();
                response.ResponseMessage = "technical error";

                //Error Log Handled
                Model.ErrorLog objError = new Model.ErrorLog();
                objError.User = new Model.User();

                objError.Error = "Api : Getvalues --" + ex.ToString();
                objError.Date = DateTime.Now;
                objError.User_ID = 1;
                objError.Id = 1;
                objError.Function_Name = "Getvalues";
                Service.srvErrorLog srvError = new Service.srvErrorLog();
                srvError.Create(objError, context);

                returnJsonData = new { response = false, responseCode = "02", data = "Invalid Request" };
            }
            return new JsonResult(returnJsonData);
        }

        [HttpPost]
        [Route("info")]
        public IActionResult Info([FromBody] JsonObject Objinfo)
        {
            HttpContext context = HttpContext;
            Model.response.WebResponse response = null;
            string json = System.Text.Json.JsonSerializer.Serialize(Objinfo);
            dynamic data = JObject.Parse(json);
            CompanyInfo.InsertrequestLogTracker("info full request body: " + JObject.Parse(json), 0, 0, 0, 0, "send-Info", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
            var validateJsonData = (dynamic)null;
            Transaction obj = JsonConvert.DeserializeObject<Transaction>(json);
            try
            {
                if (!SqlInjectionProtector.ReadJsonObjectValues(Objinfo) || !SqlInjectionProtector.ReadJsonObjectValuesScript(Objinfo))
                {
                    var errorResponse = new { response = false, responseCode = "02", data = "Invalid input detected." };
                    return new JsonResult(errorResponse) { StatusCode = 400 };
                }

                Service.srvViewTransferHistory srv = new Service.srvViewTransferHistory();
                DataTable li1 = srv.ViewTransfer(obj);
                Model.Dashboard _Obj = new Model.Dashboard();
                if (li1 != null && li1.Rows.Count > 0)
                {
                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_SUCCESS);
                    response.data = "success";
                    response.ObjData = li1;
                    response.ResponseCode = 0;
                    var responseData = li1.Rows.OfType<DataRow>()
                    .Select(row => li1.Columns.OfType<DataColumn>()
                        .ToDictionary(col => col.ColumnName, c => row[c].ToString()));
                    validateJsonData = new { response = true, responseCode = "00", data = responseData };
                    return new JsonResult(validateJsonData);
                }
                else
                {
                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_TECHNICAL_ERROR);
                    response.ResponseMessage = "No Records Found.";
                    response.ResponseCode = 0;
                    validateJsonData = new { response = false, responseCode = "02", data = response.ResponseMessage };
                    return new JsonResult(validateJsonData);
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
                objError.Error = "Api : send-Info --" + ex.ToString();
                objError.Date = DateTime.Now;
                objError.User_ID = 1;
                objError.Client_ID = data.Client_ID;
                objError.Function_Name = "Info";
                Service.srvErrorLog srvError = new Service.srvErrorLog();
                srvError.Create(objError, context);

                validateJsonData = new { response = false, responseCode = "01", data = "Invalid Request" };
                return new JsonResult(validateJsonData);
            }

        }

            [HttpPost]
        [Route("saveaudit")]
        public IActionResult saveaudit([FromBody] JsonObject Obj)
        {
            Model.response.WebResponse response = null;
            string json = System.Text.Json.JsonSerializer.Serialize(Obj);
            dynamic data = JObject.Parse(json);
            CompanyInfo.InsertrequestLogTracker("saveaudit full request body: " + JObject.Parse(json), 0, 0, 0, 0, "saveaudit", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
            var validateJsonData = (dynamic)null;
            Transaction obj = JsonConvert.DeserializeObject<Transaction>(json);
            try
            {
                

                int number;
                if (!int.TryParse(Convert.ToString(data.clientID), out number) || data.clientID == "" || data.clientID == null)
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "Invalid Client ID." };
                    return new JsonResult(validateJsonData);
                }
                if (data.customerID == "" || data.customerID == null)
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "Invalid customer ID ." };
                    return new JsonResult(validateJsonData);
                }
                object o = JsonConvert.DeserializeObject(json);
                bool checkinjection = CompanyInfo.checksqlinjectiondata(o);
                if (!checkinjection)
                {
                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_TECHNICAL_ERROR);
                    response.ResponseMessage = "Invalid Field Values.";
                    response.ResponseCode = 6;
                    validateJsonData = new { response = false, responseCode = "06", data = response.ResponseMessage };
                    return new JsonResult(validateJsonData);
                }
                obj.Client_ID = data.clientID;
                obj.Customer_ID = data.customerID;
               
                int Customer_ID = Convert.ToInt32(CompanyInfo.Decrypt(obj.Customer_ID, true));
                CompanyInfo.InsertActivityLogDetails("App - " + obj.SourceComment + "", obj.User_ID, obj.Transaction_ID, obj.User_ID, Convert.ToInt32(Customer_ID), "Send-Audit", obj.CB_ID, obj.Client_ID, "Audit",HttpContext);
                Model.Dashboard _Obj = new Model.Dashboard();

                response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_SUCCESS);
                response.data = "success";
                response.ObjData = 0;
                response.ResponseCode = 0;

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
                objError.Error = "Api : Send- Save Audit --" + ex.ToString();
                objError.Date = DateTime.Now;
                objError.User_ID = 1;
                objError.Client_ID = obj.Client_ID;
                objError.Function_Name = "SaveAudit";
                Service.srvErrorLog srvError = new Service.srvErrorLog();
                srvError.Create(objError,HttpContext);
                validateJsonData = new { response = false, responseCode = "01", data = response.ResponseMessage };
                return new JsonResult(validateJsonData);
               
            }
            validateJsonData = new { response = true, responseCode = "00", data = "Transaction Audit saved successfully" };
            return new JsonResult(validateJsonData);
        }

        [HttpPost]
        [Route("saverequestresponse")]
        public IActionResult saverequestresponse([FromBody] JsonObject Obj)
        {
            Model.response.WebResponse response = null;
            string json = System.Text.Json.JsonSerializer.Serialize(Obj);
            dynamic data = JObject.Parse(json);
            CompanyInfo.InsertrequestLogTracker("saverequestresponse full request body: " + JObject.Parse(json), 0, 0, 0, 0, "saverequestresponse", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
            var validateJsonData = (dynamic)null;
            Transaction obj = JsonConvert.DeserializeObject<Transaction>(json);
            try {
                if (!SqlInjectionProtector.ReadJsonObjectValues(Obj) || !SqlInjectionProtector.ReadJsonObjectValuesScript(Obj))
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
                    validateJsonData = new { response = false, responseCode = "02", data = "Invalid branch ID." };
                    return new JsonResult(validateJsonData);
                }
                if (!int.TryParse(Convert.ToString(data.deleteStatus), out number) || data.deleteStatus == "" || data.deleteStatus == null)
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "Invalid Status Value." };
                    return new JsonResult(validateJsonData);
                }
                
                object o = JsonConvert.DeserializeObject(json);
                bool checkinjection = CompanyInfo.checksqlinjectiondata(o);
                if (!checkinjection)
                {
                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_TECHNICAL_ERROR);
                    response.ResponseMessage = "Invalid Field Values.";
                    response.ResponseCode = 6;
                    validateJsonData = new { response = false, responseCode = "06", data = response.ResponseMessage };
                    return new JsonResult(validateJsonData);
                }
                obj.Client_ID = data.clientID;
                obj.Customer_ID = data.customerID;
                obj.Branch_ID = data.branchID;
                obj.Delete_Status = data.deleteStatus;
                obj.referenceNumber = data.referenceNumber;
                obj.Worldpay_Response = data.responsepara;

                Service.srvSendMoney srv = new Service.srvSendMoney();
                int li1 = srv.SaveRequestResponse(obj);
                Model.Dashboard _Obj = new Model.Dashboard();
                if (li1 != null && li1 > 0)
                {
                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_SUCCESS);
                    response.data = "success";
                    response.ObjData = li1;
                    response.ResponseCode = 0;
                    validateJsonData = new { response = true, responseCode = "00", data = response.data };
                    return new JsonResult(validateJsonData);
                }
                else
                {
                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_TECHNICAL_ERROR);
                    response.ResponseMessage = "Something went wrong";
                    response.ResponseCode = 0;
                    validateJsonData = new { response = false, responseCode = "02", data = response.ResponseMessage };
                    return new JsonResult(validateJsonData);
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
                objError.Error = "Api : saverequestresponse Save Audit --" + ex.ToString();
                objError.Date = DateTime.Now;
                objError.User_ID = 1;
                objError.Client_ID = obj.Client_ID;
                objError.Function_Name = "saverequestresponse";
                Service.srvErrorLog srvError = new Service.srvErrorLog();
                srvError.Create(objError, HttpContext);
                validateJsonData = new { response = false, responseCode = "01", data = response.ResponseMessage };
                return new JsonResult(validateJsonData);
            }
        }


        [HttpPost]
        [Route("getecommpaytransactionstatus")]
        public IActionResult getEcommpayTransactionStatus([FromBody] JsonObject Obj)
        {
            HttpContext context =null; var validateJsonData = (dynamic)null;
            DataTable dt = new DataTable();
            dt.Columns.Add("Success", typeof(string));
            dt.Columns.Add("Client_ID", typeof(string));
            dt.Columns.Add("User_ID", typeof(string));
            dt.Columns.Add("CB_ID", typeof(string));
            dt.Columns.Add("transactionref", typeof(string));
            dt.Columns.Add("mainamount", typeof(string));
            dt.Columns.Add("currency", typeof(string));
            try
            {
                Model.response.WebResponse response = null;
                string json = System.Text.Json.JsonSerializer.Serialize(Obj);
                dynamic data = JObject.Parse(json);
                
                Transaction obj = JsonConvert.DeserializeObject<Transaction>(json);

                CompanyInfo.InsertrequestLogTracker("getecommpaytransactionstatus full request body: " + JObject.Parse(json), 0, 0, 0, 0, "getecommpaytransactionstatus", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
                if (!SqlInjectionProtector.ReadJsonObjectValues(Obj) || !SqlInjectionProtector.ReadJsonObjectValuesScript(Obj))
                {
                    var errorResponse = new { response = false, responseCode = "02", data = "Invalid input detected." };
                    return new JsonResult(errorResponse) { StatusCode = 400 };
                }

                obj.Client_ID = data.clientID;
                obj.ReferenceNo = data.referenceNumber;

                Service.srvSendMoney srv = new Service.srvSendMoney();
                DataTable li_ecommpay = srv.GetEcommpayPayStatus(obj);
                string transactionStatus = "";
                string mainamount = "", currency = "";
                if (li_ecommpay != null)
                {
                    if (li_ecommpay.Rows.Count > 0)
                    {
                        transactionStatus = Convert.ToString(li_ecommpay.Rows[0][1]);
                        try { mainamount = Convert.ToString(li_ecommpay.Rows[0][4]); } catch { }
                        try { currency = Convert.ToString(li_ecommpay.Rows[0][5]); } catch { }
                    }
                }

                if (transactionStatus == "success")
                    dt.Rows.Add(true, transactionStatus, "", "", "", mainamount, currency);
                else
                    dt.Rows.Add(false, transactionStatus, "", "", "", mainamount, currency);

                var relationshipData = dt.Rows.OfType<DataRow>()
                .Select(row => dt.Columns.OfType<DataColumn>()
                    .ToDictionary(col => col.ColumnName, c => row[c]));

                if (transactionStatus == "success")
                {
                    validateJsonData = new { response = true, responseCode = "00", data = relationshipData };
                    return new JsonResult(validateJsonData);
                }
                else
                {
                    validateJsonData = new { response = false, responseCode = "02", data = relationshipData };
                    return new JsonResult(validateJsonData);
                }

            }
            catch (Exception ex)
            {
                CompanyInfo.InsertActivityLogDetails(" Ecommpay getEcommpayTransactionStatus Error : " + ex.ToString(), 0, 0, 0, 0, "getEcommpayTransactionStatus", 0, 0, "Send Money", context);
                dt.Rows.Add(false, "", "", "", "", "", "");
            }
            var relationshipDatad = dt.Rows.OfType<DataRow>()
                .Select(row => dt.Columns.OfType<DataColumn>()
                    .ToDictionary(col => col.ColumnName, c => row[c]));
            validateJsonData = new { response = false, responseCode = "01", data = dt };
            return new JsonResult(relationshipDatad);
        }


        [HttpPost]
        [Route("getemerchanttransactionstatus")]
        public IActionResult getEmerchantTransactionStatus([FromBody] JsonObject Obj)
        {   //  Emerchant transaction
               var validateJsonData = (dynamic)null; HttpContext context=null;
            DataTable dt = new DataTable();
            dt.Columns.Add("Success", typeof(string));
            dt.Columns.Add("Client_ID", typeof(string));
            dt.Columns.Add("User_ID", typeof(string));
            dt.Columns.Add("CB_ID", typeof(string));
            dt.Columns.Add("transactionref", typeof(string));
            dt.Columns.Add("mainamount", typeof(string));
            dt.Columns.Add("currency", typeof(string));
            try
            {
                Model.response.WebResponse response = null;
                string json = System.Text.Json.JsonSerializer.Serialize(Obj);
                dynamic data = JObject.Parse(json);

                CompanyInfo.InsertrequestLogTracker("getemerchanttransactionstatus full request body: " + JObject.Parse(json), 0, 0, 0, 0, "getEmerchantTransactionStatus", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
                if (!SqlInjectionProtector.ReadJsonObjectValues(Obj) || !SqlInjectionProtector.ReadJsonObjectValuesScript(Obj))
                {
                    var errorResponse = new { response = false, responseCode = "02", data = "Invalid input detected." };
                    return new JsonResult(errorResponse) { StatusCode = 400 };
                }

                Transaction obj = JsonConvert.DeserializeObject<Transaction>(json);

                obj.Client_ID = data.clientID;
                obj.ReferenceNo = data.referenceNumber;

                // Get this information for customer pay through url
                string refNumber = obj.ReferenceNo;
                string allResponseURL = obj.Worldpay_Response;
                CompanyInfo.InsertActivityLogDetails(" App Side getEmerchantTransactionStatus : " + allResponseURL, 0, 0, 0, 0, "getEmerchantTransactionStatus", 0, 0, "Send Money", context);
                string transactionStatus = "", responsecode = "";

                string whereclause = " and aa.ReferenceNo = '" + refNumber + "' ";
                Service.srvSendMoney srv = new Service.srvSendMoney();

                MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("Get_bankingpartner_refnumber");
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.Parameters.AddWithValue("_ReferenceNo", refNumber.Trim());
                _cmd.Parameters.AddWithValue("_apiid", 7);
                DataTable dtorderDetails = db_connection.ExecuteQueryDataTableProcedure(_cmd);
                string unique_id = "";
                if (dtorderDetails.Rows.Count > 0)
                {
                    unique_id = Convert.ToString(dtorderDetails.Rows[0]["payinpartnernumber"]);
                }

                DataTable dtorderDetails3 = srv.GetEmerchantpayPaymentStatus(obj, unique_id,context);
                if (dtorderDetails3.Rows.Count > 0)
                {
                    responsecode = Convert.ToString(dtorderDetails3.Rows[0]["responsecode"]);
                    transactionStatus = Convert.ToString(dtorderDetails3.Rows[0]["status"]);
                }

                string mainamount = "", currency = "";
                DataTable dtorderDetails2 = srv.GetEmerchantpayOrderNumberDetails(obj, whereclause, refNumber, context);
                if (dtorderDetails2.Rows.Count > 0)
                {
                    mainamount = Convert.ToString(dtorderDetails2.Rows[0]["mainamount"]);
                    currency = Convert.ToString(dtorderDetails2.Rows[0]["currency"]);
                }

                if (transactionStatus == "approved" && responsecode == "00")
                    dt.Rows.Add(true, transactionStatus, "", "", "", mainamount, currency);
                else
                    dt.Rows.Add(false, transactionStatus, "", "", "", mainamount, currency);
                /*else if (transactionStatus == "pending")
                    dt.Rows.Add(false, transactionStatus, "", "", "", mainamount, currency);
                else if (transactionStatus == "decline")
                    dt.Rows.Add(false, transactionStatus, "", "", "", mainamount, currency);*/

                var relationshipData = dt.Rows.OfType<DataRow>()
                .Select(row => dt.Columns.OfType<DataColumn>()
                    .ToDictionary(col => col.ColumnName, c => row[c]));

                if (transactionStatus == "approved" && responsecode == "00")
                {
                    validateJsonData = new { response = true, responseCode = "00", data = relationshipData };
                    return new JsonResult(validateJsonData);
                }
                else
                {
                    validateJsonData = new { response = false, responseCode = "02", data = relationshipData };
                    return new JsonResult(validateJsonData);
                }

            }
            catch (Exception ex)
            {
                CompanyInfo.InsertActivityLogDetails(" App side getEmerchantTransactionStatus Error : " + ex.ToString(), 0, 0, 0, 0, "getEmerchantTransactionStatus", 0, 0, "Send Money", context);
                dt.Rows.Add(false, "", "", "", "", "", "");
            }
            var relationshipDatad = dt.Rows.OfType<DataRow>()
                 .Select(row => dt.Columns.OfType<DataColumn>()
                     .ToDictionary(col => col.ColumnName, c => row[c]));
            validateJsonData = new { response = false, responseCode = "01", data = dt };
            return new JsonResult(relationshipDatad);
        }

        [HttpPost]
        [Route("getordernumberdetails")]
        public IActionResult getordernumberdetails([FromBody] JsonObject Obj)
        {
            var validateJsonData = (dynamic)null; HttpContext context = null;

            DataTable dt = new DataTable();
            dt.Columns.Add("Success", typeof(string));
            dt.Columns.Add("Client_ID", typeof(string));
            dt.Columns.Add("User_ID", typeof(string));
            dt.Columns.Add("CB_ID", typeof(string));
            dt.Columns.Add("transactionref", typeof(string));
            dt.Columns.Add("mainamount", typeof(string));
            dt.Columns.Add("currency", typeof(string));
            try
            {
                Model.response.WebResponse response = null;
                string json = System.Text.Json.JsonSerializer.Serialize(Obj);
                dynamic data = JObject.Parse(json);

                CompanyInfo.InsertrequestLogTracker("getordernumberdetails full request body: " + JObject.Parse(json), 0, 0, 0, 0, "getordernumberdetails", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
                if (!SqlInjectionProtector.ReadJsonObjectValues(Obj) || !SqlInjectionProtector.ReadJsonObjectValuesScript(Obj))
                {
                    var errorResponse = new { response = false, responseCode = "02", data = "Invalid input detected." };
                    return new JsonResult(errorResponse) { StatusCode = 400 };
                }

                Transaction obj = JsonConvert.DeserializeObject<Transaction>(json);

                obj.GuavapayRefNo = data.GuavapayRefNo;

                int shft = 5; string status = "", order_status = "";
                string encrypted = obj.GuavapayRefNo;
                string decrypted = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(encrypted)).Select(ch => ((int)ch) >> shft).Aggregate("", (current, val) => current + (char)(val / 2));
                string whereclause = " and aa.ReferenceNo = '" + decrypted + "' ";
                Service.srvSendMoney srv = new Service.srvSendMoney();
                DataTable dtorderDetails = srv.GetGuavaPayOrderNumberDetails(obj, whereclause, decrypted, context);

                /*MySqlCommand _cmd = new MySqlCommand("View_transfers");
                _cmd.CommandType = CommandType.StoredProcedure;                
                _cmd.Parameters.AddWithValue("_whereclause", whereclause);
                DataTable dtorderDetails = db_connection.ExecuteQueryDataTableProcedure(_cmd);*/
                string CB_ID = "", User_ID = "", Client_ID = "", mainamount = "", currency = "";
                if (dtorderDetails.Rows.Count > 0)
                {
                    CB_ID = Convert.ToString(dtorderDetails.Rows[0]["CB_ID"]);
                    User_ID = Convert.ToString(dtorderDetails.Rows[0]["User_ID"]);
                    Client_ID = Convert.ToString(dtorderDetails.Rows[0]["Client_ID"]);
                    decrypted = Convert.ToString(decrypted);
                    mainamount = Convert.ToString(dtorderDetails.Rows[0]["mainamount"]); //Convert.ToString(Convert.ToDouble(dtorderDetails.Rows[0]["AmountInGBP"]) + Convert.ToDouble(dtorderDetails.Rows[0]["Transfer_Fees"]));
                    currency = Convert.ToString(dtorderDetails.Rows[0]["currency"]);
                    dt.Rows.Add(true, Client_ID, User_ID, CB_ID, decrypted, mainamount, currency);

                    var relationshipData  = dt.Rows.OfType<DataRow>()
               .Select(row => dt.Columns.OfType<DataColumn>()
                   .ToDictionary(col => col.ColumnName, c => row[c]));
                    validateJsonData = new { response = true, responseCode = "00", data = dt };
                    return new JsonResult(relationshipData);
                }
            }
            catch (Exception ex)
            {
                CompanyInfo.InsertActivityLogDetails(" Guavapay getOrderNumberDetails Error : " + ex.ToString(), 0, 0, 0, 0, "getOrderNumberDetails", 0, 0, "Send Money", context);
                dt.Rows.Add(false, "", "", "", "", "", "");
            }
            var relationshipDatad = dt.Rows.OfType<DataRow>()
                .Select(row => dt.Columns.OfType<DataColumn>()
                    .ToDictionary(col => col.ColumnName, c => row[c]));
            validateJsonData = new { response = false, responseCode = "01", data = dt };
            return new JsonResult(relationshipDatad);
        }

        [HttpPost]
        [Route("getordernumber")]
        public IActionResult getordernumber([FromBody] JsonObject Obj)
        {
            var validateJsonData = (dynamic)null; HttpContext context = null;
            DataTable dt = new DataTable();
            dt.Columns.Add("Success", typeof(string));
            dt.Columns.Add("Status", typeof(string));
            dt.Columns.Add("order_status", typeof(string));
            dt.Columns.Add("transactionref", typeof(string));
            try
            {
                Model.response.WebResponse response = null;
                string json = System.Text.Json.JsonSerializer.Serialize(Obj);
                dynamic data = JObject.Parse(json);

                CompanyInfo.InsertrequestLogTracker("getordernumber full request body: " + JObject.Parse(json), 0, 0, 0, 0, "getordernumber", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
                if (!SqlInjectionProtector.ReadJsonObjectValues(Obj) || !SqlInjectionProtector.ReadJsonObjectValuesScript(Obj))
                {
                    var errorResponse = new { response = false, responseCode = "02", data = "Invalid input detected." };
                    return new JsonResult(errorResponse) { StatusCode = 400 };
                }

                Transaction obj = JsonConvert.DeserializeObject<Transaction>(json);
                obj.GuavapayRefNo = data.GuavapayRefNo;
                obj.Client_ID = data.clientID;
                int shft = 5; string status = "", order_status = "";
                string encrypted = obj.GuavapayRefNo;
                string decrypted = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(encrypted)).Select(ch => ((int)ch) >> shft).Aggregate("", (current, val) => current + (char)(val / 2));

                Service.srvSendMoney srv = new Service.srvSendMoney();
                DataTable li_guavapay = srv.GetGuavaPayStatus(obj, context);
                bool Success = false;
                if (li_guavapay != null)
                {
                    if (li_guavapay.Rows.Count > 0)
                    {
                        Success = Convert.ToBoolean(li_guavapay.Rows[0][0]);
                        status = Convert.ToString(li_guavapay.Rows[0][1]);
                        order_status = Convert.ToString(li_guavapay.Rows[0][2]);
                    }
                }
                dt.Rows.Add(Success, status, order_status, decrypted);
                var relationshipData = dt.Rows.OfType<DataRow>()
               .Select(row => dt.Columns.OfType<DataColumn>()
                   .ToDictionary(col => col.ColumnName, c => row[c]));

                if(Success)
                validateJsonData = new { response = true, responseCode = "00", data = dt };
                else
                validateJsonData = new { response = false, responseCode = "02", data = dt };

                return new JsonResult(relationshipData);
            }
            catch (Exception ex)
            {
                bool Success = false;
                dt.Rows.Add(Success, "", "", "");
            }
            var relationshipDatad = dt.Rows.OfType<DataRow>()
                .Select(row => dt.Columns.OfType<DataColumn>()
                    .ToDictionary(col => col.ColumnName, c => row[c]));
            validateJsonData = new { response = false, responseCode = "01", data = dt };
            return new JsonResult(relationshipDatad);
        }


        [HttpPost]
        [Route("getpaycrossstatus")]
        public IActionResult getpaycrossstatus([FromBody] JsonObject Obj)
        {
            var validateJsonData = (dynamic)null; HttpContext context = null;
            DataTable dt = new DataTable();
            dt.Columns.Add("Success", typeof(string));
            dt.Columns.Add("Client_ID", typeof(string));
            dt.Columns.Add("User_ID", typeof(string));
            dt.Columns.Add("CB_ID", typeof(string));
            dt.Columns.Add("transactionref", typeof(string));
            dt.Columns.Add("mainamount", typeof(string));
            dt.Columns.Add("currency", typeof(string));
            try
            { 
                // Get this information for customer pay through url
                Model.response.WebResponse response = null;
                string json = System.Text.Json.JsonSerializer.Serialize(Obj);
                dynamic data = JObject.Parse(json);

                CompanyInfo.InsertrequestLogTracker("getpaycrossstatus full request body: " + JObject.Parse(json), 0, 0, 0, 0, "getpaycrossstatus", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
                if (!SqlInjectionProtector.ReadJsonObjectValues(Obj) || !SqlInjectionProtector.ReadJsonObjectValuesScript(Obj))
                {
                    var errorResponse = new { response = false, responseCode = "02", data = "Invalid input detected." };
                    return new JsonResult(errorResponse) { StatusCode = 400 };
                }

                Transaction obj = JsonConvert.DeserializeObject<Transaction>(json);

                obj.ReferenceNo = data.ReferenceNo;
                obj.Worldpay_Response = data.Worldpay_Response;
                obj.Client_ID = data.Client_ID;
                obj.GuavapayorderId = data.GuavapayorderId;

                string refNumber = obj.ReferenceNo;
                string allResponseURL = obj.Worldpay_Response;
                CompanyInfo.InsertActivityLogDetails(" App Side getpaycrossstatus : " + allResponseURL, 0, 0, 0, 0, "getpaycrossstatus", 0, 0, "Send Money", context);
                string transactionStatus = "", responsecode = "";

                string whereclause = " and aa.ReferenceNo = '" + refNumber + "' ";
                Service.srvSendMoney srv = new Service.srvSendMoney();

                MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("Get_bankingpartner_refnumber");
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.Parameters.AddWithValue("_ReferenceNo", refNumber.Trim());
                _cmd.Parameters.AddWithValue("_apiid", 8);
                DataTable dtorderDetails = db_connection.ExecuteQueryDataTableProcedure(_cmd);
                string unique_id = "";
                unique_id = obj.GuavapayorderId;
                if (dtorderDetails.Rows.Count > 0)
                {
                    unique_id = Convert.ToString(dtorderDetails.Rows[0]["payinpartnernumber"]);
                }

                DataTable dtorderDetails3 = srv.GetPaycrossPaymentStatus(obj, unique_id, context);
                if (dtorderDetails3.Rows.Count > 0)
                {
                    responsecode = Convert.ToString(dtorderDetails3.Rows[0]["responsecode"]);
                    transactionStatus = Convert.ToString(dtorderDetails3.Rows[0]["status"]);
                }


                string mainamount = "", currency = "";
                DataTable dtorderDetails2 = srv.GetEmerchantpayOrderNumberDetails(obj, whereclause, refNumber, context);
                if (dtorderDetails2.Rows.Count > 0)
                {
                    mainamount = Convert.ToString(dtorderDetails2.Rows[0]["mainamount"]);
                    currency = Convert.ToString(dtorderDetails2.Rows[0]["currency"]);
                }

                if (transactionStatus == "successful" && responsecode == "0")
                    dt.Rows.Add(true, transactionStatus, "", "", "", mainamount, currency);
                else
                    dt.Rows.Add(false, transactionStatus, "", "", "", mainamount, currency);

                var relationshipData = dt.Rows.OfType<DataRow>()
               .Select(row => dt.Columns.OfType<DataColumn>()
                   .ToDictionary(col => col.ColumnName, c => row[c]));

                if (transactionStatus == "successful" && responsecode == "0")
                    validateJsonData = new { response = true, responseCode = "00", data = dt };
                else
                    validateJsonData = new { response = false, responseCode = "02", data = dt };

                return new JsonResult(relationshipData);

            }
            catch (Exception ex)
            {
                CompanyInfo.InsertActivityLogDetails(" App side getpaycrossstatus Error : " + ex.ToString(), 0, 0, 0, 0, "getpaycrossstatus", 0, 0, "Send Money",context);
                dt.Rows.Add(false, "", "", "", "", "", "");
            }
            var relationshipDatad = dt.Rows.OfType<DataRow>()
                .Select(row => dt.Columns.OfType<DataColumn>()
                    .ToDictionary(col => col.ColumnName, c => row[c]));
            validateJsonData = new { response = false, responseCode = "01", data = dt };
            return new JsonResult(relationshipDatad);
        }


            [HttpPost]
        [Route("getnuvietransactionstatus")]
        public IActionResult getnuvietransactionstatus([FromBody] JsonObject Obj)
        {
            // Nuvie transaction
            var validateJsonData = (dynamic)null; HttpContext context = null;
            DataTable dt = new DataTable();
            dt.Columns.Add("Success", typeof(string));
            dt.Columns.Add("Client_ID", typeof(string));
            dt.Columns.Add("User_ID", typeof(string));
            dt.Columns.Add("CB_ID", typeof(string));
            dt.Columns.Add("transactionref", typeof(string));
            dt.Columns.Add("mainamount", typeof(string));
            dt.Columns.Add("currency", typeof(string));
            try
            {
                Model.response.WebResponse response = null;
                string json = System.Text.Json.JsonSerializer.Serialize(Obj);
                dynamic data = JObject.Parse(json);

                CompanyInfo.InsertrequestLogTracker("getnuvietransactionstatus full request body: " + JObject.Parse(json), 0, 0, 0, 0, "getnuvietransactionstatus", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
                if (!SqlInjectionProtector.ReadJsonObjectValues(Obj) || !SqlInjectionProtector.ReadJsonObjectValuesScript(Obj))
                {
                    var errorResponse = new { response = false, responseCode = "02", data = "Invalid input detected." };
                    return new JsonResult(errorResponse) { StatusCode = 400 };
                }

                Transaction obj = JsonConvert.DeserializeObject<Transaction>(json);

                obj.Client_ID = data.clientID;
                obj.ReferenceNo = data.referenceNumber;

                // Get this information for customer pay through url
                string refNumber = obj.ReferenceNo;
                string allResponseURL = obj.Worldpay_Response;
                CompanyInfo.InsertActivityLogDetails(" App Side getNuvieTransactionStatus Error : " + allResponseURL, 0, 0, 0, 0, "getNuvieTransactionStatus", 0, 0, "Send Money", context);
                string transactionStatus = "";
                Uri nuvieUri = new Uri(allResponseURL);
                string nuveiResponse = HttpUtility.ParseQueryString(nuvieUri.Query).Get("Status");

                if (nuveiResponse == "APPROVED")
                {
                    transactionStatus = "success";
                }
                else if (nuveiResponse == "decline")
                {
                    transactionStatus = "DECLINED";
                }
                else if (nuveiResponse == "PENDING")
                {
                    transactionStatus = "pending";
                }
                else if (nuveiResponse == "UPDATE")
                {
                    transactionStatus = "pending";
                }

                string whereclause = " and aa.ReferenceNo = '" + refNumber + "' ";
                Service.srvSendMoney srv = new Service.srvSendMoney();
                DataTable dtorderDetails = srv.GetECommpayOrderNumberDetails(obj, whereclause, refNumber);
                string mainamount = "", currency = "";
                if (dtorderDetails.Rows.Count > 0)
                {
                    mainamount = Convert.ToString(dtorderDetails.Rows[0]["mainamount"]);
                    currency = Convert.ToString(dtorderDetails.Rows[0]["currency"]);
                }

                var relationshipData = dt.Rows.OfType<DataRow>()
                .Select(row => dt.Columns.OfType<DataColumn>()
                    .ToDictionary(col => col.ColumnName, c => row[c]));

                if (transactionStatus == "success")
                    dt.Rows.Add(true, transactionStatus, "", "", "", mainamount, currency);
                else if (transactionStatus == "pending")
                    dt.Rows.Add(false, transactionStatus, "", "", "", mainamount, currency);
                else if (transactionStatus == "decline")
                    dt.Rows.Add(false, transactionStatus, "", "", "", mainamount, currency);


                if (transactionStatus == "success")
                {
                    validateJsonData = new { response = true, responseCode = "00", data = relationshipData };
                    return new JsonResult(validateJsonData);
                }
                else if (transactionStatus == "pending")
                {
                    validateJsonData = new { response = false, responseCode = "02", data = relationshipData };
                    return new JsonResult(validateJsonData);
                }
                else if (transactionStatus == "decline")
                {
                    validateJsonData = new { response = false, responseCode = "02", data = relationshipData };
                    return new JsonResult(validateJsonData);
                }
                else
                {
                    validateJsonData = new { response = false, responseCode = "02", data = relationshipData };
                    return new JsonResult(validateJsonData);
                }
            }
            catch (Exception ex)
            {
                CompanyInfo.InsertActivityLogDetails(" App side getNuvieTransactionStatus Error : " + ex.ToString(), 0, 0, 0, 0, "getNuvieTransactionStatus", 0, 0, "Send Money", context);
                dt.Rows.Add(false, "", "", "", "", "", "");
            }
            var relationshipDatad = dt.Rows.OfType<DataRow>()
                 .Select(row => dt.Columns.OfType<DataColumn>()
                     .ToDictionary(col => col.ColumnName, c => row[c]));
            validateJsonData = new { response = false, responseCode = "01", data = dt };
            return new JsonResult(relationshipDatad);
        }

        [HttpPost]
        [Route("savetransactionaudit")]
        public JsonResult savetransactionaudit([FromBody] JsonElement objdata)
        {
            HttpContext context = HttpContext;
            List<Model.Transaction> _lst = new List<Model.Transaction>();
            var returnJsonData = (dynamic)null;
            Model.response.WebResponse response = null;
            string jsonData = System.Text.Json.JsonSerializer.Serialize(objdata);

            dynamic data = JObject.Parse(jsonData);
            try
            {
                CompanyInfo.InsertrequestLogTracker("savetransactionaudit full request body: " + JObject.Parse(jsonData), 0, 0, 0, 0, "savetransactionaudit", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
                if (!SqlInjectionProtector.ReadJsonElementValues(objdata) || !SqlInjectionProtector.ReadJsonElementValuesScript(objdata))
                {
                    var errorResponse = new { response = false, responseCode = "02", data = "Invalid input detected." };
                    return new JsonResult(errorResponse) { StatusCode = 400 };
                }

                #region validateData
                try
                {
                     if (data.branchID == "" || data.branchID == null)
                    {
                        returnJsonData = new { response = false, responseCode = "02", data = "Invalid Branch ID." };
                        return new JsonResult(returnJsonData);
                    }
                    else if (data.clientID == "" || data.clientID == null)
                    {
                        returnJsonData = new { response = false, responseCode = "02", data = "Invalid Client ID." };
                        return new JsonResult(returnJsonData);
                    }
                    else if (data.customerID == "" || data.customerID == null)
                    {
                        returnJsonData = new { response = false, responseCode = "02", data = "Invalid Customer ID." };
                        return new JsonResult(returnJsonData);
                    }
                    else if (data.transactionID == "" || data.transactionID == null)
                    {
                        returnJsonData = new { response = false, responseCode = "02", data = "Invalid Transaction ID." };
                        return new JsonResult(returnJsonData);
                    }
                }
                catch (Exception ex)
                {
                    Model.ActivityLogTracker _objActivityLog = new Model.ActivityLogTracker();
                    _objActivityLog.Activity = " Exception Found savetransactionaudit Error: " + ex.ToString() + " ";
                    _objActivityLog.FunctionName = "savetransactionaudit";
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

                try {
                    Transaction obj = JsonConvert.DeserializeObject<Transaction>(jsonData);
                    obj.Client_ID = data.clientID;
                    obj.Customer_ID = data.customerID;
                    obj.SourceComment = data.sourceComment;
                    obj.Transaction_ID = data.transactionID;
                    obj.CB_ID = data.branchID;

                    int Customer_ID = Convert.ToInt32(CompanyInfo.Decrypt(obj.Customer_ID, true));
                    CompanyInfo.InsertActivityLogDetails("App - " + obj.SourceComment + "", obj.User_ID, obj.Transaction_ID, obj.User_ID, Convert.ToInt32(Customer_ID), "Send-Audit", obj.CB_ID, obj.Client_ID, "Audit", context);
                    Model.Dashboard _Obj = new Model.Dashboard();

                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_SUCCESS);
                    response.data = "success";
                    response.ObjData = 0;
                    response.ResponseCode = 0;

                    returnJsonData = new { response = false, responseCode = "00", data = response.data };
                    return new JsonResult(returnJsonData);
                }
                catch (Exception ex)
                {
                    Model.ErrorLog objError = new Model.ErrorLog();
                    objError.User = new Model.User();
                    objError.Branch = new Model.Branch();
                    objError.Client = new Model.Client();

                    objError.Error = "Api error: savetransactionaudit --" + ex.ToString();
                    objError.Branch_ID = 1;
                    objError.Date = DateTime.Now;
                    objError.User_ID = 1;
                    objError.Id = 1;
                    objError.Function_Name = "savetransactionaudit";
                    Service.srvErrorLog srvError = new Service.srvErrorLog();
                    srvError.Create(objError, HttpContext);
                    returnJsonData = new { response = false, responseCode = "01", data = "Invalid request." };
                    return new JsonResult(returnJsonData);
                }
             
            }
            catch (Exception ex)
            {
                Model.ErrorLog objError = new Model.ErrorLog();
                objError.User = new Model.User();
                objError.Branch = new Model.Branch();
                objError.Client = new Model.Client();

                objError.Error = "Api final: savetransactionaudit --" + ex.ToString();
                objError.Branch_ID = 1;
                objError.Date = DateTime.Now;
                objError.User_ID = 1;
                objError.Id = 1;
                objError.Function_Name = "savetransactionaudit";
                Service.srvErrorLog srvError = new Service.srvErrorLog();
                srvError.Create(objError, HttpContext);
                returnJsonData = new { response = false, responseCode = "01", data = "Invalid request." };
                return new JsonResult(returnJsonData);
            }
        }

        
        [HttpPost]        
        [Route("payvynepaymentstatus")]
        public JsonResult payvynepaymentstatus([FromBody] JsonElement objdata)
        {
            HttpContext context = HttpContext;
            List<Model.Transaction> _lst = new List<Model.Transaction>();
            var returnJsonData = (dynamic)null;
            Model.response.WebResponse response = null;
            string jsonData = System.Text.Json.JsonSerializer.Serialize(objdata);
            dynamic data = JObject.Parse(jsonData);

            try
            {
                CompanyInfo.InsertrequestLogTracker("payvynepaymentstatus full request body: " + JObject.Parse(jsonData), 0, 0, 0, 0, "payvynepaymentstatus", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
                if (!SqlInjectionProtector.ReadJsonElementValues(objdata) || !SqlInjectionProtector.ReadJsonElementValuesScript(objdata))
                {
                    var errorResponse = new { response = false, responseCode = "02", data = "Invalid input detected." };
                    return new JsonResult(errorResponse) { StatusCode = 400 };
                }


                Transaction obj = JsonConvert.DeserializeObject<Transaction>(jsonData);
                obj.Client_ID = data.clientID;
                obj.Customer_ID = data.customerID;                
                obj.CB_ID = data.branchID;
                obj.payvyne_trans_id = data.payvynetransid;
                obj.bankGateway = data.bankGateway;


                Service.srvSendMoney srv = new Service.srvSendMoney();
                DataTable li1 = srv.PayvynePaymentStatus(obj);
                Model.Dashboard _Obj = new Model.Dashboard();
                if (li1 != null && li1.Rows.Count > 0)
                {
                    var relationshipData = li1.Rows.OfType<DataRow>()
                .Select(row => li1.Columns.OfType<DataColumn>()
                    .ToDictionary(col => col.ColumnName, c => row[c]));

                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_SUCCESS);
                    response.data = "success";
                    response.ObjData = li1;
                    response.ResponseCode = 0;
                    returnJsonData = new { response = true, responseCode = "00", data = relationshipData };
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
                Model.ErrorLog objError = new Model.ErrorLog();
                objError.User = new Model.User();
                objError.Branch = new Model.Branch();
                objError.Client = new Model.Client();

                objError.Error = "Api : payvynepaymentstatus --" + ex.ToString();
                objError.Branch_ID = 1;
                objError.Date = DateTime.Now;
                objError.User_ID = 1;
                objError.Id = 1;
                objError.Function_Name = "payvynepaymentstatus";
                Service.srvErrorLog srvError = new Service.srvErrorLog();
                srvError.Create(objError, HttpContext);
                returnJsonData = new { response = false, responseCode = "01", data = "Invalid request." };
                return new JsonResult(returnJsonData);
            }
            return new JsonResult(returnJsonData);

        }


            [HttpPost]
        [Authorize]
        [Route("inserttransaction")]
        public JsonResult InsertTransaction([FromBody] JsonElement objdata)
        {
            HttpContext context = HttpContext;
            List<Model.Transaction> _lst = new List<Model.Transaction>();
            var returnJsonData = (dynamic)null;
            Model.response.WebResponse response = null;
            string jsonData = System.Text.Json.JsonSerializer.Serialize(objdata);
            dynamic data = JObject.Parse(jsonData);
            CompanyInfo.InsertrequestLogTracker("InsertTransaction full request body: " + JObject.Parse(jsonData), 0, 0, 0, 0, "inserttransaction", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
            try
            {
                if (!SqlInjectionProtector.ReadJsonElementValues(objdata) || !SqlInjectionProtector.ReadJsonElementValuesScript(objdata))
                {
                    var errorResponse = new { response = false, responseCode = "02", data = "Invalid input detected." };
                    return new JsonResult(errorResponse) { StatusCode = 400 };
                }

                Transaction obj = new Transaction();
                obj.tokenValue = Request.Headers[HeaderNames.Authorization];
                obj.APIBranch_Details = data.apiBranchDetails;
                //obj.ActualFXRate = data.actualFXRate;
                obj.AmountInGBP = data.sendingAmount;
                /*if(obj.AmountInGBP <= 0)
                {
                    var errorResponse = new { response = false, responseCode = "02", data = "Invalid Amount." };
                    return new JsonResult(errorResponse) { StatusCode = 400 };
                }*/

                string receivingAmt = Convert.ToString(data.receivingAmount).Trim();
                obj.AmountInPKR = Convert.ToDouble(receivingAmt);  // data.receivingAmount  ;

                obj.auth_code = data.authCode;
                obj.Beneficiary_ID = data.beneficiaryID;
                obj.BranchListAPI_ID = data.branchListAPIID;
                obj.CB_ID = data.branchID;
                obj.Client_ID = data.clientID;
                obj.CollectionPoint_ID = data.collectionPointID;
                obj.Comment = data.comment;
                obj.Country_ID = data.countryID;
                obj.Currency_Code = data.currencyCode;
                obj.Customer_ID = data.customerID;
                obj.Customer_Reference = data.customerReference;
                obj.DeliveryType_Id = data.deliveryTypeID;
                obj.DiscountType = data.discountType;
                obj.Discount_Amount = data.discountAmount;
                obj.Discount_Code = data.discountCode;
                obj.Discount_ID = data.discountID;
                obj.Discount_Perm = data.discountPerm;
                obj.Exchange_Rate = data.exchangeRate;
                obj.FromCurrency_Code = data.fromCurrencyCode;
                
                obj.HDelivery_Address = data.homeDeliveryAddress;
                obj.HDelivery_Flag = data.homeDeliveryFlag;               

                obj.PaymentDepositType_ID = data.paymentDepositeTypeID;
                obj.PaymentType = data.paymentType;
                obj.PaymentType_ID = data.paymentTypeID;
                obj.Purpose = data.purpose;
                obj.Purpose_ID = data.purposeID;
                obj.RateUpdateReason_ID = data.rateUpdateReasonId;
                obj.SOFID = data.sofID;
                obj.SourceComment = data.sourceComment;
                obj.SourceComment_Flag = data.sourceCommentFlag;
                obj.Transaction_From_Flag = data.transactionFromFlag;
                obj.TransferType = data.transferType;
                obj.Transfer_Cost = data.transferCost;
                obj.User_ID = data.userID;
                obj.Wallet_Amount = data.walletAmount;
                obj.Wallet_Currency = data.walletCurrency;
                obj.Wallet_ID = data.walletID;
                obj.Wallet_Perm = data.walletPerm;
                obj.sanction_responce_bene_aml = data.sanctionBeneficaryAml;
                obj.sanction_responce_bene_kyc = data.sanctionBeneficaryKYC;
                obj.sanction_responce_cust_aml = data.sanctionCustomerAml;
                obj.sanction_responce_cust_kyc = data.sanctionResponseCustomerKYC;
                //obj.transfer_count_days = data.transferCountDays;

                // This field added for security
                try
                {
                    obj.new_agent_id = data.newAgentID;
                }
                catch (Exception eg) { obj.new_agent_id = ""; }
                try
                {
                    obj.new_agent_branch = data.newAgentBranch;
                }
                catch (Exception eg) { obj.new_agent_branch = 0; }
                // This field added for security

                try
                {
                    obj.Transfer_Fees = data.transferFees;
                }
                catch (Exception eg) { obj.Transfer_Fees = 0; }

                try
                {
                    obj.improved_rate_flag = data.improved_rate_flag;

                    if (obj.improved_rate_flag == 1)
                    {
                        obj.improved_rate_used_flag = 0;// Improve Rates used
                    }
                    else
                    {
                        obj.improved_rate_used_flag = 1;// Improve Rates not used
                    }
                }
                catch(Exception eg) { 
                    obj.improved_rate_flag = 0;
                    obj.improved_rate_used_flag = 1;// Improve Rates not used
                }

                try
                {
                    obj.ExtraTransfer_Fees = data.ExtraTransfer_Fees;
                }
                catch (Exception eg) { obj.ExtraTransfer_Fees = 0; }
                try
                {
                    obj.offer_rate_flag = data.offer_rate_flag;
                }
                catch (Exception eg) { obj.offer_rate_flag = 1; }
                try
                {
                    obj.Benf_BankDetails_ID = data.Benf_BankDetails_ID;
                }
                catch (Exception eg) { obj.Benf_BankDetails_ID = 0; }

                int testingpurpose = 1;
                try
                {
                     testingpurpose = data.testingpurpose;
                }
                catch (Exception eg) { testingpurpose =  1; }


                Service.srvSendMoney srv = new Service.srvSendMoney();





                DataTable dt = null;
                /*if (testingpurpose == 1)
                 dt = srv.InsertTransaction(obj, context);
                else*/
                dt = srv.InsertTransaction_new(obj, context);


                if (dt != null && dt.Rows.Count > 0)
                {
                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_SUCCESS);
                    response.data = "success";
                    response.ObjData = dt;
                    response.ResponseCode = 0;
                    List<Dictionary<string, object>> Transaction = new List<Dictionary<string, object>>();

                    foreach (DataRow dr in dt.Rows)
                    {
                        Dictionary<string, object> wallet_balnce = new Dictionary<string, object>();
                        wallet_balnce["status"] = dr["Status"];
                        wallet_balnce["transactionReferenceNumber"] = dr["Refno"];
                        wallet_balnce["customerReferenceNumber"] = dr["CustRefno"];
                        wallet_balnce["transactionID"] = dr["Transaction_ID"];
                        wallet_balnce["gccPinNumber"] = dr["gcc_pinnumber"];
                        Transaction.Add(wallet_balnce);
                    }


                    returnJsonData = new { response = true, responseCode = "00", data = Transaction };
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
                Model.ErrorLog objError = new Model.ErrorLog();
                objError.User = new Model.User();
                objError.Branch = new Model.Branch();
                objError.Client = new Model.Client();

                objError.Error = "Api : InsertTransaction --" + ex.ToString();
                objError.Branch_ID = 1;
                objError.Date = DateTime.Now;
                objError.User_ID = 1;
                objError.Id = 1;
                objError.Function_Name = "InsertTransaction";
                Service.srvErrorLog srvError = new Service.srvErrorLog();
                srvError.Create(objError, HttpContext);
                returnJsonData = new { response = false, responseCode = "01", data = "Invalid request." };
                return new JsonResult(returnJsonData);
            }
            return new JsonResult(returnJsonData);
        }



        [HttpPost]
        [Authorize]
        [Route("inserttemptransaction")]
        public JsonResult InsertTemp([FromBody] JsonElement objdata)
        {
            HttpContext context = HttpContext;
            List<Model.Transaction> _lst = new List<Model.Transaction>();
            var returnJsonData = (dynamic)null;
            Model.response.WebResponse response = null;
            string jsonData = System.Text.Json.JsonSerializer.Serialize(objdata);
            dynamic data = JObject.Parse(jsonData);
            CompanyInfo.InsertrequestLogTracker("InsertTemp full request body: " + JObject.Parse(jsonData), 0, 0, 0, 0, "inserttemptransaction", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
            try
            {
                if (!SqlInjectionProtector.ReadJsonElementValues(objdata) || !SqlInjectionProtector.ReadJsonElementValuesScript(objdata))
                {
                    var errorResponse = new { response = false, responseCode = "02", data = "Invalid input detected." };
                    return new JsonResult(errorResponse) { StatusCode = 400 };
                }

                Transaction obj = new Transaction();
                obj.APIBranch_Details = data.apiBranchDetails;
                //obj.ActualFXRate = data.actualFXRate;
                obj.AmountInGBP = data.sendingAmount;
                string receivingAmt = Convert.ToString(data.receivingAmount).Trim();
                obj.AmountInPKR = Convert.ToDouble(receivingAmt);  // data.receivingAmount  ;

                obj.auth_code = data.authCode;
                obj.Beneficiary_ID = data.beneficiaryID;
                obj.BranchListAPI_ID = data.branchListAPIID;
                obj.CB_ID = data.branchID;
                obj.Client_ID = data.clientID;
                obj.CollectionPoint_ID = data.collectionPointID;
                obj.Comment = data.comment;
                obj.Country_ID = data.countryID;
                obj.Currency_Code = data.currencyCode;
                obj.Customer_ID = data.customerID;
                obj.Customer_Reference = data.customerReference;
                obj.DeliveryType_Id = data.deliveryTypeID;
                obj.DiscountType = data.discountType;
                obj.Discount_Amount = data.discountAmount;
                obj.Discount_Code = data.discountCode;
                obj.Discount_ID = data.discountID;
                obj.Discount_Perm = data.discountPerm;
                obj.Exchange_Rate = data.exchangeRate;
                obj.FromCurrency_Code = data.fromCurrencyCode;
                //obj.HomeDeliveryAddress = data.homeDeliveryAddress;
                //obj.HomeDeliveryFlag = data.homeDeliveryFlag;
                obj.PaymentDepositType_ID = data.paymentDepositeTypeID;
                obj.PaymentType = data.paymentType;
                //obj.PaymentType_ID = data.paymentTypeID;
                obj.Purpose = data.purpose;
                obj.Purpose_ID = data.purposeID;
                obj.RateUpdateReason_ID = data.rateUpdateReasonId;
                obj.SOFID = data.sofID;
                obj.SourceComment = data.sourceComment;
                obj.SourceComment_Flag = data.sourceCommentFlag;
                obj.Transaction_From_Flag = data.transactionFromFlag;
                obj.TransferType = data.transferType;
                obj.Transfer_Cost = data.transferCost;
                obj.Transfer_Fees = data.transferFees;
                obj.User_ID = data.userID;
                obj.Wallet_Amount = data.walletAmount;
                obj.Wallet_Currency = data.walletCurrency;
                obj.Wallet_ID = data.walletID;
                obj.Wallet_Perm = data.walletPerm;
                obj.PType_ID = data.pTypeID;
                obj.TotalAmount = data.totalAmount;
                obj.Transaction_Source = data.transactionSource;
                obj.Username = data.username;
                obj.payWithBankGatewayId = data.payWithBankGatewayID;
                obj.PaymentGateway_ID = obj.payWithBankGatewayId;

                if (obj.PType_ID == 3 || obj.PType_ID  == 6)
                {
                    obj.PayByCard_ID = obj.PType_ID;
                }


                // This field added for security
                try
                {
                    obj.new_agent_id = data.newAgentID;
                }
                catch (Exception eg) { obj.new_agent_id = ""; }
                try
                {
                    obj.new_agent_branch = data.newAgentBranch;
                }
                catch (Exception eg) { obj.new_agent_branch = 0; }
                // This field added for security

                try
                {
                    obj.NameOnCard = data.NameOnCard;
                }
                catch (Exception eg) { obj.NameOnCard = ""; }

                try
                {
                    obj.improved_rate_flag = data.improved_rate_flag;
                    
                    if(obj.improved_rate_flag == 1)
                    {
                        obj.improved_rate_used_flag = 0;// Improve Rates used
                    }
                    else
                    {
                        obj.improved_rate_used_flag = 1;// Improve Rates not used
                    }

                }
                catch (Exception eg) { 
                    obj.improved_rate_flag = 0;
                    obj.improved_rate_used_flag = 1;// Improve Rates not used
                }
                try
                {
                    obj.ExtraTransfer_Fees = data.ExtraTransfer_Fees;
                }
                catch (Exception eg) { obj.ExtraTransfer_Fees = 0; }
                try
                {
                    obj.offer_rate_flag = data.offer_rate_flag;
                }
                catch (Exception eg) { obj.offer_rate_flag = 1; }
                try
                {
                    obj.Benf_BankDetails_ID = data.Benf_BankDetails_ID;
                }
                catch (Exception eg) { obj.Benf_BankDetails_ID = 0; }

                 obj.testingpurpose = 1;
                try
                {
                    obj.testingpurpose  = data.testingpurpose;
                }
                catch (Exception eg) { obj.testingpurpose = 1; }



                Service.srvSendMoney srv = new Service.srvSendMoney();
                DataTable dt = srv.InsertTemp(obj, context);

                Model.Dashboard _Obj = new Model.Dashboard();
                string returnURL = "";
                if (obj.PType_ID == 3)
                {                    
                    // Guavapay Start                    
                    if (obj.PaymentGateway_ID == 4)
                    {
                        DataTable li_guavapay = srv.GetGuavaPayLink(obj, dt.Rows[0][1].ToString(), obj.AmountInGBP, context);
                        if (li_guavapay != null)
                        {
                            if (li_guavapay.Rows.Count > 0)
                            {
                                returnURL = li_guavapay.Rows[0][1].ToString();
                            }
                        }
                    }
                    else if (obj.PaymentGateway_ID == 5)
                    {   // Ecommpay
                        DataTable li_ecommpay = srv.GetEcommpayPayLink(obj, dt.Rows[0][1].ToString(), obj.AmountInGBP, obj.PType_ID, context);
                        if (li_ecommpay != null)
                        {
                            if (li_ecommpay.Rows.Count > 0)
                            {
                                returnURL = li_ecommpay.Rows[0][1].ToString();
                            }
                        }
                    }
                    else if (obj.PaymentGateway_ID == 6)
                    {   // Nuvei
                        DataTable li_nuvei = srv.GetNuveiLink(obj, dt.Rows[0][1].ToString(), obj.AmountInGBP, obj.PType_ID, context);
                        if (li_nuvei != null)
                        {
                            if (li_nuvei.Rows.Count > 0)
                            {
                                returnURL = li_nuvei.Rows[0][1].ToString();
                            }
                        }
                    }
                    else if (obj.PaymentGateway_ID == 7)
                    {
                        // Emerchantpay
                        DataTable li_emerchantpay = srv.GetEmerchantpayPaymentLink(obj, dt.Rows[0][1].ToString(), obj.AmountInGBP, obj.PType_ID, context);
                        if (li_emerchantpay != null)
                        {
                            if (li_emerchantpay.Rows.Count > 0)
                            {
                                returnURL = li_emerchantpay.Rows[0][1].ToString();
                            }
                        }
                    }
                    else if (obj.PaymentGateway_ID == 8 || obj.PaymentGateway_ID == 1 || obj.PaymentGateway_ID == 2 || obj.PaymentGateway_ID == 9)
                    {
                        foreach (DataRow dr in dt.Rows)
                        {
                            returnURL = Convert.ToString(dr["redirectUrl"]);break;
                        }                       
                    }
                    }

                string webstring = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("ConnectionStrings")["WEB_DB_CONN"];
                webstring = webstring.ToLower();
                if (webstring.IndexOf("csremit", 0, StringComparison.OrdinalIgnoreCase) != -1)
                {
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        if (dt.Rows[0]["Status"].ToString() == "30")
                        {
                            response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_TECHNICAL_ERROR);
                            response.ResponseMessage = "No Records Matched.";
                            response.ResponseCode = 0;
                            returnJsonData = new { response = false, responseCode = "02", data = response.ResponseMessage };
                            return new JsonResult(returnJsonData);
                        }
                    }
                }


                    if (dt != null && dt.Rows.Count > 0)
                    {
                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_SUCCESS);
                    response.data = "success";
                    response.ObjData = dt;
                    response.ResponseCode = 0;
                    List<Dictionary<string, object>> Transaction = new List<Dictionary<string, object>>();

                    foreach (DataRow dr in dt.Rows)
                    {
                        Dictionary<string, object> wallet_balnce = new Dictionary<string, object>();
                        wallet_balnce["status"] = dr["Status"];
                        wallet_balnce["transactionReferenceNumber"] = dr["Refno"];
                        wallet_balnce["customerReferenceNumber"] = dr["CustRefno"];

                        if (obj.PType_ID == 3)
                        {
                            wallet_balnce["redirectUrl"] = returnURL;
                        }
                        if (obj.PType_ID == 6)
                        {
                            wallet_balnce["redirectUrl"] = dr["redirectUrl"];
                        }
                        if (obj.PType_ID == 10)
                        {
                            wallet_balnce["redirectUrl"] = dr["redirectUrl"];
                        }
                        if (obj.PType_ID == 11)
                        {
                            wallet_balnce["redirectUrl"] = dr["redirectUrl"];
                        }

                        wallet_balnce["apireference"] = dr["apireference"];
                        Transaction.Add(wallet_balnce);
                    }


                    returnJsonData = new { response = true, responseCode = "00", data = Transaction };
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
                Model.ErrorLog objError = new Model.ErrorLog();
                objError.User = new Model.User();
                objError.Branch = new Model.Branch();
                objError.Client = new Model.Client();

                objError.Error = "Api : InsertTemp --" + ex.ToString();
                objError.Branch_ID = 1;
                objError.Date = DateTime.Now;
                objError.User_ID = 1;
                objError.Id = 1;
                objError.Function_Name = "InsertTemp";
                Service.srvErrorLog srvError = new Service.srvErrorLog();
                srvError.Create(objError, HttpContext);
                returnJsonData = new { response = false, responseCode = "01", data = "Invalid request." };
                return new JsonResult(returnJsonData);
            }
            return new JsonResult(returnJsonData);
        }


        [HttpPost]
        [Route("uploadreceipt")]
        public async Task<IActionResult> Upload([FromForm] TransactionReceipt obj1, [FromForm] IFormFile? f_file)
        {
            var returnJsonData = (dynamic)null;
            try
            {
                
                HttpContext context = HttpContext;
                Model.response.WebResponse response = null;
                string json1 = System.Text.Json.JsonSerializer.Serialize(obj1);
                dynamic data = JObject.Parse(json1);
                TransactionReceipt obj = new TransactionReceipt();
                //Document doo = new Document();

                 CompanyInfo.InsertrequestLogTracker("uploadreceipt full request body: " + JObject.Parse(json1), 0, 0, 0, 0, "uploadreceipt", Convert.ToInt32(0), Convert.ToInt32(0), "", context);
               
                // Populate obj1 properties if they exist in the data

                obj.Client_ID = data.clientID;
                obj.Branch_ID = data.branchID;
                obj.customerid = data.customerID;

                if (f_file != null)
                {
                    obj.is_uploaded = "Y";

                }
                else
                {
                    obj.is_uploaded = "N";

                }
                string Btn_value = data.btnValue;
                var MadeThisTransfer_Flag = 1;
                if (Btn_value == "I made this payment")
                {
                    MadeThisTransfer_Flag = 0;
                    obj.MadeThisTransfer_Flag = MadeThisTransfer_Flag;
                    obj.MadeThisTransfer_Label = Btn_value;
                }
                if (Btn_value == "I haven't made this payment")
                {
                    MadeThisTransfer_Flag = 1;
                    obj.MadeThisTransfer_Flag = MadeThisTransfer_Flag;
                    obj.MadeThisTransfer_Label = Btn_value;
                }

                var validateJsonData = (dynamic)null;

                Service.srvViewTransferHistory srv = new Service.srvViewTransferHistory();
                DataTable li1 =await srv.Upload_Reciept(obj, context, f_file);
                string exist = li1.Rows[0]["status"].ToString();
                if (exist == "Upload receipt Failed")
                {
                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_TECHNICAL_ERROR);
                    response.data = "Upload receipt Failed";
                    response.ObjData = li1;
                    response.ResponseCode = 1;
                    returnJsonData = new { response = false, responseCode = "02", data = response.data };
                    return new JsonResult(returnJsonData);
                }
                else if (exist == "Failed")
                {
                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_TECHNICAL_ERROR);
                    response.data = "Failed";
                    response.ObjData = li1;
                    response.ResponseCode = 1;
                    returnJsonData = new { response = false, responseCode = "02", data = response.data };
                    return new JsonResult(returnJsonData);
                }
                else
                {
                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_TECHNICAL_ERROR);
                    response.data = "success";
                    response.ObjData = li1;
                    response.ResponseCode = 0;
                    returnJsonData = new { response = true, responseCode = "00", data = response.data };
                    return new JsonResult(returnJsonData);
                }


            }
            catch (Exception ex)
            {
                Model.ErrorLog objError = new Model.ErrorLog();
                objError.User = new Model.User();
                objError.Branch = new Model.Branch();
                objError.Client = new Model.Client();

                objError.Error = "Api : uploadreceipt --" + ex.ToString();
                objError.Branch_ID = 1;
                objError.Date = DateTime.Now;
                objError.User_ID = 1;
                objError.Id = 1;
                objError.Function_Name = "Upload";
                Service.srvErrorLog srvError = new Service.srvErrorLog();
                srvError.Create(objError, HttpContext);
                returnJsonData = new { response = false, responseCode = "01", data = "Invalid request." };
                return new JsonResult(returnJsonData);
            }
       

            /*
            Model.Dashboard _Obj = new Model.Dashboard();
            response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_SUCCESS);
            response.data = "success";
            response.ObjData = li1;
            response.ResponseCode = 0;
            return new JsonResult(response.ObjData);*/

        }

        [HttpPost]
        [Route("notmadethistransfer")]
        public IActionResult notmadethistransfer([FromBody] JsonObject obj)
        {
            HttpContext context = HttpContext;
            Model.response.WebResponse response = null;
            string json = System.Text.Json.JsonSerializer.Serialize(obj);
            dynamic data = JObject.Parse(json);
            CompanyInfo.InsertrequestLogTracker("notmadethistransfer full request body: " + JObject.Parse(json), 0, 0, 0, 0, "notmadethistransfer", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
            var validateJsonData = (dynamic)null;
            
            Transaction Obj = JsonConvert.DeserializeObject<Transaction>(json);
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
                if (Convert.ToString(data.transactionID).Trim() == "" || Convert.ToString(data.transactionID).Trim() == "" || Convert.ToString(data.transactionID).Trim() == null)
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "Invalid Transaction ID ." };
                    return new JsonResult(validateJsonData);
                }
                if (Convert.ToString(data.customerID).Trim() == "" || Convert.ToString(data.customerID).Trim() == "" || Convert.ToString(data.customerID).Trim() == null)
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "Invalid Customer ID ." };
                    return new JsonResult(validateJsonData);
                }



                Obj.Client_ID = data.clientID;
                Obj.Branch_ID = data.branchID;
                Obj.transactionID = data.transactionID;
                Obj.Transaction_ID = data.transactionID;
                Obj.Customer_ID = data.customerID;
                Obj.MadeThisTransfer_Flag = data.madeThisTransferFlag;
                Obj.MadeThisTransfer_Label = data.madeThisTransferLabel;


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

                Service.srvViewTransferHistory srv = new Service.srvViewTransferHistory();
                DataTable li1 = srv.MadeThisTransferFlag_Mail(Obj, context);
                Model.Dashboard _Obj = new Model.Dashboard();
                response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_SUCCESS);
                response.data = "success";
                response.ObjData = li1;
                response.ResponseCode = 0;

                string output = Convert.ToString(li1.Rows[0]["Message"]);

                validateJsonData = new { response = true, responseCode = "00", data = output };
                return new JsonResult(validateJsonData);
            }
            catch (Exception ex)
            {
                validateJsonData = new { response = false, responseCode = "01", data = "Invalid Request" };
                Obj.Client_ID = data.clientID;
                CompanyInfo.InsertErrorLogTracker(ex.ToString(), 0, 0, 0, 0, "notmadethistransfer", 0, Convert.ToInt32(Obj.Client_ID), "", HttpContext);
                return new JsonResult(validateJsonData);
            }
        }


        [HttpPost]        
        [Route("checkaddressdetailscustomers")]
        public JsonResult checkaddressdetailscustomers([FromBody] JsonElement objdata)
        {
            HttpContext context = HttpContext;
            List<Model.Transaction> _lst = new List<Model.Transaction>();
            var returnJsonData = (dynamic)null;
            Model.response.WebResponse response = null;
            string jsonData = System.Text.Json.JsonSerializer.Serialize(objdata);
            dynamic data = JObject.Parse(jsonData);
            CompanyInfo.InsertrequestLogTracker("checkaddressdetailscustomers full request body: " + JObject.Parse(jsonData), 0, 0, 0, 0, "checkaddressdetailscustomers", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);

            try
            {
                if (!SqlInjectionProtector.ReadJsonElementValues(objdata) || !SqlInjectionProtector.ReadJsonElementValuesScript(objdata))
                {
                    var errorResponse = new { response = false, responseCode = "02", data = "Invalid input detected." };
                    return new JsonResult(errorResponse) { StatusCode = 400 };
                }

                Transaction obj = new Transaction();
                obj.Branch_ID = data.branchID;
                obj.Client_ID = data.clientID;
                obj.Customer_ID = data.customerID;
                //obj.Beneficiary_ID = data.beneficiaryID;

                Service.srvSendMoney srv = new Service.srvSendMoney();
                DataTable licheckAddress = srv.CheckAddressDetailsCustomers(obj);
                if (licheckAddress != null && licheckAddress.Rows.Count != 0)
                {
                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_SUCCESS);
                    response.ResponseMessage = Convert.ToString(licheckAddress.Rows[0][1]);
                    response.ResponseCode = Convert.ToInt32(licheckAddress.Rows[0][0]);
                     
                    returnJsonData = new { response = true, responseCode = "00", data = response.ResponseMessage };
                    return new JsonResult(returnJsonData);
                }
                returnJsonData = new { response = false, responseCode = "02", data = "No record found" };
                return new JsonResult(returnJsonData);

            }
            catch (Exception ex)
            {
                returnJsonData = new { response = false, responseCode = "02", data = "No record found" };
                CompanyInfo.InsertErrorLogTracker("checkaddressdetailscustomers error: "+ex.ToString(), 0, 0, 0, 0, "checkaddressdetailscustomers", 0, Convert.ToInt32(0), "", HttpContext);
                return new JsonResult(returnJsonData);
            }            
        }

        
        [HttpPost]
        [Route("checkduplicatetxn")]
        public JsonResult checkduplicatetxn([FromBody] JsonElement objdata)
        {
            HttpContext context = HttpContext;
            List<Model.Transaction> _lst = new List<Model.Transaction>();
            var returnJsonData = (dynamic)null;
            Model.response.WebResponse response = null;
            string jsonData = System.Text.Json.JsonSerializer.Serialize(objdata);
            dynamic data = JObject.Parse(jsonData);
            CompanyInfo.InsertrequestLogTracker("checkduplicatetxn full request body: " + JObject.Parse(jsonData), 0, 0, 0, 0, "checkduplicatetxn", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
            try
            {
                if (!SqlInjectionProtector.ReadJsonElementValues(objdata) || !SqlInjectionProtector.ReadJsonElementValuesScript(objdata))
                {
                    var errorResponse = new { response = false, responseCode = "02", data = "Invalid input detected." };
                    return new JsonResult(errorResponse) { StatusCode = 400 };
                }

                Transaction obj = new Transaction();
                obj.Branch_ID = data.branchID;
                obj.Client_ID = data.clientID;
                obj.Customer_ID = data.customerID;
                obj.Beneficiary_ID = data.beneficiaryID;
                obj.LoginUserName = data.loginuserName; 

                Service.srvSendMoney srv = new Service.srvSendMoney();
                DataTable li1 = srv.CheckDuplicateTxn(obj,context);
                Model.Dashboard _Obj = new Model.Dashboard();
                response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_SUCCESS);
                response.data = "success";
                response.ObjData = li1;
                response.ResponseCode = 0;

                var relationshipData = li1.Rows.OfType<DataRow>()
                .Select(row => li1.Columns.OfType<DataColumn>()
                    .ToDictionary(col => col.ColumnName, c => row[c]));

                returnJsonData = new { response = true, responseCode = "00", data = relationshipData };
                return new JsonResult(returnJsonData);
            }
            catch (Exception ex)
            {
                Transaction obj = new Transaction();                
                obj.Client_ID = data.clientID;

                response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_TECHNICAL_ERROR);
                response.sErrorExceptionText = ex.ToString();
                response.ResponseMessage = "technical error";
                response.ResponseCode = 3;
                //Error Log Handled
                Model.ErrorLog objError = new Model.ErrorLog();
                objError.User = new Model.User();
                objError.Error = "Api : Send-CheckDuplicateTxn --" + ex.ToString();
                objError.Date = DateTime.Now;
                objError.User_ID = 1;
                objError.Client_ID = obj.Client_ID;
                objError.Function_Name = "CheckDuplicateTxn";
                Service.srvErrorLog srvError = new Service.srvErrorLog();
                srvError.Create(objError, context);
                returnJsonData = new { response = false, responseCode = "01", data = "Invalid request" };
                return new JsonResult(returnJsonData);
            }
        }


        [HttpPost]
        [Authorize]
        [Route("insertonlinetransaction")]
        public JsonResult InsertOnlineTransaction([FromBody] JsonElement objdata)
        {
            HttpContext context = HttpContext;
            List<Model.Transaction> _lst = new List<Model.Transaction>();
            var returnJsonData = (dynamic)null;
            Model.response.WebResponse response = null;
            string jsonData = System.Text.Json.JsonSerializer.Serialize(objdata);
            dynamic data = JObject.Parse(jsonData);
            CompanyInfo.InsertrequestLogTracker("InsertOnlineTransaction full request body: " + JObject.Parse(jsonData), 0, 0, 0, 0, "insertonlinetransaction", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
            try
            {
                if (!SqlInjectionProtector.ReadJsonElementValues(objdata) || !SqlInjectionProtector.ReadJsonElementValuesScript(objdata))
                {
                    var errorResponse = new { response = false, responseCode = "02", data = "Invalid input detected." };
                    return new JsonResult(errorResponse) { StatusCode = 400 };
                }

                #region validateData
                //try
                //{
                //    if (data.clientID == "" || data.clientID == null)
                //    {
                //        returnJsonData = new { response = false, responseCode = "02", data = "Invalid Client ID." };
                //        return new JsonResult(returnJsonData);
                //    }
                //    else if (data.branchID == "" || data.branchID == null)
                //    {
                //        returnJsonData = new { response = false, responseCode = "02", data = "Invalid Branch ID." };
                //        return new JsonResult(returnJsonData);
                //    }
                //    else if (data.countryID == "" || data.countryID == null)
                //    {
                //        returnJsonData = new { response = false, responseCode = "02", data = "Invalid Country ID." };
                //        return new JsonResult(returnJsonData);
                //    }
                //}
                //catch (Exception ex)
                //{
                //    Model.ActivityLogTracker _objActivityLog = new Model.ActivityLogTracker();
                //    _objActivityLog.Activity = " Exception Found City Error: " + ex.ToString() + " ";
                //    _objActivityLog.FunctionName = "citylist";
                //    _objActivityLog.Transaction_ID = 0;
                //    _objActivityLog.WhoAcessed = 1;
                //    _objActivityLog.Branch_ID = 0;
                //    _objActivityLog.Client_ID = 0;

                //    Service.srvActivityLogTracker srvActivityLog = new Service.srvActivityLogTracker();
                //    int _i = srvActivityLog.Create(_objActivityLog, HttpContext);

                //    returnJsonData = new { response = false, responseCode = "01", data = "Field is missing." };
                //    return new JsonResult(returnJsonData);
                //}
                #endregion

                object o = JsonConvert.DeserializeObject(jsonData);
                bool checkinjection = CompanyInfo.checksqlinjectiondata(o);
                if (!checkinjection)
                {
                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_TECHNICAL_ERROR);
                    response.ResponseMessage = "Invalid Field Values.";
                    response.ResponseCode = 6;
                    returnJsonData = new { response = false, responseCode = "02", data = response.ResponseMessage };
                    return new JsonResult(returnJsonData);
                }

                Transaction obj = new Transaction();
                obj.CB_ID = data.branchID;
                obj.Client_ID = data.clientID;
                obj.Customer_ID = data.customerID;
                obj.ReferenceNo = data.transactionReferenceNumber;
                obj.User_ID = data.userID;
                obj.Username = data.username;

                try
                {
                    obj.PaymentGateway_ID =  Convert.ToInt32(data.PaymentGateway_ID);
                }
                catch (Exception ex) { obj.PaymentGateway_ID = 0; }

                // For Pay with bank
                try
                {
                    obj.payvyne_trans_id = data.payvynetransid;
                    obj.bankGateway = data.bankGateway;
                    obj.Worldpay_Response = data.Worldpay_Response;
                    obj.sanction_responce_bene_aml = data.sanction_responce_bene_aml;
                    obj.sanction_responce_bene_kyc = data.sanction_responce_bene_kyc;
                    obj.sanction_responce_cust_aml = data.sanction_responce_cust_aml;
                    obj.sanction_responce_cust_kyc = data.sanction_responce_cust_kyc;
                }
                catch (Exception ex)
                {
                }
                try
                {
                    obj.Benf_BankDetails_ID = data.Benf_BankDetails_ID;
                }
                catch (Exception eg) { obj.Benf_BankDetails_ID = 0; }

                int testingpurpose = 1;
                try
                {
                    testingpurpose = data.testingpurpose;
                }
                catch (Exception eg) { testingpurpose = 1; }

                Service.srvSendMoney srv = new Service.srvSendMoney();
                //DataTable dt = srv.InsertOnline(obj, context);

                DataTable dt = null;
               /* if (testingpurpose == 1)
                    dt = srv.InsertOnline(obj, context);
                else*/
                    dt = srv.InsertOnline_new(obj, context);


                if (dt != null && dt.Rows.Count > 0)
                {
                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_SUCCESS);
                    response.data = "success";
                    response.ObjData = dt;
                    response.ResponseCode = 0;

                    foreach (DataColumn column in dt.Columns)
                    {
                        // Replace "OldColumnName" with the desired new name
                        column.ColumnName = column.ColumnName.Replace("API_ID", "apiID");
                        column.ColumnName = column.ColumnName.Replace("API_BranchDetails", "apiBranchDetials");
                        column.ColumnName = column.ColumnName.Replace("Transaction_ID", "transactionID");
                        column.ColumnName = column.ColumnName.Replace("ReferenceNo", "transactionReferenceNumber");
                        column.ColumnName = column.ColumnName.Replace("Customer_ID", "customerReferenceNumber");
                        column.ColumnName = column.ColumnName.Replace("Beneficiary_ID", "beneficiaryID");
                        column.ColumnName = column.ColumnName.Replace("TransactionType_ID", "transactionTypeID");
                        column.ColumnName = column.ColumnName.Replace("PaymentType_ID", "paymentTypeID");
                        column.ColumnName = column.ColumnName.Replace("TransactionStatus_ID", "transactionStatusID");
                        column.ColumnName = column.ColumnName.Replace("AmountInGBP", "amountInGBP");
                        column.ColumnName = column.ColumnName.Replace("Exchange_Rate", "exchangeRate");
                        column.ColumnName = column.ColumnName.Replace("AmountInPKR", "ReceiveingAmount");
                        column.ColumnName = column.ColumnName.Replace("FromCurrency_Code", "fromCurrencyCode");
                        column.ColumnName = column.ColumnName.Replace("Currency_Code", "currencyCode");
                        column.ColumnName = column.ColumnName.Replace("Country_ID", "countryID");
                        column.ColumnName = column.ColumnName.Replace("Purpose_ID", "purposeID");
                        column.ColumnName = column.ColumnName.Replace("SourceofFunds", "sourceOfFund");
                        column.ColumnName = column.ColumnName.Replace("Transfer_Fees", "transferFees");
                        column.ColumnName = column.ColumnName.Replace("Credit_Status", "creditStatus");
                        column.ColumnName = column.ColumnName.Replace("Record_Insert_DateTime", "recordInsertDateTime");
                        column.ColumnName = column.ColumnName.Replace("Delete_Status", "deleteStatus");
                        column.ColumnName = column.ColumnName.Replace("PaymentDepositType_ID", "paymentDepositTypeID");
                        column.ColumnName = column.ColumnName.Replace("Agent_ID", "agentID");
                        column.ColumnName = column.ColumnName.Replace("CollectionPoint_ID", "collectionPointID");
                        column.ColumnName = column.ColumnName.Replace("DeliveryType_Id", "deleiveryTypeID");
                        column.ColumnName = column.ColumnName.Replace("CB_ID", "branchID");
                        column.ColumnName = column.ColumnName.Replace("paymentReceived_ID", "paymentReceivedID");
                        column.ColumnName = column.ColumnName.Replace("User_ID", "userID");
                        column.ColumnName = column.ColumnName.Replace("Agent_MappingID", "agentMappingID");
                        column.ColumnName = column.ColumnName.Replace("TransactionThrough_ID", "transactionThroughID");
                        column.ColumnName = column.ColumnName.Replace("SpotOnApp_SendMoneyFlag", "spotOnAppSendMoneyFlag");
                        column.ColumnName = column.ColumnName.Replace("MadeThisTransfer_Flag", "madeThisTransferFlag");
                        column.ColumnName = column.ColumnName.Replace("Proceed_Reason", "proceedReason");
                        column.ColumnName = column.ColumnName.Replace("Transaction_From_Flag", "transactionFromFlag");
                        column.ColumnName = column.ColumnName.Replace("RemainingPartial_Amount", "remainingPartialAmount");
                        column.ColumnName = column.ColumnName.Replace("DepositePartial_Amount", "DepositePartialAmount");
                        column.ColumnName = column.ColumnName.Replace("PartialDeposit_Flag", "partialDeopositFlag");
                        column.ColumnName = column.ColumnName.Replace("auth_code", "authCode");
                        column.ColumnName = column.ColumnName.Replace("Actual_ExchangeRate", "actualExchangeRate");
                        column.ColumnName = column.ColumnName.Replace("ManualRateChangedFlag", "manualRatechangedFlag");
                        column.ColumnName = column.ColumnName.Replace("RateUpdateReason_ID", "rateUpdateReasonID");
                        column.ColumnName = column.ColumnName.Replace("MsgToAgent", "messageToAgent");
                        column.ColumnName = column.ColumnName.Replace("Flag_for_cancellation", "flagForCancellation");
                        column.ColumnName = column.ColumnName.Replace("Note_From_Cashier_for_TxCancellation_Flag", "notefromcashierforcancellation");
                        column.ColumnName = column.ColumnName.Replace("cancellation_flag_By_UserId", "cancellationFlagbyUserID");
                        column.ColumnName = column.ColumnName.Replace("GCCTransactionNo", "gccTransactionNo");
                        column.ColumnName = column.ColumnName.Replace("GCCPayoutBranch_ID", "gccPayoutBrancID");
                        column.ColumnName = column.ColumnName.Replace("GCCRate", "gccRate");
                        column.ColumnName = column.ColumnName.Replace("Agent_ID", "agentID");
                    }

                    var relationshipData = dt.Rows.OfType<DataRow>()
                .Select(row => dt.Columns.OfType<DataColumn>()
                    .ToDictionary(col => col.ColumnName, c => row[c].ToString() ));
                    List<Dictionary<string, object>> Transaction = new List<Dictionary<string, object>>();

                    returnJsonData = new { response = true, responseCode = "00", data = relationshipData };
                    return new JsonResult(returnJsonData);
                }
                else
                {
                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_TECHNICAL_ERROR);
                    response.ResponseMessage = "No Records Found.";
                    response.ResponseCode = 0;
                    returnJsonData = new { response = false, responseCode = "01", data = response.ResponseMessage };
                    return new JsonResult(returnJsonData);
                }
            }
            catch (Exception ex)
            {
                Model.ErrorLog objError = new Model.ErrorLog();
                objError.User = new Model.User();
                objError.Branch = new Model.Branch();
                objError.Client = new Model.Client();

                objError.Error = "Api : InsertOnlineTransaction --" + ex.ToString();
                objError.Branch_ID = 1;
                objError.Date = DateTime.Now;
                objError.User_ID = 1;
                objError.Id = 1;
                objError.Function_Name = "InsertOnlineTransaction";
                Service.srvErrorLog srvError = new Service.srvErrorLog();
                srvError.Create(objError, HttpContext);
                returnJsonData = new { response = false, responseCode = "01", data = "Invalid request." };
                return new JsonResult(returnJsonData);
            }
            return new JsonResult(returnJsonData);
        }

        [HttpPost]
        [Authorize]
        [Route("checklimit_new")]
        public JsonResult checklimit_new([FromBody] JsonElement objdata)
        {
            HttpContext context = HttpContext;
            List<Model.Transaction> _lst = new List<Model.Transaction>();
            var returnJsonData = (dynamic)null;
            Model.response.WebResponse response = null;
            string jsonData = System.Text.Json.JsonSerializer.Serialize(objdata);
            dynamic data = JObject.Parse(jsonData);
            CompanyInfo.InsertrequestLogTracker("checklimit_new full request body: " + JObject.Parse(jsonData), 0, 0, 0, 0, "checklimit_new", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
            try
            {
                if (!SqlInjectionProtector.ReadJsonElementValues(objdata) || !SqlInjectionProtector.ReadJsonElementValuesScript(objdata))
                {
                    var errorResponse = new { response = false, responseCode = "02", data = "Invalid input detected." };
                    return new JsonResult(errorResponse) { StatusCode = 400 };
                }
                string readTokenValue = "";
                if (Request.Headers.TryGetValue("Authorization", out var authorizationHeader))
                {
                    readTokenValue = authorizationHeader.ToString();
                }


                #region validateData
                try
                {
                    if (data.sendingAmount == "" || data.sendingAmount == null)
                    {
                        returnJsonData = new { response = false, responseCode = "02", data = "Invalid Sending Amount." };
                        return new JsonResult(returnJsonData);
                    }
                    else if (data.receivingAmount == "" || data.receivingAmount == null)
                    {
                        returnJsonData = new { response = false, responseCode = "02", data = "Invalid Receiving Amount." };
                        return new JsonResult(returnJsonData);
                    }
                    else if (data.beneficiaryID == "" || data.beneficiaryID == null)
                    {
                        returnJsonData = new { response = false, responseCode = "02", data = "Invalid Beneficiary ID." };
                        return new JsonResult(returnJsonData);
                    }
                    else if (data.branchID == "" || data.branchID == null)
                    {
                        returnJsonData = new { response = false, responseCode = "02", data = "Invalid Branch ID." };
                        return new JsonResult(returnJsonData);
                    }
                    else if (data.clientID == "" || data.clientID == null)
                    {
                        returnJsonData = new { response = false, responseCode = "02", data = "Invalid Client ID." };
                        return new JsonResult(returnJsonData);
                    }
                    else if (data.customerID == "" || data.customerID == null)
                    {
                        returnJsonData = new { response = false, responseCode = "02", data = "Invalid Customer ID." };
                        return new JsonResult(returnJsonData);
                    }
                    else if (data.deliveryTypeID == "" || data.deliveryTypeID == null)
                    {
                        returnJsonData = new { response = false, responseCode = "02", data = "Invalid Delivery Type ID." };
                        return new JsonResult(returnJsonData);
                    }
                    else if (data.exchangeRate == "" || data.exchangeRate == null)
                    {
                        returnJsonData = new { response = false, responseCode = "02", data = "Invalid Exchange Rate." };
                        return new JsonResult(returnJsonData);
                    }
                    else if (data.fromCurrencyCode == "" || data.fromCurrencyCode == null)
                    {
                        returnJsonData = new { response = false, responseCode = "02", data = "Invalid From Currency Code." };
                        return new JsonResult(returnJsonData);
                    }
                    else if (data.gccAmountInGBP == "" || data.gccAmountInGBP == null)
                    {
                        returnJsonData = new { response = false, responseCode = "02", data = "Invalid Gcc Amount In GBP." };
                        return new JsonResult(returnJsonData);
                    }
                    else if (data.paymentDepositeTypeID == "" || data.paymentDepositeTypeID == null)
                    {
                        returnJsonData = new { response = false, responseCode = "02", data = "Invalid Payment Deposite Type ID." };
                        return new JsonResult(returnJsonData);
                    }
                    else if (data.paymentTypeID == "" || data.paymentTypeID == null)
                    {
                        returnJsonData = new { response = false, responseCode = "02", data = "Invalid Payment Type ID." };
                        return new JsonResult(returnJsonData);
                    }
                    else if (data.transferFees == "" || data.transferFees == null)
                    {
                        returnJsonData = new { response = false, responseCode = "02", data = "Invalid Transfer Fees." };
                        return new JsonResult(returnJsonData);
                    }
                    else if (data.username == "" || data.username == null)
                    {
                        returnJsonData = new { response = false, responseCode = "02", data = "Invalid Username." };
                        return new JsonResult(returnJsonData);
                    }
                }
                catch (Exception ex)
                {
                    Model.ActivityLogTracker _objActivityLog = new Model.ActivityLogTracker();
                    _objActivityLog.Activity = " Exception Found CheckLimit Error: " + ex.ToString() + " ";
                    _objActivityLog.FunctionName = "CheckLimit";
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

                object o = JsonConvert.DeserializeObject(jsonData);
                bool checkinjection = CompanyInfo.checksqlinjectiondata(o);
                if (!checkinjection)
                {
                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_TECHNICAL_ERROR);
                    response.ResponseMessage = "Invalid Field Values.";
                    response.ResponseCode = 6;
                    returnJsonData = new { response = false, responseCode = "02", data = response.ResponseMessage };
                    return new JsonResult(returnJsonData);
                }

                Transaction obj = new Transaction();

                obj.AmountInGBP = data.sendingAmount;

                string receivingAmt = Convert.ToString(data.receivingAmount).Trim();

                obj.AmountInPKR = Convert.ToDouble(receivingAmt);  // data.receivingAmount  ;
                obj.Beneficiary_ID = data.beneficiaryID;
                obj.CB_ID = data.branchID;
                obj.Client_ID = data.clientID;
                obj.Customer_ID = data.customerID;
                obj.DeliveryType_Id = data.deliveryTypeID;
                obj.Exchange_Rate = data.exchangeRate;
                obj.FromCurrency_Code = data.fromCurrencyCode;
                obj.GCCAmountInGBP = data.gccAmountInGBP;
                obj.PaymentDepositType_ID = data.paymentDepositeTypeID;
                obj.PaymentType_ID = data.paymentTypeID;
                obj.Transfer_Fees = data.transferFees;
                obj.Username = data.username;
                obj.readTokenValue = readTokenValue;

                Service.srvSendMoney srv = new Service.srvSendMoney();
                //DataTable dt = srv.CheckAllLimits_new(obj, context);
                DataTable dt = srv.CheckAllLimits(obj, context);
                if (dt != null && dt.Rows.Count > 0)
                {
                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_SUCCESS);
                    response.data = "success";
                    response.ObjData = dt;
                    response.ResponseCode = 0;

                    //1 : Primary Doc upload, 2 : Secondary(Addressproof) upload , 4 : Soource of fund upload, 5 : Restrict/Stop, 10 : Secondary upload with Yes No button , 11 : Wallet Balance 

                    dt.Columns.Add("settodocument", typeof(string));
                    dt.Columns.Add("showingmessage", typeof(string));
                    string message = "";
                    foreach (DataRow row in dt.Rows)
                    {
                        int settodocumentvalue = 0;
                        string showingmessage = "";

                        if (Convert.ToString(row["PrimaryIdmsg"]) == "")
                        {
                            settodocumentvalue = 5;
                            showingmessage = Convert.ToString(row["PrimaryIdmsg"]);
                        }
                        else if (Convert.ToInt32(row["BlacklistedFlag"]) == 1)
                        {

                            DataTable dtcompanydetails = (DataTable)CompanyInfo.GetBaseCurrencywisebankdetails(obj.Client_ID, obj.FromCurrency_Code, obj.PaymentDepositType_ID, obj.DeliveryType_Id);
                            if (dtcompanydetails != null && dtcompanydetails.Rows.Count > 0)
                            {
                                foreach (DataRow dr in dt.Rows)
                                {
                                    if (dr["Company_Email"] != "" && dr["Company_Email"] != null && dr["Company_mobile"] != "" && dr["Company_mobile"] != null)
                                    {
                                        message = " You can call us at <a href='tel:" + dr["Company_mobile"] + "'>" + dr["Company_mobile"] + "</a> or " +
                        "send email to <a href='mailto:" + dr["Company_Email"] + "'>" + dr["Company_Email"] + "</a>. Thank You.";
                                    }
                                    else if (dr["Company_mobile"] != "" && dr["Company_mobile"] != null)
                                    {
                                        message = message + " You can call us at <a href='tel:" + dr["Company_mobile"] + "'>" + dr["Company_mobile"] + "</a>. Thank You.";
                                    }
                                    else if (dr["Company_Email"] != "" && dr["Company_Email"] != null)
                                    {
                                        message = message + " You can send email to <a href='mailto:" + dr["Company_Email"] + "'>" + dr["Company_Email"] + "</a>. Thank You.";
                                    }

                                }
                            }

                            settodocumentvalue = 5;
                            showingmessage = Convert.ToString(row["Blacklistedmessage"]) + " " + message;
                        }
                        else
                        {
                            Beneficiary objTranLimit = JsonConvert.DeserializeObject<Beneficiary>(jsonData);
                            objTranLimit.Client_ID = obj.Client_ID;
                            string beneficiaryCountry = "", beneficiaryCollectionType = "", foreignCurrencyCode = "";

                            try
                            {
                                objTranLimit.whereclause = data.whereclause;
                                beneficiaryCountry = data.beneficiaryCountry;
                                beneficiaryCollectionType = data.beneficiaryCollectionType;
                                foreignCurrencyCode = data.foreignCurrencyCode;
                            }
                            catch (Exception egx)
                            {

                            }


                            Service.srvBeneficiary srvTranLimit = new Service.srvBeneficiary(context);
                            DataTable[] dtTranLimit = new DataTable[1];
                            dtTranLimit[0] = srvTranLimit.GetConfig(objTranLimit);
                            if (dtTranLimit[0] != null && (beneficiaryCollectionType != "" && beneficiaryCountry!= "" ))
                            {
                                foreach (DataRow rowv in dtTranLimit[0].Rows)
                                {
                                    if (Convert.ToDouble(rowv["Trans_limit_man"]) == 0)
                                    {
                                        double current_amt = Convert.ToDouble(obj.AmountInPKR);

                                        if (current_amt <= Convert.ToDouble(rowv["Trans_limit_min"]))
                                        {
                                            settodocumentvalue = 5;
                                            showingmessage = foreignCurrencyCode + " " + Convert.ToString(rowv["Trans_limit_min"]) + " minimum limit applies for " + beneficiaryCollectionType + " for " + beneficiaryCountry;
                                        }

                                        if (current_amt <= Convert.ToDouble(rowv["Trans_limit_max"]))
                                        {
                                        }
                                        else
                                        {
                                            settodocumentvalue = 5;
                                            showingmessage = foreignCurrencyCode + " " + Convert.ToString(rowv["Trans_limit_max"]) + " maximum limit applies for " + beneficiaryCollectionType + " for " + beneficiaryCountry;
                                        }

                                        if (Convert.ToString(rowv["Trans_lmt_perday"]) != null && Convert.ToString(rowv["Trans_lmt_perday"]) != "null" && Convert.ToString(rowv["Trans_lmt_perday"]) != "")
                                        {   // Transaction limit per day
                                            if ((current_amt + Convert.ToDouble(row["TotalCustAmount1"])) >= Convert.ToDouble(rowv["Trans_lmt_perday"]))
                                            {
                                                settodocumentvalue = 5;
                                                showingmessage = foreignCurrencyCode + " " + Convert.ToDouble(rowv["Trans_lmt_perday"]).ToString() + " daily limit applies for " + beneficiaryCollectionType + " for " + beneficiaryCountry;
                                            }
                                        }

                                        if (Convert.ToString(rowv["Trans_lmtt_perday_benf"]) != null && Convert.ToString(rowv["Trans_lmtt_perday_benf"]) != "null" && Convert.ToString(rowv["Trans_lmtt_perday_benf"]) != "")
                                        {   // Transaction limit per day for beneficiary
                                            if ((current_amt + Convert.ToDouble(row["totalbenfamt"])) >= Convert.ToDouble(rowv["Trans_lmtt_perday_benf"]))
                                            {
                                                settodocumentvalue = 5;
                                                showingmessage = foreignCurrencyCode + " " + Convert.ToDouble(rowv["Trans_lmtt_perday_benf"]).ToString() + " daily limit applies to this beneficiary for " + beneficiaryCollectionType + " for " + beneficiaryCountry;
                                            }
                                        }

                                        if (Convert.ToString(rowv["Trans_lmt_peryear_benf"]) != null && Convert.ToString(rowv["Trans_lmt_peryear_benf"]) != "null" && Convert.ToString(rowv["Trans_lmt_peryear_benf"]) != "")
                                        {   // Transaction limit per day for beneficiary
                                            if ((current_amt + Convert.ToDouble(row["TotalBenfAmtYr"])) >= Convert.ToDouble(rowv["Trans_lmt_peryear_benf"]))
                                            {
                                                settodocumentvalue = 5;
                                                showingmessage = foreignCurrencyCode + " " + Convert.ToDouble(rowv["Trans_lmt_peryear_benf"]).ToString() + " yearly limit applies to this beneficiary for " + beneficiaryCollectionType + " for " + beneficiaryCountry;
                                            }
                                        }

                                    }

                                }
                            }
                        }


                        if (Convert.ToInt32(row["SourceOfFunds_Limit"]) == 1)
                        {
                            settodocumentvalue = 4;
                            showingmessage = "Source of fund require";
                            showingmessage = Convert.ToString(row["IDUploadmsg"]);
                        }
                        else if (Convert.ToInt32(row["PrimaryID_Limit"]) == 1)
                        {
                            settodocumentvalue = 1;
                            showingmessage = "Primary document require";
                            showingmessage = Convert.ToString(row["IDUploadmsg"]);
                        }
                        else if (Convert.ToInt32(row["SecondaryID_Limit"]) == 1)
                        {
                            settodocumentvalue = 2;
                            showingmessage = "Secondary document require";
                            showingmessage = Convert.ToString(row["IDUploadmsg"]);
                        }

                        if (Convert.ToInt32(row["monlimit"]) == 1)
                        {
                            settodocumentvalue = 5;
                            showingmessage = Convert.ToString(row["monthlylimitmsg"]);
                        }
                        else if (Convert.ToInt32(row["monlimit"]) == 2)
                        {
                            settodocumentvalue = 4;
                            showingmessage = Convert.ToString(row["monthlylimitmsg"]);
                        }

                        if (Convert.ToInt32(row["dailylimit"]) == 1)
                        {
                            settodocumentvalue = 4;
                            showingmessage = Convert.ToString(row["Daily_Limit_Message"]);
                        }
                        else if (Convert.ToInt32(row["dailylimit"]) == 2)
                        {
                            settodocumentvalue = 5;
                            showingmessage = Convert.ToString(row["Daily_Limit_Message"]);
                        }

                        if (Convert.ToInt32(row["AMLlimit"]) == 4)
                        {
                            settodocumentvalue = 4;
                            showingmessage = Convert.ToString(row["amlLimitmsg"]);
                        }
                        else if (Convert.ToInt32(row["AMLlimit"]) == 2)
                        {
                            settodocumentvalue = 2;
                            showingmessage = Convert.ToString(row["amlLimitmsg"]);
                        }
                        else if (Convert.ToInt32(row["AMLlimit"]) == 1)
                        {
                            settodocumentvalue = 1;
                            showingmessage = Convert.ToString(row["amlLimitmsg"]);
                        }
                        else if (Convert.ToInt32(row["AMLlimit"]) == 10)
                        {
                            settodocumentvalue = 10;
                            showingmessage = Convert.ToString(row["AMLlimitmsg"]);
                        }
                        else if (Convert.ToInt32(row["payWithWallet"]) == 1)
                        {
                            settodocumentvalue = 11;
                            showingmessage = "Your wallet balance is insufficient. Please add more funds to the wallet and try again.";
                        }
                        else if (Convert.ToInt32(row["IsValidID"]) == 1)
                        {
                            settodocumentvalue = 1;
                            showingmessage = Convert.ToString(row["IDUploadmsg"]);
                        }
                        else if (Convert.ToInt32(row["daily_transfer_count"]) > 0)
                        {
                            settodocumentvalue = 5;
                            showingmessage = Convert.ToString(row["daily_transfer_msg"]);
                        }
                        else if (Convert.ToInt32(row["SOFDaysCount"]) > 0)
                        {
                            settodocumentvalue = 5;
                            showingmessage = Convert.ToString(row["SOFDaysCount_Msg"]);
                        }
                        else if (Convert.ToInt32(row["AllowProceedFlag"]) > 0)
                        {
                            settodocumentvalue = 5;
                            showingmessage = Convert.ToString(row["AllowProceedFlag_Msg"]);
                        }
                        else if (Convert.ToInt32(row["paymenttypelimit"]) == 1)
                        {
                            settodocumentvalue = 5;
                            showingmessage = Convert.ToString(row["paymenttypelimitmsg"]);
                        }
                        else if (Convert.ToInt32(row["collectiontypelimit"]) == 1)
                        {
                            settodocumentvalue = 5;
                            showingmessage = Convert.ToString(row["collectiontypelimitmsg"]);
                        }


                        row["settodocument"] = settodocumentvalue;
                        row["showingmessage"] = showingmessage;
                        break;
                    }

                    foreach (DataColumn column in dt.Columns)
                    {
                        // Replace "OldColumnName" with the desired new name
                        column.ColumnName = column.ColumnName.Replace("Status", "status");
                        column.ColumnName = column.ColumnName.Replace("Errormessage", "errorMessage");
                        column.ColumnName = column.ColumnName.Replace("BlacklistedFlag", "blackListedFlag");
                        column.ColumnName = column.ColumnName.Replace("Blacklistedmessage", "blackListedMessage");
                        column.ColumnName = column.ColumnName.Replace("SOFDaysCount", "sofDaysCount");
                        column.ColumnName = column.ColumnName.Replace("SOFDaysCount_Msg", "sofDaysCountMessage");
                        column.ColumnName = column.ColumnName.Replace("AMLlimit", "amlLimit");
                        column.ColumnName = column.ColumnName.Replace("AMLlimitmsg", "amlLimitMessage");
                        column.ColumnName = column.ColumnName.Replace("SourceOfFunds_Limit", "sourceOfFundLimit");
                        column.ColumnName = column.ColumnName.Replace("PrimaryID_Limit", "primaryIDLimit");
                        column.ColumnName = column.ColumnName.Replace("SecondaryID_Limit", "secondaryIDLimit");
                        column.ColumnName = column.ColumnName.Replace("Custlimit", "custLimit");
                        column.ColumnName = column.ColumnName.Replace("Custlimitmsg", "custLimitMesaage");
                        column.ColumnName = column.ColumnName.Replace("daily_transfer_count", "dailyTransferCount");
                        column.ColumnName = column.ColumnName.Replace("daily_transfer_msg", "dailyTransferMessage");
                        column.ColumnName = column.ColumnName.Replace("dailylimit", "dailyLimit");
                        column.ColumnName = column.ColumnName.Replace("Daily_Limit_Message", "dailyLimitMessage");
                        column.ColumnName = column.ColumnName.Replace("TotalCustAmount", "totalCustAmount");
                        column.ColumnName = column.ColumnName.Replace("paymenttypelimit", "paymentTypeLimit");
                        column.ColumnName = column.ColumnName.Replace("paymenttypelimitmsg", "paymentTypeLimitMessage");
                        column.ColumnName = column.ColumnName.Replace("collectiontypelimit", "collectionTypeLimit");
                        column.ColumnName = column.ColumnName.Replace("IsValidID", "isValidID");
                        column.ColumnName = column.ColumnName.Replace("IDUploadmsg", "idUploadMessage");
                        column.ColumnName = column.ColumnName.Replace("AllowProceedFlag", "allowProceedFlag");
                        column.ColumnName = column.ColumnName.Replace("AllowProceedFlag_Msg", "allowProceedFlagMessage");
                        column.ColumnName = column.ColumnName.Replace("TotalBenfAmount", "totlaBeneficiaryAmount");
                        column.ColumnName = column.ColumnName.Replace("TotalCustAmount1", "totalcustomerAmount");
                        column.ColumnName = column.ColumnName.Replace("TotalBenfAmtYr", "totalBeneficiaryAmountYear");
                        column.ColumnName = column.ColumnName.Replace("transfer_count_days", "transferCountDays");
                        column.ColumnName = column.ColumnName.Replace("paywithwallet", "payWithWallet");
                        column.ColumnName = column.ColumnName.Replace("PrimaryIdmsg", "primaryIDMessage");

                    }

                    var relationshipData = dt.Rows.OfType<DataRow>()
                .Select(row => dt.Columns.OfType<DataColumn>()
                    .ToDictionary(col => col.ColumnName, c => row[c].ToString()));
                    List<Dictionary<string, object>> Transaction = new List<Dictionary<string, object>>();

                    returnJsonData = new { response = true, responseCode = "00", data = relationshipData };
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
                Model.ErrorLog objError = new Model.ErrorLog();
                objError.User = new Model.User();
                objError.Branch = new Model.Branch();
                objError.Client = new Model.Client();

                objError.Error = "Api : CheckLimit --" + ex.ToString();
                objError.Branch_ID = 1;
                objError.Date = DateTime.Now;
                objError.User_ID = 1;
                objError.Id = 1;
                objError.Function_Name = "CheckLimit";
                Service.srvErrorLog srvError = new Service.srvErrorLog();
                srvError.Create(objError, HttpContext);
                returnJsonData = new { response = false, responseCode = "01", data = "Invalid request." };
                return new JsonResult(returnJsonData);
            }
            return new JsonResult(returnJsonData);
        }


        [HttpPost]
        [Authorize]
        [Route("checklimitfirststep")]
        public JsonResult CheckLimitfirststep([FromBody] JsonElement objdata)
        {
            HttpContext context = HttpContext;
            List<Model.Transaction> _lst = new List<Model.Transaction>();
            var returnJsonData = (dynamic)null;
            Model.response.WebResponse response = null;
            string jsonData = System.Text.Json.JsonSerializer.Serialize(objdata);
            dynamic data = JObject.Parse(jsonData);
            _ = Task.Run(() => CompanyInfo.InsertrequestLogTracker("checklimitfirststep full request body: " + JObject.Parse(jsonData), 0, 0, 0, 0, "checklimitfirststep", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext));
            try
            {
                if (!SqlInjectionProtector.ReadJsonElementValues(objdata) || !SqlInjectionProtector.ReadJsonElementValuesScript(objdata))
                {
                    var errorResponse = new { response = false, responseCode = "02", data = "Invalid input detected." };
                    return new JsonResult(errorResponse) { StatusCode = 400 };
                }
                string readTokenValue = "";
                if (Request.Headers.TryGetValue("Authorization", out var authorizationHeader))
                {
                    readTokenValue = authorizationHeader.ToString();
                }

                Transaction obj = new Transaction();

                obj.AmountInGBP = data.sendingAmount;

                string receivingAmt = Convert.ToString(data.receivingAmount).Trim();

                obj.AmountInPKR = Convert.ToDouble(receivingAmt);  // data.receivingAmount  ;
                obj.Beneficiary_ID = data.beneficiaryID;
                obj.CB_ID = data.branchID;
                obj.Branch_ID = data.branchID;
                obj.Client_ID = data.clientID;
                obj.Customer_ID = data.customerID;
                obj.DeliveryType_Id = data.deliveryTypeID;
                obj.Exchange_Rate = data.exchangeRate;
                obj.FromCurrency_Code = data.fromCurrencyCode;
                obj.GCCAmountInGBP = data.gccAmountInGBP;
                obj.PaymentDepositType_ID = data.paymentDepositeTypeID;
                obj.PaymentType_ID = data.paymentTypeID;
                obj.Transfer_Fees = data.transferFees;
                obj.Username = data.username;
                readTokenValue = readTokenValue.Replace("Bearer ", "");
                obj.readTokenValue = readTokenValue;
                obj.checklimit_step = 1;

                Service.srvSendMoney srv = new Service.srvSendMoney();
                //DataTable dt = srv.CheckAllLimits(obj, context);
                DataTable dt = srv.CheckAllLimits(obj, context);
                dt = dt.AsEnumerable()
                        .Reverse()
                        .CopyToDataTable();

                _ = Task.Run(() => CompanyInfo.InsertrequestLogTracker("checklimitfirststep dt count : " + dt.Rows.Count, 0, 0, 0, 0, "checklimitfirststep", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext));

                if (dt != null && dt.Rows.Count > 0)
                {
                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_SUCCESS);
                    response.data = "success";
                    response.ObjData = dt;
                    response.ResponseCode = 0;

                    //1 : Primary Doc upload, 2 : Secondary(Addressproof) upload , 4 : Soource of fund upload, 5 : Restrict/Stop, 10 : Secondary upload with Yes No button , 11 : Wallet Balance 

                    dt.Columns.Add("settodocument", typeof(string));
                    dt.Columns.Add("showingmessage", typeof(string));
                    string message = "";
                    foreach (DataRow row in dt.Rows)
                    {
                        int settodocumentvalue = 0;
                        string showingmessage = "";

                        if (Convert.ToString(row["PrimaryIdmsg"]) != "")
                        {
                            settodocumentvalue = 5;
                            showingmessage = Convert.ToString(row["PrimaryIdmsg"]);
                        }
                        else if (row["BlacklistedFlag"] != DBNull.Value)
                        {
                            if (Convert.ToInt32(row["BlacklistedFlag"]) == 1)
                            {
                                DataTable dtcompanydetails = (DataTable)CompanyInfo.GetBaseCurrencywisebankdetails(obj.Client_ID, obj.FromCurrency_Code, obj.PaymentDepositType_ID, obj.DeliveryType_Id);
                                if (dtcompanydetails != null && dtcompanydetails.Rows.Count > 0)
                                {
                                    foreach (DataRow dr in dt.Rows)
                                    {
                                        if (dr["Company_Email"] != "" && dr["Company_Email"] != null && dr["Company_mobile"] != "" && dr["Company_mobile"] != null)
                                        {
                                            message = " You can call us at <a href='tel:" + dr["Company_mobile"] + "'>" + dr["Company_mobile"] + "</a> or " +
                            "send email to <a href='mailto:" + dr["Company_Email"] + "'>" + dr["Company_Email"] + "</a>. Thank You.";
                                        }
                                        else if (dr["Company_mobile"] != "" && dr["Company_mobile"] != null)
                                        {
                                            message = message + " You can call us at <a href='tel:" + dr["Company_mobile"] + "'>" + dr["Company_mobile"] + "</a>. Thank You.";
                                        }
                                        else if (dr["Company_Email"] != "" && dr["Company_Email"] != null)
                                        {
                                            message = message + " You can send email to <a href='mailto:" + dr["Company_Email"] + "'>" + dr["Company_Email"] + "</a>. Thank You.";
                                        }

                                    }
                                }

                                settodocumentvalue = 5;
                                showingmessage = Convert.ToString(row["Blacklistedmessage"]) + " " + message;
                            }
                        }
                        else
                        {
                            Beneficiary objTranLimit = JsonConvert.DeserializeObject<Beneficiary>(jsonData);
                            objTranLimit.Client_ID = obj.Client_ID;
                            string beneficiaryCountry = "", beneficiaryCollectionType = "", foreignCurrencyCode = "";

                            try
                            {
                                objTranLimit.whereclause = data.whereclause;

                                beneficiaryCountry = data.beneficiaryCountry;

                                try
                                {
                                    beneficiaryCollectionType = Convert.ToString(data.beneficiaryCollectionType).Trim();
                                }
                                catch (Exception ex) { beneficiaryCollectionType = ""; }
                                foreignCurrencyCode = data.foreignCurrencyCode;
                            }
                            catch (Exception egx)
                            {

                            }


                            Service.srvBeneficiary srvTranLimit = new Service.srvBeneficiary(context);
                            DataTable[] dtTranLimit = new DataTable[1];
                            dtTranLimit[0] = srvTranLimit.GetConfig(objTranLimit);
                            if (dtTranLimit[0] != null && (beneficiaryCollectionType != "" && beneficiaryCountry != ""))
                            {
                                try
                                {
                                    foreach (DataRow rowv in dtTranLimit[0].Rows)
                                    {
                                        if (Convert.ToDouble(rowv["Trans_limit_man"]) == 0)
                                        {
                                            double current_amt = Convert.ToDouble(obj.AmountInPKR);

                                            if (current_amt <= Convert.ToDouble(rowv["Trans_limit_min"]))
                                            {
                                                settodocumentvalue = 5;
                                                showingmessage = foreignCurrencyCode + " " + Convert.ToString(rowv["Trans_limit_min"]) + " minimum limit applies for " + beneficiaryCollectionType + " for " + beneficiaryCountry;
                                            }

                                            if (current_amt <= Convert.ToDouble(rowv["Trans_limit_max"]))
                                            {
                                            }
                                            else
                                            {
                                                settodocumentvalue = 5;
                                                showingmessage = foreignCurrencyCode + " " + Convert.ToString(rowv["Trans_limit_max"]) + " maximum limit applies for " + beneficiaryCollectionType + " for " + beneficiaryCountry;
                                            }

                                            if (Convert.ToString(rowv["Trans_lmt_perday"]) != null && Convert.ToString(rowv["Trans_lmt_perday"]) != "null" && Convert.ToString(rowv["Trans_lmt_perday"]) != "")
                                            {   // Transaction limit per day
                                                if ((current_amt + Convert.ToDouble(row["TotalCustAmount1"])) >= Convert.ToDouble(rowv["Trans_lmt_perday"]))
                                                {
                                                    settodocumentvalue = 5;
                                                    showingmessage = foreignCurrencyCode + " " + Convert.ToDouble(rowv["Trans_lmt_perday"]).ToString() + " daily limit applies for " + beneficiaryCollectionType + " for " + beneficiaryCountry;
                                                }
                                            }
                                        }
                                    }
                                }
                                catch (Exception egxx)
                                {
                                    _ = Task.Run(() => CompanyInfo.InsertrequestLogTracker("CheckLimit Error: " + egxx.ToString(), 0, 0, 0, 0, "checklimit", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext));
                                }

                            }
                        }

                        try
                        {
                            if (Convert.ToInt32(row["SourceOfFunds_Limit"]) == 1)
                            {
                                settodocumentvalue = 4;
                                showingmessage = "Source of fund require";
                                showingmessage = Convert.ToString(row["IDUploadmsg"]);
                            }
                            else if (Convert.ToInt32(row["PrimaryID_Limit"]) == 1)
                            {
                                settodocumentvalue = 1;
                                showingmessage = "Primary document require";
                                showingmessage = Convert.ToString(row["IDUploadmsg"]);
                            }
                            else if (Convert.ToInt32(row["SecondaryID_Limit"]) == 1)
                            {
                                settodocumentvalue = 2;
                                showingmessage = "Secondary document require";
                                showingmessage = Convert.ToString(row["IDUploadmsg"]);
                            }

                            if (Convert.ToInt32(row["monlimit"]) == 1 )
                            {
                                settodocumentvalue = 5;
                                showingmessage = Convert.ToString(row["monthlylimitmsg"]);
                            }
                            /*else if (Convert.ToInt32(row["monlimit"]) == 2 )
                            {
                                settodocumentvalue = 4;
                                showingmessage = Convert.ToString(row["monthlylimitmsg"]);
                            }*/

                            if (Convert.ToInt32(row["dailylimit"]) == 1)
                            {
                                settodocumentvalue = 4;
                                showingmessage = Convert.ToString(row["Daily_Limit_Message"]);
                            }
                            else if (Convert.ToInt32(row["dailylimit"]) == 2 )
                            {
                                settodocumentvalue = 5;
                                showingmessage = Convert.ToString(row["Daily_Limit_Message"]);
                            }

                            if (Convert.ToInt32(row["AMLlimit"]) == 4)
                            {
                                settodocumentvalue = 4;
                                showingmessage = Convert.ToString(row["amlLimitmsg"]);
                            }
                            else if (Convert.ToInt32(row["AMLlimit"]) == 2 )
                            {
                                settodocumentvalue = 2;
                                showingmessage = Convert.ToString(row["amlLimitmsg"]);
                            }
                            else if (Convert.ToInt32(row["AMLlimit"]) == 1)
                            {
                                settodocumentvalue = 1;
                                showingmessage = Convert.ToString(row["amlLimitmsg"]);
                            }
                            else if (Convert.ToInt32(row["AMLlimit"]) == 10  )
                            {
                                settodocumentvalue = 10;
                                showingmessage = Convert.ToString(row["AMLlimitmsg"]);
                            }
                            else if (Convert.ToInt32(row["payWithWallet"]) == 1)
                            {
                                settodocumentvalue = 11;
                                showingmessage = "Your wallet balance is insufficient. Please add more funds to the wallet and try again.";
                            }
                            else if (Convert.ToInt32(row["IsValidID"]) == 1)
                            {
                                settodocumentvalue = 1;
                                showingmessage = Convert.ToString(row["IDUploadmsg"]);
                            }
                            else if (Convert.ToInt32(row["IsValidID"]) == 2)
                            {
                                settodocumentvalue = 2;
                                showingmessage = Convert.ToString(row["IDUploadmsg"]);
                            }
                            else if (Convert.ToInt32(row["daily_transfer_count"]) > 0)
                            {
                                settodocumentvalue = 5;
                                showingmessage = Convert.ToString(row["daily_transfer_msg"]);
                            }
                            else if (Convert.ToInt32(row["SOFDaysCount"]) > 0)
                            {
                                settodocumentvalue = 5;
                                showingmessage = Convert.ToString(row["SOFDaysCount_Msg"]);
                            }
                            else if (Convert.ToInt32(row["AllowProceedFlag"]) > 0)
                            {
                                settodocumentvalue = 5;
                                showingmessage = Convert.ToString(row["AllowProceedFlag_Msg"]);
                            }
                            /*else if (Convert.ToInt32(row["paymenttypelimit"]) == 1)
                            {
                                settodocumentvalue = 5;
                                showingmessage = Convert.ToString(row["paymenttypelimitmsg"]);
                            }
                            else if (Convert.ToInt32(row["collectiontypelimit"]) == 1)
                            {
                                settodocumentvalue = 5;
                                showingmessage = Convert.ToString(row["collectiontypelimitmsg"]);
                            }*/
                            else if (Convert.ToInt32(row["Custlimit"]) == 1)
                            {
                                settodocumentvalue = 4;
                                showingmessage = Convert.ToString(row["Custlimitmsg"]);
                            }
                        }
                        catch (Exception egxx)
                        {
                            _ = Task.Run(() => CompanyInfo.InsertrequestLogTracker("CheckLimitfirststep Error2: " + egxx.ToString(), 0, 0, 0, 0, "CheckLimitfirststep", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext));
                        }

                        row["settodocument"] = settodocumentvalue;
                        row["showingmessage"] = showingmessage;
                        break;
                    }

                    if (dt != null && dt.Rows.Count > 0)
                    {
                        dt.Columns["Status"].ColumnName = "status";
                        dt.Columns["Errormessage"].ColumnName = "errorMessage";
                        dt.Columns["BlacklistedFlag"].ColumnName = "blackListedFlag";
                        dt.Columns["Blacklistedmessage"].ColumnName = "blackListedMessage";
                        dt.Columns["SOFDaysCount"].ColumnName = "sofDaysCount";
                        dt.Columns["SOFDaysCount_Msg"].ColumnName = "sofDaysCountMessage";
                        dt.Columns["AMLlimit"].ColumnName = "amlLimit";
                        dt.Columns["AMLlimitmsg"].ColumnName = "amlLimitMessage";
                        dt.Columns["SourceOfFunds_Limit"].ColumnName = "sourceOfFundLimit";
                        dt.Columns["PrimaryID_Limit"].ColumnName = "primaryIDLimit";
                        dt.Columns["SecondaryID_Limit"].ColumnName = "secondaryIDLimit";
                        dt.Columns["Custlimit"].ColumnName = "custLimit";
                        dt.Columns["Custlimitmsg"].ColumnName = "custLimitMesaage";
                        dt.Columns["daily_transfer_count"].ColumnName = "dailyTransferCount";
                        dt.Columns["daily_transfer_msg"].ColumnName = "dailyTransferMessage";
                        dt.Columns["dailylimit"].ColumnName = "dailyLimit";
                        dt.Columns["Daily_Limit_Message"].ColumnName = "dailyLimitMessage";
                        dt.Columns["TotalCustAmount"].ColumnName = "totalCustAmount";
                        dt.Columns["paymenttypelimit"].ColumnName = "paymentTypeLimit";
                        dt.Columns["paymenttypelimitmsg"].ColumnName = "paymentTypeLimitMessage";
                        dt.Columns["collectiontypelimit"].ColumnName = "collectionTypeLimit";
                        dt.Columns["IsValidID"].ColumnName = "isValidID";
                        dt.Columns["IDUploadmsg"].ColumnName = "idUploadMessage";
                        dt.Columns["AllowProceedFlag"].ColumnName = "allowProceedFlag";
                        dt.Columns["AllowProceedFlag_Msg"].ColumnName = "allowProceedFlagMessage";
                        dt.Columns["TotalBenfAmount"].ColumnName = "totlaBeneficiaryAmount";
                        dt.Columns["TotalCustAmount1"].ColumnName = "totalcustomerAmount";
                        dt.Columns["TotalBenfAmtYr"].ColumnName = "totalBeneficiaryAmountYear";
                        dt.Columns["transfer_count_days"].ColumnName = "transferCountDays";
                        dt.Columns["paywithwallet"].ColumnName = "payWithWallet";
                        dt.Columns["PrimaryIdmsg"].ColumnName = "primaryIDMessage";
                    }

                    var relationshipData = dt.Rows.OfType<DataRow>()
            .Select(row => dt.Columns.OfType<DataColumn>()
                .ToDictionary(col => col.ColumnName, c => row[c].ToString()));
                    List<Dictionary<string, object>> Transaction = new List<Dictionary<string, object>>();

                    returnJsonData = new { response = true, responseCode = "00", data = relationshipData };
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
                Model.ErrorLog objError = new Model.ErrorLog();
                objError.User = new Model.User();
                objError.Branch = new Model.Branch();
                objError.Client = new Model.Client();

                objError.Error = "Api : CheckLimitfirststep --" + ex.ToString();
                objError.Branch_ID = 1;
                objError.Date = DateTime.Now;
                objError.User_ID = 1;
                objError.Id = 1;
                objError.Function_Name = "CheckLimitfirststep";
                Service.srvErrorLog srvError = new Service.srvErrorLog();
                srvError.Create(objError, HttpContext);
                returnJsonData = new { response = false, responseCode = "01", data = "Invalid request." };
                return new JsonResult(returnJsonData);
            }
            return new JsonResult(returnJsonData);
        }



        [HttpPost]
        [Authorize]
        [Route("checklimit")]
        public JsonResult CheckLimit([FromBody] JsonElement objdata)
        {
            HttpContext context = HttpContext;
            List<Model.Transaction> _lst = new List<Model.Transaction>();
            var returnJsonData = (dynamic)null;
            Model.response.WebResponse response = null;
            string jsonData = System.Text.Json.JsonSerializer.Serialize(objdata);
            dynamic data = JObject.Parse(jsonData);
            _ = Task.Run(() => CompanyInfo.InsertrequestLogTracker("CheckLimit full request body: " + JObject.Parse(jsonData), 0, 0, 0, 0, "checklimit", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext));
            try
            {
                if (!SqlInjectionProtector.ReadJsonElementValues(objdata) || !SqlInjectionProtector.ReadJsonElementValuesScript(objdata))
                {
                    var errorResponse = new { response = false, responseCode = "02", data = "Invalid input detected." };
                    return new JsonResult(errorResponse) { StatusCode = 400 };
                }
                string readTokenValue = "";
                if (Request.Headers.TryGetValue("Authorization", out var authorizationHeader))
                {                   
                     readTokenValue = authorizationHeader.ToString();
                }


                #region validateData
               /* try
                {
                    if (data.sendingAmount == "" || data.sendingAmount == null)
                    {
                        returnJsonData = new { response = false, responseCode = "02", data = "Invalid Sending Amount." };
                        return new JsonResult(returnJsonData);
                    }
                    else if (data.receivingAmount == "" || data.receivingAmount == null)
                    {
                        returnJsonData = new { response = false, responseCode = "02", data = "Invalid Receiving Amount." };
                        return new JsonResult(returnJsonData);
                    }
                    else if (data.beneficiaryID == "" || data.beneficiaryID == null)
                    {
                        returnJsonData = new { response = false, responseCode = "02", data = "Invalid Beneficiary ID." };
                        return new JsonResult(returnJsonData);
                    } 
                    else if (data.branchID == "" || data.branchID == null)
                    {
                        returnJsonData = new { response = false, responseCode = "02", data = "Invalid Branch ID." };
                        return new JsonResult(returnJsonData);
                    } 
                    else if (data.clientID == "" || data.clientID == null)
                    {
                        returnJsonData = new { response = false, responseCode = "02", data = "Invalid Client ID." };
                        return new JsonResult(returnJsonData);
                    } 
                    else if (data.customerID == "" || data.customerID == null)
                    {
                        returnJsonData = new { response = false, responseCode = "02", data = "Invalid Customer ID." };
                        return new JsonResult(returnJsonData);
                    } 
                    else if (data.deliveryTypeID == "" || data.deliveryTypeID == null)
                    {
                        returnJsonData = new { response = false, responseCode = "02", data = "Invalid Delivery Type ID." };
                        return new JsonResult(returnJsonData);
                    }
                    else if (data.exchangeRate == "" || data.exchangeRate == null)
                    {
                        returnJsonData = new { response = false, responseCode = "02", data = "Invalid Exchange Rate." };
                        return new JsonResult(returnJsonData);
                    } 
                    else if (data.fromCurrencyCode == "" || data.fromCurrencyCode == null)
                    {
                        returnJsonData = new { response = false, responseCode = "02", data = "Invalid From Currency Code." };
                        return new JsonResult(returnJsonData);
                    } 
                    else if (data.gccAmountInGBP == "" || data.gccAmountInGBP == null)
                    {
                        returnJsonData = new { response = false, responseCode = "02", data = "Invalid Gcc Amount In GBP." };
                        return new JsonResult(returnJsonData);
                    } 
                    else if (data.paymentDepositeTypeID == "" || data.paymentDepositeTypeID == null)
                    {
                        returnJsonData = new { response = false, responseCode = "02", data = "Invalid Payment Deposite Type ID." };
                        return new JsonResult(returnJsonData);
                    } 
                    else if (data.paymentTypeID == "" || data.paymentTypeID == null)
                    {
                        returnJsonData = new { response = false, responseCode = "02", data = "Invalid Payment Type ID." };
                        return new JsonResult(returnJsonData);
                    }
                    else if (data.transferFees == "" || data.transferFees == null)
                    {
                        returnJsonData = new { response = false, responseCode = "02", data = "Invalid Transfer Fees." };
                        return new JsonResult(returnJsonData);
                    } 
                    else if (data.username == "" || data.username == null)
                    {
                        returnJsonData = new { response = false, responseCode = "02", data = "Invalid Username." };
                        return new JsonResult(returnJsonData);
                    }
                }
                catch (Exception ex)
                {
                    Model.ActivityLogTracker _objActivityLog = new Model.ActivityLogTracker();
                    _objActivityLog.Activity = " Exception Found CheckLimit Error: " + ex.ToString() + " ";
                    _objActivityLog.FunctionName = "CheckLimit";
                    _objActivityLog.Transaction_ID = 0;
                    _objActivityLog.WhoAcessed = 1;
                    _objActivityLog.Branch_ID = 0;
                    _objActivityLog.Client_ID = 0;

                    Service.srvActivityLogTracker srvActivityLog = new Service.srvActivityLogTracker();
                    int _i = srvActivityLog.Create(_objActivityLog, HttpContext);

                    returnJsonData = new { response = false, responseCode = "01", data = "Field is missing." };
                    return new JsonResult(returnJsonData);
                }*/
                #endregion

                Transaction obj = new Transaction();

                obj.AmountInGBP = data.sendingAmount;

                string receivingAmt = Convert.ToString(data.receivingAmount).Trim();

                obj.AmountInPKR = Convert.ToDouble(receivingAmt);  // data.receivingAmount  ;
                obj.Beneficiary_ID = data.beneficiaryID;
                obj.CB_ID = data.branchID;
                obj.Branch_ID = data.branchID;
                obj.Client_ID = data.clientID;
                obj.Customer_ID = data.customerID;
                obj.DeliveryType_Id = data.deliveryTypeID;
                obj.Exchange_Rate = data.exchangeRate;
                obj.FromCurrency_Code = data.fromCurrencyCode;
                obj.GCCAmountInGBP = data.gccAmountInGBP;
                obj.PaymentDepositType_ID = data.paymentDepositeTypeID;
                obj.PaymentType_ID = data.paymentTypeID;
                obj.Transfer_Fees = data.transferFees;
                obj.Username = data.username;
                readTokenValue = readTokenValue.Replace("Bearer ", "");
                obj.readTokenValue =  readTokenValue;
                

                obj.checklimit_step = 0;

                Service.srvSendMoney srv = new Service.srvSendMoney();
                //DataTable dt = srv.CheckAllLimits(obj, context);
                DataTable dt = srv.CheckAllLimits(obj, context);
                dt = dt.AsEnumerable()
                        .Reverse()
                        .CopyToDataTable();


                if (dt != null && dt.Rows.Count > 0)
                {
                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_SUCCESS);
                    response.data = "success";
                    response.ObjData = dt;
                    response.ResponseCode = 0;

//1 : Primary Doc upload, 2 : Secondary(Addressproof) upload , 4 : Soource of fund upload, 5 : Restrict/Stop, 10 : Secondary upload with Yes No button , 11 : Wallet Balance 

                    dt.Columns.Add("settodocument", typeof(string));
                    dt.Columns.Add("showingmessage", typeof(string));
                    string message = "";
                    foreach (DataRow row in dt.Rows)
                    {
                        int settodocumentvalue = 0;
                        string showingmessage = "";

                        if (Convert.ToString(row["PrimaryIdmsg"]) != "")
                        {
                            settodocumentvalue = 5;                            
                            showingmessage = Convert.ToString(row["PrimaryIdmsg"]);
                        }
                        else if (row["BlacklistedFlag"] != DBNull.Value)
                        {
                            if (Convert.ToInt32(row["BlacklistedFlag"]) == 1) {
                                DataTable dtcompanydetails = (DataTable)CompanyInfo.GetBaseCurrencywisebankdetails(obj.Client_ID, obj.FromCurrency_Code, obj.PaymentDepositType_ID, obj.DeliveryType_Id);
                                if (dtcompanydetails != null && dtcompanydetails.Rows.Count > 0)
                                {
                                    foreach (DataRow dr in dt.Rows)
                                    {
                                        if (dr["Company_Email"] != "" && dr["Company_Email"] != null && dr["Company_mobile"] != "" && dr["Company_mobile"] != null)
                                        {
                                            message = " You can call us at <a href='tel:" + dr["Company_mobile"] + "'>" + dr["Company_mobile"] + "</a> or " +
                            "send email to <a href='mailto:" + dr["Company_Email"] + "'>" + dr["Company_Email"] + "</a>. Thank You.";
                                        }
                                        else if (dr["Company_mobile"] != "" && dr["Company_mobile"] != null)
                                        {
                                            message = message + " You can call us at <a href='tel:" + dr["Company_mobile"] + "'>" + dr["Company_mobile"] + "</a>. Thank You.";
                                        }
                                        else if (dr["Company_Email"] != "" && dr["Company_Email"] != null)
                                        {
                                            message = message + " You can send email to <a href='mailto:" + dr["Company_Email"] + "'>" + dr["Company_Email"] + "</a>. Thank You.";
                                        }

                                    }
                                }

                                settodocumentvalue = 5;
                                showingmessage = Convert.ToString(row["Blacklistedmessage"]) + " " + message;
                            }
                        }
                        else
                        {
                            Beneficiary objTranLimit = JsonConvert.DeserializeObject<Beneficiary>(jsonData);
                            objTranLimit.Client_ID = obj.Client_ID;
                            string beneficiaryCountry ="", beneficiaryCollectionType ="", foreignCurrencyCode = "";

                            try
                            {
                                objTranLimit.whereclause = data.whereclause;

                                beneficiaryCountry = data.beneficiaryCountry;

                                try
                                {
                                    beneficiaryCollectionType = Convert.ToString(data.beneficiaryCollectionType).Trim();
                                }
                                catch (Exception ex) { beneficiaryCollectionType = ""; }
                                foreignCurrencyCode = data.foreignCurrencyCode;
                            }
                            catch (Exception egx)
                            {

                            }


                            Service.srvBeneficiary srvTranLimit = new Service.srvBeneficiary(context);
                            DataTable[] dtTranLimit = new DataTable[1];
                            dtTranLimit[0] = srvTranLimit.GetConfig(objTranLimit);
                            if (dtTranLimit[0] != null && (beneficiaryCollectionType != "" && beneficiaryCountry != ""))
                            {
                                try
                                {
                                    foreach (DataRow rowv in dtTranLimit[0].Rows)
                                    {
                                        if (Convert.ToDouble(rowv["Trans_limit_man"]) == 0)
                                        {
                                            double current_amt = Convert.ToDouble(obj.AmountInPKR);

                                            if (current_amt <= Convert.ToDouble(rowv["Trans_limit_min"]))
                                            {
                                                settodocumentvalue = 5;
                                                showingmessage = foreignCurrencyCode + " " + Convert.ToString(rowv["Trans_limit_min"]) + " minimum limit applies for " + beneficiaryCollectionType + " for " + beneficiaryCountry;
                                            }

                                            if (current_amt <= Convert.ToDouble(rowv["Trans_limit_max"]))
                                            {
                                            }
                                            else
                                            {
                                                settodocumentvalue = 5;
                                                showingmessage = foreignCurrencyCode + " " + Convert.ToString(rowv["Trans_limit_max"]) + " maximum limit applies for " + beneficiaryCollectionType + " for " + beneficiaryCountry;
                                            }

                                            if (Convert.ToString(rowv["Trans_lmt_perday"]) != null && Convert.ToString(rowv["Trans_lmt_perday"]) != "null" && Convert.ToString(rowv["Trans_lmt_perday"]) != "")
                                            {   // Transaction limit per day
                                                if ((current_amt + Convert.ToDouble(row["TotalCustAmount1"])) >= Convert.ToDouble(rowv["Trans_lmt_perday"]))
                                                {
                                                    settodocumentvalue = 5;
                                                    showingmessage = foreignCurrencyCode + " " + Convert.ToDouble(rowv["Trans_lmt_perday"]).ToString() + " daily limit applies for " + beneficiaryCollectionType + " for " + beneficiaryCountry;
                                                }
                                            }

                                            if (Convert.ToString(rowv["Trans_lmtt_perday_benf"]) != null && Convert.ToString(rowv["Trans_lmtt_perday_benf"]) != "null" && Convert.ToString(rowv["Trans_lmtt_perday_benf"]) != "")
                                            {   // Transaction limit per day for beneficiary
                                                if ((current_amt + Convert.ToDouble(row["TotalBenfAmount"])) >= Convert.ToDouble(rowv["Trans_lmtt_perday_benf"]))
                                                {
                                                    settodocumentvalue = 5;
                                                    showingmessage = foreignCurrencyCode + " " + Convert.ToDouble(rowv["Trans_lmtt_perday_benf"]).ToString() + " daily limit applies to this beneficiary for " + beneficiaryCollectionType + " for " + beneficiaryCountry;
                                                }
                                            }

                                            if (Convert.ToString(rowv["Trans_lmt_peryear_benf"]) != null && Convert.ToString(rowv["Trans_lmt_peryear_benf"]) != "null" && Convert.ToString(rowv["Trans_lmt_peryear_benf"]) != "")
                                            {   // Transaction limit per day for beneficiary
                                                if ((current_amt + Convert.ToDouble(row["TotalBenfAmtYr"])) >= Convert.ToDouble(rowv["Trans_lmt_peryear_benf"]))
                                                {
                                                    settodocumentvalue = 5;
                                                    showingmessage = foreignCurrencyCode + " " + Convert.ToDouble(rowv["Trans_lmt_peryear_benf"]).ToString() + " yearly limit applies to this beneficiary for " + beneficiaryCollectionType + " for " + beneficiaryCountry;
                                                }
                                            }
                                        }

                                    }
                                }
                                catch (Exception egxx) {
                                    _ = Task.Run(() => CompanyInfo.InsertrequestLogTracker("CheckLimit Error: " + egxx.ToString(), 0, 0, 0, 0, "checklimit", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext));
                                }

                            }
                        }

                        try
                        {
                            if ( Convert.ToInt32(row["SourceOfFunds_Limit"]) == 1)
                        {
                            settodocumentvalue = 4;
                            showingmessage = "Source of fund require"; 
                                showingmessage = Convert.ToString(row["IDUploadmsg"]);
                        }
                        else if (Convert.ToInt32(row["PrimaryID_Limit"]) == 1)
                        {
                            settodocumentvalue = 1;
                            showingmessage = "Primary document require";
                            showingmessage = Convert.ToString(row["IDUploadmsg"]);
                        }
                        else if (Convert.ToInt32(row["SecondaryID_Limit"]) == 1)
                        {
                            settodocumentvalue = 2;
                            showingmessage = "Secondary document require";
                            showingmessage = Convert.ToString(row["IDUploadmsg"]);
                        }

                        if (Convert.ToInt32(row["monlimit"]) == 1)
                        {
                            settodocumentvalue = 5;
                            showingmessage = Convert.ToString(row["monthlylimitmsg"]);
                        }
                        else if (Convert.ToInt32(row["monlimit"]) == 2)
                        {
                            settodocumentvalue = 4;
                            showingmessage = Convert.ToString(row["monthlylimitmsg"]);
                        }

                        if (Convert.ToInt32(row["dailylimit"]) == 1)
                        {
                            settodocumentvalue = 4;
                            showingmessage = Convert.ToString(row["Daily_Limit_Message"]);
                        }
                        else if (Convert.ToInt32(row["dailylimit"]) == 2)
                        {
                            settodocumentvalue = 5;
                            showingmessage = Convert.ToString(row["Daily_Limit_Message"]);
                        }

                        if (Convert.ToInt32(row["AMLlimit"]) == 4 )
                        {
                            settodocumentvalue = 4;
                            showingmessage = Convert.ToString(row["amlLimitmsg"]);
                        }
                        else if (Convert.ToInt32(row["AMLlimit"]) == 2 )
                        {
                            settodocumentvalue = 2;
                            showingmessage = Convert.ToString(row["amlLimitmsg"]);
                        }
                        else if (Convert.ToInt32(row["AMLlimit"]) == 1)
                        {
                            settodocumentvalue = 1;
                            showingmessage = Convert.ToString(row["amlLimitmsg"]);
                        }
                        else if (Convert.ToInt32(row["AMLlimit"]) == 10 )
                        {
                            settodocumentvalue = 10;
                            showingmessage = Convert.ToString(row["AMLlimitmsg"]);
                        }
                        else if (Convert.ToInt32(row["payWithWallet"]) == 1)
                        {
                            settodocumentvalue = 11;
                            showingmessage = "Your wallet balance is insufficient. Please add more funds to the wallet and try again.";
                        }
                        else if (Convert.ToInt32(row["IsValidID"]) == 1)
                        {
                            settodocumentvalue = 1;
                            showingmessage = Convert.ToString(row["IDUploadmsg"]);
                        }
                            else if (Convert.ToInt32(row["IsValidID"]) == 2)
                            {
                                settodocumentvalue = 2;
                                showingmessage = Convert.ToString(row["IDUploadmsg"]);
                            }
                            else if (Convert.ToInt32(row["daily_transfer_count"]) > 0)
                        {
                            settodocumentvalue = 5;
                            showingmessage = Convert.ToString(row["daily_transfer_msg"]);
                        }
                        else if (Convert.ToInt32(row["SOFDaysCount"]) > 0)
                        {
                            settodocumentvalue = 5;
                            showingmessage = Convert.ToString(row["SOFDaysCount_Msg"]);
                        }
                        else if (Convert.ToInt32(row["AllowProceedFlag"]) > 0)
                        {
                            settodocumentvalue = 5;
                            showingmessage = Convert.ToString(row["AllowProceedFlag_Msg"]);
                        }
                        else if (Convert.ToInt32(row["paymenttypelimit"]) == 1  )
                        {
                            settodocumentvalue = 5;
                            showingmessage = Convert.ToString(row["paymenttypelimitmsg"]);
                        }
                        else if (Convert.ToInt32(row["collectiontypelimit"]) == 1 )
                        {
                            settodocumentvalue = 5;
                            showingmessage = Convert.ToString(row["collectiontypelimitmsg"]);
                        }
                        else if (Convert.ToInt32(row["Custlimit"]) == 1 )
                        {
                            settodocumentvalue = 4;
                            showingmessage = Convert.ToString(row["Custlimitmsg"]);
                        }
                        }
                        catch (Exception egxx)
                        {
                            _ = Task.Run(() => CompanyInfo.InsertrequestLogTracker("CheckLimit Error2: " + egxx.ToString(), 0, 0, 0, 0, "checklimit", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext));
                        }

                        row["settodocument"] = settodocumentvalue;
                        row["showingmessage"] = showingmessage;
                        break;
                    }

                    if (dt != null && dt.Rows.Count > 0)
                    {
                        dt.Columns["Status"].ColumnName = "status";
                        dt.Columns["Errormessage"].ColumnName = "errorMessage";
                        dt.Columns["BlacklistedFlag"].ColumnName = "blackListedFlag";
                        dt.Columns["Blacklistedmessage"].ColumnName = "blackListedMessage";
                        dt.Columns["SOFDaysCount"].ColumnName = "sofDaysCount";
                        dt.Columns["SOFDaysCount_Msg"].ColumnName = "sofDaysCountMessage";
                        dt.Columns["AMLlimit"].ColumnName = "amlLimit";
                        dt.Columns["AMLlimitmsg"].ColumnName = "amlLimitMessage";
                        dt.Columns["SourceOfFunds_Limit"].ColumnName = "sourceOfFundLimit";
                        dt.Columns["PrimaryID_Limit"].ColumnName = "primaryIDLimit";
                        dt.Columns["SecondaryID_Limit"].ColumnName = "secondaryIDLimit";
                        dt.Columns["Custlimit"].ColumnName = "custLimit";
                        dt.Columns["Custlimitmsg"].ColumnName = "custLimitMesaage";
                        dt.Columns["daily_transfer_count"].ColumnName = "dailyTransferCount";
                        dt.Columns["daily_transfer_msg"].ColumnName = "dailyTransferMessage";
                        dt.Columns["dailylimit"].ColumnName = "dailyLimit";
                        dt.Columns["Daily_Limit_Message"].ColumnName = "dailyLimitMessage";
                        dt.Columns["TotalCustAmount"].ColumnName = "totalCustAmount";
                        dt.Columns["paymenttypelimit"].ColumnName = "paymentTypeLimit";
                        dt.Columns["paymenttypelimitmsg"].ColumnName = "paymentTypeLimitMessage";
                        dt.Columns["collectiontypelimit"].ColumnName = "collectionTypeLimit";
                        dt.Columns["IsValidID"].ColumnName = "isValidID";
                        dt.Columns["IDUploadmsg"].ColumnName = "idUploadMessage";
                        dt.Columns["AllowProceedFlag"].ColumnName = "allowProceedFlag";
                        dt.Columns["AllowProceedFlag_Msg"].ColumnName = "allowProceedFlagMessage";
                        dt.Columns["TotalBenfAmount"].ColumnName = "totlaBeneficiaryAmount";
                        dt.Columns["TotalCustAmount1"].ColumnName = "totalcustomerAmount";
                        dt.Columns["TotalBenfAmtYr"].ColumnName = "totalBeneficiaryAmountYear";
                        dt.Columns["transfer_count_days"].ColumnName = "transferCountDays";
                        dt.Columns["paywithwallet"].ColumnName = "payWithWallet";
                        dt.Columns["PrimaryIdmsg"].ColumnName = "primaryIDMessage";
                    }

                        var relationshipData = dt.Rows.OfType<DataRow>()
                .Select(row => dt.Columns.OfType<DataColumn>()
                    .ToDictionary(col => col.ColumnName, c => row[c].ToString()));
                    List<Dictionary<string, object>> Transaction = new List<Dictionary<string, object>>();

                    returnJsonData = new { response = true, responseCode = "00", data = relationshipData };
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
                Model.ErrorLog objError = new Model.ErrorLog();
                objError.User = new Model.User();
                objError.Branch = new Model.Branch();
                objError.Client = new Model.Client();

                objError.Error = "Api : CheckLimit --" + ex.ToString();
                objError.Branch_ID = 1;
                objError.Date = DateTime.Now;
                objError.User_ID = 1;
                objError.Id = 1;
                objError.Function_Name = "CheckLimit";
                Service.srvErrorLog srvError = new Service.srvErrorLog();
                srvError.Create(objError, HttpContext);
                returnJsonData = new { response = false, responseCode = "01", data = "Invalid request." };
                return new JsonResult(returnJsonData);
            }
            return new JsonResult(returnJsonData);
        }


        [HttpPost]
        [Route("selectbankdetails")]
        public JsonResult SelectBankDetails([FromBody] JsonElement objdata) //Anushka collection and delivery type wise 
        {
            var returnJsonData = (dynamic)null;
            HttpContext context = null;
            Model.response.WebResponse response = null;
            string jsonData = System.Text.Json.JsonSerializer.Serialize(objdata);
            dynamic data = JObject.Parse(jsonData);
            _ = Task.Run(() => CompanyInfo.InsertrequestLogTracker("SelectBankDetails full request body: " + JObject.Parse(jsonData), 0, 0, 0, 0, "SelectBankDetails", Convert.ToInt32(0), Convert.ToInt32(0), "", context));
            try
            {
                Transaction obj = new Transaction();
                Service.srvViewTransferHistory srv = new Service.srvViewTransferHistory();
                obj.Client_ID = data.clientID;
                obj.Branch_ID = data.branchID;
                obj.basecurrency = data.baseCurrency;
                obj.DeliveryType_Id = data.deliveryTypeID;
                obj.PaymentDepositType_ID = data.paymentDepositTypeID;

                try
                {
                    int perm_status = 1;
                    MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("GetPermissions");
                    _cmd.CommandType = CommandType.StoredProcedure;
                    _cmd.Parameters.AddWithValue("_whereclause", " and PID = 158");
                    _cmd.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                    DataTable perm = db_connection.ExecuteQueryDataTableProcedure(_cmd);
                    if (perm.Rows.Count > 0)
                    {
                        perm_status = Convert.ToInt32(perm.Rows[0]["Status_ForCustomer"]);
                    }
                    if (perm_status == 1)
                    {
                        obj.DeliveryType_Id = 0;
                        obj.PaymentDepositType_ID = 0;
                    }
                }
                catch (Exception ex)
                {
                    //Error Log Handled
                    Model.ErrorLog objError = new Model.ErrorLog();
                    objError.User = new Model.User();
                    objError.Error = "Api : selectbankdetails --" + ex.ToString();
                    objError.Date = DateTime.Now;
                    objError.User_ID = 1;
                    objError.Client_ID = 1;
                    objError.Function_Name = "SelectBankDetails";
                    Service.srvErrorLog srvError = new Service.srvErrorLog();
                    srvError.Create(objError, context);
                }


                DataTable li1 = srv.SelectBankDetails(obj, context);
                //DataTable li1 = new DataTable();
                Model.Dashboard _Obj = new Model.Dashboard();
                if (li1 != null && li1.Rows.Count > 0)
                {
                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_SUCCESS);
                    response.data = "success";
                    var responseData = li1.Rows.OfType<DataRow>()
                        .Select(row => li1.Columns.OfType<DataColumn>()
                        .ToDictionary(col => col.ColumnName, col => row[col].ToString()))
                        .ToList();
                    response.ObjData = responseData;

                   
                    response.ResponseCode = 0;
                    returnJsonData = new { response = true, responseCode = "00", data = response.ObjData };
                }
                else
                {
                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_TECHNICAL_ERROR);
                    response.ResponseMessage = "Something went wrong. Please try again later.";
                    response.ResponseCode = 1;
                    returnJsonData = new { response = false, responseCode = "02", data = response.ResponseMessage };
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
                objError.Error = "Api : selectbankdetails --" + ex.ToString();
                objError.Date = DateTime.Now;
                objError.User_ID = 1;
                objError.Client_ID = 1;
                objError.Function_Name = "SelectBankDetails";
                Service.srvErrorLog srvError = new Service.srvErrorLog();
                srvError.Create(objError, context);
                returnJsonData = new { response = false, responseCode = "02", data = response.ResponseMessage };
            }
            //compress.GZipEncodePage();
            return new JsonResult(returnJsonData);
        }

        [HttpPost]
        [Route("selectbankdata")]
        public JsonResult Selectbankdata([FromBody] JsonElement objdata) //Anushka collection and delivery type wise 
        {
            var returnJsonData = (dynamic)null;
            HttpContext context = null;
            Model.response.WebResponse response = null;
            string jsonData = System.Text.Json.JsonSerializer.Serialize(objdata);
            dynamic data = JObject.Parse(jsonData);
            _ = Task.Run(() => CompanyInfo.InsertrequestLogTracker("Selectbankdata full request body: " + JObject.Parse(jsonData), 0, 0, 0, 0, "Selectbankdata", Convert.ToInt32(0), Convert.ToInt32(0), "", context));
            try
            {
                Transaction obj = new Transaction();
                Service.srvViewTransferHistory srv = new Service.srvViewTransferHistory();
                obj.Client_ID = data.clientID;
                DataTable li1 = srv.SelectBasecurrency(obj, context);
                //DataTable li1 = new DataTable();
                Model.Dashboard _Obj = new Model.Dashboard();
                if (li1 != null && li1.Rows.Count > 0)
                {

                    DataTable[] d = new DataTable[3];//**** Set Length here
                    d[0] = li1; d[0].TableName = "basecurrency";
                    Service.srvViewTransferHistory srv1 = new Service.srvViewTransferHistory();
                    DataTable li2 = srv1.SelectDeliveryType(obj, context);
                    d[1] = li2; d[1].TableName = "dType";
                    Service.srvViewTransferHistory srv3 = new Service.srvViewTransferHistory();
                    DataTable li3 = srv3.SelectcollectionType(obj, context);
                    d[2] = li3; d[1].TableName = "cType";

                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_SUCCESS);
                    response.data = "success";
                    /*response.ObjData = new
                    {
                        BaseCurrency = ConvertDataTableToList(d[0]),
                        DeliveryType = ConvertDataTableToList(d[1]),
                        CollectionType = ConvertDataTableToList(d[2])
                    };*/

                    var responseData_BaseCurrency = li1.Rows.OfType<DataRow>()
                    .Select(row => li1.Columns.OfType<DataColumn>()
                    .ToDictionary(col => col.ColumnName, col => row[col].ToString()))
                    .ToList();

                    var responseData_DeliveryType = li2.Rows.OfType<DataRow>()
                    .Select(row => li2.Columns.OfType<DataColumn>()
                    .ToDictionary(col => col.ColumnName, col => row[col].ToString()))
                    .ToList();

                    var responseData_CollectionType = li3.Rows.OfType<DataRow>()
                    .Select(row => li3.Columns.OfType<DataColumn>()
                    .ToDictionary(col => col.ColumnName, col => row[col].ToString()))
                    .ToList();

                    response.ObjData = new
                    {
                        BaseCurrency = responseData_BaseCurrency,
                        DeliveryType = responseData_DeliveryType,
                        CollectionType = responseData_CollectionType
                    };

                    response.ResponseCode = 0;
                    returnJsonData = new { response = true, responseCode = "00", data = response.ObjData };
                }
                else
                {
                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_TECHNICAL_ERROR);
                    response.ResponseMessage = "Something went wrong. Please try again later.";
                    response.ResponseCode = 1;
                    returnJsonData = new { response = false, responseCode = "02", data = response.ResponseMessage };
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
                objError.Error = "Api : selectbankdata --" + ex.ToString();
                objError.Date = DateTime.Now;
                objError.User_ID = 1;
                objError.Client_ID = 1;
                objError.Function_Name = "Selectbankdata";
                Service.srvErrorLog srvError = new Service.srvErrorLog();
                srvError.Create(objError, context);
                returnJsonData = new { response = false, responseCode = "02", data = "Something went wrong. Please try again later." };
            }
            //compress.GZipEncodePage();
            return new JsonResult(returnJsonData);
        }

        private List<Dictionary<string, object>> ConvertDataTableToList(DataTable dt)
        {
            var list = new List<Dictionary<string, object>>();
            foreach (DataRow row in dt.Rows)
            {
                var dict = new Dictionary<string, object>();
                foreach (DataColumn col in dt.Columns)
                {
                    dict[col.ColumnName] = row[col]; // Add each column-value pair
                }
                list.Add(dict);
            }
            return list;
        }

        [HttpPost]
        [Authorize]
        [Route("checkminmax")]
        public JsonResult CheckMinMax([FromBody] JsonElement objdata)
        {
            HttpContext context = HttpContext;
            List<Model.Transaction> _lst = new List<Model.Transaction>();
            var returnJsonData = (dynamic)null;
            Model.response.WebResponse response = null;
            string jsonData = System.Text.Json.JsonSerializer.Serialize(objdata);
            dynamic data = JObject.Parse(jsonData);
            CompanyInfo.InsertrequestLogTracker("CheckMinMax full request body: " + JObject.Parse(jsonData), 0, 0, 0, 0, "checkminmax", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
            try
            {
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
                    else if (data.customerID == "" || data.customerID == null)
                    {
                        returnJsonData = new { response = false, responseCode = "02", data = "Invalid Customer ID." };
                        return new JsonResult(returnJsonData);
                    }
                }
                catch (Exception ex)
                {
                    Model.ActivityLogTracker _objActivityLog = new Model.ActivityLogTracker();
                    _objActivityLog.Activity = " Exception Found CheckMinMax Error: " + ex.ToString() + " ";
                    _objActivityLog.FunctionName = "CheckMinMax";
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

                object o = JsonConvert.DeserializeObject(jsonData);
                bool checkinjection = CompanyInfo.checksqlinjectiondata(o);
                if (!checkinjection)
                {
                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_TECHNICAL_ERROR);
                    response.ResponseMessage = "Invalid Field Values.";
                    response.ResponseCode = 6;
                    returnJsonData = new { response = false, responseCode = "02", data = response.ResponseMessage };
                    return new JsonResult(returnJsonData);
                }

                Transaction obj = new Transaction();
                obj.Branch_ID = data.branchID;
                obj.Client_ID = data.clientID;
                obj.Customer_ID = data.customerID;
                

                Service.srvSendMoney srv = new Service.srvSendMoney();
                DataTable dt = srv.CheckMinMax(obj);
                if (dt != null && dt.Rows.Count > 0)
                {
                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_SUCCESS);
                    response.data = "success";
                    response.ObjData = dt;
                    response.ResponseCode = 0;
                    List<Dictionary<string, object>> Transaction = new List<Dictionary<string, object>>();

                    foreach (DataRow dr in dt.Rows)
                    {
                        Dictionary<string, object> wallet_balnce = new Dictionary<string, object>();
                        wallet_balnce["minimumTransferAmount"] = dr["Minimum_Transfer_Amount"];
                        wallet_balnce["maximumTransferAmount"] = dr["Maximum_Transfer_Amount"];
                        wallet_balnce["dailyTransferLimit"] = dr["Daily_Transfer_Limit"];
                        wallet_balnce["dailyTransferCount"] = dr["Daily_Transfer_Count"];
                        wallet_balnce["baseCurrencyID"] = dr["Base_currency_Id"];
                        Transaction.Add(wallet_balnce);
                    }

                    returnJsonData = new { response = true, responseCode = "00", data = Transaction };
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
                Model.ErrorLog objError = new Model.ErrorLog();
                objError.User = new Model.User();
                objError.Branch = new Model.Branch();
                objError.Client = new Model.Client();

                objError.Error = "Api : CheckMinMax --" + ex.ToString();
                objError.Branch_ID = 1;
                objError.Date = DateTime.Now;
                objError.User_ID = 1;
                objError.Id = 1;
                objError.Function_Name = "CheckMinMax";
                Service.srvErrorLog srvError = new Service.srvErrorLog();
                srvError.Create(objError, HttpContext);
                returnJsonData = new { response = false, responseCode = "01", data = "Invalid request." };
                return new JsonResult(returnJsonData);
            }
            return new JsonResult(returnJsonData);
        }

        [HttpPost]
        [Authorize]
        [Route("getstoreaddress")]
        public JsonResult getstoreaddress([FromBody] JsonElement objdata)
        {
            HttpContext context = HttpContext;
            List<Model.Transaction> _lst = new List<Model.Transaction>();
            var returnJsonData = (dynamic)null;
            Model.response.WebResponse response = null;
            string jsonData = System.Text.Json.JsonSerializer.Serialize(objdata);
            
            dynamic data = JObject.Parse(jsonData);
            CompanyInfo.InsertrequestLogTracker("getstoreaddress full request body: " + JObject.Parse(jsonData), 0, 0, 0, 0, "getstoreaddress", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
            try
            {
                if (!SqlInjectionProtector.ReadJsonElementValues(objdata) || !SqlInjectionProtector.ReadJsonElementValuesScript(objdata))
                {
                    var errorResponse = new { response = false, responseCode = "02", data = "Invalid input detected." };
                    return new JsonResult(errorResponse) { StatusCode = 400 };
                }

                Transaction Obj = JsonConvert.DeserializeObject<Transaction>(jsonData);
                Service.srvSendMoney srv = new Service.srvSendMoney();
                DataTable li1 = srv.GetStoreAddress(Obj);
                Model.Dashboard _Obj = new Model.Dashboard();

                if (li1 != null && li1.Rows.Count > 0)
                {
                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_SUCCESS);
                    response.data = "success";
                    response.ObjData = li1;
                    response.ResponseCode = 0;

                    var responseData = li1.Rows.OfType<DataRow>()
                .Select(row => li1.Columns.OfType<DataColumn>()
                    .ToDictionary(col => col.ColumnName, c => row[c].ToString()));
                    
                    returnJsonData = new { response = true, responseCode = "00", data = responseData };
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
                Model.ErrorLog objError = new Model.ErrorLog();
                objError.User = new Model.User();
                objError.Branch = new Model.Branch();
                objError.Client = new Model.Client();

                objError.Error = "Api : getstoreaddress --" + ex.ToString();
                objError.Branch_ID = 1;
                objError.Date = DateTime.Now;
                objError.User_ID = 1;
                objError.Id = 1;
                objError.Function_Name = "getstoreaddress";
                Service.srvErrorLog srvError = new Service.srvErrorLog();
                srvError.Create(objError, HttpContext);
                returnJsonData = new { response = false, responseCode = "01", data = "Invalid request." };
                return new JsonResult(returnJsonData);
            }            
        }

        [HttpPost]
        [Route("getpaysafebarcode")]
        public JsonResult getpaysafebarcode([FromBody] JsonElement objdata)
        {
            HttpContext context = HttpContext;
            List<Model.Transaction> _lst = new List<Model.Transaction>();
            var returnJsonData = (dynamic)null;
            Model.response.WebResponse response = null;
            string jsonData = System.Text.Json.JsonSerializer.Serialize(objdata);

            dynamic data = JObject.Parse(jsonData);
            CompanyInfo.InsertrequestLogTracker("getpaysafebarcode full request body: " + JObject.Parse(jsonData), 0, 0, 0, 0, "getpaysafebarcode", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
            try
            {
                if (!SqlInjectionProtector.ReadJsonElementValues(objdata) || !SqlInjectionProtector.ReadJsonElementValuesScript(objdata))
                {
                    var errorResponse = new { response = false, responseCode = "02", data = "Invalid input detected." };
                    return new JsonResult(errorResponse) { StatusCode = 400 };
                }

                Transaction Obj = JsonConvert.DeserializeObject<Transaction>(jsonData);
                Service.srvSendMoney srv = new Service.srvSendMoney();
                DataTable li1 = srv.GetPaysafeBarcode(Obj);
                Model.Dashboard _Obj = new Model.Dashboard();

                if (li1 != null && li1.Rows.Count > 0)
                {
                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_SUCCESS);
                    response.data = "success";
                    response.ObjData = li1;
                    response.ResponseCode = 0;

                    var responseData = li1.Rows.OfType<DataRow>()
                .Select(row => li1.Columns.OfType<DataColumn>()
                    .ToDictionary(col => col.ColumnName, c => row[c].ToString()));

                    returnJsonData = new { response = true, responseCode = "00", data = responseData };
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
                Model.ErrorLog objError = new Model.ErrorLog();
                objError.User = new Model.User();
                objError.Branch = new Model.Branch();
                objError.Client = new Model.Client();

                objError.Error = "Api : getpaysafebarcode --" + ex.ToString();
                objError.Branch_ID = 1;
                objError.Date = DateTime.Now;
                objError.User_ID = 1;
                objError.Id = 1;
                objError.Function_Name = "getpaysafebarcode";
                Service.srvErrorLog srvError = new Service.srvErrorLog();
                srvError.Create(objError, HttpContext);
                returnJsonData = new { response = false, responseCode = "01", data = "Invalid request." };
                return new JsonResult(returnJsonData);
            }            
        }


            [HttpPost]
        [Authorize]
        [Route("sendmoneydetails")]
        public JsonResult SendMoneyDetails([FromBody] JsonElement objdata)
        {
            HttpContext context = HttpContext;
            List<Model.Transaction> _lst = new List<Model.Transaction>();
            var returnJsonData = (dynamic)null;
            Model.response.WebResponse response = null;
            string jsonData = System.Text.Json.JsonSerializer.Serialize(objdata);
            dynamic data = JObject.Parse(jsonData);
            CompanyInfo.InsertrequestLogTracker("sendmoneydetails full request body: " + JObject.Parse(jsonData), 0, 0, 0, 0, "SendMoneyDetails", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
            try
            {
                if (!SqlInjectionProtector.ReadJsonElementValues(objdata) || !SqlInjectionProtector.ReadJsonElementValuesScript(objdata))
                {
                    var errorResponse = new { response = false, responseCode = "02", data = "Invalid input detected." };
                    return new JsonResult(errorResponse) { StatusCode = 400 };
                }

                #region validateData
                try
                {
                    if (data.sendingAmount == "" || data.sendingAmount == null)
                    {
                        returnJsonData = new { response = false, responseCode = "02", data = "Invalid Sending Amount." };
                        return new JsonResult(returnJsonData);
                    }
                    else if (data.receivingAmount == "" || data.receivingAmount == null)
                    {
                        returnJsonData = new { response = false, responseCode = "02", data = "Invalid Receiving Amount." };
                        return new JsonResult(returnJsonData);
                    }
                    else if (data.beneficiaryID == "" || data.beneficiaryID == null)
                    {
                        returnJsonData = new { response = false, responseCode = "02", data = "Invalid Beneficiary ID." };
                        return new JsonResult(returnJsonData);
                    }
                    else if (data.branchID == "" || data.branchID == null)
                    {
                        returnJsonData = new { response = false, responseCode = "02", data = "Invalid Branch ID." };
                        return new JsonResult(returnJsonData);
                    }
                    else if (data.clientID == "" || data.clientID == null)
                    {
                        returnJsonData = new { response = false, responseCode = "02", data = "Invalid Client ID." };
                        return new JsonResult(returnJsonData);
                    }
                    else if (data.customerID == "" || data.customerID == null)
                    {
                        returnJsonData = new { response = false, responseCode = "02", data = "Invalid Customer ID." };
                        return new JsonResult(returnJsonData);
                    }
                    else if (data.deliveryTypeID == "" || data.deliveryTypeID == null)
                    {
                        returnJsonData = new { response = false, responseCode = "02", data = "Invalid Delivery Type ID." };
                        return new JsonResult(returnJsonData);
                    }
                    else if (data.exchangeRate == "" || data.exchangeRate == null)
                    {
                        returnJsonData = new { response = false, responseCode = "02", data = "Invalid Exchange Rate." };
                        return new JsonResult(returnJsonData);
                    }
                   
                    else if (data.paymentDepositeTypeID == "" || data.paymentDepositeTypeID == null)
                    {
                        returnJsonData = new { response = false, responseCode = "02", data = "Invalid Payment Deposite Type ID." };
                        return new JsonResult(returnJsonData);
                    }
                    
                    else if (data.transferFees == "" || data.transferFees == null)
                    {
                        returnJsonData = new { response = false, responseCode = "02", data = "Invalid Transfer Fees." };
                        return new JsonResult(returnJsonData);
                    }
                   
                }
                catch (Exception ex)
                {
                    Model.ActivityLogTracker _objActivityLog = new Model.ActivityLogTracker();
                    _objActivityLog.Activity = " Exception Found CheckLimit Error: " + ex.ToString() + " ";
                    _objActivityLog.FunctionName = "citylist";
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

                object o = JsonConvert.DeserializeObject(jsonData);
                bool checkinjection = CompanyInfo.checksqlinjectiondata(o);
                if (!checkinjection)
                {
                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_TECHNICAL_ERROR);
                    response.ResponseMessage = "Invalid Field Values.";
                    response.ResponseCode = 6;
                    returnJsonData = new { response = false, responseCode = "02", data = response.ResponseMessage };
                    return new JsonResult(returnJsonData);
                }

                Transaction obj = new Transaction();

                obj.AmountInGBP = data.sendingAmount;
                obj.AmountInPKR = data.receivingAmount;
                obj.Beneficiary_ID = data.beneficiaryID;
                obj.CB_ID = data.branchID;
                obj.Client_ID = data.clientID;
                obj.Customer_ID = data.customerID;
                obj.DeliveryType_Id = data.deliveryTypeID;
                obj.Discount_Amount = data.discountAmount;
                obj.Discount_Code = data.discountCode;
                obj.Discount_ID = data.discountID;
                obj.Discount_Perm = data.discountPerm;
                obj.Exchange_Rate = data.exchangeRate;
                obj.PType_ID = data.pTypeID;
                obj.PaymentDepositType_ID = data.paymentDepositeTypeID;
                obj.SOFID = data.sofID;
                obj.Transfer_Cost = data.transferCost;
                obj.Transfer_Fees = data.transferFees;
                obj.Wallet_Amount = data.walletAmount;
                obj.Wallet_Currency = data.walletCurrency;
                obj.Wallet_ID = data.walletID;
                obj.Wallet_Perm = data.walletPerm;

                if (!SqlInjectionProtector.ValidateObjectForSqlInjection(obj) || !SqlInjectionProtector.ValidateObjectForScriptSqlInjection(obj))
                {
                    return new JsonResult(returnJsonData) { StatusCode = StatusCodes.Status400BadRequest, Value = "Invalid input detected." };
                }

                Service.srvSendMoney srv = new Service.srvSendMoney();
                DataTable dt = srv.SendMoneyDetails(obj, context);
                if (dt != null && dt.Rows.Count > 0)
                {
                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_SUCCESS);
                    response.data = "success";
                    response.ObjData = dt;
                    response.ResponseCode = 0;

                    foreach (DataColumn column in dt.Columns)
                    {
                        // Replace "OldColumnName" with the desired new name
                        column.ColumnName = column.ColumnName.Replace("Status", "status");
                        column.ColumnName = column.ColumnName.Replace("CallBackUrl", "callBackUrl");
                    }

                    var relationshipData = dt.Rows.OfType<DataRow>()
                .Select(row => dt.Columns.OfType<DataColumn>()
                    .ToDictionary(col => col.ColumnName, c => row[c]));
                    List<Dictionary<string, object>> Transaction = new List<Dictionary<string, object>>();

                    returnJsonData = new { response = true, responseCode = "00", data = relationshipData };
                    return new JsonResult(returnJsonData);
                }
                else
                {
                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_TECHNICAL_ERROR);
                    response.ResponseMessage = "No Records Found.";
                    response.ResponseCode = 0;
                    returnJsonData = new { response = false, responseCode = "01", data = response.ResponseMessage };
                    return new JsonResult(returnJsonData);
                }
            }
            catch (Exception ex)
            {
                Model.ErrorLog objError = new Model.ErrorLog();
                objError.User = new Model.User();
                objError.Branch = new Model.Branch();
                objError.Client = new Model.Client();

                objError.Error = "Api : CheckLimit --" + ex.ToString();
                objError.Branch_ID = 1;
                objError.Date = DateTime.Now;
                objError.User_ID = 1;
                objError.Id = 1;
                objError.Function_Name = "CheckLimit";
                Service.srvErrorLog srvError = new Service.srvErrorLog();
                srvError.Create(objError, HttpContext);
                returnJsonData = new { response = false, responseCode = "01", data = "Invalid request." };
                return new JsonResult(returnJsonData);
            }
            return new JsonResult(returnJsonData);
        }

        [HttpPost]
        [Route("bindreceiptconfiguration")]
        public JsonResult BindReceiptConfiguration([FromBody] JsonObject Obj)// Digvijay
        {
            var returnJsonData = (dynamic)null;
            HttpContext context = null;
            DataTable dt_receipt_Configuration = new DataTable();
            
            string json = System.Text.Json.JsonSerializer.Serialize(Obj);
            dynamic data = JObject.Parse(json);

            Transaction obj = JsonConvert.DeserializeObject<Transaction>(json);
            _ = Task.Run(() => CompanyInfo.InsertrequestLogTracker("bindreceiptconfiguration full request body: " + JObject.Parse(json), 0, 0, 0, 0, "BindReceiptConfiguration", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext));
            try
            {
                Service.srvViewTransferHistory srv = new Service.srvViewTransferHistory();
                dt_receipt_Configuration = srv.BindReceiptConfiguration(obj);

                var responseData = dt_receipt_Configuration.Rows.OfType<DataRow>()
                .Select(row => dt_receipt_Configuration.Columns.OfType<DataColumn>()
                    .ToDictionary(col => col.ColumnName, c => row[c].ToString().Replace("\n", "")));

                returnJsonData = new { response = true, responseCode = "00", data = responseData };
                return new JsonResult(returnJsonData);
            }
            catch (Exception ex)
            {
                //Error Log Handled
                Model.ErrorLog objError = new Model.ErrorLog();
                objError.User = new Model.User();
                objError.Error = "Api : BindReceiptConfiguration --" + ex.ToString();
                objError.Date = DateTime.Now;
                objError.User_ID = 1;
                objError.Client_ID = obj.Client_ID;
                objError.Function_Name = "BindReceiptConfiguration";
                Service.srvErrorLog srvError = new Service.srvErrorLog();
                srvError.Create(objError, context);
                returnJsonData = new { response = false, responseCode = "02", data = "No Records Found." };
                return new JsonResult(returnJsonData);
            }
          
        }

        [HttpPost]
        [Authorize]
        [Route("getAxcessmsTransactionStatus")]
        public JsonResult getAxcessmsTransactionStatus([FromBody] JsonObject Obj)// Rushikesh
        {
            var returnJsonData = (dynamic)null;
            HttpContext context = null; var validateJsonData = (dynamic)null;
            DataTable dt = new DataTable();
            dt.Columns.Add("Success", typeof(string));
            dt.Columns.Add("Client_ID", typeof(string));
            dt.Columns.Add("User_ID", typeof(string));
            dt.Columns.Add("CB_ID", typeof(string));
            dt.Columns.Add("transactionref", typeof(string));
            dt.Columns.Add("mainamount", typeof(string));
            dt.Columns.Add("currency", typeof(string));
            try
            {
                Model.response.WebResponse response = null;
                string json = System.Text.Json.JsonSerializer.Serialize(Obj);
                dynamic data = JObject.Parse(json);

                Transaction obj = JsonConvert.DeserializeObject<Transaction>(json);

                _ = Task.Run(() => CompanyInfo.InsertrequestLogTracker("getAxcessmstransactionstatus full request body: " + JObject.Parse(json), 0, 0, 0, 0, "getAxcessmstransactionstatus", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext));
                
                if (!SqlInjectionProtector.ReadJsonObjectValues(Obj) || !SqlInjectionProtector.ReadJsonObjectValuesScript(Obj))
                {
                    var errorResponse = new { response = false, responseCode = "02", data = "Invalid input detected." };
                    return new JsonResult(errorResponse) { StatusCode = 400 };
                }
                
                string refNumber = obj.ReferenceNo;
                Service.srvSendMoney srv = new Service.srvSendMoney();
                DataTable li1 = srv.getAxcessmsTransactionStatus(obj, obj.ReferenceNo);
                if (li1 != null && li1.Rows.Count > 0)
                {
                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_SUCCESS);
                    response.data = "success";
                    response.ObjData = li1;
                    response.ResponseCode = 0;

                    var responseData = li1.Rows.OfType<DataRow>()
                .Select(row => li1.Columns.OfType<DataColumn>()
                    .ToDictionary(col => col.ColumnName, c => row[c].ToString()));

                    returnJsonData = new { response = true, responseCode = "00", data = responseData };
                    return new JsonResult(returnJsonData);
                }
                else
                {
                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_TECHNICAL_ERROR);
                    response.ResponseMessage = "No Records Found.";
                    response.ResponseCode = 0;
                    returnJsonData = new { response = false, responseCode = "02", data = "No Records Found." };
                    return new JsonResult(returnJsonData);
                }

            }
            catch (Exception ex)
            {
                _ = Task.Run(() => CompanyInfo.InsertActivityLogDetails(" Axcessms getAxcessmsTransactionStatus Error : " + ex.ToString(), 0, 0, 0, 0, "getAxcessmsTransactionStatus", 0, 0, "Send Money", context));
                dt.Rows.Add(false, "", "", "", "", "", "");
                returnJsonData = new { response = false, responseCode = "02", data = "No Records Found." };
                return new JsonResult(returnJsonData);
            }
            
        }

        [HttpPost]
        [Route("reviewdetails")]
        public JsonResult ReviewDetails([FromBody] JsonElement objdata)
        {
            HttpContext context = HttpContext;
            var returnJsonData = (dynamic)null;
            Model.response.WebResponse response = null;
            string jsonData = System.Text.Json.JsonSerializer.Serialize(objdata);
            dynamic data = JObject.Parse(jsonData);
            CompanyInfo.InsertrequestLogTracker("ReviewDetails full request body: " + JObject.Parse(jsonData), 0, 0, 0, 0, "ReviewDetails", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
            try
            {
                if (!SqlInjectionProtector.ReadJsonElementValues(objdata) || !SqlInjectionProtector.ReadJsonElementValuesScript(objdata))
                {
                    var errorResponse = new { response = false, responseCode = "02", data = "Invalid input detected." };
                    return new JsonResult(errorResponse) { StatusCode = 400 };
                }

                Transaction obj = new Transaction();
                obj.Branch_ID = data.branchID;
                obj.Client_ID = data.clientID;
                obj.Customer_ID = data.customerID;
                obj.count = data.transactionCount;
                obj.Perm_ID = data.permID;

                Service.srvSendMoney srv = new Service.srvSendMoney();
                Model.Transaction dt = srv.ReviewDetails(obj, context);
                if (dt != null)
                {
                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_SUCCESS);
                    response.data = "success";
                    response.ObjData = dt;
                    response.ResponseCode = 0;
                    returnJsonData = new { response = true, responseCode = "00", data = dt.CallBackURL };
                }
                else
                {
                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_TECHNICAL_ERROR);
                    response.ResponseMessage = "No Records Found.";
                    response.ResponseCode = 0;
                    returnJsonData = new { response = false, responseCode = "02", data = response.ResponseMessage };
                }
            }
            catch (Exception ex)
            {
                Model.ErrorLog objError = new Model.ErrorLog();
                objError.User = new Model.User();
                objError.Branch = new Model.Branch();
                objError.Client = new Model.Client();

                objError.Error = "Api : ReviewDetails --" + ex.ToString();
                objError.Branch_ID = 1;
                objError.Date = DateTime.Now;
                objError.User_ID = 1;
                objError.Id = 1;
                objError.Function_Name = "ReviewDetails";
                Service.srvErrorLog srvError = new Service.srvErrorLog();
                srvError.Create(objError, HttpContext);
                returnJsonData = new { response = false, responseCode = "02", data = "No Records Found." };
                return new JsonResult(returnJsonData);
            }
            return new JsonResult(returnJsonData);
        }


        [HttpPost]
        [Route("checkcustomerpersonaldetails")]
        public IActionResult CheckCustomerPersonalDetails([FromBody] JsonElement objdata)
        {
            HttpContext context = HttpContext;
            var returnJsonData = (dynamic)null;
            Model.response.WebResponse response = null;
            string jsonData = System.Text.Json.JsonSerializer.Serialize(objdata);
            dynamic data = JObject.Parse(jsonData);
            CompanyInfo.InsertrequestLogTracker("CheckCustomerPersonalDetails full request body: " + JObject.Parse(jsonData), 0, 0, 0, 0, "CheckCustomerPersonalDetails", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
            try
            {
                if (!SqlInjectionProtector.ReadJsonElementValues(objdata) || !SqlInjectionProtector.ReadJsonElementValuesScript(objdata))
                {
                    var errorResponse = new { response = false, responseCode = "02", data = "Invalid input detected." };
                    return new JsonResult(errorResponse) { StatusCode = 400 };
                }

                Transaction obj = new Transaction();
                obj.Branch_ID = data.Branch_ID;
                obj.Client_ID = data.Client_ID;
                obj.Customer_ID = data.Customer_ID;
                obj.UserName = data.UserName;
                Service.srvSendMoney srv = new Service.srvSendMoney();
                DataTable licheckAddress = srv.CheckCustomerPersonalDetails(obj);
                if (licheckAddress != null && licheckAddress.Rows.Count != 0)
                {
                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_SUCCESS);
                    response.ResponseMessage = Convert.ToString(licheckAddress.Rows[0][1]);
                    response.ResponseCode = Convert.ToInt32(licheckAddress.Rows[0][0]);

              
                    List<Dictionary<string, object>> Transaction_count = new List<Dictionary<string, object>>();

                     
                        foreach (DataRow dr in licheckAddress.Rows)
                        {
                        Dictionary<string, object> Transactioncount = new Dictionary<string, object>();
                        Transactioncount["ResponseCode"] = Convert.ToInt32(licheckAddress.Rows[0][0]);
                        Transactioncount["ResponseMessage"] = Convert.ToString(licheckAddress.Rows[0][1]);
                        Transaction_count.Add(Transactioncount);
                    }


                    var jsonDatag = new { response = true, responseCode = "00", data = Transaction_count };
                    return new JsonResult(jsonDatag);


                }
                else
                {
                    var jsonDatag = new { response = false, responseCode = "02", data = "No Records Found." };
                    return new JsonResult(jsonDatag);
                }

            }
            catch (Exception ex)
            {
                Model.ErrorLog objError = new Model.ErrorLog();
                objError.User = new Model.User();
                objError.Branch = new Model.Branch();
                objError.Client = new Model.Client();

                objError.Error = "Api : CheckCustomerPersonalDetails --" + ex.ToString();
                objError.Branch_ID = 1;
                objError.Date = DateTime.Now;
                objError.User_ID = 1;
                objError.Id = 1;
                objError.Function_Name = "CheckCustomerPersonalDetails";
                Service.srvErrorLog srvError = new Service.srvErrorLog();
                srvError.Create(objError, HttpContext);
                var jsonDatag = new { response = false, responseCode = "02", data = "No Records Found." };
                return new JsonResult(jsonDatag);
            }
            
        }

    }
        

}
