using Auth0.ManagementApi.Models;
using Calyx_Solutions.Model;
using Calyx_Solutions.Service;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
/*using mts.da;*/
using MySql.Data.MySqlClient;
using MySql.Data.MySqlClient.Memcached;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Reflection;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using System.Transactions;
using System.Web;
using System.Web.Helpers;
using WebGrease.Css.Ast;
using static Google.Apis.Requests.BatchRequest;
 

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Calyx_Solutions.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ForgotPasswordController : ControllerBase
    {

        [HttpPost]
        [Route("checkotp")]

        public IActionResult CheckOtp([FromBody] JsonElement obj)
        {
            HttpContext context = HttpContext;
            Model.response.WebResponse response = null;
            //Login obj1 = new Login();
            string otp = ""; string output = ""; string activityLog = ""; String multi_att_activity = ""; string Message = "";
            string json = System.Text.Json.JsonSerializer.Serialize(obj);
            dynamic data = JObject.Parse(json);
            CompanyInfo.InsertrequestLogTracker("checkotp full request body: " + JObject.Parse(json), 0, 0, 0, 0, "CheckOtp", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
            var validateJsonData = (dynamic)null;
            Customer obj1 = JsonConvert.DeserializeObject<Customer>(json);

            if (!SqlInjectionProtector.ReadJsonElementValues(obj) || !SqlInjectionProtector.ReadJsonElementValuesScript(obj))
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
            else if (data.userName == "" || data.userName == null)
            {
                validateJsonData = new { response = false, responseCode = "02", data = "Invalid userName field." };
                return new JsonResult(validateJsonData);
            }
            else if (data.newpassword == "" || data.newpassword == null)
            {
                validateJsonData = new { response = false, responseCode = "02", data = "Invalid password field." };
                return new JsonResult(validateJsonData);
            }
            else if (data.otp == "" || data.otp == null)
            {
                validateJsonData = new { response = false, responseCode = "02", data = "Invalid OTP field." };
                return new JsonResult(validateJsonData);
            }
            else if (!int.TryParse(Convert.ToString(data.branchID), out number) || data.branchID == "" || data.branchID == null)
            {
                validateJsonData = new { response = false, responseCode = "02", data = "Invalid branchID field." };
                return new JsonResult(validateJsonData);
            }

            obj1.UserName = data.userName;
            obj1.Client_ID = data.clientID;
            obj1.Branch_ID = data.branchID;
            obj1.OTP = data.otp;
            obj1.newpassword = data.newpassword;
            obj1.WireTransfer_ReferanceNo = data.referanceNo;
            obj1.checkparam = data.checkparam;

            DataTable dt = new DataTable();

            try
            {
                MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand();
                _cmd = new MySqlConnector.MySqlCommand("sp_check_login");//"select    cast(AES_DECRYPT(UNHEX(u.Password), '" + obj.SecurityKey.Trim() + "' ) as  char(500)) as Password,u.First_Name,u.Last_Name,u.Customer_ID,U.WireTransfer_ReferanceNo,u.Email_ID,u.Branch_ID,u.Client_ID from customer_registration u where    Email_ID = '" + obj.Name.Trim() + "' and Password=HEX(AES_ENCRYPT('" + obj.Password.Trim() + "','" + obj.SecurityKey.Trim() + "'))  and u.delete_status=0 ;");
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.Parameters.AddWithValue("_securityKey", CompanyInfo.SecurityKey());
                _cmd.Parameters.AddWithValue("_loginName", obj1.UserName.Trim());
                //_cmd.Parameters.AddWithValue("_password", obj.Password.Trim());
                _cmd.Parameters.AddWithValue("_Client_ID", obj1.Client_ID);
                dt = db_connection.ExecuteQueryDataTableProcedure(_cmd);

                if (dt.Rows.Count > 0)
                {
                    int bool_check = 1;
                    bool b1 = Convert.ToBoolean(bool_check);
                    string custRefNumber = Convert.ToString(dt.Rows[0]["WireTransfer_ReferanceNo"]);
                    string str_de2 = Convert.ToString(CompanyInfo.Encrypt(custRefNumber, b1));

                    string intValue = Convert.ToString(dt.Rows[0]["Customer_ID"]);
                    obj1.Id = intValue;
                    int intValue1 = Convert.ToInt32(dt.Rows[0]["Country_ID"]);
                    String Encrypted_time1 = Convert.ToString(CompanyInfo.gettime(obj1.Client_ID, intValue, intValue1, context));

                    string IPAddress1 = context.Connection.RemoteIpAddress.ToString(); //System.Web.HttpContext.Current.Request.UserHostAddress;

                    MySqlConnector.MySqlCommand cmd_email = new MySqlConnector.MySqlCommand("Update_email_count");
                    cmd_email.CommandType = CommandType.StoredProcedure;
                    cmd_email.Parameters.AddWithValue("_IP_address", IPAddress1);
                    cmd_email.Parameters.AddWithValue("_Date_chk", Encrypted_time1);
                    cmd_email.Parameters.Add(new MySqlConnector.MySqlParameter("_ID", MySqlConnector.MySqlDbType.Int32));
                    cmd_email.Parameters["_ID"].Direction = ParameterDirection.Output;
                    DataTable db_email = (DataTable)db_connection.ExecuteQueryDataTableProcedure(cmd_email);
                    int resetEmailCount = Convert.ToInt32(db_email.Rows[0]["reset_email_count"]);
                    if (resetEmailCount <= 3 || true)
                    {
                        MySqlConnector.MySqlCommand _cmd_check = new MySqlConnector.MySqlCommand("chk_email_forgotpassword_Otp_app");
                        _cmd_check.CommandType = CommandType.StoredProcedure;
                        _cmd_check.Parameters.AddWithValue("_Customer_ID", Convert.ToInt32(dt.Rows[0]["Customer_ID"]));
                        _cmd_check.Parameters.AddWithValue("_OTP", Convert.ToInt32(obj1.OTP));
                        _cmd_check.Parameters.AddWithValue("_Date", Convert.ToDateTime(Encrypted_time1));
                        DataTable dt1 = db_connection.ExecuteQueryDataTableProcedure(_cmd_check);

                        CompanyInfo.InsertrequestLogTracker("checkotp full count: " + dt1.Rows.Count, 0, 0, 0, 0, "CheckOtp", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
                        CompanyInfo.InsertrequestLogTracker("checkotp full count values : " + Convert.ToInt32(dt.Rows[0]["Customer_ID"]) + "  & "+ Convert.ToInt32(obj1.OTP) +" & "+ Convert.ToDateTime(Encrypted_time1), 0, 0, 0, 0, "CheckOtp", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);

                        if (dt1.Rows.Count == 1)
                        {

                            CompanyInfo.InsertrequestLogTracker("checkotp full count values obj1.OTP: " + obj1.OTP + "  & OTP: " + Convert.ToString(dt1.Rows[0]["OTP"]) + " & " + Convert.ToDateTime(Encrypted_time1), 0, 0, 0, 0, "CheckOtp", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
                            CompanyInfo.InsertrequestLogTracker("checkotp full count values obj1.OTP2: " + Convert.ToDateTime(Encrypted_time1) + "  & OTP2: " + Convert.ToDateTime(dt1.Rows[0]["OTP_Expiry"])  , 0, 0, 0, 0, "CheckOtp", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
                            if (Convert.ToDateTime(dt1.Rows[0]["OTP_Expiry"]) >= Convert.ToDateTime(Encrypted_time1) && obj1.OTP == Convert.ToString(dt1.Rows[0]["OTP"]))
                            {
                                Model.Customer _ObjCustomer = new Model.Customer();

                                _ObjCustomer.WireTransfer_ReferanceNo = obj1.WireTransfer_ReferanceNo;
                                _ObjCustomer.checkparam = obj1.checkparam;
                                _ObjCustomer.AllCustomer_Flag = "";
                                _ObjCustomer.UserName = obj1.UserName;
                                _ObjCustomer.Client_ID = obj1.Client_ID;
                                _ObjCustomer.Branch_ID = obj1.Branch_ID;
                                _ObjCustomer.OTP = obj1.OTP;
                                _ObjCustomer.newpassword = obj1.newpassword;

                                Service.srvCustomer srvCustomer = new Service.srvCustomer();
                                response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_SUCCESS);
                                _ObjCustomer = srvCustomer.UpdateForgotPassword(_ObjCustomer, context);
                                response.ResponseCode = 0;
                                response.ObjData = _ObjCustomer;

                                if (_ObjCustomer.Message == "Success")
                                {
                                    Message = "Password updated successfully.";
                                    response.ResponseMessage = Message;
                                    validateJsonData = new { response = true, responseCode = "00", data = response.ResponseMessage };

                                }
                                else if (_ObjCustomer.Message == "expired")
                                {
                                    Message = "Forgot password has expired.";
                                    response.ResponseMessage = Message;
                                    validateJsonData = new { response = false, responseCode = "02", data = response.ResponseMessage };

                                }
                                else if (_ObjCustomer.Message == "Exceeded")
                                {
                                    Message = "Your reset password limit has exceeded.";
                                    response.ResponseMessage = Message;
                                    validateJsonData = new { response = false, responseCode = "02", data = response.ResponseMessage };

                                }
                                else
                                {
                                    Message = "Oop's.. Something went wrong. Please try again later.";
                                    response.ResponseMessage = Message;
                                    validateJsonData = new { response = false, responseCode = "02", data = response.ResponseMessage };
                                }                               
                            }
                            else
                            {
                                Message = "You have enterd Invalid OTP or OTP has expired.";
                                validateJsonData = new { response = false, responseCode = "02", data = Message };
                            }

                        }
                        else
                        {
                            Message = "You have entered wrong OTP.";
                            validateJsonData = new { response = false, responseCode = "02", data = Message };

                        }

                    }
                    else
                    {
                        response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_TECHNICAL_ERROR);
                        response.ResponseMessage = "Your limit for send email is expired please try again tomorrow.";
                        response.ResponseCode = 1;
                        validateJsonData = new { response = false, responseCode = "02", data = response.ResponseMessage };
                    }
                    
                }
                return new JsonResult(validateJsonData);
            }
            catch (Exception ex)
            {
                validateJsonData = new { response = false, responseCode = "01", data = "Invalid Request" };
                CompanyInfo.InsertErrorLogTracker(ex.ToString(), 0, 0, 0, 0, "checkOTP", 0, Convert.ToInt32(obj1.Client_ID), "", HttpContext);
                return new JsonResult(validateJsonData);
            }

            


        }

        [HttpPost]
        [Route("getsecuritydetails")]
        public IActionResult getsecurity_details([FromBody] JsonElement obj)
        {
            HttpContext context = HttpContext;
            Model.response.WebResponse response = null;
            Login obj1 = new Login();
            string json = System.Text.Json.JsonSerializer.Serialize(obj);
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
            var validateJsonData = (dynamic)null;
            Login Obj = JsonConvert.DeserializeObject<Login>(json);
            CompanyInfo.InsertrequestLogTracker("getsecurity_details full request body: " + JObject.Parse(json), 0, 0, 0, 0, "getsecuritydetails", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
            try
            {
                if (!SqlInjectionProtector.ReadJsonElementValues(obj) || !SqlInjectionProtector.ReadJsonElementValuesScript(obj))
                {
                    var errorResponse = new { response = false, responseCode = "02", data = "Invalid input detected." };
                    return new JsonResult(errorResponse) { StatusCode = 400 };
                }

                Service.srvPermission srvPermission = new Service.srvPermission();
                response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_SUCCESS);

                Service.srvLogin srvLogin = new Service.srvLogin();
                DataTable dt = new DataTable();
                dt = srvLogin.getsecurity_details(Obj);
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
            catch (Exception ex)
            {
                string Activity = "Api:- getsecuritydetails" + ex.ToString();
                validateJsonData = new { response = false, responseCode = "01", data = "Invalid Request" };
                CompanyInfo.InsertErrorLogTracker(Activity.ToString(), 0, 0, 0, 0, "getsecurity_details", Convert.ToInt32(0), Convert.ToInt32(0), "", context);
                return new JsonResult(validateJsonData);

            }

        }


            [HttpPost]
        [Route("SetAuthentication")]
        public IActionResult SetAuthentication([FromBody] JsonElement obj)
        {

            HttpContext context = HttpContext;
            Model.response.WebResponse response = null;
            Login obj1 = new Login();
            string json = System.Text.Json.JsonSerializer.Serialize(obj);
            dynamic data = JObject.Parse(json);
            CompanyInfo.InsertrequestLogTracker("SetAuthentication full request body: " + JObject.Parse(json), 0, 0, 0, 0, "SetAuthentication", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
            //DeliveryTypeController.Getdeliverytype(data);
            var validateJsonData = (dynamic)null;
            Login Obj = JsonConvert.DeserializeObject<Login>(json);
            try
            {
                if (!SqlInjectionProtector.ReadJsonElementValues(obj) || !SqlInjectionProtector.ReadJsonElementValuesScript(obj))
                {
                    var errorResponse = new { response = false, responseCode = "02", data = "Invalid input detected." };
                    return new JsonResult(errorResponse) { StatusCode = 400 };
                }
                //Service.srvPermission srvPermission = new Service.srvPermission();
                response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_SUCCESS);

                Service.srvLogin srvLogin = new Service.srvLogin();
                DataTable dt = new DataTable();
                dt = srvLogin.SetAuthentication(Obj);
                response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_SUCCESS);
                response.data = "success";
                response.ObjData = dt;
                if (dt != null)
                {
                    var responseData = dt.Rows.OfType<DataRow>()
                        .Select(row => dt.Columns.OfType<DataColumn>()
                            .ToDictionary(col => col.ColumnName, c => row[c].ToString()));
                    validateJsonData = new { response = true, responseCode = "00", data = responseData };
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
                string Activity = "Api:- SetAuthentication :" + ex.ToString();
                validateJsonData = new { response = false, responseCode = "01", data = "Invalid Request" };
                CompanyInfo.InsertErrorLogTracker(Activity.ToString(), 0, 0, 0, 0, "SetAuthentication", Convert.ToInt32(obj1.Branch_ID), Convert.ToInt32(obj1.Client_ID), "", context);
                return new JsonResult(validateJsonData);

            }

        }



        [HttpPost]
        [Route("sendotp")]

        public IActionResult send([FromBody] JsonElement obj1)
        {
            HttpContext context = HttpContext;
            Model.response.WebResponse response = null;
            //Login obj1 = new Login();
            string otp = ""; string output = ""; string activityLog = "";
            string json = System.Text.Json.JsonSerializer.Serialize(obj1);
            dynamic data = JObject.Parse(json);
            CompanyInfo.InsertrequestLogTracker("sendotp full request body: " + JObject.Parse(json), 0, 0, 0, 0, "send", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
            var validateJsonData = (dynamic)null;
            Login obj = JsonConvert.DeserializeObject<Login>(json);

            if (!SqlInjectionProtector.ReadJsonElementValues(obj1) || !SqlInjectionProtector.ReadJsonElementValuesScript(obj1))
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
            else if (data.userName == "" || data.userName == null)
            {
                validateJsonData = new { response = false, responseCode = "02", data = "Invalid userName field." };
                return new JsonResult(validateJsonData);
            }
            else if (!int.TryParse(Convert.ToString(data.branchID), out number) || data.branchID == "" || data.branchID == null)
            {
                validateJsonData = new { response = false, responseCode = "02", data = "Invalid branchID field." };
                return new JsonResult(validateJsonData);
            }


            obj.UserName = data.userName;
            obj.Client_ID = data.clientID;
            obj.Branch_ID = data.branchID;

            activityLog = "Data is assigned to variable";

            try
            {

                Service.srvLogin srvLogin = new Service.srvLogin();
                DataTable dt = new DataTable();
                dt = srvLogin.IsValidEmail(obj);
                if (dt.Rows.Count == 1)
                {
                    activityLog = activityLog + "user is on 1st step";

                    //Parth Change for calling function GetLastOtpForCustomer and assigning necessary values
                    #region Calling function GetLastOtpForCustomer and assigning necessary values
                    // Get the last OTP for the customer
                    DataTable lastOtpData = GetLastOtpForCustomer(obj.UserName);
                    DateTime otpExpiryTime = DateTime.MinValue; // Default OTP expiry time if no data is found

                    // Check if the result contains any rows
                    if (lastOtpData != null && lastOtpData.Rows.Count > 0)
                    {
                        otpExpiryTime = Convert.ToDateTime(lastOtpData.Rows[0]["OTP_Expiry"]);  // Extract the OTP expiry time if data exists
                    }
                    //Values assigned for Encrypt DateTime according to country timezone
                    string intValue = Convert.ToString(dt.Rows[0]["Customer_ID"]);
                    int intValue1 = Convert.ToInt32(dt.Rows[0]["Country_ID"]);
                    String Encrypted_time1 = Convert.ToString(CompanyInfo.gettime(obj.Client_ID, intValue, intValue1, context));

                    DateTime dateTime = Convert.ToDateTime(Encrypted_time1).AddMinutes(15); //Encrypting current datetime according to country timezone

                    double differenceInMinutes = dateTime.Subtract(otpExpiryTime).TotalMinutes; //Substracting current datetime to user's last otp datetime
                    #endregion
                    //End Parth change for calling function GetLastOtpForCustomer and assigning necessary values

                    DateTime RecordDate = Convert.ToDateTime(CompanyInfo.gettime(obj.Client_ID, context));
                    string IPAddress1 = context.Connection.RemoteIpAddress.ToString(); //System.Web.HttpContext.Current.Request.UserHostAddress;

                    MySqlConnector.MySqlCommand cmd_email = new MySqlConnector.MySqlCommand("Update_email_count");
                    cmd_email.CommandType = CommandType.StoredProcedure;
                    cmd_email.Parameters.AddWithValue("_IP_address", IPAddress1);
                    cmd_email.Parameters.AddWithValue("_Date_chk", RecordDate);
                    cmd_email.Parameters.Add(new MySqlConnector.MySqlParameter("_ID", MySqlConnector.MySqlDbType.Int32));
                    cmd_email.Parameters["_ID"].Direction = ParameterDirection.Output;
                    DataTable db_email = (DataTable)db_connection.ExecuteQueryDataTableProcedure(cmd_email);
                    int resetEmailCount = Convert.ToInt32(db_email.Rows[0]["reset_email_count"]);
                    if (resetEmailCount > 3)
                    {
                        response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_TECHNICAL_ERROR);
                        response.ResponseMessage = "Your limit for send email is expired please try again tomorrow.";
                        response.ResponseCode = 1;
                        validateJsonData = new { response = false, responseCode = "02", data = response.ResponseMessage };

                    }
                    //Parth change to resend same otp requested by customer within a minute
                    #region Resend same otp requested by a customer within a minute
                    else if (lastOtpData.Rows.Count > 0 && differenceInMinutes < 1)
                    {
                        otp = lastOtpData.Rows[0]["OTP"].ToString(); // Reusing the same OTP
                        response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_SUCCESS);
                        response.data = otp;    // Assigning the same OTP here
                        response.ResponseCode = 0;
                        validateJsonData = new { response = true, responseCode = "00", data = "otp :- " + otp };    // Sending the OTP directly
                        #region sending email of same otp to user when user does resend otp within a minute
                        //sending email of same otp to user when user does resend otp within a minute
                        string email = obj.UserName;
                        //string subject = string.Empty;
                        string body = string.Empty, subject = string.Empty;
                        string body1 = string.Empty;
                        string template = "";
                        HttpWebRequest httpRequest = null, httpRequest1 = null;
                        DataTable dtc = CompanyInfo.get(obj.Client_ID, context);
                        if (dtc.Rows.Count == 1 && email != null)
                        {
                            int bool_check1 = 1;
                            bool b2 = Convert.ToBoolean(bool_check1);
                            activityLog = activityLog + "user is on 3rd step";


                            string URL = Convert.ToString(dtc.Rows[0]["Company_URL_Admin"]);
                            string cust_url = Convert.ToString(dtc.Rows[0]["Company_URL_Customer"]);


                            httpRequest = (HttpWebRequest)WebRequest.Create(URL + "Email/Two-step-authentication.html");
                            httpRequest.UserAgent = "Code Sample Web Client";
                            HttpWebResponse webResponse = (HttpWebResponse)httpRequest.GetResponse();
                            using (StreamReader reader = new StreamReader(webResponse.GetResponseStream()))
                            {
                                body = reader.ReadToEnd();
                            }
                            body = body.Replace("[name]", Convert.ToString(dt.Rows[0]["First_Name"])); //dt.Rows[0]["full_name"].ToString());
                            string ref_no = Convert.ToString(dt.Rows[0]["WireTransfer_ReferanceNo"]);
                            int bool_check = 1;
                            bool b1 = Convert.ToBoolean(bool_check);
                            string s2 = Convert.ToString(dt.Rows[0]["WireTransfer_ReferanceNo"]);
                            string str_de2 = Convert.ToString(CompanyInfo.Encrypt(s2, b1));
                            //body = body.Replace("[link]", cust_url + "new-password.html?reference=" + HttpUtility.UrlEncode(str_de2) + "&checkparam=" + HttpUtility.UrlEncode(Encrypted_time1));
                            body = body.Replace("[link]", otp);
                            body = body.Replace("[insert body]", body1);
                            httpRequest1 = (HttpWebRequest)WebRequest.Create(URL + "Email/Two-step-authentication.txt");
                            httpRequest1.UserAgent = "Code Sample Web Client";
                            HttpWebResponse webResponse1 = (HttpWebResponse)httpRequest1.GetResponse();
                            using (StreamReader reader = new StreamReader(webResponse1.GetResponseStream()))
                            {
                                subject = reader.ReadLine();
                            }
                            activityLog = activityLog + "user is on 4th step";
                            string msg = (string)CompanyInfo.Send_Mail(dtc, obj.UserName, body, subject, obj.Client_ID, obj.Branch_ID, "", "", "", context);
                            activityLog = activityLog + "user is on 5th step";

                            msg = (string)CompanyInfo.Send_Mail(dtc, obj.UserName, body, subject, obj.Client_ID, obj.Branch_ID, "", "", "", context);

                            int stattus1 = (int)CompanyInfo.InsertActivityLogDetails(msg + obj.UserName.Trim(), 0, 0, 0, 0, "check_login", 2, 1, "check_login", context);

                            if (msg == "Success")
                            {
                                activityLog = activityLog + "user is on 6th step";
                                output = "ReferenceNo :-" + ref_no + "\n" + "otp :-" + otp + "\n" + "Encrypted_time :-" + "" + Encrypted_time1 + "";

                                response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_SUCCESS);
                                response.data = "success";
                                response.ResponseCode = 0;
                                Model.ActivityLogTracker _objActivityLog = new Model.ActivityLogTracker();
                                _objActivityLog.Activity = "Forgot password email sent to " + obj.UserName + " ";
                                _objActivityLog.FunctionName = "Check Login";
                                _objActivityLog.Transaction_ID = 0;
                                _objActivityLog.WhoAcessed = 0;
                                _objActivityLog.Branch_ID = obj.Branch_ID;
                                _objActivityLog.Client_ID = obj.Client_ID;

                                CompanyInfo.InsertActivityLogDetails(activityLog, 0, 0, 0, 0, "forgotpasswordlog", Convert.ToInt32(obj.Client_ID), 0, "", HttpContext);

                                Service.srvActivityLogTracker srvActivityLog = new Service.srvActivityLogTracker();
                                int stattus = (int)CompanyInfo.InsertActivityLogDetails("Invalid Email tried to reset password " + obj.UserName.Trim(), 0, 0, 0, 0, "check_login", 2, 1, "check_login", context);
                                validateJsonData = new { response = false, responseCode = "02", data = output };
                            }
                            else
                            {
                                CompanyInfo.InsertActivityLogDetails(activityLog, 0, 0, 0, 0, "forgotpasswordlog", Convert.ToInt32(obj.Client_ID), 0, "", HttpContext);
                                response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_TECHNICAL_ERROR);
                                response.ResponseMessage = "Something went wrong. Please try again later.";
                                response.ResponseCode = 1;
                                validateJsonData = new { response = false, responseCode = "02", data = response.ResponseMessage };
                            }
                        }
                        #endregion
                    }
                    #endregion
                    //End Parth change to resend same otp requested by customer within a minute
                    else
                    {
                        activityLog = activityLog  ;

                        int otpLength = 6;
                        otp = GenerateOTP(otpLength);
                        string email = obj.UserName;
                        //string subject = string.Empty;
                        string body = string.Empty, subject = string.Empty;
                        string body1 = string.Empty;
                        string template = "";
                        HttpWebRequest httpRequest = null, httpRequest1 = null;
                        DataTable dtc = CompanyInfo.get(obj.Client_ID, context);
                        if (dtc.Rows.Count == 1 && email != null)
                        {

                            DateTime date = DateTime.Now;
                            DateTime date3 = date.AddMinutes(15);
                            int bool_check1 = 1;
                            bool b2 = Convert.ToBoolean(bool_check1);

                            //string intValue = Convert.ToString(dt.Rows[0]["Customer_ID"]);
                            //int intValue1 = Convert.ToInt32(dt.Rows[0]["Country_ID"]);
                            //String Encrypted_time1 = Convert.ToString(CompanyInfo.gettime(obj.Client_ID, intValue, intValue1, context));


                            MySqlConnector.MySqlCommand _cmd_check = new MySqlConnector.MySqlCommand("sp_save_otp");

                            _cmd_check.CommandType = CommandType.StoredProcedure;
                            _cmd_check.Parameters.AddWithValue("_OTP", otp);
                            _cmd_check.Parameters.AddWithValue("_OTP_Expiry", Convert.ToDateTime(Encrypted_time1).AddMinutes(15));
                            _cmd_check.Parameters.AddWithValue("_Customer_ID", dt.Rows[0]["Customer_ID"]);
                            _cmd_check.Parameters.Add(new MySqlConnector.MySqlParameter("setflag", MySqlConnector.MySqlDbType.String));
                            _cmd_check.Parameters["setflag"].Direction = ParameterDirection.Output;
                            DataTable d_t = db_connection.ExecuteQueryDataTableProcedure(_cmd_check);

                            // Parth Code for restrict OTP email send
                            /*string setFlagValue = _cmd_check.Parameters["setflag"].Value.ToString();
                            try
                            {
                                if (setFlagValue == "2")
                                {
                                    activityLog += " OTP request limit exceeded.";
                                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_TECHNICAL_ERROR);
                                    response.ResponseMessage = "OTP request limit exceeded. Please try again later.";
                                    response.ResponseCode = 2;
                                    validateJsonData = new { response = false, responseCode = "02", data = response.ResponseMessage };
                                    return new JsonResult(validateJsonData);
                                }
                            }
                            catch (Exception ex)
                            {
                                _ = Task.Run(() => CompanyInfo.InsertrequestLogTracker("sendotp otp count setflagValue: " + setFlagValue, 0, 0, 0, 0, "send", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext));
                            }*/
                            // End Parth Code for restrict OTP email send

                            activityLog = activityLog + "user is on 3rd step";


                            string URL = Convert.ToString(dtc.Rows[0]["Company_URL_Admin"]);
                            string cust_url = Convert.ToString(dtc.Rows[0]["Company_URL_Customer"]);

                            CompanyInfo.InsertrequestLogTracker("sendotp email url used: " + URL, 0, 0, 0, 0, "send", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);

                            httpRequest = (HttpWebRequest)WebRequest.Create(URL + "Email/Two-step-authentication.html");
                            httpRequest.UserAgent = "Code Sample Web Client";
                            HttpWebResponse webResponse = (HttpWebResponse)httpRequest.GetResponse();
                            using (StreamReader reader = new StreamReader(webResponse.GetResponseStream()))
                            {
                                body = reader.ReadToEnd();
                            }
                            body = body.Replace("[name]", Convert.ToString(dt.Rows[0]["First_Name"])); //dt.Rows[0]["full_name"].ToString());
                            string ref_no = Convert.ToString(dt.Rows[0]["WireTransfer_ReferanceNo"]);
                            int bool_check = 1;
                            bool b1 = Convert.ToBoolean(bool_check);
                            string s2 = Convert.ToString(dt.Rows[0]["WireTransfer_ReferanceNo"]);
                            string str_de2 = Convert.ToString(CompanyInfo.Encrypt(s2, b1));
                            //body = body.Replace("[link]", cust_url + "new-password.html?reference=" + HttpUtility.UrlEncode(str_de2) + "&checkparam=" + HttpUtility.UrlEncode(Encrypted_time1));
                            body = body.Replace("[link]", otp);
                            body = body.Replace("[insert body]", body1);
                            httpRequest1 = (HttpWebRequest)WebRequest.Create(URL + "Email/Two-step-authentication.txt");
                            httpRequest1.UserAgent = "Code Sample Web Client";
                            HttpWebResponse webResponse1 = (HttpWebResponse)httpRequest1.GetResponse();
                            using (StreamReader reader = new StreamReader(webResponse1.GetResponseStream()))
                            {
                                subject = reader.ReadLine();
                            }
                            activityLog = activityLog + "user is on 4th step";
                            string msg = (string)CompanyInfo.Send_Mail(dtc, obj.UserName, body, subject, obj.Client_ID, obj.Branch_ID, "", "", "", context);
                            activityLog = activityLog + "user is on 5th step";

                            msg = (string)CompanyInfo.Send_Mail(dtc, obj.UserName, body, subject, obj.Client_ID, obj.Branch_ID, "", "", "", context);

                            int stattus1 = (int)CompanyInfo.InsertActivityLogDetails(msg + obj.UserName.Trim(), 0, 0, 0, 0, "check_login", 2, 1, "check_login", context);
                            
                            if (msg == "Success")
                            {
                                activityLog = activityLog + "user is on 6th step";
                                output = "ReferenceNo :-" + ref_no + "\n" + "otp :-" + otp + "\n" + "Encrypted_time :-" + "" + Encrypted_time1 + "";

                                response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_SUCCESS);
                                response.data = "success";
                                response.ResponseCode = 0;
                                Model.ActivityLogTracker _objActivityLog = new Model.ActivityLogTracker();
                                _objActivityLog.Activity = "Forgot password email sent to " + obj.UserName + " ";
                                _objActivityLog.FunctionName = "Check Login";
                                _objActivityLog.Transaction_ID = 0;
                                _objActivityLog.WhoAcessed = 0;
                                _objActivityLog.Branch_ID = obj.Branch_ID;
                                _objActivityLog.Client_ID = obj.Client_ID;

                                CompanyInfo.InsertActivityLogDetails(activityLog, 0, 0, 0, 0, "forgotpasswordlog", Convert.ToInt32(obj.Client_ID), 0, "", HttpContext);

                                Service.srvActivityLogTracker srvActivityLog = new Service.srvActivityLogTracker();
                                int stattus = (int)CompanyInfo.InsertActivityLogDetails("Invalid Email tried to reset password " + obj.UserName.Trim(), 0, 0, 0, 0, "check_login", 2, 1, "check_login", context);
                                validateJsonData = new { response = false, responseCode = "02", data = output };
                            }
                            else
                            {
                                CompanyInfo.InsertActivityLogDetails(activityLog, 0, 0, 0, 0, "forgotpasswordlog", Convert.ToInt32(obj.Client_ID), 0, "", HttpContext);
                                response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_TECHNICAL_ERROR);
                                response.ResponseMessage = "Something went wrong. Please try again later.";
                                response.ResponseCode = 1;
                                validateJsonData = new { response = false, responseCode = "02", data = response.ResponseMessage };
                            }
                        }
                        else
                        {
                            CompanyInfo.InsertActivityLogDetails(activityLog, 0, 0, 0, 0, "forgotpasswordlog", Convert.ToInt32(obj.Client_ID), 0, "", HttpContext);
                            response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_TECHNICAL_ERROR);
                            response.ResponseMessage = "Something went wrong. Please try again later.";
                            response.ResponseCode = 2;
                            validateJsonData = new { response = false, responseCode = "02", data = response.ResponseMessage };
                        }
                    }
                }
                else
                {
                    CompanyInfo.InsertActivityLogDetails(activityLog, 0, 0, 0, 0, "forgotpasswordlog", Convert.ToInt32(obj.Client_ID), 0, "", HttpContext);
                    int stattus = (int)CompanyInfo.InsertActivityLogDetails("Invalid Email tried to reset password " + obj.UserName.Trim(), 0, 0, 0, 0, "check_login", 2, 1, "check_login", context);
                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_TECHNICAL_ERROR);
                    response.ResponseMessage = "This Email is not registered with us or something went wrong. Please try again later.";
                    response.ResponseCode = 3;
                    validateJsonData = new { response = false, responseCode = "02", data = response.ResponseMessage };
                }
            }
            catch (Exception ex)
            {
                CompanyInfo.InsertActivityLogDetails(activityLog, 0, 0, 0, 0, "forgotpasswordlog", Convert.ToInt32(obj.Client_ID), 0, "", HttpContext);
                response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_TECHNICAL_ERROR);
                response.sErrorExceptionText = ex.ToString();
                response.ResponseMessage = "Technical error. Please try again later.";
                response.ResponseCode = 3;
                //Error Log Handled
                Model.ErrorLog objError = new Model.ErrorLog();
                objError.User = new Model.User();
                objError.Error = "Api : Password  --" + ex.ToString();
                objError.Date = DateTime.Now;
                objError.User_ID = 1;
                objError.Client_ID = obj.Client_ID;
                objError.Function_Name = "send";
                Service.srvErrorLog srvError = new Service.srvErrorLog();
                validateJsonData = new { response = false, responseCode = "01", data = "Invalid Request" };
                CompanyInfo.InsertErrorLogTracker(ex.ToString(), 0, 0, 0, 0, "sendotp", 0, Convert.ToInt32(obj.Client_ID), "", HttpContext);
                return new JsonResult(validateJsonData);
            }


            return new JsonResult(validateJsonData);

        }
        static string GenerateOTP(int length)
        {
            const string chars = "0123456789";
            Random random = new Random();
            char[] otp = new char[length];

            for (int i = 0; i < length; i++)
            {
                otp[i] = chars[random.Next(chars.Length)];
            }

            return new string(otp);
        }
        //Parth changes for getting last Otp of customer
        #region Get Last OTP For Customer method
        public static DataTable GetLastOtpForCustomer(string userName)
        {
            MySqlConnector.MySqlCommand _cmd_check = new MySqlConnector.MySqlCommand("GetLastOtpForCustomer");
            _cmd_check.CommandType = CommandType.StoredProcedure;
            _cmd_check.Parameters.AddWithValue("@UserName", userName);  // Passing the email to the procedure
            DataTable result = db_connection.ExecuteQueryDataTableProcedure(_cmd_check);

            return result;
        }
        #endregion
        //End Parth changes for getting last Otp of customer
        [HttpPost]
        [Route("isvalidemail")]        
        public IActionResult IsValidEmail([FromBody] JsonElement obj)
        {
            HttpContext context = HttpContext;
            Model.response.WebResponse response = null;
            Login obj1 = new Login();
            string json = System.Text.Json.JsonSerializer.Serialize(obj);
            dynamic data = JObject.Parse(json);
            CompanyInfo.InsertrequestLogTracker("isvalidemail full request body: " + JObject.Parse(json), 0, 0, 0, 0, "IsValidEmail", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
            var validateJsonData = (dynamic)null;
            Login Obj = JsonConvert.DeserializeObject<Login>(json);
            try
            {
                if (!SqlInjectionProtector.ReadJsonElementValues(obj) || !SqlInjectionProtector.ReadJsonElementValuesScript(obj))
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
                else if (data.userName == "" || data.userName == null)
            {
                validateJsonData = new { response = false, responseCode = "02", data = "Invalid userName field." };
                return new JsonResult(validateJsonData);
            }
            else if (!int.TryParse(Convert.ToString(data.branchID), out number) || data.branchID == "" || data.branchID == null)
            {
                validateJsonData = new { response = false, responseCode = "02", data = "Invalid branchID field." };
                return new JsonResult(validateJsonData);
            }
              

                Obj.UserName = data.userName;
            Obj.Client_ID = data.clientID;
            Obj.Branch_ID = data.branchID;
                try { Obj.Captcha = data.Captcha; } catch (Exception ex) { Obj.Captcha = ""; }

                string msg = "";
                List<Model.Login> _lst = new List<Model.Login>();
                //Start Parth Added for Check Captcha
                #region Check Captcha
                try
                {
                    DataTable dtperm_status = new DataTable();
                    MySqlConnector.MySqlCommand cmd_captcha = new MySqlConnector.MySqlCommand("GetPermissions");
                    cmd_captcha.CommandType = CommandType.StoredProcedure;
                    int per102 = 1;
                    cmd_captcha.Parameters.AddWithValue("_Client_ID", Obj.Client_ID);
                    cmd_captcha.Parameters.AddWithValue("_whereclause", " and PID = 102");
                    dtperm_status = db_connection.ExecuteQueryDataTableProcedure(cmd_captcha);
                    per102 = Convert.ToInt32(dtperm_status.Rows[0]["Status_ForCustomer"]);

                    if (per102 == 0 && !string.IsNullOrWhiteSpace(Obj.Captcha))
                    {
                        Boolean checkCaptcha = true;

                        Service.srvCaptcha srv = new Service.srvCaptcha();
                        checkCaptcha = srv.VerifyCaptcha_withUserName(Obj.UserName, Obj.Captcha);
                        int status = (int)CompanyInfo.InsertActivityLogDetails(" Captcha response value: " + Convert.ToString(checkCaptcha), 0, 0, Convert.ToInt32(0), 1, "IsValidCustomer", Convert.ToInt32(0), Convert.ToInt32(0), "IsValidCustomer", context);
                        if (!checkCaptcha)
                        {
                            validateJsonData = new { response = false, responseCode = "02", responseMessage = "Invalid Captcha" };
                            return new JsonResult(validateJsonData);
                        }
                    }

                }
                catch (Exception ex)
                {
                    Model.ErrorLog objError = new Model.ErrorLog();
                    objError.User = new Model.User();
                    objError.Error = "Api : Customer add --" + ex.ToString();
                    objError.Date = DateTime.Now;
                    objError.User_ID = 1;
                    objError.Client_ID = Obj.Client_ID;

                    Service.srvErrorLog srvError = new Service.srvErrorLog();
                    srvError.Create(objError, context);
                }
                #endregion Check Captcha
                DataTable dt = new DataTable();
                MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("CustomerDetailsByEmail");
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.Parameters.AddWithValue("_Email_ID", Obj.UserName.Trim());
                _cmd.Parameters.AddWithValue("_Client_ID", Obj.Client_ID);
                dt = db_connection.ExecuteQueryDataTableProcedure(_cmd);

                List<Dictionary<string, object>> Emails = new List<Dictionary<string, object>>();

                foreach (DataRow dr in dt.Rows)
                {
                    if (dr != null)
                    {
                        Dictionary<string, object> email = new Dictionary<string, object>();
                        email["fullName"] = dr["full_name"];

                        //string subject = string.Empty;
                        string body = string.Empty, subject = string.Empty;
                        string body1 = string.Empty;
                        string template = "";
                        HttpWebRequest httpRequest = null, httpRequest1 = null;
                        DataTable dtc = new DataTable();
                        MySqlConnector.MySqlCommand cmd = new MySqlConnector.MySqlCommand("Get_CompanyInfo");
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("_Client_ID", Obj.Client_ID);
                        cmd.Parameters.AddWithValue("_Customer_ID", 0);
                        cmd.Parameters.AddWithValue("_SecurityKey", CompanyInfo.SecurityKey());
                        dtc = db_connection.ExecuteQueryDataTableProcedure(cmd);
                        if (dtc.Rows.Count == 1 && email != null)
                        {
                            string intValue = Convert.ToString(dt.Rows[0]["Customer_ID"]);
                            //obj.Id = intValue;
                            int intValue1 = Convert.ToInt32(dt.Rows[0]["Country_ID"]);
                            String Encrypted_time1 = Convert.ToString(CompanyInfo.gettime(obj1.Client_ID, intValue, intValue1, context));
                            DateTime Encrypted_time = Convert.ToDateTime(Encrypted_time1);
                            DateTime date3 = Encrypted_time.AddMinutes(15);
                            int bool_check1 = 1;
                            bool b2 = Convert.ToBoolean(bool_check1);
                            Encrypted_time1 = Convert.ToString(CompanyInfo.Encrypt(Convert.ToString(date3), b2));
                            //Parth Change for calling function GetLastOtpForCustomer and assigning necessary values
                            #region Calling function GetLastOtpForCustomer and assigning necessary values
                            double differenceInMinutes = 1;
                            string lastotp = "";
                            try
                            {
                                // Get the last OTP for the customer
                                DataTable lastOtpData = GetLastOtpForgotPassword(Obj.UserName);
                                DateTime otpExpiryTime = DateTime.MinValue; // Default OTP expiry time if no data is found

                                // Check if the result contains any rows
                                if (lastOtpData != null && lastOtpData.Rows.Count > 0)
                                {
                                    otpExpiryTime = Convert.ToDateTime(lastOtpData.Rows[0]["OTP_Expiry"]);  // Extract the OTP expiry time if data exists
                                    lastotp = lastOtpData.Rows[0]["OTP"].ToString();    // Extract the last OTP if data exists
                                }
                                String currentDateTime = Convert.ToString(CompanyInfo.gettime(obj1.Client_ID, intValue, intValue1, context));
                                DateTime dateTime = Convert.ToDateTime(currentDateTime).AddMinutes(15);
                                //DateTime dateTime = Convert.ToDateTime(Encrypted_time1).AddMinutes(15); //Encrypting current datetime according to country timezone

                                differenceInMinutes = dateTime.Subtract(otpExpiryTime).TotalMinutes; //Substracting current datetime to user's last otp datetime
                            }
                            catch (Exception ex)
                            {
                                _ = Task.Run(() => CompanyInfo.InsertrequestLogTracker("isvalidemail sending same otp exception: " + ex, 0, 0, 0, 0, "send", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext));
                            }
                            
                            #endregion
                            //End Parth change for calling function GetLastOtpForCustomer and assigning necessary values
                            string URL = Convert.ToString(dtc.Rows[0]["Company_URL_Admin"]);
                            string cust_url = Convert.ToString(dtc.Rows[0]["Company_URL_Customer"]);
                            httpRequest = (HttpWebRequest)WebRequest.Create(URL + "Email/forgot-password.html");
                            httpRequest.UserAgent = "Code Sample Web Client";
                            HttpWebResponse webResponse = (HttpWebResponse)httpRequest.GetResponse();
                            using (StreamReader reader = new StreamReader(webResponse.GetResponseStream()))
                            {
                                body = reader.ReadToEnd();
                            }
                            body = body.Replace("[name]", Convert.ToString(dt.Rows[0]["First_Name"])); //dt.Rows[0]["full_name"].ToString());
                            string ref_no = Convert.ToString(dt.Rows[0]["WireTransfer_ReferanceNo"]);
                            int bool_check = 1;
                            bool b1 = Convert.ToBoolean(bool_check);
                            string s2 = Convert.ToString(dt.Rows[0]["WireTransfer_ReferanceNo"]);
                            string str_de2 = Convert.ToString(CompanyInfo.Encrypt(s2, b1));

                            Random generator = new Random();
                            String randomOTP = generator.Next(0, 1000000).ToString("D6");
                            //Start Parth Added for reusing the same otp
                            if (differenceInMinutes < 1 && !string.IsNullOrEmpty(lastotp)) 
                            {
                                randomOTP = lastotp; // Reusing the same OTP
                            }
                            //End Parth Added for reusing the same otp
                            string Query = "SELECT  First_Name, CONCAT(First_Name,' ',Last_Name)AS full_name,WireTransfer_ReferanceNo,Customer_ID FROM customer_registration"+
                                " WHERE Email_ID = '"+ Obj.UserName.Trim() + "' AND delete_status = 0 AND Client_ID = "+ Obj.Client_ID + " ";
                            MySqlConnector.MySqlCommand cmdforcustId = new MySqlConnector.MySqlCommand(Query);
                            DataTable dtt = db_connection.ExecuteQueryDataTableProcedure(cmdforcustId);

                            int customerId = 0;
                            if (dtt.Rows.Count > 0)
                            {
                                customerId = Convert.ToInt32(dtt.Rows[0]["Customer_ID"]);                               
                            }


                            MySqlConnector.MySqlCommand _cmd_check = new MySqlConnector.MySqlCommand("sp_forgotpassword_save_otp");

                            _cmd_check.CommandType = CommandType.StoredProcedure;
                            _cmd_check.Parameters.AddWithValue("_OTP", randomOTP);
                            _cmd_check.Parameters.AddWithValue("_OTP_Expiry", date3);
                            _cmd_check.Parameters.AddWithValue("_Customer_ID", customerId);
                            _cmd_check.Parameters.Add(new MySqlConnector.MySqlParameter("setflag", MySqlConnector.MySqlDbType.String));
                            _cmd_check.Parameters["setflag"].Direction = ParameterDirection.Output;
                            DataTable d_t = db_connection.ExecuteQueryDataTableProcedure(_cmd_check);

                            int check_otpSent = Convert.ToInt32(d_t.Rows[0]["setflag"]);
                            if (check_otpSent == 1)  // IF 1 then send email 
                            {

                            }
                            else
                            {
                                // return user from here
                            }

                            try
                            {
                                if (check_otpSent == 2)
                                {
                                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_TECHNICAL_ERROR);
                                    response.ResponseMessage = "OTP request limit exceeded. Please try again tomorrow.";
                                    response.ResponseCode = 2;
                                    validateJsonData = new { response = false, responseCode = "02", responseMessage = response.ResponseMessage };
                                    _ = Task.Run(() => CompanyInfo.InsertrequestLogTracker("isvalidemail otp count setflagValue: " + check_otpSent, 0, 0, 0, 0, "send", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext));
                                    return new JsonResult(validateJsonData);
                                }
                            }
                            catch (Exception ex)
                            {
                                _ = Task.Run(() => CompanyInfo.InsertrequestLogTracker("isvalidemail otp count exception: " + ex, 0, 0, 0, 0, "send", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext));
                            }

                            /*body = body.Replace("[link]", cust_url + "new-password.html?reference=" + HttpUtility.UrlEncode(str_de2) + "&checkparam=" + HttpUtility.UrlEncode(Encrypted_time1));
                            body = body.Replace("[insert body]", body1);*/

                            body = body.Replace("[link]", randomOTP);

                           
                            httpRequest1 = (HttpWebRequest)WebRequest.Create(URL + "Email/new-password.txt");
                            httpRequest1.UserAgent = "Code Sample Web Client";
                            HttpWebResponse webResponse1 = (HttpWebResponse)httpRequest1.GetResponse();
                            using (StreamReader reader = new StreamReader(webResponse1.GetResponseStream()))
                            {
                                subject = reader.ReadLine();
                            }

                            
                            

                            string msg1 = (string)CompanyInfo.Send_Mail(dtc, Obj.UserName, body, subject, Obj.Client_ID, Obj.Branch_ID, "", "", "", context);
                            email["referanceNo"] = str_de2;
                            email["checkParameter"] = Encrypted_time1;
                            Emails.Add(email);
                            if (msg1 == "Success")
                            {
                                response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_SUCCESS);
                                response.data = "success";
                                response.ResponseCode = 0;
                                Model.ActivityLogTracker _objActivityLog = new Model.ActivityLogTracker();
                                _objActivityLog.Activity = "Forgot password email sent to " + Obj.UserName + " ";
                                _objActivityLog.FunctionName = "Check Login";
                                _objActivityLog.Transaction_ID = 0;
                                _objActivityLog.WhoAcessed = 0;
                                _objActivityLog.Branch_ID = Obj.Branch_ID;
                                _objActivityLog.Client_ID = Obj.Client_ID;
                            }
                            else
                            {
                                string msg_companyDetails = "";
                                try
                                {
                                    MySqlConnector.MySqlCommand cmd_companyDetails = new MySqlConnector.MySqlCommand("sp_company_details");
                                    cmd_companyDetails.CommandType = CommandType.StoredProcedure;
                                    DataTable dt_companyDetails = db_connection.ExecuteQueryDataTableProcedure(cmd_companyDetails);                                    
                                    foreach (DataRow dr_companyDetails in dt_companyDetails.Rows)
                                    {
                                        if (dr_companyDetails["Company_Email"] != "" && dr_companyDetails["Company_Email"] != null && dr_companyDetails["Company_mobile"] != "" && dr_companyDetails["Company_mobile"] != null)
                                        {
                                            msg_companyDetails = " You can call us at <a href='tel:" + dr_companyDetails["Company_mobile"] + "'>" + dr_companyDetails["Company_mobile"] + "</a> or " +
                                            "send email to <a href='mailto:" + dr_companyDetails["Company_Email"] + "'>" + dr_companyDetails["Company_Email"] + "</a>. Thank You.";
                                        }
                                        else if (dr_companyDetails["Company_mobile"] != "" && dr_companyDetails["Company_mobile"] != null)
                                        {
                                            msg_companyDetails = " You can call us at <a href='tel:" + dr_companyDetails["Company_mobile"] + "'>" + dr_companyDetails["Company_mobile"] + "</a>. Thank You.";
                                        }
                                        else if (dr_companyDetails["Company_Email"] != "" && dr_companyDetails["Company_Email"] != null)
                                        {
                                            msg_companyDetails = " You can send email to <a href='mailto:" + dr_companyDetails["Company_Email"] + "'>" + dr_companyDetails["Company_Email"] + "</a>. Thank You.";
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    msg_companyDetails = "Something went wrong. Please try again later.";
                                }
                                validateJsonData = new { response = true, responseCode = "02", responseMessage = "Email sending failed." + msg_companyDetails };
                                CompanyInfo.InsertActivityLogDetails(msg_companyDetails, 0, 0, 0, 0, "IsValidEmail", Obj.Client_ID, Obj.Branch_ID, "", context);
                                return new JsonResult(validateJsonData);
                            }
                        }
                    }
                    validateJsonData = new { response = true, responseCode = "00", responseMessage = "Mail send successfully", data = Emails };
                    CompanyInfo.InsertActivityLogDetails("Mail send successfully", 0, 0, 0, 0, "IsValidEmail", Obj.Client_ID, Obj.Branch_ID, "", context);
                    return new JsonResult(validateJsonData);
                }
                validateJsonData = new { response = false, responseCode = "02", responseMessage = "Invalid email address." };
                return new JsonResult(validateJsonData);
            }
            catch (Exception ex)
            {
                CompanyInfo.InsertActivityLogDetails("Error:"+ ex.ToString(), 0, 0, 0, 0, "IsValidEmail", 0, 0, "", context);
                validateJsonData = new { response = false, responseCode = "01", data = "Invalid Request" };
                CompanyInfo.InsertErrorLogTracker(ex.ToString(), 0, 0, 0, 0, "IsValidEmail", 0, Convert.ToInt32(0), "", HttpContext);
                return new JsonResult(validateJsonData);
            }
        }
        //Parth changes for getting last Otp of customer for forgot password
        #region Get Last OTP For Customer in Forgot Password method
        public static DataTable GetLastOtpForgotPassword(string userName)
        {
            MySqlConnector.MySqlCommand _cmd_check = new MySqlConnector.MySqlCommand("GetLastOtpForgotPassword");
            _cmd_check.CommandType = CommandType.StoredProcedure;
            _cmd_check.Parameters.AddWithValue("@UserName", userName);  // Passing the email to the procedure
            DataTable result = db_connection.ExecuteQueryDataTableProcedure(_cmd_check);

            return result;
        }
        #endregion
        //End Parth changes for getting last Otp of customer for forgot password
        [HttpPost]
        [Route("validatepassword")]
        public IActionResult validate_password([FromBody] JsonObject Obj)
        {
            Model.response.WebResponse response = null;
            var validateJsonData = (dynamic)null;
            List<Model.Customer> _lst = new List<Model.Customer>();
            string json = System.Text.Json.JsonSerializer.Serialize(Obj);
            dynamic data = JObject.Parse(json);

            Customer obj = JsonConvert.DeserializeObject<Customer>(json);
            _ = Task.Run(() => CompanyInfo.InsertrequestLogTracker("validatepassword full request body: " + obj, 0, 0, 0, 0, "validate_password", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext));
            try
            {
                Service.srvCustomer srv = new Service.srvCustomer();
                _lst = srv.validate_password(obj);
                if (_lst != null)
                {
                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_SUCCESS);
                    response.data = "success";
                    response.ObjData = _lst;
                    response.ResponseCode = 00;
                    validateJsonData = new { response = true, responseCode = "00", data = _lst };
                }
                else
                {
                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_TECHNICAL_ERROR);
                    response.ResponseMessage = "No Records Found.";
                    response.ResponseCode = 02;
                    validateJsonData = new { response = false, responseCode = "02", data = response.ResponseMessage };
                }
            }
            catch (Exception ex)
            {
                validateJsonData = new { response = false, responseCode = "02", data = "No Records Found." };
                _ = Task.Run(() => CompanyInfo.InsertErrorLogTracker(ex.ToString(), 0, 0, 0, 0, "validatepassword", 0, Convert.ToInt32(obj.Client_ID), "", HttpContext));
            }
            return new JsonResult(validateJsonData);
        }

        [HttpPost]
        [Route("updateforgotpassword")]
        public JsonResult UpdateForgotPassword([FromBody] JsonElement Obj)
        {
            Model.response.WebResponse response = null;
            string json = System.Text.Json.JsonSerializer.Serialize(Obj);
            dynamic data = JObject.Parse(json);
            CompanyInfo.InsertrequestLogTracker("updateforgotpassword full request body: " + JObject.Parse(json), 0, 0, 0, 0, "UpdateForgotPassword", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
            var validateJsonData = (dynamic)null;
            Customer obj = JsonConvert.DeserializeObject<Customer>(json);
            string status = "";
            try {
                if (!SqlInjectionProtector.ReadJsonElementValues(Obj) || !SqlInjectionProtector.ReadJsonElementValuesScript(Obj))
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
                if (data.wireTransferReferanceNo == "" || data.wireTransferReferanceNo == null)
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "Invalid ReferanceNo." };
                    return new JsonResult(validateJsonData);
                }
                if (data.checkParameter == "" || data.checkParameter == null)
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "Invalid checkParam." };
                    return new JsonResult(validateJsonData);
                }
                if (!int.TryParse(Convert.ToString(data.branchID), out number) || data.branchID == "" || data.branchID == null)
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "Invalid branch ID." };
                    return new JsonResult(validateJsonData);
                }
                
                obj.newpassword = data.newPassword;
                obj.Client_ID = Convert.ToInt32(data.clientID);
                obj.WireTransfer_ReferanceNo = data.wireTransferReferanceNo;
                obj.checkparam = data.checkParameter;
                obj.Branch_ID = data.branchID;
                obj.AllCustomer_Flag = data.allCustomerFlag;


                try { obj.Captcha = data.Captcha; } catch { obj.Captcha = ""; }
                DataTable probable_dt = (DataTable)CompanyInfo.getEmailPermission(1, 102);
                if (Convert.ToInt32(probable_dt.Rows[0]["PID"]) == 102 && Convert.ToInt32(probable_dt.Rows[0]["Status_ForCustomer"]) == 0)
                {  //Check if CAPTCHA is provided before verification
                    if (!string.IsNullOrEmpty(obj.Captcha))
                    {
                        Service.srvCaptcha srv = new Service.srvCaptcha();
                        bool checkCaptcha = srv.VerifyCaptcha(obj.Captcha);

                        if (!checkCaptcha)
                        {
                            validateJsonData = new { response = false, responseCode = "05", data = "Invalid Captcha." };
                            return new JsonResult(validateJsonData);
                        }
                    }
                }

                /*object o = JsonConvert.DeserializeObject(json);
                bool checkinjection = CompanyInfo.checksqlinjectiondata(o);
                if (!checkinjection)
                {
                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_TECHNICAL_ERROR);
                    response.ResponseMessage = "Invalid Field Values.";
                    response.ResponseCode = 6;
                    validateJsonData = new { response = false, responseCode = "01", data = response.ResponseMessage };
                    return new JsonResult(validateJsonData);
                }*/

                if (obj.newpassword != data.confirmPassword)
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "Password Does not match" };
                    return new JsonResult(validateJsonData);
                }

                string from_querystring = Convert.ToString(obj.WireTransfer_ReferanceNo);
                string split_string = "", ref_no = "";
                if (from_querystring != "" && from_querystring != null)
                {
                    split_string = from_querystring.Replace("reference=", "");
                    ref_no = CompanyInfo.Decrypt(split_string, Convert.ToBoolean(1));
                    if (ref_no == "")
                    {
                        validateJsonData = new { response = false, responseCode = "02", data = "Invalid Referance Number." };
                        return new JsonResult(validateJsonData);
                    }
                }
                string from_querystring1 = Convert.ToString(obj.checkparam);//Anushka
                string split_string1 = "", check_param = "";
                if (from_querystring1 != "" && from_querystring1 != null)
                {
                    split_string1 = from_querystring1.Replace("checkparam=", "");
                    check_param = CompanyInfo.Decrypt(split_string1, Convert.ToBoolean(1));
                }
                string check_flag = "";
                string from_querystring2 = Convert.ToString(obj.AllCustomer_Flag);
                if (from_querystring2 != "" && from_querystring2 != null)
                {
                    check_flag = from_querystring2.Replace("flag=", "");
                    check_flag = CompanyInfo.Decrypt(check_flag, Convert.ToBoolean(1));
                }

                obj.checkparam = check_param;
                DateTime dt_cust1 = DateTime.Now;
                try { dt_cust1 = Convert.ToDateTime(obj.checkparam); }
                catch (Exception ex) {
                    validateJsonData = new { response = false, responseCode = "02", data = "Invalid checkparam field." };
                    return new JsonResult(validateJsonData);
                }

                obj.WireTransfer_ReferanceNo = ref_no;
                MySqlConnector.MySqlCommand cmd_dt = new MySqlConnector.MySqlCommand("customer_details_by_param");
                cmd_dt.CommandType = CommandType.StoredProcedure;
                string where_dt = " and cr.WireTransfer_ReferanceNo = '" + obj.WireTransfer_ReferanceNo + "' and cr.Client_ID=" + obj.Client_ID + "";
                cmd_dt.Parameters.AddWithValue("_whereclause", where_dt);
                cmd_dt.Parameters.AddWithValue("_SecurityKey", CompanyInfo.SecurityKey());
                DataTable dtt1 = db_connection.ExecuteQueryDataTableProcedure(cmd_dt);
                obj.Id = Convert.ToString(dtt1.Rows[0]["Customer_ID"]);
                String Encrypted_time = Convert.ToString(CompanyInfo.gettime(obj.Client_ID, obj.Id, Convert.ToInt32(dtt1.Rows[0]["Country_ID"]), HttpContext));
                DateTime dt1 = Convert.ToDateTime(Encrypted_time);

                //DateTime dt1 = DateTime.Now;
                string check_exp = "";
                if (dt_cust1 >= dt1)
                {
                    obj.Message = "Not expired";
                    check_exp = obj.Message;
                }
                else
                {
                    obj.Message = "expired";
                    check_exp = obj.Message;
                }
                if (check_exp != "expired")
                {
                    obj.WireTransfer_ReferanceNo = ref_no;
                    MySqlConnector.MySqlCommand cmdn = new MySqlConnector.MySqlCommand("customer_details_by_param");
                    cmdn.CommandType = CommandType.StoredProcedure;
                    string where = " and cr.WireTransfer_ReferanceNo = '" + obj.WireTransfer_ReferanceNo + "' and cr.Client_ID=" + obj.Client_ID + "";
                    cmdn.Parameters.AddWithValue("_whereclause", where);
                    cmdn.Parameters.AddWithValue("_SecurityKey", CompanyInfo.SecurityKey());
                    DataTable dtt = db_connection.ExecuteQueryDataTableProcedure(cmdn);
                    if (dtt.Rows.Count > 0)
                    {
                        string custid = Convert.ToString(dtt.Rows[0]["Customer_ID"]);
                        if (custid != null && custid != "")
                        {
                            obj.Id = Convert.ToString(custid);
                            MySqlTransaction transaction;
                            //transaction = con.BeginTransaction(IsolationLevel.ReadCommitted);
                            try
                            {
                                //string IPAddress1 = HttpContext.Connection.RemoteIpAddress.ToString();
                                obj.CommentUserId = 1;
                                obj.SecurityKey = CompanyInfo.SecurityKey();
                                String Encrypted_time1 = Convert.ToString(CompanyInfo.gettime(obj.Client_ID, obj.Id, Convert.ToInt32(dtt.Rows[0]["Country_ID"]), HttpContext));
                                using (MySqlConnector.MySqlCommand cmd_cnt = new MySqlConnector.MySqlCommand("Update_password_count"))
                                {
                                    cmd_cnt.CommandType = CommandType.StoredProcedure;
                                    cmd_cnt.Parameters.AddWithValue("_Date_chk", Encrypted_time1);
                                    cmd_cnt.Parameters.AddWithValue("_Customer_ID", obj.Id);
                                    cmd_cnt.Parameters.AddWithValue("reset_pass_date", Convert.ToDateTime(Encrypted_time1));

                                    DataTable chck_limit = db_connection.ExecuteQueryDataTableProcedure(cmd_cnt);
                                    if (chck_limit.Rows.Count == 1)
                                    {
                                        if (Convert.ToInt32(chck_limit.Rows[0]["reset_pass_cnt"]) <= 3)
                                        {
                                            MySqlConnector.MySqlCommand cmd = new MySqlConnector.MySqlCommand("update_password");

                                            cmd.CommandType = CommandType.StoredProcedure;
                                            var newpassword = Convert.ToString(obj.newpassword);
                                            cmd.Parameters.AddWithValue("_Password", newpassword);
                                            cmd.Parameters.AddWithValue("_Id", obj.Id);
                                            cmd.Parameters.AddWithValue("_Branch_ID", obj.Branch_ID);
                                            cmd.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                                            //cmd.Parameters.AddWithValue("_flag", 0);
                                            cmd.Parameters.AddWithValue("_key", obj.SecurityKey);
                                            int n = db_connection.ExecuteNonQueryProcedure(cmd);

                                            if (check_flag == "SecureFlagOff")
                                            {
                                                obj.Record_Insert_DateTime = CompanyInfo.gettime(obj.Client_ID, custid, Convert.ToInt32(dtt.Rows[0]["Country_Id"]), HttpContext);
                                                using (MySqlConnector.MySqlCommand cmd2 = new MySqlConnector.MySqlCommand("update_securtity_flag"))
                                                {
                                                    cmd2.CommandType = CommandType.StoredProcedure;

                                                    cmd2.Parameters.AddWithValue("_Customer_ID", obj.Id);
                                                    cmd2.Parameters.AddWithValue("_date", obj.Record_Insert_DateTime);
                                                    cmd2.Parameters.AddWithValue("_clientId", obj.Client_ID);
                                                    cmd2.Parameters.AddWithValue("_branchId", obj.Branch_ID);
                                                    cmd2.Parameters.AddWithValue("_security_flag", 1);

                                                    n = db_connection.ExecuteNonQueryProcedure(cmd2);
                                                }
                                            }
                                            Model.Customer _ObjCustomer = new Model.Customer();
                                            Service.srvLogin srvLogin = new Service.srvLogin();
                                            DataTable dt = new DataTable();
                                            Model.Login objLogin = new Model.Login();
                                            Model.Customer objcust = new Model.Customer();

                                            objLogin.Client_ID = obj.Client_ID;
                                            MySqlConnector.MySqlCommand cmd1 = new MySqlConnector.MySqlCommand("GetCustDetailsByID");
                                            cmd1.CommandType = CommandType.StoredProcedure;
                                            cmd1.Parameters.AddWithValue("cust_ID", obj.Id);
                                            DataTable dt_cust = db_connection.ExecuteQueryDataTableProcedure(cmd1);

                                            objLogin.UserName = Convert.ToString(dt_cust.Rows[0]["Email_ID"]);
                                            dt = srvLogin.IsValidEmail(objLogin);

                                            if (n > 0)
                                            {
                                                obj.Message = "Success";
                                                //Activity = "<b>" + Username + "</b>" + " Password changed.  </br>";
                                                //int stattus = (int)CompanyInfo.InsertActivityLogDetails(Activity, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(obj.Id), "UpdatePassword", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "Update Password");

                                                string email = obj.Email;
                                                string body = string.Empty, subject = string.Empty;
                                                string body1 = string.Empty;
                                                //string template = "";
                                                System.Net.HttpWebRequest httpRequest = null, httpRequest1 = null;
                                                DataTable dtc = CompanyInfo.get(obj.Client_ID, HttpContext);
                                                if (dtc.Rows.Count > 0)
                                                {
                                                    string company_name = Convert.ToString(dtc.Rows[0]["Company_Name"]);
                                                    string URL = Convert.ToString(dtc.Rows[0]["Company_URL_Admin"]);
                                                    string cust_url = Convert.ToString(dtc.Rows[0]["Company_URL_Customer"]);
                                                    httpRequest = (System.Net.HttpWebRequest)WebRequest.Create(URL + "Email/my-account-change.html");
                                                    httpRequest.UserAgent = "Code Sample Web Client";
                                                    HttpWebResponse webResponse = (HttpWebResponse)httpRequest.GetResponse();
                                                    using (StreamReader reader = new StreamReader(webResponse.GetResponseStream()))
                                                    {
                                                        body = reader.ReadToEnd();
                                                    }
                                                    obj.Full_Name = Convert.ToString(dt_cust.Rows[0]["Full_name"]);
                                                    body = body.Replace("[name]", Convert.ToString(dt_cust.Rows[0]["First_Name"])); //obj.Full_Name);
                                                    body = body.Replace("[msg]", "Password updated successfully.");

                                                    httpRequest1 = (HttpWebRequest)WebRequest.Create(URL + "Email/my-account.txt");
                                                    httpRequest1.UserAgent = "Code Sample Web Client";
                                                    HttpWebResponse webResponse1 = (HttpWebResponse)httpRequest1.GetResponse();
                                                    using (StreamReader reader = new StreamReader(webResponse1.GetResponseStream()))
                                                    {
                                                        subject = reader.ReadLine();
                                                    }
                                                    string newsubject = company_name + " - " + subject + " - " + Convert.ToString("");
                                                    string msg = (string)CompanyInfo.Send_Mail(dtc, objLogin.UserName, body, newsubject, obj.Client_ID, obj.Branch_ID, "", "", "", HttpContext);
                                                }
                                                validateJsonData = new { response = true, responseCode = "00", data = "Password updated successfully." };
                                                CompanyInfo.InsertActivityLogDetails("Password updated successfully.", 0, 0, 0, Convert.ToInt32(custid), "updateforgotpassword", obj.Client_ID, obj.Branch_ID, "", HttpContext);
                                                return new JsonResult(validateJsonData);

                                            }
                                            validateJsonData = new { response = false, responseCode = "02", data = "Password update failed." };
                                            CompanyInfo.InsertActivityLogDetails("Password update failed.", 0, 0, 0, Convert.ToInt32(custid), "updateforgotpassword", obj.Client_ID, obj.Branch_ID, "", HttpContext);
                                            return new JsonResult(validateJsonData);

                                        }
                                        else
                                        {
                                            validateJsonData = new { response = false, responseCode = "02", data = "You have exceeded the maximum number of password reset attempts." };
                                            CompanyInfo.InsertActivityLogDetails("You have exceeded the maximum number of password reset attempts.", 0, 0, 0, Convert.ToInt32(custid), "updateforgotpassword", obj.Client_ID, obj.Branch_ID, "", HttpContext);
                                            return new JsonResult(validateJsonData);
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                validateJsonData = new { response = false, responseCode = "01", data = "Invalid Request" };
                                CompanyInfo.InsertErrorLogTracker(ex.ToString(), 0, 0, 0, 0, "updateforgotpassword", 0, Convert.ToInt32(obj.Client_ID), "", HttpContext);
                                return new JsonResult(validateJsonData);
                            }
                        }
                    }
                }
                else
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "Time is expired" };
                    return new JsonResult(validateJsonData);
                }
             }
            catch (Exception ex)
            {
                validateJsonData = new { response = false, responseCode = "01", data = "Invalid Request" };
                CompanyInfo.InsertErrorLogTracker(ex.ToString(), 0, 0, 0, 0, "updateforgotpassword", 0, Convert.ToInt32(obj.Client_ID), "", HttpContext);
                return new JsonResult(validateJsonData);
            }
            return new JsonResult(validateJsonData);
         }

        [HttpPost]
        [Authorize]
        [Route("updatepassword")]
        public IActionResult UpdatePassword([FromBody] JsonElement Obj)
        {
            Model.response.WebResponse response = null;
            string json = System.Text.Json.JsonSerializer.Serialize(Obj);
            dynamic data = JObject.Parse(json);
            var claimsIdentity = User.Identity as ClaimsIdentity;//sanket
            var authHeader = "";
            try { authHeader = HttpContext.Request.Headers["Authorization"].ToString(); } catch (Exception egx) { authHeader = ""; }

            bool auth = AuthController.checkAuth(claimsIdentity, Obj, Convert.ToString(authHeader));
            if (!auth)
            {
                var errorResponse = new { response = false, responseCode = "403", data = "Access Forbidden" };
                return new JsonResult(errorResponse) { StatusCode = (int)HttpStatusCode.Forbidden };
            }
            CompanyInfo.InsertrequestLogTracker("updatepassword full request body: " + JObject.Parse(json), 0, 0, 0, 0, "UpdatePassword", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
            var validateJsonData = (dynamic)null;
            Customer obj = JsonConvert.DeserializeObject<Customer>(json);
            try
            {
                if (!SqlInjectionProtector.ReadJsonElementValues(Obj) || !SqlInjectionProtector.ReadJsonElementValuesScript(Obj))
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
                if (!int.TryParse(Convert.ToString(data.branchID), out number) || data.branchID == "" || data.branchID == null)
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "Invalid branchID field." };
                    return new JsonResult(validateJsonData);
                }
                if (data.oldPassword == "" || data.oldPassword == null)
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "Invalid oldPassword field." };
                    return new JsonResult(validateJsonData);
                }
                if (data.newPassword == "" || data.newPassword == null)
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "Invalid newpassword field." };
                    return new JsonResult(validateJsonData);
                }
                if (data.confirmPassword == "" || data.confirmPassword == null)
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "Invalid confirmPassword field." };
                    return new JsonResult(validateJsonData);
                }
                if (!Regex.IsMatch(Convert.ToString(data.newPassword), "[A-Z]"))
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "Password should contain at least one capital letter." };
                    return new JsonResult(validateJsonData);
                }

                if (!Regex.IsMatch(Convert.ToString(data.newPassword), @"[!@#$%^&*()_+{}\[\]:;<>,.?~]"))
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "Password should contain at least one special character." };
                    return new JsonResult(validateJsonData);
                }
                if (Convert.ToString(data.oldPassword) == Convert.ToString(data.newPassword))
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "New password is same as old password." };
                    return new JsonResult(validateJsonData);
                }
                if (data.newPassword != data.confirmPassword)
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "Password does not match" };
                    return new JsonResult(validateJsonData);
                }
                if (Convert.ToString(data.newPassword).Length < 5 || Convert.ToString(data.newPassword).Length > 15)
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "Password length should be between 5 and 15 characters." };
                    return new JsonResult(validateJsonData);
                }

                obj.newpassword = Convert.ToString(data.newPassword);
                obj.Client_ID = Convert.ToInt32(data.clientID);
                obj.Id = data.customerID;
                obj.Branch_ID = data.branchID;
                obj.oldpassword = Convert.ToString(data.oldPassword);

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
               
                int Customer_ID = Convert.ToInt32(CompanyInfo.Decrypt(obj.Id, true));
                string Activity = string.Empty; string notification_icon = ""; string notification_message = "";
                string Username = obj.UserName; string error_msg = "";

                try
                {
                    MySqlConnector.MySqlCommand cmdupdate1 = new MySqlConnector.MySqlCommand("Get_Permissions");
                    cmdupdate1.CommandType = CommandType.StoredProcedure;
                    cmdupdate1.Parameters.AddWithValue("Per_ID", 42);
                    cmdupdate1.Parameters.AddWithValue("ClientID", obj.Client_ID);

                    obj.CommentUserId = 1;

                    DataTable dt1 = db_connection.ExecuteQueryDataTableProcedure(cmdupdate1);

                    if (dt1.Rows.Count == 0)
                    {
                        validateJsonData = new { response = false, responseCode = "01", data = "Record not found" };                        
                        return new JsonResult(validateJsonData);
                    }


                    obj.per_status = Convert.ToInt32(dt1.Rows[0]["Status_ForCustomer"]);
                    // var per=0;
                    //    if (obj.per_status == 1)
                    //     {
                    obj.SecurityKey = CompanyInfo.SecurityKey();
                    MySqlConnector.MySqlCommand cmdchckold = new MySqlConnector.MySqlCommand("get_oldpassword");
                    cmdchckold.CommandType = CommandType.StoredProcedure;
                    cmdchckold.Parameters.AddWithValue("_Id", Customer_ID);
                    cmdchckold.Parameters.AddWithValue("_Branch_ID", obj.Branch_ID);
                    cmdchckold.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                    cmdchckold.Parameters.AddWithValue("_key", obj.SecurityKey);
                    DataTable db = db_connection.ExecuteQueryDataTableProcedure(cmdchckold);
                     string PASSWORD =  Convert.ToString(db.Rows[0]["PASSWORD"]);
                    if (db.Rows.Count > 0)
                    {
                        if (obj.oldpassword != PASSWORD)
                        {
                            validateJsonData = new { response = false, responseCode = "02", data = "Old password is not matched." };
                            return new JsonResult(validateJsonData);
                        }
                    }

                    MySqlConnector.MySqlCommand cmd = new MySqlConnector.MySqlCommand("update_password");

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("_Password", Convert.ToString(obj.newpassword));
                    cmd.Parameters.AddWithValue("_Id", Customer_ID);
                    cmd.Parameters.AddWithValue("_Branch_ID", obj.Branch_ID);
                    cmd.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                    cmd.Parameters.AddWithValue("_key", obj.SecurityKey);
                    int n = db_connection.ExecuteNonQueryProcedure(cmd);

                    Model.Customer _ObjCustomer = new Model.Customer();
                        Service.srvLogin srvLogin = new Service.srvLogin();
                        DataTable dt = new DataTable();
                        Model.Login objLogin = new Model.Login();
                        Model.Customer objcust = new Model.Customer();

                        objLogin.Client_ID = obj.Client_ID;
                        MySqlConnector.MySqlCommand cmd1 = new MySqlConnector.MySqlCommand("GetCustDetailsByID");
                        cmd1.CommandType = CommandType.StoredProcedure;
                        cmd1.Parameters.AddWithValue("cust_ID", Customer_ID);

                        DataTable dt_cust = db_connection.ExecuteQueryDataTableProcedure(cmd1);
                        objLogin.UserName = Convert.ToString(dt_cust.Rows[0]["Email_ID"]);
                        dt = srvLogin.IsValidEmail(objLogin);

                    if (n > 0)
                    {
                        notification_icon = "password.jpg";
                        notification_message = "<span class='cls-admin'>successfully changed <strong class='cls-change-pass'>Password</strong>.</span><span class='cls-customer'><strong>Password changed</strong><span>You have successfully changed your password.</span></span>";
                        // CompanyInfo.save_notification(notification_message, notification_icon, Convert.ToInt32(Customer_ID), Convert.ToDateTime(obj.RecordDate), Convert.ToInt32(obj.Client_ID), 1, 0, Convert.ToInt32(obj.Branch_ID), 0, 0, 1, 0);

                        Activity = "<b>" + Username + "</b>" + "Password changed.  </br>";
                        // CompanyInfo.InsertActivityLogDetails(Activity, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(Customer_ID), "UpdatePassword", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "Update Password");


                        string email = obj.Email;
                        string body = string.Empty, subject = string.Empty;
                        string body1 = string.Empty;
                        //string template = "";
                        System.Net.HttpWebRequest httpRequest = null, httpRequest1 = null;
                        DataTable dtc = CompanyInfo.get(obj.Client_ID, HttpContext);
                        if (dtc.Rows.Count > 0)
                        {

                            string company_name = Convert.ToString(dtc.Rows[0]["Company_Name"]);
                            string URL = Convert.ToString(dtc.Rows[0]["Company_URL_Admin"]);
                            string cust_url = Convert.ToString(dtc.Rows[0]["Company_URL_Customer"]);
                            httpRequest = (System.Net.HttpWebRequest)WebRequest.Create(URL + "Email/my-account-change.html");
                            httpRequest.UserAgent = "Code Sample Web Client";
                            HttpWebResponse webResponse = (HttpWebResponse)httpRequest.GetResponse();
                            using (StreamReader reader = new StreamReader(webResponse.GetResponseStream()))
                            {
                                body = reader.ReadToEnd();
                            }
                            obj.Full_Name = Convert.ToString(dt_cust.Rows[0]["Full_name"]);
                            body = body.Replace("[name]", Convert.ToString(dt_cust.Rows[0]["First_Name"])); //obj.Full_Name);
                            body = body.Replace("[msg]", "Password updated successfully.");

                            httpRequest1 = (HttpWebRequest)WebRequest.Create(URL + "Email/my-account.txt");
                            httpRequest1.UserAgent = "Code Sample Web Client";
                            HttpWebResponse webResponse1 = (HttpWebResponse)httpRequest1.GetResponse();
                            using (StreamReader reader = new StreamReader(webResponse1.GetResponseStream()))
                            {
                                subject = reader.ReadLine();
                            }
                            string newsubject = company_name + " - " + subject + " - " + Convert.ToString("");
                            string msg = (string)CompanyInfo.Send_Mail(dtc, objLogin.UserName, body, newsubject, obj.Client_ID, obj.Branch_ID, "", "", "", HttpContext);
                        }
                    }
                }
                catch (Exception ex)
                {
                    validateJsonData = new { response = false, responseCode = "01", data = "Invalid Request" };
                    CompanyInfo.InsertErrorLogTracker(ex.ToString(), 0, 0, 0, 0, "UpdatePassword", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "", HttpContext);
                    return new JsonResult(validateJsonData);
                }
                validateJsonData = new { response = true, responseCode = "00", data = "Password Updated" };
                return new JsonResult(validateJsonData);
            }
            catch (Exception ex)
            {
                validateJsonData = new { response = false, responseCode = "01", data = "Invalid Request" };
                CompanyInfo.InsertErrorLogTracker(ex.ToString(), 0, 0, 0, 0, "UpdatePassword", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "", HttpContext);
                return new JsonResult(validateJsonData);
            }
           }

        [HttpPost]
        [Authorize]
        [Route("resendmail")]
        public IActionResult ResendMail([FromBody] JsonElement obj)
        {
            
            HttpContext context = HttpContext;
            Model.response.WebResponse response = null;
            Login obj1 = new Login();
            string json = System.Text.Json.JsonSerializer.Serialize(obj);
            dynamic data = JObject.Parse(json);
            //DeliveryTypeController.Getdeliverytype(data);
            CompanyInfo.InsertrequestLogTracker("resendmail full request body: " + JObject.Parse(json), 0, 0, 0, 0, "ResendMail", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
            var validateJsonData = (dynamic)null;
            Login Obj = JsonConvert.DeserializeObject<Login>(json);
            try
            {
                if (!SqlInjectionProtector.ReadJsonElementValues(obj) || !SqlInjectionProtector.ReadJsonElementValuesScript(obj))
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
                else if (data.userName == "" || data.userName == null)
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "Invalid userName field." };
                    return new JsonResult(validateJsonData);
                }
                else if (!int.TryParse(Convert.ToString(data.branchID), out number) || data.branchID == "" || data.branchID == null)
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "Invalid branchID field." };
                    return new JsonResult(validateJsonData);
                }

                Obj.UserName = data.userName;
                Obj.Client_ID = data.clientID;
                Obj.Branch_ID = data.branchID;
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

                string Email_regex = validation.validate(Obj.UserName, 1, 1, 1, 1, 0, 1, 1, 1, 1);
                if (Email_regex == "false")
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "Email Should be valid " };
                    return new JsonResult(validateJsonData);
                }
                string msg = "";
                List<Model.Login> _lst = new List<Model.Login>();
                DataTable dt = new DataTable();
                MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("CustomerDetailsByEmail");
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.Parameters.AddWithValue("_Email_ID", Obj.UserName.Trim());
                _cmd.Parameters.AddWithValue("_Client_ID", Obj.Client_ID);
                dt = db_connection.ExecuteQueryDataTableProcedure(_cmd);

                List<Dictionary<string, object>> Emails = new List<Dictionary<string, object>>();

                foreach (DataRow dr in dt.Rows)
                {
                    if (dr != null)
                    {
                        Dictionary<string, object> email = new Dictionary<string, object>();
                        email["fullName"] = dr["full_name"];

                        //string subject = string.Empty;
                        string body = string.Empty, subject = string.Empty;
                        string body1 = string.Empty;
                        string template = "";
                        HttpWebRequest httpRequest = null, httpRequest1 = null;
                        DataTable dtc = new DataTable();
                        MySqlConnector.MySqlCommand cmd = new MySqlConnector.MySqlCommand("Get_CompanyInfo");
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("_Client_ID", Obj.Client_ID);
                        cmd.Parameters.AddWithValue("_Customer_ID", 0);
                        cmd.Parameters.AddWithValue("_SecurityKey", CompanyInfo.SecurityKey());
                        dtc = db_connection.ExecuteQueryDataTableProcedure(cmd);
                        if (dtc.Rows.Count > 0)
                        {

                            string URL = Convert.ToString(dtc.Rows[0]["Company_URL_Admin"]);
                            string cust_url = Convert.ToString(dtc.Rows[0]["Company_URL_Customer"]);
                            httpRequest = (HttpWebRequest)WebRequest.Create(URL + "Email/verify-email.html");
                            httpRequest.UserAgent = "Code Sample Web Client";
                            HttpWebResponse webResponse = (HttpWebResponse)httpRequest.GetResponse();
                            using (StreamReader reader = new StreamReader(webResponse.GetResponseStream()))
                            {
                                body = reader.ReadToEnd();
                            }
                            body = body.Replace("[name]", Convert.ToString(dt.Rows[0]["First_Name"])); 
                            string ref_no = Convert.ToString(dt.Rows[0]["WireTransfer_ReferanceNo"]);
                            string str = CompanyInfo.Encrypt1(ref_no);
                            body = body.Replace("[link]", cust_url + "verify-email?reference=" + str + ""); 
                            httpRequest1 = (HttpWebRequest)WebRequest.Create(URL + "Email/verify-email.txt");
                            httpRequest1.UserAgent = "Code Sample Web Client";
                            HttpWebResponse webResponse1 = (HttpWebResponse)httpRequest1.GetResponse();
                            using (StreamReader reader = new StreamReader(webResponse1.GetResponseStream()))
                            {
                                subject = reader.ReadLine();
                            }
                            msg = (string)CompanyInfo.Send_Mail(dtc, Obj.UserName, body, subject, Obj.Client_ID, Obj.Branch_ID, "", "", "",HttpContext);
                            if (msg == "Success")
                            {
                                response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_SUCCESS);
                                response.data = "success";
                                response.ResponseCode = 0;
                                Model.ActivityLogTracker _objActivityLog = new Model.ActivityLogTracker();
                                _objActivityLog.Activity = "Email Verify mail sent to " + Obj.UserName + " ";
                                _objActivityLog.FunctionName = "Send_verify_mail";
                                _objActivityLog.Transaction_ID = 0;
                                _objActivityLog.WhoAcessed = 0;
                                _objActivityLog.Branch_ID = Obj.Branch_ID;
                                _objActivityLog.Client_ID = Obj.Client_ID;
                                Service.srvActivityLogTracker srvActivityLog = new Service.srvActivityLogTracker();
                                int _i = srvActivityLog.Create(_objActivityLog,HttpContext);
                            }
                            else
                            {
                                response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_TECHNICAL_ERROR);
                                response.ResponseMessage = "Something went wrong. Please try again later.";
                                response.ResponseCode = 1;
                                response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_SUCCESS);
                                response.data = "success";
                                response.ResponseCode = 0;
                                Model.ActivityLogTracker _objActivityLog = new Model.ActivityLogTracker();
                                _objActivityLog.Activity = "Email Verify mail not sent to " + Obj.UserName + " ";
                                _objActivityLog.FunctionName = "Send_verify_mail";
                                _objActivityLog.Transaction_ID = 0;
                                _objActivityLog.WhoAcessed = 0;
                                _objActivityLog.Branch_ID = Obj.Branch_ID;
                                _objActivityLog.Client_ID = Obj.Client_ID;
                                Service.srvActivityLogTracker srvActivityLog = new Service.srvActivityLogTracker();
                                int _i = srvActivityLog.Create(_objActivityLog,HttpContext);
                                validateJsonData = new { response = false, responseCode = "02", responseMessage =" Email Verify mail not sent to " + Obj.UserName + "" };
                                return new JsonResult(validateJsonData);
                            }
                        }
                        else
                        {
                            response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_TECHNICAL_ERROR);
                            response.ResponseMessage = "Something went wrong. Please try again later.";
                            response.ResponseCode = 2;
                            validateJsonData = new { response = false, responseCode = "02", responseMessage = "Something went wrong. Please try again later." };
                            return new JsonResult(validateJsonData);
                        }
                        validateJsonData = new { response = true, responseCode = "00", responseMessage = "Email Verify mail sent to " + Obj.UserName + " " };
                        CompanyInfo.InsertActivityLogDetails("Mail send successfully", 0, 0, 0, 0, "IsValidEmail", Obj.Client_ID, Obj.Branch_ID, "", context);
                        return new JsonResult(validateJsonData);
                    } 
                }
                validateJsonData = new { response = false, responseCode = "02", responseMessage = "Something went wrong. Please try again later." };
                return new JsonResult(validateJsonData);
            }
            catch (Exception ex)
            {
                validateJsonData = new { response = false, responseCode = "01", data = "Invalid Request" };
                CompanyInfo.InsertErrorLogTracker(ex.ToString(), 0, 0, 0, 0, "IsValidEmail", 0, Convert.ToInt32(Obj.Client_ID), "", HttpContext);
                return new JsonResult(validateJsonData);
            }
        }


    }
}
