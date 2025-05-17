using Calyx_Solutions.Model;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Data;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Authorization;
using Calyx_Solutions.Service;
using System.Net;
using System.Security.Claims;
using System.Text.Json;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Calyx_Solutions.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanydetailsController : ControllerBase
    {
        [HttpPost]         
        [Route("getcompanydetails")]
         public IActionResult Companydetails([FromBody] JsonObject obj)
           {
               Model.response.WebResponse response = null;
               string json = System.Text.Json.JsonSerializer.Serialize(obj);
               dynamic data = JObject.Parse(json);
               CompanyInfo.InsertrequestLogTracker("getcompanydetails full request body: " + JObject.Parse(json), 0, 0, 0, 0, "Companydetails", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
               var validateJsonData = (dynamic)null;
               Customer Obj = JsonConvert.DeserializeObject<Customer>(json);
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
                       validateJsonData = new { response = false, responseCode = "02", data = "Invalid client ID." };
                       return new JsonResult(validateJsonData);
                   }
                   if (!int.TryParse(Convert.ToString(data.branchID), out number) || data.branchID == "" || data.branchID == null)
                   {
                       validateJsonData = new { response = false, responseCode = "02", data = "Invalid branch ID ." };
                       return new JsonResult(validateJsonData);
                   }

                   Obj.Client_ID = data.clientID;
                   Obj.Branch_ID = data.branchID;
                   string Base_currency = data.baseCurrency;
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
                   List<Model.CompanyDetails> _lst = new List<Model.CompanyDetails>();
                   string Client_ID1_regex = CompanyInfo.testInjection(Convert.ToString(Obj.Client_ID));
                   String Client_ID_regex = validation.validate(Convert.ToString(Obj.Client_ID), 0, 1, 1, 1, 1, 1, 1, 1, 1);
                   string base_cur_code = "true", base_cur_code1 = "1", base_cur_code_len = "1";
                   if (Base_currency != "" && Base_currency != null)
                   {
                       base_cur_code = validation.validate_curcode(Convert.ToString(Base_currency));
                       base_cur_code1 = CompanyInfo.testInjection(Convert.ToString(Base_currency));
                       if (Base_currency.Length != 3)
                       {
                           base_cur_code_len = "0";
                       }
                   }
                   Model.CompanyDetails _obj = new Model.CompanyDetails();
                   if (Client_ID1_regex == "1" && Client_ID_regex != "false" && base_cur_code1 == "1" && base_cur_code != "false" && base_cur_code_len == "1")
                   {
                       DataTable dt = (DataTable)CompanyInfo.GetBaseCurrencywisebankdetails(Obj.Client_ID, Base_currency,0,0);

                       if (dt != null && dt.Rows.Count > 0)
                       {
                           List<Dictionary<string, object>> companies = new List<Dictionary<string, object>>();

                           foreach (DataRow dr in dt.Rows)
                           {
                               Dictionary<string, object> company = new Dictionary<string, object>();
                               company["companyName"] = dr["Company_Name1"];
                               company["companyAddress"] = dr["Company_Address"];
                               company["companyEmailAddress"] = dr["Company_Email"];
                               company["companyPhoneNumber"] = dr["CompanyPhone_One"];
                               company["companyWhatsAppNumber"] = dr["Whats_app_no"];
                               company["companyHouseNumber"] = dr["CompanyHouse_Number"];
                               company["companyWebsite"] = dr["Company_website"];

                            string webstring = "";
                            try
                            {
                                webstring = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("ConnectionStrings")["APIName"];
                                webstring = webstring.ToLower();
                            }catch(Exception ex) { }

                            if (webstring.IndexOf("testmtsapp", 0, StringComparison.OrdinalIgnoreCase) != -1)
                            {
                                string companyWebsiteURL = Convert.ToString(dr["Company_website"]).Trim();
                                char lastChar = companyWebsiteURL[companyWebsiteURL.Length - 1]; 

                                if (lastChar != '/')
                                {
                                    companyWebsiteURL = Convert.ToString(dr["Company_website"]).Trim() + "/";  
                                }
                                bool exists = companyWebsiteURL.Contains("test-mts");

                                if (exists)
                                {
                                    company["imagePath"] = companyWebsiteURL + "assets/";
                                }
                                else {
                                    company["imagePath"] = companyWebsiteURL + "test-mts" + "/assets/";
                                }                                
                            }
                            else if (webstring.IndexOf("csremitmtapp", 0, StringComparison.OrdinalIgnoreCase) != -1)
                            {
                                string companyWebsiteURL = Convert.ToString(dr["Company_website"]).Trim();
                                char lastChar = companyWebsiteURL[companyWebsiteURL.Length - 1];

                                if (lastChar != '/')
                                {
                                    companyWebsiteURL = Convert.ToString(dr["Company_website"]).Trim() + "/"; 
                                }
                                bool exists = companyWebsiteURL.Contains("csremit-admin");
                                if (exists)
                                {
                                    company["imagePath"] = companyWebsiteURL + "assets/";
                                }
                                else
                                {
                                    company["imagePath"] = companyWebsiteURL + "/assets/";
                                }

                                
                            }
                            else
                            {
                                if (webstring.IndexOf("aphaomegaapp", 0, StringComparison.OrdinalIgnoreCase) != -1)
                                {
                                    company["imagePath"] = dr["Company_URL_Customer"] + "assets/";
                                }
                                else if (webstring.IndexOf("ifepayapp", 0, StringComparison.OrdinalIgnoreCase) != -1)
                                {
                                    company["imagePath"] = dr["Company_URL_Customer"] + "assets/";
                                }
                                else if (webstring.IndexOf("myremitapp", 0, StringComparison.OrdinalIgnoreCase) != -1)
                                {
                                    company["imagePath"] = dr["Company_URL_Customer"] + "assets/";
                                }
                                else if (webstring.IndexOf("balazooapp", 0, StringComparison.OrdinalIgnoreCase) != -1)
                                {
                                    company["imagePath"] = dr["Company_URL_Customer"] + "assets/";
                                }
                                else if (webstring.IndexOf("supertransferapp", 0, StringComparison.OrdinalIgnoreCase) != -1)
                                {
                                    company["imagePath"] = dr["Company_URL_Customer"] + "assets/";
                                }
                                else if (webstring.IndexOf("riyoremitapp", 0, StringComparison.OrdinalIgnoreCase) != -1)
                                {
                                    company["imagePath"] = dr["Company_URL_Customer"] + "assets/";
                                }
                                else if (webstring.IndexOf("truslypayapp", 0, StringComparison.OrdinalIgnoreCase) != -1)
                                {
                                    company["imagePath"] = dr["Company_URL_Customer"] + "assets/";
                                }
                                else if (webstring.IndexOf("tassapayliveapi", 0, StringComparison.OrdinalIgnoreCase) != -1)
                                {
                                    company["imagePath"] = dr["Company_URL_Customer"] + "assets/";
                                }
                                else
                                {
                                    company["imagePath"] = dr["Company_website"] + "/assets/";
                                }
                            }

                            company["Brand_Color"] = dr["Brand_Color"];
                            company["Second_Color"] = dr["Second_Color"];

                            company["Brand_Color_flutter"] = Convert.ToString(dr["Brand_Color_flutter"]);
                            company["Second_Color_flutter"] = Convert.ToString(dr["Second_Color_flutter"]);                            

                            company["companyRefNo"] = dr["CompanyRef_No"];
                               company["companyRegNo"] = dr["CompanyReg_No"];
                               company["companyPostode"] = dr["Company_Postode"];
                               company["companyPhoneTwo"] = dr["CompanyPhone_two"];
                               company["Company_URL_Admin"] = dr["Company_URL_Admin"];
                               company["companyURLCustomer"] = dr["Company_URL_Customer"];
                               company["adminMail"] = dr["admin_mail"];
                               company["companyMobile"] = dr["Company_mobile"];
                               company["baseCurrencySign"] = dr["BaseCurrency_Sign"];
                               company["baseCurrencyCountry"] = dr["BaseCurrency_Country"];
                               company["baseCountryID"] = dr["BaseCountry_ID"];
                               company["baseCurrencyCode"] = dr["BaseCurrency_Code"];
                               company["baseCurrencyTimezone"] = dr["BaseCurrency_Timezone"];
                               company["companyBankName"] = dr["BankName"];
                               company["companyAccountHolderName"] = dr["AccountHolderName"];
                               company["companyAccountNumber"] = dr["AccountNumber"];
                               company["bankBranch"] = dr["Branch"];
                               company["bankSortCode"] = dr["Sort_ID"];
                               company["bankIBAN"] = dr["IBAN"];
                               company["expiryMonth"] = dr["ExpiryMonths"];
                            try { company["apiurl"] = dr["apibaseurl"]; }
                            catch (Exception ex)
                            { company["apiurl"] = ""; }
                                try { company["image"] = dr["Image"]; } catch (Exception ex) { company["image"] = ""; }

                            companies.Add(company);
                           }
                           validateJsonData = new { response = true, responseCode = "00", data = companies };
                           return new JsonResult(validateJsonData);
                       }
                       validateJsonData = new { response = false, responseCode = "02", data = "No record found." };
                       return new JsonResult(validateJsonData);
                   }
                   else
                   {
                       string msg1 = "Validation Error";
                       string msg = "Validation Error Client_ID_regex- +" + Client_ID_regex + " base_cur_code - +" + base_cur_code;
                       int stattus = (int)CompanyInfo.InsertActivityLogDetailsSecurity(msg, Convert.ToInt32(Obj.User_ID), 0, Convert.ToInt32(Obj.User_ID), 1, "srvCompanyDetails", Convert.ToInt32(Obj.Branch_ID), Convert.ToInt32(Obj.Client_ID), "getInfo", 0,HttpContext);
                       _obj.ResponseCode = 20;
                       _obj.ResponseMessage = msg1;
                       _lst.Add(_obj);
                       int status = (int)CompanyInfo.InsertActivityLogDetailsSecurity(msg, Convert.ToInt32(Obj.User_ID), 0, Convert.ToInt32(Obj.User_ID), 1, "srvcustomer", Convert.ToInt32(Obj.Branch_ID), Convert.ToInt32(Obj.Client_ID), "SaveCustomerComplaint", 0, HttpContext);
                       validateJsonData = new { response = false, responseCode = "02", data = msg };
                       return new JsonResult(validateJsonData);
                   }

               }
               catch (Exception ex)
               {
                   validateJsonData = new { response = false, responseCode = "01", data = "Invalid Request" };
                   CompanyInfo.InsertErrorLogTracker(ex.ToString(), 0, 0, 0, 0, "Companydetails", Convert.ToInt32(Obj.Branch_ID), Convert.ToInt32(Obj.Client_ID), "", HttpContext);
                   return new JsonResult(validateJsonData);
               }
           }

        [HttpPost]
        [Authorize]
        [Route("checkvalid")]
        public IActionResult check_valid([FromBody] JsonObject obj)
        {
            Model.response.WebResponse response = null;
            string json = System.Text.Json.JsonSerializer.Serialize(obj);
            dynamic data = JObject.Parse(json);
            var validateJsonData = (dynamic)null;
            HttpContext context = HttpContext;
            Login Obj = JsonConvert.DeserializeObject<Login>(json);
            _ = Task.Run(() => CompanyInfo.InsertrequestLogTracker("checkvalid full request body: " + JObject.Parse(json), 0, 0, 0, 0, "check_valid", Convert.ToInt32(0), Convert.ToInt32(0), "", context));
            try
            {

                if (!SqlInjectionProtector.ReadJsonObjectValues(obj) || !SqlInjectionProtector.ReadJsonObjectValuesScript(obj))
                {
                    var errorResponse = new { response = false, responseCode = "02", data = "Invalid input detected." };
                    return new JsonResult(errorResponse) { StatusCode = 400 };
                }

                JsonElement jsonElement = System.Text.Json.JsonSerializer.SerializeToElement(obj);//sanket // Converted JsonObject to JsonElement
                var claimsIdentity = User.Identity as ClaimsIdentity;//sanket
                var authHeader = "";
                try { authHeader = HttpContext.Request.Headers["Authorization"].ToString(); } catch (Exception egx) { authHeader = ""; }
                
                bool auth = AuthController.checkAuth(claimsIdentity, jsonElement, Convert.ToString(authHeader) );
                if (!auth)
                {
                    var errorResponse = new { response = false, responseCode = "403", data = "Access Forbidden" };
                    return new JsonResult(errorResponse) { StatusCode = (int)HttpStatusCode.Forbidden };
                }

                Obj.Customer_ID = CompanyInfo.Decrypt1(Obj.Customer_ID);
                // Call get sp which sets Security_Flag
                MySqlConnector.MySqlCommand cmd = new MySqlConnector.MySqlCommand("Get_CompanyInfo");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("_Client_ID", Obj.Client_ID);
                cmd.Parameters.AddWithValue("_Customer_ID", Obj.Customer_ID);
                cmd.Parameters.AddWithValue("_SecurityKey", CompanyInfo.SecurityKey());
                DataTable dt = (DataTable)db_connection.ExecuteQueryDataTableProcedure(cmd);
                if (dt.Rows.Count > 0 && Convert.ToString(dt.Rows[0]["Security_Flag"]) == "0")
                {
                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_SUCCESS);
                    response.data = "invalid";
                    response.ObjData = "";
                    response.ResponseCode = 0;

                    Model.ActivityLogTracker _objActivityLog = new Model.ActivityLogTracker();
                    _objActivityLog.Activity = "Automictic logout  App Side.<br/>Details: " + context.Session.GetString("Security_Flag") + " Security_Flag : " + Convert.ToBoolean(context.Session.GetString("Security_Flag")) + " , Session_Cust_Id : " + context.Session.GetString("Customer_ID") + " , scrutiny : " + context.Session.GetString("scrutiny");
                    _objActivityLog.FunctionName = "Api : CompanydetailsController.cs : Function Nm : check_valid();";
                    _objActivityLog.Transaction_ID = 0;
                    _objActivityLog.WhoAcessed = 1;
                    _objActivityLog.Branch_ID = 1;
                    _objActivityLog.Client_ID = 1;

                    Service.srvActivityLogTracker srvActivityLog = new Service.srvActivityLogTracker();
                    int _i = srvActivityLog.Create(_objActivityLog, HttpContext);
                    validateJsonData = new { response = true, responseCode = "00", data = "Invalid" };
                }
                else
                {
                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_TECHNICAL_ERROR);
                    response.ResponseMessage = "valid";
                    response.ResponseCode = 6;
                    validateJsonData = new { response = true, responseCode = "00", data = "Valid" };
                }
            }
            catch (Exception ex)
            {

                CompanyInfo.InsertErrorLogTracker("checkvalid error: "+ex.ToString(), 0, 0, 0, 0, "checkvalid", Convert.ToInt32(0), Convert.ToInt32(0), "", context);
                response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_TECHNICAL_ERROR);
                response.sErrorExceptionText = ex.ToString();
                response.ResponseMessage = "technical error";
                response.ResponseCode = 3;
                //Error Log Handled
                Model.ErrorLog objError = new Model.ErrorLog();
                objError.User = new Model.User();
                objError.Error = "Api : check_valid --" + ex.ToString();
                objError.Date = DateTime.Now;
                objError.User_ID = 1;
                objError.Client_ID = 1;
                objError.Function_Name = "check_valid";
                Service.srvErrorLog srvError = new Service.srvErrorLog();
                srvError.Create(objError, context);

                validateJsonData = new { response = false, responseCode = "01", data = "Invalid Request" };
            }
            return new JsonResult(validateJsonData);
        }

    }
}

