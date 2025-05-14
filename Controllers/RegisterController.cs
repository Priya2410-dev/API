using Microsoft.AspNetCore.Mvc;
using Calyx_Solutions.Model;
using Calyx_Solutions.Service;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Text.Json;
 
using System.Net;
using System.Security.Claims;
using System.Web;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Calyx_Solutions.Controllers
{
    

    [Route("api/[controller]")]
    [ApiController]
    public class RegisterController : ControllerBase
    {
        [HttpPost]
        [Route("savetoken")]
        public IActionResult saveToken([FromBody] JsonElement objdata)
        {
            HttpContext context = HttpContext;
            Model.response.WebResponse response = null;
            string json = System.Text.Json.JsonSerializer.Serialize(objdata);
            dynamic data = JObject.Parse(json);
            CompanyInfo.InsertrequestLogTracker("savetoken full request body: " + JObject.Parse(json), 0, 0, 0, 0, "saveToken", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
            var validateJsonData = (dynamic)null;
            Customer obj = JsonConvert.DeserializeObject<Customer>(json);
            try
            {
                /*if (!SqlInjectionProtector.ReadJsonElementValues(objdata) || !SqlInjectionProtector.ReadJsonElementValuesScript(objdata))
                {
                    var errorResponse = new { response = false, responseCode = "02", data = "Invalid input detected." };
                    return new JsonResult(errorResponse) { StatusCode = 400 };
                }*/

                Service.srvCustomer srv = new Service.srvCustomer();
                int a = srv.saveToken(obj);

                if (a > 0)
                {                    
                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_SUCCESS);
                    response.ResponseMessage = "Saved";                                                            
                }
                else
                {
                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_SUCCESS);
                    response.ResponseMessage = "Exist";                                                                                
                }
                validateJsonData = new { response = true, responseCode = "00", data = response.ResponseMessage };
                return new JsonResult(validateJsonData);
            }
            catch (Exception ex)
            {
                response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_TECHNICAL_ERROR);
                response.sErrorExceptionText = ex.ToString();
                response.ResponseMessage = "technical error";

                //Error Log Handled
                Model.ErrorLog objError = new Model.ErrorLog();
                objError.User = new Model.User();

                objError.Error = "Api : saveToken --" + ex.ToString();
                objError.Date = DateTime.Now;
                objError.User_ID = 1;
                objError.Id = 1;
                objError.Function_Name = "saveToken";
                Service.srvErrorLog srvError = new Service.srvErrorLog();
                srvError.Create(objError, context);
                validateJsonData = new { response = false, responseCode = "01", data = "Invalid Request" };
                return validateJsonData;
            }
        }

        [HttpPost]
        [Route("bindcurrencywiseannualsalary")]
        public IActionResult Bind_Currencywise_Annual_Salary([FromBody] JsonElement obj)
        {
            HttpContext context = HttpContext;
            Model.response.WebResponse response = null;
            string json = System.Text.Json.JsonSerializer.Serialize(obj);
            dynamic data = JObject.Parse(json);
            CompanyInfo.InsertrequestLogTracker("bindcurrencywiseannualsalary full request body: " + JObject.Parse(json), 0, 0, 0, 0, "bindcurrencywiseannualsalary", Convert.ToInt32(0), Convert.ToInt32(0), "", context);
            var validateJsonData = (dynamic)null;
            Country Obj = JsonConvert.DeserializeObject<Country>(json);
            try
            {
                Service.srvCountry srv = new Service.srvCountry();
                var result = srv.Bind_Currencywise_Annual_Salary(Obj);
                if (result != null)
                {
                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_SUCCESS);
                    response.data = "success";
                    response.ObjData = result;
                    response.ResponseCode = 0;
                    validateJsonData = new { response = true, responseCode = "00", data = response.ObjData };
                }
                else
                {
                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_TECHNICAL_ERROR);
                    response.ResponseMessage = "No Records Found.";
                    response.ResponseCode = 0;
                    validateJsonData = new { response = false, responseCode = "02", data = response.ObjData };
                }
            }
            catch (Exception ex)
            {
                Model.ErrorLog objError = new Model.ErrorLog();
                objError.User = new Model.User();
                objError.Branch = new Model.Branch();
                objError.Client = new Model.Client();

                objError.Error = "Api : Select_Salary --" + ex.ToString();
                objError.Branch_ID = 1;
                objError.Date = DateTime.Now;
                objError.User_ID = 1;
                objError.Id = 1;
                objError.Function_Name = "Select_Salary";
                Service.srvErrorLog srvError = new Service.srvErrorLog();
                srvError.Create(objError, context);
                validateJsonData = new { response = false, responseCode = "02", data = "No Records Found." };
            }
            return new JsonResult(validateJsonData);
        }

        [HttpPost]
        [Route("checkmobilenoisexist")]
        public IActionResult check_MobileNo_Is_Exist([FromBody] JsonElement obj)
        {
            HttpContext context = HttpContext;
            Model.response.WebResponse response = null;
            string json = System.Text.Json.JsonSerializer.Serialize(obj);
            dynamic data = JObject.Parse(json);
            CompanyInfo.InsertrequestLogTracker("checkmobilenoisexist full request body: " + JObject.Parse(json), 0, 0, 0, 0, "checkmobilenoisexist", Convert.ToInt32(0), Convert.ToInt32(0), "", context);
            var validateJsonData = (dynamic)null;
            Customer Obj = JsonConvert.DeserializeObject<Customer>(json);

            try
            {
                Service.srvCustomer srvCustomer = new Service.srvCustomer();
                response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_SUCCESS);
                var result = srvCustomer.check_MobileNo_Is_Exist(Obj);
                if (result != null)
                {
                    validateJsonData = new { response = true, responseCode = "00", data = result };
                }
                else
                {
                    validateJsonData = new { response = true, responseCode = "00", data = "No Records Found" };
                }
            }
            catch (Exception ex)
            {
                response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_TECHNICAL_ERROR);
                response.sErrorExceptionText = ex.Message.ToString();
                response.ResponseMessage = "technical error";

                //Error Log Handled
                Model.ErrorLog objError = new Model.ErrorLog();
                objError.User = new Model.User();

                objError.Error = "Api : check_MobileNo_Is_Exist --" + ex.ToString();
                objError.Date = DateTime.Now;
                objError.User_ID = 1;
                objError.Id = 1;
                objError.Function_Name = "check_MobileNo_Is_Exist";
                Service.srvErrorLog srvError = new Service.srvErrorLog();
                srvError.Create(objError, context);

                validateJsonData = new { response = true, responseCode = "00", data = "No Records Found" };
            }
            return new JsonResult(validateJsonData);
        }

        [HttpPost]
        [Route("emailflag")]
        public IActionResult email_flag([FromBody] JsonElement obj)
        {
            HttpContext context = HttpContext;
            Model.response.WebResponse response = null;
            string json = System.Text.Json.JsonSerializer.Serialize(obj);
            dynamic data = JObject.Parse(json);
            CompanyInfo.InsertrequestLogTracker("emailflag full request body: " + JObject.Parse(json), 0, 0, 0, 0, "emailflag", Convert.ToInt32(0), Convert.ToInt32(0), "", context);
            var validateJsonData = (dynamic)null;

            if (!SqlInjectionProtector.ReadJsonElementValues(obj) || !SqlInjectionProtector.ReadJsonElementValuesScript(obj))
            {
                var errorResponse = new { response = false, responseCode = "02", data = "Invalid input detected." };
                return new JsonResult(errorResponse) { StatusCode = 400 };
            }
            try
            {
                // Register customer
                Customer Obj = JsonConvert.DeserializeObject<Customer>(json);
                Service.srvCustomer srvCustomer = new Service.srvCustomer();
                Obj.Customer_ID = CompanyInfo.Decrypt(Convert.ToString(Obj.Customer_ID), true);
                var result = srvCustomer.Verify_Email(Obj, context);

                try
                {
                    CompanyInfo.check_location(Obj.Client_ID, "Verified_Mail", context);
                }
                catch (Exception ex)
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "Invalid Request" };
                }
                validateJsonData = new { response = true, responseCode = "00", data = result  };
            }
            catch (Exception ex)
            {
                response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_TECHNICAL_ERROR);
                response.sErrorExceptionText = ex.ToString();
                response.ResponseMessage = "technical error";

                //Error Log Handled
                Model.ErrorLog objError = new Model.ErrorLog();
                objError.User = new Model.User();

                objError.Error = "Api : email_flag --" + ex.ToString();
                objError.Date = DateTime.Now;
                objError.User_ID = 1;
                objError.Id = 1;
                objError.Function_Name = "email_flag";
                Service.srvErrorLog srvError = new Service.srvErrorLog();
                srvError.Create(objError, context);

                validateJsonData = new { response = false, responseCode = "02", data = "Invalid Request" };
            }
            return new JsonResult(validateJsonData);
        }

        // DELETE api/<RegisterController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        [HttpPost]
        [Route("securestep2")]
        public IActionResult Secure_Step2([FromBody] JsonElement obj)
        {
            HttpContext context = HttpContext;
            Model.response.WebResponse response = null;
            string json = System.Text.Json.JsonSerializer.Serialize(obj);
            dynamic data = JObject.Parse(json);
            _ = Task.Run(() => CompanyInfo.InsertrequestLogTracker("emailflag full request body: " + JObject.Parse(json), 0, 0, 0, 0, "emailflag", Convert.ToInt32(0), Convert.ToInt32(0), "", context));
            var validateJsonData = (dynamic)null;
            if (!SqlInjectionProtector.ReadJsonElementValues(obj) || !SqlInjectionProtector.ReadJsonElementValuesScript(obj))
            {
                var errorResponse = new { response = false, responseCode = "02", data = "Invalid input detected." };
                return new JsonResult(errorResponse) { StatusCode = 400 };
            }
            try
            {
                // Register customer
                Customer Obj = JsonConvert.DeserializeObject<Customer>(json);
                Service.srvCustomer srvCustomer = new Service.srvCustomer();
                response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_SUCCESS);
                DataTable dtc = CompanyInfo.get(Obj.Client_ID, context);
                string cust_url = Convert.ToString(dtc.Rows[0]["Company_URL_Customer"]);
                DateTime date = DateTime.Now;
                DateTime date3 = date.AddMinutes(15);
                String Encrypted_time1 = Convert.ToString(CompanyInfo.Encrypt(Convert.ToString(date3), true));
                string from_querystring = Convert.ToString(Obj.WireTransfer_ReferanceNo);
                string split_string = "", ref_no = "";
                if (from_querystring != "" && from_querystring != null)
                {
                    split_string = from_querystring.Replace("reference=", "");
                    //ref_no = CompanyInfo.Decrypt(split_string, Convert.ToBoolean(1));
                    ref_no = CompanyInfo.Encrypt(split_string, true);

                    ref_no = CompanyInfo.Decrypt(split_string, true);
                }
                Obj.WireTransfer_ReferanceNo = ref_no;
                string s2 = Convert.ToString(Obj.WireTransfer_ReferanceNo);
                string str_de2 = Convert.ToString(CompanyInfo.Encrypt(s2, true));

                string flags = Convert.ToString(CompanyInfo.Encrypt("SecureFlagOff", true));
                string url = cust_url + "new-password?reference=" + HttpUtility.UrlEncode(str_de2) + "&checkparam=" + HttpUtility.UrlEncode(Encrypted_time1) + "&flag=" + HttpUtility.UrlEncode(flags);
                //_ObjCustomer = srvCustomer.secure_Account(obj);
                validateJsonData = new { response = true, responseCode = "00", data = url };
            }
            catch (Exception ex)
            {
                response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_TECHNICAL_ERROR);
                response.sErrorExceptionText = ex.ToString();
                response.ResponseMessage = "technical error";

                //Error Log Handled
                Model.ErrorLog objError = new Model.ErrorLog();
                objError.User = new Model.User();

                objError.Error = "Api : Secure_Step2 --" + ex.ToString();
                objError.Date = DateTime.Now;
                objError.User_ID = 1;
                objError.Id = 1;
                objError.Function_Name = "Secure_Step2";
                Service.srvErrorLog srvError = new Service.srvErrorLog();
                srvError.Create(objError, context);
                validateJsonData = new { response = false, responseCode = "02", data = "Invalid Request" };
            }
            return new JsonResult(validateJsonData);
        }

        [HttpPost]
        [Route("secureaccount")]
        public IActionResult secure_Account([FromBody] JsonElement obj)
        {
            HttpContext context = HttpContext;
            Model.response.WebResponse response = null;
            string json = System.Text.Json.JsonSerializer.Serialize(obj);
            dynamic data = JObject.Parse(json);
            _ = Task.Run(() => CompanyInfo.InsertrequestLogTracker("emailflag full request body: " + JObject.Parse(json), 0, 0, 0, 0, "emailflag", Convert.ToInt32(0), Convert.ToInt32(0), "", context));
            var validateJsonData = (dynamic)null;
            if (!SqlInjectionProtector.ReadJsonElementValues(obj) || !SqlInjectionProtector.ReadJsonElementValuesScript(obj))
            {
                var errorResponse = new { response = false, responseCode = "02", data = "Invalid input detected." };
                return new JsonResult(errorResponse) { StatusCode = 400 };
            }
            try
            {
                Customer Obj = JsonConvert.DeserializeObject<Customer>(json);
                
                // Register customer
                Model.Customer _ObjCustomer = new Model.Customer();
                Service.srvCustomer srvCustomer = new Service.srvCustomer();
                response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_SUCCESS);
                _ObjCustomer = srvCustomer.secure_Account(Obj, context);
                response.ResponseCode = 0;
                response.ObjData = _ObjCustomer;
                validateJsonData = new { response = true, responseCode = "00", data =  _ObjCustomer};

            }
            catch (Exception ex)
            {
                response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_TECHNICAL_ERROR);
                response.sErrorExceptionText = ex.ToString();
                response.ResponseMessage = "technical error";

                //Error Log Handled
                Model.ErrorLog objError = new Model.ErrorLog();
                objError.User = new Model.User();

                objError.Error = "Api : UpdatePassword --" + ex.ToString();
                objError.Date = DateTime.Now;
                objError.User_ID = 1;
                objError.Id = 1;
                objError.Function_Name = "UpdatePassword";
                Service.srvErrorLog srvError = new Service.srvErrorLog();
                srvError.Create(objError, context);

                validateJsonData = new { response = false, responseCode = "02", data = "Invalid Request" };
            }
            return new JsonResult(validateJsonData);
        }


       
    }
}
