using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net;
using System.IO;

using System.Web;
using Calyx_Solutions.Model;
using Calyx_Solutions.Service;
using RestSharp;
using System.Web.Razor.Tokenizer;
using Microsoft.Net.Http.Headers;
 
using System.Data.Common;
using Auth0.ManagementApi.Models;
using MySqlConnector;
using Newtonsoft.Json.Linq;
using static Calyx_Solutions.mtsIntegrationmethods;
using System.Xml;
using System.Text.Json;

namespace Calyx_Solutions.Service
{
    public class srvBeneficiary
    {
        private readonly HttpContext _srvBeneficiaryHttpContext;
        private readonly IDbConnection _dbConnection;
        public srvBeneficiary(HttpContext context)
        {
            this._srvBeneficiaryHttpContext = context;


        }

        public DataTable GetConfig(Model.Beneficiary obj)
        {
            DataTable dt = new DataTable();
            string Client_ID1_regex = CompanyInfo.testInjection(Convert.ToString(obj.Client_ID));
            string Client_ID_regex = validation.validate(Convert.ToString(obj.Client_ID), 0, 1, 1, 1, 1, 1, 1, 1, 1);

            if (Client_ID1_regex == "1" && Client_ID_regex != "false")
            {
                string whereclause = " and 1=1";
                MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("GetBenefConfig");
                if (obj.whereclause != "" && obj.whereclause != null)
                {
                    whereclause = obj.whereclause;
                }
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                _cmd.Parameters.AddWithValue("_whereclause", whereclause);

                dt = db_connection.ExecuteQueryDataTableProcedure(_cmd);
            }
            else
            {
                string msg = "Validation Error Client_ID_regex- " + Client_ID_regex;
                int stattus = (int)CompanyInfo.InsertActivityLogDetailsSecurity(msg, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), 1, "srvBeneficiary", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "GetConfig", 0, _srvBeneficiaryHttpContext);
            }
            return dt;
        }
        public DataTable SelectRelations(Model.Beneficiary obj)
        {
            MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("SP_Get_Relations");
            _cmd.CommandType = CommandType.StoredProcedure;
            DataTable dt = db_connection.ExecuteQueryDataTableProcedure(_cmd);
            return dt;
        }
        public DataTable BankValidations(Model.Beneficiary obj)
        {
            MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("Bank_Validations");
            _cmd.CommandType = CommandType.StoredProcedure;
            _cmd.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
            _cmd.Parameters.AddWithValue("_Country_ID", obj.Country_ID);
            DataTable dt = db_connection.ExecuteQueryDataTableProcedure(_cmd);
            return dt;
        }
        public DataTable CollectionTypeConfig(Model.Beneficiary obj)
        {
            string Client_ID_regex = validation.validate(Convert.ToString(obj.Client_ID), 0, 1, 1, 1, 1, 1, 1, 1, 1);
            DataTable dt = new DataTable();
            if (Client_ID_regex != "false")
            {
                MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("GetPayDepositType_Mapping");
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.Parameters.AddWithValue("_TransferTypeFlag", 0);
                _cmd.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                _cmd.Parameters.AddWithValue("_where", "");
                dt = db_connection.ExecuteQueryDataTableProcedure(_cmd);
            }
            else
            {
                string msg = "Validation Error Client_ID_regex- +" + Client_ID_regex;
                int stattus = (int)CompanyInfo.InsertActivityLogDetailsSecurity(msg, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), 1, "srvBeneficiary", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "CollectionTypeConfig", 0, _srvBeneficiaryHttpContext);
            }
            return dt;
        }
        public DataTable Select_BeneficiaryCountries(Model.Beneficiary obj)
        {
            List<Model.Beneficiary> _lst = new List<Model.Beneficiary>();

            MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("SP_Beneficiary_Countries");
            _cmd.CommandType = CommandType.StoredProcedure;
            _cmd.Parameters.AddWithValue("_Client_ID", obj.Client_ID);

            DataTable dt = db_connection.ExecuteQueryDataTableProcedure(_cmd);

            return dt;
        }
        public DataTable GetTrackDetails(Model.Beneficiary obj)
        {
            List<Model.Beneficiary> _lst = new List<Model.Beneficiary>();

            MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("Get_Track_Details");
            _cmd.CommandType = CommandType.StoredProcedure;
            _cmd.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
            _cmd.Parameters.AddWithValue("_Transaction_ID", obj.Id);
            DataTable dt = db_connection.ExecuteQueryDataTableProcedure(_cmd);
            return dt;
        }
        public DataTable GetTrackNames(Model.Beneficiary obj)
        {
            List<Model.Beneficiary> _lst = new List<Model.Beneficiary>();
            MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("Get_Track_Names");
            _cmd.CommandType = CommandType.StoredProcedure;
            DataTable dt = db_connection.ExecuteQueryDataTableProcedure(_cmd);
            return dt;
        }
        public DataTable GetReceiptConfig(Model.Beneficiary obj)
        {
            List<Model.Beneficiary> _lst = new List<Model.Beneficiary>();

            MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("SP_GetReceiptConfig");
            _cmd.CommandType = CommandType.StoredProcedure;
            _cmd.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
            DataTable dt = db_connection.ExecuteQueryDataTableProcedure(_cmd);
            return dt;
        }



        public DataTable GetProviders(Model.Beneficiary obj)
        {
            List<Model.Beneficiary> _lst = new List<Model.Beneficiary>();

            MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("Get_Providers");
            _cmd.CommandType = CommandType.StoredProcedure;
            _cmd.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
            _cmd.Parameters.AddWithValue("_Country_ID", obj.Country_ID);
            DataTable dt = db_connection.ExecuteQueryDataTableProcedure(_cmd);
            return dt;
        }

        public string[] AutoCheckSanctionList(Model.Beneficiary obj, HttpContext context)
        {
            string[] Alert_Msg = new string[7];
            string return_msg = string.Empty;
            int Customer_ID = Convert.ToInt32(CompanyInfo.Decrypt(obj.Customer_ID, true));
            try
            {
                int check_records_found = 0;
                string abc = ""; int Alert_count = 0; string Alert = string.Empty;

                db_connection dbConn = new db_connection();
                using (MySqlConnector.MySqlConnection con = new MySqlConnector.MySqlConnection(dbConn.ConnectionStringSection()))
                {
                    con.Open();
                    // MySqlTransaction transaction;
                    DataTable dt_cust = new DataTable();

                    MySqlConnector.MySqlCommand cmdupdate1 = new MySqlConnector.MySqlCommand("Get_Permissions", con);
                    cmdupdate1.CommandType = CommandType.StoredProcedure;
                    cmdupdate1.Parameters.AddWithValue("Per_ID", 39);
                    cmdupdate1.Parameters.AddWithValue("ClientID", obj.Client_ID);
                    //cmdupdate1.ExecuteScalar();
                    obj.CommentUserId = 1;
                    // int abcd = Convert.ToInt32(cmdupdate1.Parameters["Status"].Value);
                    DataTable dt1 = db_connection.ExecuteQueryDataTableProcedure(cmdupdate1); //Convert.ToInt32(cmdupdate1.Parameters["Status"].Value);
                    obj.per_status = Convert.ToInt32(dt1.Rows[0]["Status"]);
                    if (obj.per_status == 0)
                    {
                        DataTable dt = new DataTable(); DataTable dt11 = new DataTable(); DataTable dt12 = new DataTable();
                        DataTable dt13 = new DataTable(); DataTable dt14 = new DataTable(); DataTable dt15 = new DataTable();
                        //using (MySqlCommand cmd1 = new MySqlCommand("GetCustDetailsByID", con))
                        //{
                        MySqlConnector.MySqlCommand cmd1 = new MySqlConnector.MySqlCommand("GetCustDetailsByID", con);
                        cmd1.CommandType = CommandType.StoredProcedure;
                        cmd1.Parameters.AddWithValue("cust_ID", Customer_ID);

                        dt_cust = db_connection.ExecuteQueryDataTableProcedure(cmd1);//(DataTable)cmd1.ExecuteScalar();

                        //   if (dt_cust.Rows.Count > 0)
                        //      {
                        dt = (DataTable)DBHelper.GetSanctionList(obj.Beneficiary_Name);
                        DataRow[] dr = dt.Select("name='" + obj.Beneficiary_Name + "' OR   Passport_Details='" + obj.Beneficiary_Name + "'");
                        int uk_cnt = 0;
                        for (int i = 0; i < dr.Length; i++)
                        {
                            uk_cnt++;
                        }
                        dt11 = (DataTable)DBHelper.GetUNSanctionList(obj.Beneficiary_Name);
                        DataRow[] dr1 = dt11.Select("Name='" + obj.Beneficiary_Name + "' OR   INDIVIDUAL_DOCUMENT='" + obj.Beneficiary_Name + "'");
                        int un_cnt = 0;
                        for (int i = 0; i < dr1.Length; i++)
                        {
                            un_cnt++;
                        }
                        dt12 = (DataTable)DBHelper.GetUSASanctionList(obj.Beneficiary_Name);
                        DataRow[] dr2 = dt12.Select("Name='" + obj.Beneficiary_Name + "' ");
                        int usa_cnt = 0;
                        for (int i = 0; i < dr2.Length; i++)
                        {
                            usa_cnt++;
                        }
                        dt13 = (DataTable)DBHelper.GetEUSanctionList(obj.Beneficiary_Name);
                        DataRow[] dr3 = dt13.Select("Name='" + obj.Beneficiary_Name + "'  OR  citizenship_countryDescription='" + obj.Beneficiary_Name + "' ");
                        int eu_cnt = 0;
                        for (int i = 0; i < dr3.Length; i++)
                        {
                            eu_cnt++;
                        }
                        dt14 = (DataTable)DBHelper.GetAUDSanctionList(obj.Beneficiary_Name);
                        DataRow[] dr4 = dt14.Select("name='" + obj.Beneficiary_Name + "'  OR  Passport_Details='" + obj.Beneficiary_Name + "' ");
                        int aud_cnt = 0;
                        for (int i = 0; i < dr4.Length; i++)
                        {
                            aud_cnt++;
                        }
                        dt15 = (DataTable)DBHelper.GetPEPSanctionList(obj.Beneficiary_Name);
                        DataRow[] dr5 = dt15.Select("Name='" + obj.Beneficiary_Name + "'  ");
                        int pep_cnt = 0;
                        for (int i = 0; i < dr5.Length; i++)
                        {
                            pep_cnt++;
                        }
                        //  }
                        var flag = "";
                        //     obj.Name = obj.FirstName + ' ' + obj.LastName;
                        // obj.CustomerId = obj.Id;
                        obj.Id1 = 0;
                        obj.status = 0;
                        obj.RecordDate = obj.Record_Insert_DateTime;//Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));// (string)mtsmethods.gettodate(c);
                        obj.DeleteStatus = 0;
                        obj.whereclause = "";
                        // obj.Client_ID = obj.Client_ID;
                        //if (dt.Rows.Count > 0)
                        //{
                        obj.Id1 = dt.Rows.Count;
                        if (dt.Rows.Count > 0) { obj.status = 2; check_records_found++; }
                        flag = "0";
                        obj.whereclause = flag;

                        MySqlConnector.MySqlCommand cmd4 = new MySqlConnector.MySqlCommand("SPBenf_SanctionList_Save", con);
                        cmd4.CommandType = CommandType.StoredProcedure;

                        cmd4.Parameters.AddWithValue("Benf_Name", obj.Beneficiary_Name);
                        cmd4.Parameters.AddWithValue("CustomerId", 0);
                        cmd4.Parameters.AddWithValue("_Exact_Match", Convert.ToInt32(uk_cnt));
                        cmd4.Parameters.AddWithValue("Id", obj.Id1);//records count
                        cmd4.Parameters.AddWithValue("_date", obj.RecordDate);
                        cmd4.Parameters.AddWithValue("_status", obj.status);//legitimate
                        cmd4.Parameters.AddWithValue("wherclause", obj.whereclause);//flag 0 to 6
                        cmd4.Parameters.AddWithValue("userId", obj.CommentUserId);//user id
                        cmd4.Parameters.AddWithValue("deletestatus", obj.DeleteStatus);
                        cmd4.Parameters.AddWithValue("_Benf_ID", obj.Beneficiary_ID);


                        string msg = Convert.ToString(cmd4.ExecuteNonQuery());
                        if (msg == "0")
                        {
                            Alert_Msg[Alert_count] = Alert + " UK Sanction List";
                            Alert_count++;
                        }



                        obj.Id1 = dt11.Rows.Count;
                        if (dt11.Rows.Count > 0) { obj.status = 2; check_records_found++; }
                        flag = "3";
                        obj.whereclause = flag;

                        MySqlConnector.MySqlCommand cmd55 = new MySqlConnector.MySqlCommand("SPBenf_SanctionList_Save", con);
                        cmd55.CommandType = CommandType.StoredProcedure;

                        cmd55.Parameters.AddWithValue("Benf_Name", obj.Beneficiary_Name);
                        cmd55.Parameters.AddWithValue("CustomerId", 0);
                        cmd55.Parameters.AddWithValue("_Exact_Match", Convert.ToInt32(un_cnt));
                        cmd55.Parameters.AddWithValue("Id", obj.Id1);
                        cmd55.Parameters.AddWithValue("_date", obj.RecordDate);
                        cmd55.Parameters.AddWithValue("_status", obj.status);
                        cmd55.Parameters.AddWithValue("wherclause", obj.whereclause);
                        cmd55.Parameters.AddWithValue("userId", obj.CommentUserId);
                        cmd55.Parameters.AddWithValue("deletestatus", obj.DeleteStatus);
                        cmd55.Parameters.AddWithValue("_Benf_ID", obj.Beneficiary_ID);
                        string msg1 = Convert.ToString(cmd55.ExecuteNonQuery());
                        if (msg1 == "0")
                        {
                            Alert_Msg[Alert_count] = Alert + " UN Sanction List";
                            Alert_count++;
                        }




                        obj.Id1 = dt12.Rows.Count;
                        if (dt12.Rows.Count > 0) { obj.status = 2; check_records_found++; }
                        flag = "1";
                        obj.whereclause = flag;

                        MySqlConnector.MySqlCommand cmd6 = new MySqlConnector.MySqlCommand("SPBenf_SanctionList_Save", con);
                        cmd6.CommandType = CommandType.StoredProcedure;

                        cmd6.Parameters.AddWithValue("Benf_Name", obj.Beneficiary_Name);
                        cmd6.Parameters.AddWithValue("CustomerId", 0);
                        cmd6.Parameters.AddWithValue("Id", obj.Id1);
                        cmd6.Parameters.AddWithValue("_Exact_Match", Convert.ToInt32(usa_cnt));
                        cmd6.Parameters.AddWithValue("_date", obj.RecordDate);
                        cmd6.Parameters.AddWithValue("_status", obj.status);
                        cmd6.Parameters.AddWithValue("wherclause", obj.whereclause);
                        cmd6.Parameters.AddWithValue("userId", obj.CommentUserId);
                        cmd6.Parameters.AddWithValue("deletestatus", obj.DeleteStatus);
                        cmd6.Parameters.AddWithValue("_Benf_ID", obj.Beneficiary_ID);
                        string msg2 = Convert.ToString(cmd6.ExecuteNonQuery());
                        if (msg2 == "0")
                        {
                            Alert_Msg[Alert_count] = Alert + " USA Sanction List";
                            Alert_count++;
                        }




                        obj.Id1 = dt13.Rows.Count;
                        if (dt13.Rows.Count > 0) { obj.status = 2; check_records_found++; }
                        flag = "2";
                        obj.whereclause = flag;

                        MySqlConnector.MySqlCommand cmd7 = new MySqlConnector.MySqlCommand("SPBenf_SanctionList_Save", con);
                        cmd7.CommandType = CommandType.StoredProcedure;

                        cmd7.Parameters.AddWithValue("Benf_Name", obj.Beneficiary_Name);
                        cmd7.Parameters.AddWithValue("CustomerId", 0);
                        cmd7.Parameters.AddWithValue("_Exact_Match", Convert.ToInt32(eu_cnt));
                        cmd7.Parameters.AddWithValue("Id", obj.Id1);
                        cmd7.Parameters.AddWithValue("_date", obj.RecordDate);
                        cmd7.Parameters.AddWithValue("_status", obj.status);
                        cmd7.Parameters.AddWithValue("wherclause", obj.whereclause);
                        cmd7.Parameters.AddWithValue("userId", obj.CommentUserId);
                        cmd7.Parameters.AddWithValue("deletestatus", obj.DeleteStatus);
                        cmd7.Parameters.AddWithValue("_Benf_ID", obj.Beneficiary_ID);
                        string msg7 = Convert.ToString(cmd7.ExecuteNonQuery());
                        if (msg7 == "0")
                        {
                            Alert_Msg[Alert_count] = Alert + " EU Sanction List";
                            Alert_count++;
                        }

                        obj.Id1 = dt14.Rows.Count;
                        if (dt14.Rows.Count > 0) { obj.status = 2; check_records_found++; }
                        flag = "5";
                        obj.whereclause = flag;

                        MySqlConnector.MySqlCommand cmd8 = new MySqlConnector.MySqlCommand("SPBenf_SanctionList_Save", con);
                        cmd8.CommandType = CommandType.StoredProcedure;

                        cmd8.Parameters.AddWithValue("Benf_Name", obj.Beneficiary_Name);
                        cmd8.Parameters.AddWithValue("CustomerId", 0);
                        cmd8.Parameters.AddWithValue("_Exact_Match", Convert.ToInt32(aud_cnt));
                        cmd8.Parameters.AddWithValue("Id", obj.Id1);
                        cmd8.Parameters.AddWithValue("_date", obj.RecordDate);
                        cmd8.Parameters.AddWithValue("_status", obj.status);
                        cmd8.Parameters.AddWithValue("wherclause", obj.whereclause);
                        cmd8.Parameters.AddWithValue("userId", obj.CommentUserId);
                        cmd8.Parameters.AddWithValue("deletestatus", obj.DeleteStatus);
                        cmd8.Parameters.AddWithValue("_Benf_ID", obj.Beneficiary_ID);
                        string msg8 = Convert.ToString(cmd8.ExecuteNonQuery());
                        if (msg8 == "0")
                        {
                            Alert_Msg[Alert_count] = Alert + "AUD Sanction List";
                            Alert_count++;
                        }

                        obj.Id1 = dt15.Rows.Count;
                        if (dt14.Rows.Count > 0) { obj.status = 2; check_records_found++; }
                        flag = "6";
                        obj.whereclause = flag;

                        MySqlConnector.MySqlCommand cmd9 = new MySqlConnector.MySqlCommand("SPBenf_SanctionList_Save", con);
                        cmd9.CommandType = CommandType.StoredProcedure;

                        cmd9.Parameters.AddWithValue("Benf_Name", obj.Beneficiary_Name);
                        cmd9.Parameters.AddWithValue("CustomerId", 0);
                        cmd9.Parameters.AddWithValue("_Exact_Match", Convert.ToInt32(pep_cnt));
                        cmd9.Parameters.AddWithValue("Id", obj.Id1);
                        cmd9.Parameters.AddWithValue("_date", obj.RecordDate);
                        cmd9.Parameters.AddWithValue("_status", obj.status);
                        cmd9.Parameters.AddWithValue("wherclause", obj.whereclause);
                        cmd9.Parameters.AddWithValue("userId", obj.CommentUserId);
                        cmd9.Parameters.AddWithValue("deletestatus", obj.DeleteStatus);
                        cmd9.Parameters.AddWithValue("_Benf_ID", obj.Beneficiary_ID);
                        string msg9 = Convert.ToString(cmd9.ExecuteNonQuery());
                        if (msg9 == "0")
                        {
                            Alert_Msg[Alert_count] = Alert + " PEP Sanction List";
                            Alert_count++;
                        }



                        if (check_records_found > 0)//match found in sanction list
                        {
                            try
                            {
                                MySqlConnector.MySqlCommand cmd_sanctionlist_save = new MySqlConnector.MySqlCommand("SP_Black_And_WhiteList_Benf", con);
                                cmd_sanctionlist_save.CommandType = CommandType.StoredProcedure;
                                cmd_sanctionlist_save.Parameters.AddWithValue("_isblackListed", 1);
                                cmd_sanctionlist_save.Parameters.AddWithValue("_benf_id", obj.Beneficiary_ID);
                                cmd_sanctionlist_save.Parameters.AddWithValue("_client_id", obj.Client_ID);
                                cmd_sanctionlist_save.Parameters.AddWithValue("_flag", "BlackList");
                                int is_blacklisted = (int)cmd_sanctionlist_save.ExecuteNonQuery();
                                if (is_blacklisted > 0)
                                {
                                    Alert_Msg[6] = "BlackListed";
                                }
                                DataTable d1 = (DataTable)CompanyInfo.getCustomerDetails(obj.Client_ID, Customer_ID);
                                //send mail to admin for further investigation

                                //   string email = objLogin.UserName;
                                string body = string.Empty, subject = string.Empty;
                                string body1 = string.Empty;
                                //string template = "";
                                HttpWebRequest httpRequest = null, httpRequest1 = null;
                                DataTable dtc = CompanyInfo.get(obj.Client_ID, context);
                                if (dtc.Rows.Count > 0)
                                {
                                    DataTable d2 = (DataTable)CompanyInfo.getCustomerDetails(obj.Client_ID, Customer_ID);
                                    string sendmsg = " While adding Beneficiary " + obj.Beneficiary_Name + "  for Sender  " + d2.Rows[0]["WireTransfer_ReferanceNo"] + " ,there was a match found in the Sanctions List. Please check Beneficiary profile to investigate further. ";
                                    string company_name = Convert.ToString(dtc.Rows[0]["Company_Name"]);
                                    string URL = Convert.ToString(dtc.Rows[0]["Company_URL_Admin"]);
                                    string cust_url = Convert.ToString(dtc.Rows[0]["Company_URL_Customer"]);
                                    httpRequest = (HttpWebRequest)WebRequest.Create(URL + "Email/customemail.html");
                                    httpRequest.UserAgent = "Code Sample Web Client";

                                    HttpWebResponse webResponse = (HttpWebResponse)httpRequest.GetResponse();
                                    using (StreamReader reader = new StreamReader(webResponse.GetResponseStream()))
                                    {
                                        body = reader.ReadToEnd();
                                    }
                                    body = body.Replace("[name]", "Administrators");
                                    body = body.Replace("[msg]", sendmsg);


                                    DataTable dt_admin_list = (DataTable)CompanyInfo.getAdminEmailList(obj.Client_ID, obj.Branch_ID);
                                    string EmailID = "";

                                    if (dt_admin_list.Rows.Count > 0)
                                    {
                                        for (int a = 0; a < dt_admin_list.Rows.Count; a++)
                                        {
                                            string AdminEmailID = Convert.ToString(dt_admin_list.Rows[a]["Email_ID"]) + ",";
                                            EmailID += AdminEmailID;
                                        }
                                    }

                                    subject = company_name + " - Beneficiary Sanctions List Match - Alert " + d1.Rows[0]["WireTransfer_ReferanceNo"];
                                    //       string newsubject = company_name + " - " + subject + " - " + Convert.ToString(dt.Rows[0]["WireTransfer_ReferanceNo"]);
                                    string send_mail = (string)CompanyInfo.Send_Mail(dtc, EmailID, body, subject, obj.Client_ID, obj.Branch_ID, "", "", "", context);

                                }
                            }
                            catch (Exception _x)
                            {
                            }
                        }
                        else
                        {
                            Alert_Msg[6] = "NotBlackListed";
                        }
                        // string sendmsg = "Customer Registered successfully but we are unable to check following:" + "<br/>" + final_msg;
                    }
                }
            }

            catch (Exception _x)
            {
                //transaction.Rollback();
                throw _x;
            }
            finally
            {
                // con.Close();
            }
            /// }
            /// 
            return Alert_Msg;
            // return Alert_Msg;
        }

        public Model.Beneficiary Create(Model.Beneficiary obj, HttpContext context)
        {
            string[] Alert_Msg = new string[7];
            string Activity = string.Empty;
            //var context = System.Web.HttpContext.Current;
            string error_msg = ""; string error_invalid_data = "";
            //string Username = Convert.ToString(context.Request.Form["Username"]);
            string Username = context.User.Identity.Name;

            int m = 0; DataTable dsp = new DataTable();
            DateTime Birth_Date_DateTime = new DateTime();
            if (obj.Birth_Date != "Invalid date" && obj.Birth_Date != "" && obj.Birth_Date != null && (obj.Birth_Date).ToLower() != "undefined")
            {
                //Birth_Date_DateTime = Convert.ToDateTime(obj.Birth_Date);                    
            }
            int Customer_ID = Convert.ToInt32(CompanyInfo.Decrypt(obj.Customer_ID, true));

            string FirstName_regex = validation.validate_accnm(obj.Beneficiary_Name, 0);
            //string FirstName_regex = validation.validate(obj.Beneficiary_Name, 1, 1, 1, 1, 1, 1, 0, 1, 1);
            string address1_regex = validation.validate(obj.Beneficiary_Address, 1, 1, 1, 1, 1, 1, 1, 1, 0);
            string TelephoneNumber_regex = validation.validate(obj.Beneficiary_Telephone, 1, 1, 1, 0, 1, 1, 1, 1, 1);
            string MobileNumber_regex = validation.validate(obj.Beneficiary_Mobile, 1, 1, 1, 0, 1, 1, 1, 1, 1);
            string address2_regex = validation.validate(obj.Beneficiary_Address1, 1, 1, 1, 1, 1, 1, 1, 1, 0);
            string PostCode_regex = validation.validate(obj.Beneficiary_PostCode, 1, 1, 0, 1, 1, 1, 1, 1, 1);
            string AccountHolderName_regex = validation.validate_accnm(obj.AccountHolderName, 0);
            //string AccountHolderName_regex = validation.validate(obj.AccountHolderName, 1, 1, 1, 1, 1, 1, 0, 1, 1);
            string Account_Number_regex = validation.validate(obj.Account_Number, 1, 1, 0, 1, 1, 1, 1, 1, 1);
            string BankCode_regex = validation.validate(obj.BankCode, 1, 1, 0, 1, 1, 1, 1, 1, 1);
            string Branch_regex = validation.validate(obj.Branch, 1, 1, 1, 1, 1, 1, 1, 1, 0);
            string Ifsc_Code_regex = validation.validate(obj.Ifsc_Code, 1, 1, 0, 1, 1, 1, 1, 1, 1);
            string BranchCode_regex = validation.validate(obj.BranchCode, 1, 1, 0, 1, 1, 1, 1, 1, 1);
            string Benf_Iban_regex = validation.validate(obj.Benf_Iban, 1, 1, 0, 1, 1, 1, 1, 1, 1);
            string Benf_BIC_regex = validation.validate(obj.Benf_BIC, 1, 1, 0, 1, 1, 1, 1, 1, 1);

            MySqlCommand _cmdl = new MySqlCommand("GetPermissions");
            _cmdl.CommandType = CommandType.StoredProcedure;
            _cmdl.Parameters.AddWithValue("_whereclause", " and PID=204");
            _cmdl.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
            DataTable dttp = db_connection.ExecuteQueryDataTableProcedure(_cmdl);
            //Check Beneficiery AML AT the time of registration perm
            int check_benef_aml_through_compliance_assist = 1;
            DataRow[] dr = dttp.Select("PID=204");
            if (dr.Count() > 0)
            {
                foreach (DataRow drr in dr)
                {
                    check_benef_aml_through_compliance_assist = Convert.ToInt32(drr["Status_ForCustomer"]);
                }
            }

            int address_regex_len = 100;
            string HouseNumber_len = "true"; string Street_len = "true"; string AddressLine1_len = "true"; string AddressLine2_len = "true";
            if (Benf_BIC_regex == "false")
            {
                error_msg = error_msg + "BIC code should be alpanumeric without space.";
                error_invalid_data = error_invalid_data + " BIC code: " + obj.Benf_BIC;
            }
            if (BankCode_regex == "false")
            {
                error_msg = error_msg + "Bank code should be alpanumeric without space.";
                error_invalid_data = error_invalid_data + " BankCode: " + obj.BankCode;
            }
            if (Branch_regex == "false")
            {
                error_msg = error_msg + "Branch should be alphanumeric with space and special characters like , , , . , ' , ( , ) , { , } , - , @ , & ";
                error_invalid_data = error_invalid_data + " Branch: " + obj.Branch;
            }
            if (Branch_regex == "false")
            {
                error_msg = error_msg + "Branch code should be alphanumeric without space.";
                error_invalid_data = error_invalid_data + " Branch: " + obj.BranchCode;
            }
            if (Ifsc_Code_regex == "false")
            {
                error_msg = error_msg + "IFSC code should be alphanumeric without space.";
                error_invalid_data = error_invalid_data + " Branch: " + obj.Ifsc_Code;
            }
            if (Benf_Iban_regex == "false")
            {
                error_msg = error_msg + "IBAN code should be alphanumeric without space.";
                error_invalid_data = error_invalid_data + " IBAN: " + obj.Benf_Iban;
            }
            if (Account_Number_regex == "false")
            {
                error_msg = error_msg + "Account Number should be alphanumeric only without space.";
                error_invalid_data = error_invalid_data + " Account_Number: " + obj.Account_Number;
            }

            if (obj.Beneficiary_Address.Length >= address_regex_len)
            {
                AddressLine1_len = "false";
            }
            if (obj.Beneficiary_Address1.Length >= address_regex_len)
            {
                AddressLine2_len = "false";
            }
            if (FirstName_regex == "false" || AccountHolderName_regex == "false")
            {
                error_msg = error_msg + "Name should include only charactes with space.";
                if (FirstName_regex == "false" || AccountHolderName_regex == "false")
                {
                    error_invalid_data = error_invalid_data + " Beneficiary_Name: " + obj.Beneficiary_Name;
                }
                if (AccountHolderName_regex == "false")
                {
                    error_invalid_data = error_invalid_data + " Accountname: " + obj.AccountHolderName;
                }

            }
            if (address1_regex == "false" || address2_regex == "false")
            {
                string len_house = "";
                if (AddressLine1_len == "false" || AddressLine2_len == "false")
                {
                    len_house = len_house + "Address should be of valid length.";
                    error_invalid_data = error_invalid_data + "  Address length: " + obj.Beneficiary_Address.Length;
                }
                error_msg = error_msg + " Address should be alphanumeric with space and special characters like , , , . , ' , ( , ) , { , } , - , @ , & " + len_house;
                error_invalid_data = error_invalid_data + " Address: " + obj.Beneficiary_Address;
            }
            if (AddressLine1_len == "false" || AddressLine2_len == "false")
            {
                string len_house = "";
                error_invalid_data = error_invalid_data + "  Address length: " + obj.Beneficiary_Address.Length;
            }
            if (TelephoneNumber_regex == "false" || MobileNumber_regex == "false")
            {
                error_msg = error_msg + " Phone and Mobile number Should be numeric without space";
                if (TelephoneNumber_regex == "false")
                { error_invalid_data = error_invalid_data + "  MobileNumber: " + obj.Beneficiary_Mobile; }
                if (MobileNumber_regex == "false")
                { error_invalid_data = error_invalid_data + "  PhoneNumber: " + obj.Beneficiary_Mobile; }

            }
            if (PostCode_regex == "false")
            {
                error_msg = error_msg + " Postcode Should be alphanumeric without space";
                error_invalid_data = error_invalid_data + "  Postcode: " + obj.Beneficiary_PostCode;
            }
            if (Benf_BIC_regex == "" && Benf_Iban_regex == "" && BranchCode_regex == "" && Ifsc_Code_regex == "" && Branch_regex == "" && AccountHolderName_regex == "" && BankCode_regex == "" && Account_Number_regex == "" && Benf_Iban_regex == "" && PostCode_regex == "" && FirstName_regex == "" && address1_regex == "" && TelephoneNumber_regex == "" && MobileNumber_regex == "" && address2_regex == "" && PostCode_regex == "" && AddressLine1_len == "true" && AddressLine2_len == "true")
            {
                FirstName_regex = "true"; BranchCode_regex = "true"; Ifsc_Code_regex = "true"; Branch_regex = "true"; AccountHolderName_regex = "true"; BankCode_regex = "true";
                address1_regex = "true"; Account_Number_regex = "true";
                TelephoneNumber_regex = "true";
                MobileNumber_regex = "true";
                address2_regex = "true";
                PostCode_regex = "true";
                Benf_Iban_regex = "true";
                Benf_BIC_regex = "true";
                BranchCode_regex = "true";

            }
            if (Benf_BIC_regex != "false" && Benf_Iban_regex != "false" && BranchCode_regex != "false" && Ifsc_Code_regex != "false" && Branch_regex != "false" && AccountHolderName_regex != "false" && BankCode_regex != "false" && Account_Number_regex != "false" && Benf_Iban_regex != "false" && PostCode_regex != "false" && FirstName_regex != "false" && address1_regex != "false" && TelephoneNumber_regex != "false" && MobileNumber_regex != "false" && address2_regex != "false" && PostCode_regex != "false" && AddressLine1_len != "false" && AddressLine2_len != "false")
            {
                //Service.mts_connection _MTS = new Service.mts_connection();

                db_connection _dbConnection = new db_connection();

                using (MySqlConnector.MySqlConnection con = new MySqlConnector.MySqlConnection(_dbConnection.ConnectionStringSection()))
                {
                    con.Open();
                    obj.Record_Insert_DateTime = Convert.ToDateTime(CompanyInfo.gettime(obj.Client_ID, Customer_ID.ToString(), obj.Country_ID, context));
                    string Beneficiary_Name = CompanyInfo.testInjection(obj.Beneficiary_Name);
                    string Beneficiary_Address = CompanyInfo.testInjection(obj.Beneficiary_Address);
                    string Beneficiary_City_ID = CompanyInfo.testInjection(Convert.ToString(obj.Beneficiary_City_ID));
                    string Beneficiary_Country_ID = CompanyInfo.testInjection(Convert.ToString(obj.Beneficiary_Country_ID));
                    string Beneficiary_Telephone = CompanyInfo.testInjection(Convert.ToString(obj.Beneficiary_Telephone));
                    string Beneficiary_Mobile = CompanyInfo.testInjection(Convert.ToString(obj.Beneficiary_Mobile));
                    string Record_Insert_DateTime = CompanyInfo.testInjection(Convert.ToString(obj.Record_Insert_DateTime));
                    string Delete_Status = CompanyInfo.testInjection(Convert.ToString(obj.Delete_Status));
                    string Created_By_User_ID = CompanyInfo.testInjection(Convert.ToString(obj.Created_By_User_ID));

                    string Customer_ID1 = CompanyInfo.testInjection(Convert.ToString(Customer_ID));
                    string Beneficiary_Address1 = CompanyInfo.testInjection(Convert.ToString(obj.Beneficiary_Address1));
                    string Beneficiary_PostCode = CompanyInfo.testInjection(Convert.ToString(obj.Beneficiary_PostCode));
                    string cashcollection_flag = CompanyInfo.testInjection(Convert.ToString(obj.cashcollection_flag));
                    string Agent_MappingID = CompanyInfo.testInjection(Convert.ToString(obj.Agent_MappingID));
                    string Client_ID = CompanyInfo.testInjection(Convert.ToString(obj.Client_ID));
                    string Branch_ID = CompanyInfo.testInjection(Convert.ToString(obj.Branch_ID));
                    string Relation_ID = CompanyInfo.testInjection(Convert.ToString(obj.Relation_ID));

                    string Beneficiary_PostCode1 = CompanyInfo.testInjection(Convert.ToString(obj.Beneficiary_PostCode));

                    string AccountHolderName = CompanyInfo.testInjection(Convert.ToString(obj.AccountHolderName));
                    string Account_Number = CompanyInfo.testInjection(Convert.ToString(obj.Account_Number));
                    string BankCode = CompanyInfo.testInjection(Convert.ToString(obj.BankCode));
                    string Branch = CompanyInfo.testInjection(Convert.ToString(obj.Branch));
                    string Ifsc_Code = CompanyInfo.testInjection(Convert.ToString(obj.Ifsc_Code));
                    string BranchCode = CompanyInfo.testInjection(Convert.ToString(obj.BranchCode));
                    string Benf_Iban = CompanyInfo.testInjection(Convert.ToString(obj.Benf_Iban));
                    string Benf_BIC = CompanyInfo.testInjection(Convert.ToString(obj.Benf_BIC));
                    if (Branch == "1" && Ifsc_Code == "1" && BranchCode == "1" && Benf_Iban == "1" && Benf_BIC == "1" && BankCode == "1" && Account_Number == "1" && AccountHolderName == "1" && Beneficiary_PostCode1 == "1" && Beneficiary_Name == "1" && Beneficiary_Address == "1" && Beneficiary_City_ID == "1" && Beneficiary_Country_ID == "1" &&
                     Beneficiary_Telephone == "1" && Beneficiary_Mobile == "1" && Record_Insert_DateTime == "1" && Delete_Status == "1" &&
                     Created_By_User_ID == "1" && Customer_ID1 == "1" && Beneficiary_Address1 == "1" && Beneficiary_PostCode == "1" &&
                         cashcollection_flag == "1" && Agent_MappingID == "1" && Client_ID == "1" && Branch_ID == "1" && Relation_ID == "1"
                         )
                    {


                        //MySqlConnector.MySqlTransaction transaction;
                        //transaction = con.BeginTransaction(IsolationLevel.ReadCommitted);
                        MySqlConnector.MySqlTransaction transaction;
                        transaction = con.BeginTransaction(IsolationLevel.ReadCommitted);


                        try
                        {
                            using (MySqlConnector.MySqlCommand cmd = new MySqlConnector.MySqlCommand("SP_Save_Beneficiary", con))
                            {

                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Transaction = transaction;
                                cmd.CommandTimeout = 200;
                                cmd.Parameters.AddWithValue("_Beneficiary_Name", obj.Beneficiary_Name.Trim());
                                cmd.Parameters.AddWithValue("_Beneficiary_Address", obj.Beneficiary_Address.Trim());
                                cmd.Parameters.AddWithValue("_Beneficiary_City_ID", obj.Beneficiary_City_ID);
                                cmd.Parameters.AddWithValue("_Beneficiary_Country_ID", obj.Beneficiary_Country_ID);
                                cmd.Parameters.AddWithValue("_Beneficiary_Telephone", obj.Beneficiary_Telephone.Trim());

                                cmd.Parameters.AddWithValue("_Beneficiary_Mobile", obj.Beneficiary_Mobile.Trim());

                                cmd.Parameters.AddWithValue("_Record_Insert_DateTime", obj.Record_Insert_DateTime);
                                cmd.Parameters.AddWithValue("_Delete_Status", obj.Delete_Status);

                                cmd.Parameters.AddWithValue("_Created_By_User_ID", obj.Created_By_User_ID);
                                cmd.Parameters.AddWithValue("_Customer_ID", Customer_ID);

                                cmd.Parameters.AddWithValue("_Beneficiary_Address1", obj.Beneficiary_Address1.Trim());
                                cmd.Parameters.AddWithValue("_Beneficiary_PostCode", obj.Beneficiary_PostCode.Trim());
                                cmd.Parameters.AddWithValue("_State", obj.Beneficiary_State.Trim());
                                cmd.Parameters.AddWithValue("_cashcollection_flag", obj.cashcollection_flag);
                                cmd.Parameters.AddWithValue("_Agent_MappingID", obj.Agent_MappingID);
                                cmd.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                                cmd.Parameters.AddWithValue("_Branch_ID", obj.Branch_ID);
                                cmd.Parameters.AddWithValue("_Relation_ID", obj.Relation_ID);
                                cmd.Parameters.AddWithValue("_Iban_id", obj.Benf_Iban);
                                if (obj.Birth_Date != "Invalid date" && obj.Birth_Date != "" && obj.Birth_Date != null && (obj.Birth_Date).ToLower() != "undefined")
                                {
                                    DateTime DOB = DateTime.ParseExact(obj.Birth_Date, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                                    string Birth_Date = DOB.ToString("yyyy-MM-dd HH:mm:ss");
                                    cmd.Parameters.AddWithValue("_DOB", Convert.ToDateTime(Birth_Date)); //cmd.Parameters.AddWithValue("_DOB", Birth_Date_DateTime);
                                }
                                else
                                {
                                    cmd.Parameters.AddWithValue("_DOB", null);
                                }
                                cmd.Parameters.AddWithValue("_Beneficiary_Gender", obj.Beneficiary_Gender);
                                cmd.Parameters.AddWithValue("_Collection_type_Id", obj.Collection_type_Id);
                                cmd.Parameters.AddWithValue("_Wallet_Id", obj.Wallet_Id);
                                cmd.Parameters.AddWithValue("_Mobile_provider", obj.Mobile_provider);
                                cmd.Parameters.AddWithValue("_Wallet_provider", obj.Wallet_provider);
                                cmd.Parameters.AddWithValue("_BBank_ID", obj.BBank_ID);
                                cmd.Parameters.AddWithValue("_Account_Number", obj.Account_Number);
                                cmd.Parameters.AddWithValue("_BankCode", obj.BankCode);
                                cmd.Parameters.AddWithValue("_Branch", obj.Branch);


                                cmd.Parameters.Add(new MySqlConnector.MySqlParameter("_Benf_ID", MySqlConnector.MySqlDbType.Int32));
                                cmd.Parameters["_Benf_ID"].Direction = ParameterDirection.Output;

                                cmd.Parameters.Add(new MySqlConnector.MySqlParameter("_existBenf", MySqlConnector.MySqlDbType.Int32));
                                cmd.Parameters["_existBenf"].Direction = ParameterDirection.Output;

                                int n = cmd.ExecuteNonQuery();

                                //    obj.ExistBeneficiary = Convert.ToInt32(cmd.Parameters["_existBenf"].Value);
                                object ExistBeneficiary = cmd.Parameters["_existBenf"].Value;
                                ExistBeneficiary = (ExistBeneficiary == DBNull.Value) ? null : ExistBeneficiary;
                                // row["Transaction_Master_ID"] != DBNull.Value;

                                obj.ExistBeneficiary = Convert.ToInt32(ExistBeneficiary);
                                if (obj.ExistBeneficiary == 1)
                                {
                                    //return "exist_mobile";
                                    obj.Message = "exist_Beneficiary";
                                    string Activity1 = "<b>Add Benficiary: Beneficiary already exists.</b> Beneficiary : " + obj.Beneficiary_Name + " Mobile: " + obj.Beneficiary_Mobile + " Country: " + obj.Beneficiary_Country_ID + " City: " + obj.Beneficiary_City_ID + "";
                                    if (obj.cashcollection_flag == 1) //uncheked
                                    {
                                        Activity1 = Activity1 + " Account_Number : " + obj.Account_Number + " Branch: " + obj.Branch + " BBank_ID: " + obj.BBank_ID + "";
                                    }
                                    int stattus = (int)CompanyInfo.InsertActivityLogDetails(Activity, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(Customer_ID), "Add Benficiary", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "Add Benficiary", context);
                                }
                                else
                                {
                                    if (obj.Beneficiary_ID == 0) // insert
                                    {
                                        try
                                        {
                                            obj.Beneficiary_ID = Convert.ToInt32(cmd.Parameters["_Benf_ID"].Value);

                                        }
                                        catch (Exception)
                                        {

                                            //transaction.Rollback();
                                            //throw new System.InvalidOperationException("This company allready exist ..");
                                        }
                                    }

                                    //obj.Beneficiary_ID = 0;
                                    //object result = db_connection.ExecuteScalarProcedure(cmd);

                                    //result = (result == DBNull.Value) ? null : result;
                                    //obj.Beneficiary_ID = Convert.ToInt32(result);
                                    cmd.Dispose();

                                    if (obj.Beneficiary_ID != 0)
                                    {
                                        //Parvej changes to add defauly limits to beneficiary
                                        #region Add default limits to beneficiery

                                        try
                                        {
                                            int p = 0;
                                            using (MySqlCommand cmdbenflimit = new MySqlCommand("Add_default_limits_to_beneficiery"))
                                            {
                                                cmdbenflimit.CommandType = CommandType.StoredProcedure;
                                                cmdbenflimit.Parameters.AddWithValue("_Beneficiary_ID", obj.Beneficiary_ID);
                                                cmdbenflimit.Parameters.AddWithValue("_Customer_ID", Customer_ID);
                                                cmdbenflimit.Parameters.AddWithValue("_Beneficiary_Name", obj.Beneficiary_Name);
                                                cmdbenflimit.Parameters.AddWithValue("_Record_Insert_DateTime", obj.Record_Insert_DateTime);
                                                cmdbenflimit.Parameters.AddWithValue("_Update_Date", obj.Record_Insert_DateTime);
                                                cmdbenflimit.Parameters.AddWithValue("_Start_Date", obj.Record_Insert_DateTime);
                                                cmdbenflimit.Parameters.AddWithValue("_Client_id", obj.Client_ID);
                                                cmdbenflimit.Parameters.AddWithValue("_branch_id", obj.Branch_ID);


                                                p = db_connection.ExecuteNonQueryProcedure(cmdbenflimit);
                                            }

                                            string Activitys1 = "";
                                            if (p > 0)
                                            {
                                                Activitys1 = "Successfully Insert Default Limits to Customer Id : " + obj.Customer_ID + ",Beneficiary_ID : " + obj.Beneficiary_ID + ", User ID :" + obj.User_ID;
                                            }
                                            else
                                            {
                                                Activitys1 = "Failed to Insert Default Limits to Customer Id : " + obj.Customer_ID + ",Beneficiary_ID : " + obj.Beneficiary_ID + ", User ID :" + obj.User_ID;
                                            }
                                            int stattus = (int)CompanyInfo.InsertActivityLogDetails(Activitys1, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(Customer_ID), "App Add Beneficiary", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "App Add Beneficiary", context);
                                        }
                                        catch (Exception ex)
                                        {
                                            string stattus = (string)CompanyInfo.InsertErrorLogDetails(ex.Message.Replace("\'", "\\'"), 0, "App Add Beneficiary", obj.Branch_ID, obj.Client_ID);
                                        }

                                        #endregion

                                        #region insert into Benef Collection Mapping table
                                        try
                                        {
                                            MySqlCommand cmdaw = new MySqlCommand("Insert_benef_collectiontype_mapping");
                                            cmdaw.CommandType = CommandType.StoredProcedure;
                                            cmdaw.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                                            cmdaw.Parameters.AddWithValue("_Branch_ID", obj.Branch_ID);
                                            cmdaw.Parameters.AddWithValue("_User_ID", obj.Created_By_User_ID);

                                            cmdaw.Parameters.AddWithValue("_Customer_ID", Customer_ID);
                                            cmdaw.Parameters.AddWithValue("_Beneficiary_ID", obj.Beneficiary_ID);

                                            cmdaw.Parameters.AddWithValue("_Collection_type_Id", obj.Collection_type_Id);
                                            cmdaw.Parameters.AddWithValue("_Record_Insert_DateTime", obj.Record_Insert_DateTime);

                                            cmdaw.Parameters.Add(new MySqlParameter("_existflag", MySqlDbType.Int32));
                                            cmdaw.Parameters["_existflag"].Direction = ParameterDirection.Output;

                                            int p = db_connection.ExecuteNonQueryProcedure(cmdaw);

                                            var insertflag = Convert.ToInt32(cmdaw.Parameters["_existflag"].Value);

                                            string Activitys1 = "Already Exist Bnef Collection Mapping Customer Id : " + Customer_ID + ",Beneficiary_ID : " + obj.Beneficiary_ID + ",Collection Type Id : " + obj.Collection_type_Id + ", User ID :" + obj.Created_By_User_ID;
                                            if (insertflag > 0)
                                            {
                                                Activitys1 = "Successfully Insert Into Benef Collection Mapping Customer Id : " + Customer_ID + ",Beneficiary_ID : " + obj.Beneficiary_ID + ",Collection Type Id : " + obj.Collection_type_Id + ", User ID :" + obj.Created_By_User_ID;
                                            }
                                            int stattus = (int)CompanyInfo.InsertActivityLogDetails(Activitys1, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(Customer_ID), "App Add Beneficiary", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "App Add Beneficiary", context);
                                        }
                                        catch (Exception ex)
                                        {

                                            string stattus = (string)CompanyInfo.InsertErrorLogDetails(ex.Message.Replace("\'", "\\'"), 0, "App Add Beneficiary", obj.Branch_ID, obj.Client_ID);

                                        }
                                        #endregion



                                        try
                                        {
                                            #region beneficiary probable match
                                            obj.whereclause = " Beneficiary_ID != " + obj.Beneficiary_ID + " and Beneficiary_Country_ID =" + obj.Beneficiary_Country_ID + " and Beneficiary_City_ID =" + obj.Beneficiary_City_ID + " and bm.Client_ID= " + obj.Client_ID;
                                            if (obj.Birth_Date != "" && obj.Birth_Date != null && obj.Birth_Date != "null")
                                            {
                                                obj.whereclause = obj.whereclause + " and (date(DateOf_Birth) =date(" + obj.Birth_Date + ") or 1=1 ) ";
                                            }
                                            if (obj.Beneficiary_Address != "" && obj.Beneficiary_Address != null && obj.Beneficiary_Address != "null")
                                            {
                                                obj.whereclause = obj.whereclause + " AND   (Beneficiary_Address = '" + obj.Beneficiary_Address + "' or 1=1)";
                                            }
                                            MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("Beneficiary_probable_match");
                                            _cmd.CommandType = CommandType.StoredProcedure;
                                            _cmd.Parameters.AddWithValue("_whereclause", obj.whereclause);
                                            _cmd.Parameters.AddWithValue("_first_name", obj.First_Name);
                                            _cmd.Parameters.AddWithValue("_middle_name", obj.Middle_Name);
                                            _cmd.Parameters.AddWithValue("_last_name", obj.Last_Name);
                                            dsp = db_connection.ExecuteQueryDataTableProcedure(_cmd);
                                            m = dsp.Rows.Count;

                                            #endregion
                                        }
                                        catch (Exception exb) { }
                                        Model.Beneficiary _Objbenef = new Model.Beneficiary();
                                        _Objbenef.Beneficiary_ID = obj.Beneficiary_ID;
                                        _Objbenef.Delete_Status = 0;

                                        string notification_icon = "beneficiary.jpg";
                                        string notification_message = "<span class='cls-admin'>New <strong class='cls-new-benf'>Beneficiary</strong> successfully added.</span><span class='cls-customer'></span>";
                                        MySqlConnector.MySqlCommand cmd2 = new MySqlConnector.MySqlCommand("SP_Save_Beneificiary_BankDetails");
                                        cmd2.CommandType = CommandType.StoredProcedure;
                                        cmd2.Transaction = transaction;
                                        cmd2.Connection = con;
                                        cmd2.Parameters.AddWithValue("_Notification", notification_message);
                                        cmd2.Parameters.AddWithValue("_Notification_Icon", notification_icon);
                                        cmd2.Parameters.AddWithValue("_Branch_ID", obj.Branch_ID);
                                        cmd2.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                                        cmd2.Parameters.AddWithValue("_Beneficiary_ID", obj.Beneficiary_ID);
                                        cmd2.Parameters.AddWithValue("_AccountHolderName", obj.AccountHolderName);
                                        cmd2.Parameters.AddWithValue("_BBank_ID", obj.BBank_ID);
                                        cmd2.Parameters.AddWithValue("_Account_Number", obj.Account_Number);
                                        cmd2.Parameters.AddWithValue("_BankCode", obj.BankCode);

                                        cmd2.Parameters.AddWithValue("_Branch", obj.Branch);
                                        cmd2.Parameters.AddWithValue("_Record_Insert_DateTime", obj.Record_Insert_DateTime);
                                        cmd2.Parameters.AddWithValue("_Delete_Status", obj.Delete_Status);
                                        cmd2.Parameters.AddWithValue("_Customer_ID", Customer_ID);
                                        cmd2.Parameters.AddWithValue("_Ifsc_Code", obj.Ifsc_Code);
                                        cmd2.Parameters.AddWithValue("_BranchCode", obj.BranchCode);
                                        cmd2.Parameters.AddWithValue("_Benf_Iban", obj.Benf_Iban);
                                        cmd2.Parameters.AddWithValue("_Benf_BIC", obj.Benf_BIC);
                                        cmd2.Parameters.AddWithValue("_BankBranch_ID", obj.BankBranch_ID);
                                        string[] check_alert = new string[7];
                                        int n1 = cmd2.ExecuteNonQuery();
                                        if (n1 > 0)//success
                                        {
                                            try
                                            {
                                                DataTable dt_notif = CompanyInfo.set_notification_data(5);
                                                if (dt_notif.Rows.Count > 0)
                                                {
                                                    int SMS = Convert.ToInt32(dt_notif.Rows[0]["SMS"]);
                                                    int Email = Convert.ToInt32(dt_notif.Rows[0]["Email"]);
                                                    int Notif_status = Convert.ToInt32(dt_notif.Rows[0]["Notification"]);
                                                    string notification_msg = Convert.ToString(dt_notif.Rows[0]["notification_msg"]);
                                                    if (notification_msg.Contains("[Benf_name]") == true)
                                                    {
                                                        notification_msg = notification_msg.Replace("[Benf_name]", Convert.ToString(Beneficiary_Name));
                                                    }

                                                    int i = CompanyInfo.check_notification_perm(Convert.ToString(Customer_ID), obj.Client_ID, obj.Branch_ID, 2, 5, Convert.ToDateTime(obj.RecordDate), 1, SMS, Email, Notif_status, "App - Beneficiary Added Notification - 5", notification_msg, context);
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                CompanyInfo.InsertrequestLogTracker("create beneficiary erro : " + ex.ToString(), 0, 0, 0, 0, "App - Beneficiary Added Notification - 5", Convert.ToInt32(0), Convert.ToInt32(0), "", context);
                                            }

                                            //transaction.Commit();
                                            #region check loaction
                                            DataTable dtc = CompanyInfo.get(obj.Client_ID, context);
                                            try
                                            {

                                                string act = "";

                                                Boolean chkLocation = true;
                                                string country_log = "";
                                                string device_ty = "";

                                                try
                                                {

                                                    DataTable chkLocation1 = CompanyInfo.check_location(obj.Client_ID, obj.userAgent, context);
                                                    try
                                                    {
                                                        chkLocation = Convert.ToBoolean(chkLocation1.Rows[0]["is_valid"]);
                                                        country_log = Convert.ToString(chkLocation1.Rows[0]["Country"]);
                                                        device_ty = Convert.ToString(chkLocation1.Rows[0]["device_ty"]);
                                                    }
                                                    catch
                                                    {
                                                    }

                                                }
                                                catch (Exception ex)
                                                {

                                                }
                                                if (!chkLocation)
                                                {
                                                    //Start Parth added for restricting to send email if location is null or empty
                                                    #region restricting to send email if location is null or empty

                                                    _ = Task.Run(() => CompanyInfo.InsertActivityLogDetails(country_log, 0, 0, 0, 0, "SaveBeneficiary: Restrict email for Location", 0, 1, "", context));

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
                                                            DataTable dt_notif = CompanyInfo.set_notification_data(27);
                                                            if (dt_notif.Rows.Count > 0)
                                                            {
                                                                int SMS = Convert.ToInt32(dt_notif.Rows[0]["SMS"]);
                                                                int Email = Convert.ToInt32(dt_notif.Rows[0]["Email"]);
                                                                int Notif_status = Convert.ToInt32(dt_notif.Rows[0]["Notification"]);
                                                                string notification_msg = Convert.ToString(dt_notif.Rows[0]["notification_msg"]);
                                                                if (notification_msg.Contains("[Benf_name]") == true)
                                                                {
                                                                    notification_msg = notification_msg.Replace("[Benf_name]", Convert.ToString(obj.Beneficiary_Name));
                                                                }

                                                                int i = CompanyInfo.check_notification_perm(Convert.ToString(Customer_ID), obj.Client_ID, obj.Branch_ID, 2, 27, Convert.ToDateTime(obj.RecordDate), 1, SMS, Email, Notif_status, "App - Beneficiary Added from new location Notification - 27", notification_msg, context);
                                                            }
                                                        }
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                    }
                                                    string body = string.Empty, subject = string.Empty;
                                                    string body1 = string.Empty;
                                                    HttpWebRequest httpRequest = null, httpRequest1 = null;
                                                    DataTable d2 = (DataTable)CompanyInfo.getCustomerDetails(obj.Client_ID, Customer_ID);
                                                    string sendmsg1 = "Beneficiary Added From New Location";
                                                    string company_name = Convert.ToString(dtc.Rows[0]["Company_Name"]);
                                                    string URL = Convert.ToString(dtc.Rows[0]["Company_URL_Admin"]);
                                                    string cust_url = Convert.ToString(dtc.Rows[0]["Company_URL_Customer"]);
                                                    httpRequest = (HttpWebRequest)WebRequest.Create(URL + "Email/new-login.html");
                                                    httpRequest.UserAgent = "Code Sample Web Client";
                                                    HttpWebResponse webResponse = (HttpWebResponse)httpRequest.GetResponse();
                                                    using (StreamReader reader = new StreamReader(webResponse.GetResponseStream()))
                                                    {
                                                        body = reader.ReadToEnd();
                                                    }
                                                    body = body.Replace("[name]", Convert.ToString(d2.Rows[0]["First_Name"]));
                                                    string enc_ref = CompanyInfo.Encrypt(Convert.ToString(d2.Rows[0]["WireTransfer_ReferanceNo"]), true);
                                                    string link = cust_url + "/secure-account-verfiy?reference=" + enc_ref;
                                                    body = body.Replace("[link]", link);
                                                    body = body.Replace("[New_Login_Detected]", "Beneficiary Added From New Location ");
                                                    body = body.Replace("[country]", country_log);
                                                    body = body.Replace("[time]", (Convert.ToDateTime(CompanyInfo.gettime(obj.Client_ID, context))).ToString("dddd, dd MMMM yyyy HH:mm:ss"));
                                                    body = body.Replace("[location_msg]", "We noticed a new beneficiary upload to your account from new location that we don't recognise. If this wasn't you, we'll help you secure your account.");
                                                    body = body.Replace("[device]", device_ty);




                                                    string EmailID = "";
                                                    EmailID = Convert.ToString(d2.Rows[0]["Email_ID"]);


                                                    subject = company_name + " - Beneficiary Added From New Location - Alert " + d2.Rows[0]["WireTransfer_ReferanceNo"];
                                                    string send_mail = (string)CompanyInfo.Send_Mail(dtc, EmailID, body, subject, obj.Client_ID, obj.Branch_ID, "", "", "", context);

                                                    //Notification
                                                    notification_icon = "beneficiary.jpg";
                                                    notification_message = "<span class='cls-admin'>Beneficiary <strong class='cls-new-benf'>Added From New Location </strong></span>";
                                                    CompanyInfo.save_notification_compliance(notification_message, notification_icon, Convert.ToString(Customer_ID), Convert.ToDateTime(obj.Record_Insert_DateTime), obj.Client_ID, 1, 0, obj.Branch_ID, 0, 1, 1, 0, context);
                                                }
                                            }
                                            catch
                                            {

                                            }
                                            #endregion
                                            transaction.Commit();
                                            check_alert = AutoCheckSanctionList(obj, context);
                                            int var1 = 0;
                                            List<string> l = new List<string>();
                                            var arr = Alert_Msg.Union(check_alert).ToArray();
                                            if (arr.Length > 0)
                                            {
                                                for (int j = 0; j < arr.Length - 1; j++)
                                                {
                                                    if (arr[j] != "" && arr[j] != "null" && arr[j] != null)
                                                    {
                                                        //    arr[j] = arr[j];
                                                        l.Add(arr[j]);
                                                        //var1 = 1;
                                                    }
                                                }
                                            }
                                            string final_msg = string.Empty; int cnt = 1;
                                            for (int i = var1; i < l.Count; i++)
                                            {

                                                if (l[i] != "" && l[i] != "null" && l[i] != null)
                                                {
                                                    final_msg += cnt + "  ." + l[i] + "<br />";
                                                    cnt++;
                                                }
                                            }
                                            string sendmsg = "Beneficiary details saved successfully but we are unable to check following:" + "<br/>" + final_msg;
                                            if (l.Count > var1)
                                            {
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

                                                dtc = CompanyInfo.get(obj.Client_ID, context);
                                                string company_name = Convert.ToString(dtc.Rows[0]["Company_Name"]);
                                                string URL = Convert.ToString(dtc.Rows[0]["Company_URL_Admin"]);
                                                string cust_url = Convert.ToString(dtc.Rows[0]["Company_URL_Customer"]);



                                                HttpWebRequest httpRequest;

                                                httpRequest = (HttpWebRequest)WebRequest.Create(URL + "Email/customemail.html");

                                                httpRequest.UserAgent = "Code Sample Web Client";
                                                HttpWebResponse webResponse = (HttpWebResponse)httpRequest.GetResponse();
                                                using (StreamReader reader = new StreamReader(webResponse.GetResponseStream()))
                                                {
                                                    body = reader.ReadToEnd();
                                                }
                                                body = body.Replace("[name]", "Administrators");

                                                body = body.Replace("[msg]", sendmsg);


                                                DataTable d1 = (DataTable)CompanyInfo.getCustomerDetails(obj.Client_ID, Customer_ID);



                                                subject = company_name + " - Compliance Alert  " + d1.Rows[0]["WireTransfer_ReferanceNo"];



                                                //  HttpWebRequest httpRequest1 = (HttpWebRequest)WebRequest.Create("" + HttpContext.Current.Session["EmailUrl"] + "Email/RegisteredSuccessMail.txt");

                                                //httpRequest1.UserAgent = "Code Sample Web Client";
                                                //HttpWebResponse webResponse1 = (HttpWebResponse)httpRequest1.GetResponse();
                                                //using (StreamReader reader = new StreamReader(webResponse1.GetResponseStream()))
                                                //{
                                                //    subject = reader.ReadLine();
                                                //}
                                                string send_mail = (string)CompanyInfo.Send_Mail(dtc, EmailID, body, subject, obj.Client_ID, obj.Branch_ID, "", "", "", context);
                                                //  string    mail_send = (string)CompanyInfo.Send_Mail(email, body, subject, Convert.ToInt32(c.Client_ID), Convert.ToInt32(c.Branch_ID), "Alert Admins", "", "");
                                            }
                                        }
                                        cmd2.Dispose();
                                        if (n > 0 && obj.Beneficiary_ID != 0)
                                        {
                                            if (m > 0)
                                            {
                                                try
                                                {
                                                    m = (int)(dsp.Rows[0]["Beneficiary_ID"]);
                                                    MySqlConnector.MySqlCommand cmd_update = new MySqlConnector.MySqlCommand("update beneficiary_master set ProbableMatch_Flag=0,Matching_ID=@Matching_ID  where Beneficiary_ID=@Beneficiary_Id and Client_ID=@Client_ID");
                                                    cmd_update.Parameters.AddWithValue("@Matching_ID", m);
                                                    cmd_update.Parameters.AddWithValue("@Beneficiary_Id", obj.Beneficiary_ID);
                                                    cmd_update.Parameters.AddWithValue("@Client_ID", obj.Client_ID);
                                                    db_connection.ExecuteNonQueryProcedure(cmd_update);
                                                }
                                                catch (Exception ex)
                                                {
                                                    Activity = "";
                                                    Activity = " Beneficiary probable match found ";
                                                }
                                                //string Activity = "";
                                                //Activity = " Beneficiary probable match found ";
                                                //mtsmethods.InsertActivityLogDetails_Beneficiary(Activity, user_id, 0, user_id, cust_id, "UpdateBenf_Details", c.Branch_ID, c.Client_ID, c.Beneficiary_ID);
                                                Activity = "Add Benficiary: Probable match found for beneficiary " + obj.First_Name + " " + obj.Middle_Name + " " + obj.Last_Name + ".";
                                                int stattus = (int)CompanyInfo.InsertActivityLogDetails(Activity, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(Customer_ID), "Update", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "Update Beneficiary", context);
                                            }
                                            obj.Message = "success";
                                            Activity = "<b>" + Username + "</b>" + "Beneficiary Inserted.  </br>";
                                        }
                                        else
                                        {
                                            obj.Message = "notsuccess";
                                            Activity = "<b>" + Username + "</b>" + "Failed to update Beneficiary details.  </br>";
                                        }
                                        obj.Message = "success";
                                        Activity = "<b>" + Username + "</b>" + "Beneficiary Updated.  </br>";
                                        int stattus1 = (int)CompanyInfo.InsertActivityLogDetails(Activity, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(Customer_ID), "Create", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "Insert Beneficiary", context);
                                    }
                                }


                            }
                        }
                        catch (Exception ex)
                        {
                            int stattus = (int)CompanyInfo.InsertActivityLogDetailsSecurity(ex.ToString(), Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), 1, "srvBenificiary", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "create", 0, context);
                            //transaction.Rollback();
                            throw ex;
                        }
                    }
                    else
                    {
                        string msg = "SQl Enjection detected";
                        obj.Id = -1;
                        // Activity = "<b>" + Username + "</b>" + " Password changed.  </br>";
                        //int stattus = (int)CompanyInfo.InsertActivityLogDetails(msg, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID),obj.Id, "IsValidCustomer", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "IsValidCustomer");
                        int stattus = (int)CompanyInfo.InsertActivityLogDetailsSecurity(msg, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), 1, "srvBenificiary", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "create", 0, context);
                    }

                }
            }
            else
            {
                obj.Id = 0;
                string msg = error_invalid_data;
                obj.whereclause = "Validation Failed";
                obj.Message = msg;
                int stattus = (int)CompanyInfo.InsertActivityLogDetails(msg, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), 1, "srvBenificiary", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "Create", context);
            }
            return obj;
            //}

            //     return obj.Beneficiary_ID;
        }

        public Model.Beneficiary Update_RelationDetails(Model.Beneficiary obj, HttpContext context)
        {
            string Activity = string.Empty;
            //string Username = Convert.ToString(context.Request.Form["Username"]);
            mts_connection _MTS = new mts_connection();
            int Customer_ID = Convert.ToInt32(CompanyInfo.Decrypt(obj.Customer_ID, true));
            using (MySqlConnection con = new MySqlConnection(_MTS.WebConnSetting()))
            {
                con.Open();


                MySqlCommand cmd = new MySqlCommand("SP_Update_BeneficiaryRelation");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = con;
                cmd.Parameters.AddWithValue("_Beneficiary_ID", obj.Beneficiary_ID);
                cmd.Parameters.AddWithValue("_Relation_ID", obj.Relation_ID);
                cmd.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                int result = cmd.ExecuteNonQuery();
                if (result > 0)
                {
                    obj.Message = "success";
                }
                else
                {
                    obj.Message = "notsuccess";
                }
                cmd.Dispose();
                con.Close();
                Activity = /*"<b>" + Username + "</b>" +*/ "Beneficiary Relation Updated.  </br>";
                int stattus = (int)CompanyInfo.InsertActivityLogDetails(Activity, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(Customer_ID), "Update", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "Update Beneficiary Relation", context);
            }

            return obj;
        }

        public Model.Beneficiary Update_MobileDetails(Model.Beneficiary obj, HttpContext context)
        {
            string Activity = string.Empty;
            //string Username = Convert.ToString(context.Request.Form["Username"]);
            mts_connection _MTS = new mts_connection();
            int Customer_ID = Convert.ToInt32(CompanyInfo.Decrypt(obj.Customer_ID, true));
            using (MySqlConnection con = new MySqlConnection(_MTS.WebConnSetting()))
            {
                con.Open();

                MySqlCommand cmd = new MySqlCommand("SP_Update_BeneficiaryMobile");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = con;
                cmd.Parameters.AddWithValue("_Beneficiary_ID", obj.Beneficiary_ID);
                cmd.Parameters.AddWithValue("_Mobile", obj.Beneficiary_Mobile);
                cmd.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                int result = cmd.ExecuteNonQuery();
                if (result > 0)
                {
                    obj.Message = "success";
                }
                else
                {
                    obj.Message = "notsuccess";
                }
                cmd.Dispose();
                con.Close();
                Activity = /*"<b>" + Username + "</b>" +*/ "Beneficiary Mobile Updated.  </br>";
                int stattus = (int)CompanyInfo.InsertActivityLogDetails(Activity, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(Customer_ID), "Update", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "Update Beneficiary Mobile", context);
            }

            return obj;
        }

        public Model.Beneficiary Update(Model.Beneficiary obj, HttpContext context)
        {
            int Customer_ID = Convert.ToInt32(CompanyInfo.Decrypt(obj.Customer_ID, true));
            string Activity = string.Empty;
            //var context = System.Web.HttpContext.Current;
            // string Username = Convert.ToString(context.Request.Form["Username"]);
            DataTable dtbenfconfig = new DataTable();
            mts_connection _MTS = new mts_connection();
            using (MySqlConnector.MySqlConnection con = new MySqlConnector.MySqlConnection(_MTS.WebConnSetting()))
            {
                con.Open();
                string error_msg = ""; string error_invalid_data = "";
                DateTime Birth_Date_DateTime = new DateTime();
                if (obj.Birth_Date != "Invalid date" && obj.Birth_Date != "" && obj.Birth_Date != null && (obj.Birth_Date).ToLower() != "undefined")
                {
                    //Birth_Date_DateTime = Convert.ToDateTime(obj.Birth_Date);                    
                }
                string FirstName_regex = validation.validate_accnm(obj.Beneficiary_Name, 0);// validation.validate(obj.Beneficiary_Name, 1, 1, 1, 1, 1, 1, 0, 1, 1);
                string address1_regex = validation.validate(obj.Beneficiary_Address, 1, 1, 1, 1, 1, 1, 1, 1, 0);
                string TelephoneNumber_regex = validation.validate(obj.Beneficiary_Telephone, 1, 1, 1, 0, 1, 1, 1, 1, 1);
                string MobileNumber_regex = validation.validate(obj.Beneficiary_Mobile, 1, 1, 1, 0, 1, 1, 1, 1, 1);
                string address2_regex = validation.validate(obj.Beneficiary_Address1, 1, 1, 1, 1, 1, 1, 1, 1, 0);
                string PostCode_regex = validation.validate(obj.Beneficiary_PostCode, 1, 1, 0, 1, 1, 1, 1, 1, 1);

                string AccountHolderName_regex = validation.validate_accnm(obj.AccountHolderName, 0); //validation.validate(obj.AccountHolderName, 1, 1, 1, 1, 1, 1, 0, 1, 1);
                string Account_Number_regex = validation.validate(obj.Account_Number, 0, 1, 1, 1, 1, 1, 1, 1, 1);
                string BankCode_regex = validation.validate(obj.BankCode, 1, 1, 0, 1, 1, 1, 1, 1, 1);
                string Branch_regex = validation.validate(obj.Branch, 1, 1, 1, 1, 1, 1, 1, 1, 0);
                string Ifsc_Code_regex = validation.validate(obj.Ifsc_Code, 1, 1, 0, 1, 1, 1, 1, 1, 1);
                string BranchCode_regex = validation.validate(obj.BranchCode, 1, 1, 0, 1, 1, 1, 1, 1, 1);
                string Benf_Iban_regex = validation.validate(obj.Benf_Iban, 1, 1, 0, 1, 1, 1, 1, 1, 1);
                string Benf_BIC_regex = validation.validate(obj.Benf_BIC, 1, 1, 0, 1, 1, 1, 1, 1, 1);

                int address_regex_len = 100;
                string HouseNumber_len = "true"; string Street_len = "true"; string AddressLine1_len = "true"; string AddressLine2_len = "true";
                if (Benf_BIC_regex == "false")
                {
                    error_msg = error_msg + "BIC code should be alpanumeric without space.";
                    error_invalid_data = error_invalid_data + " BIC code: " + obj.Benf_BIC;
                }
                if (BankCode_regex == "false")
                {
                    error_msg = error_msg + "Bank code should be alpanumeric without space.";
                    error_invalid_data = error_invalid_data + " BankCode: " + obj.BankCode;
                }
                if (Branch_regex == "false")
                {
                    error_msg = error_msg + "Branch should be alphanumeric with space and special characters like , , , . , ' , ( , ) , { , } , - , @ , & ";
                    error_invalid_data = error_invalid_data + " Branch: " + obj.Branch;
                }
                if (Branch_regex == "false")
                {
                    error_msg = error_msg + "Branch code should be alphanumeric without space.";
                    error_invalid_data = error_invalid_data + " Branch: " + obj.BranchCode;
                }
                if (Ifsc_Code_regex == "false")
                {
                    error_msg = error_msg + "IFSC code should be alphanumeric without space.";
                    error_invalid_data = error_invalid_data + " Branch: " + obj.Ifsc_Code;
                }
                if (Benf_Iban_regex == "false")
                {
                    error_msg = error_msg + "IBAN code should be alphanumeric without space.";
                    error_invalid_data = error_invalid_data + " IBAN: " + obj.Benf_Iban;
                }
                if (Account_Number_regex == "false")
                {
                    error_msg = error_msg + "Account Number should be numeric only without space.";
                    error_invalid_data = error_invalid_data + " Account_Number: " + obj.Account_Number;
                }

                if (obj.Beneficiary_Address.Length >= address_regex_len)
                {
                    AddressLine1_len = "false";
                }
                if (obj.Beneficiary_Address1.Length >= address_regex_len)
                {
                    AddressLine2_len = "false";
                }
                if (FirstName_regex == "false" || AccountHolderName_regex == "false")
                {
                    error_msg = error_msg + "Name should include only charactes with space.";
                    if (FirstName_regex == "false" || AccountHolderName_regex == "false")
                    {
                        error_invalid_data = error_invalid_data + " Beneficiary_Name: " + obj.Beneficiary_Name;
                    }
                    if (AccountHolderName_regex == "false")
                    {
                        error_invalid_data = error_invalid_data + " Accountname: " + obj.AccountHolderName;
                    }

                }
                if (address1_regex == "false" || address2_regex == "false")
                {
                    string len_house = "";
                    if (AddressLine1_len == "false" || AddressLine2_len == "false")
                    {
                        len_house = len_house + "Address should be of valid length.";
                        error_invalid_data = error_invalid_data + "  Address length: " + obj.Beneficiary_Address.Length;
                    }
                    error_msg = error_msg + " Address should be alphanumeric with space and special characters like , , , . , ' , ( , ) , { , } , - , @ , & " + len_house;
                    error_invalid_data = error_invalid_data + " Address: " + obj.Beneficiary_Address;
                }
                if (AddressLine1_len == "false" || AddressLine2_len == "false")
                {
                    string len_house = "";
                    error_invalid_data = error_invalid_data + "  Address length: " + obj.Beneficiary_Address.Length;
                }
                if (TelephoneNumber_regex == "false" || MobileNumber_regex == "false")
                {
                    error_msg = error_msg + " Phone and Mobile number Should be numeric without space";
                    if (TelephoneNumber_regex == "false")
                    { error_invalid_data = error_invalid_data + "  MobileNumber: " + obj.Beneficiary_Mobile; }
                    if (MobileNumber_regex == "false")
                    { error_invalid_data = error_invalid_data + "  PhoneNumber: " + obj.Beneficiary_Mobile; }

                }
                if (PostCode_regex == "false")
                {
                    error_msg = error_msg + " Postcode Should be alphanumeric without space";
                    error_invalid_data = error_invalid_data + "  Postcode: " + obj.Beneficiary_PostCode;
                }
                if (Benf_BIC_regex == "" && Benf_Iban_regex == "" && BranchCode_regex == "" && Ifsc_Code_regex == "" && Branch_regex == "" && AccountHolderName_regex == "" && BankCode_regex == "" && Account_Number_regex == "" && Benf_Iban_regex == "" && PostCode_regex == "" && FirstName_regex == "" && address1_regex == "" && TelephoneNumber_regex == "" && MobileNumber_regex == "" && address2_regex == "" && PostCode_regex == "" && AddressLine1_len == "true" && AddressLine2_len == "true")
                {
                    FirstName_regex = "true"; BranchCode_regex = "true"; Ifsc_Code_regex = "true"; Branch_regex = "true"; AccountHolderName_regex = "true"; BankCode_regex = "true";
                    address1_regex = "true"; Account_Number_regex = "true";
                    TelephoneNumber_regex = "true";
                    MobileNumber_regex = "true";
                    address2_regex = "true";
                    PostCode_regex = "true";
                    Benf_Iban_regex = "true";
                    Benf_BIC_regex = "true";
                    BranchCode_regex = "true";

                }
                if (Benf_BIC_regex != "false" && Benf_Iban_regex != "false" && BranchCode_regex != "false" && Ifsc_Code_regex != "false" && Branch_regex != "false" && AccountHolderName_regex != "false" && BankCode_regex != "false" && Account_Number_regex != "false" && Benf_Iban_regex != "false" && PostCode_regex != "false" && FirstName_regex != "false" && address1_regex != "false" && TelephoneNumber_regex != "false" && MobileNumber_regex != "false" && address2_regex != "false" && PostCode_regex != "false" && AddressLine1_len != "false" && AddressLine2_len != "false")
                {
                    string Beneficiary_Name = CompanyInfo.testInjection(obj.Beneficiary_Name);
                    string Beneficiary_Address = CompanyInfo.testInjection(obj.Beneficiary_Address);
                    string Beneficiary_City_ID = CompanyInfo.testInjection(Convert.ToString(obj.Beneficiary_City_ID));
                    string Beneficiary_Country_ID = CompanyInfo.testInjection(Convert.ToString(obj.Beneficiary_Country_ID));
                    string Beneficiary_Telephone = CompanyInfo.testInjection(Convert.ToString(obj.Beneficiary_Telephone));
                    string Beneficiary_Mobile = CompanyInfo.testInjection(Convert.ToString(obj.Beneficiary_Mobile));
                    string Record_Insert_DateTime = CompanyInfo.testInjection(Convert.ToString(obj.Record_Insert_DateTime));
                    string Delete_Status = CompanyInfo.testInjection(Convert.ToString(obj.Delete_Status));
                    string Created_By_User_ID = CompanyInfo.testInjection(Convert.ToString(obj.Created_By_User_ID));

                    string Customer_ID1 = CompanyInfo.testInjection(Convert.ToString(Customer_ID));
                    string Beneficiary_Address1 = CompanyInfo.testInjection(Convert.ToString(obj.Beneficiary_Address1));
                    string Beneficiary_PostCode = CompanyInfo.testInjection(Convert.ToString(obj.Beneficiary_PostCode));
                    string cashcollection_flag = CompanyInfo.testInjection(Convert.ToString(obj.cashcollection_flag));
                    string Agent_MappingID = CompanyInfo.testInjection(Convert.ToString(obj.Agent_MappingID));
                    string Client_ID = CompanyInfo.testInjection(Convert.ToString(obj.Client_ID));
                    string Branch_ID = CompanyInfo.testInjection(Convert.ToString(obj.Branch_ID));
                    string Relation_ID = CompanyInfo.testInjection(Convert.ToString(obj.Relation_ID));


                    string Beneficiary_PostCode1 = CompanyInfo.testInjection(Convert.ToString(obj.Beneficiary_PostCode));

                    string AccountHolderName = CompanyInfo.testInjection(Convert.ToString(obj.AccountHolderName));
                    string Account_Number = CompanyInfo.testInjection(Convert.ToString(obj.Account_Number));
                    string BankCode = CompanyInfo.testInjection(Convert.ToString(obj.BankCode));
                    string Branch = CompanyInfo.testInjection(Convert.ToString(obj.Branch));
                    string Ifsc_Code = CompanyInfo.testInjection(Convert.ToString(obj.Ifsc_Code));
                    string BranchCode = CompanyInfo.testInjection(Convert.ToString(obj.BranchCode));
                    string Benf_Iban = CompanyInfo.testInjection(Convert.ToString(obj.Benf_Iban));
                    string Benf_BIC = CompanyInfo.testInjection(Convert.ToString(obj.Benf_BIC));
                    string sectionvalue = CompanyInfo.testInjection(Convert.ToString(obj.sectionvalue));


                    if (Branch == "1" && Ifsc_Code == "1" && BranchCode == "1" && Benf_Iban == "1" && Benf_BIC == "1" && BankCode == "1" && Account_Number == "1" && AccountHolderName == "1" && Beneficiary_PostCode1 == "1" && Beneficiary_Name == "1" && Beneficiary_Address == "1" && Beneficiary_City_ID == "1" && Beneficiary_Country_ID == "1" &&
                     Beneficiary_Telephone == "1" && Beneficiary_Mobile == "1" && Record_Insert_DateTime == "1" && Delete_Status == "1" &&
                     Created_By_User_ID == "1" && Customer_ID1 == "1" && Beneficiary_Address1 == "1" && Beneficiary_PostCode == "1" &&
                         cashcollection_flag == "1" && Agent_MappingID == "1" && Client_ID == "1" && Branch_ID == "1" && Relation_ID == "1"
                         )
                    {
                        MySqlConnector.MySqlTransaction transaction;
                        transaction = con.BeginTransaction(IsolationLevel.ReadCommitted);
                        //Anushka
                        //MySqlCommand cmd_update1 = new MySqlCommand("select * from beneficiary_configurations where Country_Id=@Country_Id and Collection_Type=@Collection_Type");
                        //cmd_update1.Parameters.AddWithValue("@Country_Id", obj.Beneficiary_Country_ID);
                        //cmd_update1.Parameters.AddWithValue("@Collection_Type", obj.Collection_type_Id);
                        MySqlConnector.MySqlCommand cmd_update1 = new MySqlConnector.MySqlCommand("SP_Beneficiary_Configuration");
                        cmd_update1.CommandType = CommandType.StoredProcedure;

                        cmd_update1.Connection = con;
                        cmd_update1.Parameters.AddWithValue("_Country_ID", obj.Beneficiary_Country_ID);
                        cmd_update1.Parameters.AddWithValue("_Collection_Type", obj.Collection_type_Id);
                        db_connection.ExecuteNonQueryProcedure(cmd_update1);
                        dtbenfconfig = db_connection.ExecuteQueryDataTableProcedure(cmd_update1);

                        MySqlConnector.MySqlCommand cmd = new MySqlConnector.MySqlCommand("SP_Update_Beneficiary");
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Transaction = transaction;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("_Config_name", dtbenfconfig.Rows[0]["Beneficiary_Name"]);
                        cmd.Parameters.AddWithValue("_Config_Address", dtbenfconfig.Rows[0]["Beneficiary_Address"]);
                        cmd.Parameters.AddWithValue("_Config_mobile", dtbenfconfig.Rows[0]["Beneficiary_Mobile"]);
                        cmd.Parameters.AddWithValue("_Config_city", dtbenfconfig.Rows[0]["Beneficiary_City"]);
                        cmd.Parameters.AddWithValue("_Config_country", dtbenfconfig.Rows[0]["Beneficiary_Country"]);
                        cmd.Parameters.AddWithValue("_Config_relation", dtbenfconfig.Rows[0]["Beneficiary_relation"]);
                        cmd.Parameters.AddWithValue("_Config_postcode", dtbenfconfig.Rows[0]["Beneficiary_postcode"]);
                        cmd.Parameters.AddWithValue("_Config_state", dtbenfconfig.Rows[0]["Beneficiary_state"]);
                        try { cmd.Parameters.AddWithValue("_Config_gender", dtbenfconfig.Rows[0]["Beneficiary_Gender"]); }
                        catch
                        {
                            cmd.Parameters.AddWithValue("_Config_gender", 2);
                        }


                        cmd.Parameters.AddWithValue("_Beneficiary_ID", obj.Beneficiary_ID);
                        cmd.Parameters.AddWithValue("_Beneficiary_Name", obj.Beneficiary_Name.Trim());
                        cmd.Parameters.AddWithValue("_Beneficiary_Address", obj.Beneficiary_Address.Trim());
                        cmd.Parameters.AddWithValue("_Beneficiary_City_ID", obj.Beneficiary_City_ID);
                        cmd.Parameters.AddWithValue("_Beneficiary_Country_ID", obj.Beneficiary_Country_ID);
                        cmd.Parameters.AddWithValue("_Beneficiary_Telephone", obj.Beneficiary_Telephone.Trim());
                        cmd.Parameters.AddWithValue("_Beneficiary_Mobile", obj.Beneficiary_Mobile.Trim());
                        cmd.Parameters.AddWithValue("_cashcollection_flag", obj.cashcollection_flag);
                        cmd.Parameters.AddWithValue("_Relation_ID", obj.Relation_ID);

                        cmd.Parameters.AddWithValue("_State", obj.Beneficiary_State);
                        cmd.Parameters.AddWithValue("_Beneficiary_PostCode", obj.Beneficiary_PostCode);

                        cmd.Parameters.AddWithValue("_Config_dob", dtbenfconfig.Rows[0]["DateOf_Birth"]);  // Digvijay 20-11-2024

                        if (obj.Birth_Date != "Invalid date" && obj.Birth_Date != "" && obj.Birth_Date != null && (obj.Birth_Date).ToLower() != "undefined")
                        {
                            DateTime DOB = DateTime.ParseExact(obj.Birth_Date, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                            string Birth_Date = DOB.ToString("yyyy-MM-dd HH:mm:ss");
                            cmd.Parameters.AddWithValue("_DOB", Convert.ToDateTime(Birth_Date));//cmd.Parameters.AddWithValue("_DOB", Birth_Date_DateTime);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("_DOB", null);
                        }
                        cmd.Parameters.AddWithValue("_Beneficiary_Gender", obj.Beneficiary_Gender);
                        cmd.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                        cmd.Parameters.AddWithValue("_Wallet_Id", obj.Wallet_Id);
                        cmd.Parameters.AddWithValue("_Mobile_provider", obj.Mobile_provider);
                        cmd.Parameters.AddWithValue("_Wallet_provider", obj.Wallet_provider);
                        int result = cmd.ExecuteNonQuery();
                        //object result = db_connection.ExecuteScalarProcedure(cmd);

                        //result = (result == DBNull.Value) ? null : result;
                        //obj.Beneficiary_ID = Convert.ToInt32(result);
                        cmd.Dispose();

                        if (obj.Beneficiary_ID != 0)
                        {
                            string[] check_alert = new string[7];
                            if (obj.sectionvalue == 1)
                            {
                                check_alert = AutoCheckSanctionList(obj, context);
                                int cnt_blacklisted = 0;
                                for (int i = 0; i < check_alert.Length; i++)
                                {
                                    if (check_alert[i] == "BlackListed")
                                    {
                                        cnt_blacklisted = cnt_blacklisted + 1;
                                    }
                                }
                                if (cnt_blacklisted == 0)
                                {
                                    if (obj.blacklisted == 1)
                                    {

                                        MySqlConnector.MySqlCommand cmd_sanction = new MySqlConnector.MySqlCommand("SP_Black_And_WhiteList_Benf");
                                        cmd_sanction.CommandType = CommandType.StoredProcedure;
                                        cmd_sanction.Transaction = transaction;
                                        cmd_sanction.Connection = con;

                                        cmd_sanction.Parameters.AddWithValue("_benf_id", obj.Beneficiary_ID);
                                        cmd_sanction.Parameters.AddWithValue("_isblackListed", 0);
                                        cmd_sanction.Parameters.AddWithValue("_client_id", obj.Client_ID);
                                        cmd_sanction.Parameters.AddWithValue("_flag", "BlackList");
                                        int result_sanction = cmd_sanction.ExecuteNonQuery();
                                    }
                                }
                            }

                            Model.Beneficiary _Objbenef = new Model.Beneficiary();
                            _Objbenef.Beneficiary_ID = obj.Beneficiary_ID;
                            _Objbenef.Delete_Status = 0;

                            int BBDetails_ID = 0;
                            if (Convert.ToInt32(obj.BBDetails_ID) != 0)
                            {
                                BBDetails_ID = obj.BBDetails_ID;
                            }


                            MySqlConnector.MySqlCommand cmd2 = new MySqlConnector.MySqlCommand("SP_Update_BeneficiaryBank_Details");

                            cmd2.CommandType = CommandType.StoredProcedure;
                            cmd2.Transaction = transaction;
                            cmd2.Connection = con;
                            // cmd.Parameters.AddWithValue("_Id", _Objbenef.Id);
                            //Anushka
                            cmd2.Parameters.AddWithValue("_config_accname", dtbenfconfig.Rows[0]["Beneficiary_Name"]);
                            cmd2.Parameters.AddWithValue("_config_accnum", dtbenfconfig.Rows[0]["ShowAccount_No"]);
                            cmd2.Parameters.AddWithValue("_config_ifsc", dtbenfconfig.Rows[0]["IFSC"]);
                            cmd2.Parameters.AddWithValue("_config_bnkname", dtbenfconfig.Rows[0]["Bank_Name"]);
                            cmd2.Parameters.AddWithValue("_config_bankcode", dtbenfconfig.Rows[0]["Bank_Code"]);
                            cmd2.Parameters.AddWithValue("_config_branchnm", dtbenfconfig.Rows[0]["Branch"]);
                            cmd2.Parameters.AddWithValue("_config_branchcode", dtbenfconfig.Rows[0]["Branch_Code"]);
                            cmd2.Parameters.AddWithValue("_config_iban", dtbenfconfig.Rows[0]["IBAN_Status"]);
                            cmd2.Parameters.AddWithValue("_config_bic", dtbenfconfig.Rows[0]["BIC_Status"]);





                            cmd2.Parameters.AddWithValue("_Beneficiary_ID", obj.Beneficiary_ID);
                            cmd2.Parameters.AddWithValue("_AccountHolderName", obj.AccountHolderName);
                            cmd2.Parameters.AddWithValue("_BBank_ID", obj.BBank_ID);
                            cmd2.Parameters.AddWithValue("_Account_Number", obj.Account_Number);
                            cmd2.Parameters.AddWithValue("_BankCode", obj.BankCode);
                            cmd2.Parameters.AddWithValue("_Bank_Name", obj.Bank_Name);
                            cmd2.Parameters.AddWithValue("_Ifsc_Code", obj.Ifsc_Code);
                            cmd2.Parameters.AddWithValue("_BranchCode", obj.BranchCode);
                            //  cmd2.Parameters.AddWithValue("_Client_ID",obj.Client_ID);
                            cmd2.Parameters.AddWithValue("_Branch", obj.Branch);
                            cmd2.Parameters.AddWithValue("_Benf_Iban", obj.Benf_Iban);
                            cmd2.Parameters.AddWithValue("_Benf_BIC", obj.Benf_BIC);
                            cmd2.Parameters.AddWithValue("_BBDetails_ID", BBDetails_ID);//parvej added
                            try { cmd2.Parameters.AddWithValue("_BankBranch_ID", obj.BankBranch_ID); } catch (Exception ex) { }

                            cmd2.Parameters.Add(new MySqlConnector.MySqlParameter("_existflag", MySqlConnector.MySqlDbType.Int32));
                            cmd2.Parameters["_existflag"].Direction = ParameterDirection.Output;
                            int result2 = cmd2.ExecuteNonQuery();
                            //object result2 = db_connection.ExecuteScalarProcedure(cmd2);
                            //result2 = (result2 == DBNull.Value) ? null : result2;
                            //int i = Convert.ToInt32(result2);
                            cmd2.Dispose();

                            if (result > 0 && result2 > 0)
                            {
                                obj.Message = "success";
                                #region check loaction
                                DataTable dtc = CompanyInfo.get(obj.Client_ID, context);
                                try
                                {

                                    string act = "";

                                    Boolean chkLocation = true;
                                    string country_log = "";
                                    string device_ty = "";

                                    try
                                    {
                                        DataTable chkLocation1 = CompanyInfo.check_location(obj.Client_ID, "1", context);
                                        try
                                        {
                                            chkLocation = Convert.ToBoolean(chkLocation1.Rows[0]["is_valid"]);
                                            country_log = Convert.ToString(chkLocation1.Rows[0]["Country"]);
                                            device_ty = Convert.ToString(chkLocation1.Rows[0]["device_ty"]);


                                        }
                                        catch
                                        {

                                        }

                                    }
                                    catch (Exception ex)
                                    {

                                    }
                                    if (!chkLocation)
                                    {
                                        string body = string.Empty, subject = string.Empty;
                                        string body1 = string.Empty;
                                        HttpWebRequest httpRequest = null, httpRequest1 = null;
                                        DataTable d2 = (DataTable)CompanyInfo.getCustomerDetails(obj.Client_ID, Customer_ID);
                                        string sendmsg1 = "Beneficiary Added From New Location ";
                                        string company_name = Convert.ToString(dtc.Rows[0]["Company_Name"]);
                                        string URL = Convert.ToString(dtc.Rows[0]["Company_URL_Admin"]);
                                        string cust_url = Convert.ToString(dtc.Rows[0]["Company_URL_Customer"]);
                                        httpRequest = (HttpWebRequest)WebRequest.Create(URL + "Email/new-login.html");
                                        httpRequest.UserAgent = "Code Sample Web Client";
                                        HttpWebResponse webResponse = (HttpWebResponse)httpRequest.GetResponse();
                                        using (StreamReader reader = new StreamReader(webResponse.GetResponseStream()))
                                        {
                                            body = reader.ReadToEnd();
                                        }
                                        body = body.Replace("[name]", Convert.ToString(d2.Rows[0]["First_Name"]));
                                        string enc_ref = CompanyInfo.Encrypt(Convert.ToString(d2.Rows[0]["WireTransfer_ReferanceNo"]), true);
                                        string link = cust_url + "/secure-account-verfiy?reference=" + enc_ref;
                                        body = body.Replace("[link]", link);
                                        body = body.Replace("[New_Login_Detected]", "Beneficiary Added From New Location ");
                                        body = body.Replace("[country]", country_log);
                                        body = body.Replace("[time]", (Convert.ToDateTime(CompanyInfo.gettime(obj.Client_ID, context))).ToString("dddd, dd MMMM yyyy HH:mm:ss"));
                                        body = body.Replace("[location_msg]", "We noticed a new beneficiary upload to your account from new location that we don't recognise. If this wasn't you, we'll help you secure your account.");
                                        body = body.Replace("[device]", device_ty);




                                        string EmailID = "";
                                        EmailID = Convert.ToString(d2.Rows[0]["Email_ID"]);


                                        subject = company_name + " - New Beneficiary Upload location - Alert " + d2.Rows[0]["WireTransfer_ReferanceNo"];
                                        string send_mail = (string)CompanyInfo.Send_Mail(dtc, EmailID, body, subject, obj.Client_ID, obj.Branch_ID, "", "", "", context);

                                        //Notification
                                        string notification_icon = "beneficiary.jpg";
                                        string notification_message = "<span class='cls-admin'>Beneficiary <strong class='cls-new-benf'>Added From New Location </strong></span>";
                                        CompanyInfo.save_notification_compliance(notification_message, notification_icon, Convert.ToString(Customer_ID), Convert.ToDateTime(obj.Record_Insert_DateTime), obj.Client_ID, 1, 0, obj.Branch_ID, 0, 1, 1, 0, context);
                                    }
                                }
                                catch
                                {

                                }
                                #endregion
                            }


                            try
                            {
                                #region beneficiary probable match
                                obj.whereclause = " Beneficiary_ID != " + obj.Beneficiary_ID + " and Beneficiary_Country_ID =" + obj.Beneficiary_Country_ID + " and Beneficiary_City_ID =" + obj.Beneficiary_City_ID + " and bm.Client_ID= " + obj.Client_ID;
                                if (obj.Birth_Date != "" && obj.Birth_Date != null && obj.Birth_Date != "null")
                                {
                                    obj.whereclause = obj.whereclause + " and (date(DateOf_Birth) =date(" + obj.Birth_Date + ") or 1=1 ) ";
                                }
                                if (obj.Beneficiary_Address != "" && obj.Beneficiary_Address != null && obj.Beneficiary_Address != "null")
                                {
                                    obj.whereclause = obj.whereclause + " AND   (Beneficiary_Address = '" + obj.Beneficiary_Address + "' or 1=1)";
                                }
                                MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("Beneficiary_probable_match");
                                _cmd.CommandType = CommandType.StoredProcedure;
                                _cmd.Parameters.AddWithValue("_whereclause", obj.whereclause);
                                _cmd.Parameters.AddWithValue("_first_name", obj.First_Name);
                                _cmd.Parameters.AddWithValue("_middle_name", obj.Middle_Name);
                                _cmd.Parameters.AddWithValue("_last_name", obj.Last_Name);
                                DataTable dsp = db_connection.ExecuteQueryDataTableProcedure(_cmd);
                                int n = dsp.Rows.Count;
                                if (n > 0)
                                {
                                    try
                                    {
                                        n = (int)(dsp.Rows[0]["Beneficiary_ID"]);
                                        MySqlConnector.MySqlCommand cmd_update = new MySqlConnector.MySqlCommand("update beneficiary_master set ProbableMatch_Flag=0,Matching_ID=@Matching_ID  where Beneficiary_ID=@Beneficiary_Id and Client_ID=@Client_ID");
                                        cmd_update.Parameters.AddWithValue("@Matching_ID", n);
                                        cmd_update.Parameters.AddWithValue("@Beneficiary_Id", obj.Beneficiary_ID);
                                        cmd_update.Parameters.AddWithValue("@Client_ID", obj.Client_ID);
                                        db_connection.ExecuteNonQueryProcedure(cmd_update);
                                        //string msg = "probable";
                                    }
                                    catch (Exception ex)
                                    {
                                        Activity = "";
                                        Activity = " Beneficiary probable match found ";
                                    }
                                    //string Activity = "";
                                    //Activity = " Beneficiary probable match found ";
                                    //mtsmethods.InsertActivityLogDetails_Beneficiary(Activity, user_id, 0, user_id, cust_id, "UpdateBenf_Details", c.Branch_ID, c.Client_ID, c.Beneficiary_ID);
                                    Activity = "Update Benficiary: Probable match found for beneficiary " + obj.First_Name + " " + obj.Middle_Name + " " + obj.Last_Name + ".";
                                    int stattus = (int)CompanyInfo.InsertActivityLogDetails(Activity, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(Customer_ID), "Update", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "Update Beneficiary", context);
                                }
                                #endregion
                            }
                            catch (Exception exb)
                            {
                                int stattuspro = (int)CompanyInfo.InsertActivityLogDetailsSecurity("beneficiary probable match error:" + exb.ToString(), Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), 1, "Update", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "Update Beneficiary", 0, context);
                            }
                            #region insert into Benef Collection Mapping table
                            if (obj.Beneficiary_ID > 0)
                            {
                                try
                                {
                                    MySqlCommand cmdaw = new MySqlCommand("Insert_benef_collectiontype_mapping");
                                    cmdaw.CommandType = CommandType.StoredProcedure;
                                    cmdaw.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                                    cmdaw.Parameters.AddWithValue("_Branch_ID", obj.Branch_ID);
                                    cmdaw.Parameters.AddWithValue("_User_ID", obj.Created_By_User_ID);

                                    cmdaw.Parameters.AddWithValue("_Customer_ID", Customer_ID);
                                    cmdaw.Parameters.AddWithValue("_Beneficiary_ID", obj.Beneficiary_ID);

                                    cmdaw.Parameters.AddWithValue("_Collection_type_Id", obj.Collection_type_Id);
                                    cmdaw.Parameters.AddWithValue("_Record_Insert_DateTime", obj.Record_Insert_DateTime);

                                    cmdaw.Parameters.Add(new MySqlParameter("_existflag", MySqlDbType.Int32));
                                    cmdaw.Parameters["_existflag"].Direction = ParameterDirection.Output;

                                    int p = db_connection.ExecuteNonQueryProcedure(cmdaw);

                                    var insertflag = Convert.ToInt32(cmdaw.Parameters["_existflag"].Value);

                                    string Activitys1 = "Already Exist Bnef Collection Mapping Customer Id : " + Customer_ID + ",Beneficiary_ID : " + obj.Beneficiary_ID + ",Collection Type Id : " + obj.Collection_type_Id + ", User ID :" + obj.Created_By_User_ID;
                                    if (insertflag > 0)
                                    {
                                        Activitys1 = "Successfully Insert Into Benef Collection Mapping Customer Id : " + Customer_ID + ",Beneficiary_ID : " + obj.Beneficiary_ID + ",Collection Type Id : " + obj.Collection_type_Id + ", User ID :" + obj.Created_By_User_ID;
                                    }
                                    int stattus = (int)CompanyInfo.InsertActivityLogDetails(Activitys1, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(Customer_ID), "App Update Beneficiary", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), " Update Beneficiary", context);
                                }
                                catch (Exception ex)
                                {
                                    string stattus = (string)CompanyInfo.InsertErrorLogDetails(ex.Message.Replace("\'", "\\'"), 0, "App Update Beneficiary", obj.Branch_ID, obj.Client_ID);
                                }
                            }
                            #endregion
                            //if (_Objbenef.Beneficiary_ID != 0)
                            //{
                            //    Model.ActivityLogTracker _LogTracker = new Model.ActivityLogTracker();
                            //    string _benefDetails = "Beneficiary Details:  Beneficiary Name:" + obj.Beneficiary_Name + " Mobile No: " + obj.Beneficiary_Mobile;
                            //    string _bankDetails = "Bank Details: Bank Name: " + _Objbenef.Bank_Name + " Account Holder: " + _Objbenef.AccountHolderName + "  Account Number: " + _Objbenef.Account_Number;
                            //    if (_Objbenef.Ifsc_Code != null && _Objbenef.Ifsc_Code != "")
                            //    {
                            //        _bankDetails = _bankDetails + " IFSC Code: " + _Objbenef.Ifsc_Code;
                            //    }
                            //    if (_Objbenef.Bank_Name != null && _Objbenef.Bank_Name != "")
                            //    {
                            //        _bankDetails = _bankDetails + " Branch: " + _Objbenef.Bank_Name;
                            //    }
                            //    if (_Objbenef.BankCode != null && _Objbenef.BankCode != "")
                            //    {
                            //        _bankDetails = _bankDetails + " Branch Code: " + _Objbenef.BankCode;
                            //    }

                            //    _LogTracker.Activity = " Beneficiary updated successfully " + obj.Bank_Name + ". </br>" + _benefDetails + "</br>" + _bankDetails + " .";
                            //    _LogTracker.Customer_ID = Customer_ID;
                            //    _LogTracker.FunctionName = "save beneficiary and bank details";


                            //    Service.srvActivityLogTracker srvLogTracker = new srvActivityLogTracker();
                            //    int i_result = srvLogTracker.Create(_LogTracker);

                            //}
                            transaction.Commit();
                            Activity = "Beneficiary Updated.  </br>";
                            int stattus1 = (int)CompanyInfo.InsertActivityLogDetails(Activity, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(Customer_ID), "Update", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "Update Beneficiary", context);

                        }
                    }
                    else
                    {
                        string msg = "SQl Enjection detected . security_code=0";
                        obj.Id = -1;
                        int stattus = (int)CompanyInfo.InsertActivityLogDetailsSecurity(msg, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), 1, "srvBenificiary", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "Update", 0, context);
                    }

                }
                else
                {
                    obj.Id = 1;
                    string msg = error_invalid_data;
                    obj.whereclause = "Validation Failed";
                    obj.Message = msg;
                    int stattus = (int)CompanyInfo.InsertActivityLogDetails(msg, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), 1, "srvBenificiary", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "Create", context);
                }
            }

            return obj;
            //    return obj.Beneficiary_ID;
        }

        public Model.Beneficiary AccHolderNameSearch(Model.Beneficiary obj, HttpContext context)
        {
            mtsIntegrationmethods mts = new mtsIntegrationmethods();
            DataTable ds = new DataTable();
            try
            {
                int Customer_ID = Convert.ToInt32(CompanyInfo.Decrypt(obj.Customer_ID, true));
                obj.Customer_ID = Customer_ID.ToString();
                MySqlConnector.MySqlCommand cmd = new MySqlConnector.MySqlCommand("Get_APIDetails");
                cmd.CommandType = CommandType.StoredProcedure;
                string whereclause = " and a.Client_ID=" + obj.Client_ID + "";
                cmd.Parameters.AddWithValue("_security_key", CompanyInfo.SecurityKey());
                cmd.Parameters.AddWithValue("_whereclause", whereclause);
                DataTable dt = db_connection.ExecuteQueryDataTableProcedure(cmd);
                CompanyInfo.InsertActivityLogDetails(" where clause" + whereclause + " cnt: " + dt.Rows.Count, 0, 0, 0, 0, "Search", 0, 0, "Generate TokenGenration Kmoney response error", context);


                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    int activeforbeneficiaryvalidation = 1;
                    int api_id = Convert.ToInt32(dt.Rows[i]["ID"]);
                    string api_fields = Convert.ToString(dt.Rows[i]["api_Fields"]);
                    if (api_fields != "" && api_fields != null)
                    {
                        Newtonsoft.Json.Linq.JObject obj_newton = Newtonsoft.Json.Linq.JObject.Parse(api_fields);
                        activeforbeneficiaryvalidation = obj_newton["activeforbeneficiaryvalidation"]?.Value<int>() ?? activeforbeneficiaryvalidation;
                    }
                    string bankCode = Convert.ToString(obj.BBank_ID);
                    string Beneficiary_Country_ID = Convert.ToString(obj.Beneficiary_Country_ID);
                    string apiurl = Convert.ToString(dt.Rows[i]["API_URL"]);
                    string apiuser = Convert.ToString(dt.Rows[i]["APIUser_ID"]);
                    string apipass = Convert.ToString(dt.Rows[i]["Password"]);
                    string accesscode = Convert.ToString(dt.Rows[i]["APIAccess_Code"]);//unique code
                    string apicompany_id = Convert.ToString(dt.Rows[i]["APICompany_ID"]);
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    cmd = new MySqlCommand("SearchHolderName");
                    cmd.CommandType = CommandType.StoredProcedure;
                    string wherecondition = "   bank_id=" + bankCode + " and api_id = " + api_id;
                    cmd.Parameters.AddWithValue("_security_key", CompanyInfo.SecurityKey());
                    cmd.Parameters.AddWithValue("wherecondition", wherecondition);
                    DataTable dtofKMOBankCode = db_connection.ExecuteQueryDataTableProcedure(cmd);


                    if (api_id == 27 && activeforbeneficiaryvalidation == 0)
                    {
                        try
                        {
                            bankCode = Convert.ToString(dtofKMOBankCode.Rows[0]["bank_code"]);
                            string beneficiary_bank_id = "0";
                           
                            var client = new RestClient(apiurl + "getbanks");
                            client.Timeout = -1;
                            var request = new RestRequest(Method.GET);
                            IRestResponse response = client.Execute(request);
                            //CompanyInfo.InsertActivityLogDetails("getbanks Transfer rocket response parameters: <br/>" + response.Content, t.User_ID, 0, t.User_ID, t.Customer_ID, "API request id ", t.CB_ID, t.Client_ID);
                            dynamic json = Newtonsoft.Json.JsonConvert.DeserializeObject(response.Content);
                            dynamic result = null;
                            if (json.content != "")
                                result = json["data"];
                            foreach (var item in result)
                            {


                                if (bankCode == item.bankCode.Value.ToString())

                                {

                                    beneficiary_bank_id = item.bankId.Value.ToString();

                                }
                            }
                            client = new RestClient(apiurl + "validatebeneficiarybankdetails");
                            //var request = new RestRequest(Method.POST);
                            request = new RestRequest(Method.POST);
                            //request.Method = Method.POST;
                            //request.AddHeader("Authorization", "Bearer " + accesscode);
                            request.AddHeader("Content-Type", "application/json");

                            var body = @"{
" + "\n" +
@"    ""beneficiaryBank"": {
" + "\n" +
@"        ""accountNumber"": """ + obj.Account_Number + @""",
" + "\n" +
@"        ""bankId"": " + beneficiary_bank_id + @"
" + "\n" +
@"    }
" + "\n" +
@"}";

                            //request.AddStringBody(body, DataFormat.Json);
                            //RestResponse response = await client.ExecuteAsync(request);
                            request.AddParameter("application/json", body, ParameterType.RequestBody);
                            //CompanyInfo.InsertActivityLogDetails("Muleshil bank validate api request: " + body, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(obj.Customer_ID), "getAccountHolderDetails", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "Get Beneficiary Bank Account Details");
                            CompanyInfo.InsertActivityLogDetails("Muleshil bank validate api request: " + body, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(obj.Customer_ID), "getAccountHolderDetails", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "Get Beneficiary Bank Account Details", context);
                            response = client.Execute(request);
                            CompanyInfo.InsertActivityLogDetails("Muleshil bank validate api request: " + response.Content, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(obj.Customer_ID), "getAccountHolderDetails", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "Get Beneficiary Bank Account Details", context);
                            dynamic Json = null;
                            Json = Newtonsoft.Json.JsonConvert.DeserializeObject(response.Content);
                            try
                            {
                                obj.AccountHolderName = Json.data.beneficiaryBank.accountName;
                            }
                            catch (Exception ex)
                            {
                                obj.AccountHolderName = "";
                            }

                        }
                        catch (Exception ex)
                        {
                            CompanyInfo.InsertActivityLogDetails("Muleshil api error." + ex.ToString(), Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(obj.Customer_ID), "getAccountHolderDetails", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "Get Beneficiary Bank Account Details", context);
                        }
                    }
                    if (api_id == 28 && activeforbeneficiaryvalidation == 0)
                    {
                        try
                        {
                            bankCode = Convert.ToString(dtofKMOBankCode.Rows[0]["bank_code"]);
                            var client = new RestClient(apiurl + "accounts/resolve");
                            var request = new RestRequest(Method.POST);
                            request.AddHeader("Authorization", accesscode);
                            request.AddHeader("Content-Type", "application/json");
                            var body = @"{
" + "\n" +
                            @"    ""account_number"": """ + obj.Account_Number + @""",
" + "\n" +
                            @"    ""account_bank"": """ + bankCode + @"""
" + "\n" +
                            @"}";
                            request.AddParameter("application/json", body, ParameterType.RequestBody);
                            CompanyInfo.InsertActivityLogDetails("Flutterwave bank validate api request: " + body, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(obj.Customer_ID), "getAccountHolderDetails", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "Get Beneficiary Bank Account Details", context);
                            var response = client.Execute(request);
                            CompanyInfo.InsertActivityLogDetails("Flutterwave bank validate api response: " + response.Content, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(obj.Customer_ID), "getAccountHolderDetails", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "Get Beneficiary Bank Account Details", context);
                            dynamic Json = null;
                            Json = Newtonsoft.Json.JsonConvert.DeserializeObject(response.Content);
                            try
                            {
                                obj.AccountHolderName = Json.data.account_name;
                            }
                            catch (Exception ex) { }

                        }
                        catch (Exception ex)
                        {
                            CompanyInfo.InsertActivityLogDetails("Flutterwave api error." + ex.ToString(), Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(obj.Customer_ID), "getAccountHolderDetails", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "Get Beneficiary Bank Account Details", context);
                        }
                    }
                    else if (api_id == 14 && activeforbeneficiaryvalidation == 0)
                    {

                        string accHolderName = "";
                        try
                        {
                            if (api_id == 14)
                            {
                                string token = mtsIntegrationmethods.TokenGenration(dt, context);
                                Model.Beneficiary _ObjCustomer = new Model.Beneficiary();

                                _ObjCustomer = Holdername(obj, context);

                                try
                                {
                                    string accNumber = Convert.ToString(obj.Account_Number).Trim();
                                    //bankCode = Convert.ToString(dtofKMOBankCode.Rows[0]["bank_code"]);
                                    string gettoken = "", getclientID = "";
                                    api_fields = Convert.ToString(dt.Rows[i]["api_Fields"]);
                                    if (api_fields != "" && api_fields != null)
                                    {
                                        Newtonsoft.Json.Linq.JObject objAPI = Newtonsoft.Json.Linq.JObject.Parse(api_fields);
                                        gettoken = Convert.ToString(objAPI["gettoken"]);
                                        getclientID = Convert.ToString(objAPI["clientid"]);
                                    }

                                    if (accNumber != null || accNumber == "")
                                    {
                                        string authbaseurl = gettoken;
                                        string awsClientId = Convert.ToString(dt.Rows[i]["APIUser_ID"]).Trim();
                                        string awsSecret = Convert.ToString(dt.Rows[i]["APIAccess_Code"]).Trim();
                                        var plainTextBytes = Encoding.UTF8.GetBytes($"{awsClientId}:{awsSecret}");
                                        string authorizationheader = Convert.ToBase64String(plainTextBytes);
                                        var client = new RestClient(authbaseurl);
                                        var request = new RestRequest(Method.POST);
                                        request.AddHeader("Content-type", "application/x-www-form-urlencoded");
                                        request.AddHeader("Authorization", $"Basic {authorizationheader}");
                                        //  request.AddParameter("scope", "nairapayoutmerchant-resource-server/merchant");
                                        request.Resource = "/oauth2/token";
                                        request.AddParameter("client_id", awsClientId, ParameterType.GetOrPost);
                                        request.AddParameter("grant_type", "client_credentials", ParameterType.GetOrPost);

                                        var response = client.Execute(request);
                                        if (response.StatusCode == System.Net.HttpStatusCode.OK || response.StatusCode == System.Net.HttpStatusCode.BadRequest || response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                                        {
                                            CompanyInfo.InsertActivityLogDetails("Payceler Token Generated.", Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(obj.Customer_ID), "getAccountHolderDetails", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "Get Beneficiary Bank Account Details", context);
                                            Console.WriteLine(response.Content);
                                            kmoney json = Newtonsoft.Json.JsonConvert.DeserializeObject<kmoney>(response.Content);
                                            token = json.access_token;
                                        }

                                        dynamic Json = null;
                                        response = null;
                                        try
                                        {
                                            client = new RestClient(Convert.ToString(dt.Rows[i]["API_URL"]) + "/client/getAPIClientGatewayBalanceList/" + Convert.ToString(getclientID) + "");
                                            client.Timeout = -1;
                                            request = new RestRequest(Method.GET);
                                            request.AddHeader("Authorization", token);
                                            response = client.Execute(request);
                                            Json = Newtonsoft.Json.JsonConvert.DeserializeObject(response.Content);
                                            CompanyInfo.InsertActivityLogDetails("Payceler Balance and Gateway response: <br>:" + response.Content, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(obj.Customer_ID), "getAccountHolderDetails", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "Get Beneficiary Bank Account Details", context);
                                        }
                                        catch (Exception ex)
                                        {
                                            CompanyInfo.InsertActivityLogDetails("Payceler Balance and Gateway error : <br>:" + ex.ToString().Replace("'", "\""), Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(obj.Customer_ID), "getAccountHolderDetails", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "Get Beneficiary Bank Account Details", context);
                                        }


                                        foreach (var gtwayNumber in Json)
                                        {
                                            string gatewayId = Convert.ToString(gtwayNumber.gatewayId);
                                            string acccountValid = "F"; string bnkId = "";
                                            try
                                            {
                                                cmd = new MySqlCommand("SearchHolderName");
                                                cmd.CommandType = CommandType.StoredProcedure;
                                                wherecondition = "   bank_id=" + bankCode + " and api_id = " + api_id + " and bank_name like '%" + gatewayId + "%' ";
                                                cmd.Parameters.AddWithValue("_security_key", CompanyInfo.SecurityKey());
                                                cmd.Parameters.AddWithValue("wherecondition", wherecondition);
                                                DataTable dtofKMOBankCodePayceller = db_connection.ExecuteQueryDataTableProcedure(cmd);
                                                try
                                                {
                                                    bnkId = Convert.ToString(dtofKMOBankCodePayceller.Rows[0]["bank_code"]);
                                                }
                                                catch (Exception ex)
                                                {
                                                }

                                                client = new RestClient(Convert.ToString(dt.Rows[i]["API_URL"]));
                                                request = new RestRequest(Method.GET);
                                                request.AddHeader("Content-type", "application/json");
                                                request.AddHeader("Authorization", token);
                                                request.Resource = $"/client/nameEnquiry/" + gatewayId + "/" + Convert.ToString(bnkId).Trim() + "/" + Convert.ToString(accNumber).Trim();
                                                string urlR = "/client/nameEnquiry/" + gatewayId + "/" + Convert.ToString(bnkId).Trim() + "/" + Convert.ToString(accNumber).Trim();
                                                response = client.Execute(request);
                                                CompanyInfo.InsertActivityLogDetails("Payceler Validate Request : <br/>" + urlR, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(obj.Customer_ID), "getAccountHolderDetails", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "validateAccountNumber", context);
                                                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                                                {
                                                    Console.WriteLine("Success Is Ok");
                                                    accHolderName = Convert.ToString(response.Content);
                                                    accHolderName = accHolderName.Trim().Replace(@"\", "").Replace(@"""", "");
                                                    acccountValid = "T"; break;
                                                }
                                                else
                                                {
                                                    Console.WriteLine("Bad Request " + response.StatusDescription);
                                                    acccountValid = "F";
                                                }
                                                Console.WriteLine(response.Content);
                                                CompanyInfo.InsertActivityLogDetails("App Payceler Validate Account response: <br/>" + response.Content, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(obj.Customer_ID), "getAccountHolderDetails", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "validateAccountNumber", context);
                                            }
                                            catch (Exception ex)
                                            {
                                                acccountValid = "F";
                                                CompanyInfo.InsertActivityLogDetails("App Payceler Validate Account error: <br/>" + ex.ToString().Replace("'", "\""), Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(obj.Customer_ID), "getAccountHolderDetails", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "validateAccountNumber", context);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        Console.Write("alert(Account Number or Account Holder Name is Empty)");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    CompanyInfo.InsertActivityLogDetails("Error Get account details response <br>:" + api_id + " " + ex.ToString(), Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(obj.Customer_ID), "getAccountHolderDetails", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "Get Beneficiary Bank Account Details", context);
                                }
                                obj.AccountHolderName = accHolderName;
                            }
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                    else if (api_id == 15 && activeforbeneficiaryvalidation == 0)
                    {
                        string accHolderName = "", clientkey = "", secretkey = "", token = "";
                        try
                        {
                            if (api_fields != "" && api_fields != null)
                            {

                                Newtonsoft.Json.Linq.JObject obj1 = Newtonsoft.Json.Linq.JObject.Parse(api_fields);
                                clientkey = Convert.ToString(obj1["clientkey"]);
                                secretkey = Convert.ToString(obj1["secretkey"]);

                            }


                            string cred = Convert.ToBase64String(Encoding.Default.GetBytes(apiuser + ":" + apipass));
                            try
                            {
                                var client = new RestClient(apiurl + "/Auth/Token");
                                client.Timeout = -1;
                                var request = new RestRequest(Method.GET);
                                request.AddHeader("username", apiuser);
                                request.AddHeader("secretkey", secretkey);
                                request.AddHeader("Authorization", "Basic " + cred);
                                ServicePointManager.Expect100Continue = true;
                                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                                System.Net.ServicePointManager.ServerCertificateValidationCallback += delegate { return true; };
                                IRestResponse response = client.Execute(request);
                                Console.WriteLine(response.Content);
                                amal json = Newtonsoft.Json.JsonConvert.DeserializeObject<amal>(response.Content);
                                token = json.response.result.token;

                            }
                            catch (Exception ex)
                            {
                                CompanyInfo.InsertActivityLogDetails("Generate TokenGenration Amal response Error :<br>:" + ex.ToString(), 0, 0, 0, 0, "Search", 0, 0, "Generate TokenGenration Amal response error", context);
                            }
                            try
                            {
                                Newtonsoft.Json.Linq.JObject json = new Newtonsoft.Json.Linq.JObject();
                                var client = new RestClient(apiurl + "/Services/SearchCustomerByMobile");
                                client.Timeout = -1;
                                var request = new RestRequest(Method.POST);
                                request.AddHeader("Authorization", "Bearer " + token);
                                request.AddHeader("username", apiuser);
                                request.AddHeader("secretkey", secretkey);
                                request.AddHeader("Content-Type", "application/json");
                                var body = @"{
                                " + "\n" +
                                 @"	""clientkey"": """ + clientkey + @""",
                                " + "\n" +
                                 @"	  ""mobile"": """ + obj.Beneficiary_Mobile + @""",
                                " + "\n" +
                                 @"	""requestId"": ""0""
                                " + "\n" +
                                 @"}";
                                request.AddParameter("application/json", body, ParameterType.RequestBody);
                                ServicePointManager.Expect100Continue = true;
                                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                                System.Net.ServicePointManager.ServerCertificateValidationCallback += delegate { return true; };
                                var response = client.Execute(request);
                                json = Newtonsoft.Json.Linq.JObject.Parse(response.Content);

                                accHolderName = json["response"]?["result"]?["Customer"]?["CustomerDetails"]?["FullName"]?.ToString();
                            }
                            catch (Exception ex)
                            {
                                CompanyInfo.InsertActivityLogDetails("SearchCustomerByMobile Amal response Error :<br>:" + ex.ToString(), 0, 0, 0, 0, "Search", 0, 0, "SearchCustomerByMobile Amal response error", context);
                            }
                        }
                        catch (Exception ex)
                        {
                            CompanyInfo.InsertActivityLogDetails("SearchCustomerByMobile Amal Error :<br>:" + ex.ToString(), 0, 0, 0, 0, "Search", 0, 0, "SearchCustomerByMobile Amal error", context);
                        }
                        obj.AccountHolderName = accHolderName;
                    }
                    else if (api_id == 20 && activeforbeneficiaryvalidation == 0)
                    {
                        try
                        {
                            string token = mtsIntegrationmethods.TokenGenration(dt, context);
                            bankCode = Convert.ToString(dtofKMOBankCode.Rows[0]["bank_code"]);
                            var client = new RestClient(apiurl + "account/lookup");
                            client.Timeout = -1;
                            var request = new RestRequest(Method.POST);
                            request.AddHeader("Accept", "application/json");
                            request.AddHeader("Authorization", "Bearer " + token);
                            request.AddHeader("Content-Type", "application/json");
                            var body = @"{
                        " + "\n" +
                            @"    ""bank"":""" + bankCode + @""",
                        " + "\n" +
                            @"    ""account"": """ + obj.Account_Number + @"""
                        " + "\n" +
                            @"}";
                            request.AddParameter("application/json", body, ParameterType.RequestBody);
                            //string activity = " Get Ac Details URL: " + Convert.ToString(dtt.Rows[i]["API_URL"]) + "/NameEnquiry/" + Convert.ToString(bankCode) + "/" + obj.Account_Number;
                            CompanyInfo.InsertActivityLogDetails("Get TierMoney account details request <br>:" + body, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(obj.Customer_ID), "getAccountHolderDetails TierMoney", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "Get Beneficiary Bank Account Details", context);
                            IRestResponse response = client.Execute(request);
                            CompanyInfo.InsertActivityLogDetails("Get TierMoney account details response <br>:" + response.Content, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(obj.Customer_ID), "getAccountHolderDetails TierMoney", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "Get Beneficiary Bank Account Details", context);
                            kmoney json = Newtonsoft.Json.JsonConvert.DeserializeObject<kmoney>(response.Content);
                            obj.AccountHolderName = json.data.accountName; ;
                        }
                        catch (Exception ex)
                        {
                            CompanyInfo.InsertActivityLogDetails("Generate TokenGenration Kmoney response Error :<br>:" + ex.ToString(), 0, 0, 0, 0, "Search", 0, 0, "Generate TokenGenration Kmoney response error", context);
                        }

                    }
                    else if (api_id == 23 && activeforbeneficiaryvalidation == 0)
                    {
                        try
                        {
                            string token = mtsIntegrationmethods.TokenGenration(dt, context);
                            try
                            {
                                bankCode = Convert.ToString(dtofKMOBankCode.Rows[0]["bank_code"]);
                                var client = new RestClient(apiurl + "getBeneficiaryAccountName/100102/NGR/" + bankCode + "/" + obj.Account_Number);
                                client.Timeout = -1;
                                var request = new RestRequest(Method.GET);
                                // CompanyInfo.InsertActivityLogDetails("Get KoboTrade account details request <br>:" + body, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(obj.Customer_ID), "getAccountHolderDetails Kobo", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "Get Beneficiary Bank Account Details");
                                IRestResponse response = client.Execute(request);
                                //CompanyInfo.InsertActivityLogDetails("Get KoboTrade account details response <br>:" + response.Content, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(obj.Customer_ID), "getAccountHolderDetails KObo", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "Get Beneficiary Bank Account Details");
                                dynamic json = Newtonsoft.Json.JsonConvert.DeserializeObject(response.Content);
                                obj.AccountHolderName = json.data;
                            }
                            catch (Exception ex)
                            {
                                CompanyInfo.InsertActivityLogDetails("Generate TokenGenration Kobo response Error :<br>:" + ex.ToString(), 0, 0, 0, 0, "Search", 0, 0, "Generate TokenGenration Kobo response error", context);
                            }
                        }
                        catch (Exception ex)
                        {
                            CompanyInfo.InsertActivityLogDetails("Generate TokenGenration Kobo response Error :<br>:" + ex.ToString(), 0, 0, 0, 0, "Search", 0, 0, "Generate TokenGenration Kobo response error", context);
                        }
                    }
                    else if (api_id == 26 && activeforbeneficiaryvalidation == 0)
                    {
                        string token = "";
                        try
                        {
                            string provider_id = "";
                            var client = new RestClient(apiurl + "login");
                            client.Timeout = -1;
                            var request = new RestRequest(Method.POST);
                            request.AddHeader("Content-Type", "application/json");
                            var body = @"{
" + "\n" +
                            @"    ""userId"": """ + apiuser + @""",
" + "\n" +
                            @"    ""password"": """ + apipass + @""",
" + "\n" +
                            @"    ""partnerID"": """ + accesscode + @"""
" + "\n" +
                            @"}";
                            request.AddParameter("application/json", body, ParameterType.RequestBody);
                            CompanyInfo.InsertActivityLogDetails("Bakaal Login Request: <br/>" + body + "", 0, 0, 0, 0, "Bakaal login ", 0, 1, "Bakaal login", context);
                            IRestResponse response = client.Execute(request);
                            CompanyInfo.InsertActivityLogDetails("Bakaal login Response: <br/>" + response.Content + "", 0, 0, 0, 0, "Bakaal login", 0, 1, "Bakaal login", context);
                            dynamic json = Newtonsoft.Json.JsonConvert.DeserializeObject(response.Content);
                            token = json.token;




                            cmd = new MySqlCommand("api_providerid_against_country_collectiontype");
                            cmd.CommandType = CommandType.StoredProcedure;

                            cmd.Parameters.AddWithValue("_Country_ID", Beneficiary_Country_ID);
                            cmd.Parameters.AddWithValue("_Collection_Type", 3);
                            cmd.Parameters.AddWithValue("_API_ID", "26");

                            DataTable dttt = db_connection.ExecuteQueryDataTableProcedure(cmd);


                            foreach (DataRow row in dttt.Rows)
                            {

                                int mobile_provider = Convert.ToInt32(row["Mobile_provider"]);

                                if (obj.Mobile_provider == mobile_provider)
                                {
                                    provider_id = Convert.ToString(row["ProviderPayerID"]);
                                }
                            }

                            client = new RestClient(apiurl + "Validate");
                            client.Timeout = -1;
                            request = new RestRequest(Method.POST);
                            request.AddHeader("Content-Type", "application/json");
                            request.AddHeader("Authorization", token);
                            body = @"{
" + "\n" +
                            @"    ""serviceId"": """ + provider_id + @""",
" + "\n" +
                            @"    ""rmobileNo"": """ + obj.Beneficiary_Mobile + @""",
" + "\n" +
                            @"    ""userId"": """ + apiuser + @"""
" + "\n" +
                            @"}";
                            request.AddParameter("application/json", body, ParameterType.RequestBody);
                            CompanyInfo.InsertActivityLogDetails("Bakaal Login Request: <br/>" + body + "", 0, 0, 0, 0, "Bakaal login ", 0, 1, "Bakaal login", context);
                            response = client.Execute(request);
                            CompanyInfo.InsertActivityLogDetails("Bakaal login Response: <br/>" + response.Content + "", 0, 0, 0, 0, "Bakaal login", 0, 1, "Bakaal login", context);
                            json = Newtonsoft.Json.JsonConvert.DeserializeObject(response.Content);
                            obj.AccountHolderName = json.rName;
                        }
                        catch (Exception ex)
                        {
                            CompanyInfo.InsertActivityLogDetails("Get AccountHolder Name Dynathopia response Error :<br>:" + ex.ToString(), 0, 0, 0, 0, "Search", 0, 0, "Get AccountHolder Name Dynathopia response error", context);
                        }
                    }
                    else if (api_id == 31 && activeforbeneficiaryvalidation == 0)
                    {
                        try
                        {
                            string token = mtsIntegrationmethods.TokenGenration(dt, context);
                            string accountName = "";
                            try
                            {

                                try
                                {
                                    string requestLink = "";
                                    api_fields = Convert.ToString(dt.Rows[i]["api_Fields"]);
                                    bankCode = Convert.ToString(dtofKMOBankCode.Rows[0]["bank_code"]);
                                    if (api_fields != "" && api_fields != null)
                                    {
                                        Newtonsoft.Json.Linq.JObject objAPI = Newtonsoft.Json.Linq.JObject.Parse(api_fields);
                                        requestLink = Convert.ToString(objAPI["requestlink"]);
                                    }
                                    string AcNo = obj.Account_Number;
                                    var client = new RestClient(apiurl);
                                    var request = new RestRequest(requestLink);
                                    request.Method = Method.POST;
                                    request.AddHeader("Content-Type", "application/xml");
                                    var body = @"<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:prov=""http://providus.com/""> " + "\n" +
                                        @"   <soapenv:Header/> " + "\n" +
                                        @"   <soapenv:Body> " + "\n" +
                                        @"     <prov:GetNIPAccount>" + "\n" +
                                        @"	<account_number>" + AcNo + "</account_number>" + "\n" +
                                        @"	<bank_code>" + bankCode + "</bank_code>" + "\n" +
                                        @"	<username>" + apiuser + "</username>" + "\n" +
                                        @"	<password>" + apipass + "</password>" + "\n" +
                                        @"	</prov:GetNIPAccount>" + "\n" +
                                        @"   </soapenv:Body> " + "\n" +
                                        @"</soapenv:Envelope>";
                                    request.AddParameter("text/xml; charset=utf-8", body, ParameterType.RequestBody);
                                    CompanyInfo.InsertActivityLogDetails(" Providus request parameters GetNIPAccount: <br/>" + body + "", 0, 0, 0, 0, " Get Providus Transaction Status", 0, 0, " Get Providus Transaction Status", context);
                                    IRestResponse response1 = client.Execute(request);
                                    CompanyInfo.InsertActivityLogDetails(" Providus response parameters GetNIPAccount: <br/>" + response1.Content + "", 0, 0, 0, 0, " Get Providus Transaction Status", 0, 0, " Get Providus Transaction Status", context);
                                    XmlDocument doc = new XmlDocument();

                                    doc.LoadXml(response1.Content);

                                    XmlNodeList nodeList = doc.GetElementsByTagName("return");
                                    string responseCode = "", responseMessage = "";
                                    foreach (XmlNode node in nodeList)
                                    {
                                        string jsonContent = node.InnerText;

                                        try
                                        {
                                            JObject jsonObject = JObject.Parse(jsonContent);

                                            responseCode = (string)jsonObject["responseCode"];
                                            responseMessage = (string)jsonObject["responseMessage"];
                                            accountName = (string)jsonObject["accountName"];
                                        }
                                        catch (JsonException ex)
                                        {
                                            Console.WriteLine("Error parsing JSON: " + ex.Message);
                                        }
                                    }

                                }
                                catch (Exception ex)
                                {
                                    CompanyInfo.InsertActivityLogDetails("Generate TokenGenration Providus response Error :<br>:" + ex.ToString(), 0, 0, 0, 0, "Search", 0, 0, "Generate TokenGenration Providus response error", context);
                                }
                                if (accountName != "NA")
                                    obj.AccountHolderName = accountName;
                                else
                                    obj.AccountHolderName = "";


                            }
                            catch (Exception ex)
                            {
                                CompanyInfo.InsertActivityLogDetails("Generate TokenGenration Kobo response Error :<br>:" + ex.ToString(), 0, 0, 0, 0, "Search", 0, 0, "Generate TokenGenration Kobo response error", context);
                            }
                        }
                        catch (Exception ex)
                        {
                            CompanyInfo.InsertActivityLogDetails("Generate TokenGenration Kobo response Error :<br>:" + ex.ToString(), 0, 0, 0, 0, "Search", 0, 0, "Generate TokenGenration Kobo response error", context);
                        }
                    }
                    else if (api_id == 36 && activeforbeneficiaryvalidation == 0)
                    {
                        try
                        {
                            string token = "";
                            string username = "";
                            string password = "";
                            if (api_id == 36 && api_fields != "" && api_fields != null)
                            {
                                Newtonsoft.Json.Linq.JObject objj = Newtonsoft.Json.Linq.JObject.Parse(api_fields);
                                username = Convert.ToString(objj["username"]);
                                password = Convert.ToString(objj["password"]);

                            }
                            string authToken = "";
                            int PaymentDepositType_ID = 0;
                            string targetchannel = "";
                            string Beneficiary_Country = "";
                            string country_no = "";
                            string channelNo = "";
                            string beneficiary_bank_code = "";
                            string Provider_name = "";
                            string destinationNo = "";
                            string Account_Number = "";
                            string Beneficiary_Mobile = "";
                            bool status = false;
                            string accountName = "";


                            MySqlCommand cmd_cou_ser = new MySqlCommand("Country_Search");
                            cmd_cou_ser.CommandType = CommandType.StoredProcedure;
                            whereclause = "and Country_ID=" + obj.Beneficiary_Country_ID;
                            cmd_cou_ser.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                            cmd_cou_ser.Parameters.AddWithValue("_whereclause", whereclause);
                            DataTable dt_cou_ser = db_connection.ExecuteQueryDataTableProcedure(cmd_cou_ser);
                            if (dt_cou_ser.Rows.Count > 0)
                            {
                                Beneficiary_Country = Convert.ToString(dt_cou_ser.Rows[0]["Country_Name"]);
                            }
                            MySqlCommand cmd_prov_ser = new MySqlCommand("Get_Providerbyid");
                            cmd_prov_ser.CommandType = CommandType.StoredProcedure;
                            cmd_prov_ser.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                            cmd_prov_ser.Parameters.AddWithValue("_Provider_Id", obj.Mobile_provider);
                            cmd_prov_ser.Parameters.AddWithValue("_Country_ID", obj.Beneficiary_Country_ID);
                            DataTable dt_prov_ser = db_connection.ExecuteQueryDataTableProcedure(cmd_prov_ser);
                            if (dt_prov_ser.Rows.Count > 0)
                            {
                                Provider_name = Convert.ToString(dt_prov_ser.Rows[0]["Provider_name"]);
                            }


                            PaymentDepositType_ID = obj.Collection_type_Id;
                            Beneficiary_Mobile = obj.Beneficiary_Mobile;
                            Account_Number = obj.Account_Number;
                            beneficiary_bank_code = bankCode;

                            try
                            {
                                // login
                                ServicePointManager.Expect100Continue = true;
                                /*ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
                                                       | SecurityProtocolType.Tls11
                                                       | SecurityProtocolType.Tls12
                                                       | SecurityProtocolType.Ssl3;*/

                                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
                                                       | SecurityProtocolType.Tls11
                                                       | SecurityProtocolType.Tls12;
                                var client = new RestClient(apiurl + "/auth/login");
                                client.Timeout = -1;
                                var request = new RestRequest(Method.POST);
                                request.AddHeader("x-api-key", accesscode);
                                request.AddHeader("Content-Type", "application/json");
                                var body = @" {
" + "\n" +
                     @" ""username"": """ + username + @""",
" + "\n" +
                     @" ""password"": """ + password + @"""
" + "\n" +
                     @" } ";

                                request.AddParameter("application/json", body, ParameterType.RequestBody);

                                string req = apiurl + "/auth/login" + body;
                                CompanyInfo.InsertActivityLogDetails("Asal Access Token Request: <br/>" + req + "", obj.User_ID, 0, obj.User_ID, 0, "Asal Proceed", 0, obj.Client_ID, "Asal Proceed", context);

                                IRestResponse response = client.Execute(request);
                                CompanyInfo.InsertActivityLogDetails("Asal Access Token Response: <br/>" + response.Content + "", obj.User_ID, 0, obj.User_ID, 0, "Asal Proceed", 0, obj.Client_ID, "Asal Proceed", context);



                                dynamic json = Newtonsoft.Json.JsonConvert.DeserializeObject(response.Content);

                                var jsonObject = JObject.Parse(response.Content);


                                authToken = jsonObject["access_token"].ToString();

                            }
                            catch (Exception ex)
                            {
                                CompanyInfo.InsertErrorLogDetails(ex.ToString(), obj.User_ID, "Asal Access Token Error", 0, obj.Client_ID);
                            }




                            try
                            {

                                // Get Countries

                                var client = new RestClient(apiurl + "/remit/country");
                                client.Timeout = -1;
                                var request = new RestRequest(Method.GET);
                                request.AddHeader("x-api-key", accesscode);
                                request.AddHeader("Authorization", "Bearer " + authToken);
                                request.AddHeader("Content-Type", "application/json");

                                string req = apiurl + "/remit/country" + authToken;
                                CompanyInfo.InsertActivityLogDetails("Asal Get country Request: <br/>" + req + "", obj.User_ID, 0, obj.User_ID, 0, "Asal Proceed", 0, obj.Client_ID, "Asal Proceed", context);

                                IRestResponse response = client.Execute(request);
                                CompanyInfo.InsertActivityLogDetails("Asal Get country Response: <br/>" + response.Content + "", obj.User_ID, 0, obj.User_ID, 0, "Asal Proceed", 0, obj.Client_ID, "Asal Proceed", context);

                                if (PaymentDepositType_ID == 1 || PaymentDepositType_ID == 3)
                                {
                                    targetchannel = "channel";
                                }
                                else if (PaymentDepositType_ID == 2)
                                {
                                    targetchannel = "cash";
                                }

                                dynamic json = Newtonsoft.Json.JsonConvert.DeserializeObject(response.Content);

                                var jsonObject = JObject.Parse(response.Content);


                                string targetCountryName = Beneficiary_Country;//"Somalia";


                                foreach (var country in jsonObject["data"])
                                {

                                    if (country["countryName"].ToString() == targetCountryName && country["channel"].ToString() == targetchannel)
                                    {

                                        string countryname = country["countryName"].ToString();
                                        country_no = country["countryNO"].ToString();
                                        string channel = country["channel"].ToString();
                                        break;
                                    }
                                }

                            }
                            catch (Exception ex)
                            {
                                CompanyInfo.InsertErrorLogDetails(ex.ToString(), obj.User_ID, "Asal Get country Error", 0, obj.Client_ID);
                            }



                            if (PaymentDepositType_ID == 1 || PaymentDepositType_ID == 3)
                            {
                                try
                                {
                                    // Get Channel

                                    var client = new RestClient(apiurl + "/remit/channels");
                                    client.Timeout = -1;
                                    var request = new RestRequest(Method.POST);
                                    request.AddHeader("x-api-key", accesscode);
                                    request.AddHeader("Authorization", "Bearer " + authToken);
                                    request.AddHeader("Content-Type", "application/json");
                                    var body = @" {
" + "" +
                         @" ""countryNo"": """ + country_no + @"""
" + "" +
                                    @" } ";

                                    request.AddParameter("application/json", body, ParameterType.RequestBody);

                                    string req = apiurl + "/remit/channels" + body;
                                    CompanyInfo.InsertActivityLogDetails("Asal Get Channel Request: <br/>" + req + "", obj.User_ID, 0, obj.User_ID, 0, "Asal Proceed", 0, obj.Client_ID, "Asal Proceed", context);

                                    IRestResponse response = client.Execute(request);
                                    CompanyInfo.InsertActivityLogDetails("Asal Get Channel Response: <br/>" + response.Content + "", obj.User_ID, 0, 0, 0, "Asal Proceed", 0, obj.Client_ID, "Asal Proceed", context);



                                    dynamic json = Newtonsoft.Json.JsonConvert.DeserializeObject(response.Content);

                                    var jsonObject = JObject.Parse(response.Content);




                                    if (PaymentDepositType_ID == 1)
                                    {
                                        targetchannel = "Bank";

                                        foreach (var channel in jsonObject["data"])
                                        {

                                            if (channel["channelType"].ToString() == targetchannel && beneficiary_bank_code == channel["channelNo"].ToString())
                                            {

                                                string channelType = channel["channelType"].ToString();
                                                string channelName = channel["channelName"].ToString();
                                                channelNo = channel["channelNo"].ToString();
                                                break;
                                            }
                                        }
                                    }
                                    else if (PaymentDepositType_ID == 3)
                                    {
                                        targetchannel = "Wallet";

                                        foreach (var channel in jsonObject["data"])
                                        {
                                            string normalizedChannelName = new string(channel["channelName"].ToString()
                                                 .Where(c => char.IsLetterOrDigit(c))
                                                 .ToArray())
                                                 .ToLower();
                                            if (channel["channelType"].ToString() == targetchannel && normalizedChannelName == Provider_name.ToLower())
                                            {

                                                string channelType = channel["channelType"].ToString();
                                                string channelName = channel["channelName"].ToString();
                                                channelNo = channel["channelNo"].ToString();
                                                break;
                                            }
                                        }
                                    }



                                }
                                catch (Exception ex)
                                {
                                    CompanyInfo.InsertErrorLogDetails(ex.ToString(), obj.User_ID, "Asal Get Channel Error", 0, obj.Client_ID);
                                }





                                // 3 is mobile wallet
                                // 2 is cash picup
                                // 1 is direct to bank

                                if (PaymentDepositType_ID == 1)
                                {
                                    destinationNo = Account_Number;
                                }
                                else if (PaymentDepositType_ID == 3)
                                {
                                    destinationNo = Beneficiary_Mobile;
                                }



                                try
                                {
                                    // Validate Account

                                    var client = new RestClient(apiurl + "/remit/validateAccount");
                                    client.Timeout = -1;
                                    var request = new RestRequest(Method.POST);
                                    request.AddHeader("x-api-key", accesscode);
                                    request.AddHeader("Authorization", "Bearer " + authToken);
                                    request.AddHeader("Content-Type", "application/json");
                                    var body = @" {
"
                                    + "\n" +
                         @" ""destinationNo"": """ + destinationNo + @""",
" + "\n" +
                         @" ""channelNo"": """ + channelNo + @"""
" + "\n" +
                         @" } ";

                                    request.AddParameter("application/json", body, ParameterType.RequestBody);
                                    string req = apiurl + "/remit/validateAccount" + body;
                                    CompanyInfo.InsertActivityLogDetails("Asal Validate Account Request: <br/>" + req + "", obj.User_ID, 0, obj.User_ID, 0, "Asal Proceed", 0, obj.Client_ID, "Asal Proceed", context);

                                    IRestResponse response = client.Execute(request);
                                    CompanyInfo.InsertActivityLogDetails("Asal Validate Account Response: <br/>" + response.Content + "", obj.User_ID, 0, obj.User_ID, 0, "Asal Proceed", 0, obj.Client_ID, "Asal Proceed", context);



                                    dynamic json = Newtonsoft.Json.JsonConvert.DeserializeObject(response.Content);

                                    var jsonObject = JObject.Parse(response.Content);

                                    status = (bool)jsonObject["status"];
                                    if (status == true)
                                    {
                                        string message = (string)jsonObject["message"];
                                        string channelName = (string)jsonObject["channel_name"];
                                        accountName = (string)jsonObject["account_name"];
                                        string accountNumber = (string)jsonObject["account_number"];

                                    }
                                    else if (status == false)
                                    {
                                        string message = (string)jsonObject["detail"];
                                    }
                                    obj.AccountHolderName = accountName;
                                }
                                catch (Exception ex)
                                {
                                    CompanyInfo.InsertErrorLogDetails(ex.ToString(), obj.User_ID, "Asal Validate Account Error", 0, obj.Client_ID);
                                }
                            }

                        }
                        catch (Exception ex)
                        {
                            CompanyInfo.InsertActivityLogDetails("Get AccountHolder Name Dynathopia response Error :<br>:" + ex.ToString(), 0, 0, 0, 0, "Search", 0, 0, "Get AccountHolder Name Dynathopia response error", context);
                        }
                    }
                    else if (api_id == 38 && activeforbeneficiaryvalidation == 0)
                    {
                        string acc_holder_name = "";
                        try
                        {
                            string accNumber = Convert.ToString(obj.Account_Number).Trim();
                            bankCode = Convert.ToString(dtofKMOBankCode.Rows[0]["bank_code"]);
                            try
                            {
                                var client = new RestClient(apiurl + "/payments/verify-account/" + bankCode + "/" + accNumber);
                                client.Timeout = -1;
                                var request = new RestRequest(Method.GET);
                                request.AddHeader("Content-Type", "application/json");
                                request.AddHeader("x-anchor-key", accesscode);
                                string req = apiurl + "/payments/verify-account/" + bankCode + "/" + accNumber;
                                CompanyInfo.InsertActivityLogDetails("Anchor Verify Account Request: <br/>" + req + "", obj.User_ID, 0, obj.User_ID, 0, "Anchor Proceed", 0, obj.Client_ID, "Anchor Benef Name Search", context);
                                IRestResponse response = client.Execute(request);
                                CompanyInfo.InsertActivityLogDetails("Anchor Verify Account Response: <br/>" + response.Content + "", obj.User_ID, 0, obj.User_ID, 0, "Anchor Proceed", 0, obj.Client_ID, "Anchor Benef Name Search", context);
                                var jsonObject = JObject.Parse(response.Content);
                                acc_holder_name = jsonObject["data"]?["attributes"]?["accountName"]?.ToString();

                            }
                            catch (Exception ex)
                            {
                                CompanyInfo.InsertActivityLogDetails("Error Get account details For Budpay response <br>:" + api_id + " " + ex.ToString(), Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(obj.Customer_ID), "getAccountHolderDetails", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "Get Budpay Beneficiary Bank Account Details", context);
                            }
                            obj.AccountHolderName = acc_holder_name;
                        }
                        catch (Exception ex)
                        {
                            CompanyInfo.InsertActivityLogDetails("Get AccountHolder Name Budpay response Error :<br>:" + ex.ToString(), 0, 0, 0, 0, "Search", 0, 0, "Get AccountHolder Name Budpay response error", context);
                        }
                    }
                    else if (api_id == 41 && activeforbeneficiaryvalidation == 0)//pradip
                    {
                        string acc_holder_name = "";
                        try
                        {
                            string token = mtsIntegrationmethods.TokenGenration(dt, context);
                            if (token != "ERROR")
                            {
                                try
                                {
                                    string apiField = Convert.ToString(dt.Rows[i]["api_Fields"]);
                                    string Token = "", AgentID = "", EmployeeCode = "", EmployeePassword = "";
                                    if (apiField != "" && apiField != null)
                                    {
                                        Newtonsoft.Json.Linq.JObject objAPI = Newtonsoft.Json.Linq.JObject.Parse(apiField);
                                        Token = Convert.ToString(objAPI["Token"]);
                                        AgentID = Convert.ToString(objAPI["AgentID"]);
                                        EmployeeCode = Convert.ToString(objAPI["EmployeeCode"]);
                                        EmployeePassword = Convert.ToString(objAPI["EmployeePassword"]);
                                    }
                                    bankCode = Convert.ToString(dtofKMOBankCode.Rows[0]["bank_code"]);

                                    string AcNo = obj.Account_Number;
                                    string Temp_url = apiurl + "/api/ValidateAccount";
                                    var clientAcValidation = new RestClient(Temp_url);
                                    clientAcValidation.Timeout = -1;
                                    var requestAcValidation = new RestRequest();
                                    requestAcValidation.Method = Method.POST;
                                    requestAcValidation.AddHeader("Authentication", Token + ":" + token);
                                    requestAcValidation.AddHeader("Content-Type", "application/json");
                                    var body = @"{
" + "\n" +
                                    @"""accountnumber"" :""" + AcNo + @""",
" + "\n" +
                                    @"""bank"":""" + bankCode + @"""
" + "\n" +
                                    @"}";
                                    requestAcValidation.AddParameter("application/json", body, ParameterType.RequestBody);
                                    CompanyInfo.InsertActivityLogDetails(" quickremit request parameters GetNIPAccount: <br/>" + body + "", 0, 0, 0, 0, " Get quickremit Transaction Status", 0, 0, " Get Providus Transaction Status", context);
                                    IRestResponse responseAcValidation = clientAcValidation.Execute(requestAcValidation);
                                    string jsonResponse = responseAcValidation.Content;
                                    CompanyInfo.InsertActivityLogDetails(" quickremit response parameters GetNIPAccount: <br/>" + responseAcValidation.Content + "", 0, 0, 0, 0, " Get quickremit Transaction Status", 0, 0, " Get Providus Transaction Status", context);
                                    JObject responseObject = JObject.Parse(jsonResponse);
                                    int Message = 0;
                                    if (responseObject["StatusCode"]?.ToString() == "200") { obj.AccountHolderName = responseObject["AccountName"]?.ToString(); }
                                    else { Message = 0; }


                                    acc_holder_name = responseObject["AccountName"]?.ToString();

                                }
                                catch (Exception ex)
                                {
                                    CompanyInfo.InsertActivityLogDetails("Generate TokenGenration quickremit response Error :<br>:" + ex.ToString(), 0, 0, 0, 0, "Search", 0, 0, "Generate TokenGenration quickremit response error", context);
                                }
                                obj.AccountHolderName = acc_holder_name;
                            }
                            else
                            {
                                CompanyInfo.InsertActivityLogDetails("Generate TokenGenration Quick Remit response Error :<br>:" + token, 0, 0, 0, 0, "Search", 0, 0, "Generate TokenGenration Quick Remit  response error", context);
                            }
                        }
                        catch (Exception ex)
                        {
                            CompanyInfo.InsertActivityLogDetails("Generate TokenGenration Quick Remit response Error :<br>:" + ex.ToString(), 0, 0, 0, 0, "Search", 0, 0, "Generate TokenGenration Quick Remit  response error", context);
                        }
                    }
                    else if (api_id == 45 && activeforbeneficiaryvalidation == 0)
                    {
                        string acc_holder_name = "";
                        try
                        {
                            string token = "";
                            try
                            {
                                string accNumber = Convert.ToString(obj.Account_Number).Trim();
                                bankCode = Convert.ToString(dtofKMOBankCode.Rows[0]["bank_code"]);
                                var client = new RestClient(apiurl + "/fxwalletapi/v1/service/feluwaaddai/checkaccount/");
                                client.Timeout = -1;
                                var request = new RestRequest(Method.POST);
                                request.AddHeader("Content-Type", "application/json");
                                request.AddHeader("accesskey", apiuser);
                                request.AddHeader("secrete", apipass);
                                var body = @"{" + "\n" + @"   
                        " + "\n" +
                             @"    ""accountnumber"": """ + accNumber + @""",
                        " + "\n" +
                             @"    ""bank"": """ + bankCode + @"""
                        " + "\n" +
                             @"}";

                                request.AddParameter("application/json", body, ParameterType.RequestBody);
                                string req = apiurl + "/fxwalletapi/v1/service/feluwaaddai/checkaccount/" + body;
                                CompanyInfo.InsertActivityLogDetails("Dynathopia Get Beneficiary Name Request: <br/>" + req + "", obj.User_ID, 0, obj.User_ID, 0, "getAccountHolderDetails", 0, obj.Client_ID, "getAccountHolderDetails", context);
                                IRestResponse response = client.Execute(request);
                                CompanyInfo.InsertActivityLogDetails("Dynathopia Get Beneficiary Name Response: <br/>" + response.Content + "", obj.User_ID, 0, obj.User_ID, 0, "getAccountHolderDetails", 0, obj.Client_ID, "getAccountHolderDetails", context);
                                var jsonObject = JObject.Parse(response.Content);

                                acc_holder_name = jsonObject["name"]?.ToString();
                            }
                            catch (Exception ex)
                            {
                                CompanyInfo.InsertActivityLogDetails("Error In Dynathopia Get Beneficiary Name Response: <br/>" + ex.ToString() + "", obj.User_ID, 0, obj.User_ID, 0, "getAccountHolderDetails", 0, obj.Client_ID, "getAccountHolderDetails", context);
                            }
                            obj.AccountHolderName = acc_holder_name;
                        }
                        catch (Exception ex)
                        {
                            CompanyInfo.InsertActivityLogDetails("Get AccountHolder Name Dynathopia response Error :<br>:" + ex.ToString(), 0, 0, 0, 0, "Search", 0, 0, "Get AccountHolder Name Dynathopia response error", context);
                        }
                    }
                    else if (api_id == 47 && activeforbeneficiaryvalidation == 0)// Rushikesh 28-02-2025
                    {
                        string acc_holder_name = "";
                        try
                        {
                            string accNumber = Convert.ToString(obj.Account_Number).Trim();
                            bankCode = Convert.ToString(dtofKMOBankCode.Rows[0]["bank_code"]);
                            try
                            {
                                var client = new RestClient(apiurl + "/api/v1/account_name_verify");
                                client.Timeout = -1;
                                var request = new RestRequest(Method.POST);
                                request.AddHeader("Content-Type", "application/json");
                                request.AddHeader("Authorization", "Bearer " + accesscode);
                                var body = @"{
                                    " + "\n" +
                                @"    ""bank_code"": """ + bankCode + @""",
                                    " + "\n" +
                                @"    ""account_number"": """ + accNumber + @"""
                                    " + "\n" +
                                @"}";
                                request.AddParameter("application/json", body, ParameterType.RequestBody);
                                string req = apiurl + "/api/v2/account_name_verify " + body;
                                CompanyInfo.InsertActivityLogDetails("Budpay Verify Account Request: <br/>" + req + "", obj.User_ID, 0, obj.User_ID, 0, "Budpay Proceed", 0, obj.Client_ID, "Budpay Benef Name Search", context);
                                IRestResponse response = client.Execute(request);
                                CompanyInfo.InsertActivityLogDetails("Budpay Verify Account Response: <br/>" + response.Content + "", obj.User_ID, 0, obj.User_ID, 0, "Budpay Proceed", 0, obj.Client_ID, "Budpay Benef Name Search", context);
                                var jsonObject = JObject.Parse(response.Content);
                                bool success = jsonObject["success"].Value<bool>();
                                if (success == true)
                                {
                                    string message = jsonObject["message"].ToString();
                                    string data = jsonObject["data"].ToString();
                                    acc_holder_name = data;
                                }
                                else
                                {
                                    CompanyInfo.InsertActivityLogDetails("Error Get account details For Budpay API response <br>:" + api_id + " " + jsonObject["message"].ToString(), Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(obj.Customer_ID), "getAccountHolderDetails", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "Get Budpay Beneficiary Bank Account Details", context);
                                }
                            }
                            catch (Exception ex)
                            {
                                CompanyInfo.InsertActivityLogDetails("Error Get account details For Budpay response <br>:" + api_id + " " + ex.ToString(), Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(obj.Customer_ID), "getAccountHolderDetails", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "Get Budpay Beneficiary Bank Account Details", context);
                            }
                            obj.AccountHolderName = acc_holder_name;
                        }
                        catch (Exception ex)
                        {
                            CompanyInfo.InsertActivityLogDetails("Get AccountHolder Name Budpay response Error :<br>:" + ex.ToString(), 0, 0, 0, 0, "Search", 0, 0, "Get AccountHolder Name Budpay response error", context);
                        }
                    }

                }

            }
            catch (Exception ex)
            {
                CompanyInfo.InsertActivityLogDetails("Get AccountHolder Name Error :<br>:" + ex.ToString(), 0, 0, 0, 0, "Search", 0, 0, "Get AccountHolder Name Dynathopia response error", context);
            }
            obj.Customer_ID = CompanyInfo.Encrypt(obj.Customer_ID, true);
            return obj;
        }


        public Model.Beneficiary Holdername(Model.Beneficiary obj, HttpContext context)
        {
            Model.Beneficiary _ObjCustomer = new Model.Beneficiary();
            Service.srvBeneficiary srvBenf = new Service.srvBeneficiary(context);
            string AcHolder = obj.AccountHolderName;
            string AcNo = obj.Account_Number;
            return obj;

        }

        public DataTable Check_sanction_config_beneficiary(Model.Beneficiary obj, HttpContext context)
        {
            DataTable ds = new DataTable();
            ds.Columns.Add("Status", typeof(int));
            ds.Columns.Add("showalert", typeof(string));
            //ds.Columns.Add("Alertmessage", typeof(string));

            try
            {
                int Sanction_check = 2;

                string Whereclause = string.Empty;
                Whereclause = Whereclause + " and Client_ID = " + obj.Client_ID;
                Whereclause = Whereclause + " and Flag_cust_bene = " + Sanction_check;
                Whereclause = Whereclause + " order by ID desc";
                //Whereclause = Whereclause + " and Screening_Type = " + Screening_Type;
                MySqlConnector.MySqlCommand _cmd_chk = new MySqlConnector.MySqlCommand("select_sanction_config");
                _cmd_chk.CommandType = CommandType.StoredProcedure;
                _cmd_chk.Parameters.AddWithValue("_Whereclause", Whereclause);
                DataTable dt_sanction_config = db_connection.ExecuteQueryDataTableProcedure(_cmd_chk);

                string check_sanction_aml = "";
                string check_sanction_kyc = "";
                string check_sanction_aml_id = "";
                string check_sanction_kyc_id = "";
                int Days_Timeframe_aml = 0;
                int Days_Timeframe_kyc = 0;

                if (dt_sanction_config.Rows.Count > 0)
                {
                    var tt = dt_sanction_config.Rows.Count - 1;
                    int mand_flag = 0;
                    int Screening_Type = 0;
                    // for (int i = tt; i >= 0; i--)
                    for (int i = 0; i < dt_sanction_config.Rows.Count; i++)
                    {
                        mand_flag = Convert.ToInt16(dt_sanction_config.Rows[i]["Mandatory_Flag"]);
                        Screening_Type = Convert.ToInt16(dt_sanction_config.Rows[i]["Screening_Type"]);
                        if (mand_flag == 0 && Screening_Type == 2)
                        {
                            check_sanction_aml = Convert.ToString(dt_sanction_config.Rows[i]["Sanctions_Check"]);
                            check_sanction_aml_id = Convert.ToString(dt_sanction_config.Rows[i]["ID"]);
                            if (dt_sanction_config.Rows[i]["Days_Timeframe"] != DBNull.Value)
                            {
                                Days_Timeframe_aml = Convert.ToInt16(dt_sanction_config.Rows[i]["Days_Timeframe"]);
                            }
                            // break;
                        }
                        else if (mand_flag == 0 && Screening_Type == 1)
                        {
                            check_sanction_kyc = Convert.ToString(dt_sanction_config.Rows[i]["Sanctions_Check"]);
                            check_sanction_kyc_id = Convert.ToString(dt_sanction_config.Rows[i]["ID"]);
                            if (dt_sanction_config.Rows[i]["Days_Timeframe"] != DBNull.Value)
                            {
                                Days_Timeframe_kyc = Convert.ToInt16(dt_sanction_config.Rows[i]["Days_Timeframe"]);
                            }
                            // break;
                        }
                    }
                    string status_check_aml = "4";
                    int sanction_flag_aml = 0;
                    if (check_sanction_aml_id != "16")
                    {
                        if (check_sanction_aml_id == "15")
                        {
                            sanction_flag_aml = 3;
                        }
                        else if (check_sanction_aml_id == "14")
                        {
                            sanction_flag_aml = 2;
                        }
                        else if (check_sanction_aml_id == "13")
                        {
                            sanction_flag_aml = 1;
                        }
                        status_check_aml = CompanyInfo.CheckPepSanction(obj.Client_ID, 0, Convert.ToInt32(obj.Branch_ID), obj.Beneficiary_ID, sanction_flag_aml, Days_Timeframe_aml, 2, 0, context);
                    }

                    // status_check_aml = "2";
                    // HttpContext.Current.Session["sanction_responce_bene_aml"] = status_check_aml;
                    ds.Rows.Add(mand_flag, status_check_aml);


                    string status_check_kyc = "4";
                    int sanction_flag_kyc = 0;
                    if (check_sanction_kyc_id != "12")
                    {
                        if (check_sanction_kyc_id == "11")
                        {
                            sanction_flag_kyc = 3;
                        }
                        else if (check_sanction_kyc_id == "10")
                        {
                            sanction_flag_kyc = 2;
                        }
                        else if (check_sanction_kyc_id == "9")
                        {
                            sanction_flag_kyc = 1;
                        }
                        status_check_kyc = CompanyInfo.CheckPepSanction(obj.Client_ID, 0, Convert.ToInt32(obj.Branch_ID), obj.Beneficiary_ID, sanction_flag_kyc, Days_Timeframe_kyc, 1, 0, context);
                    }

                    //status_check_kyc = "2";
                    // HttpContext.Current.Session["sanction_responce_bene_kyc"] = status_check_kyc;
                    ds.Rows.Add(mand_flag, status_check_kyc);

                }
                return ds;
            }
            catch (Exception ex)
            {
                CompanyInfo.InsertActivityLogDetails("Check_sanction_config_beneficiary Error :<br>:" + ex.ToString(), 0, 0, 0, 0, "Get", 0, 0, "Check_sanction_config_beneficiary", context);
                return ds;
            }
        }

        public DataTable Check_sanction_config_customer(Model.Beneficiary obj, HttpContext context)
        {
            DataTable ds = new DataTable();
            ds.Columns.Add("Status", typeof(int));
            ds.Columns.Add("showalert", typeof(string));
            //ds.Columns.Add("Alertmessage", typeof(string));
            #region check sanction 
            try
            {
                string Whereclause = string.Empty;
                DataTable dt_sanction_config = new DataTable();
                Whereclause = Whereclause + " and Client_ID = " + obj.Client_ID;
                Whereclause = Whereclause + " and Flag_cust_bene = 1";
                //Whereclause = Whereclause + " and Screening_Type = 2";
                Whereclause = Whereclause + " order by ID desc";
                MySqlConnector.MySqlCommand _cmd_chk = new MySqlConnector.MySqlCommand("select_sanction_config");
                _cmd_chk.CommandType = CommandType.StoredProcedure;
                _cmd_chk.Parameters.AddWithValue("_Whereclause", Whereclause);
                dt_sanction_config = db_connection.ExecuteQueryDataTableProcedure(_cmd_chk);

                string check_sanction_aml = "";
                string check_sanction_kyc = "";
                string check_sanction_aml_id = "";
                string check_sanction_kyc_id = "";
                int Days_Timeframe_aml = 0;
                int Days_Timeframe_kyc = 0;
                int sanction_flag_aml_nrf = 0; //vyankatesh 17-10
                int sanction_flag_aml_kyc = 0; //vyankatesh 17-10

                if (dt_sanction_config.Rows.Count > 0)
                {
                    var tt = dt_sanction_config.Rows.Count - 1;
                    int mand_flag = 0;
                    int Screening_Type = 0;

                    //for (int i = tt; i >= 0; i--)
                    for (int i = 0; i < dt_sanction_config.Rows.Count; i++)
                    {
                        mand_flag = Convert.ToInt16(dt_sanction_config.Rows[i]["Mandatory_Flag"]);
                        Screening_Type = Convert.ToInt16(dt_sanction_config.Rows[i]["Screening_Type"]);
                        if (mand_flag == 0 && Screening_Type == 2)
                        {
                            //check_sanction_aml = Convert.ToString(dt_sanction_config.Rows[i]["Sanctions_Check"]);
                            check_sanction_aml_id = Convert.ToString(dt_sanction_config.Rows[i]["ID"]);
                            if (dt_sanction_config.Rows[i]["Days_Timeframe"] != DBNull.Value)
                            {
                                Days_Timeframe_aml = Convert.ToInt16(dt_sanction_config.Rows[i]["Days_Timeframe"]);
                            }
                            //break;
                        }
                        else if (mand_flag == 0 && Screening_Type == 1)
                        {
                            //check_sanction_kyc = Convert.ToString(dt_sanction_config.Rows[i]["Sanctions_Check"]);
                            check_sanction_kyc_id = Convert.ToString(dt_sanction_config.Rows[i]["ID"]);
                            if (dt_sanction_config.Rows[i]["Days_Timeframe"] != DBNull.Value)
                            {
                                Days_Timeframe_kyc = Convert.ToInt16(dt_sanction_config.Rows[i]["Days_Timeframe"]);
                            }
                            //break;
                        }
                        try
                        {
                            if (Convert.ToInt32(dt_sanction_config.Rows[i]["ID"]) == 17 && Convert.ToUInt32(dt_sanction_config.Rows[i]["Delete_Status"]) == 0) //vyankatesh 17-10
                            {
                                sanction_flag_aml_kyc = 1;
                            }
                            if (Convert.ToInt32(dt_sanction_config.Rows[i]["ID"]) == 18 && Convert.ToUInt32(dt_sanction_config.Rows[i]["Delete_Status"]) == 0) //vyankatesh 17-10
                            {
                                sanction_flag_aml_nrf = 1;
                            }
                        }
                        catch(Exception ex)
                        {
                            _ = Task.Run (() => CompanyInfo.InsertActivityLogDetails("Check_sanction_config_customer Error :<br>:" + ex.ToString(), 0, 0, 0, 0, "Get", 0, 0, "Check_sanction_config_customer", context));
                        }
                    }
                    string status_check_kyc = "4";
                    int sanction_flag_kyc = 0;
                    string status_check_aml = "4";
                    int sanction_flag_aml = 0;
                    if (check_sanction_kyc_id != "4")
                    {
                        if (check_sanction_kyc_id == "3")
                        {
                            sanction_flag_kyc = 3;
                        }
                        else if (check_sanction_kyc_id == "2")
                        {
                            sanction_flag_kyc = 2;
                        }
                        else if (check_sanction_kyc_id == "1")
                        {
                            sanction_flag_kyc = 1;
                        }
                        status_check_kyc = CompanyInfo.CheckPepSanction(obj.Client_ID, Convert.ToInt32(CompanyInfo.Decrypt(Convert.ToString(obj.Customer_ID), true)), Convert.ToInt32(obj.Branch_ID), 0, sanction_flag_kyc, Days_Timeframe_kyc, 1, sanction_flag_aml_kyc, context);
                    }
                    else if (sanction_flag_aml_kyc == 1)
                    {
                        MySqlCommand _cmd1 = new MySqlCommand("CheckIDExpiry");
                        _cmd1.CommandType = CommandType.StoredProcedure;
                        _cmd1.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                        _cmd1.Parameters.AddWithValue("_IDType_ID", 1);
                        _cmd1.Parameters.AddWithValue("_Customer_ID", CompanyInfo.Decrypt(Convert.ToString(obj.Customer_ID), true));
                        DataTable table = db_connection.ExecuteQueryDataTableProcedure(_cmd1);

                        if (table.Rows.Count == 0) //to check nrf
                        {
                            status_check_kyc = CompanyInfo.CheckPepSanction(obj.Client_ID, Convert.ToInt32(CompanyInfo.Decrypt(Convert.ToString(obj.Customer_ID), true)), Convert.ToInt32(obj.Branch_ID), 0, sanction_flag_kyc, Days_Timeframe_kyc, 1, sanction_flag_aml_kyc, context);
                        }
                    }
                    if (status_check_kyc == "0")
                    {
                        // HttpContext.Current.Session["sanction_responce_cust_kyc"] = status_check_kyc;
                        ds.Rows.Add(2, "", 0, "", Convert.ToInt32(status_check_kyc), check_sanction_kyc);

                        // HttpContext.Current.Session["sanction_responce_cust_aml"] = status_check_aml;
                        ds.Rows.Add(2, "", 0, "", Convert.ToInt32(status_check_aml), check_sanction_aml);

                        //status_check_kyc = "2";

                        return ds;
                    }
                    //status_check_kyc = "2";

                    //HttpContext.Current.Session["sanction_responce_cust_kyc"] = status_check_kyc;
                    ds.Rows.Add(mand_flag, status_check_kyc);



                    if (check_sanction_aml_id != "8")
                    {
                        if (check_sanction_aml_id == "7")
                        {
                            sanction_flag_aml = 3;
                        }
                        else if (check_sanction_aml_id == "6")
                        {
                            sanction_flag_aml = 2;
                        }
                        else if (check_sanction_aml_id == "5")
                        {
                            sanction_flag_aml = 1;
                        }
                        status_check_aml = CompanyInfo.CheckPepSanction(obj.Client_ID, Convert.ToInt32(CompanyInfo.Decrypt(Convert.ToString(obj.Customer_ID), true)), Convert.ToInt32(obj.Branch_ID), 0, sanction_flag_aml, Days_Timeframe_aml, 2, sanction_flag_aml_nrf, context);
                    }
                    else if (sanction_flag_aml_nrf == 1)
                    {
                        MySqlCommand _cmd1 = new MySqlCommand("CheckIDExpiry");
                        _cmd1.CommandType = CommandType.StoredProcedure;
                        _cmd1.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                        _cmd1.Parameters.AddWithValue("_IDType_ID", 1);
                        _cmd1.Parameters.AddWithValue("_Customer_ID", CompanyInfo.Decrypt(Convert.ToString(obj.Customer_ID), true));
                        DataTable table = db_connection.ExecuteQueryDataTableProcedure(_cmd1);

                        if (table.Rows.Count == 0) //to check nrf
                        {
                            status_check_aml = CompanyInfo.CheckPepSanction(obj.Client_ID, Convert.ToInt32(CompanyInfo.Decrypt(Convert.ToString(obj.Customer_ID), true)), Convert.ToInt32(obj.Branch_ID), 0, sanction_flag_aml, Days_Timeframe_aml, 2, sanction_flag_aml_nrf, context);
                        }
                    }
                        //status_check_aml = "2";
                        // HttpContext.Current.Session["sanction_responce_cust_aml"] = status_check_aml;
                        ds.Rows.Add(mand_flag, status_check_aml);
                }
                return ds;
            }
            catch (Exception ex_chk)
            {

                CompanyInfo.InsertActivityLogDetails("Check_sanction_config_customer Error :<br>:" + ex_chk.ToString(), 0, 0, 0, 0, "Get", 0, 0, "Check_sanction_config_customer", context);
                return ds;
            }
            #endregion

        }

        public DataTable check_Rekyc_customer(Model.Beneficiary obj, HttpContext context)
        {
            DataTable ds = new DataTable();
            ds.Columns.Add("Status", typeof(int));
            ds.Columns.Add("showalert", typeof(string));
            //ds.Columns.Add("Alertmessage", typeof(string));
            #region check sanction 
            try
            {
                string Whereclause = string.Empty;
                DataTable dt_sanction_config = new DataTable();
                string Query = string.Empty;
                int Customer_id = Convert.ToInt32(CompanyInfo.Decrypt(Convert.ToString(obj.Customer_ID), true));
                Query = "select ReKyc_Eligibility  from customer_registration where Customer_ID = " + Customer_id + " ";
                MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("Default_SP");
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.Parameters.AddWithValue("_Query", Query);
                dt_sanction_config = db_connection.ExecuteQueryDataTableProcedure(_cmd);

                if (dt_sanction_config.Rows.Count > 0)
                {
                    //Activity += " ReKyc Eligibility  " + dt_sanction_config.Rows[0]["ReKyc_Eligibility"];

                    ds.Rows.Add(0, Convert.ToInt32(dt_sanction_config.Rows[0]["ReKyc_Eligibility"]));

                }

                return ds;
            }
            catch (Exception ex_chk)
            {

                CompanyInfo.InsertActivityLogDetails("check_Rekyc_customer Error :<br>:" + ex_chk.ToString(), 0, 0, 0, 0, "Get", 0, 0, "Check_sanction_config_customer", context);
                return ds;
            }
            #endregion

        }

        public Model.Beneficiary CreateMedicare(Model.Beneficiary obj, HttpContext context)
        {
            try
            {
                int Customer_ID = Convert.ToInt32(CompanyInfo.Decrypt(Convert.ToString(obj.Customer_ID), true));
                string log = "start,"; string Activity = string.Empty; string error_msg = ""; string error_invalid_data = ""; string enjected_data = ""; string notification_icon = ""; string notification_message = "";
                //var context = System.Web.HttpContext.Current;
                //string Username = Convert.ToString(context.Request.Form["Username"]);
                string MobileNumber_regex = validation.validate(obj.Beneficiary_Telephone, 1, 1, 1, 0, 1, 1, 1, 1, 1);
                string Name_regex = validation.validate(obj.Beneficiary_Name, 1, 1, 1, 1, 1, 1, 0, 1, 1);
                string Email_regex = validation.validate(obj.Beneficiary_Email, 1, 1, 1, 1, 0, 1, 1, 1, 1);

                obj.Record_Insert_DateTime = Convert.ToDateTime(CompanyInfo.gettime(obj.Client_ID, context));

                if (MobileNumber_regex == "false")
                {
                    error_msg = error_msg + " Phone number Should be numeric without space";
                    error_invalid_data = "Phone number: " + obj.Beneficiary_Telephone;
                }
                if (Name_regex == "false")
                {
                    error_msg = error_msg + "Name should be characters with space "; ;
                    error_invalid_data = "Name: " + obj.Beneficiary_Name;
                }
                if (Email_regex == "false")
                {
                    error_msg = error_msg + " Email Should be valid without space."; ;
                    error_invalid_data = "Email: " + obj.Beneficiary_Email;
                }


                if (Email_regex != "false" && Name_regex != "false" && MobileNumber_regex != "false")
                {
                    log += "inside regex check,";
                    string MobileNumber = CompanyInfo.testInjection(Convert.ToString(obj.Beneficiary_Telephone));
                    string Name = CompanyInfo.testInjection(Convert.ToString(obj.Beneficiary_Name));

                    string Email = CompanyInfo.testInjection(Convert.ToString(obj.Beneficiary_Email));

                    if (Email == "1" && MobileNumber == "1" && Name == "1")
                    {
                        log += "inside if (Email == \"1\" &&,";
                        try
                        {
                            if (obj.Beneficiary_ID != 0)
                            {
                                #region add beneficiary to health care
                                log += "inside if (obj.Beneficiary_ID != 0),";
                                MySqlCommand cmdc = new MySqlCommand("get_benef_medicare_data_api");
                                cmdc.CommandType = CommandType.StoredProcedure;
                                cmdc.Parameters.AddWithValue("_Beneficiary_Telephone", obj.Beneficiary_Telephone);
                                cmdc.Parameters.Add(new MySqlParameter("_status", MySqlDbType.String));
                                cmdc.Parameters["_status"].Direction = ParameterDirection.Output;

                                DataTable dtm = db_connection.ExecuteQueryDataTableProcedure(cmdc);

                                if (dtm == null || dtm.Rows.Count == 0 || Convert.ToString(dtm.Rows[0]["Active_status"]) != "Active")
                                {
                                    log += "inside if (dtm == null ||,";
                                    int total_members = 0;

                                    string where_clause = " Customer_ID=" + Customer_ID + " and Client_ID=" + obj.Client_ID;
                                    MySqlCommand _cmda = new MySqlCommand("get_referral_count");
                                    _cmda.CommandType = CommandType.StoredProcedure;
                                    _cmda.Parameters.AddWithValue("_Whereclause", where_clause);
                                    DataTable dt_ref = db_connection.ExecuteQueryDataTableProcedure(_cmda);
                                    if (dt_ref != null && dt_ref.Rows.Count > 0)
                                    {
                                        log += "inside if (dt_ref != null &&,";
                                        if (dt_ref.Rows[0]["referral_medicare_count"] != null)
                                        {
                                            total_members = Convert.ToInt32(dt_ref.Rows[0]["referral_medicare_count"]);
                                        }
                                        DateTime Next_Deduction_Date = obj.Record_Insert_DateTime.AddDays(92);
                                        DataTable dt = new DataTable();
                                        mts_connection _MTS = new mts_connection();
                                        using (MySqlConnection con = new MySqlConnection(_MTS.WebConnSetting()))
                                        {
                                            con.Open();
                                            MySqlTransaction transaction;
                                            transaction = con.BeginTransaction(IsolationLevel.ReadCommitted);
                                            try
                                            {
                                                if (total_members > 0)
                                                {
                                                    log += "inside if (total_members > 0),";
                                                    if (dt_ref.Rows[0]["referral_medicare_days"] != null)
                                                    {
                                                        obj.Medicare_Expiry_Date = obj.Record_Insert_DateTime.AddDays((Convert.ToInt32(dt_ref.Rows[0]["referral_medicare_days"]) + 1));
                                                        Next_Deduction_Date = obj.Record_Insert_DateTime.AddDays((Convert.ToInt32(dt_ref.Rows[0]["referral_medicare_days"]) + 2)); //next day after expiry
                                                    }

                                                    int new_cnt = total_members - 1;
                                                    string where = " Customer_ID=" + Customer_ID + " and Client_ID=" + obj.Client_ID;
                                                    MySqlCommand cmd = new MySqlCommand("update_referral_medicare_count", con);
                                                    cmd.CommandType = CommandType.StoredProcedure;
                                                    cmd.Transaction = transaction;
                                                    cmd.Parameters.AddWithValue("_referral_medicare_count", "-1");
                                                    cmd.Parameters.AddWithValue("_referral_medicare_days", dt_ref.Rows[0]["referral_medicare_days"]);
                                                    cmd.Parameters.AddWithValue("_whereclause", where);
                                                    //int ur = db_connection.ExecuteNonQueryProcedure(cmd);
                                                    int ur = cmd.ExecuteNonQuery();

                                                    if (ur > 0)
                                                    {
                                                        log += "inside if (ur > 0),";
                                                        string act = " Referral medicare count updated for Customer_ID= " + Customer_ID + ". New Count: " + new_cnt + ", Old Count: " + total_members + " ";
                                                        int sts = (int)CompanyInfo.InsertActivityLogDetails(act, obj.User_ID, 0, obj.User_ID, Customer_ID, "CreateMedicare", obj.Branch_ID, obj.Client_ID, "CreateMedicare", context);

                                                        MySqlCommand _cmd = new MySqlCommand("save_beneficiary_medicare", con);
                                                        _cmd.CommandType = CommandType.StoredProcedure;
                                                        _cmd.Transaction = transaction;
                                                        _cmd.Parameters.AddWithValue("_Customer_ID", Customer_ID);
                                                        _cmd.Parameters.AddWithValue("_Beneficiary_ID", obj.Beneficiary_ID);
                                                        _cmd.Parameters.AddWithValue("_Membership_ID", 0);
                                                        _cmd.Parameters.AddWithValue("_Referral_Flag", 0);
                                                        _cmd.Parameters.AddWithValue("_Temporary_Inactive", 1);
                                                        _cmd.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                                                        _cmd.Parameters.AddWithValue("_Next_Deduction_DateTime", Next_Deduction_Date);
                                                        _cmd.Parameters.AddWithValue("_Medicare_Expiry_Date", obj.Medicare_Expiry_Date);
                                                        _cmd.Parameters.AddWithValue("_Last_Updated_DateTime", obj.Record_Insert_DateTime);
                                                        _cmd.Parameters.AddWithValue("_Record_Insert_DateTime", obj.Record_Insert_DateTime);
                                                        _cmd.Parameters.AddWithValue("_Total_Members", 0);
                                                        _cmd.Parameters.AddWithValue("_Slots_For_Customer", 0);
                                                        _cmd.Parameters.AddWithValue("_Person_Flag", 0);
                                                        //dt = db_connection.ExecuteQueryDataTableProcedure(_cmd);
                                                        MySqlDataAdapter da = new MySqlDataAdapter(_cmd);
                                                        da.Fill(dt);
                                                    }
                                                    else
                                                    {
                                                        log += "inside else of if (ur > 0),";
                                                        obj.Message = "Failed";
                                                        string act = "Failed to update referral medicare count for Customer_ID= " + Customer_ID + ". Old referral medicare count: " + total_members + " ";
                                                        int sts = (int)CompanyInfo.InsertActivityLogDetails(act, obj.User_ID, 0, obj.User_ID, Customer_ID, "CreateMedicare", obj.Branch_ID, obj.Client_ID, "CreateMedicare", context);
                                                        return obj;
                                                    }
                                                }
                                                else
                                                {
                                                    log += "inside else of if (total_members > 0),";
                                                    obj.Medicare_Expiry_Date = obj.Record_Insert_DateTime.AddYears(1);
                                                    string whereclause = " and cmmt.Customer_ID=" + Customer_ID + " and cmmt.Client_ID=" + obj.Client_ID + " and mbmt.Benefit_ID=1";
                                                    MySqlCommand _cmds = new MySqlCommand("get_customer_membership_mapping");
                                                    _cmds.CommandType = CommandType.StoredProcedure;
                                                    _cmds.Parameters.AddWithValue("_Whereclause", whereclause);
                                                    DataTable dt_data = db_connection.ExecuteQueryDataTableProcedure(_cmds);

                                                    if (dt_data != null && dt_data.Rows.Count > 0)
                                                    {
                                                        log += "inside if (dt_data != null &&,";
                                                        double benefit_points = Convert.ToDouble(dt_data.Rows[0]["Points"]);
                                                        total_members = Convert.ToInt32(dt_data.Rows[0]["Total_Members"]);
                                                        int Membership_ID = Convert.ToInt32(dt_data.Rows[0]["Membership_ID"]);

                                                        if (dt_data.Rows[0]["Validity_In_Days"] != null)
                                                        {
                                                            Next_Deduction_Date = obj.Record_Insert_DateTime.AddDays((Convert.ToInt32(dt_data.Rows[0]["Validity_In_Days"]) + 2)); //next day after expiry
                                                        }

                                                        MySqlCommand _cmd1 = new MySqlCommand("get_points_wallet_details");
                                                        _cmd1.CommandType = CommandType.StoredProcedure;
                                                        _cmd1.Parameters.AddWithValue("_Customer_ID", Customer_ID);
                                                        _cmd1.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                                                        DataTable dtwallets = db_connection.ExecuteQueryDataTableProcedure(_cmd1);

                                                        if (dtwallets != null && dtwallets.Rows.Count > 0)
                                                        {

                                                            log += "inside if (dtwallets != null &&,";
                                                            int points_wallet_id = Convert.ToInt32(dtwallets.Rows[0]["Points_Wallet_ID"]);
                                                            int wallet_active = Convert.ToInt32(dtwallets.Rows[0]["Delete_Status"]);
                                                            string wallet_reference = Convert.ToString(dtwallets.Rows[0]["Wallet_Ref_No"]);
                                                            double oldwalletbalance = Convert.ToDouble(dtwallets.Rows[0]["Total_Points"]);
                                                            double newwalletbalance = Math.Round(oldwalletbalance - benefit_points, MidpointRounding.AwayFromZero);

                                                            if (wallet_active == 0)
                                                            {
                                                                log += "inside if (wallet_active == 0),";
                                                                if (oldwalletbalance >= benefit_points)
                                                                {
                                                                    log += "inside if (oldwalletbalance >= benefit_points),";
                                                                    MySqlCommand _cmd4 = new MySqlCommand("update_wallet_points", con);
                                                                    _cmd4.CommandType = CommandType.StoredProcedure;
                                                                    _cmd4.Transaction = transaction;
                                                                    _cmd4.Parameters.AddWithValue("_Wallet_Ref_No", wallet_reference);
                                                                    _cmd4.Parameters.AddWithValue("_Total_Points", newwalletbalance);
                                                                    _cmd4.Parameters.AddWithValue("_Customer_ID", Customer_ID);
                                                                    _cmd4.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                                                                    //int ipw = db_connection.ExecuteNonQueryProcedure(_cmd4);
                                                                    int ipw = _cmd4.ExecuteNonQuery();

                                                                    if (ipw > 0)
                                                                    {
                                                                        log += "inside if (ipw > 0),";
                                                                        notification_icon = "points-debited.png";
                                                                        notification_message = "<span class='cls-admin'><strong class='cls-reward'>" + benefit_points + "</strong> Points debited from Customer's Points Wallet.</span><span class='cls-customer'><strong></strong><span>" + benefit_points + " Points debited from your Points Wallet.</span></span>";
                                                                        CompanyInfo.save_notification(notification_message, notification_icon, Customer_ID, Convert.ToDateTime(obj.Record_Insert_DateTime), obj.Client_ID, 1, 0, obj.Branch_ID, 1, 0, 1, 0, context);
                                                                        try
                                                                        {
                                                                            DataTable dt_notif = CompanyInfo.set_notification_data(72);
                                                                            if (dt_notif.Rows.Count > 0)
                                                                            {
                                                                                int SMS = Convert.ToInt32(dt_notif.Rows[0]["SMS"]);
                                                                                int Email1 = Convert.ToInt32(dt_notif.Rows[0]["Email"]);
                                                                                int Notif_status = Convert.ToInt32(dt_notif.Rows[0]["Notification"]);
                                                                                string notification_msg = Convert.ToString(dt_notif.Rows[0]["notification_msg"]);
                                                                                if (notification_msg.Contains("[points]") == true)
                                                                                {
                                                                                    notification_msg = notification_msg.Replace("[points]", Convert.ToString(benefit_points));
                                                                                }

                                                                                int i2 = CompanyInfo.check_notification_perm(Convert.ToString(Customer_ID), obj.Client_ID, obj.Branch_ID, 1, 72, Convert.ToDateTime(obj.Record_Insert_DateTime), 1, SMS, Email1, Notif_status, "App- Points Debited - 72", notification_msg, context);
                                                                            }
                                                                        }
                                                                        catch (Exception ex)
                                                                        {
                                                                        }

                                                                        string act = benefit_points + " Points Debited against KMoney Health Care for Beneficiary_ID= " + obj.Beneficiary_ID + ". New Wallet Points: " + newwalletbalance + ", Old Wallet Points: " + oldwalletbalance + " ";
                                                                        int sts = (int)CompanyInfo.InsertActivityLogDetails(act, obj.User_ID, 0, obj.User_ID, Customer_ID, "CreateMedicare", obj.Branch_ID, obj.Client_ID, "CreateMedicare", context);

                                                                        MySqlCommand _cmd = new MySqlCommand("save_beneficiary_medicare", con);
                                                                        _cmd.CommandType = CommandType.StoredProcedure;
                                                                        _cmd.Transaction = transaction;
                                                                        _cmd.Parameters.AddWithValue("_Customer_ID", Customer_ID);
                                                                        _cmd.Parameters.AddWithValue("_Beneficiary_ID", obj.Beneficiary_ID);
                                                                        _cmd.Parameters.AddWithValue("_Membership_ID", Membership_ID);
                                                                        _cmd.Parameters.AddWithValue("_Referral_Flag", 1);
                                                                        _cmd.Parameters.AddWithValue("_Temporary_Inactive", 1);
                                                                        _cmd.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                                                                        _cmd.Parameters.AddWithValue("_Next_Deduction_DateTime", Next_Deduction_Date);
                                                                        _cmd.Parameters.AddWithValue("_Medicare_Expiry_Date", obj.Medicare_Expiry_Date);
                                                                        _cmd.Parameters.AddWithValue("_Last_Updated_DateTime", obj.Record_Insert_DateTime);
                                                                        _cmd.Parameters.AddWithValue("_Record_Insert_DateTime", obj.Record_Insert_DateTime);
                                                                        _cmd.Parameters.AddWithValue("_Total_Members", total_members);
                                                                        _cmd.Parameters.AddWithValue("_Slots_For_Customer", 0);
                                                                        _cmd.Parameters.AddWithValue("_Person_Flag", 0);
                                                                        //dt = db_connection.ExecuteQueryDataTableProcedure(_cmd);
                                                                        MySqlDataAdapter da = new MySqlDataAdapter(_cmd);
                                                                        da.Fill(dt);

                                                                        MySqlCommand _cmd5 = new MySqlCommand("save_points_mapping", con);
                                                                        _cmd5.CommandType = CommandType.StoredProcedure;
                                                                        _cmd5.Transaction = transaction;
                                                                        _cmd5.Parameters.AddWithValue("_Points_Wallet_Id", points_wallet_id);
                                                                        _cmd5.Parameters.AddWithValue("_Customer_ID", Customer_ID);
                                                                        _cmd5.Parameters.AddWithValue("_Transfer_Type", "5");
                                                                        _cmd5.Parameters.AddWithValue("_Points", benefit_points);
                                                                        _cmd5.Parameters.AddWithValue("_Old_Balance", oldwalletbalance);
                                                                        _cmd5.Parameters.AddWithValue("_New_Balance", newwalletbalance);
                                                                        _cmd5.Parameters.AddWithValue("_Record_Insert_DateTime", obj.Record_Insert_DateTime);
                                                                        _cmd5.Parameters.AddWithValue("_User_ID", obj.User_ID);
                                                                        _cmd5.Parameters.AddWithValue("_Transaction_ID", 0);
                                                                        _cmd5.Parameters.AddWithValue("_Comments", "Debited against Health Care Added for Beneficiary: " + obj.Beneficiary_Name);
                                                                        _cmd5.Parameters.AddWithValue("_Benefit_ID", 1);
                                                                        _cmd5.Parameters.AddWithValue("_Client_ID", obj.Client_ID);

                                                                        _cmd5.Parameters.Add(new MySqlParameter("_points_mapping_id", MySqlDbType.Int32));
                                                                        _cmd5.Parameters["_points_mapping_id"].Direction = ParameterDirection.Output;
                                                                        //int pmt = db_connection.ExecuteNonQueryProcedure(_cmd5);
                                                                        int pmt = _cmd5.ExecuteNonQuery();
                                                                    }
                                                                    else
                                                                    {
                                                                        log += "inside else of if (ipw > 0),";
                                                                        obj.Message = "Failed";
                                                                        string act = "Failed to Debit " + benefit_points + " Points against KMoney Health Care for Beneficiary_ID= " + obj.Beneficiary_ID + ". Old Wallet Points: " + oldwalletbalance + " ";
                                                                        int sts = (int)CompanyInfo.InsertActivityLogDetails(act, obj.User_ID, 0, obj.User_ID, Customer_ID, "CreateMedicare", obj.Branch_ID, obj.Client_ID, "CreateMedicare", context);
                                                                        return obj;
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    log += "inside else of if (oldwalletbalance >= benefit_points),";
                                                                    obj.Message = "no_enough_points";

                                                                    string act = "No Enought Points. Failed to Debit " + benefit_points + " Points against KMoney Health Care for Beneficiary_ID= " + obj.Beneficiary_ID + ". Old Wallet Points: " + oldwalletbalance + " ";
                                                                    int sts = (int)CompanyInfo.InsertActivityLogDetails(act, obj.User_ID, 0, obj.User_ID, Customer_ID, "CreateMedicare", obj.Branch_ID, obj.Client_ID, "CreateMedicare", context);
                                                                    return obj;
                                                                }
                                                            }
                                                            else
                                                            {
                                                                log += "inside else of if (wallet_active == 0),";
                                                                obj.Message = "wallet_inactive";

                                                                string act = "Points wallet is inactive. Failed to Debit " + benefit_points + " Points against KMoney Health Care for Beneficiary_ID= " + obj.Beneficiary_ID + ". Old Wallet Points: " + oldwalletbalance + " ";
                                                                int sts = (int)CompanyInfo.InsertActivityLogDetails(act, obj.User_ID, 0, obj.User_ID, Customer_ID, "CreateMedicare", obj.Branch_ID, obj.Client_ID, "CreateMedicare", context);
                                                                return obj;
                                                            }
                                                        }
                                                    }
                                                }
                                                if (dt != null && dt.Rows.Count > 0)
                                                {

                                                    log += "inside if if (dt != null && dt.Rows.Count > 0),";
                                                    int RowsUpdated = 0; int RowsInserted = 0; string Status = "";
                                                    if (dt.Rows[0]["RowsUpdated"] != DBNull.Value)
                                                    {
                                                        RowsUpdated = Convert.ToInt32(dt.Rows[0]["RowsUpdated"]);
                                                    }
                                                    if (dt.Rows[0]["RowsInserted"] != DBNull.Value)
                                                    {
                                                        RowsInserted = Convert.ToInt32(dt.Rows[0]["RowsInserted"]);
                                                    }
                                                    if (dt.Rows[0]["Status"] != DBNull.Value)
                                                    {
                                                        Status = Convert.ToString(dt.Rows[0]["Status"]);
                                                    }

                                                    log += "Status=" + Status + ",";
                                                    if (Status == "success")
                                                    {
                                                        log += "inside if (Status == \"success\"),";
                                                        MySqlCommand cmd = new MySqlCommand("Update_Beneficiary_Medicare", con);
                                                        cmd.CommandType = CommandType.StoredProcedure;
                                                        cmd.Transaction = transaction;
                                                        cmd.Parameters.AddWithValue("_Benef_Medicare", obj.Medicare_Benef);
                                                        cmd.Parameters.AddWithValue("_Benef_address", obj.Beneficiary_Address);
                                                        cmd.Parameters.AddWithValue("_Benef_Phn_num", obj.Beneficiary_Telephone);
                                                        cmd.Parameters.AddWithValue("_Benef_email", obj.Beneficiary_Email);
                                                        cmd.Parameters.AddWithValue("_Beneficiary_Id", obj.Beneficiary_ID);
                                                        cmd.Parameters.AddWithValue("_Provience_id", obj.Provience_id);
                                                        cmd.Parameters.AddWithValue("_LGA_id", obj.LGA_Id);
                                                        cmd.Parameters.AddWithValue("_Uk_Address_Flag", obj.Address_Flag);
                                                        cmd.Parameters.AddWithValue("_Country_id", obj.Beneficiary_Country_ID);
                                                        cmd.Parameters.AddWithValue("_City_id", obj.Beneficiary_City_ID);
                                                        cmd.Parameters.AddWithValue("_Customer_ID", Customer_ID);
                                                        cmd.Parameters.AddWithValue("_Person_Flag", 0);
                                                        //int n1 = db_connection.ExecuteNonQueryProcedure(cmd);
                                                        int n1 = cmd.ExecuteNonQuery();
                                                        if (n1 > 0)
                                                        {
                                                            log += "inside if (n1 > 0),";
                                                            Activity = "Beneficiary added succesfully for medicare services.  </br>";
                                                            int stattus = (int)CompanyInfo.InsertActivityLogDetails(Activity, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(Customer_ID), "CreateMedicare", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "CreateMedicare", context);

                                                            obj.Message = "success";
                                                            transaction.Commit();

                                                            try
                                                            {
                                                                DataTable dt_notif = CompanyInfo.set_notification_data(64);
                                                                if (dt_notif.Rows.Count > 0)
                                                                {
                                                                    int SMS = Convert.ToInt32(dt_notif.Rows[0]["SMS"]);
                                                                    int Email1 = Convert.ToInt32(dt_notif.Rows[0]["Email"]);
                                                                    int Notif_status = Convert.ToInt32(dt_notif.Rows[0]["Notification"]);
                                                                    string notification_msg = Convert.ToString(dt_notif.Rows[0]["notification_msg"]);
                                                                    if (notification_msg.Contains("[benef_name]") == true)
                                                                    {
                                                                        notification_msg = notification_msg.Replace("[benef_name]", obj.Beneficiary_Name);
                                                                    }

                                                                    int ip = CompanyInfo.check_notification_perm(Convert.ToString(Customer_ID), obj.Client_ID, obj.Branch_ID, 1, 64, Convert.ToDateTime(obj.RecordDate), 1, SMS, Email1, Notif_status, "App - Beneficiary Added to Health Care - 64", notification_msg, context);
                                                                }

                                                            }
                                                            catch (Exception ex)
                                                            {

                                                            }
                                                            notification_icon = "health-care-added.png";
                                                            notification_message = "<span class='cls-admin'> Customer has added <strong class='cls-reward'>" + obj.Beneficiary_Name + "</strong> to KMoney Health Care successfully.</span><span class='cls-customer'>You have added <strong>" + obj.Beneficiary_Name + "</strong><span> to KMoney Health Care successfully.</span></span>";
                                                            CompanyInfo.save_notification(notification_message, notification_icon, Customer_ID, Convert.ToDateTime(obj.Record_Insert_DateTime), obj.Client_ID, 1, 0, obj.Branch_ID, 1, 0, 1, 0, context);


                                                            #region send mail to Customer for Add to Health Care Beneficiary
                                                            try
                                                            {
                                                                log += "inside send mail cust,";
                                                                string email = obj.Customer.Email;
                                                                if (email != "" && email != null)
                                                                {
                                                                    string Bene_Name = obj.Beneficiary_Name;
                                                                    string Cust_Name = obj.Customer.Name;

                                                                    string subject = string.Empty;
                                                                    string body = string.Empty;

                                                                    DataTable dtc = CompanyInfo.get(obj.Client_ID, context);
                                                                    string company_name = Convert.ToString(dtc.Rows[0]["Company_Name"]);
                                                                    string URL = Convert.ToString(dtc.Rows[0]["Company_URL_Admin"]);
                                                                    string cust_url = Convert.ToString(dtc.Rows[0]["Company_URL_Customer"]);

                                                                    HttpWebRequest httpRequest;
                                                                    httpRequest = (HttpWebRequest)WebRequest.Create(URL + "Email/health-care-active.html");

                                                                    httpRequest.UserAgent = "Code Sample Web Client";
                                                                    HttpWebResponse webResponse = (HttpWebResponse)httpRequest.GetResponse();
                                                                    using (StreamReader reader = new StreamReader(webResponse.GetResponseStream()))
                                                                    {
                                                                        body = reader.ReadToEnd();
                                                                    }

                                                                    body = body.Replace("[name]", Cust_Name);
                                                                    body = body.Replace("[beneficiary_name]", Bene_Name);
                                                                    body = body.Replace("[cust_url]", cust_url);


                                                                    DataTable d1 = (DataTable)CompanyInfo.getCustomerDetails(obj.Client_ID, Customer_ID);

                                                                    subject = company_name + " - Beneficiary Health Care Activated - " + d1.Rows[0]["WireTransfer_ReferanceNo"];

                                                                    string send_mail = (string)CompanyInfo.Send_Mail(dtc, email, body, subject, obj.Client_ID, obj.Branch_ID, "", "", "", context);

                                                                    string act = "Customer Healthcare Email send status = " + send_mail;
                                                                    int stattus1 = (int)CompanyInfo.InsertActivityLogDetails(act, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(Customer_ID), "CreateMedicare", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "CreateMedicare", context);
                                                                }
                                                            }
                                                            catch (Exception ex)
                                                            {
                                                                Model.ErrorLog objError = new Model.ErrorLog();
                                                                objError.User = new Model.User();

                                                                objError.Error = "srvBeneficiary : CreateMedicare -" + ex.ToString();
                                                                objError.Date = DateTime.Now;
                                                                objError.User.Id = 1;
                                                                objError.Id = 1;
                                                                objError.Function_Name = "CreateMedicare";
                                                                Service.srvErrorLog srvError = new Service.srvErrorLog();
                                                                srvError.Create(objError, context);
                                                            }
                                                            #endregion


                                                            #region send mail to Beneficiary for Added to Health Care
                                                            try
                                                            {
                                                                log += "inside send mail benef,";
                                                                if (obj.Beneficiary_Email != null && obj.Beneficiary_Email != "")
                                                                {
                                                                    string email = obj.Beneficiary_Email;
                                                                    string Bene_Name = obj.Beneficiary_Name;
                                                                    string Cust_Name = obj.Customer.Name;

                                                                    string subject = string.Empty;
                                                                    string body = string.Empty;

                                                                    DataTable dtc = CompanyInfo.get(obj.Client_ID, context);
                                                                    string company_name = Convert.ToString(dtc.Rows[0]["Company_Name"]);
                                                                    string URL = Convert.ToString(dtc.Rows[0]["Company_URL_Admin"]);
                                                                    string cust_url = Convert.ToString(dtc.Rows[0]["Company_URL_Customer"]);

                                                                    HttpWebRequest httpRequest;
                                                                    httpRequest = (HttpWebRequest)WebRequest.Create(URL + "Email/healthcare_benef_email.html");

                                                                    httpRequest.UserAgent = "Code Sample Web Client";
                                                                    HttpWebResponse webResponse = (HttpWebResponse)httpRequest.GetResponse();
                                                                    using (StreamReader reader = new StreamReader(webResponse.GetResponseStream()))
                                                                    {
                                                                        body = reader.ReadToEnd();
                                                                    }

                                                                    body = body.Replace("[customer_name]", Cust_Name);
                                                                    body = body.Replace("[beneficiary_name]", Bene_Name);
                                                                    body = body.Replace("[cust_url]", cust_url);


                                                                    DataTable d1 = (DataTable)CompanyInfo.getCustomerDetails(obj.Client_ID, Customer_ID);

                                                                    subject = company_name + " - Health Care Activated - " + d1.Rows[0]["WireTransfer_ReferanceNo"];

                                                                    string send_mail = (string)CompanyInfo.Send_Mail(dtc, email, body, subject, obj.Client_ID, obj.Branch_ID, "", "", "", context);

                                                                    string act = "Beneficiary Healthcare Email send status = " + send_mail;
                                                                    int stattus1 = (int)CompanyInfo.InsertActivityLogDetails(act, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(Customer_ID), "CreateMedicare", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "CreateMedicare", context);
                                                                }
                                                            }
                                                            catch (Exception ex)
                                                            {
                                                                Model.ErrorLog objError = new Model.ErrorLog();
                                                                objError.User = new Model.User();

                                                                objError.Error = "srvBeneficiary : CreateMedicare -" + ex.ToString();
                                                                objError.Date = DateTime.Now;
                                                                objError.User.Id = 1;
                                                                objError.Id = 1;
                                                                objError.Function_Name = "CreateMedicare";
                                                                Service.srvErrorLog srvError = new Service.srvErrorLog();
                                                                srvError.Create(objError, context);
                                                            }
                                                            #endregion
                                                        }
                                                        else
                                                        {
                                                            log += "inside else of if (n1 > 0),";
                                                            obj.Message = "Failed";
                                                            Activity = " Failed to save Beneficiary Healthcare details.  </br>";
                                                            int stattus = (int)CompanyInfo.InsertActivityLogDetails(Activity, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(Customer_ID), "CreateMedicare", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "CreateMedicare", context);
                                                        }
                                                    }
                                                    else if (Status == "already_exist")
                                                    {
                                                        obj.Message = "already_exist";
                                                        Activity = " Beneficiary already exist for medicare service.  </br>";
                                                        int stattus = (int)CompanyInfo.InsertActivityLogDetails(Activity, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(Customer_ID), "CreateMedicare", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "CreateMedicare", context);
                                                    }
                                                    else if (Status == "limit_exceed")
                                                    {
                                                        obj.Message = "limit_exceed";
                                                        Activity = " Beneficiary Limit exceed for medicare services.  </br>";
                                                        int stattus = (int)CompanyInfo.InsertActivityLogDetails(Activity, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(Customer_ID), "CreateMedicare", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "CreateMedicare", context);
                                                    }
                                                    else
                                                    {
                                                        obj.Message = "Failed";
                                                        Activity = " Failed to add Beneficiary to medicare services.  </br>";
                                                        int stattus = (int)CompanyInfo.InsertActivityLogDetails(Activity, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(Customer_ID), "CreateMedicare", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "CreateMedicare", context);
                                                    }
                                                }
                                                else if (obj.Message == "" || obj.Message == null)
                                                {
                                                    obj.Message = "Failed";
                                                    Activity = " Failed to add Beneficiary to medicare services.  </br>";
                                                    int stattus = (int)CompanyInfo.InsertActivityLogDetails(Activity, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(Customer_ID), "CreateMedicare", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "CreateMedicare", context);
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                log += "exception=" + ex.ToString() + ",";
                                                transaction.Rollback();
                                                throw ex;
                                            }
                                            finally
                                            {
                                                con.Close();
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    log += "inside else of if (dtm == null ||,";
                                    obj.Message = "duplicate";
                                    Activity = " Beneficiary with mobile number (" + obj.Beneficiary_Telephone + ") already exist for medicare service.</br>";
                                    int stattus = (int)CompanyInfo.InsertActivityLogDetails(Activity, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(Customer_ID), "CreateMedicare", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "CreateMedicare", context);
                                }
                                #endregion add beneficiary to health care
                            }
                            else  //add customer to health care
                            {
                                #region add customer to health care
                                log += "inside else of if (obj.Beneficiary_ID != 0),";
                                DateTime Next_Deduction_Date = new DateTime();

                                obj.Medicare_Expiry_Date = obj.Record_Insert_DateTime.AddYears(1);
                                string whereclause = " and cmmt.Customer_ID=" + Customer_ID + " and cmmt.Client_ID=" + obj.Client_ID + " and mbmt.Benefit_ID=1";
                                MySqlCommand _cmds = new MySqlCommand("get_customer_membership_mapping");
                                _cmds.CommandType = CommandType.StoredProcedure;
                                _cmds.Parameters.AddWithValue("_Whereclause", whereclause);
                                DataTable dt_data = db_connection.ExecuteQueryDataTableProcedure(_cmds);

                                if (dt_data != null && dt_data.Rows.Count > 0)
                                {
                                    log += "inside if (dt_data != null &&,";
                                    double benefit_points = Convert.ToDouble(dt_data.Rows[0]["Points"]);
                                    int total_members = Convert.ToInt32(dt_data.Rows[0]["Total_Members"]);
                                    int Slots_For_Customer = Convert.ToInt32(dt_data.Rows[0]["Slots_For_Customer"]);
                                    int Membership_ID = Convert.ToInt32(dt_data.Rows[0]["Membership_ID"]);

                                    if (Slots_For_Customer > 0)
                                    {
                                        log += "inside if (Slots_For_Customer > 0),";
                                        if (dt_data.Rows[0]["Validity_In_Days"] != null)
                                        {
                                            obj.Medicare_Expiry_Date = obj.Record_Insert_DateTime.AddDays((Convert.ToInt32(dt_data.Rows[0]["Validity_In_Days"]) + 1));
                                            Next_Deduction_Date = obj.Record_Insert_DateTime.AddDays((Convert.ToInt32(dt_data.Rows[0]["Validity_In_Days"]) + 2)); //next day after expiry
                                        }

                                        MySqlCommand _cmd = new MySqlCommand("save_beneficiary_medicare"); //, con
                                        _cmd.CommandType = CommandType.StoredProcedure;
                                        _cmd.Parameters.AddWithValue("_Customer_ID", Customer_ID);
                                        _cmd.Parameters.AddWithValue("_Beneficiary_ID", obj.Beneficiary_ID);
                                        _cmd.Parameters.AddWithValue("_Membership_ID", Membership_ID);
                                        _cmd.Parameters.AddWithValue("_Referral_Flag", 1);
                                        _cmd.Parameters.AddWithValue("_Temporary_Inactive", 1);
                                        _cmd.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                                        _cmd.Parameters.AddWithValue("_Next_Deduction_DateTime", Next_Deduction_Date);
                                        _cmd.Parameters.AddWithValue("_Medicare_Expiry_Date", obj.Medicare_Expiry_Date);
                                        _cmd.Parameters.AddWithValue("_Last_Updated_DateTime", obj.Record_Insert_DateTime);
                                        _cmd.Parameters.AddWithValue("_Record_Insert_DateTime", obj.Record_Insert_DateTime);
                                        _cmd.Parameters.AddWithValue("_Total_Members", total_members);
                                        _cmd.Parameters.AddWithValue("_Slots_For_Customer", Slots_For_Customer);
                                        _cmd.Parameters.AddWithValue("_Person_Flag", 1);
                                        DataTable dt = db_connection.ExecuteQueryDataTableProcedure(_cmd);
                                        //MySqlDataAdapter da = new MySqlDataAdapter(_cmd);
                                        //da.Fill(dt);

                                        if (dt != null && dt.Rows.Count > 0)
                                        {
                                            log += "inside if (dt != null && dt.Rows.Count > 0),";
                                            int RowsUpdated = 0; int RowsInserted = 0; string Status = "";
                                            if (dt.Rows[0]["RowsUpdated"] != DBNull.Value)
                                            {
                                                RowsUpdated = Convert.ToInt32(dt.Rows[0]["RowsUpdated"]);
                                            }
                                            if (dt.Rows[0]["RowsInserted"] != DBNull.Value)
                                            {
                                                RowsInserted = Convert.ToInt32(dt.Rows[0]["RowsInserted"]);
                                            }
                                            if (dt.Rows[0]["Status"] != DBNull.Value)
                                            {
                                                Status = Convert.ToString(dt.Rows[0]["Status"]);
                                            }

                                            log += "Status=" + Status + ",";
                                            if (Status == "success")
                                            {
                                                log += "inside if (Status == \"success\"),";
                                                if (obj.Address_Flag == 1)
                                                {
                                                    obj.Beneficiary_Country_ID = 0;
                                                    obj.Beneficiary_City_ID = 0;
                                                }
                                                else
                                                {
                                                    obj.Provience_id = 0;
                                                    obj.LGA_Id = 0;
                                                }
                                                MySqlCommand cmd = new MySqlCommand("Update_Beneficiary_Medicare"); //, con
                                                cmd.CommandType = CommandType.StoredProcedure;
                                                //cmd.Parameters.AddWithValue("_Benef_Medicare", obj.Medicare_Benef); //removed from SP
                                                cmd.Parameters.AddWithValue("_Benef_address", obj.Beneficiary_Address);
                                                cmd.Parameters.AddWithValue("_Benef_Phn_num", obj.Beneficiary_Telephone);
                                                cmd.Parameters.AddWithValue("_Benef_email", obj.Beneficiary_Email);
                                                cmd.Parameters.AddWithValue("_Beneficiary_Id", obj.Beneficiary_ID);
                                                cmd.Parameters.AddWithValue("_Provience_id", obj.Provience_id);
                                                cmd.Parameters.AddWithValue("_LGA_id", obj.LGA_Id);
                                                cmd.Parameters.AddWithValue("_Uk_Address_Flag", obj.Address_Flag);
                                                cmd.Parameters.AddWithValue("_Country_id", obj.Beneficiary_Country_ID);
                                                cmd.Parameters.AddWithValue("_City_id", obj.Beneficiary_City_ID);
                                                cmd.Parameters.AddWithValue("_Customer_ID", Customer_ID);
                                                cmd.Parameters.AddWithValue("_Person_Flag", 1);
                                                int n1 = db_connection.ExecuteNonQueryProcedure(cmd);
                                                //int n1 = cmd.ExecuteNonQuery();

                                                if (n1 > 0)
                                                {
                                                    log += "inside if (n1 > 0),";
                                                    Activity = "Beneficiary added succesfully for medicare services.  </br>";
                                                    int stattus = (int)CompanyInfo.InsertActivityLogDetails(Activity, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(Customer_ID), "CreateMedicare", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "CreateMedicare", context);

                                                    obj.Message = "success";

                                                    try
                                                    {
                                                        DataTable dt_notif = CompanyInfo.set_notification_data(64);
                                                        if (dt_notif.Rows.Count > 0)
                                                        {
                                                            int SMS = Convert.ToInt32(dt_notif.Rows[0]["SMS"]);
                                                            int Email1 = Convert.ToInt32(dt_notif.Rows[0]["Email"]);
                                                            int Notif_status = Convert.ToInt32(dt_notif.Rows[0]["Notification"]);
                                                            string notification_msg = Convert.ToString(dt_notif.Rows[0]["notification_msg"]);
                                                            if (notification_msg.Contains("[benef_name]") == true)
                                                            {
                                                                notification_msg = notification_msg.Replace("[benef_name]", obj.Beneficiary_Name);
                                                            }

                                                            int ip = CompanyInfo.check_notification_perm(Convert.ToString(Customer_ID), obj.Client_ID, obj.Branch_ID, 1, 64, Convert.ToDateTime(obj.RecordDate), 1, SMS, Email1, Notif_status, "App - Beneficiary Added to Health Care - 64", notification_msg, context);
                                                        }

                                                    }
                                                    catch (Exception ex)
                                                    {

                                                    }
                                                    notification_icon = "health-care-added.png";
                                                    notification_message = "<span class='cls-admin'> Customer has added himself to KMoney Health Care successfully.</span><span class='cls-customer'>You have added yourself to KMoney Health Care successfully.</span></span>";
                                                    CompanyInfo.save_notification(notification_message, notification_icon, Customer_ID, Convert.ToDateTime(obj.Record_Insert_DateTime), obj.Client_ID, 1, 0, obj.Branch_ID, 1, 0, 1, 0, context);

                                                    #region send mail to Customer for Added Himself to Health Care
                                                    try
                                                    {
                                                        log += "inside send mail cust self,";
                                                        string email = obj.Customer.Email;
                                                        if (email != "" && email != null)
                                                        {
                                                            string Cust_Name = obj.Customer.Name;

                                                            string subject = string.Empty;
                                                            string body = string.Empty;

                                                            DataTable dtc = CompanyInfo.get(obj.Client_ID, context);
                                                            string company_name = Convert.ToString(dtc.Rows[0]["Company_Name"]);
                                                            string URL = Convert.ToString(dtc.Rows[0]["Company_URL_Admin"]);
                                                            string cust_url = Convert.ToString(dtc.Rows[0]["Company_URL_Customer"]);

                                                            HttpWebRequest httpRequest;
                                                            httpRequest = (HttpWebRequest)WebRequest.Create(URL + "Email/healthcare_customer_email.html");

                                                            httpRequest.UserAgent = "Code Sample Web Client";
                                                            HttpWebResponse webResponse = (HttpWebResponse)httpRequest.GetResponse();
                                                            using (StreamReader reader = new StreamReader(webResponse.GetResponseStream()))
                                                            {
                                                                body = reader.ReadToEnd();
                                                            }

                                                            body = body.Replace("[customer_name]", Cust_Name);
                                                            body = body.Replace("[cust_url]", cust_url);


                                                            DataTable d1 = (DataTable)CompanyInfo.getCustomerDetails(obj.Client_ID, Customer_ID);

                                                            subject = company_name + " - Health Care Activated - " + d1.Rows[0]["WireTransfer_ReferanceNo"];

                                                            string send_mail = (string)CompanyInfo.Send_Mail(dtc, email, body, subject, obj.Client_ID, obj.Branch_ID, "", "", "", context);

                                                            string act = "Customer Self Healthcare Email send status = " + send_mail;
                                                            int stattus1 = (int)CompanyInfo.InsertActivityLogDetails(act, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(Customer_ID), "CreateMedicare", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "CreateMedicare", context);
                                                        }
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        Model.ErrorLog objError = new Model.ErrorLog();
                                                        objError.User = new Model.User();

                                                        objError.Error = "srvBeneficiary : CreateMedicare -" + ex.ToString();
                                                        objError.Date = DateTime.Now;
                                                        objError.User.Id = 1;
                                                        objError.Id = 1;
                                                        objError.Function_Name = "CreateMedicare";
                                                        Service.srvErrorLog srvError = new Service.srvErrorLog();
                                                        srvError.Create(objError, context);
                                                    }
                                                    #endregion
                                                }
                                                else
                                                {
                                                    log += "inside else of if (n1 > 0),";
                                                    obj.Message = "Failed";
                                                    Activity = " Failed to save Customer Healthcare details.  </br>";
                                                    int stattus = (int)CompanyInfo.InsertActivityLogDetails(Activity, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(Customer_ID), "CreateMedicare", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "CreateMedicare", context);
                                                }
                                            }
                                            else if (Status == "already_exist")
                                            {
                                                obj.Message = "already_exist";
                                                Activity = " Customer already exist for medicare service.  </br>";
                                                int stattus = (int)CompanyInfo.InsertActivityLogDetails(Activity, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(Customer_ID), "CreateMedicare", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "CreateMedicare", context);
                                            }
                                            else if (Status == "limit_exceed")
                                            {
                                                obj.Message = "limit_exceed";
                                                Activity = " Customer Limit exceed for medicare services.  </br>";
                                                int stattus = (int)CompanyInfo.InsertActivityLogDetails(Activity, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(Customer_ID), "CreateMedicare", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "CreateMedicare", context);
                                            }
                                            else
                                            {
                                                obj.Message = "Failed";
                                                Activity = " Failed to add Customer to medicare services.  </br>";
                                                int stattus = (int)CompanyInfo.InsertActivityLogDetails(Activity, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(Customer_ID), "CreateMedicare", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "CreateMedicare", context);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        log += "inside else of if (Slots_For_Customer > 0),";
                                        obj.Message = "limit_exceed";
                                        Activity = " No slot available for Customer medicare services.  </br>";
                                        int stattus = (int)CompanyInfo.InsertActivityLogDetails(Activity, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(Customer_ID), "CreateMedicare", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "CreateMedicare", context);
                                    }
                                }
                                #endregion add customer to health care
                            }
                        }
                        catch (Exception _x)
                        {
                            log += "inside exception=" + _x.ToString() + ",";
                            throw _x;
                        }
                        finally
                        {
                            log += "inside finally,";
                            int stattus = (int)CompanyInfo.InsertActivityLogDetails(log, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(Customer_ID), "CreateMedicare", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "CreateMedicare", context);
                        }
                    }
                    else
                    {
                        string msg = "SQl Enjection detected";
                        int stattus = (int)CompanyInfo.InsertActivityLogDetailsSecurity(msg, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), 1, "CreateMedicare", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "CreateMedicare", 0, context);

                    }
                }
                else
                {
                    obj.Id = 0;
                    string msg = "Validation Failed <br/> " + error_invalid_data;
                    obj.Message = "Validation Failed";
                    int stattus = (int)CompanyInfo.InsertActivityLogDetails(msg, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), 1, "CreateMedicare", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "CreateMedicare", context);

                }
            }
            catch (Exception ex)
            {
                string msg = "Api CreateMedicare: " + ex.ToString() + " ";
                CompanyInfo.InsertErrorLogTracker(msg, 0, 0, 0, 0, "CreateMedicare", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "CreateMedicare", context);
            }
            return obj;
        }

        public Model.Beneficiary RemoveMedicare(Model.Beneficiary obj, HttpContext context)
        {
            try
            {
                int Customer_ID = Convert.ToInt32(CompanyInfo.Decrypt(Convert.ToString(obj.Customer_ID), true));
                string Activity = string.Empty;

                obj.Record_Insert_DateTime = Convert.ToDateTime(CompanyInfo.gettime(obj.Client_ID, context));

                string where = "Beneficiary_ID=" + obj.Beneficiary_ID + " and Customer_ID=" + Customer_ID + " and Client_ID=" + obj.Client_ID;
                MySqlCommand cmd = new MySqlCommand("remove_medicare");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("_Whereclause", where);
                cmd.Parameters.AddWithValue("_Last_Updated_DateTime", obj.Record_Insert_DateTime);
                int ur = db_connection.ExecuteNonQueryProcedure(cmd);

                if (ur > 0)
                {
                    obj.Message = "success";
                    string act = "KMoney Health Care Deactivated for Beneficiary_ID= " + obj.Beneficiary_ID + ".";
                    int sts = (int)CompanyInfo.InsertActivityLogDetails(act, obj.User_ID, 0, obj.User_ID, Customer_ID, "RemoveMedicare", obj.Branch_ID, obj.Client_ID, "RemoveMedicare", context);

                    try
                    {
                        DataTable dt_notif = CompanyInfo.set_notification_data(69);
                        if (dt_notif.Rows.Count > 0)
                        {
                            int SMS = Convert.ToInt32(dt_notif.Rows[0]["SMS"]);
                            int Email1 = Convert.ToInt32(dt_notif.Rows[0]["Email"]);
                            int Notif_status = Convert.ToInt32(dt_notif.Rows[0]["Notification"]);
                            string notification_msg = Convert.ToString(dt_notif.Rows[0]["notification_msg"]);
                            if (notification_msg.Contains("[benef_name]") == true)
                            {
                                notification_msg = notification_msg.Replace("[benef_name]", obj.Beneficiary_Name);
                            }

                            int ip = CompanyInfo.check_notification_perm(Convert.ToString(Customer_ID), obj.Client_ID, obj.Branch_ID, 1, 69, Convert.ToDateTime(obj.RecordDate), 1, SMS, Email1, Notif_status, "App - Health Care removed - 69", notification_msg, context);
                        }
                    }
                    catch (Exception ex)
                    {

                    }

                    string notification_icon = "health-care-removed.png";
                    string notification_message = "<span class='cls-admin'> Customer has removed <strong class='cls-reward'>" + obj.Beneficiary_Name + "</strong> from KMoney Health Care successfully.</span><span class='cls-customer'>You have removed <strong>" + obj.Beneficiary_Name + "</strong><span> from KMoney Health Care successfully.</span></span>";
                    CompanyInfo.save_notification(notification_message, notification_icon, Customer_ID, Convert.ToDateTime(obj.Record_Insert_DateTime), obj.Client_ID, 1, 0, obj.Branch_ID, 1, 0, 1, 0, context);
                    //revert perk before 48 hrs
                    #region revert perk before 48 hrs
                    string whereclause1 = " and mt.Beneficiary_ID=" + obj.Beneficiary_ID + " and mt.Customer_ID =" + Customer_ID + " and mt.Client_ID =" + obj.Client_ID;
                    MySqlCommand _cmdm = new MySqlCommand("get_medicare_table");
                    _cmdm.CommandType = CommandType.StoredProcedure;
                    _cmdm.Parameters.AddWithValue("_Whereclause", whereclause1);
                    DataTable dtmed = db_connection.ExecuteQueryDataTableProcedure(_cmdm);
                    if (dtmed != null && dtmed.Rows.Count > 0)
                    {
                        DateTime active_date_limit = Convert.ToDateTime(dtmed.Rows[0]["Medicare_Activation_DateTime"]).AddHours(48);
                        int referral_flag = Convert.ToInt32(dtmed.Rows[0]["Referral_Flag"]);

                        if (active_date_limit > obj.Record_Insert_DateTime)// check 48 hrs
                        {
                            if (referral_flag == 0)//referral perk revert
                            {
                                string where_clause = " Customer_ID=" + Customer_ID + " and Client_ID=" + obj.Client_ID;
                                MySqlCommand _cmda = new MySqlCommand("get_referral_count");
                                _cmda.CommandType = CommandType.StoredProcedure;
                                _cmda.Parameters.AddWithValue("_Whereclause", where_clause);
                                DataTable dt_ref = db_connection.ExecuteQueryDataTableProcedure(_cmda);

                                if (dt_ref != null && dt_ref.Rows.Count > 0)
                                {
                                    string where1 = " Customer_ID=" + Customer_ID + " and Client_ID=" + obj.Client_ID;
                                    MySqlCommand cmd1 = new MySqlCommand("update_referral_medicare_count");
                                    cmd1.CommandType = CommandType.StoredProcedure;
                                    cmd1.Parameters.AddWithValue("_referral_medicare_count", "+1");
                                    cmd1.Parameters.AddWithValue("_referral_medicare_days", dt_ref.Rows[0]["referral_medicare_days"]);
                                    cmd1.Parameters.AddWithValue("_whereclause", where1);
                                    int urm = db_connection.ExecuteNonQueryProcedure(cmd1);

                                    if (urm > 0)
                                    {
                                        string Activity1 = "Reverted 1 beneficiary to Health Care count after removal for Customer_ID=" + Customer_ID + " and Beneficiary_ID=" + obj.Beneficiary_ID;
                                        int stattus1 = (int)CompanyInfo.InsertActivityLogDetails(Activity1, obj.User_ID, 0, obj.User_ID, Customer_ID, "RemoveMedicare", obj.Branch_ID, obj.Client_ID, "RemoveMedicare", context);
                                    }
                                    else
                                    {
                                        string Activity1 = "Failed to revert 1 beneficiary to Health Care count after removal for Customer_ID=" + Customer_ID + " and Beneficiary_ID=" + obj.Beneficiary_ID;
                                        int stattus1 = (int)CompanyInfo.InsertActivityLogDetails(Activity1, obj.User_ID, 0, obj.User_ID, Customer_ID, "RemoveMedicare", obj.Branch_ID, obj.Client_ID, "RemoveMedicare", context);
                                    }
                                }
                            }
                            else          //membership perk revert
                            {
                                string whereclause = " and cmmt.Customer_ID=" + Customer_ID + " and cmmt.Client_ID=" + obj.Client_ID + " and mbmt.Benefit_ID=1";
                                MySqlCommand _cmds = new MySqlCommand("get_customer_membership_mapping");
                                _cmds.CommandType = CommandType.StoredProcedure;
                                _cmds.Parameters.AddWithValue("_Whereclause", whereclause);
                                DataTable dt_data = db_connection.ExecuteQueryDataTableProcedure(_cmds);

                                if (dt_data != null && dt_data.Rows.Count > 0)
                                {
                                    double benefit_points = Convert.ToDouble(dt_data.Rows[0]["Points"]);
                                    int total_members = Convert.ToInt32(dt_data.Rows[0]["Total_Members"]);
                                    int Membership_ID = Convert.ToInt32(dt_data.Rows[0]["Membership_ID"]);

                                    MySqlCommand _cmd1 = new MySqlCommand("get_points_wallet_details");
                                    _cmd1.CommandType = CommandType.StoredProcedure;
                                    _cmd1.Parameters.AddWithValue("_Customer_ID", Customer_ID);
                                    _cmd1.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                                    DataTable dtwallets = db_connection.ExecuteQueryDataTableProcedure(_cmd1);

                                    if (dtwallets != null && dtwallets.Rows.Count > 0)
                                    {
                                        int points_wallet_id = Convert.ToInt32(dtwallets.Rows[0]["Points_Wallet_ID"]);
                                        string wallet_reference = Convert.ToString(dtwallets.Rows[0]["Wallet_Ref_No"]);
                                        double oldwalletbalance = Convert.ToDouble(dtwallets.Rows[0]["Total_Points"]);
                                        double newwalletbalance = Math.Round(oldwalletbalance + benefit_points, MidpointRounding.AwayFromZero);

                                        MySqlCommand _cmd4 = new MySqlCommand("update_wallet_points");
                                        _cmd4.CommandType = CommandType.StoredProcedure;
                                        _cmd4.Parameters.AddWithValue("_Wallet_Ref_No", wallet_reference);
                                        _cmd4.Parameters.AddWithValue("_Total_Points", newwalletbalance);
                                        _cmd4.Parameters.AddWithValue("_Customer_ID", Customer_ID);
                                        _cmd4.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                                        int ipw = db_connection.ExecuteNonQueryProcedure(_cmd4);

                                        if (ipw > 0)
                                        {
                                            string act1 = benefit_points + " Points Reverted against removal from KMoney Health Care for Beneficiary_ID= " + obj.Beneficiary_ID + ". New Wallet Points: " + newwalletbalance + ", Old Wallet Points: " + oldwalletbalance + " ";
                                            int sts1 = (int)CompanyInfo.InsertActivityLogDetails(act, obj.User_ID, 0, obj.User_ID, Customer_ID, "RemoveMedicare", obj.Branch_ID, obj.Client_ID, "RemoveMedicare", context);

                                            MySqlCommand _cmd5 = new MySqlCommand("save_points_mapping");
                                            _cmd5.CommandType = CommandType.StoredProcedure;
                                            _cmd5.Parameters.AddWithValue("_Points_Wallet_Id", points_wallet_id);
                                            _cmd5.Parameters.AddWithValue("_Customer_ID", Customer_ID);
                                            _cmd5.Parameters.AddWithValue("_Transfer_Type", "4");
                                            _cmd5.Parameters.AddWithValue("_Points", benefit_points);
                                            _cmd5.Parameters.AddWithValue("_Old_Balance", oldwalletbalance);
                                            _cmd5.Parameters.AddWithValue("_New_Balance", newwalletbalance);
                                            _cmd5.Parameters.AddWithValue("_Record_Insert_DateTime", obj.Record_Insert_DateTime);
                                            _cmd5.Parameters.AddWithValue("_User_ID", obj.User_ID);
                                            _cmd5.Parameters.AddWithValue("_Transaction_ID", 0);
                                            _cmd5.Parameters.AddWithValue("_Comments", "Credited against Health Care Removed for Beneficiary '" + obj.Beneficiary_Name + "'");
                                            _cmd5.Parameters.AddWithValue("_Benefit_ID", 1);
                                            _cmd5.Parameters.AddWithValue("_Client_ID", obj.Client_ID);

                                            _cmd5.Parameters.Add(new MySqlParameter("_points_mapping_id", MySqlDbType.Int32));
                                            _cmd5.Parameters["_points_mapping_id"].Direction = ParameterDirection.Output;
                                            int pmt = db_connection.ExecuteNonQueryProcedure(_cmd5);

                                            try
                                            {
                                                DataTable dt_notif = CompanyInfo.set_notification_data(70);
                                                if (dt_notif.Rows.Count > 0)
                                                {
                                                    int SMS = Convert.ToInt32(dt_notif.Rows[0]["SMS"]);
                                                    int Email1 = Convert.ToInt32(dt_notif.Rows[0]["Email"]);
                                                    int Notif_status = Convert.ToInt32(dt_notif.Rows[0]["Notification"]);
                                                    string notification_msg = Convert.ToString(dt_notif.Rows[0]["notification_msg"]);
                                                    if (notification_msg.Contains("[benefit_points]") == true)
                                                    {
                                                        notification_msg = notification_msg.Replace("[benefit_points]", benefit_points.ToString());
                                                    }

                                                    int ip = CompanyInfo.check_notification_perm(Convert.ToString(Customer_ID), obj.Client_ID, obj.Branch_ID, 1, 70, Convert.ToDateTime(obj.RecordDate), 1, SMS, Email1, Notif_status, "App - Points perk revert - 70", notification_msg, context);
                                                }
                                            }
                                            catch (Exception ex)
                                            {

                                            }

                                            string notification_icon1 = "points-credited.png";
                                            string notification_message1 = "<span class='cls-admin'><strong class='cls-reward'>" + benefit_points + " Points</strong> reverted to customer's Points Wallet successfully.</span><span class='cls-customer'><strong>" + benefit_points + " Points</strong><span> reverted to your Points Wallet successfully.</span></span>";
                                            CompanyInfo.save_notification(notification_message1, notification_icon1, Customer_ID, Convert.ToDateTime(obj.Record_Insert_DateTime), obj.Client_ID, 1, 0, obj.Branch_ID, 1, 0, 1, 0, context);
                                        }
                                        else
                                        {
                                            //obj.Message = "Failed";
                                            string act1 = "Failed to Revert " + benefit_points + " Points against removal from KMoney Health Care for Beneficiary_ID= " + obj.Beneficiary_ID + ". Old Wallet Points: " + oldwalletbalance + " ";
                                            int sts1 = (int)CompanyInfo.InsertActivityLogDetails(act, obj.User_ID, 0, obj.User_ID, Customer_ID, "RemoveMedicare", obj.Branch_ID, obj.Client_ID, "RemoveMedicare", context);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    #endregion
                }
                else
                {
                    obj.Message = "failed";
                    string act = "Failed to Deactivate KMoney Health Care for Beneficiary_ID= " + obj.Beneficiary_ID + ".";
                    int sts = (int)CompanyInfo.InsertActivityLogDetails(act, obj.User_ID, 0, obj.User_ID, Customer_ID, "RemoveMedicare", obj.Branch_ID, obj.Client_ID, "RemoveMedicare", context);
                }

            }
            catch (Exception ex)
            {
                Model.ErrorLog objError = new Model.ErrorLog();
                objError.User = new Model.User();

                objError.Error = "srvBeneficiary : RemoveMedicare --" + ex.ToString();
                objError.Date = DateTime.Now;
                objError.User.Id = 1;
                objError.Id = 1;
                objError.Function_Name = "RemoveMedicare";
                Service.srvErrorLog srvError = new Service.srvErrorLog();
                srvError.Create(objError, context);
            }
            return obj;
        }

        public Model.Beneficiary AddBenfBankAccount(Model.Beneficiary obj, HttpContext context)
        {
            string activity_benf = "";
            activity_benf = activity_benf + "Beneficiary Save Bank Start";
            string[] Alert_Msg = new string[7];
            string Activity = string.Empty;
            // var context = System.Web.HttpContext.Current;
            string error_msg = ""; string error_invalid_data = "";
            //string Username = Convert.ToString(context.Request.Form["Username"]);

            int Customer_ID = Convert.ToInt32(CompanyInfo.Decrypt(obj.Customer_ID, true));

            string AccountHolderName_regex = validation.validate_accnm(obj.AccountHolderName, 0);
            string Account_Number_regex = validation.validate(obj.Account_Number, 1, 1, 0, 1, 1, 1, 1, 1, 1);
            string BankCode_regex = validation.validate(obj.BankCode, 1, 1, 0, 1, 1, 1, 1, 1, 1);
            string Branch_regex = validation.validate(obj.Branch, 1, 1, 1, 1, 1, 1, 1, 1, 0);
            string Ifsc_Code_regex = validation.validate(obj.Ifsc_Code, 1, 1, 0, 1, 1, 1, 1, 1, 1);
            string BranchCode_regex = validation.validate(obj.BranchCode, 1, 1, 0, 1, 1, 1, 1, 1, 1);
            string Benf_Iban_regex = validation.validate(obj.Benf_Iban, 1, 1, 0, 1, 1, 1, 1, 1, 1);
            string Benf_BIC_regex = validation.validate(obj.Benf_BIC, 1, 1, 0, 1, 1, 1, 1, 1, 1);
            string Beneficiary_Country_ID_regex = validation.validate(Convert.ToString(obj.Beneficiary_Country_ID), 1, 1, 0, 1, 1, 1, 1, 1, 1); //Parvej Added


            if (Beneficiary_Country_ID_regex == "false")
            {
                error_msg = error_msg + "Invalid Benficiary Coutry ID";
            }
            if (Benf_BIC_regex == "false")
            {
                error_msg = error_msg + "BIC code should be alpanumeric without space.";
                error_invalid_data = error_invalid_data + " BIC code: " + obj.Benf_BIC;
            }
            if (BankCode_regex == "false")
            {
                error_msg = error_msg + "Bank code should be alpanumeric without space.";
                error_invalid_data = error_invalid_data + " BankCode: " + obj.BankCode;
            }
            if (Branch_regex == "false")
            {
                error_msg = error_msg + "Branch should be alphanumeric with space and special characters like , , , . , ' , ( , ) , { , } , - , @ , & ";
                error_invalid_data = error_invalid_data + " Branch: " + obj.Branch;
            }
            if (Branch_regex == "false")
            {
                error_msg = error_msg + "Branch code should be alphanumeric without space.";
                error_invalid_data = error_invalid_data + " Branch: " + obj.BranchCode;
            }
            if (Ifsc_Code_regex == "false")
            {
                error_msg = error_msg + "IFSC code should be alphanumeric without space.";
                error_invalid_data = error_invalid_data + " Branch: " + obj.Ifsc_Code;
            }
            if (Benf_Iban_regex == "false")
            {
                error_msg = error_msg + "IBAN code should be alphanumeric without space.";
                error_invalid_data = error_invalid_data + " IBAN: " + obj.Benf_Iban;
            }
            if (Account_Number_regex == "false")
            {
                error_msg = error_msg + "Account Number should be alphanumeric only without space.";
                error_invalid_data = error_invalid_data + " Account_Number: " + obj.Account_Number;
            }
            if (Beneficiary_Country_ID_regex == "" && Benf_BIC_regex == "" && Benf_Iban_regex == "" && BranchCode_regex == "" && Ifsc_Code_regex == "" && Branch_regex == "" && AccountHolderName_regex == "" && BankCode_regex == "" && Account_Number_regex == "" && Benf_Iban_regex == "")
            {
                BranchCode_regex = "true"; Ifsc_Code_regex = "true"; Branch_regex = "true"; AccountHolderName_regex = "true"; BankCode_regex = "true";

                Account_Number_regex = "true";
                Benf_Iban_regex = "true";
                Benf_BIC_regex = "true";
                BranchCode_regex = "true";
                Beneficiary_Country_ID_regex = "true"; //PArvej Added

            }
            if (Beneficiary_Country_ID_regex != "false" && Benf_BIC_regex != "false" && Benf_Iban_regex != "false" && BranchCode_regex != "false" && Ifsc_Code_regex != "false" && Branch_regex != "false" && AccountHolderName_regex != "false" && BankCode_regex != "false" && Account_Number_regex != "false" && Benf_Iban_regex != "false")
            {
                mts_connection _MTS = new mts_connection();

                using (MySqlConnection con = new MySqlConnection(_MTS.WebConnSetting()))
                {
                    con.Open();
                    string Beneficiary_Name = CompanyInfo.testInjection(obj.Beneficiary_Name);
                    //obj.Record_Insert_DateTime = Convert.ToDateTime(CompanyInfo.gettime(obj.Client_ID, Customer_ID.ToString(), obj.Country_ID));
                    string Record_Insert_DateTime1 = obj.Record_Insert_DateTime.ToString("yyyy-MM-dd");
                    string Record_Insert_DateTime = CompanyInfo.testInjection(Convert.ToString(obj.Record_Insert_DateTime));
                    string Delete_Status = CompanyInfo.testInjection(Convert.ToString(obj.Delete_Status));
                    string Created_By_User_ID = CompanyInfo.testInjection(Convert.ToString(obj.Created_By_User_ID));

                    string Customer_ID1 = CompanyInfo.testInjection(Convert.ToString(Customer_ID));
                    string Client_ID = CompanyInfo.testInjection(Convert.ToString(obj.Client_ID));
                    string Branch_ID = CompanyInfo.testInjection(Convert.ToString(obj.Branch_ID));

                    string AccountHolderName = CompanyInfo.testInjection(Convert.ToString(obj.AccountHolderName));
                    string Account_Number = CompanyInfo.testInjection(Convert.ToString(obj.Account_Number));
                    string BankCode = CompanyInfo.testInjection(Convert.ToString(obj.BankCode));
                    string Branch = CompanyInfo.testInjection(Convert.ToString(obj.Branch));
                    string Ifsc_Code = CompanyInfo.testInjection(Convert.ToString(obj.Ifsc_Code));
                    string BranchCode = CompanyInfo.testInjection(Convert.ToString(obj.BranchCode));
                    string Benf_Iban = CompanyInfo.testInjection(Convert.ToString(obj.Benf_Iban));
                    string Benf_BIC = CompanyInfo.testInjection(Convert.ToString(obj.Benf_BIC));
                    string Beneficiary_Country_IDChk = CompanyInfo.testInjection(Convert.ToString(obj.Beneficiary_Country_ID));
                    if (Beneficiary_Country_IDChk == "1" && Branch == "1" && Ifsc_Code == "1" && BranchCode == "1" && Benf_Iban == "1" && Benf_BIC == "1" && BankCode == "1" && Account_Number == "1" && AccountHolderName == "1" && Record_Insert_DateTime == "1" && Delete_Status == "1" &&
                     Created_By_User_ID == "1" && Customer_ID1 == "1" && Client_ID == "1" && Branch_ID == "1")
                    {
                        //MySqlConnector.MySqlTransaction transaction;
                        //transaction = con.BeginTransaction(IsolationLevel.ReadCommitted);
                        try
                        {
                            activity_benf = "Beneficiary Name: " + obj.Beneficiary_Name + " Bank branch: " + obj.BankBranch_ID + " Collection type: " + obj.Collection_type_Id +   //Parvej removed
                                "Beneficiary Bank Details: Account holder Name: " + obj.AccountHolderName + " Bank ID: " + obj.BBank_ID + "Account no: " + obj.Account_Number + " Bank Code: " + obj.BankCode + " Branch: " + obj.Branch + " IFSC code: " + obj.Ifsc_Code + " Branch Code: " + obj.BranchCode + " Iban no: " + obj.Benf_Iban + " BIC Code: " + obj.Benf_BIC +
                                " From  Branch : " + obj.Branch_ID + " For Customer : " + Customer_ID;



                            using (MySqlConnector.MySqlCommand cmd2 = new MySqlConnector.MySqlCommand("SP_Add_Beneficiary_Bank"))
                            {
                                cmd2.CommandType = CommandType.StoredProcedure;
                                //cmd2.Connection = con;

                                cmd2.Parameters.AddWithValue("_Branch_ID", obj.Branch_ID);
                                cmd2.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                                cmd2.Parameters.AddWithValue("_Beneficiary_ID", obj.Beneficiary_ID);
                                cmd2.Parameters.AddWithValue("_AccountHolderName", obj.AccountHolderName);
                                cmd2.Parameters.AddWithValue("_BBank_ID", obj.BBank_ID);
                                cmd2.Parameters.AddWithValue("_Account_Number", obj.Account_Number);
                                cmd2.Parameters.AddWithValue("_BankCode", obj.BankCode);

                                cmd2.Parameters.AddWithValue("_Branch", obj.Branch);
                                cmd2.Parameters.AddWithValue("_Record_Insert_DateTime", obj.Record_Insert_DateTime);
                                cmd2.Parameters.AddWithValue("_Delete_Status", obj.Delete_Status);
                                cmd2.Parameters.AddWithValue("_Customer_ID", Customer_ID);
                                cmd2.Parameters.AddWithValue("_Ifsc_Code", obj.Ifsc_Code);
                                cmd2.Parameters.AddWithValue("_BranchCode", obj.BranchCode);
                                cmd2.Parameters.AddWithValue("_Benf_Iban", obj.Benf_Iban);
                                cmd2.Parameters.AddWithValue("_Benf_BIC", obj.Benf_BIC);
                                cmd2.Parameters.AddWithValue("_BankBranch_ID", obj.BankBranch_ID);
                                cmd2.Parameters.AddWithValue("_Country_ID", obj.Beneficiary_Country_ID);//Parvej added

                                cmd2.Parameters.Add(new MySqlConnector.MySqlParameter("_existflag", MySqlConnector.MySqlDbType.Int32));
                                cmd2.Parameters["_existflag"].Direction = ParameterDirection.Output;


                                int n = db_connection.ExecuteNonQueryProcedure(cmd2);
                                if (n > 0)
                                {

                                    object ExistBeneficiary = cmd2.Parameters["_existflag"].Value;
                                    ExistBeneficiary = (ExistBeneficiary == DBNull.Value) ? null : ExistBeneficiary;


                                    obj.ExistBeneficiary = Convert.ToInt32(ExistBeneficiary);



                                    if (obj.ExistBeneficiary == 0)
                                    {

                                        obj.Message = "exist_Beneficiary";
                                        string Activity1 = "<b>Add Benficiary Bank: Beneficiary Bank already exists.</b> Beneficiary : " + obj.Beneficiary_Name + " Mobile: " + obj.Beneficiary_Mobile + " Country: " + obj.Beneficiary_Country_ID + " City: " + obj.Beneficiary_City_ID + "";
                                        if (obj.cashcollection_flag == 1) //uncheked
                                        {
                                            Activity1 = Activity1 + " Account_Number : " + obj.Account_Number + " Branch: " + obj.Branch + " BBank_ID: " + obj.BBank_ID + "";
                                        }
                                        int stattus = (int)CompanyInfo.InsertActivityLogDetails(Activity, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(Customer_ID), "Add Benficiary Bank", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "Add Benficiary Bank", context);
                                    }
                                    else
                                    {
                                        try
                                        {
                                            DataTable dt_notif = CompanyInfo.set_notification_data(78);
                                            if (dt_notif.Rows.Count > 0)
                                            {
                                                int SMS = Convert.ToInt32(dt_notif.Rows[0]["SMS"]);
                                                int Email = Convert.ToInt32(dt_notif.Rows[0]["Email"]);
                                                int Notif_status = Convert.ToInt32(dt_notif.Rows[0]["Notification"]);
                                                string notification_msg = Convert.ToString(dt_notif.Rows[0]["notification_msg"]);
                                                if (notification_msg.Contains("[Benf_name]") == true)
                                                {
                                                    notification_msg = notification_msg.Replace("[Benf_name]", Convert.ToString(obj.Beneficiary_Name));
                                                }

                                                int i = CompanyInfo.check_notification_perm(Convert.ToString(Customer_ID), obj.Client_ID, obj.Branch_ID, 2, 78, Convert.ToDateTime(obj.RecordDate), 1, SMS, Email, Notif_status, "App - Beneficiary Bank added Notification - 78", notification_msg, context);
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                        }
                                        //Parth moved success message in else cond'n to forward exist_Beneficiary message to return
                                        obj.Message = "success";
                                    }


                                    
                                }
                                else
                                {
                                    obj.Message = "Notsucess";
                                }
                            }
                            //cmd2.Dispose();
                        }
                        catch (Exception ex)
                        {
                            int stattus = (int)CompanyInfo.InsertActivityLogDetailsSecurity(ex.ToString(), Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), 1, "srvBenificiary", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "AddBenfBankAccount", 0, context);
                            //transaction.Rollback();

                        }
                        finally
                        {

                            int stattus = (int)CompanyInfo.InsertActivityLogDetailsSecurity(activity_benf, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), 1, "srvBenificiary", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "AddBenfBankAccount", 0, context);

                        }
                    }
                    else
                    {
                        string msg = "SQl Enjection detected";
                        obj.Id = -1;
                        int stattus = (int)CompanyInfo.InsertActivityLogDetailsSecurity(msg, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), 1, "srvBenificiary", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "AddBenfBankAccount", 0, context);
                    }

                }
            }
            else
            {
                obj.Id = 0;
                string msg = "Validation Failed " + " <br/>  " + error_invalid_data;
                obj.whereclause = "Validation Failed";
                obj.Message = error_msg;
                int stattus = (int)CompanyInfo.InsertActivityLogDetails(msg, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), 1, "srvBenificiary", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "Create", context);
            }
            return obj;
        }

        public Model.Beneficiary UpdateBank(Model.Beneficiary obj, HttpContext context)
        {

            int BBDetails_ID = Convert.ToInt32(obj.BBDetails_ID);
            int Customer_ID = Convert.ToInt32(CompanyInfo.Decrypt(obj.Customer_ID, true));
            string Activity = string.Empty;
            //var context = System.Web.HttpContext.Current;
            string Username = "calyx-api";
            DataTable dtbenfconfig = new DataTable();
            mts_connection _MTS = new mts_connection();

            using (MySqlConnector.MySqlConnection con = new MySqlConnector.MySqlConnection(_MTS.WebConnSetting()))
            {
                con.Open();
                obj.Record_Insert_DateTime = Convert.ToDateTime(CompanyInfo.gettime(obj.Client_ID, context));

                string error_msg = ""; string error_invalid_data = "";

                string FirstName_regex = validation.validate_accnm(obj.Beneficiary_Name, 0);
                string AccountHolderName_regex = validation.validate_accnm(obj.AccountHolderName, 0);
                string Account_Number_regex = validation.validate(obj.Account_Number, 1, 1, 0, 1, 1, 1, 1, 1, 1);
                string BankCode_regex = validation.validate(obj.BankCode, 1, 1, 0, 1, 1, 1, 1, 1, 1);
                string Branch_regex = validation.validate(obj.Branch, 1, 1, 1, 1, 1, 1, 1, 1, 0);
                string Ifsc_Code_regex = validation.validate(obj.Ifsc_Code, 1, 1, 0, 1, 1, 1, 1, 1, 1);
                string BranchCode_regex = validation.validate(obj.BranchCode, 1, 1, 0, 1, 1, 1, 1, 1, 1);
                string Benf_Iban_regex = validation.validate(obj.Benf_Iban, 1, 1, 0, 1, 1, 1, 1, 1, 1);
                string Benf_BIC_regex = validation.validate(obj.Benf_BIC, 1, 1, 0, 1, 1, 1, 1, 1, 1);
                string BBDetails_ID_regex = validation.validate(Convert.ToString(obj.BBDetails_ID), 1, 1, 0, 1, 1, 1, 1, 1, 1);
                string Beneficiary_Country_ID_regex = validation.validate(Convert.ToString(obj.Beneficiary_Country_ID), 1, 1, 0, 1, 1, 1, 1, 1, 1);


                if (BBDetails_ID_regex == "false")
                {
                    error_msg = error_msg + "Invalid BBDetails_ID";
                }
                if (Beneficiary_Country_ID_regex == "false")
                {
                    error_msg = error_msg + "Invalid Benficiary Coutry ID";
                }

                if (Benf_BIC_regex == "false")
                {
                    error_msg = error_msg + "BIC code should be alpanumeric without space.";
                    error_invalid_data = error_invalid_data + " BIC code: " + obj.Benf_BIC;
                }
                if (BankCode_regex == "false")
                {
                    error_msg = error_msg + "Bank code should be alpanumeric without space.";
                    error_invalid_data = error_invalid_data + " BankCode: " + obj.BankCode;
                }
                if (Branch_regex == "false")
                {
                    error_msg = error_msg + "Branch should be alphanumeric with space and special characters like , , , . , ' , ( , ) , { , } , - , @ , & ";
                    error_invalid_data = error_invalid_data + " Branch: " + obj.Branch;
                }
                if (Branch_regex == "false")
                {
                    error_msg = error_msg + "Branch code should be alphanumeric without space.";
                    error_invalid_data = error_invalid_data + " Branch: " + obj.BranchCode;
                }
                if (Ifsc_Code_regex == "false")
                {
                    error_msg = error_msg + "IFSC code should be alphanumeric without space.";
                    error_invalid_data = error_invalid_data + " Branch: " + obj.Ifsc_Code;
                }
                if (Benf_Iban_regex == "false")
                {
                    error_msg = error_msg + "IBAN code should be alphanumeric without space.";
                    error_invalid_data = error_invalid_data + " IBAN: " + obj.Benf_Iban;
                }
                if (Account_Number_regex == "false")
                {
                    error_msg = error_msg + "Account Number should be alphanumeric only without space.";
                    error_invalid_data = error_invalid_data + " Account_Number: " + obj.Account_Number;
                }
                if (FirstName_regex == "false" || AccountHolderName_regex == "false")
                {
                    error_msg = error_msg + "Name should include only charactes with space.";
                    if (FirstName_regex == "false" || AccountHolderName_regex == "false")
                    {
                        error_invalid_data = error_invalid_data + " Beneficiary_Name: " + obj.Beneficiary_Name;
                    }
                    if (AccountHolderName_regex == "false")
                    {
                        error_invalid_data = error_invalid_data + " Accountname: " + obj.AccountHolderName;
                    }

                }
                if (Beneficiary_Country_ID_regex == "" && BBDetails_ID_regex == "" && Benf_BIC_regex == "" && Benf_Iban_regex == "" && BranchCode_regex == "" && Ifsc_Code_regex == "" && Branch_regex == "" && AccountHolderName_regex == "" && BankCode_regex == "" && Account_Number_regex == "" && Benf_Iban_regex == "")
                {
                    FirstName_regex = "true"; BranchCode_regex = "true"; Ifsc_Code_regex = "true"; Branch_regex = "true"; AccountHolderName_regex = "true"; BankCode_regex = "true";
                    Account_Number_regex = "true";
                    Benf_Iban_regex = "true";
                    Benf_BIC_regex = "true";
                    BranchCode_regex = "true";
                    Beneficiary_Country_ID_regex = "true";
                    BBDetails_ID_regex = "true";

                }
                if (Benf_BIC_regex != "false" && Benf_Iban_regex != "false" && BranchCode_regex != "false" && Ifsc_Code_regex != "false" && Branch_regex != "false" && AccountHolderName_regex != "false" && BankCode_regex != "false" && Account_Number_regex != "false" && Benf_Iban_regex != "false" && FirstName_regex != "false")
                {
                    string Beneficiary_Name = CompanyInfo.testInjection(obj.Beneficiary_Name);
                    string Beneficiary_Country_ID = CompanyInfo.testInjection(Convert.ToString(obj.Beneficiary_Country_ID));
                    string Delete_Status = CompanyInfo.testInjection(Convert.ToString(obj.Delete_Status));
                    string Created_By_User_ID = CompanyInfo.testInjection(Convert.ToString(obj.Created_By_User_ID));

                    string Customer_ID1 = CompanyInfo.testInjection(Convert.ToString(Customer_ID));
                    string cashcollection_flag = CompanyInfo.testInjection(Convert.ToString(obj.cashcollection_flag));
                    string Client_ID = CompanyInfo.testInjection(Convert.ToString(obj.Client_ID));
                    string Branch_ID = CompanyInfo.testInjection(Convert.ToString(obj.Branch_ID));


                    string AccountHolderName = CompanyInfo.testInjection(Convert.ToString(obj.AccountHolderName));
                    string Account_Number = CompanyInfo.testInjection(Convert.ToString(obj.Account_Number));
                    string BankCode = CompanyInfo.testInjection(Convert.ToString(obj.BankCode));
                    string Branch = CompanyInfo.testInjection(Convert.ToString(obj.Branch));
                    string Ifsc_Code = CompanyInfo.testInjection(Convert.ToString(obj.Ifsc_Code));
                    string BranchCode = CompanyInfo.testInjection(Convert.ToString(obj.BranchCode));
                    string Benf_Iban = CompanyInfo.testInjection(Convert.ToString(obj.Benf_Iban));
                    string Benf_BIC = CompanyInfo.testInjection(Convert.ToString(obj.Benf_BIC));
                    string sectionvalue = CompanyInfo.testInjection(Convert.ToString(obj.sectionvalue));
                    string BBDetails_IDchk = CompanyInfo.testInjection(Convert.ToString(obj.BBDetails_ID));
                    string Beneficiary_Country_IDChk = CompanyInfo.testInjection(Convert.ToString(obj.Beneficiary_Country_ID));


                    if (Branch == "1" && Ifsc_Code == "1" && BranchCode == "1" && Benf_Iban == "1" && Benf_BIC == "1" && BankCode == "1" && Account_Number == "1"
                        && AccountHolderName == "1" && Beneficiary_Name == "1"
                        && Beneficiary_Country_ID == "1"
                        && Delete_Status == "1" && Created_By_User_ID == "1" && Customer_ID1 == "1" &&
                        cashcollection_flag == "1" && Client_ID == "1" && Branch_ID == "1" && Beneficiary_Country_IDChk == "1" && BBDetails_IDchk == "1")
                    {
                        MySqlConnector.MySqlCommand cmd_update1 = new MySqlConnector.MySqlCommand("SP_Beneficiary_Configuration");
                        cmd_update1.CommandType = CommandType.StoredProcedure;
                        cmd_update1.Connection = con;
                        cmd_update1.Parameters.AddWithValue("_Country_ID", obj.Beneficiary_Country_ID);
                        cmd_update1.Parameters.AddWithValue("_Collection_Type", obj.Collection_type_Id);
                        db_connection.ExecuteNonQueryProcedure(cmd_update1);
                        dtbenfconfig = db_connection.ExecuteQueryDataTableProcedure(cmd_update1);
                        if (obj.Beneficiary_ID != 0)
                        {
                            string[] check_alert = new string[7];
                            MySqlConnector.MySqlCommand cmd2 = new MySqlConnector.MySqlCommand("SP_Update_BenefBank");

                            cmd2.CommandType = CommandType.StoredProcedure;
                            cmd2.Connection = con;

                            cmd2.Parameters.AddWithValue("_config_accname", dtbenfconfig.Rows[0]["Beneficiary_Name"]);
                            cmd2.Parameters.AddWithValue("_config_accnum", dtbenfconfig.Rows[0]["ShowAccount_No"]);
                            cmd2.Parameters.AddWithValue("_config_ifsc", dtbenfconfig.Rows[0]["IFSC"]);
                            cmd2.Parameters.AddWithValue("_config_bnkname", dtbenfconfig.Rows[0]["Bank_Name"]);
                            cmd2.Parameters.AddWithValue("_config_bankcode", dtbenfconfig.Rows[0]["Bank_Code"]);
                            cmd2.Parameters.AddWithValue("_config_branchnm", dtbenfconfig.Rows[0]["Branch"]);
                            cmd2.Parameters.AddWithValue("_config_branchcode", dtbenfconfig.Rows[0]["Branch_Code"]);
                            cmd2.Parameters.AddWithValue("_config_iban", dtbenfconfig.Rows[0]["IBAN_Status"]);
                            cmd2.Parameters.AddWithValue("_config_bic", dtbenfconfig.Rows[0]["BIC_Status"]);

                            cmd2.Parameters.AddWithValue("_Beneficiary_ID", obj.Beneficiary_ID);
                            cmd2.Parameters.AddWithValue("_AccountHolderName", obj.AccountHolderName);
                            cmd2.Parameters.AddWithValue("_BBank_ID", obj.BBank_ID);
                            cmd2.Parameters.AddWithValue("_Account_Number", obj.Account_Number);
                            cmd2.Parameters.AddWithValue("_BankCode", obj.BankCode);
                            cmd2.Parameters.AddWithValue("_Bank_Name", obj.Bank_Name);
                            cmd2.Parameters.AddWithValue("_Ifsc_Code", obj.Ifsc_Code);
                            cmd2.Parameters.AddWithValue("_BranchCode", obj.BranchCode);
                            cmd2.Parameters.AddWithValue("_Branch", obj.Branch);
                            cmd2.Parameters.AddWithValue("_Benf_Iban", obj.Benf_Iban);
                            cmd2.Parameters.AddWithValue("_Benf_BIC", obj.Benf_BIC);
                            cmd2.Parameters.AddWithValue("_BBDetails_ID", obj.BBDetails_ID);
                            cmd2.Parameters.AddWithValue("_Country_ID", obj.Beneficiary_Country_ID);//Added

                            cmd2.Parameters.Add(new MySqlConnector.MySqlParameter("_existflag", MySqlConnector.MySqlDbType.Int32));
                            cmd2.Parameters["_existflag"].Direction = ParameterDirection.Output;


                            int result2 = cmd2.ExecuteNonQuery();

                            int ExistBeneficiaryBank = Convert.ToInt32(cmd2.Parameters["_existflag"].Value);

                            if (ExistBeneficiaryBank == 0)
                            {

                                obj.Message = "exist_Beneficiary_Bank";
                                string msg = "Bank Already Exist";
                                obj.Id = 3;
                                int stattus = (int)CompanyInfo.InsertActivityLogDetailsSecurity(msg, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), 1, "srvBenificiary", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "Update Beneficiary Bank", 0, context);
                            }
                            else if (result2 > 0 && ExistBeneficiaryBank == 1)
                            {
                                obj.Message = "success";
                                obj.Id = 0;
                            }
                            else
                            {
                                obj.Message = "Notsucess";
                            }

                            cmd2.Dispose();
                            #region insert into Benef Collection Mapping table
                            if (obj.Message == "success" && obj.Beneficiary_ID > 0)
                            {
                                try
                                {
                                    MySqlConnector.MySqlCommand cmdaw = new MySqlConnector.MySqlCommand("Insert_benef_collectiontype_mapping");
                                    cmdaw.CommandType = CommandType.StoredProcedure;
                                    cmdaw.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                                    cmdaw.Parameters.AddWithValue("_Branch_ID", obj.Branch_ID);
                                    cmdaw.Parameters.AddWithValue("_User_ID", obj.Created_By_User_ID);

                                    cmdaw.Parameters.AddWithValue("_Customer_ID", Customer_ID);
                                    cmdaw.Parameters.AddWithValue("_Beneficiary_ID", obj.Beneficiary_ID);

                                    cmdaw.Parameters.AddWithValue("_Collection_type_Id", obj.Collection_type_Id);
                                    cmdaw.Parameters.AddWithValue("_Record_Insert_DateTime", obj.Record_Insert_DateTime);

                                    cmdaw.Parameters.Add(new MySqlConnector.MySqlParameter("_existflag", MySqlConnector.MySqlDbType.Int32));
                                    cmdaw.Parameters["_existflag"].Direction = ParameterDirection.Output;

                                    int p = db_connection.ExecuteNonQueryProcedure(cmdaw);

                                    var insertflag = Convert.ToInt32(cmdaw.Parameters["_existflag"].Value);

                                    string Activitys1 = "Already Exist Bnef Collection Mapping Customer Id : " + Customer_ID + ",Beneficiary_ID : " + obj.Beneficiary_ID + ",Collection Type Id : " + obj.Collection_type_Id + ", User ID :" + obj.Created_By_User_ID;
                                    if (insertflag > 0)
                                    {
                                        Activitys1 = "Successfully Insert Into Benef Collection Mapping Customer Id : " + Customer_ID + ",Beneficiary_ID : " + obj.Beneficiary_ID + ",Collection Type Id : " + obj.Collection_type_Id + ", User ID :" + obj.Created_By_User_ID;
                                    }
                                    int stattus = (int)CompanyInfo.InsertActivityLogDetails(Activitys1, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(Customer_ID), "App Update Beneficiary", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), " Update Beneficiary", context);
                                }
                                catch (Exception ex)
                                {

                                    string stattus = (string)CompanyInfo.InsertErrorLogDetails(ex.Message.Replace("\'", "\\'"), 0, "App Update Beneficiary", obj.Branch_ID, obj.Client_ID);

                                }
                            }
                            #endregion
                            if (obj.Message == "success")
                            {
                                Activity = "<b>" + Username + "</b>" + "Beneficiary Bank Updated.  </br>";
                                int stattus1 = (int)CompanyInfo.InsertActivityLogDetails(Activity, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(Customer_ID), "UpdateBank", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "Update Beneficiary Bank", context);



                                #region updatetransaction
                                //vyankatesh change	
                                try
                                {
                                    MySqlConnector.MySqlCommand cperm = new MySqlConnector.MySqlCommand("GetPermissions");
                                    cperm.CommandType = CommandType.StoredProcedure;
                                    cperm.Parameters.AddWithValue("_whereclause", " and PID in (116)");
                                    cperm.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                                    DataTable drm = db_connection.ExecuteQueryDataTableProcedure(cperm);
                                    var perstatus = Convert.ToInt32(drm.Rows[0]["Status_ForCustomer"]);

                                    if (perstatus == 0)
                                    {
                                        MySqlConnector.MySqlCommand cmd = new MySqlConnector.MySqlCommand("select GROUP_CONCAT(Transaction_ID SEPARATOR ',') as Transaction_IDs from transaction_table where TransactionStatus_ID in (1,6) " +
                                        " and Beneficiary_ID=" + obj.Beneficiary_ID + " and PaymentDepositType_ID!=" + obj.cashcollection_flag + ";");
                                        string transactions = Convert.ToString(db_connection.ExecuteScalarProcedure(cmd));
                                        //var list = dt.AsEnumerable().Select(r => r["COLUMN1"].ToString());
                                        //string value = string.Join(",", list);
                                        string transwhere = " and TransactionStatus_ID in (1,6) ";
                                        string transwhere1 = " and TransactionStatus_ID in (1,6) ";//added by vyankatesh
                                        if (transactions != null && transactions != "")
                                        {
                                            cmd.Dispose();
                                            cmd = new MySqlConnector.MySqlCommand("update Transaction_Table set PaymentDepositType_ID = '" + obj.cashcollection_flag + "' " +
                                                "where Customer_ID = " + Customer_ID + " and Beneficiary_ID=" + obj.Beneficiary_ID + " and Transaction_ID in (" + transactions + ")");
                                            int a = db_connection.ExecuteNonQueryProcedure(cmd);
                                            cmd.Dispose();

                                            transwhere = transwhere + " and t.Transaction_ID in (" + transactions + ") ";
                                            transwhere1 = transwhere1 + " and Transaction_ID in (" + transactions + ") ";
                                        }
                                        if (obj.cashcollection_flag == 1)
                                        {
                                            string _transclause = " and Customer_ID = " + Customer_ID + " and Beneficiary_ID = " + obj.Beneficiary_ID + " and TransactionStatus_ID in (1,6) and Transaction_ID not in (select Transaction_ID from transwise_benfbankdetails where Beneficiary_ID=" + obj.Beneficiary_ID + " " + transwhere1 + ")";
                                            cmd = new MySqlConnector.MySqlCommand("get_transaction_table");
                                            cmd.CommandType = CommandType.StoredProcedure;
                                            cmd.Parameters.AddWithValue("_whereclause", _transclause);
                                            DataTable dt = (DataTable)db_connection.ExecuteQueryDataTableProcedure(cmd);
                                            cmd.Dispose();

                                            if (dt.Rows.Count > 0)
                                            {
                                                for (int i = 0; i < dt.Rows.Count; i++)
                                                {
                                                    using (MySqlConnector.MySqlCommand cm = new MySqlConnector.MySqlCommand("Insert_BankDepositDetails"))
                                                    {
                                                        cm.CommandType = CommandType.StoredProcedure;
                                                        cm.Parameters.AddWithValue("_CB_ID", obj.Branch_ID);
                                                        cm.Parameters.AddWithValue("_BBank_ID", obj.BBank_ID);
                                                        cm.Parameters.AddWithValue("_Account_Number", obj.Account_Number);
                                                        cm.Parameters.AddWithValue("_AccountHolderName", obj.AccountHolderName);
                                                        cm.Parameters.AddWithValue("_Branch", obj.Branch);
                                                        cm.Parameters.AddWithValue("_BranchCode", obj.BranchCode);
                                                        cm.Parameters.AddWithValue("_BankCode", obj.BankCode);
                                                        cm.Parameters.AddWithValue("_Ifsc_Code", obj.Ifsc_Code);
                                                        cm.Parameters.AddWithValue("_Beneficiary_Country_ID", obj.Beneficiary_Country_ID);
                                                        cm.Parameters.AddWithValue("_Beneficiary_City_ID", obj.Beneficiary_City_ID);
                                                        cm.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                                                        cm.Parameters.AddWithValue("_Customer_ID", Customer_ID);
                                                        cm.Parameters.AddWithValue("_Transaction_ID", Convert.ToString(dt.Rows[0]["Transaction_ID"]));
                                                        cm.Parameters.AddWithValue("_Benf_Iban", obj.Benf_Iban);
                                                        cm.Parameters.AddWithValue("_Benf_BIC", obj.Benf_BIC);
                                                        int bankmsgss = db_connection.ExecuteNonQueryProcedure(cm);
                                                    }
                                                }
                                            }
                                        }
                                        cmd = new MySqlConnector.MySqlCommand("update transwise_benfbankdetails tb inner join transaction_table t on tb.Transaction_ID=t.Transaction_ID  set AccountHolderName = '" + obj.AccountHolderName.Replace("'", "''").Trim() + "' , BBank_ID = '" + obj.BBank_ID + "' , " +
                                                "Beneficiary_City_ID = '" + obj.Beneficiary_City_ID + "' , Account_Number= '" + obj.Account_Number + "' , BankCode = '" + obj.BankCode + "' ,Branch = '" + obj.Branch.Replace("'", "''").Trim() + "'," +
                                                " Ifsc_Code ='" + obj.Ifsc_Code + "' , BranchCode = '" + obj.BranchCode + "', Iban_ID= '" + obj.Benf_Iban + "', BIC_Code ='" + obj.Benf_BIC + "' " +
                                                "where t.Customer_ID = " + Customer_ID + " and t.Beneficiary_ID=" + obj.Beneficiary_ID + " " + transwhere + "");
                                        int a1 = db_connection.ExecuteNonQueryProcedure(cmd);
                                        if (a1 > 0)
                                        {
                                            var Acts = " Beneficiary bank details update against transaction. Whereclause : t.Customer_ID = " + Customer_ID + " and t.Beneficiary_ID=" + obj.Beneficiary_ID + " and transaction list " + transwhere;
                                            int stattus = (int)CompanyInfo.InsertActivityLogDetailsSecurity(Acts, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), 1, "srvBenificiary", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "UpdateBeneficiaryBank-app", 0, context);
                                        }
                                    }
                                }
                                catch (Exception _x)
                                {
                                    string stattus = (string)CompanyInfo.InsertErrorLogDetails(_x.Message.Replace("\'", "\\'"), 0, "UpdateBeneficiaryBank-app", obj.Branch_ID, obj.Client_ID);
                                }
                                #endregion updatetransaction

                            }

                        }
                    }
                    else
                    {
                        string msg = "SQl Enjection detected . security_code=0";
                        obj.Id = -1;
                        int stattus = (int)CompanyInfo.InsertActivityLogDetailsSecurity(msg, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), 1, "srvBenificiary", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "Update Beneficiary Bank", 0, context);
                    }

                }
                else
                {
                    obj.Id = 1;
                    string msg = "Validation Failed " + " <br/>  " + error_invalid_data;
                    obj.whereclause = "Validation Failed";
                    obj.Message = error_msg;
                    int stattus = (int)CompanyInfo.InsertActivityLogDetails(msg, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), 1, "srvBenificiary", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "UpdateBank", context);
                }
            }

            return obj;
            //    return obj.Beneficiary_ID;
        }

        public DataTable GetTxnStatus(Model.Beneficiary obj, HttpContext context)  //Parvej added to get txn status
        {
            DataTable dt = new DataTable();
            string error_msg = string.Empty; string error_invalid_data = string.Empty;
            int Customer_ID = Convert.ToInt32(CompanyInfo.Decrypt(obj.Customer_ID, true));
            string Customer_ID_regex = validation.validate(Convert.ToString(Customer_ID), 0, 1, 1, 1, 1, 1, 0, 1, 1);
            string Beneficiary_ID_regex = validation.validate(Convert.ToString(obj.Beneficiary_ID), 0, 1, 1, 1, 1, 1, 1, 1, 1);
            string Client_ID_regex = validation.validate(Convert.ToString(obj.Client_ID), 0, 1, 1, 1, 1, 1, 1, 1, 1);
            string Branch_ID_regex = validation.validate(Convert.ToString(obj.Branch_ID), 0, 1, 1, 1, 1, 1, 1, 1, 1);
            string Country_ID_regex = validation.validate(Convert.ToString(obj.Country_ID), 0, 1, 1, 1, 1, 1, 1, 1, 1);
            string BBank_ID_regex = validation.validate(Convert.ToString(obj.BBank_ID), 0, 1, 1, 1, 1, 1, 1, 1, 1);

            if (Customer_ID_regex != "false" && Beneficiary_ID_regex != "false" && Client_ID_regex != "false" && Branch_ID_regex != "false" && Country_ID_regex != "false" && BBank_ID_regex != "false")
            {
                string Customer_ID1 = CompanyInfo.testInjection(Convert.ToString(obj.Customer_ID));
                string Beneficiary_ID = CompanyInfo.testInjection(Convert.ToString(obj.Beneficiary_ID));
                string Country_ID = CompanyInfo.testInjection(Convert.ToString(obj.Country_ID));
                string BBank_ID = CompanyInfo.testInjection(Convert.ToString(obj.BBank_ID));
                string Client_ID = CompanyInfo.testInjection(Convert.ToString(obj.Client_ID));
                string Branch_ID = CompanyInfo.testInjection(Convert.ToString(obj.Branch_ID));
                if (Customer_ID1 == "1" && Beneficiary_ID == "1" && Country_ID == "1" && BBank_ID == "1" && Client_ID == "1" && Branch_ID == "1")
                {
                    string Whereclause = string.Empty;
                    if (Customer_ID != 0)
                    {
                        Whereclause = Whereclause + " and tt.Customer_ID =" + Customer_ID;
                    }
                    if (obj.Beneficiary_ID != 0)
                    {
                        Whereclause = Whereclause + " and tt.Beneficiary_ID =" + obj.Beneficiary_ID;
                    }
                    if (obj.Country_ID != 0)
                    {
                        Whereclause = Whereclause + " and tb.Beneficiary_Country_ID =" + obj.Country_ID;
                    }
                    if (obj.BBank_ID != 0)
                    {
                        Whereclause = Whereclause + " and tb.BBank_ID =" + obj.BBank_ID;
                    }

                    MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("Get_Txn_Status");
                    _cmd.CommandType = CommandType.StoredProcedure;
                    _cmd.Parameters.AddWithValue("_Whereclause", Whereclause);
                    dt = db_connection.ExecuteQueryDataTableProcedure(_cmd);
                }
                else
                {
                    string msg = "SQl Enjection detected . security_code=0";
                    obj.Id = -1;
                    int stattus = (int)CompanyInfo.InsertActivityLogDetailsSecurity(msg, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), 1, "GetTxnStatus", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "srvBenificiary", 0, context);
                }
            }
            else
            {
                //string msg = "Validation Failed " + " <br/>  " + error_invalid_data;
                string msg = "Validation Error Customer_ID_regex- +" + Customer_ID_regex + "Beneficiary_ID_regex- " + Beneficiary_ID_regex + " Client_ID_regex- +" + Client_ID_regex + " +Branch_ID_regex- " + Branch_ID_regex + "Country_ID_regex- " + Country_ID_regex + "BBank_ID_regex-" + BBank_ID_regex;
                int stattus = (int)CompanyInfo.InsertActivityLogDetails(msg, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), 1, "srvBenificiary", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "UpdateBank", context);
            }

            return dt;
        }

        public Model.Beneficiary Create_new(Model.Beneficiary obj, HttpContext context)
        {
            string activity_benf = "";
            int txn_id = 0;
            activity_benf = activity_benf + "Beneficiary Save Start";
            string[] Alert_Msg = new string[7];
            string Activity = string.Empty;
            //var context = System.Web.HttpContext.Current;
            string error_msg = ""; string error_invalid_data = "";
            //string Username = Convert.ToString(context.Request.Form["Username"]);
            string Username = context.User.Identity.Name;

            int m = 0; DataTable dsp = new DataTable();
            DateTime Birth_Date_DateTime = new DateTime();
            if (obj.Birth_Date != "Invalid date" && obj.Birth_Date != "" && obj.Birth_Date != null && (obj.Birth_Date).ToLower() != "undefined")
            {
                //Birth_Date_DateTime = Convert.ToDateTime(obj.Birth_Date);                    
            }
            int Customer_ID = Convert.ToInt32(CompanyInfo.Decrypt(obj.Customer_ID, true));
            int checkaml = 1;
            string FirstName_regex = validation.validate_accnm(obj.Beneficiary_Name, 0);
            //string FirstName_regex = validation.validate(obj.Beneficiary_Name, 1, 1, 1, 1, 1, 1, 0, 1, 1);
            string address1_regex = validation.validate(obj.Beneficiary_Address, 1, 1, 1, 1, 1, 1, 1, 1, 0);
            string TelephoneNumber_regex = validation.validate(obj.Beneficiary_Telephone, 1, 1, 1, 0, 1, 1, 1, 1, 1);
            string MobileNumber_regex = validation.validate(obj.Beneficiary_Mobile, 1, 1, 1, 0, 1, 1, 1, 1, 1);
            string address2_regex = validation.validate(obj.Beneficiary_Address1, 1, 1, 1, 1, 1, 1, 1, 1, 0);
            string PostCode_regex = validation.validate(obj.Beneficiary_PostCode, 1, 1, 1, 1, 1, 1, 1, 0, 1);
            string state_regex = validation.validate(obj.Beneficiary_State, 1, 1, 1, 1, 1, 1, 0, 1, 1); //added by siddhi
            string AccountHolderName_regex = validation.validate_accnm(obj.AccountHolderName, 0);
            //string AccountHolderName_regex = validation.validate(obj.AccountHolderName, 1, 1, 1, 1, 1, 1, 0, 1, 1);
            string Account_Number_regex = validation.validate(obj.Account_Number, 1, 1, 0, 1, 1, 1, 1, 1, 1);
            string BankCode_regex = validation.validate(obj.BankCode, 1, 1, 0, 1, 1, 1, 1, 1, 1);
            string Branch_regex = validation.validate(obj.Branch, 1, 1, 1, 1, 1, 1, 1, 1, 0);
            string Ifsc_Code_regex = validation.validate(obj.Ifsc_Code, 1, 1, 0, 1, 1, 1, 1, 1, 1);
            string BranchCode_regex = validation.validate(obj.BranchCode, 1, 1, 0, 1, 1, 1, 1, 1, 1);
            string Benf_Iban_regex = validation.validate(obj.Benf_Iban, 1, 1, 0, 1, 1, 1, 1, 1, 1);
            string Benf_BIC_regex = validation.validate(obj.Benf_BIC, 1, 1, 0, 1, 1, 1, 1, 1, 1);

            MySqlCommand _cmdl = new MySqlCommand("GetPermissions");
            _cmdl.CommandType = CommandType.StoredProcedure;
            _cmdl.Parameters.AddWithValue("_whereclause", " and PID=204");
            _cmdl.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
            DataTable dttp = db_connection.ExecuteQueryDataTableProcedure(_cmdl);
            //Check Beneficiery AML AT the time of registration perm
            int check_benef_aml_through_compliance_assist = 1;
            DataRow[] dr = dttp.Select("PID=204");
            if (dr.Count() > 0)
            {
                foreach (DataRow drr in dr)
                {
                    check_benef_aml_through_compliance_assist = Convert.ToInt32(drr["Status_ForCustomer"]);
                }
            }

            int address_regex_len = 100;
            string HouseNumber_len = "true"; string Street_len = "true"; string AddressLine1_len = "true"; string AddressLine2_len = "true";
            if (Benf_BIC_regex == "false")
            {
                error_msg = error_msg + "BIC code should be alpanumeric without space.";
                error_invalid_data = error_invalid_data + " BIC code: " + obj.Benf_BIC;
            }
            if (BankCode_regex == "false")
            {
                error_msg = error_msg + "Bank code should be alpanumeric without space.";
                error_invalid_data = error_invalid_data + " BankCode: " + obj.BankCode;
            }
            if (Branch_regex == "false")
            {
                error_msg = error_msg + "Branch should be alphanumeric with space and special characters like , , , . , ' , ( , ) , { , } , - , @ , & ";
                error_invalid_data = error_invalid_data + " Branch: " + obj.Branch;
            }
            if (Branch_regex == "false")
            {
                error_msg = error_msg + "Branch code should be alphanumeric without space.";
                error_invalid_data = error_invalid_data + " Branch: " + obj.BranchCode;
            }
            if (Ifsc_Code_regex == "false")
            {
                error_msg = error_msg + "IFSC code should be alphanumeric without space.";
                error_invalid_data = error_invalid_data + " Branch: " + obj.Ifsc_Code;
            }
            if (Benf_Iban_regex == "false")
            {
                error_msg = error_msg + "IBAN code should be alphanumeric without space.";
                error_invalid_data = error_invalid_data + " IBAN: " + obj.Benf_Iban;
            }
            if (Account_Number_regex == "false")
            {
                error_msg = error_msg + "Account Number should be alphanumeric only without space.";
                error_invalid_data = error_invalid_data + " Account_Number: " + obj.Account_Number;
            }

            if (obj.Beneficiary_Address.Length >= address_regex_len)
            {
                AddressLine1_len = "false";
            }
            if (obj.Beneficiary_Address1.Length >= address_regex_len)
            {
                AddressLine2_len = "false";
            }
            if (FirstName_regex == "false" || AccountHolderName_regex == "false")
            {
                error_msg = error_msg + "Name should include only charactes with space.";
                if (FirstName_regex == "false" || AccountHolderName_regex == "false")
                {
                    error_invalid_data = error_invalid_data + " Beneficiary_Name: " + obj.Beneficiary_Name;
                }
                if (AccountHolderName_regex == "false")
                {
                    error_invalid_data = error_invalid_data + " Accountname: " + obj.AccountHolderName;
                }

            }
            if (address1_regex == "false" || address2_regex == "false")
            {
                string len_house = "";
                if (AddressLine1_len == "false" || AddressLine2_len == "false")
                {
                    len_house = len_house + "Address should be of valid length.";
                    error_invalid_data = error_invalid_data + "  Address length: " + obj.Beneficiary_Address.Length;
                }
                error_msg = error_msg + " Address should be alphanumeric with space and special characters like , , , . , ' , ( , ) , { , } , - , @ , & " + len_house;
                error_invalid_data = error_invalid_data + " Address: " + obj.Beneficiary_Address;
            }
            if (AddressLine1_len == "false" || AddressLine2_len == "false")
            {
                string len_house = "";
                error_invalid_data = error_invalid_data + "  Address length: " + obj.Beneficiary_Address.Length;
            }
            if (TelephoneNumber_regex == "false" || MobileNumber_regex == "false")
            {
                error_msg = error_msg + " Phone and Mobile number Should be numeric without space";
                if (TelephoneNumber_regex == "false")
                { error_invalid_data = error_invalid_data + "  MobileNumber: " + obj.Beneficiary_Mobile; }
                if (MobileNumber_regex == "false")
                { error_invalid_data = error_invalid_data + "  PhoneNumber: " + obj.Beneficiary_Mobile; }

            }
            if (PostCode_regex == "false")
            {
                error_msg = error_msg + " Postcode Should be alphanumeric.";
                error_invalid_data = error_invalid_data + "  Postcode: " + obj.Beneficiary_PostCode;
            }
            if (state_regex == "false")
            {
                error_msg = error_msg + " State Should be alphabates only.";
                error_invalid_data = error_invalid_data + "  State: " + obj.Beneficiary_State;
            }
            if (Benf_BIC_regex == "" && Benf_Iban_regex == "" && BranchCode_regex == "" && Ifsc_Code_regex == "" && Branch_regex == "" && AccountHolderName_regex == "" && BankCode_regex == "" && Account_Number_regex == "" && Benf_Iban_regex == "" && PostCode_regex == "" && FirstName_regex == "" && address1_regex == "" && TelephoneNumber_regex == "" && MobileNumber_regex == "" && address2_regex == "" && PostCode_regex == "" && AddressLine1_len == "true" && AddressLine2_len == "true")
            {
                FirstName_regex = "true"; BranchCode_regex = "true"; Ifsc_Code_regex = "true"; Branch_regex = "true"; AccountHolderName_regex = "true"; BankCode_regex = "true";
                address1_regex = "true"; Account_Number_regex = "true";
                TelephoneNumber_regex = "true";
                MobileNumber_regex = "true";
                address2_regex = "true";
                PostCode_regex = "true";
                Benf_Iban_regex = "true";
                Benf_BIC_regex = "true";
                BranchCode_regex = "true";

            }
            if (Benf_BIC_regex != "false" && Benf_Iban_regex != "false" && BranchCode_regex != "false" && Ifsc_Code_regex != "false" && Branch_regex != "false" && AccountHolderName_regex != "false" && BankCode_regex != "false" && Account_Number_regex != "false" && Benf_Iban_regex != "false" && PostCode_regex != "false" && FirstName_regex != "false" && address1_regex != "false" && TelephoneNumber_regex != "false" && MobileNumber_regex != "false" && address2_regex != "false" && PostCode_regex != "false" && AddressLine1_len != "false" && AddressLine2_len != "false")
            {
                //Service.mts_connection _MTS = new Service.mts_connection();

                db_connection _dbConnection = new db_connection();

                using (MySqlConnector.MySqlConnection con = new MySqlConnector.MySqlConnection(_dbConnection.ConnectionStringSection()))
                {
                    con.Open();
                    obj.Record_Insert_DateTime = Convert.ToDateTime(CompanyInfo.gettime(obj.Client_ID, Customer_ID.ToString(), obj.Country_ID, context));
                    string Beneficiary_Name = CompanyInfo.testInjection(obj.Beneficiary_Name);
                    //Digvijay changes for restricted keywords for beneficiary name eg.ltd,pvt,private,limited
                    MySqlCommand _cmdperm = new MySqlCommand("GetPermissions");
                    _cmdperm.CommandType = CommandType.StoredProcedure;
                    _cmdperm.Parameters.AddWithValue("_whereclause", " and PID in (182)");
                    _cmdperm.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                    DataTable dtperm = db_connection.ExecuteQueryDataTableProcedure(_cmdperm);
                    if (dtperm.Rows.Count > 0)
                    {
                        if (Convert.ToInt32(dtperm.Rows[0]["Status_ForCustomer"]) == 0)
                        {
                            var benf_name = obj.Beneficiary_Name;
                            //string benfNameCheck = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["benfNameCheck"]);
                            // Get IServiceProvider from HttpContext
                            var serviceProvider = context.RequestServices;

                            // Dynamically resolve IConfiguration
                            var configuration = serviceProvider.GetRequiredService<IConfiguration>();

                            // Get benfNameCheck from appsettings.json
                            string benfNameCheck = configuration["AppSettings:benfNameCheck"];
                            // Split the forbidden terms into an array
                            string[] forbiddenTerms = benfNameCheck.Split(',').Select(term => term.Trim()).ToArray();
                            // Check if the beneficiary name contains any forbidden terms
                            bool containsForbiddenWord = forbiddenTerms.Any(term => benf_name.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0);

                            if (containsForbiddenWord)
                            {
                                obj.Message = "InValidName";
                                string Activity1 = "<b>Add Benficiary: Beneficiary name invalid.</b> Beneficiary : " + obj.Beneficiary_Name + " Mobile: " + obj.Beneficiary_Mobile + " Country: " + obj.Beneficiary_Country_ID + " City: " + obj.Beneficiary_City_ID + "";
                                if (obj.cashcollection_flag == 1) //uncheked
                                {
                                    Activity1 = Activity1 + " Account_Number : " + obj.Account_Number + " Branch: " + obj.Branch + " BBank_ID: " + obj.BBank_ID + "";
                                }
                                int stattus = (int)CompanyInfo.InsertActivityLogDetails(Activity, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(Customer_ID), "Add Benficiary", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "Add Benficiary", context);
                                return obj;


                            }
                        }

                    }//end
                    string Beneficiary_Address = CompanyInfo.testInjection(obj.Beneficiary_Address);
                    string Beneficiary_City_ID = CompanyInfo.testInjection(Convert.ToString(obj.Beneficiary_City_ID));
                    string Beneficiary_Country_ID = CompanyInfo.testInjection(Convert.ToString(obj.Beneficiary_Country_ID));
                    string Beneficiary_Telephone = CompanyInfo.testInjection(Convert.ToString(obj.Beneficiary_Telephone));
                    string Beneficiary_Mobile = CompanyInfo.testInjection(Convert.ToString(obj.Beneficiary_Mobile));
                    string Record_Insert_DateTime = CompanyInfo.testInjection(Convert.ToString(obj.Record_Insert_DateTime));
                    string Delete_Status = CompanyInfo.testInjection(Convert.ToString(obj.Delete_Status));
                    string Created_By_User_ID = CompanyInfo.testInjection(Convert.ToString(obj.Created_By_User_ID));

                    string Customer_ID1 = CompanyInfo.testInjection(Convert.ToString(Customer_ID));
                    string Beneficiary_Address1 = CompanyInfo.testInjection(Convert.ToString(obj.Beneficiary_Address1));
                    string Beneficiary_PostCode = CompanyInfo.testInjection(Convert.ToString(obj.Beneficiary_PostCode));
                    string cashcollection_flag = CompanyInfo.testInjection(Convert.ToString(obj.cashcollection_flag));
                    string Agent_MappingID = CompanyInfo.testInjection(Convert.ToString(obj.Agent_MappingID));
                    string Client_ID = CompanyInfo.testInjection(Convert.ToString(obj.Client_ID));
                    string Branch_ID = CompanyInfo.testInjection(Convert.ToString(obj.Branch_ID));
                    string Relation_ID = CompanyInfo.testInjection(Convert.ToString(obj.Relation_ID));

                    string Beneficiary_PostCode1 = CompanyInfo.testInjection(Convert.ToString(obj.Beneficiary_PostCode));
                    string Beneficiary_State = CompanyInfo.testInjection(Convert.ToString(obj.Beneficiary_State));

                    string AccountHolderName = CompanyInfo.testInjection(Convert.ToString(obj.AccountHolderName));
                    string Account_Number = CompanyInfo.testInjection(Convert.ToString(obj.Account_Number));
                    string BankCode = CompanyInfo.testInjection(Convert.ToString(obj.BankCode));
                    string Branch = CompanyInfo.testInjection(Convert.ToString(obj.Branch));
                    string Ifsc_Code = CompanyInfo.testInjection(Convert.ToString(obj.Ifsc_Code));
                    string BranchCode = CompanyInfo.testInjection(Convert.ToString(obj.BranchCode));
                    string Benf_Iban = CompanyInfo.testInjection(Convert.ToString(obj.Benf_Iban));
                    string Benf_BIC = CompanyInfo.testInjection(Convert.ToString(obj.Benf_BIC));
                    if (Branch == "1" && Ifsc_Code == "1" && BranchCode == "1" && Benf_Iban == "1" && Benf_BIC == "1" && BankCode == "1" && Account_Number == "1" && AccountHolderName == "1" && Beneficiary_PostCode1 == "1" && Beneficiary_Name == "1" && Beneficiary_Address == "1" && Beneficiary_City_ID == "1" && Beneficiary_Country_ID == "1" &&
                     Beneficiary_Telephone == "1" && Beneficiary_Mobile == "1" && Record_Insert_DateTime == "1" && Delete_Status == "1" &&
                     Created_By_User_ID == "1" && Customer_ID1 == "1" && Beneficiary_Address1 == "1" && Beneficiary_PostCode == "1" &&
                         cashcollection_flag == "1" && Agent_MappingID == "1" && Client_ID == "1" && Branch_ID == "1" && Relation_ID == "1" && Beneficiary_State == "1"
                         )
                    {
                        if (con.State != System.Data.ConnectionState.Open)
                        {
                            con.Open();
                        }
                        int existflag = 1;  //Parvej change
                                            //check bank existance
                        if (obj.cashcollection_flag == 1)
                        {
                            using (MySqlCommand cmd3 = new MySqlCommand("sp_check_bank_exists", con))
                            {  //Parvej added
                                cmd3.CommandType = CommandType.StoredProcedure;
                                cmd3.Parameters.AddWithValue("_BBDetails_ID", 0);
                                cmd3.Parameters.AddWithValue("_BBank_ID", obj.BBank_ID);
                                cmd3.Parameters.AddWithValue("_Account_Number", obj.Account_Number);
                                cmd3.Parameters.AddWithValue("_Benf_Iban", obj.Benf_Iban);
                                cmd3.Parameters.Add(new MySqlParameter("_existflag", MySqlDbType.Int32));
                                cmd3.Parameters["_existflag"].Direction = ParameterDirection.Output;

                                int n2 = cmd3.ExecuteNonQuery();

                                existflag = Convert.ToInt32(cmd3.Parameters["_existflag"].Value);

                                obj.ExistBeneficiary = Convert.ToInt32(existflag);
                                if (obj.ExistBeneficiary == 0)
                                {
                                    //return "exist_mobile";
                                    obj.Message = "exist_Beneficiary";
                                    string Activity1 = "<b>Add Benficiary: Beneficiary Bank already exists.</b> Beneficiary : " + obj.Beneficiary_Name + " Mobile: " + obj.Beneficiary_Mobile + " Country: " + obj.Beneficiary_Country_ID + " City: " + obj.Beneficiary_City_ID + "";
                                    if (obj.cashcollection_flag == 1) //uncheked
                                    {
                                        Activity1 = Activity1 + " Account_Number : " + obj.Account_Number + " Branch: " + obj.Branch + " BBank_ID: " + obj.BBank_ID + "";
                                    }
                                    int stattus = (int)CompanyInfo.InsertActivityLogDetails(Activity, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(Customer_ID), "Add Benficiary", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "Add Benficiary", context);
                                }
                            }
                        }
                        if (existflag == 1)
                        {
                            //MySqlConnector.MySqlTransaction transaction;
                            //transaction = con.BeginTransaction(IsolationLevel.ReadCommitted);
                            MySqlConnector.MySqlTransaction transaction;
                            transaction = con.BeginTransaction(IsolationLevel.ReadCommitted);


                            try
                            {
                                try
                                {
                                    activity_benf = "Beneficiary Personal Details Name: " + obj.Beneficiary_Name + " Address: " + obj.Beneficiary_Address + " Beneficiary postcode: " + obj.Beneficiary_PostCode + " Beneficiary State:" + obj.Beneficiary_State + " City: " + obj.Beneficiary_City_ID + " Country: " + obj.Beneficiary_Country_ID + " Tel: " + obj.Beneficiary_Telephone + " Mob: " + obj.Beneficiary_Mobile +
                                        "Customer id: " + obj.Customer_ID + " Relation: " + obj.Relation_ID + " Bank branch: " + obj.BankBranch_ID + " Collection type: " + obj.Collection_type_Id + " Wallet ID: " + obj.Wallet_Id + " Wallet provider: " + obj.Wallet_provider + " Mobile Provider: " + obj.Mobile_provider +
                                        "Beneficiary Bank Details: Account holder Name: " + obj.AccountHolderName + " Bank ID: " + obj.BBank_ID + "Account no: " + obj.Account_Number + " Bank Code: " + obj.BankCode + " Branch: " + obj.Branch + " IFSC code: " + obj.Ifsc_Code + " Branch Code: " + obj.BranchCode + " Iban no: " + obj.Benf_Iban + " BIC Code: " + obj.Benf_BIC +
                                        " From  Branch : " + obj.Branch_ID + " For Customer : " + Customer_ID;

                                }
                                catch (Exception ex)
                                {
                                }
                                using (MySqlConnector.MySqlCommand cmd = new MySqlConnector.MySqlCommand("SP_Save_Beneficiary", con))
                                {

                                    cmd.CommandType = CommandType.StoredProcedure;
                                    cmd.Transaction = transaction;
                                    cmd.CommandTimeout = 200;
                                    cmd.Parameters.AddWithValue("_Beneficiary_Name", obj.Beneficiary_Name.Trim());
                                    cmd.Parameters.AddWithValue("_Beneficiary_Address", obj.Beneficiary_Address.Trim());
                                    cmd.Parameters.AddWithValue("_Beneficiary_City_ID", obj.Beneficiary_City_ID);
                                    cmd.Parameters.AddWithValue("_Beneficiary_Country_ID", obj.Beneficiary_Country_ID);
                                    cmd.Parameters.AddWithValue("_Beneficiary_Telephone", obj.Beneficiary_Telephone.Trim());

                                    cmd.Parameters.AddWithValue("_Beneficiary_Mobile", obj.Beneficiary_Mobile.Trim());

                                    cmd.Parameters.AddWithValue("_Record_Insert_DateTime", obj.Record_Insert_DateTime);
                                    cmd.Parameters.AddWithValue("_Delete_Status", obj.Delete_Status);

                                    cmd.Parameters.AddWithValue("_Created_By_User_ID", obj.Created_By_User_ID);
                                    cmd.Parameters.AddWithValue("_Customer_ID", Customer_ID);

                                    cmd.Parameters.AddWithValue("_Beneficiary_Address1", obj.Beneficiary_Address1.Trim());
                                    cmd.Parameters.AddWithValue("_Beneficiary_PostCode", obj.Beneficiary_PostCode.Trim());
                                    cmd.Parameters.AddWithValue("_State", obj.Beneficiary_State.Trim());
                                    cmd.Parameters.AddWithValue("_cashcollection_flag", obj.cashcollection_flag);
                                    cmd.Parameters.AddWithValue("_Agent_MappingID", obj.Agent_MappingID);
                                    cmd.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                                    cmd.Parameters.AddWithValue("_Branch_ID", obj.Branch_ID);
                                    cmd.Parameters.AddWithValue("_Relation_ID", obj.Relation_ID);
                                    cmd.Parameters.AddWithValue("_Iban_id", obj.Benf_Iban);
                                    if (obj.Birth_Date != "Invalid date" && obj.Birth_Date != "" && obj.Birth_Date != null && (obj.Birth_Date).ToLower() != "undefined")
                                    {
                                        DateTime DOB = DateTime.ParseExact(obj.Birth_Date, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                                        string Birth_Date = DOB.ToString("yyyy-MM-dd HH:mm:ss");
                                        cmd.Parameters.AddWithValue("_DOB", Convert.ToDateTime(Birth_Date)); //cmd.Parameters.AddWithValue("_DOB", Birth_Date_DateTime);
                                    }
                                    else
                                    {
                                        cmd.Parameters.AddWithValue("_DOB", null);
                                    }
                                    cmd.Parameters.AddWithValue("_Beneficiary_Gender", obj.Beneficiary_Gender);
                                    //cmd.Parameters.AddWithValue("_Beneficiary_Gender", obj.Benf_Gender);//Digvijay changes for add gender
                                    cmd.Parameters.AddWithValue("_Collection_type_Id", obj.Collection_type_Id);
                                    cmd.Parameters.AddWithValue("_Wallet_Id", obj.Wallet_Id);
                                    cmd.Parameters.AddWithValue("_Mobile_provider", obj.Mobile_provider);
                                    cmd.Parameters.AddWithValue("_Wallet_provider", obj.Wallet_provider);
                                    cmd.Parameters.AddWithValue("_BBank_ID", obj.BBank_ID);
                                    cmd.Parameters.AddWithValue("_Account_Number", obj.Account_Number);
                                    cmd.Parameters.AddWithValue("_BankCode", obj.BankCode);
                                    cmd.Parameters.AddWithValue("_Branch", obj.Branch);


                                    cmd.Parameters.Add(new MySqlConnector.MySqlParameter("_Benf_ID", MySqlConnector.MySqlDbType.Int32));
                                    cmd.Parameters["_Benf_ID"].Direction = ParameterDirection.Output;

                                    cmd.Parameters.Add(new MySqlConnector.MySqlParameter("_existBenf", MySqlConnector.MySqlDbType.Int32));
                                    cmd.Parameters["_existBenf"].Direction = ParameterDirection.Output;

                                    int n = cmd.ExecuteNonQuery();

                                    //    obj.ExistBeneficiary = Convert.ToInt32(cmd.Parameters["_existBenf"].Value);
                                    object ExistBeneficiary = cmd.Parameters["_existBenf"].Value;
                                    ExistBeneficiary = (ExistBeneficiary == DBNull.Value) ? null : ExistBeneficiary;
                                    // row["Transaction_Master_ID"] != DBNull.Value;

                                    obj.ExistBeneficiary = Convert.ToInt32(ExistBeneficiary);
                                    if (obj.ExistBeneficiary == 1)
                                    {
                                        //return "exist_mobile";
                                        obj.Message = "exist_Beneficiary";
                                        string Activity1 = "<b>Add Benficiary: Beneficiary already exists.</b> Beneficiary : " + obj.Beneficiary_Name + " Mobile: " + obj.Beneficiary_Mobile + " Country: " + obj.Beneficiary_Country_ID + " City: " + obj.Beneficiary_City_ID + "";
                                        if (obj.cashcollection_flag == 1) //uncheked
                                        {
                                            Activity1 = Activity1 + " Account_Number : " + obj.Account_Number + " Branch: " + obj.Branch + " BBank_ID: " + obj.BBank_ID + "";
                                        }
                                        int stattus = (int)CompanyInfo.InsertActivityLogDetails(Activity, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(Customer_ID), "Add Benficiary", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "Add Benficiary", context);
                                    }
                                    else
                                    {
                                        if (obj.Beneficiary_ID == 0) // insert
                                        {
                                            try
                                            {
                                                obj.Beneficiary_ID = Convert.ToInt32(cmd.Parameters["_Benf_ID"].Value);
                                                activity_benf = activity_benf + " and personal details saved Beneficiary ID: " + obj.Beneficiary_ID;
                                            }
                                            catch (Exception)
                                            {

                                                //transaction.Rollback();
                                                //throw new System.InvalidOperationException("This company allready exist ..");
                                            }
                                        }

                                        //obj.Beneficiary_ID = 0;
                                        //object result = db_connection.ExecuteScalarProcedure(cmd);

                                        //result = (result == DBNull.Value) ? null : result;
                                        //obj.Beneficiary_ID = Convert.ToInt32(result);
                                        cmd.Dispose();

                                        if (obj.Beneficiary_ID != 0)
                                        {
                                            //Parvej changes to add defauly limits to beneficiary
                                            #region Add default limits to beneficiery

                                            try
                                            {
                                                int p = 0;
                                                using (MySqlCommand cmdbenflimit = new MySqlCommand("Add_default_limits_to_beneficiery"))
                                                {


                                                    cmdbenflimit.CommandType = CommandType.StoredProcedure;
                                                    cmdbenflimit.Parameters.AddWithValue("_Beneficiary_ID", obj.Beneficiary_ID);
                                                    cmdbenflimit.Parameters.AddWithValue("_Customer_ID", Customer_ID);
                                                    cmdbenflimit.Parameters.AddWithValue("_Beneficiary_Name", obj.Beneficiary_Name);
                                                    cmdbenflimit.Parameters.AddWithValue("_Record_Insert_DateTime", obj.Record_Insert_DateTime);
                                                    cmdbenflimit.Parameters.AddWithValue("_Update_Date", obj.Record_Insert_DateTime);
                                                    cmdbenflimit.Parameters.AddWithValue("_Start_Date", obj.Record_Insert_DateTime);
                                                    cmdbenflimit.Parameters.AddWithValue("_Client_id", obj.Client_ID);
                                                    cmdbenflimit.Parameters.AddWithValue("_branch_id", obj.Branch_ID);


                                                    p = db_connection.ExecuteNonQueryProcedure(cmdbenflimit);
                                                }



                                                string Activitys1 = "";
                                                if (p > 0)
                                                {
                                                    Activitys1 = "Successfully Insert Default Limits to Customer Id : " + obj.Customer_ID + ",Beneficiary_ID : " + obj.Beneficiary_ID + ", User ID :" + obj.User_ID;
                                                }
                                                else
                                                {
                                                    Activitys1 = "Failed to Insert Default Limits to Customer Id : " + obj.Customer_ID + ",Beneficiary_ID : " + obj.Beneficiary_ID + ", User ID :" + obj.User_ID;
                                                }
                                                int stattus = (int)CompanyInfo.InsertActivityLogDetails(Activitys1, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(Customer_ID), "App Add Beneficiary", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "App Add Beneficiary", context);
                                            }
                                            catch (Exception ex)
                                            {
                                                string stattus = (string)CompanyInfo.InsertErrorLogDetails(ex.Message.Replace("\'", "\\'"), 0, "App Add Beneficiary", obj.Branch_ID, obj.Client_ID);
                                            }

                                            #endregion

                                            #region insert into Benef Collection Mapping table
                                            try
                                            {
                                                MySqlCommand cmdaw = new MySqlCommand("Insert_benef_collectiontype_mapping");
                                                cmdaw.CommandType = CommandType.StoredProcedure;
                                                cmdaw.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                                                cmdaw.Parameters.AddWithValue("_Branch_ID", obj.Branch_ID);
                                                cmdaw.Parameters.AddWithValue("_User_ID", obj.Created_By_User_ID);

                                                cmdaw.Parameters.AddWithValue("_Customer_ID", Customer_ID);
                                                cmdaw.Parameters.AddWithValue("_Beneficiary_ID", obj.Beneficiary_ID);

                                                cmdaw.Parameters.AddWithValue("_Collection_type_Id", obj.Collection_type_Id);
                                                cmdaw.Parameters.AddWithValue("_Record_Insert_DateTime", obj.Record_Insert_DateTime);

                                                cmdaw.Parameters.Add(new MySqlParameter("_existflag", MySqlDbType.Int32));
                                                cmdaw.Parameters["_existflag"].Direction = ParameterDirection.Output;

                                                int p = db_connection.ExecuteNonQueryProcedure(cmdaw);

                                                var insertflag = Convert.ToInt32(cmdaw.Parameters["_existflag"].Value);

                                                string Activitys1 = "Already Exist Bnef Collection Mapping Customer Id : " + Customer_ID + ",Beneficiary_ID : " + obj.Beneficiary_ID + ",Collection Type Id : " + obj.Collection_type_Id + ", User ID :" + obj.Created_By_User_ID;
                                                if (insertflag > 0)
                                                {
                                                    Activitys1 = "Successfully Insert Into Benef Collection Mapping Customer Id : " + Customer_ID + ",Beneficiary_ID : " + obj.Beneficiary_ID + ",Collection Type Id : " + obj.Collection_type_Id + ", User ID :" + obj.Created_By_User_ID;
                                                }
                                                int stattus = (int)CompanyInfo.InsertActivityLogDetails(Activitys1, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(Customer_ID), "App Add Beneficiary", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "App Add Beneficiary", context);
                                            }
                                            catch (Exception ex)
                                            {

                                                string stattus = (string)CompanyInfo.InsertErrorLogDetails(ex.Message.Replace("\'", "\\'"), 0, "App Add Beneficiary", obj.Branch_ID, obj.Client_ID);

                                            }
                                            #endregion

                                            try
                                            {
                                                #region beneficiary probable match
                                                obj.whereclause = " Beneficiary_ID != " + obj.Beneficiary_ID + " and Beneficiary_Country_ID =" + obj.Beneficiary_Country_ID + " and Beneficiary_City_ID =" + obj.Beneficiary_City_ID + " and bm.Client_ID= " + obj.Client_ID;
                                                if (obj.Birth_Date != "" && obj.Birth_Date != null && obj.Birth_Date != "null")
                                                {
                                                    obj.whereclause = obj.whereclause + " and (date(DateOf_Birth) =date(" + obj.Birth_Date + ") or 1=1 ) ";
                                                }
                                                if (obj.Beneficiary_Address != "" && obj.Beneficiary_Address != null && obj.Beneficiary_Address != "null")
                                                {
                                                    obj.whereclause = obj.whereclause + " AND   (Beneficiary_Address = '" + obj.Beneficiary_Address + "' or 1=1)";
                                                }
                                                MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("Beneficiary_probable_match");
                                                _cmd.CommandType = CommandType.StoredProcedure;
                                                _cmd.Parameters.AddWithValue("_whereclause", obj.whereclause);
                                                _cmd.Parameters.AddWithValue("_first_name", obj.First_Name);
                                                _cmd.Parameters.AddWithValue("_middle_name", obj.Middle_Name);
                                                _cmd.Parameters.AddWithValue("_last_name", obj.Last_Name);
                                                dsp = db_connection.ExecuteQueryDataTableProcedure(_cmd);
                                                m = dsp.Rows.Count;

                                                #endregion
                                            }
                                            catch (Exception exb) { }
                                            Model.Beneficiary _Objbenef = new Model.Beneficiary();
                                            _Objbenef.Beneficiary_ID = obj.Beneficiary_ID;
                                            _Objbenef.Delete_Status = 0;

                                            string notification_icon = "beneficiary.jpg";
                                            string notification_message = "<span class='cls-admin'>New <strong class='cls-new-benf'>Beneficiary</strong> successfully added.</span><span class='cls-customer'></span>";
                                            MySqlConnector.MySqlCommand cmd2 = new MySqlConnector.MySqlCommand("SP_Save_Beneificiary_BankDetails");
                                            cmd2.CommandType = CommandType.StoredProcedure;
                                            cmd2.Transaction = transaction;
                                            cmd2.Connection = con;
                                            cmd2.Parameters.AddWithValue("_Notification", notification_message);
                                            cmd2.Parameters.AddWithValue("_Notification_Icon", notification_icon);
                                            cmd2.Parameters.AddWithValue("_Branch_ID", obj.Branch_ID);
                                            cmd2.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                                            cmd2.Parameters.AddWithValue("_Beneficiary_ID", obj.Beneficiary_ID);
                                            cmd2.Parameters.AddWithValue("_AccountHolderName", obj.AccountHolderName);
                                            cmd2.Parameters.AddWithValue("_BBank_ID", obj.BBank_ID);
                                            cmd2.Parameters.AddWithValue("_Account_Number", obj.Account_Number);
                                            cmd2.Parameters.AddWithValue("_BankCode", obj.BankCode);

                                            cmd2.Parameters.AddWithValue("_Branch", obj.Branch);
                                            cmd2.Parameters.AddWithValue("_Record_Insert_DateTime", obj.Record_Insert_DateTime);
                                            cmd2.Parameters.AddWithValue("_Delete_Status", obj.Delete_Status);
                                            cmd2.Parameters.AddWithValue("_Customer_ID", Customer_ID);
                                            cmd2.Parameters.AddWithValue("_Ifsc_Code", obj.Ifsc_Code);
                                            cmd2.Parameters.AddWithValue("_BranchCode", obj.BranchCode);
                                            cmd2.Parameters.AddWithValue("_Benf_Iban", obj.Benf_Iban);
                                            cmd2.Parameters.AddWithValue("_Benf_BIC", obj.Benf_BIC);
                                            cmd2.Parameters.AddWithValue("_BankBranch_ID", obj.BankBranch_ID);
                                            string[] check_alert = new string[7];
                                            int n1 = cmd2.ExecuteNonQuery();
                                            if (n1 > 0)//success
                                            {
                                                activity_benf = activity_benf + " for Bank details Beneficiary ID: " + obj.Beneficiary_ID;
                                                try
                                                {
                                                    DataTable dt_notif = CompanyInfo.set_notification_data(5);
                                                    if (dt_notif.Rows.Count > 0)
                                                    {
                                                        int SMS = Convert.ToInt32(dt_notif.Rows[0]["SMS"]);
                                                        int Email = Convert.ToInt32(dt_notif.Rows[0]["Email"]);
                                                        int Notif_status = Convert.ToInt32(dt_notif.Rows[0]["Notification"]);
                                                        string notification_msg = Convert.ToString(dt_notif.Rows[0]["notification_msg"]);
                                                        if (notification_msg.Contains("[Benf_name]") == true)
                                                        {
                                                            notification_msg = notification_msg.Replace("[Benf_name]", Convert.ToString(Beneficiary_Name));
                                                        }

                                                        int i = CompanyInfo.check_notification_perm(Convert.ToString(Customer_ID), obj.Client_ID, obj.Branch_ID, 2, 5, Convert.ToDateTime(obj.RecordDate), 1, SMS, Email, Notif_status, "App - Beneficiary Added Notification - 5", notification_msg, context);
                                                    }
                                                }
                                                catch (Exception ex)
                                                {
                                                }

                                                //transaction.Commit();
                                                #region check loaction
                                                DataTable dtc = CompanyInfo.get(obj.Client_ID, context);
                                                try
                                                {

                                                    string act = "";

                                                    Boolean chkLocation = true;
                                                    string country_log = "";
                                                    string device_ty = "";

                                                    try
                                                    {

                                                        DataTable chkLocation1 = CompanyInfo.check_location(obj.Client_ID, obj.userAgent, context);
                                                        try
                                                        {
                                                            chkLocation = Convert.ToBoolean(chkLocation1.Rows[0]["is_valid"]);
                                                            country_log = Convert.ToString(chkLocation1.Rows[0]["Country"]);
                                                            device_ty = Convert.ToString(chkLocation1.Rows[0]["device_ty"]);
                                                        }
                                                        catch
                                                        {
                                                        }

                                                    }
                                                    catch (Exception ex)
                                                    {

                                                    }
                                                    if (!chkLocation)
                                                    {

                                                        try
                                                        {
                                                            DataTable dt_notif = CompanyInfo.set_notification_data(27);
                                                            if (dt_notif.Rows.Count > 0)
                                                            {
                                                                int SMS = Convert.ToInt32(dt_notif.Rows[0]["SMS"]);
                                                                int Email = Convert.ToInt32(dt_notif.Rows[0]["Email"]);
                                                                int Notif_status = Convert.ToInt32(dt_notif.Rows[0]["Notification"]);
                                                                string notification_msg = Convert.ToString(dt_notif.Rows[0]["notification_msg"]);
                                                                if (notification_msg.Contains("[Benf_name]") == true)
                                                                {
                                                                    notification_msg = notification_msg.Replace("[Benf_name]", Convert.ToString(obj.Beneficiary_Name));
                                                                }

                                                                int i = CompanyInfo.check_notification_perm(Convert.ToString(Customer_ID), obj.Client_ID, obj.Branch_ID, 2, 27, Convert.ToDateTime(obj.RecordDate), 1, SMS, Email, Notif_status, "App - Beneficiary Added from new location Notification - 27", notification_msg, context);
                                                            }
                                                        }
                                                        catch (Exception ex)
                                                        {
                                                        }
                                                        string body = string.Empty, subject = string.Empty;
                                                        string body1 = string.Empty;
                                                        HttpWebRequest httpRequest = null, httpRequest1 = null;
                                                        DataTable d2 = (DataTable)CompanyInfo.getCustomerDetails(obj.Client_ID, Customer_ID);
                                                        string sendmsg1 = "Beneficiary Added From New Location";
                                                        string company_name = Convert.ToString(dtc.Rows[0]["Company_Name"]);
                                                        string URL = Convert.ToString(dtc.Rows[0]["Company_URL_Admin"]);
                                                        string cust_url = Convert.ToString(dtc.Rows[0]["Company_URL_Customer"]);
                                                        httpRequest = (HttpWebRequest)WebRequest.Create(URL + "Email/new-login.html");
                                                        httpRequest.UserAgent = "Code Sample Web Client";
                                                        HttpWebResponse webResponse = (HttpWebResponse)httpRequest.GetResponse();
                                                        using (StreamReader reader = new StreamReader(webResponse.GetResponseStream()))
                                                        {
                                                            body = reader.ReadToEnd();
                                                        }
                                                        body = body.Replace("[name]", Convert.ToString(d2.Rows[0]["First_Name"]));
                                                        string enc_ref = CompanyInfo.Encrypt(Convert.ToString(d2.Rows[0]["WireTransfer_ReferanceNo"]), true);
                                                        string link = cust_url + "/secure-account-verfiy?reference=" + enc_ref;
                                                        body = body.Replace("[link]", link);
                                                        body = body.Replace("[New_Login_Detected]", "Beneficiary Added From New Location ");
                                                        body = body.Replace("[country]", country_log);
                                                        body = body.Replace("[time]", (Convert.ToDateTime(CompanyInfo.gettime(obj.Client_ID, context))).ToString("dddd, dd MMMM yyyy HH:mm:ss"));
                                                        body = body.Replace("[location_msg]", "We noticed a new beneficiary upload to your account from new location that we don't recognise. If this wasn't you, we'll help you secure your account.");
                                                        body = body.Replace("[device]", device_ty);




                                                        string EmailID = "";
                                                        EmailID = Convert.ToString(d2.Rows[0]["Email_ID"]);


                                                        subject = company_name + " - Beneficiary Added From New Location - Alert " + d2.Rows[0]["WireTransfer_ReferanceNo"];
                                                        string send_mail = (string)CompanyInfo.Send_Mail(dtc, EmailID, body, subject, obj.Client_ID, obj.Branch_ID, "", "", "", context);

                                                        //Notification
                                                        notification_icon = "beneficiary.jpg";
                                                        notification_message = "<span class='cls-admin'>Beneficiary <strong class='cls-new-benf'>Added From New Location </strong></span>";
                                                        CompanyInfo.save_notification_compliance(notification_message, notification_icon, Convert.ToString(Customer_ID), Convert.ToDateTime(obj.Record_Insert_DateTime), obj.Client_ID, 1, 0, obj.Branch_ID, 0, 1, 1, 0, context);
                                                    }
                                                }
                                                catch
                                                {

                                                }
                                                #endregion
                                                transaction.Commit();
                                                check_alert = AutoCheckSanctionList(obj, context);
                                                int var1 = 0;
                                                List<string> l = new List<string>();
                                                var arr = Alert_Msg.Union(check_alert).ToArray();
                                                if (arr.Length > 0)
                                                {
                                                    for (int j = 0; j < arr.Length - 1; j++)
                                                    {
                                                        if (arr[j] != "" && arr[j] != "null" && arr[j] != null)
                                                        {
                                                            //    arr[j] = arr[j];
                                                            l.Add(arr[j]);
                                                            //var1 = 1;
                                                        }
                                                    }
                                                }
                                                try
                                                {
                                                    if (check_benef_aml_through_compliance_assist == 0)
                                                    {
                                                        if (checkaml == 1)
                                                        {
                                                            CHECK_AML(obj, obj.Beneficiary_ID, "Beneficeary Register", context);
                                                        }
                                                    }
                                                }
                                                catch (Exception ex)
                                                {


                                                }
                                                string final_msg = string.Empty; int cnt = 1;
                                                for (int i = var1; i < l.Count; i++)
                                                {

                                                    if (l[i] != "" && l[i] != "null" && l[i] != null)
                                                    {
                                                        final_msg += cnt + "  ." + l[i] + "<br />";
                                                        cnt++;
                                                    }
                                                }
                                                string sendmsg = "Beneficiary details saved successfully but we are unable to check following:" + "<br/>" + final_msg;
                                                if (l.Count > var1)
                                                {
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

                                                    dtc = CompanyInfo.get(obj.Client_ID, context);
                                                    string company_name = Convert.ToString(dtc.Rows[0]["Company_Name"]);
                                                    string URL = Convert.ToString(dtc.Rows[0]["Company_URL_Admin"]);
                                                    string cust_url = Convert.ToString(dtc.Rows[0]["Company_URL_Customer"]);



                                                    HttpWebRequest httpRequest;

                                                    httpRequest = (HttpWebRequest)WebRequest.Create(URL + "Email/customemail.html");

                                                    httpRequest.UserAgent = "Code Sample Web Client";
                                                    HttpWebResponse webResponse = (HttpWebResponse)httpRequest.GetResponse();
                                                    using (StreamReader reader = new StreamReader(webResponse.GetResponseStream()))
                                                    {
                                                        body = reader.ReadToEnd();
                                                    }
                                                    body = body.Replace("[name]", "Administrators");

                                                    body = body.Replace("[msg]", sendmsg);


                                                    DataTable d1 = (DataTable)CompanyInfo.getCustomerDetails(obj.Client_ID, Customer_ID);



                                                    subject = company_name + " - Compliance Alert  " + d1.Rows[0]["WireTransfer_ReferanceNo"];



                                                    //  HttpWebRequest httpRequest1 = (HttpWebRequest)WebRequest.Create("" + HttpContext.Current.Session["EmailUrl"] + "Email/RegisteredSuccessMail.txt");

                                                    //httpRequest1.UserAgent = "Code Sample Web Client";
                                                    //HttpWebResponse webResponse1 = (HttpWebResponse)httpRequest1.GetResponse();
                                                    //using (StreamReader reader = new StreamReader(webResponse1.GetResponseStream()))
                                                    //{
                                                    //    subject = reader.ReadLine();
                                                    //}
                                                    string send_mail = (string)CompanyInfo.Send_Mail(dtc, EmailID, body, subject, obj.Client_ID, obj.Branch_ID, "", "", "", context);
                                                    //  string    mail_send = (string)CompanyInfo.Send_Mail(email, body, subject, Convert.ToInt32(c.Client_ID), Convert.ToInt32(c.Branch_ID), "Alert Admins", "", "");
                                                }
                                            }
                                            cmd2.Dispose();
                                            if (n > 0 && obj.Beneficiary_ID != 0)
                                            {
                                                if (m > 0)
                                                {
                                                    try
                                                    {
                                                        m = (int)(dsp.Rows[0]["Beneficiary_ID"]);
                                                        MySqlConnector.MySqlCommand cmd_update = new MySqlConnector.MySqlCommand("update beneficiary_master set ProbableMatch_Flag=0,Matching_ID=@Matching_ID  where Beneficiary_ID=@Beneficiary_Id and Client_ID=@Client_ID");
                                                        cmd_update.Parameters.AddWithValue("@Matching_ID", m);
                                                        cmd_update.Parameters.AddWithValue("@Beneficiary_Id", obj.Beneficiary_ID);
                                                        cmd_update.Parameters.AddWithValue("@Client_ID", obj.Client_ID);
                                                        db_connection.ExecuteNonQueryProcedure(cmd_update);
                                                        Activity = "Add Benficiary: Probable match found for beneficiary " + obj.First_Name + " " + obj.Middle_Name + " " + obj.Last_Name + ".";
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        //Activity = "";
                                                        //Activity = " Beneficiary probable match found ";
                                                        Activity = " Beneficiary probable match found failed to update.";
                                                    }
                                                    //string Activity = "";
                                                    //Activity = " Beneficiary probable match found ";
                                                    //mtsmethods.InsertActivityLogDetails_Beneficiary(Activity, user_id, 0, user_id, cust_id, "UpdateBenf_Details", c.Branch_ID, c.Client_ID, c.Beneficiary_ID);
                                                    //Activity = "Add Benficiary: Probable match found for beneficiary " + obj.First_Name + " " + obj.Middle_Name + " " + obj.Last_Name + ".";
                                                    int stattus = (int)CompanyInfo.InsertActivityLogDetails(Activity, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(Customer_ID), "Update", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "Update Beneficiary", context);
                                                }
                                                obj.Message = "success";
                                                Activity = "<b>" + Username + "</b>" + "Beneficiary Inserted.  </br>";
                                            }
                                            else
                                            {
                                                obj.Message = "notsuccess";
                                                Activity = "<b>" + Username + "</b>" + "Failed to Insert Beneficiary details.  </br>";
                                            }
                                            obj.Message = "success";
                                            Activity = Activity + "Beneficiary Details:  Beneficiary Name:" + obj.Beneficiary_Name + " Mobile No: " + obj.Beneficiary_Mobile;
                                            if (obj.BBank_ID != null && obj.BBank_ID != 1 && obj.BBank_ID != 0)
                                            {
                                                string bank_nm = Convert.ToString(obj.Bank_Name);
                                                string bank_details = "Bank Details: Bank Name: " + bank_nm + " Account Holder: " + obj.AccountHolderName + "  Account Number: " + obj.Account_Number;
                                                if (obj.Ifsc_Code != null && obj.Ifsc_Code != "")
                                                {
                                                    bank_details = bank_details + " IFSC Code: " + obj.Ifsc_Code;
                                                }
                                                if (obj.Branch != null && obj.Branch != "")
                                                {
                                                    bank_details = bank_details + " Branch: " + obj.Branch;
                                                }
                                                if (obj.BranchCode != null && obj.BranchCode != "")
                                                {
                                                    bank_details = bank_details + " Branch Code: " + obj.BranchCode;
                                                }
                                                Activity = Activity + " " + bank_details;
                                            }
                                            Activity = "<b> App - </b>" + Activity + "  </br>";
                                            int stattus1 = (int)CompanyInfo.InsertActivityLogDetails(Activity, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(Customer_ID), "Create", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "Insert Beneficiary", context);
                                        }
                                    }


                                }
                            }
                            catch (Exception ex)
                            {
                                int stattus = (int)CompanyInfo.InsertActivityLogDetailsSecurity(ex.ToString(), Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), 1, "srvBenificiary", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "create", 0, context);
                                //transaction.Rollback();
                                //throw ex;
                                txn_id++;
                            }
                            finally
                            {
                                if (txn_id == 0)
                                {
                                    int stattus = (int)CompanyInfo.InsertActivityLogDetailsSecurity(activity_benf, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), 1, "srvBenificiary", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "create", 0, context);
                                }

                                con.Close();
                            }
                        }
                    }
                    else
                    {
                        string msg = "SQl Enjection detected";
                        obj.Id = -1;
                        // Activity = "<b>" + Username + "</b>" + " Password changed.  </br>";
                        //int stattus = (int)CompanyInfo.InsertActivityLogDetails(msg, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID),obj.Id, "IsValidCustomer", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "IsValidCustomer");
                        int stattus = (int)CompanyInfo.InsertActivityLogDetailsSecurity(msg, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), 1, "srvBenificiary", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "create", 0, context);
                    }

                }
            }
            else
            {
                obj.Id = 0;
                string msg = "Validation Failed " + " <br/>  " + error_invalid_data;
                obj.whereclause = "Validation Failed";
                obj.Message = error_msg;
                int stattus = (int)CompanyInfo.InsertActivityLogDetails(msg, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), 1, "srvBenificiary", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "Create", context);
            }
            return obj;
            //}

            //     return obj.Beneficiary_ID;
        }

        public Model.Beneficiary Update_new(Model.Beneficiary obj, HttpContext context)
        {
            int Customer_ID = Convert.ToInt32(CompanyInfo.Decrypt(obj.Customer_ID, true));
            string Activity = string.Empty;
            //var context = System.Web.HttpContext.Current;
            // string Username = Convert.ToString(context.Request.Form["Username"]);
            DataTable dtbenfconfig = new DataTable();
            mts_connection _MTS = new mts_connection();
            int benefnamechange_flag = 0;
            using (MySqlConnector.MySqlConnection con = new MySqlConnector.MySqlConnection(_MTS.WebConnSetting()))
            {
                con.Open();
                obj.Record_Insert_DateTime = Convert.ToDateTime(CompanyInfo.gettime(obj.Client_ID, context));
                string error_msg = ""; string error_invalid_data = "";
                DateTime Birth_Date_DateTime = new DateTime();
                if (obj.Birth_Date != "Invalid date" && obj.Birth_Date != "" && obj.Birth_Date != null && (obj.Birth_Date).ToLower() != "undefined")
                {
                    //Birth_Date_DateTime = Convert.ToDateTime(obj.Birth_Date);                    
                }
                string FirstName_regex = validation.validate_accnm(obj.Beneficiary_Name, 0);// validation.validate(obj.Beneficiary_Name, 1, 1, 1, 1, 1, 1, 0, 1, 1);
                string address1_regex = validation.validate(obj.Beneficiary_Address, 1, 1, 1, 1, 1, 1, 1, 1, 0);
                string TelephoneNumber_regex = validation.validate(obj.Beneficiary_Telephone, 1, 1, 1, 0, 1, 1, 1, 1, 1);
                string MobileNumber_regex = validation.validate(obj.Beneficiary_Mobile, 1, 1, 1, 0, 1, 1, 1, 1, 1);
                string address2_regex = validation.validate(obj.Beneficiary_Address1, 1, 1, 1, 1, 1, 1, 1, 1, 0);
                //string PostCode_regex = validation.validate(obj.Beneficiary_PostCode, 1, 1, 0, 1, 1, 1, 1, 1, 1);
                string PostCode_regex = validation.validate(obj.Beneficiary_PostCode, 1, 1, 1, 1, 1, 1, 1, 0, 1);
                string state_regex = validation.validate(obj.Beneficiary_State, 1, 1, 1, 1, 1, 1, 0, 1, 1); //added by siddhi

                string AccountHolderName_regex = validation.validate_accnm(obj.AccountHolderName, 0); //validation.validate(obj.AccountHolderName, 1, 1, 1, 1, 1, 1, 0, 1, 1);
                                                                                                      //string Account_Number_regex = validation.validate(obj.Account_Number, 0, 1, 1, 1, 1, 1, 1, 1, 1);
                                                                                                      //updated account number validation changes by siddhi
                string Account_Number_regex = validation.validate(obj.Account_Number, 1, 1, 0, 1, 1, 1, 1, 1, 1);
                string BankCode_regex = validation.validate(obj.BankCode, 1, 1, 0, 1, 1, 1, 1, 1, 1);
                string Branch_regex = validation.validate(obj.Branch, 1, 1, 1, 1, 1, 1, 1, 1, 0);
                string Ifsc_Code_regex = validation.validate(obj.Ifsc_Code, 1, 1, 0, 1, 1, 1, 1, 1, 1);
                string BranchCode_regex = validation.validate(obj.BranchCode, 1, 1, 0, 1, 1, 1, 1, 1, 1);
                string Benf_Iban_regex = validation.validate(obj.Benf_Iban, 1, 1, 0, 1, 1, 1, 1, 1, 1);
                string Benf_BIC_regex = validation.validate(obj.Benf_BIC, 1, 1, 0, 1, 1, 1, 1, 1, 1);

                MySqlCommand _cmdl = new MySqlCommand("GetPermissions");
                _cmdl.CommandType = CommandType.StoredProcedure;
                _cmdl.Parameters.AddWithValue("_whereclause", " and PID=204");
                _cmdl.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                DataTable dttp = db_connection.ExecuteQueryDataTableProcedure(_cmdl);
                //Check Beneficiery AML AT the time of registration perm
                int check_benef_aml_through_compliance_assist = 1;
                DataRow[] dr = dttp.Select("PID=204");
                if (dr.Count() > 0)
                {
                    foreach (DataRow drr in dr)
                    {
                        check_benef_aml_through_compliance_assist = Convert.ToInt32(drr["Status_ForCustomer"]);
                    }
                }

                int address_regex_len = 100;
                string HouseNumber_len = "true"; string Street_len = "true"; string AddressLine1_len = "true"; string AddressLine2_len = "true";
                if (Benf_BIC_regex == "false")
                {
                    error_msg = error_msg + "BIC code should be alpanumeric without space.";
                    error_invalid_data = error_invalid_data + " BIC code: " + obj.Benf_BIC;
                }
                if (BankCode_regex == "false")
                {
                    error_msg = error_msg + "Bank code should be alpanumeric without space.";
                    error_invalid_data = error_invalid_data + " BankCode: " + obj.BankCode;
                }
                if (Branch_regex == "false")
                {
                    error_msg = error_msg + "Branch should be alphanumeric with space and special characters like , , , . , ' , ( , ) , { , } , - , @ , & ";
                    error_invalid_data = error_invalid_data + " Branch: " + obj.Branch;
                }
                if (Branch_regex == "false")
                {
                    error_msg = error_msg + "Branch code should be alphanumeric without space.";
                    error_invalid_data = error_invalid_data + " Branch: " + obj.BranchCode;
                }
                if (Ifsc_Code_regex == "false")
                {
                    error_msg = error_msg + "IFSC code should be alphanumeric without space.";
                    error_invalid_data = error_invalid_data + " Branch: " + obj.Ifsc_Code;
                }
                if (Benf_Iban_regex == "false")
                {
                    error_msg = error_msg + "IBAN code should be alphanumeric without space.";
                    error_invalid_data = error_invalid_data + " IBAN: " + obj.Benf_Iban;
                }
                if (Account_Number_regex == "false")
                {
                    //error_msg = error_msg + "Account Number should be numeric only without space.";
                    error_msg = error_msg + "Account Number should be alphanumeric only without space.";
                    error_invalid_data = error_invalid_data + " Account_Number: " + obj.Account_Number;
                }

                if (obj.Beneficiary_Address.Length >= address_regex_len)
                {
                    AddressLine1_len = "false";
                }
                if (obj.Beneficiary_Address1.Length >= address_regex_len)
                {
                    AddressLine2_len = "false";
                }
                if (FirstName_regex == "false" || AccountHolderName_regex == "false")
                {
                    error_msg = error_msg + "Name should include only charactes with space.";
                    if (FirstName_regex == "false" || AccountHolderName_regex == "false")
                    {
                        error_invalid_data = error_invalid_data + " Beneficiary_Name: " + obj.Beneficiary_Name;
                    }
                    if (AccountHolderName_regex == "false")
                    {
                        error_invalid_data = error_invalid_data + " Accountname: " + obj.AccountHolderName;
                    }

                }
                if (address1_regex == "false" || address2_regex == "false")
                {
                    string len_house = "";
                    if (AddressLine1_len == "false" || AddressLine2_len == "false")
                    {
                        len_house = len_house + "Address should be of valid length.";
                        error_invalid_data = error_invalid_data + "  Address length: " + obj.Beneficiary_Address.Length;
                    }
                    error_msg = error_msg + " Address should be alphanumeric with space and special characters like , , , . , ' , ( , ) , { , } , - , @ , & " + len_house;
                    error_invalid_data = error_invalid_data + " Address: " + obj.Beneficiary_Address;
                }
                if (AddressLine1_len == "false" || AddressLine2_len == "false")
                {
                    string len_house = "";
                    error_invalid_data = error_invalid_data + "  Address length: " + obj.Beneficiary_Address.Length;
                }
                if (TelephoneNumber_regex == "false" || MobileNumber_regex == "false")
                {
                    error_msg = error_msg + " Phone and Mobile number Should be numeric without space";
                    if (TelephoneNumber_regex == "false")
                    { error_invalid_data = error_invalid_data + "  MobileNumber: " + obj.Beneficiary_Mobile; }
                    if (MobileNumber_regex == "false")
                    { error_invalid_data = error_invalid_data + "  PhoneNumber: " + obj.Beneficiary_Mobile; }

                }
                if (PostCode_regex == "false")
                {
                    //error_msg = error_msg + " Postcode Should be alphanumeric without space";
                    error_msg = error_msg + " Postcode Should be alphanumeric.";
                    error_invalid_data = error_invalid_data + "  Postcode: " + obj.Beneficiary_PostCode;
                }
                if (state_regex == "false")
                {
                    error_msg = error_msg + " State Should be alphabates only.";
                    error_invalid_data = error_invalid_data + "  State: " + obj.Beneficiary_State;
                }
                if (Benf_BIC_regex == "" && Benf_Iban_regex == "" && BranchCode_regex == "" && Ifsc_Code_regex == "" && Branch_regex == "" && AccountHolderName_regex == "" && BankCode_regex == "" && Account_Number_regex == "" && Benf_Iban_regex == "" && PostCode_regex == "" && FirstName_regex == "" && address1_regex == "" && TelephoneNumber_regex == "" && MobileNumber_regex == "" && address2_regex == "" && PostCode_regex == "" && AddressLine1_len == "true" && AddressLine2_len == "true")
                {
                    FirstName_regex = "true"; BranchCode_regex = "true"; Ifsc_Code_regex = "true"; Branch_regex = "true"; AccountHolderName_regex = "true"; BankCode_regex = "true";
                    address1_regex = "true"; Account_Number_regex = "true";
                    TelephoneNumber_regex = "true";
                    MobileNumber_regex = "true";
                    address2_regex = "true";
                    PostCode_regex = "true";
                    Benf_Iban_regex = "true";
                    Benf_BIC_regex = "true";
                    BranchCode_regex = "true";

                }
                if (Benf_BIC_regex != "false" && Benf_Iban_regex != "false" && BranchCode_regex != "false" && Ifsc_Code_regex != "false" && Branch_regex != "false" && AccountHolderName_regex != "false" && BankCode_regex != "false" && Account_Number_regex != "false" && Benf_Iban_regex != "false" && PostCode_regex != "false" && FirstName_regex != "false" && address1_regex != "false" && TelephoneNumber_regex != "false" && MobileNumber_regex != "false" && address2_regex != "false" && PostCode_regex != "false" && AddressLine1_len != "false" && AddressLine2_len != "false")
                {
                    string Beneficiary_Name = CompanyInfo.testInjection(obj.Beneficiary_Name);
                    //Digvijay changes for restricted keywords for beneficiary name eg.ltd,pvt,private,limited
                    MySqlCommand _cmdperm = new MySqlCommand("GetPermissions");
                    _cmdperm.CommandType = CommandType.StoredProcedure;
                    _cmdperm.Parameters.AddWithValue("_whereclause", " and PID in (182)");
                    _cmdperm.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                    DataTable dtperm = db_connection.ExecuteQueryDataTableProcedure(_cmdperm);
                    if (dtperm.Rows.Count > 0)
                    {
                        if (Convert.ToInt32(dtperm.Rows[0]["Status_ForCustomer"]) == 0)
                        {
                            var benf_name = obj.Beneficiary_Name;
                            //string benfNameCheck = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["benfNameCheck"]);
                            // Get IServiceProvider from HttpContext
                            var serviceProvider = context.RequestServices;

                            // Dynamically resolve IConfiguration
                            var configuration = serviceProvider.GetRequiredService<IConfiguration>();

                            // Get benfNameCheck from appsettings.json
                            string benfNameCheck = configuration["AppSettings:benfNameCheck"];
                            // Split the forbidden terms into an array
                            string[] forbiddenTerms = benfNameCheck.Split(',').Select(term => term.Trim()).ToArray();
                            // Check if the beneficiary name contains any forbidden terms
                            bool containsForbiddenWord = forbiddenTerms.Any(term => benf_name.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0);

                            if (containsForbiddenWord)
                            {
                                obj.Message = "InValidName";
                                string Activity1 = "<b>Add Benficiary: Beneficiary name invalid.</b> Beneficiary : " + obj.Beneficiary_Name + " Mobile: " + obj.Beneficiary_Mobile + " Country: " + obj.Beneficiary_Country_ID + " City: " + obj.Beneficiary_City_ID + "";
                                if (obj.cashcollection_flag == 1) //uncheked
                                {
                                    Activity1 = Activity1 + " Account_Number : " + obj.Account_Number + " Branch: " + obj.Branch + " BBank_ID: " + obj.BBank_ID + "";
                                }
                                int stattus = (int)CompanyInfo.InsertActivityLogDetails(Activity, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(Customer_ID), "Add Benficiary", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "Add Benficiary", context);
                                return obj;


                            }
                        }

                    }//end
                    string Beneficiary_Address = CompanyInfo.testInjection(obj.Beneficiary_Address);
                    string Beneficiary_City_ID = CompanyInfo.testInjection(Convert.ToString(obj.Beneficiary_City_ID));
                    string Beneficiary_Country_ID = CompanyInfo.testInjection(Convert.ToString(obj.Beneficiary_Country_ID));
                    string Beneficiary_Telephone = CompanyInfo.testInjection(Convert.ToString(obj.Beneficiary_Telephone));
                    string Beneficiary_Mobile = CompanyInfo.testInjection(Convert.ToString(obj.Beneficiary_Mobile));
                    string Record_Insert_DateTime = CompanyInfo.testInjection(Convert.ToString(obj.Record_Insert_DateTime));
                    string Delete_Status = CompanyInfo.testInjection(Convert.ToString(obj.Delete_Status));
                    string Created_By_User_ID = CompanyInfo.testInjection(Convert.ToString(obj.Created_By_User_ID));

                    string Customer_ID1 = CompanyInfo.testInjection(Convert.ToString(Customer_ID));
                    string Beneficiary_Address1 = CompanyInfo.testInjection(Convert.ToString(obj.Beneficiary_Address1));
                    string Beneficiary_PostCode = CompanyInfo.testInjection(Convert.ToString(obj.Beneficiary_PostCode));
                    string cashcollection_flag = CompanyInfo.testInjection(Convert.ToString(obj.cashcollection_flag));
                    string Agent_MappingID = CompanyInfo.testInjection(Convert.ToString(obj.Agent_MappingID));
                    string Client_ID = CompanyInfo.testInjection(Convert.ToString(obj.Client_ID));
                    string Branch_ID = CompanyInfo.testInjection(Convert.ToString(obj.Branch_ID));
                    string Relation_ID = CompanyInfo.testInjection(Convert.ToString(obj.Relation_ID));


                    //string Beneficiary_PostCode1 = CompanyInfo.testInjection(Convert.ToString(obj.Beneficiary_PostCode));
                    string Beneficiary_state = CompanyInfo.testInjection(Convert.ToString(obj.Beneficiary_State));
                    string AccountHolderName = CompanyInfo.testInjection(Convert.ToString(obj.AccountHolderName));
                    string Account_Number = CompanyInfo.testInjection(Convert.ToString(obj.Account_Number));
                    string BankCode = CompanyInfo.testInjection(Convert.ToString(obj.BankCode));
                    string Branch = CompanyInfo.testInjection(Convert.ToString(obj.Branch));
                    string Ifsc_Code = CompanyInfo.testInjection(Convert.ToString(obj.Ifsc_Code));
                    string BranchCode = CompanyInfo.testInjection(Convert.ToString(obj.BranchCode));
                    string Benf_Iban = CompanyInfo.testInjection(Convert.ToString(obj.Benf_Iban));
                    string Benf_BIC = CompanyInfo.testInjection(Convert.ToString(obj.Benf_BIC));
                    string sectionvalue = CompanyInfo.testInjection(Convert.ToString(obj.sectionvalue));


                    //if (Branch == "1" && Ifsc_Code == "1" && BranchCode == "1" && Benf_Iban == "1" && Benf_BIC == "1" && BankCode == "1" && Account_Number == "1" && AccountHolderName == "1" && Beneficiary_PostCode1 == "1" && Beneficiary_Name == "1" && Beneficiary_Address == "1" && Beneficiary_City_ID == "1" && Beneficiary_Country_ID == "1" &&
                    // Beneficiary_Telephone == "1" && Beneficiary_Mobile == "1" && Record_Insert_DateTime == "1" && Delete_Status == "1" &&
                    // Created_By_User_ID == "1" && Customer_ID1 == "1" && Beneficiary_Address1 == "1" && Beneficiary_PostCode == "1" &&
                    //     cashcollection_flag == "1" && Agent_MappingID == "1" && Client_ID == "1" && Branch_ID == "1" && Relation_ID == "1"
                    //     )
                    if (Branch == "1" && Ifsc_Code == "1" && BranchCode == "1" && Benf_Iban == "1" && Benf_BIC == "1" && BankCode == "1" && Account_Number == "1"
                        && AccountHolderName == "1" && PostCode_regex == "true" && Beneficiary_Name == "1" && Beneficiary_Address == "1" && Beneficiary_City_ID == "1"
                        && Beneficiary_Country_ID == "1" && Beneficiary_Telephone == "1" && Beneficiary_Mobile == "1" && Record_Insert_DateTime == "1"
                        && Delete_Status == "1" && Created_By_User_ID == "1" && Customer_ID1 == "1" && Beneficiary_Address1 == "1" && Beneficiary_PostCode == "1" &&
                        cashcollection_flag == "1" && Agent_MappingID == "1" && Client_ID == "1" && Branch_ID == "1" && Relation_ID == "1" && Beneficiary_state == "1")
                    {
                        //Anushka
                        //MySqlCommand cmd_update1 = new MySqlCommand("select * from beneficiary_configurations where Country_Id=@Country_Id and Collection_Type=@Collection_Type");
                        //cmd_update1.Parameters.AddWithValue("@Country_Id", obj.Beneficiary_Country_ID);
                        //cmd_update1.Parameters.AddWithValue("@Collection_Type", obj.Collection_type_Id);
                        MySqlConnector.MySqlCommand cmd_update1 = new MySqlConnector.MySqlCommand("SP_Beneficiary_Configuration");
                        cmd_update1.CommandType = CommandType.StoredProcedure;
                        cmd_update1.Connection = con;
                        cmd_update1.Parameters.AddWithValue("_Country_ID", obj.Beneficiary_Country_ID);
                        cmd_update1.Parameters.AddWithValue("_Collection_Type", obj.Collection_type_Id);
                        db_connection.ExecuteNonQueryProcedure(cmd_update1);
                        dtbenfconfig = db_connection.ExecuteQueryDataTableProcedure(cmd_update1);

                        MySqlConnector.MySqlCommand cmd = new MySqlConnector.MySqlCommand("SP_Update_Beneficiary");
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("_Config_name", dtbenfconfig.Rows[0]["Beneficiary_Name"]);
                        cmd.Parameters.AddWithValue("_Config_Address", dtbenfconfig.Rows[0]["Beneficiary_Address"]);
                        cmd.Parameters.AddWithValue("_Config_mobile", dtbenfconfig.Rows[0]["Beneficiary_Mobile"]);
                        cmd.Parameters.AddWithValue("_Config_city", dtbenfconfig.Rows[0]["Beneficiary_City"]);
                        cmd.Parameters.AddWithValue("_Config_country", dtbenfconfig.Rows[0]["Beneficiary_Country"]);
                        cmd.Parameters.AddWithValue("_Config_relation", dtbenfconfig.Rows[0]["Beneficiary_relation"]);

                        cmd.Parameters.AddWithValue("_Config_postcode", dtbenfconfig.Rows[0]["Beneficiary_postcode"]);
                        cmd.Parameters.AddWithValue("_Config_state", dtbenfconfig.Rows[0]["Beneficiary_state"]);

                        cmd.Parameters.AddWithValue("_Beneficiary_ID", obj.Beneficiary_ID);
                        cmd.Parameters.AddWithValue("_Beneficiary_Name", obj.Beneficiary_Name.Trim());
                        cmd.Parameters.AddWithValue("_Beneficiary_Address", obj.Beneficiary_Address.Trim());
                        cmd.Parameters.AddWithValue("_Beneficiary_City_ID", obj.Beneficiary_City_ID);
                        cmd.Parameters.AddWithValue("_Beneficiary_Country_ID", obj.Beneficiary_Country_ID);
                        cmd.Parameters.AddWithValue("_Beneficiary_Telephone", obj.Beneficiary_Telephone.Trim());
                        cmd.Parameters.AddWithValue("_Beneficiary_Mobile", obj.Beneficiary_Mobile.Trim());
                        cmd.Parameters.AddWithValue("_cashcollection_flag", obj.cashcollection_flag);
                        cmd.Parameters.AddWithValue("_Relation_ID", obj.Relation_ID);

                        cmd.Parameters.AddWithValue("_State", obj.Beneficiary_State);
                        cmd.Parameters.AddWithValue("_Beneficiary_PostCode", obj.Beneficiary_PostCode);

                        cmd.Parameters.AddWithValue("_Config_dob", dtbenfconfig.Rows[0]["DateOf_Birth"]);  // Digvijay 20-11-2024

                        if (obj.Birth_Date != "Invalid date" && obj.Birth_Date != "" && obj.Birth_Date != null && (obj.Birth_Date).ToLower() != "undefined")
                        {
                            DateTime DOB = DateTime.ParseExact(obj.Birth_Date, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                            string Birth_Date = DOB.ToString("yyyy-MM-dd HH:mm:ss");
                            cmd.Parameters.AddWithValue("_DOB", Convert.ToDateTime(Birth_Date));//cmd.Parameters.AddWithValue("_DOB", Birth_Date_DateTime);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("_DOB", null);
                        }
                        cmd.Parameters.AddWithValue("_Beneficiary_Gender", obj.Beneficiary_Gender);
                        //cmd.Parameters.AddWithValue("_Beneficiary_Gender", obj.Benf_Gender);//Digvijay changes for update gender
                        cmd.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                        cmd.Parameters.AddWithValue("_Wallet_Id", obj.Wallet_Id);
                        cmd.Parameters.AddWithValue("_Mobile_provider", obj.Mobile_provider);
                        cmd.Parameters.AddWithValue("_Wallet_provider", obj.Wallet_provider);
                        int result = cmd.ExecuteNonQuery();
                        //object result = db_connection.ExecuteScalarProcedure(cmd);

                        //result = (result == DBNull.Value) ? null : result;
                        //obj.Beneficiary_ID = Convert.ToInt32(result);
                        cmd.Dispose();

                        MySqlCommand getbenef = new MySqlCommand("Get_beneficiery");
                        getbenef.CommandType = CommandType.StoredProcedure;

                        getbenef.Parameters.AddWithValue("beneficiaryID", obj.Beneficiary_ID);

                        DataTable bt = db_connection.ExecuteQueryDataTableProcedure(getbenef);

                        if (bt.Rows.Count > 0)
                        {
                            string benefold_name = Convert.ToString(bt.Rows[0]["Beneficiary_Name"]);
                            string benefnew_name = obj.Beneficiary_Name;

                            string bname1 = Convert.ToString(benefold_name).Trim();
                            string bfname1 = bname1;
                            string blname1 = " ";
                            if (bname1.Contains(" "))
                            {
                                string[] spli = bname1.Split(' ');
                                if (spli.Length > 1)
                                {
                                    bfname1 = bname1.Substring(0, (bname1.Length - spli[spli.Length - 1].Length));
                                    blname1 = spli[spli.Length - 1];
                                }
                            }

                            string bname2 = Convert.ToString(benefnew_name).Trim();
                            string bfname2 = bname2;
                            string blname2 = " ";
                            if (bname2.Contains(" "))
                            {
                                string[] spli = bname2.Split(' ');
                                if (spli.Length > 1)
                                {
                                    bfname2 = bname2.Substring(0, (bname2.Length - spli[spli.Length - 1].Length));
                                    blname2 = spli[spli.Length - 1];
                                }
                            }


                            if (bfname1 != bfname2 || blname1 != blname2)
                            {
                                benefnamechange_flag = 1;
                            }

                        }

                        if (benefnamechange_flag == 1)
                        {
                            if (check_benef_aml_through_compliance_assist == 0)
                            {
                                try
                                {
                                    CHECK_AML(obj, obj.Beneficiary_ID, "Beneficeary Update", context);
                                }
                                catch (Exception ex)
                                {
                                }
                            }
                        }

                        if (obj.Beneficiary_ID != 0)
                        {
                            string[] check_alert = new string[7];
                            if (obj.sectionvalue == 1)
                            {
                                check_alert = AutoCheckSanctionList(obj, context);
                                int cnt_blacklisted = 0;
                                for (int i = 0; i < check_alert.Length; i++)
                                {
                                    if (check_alert[i] == "BlackListed")
                                    {
                                        cnt_blacklisted = cnt_blacklisted + 1;
                                    }
                                }
                                if (cnt_blacklisted == 0)
                                {
                                    if (obj.blacklisted == 1)
                                    {

                                        MySqlConnector.MySqlCommand cmd_sanction = new MySqlConnector.MySqlCommand("SP_Black_And_WhiteList_Benf");
                                        cmd_sanction.CommandType = CommandType.StoredProcedure;
                                        cmd_sanction.Connection = con;
                                        cmd_sanction.Parameters.AddWithValue("_benf_id", obj.Beneficiary_ID);
                                        cmd_sanction.Parameters.AddWithValue("_isblackListed", 0);
                                        cmd_sanction.Parameters.AddWithValue("_client_id", obj.Client_ID);
                                        cmd_sanction.Parameters.AddWithValue("_flag", "BlackList");
                                        int result_sanction = cmd_sanction.ExecuteNonQuery();
                                    }
                                }
                            }

                            Model.Beneficiary _Objbenef = new Model.Beneficiary();
                            _Objbenef.Beneficiary_ID = obj.Beneficiary_ID;
                            _Objbenef.Delete_Status = 0;

                            if (Convert.ToInt32(obj.BBDetails_ID) != 0)
                            {
                                int BBDetails_ID = obj.BBDetails_ID;

                            }
                            MySqlConnector.MySqlCommand cmd2 = new MySqlConnector.MySqlCommand("SP_Update_BeneficiaryBank_Details");

                            cmd2.CommandType = CommandType.StoredProcedure;
                            cmd2.Connection = con;
                            // cmd.Parameters.AddWithValue("_Id", _Objbenef.Id);
                            //Anushka
                            cmd2.Parameters.AddWithValue("_config_accname", dtbenfconfig.Rows[0]["Beneficiary_Name"]);
                            cmd2.Parameters.AddWithValue("_config_accnum", dtbenfconfig.Rows[0]["ShowAccount_No"]);
                            cmd2.Parameters.AddWithValue("_config_ifsc", dtbenfconfig.Rows[0]["IFSC"]);
                            cmd2.Parameters.AddWithValue("_config_bnkname", dtbenfconfig.Rows[0]["Bank_Name"]);
                            cmd2.Parameters.AddWithValue("_config_bankcode", dtbenfconfig.Rows[0]["Bank_Code"]);
                            cmd2.Parameters.AddWithValue("_config_branchnm", dtbenfconfig.Rows[0]["Branch"]);
                            cmd2.Parameters.AddWithValue("_config_branchcode", dtbenfconfig.Rows[0]["Branch_Code"]);
                            cmd2.Parameters.AddWithValue("_config_iban", dtbenfconfig.Rows[0]["IBAN_Status"]);
                            cmd2.Parameters.AddWithValue("_config_bic", dtbenfconfig.Rows[0]["BIC_Status"]);

                            cmd2.Parameters.AddWithValue("_Beneficiary_ID", obj.Beneficiary_ID);
                            cmd2.Parameters.AddWithValue("_AccountHolderName", obj.AccountHolderName);
                            cmd2.Parameters.AddWithValue("_BBank_ID", obj.BBank_ID);
                            cmd2.Parameters.AddWithValue("_Account_Number", obj.Account_Number);
                            cmd2.Parameters.AddWithValue("_BankCode", obj.BankCode);
                            cmd2.Parameters.AddWithValue("_Bank_Name", obj.Bank_Name);
                            cmd2.Parameters.AddWithValue("_Ifsc_Code", obj.Ifsc_Code);
                            cmd2.Parameters.AddWithValue("_BranchCode", obj.BranchCode);
                            //  cmd2.Parameters.AddWithValue("_Client_ID",obj.Client_ID);
                            cmd2.Parameters.AddWithValue("_Branch", obj.Branch);
                            cmd2.Parameters.AddWithValue("_Benf_Iban", obj.Benf_Iban);
                            cmd2.Parameters.AddWithValue("_Benf_BIC", obj.Benf_BIC);
                            //cmd2.Parameters.AddWithValue("_BBDetails_ID", 0);//parvej added
                            cmd2.Parameters.AddWithValue("_BBDetails_ID", obj.BBDetails_ID);//parvej added
                            cmd2.Parameters.Add(new MySqlConnector.MySqlParameter("_existflag", MySqlConnector.MySqlDbType.Int32));
                            cmd2.Parameters["_existflag"].Direction = ParameterDirection.Output;
                            int result2 = cmd2.ExecuteNonQuery();
                            //object result2 = db_connection.ExecuteScalarProcedure(cmd2);
                            //result2 = (result2 == DBNull.Value) ? null : result2;
                            //int i = Convert.ToInt32(result2);
                            int ExistBeneficiaryBank = Convert.ToInt32(cmd2.Parameters["_existflag"].Value);

                            if (ExistBeneficiaryBank == 0)
                            {

                                obj.Message = "exist_Beneficiary_Bank";
                                string msg = "Bank Already Exist";
                                obj.Id = 3;
                                int stattus = (int)CompanyInfo.InsertActivityLogDetailsSecurity(msg, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), 1, "srvBenificiary", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "Update Beneficiary Bank", 0, context);
                            }
                            else
                            {
                                if (result2 > 0)
                                {
                                    obj.Message = "success";
                                }
                            }
                            cmd2.Dispose();
                            string Acty = "Transcation Bank Update  : result : " + result + " , result2 : " + result2 + ", ExistBeneficiaryBank : " + ExistBeneficiaryBank;
                            int stas = (int)CompanyInfo.InsertActivityLogDetails(Acty, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(Customer_ID), "App Update Beneficiary", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), " Update Beneficiary", context);
                            if (result > 0 && result2 > 0 && ExistBeneficiaryBank != 0)
                            {
                                obj.Message = "success";
                                #region check loaction
                                DataTable dtc = CompanyInfo.get(obj.Client_ID, context);
                                try
                                {

                                    string act = "";

                                    Boolean chkLocation = true;
                                    string country_log = "";
                                    string device_ty = "";

                                    try
                                    {
                                        DataTable chkLocation1 = CompanyInfo.check_location(obj.Client_ID, "1", context);
                                        try
                                        {
                                            chkLocation = Convert.ToBoolean(chkLocation1.Rows[0]["is_valid"]);
                                            country_log = Convert.ToString(chkLocation1.Rows[0]["Country"]);
                                            device_ty = Convert.ToString(chkLocation1.Rows[0]["device_ty"]);


                                        }
                                        catch
                                        {

                                        }

                                    }
                                    catch (Exception ex)
                                    {

                                    }
                                    if (!chkLocation)
                                    {
                                        string body = string.Empty, subject = string.Empty;
                                        string body1 = string.Empty;
                                        HttpWebRequest httpRequest = null, httpRequest1 = null;
                                        DataTable d2 = (DataTable)CompanyInfo.getCustomerDetails(obj.Client_ID, Customer_ID);
                                        string sendmsg1 = "Beneficiary Added From New Location ";
                                        string company_name = Convert.ToString(dtc.Rows[0]["Company_Name"]);
                                        string URL = Convert.ToString(dtc.Rows[0]["Company_URL_Admin"]);
                                        string cust_url = Convert.ToString(dtc.Rows[0]["Company_URL_Customer"]);
                                        httpRequest = (HttpWebRequest)WebRequest.Create(URL + "Email/new-login.html");
                                        httpRequest.UserAgent = "Code Sample Web Client";
                                        HttpWebResponse webResponse = (HttpWebResponse)httpRequest.GetResponse();
                                        using (StreamReader reader = new StreamReader(webResponse.GetResponseStream()))
                                        {
                                            body = reader.ReadToEnd();
                                        }
                                        body = body.Replace("[name]", Convert.ToString(d2.Rows[0]["First_Name"]));
                                        string enc_ref = CompanyInfo.Encrypt(Convert.ToString(d2.Rows[0]["WireTransfer_ReferanceNo"]), true);
                                        string link = cust_url + "/secure-account-verfiy?reference=" + enc_ref;
                                        body = body.Replace("[link]", link);
                                        body = body.Replace("[New_Login_Detected]", "Beneficiary Added From New Location ");
                                        body = body.Replace("[country]", country_log);
                                        body = body.Replace("[time]", (Convert.ToDateTime(CompanyInfo.gettime(obj.Client_ID, context))).ToString("dddd, dd MMMM yyyy HH:mm:ss"));
                                        body = body.Replace("[location_msg]", "We noticed a new beneficiary upload to your account from new location that we don't recognise. If this wasn't you, we'll help you secure your account.");
                                        body = body.Replace("[device]", device_ty);




                                        string EmailID = "";
                                        EmailID = Convert.ToString(d2.Rows[0]["Email_ID"]);


                                        subject = company_name + " - New Beneficiary Upload location - Alert " + d2.Rows[0]["WireTransfer_ReferanceNo"];
                                        string send_mail = (string)CompanyInfo.Send_Mail(dtc, EmailID, body, subject, obj.Client_ID, obj.Branch_ID, "", "", "", context);

                                        //Notification
                                        string notification_icon = "beneficiary.jpg";
                                        string notification_message = "<span class='cls-admin'>Beneficiary <strong class='cls-new-benf'>Added From New Location </strong></span>";
                                        CompanyInfo.save_notification_compliance(notification_message, notification_icon, Convert.ToString(Customer_ID), Convert.ToDateTime(obj.Record_Insert_DateTime), obj.Client_ID, 1, 0, obj.Branch_ID, 0, 1, 1, 0, context);
                                    }
                                }
                                catch
                                {

                                }
                                #endregion
                            }

                            if (ExistBeneficiaryBank != 0)
                            {
                                #region updatetransaction 
                                //vyanakatesh change 18-12-24	
                                Acty = Acty + "update transcation 1.1";
                                try
                                {
                                    MySqlCommand cperm = new MySqlCommand("GetPermissions");
                                    cperm.CommandType = CommandType.StoredProcedure;
                                    cperm.Parameters.AddWithValue("_whereclause", " and PID in (116)");
                                    cperm.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                                    DataTable drm = db_connection.ExecuteQueryDataTableProcedure(cperm);
                                    var perstatus = Convert.ToInt32(drm.Rows[0]["Status_ForCustomer"]);

                                    Acty = Acty + ", perm status :" + perstatus + " 1.2";
                                    if (perstatus == 0)
                                    {
                                        cmd.Dispose();
                                        cmd = new MySqlCommand("select GROUP_CONCAT(Transaction_ID SEPARATOR ',') as Transaction_IDs from transaction_table where TransactionStatus_ID in (1,6) " +
                                        " and Beneficiary_ID=" + obj.Beneficiary_ID + " and PaymentDepositType_ID!=" + obj.cashcollection_flag + ";");
                                        string transactions = Convert.ToString(db_connection.ExecuteScalarProcedure(cmd));
                                        //var list = dt.AsEnumerable().Select(r => r["COLUMN1"].ToString());
                                        //string value = string.Join(",", list);

                                        Acty = Acty + ", transactions :" + transactions + " : 1.3";
                                        string transwhere = " and TransactionStatus_ID in (1,6) ";
                                        string transwhere1 = " and TransactionStatus_ID in (1,6) ";//added by vyankatesh
                                        if (transactions != null && transactions != "")
                                        {
                                            cmd.Dispose();
                                            cmd = new MySqlCommand("update Transaction_Table set PaymentDepositType_ID = '" + obj.cashcollection_flag + "' " +
                                                "where Customer_ID = " + Customer_ID + " and Beneficiary_ID=" + obj.Beneficiary_ID + " and Transaction_ID in (" + transactions + ")");
                                            int a = db_connection.ExecuteNonQueryProcedure(cmd);
                                            cmd.Dispose();

                                            transwhere = transwhere + " and t.Transaction_ID in (" + transactions + ") ";
                                            transwhere1 = transwhere1 + " and Transaction_ID in (" + transactions + ") ";
                                        }
                                        Acty = Acty + ", cashcollection_flag :" + obj.cashcollection_flag + " : 1.4";
                                        if (obj.cashcollection_flag == 1)
                                        {
                                            string _transclause = " and Customer_ID = " + Customer_ID + " and Beneficiary_ID = " + obj.Beneficiary_ID + " and TransactionStatus_ID in (1,6) and Transaction_ID not in (select Transaction_ID from transwise_benfbankdetails where Beneficiary_ID=" + obj.Beneficiary_ID + " " + transwhere1 + ")";
                                            cmd = new MySqlCommand("get_transaction_table");
                                            cmd.CommandType = CommandType.StoredProcedure;
                                            cmd.Parameters.AddWithValue("_whereclause", _transclause);

                                            DataTable dt = (DataTable)db_connection.ExecuteQueryDataTableProcedure(cmd);
                                            cmd.Dispose();

                                            Acty = Acty + ", transcation count :" + dt.Rows.Count + " : 1.5";
                                            if (dt.Rows.Count > 0)
                                            {
                                                for (int i = 0; i < dt.Rows.Count; i++)
                                                {
                                                    using (MySqlCommand cm = new MySqlCommand("Insert_BankDepositDetails"))
                                                    {
                                                        cm.CommandType = CommandType.StoredProcedure;
                                                        cm.Parameters.AddWithValue("_CB_ID", obj.Branch_ID);
                                                        cm.Parameters.AddWithValue("_BBank_ID", obj.BBank_ID);
                                                        cm.Parameters.AddWithValue("_Account_Number", obj.Account_Number);
                                                        cm.Parameters.AddWithValue("_AccountHolderName", obj.AccountHolderName);
                                                        cm.Parameters.AddWithValue("_Branch", obj.Branch);
                                                        cm.Parameters.AddWithValue("_BranchCode", obj.BranchCode);
                                                        cm.Parameters.AddWithValue("_BankCode", obj.BankCode);
                                                        cm.Parameters.AddWithValue("_Ifsc_Code", obj.Ifsc_Code);
                                                        cm.Parameters.AddWithValue("_Beneficiary_Country_ID", obj.Beneficiary_Country_ID);
                                                        cm.Parameters.AddWithValue("_Beneficiary_City_ID", obj.Beneficiary_City_ID);
                                                        cm.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                                                        cm.Parameters.AddWithValue("_Customer_ID", Customer_ID);
                                                        cm.Parameters.AddWithValue("_Transaction_ID", Convert.ToString(dt.Rows[0]["Transaction_ID"]));
                                                        cm.Parameters.AddWithValue("_Benf_Iban", obj.Benf_Iban);
                                                        cm.Parameters.AddWithValue("_Benf_BIC", obj.Benf_BIC);
                                                        int bankmsgss = db_connection.ExecuteNonQueryProcedure(cm);
                                                    }
                                                }
                                            }
                                        }

                                        Acty = Acty + "  1.6";
                                        cmd = new MySqlCommand("update transwise_benfbankdetails tb inner join transaction_table t on tb.Transaction_ID=t.Transaction_ID  set AccountHolderName = '" + obj.AccountHolderName.Replace("'", "''").Trim() + "' , BBank_ID = '" + obj.BBank_ID + "' , " +
                                                "Beneficiary_City_ID = '" + obj.Beneficiary_City_ID + "' , Account_Number= '" + obj.Account_Number + "' , BankCode = '" + obj.BankCode + "' ,Branch = '" + obj.Branch.Replace("'", "''").Trim() + "'," +
                                                " Ifsc_Code ='" + obj.Ifsc_Code + "' , BranchCode = '" + obj.BranchCode + "', Iban_ID= '" + obj.Benf_Iban + "', BIC_Code ='" + obj.Benf_BIC + "' " +
                                                "where t.Customer_ID = " + Customer_ID + " and t.Beneficiary_ID=" + obj.Beneficiary_ID + " " + transwhere + "");
                                        int a1 = db_connection.ExecuteNonQueryProcedure(cmd);
                                        if (a1 > 0)
                                        {
                                            Acty = Acty + ", a1 count :" + a1 + " : 1.7";
                                            var Acts = " Beneficiary bank details update against transaction. Whereclause : t.Customer_ID = " + Customer_ID + " and t.Beneficiary_ID=" + obj.Beneficiary_ID + " and transaction list " + transwhere;
                                            int stattus = (int)CompanyInfo.InsertActivityLogDetailsSecurity(Acts, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), 1, "srvBenificiary", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "Update Beneficiary app", 0, context);
                                        }
                                    }
                                }
                                catch (Exception _x)
                                {
                                    Acty = Acty + ", exception: 1.8";
                                    Acty = Acty + _x.Message.ToString();
                                    //string stattus = (string)CompanyInfo.InsertErrorLogDetails(_x.Message.Replace("\'", "\\'"), 0, "Update Beneficiary app", obj.Branch_ID, obj.Client_ID);
                                }
                                finally
                                {
                                    int stattus = (int)CompanyInfo.InsertActivityLogDetails(Acty, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(Customer_ID), "Update", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "Update Beneficiary app", context);
                                }
                                #endregion updatetransaction
                            }

                            try
                            {
                                #region beneficiary probable match
                                obj.whereclause = " Beneficiary_ID != " + obj.Beneficiary_ID + " and Beneficiary_Country_ID =" + obj.Beneficiary_Country_ID + " and Beneficiary_City_ID =" + obj.Beneficiary_City_ID + " and bm.Client_ID= " + obj.Client_ID;
                                if (obj.Birth_Date != "" && obj.Birth_Date != null && obj.Birth_Date != "null")
                                {
                                    obj.whereclause = obj.whereclause + " and (date(DateOf_Birth) =date(" + obj.Birth_Date + ") or 1=1 ) ";
                                }
                                if (obj.Beneficiary_Address != "" && obj.Beneficiary_Address != null && obj.Beneficiary_Address != "null")
                                {
                                    obj.whereclause = obj.whereclause + " AND   (Beneficiary_Address = '" + obj.Beneficiary_Address + "' or 1=1)";
                                }
                                MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("Beneficiary_probable_match");
                                _cmd.CommandType = CommandType.StoredProcedure;
                                _cmd.Parameters.AddWithValue("_whereclause", obj.whereclause);
                                _cmd.Parameters.AddWithValue("_first_name", obj.First_Name);
                                _cmd.Parameters.AddWithValue("_middle_name", obj.Middle_Name);
                                _cmd.Parameters.AddWithValue("_last_name", obj.Last_Name);
                                DataTable dsp = db_connection.ExecuteQueryDataTableProcedure(_cmd);
                                int n = dsp.Rows.Count;
                                if (n > 0)
                                {
                                    try
                                    {
                                        n = (int)(dsp.Rows[0]["Beneficiary_ID"]);
                                        MySqlConnector.MySqlCommand cmd_update = new MySqlConnector.MySqlCommand("update beneficiary_master set ProbableMatch_Flag=0,Matching_ID=@Matching_ID  where Beneficiary_ID=@Beneficiary_Id and Client_ID=@Client_ID");
                                        cmd_update.Parameters.AddWithValue("@Matching_ID", n);
                                        cmd_update.Parameters.AddWithValue("@Beneficiary_Id", obj.Beneficiary_ID);
                                        cmd_update.Parameters.AddWithValue("@Client_ID", obj.Client_ID);
                                        db_connection.ExecuteNonQueryProcedure(cmd_update);
                                        //string msg = "probable";
                                    }
                                    catch (Exception ex)
                                    {
                                        Activity = "";
                                        Activity = " Beneficiary probable match found ";
                                    }
                                    //string Activity = "";
                                    //Activity = " Beneficiary probable match found ";
                                    //mtsmethods.InsertActivityLogDetails_Beneficiary(Activity, user_id, 0, user_id, cust_id, "UpdateBenf_Details", c.Branch_ID, c.Client_ID, c.Beneficiary_ID);
                                    Activity = "Update Benficiary: Probable match found for beneficiary " + obj.First_Name + " " + obj.Middle_Name + " " + obj.Last_Name + ".";
                                    int stattus = (int)CompanyInfo.InsertActivityLogDetails(Activity, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(Customer_ID), "Update", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "Update Beneficiary", context);
                                }
                                #endregion
                            }
                            catch (Exception exb)
                            {
                                //int stattuspro = (int)CompanyInfo.InsertActivityLogDetailsSecurity("beneficiary probable match error:" + exb.ToString(), Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), 1, "Update", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "Update Beneficiary", 0, context);
                            }
                            #region insert into Benef Collection Mapping table
                            if (obj.Beneficiary_ID > 0)
                            {
                                try
                                {
                                    MySqlCommand cmdaw = new MySqlCommand("Insert_benef_collectiontype_mapping");
                                    cmdaw.CommandType = CommandType.StoredProcedure;
                                    cmdaw.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                                    cmdaw.Parameters.AddWithValue("_Branch_ID", obj.Branch_ID);
                                    cmdaw.Parameters.AddWithValue("_User_ID", obj.Created_By_User_ID);

                                    cmdaw.Parameters.AddWithValue("_Customer_ID", Customer_ID);
                                    cmdaw.Parameters.AddWithValue("_Beneficiary_ID", obj.Beneficiary_ID);

                                    cmdaw.Parameters.AddWithValue("_Collection_type_Id", obj.Collection_type_Id);
                                    cmdaw.Parameters.AddWithValue("_Record_Insert_DateTime", obj.Record_Insert_DateTime);

                                    cmdaw.Parameters.Add(new MySqlParameter("_existflag", MySqlDbType.Int32));
                                    cmdaw.Parameters["_existflag"].Direction = ParameterDirection.Output;

                                    int p = db_connection.ExecuteNonQueryProcedure(cmdaw);

                                    var insertflag = Convert.ToInt32(cmdaw.Parameters["_existflag"].Value);

                                    string Activitys1 = "Already Exist Bnef Collection Mapping Customer Id : " + Customer_ID + ",Beneficiary_ID : " + obj.Beneficiary_ID + ",Collection Type Id : " + obj.Collection_type_Id + ", User ID :" + obj.Created_By_User_ID;
                                    if (insertflag > 0)
                                    {
                                        Activitys1 = "Successfully Insert Into Benef Collection Mapping Customer Id : " + Customer_ID + ",Beneficiary_ID : " + obj.Beneficiary_ID + ",Collection Type Id : " + obj.Collection_type_Id + ", User ID :" + obj.Created_By_User_ID;
                                    }
                                    int stattus = (int)CompanyInfo.InsertActivityLogDetails(Activitys1, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(Customer_ID), "App Update Beneficiary", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), " Update Beneficiary", context);
                                }
                                catch (Exception ex)
                                {

                                    string stattus = (string)CompanyInfo.InsertErrorLogDetails(ex.Message.Replace("\'", "\\'"), 0, "App Update Beneficiary", obj.Branch_ID, obj.Client_ID);

                                }
                            }
                            #endregion
                            //if (_Objbenef.Beneficiary_ID != 0)
                            //{
                            //    Model.ActivityLogTracker _LogTracker = new Model.ActivityLogTracker();
                            //    string _benefDetails = "Beneficiary Details:  Beneficiary Name:" + obj.Beneficiary_Name + " Mobile No: " + obj.Beneficiary_Mobile;
                            //    string _bankDetails = "Bank Details: Bank Name: " + _Objbenef.Bank_Name + " Account Holder: " + _Objbenef.AccountHolderName + "  Account Number: " + _Objbenef.Account_Number;
                            //    if (_Objbenef.Ifsc_Code != null && _Objbenef.Ifsc_Code != "")
                            //    {
                            //        _bankDetails = _bankDetails + " IFSC Code: " + _Objbenef.Ifsc_Code;
                            //    }
                            //    if (_Objbenef.Bank_Name != null && _Objbenef.Bank_Name != "")
                            //    {
                            //        _bankDetails = _bankDetails + " Branch: " + _Objbenef.Bank_Name;
                            //    }
                            //    if (_Objbenef.BankCode != null && _Objbenef.BankCode != "")
                            //    {
                            //        _bankDetails = _bankDetails + " Branch Code: " + _Objbenef.BankCode;
                            //    }

                            //    _LogTracker.Activity = " Beneficiary updated successfully " + obj.Bank_Name + ". </br>" + _benefDetails + "</br>" + _bankDetails + " .";
                            //    _LogTracker.Customer_ID = Customer_ID;
                            //    _LogTracker.FunctionName = "save beneficiary and bank details";


                            //    Service.srvActivityLogTracker srvLogTracker = new srvActivityLogTracker();
                            //    int i_result = srvLogTracker.Create(_LogTracker);

                            //}

                            Activity = "Beneficiary Updated.  </br>";
                            //Activity = "<b>" + Username + "</b>" + "Beneficiary Updated.  </br>";
                            int stattus1 = (int)CompanyInfo.InsertActivityLogDetails(Activity, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(Customer_ID), "Update", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "Update Beneficiary", context);

                        }
                    }
                    else
                    {
                        string msg = "SQl Enjection detected . security_code=0";
                        obj.Id = -1;
                        int stattus = (int)CompanyInfo.InsertActivityLogDetailsSecurity(msg, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), 1, "srvBenificiary", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "Update", 0, context);
                    }

                }
                else
                {
                    obj.Id = 1;
                    string msg = "Validation Failed " + " <br/>  " + error_invalid_data;
                    obj.whereclause = "Validation Failed";
                    obj.Message = error_msg;
                    int stattus = (int)CompanyInfo.InsertActivityLogDetails(msg, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), 1, "srvBenificiary", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "Create", context);
                }
            }

            return obj;
            //    return obj.Beneficiary_ID;
        }

        public string CHECK_AML(Model.Beneficiary obj, int benef_id, string CallFrom, HttpContext context)  // Complience  Assist AMl Check Rushikesh.
        {

            int beneficiery_id = benef_id;
            // double amount = amountgbp;
            string aml_result = "0";
            int flag1 = 0, flag2 = 0, flag4 = 0, flag5 = 0, flag6 = 0, flag7 = 0;
            int flag = 0;
            string monitorCommand = "";

            MySqlCommand cmdupdate1 = new MySqlCommand("Get_Permissions");
            cmdupdate1.CommandType = CommandType.StoredProcedure;
            cmdupdate1.Parameters.AddWithValue("Per_ID", 204);
            cmdupdate1.Parameters.AddWithValue("ClientID", obj.Client_ID);
            DataTable pm = db_connection.ExecuteQueryDataTableProcedure(cmdupdate1);
            if (pm.Rows.Count > 0)
            {
                if (Convert.ToString(pm.Rows[0]["Status_ForCustomer"]) == "0")
                {
                    MySqlCommand getbenef = new MySqlCommand("Get_beneficiery");
                    getbenef.CommandType = CommandType.StoredProcedure;

                    getbenef.Parameters.AddWithValue("beneficiaryID", beneficiery_id);

                    DataTable bt = db_connection.ExecuteQueryDataTableProcedure(getbenef);

                    try
                    {
                        //Insert Bank details
                        if (bt.Rows.Count > 0)
                        {
                            string Transaction_ID = "";//transactionID;
                            int BBank_ID = Convert.ToInt32(bt.Rows[0]["BBank_ID"]);
                            string Account_Number = Convert.ToString(bt.Rows[0]["Account_Number"]);
                            string AccountHolderName = Convert.ToString(bt.Rows[0]["AccountHolderName"]);
                            string Branch = Convert.ToString(bt.Rows[0]["Branch"]);
                            string BranchCode = Convert.ToString(bt.Rows[0]["BranchCode"]);
                            string Bank_code = Convert.ToString(bt.Rows[0]["BankCode"]);
                            string Ifsc_Code = Convert.ToString(bt.Rows[0]["Ifsc_Code"]);
                            int Country_ID = Convert.ToInt32(bt.Rows[0]["Beneficiary_Country_ID"]);
                            int City_ID = Convert.ToInt32(bt.Rows[0]["Beneficiary_City_ID"]);
                            string Benf_Iban = Convert.ToString(bt.Rows[0]["Iban_ID"]);
                            string BIC_Code = Convert.ToString(bt.Rows[0]["BIC_Code"]);
                            string formattedDateOfBirth = "";
                            //reset to previous value
                            //int Country_ID = 0;//Convert.ToInt32(dictObjMain["Country_ID"]);
                            string name = bt.Rows[0]["Beneficiary_Name"].ToString();


                            string bname = Convert.ToString(name).Trim(); string bfname = bname; string blname = " ";
                            if (bname.Contains(" "))
                            {
                                string[] spli = bname.Split(' ');
                                if (spli.Length > 1) { bfname = bname.Substring(0, (bname.Length - spli[spli.Length - 1].Length)); blname = spli[spli.Length - 1]; }
                            }
                            try
                            {
                                DateTime dateOfBirth = Convert.ToDateTime(bt.Rows[0]["DateOf_Birth"]);
                                if (dateOfBirth != null)
                                {
                                    formattedDateOfBirth = dateOfBirth.ToString("yyyy-MM-dd");

                                }
                            }
                            catch { }

                            string Beneficiary_Address = Convert.ToString(bt.Rows[0]["Beneficiary_Address"]);
                            string Beneficiary_Address1 = Convert.ToString(bt.Rows[0]["Beneficiary_Address1"]);
                            string City_Name = Convert.ToString(bt.Rows[0]["City_Name"]);
                            string Country_Name = Convert.ToString(bt.Rows[0]["Country_Name"]);
                            string Beneficiary_PostCode = Convert.ToString(bt.Rows[0]["Beneficiary_PostCode"]);
                            string Customer_ID = Convert.ToString(bt.Rows[0]["Customer_ID"]);
                            int Client_ID = Convert.ToInt32(bt.Rows[0]["Client_ID"]);


                            string Record_Insert_DateTime = CompanyInfo.gettime(Client_ID, context); //Convert.ToString(bt.Rows[0]["Beneficiary_PostCode"]);

                            int User_ID = Convert.ToInt32(bt.Rows[0]["Created_By_User_ID"]);
                            int Branch_ID = Convert.ToInt32(bt.Rows[0]["Branch_ID"]);

                            MySqlCommand cmdp_active = new MySqlCommand("active_thirdparti_aml_api");
                            cmdp_active.CommandType = CommandType.StoredProcedure;
                            string whereclause = "API_ID =" + 10;
                            cmdp_active.Parameters.AddWithValue("_whereclause", whereclause);

                            DataTable dtApi = db_connection.ExecuteQueryDataTableProcedure(cmdp_active);


                            if (dtApi.Rows.Count > 0)
                            {
                                string rooturl = "";
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
                                var Request_Type_Code = "";
                                string bodyJson = "";
                                string RefNum = "";
                                string requestStatus = "";

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
                                Random random = new Random();
                                string referenceCode = RandomString(2, random) + RandomNumber(2, random) + beneficiery_id;

                                Request_Type_Code = Request_Type_Code_Benef;



                                try
                                {


                                    MySqlCommand cmdp_active5 = new MySqlCommand("Check_AML_history_Beneficiery");
                                    cmdp_active5.CommandType = CommandType.StoredProcedure;
                                    whereclause = "API_ID =" + 10;
                                    cmdp_active5.Parameters.AddWithValue("_Beneficiary_ID", beneficiery_id);
                                    cmdp_active5.Parameters.AddWithValue("_Whereclause", whereclause);

                                    DataTable dtApi10 = db_connection.ExecuteQueryDataTableProcedure(cmdp_active5);




                                    try
                                    {


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



                                        string accessToken = jsonResponse["access_token"].ToString();



                                        // Assuming you have already obtained the accessToken variable from the previous step

                                        url = _url;//"https://web-u.complianceassist.co.uk/api/v2_0/";



                                        if (dtApi10.Rows.Count > 0)
                                        {

                                            RefNum = Convert.ToString(dtApi10.Rows[0]["Beneficiary_Ref"]);
                                            try
                                            {

                                                client = new RestClient(url + "subjects?reference=" + RefNum);
                                                client.Timeout = -1;
                                                request = new RestRequest(Method.GET);
                                                request.AddHeader("Authorization", "Bearer " + accessToken);
                                                request.AddHeader("Accept", "application/json");

                                                response1 = client.Execute(request);


                                                // Parse the JSON response
                                                jsonResponse = JObject.Parse(response1.Content);


                                                string status3 = (string)jsonResponse["status"];
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
                                                        }

                                                    }
                                                }

                                                if (status3 == "success")
                                                {
                                                    monitorCommand = "CHANGE";
                                                    referenceCode = RefNum;
                                                }
                                                else
                                                {
                                                    monitorCommand = "ADD";
                                                }



                                            }

                                            catch (Exception ex)
                                            {
                                                string stattus = (string)CompanyInfo.InsertErrorLogDetails(ex.Message.Replace("\'", "\\'"), 0, "Complience Assist AMl Check - 4th api call (Subject Information)  ", Branch_ID, Client_ID);
                                                monitorCommand = "ADD";
                                            }



                                        }
                                        else
                                        {
                                            monitorCommand = "ADD";
                                        }

                                        bodyJson = @"
{
    ""requestType"": """ + Request_Type_Code + @""",
    ""subjectType"": ""INDIVIDUAL"",
  ""subjectReference"": """ + referenceCode + @""",
 ""requestReference"": ""Onboarding " + referenceCode + @""",
    ""subjectDetails"": {
        ""firstName"": """ + bfname + @""",
        ""surname"": """ + blname + @""",
        ""dateOfBirth"": """ + formattedDateOfBirth + @""",
        
        ""houseNumber"": """ + 0 + @""",
        ""addressLine1"":""" + Beneficiary_Address + @""",
        ""addressLine2"": """ + Beneficiary_Address + @""",
        ""city"":""" + City_Name + @""",
        ""postcode"": """ + Beneficiary_PostCode + @""",
        ""country"": """ + Country_Name + @"""
    },
    ""notes"": ""Onboarded under project Alpha."",
     ""monitorCommand"": """ + monitorCommand + @""",
    ""callbackUrl"": """"
}
";




                                        try
                                        {
                                            ServicePointManager.Expect100Continue = true;
                                            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
                                                   | SecurityProtocolType.Tls11
                                                   | SecurityProtocolType.Tls12
                                                   | SecurityProtocolType.Ssl3;
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



                                            string status = (string)jsonResponse["status"];
                                            int apiRequestId = (int)jsonResponse["apiRequestId"];
                                            DateTime dateTime = (DateTime)jsonResponse["dateTime"];
                                            long requestId = (long)jsonResponse["requestId"];
                                            string requestReference = (string)jsonResponse["requestReference"];
                                            long subjectId = (long)jsonResponse["subjectId"];
                                            string subjectReference = (string)jsonResponse["subjectReference"];
                                            requestStatus = (string)jsonResponse["requestStatus"];



                                            // Extract nested properties under "results"
                                            JObject resultsObj = (JObject)jsonResponse["results"];
                                            JObject watchlistsResultsObj = (JObject)resultsObj["watchlistsResults"];
                                            int totalNumberOfMatches = (int)watchlistsResultsObj["totalNumberOfMatches"];

                                            // Extract array values under "matchStatuses"
                                            JArray matchStatusesArr = (JArray)watchlistsResultsObj["matchStatuses"];
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
                                            }
                                            else
                                            {
                                                remark = 1;
                                            }

                                            string request3 = url + "requests" + bodyJson;
                                            CallFrom = "Compliance_Request_benef";

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
                                                _cmd.Parameters.AddWithValue("_Branch_ID", Branch_ID);
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
                                            CallFrom = "Complience_Response_benef";
                                            try
                                            {
                                                MySqlCommand _cmd = new MySqlCommand("SaveAPIRequestResponce");
                                                _cmd.CommandType = CommandType.StoredProcedure;
                                                _cmd.Parameters.AddWithValue("_API_ID", 10);
                                                _cmd.Parameters.AddWithValue("_Client_ID", Client_ID);
                                                _cmd.Parameters.AddWithValue("_Customer_ID", Customer_ID);
                                                _cmd.Parameters.AddWithValue("_status", 1);
                                                _cmd.Parameters.AddWithValue("_Function_name", CallFrom);
                                                _cmd.Parameters.AddWithValue("_Remark", remark);
                                                _cmd.Parameters.AddWithValue("_comments", jsonResponse);
                                                _cmd.Parameters.AddWithValue("_Record_Insert_DateTime", Record_Insert_DateTime);
                                                _cmd.Parameters.AddWithValue("_Branch_ID", Branch_ID);
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

                                            DataTable dtb = CompanyInfo.get(Client_ID, context);
                                            if (dtb.Rows.Count > 0)
                                            {
                                                //CURL = Convert.ToString(dtb.Rows[0]["RootURL"]);
                                                //Company_Name = Convert.ToString(dtb.Rows[0]["Company_Name"]);

                                                rooturl = Convert.ToString(dtb.Rows[0]["RootURL"]);
                                            }
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
                                                        string filePath = "assets/Other_Docs/" + "PDF-" + referenceCode + "-" + Record_Insert_DateTime.Replace(":", "") + ".pdf";
                                                        string URL = rooturl + filePath;
                                                        try
                                                        {
                                                            try
                                                            {
                                                                File.WriteAllBytes(URL, pdfData);
                                                                Console.WriteLine("PDF saved successfully at: " + filePath);

                                                            }
                                                            catch (Exception ex)
                                                            {

                                                            }


                                                            CallFrom = "Compliance_Request_benef";

                                                            string pdfdownload = "0";

                                                            request3 = url + "requests" + bodyJson;

                                                            MySqlCommand _cmd = new MySqlCommand("Insert_benef_aml_detail");
                                                            _cmd.CommandType = CommandType.StoredProcedure;
                                                            _cmd.Parameters.AddWithValue("p_Beneficiary_ID", beneficiery_id);
                                                            _cmd.Parameters.AddWithValue("p_Beneficiary_Name", bfname + " " + blname);
                                                            _cmd.Parameters.AddWithValue("p_PDFGenerate_Status", pdfdownload);
                                                            _cmd.Parameters.AddWithValue("p_PDF_FileName", filePath);
                                                            _cmd.Parameters.AddWithValue("p_Record_Insert_DateTime", Record_Insert_DateTime);
                                                            _cmd.Parameters.AddWithValue("p_Beneficiary_AML_Status", requestStatus);
                                                            _cmd.Parameters.AddWithValue("p_Beneficiary_Ref", referenceCode);
                                                            _cmd.Parameters.AddWithValue("p_Client_ID", Client_ID);
                                                            _cmd.Parameters.AddWithValue("p_Branch_Id", Branch_ID);
                                                            _cmd.Parameters.AddWithValue("p_Delete_Status", 0);
                                                            _cmd.Parameters.AddWithValue("p_Customer_ID", Customer_ID);
                                                            _cmd.Parameters.AddWithValue("p_Parameter", request3);
                                                            _cmd.Parameters.AddWithValue("p_APICall_From", CallFrom);
                                                            _cmd.Parameters.AddWithValue("p_Response_Code", remark);
                                                            _cmd.Parameters.AddWithValue("p_RequestResponse_Flag", 0);
                                                            _cmd.Parameters.AddWithValue("p_API_ID", 10);
                                                            _cmd.Parameters.AddWithValue("p_Transaction_Ref", 0);
                                                            _cmd.Parameters.AddWithValue("p_AML_check_profile", Request_Type_Code);
                                                            int msg1 = db_connection.ExecuteNonQueryProcedure(_cmd);


                                                            CallFrom = "Compliance_Response_benef";


                                                            _cmd = new MySqlCommand("Insert_benef_aml_detail");
                                                            _cmd.CommandType = CommandType.StoredProcedure;
                                                            _cmd.Parameters.AddWithValue("p_Beneficiary_ID", beneficiery_id);
                                                            _cmd.Parameters.AddWithValue("p_Beneficiary_Name", bfname + " " + blname);
                                                            _cmd.Parameters.AddWithValue("p_PDFGenerate_Status", pdfdownload);
                                                            _cmd.Parameters.AddWithValue("p_PDF_FileName", filePath);
                                                            _cmd.Parameters.AddWithValue("p_Record_Insert_DateTime", Record_Insert_DateTime);
                                                            _cmd.Parameters.AddWithValue("p_Beneficiary_AML_Status", requestStatus);
                                                            _cmd.Parameters.AddWithValue("p_Beneficiary_Ref", referenceCode);
                                                            _cmd.Parameters.AddWithValue("p_Client_ID", Client_ID);
                                                            _cmd.Parameters.AddWithValue("p_Branch_Id", Branch_ID);
                                                            _cmd.Parameters.AddWithValue("p_Delete_Status", 0);
                                                            _cmd.Parameters.AddWithValue("p_Customer_ID", Customer_ID);
                                                            _cmd.Parameters.AddWithValue("p_Parameter", jsonResponse);
                                                            _cmd.Parameters.AddWithValue("p_APICall_From", CallFrom);
                                                            _cmd.Parameters.AddWithValue("p_Response_Code", remark);
                                                            _cmd.Parameters.AddWithValue("p_RequestResponse_Flag", 1);
                                                            _cmd.Parameters.AddWithValue("p_API_ID", 10);
                                                            _cmd.Parameters.AddWithValue("p_Transaction_Ref", 0);
                                                            _cmd.Parameters.AddWithValue("p_AML_check_profile", Request_Type_Code);
                                                            msg1 = db_connection.ExecuteNonQueryProcedure(_cmd);

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
                                                string stattus = (string)CompanyInfo.InsertErrorLogDetails(ex.Message.Replace("\'", "\\'"), User_ID, "Complience Assist AMl Check - 2st api call (PDF download)  ", Branch_ID, Client_ID);

                                            }
                                            if (flag > 0)
                                            {
                                                CallFrom = "Compliance_PEP_Sanction";

                                                try
                                                {
                                                    MySqlCommand _cmd = new MySqlCommand("SaveAPIRequestResponce");
                                                    _cmd.CommandType = CommandType.StoredProcedure;
                                                    _cmd.Parameters.AddWithValue("_API_ID", 10);
                                                    _cmd.Parameters.AddWithValue("_Client_ID", Client_ID);
                                                    _cmd.Parameters.AddWithValue("_Customer_ID", Customer_ID);
                                                    _cmd.Parameters.AddWithValue("_status", 0);
                                                    _cmd.Parameters.AddWithValue("_Function_name", CallFrom);
                                                    _cmd.Parameters.AddWithValue("_Remark", 1);
                                                    _cmd.Parameters.AddWithValue("_comments", Convert.ToString(jsonResponse));
                                                    _cmd.Parameters.AddWithValue("_Record_Insert_DateTime", Record_Insert_DateTime);
                                                    _cmd.Parameters.AddWithValue("_Branch_ID", Branch_ID);
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





                                                if (flag1 > 0 && flag2 > 0)
                                                {
                                                    try
                                                    {
                                                        MySqlCommand cmd_update = new MySqlCommand("UPDATE beneficiary_master SET PEPAndSanctions = '3' WHERE Beneficiary_ID = @Bneficiary_ID");
                                                        cmd_update.Parameters.AddWithValue("@Bneficiary_ID", benef_id);
                                                        db_connection.ExecuteNonQueryProcedure(cmd_update);


                                                        //alert fro pep and sanctions.
                                                        string description1 = "Customer Found in Pep AND Sanctions";
                                                        // string Record_DateTime = "0";
                                                        string notification_icon = "pep-match-not-found.jpg";
                                                        //notification_message = "<span class='cls-admin'> - <span class='cls-cancel'><strong> International Sanctions Alert  </strong>- </span><span class='cls-addr-confirm'>" + description1 + " </span></span></span><span class='cls-customer'></span>";
                                                        string notification_message = "<span class='cls-admin'> - <span class='cls-cancel'><strong> International Pep and Sanctions Alert </strong>- </span><span class='cls-addr-confirm'>" + description1 + " </span></span></span><span class='cls-customer'></span>";
                                                        CompanyInfo.save_notification(notification_message, notification_icon, Convert.ToInt32(Customer_ID), Convert.ToDateTime(Record_Insert_DateTime), Convert.ToInt32(Client_ID), 1, User_ID, Convert.ToInt32(Branch_ID), 0, 0, 1, 0, context);



                                                    }
                                                    catch (Exception ex)
                                                    {
                                                    }

                                                }
                                                else if (flag1 > 0)
                                                {
                                                    try
                                                    {
                                                        MySqlCommand cmd_update = new MySqlCommand("UPDATE beneficiary_master SET PEPAndSanctions = '1' WHERE Beneficiary_ID = @Bneficiary_ID");
                                                        cmd_update.Parameters.AddWithValue("@Bneficiary_ID", benef_id);
                                                        db_connection.ExecuteNonQueryProcedure(cmd_update);

                                                        //alert for pep
                                                        string description1 = "Customer Found In pep";
                                                        string Record_DateTime = Record_Insert_DateTime;
                                                        string notification_icon = "aml-referd.jpg";
                                                        //string notification_message = "<span class='cls-admin'> International PEP Alert - <strong class='cls-cancel'></strong><br/>" + description1 + "</span><span class='cls-customer'></span>";
                                                        string notification_message = "<span class='cls-admin'> - <span class='cls-cancel'><strong> International PEP Alert </strong>- </span><span class='cls-addr-confirm'>" + description1 + " </span></span></span><span class='cls-customer'></span>";
                                                        CompanyInfo.save_notification(notification_message, notification_icon, Convert.ToInt32(Customer_ID), Convert.ToDateTime(Record_DateTime), Client_ID, 1, User_ID, Branch_ID, 0, 1, 1, 0, context);


                                                    }
                                                    catch { }
                                                }
                                                else if (flag2 > 0)
                                                {
                                                    try
                                                    {
                                                        MySqlCommand cmd_update = new MySqlCommand("UPDATE beneficiary_master SET PEPAndSanctions = '2' WHERE Beneficiary_ID = @Bneficiary_ID");
                                                        cmd_update.Parameters.AddWithValue("@Bneficiary_ID", benef_id);
                                                        db_connection.ExecuteNonQueryProcedure(cmd_update);

                                                        //alert for sanction.
                                                        string description1 = "Customer Found in Sanctions";
                                                        string Record_DateTime = Record_Insert_DateTime;
                                                        string notification_icon = "pep-match-not-found.jpg";
                                                        //notification_message = "<span class='cls-admin'> - <span class='cls-cancel'><strong> International Sanctions Alert  </strong>- </span><span class='cls-addr-confirm'>" + description1 + " </span></span></span><span class='cls-customer'></span>";
                                                        string notification_message = "<span class='cls-admin'> - <span class='cls-cancel'><strong> International Sanctions Alert </strong>- </span><span class='cls-addr-confirm'>" + description1 + " </span></span></span><span class='cls-customer'></span>";
                                                        CompanyInfo.save_notification(notification_message, notification_icon, Convert.ToInt32(Customer_ID), Convert.ToDateTime(Record_Insert_DateTime), Convert.ToInt32(Client_ID), 1, User_ID, Convert.ToInt32(Branch_ID), 0, 0, 1, 0, context);


                                                    }
                                                    catch { }
                                                }
                                                else if (flag4 > 0)
                                                {
                                                    try
                                                    {
                                                        MySqlCommand cmd_update = new MySqlCommand("UPDATE beneficiary_master SET PEPAndSanctions = '4' WHERE Beneficiary_ID = @Bneficiary_ID");
                                                        cmd_update.Parameters.AddWithValue("@Bneficiary_ID", benef_id);
                                                        db_connection.ExecuteNonQueryProcedure(cmd_update);

                                                        //alert for sanction.
                                                        string description1 = "adverse media matches";
                                                        string Record_DateTime = Record_Insert_DateTime;
                                                        string notification_icon = "pep-match-not-found.jpg";
                                                        //notification_message = "<span class='cls-admin'> - <span class='cls-cancel'><strong> International Sanctions Alert  </strong>- </span><span class='cls-addr-confirm'>" + description1 + " </span></span></span><span class='cls-customer'></span>";
                                                        string notification_message = "<span class='cls-admin'> - <span class='cls-cancel'><strong> Beneficiery found in adverse media matches </strong>- </span><span class='cls-addr-confirm'>" + description1 + " </span></span></span><span class='cls-customer'></span>";
                                                        CompanyInfo.save_notification(notification_message, notification_icon, Convert.ToInt32(Customer_ID), Convert.ToDateTime(Record_Insert_DateTime), Convert.ToInt32(Client_ID), 1, User_ID, Convert.ToInt32(Branch_ID), 0, 0, 1, 0, context);


                                                    }
                                                    catch { }
                                                }
                                                else if (flag5 > 0)
                                                {
                                                    try
                                                    {
                                                        MySqlCommand cmd_update = new MySqlCommand("UPDATE beneficiary_master SET PEPAndSanctions = '4' WHERE Beneficiary_ID = @Bneficiary_ID");
                                                        cmd_update.Parameters.AddWithValue("@Bneficiary_ID", benef_id);
                                                        db_connection.ExecuteNonQueryProcedure(cmd_update);

                                                        //alert for sanction.
                                                        string description1 = "Customer Found in Sanctions";
                                                        string Record_DateTime = Record_Insert_DateTime;
                                                        string notification_icon = "pep-match-not-found.jpg";
                                                        //notification_message = "<span class='cls-admin'> - <span class='cls-cancel'><strong> International Sanctions Alert  </strong>- </span><span class='cls-addr-confirm'>" + description1 + " </span></span></span><span class='cls-customer'></span>";
                                                        string notification_message = "<span class='cls-admin'> - <span class='cls-cancel'><strong> Beneficiery found in (relatives or close associates) matches Alert </strong>- </span><span class='cls-addr-confirm'>" + description1 + " </span></span></span><span class='cls-customer'></span>";
                                                        CompanyInfo.save_notification(notification_message, notification_icon, Convert.ToInt32(Customer_ID), Convert.ToDateTime(Record_Insert_DateTime), Convert.ToInt32(Client_ID), 1, User_ID, Convert.ToInt32(Branch_ID), 0, 0, 1, 0, context);


                                                    }
                                                    catch { }
                                                }
                                                else if (flag6 > 0)
                                                {
                                                    try
                                                    {
                                                        MySqlCommand cmd_update = new MySqlCommand("UPDATE beneficiary_master SET PEPAndSanctions = '6' WHERE Beneficiary_ID = @Bneficiary_ID");
                                                        cmd_update.Parameters.AddWithValue("@Bneficiary_ID", benef_id);
                                                        db_connection.ExecuteNonQueryProcedure(cmd_update);

                                                        //alert for sanction.
                                                        string description1 = " Beneficiery found in SOC (state owned companies)";
                                                        string Record_DateTime = Record_Insert_DateTime;
                                                        string notification_icon = "pep-match-not-found.jpg";
                                                        //notification_message = "<span class='cls-admin'> - <span class='cls-cancel'><strong> International Sanctions Alert  </strong>- </span><span class='cls-addr-confirm'>" + description1 + " </span></span></span><span class='cls-customer'></span>";
                                                        string notification_message = "<span class='cls-admin'> - <span class='cls-cancel'><strong> Beneficiery found in SOC (state owned companies) matches Alert </strong>- </span><span class='cls-addr-confirm'>" + description1 + " </span></span></span><span class='cls-customer'></span>";
                                                        CompanyInfo.save_notification(notification_message, notification_icon, Convert.ToInt32(Customer_ID), Convert.ToDateTime(Record_Insert_DateTime), Convert.ToInt32(Client_ID), 1, User_ID, Convert.ToInt32(Branch_ID), 0, 0, 1, 0, context);


                                                    }
                                                    catch { }
                                                }
                                                else if (flag7 > 0)
                                                {
                                                    try
                                                    {
                                                        MySqlCommand cmd_update = new MySqlCommand("UPDATE beneficiary_master SET PEPAndSanctions = '7' WHERE Beneficiary_ID = @Bneficiary_ID");
                                                        cmd_update.Parameters.AddWithValue("@Bneficiary_ID", benef_id);
                                                        db_connection.ExecuteNonQueryProcedure(cmd_update);

                                                        //alert for sanction.
                                                        string description1 = "Beneficiery found in other types of matches";
                                                        string Record_DateTime = Record_Insert_DateTime;
                                                        string notification_icon = "pep-match-not-found.jpg";
                                                        //notification_message = "<span class='cls-admin'> - <span class='cls-cancel'><strong> International Sanctions Alert  </strong>- </span><span class='cls-addr-confirm'>" + description1 + " </span></span></span><span class='cls-customer'></span>";
                                                        string notification_message = "<span class='cls-admin'> - <span class='cls-cancel'><strong> Beneficiery found in other types of matches Alert </strong>- </span><span class='cls-addr-confirm'>" + description1 + " </span></span></span><span class='cls-customer'></span>";
                                                        CompanyInfo.save_notification(notification_message, notification_icon, Convert.ToInt32(Customer_ID), Convert.ToDateTime(Record_Insert_DateTime), Convert.ToInt32(Client_ID), 1, User_ID, Convert.ToInt32(Branch_ID), 0, 0, 1, 0, context);


                                                    }
                                                    catch { }
                                                }

                                            }

                                            else if (requestStatus == "Complete")
                                            {
                                                //string Customer_ID = "0";
                                                //string Record_Insert_DateTime = "0";
                                                //int Client_ID = 0;
                                                //int User_ID = 0;
                                                //int Branch_ID = 0;
                                                string Record_DateTime = Record_Insert_DateTime;
                                                string notification_icon = "primary-id-upload.jpg";
                                                string notification_message = "<span class='cls-admin'>Complience AML check result is <strong class='cls-priamary'>" + requestStatus + ".</strong></span><span class='cls-customer'></span>";
                                                CompanyInfo.save_notification(notification_message, notification_icon, Convert.ToInt32(Customer_ID), Convert.ToDateTime(Record_DateTime), Client_ID, 1, User_ID, Branch_ID, 0, 1, 1, 0, context);




                                                MySqlCommand cmd_update = new MySqlCommand("UPDATE beneficiary_master SET PEPAndSanctions = '0' WHERE Beneficiary_ID = @Bneficiary_ID");
                                                cmd_update.Parameters.AddWithValue("@Bneficiary_ID", benef_id);
                                                db_connection.ExecuteNonQueryProcedure(cmd_update);

                                                MySqlCommand cmd = new MySqlCommand("update_sanction_flag");
                                                cmd.CommandType = CommandType.StoredProcedure;
                                                cmd.Parameters.AddWithValue("_Customer_ID", Customer_ID);
                                                cmd.Parameters.AddWithValue("_record_date", Record_Insert_DateTime);
                                                cmd.Parameters.AddWithValue("_clientId", Client_ID);
                                                cmd.Parameters.AddWithValue("_branchId", Branch_ID);
                                                cmd.Parameters.AddWithValue("_sanction_flag", 0);
                                                int n = db_connection.ExecuteNonQueryProcedure(cmd);
                                            }


                                            if (flag > 0 || flag1 > 0 || flag2 > 0 || flag4 > 0 || flag5 > 0 || flag6 > 0 || flag7 > 0)
                                            {
                                                flag = 1;
                                                try
                                                {
                                                    MySqlCommand cmd_update = new MySqlCommand("UPDATE beneficiary_master SET PEPAndSanctions = '1' WHERE Beneficiary_ID = @Bneficiary_ID");
                                                    cmd_update.Parameters.AddWithValue("@Bneficiary_ID", beneficiery_id);
                                                    db_connection.ExecuteNonQueryProcedure(cmd_update);
                                                }
                                                catch { }
                                                MySqlCommand _cmd1 = new MySqlCommand("SanctionList_Save_Benf");
                                                _cmd1.CommandType = CommandType.StoredProcedure;
                                                _cmd1.Parameters.AddWithValue("_Exact_Match", 0);
                                                _cmd1.Parameters.AddWithValue("_Beneficiary_ID", beneficiery_id);
                                                _cmd1.Parameters.AddWithValue("_name_string", bfname + " " + blname);
                                                _cmd1.Parameters.AddWithValue("_Customer_ID", Customer_ID);
                                                _cmd1.Parameters.AddWithValue("_recordssearched", 0);
                                                _cmd1.Parameters.AddWithValue("_recordinsertdate", Record_Insert_DateTime);
                                                _cmd1.Parameters.AddWithValue("_legitimate", 0);
                                                _cmd1.Parameters.AddWithValue("_flag", flag);
                                                _cmd1.Parameters.AddWithValue("_User_ID", User_ID);
                                                _cmd1.Parameters.AddWithValue("_Delete_statue", 0);
                                                try
                                                {
                                                    int msg11 = db_connection.ExecuteNonQueryProcedure(_cmd1);
                                                }
                                                catch { }



                                                _cmd1 = new MySqlCommand("Blacklist_Beneficiary");
                                                _cmd1.CommandType = CommandType.StoredProcedure;
                                                _cmd1.Parameters.AddWithValue("_flag", 1);
                                                _cmd1.Parameters.AddWithValue("_Beneficiary_ID", beneficiery_id);
                                                _cmd1.Parameters.AddWithValue("_Client_ID", Client_ID);

                                                try
                                                {
                                                    int msg11 = db_connection.ExecuteNonQueryProcedure(_cmd1);
                                                }
                                                catch { }



                                            }
                                            else
                                            {
                                                flag = 0;
                                                try
                                                {
                                                    MySqlCommand cmd_update = new MySqlCommand("UPDATE beneficiary_master SET PEPAndSanctions = '0' WHERE Beneficiary_ID = @Bneficiary_ID");
                                                    cmd_update.Parameters.AddWithValue("@Bneficiary_ID", beneficiery_id);
                                                    db_connection.ExecuteNonQueryProcedure(cmd_update);
                                                }
                                                catch { }
                                                MySqlCommand _cmd1 = new MySqlCommand("SanctionList_Save_Benf");
                                                _cmd1.CommandType = CommandType.StoredProcedure;
                                                _cmd1.Parameters.AddWithValue("_Exact_Match", 0);
                                                _cmd1.Parameters.AddWithValue("_Beneficiary_ID", beneficiery_id);
                                                _cmd1.Parameters.AddWithValue("_name_string", bfname + " " + blname);
                                                _cmd1.Parameters.AddWithValue("_Customer_ID", Customer_ID);
                                                _cmd1.Parameters.AddWithValue("_recordssearched", 0);
                                                _cmd1.Parameters.AddWithValue("_recordinsertdate", Record_Insert_DateTime);
                                                _cmd1.Parameters.AddWithValue("_legitimate", 0);
                                                _cmd1.Parameters.AddWithValue("_flag", flag);
                                                _cmd1.Parameters.AddWithValue("_User_ID", User_ID);
                                                _cmd1.Parameters.AddWithValue("_Delete_statue", 0);
                                                try
                                                {
                                                    int msg11 = db_connection.ExecuteNonQueryProcedure(_cmd1);
                                                }
                                                catch { }



                                                _cmd1 = new MySqlCommand("Blacklist_Beneficiary");
                                                _cmd1.CommandType = CommandType.StoredProcedure;
                                                _cmd1.Parameters.AddWithValue("_flag", 0);
                                                _cmd1.Parameters.AddWithValue("_Beneficiary_ID", beneficiery_id);
                                                _cmd1.Parameters.AddWithValue("_Client_ID", Client_ID);

                                                try
                                                {
                                                    int msg11 = db_connection.ExecuteNonQueryProcedure(_cmd1);
                                                }
                                                catch { }
                                            }

                                        }
                                        catch (Exception ex)
                                        {
                                            //string stattus = (string)mtsmethods.InsertErrorLogDetails(ex.Message.Replace("\'", "\\'"), user_id, "Complience Assist AMl Check - 1st api call ", i.Branch_ID, i.Client_ID);

                                        }
                                    }
                                    catch (Exception ex)
                                    {

                                    }
                                }
                                catch { }


                            }

                        }


                    }
                    catch { }
                }
            }

            aml_result = flag.ToString();

            return aml_result;
        }
        private string RandomString(int length, Random random)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        // Helper method to generate random number of specified length
        private string RandomNumber(int length, Random random)
        {
            const string digits = "0123456789";
            return new string(Enumerable.Repeat(digits, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public string GetPayoutCountries(string customerID)
        {
            string payoutCountries = "";
            string query = "SELECT payout_countries FROM customer_registration WHERE Customer_ID = @Customer_ID";

            MySqlConnector.MySqlCommand cmd = new MySqlConnector.MySqlCommand(query);
            cmd.Parameters.AddWithValue("@Customer_ID", customerID);

            DataTable dt = db_connection.ExecuteQueryDataTableProcedure(cmd);

            if (dt != null && dt.Rows.Count > 0)
            {
                payoutCountries = Convert.ToString(dt.Rows[0]["payout_countries"]);
            }

            return payoutCountries;
        }

        public DataTable get_benf_mobile_count(Model.Beneficiary obj)
        {
            List<Model.Beneficiary> _lst = new List<Model.Beneficiary>();

            string Whereclause = string.Empty;
            if (obj.Beneficiary_ID != 0)
            {
                Whereclause = " and Beneficiary_ID !=" + obj.Beneficiary_ID;
            }
            MySqlCommand _cmd = new MySqlCommand("Beneficiary_chk_duplicatemobile_cnt");
            _cmd.CommandType = CommandType.StoredProcedure;
            _cmd.Parameters.AddWithValue("_Whereclause", Whereclause);
            _cmd.Parameters.AddWithValue("_Beneficiary_Mobile", obj.Beneficiary_Mobile);
            _cmd.Parameters.AddWithValue("_Country_code", obj.Country_Code);
            DataTable dt = db_connection.ExecuteQueryDataTableProcedure(_cmd);
            return dt;
        }

        public DataTable GetInfo(Model.Beneficiary obj , HttpContext context) //vyankatesh 11-12-24
        {
            int Customer_ID = Convert.ToInt32(CompanyInfo.Decrypt(obj.Customer_ID, true));
            string Activity = string.Empty; string[] Alert_Msg = new string[7]; string sendmsg = string.Empty; string notification_icon = ""; string notification_message = "";
           // var context = System.Web.HttpContext.Current;
            string Username = "Calyx-api";
            string error_invalid_data = "";
            string error_msg = "";
            string Client_ID_regex = validation.validate(Convert.ToString(obj.Client_ID), 0, 1, 1, 1, 1, 1, 1, 1, 1);
            string Beneficiary_ID_regex = validation.validate(Convert.ToString(obj.Beneficiary_ID), 0, 1, 1, 1, 1, 1, 1, 1, 1);
            string Delete_Status_regex = validation.validate(Convert.ToString(obj.Delete_Status), 0, 1, 1, 1, 1, 1, 1, 1, 1);
            string Customer_ID_regex = validation.validate(Convert.ToString(Customer_ID), 0, 1, 1, 1, 1, 1, 0, 1, 1);
            string Beneficiary_Country_ID_regex = validation.validate(Convert.ToString(obj.Beneficiary_Country_ID), 0, 1, 1, 1, 1, 1, 1, 1, 1);
            string Sending_Flag_regex = validation.validate(Convert.ToString(obj.Sending_Flag), 0, 1, 1, 1, 1, 1, 1, 1, 1);
            if (Client_ID_regex == "" && Beneficiary_ID_regex == "" && Delete_Status_regex == "" && Customer_ID_regex == "" && Beneficiary_Country_ID_regex == "" && Sending_Flag_regex == "")
            {
                Client_ID_regex = "true";
                Beneficiary_ID_regex = "true";
                Delete_Status_regex = "true";
                Customer_ID_regex = "true";
                Beneficiary_Country_ID_regex = "true";
                Sending_Flag_regex = "true";
            }
            DataTable dt = new DataTable();
            if (Client_ID_regex != "false" && Beneficiary_ID_regex != "false" && Delete_Status_regex != "false" && Customer_ID_regex != "false" && Beneficiary_Country_ID_regex != "false" && Sending_Flag_regex != "false")
            {
                List<Model.Beneficiary> _lst = new List<Model.Beneficiary>();
                //DataTable dtperm = (DataTable)CompanyInfo.getEmailPermission(obj.Client_ID, 47);
                MySqlCommand _cmdperm = new MySqlCommand("GetPermissions");
                _cmdperm.CommandType = CommandType.StoredProcedure;

                _cmdperm.Parameters.AddWithValue("_whereclause", " and PID in (47,151)");
                _cmdperm.Parameters.AddWithValue("_Client_ID", obj.Client_ID);


                DataTable dtperm = db_connection.ExecuteQueryDataTableProcedure(_cmdperm);
                MySqlCommand _cmd = new MySqlCommand("Beneficiary_Search_Details"); //change for multiple bank account 
                _cmd.CommandType = CommandType.StoredProcedure;
                string whereclause = " ";
                if (obj.Beneficiary_ID > 0)
                {
                    whereclause = whereclause + " and bb.Beneficiary_ID=" + obj.Beneficiary_ID + "";
                }
                if (obj.Customer != null)
                {
                    if (obj.Customer.WireTransfer_ReferanceNo != "" && obj.Customer.WireTransfer_ReferanceNo != null)
                    {
                        whereclause = whereclause + " and WireTransfer_ReferanceNo  LIKE '%" + obj.Customer.WireTransfer_ReferanceNo + "%'";
                    }
                    if (obj.Customer.Full_Name != "" && obj.Customer.Full_Name != null)
                    {
                        whereclause = whereclause + " and concat(first_Name,' ',Last_Name) LIKE '%" + obj.Customer.Full_Name + "%'";
                    }
                }
                if (obj.Beneficiary_Name != null && obj.Beneficiary_Name != "")
                {
                    whereclause = whereclause + " and bb.Beneficiary_Name LIKE '%" + obj.Beneficiary_Name + "%'";
                }
                if (dtperm.Rows.Count > 0)
                {
                    if (Convert.ToInt32(dtperm.Rows[1]["Status_ForCustomer"]) == 1)
                    {
                        if (obj.Delete_Status != null && obj.Delete_Status != -1 && obj.whereclause != "View_benf")
                        {
                            whereclause = whereclause + " and bb.delete_status=" + obj.Delete_Status + "";
                        }
                    }
                    else
                    {
                        whereclause = whereclause + " and bb.delete_status=0";
                    }
                }
                else if (obj.Delete_Status != null && obj.Delete_Status != -1)
                {
                    whereclause = whereclause + " and bb.delete_status=" + obj.Delete_Status + "";
                }
                if (Customer_ID != null && Customer_ID > 0)
                {
                    whereclause = whereclause + " and bb.Customer_ID=" + Customer_ID + "";
                }
                if (obj.Beneficiary_Country_ID != null && obj.Beneficiary_Country_ID > 0)
                {
                    whereclause = whereclause + " and bb.beneficiary_country_id=" + obj.Beneficiary_Country_ID + "";
                }
                if (obj.Sending_Flag == 1 || obj.Sending_Flag == 0)
                {
                    whereclause = whereclause + " and dd.Sending_Flag=" + obj.Sending_Flag + "";
                }
                if (obj.status == 2)
                {
                    whereclause = whereclause + " and paymentdeposittype_master.ShowOnCustSide = 0 ";
                }


                if (obj.Benf_BankDetails_ID != 0) //vyankatesh 25-11-24
                {
                    whereclause = whereclause + " and ee.BBDetails_ID='" + obj.Benf_BankDetails_ID + "'";
                    _cmd.Parameters.AddWithValue("_conditionclause", "");
                }
                else
                {
                    _cmd.Parameters.AddWithValue("_conditionclause", " and ee.BBDetails_ID=tmm.Benf_BankDetails_ID");
                }
                whereclause = whereclause + " and bb.Client_ID=" + obj.Client_ID + "";


                _cmd.Parameters.AddWithValue("_whereclause", whereclause);

                dt = db_connection.ExecuteQueryDataTableProcedure(_cmd);
                dt.Columns.Add("EditPerm", typeof(int));
                if (dtperm.Rows.Count > 0 && dt.Rows.Count > 0)
                {
                    dt.Rows[0]["EditPerm"] = dtperm.Rows[0]["Status_ForCustomer"];
                }
            }
            else
            {
                string msg = "Validation Error Client_ID_regex- +" + Client_ID_regex + "Beneficiary_ID_regex- " + Beneficiary_ID_regex + " Delete_Status_regex- +" + Delete_Status_regex + " +Customer_ID_regex- " + Customer_ID_regex + "Beneficiary_Country_ID_regex- " + Beneficiary_Country_ID_regex + "Sending_Flag_regex-" + Sending_Flag_regex;
                int stattus = (int)CompanyInfo.InsertActivityLogDetailsSecurity(msg, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), 1, "srvDiscount", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "create", 0,context);
            }
            return dt;
        }

        #region Amal
        public class amal
        {
            public string code { get; set; }
            public string zeepay_id { get; set; }
            public string amount { get; set; }
            public string message { get; set; }
            public string token_type { get; set; }
            public string expires_in { get; set; }
            public string access_token { get; set; }
            public data1 response { get; set; }
        }
        public class data1
        {
            public data2 result { get; set; }

        }
        public class data2
        {
            public string token { get; set; }
        }

        #endregion Amal

    }
}
