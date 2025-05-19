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
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Reflection;
using System.Security.Claims;
using System.Text.Json;
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
    public class ComplaintController : ControllerBase
    {

        [HttpPost]
        [Authorize]
        [Route("insertcomplaintform")]
        public IActionResult InsertComplaintForm([FromBody] JsonElement Obj)
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
            CompanyInfo.InsertrequestLogTracker("insertcomplaintform full request body: " + JObject.Parse(json), 0, 0, 0, 0, "InsertComplaintForm", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
            var validateJsonData = (dynamic)null;
            Customer obj = JsonConvert.DeserializeObject<Customer>(json);
            string status = "";
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
                if (data.customerReferanceNo == "" || data.customerReferanceNo == null)
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "Invalid customer Reference Number field." };
                    return new JsonResult(validateJsonData);
                }
                if (data.customerID == "" || data.customerID == null)
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "Invalid customerID field." };
                    return new JsonResult(validateJsonData);
                }
                if (data.customerName == "" || data.customerName == null)
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "Invalid customerName field." };
                    return new JsonResult(validateJsonData);
                }
                if (Convert.ToString(data.mobileNumber).Length < 9 || Convert.ToString(data.mobileNumber).Length > 15)
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "mobileNumber length should be between 9 and 15 characters." };
                    return new JsonResult(validateJsonData);
                }

                obj.Client_ID = Convert.ToInt32(data.clientID);
                obj.WireTransfer_ReferanceNo = data.customerReferanceNo;
                obj.Email = data.email;
                obj.MobileNumber = Convert.ToString(data.mobileNumber);
                obj.Comment = data.complaintDetails;
                obj.Id = data.customerID;
                obj.Name = data.customerName;
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
                string from_querystring = Convert.ToString(obj.WireTransfer_ReferanceNo);
                string split_string = "", ref_no = "";
                if (from_querystring != "" && from_querystring != null)
                {
                    split_string = from_querystring.Replace("reference=", "");
                    ref_no = CompanyInfo.Decrypt(split_string, Convert.ToBoolean(1));
                    if (ref_no == "")
                    {
                        validateJsonData = new { response = false, responseCode = "02", data = "Invalid customer Reference Number field." };
                        return new JsonResult(validateJsonData);
                    }
                }
                int Customer_ID = Convert.ToInt32(CompanyInfo.Decrypt(obj.Id, true));
                string Activity = string.Empty; string notification_icon = ""; string notification_message = "";
                string Username = obj.UserName; string error_msg = "";
                string MobileNumber_regex = validation.validate(obj.MobileNumber, 1, 1, 1, 0, 1, 1, 1, 1, 1);
                string Name_regex = validation.validate(obj.Name, 1, 1, 1, 1, 1, 1, 0, 1, 1);
                string Email_regex = validation.validate(obj.Email, 1, 1, 1, 1, 0, 1, 1, 1, 1);
                string WireTransferRefNumber_regex = validation.validate(obj.WireTransferRefNumber, 1, 1, 0, 1, 1, 1, 1, 1, 1);
                string Comment_regex = validation.validate(obj.Comment, 1, 1, 1, 1, 1, 1, 1, 1, 0);

                if (MobileNumber_regex == "false")
                {
                    validateJsonData = new { response = false, responseCode = "02", data = " Mobile number Should be numeric"};
                    return new JsonResult(validateJsonData );
                }
                if (Name_regex == "false")
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "Name should be characters with space" };
                    return new JsonResult(validateJsonData );
                }
                if (Comment_regex == "false")
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "Message should be alphanumeric with space and special characters like , , , . , ' , ( , ) , { , } , - , @ , &" };
                    return new JsonResult(validateJsonData );
                }
                if (WireTransferRefNumber_regex == "false")
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "WireTransferRefNumber should be Alphanumeric " };
                    return new JsonResult(validateJsonData);
                }
                if (Email_regex == "false")
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "Email Should be valid " };
                    return new JsonResult(validateJsonData);
                }


                if (Comment_regex != "false" && WireTransferRefNumber_regex != "false" && Email_regex != "false" && Name_regex != "false" && MobileNumber_regex != "false")
                {
                    string MobileNumber = CompanyInfo.testInjection(Convert.ToString(obj.MobileNumber));
                    string Name = CompanyInfo.testInjection(Convert.ToString(obj.Name));
                    string Comment = CompanyInfo.testInjection(Convert.ToString(obj.Comment));
                    string WireTransferRefNumber = CompanyInfo.testInjection(Convert.ToString(obj.WireTransferRefNumber));
                    string Email = CompanyInfo.testInjection(Convert.ToString(obj.Email));

                    if (Email == "1" && WireTransferRefNumber == "1" && MobileNumber == "1" && Name == "1" && Comment == "1")
                    {
                        try
                        {
                            MySqlConnector.MySqlCommand cmd = new MySqlConnector.MySqlCommand("SaveCustomerComplaint");

                            obj.RecordDate = Convert.ToDateTime(CompanyInfo.gettime(obj.Client_ID, Customer_ID.ToString(), 0, HttpContext));
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("_custRef", ref_no);
                            cmd.Parameters.AddWithValue("_message", obj.Comment);
                            cmd.Parameters.AddWithValue("_mobile_number", obj.MobileNumber);
                            cmd.Parameters.AddWithValue("_Name", obj.Name);
                            cmd.Parameters.AddWithValue("_Email", obj.Email);
                            cmd.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                            cmd.Parameters.AddWithValue("_Record_Insert_Date", obj.RecordDate);
                            cmd.Parameters.AddWithValue("_Added_from", "Web Branch");
                            cmd.Parameters.Add(new MySqlConnector.MySqlParameter("_Enquiry_ID", MySqlConnector.MySqlDbType.Int32));
                            cmd.Parameters["_Enquiry_ID"].Direction = ParameterDirection.Output;

                            int n1 = db_connection.ExecuteNonQueryProcedure(cmd);
                            int i = Convert.ToInt32(cmd.Parameters["_Enquiry_ID"].Value);

                            if (i > 0)
                            {

                                Activity = "<b>" + Username + "</b>" + "complaint added.  </br>";
                                int stattus = (int)CompanyInfo.InsertActivityLogDetails(Activity, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(obj.Customer_ID), "UpdatePhone", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "Save Complaint", HttpContext);

                                MySqlConnector.MySqlCommand cmd1 = new MySqlConnector.MySqlCommand("GetCustDetailsByID");
                                cmd1.CommandType = CommandType.StoredProcedure;
                                cmd1.Parameters.AddWithValue("cust_ID", Customer_ID);

                                DataTable dt_cust = db_connection.ExecuteQueryDataTableProcedure(cmd1);
                                string email = Convert.ToString(dt_cust.Rows[0]["Email_ID"]);
                                if(email != obj.Email)
                                {
                                    validateJsonData = new { response = false, responseCode = "02", data = "Email Should be valid " };
                                    return new JsonResult(validateJsonData);
                                }
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
                                    httpRequest = (System.Net.HttpWebRequest)WebRequest.Create(URL + "Email/complaint.htm");
                                    httpRequest.UserAgent = "Code Sample Web Client";
                                    HttpWebResponse webResponse = (HttpWebResponse)httpRequest.GetResponse();
                                    using (StreamReader reader = new StreamReader(webResponse.GetResponseStream()))
                                    {
                                        body = reader.ReadToEnd();
                                    }

                                    obj.Full_Name = Convert.ToString(obj.Name);
                                    body = body.Replace("[Cust_Reference]", Convert.ToString(ref_no));
                                    body = body.Replace("[Your_name]", Convert.ToString(obj.Name)); //obj.Full_Name);
                                    body = body.Replace("[Email]", Convert.ToString(obj.Email));
                                    body = body.Replace("[Phone]", Convert.ToString(obj.MobileNumber)); //obj.Full_Name);
                                    body = body.Replace("[Message]", Convert.ToString(obj.Comment));
                                    body = body.Replace("[company_website]", Convert.ToString(dtc.Rows[0]["company_website"]));
                                    //from = company_email_details.Rows[0]["Email_Convey_from"].ToString();
                                    //password = company_email_details.Rows[0]["Password"].ToString();
                                    body = body.Replace("[company_name]", dtc.Rows[0]["Company_Name"].ToString());
                                    body = body.Replace("[Company_reg_no]", dtc.Rows[0]["CompanyReg_No"].ToString());
                                    body = body.Replace("[Company_reg_office]", dtc.Rows[0]["Company_Address"].ToString());
                                    body = body.Replace("[contact_no]", dtc.Rows[0]["Company_mobile"].ToString());
                                    body = body.Replace("[email_id]", dtc.Rows[0]["Company_Email"].ToString());
                                    string send_mail = "mailto:" + dtc.Rows[0]["Company_Email"].ToString();
                                    body = body.Replace("[email_id1]", send_mail);
                                    body = body.Replace("[privacy_policy]", dtc.Rows[0]["privacy_policy"].ToString());
                                    body = body.Replace("[company_logo]", dtc.Rows[0]["Image"].ToString());
                                    body = body.Replace("[theme_color]", Convert.ToString(dtc.Rows[0]["Brand_Color"]));
                                    httpRequest1 = (HttpWebRequest)WebRequest.Create(URL + "Email/complaint.txt");
                                    httpRequest1.UserAgent = "Code Sample Web Client";
                                    HttpWebResponse webResponse1 = (HttpWebResponse)httpRequest1.GetResponse();
                                    using (StreamReader reader = new StreamReader(webResponse1.GetResponseStream()))
                                    {
                                        subject = reader.ReadLine();
                                    }
                                    string newsubject = company_name + " - " + subject + Convert.ToString("");
                                    string msg = (string)CompanyInfo.Send_Mail(dtc, obj.Email, body, newsubject, obj.Client_ID, 2, "", "", "", HttpContext);
                                    obj.Message = "success";
                                }
                                validateJsonData = new { response = true, responseCode = "00", data = "Complaint form filled successfully" };
                                return new JsonResult(validateJsonData);

                            }
                            validateJsonData = new { response = false, responseCode = "02", data = "Invalid Request" };
                            return new JsonResult(validateJsonData);

                        }
                        catch (Exception ex)
                        {
                            validateJsonData = new { response = false, responseCode = "01", data = "Invalid Request" };
                            CompanyInfo.InsertErrorLogTracker(ex.ToString(), 0, 0, 0, 0, "InsertComplaintForm", 0, Convert.ToInt32(obj.Client_ID), "", HttpContext);
                            return new JsonResult(validateJsonData);
                        }
                    }
                    else
                    {
                        string msg = "SQl Enjection detected";
                        int stattus = (int)CompanyInfo.InsertActivityLogDetailsSecurity(msg, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), 1, "srvcustomer", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "SaveCustomerComplaint", 0, HttpContext);
                        validateJsonData = new { response = false, responseCode = "02", data = msg };
                        return new JsonResult(validateJsonData);
                    }
                }
                else
                {
                    obj.Id = "0";
                    string msg = "Validation Failed <br/> " ;
                    obj.Message = "Validation Failed";
                    obj.Comment = error_msg;
                    int stattus = (int)CompanyInfo.InsertActivityLogDetailsSecurity(msg, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), 1, "srvcustomer", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "SaveCustomerComplaint", 0, HttpContext);
                    validateJsonData = new { response = false, responseCode = "02", data = msg };
                    return new JsonResult(validateJsonData);
                }
            }
            catch (Exception ex)
            {
                validateJsonData = new { response = false, responseCode = "01", data = "Invalid Request" };
                CompanyInfo.InsertErrorLogTracker(ex.ToString(), 0, 0, 0, 0, "InsertComplaintForm", 0, Convert.ToInt32(0), "", HttpContext);
                return new JsonResult(validateJsonData);
            }
        }
    }
}
