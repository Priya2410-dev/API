using System.Data;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
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
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Http.Extensions;
using Borland.Vcl;
using System.Linq;
using System.Web.Http.Results;
using System.Data.Common;
 
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using System.Web.Helpers;




// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Calyx_Solutions.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class UserLoginController : ControllerBase
    {

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;

        public UserLoginController(IConfiguration  configuration , IHttpContextAccessor  httpContextAccessor)
        {
            this._configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public string bb( )
        {
            return "g";
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public string userAgent()
        {
            string userAgent = Request.Headers[HeaderNames.UserAgent];
            return userAgent;
        }

        // GET: api/<UserLoginController>

        [HttpGet]
        [Authorize]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<UserLoginController>/5
        [HttpGet("{id}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public string Get(int id)
        {
            return Convert.ToString(id);  
        }



        [HttpGet]
        [Route("loginconfiguration")]
        public IActionResult loginconfiguration(Model.Transaction obj)
        {

            try
            {
                MySqlConnector.MySqlCommand _cmdl = new MySqlConnector.MySqlCommand("GetPermissions");
                _cmdl.CommandType = CommandType.StoredProcedure;
                _cmdl.Parameters.AddWithValue("_whereclause", " and PID in (102,135,136)");
                _cmdl.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                db_connection objdbConn = new db_connection();
                DataTable all_per = db_connection.ExecuteQueryDataTableProcedure(_cmdl);
                DataTable dt5 = new DataTable();

                int chkdailycount = 1;
                DataRow[] li1_captcha = all_per.Select("PID=102");              
                DataRow[] paascode_list = all_per.Select("PID=135");//passcode
                DataRow[] biometric_list = all_per.Select("PID=136");//biometric

                int Biometric_per = Convert.ToInt32(biometric_list[0]["Status_ForCustomer"]);
                int Passcode_per = Convert.ToInt32(paascode_list[0]["Status_ForCustomer"]);

                var jsonData = new { response = false, responseCode = "02", data = "Invalid Client_ID" };
                return new JsonResult(jsonData);

            }
            catch (Exception ex)
            {
                var jsonData = new { response = false, responseCode = "02", data = "Invalid Client_ID" };
                return new JsonResult(jsonData);
            }
        }



            // POST api/CheckLogin
            [HttpPost]
        [Route("CheckLogin")]
        
       // public JsonResult CheckLogin([FromBody] JsonElement objdata)
        public IActionResult CheckLogin([FromBody] JsonElement objdata)
        {
            HttpContext context =  HttpContext;
            Model.response.WebResponse response = null;
            var jsonData = new { };
            try
            {
                string path_query = CompanyInfo.path_query2(HttpContext);
                string fullUrl = this.HttpContext.Request.GetDisplayUrl();

                int index = fullUrl.IndexOf("/api");
                if (index > 0)
                {
                    fullUrl = fullUrl.Substring(0, index);
                    fullUrl =  fullUrl + "/api/auth";
                }
                
                Login userLoginObj = new Login();
                string json = System.Text.Json.JsonSerializer.Serialize(objdata);
                dynamic data = JObject.Parse(json);
                _ = Task.Run(() => CompanyInfo.InsertrequestLogTracker("API Login Request body: " + JObject.Parse(json), 0, 0, 0, 0, "UserLoginController", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext));
               
                var validateJsonData = (dynamic)null;
                try
                {
                    //Start Parth added for proper fields validation
                    #region proper fields validation
                    if (!SqlInjectionProtector.ReadJsonElementValues_SingleElement(objdata).isValid)
                    {
                        string fieldName = string.Empty;

                        var sqlValidationResult = SqlInjectionProtector.ReadJsonElementValues_SingleElement(objdata);   // Checked SQL injection validation
                        if (!sqlValidationResult.isValid)
                        {
                            fieldName = sqlValidationResult.fieldName;  // Captured the field name from the validation
                        }

                        string errorMessage = fieldName switch
                        {
                            "userName" => "Invalid input detected for Email address",
                            "password" => "Invalid input detected for Password",
                            "credentialFlag" => "Invalid input detected for Credential Flag",
                            "otp" => "Invalid input detected for OTP",
                            "resendotp" => "Invalid input detected for Resend OTP",                           
                            "latitude" => "Invalid input detected for Latitude",
                            "longitude" => "Invalid input detected for Longitude",
                            "deviceId" => "Invalid input detected for Device ID",
                            "ipAddress" => "Invalid input detected for IP Address",
                            _ => "Invalid input detected for " + fieldName
                        };

                        if (errorMessage != null)
                        {
                            var errorResponse = new
                            {
                                response = false,
                                responseCode = "02",
                                data = errorMessage
                            };
                            return new JsonResult(errorResponse) { StatusCode = 400 };
                        }
                    }
                    #endregion proper fields validation
                    //End Parth added for proper fields validation

                   

                    int number;
                    if (!int.TryParse(Convert.ToString(data.clientID), out number))
                    {
                        validateJsonData = new { response = false, responseCode = "02", data = "Invalid Client ID." };
                        return new JsonResult(validateJsonData);
                    }

                    else if (data.userName == "" || data.userName == null)
                    {
                        validateJsonData = new { response = false, responseCode = "02", data = "Invalid UserName." };
                        return new JsonResult(validateJsonData);
                    }
                    else if (data.password == "" || data.password == null && data.credentialFlag == "")
                    {
                        validateJsonData = new { response = false, responseCode = "02", data = "Invalid Password." };
                        return new JsonResult(validateJsonData);
                    }
                    else if (data.password == "" || data.password == null && data.credentialFlag == "1")
                    {
                        validateJsonData = new { response = false, responseCode = "02", data = "Please submit Passcode." };
                        return new JsonResult(validateJsonData);
                    }
                    else if (!int.TryParse(Convert.ToString(data.branchID), out number))
                    {
                        validateJsonData = new { response = false, responseCode = "02", data = "Invalid Branch ID." };
                        return new JsonResult(validateJsonData);
                    }
                }
                catch (Exception ex)
                {
                    Model.ActivityLogTracker _objActivityLog = new Model.ActivityLogTracker();
                    _objActivityLog.Activity = " Exception Found User login: " + ex.ToString() + " ";
                    _objActivityLog.FunctionName = "Check Login";
                    _objActivityLog.Transaction_ID = 0;
                    _objActivityLog.WhoAcessed = 1;
                    _objActivityLog.Branch_ID = 0;
                    _objActivityLog.Client_ID = 0;

                    Service.srvActivityLogTracker srvActivityLog = new Service.srvActivityLogTracker();
                    int _i = srvActivityLog.Create(_objActivityLog, context);

                    validateJsonData = new { response = false, responseCode = "02", data = "Field is missing." };
                    return new JsonResult(validateJsonData);
                }


                Login obj = JsonConvert.DeserializeObject<Login>(json);

                

                string userAgent = Request.Headers[HeaderNames.UserAgent];
                // Stating
                string tokenRequest = "F";
                string tokenValue = null;
                obj.tokenValue = tokenValue;
                obj.tokenRequest = tokenRequest;
                obj.userAgent = userAgent;


                obj.UserName = Convert.ToString(data.userName).Trim();
                obj.Password = Convert.ToString(data.password).Trim();
                obj.Client_ID = data.clientID;
                obj.Branch_ID = data.branchID;
                obj.CredentialFlag = data.credentialFlag;
                try
                {
                    obj.OTP = data.otp;
                    obj.Resend_otp = data.resendotp;
                    obj.Captcha = data.captcha;
                }
                catch { }

                try
                {
                    obj.latitude = data.latitude;
                    
                }
                catch { }
                try { obj.ipAddress = data.ipAddress; } catch { }
                try { obj.deviceId = data.deviceId; } catch { }
                try { obj.longitude = data.longitude; } catch { }

                // Check where request from
                try { obj.flutterval = data.flutterval; } catch { obj.flutterval = ""; }

                /*if (!SqlInjectionProtector.ValidateObjectForSqlInjection(obj) || !SqlInjectionProtector.ValidateObjectForScriptSqlInjection(obj))
                {
                    var errorResponse = new
                    { response = false, responseCode = "02", data = "Invalid input detected." };
                    return new JsonResult(errorResponse) { StatusCode = 400 };
                }*/

                Service.srvLogin srvLogin = new Service.srvLogin();
                List<Model.Customer> LstCustomer = new List<Model.Customer>();
                //LstCustomer = srvLogin.IsValidCustomer(obj, context);

                var host = $"{context.Request.Scheme}://{context.Request.Host}";                
                LstCustomer = srvLogin.IsValidCustomer(obj, context);
                

                Model.Login _LoginObj = new Model.Login();

                if (LstCustomer != null && LstCustomer.Count != 0)
                {
                    var _LoginItem = LstCustomer.SingleOrDefault();
                    _ = Task.Run(() => CompanyInfo.InsertActivityLogDetailsasync("API Login Flag : " + Convert.ToString(_LoginItem.Flag)+" Login Id :"+ Convert.ToString(_LoginItem.Id), Convert.ToInt32(0), 0, Convert.ToInt32(0), 0, "UserLoginController", Convert.ToInt32(0), 1, "CheckLogin", HttpContext));
                    if (_LoginItem.Flag == 1)
                    {

                        response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_TECHNICAL_ERROR);
                        response.ResponseMessage = "security on riskt";
                        response.ResponseCode = 5;
                    }
                    else if (_LoginItem.Flag == 2  )
                    {
                        response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_TECHNICAL_ERROR);
                        response.ResponseCode = 2;
                    }
                    else if (_LoginItem.Flag == 3)
                    {
                        response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_TECHNICAL_ERROR);
                        response.ResponseMessage = "Verification code is sent to your email address.Also check spam mail for verifcation code.";
                        response.ResponseCode = 3;
                    }
                    else if (_LoginItem.Flag == 4)
                    {
                        response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_TECHNICAL_ERROR);
                        response.ResponseMessage = "The verification code you entered is either invalid or has expired. Please check the code entered or resend if not received.";
                        response.ResponseCode = 4;
                    }
                    else if (_LoginItem.Flag == 5)
                    {
                        response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_TECHNICAL_ERROR);
                        response.ResponseCode = 5;
                    }
                    else if (_LoginItem.Flag == 6)
                    {
                        response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_TECHNICAL_ERROR);                        
                        response.ResponseCode = 6;
                    }
                    else if (_LoginItem.Flag == 7)
                    {
                        response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_TECHNICAL_ERROR);
                        response.ResponseCode = 7;
                    }
                    else if (_LoginItem.Email != null)
                    {
                        if (obj.UserName.ToLower() == _LoginItem.Email.ToLower() && obj.Password == _LoginItem.Password && obj.UserName != "" && obj.Password != "")
                        if (obj.UserName.ToLower() == _LoginItem.Email.ToLower() && obj.Password == _LoginItem.Password && obj.UserName != "" && obj.Password != "")
                        {
                            _LoginObj.UserName = _LoginItem.Email;
                            _LoginObj.Password = _LoginItem.Password;
                            _LoginObj.Customer_ID = CompanyInfo.Encrypt(Convert.ToString(_LoginItem.Id), true);
                            //context.Session["Customer_ID"] = _LoginItem.Id;//_LoginObj.Customer_ID = _LoginItem.Id;
                                                        
                            CompanyInfo.setSessionHttpValStr("Customer_ID", Convert.ToString(_LoginItem.Id), context);

                            _LoginObj.FullName = _LoginItem.Name;
                            _LoginObj.Branch_ID = obj.Branch_ID;
                            _LoginObj.Client_ID = _LoginItem.Client_ID;
                            _LoginObj.WireTransferRefNumber = _LoginItem.WireTransfer_ReferanceNo;
                            _LoginObj.DocUploadCount = _LoginItem.DocumentUploadCount;
                            _LoginObj.Mobile_Verification_flag = _LoginItem.Mobile_Verification_flag;
                            _LoginObj.Currency_Code = _LoginItem.Currency_Code;
                            _LoginObj.Currency_Id = _LoginItem.Currency_Id;

                            _LoginObj.Currency_Sign = _LoginItem.Currency_Sign;
                            _LoginObj.Country_Flag = _LoginItem.Country_Flag;
                            _LoginObj.Country_Name = _LoginItem.Country_Name;
                            _LoginObj.Country_Id = _LoginItem.Country_Id;
                            _LoginObj.Agent_User_ID = _LoginItem.Agent_User_ID;
                            _LoginObj.Agent_branch = _LoginItem.Agent_branch;
                            _LoginObj.New_Agent_Branch = _LoginItem.New_Agent_Branch;

                            

                                if (_LoginItem.block_login_flag == 0)
                            {
                                response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_SUCCESS);
                                response.data = "success";
                                response.ObjData = _LoginObj;
                                response.ResponseCode = 0;
                                response.RedirectUrl = "app-dashboard.html";//response.RedirectUrl = "transaction-list.html";//
                                SaveLoginDetails(_LoginObj,context);
                                                                   
                                }
                            else
                            {
                                response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_TECHNICAL_ERROR);
                                response.ResponseMessage = "Having trouble with login? Please try Forgot Password or contact our Support Team.";
                                response.ResponseCode = 4;

                                    string message = "";
                                    try
                                    {
                                        DataTable dtcompanydetails = (DataTable)CompanyInfo.GetBaseCurrencywisebankdetails(_LoginObj.Client_ID, "");

                                        if (dtcompanydetails != null && dtcompanydetails.Rows.Count > 0)
                                        {
                                            foreach (DataRow drk in dtcompanydetails.Rows)
                                            {
                                                if (drk["Company_Email"] != "" && drk["Company_Email"] != null && drk["Company_mobile"] != "" && drk["Company_mobile"] != null)
                                                {
                                                    message = " You can call us at <a href='tel:" + drk["Company_mobile"] + "'>" + drk["Company_mobile"] + "</a> or " +
                                    "send email to <a href='mailto:" + drk["Company_Email"] + "'>" + drk["Company_Email"] + "</a>. Thank You.";
                                                }
                                                else if (drk["Company_mobile"] != "" && drk["Company_mobile"] != null)
                                                {
                                                    message = message + " You can call us at <a href='tel:" + drk["Company_mobile"] + "'>" + drk["Company_mobile"] + "</a>. Thank You.";
                                                }
                                                else if (drk["Company_Email"] != "" && drk["Company_Email"] != null)
                                                {
                                                    message = message + " You can send email to <a href='mailto:" + drk["Company_Email"] + "'>" + drk["Company_Email"] + "</a>. Thank You.";
                                                }
                                                break;
                                            }
                                        }
                                    }
                                    catch (Exception egx) { }
                                    response.ResponseMessage = response.ResponseMessage + message;
                                }
                        }
                        else
                        {
                            response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_TECHNICAL_ERROR);
                            response.ResponseMessage = "Username or Password is incorrect";
                                if(data.credentialFlag == "1")
                                {
                                    response.ResponseMessage = "Username or Passcode is incorrect";
                                }
                                if (data.credentialFlag == "2")
                                {
                                    response.ResponseMessage = "Unable to login.";
                                }

                                response.ResponseCode = 1;                                
                            }
                    }
                    else
                    {
                        response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_TECHNICAL_ERROR);
                        response.ResponseMessage = "Username or Password is incorrect";
                        if (data.credentialFlag == "1")
                        {
                            response.ResponseMessage = "Username or Passcode is incorrect";
                        }
                        if (data.credentialFlag == "2")
                        {
                            response.ResponseMessage = "Unable to login.";
                        }
                        response.ResponseCode = 1;                        
                    }
                    _LoginObj.Captcha = Convert.ToString(_LoginItem.Retry_Cnt);
                    response.ObjData = _LoginObj;

                    if (response.ResponseCode == 0)
                    {
                        // Create Authentication here
                        try
                        {
                            string ipAddress = context.Connection.RemoteIpAddress.ToString();
                            string capIp = ipAddress;
                            string Loginct_IPAdress = capIp;
                            string Loginct_Email = obj.UserName;
                            int Loginct_IpAddress_count = 0;
                            DateTime Loginct_Date = DateTime.Now;
                            DateTime Loginip_Date = DateTime.Now;
                            int Loginct_email_count = 0;

                            MySqlConnector.MySqlCommand _cmdh = new MySqlConnector.MySqlCommand();
                            _cmdh = new MySqlConnector.MySqlCommand("UpdateLoginCounts");
                            _cmdh.CommandType = CommandType.StoredProcedure;
                            _cmdh.Parameters.AddWithValue("_Email", Loginct_Email);
                            _cmdh.Parameters.AddWithValue("_IPAdress", Loginct_IPAdress);
                            _cmdh.Parameters.AddWithValue("_email_count", Loginct_email_count);
                            _cmdh.Parameters.AddWithValue("_IpAddress_count", Loginct_IpAddress_count);
                            _cmdh.Parameters.AddWithValue("_Date", Loginct_Date);
                            string success = Convert.ToString(db_connection.ExecuteNonQueryProcedure(_cmdh));
                        }
                        catch (Exception egx) { }
                       

                        string webstring = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("ConnectionStrings")["APIName"];
                        webstring = webstring.ToLower();
                        if (webstring.IndexOf("ifepayapp", 0, StringComparison.OrdinalIgnoreCase) != -1)
                        {
                            fullUrl = "https://ifepay.co.uk/apilive/api/auth";
                        }

                        var client = new RestClient(fullUrl);
                        client.Timeout = -1;
                        var request = new RestRequest(Method.POST);
                        string passedEmail = "\"" + obj.UserName.Trim() + "\"";
                        string passedPassword = "\"" + obj.Password.Trim() + "\"";
                        request.AddHeader("Content-Type", "application/json");
                        var body = @"{
                                                " + "\n" +
                                    @"    ""userEmail"" : " + passedEmail + ", " +
                                    @"    ""userPassword"" : " + passedPassword + " " + @"}";
                        request.AddParameter("application/json", body, ParameterType.RequestBody);

                        ServicePointManager.Expect100Continue = true;
                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
                               | SecurityProtocolType.Tls11
                               | SecurityProtocolType.Tls12;

                        IRestResponse responseg = client.Execute(request);
                        string token = responseg.Content.Replace("\"", "");
                     
                        if (token == "" || true)
                        {                        
                            var issuer = _configuration["Jwt:Issuer"];
                            var audience = _configuration["Jwt:Audience"];
                            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:SecurityKey"]);
                            var signingCredentials = new SigningCredentials(
                                                    new SymmetricSecurityKey(key),
                                                    SecurityAlgorithms.HmacSha512Signature
                                                );
                            /*  var subject = new ClaimsIdentity(new[]
                                   {
                          new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub, passedEmail),
                          new Claim(JwtRegisteredClaimNames.Email, passedEmail),
                          }); */

                            var jti = Guid.NewGuid().ToString();
                            var subject = new ClaimsIdentity(new[]
                                 {
                        new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub, passedEmail),
                        new Claim(JwtRegisteredClaimNames.Email, passedEmail),
                        new Claim("UserName", _LoginObj.UserName),//sanket
                        new Claim("Customer_ID", _LoginObj.Customer_ID),//sanket
                        new Claim("Client_ID", Convert.ToString(_LoginObj.Client_ID)),//sanket
                        new Claim("Country_Id", Convert.ToString(_LoginObj.Country_Id)),//sanket
                        new Claim( JwtRegisteredClaimNames.Jti, jti),
            new Claim(JwtRegisteredClaimNames.Exp, new DateTimeOffset(DateTime.UtcNow.AddMinutes(120)).ToUnixTimeSeconds().ToString())



                        });

                            var expires = DateTime.UtcNow.AddMinutes(1);

                            var tokenDescriptor = new SecurityTokenDescriptor
                            {
                                Subject = subject,
                                Expires = DateTime.UtcNow.AddMinutes(120),
                                Issuer = issuer,
                                Audience = audience,
                                SigningCredentials = signingCredentials
                            };
                            var tokenHandler = new JwtSecurityTokenHandler();                            
                            var token_Value = tokenHandler.CreateToken(tokenDescriptor);
                            token = tokenHandler.WriteToken(token_Value);
                        }
                        
                        var dataList = new Dictionary<string, object>()
                                {
                                    { "customerID", CompanyInfo.Encrypt(Convert.ToString(_LoginItem.Id).Trim(), true) },
                                    { "userName", Convert.ToString(_LoginItem.Email).Trim() },
                                    { "fullName", Convert.ToString(_LoginItem.FirstName + " "+_LoginItem.LastName).Trim() },
                                    { "branchID", Convert.ToString(_LoginObj.Branch_ID) },
                                    { "clientID", Convert.ToString(_LoginItem.Client_ID) },
                                    { "WireTransferRefNumber", Convert.ToString(_LoginItem.WireTransfer_ReferanceNo) },
                                    { "custRefNumber",  CompanyInfo.Encrypt(Convert.ToString(_LoginItem.WireTransfer_ReferanceNo).Trim(), true)  },
                                    { "docUploadCount", Convert.ToString(_LoginItem.DocumentUploadCount) },
                                    { "mobileVerificationFlag", Convert.ToString(_LoginItem.Mobile_Verification_flag) },
                                    { "currencyCode", Convert.ToString(_LoginItem.Currency_Code) },
                                    { "currencyID", Convert.ToString(_LoginItem.Currency_Id) },
                                    { "currencySign", Convert.ToString(_LoginItem.Currency_Sign) },
                                    { "countryFlag", Convert.ToString(_LoginItem.Country_Flag) },
                                    { "countryName", Convert.ToString(_LoginItem.Country_Name) },
                                    { "countryID", Convert.ToString(_LoginItem.Country_Id) },
                                    { "agentUserID", Convert.ToString(_LoginItem.Agent_User_ID) },
                                    { "agentBranch", Convert.ToString(_LoginItem.Agent_branch) },
                                    { "newAgentBranch", Convert.ToString(_LoginItem.New_Agent_Branch) },
                                    { "stepComplete", Convert.ToString(_LoginItem.customerRegStep) },
                                    { "payoutCountryId", Convert.ToString(_LoginItem.payout_countries) },
                                    { "customer_profileimage", Convert.ToString(_LoginItem.customer_profileimage) },
                                    { "token", token.Replace(@"\", string.Empty) },
                                    { "passcodeflag", Convert.ToString(_LoginItem.passcode_flag) },
                                    { "encrypt_username", CompanyInfo.Encrypt(Convert.ToString(_LoginItem.Email).Trim(), true) },
                                    { "encrypt_password", CompanyInfo.Encrypt(Convert.ToString(_LoginItem.Password).Trim(), true) }
                                };
                        _ = Task.Run(() => CompanyInfo.InsertActivityLogDetailsasync("API Login Response : " + Convert.ToString(dataList), Convert.ToInt32(0), 0, Convert.ToInt32(0), 0, "UserLoginController", Convert.ToInt32(0), 1, "CheckLogin", HttpContext));

                        var successjsonData = new { response = true, responseCode = "00", data = dataList };
                        //var successjsonData = new { response = true, responseCode = "00", data = dataList };
                        return new JsonResult(successjsonData);
                    }
                    else if (response.ResponseCode == 1)
                    {
                        var successjsonData = new { response = false, responseCode = "02", data = response.ResponseMessage };
                        return new JsonResult(successjsonData);
                    }
                    else if (response.ResponseCode == 4)
                    {
                        var successjsonData = new { response = false, responseCode = "04", data = response.ResponseMessage };
                        return new JsonResult(successjsonData);
                    }
                    else if (response.ResponseCode == 3)
                    {
                        var successjsonData = new { response = false, responseCode = "03", data = response.ResponseMessage };
                        return new JsonResult(successjsonData);
                    }
                    else if (response.ResponseCode == 2)
                    {
                        var successjsonData = new { response = false, responseCode = "05", data = "Invalid Captcha." };
                        return new JsonResult(successjsonData);
                    }
                    else if (response.ResponseCode == 5)
                    {
                        var successjsonData = new { response = false, responseCode = "02", data = "Your account has been temporarily blocked for security reasons. You may try accessing your account tomorrow or contact your nearest branch for assistance" };
                        return new JsonResult(successjsonData);
                    }
                    else if (response.ResponseCode == 6)
                    {
                        var successjsonData = new { response = false, responseCode = "06", data = "We detected a login from a new device. Please enter the passcode to proceed." };
                        return new JsonResult(successjsonData);
                    }
                    else if (response.ResponseCode == 7)
                    {
                        var successjsonData = new { response = false, responseCode = "02", data = "Customer is blocked because of the previous invalid attempts." };
                        return new JsonResult(successjsonData);
                    }
                    else
                    {
                        if (response.ResponseMessage == null)
                        {
                            response.ResponseMessage = "Multiple times login failed.";
                        }

                        var successjsonData = new { response = false, responseCode = "02", data = response.ResponseMessage };
                        return new JsonResult(successjsonData);
                    }

                }
                else
                {
                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_TECHNICAL_ERROR);
                    response.ResponseMessage = "Username or Password is incorrect";
                    if (data.credentialFlag == "1")
                    {
                        response.ResponseMessage = "Username or Passcode is incorrect";
                    }
                    if (data.credentialFlag == "2")
                    {
                        response.ResponseMessage = "Unable to login.";
                    }

                    response.ResponseCode = 2;
                    Model.ActivityLogTracker _objActivityLog = new Model.ActivityLogTracker();
                    _objActivityLog.Activity = "API Login attempt failed. Incorrect Login Details. <br/>Details: User Name " + _LoginObj.UserName + " ";
                    _objActivityLog.FunctionName = "Check Login";
                    _objActivityLog.Transaction_ID = 0;
                    _objActivityLog.WhoAcessed = 1;
                    _objActivityLog.Branch_ID = _LoginObj.Branch_ID;
                    _objActivityLog.Client_ID = _LoginObj.Client_ID;

                    Service.srvActivityLogTracker srvActivityLog = new Service.srvActivityLogTracker();
                    int _i = srvActivityLog.Create(_objActivityLog, context);

                    var successjsonData = new { response = false, responseCode = "02", data = response.ResponseMessage };
                    return new JsonResult(successjsonData);
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
                objError.Error = "Api : Login --" + ex.ToString();
                objError.Date = DateTime.Now;
                objError.User_ID = 1;
                objError.Client_ID = obj.Client_ID;
                objError.Function_Name = "CheckLogin";
                Service.srvErrorLog srvError = new Service.srvErrorLog();
                srvError.Create(objError, context);

               
                var successjsonData = new { response = false, responseCode = "01", data = "Request failed" };
                return new JsonResult(successjsonData);
            }

           
            return new JsonResult(jsonData);
        }

        [HttpPost]
        [Route("getdecryptcred")]
        public IActionResult GetDecryptCredentials([FromBody] JsonElement objdata)
        {
            HttpContext context = HttpContext;
            Model.response.WebResponse response = null;
            string jsonData = System.Text.Json.JsonSerializer.Serialize(objdata);
            dynamic data = JObject.Parse(jsonData);
            CompanyInfo.InsertrequestLogTracker("GetDecryptCredentials full request body: " + JObject.Parse(jsonData), 0, 0, 0, 0, "GetDecryptCredentials", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
            try
            {
                userLogin obj = new userLogin();

                try { obj.userEmail = data.userName; } catch { data.userName = ""; }
                try { obj.userPassword = data.password; } catch { data.password = ""; }

                string decrypt_userName = CompanyInfo.Decrypt(Convert.ToString(obj.userEmail), true);
                string decrypt_password = CompanyInfo.Decrypt(Convert.ToString(obj.userPassword), true);

                var successjsonData = new
                {
                    response = true,
                    responseCode = "00",
                    data = new
                    {

                        DecryptedUsername = decrypt_userName,

                        DecryptedPassword = decrypt_password
                    }
                };

                return new JsonResult(successjsonData);
            }
            catch (Exception ex)
            {
                Model.ActivityLogTracker _objActivityLog = new Model.ActivityLogTracker();
                _objActivityLog.Activity = "Exception Get Decrypt Credentials Error: " + ex.ToString() + " ";
                _objActivityLog.FunctionName = "GetDecryptCredentials";
                _objActivityLog.Branch_ID = 0;
                _objActivityLog.Client_ID = 0;

                Service.srvActivityLogTracker srvActivityLog = new Service.srvActivityLogTracker();
                int _i = srvActivityLog.Create(_objActivityLog, HttpContext);

                CompanyInfo.InsertErrorLogTracker("Exception GetDecryptCredentials Error: " + ex.ToString(), 0, 0, 0, 0, "GetDecryptCredentials", Convert.ToInt32(data.Branch_ID), Convert.ToInt32(data.Client_ID), "", HttpContext);

                var declinedjsonData = new { response = false, responseCode = "01", data = "Invalid Request" };
                return new JsonResult(declinedjsonData);
            }
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        private void SaveLoginDetails(Model.Login obj, HttpContext context)
        {
            try
            {
                string strUserAgent = Request.Headers[HeaderNames.UserAgent].ToString().ToLower();
                

                //string strUserAgent = HttpContext.Current.Request.UserAgent.ToString().ToLower();
                if (strUserAgent != null)
                {
                   /* if (HttpContext.Current.Request.Browser.IsMobileDevice == true || strUserAgent.Contains("iphone") ||
                        strUserAgent.Contains("blackberry") || strUserAgent.Contains("mobile") ||
                        strUserAgent.Contains("windows ce") || strUserAgent.Contains("opera mini") ||
                        strUserAgent.Contains("palm"))
                    {                        
                    }*/
                }


                /*string IPAddress1 = Microsoft.AspNetCore.Http.HttpContext.Request.UserHostAddress;
                string host = HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
                string serverName = HttpContext.Current.Request.ServerVariables["server_name"];
                string serverport = HttpContext.Current.Request.ServerVariables["SERVER_PORT"];
                string localAddr = HttpContext.Current.Request.ServerVariables["LOCAL_ADDR"];
                string user = HttpContext.Current.Request.ServerVariables["LOGON_USER"];
                string Remoteuser = HttpContext.Current.Request.ServerVariables["REMOTE_USER"];
                string Remotehost = HttpContext.Current.Request.ServerVariables["REMOTE_HOST"];
                string serverSft = HttpContext.Current.Request.ServerVariables["SERVER_SOFTWARE"];
                string browserDetails = string.Empty;*/

                string IPAddress1 = HttpContext.Connection.RemoteIpAddress.ToString();

                string host = HttpContext.Connection.RemoteIpAddress.ToString(); //"REMOTE_HOST"
                string serverName = HttpContext.Request.Host.Host;  //"server_name"
                int serverport = HttpContext.Request.Host.Port ?? 80;   //"SERVER_PORT"
                string localAddr = HttpContext.Connection.LocalIpAddress.ToString(); //"LOCAL_ADDR"
                string user = HttpContext.User.Identity.Name; // "LOGON_USER"
                string Remoteuser = HttpContext.User.Identity.Name;//"REMOTE_USER"
                string Remotehost = host;
                string serverSft = HttpContext.Request.Headers["Server"]; //"SERVER_SOFTWARE"

                string browserDetails = string.Empty;

                string userAgentg = HttpContext.Request.Headers["User-Agent"];
                browserDetails = userAgentg +", "
                + " Device details =" + strUserAgent;

                obj.Location = "API Browser information : " + browserDetails + ",Logon user : " + user + ", Server name : " + serverName + " Server port :" + serverport + " , Remote User :" + Remoteuser + ", server software : " + serverSft + "";
                obj.IP_Address = IPAddress1;

                

                obj.Login_DateTime = CompanyInfo.gettime(obj.Client_ID, context);
                obj.Login_Time = Convert.ToDateTime(obj.Login_DateTime).ToString("HH:mm:ss");
                Service.srvLogin srvLogin = new Service.srvLogin();
                srvLogin.InsertLoginDetails(obj, context);
            }
            catch (Exception ex) { }
            }


            // PUT api/<UserLoginController>/5
            [HttpPut("{id}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<UserLoginController>/5
        [HttpDelete("{id}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public void Delete(int id)
        {
        }




    }
}
