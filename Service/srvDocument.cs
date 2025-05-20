//using Microsoft.Ajax.Utilities;
using System.Data;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using iTextSharp.text.pdf;
using System.Net;
using System.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
//using System.Web.Mvc;
using MySql.Data.MySqlClient;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;

using Microsoft.AspNetCore.Http.Headers;

using Microsoft.Extensions.Configuration;
using System;

using System.Security.Cryptography;

using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using System.Data.Common;
using RestSharp;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using Calyx_Solutions.Model;
using MySqlCommand = MySqlConnector.MySqlCommand;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Numerics;
using System.Web.Helpers;
using Microsoft.Ajax.Utilities;
using Yoti.Auth.DocScan.Session.Create;
using Yoti.Auth.DocScan.Session.Create.Check;
using Yoti.Auth.Exceptions;
using Yoti.Auth;
using Microsoft.Extensions.Options;
using Yoti.Auth.DocScan;
using Yoti.Auth.DocScan.Session.Create.Task;
using Yoti.Auth.DocScan.Session.Retrieve;
using Newtonsoft.Json;

using Yoti.Auth.DocScan.Session.Retrieve.Check;
using Yoti.Auth.DocScan.Session.Retrieve.Resource;
using Auth0.ManagementApi.Models;
using static Google.Apis.Requests.BatchRequest;
namespace Calyx_Solutions.Service
{

    public class HttpPostedFileBase
    {
        public string FileName { get; }
        public string ContentType { get; }
        public byte[] Content { get; }

        public HttpPostedFileBase(string fileName, string contentType, byte[] content)
        {
            FileName = fileName ?? throw new ArgumentNullException(nameof(fileName));
            ContentType = contentType ?? throw new ArgumentNullException(nameof(contentType));
            Content = content ?? throw new ArgumentNullException(nameof(content));
        }

        public Stream InputStream => new MemoryStream(Content);

        public void SaveAs(string path)
        {
            using (var fileStream = new FileStream(path, FileMode.Create))
            {
                fileStream.Write(Content, 0, Content.Length);
            }
        }
    }

    public static class FormFileExtensions
    {
        public static HttpPostedFileBase ToHttpPostedFileBase(this IFormFile formFile)
        {
            var ms = new MemoryStream();
            formFile.CopyTo(ms);
            return new HttpPostedFileBase(formFile.FileName, formFile.ContentType, ms.ToArray());
        }
    }

    public class srvDocument
    {



        db_connection _dbConnection = new db_connection();


        public DataTable Select_Document_List(Model.Document obj, HttpContext context)
        {
            DataTable dt = new DataTable();

            Customer obj_1 = new Customer();

            obj_1.Id = obj.Customer_ID;

            string Activity = string.Empty; string[] Alert_Msg = new string[7]; string sendmsg = string.Empty; string notification_icon = ""; string notification_message = "";
            //var context = System.Web.HttpContext.Current;
            //string Username = Convert.ToString(context.Request.Form["Username"]);
            string error_invalid_data = "";
            string error_msg = ""; string Password_regex = "true";
            string Client_ID_regex = validation.validate(Convert.ToString(obj.Client_ID), 0, 1, 1, 1, 1, 1, 1, 1, 1);
            string Beneficiary_ID_regex = validation.validate(Convert.ToString(obj.Beneficiary_ID), 0, 1, 1, 1, 1, 1, 1, 1, 1);
            string IDType_ID_regex = validation.validate(Convert.ToString(obj.IDType_ID), 0, 1, 1, 1, 1, 1, 1, 1, 1);
            string ID_Name_regex = validation.validate(Convert.ToString(obj.ID_Name), 1, 1, 1, 1, 1, 1, 0, 1, 1);
            string SenderID_ID_regex = validation.validate(Convert.ToString(obj.SenderID_ID), 0, 1, 1, 1, 1, 1, 1, 1, 1);

            int Customer_ID = Convert.ToInt32(CompanyInfo.Decrypt(obj_1.Id, true));
            string Customer_Id_regex = validation.validate(Convert.ToString(Customer_ID), 0, 1, 1, 1, 1, 1, 1, 1, 1);
            List<Model.DocumentDetails> _lst = new List<Model.DocumentDetails>();
            if (Client_ID_regex != "false" && IDType_ID_regex != "false" && ID_Name_regex != "false" && SenderID_ID_regex != "false" && Customer_Id_regex != "false" && Beneficiary_ID_regex != "false")
            {
                MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("IDDoc_List");
                _cmd.CommandType = CommandType.StoredProcedure;
                int benf_id = Convert.ToInt32(obj.Beneficiary_ID);

                //     _cmd.Parameters.AddWithValue("_Customer_ID", obj.Customer.Id);
                _cmd.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                string _whereclause = "";

                if (obj.SenderID_ID > 0)
                {
                    _whereclause = " and SenderID_ID =" + obj.SenderID_ID;
                }
                if (obj.IDType_ID == 1)
                {
                    _whereclause = _whereclause + " and documents_details.IDType_ID=" + obj.IDType_ID;
                }

                if (obj.ID_Name != "" && obj.ID_Name != null)
                {
                    _whereclause = _whereclause + " and ID_Name LIKE '%" + obj.ID_Name + "%'";
                }
                if (benf_id != -1 && benf_id != null && benf_id != 0)
                {
                    _whereclause = _whereclause + " and Beneficiary_ID= " + benf_id + "";
                }
                string get_flag = obj.Comments;
                if (get_flag == "from_id_documents")
                {
                    _whereclause = _whereclause + " and ( Beneficiary_ID is null  OR Beneficiary_ID=0) ";
                }
                if (Customer_ID != -1 && Customer_ID != 0)
                {
                    if (get_flag == "from_benf_id_documents")
                    {
                        //   _whereclause = _whereclause + " and ( Customer_ID is null  OR Customer_ID=0) ";
                        //_whereclause = _whereclause + " and ( Customer_ID is null  OR Customer_ID=0) ";
                    }
                    else
                    {
                        if (obj.SenderID_ID > 0)
                        {
                        }
                        else
                        {
                            _whereclause = _whereclause + " and customer_id= " + Customer_ID + "";
                        }

                    }

                }

                //else
                //{
                //    _whereclause = _whereclause + " and ( Beneficiary_ID= null OR Beneficiary_ID=0) ";
                //}
                _cmd.Parameters.AddWithValue("_whereclause", _whereclause);
                dt = db_connection.ExecuteQueryDataTableProcedure(_cmd);
                if (dt != null && dt.Rows.Count > 0)
                {
                    DataTable dtc = CompanyInfo.get(obj.Client_ID, context);
                    //context = HttpContext.Current;
                    foreach (DataRow row in dt.Rows)
                    {
                        if (obj.SenderID_ID > 0)
                        {
                            if (row["BackID_Document"] != DBNull.Value && row["BackID_Document"].ToString() != "")
                            {

                                string image_link = (dtc.Rows[0]["Company_URL_Admin"]).ToString() + row["BackID_Document"].ToString();

                                string base64str = CompanyInfo.ConvertImageLinkToBase64(image_link);
                                row["BackID_Document"] = base64str;
                            }
                            if (row["FileNameWithExt"] != DBNull.Value && row["FileNameWithExt"].ToString() != "")
                            {

                                string image_link = (dtc.Rows[0]["Company_URL_Admin"]).ToString() + row["FileNameWithExt"].ToString();

                                string base64str = CompanyInfo.ConvertImageLinkToBase64(image_link);
                                row["FileNameWithExt"] = base64str;
                            }

                        }
                        else
                        {
                            if (row["BackID_Document"] != DBNull.Value && row["BackID_Document"].ToString() != "")
                            {


                                row["BackID_Document"] = "";
                            }
                            if (row["FileNameWithExt"] != DBNull.Value && row["FileNameWithExt"].ToString() != "")
                            {

                                row["FileNameWithExt"] = "";
                            }
                        }
                    }


                }
            }
            else
            {
                string msg = "Validation Error Client_ID_regex- " + Client_ID_regex + " IDType_ID_regex- " + IDType_ID_regex + "ID_Name_regex-" + ID_Name_regex + " SenderID_ID_regex-" + SenderID_ID_regex + "Customer_Id_regex-" + Customer_Id_regex + "Beneficiary_ID_regex-" + Beneficiary_ID_regex;
                int stattus = (int)CompanyInfo.InsertActivityLogDetailsSecurity(msg, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), 1, "srvDocument", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "Select_Document_List", 0, context);

            }
            return dt;
        }


        public DataTable select_idconfig(Model.Document obj, HttpContext context)// for customer
        {

            DataTable dt_id_config = new DataTable();
            string IDType_ID_regex = CompanyInfo.testInjection(Convert.ToString(obj.IDType_ID));
            string Client_ID_regex = CompanyInfo.testInjection(Convert.ToString(obj.Client_ID));
            string Branch_ID_regex = CompanyInfo.testInjection(Convert.ToString(obj.Branch_ID));
            string Client_ID1_regex = validation.validate(Convert.ToString(obj.Client_ID), 0, 1, 1, 1, 1, 1, 1, 1, 1);
            string IDType_ID1_regex = validation.validate(Convert.ToString(obj.IDType_ID), 0, 1, 1, 1, 1, 1, 1, 1, 1);
            string Country_ID1_regex = validation.validate(Convert.ToString(obj.Branch_ID), 0, 1, 1, 1, 1, 1, 1, 1, 1);


            if (IDType_ID_regex == "1" && Client_ID_regex == "1" && Branch_ID_regex == "1" && Client_ID1_regex != "false" && IDType_ID1_regex != "false" && Country_ID1_regex != "false")
            {

                string clause = string.Empty;
                string Whereclause = "";
                //Get_MainModules  c.Flag = Convert.ToString(dictObjMain["permission"]);
                if (obj.IDType_ID > 0)
                {
                    Whereclause = Whereclause + " and ID_Type = " + obj.IDType_ID;
                }
                MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("sp_select_ID_Config");
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.Parameters.AddWithValue("_ClientId", obj.Client_ID);
                _cmd.Parameters.AddWithValue("_BranchId", obj.Branch_ID);
                _cmd.Parameters.AddWithValue("_Whereclause", Whereclause);
                dt_id_config = db_connection.ExecuteQueryDataTableProcedure(_cmd);
            }
            else
            {
                string msg = "Validation Error Client_ID1_regex-" + Client_ID1_regex + "Country_ID1_regex-" + Country_ID1_regex + "IDType_ID1_regex- " + IDType_ID1_regex;
                int stattus = (int)CompanyInfo.InsertActivityLogDetailsSecurity(msg, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), 1, "srvDocument", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "select_idconfig", 0, context);
            }
            return dt_id_config;
        }


        public DataTable GetToken(Model.Document obj, HttpContext context)
        {
            int Customer_ID = Convert.ToInt32(CompanyInfo.Decrypt(obj.Customer_ID, true));

            DataTable dt = new DataTable();
            dt.Columns.Add("Status", typeof(int));
            dt.Columns.Add("RedirectURL", typeof(string));
            string idscan_defaultinput = "";
            var externalUserId = "";
            string applicant_id = ""; // 65e80e1c62c95a48a68f6a5c
            string externaluser_id = ""; // MX01237486e8f085f7
            try
            {
                MySqlConnector.MySqlCommand cmd = new MySqlConnector.MySqlCommand("select IDScan_UsedCount from customer_aml_table where date(IDScan_Date)=date(SYSDATE()) and Customer_ID = @Customer_ID and Client_ID=@Client_ID");
                cmd.Parameters.AddWithValue("@Client_ID", obj.Client_ID);
                cmd.Parameters.AddWithValue("@Customer_ID", Customer_ID);// API Status
                string usedc = Convert.ToString(db_connection.ExecuteScalarProcedure(cmd));
                if (usedc != null && usedc != "")
                {
                    if (Convert.ToInt32(usedc) >= 1)
                    {
                        idscan_defaultinput = "FILESYSTEM";//"CAMERA";OR"FILESYSTEM"
                    }
                }
            }
            catch { }
            try
            {
                DataTable dtc = CompanyInfo.get(obj.Client_ID, context);
                string appurl = Convert.ToString(dtc.Rows[0]["idscan_url"]);
                //string appurl = Convert.ToString(dtc.Rows[0]["Company_website"]);//"https://currencygenie.co.uk/";//"http://localhost:32720/";//Convert.ToString(dtc.Rows[0]["Company_website"]);// or  ["Company_URL_Admin"] //
                string Status = string.Empty;
                string[] response = { Status.ToString() };
                string Applicat_ID = "";
                string urll = "", UserName = "", Password = "";
                int API_ID = 3;//GBG API ID
                obj.Record_Insert_DateTime = CompanyInfo.gettime(obj.Client_ID, context);
                string company_url = Convert.ToString(dtc.Rows[0]["Company_URL_Customer"]);
                string apiUser_ID = Convert.ToString(dtc.Rows[0]["APIUser_ID"]);
                string apiAccess_Code = Convert.ToString(dtc.Rows[0]["APIAccess_Code"]);
                string reviewStatus = "";
                string reviewAnswer = "";

                MySqlConnector.MySqlCommand cmdp_active = new MySqlConnector.MySqlCommand("active_thirdparti_api");
                cmdp_active.CommandType = CommandType.StoredProcedure;
                DataTable dtApi_active = db_connection.ExecuteQueryDataTableProcedure(cmdp_active);
                if (dtApi_active.Rows.Count > 0)
                {
                    API_ID = Convert.ToInt32(dtApi_active.Rows[0]["API_ID"]);
                }

                MySqlConnector.MySqlCommand cmd = new MySqlConnector.MySqlCommand("GetAPIDetails");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("_API_ID", API_ID);//GBG API ID or Shuftipro
                cmd.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                cmd.Parameters.AddWithValue("_status", 0);// API Status
                DataTable dtApi = db_connection.ExecuteQueryDataTableProcedure(cmd);
                if (dtApi.Rows.Count > 0)
                {
                    urll = Convert.ToString(dtApi.Rows[0]["API_URL"]);
                    UserName = Convert.ToString(dtApi.Rows[0]["UserName"]);
                    Password = Convert.ToString(dtApi.Rows[0]["Password"]);
                }

                if (API_ID == 3)
                {
                    dt.Columns.Add("journeyid", typeof(string));
                    dt.Columns.Add("redirecturl", typeof(string));
                    dt.Columns.Add("qrcode", typeof(string));
                    dt.Columns.Add("gbgtoken", typeof(string));
                    dt.Columns.Add("uniquevalue", typeof(string));
                    dt.Columns.Add("redirectsuccessurl", typeof(string));


                    try
                    {
                        MySqlConnector.MySqlCommand cmd1252 = new MySqlConnector.MySqlCommand("GetCustDetailsByID");
                        cmd1252.CommandType = CommandType.StoredProcedure;
                        cmd1252.Parameters.AddWithValue("cust_ID", Customer_ID);
                        DataTable dtApi1252 = db_connection.ExecuteQueryDataTableProcedure(cmd1252);

                        if (dtApi1252.Rows.Count > 0)
                        {
                            obj.Custmer_Ref = Convert.ToString(dtApi1252.Rows[0]["WireTransfer_ReferanceNo"]);
                        }
                    }
                    catch
                    {

                    }


                    int Flag = 11;
                    string currentDateTime = CompanyInfo.gettime(obj.Client_ID, context);

                    DateTime currentDateTime1 = Convert.ToDateTime(currentDateTime);
                    MySqlConnector.MySqlCommand checkCmd = new MySqlConnector.MySqlCommand("GetLatestCustomerIDScan");
                    checkCmd.CommandType = CommandType.StoredProcedure;
                    checkCmd.Parameters.AddWithValue("p_Customer_ID", Customer_ID);
                    checkCmd.Parameters.AddWithValue("p_Client_ID", obj.Client_ID);
                    checkCmd.Parameters.AddWithValue("p_Delete_Status", 0);

                    DataTable existingScan = db_connection.ExecuteQueryDataTableProcedure(checkCmd);

                    if (existingScan.Rows.Count > 0 && 1 == 2)
                    {
                        DateTime recordInsertDateTime = Convert.ToDateTime(existingScan.Rows[0]["Record_Insert_Date_Time"]);
                        TimeSpan timeDiff = currentDateTime1 - recordInsertDateTime;

                        if (timeDiff.TotalHours <= 24)
                        {
                            string ID_Scan_Url = Convert.ToString(existingScan.Rows[0]["ID_Scan_Url"]);
                            string journeyid = Convert.ToString(existingScan.Rows[0]["journeyid"]);
                            string redirecturl = Convert.ToString(existingScan.Rows[0]["redirecturl"]);
                            string qrcode = Convert.ToString(existingScan.Rows[0]["qrcode"]);
                            string gbgtoken = Convert.ToString(existingScan.Rows[0]["gbgtoken"]);
                            string uniquevalue = Convert.ToString(existingScan.Rows[0]["uniquevalue"]);
                            string redirectsuccessurl = Convert.ToString(existingScan.Rows[0]["redirectsuccessurl"]);

                            dt.Rows.Add(0, ID_Scan_Url, journeyid, redirecturl, qrcode, gbgtoken, uniquevalue, redirectsuccessurl);

                            MySqlConnector.MySqlCommand checkCmd12 = new MySqlConnector.MySqlCommand("UpdateCustomerIDScan");
                            checkCmd12.CommandType = CommandType.StoredProcedure;
                            checkCmd12.Parameters.AddWithValue("p_Customer_ID", Customer_ID);
                            checkCmd12.Parameters.AddWithValue("p_Client_ID", obj.Client_ID);
                            checkCmd12.Parameters.AddWithValue("p_Delete_Status", 1);

                            int n = db_connection.ExecuteNonQueryProcedure(checkCmd12);
                            return dt;
                        }
                        else
                        {
                            Flag = 1;
                            MySqlConnector.MySqlCommand checkCmd12 = new MySqlConnector.MySqlCommand("UpdateCustomerIDScan");
                            checkCmd12.CommandType = CommandType.StoredProcedure;
                            checkCmd12.Parameters.AddWithValue("p_Customer_ID", Customer_ID);
                            checkCmd12.Parameters.AddWithValue("p_Client_ID", obj.Client_ID);
                            checkCmd12.Parameters.AddWithValue("p_Delete_Status", 1);

                            int n = db_connection.ExecuteNonQueryProcedure(checkCmd12);
                        }
                    }
                    else
                    {
                        Flag = 1;
                    }

                    if (Flag == 1)
                    {


                        string ProfileID = "", journeyid = "", redirecturl = "";
                        string env = "", lang = "", method = "", qrapiurl = "", custNo = Convert.ToString(Customer_ID);
                        string fileselection = "";
                        if (dtApi.Rows.Count > 0)
                        {
                            ProfileID = Convert.ToString(dtApi.Rows[0]["ProfileID"]);

                            if (ProfileID != "" && ProfileID != null)
                            {
                                Newtonsoft.Json.Linq.JObject objdata = Newtonsoft.Json.Linq.JObject.Parse(ProfileID);
                                journeyid = Convert.ToString(objdata["journeyid"]);
                                redirecturl = Convert.ToString(objdata["redirecturl"]);
                                env = Convert.ToString(objdata["env"]);
                                lang = Convert.ToString(objdata["lang"]);
                                method = Convert.ToString(objdata["method"]);
                                qrapiurl = Convert.ToString(objdata["qrapiurl"]);
                                try
                                {
                                    fileselection = Convert.ToString(objdata["fileselection"]);
                                }
                                catch (Exception egb) { fileselection = "F"; }
                            }
                        }

                        WebResponse response2 = Token(urll, UserName, Password, context);

                        string base64String = string.Empty;
                        using (var reader = new StreamReader(response2.GetResponseStream()))
                        {
                            string ApiStatus = reader.ReadToEnd();//"";//                                        
                            string s13 = string.Empty;
                            var entries = ApiStatus.TrimStart('{').TrimEnd('}').Replace("\"", String.Empty).Split(',');
                            foreach (var entry in entries)
                            {
                                if (entry.Split(':')[0] == "access_token")
                                {
                                    s13 = entry.Split(':')[1];

                                    string qrcode = "", redirectgbgurl = "", uniquevalue = "";
                                    DataTable li1 = gbgqrcode(qrapiurl, s13, journeyid, env, lang, method, custNo, context, obj.Client_ID, idscan_defaultinput, redirecturl, fileselection);
                                    foreach (DataRow dr in li1.Rows)
                                    {
                                        qrcode = Convert.ToString(dr["qrcode"]);
                                        redirectgbgurl = Convert.ToString(dr["redirecturl"]);
                                        uniquevalue = Convert.ToString(dr["uniquevalue"]);
                                    }
                                    try
                                    {
                                        string p_ID_Scan_Url = "" + appurl + "?id=" + HttpUtility.UrlEncode(CompanyInfo.Encrypt(Convert.ToString(Customer_ID), true))
                                    + "&key=" + s13 + "&defaultinput=" + HttpUtility.UrlEncode(idscan_defaultinput);

                                        obj.Record_Insert_DateTime = CompanyInfo.gettime(obj.Client_ID, context);

                                        MySqlConnector.MySqlCommand cmd12 = new MySqlConnector.MySqlCommand("InsertCustomerIDScanJournyDetails");
                                        cmd12.CommandType = CommandType.StoredProcedure;
                                        cmd12.Parameters.AddWithValue("p_Customer_ID", custNo);
                                        cmd12.Parameters.AddWithValue("p_ID_Scan_Url", p_ID_Scan_Url);
                                        cmd12.Parameters.AddWithValue("p_Record_Insert_Date_Time", obj.Record_Insert_DateTime);
                                        cmd12.Parameters.AddWithValue("p_API_ID", API_ID);
                                        cmd12.Parameters.AddWithValue("p_Client_ID", obj.Client_ID);
                                        cmd12.Parameters.AddWithValue("p_Branch_ID", obj.Branch_ID);
                                        cmd12.Parameters.AddWithValue("p_Delete_Status", 0);
                                        cmd12.Parameters.AddWithValue("p_journeyid", journeyid);
                                        cmd12.Parameters.AddWithValue("p_redirecturl", redirectgbgurl);
                                        cmd12.Parameters.AddWithValue("p_qrcode", qrcode);
                                        cmd12.Parameters.AddWithValue("p_gbgtoken", s13);
                                        cmd12.Parameters.AddWithValue("p_uniquevalue", uniquevalue);
                                        cmd12.Parameters.AddWithValue("p_redirectsuccessurl", redirecturl);
                                        int a = db_connection.ExecuteNonQueryProcedure(cmd12);
                                    }
                                    catch
                                    {

                                    }

                                    dt.Rows.Add(0, "" + appurl + "?id=" + HttpUtility.UrlEncode(CompanyInfo.Encrypt(Convert.ToString(Customer_ID), true))
                                    //dt.Rows.Add(0, "" + appurl + "idscan/index.html?id=" + HttpUtility.UrlEncode(CompanyInfo.Encrypt(Convert.ToString(obj.Customer_ID), true))
                                    //dt.Rows.Add(0, "http://localhost:8307/sangerwal-idscan/index.html?id=" + HttpUtility.UrlEncode(CompanyInfo.Encrypt(Convert.ToString(obj.Customer_ID), true))
                                    + "&key=" + s13 + "&defaultinput=" + HttpUtility.UrlEncode(idscan_defaultinput), journeyid, redirectgbgurl, qrcode, s13, uniquevalue, redirecturl);//http://localhost:1698/WSDK_9.1.3/index.html
                                }
                            }
                        }
                    }
                }
                if (API_ID == 9)
                {
                    string email = obj.customer_email;//"john.smith@sumsub.com";
                    string phone = obj.customer_mobile;//"+449112081223";
                    string country = obj.country_code;//"GBR";
                    string placeOfBirth = obj.city;
                    string street = obj.strete; // Insert the actual street value here
                    string town = obj.Adderess;   // Insert the actual town value here
                    string postCode = obj.post_code;// Insert the actual postCode value here
                    string countryAddress = obj.country_code;

                    cmd = new MySqlCommand("Check_applicant_exist");
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("_customer_id", Customer_ID);// API Status
                    dtApi = db_connection.ExecuteQueryDataTableProcedure(cmd);

                    foreach (DataRow row in dtApi.Rows)
                    {
                        Applicat_ID = Convert.ToString(row["Parameter"]);
                        string[] parts = Applicat_ID.Split('=');

                        // Assuming there are always two parts separated by '='
                        if (parts.Length == 2)
                        {
                            Applicat_ID = parts[0]; // 65e80e1c62c95a48a68f6a5c
                            externaluser_id = parts[1]; // MX01237486e8f085f7
                        }
                    }

                    var stamp = ((long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds).ToString();

                    // Generate the externalUserId
                    if (externaluser_id == "")
                    {
                        externalUserId = obj.Custmer_Ref;
                    }
                    else
                    {
                        externalUserId = externaluser_id;
                    }

                    // Construct the JSON body
                    var body = new
                    {
                        externalUserId,
                        email,
                        phone,
                        fixedInfo = new
                        {


                            addresses = new[]
                            {
                new
                {
                    street = obj.strete,
                    town,
                    placeOfBirth,
                    postCode
                }
            }
                        }
                    };

                    var bodyJson = JsonConvert.SerializeObject(body);

                    // Calculate the value to sign
                    var valueToSign = stamp + "POST" + "/resources/applicants?levelName=basic-kyc-level" + bodyJson;

                    // Calculate the signature
                    var secretKey = Password; // Replace with your secret key
                    var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey));
                    var signatureBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(valueToSign));
                    var signature = BitConverter.ToString(signatureBytes).Replace("-", "").ToLower();

                    // Set the request parameters
                    var appToken = UserName;

                    var restClient = new RestClient(urll);
                    var requesttt = new RestRequest("/resources/applicants?levelName=basic-kyc-level", Method.POST);

                    requesttt.AddHeader("Content-Type", "application/json");
                    requesttt.AddHeader("X-App-Token", appToken);
                    requesttt.AddHeader("X-App-Access-Ts", stamp);
                    requesttt.AddHeader("X-App-Access-Sig", signature);
                    requesttt.AddParameter("application/json", bodyJson, ParameterType.RequestBody);

                    var responsed = restClient.Execute(requesttt);
                    dynamic dynJsonn = JsonConvert.DeserializeObject(responsed.Content);

                    // Second part of your code starts here

                    stamp = ((long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds).ToString();
                    string userId = externalUserId;
                    // Calculate the value to sign (excluding the body)
                    valueToSign = stamp + "POST" + $"/resources/accessTokens?userId={userId}&levelName=basic-kyc-level";

                    // Calculate the signature
                    hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey));
                    signatureBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(valueToSign));
                    signature = BitConverter.ToString(signatureBytes).Replace("-", "").ToLower();

                    // Set your dynamic userId here

                    var client = new RestClient(urll);
                    var request = new RestRequest($"/resources/accessTokens?userId={userId}&levelName=basic-kyc-level", Method.POST);
                    request.AddHeader("Content-Type", "application/json");
                    request.AddHeader("X-App-Token", appToken);
                    request.AddHeader("X-App-Access-Ts", stamp);
                    request.AddHeader("X-App-Access-Sig", signature);

                    var responseT = client.Execute(request);
                    JObject jsonResponse = JObject.Parse(responseT.Content);

                    // Extract values
                    string Token = jsonResponse["token"].ToString();
                    string UserId = jsonResponse["userId"].ToString();
                    //Console.WriteLine(responseT.Content);



                    if (dtApi.Rows.Count > 0)
                    {

                        stamp = ((long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds).ToString();
                        userId = externaluser_id;
                        applicant_id = Applicat_ID;
                        // Calculate the value to sign (excluding the body)
                        valueToSign = stamp + "GET" + $"/resources/applicants/{applicant_id}/one";


                        appToken = UserName;
                        // Replace placeholders with actual values
                        valueToSign = valueToSign.Replace("{{sumsub_root_url}}", "")
                                                 .Replace("{{applicantId}}", "")
                                                 .Replace("{{levelName}}", "basic-kyc-level")
                                                 .Replace("{{userId}}", userId);

                        // Calculate the signature
                        secretKey = Password; // Replace with your secret key
                        hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey));
                        signatureBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(valueToSign));
                        signature = BitConverter.ToString(signatureBytes).Replace("-", "").ToLower();

                        // Set your dynamic userId here

                        var client1 = new RestClient(urll);
                        var request1 = new RestRequest($"/resources/applicants/{applicant_id}/one", Method.GET);
                        request1.AddHeader("Content-Type", "application/json");
                        request1.AddHeader("X-App-Token", appToken);
                        request1.AddHeader("X-App-Access-Ts", stamp);
                        request1.AddHeader("X-App-Access-Sig", signature);
                        responseT = client1.Execute(request1);
                        jsonResponse = JObject.Parse(responseT.Content);


                        // CompanyInfo.InsertActivityLogDetails("Get The Sumsub Docs Details and applicantID 6 for Id scan " + responseT.Content + applicant_id + ". ", Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(Customer_ID), "GetJourney", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "ID Upload");

                        //activity_log = "Get The Sumsub Docs Details and applicantID 6 for Id scan " + responseT.Content + applicant_id + "";

                        // Accessing values from the response

                        try
                        {
                            string id = (string)jsonResponse["id"];
                            DateTime createdAt = (DateTime)jsonResponse["createdAt"];
                            string key = (string)jsonResponse["key"];
                            string clientId = (string)jsonResponse["clientId"];

                            externalUserId = (string)jsonResponse["externalUserId"];

                            JObject review = (JObject)jsonResponse["review"];

                            reviewStatus = (string)review["reviewStatus"];

                            // Accessing nested objects
                            JObject reviewResult = (JObject)review["reviewResult"];
                            reviewAnswer = (string)reviewResult["reviewAnswer"];

                        }
                        catch (Exception ex)
                        {

                        }

                        string returnFinalURL = company_url +
                                "idscan/index?SumsubIDScan" +
                                "&Token=" + Uri.EscapeDataString(Token) +
                                "&appliocantID=" + Uri.EscapeDataString(HttpUtility.UrlEncode(CompanyInfo.Encrypt(Convert.ToString(Customer_ID), true))) +
                                "&Querystring=" + Uri.EscapeDataString("" + appurl + "?id=" + HttpUtility.UrlEncode(CompanyInfo.Encrypt(Convert.ToString(Customer_ID), true))

                                   + "&key=" + Token + "&defaultinput=" + HttpUtility.UrlEncode(applicant_id)) +
                                "&APIUser_ID=" + Uri.EscapeDataString(apiUser_ID) +
                                "&APIAccess_Code=" + Uri.EscapeDataString(apiAccess_Code) +
                                "&Client_ID=" + Uri.EscapeDataString(Convert.ToString(obj.Client_ID)) +
                                "&Branch_ID=" + Uri.EscapeDataString(Convert.ToString(obj.Branch_ID)) +
                                "&applicant_id=" + Uri.EscapeDataString(HttpUtility.UrlEncode(applicant_id)) +
                                "&Custmer_Ref=" + Uri.EscapeDataString(obj.Custmer_Ref) +
                                "&Customer_ID=" + Uri.EscapeDataString(CompanyInfo.Encrypt(Convert.ToString(Customer_ID), true)) +
                                "&IDType_ID=" + Uri.EscapeDataString("1");

                        if (reviewAnswer == "GREEN" && reviewStatus == "completed")
                        {

                            /* dt.Rows.Add(5, "" + appurl + "?id=" + HttpUtility.UrlEncode(CompanyInfo.Encrypt(Convert.ToString(Customer_ID), true))

                                + "&key=" + Token + "&defaultinput=" + HttpUtility.UrlEncode(Applicat_ID));*/
                            dt.Rows.Add(5, returnFinalURL);

                        }
                        else
                        {
                            dt.Rows.Add(1, returnFinalURL);

                            /* dt.Rows.Add(1, "" + appurl + "?id=" + HttpUtility.UrlEncode(CompanyInfo.Encrypt(Convert.ToString(Customer_ID), true))

                                  + "&key=" + Token + "&defaultinput=" + HttpUtility.UrlEncode(Applicat_ID));*/
                        }


                    }
                    else
                    {
                        string returnFinalURL = company_url +
                                "idscan/index?SumsubIDScan" +
                                "&Token=" + Uri.EscapeDataString(Token) +
                                "&appliocantID=" + Uri.EscapeDataString(HttpUtility.UrlEncode(CompanyInfo.Encrypt(Convert.ToString(Customer_ID), true))) +
                                "&Querystring=" + Uri.EscapeDataString("" + appurl + "?id=" + HttpUtility.UrlEncode(CompanyInfo.Encrypt(Convert.ToString(Customer_ID), true))

                                   + "&key=" + Token + "&defaultinput=" + HttpUtility.UrlEncode(applicant_id)) +
                                "&APIUser_ID=" + Uri.EscapeDataString(apiUser_ID) +
                                "&APIAccess_Code=" + Uri.EscapeDataString(apiAccess_Code) +
                                "&Client_ID=" + Uri.EscapeDataString(Convert.ToString(obj.Client_ID)) +
                                "&Branch_ID=" + Uri.EscapeDataString(Convert.ToString(obj.Branch_ID)) +
                                "&applicant_id=" + Uri.EscapeDataString(HttpUtility.UrlEncode(applicant_id)) +
                                "&Custmer_Ref=" + Uri.EscapeDataString(obj.Custmer_Ref) +
                                "&Customer_ID=" + Uri.EscapeDataString(CompanyInfo.Encrypt(Convert.ToString(Customer_ID), true)) +
                                "&IDType_ID=" + Uri.EscapeDataString("1");

                        /*dt.Rows.Add(1, "" + appurl + "?id=" + HttpUtility.UrlEncode(CompanyInfo.Encrypt(Convert.ToString(Customer_ID), true))

                              + "&key=" + Token + "&defaultinput=" + HttpUtility.UrlEncode(Applicat_ID));*/
                        dt.Rows.Add(1, returnFinalURL);
                    }


                }
                if (API_ID == 5)
                {
                    string url_scan = "";
                    if (dtApi.Rows.Count > 0)
                    {
                        url_scan = Convert.ToString(dtApi.Rows[0]["ProfileID"]);
                    }
                    string isocode = "GB";
                    /*
                    MySqlCommand cmd_iso = new MySqlCommand("SELECT cm.ISO_Code as isocode FROM  country_master as cm , customer_registration as c where c.Country_ID = cm.Country_ID and c.Customer_ID = " + obj.Customer_ID + " ");
                    DataTable dttiso = db_connection.ExecuteQueryDataTableProcedure(cmd_iso);
                    if (dttiso.Rows.Count > 0)
                    {
                        isocode = Convert.ToString(dttiso.Rows[0]["isocode"]);
                    }
                    cmd_iso.Dispose();
                    */
                    MySqlConnector.MySqlCommand cmdp = new MySqlConnector.MySqlCommand("getiso_data");
                    cmdp.CommandType = CommandType.StoredProcedure;
                    cmdp.Parameters.AddWithValue("_Cust_ID", Customer_ID);
                    DataTable dtiso = db_connection.ExecuteQueryDataTableProcedure(cmdp);
                    if (dtApi.Rows.Count > 0)
                    {
                        isocode = Convert.ToString(dtiso.Rows[0]["isocode"]);
                    }

                    if (idscan_defaultinput == "FILESYSTEM")
                    {
                        idscan_defaultinput = "File";
                    }

                    MySqlConnector.MySqlCommand cmdcustdata = new MySqlConnector.MySqlCommand("Customer_RegDetails");
                    cmdcustdata.CommandType = CommandType.StoredProcedure;
                    cmdcustdata.Parameters.AddWithValue("_Customer_ID", Customer_ID);
                    cmdcustdata.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                    DataTable dt_custdata = db_connection.ExecuteQueryDataTableProcedure(cmdcustdata);
                    string cfname = "", clname = "", cbd = "";
                    if (dt_custdata.Rows.Count > 0)
                    {
                        cfname = Convert.ToString(dt_custdata.Rows[0]["First_Name"]);
                        clname = Convert.ToString(dt_custdata.Rows[0]["Last_Name"]);
                        cbd = Convert.ToString(dt_custdata.Rows[0]["DateOf_Birth"]);

                        if (cbd != "" || cbd != null)
                        {
                            try
                            {
                                cbd = cbd.Split()[0].Trim();
                                DateTime dtdd = DateTime.Parse(cbd);
                                cbd = dtdd.ToString("yyyy-MM-dd");
                            }
                            catch (Exception ex)
                            {
                            }
                        }
                    }
                    string strdata = "&cfname=" + cfname + "&cmname=" + clname + "&cbd=" + cbd;

                    dt.Rows.Add(0, url_scan + "?add=" + 0 + "&secretkey=" + UserName + "&clientkey=" + Password + "&iso=" + isocode + "&defaultinput=" + HttpUtility.UrlEncode(idscan_defaultinput) + strdata);
                }
                if (API_ID == 7)
                {
                    string url_scan = "", isocode = "GB";
                    dt.Columns.Add("veriffsessionid", typeof(string));
                    MySqlConnector.MySqlCommand cmdp = new MySqlConnector.MySqlCommand("getiso_data");
                    cmdp.CommandType = CommandType.StoredProcedure;
                    cmdp.Parameters.AddWithValue("_Cust_ID", Customer_ID);
                    DataTable dtiso = db_connection.ExecuteQueryDataTableProcedure(cmdp);
                    if (dtiso.Rows.Count > 0)
                    {
                        isocode = Convert.ToString(dtiso.Rows[0]["isocode"]);
                    }
                    isocode = "\"" + isocode + "\"";
                    MySqlConnector.MySqlCommand cmdcustdata = new MySqlConnector.MySqlCommand("Customer_RegDetails");
                    cmdcustdata.CommandType = CommandType.StoredProcedure;
                    cmdcustdata.Parameters.AddWithValue("_Customer_ID", Customer_ID);
                    cmdcustdata.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                    DataTable dt_custdata = db_connection.ExecuteQueryDataTableProcedure(cmdcustdata);
                    string cfname = "", clname = "", cbd = "";
                    if (dt_custdata.Rows.Count > 0)
                    {
                        cfname = "\"" + Convert.ToString(dt_custdata.Rows[0]["First_Name"]) + "\"";
                        clname = "\"" + Convert.ToString(dt_custdata.Rows[0]["Last_Name"]) + "\"";
                        cbd = "\"" + Convert.ToString(dt_custdata.Rows[0]["DateOf_Birth"]) + "\"";

                        if (cbd != "" || cbd != null)
                        {
                            try
                            {
                                cbd = cbd.Split()[0].Trim();
                                DateTime dtdd = DateTime.Parse(cbd);
                                cbd = dtdd.ToString("yyyy-MM-dd");
                            }
                            catch (Exception ex)
                            {
                            }
                        }
                    }


                    //                string baseUrl = System.Web.HttpContext.Current.Request.Url.Scheme + "://" 
                    //+ System.Web.HttpContext.Current.Request.Url.Authority
                    //+ System.Web.HttpContext.Current.Request.ApplicationPath.TrimEnd('/') + '/'; 

                    string baseUrl = obj.Path;



                    var callback_url = "\"" + baseUrl + "id-scan-confirmation.html" + "\"";
                    //callback_url = "\"" + "https://currencyexchangesoftware.eu/csremit-customer/id-scan-confirmation.html" + "\"";

                    var client = new RestClient(urll);
                    client.Timeout = -1;
                    var request = new RestRequest(Method.POST);
                    request.AddHeader("Content-Type", "application/json");
                    request.AddHeader("X-AUTH-CLIENT", UserName);

                    var body = @"{
                        " + "\n" +
                        @"    ""verification"":
                        " + "\n" +
                        @"{
                        " + "\n" +
                        @"""callback"":" + callback_url + "," +
                        @"""person"":
                        " + "\n" +
                        @"{
                        " + "\n" +
                        @"    ""firstName"":" + cfname + "," +
                        @"""lastName"":" + clname + "" +
                        @"},
                        " + "\n" +
                        @"""document"":
                        " + "\n" +
                        @"{" +
                        @"""country"":" + isocode + "" +
                        @"},
                        " + "\n" +
                        @"""vendorData"":""""
                        " + "\n" +
                        @"}
                        " + "\n" +
                        @"}";

                    request.AddParameter("application/json", body, ParameterType.RequestBody);
                    IRestResponse responseData = client.Execute(request);
                    Console.WriteLine(responseData.Content);

                    dynamic dynJson = Newtonsoft.Json.JsonConvert.DeserializeObject(responseData.Content);
                    string responseCode = dynJson.status;

                    if (responseCode == "success")
                    {
                        string sessionId = dynJson.verification.id;
                        url_scan = dynJson.verification.url;
                        dt.Rows.Add(0, url_scan, sessionId);
                    }


                }
                if (API_ID == 14)
                {

                    string API_Codes = Convert.ToString(dtApi.Rows[0]["ProfileID"]);
                    Newtonsoft.Json.Linq.JObject o = Newtonsoft.Json.Linq.JObject.Parse(API_Codes);

                    // string return_success_url = Convert.ToString(o["return_success_url"])+"&ref="+ refNumber;
                    string return_success_url = Convert.ToString(o["successurl"]);
                    string return_failure_url = Convert.ToString(o["failedurl"]);
                    string webhookendpoint = Convert.ToString(o["webhookendpoint"]);
                    string PEM_PATH = Convert.ToString(o["PEM_PATH"]);
                    string YOTI_CLIENT_SDK_ID = Convert.ToString(o["YOTI_CLIENT_SDK_ID"]);
                    string yotiurl = Convert.ToString(o["yotiurl"]);
                    string YOTI_IDV_API_URL = Convert.ToString(o["YOTI_IDV_API_URL"]);
                    // const string YOTI_CLIENT_SDK_ID = "2f352d26-506c-4526-9024-9bc486a09237";
                    // const string PEM_PATH = "D://Ongoing chnages//Yoti//sandbox-tassa-test//privateKey.pem";
                    // string customerId = "MX568587346234857";
                    StreamReader privateKeyStream = System.IO.File.OpenText(PEM_PATH);
                    var key = CryptoEngine.LoadRsaKey(privateKeyStream);
                    //_docScanClient = new DocScanClient(YOTI_CLIENT_SDK_ID, key, new HttpClient(), new Uri("https://api.yoti.com/sandbox/idverify/v1"));

                    var docScanClient = new DocScanClient(YOTI_CLIENT_SDK_ID, key, new HttpClient(), new Uri(YOTI_IDV_API_URL));//"https://api.yoti.com/sandbox/idverify/v1"
                    var sessionSpec = new SessionSpecificationBuilder()
                    .WithClientSessionTokenTtl(600)
                    .WithResourcesTtl(42336000)
                    .WithUserTrackingId(obj.Custmer_Ref)
                    .WithRequestedCheck(
                    new RequestedDocumentAuthenticityCheckBuilder()
                    .Build()
                    )
                    .WithRequestedCheck(
                    new RequestedLivenessCheckBuilder()
                    .ForStaticLiveness()
                    .WithMaxRetries(3)
                    .Build()
                    )
                    .WithRequestedCheck(
                    new RequestedFaceMatchCheckBuilder()
                    .WithManualCheckFallback()
                    .Build()
                    )
                    .WithRequestedCheck(
                        new RequestedWatchlistScreeningCheckBuilder()
                            //.ForAdverseMedia()
                            .ForSanctions()
                            .Build()
                        )
                    .WithRequestedTask(
                    new RequestedTextExtractionTaskBuilder()
                    .WithManualCheckFallback()
                    .Build()
                    )
                    .WithNotifications(//https://developers.yoti.com/identity-verification/notifications
                    new NotificationConfigBuilder()
                    .WithEndpoint(webhookendpoint)
                    .ForResourceUpdate()
                    .ForTaskCompletion()
                    .ForCheckCompletion()
                    .ForSessionCompletion()
                    .Build()
                    )
                    .WithSdkConfig(
                    new SdkConfigBuilder()
                    .WithAllowsCameraAndUpload()
                    .WithPrimaryColour("#2d9fff")
                    .WithPresetIssuingCountry("GBR")
                    .WithSuccessUrl(return_success_url)
                    .WithErrorUrl(return_failure_url)
                    .WithAllowHandoff(true)
                    .Build()
                    )
                    .Build();

                    CreateSessionResult createSessionResult = docScanClient.CreateSession(sessionSpec);
                    Console.WriteLine("SessionResult: SessionId={0}", createSessionResult.SessionId);
                    string function_name = "Yoti_SessionId";

                    try
                    {

                        MySqlCommand _cmd = new MySqlCommand("SaveAPIRequestResponce");
                        _cmd.CommandType = CommandType.StoredProcedure;
                        _cmd.Parameters.AddWithValue("_API_ID", API_ID);
                        _cmd.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                        _cmd.Parameters.AddWithValue("_Customer_ID", Customer_ID);
                        _cmd.Parameters.AddWithValue("_status", 0);
                        _cmd.Parameters.AddWithValue("_Function_name", function_name);
                        _cmd.Parameters.AddWithValue("_Remark", 0);
                        _cmd.Parameters.AddWithValue("_comments", createSessionResult.SessionId);
                        _cmd.Parameters.AddWithValue("_Record_Insert_DateTime", obj.Record_Insert_DateTime);
                        _cmd.Parameters.AddWithValue("_Branch_ID", obj.Branch_ID);
                        int msg1 = db_connection.ExecuteNonQueryProcedure(_cmd);
                    }
                    catch (Exception ex)
                    {
                        string error = ex.ToString().Replace("\'", "\\'");
                        MySqlCommand _cmd = new MySqlCommand("SaveException");
                        _cmd.CommandType = CommandType.StoredProcedure;
                        _cmd.Parameters.AddWithValue("_Record_Insert_DateTime", obj.Record_Insert_DateTime);
                        _cmd.Parameters.AddWithValue("_error", error);
                        _cmd.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                        int msg1 = db_connection.ExecuteNonQueryProcedure(_cmd);
                    }

                    string redirectUrl = "";
                    redirectUrl = yotiurl + createSessionResult.SessionId + "&sessionToken=" + createSessionResult.ClientSessionToken;

                    dt.Rows.Add(1, redirectUrl);
                }
                if (dt.Rows.Count <= 0)
                {
                    dt.Rows.Add(1, "");
                }
                return dt;
            }
            catch (Exception ex)
            {
                if (dt.Rows.Count <= 0)
                {
                    dt.Rows.Add(2, ex.ToString());
                }
                return dt;
            }
        }


        public DataTable CheckIDScanPerm(Model.Document obj, HttpContext context)
        {
            int Customer_ID = Convert.ToInt32(CompanyInfo.Decrypt(obj.Customer_ID, true));
            DataTable dt = new DataTable();
            dt.Columns.Add("IDScanPerm", typeof(int));
            dt.Columns.Add("IDScanStatus", typeof(int));
            dt.Columns.Add("IDScanLimit", typeof(int));//0 valid and 1 exceeded
            dt.Columns.Add("MultipleIDScan", typeof(int));
            string urll = "", UserName = "", Password = "";
            int API_ID = 0;
            string externaluser_id = "";
            string applicant_id = "";
            string Applicat_ID = "";
            var externalUserId = "";
            string reviewAnswer = "";
            string reviewStatus = "";
            string Client_ID_regex = validation.validate(Convert.ToString(obj.Client_ID), 0, 1, 1, 1, 1, 1, 1, 1, 1);
            string Customer_ID_regex = validation.validate(Convert.ToString(Customer_ID), 0, 1, 1, 1, 1, 1, 1, 1, 1);
            if (Client_ID_regex != "false" && Customer_ID_regex != "false")
            {
                DataTable li1 = (DataTable)CompanyInfo.getEmailPermission(obj.Client_ID, 54);//Check GBG ID Scan permission
                int perm = 1, IDScanLimit = 0, AllowIDScan = 1;
                if (li1.Rows.Count > 0)
                {
                    perm = Convert.ToInt32(li1.Rows[0]["Status_ForCustomer"]);
                }

                obj.Record_Insert_DateTime = CompanyInfo.gettime(obj.Client_ID, Customer_ID.ToString(), obj.Country_ID, context);
                MySqlCommand cmdc = new MySqlCommand("UpdateIDScanCount");
                cmdc.CommandType = CommandType.StoredProcedure;
                cmdc.Parameters.AddWithValue("_Customer_ID", Customer_ID);//Customer ID
                cmdc.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                cmdc.Parameters.AddWithValue("_Queryflag", 2);//Get ID Scan Result
                cmdc.Parameters.AddWithValue("_nowdate", obj.Record_Insert_DateTime);
                string result = Convert.ToString(db_connection.ExecuteScalarProcedure(cmdc));
                if (result != null && result != "")
                {
                    if (result == "2")
                    {
                        perm = 1; IDScanLimit = 1;
                    }
                }
                int chkexpiry = 0, idupload = 0;
                MySqlCommand cmdp_active = new MySqlCommand("active_thirdparti_api");
                cmdp_active.CommandType = CommandType.StoredProcedure;
                DataTable dtApi_active = db_connection.ExecuteQueryDataTableProcedure(cmdp_active);
                if (dtApi_active.Rows.Count > 0)
                {
                    API_ID = Convert.ToInt32(dtApi_active.Rows[0]["API_ID"]);
                }

                MySqlCommand cmd = new MySqlCommand("GetAPIDetails");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("_API_ID", API_ID);//GBG API ID or Shuftipro
                cmd.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                cmd.Parameters.AddWithValue("_status", 0);// API Status
                DataTable dtApi = db_connection.ExecuteQueryDataTableProcedure(cmd);
                if (dtApi.Rows.Count > 0)
                {
                    urll = Convert.ToString(dtApi.Rows[0]["API_URL"]);
                    UserName = Convert.ToString(dtApi.Rows[0]["UserName"]);
                    Password = Convert.ToString(dtApi.Rows[0]["Password"]);
                }

                if (API_ID == 9)
                {

                    cmd = new MySqlCommand("Check_applicant_exist");
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("_customer_id", Customer_ID);// API Status
                    dtApi = db_connection.ExecuteQueryDataTableProcedure(cmd);

                    foreach (DataRow row in dtApi.Rows)
                    {
                        Applicat_ID = Convert.ToString(row["Parameter"]);
                        string[] parts = Applicat_ID.Split('=');

                        // Assuming there are always two parts separated by '='
                        if (parts.Length == 2)
                        {
                            Applicat_ID = parts[0]; // 65e80e1c62c95a48a68f6a5c
                            externaluser_id = parts[1]; // MX01237486e8f085f7
                        }
                    }

                    if (dtApi.Rows.Count > 0)
                    {

                        var stamp = ((long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds).ToString();
                        var userId = externaluser_id;
                        applicant_id = Applicat_ID;
                        // Calculate the value to sign (excluding the body)
                        var valueToSign = stamp + "GET" + $"/resources/applicants/{applicant_id}/one";


                        var appToken = UserName;
                        // Replace placeholders with actual values
                        valueToSign = valueToSign.Replace("{{sumsub_root_url}}", "")
                                                 .Replace("{{applicantId}}", "")
                                                 .Replace("{{levelName}}", "basic-kyc-level")
                                                 .Replace("{{userId}}", userId);

                        // Calculate the signature
                        var secretKey = Password; // Replace with your secret key
                        var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey));
                        var signatureBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(valueToSign));
                        var signature = BitConverter.ToString(signatureBytes).Replace("-", "").ToLower();

                        // Set your dynamic userId here

                        var client1 = new RestClient(urll);
                        var request1 = new RestRequest($"/resources/applicants/{applicant_id}/one", Method.GET);
                        request1.AddHeader("Content-Type", "application/json");
                        request1.AddHeader("X-App-Token", appToken);
                        request1.AddHeader("X-App-Access-Ts", stamp);
                        request1.AddHeader("X-App-Access-Sig", signature);
                        var responseT = client1.Execute(request1);
                        JObject jsonResponse = JObject.Parse(responseT.Content);


                        // CompanyInfo.InsertActivityLogDetails("Get The Sumsub Docs Details and applicantID 6 for Id scan " + responseT.Content + applicant_id + ". ", Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(Customer_ID), "GetJourney", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "ID Upload");

                        //activity_log = "Get The Sumsub Docs Details and applicantID 6 for Id scan " + responseT.Content + applicant_id + "";

                        // Accessing values from the response
                        try
                        {
                            string id = (string)jsonResponse["id"];
                            DateTime createdAt = (DateTime)jsonResponse["createdAt"];
                            string key = (string)jsonResponse["key"];
                            string clientId = (string)jsonResponse["clientId"];

                            externalUserId = (string)jsonResponse["externalUserId"];

                            JObject review = (JObject)jsonResponse["review"];

                            reviewStatus = (string)review["reviewStatus"];

                            // Accessing nested objects
                            JObject reviewResult = (JObject)review["reviewResult"];
                            reviewAnswer = (string)reviewResult["reviewAnswer"];
                        }
                        catch (Exception ex)
                        {

                        }


                        if (reviewAnswer == "GREEN" && reviewStatus == "completed")
                        {

                            //dt.Rows.Add(2, "" + appurl + "?id=" + HttpUtility.UrlEncode(CompanyInfo.Encrypt(Convert.ToString(Customer_ID), true))

                            //   + "&key=" + Token + "&defaultinput=" + HttpUtility.UrlEncode(Applicat_ID));
                            perm = 1; IDScanLimit = 1;

                        }
                        //else
                        //{

                        //    dt.Rows.Add(1, "" + appurl + "?id=" + HttpUtility.UrlEncode(CompanyInfo.Encrypt(Convert.ToString(Customer_ID), true))

                        //         + "&key=" + Token + "&defaultinput=" + HttpUtility.UrlEncode(Applicat_ID));

                        //}
                    }

                }

                if (perm == 0)
                {
                    li1 = null;
                    li1 = (DataTable)CompanyInfo.getEmailPermission(obj.Client_ID, 77);// Allow 3 times ID scan even if ID is valid/invalid
                    if (li1.Rows.Count > 0)
                    {
                        AllowIDScan = Convert.ToInt32(li1.Rows[0]["Status_ForCustomer"]);
                    }
                    //For Primary
                    MySqlCommand _cmd = new MySqlCommand("CheckIDExpiry");
                    _cmd.CommandType = CommandType.StoredProcedure;
                    _cmd.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                    _cmd.Parameters.AddWithValue("_IDType_ID", 1);
                    _cmd.Parameters.AddWithValue("_Customer_ID", Customer_ID);
                    DataTable table = db_connection.ExecuteQueryDataTableProcedure(_cmd);

                    DateTime dt1 = DateTime.Now.Date;
                    DateTime dt3 = DateTime.Now.Date.AddDays(60);
                    if (table.Rows.Count > 0)// Check 1
                    {
                        for (int i = 0; i < table.Rows.Count; i++)
                        {
                            string senPExpDate = Convert.ToString(table.Rows[i]["SenderID_ExpiryDate"]);
                            if (senPExpDate != "" && senPExpDate != null && senPExpDate != "VGrYRT2Em7s=")
                            {
                                DateTime dt2 = DateTime.ParseExact(Convert.ToDateTime(senPExpDate).ToString("dd/MM/yyyy"), "dd/MM/yyyy", null);
                                if (dt1 > dt2)
                                {
                                    //"Identification document is expired and need to be uploaded to proceed this transfer. Do you want to Upload?";
                                    chkexpiry = 1;
                                }
                            }
                        }
                    }
                    else
                    {
                        idupload = 1;
                    }
                    if (AllowIDScan == 0)
                    {
                        chkexpiry = 0;
                    }
                }
                if (perm == 0 && idupload == 1 || perm == 0 && chkexpiry == 1)
                {
                    dt.Rows.Add(perm, 0, IDScanLimit, AllowIDScan);
                }
                else
                {
                    dt.Rows.Add(perm, 1, IDScanLimit, AllowIDScan);
                }
                if (IDScanLimit == 1)
                {

                }
            }
            else
            {
                string msg = "Validation Error Client_ID_regex-" + Client_ID_regex + "Customer_ID_regex- " + Customer_ID_regex;
                int stattus = (int)CompanyInfo.InsertActivityLogDetailsSecurity(msg, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), 1, "srvDocument", 2, Convert.ToInt32(obj.Client_ID), "CheckIDScanPerm", 0, context);
            }
            return dt;
        }

        public DataTable CheckIDScanPerm_old(Model.Document obj, HttpContext context)
        {
            int Customer_ID = Convert.ToInt32(CompanyInfo.Decrypt(obj.Customer_ID, true));
            DataTable dt = new DataTable();
            dt.Columns.Add("IDScanPerm", typeof(int));
            dt.Columns.Add("IDScanStatus", typeof(int));
            dt.Columns.Add("IDScanLimit", typeof(int));//0 valid and 1 exceeded
            dt.Columns.Add("MultipleIDScan", typeof(int));
            string Client_ID_regex = validation.validate(Convert.ToString(obj.Client_ID), 0, 1, 1, 1, 1, 1, 1, 1, 1);
            string Customer_ID_regex = validation.validate(Convert.ToString(Customer_ID), 0, 1, 1, 1, 1, 1, 1, 1, 1);
            if (Client_ID_regex != "false" && Customer_ID_regex != "false")
            {
                DataTable li1 = (DataTable)CompanyInfo.getEmailPermission(obj.Client_ID, 54);//Check GBG ID Scan permission
                int perm = 1, IDScanLimit = 0, AllowIDScan = 1;
                if (li1.Rows.Count > 0)
                {
                    perm = Convert.ToInt32(li1.Rows[0]["Status_ForCustomer"]);
                }

                obj.Record_Insert_DateTime = CompanyInfo.gettime(obj.Client_ID, Customer_ID.ToString(), obj.Country_ID, context);
                MySqlConnector.MySqlCommand cmdc = new MySqlConnector.MySqlCommand("UpdateIDScanCount");
                cmdc.CommandType = CommandType.StoredProcedure;
                cmdc.Parameters.AddWithValue("_Customer_ID", Customer_ID);//Customer ID
                cmdc.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                cmdc.Parameters.AddWithValue("_Queryflag", 2);//Get ID Scan Result
                cmdc.Parameters.AddWithValue("_nowdate", obj.Record_Insert_DateTime);
                string result = Convert.ToString(db_connection.ExecuteScalarProcedure(cmdc));
                if (result != null && result != "")
                {
                    if (result == "2")
                    {
                        perm = 1; IDScanLimit = 1;
                    }
                }
                int chkexpiry = 0, idupload = 0;
                if (perm == 0)
                {
                    li1 = null;
                    li1 = (DataTable)CompanyInfo.getEmailPermission(obj.Client_ID, 77);// Allow 3 times ID scan even if ID is valid/invalid
                    if (li1.Rows.Count > 0)
                    {
                        AllowIDScan = Convert.ToInt32(li1.Rows[0]["Status_ForCustomer"]);
                    }
                    //For Primary
                    MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("CheckIDExpiry");
                    _cmd.CommandType = CommandType.StoredProcedure;
                    _cmd.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                    _cmd.Parameters.AddWithValue("_IDType_ID", 1);
                    _cmd.Parameters.AddWithValue("_Customer_ID", Customer_ID);
                    DataTable table = db_connection.ExecuteQueryDataTableProcedure(_cmd);

                    DateTime dt1 = DateTime.Now.Date;
                    DateTime dt3 = DateTime.Now.Date.AddDays(60);
                    if (table.Rows.Count > 0)// Check 1
                    {
                        for (int i = 0; i < table.Rows.Count; i++)
                        {
                            string senPExpDate = Convert.ToString(table.Rows[i]["SenderID_ExpiryDate"]);
                            if (senPExpDate != "" && senPExpDate != null && senPExpDate != "VGrYRT2Em7s=")
                            {
                                DateTime dt2 = DateTime.ParseExact(Convert.ToDateTime(senPExpDate).ToString("dd/MM/yyyy"), "dd/MM/yyyy", null);
                                if (dt1 > dt2)
                                {
                                    //"Identification document is expired and need to be uploaded to proceed this transfer. Do you want to Upload?";
                                    chkexpiry = 1;
                                }
                            }
                        }
                    }
                    else
                    {
                        idupload = 1;
                    }
                    if (AllowIDScan == 0)
                    {
                        chkexpiry = 0;
                    }
                }
                if (perm == 0 && idupload == 1 || perm == 0 && chkexpiry == 1)
                {
                    dt.Rows.Add(perm, 0, IDScanLimit, AllowIDScan);
                }
                else
                {
                    dt.Rows.Add(perm, 1, IDScanLimit, AllowIDScan);
                }
                if (IDScanLimit == 1)
                {
                    //obj.Record_Insert_DateTime = DateTime.Now.ToString();
                    //string notification_icon = "sanction-list-match-found.jpg";
                    //string notification_message = "<span class='cls-admin'>Daily ID scan limit<strong class='cls-cancel'> exceeded.</strong></span><span class='cls-customer'></span>";
                    //CompanyInfo.save_notification_compliance(notification_message, notification_icon, Convert.ToInt32(obj.Customer_ID), Convert.ToDateTime(obj.Record_Insert_DateTime), Convert.ToInt32(obj.Client_ID), 1, Convert.ToInt32(obj.User_ID), Convert.ToInt32(obj.Branch_ID), 0, 1, 1, 0);
                }
            }
            else
            {
                string msg = "Validation Error Client_ID_regex-" + Client_ID_regex + "Customer_ID_regex- " + Customer_ID_regex;
                int stattus = (int)CompanyInfo.InsertActivityLogDetailsSecurity(msg, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), 1, "srvDocument", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "CheckIDScanPerm", 0, context);
            }
            return dt;
        }
        //_bef_allowing_invalidvalues







        public DataTable GetJourney(Model.Document obj, HttpContext context)
        {
            int Status = 1, Customer_ID = 0;
            int API_ID = 3;//GBG API ID
            string Token_sumsub = "";
            string reviewAnswer = "";
            string reviewStatus = "";
            var inspectionId = "";
            string requestapi = "";
            string responseapi = "";
            string resresult = "";
            int Aml_check = 0;
            string activity_log = "";
            string back_idpath = ""; string idDocMrzLine1 = "";
            string idDocMrzLine2 = "";
            string placeOfBirth = "";
            dynamic dynJson1 = "";
            string BackSideWhiteImageUrl = "";
            int fitnessProbityFlag = 1;
            int pepFlag = 1;
            int sanctionFlag = 1;
            int warningFlag = 1;

            string Yoti_SessionId = "";
            string idStr = "", exStr = "", statusStr = "", isCanceledStr = "", isCompletedStr = "", isCompletedSuccessfullyStr = "";
            string creationOptionsStr = "", asyncStateStr = "", isFaultedStr = "";
            string sessionIdStr = "", userTrackingIdStr = "", createdAtStr = "", overallStatusStr = "";
            string documentStatusStr = "", documentRecommendationStr = "", documentBreakdownsStr = "", documentTypeStr = "";
            string issuingCountryStr = "", documentIdStr = "", documentPhotoStr = "", documentFullNameStr = "";
            string documentGivenNamesStr = "", documentFirstNameStr = "", documentMiddleNameStr = "", documentFamilyNameStr = "";
            string documentNamePrefixStr = "", documentDateOfBirthStr = "", documentNationalityStr = "", documentPlaceOfBirthStr = "";
            string documentCountryOfBirthStr = "", documentGenderStr = "", documentNumberStr = "", documentExpirationDateStr = "";
            string documentDateOfIssueStr = "", documentIssuingAuthorityStr = "";
            string livenessStatusStr = "", livenessRecommendationStr = "", livenessBreakdownsStr = "";
            string faceMatchStatusStr = "", faceMatchRecommendationStr = "", faceMatchConfidenceScoreStr = "", faceMatchBreakdownsStr = "";
            string watchlistStatusStr = "", watchlistRecommendationStr = "", watchlistBreakdownsStr = "";
            string textDataStr = "", Req = ""; var journeyID = new object();
            //obj.Custmer_Ref= "MT00123654";
            //obj.applicant_id= "65d71d4e08ca10763627b71f";
            DataTable dt = new DataTable();
            dt.Columns.Add("Status", typeof(int));
            dt.Columns.Add("ResponseMessage", typeof(string)); dt.Columns.Add("ResultID", typeof(int));
            string cname = "", mname = "", lname = "";
            //var context = HttpContext.Current;
            try
            {
                Customer_ID = Convert.ToInt32(CompanyInfo.Decrypt(obj.UserName, true));//New
                MySqlCommand cmdp_active = new MySqlCommand("active_thirdparti_api");
                cmdp_active.CommandType = CommandType.StoredProcedure;
                DataTable dtApi_active = db_connection.ExecuteQueryDataTableProcedure(cmdp_active);
                if (dtApi_active.Rows.Count > 0)
                {
                    API_ID = Convert.ToInt32(dtApi_active.Rows[0]["API_ID"]);
                    Aml_check = Convert.ToInt32(dtApi_active.Rows[0]["Aml_check"]);
                    activity_log = "Get The API ID for Id scan 1" + API_ID + "";
                }
                string Shufti_refId = obj.ShuftiId;
                if (API_ID == 9)
                {
                    obj.UserName = obj.Customer_ID;
                }

                if (API_ID == 14)
                {
                    MySqlCommand SessionId = new MySqlCommand("Get_SessionId");
                    SessionId.CommandType = CommandType.StoredProcedure;
                    SessionId.Parameters.AddWithValue("_Customer_ID", Customer_ID);
                    DataTable Get_SessionId = db_connection.ExecuteQueryDataTableProcedure(SessionId);
                    if (Get_SessionId.Rows.Count > 0)
                    {
                        Yoti_SessionId = Convert.ToString(Get_SessionId.Rows[0]["Parameter"]);
                        obj.JourneyID = Yoti_SessionId;
                    }
                }


                try
                {
                    string scanstatus = obj.scanstatus;
                }
                catch (Exception egx) { obj.scanstatus = ""; }


                obj.Customer_ID = Customer_ID.ToString();
                string urll = "", UserName = "", Password = "";// https://poc.idscan.cloud/idscanenterprisesvc/";// test
                //var urll = "https://prod.idscan.cloud/idscanenterprisesvc/";//live            


                // For Get API id
                /*MySqlCommand cmd_select = new MySqlCommand("select API_ID from thirdpartyapi_master where Active_ScanId = 0 and Delete_Status = 0 ");
                DataTable dtt = db_connection.ExecuteQueryDataTableProcedure(cmd_select);
                if (dtt.Rows.Count > 0)
                {
                    API_ID = Convert.ToInt32(dtt.Rows[0]["API_ID"]);
                }
                cmd_select.Dispose();*/


                obj.Record_Insert_DateTime = CompanyInfo.gettime(obj.Client_ID, Customer_ID.ToString(), obj.Country_ID, context);

                MySqlCommand cmdc = new MySqlCommand("UpdateIDScanCount");
                cmdc.CommandType = CommandType.StoredProcedure;
                cmdc.Parameters.AddWithValue("_Customer_ID", Customer_ID);//GBG API ID
                cmdc.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                cmdc.Parameters.AddWithValue("_Queryflag", 1);
                cmdc.Parameters.AddWithValue("_nowdate", obj.Record_Insert_DateTime);
                db_connection.ExecuteQueryDataTableProcedure(cmdc);

                #region notif
                cmdc = new MySqlCommand("UpdateIDScanCount");
                cmdc.CommandType = CommandType.StoredProcedure;
                cmdc.Parameters.AddWithValue("_Customer_ID", Customer_ID);//Customer ID
                cmdc.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                cmdc.Parameters.AddWithValue("_Queryflag", 2);//Get ID Scan Result
                cmdc.Parameters.AddWithValue("_nowdate", obj.Record_Insert_DateTime);
                string resultk = Convert.ToString(db_connection.ExecuteScalarProcedure(cmdc));
                if (resultk != null && resultk != "")
                {
                    if (resultk == "2")
                    {
                        string notification_icon = "sanction-list-match-found.jpg";
                        string notification_message = "<span class='cls-admin'>Daily ID scan limit<strong class='cls-cancel'> reached.</strong></span><span class='cls-customer'></span>";
                        CompanyInfo.save_notification_compliance(notification_message, notification_icon, Convert.ToString(Customer_ID), Convert.ToDateTime(obj.Record_Insert_DateTime), Convert.ToInt32(obj.Client_ID), 1, Convert.ToInt32(obj.User_ID), Convert.ToInt32(obj.Branch_ID), 0, 1, 1, 0, context);
                    }
                }
                #endregion


                MySqlCommand cmdp = new MySqlCommand("GetAPIDetails");
                cmdp.CommandType = CommandType.StoredProcedure;
                cmdp.Parameters.AddWithValue("_API_ID", API_ID);//GBG API ID
                cmdp.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                cmdp.Parameters.AddWithValue("_status", 0);// API Status
                DataTable dtApi = db_connection.ExecuteQueryDataTableProcedure(cmdp);
                if (dtApi.Rows.Count > 0)
                {
                    urll = Convert.ToString(dtApi.Rows[0]["API_URL"]);
                    UserName = Convert.ToString(dtApi.Rows[0]["UserName"]);
                    Password = Convert.ToString(dtApi.Rows[0]["Password"]);
                }


                //get company details
                DataTable dtc = CompanyInfo.get(obj.Client_ID, context);

                activity_log = "Get The Company Details 3 for Id scan " + dtc + "";
                string base64String = string.Empty;
                string s13 = string.Empty;
                if (API_ID == 3)
                {
                    //Get Token              
                    WebResponse response2 = Token(urll, UserName, Password, context);
                    using (var reader = new StreamReader(response2.GetResponseStream()))
                    {
                        string ApiStatus = reader.ReadToEnd();
                        var entries = ApiStatus.TrimStart('{').TrimEnd('}').Replace("\"", String.Empty).Split(',');
                        foreach (var entry in entries)
                        {
                            if (entry.Split(':')[0] == "access_token")
                            {
                                s13 = entry.Split(':')[1]; break;
                            }
                        }
                    }
                }
                //Get Token Sumsub
                if (API_ID == 9)
                {
                    //Get externaluserId            


                    string createdAtDate = "";

                    var stamp = ((long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds).ToString();
                    // string userId = "MX0123748645d71714"; //obj.Custmer_Ref;
                    var applicant_id = obj.applicant_id;
                    // Calculate the value to sign (excluding the body)
                    var valueToSign = stamp + "GET" + $"/resources/applicants/{applicant_id}/one";


                    string appToken = UserName;
                    // Replace placeholders with actual values
                    valueToSign = valueToSign.Replace("{{sumsub_root_url}}", "")
                                             .Replace("{{applicantId}}", "")
                                             .Replace("{{levelName}}", "basic-kyc-level");


                    // Calculate the signature
                    var secretKey = Password; // Replace with your secret key
                    var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey));
                    var signatureBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(valueToSign));
                    var signature = BitConverter.ToString(signatureBytes).Replace("-", "").ToLower();

                    CompanyInfo.InsertrequestLogTracker("GetJourney_newupdates: " + urll + "  applicant_id: " + applicant_id, 0, 0, 0, 0, "GetJourney_newupdates", Convert.ToInt32(0), Convert.ToInt32(0), "", context);


                    // Set your dynamic userId here
                    var client1 = new RestClient(urll);
                    var request1 = new RestRequest($"/resources/applicants/{applicant_id}/one", Method.GET);
                    request1.AddHeader("Content-Type", "application/json");
                    request1.AddHeader("X-App-Token", appToken);
                    request1.AddHeader("X-App-Access-Ts", stamp);
                    request1.AddHeader("X-App-Access-Sig", signature);
                    IRestResponse responseT = client1.Execute(request1);
                    JObject jsonResponse = JObject.Parse(responseT.Content);

                    //CompanyInfo.InsertActivityLogDetails("Get TheGet externaluserId 4 for Id scan " + responseT.Content + ". ", Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(Customer_ID), "GetJourney", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "ID Upload");

                    activity_log = "Get TheGet externaluserId 4 for Id scan " + responseT.Content + "";
                    // Accessing values from the response
                    string id = (string)jsonResponse["id"];
                    DateTime createdAt = (DateTime)jsonResponse["createdAt"];
                    string key = (string)jsonResponse["key"];
                    string clientId = (string)jsonResponse["clientId"];
                    inspectionId = (string)jsonResponse["inspectionId"];
                    string externalUserId = (string)jsonResponse["externalUserId"];
                    try
                    {
                        placeOfBirth = (string)jsonResponse["info"]["placeOfBirth"];
                        if (placeOfBirth == "")
                        {
                            placeOfBirth = (string)jsonResponse["info"]["placeOfBirthEn"];
                        }


                    }
                    catch { }

                    //DateTime dob = (DateTime)jsonResponse["info"]["dob"];
                    //string country = (string)jsonResponse["info"]["country"];
                    // Accessing nested arrays
                    try
                    {
                        JObject agreement = (JObject)jsonResponse["agreement"];

                        // Extract and print values from the "agreement" object
                        string createdAt1 = agreement["createdAt"].ToString();

                        DateTime createdAtDateTime = DateTime.ParseExact(createdAt1, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                        createdAtDate = createdAtDateTime.ToString("yyyy-MM-dd");
                        string acceptedAt = agreement["acceptedAt"].ToString();
                    }
                    catch { }

                    JArray idDocs5 = new JArray();
                    // Iterate through each idDoc in idDocs array
                    try
                    {
                        idDocs5 = (JArray)jsonResponse["info"]["idDocs"];
                    }
                    catch { }
                    // Accessing nested objects




                    obj.Custmer_Ref = externalUserId;


                    string sumsub_applicant_id = applicant_id + "=" + externalUserId;
                    string function_name = "sumsub_applicant_id";

                    string Applicat_ID = "";

                    string inumber = "";



                    // check applicant is saved or not

                    try
                    {
                        if (idDocs5.Count > 0)
                        {
                            JObject idDoc = (JObject)idDocs5[0];


                            if (idDoc["number"] != null && !string.IsNullOrWhiteSpace(idDoc["number"].ToString()))
                            {
                                inumber = idDoc["number"].ToString().Replace(" ", string.Empty);
                            }


                        }
                    }
                    catch (Exception ex) { }

                    try
                    {
                        MySqlCommand cmddd1 = new MySqlCommand("Check_Sumsub_id_exist");
                        cmddd1.CommandType = CommandType.StoredProcedure;
                        cmddd1.Parameters.AddWithValue("_customer_id", Customer_ID);
                        cmddd1.Parameters.AddWithValue("_SenderID_Number", inumber);
                        cmddd1.Parameters.AddWithValue("_Inserted_Date_Time", createdAtDate);
                        dtApi = db_connection.ExecuteQueryDataTableProcedure(cmddd1);


                        if (dtApi.Rows.Count > 0)
                        {
                            dt.Rows.Add(0, "Alleredy Saved", obj.SenderID_ID);
                            return dt;
                        }

                    }
                    catch { }


                    MySqlCommand cmddd = new MySqlCommand("Check_applicant_exist");
                    cmddd.CommandType = CommandType.StoredProcedure;
                    cmddd.Parameters.AddWithValue("_customer_id", Customer_ID);// API Status
                    dtApi = db_connection.ExecuteQueryDataTableProcedure(cmddd);

                    foreach (DataRow row in dtApi.Rows)
                    {
                        Applicat_ID = Convert.ToString(row["Parameter"]);
                    }

                    if (Applicat_ID == "")
                    {

                        try
                        {

                            MySqlCommand _cmd = new MySqlCommand("SaveAPIRequestResponce");
                            _cmd.CommandType = CommandType.StoredProcedure;
                            _cmd.Parameters.AddWithValue("_API_ID", API_ID);
                            _cmd.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                            _cmd.Parameters.AddWithValue("_Customer_ID", Customer_ID);
                            _cmd.Parameters.AddWithValue("_status", 0);
                            _cmd.Parameters.AddWithValue("_Function_name", function_name);
                            _cmd.Parameters.AddWithValue("_Remark", 0);
                            _cmd.Parameters.AddWithValue("_comments", sumsub_applicant_id);
                            _cmd.Parameters.AddWithValue("_Record_Insert_DateTime", obj.Record_Insert_DateTime);
                            _cmd.Parameters.AddWithValue("_Branch_ID", obj.Branch_ID);
                            int msg1 = db_connection.ExecuteNonQueryProcedure(_cmd);
                        }
                        catch (Exception ex)
                        {
                            string error = ex.ToString().Replace("\'", "\\'");
                            MySqlCommand _cmd = new MySqlCommand("SaveException");
                            _cmd.CommandType = CommandType.StoredProcedure;
                            _cmd.Parameters.AddWithValue("_Record_Insert_DateTime", obj.Record_Insert_DateTime);
                            _cmd.Parameters.AddWithValue("_error", error);
                            _cmd.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                            int msg1 = db_connection.ExecuteNonQueryProcedure(_cmd);
                        }
                    }
                }



                //Get yoti all details using session id.

                if (API_ID == 14)
                {

                    var session = GetVerificationSummary(Yoti_SessionId, API_ID, obj.Client_ID);
                    Req = "GetVerificationSummary Yoti_SessionId :" + Yoti_SessionId + "API_ID :" + API_ID + "Client_ID :" + obj.Client_ID;
                    string jsonResponse = JsonConvert.SerializeObject(session, Newtonsoft.Json.Formatting.Indented);
                    dynJson1 = Newtonsoft.Json.JsonConvert.DeserializeObject(jsonResponse);
                    try
                    {
                        MySqlCommand _cmd = new MySqlCommand("SaveAPIRequestResponce");
                        _cmd.CommandType = CommandType.StoredProcedure;
                        _cmd.Parameters.AddWithValue("_API_ID", API_ID);
                        _cmd.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                        _cmd.Parameters.AddWithValue("_Customer_ID", Customer_ID);
                        _cmd.Parameters.AddWithValue("_status", 0);
                        _cmd.Parameters.AddWithValue("_Function_name", "Yoti_response");
                        _cmd.Parameters.AddWithValue("_Remark", 0);
                        _cmd.Parameters.AddWithValue("_comments", jsonResponse);
                        _cmd.Parameters.AddWithValue("_Record_Insert_DateTime", obj.Record_Insert_DateTime);
                        _cmd.Parameters.AddWithValue("_Branch_ID", obj.Branch_ID);
                        int msg1 = db_connection.ExecuteNonQueryProcedure(_cmd);
                    }
                    catch (Exception ex)
                    {
                        string error = ex.ToString().Replace("\'", "\\'");
                        MySqlCommand _cmd = new MySqlCommand("SaveException");
                        _cmd.CommandType = CommandType.StoredProcedure;
                        _cmd.Parameters.AddWithValue("_Record_Insert_DateTime", obj.Record_Insert_DateTime);
                        _cmd.Parameters.AddWithValue("_error", error);
                        _cmd.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                        int msg1 = db_connection.ExecuteNonQueryProcedure(_cmd);
                    }

                    // === Root Level ===
                    // Declare all string variables globally
                    try { var overallStatus = session.OverallStatus; if (!string.IsNullOrEmpty(Convert.ToString(overallStatus))) overallStatusStr = overallStatus.ToString(); } catch { }

                    if (overallStatusStr == "ONGOING" || overallStatusStr != "COMPLETED")
                    {
                        MySqlCommand _cmd = new MySqlCommand("Insert_thirdparty_journeystatus_table");
                        _cmd.CommandType = CommandType.StoredProcedure;
                        _cmd.Parameters.AddWithValue("_journeyuniqueid", Yoti_SessionId);
                        _cmd.Parameters.AddWithValue("_customerid", Customer_ID);
                        _cmd.Parameters.AddWithValue("_uniqueid", "Yoti_response");
                        _cmd.Parameters.AddWithValue("_journeystatus", overallStatusStr);
                        _cmd.Parameters.AddWithValue("_journeystatusfinished", 1);
                        _cmd.Parameters.AddWithValue("_insertat", obj.Record_Insert_DateTime);
                        _cmd.Parameters.AddWithValue("_api_id", API_ID);
                        int msg1 = db_connection.ExecuteNonQueryProcedure(_cmd);

                        dt.Rows.Add(3, "Your ID Scan Journey Is Completed and we are verifying it.", obj.SenderID_ID);
                        return dt;
                    }

                    try { var id = dynJson1.Id; if (id != null) idStr = id.ToString(); } catch { }
                    try { var exception = dynJson1.Exception; if (exception != null) exStr = exception.ToString(); } catch { }
                    try { var status = dynJson1.Status; if (status != null) statusStr = status.ToString(); } catch { }
                    try { var isCanceled = dynJson1.IsCanceled; isCanceledStr = isCanceled.ToString(); } catch { }
                    try { var isCompleted = dynJson1.IsCompleted; isCompletedStr = isCompleted.ToString(); } catch { }
                    try { var isCompletedSuccessfully = dynJson1.IsCompletedSuccessfully; isCompletedSuccessfullyStr = isCompletedSuccessfully.ToString(); } catch { }
                    try { var creationOptions = dynJson1.CreationOptions; if (creationOptions != null) creationOptionsStr = creationOptions.ToString(); } catch { }
                    try { var asyncState = dynJson1.AsyncState; if (asyncState != null) asyncStateStr = asyncState.ToString(); } catch { }
                    try { var isFaulted = dynJson1.IsFaulted; isFaultedStr = isFaulted.ToString(); } catch { }

                    // === Result Level ===
                    try { var sessionId = session.SessionId; if (!string.IsNullOrEmpty(Convert.ToString(sessionId))) sessionIdStr = sessionId.ToString(); journeyID = sessionIdStr; } catch { }
                    try { var userTrackingId = session.UserTrackingId; if (!string.IsNullOrEmpty(Convert.ToString(userTrackingId))) userTrackingIdStr = userTrackingId.ToString(); } catch { }
                    try { var createdAt = session.CreatedAt; if (!string.IsNullOrEmpty(Convert.ToString(createdAt))) createdAtStr = createdAt.ToString(); } catch { }
                    try { var overallStatus = session.OverallStatus; if (!string.IsNullOrEmpty(Convert.ToString(overallStatus))) overallStatusStr = overallStatus.ToString(); } catch { }

                    // === Document Level ===
                    try { var documentStatus = session.Document.Status; if (!string.IsNullOrEmpty(Convert.ToString(documentStatus))) documentStatusStr = documentStatus.ToString(); } catch { }
                    try { var documentRecommendation = session.Document.Recommendation; if (!string.IsNullOrEmpty(Convert.ToString(documentRecommendation))) documentRecommendationStr = documentRecommendation.ToString(); } catch { }
                    try { var documentBreakdowns = session.Document.Breakdowns; if (documentBreakdowns != null) documentBreakdownsStr = documentBreakdowns.ToString(); } catch { }
                    try { var documentType = session.Document.DocumentType; if (!string.IsNullOrEmpty(Convert.ToString(documentType))) documentTypeStr = documentType.ToString(); obj.DocumentName = documentTypeStr; } catch { }
                    try { var issuingCountry = session.Document.IssuingCountry; if (!string.IsNullOrEmpty(Convert.ToString(issuingCountry))) issuingCountryStr = issuingCountry.ToString(); } catch { }
                    try { var documentId = session.Document.DocumentId; if (!string.IsNullOrEmpty(Convert.ToString(documentId))) documentIdStr = documentId.ToString(); } catch { }
                    try { var documentPhoto = session.Document.DocumentPhoto; if (!string.IsNullOrEmpty(Convert.ToString(documentPhoto))) documentPhotoStr = documentPhoto.ToString(); } catch { }
                    try { var documentFullName = session.Document.FullName; if (!string.IsNullOrEmpty(Convert.ToString(documentFullName))) documentFullNameStr = documentFullName.ToString(); obj.SenderNameOnID = documentFullNameStr; } catch { }
                    try { var documentGivenNames = session.Document.GivenNames; if (!string.IsNullOrEmpty(Convert.ToString(documentGivenNames))) documentGivenNamesStr = documentGivenNames.ToString(); } catch { }
                    try { var documentFirstName = session.Document.FirstName; if (!string.IsNullOrEmpty(Convert.ToString(documentFirstName))) documentFirstNameStr = documentFirstName.ToString(); } catch { }
                    try { var documentMiddleName = session.Document.MiddleName; if (!string.IsNullOrEmpty(Convert.ToString(documentMiddleName))) documentMiddleNameStr = documentMiddleName.ToString(); } catch { }
                    try { var documentFamilyName = session.Document.FamilyName; if (!string.IsNullOrEmpty(Convert.ToString(documentFamilyName))) documentFamilyNameStr = documentFamilyName.ToString(); } catch { }
                    try { var documentNamePrefix = session.Document.NamePrefix; if (!string.IsNullOrEmpty(Convert.ToString(documentNamePrefix))) documentNamePrefixStr = documentNamePrefix.ToString(); } catch { }
                    try { var documentDateOfBirth = session.Document.DateOfBirth; if (!string.IsNullOrEmpty(Convert.ToString(documentDateOfBirth))) documentDateOfBirthStr = documentDateOfBirth.ToString(); obj.Sender_DateOfBirth = documentDateOfBirthStr; } catch { }
                    try { var documentNationality = session.Document.Nationality; if (!string.IsNullOrEmpty(Convert.ToString(documentNationality))) documentNationalityStr = documentNationality.ToString(); } catch { }
                    try { var documentPlaceOfBirth = session.Document.PlaceOfBirth; if (!string.IsNullOrEmpty(Convert.ToString(documentPlaceOfBirth))) documentPlaceOfBirthStr = documentPlaceOfBirth.ToString(); } catch { }
                    try { var documentCountryOfBirth = session.Document.CountryOfBirth; if (!string.IsNullOrEmpty(Convert.ToString(documentCountryOfBirth))) documentCountryOfBirthStr = documentCountryOfBirth.ToString(); } catch { }
                    try { var documentGender = session.Document.Gender; if (!string.IsNullOrEmpty(Convert.ToString(documentGender))) documentGenderStr = documentGender.ToString(); } catch { }
                    try { var documentNumber = session.Document.DocumentNumber; if (!string.IsNullOrEmpty(Convert.ToString(documentNumber))) documentNumberStr = documentNumber.ToString(); obj.SenderID_Number = documentNumberStr; } catch { }
                    try { var documentExpirationDate = session.Document.ExpirationDate; if (!string.IsNullOrEmpty(Convert.ToString(documentExpirationDate))) documentExpirationDateStr = documentExpirationDate.ToString(); obj.SenderID_ExpiryDate = documentExpirationDateStr; } catch { }
                    try { var documentDateOfIssue = session.Document.DateOfIssue; if (!string.IsNullOrEmpty(Convert.ToString(documentDateOfIssue))) documentDateOfIssueStr = documentDateOfIssue.ToString(); obj.Issue_date = documentDateOfIssueStr; } catch { }
                    try { var documentIssuingAuthority = session.Document.IssuingAuthority; if (!string.IsNullOrEmpty(Convert.ToString(documentIssuingAuthority))) documentIssuingAuthorityStr = documentIssuingAuthority.ToString(); } catch { }
                    //try { var docmrznumber1 = session.Document.mrz.line1; if (!string.IsNullOrEmpty(Convert.ToString(docmrznumber1))) obj.MRZ_number = docmrznumber1.ToString(); } catch { }
                    //try { var docmrznumber2 = session.Document.mrz.line2; if (!string.IsNullOrEmpty(Convert.ToString(docmrznumber2))) idDocMrzLine2 = docmrznumber2.ToString(); } catch { }
                    // === Liveness Level ===
                    try { var livenessStatus = session.Liveness.Status; if (!string.IsNullOrEmpty(Convert.ToString(livenessStatus))) livenessStatusStr = livenessStatus.ToString(); } catch { }
                    try { var livenessRecommendation = session.Liveness.Recommendation; if (!string.IsNullOrEmpty(Convert.ToString(livenessRecommendation))) livenessRecommendationStr = livenessRecommendation.ToString(); } catch { }
                    try { var livenessBreakdowns = session.Liveness.Breakdowns; if (livenessBreakdowns != null) livenessBreakdownsStr = livenessBreakdowns.ToString(); } catch { }

                    // === FaceMatch Level ===
                    try { var faceMatchStatus = session.FaceMatch.Status; if (!string.IsNullOrEmpty(Convert.ToString(faceMatchStatus))) faceMatchStatusStr = faceMatchStatus.ToString(); } catch { }
                    try { var faceMatchRecommendation = session.FaceMatch.Recommendation; if (!string.IsNullOrEmpty(Convert.ToString(faceMatchRecommendation))) faceMatchRecommendationStr = faceMatchRecommendation.ToString(); } catch { }
                    try { var faceMatchConfidenceScore = session.FaceMatch.ConfidenceScore; if (!string.IsNullOrEmpty(Convert.ToString(faceMatchConfidenceScore))) faceMatchConfidenceScoreStr = faceMatchConfidenceScore.ToString(); } catch { }
                    try { var faceMatchBreakdowns = session.FaceMatch.Breakdowns; if (faceMatchBreakdowns != null) faceMatchBreakdownsStr = faceMatchBreakdowns.ToString(); } catch { }

                    // === Watchlist Level ===
                    try { var watchlistStatus = session.Watchlist.Status; if (!string.IsNullOrEmpty(Convert.ToString(watchlistStatus))) watchlistStatusStr = watchlistStatus.ToString(); } catch { }
                    try { var watchlistRecommendation = session.Watchlist.Recommendation; if (!string.IsNullOrEmpty(Convert.ToString(watchlistRecommendation))) watchlistRecommendationStr = watchlistRecommendation.ToString(); } catch { }
                    try { var watchlistBreakdowns = session.Watchlist.Breakdowns; if (watchlistBreakdowns != null) watchlistBreakdownsStr = watchlistBreakdowns.ToString(); } catch { }

                    // === TextData Level ===
                    try { var textData = session.TextData; if (textData != null) textDataStr = textData.ToString(); } catch { }

                    // === Document Pages Array ===



                    // === Document Extra Fields ===




                    // === FaceMatch Level ===
                    // === FaceMatch FaceCaptures Array ===
                    try
                    {
                        foreach (var faceCapture in dynJson1.Result.FaceMatch.FaceCaptures)
                        {
                            // if any properties exist under FaceCaptures, access here.
                        }
                    }
                    catch { }


                }

                // Get Journey
                string data = "journeyID=" + obj.JourneyID + "";
                //Check duplicate journey upload
                cmdc = new MySqlCommand("select count(ifnull(SenderID_ID,0)) as IDCount from documents_details where JourneyID=@JourneyID and Client_ID=@Client_ID and Customer_ID=@Customer_ID");
                cmdc.Parameters.AddWithValue("@Customer_ID", Customer_ID);//GBG API ID
                cmdc.Parameters.AddWithValue("@Client_ID", obj.Client_ID);
                cmdc.Parameters.AddWithValue("@JourneyID", obj.JourneyID);
                DataTable idc = db_connection.ExecuteQueryDataTableProcedure(cmdc);
                if (idc.Rows.Count > 0)
                {
                    int cnt = Convert.ToInt32(idc.Rows[0]["IDCount"]);
                    if (cnt > 0)
                    {
                        dt.Rows.Add(0, "Duplicate Journey", obj.SenderID_ID);
                        CompanyInfo.InsertActivityLogDetails("Duplicate call for journey " + obj.JourneyID + ". ", Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(Customer_ID), "GetJourney", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "ID Upload", context);
                        return dt;
                    }
                }

                byte[] dataStream = Encoding.UTF8.GetBytes(data);


                string CallFrom = "";
                if (API_ID == 3)
                {
                    CallFrom = "Journey Details";
                    Req = "journeyID=" + obj.JourneyID + " Customer_ID=" + Customer_ID + "";
                }
                if (API_ID == 5)
                {
                    CallFrom = "Shuftipro Scan";
                    Req = obj.JourneyID;
                }
                // Declare sumsub callform
                if (API_ID == 9)
                {
                    CallFrom = "Sumsub_ID_Scan_Request";
                }
                if (API_ID == 14)
                {
                    CallFrom = "Yoti_response";
                }
                string veriffstatus = "", veriffresponseCode = "", veriffcountryname = "", veriffDocumentName = "", veriffSenderNameOnID = "", veriffcname1 = "";
                string veriffmname1 = "", verifflname1 = "", veriffSenderID_Number = "", veriffIssue_date = "", veriffSex = "", veriffDocumentCategory = "",
                    veriffDocumentType = "", veriffFront_DocumentTypeID = "", veriffIssuingStateName = "", veriffSender_DateOfBirth = "", verifffrontdoc_expired = "";
                dynamic dynJson = null, dynJsonMedia = null;
                string veriffFrontDocURL = "", veriffBackDocURL = "", veriffFaceURL = "";
                string veriffFrontDocURLID = "", veriffBackDocURLID = "", veriffFaceURLID = "";
                string idDocType = "";
                string idDocCountry = "";
                string idDocFirstName = "";
                string idDocFirstNameEn = "";
                string idDocLastName = "";
                string idDocLastNameEn = "";
                DateTime idDocValidUntil = new DateTime(); string idDocValidUntildate = "";
                string idDocNumber = "";
                DateTime idDocDob = new DateTime();
                string idDocBgCheckViolations = "";
                string idDocOcrDocTypes = "";

                JArray identityImageIds = new JArray();
                DateTime SID_Expiry = new DateTime();
                JArray selfieImageIds = new JArray();
                string DocumentType = "";
                DateTime SID_bdate = new DateTime();
                string Remark = "";
                string stat = "";
                if (API_ID == 7)
                {
                    CallFrom = "Veriff Scan";
                    Req = obj.JourneyID;
                    string xHmacSignature = "";
                    string sharedSecretKey = Password;
                    string sessionId = obj.JourneyID;
                    byte[] sharedSecretKeyBytes = Encoding.UTF8.GetBytes(sharedSecretKey);
                    byte[] sessionIdBytes = Encoding.UTF8.GetBytes(sessionId);

                    using (var hmac = new HMACSHA256(sharedSecretKeyBytes))
                    {
                        byte[] hash = hmac.ComputeHash(sessionIdBytes);
                        xHmacSignature = BitConverter.ToString(hash).Replace("-", "").ToLower();
                    }

                    var client_ = new RestClient(urll + obj.JourneyID + "/decision");
                    client_.Timeout = -1;
                    var request_ = new RestRequest();
                    request_.AddHeader("Content-Type", "application/json");
                    request_.AddHeader("X-AUTH-CLIENT", UserName);
                    request_.AddHeader("X-HMAC-SIGNATURE", xHmacSignature);
                    var body = @"";
                    request_.AddParameter("application/json", body, ParameterType.RequestBody);
                    IRestResponse response = client_.Execute(request_);
                    dynJson = Newtonsoft.Json.JsonConvert.DeserializeObject(response.Content);
                    // string responseCode = dynJson["verification"].ToString();
                    veriffresponseCode = dynJson.ToString();
                    try
                    {
                        veriffstatus = dynJson["verification"]["status"].ToString();
                    }
                    catch (Exception exv) { }

                    client_ = new RestClient(urll + obj.JourneyID + "/media");
                    client_.Timeout = -1;
                    request_ = new RestRequest();
                    request_.AddHeader("Content-Type", "application/json");
                    request_.AddHeader("X-AUTH-CLIENT", UserName);
                    request_.AddHeader("X-HMAC-SIGNATURE", xHmacSignature);
                    body = @"";
                    request_.AddParameter("application/json", body, ParameterType.RequestBody);
                    IRestResponse responseMedia = client_.Execute(request_);
                    dynJsonMedia = Newtonsoft.Json.JsonConvert.DeserializeObject(responseMedia.Content);
                    int count = 0;
                    foreach (var items in dynJsonMedia["images"])
                    {
                        try
                        {
                            if (dynJsonMedia["images"][count]["name"].ToString() == "face")
                            {
                                veriffFaceURL = dynJsonMedia["images"][count]["url"].ToString();
                                veriffFaceURLID = dynJsonMedia["images"][count]["id"].ToString();
                            }
                            if (dynJsonMedia["images"][count]["name"].ToString() == "document-front-pre")
                            {
                                veriffFrontDocURL = dynJsonMedia["images"][count]["url"].ToString();
                                veriffFrontDocURLID = dynJsonMedia["images"][count]["id"].ToString();
                            }
                            if (dynJsonMedia["images"][count]["name"].ToString() == "document-back-pre")
                            {
                                veriffBackDocURL = dynJsonMedia["images"][count]["url"].ToString();
                                veriffBackDocURLID = dynJsonMedia["images"][count]["id"].ToString();
                            }
                        }
                        catch (Exception exv) { }
                        count++;
                    }

                    /*urll = urll.Replace("sessions", "media");
                    client_ = new RestClient(urll + veriffFaceURLID);
                    client_.Timeout = -1;
                    request_ = new RestRequest();
                    request_.AddHeader("Content-Type", "application/json");
                    request_.AddHeader("X-AUTH-CLIENT", UserName);
                    request_.AddHeader("X-HMAC-SIGNATURE", xHmacSignature);
                    body = @"";
                    request_.AddParameter("application/json", body, ParameterType.RequestBody);
                    responseMedia = client_.Execute(request_);
                    dynJsonMedia = Newtonsoft.Json.JsonConvert.DeserializeObject(responseMedia.Content);
                    */
                }

                if (API_ID == 9)
                {

                    var stamp = ((long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds).ToString();
                    string userId = obj.Custmer_Ref;
                    var applicant_id = obj.applicant_id;
                    // Calculate the value to sign (excluding the body)
                    var valueToSign = stamp + "GET" + $"/resources/applicants/{applicant_id}/one";


                    string appToken = UserName;
                    // Replace placeholders with actual values
                    valueToSign = valueToSign.Replace("{{sumsub_root_url}}", "")
                                             .Replace("{{applicantId}}", "")
                                             .Replace("{{levelName}}", "basic-kyc-level")
                                             .Replace("{{userId}}", userId);

                    // Calculate the signature
                    var secretKey = Password; // Replace with your secret key
                    var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey));
                    var signatureBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(valueToSign));
                    var signature = BitConverter.ToString(signatureBytes).Replace("-", "").ToLower();

                    // Set your dynamic userId here

                    var client1 = new RestClient(urll);
                    var request1 = new RestRequest($"/resources/applicants/{applicant_id}/one", Method.GET);
                    request1.AddHeader("Content-Type", "application/json");
                    request1.AddHeader("X-App-Token", appToken);
                    request1.AddHeader("X-App-Access-Ts", stamp);
                    request1.AddHeader("X-App-Access-Sig", signature);
                    IRestResponse responseT = client1.Execute(request1);
                    JObject jsonResponse = JObject.Parse(responseT.Content);


                    // CompanyInfo.InsertActivityLogDetails("Get The Sumsub Docs Details and applicantID 6 for Id scan " + responseT.Content + applicant_id + ". ", Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(Customer_ID), "GetJourney", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "ID Upload");

                    activity_log = "Get The Sumsub Docs Details and applicantID 6 for Id scan " + responseT.Content + applicant_id + "";

                    // Accessing values from the response
                    string id = (string)jsonResponse["id"];
                    DateTime createdAt = (DateTime)jsonResponse["createdAt"];
                    string key = (string)jsonResponse["key"];
                    string clientId = (string)jsonResponse["clientId"];
                    inspectionId = (string)jsonResponse["inspectionId"];
                    string externalUserId = (string)jsonResponse["externalUserId"];
                    //string firstName = (string)jsonResponse["info"]["firstName"];
                    //string lastName = (string)jsonResponse["info"]["lastName"];
                    //DateTime dob = (DateTime)jsonResponse["info"]["dob"];
                    //string country = (string)jsonResponse["info"]["country"];
                    // Accessing nested arrays
                    JArray idDocs = (JArray)jsonResponse["info"]["idDocs"];

                    // Check if idDocs is not empty
                    if (idDocs != null && idDocs.Count > 0)
                    {
                        // Iterate through each idDoc in idDocs array
                        foreach (JObject idDoc in idDocs)
                        {
                            try
                            {
                                if (idDoc["idDocType"] != null && !string.IsNullOrWhiteSpace(idDoc["idDocType"].ToString()))
                                {
                                    DocumentType = (string)idDoc["idDocType"];
                                }
                            }
                            catch (Exception ex) { }

                            try
                            {
                                if (idDoc["country"] != null && !string.IsNullOrWhiteSpace(idDoc["country"].ToString()))
                                {
                                    idDocCountry = (string)idDoc["country"];
                                }
                            }
                            catch (Exception ex) { }

                            try
                            {
                                if (idDoc["firstName"] != null && !string.IsNullOrWhiteSpace(idDoc["firstName"].ToString()))
                                {
                                    idDocFirstName = (string)idDoc["firstName"];
                                }
                            }
                            catch (Exception ex) { }

                            try
                            {
                                if (idDoc["firstNameEn"] != null && !string.IsNullOrWhiteSpace(idDoc["firstNameEn"].ToString()))
                                {
                                    idDocFirstNameEn = (string)idDoc["firstNameEn"];
                                }
                            }
                            catch (Exception ex) { }

                            try
                            {
                                if (idDoc["lastName"] != null && !string.IsNullOrWhiteSpace(idDoc["lastName"].ToString()))
                                {
                                    idDocLastName = (string)idDoc["lastName"];
                                }
                            }
                            catch (Exception ex) { }

                            try
                            {
                                if (idDoc["lastName"] != null && !string.IsNullOrWhiteSpace(idDoc["lastName"].ToString()))
                                {
                                    idDocLastName = (string)idDoc["lastName"];
                                }
                            }
                            catch (Exception ex) { }

                            try
                            {
                                if (idDoc["lastNameEn"] != null && !string.IsNullOrWhiteSpace(idDoc["lastNameEn"].ToString()))
                                {
                                    idDocLastNameEn = (string)idDoc["lastNameEn"];
                                }
                            }
                            catch (Exception ex) { }

                            try
                            {
                                if (idDoc["validUntil"] != null && !string.IsNullOrWhiteSpace(idDoc["validUntil"].ToString()))
                                {
                                    idDocValidUntil = (DateTime)idDoc["validUntil"];
                                    idDocValidUntildate = (string)idDoc["validUntil"];
                                }
                            }
                            catch (Exception ex) { }

                            try
                            {
                                if (idDoc["dob"] != null && !string.IsNullOrWhiteSpace(idDoc["dob"].ToString()))
                                {
                                    idDocDob = (DateTime)idDoc["dob"];
                                }
                            }
                            catch (Exception ex) { }

                            try
                            {
                                if (idDoc["number"] != null && !string.IsNullOrWhiteSpace(idDoc["number"].ToString()))
                                {
                                    idDocNumber = idDoc["number"].ToString().Replace(" ", string.Empty);
                                }
                            }
                            catch (Exception ex) { }


                            try
                            {
                                if (idDoc["bgCheckViolations"] != null && !string.IsNullOrWhiteSpace(idDoc["bgCheckViolations"].ToString()))
                                {
                                    idDocBgCheckViolations = (string)idDoc["bgCheckViolations"];
                                }
                            }
                            catch (Exception ex) { }

                            try
                            {
                                if (idDoc["ocrDocTypes"] != null && !string.IsNullOrWhiteSpace(idDoc["ocrDocTypes"].ToString()))
                                {
                                    idDocOcrDocTypes = (string)idDoc["ocrDocTypes"];
                                }
                            }
                            catch (Exception ex) { }

                            try
                            {
                                if (idDoc["mrzLine1"] != null && !string.IsNullOrWhiteSpace(idDoc["mrzLine1"].ToString()))
                                {
                                    idDocMrzLine1 = (string)idDoc["mrzLine1"];
                                }
                            }
                            catch (Exception ex) { }


                            try
                            {
                                if (idDoc["mrzLine2"] != null && !string.IsNullOrWhiteSpace(idDoc["mrzLine2"].ToString()))
                                {
                                    idDocMrzLine2 = (string)idDoc["mrzLine2"];
                                }
                            }
                            catch (Exception ex) { }



                        }
                    }


                    // Accessing nested objects




                    JObject review = (JObject)jsonResponse["review"];

                    reviewStatus = (string)review["reviewStatus"];

                    // Accessing nested objects
                    JObject reviewResult = (JObject)review["reviewResult"];
                    reviewAnswer = (string)reviewResult["reviewAnswer"];

                    if (reviewAnswer == "RED")
                    {
                        JArray rejectLabels = (JArray)reviewResult["rejectLabels"];
                        List<string> rejectLabelsList = rejectLabels.Select(x => (string)x).ToList();
                        string reviewRejectType = (string)reviewResult["reviewRejectType"];
                        stat = "1";
                    }
                    stat = "0";
                    responseapi = responseT.Content;

                    string applId = applicant_id;
                    requestapi = "/resources/applicants/" + applId + "/status" + signature + appToken + stamp;
                    Req = requestapi;


                    obj.Comments = responseT.Content;

                    obj.SenderNameOnID = idDocFirstNameEn + ' ' + idDocLastNameEn;

                    obj.SenderID_ExpiryDate = idDocValidUntil.ToString();
                    if (DateTime.TryParseExact(obj.SenderID_ExpiryDate, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out SID_Expiry))

                    {
                        obj.SenderID_ExpiryDate = SID_Expiry.ToString("yyyy-MM-dd");
                    }

                    if (DateTime.TryParseExact(idDocDob.ToString(), "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out SID_bdate))

                    {
                        obj.Sender_DateOfBirth = SID_bdate.ToString("yyyy-MM-dd");
                    }
                    CompanyInfo.InsertActivityLogDetails("Get The Sumsub Docs Details fetch all and applicantID 7 for Id scan " + responseT.Content + applicant_id + ". ", Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(Customer_ID), "GetJourney", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "ID Upload", context);
                }
                try
                {
                    MySqlCommand _cmd = new MySqlCommand("SaveAPIRequestResponce");
                    _cmd.CommandType = CommandType.StoredProcedure;
                    _cmd.Parameters.AddWithValue("_API_ID", API_ID);
                    _cmd.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                    _cmd.Parameters.AddWithValue("_Customer_ID", Customer_ID);
                    _cmd.Parameters.AddWithValue("_status", 0);
                    _cmd.Parameters.AddWithValue("_Function_name", CallFrom);
                    _cmd.Parameters.AddWithValue("_Remark", 0);
                    _cmd.Parameters.AddWithValue("_comments", Req);
                    _cmd.Parameters.AddWithValue("_Record_Insert_DateTime", obj.Record_Insert_DateTime);
                    _cmd.Parameters.AddWithValue("_Branch_ID", obj.Branch_ID);
                    int msg1 = db_connection.ExecuteNonQueryProcedure(_cmd);
                }
                catch (Exception ex)
                {
                    string error = ex.ToString().Replace("\'", "\\'");
                    MySqlCommand _cmd = new MySqlCommand("SaveException");
                    _cmd.CommandType = CommandType.StoredProcedure;
                    _cmd.Parameters.AddWithValue("_Record_Insert_DateTime", obj.Record_Insert_DateTime);
                    _cmd.Parameters.AddWithValue("_error", error);
                    _cmd.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                    int msg1 = db_connection.ExecuteNonQueryProcedure(_cmd);
                }
                IRestResponse response1 = new RestResponse();
                if (API_ID == 3)
                {

                    string userAuthenticationURI = "" + urll + "Search/GetEvaluatedPersonEntryValidationResults";

                    var client = new RestClient(userAuthenticationURI + "?id=" + obj.JourneyID + "");
                    client.Timeout = -1;
                    var request = new RestRequest(Method.GET);
                    request.AddHeader("Content-Type", "application/json");
                    request.AddHeader("Authorization", s13);
                    request.AddHeader("Cookie", "token " + s13);
                    response1 = client.Execute(request);
                    JObject jsonObject = JObject.Parse(response1.Content);
                    if (jsonObject.ContainsKey("BackSideWhiteImageUrl"))
                    {
                        BackSideWhiteImageUrl = jsonObject["BackSideWhiteImageUrl"]?.ToString();
                    }
                    resresult = response1.Content;

                    _ = Task.Run(() => CompanyInfo.InsertrequestLogTracker("getJourney Resonse body: " + resresult, 0, 0, 0, 0, "getJourneyresponse", Convert.ToInt32(0), Convert.ToInt32(0), "", context));

                }
                int score = 0; //string Remark = ""; 
                try
                {
                    string Bandtext = "", saveresp = "";
                    if (resresult != "" && API_ID == 3)
                    {
                        try
                        {
                            Newtonsoft.Json.Linq.JObject ob = Newtonsoft.Json.Linq.JObject.Parse(resresult);
                            Bandtext = Convert.ToString(ob["HighLevelResult"]);


                            //var arr = ob["ProcessedDocuments"][0]["ExtractedFields"];String roundTrip = Convert.ToString(resresult); byte[] bytes = System.Text.Encoding.UTF8.GetBytes(roundTrip); //string finalresp = System.Text.Encoding.UTF8.GetString(bytes);
                            try
                            {
                                saveresp = System.Text.Encoding.ASCII.GetString(System.Text.Encoding.GetEncoding("ISO-8859-8").GetBytes(resresult));
                            }
                            catch (Exception egc) { saveresp = resresult; }
                            // "Journey ID: " + journeyID + " HighLevelResult: " + Bandtext + " HighLevelResultDetails: " + ob["HighLevelResultDetails"] + " ProcessedDocuments: " + arr;

                            try
                            {
                                if (saveresp.Contains("error has occurred")) //retry 1
                                {
                                    string userAuthenticationURI = "" + urll + "Search/GetEvaluatedPersonEntryValidationResults";
                                    CompanyInfo.InsertActivityLogDetails("ID Scan " + CallFrom + " Audit 1: " + saveresp.Replace("\"", "") + " ", Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(Customer_ID), "GetJourney", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "ID Upload", context);
                                    response1 = new RestResponse();
                                    var client = new RestClient(userAuthenticationURI + "?id=" + obj.JourneyID + "");
                                    client.Timeout = -1;
                                    var request = new RestRequest(Method.GET);
                                    request.AddHeader("Content-Type", "application/json");
                                    request.AddHeader("Authorization", s13);
                                    request.AddHeader("Cookie", "token " + s13);
                                    response1 = client.Execute(request);
                                    resresult = response1.Content;
                                    if (resresult != "")
                                    {
                                        ob = Newtonsoft.Json.Linq.JObject.Parse(resresult);
                                        Bandtext = Convert.ToString(ob["HighLevelResult"]);
                                        saveresp = System.Text.Encoding.ASCII.GetString(System.Text.Encoding.GetEncoding("ISO-8859-8").GetBytes(resresult));
                                        if (saveresp.Contains("error has occurred")) //retry 2
                                        {
                                            CompanyInfo.InsertActivityLogDetails("ID Scan " + CallFrom + " Audit 2: " + saveresp.Replace("\"", "") + " ", Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(Customer_ID), "GetJourney", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "ID Upload", context);
                                            response1 = new RestResponse();
                                            response1 = client.Execute(request);
                                            resresult = response1.Content;
                                            if (resresult != "")
                                            {
                                                ob = Newtonsoft.Json.Linq.JObject.Parse(resresult);
                                                Bandtext = Convert.ToString(ob["HighLevelResult"]);
                                                saveresp = System.Text.Encoding.ASCII.GetString(System.Text.Encoding.GetEncoding("ISO-8859-8").GetBytes(resresult));
                                                if (saveresp.Contains("error has occurred")) //retry 3
                                                {
                                                    CompanyInfo.InsertActivityLogDetails("ID Scan " + CallFrom + " Audit 2: " + saveresp.Replace("\"", "") + " ", Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(Customer_ID), "GetJourney", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "ID Upload", context);
                                                    response1 = new RestResponse();
                                                    response1 = client.Execute(request);
                                                    resresult = response1.Content;
                                                    if (resresult != "")
                                                    {
                                                        ob = Newtonsoft.Json.Linq.JObject.Parse(resresult);
                                                        Bandtext = Convert.ToString(ob["HighLevelResult"]);
                                                        saveresp = System.Text.Encoding.ASCII.GetString(System.Text.Encoding.GetEncoding("ISO-8859-8").GetBytes(resresult));

                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            catch { }
                        }
                        catch (Exception ex) { }
                    }
                    else
                    // seetting Review status and facematch status
                    if (API_ID == 9)
                    {

                        try
                        {
                            Bandtext = reviewStatus;
                            if (reviewAnswer == "GREEN")
                            {
                                score = 100;
                            }
                            else
                            {
                                score = 0;
                            }
                        }
                        catch (Exception ex) { }

                    }
                    if (API_ID == 14)
                    {
                        if (faceMatchConfidenceScoreStr != "")
                        {
                            score = Convert.ToInt32(faceMatchConfidenceScoreStr);
                        }
                    }
                    if (API_ID == 5)
                    {
                        try
                        {
                            score = Convert.ToInt32(obj.shuftipro_doc_face_match_confidence);
                        }
                        catch { score = 0; }
                        try
                        {
                            Bandtext = Convert.ToString(obj.shufti_status).ToLower();

                            int index = Bandtext.IndexOf(".");
                            Bandtext = Bandtext.Substring(Bandtext.LastIndexOf('.') + 1);
                        }
                        catch { }
                    }
                    if (API_ID == 7)
                    {
                        try
                        {
                            Bandtext = veriffstatus;
                        }
                        catch { }
                    }

                    if (API_ID == 9)
                    {
                        Remark = stat;
                    }
                    if (API_ID == 14)
                    {
                        try
                        {
                            Bandtext = overallStatusStr;
                        }
                        catch { }
                    }
                    else
                    {
                        Remark = Convert.ToString(CompanyInfo.getAPIStatus(Bandtext, obj.Client_ID));
                        if (Remark != null && Remark != "")
                            Status = 0;
                    }
                    MySqlCommand _cmd;
                    if (API_ID == 3)
                    {
                        if (obj.scanstatus == "INPROGRESS")
                        {
                        }
                        else
                        {
                            score = GetFaceMatchScore(s13, urll, obj);
                        }
                    }
                    //code is moved
                    try
                    {
                        _cmd = new MySqlCommand("UpdateCustIDScanResults");//Update bandtext and score
                        _cmd.CommandType = CommandType.StoredProcedure;
                        _cmd.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                        _cmd.Parameters.AddWithValue("_Customer_ID", Customer_ID);
                        _cmd.Parameters.AddWithValue("_Remark", Remark);
                        _cmd.Parameters.AddWithValue("_Score", score);
                        int msg = db_connection.ExecuteNonQueryProcedure(_cmd);
                        _cmd.Dispose();
                    }
                    catch (Exception egx) { }

                    if (API_ID == 5)
                    {
                        CallFrom = "Shuftipro Scan";
                        saveresp = obj.JourneyID;
                    }

                    if (API_ID == 7)
                    {
                        CallFrom = "Veriff Scan Response";
                        saveresp = veriffresponseCode;
                        if (veriffstatus == "approved") { Status = 0; }
                    }

                    if (API_ID == 9)
                    {
                        CallFrom = "Sumsub_ID_Scan_Response";
                        saveresp = responseapi;
                    }
                    _cmd = new MySqlCommand("SaveAPIRequestResponce");
                    _cmd.CommandType = CommandType.StoredProcedure;
                    _cmd.Parameters.AddWithValue("_API_ID", API_ID);
                    _cmd.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                    _cmd.Parameters.AddWithValue("_Customer_ID", Customer_ID);
                    _cmd.Parameters.AddWithValue("_status", 1);
                    _cmd.Parameters.AddWithValue("_Function_name", CallFrom);
                    _cmd.Parameters.AddWithValue("_Remark", Remark);
                    if (API_ID == 9)
                    {
                        _cmd.Parameters.AddWithValue("_comments", saveresp);
                    }
                    else
                    {
                        _cmd.Parameters.AddWithValue("_comments", saveresp.Replace("\"", ""));
                    }

                    _cmd.Parameters.AddWithValue("_Record_Insert_DateTime", obj.Record_Insert_DateTime);
                    _cmd.Parameters.AddWithValue("_Branch_ID", obj.Branch_ID);
                    int msg1 = db_connection.ExecuteNonQueryProcedure(_cmd);
                    try
                    {
                        _cmd.Dispose();
                        _cmd = new MySqlCommand("customer_details_by_param");
                        _cmd.CommandType = CommandType.StoredProcedure;
                        string _whereclause = " and cr.Client_ID=" + obj.Client_ID + " and cr.Customer_ID=" + Customer_ID;

                        _cmd.Parameters.AddWithValue("_whereclause", _whereclause);
                        CompanyInfo.InsertActivityLogDetails("Cust Details where clause : " + _whereclause + "     ", Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(Customer_ID), "GetJourney", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "ID Upload", context);
                        _cmd.Parameters.AddWithValue("_SecurityKey", CompanyInfo.SecurityKey());
                        DataTable d1 = db_connection.ExecuteQueryDataTableProcedure(_cmd);
                        if (d1.Rows.Count == 1)
                        {
                            cname = Convert.ToString(d1.Rows[0]["First_Name"]).ToUpper();
                            mname = Convert.ToString(d1.Rows[0]["Middle_Name"]).ToUpper();
                            lname = Convert.ToString(d1.Rows[0]["Last_Name"]).ToUpper();
                        }
                        CompanyInfo.InsertActivityLogDetails("Cust Details where clause : " + _whereclause + "   & Fnm: " + cname + " Mnm: " + mname + "  Lnm: " + lname + ". ", Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(Customer_ID), "GetJourney", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "ID Upload", context);
                        if (API_ID == 3 || API_ID == 5 || API_ID == 7 || API_ID == 9 || API_ID == 14)
                        {
                            if (Remark == "1" || Remark == "2" || Remark == "3" || Remark == "4" || Remark == "5")//alert or refer or notsupported or undefined or INPROGRESS then  send mail to admin
                            {
                                string sendmsg = "";
                                string EmailUrl = "", Company_Name = "";
                                if (dtc.Rows.Count > 0)
                                {
                                    EmailUrl = Convert.ToString(dtc.Rows[0]["Company_URL_Admin"]);
                                    Company_Name = Convert.ToString(dtc.Rows[0]["Company_Name"]);
                                }
                                if (API_ID == 3)
                                {
                                    sendmsg = "Response from GBG ID Scan for  " + d1.Rows[0]["WireTransfer_ReferanceNo"] + "   is   " + Bandtext + "";
                                }
                                if (API_ID == 7)
                                {
                                    sendmsg = "Response from Veriff Station for  " + obj.JourneyID + "   is   " + Bandtext + "";
                                }
                                if (API_ID == 14)
                                {
                                    sendmsg = "Response from Yoti ID Sacn for  " + obj.JourneyID + "   is   " + Bandtext + "";
                                }


                                string EmailID = "";
                                DataTable dt_admin_Email_list = (DataTable)CompanyInfo.getAdminEmailList(obj.Client_ID, obj.Branch_ID);
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

                                subject = "" + Company_Name + " - Compliance - ID Scan Alert - " + d1.Rows[0]["WireTransfer_ReferanceNo"];

                                string mail_send = (string)CompanyInfo.Send_Mail(dtc, EmailID, body, subject, obj.Client_ID, obj.Branch_ID, "", "", "", context);
                            }
                        }
                    }
                    catch (Exception ae)
                    {
                        //string stattus = (string)mtsmethods.InsertErrorLogDetails(ae.Message.Replace("\'", "\\'"), user_id, "Check Credit Safe", i.Branch_ID, i.Client_ID);
                    }

                }
                catch (Exception ex)
                {
                    string error = ex.ToString().Replace("\'", "\\'");
                    MySqlCommand _cmd = new MySqlCommand("SaveException");
                    _cmd.CommandType = CommandType.StoredProcedure;
                    _cmd.Parameters.AddWithValue("_Record_Insert_DateTime", obj.Record_Insert_DateTime);
                    _cmd.Parameters.AddWithValue("_error", error);
                    _cmd.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                    int msg1 = db_connection.ExecuteNonQueryProcedure(_cmd);
                }

                if (overallStatusStr != "" || reviewStatus != "" || response1.StatusCode.ToString() == "OK" || obj.JourneyID.Contains("SP_") || veriffstatus != "")
                {

                    var countryname = new object();
                    var DocumentName = new object();
                    Newtonsoft.Json.Linq.JObject ob = new Newtonsoft.Json.Linq.JObject();
                    if (API_ID == 3)
                    {
                        /*string formattedstr = System.Text.Encoding.ASCII.GetString(System.Text.Encoding.GetEncoding("ISO-8859-8").GetBytes(resresult));
                        ob = Newtonsoft.Json.Linq.JObject.Parse(formattedstr);
                        journeyID = ob["EvaluatedPersonEntryId"]; countryname = ob["CountryName"];
                        DocumentName = ob["DocumentName"];
                        obj.SenderNameOnID = Convert.ToString(ob["FullName"]);
                        */
                        try
                        {
                            ob = Newtonsoft.Json.Linq.JObject.Parse(resresult);
                            journeyID = ob["EvaluatedPersonEntryId"]; countryname = ob["CountryName"];
                            DocumentName = ob["DocumentName"];
                            obj.SenderNameOnID = Convert.ToString(ob["FullName"]);
                        }
                        catch (Exception ex) { }

                    }

                    string BirthPlace = "", Sex = "", DocumentCategory = "", IssuingStateName = "", Front_DocumentTypeID = "";
                    //obj.SenderNameOnID = Convert.ToString(ob["FirstName"]) + " " + Convert.ToString(ob["LastName"]);  obj.SenderID_Number = Convert.ToString(ob["DocumentNumber"]);  obj.SenderID_ExpiryDate = Convert.ToString(ob["ExpiryDate"]); obj.Issue_date = Convert.ToString(ob["IssueDate"]);  var xyz = Convert.ToString(ob["IssuingAuthority"]); BirthPlace = Convert.ToString(ob["BirthPlace"]);
                    //Sex = Convert.ToString(ob["Gender"]); DocumentCategory = Convert.ToString(ob["DocumentCategory"]); DocumentType = Convert.ToString(ob["DocumentType"]);
                    string cname1 = "", mname1 = "", lname1 = "", MRZno = "";

                    if (API_ID == 3)
                    {
                        var arr = ob["ExtractedFields"];
                        for (var i = 0; i < arr.Count(); i++)
                        {
                            string name = Convert.ToString(arr[i]["Name"]);
                            if (name == "ExpiryDate") { obj.SenderID_ExpiryDate = Convert.ToString(arr[i]["Value"]); }
                            try { if (name == "FirstName") { cname1 = Convert.ToString(arr[i]["Value"]); } if (name == "MiddleName") { mname1 = Convert.ToString(arr[i]["Value"]); } if (name == "LastName") { lname1 = Convert.ToString(arr[i]["Value"]); } }
                            catch { }
                            if (name == "BirthPlace") { BirthPlace = Convert.ToString(arr[i]["Value"]); }
                            if (name == "DocumentNumber") { obj.SenderID_Number = Convert.ToString(arr[i]["Value"]).Replace(" ", ""); }
                            if (name == "IssueDate") { obj.Issue_date = Convert.ToString(arr[i]["Value"]); }
                            if (name == "IssuingAuthority") { var xyz = Convert.ToString(arr[i]["Value"]); }
                            if (name == "Sex") { Sex = Convert.ToString(arr[i]["Value"]); }
                            if (name == "DocumentCategory") { DocumentCategory = Convert.ToString(arr[i]["Value"]); }
                            if (name == "DocumentType") { DocumentType = Convert.ToString(arr[i]["Value"]); }
                            obj.DocumentName = DocumentType;
                            if (name == "IssuingStateName") { IssuingStateName = Convert.ToString(arr[i]["Value"]); }
                            if (name == "Front Document Type ID") { Front_DocumentTypeID = Convert.ToString(arr[i]["Value"]); }
                            if (name == "BirthDate") { obj.Sender_DateOfBirth = Convert.ToString(arr[i]["Value"]); }
                            if (name == "MRZLine2") { obj.MRZ_number = Convert.ToString(arr[i]["Value"]); }
                        }
                    }

                    if (API_ID == 3)
                    {
                        placeOfBirth = BirthPlace;
                    }
                    if (API_ID == 14)
                    {
                        placeOfBirth = documentPlaceOfBirthStr;
                    }


                    MySqlCommand _cmd10 = new MySqlCommand("UpdateCustIDScanResults");//Update bandtext and score
                    _cmd10.CommandType = CommandType.StoredProcedure;
                    _cmd10.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                    _cmd10.Parameters.AddWithValue("_Customer_ID", Customer_ID);
                    _cmd10.Parameters.AddWithValue("_Remark", Remark);
                    _cmd10.Parameters.AddWithValue("_Score", score);
                    _cmd10.Parameters.AddWithValue("_Place_Of_Birth", placeOfBirth);

                    int msg10 = db_connection.ExecuteNonQueryProcedure(_cmd10);
                    _cmd10.Dispose();

                    int shuftipro_frontdoc_expired = 0;
                    if (API_ID == 5)
                    {
                        journeyID = Convert.ToString(obj.ShuftiId);
                        countryname = Convert.ToString(obj.shuftipro_document_proof_document_country);
                        DocumentName = Convert.ToString(obj.shuftipro_document_proof_document_official_name);
                        obj.SenderNameOnID = Convert.ToString(obj.shuftipro_document_proof_full_name);

                        cname1 = Convert.ToString(obj.shuftipro_document_first_name);
                        mname1 = Convert.ToString(obj.shuftipro_document_middle_name);
                        lname1 = Convert.ToString(obj.shuftipro_document_last_name);
                        string full_name = Convert.ToString(obj.shuftipro_document_proof_full_name);
                        if (cname1 == "" || cname1 == null)
                        {
                            if (full_name.Length > 0)
                                cname1 = full_name.Split(' ')[0];
                        }
                        if (mname1 == "" || mname1 == null)
                        {
                            if (full_name.Length > 1)
                                mname1 = full_name.Split(' ')[1];
                        }
                        if (lname1 == "" || lname1 == null)
                        {
                            for (int i = 2; i < full_name.Split(' ').Length; i++)
                            {
                                lname1 += full_name.Split(' ')[i];
                            }
                        }

                        obj.SenderID_Number = Convert.ToString(obj.shuftipro_document_proof_document_number);
                        obj.Issue_date = Convert.ToString(obj.shuftipro_doc_issue_date);
                        Sex = Convert.ToString(obj.shuftipro_document_proof_gender);
                        DocumentCategory = "";
                        DocumentType = Convert.ToString(obj.shuftipro_doc_selected_type);
                        Front_DocumentTypeID = Convert.ToString(obj.shuftipro_doc_selected_type);
                        IssuingStateName = "";
                        obj.Sender_DateOfBirth = Convert.ToString(obj.shuftipro_document_proof_dob);
                        score = Convert.ToInt32(obj.shuftipro_doc_face_match_confidence);
                        shuftipro_frontdoc_expired = Convert.ToInt32(obj.shuftipro_frontdoc_expired);
                    }

                    if (API_ID == 7)
                    {
                        try
                        {
                            journeyID = obj.JourneyID;
                            try
                            {
                                countryname = dynJson["verification"]["person"]["addresses"][0]["parsedAddress"]["country"].ToString();
                            }
                            catch (Exception ex) { }
                            DocumentName = dynJson["verification"]["person"]["firstName"].ToString() + " " + dynJson["verification"]["person"]["lastName"].ToString();
                            obj.SenderNameOnID = dynJson["verification"]["person"]["firstName"].ToString() + " " + dynJson["verification"]["person"]["lastName"].ToString();

                            cname1 = dynJson["verification"]["person"]["firstName"].ToString();
                            mname1 = "";
                            lname1 = dynJson["verification"]["person"]["lastName"].ToString();
                            string full_name = dynJson["verification"]["person"]["firstName"].ToString() + " " + dynJson["verification"]["person"]["lastName"].ToString();
                            if (cname1 == "" || cname1 == null)
                            {
                                if (full_name.Length > 0)
                                    cname1 = full_name.Split(' ')[0];
                            }
                            if (mname1 == "" || mname1 == null)
                            {
                                if (full_name.Length > 1)
                                    mname1 = full_name.Split(' ')[1];
                            }
                            if (lname1 == "" || lname1 == null)
                            {
                                for (int i = 2; i < full_name.Split(' ').Length; i++)
                                {
                                    lname1 += full_name.Split(' ')[i];
                                }
                            }

                            if (full_name.Length > 0)
                                cname1 = full_name.Split(' ')[0];

                            try
                            {
                                obj.Issue_date = dynJson["verification"]["document"]["firstIssue"].ToString();
                            }
                            catch (Exception ex) { obj.Issue_date = null; }
                            try { Sex = dynJson["verification"]["person"]["gender"].ToString(); } catch (Exception ex) { }
                            DocumentCategory = "";
                            try { DocumentType = dynJson["verification"]["document"]["type"].ToString(); } catch (Exception ex) { }
                            try { Front_DocumentTypeID = dynJson["verification"]["document"]["type"].ToString(); } catch (Exception ex) { }
                            try { IssuingStateName = dynJson["verification"]["document"]["issuedBy"].ToString(); } catch (Exception ex) { }
                            try
                            {
                                obj.Sender_DateOfBirth = dynJson["verification"]["person"]["dateOfBirth"].ToString();
                            }
                            catch (Exception ex) { obj.Sender_DateOfBirth = null; }
                            score = 0;
                            try
                            {
                                obj.SenderID_ExpiryDate = verifffrontdoc_expired = dynJson["verification"]["document"]["validUntil"].ToString();
                            }
                            catch (Exception ex) { obj.SenderID_ExpiryDate = null; }
                            try
                            {
                                obj.SenderID_Number = dynJson["verification"]["document"]["number"].ToString();
                            }
                            catch (Exception ex) { obj.SenderID_Number = null; }
                        }
                        catch (Exception ex) { }
                    }

                    int frontside = 0, liveness = 0, facematch = 0;
                    if (API_ID == 3)
                    {
                        var arr1 = ob["JourneySteps"];
                        for (var i = 0; i < arr1.Count(); i++)
                        {
                            string type = Convert.ToString(arr1[i]["Type"]);
                            string val = Convert.ToString(arr1[i]["HighLevelResult"]).ToLower();
                            if (type == "FRONTSIDE")
                            {
                                if (val != "passed") { frontside = 1; }
                            }
                            else if (type == "LIVENESS")
                            {
                                if (val != "passed") { liveness = 1; }
                            }
                        }
                        var arr2 = ob["AdditionalData"];
                        for (var i = 0; i < arr2.Count(); i++)
                        {
                            string type = Convert.ToString(arr2[i]["Name"]);
                            string val = Convert.ToString(arr2[i]["Value"]).ToLower();
                            if (type == "AutomatedFaceMatchResult")
                            {
                                if (val != "passed") { facematch = 1; }
                            }
                        }
                    }
                    if (API_ID == 9)
                    {
                        if (reviewAnswer != "GREEN")
                        {

                            frontside = 1;
                        }
                        else
                        {
                            frontside = 0;
                        }
                    }
                    if (API_ID == 14)
                    {
                        if (livenessRecommendationStr == "APPROVE")
                        {
                            liveness = 0;
                        }
                        else
                        {
                            liveness = 1;
                        }

                        if (faceMatchRecommendationStr == "APPROVE")
                        {
                            facematch = 0;
                            frontside = 0;
                        }
                        else
                        {
                            facematch = 1;
                            frontside = 1;
                        }
                    }
                    // Get the liveness and facematchb result for sumsub
                    if (API_ID == 9)
                    {
                        var stamp = ((long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds).ToString();
                        string userId = obj.Custmer_Ref;
                        var applicant_id = obj.applicant_id;
                        // Calculate the value to sign (excluding the body)
                        var valueToSign = stamp + "GET" + $"/resources/checks/latest?type=FACE_LIVELINESS&applicantId={applicant_id}";


                        string appToken = UserName;
                        // Replace placeholders with actual values
                        valueToSign = valueToSign.Replace("{{sumsub_root_url}}", "")
                                                 .Replace("{{applicantId}}", "")
                                                 .Replace("{{levelName}}", "basic-kyc-level")
                                                 .Replace("{{userId}}", userId);

                        // Calculate the signature
                        var secretKey = Password; // Replace with your secret key
                        var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey));
                        var signatureBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(valueToSign));
                        var signature = BitConverter.ToString(signatureBytes).Replace("-", "").ToLower();

                        // Set your dynamic userId here
                        var client1 = new RestClient(urll);
                        var request1 = new RestRequest($"/resources/checks/latest?type=FACE_LIVELINESS&applicantId={applicant_id}", Method.GET);
                        request1.AddHeader("Content-Type", "application/json");
                        request1.AddHeader("X-App-Token", appToken);
                        request1.AddHeader("X-App-Access-Ts", stamp);
                        request1.AddHeader("X-App-Access-Sig", signature);
                        IRestResponse responseT = client1.Execute(request1);
                        JObject jsonResponse = JObject.Parse(responseT.Content);

                        //CompanyInfo.InsertActivityLogDetails("Get The Sumsub liveness check Details and applicantID 8 for Id scan " + responseT.Content + applicant_id + ". ", Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(Customer_ID), "GetJourney", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "ID Upload");

                        activity_log = "Get The Sumsub liveness check Details and applicantID 8 for Id scan " + responseT.Content + applicant_id + "";

                        JArray checks = (JArray)jsonResponse["checks"];
                        foreach (JObject check in checks)
                        {
                            string answer = (string)check["answer"];
                            string checkType = (string)check["checkType"];
                            DateTime createdAt = (DateTime)check["createdAt"];
                            string id = (string)check["id"];

                            JObject livenessInfo = (JObject)check["livenessInfo"];
                            JObject livenessData = (JObject)livenessInfo["livenessData"];
                            JArray images = (JArray)livenessData["images"];

                            foreach (JObject image in images)
                            {
                                string ts = (string)image["ts"];
                                // You can access other fields in the 'image' object similarly
                            }

                            JObject livenessResult = (JObject)livenessInfo["livenessResult"];
                            string livenessResultAnswer = (string)livenessResult["answer"];
                            string deviceDesc = (string)livenessInfo["deviceDesc"];

                            if (livenessResultAnswer == "GREEN")
                            {
                                obj.background_checks = "0";
                            }



                        }

                    }

                    if (API_ID == 5)
                    {
                        if (obj.background_checks == "0")
                        {
                            liveness = 1;
                        }
                        if (obj.shuftipro_frontdoc_available == "0")
                        {
                            frontside = 1;
                        }
                        if (obj.shuftipro_face_on_document_matched == "0")
                        {
                            facematch = 1;
                        }
                    }

                    if (API_ID == 7)
                    {
                        if (dynJson["status"].ToString() != "success")
                        {
                            liveness = 1;
                        }
                        if (dynJson["verification"]["document"]["type"].ToString() == "")
                        {
                            frontside = 1;
                        }
                    }
                    //checking the liveness for sumsub

                    if (API_ID == 9)
                    {
                        if (obj.background_checks == "0")
                        {
                            liveness = 0;
                        }
                        if (reviewAnswer == "GREEN")
                        {

                            facematch = 0;
                        }
                    }
                    /**************************             Check Score               *****************************/
                    //Check probable match

                    int namevalidflag = 1; string res1 = "";
                    if (cname != "" && cname != null)
                        res1 = cname.Substring(0, 1);
                    string res3 = "";
                    if (lname != "" && lname != null)
                        res3 = lname.Substring(0, 1);

                    if (obj.SenderNameOnID == (cname + " " + lname))
                    {
                        namevalidflag = 0;//Exact Match
                    }
                    else if (mname != null && mname != "" && mname != " ")
                    {
                        string res2 = mname.Substring(0, 1);
                        if (obj.SenderNameOnID.ToLower() == (cname.ToLower() + " " + mname.ToLower() + " " + lname.ToLower()) || obj.SenderNameOnID.ToLower() == (cname.ToLower() + " " + res2.ToLower() + " " + lname.ToLower()))
                        {
                            namevalidflag = 0; // Exact Match
                        }
                        else if (obj.SenderNameOnID.ToLower() == (res1.ToLower() + " " + mname.ToLower() + " " + lname.ToLower()) || obj.SenderNameOnID.ToLower() == (cname.ToLower() + " " + mname.ToLower() + " " + res3.ToLower()) || obj.SenderNameOnID.ToLower() == (cname.ToLower() + " " + res2.ToLower() + " " + lname.ToLower())
                            || cname1.ToLower() == cname.ToLower() || cname1.ToLower() == lname.ToLower() || cname1.ToLower() == res1.ToLower() || lname1.ToLower() == lname.ToLower() || lname1.ToLower() == cname.ToLower() || lname1.ToLower() == res3.ToLower() || mname1.ToLower() == mname.ToLower() || mname1.ToLower() == res2.ToLower())
                        {
                            namevalidflag = 2;//Probable match
                        }
                    }
                    else if (obj.SenderNameOnID.ToLower() == (cname.ToLower() + " " + lname.ToLower()) || obj.SenderNameOnID.ToLower() == (res1.ToLower() + " " + lname.ToLower()) || obj.SenderNameOnID.ToLower() == (cname.ToLower() + " " + res3.ToLower())
                         || cname1.ToLower() == cname.ToLower() || cname1.ToLower() == lname.ToLower() || cname1.ToLower() == res1.ToLower() || lname1.ToLower() == lname.ToLower() || lname1.ToLower() == cname.ToLower() || lname1.ToLower() == res3.ToLower())
                    {
                        namevalidflag = 2; //Probable Match
                    }
                    else
                    {
                        namevalidflag = 1;// Invalid
                    }

                    /*if (API_ID == 5)
                    {
                        if (obj.SenderNameOnID.ToLower().Contains(cname.ToLower()) && obj.SenderNameOnID.Contains(mname.ToLower()) && obj.SenderNameOnID.Contains(lname.ToLower()))
                        {
                            namevalidflag = 0;
                        }
                        if (obj.SenderNameOnID.ToLower().Contains(cname.ToLower()) || obj.SenderNameOnID.Contains(mname.ToLower()) || obj.SenderNameOnID.Contains(lname.ToLower()))
                        {
                            namevalidflag = 2;
                        }
                    }*/


                    if (namevalidflag == 2)
                    {
                        if (API_ID == 3)
                            CompanyInfo.InsertActivityLogDetails("Probable match found for journey " + journeyID + ". Customer name on GBG ID Scan: Full Name " + obj.SenderNameOnID + ", First Name: " + obj.SenderNameOnID + ", Middle Name: " + obj.SenderNameOnID + ", Last Name: " + obj.SenderNameOnID + " and customer name in the system is " + (cname + " " + mname + " " + res3) + ". ", Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(Customer_ID), "GetJourney", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "ID Upload", context);
                        if (API_ID == 7 || API_ID == 5)
                            CompanyInfo.InsertActivityLogDetails("Probable match found for journey " + journeyID + ". Customer name on ID Scan: Full Name " + obj.SenderNameOnID + ", First Name: " + obj.SenderNameOnID + ", Middle Name: " + obj.SenderNameOnID + ", Last Name: " + obj.SenderNameOnID + " and customer name in the system is " + (cname + " " + mname + " " + res3) + ". ", Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(Customer_ID), "GetJourney", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "ID Upload", context);
                        if (API_ID == 9)
                            CompanyInfo.InsertActivityLogDetails("Probable match found for journey " + journeyID + ". Customer name on Sumsub ID Scan: Full Name " + obj.SenderNameOnID + ", First Name: " + obj.SenderNameOnID + ", Middle Name: " + obj.SenderNameOnID + ", Last Name: " + obj.SenderNameOnID + " and customer name in the system is " + (cname + " " + mname + " " + res3) + ". ", Convert.ToInt32(obj.Customer_ID), 0, Convert.ToInt32(obj.Customer_ID), Convert.ToInt32(Customer_ID), "GetJourney", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "ID Upload", context);
                        if (API_ID == 14)
                            CompanyInfo.InsertActivityLogDetails("Probable match found for journey " + obj.JourneyID + ". Customer name on Yoti ID Scan: Full Name " + obj.SenderNameOnID + ", First Name: " + obj.SenderNameOnID + ", Middle Name: " + obj.SenderNameOnID + ", Last Name: " + obj.SenderNameOnID + " and customer name in the system is " + (cname + " " + mname + " " + res3) + ". ", Convert.ToInt32(obj.Customer_ID), 0, Convert.ToInt32(obj.Customer_ID), Convert.ToInt32(Customer_ID), "GetJourney", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "ID Upload", context);

                    }
                    else if (namevalidflag == 1)
                    {
                        if (API_ID == 3)
                            CompanyInfo.InsertActivityLogDetails("Name on ID is mismatched for journey " + journeyID + ". Customer name on GBG ID Scan: Full Name " + obj.SenderNameOnID + ", First Name: " + obj.SenderNameOnID + ", Middle Name: " + obj.SenderNameOnID + ", Last Name: " + obj.SenderNameOnID + " and customer name in the system is " + (cname + " " + mname + " " + res3) + ". ", Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(Customer_ID), "GetJourney", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "ID Upload", context);
                        if (API_ID == 7 || API_ID == 5)
                            CompanyInfo.InsertActivityLogDetails("Name on ID is mismatched for journey " + journeyID + ". Customer name on ID Scan: Full Name " + obj.SenderNameOnID + ", First Name: " + obj.SenderNameOnID + ", Middle Name: " + obj.SenderNameOnID + ", Last Name: " + obj.SenderNameOnID + " and customer name in the system is " + (cname + " " + mname + " " + res3) + ". ", Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(Customer_ID), "GetJourney", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "ID Upload", context);
                        if (API_ID == 9)
                            CompanyInfo.InsertActivityLogDetails("Name on ID is mismatched for journey " + journeyID + ". Customer name on Sumsub ID Scan: Full Name " + obj.SenderNameOnID + ", First Name: " + obj.SenderNameOnID + ", Middle Name: " + obj.SenderNameOnID + ", Last Name: " + obj.SenderNameOnID + " and customer name in the system is " + (cname + " " + mname + " " + res3) + ". ", Convert.ToInt32(obj.Customer_ID), 0, Convert.ToInt32(obj.Customer_ID), Convert.ToInt32(Customer_ID), "GetJourney", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "ID Upload", context);
                        if (API_ID == 14)
                            CompanyInfo.InsertActivityLogDetails("Name on ID is mismatched for journey " + obj.JourneyID + ". Customer name on Yoti ID Scan: Full Name " + obj.SenderNameOnID + ", First Name: " + obj.SenderNameOnID + ", Middle Name: " + obj.SenderNameOnID + ", Last Name: " + obj.SenderNameOnID + " and customer name in the system is " + (cname + " " + mname + " " + res3) + ". ", Convert.ToInt32(obj.Customer_ID), 0, Convert.ToInt32(obj.Customer_ID), Convert.ToInt32(Customer_ID), "GetJourney", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "ID Upload", context);

                    }
                    int IsIdvalid = 0;// use it for probable match //|| obj.Sender_DateOfBirth == null || obj.Sender_DateOfBirth == "" 
                    if ((score <= 25 || namevalidflag == 1 || obj.SenderNameOnID == "Unknown" || Remark == "3" || obj.SenderNameOnID == null || obj.SenderNameOnID == "" ||
                        obj.SenderID_ExpiryDate == null || obj.SenderID_ExpiryDate == "" || DocumentType == "Unknown" || DocumentType == "" || DocumentType == null || frontside == 1 || liveness == 1 || facematch == 1) && API_ID != 7)
                    {
                        #region notific
                        string notification_icon = "", notification_message = "";
                        if (obj.IDType_ID == 1 || obj.IDType_ID == 5)
                        {
                            notification_icon = "wrong-id-upload.jpg";
                            notification_message = "<span class='cls-admin'>Invalid <strong class='cls-priamary'>Primary ID</strong> document found on ID Scan.</span>";
                        }
                        CompanyInfo.save_notification_compliance(notification_message, notification_icon, Convert.ToString(Customer_ID), Convert.ToDateTime(obj.Record_Insert_DateTime), obj.Client_ID, 1, 0, obj.Branch_ID, 0, 1, 1, 0, context);
                        if (frontside == 1)
                        {
                            notification_icon = "wrong-id-upload.jpg";
                            notification_message = "<span class='cls-admin'>Front side match of <strong class='cls-priamary'>Primary ID</strong> failed.</span>";
                            CompanyInfo.save_notification_compliance(notification_message, notification_icon, Convert.ToString(Customer_ID), Convert.ToDateTime(obj.Record_Insert_DateTime), obj.Client_ID, 1, 0, obj.Branch_ID, 0, 1, 1, 0, context);
                        }
                        if (liveness == 1)
                        {
                            notification_icon = "wrong-id-upload.jpg";
                            notification_message = "<span class='cls-admin'>Liveness failed.</span>";
                            CompanyInfo.save_notification_compliance(notification_message, notification_icon, Convert.ToString(Customer_ID), Convert.ToDateTime(obj.Record_Insert_DateTime), obj.Client_ID, 1, 0, obj.Branch_ID, 0, 1, 1, 0, context);
                        }
                        if (facematch == 1)
                        {
                            notification_icon = "wrong-id-upload.jpg";
                            notification_message = "<span class='cls-admin'>Face match failed.</span>";
                            CompanyInfo.save_notification_compliance(notification_message, notification_icon, Convert.ToString(Customer_ID), Convert.ToDateTime(obj.Record_Insert_DateTime), obj.Client_ID, 1, 0, obj.Branch_ID, 0, 1, 1, 0, context);
                        }

                        #endregion notific
                        CompanyInfo.InsertActivityLogDetails("Invalid Document found for journey " + journeyID + "", Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(Customer_ID), "GetJourney", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "ID Upload", context);
                        try
                        {
                            MySqlCommand _cmd = new MySqlCommand("UpdateCustIDScanResults");//Update bandtext and score
                            _cmd.CommandType = CommandType.StoredProcedure;
                            _cmd.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                            _cmd.Parameters.AddWithValue("_Customer_ID", Customer_ID);
                            _cmd.Parameters.AddWithValue("_Remark", "");
                            _cmd.Parameters.AddWithValue("_Score", 0);
                            int msg = db_connection.ExecuteNonQueryProcedure(_cmd);
                            _cmd.Dispose();
                        }
                        catch { }
                        MySqlCommand cmdupdate1 = new MySqlCommand("Get_Permissions");
                        cmdupdate1.CommandType = CommandType.StoredProcedure;
                        cmdupdate1.Parameters.AddWithValue("Per_ID", 75);// Validate IDSCan
                        cmdupdate1.Parameters.AddWithValue("ClientID", obj.Client_ID);
                        DataTable pm = db_connection.ExecuteQueryDataTableProcedure(cmdupdate1);
                        if (pm.Rows.Count > 0)
                        {
                            if (Convert.ToString(pm.Rows[0]["Status_ForCustomer"]) == "0")
                            {
                                dt.Rows.Add(1, "Invalid Document", obj.SenderID_ID);
                                return dt;
                            }
                        }
                    }

                    else if ((namevalidflag == 1 || obj.SenderNameOnID == "Unknown" || Remark == "3" || obj.SenderNameOnID == null || obj.SenderNameOnID == "" ||
                         obj.SenderID_ExpiryDate == null || obj.SenderID_ExpiryDate == "" || DocumentType == "Unknown" || DocumentType == "" || DocumentType == null || frontside == 1 || liveness == 1 || facematch == 1) && API_ID == 7)
                    {
                        #region notific
                        string notification_icon = "", notification_message = "";
                        if (obj.IDType_ID == 1 || obj.IDType_ID == 5)
                        {
                            notification_icon = "wrong-id-upload.jpg";
                            notification_message = "<span class='cls-admin'>Invalid <strong class='cls-priamary'>Primary ID</strong> document found on ID Scan.</span>";
                        }
                        CompanyInfo.save_notification_compliance(notification_message, notification_icon, Convert.ToString(Customer_ID), Convert.ToDateTime(obj.Record_Insert_DateTime), obj.Client_ID, 1, 0, obj.Branch_ID, 0, 1, 1, 0, context);
                        if (frontside == 1)
                        {
                            notification_icon = "wrong-id-upload.jpg";
                            notification_message = "<span class='cls-admin'>Front side match of <strong class='cls-priamary'>Primary ID</strong> failed.</span>";
                            CompanyInfo.save_notification_compliance(notification_message, notification_icon, Convert.ToString(Customer_ID), Convert.ToDateTime(obj.Record_Insert_DateTime), obj.Client_ID, 1, 0, obj.Branch_ID, 0, 1, 1, 0, context);
                        }
                        if (liveness == 1)
                        {
                            notification_icon = "wrong-id-upload.jpg";
                            notification_message = "<span class='cls-admin'>Liveness failed.</span>";
                            CompanyInfo.save_notification_compliance(notification_message, notification_icon, Convert.ToString(Customer_ID), Convert.ToDateTime(obj.Record_Insert_DateTime), obj.Client_ID, 1, 0, obj.Branch_ID, 0, 1, 1, 0, context);
                        }
                        if (facematch == 1)
                        {
                            notification_icon = "wrong-id-upload.jpg";
                            notification_message = "<span class='cls-admin'>Face match failed.</span>";
                            CompanyInfo.save_notification_compliance(notification_message, notification_icon, Convert.ToString(Customer_ID), Convert.ToDateTime(obj.Record_Insert_DateTime), obj.Client_ID, 1, 0, obj.Branch_ID, 0, 1, 1, 0, context);
                        }

                        #endregion notific
                        CompanyInfo.InsertActivityLogDetails("Invalid Document found for journey " + journeyID + "", Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(Customer_ID), "GetJourney", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "ID Upload", context);
                        try
                        {
                            MySqlCommand _cmd = new MySqlCommand("UpdateCustIDScanResults");//Update bandtext and score
                            _cmd.CommandType = CommandType.StoredProcedure;
                            _cmd.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                            _cmd.Parameters.AddWithValue("_Customer_ID", Customer_ID);
                            _cmd.Parameters.AddWithValue("_Remark", "");
                            _cmd.Parameters.AddWithValue("_Score", 0);
                            int msg = db_connection.ExecuteNonQueryProcedure(_cmd);
                            _cmd.Dispose();
                        }
                        catch { }
                        MySqlCommand cmdupdate1 = new MySqlCommand("Get_Permissions");
                        cmdupdate1.CommandType = CommandType.StoredProcedure;
                        cmdupdate1.Parameters.AddWithValue("Per_ID", 75);// Validate IDSCan
                        cmdupdate1.Parameters.AddWithValue("ClientID", obj.Client_ID);
                        DataTable pm = db_connection.ExecuteQueryDataTableProcedure(cmdupdate1);
                        if (pm.Rows.Count > 0)
                        {
                            if (Convert.ToString(pm.Rows[0]["Status_ForCustomer"]) == "0")
                            {
                                dt.Rows.Add(1, "Invalid Document", obj.SenderID_ID);
                                return dt;
                            }
                        }
                    }




                    //Return if document is expired
                    if ((obj.SenderID_ExpiryDate != "" && obj.SenderID_ExpiryDate != null) || (shuftipro_frontdoc_expired == 0 && API_ID == 5))
                    {
                        try
                        {
                            SID_Expiry = DateTime.ParseExact(obj.SenderID_ExpiryDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                            if (SID_Expiry.Date <= Convert.ToDateTime(obj.Record_Insert_DateTime).Date)
                            {
                                dt.Rows.Add(2, "Expired Document", obj.SenderID_ID);
                                return dt;
                            }
                            else if (DateTime.TryParseExact(obj.SenderID_ExpiryDate, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out SID_Expiry))

                            {
                                if (SID_Expiry.Date <= Convert.ToDateTime(obj.Record_Insert_DateTime).Date)
                                {
                                    dt.Rows.Add(2, "Expired Document", obj.SenderID_ID);
                                    return dt;
                                }
                                // Parsing failed, handle the case where the format is incorrect
                                // or contains time information
                                // For example, if the format is "dd-MM-yyyy HH:mm:ss", you could parse it like this:

                                //obj.Sender_DateOfBirth = SID_bdate.ToString("yyyy-MM-dd");

                            }
                        }
                        catch { }
                    }

                    //To save invalid document
                    if (obj.SenderNameOnID == "Unknown" || obj.SenderNameOnID == null || obj.SenderNameOnID == "")
                    {
                        obj.SenderNameOnID = (cname + " " + mname + " " + lname).Trim();
                    }

                    string Username = obj.CustomerName;

                    string url = Convert.ToString(ob["WhiteImageUrl"]);

                    if (API_ID == 5)
                    {
                        url = Convert.ToString(obj.shuftipro_frontdoc);
                    }
                    if (API_ID == 7)
                    {
                        url = Convert.ToString(veriffFrontDocURL);
                    }
                    if (API_ID == 9)
                    {
                        url = "Sumsub docs save";
                    }

                    if ((url != "" && url != null))
                    {

                        string path = "P-" + Customer_ID + "-" + obj.Record_Insert_DateTime.Replace(":", "") + ".jpg";
                        obj.FileNameWithExt = "assets/Uploads/" + path;//
                        if (API_ID == 9)
                        {
                            byte[] imageData;
                            string contentType = "";
                            string fileExtension = "";
                            var stamp = ((long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds).ToString();
                            string userId = obj.Custmer_Ref;
                            var applicant_id = obj.applicant_id;
                            // Calculate the value to sign (excluding the body)
                            var valueToSign = stamp + "GET" + $"/resources/applicants/{applicant_id}/requiredIdDocsStatus";


                            string appToken = UserName;
                            // Replace placeholders with actual values
                            valueToSign = valueToSign.Replace("{{sumsub_root_url}}", "")
                                                     .Replace("{{applicantId}}", "")
                                                     .Replace("{{levelName}}", "basic-kyc-level")
                                                     .Replace("{{userId}}", userId);

                            // Calculate the signature
                            var secretKey = Password; // Replace with your secret key
                            var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey));
                            var signatureBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(valueToSign));
                            var signature = BitConverter.ToString(signatureBytes).Replace("-", "").ToLower();

                            // Set your dynamic userId here
                            var client1 = new RestClient(urll);
                            var request1 = new RestRequest($"/resources/applicants/{applicant_id}/requiredIdDocsStatus", Method.GET);
                            request1.AddHeader("Content-Type", "application/json");
                            request1.AddHeader("X-App-Token", appToken);
                            request1.AddHeader("X-App-Access-Ts", stamp);
                            request1.AddHeader("X-App-Access-Sig", signature);
                            IRestResponse responseT = client1.Execute(request1);
                            JObject jsonResponse = JObject.Parse(responseT.Content);



                            // Parse the JSON response
                            //JObject jsonResponse = JObject.Parse(responseT.Content);

                            // Access the values from the response
                            JToken identity = jsonResponse["IDENTITY"];
                            string identityReviewAnswer = (string)identity["reviewResult"]["reviewAnswer"];
                            string identityCountry = (string)identity["country"];
                            string identityIdDocType = (string)identity["idDocType"];
                            //identityImageIds = (JArray)identity["imageIds"];
                            JToken identityImageReviewResults = identity["imageReviewResults"];
                            bool identityForbidden = (bool)identity["forbidden"];

                            JToken selfie = jsonResponse["SELFIE"];
                            string selfieReviewAnswer = (string)selfie["reviewResult"]["reviewAnswer"];
                            string selfieCountry = (string)selfie["country"];
                            string selfieIdDocType = (string)selfie["idDocType"];
                            //selfieImageIds = (JArray)selfie["imageIds"];
                            JToken selfieImageReviewResults = selfie["imageReviewResults"];
                            bool selfieForbidden = (bool)selfie["forbidden"];


                            identityImageIds = (JArray)jsonResponse["IDENTITY"]["imageIds"];
                            selfieImageIds = (JArray)jsonResponse["SELFIE"]["imageIds"];


                            if (identityImageIds.Count > 0)
                            {
                                // If you expect exactly two values
                                if (identityImageIds.Count >= 2)
                                {
                                    int imageId1 = identityImageIds[0].ToObject<int>();
                                    int imageId2 = identityImageIds[1].ToObject<int>();




                                    stamp = ((long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds).ToString();
                                    userId = obj.Custmer_Ref;
                                    applicant_id = obj.applicant_id;
                                    //var inspectionId = "65d71d4e08ca10763627b720"; 
                                    // Calculate the value to sign (excluding the body)
                                    valueToSign = stamp + "GET" + $"/resources/inspections/{inspectionId}/resources/{imageId2}";


                                    appToken = UserName;
                                    // Replace placeholders with actual values
                                    valueToSign = valueToSign.Replace("{{sumsub_root_url}}", "")
                                                             .Replace("{{applicantId}}", "")
                                                             .Replace("{{levelName}}", "basic-kyc-level")
                                                             .Replace("{{userId}}", userId);

                                    // Calculate the signature
                                    secretKey = Password; // Replace with your secret key
                                    hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey));
                                    signatureBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(valueToSign));
                                    signature = BitConverter.ToString(signatureBytes).Replace("-", "").ToLower();

                                    // Set your dynamic userId here
                                    client1 = new RestClient(urll);
                                    request1 = new RestRequest($"/resources/inspections/{inspectionId}/resources/{imageId2}", Method.GET);
                                    request1.AddHeader("Content-Type", "application/json");
                                    request1.AddHeader("X-App-Token", appToken);
                                    request1.AddHeader("X-App-Access-Ts", stamp);
                                    request1.AddHeader("X-App-Access-Sig", signature);
                                    responseT = client1.Execute(request1);
                                    //jsonResponse = JObject.Parse(responseT.Content);
                                    // Determine the file extension based on the Content-Type header
                                    contentType = responseT.ContentType;
                                    fileExtension = GetFileExtension(contentType);



                                    if (fileExtension == "")
                                    {
                                        fileExtension = ".jpg";
                                    }
                                    path = "P-" + Customer_ID + "-" + imageId2 + obj.Record_Insert_DateTime.Replace(":", "") + fileExtension;
                                    obj.FileNameWithExt = "assets/Uploads/" + path;

                                    back_idpath = obj.FileNameWithExt;
                                    // Parse the JSON response
                                    //jsonResponse = JObject.Parse(responseT.Content);

                                    // Extract the binary content
                                    imageData = responseT.RawBytes;

                                    //// Determine the file extension based on the Content-Type header
                                    //contentType = responseT.ContentType;
                                    //fileExtension = GetFileExtension(contentType);




                                    // Save the image to a file
                                    //string filePath = $"path/to/save/image.{fileExtension}";
                                    //File.WriteAllBytes(obj.FileNameWithExt, imageData);


                                    File.WriteAllBytes(Convert.ToString(dtc.Rows[0]["RootURL"]) + obj.FileNameWithExt + "", imageData);



                                    stamp = ((long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds).ToString();
                                    userId = obj.Custmer_Ref;
                                    applicant_id = obj.applicant_id;
                                    //var inspectionId = "65d71d4e08ca10763627b720"; 
                                    // Calculate the value to sign (excluding the body)
                                    valueToSign = stamp + "GET" + $"/resources/inspections/{inspectionId}/resources/{imageId1}";


                                    appToken = UserName;
                                    // Replace placeholders with actual values
                                    valueToSign = valueToSign.Replace("{{sumsub_root_url}}", "")
                                                             .Replace("{{applicantId}}", "")
                                                             .Replace("{{levelName}}", "basic-kyc-level")
                                                             .Replace("{{userId}}", userId);

                                    // Calculate the signature
                                    secretKey = Password; // Replace with your secret key
                                    hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey));
                                    signatureBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(valueToSign));
                                    signature = BitConverter.ToString(signatureBytes).Replace("-", "").ToLower();

                                    // Set your dynamic userId here
                                    client1 = new RestClient(urll);
                                    request1 = new RestRequest($"/resources/inspections/{inspectionId}/resources/{imageId1}", Method.GET);
                                    request1.AddHeader("Content-Type", "application/json");
                                    request1.AddHeader("X-App-Token", appToken);
                                    request1.AddHeader("X-App-Access-Ts", stamp);
                                    request1.AddHeader("X-App-Access-Sig", signature);
                                    responseT = client1.Execute(request1);
                                    //jsonResponse = JObject.Parse(responseT.Content);


                                    // Parse the JSON response
                                    //jsonResponse = JObject.Parse(responseT.Content);

                                    // Extract the binary content
                                    imageData = responseT.RawBytes;

                                    // Determine the file extension based on the Content-Type header
                                    contentType = responseT.ContentType;
                                    fileExtension = GetFileExtension(contentType);
                                    contentType = responseT.ContentType;
                                    fileExtension = GetFileExtension(contentType);

                                    if (fileExtension == "")
                                    {
                                        fileExtension = ".jpg";
                                    }
                                    path = "P-" + Customer_ID + "-" + imageId1 + obj.Record_Insert_DateTime.Replace(":", "") + fileExtension;
                                    obj.FileNameWithExt = "assets/Uploads/" + path;



                                    // Save the image to a file
                                    //string filePath = $"path/to/save/image.{fileExtension}";
                                    //File.WriteAllBytes(obj.FileNameWithExt, imageData);
                                    File.WriteAllBytes(Convert.ToString(dtc.Rows[0]["RootURL"]) + obj.FileNameWithExt + "", imageData);

                                }
                                else
                                {
                                    identityImageIds = (JArray)jsonResponse["IDENTITY"]["imageIds"];
                                    string identityImageIdsString = string.Join(",", identityImageIds.Select(id => id.ToString()));
                                    stamp = ((long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds).ToString();
                                    userId = obj.Custmer_Ref;
                                    applicant_id = obj.applicant_id;
                                    //var inspectionId = "65d71d4e08ca10763627b720"; 
                                    // Calculate the value to sign (excluding the body)
                                    valueToSign = stamp + "GET" + $"/resources/inspections/{inspectionId}/resources/{identityImageIdsString}";


                                    appToken = UserName;
                                    // Replace placeholders with actual values
                                    valueToSign = valueToSign.Replace("{{sumsub_root_url}}", "")
                                                             .Replace("{{applicantId}}", "")
                                                             .Replace("{{levelName}}", "basic-kyc-level")
                                                             .Replace("{{userId}}", userId);

                                    // Calculate the signature
                                    secretKey = Password; // Replace with your secret key
                                    hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey));
                                    signatureBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(valueToSign));
                                    signature = BitConverter.ToString(signatureBytes).Replace("-", "").ToLower();

                                    // Set your dynamic userId here
                                    client1 = new RestClient(urll);
                                    request1 = new RestRequest($"/resources/inspections/{inspectionId}/resources/{identityImageIdsString}", Method.GET);
                                    request1.AddHeader("Content-Type", "application/json");
                                    request1.AddHeader("X-App-Token", appToken);
                                    request1.AddHeader("X-App-Access-Ts", stamp);
                                    request1.AddHeader("X-App-Access-Sig", signature);
                                    responseT = client1.Execute(request1);
                                    //jsonResponse = JObject.Parse(responseT.Content);
                                    contentType = responseT.ContentType;
                                    fileExtension = GetFileExtension(contentType);

                                    if (fileExtension == "")
                                    {
                                        fileExtension = ".jpg";
                                    }
                                    path = "P-" + Customer_ID + "-" + identityImageIdsString + obj.Record_Insert_DateTime.Replace(":", "") + fileExtension;
                                    obj.FileNameWithExt = "assets/Uploads/" + path;

                                    // Parse the JSON response
                                    //jsonResponse = JObject.Parse(responseT.Content);

                                    // Extract the binary content
                                    imageData = responseT.RawBytes;

                                    // Determine the file extension based on the Content-Type header





                                    // Save the image to a file
                                    //string filePath = $"path/to/save/image.{fileExtension}";
                                    //File.WriteAllBytes(obj.FileNameWithExt, imageData);
                                    File.WriteAllBytes(Convert.ToString(dtc.Rows[0]["RootURL"]) + obj.FileNameWithExt + "", imageData);
                                }
                            }







                            // Convert the JArray to comma-separated strings
                            //string identityImageIdsString = string.Join(",", identityImageIds.Select(id => id.ToString()));
                            //string selfieImageIdsString = string.Join(",", selfieImageIds.Select(id => id.ToString()));

                            // Now you can use these values in your code as needed





                            string idDocValidUntilString = idDocValidUntil.ToString();
                            obj.SenderID_ExpiryDate = idDocValidUntilString;

                            // obj.Issue_date=
                            //CompanyInfo.InsertActivityLogDetails("Get The Sumsub Docs img   and applicantID 9 for Id scan " + obj.FileNameWithExt + applicant_id + ". ", Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(Customer_ID), "GetJourney", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "ID Upload");

                            activity_log = "Get The Sumsub Docs img   and applicantID 9 for Id scan " + obj.FileNameWithExt + applicant_id + "";


                            string idDocidDocDobString = idDocDob.ToString();
                            //obj.Sender_DateOfBirth = idDocidDocDobString;
                        }
                        if (API_ID == 14)
                        {
                            try
                            {
                                var session = GetVerificationSummary(Yoti_SessionId, API_ID, obj.Client_ID);

                                int counter = 1;  // To number your image files

                                foreach (var page in session.Document.Pages)
                                {
                                    var pageCaptureMethod = page.CaptureMethod;
                                    var pageMediaId = page.MediaId;
                                    var pageBase64Image = Convert.ToString(page.Base64Image)?.Trim();

                                    if (counter == 1)
                                    {
                                        path = "Pfront-" + Customer_ID + "-" + obj.Record_Insert_DateTime.Replace(":", "") + ".jpg";
                                        obj.FileNameWithExt = "assets/Uploads/" + path;//
                                        if (!string.IsNullOrEmpty(pageBase64Image))
                                        {
                                            // Remove base64 header if present
                                            if (pageBase64Image.StartsWith("data:image", StringComparison.OrdinalIgnoreCase))
                                            {
                                                int commaIndex = pageBase64Image.IndexOf(',');
                                                if (commaIndex > -1)
                                                {
                                                    pageBase64Image = pageBase64Image.Substring(commaIndex + 1);
                                                }
                                            }

                                            try
                                            {
                                                byte[] imageData = Convert.FromBase64String(pageBase64Image);

                                                string fileName = $"Page_{counter}.png";
                                                string rootPath = "D://images";  // Change to your folder
                                                string fullPath = Path.Combine(rootPath, fileName);

                                                File.WriteAllBytes(Convert.ToString(dtc.Rows[0]["RootURL"]) + obj.FileNameWithExt + "", imageData);
                                                counter++;
                                            }
                                            catch (FormatException fe)
                                            {
                                                Console.WriteLine($"Invalid Base64 for Page {counter}: " + fe.Message);
                                            }
                                        }
                                    }
                                    else if (counter == 2)
                                    {
                                        path = "Pback-" + Customer_ID + "-" + obj.Record_Insert_DateTime.Replace(":", "") + ".jpg";
                                        back_idpath = "assets/Uploads/" + path;//
                                        if (!string.IsNullOrEmpty(pageBase64Image))
                                        {
                                            // Remove base64 header if present
                                            if (pageBase64Image.StartsWith("data:image", StringComparison.OrdinalIgnoreCase))
                                            {
                                                int commaIndex = pageBase64Image.IndexOf(',');
                                                if (commaIndex > -1)
                                                {
                                                    pageBase64Image = pageBase64Image.Substring(commaIndex + 1);
                                                }
                                            }

                                            try
                                            {
                                                byte[] imageData = Convert.FromBase64String(pageBase64Image);


                                                File.WriteAllBytes(Convert.ToString(dtc.Rows[0]["RootURL"]) + obj.FileNameWithExt + "", imageData);
                                                counter++;
                                            }
                                            catch (FormatException fe)
                                            {
                                                Console.WriteLine($"Invalid Base64 for Page {counter}: " + fe.Message);
                                            }
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("Error saving images: " + ex.Message);
                            }
                        }
                        else
                        {
                            using (var webClient = new WebClient())
                            {
                                try
                                {
                                    webClient.Headers.Add("Content-Type", "application/json"); webClient.Headers.Add("Authorization", s13); webClient.Headers.Add("Cookie", "token " + s13);
                                    byte[] imageBytes = webClient.DownloadData(url);
                                    File.WriteAllBytes(Convert.ToString(dtc.Rows[0]["RootURL"]) + obj.FileNameWithExt + "", imageBytes);
                                }
                                catch { }
                            }
                        }
                        if (BackSideWhiteImageUrl != "")
                        {
                            try
                            {
                                path = "Pback-" + obj.Customer_ID + "-" + obj.Record_Insert_DateTime.Replace(":", "") + ".jpg";
                                back_idpath = "assets/Uploads/" + path;//
                                using (var webClient = new WebClient())
                                {
                                    try
                                    {
                                        webClient.Headers.Add("Content-Type", "application/json"); webClient.Headers.Add("Authorization", s13); webClient.Headers.Add("Cookie", "token " + s13);
                                        byte[] imageBytes = webClient.DownloadData(BackSideWhiteImageUrl);
                                        File.WriteAllBytes(Convert.ToString(dtc.Rows[0]["RootURL"]) + back_idpath + "", imageBytes);
                                        CompanyInfo.InsertActivityLogDetails("Customers Back ID Document Resubmitted At " + Convert.ToString(dtc.Rows[0]["RootURL"]) + obj.FileNameWithExt + ". ", Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(obj.Customer_ID), "GetJourny", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "", context);

                                    }
                                    catch { }
                                }

                            }
                            catch { }
                        }
                        if (obj.SenderID_ExpiryDate != "" && obj.SenderID_ExpiryDate != null)
                        {
                            try
                            {
                                if (API_ID == 9)
                                {
                                    obj.SenderID_ExpiryDate = idDocValidUntildate;
                                }
                                if (API_ID == 14)
                                {
                                    obj.SenderID_ExpiryDate = documentExpirationDateStr;
                                }

                                SID_Expiry = DateTime.ParseExact(obj.SenderID_ExpiryDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                                obj.SenderID_ExpiryDate = SID_Expiry.ToString("yyyy-MM-dd");
                                CompanyInfo.InsertActivityLogDetails("Inside the GBG part of sender expiry " + obj.SenderID_ExpiryDate + ". ", Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(Customer_ID), "GetJourney", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "ID Upload", context);

                            }
                            catch { }
                            finally
                            {
                                //CompanyInfo.InsertActivityLogDetails("Inside the finally part of sender expiry " + obj.SenderID_ExpiryDate + ". ", Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(Customer_ID), "GetJourney", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "ID Upload");
                            }
                        }
                        else
                        {
                            DateTime date = DateTime.Now.AddMonths(3);
                            obj.SenderID_ExpiryDate = date.ToString("yyyy-MM-dd");
                        }

                        if (obj.Issue_date != "" && obj.Issue_date != null)
                        {
                            try
                            {
                                DateTime SID_Issuedate;
                                if (API_ID == 9 && DateTime.TryParseExact(obj.Issue_date, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out SID_Issuedate))
                                {
                                    // Parsing failed, handle the case where the format is incorrect
                                    // or contains time information
                                    // For example, if the format is "dd-MM-yyyy HH:mm:ss", you could parse it like this:
                                    //SID_Issuedate = DateTime.ParseExact(obj.Issue_date, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                                    obj.Issue_date = SID_Issuedate.ToString("yyyy-MM-dd");

                                }
                                else
                                {
                                    SID_Issuedate = DateTime.ParseExact(obj.Issue_date, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                                    obj.Issue_date = SID_Issuedate.ToString("yyyy-MM-dd");
                                }
                            }
                            catch { }
                        }
                        else
                        {
                            DateTime date = DateTime.Now;
                            obj.Issue_date = date.ToString("yyyy-MM-dd");
                        }
                        if (obj.Sender_DateOfBirth != "" && obj.Sender_DateOfBirth != null)
                        {
                            try
                            {
                                SID_bdate = DateTime.ParseExact(obj.Sender_DateOfBirth, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                                obj.Sender_DateOfBirth = SID_bdate.ToString("yyyy-MM-dd");
                            }
                            catch { }
                        }
                        else
                        {
                            //DateTime bdt = DateTime.Now.AddYears(-18);
                            //obj.Sender_DateOfBirth = bdt.ToString("yyyy-MM-dd");
                            obj.Sender_DateOfBirth = string.Empty;
                        }
                        if (API_ID == 9)
                        {
                            if (DocumentType == "DRIVERS")
                            {
                                DocumentType = "Driving License";
                            }
                        }

                        if (DocumentType == "" && API_ID == 3)
                        {
                            DocumentType = obj.DocumentName;
                        }
                        if (API_ID == 14)
                        {
                            DocumentType = documentTypeStr;
                        }
                        MySqlCommand _cmd1 = new MySqlCommand("GetIDNames");
                        _cmd1.CommandType = CommandType.StoredProcedure;
                        _cmd1.Parameters.AddWithValue("Client_ID", obj.Client_ID);
                        _cmd1.Parameters.AddWithValue("clause", " and im.IDType_ID=" + obj.IDType_ID + " and ID_Name like '%" + DocumentType + "%'");
                        DataTable d = db_connection.ExecuteQueryDataTableProcedure(_cmd1);
                        int stattus12 = (int)CompanyInfo.InsertActivityLogDetails("After GetIDNames valuesL: obj.Client_ID=" + obj.Client_ID + "clause" + " and im.IDType_ID=" + obj.IDType_ID + " and ID_Name like '%" + DocumentType + "%'", Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(Customer_ID), "GetJourney", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "ID Upload", context);

                        obj.IDName_ID = 1;
                        if (d.Rows.Count > 0)
                        {
                            string idname = Convert.ToString(d.Rows[0]["IDName_ID"]);
                            stattus12 = (int)CompanyInfo.InsertActivityLogDetails("After GetIDNames values: idname=" + idname, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(Customer_ID), "GetJourney", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "ID Upload", context);

                            if (idname != null && idname != "" && idname != "0")
                                obj.IDName_ID = Convert.ToInt32(idname);
                        }
                        stattus12 = (int)CompanyInfo.InsertActivityLogDetails("After GetIDNames values: idname2=" + obj.IDName_ID, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(Customer_ID), "GetJourney", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "ID Upload", context);

                        string IDScanpdf = string.Empty;
                        int pdfdownload = 1;
                        try
                        {
                            if (API_ID == 3)
                            {
                                IDScanpdf = "assets/Other_Docs/" + "PDF-" + Customer_ID + "-" + obj.Record_Insert_DateTime.Replace(":", "") + ".zip";//Convert.ToString(dtc.Rows[0]["RootURL"]) assets/Upload_Profile_Images/
                                using (var webClient = new WebClient())
                                {
                                    webClient.Headers.Add("Content-Type", "application/json"); webClient.Headers.Add("Authorization", s13); webClient.Headers.Add("Cookie", "token " + s13);
                                    byte[] imageBytes = webClient.DownloadData("" + urll + "reporting/ExportJourneyReports?evaluatedPersonEntryIds=['" + journeyID + "']");
                                    //File.WriteAllBytes(Convert.ToString(dtc.Rows[0]["RootURL"]) + IDScanpdf, imageBytes);
                                    FileStream fs = new FileStream(@"" + Convert.ToString(dtc.Rows[0]["RootURL"]) + "" + IDScanpdf + "", FileMode.OpenOrCreate);
                                    fs.Write(imageBytes, 0, imageBytes.Length);
                                    fs.Close();

                                    //using (var compressedStream = new MemoryStream(imageBytes))
                                    //using (var zipStream = new System.IO.Compression.GZipStream(compressedStream, System.IO.Compression.CompressionMode.Decompress))
                                    //using (var resultStream = new MemoryStream())
                                    //{
                                    //    zipStream.CopyTo(resultStream);
                                    //    var x = resultStream.ToArray();
                                    //}
                                    //byte[] decompressedBytes = new byte[imageBytes.Length];
                                    //using (FileStream fileToDecompress = File.Open("" + Convert.ToString(dtc.Rows[0]["RootURL"]) + "" + IDScanpdf + "", FileMode.Open))
                                    //{
                                    //    using (System.IO.Compression.GZipStream decompressionStream = new System.IO.Compression.GZipStream(fileToDecompress, System.IO.Compression.CompressionMode.Decompress))
                                    //    {
                                    //        decompressionStream.Read(decompressedBytes, 0, imageBytes.Length);
                                    //    }
                                    //}

                                    pdfdownload = 0;
                                    string Activity = "Generated GBG ID scan PDF for <b>" + Username + "</b>";
                                    int stattus = (int)CompanyInfo.InsertActivityLogDetails(Activity, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(Customer_ID), "GetJourney", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "ID Upload", context);

                                }
                            }

                            if (API_ID == 9)
                            {


                                IDScanpdf = "assets/Other_Docs/" + "PDF-" + Customer_ID + "-" + obj.Record_Insert_DateTime.Replace(":", "") + ".pdf";

                                var stamp = ((long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds).ToString();


                                var userId = obj.Custmer_Ref;
                                var applicant_id = obj.applicant_id;

                                // Calculate the value to sign (excluding the body)
                                var valueToSign = stamp + "GET" + $"/resources/applicants/{applicant_id}/summary/report?reportType=applicantReport&lang=en";

                                string appToken = UserName;
                                // Replace placeholders with actual values
                                valueToSign = valueToSign.Replace("{{sumsub_root_url}}", "")
                                                         .Replace("{{applicantId}}", "")
                                                         .Replace("{{levelName}}", "basic-kyc-level")
                                                         .Replace("{{userId}}", userId);

                                // Calculate the signature
                                var secretKey = Password; // Replace with your secret key
                                var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey));
                                var signatureBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(valueToSign));
                                var signature = BitConverter.ToString(signatureBytes).Replace("-", "").ToLower();

                                var client1 = new RestClient(urll);
                                var request1 = new RestRequest($"/resources/applicants/{applicant_id}/summary/report?reportType=applicantReport&lang=en", Method.GET);
                                request1.AddHeader("Content-Type", "application/json");
                                request1.AddHeader("Accept", "application/pdf"); // Force accept PDF response
                                request1.AddHeader("X-App-Token", appToken);
                                request1.AddHeader("X-App-Access-Ts", stamp);
                                request1.AddHeader("X-App-Access-Sig", signature);
                                IRestResponse responseT = client1.Execute(request1);

                                if (responseT.StatusCode == HttpStatusCode.OK)
                                {
                                    byte[] pdfData = responseT.RawBytes;

                                    // Write the PDF data to a file
                                    File.WriteAllBytes(Convert.ToString(dtc.Rows[0]["RootURL"]) + IDScanpdf + "", pdfData);
                                    Console.WriteLine("PDF file downloaded successfully.");
                                    //CompanyInfo.InsertActivityLogDetails("Get The Sumsub PDF and applicantID 10 for Id scan " +IDScanpdf + applicant_id + ". ", Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(Customer_ID), "GetJourney", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "ID Upload");
                                    activity_log = "Get The Sumsub PDF and applicantID 10 for Id scan " + IDScanpdf + applicant_id + "";
                                }




                                pdfdownload = 0;

                                obj.SenderID_Number = idDocNumber;

                                obj.SenderID_PlaceOfIssue = idDocCountry;

                                obj.DocumentName = DocumentType;

                            }
                        }
                        catch { }
                        if (API_ID == 9)
                        {
                            obj.Comments = "Sumsub_Id_Scan";
                        }
                        else
                        {
                            obj.Comments = "ID Scan";
                        }


                        using (MySqlConnector.MySqlConnection con = new MySqlConnector.MySqlConnection(_dbConnection.ConnectionStringSection()))
                        {
                            if (con.State != ConnectionState.Open)
                                con.Open();

                            MySqlConnector.MySqlTransaction transaction;
                            transaction = con.BeginTransaction(IsolationLevel.ReadCommitted);

                            using (MySqlConnector.MySqlCommand cmd = new MySqlConnector.MySqlCommand("Customer_InsertPID", con))
                            {
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Transaction = transaction;
                                cmd.CommandTimeout = 200;

                                // This code for INPROGRESS
                                if (DateTime.TryParseExact(obj.SenderID_ExpiryDate, "MM-yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime date_n))
                                {
                                    obj.SenderID_ExpiryDate = date_n.ToString("yyyy-MM-dd");
                                }
                                if (DateTime.TryParseExact(obj.Issue_date, "MM-yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime date_n1))
                                {
                                    obj.Issue_date = date_n1.ToString("yyyy-MM-dd");
                                }
                                if (DateTime.TryParse(obj.SenderID_ExpiryDate, out DateTime Temp) == false)
                                {
                                    DateTime date = DateTime.Now.AddMonths(3);
                                    obj.SenderID_ExpiryDate = date.ToString("yyyy-MM-dd");
                                }
                                if (DateTime.TryParse(obj.Issue_date, out DateTime Temp2) == false)
                                {
                                    DateTime date = DateTime.Now;
                                    obj.Issue_date = date.ToString("yyyy-MM-dd");
                                }
                                if (DateTime.TryParse(obj.Sender_DateOfBirth, out DateTime Temp3) == false)
                                {
                                    obj.Sender_DateOfBirth = string.Empty;
                                }

                                cmd.Parameters.AddWithValue("_SenderNameOnID", obj.SenderNameOnID);
                                cmd.Parameters.AddWithValue("_IDType_ID", obj.IDType_ID);
                                cmd.Parameters.AddWithValue("_SenderID_Number", obj.SenderID_Number);
                                cmd.Parameters.AddWithValue("_SenderID_ExpiryDate", obj.SenderID_ExpiryDate);
                                cmd.Parameters.AddWithValue("_SenderID_PlaceOfIssue", obj.SenderID_PlaceOfIssue);
                                if (obj.Sender_DateOfBirth == null || obj.Sender_DateOfBirth == "")
                                    cmd.Parameters.Add("_Sender_DateOfBirth", (DbType)SqlDbType.DateTime).Value = DBNull.Value;
                                else
                                    cmd.Parameters.AddWithValue("_Sender_DateOfBirth", obj.Sender_DateOfBirth);
                                cmd.Parameters.AddWithValue("_Customer_ID", Customer_ID);
                                cmd.Parameters.AddWithValue("_IDName_ID", obj.IDName_ID);
                                cmd.Parameters.AddWithValue("_Record_Insert_DateTime", obj.Record_Insert_DateTime);
                                cmd.Parameters.AddWithValue("_Delete_Status", obj.Delete_Status);
                                cmd.Parameters.AddWithValue("_User_ID", obj.User_ID);
                                cmd.Parameters.AddWithValue("_documents_details_Id", obj.IDType_ID);
                                cmd.Parameters.AddWithValue("_CB_ID", obj.Branch_ID);
                                cmd.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                                cmd.Parameters.AddWithValue("_Doc_Name", obj.DocumentName);
                                cmd.Parameters.AddWithValue("_comments", obj.Comments);
                                cmd.Parameters.AddWithValue("_FileNameWithExt", obj.FileNameWithExt);
                                cmd.Parameters.AddWithValue("_Benf_ID", obj.Beneficiary_ID);
                                cmd.Parameters.AddWithValue("_issue_date", obj.Issue_date);
                                cmd.Parameters.AddWithValue("_BackID_Document", back_idpath);
                                cmd.Parameters.AddWithValue("_JourneyID", journeyID.ToString());
                                cmd.Parameters.AddWithValue("_PDFGenerate_Status", pdfdownload);//default 1 - means not yet downloaded
                                cmd.Parameters.AddWithValue("_PDF_FileName", IDScanpdf);

                                cmd.Parameters.AddWithValue("_FrontResult", frontside);
                                cmd.Parameters.AddWithValue("_LivenessResult", liveness);
                                cmd.Parameters.AddWithValue("_FaceMatchResult", facematch);
                                cmd.Parameters.AddWithValue("_MRZ_number", obj.MRZ_number);
                                cmd.Parameters.AddWithValue("_MRZ_number2", idDocMrzLine2);

                                cmd.Parameters.Add(new MySqlConnector.MySqlParameter("_SenderID_ID", MySqlConnector.MySqlDbType.Int32));
                                cmd.Parameters["_SenderID_ID"].Direction = ParameterDirection.Output;

                                obj.SenderID_ID = 0;
                                object result = cmd.ExecuteNonQuery();
                                try
                                {
                                    obj.SenderID_ID = Convert.ToInt32(cmd.Parameters["_SenderID_ID"].Value);
                                }
                                catch (Exception egx) { }
                                result = (result == DBNull.Value) ? null : result;
                                //        obj.SenderID_ID = Convert.ToInt32(result);
                                int res = Convert.ToInt32(result);
                                dt.Rows.Add(0, "Success", obj.SenderID_ID);
                                cmd.Dispose();
                                string Activity = string.Empty;
                                string idtype = string.Empty;
                                //CompanyInfo.InsertActivityLogDetails("Insert the Sumsub data Customer_InsertPID and applicantID 12 for Id scan " + obj.SenderNameOnID + ". ", Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(Customer_ID), "GetJourney", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "ID Upload");


                                //  context.Session["Username"] = Username;
                                string country_details = "Document Details: ";

                                if (obj.IDType_ID != null && obj.IDType_ID != -1)
                                {
                                    if (obj.IDType_ID == 1 || obj.IDType_ID == 5)
                                    {
                                        idtype = "Primary";
                                    }
                                    if (obj.IDType_ID == 2)
                                    {
                                        idtype = "Secondary";
                                    }
                                    if (obj.IDType_ID == 3)
                                    {
                                        idtype = "Other Documents";
                                    }
                                    if (obj.IDType_ID == 4)
                                    {
                                        idtype = "Source Of Funds";
                                    }
                                    country_details = country_details + " Id Type: " + idtype;
                                }
                                if (obj.SenderNameOnID != null && obj.SenderNameOnID != "")
                                {
                                    country_details = country_details + " Name on ID: " + obj.SenderNameOnID;
                                }
                                if (obj.Sender_DateOfBirth != null && obj.Sender_DateOfBirth != "")
                                {
                                    country_details = country_details + " Date of Birth: " + obj.Sender_DateOfBirth;
                                }
                                if (obj.IDName_ID != null && obj.IDName_ID != -1)
                                {
                                    country_details = country_details + " ID: " + DocumentType;
                                }
                                if (obj.SenderID_Number != null && obj.SenderID_Number != "")
                                {
                                    country_details = country_details + "<br/> ID Number: " + obj.SenderID_Number;
                                }
                                if (obj.SenderID_ExpiryDate != null && obj.SenderID_ExpiryDate != "")
                                {
                                    country_details = country_details + " Expiry Date: " + obj.SenderID_ExpiryDate;
                                }
                                if (obj.SenderID_PlaceOfIssue != null && obj.SenderID_PlaceOfIssue != "")
                                {
                                    country_details = country_details + " Place of Issue: " + obj.SenderID_PlaceOfIssue;
                                }

                                if (res > 0 && obj.scanstatus != "INPROGRESS")
                                {
                                    string risk_factors = "2,4,5,6,7,8,10,13";
                                    CompanyInfo.add_check_rules(Customer_ID, risk_factors, obj.Branch_ID, obj.Client_ID, context);

                                    string notification_icon = "", notification_message = "";
                                    if (obj.IDType_ID == 1 || obj.IDType_ID == 5)
                                    {
                                        notification_icon = "primary-id-upload.jpg";
                                        notification_message = "<span class='cls-admin'>successfully uploaded <strong class='cls-priamary'>Primary ID</strong>.</span>";
                                    }
                                    if (obj.IDType_ID == 2)
                                    {
                                        notification_icon = "secondary-id-upload.jpg";
                                        notification_message = "<span class='cls-admin'>successfully uploaded <strong class='cls-secondary'>Secondary ID</strong>.</span>";
                                    }
                                    if (obj.IDType_ID == 3)
                                    {
                                        notification_icon = "other-doc.jpg";
                                        notification_message = "<span class='cls-admin'>successfully uploaded <strong class='cls-otherdoc'>Other document</strong>.</span>";
                                    }
                                    if (obj.IDType_ID == 4)
                                    {
                                        notification_icon = "sof.jpg";
                                        notification_message = "<span class='cls-admin'>successfully uploaded <strong class='cls-srcfund'>Source of fund document</strong>.</span>";
                                    }
                                    CompanyInfo.save_notification(notification_message, notification_icon, Customer_ID, Convert.ToDateTime(obj.Record_Insert_DateTime), obj.Client_ID, 1, 0, obj.Branch_ID, 0, 1, 1, 0, context);

                                    Activity = "<b>" + Username + "</b> scanned ID and checked Liveness. ID Document Details: " + country_details + "</br>";
                                    int stattus = (int)CompanyInfo.InsertActivityLogDetails(Activity, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(Customer_ID), "GetJourney", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "ID Upload", context);

                                    MySqlCommand cmd3 = new MySqlCommand("customer_details_by_param");
                                    cmd3.CommandType = CommandType.StoredProcedure;
                                    string _whereclause = " and cr.Client_ID=" + obj.Client_ID;
                                    if (Customer_ID > 0)
                                    {
                                        _whereclause = " and cr.Customer_ID=" + Customer_ID;
                                    }
                                    cmd3.Parameters.AddWithValue("_whereclause", _whereclause);
                                    cmd3.Parameters.AddWithValue("_SecurityKey", CompanyInfo.SecurityKey());
                                    DataTable dt_cust = db_connection.ExecuteQueryDataTableProcedure(cmd3);


                                    string[] response24 = CheckCreditSafeByName(obj, dt_cust, "ID Upload", context);

                                    string checkatthetimeofidupload = "";
                                    string checkatthetimeoftransaction = "";


                                    MySqlCommand cmd4 = new MySqlCommand("active_thirdparti_aml_api");
                                    cmd4.CommandType = CommandType.StoredProcedure;
                                    _whereclause = " Client_ID= 1";
                                    cmd4.Parameters.AddWithValue("_whereclause", _whereclause);
                                    DataTable dtApi_activeAML = db_connection.ExecuteQueryDataTableProcedure(cmd4);
                                    stattus12 = (int)CompanyInfo.InsertActivityLogDetails("after active_thirdparti_aml_api " + _whereclause, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(Customer_ID), "GetJourney", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "ID Upload",context);


                                    try
                                    {

                                        foreach (DataRow row in dtApi_activeAML.Rows)
                                        {
                                            string api_fields = Convert.ToString(row["API_Fields"]);
                                            int appi_idd = Convert.ToInt32(row["API_ID"]);

                                            if (api_fields != "")
                                            {
                                                Newtonsoft.Json.Linq.JObject objf = Newtonsoft.Json.Linq.JObject.Parse(api_fields);
                                                checkatthetimeofidupload = Convert.ToString(objf["checkatthetimeofidupload"]);
                                                checkatthetimeoftransaction = Convert.ToString(objf["checkatthetimeoftransaction"]);
                                            }
                                            if (appi_idd == 1)
                                            {
                                                if (checkatthetimeofidupload == "1")
                                                {
                                                    stattus12 = (int)CompanyInfo.InsertActivityLogDetails("Inside check credit safe  " + _whereclause, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(Customer_ID), "GetJourney", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "ID Upload",context);

                                                    string[] response10 = CheckCreditSafe(obj, dt_cust, "ID Upload",context);
                                                }
                                            }
                                            if (appi_idd == 10)
                                            {
                                                if (checkatthetimeofidupload == "1")
                                                {
                                                    stattus12 = (int)CompanyInfo.InsertActivityLogDetails("Inside check aml compliance   " + Customer_ID, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(Customer_ID), "GetJourney", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "ID Upload",context);

                                                    string[] response = CHECK_AML(obj, Customer_ID, "ID Upload",context);

                                                }
                                            }


                                        }
                                    }
                                    catch { }

                                    if (checkatthetimeofidupload == "0")
                                    {
                                        if (IsIdvalid != 1 && API_ID == 3 || API_ID == 9 && Aml_check == 1)
                                    {
                                        //Check Credit Safe
                                        string[] response = CheckCreditSafe(obj, dt_cust, "ID Upload", context);
                                        string scorecount = Convert.ToString(dtc.Rows[0]["FaceScore_ValidFrom"]);
                                        if (scorecount == null || scorecount == "")
                                        {
                                            scorecount = "0";
                                        }
                                        if (response != null && score >= Convert.ToInt32(scorecount) && frontside != 1)
                                        {
                                            if (response.Length == 4)
                                            {
                                                if (response[2] == "0" && response[3] == "0")
                                                {
                                                    //Auto verifiy ID document on perm 125
                                                    MySqlCommand cmdp1 = new MySqlCommand("AutoVerify_IDdoc");
                                                    cmdp1.CommandType = CommandType.StoredProcedure;
                                                    cmdp1.Parameters.AddWithValue("_UserName", "Auto Verified");
                                                    cmdp1.Parameters.AddWithValue("_status", 0);
                                                    cmdp1.Parameters.AddWithValue("_Record_Insert_DateTime", obj.Record_Insert_DateTime);
                                                    cmdp1.Parameters.AddWithValue("_SenderID_ID", obj.SenderID_ID);
                                                    try
                                                    {
                                                        int dr = db_connection.ExecuteNonQueryProcedure(cmdp1);
                                                        if (dr > 0)
                                                        {
                                                            //string Activity1 = "Updated profile image of <b>" + Username + "</b>";
                                                            //int stattus1 = (int)CompanyInfo.InsertActivityLogDetails(Activity1, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(Customer_ID), "GetJourney", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "ID Upload");
                                                            string EmailUrl = "", Company_Name = "";
                                                            if (dtc.Rows.Count > 0)
                                                            {
                                                                EmailUrl = Convert.ToString(dtc.Rows[0]["Company_URL_Admin"]);
                                                                Company_Name = Convert.ToString(dtc.Rows[0]["Company_Name"]);
                                                            }

                                                            string email = Convert.ToString(dt_cust.Rows[0]["Email_ID"]);
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
                                                            var message = "Your account has been fully verified and ready for use.";

                                                            body = body.Replace("[name]", Convert.ToString(dt_cust.Rows[0]["Full_name"]));
                                                            body = body.Replace("[msg]", message);

                                                            subject = "" + Company_Name + " - ID Verification - " + Convert.ToString(dt_cust.Rows[0]["WireTransfer_ReferanceNo"]);

                                                            string mail_send = (string)CompanyInfo.Send_Mail(dtc, email, body, subject, obj.Client_ID, obj.Branch_ID, "", "", "", context);
                                                        }
                                                    }
                                                    catch { }
                                                }
                                            }
                                        }
                                    }
                                    }
                                    string full_name = dt_cust.Rows[0]["Full_name"].ToString();
                                    string email_id = dt_cust.Rows[0]["Email_ID"].ToString();
                                    string referno = Convert.ToString(dt_cust.Rows[0]["WireTransfer_ReferanceNo"]);

                                    if (con.State != ConnectionState.Open)
                                        con.Open();

                                    //MySqlCommand cmdupdate1 = new MySqlCommand("SP_Get_Email_Permission", con);
                                    MySqlConnector.MySqlCommand cmdupdate1 = new MySqlConnector.MySqlCommand("SP_Get_Email_Permission", con);
                                    cmdupdate1.CommandType = CommandType.StoredProcedure;
                                    cmdupdate1.Parameters.AddWithValue("_ID", 2);
                                    cmdupdate1.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                                    string permission_status = string.Empty;
                                    DataTable dt1 = db_connection.ExecuteQueryDataTableProcedure(cmdupdate1);

                                    if (dt1.Rows.Count > 0)
                                    {
                                        permission_status = Convert.ToString(dt1.Rows[0]["Status_ForCustomer"]);
                                    }
                                    if (permission_status == "0")//send mail
                                    {
                                        string get_email_status = send_mails(dtc, obj.Client_ID, obj.Branch_ID, Convert.ToString(dt_cust.Rows[0]["First_Name"]), referno, "Thank you. Your ID Scan and Liveness check is complete.", email_id, context);
                                    }
                                }
                                else
                                {
                                    Activity = "<b>" + Username + "</b>" + " failed to add new ID Document. " + country_details + "</br>";
                                    try
                                    {
                                        string stattus = (string)CompanyInfo.InsertActivityLogDetails(Activity, obj.User_ID, 0, obj.User_ID, Customer_ID, "GetJourney", obj.Branch_ID, obj.Client_ID, "ID Upload", context);
                                    }
                                    catch (Exception egx) { }
                                }
                                transaction.Commit();
                            }
                            if (con.State != ConnectionState.Closed)
                                con.Close();
                        }
                    }

                    if (API_ID == 14)
                    {
                        try
                        {
                            var session = GetVerificationSummary(Yoti_SessionId, API_ID, obj.Client_ID);

                            var watchlistBreakdowns = session.Watchlist.Breakdowns;
                            if (watchlistBreakdowns != null)
                            {
                                foreach (var breakdown in watchlistBreakdowns)
                                {
                                    string subCheck = Convert.ToString(breakdown.SubCheck);
                                    string result = Convert.ToString(breakdown.Result);

                                    if (subCheck == "fitness_probity")
                                    {
                                        fitnessProbityFlag = result == "PASS" ? 0 : 1;
                                    }
                                    else if (subCheck == "pep")
                                    {
                                        pepFlag = result == "PASS" ? 0 : 1;
                                    }
                                    else if (subCheck == "sanction")
                                    {
                                        sanctionFlag = result == "PASS" ? 0 : 1;
                                    }
                                    else if (subCheck == "warning")
                                    {
                                        warningFlag = result == "PASS" ? 0 : 1;
                                    }

                                    if (fitnessProbityFlag > 0 || pepFlag > 0 || sanctionFlag > 0 || warningFlag > 0)
                                    {
                                        int flag = 1;
                                        using (MySqlCommand cmd = new MySqlCommand("update_sanction_flag"))//, cn
                                        {
                                            //if (cn.State != ConnectionState.Open) { cn.Open(); }
                                            cmd.CommandType = CommandType.StoredProcedure;
                                            cmd.Parameters.AddWithValue("_Customer_ID", obj.Customer_ID);
                                            cmd.Parameters.AddWithValue("_record_date", obj.Record_Insert_DateTime);
                                            cmd.Parameters.AddWithValue("_clientId", 1);
                                            cmd.Parameters.AddWithValue("_branchId", obj.Branch_ID);
                                            cmd.Parameters.AddWithValue("_sanction_flag", flag);
                                            int n = db_connection.ExecuteNonQueryProcedure(cmd);

                                        }

                                        using (MySqlCommand cmd = new MySqlCommand("Insert_pep_sanc_detail"))
                                        {

                                            cmd.CommandType = CommandType.StoredProcedure;
                                            cmd.Parameters.AddWithValue("p_SenderID_ID", obj.SenderID_ID);
                                            cmd.Parameters.AddWithValue("p_Flag", 1);
                                            cmd.Parameters.AddWithValue("p_Record_Insert_DateTime", obj.Record_Insert_DateTime.ToString());
                                            cmd.Parameters.AddWithValue("p_Customer_ID", obj.Customer_ID);
                                            cmd.Parameters.AddWithValue("p_API_ID", 14);
                                            int n = db_connection.ExecuteNonQueryProcedure(cmd);

                                        }


                                        try
                                        {
                                            MySqlCommand cmd_update = new MySqlCommand("update documents_details set aml_sanctions_flag=1 where SenderID_ID=@SenderID_ID");
                                            cmd_update.Parameters.AddWithValue("@SenderID_ID", obj.SenderID_ID);
                                            db_connection.ExecuteNonQueryProcedure(cmd_update);
                                        }
                                        catch
                                        {

                                        }

                                    }

                                    if (fitnessProbityFlag == 1)
                                    {
                                        string description1 = "Customer Found In fitness Probity";
                                        string Record_DateTime = obj.Record_Insert_DateTime;
                                        string notification_icon = "pep-match-not-found.jpg";
                                        //notification_message = "<span class='cls-admin'> - <span class='cls-cancel'><strong> International Sanctions Alert  </strong>- </span><span class='cls-addr-confirm'>" + description1 + " </span></span></span><span class='cls-customer'></span>";
                                        string notification_message = "<span class='cls-admin'> - <span class='cls-cancel'><strong> Fitness Probity Alert </strong>- </span><span class='cls-addr-confirm'>" + description1 + " </span></span></span><span class='cls-customer'></span>";
                                        CompanyInfo.save_notification(notification_message, notification_icon, Convert.ToInt32(obj.Customer_ID), Convert.ToDateTime(obj.Record_Insert_DateTime), Convert.ToInt32(obj.Client_ID), 1, obj.User_ID, Convert.ToInt32(obj.Branch_ID), 0, 0, 1, 0, context);

                                    }
                                    if (pepFlag == 1)
                                    {
                                        string description1 = "Customer Found In Sanctions";
                                        string Record_DateTime = obj.Record_Insert_DateTime;
                                        string notification_icon = "pep-match-not-found.jpg";
                                        //notification_message = "<span class='cls-admin'> - <span class='cls-cancel'><strong> International Sanctions Alert  </strong>- </span><span class='cls-addr-confirm'>" + description1 + " </span></span></span><span class='cls-customer'></span>";
                                        string notification_message = "<span class='cls-admin'> - <span class='cls-cancel'><strong> International Pep Alert </strong>- </span><span class='cls-addr-confirm'>" + description1 + " </span></span></span><span class='cls-customer'></span>";
                                        CompanyInfo.save_notification(notification_message, notification_icon, Convert.ToInt32(obj.Customer_ID), Convert.ToDateTime(obj.Record_Insert_DateTime), Convert.ToInt32(obj.Client_ID), 1, obj.User_ID, Convert.ToInt32(obj.Branch_ID), 0, 0, 1, 0, context);

                                    }
                                    if (sanctionFlag == 1)
                                    {
                                        string description1 = "Customer Found In Sanctions";
                                        string Record_DateTime = obj.Record_Insert_DateTime;
                                        string notification_icon = "pep-match-not-found.jpg";
                                        //notification_message = "<span class='cls-admin'> - <span class='cls-cancel'><strong> International Sanctions Alert  </strong>- </span><span class='cls-addr-confirm'>" + description1 + " </span></span></span><span class='cls-customer'></span>";
                                        string notification_message = "<span class='cls-admin'> - <span class='cls-cancel'><strong> International Sanctions Alert </strong>- </span><span class='cls-addr-confirm'>" + description1 + " </span></span></span><span class='cls-customer'></span>";
                                        CompanyInfo.save_notification(notification_message, notification_icon, Convert.ToInt32(obj.Customer_ID), Convert.ToDateTime(obj.Record_Insert_DateTime), Convert.ToInt32(obj.Client_ID), 1, obj.User_ID, Convert.ToInt32(obj.Branch_ID), 0, 0, 1, 0, context);

                                    }
                                    if (warningFlag == 1)
                                    {
                                        string description1 = "Customer Found In Warning";
                                        string Record_DateTime = obj.Record_Insert_DateTime;
                                        string notification_icon = "pep-match-not-found.jpg";
                                        //notification_message = "<span class='cls-admin'> - <span class='cls-cancel'><strong> International Sanctions Alert  </strong>- </span><span class='cls-addr-confirm'>" + description1 + " </span></span></span><span class='cls-customer'></span>";
                                        string notification_message = "<span class='cls-admin'> - <span class='cls-cancel'><strong> Warning Alert </strong>- </span><span class='cls-addr-confirm'>" + description1 + " </span></span></span><span class='cls-customer'></span>";
                                        CompanyInfo.save_notification(notification_message, notification_icon, Convert.ToInt32(obj.Customer_ID), Convert.ToDateTime(obj.Record_Insert_DateTime), Convert.ToInt32(obj.Client_ID), 1, obj.User_ID, Convert.ToInt32(obj.Branch_ID), 0, 0, 1, 0, context);

                                    }
                                }
                            }
                        }
                        catch
                        {
                            // Handle or log the error if needed
                        }
                    }

                    if (API_ID == 3)
                    {
                        url = Convert.ToString(ob["SelfiePhotoImageUrl"]);
                    }
                    if (API_ID == 5)
                    {
                        url = Convert.ToString(obj.shuftipro_face);
                    }
                    if (API_ID == 7)
                    {
                        url = Convert.ToString(veriffFaceURL);
                    }
                    if (API_ID == 9)
                    {
                        url = "Sumsub ID Scan";
                    }

                    if ((url != "" && url != null) && (API_ID == 3 || API_ID == 5))
                    {
                        try
                        {
                            obj.Record_Insert_DateTime = CompanyInfo.gettime(obj.Client_ID, Customer_ID.ToString(), obj.Country_ID, context);
                            string path = Customer_ID + "-" + obj.Record_Insert_DateTime.Replace(":", "") + ".jpg";
                            obj.FileNameWithExt = "assets/Upload_Profile_Images/" + path;//Convert.ToString(dtc.Rows[0]["RootURL"]) assets/Upload_Profile_Images/
                            using (var webClient = new WebClient())
                            {
                                webClient.Headers.Add("Content-Type", "application/json"); webClient.Headers.Add("Authorization", s13); webClient.Headers.Add("Cookie", "token " + s13);
                                byte[] imageBytes = webClient.DownloadData(url);
                                File.WriteAllBytes(Convert.ToString(dtc.Rows[0]["RootURL"]) + obj.FileNameWithExt, imageBytes);
                                MySqlCommand _cmd1 = new MySqlCommand("UpdateProfile_Image");
                                _cmd1.CommandType = CommandType.StoredProcedure;
                                _cmd1.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                                _cmd1.Parameters.AddWithValue("_Customer_ID", Customer_ID);
                                _cmd1.Parameters.AddWithValue("_FileNameWithExt", obj.FileNameWithExt);
                                int d = db_connection.ExecuteNonQueryProcedure(_cmd1);
                                if (d > 0)
                                {
                                    string Activity = "Updated profile image of <b>" + Username + "</b>";
                                    int stattus = (int)CompanyInfo.InsertActivityLogDetails(Activity, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(Customer_ID), "GetJourney", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "ID Upload", context);
                                }
                            }
                        }
                        catch { }
                    }
                    else if (API_ID == 9 && url != "" && url != null)
                    {

                        obj.Record_Insert_DateTime = CompanyInfo.gettime(obj.Client_ID, Customer_ID.ToString(), obj.Country_ID, context);
                        string path = Customer_ID + "-" + obj.Record_Insert_DateTime.Replace(":", "") + ".jpg";
                        obj.FileNameWithExt = "assets/Upload_Profile_Images/" + path;//Convert.ToString(dtc.Rows[0]["RootURL"]) assets/Upload_Profile_Images/





                        string selfieImageIdsString = string.Join(",", selfieImageIds.Select(id => id.ToString()));

                        // Now you can use these values in your code as needed



                        var stamp = ((long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds).ToString();
                        var userId = obj.Custmer_Ref;
                        var applicant_id = obj.applicant_id;
                        //inspectionId = "65d71d4e08ca10763627b720";
                        // Calculate the value to sign (excluding the body)
                        var valueToSign = stamp + "GET" + $"/resources/inspections/{inspectionId}/resources/{selfieImageIdsString}";


                        var appToken = UserName;
                        // Replace placeholders with actual values
                        valueToSign = valueToSign.Replace("{{sumsub_root_url}}", "")
                                                 .Replace("{{applicantId}}", "")
                                                 .Replace("{{levelName}}", "basic-kyc-level")
                                                 .Replace("{{userId}}", userId);

                        // Calculate the signature
                        var secretKey = Password; // Replace with your secret key
                        var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey));
                        var signatureBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(valueToSign));
                        var signature = BitConverter.ToString(signatureBytes).Replace("-", "").ToLower();

                        // Set your dynamic userId here
                        var client1 = new RestClient(urll);
                        var request1 = new RestRequest($"/resources/inspections/{inspectionId}/resources/{selfieImageIdsString}", Method.GET);
                        request1.AddHeader("Content-Type", "application/json");
                        request1.AddHeader("X-App-Token", appToken);
                        request1.AddHeader("X-App-Access-Ts", stamp);
                        request1.AddHeader("X-App-Access-Sig", signature);
                        IRestResponse responseT = client1.Execute(request1);
                        //jsonResponse = JObject.Parse(responseT.Content);




                        // Extract the binary content
                        byte[] imageData = responseT.RawBytes;

                        // Determine the file extension based on the Content-Type header
                        string contentType = responseT.ContentType;
                        string fileExtension = GetFileExtension(contentType);




                        // Save the image to a file
                        //string filePath = $"path/to/save/image.{fileExtension}";
                        //File.WriteAllBytes(obj.FileNameWithExt, imageData);
                        File.WriteAllBytes(Convert.ToString(dtc.Rows[0]["RootURL"]) + obj.FileNameWithExt + "", imageData);

                        CompanyInfo.InsertActivityLogDetails("GET  The Sumsub Selfie and applicantID 13 for Id scan " + obj.FileNameWithExt + ". ", Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(Customer_ID), "GetJourney", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "ID Upload", context);

                        activity_log = "GET  The Sumsub Selfie and applicantID 13 for Id scan " + obj.FileNameWithExt + "";
                        // Save the image to a file

                        MySqlCommand _cmd1 = new MySqlCommand("UpdateProfile_Image");
                        _cmd1.CommandType = CommandType.StoredProcedure;
                        _cmd1.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                        _cmd1.Parameters.AddWithValue("_Customer_ID", Customer_ID);
                        _cmd1.Parameters.AddWithValue("_FileNameWithExt", obj.FileNameWithExt);
                        int d = db_connection.ExecuteNonQueryProcedure(_cmd1);
                        if (d > 0)
                        {
                            string Activity = "Updated profile image of <b>" + Username + "</b>";
                            int stattus = (int)CompanyInfo.InsertActivityLogDetails(Activity, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(Customer_ID), "GetJourney", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "ID Upload", context);
                        }
                    }
                    else
                    {
                        url = Convert.ToString(ob["FacePhotoImageUrl"]);
                        if (url != "" && url != null)
                        {
                            try
                            {
                                obj.Record_Insert_DateTime = CompanyInfo.gettime(obj.Client_ID, Customer_ID.ToString(), obj.Country_ID, context);
                                string path = Customer_ID + "-" + obj.Record_Insert_DateTime.Replace(":", "") + ".jpg";
                                obj.FileNameWithExt = "assets/Upload_Profile_Images/" + path;//Convert.ToString(dtc.Rows[0]["RootURL"]) assets/Upload_Profile_Images/
                                using (var webClient = new WebClient())
                                {
                                    webClient.Headers.Add("Content-Type", "application/json"); webClient.Headers.Add("Authorization", s13); webClient.Headers.Add("Cookie", "token " + s13);
                                    byte[] imageBytes = webClient.DownloadData(url);
                                    File.WriteAllBytes(Convert.ToString(dtc.Rows[0]["RootURL"]) + obj.FileNameWithExt, imageBytes);
                                    MySqlCommand _cmd1 = new MySqlCommand("UpdateProfile_Image");
                                    _cmd1.CommandType = CommandType.StoredProcedure;
                                    _cmd1.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                                    _cmd1.Parameters.AddWithValue("_Customer_ID", Customer_ID);
                                    _cmd1.Parameters.AddWithValue("_FileNameWithExt", obj.FileNameWithExt);
                                    int d = db_connection.ExecuteNonQueryProcedure(_cmd1);
                                    if (d > 0)
                                    {
                                        string Activity = "Updated profile image of <b>" + Username + "</b>";
                                        int stattus = (int)CompanyInfo.InsertActivityLogDetails(Activity, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(Customer_ID), "GetJourney", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "ID Upload", context);
                                    }
                                }
                            }
                            catch { }
                        }
                    }

                    if (API_ID == 14)
                    {
                        string path = Customer_ID + "-" + obj.Record_Insert_DateTime.Replace(":", "") + ".jpg";
                        obj.FileNameWithExt = "assets/Upload_Profile_Images/" + path;//Convert.ToString(dtc.Rows[0]["RootURL"]) assets/Upload_Profile_Images/

                        try
                        { var session = GetVerificationSummary(Yoti_SessionId, API_ID, obj.Client_ID);

                            int counter = 1;  // For numbering the images
                            byte[] imageData = new byte[0];
                            foreach (var image in session.Liveness.Images)
                            {
                                var livenessMediaId = image.MediaId;
                                var livenessBase64Image = Convert.ToString(image.Base64Image)?.Trim();

                                if (!string.IsNullOrEmpty(livenessBase64Image))
                                {
                                    // Remove base64 header if present (like data:image/png;base64,...)
                                    if (livenessBase64Image.StartsWith("data:image", StringComparison.OrdinalIgnoreCase))
                                    {
                                        int commaIndex = livenessBase64Image.IndexOf(',');
                                        if (commaIndex > -1)
                                        {
                                            livenessBase64Image = livenessBase64Image.Substring(commaIndex + 1);
                                        }
                                    }

                                    try
                                    {
                                        imageData = Convert.FromBase64String(livenessBase64Image);
                                        counter++;  // Increment for next image
                                    }
                                    catch (FormatException fe)
                                    {
                                        Console.WriteLine($"Invalid Base64 for Liveness image {counter}: " + fe.Message);
                                    }
                                }
                            }
                            File.WriteAllBytes(Convert.ToString(dtc.Rows[0]["RootURL"]) + obj.FileNameWithExt + "", imageData);
                            MySqlCommand _cmd1 = new MySqlCommand("UpdateProfile_Image");
                            _cmd1.CommandType = CommandType.StoredProcedure;
                            _cmd1.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                            _cmd1.Parameters.AddWithValue("_Customer_ID", Customer_ID);
                            _cmd1.Parameters.AddWithValue("_FileNameWithExt", obj.FileNameWithExt);
                            int d = db_connection.ExecuteNonQueryProcedure(_cmd1);
                            if (d > 0)
                            {
                                string Activity = "Updated profile image of <b>" + Username + "</b>";
                                int stattus = (int)CompanyInfo.InsertActivityLogDetails(Activity, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(Customer_ID), "GetJourney", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "ID Upload", context);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Error saving liveness images: " + ex.Message);
                        }
                    }
                }
                if (dt.Rows.Count <= 0)
                    dt.Rows.Add(1, response1.StatusCode, obj.SenderID_ID);

            }
            catch (Exception e)
            {
                string Activity = "ID Scan Error Details: " + e.ToString() + "</br>";
                int stattus = (int)CompanyInfo.InsertActivityLogDetails(Activity, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(Customer_ID), "GetJourney", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "ID Upload", context);

                dt.Rows.Add(2, e.ToString(), obj.SenderID_ID);
            }
            finally
            {
                CompanyInfo.InsertActivityLogDetails("Id Scan Activity " + activity_log + ". ", Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(Customer_ID), "GetJourney", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "ID Upload", context);
            }
            return dt;
        }




        public DataTable GetJourney_old_25_2_2025(Model.Document obj, HttpContext context)
        {
            int Status = 1, Customer_ID = 0;
            int API_ID = 3;//GBG API ID
            DataTable dt = new DataTable();
            dt.Columns.Add("Status", typeof(int));
            dt.Columns.Add("ResponseMessage", typeof(string)); dt.Columns.Add("ResultID", typeof(int));
            string cname = "", mname = "", lname = "";

            try
            {
                string Shufti_refId = obj.ShuftiId;

                try
                {
                    string scanstatus = obj.scanstatus;
                }
                catch (Exception egx) { obj.scanstatus = ""; }

                Customer_ID = Convert.ToInt32(CompanyInfo.Decrypt(obj.UserName, true));//New
                obj.Customer_ID = Customer_ID.ToString();
                string urll = "", UserName = "", Password = "";// https://poc.idscan.cloud/idscanenterprisesvc/";// test
                //var urll = "https://prod.idscan.cloud/idscanenterprisesvc/";//live            


                MySqlConnector.MySqlCommand cmdp_active = new MySqlConnector.MySqlCommand("active_thirdparti_api");
                cmdp_active.CommandType = CommandType.StoredProcedure;
                DataTable dtApi_active = db_connection.ExecuteQueryDataTableProcedure(cmdp_active);
                if (dtApi_active.Rows.Count > 0)
                {
                    API_ID = Convert.ToInt32(dtApi_active.Rows[0]["API_ID"]);
                }


                obj.Record_Insert_DateTime = CompanyInfo.gettime(obj.Client_ID, Customer_ID.ToString(), obj.Country_ID, context);

                MySqlConnector.MySqlCommand cmdc = new MySqlConnector.MySqlCommand("UpdateIDScanCount");
                cmdc.CommandType = CommandType.StoredProcedure;
                cmdc.Parameters.AddWithValue("_Customer_ID", Customer_ID);//GBG API ID
                cmdc.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                cmdc.Parameters.AddWithValue("_Queryflag", 1);
                cmdc.Parameters.AddWithValue("_nowdate", obj.Record_Insert_DateTime);
                db_connection.ExecuteQueryDataTableProcedure(cmdc);

                #region notif
                cmdc = new MySqlConnector.MySqlCommand("UpdateIDScanCount");
                cmdc.CommandType = CommandType.StoredProcedure;
                cmdc.Parameters.AddWithValue("_Customer_ID", Customer_ID);//Customer ID
                cmdc.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                cmdc.Parameters.AddWithValue("_Queryflag", 2);//Get ID Scan Result
                cmdc.Parameters.AddWithValue("_nowdate", obj.Record_Insert_DateTime);
                string resultk = Convert.ToString(db_connection.ExecuteScalarProcedure(cmdc));
                if (resultk != null && resultk != "")
                {
                    if (resultk == "2")
                    {
                        string notification_icon = "sanction-list-match-found.jpg";
                        string notification_message = "<span class='cls-admin'>Daily ID scan limit<strong class='cls-cancel'> reached.</strong></span><span class='cls-customer'></span>";
                        CompanyInfo.save_notification_compliance(notification_message, notification_icon, Convert.ToString(Customer_ID), Convert.ToDateTime(obj.Record_Insert_DateTime), Convert.ToInt32(obj.Client_ID), 1, Convert.ToInt32(obj.User_ID), Convert.ToInt32(obj.Branch_ID), 0, 1, 1, 0, context);
                    }
                }
                #endregion


                MySqlConnector.MySqlCommand cmdp = new MySqlConnector.MySqlCommand("GetAPIDetails");
                cmdp.CommandType = CommandType.StoredProcedure;
                cmdp.Parameters.AddWithValue("_API_ID", API_ID);//GBG API ID
                cmdp.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                cmdp.Parameters.AddWithValue("_status", 0);// API Status
                DataTable dtApi = db_connection.ExecuteQueryDataTableProcedure(cmdp);
                if (dtApi.Rows.Count > 0)
                {
                    urll = Convert.ToString(dtApi.Rows[0]["API_URL"]);
                    UserName = Convert.ToString(dtApi.Rows[0]["UserName"]);
                    Password = Convert.ToString(dtApi.Rows[0]["Password"]);
                }


                //get company details
                DataTable dtc = CompanyInfo.get(obj.Client_ID, context);

                string base64String = string.Empty;
                string s13 = string.Empty;
                if (API_ID == 3)
                {
                    //Get Token              
                    WebResponse response2 = Token(urll, UserName, Password, context);
                    using (var reader = new StreamReader(response2.GetResponseStream()))
                    {
                        string ApiStatus = reader.ReadToEnd();
                        var entries = ApiStatus.TrimStart('{').TrimEnd('}').Replace("\"", String.Empty).Split(',');
                        foreach (var entry in entries)
                        {
                            if (entry.Split(':')[0] == "access_token")
                            {
                                s13 = entry.Split(':')[1]; break;
                            }
                        }
                    }
                }

                // Get Journey
                string data = "journeyID=" + obj.JourneyID + "";
                //Check duplicate journey upload
                cmdc = new MySqlConnector.MySqlCommand("select count(ifnull(SenderID_ID,0)) as IDCount from documents_details where JourneyID=@JourneyID and Client_ID=@Client_ID and Customer_ID=@Customer_ID");
                cmdc.Parameters.AddWithValue("@Customer_ID", Customer_ID);//GBG API ID
                cmdc.Parameters.AddWithValue("@Client_ID", obj.Client_ID);
                cmdc.Parameters.AddWithValue("@JourneyID", obj.JourneyID);
                DataTable idc = db_connection.ExecuteQueryDataTableProcedure(cmdc);
                if (idc.Rows.Count > 0)
                {
                    int cnt = Convert.ToInt32(idc.Rows[0]["IDCount"]);
                    if (cnt > 0)
                    {
                        dt.Rows.Add(0, "Duplicate Journey", obj.SenderID_ID);
                        CompanyInfo.InsertActivityLogDetails("Duplicate call for journey " + obj.JourneyID + ". ", Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(Customer_ID), "GetJourney", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "ID Upload", context);
                        return dt;
                    }
                }

                byte[] dataStream = Encoding.UTF8.GetBytes(data);

                string userAuthenticationURI = "" + urll + "Search/GetEvaluatedPersonEntryValidationResults";
                string resresult = "";
                var client = new RestClient(userAuthenticationURI + "?id=" + obj.JourneyID + "");
                client.Timeout = -1;
                var request = new RestRequest(Method.GET);
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("Authorization", s13);
                request.AddHeader("Cookie", "token " + s13); // Extra Parameter for DataField "scanningToken=" + 
                string CallFrom = "", Req = "";
                if (API_ID == 3)
                {
                    CallFrom = "Journey Details";
                    Req = "journeyID=" + obj.JourneyID + " Customer_ID=" + Customer_ID + "";
                }
                if (API_ID == 5)
                {
                    CallFrom = "Shuftipro Scan";
                    Req = obj.JourneyID;
                }

                string veriffstatus = "", veriffresponseCode = "", veriffcountryname = "", veriffDocumentName = "", veriffSenderNameOnID = "", veriffcname1 = "";
                string veriffmname1 = "", verifflname1 = "", veriffSenderID_Number = "", veriffIssue_date = "", veriffSex = "", veriffDocumentCategory = "",
                    veriffDocumentType = "", veriffFront_DocumentTypeID = "", veriffIssuingStateName = "", veriffSender_DateOfBirth = "", verifffrontdoc_expired = "";
                dynamic dynJson = null, dynJsonMedia = null;
                string veriffFrontDocURL = "", veriffBackDocURL = "", veriffFaceURL = "";
                string veriffFrontDocURLID = "", veriffBackDocURLID = "", veriffFaceURLID = "";

                string idDocType = "";
                string idDocCountry = "";
                string idDocFirstName = "";
                string idDocFirstNameEn = "";
                string idDocLastName = "";
                string idDocLastNameEn = "";
                DateTime idDocValidUntil = new DateTime(); string idDocValidUntildate = "";
                string idDocNumber = "";
                DateTime idDocDob = new DateTime();
                string idDocBgCheckViolations = "";
                string idDocOcrDocTypes = "";
                string idDocMrzLine1 = "";
                string idDocMrzLine2 = "";
                JArray identityImageIds = new JArray();
                DateTime SID_Expiry = new DateTime();
                JArray selfieImageIds = new JArray();
                string DocumentType = "";
                DateTime SID_bdate = new DateTime();
                string Remark = "";
                string stat = "";

                if (API_ID == 7)
                {
                    CallFrom = "Veriff Scan";
                    Req = obj.JourneyID;
                    string xHmacSignature = "";
                    string sharedSecretKey = Password;
                    string sessionId = obj.JourneyID;
                    byte[] sharedSecretKeyBytes = Encoding.UTF8.GetBytes(sharedSecretKey);
                    byte[] sessionIdBytes = Encoding.UTF8.GetBytes(sessionId);

                    using (var hmac = new HMACSHA256(sharedSecretKeyBytes))
                    {
                        byte[] hash = hmac.ComputeHash(sessionIdBytes);
                        xHmacSignature = BitConverter.ToString(hash).Replace("-", "").ToLower();
                    }

                    var client_ = new RestClient(urll + obj.JourneyID + "/decision");
                    client_.Timeout = -1;
                    var request_ = new RestRequest();
                    request_.AddHeader("Content-Type", "application/json");
                    request_.AddHeader("X-AUTH-CLIENT", UserName);
                    request_.AddHeader("X-HMAC-SIGNATURE", xHmacSignature);
                    var body = @"";
                    request_.AddParameter("application/json", body, ParameterType.RequestBody);
                    IRestResponse response = client_.Execute(request_);
                    dynJson = Newtonsoft.Json.JsonConvert.DeserializeObject(response.Content);
                    // string responseCode = dynJson["verification"].ToString();
                    veriffresponseCode = dynJson.ToString();
                    try
                    {
                        veriffstatus = dynJson["verification"]["status"].ToString();
                    }
                    catch (Exception exv) { }

                    client_ = new RestClient(urll + obj.JourneyID + "/media");
                    client_.Timeout = -1;
                    request_ = new RestRequest();
                    request_.AddHeader("Content-Type", "application/json");
                    request_.AddHeader("X-AUTH-CLIENT", UserName);
                    request_.AddHeader("X-HMAC-SIGNATURE", xHmacSignature);
                    body = @"";
                    request_.AddParameter("application/json", body, ParameterType.RequestBody);
                    IRestResponse responseMedia = client_.Execute(request_);
                    dynJsonMedia = Newtonsoft.Json.JsonConvert.DeserializeObject(responseMedia.Content);
                    int count = 0;
                    foreach (var items in dynJsonMedia["images"])
                    {
                        try
                        {
                            if (dynJsonMedia["images"][count]["name"].ToString() == "face")
                            {
                                veriffFaceURL = dynJsonMedia["images"][count]["url"].ToString();
                                veriffFaceURLID = dynJsonMedia["images"][count]["id"].ToString();
                            }
                            if (dynJsonMedia["images"][count]["name"].ToString() == "document-front-pre")
                            {
                                veriffFrontDocURL = dynJsonMedia["images"][count]["url"].ToString();
                                veriffFrontDocURLID = dynJsonMedia["images"][count]["id"].ToString();
                            }
                            if (dynJsonMedia["images"][count]["name"].ToString() == "document-back-pre")
                            {
                                veriffBackDocURL = dynJsonMedia["images"][count]["url"].ToString();
                                veriffBackDocURLID = dynJsonMedia["images"][count]["id"].ToString();
                            }
                        }
                        catch (Exception exv) { }
                        count++;
                    }

                    /*urll = urll.Replace("sessions", "media");
                    client_ = new RestClient(urll + veriffFaceURLID);
                    client_.Timeout = -1;
                    request_ = new RestRequest();
                    request_.AddHeader("Content-Type", "application/json");
                    request_.AddHeader("X-AUTH-CLIENT", UserName);
                    request_.AddHeader("X-HMAC-SIGNATURE", xHmacSignature);
                    body = @"";
                    request_.AddParameter("application/json", body, ParameterType.RequestBody);
                    responseMedia = client_.Execute(request_);
                    dynJsonMedia = Newtonsoft.Json.JsonConvert.DeserializeObject(responseMedia.Content);
                    */
                }

                try
                {
                    MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("SaveAPIRequestResponce");
                    _cmd.CommandType = CommandType.StoredProcedure;
                    _cmd.Parameters.AddWithValue("_API_ID", API_ID);
                    _cmd.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                    _cmd.Parameters.AddWithValue("_Customer_ID", Customer_ID);
                    _cmd.Parameters.AddWithValue("_status", 0);
                    _cmd.Parameters.AddWithValue("_Function_name", CallFrom);
                    _cmd.Parameters.AddWithValue("_Remark", 0);
                    _cmd.Parameters.AddWithValue("_comments", Req);
                    _cmd.Parameters.AddWithValue("_Record_Insert_DateTime", obj.Record_Insert_DateTime);
                    _cmd.Parameters.AddWithValue("_Branch_ID", obj.Branch_ID);
                    int msg1 = db_connection.ExecuteNonQueryProcedure(_cmd);
                }
                catch (Exception ex)
                {
                    string error = ex.ToString().Replace("\'", "\\'");
                    MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("SaveException");
                    _cmd.CommandType = CommandType.StoredProcedure;
                    _cmd.Parameters.AddWithValue("_Record_Insert_DateTime", obj.Record_Insert_DateTime);
                    _cmd.Parameters.AddWithValue("_error", error);
                    _cmd.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                    int msg1 = db_connection.ExecuteNonQueryProcedure(_cmd);
                }
                IRestResponse response1 = new RestResponse();
                if (API_ID == 3)
                {
                    response1 = client.Execute(request);
                    resresult = response1.Content;
                }
                Remark = ""; int score = 0;
                try
                {
                    string Bandtext = "", saveresp = "";
                    if (resresult != "" && API_ID == 3)
                    {
                        try
                        {
                            Newtonsoft.Json.Linq.JObject ob = Newtonsoft.Json.Linq.JObject.Parse(resresult);
                            Bandtext = Convert.ToString(ob["HighLevelResult"]);
                            //var arr = ob["ProcessedDocuments"][0]["ExtractedFields"];String roundTrip = Convert.ToString(resresult); byte[] bytes = System.Text.Encoding.UTF8.GetBytes(roundTrip); //string finalresp = System.Text.Encoding.UTF8.GetString(bytes);
                            saveresp = System.Text.Encoding.ASCII.GetString(System.Text.Encoding.GetEncoding("ISO-8859-8").GetBytes(resresult));
                            // "Journey ID: " + journeyID + " HighLevelResult: " + Bandtext + " HighLevelResultDetails: " + ob["HighLevelResultDetails"] + " ProcessedDocuments: " + arr;
                        }
                        catch (Exception ex) { }
                    }

                    if (API_ID == 5)
                    {
                        try
                        {
                            score = Convert.ToInt32(obj.shuftipro_doc_face_match_confidence);
                        }
                        catch { score = 0; }
                        try
                        {
                            Bandtext = Convert.ToString(obj.shufti_status).ToLower();

                            int index = Bandtext.IndexOf(".");
                            Bandtext = Bandtext.Substring(Bandtext.LastIndexOf('.') + 1);
                        }
                        catch { }
                    }
                    if (API_ID == 7)
                    {
                        try
                        {
                            Bandtext = veriffstatus;
                        }
                        catch { }
                    }

                    Remark = Convert.ToString(CompanyInfo.getAPIStatus(Bandtext, obj.Client_ID));
                    if (Remark != null && Remark != "")
                        Status = 0;

                    MySqlConnector.MySqlCommand _cmd;




                    if (API_ID == 3)
                    {
                        if (obj.scanstatus == "INPROGRESS")
                        {
                        }
                        else
                        {
                            score = GetFaceMatchScore(s13, urll, obj);
                        }
                    }
                    _cmd = new MySqlConnector.MySqlCommand("UpdateCustIDScanResults");//Update bandtext and score
                    _cmd.CommandType = CommandType.StoredProcedure;
                    _cmd.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                    _cmd.Parameters.AddWithValue("_Customer_ID", Customer_ID);
                    _cmd.Parameters.AddWithValue("_Remark", Remark);
                    _cmd.Parameters.AddWithValue("_Score", score);
                    int msg = db_connection.ExecuteNonQueryProcedure(_cmd);
                    _cmd.Dispose();


                    if (API_ID == 5)
                    {
                        CallFrom = "Shuftipro Scan";
                        saveresp = obj.JourneyID;
                    }

                    if (API_ID == 7)
                    {
                        CallFrom = "Veriff Scan Response";
                        saveresp = veriffresponseCode;
                        if (veriffstatus == "approved") { Status = 0; }
                    }

                    _cmd = new MySqlConnector.MySqlCommand("SaveAPIRequestResponce");
                    _cmd.CommandType = CommandType.StoredProcedure;
                    _cmd.Parameters.AddWithValue("_API_ID", API_ID);
                    _cmd.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                    _cmd.Parameters.AddWithValue("_Customer_ID", Customer_ID);
                    _cmd.Parameters.AddWithValue("_status", 1);
                    _cmd.Parameters.AddWithValue("_Function_name", CallFrom);
                    _cmd.Parameters.AddWithValue("_Remark", Remark);
                    _cmd.Parameters.AddWithValue("_comments", saveresp.Replace("\"", ""));
                    _cmd.Parameters.AddWithValue("_Record_Insert_DateTime", obj.Record_Insert_DateTime);
                    _cmd.Parameters.AddWithValue("_Branch_ID", obj.Branch_ID);
                    int msg1 = db_connection.ExecuteNonQueryProcedure(_cmd);
                    try
                    {
                        _cmd.Dispose();
                        _cmd = new MySqlConnector.MySqlCommand("customer_details_by_param");
                        _cmd.CommandType = CommandType.StoredProcedure;
                        string _whereclause = " and cr.Client_ID=" + obj.Client_ID + " and cr.Customer_ID=" + Customer_ID;

                        _cmd.Parameters.AddWithValue("_whereclause", _whereclause);
                        CompanyInfo.InsertActivityLogDetails("Cust Details where clause : " + _whereclause + "     ", Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(Customer_ID), "GetJourney", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "ID Upload", context);
                        _cmd.Parameters.AddWithValue("_SecurityKey", CompanyInfo.SecurityKey());
                        DataTable d1 = db_connection.ExecuteQueryDataTableProcedure(_cmd);
                        if (d1.Rows.Count == 1)
                        {
                            cname = Convert.ToString(d1.Rows[0]["First_Name"]).ToUpper();
                            mname = Convert.ToString(d1.Rows[0]["Middle_Name"]).ToUpper();
                            lname = Convert.ToString(d1.Rows[0]["Last_Name"]).ToUpper();
                        }
                        CompanyInfo.InsertActivityLogDetails("Cust Details where clause : " + _whereclause + "   & Fnm: " + cname + " Mnm: " + mname + "  Lnm: " + lname + ". ", Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(Customer_ID), "GetJourney", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "ID Upload", context);
                        if (API_ID == 3 || API_ID == 5 || API_ID == 7)
                        {
                            if (Remark == "1" || Remark == "2" || Remark == "3" || Remark == "4" || Remark == "5")//alert or refer or notsupported or undefined or INPROGRESS then  send mail to admin
                            {
                                string EmailUrl = "", Company_Name = "";
                                if (dtc.Rows.Count > 0)
                                {
                                    EmailUrl = Convert.ToString(dtc.Rows[0]["Company_URL_Admin"]);
                                    Company_Name = Convert.ToString(dtc.Rows[0]["Company_Name"]);
                                }

                                string sendmsg = "Response from GBG ID Scan for  " + d1.Rows[0]["WireTransfer_ReferanceNo"] + "   is   " + Bandtext + "";
                                if (API_ID == 7)
                                {
                                    sendmsg = "Response from Veriff Station for  " + obj.JourneyID + "   is   " + Bandtext + "";
                                }


                                string EmailID = "";
                                DataTable dt_admin_Email_list = (DataTable)CompanyInfo.getAdminEmailList(obj.Client_ID, obj.Branch_ID);
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

                                subject = "" + Company_Name + " - Compliance - ID Scan Alert - " + d1.Rows[0]["WireTransfer_ReferanceNo"];

                                string mail_send = (string)CompanyInfo.Send_Mail(dtc, EmailID, body, subject, obj.Client_ID, obj.Branch_ID, "", "", "", context);
                            }
                        }
                    }
                    catch (Exception ae)
                    {
                        //string stattus = (string)mtsmethods.InsertErrorLogDetails(ae.Message.Replace("\'", "\\'"), user_id, "Check Credit Safe", i.Branch_ID, i.Client_ID);
                    }

                }
                catch (Exception ex)
                {
                    string error = ex.ToString().Replace("\'", "\\'");
                    MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("SaveException");
                    _cmd.CommandType = CommandType.StoredProcedure;
                    _cmd.Parameters.AddWithValue("_Record_Insert_DateTime", obj.Record_Insert_DateTime);
                    _cmd.Parameters.AddWithValue("_error", error);
                    _cmd.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                    int msg1 = db_connection.ExecuteNonQueryProcedure(_cmd);
                }

                if (response1.StatusCode.ToString() == "OK" || obj.JourneyID.Contains("SP_") || veriffstatus != "")
                {
                    var journeyID = new object();
                    var countryname = new object();
                    var DocumentName = new object();
                    Newtonsoft.Json.Linq.JObject ob = new Newtonsoft.Json.Linq.JObject();
                    if (API_ID == 3)
                    {
                        try
                        {
                            /* string formattedstr = System.Text.Encoding.ASCII.GetString(System.Text.Encoding.GetEncoding("ISO-8859-8").GetBytes(resresult));
                             ob = Newtonsoft.Json.Linq.JObject.Parse(formattedstr); */

                            ob = Newtonsoft.Json.Linq.JObject.Parse(resresult);
                            journeyID = ob["EvaluatedPersonEntryId"]; countryname = ob["CountryName"];
                            DocumentName = ob["DocumentName"];
                            obj.SenderNameOnID = Convert.ToString(ob["FullName"]);
                        }
                        catch (Exception ex) { }
                    }

                    string BirthPlace = "", Sex = "", DocumentCategory = "", IssuingStateName = "", Front_DocumentTypeID = "";
                    //obj.SenderNameOnID = Convert.ToString(ob["FirstName"]) + " " + Convert.ToString(ob["LastName"]);  obj.SenderID_Number = Convert.ToString(ob["DocumentNumber"]);  obj.SenderID_ExpiryDate = Convert.ToString(ob["ExpiryDate"]); obj.Issue_date = Convert.ToString(ob["IssueDate"]);  var xyz = Convert.ToString(ob["IssuingAuthority"]); BirthPlace = Convert.ToString(ob["BirthPlace"]);
                    //Sex = Convert.ToString(ob["Gender"]); DocumentCategory = Convert.ToString(ob["DocumentCategory"]); DocumentType = Convert.ToString(ob["DocumentType"]);
                    string cname1 = "", mname1 = "", lname1 = "";

                    if (API_ID == 3)
                    {
                        var arr = ob["ExtractedFields"];
                        for (var i = 0; i < arr.Count(); i++)
                        {
                            string name = Convert.ToString(arr[i]["Name"]);
                            if (name == "ExpiryDate") { obj.SenderID_ExpiryDate = Convert.ToString(arr[i]["Value"]); }
                            try { if (name == "FirstName") { cname1 = Convert.ToString(arr[i]["Value"]); } if (name == "MiddleName") { mname1 = Convert.ToString(arr[i]["Value"]); } if (name == "LastName") { lname1 = Convert.ToString(arr[i]["Value"]); } }
                            catch { }
                            if (name == "BirthPlace") { BirthPlace = Convert.ToString(arr[i]["Value"]); }
                            if (name == "DocumentNumber") { obj.SenderID_Number = Convert.ToString(arr[i]["Value"]); }
                            if (name == "IssueDate") { obj.Issue_date = Convert.ToString(arr[i]["Value"]); }
                            if (name == "IssuingAuthority") { var xyz = Convert.ToString(arr[i]["Value"]); }
                            if (name == "Sex") { Sex = Convert.ToString(arr[i]["Value"]); }
                            if (name == "DocumentCategory") { DocumentCategory = Convert.ToString(arr[i]["Value"]); }
                            if (name == "DocumentType") { DocumentType = Convert.ToString(arr[i]["Value"]); }
                            if (name == "IssuingStateName") { IssuingStateName = Convert.ToString(arr[i]["Value"]); }
                            if (name == "Front Document Type ID") { Front_DocumentTypeID = Convert.ToString(arr[i]["Value"]); }
                            if (name == "BirthDate") { obj.Sender_DateOfBirth = Convert.ToString(arr[i]["Value"]); }
                        }
                    }

                    int shuftipro_frontdoc_expired = 0;
                    if (API_ID == 5)
                    {
                        journeyID = Convert.ToString(obj.ShuftiId);
                        countryname = Convert.ToString(obj.shuftipro_document_proof_document_country);
                        DocumentName = Convert.ToString(obj.shuftipro_document_proof_document_official_name);
                        obj.SenderNameOnID = Convert.ToString(obj.shuftipro_document_proof_full_name);

                        cname1 = Convert.ToString(obj.shuftipro_document_first_name);
                        mname1 = Convert.ToString(obj.shuftipro_document_middle_name);
                        lname1 = Convert.ToString(obj.shuftipro_document_last_name);
                        string full_name = Convert.ToString(obj.shuftipro_document_proof_full_name);
                        if (cname1 == "" || cname1 == null)
                        {
                            if (full_name.Length > 0)
                                cname1 = full_name.Split(' ')[0];
                        }
                        if (mname1 == "" || mname1 == null)
                        {
                            if (full_name.Length > 1)
                                mname1 = full_name.Split(' ')[1];
                        }
                        if (lname1 == "" || lname1 == null)
                        {
                            for (int i = 2; i < full_name.Split(' ').Length; i++)
                            {
                                lname1 += full_name.Split(' ')[i];
                            }
                        }

                        obj.SenderID_Number = Convert.ToString(obj.shuftipro_document_proof_document_number);
                        obj.Issue_date = Convert.ToString(obj.shuftipro_doc_issue_date);
                        Sex = Convert.ToString(obj.shuftipro_document_proof_gender);
                        DocumentCategory = "";
                        DocumentType = Convert.ToString(obj.shuftipro_doc_selected_type);
                        Front_DocumentTypeID = Convert.ToString(obj.shuftipro_doc_selected_type);
                        IssuingStateName = "";
                        obj.Sender_DateOfBirth = Convert.ToString(obj.shuftipro_document_proof_dob);
                        score = Convert.ToInt32(obj.shuftipro_doc_face_match_confidence);
                        shuftipro_frontdoc_expired = Convert.ToInt32(obj.shuftipro_frontdoc_expired);
                    }

                    if (API_ID == 7)
                    {
                        try
                        {
                            journeyID = obj.JourneyID;
                            try
                            {
                                countryname = dynJson["verification"]["person"]["addresses"][0]["parsedAddress"]["country"].ToString();
                            }
                            catch (Exception ex) { }
                            DocumentName = dynJson["verification"]["person"]["firstName"].ToString() + " " + dynJson["verification"]["person"]["lastName"].ToString();
                            obj.SenderNameOnID = dynJson["verification"]["person"]["firstName"].ToString() + " " + dynJson["verification"]["person"]["lastName"].ToString();

                            cname1 = dynJson["verification"]["person"]["firstName"].ToString();
                            mname1 = "";
                            lname1 = dynJson["verification"]["person"]["lastName"].ToString();
                            string full_name = dynJson["verification"]["person"]["firstName"].ToString() + " " + dynJson["verification"]["person"]["lastName"].ToString();
                            if (cname1 == "" || cname1 == null)
                            {
                                if (full_name.Length > 0)
                                    cname1 = full_name.Split(' ')[0];
                            }
                            if (mname1 == "" || mname1 == null)
                            {
                                if (full_name.Length > 1)
                                    mname1 = full_name.Split(' ')[1];
                            }
                            if (lname1 == "" || lname1 == null)
                            {
                                for (int i = 2; i < full_name.Split(' ').Length; i++)
                                {
                                    lname1 += full_name.Split(' ')[i];
                                }
                            }

                            if (full_name.Length > 0)
                                cname1 = full_name.Split(' ')[0];

                            try
                            {
                                obj.Issue_date = dynJson["verification"]["document"]["firstIssue"].ToString();
                            }
                            catch (Exception ex) { obj.Issue_date = null; }
                            try { Sex = dynJson["verification"]["person"]["gender"].ToString(); } catch (Exception ex) { }
                            DocumentCategory = "";
                            try { DocumentType = dynJson["verification"]["document"]["type"].ToString(); } catch (Exception ex) { }
                            try { Front_DocumentTypeID = dynJson["verification"]["document"]["type"].ToString(); } catch (Exception ex) { }
                            try { IssuingStateName = dynJson["verification"]["document"]["issuedBy"].ToString(); } catch (Exception ex) { }
                            try
                            {
                                obj.Sender_DateOfBirth = dynJson["verification"]["person"]["dateOfBirth"].ToString();
                            }
                            catch (Exception ex) { obj.Sender_DateOfBirth = null; }
                            score = 0;
                            try
                            {
                                obj.SenderID_ExpiryDate = verifffrontdoc_expired = dynJson["verification"]["document"]["validUntil"].ToString();
                            }
                            catch (Exception ex) { obj.SenderID_ExpiryDate = null; }
                            try
                            {
                                obj.SenderID_Number = dynJson["verification"]["document"]["number"].ToString();
                            }
                            catch (Exception ex) { obj.SenderID_Number = null; }
                        }
                        catch (Exception ex) { }
                    }


                    int frontside = 0, liveness = 0, facematch = 0;
                    if (API_ID == 3)
                    {
                        var arr1 = ob["JourneySteps"];
                        for (var i = 0; i < arr1.Count(); i++)
                        {
                            string type = Convert.ToString(arr1[i]["Type"]);
                            string val = Convert.ToString(arr1[i]["HighLevelResult"]).ToLower();
                            if (type == "FRONTSIDE")
                            {
                                if (val != "passed") { frontside = 1; }
                            }
                            else if (type == "LIVENESS")
                            {
                                if (val != "passed") { liveness = 1; }
                            }
                        }
                        var arr2 = ob["AdditionalData"];
                        for (var i = 0; i < arr2.Count(); i++)
                        {
                            string type = Convert.ToString(arr2[i]["Name"]);
                            string val = Convert.ToString(arr2[i]["Value"]).ToLower();
                            if (type == "AutomatedFaceMatchResult")
                            {
                                if (val != "passed") { facematch = 1; }
                            }
                        }
                    }
                    if (API_ID == 5)
                    {
                        if (obj.background_checks == "0")
                        {
                            liveness = 1;
                        }
                        if (obj.shuftipro_frontdoc_available == "0")
                        {
                            frontside = 1;
                        }
                        if (obj.shuftipro_face_on_document_matched == "0")
                        {
                            facematch = 1;
                        }
                    }

                    if (API_ID == 7)
                    {
                        if (dynJson["status"].ToString() != "success")
                        {
                            liveness = 1;
                        }
                        if (dynJson["verification"]["document"]["type"].ToString() == "")
                        {
                            frontside = 1;
                        }
                    }
                    /**************************             Check Score               *****************************/
                    //Check probable match

                    string res1 = cname.Substring(0, 1);
                    string res3 = "";
                    if (lname != "" && lname != null)
                        res3 = lname.Substring(0, 1);
                    int namevalidflag = 1;
                    if (obj.SenderNameOnID == (cname + " " + lname))
                    {
                        namevalidflag = 0;//Exact Match
                    }
                    else if (mname != null && mname != "" && mname != " ")
                    {
                        string res2 = mname.Substring(0, 1);
                        if (obj.SenderNameOnID.ToLower() == (cname.ToLower() + " " + mname.ToLower() + " " + lname.ToLower()) || obj.SenderNameOnID.ToLower() == (cname.ToLower() + " " + res2.ToLower() + " " + lname.ToLower()))
                        {
                            namevalidflag = 0; // Exact Match
                        }
                        else if (obj.SenderNameOnID.ToLower() == (res1.ToLower() + " " + mname.ToLower() + " " + lname.ToLower()) || obj.SenderNameOnID.ToLower() == (cname.ToLower() + " " + mname.ToLower() + " " + res3.ToLower()) || obj.SenderNameOnID.ToLower() == (cname.ToLower() + " " + res2.ToLower() + " " + lname.ToLower())
                            || cname1.ToLower() == cname.ToLower() || cname1.ToLower() == lname.ToLower() || cname1.ToLower() == res1.ToLower() || lname1.ToLower() == lname.ToLower() || lname1.ToLower() == cname.ToLower() || lname1.ToLower() == res3.ToLower() || mname1.ToLower() == mname.ToLower() || mname1.ToLower() == res2.ToLower())
                        {
                            namevalidflag = 2;//Probable match
                        }
                    }
                    else if (obj.SenderNameOnID.ToLower() == (cname.ToLower() + " " + lname.ToLower()) || obj.SenderNameOnID.ToLower() == (res1.ToLower() + " " + lname.ToLower()) || obj.SenderNameOnID.ToLower() == (cname.ToLower() + " " + res3.ToLower())
                         || cname1.ToLower() == cname.ToLower() || cname1.ToLower() == lname.ToLower() || cname1.ToLower() == res1.ToLower() || lname1.ToLower() == lname.ToLower() || lname1.ToLower() == cname.ToLower() || lname1.ToLower() == res3.ToLower())
                    {
                        namevalidflag = 2; //Probable Match
                    }
                    else
                    {
                        namevalidflag = 1;// Invalid
                    }

                    /*if (API_ID == 5)
                    {
                        if (obj.SenderNameOnID.ToLower().Contains(cname.ToLower()) && obj.SenderNameOnID.Contains(mname.ToLower()) && obj.SenderNameOnID.Contains(lname.ToLower()))
                        {
                            namevalidflag = 0;
                        }
                        if (obj.SenderNameOnID.ToLower().Contains(cname.ToLower()) || obj.SenderNameOnID.Contains(mname.ToLower()) || obj.SenderNameOnID.Contains(lname.ToLower()))
                        {
                            namevalidflag = 2;
                        }
                    }*/


                    if (namevalidflag == 2)
                    {
                        if (API_ID == 3)
                            CompanyInfo.InsertActivityLogDetails("Probable match found for journey " + journeyID + ". Customer name on GBG ID Scan: Full Name " + obj.SenderNameOnID + ", First Name: " + obj.SenderNameOnID + ", Middle Name: " + obj.SenderNameOnID + ", Last Name: " + obj.SenderNameOnID + " and customer name in the system is " + (cname + " " + mname + " " + res3) + ". ", Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(Customer_ID), "GetJourney", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "ID Upload", context);
                        if (API_ID == 7 || API_ID == 5)
                            CompanyInfo.InsertActivityLogDetails("Probable match found for journey " + journeyID + ". Customer name on ID Scan: Full Name " + obj.SenderNameOnID + ", First Name: " + obj.SenderNameOnID + ", Middle Name: " + obj.SenderNameOnID + ", Last Name: " + obj.SenderNameOnID + " and customer name in the system is " + (cname + " " + mname + " " + res3) + ". ", Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(Customer_ID), "GetJourney", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "ID Upload", context);
                    }
                    else if (namevalidflag == 1)
                    {
                        if (API_ID == 3)
                            CompanyInfo.InsertActivityLogDetails("Name on ID is mismatched for journey " + journeyID + ". Customer name on GBG ID Scan: Full Name " + obj.SenderNameOnID + ", First Name: " + obj.SenderNameOnID + ", Middle Name: " + obj.SenderNameOnID + ", Last Name: " + obj.SenderNameOnID + " and customer name in the system is " + (cname + " " + mname + " " + res3) + ". ", Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(Customer_ID), "GetJourney", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "ID Upload", context);
                        if (API_ID == 7 || API_ID == 5)
                            CompanyInfo.InsertActivityLogDetails("Name on ID is mismatched for journey " + journeyID + ". Customer name on ID Scan: Full Name " + obj.SenderNameOnID + ", First Name: " + obj.SenderNameOnID + ", Middle Name: " + obj.SenderNameOnID + ", Last Name: " + obj.SenderNameOnID + " and customer name in the system is " + (cname + " " + mname + " " + res3) + ". ", Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(Customer_ID), "GetJourney", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "ID Upload", context);
                    }
                    int IsIdvalid = 0;// use it for probable match //|| obj.Sender_DateOfBirth == null || obj.Sender_DateOfBirth == "" 
                    if ((score <= 25 || namevalidflag == 1 || obj.SenderNameOnID == "Unknown" || Remark == "3" || obj.SenderNameOnID == null || obj.SenderNameOnID == "" ||
                        obj.SenderID_ExpiryDate == null || obj.SenderID_ExpiryDate == "" || DocumentType == "Unknown" || DocumentType == "" || DocumentType == null || frontside == 1 || liveness == 1 || facematch == 1) && API_ID != 7)
                    {
                        #region notific
                        string notification_icon = "", notification_message = "";
                        if (obj.IDType_ID == 1 || obj.IDType_ID == 5)
                        {
                            notification_icon = "wrong-id-upload.jpg";
                            notification_message = "<span class='cls-admin'>Invalid <strong class='cls-priamary'>Primary ID</strong> document found on ID Scan.</span>";
                        }
                        CompanyInfo.save_notification_compliance(notification_message, notification_icon, Convert.ToString(Customer_ID), Convert.ToDateTime(obj.Record_Insert_DateTime), obj.Client_ID, 1, 0, obj.Branch_ID, 0, 1, 1, 0, context);
                        if (frontside == 1)
                        {
                            notification_icon = "wrong-id-upload.jpg";
                            notification_message = "<span class='cls-admin'>Front side match of <strong class='cls-priamary'>Primary ID</strong> failed.</span>";
                            CompanyInfo.save_notification_compliance(notification_message, notification_icon, Convert.ToString(Customer_ID), Convert.ToDateTime(obj.Record_Insert_DateTime), obj.Client_ID, 1, 0, obj.Branch_ID, 0, 1, 1, 0, context);
                        }
                        if (liveness == 1)
                        {
                            notification_icon = "wrong-id-upload.jpg";
                            notification_message = "<span class='cls-admin'>Liveness failed.</span>";
                            CompanyInfo.save_notification_compliance(notification_message, notification_icon, Convert.ToString(Customer_ID), Convert.ToDateTime(obj.Record_Insert_DateTime), obj.Client_ID, 1, 0, obj.Branch_ID, 0, 1, 1, 0, context);
                        }
                        if (facematch == 1)
                        {
                            notification_icon = "wrong-id-upload.jpg";
                            notification_message = "<span class='cls-admin'>Face match failed.</span>";
                            CompanyInfo.save_notification_compliance(notification_message, notification_icon, Convert.ToString(Customer_ID), Convert.ToDateTime(obj.Record_Insert_DateTime), obj.Client_ID, 1, 0, obj.Branch_ID, 0, 1, 1, 0, context);
                        }

                        #endregion notific
                        CompanyInfo.InsertActivityLogDetails("Invalid Document found for journey " + journeyID + "", Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(Customer_ID), "GetJourney", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "ID Upload", context);
                        try
                        {
                            MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("UpdateCustIDScanResults");//Update bandtext and score
                            _cmd.CommandType = CommandType.StoredProcedure;
                            _cmd.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                            _cmd.Parameters.AddWithValue("_Customer_ID", Customer_ID);
                            _cmd.Parameters.AddWithValue("_Remark", "");
                            _cmd.Parameters.AddWithValue("_Score", 0);
                            int msg = db_connection.ExecuteNonQueryProcedure(_cmd);
                            _cmd.Dispose();
                        }
                        catch { }
                        MySqlConnector.MySqlCommand cmdupdate1 = new MySqlConnector.MySqlCommand("Get_Permissions");
                        cmdupdate1.CommandType = CommandType.StoredProcedure;
                        cmdupdate1.Parameters.AddWithValue("Per_ID", 75);// Validate IDSCan
                        cmdupdate1.Parameters.AddWithValue("ClientID", obj.Client_ID);
                        DataTable pm = db_connection.ExecuteQueryDataTableProcedure(cmdupdate1);
                        if (pm.Rows.Count > 0)
                        {
                            if (Convert.ToString(pm.Rows[0]["Status_ForCustomer"]) == "0")
                            {
                                dt.Rows.Add(1, "Invalid Document", obj.SenderID_ID);
                                return dt;
                            }
                        }
                    }

                    else if ((namevalidflag == 1 || obj.SenderNameOnID == "Unknown" || Remark == "3" || obj.SenderNameOnID == null || obj.SenderNameOnID == "" ||
                         obj.SenderID_ExpiryDate == null || obj.SenderID_ExpiryDate == "" || DocumentType == "Unknown" || DocumentType == "" || DocumentType == null || frontside == 1 || liveness == 1 || facematch == 1) && API_ID == 7)
                    {
                        #region notific
                        string notification_icon = "", notification_message = "";
                        if (obj.IDType_ID == 1 || obj.IDType_ID == 5)
                        {
                            notification_icon = "wrong-id-upload.jpg";
                            notification_message = "<span class='cls-admin'>Invalid <strong class='cls-priamary'>Primary ID</strong> document found on ID Scan.</span>";
                        }
                        CompanyInfo.save_notification_compliance(notification_message, notification_icon, Convert.ToString(Customer_ID), Convert.ToDateTime(obj.Record_Insert_DateTime), obj.Client_ID, 1, 0, obj.Branch_ID, 0, 1, 1, 0, context);
                        if (frontside == 1)
                        {
                            notification_icon = "wrong-id-upload.jpg";
                            notification_message = "<span class='cls-admin'>Front side match of <strong class='cls-priamary'>Primary ID</strong> failed.</span>";
                            CompanyInfo.save_notification_compliance(notification_message, notification_icon, Convert.ToString(Customer_ID), Convert.ToDateTime(obj.Record_Insert_DateTime), obj.Client_ID, 1, 0, obj.Branch_ID, 0, 1, 1, 0, context);
                        }
                        if (liveness == 1)
                        {
                            notification_icon = "wrong-id-upload.jpg";
                            notification_message = "<span class='cls-admin'>Liveness failed.</span>";
                            CompanyInfo.save_notification_compliance(notification_message, notification_icon, Convert.ToString(Customer_ID), Convert.ToDateTime(obj.Record_Insert_DateTime), obj.Client_ID, 1, 0, obj.Branch_ID, 0, 1, 1, 0, context);
                        }
                        if (facematch == 1)
                        {
                            notification_icon = "wrong-id-upload.jpg";
                            notification_message = "<span class='cls-admin'>Face match failed.</span>";
                            CompanyInfo.save_notification_compliance(notification_message, notification_icon, Convert.ToString(Customer_ID), Convert.ToDateTime(obj.Record_Insert_DateTime), obj.Client_ID, 1, 0, obj.Branch_ID, 0, 1, 1, 0, context);
                        }

                        #endregion notific
                        CompanyInfo.InsertActivityLogDetails("Invalid Document found for journey " + journeyID + "", Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(Customer_ID), "GetJourney", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "ID Upload", context);
                        try
                        {
                            MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("UpdateCustIDScanResults");//Update bandtext and score
                            _cmd.CommandType = CommandType.StoredProcedure;
                            _cmd.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                            _cmd.Parameters.AddWithValue("_Customer_ID", Customer_ID);
                            _cmd.Parameters.AddWithValue("_Remark", "");
                            _cmd.Parameters.AddWithValue("_Score", 0);
                            int msg = db_connection.ExecuteNonQueryProcedure(_cmd);
                            _cmd.Dispose();
                        }
                        catch { }
                        MySqlConnector.MySqlCommand cmdupdate1 = new MySqlConnector.MySqlCommand("Get_Permissions");
                        cmdupdate1.CommandType = CommandType.StoredProcedure;
                        cmdupdate1.Parameters.AddWithValue("Per_ID", 75);// Validate IDSCan
                        cmdupdate1.Parameters.AddWithValue("ClientID", obj.Client_ID);
                        DataTable pm = db_connection.ExecuteQueryDataTableProcedure(cmdupdate1);
                        if (pm.Rows.Count > 0)
                        {
                            if (Convert.ToString(pm.Rows[0]["Status_ForCustomer"]) == "0")
                            {
                                dt.Rows.Add(1, "Invalid Document", obj.SenderID_ID);
                                return dt;
                            }
                        }
                    }




                    //Return if document is expired
                    if ((obj.SenderID_ExpiryDate != "" && obj.SenderID_ExpiryDate != null) || (shuftipro_frontdoc_expired == 0 && API_ID == 5))
                    {
                        try
                        {
                            SID_Expiry = DateTime.ParseExact(obj.SenderID_ExpiryDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                            if (SID_Expiry.Date <= Convert.ToDateTime(obj.Record_Insert_DateTime).Date)
                            {
                                dt.Rows.Add(2, "Expired Document", obj.SenderID_ID);
                                return dt;
                            }
                        }
                        catch { }
                    }

                    //To save invalid document
                    if (obj.SenderNameOnID == "Unknown" || obj.SenderNameOnID == null || obj.SenderNameOnID == "")
                    {
                        obj.SenderNameOnID = (cname + " " + mname + " " + lname).Trim();
                    }

                    string Username = obj.CustomerName;
                    string url = Convert.ToString(ob["WhiteImageUrl"]);

                    if (API_ID == 5)
                    {
                        url = Convert.ToString(obj.shuftipro_frontdoc);
                    }
                    if (API_ID == 7)
                    {
                        url = Convert.ToString(veriffFrontDocURL);
                    }

                    if ((url != "" && url != null))
                    {

                        string path = "P-" + Customer_ID + "-" + obj.Record_Insert_DateTime.Replace(":", "") + ".jpg";
                        obj.FileNameWithExt = "assets/Uploads/" + path;//
                        using (var webClient = new WebClient())
                        {
                            try
                            {
                                webClient.Headers.Add("Content-Type", "application/json"); webClient.Headers.Add("Authorization", s13); webClient.Headers.Add("Cookie", "token " + s13);
                                byte[] imageBytes = webClient.DownloadData(url);
                                File.WriteAllBytes(Convert.ToString(dtc.Rows[0]["RootURL"]) + obj.FileNameWithExt + "", imageBytes);
                            }
                            catch { }
                        }

                        if (obj.SenderID_ExpiryDate != "" && obj.SenderID_ExpiryDate != null)
                        {
                            try
                            {
                                SID_Expiry = DateTime.ParseExact(obj.SenderID_ExpiryDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                                obj.SenderID_ExpiryDate = SID_Expiry.ToString("yyyy-MM-dd");
                            }
                            catch { }
                        }
                        else
                        {
                            DateTime date = DateTime.Now.AddMonths(3);
                            obj.SenderID_ExpiryDate = date.ToString("yyyy-MM-dd");
                        }

                        if (obj.Issue_date != "" && obj.Issue_date != null)
                        {
                            try
                            {
                                DateTime SID_Issuedate = DateTime.ParseExact(obj.Issue_date, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                                obj.Issue_date = SID_Issuedate.ToString("yyyy-MM-dd");
                            }
                            catch { }
                        }
                        else
                        {
                            DateTime date = DateTime.Now;
                            obj.Issue_date = date.ToString("yyyy-MM-dd");
                        }
                        if (obj.Sender_DateOfBirth != "" && obj.Sender_DateOfBirth != null)
                        {
                            try
                            {
                                SID_bdate = DateTime.ParseExact(obj.Sender_DateOfBirth, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                                obj.Sender_DateOfBirth = SID_bdate.ToString("yyyy-MM-dd");
                            }
                            catch { }
                        }
                        else
                        {
                            //DateTime bdt = DateTime.Now.AddYears(-18);
                            //obj.Sender_DateOfBirth = bdt.ToString("yyyy-MM-dd");
                            obj.Sender_DateOfBirth = string.Empty;
                        }
                        MySqlConnector.MySqlCommand _cmd1 = new MySqlConnector.MySqlCommand("GetIDNames");
                        _cmd1.CommandType = CommandType.StoredProcedure;
                        _cmd1.Parameters.AddWithValue("Client_ID", obj.Client_ID);
                        _cmd1.Parameters.AddWithValue("clause", " and im.IDType_ID=" + obj.IDType_ID + " and ID_Name like '%" + DocumentType + "%'");
                        DataTable d = db_connection.ExecuteQueryDataTableProcedure(_cmd1);
                        obj.IDName_ID = 1;
                        if (d.Rows.Count > 0)
                        {
                            string idname = Convert.ToString(d.Rows[0]["IDName_ID"]);
                            if (idname != null && idname != "" && idname != "0")
                                obj.IDName_ID = Convert.ToInt32(idname);
                        }
                        string IDScanpdf = string.Empty;
                        int pdfdownload = 1;
                        try
                        {
                            if (API_ID == 3)
                            {
                                IDScanpdf = "assets/Other_Docs/" + "PDF-" + Customer_ID + "-" + obj.Record_Insert_DateTime.Replace(":", "") + ".zip";//Convert.ToString(dtc.Rows[0]["RootURL"]) assets/Upload_Profile_Images/
                                using (var webClient = new WebClient())
                                {
                                    webClient.Headers.Add("Content-Type", "application/json"); webClient.Headers.Add("Authorization", s13); webClient.Headers.Add("Cookie", "token " + s13);
                                    byte[] imageBytes = webClient.DownloadData("" + urll + "reporting/ExportJourneyReports?evaluatedPersonEntryIds=['" + journeyID + "']");
                                    //File.WriteAllBytes(Convert.ToString(dtc.Rows[0]["RootURL"]) + IDScanpdf, imageBytes);
                                    FileStream fs = new FileStream(@"" + Convert.ToString(dtc.Rows[0]["RootURL"]) + "" + IDScanpdf + "", FileMode.OpenOrCreate);
                                    fs.Write(imageBytes, 0, imageBytes.Length);
                                    fs.Close();

                                    //using (var compressedStream = new MemoryStream(imageBytes))
                                    //using (var zipStream = new System.IO.Compression.GZipStream(compressedStream, System.IO.Compression.CompressionMode.Decompress))
                                    //using (var resultStream = new MemoryStream())
                                    //{
                                    //    zipStream.CopyTo(resultStream);
                                    //    var x = resultStream.ToArray();
                                    //}
                                    //byte[] decompressedBytes = new byte[imageBytes.Length];
                                    //using (FileStream fileToDecompress = File.Open("" + Convert.ToString(dtc.Rows[0]["RootURL"]) + "" + IDScanpdf + "", FileMode.Open))
                                    //{
                                    //    using (System.IO.Compression.GZipStream decompressionStream = new System.IO.Compression.GZipStream(fileToDecompress, System.IO.Compression.CompressionMode.Decompress))
                                    //    {
                                    //        decompressionStream.Read(decompressedBytes, 0, imageBytes.Length);
                                    //    }
                                    //}

                                    pdfdownload = 0;
                                    string Activity = "Generated GBG ID scan PDF for <b>" + Username + "</b>";
                                    int stattus = (int)CompanyInfo.InsertActivityLogDetails(Activity, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(Customer_ID), "GetJourney", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "ID Upload", context);

                                }
                            }
                        }
                        catch { }
                        obj.Comments = "ID Scan";
                        using (MySqlConnector.MySqlConnection con = new MySqlConnector.MySqlConnection(_dbConnection.ConnectionStringSection()))
                        {
                            if (con.State != ConnectionState.Open)
                                con.Open();

                            MySqlConnector.MySqlTransaction transaction;
                            transaction = con.BeginTransaction(IsolationLevel.ReadCommitted);

                            using (MySqlConnector.MySqlCommand cmd = new MySqlConnector.MySqlCommand("Customer_InsertPID", con))
                            {
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Transaction = transaction;
                                cmd.CommandTimeout = 200;
                                if (DateTime.TryParse(obj.SenderID_ExpiryDate, out DateTime Temp) == false)
                                {
                                    DateTime date = DateTime.Now.AddMonths(3);
                                    obj.SenderID_ExpiryDate = date.ToString("yyyy-MM-dd");
                                }
                                if (DateTime.TryParse(obj.Issue_date, out DateTime Temp2) == false)
                                {
                                    DateTime date = DateTime.Now;
                                    obj.Issue_date = date.ToString("yyyy-MM-dd");
                                }
                                if (DateTime.TryParse(obj.Sender_DateOfBirth, out DateTime Temp3) == false)
                                {
                                    obj.Sender_DateOfBirth = string.Empty;
                                }

                                cmd.Parameters.AddWithValue("_SenderNameOnID", obj.SenderNameOnID);
                                cmd.Parameters.AddWithValue("_IDType_ID", obj.IDType_ID);
                                cmd.Parameters.AddWithValue("_SenderID_Number", obj.SenderID_Number);
                                cmd.Parameters.AddWithValue("_SenderID_ExpiryDate", obj.SenderID_ExpiryDate);
                                cmd.Parameters.AddWithValue("_SenderID_PlaceOfIssue", obj.SenderID_PlaceOfIssue);
                                if (obj.Sender_DateOfBirth == null || obj.Sender_DateOfBirth == "")
                                    cmd.Parameters.Add("_Sender_DateOfBirth", (DbType)SqlDbType.DateTime).Value = DBNull.Value;
                                else
                                    cmd.Parameters.AddWithValue("_Sender_DateOfBirth", obj.Sender_DateOfBirth);

                                cmd.Parameters.AddWithValue("_Customer_ID", Customer_ID);
                                cmd.Parameters.AddWithValue("_IDName_ID", obj.IDName_ID);
                                cmd.Parameters.AddWithValue("_Record_Insert_DateTime", obj.Record_Insert_DateTime);
                                cmd.Parameters.AddWithValue("_Delete_Status", obj.Delete_Status);
                                cmd.Parameters.AddWithValue("_User_ID", obj.User_ID);
                                cmd.Parameters.AddWithValue("_documents_details_Id", obj.IDType_ID);
                                cmd.Parameters.AddWithValue("_CB_ID", obj.Branch_ID);
                                cmd.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                                cmd.Parameters.AddWithValue("_Doc_Name", obj.DocumentName);
                                cmd.Parameters.AddWithValue("_MRZ_number", idDocMrzLine2);
                                cmd.Parameters.AddWithValue("_comments", obj.Comments);
                                cmd.Parameters.AddWithValue("_FileNameWithExt", obj.FileNameWithExt);
                                cmd.Parameters.AddWithValue("_Benf_ID", obj.Beneficiary_ID);
                                cmd.Parameters.AddWithValue("_issue_date", obj.Issue_date);
                                cmd.Parameters.AddWithValue("_BackID_Document", "");
                                cmd.Parameters.AddWithValue("_JourneyID", journeyID.ToString());
                                cmd.Parameters.AddWithValue("_PDFGenerate_Status", pdfdownload);//default 1 - means not yet downloaded
                                cmd.Parameters.AddWithValue("_PDF_FileName", IDScanpdf);

                                cmd.Parameters.AddWithValue("_FrontResult", frontside);
                                cmd.Parameters.AddWithValue("_LivenessResult", liveness);
                                cmd.Parameters.AddWithValue("_FaceMatchResult", facematch);
                                cmd.Parameters.Add(new MySqlConnector.MySqlParameter("_SenderID_ID", MySqlConnector.MySqlDbType.Int32));
                                cmd.Parameters["_SenderID_ID"].Direction = ParameterDirection.Output;

                                obj.SenderID_ID = 0;


                                object result = cmd.ExecuteNonQuery();
                                try
                                {
                                    obj.SenderID_ID = Convert.ToInt32(cmd.Parameters["_SenderID_ID"].Value);
                                }
                                catch (Exception egx) { }
                                result = (result == DBNull.Value) ? null : result;
                                //        obj.SenderID_ID = Convert.ToInt32(result);
                                int res = Convert.ToInt32(result);
                                dt.Rows.Add(0, "Success", obj.SenderID_ID);
                                cmd.Dispose();
                                string Activity = string.Empty;
                                string idtype = string.Empty;


                                //  context.Session["Username"] = Username;
                                string country_details = "Document Details: ";

                                if (obj.IDType_ID != null && obj.IDType_ID != -1)
                                {
                                    if (obj.IDType_ID == 1 || obj.IDType_ID == 5)
                                    {
                                        idtype = "Primary";
                                    }
                                    if (obj.IDType_ID == 2)
                                    {
                                        idtype = "Secondary";
                                    }
                                    if (obj.IDType_ID == 3)
                                    {
                                        idtype = "Other Documents";
                                    }
                                    if (obj.IDType_ID == 4)
                                    {
                                        idtype = "Source Of Funds";
                                    }
                                    country_details = country_details + " Id Type: " + idtype;
                                }
                                if (obj.SenderNameOnID != null && obj.SenderNameOnID != "")
                                {
                                    country_details = country_details + " Name on ID: " + obj.SenderNameOnID;
                                }
                                if (obj.Sender_DateOfBirth != null && obj.Sender_DateOfBirth != "")
                                {
                                    country_details = country_details + " Date of Birth: " + obj.Sender_DateOfBirth;
                                }
                                if (obj.IDName_ID != null && obj.IDName_ID != -1)
                                {
                                    country_details = country_details + " ID: " + DocumentType;
                                }
                                if (obj.SenderID_Number != null && obj.SenderID_Number != "")
                                {
                                    country_details = country_details + "<br/> ID Number: " + obj.SenderID_Number;
                                }
                                if (obj.SenderID_ExpiryDate != null && obj.SenderID_ExpiryDate != "")
                                {
                                    country_details = country_details + " Expiry Date: " + obj.SenderID_ExpiryDate;
                                }
                                if (obj.SenderID_PlaceOfIssue != null && obj.SenderID_PlaceOfIssue != "")
                                {
                                    country_details = country_details + " Place of Issue: " + obj.SenderID_PlaceOfIssue;
                                }

                                if (res > 0 && obj.scanstatus != "INPROGRESS")
                                {
                                    string risk_factors = "2,4,5,6,7,8,10,13";
                                    CompanyInfo.add_check_rules(Customer_ID, risk_factors, obj.Branch_ID, obj.Client_ID, context);

                                    string notification_icon = "", notification_message = "";
                                    if (obj.IDType_ID == 1 || obj.IDType_ID == 5)
                                    {
                                        notification_icon = "primary-id-upload.jpg";
                                        notification_message = "<span class='cls-admin'>successfully uploaded <strong class='cls-priamary'>Primary ID</strong>.</span>";
                                    }
                                    if (obj.IDType_ID == 2)
                                    {
                                        notification_icon = "secondary-id-upload.jpg";
                                        notification_message = "<span class='cls-admin'>successfully uploaded <strong class='cls-secondary'>Secondary ID</strong>.</span>";
                                    }
                                    if (obj.IDType_ID == 3)
                                    {
                                        notification_icon = "other-doc.jpg";
                                        notification_message = "<span class='cls-admin'>successfully uploaded <strong class='cls-otherdoc'>Other document</strong>.</span>";
                                    }
                                    if (obj.IDType_ID == 4)
                                    {
                                        notification_icon = "sof.jpg";
                                        notification_message = "<span class='cls-admin'>successfully uploaded <strong class='cls-srcfund'>Source of fund document</strong>.</span>";
                                    }
                                    CompanyInfo.save_notification(notification_message, notification_icon, Customer_ID, Convert.ToDateTime(obj.Record_Insert_DateTime), obj.Client_ID, 1, 0, obj.Branch_ID, 0, 1, 1, 0, context);

                                    Activity = "<b>" + Username + "</b> scanned ID and checked Liveness. ID Document Details: " + country_details + "</br>";
                                    int stattus = (int)CompanyInfo.InsertActivityLogDetails(Activity, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(Customer_ID), "GetJourney", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "ID Upload", context);

                                    MySqlConnector.MySqlCommand cmd3 = new MySqlConnector.MySqlCommand("customer_details_by_param");
                                    cmd3.CommandType = CommandType.StoredProcedure;
                                    string _whereclause = " and cr.Client_ID=" + obj.Client_ID;
                                    if (Customer_ID > 0)
                                    {
                                        _whereclause = " and cr.Customer_ID=" + Customer_ID;
                                    }
                                    cmd3.Parameters.AddWithValue("_whereclause", _whereclause);
                                    cmd3.Parameters.AddWithValue("_SecurityKey", CompanyInfo.SecurityKey());
                                    DataTable dt_cust = db_connection.ExecuteQueryDataTableProcedure(cmd3);
                                    if (IsIdvalid != 1 && API_ID == 3)
                                    {
                                        //Check Credit Safe
                                        string[] response = CheckCreditSafe(obj, dt_cust, "ID Upload", context);
                                        string scorecount = Convert.ToString(dtc.Rows[0]["FaceScore_ValidFrom"]);
                                        if (scorecount == null || scorecount == "")
                                        {
                                            scorecount = "0";
                                        }
                                        if (response != null && score >= Convert.ToInt32(scorecount) && frontside != 1)
                                        {
                                            if (response.Length == 4)
                                            {
                                                if (response[2] == "0" && response[3] == "0")
                                                {
                                                    //Auto verifiy ID document on perm 125
                                                    MySqlConnector.MySqlCommand cmdp1 = new MySqlConnector.MySqlCommand("AutoVerify_IDdoc");
                                                    cmdp1.CommandType = CommandType.StoredProcedure;
                                                    cmdp1.Parameters.AddWithValue("_UserName", "Auto Verified");
                                                    cmdp1.Parameters.AddWithValue("_status", 0);
                                                    cmdp1.Parameters.AddWithValue("_Record_Insert_DateTime", obj.Record_Insert_DateTime);
                                                    cmdp1.Parameters.AddWithValue("_SenderID_ID", obj.SenderID_ID);
                                                    try
                                                    {
                                                        int dr = db_connection.ExecuteNonQueryProcedure(cmdp1);
                                                        if (dr > 0)
                                                        {
                                                            //string Activity1 = "Updated profile image of <b>" + Username + "</b>";
                                                            //int stattus1 = (int)CompanyInfo.InsertActivityLogDetails(Activity1, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(Customer_ID), "GetJourney", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "ID Upload");
                                                            string EmailUrl = "", Company_Name = "";
                                                            if (dtc.Rows.Count > 0)
                                                            {
                                                                EmailUrl = Convert.ToString(dtc.Rows[0]["Company_URL_Admin"]);
                                                                Company_Name = Convert.ToString(dtc.Rows[0]["Company_Name"]);
                                                            }

                                                            string email = Convert.ToString(dt_cust.Rows[0]["Email_ID"]);
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
                                                            var message = "Your account has been fully verified and ready for use.";

                                                            body = body.Replace("[name]", Convert.ToString(dt_cust.Rows[0]["Full_name"]));
                                                            body = body.Replace("[msg]", message);

                                                            subject = "" + Company_Name + " - ID Verification - " + Convert.ToString(dt_cust.Rows[0]["WireTransfer_ReferanceNo"]);

                                                            string mail_send = (string)CompanyInfo.Send_Mail(dtc, email, body, subject, obj.Client_ID, obj.Branch_ID, "", "", "", context);
                                                        }
                                                    }
                                                    catch { }
                                                }
                                            }
                                        }
                                    }
                                    string full_name = dt_cust.Rows[0]["Full_name"].ToString();
                                    string email_id = dt_cust.Rows[0]["Email_ID"].ToString();
                                    string referno = Convert.ToString(dt_cust.Rows[0]["WireTransfer_ReferanceNo"]);

                                    if (con.State != ConnectionState.Open)
                                        con.Open();

                                    MySqlConnector.MySqlCommand cmdupdate1 = new MySqlConnector.MySqlCommand("SP_Get_Email_Permission", con);
                                    cmdupdate1.CommandType = CommandType.StoredProcedure;
                                    cmdupdate1.Parameters.AddWithValue("_ID", 2);
                                    cmdupdate1.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                                    string permission_status = string.Empty;
                                    DataTable dt1 = db_connection.ExecuteQueryDataTableProcedure(cmdupdate1);

                                    if (dt1.Rows.Count > 0)
                                    {
                                        permission_status = Convert.ToString(dt1.Rows[0]["Status_ForCustomer"]);
                                    }
                                    if (permission_status == "0")//send mail
                                    {
                                        string get_email_status = send_mails(dtc, obj.Client_ID, obj.Branch_ID, Convert.ToString(dt_cust.Rows[0]["First_Name"]), referno, "Thank you. Your ID Scan and Liveness check is complete.", email_id, context);
                                    }
                                }
                                else
                                {
                                    Activity = "<b>" + Username + "</b>" + " failed to add new ID Document. " + country_details + "</br>";
                                    try
                                    {
                                        string stattus = (string)CompanyInfo.InsertActivityLogDetails(Activity, obj.User_ID, 0, obj.User_ID, Customer_ID, "GetJourney", obj.Branch_ID, obj.Client_ID, "ID Upload", context);
                                    }
                                    catch (Exception egx) { }
                                }
                                transaction.Commit();
                            }
                            if (con.State != ConnectionState.Closed)
                                con.Close();
                        }
                    }

                    if (API_ID == 3)
                    {
                        url = Convert.ToString(ob["SelfiePhotoImageUrl"]);
                    }
                    if (API_ID == 5)
                    {
                        url = Convert.ToString(obj.shuftipro_face);
                    }
                    if (API_ID == 7)
                    {
                        url = Convert.ToString(veriffFaceURL);
                    }

                    if ((url != "" && url != null) && (API_ID == 3 || API_ID == 5))
                    {
                        try
                        {
                            obj.Record_Insert_DateTime = CompanyInfo.gettime(obj.Client_ID, Customer_ID.ToString(), obj.Country_ID, context);
                            string path = Customer_ID + "-" + obj.Record_Insert_DateTime.Replace(":", "") + ".jpg";
                            obj.FileNameWithExt = "assets/Upload_Profile_Images/" + path;//Convert.ToString(dtc.Rows[0]["RootURL"]) assets/Upload_Profile_Images/
                            using (var webClient = new WebClient())
                            {
                                webClient.Headers.Add("Content-Type", "application/json"); webClient.Headers.Add("Authorization", s13); webClient.Headers.Add("Cookie", "token " + s13);
                                byte[] imageBytes = webClient.DownloadData(url);
                                File.WriteAllBytes(Convert.ToString(dtc.Rows[0]["RootURL"]) + obj.FileNameWithExt, imageBytes);
                                MySqlConnector.MySqlCommand _cmd1 = new MySqlConnector.MySqlCommand("UpdateProfile_Image");
                                _cmd1.CommandType = CommandType.StoredProcedure;
                                _cmd1.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                                _cmd1.Parameters.AddWithValue("_Customer_ID", Customer_ID);
                                _cmd1.Parameters.AddWithValue("_FileNameWithExt", obj.FileNameWithExt);
                                int d = db_connection.ExecuteNonQueryProcedure(_cmd1);
                                if (d > 0)
                                {
                                    string Activity = "Updated profile image of <b>" + Username + "</b>";
                                    int stattus = (int)CompanyInfo.InsertActivityLogDetails(Activity, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(Customer_ID), "GetJourney", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "ID Upload", context);
                                }
                            }
                        }
                        catch { }
                    }
                    else
                    {
                        url = Convert.ToString(ob["FacePhotoImageUrl"]);
                        if (url != "" && url != null)
                        {
                            try
                            {
                                obj.Record_Insert_DateTime = CompanyInfo.gettime(obj.Client_ID, Customer_ID.ToString(), obj.Country_ID, context);
                                string path = Customer_ID + "-" + obj.Record_Insert_DateTime.Replace(":", "") + ".jpg";
                                obj.FileNameWithExt = "assets/Upload_Profile_Images/" + path;//Convert.ToString(dtc.Rows[0]["RootURL"]) assets/Upload_Profile_Images/
                                using (var webClient = new WebClient())
                                {
                                    webClient.Headers.Add("Content-Type", "application/json"); webClient.Headers.Add("Authorization", s13); webClient.Headers.Add("Cookie", "token " + s13);
                                    byte[] imageBytes = webClient.DownloadData(url);
                                    File.WriteAllBytes(Convert.ToString(dtc.Rows[0]["RootURL"]) + obj.FileNameWithExt, imageBytes);
                                    MySqlConnector.MySqlCommand _cmd1 = new MySqlConnector.MySqlCommand("UpdateProfile_Image");
                                    _cmd1.CommandType = CommandType.StoredProcedure;
                                    _cmd1.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                                    _cmd1.Parameters.AddWithValue("_Customer_ID", Customer_ID);
                                    _cmd1.Parameters.AddWithValue("_FileNameWithExt", obj.FileNameWithExt);
                                    int d = db_connection.ExecuteNonQueryProcedure(_cmd1);
                                    if (d > 0)
                                    {
                                        string Activity = "Updated profile image of <b>" + Username + "</b>";
                                        int stattus = (int)CompanyInfo.InsertActivityLogDetails(Activity, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(Customer_ID), "GetJourney", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "ID Upload", context);
                                    }
                                }
                            }
                            catch { }
                        }
                    }

                }
                if (dt.Rows.Count <= 0)
                    dt.Rows.Add(1, response1.StatusCode, obj.SenderID_ID);

            }
            catch (Exception e)
            {
                string Activity = "ID Scan Error Details: " + e.ToString() + "</br>";
                int stattus = (int)CompanyInfo.InsertActivityLogDetails(Activity, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(Customer_ID), "GetJourney", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "ID Upload", context);

                dt.Rows.Add(2, e.ToString(), obj.SenderID_ID);
            }
            return dt;
        }


        public string GetFileExtension(string contentType)
        {
            switch (contentType)
            {
                case "image/jpeg":
                case "image/jpg":
                    return ".jpg";
                case "image/png":
                    return ".png";
                case "application/pdf":
                    return ".pdf";
                case "video/mp4":
                    return ".mp4";
                case "video/webm":
                    return ".webm";
                case "video/quicktime":
                    return ".mov";
                default:
                    return string.Empty;
            }
        }

        public int GetFaceMatchScore(string token, string urll, Model.Document obj)
        {
            int score = 0;
            try
            {
                int API_ID = 3; string userAuthenticationURI = "" + urll + "journey/get";

                var client = new RestClient(userAuthenticationURI + "?journeyID=" + obj.JourneyID + "");
                client.Timeout = -1;
                var request = new RestRequest(Method.GET);
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("Authorization", token);
                request.AddHeader("Cookie", "token " + token); // Extra Parameter for DataField "scanningToken=" + 
                string CallFrom = "Journey Summary";
                string Req = "journeyID=" + obj.JourneyID + " Customer_ID=" + obj.Customer_ID + "";
                try
                {
                    MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("SaveAPIRequestResponce");
                    _cmd.CommandType = CommandType.StoredProcedure;
                    _cmd.Parameters.AddWithValue("_API_ID", API_ID);
                    _cmd.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                    _cmd.Parameters.AddWithValue("_Customer_ID", obj.Customer_ID);
                    _cmd.Parameters.AddWithValue("_status", 0);
                    _cmd.Parameters.AddWithValue("_Function_name", CallFrom);
                    _cmd.Parameters.AddWithValue("_Remark", 0);
                    _cmd.Parameters.AddWithValue("_comments", Req);
                    _cmd.Parameters.AddWithValue("_Record_Insert_DateTime", obj.Record_Insert_DateTime);
                    _cmd.Parameters.AddWithValue("_Branch_ID", obj.Branch_ID);
                    int msg1 = db_connection.ExecuteNonQueryProcedure(_cmd);
                }
                catch (Exception ex)
                {
                    string error = ex.ToString().Replace("\'", "\\'");
                    MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("SaveException");
                    _cmd.CommandType = CommandType.StoredProcedure;
                    _cmd.Parameters.AddWithValue("_Record_Insert_DateTime", obj.Record_Insert_DateTime);
                    _cmd.Parameters.AddWithValue("_error", error);
                    _cmd.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                    int msg1 = db_connection.ExecuteNonQueryProcedure(_cmd);
                }
                IRestResponse response1 = client.Execute(request);
                string resresult = response1.Content;

                try
                {
                    if (resresult != "")
                    {
                        int Status = 1;
                        string Bandtext = "", saveresp = "";
                        if (resresult != "")
                        {
                            Newtonsoft.Json.Linq.JObject ob = Newtonsoft.Json.Linq.JObject.Parse(resresult);
                            Bandtext = Convert.ToString(ob["HighLevelResult"]);
                            saveresp = System.Text.Encoding.ASCII.GetString(System.Text.Encoding.GetEncoding("ISO-8859-8").GetBytes(resresult));
                        }
                        string Remark = Convert.ToString(CompanyInfo.getAPIStatus(Bandtext, obj.Client_ID));
                        if (Remark != null && Remark != "")
                            Status = 0;

                        MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("SaveAPIRequestResponce");
                        _cmd.CommandType = CommandType.StoredProcedure;
                        _cmd.Parameters.AddWithValue("_API_ID", API_ID);
                        _cmd.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                        _cmd.Parameters.AddWithValue("_Customer_ID", obj.Customer_ID);
                        _cmd.Parameters.AddWithValue("_status", 1);
                        _cmd.Parameters.AddWithValue("_Function_name", CallFrom);
                        _cmd.Parameters.AddWithValue("_Remark", Remark);
                        _cmd.Parameters.AddWithValue("_comments", saveresp.Replace("\"", ""));
                        _cmd.Parameters.AddWithValue("_Record_Insert_DateTime", obj.Record_Insert_DateTime);
                        _cmd.Parameters.AddWithValue("_Branch_ID", obj.Branch_ID);
                        int msg1 = db_connection.ExecuteNonQueryProcedure(_cmd);

                        if (response1.StatusCode.ToString() == "OK")
                        {
                            string formattedstr = saveresp;// System.Text.Encoding.ASCII.GetString(System.Text.Encoding.GetEncoding("ISO-8859-8").GetBytes(resresult));
                            Newtonsoft.Json.Linq.JObject ob = Newtonsoft.Json.Linq.JObject.Parse(formattedstr);
                            var arr = ob["ProcessedDocuments"];

                            for (var i = 0; i < arr.Count(); i++)
                            {
                                string name = Convert.ToString(arr[i]["DocumentCategory"]);
                                if (name == "Selfie")
                                {
                                    string no = Convert.ToString(arr[i]["FaceMatchConfidenceScore"]);
                                    if (no != null && no != "")
                                    {
                                        decimal scoreno = Convert.ToDecimal(no);
                                        score = Convert.ToInt32(scoreno * 100);
                                        return score;// finalscore;
                                    }
                                }
                            }
                        }
                    }
                }
                catch { }
            }
            catch { }
            return score;
        }

        public static DataTable gbgqrcode(string qrapiurl, string s13, string journeyid, string env, string lang, string method, string custNo, HttpContext context, int clientId, string idscan_defaultinput, string redirecturl, string fileselection)
        {
            WebResponse response2 = null;
            DataTable dt = new DataTable();
            dt.Columns.Add("qrcode", typeof(string));
            dt.Columns.Add("redirecturl", typeof(string));
            dt.Columns.Add("uniquevalue", typeof(string));
            try
            {
                DateTime now = DateTime.Now;
                long uniqueNumberLong = Convert.ToInt64(now.ToString("yyyyMMddHHmmss"));
                try
                {
                    MySqlConnector.MySqlCommand cmd = new MySqlConnector.MySqlCommand("thirdparty_mtbs_transaction_table");
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("p_api_id", 3);
                    cmd.Parameters.AddWithValue("p_customer_id", custNo);
                    cmd.Parameters.AddWithValue("p_client_id", clientId);
                    cmd.Parameters.AddWithValue("p_Customer_API_ID", null);
                    cmd.Parameters.AddWithValue("p_Beneficary_API_ID", null);
                    cmd.Parameters.AddWithValue("p_Beneficary_ID", null);
                    cmd.Parameters.AddWithValue("p_TransactionReference", uniqueNumberLong);
                    cmd.Parameters.AddWithValue("p_MtbsTransactionTrackId", null);
                    DataTable dtmtbsdetails = db_connection.ExecuteQueryDataTableProcedure(cmd);

                    CompanyInfo.InsertrequestLogTracker("gbgqrcode parameter : " + custNo + " ," + clientId + " ," + uniqueNumberLong, 0, 0, 0, 0, "srvDocument", Convert.ToInt32(0), Convert.ToInt32(0), "", context);
                }
                catch (Exception ex)
                {
                    int stattusj = (int)CompanyInfo.InsertActivityLogDetailsSecurity("gbgqrcode thirdparty_mtbs_transaction_table error : " + ex.ToString(), Convert.ToInt32(0), 0, Convert.ToInt32(0), 1, "srvDocument", Convert.ToInt32(0), Convert.ToInt32(0), "gbgqrcode", 0, context);
                    CompanyInfo.InsertrequestLogTracker("gbgqrcode thirdparty_mtbs_transaction_table error : " + ex.ToString(), 0, 0, 0, 0, "srvDocument", Convert.ToInt32(0), Convert.ToInt32(0), "", context);
                }

                if (idscan_defaultinput != "FILESYSTEM")
                {
                    idscan_defaultinput = "CAMERA";
                }

                string fileUploadToggle = "false";

                qrapiurl = qrapiurl + "?journeyDef=" + journeyid + "&token=" + s13 + "&env=" + env + "&custNo=" + custNo + "&lang=" + lang + "&method=" + method;
                int stattus = (int)CompanyInfo.InsertActivityLogDetailsSecurity("gbgqrcode qrapiurl url : " + qrapiurl, Convert.ToInt32(0), 0, Convert.ToInt32(0), 1, "srvDocument", Convert.ToInt32(0), Convert.ToInt32(0), "gbgqrcode", 0, context);
                var client = new RestClient(qrapiurl);
                var request = new RestRequest();
                request.Method = Method.POST;
                request.AddHeader("Content-Type", "application/json");

                /*var body = @"{" + "\n" +
                            @"    ""OrganisationName"": """"," + "\n" +
                            @"    ""CustomText"": """"," + "\n" +
                            @"    ""CustomJourneyID"": """"," + "\n" +
                            @"    ""AdditionalDataField1"": " + custNostr + "," + "\n" +
                            @"    ""AdditionalDataField2"": " + uniqueNumberLongste + "," + "\n" +
                            @"    ""AdditionalDataField3"": """"," + "\n" +                            
                            @"    ""RedirectURL"": ""#""," + "\n" +
                            @"    ""RedirectLevel"": ""top""," + "\n" +
                            @"    ""ManualCaptureToggle"": ""true""," + "\n" +
                            @"    ""FileUploadToggle"": "+ fileUploadToggle + "" + "\n" +
                            @"    " + "\n" +
                            @"    " + "\n" +
                            @"}";*/

                string uniqueNumberLongste = "\"" + uniqueNumberLong + "\"";
                string custNostr = "\"" + Convert.ToString(custNo) + "\"";
                string attempts = "\"" + Convert.ToString(50) + "\"";
                redirecturl = "\"" + redirecturl + "\"";
                //fileUploadToggle = "\"" + "false" + "\"";
                //fileUploadToggle = "\"" + "true" + "\"";

                try
                {
                    if (fileselection == "T")
                    {
                        fileUploadToggle = "\"" + "true" + "\"";
                    }
                    else if (fileselection == "F")
                    {
                        fileUploadToggle = "\"" + "false" + "\"";
                    }
                }
                catch (Exception Ex)
                {
                    fileUploadToggle = "\"" + "false" + "\"";
                }


                var body = @"{" + "\n" +
                            @"    ""OrganisationName"": """"," + "\n" +
                            @"    ""CustomText"": """"," + "\n" +
                            @"    ""CustomJourneyID"": """"," + "\n" +
                            @"    ""AdditionalDataField1"": " + custNostr + "," + "\n" +
                            @"    ""AdditionalDataField2"": " + uniqueNumberLongste + "," + "\n" +
                            @"    ""AdditionalDataField3"": """"," + "\n" +
                            @"    ""RedirectURL"": " + redirecturl + "," + "\n" +
                            @"    ""RedirectLevel"": ""top""," + "\n" +
                            @"    ""Attempts"": " + attempts + "," + "\n" +
                            @"    ""ManualCaptureToggle"": ""true""," + "\n" +
                            @"    ""FileUploadToggle"": " + fileUploadToggle + "" + "\n" +
                            @"    " + "\n" +
                            @"    " + "\n" +
                            @"}";

                request.AddParameter("application/json", body, ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);
                stattus = (int)CompanyInfo.InsertActivityLogDetailsSecurity("gbgqrcode request body : " + body, Convert.ToInt32(0), 0, Convert.ToInt32(0), 1, "srvDocument", Convert.ToInt32(0), Convert.ToInt32(0), "gbgqrcode", 0, context);
                CompanyInfo.InsertrequestLogTracker("gbgqrcode request body : " + body, 0, 0, 0, 0, "srvDocument", Convert.ToInt32(0), Convert.ToInt32(0), "", context);
                stattus = (int)CompanyInfo.InsertActivityLogDetailsSecurity("gbgqrcode response : " + response.Content, Convert.ToInt32(0), 0, Convert.ToInt32(0), 1, "srvDocument", Convert.ToInt32(0), Convert.ToInt32(0), "gbgqrcode", 0, context);
                CompanyInfo.InsertrequestLogTracker("gbgqrcoderesponse: " + response.Content, 0, 0, 0, 0, "srvDocument", Convert.ToInt32(0), Convert.ToInt32(0), "", context);
                var obj11 = Newtonsoft.Json.Linq.JObject.Parse(response.Content);
                var StatusCode = Convert.ToString(obj11["success"]).ToLower();
                if (StatusCode == "true")
                {
                    dt.Rows.Add(Convert.ToString(obj11["data"]["QR_Code"]), Convert.ToString(obj11["data"]["URL"]), uniqueNumberLong);
                }
            }
            catch (Exception ex)
            {
                string error = ex.ToString().Replace("\'", "\\'");
                MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("SaveException");
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.Parameters.AddWithValue("_Record_Insert_DateTime", DateTime.Now);
                _cmd.Parameters.AddWithValue("_error", error);
                _cmd.Parameters.AddWithValue("_Client_ID", 1);
                int msg1 = db_connection.ExecuteNonQueryProcedure(_cmd);
            }
            return dt;
        }


        public WebResponse Token(string urll, string UserName, string Password, HttpContext context)
        {
            WebResponse response2 = null;
            try
            {
                //var urll = "https://poc.idscan.cloud/idscanenterprisesvc/";// test
                //var urll = "https://prod.idscan.cloud/idscanenterprisesvc/";//live
                var _url = Convert.ToString("" + urll + "token");
                //var UserName = Convert.ToString("CSS_Super");//test
                //var Password = Convert.ToString("Pbbhs7ZPnQTn");
                //var UserName = Convert.ToString("Sangerwal_Capture");// live
                //var Password = Convert.ToString("4J}{xGL`}sqf");
                var Area = "investigation";// "scanning";
                var grant_type = "password";

                // ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
                       | SecurityProtocolType.Tls11
                       | SecurityProtocolType.Tls12;

                string data = "UserName=" + UserName + "&Password=" + System.Uri.EscapeDataString(Password) + "&area=" + Area + "&grant_type=" + grant_type + "";
                byte[] dataStream = Encoding.UTF8.GetBytes(data);
                CompanyInfo.InsertrequestLogTracker("gbg Token url  : " + _url, 0, 0, 0, 0, "srvDocument", Convert.ToInt32(0), Convert.ToInt32(0), "", context);
                string userAuthenticationURI = _url;
                if (!string.IsNullOrEmpty(userAuthenticationURI))
                {
                    HttpWebRequest request1 = (HttpWebRequest)WebRequest.Create(userAuthenticationURI);
                    request1.Method = "POST";
                    request1.ContentType = "application/x-www-form-urlencoded";
                    Stream newStream = request1.GetRequestStream();
                    newStream.Write(dataStream, 0, dataStream.Length);
                    newStream.Close();
                    response2 = request1.GetResponse();
                }
            }

            catch (Exception ex)
            {
                CompanyInfo.InsertrequestLogTracker("gbg Token error : " + ex.ToString(), 0, 0, 0, 0, "srvDocument", Convert.ToInt32(0), Convert.ToInt32(0), "", context);
                string error = ex.ToString().Replace("\'", "\\'");
                MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("SaveException");
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.Parameters.AddWithValue("_Record_Insert_DateTime", DateTime.Now);
                _cmd.Parameters.AddWithValue("_error", error);
                _cmd.Parameters.AddWithValue("_Client_ID", 1);
                int msg1 = db_connection.ExecuteNonQueryProcedure(_cmd);
            }
            CompanyInfo.InsertrequestLogTracker("gbg Token body : " + response2, 0, 0, 0, 0, "srvDocument", Convert.ToInt32(0), Convert.ToInt32(0), "", context);
            return response2;


        }





        public DataTable GetIDNames(Model.Document obj, HttpContext context)
        {
            DataTable dt = new DataTable();
            string Client_ID_regex = CompanyInfo.testInjection(Convert.ToString(obj.Client_ID));
            string IDType_ID_regex = CompanyInfo.testInjection(Convert.ToString(obj.IDType_ID));
            string Country_ID_regex = CompanyInfo.testInjection(Convert.ToString(obj.Country_ID));
            string Client_ID1_regex = validation.validate(Convert.ToString(obj.Client_ID), 0, 1, 1, 1, 1, 1, 1, 1, 1);
            string IDType_ID1_regex = validation.validate(Convert.ToString(obj.IDType_ID), 0, 1, 1, 1, 1, 1, 1, 1, 1);
            string Country_ID1_regex = validation.validate(Convert.ToString(obj.Country_ID), 0, 1, 1, 1, 1, 1, 1, 1, 1);

            if (Client_ID_regex == "1" && Country_ID_regex == "1" && Client_ID1_regex != "false" && IDType_ID1_regex != "false" && Country_ID1_regex != "false")
            {
                MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("GetIDNames");
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.Parameters.AddWithValue("Client_ID", obj.Client_ID);
                string _whereclause = ""; string endtag = "";
                if (obj.IDType_ID > 0)
                {
                    _whereclause = " and  im.IDType_ID =" + obj.IDType_ID;
                }
                if (obj.Country_ID != 0 && obj.Country_ID != -1 && obj.Country_ID != null)
                {
                    _whereclause = _whereclause + "   and   (Country_ID=" + obj.Country_ID; endtag = ")";
                }
                _whereclause = _whereclause + "  OR  Common_Flag=2 " + endtag;
                _cmd.Parameters.AddWithValue("clause", _whereclause);
                dt = db_connection.ExecuteQueryDataTableProcedure(_cmd);
            }
            else
            {
                string msg = "Validation Error Client_ID1_regex- +" + Client_ID1_regex + "IDType_ID1_regex- " + IDType_ID1_regex + " IDType_ID1_regex- " + Country_ID1_regex + " +Country_ID1_regex- " + Country_ID1_regex;
                int stattus = (int)CompanyInfo.InsertActivityLogDetailsSecurity(msg, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), 1, "srvDocument", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "GetIDNames", 0, context);
            }
            return dt;
        }
        public DataTable GetIDTypes(Model.Document obj, HttpContext context)
        {
            DataTable dt = new DataTable();
            string Client_ID_regex = validation.validate(Convert.ToString(obj.Client_ID), 0, 1, 1, 1, 1, 1, 1, 1, 1);
            if (Client_ID_regex != "false")
            {
                MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("GetIDTypes");
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.Parameters.AddWithValue("Client_ID", obj.Client_ID);
                string _whereclause = "";
                _cmd.Parameters.AddWithValue("clause", _whereclause);
                dt = db_connection.ExecuteQueryDataTableProcedure(_cmd);
            }
            else
            {
                string msg = "Validation Error Client_ID_regex-" + Client_ID_regex;
                int stattus = (int)CompanyInfo.InsertActivityLogDetailsSecurity(msg, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), 1, "srvDocument", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "GetIDTypes", 0, context);
            }
            return dt;
        }


        public async Task<DataTable> Upload(Model.Doc obj, HttpContext context, IFormFile? f_file, IFormFile? b_file)
        {
            int Customer_ID = 0, res = 0; int benf_id = 0; string notification_icon = ""; string notification_message = "";
            // Model.Document obj = new Model.Document();
            DataTable ds = new DataTable();
            ds.Columns.Add("status", typeof(string));
            ds.Columns.Add("Message", typeof(string));
            // var context = HttpContext.contex;
            try
            {
                //if (context.Request.Files.Count > 0)
                //{
                string error_msg = ""; string error_invalid_data = "";
                string SenderNameOnID_regex = validation.validate(Convert.ToString(context.Request.Form["sendernameonid"]), 1, 1, 1, 1, 1, 1, 0, 1, 1);
                string SenderID_PlaceOfIssue_regex = validation.validate(context.Request.Form["senderidplaceofissue"], 1, 1, 1, 1, 1, 1, 1, 1, 0);//
                string Comments_regex = validation.validate(Convert.ToString(context.Request.Form["comments"]), 1, 1, 1, 1, 1, 1, 1, 1, 0);//string Comments_regex = validation.validate(Convert.ToString(context.Request.Form["Comments"]), 1, 1, 1, 0, 1, 1, 1, 1, 1);

                string IDnumber_regex = validation.validate(Convert.ToString(context.Request.Form["senderidnumber"]), 1, 1, 0, 1, 1, 1, 1, 1, 1);
                if (Comments_regex == "false")
                {
                    error_msg = error_msg + "Please enter valid comment.";
                    error_invalid_data = error_invalid_data + " Comment: " + Convert.ToString(context.Request.Form["comments"]);
                }
                if (SenderNameOnID_regex == "false")
                {
                    error_msg = error_msg + "Name on ID must include characters.";
                    error_invalid_data = error_invalid_data + " SenderNameOnID: " + Convert.ToString(context.Request.Form["sendernameonid"]);
                }
                if (SenderID_PlaceOfIssue_regex == "false")
                {
                    error_msg = error_msg + "Place of issue should be alphanumeric with space and special characters like , , , . , ' , ( , ) , { , } , - , @ , & ";
                    error_invalid_data = error_invalid_data + " SenderID_PlaceOfIssue: " + context.Request.Form["senderidplaceofissue"];
                }
                if (IDnumber_regex == "false")
                {
                    error_msg = error_msg + "Id Number should be alphanumeric without space.";
                    error_invalid_data = error_invalid_data + " IDnumber: " + Convert.ToString(context.Request.Form["senderidnumber"]);
                }
                if (SenderNameOnID_regex == "" && SenderID_PlaceOfIssue_regex == "" && Comments_regex == "" && IDnumber_regex == "")
                {
                    SenderNameOnID_regex = "true";
                    SenderID_PlaceOfIssue_regex = "true";
                    Comments_regex = "true";
                    IDnumber_regex = "true";
                }
                if (SenderNameOnID_regex != "false" && SenderID_PlaceOfIssue_regex != "false" && Comments_regex != "false" && IDnumber_regex != "false")
                {

                    ////CompanyInfo.GetEncounters();
                    //MySqlCommand cmd3 = new MySqlCommand("customer_details_by_param");
                    //cmd3.CommandType = CommandType.StoredProcedure;
                    //string _whereclause = " and cr.Client_ID=" + obj.Client_ID;
                    //if (Customer_ID > 0)
                    //{
                    //    _whereclause = " and cr.Customer_ID=" + Customer_ID;
                    //}
                    //cmd3.Parameters.AddWithValue("_whereclause", _whereclause);
                    //cmd3.Parameters.AddWithValue("_SecurityKey", CompanyInfo.SecurityKey());
                    //DataTable dt_cust = db_connection.ExecuteQueryDataTableProcedure(cmd3);

                    //string referno = Convert.ToString(dt_cust.Rows[0]["WireTransfer_ReferanceNo"]);


                    string SenderNameOnID = CompanyInfo.testInjection(context.Request.Form["sendernameonid"]);
                    string SenderID_Number = CompanyInfo.testInjection(Convert.ToString(context.Request.Form["senderidnumber"]));
                    string IDType_ID = CompanyInfo.testInjection(Convert.ToString(context.Request.Form["typeID"]));
                    string IDName_ID = CompanyInfo.testInjection(Convert.ToString(context.Request.Form["nameID"]));
                    string SenderID_ExpiryDate = CompanyInfo.testInjection(Convert.ToString(context.Request.Form["senderidexpirydate"]));
                    string Customer_ID1 = CompanyInfo.testInjection(Convert.ToString(context.Request.Form["customerid"]));
                    string Sender_DateOfBirth = CompanyInfo.testInjection(Convert.ToString(context.Request.Form["senderdateofbirth"]));
                    string Client_ID = CompanyInfo.testInjection(Convert.ToString(context.Request.Form["clientID"]));
                    string Comments = CompanyInfo.testInjection(Convert.ToString(context.Request.Form["comments"]));
                    string DocumentName = CompanyInfo.testInjection(Convert.ToString(context.Request.Form["documentname"]));
                    string MRZ_number = CompanyInfo.testInjection(Convert.ToString(context.Request.Form["MRZ_number"]));
                    string MRZ_number_Second = CompanyInfo.testInjection(Convert.ToString(context.Request.Form["MRZ_number_Second"])); //Digvijay changes for mrz number second line field add on document details table 13-03-25

                    if (SenderNameOnID == "1" && SenderID_Number == "1" && IDType_ID == "1" && IDName_ID == "1" &&
                        SenderID_ExpiryDate == "1" && Customer_ID1 == "1" && Sender_DateOfBirth == "1" && Client_ID == "1" &&
                        Comments == "1" && Customer_ID1 == "1" && DocumentName == "1" && MRZ_number == "1" && MRZ_number_Second == "1"
                            )

                    {
                        obj.SenderNameOnID = Convert.ToString(context.Request.Form["SenderNameOnID"]);
                        obj.SenderID_Number = Convert.ToString(context.Request.Form["senderidnumber"]);
                        obj.IDType_ID = Convert.ToInt32(context.Request.Form["typeID"]);
                        obj.IDName_ID = Convert.ToInt32(context.Request.Form["nameID"]);
                        obj.SenderID_ExpiryDate = Convert.ToString(context.Request.Form["senderidexpirydate"]);
                        Customer_ID = Convert.ToInt32(CompanyInfo.Decrypt(context.Request.Form["customerid"], true));
                        obj.Customer_ID = Convert.ToString(Customer_ID);
                        obj.Sender_DateOfBirth = Convert.ToString(context.Request.Form["senderdateofbirth"]);
                        obj.Client_ID = Convert.ToInt32(context.Request.Form["clientID"]);
                        obj.Comments = Convert.ToString(context.Request.Form["comments"]);
                        obj.DocumentName = Convert.ToString(context.Request.Form["documentname"]);
                        try { obj.MRZ_number = Convert.ToString(context.Request.Form["mrznumber"]); } catch (Exception ex) { }
                        try { obj.MRZ_number_Second = Convert.ToString(context.Request.Form["mrznumbersecond"]); } catch (Exception ex) { }//Digvijay changes for mrz number second line field add on document details table 13-03-25
                        if (Convert.ToInt32(context.Request.Form["Benf_ID"]) != null && Convert.ToInt32(context.Request.Form["Benf_ID"]) != 0 && Convert.ToInt32(context.Request.Form["Benf_ID"]) != -1)
                        {
                            obj.Beneficiary_ID = Convert.ToInt32(context.Request.Form["Benf_ID"]);
                        }
                        string Username = Convert.ToString(context.Request.Form["Username"]);

                        if (obj.SenderID_ExpiryDate != "" && obj.SenderID_ExpiryDate != null)
                        {
                            DateTime SID_Expiry = DateTime.ParseExact(obj.SenderID_ExpiryDate, "d/M/yyyy", CultureInfo.InvariantCulture);
                            obj.SenderID_ExpiryDate = SID_Expiry.ToString("yyyy-MM-dd");
                        }
                        //else
                        //{
                        //    DateTime dt = DateTime.Now.AddYears(20);
                        //    obj.SenderID_ExpiryDate = dt.ToString("yyyy-MM-dd");
                        //}
                        obj.SenderID_PlaceOfIssue = Convert.ToString(context.Request.Form["SenderID_PlaceOfIssue"]);
                        if (obj.SenderID_PlaceOfIssue != "" && obj.SenderID_PlaceOfIssue != null)
                        {
                            obj.SenderID_PlaceOfIssue = "" + obj.SenderID_PlaceOfIssue + "";
                        }

                        obj.Place_Of_ID = Convert.ToInt32(context.Request.Form["Place_Of_ID"]);

                        obj.Branch_ID = Convert.ToInt32(context.Request.Form["branchID"]);
                        if (obj.Branch_ID != -1 && obj.Branch_ID != null)
                        {
                            obj.Branch_ID = obj.Branch_ID;
                        }
                        obj.Sender_DateOfBirth = Convert.ToString(context.Request.Form["senderdateofbirth"]);
                        if (obj.Sender_DateOfBirth != "" && obj.Sender_DateOfBirth != null)
                        {
                            DateTime SID_DOB = DateTime.ParseExact(obj.Sender_DateOfBirth, "d/M/yyyy", CultureInfo.InvariantCulture);
                            obj.Sender_DateOfBirth = SID_DOB.ToString("yyyy-MM-dd");
                        }
                        //else
                        //{
                        //    DateTime dt = DateTime.Now.AddYears(-18);
                        //    obj.Sender_DateOfBirth = dt.ToString("yyyy-MM-dd");
                        //}
                        obj.Issue_date = Convert.ToString(context.Request.Form["issuedate"]);
                        if (obj.Issue_date != "" && obj.Issue_date != null)
                        {
                            DateTime SID_Issuedate = DateTime.ParseExact(obj.Issue_date, "d/M/yyyy", CultureInfo.InvariantCulture);
                            obj.Issue_date = SID_Issuedate.ToString("yyyy-MM-dd");
                        }
                        //else
                        //{
                        //    DateTime dt = DateTime.Now;
                        //    obj.Issue_date = dt.ToString("yyyy-MM-dd");
                        //}
                        obj.Permission_status = Convert.ToInt32(context.Request.Form["Dup_per"]);
                        if (obj.Permission_status != null)
                        {
                            obj.Permission_status = obj.Permission_status;
                        }
                        //c.Issue_Date = Convert.ToString(context.Request.Form["Issue_Date"]);
                        //if (c.Issue_Date != "" && c.Issue_Date != null)
                        //{
                        //    DateTime SID_IssueDate = DateTime.ParseExact(c.Issue_Date, "d/M/yyyy", CultureInfo.InvariantCulture);
                        //    c.Issue_Date = SID_IssueDate.ToString("yyyy-MM-dd");
                        //}
                        obj.Record_Insert_DateTime = CompanyInfo.gettime(obj.Client_ID, obj.Customer_ID, obj.Country_ID, context);
                        obj.Delete_Status = 0;
                        obj.User_ID = Convert.ToInt32(context.Request.Form["userID"]);





                        MySqlConnector.MySqlCommand cmd3 = new MySqlConnector.MySqlCommand("customer_details_by_param");//Anushka
                        cmd3.CommandType = CommandType.StoredProcedure;
                        string _whereclause = " and cr.Client_ID=" + obj.Client_ID;
                        if (Customer_ID > 0)
                        {
                            _whereclause = " and cr.Customer_ID=" + Customer_ID;
                        }
                        cmd3.Parameters.AddWithValue("_whereclause", _whereclause);
                        cmd3.Parameters.AddWithValue("_SecurityKey", CompanyInfo.SecurityKey());
                        DataTable dt_cust = db_connection.ExecuteQueryDataTableProcedure(cmd3);

                        string referno = Convert.ToString(dt_cust.Rows[0]["WireTransfer_ReferanceNo"]);

                        MySqlConnector.MySqlCommand cmdper = new MySqlConnector.MySqlCommand("Get_Permissions");
                        cmdper.CommandType = CommandType.StoredProcedure;
                        cmdper.Parameters.AddWithValue("Per_ID", 87);
                        cmdper.Parameters.AddWithValue("ClientID", obj.Client_ID);
                        DataTable dt1_watermark = db_connection.ExecuteQueryDataTableProcedure(cmdper);
                        IFormFile postedFile = null;
                        //Set the Folder Path.
                        string fileName = string.Empty;
                        DataTable dtc = CompanyInfo.get(obj.Client_ID, context);

                        string folderPath = string.Empty;
                        //   string folderPath = Convert.ToString(dtc.Rows[0]["RootURL"]) + "assets/Uploads/";
                        string filen;

                        string ext = "";
                        filen = obj.Record_Insert_DateTime;
                        //string file_yes_or_not = Convert.ToString(context.Request.Form["contains_file"]);
                        string file_yes_or_not = "";
                        if (f_file != null)
                        {
                            file_yes_or_not = "Yes";
                        }
                        else
                        {
                            file_yes_or_not = "No";
                        }
                        if (file_yes_or_not == "Yes")
                        {
                            // postedFile = context.Request.Files[0]; 
                            var files = context.Request.Form.Files;
                            if (files.Count > 0)
                            {
                                postedFile = files[0];
                            }
                            IFormFile postedFile1 = postedFile;
                            ext = Path.GetExtension(postedFile1.FileName);
                            bool checkvalidext = CompanyInfo.chkValidExtensionforall(ext);
                            if (!checkvalidext)
                            {
                                string msg = " SQl Enjection detected . Invalid File Detected";
                                //Activity = "<b>" + Username + "</b>" + " failed to add new ID Document. " + country_details + "</br>";
                                CompanyInfo.InsertActivityLogDetails("Message 1" + msg, obj.User_ID, 0, obj.User_ID, Customer_ID, "InsertPrimaryDocumentID", obj.Branch_ID, obj.Client_ID, "ID Upload", context);
                                ds.Rows.Add("Validation Failed", "Invalid File Detected in Front Page");
                                return ds;

                            }
                            if (Convert.ToInt32(context.Request.Form["Benf_ID"]) != null && Convert.ToInt32(context.Request.Form["Benf_ID"]) != 0 && Convert.ToInt32(context.Request.Form["Benf_ID"]) != -1)
                            {
                                //     folderPath = context.Server.MapPath("~/assets/Benf_Uploads/");
                                folderPath = Convert.ToString(dtc.Rows[0]["RootURL"]) + "assets/Benf_Uploads/";
                                if (obj.IDType_ID == 1)
                                {
                                    string path = "P-" + Convert.ToInt32(context.Request.Form["Benf_ID"]) + "-" + filen.Replace(":", "") + "-" + referno + "" + ext + "";
                                    //postedFile.SaveAs(folderPath + path);
                                    //obj.FileNameWithExt = "assets/Benf_Uploads/" + path;
                                    if (dt1_watermark.Rows[0]["Status_ForCustomer"].ToString() == "0")
                                    {
                                        if (chkValidExtension(Path.GetExtension(path)))//".jpeg", ".jpg", ".png",".bmp"
                                        {
                                            SaveImageFileWithWaterMark(postedFile, folderPath, path, obj);
                                        }
                                        else if (IsPDFFile(Path.GetExtension(path)))//pdf
                                        {
                                            //save pdf with watermark
                                            SavePDFWithWaterMark(postedFile, folderPath, path, obj);
                                            //postedFile.SaveAs(folderPath + path);
                                        }
                                        else
                                        {   //save files other than img,pdf
                                            //postedFile.SaveAs(folderPath + path);
                                            var filePath = Path.Combine(folderPath, f_file.FileName);
                                            using (var fileStream = new FileStream(filePath, FileMode.Create))
                                            {
                                                postedFile.CopyToAsync(fileStream);
                                            }
                                        }

                                    }
                                    else
                                    {   //save files other than img,pdf
                                        // postedFile.SaveAs(folderPath + path);

                                        var filePath = Path.Combine(folderPath, f_file.FileName);
                                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                                        {
                                            postedFile.CopyToAsync(fileStream);
                                        }
                                    }
                                    obj.FileNameWithExt = "assets/Benf_Uploads/" + path;
                                }
                                if (obj.IDType_ID == 2)
                                {
                                    string path = "S-" + Convert.ToInt32(context.Request.Form["Benf_ID"]) + "-" + filen.Replace(":", "") + "-" + referno + "" + ext + "";
                                    //postedFile.SaveAs(folderPath + path);
                                    //obj.FileNameWithExt = "assets/Benf_Uploads/" + path;
                                    if (dt1_watermark.Rows[0]["Status_ForCustomer"].ToString() == "0")
                                    {
                                        if (chkValidExtension(Path.GetExtension(path)))//".jpeg", ".jpg", ".png",".bmp"
                                        {
                                            SaveImageFileWithWaterMark(postedFile, folderPath, path, obj);
                                        }
                                        else if (IsPDFFile(Path.GetExtension(path)))//pdf
                                        {
                                            //save pdf with watermark
                                            SavePDFWithWaterMark(postedFile, folderPath, path, obj);
                                            //postedFile.SaveAs(folderPath + path);


                                        }
                                        else
                                        {   //save files other than img,pdf
                                            //postedFile.SaveAs(folderPath + path);
                                            var filePath = Path.Combine(folderPath, f_file.FileName);
                                            using (var fileStream = new FileStream(filePath, FileMode.Create))
                                            {
                                                postedFile.CopyToAsync(fileStream);
                                            }
                                        }
                                    }
                                    else
                                    {   //save files other than img,pdf
                                        //postedFile.SaveAs(folderPath + path);

                                        var filePath = Path.Combine(folderPath, f_file.FileName);
                                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                                        {
                                            postedFile.CopyToAsync(fileStream);
                                        }
                                    }
                                    obj.FileNameWithExt = "assets/Benf_Uploads/" + path;
                                }
                                if (obj.IDType_ID == 3)
                                {
                                    string path = "O-" + Convert.ToInt32(context.Request.Form["Benf_ID"]) + "-" + filen.Replace(":", "") + "-" + referno + "" + ext + "";
                                    //postedFile.SaveAs(folderPath + path);
                                    //obj.FileNameWithExt = "assets/Benf_Uploads/" + path;
                                    if (dt1_watermark.Rows[0]["Status_ForCustomer"].ToString() == "0")
                                    {
                                        if (chkValidExtension(Path.GetExtension(path)))//".jpeg", ".jpg", ".png",".bmp"
                                        {
                                            SaveImageFileWithWaterMark(postedFile, folderPath, path, obj);
                                        }
                                        else if (IsPDFFile(Path.GetExtension(path)))//pdf
                                        {
                                            //save pdf with watermark
                                            SavePDFWithWaterMark(postedFile, folderPath, path, obj);
                                            //postedFile.SaveAs(folderPath + path);
                                        }
                                        else
                                        {   //save files other than img,pdf
                                            //postedFile.SaveAs(folderPath + path);
                                            var filePath = Path.Combine(folderPath, f_file.FileName);
                                            using (var fileStream = new FileStream(filePath, FileMode.Create))
                                            {
                                                postedFile.CopyToAsync(fileStream);
                                            }
                                        }
                                    }
                                    else
                                    {   //save files other than img,pdf
                                        //postedFile.SaveAs(folderPath + path);

                                        var filePath = Path.Combine(folderPath, f_file.FileName);
                                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                                        {
                                            postedFile.CopyToAsync(fileStream);
                                        }
                                    }
                                    obj.FileNameWithExt = "assets/Benf_Uploads/" + path;
                                }
                                if (obj.IDType_ID == 4)
                                {
                                    string path = "Src-" + Convert.ToInt32(context.Request.Form["Benf_ID"]) + "-" + filen.Replace(":", "") + "-" + referno + "" + ext + "";
                                    //postedFile.SaveAs(folderPath + path);
                                    //obj.FileNameWithExt = "assets/Benf_Uploads/" + path;
                                    if (dt1_watermark.Rows[0]["Status_ForCustomer"].ToString() == "0")
                                    {
                                        if (chkValidExtension(Path.GetExtension(path)))//".jpeg", ".jpg", ".png",".bmp"
                                        {
                                            SaveImageFileWithWaterMark(postedFile, folderPath, path, obj);
                                        }
                                        else if (IsPDFFile(Path.GetExtension(path)))//pdf
                                        {
                                            //save pdf with watermark
                                            SavePDFWithWaterMark(postedFile, folderPath, path, obj);
                                            //postedFile.SaveAs(folderPath + path);
                                        }
                                        else
                                        {   //save files other than img,pdf
                                            // postedFile.SaveAs(folderPath + path);
                                            var filePath = Path.Combine(folderPath, f_file.FileName);
                                            using (var fileStream = new FileStream(filePath, FileMode.Create))
                                            {
                                                postedFile.CopyToAsync(fileStream);
                                            }
                                        }
                                    }
                                    else
                                    {   //save files other than img,pdf
                                        // postedFile.SaveAs(folderPath + path);
                                        var filePath = Path.Combine(folderPath, f_file.FileName);
                                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                                        {
                                            postedFile.CopyToAsync(fileStream);
                                        }
                                    }
                                    obj.FileNameWithExt = "assets/Benf_Uploads/" + path;
                                }
                            }
                            else
                            {

                                //   folderPath = context.Server.MapPath("~/assets/Uploads/");
                                folderPath = Convert.ToString(dtc.Rows[0]["RootURL"]) + "assets/Uploads/";

                                if (obj.IDType_ID == 1)
                                {
                                    string path = "P-" + Customer_ID + "-" + filen.Replace(":", "") + "-" + referno + "" + ext + "";
                                    //postedFile.SaveAs(folderPath + path);
                                    if (dt1_watermark.Rows[0]["Status_ForCustomer"].ToString() == "0")
                                    {
                                        if (chkValidExtension(Path.GetExtension(path)))//".jpeg", ".jpg", ".png",".bmp"
                                        {
                                            SaveImageFileWithWaterMark(postedFile, folderPath, path, obj);
                                        }
                                        else if (IsPDFFile(Path.GetExtension(path)))//pdf
                                        {
                                            //save pdf with watermark
                                            SavePDFWithWaterMark(postedFile, folderPath, path, obj);
                                            //postedFile.SaveAs(folderPath + path);
                                        }
                                        else
                                        {   //save files other than img,pdf
                                            // postedFile.SaveAs(folderPath + path);



                                            var filePath = Path.Combine(folderPath, path);
                                            await using (var fileStream = new FileStream(filePath, FileMode.Create))
                                            {
                                                await postedFile.CopyToAsync(fileStream);
                                            }


                                        }
                                    }
                                    else
                                    {   //save files other than img,pdf
                                        // postedFile.SaveAs(folderPath + path);

                                        var filePath = Path.Combine(folderPath, path);
                                        await using (var fileStream = new FileStream(filePath, FileMode.Create))
                                        {
                                            await postedFile.CopyToAsync(fileStream);
                                        }


                                    }
                                    obj.FileNameWithExt = "assets/Uploads/" + path;
                                }
                                if (obj.IDType_ID == 2)
                                {
                                    string path = "S-" + Customer_ID + "-" + filen.Replace(":", "") + "-" + referno + "" + ext + "";
                                    //postedFile.SaveAs(folderPath + path);
                                    if (dt1_watermark.Rows[0]["Status_ForCustomer"].ToString() == "0")
                                    {
                                        if (chkValidExtension(Path.GetExtension(path)))//".jpeg", ".jpg", ".png",".bmp"
                                        {
                                            SaveImageFileWithWaterMark(postedFile, folderPath, path, obj);
                                        }
                                        else if (IsPDFFile(Path.GetExtension(path)))//pdf
                                        {
                                            //save pdf with watermark
                                            SavePDFWithWaterMark(postedFile, folderPath, path, obj);
                                            //postedFile.SaveAs(folderPath + path);
                                        }
                                        else
                                        {   //save files other than img,pdf
                                            // postedFile.SaveAs(folderPath + path);
                                            var filePath = Path.Combine(folderPath, path);
                                            await using (var fileStream = new FileStream(filePath, FileMode.Create))
                                            {
                                                await postedFile.CopyToAsync(fileStream);
                                            }
                                        }
                                    }
                                    else
                                    {   //save files other than img,pdf
                                        //postedFile.SaveAs(folderPath + path);

                                        var filePath = Path.Combine(folderPath, path);
                                        await using (var fileStream = new FileStream(filePath, FileMode.Create))
                                        {
                                            await postedFile.CopyToAsync(fileStream);
                                        }
                                    }
                                    obj.FileNameWithExt = "assets/Uploads/" + path;
                                }
                                if (obj.IDType_ID == 3)
                                {
                                    string path = "O-" + Customer_ID + "-" + filen.Replace(":", "") + "-" + referno + "" + ext + "";
                                    //postedFile.SaveAs(folderPath + path);
                                    if (dt1_watermark.Rows[0]["Status_ForCustomer"].ToString() == "0")
                                    {
                                        if (chkValidExtension(Path.GetExtension(path)))//".jpeg", ".jpg", ".png",".bmp"
                                        {
                                            SaveImageFileWithWaterMark(postedFile, folderPath, path, obj);
                                        }
                                        else if (IsPDFFile(Path.GetExtension(path)))//pdf
                                        {
                                            //save pdf with watermark
                                            SavePDFWithWaterMark(postedFile, folderPath, path, obj);
                                            //postedFile.SaveAs(folderPath + path);
                                        }
                                        else
                                        {   //save files other than img,pdf
                                            //postedFile.SaveAs(folderPath + path);
                                            var filePath = Path.Combine(folderPath, path);
                                            await using (var fileStream = new FileStream(filePath, FileMode.Create))
                                            {
                                                await postedFile.CopyToAsync(fileStream);
                                            }
                                        }
                                    }
                                    else
                                    {   //save files other than img,pdf
                                        //postedFile.SaveAs(folderPath + path);
                                        var filePath = Path.Combine(folderPath, path);
                                        await using (var fileStream = new FileStream(filePath, FileMode.Create))
                                        {
                                            await postedFile.CopyToAsync(fileStream);
                                        }
                                    }
                                    obj.FileNameWithExt = "assets/Uploads/" + path;
                                }
                                if (obj.IDType_ID == 4)
                                {
                                    string path = "Src-" + Customer_ID + "-" + filen.Replace(":", "") + "-" + referno + "" + ext + "";
                                    //postedFile.SaveAs(folderPath + path);
                                    if (dt1_watermark.Rows[0]["Status_ForCustomer"].ToString() == "0")
                                    {
                                        if (chkValidExtension(Path.GetExtension(path)))//".jpeg", ".jpg", ".png",".bmp"
                                        {
                                            SaveImageFileWithWaterMark(postedFile, folderPath, path, obj);
                                        }
                                        else if (IsPDFFile(Path.GetExtension(path)))//pdf
                                        {
                                            //save pdf with watermark
                                            SavePDFWithWaterMark(postedFile, folderPath, path, obj);
                                            //postedFile.SaveAs(folderPath + path);
                                        }
                                        else
                                        {   //save files other than img,pdf
                                            // postedFile.SaveAs(folderPath + path);
                                            var filePath = Path.Combine(folderPath, path);
                                            await using (var fileStream = new FileStream(filePath, FileMode.Create))
                                            {
                                                await postedFile.CopyToAsync(fileStream);
                                            }
                                        }
                                    }
                                    else
                                    {   //save files other than img,pdf
                                        //postedFile.SaveAs(folderPath + path);
                                        var filePath = Path.Combine(folderPath, path);
                                        await using (var fileStream = new FileStream(filePath, FileMode.Create))
                                        {
                                            await postedFile.CopyToAsync(fileStream);
                                        }
                                    }
                                    obj.FileNameWithExt = "assets/Uploads/" + path;
                                }
                            }
                        }
                        if (file_yes_or_not == "No")
                        {
                            //  context.Session.Add("doc_details", true);
                        }
                        //string file_yes_or_not1 = Convert.ToString(context.Request.Form["contains_backfile"]);
                        string file_yes_or_not1 = "";
                        if (b_file != null)
                        {
                            file_yes_or_not1 = "Yes";
                        }
                        else
                        {
                            file_yes_or_not1 = "No";
                        }
                        if (file_yes_or_not1 == "Yes")
                        {

                            if (file_yes_or_not == "No")
                            {
                                postedFile = context.Request.Form.Files[0];
                            }
                            else
                            {
                                postedFile = context.Request.Form.Files[1];
                            }
                            ext = Path.GetExtension(postedFile.FileName);
                            bool checkvalidext = CompanyInfo.chkValidExtensionforall(Path.GetExtension(postedFile.FileName));
                            if (!checkvalidext)
                            {
                                string msg = " SQl Enjection detetcted . Invalid File Detected";
                                //Activity = "<b>" + Username + "</b>" + " failed to add new ID Document. " + country_details + "</br>";
                                CompanyInfo.InsertActivityLogDetails("Message 2:" + msg, obj.User_ID, 0, obj.User_ID, Customer_ID, "InsertPrimaryDocumentID", obj.Branch_ID, obj.Client_ID, "ID Upload", context);
                                ds.Rows.Add("Validation Failed", "Invalid File Detected in Back Page");

                                return ds;

                            }
                            if (Convert.ToInt32(context.Request.Form["Benf_ID"]) != null && Convert.ToInt32(context.Request.Form["Benf_ID"]) != 0 && Convert.ToInt32(context.Request.Form["Benf_ID"]) != -1)
                            {
                                //      folderPath = context.Server.MapPath("~/assets/Benf_Uploads/");
                                folderPath = Convert.ToString(dtc.Rows[0]["RootURL"]) + "assets/Benf_Uploads/";
                                if (obj.IDType_ID == 1)
                                {
                                    string path = "Pback-" + Convert.ToInt32(context.Request.Form["Benf_ID"]) + "-" + filen.Replace(":", "") + "-" + referno + "" + ext + "";
                                    //postedFile.SaveAs(folderPath + path);
                                    //obj.secondaryFileNameWithExt = "assets/Benf_Uploads/" + path;
                                    if (dt1_watermark.Rows[0]["Status_ForCustomer"].ToString() == "0")
                                    {
                                        if (chkValidExtension(Path.GetExtension(path)))//".jpeg", ".jpg", ".png",".bmp"
                                        {
                                            SaveImageFileWithWaterMark(postedFile, folderPath, path, obj);
                                        }
                                        else if (IsPDFFile(Path.GetExtension(path)))//pdf
                                        {
                                            //save pdf with watermark
                                            SavePDFWithWaterMark(postedFile, folderPath, path, obj);
                                            //postedFile.SaveAs(folderPath + path);
                                        }
                                        else
                                        {   //save files other than img,pdf
                                            //await postedFile.SaveAsAsync(folderPath + path);
                                            var filePath = Path.Combine(folderPath, path);
                                            await using (var fileStream = new FileStream(filePath, FileMode.Create))
                                            {
                                                await postedFile.CopyToAsync(fileStream);
                                            }
                                        }
                                    }
                                    else
                                    {   //save files other than img,pdf
                                        //postedFile.SaveAs(folderPath + path);

                                        var filePath = Path.Combine(folderPath, path);
                                        await using (var fileStream = new FileStream(filePath, FileMode.Create))
                                        {
                                            await postedFile.CopyToAsync(fileStream);
                                        }
                                    }
                                    obj.secondaryFileNameWithExt = "assets/Benf_Uploads/" + path;
                                }
                                if (obj.IDType_ID == 2)
                                {
                                    string path = "Sback-" + Convert.ToInt32(context.Request.Form["Benf_ID"]) + "-" + filen.Replace(":", "") + "-" + referno + "" + ext + "";
                                    //postedFile.SaveAs(folderPath + path);
                                    //obj.secondaryFileNameWithExt = "assets/Benf_Uploads/" + path;
                                    if (dt1_watermark.Rows[0]["Status_ForCustomer"].ToString() == "0")
                                    {
                                        if (chkValidExtension(Path.GetExtension(path)))//".jpeg", ".jpg", ".png",".bmp"
                                        {
                                            SaveImageFileWithWaterMark(postedFile, folderPath, path, obj);
                                        }
                                        else if (IsPDFFile(Path.GetExtension(path)))//pdf
                                        {
                                            //save pdf with watermark
                                            SavePDFWithWaterMark(postedFile, folderPath, path, obj);
                                            //postedFile.SaveAs(folderPath + path);
                                        }
                                        else
                                        {   //save files other than img,pdf
                                            // postedFile.SaveAs(folderPath + path);
                                            var filePath = Path.Combine(folderPath, path);
                                            await using (var fileStream = new FileStream(filePath, FileMode.Create))
                                            {
                                                await postedFile.CopyToAsync(fileStream);
                                            }
                                        }
                                    }
                                    else
                                    {   //save files other than img,pdf
                                        //postedFile.SaveAs(folderPath + path);
                                        var filePath = Path.Combine(folderPath, path);
                                        await using (var fileStream = new FileStream(filePath, FileMode.Create))
                                        {
                                            await postedFile.CopyToAsync(fileStream);
                                        }
                                    }
                                    obj.secondaryFileNameWithExt = "assets/Benf_Uploads/" + path;
                                }
                                if (obj.IDType_ID == 3)
                                {
                                    string path = "Oback-" + Convert.ToInt32(context.Request.Form["Benf_ID"]) + "-" + filen.Replace(":", "") + "-" + referno + "" + ext + "";
                                    //postedFile.SaveAs(folderPath + path);
                                    //obj.secondaryFileNameWithExt = "assets/Benf_Uploads/" + path;
                                    if (dt1_watermark.Rows[0]["Status_ForCustomer"].ToString() == "0")
                                    {
                                        if (chkValidExtension(Path.GetExtension(path)))//".jpeg", ".jpg", ".png",".bmp"
                                        {
                                            SaveImageFileWithWaterMark(postedFile, folderPath, path, obj);
                                        }
                                        else if (IsPDFFile(Path.GetExtension(path)))//pdf
                                        {
                                            //save pdf with watermark
                                            SavePDFWithWaterMark(postedFile, folderPath, path, obj);
                                            //postedFile.SaveAs(folderPath + path);
                                        }
                                        else
                                        {   //save files other than img,pdf
                                            // postedFile.SaveAs(folderPath + path);
                                            var filePath = Path.Combine(folderPath, path);
                                            await using (var fileStream = new FileStream(filePath, FileMode.Create))
                                            {
                                                await postedFile.CopyToAsync(fileStream);
                                            }
                                        }
                                    }
                                    else
                                    {   //save files other than img,pdf
                                        // postedFile.SaveAs(folderPath + path);
                                        var filePath = Path.Combine(folderPath, path);
                                        await using (var fileStream = new FileStream(filePath, FileMode.Create))
                                        {
                                            await postedFile.CopyToAsync(fileStream);
                                        }
                                    }
                                    obj.secondaryFileNameWithExt = "assets/Benf_Uploads/" + path;
                                }
                                if (obj.IDType_ID == 4)
                                {
                                    string path = "Srcback-" + Convert.ToInt32(context.Request.Form["Benf_ID"]) + "-" + filen.Replace(":", "") + "-" + referno + "" + ext + "";
                                    //postedFile.SaveAs(folderPath + path);
                                    //obj.secondaryFileNameWithExt = "assets/Benf_Uploads/" + path;
                                    if (dt1_watermark.Rows[0]["Status_ForCustomer"].ToString() == "0")
                                    {
                                        if (chkValidExtension(Path.GetExtension(path)))//".jpeg", ".jpg", ".png",".bmp"
                                        {
                                            SaveImageFileWithWaterMark(postedFile, folderPath, path, obj);
                                        }
                                        else if (IsPDFFile(Path.GetExtension(path)))//pdf
                                        {
                                            //save pdf with watermark
                                            SavePDFWithWaterMark(postedFile, folderPath, path, obj);
                                            //postedFile.SaveAs(folderPath + path);
                                        }
                                        else
                                        {   //save files other than img,pdf
                                            // postedFile.SaveAs(folderPath + path);
                                            var filePath = Path.Combine(folderPath, path);
                                            await using (var fileStream = new FileStream(filePath, FileMode.Create))
                                            {
                                                await postedFile.CopyToAsync(fileStream);
                                            }
                                        }
                                    }
                                    else
                                    {   //save files other than img,pdf
                                        //  postedFile.SaveAs(folderPath + path);
                                        var filePath = Path.Combine(folderPath, path);
                                        await using (var fileStream = new FileStream(filePath, FileMode.Create))
                                        {
                                            await postedFile.CopyToAsync(fileStream);
                                        }
                                    }
                                    obj.secondaryFileNameWithExt = "assets/Benf_Uploads/" + path;
                                }
                            }
                            else
                            {
                                //     folderPath = context.Server.MapPath("~/assets/Uploads/");
                                folderPath = Convert.ToString(dtc.Rows[0]["RootURL"]) + "/assets/Uploads/";
                                if (obj.IDType_ID == 1)
                                {
                                    string path = "Pback-" + Customer_ID + "-" + filen.Replace(":", "") + "-" + referno + "" + ext + "";
                                    //postedFile.SaveAs(folderPath + path);
                                    if (dt1_watermark.Rows[0]["Status_ForCustomer"].ToString() == "0")
                                    {
                                        if (chkValidExtension(Path.GetExtension(path)))//".jpeg", ".jpg", ".png",".bmp"
                                        {
                                            SaveImageFileWithWaterMark(postedFile, folderPath, path, obj);
                                        }
                                        else if (IsPDFFile(Path.GetExtension(path)))//pdf
                                        {
                                            //save pdf with watermark
                                            SavePDFWithWaterMark(postedFile, folderPath, path, obj);
                                            //postedFile.SaveAs(folderPath + path);
                                        }
                                        else
                                        {   //save files other than img,pdf
                                            // postedFile.SaveAs(folderPath + path);
                                            var filePath = Path.Combine(folderPath, path);
                                            await using (var fileStream = new FileStream(filePath, FileMode.Create))
                                            {
                                                await postedFile.CopyToAsync(fileStream);
                                            }
                                        }
                                    }
                                    else
                                    {   //save files other than img,pdf
                                        // postedFile.SaveAs(folderPath + path);
                                        var filePath = Path.Combine(folderPath, path);
                                        await using (var fileStream = new FileStream(filePath, FileMode.Create))
                                        {
                                            await postedFile.CopyToAsync(fileStream);
                                        }
                                    }
                                    obj.secondaryFileNameWithExt = "assets/Uploads/" + path;
                                }
                                if (obj.IDType_ID == 2)
                                {
                                    string path = "Sback-" + Customer_ID + "-" + filen.Replace(":", "") + "-" + referno + "" + ext + "";
                                    //postedFile.SaveAs(folderPath + path);
                                    if (dt1_watermark.Rows[0]["Status_ForCustomer"].ToString() == "0")
                                    {
                                        if (chkValidExtension(Path.GetExtension(path)))//".jpeg", ".jpg", ".png",".bmp"
                                        {
                                            SaveImageFileWithWaterMark(postedFile, folderPath, path, obj);
                                        }
                                        else if (IsPDFFile(Path.GetExtension(path)))//pdf
                                        {
                                            //save pdf with watermark
                                            SavePDFWithWaterMark(postedFile, folderPath, path, obj);
                                            //postedFile.SaveAs(folderPath + path);
                                        }
                                        else
                                        {   //save files other than img,pdf
                                            // postedFile.SaveAs(folderPath + path);
                                            var filePath = Path.Combine(folderPath, path);
                                            await using (var fileStream = new FileStream(filePath, FileMode.Create))
                                            {
                                                await postedFile.CopyToAsync(fileStream);
                                            }
                                        }
                                    }
                                    else
                                    {   //save files other than img,pdf
                                        //postedFile.SaveAs(folderPath + path);
                                        var filePath = Path.Combine(folderPath, path);
                                        await using (var fileStream = new FileStream(filePath, FileMode.Create))
                                        {
                                            await postedFile.CopyToAsync(fileStream);
                                        }
                                    }
                                    obj.secondaryFileNameWithExt = "assets/Uploads/" + path;
                                }
                                if (obj.IDType_ID == 3)
                                {
                                    string path = "Oback-" + Customer_ID + "-" + filen.Replace(":", "") + "-" + referno + "" + ext + "";
                                    //postedFile.SaveAs(folderPath + path);
                                    if (dt1_watermark.Rows[0]["Status_ForCustomer"].ToString() == "0")
                                    {
                                        if (chkValidExtension(Path.GetExtension(path)))//".jpeg", ".jpg", ".png",".bmp"
                                        {
                                            SaveImageFileWithWaterMark(postedFile, folderPath, path, obj);
                                        }
                                        else if (IsPDFFile(Path.GetExtension(path)))//pdf
                                        {
                                            //save pdf with watermark
                                            SavePDFWithWaterMark(postedFile, folderPath, path, obj);
                                            //postedFile.SaveAs(folderPath + path);
                                        }
                                        else
                                        {   //save files other than img,pdf
                                            // postedFile.SaveAs(folderPath + path);
                                            var filePath = Path.Combine(folderPath, path);
                                            await using (var fileStream = new FileStream(filePath, FileMode.Create))
                                            {
                                                await postedFile.CopyToAsync(fileStream);
                                            }
                                        }
                                    }
                                    else
                                    {   //save files other than img,pdf
                                        // postedFile.SaveAs(folderPath + path);
                                        var filePath = Path.Combine(folderPath, path);
                                        await using (var fileStream = new FileStream(filePath, FileMode.Create))
                                        {
                                            await postedFile.CopyToAsync(fileStream);
                                        }
                                    }
                                    obj.secondaryFileNameWithExt = "assets/Uploads/" + path;
                                }
                                if (obj.IDType_ID == 4)
                                {
                                    string path = "Srcback-" + Customer_ID + "-" + filen.Replace(":", "") + "-" + referno + "" + ext + "";
                                    //postedFile.SaveAs(folderPath + path);
                                    if (dt1_watermark.Rows[0]["Status_ForCustomer"].ToString() == "0")
                                    {
                                        if (chkValidExtension(Path.GetExtension(path)))//".jpeg", ".jpg", ".png",".bmp"
                                        {
                                            SaveImageFileWithWaterMark(postedFile, folderPath, path, obj);
                                        }
                                        else if (IsPDFFile(Path.GetExtension(path)))//pdf
                                        {
                                            //save pdf with watermark
                                            SavePDFWithWaterMark(postedFile, folderPath, path, obj);
                                            //postedFile.SaveAs(folderPath + path);
                                        }
                                        else
                                        {   //save files other than img,pdf
                                            // postedFile.SaveAs(folderPath + path);
                                            var filePath = Path.Combine(folderPath, path);
                                            await using (var fileStream = new FileStream(filePath, FileMode.Create))
                                            {
                                                await postedFile.CopyToAsync(fileStream);
                                            }
                                        }
                                    }
                                    else
                                    {   //save files other than img,pdf
                                        // postedFile.SaveAs(folderPath + path);
                                        var filePath = Path.Combine(folderPath, path);
                                        await using (var fileStream = new FileStream(filePath, FileMode.Create))
                                        {
                                            await postedFile.CopyToAsync(fileStream);
                                        }
                                    }
                                    obj.secondaryFileNameWithExt = "assets/Uploads/" + path;
                                }
                            }
                        }
                        if (file_yes_or_not1 == "No")
                        {
                            //    context.Session.Add("doc_details", true);
                        }
                        using (MySqlConnector.MySqlConnection con = new MySqlConnector.MySqlConnection(_dbConnection.ConnectionStringSection()))
                        {
                            if (con.State != ConnectionState.Open)
                                con.Open();
                            try
                            {
                                using (MySqlConnector.MySqlCommand cmd = new MySqlConnector.MySqlCommand("Customer_InsertPID2", con))
                                {


                                    cmd.CommandType = CommandType.StoredProcedure;
                                    cmd.Parameters.AddWithValue("_SenderNameOnID", obj.SenderNameOnID.Trim());
                                    cmd.Parameters.AddWithValue("_IDType_ID", obj.IDType_ID);
                                    cmd.Parameters.AddWithValue("_SenderID_Number", obj.SenderID_Number);
                                    if (obj.SenderID_ExpiryDate == "" || obj.SenderID_ExpiryDate == null)
                                    {
                                        cmd.Parameters.Add("_SenderID_ExpiryDate", (DbType)SqlDbType.DateTime).Value = DBNull.Value;
                                    }
                                    else
                                    {
                                        cmd.Parameters.AddWithValue("_SenderID_ExpiryDate", obj.SenderID_ExpiryDate);
                                    }
                                    if (obj.Sender_DateOfBirth == "" || obj.Sender_DateOfBirth == null)
                                    {
                                        cmd.Parameters.Add("_Sender_DateOfBirth", (DbType)SqlDbType.DateTime).Value = DBNull.Value;
                                    }
                                    else
                                    {
                                        cmd.Parameters.AddWithValue("_Sender_DateOfBirth", obj.Sender_DateOfBirth);
                                    }
                                    if (obj.Issue_date == "" || obj.Issue_date == null)
                                    {
                                        cmd.Parameters.Add("_issue_date", (DbType)SqlDbType.DateTime).Value = DBNull.Value;
                                    }
                                    else
                                    {
                                        cmd.Parameters.AddWithValue("_issue_date", obj.Issue_date);
                                    }

                                    //cmd.Parameters.AddWithValue("_SenderID_PlaceOfIssue", obj.SenderID_PlaceOfIssue.Trim());
                                    cmd.Parameters.AddWithValue("_SenderID_PlaceOfIssue", obj.SenderID_PlaceOfIssue);
                                    cmd.Parameters.AddWithValue("_Place_Of_ID", obj.Place_Of_ID);
                                    cmd.Parameters.AddWithValue("_Customer_ID", Customer_ID);
                                    cmd.Parameters.AddWithValue("_IDName_ID", obj.IDName_ID);
                                    cmd.Parameters.AddWithValue("_Record_Insert_DateTime", obj.Record_Insert_DateTime);
                                    cmd.Parameters.AddWithValue("_Delete_Status", obj.Delete_Status);
                                    cmd.Parameters.AddWithValue("_User_ID", obj.User_ID);
                                    cmd.Parameters.AddWithValue("_documents_details_Id", obj.IDType_ID);
                                    cmd.Parameters.AddWithValue("_CB_ID", obj.Branch_ID);
                                    cmd.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                                    cmd.Parameters.AddWithValue("_Doc_Name", obj.DocumentName);
                                    cmd.Parameters.AddWithValue("_comments", obj.Comments);
                                    cmd.Parameters.AddWithValue("_FileNameWithExt", obj.FileNameWithExt);
                                    //   cmd.Parameters.AddWithValue("_secondaryFileNameWithExt", obj.secondaryFileNameWithExt);
                                    cmd.Parameters.AddWithValue("_Benf_ID", obj.Beneficiary_ID);
                                    cmd.Parameters.AddWithValue("_BackID_Document", obj.secondaryFileNameWithExt);
                                    cmd.Parameters.AddWithValue("_JourneyID", "");
                                    cmd.Parameters.AddWithValue("_PDFGenerate_Status", 1);//default 1 - means not yet downloaded
                                    cmd.Parameters.AddWithValue("_MRZ_number", obj.MRZ_number);
                                    cmd.Parameters.AddWithValue("_MRZ_number2", obj.MRZ_number_Second);
                                    cmd.Parameters.AddWithValue("_PDF_FileName", "");


                                    if (obj.Beneficiary_ID == 0)
                                    {
                                        if (obj.IDType_ID != null && obj.IDType_ID != -1)
                                        {
                                            if (obj.IDType_ID == 1)
                                            {
                                                notification_icon = "primary-id-upload.jpg";
                                                notification_message = "<span class='cls-admin'>successfully uploaded <strong class='cls-priamary'>Primary ID</strong>.</span><span class='cls-customer'></span>";
                                            }
                                            if (obj.IDType_ID == 2)
                                            {
                                                notification_icon = "secondary-id-upload.jpg";
                                                notification_message = "<span class='cls-admin'>successfully uploaded <strong class='cls-secondary'>Secondary ID</strong>.</span><span class='cls-customer'></span>";
                                            }
                                            if (obj.IDType_ID == 3)
                                            {
                                                notification_icon = "other-doc.jpg";
                                                notification_message = "<span class='cls-admin'>successfully uploaded <strong class='cls-otherdoc'>Other document</strong>.</span><span class='cls-customer'></span>";
                                            }
                                            if (obj.IDType_ID == 4)
                                            {
                                                notification_icon = "sof.jpg";
                                                notification_message = "<span class='cls-admin'>successfully uploaded <strong class='cls-srcfund'>Source of fund document</strong>.</span><span class='cls-customer'></span>";
                                            }
                                        }
                                        cmd.Parameters.AddWithValue("_whereclause", " (Beneficiary_ID is null  or Beneficiary_ID = 0)");
                                    }
                                    else
                                    {
                                        if (obj.IDType_ID != null && obj.IDType_ID != -1)
                                        {
                                            if (obj.IDType_ID == 1)
                                            {
                                                notification_icon = "beneficiary-id-upload.jpg";
                                                notification_message = "<span class='cls-admin'>successfully uploaded <strong class='cls-benf-priamary'>Beneficiary Primary ID</strong>.Beneficiary name: <strong>" + obj.SenderNameOnID.Trim() + "</strong></span><span class='cls-customer'></span>";
                                            }
                                            if (obj.IDType_ID == 2)
                                            {
                                                notification_icon = "secondary-beni.jpg";
                                                notification_message = "<span class='cls-admin'>successfully uploaded <strong class='cls-benf-secondary'>Beneficiary Secondary ID</strong>.Beneficiary name: <strong>" + obj.SenderNameOnID.Trim() + "</strong></span><span class='cls-customer'></span>";
                                            }
                                            if (obj.IDType_ID == 3)
                                            {
                                                notification_icon = "otherdoc-beni.jpg";
                                                notification_message = "<span class='cls-admin'>successfully uploaded <strong class='cls-benf-otherdoc'>Beneficiary Other document</strong>.Beneficiary name: <strong>" + obj.SenderNameOnID.Trim() + "</strong></span><span class='cls-customer'></span>";
                                            }
                                            if (obj.IDType_ID == 4)
                                            {
                                                notification_icon = "sof-beni.jpg";
                                                notification_message = "<span class='cls-admin'>successfully uploaded <strong class='cls-benf-srcfund'>Beneficiary Source of fund document</strong>.Beneficiary name: <strong>" + obj.SenderNameOnID.Trim() + "</strong></span><span class='cls-customer'></span>";
                                            }
                                        }
                                        cmd.Parameters.AddWithValue("_whereclause", " (Beneficiary_ID is not null  or Beneficiary_ID !=0)");
                                    }
                                    cmd.Parameters.AddWithValue("_Dup_Per", obj.Permission_status);
                                    cmd.Parameters.Add(new MySqlConnector.MySqlParameter("_SenderID_ID", MySqlConnector.MySqlDbType.Int32));
                                    cmd.Parameters["_SenderID_ID"].Direction = ParameterDirection.Output;
                                    cmd.Parameters.Add(new MySqlConnector.MySqlParameter("_existid", MySqlConnector.MySqlDbType.Int32));
                                    cmd.Parameters["_existid"].Direction = ParameterDirection.Output;
                                    obj.SenderID_ID = 0;
                                    object result = cmd.ExecuteNonQuery();




                                    object ExistId = cmd.Parameters["_existid"].Value;
                                    ExistId = (ExistId == DBNull.Value) ? null : ExistId;
                                    // row["Transaction_Master_ID"] != DBNull.Value;

                                    obj.ExistId = Convert.ToInt32(ExistId);
                                    if (obj.ExistId == -1)
                                    {
                                    }
                                    if (obj.ExistId == 1)
                                    {
                                        //return "exist_mobile";
                                        obj.Message = "exist_Id";
                                        string Activities = "<b>Same document already exists in our system " + obj.UserName + "," + obj.SenderID_ExpiryDate + "," + obj.IDName_ID + "," + obj.SenderID_Number + " </b>";
                                        int stattus = (int)CompanyInfo.InsertActivityLogDetails(Activities, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(Customer_ID), "InsertPrimaryDocumentID", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "ID Upload", context);
                                    }
                                    else
                                    {
                                        obj.SenderID_ID = Convert.ToInt32(cmd.Parameters["_SenderID_ID"].Value);

                                    }
                                    result = (result == DBNull.Value) ? null : result;
                                    //        obj.SenderID_ID = Convert.ToInt32(result);
                                    res = Convert.ToInt32(result);
                                    cmd.Dispose();
                                }
                            }
                            catch (Exception ex)
                            {
                                Model.ErrorLog objError = new Model.ErrorLog();
                                objError.User = new Model.User();
                                objError.Branch = new Model.Branch();
                                objError.Client = new Model.Client();

                                objError.Error = " srvDocument : Upload --" + ex.ToString();
                                objError.Branch_ID = 1;
                                objError.Date = DateTime.Now;
                                objError.User_ID = 1;
                                objError.Id = 1;
                                objError.Function_Name = "Upload";
                                Service.srvErrorLog srvError = new Service.srvErrorLog();
                                srvError.Create(objError, context);
                            }
                            if (con.State != ConnectionState.Closed)
                                con.Close();
                        }
                        string Activity = string.Empty;
                        string idtype = string.Empty;


                        //  context.Session["Username"] = Username;
                        string country_details = "Document Details: ";


                        if (obj.IDType_ID != null && obj.IDType_ID != -1)
                        {
                            if (obj.IDType_ID == 1)
                            {
                                idtype = "Primary";
                            }
                            if (obj.IDType_ID == 2)
                            {
                                idtype = "Secondary";
                            }
                            if (obj.IDType_ID == 3)
                            {
                                idtype = "Other Documents";
                            }
                            if (obj.IDType_ID == 4)
                            {
                                idtype = "Source Of Funds";
                            }
                            country_details = country_details + " Id Type: " + idtype;
                        }
                        if (obj.SenderNameOnID != null && obj.SenderNameOnID != "")
                        {
                            country_details = country_details + " Name on ID: " + obj.SenderNameOnID;
                        }
                        if (obj.Sender_DateOfBirth != null && obj.Sender_DateOfBirth != "")
                        {
                            country_details = country_details + " Date of Birth: " + obj.Sender_DateOfBirth;
                        }
                        if (obj.IDName_ID != null && obj.IDName_ID != -1)
                        {
                            country_details = country_details + " ID: " + context.Request.Form["IDName"];
                        }
                        if (obj.SenderID_Number != null && obj.SenderID_Number != "")
                        {
                            country_details = country_details + "<br/> ID Number: " + obj.SenderID_Number;
                        }
                        if (obj.SenderID_ExpiryDate != null && obj.SenderID_ExpiryDate != "")
                        {
                            country_details = country_details + " Expiry Date: " + obj.SenderID_ExpiryDate;
                        }
                        if (obj.SenderID_PlaceOfIssue != null && obj.SenderID_PlaceOfIssue != "")
                        {
                            country_details = country_details + " Place of Issue: " + obj.SenderID_PlaceOfIssue;
                        }

                        if (res > 0 && obj.SenderID_ID != 0 && obj.SenderID_ID != null)
                        {



                            CompanyInfo.save_notification(notification_message, notification_icon, Customer_ID, Convert.ToDateTime(obj.Record_Insert_DateTime), obj.Client_ID, 1, 0, obj.Branch_ID, 0, 1, 1, 0, context);
                            try
                            {
                                DataTable dt_notif = CompanyInfo.set_notification_data(58);
                                if (dt_notif.Rows.Count > 0)
                                {
                                    int SMS = Convert.ToInt32(dt_notif.Rows[0]["SMS"]);
                                    int Email = Convert.ToInt32(dt_notif.Rows[0]["Email"]);
                                    int Notif_status = Convert.ToInt32(dt_notif.Rows[0]["Notification"]);
                                    string notification_msg = Convert.ToString(dt_notif.Rows[0]["notification_msg"]);
                                    int i = CompanyInfo.check_notification_perm(Convert.ToString(Customer_ID), obj.Client_ID, obj.Branch_ID, 1, 58, Convert.ToDateTime(obj.RecordDate), 1, SMS, Email, Notif_status, "App - ID Upload Notification - 58", notification_msg, context);
                                }
                            }
                            catch (Exception ex) { }
                            if (obj.Beneficiary_ID == 0)
                            {
                                string risk_factors = "2,8,10,13";
                                CompanyInfo.add_check_rules(Customer_ID, risk_factors, obj.Branch_ID, obj.Client_ID, context);
                                if (context.Request.Form.ToString().Contains("Aml_Alert") == true)
                                {
                                    int aml_alert = Convert.ToInt32(context.Request.Form["Aml_Alert"]);
                                    if (aml_alert > 0)
                                    {
                                        if (aml_alert == 2)
                                        {
                                            notification_icon = "id-threshold.jpg";
                                            notification_message = "<span class='cls-admin'>ID threshold <strong>secondary</strong> limit<strong class='cls-cancel'> exceeded</strong> and document uploaded.</span><span class='cls-customer'></span>";
                                            CompanyInfo.save_notification_compliance(notification_message, notification_icon, Convert.ToString(Customer_ID), Convert.ToDateTime(obj.Record_Insert_DateTime), Convert.ToInt32(obj.Client_ID), 1, Convert.ToInt32(obj.User_ID), Convert.ToInt32(obj.Branch_ID), 0, 1, 1, 0, context);

                                        }
                                        else if (aml_alert == 4)
                                        {
                                            notification_icon = "id-threshold.jpg";
                                            notification_message = "<span class='cls-admin'>ID threshold <strong>source of funds</strong> limit<strong class='cls-cancel'> exceeded</strong> and document uploaded.</span><span class='cls-customer'></span>";
                                            CompanyInfo.save_notification_compliance(notification_message, notification_icon, Convert.ToString(Customer_ID), Convert.ToDateTime(obj.Record_Insert_DateTime), Convert.ToInt32(obj.Client_ID), 1, Convert.ToInt32(obj.User_ID), Convert.ToInt32(obj.Branch_ID), 0, 1, 1, 0, context);
                                        }
                                    }

                                }
                                if (obj.IDType_ID == 4)
                                {

                                    string token = "";
                                    //HttpContext.Current.Session["Sof_Uploaded"] = true;
                                    string SendMoneyToken = "";
                                    string RewardToken = "";
                                    DateTime sendhashexpire = DateTime.Now;
                                    string SOFdocupload = "";
                                    int uploadSOF = 1;
                                    DateTime TransactionStartdate = Convert.ToDateTime("0001-01-01");
                                    try
                                    {
                                        token = obj.tokenValue;
                                        token = token.Replace("Bearer ", "");



                                        MySqlCommand _cmd = new MySqlCommand("SetTransactionJourney");
                                        _cmd.CommandType = CommandType.StoredProcedure;
                                        _cmd.Parameters.AddWithValue("_Token", token);
                                        _cmd.Parameters.AddWithValue("_SendMoneyToken", DBNull.Value);
                                        _cmd.Parameters.AddWithValue("_RewardToken", DBNull.Value);
                                        _cmd.Parameters.AddWithValue("_SOFdocupload", DBNull.Value);
                                        _cmd.Parameters.AddWithValue("_sendmoneyhashexpire", DBNull.Value);
                                        _cmd.Parameters.AddWithValue("_TransactionStartdate", DBNull.Value);
                                        _cmd.Parameters.AddWithValue("_Customer_ID", DBNull.Value);
                                        DataTable journerydata = db_connection.ExecuteQueryDataTableProcedure(_cmd);
                                        if (journerydata.Rows.Count > 0)
                                        {
                                            if (journerydata.Rows[0]["uploadSOF"] != DBNull.Value)
                                            {
                                                uploadSOF = Convert.ToInt32(journerydata.Rows[0]["uploadSOF"]);
                                                if (uploadSOF == 0 || uploadSOF == 1)
                                                {
                                                    _cmd = new MySqlCommand("UpdateTransactionJourney");
                                                    _cmd.CommandType = CommandType.StoredProcedure;
                                                    _cmd.Parameters.AddWithValue("_Token", token);
                                                    _cmd.Parameters.AddWithValue("_SendMoneyToken", SendMoneyToken);
                                                    _cmd.Parameters.AddWithValue("_RewardToken", RewardToken);
                                                    _cmd.Parameters.AddWithValue("_SOFdocupload", SOFdocupload);
                                                    _cmd.Parameters.AddWithValue("_sendmoneyhashexpire", sendhashexpire);
                                                    _cmd.Parameters.AddWithValue("_TransactionStartdate", TransactionStartdate);
                                                    _cmd.Parameters.AddWithValue("_chk_validity", DBNull.Value);
                                                    _cmd.Parameters.AddWithValue("_flag", 3);
                                                    _cmd.Parameters.AddWithValue("_uploadSOF", 2);
                                                    string success = Convert.ToString(db_connection.ExecuteNonQueryProcedure(_cmd));
                                                }
                                            }

                                        }
                                    }
                                    catch (Exception ex) { }


                                }
                            }
                            Activity = "<b>" + Username + "</b>" + " uploaded new ID Document. " + country_details + "</br>";
                            int stattus = (int)CompanyInfo.InsertActivityLogDetails(Activity, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(Customer_ID), "InsertPrimaryDocumentID", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "ID Upload", context);

                            string full_name = dt_cust.Rows[0]["Full_name"].ToString();
                            string email_id = dt_cust.Rows[0]["Email_ID"].ToString();

                            db_connection _MTS = new db_connection();
                            MySqlConnector.MySqlConnection con = new MySqlConnector.MySqlConnection(_MTS.WebConnSetting());
                            con.Open();

                            MySqlConnector.MySqlCommand cmdupdate1 = new MySqlConnector.MySqlCommand("SP_Get_Email_Permission");
                            cmdupdate1.CommandType = CommandType.StoredProcedure;
                            cmdupdate1.Parameters.AddWithValue("_ID", 2);
                            cmdupdate1.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                            //cmdupdate1.ExecuteScalar();
                            // obj.CommentUserId = 1;
                            // int abcd = Convert.ToInt32(cmdupdate1.Parameters["Status"].Value);
                            string permission_status = string.Empty;
                            DataTable dt1 = db_connection.ExecuteQueryDataTableProcedure(cmdupdate1);
                            if (dt1.Rows.Count > 0)
                            {
                                permission_status = Convert.ToString(dt1.Rows[0]["Status_ForCustomer"]);
                            }
                            if (permission_status == "0")//send mail
                            {
                                string get_email_status = send_mails(dtc, obj.Client_ID, obj.Branch_ID, Convert.ToString(dt_cust.Rows[0]["First_Name"]), referno, " Id uploaded successfully.", email_id, context);
                            }

                            //string get_email_status = send_mails(full_name, " Id uploaded successfully.", email_id, Convert.ToString(client_id), Convert.ToString(branch_id));
                            if (obj.IDType_ID == 1)//Primary
                            {
                                CheckCreditSafe(obj, dt_cust, "ID Upload", context);
                            }
                            ds.Rows.Add(obj.SenderID_ID);
                            //   ds.Rows.Add("0");
                        }
                        else if (obj.ExistId != 0 && obj.ExistId != null)
                        {
                            ds.Rows.Add(obj.Message);
                        }
                        else
                        {
                            Activity = "<b>" + Username + "</b>" + " failed to add new ID Document. " + country_details + "</br>";
                            string stattus = (string)CompanyInfo.InsertActivityLogDetails(Activity, obj.User_ID, 0, obj.User_ID, Customer_ID, "InsertPrimaryDocumentID", obj.Branch_ID, obj.Client_ID, "ID Upload", context);
                            ds.Rows.Add("1");
                        }

                    }
                    else
                    {
                        string msg = " SQl Enjection detected . security_code=0";
                        //Activity = "<b>" + Username + "</b>" + " failed to add new ID Document. " + country_details + "</br>";
                        string stattus = (string)CompanyInfo.InsertActivityLogDetails(msg, obj.User_ID, 0, obj.User_ID, Customer_ID, "InsertPrimaryDocumentID", obj.Branch_ID, obj.Client_ID, "ID Upload", context);
                        ds.Rows.Add("1");
                        //obj.Id = -1;
                        //// Activity = "<b>" + Username + "</b>" + " Password changed.  </br>";
                        ////int stattus = (int)CompanyInfo.InsertActivityLogDetails(msg, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID),obj.Id, "IsValidCustomer", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "IsValidCustomer");
                        //int stattus = (int)CompanyInfo.InsertActivityLogDetailsSecurity(msg, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), 1, "srvBenificiary", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "Update", 0);
                    }
                }
                else
                {
                    // obj.Id = 0;
                    // Model.Customer _obj = new Model.Customer();
                    //_obj = new Model.Customer();
                    string msg = "Validation Failed " + " <br/>  " + error_invalid_data;
                    obj.Comments = "Validation Failed";
                    obj.Message = error_msg;
                    int stattus = (Int32)CompanyInfo.InsertActivityLogDetails("Message 3: " + msg, obj.User_ID, 0, obj.User_ID, Customer_ID, "InsertPrimaryDocumentID", obj.Branch_ID, obj.Client_ID, "ID Upload", context);
                    ds.Rows.Add(obj.Comments, obj.Message);
                }
                //}

            }
            catch (Exception ae)
            {
                Model.ErrorLog objError = new Model.ErrorLog();
                objError.User = new Model.User();
                objError.Branch = new Model.Branch();
                objError.Client = new Model.Client();

                objError.Error = " srvDocument : Upload --" + ae.ToString();
                objError.Branch_ID = 1;
                objError.Date = DateTime.Now;
                objError.User_ID = 1;
                objError.Id = 1;
                objError.Function_Name = "Upload";
                Service.srvErrorLog srvError = new Service.srvErrorLog();
                srvError.Create(objError, context);

            }
            return ds;
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
                //HttpContext.Current.Session["excweb"] = ex;
            }
        }
        public string[] CheckCreditSafe(Model.Document i, DataTable dc, string CallFrom, HttpContext context)
        {
            int Status = 1; string Remark = string.Empty; string mail_send = string.Empty; string EmailUrl = string.Empty; string Company_Name = string.Empty;
            string Bandtext = "";
            int flag1 = 0, flag2 = 0;//pep or international
            try
            {
                MySqlConnector.MySqlCommand cmdupdate1 = new MySqlConnector.MySqlCommand("Get_Permissions");
                cmdupdate1.CommandType = CommandType.StoredProcedure;
                cmdupdate1.Parameters.AddWithValue("Per_ID", 32);
                cmdupdate1.Parameters.AddWithValue("ClientID", i.Client_ID);
                DataTable pm = db_connection.ExecuteQueryDataTableProcedure(cmdupdate1);
                if (pm.Rows.Count > 0)
                {
                    if (Convert.ToString(pm.Rows[0]["Status_ForCustomer"]) == "0")
                    {
                        int API_ID = 1;//Credit Safe API ID
                        MySqlConnector.MySqlCommand cmd = new MySqlConnector.MySqlCommand("GetAPIDetails_byCountry");//("GetAPIDetails");
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("_API_ID", API_ID);//Credit Safe API ID
                        cmd.Parameters.AddWithValue("_Client_ID", i.Client_ID);
                        cmd.Parameters.AddWithValue("_status", 0);// API Status
                        cmd.Parameters.AddWithValue("_Customer_ID", i.Customer_ID);//for country
                        DataTable dtApi = db_connection.ExecuteQueryDataTableProcedure(cmd);

                        if (dtApi.Rows.Count > 0)
                        {
                            //get Customer details

                            string First_Name = Convert.ToString(dc.Rows[0]["First_Name"]);
                            string Middle_Name = Convert.ToString(dc.Rows[0]["Middle_Name"]);
                            string Last_Name = Convert.ToString(dc.Rows[0]["Last_Name"]);
                            string Country1 = Convert.ToString(dc.Rows[0]["Country_Name"]).ToLower();
                            string House_Number = Convert.ToString(dc.Rows[0]["House_Number"]);
                            string Street = Convert.ToString(dc.Rows[0]["Street"]);
                            string City = Convert.ToString(dc.Rows[0]["City_Name"]);
                            string Post_Code = Convert.ToString(dc.Rows[0]["Post_Code"]);
                            i.Record_Insert_DateTime = (string)CompanyInfo.gettime(i.Client_ID, i.Customer_ID, i.Country_ID, context);
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
                            string expiry = Convert.ToString(i.SenderID_ExpiryDate);
                            if (expiry != "" && expiry != null)
                            {
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

                            if (i.IDName_ID == 1)// if Passport
                            {
                                passdoc += "<ns:IdentityDocuments><ns:InternationalPassport>";
                                passdoc += "<ns:Number>" + i.SenderID_Number + @"</ns:Number> " + passexpiry + "";
                                if (Country != "") { passdoc += "<ns:CountryOfOrigin>" + Country + @"</ns:CountryOfOrigin>"; }
                                passdoc += "</ns:InternationalPassport></ns:IdentityDocuments>";

                            }
                            else if (i.IDName_ID == 2 && Country != "")//if Driving licence
                            {
                                passdoc += "<ns:IdentityDocuments><ns:" + Country + @"><ns:DrivingLicence>";
                                passdoc += "<ns:Number>" + i.SenderID_Number + @"</ns:Number> " + passexpiry + "";
                                passdoc += "</ns:DrivingLicence></ns:" + Country + @"></ns:IdentityDocuments>";
                            }
                            else //if (i.IDName_ID == 3)//EU Nationality Card
                            {
                                passdoc += "<ns:IdentityDocuments><ns:IdentityCard>";
                                passdoc += "<ns:Number>" + i.SenderID_Number + @"</ns:Number>";
                                if (Country != "") { passdoc += "<ns:Country>" + Country + @"</ns:Country>"; }
                                passdoc += "</ns:IdentityCard></ns:IdentityDocuments>";
                            }

                            string passdob = "";
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
                                MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("SaveAPIRequestResponce");
                                _cmd.CommandType = CommandType.StoredProcedure;
                                _cmd.Parameters.AddWithValue("_API_ID", API_ID);
                                _cmd.Parameters.AddWithValue("_Client_ID", i.Client_ID);
                                _cmd.Parameters.AddWithValue("_Customer_ID", i.Customer_ID);
                                _cmd.Parameters.AddWithValue("_status", 0);
                                _cmd.Parameters.AddWithValue("_Function_name", CallFrom);
                                _cmd.Parameters.AddWithValue("_Remark", 0);
                                _cmd.Parameters.AddWithValue("_comments", SendTransferReq.Replace("'", "''"));
                                _cmd.Parameters.AddWithValue("_Record_Insert_DateTime", i.Record_Insert_DateTime);
                                _cmd.Parameters.AddWithValue("_Branch_ID", i.Branch_ID);
                                int msg1 = db_connection.ExecuteNonQueryProcedure(_cmd);
                            }
                            catch (Exception ex)
                            {
                                string error = ex.ToString().Replace("\'", "\\'");

                                MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("SaveException");
                                _cmd.CommandType = CommandType.StoredProcedure;
                                _cmd.Parameters.AddWithValue("_Record_Insert_DateTime", i.Record_Insert_DateTime);
                                _cmd.Parameters.AddWithValue("_error", error);
                                _cmd.Parameters.AddWithValue("_Client_ID", i.Client_ID);
                                int msg1 = db_connection.ExecuteNonQueryProcedure(_cmd);
                            }

                            HttpWebRequest webRequest = CreateWebRequest(_url, _action);
                            InsertSoapEnvelopeIntoWebRequest(soapEnvelopeXml, webRequest);
                            IAsyncResult asyncResult = webRequest.BeginGetResponse(null, null);
                            asyncResult.AsyncWaitHandle.WaitOne();
                            string soapResult = "";
                            try
                            {
                                using (WebResponse webResponse = webRequest.EndGetResponse(asyncResult))
                                {
                                    using (StreamReader rd = new StreamReader(webResponse.GetResponseStream()))
                                    {
                                        soapResult = rd.ReadToEnd();
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                //send complaince alert
                                string Record_DateTime_exception = i.Record_Insert_DateTime;
                                string notification_icon_exception = "aml-referd.jpg";
                                string notification_message_exception = "<span class='cls-admin'>GBG AML check <strong class='cls-cancel'> Request failed.</strong></span><span class='cls-customer'></span>";
                                CompanyInfo.save_notification_compliance(notification_message_exception, notification_icon_exception, Convert.ToString(i.Customer_ID), Convert.ToDateTime(Record_DateTime_exception), i.Client_ID, 1, i.User_ID, i.Branch_ID, 0, 1, 1, 0, context);

                                string error = ex.ToString().Replace("\'", "\\'");
                                MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("SaveException");
                                _cmd.CommandType = CommandType.StoredProcedure;
                                _cmd.Parameters.AddWithValue("_Record_Insert_DateTime", i.Record_Insert_DateTime);
                                _cmd.Parameters.AddWithValue("_error", error);
                                _cmd.Parameters.AddWithValue("_Client_ID", i.Client_ID);
                                int msg1 = db_connection.ExecuteNonQueryProcedure(_cmd);


                                DataTable dtc = CompanyInfo.get(i.Client_ID, context);
                                mail_send = string.Empty;
                                string sendmsg = "GBG AML check Request failed. Please contact GBG support team. ";
                                string EmailID = "";
                                DataTable dt_admin_Email_list = (DataTable)CompanyInfo.getAdminEmailList(i.Client_ID, i.Branch_ID);
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
                                CompanyInfo.Send_Mail(dtc, EmailID, body, subject, Convert.ToInt32(i.Client_ID), Convert.ToInt32(i.Branch_ID), "", "", "", context);

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
                                                        string description1 = childd["Description"].InnerText.ToString();
                                                        string Record_DateTime = i.Record_Insert_DateTime;
                                                        string notification_icon = "pep-match-not-found.jpg";
                                                        //string notification_message = "<span class='cls-admin'>International Sanctions: <strong class='cls-cancel'>.</strong><br/>" + description1 + "</span><span class='cls-customer'></span>";
                                                        string notification_message = "<span class='cls-admin'> - <span class='cls-cancel'><strong> International PEP Alert </strong>- </span><span class='cls-addr-confirm'>" + description1 + " </span></span></span><span class='cls-customer'></span>";
                                                        CompanyInfo.save_notification_compliance(notification_message, notification_icon, Convert.ToString(i.Customer_ID), Convert.ToDateTime(Record_DateTime), i.Client_ID, 1, i.User_ID, i.Branch_ID, 0, 1, 1, 0, context);
                                                        flag1++;

                                                    }
                                                }
                                            }
                                        }
                                        else if (node.InnerText.Contains("International Sanctions"))
                                        {
                                            if (child.Name == "Mismatch")
                                            {
                                                foreach (XmlNode childd in child.ChildNodes)
                                                {
                                                    string description1 = childd["Description"].InnerText.ToString();
                                                    string Record_DateTime = i.Record_Insert_DateTime;
                                                    string notification_icon = "pep-match-not-found.jpg";
                                                    //  string notification_message = "<span class='cls-admin'>International Sanctions: <strong class='cls-cancel'>.</strong><br/>" + description1 + "</span><span class='cls-customer'></span>";
                                                    string notification_message = "<span class='cls-admin'> - <span class='cls-cancel'><strong> International Sanctions Alert </strong>- </span><span class='cls-addr-confirm'>" + description1 + " </span></span></span><span class='cls-customer'></span>";
                                                    CompanyInfo.save_notification_compliance(notification_message, notification_icon, Convert.ToString(i.Customer_ID), Convert.ToDateTime(Record_DateTime), i.Client_ID, 1, i.User_ID, i.Branch_ID, 0, 1, 1, 0, context);
                                                    flag2++;
                                                }
                                            }
                                        }
                                    }
                                }
                                try
                                {
                                    if (Convert.ToInt32(i.Customer_ID) > 0 && i.Beneficiary_ID == 0)
                                    {
                                        string Query = "UPDATE `customer_registration` SET `ReKyc_Eligibility` = '0' WHERE (`Customer_ID` = " + i.Customer_ID + ")";
                                        MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("Default_SP");
                                        _cmd.CommandType = CommandType.StoredProcedure;
                                        _cmd.Parameters.AddWithValue("_Query", Query);
                                        db_connection.ExecuteNonQueryProcedure(_cmd);
                                    }

                                }
                                catch
                                {

                                }

                                if (flag1 > 0 && flag2 > 0)
                                {
                                    try
                                    {
                                        MySqlConnector.MySqlCommand cmd_update = new MySqlConnector.MySqlCommand("update documents_details set aml_sanctions_flag=3 where SenderID_ID=@SenderID_ID");
                                        cmd_update.Parameters.AddWithValue("@SenderID_ID", i.SenderID_ID);
                                        db_connection.ExecuteNonQueryProcedure(cmd_update);
                                    }
                                    catch { }
                                }
                                else if (flag1 > 0)
                                {
                                    try
                                    {
                                        MySqlConnector.MySqlCommand cmd_update = new MySqlConnector.MySqlCommand("update documents_details set aml_sanctions_flag=1 where SenderID_ID=@SenderID_ID");
                                        cmd_update.Parameters.AddWithValue("@SenderID_ID", i.SenderID_ID);
                                        db_connection.ExecuteNonQueryProcedure(cmd_update);
                                    }
                                    catch { }
                                }
                                else if (flag2 > 0)
                                {
                                    try
                                    {
                                        MySqlConnector.MySqlCommand cmd_update = new MySqlConnector.MySqlCommand("update documents_details set aml_sanctions_flag=2 where SenderID_ID=@SenderID_ID");
                                        cmd_update.Parameters.AddWithValue("@SenderID_ID", i.SenderID_ID);
                                        db_connection.ExecuteNonQueryProcedure(cmd_update);
                                    }
                                    catch { }
                                }
                                try
                                {
                                    int status = 1; string Function_name = CallFrom;
                                    Remark = Convert.ToString(CompanyInfo.getAPIStatus(Bandtext, i.Client_ID));
                                    if (Remark != null && Remark != "")
                                        Status = 0;

                                    MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("UpdateCustomerBandText");
                                    _cmd.CommandType = CommandType.StoredProcedure;
                                    _cmd.Parameters.AddWithValue("_Client_ID", i.Client_ID);
                                    _cmd.Parameters.AddWithValue("_Customer_ID", i.Customer_ID);
                                    _cmd.Parameters.AddWithValue("_Remark", Remark);
                                    int msg = db_connection.ExecuteNonQueryProcedure(_cmd);
                                    _cmd.Dispose();

                                    _cmd = new MySqlConnector.MySqlCommand("SaveAPIRequestResponce");
                                    _cmd.CommandType = CommandType.StoredProcedure;
                                    _cmd.Parameters.AddWithValue("_API_ID", API_ID);
                                    _cmd.Parameters.AddWithValue("_Client_ID", i.Client_ID);
                                    _cmd.Parameters.AddWithValue("_Customer_ID", i.Customer_ID);
                                    _cmd.Parameters.AddWithValue("_status", status);
                                    _cmd.Parameters.AddWithValue("_Function_name", Function_name);
                                    _cmd.Parameters.AddWithValue("_Remark", Remark);
                                    _cmd.Parameters.AddWithValue("_comments", soapResult.Replace("'", "''"));
                                    _cmd.Parameters.AddWithValue("_Record_Insert_DateTime", i.Record_Insert_DateTime);
                                    _cmd.Parameters.AddWithValue("_Branch_ID", i.Branch_ID);
                                    int msg1 = db_connection.ExecuteNonQueryProcedure(_cmd);

                                }
                                catch (Exception ex)
                                {
                                    string error = ex.ToString().Replace("\'", "\\'");

                                    MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("SaveException");
                                    _cmd.CommandType = CommandType.StoredProcedure;
                                    _cmd.Parameters.AddWithValue("_Record_Insert_DateTime", i.Record_Insert_DateTime);
                                    _cmd.Parameters.AddWithValue("_error", error);
                                    _cmd.Parameters.AddWithValue("_Client_ID", i.Client_ID);
                                    int msg1 = db_connection.ExecuteNonQueryProcedure(_cmd);
                                }
                                try
                                {
                                    if (Remark == "2" || Remark == "1")//alert or refer then  send mail to admin
                                    {
                                        string Record_DateTime = i.Record_Insert_DateTime;
                                        string notification_icon = "aml-referd.jpg";
                                        string notification_message = "<span class='cls-admin'>GBG AML check result is <strong class='cls-cancel'>" + Bandtext + ".</strong></span><span class='cls-customer'></span>";
                                        CompanyInfo.save_notification_compliance(notification_message, notification_icon, Convert.ToString(i.Customer_ID), Convert.ToDateTime(Record_DateTime), i.Client_ID, 1, i.User_ID, i.Branch_ID, 0, 1, 1, 0, context);
                                    }
                                    else
                                    {
                                        string Record_DateTime = i.Record_Insert_DateTime;
                                        string notification_icon = "primary-id-upload.jpg";
                                        string notification_message = "<span class='cls-admin'>GBG AML check result is <strong class='cls-priamary'>" + Bandtext + ".</strong></span><span class='cls-customer'></span>";
                                        CompanyInfo.save_notification_compliance(notification_message, notification_icon, Convert.ToString(i.Customer_ID), Convert.ToDateTime(Record_DateTime), i.Client_ID, 1, i.User_ID, i.Branch_ID, 0, 1, 1, 0, context);
                                    }

                                }
                                catch (Exception e)
                                {
                                }
                                try
                                {
                                    if (Remark == "1" || Remark == "2" || Remark == "3")//alert or refer then  send mail to admin
                                    {

                                        DataTable dtc = CompanyInfo.get(i.Client_ID, context);

                                        if (dtc.Rows.Count > 0)
                                        {
                                            EmailUrl = Convert.ToString(dtc.Rows[0]["Company_URL_Admin"]);
                                            Company_Name = Convert.ToString(dtc.Rows[0]["Company_Name"]);
                                            //HttpContext.Current.Session["BaseCurrency_Code"] = Convert.ToString(dtc.Rows[0]["BaseCurrency_Code"]);
                                            //HttpContext.Current.Session["BaseCurrency_TimeZone"] = Convert.ToString(dtc.Rows[0]["BaseCurrency_TimeZone"]);
                                            //HttpContext.Current.Session["BaseCurrency_Sign"] = Convert.ToString(dtc.Rows[0]["BaseCurrency_Sign"]);
                                            //HttpContext.Current.Session["BaseCurrency_Country"] = Convert.ToString(dtc.Rows[0]["BaseCurrency_Country"]);
                                            //HttpContext.Current.Session["BaseCountry_ID"] = Convert.ToString(dtc.Rows[0]["BaseCountry_ID"]);
                                            //HttpContext.Current.Session["EmailUrl"] = Convert.ToString(dtc.Rows[0]["Company_URL_Admin"]);
                                            //HttpContext.Current.Session["CustomerURL"] = Convert.ToString(dtc.Rows[0]["Company_URL_Customer"]);
                                            //HttpContext.Current.Session["Company_Name"] = Convert.ToString(dtc.Rows[0]["Company_Name"]);
                                            //HttpContext.Current.Session["Cancel_Transaction_Hours"] = Convert.ToString(dtc.Rows[0]["Cancel_Transaction_Hours"]);
                                        }

                                        MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("customer_details_by_param");
                                        _cmd.CommandType = CommandType.StoredProcedure;
                                        string _whereclause = " and cr.Client_ID=" + i.Client_ID + " and cr.Customer_ID=" + i.Customer_ID;
                                        //  DataTable d1 = (DataTable)mtsmethods.GetCustDetailsByID(c.Customer_ID); //get customer details by id

                                        _cmd.Parameters.AddWithValue("_whereclause", _whereclause);
                                        _cmd.Parameters.AddWithValue("_SecurityKey", CompanyInfo.SecurityKey());
                                        DataTable d1 = db_connection.ExecuteQueryDataTableProcedure(_cmd);
                                        string sendmsg = "Response from third Party AML check for  " + d1.Rows[0]["WireTransfer_ReferanceNo"] + "   is   " + Bandtext + "";
                                        //      string sendmsg = "Probable duplicate  match found for newly registered customer " + c.First_Name + " " + c.Middle_Name + " " + c.Last_Name + "  with reference number " + c.WireTransfer_ReferanceNo + "";
                                        //string sendmsg = " While adding Customer " + c.First_Name + "  " + c.Last_Name + Convert.ToString(c.WireTransfer_ReferanceNo) + " ,there was a probable match found . Please check Customer profile. ";





                                        string EmailID = "";
                                        DataTable dt_admin_Email_list = (DataTable)CompanyInfo.getAdminEmailList(i.Client_ID, i.Branch_ID);
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

                                        subject = "" + Company_Name + " - Compliance - Alert - " + d1.Rows[0]["WireTransfer_ReferanceNo"];
                                        //subject = "" + HttpContext.Current.Session["Company_Name"] + " -  Incomplete Customer Registration Details " + c.WireTransfer_ReferanceNo;
                                        //    mail_send = (string)mtsmethods.Send_Mail(email, body, subject, Convert.ToInt32(c.Client_ID), Convert.ToInt32(c.Branch_ID), "Alert Admins", "", "");

                                        mail_send = (string)CompanyInfo.Send_Mail(dtc, EmailID, body, subject, i.Client_ID, i.Branch_ID, "", "", "", context);
                                    }
                                }
                                catch (Exception ae)
                                {
                                    //string stattus = (string)mtsmethods.InsertErrorLogDetails(ae.Message.Replace("\'", "\\'"), user_id, "Check Credit Safe", i.Branch_ID, i.Client_ID);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //string stattus = (string)mtsmethods.InsertErrorLogDetails(ex.Message.Replace("\'", "\\'"), user_id, "Check Credit Safe", i.Branch_ID, i.Client_ID);
            }

            string[] response = { Status.ToString(), Bandtext, flag1.ToString(), flag2.ToString() };
            return response;
        }
        public string[] CheckCreditSafe(Model.Doc i, DataTable dc, string CallFrom, HttpContext context)
        {
            int Status = 1; string Remark = string.Empty; string mail_send = string.Empty; string EmailUrl = string.Empty; string Company_Name = string.Empty;
            string Bandtext = "";
            int flag1 = 0, flag2 = 0;//pep or international
            try
            {
                MySqlConnector.MySqlCommand cmdupdate1 = new MySqlConnector.MySqlCommand("Get_Permissions");
                cmdupdate1.CommandType = CommandType.StoredProcedure;
                cmdupdate1.Parameters.AddWithValue("Per_ID", 32);
                cmdupdate1.Parameters.AddWithValue("ClientID", i.Client_ID);
                DataTable pm = db_connection.ExecuteQueryDataTableProcedure(cmdupdate1);
                if (pm.Rows.Count > 0)
                {
                    if (Convert.ToString(pm.Rows[0]["Status_ForCustomer"]) == "0")
                    {
                        int API_ID = 1;//Credit Safe API ID
                        MySqlConnector.MySqlCommand cmd = new MySqlConnector.MySqlCommand("GetAPIDetails_byCountry");//("GetAPIDetails");
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("_API_ID", API_ID);//Credit Safe API ID
                        cmd.Parameters.AddWithValue("_Client_ID", i.Client_ID);
                        cmd.Parameters.AddWithValue("_status", 0);// API Status
                        cmd.Parameters.AddWithValue("_Customer_ID", i.Customer_ID);//for country
                        DataTable dtApi = db_connection.ExecuteQueryDataTableProcedure(cmd);

                        if (dtApi.Rows.Count > 0)
                        {
                            //get Customer details

                            string First_Name = Convert.ToString(dc.Rows[0]["First_Name"]);
                            string Middle_Name = Convert.ToString(dc.Rows[0]["Middle_Name"]);
                            string Last_Name = Convert.ToString(dc.Rows[0]["Last_Name"]);
                            string Country1 = Convert.ToString(dc.Rows[0]["Country_Name"]).ToLower();
                            string House_Number = Convert.ToString(dc.Rows[0]["House_Number"]);
                            string Street = Convert.ToString(dc.Rows[0]["Street"]);
                            string City = Convert.ToString(dc.Rows[0]["City_Name"]);
                            string Post_Code = Convert.ToString(dc.Rows[0]["Post_Code"]);
                            i.Record_Insert_DateTime = (string)CompanyInfo.gettime(i.Client_ID, i.Customer_ID, i.Country_ID, context);
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
                            string expiry = Convert.ToString(i.SenderID_ExpiryDate);
                            if (expiry != "" && expiry != null)
                            {
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

                            if (i.IDName_ID == 1)// if Passport
                            {
                                passdoc += "<ns:IdentityDocuments><ns:InternationalPassport>";
                                passdoc += "<ns:Number>" + i.SenderID_Number + @"</ns:Number> " + passexpiry + "";
                                if (Country != "") { passdoc += "<ns:CountryOfOrigin>" + Country + @"</ns:CountryOfOrigin>"; }
                                passdoc += "</ns:InternationalPassport></ns:IdentityDocuments>";

                            }
                            else if (i.IDName_ID == 2 && Country != "")//if Driving licence
                            {
                                passdoc += "<ns:IdentityDocuments><ns:" + Country + @"><ns:DrivingLicence>";
                                passdoc += "<ns:Number>" + i.SenderID_Number + @"</ns:Number> " + passexpiry + "";
                                passdoc += "</ns:DrivingLicence></ns:" + Country + @"></ns:IdentityDocuments>";
                            }
                            else //if (i.IDName_ID == 3)//EU Nationality Card
                            {
                                passdoc += "<ns:IdentityDocuments><ns:IdentityCard>";
                                passdoc += "<ns:Number>" + i.SenderID_Number + @"</ns:Number>";
                                if (Country != "") { passdoc += "<ns:Country>" + Country + @"</ns:Country>"; }
                                passdoc += "</ns:IdentityCard></ns:IdentityDocuments>";
                            }

                            string passdob = "";
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
                                MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("SaveAPIRequestResponce");
                                _cmd.CommandType = CommandType.StoredProcedure;
                                _cmd.Parameters.AddWithValue("_API_ID", API_ID);
                                _cmd.Parameters.AddWithValue("_Client_ID", i.Client_ID);
                                _cmd.Parameters.AddWithValue("_Customer_ID", i.Customer_ID);
                                _cmd.Parameters.AddWithValue("_status", 0);
                                _cmd.Parameters.AddWithValue("_Function_name", CallFrom);
                                _cmd.Parameters.AddWithValue("_Remark", 0);
                                _cmd.Parameters.AddWithValue("_comments", SendTransferReq.Replace("'", "''"));
                                _cmd.Parameters.AddWithValue("_Record_Insert_DateTime", i.Record_Insert_DateTime);
                                _cmd.Parameters.AddWithValue("_Branch_ID", i.Branch_ID);
                                int msg1 = db_connection.ExecuteNonQueryProcedure(_cmd);
                            }
                            catch (Exception ex)
                            {
                                string error = ex.ToString().Replace("\'", "\\'");

                                MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("SaveException");
                                _cmd.CommandType = CommandType.StoredProcedure;
                                _cmd.Parameters.AddWithValue("_Record_Insert_DateTime", i.Record_Insert_DateTime);
                                _cmd.Parameters.AddWithValue("_error", error);
                                _cmd.Parameters.AddWithValue("_Client_ID", i.Client_ID);
                                int msg1 = db_connection.ExecuteNonQueryProcedure(_cmd);
                            }

                            HttpWebRequest webRequest = CreateWebRequest(_url, _action);
                            InsertSoapEnvelopeIntoWebRequest(soapEnvelopeXml, webRequest);
                            IAsyncResult asyncResult = webRequest.BeginGetResponse(null, null);
                            asyncResult.AsyncWaitHandle.WaitOne();
                            string soapResult = "";
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
                                //send complaince alert
                                string Record_DateTime_exception = i.Record_Insert_DateTime;
                                string notification_icon_exception = "aml-referd.jpg";
                                string notification_message_exception = "<span class='cls-admin'>GBG AML check <strong class='cls-cancel'> Request failed.</strong></span><span class='cls-customer'></span>";
                                CompanyInfo.save_notification_compliance(notification_message_exception, notification_icon_exception, Convert.ToString(i.Customer_ID), Convert.ToDateTime(Record_DateTime_exception), i.Client_ID, 1, i.User_ID, i.Branch_ID, 0, 1, 1, 0, context);

                                string error = ex.ToString().Replace("\'", "\\'");
                                MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("SaveException");
                                _cmd.CommandType = CommandType.StoredProcedure;
                                _cmd.Parameters.AddWithValue("_Record_Insert_DateTime", i.Record_Insert_DateTime);
                                _cmd.Parameters.AddWithValue("_error", error);
                                _cmd.Parameters.AddWithValue("_Client_ID", i.Client_ID);
                                int msg1 = db_connection.ExecuteNonQueryProcedure(_cmd);


                                DataTable dtc = CompanyInfo.get(i.Client_ID, context);
                                mail_send = string.Empty;
                                string sendmsg = "GBG AML check Request failed. Please contact GBG support team. ";
                                string EmailID = "";
                                DataTable dt_admin_Email_list = (DataTable)CompanyInfo.getAdminEmailList(i.Client_ID, i.Branch_ID);
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
                                CompanyInfo.Send_Mail(dtc, EmailID, body, subject, Convert.ToInt32(i.Client_ID), Convert.ToInt32(i.Branch_ID), "", "", "", context);

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
                                                        string description1 = childd["Description"].InnerText.ToString();
                                                        string Record_DateTime = i.Record_Insert_DateTime;
                                                        string notification_icon = "pep-match-not-found.jpg";
                                                        //string notification_message = "<span class='cls-admin'>International Sanctions: <strong class='cls-cancel'>.</strong><br/>" + description1 + "</span><span class='cls-customer'></span>";
                                                        string notification_message = "<span class='cls-admin'> - <span class='cls-cancel'><strong> International PEP Alert </strong>- </span><span class='cls-addr-confirm'>" + description1 + " </span></span></span><span class='cls-customer'></span>";
                                                        CompanyInfo.save_notification_compliance(notification_message, notification_icon, Convert.ToString(i.Customer_ID), Convert.ToDateTime(Record_DateTime), i.Client_ID, 1, i.User_ID, i.Branch_ID, 0, 1, 1, 0, context);
                                                        flag1++;

                                                    }
                                                }
                                            }
                                        }
                                        else if (node.InnerText.Contains("International Sanctions"))
                                        {
                                            if (child.Name == "Mismatch")
                                            {
                                                foreach (XmlNode childd in child.ChildNodes)
                                                {
                                                    string description1 = childd["Description"].InnerText.ToString();
                                                    string Record_DateTime = i.Record_Insert_DateTime;
                                                    string notification_icon = "pep-match-not-found.jpg";
                                                    //  string notification_message = "<span class='cls-admin'>International Sanctions: <strong class='cls-cancel'>.</strong><br/>" + description1 + "</span><span class='cls-customer'></span>";
                                                    string notification_message = "<span class='cls-admin'> - <span class='cls-cancel'><strong> International Sanctions Alert </strong>- </span><span class='cls-addr-confirm'>" + description1 + " </span></span></span><span class='cls-customer'></span>";
                                                    CompanyInfo.save_notification_compliance(notification_message, notification_icon, Convert.ToString(i.Customer_ID), Convert.ToDateTime(Record_DateTime), i.Client_ID, 1, i.User_ID, i.Branch_ID, 0, 1, 1, 0, context);
                                                    flag2++;
                                                }
                                            }
                                        }
                                    }
                                }
                                try
                                {
                                    if (Convert.ToInt32(i.Customer_ID) > 0 && i.Beneficiary_ID == 0)
                                    {
                                        string Query = "UPDATE `customer_registration` SET `ReKyc_Eligibility` = '0' WHERE (`Customer_ID` = " + i.Customer_ID + ")";
                                        MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("Default_SP");
                                        _cmd.CommandType = CommandType.StoredProcedure;
                                        _cmd.Parameters.AddWithValue("_Query", Query);
                                        db_connection.ExecuteNonQueryProcedure(_cmd);
                                    }

                                }
                                catch
                                {

                                }

                                if (flag1 > 0 && flag2 > 0)
                                {
                                    try
                                    {
                                        MySqlConnector.MySqlCommand cmd_update = new MySqlConnector.MySqlCommand("update documents_details set aml_sanctions_flag=3 where SenderID_ID=@SenderID_ID");
                                        cmd_update.Parameters.AddWithValue("@SenderID_ID", i.SenderID_ID);
                                        db_connection.ExecuteNonQueryProcedure(cmd_update);
                                    }
                                    catch { }
                                }
                                else if (flag1 > 0)
                                {
                                    try
                                    {
                                        MySqlConnector.MySqlCommand cmd_update = new MySqlConnector.MySqlCommand("update documents_details set aml_sanctions_flag=1 where SenderID_ID=@SenderID_ID");
                                        cmd_update.Parameters.AddWithValue("@SenderID_ID", i.SenderID_ID);
                                        db_connection.ExecuteNonQueryProcedure(cmd_update);
                                    }
                                    catch { }
                                }
                                else if (flag2 > 0)
                                {
                                    try
                                    {
                                        MySqlConnector.MySqlCommand cmd_update = new MySqlConnector.MySqlCommand("update documents_details set aml_sanctions_flag=2 where SenderID_ID=@SenderID_ID");
                                        cmd_update.Parameters.AddWithValue("@SenderID_ID", i.SenderID_ID);
                                        db_connection.ExecuteNonQueryProcedure(cmd_update);
                                    }
                                    catch { }
                                }
                                try
                                {
                                    int status = 1; string Function_name = CallFrom;
                                    Remark = Convert.ToString(CompanyInfo.getAPIStatus(Bandtext, i.Client_ID));
                                    if (Remark != null && Remark != "")
                                        Status = 0;

                                    MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("UpdateCustomerBandText");
                                    _cmd.CommandType = CommandType.StoredProcedure;
                                    _cmd.Parameters.AddWithValue("_Client_ID", i.Client_ID);
                                    _cmd.Parameters.AddWithValue("_Customer_ID", i.Customer_ID);
                                    _cmd.Parameters.AddWithValue("_Remark", Remark);
                                    int msg = db_connection.ExecuteNonQueryProcedure(_cmd);
                                    _cmd.Dispose();

                                    _cmd = new MySqlConnector.MySqlCommand("SaveAPIRequestResponce");
                                    _cmd.CommandType = CommandType.StoredProcedure;
                                    _cmd.Parameters.AddWithValue("_API_ID", API_ID);
                                    _cmd.Parameters.AddWithValue("_Client_ID", i.Client_ID);
                                    _cmd.Parameters.AddWithValue("_Customer_ID", i.Customer_ID);
                                    _cmd.Parameters.AddWithValue("_status", status);
                                    _cmd.Parameters.AddWithValue("_Function_name", Function_name);
                                    _cmd.Parameters.AddWithValue("_Remark", Remark);
                                    _cmd.Parameters.AddWithValue("_comments", soapResult.Replace("'", "''"));
                                    _cmd.Parameters.AddWithValue("_Record_Insert_DateTime", i.Record_Insert_DateTime);
                                    _cmd.Parameters.AddWithValue("_Branch_ID", i.Branch_ID);
                                    int msg1 = db_connection.ExecuteNonQueryProcedure(_cmd);

                                }
                                catch (Exception ex)
                                {
                                    string error = ex.ToString().Replace("\'", "\\'");

                                    MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("SaveException");
                                    _cmd.CommandType = CommandType.StoredProcedure;
                                    _cmd.Parameters.AddWithValue("_Record_Insert_DateTime", i.Record_Insert_DateTime);
                                    _cmd.Parameters.AddWithValue("_error", error);
                                    _cmd.Parameters.AddWithValue("_Client_ID", i.Client_ID);
                                    int msg1 = db_connection.ExecuteNonQueryProcedure(_cmd);
                                }
                                try
                                {
                                    if (Remark == "2" || Remark == "1")//alert or refer then  send mail to admin
                                    {
                                        string Record_DateTime = i.Record_Insert_DateTime;
                                        string notification_icon = "aml-referd.jpg";
                                        string notification_message = "<span class='cls-admin'>GBG AML check result is <strong class='cls-cancel'>" + Bandtext + ".</strong></span><span class='cls-customer'></span>";
                                        CompanyInfo.save_notification_compliance(notification_message, notification_icon, Convert.ToString(i.Customer_ID), Convert.ToDateTime(Record_DateTime), i.Client_ID, 1, i.User_ID, i.Branch_ID, 0, 1, 1, 0, context);
                                    }
                                    else
                                    {
                                        string Record_DateTime = i.Record_Insert_DateTime;
                                        string notification_icon = "primary-id-upload.jpg";
                                        string notification_message = "<span class='cls-admin'>GBG AML check result is <strong class='cls-priamary'>" + Bandtext + ".</strong></span><span class='cls-customer'></span>";
                                        CompanyInfo.save_notification_compliance(notification_message, notification_icon, Convert.ToString(i.Customer_ID), Convert.ToDateTime(Record_DateTime), i.Client_ID, 1, i.User_ID, i.Branch_ID, 0, 1, 1, 0, context);
                                    }

                                }
                                catch (Exception e)
                                {
                                }
                                try
                                {
                                    if (Remark == "1" || Remark == "2" || Remark == "3")//alert or refer then  send mail to admin
                                    {

                                        DataTable dtc = CompanyInfo.get(i.Client_ID, context);

                                        if (dtc.Rows.Count > 0)
                                        {
                                            EmailUrl = Convert.ToString(dtc.Rows[0]["Company_URL_Admin"]);
                                            Company_Name = Convert.ToString(dtc.Rows[0]["Company_Name"]);
                                            //HttpContext.Current.Session["BaseCurrency_Code"] = Convert.ToString(dtc.Rows[0]["BaseCurrency_Code"]);
                                            //HttpContext.Current.Session["BaseCurrency_TimeZone"] = Convert.ToString(dtc.Rows[0]["BaseCurrency_TimeZone"]);
                                            //HttpContext.Current.Session["BaseCurrency_Sign"] = Convert.ToString(dtc.Rows[0]["BaseCurrency_Sign"]);
                                            //HttpContext.Current.Session["BaseCurrency_Country"] = Convert.ToString(dtc.Rows[0]["BaseCurrency_Country"]);
                                            //HttpContext.Current.Session["BaseCountry_ID"] = Convert.ToString(dtc.Rows[0]["BaseCountry_ID"]);
                                            //HttpContext.Current.Session["EmailUrl"] = Convert.ToString(dtc.Rows[0]["Company_URL_Admin"]);
                                            //HttpContext.Current.Session["CustomerURL"] = Convert.ToString(dtc.Rows[0]["Company_URL_Customer"]);
                                            //HttpContext.Current.Session["Company_Name"] = Convert.ToString(dtc.Rows[0]["Company_Name"]);
                                            //HttpContext.Current.Session["Cancel_Transaction_Hours"] = Convert.ToString(dtc.Rows[0]["Cancel_Transaction_Hours"]);
                                        }

                                        MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("customer_details_by_param");
                                        _cmd.CommandType = CommandType.StoredProcedure;
                                        string _whereclause = " and cr.Client_ID=" + i.Client_ID + " and cr.Customer_ID=" + i.Customer_ID;
                                        //  DataTable d1 = (DataTable)mtsmethods.GetCustDetailsByID(c.Customer_ID); //get customer details by id

                                        _cmd.Parameters.AddWithValue("_whereclause", _whereclause);
                                        _cmd.Parameters.AddWithValue("_SecurityKey", CompanyInfo.SecurityKey());
                                        DataTable d1 = db_connection.ExecuteQueryDataTableProcedure(_cmd);
                                        string sendmsg = "Response from third Party AML check for  " + d1.Rows[0]["WireTransfer_ReferanceNo"] + "   is   " + Bandtext + "";
                                        //      string sendmsg = "Probable duplicate  match found for newly registered customer " + c.First_Name + " " + c.Middle_Name + " " + c.Last_Name + "  with reference number " + c.WireTransfer_ReferanceNo + "";
                                        //string sendmsg = " While adding Customer " + c.First_Name + "  " + c.Last_Name + Convert.ToString(c.WireTransfer_ReferanceNo) + " ,there was a probable match found . Please check Customer profile. ";





                                        string EmailID = "";
                                        DataTable dt_admin_Email_list = (DataTable)CompanyInfo.getAdminEmailList(i.Client_ID, i.Branch_ID);
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

                                        subject = "" + Company_Name + " - Compliance - Alert - " + d1.Rows[0]["WireTransfer_ReferanceNo"];
                                        //subject = "" + HttpContext.Current.Session["Company_Name"] + " -  Incomplete Customer Registration Details " + c.WireTransfer_ReferanceNo;
                                        //    mail_send = (string)mtsmethods.Send_Mail(email, body, subject, Convert.ToInt32(c.Client_ID), Convert.ToInt32(c.Branch_ID), "Alert Admins", "", "");

                                        mail_send = (string)CompanyInfo.Send_Mail(dtc, EmailID, body, subject, i.Client_ID, i.Branch_ID, "", "", "", context);
                                    }
                                }
                                catch (Exception ae)
                                {
                                    //string stattus = (string)mtsmethods.InsertErrorLogDetails(ae.Message.Replace("\'", "\\'"), user_id, "Check Credit Safe", i.Branch_ID, i.Client_ID);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //string stattus = (string)mtsmethods.InsertErrorLogDetails(ex.Message.Replace("\'", "\\'"), user_id, "Check Credit Safe", i.Branch_ID, i.Client_ID);
            }

            string[] response = { Status.ToString(), Bandtext, flag1.ToString(), flag2.ToString() };
            return response;
        }

        public string send_mails(DataTable dtc, int client_id, int branch_id, string full_name, string Reference, string msg, string email_id, HttpContext context)
        {
            try
            {
                string email = email_id;
                string subject = string.Empty;
                string body = string.Empty;
                int client = Convert.ToInt32(client_id);
                int branch = Convert.ToInt32(branch_id);
                HttpWebRequest httpRequest;
                string URL = Convert.ToString(dtc.Rows[0]["Company_URL_Admin"]);
                httpRequest = (HttpWebRequest)WebRequest.Create(URL + "Email/my-account-change.html");
                //}
                httpRequest.UserAgent = "Code Sample Web Client";
                HttpWebResponse webResponse = (HttpWebResponse)httpRequest.GetResponse();
                using (StreamReader reader = new StreamReader(webResponse.GetResponseStream()))
                {
                    body = reader.ReadToEnd();
                }
                body = body.Replace("[name]", full_name);
                body = body.Replace("[msg]", msg);

                subject = "[company_name] -  Profile updated successfully " + Reference;
                string mail_send = (string)CompanyInfo.Send_Mail(dtc, email, body, subject, client, branch, "standard_email", "", "", context);
                if (mail_send == "Success")
                {
                    msg = "Success";
                }
                else //not success
                {
                    msg = "NotSuccess";
                }
            }
            catch
            {
                msg = "NotSuccess";
                return msg;
            }
            return msg;
        }
        public bool chkValidExtension(string ext)
        {
            string[] PosterAllowedExtensions = new string[] { ".jpeg", ".jpg", ".png", ".bmp" };
            for (int i = 0; i < PosterAllowedExtensions.Length; i++)
            {
                if (ext.ToLower() == PosterAllowedExtensions[i])
                    return true;
            }
            return false;
        }

        public bool IsPDFFile(string ext)
        {
            string[] PosterAllowedExtensions = new string[] { ".pdf" };
            for (int i = 0; i < PosterAllowedExtensions.Length; i++)
            {
                if (ext.ToLower() == PosterAllowedExtensions[i])
                    return true;
            }
            return false;
        }

        private void SavePDFWithWaterMark(IFormFile postedfile, string folderPath, string filenameWithExtension, Model.Document c)
        {
            HttpPostedFileBase httpPostedFile = postedfile.ToHttpPostedFileBase();

            string _SecurityKey = srvCommon.SecurityKey();

            MySqlConnector.MySqlCommand cmd = new MySqlConnector.MySqlCommand("sp_select_company_details_by_param");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("_securityKey", _SecurityKey);
            cmd.Parameters.AddWithValue("_clientId", c.Client_ID);

            DataTable ds = db_connection.ExecuteQueryDataTableProcedure(cmd);

            string watermarktext = (ds.Rows[0]["Company_Name"]).ToString();
            if (IsPDFFile(Path.GetExtension(filenameWithExtension)))
            {
                try
                {
                    Color color = Color.FromArgb(200, 0, 0, 255);

                    Stream fs = postedfile.OpenReadStream();
                    BinaryReader br = new BinaryReader(fs);
                    byte[] bytes = br.ReadBytes((Int32)fs.Length);
                    string filename = postedfile.FileName;
                    using (PdfReader PDFReader = new PdfReader(bytes))
                    {
                        using (FileStream Stream = new FileStream(folderPath + filenameWithExtension, FileMode.Create, FileAccess.Write))
                        {
                            using (PdfStamper PDFStamper = new PdfStamper(PDFReader, Stream))
                            {
                                for (int iCount = 0; iCount < PDFStamper.Reader.NumberOfPages; iCount++)
                                {
                                    PdfContentByte PDFData = PDFStamper.GetOverContent(iCount + 1);
                                    BaseFont baseFont = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.WINANSI, BaseFont.EMBEDDED);
                                    PDFData.BeginText();
                                    PDFData.SetColorFill(CMYKColor.LIGHT_GRAY);


                                    //PDFData.SetFontAndSize(baseFont, 80);

                                    //PDFData.ShowTextAligned(PdfContentByte.ALIGN_CENTER, watermarktext, 300, 400, 45);


                                    string watermarkstr = watermarktext + "   " + watermarktext + "  " + watermarktext + "   " + watermarktext + "  " + watermarktext + "   " + watermarktext + "   " + watermarktext + "  " + watermarktext + "   " + watermarktext;
                                    PDFData.SetFontAndSize(baseFont, 12);
                                    //line-1
                                    PDFData.ShowTextAligned(PdfContentByte.ALIGN_CENTER, " " + watermarkstr + " " + watermarkstr, 100, 670, 45);

                                    //line0
                                    PDFData.ShowTextAligned(PdfContentByte.ALIGN_CENTER, " " + watermarkstr + " " + watermarkstr, 100, 500, 45);

                                    //line1
                                    PDFData.ShowTextAligned(PdfContentByte.ALIGN_CENTER, " " + watermarkstr + " " + watermarkstr, 200, 450, 45);

                                    //line 2
                                    PDFData.ShowTextAligned(PdfContentByte.ALIGN_CENTER, " " + watermarkstr + " " + watermarkstr, 120, 230, 45);

                                    //line 3
                                    PDFData.ShowTextAligned(PdfContentByte.ALIGN_CENTER, " " + watermarkstr + " " + watermarkstr, 300, 280, 45); //1st para decrease come back,2nd para inscresed goes up.

                                    ///line 4
                                    PDFData.ShowTextAligned(PdfContentByte.ALIGN_CENTER, " " + watermarkstr + " " + watermarkstr, 400, 230, 45); //1st para decrease come back,2nd para inscresed goes up.

                                    //line 5
                                    PDFData.ShowTextAligned(PdfContentByte.ALIGN_CENTER, " " + watermarkstr + " " + watermarkstr, 500, 160, 45); //1st para decrease come back,2nd para inscresed goes up.
                                    //PDFData.ShowTextAligned(PdfContentByte.ALIGN_CENTER, watermarktext + "  " + "  " + watermarktext, 500, 1000, 45);

                                    PDFData.EndText();
                                }
                            }
                        }
                        //Stream.Close();
                    }
                    //PDFStamper.Close();
                    //PDFReader.Close();
                }
                catch (Exception e)
                { }
            }

        }
        private void SavePDFWithWaterMark(IFormFile postedfile, string folderPath, string filenameWithExtension, Model.Doc c)
        {
            HttpPostedFileBase httpPostedFile = postedfile.ToHttpPostedFileBase();

            string _SecurityKey = srvCommon.SecurityKey();

            MySqlConnector.MySqlCommand cmd = new MySqlConnector.MySqlCommand("sp_select_company_details_by_param");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("_securityKey", _SecurityKey);
            cmd.Parameters.AddWithValue("_clientId", c.Client_ID);

            DataTable ds = db_connection.ExecuteQueryDataTableProcedure(cmd);

            string watermarktext = (ds.Rows[0]["Company_Name"]).ToString();
            if (IsPDFFile(Path.GetExtension(filenameWithExtension)))
            {
                try
                {
                    Color color = Color.FromArgb(200, 0, 0, 255);

                    Stream fs = postedfile.OpenReadStream();
                    BinaryReader br = new BinaryReader(fs);
                    byte[] bytes = br.ReadBytes((Int32)fs.Length);
                    string filename = postedfile.FileName;
                    using (PdfReader PDFReader = new PdfReader(bytes))
                    {
                        using (FileStream Stream = new FileStream(folderPath + filenameWithExtension, FileMode.Create, FileAccess.Write))
                        {
                            using (PdfStamper PDFStamper = new PdfStamper(PDFReader, Stream))
                            {
                                for (int iCount = 0; iCount < PDFStamper.Reader.NumberOfPages; iCount++)
                                {
                                    PdfContentByte PDFData = PDFStamper.GetOverContent(iCount + 1);
                                    BaseFont baseFont = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.WINANSI, BaseFont.EMBEDDED);
                                    PDFData.BeginText();
                                    PDFData.SetColorFill(CMYKColor.LIGHT_GRAY);


                                    //PDFData.SetFontAndSize(baseFont, 80);

                                    //PDFData.ShowTextAligned(PdfContentByte.ALIGN_CENTER, watermarktext, 300, 400, 45);


                                    string watermarkstr = watermarktext + "   " + watermarktext + "  " + watermarktext + "   " + watermarktext + "  " + watermarktext + "   " + watermarktext + "   " + watermarktext + "  " + watermarktext + "   " + watermarktext;
                                    PDFData.SetFontAndSize(baseFont, 12);
                                    //line-1
                                    PDFData.ShowTextAligned(PdfContentByte.ALIGN_CENTER, " " + watermarkstr + " " + watermarkstr, 100, 670, 45);

                                    //line0
                                    PDFData.ShowTextAligned(PdfContentByte.ALIGN_CENTER, " " + watermarkstr + " " + watermarkstr, 100, 500, 45);

                                    //line1
                                    PDFData.ShowTextAligned(PdfContentByte.ALIGN_CENTER, " " + watermarkstr + " " + watermarkstr, 200, 450, 45);

                                    //line 2
                                    PDFData.ShowTextAligned(PdfContentByte.ALIGN_CENTER, " " + watermarkstr + " " + watermarkstr, 120, 230, 45);

                                    //line 3
                                    PDFData.ShowTextAligned(PdfContentByte.ALIGN_CENTER, " " + watermarkstr + " " + watermarkstr, 300, 280, 45); //1st para decrease come back,2nd para inscresed goes up.

                                    ///line 4
                                    PDFData.ShowTextAligned(PdfContentByte.ALIGN_CENTER, " " + watermarkstr + " " + watermarkstr, 400, 230, 45); //1st para decrease come back,2nd para inscresed goes up.

                                    //line 5
                                    PDFData.ShowTextAligned(PdfContentByte.ALIGN_CENTER, " " + watermarkstr + " " + watermarkstr, 500, 160, 45); //1st para decrease come back,2nd para inscresed goes up.
                                    //PDFData.ShowTextAligned(PdfContentByte.ALIGN_CENTER, watermarktext + "  " + "  " + watermarktext, 500, 1000, 45);

                                    PDFData.EndText();
                                }
                            }
                        }
                        //Stream.Close();
                    }
                    //PDFStamper.Close();
                    //PDFReader.Close();
                }
                catch (Exception e)
                { }
            }

        }


        private async Task<string> SaveImageFileWithWaterMark(IFormFile postedFile, string folderPath, string filenameWithExtension, Model.Document c)
        {

            string _SecurityKey = srvCommon.SecurityKey();

            MySqlConnector.MySqlCommand cmd = new MySqlConnector.MySqlCommand("sp_select_company_details_by_param");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("_securityKey", _SecurityKey);
            cmd.Parameters.AddWithValue("_clientId", c.Client_ID);

            DataTable ds = db_connection.ExecuteQueryDataTableProcedure(cmd);

            string watermarktext = (ds.Rows[0]["Company_Name"]).ToString();
            string watermark_Color = (ds.Rows[0]["Watermark_Color"]).ToString();

            //Stream strm = postedFile.InputStream;
            int OneMegaBytes = 1 * 1024 * 1024;//1MB
            var fileSize = postedFile;
            if (chkValidExtension(Path.GetExtension(filenameWithExtension)))
            {
                try
                {
                    using (Stream strm = postedFile.OpenReadStream())
                    {
                        using (var image = System.Drawing.Image.FromStream(strm))
                        {

                            double scaleFactor = 0.6;
                            var newWidth = (int)(image.Width * scaleFactor);
                            var newHeight = (int)(image.Height * scaleFactor);
                            var thumbnailImg = new Bitmap(newWidth, newHeight);
                            var thumbGraph = Graphics.FromImage(thumbnailImg);
                            thumbGraph.CompositingQuality = CompositingQuality.HighQuality;
                            thumbGraph.SmoothingMode = SmoothingMode.HighQuality;
                            thumbGraph.InterpolationMode = InterpolationMode.HighQualityBicubic;
                            System.Drawing.Rectangle imageRectangle = new System.Drawing.Rectangle(0, 0, newWidth, newHeight);
                            thumbGraph.DrawImage(image, imageRectangle);
                            thumbnailImg.Save(folderPath + filenameWithExtension, image.RawFormat);

                            /*Dont Delete: this will make fixed size image.
                            if (fileSize > OneMegaBytes)//If File size is greater than 1MB then only resize
                            { }
                            //SMS:resize Image
                            int newWidth = 240; // New Width of Image in Pixel  
                            int newHeight = 240; // New Height of Image in Pixel  
                            var thumbImg = new Bitmap(newWidth, newHeight);
                            var thumbGraph = Graphics.FromImage(thumbImg);
                            thumbGraph.CompositingQuality = CompositingQuality.HighQuality;
                            thumbGraph.SmoothingMode = SmoothingMode.HighQuality;
                            thumbGraph.InterpolationMode = InterpolationMode.HighQualityBicubic;
                            var imgRectangle = new System.Drawing.Rectangle(0, 0, newWidth, newHeight);
                            thumbGraph.DrawImage(image, imgRectangle);
                            // Save the file  
                            //string targetPath = Server.MapPath(@"~\Images\") + FileUpload1.FileName;
                            thumbImg.Save(folderPath + filenameWithExtension, image.RawFormat);
                            // Print new Size of file (height or Width)  
                            //Label2.Text = thumbImg.Size.ToString();
                            //Show Image  
                            //Image1.ImageUrl = @"~\Images\" + FileUpload1.FileName;
                            */



                            //https://stackoverflow.com/questions/70314147/drawing-diagonal-text-all-over-bitmap
                            Bitmap bmp = new Bitmap(strm);//image
                            System.Drawing.Rectangle rcBmp = new System.Drawing.Rectangle(new Point(0, 0), bmp.Size);
                            Graphics g = Graphics.FromImage(bmp);
                            System.Drawing.Font font = new System.Drawing.Font(FontFamily.GenericSansSerif.Name, 10);
                            SizeF size = g.MeasureString(watermarktext, font);
                            int textwidth = (int)size.Width;
                            int textheight = (int)size.Height;
                            WatermarkDirection direction = WatermarkDirection.BottomLeftToTopRight;
                            int angle = (direction == WatermarkDirection.BottomLeftToTopRight) ? -45 : 45;
                            int y_main = Math.Abs((int)(textwidth * Math.Sin(angle * Math.PI / 180.0)));
                            int x_main = Math.Abs((int)(textwidth * Math.Cos(angle * Math.PI / 180.0)));
                            int y_add = Math.Abs((int)(textheight * Math.Cos(angle * Math.PI / 180.0)));
                            int x_add = Math.Abs((int)(textheight * Math.Sin(angle * Math.PI / 180.0)));
                            int totalX = x_main + x_add;
                            int totalY = y_main + y_add;

                            ///first paramter alpha: Valid alpha values are 0 through 255. Where 255 is the most opaque color and 0 a totally transparent color.
                            //Color color = Color.FromArgb(250, 211, 211, 211);//240,255,240
                            Color color = ColorTranslator.FromHtml(watermark_Color);
                            SolidBrush brush = new SolidBrush(color);

                            // keep going down until we find a line that doesn't cross our bmp
                            System.Drawing.Rectangle rcText;
                            bool intersected = true;
                            int multiplier = (direction == WatermarkDirection.BottomLeftToTopRight) ? 1 : -1;
                            int offset = (direction == WatermarkDirection.BottomLeftToTopRight) ? -totalY : 0;
                            for (int startY = totalY; intersected; startY += (1 * totalY))
                            {
                                intersected = false;
                                int x = (direction == WatermarkDirection.BottomLeftToTopRight) ? 0 : (bmp.Width - totalX);
                                int y = startY;

                                rcText = new System.Drawing.Rectangle(new Point(x, y + offset), new Size(totalX, totalY));
                                if (rcBmp.IntersectsWith(rcText)) { intersected = true; }
                                while (((direction == WatermarkDirection.BottomLeftToTopRight) && (x <= bmp.Width || y >= 0)) ||
                                        ((direction == WatermarkDirection.TopLeftToBottomRight) && (x >= 0 || y >= 0)))
                                {
                                    g.TranslateTransform(x, y);
                                    g.RotateTransform(angle);
                                    g.DrawString(watermarktext, font, brush, 0, 0);//Brushes.Yellow
                                    g.ResetTransform();

                                    x += (totalX * multiplier);
                                    y -= totalY;
                                    rcText = new System.Drawing.Rectangle(new Point(x, y + offset), new Size(totalX, totalY));
                                    if (rcBmp.IntersectsWith(rcText)) { intersected = true; }
                                }
                            }
                            //pb.Image = bmp;
                            bmp.Save(folderPath + filenameWithExtension);
                            g.Dispose();
                            font.Dispose();


                            //https://stackoverflow.com/questions/70314147/drawing-diagonal-text-all-over-bitmap
                            //SMS:Create Watermark at CENTER of image sand save.
                            /*using (System.Drawing.Image img = System.Drawing.Image.FromFile(folderPath + filenameWithExtension))
                            {
                                Bitmap bmp = new Bitmap(img);
                                // choose font for text
                                System.Drawing.Font font = new System.Drawing.Font("Arial", 18, FontStyle.Bold, GraphicsUnit.Pixel);
                                //choose color and transparency
                                Color color = Color.FromArgb(100, 211, 211, 211);
                                //location of the watermark text in the parent image
                                Point point = new Point(bmp.Width / 2, bmp.Height / 2);

                                //Working Code
                                StringFormat sf = new StringFormat();
                                sf.Alignment = StringAlignment.Center;
                                sf.LineAlignment = StringAlignment.Center;

                                SolidBrush brush = new SolidBrush(color);
                                //draw text on image
                                Graphics g = Graphics.FromImage(bmp);
                                //g.DrawString(watermarktext, font, brush, point);
                                g.DrawString(watermarktext, font, brush, point, sf);
                                g.Dispose();
                                img.Dispose();
                                bmp.Save(folderPath + filenameWithExtension);



                            }*/
                        }
                    }
                    return "success";
                }
                catch (Exception e)
                {
                    // postedFile.SaveAs(folderPath + filenameWithExtension);
                    using (var fileStream = new FileStream(Path.Combine(folderPath, filenameWithExtension), FileMode.Create))
                    {
                        await postedFile.CopyToAsync(fileStream);
                    }

                    return "failed";
                }
            }
            else
            {
                return "failed";
            }
        }
        #region Yoti

        public YotiVerificationSummary GetVerificationSummary(string sessionId, int API_ID, int Client_ID)
        {
            var session = GetSession(sessionId, API_ID, Client_ID);
            var idDocument = session.Resources?.IdDocuments?.FirstOrDefault();

            var summary = new YotiVerificationSummary
            {
                SessionId = session.SessionId,
                UserTrackingId = session.UserTrackingId,
                CreatedAt = session.BiometricConsentTimestamp ?? DateTime.UtcNow,
                OverallStatus = session.State,
                Document = MapDocumentCheck(sessionId, session.Checks, idDocument),
                Liveness = MapLivenessCheck(sessionId, session.Checks, session),
                FaceMatch = MapFaceMatchCheck(sessionId, session.Checks, session),
                Watchlist = MapWatchlistCheck(session.Checks),
                TextData = MapTextDataCheck(sessionId, idDocument)
            };

            return summary;
        }


        public GetSessionResult GetSession(string sessionID, int API_ID, int Client_ID)
        {
            string API_Codes = "";
            MySqlConnector.MySqlCommand cmd = new MySqlConnector.MySqlCommand("GetAPIDetails");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("_API_ID", API_ID);//GBG API ID or Shuftipro
            cmd.Parameters.AddWithValue("_Client_ID", Client_ID);
            cmd.Parameters.AddWithValue("_status", 0);// API Status
            DataTable dtApi = db_connection.ExecuteQueryDataTableProcedure(cmd);
            if (dtApi.Rows.Count > 0)
            {
                API_Codes = Convert.ToString(dtApi.Rows[0]["ProfileID"]);
            }

            Newtonsoft.Json.Linq.JObject o = Newtonsoft.Json.Linq.JObject.Parse(API_Codes);

            // string return_success_url = Convert.ToString(o["return_success_url"])+"&ref="+ refNumber;
            string return_success_url = Convert.ToString(o["successurl"]);
            string return_failure_url = Convert.ToString(o["failedurl"]);
            string webhookendpoint = Convert.ToString(o["webhookendpoint"]);
            string PEM_PATH = Convert.ToString(o["PEM_PATH"]);
            string YOTI_CLIENT_SDK_ID = Convert.ToString(o["YOTI_CLIENT_SDK_ID"]);
            string yotiurl = Convert.ToString(o["yotiurl"]);
            string YOTI_IDV_API_URL = Convert.ToString(o["YOTI_IDV_API_URL"]);

            StreamReader privateKeyStream = System.IO.File.OpenText(PEM_PATH);
            var key = CryptoEngine.LoadRsaKey(privateKeyStream);
            //_docScanClient = new DocScanClient(YOTI_CLIENT_SDK_ID, key, new HttpClient(), new Uri("https://api.yoti.com/sandbox/idverify/v1"));

            var docScanClient = new DocScanClient(YOTI_CLIENT_SDK_ID, key, new HttpClient(), new Uri(YOTI_IDV_API_URL));//"https://api.yoti.com/sandbox/idverify/v1"
            try
            {
                var sessionResult = docScanClient.GetSession(sessionID);
                return sessionResult;
            }
            catch (YotiException ex)
            {
                // Handle error from Yoti API
                throw new Exception("Error retrieving session result from Yoti", ex);
            }
        }
        private DocumentCheck MapDocumentCheck(string sessionId, IList<CheckResponse> checks, IdDocumentResourceResponse document)
        {
            if (document == null) return null;

            var documentCheck = new DocumentCheck
            {
                // Document Authenticity Check properties
                Status = checks?.FirstOrDefault(c => c.Type == "ID_DOCUMENT_AUTHENTICITY")?.State,
                Recommendation = checks?.FirstOrDefault(c => c.Type == "ID_DOCUMENT_AUTHENTICITY")?.Report?.Recommendation?.Value,
                Breakdowns = checks?.FirstOrDefault(c => c.Type == "ID_DOCUMENT_AUTHENTICITY")?.Report?.Breakdown?.Select(b => new Breakdown
                {
                    SubCheck = b.SubCheck,
                    Result = b.Result,
                    Details = b.Details?.Select(d => new Detail
                    {
                        Name = d.Name,
                        Value = d.Value
                    }).ToList()
                }).ToList(),

                // Document Details
                DocumentType = document.DocumentType,
                IssuingCountry = document.IssuingCountry,
                DocumentId = document.Id,
                Pages = new List<DocumentPage>(),
                DocumentPhoto = document.DocumentIdPhoto != null ? new Model.Image
                {
                    MediaId = document.DocumentIdPhoto.Media?.Id
                } : null
            };

            // Get base64-encoded images for each page
            if (document.Pages != null)
            {
                foreach (var page in document.Pages)
                {
                    if (page.Media?.Id != null)
                    {
                        var media = GetMediaSafely(sessionId, page.Media.Id);
                        if (media != null)
                        {
                            documentCheck.Pages.Add(new DocumentPage
                            {
                                CaptureMethod = page.CaptureMethod,
                                MediaId = page.Media.Id,
                                Base64Image = media
                            });
                        }
                    }
                }
            }

            // Extract document fields from media content
            if (document.DocumentFields?.Media != null)
            {
                var media = GetMediaSafely(sessionId, document.DocumentFields.Media.Id);
                if (media != null)
                {
                    try
                    {
                        var documentFields = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(media);

                        if (documentFields != null)
                        {
                            documentCheck.FullName = documentFields.GetValueOrDefault("full_name");
                            documentCheck.GivenNames = documentFields.GetValueOrDefault("given_names");
                            documentCheck.FirstName = documentFields.GetValueOrDefault("first_name");
                            documentCheck.MiddleName = documentFields.GetValueOrDefault("middle_name");
                            documentCheck.FamilyName = documentFields.GetValueOrDefault("family_name");
                            documentCheck.NamePrefix = documentFields.GetValueOrDefault("name_prefix");
                            documentCheck.DateOfBirth = documentFields.GetValueOrDefault("date_of_birth");
                            documentCheck.Nationality = documentFields.GetValueOrDefault("nationality");
                            documentCheck.PlaceOfBirth = documentFields.GetValueOrDefault("place_of_birth");
                            documentCheck.CountryOfBirth = documentFields.GetValueOrDefault("country_of_birth");
                            documentCheck.Gender = documentFields.GetValueOrDefault("gender");
                            documentCheck.DocumentNumber = documentFields.GetValueOrDefault("document_number");
                            documentCheck.ExpirationDate = documentFields.GetValueOrDefault("expiration_date");
                            documentCheck.DateOfIssue = documentFields.GetValueOrDefault("date_of_issue");
                            documentCheck.IssuingAuthority = documentFields.GetValueOrDefault("issuing_authority");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error parsing document fields: {ex.Message}");
                    }
                }
            }

            return documentCheck;
        }

        private string GetMediaSafely(string sessionId, string mediaId)
        {
            try
            {
                return GetMedia(sessionId, mediaId);
            }
            catch (DocScanException ex) when (ex.Message.Contains("MEDIA_CONTENT_NOT_FOUND"))
            {
                Console.WriteLine($"Media content not found for session {sessionId} and media ID {mediaId}");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving media for session {sessionId} and media ID {mediaId}: {ex.Message}");
                return null;
            }
        }

        public string GetMedia(string sessionId, string id)
        {
            string API_Codes = "";
            MySqlConnector.MySqlCommand cmd = new MySqlConnector.MySqlCommand("GetAPIDetails");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("_API_ID", 14);//GBG API ID or Shuftipro
            cmd.Parameters.AddWithValue("_Client_ID", 1);
            cmd.Parameters.AddWithValue("_status", 0);// API Status
            DataTable dtApi = db_connection.ExecuteQueryDataTableProcedure(cmd);
            if (dtApi.Rows.Count > 0)
            {
                API_Codes = Convert.ToString(dtApi.Rows[0]["ProfileID"]);
            }

            Newtonsoft.Json.Linq.JObject o = Newtonsoft.Json.Linq.JObject.Parse(API_Codes);

            // string return_success_url = Convert.ToString(o["return_success_url"])+"&ref="+ refNumber;
            string return_success_url = Convert.ToString(o["successurl"]);
            string return_failure_url = Convert.ToString(o["failedurl"]);
            string webhookendpoint = Convert.ToString(o["webhookendpoint"]);
            string PEM_PATH = Convert.ToString(o["PEM_PATH"]);
            string YOTI_CLIENT_SDK_ID = Convert.ToString(o["YOTI_CLIENT_SDK_ID"]);
            string yotiurl = Convert.ToString(o["yotiurl"]);
            string YOTI_IDV_API_URL = Convert.ToString(o["YOTI_IDV_API_URL"]);

            StreamReader privateKeyStream = System.IO.File.OpenText(PEM_PATH);
            var key = CryptoEngine.LoadRsaKey(privateKeyStream);
            //_docScanClient = new DocScanClient(YOTI_CLIENT_SDK_ID, key, new HttpClient(), new Uri("https://api.yoti.com/sandbox/idverify/v1"));

            var docScanClient = new DocScanClient(YOTI_CLIENT_SDK_ID, key, new HttpClient(), new Uri(YOTI_IDV_API_URL));//"https://api.yoti.com/sandbox/idverify/v1"
            try
            {
                var media = docScanClient.GetMediaContent(sessionId, id);
                return media.GetBase64URI();
            }
            catch (YotiException ex)
            {
                // Handle error from Yoti API
                throw new Exception("Error retrieving session result from Yoti", ex);
            }
        }

        private LivenessCheck MapLivenessCheck(string sessionId, IList<CheckResponse> checks, GetSessionResult session)
        {
            var check = checks?.FirstOrDefault(c => c.Type == "LIVENESS");
            if (check == null) return null;

            var livenessCheck = new LivenessCheck
            {
                Status = check.State,
                Recommendation = check.Report?.Recommendation?.Value,
                Breakdowns = check.Report?.Breakdown?.Select(b => new Breakdown
                {
                    SubCheck = b.SubCheck,
                    Result = b.Result,
                    Details = b.Details?.Select(d => new Detail
                    {
                        Name = d.Name,
                        Value = d.Value
                    }).ToList()
                }).ToList(),
                Images = new List<Model.Image>()
            };

            // Get all liveness images from static liveness resources
            if (session.Resources?.StaticLivenessResources != null)
            {
                foreach (var resource in session.Resources.StaticLivenessResources)
                {
                    if (resource.image?.Media?.Id != null)
                    {
                        var media = GetMediaSafely(sessionId, resource.image.Media.Id);
                        if (media != null)
                        {
                            livenessCheck.Images.Add(new Model.Image
                            {
                                MediaId = resource.image.Media.Id,
                                Base64Image = media
                            });
                        }
                    }
                }
            }

            return livenessCheck;
        }

        private FaceMatchCheck MapFaceMatchCheck(string sessionId, IList<CheckResponse> checks, GetSessionResult session)
        {
            var check = checks?.FirstOrDefault(c => c.Type == "ID_DOCUMENT_FACE_MATCH");
            if (check == null) return null;

            var confidenceScore = check.Report?.Breakdown?
                .FirstOrDefault(b => b.SubCheck == "ai_face_match")?
                .Details?
                .FirstOrDefault(d => d.Name == "confidence_score")?
                .Value;

            var faceMatchCheck = new FaceMatchCheck
            {
                Status = check.State,
                Recommendation = check.Report?.Recommendation?.Value,
                ConfidenceScore = confidenceScore != null ? double.Parse(confidenceScore) : 0,
                Breakdowns = check.Report?.Breakdown?.Select(b => new Breakdown
                {
                    SubCheck = b.SubCheck,
                    Result = b.Result,
                    Details = b.Details?.Select(d => new Detail
                    {
                        Name = d.Name,
                        Value = d.Value
                    }).ToList()
                }).ToList(),
                FaceCaptures = new List<Model.Image>()
            };

            // Get all face capture images
            if (session.Resources?.FaceCapture != null)
            {
                foreach (var faceCapture in session.Resources.FaceCapture)
                {
                    if (faceCapture?.Image?.Media?.Id != null)
                    {
                        var media = GetMediaSafely(sessionId, faceCapture.Image.Media.Id);
                        if (media != null)
                        {
                            faceMatchCheck.FaceCaptures.Add(new Model.Image
                            {
                                MediaId = faceCapture.Image.Media.Id,
                                Base64Image = media
                            });
                        }
                    }
                }
            }

            return faceMatchCheck;
        }



        private WatchlistCheck MapWatchlistCheck(IList<CheckResponse> checks)
        {
            var check = checks?.FirstOrDefault(c => c.Type == "WATCHLIST_SCREENING");
            if (check == null) return null;

            return new WatchlistCheck
            {
                Status = check.State,
                Recommendation = check.Report?.Recommendation?.Value,
                Breakdowns = check.Report?.Breakdown?.Select(b => new Breakdown
                {
                    SubCheck = b.SubCheck,
                    Result = b.Result,
                    Details = b.Details?.Select(d => new Detail
                    {
                        Name = d.Name,
                        Value = d.Value
                    }).ToList()
                }).ToList()
            };
        }

        private TextDataCheck MapTextDataCheck(string sessionId, IdDocumentResourceResponse document)
        {
            if (document?.DocumentFields == null) return null;

            return new TextDataCheck
            {
                Status = "DONE", // Document fields are only available when extraction is done
                DocumentFields = new Dictionary<string, string>()
            };
        }

        #endregion Yoti
        private async Task<string> SaveImageFileWithWaterMark(IFormFile postedFile, string folderPath, string filenameWithExtension, Model.Doc c)
        {

            string _SecurityKey = srvCommon.SecurityKey();

            MySqlConnector.MySqlCommand cmd = new MySqlConnector.MySqlCommand("sp_select_company_details_by_param");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("_securityKey", _SecurityKey);
            cmd.Parameters.AddWithValue("_clientId", c.Client_ID);

            DataTable ds = db_connection.ExecuteQueryDataTableProcedure(cmd);

            string watermarktext = (ds.Rows[0]["Company_Name"]).ToString();
            string watermark_Color = (ds.Rows[0]["Watermark_Color"]).ToString();

            //Stream strm = postedFile.InputStream;
            int OneMegaBytes = 1 * 1024 * 1024;//1MB
            var fileSize = postedFile;
            if (chkValidExtension(Path.GetExtension(filenameWithExtension)))
            {
                try
                {
                    using (Stream strm = postedFile.OpenReadStream())
                    {
                        using (var image = System.Drawing.Image.FromStream(strm))
                        {

                            double scaleFactor = 0.6;
                            var newWidth = (int)(image.Width * scaleFactor);
                            var newHeight = (int)(image.Height * scaleFactor);
                            var thumbnailImg = new Bitmap(newWidth, newHeight);
                            var thumbGraph = Graphics.FromImage(thumbnailImg);
                            thumbGraph.CompositingQuality = CompositingQuality.HighQuality;
                            thumbGraph.SmoothingMode = SmoothingMode.HighQuality;
                            thumbGraph.InterpolationMode = InterpolationMode.HighQualityBicubic;
                            System.Drawing.Rectangle imageRectangle = new System.Drawing.Rectangle(0, 0, newWidth, newHeight);
                            thumbGraph.DrawImage(image, imageRectangle);
                            thumbnailImg.Save(folderPath + filenameWithExtension, image.RawFormat);

                            /*Dont Delete: this will make fixed size image.
                            if (fileSize > OneMegaBytes)//If File size is greater than 1MB then only resize
                            { }
                            //SMS:resize Image
                            int newWidth = 240; // New Width of Image in Pixel  
                            int newHeight = 240; // New Height of Image in Pixel  
                            var thumbImg = new Bitmap(newWidth, newHeight);
                            var thumbGraph = Graphics.FromImage(thumbImg);
                            thumbGraph.CompositingQuality = CompositingQuality.HighQuality;
                            thumbGraph.SmoothingMode = SmoothingMode.HighQuality;
                            thumbGraph.InterpolationMode = InterpolationMode.HighQualityBicubic;
                            var imgRectangle = new System.Drawing.Rectangle(0, 0, newWidth, newHeight);
                            thumbGraph.DrawImage(image, imgRectangle);
                            // Save the file  
                            //string targetPath = Server.MapPath(@"~\Images\") + FileUpload1.FileName;
                            thumbImg.Save(folderPath + filenameWithExtension, image.RawFormat);
                            // Print new Size of file (height or Width)  
                            //Label2.Text = thumbImg.Size.ToString();
                            //Show Image  
                            //Image1.ImageUrl = @"~\Images\" + FileUpload1.FileName;
                            */



                            //https://stackoverflow.com/questions/70314147/drawing-diagonal-text-all-over-bitmap
                            Bitmap bmp = new Bitmap(strm);//image
                            System.Drawing.Rectangle rcBmp = new System.Drawing.Rectangle(new Point(0, 0), bmp.Size);
                            Graphics g = Graphics.FromImage(bmp);
                            System.Drawing.Font font = new System.Drawing.Font(FontFamily.GenericSansSerif.Name, 10);
                            SizeF size = g.MeasureString(watermarktext, font);
                            int textwidth = (int)size.Width;
                            int textheight = (int)size.Height;
                            WatermarkDirection direction = WatermarkDirection.BottomLeftToTopRight;
                            int angle = (direction == WatermarkDirection.BottomLeftToTopRight) ? -45 : 45;
                            int y_main = Math.Abs((int)(textwidth * Math.Sin(angle * Math.PI / 180.0)));
                            int x_main = Math.Abs((int)(textwidth * Math.Cos(angle * Math.PI / 180.0)));
                            int y_add = Math.Abs((int)(textheight * Math.Cos(angle * Math.PI / 180.0)));
                            int x_add = Math.Abs((int)(textheight * Math.Sin(angle * Math.PI / 180.0)));
                            int totalX = x_main + x_add;
                            int totalY = y_main + y_add;

                            ///first paramter alpha: Valid alpha values are 0 through 255. Where 255 is the most opaque color and 0 a totally transparent color.
                            //Color color = Color.FromArgb(250, 211, 211, 211);//240,255,240
                            Color color = ColorTranslator.FromHtml(watermark_Color);
                            SolidBrush brush = new SolidBrush(color);

                            // keep going down until we find a line that doesn't cross our bmp
                            System.Drawing.Rectangle rcText;
                            bool intersected = true;
                            int multiplier = (direction == WatermarkDirection.BottomLeftToTopRight) ? 1 : -1;
                            int offset = (direction == WatermarkDirection.BottomLeftToTopRight) ? -totalY : 0;
                            //for (int startY = totalY; intersected; startY += (1 * totalY))
                            //{
                            //    intersected = false;
                            //    int x = (direction == WatermarkDirection.BottomLeftToTopRight) ? 0 : (bmp.Width - totalX);
                            //    int y = startY;

                            //    rcText = new System.Drawing.Rectangle(new Point(x, y + offset), new Size(totalX, totalY));
                            //    if (rcBmp.IntersectsWith(rcText)) { intersected = true; }
                            //    while (((direction == WatermarkDirection.BottomLeftToTopRight) && (x <= bmp.Width || y >= 0)) ||
                            //            ((direction == WatermarkDirection.TopLeftToBottomRight) && (x >= 0 || y >= 0)))
                            //    {
                            //        g.TranslateTransform(x, y);
                            //        g.RotateTransform(angle);
                            //        g.DrawString(watermarktext, font, brush, 0, 0);//Brushes.Yellow
                            //        g.ResetTransform();

                            //        x += (totalX * multiplier);
                            //        y -= totalY;
                            //        rcText = new System.Drawing.Rectangle(new Point(x, y + offset), new Size(totalX, totalY));
                            //        if (rcBmp.IntersectsWith(rcText)) { intersected = true; }
                            //    }
                            //}

                            for (int startY = totalY; intersected; startY += totalY)
                            {
                                intersected = false;
                                int x = (direction == WatermarkDirection.BottomLeftToTopRight) ? 0 : (bmp.Width - totalX);
                                int y = startY;

                                rcText = new System.Drawing.Rectangle(new Point(x, y + offset), new Size(totalX, totalY));
                                if (rcBmp.IntersectsWith(rcText)) { intersected = true; }

                                bool condition = (direction == WatermarkDirection.BottomLeftToTopRight);
                                while ((condition && (x <= bmp.Width || y >= 0)) || (!condition && (x >= 0 || y >= 0)))
                                {
                                    g.TranslateTransform(x, y);
                                    g.RotateTransform(angle);
                                    g.DrawString(watermarktext, font, brush, 0, 0);
                                    g.ResetTransform();

                                    x += totalX * multiplier;
                                    y -= totalY;
                                    rcText = new System.Drawing.Rectangle(new Point(x, y + offset), new Size(totalX, totalY));
                                    if (rcBmp.IntersectsWith(rcText)) { intersected = true; }
                                }
                            }
                            //pb.Image = bmp;
                            bmp.Save(folderPath + filenameWithExtension);
                            g.Dispose();
                            font.Dispose();


                            //https://stackoverflow.com/questions/70314147/drawing-diagonal-text-all-over-bitmap
                            //SMS:Create Watermark at CENTER of image sand save.
                            /*using (System.Drawing.Image img = System.Drawing.Image.FromFile(folderPath + filenameWithExtension))
                            {
                                Bitmap bmp = new Bitmap(img);
                                // choose font for text
                                System.Drawing.Font font = new System.Drawing.Font("Arial", 18, FontStyle.Bold, GraphicsUnit.Pixel);
                                //choose color and transparency
                                Color color = Color.FromArgb(100, 211, 211, 211);
                                //location of the watermark text in the parent image
                                Point point = new Point(bmp.Width / 2, bmp.Height / 2);

                                //Working Code
                                StringFormat sf = new StringFormat();
                                sf.Alignment = StringAlignment.Center;
                                sf.LineAlignment = StringAlignment.Center;

                                SolidBrush brush = new SolidBrush(color);
                                //draw text on image
                                Graphics g = Graphics.FromImage(bmp);
                                //g.DrawString(watermarktext, font, brush, point);
                                g.DrawString(watermarktext, font, brush, point, sf);
                                g.Dispose();
                                img.Dispose();
                                bmp.Save(folderPath + filenameWithExtension);



                            }*/
                        }
                    }
                    return "success";
                }
                catch (Exception e)
                {
                    // postedFile.SaveAs(folderPath + filenameWithExtension);
                    using (var fileStream = new FileStream(Path.Combine(folderPath, filenameWithExtension), FileMode.Create))
                    {
                        await postedFile.CopyToAsync(fileStream);
                    }

                    return "failed";
                }
            }
            else
            {
                return "failed";
            }
        }

        public DataTable QRGetIDScanToken(Model.Document obj, HttpContext context)
        {
            int Customer_ID = Convert.ToInt32(CompanyInfo.Decrypt(obj.Customer_ID, true));

            DataTable dt = new DataTable();
            dt.Columns.Add("Status", typeof(int));
            dt.Columns.Add("RedirectURL", typeof(string));
            string idscan_defaultinput = "";
            string applicant_id = ""; // 65e80e1c62c95a48a68f6a5c
            string externaluser_id = ""; // MX01237486e8f085f7

            var externalUserId = "";
            try
            {
                MySqlCommand cmd = new MySqlCommand("select IDScan_UsedCount from customer_aml_table where date(IDScan_Date)=date(SYSDATE()) and Customer_ID = @Customer_ID and Client_ID=@Client_ID");
                cmd.Parameters.AddWithValue("@Client_ID", obj.Client_ID);
                cmd.Parameters.AddWithValue("@Customer_ID", Customer_ID);// API Status
                string usedc = Convert.ToString(db_connection.ExecuteScalarProcedure(cmd));
                if (usedc != null && usedc != "")
                {
                    if (Convert.ToInt32(usedc) >= 1)
                    {
                        idscan_defaultinput = "FILESYSTEM";//"CAMERA";OR"FILESYSTEM"
                    }
                }
            }
            catch { }
            try
            {
                DataTable dtc = CompanyInfo.get(obj.Client_ID, context);


                //MySqlCommand cmdt1 = new MySqlCommand("sp_select_company_details_by_param");
                //cmdt1.CommandType = CommandType.StoredProcedure;
                //cmdt1.Parameters.AddWithValue("_securityKey", CompanyInfo.SecurityKey());
                //cmdt1.Parameters.AddWithValue("_clientId", obj.Client_ID);
                //DataTable dtc = db_connection.ExecuteQueryDataTableProcedure(cmdt1);

                string appurl = Convert.ToString(dtc.Rows[0]["idscan_url"]);
                //string appurl = Convert.ToString(dtc.Rows[0]["Company_website"]);//"https://currencygenie.co.uk/";//"http://localhost:32720/";//Convert.ToString(dtc.Rows[0]["Company_website"]);// or  ["Company_URL_Admin"] //
                string Status = string.Empty;
                string[] response = { Status.ToString() };
                string Applicat_ID = "";
                string urll = "", UserName = "", Password = "";
                int API_ID = 3;//GBG API ID
                string selfieperm = "1";//selfie Permission


                MySqlCommand cmdupdate1 = new MySqlCommand("Get_Permissions");
                cmdupdate1.CommandType = CommandType.StoredProcedure;
                cmdupdate1.Parameters.AddWithValue("Per_ID", 209);// Check selfie option on gbg id scan
                cmdupdate1.Parameters.AddWithValue("ClientID", obj.Client_ID);
                DataTable pm = db_connection.ExecuteQueryDataTableProcedure(cmdupdate1);
                if (pm.Rows.Count > 0)
                {
                    selfieperm = Convert.ToString(pm.Rows[0]["Status_ForCustomer"]);
                }

                String cust_where = " and cr.Customer_ID = " + Customer_ID + " and cr.Client_ID = " + obj.Client_ID + "";

                MySqlCommand cmdt = new MySqlCommand("GetCustomer_all_details");
                cmdt.CommandType = CommandType.StoredProcedure;
                cmdt.Parameters.AddWithValue("_whereclause", cust_where);
                DataTable cust_info = db_connection.ExecuteQueryDataTableProcedure(cmdt);

                obj.Custmer_Ref = Convert.ToString(cust_info.Rows[0]["WireTransfer_ReferanceNo"]);
                obj.customer_email = Convert.ToString(cust_info.Rows[0]["Email_ID"]);
                obj.customer_mobile = Convert.ToString(cust_info.Rows[0]["Mobile_Number"]);
                obj.city = Convert.ToString(cust_info.Rows[0]["City_Name"]);
                obj.strete = Convert.ToString(cust_info.Rows[0]["Street"]);
                obj.Adderess = Convert.ToString(cust_info.Rows[0]["Address"]);  //Addressline_2
                obj.post_code = Convert.ToString(cust_info.Rows[0]["Post_Code"]);
                //obj.country_code = Convert.ToString(cust_info.Rows[0]["Post_Code"]); //ISO_Code

                //string externalUserId = "MT00123654";
                string email = obj.customer_email;//"john.smith@sumsub.com";
                string phone = obj.customer_mobile;//"+449112081223";
                string country = obj.country_code;//"GBR";
                string placeOfBirth = obj.city;
                string street = obj.strete; // Insert the actual street value here
                string town = obj.Adderess;   // Insert the actual town value here
                string postCode = obj.post_code;// Insert the actual postCode value here
                string countryAddress = obj.country_code;


                string reviewStatus = "";
                string reviewAnswer = "";
                /* // Check here active third party API
                MySqlCommand cmd_select = new MySqlCommand("select API_ID from thirdpartyapi_master where Active_ScanId = 0 and Delete_Status = 0 ");
                DataTable dttActive = db_connection.ExecuteQueryDataTableProcedure(cmd_select);
                if (dttActive.Rows.Count > 0)
                {
                    API_ID = Convert.ToInt32(dttActive.Rows[0]["API_ID"]);
                }
                */
                MySqlCommand cmdp_active = new MySqlCommand("active_thirdparti_api");
                cmdp_active.CommandType = CommandType.StoredProcedure;
                DataTable dtApi_active = db_connection.ExecuteQueryDataTableProcedure(cmdp_active);
                if (dtApi_active.Rows.Count > 0)
                {
                    API_ID = Convert.ToInt32(dtApi_active.Rows[0]["API_ID"]);
                }

                MySqlCommand cmd = new MySqlCommand("GetAPIDetails");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("_API_ID", API_ID);//GBG API ID or Shuftipro
                cmd.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                cmd.Parameters.AddWithValue("_status", 0);// API Status
                DataTable dtApi = db_connection.ExecuteQueryDataTableProcedure(cmd);
                if (dtApi.Rows.Count > 0)
                {
                    urll = Convert.ToString(dtApi.Rows[0]["API_URL"]);
                    UserName = Convert.ToString(dtApi.Rows[0]["UserName"]);
                    Password = Convert.ToString(dtApi.Rows[0]["Password"]);
                }

                if (API_ID == 3)
                {
                    WebResponse response2 = Token(urll, UserName, Password, context);
                    string base64String = string.Empty;
                    using (var reader = new StreamReader(response2.GetResponseStream()))
                    {
                        string ApiStatus = reader.ReadToEnd();//"";//                                        
                        string s13 = string.Empty;
                        var entries = ApiStatus.TrimStart('{').TrimEnd('}').Replace("\"", String.Empty).Split(',');
                        foreach (var entry in entries)
                        {
                            if (entry.Split(':')[0] == "access_token")
                            {
                                s13 = entry.Split(':')[1];



                                dt.Rows.Add(0, "" + appurl + "?id=" + HttpUtility.UrlEncode(CompanyInfo.Encrypt(Convert.ToString(Customer_ID), true))
                                    //dt.Rows.Add(0, "" + appurl + "idscan/index.html?id=" + HttpUtility.UrlEncode(CompanyInfo.Encrypt(Convert.ToString(obj.Customer_ID), true))
                                    //dt.Rows.Add(0, "http://localhost:8307/sangerwal-idscan/index.html?id=" + HttpUtility.UrlEncode(CompanyInfo.Encrypt(Convert.ToString(obj.Customer_ID), true))
                                    + "&key=" + s13 + "&perm=" + selfieperm + "&defaultinput=" + HttpUtility.UrlEncode(idscan_defaultinput));//http://localhost:1698/WSDK_9.1.3/index.html

                                //dt.Rows.Add(0, "" + appurl + "?id=" + HttpUtility.UrlEncode(CompanyInfo.Encrypt(Convert.ToString(Customer_ID), true))
                                //    //dt.Rows.Add(0, "" + appurl + "idscan/index.html?id=" + HttpUtility.UrlEncode(CompanyInfo.Encrypt(Convert.ToString(obj.Customer_ID), true))
                                //    //dt.Rows.Add(0, "http://localhost:8307/sangerwal-idscan/index.html?id=" + HttpUtility.UrlEncode(CompanyInfo.Encrypt(Convert.ToString(obj.Customer_ID), true))
                                //    + "&key=" + s13 + "&defaultinput=" + HttpUtility.UrlEncode(idscan_defaultinput));//http://localhost:1698/WSDK_9.1.3/index.html
                            }
                        }
                    }
                }
                if (API_ID == 9)
                {

                    cmd = new MySqlCommand("Check_applicant_exist");
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("_customer_id", Customer_ID);// API Status
                    dtApi = db_connection.ExecuteQueryDataTableProcedure(cmd);

                    foreach (DataRow row in dtApi.Rows)
                    {
                        Applicat_ID = Convert.ToString(row["Parameter"]);
                        string[] parts = Applicat_ID.Split('=');

                        // Assuming there are always two parts separated by '='
                        if (parts.Length == 2)
                        {
                            Applicat_ID = parts[0]; // 65e80e1c62c95a48a68f6a5c
                            externaluser_id = parts[1]; // MX01237486e8f085f7
                        }
                    }

                    var stamp = ((long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds).ToString();

                    // Generate the externalUserId
                    if (externaluser_id == "")
                    {
                        externalUserId = obj.Custmer_Ref;
                    }
                    else
                    {
                        externalUserId = externaluser_id;
                    }

                    // Construct the JSON body
                    var body = new
                    {
                        externalUserId,
                        email,
                        phone,
                        fixedInfo = new
                        {


                            addresses = new[]
                            {
                new
                {
                    street = obj.strete,
                    town,
                    placeOfBirth,
                    postCode
                }
            }
                        }
                    };

                    var bodyJson = JsonConvert.SerializeObject(body);

                    // Calculate the value to sign
                    var valueToSign = stamp + "POST" + "/resources/applicants?levelName=basic-kyc-level" + bodyJson;

                    // Calculate the signature
                    var secretKey = Password; // Replace with your secret key
                    var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey));
                    var signatureBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(valueToSign));
                    var signature = BitConverter.ToString(signatureBytes).Replace("-", "").ToLower();

                    // Set the request parameters
                    var appToken = UserName;

                    var restClient = new RestClient(urll);
                    var requesttt = new RestRequest("/resources/applicants?levelName=basic-kyc-level", Method.POST);

                    requesttt.AddHeader("Content-Type", "application/json");
                    requesttt.AddHeader("X-App-Token", appToken);
                    requesttt.AddHeader("X-App-Access-Ts", stamp);
                    requesttt.AddHeader("X-App-Access-Sig", signature);
                    requesttt.AddParameter("application/json", bodyJson, ParameterType.RequestBody);

                    var responsed = restClient.Execute(requesttt);
                    dynamic dynJsonn = JsonConvert.DeserializeObject(responsed.Content);

                    // Second part of your code starts here

                    stamp = ((long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds).ToString();
                    string userId = externalUserId;
                    // Calculate the value to sign (excluding the body)
                    valueToSign = stamp + "POST" + $"/resources/accessTokens?userId={userId}&levelName=basic-kyc-level";

                    // Calculate the signature
                    hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey));
                    signatureBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(valueToSign));
                    signature = BitConverter.ToString(signatureBytes).Replace("-", "").ToLower();

                    // Set your dynamic userId here

                    var client = new RestClient(urll);
                    var request = new RestRequest($"/resources/accessTokens?userId={userId}&levelName=basic-kyc-level", Method.POST);
                    request.AddHeader("Content-Type", "application/json");
                    request.AddHeader("X-App-Token", appToken);
                    request.AddHeader("X-App-Access-Ts", stamp);
                    request.AddHeader("X-App-Access-Sig", signature);

                    var responseT = client.Execute(request);
                    JObject jsonResponse = JObject.Parse(responseT.Content);

                    // Extract values
                    string Token = jsonResponse["token"].ToString();
                    string UserId = jsonResponse["userId"].ToString();
                    //Console.WriteLine(responseT.Content);



                    if (dtApi.Rows.Count > 0)
                    {

                        stamp = ((long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds).ToString();
                        userId = externaluser_id;
                        applicant_id = Applicat_ID;
                        // Calculate the value to sign (excluding the body)
                        valueToSign = stamp + "GET" + $"/resources/applicants/{applicant_id}/one";


                        appToken = UserName;
                        // Replace placeholders with actual values
                        valueToSign = valueToSign.Replace("{{sumsub_root_url}}", "")
                                                 .Replace("{{applicantId}}", "")
                                                 .Replace("{{levelName}}", "basic-kyc-level")
                                                 .Replace("{{userId}}", userId);

                        // Calculate the signature
                        secretKey = Password; // Replace with your secret key
                        hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey));
                        signatureBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(valueToSign));
                        signature = BitConverter.ToString(signatureBytes).Replace("-", "").ToLower();

                        // Set your dynamic userId here

                        var client1 = new RestClient(urll);
                        var request1 = new RestRequest($"/resources/applicants/{applicant_id}/one", Method.GET);
                        request1.AddHeader("Content-Type", "application/json");
                        request1.AddHeader("X-App-Token", appToken);
                        request1.AddHeader("X-App-Access-Ts", stamp);
                        request1.AddHeader("X-App-Access-Sig", signature);
                        responseT = client1.Execute(request1);
                        jsonResponse = JObject.Parse(responseT.Content);


                        // CompanyInfo.InsertActivityLogDetails("Get The Sumsub Docs Details and applicantID 6 for Id scan " + responseT.Content + applicant_id + ". ", Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(Customer_ID), "GetJourney", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "ID Upload");

                        //activity_log = "Get The Sumsub Docs Details and applicantID 6 for Id scan " + responseT.Content + applicant_id + "";

                        // Accessing values from the response

                        try
                        {
                            string id = (string)jsonResponse["id"];
                            DateTime createdAt = (DateTime)jsonResponse["createdAt"];
                            string key = (string)jsonResponse["key"];
                            string clientId = (string)jsonResponse["clientId"];

                            externalUserId = (string)jsonResponse["externalUserId"];

                            JObject review = (JObject)jsonResponse["review"];

                            reviewStatus = (string)review["reviewStatus"];

                            // Accessing nested objects
                            JObject reviewResult = (JObject)review["reviewResult"];
                            reviewAnswer = (string)reviewResult["reviewAnswer"];

                        }
                        catch (Exception ex)
                        {

                        }
                        if (reviewAnswer == "GREEN" && reviewStatus == "completed")
                        {

                            dt.Rows.Add(2, "" + appurl + "?id=" + HttpUtility.UrlEncode(CompanyInfo.Encrypt(Convert.ToString(Customer_ID), true))

                               + "&key=" + Token + "&defaultinput=" + HttpUtility.UrlEncode(Applicat_ID));
                        }
                        else
                        {

                            dt.Rows.Add(1, "" + appurl + "?id=" + HttpUtility.UrlEncode(CompanyInfo.Encrypt(Convert.ToString(Customer_ID), true))

                                 + "&key=" + Token + "&defaultinput=" + HttpUtility.UrlEncode(Applicat_ID));

                        }
                    }
                    else
                    {

                        dt.Rows.Add(1, "" + appurl + "?id=" + HttpUtility.UrlEncode(CompanyInfo.Encrypt(Convert.ToString(Customer_ID), true))

                              + "&key=" + Token + "&defaultinput=" + HttpUtility.UrlEncode(Applicat_ID));

                    }
                }
                if (API_ID == 5)
                {
                    string url_scan = "";
                    if (dtApi.Rows.Count > 0)
                    {
                        url_scan = Convert.ToString(dtApi.Rows[0]["ProfileID"]);
                    }
                    string isocode = "GB";
                    /*
                    MySqlCommand cmd_iso = new MySqlCommand("SELECT cm.ISO_Code as isocode FROM  country_master as cm , customer_registration as c where c.Country_ID = cm.Country_ID and c.Customer_ID = " + obj.Customer_ID + " ");
                    DataTable dttiso = db_connection.ExecuteQueryDataTableProcedure(cmd_iso);
                    if (dttiso.Rows.Count > 0)
                    {
                        isocode = Convert.ToString(dttiso.Rows[0]["isocode"]);
                    }
                    cmd_iso.Dispose();
                    */
                    MySqlCommand cmdp = new MySqlCommand("getiso_data");
                    cmdp.CommandType = CommandType.StoredProcedure;
                    cmdp.Parameters.AddWithValue("_Cust_ID", Customer_ID);
                    DataTable dtiso = db_connection.ExecuteQueryDataTableProcedure(cmdp);
                    if (dtApi.Rows.Count > 0)
                    {
                        isocode = Convert.ToString(dtiso.Rows[0]["isocode"]);
                    }

                    if (idscan_defaultinput == "FILESYSTEM")
                    {
                        idscan_defaultinput = "File";
                    }

                    MySqlCommand cmdcustdata = new MySqlCommand("Customer_RegDetails");
                    cmdcustdata.CommandType = CommandType.StoredProcedure;
                    cmdcustdata.Parameters.AddWithValue("_Customer_ID", Customer_ID);
                    cmdcustdata.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                    DataTable dt_custdata = db_connection.ExecuteQueryDataTableProcedure(cmdcustdata);
                    string cfname = "", clname = "", cbd = "";
                    if (dt_custdata.Rows.Count > 0)
                    {
                        cfname = Convert.ToString(dt_custdata.Rows[0]["First_Name"]);
                        clname = Convert.ToString(dt_custdata.Rows[0]["Last_Name"]);
                        cbd = Convert.ToString(dt_custdata.Rows[0]["DateOf_Birth"]);

                        if (cbd != "" || cbd != null)
                        {
                            try
                            {
                                cbd = cbd.Split()[0].Trim();
                                DateTime dtdd = DateTime.Parse(cbd);
                                cbd = dtdd.ToString("yyyy-MM-dd");
                            }
                            catch (Exception ex)
                            {
                            }
                        }
                    }
                    string strdata = "&cfname=" + cfname + "&cmname=" + clname + "&cbd=" + cbd;

                    dt.Rows.Add(0, url_scan + "?add=" + 0 + "&secretkey=" + UserName + "&clientkey=" + Password + "&iso=" + isocode + "&defaultinput=" + HttpUtility.UrlEncode(idscan_defaultinput) + strdata);
                }
                if (API_ID == 7)
                {
                    string url_scan = "", isocode = "GB";
                    dt.Columns.Add("veriffsessionid", typeof(string));
                    MySqlCommand cmdp = new MySqlCommand("getiso_data");
                    cmdp.CommandType = CommandType.StoredProcedure;
                    cmdp.Parameters.AddWithValue("_Cust_ID", Customer_ID);
                    DataTable dtiso = db_connection.ExecuteQueryDataTableProcedure(cmdp);
                    if (dtiso.Rows.Count > 0)
                    {
                        isocode = Convert.ToString(dtiso.Rows[0]["isocode"]);
                    }
                    isocode = "\"" + isocode + "\"";
                    MySqlCommand cmdcustdata = new MySqlCommand("Customer_RegDetails");
                    cmdcustdata.CommandType = CommandType.StoredProcedure;
                    cmdcustdata.Parameters.AddWithValue("_Customer_ID", Customer_ID);
                    cmdcustdata.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                    DataTable dt_custdata = db_connection.ExecuteQueryDataTableProcedure(cmdcustdata);
                    string cfname = "", clname = "", cbd = "";
                    if (dt_custdata.Rows.Count > 0)
                    {
                        cfname = "\"" + Convert.ToString(dt_custdata.Rows[0]["First_Name"]) + "\"";
                        clname = "\"" + Convert.ToString(dt_custdata.Rows[0]["Last_Name"]) + "\"";
                        cbd = "\"" + Convert.ToString(dt_custdata.Rows[0]["DateOf_Birth"]) + "\"";

                        if (cbd != "" || cbd != null)
                        {
                            try
                            {
                                cbd = cbd.Split()[0].Trim();
                                DateTime dtdd = DateTime.Parse(cbd);
                                cbd = dtdd.ToString("yyyy-MM-dd");
                            }
                            catch (Exception ex)
                            {
                            }
                        }
                    }
                    var requesturl = context.Request;

                    string baseUrl = $"{requesturl.Scheme}://{requesturl.Host}{requesturl.PathBase}";
                    string callback_url = $"{baseUrl}/id-scan-confirmation.html";

                    //string baseUrl = System.Web.HttpContext.Current.Request.Url.Scheme + "://"
                    //    + System.Web.HttpContext.Current.Request.Url.Authority
                    //    + System.Web.HttpContext.Current.Request.ApplicationPath.TrimEnd('/') + '/';
                    //var callback_url = "\"" + baseUrl + "id-scan-confirmation.html" + "\"";
                    //callback_url = "\"" + "https://currencyexchangesoftware.eu/csremit-customer/id-scan-confirmation.html" + "\"";

                    var client = new RestClient(urll);
                    client.Timeout = -1;
                    var request = new RestRequest(Method.POST);
                    request.AddHeader("Content-Type", "application/json");
                    request.AddHeader("X-AUTH-CLIENT", UserName);

                    var body = @"{
                        " + "\n" +
                        @"    ""verification"":
                        " + "\n" +
                        @"{
                        " + "\n" +
                        @"""callback"":" + callback_url + "," +
                        @"""person"":
                        " + "\n" +
                        @"{
                        " + "\n" +
                        @"    ""firstName"":" + cfname + "," +
                        @"""lastName"":" + clname + "" +
                        @"},
                        " + "\n" +
                        @"""document"":
                        " + "\n" +
                        @"{" +
                        @"""country"":" + isocode + "" +
                        @"},
                        " + "\n" +
                        @"""vendorData"":""""
                        " + "\n" +
                        @"}
                        " + "\n" +
                        @"}";

                    request.AddParameter("application/json", body, ParameterType.RequestBody);
                    IRestResponse responseData = client.Execute(request);
                    Console.WriteLine(responseData.Content);

                    dynamic dynJson = Newtonsoft.Json.JsonConvert.DeserializeObject(responseData.Content);
                    string responseCode = dynJson.status;

                    if (responseCode == "success")
                    {
                        string sessionId = dynJson.verification.id;
                        url_scan = dynJson.verification.url;
                        dt.Rows.Add(0, url_scan, sessionId);
                    }
                }
                if (dt.Rows.Count <= 0)
                {
                    dt.Rows.Add(1, "");
                }
                return dt;
            }
            catch (Exception ex)
            {
                if (dt.Rows.Count <= 0)
                {
                    dt.Rows.Add(2, ex.ToString());
                }
                return dt;
            }
        }

        public DataTable ChkCustIDParams(Model.Document obj, HttpContext context)
        {
            int perm = 1, IDScanLimit = 0, AllowIDScan = 1, camperm88 = 1, perm153placeissue = 1;
            DataTable dt = new DataTable();
            string Custname = "";
            try
            {
                int Customer_ID = Convert.ToInt32(CompanyInfo.Decrypt(obj.Customer_ID, true));


                dt.Columns.Add("CustName", typeof(string));
                dt.Columns.Add("CustRef", typeof(string));

                dt.Columns.Add("IDScanPerm", typeof(int));
                dt.Columns.Add("IDScanStatus", typeof(int));
                dt.Columns.Add("IDScanLimit", typeof(int));//0 valid and 1 exceeded
                dt.Columns.Add("MultipleIDScan", typeof(int));
                dt.Columns.Add("cameraperm88", typeof(int));
                dt.Columns.Add("perm153placeissue", typeof(int));

                dt.Columns.Add("checkparam", typeof(string));

                string urll = "", UserName = "", Password = "", Encrypted_time1 = "";
                int API_ID = 0;
                string externaluser_id = "";
                string applicant_id = "";
                string Applicat_ID = "";
                var externalUserId = "";
                string reviewAnswer = "";
                string reviewStatus = "";
                string Client_ID_regex = validation.validate(Convert.ToString(obj.Client_ID), 0, 1, 1, 1, 1, 1, 1, 1, 1);
                string Customer_ID_regex = validation.validate(Convert.ToString(Customer_ID), 0, 1, 1, 1, 1, 1, 1, 1, 1);
                if (Client_ID_regex != "false" && Customer_ID_regex != "false")
                {

                    String cust_where = " and cr.Customer_ID = " + Customer_ID + " and cr.Client_ID = " + obj.Client_ID + "";
                    MySqlCommand cmdt = new MySqlCommand("GetCustomer_all_details");
                    cmdt.CommandType = CommandType.StoredProcedure;
                    cmdt.Parameters.AddWithValue("_whereclause", cust_where);
                    DataTable cust_info = db_connection.ExecuteQueryDataTableProcedure(cmdt);

                    obj.Custmer_Ref = Convert.ToString(cust_info.Rows[0]["WireTransfer_ReferanceNo"]);
                    Custname = Convert.ToString(cust_info.Rows[0]["Full_Name"]);

                    DataTable li1 = (DataTable)CompanyInfo.getEmailPermission(obj.Client_ID, 54);//Check GBG ID Scan permission

                    if (li1.Rows.Count > 0)
                    {
                        perm = Convert.ToInt32(li1.Rows[0]["Status_ForCustomer"]);
                    }

                    obj.Record_Insert_DateTime = CompanyInfo.gettime(obj.Client_ID, Customer_ID.ToString(), obj.Country_ID, context);
                    MySqlCommand cmdc = new MySqlCommand("UpdateIDScanCount");
                    cmdc.CommandType = CommandType.StoredProcedure;
                    cmdc.Parameters.AddWithValue("_Customer_ID", Customer_ID);//Customer ID
                    cmdc.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                    cmdc.Parameters.AddWithValue("_Queryflag", 2);//Get ID Scan Result
                    cmdc.Parameters.AddWithValue("_nowdate", obj.Record_Insert_DateTime);
                    string result = Convert.ToString(db_connection.ExecuteScalarProcedure(cmdc));
                    if (result != null && result != "")
                    {
                        if (result == "2")
                        {
                            perm = 1; IDScanLimit = 1;
                        }
                    }
                    int chkexpiry = 0, idupload = 0;



                    MySqlCommand cmdp_active = new MySqlCommand("active_thirdparti_api");
                    cmdp_active.CommandType = CommandType.StoredProcedure;
                    DataTable dtApi_active = db_connection.ExecuteQueryDataTableProcedure(cmdp_active);
                    if (dtApi_active.Rows.Count > 0)
                    {
                        API_ID = Convert.ToInt32(dtApi_active.Rows[0]["API_ID"]);
                    }

                    MySqlCommand cmd = new MySqlCommand("GetAPIDetails");
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("_API_ID", API_ID);//GBG API ID or Shuftipro
                    cmd.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                    cmd.Parameters.AddWithValue("_status", 0);// API Status
                    DataTable dtApi = db_connection.ExecuteQueryDataTableProcedure(cmd);
                    if (dtApi.Rows.Count > 0)
                    {
                        urll = Convert.ToString(dtApi.Rows[0]["API_URL"]);
                        UserName = Convert.ToString(dtApi.Rows[0]["UserName"]);
                        Password = Convert.ToString(dtApi.Rows[0]["Password"]);
                    }

                    if (API_ID == 9)
                    {

                        cmd = new MySqlCommand("Check_applicant_exist");
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("_customer_id", Customer_ID);// API Status
                        dtApi = db_connection.ExecuteQueryDataTableProcedure(cmd);

                        foreach (DataRow row in dtApi.Rows)
                        {
                            Applicat_ID = Convert.ToString(row["Parameter"]);
                            string[] parts = Applicat_ID.Split('=');

                            // Assuming there are always two parts separated by '='
                            if (parts.Length == 2)
                            {
                                Applicat_ID = parts[0]; // 65e80e1c62c95a48a68f6a5c
                                externaluser_id = parts[1]; // MX01237486e8f085f7
                            }
                        }

                        if (dtApi.Rows.Count > 0)
                        {

                            var stamp = ((long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds).ToString();
                            var userId = externaluser_id;
                            applicant_id = Applicat_ID;
                            // Calculate the value to sign (excluding the body)
                            var valueToSign = stamp + "GET" + $"/resources/applicants/{applicant_id}/one";


                            var appToken = UserName;
                            // Replace placeholders with actual values
                            valueToSign = valueToSign.Replace("{{sumsub_root_url}}", "")
                                                     .Replace("{{applicantId}}", "")
                                                     .Replace("{{levelName}}", "basic-kyc-level")
                                                     .Replace("{{userId}}", userId);

                            // Calculate the signature
                            var secretKey = Password; // Replace with your secret key
                            var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey));
                            var signatureBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(valueToSign));
                            var signature = BitConverter.ToString(signatureBytes).Replace("-", "").ToLower();

                            // Set your dynamic userId here

                            var client1 = new RestClient(urll);
                            var request1 = new RestRequest($"/resources/applicants/{applicant_id}/one", Method.GET);
                            request1.AddHeader("Content-Type", "application/json");
                            request1.AddHeader("X-App-Token", appToken);
                            request1.AddHeader("X-App-Access-Ts", stamp);
                            request1.AddHeader("X-App-Access-Sig", signature);
                            var responseT = client1.Execute(request1);
                            JObject jsonResponse = JObject.Parse(responseT.Content);


                            // CompanyInfo.InsertActivityLogDetails("Get The Sumsub Docs Details and applicantID 6 for Id scan " + responseT.Content + applicant_id + ". ", Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(Customer_ID), "GetJourney", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "ID Upload");

                            //activity_log = "Get The Sumsub Docs Details and applicantID 6 for Id scan " + responseT.Content + applicant_id + "";

                            // Accessing values from the response
                            try
                            {
                                string id = (string)jsonResponse["id"];
                                DateTime createdAt = (DateTime)jsonResponse["createdAt"];
                                string key = (string)jsonResponse["key"];
                                string clientId = (string)jsonResponse["clientId"];

                                externalUserId = (string)jsonResponse["externalUserId"];






                                JObject review = (JObject)jsonResponse["review"];

                                reviewStatus = (string)review["reviewStatus"];

                                // Accessing nested objects
                                JObject reviewResult = (JObject)review["reviewResult"];
                                reviewAnswer = (string)reviewResult["reviewAnswer"];
                            }
                            catch (Exception ex)
                            {

                            }


                            if (reviewAnswer == "GREEN" && reviewStatus == "completed")
                            {

                                //dt.Rows.Add(2, "" + appurl + "?id=" + HttpUtility.UrlEncode(CompanyInfo.Encrypt(Convert.ToString(Customer_ID), true))

                                //   + "&key=" + Token + "&defaultinput=" + HttpUtility.UrlEncode(Applicat_ID));
                                perm = 1; IDScanLimit = 1;

                            }
                            //else
                            //{

                            //    dt.Rows.Add(1, "" + appurl + "?id=" + HttpUtility.UrlEncode(CompanyInfo.Encrypt(Convert.ToString(Customer_ID), true))

                            //         + "&key=" + Token + "&defaultinput=" + HttpUtility.UrlEncode(Applicat_ID));

                            //}


                        }


                    }

                    if (perm == 0)
                    {
                        li1 = null;
                        li1 = (DataTable)CompanyInfo.getEmailPermission(obj.Client_ID, 77);// Allow multiple times ID scan even if ID is valid/invalid
                        if (li1.Rows.Count > 0)
                        {
                            AllowIDScan = Convert.ToInt32(li1.Rows[0]["Status_ForCustomer"]);
                        }
                        //For Primary
                        MySqlCommand _cmd = new MySqlCommand("CheckIDExpiry");
                        _cmd.CommandType = CommandType.StoredProcedure;
                        _cmd.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                        _cmd.Parameters.AddWithValue("_IDType_ID", 1);
                        _cmd.Parameters.AddWithValue("_Customer_ID", Customer_ID);
                        DataTable table = db_connection.ExecuteQueryDataTableProcedure(_cmd);

                        DateTime dt1 = DateTime.Now.Date;
                        DateTime dt3 = DateTime.Now.Date.AddDays(60);
                        if (table.Rows.Count > 0)// Check 1
                        {
                            for (int i = 0; i < table.Rows.Count; i++)
                            {
                                string senPExpDate = Convert.ToString(table.Rows[i]["SenderID_ExpiryDate"]);
                                if (senPExpDate != "" && senPExpDate != null && senPExpDate != "VGrYRT2Em7s=")
                                {
                                    DateTime dt2 = DateTime.ParseExact(Convert.ToDateTime(senPExpDate).ToString("dd/MM/yyyy"), "dd/MM/yyyy", null);
                                    if (dt1 > dt2)
                                    {
                                        //"Identification document is expired and need to be uploaded to proceed this transfer. Do you want to Upload?";
                                        chkexpiry = 1;
                                    }
                                }
                            }
                        }
                        else
                        {
                            idupload = 1;
                        }
                        if (AllowIDScan == 0)
                        {
                            chkexpiry = 0;
                        }
                    }

                    li1 = null;
                    li1 = (DataTable)CompanyInfo.getEmailPermission(obj.Client_ID, 88);// Show ID Scan Types - File System or Camera
                    if (li1.Rows.Count > 0)
                    {
                        camperm88 = Convert.ToInt32(li1.Rows[0]["Status_ForCustomer"]);
                    }

                    li1 = null;
                    li1 = (DataTable)CompanyInfo.getEmailPermission(obj.Client_ID, 153);// Perm Country Wise Place of Issue
                    if (li1.Rows.Count > 0)
                    {
                        perm153placeissue = Convert.ToInt32(li1.Rows[0]["Status_ForCustomer"]);
                    }

                    if (obj.checkparam == "" || obj.checkparam == null || obj.checkparam == "0")
                    {
                        DateTime date = DateTime.Now;
                        DateTime date3 = date.AddMinutes(10);
                        int bool_check1 = 1;
                        bool b2 = Convert.ToBoolean(bool_check1);
                        Encrypted_time1 = Convert.ToString(CompanyInfo.Encrypt(Convert.ToString(date3), b2));
                    }
                    else
                    {
                        Encrypted_time1 = obj.checkparam;
                    }

                    if (perm == 0 && idupload == 1 || perm == 0 && chkexpiry == 1)
                    {
                        dt.Rows.Add(Custname, obj.Custmer_Ref, perm, 0, IDScanLimit, AllowIDScan, camperm88, perm153placeissue, Encrypted_time1);
                    }
                    else
                    {
                        dt.Rows.Add(Custname, obj.Custmer_Ref, perm, 1, IDScanLimit, AllowIDScan, camperm88, perm153placeissue, Encrypted_time1);
                    }
                    if (IDScanLimit == 1)
                    {
                        //obj.Record_Insert_DateTime = DateTime.Now.ToString();
                        //string notification_icon = "sanction-list-match-found.jpg";
                        //string notification_message = "<span class='cls-admin'>Daily ID scan limit<strong class='cls-cancel'> exceeded.</strong></span><span class='cls-customer'></span>";
                        //CompanyInfo.save_notification_compliance(notification_message, notification_icon, Convert.ToInt32(obj.Customer_ID), Convert.ToDateTime(obj.Record_Insert_DateTime), Convert.ToInt32(obj.Client_ID), 1, Convert.ToInt32(obj.User_ID), Convert.ToInt32(obj.Branch_ID), 0, 1, 1, 0);
                    }
                }
                else
                {
                    string msg = "Validation Error Client_ID_regex-" + Client_ID_regex + "Customer_ID_regex- " + Customer_ID_regex;
                    int stattus = (int)CompanyInfo.InsertActivityLogDetailsSecurity(msg, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), 1, "srvDocument", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "ChkCustIDParams", 0, context);
                }
            }
            catch (Exception ex)
            {
                string stattusss = (string)CompanyInfo.InsertErrorLogDetails(ex.Message.Replace("\'", "\\'"), 0, "ChkCustIDParams", obj.Branch_ID, obj.Client_ID);
                dt.Rows.Add(Custname, obj.Custmer_Ref, perm, 1, IDScanLimit, AllowIDScan, camperm88, perm153placeissue, "0");
            }
            return dt;
        }

        //public async Task<string> SaveImageFileWithWaterMark(IFormFile postedFile, string folderPath, string filenameWithExtension, Model.Document c)
        //{
        //    string _SecurityKey = srvCommon.SecurityKey();
        //    string watermark_Color;

        //    using (var cmd = new MySqlConnector.MySqlCommand("sp_select_company_details_by_param"))
        //    {
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.Parameters.AddWithValue("_securityKey", _SecurityKey);
        //        cmd.Parameters.AddWithValue("_clientId", c.Client_ID);

        //        DataTable ds = db_connection.ExecuteQueryDataTableProcedure(cmd);
        //        watermark_Color = (ds.Rows[0]["Watermark_Color"]).ToString();
        //    }

        //    if (chkValidExtension(Path.GetExtension(filenameWithExtension)))
        //    {
        //        try
        //        {
        //            using (Stream strm = postedFile.OpenReadStream())
        //            using (var image = System.Drawing.Image.FromStream(strm))
        //            {
        //                double scaleFactor = 0.6;
        //                var newWidth = (int)(image.Width * scaleFactor);
        //                var newHeight = (int)(image.Height * scaleFactor);
        //                var thumbnailImg = new Bitmap(newWidth, newHeight);
        //                var thumbGraph = Graphics.FromImage(thumbnailImg);
        //                thumbGraph.CompositingQuality = CompositingQuality.HighQuality;
        //                thumbGraph.SmoothingMode = SmoothingMode.HighQuality;
        //                thumbGraph.InterpolationMode = InterpolationMode.HighQualityBicubic;
        //                System.Drawing.Rectangle imageRectangle = new System.Drawing.Rectangle(0, 0, newWidth, newHeight);
        //                thumbGraph.DrawImage(image, imageRectangle);
        //                thumbnailImg.Save(Path.Combine(folderPath, filenameWithExtension), ImageFormat.Jpeg);

        //                Bitmap bmp = new Bitmap(image);
        //                Graphics g = Graphics.FromImage(bmp);
        //                System.Drawing.Font font = new System.Drawing.Font(FontFamily.GenericSansSerif.Name, 10);
        //                Color color = ColorTranslator.FromHtml(watermark_Color);
        //                SolidBrush brush = new SolidBrush(color);

        //                Watermark placement logic here...

        //                bmp.Save(Path.Combine(folderPath, filenameWithExtension), ImageFormat.Jpeg);
        //                g.Dispose();
        //                font.Dispose();
        //            }
        //            return "success";
        //        }
        //        catch (Exception e)
        //        {
        //            using (var fileStream = new FileStream(Path.Combine(folderPath, filenameWithExtension), FileMode.Create))
        //            {
        //                await postedFile.CopyToAsync(fileStream);
        //            }
        //            return "failed";
        //        }
        //    }
        //    else
        //    {
        //        return "failed";
        //    }
        //}
        // }



        public string[] CheckCreditSafeByName(Model.Document i, DataTable dc, string CallFrom, HttpContext context)
        {
            string mail_send = string.Empty;
            int Status = 1;
            string Bandtext = "";
            int perm_check_all_param_for_addre = 1;
            int per_address_mismatch = 1;
            string Remark = "";
            string EmailUrl = "";

            try
            {
                MySqlCommand cmdupdate1 = new MySqlCommand("Get_Permissions");
                cmdupdate1.CommandType = CommandType.StoredProcedure;
                cmdupdate1.Parameters.AddWithValue("Per_ID", 263);
                cmdupdate1.Parameters.AddWithValue("ClientID", i.Client_ID);
                string permissioncheckby_name = string.Empty;
                DataTable dt1 = db_connection.ExecuteQueryDataTableProcedure(cmdupdate1);

                if (dt1.Rows.Count > 0)
                {
                    permissioncheckby_name = Convert.ToString(dt1.Rows[0]["Status_ForCustomer"]);
                }

                if (permissioncheckby_name == "0")
                {




                    int API_ID = 8;

                    MySqlConnector.MySqlCommand cmd = new MySqlConnector.MySqlCommand("GetAPIDetails_byCountry");//("GetAPIDetails");
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("_API_ID", API_ID);
                    cmd.Parameters.AddWithValue("_Client_ID", i.Client_ID);
                    cmd.Parameters.AddWithValue("_status", 0);
                    cmd.Parameters.AddWithValue("_Customer_ID", i.Customer_ID);
                    DataTable dtApi = db_connection.ExecuteQueryDataTableProcedure(cmd);
                    if (dtApi.Rows.Count > 0)
                    {
                        string StateDistrict = "";


                        MySqlCommand cmd7 = new MySqlCommand("GetCustDetailsByID");
                        cmd7.CommandType = CommandType.StoredProcedure;
                        cmd7.Parameters.AddWithValue("cust_ID", i.Customer_ID);
                        DataTable dc1 = db_connection.ExecuteQueryDataTableProcedure(cmd7);
                        string First_Name = Convert.ToString(dc1.Rows[0]["First_Name"]).Replace("&", "AND");
                        string Middle_Name = Convert.ToString(dc1.Rows[0]["Middle_Name"]).Replace("&", "AND");
                        string Last_Name = Convert.ToString(dc1.Rows[0]["Last_Name"]);



                        var _url = Convert.ToString(dtApi.Rows[0]["API_URL"]);
                        var UserName = Convert.ToString(dtApi.Rows[0]["UserName"]);
                        var Password = Convert.ToString(dtApi.Rows[0]["Password"]);
                        var ProfileID = Convert.ToString(dtApi.Rows[0]["ProfileID"]);
                        var _action = "http://www.id3global.com/ID3gWS/2013/04/IGlobalAuthenticate/AuthenticateSP";
                        XmlDocument soapEnvelopeXml = new XmlDocument();
                        var iddoc = "";

                        string passdoc = "", passid = "", endid = "";


                        string province = "";

                        string passexpiry = "";

                        string strmiddlename = "";

                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
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
                                   
                                 </ns:PersonalDetails>
                               </ns:Personal>
                       
                                                     
                            </ns:InputData>
                            </ns:AuthenticateSP>                            
                      </soapenv:Body>
                 </soapenv:Envelope>
                ");
                        string SendTransferReq = soapEnvelopeXml.InnerXml;
                        try
                        {
                            MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("SaveAPIRequestResponce");
                            _cmd.CommandType = CommandType.StoredProcedure;
                            _cmd.Parameters.AddWithValue("_API_ID", API_ID);
                            _cmd.Parameters.AddWithValue("_Client_ID", i.Client_ID);
                            _cmd.Parameters.AddWithValue("_Customer_ID", i.Customer_ID);
                            _cmd.Parameters.AddWithValue("_status", 0);
                            _cmd.Parameters.AddWithValue("_Function_name", CallFrom);
                            _cmd.Parameters.AddWithValue("_Remark", 0);
                            _cmd.Parameters.AddWithValue("_comments", SendTransferReq.Replace("'", "''"));
                            _cmd.Parameters.AddWithValue("_Record_Insert_DateTime", i.Record_Insert_DateTime);
                            _cmd.Parameters.AddWithValue("_Branch_ID", i.Branch_ID);
                            int msg1 = db_connection.ExecuteNonQueryProcedure(_cmd);
                        }
                        catch (Exception ex)
                        {
                            string error = ex.ToString().Replace("\'", "\\'");

                            MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("SaveException");
                            _cmd.CommandType = CommandType.StoredProcedure;
                            _cmd.Parameters.AddWithValue("_Record_Insert_DateTime", i.Record_Insert_DateTime);
                            _cmd.Parameters.AddWithValue("_error", error);
                            _cmd.Parameters.AddWithValue("_Client_ID", i.Client_ID);
                            int msg1 = db_connection.ExecuteNonQueryProcedure(_cmd);
                        }

                        HttpWebRequest webRequest = CreateWebRequest(_url, _action);
                        InsertSoapEnvelopeIntoWebRequest(soapEnvelopeXml, webRequest);
                        IAsyncResult asyncResult = webRequest.BeginGetResponse(null, null);
                        asyncResult.AsyncWaitHandle.WaitOne();
                        string soapResult = "";
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
                            //send complaince alert
                            string Record_DateTime_exception = i.Record_Insert_DateTime;
                            string notification_icon_exception = "aml-referd.jpg";
                            string notification_message_exception = "<span class='cls-admin'>GBG AML check <strong class='cls-cancel'> Request failed.</strong></span><span class='cls-customer'></span>";
                            CompanyInfo.save_notification_compliance(notification_message_exception, notification_icon_exception, Convert.ToString(i.Customer_ID), Convert.ToDateTime(Record_DateTime_exception), i.Client_ID, 1, i.User_ID, i.Branch_ID, 0, 1, 1, 0, context);

                            string error = ex.ToString().Replace("\'", "\\'");
                            MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("SaveException");
                            _cmd.CommandType = CommandType.StoredProcedure;
                            _cmd.Parameters.AddWithValue("_Record_Insert_DateTime", i.Record_Insert_DateTime);
                            _cmd.Parameters.AddWithValue("_error", error);
                            _cmd.Parameters.AddWithValue("_Client_ID", i.Client_ID);
                            int msg1 = db_connection.ExecuteNonQueryProcedure(_cmd);


                            DataTable dtc = CompanyInfo.get(i.Client_ID, context);
                            mail_send = string.Empty;
                            string sendmsg = "GBG AML check Request failed. Please contact GBG support team. ";
                            string EmailID = "";
                            DataTable dt_admin_Email_list = (DataTable)CompanyInfo.getAdminEmailList(i.Client_ID, i.Branch_ID);
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
                            CompanyInfo.Send_Mail(dtc, EmailID, body, subject, Convert.ToInt32(i.Client_ID), Convert.ToInt32(i.Branch_ID), "", "", "", context);

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
                            int flag1 = 0, flag2 = 0;
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
                                                    string description1 = childd["Description"].InnerText.ToString();
                                                    string Record_DateTime = i.Record_Insert_DateTime;
                                                    string notification_icon = "pep-match-not-found.jpg";
                                                    //string notification_message = "<span class='cls-admin'>International Sanctions: <strong class='cls-cancel'>.</strong><br/>" + description1 + "</span><span class='cls-customer'></span>";
                                                    string notification_message = "<span class='cls-admin'> - <span class='cls-cancel'><strong> International PEP Alert </strong>- </span><span class='cls-addr-confirm'>" + description1 + " </span></span></span><span class='cls-customer'></span>";
                                                    CompanyInfo.save_notification_compliance(notification_message, notification_icon, Convert.ToString(i.Customer_ID), Convert.ToDateTime(Record_DateTime), i.Client_ID, 1, i.User_ID, i.Branch_ID, 0, 1, 1, 0, context);
                                                    flag1++;

                                                }
                                            }
                                        }
                                    }
                                    else if (node.InnerText.Contains("International Sanctions"))
                                    {
                                        if (child.Name == "Mismatch")
                                        {
                                            foreach (XmlNode childd in child.ChildNodes)
                                            {
                                                string description1 = childd["Description"].InnerText.ToString();
                                                string Record_DateTime = i.Record_Insert_DateTime;
                                                string notification_icon = "pep-match-not-found.jpg";
                                                //  string notification_message = "<span class='cls-admin'>International Sanctions: <strong class='cls-cancel'>.</strong><br/>" + description1 + "</span><span class='cls-customer'></span>";
                                                string notification_message = "<span class='cls-admin'> - <span class='cls-cancel'><strong> International Sanctions Alert </strong>- </span><span class='cls-addr-confirm'>" + description1 + " </span></span></span><span class='cls-customer'></span>";
                                                CompanyInfo.save_notification_compliance(notification_message, notification_icon, Convert.ToString(i.Customer_ID), Convert.ToDateTime(Record_DateTime), i.Client_ID, 1, i.User_ID, i.Branch_ID, 0, 1, 1, 0, context);
                                                flag2++;
                                            }
                                        }
                                    }
                                }
                            }
                            try
                            {
                                if (Convert.ToInt32(i.Customer_ID) > 0 && i.Beneficiary_ID == 0)
                                {
                                    string Query = "UPDATE `customer_registration` SET `ReKyc_Eligibility` = '0' WHERE (`Customer_ID` = " + i.Customer_ID + ")";
                                    MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("Default_SP");
                                    _cmd.CommandType = CommandType.StoredProcedure;
                                    _cmd.Parameters.AddWithValue("_Query", Query);
                                    db_connection.ExecuteNonQueryProcedure(_cmd);
                                }

                            }
                            catch
                            {

                            }

                            if (flag1 > 0 && flag2 > 0)
                            {
                                try
                                {
                                    MySqlConnector.MySqlCommand cmd_update = new MySqlConnector.MySqlCommand("update documents_details set aml_sanctions_flag=3 where SenderID_ID=@SenderID_ID");
                                    cmd_update.Parameters.AddWithValue("@SenderID_ID", i.SenderID_ID);
                                    db_connection.ExecuteNonQueryProcedure(cmd_update);
                                }
                                catch { }
                            }
                            else if (flag1 > 0)
                            {
                                try
                                {
                                    MySqlConnector.MySqlCommand cmd_update = new MySqlConnector.MySqlCommand("update documents_details set aml_sanctions_flag=1 where SenderID_ID=@SenderID_ID");
                                    cmd_update.Parameters.AddWithValue("@SenderID_ID", i.SenderID_ID);
                                    db_connection.ExecuteNonQueryProcedure(cmd_update);
                                }
                                catch { }
                            }
                            else if (flag2 > 0)
                            {
                                try
                                {
                                    MySqlConnector.MySqlCommand cmd_update = new MySqlConnector.MySqlCommand("update documents_details set aml_sanctions_flag=2 where SenderID_ID=@SenderID_ID");
                                    cmd_update.Parameters.AddWithValue("@SenderID_ID", i.SenderID_ID);
                                    db_connection.ExecuteNonQueryProcedure(cmd_update);
                                }
                                catch { }
                            }
                            try
                            {
                                int status = 1; string Function_name = CallFrom;
                                Remark = Convert.ToString(CompanyInfo.getAPIStatus(Bandtext, i.Client_ID));
                                if (Remark != null && Remark != "")
                                    Status = 0;

                                MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("UpdateCustomerBandText");
                                _cmd.CommandType = CommandType.StoredProcedure;
                                _cmd.Parameters.AddWithValue("_Client_ID", i.Client_ID);
                                _cmd.Parameters.AddWithValue("_Customer_ID", i.Customer_ID);
                                _cmd.Parameters.AddWithValue("_Remark", Remark);
                                int msg = db_connection.ExecuteNonQueryProcedure(_cmd);
                                _cmd.Dispose();

                                _cmd = new MySqlConnector.MySqlCommand("SaveAPIRequestResponce");
                                _cmd.CommandType = CommandType.StoredProcedure;
                                _cmd.Parameters.AddWithValue("_API_ID", API_ID);
                                _cmd.Parameters.AddWithValue("_Client_ID", i.Client_ID);
                                _cmd.Parameters.AddWithValue("_Customer_ID", i.Customer_ID);
                                _cmd.Parameters.AddWithValue("_status", status);
                                _cmd.Parameters.AddWithValue("_Function_name", Function_name);
                                _cmd.Parameters.AddWithValue("_Remark", Remark);
                                _cmd.Parameters.AddWithValue("_comments", soapResult.Replace("'", "''"));
                                _cmd.Parameters.AddWithValue("_Record_Insert_DateTime", i.Record_Insert_DateTime);
                                _cmd.Parameters.AddWithValue("_Branch_ID", i.Branch_ID);
                                int msg1 = db_connection.ExecuteNonQueryProcedure(_cmd);

                            }
                            catch (Exception ex)
                            {
                                string error = ex.ToString().Replace("\'", "\\'");

                                MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("SaveException");
                                _cmd.CommandType = CommandType.StoredProcedure;
                                _cmd.Parameters.AddWithValue("_Record_Insert_DateTime", i.Record_Insert_DateTime);
                                _cmd.Parameters.AddWithValue("_error", error);
                                _cmd.Parameters.AddWithValue("_Client_ID", i.Client_ID);
                                int msg1 = db_connection.ExecuteNonQueryProcedure(_cmd);
                            }
                            try
                            {
                                if (Remark == "2" || Remark == "1")//alert or refer then  send mail to admin
                                {
                                    string Record_DateTime = i.Record_Insert_DateTime;
                                    string notification_icon = "aml-referd.jpg";
                                    string notification_message = "<span class='cls-admin'>GBG AML check result is <strong class='cls-cancel'>" + Bandtext + ".</strong></span><span class='cls-customer'></span>";
                                    CompanyInfo.save_notification_compliance(notification_message, notification_icon, Convert.ToString(i.Customer_ID), Convert.ToDateTime(Record_DateTime), i.Client_ID, 1, i.User_ID, i.Branch_ID, 0, 1, 1, 0, context);
                                }
                                else
                                {
                                    string Record_DateTime = i.Record_Insert_DateTime;
                                    string notification_icon = "primary-id-upload.jpg";
                                    string notification_message = "<span class='cls-admin'>GBG AML check result is <strong class='cls-priamary'>" + Bandtext + ".</strong></span><span class='cls-customer'></span>";
                                    CompanyInfo.save_notification_compliance(notification_message, notification_icon, Convert.ToString(i.Customer_ID), Convert.ToDateTime(Record_DateTime), i.Client_ID, 1, i.User_ID, i.Branch_ID, 0, 1, 1, 0, context);
                                }

                            }
                            catch (Exception e)
                            {
                            }
                            try
                            {
                                if (Remark == "1" || Remark == "2" || Remark == "3")//alert or refer then  send mail to admin
                                {

                                    string Company_Name = "";
                                    DataTable dtc = CompanyInfo.get(i.Client_ID, context);

                                    if (dtc.Rows.Count > 0)
                                    {
                                        EmailUrl = Convert.ToString(dtc.Rows[0]["Company_URL_Admin"]);
                                        Company_Name = Convert.ToString(dtc.Rows[0]["Company_Name"]);

                                    }

                                    MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("customer_details_by_param");
                                    _cmd.CommandType = CommandType.StoredProcedure;
                                    string _whereclause = " and cr.Client_ID=" + i.Client_ID + " and cr.Customer_ID=" + i.Customer_ID;
                                    //  DataTable d1 = (DataTable)mtsmethods.GetCustDetailsByID(c.Customer_ID); //get customer details by id

                                    _cmd.Parameters.AddWithValue("_whereclause", _whereclause);
                                    _cmd.Parameters.AddWithValue("_SecurityKey", CompanyInfo.SecurityKey());
                                    DataTable d1 = db_connection.ExecuteQueryDataTableProcedure(_cmd);
                                    string sendmsg = "Response from third Party AML check for  " + d1.Rows[0]["WireTransfer_ReferanceNo"] + "   is   " + Bandtext + "";
                                    //      string sendmsg = "Probable duplicate  match found for newly registered customer " + c.First_Name + " " + c.Middle_Name + " " + c.Last_Name + "  with reference number " + c.WireTransfer_ReferanceNo + "";
                                    //string sendmsg = " While adding Customer " + c.First_Name + "  " + c.Last_Name + Convert.ToString(c.WireTransfer_ReferanceNo) + " ,there was a probable match found . Please check Customer profile. ";





                                    string EmailID = "";
                                    DataTable dt_admin_Email_list = (DataTable)CompanyInfo.getAdminEmailList(i.Client_ID, i.Branch_ID);
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

                                    subject = "" + Company_Name + " - Compliance - Alert - " + d1.Rows[0]["WireTransfer_ReferanceNo"];
                                    //subject = "" + HttpContext.Current.Session["Company_Name"] + " -  Incomplete Customer Registration Details " + c.WireTransfer_ReferanceNo;
                                    //    mail_send = (string)mtsmethods.Send_Mail(email, body, subject, Convert.ToInt32(c.Client_ID), Convert.ToInt32(c.Branch_ID), "Alert Admins", "", "");

                                    mail_send = (string)CompanyInfo.Send_Mail(dtc, EmailID, body, subject, i.Client_ID, i.Branch_ID, "", "", "", context);
                                }
                            }
                            catch (Exception ae)
                            {
                                //string stattus = (string)mtsmethods.InsertErrorLogDetails(ae.Message.Replace("\'", "\\'"), user_id, "Check Credit Safe", i.Branch_ID, i.Client_ID);
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                string stattus = (string)CompanyInfo.InsertErrorLogDetails(ex.Message.Replace("\'", "\\'"), 0, "Check Credit Safe", i.Branch_ID, i.Client_ID);
            }

            string[] response = { Status.ToString(), Bandtext };
            return response;
        }

        public string[] CHECK_AML(Model.Doc obj, int Customer_ID, string CallFrom, HttpContext context)  // Complience  Assist AMl Check Rushikesh.
        {
            int Status = 1;
            string Bandtext = "";
            int Client_ID = obj.Client_ID;
            string BaseCurrency_Code = string.Empty;
            string BaseCurrency_TimeZone = string.Empty;
            string BaseCurrency_Sign = string.Empty;
            string Cancel_Transaction_Hours = string.Empty;
            string Company_Name = string.Empty;
            string CustomerURL = string.Empty;
            string EmailUrl = string.Empty;
            string BaseCountry_ID = string.Empty;
            string BaseCurrency_Country = string.Empty;
            string RootURL = string.Empty;
            DataTable ds1 = CompanyInfo.get(Client_ID, context);
            int stattus12 = (int)CompanyInfo.InsertActivityLogDetails("Inside CHECK_AML" + Customer_ID, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(Customer_ID), "GetJourney", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "ID Upload", context);
            try
            {

                string mail_send = string.Empty;

                string RefNum = "";
                string Addressline_2 = "";
                string Gender = "";
                string formattedDateOfBirth = "";
                int flag1 = 0, flag2 = 0, flag4 = 0, flag5 = 0, flag6 = 0, flag7 = 0;
                int flag = 0;
                string requestStatus = "";
                string status = "";
                int apiRequestId = 0;
                DateTime dateTime = new DateTime();
                long requestId = 0;
                string requestReference = "";
                long subjectId = 0;
                string subjectReference = "";
                JArray matchStatusesArr = new JArray();
                string First_Name = "";
                string Middle_Name = "";
                string Last_Name = "";
                string Country = "";
                string House_Number = "";
                string Street = "";
                string City = "";
                string Post_Code = "";
                string Record_Insert_DateTime = "";
                string SenderID_ExpiryDate = "";
                string WireTransfer_ReferanceNo = "";
                DateTime dateOfBirth = new DateTime();
                string SenderID_Number = "";
                int IDName_ID = 0;
                int SenderID_ID = 0;
                string monitorCommand = "";




                string Remark = "";
                try
                {
                    MySqlCommand cmdupdate1 = new MySqlCommand("Get_Permissions");
                    cmdupdate1.CommandType = CommandType.StoredProcedure;
                    cmdupdate1.Parameters.AddWithValue("Per_ID", 32);
                    cmdupdate1.Parameters.AddWithValue("ClientID", obj.Client_ID);
                    DataTable pm = db_connection.ExecuteQueryDataTableProcedure(cmdupdate1);
                    if (pm.Rows.Count > 0)
                    {
                        if (Convert.ToString(pm.Rows[0]["Status_ForCustomer"]) == "0")
                        {

                            obj.Customer_ID = Customer_ID.ToString();
                            int API_ID = 10;


                            MySqlCommand cmdp_active = new MySqlCommand("active_thirdparti_aml_api");
                            cmdp_active.CommandType = CommandType.StoredProcedure;
                            string whereclause = "API_ID =" + 10;
                            cmdp_active.Parameters.AddWithValue("_whereclause", whereclause);

                            DataTable dtApi = db_connection.ExecuteQueryDataTableProcedure(cmdp_active);


                            if (dtApi.Rows.Count > 0)
                            {
                                stattus12 = (int)CompanyInfo.InsertActivityLogDetails("Inside active_thirdparti_aml_api" + Customer_ID, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(Customer_ID), "GetJourney", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "ID Upload", context);
                                string StateDistrict = "";
                                //get Customer details
                                MySqlCommand getcust = new MySqlCommand("GetCustomer_all_details"); //  chnages by rushikesh
                                getcust.CommandType = CommandType.StoredProcedure;
                                whereclause = "and cr.Customer_ID=" + Customer_ID + " and cr.Client_ID=" + Client_ID;
                                getcust.Parameters.AddWithValue("_whereclause", whereclause);

                                DataTable dc = db_connection.ExecuteQueryDataTableProcedure(getcust);
                                if (dc.Rows.Count > 0)
                                {
                                    try
                                    {
                                        stattus12 = (int)CompanyInfo.InsertActivityLogDetails("Inside GetCustomer_all_details" + whereclause, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(Customer_ID), "GetJourney", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "ID Upload", context);
                                        WireTransfer_ReferanceNo = Convert.ToString(dc.Rows[0]["WireTransfer_ReferanceNo"]);
                                        First_Name = Convert.ToString(dc.Rows[0]["First_Name"]).Replace("&", "AND");
                                        Middle_Name = Convert.ToString(dc.Rows[0]["Middle_Name"]).Replace("&", "AND");
                                        Last_Name = Convert.ToString(dc.Rows[0]["Last_Name"]);
                                        Country = Convert.ToString(dc.Rows[0]["Country_Name"]).ToLower();
                                        House_Number = Convert.ToString(dc.Rows[0]["House_Number"]).Replace("&", "AND");
                                        Street = Convert.ToString(dc.Rows[0]["Street"]).Replace("&", "AND");
                                        City = Convert.ToString(dc.Rows[0]["City_Name"]).Replace("&", "AND");
                                        Post_Code = Convert.ToString(dc.Rows[0]["Post_Code"]).Replace("&", "AND");
                                        Record_Insert_DateTime = CompanyInfo.gettime(Client_ID, context);
                                        //SenderID_ExpiryDate = Convert.ToString(dc.Rows[0]["SenderID_ExpiryDate"]);
                                        dateOfBirth = Convert.ToDateTime(dc.Rows[0]["DateOf_Birth"]);
                                        try { formattedDateOfBirth = dateOfBirth.ToString("yyyy-MM-dd"); } catch { }
                                        //SenderID_Number = Convert.ToString(dc.Rows[0]["SenderID_Number"]);
                                        //IDName_ID = Convert.ToInt32(dc.Rows[0]["IDName_ID"]);
                                        SenderID_ID = Convert.ToInt32(dc.Rows[0]["SenderID_ID"]);
                                        //StateDistrict = Convert.ToString(dc.Rows[0]["code"]);
                                        RefNum = Convert.ToString(dc.Rows[0]["WireTransfer_ReferanceNo"]);
                                        Addressline_2 = Convert.ToString(dc.Rows[0]["Addressline_2"]);
                                        Gender = Convert.ToString(dc.Rows[0]["Gender"]).ToUpper();
                                    }
                                    catch (Exception ex)
                                    {
                                        string stattus = (string)CompanyInfo.InsertErrorLogDetails("Error in accessing values from GetCustomer_all_details" + ex.ToString().Replace("\'", "\\'"), 0, "CHECK_AML", obj.Branch_ID, obj.Client_ID);
                                    }


                                    var _url = ""; //Convert.ToString(dtApi.Rows[0]["API_URL"]);
                                    var UserName = "";//Convert.ToString(dtApi.Rows[0]["UserName"]);
                                    var Password = "";// Convert.ToString(dtApi.Rows[0]["Password"]);
                                                      //var ProfileID = Convert.ToString(dtApi.Rows[0]["ProfileID"]);
                                    var _action = "http://www.id3global.com/ID3gWS/2013/04/IGlobalAuthenticate/AuthenticateSP";
                                    var ProfileID = "";
                                    var tokenurl = "";
                                    var Encryption_Key = "";
                                    var Request_Type_Code_Benef = "";
                                    var Request_Type_Code_Cust = "";
                                    string status3 = "";
                                    if (dtApi.Rows.Count > 0)
                                    {
                                        _url = Convert.ToString(dtApi.Rows[0]["API_URL"]);
                                        UserName = Convert.ToString(dtApi.Rows[0]["UserName"]);
                                        Password = Convert.ToString(dtApi.Rows[0]["Password"]);

                                        ProfileID = Convert.ToString(dtApi.Rows[0]["ProfileID"]);
                                        if (ProfileID != "" && ProfileID != null)
                                        {
                                            Newtonsoft.Json.Linq.JObject objj = Newtonsoft.Json.Linq.JObject.Parse(ProfileID);
                                            tokenurl = Convert.ToString(objj["tokenurl"]);
                                            Encryption_Key = Convert.ToString(objj["Encryption_Key"]);
                                            Request_Type_Code_Benef = Convert.ToString(objj["Request_Type_Code_Benef"]);
                                            Request_Type_Code_Cust = Convert.ToString(objj["Request_Type_Code_Cust"]);

                                        }
                                    }
                                    XmlDocument soapEnvelopeXml = new XmlDocument();
                                    var iddoc = "";

                                    string passdoc = "", passid = "", endid = "";

                                    if (Country == "united kingdom" || Country == "uk") { Country = "UK"; }
                                    else if (Country == "new zealand") { Country = "NewZealand"; }
                                    else if (Country == "us" || Country == "united states") { Country = "US"; }
                                    else if (Country == "china" || Country == "india" || Country == "canada" || Country == "mexico" || Country == "brazil" || Country == "spain" || Country == "argentina")
                                    {
                                        Country = obj.country_code;
                                    }
                                    else { Country = ""; }
                                    string province = "";
                                    if (Country == "canada")
                                    {
                                        province = "<ns:StateDistrict>" + StateDistrict + @"</ns:StateDistrict>" +
                                                    "<ns:Region>" + StateDistrict + @"</ns:Region>";
                                    }

                                    string passexpiry = "";
                                    string expiry = Convert.ToString(SenderID_ExpiryDate);
                                    if (expiry != "" && expiry != null)
                                    {
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

                                    if (IDName_ID == 1)// if Passport
                                    {
                                        passdoc += "<ns:IdentityDocuments><ns:InternationalPassport>";
                                        passdoc += "<ns:Number>" + SenderID_Number + @"</ns:Number> " + passexpiry + "";
                                        if (Country != "") { passdoc += "<ns:CountryOfOrigin>" + Country + @"</ns:CountryOfOrigin>"; }
                                        passdoc += "</ns:InternationalPassport></ns:IdentityDocuments>";

                                    }
                                    else if (IDName_ID == 2 && Country != "")//if Driving licence
                                    {
                                        passdoc += "<ns:IdentityDocuments><ns:" + Country + @"><ns:DrivingLicence>";
                                        passdoc += "<ns:Number>" + SenderID_Number + @"</ns:Number> " + passexpiry + "";
                                        passdoc += "</ns:DrivingLicence></ns:" + Country + @"></ns:IdentityDocuments>";
                                    }
                                    else if (IDName_ID == 3)//EU Nationality Card
                                    {
                                        passdoc += "<ns:IdentityDocuments><ns:IdentityCard>";
                                        passdoc += "<ns:Number>" + SenderID_Number + @"</ns:Number>";
                                        if (Country != "") { passdoc += "<ns:Country>" + Country + @"</ns:Country>"; }
                                        passdoc += "</ns:IdentityCard></ns:IdentityDocuments>";
                                    }

                                    string passdob = "";

                                    string strmiddlename = "";
                                    if (Middle_Name != "" && Middle_Name != null)
                                    {
                                        strmiddlename = "<ns:MiddleName>" + Middle_Name + @"</ns:MiddleName>";
                                    }
                                    // Complience API CAll
                                    DataTable dtb = CompanyInfo.get(Client_ID, context);
                                    string rooturl = "";
                                    if (dtb.Rows.Count > 0)
                                    {
                                        //CURL = Convert.ToString(dtb.Rows[0]["RootURL"]);
                                        //Company_Name = Convert.ToString(dtb.Rows[0]["Company_Name"]);

                                        rooturl = Convert.ToString(dtb.Rows[0]["RootURL"]);
                                    }





                                    try
                                    {
                                        stattus12 = (int)CompanyInfo.InsertActivityLogDetails("befor tokenurl" + Customer_ID, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(Customer_ID), "GetJourney", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "ID Upload", context);


                                        string url = tokenurl;// "https://auth-u.complianceassist.co.uk";
                                        string clientId = UserName;///"4i37l17880j149sin1gpqid8r2";
                                        string clientSecret = Password;//"1lih0ev0bo7jvd2fl9emvl9k0b0dm2ulji7qe0naq9gf3g9mu3op";

                                        string[] scopes = {
                                 "uat/requesttypes.read",
                                 "uat/requests.read",
                                 "uat/requests.write",
                                 "uat/subjects.read",
                                 "uat/subjects.write"
                                 };

                                        string scope = string.Join(" ", scopes);





                                        // Other code remains the same
                                        var client = new RestClient(url + "/oauth2/token");
                                        client.Timeout = -1;
                                        var request = new RestRequest(Method.POST);
                                        string authHeaderValue = Convert.ToBase64String(Encoding.UTF8.GetBytes(clientId + ":" + clientSecret));
                                        request.AddHeader("Authorization", "Basic " + authHeaderValue);
                                        request.AddHeader("Accept", "application/json");
                                        request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                                        request.AddParameter("grant_type", "client_credentials");
                                        request.AddParameter("scope", scope);

                                        IRestResponse response1 = client.Execute(request);

                                        JObject jsonResponse = JObject.Parse(response1.Content);

                                        stattus12 = (int)CompanyInfo.InsertActivityLogDetails("after tokenurl" + response1.Content, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(Customer_ID), "GetJourney", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "ID Upload", context);


                                        string accessToken = jsonResponse["access_token"].ToString();



                                        // Assuming you have already obtained the accessToken variable from the previous step

                                        url = _url;//"https://web-u.complianceassist.co.uk/api/v2_0/";


                                        if (RefNum == "")
                                        {
                                            RefNum = WireTransfer_ReferanceNo;
                                            stattus12 = (int)CompanyInfo.InsertActivityLogDetails("Reference number =" + RefNum, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(Customer_ID), "GetJourney", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "ID Upload", context);
                                        }
                                        try
                                        {
                                            stattus12 = (int)CompanyInfo.InsertActivityLogDetails("before subjects?reference=", Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(Customer_ID), "GetJourney", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "ID Upload", context);

                                            client = new RestClient(url + "subjects?reference=" + RefNum + "&details=true");
                                            client.Timeout = -1;
                                            request = new RestRequest(Method.GET);
                                            request.AddHeader("Authorization", "Bearer " + accessToken);
                                            request.AddHeader("Accept", "application/json");

                                            response1 = client.Execute(request);


                                            // Parse the JSON response
                                            jsonResponse = JObject.Parse(response1.Content);
                                            stattus12 = (int)CompanyInfo.InsertActivityLogDetails("after subjects?reference=" + response1.Content, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(Customer_ID), "GetJourney", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "ID Upload", context);


                                            status3 = (string)jsonResponse["status"];
                                            int apiRequestId3 = (int)jsonResponse["apiRequestId"];
                                            DateTime dateTime3 = (DateTime)jsonResponse["dateTime"];

                                            JArray subjects = (JArray)jsonResponse["subjects"];

                                            if (subjects != null)
                                            {
                                                foreach (var subject in subjects)
                                                {
                                                    JObject subjectObject = (JObject)subject;
                                                    int subjectId3 = (int)subjectObject["subjectId"];
                                                    string subjectReference3 = (string)subjectObject["subjectReference"];
                                                    string subjectIdentifier = (string)subjectObject["subjectIdentifier"];
                                                    string subjectType = (string)subjectObject["subjectType"];
                                                    string subjectStatus = (string)subjectObject["subjectStatus"];
                                                    bool isMonitored = (bool)subjectObject["isMonitored"];
                                                    string monitoredRequestType = (string)subjectObject["monitoredRequestType"];
                                                    string riskRating = (string)subjectObject["riskRating"];
                                                    if (riskRating == "Low")
                                                    {
                                                        requestStatus = "Completed";
                                                        Remark = "0";
                                                    }
                                                    else
                                                    {
                                                        requestStatus = "Processing";
                                                        Remark = "1";
                                                    }

                                                }
                                            }
                                            if (status3 == "success")
                                            {

                                                stattus12 = (int)CompanyInfo.InsertActivityLogDetails("inside  monitorCommand = CHANGE", Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(Customer_ID), "GetJourney", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "ID Upload", context);

                                                monitorCommand = "CHANGE";

                                            }
                                            else
                                            {
                                                stattus12 = (int)CompanyInfo.InsertActivityLogDetails("inside  monitorCommand = ADD", Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(Customer_ID), "GetJourney", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "ID Upload", context);

                                                monitorCommand = "ADD";
                                            }

                                        }

                                        catch (Exception ex)
                                        {
                                            string stattus = (string)CompanyInfo.InsertErrorLogDetails(ex.Message.Replace("\'", "\\'"), 0, "Complience Assist AMl Check - 4th api call (Subject Information)  ", obj.Branch_ID, obj.Client_ID);

                                        }


                                        if (status3 != "success" || status3 == "success")
                                        {
                                            stattus12 = (int)CompanyInfo.InsertActivityLogDetails("befor body" + response1.Content, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(Customer_ID), "GetJourney", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "ID Upload", context);

                                            string bodyJson = @"
{
    ""requestType"": """ + Request_Type_Code_Cust + @""",
    ""subjectType"": ""INDIVIDUAL"",
    ""subjectReference"": """ + RefNum + @""",
    ""requestReference"": ""Onboarding " + RefNum + @""",
    ""subjectDetails"": {
        ""firstName"": """ + First_Name + @""",
        ""surname"": """ + Last_Name + @""",
        ""dateOfBirth"": """ + formattedDateOfBirth + @""",
        ""gender"": """ + Gender + @""",
        ""houseNumber"": """ + House_Number + @""",
        ""addressLine1"":""" + Addressline_2 + @""",
        ""addressLine2"": """ + Addressline_2 + @""",
        ""city"":""" + City + @""",
        ""postcode"": """ + Post_Code + @""",
        ""country"": """ + Country + @"""
    },
    ""notes"": ""Onboarded under project Alpha."",
    ""monitorCommand"": """ + monitorCommand + @""",
    ""callbackUrl"": """"
}
";

                                            try
                                            {

                                                client = new RestClient(url + "requests");
                                                client.Timeout = -1;
                                                request = new RestRequest(Method.POST);
                                                request.AddHeader("Authorization", "Bearer " + accessToken);
                                                request.AddHeader("Accept", "application/json");
                                                request.AddHeader("Content-Type", "application/json");
                                                request.AddParameter("application/json", bodyJson, ParameterType.RequestBody);

                                                response1 = client.Execute(request);

                                                // Parse the JSON response
                                                jsonResponse = JObject.Parse(response1.Content);

                                                stattus12 = (int)CompanyInfo.InsertActivityLogDetails("after create customer" + response1.Content, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(Customer_ID), "GetJourney", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "ID Upload", context);


                                                status = (string)jsonResponse["status"];
                                                apiRequestId = (int)jsonResponse["apiRequestId"];
                                                dateTime = (DateTime)jsonResponse["dateTime"];
                                                requestId = (long)jsonResponse["requestId"];
                                                requestReference = (string)jsonResponse["requestReference"];
                                                subjectId = (long)jsonResponse["subjectId"];
                                                subjectReference = (string)jsonResponse["subjectReference"];
                                                requestStatus = (string)jsonResponse["requestStatus"];
                                                Bandtext = requestStatus;


                                                // Extract nested properties under "results"
                                                JObject resultsObj = (JObject)jsonResponse["results"];
                                                JObject watchlistsResultsObj = (JObject)resultsObj["watchlistsResults"];
                                                int totalNumberOfMatches = (int)watchlistsResultsObj["totalNumberOfMatches"];

                                                // Extract array values under "matchStatuses"
                                                matchStatusesArr = (JArray)watchlistsResultsObj["matchStatuses"];
                                                if (matchStatusesArr != null)
                                                {
                                                    // Iterate through each item in the "matchStatuses" array
                                                    foreach (JObject matchStatusObj in matchStatusesArr)
                                                    {
                                                        string matchStatus = (string)matchStatusObj["matchStatus"];
                                                        int numberOfMatches = (int)matchStatusObj["numberOfMatches"];

                                                        // Extract nested properties under "matchTypes"
                                                        JObject matchTypesObj = (JObject)matchStatusObj["matchTypes"];
                                                        bool isSan = (bool)matchTypesObj["san"];   // Indicates if there are sanction matches


                                                        bool isAdv = (bool)matchTypesObj["adv"];   //Indicates if there are adverse media matches


                                                        bool isPep = (bool)matchTypesObj["pep"];   //Indicates if there are PEP (politically exposed persons) matches


                                                        bool isRca = (bool)matchTypesObj["rca"];   //Indicates if there are RCA (relatives or close associates) matches


                                                        bool isSoc = (bool)matchTypesObj["soc"];   //Indicates if there are SOC (state owned companies) matches


                                                        bool isOther = (bool)matchTypesObj["other"]; //Indicates if there are other types of matches





                                                        if (isPep == true)
                                                        {
                                                            flag1 = 1; // set flag 1
                                                            flag = 1;
                                                        }
                                                        else if (isSan == true)
                                                        {
                                                            flag2 = 2; // set flag 2
                                                            flag = 2;
                                                        }
                                                        else if (isAdv == true)
                                                        {
                                                            flag4 = 4; // set flag 2
                                                            flag = 4;
                                                        }
                                                        else if (isRca == true)
                                                        {
                                                            flag5 = 5; // set flag 2
                                                            flag = 5;
                                                        }
                                                        else if (isSoc == true)
                                                        {
                                                            flag6 = 6; // set flag 2
                                                            flag = 6;
                                                        }
                                                        else if (isOther == true)
                                                        {
                                                            flag7 = 7; // set flag 2
                                                            flag = 7;
                                                        }
                                                        if (flag1 > 0 && flag2 > 0)
                                                        {
                                                            flag = 3; // Set flag 3
                                                        }


                                                    }
                                                }
                                                int remark = 0;
                                                if (flag == 0)
                                                {
                                                    remark = 0;
                                                    Remark = "0";
                                                }
                                                else
                                                {
                                                    remark = 1;
                                                    Remark = "1";
                                                }

                                                string request3 = url + "requests" + bodyJson;

                                                try
                                                {
                                                    MySqlCommand _cmd = new MySqlCommand("SaveAPIRequestResponce");
                                                    _cmd.CommandType = CommandType.StoredProcedure;
                                                    _cmd.Parameters.AddWithValue("_API_ID", 10);
                                                    _cmd.Parameters.AddWithValue("_Client_ID", Client_ID);
                                                    _cmd.Parameters.AddWithValue("_Customer_ID", Customer_ID);
                                                    _cmd.Parameters.AddWithValue("_status", 0);
                                                    _cmd.Parameters.AddWithValue("_Function_name", CallFrom);
                                                    _cmd.Parameters.AddWithValue("_Remark", remark.ToString());
                                                    _cmd.Parameters.AddWithValue("_comments", request3);
                                                    _cmd.Parameters.AddWithValue("_Record_Insert_DateTime", Record_Insert_DateTime);
                                                    _cmd.Parameters.AddWithValue("_Branch_ID", obj.Branch_ID);
                                                    int msg1 = db_connection.ExecuteNonQueryProcedure(_cmd);
                                                }
                                                catch (Exception ex)
                                                {
                                                    string error = ex.ToString().Replace("\'", "\\'");
                                                    MySqlCommand _cmd = new MySqlCommand("SaveException");
                                                    _cmd.CommandType = CommandType.StoredProcedure;
                                                    _cmd.Parameters.AddWithValue("_Record_Insert_DateTime", Record_Insert_DateTime);
                                                    _cmd.Parameters.AddWithValue("_error", error);
                                                    _cmd.Parameters.AddWithValue("_Client_ID", Client_ID);
                                                    int msg1 = db_connection.ExecuteNonQueryProcedure(_cmd);
                                                }


                                                // string request = url + "requests" + bodyJson;

                                                try
                                                {
                                                    MySqlCommand _cmd = new MySqlCommand("SaveAPIRequestResponce");
                                                    _cmd.CommandType = CommandType.StoredProcedure;
                                                    _cmd.Parameters.AddWithValue("_API_ID", API_ID);
                                                    _cmd.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                                                    _cmd.Parameters.AddWithValue("_Customer_ID", Customer_ID);
                                                    _cmd.Parameters.AddWithValue("_status", 1);
                                                    _cmd.Parameters.AddWithValue("_Function_name", CallFrom);
                                                    _cmd.Parameters.AddWithValue("_Remark", remark);
                                                    _cmd.Parameters.AddWithValue("_comments", jsonResponse);
                                                    _cmd.Parameters.AddWithValue("_Record_Insert_DateTime", Record_Insert_DateTime);
                                                    _cmd.Parameters.AddWithValue("_Branch_ID", obj.Branch_ID);
                                                    int msg1 = db_connection.ExecuteNonQueryProcedure(_cmd);
                                                }
                                                catch (Exception ex)
                                                {
                                                    string error = ex.ToString().Replace("\'", "\\'");
                                                    MySqlCommand _cmd = new MySqlCommand("SaveException");
                                                    _cmd.CommandType = CommandType.StoredProcedure;
                                                    _cmd.Parameters.AddWithValue("_Record_Insert_DateTime", Record_Insert_DateTime);
                                                    _cmd.Parameters.AddWithValue("_error", error);
                                                    _cmd.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                                                    int msg1 = db_connection.ExecuteNonQueryProcedure(_cmd);
                                                }

                                            }
                                            catch (Exception ex)
                                            {
                                                string stattus = (string)CompanyInfo.InsertErrorLogDetails(ex.Message.Replace("\'", "\\'"), 0, "Complience Assist AMl Check - 1st api call ", obj.Branch_ID, obj.Client_ID);

                                            }
                                        }
                                        ///""WGMG001"",

                                        if (requestId != 0)
                                        {

                                            try
                                            {

                                                //ServicePointManager.Expect100Continue = true;
                                                //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
                                                //       | SecurityProtocolType.Tls11
                                                //       | SecurityProtocolType.Tls12
                                                //       | SecurityProtocolType.Ssl3;
                                                client = new RestClient(url + "requests/" + requestId);
                                                client.Timeout = 3000; // Set a timeout of 3 seconds
                                                request = new RestRequest(Method.GET);
                                                request.AddHeader("Authorization", "Bearer " + accessToken);
                                                request.AddHeader("Accept", "application/pdf");
                                                request.AddHeader("Content-Type", "application/json"); // Add Content-Type header
                                                response1 = client.Execute(request);

                                                if (response1.StatusCode == HttpStatusCode.OK)
                                                {
                                                    byte[] pdfData = response1.RawBytes;

                                                    // Write the PDF data to a file


                                                    if (pdfData != null && pdfData.Length > 0)
                                                    {
                                                        // Write the PDF data to a file
                                                        string filePath = "assets/Other_Docs/" + "PDF-" + RefNum + "-" + Record_Insert_DateTime.Replace(":", "") + ".pdf";
                                                        string URL = rooturl + filePath;
                                                        try
                                                        {
                                                            File.WriteAllBytes(URL, pdfData);
                                                            Console.WriteLine("PDF saved successfully at: " + filePath);

                                                            string pdfdownload = "0";

                                                            string func_name = "Save_compliance_pdf";
                                                            filePath = filePath + "=" + requestId;
                                                            try
                                                            {
                                                                MySqlCommand _cmd5 = new MySqlCommand("Insert_PDF_Data");
                                                                _cmd5.CommandType = CommandType.StoredProcedure;
                                                                _cmd5.Parameters.AddWithValue("_API_ID", API_ID);
                                                                _cmd5.Parameters.AddWithValue("_Client_ID", Client_ID);
                                                                _cmd5.Parameters.AddWithValue("_Customer_ID", Customer_ID);
                                                                _cmd5.Parameters.AddWithValue("_status", 1);
                                                                _cmd5.Parameters.AddWithValue("_Function_name", func_name);
                                                                _cmd5.Parameters.AddWithValue("_Remark", Remark);
                                                                _cmd5.Parameters.AddWithValue("_comments", filePath);
                                                                _cmd5.Parameters.AddWithValue("_Record_Insert_DateTime", Record_Insert_DateTime);
                                                                _cmd5.Parameters.AddWithValue("_Branch_ID", obj.Branch_ID);
                                                                int msg5 = db_connection.ExecuteNonQueryProcedure(_cmd5);
                                                            }
                                                            catch (Exception ex)
                                                            {
                                                                string error = ex.ToString().Replace("\'", "\\'");
                                                                MySqlCommand _cmd5 = new MySqlCommand("SaveException");
                                                                _cmd5.CommandType = CommandType.StoredProcedure;
                                                                _cmd5.Parameters.AddWithValue("_Record_Insert_DateTime", Record_Insert_DateTime);
                                                                _cmd5.Parameters.AddWithValue("_error", error);
                                                                _cmd5.Parameters.AddWithValue("_Client_ID", Client_ID);
                                                                int msg5 = db_connection.ExecuteNonQueryProcedure(_cmd5);
                                                            }



                                                        }
                                                        catch (Exception ex)
                                                        {
                                                            Console.WriteLine("Error saving PDF: " + ex.Message);
                                                        }
                                                    }

                                                }
                                                else
                                                {
                                                    // Handle the case where the request fails or returns an error
                                                    Console.WriteLine("Error: " + response1.StatusCode);
                                                }

                                            }

                                            catch (Exception ex)
                                            {
                                                string stattus = (string)CompanyInfo.InsertErrorLogDetails(ex.Message.Replace("\'", "\\'"), 0, "Complience Assist AMl Check - 2st api call (PDF download)  ", obj.Branch_ID, obj.Client_ID);

                                            }


                                        }




                                        try
                                        {

                                            client = new RestClient(url + "requestTypes/WGMG001");
                                            client.Timeout = -1;
                                            request = new RestRequest(Method.GET);
                                            request.AddHeader("Authorization", "Bearer " + accessToken);
                                            request.AddHeader("Accept", "application/json");


                                            response1 = client.Execute(request);


                                            //Parse the JSON response
                                            jsonResponse = JObject.Parse(response1.Content);

                                            string status2 = (string)jsonResponse["status"];
                                            int apiRequestId2 = (int)jsonResponse["apiRequestId"];
                                            DateTime dateTime2 = (DateTime)jsonResponse["dateTime"];
                                            string shortCode = (string)jsonResponse["shortCode"];

                                            JArray subjectTypes = (JArray)jsonResponse["subjectTypes"];
                                            JArray requestFields = (JArray)jsonResponse["requestFields"];
                                            JArray responseElements = (JArray)jsonResponse["responseElements"];

                                            foreach (var subjectType in subjectTypes)
                                            {
                                                Console.WriteLine("Subject Type: " + subjectType);
                                            }

                                            foreach (var requestField in requestFields)
                                            {
                                                JObject fieldObject = (JObject)requestField;
                                                string subjectType = (string)fieldObject["subjectType"];
                                                Console.WriteLine("Subject Type: " + subjectType);
                                                JArray fields = (JArray)fieldObject["fields"];
                                                foreach (var field in fields)
                                                {
                                                    JObject fieldDetails = (JObject)field;
                                                    string fieldName = (string)fieldDetails["fieldName"];
                                                    string displayName = (string)fieldDetails["displayName"];
                                                    bool mandatory = (bool)fieldDetails["mandatory"];
                                                    Console.WriteLine("Field Name: " + fieldName + ", Display Name: " + displayName + ", Mandatory: " + mandatory);
                                                }
                                            }

                                            foreach (var responseElement in responseElements)
                                            {
                                                Console.WriteLine("Response Element: " + responseElement);
                                            }

                                        }

                                        catch (Exception ex)
                                        {
                                            string stattus = (string)CompanyInfo.InsertErrorLogDetails(ex.Message.Replace("\'", "\\'"), 0, "Complience Assist AMl Check - 3rd api call  ", obj.Branch_ID, obj.Client_ID);

                                        }

                                        if (subjectId != 0)
                                        {

                                            try
                                            {

                                                client = new RestClient(url + "subjects?id=" + subjectId);
                                                client.Timeout = -1;
                                                request = new RestRequest(Method.GET);
                                                request.AddHeader("Authorization", "Bearer " + accessToken);
                                                request.AddHeader("Accept", "application/json");

                                                response1 = client.Execute(request);


                                                // Parse the JSON response
                                                jsonResponse = JObject.Parse(response1.Content);


                                                status3 = (string)jsonResponse["status"];
                                                int apiRequestId3 = (int)jsonResponse["apiRequestId"];
                                                DateTime dateTime3 = (DateTime)jsonResponse["dateTime"];

                                                JArray subjects = (JArray)jsonResponse["subjects"];

                                                if (subjects != null)
                                                {
                                                    foreach (var subject in subjects)
                                                    {
                                                        JObject subjectObject = (JObject)subject;
                                                        int subjectId3 = (int)subjectObject["subjectId"];
                                                        string subjectReference3 = (string)subjectObject["subjectReference"];
                                                        string subjectIdentifier = (string)subjectObject["subjectIdentifier"];
                                                        string subjectType = (string)subjectObject["subjectType"];
                                                        string subjectStatus = (string)subjectObject["subjectStatus"];
                                                        bool isMonitored = (bool)subjectObject["isMonitored"];
                                                        string monitoredRequestType = (string)subjectObject["monitoredRequestType"];
                                                        string riskRating = (string)subjectObject["riskRating"];

                                                    }
                                                }


                                                string request3 = url + "subjects?id=" + subjectId;
                                                try
                                                {
                                                    MySqlCommand _cmd = new MySqlCommand("SaveAPIRequestResponce");
                                                    _cmd.CommandType = CommandType.StoredProcedure;
                                                    _cmd.Parameters.AddWithValue("_API_ID", 10);
                                                    _cmd.Parameters.AddWithValue("_Client_ID", Client_ID);
                                                    _cmd.Parameters.AddWithValue("_Customer_ID", Customer_ID);
                                                    _cmd.Parameters.AddWithValue("_status", 0);
                                                    _cmd.Parameters.AddWithValue("_Function_name", CallFrom);
                                                    _cmd.Parameters.AddWithValue("_Remark", Remark);
                                                    _cmd.Parameters.AddWithValue("_comments", request3);
                                                    _cmd.Parameters.AddWithValue("_Record_Insert_DateTime", Record_Insert_DateTime);
                                                    _cmd.Parameters.AddWithValue("_Branch_ID", obj.Branch_ID);
                                                    int msg1 = db_connection.ExecuteNonQueryProcedure(_cmd);
                                                }
                                                catch (Exception ex)
                                                {
                                                    string error = ex.ToString().Replace("\'", "\\'");
                                                    MySqlCommand _cmd = new MySqlCommand("SaveException");
                                                    _cmd.CommandType = CommandType.StoredProcedure;
                                                    _cmd.Parameters.AddWithValue("_Record_Insert_DateTime", Record_Insert_DateTime);
                                                    _cmd.Parameters.AddWithValue("_error", error);
                                                    _cmd.Parameters.AddWithValue("_Client_ID", Client_ID);
                                                    int msg1 = db_connection.ExecuteNonQueryProcedure(_cmd);
                                                }

                                            }

                                            catch (Exception ex)
                                            {
                                                string stattus = (string)CompanyInfo.InsertErrorLogDetails(ex.Message.Replace("\'", "\\'"), 0, "Complience Assist AMl Check - 4th api call (Subject Information)  ", obj.Branch_ID, obj.Client_ID);

                                            }

                                            //string Remark = Convert.ToString(CompanyInfo.getAPIStatus(Bandtext, obj.Client_ID));

                                            try
                                            {
                                                MySqlCommand _cmd = new MySqlCommand("SaveAPIRequestResponce");
                                                _cmd.CommandType = CommandType.StoredProcedure;
                                                _cmd.Parameters.AddWithValue("_API_ID", API_ID);
                                                _cmd.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                                                _cmd.Parameters.AddWithValue("_Customer_ID", Customer_ID);
                                                _cmd.Parameters.AddWithValue("_status", 1);
                                                _cmd.Parameters.AddWithValue("_Function_name", CallFrom);
                                                _cmd.Parameters.AddWithValue("_Remark", Remark);
                                                _cmd.Parameters.AddWithValue("_comments", jsonResponse);
                                                _cmd.Parameters.AddWithValue("_Record_Insert_DateTime", Record_Insert_DateTime);
                                                _cmd.Parameters.AddWithValue("_Branch_ID", obj.Branch_ID);
                                                int msg1 = db_connection.ExecuteNonQueryProcedure(_cmd);
                                            }
                                            catch (Exception ex)
                                            {
                                                string error = ex.ToString().Replace("\'", "\\'");
                                                MySqlCommand _cmd = new MySqlCommand("SaveException");
                                                _cmd.CommandType = CommandType.StoredProcedure;
                                                _cmd.Parameters.AddWithValue("_Record_Insert_DateTime", Record_Insert_DateTime);
                                                _cmd.Parameters.AddWithValue("_error", error);
                                                _cmd.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                                                int msg1 = db_connection.ExecuteNonQueryProcedure(_cmd);
                                            }


                                        }

                                    }
                                    catch (Exception ex)

                                    {
                                        //send complaince alert
                                        string Record_DateTime_exception = Record_Insert_DateTime;
                                        string notification_icon_exception = "aml-referd.jpg";
                                        string notification_message_exception = "<span class='cls-admin'>Complience AML check <strong class='cls-cancel'> Request failed.</strong></span><span class='cls-customer'></span>";
                                        CompanyInfo.save_notification_compliance(notification_message_exception, notification_icon_exception, Convert.ToString(Customer_ID), Convert.ToDateTime(Record_DateTime_exception), obj.Client_ID, 1, obj.User_ID, obj.Branch_ID, 0, 1, 1, 0, context);


                                        try
                                        {
                                            mail_send = string.Empty;
                                            string sendmsg = "Complience AML check Request failed. Please contact Complience support team. ";
                                            string EmailID = "";
                                            DataTable dt_admin_Email_list = (DataTable)getAdminEmailList(obj.Client_ID, obj.Branch_ID);
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

                                            subject = "" + Company_Name + " - Compliance Alert - " + WireTransfer_ReferanceNo;
                                            mail_send = (string)CompanyInfo.Send_Mail(dt_admin_Email_list, email, body, subject, Convert.ToInt32(obj.Client_ID), Convert.ToInt32(obj.Branch_ID), "", "", "", context);

                                        }
                                        catch { }

                                    }






                                    if (flag > 0)
                                    {
                                        if (obj.Branch_ID == 0)
                                        {
                                            obj.Branch_ID = 2;
                                        }
                                        using (MySqlCommand cmd = new MySqlCommand("update_sanction_flag"))//, cn
                                        {
                                            //if (cn.State != ConnectionState.Open) { cn.Open(); }
                                            cmd.CommandType = CommandType.StoredProcedure;
                                            cmd.Parameters.AddWithValue("_Customer_ID", Customer_ID);
                                            cmd.Parameters.AddWithValue("_record_date", Record_Insert_DateTime);
                                            cmd.Parameters.AddWithValue("_clientId", 1);
                                            cmd.Parameters.AddWithValue("_branchId", obj.Branch_ID);
                                            cmd.Parameters.AddWithValue("_sanction_flag", flag);
                                            int n = db_connection.ExecuteNonQueryProcedure(cmd);

                                        }

                                        using (MySqlCommand cmd = new MySqlCommand("Insert_pep_sanc_detail"))
                                        {

                                            cmd.CommandType = CommandType.StoredProcedure;
                                            cmd.Parameters.AddWithValue("p_SenderID_ID", SenderID_ID);
                                            cmd.Parameters.AddWithValue("p_Flag", 1);
                                            cmd.Parameters.AddWithValue("p_Record_Insert_DateTime", Record_Insert_DateTime.ToString());
                                            cmd.Parameters.AddWithValue("p_Customer_ID", Customer_ID);
                                            cmd.Parameters.AddWithValue("p_API_ID", 10);
                                            int n = db_connection.ExecuteNonQueryProcedure(cmd);

                                        }


                                        try
                                        {
                                            MySqlCommand cmd_update = new MySqlCommand("update documents_details set aml_sanctions_flag=1 where SenderID_ID=@SenderID_ID");
                                            cmd_update.Parameters.AddWithValue("@SenderID_ID", SenderID_ID);
                                            db_connection.ExecuteNonQueryProcedure(cmd_update);
                                        }
                                        catch
                                        {

                                        }

                                    }
                                    if (flag == 0)
                                    {
                                        if (obj.Branch_ID == 0)
                                        {
                                            obj.Branch_ID = 2;
                                        }
                                        using (MySqlCommand cmd = new MySqlCommand("update_sanction_flag"))
                                        {

                                            cmd.CommandType = CommandType.StoredProcedure;
                                            cmd.Parameters.AddWithValue("_Customer_ID", Customer_ID);
                                            cmd.Parameters.AddWithValue("_record_date", Record_Insert_DateTime);
                                            cmd.Parameters.AddWithValue("_clientId", 1);
                                            cmd.Parameters.AddWithValue("_branchId", obj.Branch_ID);
                                            cmd.Parameters.AddWithValue("_sanction_flag", 0);
                                            int n = db_connection.ExecuteNonQueryProcedure(cmd);

                                        }

                                        using (MySqlCommand cmd = new MySqlCommand("Insert_pep_sanc_detail"))
                                        {

                                            cmd.CommandType = CommandType.StoredProcedure;
                                            cmd.Parameters.AddWithValue("p_SenderID_ID", SenderID_ID);
                                            cmd.Parameters.AddWithValue("p_Flag", 0);
                                            cmd.Parameters.AddWithValue("p_Record_Insert_DateTime", Record_Insert_DateTime.ToString());
                                            cmd.Parameters.AddWithValue("p_Customer_ID", Customer_ID);
                                            cmd.Parameters.AddWithValue("p_API_ID", 10);
                                            int n = db_connection.ExecuteNonQueryProcedure(cmd);

                                        }


                                    }




                                    if (requestStatus != "" && requestStatus != null)
                                    {

                                        if (matchStatusesArr != null)
                                        {

                                            foreach (JObject matchStatusObj in matchStatusesArr)
                                            {
                                                string matchStatus1 = (string)matchStatusObj["matchStatus"];
                                                int numberOfMatches1 = (int)matchStatusObj["numberOfMatches"];

                                                // Extract nested properties under "matchTypes"
                                                JObject matchTypesObj1 = (JObject)matchStatusObj["matchTypes"];
                                                bool isSan = (bool)matchTypesObj1["san"];
                                                bool isAdv = (bool)matchTypesObj1["adv"];
                                                bool isPep = (bool)matchTypesObj1["pep"];
                                                bool isRca = (bool)matchTypesObj1["rca"];
                                                bool isSoc = (bool)matchTypesObj1["soc"];
                                                bool isOther = (bool)matchTypesObj1["other"];
                                                if (isPep == true)
                                                {


                                                    string description1 = "Customer Found In Pep";
                                                    string Record_DateTime = Record_Insert_DateTime;
                                                    string notification_icon = "aml-referd.jpg";
                                                    //string notification_message = "<span class='cls-admin'> International PEP Alert - <strong class='cls-cancel'></strong><br/>" + description1 + "</span><span class='cls-customer'></span>";
                                                    string notification_message = "<span class='cls-admin'> - <span class='cls-cancel'><strong> International PEP Alert </strong>- </span><span class='cls-addr-confirm'>" + description1 + " </span></span></span><span class='cls-customer'></span>";
                                                    CompanyInfo.save_notification_compliance(notification_message, notification_icon, Convert.ToString(Customer_ID), Convert.ToDateTime(Record_DateTime), obj.Client_ID, 1, obj.User_ID, obj.Branch_ID, 0, 1, 1, 0, context);





                                                }
                                                if (isSan == true)
                                                {



                                                    string description1 = "Customer Found In Sanctions";
                                                    string Record_DateTime = Record_Insert_DateTime;
                                                    string notification_icon = "pep-match-not-found.jpg";
                                                    //notification_message = "<span class='cls-admin'> - <span class='cls-cancel'><strong> International Sanctions Alert  </strong>- </span><span class='cls-addr-confirm'>" + description1 + " </span></span></span><span class='cls-customer'></span>";
                                                    string notification_message = "<span class='cls-admin'> - <span class='cls-cancel'><strong> International Sanctions Alert </strong>- </span><span class='cls-addr-confirm'>" + description1 + " </span></span></span><span class='cls-customer'></span>";
                                                    CompanyInfo.save_notification(notification_message, notification_icon, Convert.ToInt32(Customer_ID), Convert.ToDateTime(Record_Insert_DateTime), Convert.ToInt32(obj.Client_ID), 1, obj.User_ID, Convert.ToInt32(obj.Branch_ID), 0, 0, 1, 0, context);




                                                }
                                                if (isAdv == true)
                                                {



                                                    string description1 = "Customer Found In Adverse media matchess";
                                                    string Record_DateTime = Record_Insert_DateTime;
                                                    string notification_icon = "pep-match-not-found.jpg";
                                                    //notification_message = "<span class='cls-admin'> - <span class='cls-cancel'><strong> International Sanctions Alert  </strong>- </span><span class='cls-addr-confirm'>" + description1 + " </span></span></span><span class='cls-customer'></span>";
                                                    string notification_message = "<span class='cls-admin'> - <span class='cls-cancel'><strong> Adverse media matches Alert </strong>- </span><span class='cls-addr-confirm'>" + description1 + " </span></span></span><span class='cls-customer'></span>";
                                                    CompanyInfo.save_notification(notification_message, notification_icon, Convert.ToInt32(Customer_ID), Convert.ToDateTime(Record_Insert_DateTime), Convert.ToInt32(obj.Client_ID), 1, obj.User_ID, Convert.ToInt32(obj.Branch_ID), 0, 0, 1, 0, context);




                                                }
                                                if (isRca == true)
                                                {



                                                    string description1 = "Customer Found In RCA (relatives or close associates) matches";
                                                    string Record_DateTime = Record_Insert_DateTime;
                                                    string notification_icon = "pep-match-not-found.jpg";
                                                    //notification_message = "<span class='cls-admin'> - <span class='cls-cancel'><strong> International Sanctions Alert  </strong>- </span><span class='cls-addr-confirm'>" + description1 + " </span></span></span><span class='cls-customer'></span>";
                                                    string notification_message = "<span class='cls-admin'> - <span class='cls-cancel'><strong> RCA (relatives or close associates) matches Alert </strong>- </span><span class='cls-addr-confirm'>" + description1 + " </span></span></span><span class='cls-customer'></span>";
                                                    CompanyInfo.save_notification(notification_message, notification_icon, Convert.ToInt32(Customer_ID), Convert.ToDateTime(Record_Insert_DateTime), Convert.ToInt32(obj.Client_ID), 1, obj.User_ID, Convert.ToInt32(obj.Branch_ID), 0, 0, 1, 0, context);




                                                }
                                                if (isSoc == true)
                                                {



                                                    string description1 = "Customer Found In SOC (state owned companies) matches";
                                                    string Record_DateTime = Record_Insert_DateTime;
                                                    string notification_icon = "pep-match-not-found.jpg";
                                                    //notification_message = "<span class='cls-admin'> - <span class='cls-cancel'><strong> International Sanctions Alert  </strong>- </span><span class='cls-addr-confirm'>" + description1 + " </span></span></span><span class='cls-customer'></span>";
                                                    string notification_message = "<span class='cls-admin'> - <span class='cls-cancel'><strong> SOC (state owned companies) matches Alert </strong>- </span><span class='cls-addr-confirm'>" + description1 + " </span></span></span><span class='cls-customer'></span>";
                                                    CompanyInfo.save_notification(notification_message, notification_icon, Convert.ToInt32(Customer_ID), Convert.ToDateTime(Record_Insert_DateTime), Convert.ToInt32(obj.Client_ID), 1, obj.User_ID, Convert.ToInt32(obj.Branch_ID), 0, 0, 1, 0, context);




                                                }
                                                if (isOther == true)
                                                {



                                                    string description1 = "Cutomer Found in others";
                                                    string Record_DateTime = Record_Insert_DateTime;
                                                    string notification_icon = "pep-match-not-found.jpg";
                                                    //notification_message = "<span class='cls-admin'> - <span class='cls-cancel'><strong> International Sanctions Alert  </strong>- </span><span class='cls-addr-confirm'>" + description1 + " </span></span></span><span class='cls-customer'></span>";
                                                    string notification_message = "<span class='cls-admin'> - <span class='cls-cancel'><strong> International AML Alert </strong>- </span><span class='cls-addr-confirm'>" + description1 + " </span></span></span><span class='cls-customer'></span>";
                                                    CompanyInfo.save_notification(notification_message, notification_icon, Convert.ToInt32(Customer_ID), Convert.ToDateTime(Record_Insert_DateTime), Convert.ToInt32(obj.Client_ID), 1, obj.User_ID, Convert.ToInt32(obj.Branch_ID), 0, 0, 1, 0, context);




                                                }

                                                //string ssa = childd.InnerXml;
                                                //string Address_match = childd.InnerText.ToString();






                                            }
                                        }

                                        if (flag1 > 0 && flag2 > 0)
                                        {
                                            try
                                            {
                                                MySqlCommand cmd_update = new MySqlCommand("update documents_details set aml_sanctions_flag=3 where SenderID_ID=@SenderID_ID");
                                                cmd_update.Parameters.AddWithValue("@SenderID_ID", SenderID_ID);
                                                db_connection.ExecuteNonQueryProcedure(cmd_update);

                                                using (MySqlCommand cmd = new MySqlCommand("update_sanction_flag"))//, cn
                                                {
                                                    //if (cn.State != ConnectionState.Open) { cn.Open(); }
                                                    cmd.CommandType = CommandType.StoredProcedure;
                                                    cmd.Parameters.AddWithValue("_Customer_ID", Customer_ID);
                                                    cmd.Parameters.AddWithValue("_record_date", Record_Insert_DateTime);
                                                    cmd.Parameters.AddWithValue("_clientId", obj.Client_ID);
                                                    cmd.Parameters.AddWithValue("_branchId", obj.Branch_ID);
                                                    cmd.Parameters.AddWithValue("_sanction_flag", flag1);
                                                    int n = db_connection.ExecuteNonQueryProcedure(cmd);

                                                }
                                            }
                                            catch { }
                                        }
                                        else if (flag1 > 0)
                                        {
                                            try
                                            {
                                                MySqlCommand cmd_update = new MySqlCommand("update documents_details set aml_sanctions_flag=1 where SenderID_ID=@SenderID_ID");
                                                cmd_update.Parameters.AddWithValue("@SenderID_ID", SenderID_ID);
                                                db_connection.ExecuteNonQueryProcedure(cmd_update);

                                                using (MySqlCommand cmd = new MySqlCommand("update_sanction_flag"))//, cn
                                                {
                                                    //if (cn.State != ConnectionState.Open) { cn.Open(); }
                                                    cmd.CommandType = CommandType.StoredProcedure;
                                                    cmd.Parameters.AddWithValue("_Customer_ID", obj.Customer_ID);
                                                    cmd.Parameters.AddWithValue("_record_date", obj.Record_Insert_DateTime);
                                                    cmd.Parameters.AddWithValue("_clientId", obj.Client_ID);
                                                    cmd.Parameters.AddWithValue("_branchId", obj.Branch_ID);
                                                    cmd.Parameters.AddWithValue("_sanction_flag", flag1);
                                                    int n = db_connection.ExecuteNonQueryProcedure(cmd);

                                                }

                                            }
                                            catch { }
                                        }
                                        else if (flag2 > 0)
                                        {
                                            try
                                            {
                                                MySqlCommand cmd_update = new MySqlCommand("update documents_details set aml_sanctions_flag=2 where SenderID_ID=@SenderID_ID");
                                                cmd_update.Parameters.AddWithValue("@SenderID_ID", SenderID_ID);
                                                db_connection.ExecuteNonQueryProcedure(cmd_update);


                                                using (MySqlCommand cmd = new MySqlCommand("update_sanction_flag"))//, cn
                                                {
                                                    //if (cn.State != ConnectionState.Open) { cn.Open(); }
                                                    cmd.CommandType = CommandType.StoredProcedure;
                                                    cmd.Parameters.AddWithValue("_Customer_ID", Customer_ID);
                                                    cmd.Parameters.AddWithValue("_record_date", Record_Insert_DateTime);
                                                    cmd.Parameters.AddWithValue("_clientId", obj.Client_ID);
                                                    cmd.Parameters.AddWithValue("_branchId", obj.Branch_ID);
                                                    cmd.Parameters.AddWithValue("_sanction_flag", flag2);
                                                    int n = db_connection.ExecuteNonQueryProcedure(cmd);

                                                }
                                            }
                                            catch { }
                                        }
                                        else
                                if (requestStatus == "Complete" && flag == 0)
                                        {
                                            string Record_DateTime = Record_Insert_DateTime;
                                            string notification_icon = "primary-id-upload.jpg";
                                            string notification_message = "<span class='cls-admin'>Complience AML check result is <strong class='cls-priamary'>" + requestStatus + ".</strong></span><span class='cls-customer'></span>";
                                            CompanyInfo.save_notification(notification_message, notification_icon, Convert.ToInt32(Customer_ID), Convert.ToDateTime(Record_DateTime), obj.Client_ID, 1, obj.User_ID, obj.Branch_ID, 0, 1, 1, 0, context);

                                        }

                                        try
                                        {
                                            //string Remark = Convert.ToString(CompanyInfo.getAPIStatus(Bandtext, obj.Client_ID));
                                            if (Remark == "2" || Remark == "1")//alert or refer then  send mail to admin
                                            {
                                                string Record_DateTime = DateTime.Now.ToString();
                                                string notification_icon = "aml-referd.jpg";
                                                string notification_message = "<span class='cls-admin'>Complience AML check result is <strong class='cls-cancel'>" + Bandtext + ".</strong></span><span class='cls-customer'></span>";
                                                CompanyInfo.save_notification_compliance(notification_message, notification_icon, Convert.ToString(Customer_ID), Convert.ToDateTime(Record_DateTime), obj.Client_ID, 1, obj.User_ID, obj.Branch_ID, 0, 1, 1, 0, context);
                                            }
                                            else
                                            {
                                                string Record_DateTime = Record_Insert_DateTime;
                                                string notification_icon = "primary-id-upload.jpg";
                                                string notification_message = "<span class='cls-admin'>Complience AML check result is <strong class='cls-priamary'>" + Bandtext + ".</strong></span><span class='cls-customer'></span>";
                                                CompanyInfo.save_notification_compliance(notification_message, notification_icon, Convert.ToString(Customer_ID), Convert.ToDateTime(Record_DateTime), obj.Client_ID, 1, obj.User_ID, obj.Branch_ID, 0, 1, 1, 0, context);
                                            }

                                        }
                                        catch (Exception e)
                                        {
                                        }
                                        try
                                        {
                                            //string Remark = Convert.ToString(CompanyInfo.getAPIStatus(Bandtext, obj.Client_ID));
                                            if (Remark == "1" || Remark == "2" || Remark == "3")//alert or refer then  send mail to admin
                                            {

                                                //DataTable ds1 = CompanyInfo.get(Client_ID, context);
                                                if (ds1.Rows.Count > 0)
                                                {
                                                    BaseCurrency_Code = Convert.ToString(ds1.Rows[0]["BaseCurrency_Code"]);
                                                    BaseCurrency_TimeZone = Convert.ToString(ds1.Rows[0]["BaseCurrency_TimeZone"]);
                                                    BaseCurrency_Sign = Convert.ToString(ds1.Rows[0]["BaseCurrency_Sign"]);
                                                    BaseCurrency_Country = Convert.ToString(ds1.Rows[0]["BaseCurrency_Country"]);
                                                    BaseCountry_ID = Convert.ToString(ds1.Rows[0]["BaseCountry_ID"]);
                                                    EmailUrl = Convert.ToString(ds1.Rows[0]["Company_URL_Admin"]);
                                                    CustomerURL = Convert.ToString(ds1.Rows[0]["Company_URL_Customer"]);
                                                    Company_Name = Convert.ToString(ds1.Rows[0]["Company_Name"]);
                                                    Cancel_Transaction_Hours = Convert.ToString(ds1.Rows[0]["Cancel_Transaction_Hours"]);
                                                }


                                                MySqlCommand _cmd = new MySqlCommand("customer_details_by_param");
                                                _cmd.CommandType = CommandType.StoredProcedure;
                                                string _whereclause = " and cr.Client_ID=" + obj.Client_ID + " and cr.Customer_ID=" + Customer_ID;
                                                //  DataTable d1 = (DataTable)mtsmethods.GetCustDetailsByID(c.Customer_ID); //get customer details by id

                                                _cmd.Parameters.AddWithValue("_whereclause", _whereclause);
                                                _cmd.Parameters.AddWithValue("_SecurityKey", CompanyInfo.SecurityKey());
                                                DataTable d1 = db_connection.ExecuteQueryDataTableProcedure(_cmd);
                                                string sendmsg = "Response from third Party AML check for  " + d1.Rows[0]["WireTransfer_ReferanceNo"] + "   is   " + Bandtext + "";




                                                string EmailID = "";
                                                DataTable dt_admin_Email_list = (DataTable)getAdminEmailList(obj.Client_ID, obj.Branch_ID);
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

                                                subject = "" + Company_Name + " - Compliance - Alert - " + d1.Rows[0]["WireTransfer_ReferanceNo"];
                                                //subject = "" + HttpContext.Current.Session["Company_Name"] + " -  Incomplete Customer Registration Details " + c.WireTransfer_ReferanceNo;
                                                mail_send = (string)CompanyInfo.Send_Mail(dt_admin_Email_list, email, body, subject, Convert.ToInt32(obj.Client_ID), Convert.ToInt32(obj.Branch_ID), "Alert Admins", "", "", context);
                                            }
                                        }
                                        catch (Exception ae)
                                        {
                                            string stattus = (string)CompanyInfo.InsertErrorLogDetails(ae.Message.Replace("\'", "\\'"), 0, "Check Credit Safe", obj.Branch_ID, obj.Client_ID);
                                        }
                                    }
                                }
                                else
                                {
                                    stattus12 = (int)CompanyInfo.InsertActivityLogDetails("Records Not Found in ds datatable" + whereclause + Customer_ID + Client_ID, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(Customer_ID), "GetJourney", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "ID Upload", context);
                                }


                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    mail_send = string.Empty;
                    string stattus = (string)CompanyInfo.InsertErrorLogDetails(ex.ToString().Replace("\'", "\\'"), 0, "Check_AML", obj.Branch_ID, obj.Client_ID);
                    string sendmsg = "Customer Registered successfully but we are unable to check Credit Safe";
                    string EmailID = "";
                    DataTable dt_admin_Email_list = (DataTable)getAdminEmailList(obj.Client_ID, obj.Branch_ID);
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

                    if (Convert.ToString(EmailUrl) == "" || Convert.ToString(EmailUrl) == null)
                    {

                        MySqlCommand _cmd = new MySqlCommand("customer_details_by_param");
                        _cmd.CommandType = CommandType.StoredProcedure;
                        string _whereclause = " and cr.Client_ID=" + obj.Client_ID + " and cr.Customer_ID=" + Customer_ID;
                        //  DataTable d1 = (DataTable)mtsmethods.GetCustDetailsByID(c.Customer_ID); //get customer details by id

                        _cmd.Parameters.AddWithValue("_whereclause", _whereclause);
                        _cmd.Parameters.AddWithValue("_SecurityKey", CompanyInfo.SecurityKey());
                        DataTable dss = db_connection.ExecuteQueryDataTableProcedure(_cmd);

                        if (dss.Rows.Count > 0)
                        {
                            BaseCurrency_Code = Convert.ToString(dss.Rows[0]["BaseCurrency_Code"]);
                            BaseCurrency_TimeZone = Convert.ToString(dss.Rows[0]["BaseCurrency_TimeZone"]);
                            BaseCurrency_Sign = Convert.ToString(dss.Rows[0]["BaseCurrency_Sign"]);
                            BaseCurrency_Country = Convert.ToString(dss.Rows[0]["BaseCurrency_Country"]);
                            BaseCountry_ID = Convert.ToString(dss.Rows[0]["BaseCountry_ID"]);
                            EmailUrl = Convert.ToString(dss.Rows[0]["Company_URL_Admin"]);
                            CustomerURL = Convert.ToString(dss.Rows[0]["Company_URL_Customer"]);
                            RootURL = Convert.ToString(dss.Rows[0]["RootURL"]); //"C:/inetpub/wwwroot/bureaudechangesoftware-LIVE/MTS/MTS/";//"C:/inetpub/wwwroot/mts-teeparam-admin/";
                        }
                    }



                    httpRequest = (HttpWebRequest)WebRequest.Create("" + EmailUrl + "Email/customemail.html");

                    httpRequest.UserAgent = "Code Sample Web Client";
                    HttpWebResponse webResponse = (HttpWebResponse)httpRequest.GetResponse();
                    using (StreamReader reader = new StreamReader(webResponse.GetResponseStream()))
                    {
                        body = reader.ReadToEnd();
                    }
                    body = body.Replace("[name]", "Administrators");

                    body = body.Replace("[msg]", sendmsg);
                    subject = "" + Company_Name + " -  Incomplete Customer Registration Details.";
                    //HttpWebRequest httpRequest1 = (HttpWebRequest)WebRequest.Create("" + HttpContext.Current.Session["EmailUrl"] + "Email/RegisteredSuccessMail.txt");

                    //httpRequest1.UserAgent = "Code Sample Web Client";
                    //HttpWebResponse webResponse1 = (HttpWebResponse)httpRequest1.GetResponse();
                    //using (StreamReader reader = new StreamReader(webResponse1.GetResponseStream()))
                    //{
                    //    subject = reader.ReadLine();
                    //}
                    mail_send = (string)CompanyInfo.Send_Mail(dt_admin_Email_list, email, body, subject, Convert.ToInt32(obj.Client_ID), Convert.ToInt32(obj.Branch_ID), "Alert Admins", "", "", context);
                    stattus = (string)CompanyInfo.InsertErrorLogDetails(ex.Message.Replace("\'", "\\'"), 0, "Check_AML", obj.Branch_ID, obj.Client_ID);
                }
            }
            catch (Exception ex)
            {
                string stattus = (string)CompanyInfo.InsertErrorLogDetails("Error In Check_AML funcrtion" + ex.Message.Replace("\'", "\\'"), 0, "Check_AML", obj.Branch_ID, obj.Client_ID);
            }

            string[] response = { Status.ToString(), Bandtext };
            return response;
        }

        public static object getAdminEmailList(int Client_ID, int Branch_ID)
        {
            DataTable dt = new DataTable();
            try
            {
                MySqlCommand cmd = new MySqlCommand("GetAdmins");
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

        public string[] CHECK_AML(Model.Document obj, int Customer_ID, string CallFrom, HttpContext context)  // Complience  Assist AMl Check Rushikesh.
        {
            int Status = 1;
            string Bandtext = "";
            int Client_ID = obj.Client_ID;
            string BaseCurrency_Code = string.Empty;
            string BaseCurrency_TimeZone = string.Empty;
            string BaseCurrency_Sign = string.Empty;
            string Cancel_Transaction_Hours = string.Empty;
            string Company_Name = string.Empty;
            string CustomerURL = string.Empty;
            string EmailUrl = string.Empty;
            string BaseCountry_ID = string.Empty;
            string BaseCurrency_Country = string.Empty;
            string RootURL = string.Empty;
            DataTable ds1 = CompanyInfo.get(Client_ID, context);
            int stattus12 = (int)CompanyInfo.InsertActivityLogDetails("Inside CHECK_AML" + Customer_ID, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(Customer_ID), "GetJourney", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "ID Upload", context);
            try
            {

                string mail_send = string.Empty;

                string RefNum = "";
                string Addressline_2 = "";
                string Gender = "";
                string formattedDateOfBirth = "";
                int flag1 = 0, flag2 = 0, flag4 = 0, flag5 = 0, flag6 = 0, flag7 = 0;
                int flag = 0;
                string requestStatus = "";
                string status = "";
                int apiRequestId = 0;
                DateTime dateTime = new DateTime();
                long requestId = 0;
                string requestReference = "";
                long subjectId = 0;
                string subjectReference = "";
                JArray matchStatusesArr = new JArray();
                string First_Name = "";
                string Middle_Name = "";
                string Last_Name = "";
                string Country = "";
                string House_Number = "";
                string Street = "";
                string City = "";
                string Post_Code = "";
                string Record_Insert_DateTime = "";
                string SenderID_ExpiryDate = "";
                string WireTransfer_ReferanceNo = "";
                DateTime dateOfBirth = new DateTime();
                string SenderID_Number = "";
                int IDName_ID = 0;
                int SenderID_ID = 0;
                string monitorCommand = "";




                string Remark = "";
                try
                {
                    MySqlCommand cmdupdate1 = new MySqlCommand("Get_Permissions");
                    cmdupdate1.CommandType = CommandType.StoredProcedure;
                    cmdupdate1.Parameters.AddWithValue("Per_ID", 32);
                    cmdupdate1.Parameters.AddWithValue("ClientID", obj.Client_ID);
                    DataTable pm = db_connection.ExecuteQueryDataTableProcedure(cmdupdate1);
                    if (pm.Rows.Count > 0)
                    {
                        if (Convert.ToString(pm.Rows[0]["Status_ForCustomer"]) == "0")
                        {

                            obj.Customer_ID = Customer_ID.ToString();
                            int API_ID = 10;


                            MySqlCommand cmdp_active = new MySqlCommand("active_thirdparti_aml_api");
                            cmdp_active.CommandType = CommandType.StoredProcedure;
                            string whereclause = "API_ID =" + 10;
                            cmdp_active.Parameters.AddWithValue("_whereclause", whereclause);

                            DataTable dtApi = db_connection.ExecuteQueryDataTableProcedure(cmdp_active);


                            if (dtApi.Rows.Count > 0)
                            {
                                stattus12 = (int)CompanyInfo.InsertActivityLogDetails("Inside active_thirdparti_aml_api" + Customer_ID, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(Customer_ID), "GetJourney", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "ID Upload", context);
                                string StateDistrict = "";
                                //get Customer details
                                MySqlCommand getcust = new MySqlCommand("GetCustomer_all_details"); //  chnages by rushikesh
                                getcust.CommandType = CommandType.StoredProcedure;
                                whereclause = "and cr.Customer_ID=" + Customer_ID + " and cr.Client_ID=" + Client_ID;
                                getcust.Parameters.AddWithValue("_whereclause", whereclause);

                                DataTable dc = db_connection.ExecuteQueryDataTableProcedure(getcust);
                                if (dc.Rows.Count > 0)
                                {
                                    try
                                    {
                                        stattus12 = (int)CompanyInfo.InsertActivityLogDetails("Inside GetCustomer_all_details" + whereclause, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(Customer_ID), "GetJourney", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "ID Upload", context);
                                        WireTransfer_ReferanceNo = Convert.ToString(dc.Rows[0]["WireTransfer_ReferanceNo"]);
                                        First_Name = Convert.ToString(dc.Rows[0]["First_Name"]).Replace("&", "AND");
                                        Middle_Name = Convert.ToString(dc.Rows[0]["Middle_Name"]).Replace("&", "AND");
                                        Last_Name = Convert.ToString(dc.Rows[0]["Last_Name"]);
                                        Country = Convert.ToString(dc.Rows[0]["Country_Name"]).ToLower();
                                        House_Number = Convert.ToString(dc.Rows[0]["House_Number"]).Replace("&", "AND");
                                        Street = Convert.ToString(dc.Rows[0]["Street"]).Replace("&", "AND");
                                        City = Convert.ToString(dc.Rows[0]["City_Name"]).Replace("&", "AND");
                                        Post_Code = Convert.ToString(dc.Rows[0]["Post_Code"]).Replace("&", "AND");
                                        Record_Insert_DateTime = CompanyInfo.gettime(Client_ID, context);
                                        //SenderID_ExpiryDate = Convert.ToString(dc.Rows[0]["SenderID_ExpiryDate"]);
                                        dateOfBirth = Convert.ToDateTime(dc.Rows[0]["DateOf_Birth"]);
                                        try { formattedDateOfBirth = dateOfBirth.ToString("yyyy-MM-dd"); } catch { }
                                        //SenderID_Number = Convert.ToString(dc.Rows[0]["SenderID_Number"]);
                                        //IDName_ID = Convert.ToInt32(dc.Rows[0]["IDName_ID"]);
                                        SenderID_ID = Convert.ToInt32(dc.Rows[0]["SenderID_ID"]);
                                        //StateDistrict = Convert.ToString(dc.Rows[0]["code"]);
                                        RefNum = Convert.ToString(dc.Rows[0]["WireTransfer_ReferanceNo"]);
                                        Addressline_2 = Convert.ToString(dc.Rows[0]["Addressline_2"]);
                                        Gender = Convert.ToString(dc.Rows[0]["Gender"]).ToUpper();
                                    }
                                    catch (Exception ex)
                                    {
                                        string stattus = (string)CompanyInfo.InsertErrorLogDetails("Error in accessing values from GetCustomer_all_details" + ex.ToString().Replace("\'", "\\'"), 0, "CHECK_AML", obj.Branch_ID, obj.Client_ID);
                                    }


                                    var _url = ""; //Convert.ToString(dtApi.Rows[0]["API_URL"]);
                                    var UserName = "";//Convert.ToString(dtApi.Rows[0]["UserName"]);
                                    var Password = "";// Convert.ToString(dtApi.Rows[0]["Password"]);
                                                      //var ProfileID = Convert.ToString(dtApi.Rows[0]["ProfileID"]);
                                    var _action = "http://www.id3global.com/ID3gWS/2013/04/IGlobalAuthenticate/AuthenticateSP";
                                    var ProfileID = "";
                                    var tokenurl = "";
                                    var Encryption_Key = "";
                                    var Request_Type_Code_Benef = "";
                                    var Request_Type_Code_Cust = "";
                                    string status3 = "";
                                    if (dtApi.Rows.Count > 0)
                                    {
                                        _url = Convert.ToString(dtApi.Rows[0]["API_URL"]);
                                        UserName = Convert.ToString(dtApi.Rows[0]["UserName"]);
                                        Password = Convert.ToString(dtApi.Rows[0]["Password"]);

                                        ProfileID = Convert.ToString(dtApi.Rows[0]["ProfileID"]);
                                        if (ProfileID != "" && ProfileID != null)
                                        {
                                            Newtonsoft.Json.Linq.JObject objj = Newtonsoft.Json.Linq.JObject.Parse(ProfileID);
                                            tokenurl = Convert.ToString(objj["tokenurl"]);
                                            Encryption_Key = Convert.ToString(objj["Encryption_Key"]);
                                            Request_Type_Code_Benef = Convert.ToString(objj["Request_Type_Code_Benef"]);
                                            Request_Type_Code_Cust = Convert.ToString(objj["Request_Type_Code_Cust"]);

                                        }
                                    }
                                    XmlDocument soapEnvelopeXml = new XmlDocument();
                                    var iddoc = "";

                                    string passdoc = "", passid = "", endid = "";

                                    if (Country == "united kingdom" || Country == "uk") { Country = "UK"; }
                                    else if (Country == "new zealand") { Country = "NewZealand"; }
                                    else if (Country == "us" || Country == "united states") { Country = "US"; }
                                    else if (Country == "china" || Country == "india" || Country == "canada" || Country == "mexico" || Country == "brazil" || Country == "spain" || Country == "argentina")
                                    {
                                        Country = obj.country_code;
                                    }
                                    else { Country = ""; }
                                    string province = "";
                                    if (Country == "canada")
                                    {
                                        province = "<ns:StateDistrict>" + StateDistrict + @"</ns:StateDistrict>" +
                                                    "<ns:Region>" + StateDistrict + @"</ns:Region>";
                                    }

                                    string passexpiry = "";
                                    string expiry = Convert.ToString(SenderID_ExpiryDate);
                                    if (expiry != "" && expiry != null)
                                    {
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

                                    if (IDName_ID == 1)// if Passport
                                    {
                                        passdoc += "<ns:IdentityDocuments><ns:InternationalPassport>";
                                        passdoc += "<ns:Number>" + SenderID_Number + @"</ns:Number> " + passexpiry + "";
                                        if (Country != "") { passdoc += "<ns:CountryOfOrigin>" + Country + @"</ns:CountryOfOrigin>"; }
                                        passdoc += "</ns:InternationalPassport></ns:IdentityDocuments>";

                                    }
                                    else if (IDName_ID == 2 && Country != "")//if Driving licence
                                    {
                                        passdoc += "<ns:IdentityDocuments><ns:" + Country + @"><ns:DrivingLicence>";
                                        passdoc += "<ns:Number>" + SenderID_Number + @"</ns:Number> " + passexpiry + "";
                                        passdoc += "</ns:DrivingLicence></ns:" + Country + @"></ns:IdentityDocuments>";
                                    }
                                    else if (IDName_ID == 3)//EU Nationality Card
                                    {
                                        passdoc += "<ns:IdentityDocuments><ns:IdentityCard>";
                                        passdoc += "<ns:Number>" + SenderID_Number + @"</ns:Number>";
                                        if (Country != "") { passdoc += "<ns:Country>" + Country + @"</ns:Country>"; }
                                        passdoc += "</ns:IdentityCard></ns:IdentityDocuments>";
                                    }

                                    string passdob = "";

                                    string strmiddlename = "";
                                    if (Middle_Name != "" && Middle_Name != null)
                                    {
                                        strmiddlename = "<ns:MiddleName>" + Middle_Name + @"</ns:MiddleName>";
                                    }
                                    // Complience API CAll
                                    DataTable dtb = CompanyInfo.get(Client_ID, context);
                                    string rooturl = "";
                                    if (dtb.Rows.Count > 0)
                                    {
                                        //CURL = Convert.ToString(dtb.Rows[0]["RootURL"]);
                                        //Company_Name = Convert.ToString(dtb.Rows[0]["Company_Name"]);

                                        rooturl = Convert.ToString(dtb.Rows[0]["RootURL"]);
                                    }





                                    try
                                    {
                                        stattus12 = (int)CompanyInfo.InsertActivityLogDetails("befor tokenurl" + Customer_ID, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(Customer_ID), "GetJourney", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "ID Upload", context);


                                        string url = tokenurl;// "https://auth-u.complianceassist.co.uk";
                                        string clientId = UserName;///"4i37l17880j149sin1gpqid8r2";
                                        string clientSecret = Password;//"1lih0ev0bo7jvd2fl9emvl9k0b0dm2ulji7qe0naq9gf3g9mu3op";

                                        string[] scopes = {
                                 "uat/requesttypes.read",
                                 "uat/requests.read",
                                 "uat/requests.write",
                                 "uat/subjects.read",
                                 "uat/subjects.write"
                                 };

                                        string scope = string.Join(" ", scopes);





                                        // Other code remains the same
                                        var client = new RestClient(url + "/oauth2/token");
                                        client.Timeout = -1;
                                        var request = new RestRequest(Method.POST);
                                        string authHeaderValue = Convert.ToBase64String(Encoding.UTF8.GetBytes(clientId + ":" + clientSecret));
                                        request.AddHeader("Authorization", "Basic " + authHeaderValue);
                                        request.AddHeader("Accept", "application/json");
                                        request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                                        request.AddParameter("grant_type", "client_credentials");
                                        request.AddParameter("scope", scope);

                                        IRestResponse response1 = client.Execute(request);

                                        JObject jsonResponse = JObject.Parse(response1.Content);

                                        stattus12 = (int)CompanyInfo.InsertActivityLogDetails("after tokenurl" + response1.Content, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(Customer_ID), "GetJourney", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "ID Upload", context);


                                        string accessToken = jsonResponse["access_token"].ToString();



                                        // Assuming you have already obtained the accessToken variable from the previous step

                                        url = _url;//"https://web-u.complianceassist.co.uk/api/v2_0/";


                                        if (RefNum == "")
                                        {
                                            RefNum = WireTransfer_ReferanceNo;
                                            stattus12 = (int)CompanyInfo.InsertActivityLogDetails("Reference number =" + RefNum, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(Customer_ID), "GetJourney", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "ID Upload", context);
                                        }
                                        try
                                        {
                                            stattus12 = (int)CompanyInfo.InsertActivityLogDetails("before subjects?reference=", Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(Customer_ID), "GetJourney", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "ID Upload", context);

                                            client = new RestClient(url + "subjects?reference=" + RefNum + "&details=true");
                                            client.Timeout = -1;
                                            request = new RestRequest(Method.GET);
                                            request.AddHeader("Authorization", "Bearer " + accessToken);
                                            request.AddHeader("Accept", "application/json");

                                            response1 = client.Execute(request);


                                            // Parse the JSON response
                                            jsonResponse = JObject.Parse(response1.Content);
                                            stattus12 = (int)CompanyInfo.InsertActivityLogDetails("after subjects?reference=" + response1.Content, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(Customer_ID), "GetJourney", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "ID Upload", context);


                                            status3 = (string)jsonResponse["status"];
                                            int apiRequestId3 = (int)jsonResponse["apiRequestId"];
                                            DateTime dateTime3 = (DateTime)jsonResponse["dateTime"];

                                            JArray subjects = (JArray)jsonResponse["subjects"];

                                            if (subjects != null)
                                            {
                                                foreach (var subject in subjects)
                                                {
                                                    JObject subjectObject = (JObject)subject;
                                                    int subjectId3 = (int)subjectObject["subjectId"];
                                                    string subjectReference3 = (string)subjectObject["subjectReference"];
                                                    string subjectIdentifier = (string)subjectObject["subjectIdentifier"];
                                                    string subjectType = (string)subjectObject["subjectType"];
                                                    string subjectStatus = (string)subjectObject["subjectStatus"];
                                                    bool isMonitored = (bool)subjectObject["isMonitored"];
                                                    string monitoredRequestType = (string)subjectObject["monitoredRequestType"];
                                                    string riskRating = (string)subjectObject["riskRating"];
                                                    if (riskRating == "Low")
                                                    {
                                                        requestStatus = "Completed";
                                                        Remark = "0";
                                                    }
                                                    else
                                                    {
                                                        requestStatus = "Processing";
                                                        Remark = "1";
                                                    }

                                                }
                                            }
                                            if (status3 == "success")
                                            {

                                                stattus12 = (int)CompanyInfo.InsertActivityLogDetails("inside  monitorCommand = CHANGE", Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(Customer_ID), "GetJourney", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "ID Upload", context);

                                                monitorCommand = "CHANGE";

                                            }
                                            else
                                            {
                                                stattus12 = (int)CompanyInfo.InsertActivityLogDetails("inside  monitorCommand = ADD", Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(Customer_ID), "GetJourney", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "ID Upload", context);

                                                monitorCommand = "ADD";
                                            }

                                        }

                                        catch (Exception ex)
                                        {
                                            string stattus = (string)CompanyInfo.InsertErrorLogDetails(ex.Message.Replace("\'", "\\'"), 0, "Complience Assist AMl Check - 4th api call (Subject Information)  ", obj.Branch_ID, obj.Client_ID);

                                        }


                                        if (status3 != "success" || status3 == "success")
                                        {
                                            stattus12 = (int)CompanyInfo.InsertActivityLogDetails("befor body" + response1.Content, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(Customer_ID), "GetJourney", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "ID Upload", context);

                                            string bodyJson = @"
{
    ""requestType"": """ + Request_Type_Code_Cust + @""",
    ""subjectType"": ""INDIVIDUAL"",
    ""subjectReference"": """ + RefNum + @""",
    ""requestReference"": ""Onboarding " + RefNum + @""",
    ""subjectDetails"": {
        ""firstName"": """ + First_Name + @""",
        ""surname"": """ + Last_Name + @""",
        ""dateOfBirth"": """ + formattedDateOfBirth + @""",
        ""gender"": """ + Gender + @""",
        ""houseNumber"": """ + House_Number + @""",
        ""addressLine1"":""" + Addressline_2 + @""",
        ""addressLine2"": """ + Addressline_2 + @""",
        ""city"":""" + City + @""",
        ""postcode"": """ + Post_Code + @""",
        ""country"": """ + Country + @"""
    },
    ""notes"": ""Onboarded under project Alpha."",
    ""monitorCommand"": """ + monitorCommand + @""",
    ""callbackUrl"": """"
}
";

                                            try
                                            {

                                                client = new RestClient(url + "requests");
                                                client.Timeout = -1;
                                                request = new RestRequest(Method.POST);
                                                request.AddHeader("Authorization", "Bearer " + accessToken);
                                                request.AddHeader("Accept", "application/json");
                                                request.AddHeader("Content-Type", "application/json");
                                                request.AddParameter("application/json", bodyJson, ParameterType.RequestBody);

                                                response1 = client.Execute(request);

                                                // Parse the JSON response
                                                jsonResponse = JObject.Parse(response1.Content);

                                                stattus12 = (int)CompanyInfo.InsertActivityLogDetails("after create customer" + response1.Content, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(Customer_ID), "GetJourney", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "ID Upload", context);


                                                status = (string)jsonResponse["status"];
                                                apiRequestId = (int)jsonResponse["apiRequestId"];
                                                dateTime = (DateTime)jsonResponse["dateTime"];
                                                requestId = (long)jsonResponse["requestId"];
                                                requestReference = (string)jsonResponse["requestReference"];
                                                subjectId = (long)jsonResponse["subjectId"];
                                                subjectReference = (string)jsonResponse["subjectReference"];
                                                requestStatus = (string)jsonResponse["requestStatus"];
                                                Bandtext = requestStatus;


                                                // Extract nested properties under "results"
                                                JObject resultsObj = (JObject)jsonResponse["results"];
                                                JObject watchlistsResultsObj = (JObject)resultsObj["watchlistsResults"];
                                                int totalNumberOfMatches = (int)watchlistsResultsObj["totalNumberOfMatches"];

                                                // Extract array values under "matchStatuses"
                                                matchStatusesArr = (JArray)watchlistsResultsObj["matchStatuses"];
                                                if (matchStatusesArr != null)
                                                {
                                                    // Iterate through each item in the "matchStatuses" array
                                                    foreach (JObject matchStatusObj in matchStatusesArr)
                                                    {
                                                        string matchStatus = (string)matchStatusObj["matchStatus"];
                                                        int numberOfMatches = (int)matchStatusObj["numberOfMatches"];

                                                        // Extract nested properties under "matchTypes"
                                                        JObject matchTypesObj = (JObject)matchStatusObj["matchTypes"];
                                                        bool isSan = (bool)matchTypesObj["san"];   // Indicates if there are sanction matches


                                                        bool isAdv = (bool)matchTypesObj["adv"];   //Indicates if there are adverse media matches


                                                        bool isPep = (bool)matchTypesObj["pep"];   //Indicates if there are PEP (politically exposed persons) matches


                                                        bool isRca = (bool)matchTypesObj["rca"];   //Indicates if there are RCA (relatives or close associates) matches


                                                        bool isSoc = (bool)matchTypesObj["soc"];   //Indicates if there are SOC (state owned companies) matches


                                                        bool isOther = (bool)matchTypesObj["other"]; //Indicates if there are other types of matches





                                                        if (isPep == true)
                                                        {
                                                            flag1 = 1; // set flag 1
                                                            flag = 1;
                                                        }
                                                        else if (isSan == true)
                                                        {
                                                            flag2 = 2; // set flag 2
                                                            flag = 2;
                                                        }
                                                        else if (isAdv == true)
                                                        {
                                                            flag4 = 4; // set flag 2
                                                            flag = 4;
                                                        }
                                                        else if (isRca == true)
                                                        {
                                                            flag5 = 5; // set flag 2
                                                            flag = 5;
                                                        }
                                                        else if (isSoc == true)
                                                        {
                                                            flag6 = 6; // set flag 2
                                                            flag = 6;
                                                        }
                                                        else if (isOther == true)
                                                        {
                                                            flag7 = 7; // set flag 2
                                                            flag = 7;
                                                        }
                                                        if (flag1 > 0 && flag2 > 0)
                                                        {
                                                            flag = 3; // Set flag 3
                                                        }


                                                    }
                                                }
                                                int remark = 0;
                                                if (flag == 0)
                                                {
                                                    remark = 0;
                                                    Remark = "0";
                                                }
                                                else
                                                {
                                                    remark = 1;
                                                    Remark = "1";
                                                }

                                                string request3 = url + "requests" + bodyJson;

                                                try
                                                {
                                                    MySqlCommand _cmd = new MySqlCommand("SaveAPIRequestResponce");
                                                    _cmd.CommandType = CommandType.StoredProcedure;
                                                    _cmd.Parameters.AddWithValue("_API_ID", 10);
                                                    _cmd.Parameters.AddWithValue("_Client_ID", Client_ID);
                                                    _cmd.Parameters.AddWithValue("_Customer_ID", Customer_ID);
                                                    _cmd.Parameters.AddWithValue("_status", 0);
                                                    _cmd.Parameters.AddWithValue("_Function_name", CallFrom);
                                                    _cmd.Parameters.AddWithValue("_Remark", remark.ToString());
                                                    _cmd.Parameters.AddWithValue("_comments", request3);
                                                    _cmd.Parameters.AddWithValue("_Record_Insert_DateTime", Record_Insert_DateTime);
                                                    _cmd.Parameters.AddWithValue("_Branch_ID", obj.Branch_ID);
                                                    int msg1 = db_connection.ExecuteNonQueryProcedure(_cmd);
                                                }
                                                catch (Exception ex)
                                                {
                                                    string error = ex.ToString().Replace("\'", "\\'");
                                                    MySqlCommand _cmd = new MySqlCommand("SaveException");
                                                    _cmd.CommandType = CommandType.StoredProcedure;
                                                    _cmd.Parameters.AddWithValue("_Record_Insert_DateTime", Record_Insert_DateTime);
                                                    _cmd.Parameters.AddWithValue("_error", error);
                                                    _cmd.Parameters.AddWithValue("_Client_ID", Client_ID);
                                                    int msg1 = db_connection.ExecuteNonQueryProcedure(_cmd);
                                                }


                                                // string request = url + "requests" + bodyJson;

                                                try
                                                {
                                                    MySqlCommand _cmd = new MySqlCommand("SaveAPIRequestResponce");
                                                    _cmd.CommandType = CommandType.StoredProcedure;
                                                    _cmd.Parameters.AddWithValue("_API_ID", API_ID);
                                                    _cmd.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                                                    _cmd.Parameters.AddWithValue("_Customer_ID", Customer_ID);
                                                    _cmd.Parameters.AddWithValue("_status", 1);
                                                    _cmd.Parameters.AddWithValue("_Function_name", CallFrom);
                                                    _cmd.Parameters.AddWithValue("_Remark", remark);
                                                    _cmd.Parameters.AddWithValue("_comments", jsonResponse);
                                                    _cmd.Parameters.AddWithValue("_Record_Insert_DateTime", Record_Insert_DateTime);
                                                    _cmd.Parameters.AddWithValue("_Branch_ID", obj.Branch_ID);
                                                    int msg1 = db_connection.ExecuteNonQueryProcedure(_cmd);
                                                }
                                                catch (Exception ex)
                                                {
                                                    string error = ex.ToString().Replace("\'", "\\'");
                                                    MySqlCommand _cmd = new MySqlCommand("SaveException");
                                                    _cmd.CommandType = CommandType.StoredProcedure;
                                                    _cmd.Parameters.AddWithValue("_Record_Insert_DateTime", Record_Insert_DateTime);
                                                    _cmd.Parameters.AddWithValue("_error", error);
                                                    _cmd.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                                                    int msg1 = db_connection.ExecuteNonQueryProcedure(_cmd);
                                                }

                                            }
                                            catch (Exception ex)
                                            {
                                                string stattus = (string)CompanyInfo.InsertErrorLogDetails(ex.Message.Replace("\'", "\\'"), 0, "Complience Assist AMl Check - 1st api call ", obj.Branch_ID, obj.Client_ID);

                                            }
                                        }
                                        ///""WGMG001"",

                                        if (requestId != 0)
                                        {

                                            try
                                            {

                                                //ServicePointManager.Expect100Continue = true;
                                                //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
                                                //       | SecurityProtocolType.Tls11
                                                //       | SecurityProtocolType.Tls12
                                                //       | SecurityProtocolType.Ssl3;
                                                client = new RestClient(url + "requests/" + requestId);
                                                client.Timeout = 3000; // Set a timeout of 3 seconds
                                                request = new RestRequest(Method.GET);
                                                request.AddHeader("Authorization", "Bearer " + accessToken);
                                                request.AddHeader("Accept", "application/pdf");
                                                request.AddHeader("Content-Type", "application/json"); // Add Content-Type header
                                                response1 = client.Execute(request);

                                                if (response1.StatusCode == HttpStatusCode.OK)
                                                {
                                                    byte[] pdfData = response1.RawBytes;

                                                    // Write the PDF data to a file


                                                    if (pdfData != null && pdfData.Length > 0)
                                                    {
                                                        // Write the PDF data to a file
                                                        string filePath = "assets/Other_Docs/" + "PDF-" + RefNum + "-" + Record_Insert_DateTime.Replace(":", "") + ".pdf";
                                                        string URL = rooturl + filePath;
                                                        try
                                                        {
                                                            File.WriteAllBytes(URL, pdfData);
                                                            Console.WriteLine("PDF saved successfully at: " + filePath);

                                                            string pdfdownload = "0";

                                                            string func_name = "Save_compliance_pdf";
                                                            filePath = filePath + "=" + requestId;
                                                            try
                                                            {
                                                                MySqlCommand _cmd5 = new MySqlCommand("Insert_PDF_Data");
                                                                _cmd5.CommandType = CommandType.StoredProcedure;
                                                                _cmd5.Parameters.AddWithValue("_API_ID", API_ID);
                                                                _cmd5.Parameters.AddWithValue("_Client_ID", Client_ID);
                                                                _cmd5.Parameters.AddWithValue("_Customer_ID", Customer_ID);
                                                                _cmd5.Parameters.AddWithValue("_status", 1);
                                                                _cmd5.Parameters.AddWithValue("_Function_name", func_name);
                                                                _cmd5.Parameters.AddWithValue("_Remark", Remark);
                                                                _cmd5.Parameters.AddWithValue("_comments", filePath);
                                                                _cmd5.Parameters.AddWithValue("_Record_Insert_DateTime", Record_Insert_DateTime);
                                                                _cmd5.Parameters.AddWithValue("_Branch_ID", obj.Branch_ID);
                                                                int msg5 = db_connection.ExecuteNonQueryProcedure(_cmd5);
                                                            }
                                                            catch (Exception ex)
                                                            {
                                                                string error = ex.ToString().Replace("\'", "\\'");
                                                                MySqlCommand _cmd5 = new MySqlCommand("SaveException");
                                                                _cmd5.CommandType = CommandType.StoredProcedure;
                                                                _cmd5.Parameters.AddWithValue("_Record_Insert_DateTime", Record_Insert_DateTime);
                                                                _cmd5.Parameters.AddWithValue("_error", error);
                                                                _cmd5.Parameters.AddWithValue("_Client_ID", Client_ID);
                                                                int msg5 = db_connection.ExecuteNonQueryProcedure(_cmd5);
                                                            }



                                                        }
                                                        catch (Exception ex)
                                                        {
                                                            Console.WriteLine("Error saving PDF: " + ex.Message);
                                                        }
                                                    }

                                                }
                                                else
                                                {
                                                    // Handle the case where the request fails or returns an error
                                                    Console.WriteLine("Error: " + response1.StatusCode);
                                                }

                                            }

                                            catch (Exception ex)
                                            {
                                                string stattus = (string)CompanyInfo.InsertErrorLogDetails(ex.Message.Replace("\'", "\\'"), 0, "Complience Assist AMl Check - 2st api call (PDF download)  ", obj.Branch_ID, obj.Client_ID);

                                            }


                                        }




                                        try
                                        {

                                            client = new RestClient(url + "requestTypes/WGMG001");
                                            client.Timeout = -1;
                                            request = new RestRequest(Method.GET);
                                            request.AddHeader("Authorization", "Bearer " + accessToken);
                                            request.AddHeader("Accept", "application/json");


                                            response1 = client.Execute(request);


                                            //Parse the JSON response
                                            jsonResponse = JObject.Parse(response1.Content);

                                            string status2 = (string)jsonResponse["status"];
                                            int apiRequestId2 = (int)jsonResponse["apiRequestId"];
                                            DateTime dateTime2 = (DateTime)jsonResponse["dateTime"];
                                            string shortCode = (string)jsonResponse["shortCode"];

                                            JArray subjectTypes = (JArray)jsonResponse["subjectTypes"];
                                            JArray requestFields = (JArray)jsonResponse["requestFields"];
                                            JArray responseElements = (JArray)jsonResponse["responseElements"];

                                            foreach (var subjectType in subjectTypes)
                                            {
                                                Console.WriteLine("Subject Type: " + subjectType);
                                            }

                                            foreach (var requestField in requestFields)
                                            {
                                                JObject fieldObject = (JObject)requestField;
                                                string subjectType = (string)fieldObject["subjectType"];
                                                Console.WriteLine("Subject Type: " + subjectType);
                                                JArray fields = (JArray)fieldObject["fields"];
                                                foreach (var field in fields)
                                                {
                                                    JObject fieldDetails = (JObject)field;
                                                    string fieldName = (string)fieldDetails["fieldName"];
                                                    string displayName = (string)fieldDetails["displayName"];
                                                    bool mandatory = (bool)fieldDetails["mandatory"];
                                                    Console.WriteLine("Field Name: " + fieldName + ", Display Name: " + displayName + ", Mandatory: " + mandatory);
                                                }
                                            }

                                            foreach (var responseElement in responseElements)
                                            {
                                                Console.WriteLine("Response Element: " + responseElement);
                                            }

                                        }

                                        catch (Exception ex)
                                        {
                                            string stattus = (string)CompanyInfo.InsertErrorLogDetails(ex.Message.Replace("\'", "\\'"), 0, "Complience Assist AMl Check - 3rd api call  ", obj.Branch_ID, obj.Client_ID);

                                        }

                                        if (subjectId != 0)
                                        {

                                            try
                                            {

                                                client = new RestClient(url + "subjects?id=" + subjectId);
                                                client.Timeout = -1;
                                                request = new RestRequest(Method.GET);
                                                request.AddHeader("Authorization", "Bearer " + accessToken);
                                                request.AddHeader("Accept", "application/json");

                                                response1 = client.Execute(request);


                                                // Parse the JSON response
                                                jsonResponse = JObject.Parse(response1.Content);


                                                status3 = (string)jsonResponse["status"];
                                                int apiRequestId3 = (int)jsonResponse["apiRequestId"];
                                                DateTime dateTime3 = (DateTime)jsonResponse["dateTime"];

                                                JArray subjects = (JArray)jsonResponse["subjects"];

                                                if (subjects != null)
                                                {
                                                    foreach (var subject in subjects)
                                                    {
                                                        JObject subjectObject = (JObject)subject;
                                                        int subjectId3 = (int)subjectObject["subjectId"];
                                                        string subjectReference3 = (string)subjectObject["subjectReference"];
                                                        string subjectIdentifier = (string)subjectObject["subjectIdentifier"];
                                                        string subjectType = (string)subjectObject["subjectType"];
                                                        string subjectStatus = (string)subjectObject["subjectStatus"];
                                                        bool isMonitored = (bool)subjectObject["isMonitored"];
                                                        string monitoredRequestType = (string)subjectObject["monitoredRequestType"];
                                                        string riskRating = (string)subjectObject["riskRating"];

                                                    }
                                                }


                                                string request3 = url + "subjects?id=" + subjectId;
                                                try
                                                {
                                                    MySqlCommand _cmd = new MySqlCommand("SaveAPIRequestResponce");
                                                    _cmd.CommandType = CommandType.StoredProcedure;
                                                    _cmd.Parameters.AddWithValue("_API_ID", 10);
                                                    _cmd.Parameters.AddWithValue("_Client_ID", Client_ID);
                                                    _cmd.Parameters.AddWithValue("_Customer_ID", Customer_ID);
                                                    _cmd.Parameters.AddWithValue("_status", 0);
                                                    _cmd.Parameters.AddWithValue("_Function_name", CallFrom);
                                                    _cmd.Parameters.AddWithValue("_Remark", Remark);
                                                    _cmd.Parameters.AddWithValue("_comments", request3);
                                                    _cmd.Parameters.AddWithValue("_Record_Insert_DateTime", Record_Insert_DateTime);
                                                    _cmd.Parameters.AddWithValue("_Branch_ID", obj.Branch_ID);
                                                    int msg1 = db_connection.ExecuteNonQueryProcedure(_cmd);
                                                }
                                                catch (Exception ex)
                                                {
                                                    string error = ex.ToString().Replace("\'", "\\'");
                                                    MySqlCommand _cmd = new MySqlCommand("SaveException");
                                                    _cmd.CommandType = CommandType.StoredProcedure;
                                                    _cmd.Parameters.AddWithValue("_Record_Insert_DateTime", Record_Insert_DateTime);
                                                    _cmd.Parameters.AddWithValue("_error", error);
                                                    _cmd.Parameters.AddWithValue("_Client_ID", Client_ID);
                                                    int msg1 = db_connection.ExecuteNonQueryProcedure(_cmd);
                                                }

                                            }

                                            catch (Exception ex)
                                            {
                                                string stattus = (string)CompanyInfo.InsertErrorLogDetails(ex.Message.Replace("\'", "\\'"), 0, "Complience Assist AMl Check - 4th api call (Subject Information)  ", obj.Branch_ID, obj.Client_ID);

                                            }

                                            //string Remark = Convert.ToString(CompanyInfo.getAPIStatus(Bandtext, obj.Client_ID));

                                            try
                                            {
                                                MySqlCommand _cmd = new MySqlCommand("SaveAPIRequestResponce");
                                                _cmd.CommandType = CommandType.StoredProcedure;
                                                _cmd.Parameters.AddWithValue("_API_ID", API_ID);
                                                _cmd.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                                                _cmd.Parameters.AddWithValue("_Customer_ID", Customer_ID);
                                                _cmd.Parameters.AddWithValue("_status", 1);
                                                _cmd.Parameters.AddWithValue("_Function_name", CallFrom);
                                                _cmd.Parameters.AddWithValue("_Remark", Remark);
                                                _cmd.Parameters.AddWithValue("_comments", jsonResponse);
                                                _cmd.Parameters.AddWithValue("_Record_Insert_DateTime", Record_Insert_DateTime);
                                                _cmd.Parameters.AddWithValue("_Branch_ID", obj.Branch_ID);
                                                int msg1 = db_connection.ExecuteNonQueryProcedure(_cmd);
                                            }
                                            catch (Exception ex)
                                            {
                                                string error = ex.ToString().Replace("\'", "\\'");
                                                MySqlCommand _cmd = new MySqlCommand("SaveException");
                                                _cmd.CommandType = CommandType.StoredProcedure;
                                                _cmd.Parameters.AddWithValue("_Record_Insert_DateTime", Record_Insert_DateTime);
                                                _cmd.Parameters.AddWithValue("_error", error);
                                                _cmd.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                                                int msg1 = db_connection.ExecuteNonQueryProcedure(_cmd);
                                            }


                                        }

                                    }
                                    catch (Exception ex)

                                    {
                                        //send complaince alert
                                        string Record_DateTime_exception = Record_Insert_DateTime;
                                        string notification_icon_exception = "aml-referd.jpg";
                                        string notification_message_exception = "<span class='cls-admin'>Complience AML check <strong class='cls-cancel'> Request failed.</strong></span><span class='cls-customer'></span>";
                                        CompanyInfo.save_notification_compliance(notification_message_exception, notification_icon_exception, Convert.ToString(Customer_ID), Convert.ToDateTime(Record_DateTime_exception), obj.Client_ID, 1, obj.User_ID, obj.Branch_ID, 0, 1, 1, 0, context);


                                        try
                                        {
                                            mail_send = string.Empty;
                                            string sendmsg = "Complience AML check Request failed. Please contact Complience support team. ";
                                            string EmailID = "";
                                            DataTable dt_admin_Email_list = (DataTable)getAdminEmailList(obj.Client_ID, obj.Branch_ID);
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

                                            subject = "" + Company_Name + " - Compliance Alert - " + WireTransfer_ReferanceNo;
                                            mail_send = (string)CompanyInfo.Send_Mail(dt_admin_Email_list, email, body, subject, Convert.ToInt32(obj.Client_ID), Convert.ToInt32(obj.Branch_ID), "", "", "", context);

                                        }
                                        catch { }

                                    }






                                    if (flag > 0)
                                    {
                                        if (obj.Branch_ID == 0)
                                        {
                                            obj.Branch_ID = 2;
                                        }
                                        using (MySqlCommand cmd = new MySqlCommand("update_sanction_flag"))//, cn
                                        {
                                            //if (cn.State != ConnectionState.Open) { cn.Open(); }
                                            cmd.CommandType = CommandType.StoredProcedure;
                                            cmd.Parameters.AddWithValue("_Customer_ID", Customer_ID);
                                            cmd.Parameters.AddWithValue("_record_date", Record_Insert_DateTime);
                                            cmd.Parameters.AddWithValue("_clientId", 1);
                                            cmd.Parameters.AddWithValue("_branchId", obj.Branch_ID);
                                            cmd.Parameters.AddWithValue("_sanction_flag", flag);
                                            int n = db_connection.ExecuteNonQueryProcedure(cmd);

                                        }

                                        using (MySqlCommand cmd = new MySqlCommand("Insert_pep_sanc_detail"))
                                        {

                                            cmd.CommandType = CommandType.StoredProcedure;
                                            cmd.Parameters.AddWithValue("p_SenderID_ID", SenderID_ID);
                                            cmd.Parameters.AddWithValue("p_Flag", 1);
                                            cmd.Parameters.AddWithValue("p_Record_Insert_DateTime", Record_Insert_DateTime.ToString());
                                            cmd.Parameters.AddWithValue("p_Customer_ID", Customer_ID);
                                            cmd.Parameters.AddWithValue("p_API_ID", 10);
                                            int n = db_connection.ExecuteNonQueryProcedure(cmd);

                                        }


                                        try
                                        {
                                            MySqlCommand cmd_update = new MySqlCommand("update documents_details set aml_sanctions_flag=1 where SenderID_ID=@SenderID_ID");
                                            cmd_update.Parameters.AddWithValue("@SenderID_ID", SenderID_ID);
                                            db_connection.ExecuteNonQueryProcedure(cmd_update);
                                        }
                                        catch
                                        {

                                        }

                                    }
                                    if (flag == 0)
                                    {
                                        if (obj.Branch_ID == 0)
                                        {
                                            obj.Branch_ID = 2;
                                        }
                                        using (MySqlCommand cmd = new MySqlCommand("update_sanction_flag"))
                                        {

                                            cmd.CommandType = CommandType.StoredProcedure;
                                            cmd.Parameters.AddWithValue("_Customer_ID", Customer_ID);
                                            cmd.Parameters.AddWithValue("_record_date", Record_Insert_DateTime);
                                            cmd.Parameters.AddWithValue("_clientId", 1);
                                            cmd.Parameters.AddWithValue("_branchId", obj.Branch_ID);
                                            cmd.Parameters.AddWithValue("_sanction_flag", 0);
                                            int n = db_connection.ExecuteNonQueryProcedure(cmd);

                                        }

                                        using (MySqlCommand cmd = new MySqlCommand("Insert_pep_sanc_detail"))
                                        {

                                            cmd.CommandType = CommandType.StoredProcedure;
                                            cmd.Parameters.AddWithValue("p_SenderID_ID", SenderID_ID);
                                            cmd.Parameters.AddWithValue("p_Flag", 0);
                                            cmd.Parameters.AddWithValue("p_Record_Insert_DateTime", Record_Insert_DateTime.ToString());
                                            cmd.Parameters.AddWithValue("p_Customer_ID", Customer_ID);
                                            cmd.Parameters.AddWithValue("p_API_ID", 10);
                                            int n = db_connection.ExecuteNonQueryProcedure(cmd);

                                        }


                                    }




                                    if (requestStatus != "" && requestStatus != null)
                                    {

                                        if (matchStatusesArr != null)
                                        {

                                            foreach (JObject matchStatusObj in matchStatusesArr)
                                            {
                                                string matchStatus1 = (string)matchStatusObj["matchStatus"];
                                                int numberOfMatches1 = (int)matchStatusObj["numberOfMatches"];

                                                // Extract nested properties under "matchTypes"
                                                JObject matchTypesObj1 = (JObject)matchStatusObj["matchTypes"];
                                                bool isSan = (bool)matchTypesObj1["san"];
                                                bool isAdv = (bool)matchTypesObj1["adv"];
                                                bool isPep = (bool)matchTypesObj1["pep"];
                                                bool isRca = (bool)matchTypesObj1["rca"];
                                                bool isSoc = (bool)matchTypesObj1["soc"];
                                                bool isOther = (bool)matchTypesObj1["other"];
                                                if (isPep == true)
                                                {


                                                    string description1 = "Customer Found In Pep";
                                                    string Record_DateTime = Record_Insert_DateTime;
                                                    string notification_icon = "aml-referd.jpg";
                                                    //string notification_message = "<span class='cls-admin'> International PEP Alert - <strong class='cls-cancel'></strong><br/>" + description1 + "</span><span class='cls-customer'></span>";
                                                    string notification_message = "<span class='cls-admin'> - <span class='cls-cancel'><strong> International PEP Alert </strong>- </span><span class='cls-addr-confirm'>" + description1 + " </span></span></span><span class='cls-customer'></span>";
                                                    CompanyInfo.save_notification_compliance(notification_message, notification_icon, Convert.ToString(Customer_ID), Convert.ToDateTime(Record_DateTime), obj.Client_ID, 1, obj.User_ID, obj.Branch_ID, 0, 1, 1, 0, context);





                                                }
                                                if (isSan == true)
                                                {



                                                    string description1 = "Customer Found In Sanctions";
                                                    string Record_DateTime = Record_Insert_DateTime;
                                                    string notification_icon = "pep-match-not-found.jpg";
                                                    //notification_message = "<span class='cls-admin'> - <span class='cls-cancel'><strong> International Sanctions Alert  </strong>- </span><span class='cls-addr-confirm'>" + description1 + " </span></span></span><span class='cls-customer'></span>";
                                                    string notification_message = "<span class='cls-admin'> - <span class='cls-cancel'><strong> International Sanctions Alert </strong>- </span><span class='cls-addr-confirm'>" + description1 + " </span></span></span><span class='cls-customer'></span>";
                                                    CompanyInfo.save_notification(notification_message, notification_icon, Convert.ToInt32(Customer_ID), Convert.ToDateTime(Record_Insert_DateTime), Convert.ToInt32(obj.Client_ID), 1, obj.User_ID, Convert.ToInt32(obj.Branch_ID), 0, 0, 1, 0, context);




                                                }
                                                if (isAdv == true)
                                                {



                                                    string description1 = "Customer Found In Adverse media matchess";
                                                    string Record_DateTime = Record_Insert_DateTime;
                                                    string notification_icon = "pep-match-not-found.jpg";
                                                    //notification_message = "<span class='cls-admin'> - <span class='cls-cancel'><strong> International Sanctions Alert  </strong>- </span><span class='cls-addr-confirm'>" + description1 + " </span></span></span><span class='cls-customer'></span>";
                                                    string notification_message = "<span class='cls-admin'> - <span class='cls-cancel'><strong> Adverse media matches Alert </strong>- </span><span class='cls-addr-confirm'>" + description1 + " </span></span></span><span class='cls-customer'></span>";
                                                    CompanyInfo.save_notification(notification_message, notification_icon, Convert.ToInt32(Customer_ID), Convert.ToDateTime(Record_Insert_DateTime), Convert.ToInt32(obj.Client_ID), 1, obj.User_ID, Convert.ToInt32(obj.Branch_ID), 0, 0, 1, 0, context);




                                                }
                                                if (isRca == true)
                                                {



                                                    string description1 = "Customer Found In RCA (relatives or close associates) matches";
                                                    string Record_DateTime = Record_Insert_DateTime;
                                                    string notification_icon = "pep-match-not-found.jpg";
                                                    //notification_message = "<span class='cls-admin'> - <span class='cls-cancel'><strong> International Sanctions Alert  </strong>- </span><span class='cls-addr-confirm'>" + description1 + " </span></span></span><span class='cls-customer'></span>";
                                                    string notification_message = "<span class='cls-admin'> - <span class='cls-cancel'><strong> RCA (relatives or close associates) matches Alert </strong>- </span><span class='cls-addr-confirm'>" + description1 + " </span></span></span><span class='cls-customer'></span>";
                                                    CompanyInfo.save_notification(notification_message, notification_icon, Convert.ToInt32(Customer_ID), Convert.ToDateTime(Record_Insert_DateTime), Convert.ToInt32(obj.Client_ID), 1, obj.User_ID, Convert.ToInt32(obj.Branch_ID), 0, 0, 1, 0, context);




                                                }
                                                if (isSoc == true)
                                                {



                                                    string description1 = "Customer Found In SOC (state owned companies) matches";
                                                    string Record_DateTime = Record_Insert_DateTime;
                                                    string notification_icon = "pep-match-not-found.jpg";
                                                    //notification_message = "<span class='cls-admin'> - <span class='cls-cancel'><strong> International Sanctions Alert  </strong>- </span><span class='cls-addr-confirm'>" + description1 + " </span></span></span><span class='cls-customer'></span>";
                                                    string notification_message = "<span class='cls-admin'> - <span class='cls-cancel'><strong> SOC (state owned companies) matches Alert </strong>- </span><span class='cls-addr-confirm'>" + description1 + " </span></span></span><span class='cls-customer'></span>";
                                                    CompanyInfo.save_notification(notification_message, notification_icon, Convert.ToInt32(Customer_ID), Convert.ToDateTime(Record_Insert_DateTime), Convert.ToInt32(obj.Client_ID), 1, obj.User_ID, Convert.ToInt32(obj.Branch_ID), 0, 0, 1, 0, context);




                                                }
                                                if (isOther == true)
                                                {



                                                    string description1 = "Cutomer Found in others";
                                                    string Record_DateTime = Record_Insert_DateTime;
                                                    string notification_icon = "pep-match-not-found.jpg";
                                                    //notification_message = "<span class='cls-admin'> - <span class='cls-cancel'><strong> International Sanctions Alert  </strong>- </span><span class='cls-addr-confirm'>" + description1 + " </span></span></span><span class='cls-customer'></span>";
                                                    string notification_message = "<span class='cls-admin'> - <span class='cls-cancel'><strong> International AML Alert </strong>- </span><span class='cls-addr-confirm'>" + description1 + " </span></span></span><span class='cls-customer'></span>";
                                                    CompanyInfo.save_notification(notification_message, notification_icon, Convert.ToInt32(Customer_ID), Convert.ToDateTime(Record_Insert_DateTime), Convert.ToInt32(obj.Client_ID), 1, obj.User_ID, Convert.ToInt32(obj.Branch_ID), 0, 0, 1, 0, context);




                                                }

                                                //string ssa = childd.InnerXml;
                                                //string Address_match = childd.InnerText.ToString();






                                            }
                                        }

                                        if (flag1 > 0 && flag2 > 0)
                                        {
                                            try
                                            {
                                                MySqlCommand cmd_update = new MySqlCommand("update documents_details set aml_sanctions_flag=3 where SenderID_ID=@SenderID_ID");
                                                cmd_update.Parameters.AddWithValue("@SenderID_ID", SenderID_ID);
                                                db_connection.ExecuteNonQueryProcedure(cmd_update);

                                                using (MySqlCommand cmd = new MySqlCommand("update_sanction_flag"))//, cn
                                                {
                                                    //if (cn.State != ConnectionState.Open) { cn.Open(); }
                                                    cmd.CommandType = CommandType.StoredProcedure;
                                                    cmd.Parameters.AddWithValue("_Customer_ID", Customer_ID);
                                                    cmd.Parameters.AddWithValue("_record_date", Record_Insert_DateTime);
                                                    cmd.Parameters.AddWithValue("_clientId", obj.Client_ID);
                                                    cmd.Parameters.AddWithValue("_branchId", obj.Branch_ID);
                                                    cmd.Parameters.AddWithValue("_sanction_flag", flag1);
                                                    int n = db_connection.ExecuteNonQueryProcedure(cmd);

                                                }
                                            }
                                            catch { }
                                        }
                                        else if (flag1 > 0)
                                        {
                                            try
                                            {
                                                MySqlCommand cmd_update = new MySqlCommand("update documents_details set aml_sanctions_flag=1 where SenderID_ID=@SenderID_ID");
                                                cmd_update.Parameters.AddWithValue("@SenderID_ID", SenderID_ID);
                                                db_connection.ExecuteNonQueryProcedure(cmd_update);

                                                using (MySqlCommand cmd = new MySqlCommand("update_sanction_flag"))//, cn
                                                {
                                                    //if (cn.State != ConnectionState.Open) { cn.Open(); }
                                                    cmd.CommandType = CommandType.StoredProcedure;
                                                    cmd.Parameters.AddWithValue("_Customer_ID", obj.Customer_ID);
                                                    cmd.Parameters.AddWithValue("_record_date", obj.Record_Insert_DateTime);
                                                    cmd.Parameters.AddWithValue("_clientId", obj.Client_ID);
                                                    cmd.Parameters.AddWithValue("_branchId", obj.Branch_ID);
                                                    cmd.Parameters.AddWithValue("_sanction_flag", flag1);
                                                    int n = db_connection.ExecuteNonQueryProcedure(cmd);

                                                }

                                            }
                                            catch { }
                                        }
                                        else if (flag2 > 0)
                                        {
                                            try
                                            {
                                                MySqlCommand cmd_update = new MySqlCommand("update documents_details set aml_sanctions_flag=2 where SenderID_ID=@SenderID_ID");
                                                cmd_update.Parameters.AddWithValue("@SenderID_ID", SenderID_ID);
                                                db_connection.ExecuteNonQueryProcedure(cmd_update);


                                                using (MySqlCommand cmd = new MySqlCommand("update_sanction_flag"))//, cn
                                                {
                                                    //if (cn.State != ConnectionState.Open) { cn.Open(); }
                                                    cmd.CommandType = CommandType.StoredProcedure;
                                                    cmd.Parameters.AddWithValue("_Customer_ID", Customer_ID);
                                                    cmd.Parameters.AddWithValue("_record_date", Record_Insert_DateTime);
                                                    cmd.Parameters.AddWithValue("_clientId", obj.Client_ID);
                                                    cmd.Parameters.AddWithValue("_branchId", obj.Branch_ID);
                                                    cmd.Parameters.AddWithValue("_sanction_flag", flag2);
                                                    int n = db_connection.ExecuteNonQueryProcedure(cmd);

                                                }
                                            }
                                            catch { }
                                        }
                                        else
                                if (requestStatus == "Complete" && flag == 0)
                                        {
                                            string Record_DateTime = Record_Insert_DateTime;
                                            string notification_icon = "primary-id-upload.jpg";
                                            string notification_message = "<span class='cls-admin'>Complience AML check result is <strong class='cls-priamary'>" + requestStatus + ".</strong></span><span class='cls-customer'></span>";
                                            CompanyInfo.save_notification(notification_message, notification_icon, Convert.ToInt32(Customer_ID), Convert.ToDateTime(Record_DateTime), obj.Client_ID, 1, obj.User_ID, obj.Branch_ID, 0, 1, 1, 0, context);

                                        }

                                        try
                                        {
                                            //string Remark = Convert.ToString(CompanyInfo.getAPIStatus(Bandtext, obj.Client_ID));
                                            if (Remark == "2" || Remark == "1")//alert or refer then  send mail to admin
                                            {
                                                string Record_DateTime = DateTime.Now.ToString();
                                                string notification_icon = "aml-referd.jpg";
                                                string notification_message = "<span class='cls-admin'>Complience AML check result is <strong class='cls-cancel'>" + Bandtext + ".</strong></span><span class='cls-customer'></span>";
                                                CompanyInfo.save_notification_compliance(notification_message, notification_icon, Convert.ToString(Customer_ID), Convert.ToDateTime(Record_DateTime), obj.Client_ID, 1, obj.User_ID, obj.Branch_ID, 0, 1, 1, 0, context);
                                            }
                                            else
                                            {
                                                string Record_DateTime = Record_Insert_DateTime;
                                                string notification_icon = "primary-id-upload.jpg";
                                                string notification_message = "<span class='cls-admin'>Complience AML check result is <strong class='cls-priamary'>" + Bandtext + ".</strong></span><span class='cls-customer'></span>";
                                                CompanyInfo.save_notification_compliance(notification_message, notification_icon, Convert.ToString(Customer_ID), Convert.ToDateTime(Record_DateTime), obj.Client_ID, 1, obj.User_ID, obj.Branch_ID, 0, 1, 1, 0, context);
                                            }

                                        }
                                        catch (Exception e)
                                        {
                                        }
                                        try
                                        {
                                            //string Remark = Convert.ToString(CompanyInfo.getAPIStatus(Bandtext, obj.Client_ID));
                                            if (Remark == "1" || Remark == "2" || Remark == "3")//alert or refer then  send mail to admin
                                            {

                                                //DataTable ds1 = CompanyInfo.get(Client_ID, context);
                                                if (ds1.Rows.Count > 0)
                                                {
                                                    BaseCurrency_Code = Convert.ToString(ds1.Rows[0]["BaseCurrency_Code"]);
                                                    BaseCurrency_TimeZone = Convert.ToString(ds1.Rows[0]["BaseCurrency_TimeZone"]);
                                                    BaseCurrency_Sign = Convert.ToString(ds1.Rows[0]["BaseCurrency_Sign"]);
                                                    BaseCurrency_Country = Convert.ToString(ds1.Rows[0]["BaseCurrency_Country"]);
                                                    BaseCountry_ID = Convert.ToString(ds1.Rows[0]["BaseCountry_ID"]);
                                                    EmailUrl = Convert.ToString(ds1.Rows[0]["Company_URL_Admin"]);
                                                    CustomerURL = Convert.ToString(ds1.Rows[0]["Company_URL_Customer"]);
                                                    Company_Name = Convert.ToString(ds1.Rows[0]["Company_Name"]);
                                                    Cancel_Transaction_Hours = Convert.ToString(ds1.Rows[0]["Cancel_Transaction_Hours"]);
                                                }


                                                MySqlCommand _cmd = new MySqlCommand("customer_details_by_param");
                                                _cmd.CommandType = CommandType.StoredProcedure;
                                                string _whereclause = " and cr.Client_ID=" + obj.Client_ID + " and cr.Customer_ID=" + Customer_ID;
                                                //  DataTable d1 = (DataTable)mtsmethods.GetCustDetailsByID(c.Customer_ID); //get customer details by id

                                                _cmd.Parameters.AddWithValue("_whereclause", _whereclause);
                                                _cmd.Parameters.AddWithValue("_SecurityKey", CompanyInfo.SecurityKey());
                                                DataTable d1 = db_connection.ExecuteQueryDataTableProcedure(_cmd);
                                                string sendmsg = "Response from third Party AML check for  " + d1.Rows[0]["WireTransfer_ReferanceNo"] + "   is   " + Bandtext + "";




                                                string EmailID = "";
                                                DataTable dt_admin_Email_list = (DataTable)getAdminEmailList(obj.Client_ID, obj.Branch_ID);
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

                                                subject = "" + Company_Name + " - Compliance - Alert - " + d1.Rows[0]["WireTransfer_ReferanceNo"];
                                                //subject = "" + HttpContext.Current.Session["Company_Name"] + " -  Incomplete Customer Registration Details " + c.WireTransfer_ReferanceNo;
                                                mail_send = (string)CompanyInfo.Send_Mail(dt_admin_Email_list, email, body, subject, Convert.ToInt32(obj.Client_ID), Convert.ToInt32(obj.Branch_ID), "Alert Admins", "", "", context);
                                            }
                                        }
                                        catch (Exception ae)
                                        {
                                            string stattus = (string)CompanyInfo.InsertErrorLogDetails(ae.Message.Replace("\'", "\\'"), 0, "Check Credit Safe", obj.Branch_ID, obj.Client_ID);
                                        }
                                    }
                                }
                                else
                                {
                                    stattus12 = (int)CompanyInfo.InsertActivityLogDetails("Records Not Found in ds datatable" + whereclause + Customer_ID + Client_ID, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(Customer_ID), "GetJourney", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "ID Upload", context);
                                }


                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    mail_send = string.Empty;
                    string stattus = (string)CompanyInfo.InsertErrorLogDetails(ex.ToString().Replace("\'", "\\'"), 0, "Check_AML", obj.Branch_ID, obj.Client_ID);
                    string sendmsg = "Customer Registered successfully but we are unable to check Credit Safe";
                    string EmailID = "";
                    DataTable dt_admin_Email_list = (DataTable)getAdminEmailList(obj.Client_ID, obj.Branch_ID);
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

                    if (Convert.ToString(EmailUrl) == "" || Convert.ToString(EmailUrl) == null)
                    {

                        MySqlCommand _cmd = new MySqlCommand("customer_details_by_param");
                        _cmd.CommandType = CommandType.StoredProcedure;
                        string _whereclause = " and cr.Client_ID=" + obj.Client_ID + " and cr.Customer_ID=" + Customer_ID;
                        //  DataTable d1 = (DataTable)mtsmethods.GetCustDetailsByID(c.Customer_ID); //get customer details by id

                        _cmd.Parameters.AddWithValue("_whereclause", _whereclause);
                        _cmd.Parameters.AddWithValue("_SecurityKey", CompanyInfo.SecurityKey());
                        DataTable dss = db_connection.ExecuteQueryDataTableProcedure(_cmd);

                        if (dss.Rows.Count > 0)
                        {
                            BaseCurrency_Code = Convert.ToString(dss.Rows[0]["BaseCurrency_Code"]);
                            BaseCurrency_TimeZone = Convert.ToString(dss.Rows[0]["BaseCurrency_TimeZone"]);
                            BaseCurrency_Sign = Convert.ToString(dss.Rows[0]["BaseCurrency_Sign"]);
                            BaseCurrency_Country = Convert.ToString(dss.Rows[0]["BaseCurrency_Country"]);
                            BaseCountry_ID = Convert.ToString(dss.Rows[0]["BaseCountry_ID"]);
                            EmailUrl = Convert.ToString(dss.Rows[0]["Company_URL_Admin"]);
                            CustomerURL = Convert.ToString(dss.Rows[0]["Company_URL_Customer"]);
                            RootURL = Convert.ToString(dss.Rows[0]["RootURL"]); //"C:/inetpub/wwwroot/bureaudechangesoftware-LIVE/MTS/MTS/";//"C:/inetpub/wwwroot/mts-teeparam-admin/";
                        }
                    }



                    httpRequest = (HttpWebRequest)WebRequest.Create("" + EmailUrl + "Email/customemail.html");

                    httpRequest.UserAgent = "Code Sample Web Client";
                    HttpWebResponse webResponse = (HttpWebResponse)httpRequest.GetResponse();
                    using (StreamReader reader = new StreamReader(webResponse.GetResponseStream()))
                    {
                        body = reader.ReadToEnd();
                    }
                    body = body.Replace("[name]", "Administrators");

                    body = body.Replace("[msg]", sendmsg);
                    subject = "" + Company_Name + " -  Incomplete Customer Registration Details.";
                    //HttpWebRequest httpRequest1 = (HttpWebRequest)WebRequest.Create("" + HttpContext.Current.Session["EmailUrl"] + "Email/RegisteredSuccessMail.txt");

                    //httpRequest1.UserAgent = "Code Sample Web Client";
                    //HttpWebResponse webResponse1 = (HttpWebResponse)httpRequest1.GetResponse();
                    //using (StreamReader reader = new StreamReader(webResponse1.GetResponseStream()))
                    //{
                    //    subject = reader.ReadLine();
                    //}
                    mail_send = (string)CompanyInfo.Send_Mail(dt_admin_Email_list, email, body, subject, Convert.ToInt32(obj.Client_ID), Convert.ToInt32(obj.Branch_ID), "Alert Admins", "", "", context);
                    stattus = (string)CompanyInfo.InsertErrorLogDetails(ex.Message.Replace("\'", "\\'"), 0, "Check_AML", obj.Branch_ID, obj.Client_ID);
                }
            }
            catch (Exception ex)
            {
                string stattus = (string)CompanyInfo.InsertErrorLogDetails("Error In Check_AML funcrtion" + ex.Message.Replace("\'", "\\'"), 0, "Check_AML", obj.Branch_ID, obj.Client_ID);
            }

            string[] response = { Status.ToString(), Bandtext };
            return response;
        }

    }

   

}


internal class WatermarkDirection
{
    public static WatermarkDirection BottomLeftToTopRight { get; internal set; }
    public static WatermarkDirection TopLeftToBottomRight { get; internal set; }
}


