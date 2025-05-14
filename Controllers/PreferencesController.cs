using Calyx_Solutions.Model;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Data;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Authorization;
using FirebaseAdmin.Messaging;
using System.Net;
using System.Security.Claims;
using System.Text.Json;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Calyx_Solutions.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PreferencesController : ControllerBase
    {

        // POST api/<PreferencesController>
        [HttpPost]
        [Authorize]
        [Route("selectpreferences")]
        public IActionResult SelectPreferences([FromBody] JsonObject Obj)
        {
            Model.response.WebResponse response = null;
            string json = System.Text.Json.JsonSerializer.Serialize(Obj);
            dynamic data = JObject.Parse(json);
            CompanyInfo.InsertrequestLogTracker("selectpreferences full request body: " + JObject.Parse(json ), 0, 0, 0, 0, "SelectPreferences", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
            JsonElement jsonElement = System.Text.Json.JsonSerializer.SerializeToElement(Obj);//sanket // Converted JsonObject to JsonElement
            var claimsIdentity = User.Identity as ClaimsIdentity;//sanket
            var authHeader = "";
            try { authHeader = HttpContext.Request.Headers["Authorization"].ToString(); } catch (Exception egx) { authHeader = ""; }

            bool auth = AuthController.checkAuth(claimsIdentity, jsonElement, Convert.ToString(authHeader));
            if (!auth)
            {
                var errorResponse = new { response = false, responseCode = "403", data = "Access Forbidden" };
                return new JsonResult(errorResponse) { StatusCode = (int)HttpStatusCode.Forbidden };
            }
            var validateJsonData = (dynamic)null;
            HttpContext context = HttpContext;
            Communication_Prefer obj = JsonConvert.DeserializeObject<Communication_Prefer>(json);
            string status = "";
            List<Model.Communication_Prefer> _lst = new List<Communication_Prefer>();
            try
            {
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

                if (data.customerID == "" || data.customerID == null)
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "Invalid customerID field." };
                    return new JsonResult(validateJsonData);
                }
                obj.Client_ID = data.clientID;
                obj.Customer_ID = data.customerID;

                if (!SqlInjectionProtector.ValidateObjectForSqlInjection(obj) || !SqlInjectionProtector.ValidateObjectForScriptSqlInjection(obj))
                {
                    return new JsonResult(validateJsonData) { StatusCode = StatusCodes.Status400BadRequest, Value = "Invalid input detected." };
                }

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

                Service.srvPreference srv = new Service.srvPreference();
                _lst = srv.Select_Preference(obj);
               
            }
            catch (Exception ex)
            {
                Model.ErrorLog objError = new Model.ErrorLog();
                objError.User = new Model.User();
                objError.Branch = new Model.Branch();
                objError.Client = new Model.Client();

                objError.Error = "Api : SelectPreferences --" + ex.ToString();
                objError.Branch.Id = 1;
                objError.Date = DateTime.Now;
                objError.User.Id = 1;
                objError.Id = 1;
                objError.Function_Name = "Select_Preference";
                Service.srvErrorLog srvError = new Service.srvErrorLog();
                srvError.Create(objError, HttpContext);
                validateJsonData = new { response = false, responseCode = "01", data = "Invalid Request" };
                CompanyInfo.InsertErrorLogTracker("SelectPreferences Error:"+ex.ToString(), 0, 0, 0, 0, "SelectPreferences", obj.Branch_ID, obj.Client_ID, "", HttpContext);
                return new JsonResult(validateJsonData);
            }
            var selectedColumns = _lst.Select(item => new {
                clientID = item.Client_ID,
                cpID = item.CP_ID,
                customerID = CompanyInfo.Encrypt(Convert.ToString(item.Customer_ID).Trim(), true) ,
                preferenceID = item.Comm_preference_ID,
                preferenceStatus = item.Comm_Preference_Status
            }).ToList();

            validateJsonData = new { response = true, responseCode = "00", data = selectedColumns };
            return new JsonResult(validateJsonData);
        }


        [HttpPost]
       // [Authorize]
        [Route("updatepreferences")]
        public IActionResult UpdatePreferences([FromBody] JsonObject Obj)
        {
            Model.response.WebResponse response = null;
            string json = System.Text.Json.JsonSerializer.Serialize(Obj);
            dynamic data = JObject.Parse(json);
            CompanyInfo.InsertrequestLogTracker("updatepreferences full request body: " + JObject.Parse(json), 0, 0, 0, 0, "UpdatePreferences", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
            JsonElement jsonElement = System.Text.Json.JsonSerializer.SerializeToElement(Obj);//sanket // Converted JsonObject to JsonElement
            var claimsIdentity = User.Identity as ClaimsIdentity;//sanket
            var authHeader = "";
            try { authHeader = HttpContext.Request.Headers["Authorization"].ToString(); } catch (Exception egx) { authHeader = ""; }

            bool auth = AuthController.checkAuth(claimsIdentity, jsonElement, Convert.ToString(authHeader));
            if (!auth)
            {
                var errorResponse = new { response = false, responseCode = "403", data = "Access Forbidden" };
                return new JsonResult(errorResponse) { StatusCode = (int)HttpStatusCode.Forbidden };
            }
            var validateJsonData = (dynamic)null;
            HttpContext context = HttpContext;
            Communication_Prefer obj = JsonConvert.DeserializeObject<Communication_Prefer>(json);
            string status = "";
            try
            {
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
                if (data.customerID == "" || data.customerID == null)
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "Invalid customerID field." };
                    return new JsonResult(validateJsonData);
                }

                obj.Client_ID = data.clientID;
                obj.Customer_ID = data.customerID;
                obj.CP_ID_Email = data.commPreferenceIDEmail;
                obj.CP_ID_Phone = data.commPreferenceIDPhone;
                obj.CP_ID_SMS = data.commPreferenceIDSMS;
                obj.Comm_Preference_Status_Email = data.commPreferenceStatusEmail;
                obj.Comm_Preference_Status_Phone = data.commPreferenceStatusPhone;
                obj.Comm_Preference_Status_SMS = data.commPreferenceStatusSMS;
                obj.Comm_preference_ID_Email = data.cpIDEmail;
                obj.Comm_preference_ID_Phone = data.cpIDPhone;
                obj.Comm_preference_ID_SMS = data.cpIDSMS;

                Service.srvPreference srvPreference = new Service.srvPreference();
                response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_SUCCESS);
                //response.data = srvPreference.Update_Comm_Preference(obj, context);
                response.data = srvPreference.Update_Comm_Preference_new(obj, context);

                if (response.sStatusType == "success")
                validateJsonData = new { response = true, responseCode = "00", data = response.sStatusType};
                else
                validateJsonData = new { response = true, responseCode = "02", data = response.sStatusType };

                return new JsonResult(validateJsonData); // Return the response as JSON
            }
            catch (Exception ex)
            {
                response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_TECHNICAL_ERROR);
                response.sErrorExceptionText = ex.ToString();
                response.ResponseMessage = "Technical error";
                response.ResponseCode = 3;

                // Error Log Handled
                Model.ErrorLog objError = new Model.ErrorLog();
                objError.User = new Model.User();
                objError.Error = "Api : UpdatePreferences --" + ex.ToString();
                objError.Date = DateTime.Now;
                objError.User_ID = 1;
                objError.Client_ID = 1;
                objError.Function_Name = "UpdatePreferences";
                Service.srvErrorLog srvError = new Service.srvErrorLog();
                srvError.Create(objError, context);

                validateJsonData = new { response = false, responseCode = "01", data = response.ResponseMessage };
                CompanyInfo.InsertErrorLogTracker("UpdatePreferences Error :"+ ex.ToString(), 0, 0, 0, 0, "UpdatePreferences", 0, 1, "", context);
                return new JsonResult(validateJsonData);
            }
        }

    }





}
