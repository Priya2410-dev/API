using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
 
using System.IO; 
using System.Web;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System.Data.Common;
using System.Xml;
using System.Security.Claims;
using System.Text.Json;
using System.Xml;
using System.Data.Common;
using System.Data.Odbc;
using System.Web;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Calyx_Solutions.Controllers;
using Microsoft.AspNetCore.Mvc;
using RestSharp;
using Calyx_Solutions.Service;
using antiSQLInjection;
using System.Data.SqlClient;
 
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using MySqlConnector;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
 
 
using System.IO;
 
using gudusoft.gsqlparser;
//using antiSQLInjection;

using System.Web;
 
 
 
 
using Newtonsoft.Json.Linq;
using System.IO;
using iTextSharp.xmp.options;
 
using System.Data.Common;
using System.Xml;
 
 
using Google.Apis.Auth.OAuth2;
using System.Xml;
using System.Data.Common;
using System.Data.Odbc;

using Newtonsoft.Json;
using System.Web.Helpers;
using FirebaseAdmin.Messaging;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Net.Http.Headers;


 


namespace Calyx_Solutions
{
    public static class CompanyInfo
    {


      

        public static DataTable ConvertDateToCompanyConfiguredFormat(string dateString, int client_id) // Digvijay changes for date format changes
        {
            HttpContext context = null; 
            DataTable dtrec = new DataTable();
            string errMessage = string.Empty;
            try
            {
                // Assuming the dateString is in a proper format (e.g., 'yyyy-MM-dd HH:mm:ss')
                // You may want to convert this to an integer Unix timestamp depending on your needs.

                DateTime inputDateTime = DateTime.Parse(dateString);
                int timestamp = (int)(inputDateTime.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                MySqlCommand cm = new MySqlCommand("Get_Formated_Date");
                cm.CommandType = CommandType.StoredProcedure;
                cm.Parameters.AddWithValue("_Client_ID", client_id);
                cm.Parameters.AddWithValue("_inputdate", timestamp);
                cm.Parameters.AddWithValue("_dateperm_flag", "Status_ForCustomer");

                dtrec = db_connection.ExecuteQueryDataTableProcedure(cm);
                return dtrec;
            }
            catch (Exception ex)
            {
                Model.ErrorLog objError = new Model.ErrorLog();
                objError.User = new Model.User();

                objError.Error = ex.ToString();
                objError.Date = DateTime.Now;
                objError.User_ID = 1;
                objError.Id = 1;
                objError.Function_Name = "ConvertDateToCompanyConfiguredFormat";
                Service.srvErrorLog srvError = new Service.srvErrorLog();
                srvError.Create(objError, context);
                return dtrec;
            }
        }

        public static object InsertErrorLogDetails(string Error, int user_id, string Function_name, int branch_id, int client_id)
        {
            try
            {
                string strFilePath = @"C:\inetpub\wwwroot\cs-remit\Test-MTS\mts-admin\assets\Errorlog.txt";
                if (!File.Exists(strFilePath)) //No File? Create
                {
                    FileStream fs = File.Create(strFilePath);
                    fs.Close();
                }
                File.AppendAllText(strFilePath, Environment.NewLine + DateTime.Now + Environment.NewLine +
                    " Error Log=>" + Error + "=> Function=>" + Function_name + Environment.NewLine);
            }
            catch { }

            return "";
        }
        public static string getAgentContext( )
        {
           /* HttpContext context;
             
            var userAgent = Request.Headers.UserAgent.ToString();
             return userAgent; */
            return "";
        }
        public static string getUrlReferrer()
        {
            /*var urlReferrer = Request.Headers["Referer"].ToString();
            return urlReferrer;*/
            return "";
        }

        public static string path_query(HttpContext context)
        {            
            var path = $"{context.Request.Path}";
            var query = $"{context.Request.QueryString}";
            return path + query;
        }
        public static string getSessionHttpVal(string sessionVal, HttpContext context)
        {        
            string value = context.Session.GetString(sessionVal);
            return value;
        }

        public static string path_query2(HttpContext context)
        {
            var path = $"{context.Request.Path}";
            var query = $"{context.Request.QueryString}";

            return path + query;
        }

        public static string ipAddressContext(HttpContext context)
        {   // Get Ip address           
            string ipAddress = context.Connection.RemoteIpAddress.ToString();
            return ipAddress;
        }

        public static void setSessionHttpValStr(string sessionVal, string value, HttpContext context)
        {
            try
            {
                context.Session.SetString(sessionVal, Convert.ToString(value));
            }
            catch(Exception ex) { }
        }

        public static void setSessionHttpVal(string sessionVal, bool result, HttpContext context)
        {
            try
            {
                context.Session.SetString(sessionVal, Convert.ToString(result));
            }
            catch (Exception ex) { }
        }

        #region zoho configuration
        public static string getZohoLeadIdDetails(string zohoAccessToken, int zohoAPIID, DataTable dtuser, HttpContext context)
        {
            string leadID = "";
            try
            {
                string emailaddress = dtuser.Rows[0]["emailaddress"].ToString().Trim();
                string Query = "select * from thirdpartyapi_master where Delete_Status=0 and Client_ID=1 and API_ID=" + zohoAPIID + " and API_Status= 0";
                MySqlCommand cmd5 = new MySqlCommand(Query);
                DataTable dtt = db_connection.ExecuteQueryDataTableProcedure(cmd5);

                string apiurl = "", api_fields = "";
                if (dtt.Rows.Count > 0)
                {
                    api_fields = Convert.ToString(dtt.Rows[0]["ProfileID"]);
                    apiurl = Convert.ToString(dtt.Rows[0]["API_URL"]);
                }

                var client = new RestClient(apiurl + "crm/v2/Leads/search?email=" + emailaddress);
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
                       | SecurityProtocolType.Tls11
                       | SecurityProtocolType.Tls12
                       | SecurityProtocolType.Ssl3;
                client.Timeout = -1;
                var request = new RestRequest(Method.GET);
                request.AddHeader("Authorization", zohoAccessToken);
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("Cookie", "1a99390653=0a0cd8f8f863c606ad892b381901fd90; 1ccad04dca=c0a8160b68ea01a408b495dec135dc0c; JSESSIONID=C931C8027C9CDD69B18E2A791665522E; _zcsr_tmp=d77c09c7-2935-4f36-9d27-2c542ca46d7c; crmcsr=d77c09c7-2935-4f36-9d27-2c542ca46d7c");
                var response = client.Execute(request);
                Console.WriteLine(response.Content);
                var obj1 = Newtonsoft.Json.Linq.JObject.Parse(response.Content);
                if (obj1 != null)
                {
                    leadID = Convert.ToString(obj1["data"][0]["id"]);
                }
            }
            catch (Exception ex)
            {
                leadID = "";
                int stattus = (int)CompanyInfo.InsertActivityLogDetailsSecurity("Zoho Lead Get Details error " + ex.ToString(), Convert.ToInt32(0), 0, Convert.ToInt32(0), 1, "srvDocument", Convert.ToInt32(0), Convert.ToInt32(0), "getZohoLeadIdDetails", 0, context);
            }
            return leadID;
        }
        public static string updateZohoLeadDetails(string zohoAccessToken, int zohoAPIID, DataTable dtuser, HttpContext context)
        {
            string updateRecordStatus = "F";
            try
            {
                string Query = "select * from thirdpartyapi_master where Delete_Status=0 and Client_ID=1 and API_ID=" + zohoAPIID + " and API_Status= 0";
                MySqlCommand cmd5 = new MySqlCommand(Query);
                DataTable dtt = db_connection.ExecuteQueryDataTableProcedure(cmd5);

                string client_idcode = "", client_secret = "", refresh_token = "", apiurl = "", Company_Name = "", api_fields = "", createleadID = "";
                if (dtt.Rows.Count > 0)
                {
                    client_idcode = Convert.ToString(dtt.Rows[0]["UserName"]);
                    client_secret = Convert.ToString(dtt.Rows[0]["Password"]);
                    api_fields = Convert.ToString(dtt.Rows[0]["ProfileID"]);
                    apiurl = Convert.ToString(dtt.Rows[0]["API_URL"]);

                    if (api_fields != "" && api_fields != null)
                    {
                        Newtonsoft.Json.Linq.JObject obj = Newtonsoft.Json.Linq.JObject.Parse(api_fields);
                        createleadID = Convert.ToString(obj["createLeadId"]);
                        refresh_token = Convert.ToString(obj["refresh_token"]);
                        Company_Name = Convert.ToString(obj["company_name"]);
                    }
                }

                string firstname = "", lastname = "", emailaddress = "",
                    description = "\"" + "" + "\"", street = "\"" + "" + "\"", state = "\"" + "" + "\"", city = "\"" + "" + "\"";
                string url = "\"" + "" + "\"";
                try { firstname = dtuser.Rows[0]["firstname"].ToString().Trim(); } catch (Exception ex) { }
                try { lastname = dtuser.Rows[0]["lastname"].ToString().Trim(); } catch (Exception ex) { }
                try
                { emailaddress = dtuser.Rows[0]["emailaddress"].ToString().Trim(); }
                catch (Exception ex) { }

                try
                { description = "\"" + dtuser.Rows[0]["description"].ToString() + "\""; }
                catch (Exception ex) { } //  Description
                try { street = "\"" + dtuser.Rows[0]["street"].ToString() + "\""; } catch (Exception ex) { } // Coupon code
                try { state = "\"" + dtuser.Rows[0]["state"].ToString() + "\""; } catch (Exception ex) { }// Expiry Date
                try
                { city = "\"" + dtuser.Rows[0]["city"].ToString() + "\""; }
                catch (Exception ex) { } // Ref code
                try { url = "\"" + dtuser.Rows[0]["url"].ToString() + "\""; } catch (Exception) { }// url Date
                var client = new RestClient(apiurl + "crm/v2/Leads/upsert");
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
                       | SecurityProtocolType.Tls11
                       | SecurityProtocolType.Tls12
                       | SecurityProtocolType.Ssl3;
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                request.AddHeader("Authorization", zohoAccessToken);
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("Cookie", "1a99390653=b625816955daa3a7057207cc860a644a; 1ccad04dca=4756a8194d77973b28ad9c52af71ea24; _zcsr_tmp=de223f22-dd75-4dbd-81ab-dfac39a352ee; crmcsr=de223f22-dd75-4dbd-81ab-dfac39a352ee");

                Company_Name = "\"" + Company_Name + "\"";
                string custFirstName = "\"" + Convert.ToString(firstname).Trim() + "\"";
                string custLastName = "\"" + Convert.ToString(lastname).Trim() + "\"";
                string custEmail = "\"" + Convert.ToString(emailaddress).Trim() + "\"";
                string WireTransfer_ReferanceNo = "\"" + "" + "\"";
                //createleadID = "\"" + Convert.ToString(createleadID).Trim() + "\"";
                var body = @"{
                    " + "\n" +
                                    @"    ""data"": [" +
                                    @"        {" +
                                    @"            ""Company"": " + Company_Name + "," +
                                    @"            ""Last_Name"": " + custLastName + "," +
                                    @"            ""First_Name"":  " + custFirstName + " ," +
                                    @"            ""Email"": " + custEmail + "," +
                                    @"            ""State"": " + state + "," +
                                    @"            ""Street"": " + street + "," +
                                    @"            ""Website"": " + url + "," +
                                    @"            ""City"": " + city + "," +
                                    @"            ""Description"": " + description + "," +
                                    @"        }      
                    " + "\n" +
                                    @"    ] " +
                @"}";
                request.AddParameter("application/json", body, ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);
                Console.WriteLine(response.Content);
                var obj1 = Newtonsoft.Json.Linq.JObject.Parse(response.Content);
                if (obj1 != null)
                {
                    if (Convert.ToString(obj1["data"][0]["status"]) == "success")
                    {
                        updateRecordStatus = "T";
                    }
                }

            }
            catch (Exception ex)
            {
                int stattus = (int)CompanyInfo.InsertActivityLogDetailsSecurity("Zoho Lead Update error " + ex.ToString(), Convert.ToInt32(0), 0, Convert.ToInt32(0), 1, "srvDocument", Convert.ToInt32(0), Convert.ToInt32(0), "updateZohoLeadDetails", 0, context);
                //m.InsertActivityLogDetails("Zoho Lead Create error " + ex.ToString(), 0, 0, 0, 0, "generateZohoLead", 0, 1);
            }
            return updateRecordStatus;
        }
        public static string generateZohoLead(string zohoAccessToken, int zohoAPIID, DataTable dtuser, HttpContext context)
        {
            string leadID = "";
            try
            {
                string Query = "select * from thirdpartyapi_master where Delete_Status=0 and Client_ID=1 and API_ID=" + zohoAPIID + " and API_Status= 0";
                MySqlCommand cmd5 = new MySqlCommand(Query);
                DataTable dtt = db_connection.ExecuteQueryDataTableProcedure(cmd5);

                string client_idcode = "", client_secret = "", refresh_token = "", apiurl = "", Company_Name = "", api_fields = "", createleadID = "";
                if (dtt.Rows.Count > 0)
                {
                    client_idcode = Convert.ToString(dtt.Rows[0]["UserName"]);
                    client_secret = Convert.ToString(dtt.Rows[0]["Password"]);
                    api_fields = Convert.ToString(dtt.Rows[0]["ProfileID"]);
                    apiurl = Convert.ToString(dtt.Rows[0]["API_URL"]);

                    if (api_fields != "" && api_fields != null)
                    {
                        Newtonsoft.Json.Linq.JObject obj = Newtonsoft.Json.Linq.JObject.Parse(api_fields);
                        createleadID = Convert.ToString(obj["createLeadId"]);
                        refresh_token = Convert.ToString(obj["refresh_token"]);
                        Company_Name = Convert.ToString(obj["company_name"]);
                    }
                }

                string firstname = "", lastname = "", emailaddress = "",
                    description = "\"" + "" + "\"", street = "\"" + "" + "\"", state = "\"" + "" + "\"", city = "\"" + "" + "\"";
                string url = "\"" + "" + "\"";

                try { firstname = dtuser.Rows[0]["firstname"].ToString().Trim(); } catch (Exception) { }
                try { lastname = dtuser.Rows[0]["lastname"].ToString().Trim(); } catch (Exception) { }
                try { emailaddress = dtuser.Rows[0]["emailaddress"].ToString().Trim(); } catch (Exception) { }

                try { description = "\"" + dtuser.Rows[0]["description"].ToString() + "\""; } catch (Exception) { } //  Description
                try { street = "\"" + dtuser.Rows[0]["street"].ToString() + "\""; } catch (Exception) { } // Coupon code
                try { state = "\"" + dtuser.Rows[0]["state"].ToString() + "\""; } catch (Exception) { }// Expiry Date
                try { url = "\"" + dtuser.Rows[0]["url"].ToString() + "\""; } catch (Exception) { }// url Date

                try
                { city = "\"" + dtuser.Rows[0]["city"].ToString() + "\""; }
                catch (Exception ex) { } // Ref code

                var client = new RestClient(apiurl + "crm/v2/Leads");
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
                       | SecurityProtocolType.Tls11
                       | SecurityProtocolType.Tls12
                       | SecurityProtocolType.Ssl3;
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                request.AddHeader("Authorization", zohoAccessToken);
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("Cookie", "1a99390653=b625816955daa3a7057207cc860a644a; 1ccad04dca=4756a8194d77973b28ad9c52af71ea24; _zcsr_tmp=de223f22-dd75-4dbd-81ab-dfac39a352ee; crmcsr=de223f22-dd75-4dbd-81ab-dfac39a352ee");

                Company_Name = "\"" + Company_Name + "\"";
                string custFirstName = "\"" + Convert.ToString(firstname).Trim() + "\"";
                string custLastName = "\"" + Convert.ToString(lastname).Trim() + "\"";
                string custEmail = "\"" + Convert.ToString(emailaddress).Trim() + "\"";
                string WireTransfer_ReferanceNo = "\"" + "" + "\"";
                createleadID = "\"" + Convert.ToString(createleadID).Trim() + "\"";
                var body = @"{
                    " + "\n" +
                                    @"    ""data"": [" +
                                    @"        {" +
                                    @"           ""Layout"": {" +
                                    @"                ""id"": " + createleadID + "" +
                                    @"            }," +
                                    @"            ""Lead_Source"": """"," +
                                    @"            ""Company"": " + Company_Name + "," +
                                    @"            ""Last_Name"": " + custLastName + "," +
                                    @"            ""First_Name"":  " + custFirstName + " ," +
                                    @"            ""Email"": " + custEmail + "," +
                                    @"            ""Reference_Number"": " + WireTransfer_ReferanceNo + "," +
                                    @"            ""State"": " + state + "," +
                                    @"            ""Street"": " + street + "," +
                                    @"            ""City"": " + city + "," +
                                    @"            ""Website"": " + url + "," +
                                    @"            ""Description"": " + description + "," +
                                    @"            ""Title"": """"
                    " + "\n" +
                                    @"        }      
                    " + "\n" +
                                    @"    ],
                    " + "\n" +
                                    @"    ""apply_feature_execution"": [
                    " + "\n" +
                                    @"        {
                    " + "\n" +
                                    @"            ""name"": ""layout_rules""
                    " + "\n" +
                                    @"        }
                    " + "\n" +
                                    @"    ],
                    " + "\n" +
                                    @"    ""trigger"": [
                    " + "\n" +
                                    @"        ""approval"",
                    " + "\n" +
                                    @"        ""workflow"",
                    " + "\n" +
                                    @"        ""blueprint""
                    " + "\n" +
                                    @"    ]
                    " + "\n" +
                @"}";
                request.AddParameter("application/json", body, ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);
                Console.WriteLine(response.Content);
                var obj1 = Newtonsoft.Json.Linq.JObject.Parse(response.Content);
                if (obj1 != null)
                {
                    if (Convert.ToString(obj1["data"][0]["status"]) == "success")
                    {
                        leadID = Convert.ToString(obj1["data"][0]["details"]["id"]);
                    }
                }

            }
            catch (Exception ex)
            {
                int stattus = (int)CompanyInfo.InsertActivityLogDetailsSecurity("Zoho Lead Create error " + ex.ToString(), Convert.ToInt32(0), 0, Convert.ToInt32(0), 1, "srvDocument", Convert.ToInt32(0), Convert.ToInt32(0), "generateZohoLead", 0, context);
            }
            return leadID;
        }
        public static string generateZohoToken(int zohoAPIID, HttpContext context)
        {
            string token = "";
            try
            {
                string Query = "select * from thirdpartyapi_master where Delete_Status=0 and Client_ID=1 and API_ID=" + zohoAPIID + " and API_Status= 0";
                MySqlCommand cmd5 = new MySqlCommand(Query);
                DataTable dtt = db_connection.ExecuteQueryDataTableProcedure(cmd5);

                string client_idcode = "", client_secret = "", refresh_token = "", apiurl = "", Company_Name = "", api_fields = "";
                string templateId = "", tokenurl = "";
                if (dtt.Rows.Count > 0)
                {
                    client_idcode = Convert.ToString(dtt.Rows[0]["UserName"]);
                    client_secret = Convert.ToString(dtt.Rows[0]["Password"]);
                    api_fields = Convert.ToString(dtt.Rows[0]["ProfileID"]);
                    apiurl = Convert.ToString(dtt.Rows[0]["API_URL"]);
                    Company_Name = Convert.ToString(dtt.Rows[0]["Company_Name"]);

                    if (api_fields != "" && api_fields != null)
                    {
                        Newtonsoft.Json.Linq.JObject obj = Newtonsoft.Json.Linq.JObject.Parse(api_fields);
                        templateId = Convert.ToString(obj["birthdatetempid"]);
                        refresh_token = Convert.ToString(obj["refresh_token"]);
                        tokenurl = Convert.ToString(obj["tokenurl"]);
                    }
                }

                var client = new RestClient(tokenurl);
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
                       | SecurityProtocolType.Tls11
                       | SecurityProtocolType.Tls12
                       | SecurityProtocolType.Ssl3;
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                request.AddHeader("Cookie", "_zcsr_tmp=530f134a-ee09-4863-9b40-15357f608baa; b266a5bf57=57c7a14afabcac9a0b9dfc64b3542b70; iamcsr=530f134a-ee09-4863-9b40-15357f608baa");
                request.AddParameter("refresh_token", refresh_token);
                request.AddParameter("client_id", client_idcode);
                request.AddParameter("client_secret", client_secret);
                request.AddParameter("grant_type", "refresh_token");
                IRestResponse response = client.Execute(request);
                Console.WriteLine(response.Content);

                var obj1 = Newtonsoft.Json.Linq.JObject.Parse(response.Content);
                if (obj1 != null)
                {
                    token = Convert.ToString("Zoho-oauthtoken " + obj1["access_token"]);
                }
            }
            catch (Exception ex)
            {
                token = "";
                int stattus = (int)CompanyInfo.InsertActivityLogDetailsSecurity("Zoho token error " + ex.ToString(), Convert.ToInt32(0), 0, Convert.ToInt32(0), 1, "srvDocument", Convert.ToInt32(0), Convert.ToInt32(0), "generateZohoToken", 0, context);
            }
            return token;
        }

        public static string unsubscribeZohoMail(string custId, HttpContext context)
        {
            try
            {   // 0 means regular and 1 means unsubscribed. When 1 no any mail send to customer
                string Query = "update customer_registration set unsubscribe_zoho = 1 where  Customer_ID = '" + custId + "' ";
                MySqlCommand cmd5 = new MySqlCommand(Query);
                DataTable dtt = db_connection.ExecuteQueryDataTableProcedure(cmd5);
            }
            catch (Exception ex)
            {
                int stattus = (int)CompanyInfo.InsertActivityLogDetailsSecurity("Zoho unsubscribe error " + ex.ToString(), Convert.ToInt32(0), 0, Convert.ToInt32(0), 1, "srvDocument", Convert.ToInt32(0), Convert.ToInt32(0), "unsubscribeZohoMail", 0, context);
            }
            return "T";
        }
        public static string checkZohoMailUnsubscribeStatus(string custId, HttpContext context)
        {
            string activeStatus = "N";
            try
            {
                string Query = "select  unsubscribe_zoho from  customer_registration  where  Customer_ID = '" + custId + "' and unsubscribe_zoho = 0 ";
                MySqlCommand cmd5 = new MySqlCommand(Query);
                DataTable dtt = db_connection.ExecuteQueryDataTableProcedure(cmd5);
                if (dtt.Rows.Count > 0)
                {
                    activeStatus = "A"; //Convert.ToString(dtt.Rows[0]["unsubscribe_zoho"]);
                }
            }
            catch (Exception ex)
            {
                int stattus = (int)CompanyInfo.InsertActivityLogDetailsSecurity("Zoho checkZohoMailUnsubscribeStatus error " + ex.ToString(), Convert.ToInt32(0), 0, Convert.ToInt32(0), 1, "srvDocument", Convert.ToInt32(0), Convert.ToInt32(0), "checkZohoMailUnsubscribeStatus", 0, context);
            }
            return activeStatus;
        }

        #endregion

        public static string Encrypt1(string text)
        {
            string key = SecurityKey();
            using (var md5 = new MD5CryptoServiceProvider())
            {
                using (var tdes = new TripleDESCryptoServiceProvider())
                {
                    tdes.Key = md5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
                    tdes.Mode = CipherMode.ECB;
                    tdes.Padding = PaddingMode.PKCS7;

                    using (var transform = tdes.CreateEncryptor())
                    {
                        byte[] textBytes = UTF8Encoding.UTF8.GetBytes(text);
                        byte[] bytes = transform.TransformFinalBlock(textBytes, 0, textBytes.Length);
                        return Convert.ToBase64String(bytes, 0, bytes.Length);
                    }
                }
            }
        }
        public static string Decrypt1(string cipher)
        {
            string key = SecurityKey();
            using (var md5 = new MD5CryptoServiceProvider())
            {
                using (var tdes = new TripleDESCryptoServiceProvider())
                {
                    tdes.Key = md5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
                    tdes.Mode = CipherMode.ECB;
                    tdes.Padding = PaddingMode.PKCS7;

                    using (var transform = tdes.CreateDecryptor())
                    {
                        byte[] cipherBytes = Convert.FromBase64String(cipher);
                        byte[] bytes = transform.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
                        return UTF8Encoding.UTF8.GetString(bytes);
                    }
                }
            }
        }


        public static string chk_twostep_flag(int client_id, int customer_id, string reason)
        {
            string msg = "";
            try
            {
                MySqlConnector.MySqlCommand _cmd_valid = new MySqlConnector.MySqlCommand("block_login_app");//"select    cast(AES_DECRYPT(UNHEX(u.Password), '" + obj.SecurityKey.Trim() + "' ) as  char(500)) as Password,u.First_Name,u.Last_Name,u.Customer_ID,U.WireTransfer_ReferanceNo,u.Email_ID,u.Branch_ID,u.Client_ID from customer_registration u where    Email_ID = '" + obj.Name.Trim() + "' and Password=HEX(AES_ENCRYPT('" + obj.Password.Trim() + "','" + obj.SecurityKey.Trim() + "'))  and u.delete_status=0 ;");
                _cmd_valid.CommandType = CommandType.StoredProcedure;
                _cmd_valid.Parameters.AddWithValue("_reason", reason);
                _cmd_valid.Parameters.AddWithValue("_loginName", customer_id);
                _cmd_valid.Parameters.AddWithValue("_Client_ID", client_id);
                _cmd_valid.Parameters.AddWithValue("_flag", "1");
                int i1 = Convert.ToInt32(db_connection.ExecuteScalarProcedure(_cmd_valid));
                if (i1 >= 0)
                {
                    msg = "Success";
                }
                else
                {
                    msg = "NotSuccess";
                }
            }
            catch { }
            return msg;

        }
        
        public static string Decrypt(string cipherString, bool useHashing)
        {
            try
            {
                byte[] keyArray;
                byte[] toEncryptArray = Convert.FromBase64String(cipherString);

                string key = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("Jwt")["SecurityKey"];

                /*
                System.Configuration.AppSettingsReader settingsReader = new AppSettingsReader();
                //Get your key from config file to open the lock!
                string key = (string)settingsReader.GetValue("SecurityKey", typeof(String));
                */
               
                if (useHashing)
                {
                    MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                    keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
                    hashmd5.Clear();
                }
                else
                    keyArray = UTF8Encoding.UTF8.GetBytes(key);

                TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
                tdes.Key = keyArray;
                tdes.Mode = CipherMode.ECB;
                tdes.Padding = PaddingMode.PKCS7;

                ICryptoTransform cTransform = tdes.CreateDecryptor();
                byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

                tdes.Clear();
                return UTF8Encoding.UTF8.GetString(resultArray);
            }
            catch
            {
                return "";
            }
        }

        public static async Task<int> InsertActivityLogDetailsSecurityasync(string Activity, int WhoAcessed, int Transaction_ID, int User_ID, int Customer_ID, string FunctionName, int Branch_ID, int Client_ID, string Module, int security_flag, HttpContext context)
        {
            await Task.Delay(10);
            Model.ActivityLogTracker _objActivityLog = new Model.ActivityLogTracker();
            _objActivityLog.Activity = Activity;
            _objActivityLog.FunctionName = FunctionName;
            _objActivityLog.Transaction_ID = Transaction_ID;
            _objActivityLog.WhoAcessed = WhoAcessed;
            _objActivityLog.Branch_ID = Branch_ID;
            _objActivityLog.Client_ID = Client_ID;
            _objActivityLog.Customer_ID = Customer_ID;
            _objActivityLog.Security_flag = security_flag;
            Service.srvActivityLogTracker srvActivityLog = new Service.srvActivityLogTracker();
            int _i = srvActivityLog.Create_activity_security(_objActivityLog, context);
            return _i;
        }

        public static object InsertActivityLogDetails(string Activity, int WhoAcessed, int Transaction_ID, int User_ID, int Customer_ID, string FunctionName, int Branch_ID, int Client_ID, string Module, HttpContext context)
        {
            Model.ActivityLogTracker _objActivityLog = new Model.ActivityLogTracker();
            _objActivityLog.Activity = Activity;
            _objActivityLog.FunctionName = FunctionName;
            _objActivityLog.Transaction_ID = Transaction_ID;
            _objActivityLog.WhoAcessed = WhoAcessed;
            _objActivityLog.Branch_ID = Branch_ID;
            _objActivityLog.Client_ID = Client_ID;
            _objActivityLog.Customer_ID = Customer_ID;
            Service.srvActivityLogTracker srvActivityLog = new Service.srvActivityLogTracker();
            int _i = srvActivityLog.Create(_objActivityLog, context);
            return _i;
        }

        public static object GetBaseCurrencywisebankdetails(int Client_ID, string Base_currency)
        {
            HttpContext context = null;
            string _wherecluase = "1=1 ";
            DataTable db = new DataTable();
            try
            {
                string strResult = string.Empty;
                if (Client_ID != 0 && Client_ID != null)
                {
                    _wherecluase = _wherecluase + " and cbd.company_id=" + Client_ID;
                }
                if (Base_currency != "" && Base_currency != null && Base_currency != "null")
                {
                    _wherecluase = _wherecluase + " and base_currency_code ='" + Base_currency + "'";
                }
                else
                {
                    _wherecluase = _wherecluase + " limit 1";
                }

                MySqlConnector.MySqlCommand cmd1 = new MySqlConnector.MySqlCommand("sp_select_company_bank_deatils");
                cmd1.CommandType = CommandType.StoredProcedure;
                cmd1.Parameters.AddWithValue("_wherecluase", _wherecluase);
                cmd1.Parameters.AddWithValue("_Timezone_Date", (string)CompanyInfo.gettime(Client_ID, context));
                db = (DataTable)db_connection.ExecuteQueryDataTableProcedure(cmd1);
                _wherecluase = " 1=1";
                if (db.Rows.Count == 0)
                {
                    if (Client_ID != 0 && Client_ID != null)
                    {
                        _wherecluase = _wherecluase + " and cbd.company_id=" + Client_ID;
                    }
                    _wherecluase = _wherecluase + " limit 1";
                    cmd1 = new MySqlConnector.MySqlCommand("sp_select_company_bank_deatils");
                    cmd1.CommandType = CommandType.StoredProcedure;
                    cmd1.Parameters.AddWithValue("_wherecluase", _wherecluase);
                    cmd1.Parameters.AddWithValue("_Timezone_Date", (string)CompanyInfo.gettime(Client_ID, context));
                    db = (DataTable)db_connection.ExecuteQueryDataTableProcedure(cmd1);
                }
                return db;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static object GetBaseCurrencywisebankdetails(int Client_ID, string Base_currency, int Collection_type_Id, int Delivery_type_Id)
        {
            string _wherecluase = "1=1 ";
            DataTable db = new DataTable();
            try
            {
                //added by vyankatesh
                int perm_status = 1;
                MySqlCommand _cmd = new MySqlCommand("GetPermissions");
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.Parameters.AddWithValue("_whereclause", " and PID = 158");
                _cmd.Parameters.AddWithValue("_Client_ID", Client_ID);
                DataTable perm = db_connection.ExecuteQueryDataTableProcedure(_cmd);
                if (perm.Rows.Count > 0)
                {
                    perm_status = Convert.ToInt32(perm.Rows[0]["Status_ForCustomer"]);
                }
                string strResult = string.Empty;
                if (Client_ID != 0 && Client_ID != null)
                {
                    _wherecluase = _wherecluase + " and cbd.company_id=" + Client_ID;
                }
                if (Base_currency != "" && Base_currency != null && Base_currency != "null")
                {
                    _wherecluase = _wherecluase + " and base_currency_code ='" + Base_currency + "'";
                }
                else
                {
                    _wherecluase = _wherecluase + " limit 1";
                }
                if (perm_status == 0 && Collection_type_Id != 0 && Delivery_type_Id != 0) //added by vyankatesh
                {
                    _wherecluase = _wherecluase + " and cbd.collection_type_id = " + Collection_type_Id + " and cbd.delivery_type_id = " + Delivery_type_Id;
                }

                MySqlCommand cmd1 = new MySqlCommand("sp_select_company_bank_deatils");
                cmd1.CommandType = CommandType.StoredProcedure;
                cmd1.Parameters.AddWithValue("_wherecluase", _wherecluase);
                db = (DataTable)db_connection.ExecuteQueryDataTableProcedure(cmd1);
                _wherecluase = " 1=1";
                if (db.Rows.Count == 0)
                {
                    if (Client_ID != 0 && Client_ID != null)
                    {
                        _wherecluase = _wherecluase + " and cbd.company_id=" + Client_ID;
                    }
                    _wherecluase = _wherecluase + " limit 1";
                    cmd1 = new MySqlCommand("sp_select_company_bank_deatils");
                    cmd1.CommandType = CommandType.StoredProcedure;
                    cmd1.Parameters.AddWithValue("_wherecluase", _wherecluase);
                    db = (DataTable)db_connection.ExecuteQueryDataTableProcedure(cmd1);
                }
                //Digvijay chnages for Receipt changes.- Company image converted to base64
               // if ((HttpContext.Current.Request.UrlReferrer.AbsolutePath).Contains("receipt-summary") && db.Rows.Count > 0)
                if (db.Rows.Count > 0)
                {
                    db.Rows[0]["Image"] = ConvertImageLinkToBase64(Convert.ToString(db.Rows[0]["Image"]));
                }
                return db;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static int check_notification_perm(string _Customer_ID, int _Client_ID, int _Branch_ID, int _Notification_Moule_ID, int _SubNotification_ID, DateTime dt, int Custom_notification, int Sms_status, int email_status, int notification_status, string Function_Name, string notification_message, HttpContext context)
        {
            try
            {
                string custlist = _Customer_ID;
                string Title = "";
                string Message = "";
                DataTable dt1 = new DataTable();
                string whereclause = "";
                string Record_Insert_DateTime = Convert.ToString(gettime(_Client_ID, _Customer_ID, 0, context));
                DataTable dt2 = new DataTable();
               MySqlConnector.MySqlCommand cmd_notif = new MySqlConnector.MySqlCommand("check_notification_perm");
                cmd_notif.CommandType = CommandType.StoredProcedure;
                cmd_notif.Parameters.AddWithValue("_Client_ID", _Client_ID);
                if (_Branch_ID != 0)
                {
                    whereclause = whereclause + " and  notification_access_table.Branch_ID=" + _Branch_ID;
                }
                if (_Notification_Moule_ID != 0)
                {
                    whereclause = whereclause + " and  notification_access_table.Notifications_ID=" + _Notification_Moule_ID;
                }
                if (_SubNotification_ID != 0)
                {
                    whereclause = whereclause + " and  notification_access_table.Sub_Notification_ID=" + _SubNotification_ID;
                }
                cmd_notif.Parameters.AddWithValue("_whereclause", whereclause);
                dt2 = db_connection.ExecuteQueryDataTableProcedure(cmd_notif);
                cmd_notif.Dispose();
                if (dt2.Rows.Count > 0)
                {
                    Title = Convert.ToString(dt2.Rows[0]["notification_title"]);
                }
                else
                {
                    Title = "Hi [First_name] [Last_name],";
                }


                if (Convert.ToInt32(notification_status) == 0)
                {
                    Message = Convert.ToString(notification_message);

                    if (Custom_notification == 0)
                    {
                        int i = Check_notification_perm_scheduler(Title, Message, Convert.ToInt32(_Customer_ID), Convert.ToInt32(_Client_ID), Convert.ToInt32(_Branch_ID), dt, notification_status, Sms_status, email_status, custlist, "1", Convert.ToString(_SubNotification_ID));
                        Custom_notification = 1;
                    }
                    else
                    {
                        _ = Device_Notification(Title, Message, Convert.ToInt32(_Customer_ID), _Client_ID, Function_Name, Convert.ToString(_SubNotification_ID), context);
                    }
                }
                if (Convert.ToInt32(Sms_status) == 0)
                {
                    Message = Convert.ToString(notification_message);

                    if (Custom_notification == 0)
                    {
                        int i = Check_notification_perm_scheduler(Title, Message, Convert.ToInt32(_Customer_ID), Convert.ToInt32(_Client_ID), Convert.ToInt32(_Branch_ID), dt, notification_status, Sms_status, email_status, custlist, "1", Convert.ToString(_SubNotification_ID));
                        Custom_notification = 1;

                    }
                    else
                    {
                        _ = Sms_Notification(Title, Message, Convert.ToInt32(_Customer_ID), _Client_ID, Function_Name, Convert.ToString(_SubNotification_ID), context);
                    }
                }

                try
                {
                    if ((Convert.ToInt32(notification_status) == 0 || Convert.ToInt32(Sms_status) == 0) && Custom_notification == 1)
                    {
                        MySqlCommand push_notif = new MySqlCommand("sp_save_push_notification");
                        push_notif.CommandType = CommandType.StoredProcedure;
                        string notification_message_push = string.Empty; string notification_title_push = string.Empty;
                        notification_message_push = "<span class='cls-customer'><strong>" + dt2.Rows[0]["notification_msg"] + "</strong><span></span><br/></span>";
                        notification_title_push = "<span class='cls-customer'><strong>" + dt2.Rows[0]["notification_title"] + "</strong><span>";
                        push_notif.Parameters.AddWithValue("_notification_title", notification_title_push);
                        push_notif.Parameters.AddWithValue("_notification_msg", notification_message_push);
                        push_notif.Parameters.AddWithValue("_Record_Insert_Date", Record_Insert_DateTime);
                        push_notif.Parameters.AddWithValue("_customerId", Convert.ToInt32(_Customer_ID));
                        push_notif.Parameters.AddWithValue("_delete_status", "0");
                        push_notif.Parameters.AddWithValue("_Sub_Notification_ID", _SubNotification_ID);
                        push_notif.Parameters.AddWithValue("_Branch_ID", _Branch_ID);
                        push_notif.Parameters.AddWithValue("_Client_ID", Convert.ToInt32(_Client_ID));
                        push_notif.Parameters.AddWithValue("_Notification_Icon", dt2.Rows[0]["Notification_Icon"]);

                        int module_push = db_connection.ExecuteNonQueryProcedure(push_notif);
                        if (module_push > 0)
                        {
                            string Stattus = "<b>" + Convert.ToInt32(_Customer_ID) + "</b>" + " againt push notifications are saved successfully..</br>"; ;
                            //Activity = "<b>" + Username + "</b>" + " is on sign up.</br>";
                            int stattus = (int)CompanyInfo.InsertActivityLogDetails(Stattus, 0, 0, 0, Convert.ToInt32(_Customer_ID), "UpdateAddress", _Branch_ID, Convert.ToInt32(_Client_ID), "Push Notification", context);
                            //stattus = (string)CompanyInfo.InsertActivityLogDetailsasync("push module notification save successfully", 5, 0, 5, Convert.ToInt32(_Customer_ID), "push_notification", _Branch_ID, Convert.ToInt32(_Client_ID));
                        }

                    }
                }
                catch (Exception ex)
                {
                    //string stattus = (string)CompanyInfo(ex.ToString().Replace("\'", "\\'"), 0, "module_push_notification", 0, 0);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return 0;
        }

        public static int check_notification_perm(string _Customer_ID, int _Client_ID, int _Branch_ID, int _Notification_Moule_ID, int _SubNotification_ID, DateTime dt, int Custom_notification, int Sms_status, int email_status, int notification_status, string Function_Name, string notification_message, int transaction_id)

        {
            HttpContext context = null; 
            try
            {
                string test_act = "step1";
                string custlist = _Customer_ID;
                string Title = "";
                string Message = "";
                DataTable dt1 = new DataTable();
                string whereclause = "";
                string Record_Insert_DateTime = Convert.ToString(gettime(_Client_ID, _Customer_ID, 0, context));
                DataTable dt2 = new DataTable();
                test_act = test_act + "step2";
                MySqlCommand cmd_notif = new MySqlCommand("check_notification_perm");
                cmd_notif.CommandType = CommandType.StoredProcedure;
                cmd_notif.Parameters.AddWithValue("_Client_ID", _Client_ID);
                if (_Branch_ID != 0)
                {
                    whereclause = whereclause + " and notification_access_table.Branch_ID=" + _Branch_ID;
                }
                else
                {
                    whereclause = whereclause + " and notification_access_table.Branch_ID=2";
                }
                if (_Notification_Moule_ID != 0)
                {
                    whereclause = whereclause + " and notification_access_table.Notifications_ID=" + _Notification_Moule_ID;
                }
                if (_SubNotification_ID != 0)
                {
                    whereclause = whereclause + " and notification_access_table.Sub_Notification_ID=" + _SubNotification_ID;
                }
                test_act = test_act + "step3" + whereclause;
                cmd_notif.Parameters.AddWithValue("_whereclause", whereclause);
                dt2 = db_connection.ExecuteQueryDataTableProcedure(cmd_notif);
                cmd_notif.Dispose();
                if (dt2.Rows.Count > 0)
                {
                    test_act = test_act + "step4" + dt2.Rows[0]["notification_title"];
                    Title = Convert.ToString(dt2.Rows[0]["notification_title"]);
                    notification_message = Convert.ToString(dt2.Rows[0]["notification_msg"]);
                }
                else
                {
                    Title = "Hi [First_name] [Last_name],";
                }

                InsertNotificationLogDetails(test_act, 0, 0, Convert.ToInt32(_Customer_ID), "check_notifcation_perm", 0, 0, context);

                if (transaction_id != 0)
                {
                    MySqlCommand cmd_notif1 = new MySqlCommand("Default_SP");
                    cmd_notif1.CommandType = CommandType.StoredProcedure;
                    cmd_notif1.Parameters.AddWithValue("_Query", "select AmountInPKR,FromCurrency_Code,Currency_Code, AmountInGBP as Base_amt,ReferenceNo as Txn_ref from transaction_table where transaction_id=" + transaction_id);
                    DataTable dt2_notif = db_connection.ExecuteQueryDataTableProcedure(cmd_notif1);
                    cmd_notif1.Dispose();
                    if (dt2_notif.Rows.Count > 0)
                    {
                        if (Title.Contains("[Txn_Ref]"))
                        {
                            Title = Title.Replace("[Txn_Ref]", Convert.ToString(dt2_notif.Rows[0]["Txn_ref"]));
                        }
                        if (Title.Contains("[Base_Amt]"))
                        {
                            Title = Title.Replace("[Base_Amt]", Convert.ToString(dt2_notif.Rows[0]["Base_amt"]));
                        }
                        if (Title.Contains("[Base_Cur]"))
                        {
                            Title = Title.Replace("[Base_Cur]", Convert.ToString(dt2_notif.Rows[0]["FromCurrency_Code"]));
                        }
                        if (Title.Contains("[Payout_Cur]"))
                        {
                            Title = Title.Replace("[Payout_Cur]", Convert.ToString(dt2_notif.Rows[0]["Currency_Code"]));
                        }
                        if (Title.Contains("[Payout_Amt]"))
                        {
                            Title = Title.Replace("[Payout_Amt]", Convert.ToString(dt2_notif.Rows[0]["AmountInPKR"]));
                        }

                        if (notification_message != "")
                        {
                            if (notification_message.Contains("[Txn_Ref]"))
                            {
                                notification_message = notification_message.Replace("[Txn_Ref]", Convert.ToString(dt2_notif.Rows[0]["Txn_ref"]));
                            }
                            if (notification_message.Contains("[Base_Amt]"))
                            {
                                notification_message = notification_message.Replace("[Base_Amt]", Convert.ToString(dt2_notif.Rows[0]["Base_amt"]));
                            }
                            if (notification_message.Contains("[Base_Cur]"))
                            {
                                notification_message = notification_message.Replace("[Base_Cur]", Convert.ToString(dt2_notif.Rows[0]["FromCurrency_Code"]));
                            }
                            if (notification_message.Contains("[Payout_Cur]"))
                            {
                                notification_message = notification_message.Replace("[Payout_Cur]", Convert.ToString(dt2_notif.Rows[0]["Currency_Code"]));
                            }
                            if (notification_message.Contains("[Payout_Amt]"))
                            {
                                notification_message = notification_message.Replace("[Payout_Amt]", Convert.ToString(dt2_notif.Rows[0]["AmountInPKR"]));
                            }

                        }
                    }
                }

                if (Convert.ToInt32(notification_status) == 0)
                {
                    Message = Convert.ToString(notification_message);

                    if (Custom_notification == 0)
                    {
                        int i = Check_notification_perm_scheduler(Title, Message, Convert.ToInt32(_Customer_ID), Convert.ToInt32(_Client_ID), Convert.ToInt32(_Branch_ID), dt, notification_status, Sms_status, email_status, custlist, "1", Convert.ToString(_SubNotification_ID));
                        Custom_notification = 1;
                    }
                    else
                    {
                        _ = Device_Notification(Title, Message, Convert.ToInt32(_Customer_ID), _Client_ID, Function_Name, Convert.ToString(_SubNotification_ID), context);
                    }
                }
                if (Convert.ToInt32(Sms_status) == 0)
                {
                    Message = Convert.ToString(notification_message);

                    if (Custom_notification == 0)
                    {
                        int i = Check_notification_perm_scheduler(Title, Message, Convert.ToInt32(_Customer_ID), Convert.ToInt32(_Client_ID), Convert.ToInt32(_Branch_ID), dt, notification_status, Sms_status, email_status, custlist, "1", Convert.ToString(_SubNotification_ID));
                        Custom_notification = 1;

                    }
                    else
                    {
                        _ = Sms_Notification(Title, Message, Convert.ToInt32(_Customer_ID), _Client_ID, Function_Name, Convert.ToString(_SubNotification_ID), context);
                    }
                }

                try
                {
                    if ((Convert.ToInt32(notification_status) == 0 || Convert.ToInt32(Sms_status) == 0) && Custom_notification == 1)
                    {
                        MySqlCommand push_notif = new MySqlCommand("sp_save_push_notification");
                        push_notif.CommandType = CommandType.StoredProcedure;
                        string notification_message_push = string.Empty; string notification_title_push = string.Empty;
                        notification_message_push = "<span class='cls-customer'><strong>" + notification_message + "</strong><span></span><br/></span>";
                        notification_title_push = "<span class='cls-customer'><strong>" + dt2.Rows[0]["notification_title"] + "</strong><span>";
                        push_notif.Parameters.AddWithValue("_notification_title", notification_title_push);
                        push_notif.Parameters.AddWithValue("_notification_msg", notification_message_push);
                        push_notif.Parameters.AddWithValue("_Record_Insert_Date", Record_Insert_DateTime);
                        push_notif.Parameters.AddWithValue("_customerId", Convert.ToInt32(_Customer_ID));
                        push_notif.Parameters.AddWithValue("_delete_status", "0");
                        push_notif.Parameters.AddWithValue("_Sub_Notification_ID", _SubNotification_ID);
                        push_notif.Parameters.AddWithValue("_Branch_ID", _Branch_ID);
                        push_notif.Parameters.AddWithValue("_Client_ID", Convert.ToInt32(_Client_ID));
                        push_notif.Parameters.AddWithValue("_Notification_Icon", dt2.Rows[0]["Notification_Icon"]);

                        int module_push = db_connection.ExecuteNonQueryProcedure(push_notif);
                        if (module_push > 0)
                        {
                            string Stattus = "<b>" + Convert.ToInt32(_Customer_ID) + "</b>" + " againt push notifications are saved successfully..</br>"; ;
                            //Activity = "<b>" + Username + "</b>" + " is on sign up.</br>";
                            int stattus = (int)CompanyInfo.InsertActivityLogDetails(Stattus, 0, 0, 0, Convert.ToInt32(_Customer_ID), "UpdateAddress", _Branch_ID, Convert.ToInt32(_Client_ID), "Push Notification", context);
                            //stattus = (string)CompanyInfo.InsertActivityLogDetailsasync("push module notification save successfully", 5, 0, 5, Convert.ToInt32(_Customer_ID), "push_notification", _Branch_ID, Convert.ToInt32(_Client_ID));
                        }

                    }
                }
                catch (Exception ex)
                {
                    //string stattus = (string)CompanyInfo(ex.ToString().Replace("\'", "\\'"), 0, "module_push_notification", 0, 0);
                }

            }
            catch (Exception ex)
            {
                throw ex;

            }
            return 0;
        }
        public static int Sms_Notification(string Title, string Message, int Customer_ID, int Client_ID, string Function_name, string Sub_notifcation, HttpContext context)
        {
            //  await Task.Delay(10);
            string test_act = "";

            try
            {
                DataTable dtc = new DataTable();
                string private_key = "";
                string _whereclause = "";
                string _join = "";
                String dt = gettime(1, context);

               MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("Get_Customer_mobile");
                _cmd.CommandType = CommandType.StoredProcedure;
                //_cmd.Parameters.AddWithValue("_Customer_ID", Customer_ID);
                // _cmd.Parameters.AddWithValue("_whereclause", "  and cr.Customer_ID = " + Customer_ID + " ");

                if (Convert.ToInt32(Sub_notifcation) == 21 || Convert.ToInt32(Sub_notifcation) == 22 || Convert.ToInt32(Sub_notifcation) == 23 || Convert.ToInt32(Sub_notifcation) == 24 || Convert.ToInt32(Sub_notifcation) == 25 || Convert.ToInt32(Sub_notifcation) == 26 || Convert.ToInt32(Sub_notifcation) == 28 || Convert.ToInt32(Sub_notifcation) == 30) //Rate Notification     

                {
                    _join = " inner JOIN app_notification_config anc ON cr.Customer_ID = anc.Customer_Id ";
                    _whereclause = " and anc.Module_Id = 6 and anc.Delete_Status = 0 and FIND_IN_SET('" + Convert.ToInt32(Sub_notifcation) + "', To_check) > 0";
                }
                _whereclause += "  and cr.Customer_ID = " + Customer_ID + " ";
                _cmd.Parameters.AddWithValue("_whereclause", _whereclause);
                _cmd.Parameters.AddWithValue("_join", _join);
                DataTable token_dt = db_connection.ExecuteQueryDataTableProcedure(_cmd);
                test_act += "where_clause " + _whereclause + "joins" + _join;
                if (token_dt.Rows.Count > 0)
                {
                    string username = ""; // Replace with your actual ClickSend username
                    string password = "";
                    string apiUrl = "";

                    MySqlConnector.MySqlCommand cmd = new MySqlConnector.MySqlCommand("GetAPIDetails");
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("_API_ID", 4);//Click Send API ID
                    cmd.Parameters.AddWithValue("_Client_ID", 1);
                    cmd.Parameters.AddWithValue("_status", 0);
                    DataTable dtt = db_connection.ExecuteQueryDataTableProcedure(cmd);

                    if (dtt.Rows.Count > 0)
                    {
                        apiUrl = Convert.ToString(dtt.Rows[0]["API_URL"]);
                        username = Convert.ToString(dtt.Rows[0]["UserName"]);
                        password = Convert.ToString(dtt.Rows[0]["ProfileID"]);
                    }
                    test_act += "step 1";
                    HttpClient httpClient = new HttpClient();
                    string authHeaderValue = Convert.ToBase64String(Encoding.ASCII.GetBytes(username + ":" + password));
                    httpClient.DefaultRequestHeaders.Add("Authorization", "Basic " + authHeaderValue);
                    DataTable xmldataTable = new DataTable();
                    private_key = Convert.ToString(token_dt.Rows[0]["RootURL"]) + "assets/Notification_key/private_key.json";
                    string xmlFilePath = Convert.ToString(token_dt.Rows[0]["RootURL"]) + "assets/Notification_key/ReplaceLetters.xml";
                    if (File.Exists(xmlFilePath))
                    {

                        test_act += "step 2";

                        // Load the XML file content
                        XmlDocument xmlDocument = new XmlDocument();
                        xmlDocument.Load(xmlFilePath);

                        // Get the root element
                        XmlElement root = xmlDocument.DocumentElement;

                        // Add columns with fixed names "variable_column_name" and "variable_name"
                        xmldataTable.Columns.Add("variable_name", typeof(string));
                        xmldataTable.Columns.Add("variable_column_name", typeof(string));


                        // Populate DataTable with data from XML
                        foreach (XmlNode rowNode in root.SelectNodes("row"))
                        {
                            string variableName = rowNode.SelectSingleNode("variable_name").InnerText;
                            string variableColumnName = rowNode.SelectSingleNode("variable_column_name").InnerText;


                            DataRow newRow = xmldataTable.NewRow();

                            newRow["variable_name"] = variableName;
                            newRow["variable_column_name"] = variableColumnName;
                            xmldataTable.Rows.Add(newRow);
                        }
                    }

                    List<string> columnValues = new List<string>();
                    // List<FirebaseAdmin.Messaging.Message> messages = new List<FirebaseAdmin.Messaging.Message>();

                    if (Title == "" || Title == null || Message == "" || Message == null)//if Title or the message is empty
                    {
                        test_act += "step 3";

                        return 0;
                    }
                    var messages = new List<object>();
                    foreach (DataRow row in token_dt.Rows)
                    {
                        string phoneNumber = row["mobile"].ToString(); //string title12 = ConvertMessage(Convert.ToString(dtnotifi.Rows[i]["notification_title"]), xmldataTable, row);
                        string title = ConvertMessage(Title, xmldataTable, row);
                        string message1 = ConvertMessage(Message, xmldataTable, row);
                        string From_number = row["Company_mobile"].ToString();


                        if (phoneNumber != "" && phoneNumber != null && From_number != "" && From_number != null)
                        {
                            messages.Add(new
                            {
                                source = "",
                                from = From_number,
                                body = message1,
                                to = phoneNumber
                            });
                        }



                        string payload = $@"{{ ""messages"": {Newtonsoft.Json.JsonConvert.SerializeObject(messages)} }}";

                        // Set up the HTTP request headers

                        if (apiUrl != "")
                        {
                            HttpResponseMessage response = httpClient.PostAsync(apiUrl, new StringContent(payload, Encoding.UTF8, "application/json")).Result;
                            // Read the response synchronously
                            string responseContent = response.Content.ReadAsStringAsync().Result;
                        }
                        string Activity = "";

                        Activity = Function_name;
                        test_act += "step 4";

                        InsertNotificationLogDetails(Activity, 0, 0, Customer_ID, "Sms_Notification", 0, 0, context);

                    }
                }
                return 0;
            }
            catch (Exception ex)
            {
                Model.ErrorLog objError = new Model.ErrorLog();
                objError.User = new Model.User();

                objError.Error = ex.ToString();
                objError.Date = DateTime.Now;
                objError.User_ID = 1;
                objError.Id = 1;
                objError.Function_Name = "SMS_Notification (" + Function_name + ")";
                Service.srvErrorLog srvError = new Service.srvErrorLog();
                srvError.Create(objError, context);
                //_ = CompanyInfo.InsertActivityLogDetailsasync(ex.ToString(), 0, 0, 0, 0, "SMS_Notification", 0, 1, "");

                return 0;
            }
            finally
            {
                Model.ActivityLogTracker _objActivityLog = new Model.ActivityLogTracker();
                _objActivityLog.Activity = test_act;
                _objActivityLog.FunctionName = "SMS_Notification (" + Function_name + ")"; ;
                _objActivityLog.Transaction_ID = 0;
                _objActivityLog.WhoAcessed = 0;
                _objActivityLog.Branch_ID = 2;
                _objActivityLog.Client_ID = 1;
                _objActivityLog.Customer_ID = Convert.ToInt32(Customer_ID);
                _objActivityLog.RecordInsertDate = DateTime.Now;

                Service.srvActivityLogTracker srvActivityLog = new Service.srvActivityLogTracker();
                int _i = srvActivityLog.Create(_objActivityLog, context);
                //_ = CompanyInfo.InsertActivityLogDetailsasync(test_act.ToString(), 0, 0, 0, 0, "SMS_Notification", 0, 1, "");

            }
        }

       
        public static string InsertNotificationLogDetails(string Activity, int Transaction_ID, int user_id, int cust_id, string Function_name, int Branch_ID, int Client_ID , HttpContext context)
        {
            string strResult = string.Empty;
            string Record_Insert_DateTime = (string)gettime(Client_ID, context);
            MySqlConnector.MySqlCommand cmd = new MySqlConnector.MySqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "save_notification_log";
            cmd.Parameters.AddWithValue("_activity", Activity);
            cmd.Parameters.AddWithValue("_recordInsertDate", Record_Insert_DateTime);
            cmd.Parameters.AddWithValue("_deleteStatus", 0);
            cmd.Parameters.AddWithValue("_transactionId", Transaction_ID);
            cmd.Parameters.AddWithValue("_userId", user_id);
            cmd.Parameters.AddWithValue("_custId", cust_id);
            cmd.Parameters.AddWithValue("_functionName", Function_name);
            cmd.Parameters.AddWithValue("_branchId", Branch_ID);
            cmd.Parameters.AddWithValue("_clientId", Client_ID);
            string res = Convert.ToString(db_connection.ExecuteNonQueryProcedure(cmd));
            return res;
        }
        public static int Device_Notification(string Title, string Message, int Customer_ID, int Client_ID, string Function_name, string Sub_notifcation, HttpContext context)
        {
            //await Task.Delay(10);
            try
            {
                DataTable dtc = new DataTable();
                string private_key = "";
                string _whereclause = "";
                string _join = "";
                int badges = 0;
                String dt = gettime(1 , context);
                MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("Get_device_token");
                _cmd.CommandType = CommandType.StoredProcedure;
                //_cmd.Parameters.AddWithValue("_Customer_ID", Customer_ID);
                //_cmd.Parameters.AddWithValue("_whereclause", "  and cr.Customer_ID = " + Customer_ID + " ");

                if (Convert.ToInt32(Sub_notifcation) == 21 || Convert.ToInt32(Sub_notifcation) == 22 || Convert.ToInt32(Sub_notifcation) == 23 || Convert.ToInt32(Sub_notifcation) == 24 || Convert.ToInt32(Sub_notifcation) == 25 || Convert.ToInt32(Sub_notifcation) == 26 || Convert.ToInt32(Sub_notifcation) == 28 || Convert.ToInt32(Sub_notifcation) == 30) //Rate Notification     
                {
                    _join = " inner JOIN app_notification_config anc ON cr.Customer_ID = anc.Customer_Id ";
                    _whereclause = " and anc.Module_Id = 6 and FIND_IN_SET('" + Convert.ToInt32(Sub_notifcation) + "', To_check) > 0";
                }
                _whereclause += "  and cr.Customer_ID = " + Customer_ID + " ";

                _cmd.Parameters.AddWithValue("_whereclause", _whereclause);
                _cmd.Parameters.AddWithValue("_join", _join);
                DataTable token_dt = db_connection.ExecuteQueryDataTableProcedure(_cmd);
                MySqlConnector.MySqlCommand _cmd1 = new MySqlConnector.MySqlCommand("Default_SP");
                _cmd1.CommandType = CommandType.StoredProcedure;
                _cmd1.Parameters.AddWithValue("_Query", "SELECT count(Notification_Id) as badges FROM module_push_notification where  Cust_Id = " + Customer_ID + " and Read_Count_Customer = 1; ");
                DataTable dtbadge = db_connection.ExecuteQueryDataTableProcedure(_cmd1);
                if (dtbadge.Rows.Count > 0)
                {
                    badges = Convert.ToInt32(dtbadge.Rows[0]["badges"]);
                }
                if (token_dt.Rows.Count > 0)
                {
                    DataTable xmldataTable = new DataTable();
                    private_key = Convert.ToString(token_dt.Rows[0]["RootURL"]) + "assets/Notification_key/private_key.json";
                    string xmlFilePath = Convert.ToString(token_dt.Rows[0]["RootURL"]) + "assets/Notification_key/ReplaceLetters.xml";
                    if (File.Exists(xmlFilePath))
                    {


                        // Load the XML file content
                        XmlDocument xmlDocument = new XmlDocument();
                        xmlDocument.Load(xmlFilePath);

                        // Get the root element
                        XmlElement root = xmlDocument.DocumentElement;

                        // Add columns with fixed names "variable_column_name" and "variable_name"
                        xmldataTable.Columns.Add("variable_name", typeof(string));
                        xmldataTable.Columns.Add("variable_column_name", typeof(string));


                        // Populate DataTable with data from XML
                        foreach (XmlNode rowNode in root.SelectNodes("row"))
                        {
                            string variableName = rowNode.SelectSingleNode("variable_name").InnerText;
                            string variableColumnName = rowNode.SelectSingleNode("variable_column_name").InnerText;


                            DataRow newRow = xmldataTable.NewRow();

                            newRow["variable_name"] = variableName;
                            newRow["variable_column_name"] = variableColumnName;
                            xmldataTable.Rows.Add(newRow);
                        }
                    }

                    List<string> columnValues = new List<string>();
                    List<FirebaseAdmin.Messaging.Message> messages = new List<FirebaseAdmin.Messaging.Message>();

                    if (Title == "" || Title == null || Message == "" || Message == null)//if Title or the message is empty
                    {
                        return 0;
                    }
                    foreach (DataRow row in token_dt.Rows)
                    {
                        string token = row["Token"].ToString();

                        string title = ConvertMessage(Title, xmldataTable, row);
                        string message1 = ConvertMessage(Message, xmldataTable, row);

                        var fcmMessage = new FirebaseAdmin.Messaging.Message
                        {
                            Token = token,
                            Data = new Dictionary<string, string>()
                             {
                            {    "Badge", Convert.ToString(badges) },
                            },
                            Notification = new Notification
                            {
                                Title = title,
                                Body = message1
                            },
                            Apns = new ApnsConfig
                            {
                                Aps = new Aps
                                {
                                    //Sound = "mySound",
                                    Badge = badges
                                }
                            }
                        };

                        messages.Add(fcmMessage);
                    }

                    var app = FirebaseApp.DefaultInstance;
                    if (FirebaseApp.DefaultInstance == null)
                    {
                        FirebaseApp.Create(
                         new AppOptions()
                         {
                             Credential = GoogleCredential.FromFile(private_key)
                         }
                     );
                    }


                    var response = FirebaseMessaging.DefaultInstance.SendAllAsync(messages).Result;
                    //string failedTOKENS = "";
                    //for (int j = 0; j < response.Responses.Count; j++)
                    //{
                    //    var result = response.Responses[j];
                    //    if (!result.IsSuccess)
                    //    {
                    //        string failedToken = messages[j].Token;
                    //        failedTOKENS += "," + failedToken;
                    //    }
                    //}

                    string Activity = Function_name;
                    InsertNotificationLogDetails(Activity, 0, 0, Customer_ID, "Device_Notification", 0, 0, context);







                }
                return 0;
            }
            catch (Exception ex)
            {
                _ = CompanyInfo.InsertActivityLogDetailsasync(ex.ToString(), 0, 0, 0, 0, "Device_Notification", 0, 1, "",context);

                return 0;
            }
        }

        public static DataTable set_notification_data(int submodule_id)
        {

            string filePath = "";
           MySqlConnector.MySqlCommand cmd = new MySqlConnector.MySqlCommand("Get_CompanyInfo");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("_Client_ID", 1);
            cmd.Parameters.AddWithValue("_Customer_ID", "");
            cmd.Parameters.AddWithValue("_SecurityKey", CompanyInfo.SecurityKey());
            DataTable dt_comp = (DataTable)db_connection.ExecuteQueryDataTableProcedure(cmd);
            if (dt_comp.Rows.Count > 0)
            {
                filePath = Convert.ToString(dt_comp.Rows[0]["RootURL"]); // Specify your file path
                filePath = filePath + "/assets/Notification_access/Notification_webbranch.xml";
            }

            DataTable dataTable = new DataTable();
            DataTable result = new DataTable();
            DataSet dataSet = new DataSet();
            dataSet.ReadXml(filePath);
            dataTable = dataSet.Tables[0];
            
            if (dataTable.Rows.Count > 0)
            { 
                var activeRows = dataTable.AsEnumerable().Where(row => (string)row["Sub_Notification_ID"] == Convert.ToString(submodule_id));

                result = activeRows.CopyToDataTable();
                dataTable.Clear();
            }
            return result;
        }

       
        public static string ConvertMessage(String str, DataTable xmldatable, DataRow dtr)
        {

            foreach (DataRow row in xmldatable.Rows)
            {
                string variableName = row["variable_name"].ToString();
                string variableValue = row["variable_column_name"].ToString();
                //str = str.Replace(variableName, variableValue);
                str = str
                .Replace(variableName, Convert.ToString(dtr[variableValue]));


            }
            //return Regex.Unescape(str);
            return str;

        }

        public static int Check_notification_perm_scheduler(string Title, string Message,
          int Customer_ID, int Client_ID, int Branch_ID, DateTime dt, int notif_status, int sms_status, int email_status, string custlist, string all_flag, string Sub_notifcation)
        {
            MySqlConnector.MySqlCommand cmd = new MySqlConnector.MySqlCommand("sp_save_custom_notification");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("_Noti_title", Title);
            cmd.Parameters.AddWithValue("_Noti_subtext", Message);
            cmd.Parameters.AddWithValue("_appNotification", notif_status);
            cmd.Parameters.AddWithValue("_messageNotificaiton", sms_status);
            cmd.Parameters.AddWithValue("_emailNotification", email_status);
            cmd.Parameters.AddWithValue("_Noti_date", dt);
            cmd.Parameters.AddWithValue("_Client_ID", Client_ID);
            cmd.Parameters.AddWithValue("_Branch_ID", Branch_ID);
            cmd.Parameters.AddWithValue("_Delete_status", "0");
            cmd.Parameters.AddWithValue("_all_flag", all_flag);
            cmd.Parameters.AddWithValue("_cust_list", custlist);
            cmd.Parameters.AddWithValue("custom_flag", 1);
            cmd.Parameters.AddWithValue("SubNotification_ID", Sub_notifcation);
            int i = db_connection.ExecuteNonQueryProcedure(cmd);
            return i;
        }

        public static string gettime(int Client_ID, HttpContext contaxt)
        {
            DateTime d = DateTime.Now;
            try
            {
                string strResult = string.Empty;
                DataTable dt = (DataTable)get(Client_ID, contaxt);
                if (dt.Rows.Count > 0)
                {
                    string timezone = Convert.ToString(dt.Rows[0]["BaseCurrency_Timezone"]);
                    if (timezone != "" && timezone != null)
                    {
                        var britishZone = TimeZoneInfo.FindSystemTimeZoneById("" + timezone + "");
                        d = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.Local, britishZone);
                    }
                }
                return d.ToString("yyyy-MM-dd HH:mm:ss");
            }
            catch (Exception ex)
            {
                return d.ToString("yyyy-MM-dd HH:mm:ss");
                throw ex;
            }
        }

       /* public static async Task<string> gettimeAsync(int Client_ID, HttpContext context)
        {
            DateTime d = DateTime.Now;
            try
            {
                string strResult = string.Empty;

          
                DataTable dt = (DataTable)await getAsync(Client_ID, context);

                if (dt.Rows.Count > 0)
                {
                    string timezone = Convert.ToString(dt.Rows[0]["BaseCurrency_Timezone"]);
                    if (!string.IsNullOrEmpty(timezone))
                    {
                        var britishZone = TimeZoneInfo.FindSystemTimeZoneById(timezone);
                        d = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.Local, britishZone);
                    }
                }

                return d.ToString("yyyy-MM-dd HH:mm:ss");
            }
            catch (Exception ex)
            {
                // Log the exception instead of throwing (to prevent hiding the original error)
                Console.WriteLine($"Error in gettimeAsync: {ex.Message}");
                return d.ToString("yyyy-MM-dd HH:mm:ss");
            }
        }*/


        public static string gettime(int Client_ID, string Cus_ID, int Country_ID, HttpContext contaxt)
        {
            DataTable dt = null;
            DateTime d = DateTime.Now;
            string timezone = "";
            int Customer_ID = Convert.ToInt32(Cus_ID);
            try
            {
                string strResult = string.Empty;
                if (Customer_ID != null && Customer_ID != 0)
                {
                    DataTable dt_basetimezone = (DataTable)get_basetimezone(Country_ID, Customer_ID);
                    if (dt_basetimezone.Rows.Count != 0)
                    {
                        timezone = Convert.ToString(dt_basetimezone.Rows[0]["Timezone"]);
                        if (timezone != "" && timezone != null)
                        {
                            var britishZone = TimeZoneInfo.FindSystemTimeZoneById("" + timezone + "");
                            d = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.Local, britishZone);
                        }
                    }
                    else
                    {
                        dt = (DataTable)get(Client_ID, contaxt);
                    }
                }
                else if (Country_ID != null && Country_ID != 0)
                {
                    DataTable dt_basetimezone = (DataTable)get_basetimezone(Country_ID, Customer_ID);
                    timezone = Convert.ToString(dt_basetimezone.Rows[0]["Timezone"]);
                    if (timezone != "" && timezone != null)
                    {
                        var britishZone = TimeZoneInfo.FindSystemTimeZoneById("" + timezone + "");
                        d = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.Local, britishZone);
                    }
                }
                else
                {
                    dt = (DataTable)get(Client_ID, contaxt);
                }
                if (dt != null)
                {
                    if (dt.Rows.Count > 0)
                    {
                        timezone = Convert.ToString(dt.Rows[0]["BaseCurrency_Timezone"]);
                        if (timezone != "" && timezone != null)
                        {
                            var britishZone = TimeZoneInfo.FindSystemTimeZoneById("" + timezone + "");
                            d = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.Local, britishZone);
                        }
                    }
                }
                string Activities = "DateTime:" + d.ToString("yyyy-MM-dd HH:mm:ss") + " with Time Zone : " + timezone + " for client_id: " + Client_ID + " country id : " + Country_ID + "  and customerID : " + Customer_ID;
                //  int stattus = (int)CompanyInfo.InsertActivityLogDetails_imezone(Activities,0, 0,0, Convert.ToInt32(Customer_ID), "timezone_save", 2, Convert.ToInt32(Client_ID), "timezone save",Country_ID);
                return d.ToString("yyyy-MM-dd HH:mm:ss");
            }
            catch (Exception ex)
            {
                return d.ToString("yyyy-MM-dd HH:mm:ss");
                throw ex;
            }
        }
        public static DataTable get_basetimezone(int Country_ID, int Customer_ID)
        {
            DataTable dt_basetimezone = new DataTable();
            MySqlConnector.MySqlCommand cmd = new MySqlConnector.MySqlCommand("sp_select_timezone");
            cmd.CommandType = CommandType.StoredProcedure;
            string whereclause = "";
            if (Customer_ID != null && Customer_ID != 0)
            {
                whereclause = " cc.Customer_ID=" + Customer_ID;
            }

            else if (Country_ID != null && Country_ID != 0)
            {
                whereclause = " aa.country_id=" + Country_ID;
            }

            cmd.Parameters.AddWithValue("_whereclause", whereclause);

            dt_basetimezone = (DataTable)db_connection.ExecuteQueryDataTableProcedure(cmd);
            cmd.Dispose();
            return dt_basetimezone;
        }

        public static string ConvertImageLinkToBase64(string imageUrl)
        {
            try
            {
                using (var webClient = new WebClient())
                {
                    byte[] imageData = webClient.DownloadData(imageUrl);
                    string base64String = Convert.ToBase64String(imageData);
                    return ("data:image/png;base64," + base64String);
                }
            }
            catch { return ""; }
        }

        public static string GetRawTarget(this HttpRequest request)
        {
            var httpRequestFeature = request.HttpContext.Features.Get<IHttpRequestFeature>();
            return httpRequestFeature.RawTarget;
        }

        public static DataTable get(int Client_ID, HttpContext context)
        {
            
            DataTable dt = new DataTable();
            int where_cust = 0;

            //var context = HttpContext.Current;
            string checklink = "";
            try
            {
                int Customer_ID = 0;
                try
                {                    
                    Customer_ID = Convert.ToInt32(CompanyInfo.getSessionHttpVal("Customer_ID", context)) ;  //Convert.ToInt32(context.Session["Customer_ID"]);                   
                }
                catch
                {

                }
                
                string path_query = CompanyInfo.path_query(context);
                

                checklink = path_query; // context.Request.Url.PathAndQuery.ToString();
                if (!checklink.Contains("api/CompanyInfo/Info") && !checklink.Contains("api/LoginApi/CheckLogin") && !checklink.Contains("/api/Register/check_Email_Is_Exist") &&
                    !checklink.Contains("/api/LoginApi/Get_Permission") && !checklink.Contains("/api/send/CheckPerm") && !checklink.Contains("/api/Register/validate_password") &&
                    !checklink.Contains("/api/Register/Select_Title") && !checklink.Contains("/api/Register/Select_Heard_From") && !checklink.Contains("/api/Register/Select_EmployementStatus") &&
                    !checklink.Contains("/api/Register/Select_City") && !checklink.Contains("/api/Register/Select_Country") && !checklink.Contains("/api/Register/Select_Profession") &&
                    !checklink.Contains("/api/Register/Select_AllCountries") && !checklink.Contains("/api/Register/RegisterValidations") && !checklink.Contains("/api/Register/Create_Incomplete_Registration") &&
                    !checklink.Contains("/api/Register/CreateCustomer") && !checklink.Contains("/api/Send/Save_TransactionAudit") && !checklink.Contains("api/Register/secure_Account") &&
                    !checklink.Contains("/api/Password/send") && !checklink.Contains("/api/Password/forgotpassword") && !checklink.Contains("/api/Register/Select_base_Country_Master") &&
                    !checklink.Contains("/api/customer/search_address") && !checklink.Contains("/api/Rate/Getdata_Calculator") && !checklink.Contains("/api/Register/email_flag") &&
                    !checklink.Contains("/api/Rate/GetRates_Calculator") && !checklink.Contains("/api/Register/GetReferrerName") && !checklink.Contains("/api/Register/Select_provience") &&
                    !checklink.Contains("/api/Register/Secure_Step2") && !checklink.Contains("/token") 
                    && ( !checklink.Contains("/api/UserLogin/CheckLogin") && Customer_ID != 0 )
                    )
                {
                    if (Customer_ID != null && Customer_ID != 0)
                    {
                        where_cust = Customer_ID;
                    }
                    else
                    {
                        return dt;
                    }
                }
            }
            catch
            {



            }

            MySqlConnector.MySqlCommand cmd = new MySqlConnector.MySqlCommand("Get_CompanyInfo");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("_Client_ID", Client_ID);
            cmd.Parameters.AddWithValue("_Customer_ID", where_cust);
            cmd.Parameters.AddWithValue("_SecurityKey", SecurityKey());
            dt = (DataTable)db_connection.ExecuteQueryDataTableProcedure(cmd);
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    if (row["Image"] != DBNull.Value && row["Image"].ToString() != "")
                    {
                        string image_link = row["Image"].ToString();

                        string base64str = ConvertImageLinkToBase64(image_link);
                        row["Image"] = base64str;
                    }
                }
            }
            cmd.Dispose();
            if (dt.Rows.Count > 0)
            {
                if ((dt.Rows[0]["Security_Flag"]).ToString() == "0" && (dt.Rows[0]["Security_Flag"]).ToString() != null && (dt.Rows[0]["Security_Flag"]).ToString() != "")
                {
                    //Model.ActivityLogTracker _objActivityLog = new Model.ActivityLogTracker();
                    //_objActivityLog.Activity = "Automictic logout  App Side.<br/>Details: Security_Flag : " + dt.Rows[0]["Security_Flag"] + " Tab_Cust_Id : " + dt.Rows[0]["Customer_ID"] + " Session_Cust_Id : " + where_cust + " Session_Client_ID : " + Client_ID + " ";
                    //_objActivityLog.FunctionName = "Api : CompanyInfo.cs : Function Nm : Get();";
                    //_objActivityLog.Transaction_ID = 0;
                    //_objActivityLog.WhoAcessed = 1;
                    //_objActivityLog.Branch_ID = 1;
                    //_objActivityLog.Client_ID = Client_ID;

                    //Service.srvActivityLogTracker srvActivityLog = new Service.srvActivityLogTracker();
                    //int _i = srvActivityLog.Create(_objActivityLog);
                    string Activity = "Automictic logout  App Side.<br/>Details: Security_Flag : " + dt.Rows[0]["Security_Flag"] + " Tab_Cust_Id : " + dt.Rows[0]["Customer_ID"] + " Session_Cust_Id : " + where_cust + " Session_Client_ID : " + Client_ID + " ";
                    int cusid = 0;
                    if (Convert.ToString(dt.Rows[0]["Customer_ID"]) != null && Convert.ToString(dt.Rows[0]["Customer_ID"]) != "")
                        cusid = Convert.ToInt32(dt.Rows[0]["Customer_ID"]);
                    int stattus = (int)CompanyInfo.InsertActivityLogDetailsSecurity(Activity, Convert.ToInt32(0), 0, Convert.ToInt32(0), cusid, "companyinfo", Convert.ToInt32(0), Convert.ToInt32(0), "companyinfo-get", 0, context);
                    // context.Session["Security_Flag"] = false;
                    CompanyInfo.setSessionHttpVal("Security_Flag", false, context);
                    dt = null;
                }
                else
                {
                    if (!checklink.Contains("api/CompanyInfo/Info") && !checklink.Contains("api/LoginApi/CheckLogin") && !checklink.Contains("/token"))
                    {
                        //  context.Session["Security_Flag"] = true;
                        CompanyInfo.setSessionHttpVal("Security_Flag", true, context);
                    }


                }
            }
            else
            {
                // context.Session["Security_Flag"] = true;
                CompanyInfo.setSessionHttpVal("Security_Flag", true, context);
            }
            return dt;
        }

       

        public static bool ValidateServerCertificate(
        object sender,
        X509Certificate certificate,
        X509Chain chain,
        SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.None)
            {
                return true;
            }

            Console.WriteLine("Certificate error: {0}", sslPolicyErrors);
            // Do not allow this client to communicate with unauthenticated servers.
            return false;
        }

        static async Task   aa()
        {             
            try
            {
                
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
            }
        }

        public static DataTable checklocationforNewLocation(int Client_ID, string userAgent, HttpContext context, string ipAddress, string latitude, string longitude)
        {
            string act = "";
            DataTable dt = new DataTable();
            dt.Columns.Add("browser_info", typeof(string));
            //dt.Columns.Add("birthdate", typeof(string));
            dt.Columns.Add("is_valid", typeof(Boolean));
            dt.Columns.Add("Country", typeof(string));
            dt.Columns.Add("device_ty", typeof(string));
            dt.Columns.Add("network", typeof(string));
            var countryf = "";
            string browserinfo = "";
            string device_ty = "";
            string network = "";

            //var userAgent =  HttpContext.Current.Request.Headers["User-Agent"];
             aa();


            try
            {
                if ((userAgent.ToString().Substring(13, 20)).ToLower().Contains("iphone"))
                {
                    device_ty = "Iphone " + ((userAgent.ToString().Substring(13, 20)).ToLower()).Substring(((userAgent.ToString().Substring(13, 20)).ToLower()).IndexOf("iphone") + 7).Split(' ')[1].Replace(';', ' ');
                }
                else if ((userAgent.ToString().Substring(13, 20)).ToLower().Contains("android"))
                {
                    device_ty = "Android " + ((userAgent.ToString().Substring(13, 20)).ToLower()).Substring(((userAgent.ToString().Substring(13, 20)).ToLower()).IndexOf("android") + 7).Split(' ')[1].Replace(';', ' ');

                }
                else if ((userAgent.ToString().Substring(13, 20)).ToLower().Contains("windows"))
                {
                    device_ty = "Windows";

                }
                else if ((userAgent.ToString().Substring(13, 20)).ToLower().Contains("mac"))
                {
                    device_ty = "Mac";

                }
            }
            catch (Exception ex)
            {
                device_ty = userAgent.ToString();
            }

            act = act + "| Device : " + device_ty;


            MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("GetPermissions");
            _cmd.CommandType = CommandType.StoredProcedure;

            _cmd.Parameters.AddWithValue("_whereclause", " and PID = 97");
            _cmd.Parameters.AddWithValue("_Client_ID", Client_ID);
            DataTable per = db_connection.ExecuteQueryDataTableProcedure(_cmd);
            //var context = HttpContext.Current;
            act = act + "| permission : " + Convert.ToInt32(per.Rows[0]["Status_ForCustomer"]);

            string checklink = CompanyInfo.path_query(context); 

            int Customer_ID = 0;
            try
            {
                Customer_ID = Convert.ToInt32(CompanyInfo.getSessionHttpVal("Customer_ID", context)); // Convert.ToInt32(context.Session["Customer_ID"]);
            }
            catch
            {

            }
            try
            {
                DataTable dtc = CompanyInfo.get(Client_ID, context);
                DataTable dtcust = (DataTable)getCustomerDetails(Client_ID, Customer_ID);

                string ipAddressContext = "";
                
                if(ipAddress != "" && ipAddress != null)
                {
                    ipAddressContext = ipAddress;
                }
                else
                {
                    ipAddressContext = CompanyInfo.ipAddressContext(context);
                }


                string IPAddress1 = ipAddressContext; // HttpContext.Current.Request.UserHostAddress;
                browserinfo = "IP Address: " + IPAddress1;

                try
                {
                    /*var client = new RestClient("https://tools.keycdn.com/geo.json?host=" + IPAddress1);
                    ServicePointManager.Expect100Continue = true; 
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
                       | SecurityProtocolType.Tls11
                       | SecurityProtocolType.Tls12
                       ;
                    client.Timeout = -1;
                    var request = new RestRequest(Method.GET);

                    client.UserAgent = "keycdn-tools:" + Convert.ToString(dtc.Rows[0]["company_website"]);
                    
                    request.AddHeader("Accept-Encoding", "gzip, deflate, br");

                    IRestResponse response = client.Execute(request);
                    _ = CompanyInfo.InsertActivityLogDetailsasync("Location details:"+ response.Content, 0, 0, 0, 0, "checklocationforNewLocation", 0, 0, "", context);
                    GeoLocation GeoLocationList = Newtonsoft.Json.JsonConvert.DeserializeObject<GeoLocation>(response.Content);
                    //browserinfo = Newtonsoft.Json.JsonConvert.SerializeObject(GeoLocationList.data.geo);
                    */
                    ServicePointManager.Expect100Continue = true;
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls |
                                       SecurityProtocolType.Tls11 |
                                       SecurityProtocolType.Tls12 |
                                       SecurityProtocolType.Tls13;
                    ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
                   

                }
                catch (Exception ex)
                {
                    browserinfo = "IP Address: " + IPAddress1;
                }


                try
                {
                    ServicePointManager.Expect100Continue = true;
                    ServicePointManager.Expect100Continue = true;
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;
                    //ServicePointManager.SecurityProtocol &= ~SecurityProtocolType.Tls13;
                    System.Net.ServicePointManager.ServerCertificateValidationCallback += delegate { return true; };

                    var client = new RestClient("https://tools.keycdn.com/geo.json?host=" + IPAddress1);
                    act = act + " RestClient query :" + "https://tools.keycdn.com/geo.json?host=" + IPAddress1;

                    client.Timeout = -1;
                    var request = new RestRequest(Method.GET);
                    
                    client.UserAgent = "keycdn-tools:" + Convert.ToString(dtc.Rows[0]["company_website"]); //dtc.Rows[0]["Company_URL_Customer"] 
                    act = act + " client.UserAgent :" + "keycdn-tools:" + Convert.ToString(dtc.Rows[0]["company_website"]);

                    //keycdn-tools:https://www.calyx-solutions.com
                    request.AddHeader("Accept-Encoding", "gzip, deflate, br");

                    IRestResponse response = client.Execute(request);
                    act = act + " GeoLocationList :" + Convert.ToString(response.ErrorMessage);
                    act = act + " GeoLocationListStatus :" + Convert.ToString(response.StatusCode);
                    GeoLocation GeoLocationList = Newtonsoft.Json.JsonConvert.DeserializeObject<GeoLocation>(response.Content);
                    act = act + " GeoLocationList :" + Convert.ToString(GeoLocationList);
                    try
                    {
                        act = act + " GeoLocationList.data :" + Convert.ToString(GeoLocationList.data);
                    }
                    catch (Exception ex)
                    { }
                    act = act + " GeoLocationList.data.geo :" + Convert.ToString(GeoLocationList.data.geo);
                    browserinfo = Newtonsoft.Json.JsonConvert.SerializeObject(GeoLocationList.data.geo);
                }
                catch (Exception ex)
                {
                    act = act + " inlocationException :" + ex.ToString();
                    browserinfo = "IP Address: " + IPAddress1;
                }
                //var localAddr = "{\"host\":\"86.30.8.13\",\"ip\":\"86.30.8.13\",\"rdns\":\"cpc114386-basl13-2-0-cust12.20-1.cable.virginm.net\",\"asn\":\"5089\",\"isp\":\"Virgin Media Limited\",\"country_name\":\"United Kingdom\",\"country_code\":\"GB\",\"region_name\":\"Essex\",\"region_code\":\"ESS\",\"city\":\"Basildon\",\"postal_code\":\"SS14\",\"continent_name\":\"Europe\",\"continent_code\":\"EU\",\"latitude\":\"51.5645\",\"longitude\":\"0.4574\",\"metro_code\":null,\"timezone\":\"Europe/London\",\"datetime\":\"2022-03-25 09:53:46\"}";
                act = act + "|Checking for invalid location";
                string city = "";
                string region_name = "";
                string postal_code = "";
                string country_name = "";
                try
                {
                    var Address = browserinfo.Split(',');
                    string isp = "";
                    try
                    {
                        isp = Address[Array.FindIndex(Address, x => x.Contains("isp"))].Split(':')[1];
                        network = isp;
                    }
                    catch { act = act + " Error Address" + isp; }

                    try
                    {
                        city = Address[Array.FindIndex(Address, x => x.Contains("city"))].Split(':')[1];
                    }
                    catch { act = act + " Error Address" + city; }

                    try
                    {
                        region_name = Address[Array.FindIndex(Address, x => x.Contains("region_name"))].Split(':')[1];
                    }
                    catch { act = act + " Error Address" + region_name; }

                    try
                    {
                        postal_code = Address[Array.FindIndex(Address, x => x.Contains("postal_code"))].Split(':')[1];

                    }
                    catch { act = act + " Error Address" + postal_code; }

                    try
                    {
                        country_name = Address[Array.FindIndex(Address, x => x.Contains("country_name"))].Split(':')[1];

                    }
                    catch { act = act + " Error Address" + country_name; }



                    act = act + " Address" + Address[0];
                    var country = Address[Array.FindIndex(Address, x => x.Contains("country_name"))].Split(':');
                    act = act + " country" + country[0];

                    var country1 = country[1];
                    act = act + " country1" + country1[0];

                    act = act + " | C -" + country1;
                    countryf = country1.Replace("\"", "");
                    act = act + " countryf" + countryf;
                }
                catch (Exception ex)
                {
                    act = act + " inException :" + ex.ToString();
                }
                string baseCurrency = Convert.ToString(dtcust.Rows[0]["Country_Name"]);//(dtc.Rows[0]["BaseCurrency_Country"]);
                act = act + " baseCurrency" + baseCurrency;

                act = act + " | cp -" + baseCurrency;
                if (countryf != baseCurrency)
                //act = act + " | countryf -" + countryf;
                {
                    _ = InsertActivityLogDetailsasync(act + " ( " + browserinfo + ") countryf : " + countryf + "", 0, 0, 0, 0, "check_location", 0, 0, "", context);

                    countryf = city + ", " + region_name + ", " + postal_code + ", " + country_name;
                    countryf = countryf.Replace("\"", "");
                    if (Convert.ToInt32(per.Rows[0]["Status_ForCustomer"]) == 1  )
                    {
                        act = act + "| permission is on  ";

                        dt.Rows.Add(browserinfo, true, countryf + "<br /><strong>Network</strong> " + network, device_ty, network);
                        return dt;
                    }
                    dt.Rows.Add(browserinfo, false, countryf + "<br /><strong>Network</strong> " + network, device_ty, network);

                    return dt;
                }
            }
            catch
            {

            }
            _ = CompanyInfo.InsertActivityLogDetailsasync(act + " ( " + browserinfo + "),(isp : " + network + ")", 0, 0, 0, 0, "check_location", 0, 0, "", context);

            dt.Rows.Add(browserinfo, true, countryf + "<br /><strong>Network</strong> " + network, device_ty, network);
            return dt;

        }

        public static DataTable check_location(int Client_ID, string userAgent, HttpContext context)
        {
            string act = "";
            DataTable dt = new DataTable();
            dt.Columns.Add("browser_info", typeof(string));
            //dt.Columns.Add("birthdate", typeof(string));
            dt.Columns.Add("is_valid", typeof(Boolean));
            dt.Columns.Add("Country", typeof(string));
            dt.Columns.Add("device_ty", typeof(string));
            dt.Columns.Add("network", typeof(string));
            var countryf = "";
            string browserinfo = "";
            string device_ty = "";
            string network = "";

            //var userAgent =  HttpContext.Current.Request.Headers["User-Agent"];


            try
            {
                if ((userAgent.ToString().Substring(13, 20)).ToLower().Contains("iphone"))
                {
                    device_ty = "Iphone " + ((userAgent.ToString().Substring(13, 20)).ToLower()).Substring(((userAgent.ToString().Substring(13, 20)).ToLower()).IndexOf("iphone") + 7).Split(' ')[1].Replace(';', ' ');
                }
                else if ((userAgent.ToString().Substring(13, 20)).ToLower().Contains("android"))
                {
                    device_ty = "Android " + ((userAgent.ToString().Substring(13, 20)).ToLower()).Substring(((userAgent.ToString().Substring(13, 20)).ToLower()).IndexOf("android") + 7).Split(' ')[1].Replace(';', ' ');

                }
                else if ((userAgent.ToString().Substring(13, 20)).ToLower().Contains("windows"))
                {
                    device_ty = "Windows";

                }
                else if ((userAgent.ToString().Substring(13, 20)).ToLower().Contains("mac"))
                {
                    device_ty = "Mac";

                }
            }
            catch (Exception ex)
            {
                device_ty = userAgent.ToString();
            }

            act = act + "| Device : " + device_ty;

            
            MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("GetPermissions");
            _cmd.CommandType = CommandType.StoredProcedure;

            _cmd.Parameters.AddWithValue("_whereclause", " and PID = 97");
            _cmd.Parameters.AddWithValue("_Client_ID", Client_ID);
            DataTable per = db_connection.ExecuteQueryDataTableProcedure(_cmd);
            //var context = HttpContext.Current;
            act = act + "| permission : " + Convert.ToInt32(per.Rows[0]["Status_ForCustomer"]);

            string checklink = CompanyInfo.path_query(context); // context.Request.Url.PathAndQuery.ToString();
            //if (Convert.ToInt32(per.Rows[0]["Status_ForCustomer"]) == 1 || checklink.Contains("/token"))
            //{
            //    act = act + "| returning empty : checklink: " + checklink;

            //    dt.Rows.Add(browserinfo, true, countryf, device_ty, network);
            //    return dt;
            //}
            
            int Customer_ID = 0;
            try
            {
                Customer_ID = Convert.ToInt32(CompanyInfo.getSessionHttpVal("Customer_ID", context)); // Convert.ToInt32(context.Session["Customer_ID"]);
            }
            catch
            {
            }
            try
            {
                DataTable dtc = CompanyInfo.get(Client_ID,context);
                DataTable dtcust = (DataTable)getCustomerDetails(Client_ID, Customer_ID);

                
                string ipAddressContext = CompanyInfo.ipAddressContext(context);

                string IPAddress1 = ipAddressContext; // HttpContext.Current.Request.UserHostAddress;
                browserinfo = "IP Address: " + IPAddress1;
                try
                {
                    ServicePointManager.Expect100Continue = true;
                    ServicePointManager.Expect100Continue = true;
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;
                    //ServicePointManager.SecurityProtocol &= ~SecurityProtocolType.Tls13;
                    System.Net.ServicePointManager.ServerCertificateValidationCallback += delegate { return true; };
                    //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls13;
                    //       //| SecurityProtocolType.Tls11
                    //       //| SecurityProtocolType.Tls12
                    //       //| SecurityProtocolType.Ssl3;
                    var client = new RestClient("https://tools.keycdn.com/geo.json?host=" + IPAddress1);
                    act = act + " RestClient query :" + "https://tools.keycdn.com/geo.json?host=" + IPAddress1;

                    client.Timeout = -1;
                    var request = new RestRequest(Method.GET);
                    client.UserAgent = "keycdn-tools:" + Convert.ToString(dtc.Rows[0]["company_website"]); //dtc.Rows[0]["Company_URL_Customer"] 
                    act = act + " client.UserAgent :" + "keycdn-tools:" + Convert.ToString(dtc.Rows[0]["company_website"]);

                    //keycdn-tools:https://www.calyx-solutions.com
                    request.AddHeader("Accept-Encoding", "gzip, deflate, br");

                    IRestResponse response = client.Execute(request);
                    act = act + " GeoLocationList :" + Convert.ToString(response.ErrorMessage);
                    act = act + " GeoLocationListStatus :" + Convert.ToString(response.StatusCode);
                    GeoLocation GeoLocationList = Newtonsoft.Json.JsonConvert.DeserializeObject<GeoLocation>(response.Content);
                    act = act + " GeoLocationList :" + Convert.ToString(GeoLocationList);
                    try
                    {
                        act = act + " GeoLocationList.data :" + Convert.ToString(GeoLocationList.data);
                    }
                    catch (Exception ex)
                    { }
                        act = act + " GeoLocationList.data.geo :" + Convert.ToString(GeoLocationList.data.geo);
                    browserinfo = Newtonsoft.Json.JsonConvert.SerializeObject(GeoLocationList.data.geo);
                }
                catch (Exception ex)
                {
                    act = act + " inlocationException :" + ex.ToString();
                    browserinfo = "IP Address: " + IPAddress1;
                }
                //var localAddr = "{\"host\":\"86.30.8.13\",\"ip\":\"86.30.8.13\",\"rdns\":\"cpc114386-basl13-2-0-cust12.20-1.cable.virginm.net\",\"asn\":\"5089\",\"isp\":\"Virgin Media Limited\",\"country_name\":\"United Kingdom\",\"country_code\":\"GB\",\"region_name\":\"Essex\",\"region_code\":\"ESS\",\"city\":\"Basildon\",\"postal_code\":\"SS14\",\"continent_name\":\"Europe\",\"continent_code\":\"EU\",\"latitude\":\"51.5645\",\"longitude\":\"0.4574\",\"metro_code\":null,\"timezone\":\"Europe/London\",\"datetime\":\"2022-03-25 09:53:46\"}";
                act = act + "|Checking for invalid location";
                string city = "";
                string region_name = "";
                string postal_code = "";
                string country_name = "";
                try
                {
                    var Address = browserinfo.Split(',');
                    string isp = "";
                    try
                    {
                        isp = Address[Array.FindIndex(Address, x => x.Contains("isp"))].Split(':')[1];
                        network = isp;
                    }
                    catch { act = act + " Error Address" + isp; }

                    try
                    {
                        city = Address[Array.FindIndex(Address, x => x.Contains("city"))].Split(':')[1];
                    }
                    catch { act = act + " Error Address" + city; }

                    try
                    {
                        region_name = Address[Array.FindIndex(Address, x => x.Contains("region_name"))].Split(':')[1];
                    }
                    catch { act = act + " Error Address" + region_name; }

                    try
                    {
                        postal_code = Address[Array.FindIndex(Address, x => x.Contains("postal_code"))].Split(':')[1];

                    }
                    catch { act = act + " Error Address" + postal_code; }

                    try
                    {
                        country_name = Address[Array.FindIndex(Address, x => x.Contains("country_name"))].Split(':')[1];

                    }
                    catch { act = act + " Error Address" + country_name; }



                    act = act + " Address" + Address[0];
                    var country = Address[Array.FindIndex(Address, x => x.Contains("country_name"))].Split(':');
                    act = act + " country" + country[0];

                    var country1 = country[1];
                    act = act + " country1" + country1[0];

                    act = act + " | C -" + country1;
                    countryf = country1.Replace("\"", "");
                    act = act + " countryf" + countryf;
                }
                catch (Exception ex)
                {
                    act = act + " inException :" + ex.ToString();
                }
                string baseCurrency = Convert.ToString(dtcust.Rows[0]["Country_Name"]);//(dtc.Rows[0]["BaseCurrency_Country"]);
                act = act + " baseCurrency" + baseCurrency;

                act = act + " | cp -" + baseCurrency;
                if (countryf != baseCurrency)
                //act = act + " | countryf -" + countryf;
                {
                    _ = InsertActivityLogDetailsasync(act + " ( " + browserinfo + ") countryf : " + countryf + "", 0, 0, 0, 0, "check_location", 0, 0, "", context);

                    countryf = city + ", " + region_name + ", " + postal_code + ", " + country_name;
                    countryf = countryf.Replace("\"", "");
                    if (Convert.ToInt32(per.Rows[0]["Status_ForCustomer"]) == 1 || checklink.Contains("/token"))
                    {
                        act = act + "| permission is on  ";

                        dt.Rows.Add(browserinfo, true, countryf + "<br /><strong>Network</strong> " + network, device_ty, network);
                        return dt;
                    }
                    dt.Rows.Add(browserinfo, false, countryf + "<br /><strong>Network</strong> " + network, device_ty, network);

                    return dt;
                }
            }
            catch
            {

            }
            _ = CompanyInfo.InsertActivityLogDetailsasync(act + " ( " + browserinfo + "),(isp : " + network + ")", 0, 0, 0, 0, "check_location", 0, 0, "", context);

            dt.Rows.Add(browserinfo, true, countryf + "<br /><strong>Network</strong> " + network, device_ty, network);
            return dt;

        }

        public static bool checksqlinjectiondata(object o)
        {
            bool check = true;

            foreach (JProperty x in (JToken)o)
            {  
               /* string name = x.Name;
                JToken value = x.Value;*/

                TAntiSQLInjection anti = new TAntiSQLInjection(TDbVendor.DbVMysql);

                string testSQL = "select  1 from a where  a='" + x.Value + "'";
                String msg = "";
                if (anti.isInjected(testSQL))
                {
                    check = false;
                    for (int j = 0; j < anti.getSqlInjections().Count; j++)
                    {
                        msg = msg + Environment.NewLine + ("type: " + anti.getSqlInjections()[j].getType() + ", description: " + anti.getSqlInjections()[j].getDescription());
                    }
                }
            }
            return check;
        }

            public static bool checksqlinjection(dynamic ext)
        {
            string jsonString = "";
            bool check = true;
            string type = ext.Request.ContentType.ToString().Split(';')[0];

            if (type == "multipart/form-data")
            {
                for (int i = 0; i < ext.Request.Form.Count; i++)
                {
                    TAntiSQLInjection anti = new TAntiSQLInjection(TDbVendor.DbVMysql);

                    string testSQL = "select  1 from a where  a='" + ext.Request.Form.GetValues(i)[0] + "'";
                    String msg = "";
                    if (anti.isInjected(testSQL))
                    {
                        check = false;
                        for (int j = 0; j < anti.getSqlInjections().Count; j++)
                        {
                            msg = msg + Environment.NewLine + ("type: " + anti.getSqlInjections()[j].getType() + ", description: " + anti.getSqlInjections()[j].getDescription());
                        }
                    }


                }

            }
            else
            {
                ext = ext.Request.InputStream;
                ext.Position = 0;
                using (StreamReader inputStream = new StreamReader(ext))
                {
                    jsonString = inputStream.ReadToEnd();
                }
                //JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();

                object o = JsonConvert.DeserializeObject(jsonString);
               // string json2 = JsonConvert.SerializeObject(o, Newtonsoft.Json.Formatting.Indented);

                object serJsonDetails = JsonConvert.DeserializeObject(jsonString, typeof(object));
                Dictionary<string, object> dictObj = (Dictionary<string, object>)serJsonDetails;

                /* javaScriptSerializer.MaxJsonLength = 2147483647;
                object serJsonDetails = javaScriptSerializer.Deserialize(jsonString, typeof(object));
                Dictionary<string, object> dictObj = (Dictionary<string, object>)serJsonDetails;  */

                for (int i = 0; i < dictObj.Count; i++)
                {
                    TAntiSQLInjection anti = new TAntiSQLInjection(TDbVendor.DbVMysql);

                    string testSQL = "select  1 from a where  a='" + dictObj.Values.ElementAt(i) + "'";
                    String msg = "";
                    if (anti.isInjected(testSQL))
                    {
                        check = false;
                        for (int j = 0; j < anti.getSqlInjections().Count; j++)
                        {
                            msg = msg + Environment.NewLine + ("type: " + anti.getSqlInjections()[j].getType() + ", description: " + anti.getSqlInjections()[j].getDescription());
                        }
                    }
                }
            }
            return check;

        }

        public static object InsertErrorLogTracker(string Activity, int WhoAcessed, int Transaction_ID, int User_ID, int Delete_Status, string FunctionName, int Branch_ID, int Client_ID, string Module, HttpContext context)
        {
            string msg = string.Empty;

            db_connection aController = new db_connection();
            string conneObj = aController.WebConnSetting();

            MySqlConnector.MySqlConnection con = new MySqlConnector.MySqlConnection(conneObj);
            MySqlConnector.MySqlCommand cmd = new MySqlConnector.MySqlCommand("sp_save_error_log");
            try
            {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = con;
            con.Open();
            cmd.Parameters.AddWithValue("_Error", Activity);
            cmd.Parameters.AddWithValue("_Record_insert_Date_time", CompanyInfo.gettime(Client_ID, context));
            cmd.Parameters.AddWithValue("_User_ID", User_ID);
            cmd.Parameters.AddWithValue("_Client_ID", Client_ID);
            cmd.Parameters.AddWithValue("_Branch_ID", Branch_ID);
            cmd.Parameters.AddWithValue("_Delete_Status", Delete_Status);
            cmd.Parameters.AddWithValue("_Function_Name", FunctionName);
            int n = cmd.ExecuteNonQuery();
            if (n > 0)//success
            {
                msg = "success";
            }
            else//not success
            {
                msg = "notsuccess";
            }
            }
            finally
            {
                con.Close();
                cmd.Dispose();
            }
            return msg;
        }

        public static object InsertrequestLogTracker(string Activity, int WhoAcessed, int Transaction_ID, int User_ID, int Delete_Status, string FunctionName, int Branch_ID, int Client_ID, string Module, HttpContext context)
        {            
                string msg = string.Empty;

                db_connection aController = new db_connection();
                string conneObj = aController.WebConnSetting();

                MySqlConnector.MySqlConnection con = new MySqlConnector.MySqlConnection(conneObj);
            MySqlConnector.MySqlCommand cmd = new MySqlConnector.MySqlCommand("sp_save_request_log");
            try
                {
                
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = con;
                con.Open();
                cmd.Parameters.AddWithValue("_Error", Activity);
                cmd.Parameters.AddWithValue("_Record_insert_Date_time", CompanyInfo.gettime(Client_ID, context));
                cmd.Parameters.AddWithValue("_User_ID", User_ID);
                cmd.Parameters.AddWithValue("_Client_ID", Client_ID);
                cmd.Parameters.AddWithValue("_Branch_ID", Branch_ID);
                cmd.Parameters.AddWithValue("_Delete_Status", Delete_Status);
                cmd.Parameters.AddWithValue("_Function_Name", FunctionName);
                int n = cmd.ExecuteNonQuery();
                if (n > 0)//success
                {
                    msg = "success";
                }
                else//not success
                {
                    msg = "notsuccess";
                }
               
            }

            finally
            {
                con.Close();
                cmd.Dispose();
            }

            return msg;
        }

        public static async Task<int> InsertActivityLogDetailsasync(string Activity, int WhoAcessed, int Transaction_ID, int User_ID, int Customer_ID, string FunctionName, int Branch_ID, int Client_ID, string Module, HttpContext context)
        {
            await Task.Delay(10);
            Model.ActivityLogTracker _objActivityLog = new Model.ActivityLogTracker();
            _objActivityLog.Activity = Activity;
            _objActivityLog.FunctionName = FunctionName;
            _objActivityLog.Transaction_ID = Transaction_ID;
            _objActivityLog.WhoAcessed = WhoAcessed;
            _objActivityLog.Branch_ID = Branch_ID;
            _objActivityLog.Client_ID = Client_ID;
            _objActivityLog.Customer_ID = Customer_ID;
            Service.srvActivityLogTracker srvActivityLog = new Service.srvActivityLogTracker();
            int _i = srvActivityLog.Create(_objActivityLog, context);


            return _i;
        }

        public static void save_notification_compliance(string _Notification, string _Notification_Icon, string _Customer_ID, DateTime _Record_Insert_DateTime,
      int _Client_ID, int _Read_Count,
   int _User_ID, int _Branch, int _Show_notificationsfor_Admin, int _show_notificationfor_customer, int _show_notificationfor_POC, int transaction_id, HttpContext context)
        {
            if (_Notification.Contains("Compliance monthly limit") || _Notification.Contains("Daily transfer limit") || _Notification.Contains("limit for personal transaction") || _Notification.Contains("Daily transfer count"))
            {
              MySqlConnector.MySqlCommand cmd1 = new MySqlConnector.MySqlCommand("check_notification");
                cmd1.CommandType = CommandType.StoredProcedure;
                cmd1.Parameters.AddWithValue("_Notification_msg", _Notification);
                cmd1.Parameters.AddWithValue("_Record_Insert_DateTime", _Record_Insert_DateTime);
                cmd1.Parameters.AddWithValue("_Customer_ID", _Customer_ID);
                DataTable db = (DataTable)db_connection.ExecuteQueryDataTableProcedure(cmd1);
                if (db.Rows.Count > 0)
                {
                    return;
                }
            }
            try
            {
                int Customer_ID = Convert.ToInt32(_Customer_ID);
                DateTime RecordDate = Convert.ToDateTime(CompanyInfo.gettime(_Client_ID, context));
                MySqlConnector.MySqlCommand cmd_notif = new MySqlConnector.MySqlCommand("sp_save_notification");
                cmd_notif.CommandType = CommandType.StoredProcedure;
                cmd_notif.Parameters.AddWithValue("_Notification", _Notification);
                cmd_notif.Parameters.AddWithValue("_Notification_Icon", _Notification_Icon);
                cmd_notif.Parameters.AddWithValue("_Customer_ID", Customer_ID);
                cmd_notif.Parameters.AddWithValue("_Record_Insert_DateTime", RecordDate);
                cmd_notif.Parameters.AddWithValue("_Read_Count", _Read_Count);
                cmd_notif.Parameters.AddWithValue("_User_ID", _User_ID);
                cmd_notif.Parameters.AddWithValue("_Show_notificationsfor_Admin", 0);
                cmd_notif.Parameters.AddWithValue("_show_notificationfor_customer", _show_notificationfor_customer);
                cmd_notif.Parameters.AddWithValue("_show_notificationfor_POC", _show_notificationfor_POC);
                cmd_notif.Parameters.AddWithValue("_Client_ID", _Client_ID);
                cmd_notif.Parameters.AddWithValue("_Branch", _Branch);
                cmd_notif.Parameters.AddWithValue("_Transaction_ID", transaction_id);
                cmd_notif.Parameters.AddWithValue("_notification_for_compliance", 0);
                db_connection.ExecuteNonQueryProcedure(cmd_notif);
                cmd_notif.Dispose();


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static DataTable getsecutityflag(string email, int Client_ID, int branchid)
        {
            DataTable dt = new DataTable();
            MySqlCommand cmd = new MySqlCommand("sp_check_securityflag");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("_email_id", email);
            cmd.Parameters.AddWithValue("_clientId", Client_ID);
            cmd.Parameters.AddWithValue("_branchId", branchid);
            dt = (DataTable)db_connection.ExecuteQueryDataTableProcedure(cmd);
            cmd.Dispose();
            return dt;
        }
        public static string updatesecutityflag(string email, int Client_ID, int branchid)
        {
            string sts = "";
            DataTable dt = new DataTable();
            MySqlCommand cmd = new MySqlCommand("sp_update_securityflag");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("_email_id", email);
            cmd.Parameters.AddWithValue("_date", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"));
            cmd.Parameters.AddWithValue("_clientId", Client_ID);
            cmd.Parameters.AddWithValue("_branchId", branchid);
            int st = db_connection.ExecuteNonQueryProcedure(cmd);
            if (st > 0)
            {
                sts = "success";
            }
            else
            {
                sts = "failed";
            }
            cmd.Dispose();
            return sts;
        }
        public static object InsertActivityLogDetailsSecurity(string Activity, int WhoAcessed, int Transaction_ID, int User_ID, int Customer_ID, string FunctionName, int Branch_ID, int Client_ID, string Module, int security_flag, HttpContext context)
        {
            Model.ActivityLogTracker _objActivityLog = new Model.ActivityLogTracker();
            _objActivityLog.Activity = Activity;
            _objActivityLog.FunctionName = FunctionName;
            _objActivityLog.Transaction_ID = Transaction_ID;
            _objActivityLog.WhoAcessed = WhoAcessed;
            _objActivityLog.Branch_ID = Branch_ID;
            _objActivityLog.Client_ID = Client_ID;
            _objActivityLog.Customer_ID = Customer_ID;
            _objActivityLog.Security_flag = security_flag;
            Service.srvActivityLogTracker srvActivityLog = new Service.srvActivityLogTracker();
            int _i = srvActivityLog.Create_activity_security(_objActivityLog, context);
            return _i;
        }

      


        public static string Send_RegistrationCompleted_mail(Model.Customer obj, HttpContext context)
        {
            Service.srvCustomer srv = new Service.srvCustomer();           
            if (obj.Email != null || obj.Email != "")
            {
                Service.srvLogin srvLogin = new Service.srvLogin();
                DataTable dt = new DataTable();
                Model.Login obj1 = new Model.Login();
                obj1.UserName = obj.Email;//pradip
                obj1.Client_ID = obj.Client_ID;
                dt = srvLogin.IsValidEmail(obj1);
                Model.response.WebResponse response = null;
                string email = obj1.UserName;//pradip
                obj.UserName = email;
                //string subject = string.Empty;
                string msg = string.Empty;
                string body = string.Empty, subject = string.Empty;
                string body1 = string.Empty;
                string template = "";
                HttpWebRequest httpRequest = null, httpRequest1 = null;
                DataTable dtc = CompanyInfo.get(obj.Client_ID, context);
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
                    try
                    {
                        httpRequest = (HttpWebRequest)WebRequest.Create(URL + "Email/customer-registration-last-step.html");
                        httpRequest.UserAgent = "Code Sample Web Client";
                        webResponse = (HttpWebResponse)httpRequest.GetResponse();
                        using (StreamReader reader = new StreamReader(webResponse.GetResponseStream()))
                        { body = reader.ReadToEnd(); }
                        body = body.Replace("[name]", Convert.ToString(dt.Rows[0]["First_Name"]));
                        string postRegistrationMsg = "Your account registration was successful. Please upload your Photo ID and Proof of Address in your profile for your account to be verified and activated. Once the activation process is complete you can do next steps:<br /><strong>1) Add beneficiaries<br />2) Transfer and view transaction<br />3) Receive notifications on your transfer status</strong><br /><br />";
                        body = body.Replace("[msg]", postRegistrationMsg + "Thank you for completing your registration with " + Convert.ToString(dtc.Rows[0]["Company_Name"]) + ". We are looking forward to seeing you there.");
                        //subject = Convert.ToString(dtc.Rows[0]["Company_Name"]) + " - Registration completed";
                        subject = "Your " + Convert.ToString(dtc.Rows[0]["Company_Name"]) + " Account is Ready — Start Sending Money Today!";
                        msg = (string)CompanyInfo.Send_Mail(dtc, email, body, subject, Convert.ToInt32(obj.Client_ID), Convert.ToInt32(obj.Branch_ID), "", "", "", context);

                    }
                    catch (Exception ex) { }
                    if (msg == "Success")
                    {
                        response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_SUCCESS);
                        response.data = "success";
                        response.ResponseCode = 0;
                        Model.ActivityLogTracker _objActivityLog = new Model.ActivityLogTracker();
                        _objActivityLog.Activity = "Email RegistrationCompleted mail sent to " + obj.UserName + " ";
                        _objActivityLog.FunctionName = "Send_RegistrationCompleted_mail";
                        _objActivityLog.Transaction_ID = 0;
                        _objActivityLog.WhoAcessed = 0;
                        _objActivityLog.Branch_ID = obj.Branch_ID;
                        _objActivityLog.Client_ID = obj.Client_ID;
                        Service.srvActivityLogTracker srvActivityLog = new Service.srvActivityLogTracker();
                        int _i = srvActivityLog.Create(_objActivityLog, context);
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
                        _objActivityLog.Activity = "Email RegistrationCompleted mail not sent to " + obj.UserName + " ";
                        _objActivityLog.FunctionName = "Send_RegistrationCompleted_mail";
                        _objActivityLog.Transaction_ID = 0;
                        _objActivityLog.WhoAcessed = 0;
                        _objActivityLog.Branch_ID = obj.Branch_ID;
                        _objActivityLog.Client_ID = obj.Client_ID;
                        Service.srvActivityLogTracker srvActivityLog = new Service.srvActivityLogTracker();
                        int _i = srvActivityLog.Create(_objActivityLog, context);
                    }
                }
                else
                {
                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_TECHNICAL_ERROR);
                    response.ResponseMessage = "Something went wrong. Please try again later.";
                    response.ResponseCode = 2;
                }

                return msg;
            }
            else
            {
                return "failed";
            }
        }

        public static string Send_verify_mail(Model.Customer obj, HttpContext context)
        {
            Service.srvCustomer srv = new Service.srvCustomer();

            //obj.Customer_ID = obj.Id;
            int a = 1;// srv.Email_Counter(obj);
            if (a == 1)
            {
                Service.srvLogin srvLogin = new Service.srvLogin();
                DataTable dt = new DataTable();
                Model.Login obj1 = new Model.Login();
                obj1.UserName = obj.UserName;
                obj1.Client_ID = obj.Client_ID;
                dt = srvLogin.IsValidEmail(obj1);
                Model.response.WebResponse response = null;
                string email = obj.UserName;
                obj.UserName = email;
                //string subject = string.Empty;
                string msg = string.Empty;
                string body = string.Empty, subject = string.Empty;
                string body1 = string.Empty;
                string template = "";
                HttpWebRequest httpRequest = null, httpRequest1 = null;
                DataTable dtc = CompanyInfo.get(obj.Client_ID, context);
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
                    body = body.Replace("[name]", Convert.ToString(dt.Rows[0]["First_Name"])); //Convert.ToString(dt.Rows[0]["full_name"]));
                    string ref_no = Convert.ToString(dt.Rows[0]["WireTransfer_ReferanceNo"]);
                    string str = CompanyInfo.Encrypt1(ref_no);
                    body = body.Replace("[link]", cust_url + "verify-email?reference=" + str + ""); //body = body.Replace("[link]", cust_url + "verify-email.html?reference=" + ref_no + "");
                    httpRequest1 = (HttpWebRequest)WebRequest.Create(URL + "Email/verify-email.txt");
                    httpRequest1.UserAgent = "Code Sample Web Client";
                    HttpWebResponse webResponse1 = (HttpWebResponse)httpRequest1.GetResponse();
                    using (StreamReader reader = new StreamReader(webResponse1.GetResponseStream()))
                    {
                        subject = reader.ReadLine();
                    }
                    msg = (string)CompanyInfo.Send_Mail(dtc, obj.UserName, body, subject, obj.Client_ID, obj.Branch_ID, "", "", "", context);
                    if (msg == "Success")
                    {
                        response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_SUCCESS);
                        response.data = "success";
                        response.ResponseCode = 0;
                        Model.ActivityLogTracker _objActivityLog = new Model.ActivityLogTracker();
                        _objActivityLog.Activity = "Email Verify mail sent to " + obj.UserName + " ";
                        _objActivityLog.FunctionName = "Send_verify_mail";
                        _objActivityLog.Transaction_ID = 0;
                        _objActivityLog.WhoAcessed = 0;
                        _objActivityLog.Branch_ID = obj.Branch_ID;
                        _objActivityLog.Client_ID = obj.Client_ID;
                        Service.srvActivityLogTracker srvActivityLog = new Service.srvActivityLogTracker();
                        int _i = srvActivityLog.Create(_objActivityLog, context);
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
                        _objActivityLog.Activity = "Email Verify mail not sent to " + obj.UserName + " ";
                        _objActivityLog.FunctionName = "Send_verify_mail";
                        _objActivityLog.Transaction_ID = 0;
                        _objActivityLog.WhoAcessed = 0;
                        _objActivityLog.Branch_ID = obj.Branch_ID;
                        _objActivityLog.Client_ID = obj.Client_ID;
                        Service.srvActivityLogTracker srvActivityLog = new Service.srvActivityLogTracker();
                        int _i = srvActivityLog.Create(_objActivityLog, context);
                    }
                }
                else
                {
                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_TECHNICAL_ERROR);
                    response.ResponseMessage = "Something went wrong. Please try again later.";
                    response.ResponseCode = 2;
                }

                return msg;
            }
            else
            {
                return "failed";
            }
        }


        public static string Send_Mail_usingZoho(string email_id, string test, string subject, int client_id, int branch_id, string flag, string CC, string BCC, DataTable drtem,HttpContext context)
        {
            string email = email_id;
            string msg = string.Empty; string from = string.Empty;
            string password = string.Empty;

            using (MailMessage mailMessage = new MailMessage())
            {
                string security_key = Convert.ToString(CompanyInfo.SecurityKey());
                DataTable company_email_details = (DataTable)get(client_id,context);

                int zohoAPIID = Convert.ToInt32(drtem.Rows[0]["zohoid"]);
                string leadid = drtem.Rows[0]["leadid"].ToString();
                string createdLeadId = leadid;
                string zohoAccessToken = drtem.Rows[0]["zohoToken"].ToString();
                string templateName = drtem.Rows[0]["templateName"].ToString();
                string Query = "select * from thirdpartyapi_master where Delete_Status=0 and Client_ID=1 and API_ID=" + zohoAPIID + " and API_Status= 0";
                MySqlCommand cmd5 = new MySqlCommand(Query);
                DataTable dtt = db_connection.ExecuteQueryDataTableProcedure(cmd5);

                string client_idcode = "", client_secret = "", refresh_token = "", apiurl = "", Company_Name = "", api_fields = "", templateId = "";
                if (dtt.Rows.Count > 0)
                {
                    client_idcode = Convert.ToString(dtt.Rows[0]["UserName"]);
                    client_secret = Convert.ToString(dtt.Rows[0]["Password"]);
                    api_fields = Convert.ToString(dtt.Rows[0]["ProfileID"]);
                    apiurl = Convert.ToString(dtt.Rows[0]["API_URL"]);

                    if (api_fields != "" && api_fields != null)
                    {
                        Newtonsoft.Json.Linq.JObject obj = Newtonsoft.Json.Linq.JObject.Parse(api_fields);
                        //createdLeadId = Convert.ToString(obj["createLeadId"]);
                        refresh_token = Convert.ToString(obj["refresh_token"]);
                        Company_Name = Convert.ToString(obj["company_name"]);
                        templateId = Convert.ToString(obj[templateName]);
                    }
                }

                //get send email details
                int mail_perm = 1;
                DataTable dt_perm1 = new DataTable();
                MySqlCommand cmd_prem1 = new MySqlCommand("select * from permission_master where PID=84 and Client_ID=@Client_ID");
                cmd_prem1.Parameters.AddWithValue("@Client_ID", 1);
                dt_perm1 = db_connection.ExecuteQueryDataTableProcedure(cmd_prem1);
                if (Convert.ToInt32(dt_perm1.Rows[0]["Status_ForCustomer"]) == 0)
                {
                    mail_perm = 0;
                }

                MySqlCommand _cmd = new MySqlCommand("Get_Email_Configuration");
                _cmd.CommandType = CommandType.StoredProcedure;
                string _whereclause = " and Client_ID=" + client_id + " and   priority=4";

                _cmd.Parameters.AddWithValue("_whereclause", _whereclause);
                _cmd.Parameters.AddWithValue("_SecurityKey", CompanyInfo.SecurityKey());
                DataTable dt_send_email = db_connection.ExecuteQueryDataTableProcedure(_cmd);
                if (dt_send_email.Rows.Count > 0)
                {
                    from = dt_send_email.Rows[0]["Email_Convey_from"].ToString();
                    password = dt_send_email.Rows[0]["Password"].ToString();
                }
                if (dt_send_email.Rows.Count > 0)
                {
                    from = dt_send_email.Rows[0]["Email_Convey_from"].ToString();
                    password = dt_send_email.Rows[0]["Password"].ToString();
                }
                subject = subject.Replace("[company_name]", company_email_details.Rows[0]["Company_Name"].ToString());

                if ((email == "" || email == null) && mail_perm == 0)
                {

                    email = company_email_details.Rows[0]["Company_Email"].ToString();
                    mailMessage.To.Add(new MailAddress(email));
                }
                else
                {
                    string[] bccid = email.Split(',');
                    foreach (string bccEmailId in bccid)
                    {
                        if (bccEmailId != "" && bccEmailId != null)
                        {
                            mailMessage.To.Add(new MailAddress(bccEmailId));
                            //mailMessage.Bcc.Add(new MailAddress(bccEmailId)); //Adding Multiple BCC email Id
                        }
                    }
                    //mailMessage.To.Add(new MailAddress(email));
                }

                DataTable dt_get_Permission = (DataTable)getEmailPermission(client_id, 21);
                string EmailID = "";
                if (dt_get_Permission.Rows.Count > 0)
                {

                    if (dt_get_Permission.Rows[0]["Status"].ToString() == "0")//send mail to admin
                    {
                        DataTable dt_admin_Email_list = (DataTable)getAdminEmailList(client_id, branch_id);
                        if (dt_admin_Email_list.Rows.Count > 0)
                        {
                            for (int a = 0; a < dt_admin_Email_list.Rows.Count; a++)
                            {
                                string AdminEmailID = Convert.ToString(dt_admin_Email_list.Rows[a]["Email_ID"]) + ",";
                                EmailID += AdminEmailID;
                            }
                            string[] bccid = EmailID.Split(',');
                            foreach (string bccEmailId in bccid)
                            {
                                if (bccEmailId != "" && bccEmailId != null)
                                {
                                    mailMessage.Bcc.Add(new MailAddress(bccEmailId)); //Adding Multiple BCC email Id
                                }
                            }
                        }
                    }
                    if (dt_get_Permission.Rows[0]["Status"].ToString() == "1")//no mail
                    {
                        //  mailMessage.To.Add(new MailAddress(email));
                    }
                    if (flag == "custom_email")
                    {
                        string[] ccid = CC.Split(',');
                        foreach (string ccEmailId in ccid)
                        {
                            if (ccEmailId != "" && ccEmailId != null)
                            {
                                mailMessage.CC.Add(new MailAddress(ccEmailId)); //Adding Multiple CC email Id
                            }
                        }
                        string[] bccid1 = BCC.Split(',');
                        foreach (string bccEmailId1 in bccid1)
                        {
                            if (bccEmailId1 != "" && bccEmailId1 != null)
                            {
                                mailMessage.Bcc.Add(new MailAddress(bccEmailId1)); //Adding Multiple BCC email Id
                            }
                        }
                    }
                }

                try
                {
                    var client = new RestClient(apiurl + "crm/v5/Leads/" + createdLeadId + "/actions/send_mail");
                    ServicePointManager.Expect100Continue = true;
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
                           | SecurityProtocolType.Tls11
                           | SecurityProtocolType.Tls12
                           | SecurityProtocolType.Ssl3;
                    client.Timeout = -1;
                    var request = new RestRequest(Method.POST);
                    request.AddHeader("Authorization", zohoAccessToken);
                    request.AddHeader("Content-Type", "application/json");
                    request.AddHeader("Cookie", "1ccad04dca=4756a8194d77973b28ad9c52af71ea24; _zcsr_tmp=e8e4ed86-82cf-4877-afb8-baaf526abcc9; crmcsr=e8e4ed86-82cf-4877-afb8-baaf526abcc9");

                    Company_Name = "\"" + Company_Name + "\"";
                    string subject_Mail = "\"" + Convert.ToString(subject).Trim() + "\"";
                    string toEmails = "", ccEmailList = "", bccEmailList = "";

                    // To email address code
                    string[] bccid = email_id.Split(',');
                    foreach (string bccEmailId in bccid)
                    {
                        if (bccEmailId.Trim() != "" && bccEmailId.Trim() != null)
                        {
                            toEmails += @"                {  " +
                                        @"                    ""email"": " + "\"" + Convert.ToString(bccEmailId).Trim() + "\"" + " " +
                                        @"                },";
                        }
                    }
                    char lastCharacter = ' ';
                    if (toEmails != "")
                    {
                        lastCharacter = toEmails.Last();
                        if (lastCharacter == ',')
                        {
                            toEmails = toEmails.Remove(toEmails.Length - 1, 1);
                        }
                    }

                    //customer bcc mail address
                    string custAllBccEmailList = EmailID;
                    string[] bccid1 = custAllBccEmailList.Split(',');
                    foreach (string bccEmailId1 in bccid1)
                    {
                        if (bccEmailId1.Trim() != "" && bccEmailId1.Trim() != null)
                        {
                            bccEmailList += @"{""email"": " + "\"" + bccEmailId1.Trim() + "\"" + "},";
                        }
                    }

                    if (bccEmailList != "")
                    {
                        lastCharacter = bccEmailList.Last();
                        if (lastCharacter == ',')
                        {
                            bccEmailList = bccEmailList.Remove(bccEmailList.Length - 1, 1);
                        }
                    }

                    string fromEmail = "\"" + from.Trim() + "\"";

                    var body = @"{" +
                 @"    ""data"": [ " +
                 @"        {""from"": {""user_name"": ""Calyx Solutions"",  " +
                 @"""email"": " + fromEmail + "},  " +
                 @"            ""to"": [  " +
                  toEmails +
                 @"            ], " +
                 @"            ""cc"": [  " +
                  ccEmailList +
                 @"            ], " +
                 @"            ""bcc"": [  " +
                  bccEmailList +
                 @"            ], " +
                 @"            ""org_email"": false,   " +
                 @"            ""subject"": " + subject_Mail + "," +
                 //@"            ""content"": " + "\"" + bodyContent + "\"" + "," + 
                 @"            ""mail_format"": ""html"", " +
                 @"            ""attachments"": [],  " +
                 @"            ""template"": {""id"": " + "\"" + templateId + "\"" + "}} " +
                 @"    ]  " +
                 @"}";
                    request.AddParameter("application/json", body, ParameterType.RequestBody);
                    IRestResponse response = client.Execute(request);
                    Console.WriteLine(response.Content);
                    var obj1 = Newtonsoft.Json.Linq.JObject.Parse(response.Content);

                    //{ "data":[{ "code":"NOT_ALLOWED","details":{ },"message":"You cannot send an email to this customer because this person has opted to stop the email communication","status":"error"}]}
                    // { "code":"INVALID_DATA","details":{ },"message":"the related id given seems to be invalid","status":"error"}

                    if (obj1 != null)
                    {
                        if (Convert.ToString(obj1["data"][0]["code"]) == "NOT_ALLOWED")
                        {
                            // Blocked or Unsubscribe email
                            msg = "NOT_ALLOWED";
                        }
                        if (Convert.ToString(obj1["data"][0]["status"]) == "success")
                        {
                            msg = "Success";
                        }
                    }
                }
                catch (Exception ex)
                {
                    msg = ex.ToString();
                }
            }
            return msg;
        }

        public static object Send_Mail(DataTable dt, string email_id, string body, string subject, int client_id, int branch_id, string flag, string CC, string BCC, HttpContext context)
        {
            string email = email_id; string from = string.Empty; string password = string.Empty;
            string msg = string.Empty;
            using (MailMessage mailMessage = new MailMessage())
            {
                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
                DataTable company_email_details = dt;
                if (dt == null || dt.Rows.Count <= 0)
                {
                    company_email_details = (DataTable)get(client_id, context);
                }
                body = body.Replace("[company_website]", Convert.ToString(company_email_details.Rows[0]["company_website"]));
                subject = subject.Replace("[company_name]", company_email_details.Rows[0]["Company_Name"].ToString());
                // string from = company_email_details.Rows[0]["Email_Convey_from"].ToString(), password = company_email_details.Rows[0]["Password"].ToString();
                body = body.Replace("[company_name]", company_email_details.Rows[0]["Company_Name"].ToString());
                //body = body.Replace("[contact_no]", company_email_details.Rows[0]["Company_mobile"].ToString());body = body.Replace("[email_id]", company_email_details.Rows[0]["Company_Email"].ToString());
                if ((company_email_details.Rows[0]["Company_mobile"] == null) || (company_email_details.Rows[0]["Company_mobile"] == ""))
                {
                    //body = body.Replace("[contact_no]", Convert.ToString(company_email_details.Rows[0]["Company_mobile"]));
                    body = body.Replace("[contact_no]", Convert.ToString("<a href='tel:" + company_email_details.Rows[0]["Company_mobile"] + "'>" + company_email_details.Rows[0]["Company_mobile"]) + "</a>");
                    body = body.Replace("[contact_id1]", "style='display:none'");
                    body = body.Replace("[or1]", "style='display:none'");
                }
                else
                {
                    //body = body.Replace("[contact_no]", Convert.ToString(company_email_details.Rows[0]["Company_mobile"]));
                    body = body.Replace("[contact_no]", Convert.ToString("<a href='tel:" + company_email_details.Rows[0]["Company_mobile"] + "'>" + company_email_details.Rows[0]["Company_mobile"]) + "</a>");

                }
                if ((company_email_details.Rows[0]["Company_Email"] == null) || (company_email_details.Rows[0]["Company_Email"] == ""))
                {
                    body = body.Replace("[email_id]", Convert.ToString(company_email_details.Rows[0]["Company_Email"]));
                    body = body.Replace("[email_id11]", "style='display:none'");
                    body = body.Replace("[or1]", "style='display:none'");
                }
                else
                {
                    body = body.Replace("[email_id]", Convert.ToString(company_email_details.Rows[0]["Company_Email"]));
                }
                if (((company_email_details.Rows[0]["Company_mobile"] == null) || (company_email_details.Rows[0]["Company_mobile"] == "")) && ((company_email_details.Rows[0]["Company_Email"] == null) || (company_email_details.Rows[0]["Company_Email"] == "")))
                {
                    body = body.Replace("[line_id1]", "style='display:none'");
                }
                string send_mail = "mailto:" + company_email_details.Rows[0]["Company_Email"].ToString();
                body = body.Replace("[email_id1]", send_mail);
                body = body.Replace("[privacy_policy]", company_email_details.Rows[0]["privacy_policy"].ToString());
                body = body.Replace("[company_logo]", company_email_details.Rows[0]["Email_company_image"].ToString());
                body = body.Replace("[customer_website]", Convert.ToString(company_email_details.Rows[0]["Company_URL_Customer"]));
                body = body.Replace("[company_addresss]", Convert.ToString(company_email_details.Rows[0]["Company_Address"]));
                body = body.Replace("[company_reg_no]", Convert.ToString(company_email_details.Rows[0]["CompanyReg_No"]));
                body = body.Replace("[company_reg_office]", Convert.ToString(company_email_details.Rows[0]["Company_Address"]));
                body = body.Replace("[company_ref_no].", Convert.ToString(company_email_details.Rows[0]["CompanyRef_No"]));
                body = body.Replace("[email]", email);
                string currentYear = Convert.ToString(DateTime.Now.Year);
                body = body.Replace("©", "© " + currentYear);
                //for bank transfer
                body = body.Replace("[wire1]", Convert.ToString(company_email_details.Rows[0]["AccountHolderName"]));
                body = body.Replace("[wire2]", Convert.ToString(company_email_details.Rows[0]["BankName"]));
                body = body.Replace("[wire3]", Convert.ToString(company_email_details.Rows[0]["AccountNumber"]));
                body = body.Replace("[wire5]", Convert.ToString(company_email_details.Rows[0]["Sort_ID"]));

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

                #region social-media-icons

                if (Convert.ToString(company_email_details.Rows[0]["Playstore"]) == null || (Convert.ToString(company_email_details.Rows[0]["Playstore"]) == ""))
                {
                    body = body.Replace("[Playstore-social]", "");
                }
                else
                {
                    body = body.Replace("[Playstore-social]", "<td class='img' width='40'style='font-size:0pt; line-height:0pt; text-align:left;image-rendering:auto!important;'><a href=" + Convert.ToString(company_email_details.Rows[0]["Playstore"]) + " target='_blank'><img src='" + Convert.ToString(company_email_details.Rows[0]["Company_URL_Customer"]) + "/assets/img/social-media-icons/google-play-store.png' width='100' height='30' style='max-width:100px; border='0' alt='Google Paly Store' /></a></td>");
                }
                if (Convert.ToString(company_email_details.Rows[0]["Appstore"]) == null || (Convert.ToString(company_email_details.Rows[0]["Appstore"]) == ""))
                {
                    body = body.Replace("[Appstore-social]", "");
                }
                else
                {
                    body = body.Replace("[Appstore-social]", "<td class='img' width='40'style='font-size:0pt; line-height:0pt; text-align:left;image-rendering:auto!important;'><a href=" + Convert.ToString(company_email_details.Rows[0]["Appstore"]) + " target='_blank'><img src='" + Convert.ToString(company_email_details.Rows[0]["Company_URL_Customer"]) + "/assets/img/social-media-icons/app-store.png' width='100' height='30' style='max-width:100px; border='0' alt='App Store' /></a></td>");
                }

                if (Convert.ToString(company_email_details.Rows[0]["Twitter"]) == null || (Convert.ToString(company_email_details.Rows[0]["Twitter"]) == ""))
                {
                    body = body.Replace("[twitter-social]", "");
                }
                else
                {
                    body = body.Replace("[twitter-social]", "<td class='img' width='40' style='font-size:0pt; line-height:0pt; text-align:left;image-rendering:auto!important;'> <a href=" + Convert.ToString(company_email_details.Rows[0]["Twitter"]) + " target='_blank'><img src='" + Convert.ToString(company_email_details.Rows[0]["Company_URL_Customer"]) + "/assets/img/social-media-icons/ico4_twitter.png' width='26' height='26' style='max-width:26px; border='0' alt='Twitter' /></a></td>");
                }

                if (Convert.ToString(company_email_details.Rows[0]["Facebook"]) == null || (Convert.ToString(company_email_details.Rows[0]["Facebook"]) == ""))
                {
                    body = body.Replace("[facebook-social]", "");
                }
                else
                {
                    body = body.Replace("[facebook-social]", "<td class='img' width='40' style='font-size:0pt; line-height:0pt; text-align:left;image-rendering:auto!important;'> <a href=" + Convert.ToString(company_email_details.Rows[0]["Facebook"]) + " target='_blank'><img src='" + Convert.ToString(company_email_details.Rows[0]["Company_URL_Customer"]) + "/assets/img/social-media-icons/ico4_facebook.png' width='26' height='26' style='max-width:26px; border='0' alt='Facebook' /></a></td>");
                }

                if (Convert.ToString(company_email_details.Rows[0]["Gmail"]) == null || (Convert.ToString(company_email_details.Rows[0]["Gmail"]) == ""))
                {
                    body = body.Replace("[GooglePluse-social]", "");
                }
                else
                {
                    body = body.Replace("[GooglePluse-social]", "<td class='img' width='40' style='font-size:0pt; line-height:0pt; text-align:left;image-rendering:auto!important;'> <a href='mailto:" + Convert.ToString(company_email_details.Rows[0]["Gmail"]) + "' target='_blank'><img src='" + Convert.ToString(company_email_details.Rows[0]["Company_URL_Customer"]) + "/assets/img/social-media-icons/ico4_gplus.png' width='26' height='26' style='max-width:26px; border='0' alt='Google Pluse' /></a></td>");
                }

                if (Convert.ToString(company_email_details.Rows[0]["Youtube"]) == null || (Convert.ToString(company_email_details.Rows[0]["Youtube"]) == ""))
                {
                    body = body.Replace("[YouTube-social]", "");
                }
                else
                {
                    body = body.Replace("[YouTube-social]", "<td class='img' width='40' style='font-size:0pt; line-height:0pt; text-align:left;image-rendering:auto!important;'> <a href=" + Convert.ToString(company_email_details.Rows[0]["Youtube"]) + " target='_blank'><img src='" + Convert.ToString(company_email_details.Rows[0]["Company_URL_Customer"]) + "/assets/img/social-media-icons/ico4_youtube.png' width='26' height='26' style='max-width:26px; border='0' alt='You Tube' /></a></td>");
                }

                if (Convert.ToString(company_email_details.Rows[0]["Instagram"]) == null || (Convert.ToString(company_email_details.Rows[0]["Instagram"]) == ""))
                {
                    body = body.Replace("[Instagram-social]", "");
                }
                else
                {
                    body = body.Replace("[Instagram-social]", "<td class='img' width='40' style='font-size:0pt; line-height:0pt; text-align:left;image-rendering:auto!important;'> <a href=" + Convert.ToString(company_email_details.Rows[0]["Instagram"]) + " target='_blank'><img src='" + Convert.ToString(company_email_details.Rows[0]["Company_URL_Customer"]) + "/assets/img/social-media-icons/ico4_instagram.png' width='26' height='26' style='max-width:26px; border='0' alt='Instagram' /></a></td>");
                }

                if (Convert.ToString(company_email_details.Rows[0]["Pinterest"]) == null || (Convert.ToString(company_email_details.Rows[0]["Pinterest"]) == ""))
                {
                    body = body.Replace("[Pinterest-social]", "");
                }
                else
                {
                    body = body.Replace("[Pinterest-social]", "<td class='img' width='40' style='font-size:0pt; line-height:0pt; text-align:left;image-rendering:auto!important;'> <a href=" + Convert.ToString(company_email_details.Rows[0]["Pinterest"]) + " target='_blank'><img src='" + Convert.ToString(company_email_details.Rows[0]["Company_URL_Customer"]) + "/assets/img/social-media-icons/ico4_pinterest.png' width='26' height='26' style='max-width:26px; border='0' alt='Pinterest' /></a></td>");
                }
                if (Convert.ToString(company_email_details.Rows[0]["Linkedin"]) == null || (Convert.ToString(company_email_details.Rows[0]["Linkedin"]) == ""))
                {
                    body = body.Replace("[Linkedin-social]", "");
                }
                else
                {
                    body = body.Replace("[Linkedin-social]", "<td class='img' width='40' style='font-size:0pt; line-height:0pt; text-align:left;image-rendering:auto!important;'> <a href=" + Convert.ToString(company_email_details.Rows[0]["Linkedin"]) + " target='_blank'><img src='" + Convert.ToString(company_email_details.Rows[0]["Company_URL_Customer"]) + "/assets/img/social-media-icons/ico4_linkedin.png' width='26' height='26' style='max-width:26px; border='0' alt='Linkedin' /></a></td>");
                }

                if (Convert.ToString(company_email_details.Rows[0]["Whats_app_no"]) == null || (Convert.ToString(company_email_details.Rows[0]["Whats_app_no"]) == ""))
                {
                    body = body.Replace("[Whatsapp-social]", "");
                }
                else
                {
                    body = body.Replace("[Whatsapp-social]", "<td class='img' width='40' style='font-size:0pt; line-height:0pt; text-align:left;image-rendering:auto!important;'> <a href=' https://api.whatsapp.com/send?phone=" + Convert.ToString(company_email_details.Rows[0]["Whats_app_no"]) + "&text=Hello There, I would like to enquire about money transfer.' target='_blank'><img src='" + Convert.ToString(company_email_details.Rows[0]["Company_URL_Customer"]) + "/assets/img/social-media-icons/ico4_whatsapp.png'  width='26' height='26' style='max-width:26px;' border='0' alt='Whatsapp' /></a></td>");
                }
                #endregion

                mailMessage.From = new MailAddress(from);
                mailMessage.Body = body;
                mailMessage.Subject = subject;
                mailMessage.BodyEncoding = System.Text.Encoding.UTF8;
                mailMessage.Priority = MailPriority.High;
                mailMessage.IsBodyHtml = true;

                DataTable ds = (DataTable)getEmailPermission(client_id, 84);
                string Status = Convert.ToString(ds.Rows[0]["Status"]);
                if ((email == "" || email == null) && Status == "0")
                {
                    email = from;//company_email_details.Rows[0]["Company_Email"].ToString();
                }
                else
                {
                    string[] bccid = email.Split(',');
                    foreach (string bccEmailId in bccid)
                    {
                        try
                        {
                            if (bccEmailId != "" && bccEmailId != null)
                        {
                            mailMessage.To.Add(new MailAddress(bccEmailId));
                            //mailMessage.Bcc.Add(new MailAddress(bccEmailId)); //Adding Multiple BCC email Id
                        }

                    }
                            catch(Exception egx){ }

                }
                    // mailMessage.To.Add(new MailAddress(email));
                }
                SmtpClient smtp = new SmtpClient();
                DataTable dt_get_Permission = (DataTable)getEmailPermission(client_id, 21);
                if (dt_get_Permission.Rows.Count > 0)
                {
                    string EmailID = "";
                    if (dt_get_Permission.Rows[0]["Status_ForCustomer"].ToString() == "0")//send mail to admin
                    {
                        DataTable dt_admin_Email_list = (DataTable)getAdminEmailList(client_id, branch_id);
                        if (dt_admin_Email_list.Rows.Count > 0)
                        {
                            for (int a = 0; a < dt_admin_Email_list.Rows.Count; a++)
                            {
                                string AdminEmailID = Convert.ToString(dt_admin_Email_list.Rows[a]["Email_ID"]) + ",";
                                EmailID += AdminEmailID;
                            }
                            string[] bccid = EmailID.Split(',');
                            CompanyInfo.InsertActivityLogDetails("Email list : " + EmailID, 0, 0, 0, 0, "IsValidEmail", 0, 0, "", context);

                            foreach (string bccEmailId in bccid)
                            {
                                try
                                {
                                    if (bccEmailId != "" && bccEmailId != null)
                                    {
                                        mailMessage.Bcc.Add(new MailAddress(bccEmailId)); //Adding Multiple BCC email Id
                                    }
                                }
                                catch (Exception ems) { }
                            }
                        }
                    }
                    if (dt_get_Permission.Rows[0]["Status_ForCustomer"].ToString() == "1")//no mail
                    {
                    }
                    if (flag == "custom_email")
                    {
                        string[] ccid = CC.Split(',');
                        foreach (string ccEmailId in ccid)
                        {
                            try
                            {
                                if (ccEmailId != "" && ccEmailId != null)
                            {
                                mailMessage.CC.Add(new MailAddress(ccEmailId)); //Adding Multiple CC email Id
                            }

                            }
                            catch(Exception egx){ }
                    }
                        string[] bccid1 = BCC.Split(',');
                        foreach (string bccEmailId1 in bccid1)
                        {
                            try
                            {
                                if (bccEmailId1 != "" && bccEmailId1 != null)
                            {
                                mailMessage.Bcc.Add(new MailAddress(bccEmailId1)); //Adding Multiple BCC email Id
                            }
                            }
                            catch (Exception egx) { }
                        }
                    }
                }
                //HttpContext.Current.Session.Add("Mailmsg", mailMessage);
                //smtp.Host = dt_send_email.Rows[0]["Host"].ToString();
                //smtp.EnableSsl = true;
                //System.Net.NetworkCredential NetworkCred = new System.Net.NetworkCredential(from, password);
                //smtp.UseDefaultCredentials = true;
                //smtp.Credentials = NetworkCred;
                //smtp.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
                //smtp.Port = Convert.ToInt32(dt_send_email.Rows[0]["Port"].ToString());
                //try
                //{
                //    smtp.Send(mailMessage);
                //    msg = "Success";
                //}


                from = dt_send_email.Rows[0]["Email_Convey_from"].ToString();
                password = dt_send_email.Rows[0]["Password"].ToString();
                smtp.Host = dt_send_email.Rows[0]["Host"].ToString();
                mailMessage.From = new MailAddress(from);
                smtp.EnableSsl = true;
                NetworkCredential NetworkCred = new System.Net.NetworkCredential(from, password);
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = NetworkCred;
                smtp.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
                smtp.Port = Convert.ToInt32(dt_send_email.Rows[0]["Port"].ToString());
                try
                {
                    string vl = " " + from + " and password:" + password + " and host:" + smtp.Host;
                    CompanyInfo.InsertrequestLogTracker("Step 1 sendmoney Details from: " + vl , 0, 0, 0, 0, "BeneficiaryInfo", Convert.ToInt32(0), Convert.ToInt32(0), "", context);


                    smtp.Send(mailMessage);
                    msg = "Success";
                }


                catch (Exception ex)
                {
                    msg = ex.ToString();
                    InsertActivityLogDetails("App - " + msg, 1, 1, 1, 1, "Send-Email Sending", 2, 1, "Send Money", context);
                    try
                    {
                        string from1 = string.Empty;
                        string password1 = string.Empty;
                        //get send email details
                        MySqlConnector.MySqlCommand _cmd1 = new MySqlConnector.MySqlCommand("Get_Email_Configuration");
                        _cmd1.CommandType = CommandType.StoredProcedure;
                        string _whereclause1 = " and Client_ID=" + client_id + " and   priority=2";

                        _cmd1.Parameters.AddWithValue("_whereclause", _whereclause1);
                        _cmd1.Parameters.AddWithValue("_SecurityKey", CompanyInfo.SecurityKey());
                        DataTable dt_send_email1 = db_connection.ExecuteQueryDataTableProcedure(_cmd1);

                        if (dt_send_email1.Rows.Count > 0)
                        {

                            from1 = dt_send_email1.Rows[0]["Email_Convey_from"].ToString();
                            password1 = dt_send_email1.Rows[0]["Password"].ToString();
                            smtp.Host = dt_send_email1.Rows[0]["Host"].ToString();
                            mailMessage.From = new MailAddress(from1);
                            smtp.EnableSsl = true;
                            NetworkCred = new System.Net.NetworkCredential(from1, password1);
                            smtp.UseDefaultCredentials = false;
                            smtp.Credentials = NetworkCred;
                            smtp.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
                            smtp.Port = Convert.ToInt32(dt_send_email1.Rows[0]["Port"].ToString());
                            try
                            {
                                string  vl = " " + from1 + " and password:" + password1 + " and host:" + smtp.Host;
                                CompanyInfo.InsertrequestLogTracker("Step 2 sendmoney Details from: " + vl, 0, 0, 0, 0, "BeneficiaryInfo", Convert.ToInt32(0), Convert.ToInt32(0), "", context);

                                smtp.Send(mailMessage);
                                msg = "Success";
                            }
                            catch (Exception ex2)
                            {
                                string from2 = string.Empty;
                                string password2 = string.Empty;
                                MySqlConnector.MySqlCommand _cmd2 = new MySqlConnector.MySqlCommand("Get_Email_Configuration");
                                _cmd2.CommandType = CommandType.StoredProcedure;
                                string _whereclause2 = " and Client_ID=" + client_id + " and   priority=3";

                                _cmd2.Parameters.AddWithValue("_whereclause", _whereclause2);
                                _cmd2.Parameters.AddWithValue("_SecurityKey", CompanyInfo.SecurityKey());
                                DataTable dt_send_email2 = db_connection.ExecuteQueryDataTableProcedure(_cmd2);


                                if (dt_send_email2.Rows.Count > 0)
                                {

                                    from2 = dt_send_email2.Rows[0]["Email_Convey_from"].ToString();
                                    password2 = dt_send_email2.Rows[0]["Password"].ToString();
                                    smtp.Host = dt_send_email2.Rows[0]["Host"].ToString();
                                    smtp.EnableSsl = true;
                                    NetworkCred = new System.Net.NetworkCredential(from2, password2);
                                    smtp.UseDefaultCredentials = true;
                                    mailMessage.From = new MailAddress(from2);
                                    smtp.Credentials = NetworkCred;
                                    smtp.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
                                    smtp.Port = Convert.ToInt32(dt_send_email2.Rows[0]["Port"].ToString());
                                    try
                                    {
                                        string vl = " " + from2 + " and password:" + password2 + " and host:" + smtp.Host;
                                        CompanyInfo.InsertrequestLogTracker("Step 3 sendmoney Details from: " + vl, 0, 0, 0, 0, "BeneficiaryInfo", Convert.ToInt32(0), Convert.ToInt32(0), "", context);

                                        smtp.Send(mailMessage);
                                        msg = "Success";
                                    }
                                    catch (Exception ex3)
                                    {
                                        msg = ex3.ToString();
                                        InsertActivityLogDetails("App - " + msg, 1, 1, 1, 1, "Send-Email Sending", 2, 1, "Send Money", context);
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex1)
                    {
                        msg = ex1.ToString();
                        InsertActivityLogDetails("App - " + msg, 1, 1, 1, 1, "Send-Email Sending", 2, 1, "Send Money", context);
                    }
                    //string stattus = (string)mtsmethods.InsertErrorLogDetails(ex.Message, 1, "Send_Mail", branch_id, client_id);
                }
            }
            return msg;
        }


        /*
         * This is 14-6-2024 backup
        public static object Send_Mail(DataTable dt, string email_id, string body, string subject, int client_id, int branch_id, string flag, string CC, string BCC, HttpContext context)
        {
            string email = email_id; string from = string.Empty; string password = string.Empty;
            string msg = string.Empty;
            using (MailMessage mailMessage = new MailMessage())
            {
                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
                DataTable company_email_details = dt;
                if (dt == null || dt.Rows.Count <= 0)
                {
                    company_email_details = (DataTable)get(client_id, context);
                }
                body = body.Replace("[company_website]", Convert.ToString(company_email_details.Rows[0]["company_website"]));
                subject = subject.Replace("[company_name]", company_email_details.Rows[0]["Company_Name"].ToString());
                // string from = company_email_details.Rows[0]["Email_Convey_from"].ToString(), password = company_email_details.Rows[0]["Password"].ToString();
                body = body.Replace("[company_name]", company_email_details.Rows[0]["Company_Name"].ToString());
                //body = body.Replace("[contact_no]", company_email_details.Rows[0]["Company_mobile"].ToString());body = body.Replace("[email_id]", company_email_details.Rows[0]["Company_Email"].ToString());
                if ((company_email_details.Rows[0]["Company_mobile"] == null) || (company_email_details.Rows[0]["Company_mobile"] == ""))
                {
                    //body = body.Replace("[contact_no]", Convert.ToString(company_email_details.Rows[0]["Company_mobile"]));
                    body = body.Replace("[contact_no]", Convert.ToString("<a href='tel:" + company_email_details.Rows[0]["Company_mobile"] + "'>" + company_email_details.Rows[0]["Company_mobile"]) + "</a>");
                    body = body.Replace("[contact_id1]", "style='display:none'");
                    body = body.Replace("[or1]", "style='display:none'");
                }
                else
                {
                    //body = body.Replace("[contact_no]", Convert.ToString(company_email_details.Rows[0]["Company_mobile"]));
                    body = body.Replace("[contact_no]", Convert.ToString("<a href='tel:" + company_email_details.Rows[0]["Company_mobile"] + "'>" + company_email_details.Rows[0]["Company_mobile"]) + "</a>");

                }
                if ((company_email_details.Rows[0]["Company_Email"] == null) || (company_email_details.Rows[0]["Company_Email"] == ""))
                {
                    body = body.Replace("[email_id]", Convert.ToString(company_email_details.Rows[0]["Company_Email"]));
                    body = body.Replace("[email_id11]", "style='display:none'");
                    body = body.Replace("[or1]", "style='display:none'");
                }
                else
                {
                    body = body.Replace("[email_id]", Convert.ToString(company_email_details.Rows[0]["Company_Email"]));
                }
                if (((company_email_details.Rows[0]["Company_mobile"] == null) || (company_email_details.Rows[0]["Company_mobile"] == "")) && ((company_email_details.Rows[0]["Company_Email"] == null) || (company_email_details.Rows[0]["Company_Email"] == "")))
                {
                    body = body.Replace("[line_id1]", "style='display:none'");
                }
                string send_mail = "mailto:" + company_email_details.Rows[0]["Company_Email"].ToString();
                body = body.Replace("[email_id1]", send_mail);
                body = body.Replace("[privacy_policy]", company_email_details.Rows[0]["privacy_policy"].ToString());
                body = body.Replace("[company_logo]", company_email_details.Rows[0]["Email_company_image"].ToString());
                body = body.Replace("[customer_website]", Convert.ToString(company_email_details.Rows[0]["Company_URL_Customer"]));
                body = body.Replace("[company_addresss]", Convert.ToString(company_email_details.Rows[0]["Company_Address"]));
                body = body.Replace("[company_reg_no]", Convert.ToString(company_email_details.Rows[0]["CompanyReg_No"]));
                body = body.Replace("[company_reg_office]", Convert.ToString(company_email_details.Rows[0]["Company_Address"]));
                body = body.Replace("[company_ref_no].", Convert.ToString(company_email_details.Rows[0]["CompanyRef_No"]));
                body = body.Replace("[email]", email);
                string currentYear = Convert.ToString(DateTime.Now.Year);
                body = body.Replace("©", "© " + currentYear);
                //for bank transfer
                body = body.Replace("[wire1]", Convert.ToString(company_email_details.Rows[0]["AccountHolderName"]));
                body = body.Replace("[wire2]", Convert.ToString(company_email_details.Rows[0]["BankName"]));
                body = body.Replace("[wire3]", Convert.ToString(company_email_details.Rows[0]["AccountNumber"]));
                body = body.Replace("[wire5]", Convert.ToString(company_email_details.Rows[0]["Sort_ID"]));

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

                #region social-media-icons

                if (Convert.ToString(company_email_details.Rows[0]["Playstore"]) == null || (Convert.ToString(company_email_details.Rows[0]["Playstore"]) == ""))
                {
                    body = body.Replace("[Playstore-social]", "");
                }
                else
                {
                    body = body.Replace("[Playstore-social]", "<td class='img' width='40'style='font-size:0pt; line-height:0pt; text-align:left;image-rendering:auto!important;'><a href=" + Convert.ToString(company_email_details.Rows[0]["Playstore"]) + " target='_blank'><img src='" + Convert.ToString(company_email_details.Rows[0]["Company_URL_Customer"]) + "/assets/img/social-media-icons/google-play-store.png' width='100' height='30' style='max-width:100px; border='0' alt='Google Paly Store' /></a></td>");
                }
                if (Convert.ToString(company_email_details.Rows[0]["Appstore"]) == null || (Convert.ToString(company_email_details.Rows[0]["Appstore"]) == ""))
                {
                    body = body.Replace("[Appstore-social]", "");
                }
                else
                {
                    body = body.Replace("[Appstore-social]", "<td class='img' width='40'style='font-size:0pt; line-height:0pt; text-align:left;image-rendering:auto!important;'><a href=" + Convert.ToString(company_email_details.Rows[0]["Appstore"]) + " target='_blank'><img src='" + Convert.ToString(company_email_details.Rows[0]["Company_URL_Customer"]) + "/assets/img/social-media-icons/app-store.png' width='100' height='30' style='max-width:100px; border='0' alt='App Store' /></a></td>");
                }

                if (Convert.ToString(company_email_details.Rows[0]["Twitter"]) == null || (Convert.ToString(company_email_details.Rows[0]["Twitter"]) == ""))
                {
                    body = body.Replace("[twitter-social]", "");
                }
                else
                {
                    body = body.Replace("[twitter-social]", "<td class='img' width='40' style='font-size:0pt; line-height:0pt; text-align:left;image-rendering:auto!important;'> <a href=" + Convert.ToString(company_email_details.Rows[0]["Twitter"]) + " target='_blank'><img src='" + Convert.ToString(company_email_details.Rows[0]["Company_URL_Customer"]) + "/assets/img/social-media-icons/ico4_twitter.png' width='26' height='26' style='max-width:26px; border='0' alt='Twitter' /></a></td>");
                }

                if (Convert.ToString(company_email_details.Rows[0]["Facebook"]) == null || (Convert.ToString(company_email_details.Rows[0]["Facebook"]) == ""))
                {
                    body = body.Replace("[facebook-social]", "");
                }
                else
                {
                    body = body.Replace("[facebook-social]", "<td class='img' width='40' style='font-size:0pt; line-height:0pt; text-align:left;image-rendering:auto!important;'> <a href=" + Convert.ToString(company_email_details.Rows[0]["Facebook"]) + " target='_blank'><img src='" + Convert.ToString(company_email_details.Rows[0]["Company_URL_Customer"]) + "/assets/img/social-media-icons/ico4_facebook.png' width='26' height='26' style='max-width:26px; border='0' alt='Facebook' /></a></td>");
                }

                if (Convert.ToString(company_email_details.Rows[0]["Gmail"]) == null || (Convert.ToString(company_email_details.Rows[0]["Gmail"]) == ""))
                {
                    body = body.Replace("[GooglePluse-social]", "");
                }
                else
                {
                    body = body.Replace("[GooglePluse-social]", "<td class='img' width='40' style='font-size:0pt; line-height:0pt; text-align:left;image-rendering:auto!important;'> <a href='mailto:" + Convert.ToString(company_email_details.Rows[0]["Gmail"]) + "' target='_blank'><img src='" + Convert.ToString(company_email_details.Rows[0]["Company_URL_Customer"]) + "/assets/img/social-media-icons/ico4_gplus.png' width='26' height='26' style='max-width:26px; border='0' alt='Google Pluse' /></a></td>");
                }

                if (Convert.ToString(company_email_details.Rows[0]["Youtube"]) == null || (Convert.ToString(company_email_details.Rows[0]["Youtube"]) == ""))
                {
                    body = body.Replace("[YouTube-social]", "");
                }
                else
                {
                    body = body.Replace("[YouTube-social]", "<td class='img' width='40' style='font-size:0pt; line-height:0pt; text-align:left;image-rendering:auto!important;'> <a href=" + Convert.ToString(company_email_details.Rows[0]["Youtube"]) + " target='_blank'><img src='" + Convert.ToString(company_email_details.Rows[0]["Company_URL_Customer"]) + "/assets/img/social-media-icons/ico4_youtube.png' width='26' height='26' style='max-width:26px; border='0' alt='You Tube' /></a></td>");
                }

                if (Convert.ToString(company_email_details.Rows[0]["Instagram"]) == null || (Convert.ToString(company_email_details.Rows[0]["Instagram"]) == ""))
                {
                    body = body.Replace("[Instagram-social]", "");
                }
                else
                {
                    body = body.Replace("[Instagram-social]", "<td class='img' width='40' style='font-size:0pt; line-height:0pt; text-align:left;image-rendering:auto!important;'> <a href=" + Convert.ToString(company_email_details.Rows[0]["Instagram"]) + " target='_blank'><img src='" + Convert.ToString(company_email_details.Rows[0]["Company_URL_Customer"]) + "/assets/img/social-media-icons/ico4_instagram.png' width='26' height='26' style='max-width:26px; border='0' alt='Instagram' /></a></td>");
                }

                if (Convert.ToString(company_email_details.Rows[0]["Pinterest"]) == null || (Convert.ToString(company_email_details.Rows[0]["Pinterest"]) == ""))
                {
                    body = body.Replace("[Pinterest-social]", "");
                }
                else
                {
                    body = body.Replace("[Pinterest-social]", "<td class='img' width='40' style='font-size:0pt; line-height:0pt; text-align:left;image-rendering:auto!important;'> <a href=" + Convert.ToString(company_email_details.Rows[0]["Pinterest"]) + " target='_blank'><img src='" + Convert.ToString(company_email_details.Rows[0]["Company_URL_Customer"]) + "/assets/img/social-media-icons/ico4_pinterest.png' width='26' height='26' style='max-width:26px; border='0' alt='Pinterest' /></a></td>");
                }
                if (Convert.ToString(company_email_details.Rows[0]["Linkedin"]) == null || (Convert.ToString(company_email_details.Rows[0]["Linkedin"]) == ""))
                {
                    body = body.Replace("[Linkedin-social]", "");
                }
                else
                {
                    body = body.Replace("[Linkedin-social]", "<td class='img' width='40' style='font-size:0pt; line-height:0pt; text-align:left;image-rendering:auto!important;'> <a href=" + Convert.ToString(company_email_details.Rows[0]["Linkedin"]) + " target='_blank'><img src='" + Convert.ToString(company_email_details.Rows[0]["Company_URL_Customer"]) + "/assets/img/social-media-icons/ico4_linkedin.png' width='26' height='26' style='max-width:26px; border='0' alt='Linkedin' /></a></td>");
                }

                if (Convert.ToString(company_email_details.Rows[0]["Whats_app_no"]) == null || (Convert.ToString(company_email_details.Rows[0]["Whats_app_no"]) == ""))
                {
                    body = body.Replace("[Whatsapp-social]", "");
                }
                else
                {
                    body = body.Replace("[Whatsapp-social]", "<td class='img' width='40' style='font-size:0pt; line-height:0pt; text-align:left;image-rendering:auto!important;'> <a href=' https://api.whatsapp.com/send?phone=" + Convert.ToString(company_email_details.Rows[0]["Whats_app_no"]) + "&text=Hello There, I would like to enquire about money transfer.' target='_blank'><img src='" + Convert.ToString(company_email_details.Rows[0]["Company_URL_Customer"]) + "/assets/img/social-media-icons/ico4_whatsapp.png'  width='26' height='26' style='max-width:26px;' border='0' alt='Whatsapp' /></a></td>");
                }
                #endregion

                mailMessage.From = new MailAddress(from);
                mailMessage.Body = body;
                mailMessage.Subject = subject;
                mailMessage.BodyEncoding = System.Text.Encoding.UTF8;
                mailMessage.Priority = MailPriority.High;
                mailMessage.IsBodyHtml = true;

                DataTable ds = (DataTable)getEmailPermission(client_id, 84);
                string Status = Convert.ToString(ds.Rows[0]["Status"]);
                if ((email == "" || email == null) && Status == "0")
                {
                    email = from;//company_email_details.Rows[0]["Company_Email"].ToString();
                }
                else
                {
                    string[] bccid = email.Split(',');
                    foreach (string bccEmailId in bccid)
                    {
                        if (bccEmailId != "" && bccEmailId != null)
                        {
                            mailMessage.To.Add(new MailAddress(bccEmailId));
                            //mailMessage.Bcc.Add(new MailAddress(bccEmailId)); //Adding Multiple BCC email Id
                        }
                    }
                    // mailMessage.To.Add(new MailAddress(email));
                }
                SmtpClient smtp = new SmtpClient();
                DataTable dt_get_Permission = (DataTable)getEmailPermission(client_id, 21);
                if (dt_get_Permission.Rows.Count > 0)
                {
                    string EmailID = "";
                    if (dt_get_Permission.Rows[0]["Status_ForCustomer"].ToString() == "0")//send mail to admin
                    {
                        DataTable dt_admin_Email_list = (DataTable)getAdminEmailList(client_id, branch_id);
                        if (dt_admin_Email_list.Rows.Count > 0)
                        {
                            for (int a = 0; a < dt_admin_Email_list.Rows.Count; a++)
                            {
                                string AdminEmailID = Convert.ToString(dt_admin_Email_list.Rows[a]["Email_ID"]) + ",";
                                EmailID += AdminEmailID;
                            }
                            string[] bccid = EmailID.Split(',');
                            foreach (string bccEmailId in bccid)
                            {
                                if (bccEmailId != "" && bccEmailId != null)
                                {
                                    mailMessage.Bcc.Add(new MailAddress(bccEmailId)); //Adding Multiple BCC email Id
                                }
                            }
                        }
                    }
                    if (dt_get_Permission.Rows[0]["Status_ForCustomer"].ToString() == "1")//no mail
                    {
                    }
                    if (flag == "custom_email")
                    {
                        string[] ccid = CC.Split(',');
                        foreach (string ccEmailId in ccid)
                        {
                            if (ccEmailId != "" && ccEmailId != null)
                            {
                                mailMessage.CC.Add(new MailAddress(ccEmailId)); //Adding Multiple CC email Id
                            }
                        }
                        string[] bccid1 = BCC.Split(',');
                        foreach (string bccEmailId1 in bccid1)
                        {
                            if (bccEmailId1 != "" && bccEmailId1 != null)
                            {
                                mailMessage.Bcc.Add(new MailAddress(bccEmailId1)); //Adding Multiple BCC email Id
                            }
                        }
                    }
                }
                //HttpContext.Current.Session.Add("Mailmsg", mailMessage);
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
                    msg = "Success";
                }
                catch (Exception ex)
                {
                    msg = ex.ToString();
                    InsertActivityLogDetails("App - " + msg, 1, 1, 1, 1, "Send-Email Sending", 2, 1, "Send Money", context);
                    try
                    {
                        string from1 = string.Empty;
                        string password1 = string.Empty;
                        //get send email details
                        MySqlConnector.MySqlCommand _cmd1 = new MySqlConnector.MySqlCommand("Get_Email_Configuration");
                        _cmd1.CommandType = CommandType.StoredProcedure;
                        string _whereclause1 = " and Client_ID=" + client_id + " and   priority=2";

                        _cmd1.Parameters.AddWithValue("_whereclause", _whereclause1);
                        _cmd1.Parameters.AddWithValue("_SecurityKey", CompanyInfo.SecurityKey());
                        DataTable dt_send_email1 = db_connection.ExecuteQueryDataTableProcedure(_cmd1);

                        if (dt_send_email1.Rows.Count > 0)
                        {

                            from1 = dt_send_email1.Rows[0]["Email_Convey_from"].ToString();
                            password1 = dt_send_email1.Rows[0]["Password"].ToString();
                            smtp.Host = dt_send_email1.Rows[0]["Host"].ToString();
                            mailMessage.From = new MailAddress(from1);
                            smtp.EnableSsl = true;
                            NetworkCred = new System.Net.NetworkCredential(from1, password1);
                            smtp.UseDefaultCredentials = false;
                            smtp.Credentials = NetworkCred;
                            smtp.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
                            smtp.Port = Convert.ToInt32(dt_send_email1.Rows[0]["Port"].ToString());
                            try
                            {
                                smtp.Send(mailMessage);
                                msg = "Success";
                            }
                            catch (Exception ex2)
                            {
                                string from2 = string.Empty;
                                string password2 = string.Empty;
                                MySqlConnector.MySqlCommand _cmd2 = new MySqlConnector.MySqlCommand("Get_Email_Configuration");
                                _cmd2.CommandType = CommandType.StoredProcedure;
                                string _whereclause2 = " and Client_ID=" + client_id + " and   priority=3";

                                _cmd2.Parameters.AddWithValue("_whereclause", _whereclause2);
                                _cmd2.Parameters.AddWithValue("_SecurityKey", CompanyInfo.SecurityKey());
                                DataTable dt_send_email2 = db_connection.ExecuteQueryDataTableProcedure(_cmd2);


                                if (dt_send_email2.Rows.Count > 0)
                                {

                                    from2 = dt_send_email2.Rows[0]["Email_Convey_from"].ToString();
                                    password2 = dt_send_email2.Rows[0]["Password"].ToString();
                                    smtp.Host = dt_send_email2.Rows[0]["Host"].ToString();
                                    smtp.EnableSsl = true;
                                    NetworkCred = new System.Net.NetworkCredential(from2, password2);
                                    smtp.UseDefaultCredentials = true;
                                    mailMessage.From = new MailAddress(from2);
                                    smtp.Credentials = NetworkCred;
                                    smtp.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
                                    smtp.Port = Convert.ToInt32(dt_send_email2.Rows[0]["Port"].ToString());
                                    try
                                    {
                                        smtp.Send(mailMessage);
                                        msg = "Success";
                                    }
                                    catch (Exception ex3)
                                    {
                                        msg = ex3.ToString();
                                        InsertActivityLogDetails("App - " + msg, 1, 1, 1, 1, "Send-Email Sending", 2, 1, "Send Money", context);
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex1)
                    {
                        msg = ex1.ToString();
                        InsertActivityLogDetails("App - " + msg, 1, 1, 1, 1, "Send-Email Sending", 2, 1, "Send Money", context);
                    }
                    //string stattus = (string)mtsmethods.InsertErrorLogDetails(ex.Message, 1, "Send_Mail", branch_id, client_id);
                }
            }
            return msg;
        }
        */

        #region check sanction and pep
        public static string CheckPepSanction(int client_id, int customer_id, int branch_id, int benef_id, int sanctions_check, int days, int screening_type, int nrf_sanctions_check, HttpContext context) // nrf_sanctions_check is for nrf flag //vyankatesh 17-10
        {
            int Status = 0; string Remark = string.Empty; string mail_send = string.Empty; string EmailUrl = string.Empty; string Company_Name = string.Empty;
            string Bandtext = ""; string soapResult = "";
            string Activity = "inside check pep sanction";
            string Record_Insert_DateTime = "";
            int flag1 = 0, flag2 = 0;//pep or international

            Record_Insert_DateTime = (string)CompanyInfo.gettime(client_id, context);
            DataTable dtApi = new DataTable();
            //string soapResult = "";
            try
            {
                int API_ID = 0;
                int PID = 0;
                if (screening_type == 1)//rekyc
                {
                    API_ID = 1;
                    PID = 32;
                }
                else
                {
                    API_ID = 8;
                    PID = 132;
                }
                #region check customer check

                MySqlCommand cmd = new MySqlCommand("get_thirdpartyReqRes");//("GetAPIDetails");
                cmd.CommandType = CommandType.StoredProcedure;
                if (benef_id > 0)
                {
                    cmd.Parameters.AddWithValue("_whereclause", " and Response_Code != 0 and Beneficiary_ID = " + benef_id + "  and API_ID = " + API_ID + "  order by 1 desc limit 1");

                }
                else if (customer_id > 0)
                {
                    cmd.Parameters.AddWithValue("_whereclause", " and Response_Code != 0  and Beneficiary_ID = 0 and API_ID = " + API_ID + " and Customer_ID = " + customer_id + " order by 1 desc limit 1");

                }
                DataTable dtdoccheck = db_connection.ExecuteQueryDataTableProcedure(cmd);
                if (dtdoccheck.Rows.Count > 0)
                {
                    if (DateTime.TryParse(Convert.ToString(dtdoccheck.Rows[0]["Record_Insert_Datetime"]), out DateTime date1) && DateTime.TryParse(Record_Insert_DateTime, out DateTime date2))
                    {
                        TimeSpan difference = date2 - date1;
                        int daysDifference = (int)difference.TotalDays;
                        Activity += " sanctions_check : " + sanctions_check + " daysDifference : " + daysDifference;


                        if (sanctions_check == 2 && daysDifference > 90)// Upload new Id for ReKyc 
                        {
                            if (screening_type == 1)//rekyc
                            {
                                return Convert.ToString(0);
                            }
                        }
                        if (sanctions_check == 3 && daysDifference > days)// Upload new Id for ReKyc 
                        {
                            if (screening_type == 1)//rekyc
                            {
                                return Convert.ToString(0);
                            }
                        }

                    }
                }
                else
                {

                    Activity += " dtdoccheck : " + dtdoccheck.Rows.Count;
                    if (screening_type == 1)//rekyc
                    {
                        return Convert.ToString(0);
                    }
                }
                #endregion



                cmd = new MySqlCommand("check_sanctions_flag");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("_Customer_Id", customer_id);
                cmd.Parameters.AddWithValue("_Beneficiary_Id", benef_id);
                cmd.Parameters.AddWithValue("_PID", PID);
                DataTable pm = db_connection.ExecuteQueryDataTableProcedure(cmd);

                if (screening_type == 1)//rekyc
                {


                    if (pm.Rows.Count > 0)
                    {
                        if (Convert.ToString(pm.Rows[0]["Status"]) == "1")
                        {
                            Activity += "GBG AML Check pid = 32 is off ";
                            return Convert.ToString(4);
                        }
                        if (Convert.ToString(pm.Rows[0]["Flag"]) == "4")
                        {
                            Activity += "Check GBG is off profile compliance profile compliance ";
                            return Convert.ToString(4);
                        }
                    }
                    else
                    {
                        Activity += " pm Table is empty ";
                        return Convert.ToString(4);
                    }
                    API_ID = 1;
                    if (benef_id == 0)
                    {
                        cmd = new MySqlCommand("GetAPIDetails_byCountry");//("GetAPIDetails");
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("_API_ID", API_ID);//Credit Safe API ID
                        cmd.Parameters.AddWithValue("_Client_ID", client_id);
                        cmd.Parameters.AddWithValue("_status", 0);// API Status
                        cmd.Parameters.AddWithValue("_Customer_ID", customer_id);//for country
                        dtApi = db_connection.ExecuteQueryDataTableProcedure(cmd);
                        Activity += " screening_type: " + screening_type + " benef_id: " + benef_id + " dtApi: " + dtApi.Rows.Count;
                    }
                }
                else //aml and sanctions
                {

                    if (pm.Rows.Count > 0)
                    {
                        if (Convert.ToString(pm.Rows[0]["Status"]) == "1")
                        {
                            Activity += "PEP and Saction Permission pid = 132 is off ";
                            return Convert.ToString(4);
                        }
                        if (Convert.ToString(pm.Rows[0]["Flag"]) == "4")
                        {
                            Activity += "Check PEP and Sanctions is off in profile compliance ";
                            return Convert.ToString(4);
                        }
                    }
                    API_ID = 8;
                    cmd = new MySqlCommand("GetAPIDetails");
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("_API_ID", API_ID);//Click Send API ID
                    cmd.Parameters.AddWithValue("_Client_ID", 1);
                    cmd.Parameters.AddWithValue("_status", 0);
                    dtApi = db_connection.ExecuteQueryDataTableProcedure(cmd);
                    Activity += " screening_type: " + screening_type + " benef_id: " + benef_id + " dtApi: " + dtApi.Rows.Count;
                }

                //get Customer details
                MySqlCommand cmd3 = new MySqlCommand();
                string _whereclause = " ";
                if (benef_id > 0)// for beneficiary 
                {
                    cmd3 = new MySqlCommand("Get_benef_sanctions");
                    cmd3.CommandType = CommandType.StoredProcedure;

                    _whereclause = " and bb.Beneficiary_ID = " + benef_id;

                    cmd3.Parameters.AddWithValue("_whereclause", _whereclause);
                    cmd3.Parameters.AddWithValue("_SecurityKey", CompanyInfo.SecurityKey());
                }
                else if (customer_id > 0)// for customer
                {
                    cmd3 = new MySqlCommand("customer_details_by_param");
                    cmd3.CommandType = CommandType.StoredProcedure;
                    _whereclause = " and cr.Client_ID=" + client_id;
                    if (customer_id > 0)
                    {
                        _whereclause = " and cr.Customer_ID=" + customer_id;
                    }
                    cmd3.Parameters.AddWithValue("_whereclause", _whereclause);
                    cmd3.Parameters.AddWithValue("_SecurityKey", CompanyInfo.SecurityKey());
                }

                DataTable dc = db_connection.ExecuteQueryDataTableProcedure(cmd3);
                Activity += "dc count :" + dc.Rows.Count;
                if (benef_id > 0 && screening_type == 1)
                {
                    MySqlCommand cmd1 = new MySqlCommand("Get_API_fromCountry");//("GetAPIDetails");
                    cmd1.CommandType = CommandType.StoredProcedure;
                    cmd1.Parameters.AddWithValue("_API_ID", API_ID);//Credit Safe API ID
                    cmd1.Parameters.AddWithValue("_Client_ID", client_id);
                    cmd1.Parameters.AddWithValue("_status", 0);// API Status
                    cmd1.Parameters.AddWithValue("_Country_ID", dc.Rows[0]["Country_ID"]);//for country
                    dtApi = db_connection.ExecuteQueryDataTableProcedure(cmd1);
                    Activity += "dtApi count :" + dtApi.Rows.Count;
                }
                if (dtApi.Rows.Count > 0)
                {
                    //if (dc.Rows.Count > 0)
                    //{
                    //    if (dc.Rows[0]["SenderID_Number"] == DBNull.Value)
                    //    {
                    //        return Convert.ToString(Status);
                    //    }
                    //}
                    //else { 
                    //return Convert.ToString(Status);
                    //}


                    cmd = new MySqlCommand("get_thirdpartyReqRes");//("GetAPIDetails");
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (benef_id > 0)
                    {
                        cmd.Parameters.AddWithValue("_whereclause", " and Response_Code != 0 and Beneficiary_ID = " + benef_id + "  and API_ID = " + API_ID + "  order by 1 desc limit 1");

                    }
                    else if (customer_id > 0)
                    {
                        if (screening_type == 1)//rekyc
                        {
                            cmd.Parameters.AddWithValue("_whereclause", " and Response_Code != 0  and Beneficiary_ID = 0 and API_ID = " + API_ID + " and Customer_ID = " + customer_id + " order by 1 desc limit 1");

                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("_whereclause", " and Response_Code != 0  and Beneficiary_ID = 0 and API_ID in (1,8) and Customer_ID = " + customer_id + " order by 1 desc limit 1");


                        }

                    }
                    dtdoccheck = db_connection.ExecuteQueryDataTableProcedure(cmd);
                    Activity += " dtdoccheck : " + dtdoccheck.Rows.Count;
                    if (dtdoccheck.Rows.Count > 0)
                    {
                        if (DateTime.TryParse(Convert.ToString(dtdoccheck.Rows[0]["Record_Insert_Datetime"]), out DateTime date1) && DateTime.TryParse(Record_Insert_DateTime, out DateTime date2))
                        {

                            TimeSpan difference = date2 - date1;
                            int daysDifference = (int)difference.TotalDays;
                            Activity += " sanctions_check : " + sanctions_check + " daysDifference : " + daysDifference;
                            // Check if the difference is 90 days
                            if (sanctions_check == 2 && daysDifference < 90)
                            {
                                soapResult = Convert.ToString(dtdoccheck.Rows[0]["Parameter"]).Replace("''", "'");

                                if (soapResult != "" && soapResult != null)
                                {
                                    XmlDocument xmlDoc = new XmlDocument();
                                    xmlDoc.LoadXml(soapResult);

                                    XmlNodeList nodeList = xmlDoc.GetElementsByTagName("AuthenticateSPResult");
                                    foreach (XmlNode node1 in nodeList)
                                    {
                                        foreach (XmlNode child in node1.ChildNodes)
                                        {
                                            if (child.Name == "BandText")
                                            {
                                                Bandtext = child.InnerText;
                                            }
                                        }
                                    }
                                    XmlNodeList responsestring = xmlDoc.GetElementsByTagName("GlobalItemCheckResultCodes");
                                    foreach (XmlNode node in responsestring)
                                    {
                                        string name = "";// node1["Name"].InnerText.ToString();
                                        foreach (XmlNode child in node.ChildNodes)
                                        {
                                            if (node.InnerText.Contains("PEP"))
                                            {
                                                if (child.Name == "Mismatch")
                                                {
                                                    foreach (XmlNode childd in child.ChildNodes)
                                                    {
                                                        string ss = childd.InnerXml;
                                                        string code1 = childd["Code"].InnerText.ToString();
                                                        if (code1 == "9500")
                                                        {
                                                            Activity += " PEP : found";
                                                            string description1 = childd["Description"].InnerText.ToString();
                                                            string Record_DateTime = Record_Insert_DateTime;
                                                            string notification_icon = "aml-referd.jpg";
                                                            //string notification_message = "<span class='cls-admin'> International PEP Alert - <strong class='cls-cancel'></strong><br/>" + description1 + "</span><span class='cls-customer'></span>";
                                                            string notification_message = "<span class='cls-admin'> - <span class='cls-cancel'><strong> International PEP Alert </strong>- </span><span class='cls-addr-confirm'>" + description1 + " </span></span></span><span class='cls-customer'></span>";
                                                            CompanyInfo.save_notification_compliance(notification_message, notification_icon, Convert.ToString(customer_id), Convert.ToDateTime(Record_DateTime), client_id, 1, 1, branch_id, 0, 1, 1, 0, context);
                                                            flag1++;

                                                        }
                                                    }
                                                }
                                            }
                                            else if (node.InnerText.Contains("Global Sanctions") || node.InnerText.Contains("International Sanctions"))
                                            {
                                                if (child.Name == "Mismatch")
                                                {
                                                    foreach (XmlNode childd in child.ChildNodes)
                                                    {
                                                        Activity += " Global Sanctions : found";
                                                        string description1 = childd["Description"].InnerText.ToString();
                                                        string Record_DateTime = Record_Insert_DateTime;
                                                        string notification_icon = "pep-match-not-found.jpg";
                                                        //  string notification_message = "<span class='cls-admin'>International Sanctions: <strong class='cls-cancel'>.</strong><br/>" + description1 + "</span><span class='cls-customer'></span>";
                                                        string notification_message = "<span class='cls-admin'> - <span class='cls-cancel'><strong> International Sanctions Alert </strong>- </span><span class='cls-addr-confirm'>" + description1 + " </span></span></span><span class='cls-customer'></span>";
                                                        CompanyInfo.save_notification_compliance(notification_message, notification_icon, Convert.ToString(customer_id), Convert.ToDateTime(Record_DateTime), client_id, 1, 1, branch_id, 0, 1, 1, 0, context);
                                                        flag2++;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    if (flag1 > 0 && flag2 > 0)
                                    {

                                        Status = 3;
                                    }
                                    else if (flag1 > 0)
                                    {

                                        Status = 1;
                                    }
                                    else if (flag2 > 0)
                                    {

                                        Status = 2;
                                    }
                                    else if (flag1 == 0 && flag2 == 0)
                                    {

                                        Status = 4;
                                    }

                                    try
                                    {
                                        int status = 1; string Function_name = "CheckPepSanction";
                                        Remark = Convert.ToString(CompanyInfo.getAPIStatus(Bandtext, client_id));
                                        //if (Remark != null && Remark != "")
                                        //    Status = 0;

                                        MySqlCommand _cmd = new MySqlCommand("UpdateCustomerBandText");
                                        _cmd.CommandType = CommandType.StoredProcedure;
                                        _cmd.Parameters.AddWithValue("_Client_ID", client_id);
                                        _cmd.Parameters.AddWithValue("_Customer_ID", customer_id);
                                        _cmd.Parameters.AddWithValue("_Remark", Remark);
                                        int msg = db_connection.ExecuteNonQueryProcedure(_cmd);
                                        _cmd.Dispose();



                                    }
                                    catch (Exception ex)
                                    {
                                        Activity += " Exception 1 " + ex.ToString();
                                        string error = ex.ToString().Replace("\'", "\\'");

                                        MySqlCommand _cmd = new MySqlCommand("SaveException");
                                        _cmd.CommandType = CommandType.StoredProcedure;
                                        _cmd.Parameters.AddWithValue("_Record_Insert_DateTime", Record_Insert_DateTime);
                                        _cmd.Parameters.AddWithValue("_error", error);
                                        _cmd.Parameters.AddWithValue("_Client_ID", client_id);
                                        int msg1 = db_connection.ExecuteNonQueryProcedure(_cmd);
                                    }
                                    //if (Remark == "2" || Remark == "1")//alert or refer then  send mail to admin



                                }
                                return Convert.ToString(Status);// already checked in previous 90 days
                            }
                            if (sanctions_check == 3 && daysDifference < days)
                            {
                                soapResult = Convert.ToString(dtdoccheck.Rows[0]["Parameter"]).Replace("''", "'");

                                if (soapResult != "" && soapResult != null)
                                {
                                    XmlDocument xmlDoc = new XmlDocument();
                                    xmlDoc.LoadXml(soapResult);

                                    XmlNodeList nodeList = xmlDoc.GetElementsByTagName("AuthenticateSPResult");
                                    foreach (XmlNode node1 in nodeList)
                                    {
                                        foreach (XmlNode child in node1.ChildNodes)
                                        {
                                            if (child.Name == "BandText")
                                            {
                                                Bandtext = child.InnerText;
                                            }
                                        }
                                    }
                                    XmlNodeList responsestring = xmlDoc.GetElementsByTagName("GlobalItemCheckResultCodes");
                                    foreach (XmlNode node in responsestring)
                                    {
                                        string name = "";// node1["Name"].InnerText.ToString();
                                        foreach (XmlNode child in node.ChildNodes)
                                        {
                                            if (node.InnerText.Contains("PEP"))
                                            {
                                                if (child.Name == "Mismatch")
                                                {
                                                    foreach (XmlNode childd in child.ChildNodes)
                                                    {
                                                        string ss = childd.InnerXml;
                                                        string code1 = childd["Code"].InnerText.ToString();
                                                        if (code1 == "9500")
                                                        {
                                                            Activity += " PEP : found";
                                                            string description1 = childd["Description"].InnerText.ToString();
                                                            string Record_DateTime = Record_Insert_DateTime;
                                                            string notification_icon = "aml-referd.jpg";
                                                            //string notification_message = "<span class='cls-admin'> International PEP Alert - <strong class='cls-cancel'></strong><br/>" + description1 + "</span><span class='cls-customer'></span>";
                                                            string notification_message = "<span class='cls-admin'> - <span class='cls-cancel'><strong> International PEP Alert </strong>- </span><span class='cls-addr-confirm'>" + description1 + " </span></span></span><span class='cls-customer'></span>";
                                                            CompanyInfo.save_notification_compliance(notification_message, notification_icon, Convert.ToString(customer_id), Convert.ToDateTime(Record_DateTime), client_id, 1, 1, branch_id, 0, 1, 1, 0, context);
                                                            flag1++;

                                                        }
                                                    }
                                                }
                                            }
                                            else if (node.InnerText.Contains("Global Sanctions") || node.InnerText.Contains("International Sanctions"))
                                            {
                                                if (child.Name == "Mismatch")
                                                {
                                                    foreach (XmlNode childd in child.ChildNodes)
                                                    {
                                                        Activity += " Global Sanctions : found";
                                                        string description1 = childd["Description"].InnerText.ToString();
                                                        string Record_DateTime = Record_Insert_DateTime;
                                                        string notification_icon = "pep-match-not-found.jpg";
                                                        //notification_message = "<span class='cls-admin'> - <span class='cls-cancel'><strong> International Sanctions Alert  </strong>- </span><span class='cls-addr-confirm'>" + description1 + " </span></span></span><span class='cls-customer'></span>";
                                                        string notification_message = "<span class='cls-admin'> - <span class='cls-cancel'><strong> International Sanctions Alert </strong>- </span><span class='cls-addr-confirm'>" + description1 + " </span></span></span><span class='cls-customer'></span>";
                                                        CompanyInfo.save_notification_compliance(notification_message, notification_icon, Convert.ToString(customer_id), Convert.ToDateTime(Record_DateTime), client_id, 1, 1, branch_id, 0, 1, 1, 0, context);
                                                        flag2++;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    if (flag1 > 0 && flag2 > 0)
                                    {

                                        Status = 3;
                                    }
                                    else if (flag1 > 0)
                                    {

                                        Status = 1;
                                    }
                                    else if (flag2 > 0)
                                    {

                                        Status = 2;
                                    }
                                    else if (flag1 == 0 && flag2 == 0)
                                    {

                                        Status = 4;
                                    }

                                    try
                                    {
                                        int status = 1; string Function_name = "CheckPepSanction";
                                        Remark = Convert.ToString(CompanyInfo.getAPIStatus(Bandtext, client_id));
                                        //if (Remark != null && Remark != "")
                                        //    Status = 0;

                                        MySqlCommand _cmd = new MySqlCommand("UpdateCustomerBandText");
                                        _cmd.CommandType = CommandType.StoredProcedure;
                                        _cmd.Parameters.AddWithValue("_Client_ID", client_id);
                                        _cmd.Parameters.AddWithValue("_Customer_ID", customer_id);
                                        _cmd.Parameters.AddWithValue("_Remark", Remark);
                                        int msg = db_connection.ExecuteNonQueryProcedure(_cmd);
                                        _cmd.Dispose();



                                    }
                                    catch (Exception ex)
                                    {
                                        Activity += " Exception 1 " + ex.ToString();
                                        string error = ex.ToString().Replace("\'", "\\'");

                                        MySqlCommand _cmd = new MySqlCommand("SaveException");
                                        _cmd.CommandType = CommandType.StoredProcedure;
                                        _cmd.Parameters.AddWithValue("_Record_Insert_DateTime", Record_Insert_DateTime);
                                        _cmd.Parameters.AddWithValue("_error", error);
                                        _cmd.Parameters.AddWithValue("_Client_ID", client_id);
                                        int msg1 = db_connection.ExecuteNonQueryProcedure(_cmd);
                                    }
                                    //if (Remark == "2" || Remark == "1")//alert or refer then  send mail to admin



                                }
                                return Convert.ToString(Status);// already checked in previous custom days
                            }
                            try
                            {
                                if (nrf_sanctions_check == 1)//check only once //NRF   //vyankatesh 17-10-24
                                {
                                    MySqlCommand _cmd1 = new MySqlCommand("CheckIDExpiry");
                                    _cmd1.CommandType = CommandType.StoredProcedure;
                                    _cmd1.Parameters.AddWithValue("_Client_ID", client_id);
                                    _cmd1.Parameters.AddWithValue("_IDType_ID", 1);
                                    _cmd1.Parameters.AddWithValue("_Customer_ID", customer_id);
                                    DataTable table = db_connection.ExecuteQueryDataTableProcedure(_cmd1);

                                    //DateTime dt3 = DateTime.Now.Date.AddDays(60);
                                    if (table.Rows.Count == 0)
                                    {
                                        soapResult = Convert.ToString(dtdoccheck.Rows[0]["Parameter"]).Replace("''", "'");

                                        if (soapResult != "" && soapResult != null)
                                        {
                                            XmlDocument xmlDoc = new XmlDocument();
                                            xmlDoc.LoadXml(soapResult);

                                            XmlNodeList nodeList = xmlDoc.GetElementsByTagName("AuthenticateSPResult");
                                            foreach (XmlNode node1 in nodeList)
                                            {
                                                foreach (XmlNode child in node1.ChildNodes)
                                                {
                                                    if (child.Name == "BandText")
                                                    {
                                                        Bandtext = child.InnerText;
                                                    }
                                                }
                                            }
                                            XmlNodeList responsestring = xmlDoc.GetElementsByTagName("GlobalItemCheckResultCodes");
                                            foreach (XmlNode node in responsestring)
                                            {
                                                string name = "";// node1["Name"].InnerText.ToString();
                                                foreach (XmlNode child in node.ChildNodes)
                                                {
                                                    if (node.InnerText.Contains("PEP"))
                                                    {
                                                        if (child.Name == "Mismatch")
                                                        {
                                                            foreach (XmlNode childd in child.ChildNodes)
                                                            {
                                                                string ss = childd.InnerXml;
                                                                string code1 = childd["Code"].InnerText.ToString();
                                                                if (code1 == "9500")
                                                                {
                                                                    Activity += " PEP : found";
                                                                    string description1 = childd["Description"].InnerText.ToString();
                                                                    string Record_DateTime = Record_Insert_DateTime;
                                                                    string notification_icon = "aml-referd.jpg";
                                                                    //string notification_message = "<span class='cls-admin'> International PEP Alert - <strong class='cls-cancel'></strong><br/>" + description1 + "</span><span class='cls-customer'></span>";
                                                                    string notification_message = "<span class='cls-admin'> - <span class='cls-cancel'><strong> International PEP Alert </strong>- </span><span class='cls-addr-confirm'>" + description1 + " </span></span></span><span class='cls-customer'></span>";
                                                                    CompanyInfo.save_notification_compliance(notification_message, notification_icon, Convert.ToString(customer_id), Convert.ToDateTime(Record_DateTime), client_id, 1, 1, branch_id, 0, 1, 1, 0, context);
                                                                    flag1++;

                                                                }
                                                            }
                                                        }
                                                    }
                                                    else if (node.InnerText.Contains("Global Sanctions") || node.InnerText.Contains("International Sanctions"))
                                                    {
                                                        if (child.Name == "Mismatch")
                                                        {
                                                            foreach (XmlNode childd in child.ChildNodes)
                                                            {
                                                                Activity += " Global Sanctions : found";
                                                                string description1 = childd["Description"].InnerText.ToString();
                                                                string Record_DateTime = Record_Insert_DateTime;
                                                                string notification_icon = "pep-match-not-found.jpg";
                                                                //notification_message = "<span class='cls-admin'> - <span class='cls-cancel'><strong> International Sanctions Alert  </strong>- </span><span class='cls-addr-confirm'>" + description1 + " </span></span></span><span class='cls-customer'></span>";
                                                                string notification_message = "<span class='cls-admin'> - <span class='cls-cancel'><strong> International Sanctions Alert </strong>- </span><span class='cls-addr-confirm'>" + description1 + " </span></span></span><span class='cls-customer'></span>";
                                                                CompanyInfo.save_notification_compliance(notification_message, notification_icon, Convert.ToString(customer_id), Convert.ToDateTime(Record_DateTime), client_id, 1, 1, branch_id, 0, 1, 1, 0, context);
                                                                flag2++;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            if (flag1 > 0 && flag2 > 0)
                                            {

                                                Status = 3;
                                            }
                                            else if (flag1 > 0)
                                            {

                                                Status = 1;
                                            }
                                            else if (flag2 > 0)
                                            {

                                                Status = 2;
                                            }
                                            else if (flag1 == 0 && flag2 == 0)
                                            {

                                                Status = 4;
                                            }

                                            try
                                            {
                                                int status = 1; string Function_name = "CheckPepSanction";
                                                Remark = Convert.ToString(CompanyInfo.getAPIStatus(Bandtext, client_id));
                                                //if (Remark != null && Remark != "")
                                                //    Status = 0;

                                                MySqlCommand _cmd = new MySqlCommand("UpdateCustomerBandText");
                                                _cmd.CommandType = CommandType.StoredProcedure;
                                                _cmd.Parameters.AddWithValue("_Client_ID", client_id);
                                                _cmd.Parameters.AddWithValue("_Customer_ID", customer_id);
                                                _cmd.Parameters.AddWithValue("_Remark", Remark);
                                                int msg = db_connection.ExecuteNonQueryProcedure(_cmd);
                                                _cmd.Dispose();



                                            }
                                            catch (Exception ex)
                                            {
                                                Activity += " Exception 1 " + ex.ToString();
                                                string error = ex.ToString().Replace("\'", "\\'");

                                                MySqlCommand _cmd = new MySqlCommand("SaveException");
                                                _cmd.CommandType = CommandType.StoredProcedure;
                                                _cmd.Parameters.AddWithValue("_Record_Insert_DateTime", Record_Insert_DateTime);
                                                _cmd.Parameters.AddWithValue("_error", error);
                                                _cmd.Parameters.AddWithValue("_Client_ID", client_id);
                                                int msg1 = db_connection.ExecuteNonQueryProcedure(_cmd);
                                            }
                                            //if (Remark == "2" || Remark == "1")//alert or refer then  send mail to admin



                                        }
                                        return Convert.ToString(Status);// already checked in lifetime
                                    }
                                }
                            }
                            catch(Exception ex)
                            {
                                Activity += " Exception 1 " + ex.ToString();
                                string error = ex.ToString().Replace("\'", "\\'");

                                MySqlCommand _cmd = new MySqlCommand("SaveException");
                                _cmd.CommandType = CommandType.StoredProcedure;
                                _cmd.Parameters.AddWithValue("_Record_Insert_DateTime", Record_Insert_DateTime);
                                _cmd.Parameters.AddWithValue("_error", error);
                                _cmd.Parameters.AddWithValue("_Client_ID", client_id);
                                int msg1 = db_connection.ExecuteNonQueryProcedure(_cmd);
                            }
                        }
                    }
                    string First_Name = "";
                    string Middle_Name = "";
                    string Last_Name = "";
                    if (benef_id > 0)// for beneficary to split the name
                    {
                        var parts = Convert.ToString(dc.Rows[0]["Beneficiary_Name"]).Split();
                        switch (parts.Length)
                        {
                            case 2:
                                First_Name = parts[0];
                                Middle_Name = "";
                                Last_Name = parts[1];
                                break;
                            case 3:
                                First_Name = parts[0];
                                Middle_Name = parts[1];
                                Last_Name = parts[2];
                                break;
                            default:
                                throw new ArgumentException("Invalid name format");
                        }
                    }
                    else if (customer_id > 0)
                    {
                        First_Name = Convert.ToString(dc.Rows[0]["First_Name"]);
                        Middle_Name = Convert.ToString(dc.Rows[0]["Middle_Name"]);
                        Last_Name = Convert.ToString(dc.Rows[0]["Last_Name"]);
                    }


                    string Country1 = Convert.ToString(dc.Rows[0]["Country_Name"]).ToLower();
                    string House_Number = Convert.ToString(dc.Rows[0]["House_Number"]);
                    string Street = Convert.ToString(dc.Rows[0]["Street"]);
                    string City = Convert.ToString(dc.Rows[0]["City_Name"]);
                    string Post_Code = Convert.ToString(dc.Rows[0]["Post_Code"]);
                    //i.SenderID_ID = Convert.ToInt32(dc.Rows[0]["SenderID_ID"]);

                    var _url = Convert.ToString(dtApi.Rows[0]["API_URL"]);
                    var UserName = Convert.ToString(dtApi.Rows[0]["UserName"]);
                    var Password = Convert.ToString(dtApi.Rows[0]["Password"]);
                    var ProfileID = Convert.ToString(dtApi.Rows[0]["ProfileID"]);
                    var _action = "http://www.id3global.com/ID3gWS/2013/04/IGlobalAuthenticate/AuthenticateSP";
                    XmlDocument soapEnvelopeXml = new XmlDocument();
                    var iddoc = "";
                    string Country = Country1;
                    string passdoc = "", passid = "", endid = "";

                    if (Country == "united kingdom" || Country == "uk") { Country = "UK"; }
                    else if (Country == "new zealand") { Country = "NewZealand"; }
                    else if (Country == "us" || Country == "united states") { Country = "US"; }
                    else if (Country == "china" || Country == "india" || Country == "canada" || Country == "mexico" || Country == "brazil" || Country == "spain" || Country == "argentina")
                    {
                        Country = Country1;
                    }
                    else { Country = ""; }

                    string province = "";
                    if (Country == "canada")
                    {
                        province = "<ns:StateDistrict>" + Convert.ToString(dc.Rows[0]["code"]) + @"</ns:StateDistrict>" +
                                    "<ns:Region>" + Convert.ToString(dc.Rows[0]["code"]) + @"</ns:Region>";
                    }

                    string passexpiry = "";
                    Activity += " SenderID_ExpiryDate : " + Convert.ToString(Convert.ToString(dc.Rows[0]["SenderID_ExpiryDate"]));
                    string expiry = Convert.ToString(Convert.ToString(dc.Rows[0]["SenderID_ExpiryDate"]));
                    Activity += " SenderID_Number : " + dc.Rows[0]["SenderID_Number"];
                    if (expiry != "" && expiry != null && dc.Rows[0]["SenderID_Number"] != DBNull.Value)
                    {
                        Activity += "";
                        try
                        {
                            DateTime d = Convert.ToDateTime(expiry);
                            var day = d.Day;
                            var month = d.Month;
                            var year = d.Year;
                            passexpiry = "<ns:ExpiryDay>" + day + @"</ns:ExpiryDay>" +
                                      "<ns:ExpiryMonth>" + month + @"</ns:ExpiryMonth>" +
                                      "<ns:ExpiryYear>" + year + @"</ns:ExpiryYear>";
                        }
                        catch { }
                    }
                    if (dc.Rows[0]["SenderID_Number"] != DBNull.Value)
                    {
                        if (Convert.ToInt32(dc.Rows[0]["IDName_ID"]) == 1)// if Passport
                        {
                            passdoc += "<ns:IdentityDocuments><ns:InternationalPassport>";
                            passdoc += "<ns:Number>" + Convert.ToString(dc.Rows[0]["SenderID_Number"]) + @" </ns:Number> " + passexpiry + "";
                            if (Country != "") { passdoc += "<ns:CountryOfOrigin>" + Country + @"</ns:CountryOfOrigin>"; }
                            passdoc += "</ns:InternationalPassport></ns:IdentityDocuments>";

                        }
                        else if (Convert.ToInt32(dc.Rows[0]["IDName_ID"]) == 2 && Country != "")//if Driving licence
                        {
                            passdoc += "<ns:IdentityDocuments><ns:" + Country + @"><ns:DrivingLicence>";
                            passdoc += "<ns:Number>" + Convert.ToString(dc.Rows[0]["SenderID_Number"]) + @" </ns:Number> " + passexpiry + "";
                            passdoc += "</ns:DrivingLicence></ns:" + Country + @"></ns:IdentityDocuments>";
                        }
                        else //if (i.IDName_ID == 3)//EU Nationality Card
                        {
                            passdoc += "<ns:IdentityDocuments><ns:IdentityCard>";
                            passdoc += "<ns:Number>" + Convert.ToString(dc.Rows[0]["SenderID_Number"]) + @" </ns:Number>";
                            if (Country != "") { passdoc += "<ns:Country>" + Country + @"</ns:Country>"; }
                            passdoc += "</ns:IdentityCard></ns:IdentityDocuments>";
                        }
                    }
                    string passdob = "";
                    Activity += " DateOf_Birth " + Convert.ToString(dc.Rows[0]["DateOf_Birth"]);
                    string dob = Convert.ToString(dc.Rows[0]["DateOf_Birth"]);//(i.Sender_DateOfBirth);
                    if (dob != "" && dob != null)
                    {
                        try
                        {
                            DateTime d = Convert.ToDateTime(dob);
                            var day = d.Day;
                            var month = d.Month;
                            var year = d.Year;
                            passdob = "<ns:DOBDay>" + day + @"</ns:DOBDay>" +
                                      "<ns:DOBMonth>" + month + @"</ns:DOBMonth>" +
                                      "<ns:DOBYear>" + year + @"</ns:DOBYear>";
                        }
                        catch { }
                    }
                    string strmiddlename = "";
                    if (Middle_Name != "" && Middle_Name != null)
                    {
                        strmiddlename = "<ns:MiddleName>" + Middle_Name + @"</ns:MiddleName>";
                    }
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    ////<ns:MiddleName>A</ns:MiddleName><ns:Title>Mr</ns:Title><ns:Gender>" + Convert.ToString(dc.Rows[0]["Gender"]) + @"</ns:Gender>
                    soapEnvelopeXml.LoadXml(@"<?xml version=""1.0"" encoding=""utf-8""?>
                        <soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:ns=""http://www.id3global.com/ID3gWS/2013/04"" xmlns:arr=""http://schemas.microsoft.com/2003/10/Serialization/Arrays"">                
                              <soapenv:Header>
                                <wsse:Security soapenv:mustUnderstand='1' xmlns:wsse='http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd'>
                                    <wsse:UsernameToken wsu:Id='UsernameToken-1' xmlns:wsu='http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd'>
                                    <wsse:Username>" + UserName + @"</wsse:Username>
                                    <wsse:Password>" + Password + @"</wsse:Password>    
                                    </wsse:UsernameToken>
                                </wsse:Security>
                              </soapenv:Header>
                              <soapenv:Body>
                                    <ns:AuthenticateSP>
                                        <ns:ProfileIDVersion>
                                            <ns:ID>" + ProfileID + @"</ns:ID>
                                            <ns:Version>0</ns:Version>
                                        </ns:ProfileIDVersion>         
                                      <ns:InputData>
                                       <ns:Personal>
                                         <ns:PersonalDetails>                                    
                                            <ns:Forename>" + First_Name + @"</ns:Forename>     
                                            " + strmiddlename + @"                               
                                            <ns:Surname>" + Last_Name + @"</ns:Surname>                                    
                                            " + passdob + @"
                                         </ns:PersonalDetails>
                                       </ns:Personal>
                                       <ns:Addresses>
                                        <ns:CurrentAddress>
                                            <ns:Country>" + Country + @"</ns:Country>
                                            <ns:Street>" + Street + @"</ns:Street>
                                            <ns:City>" + City + @"</ns:City>   
                                            " + province + @"
                                            <ns:ZipPostcode>" + Post_Code + @"</ns:ZipPostcode>
                                            <ns:Building>" + House_Number + @"</ns:Building>
                                        </ns:CurrentAddress>
                                       </ns:Addresses>  
                                        " + passdoc + @"                            
                                    </ns:InputData>
                                    </ns:AuthenticateSP>                            
                              </soapenv:Body>
                         </soapenv:Envelope>
                        ");
                    string SendTransferReq = soapEnvelopeXml.InnerXml;
                    try
                    {
                        if (benef_id > 0)
                        {
                            MySqlCommand _cmd = new MySqlCommand("SaveAPIReqRespforBenf");
                            _cmd.CommandType = CommandType.StoredProcedure;
                            _cmd.Parameters.AddWithValue("_API_ID", API_ID);
                            _cmd.Parameters.AddWithValue("_Client_ID", client_id);
                            _cmd.Parameters.AddWithValue("_Customer_ID", customer_id);
                            _cmd.Parameters.AddWithValue("_status", 0);
                            _cmd.Parameters.AddWithValue("_Function_name", "CheckPepSanction");
                            _cmd.Parameters.AddWithValue("_Remark", 0);
                            _cmd.Parameters.AddWithValue("_comments", SendTransferReq.Replace("'", "''"));
                            _cmd.Parameters.AddWithValue("_Record_Insert_DateTime", Record_Insert_DateTime);
                            _cmd.Parameters.AddWithValue("_Branch_ID", branch_id);
                            _cmd.Parameters.AddWithValue("_Beneficiary_ID", benef_id);
                            int msg1 = db_connection.ExecuteNonQueryProcedure(_cmd);
                        }
                        else if (customer_id > 0)
                        {
                            MySqlCommand _cmd = new MySqlCommand("SaveAPIRequestResponce");
                            _cmd.CommandType = CommandType.StoredProcedure;
                            _cmd.Parameters.AddWithValue("_API_ID", API_ID);
                            _cmd.Parameters.AddWithValue("_Client_ID", client_id);
                            _cmd.Parameters.AddWithValue("_Customer_ID", customer_id);
                            _cmd.Parameters.AddWithValue("_status", 0);
                            _cmd.Parameters.AddWithValue("_Function_name", "CheckPepSanction");
                            _cmd.Parameters.AddWithValue("_Remark", 0);
                            _cmd.Parameters.AddWithValue("_comments", SendTransferReq.Replace("'", "''"));
                            _cmd.Parameters.AddWithValue("_Record_Insert_DateTime", Record_Insert_DateTime);
                            _cmd.Parameters.AddWithValue("_Branch_ID", branch_id);
                            int msg1 = db_connection.ExecuteNonQueryProcedure(_cmd);
                        }

                    }
                    catch (Exception ex)
                    {
                        string error = ex.ToString().Replace("\'", "\\'");

                        MySqlCommand _cmd = new MySqlCommand("SaveException");
                        _cmd.CommandType = CommandType.StoredProcedure;
                        _cmd.Parameters.AddWithValue("_Record_Insert_DateTime", Record_Insert_DateTime);
                        _cmd.Parameters.AddWithValue("_error", error);
                        _cmd.Parameters.AddWithValue("_Client_ID", client_id);
                        int msg1 = db_connection.ExecuteNonQueryProcedure(_cmd);
                    }

                    HttpWebRequest webRequest = CreateWebRequest(_url, _action);
                    InsertSoapEnvelopeIntoWebRequest(soapEnvelopeXml, webRequest);
                    IAsyncResult asyncResult = webRequest.BeginGetResponse(null, null);
                    asyncResult.AsyncWaitHandle.WaitOne();

                    try
                    {
                        using (WebResponse webResponse = webRequest.GetResponse())
                        {
                            using (StreamReader rd = new StreamReader(webResponse.GetResponseStream()))
                            {
                                soapResult = rd.ReadToEnd();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Activity += " Exception : " + ex.ToString().Replace("\'", "\\'");
                        string error = ex.ToString().Replace("\'", "\\'");
                        MySqlCommand _cmd = new MySqlCommand("SaveException");
                        _cmd.CommandType = CommandType.StoredProcedure;
                        _cmd.Parameters.AddWithValue("_Record_Insert_DateTime", Record_Insert_DateTime);
                        _cmd.Parameters.AddWithValue("_error", error);
                        _cmd.Parameters.AddWithValue("_Client_ID", client_id);
                        int msg1 = db_connection.ExecuteNonQueryProcedure(_cmd);


                        DataTable dtc = CompanyInfo.get(client_id, context);
                        mail_send = string.Empty;
                        string sendmsg = "GBG AML check Request failed. Please contact GBG support team. ";
                        string EmailID = "";
                        DataTable dt_admin_Email_list = (DataTable)CompanyInfo.getAdminEmailList(client_id, 2);
                        if (dt_admin_Email_list.Rows.Count > 0)
                        {
                            for (int a = 0; a < dt_admin_Email_list.Rows.Count; a++)
                            {
                                string AdminEmailID = Convert.ToString(dt_admin_Email_list.Rows[a]["Email_ID"]) + ",";
                                EmailID += AdminEmailID;
                            }
                        }
                        string email = EmailID;
                        string subject = string.Empty;
                        string body = string.Empty;
                        HttpWebRequest httpRequest;

                        httpRequest = (HttpWebRequest)WebRequest.Create("" + EmailUrl + "Email/customemail.html");

                        httpRequest.UserAgent = "Code Sample Web Client";
                        HttpWebResponse webResponse = (HttpWebResponse)httpRequest.GetResponse();
                        using (StreamReader reader = new StreamReader(webResponse.GetResponseStream()))
                        {
                            body = reader.ReadToEnd();
                        }
                        body = body.Replace("[name]", "Administrators");

                        body = body.Replace("[msg]", sendmsg);

                        subject = " " + dtc.Rows[0]["Company_Name"] + " - Compliance Alert - " + dc.Rows[0]["WireTransfer_ReferanceNo"];
                        CompanyInfo.Send_Mail(dtc, EmailID, body, subject, Convert.ToInt32(client_id), Convert.ToInt32(branch_id), "", "", "", context);

                    }
                    if (soapResult != "" && soapResult != null)
                    {
                        XmlDocument xmlDoc = new XmlDocument();
                        xmlDoc.LoadXml(soapResult);

                        XmlNodeList nodeList = xmlDoc.GetElementsByTagName("AuthenticateSPResult");
                        foreach (XmlNode node1 in nodeList)
                        {
                            foreach (XmlNode child in node1.ChildNodes)
                            {
                                if (child.Name == "BandText")
                                {
                                    Bandtext = child.InnerText;
                                }
                            }
                        }
                        XmlNodeList responsestring = xmlDoc.GetElementsByTagName("GlobalItemCheckResultCodes");
                        foreach (XmlNode node in responsestring)
                        {
                            string name = "";// node1["Name"].InnerText.ToString();
                            foreach (XmlNode child in node.ChildNodes)
                            {
                                if (node.InnerText.Contains("PEP"))
                                {
                                    if (child.Name == "Mismatch")
                                    {
                                        foreach (XmlNode childd in child.ChildNodes)
                                        {
                                            string ss = childd.InnerXml;
                                            string code1 = childd["Code"].InnerText.ToString();
                                            if (code1 == "9500")
                                            {
                                                Activity += " PEP : found";
                                                string description1 = childd["Description"].InnerText.ToString();
                                                string Record_DateTime = Record_Insert_DateTime;
                                                string notification_icon = "pep-match-not-found.jpg";
                                                //string notification_message = "<span class='cls-admin'>International Sanctions: <strong class='cls-cancel'>.</strong><br/>" + description1 + "</span><span class='cls-customer'></span>";
                                                string notification_message = "<span class='cls-admin'> - <span class='cls-cancel'><strong> International PEP Alert </strong>- </span><span class='cls-addr-confirm'>" + description1 + " </span></span></span><span class='cls-customer'></span>";
                                                CompanyInfo.save_notification_compliance(notification_message, notification_icon, Convert.ToString(customer_id), Convert.ToDateTime(Record_DateTime), client_id, 1, 1, branch_id, 0, 1, 1, 0, context);
                                                flag1++;

                                            }
                                        }
                                    }
                                }
                                else if (node.InnerText.Contains("Global Sanctions") || node.InnerText.Contains("International Sanctions"))
                                {
                                    if (child.Name == "Mismatch")
                                    {
                                        foreach (XmlNode childd in child.ChildNodes)
                                        {
                                            Activity += " Global Sanctions : found";
                                            string description1 = childd["Description"].InnerText.ToString();
                                            string Record_DateTime = Record_Insert_DateTime;
                                            string notification_icon = "pep-match-not-found.jpg";
                                            //  string notification_message = "<span class='cls-admin'>International Sanctions: <strong class='cls-cancel'>.</strong><br/>" + description1 + "</span><span class='cls-customer'></span>";
                                            string notification_message = "<span class='cls-admin'> - <span class='cls-cancel'><strong> International Sanctions Alert </strong>- </span><span class='cls-addr-confirm'>" + description1 + " </span></span></span><span class='cls-customer'></span>";
                                            CompanyInfo.save_notification_compliance(notification_message, notification_icon, Convert.ToString(customer_id), Convert.ToDateTime(Record_DateTime), client_id, 1, 1, branch_id, 0, 1, 1, 0, context);
                                            flag2++;
                                        }
                                    }
                                }
                            }
                        }
                        if (flag1 > 0 && flag2 > 0)
                        {
                            if (screening_type == 1)//rekyc
                            {

                                try
                                {
                                    MySqlCommand cmd_update = new MySqlCommand("update documents_details set aml_sanctions_flag=3 where SenderID_ID=@SenderID_ID");
                                    cmd_update.Parameters.AddWithValue("@SenderID_ID", Convert.ToString(dc.Rows[0]["SenderID_ID"]));
                                    db_connection.ExecuteNonQueryProcedure(cmd_update);
                                }
                                catch { }
                            }
                            if (screening_type == 2)//pep and sanction
                            {
                                if (benef_id > 0)
                                {
                                    try
                                    {
                                        MySqlCommand cmd_update = new MySqlCommand("UPDATE beneficiary_master SET PEPAndSanctions = '3' WHERE Beneficiary_ID = @Bneficiary_ID");
                                        cmd_update.Parameters.AddWithValue("@Bneficiary_ID", benef_id);
                                        db_connection.ExecuteNonQueryProcedure(cmd_update);
                                    }
                                    catch { }
                                }
                                else if (customer_id > 0)
                                {
                                    try
                                    {
                                        MySqlCommand cmd_update = new MySqlCommand("UPDATE customer_registration SET ThridParty_Check = '3' WHERE Customer_ID = @Cstomer_id");
                                        cmd_update.Parameters.AddWithValue("@Cstomer_id", customer_id);
                                        db_connection.ExecuteNonQueryProcedure(cmd_update);
                                    }
                                    catch { }
                                }

                            }
                            Status = 3;
                        }
                        else if (flag1 > 0)
                        {
                            if (screening_type == 1)//rekyc
                            {
                                try
                                {
                                    MySqlCommand cmd_update = new MySqlCommand("update documents_details set aml_sanctions_flag=1 where SenderID_ID=@SenderID_ID");
                                    cmd_update.Parameters.AddWithValue("@SenderID_ID", Convert.ToString(dc.Rows[0]["SenderID_ID"]));
                                    db_connection.ExecuteNonQueryProcedure(cmd_update);
                                }
                                catch { }
                            }
                            if (screening_type == 2)//pep and sanction
                            {
                                if (benef_id > 0)
                                {
                                    try
                                    {
                                        MySqlCommand cmd_update = new MySqlCommand("UPDATE beneficiary_master SET PEPAndSanctions = '1' WHERE Beneficiary_ID = @Bneficiary_ID");
                                        cmd_update.Parameters.AddWithValue("@Bneficiary_ID", benef_id);
                                        db_connection.ExecuteNonQueryProcedure(cmd_update);
                                    }
                                    catch { }
                                }
                                else if (customer_id > 0)
                                {
                                    try
                                    {
                                        MySqlCommand cmd_update = new MySqlCommand("UPDATE customer_registration SET ThridParty_Check = '1' WHERE Customer_ID = @Cstomer_id");
                                        cmd_update.Parameters.AddWithValue("@Cstomer_id", customer_id);
                                        db_connection.ExecuteNonQueryProcedure(cmd_update);
                                    }
                                    catch { }
                                }

                            }
                            Status = 1;
                        }
                        else if (flag2 > 0)
                        {
                            if (screening_type == 1)//rekyc
                            {
                                try
                                {
                                    MySqlCommand cmd_update = new MySqlCommand("update documents_details set aml_sanctions_flag=2 where SenderID_ID=@SenderID_ID");
                                    cmd_update.Parameters.AddWithValue("@SenderID_ID", Convert.ToString(dc.Rows[0]["SenderID_ID"]));
                                    db_connection.ExecuteNonQueryProcedure(cmd_update);
                                }
                                catch { }
                            }
                            if (screening_type == 2)//pep and sanction
                            {
                                if (benef_id > 0)
                                {
                                    try
                                    {
                                        MySqlCommand cmd_update = new MySqlCommand("UPDATE beneficiary_master SET PEPAndSanctions = '2' WHERE Beneficiary_ID = @Bneficiary_ID");
                                        cmd_update.Parameters.AddWithValue("@Bneficiary_ID", benef_id);
                                        db_connection.ExecuteNonQueryProcedure(cmd_update);
                                    }
                                    catch { }
                                }
                                else if (customer_id > 0)
                                {
                                    try
                                    {
                                        MySqlCommand cmd_update = new MySqlCommand("UPDATE customer_registration SET ThridParty_Check = '2' WHERE Customer_ID = @Cstomer_id");
                                        cmd_update.Parameters.AddWithValue("@Cstomer_id", customer_id);
                                        db_connection.ExecuteNonQueryProcedure(cmd_update);
                                    }
                                    catch { }
                                }

                            }
                            Status = 2;
                        }
                        else if (flag1 == 0 && flag2 == 0)
                        {
                            //try
                            //{
                            //    MySqlCommand cmd_update = new MySqlCommand("update documents_details set aml_sanctions_flag=2 where SenderID_ID=@SenderID_ID");
                            //    cmd_update.Parameters.AddWithValue("@SenderID_ID", Convert.ToString(dc.Rows[0]["SenderID_ID"]));
                            //    dbconnection.ExecuteNonQueryProcedure(cmd_update);
                            //}
                            //catch { }
                            Status = 4;
                        }

                        try
                        {
                            int status = 1; string Function_name = "CheckPepSanction";
                            Remark = Convert.ToString(CompanyInfo.getAPIStatus(Bandtext, client_id));
                            //if (Remark != null && Remark != "")
                            //    Status = 0;

                            MySqlCommand _cmd = new MySqlCommand("UpdateCustomerBandText");
                            _cmd.CommandType = CommandType.StoredProcedure;
                            _cmd.Parameters.AddWithValue("_Client_ID", client_id);
                            _cmd.Parameters.AddWithValue("_Customer_ID", customer_id);
                            _cmd.Parameters.AddWithValue("_Remark", Remark);
                            int msg = db_connection.ExecuteNonQueryProcedure(_cmd);
                            _cmd.Dispose();
                            if (benef_id > 0)
                            {
                                _cmd = new MySqlCommand("SaveAPIReqRespforBenf");
                                _cmd.CommandType = CommandType.StoredProcedure;
                                _cmd.Parameters.AddWithValue("_API_ID", API_ID);
                                _cmd.Parameters.AddWithValue("_Client_ID", client_id);
                                _cmd.Parameters.AddWithValue("_Customer_ID", customer_id);
                                _cmd.Parameters.AddWithValue("_status", status);
                                _cmd.Parameters.AddWithValue("_Function_name", Function_name);
                                _cmd.Parameters.AddWithValue("_Remark", Remark);
                                _cmd.Parameters.AddWithValue("_comments", soapResult.Replace("'", "''"));
                                _cmd.Parameters.AddWithValue("_Record_Insert_DateTime", Record_Insert_DateTime);
                                _cmd.Parameters.AddWithValue("_Branch_ID", branch_id);
                                _cmd.Parameters.AddWithValue("_Beneficiary_ID", benef_id);

                                int msg1 = db_connection.ExecuteNonQueryProcedure(_cmd);
                            }
                            else if (customer_id > 0)
                            {
                                _cmd = new MySqlCommand("SaveAPIRequestResponce");
                                _cmd.CommandType = CommandType.StoredProcedure;
                                _cmd.Parameters.AddWithValue("_API_ID", API_ID);
                                _cmd.Parameters.AddWithValue("_Client_ID", client_id);
                                _cmd.Parameters.AddWithValue("_Customer_ID", customer_id);
                                _cmd.Parameters.AddWithValue("_status", status);
                                _cmd.Parameters.AddWithValue("_Function_name", Function_name);
                                _cmd.Parameters.AddWithValue("_Remark", Remark);
                                _cmd.Parameters.AddWithValue("_comments", soapResult.Replace("'", "''"));
                                _cmd.Parameters.AddWithValue("_Record_Insert_DateTime", Record_Insert_DateTime);
                                _cmd.Parameters.AddWithValue("_Branch_ID", branch_id);
                                int msg1 = db_connection.ExecuteNonQueryProcedure(_cmd);
                            }


                        }
                        catch (Exception ex)
                        {
                            Activity += " Exception 1 " + ex.ToString();
                            string error = ex.ToString().Replace("\'", "\\'");

                            MySqlCommand _cmd = new MySqlCommand("SaveException");
                            _cmd.CommandType = CommandType.StoredProcedure;
                            _cmd.Parameters.AddWithValue("_Record_Insert_DateTime", Record_Insert_DateTime);
                            _cmd.Parameters.AddWithValue("_error", error);
                            _cmd.Parameters.AddWithValue("_Client_ID", client_id);
                            int msg1 = db_connection.ExecuteNonQueryProcedure(_cmd);
                        }
                        //if (Remark == "2" || Remark == "1")//alert or refer then  send mail to admin



                    }


                }
            }
            catch (Exception ex)
            {
                Activity += " Exception 2 " + ex.ToString();
                Model.ErrorLog objError = new Model.ErrorLog();
                objError.User = new Model.User();

                objError.Error = ex.ToString();
                objError.Date = DateTime.Now;
                objError.User_ID = 1;
                objError.Id = 1;
                objError.Function_Name = "CheckPepSanction";
                Service.srvErrorLog srvError = new Service.srvErrorLog();
                srvError.Create(objError, context);
                //string stattus = (string)mtsmethods.InsertErrorLogDetails(ex.Message.Replace("\'", "\\'"), user_id, "Check Credit Safe", i.Branch_ID, i.Client_ID);
            }
            finally
            {
                Activity += " Status  " + Status;
                int stattus = (int)CompanyInfo.InsertActivityLogDetails(Activity, 0, 0, 0, 1, "Create", Convert.ToInt32(branch_id), Convert.ToInt32(client_id), "checkPepSanction", context);

            }
            //string[] response = { Status.ToString(), Bandtext, flag1.ToString(), flag2.ToString() };
            return Convert.ToString(Status);
        }
        public static HttpWebRequest CreateWebRequest(string url, string action)
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.Headers.Add("SOAPAction", action);
            //webRequest.Headers.Add(@"SOAP:Action");
            webRequest.ContentType = "text/xml;charset=UTF-8";
            webRequest.Accept = "Encoding: gzip,deflate";
            webRequest.Method = "POST";
            return webRequest;
        }
        public static void InsertSoapEnvelopeIntoWebRequest(XmlDocument soapEnvelopeXml, HttpWebRequest webRequest)
        {
            try
            {
                using (Stream stream = webRequest.GetRequestStream())
                {
                    soapEnvelopeXml.Save(stream);
                }
            }
            catch (Exception ex)
            {
               // HttpContext.Current.Session["excweb"] = ex;
            }
        }
        #endregion

        public static string add_check_rules(int cust_ID, string risk_factors, int Branch_ID, int Client_ID, HttpContext context)
        {
            string msg = "";
            string risk_factors1 = "";
            string sts = "";
            DataTable dt = new DataTable();
            DataTable dt1 = new DataTable();
            if (risk_factors != "")
            {
                risk_factors = "1=1 and Risk_Factors IN(" + risk_factors + ")";
            }
            try
            {
                MySqlConnector.MySqlCommand cmd = new MySqlConnector.MySqlCommand("sp_get_rules");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("_Cust_ID", cust_ID);
                cmd.Parameters.AddWithValue("_risk_factors", risk_factors);
                cmd.Parameters.AddWithValue("_clientId", Client_ID);
                cmd.Parameters.AddWithValue("_branchId", Branch_ID);
                dt = db_connection.ExecuteQueryDataTableProcedure(cmd);

                cmd = new MySqlConnector.MySqlCommand("GetCustDetailsByID");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("cust_ID", cust_ID);
                cmd.Parameters.AddWithValue("_clientId", Client_ID);
                cmd.Parameters.AddWithValue("_branchId", Branch_ID);
                dt1 = db_connection.ExecuteQueryDataTableProcedure(cmd);

                string check_rules = "";
                if (dt != null && dt.Rows.Count > 0 && dt1 != null && dt1.Rows.Count > 0)
                {
                    foreach (DataRow row in dt1.Rows)
                    {
                        if (row["check_rules"] != DBNull.Value)
                        {
                            check_rules = Convert.ToString(row["check_rules"].ToString());
                        }
                    }
                }
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        if (row["Rule_ID"] != DBNull.Value)
                        {
                            risk_factors1 = risk_factors1 + Convert.ToString(row["Rule_ID"].ToString()) + ",";
                        }
                    }
                }
                int a = Convert.ToInt32(risk_factors1.Length);
                if (risk_factors1.EndsWith(",") == true)
                {
                    risk_factors1 = risk_factors1.Remove(a - 1);
                }
                string new_check_rule = "";
                new_check_rule = check_rules;
                if (risk_factors1 != "")
                {
                    if (risk_factors1 != "")
                    {
                        int rule_len = check_rules.Length;

                        string[] values = check_rules.Split(',').Select(sValue => sValue.Trim()).ToArray();
                        string[] values1 = risk_factors1.Split(',').Select(sValue => sValue.Trim()).ToArray();

                        for (int i = 0; i < values1.Length; i++)
                        {
                            if (!new_check_rule.Contains(values1[i]))
                            {
                                if (new_check_rule == "")
                                {
                                    new_check_rule = new_check_rule + values1[i];
                                }
                                else
                                {
                                    new_check_rule = new_check_rule + "," + values1[i];
                                }
                            }
                        }
                    }
                }
                if (new_check_rule != "")
                {
                    cmd = new MySqlConnector.MySqlCommand("sp_update_checkrules");
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("_Cust_ID", cust_ID);
                    cmd.Parameters.AddWithValue("_risk_factors", new_check_rule);
                    cmd.Parameters.AddWithValue("_clientId", Client_ID);
                    cmd.Parameters.AddWithValue("_branchId", Branch_ID);
                    int st = db_connection.ExecuteNonQueryProcedure(cmd);
                    if (st > 0)
                    {
                        msg = "success";
                        string Activities = "Check Rules Added Successfully";
                        int stattus = (int)CompanyInfo.InsertActivityLogDetails(Activities, 0, 0, 0, Convert.ToInt32(cust_ID), "Save_Check_Rules", Convert.ToInt32(Branch_ID), Convert.ToInt32(Client_ID), "Check_Rule", context);
                    }
                    else
                    {
                        msg = "failed";
                        string Activities = "Check Rules failed to add";
                        int stattus = (int)CompanyInfo.InsertActivityLogDetails(Activities, 0, 0, 0, Convert.ToInt32(cust_ID), "Save_Check_Rules", Convert.ToInt32(Branch_ID), Convert.ToInt32(Client_ID), "Check_Rule", context);
                    }
                }
                else
                {
                    msg = "failed";
                    string Activities = "Check Rules failed to add";
                    int stattus = (int)CompanyInfo.InsertActivityLogDetails(Activities, 0, 0, 0, Convert.ToInt32(cust_ID), "Save_Check_Rules", Convert.ToInt32(Branch_ID), Convert.ToInt32(Client_ID), "Check_Rule", context);
                }
                cmd.Dispose();
            }
            catch (Exception ex)
            {
                string Activities = ex.ToString();
                int stattus = (int)CompanyInfo.InsertActivityLogDetails(Activities, 0, 0, 0, Convert.ToInt32(cust_ID), "Save_Check_Rules", Convert.ToInt32(Branch_ID), Convert.ToInt32(Client_ID), "Check_Rule", context);
                //throw ex;
            }



            return msg;

        }

        public static bool chkValidExtensionforall(string ext)
        {
            string[] PosterAllowedExtensions = new string[] { ".jpeg", ".jpg", ".png", ".bmp", ".pdf" };
            for (int i = 0; i < PosterAllowedExtensions.Length; i++)
            {
                if (ext.ToLower() == PosterAllowedExtensions[i])
                    return true;
            }
            return false;
        }

        public static object getAPIStatus(string BandText, int Client_ID)
        {
            int status = 0;
            try
            {
             MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("GetAPIStatus");
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.Parameters.AddWithValue("_Client_ID", Client_ID);
                _cmd.Parameters.AddWithValue("_BandText", BandText);
                status = Convert.ToInt32(db_connection.ExecuteScalarProcedure(_cmd));
            }
            catch (Exception ae)
            {

            }
            return status;
        }
        public static object getCustomerDetails(int Client_ID, int Customer_ID)
        {
            MySqlConnector.MySqlCommand cmd3 = new MySqlConnector.MySqlCommand("customer_details_by_param");
            cmd3.CommandType = CommandType.StoredProcedure;
            string _whereclause = " and cr.Client_ID=" + Client_ID;
            if (Customer_ID > 0)
            {
                _whereclause = " and cr.Customer_ID=" + Customer_ID;
            }
            cmd3.Parameters.AddWithValue("_whereclause", _whereclause);
            cmd3.Parameters.AddWithValue("_SecurityKey", CompanyInfo.SecurityKey());
            DataTable dt_cust = db_connection.ExecuteQueryDataTableProcedure(cmd3);
            return dt_cust;
        }

        public static object getAdminEmailList(int Client_ID, int Branch_ID)
        {
            DataTable dt = new DataTable();
            try
            {
                MySqlConnector.MySqlCommand cmd = new MySqlConnector.MySqlCommand("GetAdmins");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("_Client_ID", Client_ID);
                string where = "";
                if (Branch_ID > 3 || Branch_ID == 1)
                {
                    where = " and u.Branch_ID=" + Branch_ID + " ";
                }
                cmd.Parameters.AddWithValue("_whereclause", where);
                dt = (DataTable)db_connection.ExecuteQueryDataTableProcedure(cmd);
            }
            catch (Exception ae)
            {

            }
            return dt;
        }
        public static object getEmailPermission(int Client_ID, int PID)
        {
            DataTable dt = new DataTable();
            try
            {
                MySqlConnector.MySqlCommand cmd = new MySqlConnector.MySqlCommand("GetPermissions");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("_Client_ID", Client_ID);
                string where = "";
                if (PID > 0)
                {
                    where = " and PID=" + PID + "";
                }
                cmd.Parameters.AddWithValue("_whereclause", where);
                dt = (DataTable)db_connection.ExecuteQueryDataTableProcedure(cmd);
            }
            catch (Exception ex)
            {
            }
            return dt;
        }

        public static string EncryptBioMetric(string plainText)
        {
            // Dim Key() As Byte = System.Text.Encoding.ASCII.GetBytes("IMXDRkGjzeiYMyNT44m35zR9rIjxxuuE")
            byte[] Key = System.Text.Encoding.ASCII.GetBytes("XMlkfg2845acGTbvdr270FGHBfghjkdc");
            //byte[] IV = System.Text.Encoding.ASCII.GetBytes("HQreTFgdtm1485rtyFG8ertyjfsERgh4");
            //byte[] IV = System.Text.Encoding.ASCII.GetBytes("HQreTFgdtm1485rt");
            byte[] IV = System.Text.Encoding.ASCII.GetBytes("HQreTFgdtm1485rtyFG8ertyjfsERgh4");

            if (plainText.Trim() == null || plainText.Trim().Length <= 0)
                throw new ArgumentNullException("plainText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            byte[] encrypted;
            // Create an RijndaelManaged object
            // with the specified key and IV.
            using (RijndaelManaged rijAlg = new RijndaelManaged())
            {
                rijAlg.BlockSize = 128;
                //rijAlg.KeySize = 32;
                rijAlg.Mode = CipherMode.CBC;
                rijAlg.Padding = PaddingMode.Zeros;
                rijAlg.Key = Key;
                rijAlg.IV = IV; // Key ' Convert.FromBase64String("9532654BD781547023AB4FA7723F2FCD")

                // Create a decrytor to perform the stream transform.
                ICryptoTransform encryptor = rijAlg.CreateEncryptor(rijAlg.Key, rijAlg.IV);

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {

                            // Write all data to the stream.
                            swEncrypt.Write(plainText.TrimStart().TrimEnd());
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }
            // Return the encrypted bytes from the memory stream.
            return Convert.ToBase64String(encrypted).TrimStart().TrimEnd();
        }

        public static string Encrypt(string plainText)
        {
            // Dim Key() As Byte = System.Text.Encoding.ASCII.GetBytes("IMXDRkGjzeiYMyNT44m35zR9rIjxxuuE")
            byte[] Key = System.Text.Encoding.ASCII.GetBytes("XMlkfg2845acGTbvdr270FGHBfghjkdc");
            //byte[] IV = System.Text.Encoding.ASCII.GetBytes("HQreTFgdtm1485rtyFG8ertyjfsERgh4");
            //byte[] IV = System.Text.Encoding.ASCII.GetBytes("HQreTFgdtm1485rt");
            byte[] IV = System.Text.Encoding.ASCII.GetBytes("HQreTFgdtm1485rtyFG8ertyjfsERgh4");

            if (plainText.Trim() == null || plainText.Trim().Length <= 0)
                throw new ArgumentNullException("plainText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            byte[] encrypted;
            // Create an RijndaelManaged object
            // with the specified key and IV.
            using (RijndaelManaged rijAlg = new RijndaelManaged())
            {
                rijAlg.BlockSize = 256;
                //rijAlg.KeySize = 32;
                rijAlg.Mode = CipherMode.CBC;
                rijAlg.Padding = PaddingMode.Zeros;
                rijAlg.Key = Key;
                rijAlg.IV = IV; // Key ' Convert.FromBase64String("9532654BD781547023AB4FA7723F2FCD")

                // Create a decrytor to perform the stream transform.
                ICryptoTransform encryptor = rijAlg.CreateEncryptor(rijAlg.Key, rijAlg.IV);

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {

                            // Write all data to the stream.
                            swEncrypt.Write(plainText.TrimStart().TrimEnd());
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }
            // Return the encrypted bytes from the memory stream.
            return Convert.ToBase64String(encrypted).TrimStart().TrimEnd();
        }

        public static string Encrypt(string toEncrypt, bool useHashing)
        {
           // var context = HttpContext.Current;
            byte[] keyArray;
            byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt);

            // Get the key from config file
            string key = SecurityKey();

           /* if (context.Session != null)
            {
                if (context.Session["scrutiny"] != null)
                { //key = Convert.ToString(context.Session["scrutiny"]);
                }
            }//System.Windows.Forms.MessageBox.Show(key); */
            if (useHashing)
            {
                MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
                hashmd5.Clear();
            }
            else
                keyArray = UTF8Encoding.UTF8.GetBytes(key);

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            tdes.Key = keyArray;
            tdes.Mode = CipherMode.ECB;
            tdes.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = tdes.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            tdes.Clear();
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }
       

        public static string SecurityKey()
        {          
            string reCaptchaKey = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("Jwt")["SecurityKey"];
            string key = (string)reCaptchaKey.Trim();
            return key;
        }

        public static FileInfo GetNewestFile(DirectoryInfo directory)
        {
            return directory.GetFiles()
                .Union(directory.GetDirectories().Select(d => GetNewestFile(d)))
                .OrderByDescending(f => (f == null ? DateTime.MinValue : f.LastWriteTime))
                .FirstOrDefault();
        }
        public static String testInjection(string testSQL)
        {
            TAntiSQLInjection anti = new TAntiSQLInjection(TDbVendor.DbVMysql);

            testSQL = "select  1 from a where  a='" + testSQL + "'";
            String msg = "";
            if (anti.isInjected(testSQL))
            {
                msg = "0";
                for (int i = 0; i < anti.getSqlInjections().Count; i++)
                {
                    msg = msg + Environment.NewLine + ("type: " + anti.getSqlInjections()[i].getType() + ", description: " + anti.getSqlInjections()[i].getDescription());
                }
            }
            else
            {
                msg = "1";
            }
            return msg;
        }


        public static string DataTableToJSONWithJSONNet(DataTable table)
        {
            string JSONString = string.Empty;
            JSONString = JsonConvert.SerializeObject(table);
            return JSONString;
        }

  
        public static string DataTableToJSONWithNewtonsoftJson(DataTable table)
        {
            List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
            foreach (DataRow dr in table.Rows)
            {
                Dictionary<string, object> row = new Dictionary<string, object>();
                foreach (DataColumn col in table.Columns)
                {
                    row.Add(col.ColumnName, dr[col]);
                }
                rows.Add(row);
            }
            return JsonConvert.SerializeObject(rows);
        }

        public static string UnescapeJson(string jsonString)
        {
            string New_json_string = jsonString;
            New_json_string = New_json_string.Replace("\\", "");
            return New_json_string;
        }
		public static void save_notification(string _Notification, string _Notification_Icon, int _Customer_ID, DateTime _Record_Insert_DateTime,
            int _Client_ID, int _Read_Count,
         int _User_ID, int _Branch, int _Show_notificationsfor_Admin, int _show_notificationfor_customer, int _show_notificationfor_POC, int Transaction_ID,HttpContext context)
        {

            try
            {
                DateTime RecordDate = Convert.ToDateTime(CompanyInfo.gettime(_Client_ID, context));
                MySqlConnector.MySqlCommand cmd_notif = new MySqlConnector.MySqlCommand("sp_save_notification");
                cmd_notif.CommandType = CommandType.StoredProcedure;
                cmd_notif.Parameters.AddWithValue("_Notification", _Notification);
                cmd_notif.Parameters.AddWithValue("_Notification_Icon", _Notification_Icon);
                cmd_notif.Parameters.AddWithValue("_Customer_ID", _Customer_ID);
                cmd_notif.Parameters.AddWithValue("_Record_Insert_DateTime", RecordDate);
                cmd_notif.Parameters.AddWithValue("_Read_Count", _Read_Count);
                cmd_notif.Parameters.AddWithValue("_User_ID", _User_ID);
                cmd_notif.Parameters.AddWithValue("_Show_notificationsfor_Admin", 0);
                cmd_notif.Parameters.AddWithValue("_show_notificationfor_customer", _show_notificationfor_customer);
                cmd_notif.Parameters.AddWithValue("_show_notificationfor_POC", _show_notificationfor_POC);
                cmd_notif.Parameters.AddWithValue("_Client_ID", _Client_ID);
                cmd_notif.Parameters.AddWithValue("_Branch", _Branch);
                cmd_notif.Parameters.AddWithValue("_Transaction_ID", Transaction_ID);
                cmd_notif.Parameters.AddWithValue("_notification_for_compliance", 1);
                db_connection.ExecuteNonQueryProcedure(cmd_notif);
                cmd_notif.Dispose();


            }
            catch (Exception ex)
            {
                //string Message = "Failed to add notification";
                //string Activities = "<b>Same document already exists in our system " + obj.UserName + "," + obj.SenderID_ExpiryDate + "," + obj.IDName_ID + "," + obj.SenderID_Number + " </b>";
                //int stattus = (int)CompanyInfo.InsertActivityLogDetails(Activities, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(obj.Customer_ID), "InsertPrimaryDocumentID", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "ID Upload");
                throw ex;
            }
        }
        public static object InsertTrackingLogDetails(int Track_ID, int Transaction_ID, int Delete_Status, int Client_ID, int Branch_ID,HttpContext context)
        {
            mts_connection _MTS = new mts_connection();
            db_connection connection_string = new db_connection();
            using (MySqlConnector.MySqlConnection con = new MySqlConnector.MySqlConnection(connection_string.ConnectionStringSection()))
            {
                con.Open();
                MySqlConnector.MySqlCommand cmd3 = new MySqlConnector.MySqlCommand("SP_Save_TrackingDeails");
                cmd3.CommandType = CommandType.StoredProcedure;
                cmd3.Connection = con;
                cmd3.Parameters.AddWithValue("_Track_ID", Track_ID);
                cmd3.Parameters.AddWithValue("_Transaction_ID", Transaction_ID);
                cmd3.Parameters.AddWithValue("_Delete_Status", Delete_Status);
                cmd3.Parameters.AddWithValue("_Client_ID", Client_ID);
                cmd3.Parameters.AddWithValue("_Branch_ID", Branch_ID);
                cmd3.Parameters.AddWithValue("_Record_Insert_DateTime", (string)CompanyInfo.gettime(Client_ID,context));
                int n = cmd3.ExecuteNonQuery();
                return n;
            }

        }
        public static object Send_Mail(DataTable dt, string email_id, string body, string subject, int client_id, int branch_id, string flag, string CC, string BCC, string Base_currency, HttpContext context)
        {
            string email = email_id; string from = string.Empty; string password = string.Empty;
            string msg = string.Empty;
            using (MailMessage mailMessage = new MailMessage())
            {
                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
                DataTable company_email_details = dt;
                if (dt == null || dt.Rows.Count <= 0)
                {
                    company_email_details = (DataTable)GetBaseCurrencywisebankdetails(client_id, Base_currency,0,0);
                }
                body = body.Replace("[company_website]", Convert.ToString(company_email_details.Rows[0]["company_website"]));
                subject = subject.Replace("[company_name]", company_email_details.Rows[0]["Company_Name"].ToString());
                // string from = company_email_details.Rows[0]["Email_Convey_from"].ToString(), password = company_email_details.Rows[0]["Password"].ToString();
                body = body.Replace("[company_name]", company_email_details.Rows[0]["Company_Name"].ToString());
                //body = body.Replace("[contact_no]", company_email_details.Rows[0]["Company_mobile"].ToString());body = body.Replace("[email_id]", company_email_details.Rows[0]["Company_Email"].ToString());
                if ((company_email_details.Rows[0]["Company_mobile"] == null) || (company_email_details.Rows[0]["Company_mobile"] == ""))
                {
                    //body = body.Replace("[contact_no]", Convert.ToString(company_email_details.Rows[0]["Company_mobile"]));
                    body = body.Replace("[contact_no]", Convert.ToString("<a href='tel:" + company_email_details.Rows[0]["Company_mobile"] + "'>" + company_email_details.Rows[0]["Company_mobile"]) + "</a>");
                    body = body.Replace("[contact_id1]", "style='display:none'");
                    body = body.Replace("[or1]", "style='display:none'");
                }
                else
                {
                    //body = body.Replace("[contact_no]", Convert.ToString(company_email_details.Rows[0]["Company_mobile"]));
                    body = body.Replace("[contact_no]", Convert.ToString("<a href='tel:" + company_email_details.Rows[0]["Company_mobile"] + "'>" + company_email_details.Rows[0]["Company_mobile"]) + "</a>");

                }
                if ((company_email_details.Rows[0]["Company_Email"] == null) || (company_email_details.Rows[0]["Company_Email"] == ""))
                {
                    body = body.Replace("[email_id]", Convert.ToString(company_email_details.Rows[0]["Company_Email"]));
                    body = body.Replace("[email_id11]", "style='display:none'");
                    body = body.Replace("[or1]", "style='display:none'");
                }
                else
                {
                    body = body.Replace("[email_id]", Convert.ToString(company_email_details.Rows[0]["Company_Email"]));
                }
                if (((company_email_details.Rows[0]["Company_mobile"] == null) || (company_email_details.Rows[0]["Company_mobile"] == "")) && ((company_email_details.Rows[0]["Company_Email"] == null) || (company_email_details.Rows[0]["Company_Email"] == "")))
                {
                    body = body.Replace("[line_id1]", "style='display:none'");
                }
                string send_mail = "mailto:" + company_email_details.Rows[0]["Company_Email"].ToString();
                body = body.Replace("[email_id1]", send_mail);
                body = body.Replace("[privacy_policy]", company_email_details.Rows[0]["privacy_policy"].ToString());
                body = body.Replace("[company_logo]", company_email_details.Rows[0]["Email_company_image"].ToString());
                body = body.Replace("[customer_website]", Convert.ToString(company_email_details.Rows[0]["Company_URL_Customer"]));
                body = body.Replace("[company_addresss]", Convert.ToString(company_email_details.Rows[0]["Company_Address"]));
                body = body.Replace("[company_reg_no]", Convert.ToString(company_email_details.Rows[0]["CompanyReg_No"]));
                body = body.Replace("[company_reg_office]", Convert.ToString(company_email_details.Rows[0]["Company_Address"]));
                body = body.Replace("[company_ref_no].", Convert.ToString(company_email_details.Rows[0]["CompanyRef_No"]));
                body = body.Replace("[email]", email);
                string currentYear = Convert.ToString(DateTime.Now.Year);
                body = body.Replace("©", "© " + currentYear);
                //for bank transfer
                body = body.Replace("[wire1]", Convert.ToString(company_email_details.Rows[0]["AccountHolderName"]));
                body = body.Replace("[wire2]", Convert.ToString(company_email_details.Rows[0]["BankName"]));
                body = body.Replace("[wire3]", Convert.ToString(company_email_details.Rows[0]["AccountNumber"]));
                body = body.Replace("[wire5]", Convert.ToString(company_email_details.Rows[0]["Sort_ID"]));

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

                #region social-media-icons

                if (Convert.ToString(company_email_details.Rows[0]["Playstore"]) == null || (Convert.ToString(company_email_details.Rows[0]["Playstore"]) == ""))
                {
                    body = body.Replace("[Playstore-social]", "");
                }
                else
                {
                    body = body.Replace("[Playstore-social]", "<td class='img' width='40'style='font-size:0pt; line-height:0pt; text-align:left;image-rendering:auto!important;'><a href=" + Convert.ToString(company_email_details.Rows[0]["Playstore"]) + " target='_blank'><img src='" + Convert.ToString(company_email_details.Rows[0]["Company_URL_Customer"]) + "/assets/img/social-media-icons/google-play-store.png' width='100' height='30' style='max-width:100px; border='0' alt='Google Paly Store' /></a></td>");
                }
                if (Convert.ToString(company_email_details.Rows[0]["Appstore"]) == null || (Convert.ToString(company_email_details.Rows[0]["Appstore"]) == ""))
                {
                    body = body.Replace("[Appstore-social]", "");
                }
                else
                {
                    body = body.Replace("[Appstore-social]", "<td class='img' width='40'style='font-size:0pt; line-height:0pt; text-align:left;image-rendering:auto!important;'><a href=" + Convert.ToString(company_email_details.Rows[0]["Appstore"]) + " target='_blank'><img src='" + Convert.ToString(company_email_details.Rows[0]["Company_URL_Customer"]) + "/assets/img/social-media-icons/app-store.png' width='100' height='30' style='max-width:100px; border='0' alt='App Store' /></a></td>");
                }

                if (Convert.ToString(company_email_details.Rows[0]["Twitter"]) == null || (Convert.ToString(company_email_details.Rows[0]["Twitter"]) == ""))
                {
                    body = body.Replace("[twitter-social]", "");
                }
                else
                {
                    body = body.Replace("[twitter-social]", "<td class='img' width='40' style='font-size:0pt; line-height:0pt; text-align:left;image-rendering:auto!important;'> <a href=" + Convert.ToString(company_email_details.Rows[0]["Twitter"]) + " target='_blank'><img src='" + Convert.ToString(company_email_details.Rows[0]["Company_URL_Customer"]) + "/assets/img/social-media-icons/ico4_twitter.png' width='26' height='26' style='max-width:26px; border='0' alt='Twitter' /></a></td>");
                }

                if (Convert.ToString(company_email_details.Rows[0]["Facebook"]) == null || (Convert.ToString(company_email_details.Rows[0]["Facebook"]) == ""))
                {
                    body = body.Replace("[facebook-social]", "");
                }
                else
                {
                    body = body.Replace("[facebook-social]", "<td class='img' width='40' style='font-size:0pt; line-height:0pt; text-align:left;image-rendering:auto!important;'> <a href=" + Convert.ToString(company_email_details.Rows[0]["Facebook"]) + " target='_blank'><img src='" + Convert.ToString(company_email_details.Rows[0]["Company_URL_Customer"]) + "/assets/img/social-media-icons/ico4_facebook.png' width='26' height='26' style='max-width:26px; border='0' alt='Facebook' /></a></td>");
                }

                if (Convert.ToString(company_email_details.Rows[0]["Gmail"]) == null || (Convert.ToString(company_email_details.Rows[0]["Gmail"]) == ""))
                {
                    body = body.Replace("[GooglePluse-social]", "");
                }
                else
                {
                    body = body.Replace("[GooglePluse-social]", "<td class='img' width='40' style='font-size:0pt; line-height:0pt; text-align:left;image-rendering:auto!important;'> <a href='mailto:" + Convert.ToString(company_email_details.Rows[0]["Gmail"]) + "' target='_blank'><img src='" + Convert.ToString(company_email_details.Rows[0]["Company_URL_Customer"]) + "/assets/img/social-media-icons/ico4_gplus.png' width='26' height='26' style='max-width:26px; border='0' alt='Google Pluse' /></a></td>");
                }

                if (Convert.ToString(company_email_details.Rows[0]["Youtube"]) == null || (Convert.ToString(company_email_details.Rows[0]["Youtube"]) == ""))
                {
                    body = body.Replace("[YouTube-social]", "");
                }
                else
                {
                    body = body.Replace("[YouTube-social]", "<td class='img' width='40' style='font-size:0pt; line-height:0pt; text-align:left;image-rendering:auto!important;'> <a href=" + Convert.ToString(company_email_details.Rows[0]["Youtube"]) + " target='_blank'><img src='" + Convert.ToString(company_email_details.Rows[0]["Company_URL_Customer"]) + "/assets/img/social-media-icons/ico4_youtube.png' width='26' height='26' style='max-width:26px; border='0' alt='You Tube' /></a></td>");
                }

                if (Convert.ToString(company_email_details.Rows[0]["Instagram"]) == null || (Convert.ToString(company_email_details.Rows[0]["Instagram"]) == ""))
                {
                    body = body.Replace("[Instagram-social]", "");
                }
                else
                {
                    body = body.Replace("[Instagram-social]", "<td class='img' width='40' style='font-size:0pt; line-height:0pt; text-align:left;image-rendering:auto!important;'> <a href=" + Convert.ToString(company_email_details.Rows[0]["Instagram"]) + " target='_blank'><img src='" + Convert.ToString(company_email_details.Rows[0]["Company_URL_Customer"]) + "/assets/img/social-media-icons/ico4_instagram.png' width='26' height='26' style='max-width:26px; border='0' alt='Instagram' /></a></td>");
                }

                if (Convert.ToString(company_email_details.Rows[0]["Pinterest"]) == null || (Convert.ToString(company_email_details.Rows[0]["Pinterest"]) == ""))
                {
                    body = body.Replace("[Pinterest-social]", "");
                }
                else
                {
                    body = body.Replace("[Pinterest-social]", "<td class='img' width='40' style='font-size:0pt; line-height:0pt; text-align:left;image-rendering:auto!important;'> <a href=" + Convert.ToString(company_email_details.Rows[0]["Pinterest"]) + " target='_blank'><img src='" + Convert.ToString(company_email_details.Rows[0]["Company_URL_Customer"]) + "/assets/img/social-media-icons/ico4_pinterest.png' width='26' height='26' style='max-width:26px; border='0' alt='Pinterest' /></a></td>");
                }
                if (Convert.ToString(company_email_details.Rows[0]["Linkedin"]) == null || (Convert.ToString(company_email_details.Rows[0]["Linkedin"]) == ""))
                {
                    body = body.Replace("[Linkedin-social]", "");
                }
                else
                {
                    body = body.Replace("[Linkedin-social]", "<td class='img' width='40' style='font-size:0pt; line-height:0pt; text-align:left;image-rendering:auto!important;'> <a href=" + Convert.ToString(company_email_details.Rows[0]["Linkedin"]) + " target='_blank'><img src='" + Convert.ToString(company_email_details.Rows[0]["Company_URL_Customer"]) + "/assets/img/social-media-icons/ico4_linkedin.png' width='26' height='26' style='max-width:26px; border='0' alt='Linkedin' /></a></td>");
                }

                if (Convert.ToString(company_email_details.Rows[0]["Whats_app_no"]) == null || (Convert.ToString(company_email_details.Rows[0]["Whats_app_no"]) == ""))
                {
                    body = body.Replace("[Whatsapp-social]", "");
                }
                else
                {
                    body = body.Replace("[Whatsapp-social]", "<td class='img' width='40' style='font-size:0pt; line-height:0pt; text-align:left;image-rendering:auto!important;'> <a href=' https://api.whatsapp.com/send?phone=" + Convert.ToString(company_email_details.Rows[0]["Whats_app_no"]) + "&text=Hello There, I would like to enquire about money transfer.' target='_blank'><img src='" + Convert.ToString(company_email_details.Rows[0]["Company_URL_Customer"]) + "/assets/img/social-media-icons/ico4_whatsapp.png'  width='26' height='26' style='max-width:26px;' border='0' alt='Whatsapp' /></a></td>");
                }
                #endregion

                mailMessage.From = new MailAddress(from);
                mailMessage.Body = body;
                mailMessage.Subject = subject;
                mailMessage.BodyEncoding = System.Text.Encoding.UTF8;
                mailMessage.Priority = MailPriority.High;
                mailMessage.IsBodyHtml = true;

                DataTable ds = (DataTable)getEmailPermission(client_id, 84);
                string Status = Convert.ToString(ds.Rows[0]["Status"]);
                if ((email == "" || email == null) && Status == "0")
                {
                    email = from;//company_email_details.Rows[0]["Company_Email"].ToString();
                }
                else
                {
                    string[] bccid = email.Split(',');
                    foreach (string bccEmailId in bccid)
                    {
                        if (bccEmailId != "" && bccEmailId != null)
                        {
                            mailMessage.To.Add(new MailAddress(bccEmailId));
                            //mailMessage.Bcc.Add(new MailAddress(bccEmailId)); //Adding Multiple BCC email Id
                        }
                    }
                    // mailMessage.To.Add(new MailAddress(email));
                }
                SmtpClient smtp = new SmtpClient();
                DataTable dt_get_Permission = (DataTable)getEmailPermission(client_id, 21);
                if (dt_get_Permission.Rows.Count > 0)
                {
                    string EmailID = "";
                    if (dt_get_Permission.Rows[0]["Status_ForCustomer"].ToString() == "0")//send mail to admin
                    {
                        DataTable dt_admin_Email_list = (DataTable)getAdminEmailList(client_id, branch_id);
                        if (dt_admin_Email_list.Rows.Count > 0)
                        {
                            for (int a = 0; a < dt_admin_Email_list.Rows.Count; a++)
                            {
                                string AdminEmailID = Convert.ToString(dt_admin_Email_list.Rows[a]["Email_ID"]) + ",";
                                EmailID += AdminEmailID;
                            }
                            string[] bccid = EmailID.Split(',');
                            foreach (string bccEmailId in bccid)
                            {
                                if (bccEmailId != "" && bccEmailId != null)
                                {
                                    mailMessage.Bcc.Add(new MailAddress(bccEmailId)); //Adding Multiple BCC email Id
                                }
                            }
                        }
                    }
                    if (dt_get_Permission.Rows[0]["Status_ForCustomer"].ToString() == "1")//no mail
                    {
                    }
                    if (flag == "custom_email")
                    {
                        string[] ccid = CC.Split(',');
                        foreach (string ccEmailId in ccid)
                        {
                            if (ccEmailId != "" && ccEmailId != null)
                            {
                                mailMessage.CC.Add(new MailAddress(ccEmailId)); //Adding Multiple CC email Id
                            }
                        }
                        string[] bccid1 = BCC.Split(',');
                        foreach (string bccEmailId1 in bccid1)
                        {
                            if (bccEmailId1 != "" && bccEmailId1 != null)
                            {
                                mailMessage.Bcc.Add(new MailAddress(bccEmailId1)); //Adding Multiple BCC email Id
                            }
                        }
                    }
                }
                //HttpContext.Current.Session.Add("Mailmsg", mailMessage);
                smtp.Host = dt_send_email.Rows[0]["Host"].ToString();
                smtp.EnableSsl = true;
                System.Net.NetworkCredential NetworkCred = new System.Net.NetworkCredential(from, password);
                //smtp.UseDefaultCredentials = true;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = NetworkCred;
                smtp.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
                smtp.Port = Convert.ToInt32(dt_send_email.Rows[0]["Port"].ToString());
                try
                {
                    smtp.Send(mailMessage);
                    msg = "Success";
                }
                catch (Exception ex)
                {
                    msg = ex.ToString();
                    InsertActivityLogDetails("App - " + msg, 1, 1, 1, 1, "Send-Email Sending", 2, 1, "Send Money", context);
                    try
                    {
                        string from1 = string.Empty;
                        string password1 = string.Empty;
                        //get send email details
                        MySqlConnector.MySqlCommand _cmd1 = new MySqlConnector.MySqlCommand("Get_Email_Configuration");
                        _cmd1.CommandType = CommandType.StoredProcedure;
                        string _whereclause1 = " and Client_ID=" + client_id + " and   priority=2";

                        _cmd1.Parameters.AddWithValue("_whereclause", _whereclause1);
                        _cmd1.Parameters.AddWithValue("_SecurityKey", CompanyInfo.SecurityKey());
                        DataTable dt_send_email1 = db_connection.ExecuteQueryDataTableProcedure(_cmd1);

                        if (dt_send_email1.Rows.Count > 0)
                        {

                            from1 = dt_send_email1.Rows[0]["Email_Convey_from"].ToString();
                            password1 = dt_send_email1.Rows[0]["Password"].ToString();
                            smtp.Host = dt_send_email1.Rows[0]["Host"].ToString();
                            mailMessage.From = new MailAddress(from1);
                            smtp.EnableSsl = true;
                            NetworkCred = new System.Net.NetworkCredential(from1, password1);
                            smtp.UseDefaultCredentials = false;
                            smtp.Credentials = NetworkCred;
                            smtp.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
                            smtp.Port = Convert.ToInt32(dt_send_email1.Rows[0]["Port"].ToString());
                            try
                            {
                                smtp.Send(mailMessage);
                                msg = "Success";
                            }
                            catch (Exception ex2)
                            {
                                string from2 = string.Empty;
                                string password2 = string.Empty;
                                MySqlConnector.MySqlCommand _cmd2 = new MySqlConnector.MySqlCommand("Get_Email_Configuration");
                                _cmd2.CommandType = CommandType.StoredProcedure;
                                string _whereclause2 = " and Client_ID=" + client_id + " and   priority=3";

                                _cmd2.Parameters.AddWithValue("_whereclause", _whereclause2);
                                _cmd2.Parameters.AddWithValue("_SecurityKey", CompanyInfo.SecurityKey());
                                DataTable dt_send_email2 = db_connection.ExecuteQueryDataTableProcedure(_cmd2);


                                if (dt_send_email2.Rows.Count > 0)
                                {

                                    from2 = dt_send_email2.Rows[0]["Email_Convey_from"].ToString();
                                    password2 = dt_send_email2.Rows[0]["Password"].ToString();
                                    smtp.Host = dt_send_email2.Rows[0]["Host"].ToString();
                                    mailMessage.From = new MailAddress(from2);
                                    smtp.EnableSsl = true;
                                    NetworkCred = new System.Net.NetworkCredential(from2, password2);
                                    //smtp.UseDefaultCredentials = true;
                                    smtp.UseDefaultCredentials = false;
                                    smtp.Credentials = NetworkCred;
                                    smtp.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
                                    smtp.Port = Convert.ToInt32(dt_send_email2.Rows[0]["Port"].ToString());
                                    try
                                    {
                                        smtp.Send(mailMessage);
                                        msg = "Success";
                                    }
                                    catch (Exception ex3)
                                    {
                                        msg = ex3.ToString();
                                        InsertActivityLogDetails("App - " + msg, 1, 1, 1, 1, "Send-Email Sending", 2, 1, "Send Money", context);
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex1)
                    {
                        msg = ex1.ToString();
                        InsertActivityLogDetails("App - " + msg, 1, 1, 1, 1, "Send-Email Sending", 2, 1, "Send Money", context);
                    }
                    //string stattus = (string)mtsmethods.InsertErrorLogDetails(ex.Message, 1, "Send_Mail", branch_id, client_id);
                }
            }
            return msg;
        }

        public static string ConvertMySQLDateFormatToDotNet(string mysqlFormat)
        {
            Dictionary<string, string> formatMap = new Dictionary<string, string>()
    {
        { "%d", "dd" },
        { "%m", "MM" },
        { "%b", "MMM" },
        { "%M", "MMMM" },
        { "%Y", "yyyy" },
        { "%y", "yy" },
        { "%D", "dd" },
        { "%H", "HH" },
        { "%h", "hh" },
        { "%i", "mm" },
        { "%s", "ss" },
        { "%T", "HH:mm:ss" },
        { "%r", "hh:mm:ss tt" }
    };

            foreach (var entry in formatMap)
            {
                mysqlFormat = mysqlFormat.Replace(entry.Key, entry.Value);
            }

            return mysqlFormat;
        }

        public static int check_notification_perm(string _Customer_ID, int _Client_ID, int _Branch_ID, int _Notification_Moule_ID, int _SubNotification_ID, DateTime dt, int Custom_notification, int Sms_status, int email_status, int notification_status, string Function_Name, string notification_message, int transaction_id, HttpContext context)

        {
            try
            {
                string custlist = _Customer_ID;
                string Title = "";
                string Message = "";
                DataTable dt1 = new DataTable();
                string whereclause = "";
                string Record_Insert_DateTime = Convert.ToString(gettime(_Client_ID, _Customer_ID, 0, context));
                DataTable dt2 = new DataTable();
                MySqlConnector.MySqlCommand cmd_notif = new MySqlConnector.MySqlCommand("check_notification_perm");
                cmd_notif.CommandType = CommandType.StoredProcedure;
                cmd_notif.Parameters.AddWithValue("_Client_ID", _Client_ID);
                if (_Branch_ID != 0)
                {
                    whereclause = whereclause + " and  notification_access_table.Branch_ID=" + _Branch_ID;
                }
                if (_Notification_Moule_ID != 0)
                {
                    whereclause = whereclause + " and notification_access_table.Notifications_ID=" + _Notification_Moule_ID;
                }
                if (_SubNotification_ID != 0)
                {
                    whereclause = whereclause + " and  notification_access_table.Sub_Notification_ID=" + _SubNotification_ID;
                }
                cmd_notif.Parameters.AddWithValue("_whereclause", whereclause);
                dt2 = db_connection.ExecuteQueryDataTableProcedure(cmd_notif);
                cmd_notif.Dispose();
                if (dt2.Rows.Count > 0)
                {
                    Title = Convert.ToString(dt2.Rows[0]["notification_title"]);
                }
                else
                {
                    Title = "Hi [First_name] [Last_name],";
                }


                if (Convert.ToInt32(notification_status) == 0)
                {
                    Message = Convert.ToString(notification_message);

                    if (Custom_notification == 0)
                    {
                        int i = Check_notification_perm_scheduler(Title, Message, Convert.ToInt32(_Customer_ID), Convert.ToInt32(_Client_ID), Convert.ToInt32(_Branch_ID), dt, notification_status, Sms_status, email_status, custlist, "1", Convert.ToString(_SubNotification_ID));
                        Custom_notification = 1;
                    }
                    else
                    {
                        _ = Device_Notification(Title, Message, Convert.ToInt32(_Customer_ID), _Client_ID, Function_Name, Convert.ToString(_SubNotification_ID), context);
                    }
                }
                if (Convert.ToInt32(Sms_status) == 0)
                {
                    Message = Convert.ToString(notification_message);

                    if (Custom_notification == 0)
                    {
                        int i = Check_notification_perm_scheduler(Title, Message, Convert.ToInt32(_Customer_ID), Convert.ToInt32(_Client_ID), Convert.ToInt32(_Branch_ID), dt, notification_status, Sms_status, email_status, custlist, "1", Convert.ToString(_SubNotification_ID));
                        Custom_notification = 1;

                    }
                    else
                    {
                        _ = Sms_Notification(Title, Message, Convert.ToInt32(_Customer_ID), _Client_ID, Function_Name, Convert.ToString(_SubNotification_ID), context);
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return 0;
        }
        public static string GetHash(string stringToHash)
        {
            // "GBP100.00test_site123452019-05-28 14:22:37PASSWORD";
            byte[] bytesToHash = Encoding.UTF8.GetBytes(stringToHash);
            SHA256Managed sha256 = new SHA256Managed();
            byte[] hashBytes = sha256.ComputeHash(bytesToHash);
            string hashString = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            return hashString;
        }

    }
    class GeoLocation
    {
        public string status { get; set; }
        public string description { get; set; }
        public Data data { get; set; }

    }
}
