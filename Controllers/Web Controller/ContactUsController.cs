using Microsoft.Ajax.Utilities;
using Microsoft.AspNetCore.Mvc;
using System.Data.Common;
using System.Data;
using System.Drawing;
using System.Net.Mail;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using System.Net;
using MySqlConnector;
using Auth0.ManagementApi.Models;
 
using System.Web;
using System.Web.Configuration;
using Microsoft.AspNetCore.Http;
using Calyx_Solutions.Model;
using Antlr.Runtime;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using UAParser;
using System.Runtime.Serialization;
using Calyx_Solutions.Service;  // extract information about the user's browser, operating system, and device from the user-agent string.
//using Microsoft.AspNetCore.Http.Browser;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Calyx_Solutions.Web_Contollers
{
    [Route("webapi/[controller]")]
    [ApiController]    
    public class ContactUsController : ControllerBase
    {
        int branch_id = 0; int client_id = 0;
        MySqlConnection con = new MySqlConnection();


        //POST api/<ContactUsController>


       




        [HttpPost]
        [Route("savecontactdetails")]
        public IActionResult Save_ContactDetails([FromBody] Customer obj)
        {
            var errorResponse = new { response = false, responseCode = "02", data = "Invalid input detected." };
            return new JsonResult(errorResponse) { StatusCode = 400 };

            HttpContext context = HttpContext;
            var validateJsonData = (dynamic)null;

            string json = System.Text.Json.JsonSerializer.Serialize(obj);
            dynamic data = JObject.Parse(json);

            CompanyInfo.InsertrequestLogTracker("savecontactdetails full request body: " + JObject.Parse(json), 0, 0, 0, 0, "Save_ContactDetails", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
            Model.response.WebResponse response = null;

            Customer obj1 = new Customer();

            

            client_id = 1;
            //Secret key: 6LfTi0YaAAAAAK08sLLbRZWj4RmcJcCowvlqLI9n
            var secretKey = "6Lf-PmUpAAAAAOWLntC_CnnuW333VHTAArirxTkW";
            string reCaptcha = HttpContext.Request.Form["reCaptcha"];
            Boolean checkCaptcha = ReCaptchaPassed(secretKey, reCaptcha);
            if (!checkCaptcha)
            {
                context.Response.ContentType = "true";
                context.Response.WriteAsync("reCAPTCHA Error: " + "Please apply valid captcha");
                //return;
            }
            string enq_id = string.Empty;

            //string customer_name = Convert.ToString(context.Request.Form["Customer_Name"]);
            //string email_id = Convert.ToString(context.Request.Form["Email_ID"]);
            //string mobile_number = Convert.ToString(context.Request.Form["Mobile_Number"]);
            ////var subject1 = Convert.ToString(context.Request.Form["Subject"]);
            //string message = Convert.ToString(context.Request.Form["Message"]);
            
            
            string customer_name = data.name;
            string email_id = data.email;
            string mobile_number = data.phoneno;
            //var subject1 = Convert.ToString(context.Request.Form["Subject"]);
            string message = data.comment;
            Regex rgx_email = new Regex(@"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z");
            Regex rgx_name = new Regex(@"/^[a-z]+$/i");
            Regex rgx_mobile = new Regex(@"/^[0][1-9]\d{9,12}$|^[1-9]\d{9,15}$/");
            Regex rgx_msg = new Regex(@"/^[a-z0-9]+(\s[a-zA-Z0-9]+)$/i");
            Regex rgx_sub = new Regex(@"/^[a-z0-9]+(\s[a-zA-Z0-9]+)$/i");
            var resemail = rgx_email.IsMatch(email_id.ToLower());
            var resname = rgx_name.IsMatch(customer_name.ToLower());
            //var ressubject = rgx_sub.IsMatch(subject1.ToLower());
            var resmsg = rgx_msg.IsMatch(message.ToLower());
            var res_mobile_number = rgx_mobile.IsMatch(mobile_number.ToLower());
            TimeZoneInfo timeZoneInfo1 = TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time");
            DateTime dateTime = TimeZoneInfo.ConvertTime(DateTime.Now, timeZoneInfo1);

            string datetime = DateTime.Now.ToString("yyyy-MM-dd");
            var customer_name1 = CompanyInfo.testInjection(HttpContext.Request.Form["Customer_Name"]);
            var email_id1 = CompanyInfo.testInjection(HttpContext.Request.Form["Email_ID"]);
            var mobile_number1 = CompanyInfo.testInjection(HttpContext.Request.Form["Mobile_Number"]);


            var message1 = CompanyInfo.testInjection(context.Request.Form["Message"]);

            if (customer_name1 == "1" && email_id1 == "1" && mobile_number1 == "1" && message1 == "1")
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "SP_Save_contactDetails";
                cmd.Connection = con;
                // con.Open();
                //cmd.Parameters.AddWithValue("_Company_Name", company_name);
                cmd.Parameters.AddWithValue("_Name", customer_name);
                cmd.Parameters.AddWithValue("_Email_ID", email_id);
                cmd.Parameters.AddWithValue("_Message", message);
                cmd.Parameters.AddWithValue("_Mobile_Number", mobile_number);
                cmd.Parameters.AddWithValue("_Delete_Status", 0);
                cmd.Parameters.AddWithValue("_Record_Insert_DateTime", datetime);
                cmd.Parameters.Add(new MySqlParameter("_Enquiry_ID", MySqlDbType.Int32));
                cmd.Parameters["_Enquiry_ID"].Direction = ParameterDirection.Output;
                int n = 0;
                try
                {
                    n = db_connection.ExecuteNonQueryProcedure(cmd);//cmd.ExecuteNonQuery();
                }
                catch (Exception ex) { }

                con.Close();
                if (n > 0)
                {
                    enq_id = Convert.ToString(cmd.Parameters["_Enquiry_ID"].Value);
                    getEntryInLoginTabel(enq_id, customer_name,context);
                    context.Response.WriteAsJsonAsync("Customer Details added Successfully!");
                }

                string subject = string.Empty;
                string body = string.Empty;
                string body1 = string.Empty;


                System.Net.HttpWebRequest httpRequest;
                //string Newcontact = (System.Configuration.ConfigurationManager.AppSettings["contact"]);
                string Newcontact = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("ConnectionStrings")["Contact_us"];
                string Newcontacttxt = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("ConnectionStrings")["Contactus_txt"];
                // string Newcontacttxt = (System.Configuration.ConfigurationManager.AppSettings["Contacttxt"]);
                //httpRequest = (System.Net.HttpWebRequest)System.Net.WebRequest.Create("https://mathbeecomtrade.net/Email/contact.htm");
                httpRequest = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(Newcontact);

                httpRequest.UserAgent = "Code Sample Web Client";
                System.Net.HttpWebResponse webResponse = (System.Net.HttpWebResponse)httpRequest.GetResponse();
                using (StreamReader reader = new StreamReader(webResponse.GetResponseStream()))
                {
                    body = reader.ReadToEnd();
                    body1 = reader.ReadToEnd();
                }

                body = body.Replace("[Name]", Convert.ToString(customer_name));//body.Replace("[name]", (customer.Customer_Name).Trim());
                                                                               // body = body.Replace("[Company_name]", Convert.ToString(company_name));
                body = body.Replace("[Phone]", Convert.ToString(mobile_number));// (customer.Email_ID).Trim());
                body = body.Replace("[Email]", Convert.ToString(email_id));// (customer.Subject).Trim());
                body = body.Replace("[Message]", Convert.ToString(message));// (customer.Subject).Trim());
                //body = body.Replace("[Subject]", Convert.ToString(subject1));

                // System.Net.HttpWebRequest httpRequest1 = (System.Net.HttpWebRequest)System.Net.WebRequest.Create("https://mathbeecomtrade.net//Email/contact.txt");
                System.Net.HttpWebRequest httpRequest1 = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(Newcontacttxt);
                httpRequest1.UserAgent = "Code Sample Web Client";
                System.Net.HttpWebResponse webResponse1 = (System.Net.HttpWebResponse)httpRequest1.GetResponse();
                using (StreamReader reader = new StreamReader(webResponse1.GetResponseStream()))
                {
                    subject = reader.ReadLine();
                    body1 = reader.ReadToEnd();
                }

                body = body.Replace("[insert body]", body1);

                DataTable ds = new DataTable();
                Model.Customer t = new Model.Customer();
               // t = FactoryTransfer.getCustomer();
                string whereclause = "and PID=84";
                t.Client_ID = client_id;
                t.whereclause = whereclause + " ";
               // t.Is_Procedure = "QUERY";
               // t.Operation_Name = "Permission_Get";
               // ds = (DataTable)t.ReadFromDatabase();
                int Status = 0;
                if (ds.Rows.Count > 0)
                {
                    Status = Convert.ToInt32(ds.Rows[0]["Status"]);
                }


                using (MailMessage mailMessage = new MailMessage())
                {
                    Model.Customer c = new Customer();
                    string from = "", password = "";
                    System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
                    c.Client_ID = 1;
                    c.Branch_ID = 1;
                    c.SecurityKey = Convert.ToString(CompanyInfo.SecurityKey());
                    DataTable company_email_details = new DataTable(); //(DataTable)mtsmethods.GetBaseCurrencydetails(c);
                    MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("Get_Email_Configuration");
                    _cmd.CommandType = CommandType.StoredProcedure;
                    string _whereclause = " and Client_ID=" + client_id + " and   priority=1";

                    _cmd.Parameters.AddWithValue("_whereclause", _whereclause);
                    _cmd.Parameters.AddWithValue("_SecurityKey", CompanyInfo.SecurityKey());
                    DataTable dt_send_email = db_connection.ExecuteQueryDataTableProcedure(_cmd);
                    if (dt_send_email.Rows.Count > 0)
                    {
                        from = dt_send_email.Rows[0]["Email_Convey_from"].ToString();
                        password = dt_send_email.Rows[0]["Password"].ToString();
                    }


                    body = body.Replace("[company_website]", Convert.ToString(company_email_details.Rows[0]["company_website"]));
                    //from = company_email_details.Rows[0]["Email_Convey_from"].ToString();
                    //password = company_email_details.Rows[0]["Password"].ToString();
                    body = body.Replace("[company_name]", company_email_details.Rows[0]["Company_Name"].ToString());
                    body = body.Replace("[Company_reg_no]", company_email_details.Rows[0]["CompanyReg_No"].ToString());
                    body = body.Replace("[Company_reg_office]", company_email_details.Rows[0]["Company_Address"].ToString());
                    body = body.Replace("[contact_no]", company_email_details.Rows[0]["Company_mobile"].ToString());
                    body = body.Replace("[email_id]", company_email_details.Rows[0]["Company_Email"].ToString());
                    string send_mail = "mailto:" + company_email_details.Rows[0]["Company_Email"].ToString();
                    body = body.Replace("[email_id1]", send_mail);
                    body = body.Replace("[privacy_policy]", company_email_details.Rows[0]["privacy_policy"].ToString());
                    body = body.Replace("[company_logo]", company_email_details.Rows[0]["Image"].ToString());
                    body = body.Replace("[theme_color]", Convert.ToString(company_email_details.Rows[0]["Brand_Color"]));


                    mailMessage.From = new MailAddress(from);


                    string Email_ID = email_id;
                    if (Email_ID != "" && Email_ID != null)
                    {
                        mailMessage.To.Add(new MailAddress(Email_ID));
                        if (Status == 0)
                        {
                            mailMessage.Bcc.Add(new MailAddress(from));
                        }
                    }
                    else
                    {
                        if (Status == 0)
                        {
                            mailMessage.To.Add(new MailAddress(from));
                        }
                    }
                    // mailMessage.Bcc.Add(new MailAddress("info@riotransfer.com"));
                    //mailMessage.Bcc.Add(new MailAddress("samikshapatil.calyx@gmail.com"));
                    // mailMessage.From = new MailAddress(from);
                    mailMessage.Body = body;
                    mailMessage.Subject = subject;
                    mailMessage.BodyEncoding = System.Text.Encoding.UTF8;
                    mailMessage.Priority = MailPriority.High;
                    mailMessage.IsBodyHtml = true;

                    SmtpClient smtp = new SmtpClient();
                    smtp.Host = dt_send_email.Rows[0]["Host"].ToString();
                    smtp.EnableSsl = true;
                    System.Net.NetworkCredential NetworkCred = new System.Net.NetworkCredential(from, password);
                    smtp.UseDefaultCredentials = true;
                    smtp.Credentials = NetworkCred;
                    smtp.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
                    smtp.Port = Convert.ToInt32(dt_send_email.Rows[0]["Port"].ToString());
                    try
                    {
                        smtp.Send(mailMessage);
                        Content("Customer Details added Successfully!");
                    }
                    catch (Exception ex)
                    {

                        string msg = ex.ToString();
                        string stattus = (string)CompanyInfo.InsertErrorLogTracker(ex.ToString(), 0, 0, 0, 0, "send_mail", Convert.ToInt32(branch_id), Convert.ToInt32(client_id), "", HttpContext);

                        try
                        {
                            string from1 = string.Empty;
                            string password1 = string.Empty;
                            //get send email details
                            MySqlConnector.MySqlCommand _cmd1 = new MySqlConnector.MySqlCommand("Get_Email_Configuration");
                            _cmd1.CommandType = CommandType.StoredProcedure;
                            string _whereclause1 = " and Client_ID=" + client_id + " and   priority=2";

                            _cmd1.Parameters.AddWithValue("_whereclause", _whereclause);
                            _cmd1.Parameters.AddWithValue("_SecurityKey", CompanyInfo.SecurityKey());
                            DataTable dt_send_email1 = db_connection.ExecuteQueryDataTableProcedure(_cmd1);
                            if (dt_send_email1.Rows.Count > 0)
                            {

                                from1 = dt_send_email1.Rows[0]["Email_Convey_from"].ToString();
                                password1 = dt_send_email1.Rows[0]["Password"].ToString();
                                smtp.Host = dt_send_email1.Rows[0]["Host"].ToString();
                                mailMessage.From = new MailAddress(from1);
                                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
                                smtp.EnableSsl = true;
                                NetworkCred = new System.Net.NetworkCredential(from1, password1);
                                smtp.UseDefaultCredentials = true;
                                smtp.Credentials = NetworkCred;
                                smtp.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
                                smtp.Port = Convert.ToInt16(dt_send_email1.Rows[0]["Port"].ToString());
                                try
                                {
                                    smtp.Send(mailMessage);
                                    msg = "Success";
                                }
                                catch (Exception ex2)
                                {

                                    msg = ex2.ToString();
                                    string stattus1 = (string)CompanyInfo.InsertErrorLogTracker(ex.ToString(), 0, 0, 0, 0, "send_mail", Convert.ToInt32(branch_id), Convert.ToInt32(client_id), "", HttpContext);

                                    string from2 = string.Empty;
                                    string password2 = string.Empty;
                                    MySqlConnector.MySqlCommand _cmd2 = new MySqlConnector.MySqlCommand("Get_Email_Configuration");
                                    _cmd2.CommandType = CommandType.StoredProcedure;
                                    string _whereclause2 = " and Client_ID=" + client_id + " and   priority=1";

                                    _cmd.Parameters.AddWithValue("_whereclause", _whereclause);
                                    _cmd.Parameters.AddWithValue("_SecurityKey", CompanyInfo.SecurityKey());
                                    DataTable dt_send_email2 = db_connection.ExecuteQueryDataTableProcedure(_cmd);
                                    if (dt_send_email2.Rows.Count > 0)
                                    {

                                        from2 = dt_send_email2.Rows[0]["Email_Convey_from"].ToString();
                                        password2 = dt_send_email2.Rows[0]["Password"].ToString();
                                        smtp.Host = dt_send_email2.Rows[0]["Host"].ToString();
                                        smtp.EnableSsl = true;
                                        NetworkCred = new System.Net.NetworkCredential(from2, password2);
                                        smtp.UseDefaultCredentials = true;
                                        mailMessage.From = new MailAddress(from2);
                                        System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
                                        smtp.Credentials = NetworkCred;
                                        smtp.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
                                        smtp.Port = Convert.ToInt16(dt_send_email2.Rows[0]["Port"].ToString());
                                        try
                                        {
                                            smtp.Send(mailMessage);
                                            msg = "Success";
                                        }
                                        catch (Exception ex3)
                                        {
                                            msg = ex3.ToString();

                                            CompanyInfo.InsertErrorLogTracker(ex.ToString(), 0, 0, 0, 0, "send_mail", Convert.ToInt32(branch_id), Convert.ToInt32(client_id), "", HttpContext);

                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception ex1)
                        {
                            msg = ex1.ToString();
                            string stattus2 = (string)CompanyInfo.InsertErrorLogTracker(ex.ToString(), 0, 0, 0, 0, "send_mail", Convert.ToInt32(branch_id), Convert.ToInt32(client_id), "", HttpContext); ;

                        }
                    }
                }
            }
            else
            {
                context.Response.WriteAsJsonAsync("Invalid!");
                string msg = "SQl Enjection detetcted";
                string stattus3 = (string)CompanyInfo.InsertErrorLogTracker(msg, 0, 0, 0, 0, "send_mail", Convert.ToInt32(branch_id), Convert.ToInt32(client_id), "", HttpContext); ;
            }



            return validateJsonData;


        }





        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;

        public ContactUsController(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            this._configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }


        [ApiExplorerSettings(IgnoreApi = true)]
        public static bool ReCaptchaPassed(string secret_key, string gRecaptchaResponse)
        {
            HttpClient httpClient = new HttpClient();

            var res = httpClient.GetAsync("https://www.google.com/recaptcha/api/siteverify?secret=" + secret_key + "&response=" + gRecaptchaResponse + "").Result;

            if (res.StatusCode != HttpStatusCode.OK)
            {
                return false;
            }
            string JSONres = res.Content.ReadAsStringAsync().Result;
            dynamic JSONdata = JObject.Parse(JSONres);

            if (JSONdata.success != "true" || JSONdata.score <= 0.5m)
            {
                return false;
            }

            return true;
        }


        string IPAddress1 = "";

        [ApiExplorerSettings(IgnoreApi = true)]
        private string GetUserIP()
        {
            string gt = Request.HttpContext.Connection.RemoteIpAddress.ToString();
            return gt;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            // Other service configurations
        }



        [ApiExplorerSettings(IgnoreApi = true)]
        public string GetBrowserDetails(HttpContext httpContext)
        {
            string userAgent = httpContext.Request.Headers["User-Agent"].ToString();

            string browserDetails = string.Empty;

            // You can use libraries like Microsoft.AspNetCore.Http.Extensions to parse user-agent
            var uaParser = UAParser.Parser.GetDefault();
            ClientInfo clientInfo = uaParser.Parse(userAgent);

            browserDetails =
                "Name = " + clientInfo.UserAgent.Family + "," +
                "Type = " + "Web Browser" + "," + // Assuming all are web browsers
                "Version = " + clientInfo.UserAgent.Major + "." + clientInfo.UserAgent.Minor + "," +
                "Major Version = " + clientInfo.UserAgent.Major + "," +
                "Minor Version = " + clientInfo.UserAgent.Minor + "," +
                "Platform = " + clientInfo.OS.Family;

            return browserDetails;
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        public void getEntryInLoginTabel(string enq, string username, HttpContext context)
        {
            //ICustomer c = null;
            //c = FactoryCustomer.getCustomer();
            //c.Client_ID = Convert.ToInt32(HttpContext.Current.Session["Client_ID"]);
            //string BaseCurrency_Timezone = "";
            //DataTable dtbasecurrencydetails = (DataTable)mtsmethods.GetBaseCurrencydetails(c);

            //BaseCurrency_Timezone = Convert.ToString(dtbasecurrencydetails.Rows[0]["BaseCurrency_Timezone"]);

            //string username = Convert.ToString(HttpContext.Current.Session["LoginName"]);
            //int empid = Convert.ToInt32(HttpContext.Current.Session["User_ID"]);
            //string branchkey = Convert.ToString(HttpContext.Current.Session["Branch_Key"]);
            TimeZoneInfo timeZoneInfo;
            DateTime dateTime;
            //Set the time zone information to US Mountain Standard Time 
            timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time");
            dateTime = TimeZoneInfo.ConvertTime(DateTime.Now, timeZoneInfo);
            string getdate = (dateTime).ToString("yyyy-MM-dd hh:mm:ss");
            string logintime = (dateTime).ToString("HH:mm:ss");
            IPAddress1 = GetUserIP();

            var host = HttpContext.Connection.RemoteIpAddress;
            string serverName = HttpContext.Request.Host.Value;
            var serverport = HttpContext.Request.Host.Port;
            string localAddr = context.Connection.LocalIpAddress?.ToString();
            string user = HttpContext.User.Identity.Name;
            string Remoteuser = HttpContext.User.Identity.Name;
            var Remotehost = HttpContext.Connection.RemoteIpAddress;
            //string serverSft = httpContextAccessor.HttpContext.Features.Get<IServerVariablesFeature>()?["SERVER_SOFTWARE"];
            string serverSft = "";

            string browser = GetBrowserDetails(context);

            string location = "Browser information : " + browser + ",Logon user : " + user + ", Server name : " + serverName + " Server port :" + serverport + " , Remote User :" + Remoteuser + ", server software : " + serverSft + "";
            try
            {
                //string strMsg = string.Empty;
                //string strMsg1 = string.Empty;
                //ICustomer loginaudit = null;
                //loginaudit = FactoryCustomer.getCustomer();

                MySqlCommand cmd = new MySqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "SP_SaveLoginAudit";
                cmd.Connection = con;
                //    con.Open();
                //if (con.State != ConnectionState.Open)
                //{
                //    con.Open();
                //}

                //cmd.Parameters.AddWithValue("_Enquiry_Id",enq );
                cmd.Parameters.AddWithValue("_Login_Datetime", getdate);
                cmd.Parameters.AddWithValue("_Login_time", logintime);
                cmd.Parameters.AddWithValue("_Location", location);
                cmd.Parameters.AddWithValue("_IP_Address", IPAddress1);
                cmd.Parameters.AddWithValue("_UserName", username);
                int n = db_connection.ExecuteNonQueryProcedure(cmd);
                //loginaudit.User_ID = Convert.ToInt32(HttpContext.Current.Session["User_ID"]);
                //loginaudit.UserName = Convert.ToString(HttpContext.Current.Session["LoginName"]);
                //loginaudit.Branch_ID = Convert.ToInt32(HttpContext.Current.Session["Branch_ID"]);
                //loginaudit.Location = location;
                //loginaudit.IPAddress = IPAddress1;
                //loginaudit.Login_DateTime = getdate;
                //loginaudit.Login_Time = logintime;
                //loginaudit.Client_ID = Convert.ToInt32(HttpContext.Current.Session["Client_ID"]);
                //loginaudit.Is_Procedure = "QUERY";
                //loginaudit.Operation_Name = "operation_insertloginaudit";
                //strMsg = loginaudit.InsertToDatabase().ToString();

                //DataTable dt = new DataTable();
                //loginaudit.Is_Procedure = "QUERY";
                //loginaudit.Operation_Name = "get_LoginId";
                //dt = (DataTable)loginaudit.ReadFromDatabase();
                //strMsg1 = Convert.ToString(dt.Rows[0]["Login_Id"]);
                //HttpContext.Current.Session["Login_ID"] = strMsg1;

            }
            catch (Exception ee)
            {

            }
        }



        [DataContract]
        public class RecaptchaApiResponse
        {
            [DataMember(Name = "success")]
            public bool Success;

            [DataMember(Name = "error-codes")]
            public List<string> ErrorCodes;
        }
       









    }
}
