using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Data;
using System.Text.Json;
using MySqlConnector;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Net.Http.Headers;
using Calyx_Solutions.Model;
using Newtonsoft.Json;
using System.Net;
using System.Security.Claims;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Calyx_Solutions.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RewardController : ControllerBase
    {
        // GET: api/<RewardController>


        // POST api/<RewardController>

        [HttpPost]
        [Route("getlinkedemails")]
        public IActionResult Get_linked_emails([FromBody] JsonElement Obj)
        {
            var returnJsonData = (dynamic)null;
            HttpContext context = HttpContext;
            Model.response.WebResponse response = null;
            string json1 = System.Text.Json.JsonSerializer.Serialize(Obj);
            dynamic data = JObject.Parse(json1);
            CompanyInfo.InsertrequestLogTracker("getlinkedemails full request body: " + JObject.Parse(json1), 0, 0, 0, 0, "Get_linked_emails", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
            var claimsIdentity = User.Identity as ClaimsIdentity;//sanket
            var authHeader = "";
            try { authHeader = HttpContext.Request.Headers["Authorization"].ToString(); } catch (Exception egx) { authHeader = ""; }

            bool auth = AuthController.checkAuth(claimsIdentity, Obj, Convert.ToString(authHeader));
            if (!auth)
            {
                var errorResponse = new { response = false, responseCode = "403", data = "Access Forbidden" };
                return new JsonResult(errorResponse) { StatusCode = (int)HttpStatusCode.Forbidden };
            }
            var validateJsonData = (dynamic)null;
            Model.Transaction obj = new Model.Transaction();

            try
            {
                if (!SqlInjectionProtector.ReadJsonElementValues(Obj) || !SqlInjectionProtector.ReadJsonElementValuesScript(Obj))
                {
                    var errorResponse = new { response = false, responseCode = "02", data = "Invalid input detected." };
                    return new JsonResult(errorResponse) { StatusCode = 400 };
                }

                obj.Client_ID = data.Client_ID;
                obj.Customer_ID = data.Customer_ID;
              
                Service.srvWallet srv = new Service.srvWallet();
                DataTable li1 = srv.Get_linked_emails(obj);
                //Model.Dashboard _Obj = new Model.Dashboard();
                if (li1 != null && li1.Rows.Count > 0)
                {
                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_SUCCESS);
                    response.data = "success";
                    response.ObjData = li1;
                    response.ResponseCode = 0;

                    var returndata = Enumerable.Empty<Dictionary<string, object>>();
                    try
                    {
                        returndata = li1.Rows.OfType<DataRow>()
                        .Select(row => li1.Columns.OfType<DataColumn>()
                        .Where(col => col.ColumnName.ToLower() != "customer_id") // exclude customer_id
                        .ToDictionary(col => col.ColumnName, col => row[col]));

                    }
                    catch (Exception ex)
                    {
                        returndata = li1.Rows.OfType<DataRow>()
                        .Select(row => li1.Columns.OfType<DataColumn>()
                        .ToDictionary(col => col.ColumnName, c => row[c]));
                    }

                   /* var returndata = li1.Rows.OfType<DataRow>()
                    .Select(row => li1.Columns.OfType<DataColumn>()
                        .ToDictionary(col => col.ColumnName, c => row[c]));*/

                    returnJsonData = new { response = true, responseCode = "00", data = returndata };
                    return new JsonResult(returnJsonData);
                }
                else
                {
                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_TECHNICAL_ERROR);
                    response.ResponseMessage = "No Records Found.";
                    response.ResponseCode = 0;
                    var returndata = li1.Rows.OfType<DataRow>()
                    .Select(row => li1.Columns.OfType<DataColumn>()
                        .ToDictionary(col => col.ColumnName, c => row[c]));

                    returnJsonData = new { response = false, responseCode = "02", data = returndata };
                    return new JsonResult(returnJsonData);
                }
            }
            catch (Exception ex)
            {
                response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_TECHNICAL_ERROR);
                response.sErrorExceptionText = ex.ToString();
                response.ResponseMessage = "Technical error";
                response.ResponseCode = 3;
                //Error Log Handled
                Model.ErrorLog objError = new Model.ErrorLog();
                objError.User = new Model.User();
                objError.Error = "Api : Get Get_linked_emails Details --" + ex.ToString();
                objError.Date = DateTime.Now;
                objError.User_ID = 1;
                objError.Client_ID = data.Client_ID;
                objError.Function_Name = "Get_linked_emails";
                Service.srvErrorLog srvError = new Service.srvErrorLog();
                srvError.Create(objError, context);
                CompanyInfo.InsertErrorLogTracker(" Exception Get_linked_emails Error: " + ex.ToString(), 0, 0, 0, 0, "Get_linked_emails", Convert.ToInt32(data.Branch_ID), Convert.ToInt32(data.Client_ID), "Get_linked_emails", HttpContext);
                returnJsonData = new { response = false, responseCode = "01", data = "Invalid request." };
                return new JsonResult(returnJsonData);
            }

        }




            [HttpPost]
        [Route("getdiscountdetails")]
        public IActionResult GetInfo([FromBody] JsonElement Obj)
        {
            HttpContext context = HttpContext;
            Model.response.WebResponse response = null;
            string json1 = System.Text.Json.JsonSerializer.Serialize(Obj);
            dynamic data = JObject.Parse(json1);
            var validateJsonData = (dynamic)null;
            Model.Transaction obj1 = new Model.Transaction();
            CompanyInfo.InsertrequestLogTracker("getdiscountdetails full request body: " + JObject.Parse(json1), 0, 0, 0, 0, "GetInfo", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
            var claimsIdentity = User.Identity as ClaimsIdentity;//sanket
            var authHeader = "";
            try { authHeader = HttpContext.Request.Headers["Authorization"].ToString(); } catch (Exception egx) { authHeader = ""; }

            bool auth = AuthController.checkAuth(claimsIdentity, Obj, Convert.ToString(authHeader));
            if (!auth)
            {
                var errorResponse = new { response = false, responseCode = "403", data = "Access Forbidden" };
                return new JsonResult(errorResponse) { StatusCode = (int)HttpStatusCode.Forbidden };
            }
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
                if (data.customerID == "" || data.customerID == null)
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "Invalid customer ID." };
                    return new JsonResult(validateJsonData);
                }


                obj1.Client_ID = data.clientID;
                obj1.Branch_ID = data.branchID;
                obj1.Customer_ID = data.customerID;


                List<Dictionary<string, object>> collectionpoint = new List<Dictionary<string, object>>();
                Service.srvDiscount srv = new Service.srvDiscount();
                DataTable li1 = srv.getavailableDiscDetailsreferee(obj1, context);
                if (li1 != null && li1.Rows.Count > 0)
                {
                    

                    li1.Columns["ID"].ColumnName = "id";
                     li1.Columns["Discount_Code"].ColumnName = "discountCode";
                     li1.Columns["Discounttype_ID"].ColumnName = "discounttypeID";
                     li1.Columns["Amounttype_ID"].ColumnName = "amounttypeID";
                     li1.Columns["Discount_Value"].ColumnName = "discountValue";
                     li1.Columns["Customer_Eligibility_ID"].ColumnName = "customerEligibilityID";
                     li1.Columns["Usagelimit_flag"].ColumnName = "usagelimitFlag";
                     li1.Columns["Usage_limit"].ColumnName = "usagelimit";
                     li1.Columns["Start_Date"].ColumnName = "startDate";
                     li1.Columns["End_Date"].ColumnName = "endDate";
                     li1.Columns["Min_TrnAmount"].ColumnName = "minTrnAmount";
                     li1.Columns["Max_TrnAmount"].ColumnName = "maxTrnAmount";
                     li1.Columns["Delete_Status"].ColumnName = "deleteStatus";
                     li1.Columns["Record_Insert_Datetime"].ColumnName = "recordInsertDatetime";
                     li1.Columns["Branch_ID"].ColumnName = "branchID";
                     li1.Columns["Client_ID"].ColumnName = "clientID";
                     li1.Columns["Voucher_Flag"].ColumnName = "voucherFlag";
                     li1.Columns["NewRegisteration_Flag"].ColumnName = "newRegisterationFlag";
                     li1.Columns["Transaction_ID"].ColumnName = "transactionID";
                     li1.Columns["Customer_ID"].ColumnName = "customerID";
                     li1.Columns["Discount_Available"].ColumnName = "discountAvailable";
                     li1.Columns["referrer_id"].ColumnName = "referrerid";
                     li1.Columns["First_Name"].ColumnName = "firstName";
                     li1.Columns["First_Name1"].ColumnName = "firstName1";
                     li1.Columns["Record_Insert_Datetime1"].ColumnName = "recordInsertDatetime1";
                     li1.Columns["scheme_id"].ColumnName = "schemeid";
                     li1.Columns["scheme_type_id"].ColumnName = "schemetypeid";
                     li1.Columns["multiples_of"].ColumnName = "multiplesof";
                     li1.Columns["scheme_description"].ColumnName = "schemedescription";
                     li1.Columns["valid_from_date"].ColumnName = "validFromDate";
                     li1.Columns["valid_to_date"].ColumnName = "validToDate";
                     li1.Columns["referrer_perk_type"].ColumnName = "referrerPerkType";
                     li1.Columns["referrer_discount_type"].ColumnName = "referrerdiscounttype";
                     li1.Columns["referrer_value"].ColumnName = "referrervalue";
                     li1.Columns["referrer_nooftx"].ColumnName = "referrernooftx";
                     li1.Columns["referrer_maxnoofreferee"].ColumnName = "referrermaxnoofreferee";
                     li1.Columns["referee_perk_type"].ColumnName = "refereeperktype";
                     li1.Columns["referee_discount_type"].ColumnName = "referee_discount_type";
                     li1.Columns["referee_value"].ColumnName = "refereevalue";
                     li1.Columns["referee_nooftx"].ColumnName = "refereenooftx";
                     li1.Columns["referee_mintransferamount"].ColumnName = "refereemintransferamount";
                     li1.Columns["referee_applicablefor"].ColumnName = "refereeapplicablefor";
                     li1.Columns["Branch_ID1"].ColumnName = "branchID1";
                     li1.Columns["Client_ID1"].ColumnName = "clientID1";
                     li1.Columns["user_id"].ColumnName = "userID";
                     li1.Columns["record_insert_date"].ColumnName = "recordInsertdate";
                     li1.Columns["record_update_date"].ColumnName = "recordUpdatedate";
                     li1.Columns["delete_status1"].ColumnName = "deleteStatus1";
                     li1.Columns["referee_signup_flag"].ColumnName = "refereesignupflag";
                     li1.Columns["transaction_count"].ColumnName = "transactioncount";
                     li1.Columns["maxtransferamount"].ColumnName = "maxtransferamount";
                     li1.Columns["SalesRep_Flag"].ColumnName = "SalesRepFlag";
                     li1.Columns["AddPerk_AftTxn_Flag"].ColumnName = "AddPerkAftTxnFlag";
                     li1.Columns["tranfer_perk_type"].ColumnName = "Tranferperktype";
                     li1.Columns["transfer_amount_type"].ColumnName = "transferAmounttype";
                     li1.Columns["TransferPerk_Value"].ColumnName = "transferPerkValue";
                     li1.Columns["TransferPerk_Duration"].ColumnName = "TransferPerkDuration";
                     li1.Columns["EndDate"].ColumnName = "endDate1";
                     li1.Columns["IsVoucherAvailable"].ColumnName = "isVoucherAvailable";

                     var relationshipData = li1.Rows.OfType<DataRow>()
                .Select(row => li1.Columns.OfType<DataColumn>()
                    .ToDictionary(col => col.ColumnName, c => row[c].ToString()));
                     validateJsonData = new { response = true, responseCode = "00", data = relationshipData };
                     return new JsonResult(validateJsonData);

                    foreach (DataRow dr in li1.Rows)
                    {
                        Dictionary<string, object> collection_point = new Dictionary<string, object>();
                        collection_point["id"] = dr["ID"];
                        collection_point["discountCode"] = dr["Discount_Code"];
                        collection_point["discounttypeID"] = dr["Discounttype_ID"];
                        collection_point["amounttypeID"] = dr["Amounttype_ID"];
                        collection_point["discountValue"] = dr["Discount_Value"];
                        collection_point["customerEligibilityID"] = dr["Customer_Eligibility_ID"];
                        collection_point["usagelimitFlag"] = dr["Usagelimit_flag"];
                        collection_point["usagelimit"] = dr["Usage_limit"];
                        collection_point["startDate"] = dr["Start_Date"];
                        collection_point["endDate"] = dr["End_Date"];
                        collection_point["minTrnAmount"] = dr["Min_TrnAmount"];
                        collection_point["maxTrnAmount"] = dr["Max_TrnAmount"];
                        collection_point["deleteStatus"] = dr["Delete_Status"];
                        collection_point["recordInsertDatetime"] = dr["Record_Insert_Datetime"];
                        collection_point["branchID"] = dr["Branch_ID"];
                        collection_point["clientID"] = dr["Client_ID"];
                        collection_point["voucherFlag"] = dr["Voucher_Flag"];
                        collection_point["newRegisterationFlag"] = dr["NewRegisteration_Flag"];
                        collection_point["transactionID"] = dr["Transaction_ID"];
                        collection_point["customerID"] = dr["Customer_ID"];
                        collection_point["discountAvailable"] = dr["Discount_Available"];
                        collection_point["referrerid"] = dr["referrer_id"];
                        collection_point["firstName"] = dr["First_Name"];
                        collection_point["firstName1"] = dr["First_Name1"];
                        collection_point["recordInsertDatetime1"] = dr["Record_Insert_Datetime1"];
                        collection_point["schemeid"] = dr["scheme_id"];
                        collection_point["schemetypeid"] = dr["scheme_type_id"];
                        collection_point["multiplesof"] = dr["multiples_of"];
                        collection_point["schemedescription"] = dr["scheme_description"];
                        collection_point["validFromDate"] = dr["valid_from_date"];
                        collection_point["validToDate"] = dr["valid_to_date"];
                        collection_point["referrerPerkType"] = dr["referrer_perk_type"];
                        collection_point["referrerdiscounttype"] = dr["referrer_discount_type"];
                        collection_point["referrervalue"] = dr["referrer_value"];
                        collection_point["referrernooftx"] = dr["referrer_nooftx"];
                        collection_point["referrermaxnoofreferee"] = dr["referrer_maxnoofreferee"];
                        collection_point["refereeperktype"] = dr["referee_perk_type"];
                        collection_point["referee_discount_type"] = dr["referee_discount_type"];
                        collection_point["refereevalue"] = dr["referee_value"];
                        collection_point["refereenooftx"] = dr["referee_nooftx"];
                        collection_point["refereemintransferamount"] = dr["referee_mintransferamount"];
                        collection_point["refereeapplicablefor"] = dr["referee_applicablefor"];
                        collection_point["branchID1"] = dr["Branch_ID1"];
                        collection_point["clientID1"] = dr["Client_ID1"];
                        collection_point["userID"] = dr["user_id"];
                        collection_point["recordInsertdate"] = dr["record_insert_date"];
                        collection_point["recordUpdatedate"] = dr["record_update_date"];
                        collection_point["deleteStatus"] = dr["delete_status1"];
                        collection_point["refereesignupflag"] = dr["referee_signup_flag"];
                        collection_point["transactioncount"] = dr["transaction_count"];
                        collection_point["maxtransferamount"] = dr["maxtransferamount"];
                        collection_point["SalesRepFlag"] = dr["SalesRep_Flag"];
                        collection_point["AddPerkAftTxnFlag"] = dr["AddPerk_AftTxn_Flag"];
                        collection_point["Tranferperktype"] = dr["tranfer_perk_type"];
                        collection_point["transferAmounttype"] = dr["transfer_amount_type"];
                        collection_point["transferPerkValue"] = dr["TransferPerk_Value"];
                        collection_point["TransferPerkDuration"] = dr["TransferPerk_Duration"];
                        collection_point["endDate1"] = dr["EndDate"];
                        collection_point["isVoucherAvailable"] = dr["IsVoucherAvailable"];

                        collectionpoint.Add(collection_point);
                    }

                    validateJsonData = new { response = true, responseCode = "00", data = collectionpoint };
                    return new JsonResult(validateJsonData);
                    
                }
                else
                {
                    validateJsonData = new { response = false, responseCode = "02", data = collectionpoint };
                    return new JsonResult(validateJsonData);
                }
            }
            catch (Exception ex)
            {
                validateJsonData = new { response = false, responseCode = "01", data = "Invalid Request" };
                string Acitivy = "Api: getdiscountdetails " + ex.ToString();
                CompanyInfo.InsertErrorLogTracker(Acitivy.ToString(), 0, 0, 0, 0, "getdiscountdetails", Convert.ToInt32(obj1.Branch_ID), Convert.ToInt32(obj1.Client_ID), "", context);
                return new JsonResult(validateJsonData);
            }



        }

        [HttpPost]
        [Route("usedcodes")]
        public IActionResult GetInfo1([FromBody] JsonElement Obj)
        {
            HttpContext context = HttpContext;
            Model.response.WebResponse response = null;
            string json1 = System.Text.Json.JsonSerializer.Serialize(Obj);
            dynamic data = JObject.Parse(json1);
            CompanyInfo.InsertrequestLogTracker("usedcodes full request body: " + JObject.Parse(json1), 0, 0, 0, 0, "GetInfo1", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
            var validateJsonData = (dynamic)null;
            Model.Transaction obj1 = new Model.Transaction();
            var claimsIdentity = User.Identity as ClaimsIdentity;//sanket
            var authHeader = "";
            try { authHeader = HttpContext.Request.Headers["Authorization"].ToString(); } catch (Exception egx) { authHeader = ""; }

            bool auth = AuthController.checkAuth(claimsIdentity, Obj, Convert.ToString(authHeader));
            if (!auth)
            {
                var errorResponse = new { response = false, responseCode = "403", data = "Access Forbidden" };
                return new JsonResult(errorResponse) { StatusCode = (int)HttpStatusCode.Forbidden };
            }
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
                if (data.customerID == "" || data.customerID == null)
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "Invalid customer ID." };
                    return new JsonResult(validateJsonData);
                }


                obj1.Client_ID = data.clientID;
                obj1.Branch_ID = data.branchID;
                obj1.Customer_ID = data.customerID;


                List<Dictionary<string, object>> collectionpoint = new List<Dictionary<string, object>>();
                Service.srvDiscount srv = new Service.srvDiscount();
                DataTable li1 = srv.getusedDiscDetailsreferee(obj1, context);
                if (li1 != null && li1.Rows.Count > 0)
                {


                    foreach (DataRow dr in li1.Rows)
                    {
                        Dictionary<string, object> collection_point = new Dictionary<string, object>();
                        collection_point["id"] = dr["ID"];
                        collection_point["discountCode"] = dr["Discount_Code"];
                        collection_point["discounttypeID"] = dr["Discounttype_ID"];
                        collection_point["amounttypeID"] = dr["Amounttype_ID"];
                        collection_point["discountValue"] = dr["Discount_Value"];
                        collection_point["customerEligibilityID"] = dr["Customer_Eligibility_ID"];
                        collection_point["usagelimitFlag"] = dr["Usagelimit_flag"];
                        collection_point["usagelimit"] = dr["Usage_limit"];
                        collection_point["startDate"] = dr["Start_Date"];
                        collection_point["endDate"] = dr["End_Date"];
                        collection_point["minTrnAmount"] = dr["Min_TrnAmount"];
                        collection_point["maxTrnAmount"] = dr["Max_TrnAmount"];
                        collection_point["deleteStatus"] = dr["Delete_Status"];
                        collection_point["recordInsertDatetime"] = dr["Record_Insert_Datetime"];
                        collection_point["branchID"] = dr["Branch_ID"];
                        collection_point["clientID"] = dr["Client_ID"];
                        collection_point["voucherFlag"] = dr["Voucher_Flag"];
                        collection_point["newRegisterationFlag"] = dr["NewRegisteration_Flag"];
                        collection_point["transactionID"] = dr["Transaction_ID"];
                        collection_point["customerID"] = dr["Customer_ID"];
                        collection_point["discountAvailable"] = dr["Discount_Available"];
                        collection_point["referrerid"] = dr["referrer_id"];
                        collection_point["firstName"] = dr["First_Name"];
                        collection_point["firstName1"] = dr["First_Name1"];
                        collection_point["recordInsertDatetime1"] = dr["Record_Insert_Datetime1"];
                        collection_point["schemeid"] = dr["scheme_id"];
                        collection_point["schemetypeid"] = dr["scheme_type_id"];
                        collection_point["multiplesof"] = dr["multiples_of"];
                        collection_point["schemedescription"] = dr["scheme_description"];
                        collection_point["validFromDate"] = dr["valid_from_date"];
                        collection_point["validToDate"] = dr["valid_to_date"];
                        collection_point["referrerPerkType"] = dr["referrer_perk_type"];
                        collection_point["referrerdiscounttype"] = dr["referrer_discount_type"];
                        collection_point["referrervalue"] = dr["referrer_value"];
                        collection_point["referrernooftx"] = dr["referrer_nooftx"];
                        collection_point["referrermaxnoofreferee"] = dr["referrer_maxnoofreferee"];
                        collection_point["refereeperktype"] = dr["referee_perk_type"];
                        collection_point["referee_discount_type"] = dr["referee_discount_type"];
                        collection_point["refereevalue"] = dr["referee_value"];
                        collection_point["refereenooftx"] = dr["referee_nooftx"];
                        collection_point["refereemintransferamount"] = dr["referee_mintransferamount"];
                        collection_point["refereeapplicablefor"] = dr["referee_applicablefor"];
                        collection_point["branchID"] = dr["Branch_ID1"];
                        collection_point["clientID"] = dr["Client_ID1"];
                        collection_point["userID"] = dr["user_id"];
                        collection_point["recordInsertdate"] = dr["record_insert_date"];
                        collection_point["recordUpdatedate"] = dr["record_update_date"];
                        collection_point["deleteStatus"] = dr["delete_status1"];
                        collection_point["refereesignupflag"] = dr["referee_signup_flag"];
                        collection_point["transactioncount"] = dr["transaction_count"];
                        collection_point["maxtransferamount"] = dr["maxtransferamount"];
                        collection_point["SalesRepFlag"] = dr["SalesRep_Flag"];
                        collection_point["AddPerkAftTxnFlag"] = dr["AddPerk_AftTxn_Flag"];
                        collection_point["Tranferperktype"] = dr["tranfer_perk_type"];
                        collection_point["transferAmounttype"] = dr["transfer_amount_type"];
                        collection_point["transferPerkValue"] = dr["TransferPerk_Value"];
                        collection_point["TransferPerkDuration"] = dr["TransferPerk_Duration"];


                        collectionpoint.Add(collection_point);
                    }

                    validateJsonData = new { response = true, responseCode = "00", data = collectionpoint };
                    return new JsonResult(validateJsonData);
                }
                else
                {
                    validateJsonData = new { response = false, responseCode = "02", data = collectionpoint };
                    return new JsonResult(validateJsonData);
                }
            }
            catch (Exception ex)
            {
                validateJsonData = new { response = false, responseCode = "01", data = "Invalid Request" };
                string Acitivy = "Api: usedcodes " + ex.ToString();
                CompanyInfo.InsertErrorLogTracker(Acitivy.ToString(), 0, 0, 0, 0, "usedcodes", Convert.ToInt32(obj1.Branch_ID), Convert.ToInt32(obj1.Client_ID), "", context);
                return new JsonResult(validateJsonData);
            }



        }

        [HttpPost]
        [Route("expiredcodes")]
        public IActionResult GetInfo2([FromBody] JsonElement Obj)
        {
            HttpContext context = HttpContext;
            Model.response.WebResponse response = null;
            string json1 = System.Text.Json.JsonSerializer.Serialize(Obj);
            dynamic data = JObject.Parse(json1);
            CompanyInfo.InsertrequestLogTracker("expiredcodes full request body: " + JObject.Parse(json1), 0, 0, 0, 0, "GetInfo2", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
            var validateJsonData = (dynamic)null;
            Model.Transaction obj1 = new Model.Transaction();
            var claimsIdentity = User.Identity as ClaimsIdentity;//sanket
            var authHeader = "";
            try { authHeader = HttpContext.Request.Headers["Authorization"].ToString(); } catch (Exception egx) { authHeader = ""; }

            bool auth = AuthController.checkAuth(claimsIdentity, Obj, Convert.ToString(authHeader));
            if (!auth)
            {
                var errorResponse = new { response = false, responseCode = "403", data = "Access Forbidden" };
                return new JsonResult(errorResponse) { StatusCode = (int)HttpStatusCode.Forbidden };
            }
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
                if (data.customerID == "" || data.customerID == null)
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "Invalid customer ID." };
                    return new JsonResult(validateJsonData);
                }


                obj1.Client_ID = data.clientID;
                obj1.Branch_ID = data.branchID;
                obj1.Customer_ID = data.customerID;


                List<Dictionary<string, object>> collectionpoint = new List<Dictionary<string, object>>();
                Service.srvDiscount srv = new Service.srvDiscount();
                DataTable li1 = srv.getexpDiscDetailsreferee(obj1, context);
                if (li1 != null && li1.Rows.Count > 0)
                {


                    foreach (DataRow dr in li1.Rows)
                    {
                        Dictionary<string, object> collection_point = new Dictionary<string, object>();
                        collection_point["id"] = dr["ID"];
                        collection_point["discountCode"] = dr["Discount_Code"];
                        collection_point["discounttypeID"] = dr["Discounttype_ID"];
                        collection_point["amounttypeID"] = dr["Amounttype_ID"];
                        collection_point["discountValue"] = dr["Discount_Value"];
                        collection_point["customerEligibilityID"] = dr["Customer_Eligibility_ID"];
                        collection_point["usagelimitFlag"] = dr["Usagelimit_flag"];
                        collection_point["usagelimit"] = dr["Usage_limit"];
                        collection_point["startDate"] = dr["Start_Date"];
                        collection_point["endDate"] = dr["End_Date"];
                        collection_point["minTrnAmount"] = dr["Min_TrnAmount"];
                        collection_point["maxTrnAmount"] = dr["Max_TrnAmount"];
                        collection_point["deleteStatus"] = dr["Delete_Status"];
                        collection_point["recordInsertDatetime"] = dr["Record_Insert_Datetime"];
                        collection_point["branchID"] = dr["Branch_ID"];
                        collection_point["clientID"] = dr["Client_ID"];
                        collection_point["voucherFlag"] = dr["Voucher_Flag"];
                        collection_point["newRegisterationFlag"] = dr["NewRegisteration_Flag"];
                        collection_point["transactionID"] = dr["Transaction_ID"];
                        collection_point["customerID"] = dr["Customer_ID"];
                        collection_point["discountAvailable"] = dr["Discount_Available"];
                        collection_point["referrerid"] = dr["referrer_id"];
                        collection_point["firstName"] = dr["First_Name"];
                        collection_point["firstName1"] = dr["First_Name1"];

                        collection_point["schemeid"] = dr["scheme_id"];
                        collection_point["schemetypeid"] = dr["scheme_type_id"];
                        collection_point["multiplesof"] = dr["multiples_of"];
                        collection_point["schemedescription"] = dr["scheme_description"];
                        collection_point["validFromDate"] = dr["valid_from_date"];
                        collection_point["validToDate"] = dr["valid_to_date"];
                        collection_point["referrerPerkType"] = dr["referrer_perk_type"];
                        collection_point["referrerdiscounttype"] = dr["referrer_discount_type"];
                        collection_point["referrervalue"] = dr["referrer_value"];
                        collection_point["referrernooftx"] = dr["referrer_nooftx"];
                        collection_point["referrermaxnoofreferee"] = dr["referrer_maxnoofreferee"];
                        collection_point["refereeperktype"] = dr["referee_perk_type"];
                        collection_point["referee_discount_type"] = dr["referee_discount_type"];
                        collection_point["refereevalue"] = dr["referee_value"];
                        collection_point["refereenooftx"] = dr["referee_nooftx"];
                        collection_point["refereemintransferamount"] = dr["referee_mintransferamount"];
                        collection_point["refereeapplicablefor"] = dr["referee_applicablefor"];
                        collection_point["branchID"] = dr["Branch_ID1"];
                        collection_point["clientID"] = dr["Client_ID1"];
                        collection_point["userID"] = dr["user_id"];
                        collection_point["recordInsertdate"] = dr["record_insert_date"];
                        collection_point["recordUpdatedate"] = dr["record_update_date"];
                        collection_point["deleteStatus"] = dr["delete_status1"];
                        collection_point["refereesignupflag"] = dr["referee_signup_flag"];
                        collection_point["transactioncount"] = dr["transaction_count"];
                        collection_point["maxtransferamount"] = dr["maxtransferamount"];
                        collection_point["SalesRepFlag"] = dr["SalesRep_Flag"];
                        collection_point["AddPerkAftTxnFlag"] = dr["AddPerk_AftTxn_Flag"];
                        collection_point["Tranferperktype"] = dr["tranfer_perk_type"];
                        collection_point["transferAmounttype"] = dr["transfer_amount_type"];
                        collection_point["transferPerkValue"] = dr["TransferPerk_Value"];
                        collection_point["TransferPerkDuration"] = dr["TransferPerk_Duration"];


                        collectionpoint.Add(collection_point);
                    }

                    validateJsonData = new { response = true, responseCode = "00", data = collectionpoint };
                    return new JsonResult(validateJsonData);
                }
                else
                {
                    validateJsonData = new { response = false, responseCode = "02", data = collectionpoint };
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
        [Route("getwalletdetailscustomerprofile")]
        public IActionResult GetWalletDetailsActivity([FromBody] JsonElement Obj)
        {
            HttpContext context = HttpContext;
            Model.response.WebResponse response = null;
            var validateJsonData = (dynamic)null;
            string json1 = System.Text.Json.JsonSerializer.Serialize(Obj);
            dynamic data = JObject.Parse(json1);
            CompanyInfo.InsertrequestLogTracker("getwalletdetailscustomerprofile full request body: " + JObject.Parse(json1), 0, 0, 0, 0, "GetWalletDetailsActivity", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
            Model.Transaction obj = new Model.Transaction();
            var claimsIdentity = User.Identity as ClaimsIdentity;//sanket
            var authHeader = "";
            try { authHeader = HttpContext.Request.Headers["Authorization"].ToString(); } catch (Exception egx) { authHeader = ""; }

            bool auth = AuthController.checkAuth(claimsIdentity, Obj, Convert.ToString(authHeader));
            if (!auth)
            {
                var errorResponse = new { response = false, responseCode = "403", data = "Access Forbidden" };
                return new JsonResult(errorResponse) { StatusCode = (int)HttpStatusCode.Forbidden };
            }
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
                obj.Client_ID = data.clientID;
                obj.Customer_ID = data.customerID;
                obj.count = data.count;
                Service.srvWallet srv = new Service.srvWallet();
                DataTable li1 = srv.Get_WalletDetails_CustomerProfile(obj);

                if (li1 != null && li1.Rows.Count > 0)
                {
                    List<Dictionary<string, object>> Paytype = new List<Dictionary<string, object>>();

                    var returndata = li1.Rows.OfType<DataRow>()
                    .Select(row => li1.Columns.OfType<DataColumn>()
                        .ToDictionary(col => col.ColumnName, c => row[c].ToString()));

                    validateJsonData = new { response = true, responseCode = "00", data = returndata };
                    return new JsonResult(validateJsonData);

                    /*foreach (DataRow dr in li1.Rows)
                    {
                        Dictionary<string, object> Pay_Type = new Dictionary<string, object>();
                        Pay_Type["baseCountryId"] = dr["Base_Country_Id"];
                        Pay_Type["baseCurrencyId"] = dr["Base_Currency_Id"];
                        Pay_Type["clientID"] = dr["Client_ID"];
                        Pay_Type["countryActiveStatus"] = dr["Country_active_status"];
                        Pay_Type["deleteStatus"] = dr["Delete_Status"];
                        Pay_Type["deleteStatus"] = dr["Delete_Status1"];
                        Pay_Type["mappingID"] = dr["Mapping_ID"];
                        Pay_Type["maxAmountLimit"] = dr["Max_Amount_Limit"];
                        Pay_Type["payType"] = dr["PType"];
                        Pay_Type["payTypeDescription"] = dr["PType_Description"];
                        Pay_Type["payTypeID"] = dr["PType_ID"];
                        Pay_Type["paymentTypeSrc"] = dr["Payment_Type_Src"];
                        Pay_Type["paymentTypeSrcID"] = dr["Payment_Type_Src_ID"];
                        Pay_Type["reviewTransferMessage"] = dr["Review_Transfer_Message"];
                        Pay_Type["preferredFlag"] = dr["preferred_flag"];
                        Pay_Type["preferredFlag"] = dr["preferred_flag1"];
                        Paytype.Add(Pay_Type);
                    }

                    validateJsonData = new { response = true, responseCode = "00", data = Paytype };
                    return new JsonResult(validateJsonData);*/


                }
                else
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "No Record Found." };
                    return new JsonResult(validateJsonData);
                }

            }
            catch (Exception ex)
            {
                string Activity = ex.ToString();
                obj.Client_ID = data.clientID;
                obj.Branch_ID = data.branchID;
                CompanyInfo.InsertErrorLogTracker(Activity.ToString(), 0, 0, 0, 0, "GetWalletDetailsActivity", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "", context);
                validateJsonData = new { response = false, responseCode = "01", data = "Invalid request." };
                return new JsonResult(validateJsonData);
            }
            
        }


        [HttpPost]
        [Route("getrewarddetails")]
        public IActionResult GetRewardDetails([FromBody] JsonElement Obj)
        {
            HttpContext context = HttpContext;
            Model.response.WebResponse response = null;
            var validateJsonData = (dynamic)null;
            string json1 = System.Text.Json.JsonSerializer.Serialize(Obj);
            dynamic data = JObject.Parse(json1);
            CompanyInfo.InsertrequestLogTracker("getrewarddetails full request body: " + JObject.Parse(json1), 0, 0, 0, 0, "GetRewardDetails", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
            Model.Transaction obj = new Model.Transaction();
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
                if (data.transactionID == "" || data.transactionID == null)
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "Invalid Transaction ID." };
                    return new JsonResult(validateJsonData);
                }
                obj.Client_ID = data.clientID;
                obj.Transaction_ID = data.transactionID;

                Service.srvViewTransferHistory srv = new Service.srvViewTransferHistory();
                DataTable li1 = srv.ViewRewardDetails(obj);

                if (li1 != null && li1.Rows.Count > 0)

                {
                    List<Dictionary<string, object>> RewardDetails = new List<Dictionary<string, object>>();

                    foreach (DataRow dr in li1.Rows)
                    {
                        Dictionary<string, object> Reward_Details = new Dictionary<string, object>();
                        Reward_Details["Currency_type"] = dr["Currency_type"];
                        Reward_Details["transactionID"] = dr["Transaction_ID"];
                        Reward_Details["discounttypeID"] = dr["Discounttype_ID"];
                        Reward_Details["walletID"] = dr["Wallet_ID"];
                        Reward_Details["amountInGBP"] = dr["AmountInGBP"];
                        Reward_Details["discountAmount"] = dr["Discount_Amount"];
                        Reward_Details["discountID"] = dr["Discount_Id"];
                        Reward_Details["walletAmount"] = dr["Wallet_Amount"];
                        Reward_Details["discountCode"] = dr["Discount_Code"];
                        Reward_Details["walletBalance"] = dr["Wallet_balance"];
                        Reward_Details["Currency_ID"] = dr["Currency_ID"];
                        Reward_Details["Currency_Code"] = dr["Currency_Code"];

                        RewardDetails.Add(Reward_Details);

                    }
                    var jsonData1 = new { response = true, responseCode = "00", data = RewardDetails };
                    return new JsonResult(jsonData1);

                }
                else
                {
                    var jsonData1 = new { response = false, responseCode = "02", data = "No Records Found" };
                    return new JsonResult(jsonData1);
                }
            }
            catch (Exception ex)
            {
                string Activity = ex.ToString();
                validateJsonData = new { response = false, responseCode = "01", data = "Invalid Request" };
                CompanyInfo.InsertErrorLogTracker(Activity.ToString(), 0, 0, 0, 0, "getrewarddetails", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "", context);
                return new JsonResult(validateJsonData);
            }

        }

        

        [HttpPost]
        [Route("getinvitedetails")]
        public IActionResult Get_InviteDetails([FromBody] JsonElement Obj)
        {
            HttpContext context = HttpContext;
            Model.response.WebResponse response = null;
            var validateJsonData = (dynamic)null;
            string json1 = System.Text.Json.JsonSerializer.Serialize(Obj);
            dynamic data = JObject.Parse(json1);
            CompanyInfo.InsertrequestLogTracker("getinvitedetails full request body: " + JObject.Parse(json1), 0, 0, 0, 0, "Get_InviteDetails", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
            Model.Transaction obj = new Model.Transaction();
            try
            {
                if (!SqlInjectionProtector.ReadJsonElementValues(Obj) || !SqlInjectionProtector.ReadJsonElementValuesScript(Obj))
                {
                    var errorResponse = new { response = false, responseCode = "02", data = "Invalid input detected." };
                    return new JsonResult(errorResponse) { StatusCode = 400 };
                }

                if (data.Client_ID == "" || data.Client_ID == null)
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "Invalid Client ID." };
                    return new JsonResult(validateJsonData);
                }
                obj.Client_ID = data.Client_ID;

                List<Model.Transaction> _lst = new List<Model.Transaction>();
                Service.srvDiscount srv = new Service.srvDiscount();
                _lst = srv.GetInviteDetails(obj);
                if (_lst != null)
                {                   
                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_SUCCESS);
                    response.data = "success";
                    response.ObjData = _lst;
                    response.ResponseCode = 0;

                    Service.ListtoDataTableConverter converter = new Service.ListtoDataTableConverter();
                    DataTable li1 = converter.ToDataTable(_lst);

                    var relationshipData = li1.Rows.OfType<DataRow>()
               .Select(row => li1.Columns.OfType<DataColumn>()
                   .ToDictionary(col => col.ColumnName, c => row[c].ToString()));

                    validateJsonData = new { response = true, responseCode = "00", data = relationshipData };

                    return new JsonResult(validateJsonData);
                }
                else
                {
                    var jsonData1 = new { response = false, responseCode = "02", data = "No Records Found" };
                    return new JsonResult(jsonData1);
                }


            }
            catch (Exception ex)
            {
                string Activity = ex.ToString();
                validateJsonData = new { response = false, responseCode = "02", data = "No Records Found" };
                CompanyInfo.InsertErrorLogTracker("Get_InviteDetails error: "+Activity.ToString(), 0, 0, 0, 0, "Get_InviteDetails", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "", context);
                return new JsonResult(validateJsonData);
            }
        }

        [HttpPost]        
        [Route("getbonusamountcustomerprofile")]
        public IActionResult GetBonusAmountCustomerprofile([FromBody] JsonElement Obj)
        {
            HttpContext context = HttpContext;
            Model.response.WebResponse response = null;
            var validateJsonData = (dynamic)null;
            string json1 = System.Text.Json.JsonSerializer.Serialize(Obj);
            dynamic data = JObject.Parse(json1);
            CompanyInfo.InsertrequestLogTracker("getbonusamountcustomerprofile full request body: " + JObject.Parse(json1), 0, 0, 0, 0, "GetBonusAmountCustomerprofile", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
            var claimsIdentity = User.Identity as ClaimsIdentity;//sanket
            var authHeader = "";
            try { authHeader = HttpContext.Request.Headers["Authorization"].ToString(); } catch (Exception egx) { authHeader = ""; }

            bool auth = AuthController.checkAuth(claimsIdentity, Obj, Convert.ToString(authHeader));
            if (!auth)
            {
                var errorResponse = new { response = false, responseCode = "403", data = "Access Forbidden" };
                return new JsonResult(errorResponse) { StatusCode = (int)HttpStatusCode.Forbidden };
            }
            try
            {
                if (!SqlInjectionProtector.ReadJsonElementValues(Obj) || !SqlInjectionProtector.ReadJsonElementValuesScript(Obj))
                {
                    var errorResponse = new { response = false, responseCode = "02", data = "Invalid input detected." };
                    return new JsonResult(errorResponse) { StatusCode = 400 };
                }

                Transaction obj = JsonConvert.DeserializeObject<Transaction>(json1);
                Service.srvWallet srv = new Service.srvWallet();
                DataTable li1 = srv.GetRewardAmountCustomerprofile(obj);
                
                if (li1 != null && li1.Rows.Count > 0)
                {
                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_SUCCESS);
                    response.data = "success";
                    response.ObjData = li1;
                    response.ResponseCode = 0;

                    var relationshipData = li1.Rows.OfType<DataRow>()
               .Select(row => li1.Columns.OfType<DataColumn>()
                   .ToDictionary(col => col.ColumnName, c => row[c].ToString()));
                    validateJsonData = new { response = true, responseCode = "00", data = relationshipData };
                    return new JsonResult(validateJsonData);
                }
                else
                {
                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_TECHNICAL_ERROR);
                    response.ResponseMessage = "No Records Found.";
                    response.ResponseCode = 0;
                    var jsonData1 = new { response = false, responseCode = "02", data = "No Records Found." };
                    return new JsonResult(jsonData1);
                }
            }
            catch (Exception ex)
            {
                string Activity = ex.ToString();
                validateJsonData = new { response = false, responseCode = "01", data = "Invalid Request" };
                CompanyInfo.InsertErrorLogTracker(Activity.ToString(), 0, 0, 0, 0, "GetBonusAmountCustomerprofile", Convert.ToInt32(2), Convert.ToInt32(1), "", context);
                return new JsonResult(validateJsonData);
            }

        }

    }
}
