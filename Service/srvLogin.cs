using System;
using System.Threading.Tasks;
using MySqlConnector;
using Calyx_Solutions.Model;
using Calyx_Solutions.Service;
using System.Net.Http;
using System.IO;
using System.Web;
using System.Net;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore;
using System.Data.Common;
using Microsoft.AspNetCore.Mvc;
using RestSharp;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using System.Collections;
//using Borland.Delphi;
using MySql.Data.MySqlClient.Memcached;
using Calyx_Solutions.Controllers;


using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

using gudusoft.gsqlparser;
using System.Text.RegularExpressions;
using System.Net;
using RestSharp;
using System.Net.Http;
using System.IO;
using System.Web;
using System.Security.Cryptography;
using System.Net.NetworkInformation;

using static System.Net.WebRequestMethods;
using System.Globalization;
using System.Drawing;
 
using Microsoft.Ajax.Utilities;
using antiSQLInjection;
using static iTextSharp.text.pdf.AcroFields;
using Newtonsoft.Json.Linq;

namespace Calyx_Solutions.Service
{
    public class srvLogin
    {
        

        static int cnt = 0;
        static byte captcnt = 0;
        string otp = "";
        static DataTable Static_Datatable = new DataTable("Static_Datatable")
        {
            Columns =
            {
                new DataColumn("Email", typeof(string)),
                new DataColumn("Count", typeof(int)),
                new DataColumn("cap_Count", typeof(int)),
                new DataColumn("Date", typeof(DateTime))
            }
        }; static DataTable Stat_cap_table = new DataTable("Stat_cap_table")
        {
            Columns =
            {
                new DataColumn("IPAdress", typeof(string)),
                new DataColumn("Count", typeof(int)),
                new DataColumn("Date", typeof(DateTime))
            }
        };
        public Int32 DocumentUploadCount(Model.Customer obj)
        {
            //Boolean _IsUpload = false;
            MySqlConnector.MySqlCommand cmd = new MySqlConnector.MySqlCommand("sp_check_document_upload");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("_customerId", obj.Id);

            int i_Cnt = Convert.ToInt32(db_connection.ExecuteScalarProcedure( cmd ));
            cmd.Dispose();

            return i_Cnt;
        }
        public DataTable IsValidEmail(Model.Login obj)
        {
            HttpContext httpContext = null;
            DataTable dt = null;
            List<Model.Customer> _lst = new List<Model.Customer>();
            string chk_mail = validation.validate(obj.UserName, 1, 1, 1, 1, 0, 1, 1, 1, 1);
            if (chk_mail != "false")
            {
                string user = CompanyInfo.testInjection(obj.UserName);
                CompanyInfo.InsertrequestLogTracker("IsValidEmail values: " + Convert.ToString(obj.UserName), 0, 0, 0, 0, "IsValidEmail", Convert.ToInt32(0), Convert.ToInt32(0), "", httpContext);
                if (obj.UserName.Contains(" "))
                {
                    obj.UserName = obj.UserName.Replace(" ", "");
                }
                if (user != "0")
                {
                    MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("CustomerDetailsByEmail");
                    _cmd.CommandType = CommandType.StoredProcedure;
                    _cmd.Parameters.AddWithValue("_Email_ID", obj.UserName.Trim());
                    _cmd.Parameters.AddWithValue("_Client_ID", obj.Client_ID);

                    dt = db_connection.ExecuteQueryDataTableProcedure(_cmd);
                }
            }
            else
            {
                return dt;
            }
            return dt;
        }

        public DataTable getsecurity_details(Model.Login obj)
        {
            List<Model.Permission> _lst = new List<Model.Permission>();
            DataTable dt1 = new DataTable();
            string query = "select passcode_flag from customer_mapping where Customer_ID = '" + CompanyInfo.Decrypt(obj.Customer_ID, true) + "';";

            MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("Default_SP");//"select    cast(AES_DECRYPT(UNHEX(u.Password), '" + obj.SecurityKey.Trim() + "' ) as  char(500)) as Password,u.First_Name,u.Last_Name,u.Customer_ID,U.WireTransfer_ReferanceNo,u.Email_ID,u.Branch_ID,u.Client_ID from customer_registration u where    Email_ID = '" + obj.Name.Trim() + "' and Password=HEX(AES_ENCRYPT('" + obj.Password.Trim() + "','" + obj.SecurityKey.Trim() + "'))  and u.delete_status=0 ;");
            _cmd.CommandType = CommandType.StoredProcedure;
            _cmd.Parameters.AddWithValue("_Query", query);
            dt1 = db_connection.ExecuteQueryDataTableProcedure(_cmd);
            return dt1;
        }

        public DataTable SetAuthentication(Model.Login obj)
        {
            List<Model.Permission> _lst = new List<Model.Permission>();
            DataTable dt1 = new DataTable();
            string encrypted_key = "";
            dt1.Columns.Add("Isvalid", typeof(int));
            dt1.Columns.Add("Encrypted_key", typeof(string));
            MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("sp_check_login");//"select    cast(AES_DECRYPT(UNHEX(u.Password), '" + obj.SecurityKey.Trim() + "' ) as  char(500)) as Password,u.First_Name,u.Last_Name,u.Customer_ID,U.WireTransfer_ReferanceNo,u.Email_ID,u.Branch_ID,u.Client_ID from customer_registration u where    Email_ID = '" + obj.Name.Trim() + "' and Password=HEX(AES_ENCRYPT('" + obj.Password.Trim() + "','" + obj.SecurityKey.Trim() + "'))  and u.delete_status=0 ;");
            _cmd.CommandType = CommandType.StoredProcedure;
            _cmd.Parameters.AddWithValue("_securityKey", CompanyInfo.SecurityKey());
            _cmd.Parameters.AddWithValue("_loginName", obj.UserName.Trim());
            //_cmd.Parameters.AddWithValue("_password", obj.Password.Trim());
            _cmd.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
            DataTable dt = db_connection.ExecuteQueryDataTableProcedure(_cmd);
            if (dt != null && dt.Rows.Count == 1)
            {
                if (obj.Password.Trim() == Convert.ToString(dt.Rows[0]["PASSWORD"]))
                {
                    foreach (DataRow row in dt.Rows)
                    {


                        if (obj.flag == 2)
                        {
                            encrypted_key = CompanyInfo.Encrypt("Email:" + Convert.ToString(Convert.ToString(dt.Rows[0]["Email_ID"])) + ",Password:" + Convert.ToString(Convert.ToString(dt.Rows[0]["PASSWORD"])), true);
                        }
                        else if (obj.flag == 3 || obj.flag == 1)
                        {
                            _cmd = new MySqlConnector.MySqlCommand("Update_passcode");
                            _cmd.CommandType = CommandType.StoredProcedure;
                            _cmd.Parameters.AddWithValue("_Passcode", obj.Passcode);
                            _cmd.Parameters.AddWithValue("_SecurityKey", srvCommon.SecurityKey());
                            _cmd.Parameters.AddWithValue("_flag", obj.flag);
                            _cmd.Parameters.AddWithValue("_Customer_ID", Convert.ToString(dt.Rows[0]["Customer_ID"]));
                            int a = db_connection.ExecuteNonQueryProcedure(_cmd);
                        }
                        dt1.Rows.Add(0, encrypted_key);
                    }
                }
                else
                {
                    dt1.Rows.Add(1, encrypted_key);

                }
            }
            else
            {
                dt1.Rows.Add(1, encrypted_key);

            }
            return dt1;
        }
        public List<Model.Customer> IsValidCustomer(Model.Login obj, HttpContext context)
        {
            int chk_verify_status = 0;
            int sttic_cnt = 0;
            int sttic_cap_cnt = 0;
            int passcode_flag = 1;
            List<Model.Customer> _lst = new List<Model.Customer>();
            Model.Customer _obj = new Model.Customer();
            string email = obj.UserName;
            string pwd = obj.Password;

            string ipAddress = context.Connection.RemoteIpAddress.ToString();
            string capIp = ipAddress; // HttpContext.Current.Request.UserHostAddress;


            string Loginct_IPAdress = capIp;
            string Loginct_Email = email;
            int Loginct_IpAddress_count = 0;
            DateTime Loginct_Date = DateTime.Now;
            DateTime Loginip_Date = DateTime.Now;
            int Loginct_email_count = 0;
            MySqlConnector.MySqlCommand _cmdh = new MySqlConnector.MySqlCommand();
            try
            {
                _cmdh = new MySqlConnector.MySqlCommand("setLoginCounts");
                _cmdh.CommandType = CommandType.StoredProcedure;
                _cmdh.Parameters.AddWithValue("_Email", Loginct_Email);
                _cmdh.Parameters.AddWithValue("_IPAdress", Loginct_IPAdress);
                _cmdh.Parameters.AddWithValue("_email_count", Loginct_email_count);
                _cmdh.Parameters.AddWithValue("_IpAddress_count", Loginct_IpAddress_count);
                _cmdh.Parameters.AddWithValue("_Date", Loginct_Date);
                DataTable journerydata = db_connection.ExecuteQueryDataTableProcedure(_cmdh);
                if (journerydata.Rows.Count > 0)
                {
                    if (journerydata.Rows[0]["Email"] != DBNull.Value)
                    {
                        Loginct_Email = Convert.ToString(journerydata.Rows[0]["Email"]);
                    }
                    if (journerydata.Rows[1]["IPAdress"] != DBNull.Value)
                    {
                        Loginct_IPAdress = Convert.ToString(journerydata.Rows[1]["IPAdress"]);
                    }
                    if (journerydata.Rows[0]["email_count"] != DBNull.Value)
                    {
                        Loginct_email_count = Convert.ToInt32(journerydata.Rows[0]["email_count"]);
                    }
                    if (journerydata.Rows[1]["IpAddress_count"] != DBNull.Value)
                    {
                        Loginct_IpAddress_count = Convert.ToInt32(journerydata.Rows[1]["IpAddress_count"]);
                    }
                    if (journerydata.Rows[0]["Date"] != DBNull.Value)
                    {
                        Loginct_Date = Convert.ToDateTime(journerydata.Rows[0]["Date"]);
                    }
                    if (journerydata.Rows[1]["Date"] != DBNull.Value)
                    {
                        Loginip_Date = Convert.ToDateTime(journerydata.Rows[1]["Date"]);
                    }


                }
            }
            catch (Exception ex) { }

            try
            {
                string checklink = "";
                checklink = $"{context.Request.Path}" + $"{context.Request.QueryString}";
                MySqlConnector.MySqlCommand _cmdl = new MySqlConnector.MySqlCommand("GetPermissions");
                _cmdl.CommandType = CommandType.StoredProcedure;
                _cmdl.Parameters.AddWithValue("_whereclause", " and PID in (61,102,108,109,111,112,113,135,136,10,211)");
                _cmdl.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                db_connection objdbConn = new db_connection();
                DataTable all_per = db_connection.ExecuteQueryDataTableProcedure(_cmdl);
                DataTable dt5 = new DataTable();

                int chkdailycount = 1;
                DataRow[] li1_captcha = all_per.Select("PID=102");
                DataRow[] li1 = all_per.Select("PID=61");
                DataRow[] li1_captcha1 = all_per.Select("PID=61");
                DataRow[] chk_loc_perm1 = all_per.Select("PID=111");
                DataRow[] chk_loc_perm2 = all_per.Select("PID=112");
                DataRow[] chk_loc_perm3 = all_per.Select("PID=113");
                DataRow[] paascode_list = all_per.Select("PID=135");//passcode
                DataRow[] biometric_list = all_per.Select("PID=136");//biometric
                DataRow[] Primary_ID_perm = all_per.Select("PID=10");//primary id
                DataRow[] chk_country_loc_perm = all_per.Select("PID=211"); //Check Country Location
                int Primary_ID_perm1 = Convert.ToInt32(Primary_ID_perm[0]["Status_ForCustomer"]);
                int Biometric_per = Convert.ToInt32(biometric_list[0]["Status_ForCustomer"]);
                int Passcode_per = Convert.ToInt32(paascode_list[0]["Status_ForCustomer"]);

                //string input_keyexample = CompanyInfo.Encrypt(Convert.ToString(obj.Password).Trim(), true)  ;

                if (obj.CredentialFlag != "" && obj.CredentialFlag != null && obj.CredentialFlag == "2" && Biometric_per == 0)
                {   // This code for Biometric login
                    string input_key = CompanyInfo.Decrypt(obj.Password, Convert.ToBoolean(1));
                    pwd = CompanyInfo.Decrypt(obj.Password, Convert.ToBoolean(1));
                    // Define regular expressions to match "Email" and "Password" patterns
                    Regex emailRegex = new Regex(@"Email:(.*?),");
                    Regex passwordRegex = new Regex(@"Password:(.*?)$");

                    // Find email and password matches
                    Match emailMatch = emailRegex.Match(input_key);
                    Match passwordMatch = passwordRegex.Match(input_key);

                    // Extract email and password values                 
                    obj.Password = passwordMatch.Success ? passwordMatch.Groups[1].Value.Trim() : null;
                    //obj.Password = pwd;                
                }
                try
                {
                    if (Loginct_Date.Date != DateTime.Now.Date)
                    {
                        Loginct_Date = DateTime.Now;
                        Loginct_email_count = 0;
                    }
                }
                catch { }

                /* if (obj.CredentialFlag != "" && obj.CredentialFlag != null && obj.CredentialFlag == "2" && Biometric_per == 0)
                 {   // This code for Biometric login
                     string input_key = CompanyInfo.Decrypt(obj.Password, Convert.ToBoolean(1));
                     // Define regular expressions to match "Email" and "Password" patterns
                     Regex emailRegex = new Regex(@"Email:(.*?),");
                     Regex passwordRegex = new Regex(@"Password:(.*?)$");

                     // Find email and password matches
                     Match emailMatch = emailRegex.Match(input_key);
                     Match passwordMatch = passwordRegex.Match(input_key);

                     // Extract email and password values
                     obj.UserName = emailMatch.Success ? emailMatch.Groups[1].Value.Trim() : null;
                     obj.Password = passwordMatch.Success ? passwordMatch.Groups[1].Value.Trim() : null;
                     email = obj.UserName;
                     pwd = obj.Password;
                 }*/

                if (Static_Datatable.Rows.Count > 0)
                {
                    if (Convert.ToDateTime(Static_Datatable.Rows[0]["Date"]).Date != DateTime.Now.Date)
                    {
                        Static_Datatable.Clear();
                    }
                }

                if (Static_Datatable.Rows.Count > 0)
                {

                    DataRow[] dr = Static_Datatable.Select("Email='" + obj.UserName.Trim() + "'");
                    if (dr.Count() > 0)
                    {
                        //foreach (DataRow drr in dr)
                        //{
                        //    //string static_email = Convert.ToString(drr["Email"]);
                        //    if (Convert.ToString(drr["Email"]) == obj.UserName.Trim())
                        //    {
                        //        drr["Count"] = Convert.ToInt32(drr["Count"]) + 1;
                        //        sttic_cnt = Convert.ToInt32(drr["Count"]);

                        //    }
                        //}
                    }
                    else
                    {

                        Static_Datatable.Rows.Add(obj.UserName.Trim(), 0, 0, DateTime.Now);

                    }
                    //context.Session["Count"] = Convert.ToInt32(context.Session["Count"])+1;
                }
                else
                {
                    ////context.Session = new HttpSessionStateWrapper(new HttpSessionStateContainer());
                    //HttpSessionState Session = HttpContext.Current.Session;
                    //Session["FirstName"] = 1;
                    //context.Session["Count"] = 1;

                    Static_Datatable.Rows.Add(obj.UserName.Trim(), 0, 0, DateTime.Now);
                }
                if (Stat_cap_table.Rows.Count > 0)
                {
                    if (Convert.ToDateTime(Stat_cap_table.Rows[0]["Date"]).Date != DateTime.Now.Date)
                    {
                        Stat_cap_table.Clear();
                    }
                }

                if (Stat_cap_table.Rows.Count > 0)
                {

                    DataRow[] dr = Stat_cap_table.Select("IPAdress='" + capIp + "'");
                    if (dr.Count() > 0)
                    {
                    }
                    else
                    {
                        Stat_cap_table.Rows.Add(capIp, 0, DateTime.Now);
                    }
                }
                else
                {

                    Stat_cap_table.Rows.Add(capIp, 0, DateTime.Now);
                }

                if (Convert.ToInt32(li1_captcha[0]["PID"]) == 102 && Convert.ToInt32(li1_captcha[0]["Status_ForCustomer"]) == 0) //Captcha login check condition
                {
                    string reCaptchaKey = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("Jwt")["reCaptchaKey"];
                    var secretKey = CompanyInfo.Decrypt(reCaptchaKey, Convert.ToBoolean(1));
                    if (!checklink.Contains("/token"))
                    {
                        try
                        {
                            if (context.Session != null)
                            {
                                context.Session.SetString("scrutiny", null);

                                /*context.Session["scrutiny"] = null; 
                                HttpContext.Current = context;*/
                            }
                        }
                        catch (Exception ex) { }
                    }
                    //if (checklink.Contains("/token"))
                    if (obj.tokenRequest == "F" || obj.tokenRequest == "f")
                    {
                        try
                        {
                            Loginct_IpAddress_count = Convert.ToInt32(Loginct_IpAddress_count) + 1;
                            sttic_cap_cnt = Convert.ToInt32(Loginct_IpAddress_count);
                        }
                        catch { }

                        string reCaptcha = obj.Captcha;
                        string token = obj.token;
                        /*if (Stat_cap_table.Rows.Count > 0)
                        {

                            DataRow[] dr = Stat_cap_table.Select("IPAdress='" + capIp + "'");
                            if (dr.Count() > 0)
                            {
                                foreach (DataRow drr in dr)
                                {
                                    //string static_email = Convert.ToString(drr["Email"]);
                                    if (Convert.ToString(drr["IPAdress"]) == capIp)
                                    {
                                        drr["Count"] = Convert.ToInt32(drr["Count"]) + 1;
                                        sttic_cap_cnt = Convert.ToInt32(drr["Count"]);

                                    }
                                }
                            }
                            else
                            {
                                Stat_cap_table.Rows.Add(capIp, 1, DateTime.Now);
                            }

                        }*/
                        // captcnt++;
                        //Boolean checkCaptcha = ReCaptchaPassed(secretKey, reCaptcha);

                        Boolean checkCaptcha = true;
                        try
                        {
                            //checkCaptcha = ReCaptchaPassed(secretKey, reCaptcha);
                            Service.srvCaptcha srv = new Service.srvCaptcha();
                            checkCaptcha = srv.VerifyCaptcha_withUserName(obj.UserName, reCaptcha);
                            int stattus1 = (int)CompanyInfo.InsertActivityLogDetails(" Captcha response value: " + Convert.ToString(checkCaptcha), 0, 0, Convert.ToInt32(0), 1, "IsValidCustomer", Convert.ToInt32(0), Convert.ToInt32(0), "IsValidCustomer", context);

                            /*if (reCaptcha.Trim() != "" || reCaptcha.Trim() != null)
                            {
                                if (!checkCaptcha && obj.flutterval == ""  )
                                {
                                    _obj.Flag = 2;
                                    _obj.ErrorFlag = "2";
                                    _lst.Add(_obj);
                                    return _lst;
                                }
                            }
                            if (reCaptcha.Trim() == "" || reCaptcha.Trim() == null)
                            {
                                _obj.Flag = 2;
                                _obj.ErrorFlag = "2";
                                _lst.Add(_obj);
                                return _lst;
                            }*/

                        }
                        catch (Exception ex_captcha)
                        {
                            DateTime Record_Insert_DateTime = DateTime.Now;
                            string error = ex_captcha.ToString().Replace("\'", "\\'");
                            MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("SaveException");
                            _cmd.CommandType = CommandType.StoredProcedure;
                            _cmd.Parameters.AddWithValue("_Record_Insert_DateTime", Record_Insert_DateTime);
                            _cmd.Parameters.AddWithValue("_error", error);
                            _cmd.Parameters.AddWithValue("_Client_ID", 1);
                            int msg = db_connection.ExecuteNonQueryProcedure(_cmd);
                        }

                        if (sttic_cap_cnt > 2 && obj.flutterval == "0" && obj.Captcha == "")
                        {
                            obj.Captcha = "true";
                            _obj.Flag = 2;
                            _obj.ErrorFlag = "2";
                            _lst.Add(_obj);
                            return _lst;
                        }
                        else if (!checkCaptcha && sttic_cap_cnt > 2 && (obj.flutterval == ""  || obj.flutterval == null)) // && obj.flutterval == ""
                        {
                            obj.Captcha = "true";
                            _obj.Flag = 2;
                            _obj.ErrorFlag = "2";
                            _lst.Add(_obj);
                            return _lst;
                        }
                        else
                        {
                            _obj.ErrorFlag = "";

                        }
                        //else if (checkCaptcha)
                        //{
                        //    captcnt = 0;
                        //}

                    }
                }
                Regex rgx_email = new Regex(@"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z");
                Regex rgx_pass = new Regex(@"\[^'  / & #,]/");
                var resemail = rgx_email.IsMatch(email.ToLower());
                var respass = rgx_pass.IsMatch(pwd);


                if (resemail == false || pwd.Contains(" ") || pwd.Contains("-- ") || pwd.Contains(";") || pwd.Contains("/*") || pwd.Contains(" #") || pwd.Contains("# "))
                {
                    obj.UserName = "";
                    _obj.Name = _obj.FirstName + " " + _obj.LastName;
                    _obj.Flag = 1;
                    cnt += 1;
                    if (Static_Datatable.Rows.Count > 0)
                    {

                        DataRow[] dr = Static_Datatable.Select("Email='" + obj.UserName.Trim() + "'");
                        if (dr.Count() > 0)
                        {
                            foreach (DataRow drr in dr)
                            {
                                //string static_email = Convert.ToString(drr["Email"]);
                                if (Convert.ToString(drr["Email"]) == obj.UserName.Trim())
                                {
                                    drr["Count"] = Convert.ToInt32(drr["Count"]) + 1;
                                    sttic_cnt = Convert.ToInt32(drr["Count"]);

                                }
                            }
                        }
                        else
                        {

                            Static_Datatable.Rows.Add(obj.UserName.Trim(), 1, DateTime.Now);

                        }

                    }
                    //context.Session["Count"] = Convert.ToInt32(context.Session["Count"]) + 1;
                    if (sttic_cnt >= 4)
                    {
                        string msg = "Multiple attempts of invalid data. User Name: " + email + " Password: " + pwd + "";
                        _ = CompanyInfo.InsertActivityLogDetailsSecurityasync(msg, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), 1, "IsValidCustomer", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "IsValidCustomer", 2, context);
                        _lst.Add(_obj);
                        return _lst;
                    }
                    return _lst;
                }
                else
                {

                    string user = CompanyInfo.testInjection((obj.UserName));
                    string pass = CompanyInfo.testInjection((obj.Password));
                    string cid = CompanyInfo.testInjection(Convert.ToString(obj.Client_ID));

                    if (user == "1" && pass == "1" && cid == "1")
                    {
                        //_ = CompanyInfo.InsertActivityLogDetailsasync("Login Details: "+ obj.Client_ID + " Username: "+ obj.UserName +" and Password: "+ obj.Password, 0, 0, 0, 0, "IsValidCustomer", 0, 1, "");
                        //DataTable dts = CompanyInfo.getsecutityflag(obj.UserName, obj.Client_ID, obj.Branch_ID);
                        //if (dts.Rows.Count == 1 && Convert.ToString(dts.Rows[0]["Security_Flag"]) != "" && Convert.ToString(dts.Rows[0]["Security_Flag"]) == "0")
                        //{
                        //    _obj.Flag = 1;
                        //    _lst.Add(_obj);
                        //    return _lst;
                        //}
                        //else
                        //{
                        MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("sp_check_login");//"select    cast(AES_DECRYPT(UNHEX(u.Password), '" + obj.SecurityKey.Trim() + "' ) as  char(500)) as Password,u.First_Name,u.Last_Name,u.Customer_ID,U.WireTransfer_ReferanceNo,u.Email_ID,u.Branch_ID,u.Client_ID from customer_registration u where    Email_ID = '" + obj.Name.Trim() + "' and Password=HEX(AES_ENCRYPT('" + obj.Password.Trim() + "','" + obj.SecurityKey.Trim() + "'))  and u.delete_status=0 ;");
                        _cmd.CommandType = CommandType.StoredProcedure;
                        _cmd.Parameters.AddWithValue("_securityKey", CompanyInfo.SecurityKey());
                        _cmd.Parameters.AddWithValue("_loginName", obj.UserName.Trim());
                        //_cmd.Parameters.AddWithValue("_password", obj.Password.Trim());
                        _cmd.Parameters.AddWithValue("_Client_ID", obj.Client_ID);

                        DataTable dt = db_connection.ExecuteQueryDataTableProcedure(_cmd);
                        if (dt != null && dt.Rows.Count == 1)
                        {
                            //DataTable li1 = (DataTable)CompanyInfo.getEmailPermission(obj.Client_ID, 61);
                            string Status = Convert.ToString(li1[0]["Status_ForCustomer"]);
                            //if (obj.Password.Trim() == Convert.ToString(dt.Rows[0]["PASSWORD"]))

                            if ((obj.Password.Trim() == Convert.ToString(dt.Rows[0]["PASSWORD"]) && (obj.CredentialFlag == "" || obj.CredentialFlag == null || obj.CredentialFlag != "1")) ||
                                (obj.Password.Trim() == Convert.ToString(dt.Rows[0]["PASSCODE"]) && obj.CredentialFlag != "" && obj.CredentialFlag != null && obj.CredentialFlag == "1" && Passcode_per == 0)
                                )
                            {
                                try
                                {
                                    Check_Ip(dt, context);
                                }
                                catch { }
                                if (dt.Rows.Count == 1 && Convert.ToString(dt.Rows[0]["Security_Flag"]) != "" && Convert.ToString(dt.Rows[0]["Security_Flag"]) == "0")
                                {
                                    _obj.Flag = 1; _lst.Add(_obj); return _lst;
                                }

                                foreach (DataRow row in dt.Rows)
                                {
                                    string emailu = "";
                                    _obj = new Model.Customer();
                                    _obj.UserName = obj.UserName.Trim();
                                    _obj.Password = obj.Password.Trim();
                                    //if (row["Password"] != DBNull.Value)
                                    //{
                                    //    _obj.Password = (row["Password"].ToString());
                                    //}

                                    //  if (checklink.Contains("/token") && Passcode_per == 0)
                                    if (obj.tokenRequest == "F" && Passcode_per == 0)
                                    {
                                        if (dt.Rows[0]["InvalidLogin_attempt"] != DBNull.Value && Convert.ToString(dt.Rows[0]["InvalidLogin_attempt"]) != "")
                                        {
                                            if (Convert.ToInt32(dt.Rows[0]["InvalidLogin_attempt"]) > 3)
                                            {
                                                if (dt.Rows[0]["LoginAttempt_date"] != DBNull.Value && Convert.ToString(dt.Rows[0]["LoginAttempt_date"]) != "")
                                                {
                                                    if (Convert.ToDateTime(CompanyInfo.gettime(obj.Client_ID, context)).Date == Convert.ToDateTime(Convert.ToString(dt.Rows[0]["LoginAttempt_date"])).Date)
                                                    {
                                                        string msg = "Customer is blocked because of the previous invalid passcode attempts";
                                                        _ = CompanyInfo.InsertActivityLogDetailsSecurityasync(msg, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), 1, "IsValidCustomer", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "IsValidCustomer", 2, context);
                                                        _obj.Flag = 7;
                                                        if (_lst.Count == 0)
                                                        {
                                                            _lst.Add(_obj);
                                                        }
                                                        return _lst;
                                                    }

                                                }

                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (dt.Rows[0]["InvalidLogin_attempt"] != DBNull.Value && Convert.ToString(dt.Rows[0]["InvalidLogin_attempt"]) != "")
                                        {
                                            if (Convert.ToInt32(dt.Rows[0]["InvalidLogin_attempt"]) != 0)
                                            {
                                                string query = "UPDATE customer_mapping SET InvalidLogin_attempt = '0' WHERE Customer_Id = '" + row["Customer_ID"] + "';";
                                                MySqlConnector.MySqlCommand _cmddef = new MySqlConnector.MySqlCommand("Default_SP");
                                                _cmddef.CommandType = CommandType.StoredProcedure;
                                                _cmddef.Parameters.AddWithValue("_Query", query);
                                                db_connection.ExecuteQueryDataTableProcedure(_cmddef);
                                            }
                                        }

                                    }

                                    if (row["reg_step"] != DBNull.Value)
                                    {
                                        _obj.customerRegStep = Convert.ToInt32(row["reg_step"]);
                                    }
                                    if (row["First_Name"] != DBNull.Value)
                                    {
                                        _obj.FirstName = (row["First_Name"].ToString());
                                    }
                                    if (row["Last_Name"] != DBNull.Value)
                                    {
                                        _obj.LastName = (row["Last_Name"].ToString());
                                    }

                                    _obj.payout_countries = Convert.ToString(row["payout_countries"]);
                                    _obj.customer_profileimage = Convert.ToString(row["Profile_Image"]);

                                    if (row["passcode_flag"] != DBNull.Value && Passcode_per == 0)
                                    {
                                        passcode_flag = Convert.ToInt32(row["passcode_flag"]);
                                        _obj.passcode_flag = Convert.ToInt32(row["passcode_flag"]);
                                    }
                                    else
                                    {
                                        _obj.passcode_flag = 1;
                                    }

                                    if (row["Customer_ID"] != DBNull.Value)
                                    {
                                        _obj.Id = Convert.ToString(row["Customer_ID"]);
                                        if (!checklink.Contains("/token"))
                                        {

                                            //context.Session["Customer_ID"] = Convert.ToInt32(_obj.Id);
                                            /*CompanyInfo.setSessionHttpValStr("Customer_ID", Convert.ToString(_obj.Id), context);


                                            DataTable dt1 = CompanyInfo.get(obj.Client_ID, context);//Vishvesh
                                            MySqlConnector.MySqlCommand cmd = new MySqlConnector.MySqlCommand("getTxnRef_InitialChar");
                                            cmd.CommandType = CommandType.StoredProcedure;
                                            int size = Convert.ToInt32(dt1.Rows[0]["trn_ref_no_length"].ToString());
                                            var rng = new Random(Environment.TickCount);
                                            cmd.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                                            cmd.Parameters.AddWithValue("_CB_ID", obj.Branch_ID);
                                            string initialchars = Convert.ToString(db_connection.ExecuteScalarProcedure(cmd));
                                            var refNo = initialchars + string.Concat(Enumerable.Range(0, size).Select((index) => rng.Next(10).ToString()));
                                            CompanyInfo.setSessionHttpValStr("scrutiny", Convert.ToString(refNo), context);*/
                                            //refNo1 = Convert.ToString(refNo);
                                            //context.Session["scrutiny"] = Convert.ToString(refNo);//Vishvesh

                                        }
                                    }
                                    if (row["WireTransfer_ReferanceNo"] != DBNull.Value)
                                    {
                                        _obj.WireTransfer_ReferanceNo = (row["WireTransfer_ReferanceNo"].ToString());
                                    }
                                    if (row["Email_ID"] != DBNull.Value)
                                    {
                                        _obj.Email = (row["Email_ID"].ToString());
                                        emailu = Convert.ToString(_obj.Email).ToLower();
                                    }
                                    if (row["block_login_flag"] != DBNull.Value)
                                    {
                                        _obj.block_login_flag = Convert.ToInt32(row["block_login_flag"].ToString());
                                    }
                                    if (row["Client_ID"] != DBNull.Value)
                                    {
                                        _obj.Client_ID = Convert.ToInt32(row["Client_ID"].ToString());
                                    }
                                    if (row["Mobile_Verification_flag"] != DBNull.Value)
                                    {
                                        _obj.Mobile_Verification_flag = Convert.ToInt32(row["Mobile_Verification_flag"].ToString());
                                    }
                                    else
                                    {
                                        _obj.Mobile_Verification_flag = 1;
                                    }
                                    if (row["Country_Flag"] != DBNull.Value)
                                    {
                                        _obj.Country_Flag = row["Country_Flag"].ToString();
                                    }
                                    else
                                    {
                                        _obj.Country_Flag = "";
                                    }
                                    if (row["Currency_Sign"] != DBNull.Value)
                                    {
                                        _obj.Currency_Sign = row["Currency_Sign"].ToString();
                                    }
                                    else
                                    {
                                        _obj.Currency_Sign = "";
                                    }
                                    if (row["Currency_Code"] != DBNull.Value)
                                    {
                                        _obj.Currency_Code = row["Currency_Code"].ToString();
                                    }
                                    else
                                    {
                                        _obj.Currency_Code = "";
                                    }
                                    if (row["Currency_ID"] != DBNull.Value)
                                    {
                                        _obj.Currency_Id = Convert.ToInt32(row["Currency_ID"].ToString());
                                    }
                                    else
                                    {
                                        _obj.Currency_Id = 0;
                                    }
                                    if (row["Country_Name"] != DBNull.Value)
                                    {
                                        _obj.Country_Name = row["Country_Name"].ToString();
                                    }
                                    else
                                    {
                                        _obj.Country_Name = "";
                                    }
                                    if (row["Country_ID"] != DBNull.Value)
                                    {
                                        _obj.Country_Id = Convert.ToInt32(row["Country_ID"]);
                                    }
                                    else
                                    {
                                        _obj.Country_Id = 0;
                                    }
                                    if (row["User_ID"] != DBNull.Value && Convert.ToString(row["User_ID"]) != "")
                                    {
                                        _obj.Agent_User_ID = Convert.ToString(row["User_ID"]);
                                    }
                                    int stattus1 = (int)CompanyInfo.InsertActivityLogDetails("Agent_User_ID: " + _obj.Agent_User_ID, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), 1, "IsValidCustomer", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "IsValidCustomer", context);

                                    if (Convert.ToInt32(_obj.Agent_User_ID) != 0 && _obj.Agent_User_ID != null)
                                    {
                                        try
                                        {
                                            MySqlConnector.MySqlCommand _cmd1 = new MySqlConnector.MySqlCommand("Get_Cust_Info");
                                            _cmd1.CommandType = CommandType.StoredProcedure;
                                            if (_obj.Agent_User_ID != "0" && _obj.Agent_User_ID != null)
                                            {
                                                _cmd1.Parameters.AddWithValue("_User_ID", _obj.Agent_User_ID);
                                            }
                                            else
                                            {
                                                _cmd1.Parameters.AddWithValue("_User_ID", null);
                                            }
                                            _cmd1.Parameters.AddWithValue("_client_ID", _obj.Client_ID);
                                            _cmd1.Parameters.AddWithValue("_customer_Id", _obj.Id);
                                            dt5 = db_connection.ExecuteQueryDataTableProcedure(_cmd1);
                                            int stattus2 = (int)CompanyInfo.InsertActivityLogDetails("DataTable : " + dt5.Rows.Count, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), 1, "IsValidCustomer", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "IsValidCustomer", context);

                                            if (dt5.Rows.Count > 0)
                                            {
                                                if (dt5.Rows[0]["Agent_branch"] != null && Convert.ToString(dt5.Rows[0]["Agent_branch"]) != "")
                                                {
                                                    _obj.Agent_branch = Convert.ToString(dt5.Rows[0]["Agent_branch"]);
                                                }
                                                if (dt5.Rows[0]["Branch_ID"] != null)
                                                {
                                                    _obj.New_Agent_Branch = Convert.ToString(dt5.Rows[0]["Branch_ID"]);
                                                }
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            int stattus = (int)CompanyInfo.InsertActivityLogDetails(ex.ToString(), Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), 1, "IsValidCustomer", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "IsValidCustomer", context);

                                        }
                                    }

                                     
                                    if (row["Multiple_attempt"] != DBNull.Value && Status == "0")
                                    {
                                        String multi_att_activity = "";
                                        string intValue = Convert.ToString(dt.Rows[0]["Customer_ID"]);
                                        int intValue1 = Convert.ToInt32(dt.Rows[0]["Country_ID"]);
                                        String Encrypted_time1 = Convert.ToString(CompanyInfo.gettime(obj.Client_ID, intValue, intValue1, context));
                                        try
                                        {
                                            _obj.OTP = Convert.ToString(row["Multiple_attempt"]);
                                            multi_att_activity = multi_att_activity + "Multiple attempt flag : " + _obj.OTP;
                                            
                                            if (Convert.ToInt32(row["Multiple_attempt"]) == 0 && (obj.OTP == null || obj.OTP == "") && (checklink.Contains("/token") || obj.tokenRequest == "F"))
                                            {
                                                chk_verify_status++;
                                                int check_first_login = 1; //131023
                                                int otpLength = 6;
                                                otp = GenerateOTP(otpLength);

                                                //Parth Change for calling function GetLastOtpForCustomer and assigning necessary values
                                                #region Calling function GetLastOtpForCustomer and assigning necessary values
                                                // Get the last OTP for the customer
                                                double differenceInMinutes = 1;
                                                string lastotp = "";
                                                try
                                                {
                                                    DataTable lastOtpData = GetLastOtpForCustomer(obj.UserName);
                                                    DateTime otpExpiryTime = DateTime.MinValue; // Default OTP expiry time if no data is found

                                                    // Check if the result contains any rows
                                                    if (lastOtpData != null && lastOtpData.Rows.Count > 0)
                                                    {
                                                        otpExpiryTime = Convert.ToDateTime(lastOtpData.Rows[0]["OTP_Expiry"]);  // Extract the OTP expiry time if data exists
                                                        lastotp = lastOtpData.Rows[0]["OTP"].ToString();    // Extract the last OTP if data exists
                                                    }
                                                    DateTime dateTime = Convert.ToDateTime(Encrypted_time1).AddMinutes(15); //Encrypting current datetime according to country timezone

                                                    differenceInMinutes = dateTime.Subtract(otpExpiryTime).TotalMinutes; //Substracting current datetime to user's last otp datetime
                                                    if (differenceInMinutes < 1 && !string.IsNullOrEmpty(lastotp))
                                                    {
                                                        otp = lastotp; // Reusing the same OTP
                                                    }
                                                }
                                                catch(Exception ex)
                                                {
                                                    DateTime Record_Insert_DateTime = DateTime.Now;
                                                    string error = ex.ToString().Replace("\'", "\\'");
                                                    _cmdh = new MySqlConnector.MySqlCommand("SaveException");
                                                    _cmdh.CommandType = CommandType.StoredProcedure;
                                                    _cmdh.Parameters.AddWithValue("_Record_Insert_DateTime", Record_Insert_DateTime);
                                                    _cmdh.Parameters.AddWithValue("_error", error);
                                                    _cmdh.Parameters.AddWithValue("_Client_ID", 1);
                                                    int msg = db_connection.ExecuteNonQueryProcedure(_cmdh);
                                                }
                                                #endregion
                                                //End Parth change for calling function GetLastOtpForCustomer and assigning necessary values

                                                // Console.WriteLine("Random OTP: " + otp);
                                                multi_att_activity = multi_att_activity + " | OTP generated : " + otp;
                                                if (obj.Resend_otp != "Resend") //131023
                                                {

                                                    MySqlConnector.MySqlCommand _cmd_check1 = new MySqlConnector.MySqlCommand("save_new_login_token");

                                                    _cmd_check1.CommandType = CommandType.StoredProcedure;
                                                    _cmd_check1.Parameters.AddWithValue("_OTP", otp);
                                                    _cmd_check1.Parameters.AddWithValue("_OTP_Expiry", Convert.ToDateTime(Encrypted_time1).AddMinutes(15));
                                                    _cmd_check1.Parameters.AddWithValue("_Customer_ID", dt.Rows[0]["Customer_ID"]);
                                                    _cmd_check1.Parameters.AddWithValue("_flag", 0);
                                                    int n = db_connection.ExecuteNonQueryProcedure(_cmd_check1);
                                                }


                                                
                                                MySqlConnector.MySqlCommand _cmd_check = new MySqlConnector.MySqlCommand("sp_save_otp");

                                                _cmd_check.CommandType = CommandType.StoredProcedure;
                                                _cmd_check.Parameters.AddWithValue("_OTP", otp);
                                                _cmd_check.Parameters.AddWithValue("_OTP_Expiry", Convert.ToDateTime(Encrypted_time1).AddMinutes(15));
                                                _cmd_check.Parameters.AddWithValue("_Customer_ID", dt.Rows[0]["Customer_ID"]);
                                                _cmd_check.Parameters.Add(new MySqlConnector.MySqlParameter("setflag", MySqlConnector.MySqlDbType.String));
                                                _cmd_check.Parameters["setflag"].Direction = ParameterDirection.Output;
                                                DataTable d_t = db_connection.ExecuteQueryDataTableProcedure(_cmd_check);
                                                multi_att_activity = multi_att_activity + " | OTP saved to the Table for Customer_ID : " + dt.Rows[0]["Customer_ID"];
                                                check_first_login = Convert.ToInt32(d_t.Rows[0]["setflag"]);

                                                
                                                if (check_first_login == 1)
                                                {
                                                    
                                                    send_mail_on_invalidlogin(obj, dt, otp, context);
                                                    multi_att_activity = multi_att_activity + " | Email sent for OTP ";
                                                    _obj.Flag = 3;
                                                    _obj.ErrorFlag = "2";
                                                    _lst.Add(_obj);
                                                    
                                                    return _lst;
                                                }
                                                else
                                                {
                                                    if (obj.Resend_otp == "Resend") //131023
                                                    {
                                                        send_mail_on_invalidlogin(obj, dt, otp, context);
                                                        multi_att_activity = multi_att_activity + " | Email sent for OTP ";
                                                        _obj.Flag = 3;
                                                        _obj.ErrorFlag = "2";
                                                        _lst.Add(_obj);
                                                        return _lst;
                                                    }


                                                    _obj.Flag = 5;
                                                }



                                            }
                                            else if (Convert.ToInt32(row["Multiple_attempt"]) == 0 && obj.OTP != null && obj.OTP != "" && (checklink.Contains("/token") || obj.tokenRequest == "F"))
                                            {
                                                chk_verify_status++;
                                                multi_att_activity = multi_att_activity + " | Customer Entered OTP : " + obj.OTP;
                                                MySqlConnector.MySqlCommand _cmd_check = new MySqlConnector.MySqlCommand("chk_emailOtp_app");
                                                _cmd_check.CommandType = CommandType.StoredProcedure;
                                                _cmd_check.Parameters.AddWithValue("_Customer_ID", Convert.ToInt32(row["Customer_ID"]));
                                                _cmd_check.Parameters.AddWithValue("_OTP", Convert.ToInt32(obj.OTP));
                                                _cmd_check.Parameters.AddWithValue("_Date", Convert.ToDateTime(Encrypted_time1));
                                                DataTable dt1 = db_connection.ExecuteQueryDataTableProcedure(_cmd_check);
                                                if (dt1.Rows.Count == 1)
                                                {
                                                    multi_att_activity = multi_att_activity + " | we check OTP in datatable for time " + Encrypted_time1 + "  where OId : " + dt1.Rows[0]["OId"];
                                                    if (Convert.ToDateTime(dt1.Rows[0]["OTP_Expiry"]) >= Convert.ToDateTime(Encrypted_time1) && obj.OTP == Convert.ToString(dt1.Rows[0]["OTP"]))
                                                    {

                                                        MySqlConnector.MySqlCommand cmd = new MySqlConnector.MySqlCommand("update_blacklistflag");
                                                        cmd.CommandType = CommandType.StoredProcedure;
                                                        cmd.Parameters.AddWithValue("_Customer_ID", Convert.ToString(row["Customer_ID"]));
                                                        cmd.Parameters.AddWithValue("_Flag", 0);
                                                        int st = db_connection.ExecuteNonQueryProcedure(cmd);
                                                        _obj.block_login_flag = 0;
                                                        _obj.OTP = "";
                                                        multi_att_activity = multi_att_activity + " | OTP is correct, blacklisted and Multiple attempt flagis updated ";
                                                    }
                                                    else
                                                    {
                                                        multi_att_activity = multi_att_activity + " | Invalid or expired otp";
                                                        _obj.Flag = 4;
                                                        _obj.ErrorFlag = "2";
                                                        _lst.Add(_obj);
                                                        return _lst;
                                                    }
                                                }
                                                else
                                                {
                                                    multi_att_activity = multi_att_activity + " | Invalid or expired otp";

                                                    _obj.Flag = 4;
                                                    _obj.ErrorFlag = "2";
                                                    _lst.Add(_obj);
                                                    return _lst;
                                                }
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            multi_att_activity = multi_att_activity + " | Error occured " + ex.ToString();

                                        }
                                        finally
                                        {
                                            // _ = CompanyInfo.InsertActivityLogDetailsasync(multi_att_activity, 0,0,obj.User_ID, Convert.ToInt32(row["Customer_ID"]), "IsValidCustomer", obj.Branch_ID, obj.Client_ID,"");
                                            //Model.ActivityLogTracker _objActivityLog = new Model.ActivityLogTracker();
                                            //_objActivityLog.Activity = multi_att_activity;
                                            //_objActivityLog.FunctionName = "IsValidCustomer";
                                            //_objActivityLog.Transaction_ID = 0;
                                            //_objActivityLog.WhoAcessed = 0;
                                            //_objActivityLog.Branch_ID = obj.Branch_ID;
                                            //_objActivityLog.Client_ID = obj.Client_ID;
                                            //_objActivityLog.Customer_ID = Convert.ToInt32(row["Customer_ID"]);
                                            //_objActivityLog.RecordInsertDate =  Convert.ToDateTime(Encrypted_time1);

                                            //Service.srvActivityLogTracker srvActivityLog = new Service.srvActivityLogTracker();
                                            //int _i = srvActivityLog.Create(_objActivityLog);
                                        }
                                    }

                                    if (sttic_cap_cnt > 1)
                                    {
                                        _obj.Retry_Cnt = 1;// show
                                    }
                                    else
                                    {
                                        _obj.Retry_Cnt = 0;// hide
                                    }
                                    _obj.Name = _obj.FirstName + " " + _obj.LastName;
                                    _obj.DocumentUploadCount = Convert.ToInt32(row["Primary_Count"]);
                                    _lst.Add(_obj);

                                    if (obj.UserName.ToLower() == emailu && obj.Password == _obj.Password && obj.UserName != "" && obj.Password != "")
                                    {
                                        if (Static_Datatable.Rows.Count > 0)
                                        {

                                            DataRow[] dr = Static_Datatable.Select("Email='" + obj.UserName.Trim() + "'");
                                            if (dr.Count() > 0)
                                            {
                                                foreach (DataRow drr in dr)
                                                {
                                                    //string static_email = Convert.ToString(drr["Email"]);
                                                    if (Convert.ToString(drr["Email"]) == obj.UserName.Trim())
                                                    {
                                                        drr["Count"] = 0;
                                                        sttic_cnt = Convert.ToInt32(drr["Count"]);


                                                    }
                                                }
                                            }
                                            else
                                            {

                                                Static_Datatable.Rows.Add(obj.UserName.Trim(), 1, DateTime.Now);
                                            }

                                        }
                                        if (Stat_cap_table.Rows.Count > 0)
                                        {

                                            DataRow[] dr = Stat_cap_table.Select("IPAdress='" + capIp + "'");
                                            if (dr.Count() > 0)
                                            {
                                                foreach (DataRow drr in dr)
                                                {
                                                    //string static_email = Convert.ToString(drr["Email"]);
                                                    if (Convert.ToString(drr["IPAdress"]) == capIp)
                                                    {
                                                        drr["Count"] = 0;
                                                        sttic_cap_cnt = Convert.ToInt32(drr["Count"]);

                                                    }
                                                }
                                            }
                                            else
                                            {

                                                Stat_cap_table.Rows.Add(capIp, 1, DateTime.Now);

                                            }

                                        }
                                        //cnt = 0;
                                        //captcnt = 0;

                                    }
                                }

                                //DataTable li1_captcha1 = (DataTable)CompanyInfo.getEmailPermission(obj.Client_ID, 108);
                                Boolean chkLocation = true; Boolean chk_valid_loc = true;
                                Boolean check_device = true;
                                string logger = "";
                                string country_log = "";
                                string device_ty = "";
                                string New_Login_Detected = "";
                                string uniqueId = string.Empty;
                                #region check devicelogin

                                if (!checklink.Contains("/token"))
                                {
                                    string userAgent = obj.userAgent; // HttpContext.Current.Request.UserAgent;

                                    if (!string.IsNullOrEmpty(userAgent))
                                    {
                                        System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();
                                        byte[] hash = md5.ComputeHash(Encoding.Default.GetBytes(userAgent));
                                        uniqueId = BitConverter.ToString(hash).Replace("-", "").ToLower();
                                    }

                                    if (obj.deviceId != null && obj.deviceId != "")
                                        uniqueId = obj.deviceId;


                                    logger = logger + " uniqueId :" + uniqueId + " | ";
                                    MySqlConnector.MySqlCommand cmd = new MySqlConnector.MySqlCommand("save_device_info");
                                    cmd.CommandType = CommandType.StoredProcedure;
                                    cmd.Parameters.AddWithValue("_customer_id", _obj.Id);
                                    cmd.Parameters.AddWithValue("_token", uniqueId);
                                    cmd.Parameters.AddWithValue("_client_id", _obj.Client_ID);
                                    cmd.Parameters.Add(new MySqlConnector.MySqlParameter("valid", MySqlConnector.MySqlDbType.String));
                                    cmd.Parameters["valid"].Direction = ParameterDirection.Output;
                                    db_connection.ExecuteNonQueryProcedure(cmd);


                                    check_device = Convert.ToBoolean(cmd.Parameters["valid"].Value);
                                    logger = logger + "| uniqueId :" + uniqueId + "save_device_info :(" + _obj.Id + ", " + uniqueId + "," + _obj.Client_ID + ") check_device" + check_device + " | ";
                                }
                                else if ((checklink.Contains("/token") || obj.tokenRequest == "F"))
                                {
                                    string userAgent = obj.userAgent;// HttpContext.Current.Request.UserAgent;
                                    uniqueId = string.Empty;
                                    uniqueId = obj.deviceId;
                                    if (!string.IsNullOrEmpty(userAgent))
                                    {
                                        System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();
                                        byte[] hash = md5.ComputeHash(Encoding.Default.GetBytes(userAgent));
                                        uniqueId = BitConverter.ToString(hash).Replace("-", "").ToLower();
                                    }
                                    logger = logger + " uniqueId :" + uniqueId + " | ";
                                    MySqlConnector.MySqlCommand cmd = new MySqlConnector.MySqlCommand("check_device_info");
                                    cmd.CommandType = CommandType.StoredProcedure;
                                    cmd.Parameters.AddWithValue("_customer_id", _obj.Id);
                                    cmd.Parameters.AddWithValue("_token", uniqueId);
                                    cmd.Parameters.AddWithValue("_client_id", _obj.Client_ID);
                                    cmd.Parameters.Add(new MySqlConnector.MySqlParameter("valid", MySqlConnector.MySqlDbType.String));
                                    cmd.Parameters["valid"].Direction = ParameterDirection.Output;
                                    db_connection.ExecuteNonQueryProcedure(cmd);
                                    check_device = Convert.ToBoolean(cmd.Parameters["valid"].Value);
                                }

                                #endregion
                                #region check location
                                DataTable dtc = CompanyInfo.get(obj.Client_ID, context);
                                string act = "";
                                try
                                {
                                    if (!checklink.Contains("/token"))
                                    {
                                        //DataTable chkLocation1 = CompanyInfo.check_location(obj.Client_ID,  obj.userAgent ,context); //  null; //
                                        DataTable chkLocation1 = CompanyInfo.checklocationforNewLocation(obj.Client_ID, obj.userAgent, context, obj.ipAddress, obj.latitude, obj.longitude);

                                        try
                                        {
                                            chkLocation = Convert.ToBoolean(chkLocation1.Rows[0]["is_valid"]);
                                            country_log = Convert.ToString(chkLocation1.Rows[0]["Country"]);
                                            device_ty = Convert.ToString(chkLocation1.Rows[0]["device_ty"]);

                                            logger = logger + "| Try ( chkLocation " + chkLocation + " country_log " + country_log + " device_ty " + device_ty + ")";
                                        }
                                        catch
                                        {
                                            logger = logger + " catch for check Location";
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    act = act + Convert.ToString(ex);
                                }
                                #endregion
                                logger = logger + " | 1.ChkLocation and check_devic " + chkLocation + check_device + " | ";

                                if ((!checklink.Contains("/token") || obj.tokenRequest == "F") && (!chkLocation || (!check_device && Convert.ToInt32(li1_captcha1[0]["PID"]) == 108 && Convert.ToInt32(li1_captcha1[0]["Status_ForCustomer"]) == 0)) && (_obj.Id != null))
                                {
                                    string location_msg = "We noticed a login activity of your account from a new location that we do not recognise. If this wasn't you, we'll help you secure your account.  If it was you, please ignore this message";
                                    string notification_message = "<span class='cls-admin'>Customer <strong class='cls-new-benf'>New location detected</strong></span>";
                                    string Sub_ = " -  New login location detected ";
                                    logger = logger + " | ChkLocation and check_devic " + chkLocation + check_device + " | ";
                                    if (!chkLocation && (!check_device && Convert.ToInt32(li1_captcha1[0]["PID"]) == 108 && Convert.ToInt32(li1_captcha1[0]["Status_ForCustomer"]) == 0))
                                    {
                                        location_msg = "We noticed a new login activity of your account on a device that we do not recognise from a location you have not used before. If this wasn't you, we'll help you secure your account.  If it was you, please ignore this message";
                                        notification_message = "<span class='cls-admin'>Customer <strong class='cls-new-benf'>New location and new device detected</strong></span>";
                                        Sub_ = " -  New login location and new device detected ";
                                        act = act + " | location is invalid  and new device ";
                                        act = act + " |notification sent for new location and unknown Device ";
                                    }
                                    else if (chkLocation && (!check_device && Convert.ToInt32(li1_captcha1[0]["PID"]) == 108 && Convert.ToInt32(li1_captcha1[0]["Status_ForCustomer"]) == 0))
                                    {
                                        location_msg = "We noticed a login activity of your account on a device that we do not recognise. If this wasn't you, we'll help you secure your account.  If it was you, please ignore this message.";
                                        notification_message = "<span class='cls-admin'>Customer <strong class='cls-new-benf'> New device detected </strong> </span>";
                                        Sub_ = " -  New device detected ";
                                        act = act + " |  new device ";
                                        act = act + " |notification sent for unknown Device ";
                                    }

                                    //email
                                    //Start Parth added for restricting to send email if location is null or empty
                                    #region restricting to send email if location is null or empty
                                    try
                                    {
                                        _ = Task.Run(() => CompanyInfo.InsertActivityLogDetails(country_log, 0, 0, 0, 0, "IsValidCustomer: Restrict email for Location", 0, 1, "", context));
                                    }
                                    catch (Exception ex) { }
                                    if (string.IsNullOrWhiteSpace(country_log) || country_log.ToLower().Contains("null") ||
                                        country_log.Replace("<br /><strong>Network</strong>", "").Replace(",", "").Trim().Length == 0)
                                    {
                                        act = act + " | Skipped email: No valid location data.";
                                    }
                                    else
                                    {
                                    #endregion restricting to send email if location is null or empty
                                    //End Parth added for restricting to send email if location is null or empty
                                        try
                                        {
                                            string subject1 = string.Empty;
                                            string body1 = string.Empty;
                                            HttpWebRequest httpRequest = null, httpRequest1 = null;
                                            DataTable d2 = (DataTable)CompanyInfo.getCustomerDetails(obj.Client_ID, Convert.ToInt32(_obj.Id));
                                            string sendmsg = " New Login Detected ";
                                            string company_name = Convert.ToString(dtc.Rows[0]["Company_Name"]);
                                            string URL = Convert.ToString(dtc.Rows[0]["Company_URL_Admin"]);
                                            string cust_url = Convert.ToString(dtc.Rows[0]["Company_URL_Customer"]);
                                            httpRequest = (HttpWebRequest)WebRequest.Create(URL + "Email/new-login.html");
                                            httpRequest.UserAgent = "Code Sample Web Client";
                                            HttpWebResponse webResponse = (HttpWebResponse)httpRequest.GetResponse();
                                            using (StreamReader reader = new StreamReader(webResponse.GetResponseStream()))
                                            {
                                                body1 = reader.ReadToEnd();
                                            }
                                            body1 = body1.Replace("[name]", Convert.ToString(dt.Rows[0]["First_Name"]));
                                            string enc_ref = CompanyInfo.Encrypt(Convert.ToString(dt.Rows[0]["WireTransfer_ReferanceNo"]), true);
                                            string link = cust_url + "/secure-account-verfiy?reference=" + enc_ref;
                                            body1 = body1.Replace("[New_Login_Detected]", sendmsg);
                                            body1 = body1.Replace("[link]", link);
                                            body1 = body1.Replace("[country]", country_log);
                                            body1 = body1.Replace("[time]", (Convert.ToDateTime(CompanyInfo.gettime(obj.Client_ID, context))).ToString("dddd, dd MMMM yyyy HH:mm:ss"));
                                            body1 = body1.Replace("[location_msg]", location_msg);
                                            body1 = body1.Replace("[device]", device_ty);

                                            string EmailID = Convert.ToString(dt.Rows[0]["Email_ID"]);


                                            subject1 = company_name + Sub_ + dt.Rows[0]["WireTransfer_ReferanceNo"];
                                            string send_mail = (string)CompanyInfo.Send_Mail(dtc, EmailID, body1, subject1, obj.Client_ID, obj.Branch_ID, "", "", "", context);
                                        }
                                        catch { }
                                        act = act + "| email sent for invalid location";
                                        string notification_icon = "new-cust.jpg";
                                        CompanyInfo.save_notification_compliance(notification_message, notification_icon, _obj.Id, Convert.ToDateTime(Convert.ToDateTime(CompanyInfo.gettime(obj.Client_ID, context))), obj.Client_ID, 1, 0, obj.Branch_ID, 0, 1, 1, 0, context);
                                    }
                                }
                                // _ = CompanyInfo.InsertActivityLogDetailsasync(logger, 0, 0, 0, 0, "testingPurpose", 0, 1, "");
                                //DataTable chk_loc_perm1 = (DataTable)CompanyInfo.getEmailPermission(obj.Client_ID, 111);
                                //DataTable chk_loc_perm2 = (DataTable)CompanyInfo.getEmailPermission(obj.Client_ID, 112);

                                string referer = CompanyInfo.getUrlReferrer();

                                //if (checklink.Contains("/token") && chk_verify_status == 0 && !check_device && Convert.ToInt32(chk_loc_perm2[0]["PID"]) == 112 && Convert.ToInt32(chk_loc_perm2[0]["Status_ForCustomer"]) == 0 && Status == "0" && !((HttpContext.Current.Request.UrlReferrer.AbsolutePath).Contains("app-register") || (HttpContext.Current.Request.UrlReferrer.AbsolutePath).Contains("/sign-up")))
                                if ((checklink.Contains("/token") || obj.tokenRequest == "F") && chk_verify_status == 0 && !check_device && Convert.ToInt32(chk_loc_perm2[0]["PID"]) == 112 && Convert.ToInt32(chk_loc_perm2[0]["Status_ForCustomer"]) == 0 && Status == "0" && !((referer).Contains("app-register") || (referer).Contains("/sign-up")))
                                {
                                    string msg = CompanyInfo.chk_twostep_flag(obj.Client_ID, Convert.ToInt32(_obj.Id), "New device login found");
                                    if (msg == "Success")
                                    {
                                        dt.Rows[0]["Multiple_attempt"] = 0;
                                        _obj.Flag = send_otp(dt, obj, _obj, context);
                                        _obj.Flag = 3;
                                        _obj.ErrorFlag = "2";
                                        if (_lst.Count == 0)
                                            _lst.Add(_obj);
                                        return _lst;
                                    }
                                }

                                //else if (checklink.Contains("/token") && chk_verify_status == 0 && Convert.ToInt32(chk_loc_perm1[0]["PID"]) == 111 && Convert.ToInt32(chk_loc_perm1[0]["Status_ForCustomer"]) == 0 && Status == "0" && !((HttpContext.Current.Request.UrlReferrer.AbsolutePath).Contains("app-register") || (HttpContext.Current.Request.UrlReferrer.AbsolutePath).Contains("/sign-up")))
                                else if ((checklink.Contains("/token") || obj.tokenRequest == "F") && chk_verify_status == 0 && Convert.ToInt32(chk_loc_perm1[0]["PID"]) == 111 && Convert.ToInt32(chk_loc_perm1[0]["Status_ForCustomer"]) == 0 && Status == "0" && !((referer).Contains("app-register") || (referer).Contains("/sign-up")))
                                {
                                    string browserinfo = "";
                                    string countryf = "";
                                    string IPAddress1 = context.Connection.RemoteIpAddress.ToString(); //HttpContext.Current.Request.UserHostAddress;
                                    browserinfo = "IP Address: " + IPAddress1;
                                    try
                                    {
                                        ServicePointManager.Expect100Continue = true;
                                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
                                               | SecurityProtocolType.Tls11
                                               | SecurityProtocolType.Tls12
                                               | SecurityProtocolType.Ssl3;
                                        var client = new RestClient("https://tools.keycdn.com/geo.json?host=" + IPAddress1);
                                        act = act + " RestClient query :" + "https://tools.keycdn.com/geo.json?host=" + IPAddress1;

                                        client.Timeout = -1;
                                        var request = new RestRequest(Method.GET);
                                        client.UserAgent = "keycdn-tools:" + Convert.ToString(dtc.Rows[0]["company_website"]); //dtc.Rows[0]["Company_URL_Customer"] 
                                        act = act + " client.UserAgent :" + "keycdn-tools:" + Convert.ToString(dtc.Rows[0]["company_website"]);

                                        //keycdn-tools:https://www.calyx-solutions.com
                                        request.AddHeader("Accept-Encoding", "gzip, deflate, br");

                                        IRestResponse response = client.Execute(request);

                                        GeoLocation GeoLocationList = Newtonsoft.Json.JsonConvert.DeserializeObject<GeoLocation>(response.Content);
                                        act = act + " GeoLocationList :" + Convert.ToString(GeoLocationList);
                                        act = act + " GeoLocationList.data :" + Convert.ToString(GeoLocationList.data);
                                        act = act + " GeoLocationList.data.geo :" + Convert.ToString(GeoLocationList.data.geo);
                                        browserinfo = Newtonsoft.Json.JsonConvert.SerializeObject(GeoLocationList.data.geo);
                                    }
                                    catch (Exception ex)
                                    {
                                        act = act + " inlocationException :" + ex.ToString();
                                        browserinfo = "IP Address: " + IPAddress1;
                                    }

                                    try
                                    {
                                        var Address = browserinfo.Split(',');


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
                                    }
                                    if (countryf.Trim() != "" && countryf.Trim() != null)
                                    {
                                        if (countryf.Trim().ToLower() != Convert.ToString(dt.Rows[0]["Country_Name"]).Trim().ToLower())
                                        {
                                            string msg = CompanyInfo.chk_twostep_flag(obj.Client_ID, Convert.ToInt32(_obj.Id), "New login location found");
                                            if (msg == "Success")
                                            {
                                                dt.Rows[0]["Multiple_attempt"] = 0;
                                                _obj.Flag = send_otp(dt, obj, _obj, context);
                                                _obj.Flag = 3;
                                                _obj.ErrorFlag = "2";
                                                if (_lst.Count == 0)
                                                    _lst.Add(_obj);
                                                return _lst;
                                            }
                                        }
                                    }
                                }
                                //else if (checklink.Contains("/token") && obj.CredentialFlag != "1" && chk_verify_status == 0 && !check_device && Status == "1" && !((HttpContext.Current.Request.UrlReferrer.AbsolutePath).Contains("app-register") || (HttpContext.Current.Request.UrlReferrer.AbsolutePath).Contains("/sign-up")) && passcode_flag == 0 && Passcode_per == 0)
                                else if ((checklink.Contains("/token") || obj.tokenRequest == "F") && obj.CredentialFlag != "1" && chk_verify_status == 0 && !check_device && Status == "1" && !((referer).Contains("app-register") || (referer).Contains("/sign-up")) && passcode_flag == 0 && Passcode_per == 0)
                                {// if new device found we will ask for the passcode

                                    string msg = "New device found asking for the passcode";
                                    _ = CompanyInfo.InsertActivityLogDetailsSecurityasync(msg, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), 1, "IsValidCustomer", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "IsValidCustomer", 2, context);
                                    _obj.Flag = 6;
                                    if (_lst.Count == 0)
                                    {
                                        _lst.Add(_obj);
                                    }
                                    return _lst;
                                }
                            }
                            else//Siddhi
                            {
                                if ((checklink.Contains("/token") || obj.tokenRequest == "F"))
                                {
                                    try
                                    {
                                        Loginct_email_count = Convert.ToInt32(Loginct_email_count) + 1;
                                        sttic_cnt = Convert.ToInt32(Loginct_email_count);
                                    }
                                    catch { }

                                    //cnt++;
                                    //context.Session["Count"] = Convert.ToInt32(context.Session["Count"]) + 1;
                                    /*if (Static_Datatable.Rows.Count > 0)
                                    {

                                        DataRow[] dr = Static_Datatable.Select("Email='" + obj.UserName.Trim() + "'");
                                        if (dr.Count() > 0)
                                        {
                                            foreach (DataRow drr in dr)
                                            {
                                                //string static_email = Convert.ToString(drr["Email"]);
                                                if (Convert.ToString(drr["Email"]) == obj.UserName.Trim())
                                                {
                                                    drr["Count"] = Convert.ToInt32(drr["Count"]) + 1;
                                                    sttic_cnt = Convert.ToInt32(drr["Count"]);

                                                }
                                            }
                                        }
                                        else
                                        {

                                            Static_Datatable.Rows.Add(obj.UserName.Trim(), 1, DateTime.Now);

                                        }

                                    }*/
                                    if (obj.CredentialFlag != "" && obj.CredentialFlag != null && obj.CredentialFlag == "1" && Passcode_per == 0)
                                    {
                                        using (MySqlConnector.MySqlCommand cmd_cnt = new MySqlConnector.MySqlCommand("Update_passcode_count"))
                                        {
                                            cmd_cnt.CommandType = CommandType.StoredProcedure;
                                            cmd_cnt.Parameters.AddWithValue("_Customer_ID", dt.Rows[0]["Customer_ID"]);
                                            cmd_cnt.Parameters.AddWithValue("_Date_chk", Convert.ToDateTime(CompanyInfo.gettime(obj.Client_ID, context)));

                                            DataTable chck_limit = db_connection.ExecuteQueryDataTableProcedure(cmd_cnt);
                                            if (Convert.ToInt32(chck_limit.Rows[0]["InvalidLogin_attempt"]) > 3)
                                            {
                                                string msg = "Customer is blocked because of the invalid attempts";
                                                _ = CompanyInfo.InsertActivityLogDetailsSecurityasync(msg, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), 1, "IsValidCustomer", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "IsValidCustomer", 2, context);
                                                _obj.Flag = 7;
                                                if (_lst.Count == 0)
                                                {
                                                    _lst.Add(_obj);
                                                }
                                                return _lst;
                                            }
                                        }
                                    }
                                }
                                //DataTable chk_loc_perm3 = (DataTable)CompanyInfo.getEmailPermission(obj.Client_ID, 113);
                                if (sttic_cnt >= 1 && Convert.ToInt32(chk_loc_perm3[0]["PID"]) == 113 && Convert.ToInt32(chk_loc_perm3[0]["Status_ForCustomer"]) == 0 && Status == "0" && (checklink.Contains("/token") || obj.tokenRequest == "F"))
                                {
                                    MySqlConnector.MySqlCommand _cmd_valid = new MySqlConnector.MySqlCommand("block_login_app");//"select    cast(AES_DECRYPT(UNHEX(u.Password), '" + obj.SecurityKey.Trim() + "' ) as  char(500)) as Password,u.First_Name,u.Last_Name,u.Customer_ID,U.WireTransfer_ReferanceNo,u.Email_ID,u.Branch_ID,u.Client_ID from customer_registration u where    Email_ID = '" + obj.Name.Trim() + "' and Password=HEX(AES_ENCRYPT('" + obj.Password.Trim() + "','" + obj.SecurityKey.Trim() + "'))  and u.delete_status=0 ;");
                                    _cmd_valid.CommandType = CommandType.StoredProcedure;
                                    _cmd_valid.Parameters.AddWithValue("_reason", "Too many attempts for login failed");
                                    _cmd_valid.Parameters.AddWithValue("_loginName", dt.Rows[0]["Customer_ID"]);
                                    _cmd_valid.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                                    _cmd_valid.Parameters.AddWithValue("_flag", "1");
                                    int i1 = Convert.ToInt32(db_connection.ExecuteScalarProcedure(_cmd_valid));
                                }
                                if (sttic_cnt >= 3 && (checklink.Contains("/token") || obj.tokenRequest == "F"))
                                {
                                    string msg = "Multiple attempts of invalid data. User Name: " + email + " Password: " + pwd + "";
                                    _ = CompanyInfo.InsertActivityLogDetailsSecurity(msg, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), 1, "IsValidCustomer", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "IsValidCustomer", 2, context);
                                    DataTable block_status = (DataTable)CompanyInfo.getEmailPermission(obj.Client_ID, 109);
                                    if (Convert.ToInt32(block_status.Rows[0]["PID"]) == 109 && Convert.ToInt32(block_status.Rows[0]["Status_ForCustomer"]) == 0)
                                    {
                                        MySqlConnector.MySqlCommand _cmd_valid = new MySqlConnector.MySqlCommand("block_login_app");//"select    cast(AES_DECRYPT(UNHEX(u.Password), '" + obj.SecurityKey.Trim() + "' ) as  char(500)) as Password,u.First_Name,u.Last_Name,u.Customer_ID,U.WireTransfer_ReferanceNo,u.Email_ID,u.Branch_ID,u.Client_ID from customer_registration u where    Email_ID = '" + obj.Name.Trim() + "' and Password=HEX(AES_ENCRYPT('" + obj.Password.Trim() + "','" + obj.SecurityKey.Trim() + "'))  and u.delete_status=0 ;");
                                        _cmd_valid.CommandType = CommandType.StoredProcedure;
                                        _cmd_valid.Parameters.AddWithValue("_reason", "Too many attempts for login failed");
                                        _cmd_valid.Parameters.AddWithValue("_loginName", dt.Rows[0]["Customer_ID"]);
                                        _cmd_valid.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                                        _cmd_valid.Parameters.AddWithValue("_flag", "0");
                                        int i1 = Convert.ToInt32(db_connection.ExecuteScalarProcedure(_cmd_valid));
                                        msg = "Too many attempts for login failed and login blocked . User Name: " + email + " Password: " + pwd + "";
                                        _ = CompanyInfo.InsertActivityLogDetailsSecurityasync(msg, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), 1, "IsValidCustomer", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "IsValidCustomer", 2, context);
                                        //if (cnt >=3 && obj.Password.Trim() != Convert.ToString(dt.Rows[0]["PASSWORD"]))
                                        //{
                                        //    send_mail_on_invalidlogin(obj, dt);
                                        //}
                                        return _lst;
                                    }
                                    else if (Status == "0")
                                    {
                                        MySqlConnector.MySqlCommand _cmd_valid = new MySqlConnector.MySqlCommand("block_login_app");//"select    cast(AES_DECRYPT(UNHEX(u.Password), '" + obj.SecurityKey.Trim() + "' ) as  char(500)) as Password,u.First_Name,u.Last_Name,u.Customer_ID,U.WireTransfer_ReferanceNo,u.Email_ID,u.Branch_ID,u.Client_ID from customer_registration u where    Email_ID = '" + obj.Name.Trim() + "' and Password=HEX(AES_ENCRYPT('" + obj.Password.Trim() + "','" + obj.SecurityKey.Trim() + "'))  and u.delete_status=0 ;");
                                        _cmd_valid.CommandType = CommandType.StoredProcedure;
                                        _cmd_valid.Parameters.AddWithValue("_reason", "Too many attempts for login failed");
                                        _cmd_valid.Parameters.AddWithValue("_loginName", dt.Rows[0]["Customer_ID"]);
                                        _cmd_valid.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                                        _cmd_valid.Parameters.AddWithValue("_flag", "1");
                                        int i1 = Convert.ToInt32(db_connection.ExecuteScalarProcedure(_cmd_valid));
                                        msg = "Too many attempts for login failed and login blocked . User Name: " + email + " Password: " + pwd + "";
                                        _ = CompanyInfo.InsertActivityLogDetailsSecurityasync(msg, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), 1, "IsValidCustomer", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "IsValidCustomer", 2, context);
                                        //if (cnt >=3 && obj.Password.Trim() != Convert.ToString(dt.Rows[0]["PASSWORD"]))
                                        //{
                                        //    send_mail_on_invalidlogin(obj, dt);
                                        //}
                                        return _lst;
                                    }
                                }
                            }
                        }
                        else//Siddhi
                        {
                            if ((checklink.Contains("/token") || obj.tokenRequest == "F"))
                            {
                                try
                                {
                                    Loginct_email_count = Convert.ToInt32(Loginct_email_count) + 1;
                                    sttic_cnt = Convert.ToInt32(Loginct_email_count);
                                }
                                catch { }
                                //cnt++;
                                //context.Session["Count"] = Convert.ToInt32(context.Session["Count"]) + 1;
                                /* if (Static_Datatable.Rows.Count > 0)
                                 {

                                     DataRow[] dr = Static_Datatable.Select("Email='" + obj.UserName.Trim() + "'");
                                     if (dr.Count() > 0)
                                     {
                                         foreach (DataRow drr in dr)
                                         {
                                             //string static_email = Convert.ToString(drr["Email"]);
                                             if (Convert.ToString(drr["Email"]) == obj.UserName.Trim())
                                             {
                                                 drr["Count"] = Convert.ToInt32(drr["Count"]) + 1;
                                                 sttic_cnt = Convert.ToInt32(drr["Count"]);

                                             }
                                         }
                                     }
                                     else
                                     {

                                         Static_Datatable.Rows.Add(obj.UserName.Trim(), 1, DateTime.Now);

                                     }

                                 }*/
                            }
                            //cnt++;
                        }
                        return _lst;
                        //}
                    }
                    else
                    {
                        string msg = "SQl Enjection detetcted '" + obj.UserName + "' ";
                        _ = CompanyInfo.InsertActivityLogDetailsSecurityasync(msg, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), 1, "IsValidCustomer", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "IsValidCustomer", 0, context);
                        string statuss = (string)CompanyInfo.updatesecutityflag(obj.UserName, obj.Client_ID, obj.Branch_ID);
                        _obj.Flag = 1;
                        _lst.Add(_obj);
                        try
                        {
                            Loginct_email_count = Convert.ToInt32(Loginct_email_count) + 1;
                            sttic_cnt = Convert.ToInt32(Loginct_email_count);
                        }
                        catch { }
                        // cnt++;//Siddhi
                        //context.Session["Count"] = Convert.ToInt32(context.Session["Count"])+1;
                        /*if (Static_Datatable.Rows.Count > 0)
                        {

                            DataRow[] dr = Static_Datatable.Select("Email='" + obj.UserName.Trim() + "'");
                            if (dr.Count() > 0)
                            {
                                foreach (DataRow drr in dr)
                                {
                                    //string static_email = Convert.ToString(drr["Email"]);
                                    if (Convert.ToString(drr["Email"]) == obj.UserName.Trim())
                                    {
                                        drr["Count"] = Convert.ToInt32(drr["Count"]) + 1;
                                        sttic_cnt = Convert.ToInt32(drr["Count"]);

                                    }
                                }
                            }
                            else
                            {

                                Static_Datatable.Rows.Add(obj.UserName.Trim(), 1, DateTime.Now);

                            }

                        }*/

                    }


                }
            }
            catch (Exception ex)
            {
                DateTime Record_Insert_DateTime = DateTime.Now;
                string error = ex.ToString().Replace("\'", "\\'");
                _cmdh = new MySqlConnector.MySqlCommand("SaveException");
                _cmdh.CommandType = CommandType.StoredProcedure;
                _cmdh.Parameters.AddWithValue("_Record_Insert_DateTime", Record_Insert_DateTime);
                _cmdh.Parameters.AddWithValue("_error", error);
                _cmdh.Parameters.AddWithValue("_Client_ID", 1);
                int msg = db_connection.ExecuteNonQueryProcedure(_cmdh);
            }
            finally
            {
                _cmdh = new MySqlConnector.MySqlCommand("UpdateLoginCounts");
                _cmdh.CommandType = CommandType.StoredProcedure;
                _cmdh.Parameters.AddWithValue("_Email", Loginct_Email);
                _cmdh.Parameters.AddWithValue("_IPAdress", Loginct_IPAdress);
                _cmdh.Parameters.AddWithValue("_email_count", Loginct_email_count);
                _cmdh.Parameters.AddWithValue("_IpAddress_count", Loginct_IpAddress_count);
                _cmdh.Parameters.AddWithValue("_Date", Loginct_Date);
                string success = Convert.ToString(db_connection.ExecuteNonQueryProcedure(_cmdh));

            }

            return _lst;
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
        public string ipAddress(Microsoft.AspNetCore.Http.HttpContext context)
        {           
            var host = $"{context.Request.Scheme}://{context.Request.Host}";
            return host;
        }

        public void InsertLoginDetails(Model.Login obj,HttpContext context )
        {
            TAntiSQLInjection anti = new TAntiSQLInjection(TDbVendor.DbVOracle);
            String msg = "";
            if (anti.isInjected(obj.UserName.Trim()))
            {
                msg = "SQL injected found:";
            }
            string browserinfo = "";

            string IPAddress1 = context.Connection.RemoteIpAddress.ToString();
            //string IPAddress1 =  System.Web.HttpContext.Current.Request.UserHostAddress;
            browserinfo = " Geolocation Details: IP Address: " + IPAddress1;
            try
            {
                DataTable dtc = CompanyInfo.get(obj.Client_ID, context);
                ServicePointManager.Expect100Continue = true;
                /*ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
                       | SecurityProtocolType.Tls11
                       | SecurityProtocolType.Tls12
                       | SecurityProtocolType.Ssl3;*/
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                var client = new RestClient("https://tools.keycdn.com/geo.json?host=" + IPAddress1);
                client.Timeout = -1;
                var request = new RestRequest(Method.GET);
                client.UserAgent = "keycdn-tools:" + Convert.ToString(dtc.Rows[0]["company_website"]); //dtc.Rows[0]["Company_URL_Customer"] 
                //keycdn-tools:https://www.calyx-solutions.com
                request.AddHeader("Accept-Encoding", "gzip, deflate, br");

                IRestResponse response = client.Execute(request);

                GeoLocation GeoLocationList = Newtonsoft.Json.JsonConvert.DeserializeObject<GeoLocation>(response.Content);
                try
                {
                    browserinfo = " Geolocation Details: " + Newtonsoft.Json.JsonConvert.SerializeObject(GeoLocationList.data.geo);
                }
                catch(Exception exp) { }
            }
            catch (Exception ex)
            {
                browserinfo = "";// "IP Address: " + IPAddress1;
            }
            obj.UserName = obj.UserName.Replace(" ", "");
            MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("InsertLoginDetails");
            _cmd.CommandType = CommandType.StoredProcedure;
            _cmd.Parameters.AddWithValue("_Customer_Id", CompanyInfo.Decrypt(obj.Customer_ID, true));
            _cmd.Parameters.AddWithValue("_UserName", obj.UserName.Trim());
            _cmd.Parameters.AddWithValue("_Branch_ID", obj.Branch_ID);
            _cmd.Parameters.AddWithValue("_Location", obj.Location);
            _cmd.Parameters.AddWithValue("_IP_Address", obj.IP_Address.Trim());
            _cmd.Parameters.AddWithValue("_Location_Details", browserinfo);
            _cmd.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
            _cmd.Parameters.AddWithValue("_Login_DateTime", obj.Login_DateTime);
            _cmd.Parameters.AddWithValue("_Login_Time", obj.Login_Time);
            _cmd.Parameters.AddWithValue("_User_ID", obj.User_ID);
            _cmd.Parameters.Add("_Login_ID", MySqlConnector.MySqlDbType.Int32);
            _cmd.Parameters["_Login_ID"].Direction = ParameterDirection.Output;
            db_connection.ExecuteNonQueryProcedure(_cmd);
            int message = (Int32)_cmd.Parameters["_Login_ID"].Value;

        }

        public static bool ReCaptchaPassed(string secret_key, string gRecaptchaResponse)
        {
            try
            {
                HttpClient httpClient = new HttpClient();

                var res = httpClient.GetAsync("https://www.google.com/recaptcha/api/siteverify?secret=" + secret_key + "&response=" + gRecaptchaResponse + "").Result;

                if (res.StatusCode != HttpStatusCode.OK)
                {
                    return false;
                }
                string JSONres = res.Content.ReadAsStringAsync().Result;
                dynamic JSONdata = Newtonsoft.Json.Linq.JObject.Parse(JSONres);

                if (JSONdata.success != "true" || JSONdata.score <= 0.5m)
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                DateTime Record_Insert_DateTime = DateTime.Now;
                string error = "ReCaptchaPassed function: - " + ex.ToString().Replace("\'", "\\'");
                MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("SaveException");
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.Parameters.AddWithValue("_Record_Insert_DateTime", Record_Insert_DateTime);
                _cmd.Parameters.AddWithValue("_error", error);
                _cmd.Parameters.AddWithValue("_Client_ID", 1);
                int msg = db_connection.ExecuteNonQueryProcedure(_cmd);
            }
            return true;
        }

        private static string send_mail_on_invalidlogin(Model.Login obj, DataTable dt, string otp, HttpContext context)
        {
            string msg = "";
            string email = obj.UserName;
            //string subject = string.Empty;
            string body = string.Empty, subject = string.Empty;
            string body1 = string.Empty;
            string template = "";
            HttpWebRequest httpRequest = null, httpRequest1 = null;
            MySqlConnector.MySqlCommand cmd = new MySqlConnector.MySqlCommand("Get_CompanyInfo");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
            cmd.Parameters.AddWithValue("_Customer_ID", "");
            cmd.Parameters.AddWithValue("_SecurityKey", CompanyInfo.SecurityKey());
            DataTable dtc = (DataTable)db_connection.ExecuteQueryDataTableProcedure(cmd);

            if (dtc.Rows.Count == 1 && email != null)
            {

                //DateTime date = DateTime.Now;
                //int bool_check1 = 1;
                //bool b2 = Convert.ToBoolean(bool_check1);
                //String Encrypted_time1 = Convert.ToString(CompanyInfo.Encrypt(Convert.ToString(date), b2));

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
                msg = (string)CompanyInfo.Send_Mail(dtc, obj.UserName, body, subject, obj.Client_ID, obj.Branch_ID, "", "", "", context);
                if (msg == "Success")
                {

                    Model.ActivityLogTracker _objActivityLog = new Model.ActivityLogTracker();
                    _objActivityLog.Activity = "Forgot password on multiple failed attempt to login email sent to " + obj.UserName + " ";
                    _objActivityLog.FunctionName = "Check Login";
                    _objActivityLog.Transaction_ID = 0;
                    _objActivityLog.WhoAcessed = 0;
                    _objActivityLog.Branch_ID = obj.Branch_ID;
                    _objActivityLog.Client_ID = obj.Client_ID;
                    Service.srvActivityLogTracker srvActivityLog = new Service.srvActivityLogTracker();
                    int _i = srvActivityLog.Create(_objActivityLog, context);
                }
            }
            return msg;
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

        public static int send_otp(DataTable dt, Model.Login obj, Model.Customer _obj, HttpContext context)
        {            
            string path_query = CompanyInfo.path_query(context);

            //var secretKey = CompanyInfo.Decrypt(System.Configuration.ConfigurationManager.AppSettings["reCaptchaKey"], Convert.ToBoolean(1));
            //var context =  HttpContext.Current;
            string checklink = path_query;  // context.Request.Url.PathAndQuery.ToString();
            string otp = "";
            String multi_att_activity = "";
            string intValue = Convert.ToString(dt.Rows[0]["Customer_ID"]);
            int intValue1 = Convert.ToInt32(dt.Rows[0]["Country_ID"]);
            String Encrypted_time1 = Convert.ToString(CompanyInfo.gettime(obj.Client_ID, intValue, intValue1, context));
            try
            {
                _obj.OTP = Convert.ToString(dt.Rows[0]["Multiple_attempt"]);
                multi_att_activity = multi_att_activity + "Multiple attempt flag : " + _obj.OTP;

                if (Convert.ToInt32(dt.Rows[0]["Multiple_attempt"]) == 0 && (obj.OTP == null || obj.OTP == "") && checklink.Contains("/checklogin"))
                {

                    int otpLength = 6;
                    otp = GenerateOTP(otpLength);
                    // Console.WriteLine("Random OTP: " + otp);
                    multi_att_activity = multi_att_activity + " | OTP generated : " + otp;

                    if (obj.Resend_otp != "Resend") //131023
                    {

                        MySqlConnector.MySqlCommand _cmd_check1 = new MySqlConnector.MySqlCommand("save_new_login_token");

                        _cmd_check1.CommandType = CommandType.StoredProcedure;
                        _cmd_check1.Parameters.AddWithValue("_OTP", otp);
                        _cmd_check1.Parameters.AddWithValue("_OTP_Expiry", Convert.ToDateTime(Encrypted_time1).AddMinutes(15));
                        _cmd_check1.Parameters.AddWithValue("_Customer_ID", dt.Rows[0]["Customer_ID"]);
                        _cmd_check1.Parameters.AddWithValue("_flag", 0);
                        int n = db_connection.ExecuteNonQueryProcedure(_cmd_check1);
                    }
                    MySqlConnector.MySqlCommand _cmd_check = new MySqlConnector.MySqlCommand("sp_save_otp");

                    _cmd_check.CommandType = CommandType.StoredProcedure;
                    _cmd_check.Parameters.AddWithValue("_OTP", otp);
                    _cmd_check.Parameters.AddWithValue("_OTP_Expiry", Convert.ToDateTime(Encrypted_time1).AddMinutes(15));
                    _cmd_check.Parameters.AddWithValue("_Customer_ID", dt.Rows[0]["Customer_ID"]);
                    _cmd_check.Parameters.Add(new MySqlConnector.MySqlParameter("setflag", MySqlConnector.MySqlDbType.String));
                    _cmd_check.Parameters["setflag"].Direction = ParameterDirection.Output;
                    DataTable d_t = db_connection.ExecuteQueryDataTableProcedure(_cmd_check);
                    multi_att_activity = multi_att_activity + " | OTP saved to the Table for Customer_ID : " + dt.Rows[0]["Customer_ID"];
                    if (Convert.ToInt32(d_t.Rows[0]["setflag"]) == 1)
                    {
                        send_mail_on_invalidlogin(obj, dt, otp, context);
                        multi_att_activity = multi_att_activity + " | Email sent for OTP ";
                        _obj.Flag = 3;
                    }
                    else
                    {
                        _obj.Flag = 5;
                    }



                }

                else if(Convert.ToInt32(dt.Rows[0]["Multiple_attempt"]) == 0 && (obj.OTP == null || obj.OTP == "") && checklink.Contains("/token"))
                {

                    int otpLength = 6;
                    otp = GenerateOTP(otpLength);
                    // Console.WriteLine("Random OTP: " + otp);
                    multi_att_activity = multi_att_activity + " | OTP generated : " + otp;

                    if (obj.Resend_otp != "Resend") //131023
                    {

                     MySqlConnector.MySqlCommand _cmd_check1 = new MySqlConnector.MySqlCommand("save_new_login_token");

                        _cmd_check1.CommandType = CommandType.StoredProcedure;
                        _cmd_check1.Parameters.AddWithValue("_OTP", otp);
                        _cmd_check1.Parameters.AddWithValue("_OTP_Expiry", Convert.ToDateTime(Encrypted_time1).AddMinutes(15));
                        _cmd_check1.Parameters.AddWithValue("_Customer_ID", dt.Rows[0]["Customer_ID"]);
                        _cmd_check1.Parameters.AddWithValue("_flag", 0);
                        int n = db_connection.ExecuteNonQueryProcedure(_cmd_check1);
                    }
                    MySqlConnector.MySqlCommand _cmd_check = new MySqlConnector.MySqlCommand("sp_save_otp");

                    _cmd_check.CommandType = CommandType.StoredProcedure;
                    _cmd_check.Parameters.AddWithValue("_OTP", otp);
                    _cmd_check.Parameters.AddWithValue("_OTP_Expiry", Convert.ToDateTime(Encrypted_time1).AddMinutes(15));
                    _cmd_check.Parameters.AddWithValue("_Customer_ID", dt.Rows[0]["Customer_ID"]);
                    _cmd_check.Parameters.Add(new MySqlConnector.MySqlParameter("setflag", MySqlConnector.MySqlDbType.String));
                    _cmd_check.Parameters["setflag"].Direction = ParameterDirection.Output;
                    DataTable d_t = db_connection.ExecuteQueryDataTableProcedure(_cmd_check);
                    multi_att_activity = multi_att_activity + " | OTP saved to the Table for Customer_ID : " + dt.Rows[0]["Customer_ID"];
                    if (Convert.ToInt32(d_t.Rows[0]["setflag"]) == 1)
                    {
                        send_mail_on_invalidlogin(obj, dt, otp, context);
                        multi_att_activity = multi_att_activity + " | Email sent for OTP ";
                        _obj.Flag = 3;
                    }
                    else
                    {
                        _obj.Flag = 5;
                    }



                }
                else if (Convert.ToInt32(dt.Rows[0]["Multiple_attempt"]) == 0 && obj.OTP != null && obj.OTP != "" && checklink.Contains("/token"))
                {
                    multi_att_activity = multi_att_activity + " | Customer Entered OTP : " + obj.OTP;
                    MySqlConnector.MySqlCommand _cmd_check = new MySqlConnector.MySqlCommand("chk_emailOtp_app");
                    _cmd_check.CommandType = CommandType.StoredProcedure;
                    _cmd_check.Parameters.AddWithValue("_Customer_ID", Convert.ToInt32(dt.Rows[0]["Multiple_attempt"]));
                    _cmd_check.Parameters.AddWithValue("_OTP", Convert.ToInt32(obj.OTP));
                    _cmd_check.Parameters.AddWithValue("_Date", Convert.ToDateTime(Encrypted_time1));
                    DataTable dt1 = db_connection.ExecuteQueryDataTableProcedure(_cmd_check);
                    if (dt1.Rows.Count == 1)
                    {
                        multi_att_activity = multi_att_activity + " | we check OTP in datatable for time " + Encrypted_time1 + "  where OId : " + dt1.Rows[0]["OId"];
                        if (Convert.ToDateTime(dt1.Rows[0]["OTP_Expiry"]) >= Convert.ToDateTime(Encrypted_time1) && obj.OTP == Convert.ToString(dt1.Rows[0]["OTP"]))
                        {

                            MySqlConnector.MySqlCommand cmd = new MySqlConnector.MySqlCommand("update_blacklistflag");
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("_Customer_ID", Convert.ToString(intValue));
                            cmd.Parameters.AddWithValue("_Flag", 0);
                            int st = db_connection.ExecuteNonQueryProcedure(cmd);
                            _obj.block_login_flag = 0;
                            _obj.OTP = "";
                            multi_att_activity = multi_att_activity + " | OTP is correct, blacklisted and Multiple attempt flagis updated ";
                        }
                        else
                        {
                            multi_att_activity = multi_att_activity + " | Invalid or expired otp";
                            _obj.Flag = 4;
                        }
                    }
                    else
                    {
                        multi_att_activity = multi_att_activity + " | Invalid or expired otp";

                        _obj.Flag = 4;
                    }
                }
            }
            catch (Exception ex)
            {
                multi_att_activity = multi_att_activity + " | Error occured " + ex.ToString();

            }
            finally
            {
                //int stattus = (int)CompanyInfo.InsertActivityLogDetailsSecurity(multi_att_activity, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), 1, "IsValidCustomer", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "IsValidCustomer", 0);
                Model.ActivityLogTracker _objActivityLog = new Model.ActivityLogTracker();
                _objActivityLog.Activity = multi_att_activity;
                _objActivityLog.FunctionName = "IsValidCustomer";
                _objActivityLog.Transaction_ID = 0;
                _objActivityLog.WhoAcessed = 0;
                _objActivityLog.Branch_ID = obj.Branch_ID;
                _objActivityLog.Client_ID = obj.Client_ID;
                _objActivityLog.Customer_ID = Convert.ToInt32(intValue);
                _objActivityLog.RecordInsertDate = Convert.ToDateTime(Encrypted_time1);

                Service.srvActivityLogTracker srvActivityLog = new Service.srvActivityLogTracker();
                int _i = srvActivityLog.Create(_objActivityLog, context);
            }
            return _obj.Flag;
        }


        private static string Check_Ip(DataTable dt, HttpContext context)
        {
            string msg1 = "";
            string intValue = Convert.ToString(dt.Rows[0]["Customer_ID"]);
            int intValue1 = Convert.ToInt32(dt.Rows[0]["Country_ID"]);
            String Encrypted_time1 = Convert.ToString(CompanyInfo.gettime(Convert.ToInt32(dt.Rows[0]["Client_ID"]), intValue, intValue1, context));
            try
            {
                 
                string ipAddressContext = CompanyInfo.ipAddressContext(context);

                int Customer_id = Convert.ToInt32(dt.Rows[0]["Customer_ID"]);
                //DataTable chkLocation1 = CompanyInfo.check_location(obj1.Client_ID);

                // string Ip_Address = Convert.ToString(chkLocation1.Rows[0]["browser_info"]);              
                string IPAddress1 = ipAddressContext;//HttpContext.Current.Request.UserHostAddress;
                string browserinfo = "IP Address: " + IPAddress1;

               MySqlConnector.MySqlCommand cmd = new MySqlConnector.MySqlCommand("check_ip");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("_IPAddress", IPAddress1);
                DataTable dtt = db_connection.ExecuteQueryDataTableProcedure(cmd);
                if (dtt.Rows.Count > 2)
                {
                    MySqlConnector.MySqlCommand cmd1 = new MySqlConnector.MySqlCommand("block_ip");
                    cmd1.CommandType = CommandType.StoredProcedure;
                    cmd1.Parameters.AddWithValue("_IPAddress", IPAddress1);
                    cmd1.Parameters.AddWithValue("_Customer_ID", Customer_id);
                    cmd1.Parameters.AddWithValue("_record_date", Convert.ToDateTime(Encrypted_time1));
                    int i = Convert.ToInt32(db_connection.ExecuteScalarProcedure(cmd1));
                    if (i > 0)
                    {
                        msg1 = "Success";
                    }
                    else
                    {
                        msg1 = "NotSuccess";
                    }
                }
            }
            catch (Exception ex1)
            {
                DateTime Record_Insert_DateTime = DateTime.Now;
                string error = ex1.ToString().Replace("\'", "\\'");
                MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("SaveException");
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.Parameters.AddWithValue("_Record_Insert_DateTime", Record_Insert_DateTime);
                _cmd.Parameters.AddWithValue("_error", error);
                _cmd.Parameters.AddWithValue("_Client_ID", 1);
                int msg = db_connection.ExecuteNonQueryProcedure(_cmd);

            }
            return msg1;
        }

        


    }

    #region Location
    class GeoLocation
    {
        public string status { get; set; }
        public string description { get; set; }
        public Data data { get; set; }

    }
    class Data
    {
        public Geo geo { get; set; }
    }
    class Geo
    {
        public string host;
        public string ip { get; set; }
        public string rdns { get; set; }
        public string asn { get; set; }
        public string isp { get; set; }
        public string country_name { get; set; }
        public string country_code { get; set; }
        public string region_name { get; set; }
        public string region_code { get; set; }
        public string city { get; set; }
        public string postal_code { get; set; }
        public string continent_name { get; set; }
        public string continent_code { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string metro_code { get; set; }
        public string timezone { get; set; }
        public string datetime { get; set; }
    }
    #endregion


}
