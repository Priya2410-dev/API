using Calyx_Solutions.Model;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Data;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Authorization;
using gudusoft.gsqlparser;
using System.Text.Json;
using System.Net;
using System.Security.Claims;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Calyx_Solutions.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OfferController : ControllerBase
    {
        [HttpPost]
        [Route("getoffers")]
        public IActionResult Get_offers([FromBody] JsonElement objoffer)
        {
            HttpContext context = HttpContext;
            Model.response.WebResponse response = null;
            string json = System.Text.Json.JsonSerializer.Serialize(objoffer);
            dynamic data = JObject.Parse(json);
            CompanyInfo.InsertrequestLogTracker("getoffers full request body: " + JObject.Parse(json), 0, 0, 0, 0, "Get_offers", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
            var claimsIdentity = User.Identity as ClaimsIdentity;//sanket
            var authHeader = "";
            try { authHeader = HttpContext.Request.Headers["Authorization"].ToString(); } catch (Exception egx) { authHeader = ""; }

            bool auth = AuthController.checkAuth(claimsIdentity, objoffer, Convert.ToString(authHeader));
            if (!auth)
            {
                var errorResponse = new { response = false, responseCode = "403", data = "Access Forbidden" };
                return new JsonResult(errorResponse) { StatusCode = (int)HttpStatusCode.Forbidden };
            }
            var validateJsonData = (dynamic)null;
            
            Offer obj = JsonConvert.DeserializeObject<Offer>(json);
            try
            {
                if (!SqlInjectionProtector.ReadJsonElementValues(objoffer) || !SqlInjectionProtector.ReadJsonElementValuesScript(objoffer))
                {
                    var errorResponse = new { response = false, responseCode = "02", data = "Invalid input detected." };
                    return new JsonResult(errorResponse) { StatusCode = 400 };
                }

                Service.SrvOffers srv = new Service.SrvOffers();

                DataTable dt = srv.Get_offers(obj, HttpContext);
                if (dt.Rows.Count > 0)
                {
                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_SUCCESS);
                    response.data = "success";
                    response.ObjData = dt;
                    response.ResponseCode = 0;
                    var responseData = dt.Rows.OfType<DataRow>()
                    .Select(row => dt.Columns.OfType<DataColumn>()
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
                Model.ErrorLog objError = new Model.ErrorLog();
                objError.User = new Model.User();
                objError.Branch = new Model.Branch();
                objError.Client = new Model.Client();

                objError.Error = "Api : Get_offers --" + ex.ToString();
                objError.Branch.Id = 1;
                objError.Date = DateTime.Now;
                objError.User.Id = 1;
                objError.Id = 1;
                objError.Function_Name = "Get_offers";
                Service.srvErrorLog srvError = new Service.srvErrorLog();
                srvError.Create(objError, HttpContext);

                validateJsonData = new { response = false, responseCode = "01", data = "Invalid Request" };                
                return new JsonResult(validateJsonData);
            }
        }

        [HttpPost]
        [Route("checkspinlimit")]
        public IActionResult check_spin_limit([FromBody] JsonElement objoffer)
        {
            HttpContext context = HttpContext;
            Model.response.WebResponse response = null;
            string json = System.Text.Json.JsonSerializer.Serialize(objoffer);
            dynamic data = JObject.Parse(json);
            CompanyInfo.InsertrequestLogTracker("checkspinlimit full request body: " + JObject.Parse(json), 0, 0, 0, 0, "check_spin_limit", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
            var claimsIdentity = User.Identity as ClaimsIdentity;//sanket
            var authHeader = "";
            try { authHeader = HttpContext.Request.Headers["Authorization"].ToString(); } catch (Exception egx) { authHeader = ""; }

            bool auth = AuthController.checkAuth(claimsIdentity, objoffer, Convert.ToString(authHeader));
            if (!auth)
            {
                var errorResponse = new { response = false, responseCode = "403", data = "Access Forbidden" };
                return new JsonResult(errorResponse) { StatusCode = (int)HttpStatusCode.Forbidden };
            }
            var validateJsonData = (dynamic)null;
            Offer obj = JsonConvert.DeserializeObject<Offer>(json);
            try
            {
                if (!SqlInjectionProtector.ReadJsonElementValues(objoffer) || !SqlInjectionProtector.ReadJsonElementValuesScript(objoffer))
                {
                    var errorResponse = new { response = false, responseCode = "02", data = "Invalid input detected." };
                    return new JsonResult(errorResponse) { StatusCode = 400 };
                }

                Service.SrvOffers srv = new Service.SrvOffers();
                obj = srv.check_spin_limit(obj, context);
                if (obj.Spin_Id >= 0)
                {
                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_SUCCESS);
                    response.data = "success";
                    response.ObjData = obj;
                    response.ResponseCode = 0;
                   
                    validateJsonData = new { response = true, responseCode = "00", data = obj };
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
                Model.ErrorLog objError = new Model.ErrorLog();
                objError.User = new Model.User();
                objError.Branch = new Model.Branch();
                objError.Client = new Model.Client();

                objError.Error = "Api : checkspinlimit --" + ex.ToString();
                objError.Branch.Id = 1;
                objError.Date = DateTime.Now;
                objError.User.Id = 1;
                objError.Id = 1;
                objError.Function_Name = "check_spin_limit";
                Service.srvErrorLog srvError = new Service.srvErrorLog();
                srvError.Create(objError, HttpContext);
                validateJsonData = new { response = false, responseCode = "01", data = "Invalid Request" };
                return new JsonResult(validateJsonData);
            }
        }

        [HttpPost]
        [Route("checkoffer")]
        public IActionResult Check_offer([FromBody] JsonElement objoffer)
        {
            HttpContext context = HttpContext;
            Model.response.WebResponse response = null;
            string json = System.Text.Json.JsonSerializer.Serialize(objoffer);
            dynamic data = JObject.Parse(json);
            CompanyInfo.InsertrequestLogTracker("checkoffer full request body: " + JObject.Parse(json), 0, 0, 0, 0, "Check_offer", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
            var claimsIdentity = User.Identity as ClaimsIdentity;//sanket
            var authHeader = "";
            try { authHeader = HttpContext.Request.Headers["Authorization"].ToString(); } catch (Exception egx) { authHeader = ""; }

            bool auth = AuthController.checkAuth(claimsIdentity, objoffer, Convert.ToString(authHeader));
            if (!auth)
            {
                var errorResponse = new { response = false, responseCode = "403", data = "Access Forbidden" };
                return new JsonResult(errorResponse) { StatusCode = (int)HttpStatusCode.Forbidden };
            }
            var validateJsonData = (dynamic)null;
            Offer obj = JsonConvert.DeserializeObject<Offer>(json);
            try
            {
                if (!SqlInjectionProtector.ReadJsonElementValues(objoffer) || !SqlInjectionProtector.ReadJsonElementValuesScript(objoffer))
                {
                    var errorResponse = new { response = false, responseCode = "02", data = "Invalid input detected." };
                    return new JsonResult(errorResponse) { StatusCode = 400 };
                }
                Service.SrvOffers srv = new Service.SrvOffers();
               
                DataTable offer_list = new DataTable();
                offer_list = srv.Check_offer(obj);

                if (offer_list != null)
                {
                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_SUCCESS);
                    response.data = "success";
                    response.ObjData = offer_list;
                    response.ResponseCode = 0;

                    var responseData = offer_list.Rows.OfType<DataRow>()
                    .Select(row => offer_list.Columns.OfType<DataColumn>()
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
                Model.ErrorLog objError = new Model.ErrorLog();
                objError.User = new Model.User();
                objError.Branch = new Model.Branch();
                objError.Client = new Model.Client();

                objError.Error = "Api : checkoffer --" + ex.ToString();
                objError.Branch.Id = 1;
                objError.Date = DateTime.Now;
                objError.User.Id = 1;
                objError.Id = 1;
                objError.Function_Name = "Check_offer";
                Service.srvErrorLog srvError = new Service.srvErrorLog();
                srvError.Create(objError, context);
                validateJsonData = new { response = false, responseCode = "01", data = "Invalid Request" };
                return new JsonResult(validateJsonData);
            }
        }

        [HttpPost]
        [Route("bindspinconfig")]
        public IActionResult bind_spin_config([FromBody] JsonElement objoffer)
        {
            HttpContext context = HttpContext;
            Model.response.WebResponse response = null;
            string json = System.Text.Json.JsonSerializer.Serialize(objoffer);
            dynamic data = JObject.Parse(json);
            CompanyInfo.InsertrequestLogTracker("bindspinconfig full request body: " + JObject.Parse(json), 0, 0, 0, 0, "bind_spin_config", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
            var validateJsonData = (dynamic)null;
            Offer obj = JsonConvert.DeserializeObject<Offer>(json);
            try
            {
                if (!SqlInjectionProtector.ReadJsonElementValues(objoffer) || !SqlInjectionProtector.ReadJsonElementValuesScript(objoffer))
                {
                    var errorResponse = new { response = false, responseCode = "02", data = "Invalid input detected." };
                    return new JsonResult(errorResponse) { StatusCode = 400 };
                }
                Service.SrvOffers srv = new Service.SrvOffers();

                DataTable dt = srv.bind_spin_config(obj, context);
                if (dt.Rows.Count > 0)
                {
                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_SUCCESS);
                    response.data = "success";
                    response.ObjData = dt;
                    response.ResponseCode = 0;

                    var responseData = dt.Rows.OfType<DataRow>()
                    .Select(row => dt.Columns.OfType<DataColumn>()
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
                Model.ErrorLog objError = new Model.ErrorLog();
                objError.User = new Model.User();
                objError.Branch = new Model.Branch();
                objError.Client = new Model.Client();

                objError.Error = "Api : bindspinconfig --" + ex.ToString();
                objError.Branch.Id = 1;
                objError.Date = DateTime.Now;
                objError.User.Id = 1;
                objError.Id = 1;
                objError.Function_Name = "bind_spin_config";
                Service.srvErrorLog srvError = new Service.srvErrorLog();
                srvError.Create(objError, context);
                validateJsonData = new { response = false, responseCode = "01", data = "Invalid Request" };
                return new JsonResult(validateJsonData);
            }

        }

        [HttpPost]
        [Route("getwalletconfiguration")]

        public IActionResult Get_wallet_configuration([FromBody] JsonElement obj1)
        {
            HttpContext context = HttpContext;
            Model.response.WebResponse response = null;
            string json = System.Text.Json.JsonSerializer.Serialize(obj1);
            dynamic data = JObject.Parse(json);
            CompanyInfo.InsertrequestLogTracker("getwalletconfiguration full request body: " + JObject.Parse(json), 0, 0, 0, 0, "Get_wallet_configuration", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
            var validateJsonData = (dynamic)null;
            Offer obj = new Offer();

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
                if (data.transactionID == "" || data.transactionID == null)
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "Invalid Transaction ID." };
                    return new JsonResult(validateJsonData);
                }
                if (data.baseCurrencyCode == "" || data.baseCurrencyCode == null)
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "Invalid Basecurrecy Code." };
                    return new JsonResult(validateJsonData);
                }


                obj.Client_ID = data.clientID;
                obj.Branch_ID = data.branchID;
                obj.Transaction_ID = data.transactionID;
                obj.Base_currency_code = data.baseCurrencyCode;
                obj.Base_currency_id = data.baseCurrencyID;
                obj.Foreign_currency_id = data.foreignCurrencyID;

                Service.SrvOffers srv = new Service.SrvOffers();

                DataTable dt = srv.Get_wallet_configuration(obj , context);

                var relationshipData = dt.Rows.OfType<DataRow>()
              .Select(row => dt.Columns.OfType<DataColumn>()
                  .ToDictionary(col => col.ColumnName, c => row[c].ToString()));
                validateJsonData = new { response = true, responseCode = "00", data = relationshipData };
                return new JsonResult(validateJsonData);

                if (dt.Rows.Count > 0)
                {
                    List<Dictionary<string, object>> trasaction = new List<Dictionary<string, object>>();

                    foreach (DataRow dr in dt.Rows)
                    {
                        Dictionary<string, object> transaction_history = new Dictionary<string, object>();
                        transaction_history["Id"] = dr["Id"];
                        transaction_history["currecnyId"] = dr["Currency_Id"];
                        transaction_history["currecnyType"] = dr["Currency_type"];
                        transaction_history["walletUsageLimitPerTxn"] = dr["wallet_usage_lmt_per_txn"];
                        transaction_history["amountType"] = dr["Amt_type"];
                        transaction_history["deleteStatus"] = dr["Delete_Status"];
                        transaction_history["clientID"] = dr["Client_Id"];
                        transaction_history["walletApply"] = dr["Wallet_apply"];
                        transaction_history["perkValue"] = dr["Perk_value"];
                        transaction_history["perkUsageLimit"] = dr["Perk_usage_limit"];
                        trasaction.Add(transaction_history);
                    }

                    validateJsonData = new { response = true, responseCode = "00", data = trasaction };
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
                validateJsonData = new { response = false, responseCode = "01", data = "Invalid Request" };
                CompanyInfo.InsertErrorLogTracker(ex.ToString(), 0, 0, 0, 0, "getwalletconfiguration", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "", context);
                return new JsonResult(validateJsonData);
            }
            //return new JsonResult(validateJsonData);
        }




    }
}
