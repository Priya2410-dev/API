using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using Calyx_Solutions.Model;
using Newtonsoft.Json;
using System.Text.Json;
using Auth0.ManagementApi.Models;
using System.Data;
using RestSharp;
using System.Net;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Calyx_Solutions.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        IConfiguration configuration;
        private readonly ITokenBlacklistService _blacklistService;
        public AuthController(IConfiguration configuration, ITokenBlacklistService blacklistService)
        {
            this.configuration = configuration;
            _blacklistService = blacklistService;
        }

        [AllowAnonymous]
        [HttpPost]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult Auth([FromBody] UserLogin user)
        {
            IActionResult response = Unauthorized();
            CompanyInfo.InsertActivityLogDetailsasync("Token step 1: "       , Convert.ToInt32(0), 0, Convert.ToInt32(0), 0, "UserLoginController", Convert.ToInt32(0), 1, "CheckLogin", HttpContext);
            CompanyInfo.InsertrequestLogTracker("Token step 1: " + Convert.ToString("") + "  " , 0, 0, 0, 0, "UserLoginController", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);

            if (user != null)
            {

                if(!user.userEmail.Equals("") && !user.userPassword.Equals(""))
                {
                    var issuer =  configuration["Jwt:Issuer"];
                    var audience = configuration["Jwt:Audience"];
                    var key = Encoding.UTF8.GetBytes(configuration["Jwt:SecurityKey"]);
                    var signingCredentials = new SigningCredentials(
                                            new SymmetricSecurityKey(key),
                                            SecurityAlgorithms.HmacSha512Signature
                                        );
                   var subject = new ClaimsIdentity(new[]
                        {
                        new Claim(JwtRegisteredClaimNames.Sub, user.userEmail),
                        new Claim(JwtRegisteredClaimNames.Email, user.userEmail),
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
                    var token = tokenHandler.CreateToken(tokenDescriptor);
                    var jwtToken = tokenHandler.WriteToken(token);
                    CompanyInfo.InsertrequestLogTracker("Token tokenDescriptor: " + Convert.ToString(tokenDescriptor), 0, 0, 0, 0, "UserLoginController", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);

                    var jsonData = new { success = 01,token = jwtToken };
                   // return new JsonResult(jsonData);
                    return Ok(jwtToken);
                }
                else
                {
                    CompanyInfo.InsertActivityLogDetailsasync("Token step 3: ", Convert.ToInt32(0), 0, Convert.ToInt32(0), 0, "UserLoginController", Convert.ToInt32(0), 1, "CheckLogin", HttpContext);
                    CompanyInfo.InsertrequestLogTracker("Token step 3: " + Convert.ToString("") + "  ", 0, 0, 0, 0, "UserLoginController", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
                    var jsonData = new { success = 00, token = "Unauthorized." };
                    return new JsonResult(jsonData);
                    //return Unauthorized(); ;
                }

            }
            return response;
        }

        [HttpGet]
        [Route("generatecaptcha")]
        public IActionResult GenerateCaptcha()
        {
            try
            {
                Service.srvCaptcha srv = new Service.srvCaptcha();
                var captcha = srv.GenerateCaptcha();

                return Ok(new
                {
                    response = true,
                    responseCode = "00",
                    data = new
                    {
                        Captcha = captcha
                    }
                });
            }
            catch (Exception ex)
            {
                Model.ActivityLogTracker _objActivityLog = new Model.ActivityLogTracker();
                _objActivityLog.Activity = " Exception Generate Captcha Error: " + ex.ToString() + " ";
                _objActivityLog.FunctionName = "generatecaptcha";
                _objActivityLog.Transaction_ID = 0;
                _objActivityLog.WhoAcessed = 1;
                _objActivityLog.Branch_ID = 0;
                _objActivityLog.Client_ID = 0;

                Service.srvActivityLogTracker srvActivityLog = new Service.srvActivityLogTracker();
                int _i = srvActivityLog.Create(_objActivityLog, HttpContext);
                return BadRequest( new { response = false, responseCode = "01", data = "Invalid Request" });
            }
        }


        [HttpPost]
        [Route("verifycaptcha")]
        public IActionResult VerifyCaptcha([FromBody] VerifyCaptchaRequest request)
        {
            try
            {
                Service.srvCaptcha srv = new Service.srvCaptcha();
                bool isValid = srv.VerifyCaptcha(request.Captcha);

                if (!isValid)
                {
                    return Unauthorized( new
                    {
                        response = true,
                        responseCode = "00",
                        data = "CAPTCHA verification failed."
                    });
                }
                return Ok(new
                {
                    response = true,
                    responseCode = "00",
                    data = "CAPTCHA verification succeeded."
                });
            }
            catch (Exception ex)
            {
                Model.ActivityLogTracker _objActivityLog = new Model.ActivityLogTracker();
                _objActivityLog.Activity = " Exception Verify Captcha Error: " + ex.ToString() + " ";
                _objActivityLog.FunctionName = "verifycaptcha";
                _objActivityLog.Transaction_ID = 0;
                _objActivityLog.WhoAcessed = 1;
                _objActivityLog.Branch_ID = 0;
                _objActivityLog.Client_ID = 0;

                Service.srvActivityLogTracker srvActivityLog = new Service.srvActivityLogTracker();
                int _i = srvActivityLog.Create(_objActivityLog, HttpContext);
                return BadRequest(new { response = false, responseCode = "01", data = "Invalid Request" });
            }
        }

        //Generate Captcha with username
        [HttpPost]
        [Route("generatecaptchawithusername")]
        public IActionResult GenerateCaptcha_withUserName([FromBody] GenerateCaptchaRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.UserName))
                {
                    return BadRequest(new { message = "UserName is required." });
                }

                Service.srvCaptcha srv = new Service.srvCaptcha();
                var (captcha, userName) = srv.GenerateCaptcha_withUserName(request.UserName);

                return Ok(new { captcha, userName });
            }
            catch (Exception ex)
            {
                Model.ActivityLogTracker _objActivityLog = new Model.ActivityLogTracker();
                _objActivityLog.Activity = " Exception Generate Captcha with UserName Error: " + ex.ToString() + " ";
                _objActivityLog.FunctionName = "generatecaptchawithusername";
                _objActivityLog.Transaction_ID = 0;
                _objActivityLog.WhoAcessed = 1;
                _objActivityLog.Branch_ID = 0;
                _objActivityLog.Client_ID = 0;

                Service.srvActivityLogTracker srvActivityLog = new Service.srvActivityLogTracker();
                int _i = srvActivityLog.Create(_objActivityLog, HttpContext);
                return BadRequest(new { response = false, responseCode = "01", data = "Invalid Request" });
            }
        }

        [Authorize]
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            var jti = User.FindFirstValue(JwtRegisteredClaimNames.Jti);
            var expUnix = long.Parse(User.FindFirstValue(JwtRegisteredClaimNames.Exp));
            var expiry = DateTimeOffset.FromUnixTimeSeconds(expUnix).UtcDateTime;

            _blacklistService.BlacklistToken(jti, expiry);
            return Ok("Token has been force-expired.");
        }


        public static bool checkAuth(ClaimsIdentity claimsIdentity, JsonElement jsonElement,  string authHeader)//sanket
        {
            string webstring = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("ConnectionStrings")["APIName"];
            webstring = webstring.ToLower();
            if (webstring.IndexOf("csremitmtapp", 0, StringComparison.OrdinalIgnoreCase) != -1)
            {
                // return true;
            }
            else
            {
                return true;
            }

            bool status = false;

            // Get the specific claim for "UserName"
            var userName = claimsIdentity?.Claims.FirstOrDefault(c => c.Type == "UserName")?.Value;
            var Token_Customer_ID = claimsIdentity?.Claims.FirstOrDefault(c => c.Type == "Customer_ID")?.Value;

            HttpContext httpContext= null;
            

            string custIDValue = "";
            int foundcustID = 0;
            if (jsonElement.ValueKind != JsonValueKind.Null && jsonElement.ValueKind != JsonValueKind.Undefined && jsonElement.ValueKind == JsonValueKind.Object)
            {
                foreach (var property in jsonElement.EnumerateObject())
                {
                    var value = Convert.ToString(property.Value);
                    var keyName = Convert.ToString(property.Name);
                    if (value != null && keyName == "customerID")
                    {
                        foundcustID = 1;
                        custIDValue = value;
                        if (Token_Customer_ID == value)
                        {
                            status = true;
                        }
                    }
                    else if (value != null && keyName == "Customer_ID")
                    {
                        foundcustID = 1;
                        custIDValue = value;
                        if (Token_Customer_ID == value)
                        {
                            status = true;
                        }
                    }
                    else if (value != null && keyName == "Customer_Id")
                    {
                        foundcustID = 1;
                        custIDValue = value;
                        if (Token_Customer_ID == value)
                        {
                            status = true;
                        }
                    }

                    else if (value != null && keyName == "customerid")
                    {
                        foundcustID = 1;
                        custIDValue = value;
                        if (Token_Customer_ID == value)
                        {
                            status = true;
                        }
                    }

                }
            }

 
            if (foundcustID == 0) { status = true; }
            if (Token_Customer_ID == "" || Token_Customer_ID == null)
            {
                status = true;
            }
 
            if(authHeader == null || authHeader == "")
            {
                status = true;
            }

 
            try
            {
                if (custIDValue != "" && authHeader != "")
                {
                    int custID = Convert.ToInt32(CompanyInfo.Decrypt(custIDValue, true));
                    if (custID > 0)
                    {
                        string custIDVa = Convert.ToString(custID);
                        MySqlConnector.MySqlCommand cmd = new MySqlConnector.MySqlCommand("Get_CompanyInfo");
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("_Client_ID", 1);
                        cmd.Parameters.AddWithValue("_Customer_ID", custIDVa);
                        cmd.Parameters.AddWithValue("_SecurityKey", CompanyInfo.SecurityKey());
                        DataTable dt = (DataTable)db_connection.ExecuteQueryDataTableProcedure(cmd);
                        if ( (dt.Rows.Count > 0 && Convert.ToString(dt.Rows[0]["Security_Flag"]) == "0") || !status)
                        {
                            string apibaseurl = Convert.ToString(dt.Rows[0]["apibaseurl"]);
                            var client = new RestClient(apibaseurl + "api/auth/logout");
                            client.Timeout = -1;
                            var request = new RestRequest(Method.POST);

                            request.AddHeader("Content-Type", "application/json");
                            request.AddHeader("Authorization", authHeader);
                            var body = @"";
                            request.AddParameter("application/json", body, ParameterType.RequestBody);
                            ServicePointManager.Expect100Continue = true;
                            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
                                   | SecurityProtocolType.Tls11
                                   | SecurityProtocolType.Tls12;
                            IRestResponse responseg = client.Execute(request);
                            status = false;
                        }
                    }
                }
            }
            catch (Exception ex) {
                
            }

            if (!status)
            {
                CompanyInfo.InsertrequestLogTracker(" false  checkAuth value: " + authHeader + "  ", 0, 0, 0, 0, "checkAuth", Convert.ToInt32(0), Convert.ToInt32(0), "", httpContext);
            }

            return status;
        }


        [ApiExplorerSettings(IgnoreApi = true)]
        public   bool expiretokenforcelly()
        {
            var jti = User.FindFirstValue(JwtRegisteredClaimNames.Jti);
            var expUnix = long.Parse(User.FindFirstValue(JwtRegisteredClaimNames.Exp));
            var expiry = DateTimeOffset.FromUnixTimeSeconds(expUnix).UtcDateTime;

            _blacklistService.BlacklistToken(jti, expiry);            
            return true;
        }

            //Verify Captcha with username
            [HttpPost]
        [Route("verifycaptchawithusername")]
        public IActionResult VerifyCaptcha_withUserName([FromBody] VerifyCaptchaRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.UserName) || string.IsNullOrEmpty(request.Captcha))
                {
                    return BadRequest(new { message = "UserName and Captcha are required." });
                }

                Service.srvCaptcha srv = new Service.srvCaptcha();
                bool isValid = srv.VerifyCaptcha_withUserName(request.UserName, request.Captcha);

                if (!isValid)
                {
                    return Unauthorized(new {
                        response = true,
                        responseCode = "00",
                        data = "CAPTCHA verification failed."
                    });
                }

                return Ok(new {
                    response = true,
                    responseCode = "00",
                    data = "CAPTCHA verification succeeded."
                });
            }
            catch (Exception ex)
            {
                Model.ActivityLogTracker _objActivityLog = new Model.ActivityLogTracker();
                _objActivityLog.Activity = " Exception Verify Captcha with UserName Error: " + ex.ToString() + " ";
                _objActivityLog.FunctionName = "verifycaptchawithusername";
                _objActivityLog.Transaction_ID = 0;
                _objActivityLog.WhoAcessed = 1;
                _objActivityLog.Branch_ID = 0;
                _objActivityLog.Client_ID = 0;

                Service.srvActivityLogTracker srvActivityLog = new Service.srvActivityLogTracker();
                int _i = srvActivityLog.Create(_objActivityLog, HttpContext);
                return BadRequest(new { response = false, responseCode = "01", data = "Invalid Request" });
            }
        }

        public class GenerateCaptchaRequest
        {
            public string UserName { get; set; }
        }

        public class VerifyCaptchaRequest
        {
            public string? UserName { get; set; }
            public string Captcha { get; set; }
        }

    }
}
