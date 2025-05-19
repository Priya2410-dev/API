using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Net;
using System.Text;
using System.Xml;
using MySqlConnector;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace Calyx_Solutions.Service
{
    public class srvCustomer
    {
        mts_connection _MTS = new mts_connection();

        public DataTable RegisterValidations(Model.Customer obj)
        {
            MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("Registration_Configuration");
            _cmd.CommandType = CommandType.StoredProcedure;
            _cmd.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
            DataTable dt = db_connection.ExecuteQueryDataTableProcedure(_cmd);
            return dt;
        }

        public List<Model.Customer> validate_password(Model.Customer obj)
        {

            List<Model.Customer> _lst = new List<Model.Customer>();
            MySqlCommand _cmd = new MySqlCommand("get_Password_Permission");
            _cmd.CommandType = CommandType.StoredProcedure;
            _cmd.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
            DataTable dt = db_connection.ExecuteQueryDataTableProcedure(_cmd);

            Model.Customer _obj = new Model.Customer();

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    _obj = new Model.Customer();

                    if (row["Uppercase"] != DBNull.Value)
                    {
                        _obj.Uppercase = Convert.ToInt32(row["Uppercase"]);
                    }
                    if (row["Lowercase"] != DBNull.Value)
                    {
                        _obj.Lowercase = Convert.ToDecimal(row["Lowercase"]);
                    }
                    if (row["Digit"] != DBNull.Value)
                    {
                        _obj.Digit = Convert.ToDecimal(row["Digit"]);
                    }

                    if (row["Isspecial_Char"] != DBNull.Value)
                    {
                        _obj.Isspecial_Char = Convert.ToInt32(row["Isspecial_Char"]);
                    }
                    if (row["Special_char"] != DBNull.Value)
                    {
                        _obj.Special_char = Convert.ToString(row["Special_char"]);
                    }
                    if (row["Minpass_length"] != DBNull.Value)
                    {
                        _obj.Minpass_length = Convert.ToInt32(row["Minpass_length"]);
                    }
                    if (row["Maxpass_Length"] != DBNull.Value)
                    {
                        _obj.Maxpass_Length = Convert.ToInt32(row["Maxpass_Length"]);
                    }
                    _lst.Add(_obj);
                }
            }

            return _lst;
        }

        public string check_MobileNo_Is_Exist(Model.Customer obj)
        {
            string MobileNumber_regex = "true";
            MobileNumber_regex = validation.validate(obj.MobileNumber, 1, 1, 1, 0, 1, 1, 1, 1, 1);
            if (MobileNumber_regex != "false")
            {
                using (MySqlConnection con = new MySqlConnection(_MTS.WebConnSetting()))
                {
                    con.Open();
                    try
                    {

                        using (MySqlCommand cmd = new MySqlCommand("sp_check_MobileNo_Is_Exist", con))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;

                            cmd.Parameters.AddWithValue("_BranchId", obj.Branch_ID);
                            cmd.Parameters.AddWithValue("_ClientId", obj.Client_ID);
                            cmd.Parameters.AddWithValue("_MobileNo", obj.MobileNumber);

                            cmd.Parameters.Add(new MySqlParameter("_existsMobile", MySqlDbType.Int32));
                            cmd.Parameters["_existsMobile"].Direction = ParameterDirection.Output;

                            cmd.ExecuteScalar();

                            obj.ExistMobile = Convert.ToInt32(cmd.Parameters["_existsMobile"].Value);
                            if (obj.ExistMobile == 1)
                            {
                                return "exist_mobile";
                            }
                            else
                            {
                                return "not_exist_mobile";
                            }
                        }
                        // transaction.Commit();
                    }
                    catch (Exception _x)
                    {
                        //transaction.Rollback();
                        throw _x;
                    }
                    finally
                    {
                        con.Close();
                    }
                }
            }
            else
            {
                return "validation failed";
            }
        }

        public int saveToken(Model.Customer obj)
        {
            int Customer_ID = Convert.ToInt32(CompanyInfo.Decrypt(obj.Id, true));
            string Activity = string.Empty;
            MySqlConnector.MySqlCommand cmd = new MySqlConnector.MySqlCommand("saveToken");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("_Customer_ID", Customer_ID);
            cmd.Parameters.AddWithValue("_token", obj.Name);
            cmd.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
            int n = db_connection.ExecuteNonQueryProcedure(cmd);
            cmd.Dispose();
            return n;
        }

        public List<Model.Customer> Getdeleteacc(Model.Customer obj)
        {
            HttpContext context = null;
            int Customer_ID = Convert.ToInt32(CompanyInfo.Decrypt(Convert.ToString(obj.Id), true));
            List<Model.Customer> _lst = new List<Model.Customer>();
            string Activity = string.Empty;
            string notification_icon = "";
            string notification_message = "";
            MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("Edit_Request");
            _cmd.CommandType = CommandType.StoredProcedure;
            //_cmd.Parameters.AddWithValue("_ClientId", obj.Client_ID);
            //_cmd.Parameters.AddWithValue("_BranchId", obj.Branch_ID);
            //_cmd.Parameters.AddWithValue("_Customer_ID", obj.Id);
            //_cmd.Parameters.AddWithValue("_UpdateFlag", obj.Flag);
            string splitter = "<br/>"; var request = "";
            string subject_line = "Delete Account";
            //string final_subject_line = "Edit " + (obj.chk_changed).ToLower() + " number request sent.";
            //obj.whereclause = "Request From Customer App : Old Phone No: <b>" +  +
            //    "</b> Old Mobile No: <b>" + + "</b>" +
            //    "<br/>Request Details :" + request.TrimEnd();



            string final_subject_line = "edit request";
            //obj.whereclause = "Request From Customer App";
            obj.Comment = final_subject_line;
            obj.AllCustomer_Flag = "1";//response flag
            obj.Delete_Status = 0;
            obj.cashcollection_flag = "1";//active status----default status is pending
            obj.Record_Insert_DateTime = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
            obj.RecordDate = Convert.ToDateTime(CompanyInfo.gettime(obj.Client_ID, "0", 0, context));
            obj.Flag = 2;//editrequest flag
            using (MySqlConnector.MySqlCommand cmd1 = new MySqlConnector.MySqlCommand("Edit_Request"))
            {
                cmd1.CommandType = CommandType.StoredProcedure;
                //obj.HouseNumber = "cc";
                //obj.Street = "23";

                cmd1.Parameters.AddWithValue("_Id", Customer_ID);

                cmd1.Parameters.AddWithValue("_AllCustomer_Flag", obj.AllCustomer_Flag);
                cmd1.Parameters.AddWithValue("_Reason", obj.whereclause);
                cmd1.Parameters.AddWithValue("_Delete_Status", obj.Delete_Status);
                cmd1.Parameters.AddWithValue("_cashcollection_flag", obj.cashcollection_flag);
                cmd1.Parameters.AddWithValue("_Record_Insert_DateTime", obj.RecordDate);
                cmd1.Parameters.AddWithValue("_Branch_ID", obj.Branch_ID);
                cmd1.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                cmd1.Parameters.AddWithValue("_comments", obj.Comment);
                cmd1.Parameters.AddWithValue("_flag", obj.Flag);
                //int n = cmd1.ExecuteNonQuery();
                //int n = db_connection.ExecuteNonQueryProcedure(cmd1);
                obj.Message = "Request";

                DataTable dt = db_connection.ExecuteQueryDataTableProcedure(cmd1);
                DateTime today = Convert.ToDateTime(obj.RecordDate.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture), CultureInfo.InvariantCulture);
                notification_icon = "cancel.jpg";
                notification_message = "<span class='cls-admin'>successfully sent <strong class='cls-change-pass'>delete</strong>.</span><span class='cls-customer'><strong>Delete</strong><span>You have sent request.</span></span>";
                CompanyInfo.save_notification(notification_message, notification_icon, Convert.ToInt32(Customer_ID), Convert.ToDateTime(obj.RecordDate), Convert.ToInt32(obj.Client_ID), 1, 0, Convert.ToInt32(obj.Branch_ID), 0, 0, 1, 0, context);
                Model.Customer _obj = new Model.Customer();
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        _obj = new Model.Customer();

                        if (row["flag"] != DBNull.Value)
                        {
                            _obj.Name = Convert.ToString(row["Active_Status"]);
                        }
                        _lst.Add(_obj);
                    }
                }
                return _lst;
            }



        }

        public DataTable GeteditrequestALL(Model.Customer obj)
        {
            int Customer_ID = Convert.ToInt32(CompanyInfo.Decrypt(Convert.ToString(obj.Id), true));
            List<Model.Customer> _lst = new List<Model.Customer>();
            MySqlCommand _cmd = new MySqlCommand("sp_GetEditrequestAll");
            _cmd.CommandType = CommandType.StoredProcedure;
            _cmd.Parameters.AddWithValue("_ClientId", obj.Client_ID);
            _cmd.Parameters.AddWithValue("_BranchId", obj.Branch_ID);
            _cmd.Parameters.AddWithValue("_Customer_ID", Customer_ID);
            //string whereclause = " and wtt.Client_ID=" + obj.Client_ID + " and Customer_ID= " + obj.Customer_ID;
            //  string whereclause = " and c.Client_ID=" + obj.Client_ID + " and w.Client_ID=" + obj.Client_ID + " and w.Customer_ID=" + obj.Customer_ID + " and Currency_Code like '%" + obj.Currency_Code + "%'";
            // _cmd.Parameters.AddWithValue("_whereclause", whereclause);
            DataTable dt = db_connection.ExecuteQueryDataTableProcedure(_cmd);
            return dt;
        }

        public List<Model.Customer> Geteditrequest(Model.Customer obj)
        {
            int Customer_ID = Convert.ToInt32(CompanyInfo.Decrypt(Convert.ToString(obj.Id), true));
            List<Model.Customer> _lst = new List<Model.Customer>();
            MySqlCommand _cmd = new MySqlCommand("sp_GetEditrequest");
            _cmd.CommandType = CommandType.StoredProcedure;
            _cmd.Parameters.AddWithValue("_ClientId", obj.Client_ID);
            _cmd.Parameters.AddWithValue("_BranchId", obj.Branch_ID);
            _cmd.Parameters.AddWithValue("_Customer_ID", Customer_ID);
            _cmd.Parameters.AddWithValue("_UpdateFlag", obj.Flag);

            DataTable dt = db_connection.ExecuteQueryDataTableProcedure(_cmd);
            Model.Customer _obj = new Model.Customer();
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    _obj = new Model.Customer();

                    if (row["flag"] != DBNull.Value)
                    {
                        _obj.Name = Convert.ToString(row["Active_Status"]);
                    }
                    _lst.Add(_obj);
                }
            }
            return _lst;
        }
        public DataTable dataValidations(int clientID, int branchID)
        {
            MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("Registration_Config");
            _cmd.CommandType = CommandType.StoredProcedure;
            _cmd.Parameters.AddWithValue("_Client_ID", clientID);
            _cmd.Parameters.AddWithValue("_CB_ID", branchID);

            DataTable dt = db_connection.ExecuteQueryDataTableProcedure(_cmd);
            return dt;
        }

        public DataTable customer_transferamount_minmax(Model.Customer obj)
        {
            int Customer_ID = Convert.ToInt32(CompanyInfo.Decrypt(Convert.ToString(obj.Customer_ID), true));
            MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("customer_transferamount_minmax");
            _cmd.CommandType = CommandType.StoredProcedure;
            _cmd.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
            _cmd.Parameters.AddWithValue("_Customer_ID", Customer_ID);
            DataTable dt = db_connection.ExecuteQueryDataTableProcedure(_cmd);
            Model.CustomerLimit _obj = new Model.CustomerLimit();
            return dt;
        }

        public DataTable Customer_Details(Model.Customer obj)
        {
            HttpContext context = null;
            int Customer_ID = Convert.ToInt32(CompanyInfo.Decrypt(Convert.ToString(obj.Id), true));
            List<Model.Customer> _lst = new List<Model.Customer>();
            string Client_ID_regex = validation.validate(Convert.ToString(obj.Client_ID), 0, 1, 1, 1, 1, 1, 1, 1, 1);
            string Id_regex = validation.validate(Convert.ToString(Customer_ID), 0, 1, 1, 1, 1, 1, 1, 1, 1);
            DataTable dt = new DataTable();
            if (Client_ID_regex != "false" && Id_regex != "false")
            {
                MySqlCommand _cmd = new MySqlCommand("customer_details_by_param");
                _cmd.CommandType = CommandType.StoredProcedure;
                string _whereclause = " and cr.Client_ID=" + obj.Client_ID;
                //if (obj.Id > 0)
                //{
                _whereclause = " and cr.Customer_ID=" + Customer_ID;
                //}

                _cmd.Parameters.AddWithValue("_whereclause", _whereclause);
                _cmd.Parameters.AddWithValue("_SecurityKey", CompanyInfo.SecurityKey());
                dt = db_connection.ExecuteQueryDataTableProcedure(_cmd);
                if (dt.Rows.Count > 0)
                {

                    //var context = System.Web.HttpContext.Current;
                    foreach (DataRow row in dt.Rows)
                    {
                        if (row["Profile_Image"] != DBNull.Value && row["Profile_Image"].ToString() != "")
                        {
                            string URL = "";
                            DataTable dtc = CompanyInfo.get(obj.Client_ID, context);
                            if (dtc.Rows.Count > 0)
                            {
                                URL = Convert.ToString(dtc.Rows[0]["Company_URL_Admin"]);
                            }

                            // string image_link = (context.Session["Company_URL_Admin"]).ToString() + row["Profile_Image"].ToString();
                            string image_link = URL + row["Profile_Image"].ToString();

                            string base64str = CompanyInfo.ConvertImageLinkToBase64(image_link);
                            row["Profile_Image"] = base64str;
                        }
                    }
                }
            }
            else
            {
                string msg = "Validation Error Client_ID_regex-" + Client_ID_regex + "Id_regex-" + Id_regex;
                int stattus = (int)CompanyInfo.InsertActivityLogDetailsSecurity(msg, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), 1, "srvcustomer", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "create", 0, context);

            }
            return dt;
        }

        public string[] AutoCheckSanctionList(Model.Customer obj, HttpContext context)
        {
            string[] Alert_Msg = new string[7];
            try
            {

                int uk_cnt = 0; int usa_cnt = 0; int eu_cnt = 0; int aud_cnt = 0; int pep_cnt = 0; int check_records_found = 0; int un_cnt = 0;
                string abc = ""; int Alert_count = 0; string Alert = string.Empty;
                using (MySqlConnection con = new MySqlConnection(_MTS.WebConnSetting()))
                {
                    con.Open();
                    // MySqlTransaction transaction;
                    DataTable dt_cust = new DataTable();

                    MySqlCommand cmdupdate1 = new MySqlCommand("Get_Permissions", con);
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
                        obj.Name = obj.FirstName + ' ' + obj.Middle_Name + ' ' + obj.LastName;
                        MySqlCommand cmd1 = new MySqlCommand("GetCustDetailsByID", con);
                        cmd1.CommandType = CommandType.StoredProcedure;
                        cmd1.Parameters.AddWithValue("cust_ID", obj.Id);

                        dt_cust = db_connection.ExecuteQueryDataTableProcedure(cmd1);//(DataTable)cmd1.ExecuteScalar();
                        string full_name = string.Empty; string last_name = string.Empty; string middle_name = string.Empty;
                        if (Convert.ToString(dt_cust.Rows[0]["First_Name"]) != "" && Convert.ToString(dt_cust.Rows[0]["First_Name"]) != null)
                        {
                            full_name = Convert.ToString(dt_cust.Rows[0]["First_Name"]);
                        }
                        if (Convert.ToString(dt_cust.Rows[0]["Middle_Name"]) != "" && Convert.ToString(dt_cust.Rows[0]["Middle_Name"]) != null)
                        {
                            full_name = full_name + " " + Convert.ToString(dt_cust.Rows[0]["Middle_Name"]);
                        }
                        if (Convert.ToString(dt_cust.Rows[0]["Last_Name"]) != "" && Convert.ToString(dt_cust.Rows[0]["Last_Name"]) != null)
                        {
                            full_name = full_name + " " + Convert.ToString(dt_cust.Rows[0]["Last_Name"]);
                        }
                        if (dt_cust.Rows.Count > 0)
                        {
                            dt = (DataTable)DBHelper.GetSanctionList(full_name);
                            DataRow[] dr = dt.Select("name='" + obj.Name + "' OR   Passport_Details='" + obj.Name + "'");

                            for (int i = 0; i < dr.Length; i++)
                            {
                                if (i == 0)
                                {
                                    obj.Record_Insert_DateTime = DateTime.Now.ToString();
                                    string notification_icon = "sanction-list-match-found.jpg";
                                    string notification_message = "<span class='cls-admin'>Match found in <strong>UK</strong> sanction list.</span><span class='cls-customer'></span>";
                                    CompanyInfo.save_notification_compliance(notification_message, notification_icon, Convert.ToString(obj.Customer_ID), Convert.ToDateTime(obj.Record_Insert_DateTime), Convert.ToInt32(obj.Client_ID), 1, Convert.ToInt32(obj.User_ID), Convert.ToInt32(obj.Branch_ID), 0, 1, 1, 0, context);
                                }

                                uk_cnt++;
                            }
                            dt11 = (DataTable)DBHelper.GetUNSanctionList(full_name);
                            DataRow[] dr1 = dt11.Select("Name='" + obj.Name + "' OR   INDIVIDUAL_DOCUMENT='" + obj.Name + "'");
                            //int un_cnt = 0;
                            for (int i = 0; i < dr1.Length; i++)
                            {
                                if (i == 0)
                                {
                                    obj.Record_Insert_DateTime = DateTime.Now.ToString();
                                    string notification_icon = "sanction-list-match-found.jpg";
                                    string notification_message = "<span class='cls-admin'>Match found in <strong>UN</strong> sanction list.</span><span class='cls-customer'></span>";
                                    CompanyInfo.save_notification_compliance(notification_message, notification_icon, Convert.ToString(obj.Customer_ID), Convert.ToDateTime(obj.Record_Insert_DateTime), Convert.ToInt32(obj.Client_ID), 1, Convert.ToInt32(obj.User_ID), Convert.ToInt32(obj.Branch_ID), 0, 1, 1, 0, context);
                                }
                                un_cnt++;
                            }
                            dt12 = (DataTable)DBHelper.GetUSASanctionList(full_name);
                            DataRow[] dr2 = dt12.Select("Name='" + obj.Name + "' ");

                            for (int i = 0; i < dr2.Length; i++)
                            {
                                if (i == 0)
                                {
                                    obj.Record_Insert_DateTime = DateTime.Now.ToString();
                                    string notification_icon = "sanction-list-match-found.jpg";
                                    string notification_message = "<span class='cls-admin'>Match found in <strong>USA</strong> sanction list.</span><span class='cls-customer'></span>";
                                    CompanyInfo.save_notification_compliance(notification_message, notification_icon, Convert.ToString(obj.Customer_ID), Convert.ToDateTime(obj.Record_Insert_DateTime), Convert.ToInt32(obj.Client_ID), 1, Convert.ToInt32(obj.User_ID), Convert.ToInt32(obj.Branch_ID), 0, 1, 1, 0, context);
                                }
                                usa_cnt++;
                            }
                            dt13 = (DataTable)DBHelper.GetEUSanctionList(full_name);
                            DataRow[] dr3 = dt13.Select("Name='" + obj.Name + "'  OR  citizenship_countryDescription='" + obj.Name + "' ");

                            for (int i = 0; i < dr3.Length; i++)
                            {
                                if (i == 0)
                                {
                                    obj.Record_Insert_DateTime = DateTime.Now.ToString();
                                    string notification_icon = "sanction-list-match-found.jpg";
                                    string notification_message = "<span class='cls-admin'>Match found in <strong>EU</strong> sanction list.</span><span class='cls-customer'></span>";
                                    CompanyInfo.save_notification_compliance(notification_message, notification_icon, Convert.ToString(obj.Customer_ID), Convert.ToDateTime(obj.Record_Insert_DateTime), Convert.ToInt32(obj.Client_ID), 1, Convert.ToInt32(obj.User_ID), Convert.ToInt32(obj.Branch_ID), 0, 1, 1, 0, context);
                                }
                                eu_cnt++;
                            }
                            dt14 = (DataTable)DBHelper.GetAUDSanctionList(full_name);
                            DataRow[] dr4 = dt14.Select("name='" + obj.Name + "'  OR  Passport_Details='" + obj.Name + "' ");

                            for (int i = 0; i < dr4.Length; i++)
                            {
                                if (i == 0)
                                {
                                    obj.Record_Insert_DateTime = DateTime.Now.ToString();
                                    string notification_icon = "sanction-list-match-found.jpg";
                                    string notification_message = "<span class='cls-admin'>Match found in <strong>AUD</strong> sanction list.</span><span class='cls-customer'></span>";
                                    CompanyInfo.save_notification_compliance(notification_message, notification_icon, Convert.ToString(obj.Customer_ID), Convert.ToDateTime(obj.Record_Insert_DateTime), Convert.ToInt32(obj.Client_ID), 1, Convert.ToInt32(obj.User_ID), Convert.ToInt32(obj.Branch_ID), 0, 1, 1, 0, context);
                                }
                                aud_cnt++;
                            }
                            dt15 = (DataTable)DBHelper.GetPEPSanctionList(full_name);
                            DataRow[] dr5 = dt15.Select("Name='" + obj.Name + "'  ");

                            for (int i = 0; i < dr5.Length; i++)
                            {
                                if (i == 0)
                                {
                                    obj.Record_Insert_DateTime = DateTime.Now.ToString();
                                    string notification_icon = "sanction-list-match-found.jpg";
                                    string notification_message = "<span class='cls-admin'>Match found in <strong>PEP</strong> sanction list.</span><span class='cls-customer'></span>";
                                    CompanyInfo.save_notification_compliance(notification_message, notification_icon, Convert.ToString(obj.Customer_ID), Convert.ToDateTime(obj.Record_Insert_DateTime), Convert.ToInt32(obj.Client_ID), 1, Convert.ToInt32(obj.User_ID), Convert.ToInt32(obj.Branch_ID), 0, 1, 1, 0, context);
                                }
                                pep_cnt++;
                            }
                        }
                        var flag = "";

                        // obj.CustomerId = obj.Id;
                        obj.Id1 = 0;
                        obj.status = 0;
                        // obj.RecordDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));// (string)mtsmethods.gettodate(c);
                        obj.DeleteStatus = 0;
                        obj.whereclause = "";
                        // obj.Client_ID = obj.Client_ID;
                        //if (dt.Rows.Count > 0)
                        //{
                        obj.Id1 = dt.Rows.Count;
                        if (dt.Rows.Count > 0) { obj.status = 2; check_records_found++; }
                        flag = "0";
                        obj.whereclause = flag;

                        MySqlCommand cmd4 = new MySqlCommand("SanctionList_Save", con);
                        cmd4.CommandType = CommandType.StoredProcedure;

                        cmd4.Parameters.AddWithValue("customername", obj.Name);
                        cmd4.Parameters.AddWithValue("_Exact_Match", Convert.ToString(uk_cnt));
                        cmd4.Parameters.AddWithValue("CustomerId", obj.Id);
                        cmd4.Parameters.AddWithValue("Id", obj.Id1);
                        cmd4.Parameters.AddWithValue("_date", obj.RecordDate);
                        cmd4.Parameters.AddWithValue("_status", obj.status);
                        cmd4.Parameters.AddWithValue("wherclause", obj.whereclause);
                        cmd4.Parameters.AddWithValue("userId", obj.CommentUserId);
                        cmd4.Parameters.AddWithValue("deletestatus", obj.DeleteStatus);

                        string msg = Convert.ToString(cmd4.ExecuteNonQuery());
                        if (msg == "0")
                        {
                            Alert_Msg[Alert_count] = Alert + " UK Sanction List";
                            Alert_count++;
                        }



                        obj.Id1 = dt11.Rows.Count; obj.status = 0;
                        if (dt11.Rows.Count > 0) { obj.status = 2; check_records_found++; }
                        flag = "3";
                        obj.whereclause = flag;

                        MySqlCommand cmd55 = new MySqlCommand("SanctionList_Save", con);
                        cmd55.CommandType = CommandType.StoredProcedure;

                        cmd55.Parameters.AddWithValue("customername", obj.Name);
                        cmd55.Parameters.AddWithValue("CustomerId", obj.Id);
                        cmd55.Parameters.AddWithValue("_Exact_Match", Convert.ToInt32(un_cnt));
                        cmd55.Parameters.AddWithValue("Id", obj.Id1);
                        cmd55.Parameters.AddWithValue("_date", obj.RecordDate);
                        cmd55.Parameters.AddWithValue("_status", obj.status);
                        cmd55.Parameters.AddWithValue("wherclause", obj.whereclause);
                        cmd55.Parameters.AddWithValue("userId", obj.CommentUserId);
                        cmd55.Parameters.AddWithValue("deletestatus", obj.DeleteStatus);

                        string msg1 = Convert.ToString(cmd55.ExecuteNonQuery());
                        if (msg1 == "0")
                        {
                            Alert_Msg[Alert_count] = Alert + " UN Sanction List";
                            Alert_count++;
                        }




                        obj.Id1 = dt12.Rows.Count; obj.status = 0;
                        if (dt12.Rows.Count > 0) { obj.status = 2; check_records_found++; }
                        flag = "1";
                        obj.whereclause = flag;

                        MySqlCommand cmd6 = new MySqlCommand("SanctionList_Save", con);
                        cmd6.CommandType = CommandType.StoredProcedure;

                        cmd6.Parameters.AddWithValue("customername", obj.Name);
                        cmd6.Parameters.AddWithValue("CustomerId", obj.Id);
                        cmd6.Parameters.AddWithValue("_Exact_Match", Convert.ToInt32(usa_cnt));
                        cmd6.Parameters.AddWithValue("Id", obj.Id1);
                        cmd6.Parameters.AddWithValue("_date", obj.RecordDate);
                        cmd6.Parameters.AddWithValue("_status", obj.status);
                        cmd6.Parameters.AddWithValue("wherclause", obj.whereclause);
                        cmd6.Parameters.AddWithValue("userId", obj.CommentUserId);
                        cmd6.Parameters.AddWithValue("deletestatus", obj.DeleteStatus);

                        string msg2 = Convert.ToString(cmd6.ExecuteNonQuery());
                        if (msg2 == "0")
                        {
                            Alert_Msg[Alert_count] = Alert + " USA Sanction List";
                            Alert_count++;
                        }




                        obj.Id1 = dt13.Rows.Count; obj.status = 0;
                        if (dt13.Rows.Count > 0) { obj.status = 2; check_records_found++; }
                        flag = "2";
                        obj.whereclause = flag;

                        MySqlCommand cmd7 = new MySqlCommand("SanctionList_Save", con);
                        cmd7.CommandType = CommandType.StoredProcedure;
                        cmd7.Parameters.AddWithValue("_Exact_Match", Convert.ToInt32(eu_cnt));
                        cmd7.Parameters.AddWithValue("customername", obj.Name);
                        cmd7.Parameters.AddWithValue("CustomerId", obj.Id);
                        cmd7.Parameters.AddWithValue("Id", obj.Id1);
                        cmd7.Parameters.AddWithValue("_date", obj.RecordDate);
                        cmd7.Parameters.AddWithValue("_status", obj.status);
                        cmd7.Parameters.AddWithValue("wherclause", obj.whereclause);
                        cmd7.Parameters.AddWithValue("userId", obj.CommentUserId);
                        cmd7.Parameters.AddWithValue("deletestatus", obj.DeleteStatus);

                        string msg7 = Convert.ToString(cmd7.ExecuteNonQuery());
                        if (msg7 == "0")
                        {
                            Alert_Msg[Alert_count] = Alert + " EU Sanction List";
                            Alert_count++;
                        }

                        obj.Id1 = dt14.Rows.Count; obj.status = 0;
                        if (dt14.Rows.Count > 0) { obj.status = 2; check_records_found++; }
                        flag = "5";
                        obj.whereclause = flag;

                        MySqlCommand cmd8 = new MySqlCommand("SanctionList_Save", con);
                        cmd8.CommandType = CommandType.StoredProcedure;
                        cmd8.Parameters.AddWithValue("_Exact_Match", Convert.ToInt32(aud_cnt));
                        cmd8.Parameters.AddWithValue("customername", obj.Name);
                        cmd8.Parameters.AddWithValue("CustomerId", obj.Id);
                        cmd8.Parameters.AddWithValue("Id", obj.Id1);
                        cmd8.Parameters.AddWithValue("_date", obj.RecordDate);
                        cmd8.Parameters.AddWithValue("_status", obj.status);
                        cmd8.Parameters.AddWithValue("wherclause", obj.whereclause);
                        cmd8.Parameters.AddWithValue("userId", obj.CommentUserId);
                        cmd8.Parameters.AddWithValue("deletestatus", obj.DeleteStatus);

                        string msg8 = Convert.ToString(cmd8.ExecuteNonQuery());
                        if (msg8 == "0")
                        {
                            Alert_Msg[Alert_count] = Alert + "AUD Sanction List";
                            Alert_count++;
                        }


                        obj.Id1 = dt15.Rows.Count; obj.status = 0;
                        if (dt14.Rows.Count > 0) { obj.status = 2; check_records_found++; }
                        flag = "6";
                        obj.whereclause = flag;

                        MySqlCommand cmd9 = new MySqlCommand("SanctionList_Save", con);
                        cmd9.CommandType = CommandType.StoredProcedure;
                        cmd9.Parameters.AddWithValue("_Exact_Match", Convert.ToInt32(pep_cnt));
                        cmd9.Parameters.AddWithValue("customername", obj.Name);
                        cmd9.Parameters.AddWithValue("CustomerId", obj.Id);
                        cmd9.Parameters.AddWithValue("Id", obj.Id1);
                        cmd9.Parameters.AddWithValue("_date", obj.RecordDate);
                        cmd9.Parameters.AddWithValue("_status", obj.status);
                        cmd9.Parameters.AddWithValue("wherclause", obj.whereclause);
                        cmd9.Parameters.AddWithValue("userId", obj.CommentUserId);
                        cmd9.Parameters.AddWithValue("deletestatus", obj.DeleteStatus);

                        string msg9 = Convert.ToString(cmd9.ExecuteNonQuery());
                        if (msg9 == "0")
                        {
                            Alert_Msg[Alert_count] = Alert + " PEP Sanction List";
                            Alert_count++;
                        }


                        if (check_records_found > 0)//records found in sanctionslist
                        {
                            string body = string.Empty, subject = string.Empty;
                            string body1 = string.Empty;
                            //string template = "";
                            HttpWebRequest httpRequest = null, httpRequest1 = null;

                            DataTable dtc = CompanyInfo.get(obj.Client_ID, context);
                            if (dtc.Rows.Count > 0)
                            {
                                string sendmsg = " While adding Customer " + full_name + "  " + obj.WireTransfer_ReferanceNo + " ,there was a match found in the Sanctions List. Please check Customer profile to investigate further. ";
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

                                subject = company_name + " - Customer Sanctions List Match - Alert " + obj.WireTransfer_ReferanceNo;
                                //       string newsubject = company_name + " - " + subject + " - " + Convert.ToString(dt.Rows[0]["WireTransfer_ReferanceNo"]);
                                string send_mail = (string)CompanyInfo.Send_Mail(dtc, EmailID, body, subject, obj.Client_ID, obj.Branch_ID, "", "", "", context);

                            }
                        }
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

        public Model.Customer updatecustomerdetails(Model.Customer obj, HttpContext context)
        {
            string Activity = string.Empty; string[] Alert_Msg = new string[7]; string sendmsg = string.Empty; string notification_icon = ""; string notification_message = "";
            Double Total_match = 0;
            int ProbableMatch_Flag = 1, Matched_id = 0;

            string Username = context.User.Identity.Name; //  Convert.ToString(context.Request.Form["Username"]);
            string error_invalid_data = "";
            string error_msg = ""; string Password_regex = "true";
            //obj.Agent_User_ID = Convert.ToString(CompanyInfo.Decrypt(obj.Agent_User_ID.ToString(), true));
            string FirstName_regex = validation.validate(obj.FirstName, 1, 1, 1, 1, 1, 1, 0, 1, 1);
            string Middle_Name_regex = validation.validate(obj.Middle_Name, 1, 1, 1, 1, 1, 1, 0, 1, 1);
            string LastName_regex = validation.validate(obj.LastName, 1, 1, 1, 1, 1, 1, 0, 1, 1);
            string PostCode_regex = validation.validate(obj.PostCode, 1, 1, 1, 1, 1, 1, 1, 0, 1); //string PostCode_regex = validation.validate(obj.PostCode, 1, 1, 0, 1, 1, 1, 1, 1, 1);
            string HouseNumber_regex = validation.validate(obj.HouseNumber, 1, 1, 1, 1, 1, 1, 1, 1, 0);
            string Street_regex = validation.validate(obj.Street, 1, 1, 1, 1, 1, 1, 1, 1, 0);
            string MobileNumber_regex = validation.validate(obj.MobileNumber, 1, 1, 1, 0, 1, 1, 1, 1, 1);
            string address2_regex = validation.validate(obj.AddressLine2, 1, 1, 1, 1, 1, 1, 1, 1, 0);//
            string CompanyName_regex = validation.validate(obj.CompanyName, 1, 1, 1, 1, 1, 1, 1, 1, 0);
            string Email_regex = validation.validate(obj.Email, 1, 1, 1, 1, 0, 1, 1, 1, 1);
            // string Agent_user_Id_regex = validation.validate(obj.Agent_User_ID.ToString(), 1, 1, 1, 0, 1, 1, 1, 1, 1);
            string refcode_regex = "true"; string referral_regex = "true";
            int address_regex_len = 100; string HouseNumber_len = "true"; string Street_len = "true"; string company_len = "true"; string AddressLine2_len = "true";
            string refcode = "";

            if (obj.HouseNumber.Length >= address_regex_len)
            {
                HouseNumber_len = "false";

            }
            if (obj.Street.Length >= address_regex_len)
            {
                Street_len = "false";

            }
            if (obj.AddressLine2.Length >= address_regex_len)
            {
                AddressLine2_len = "false";

            }
            if (obj.CompanyName.Length >= address_regex_len)
            {
                company_len = "false";

            }
            if (FirstName_regex == "false" || Middle_Name_regex == "false" || LastName_regex == "false")
            {
                if (FirstName_regex == "false")
                { error_invalid_data = error_invalid_data + " FirstName: " + obj.FirstName; }
                if (Middle_Name_regex == "false")
                { error_invalid_data = error_invalid_data + " Middle_Name: " + obj.Middle_Name; }
                if (LastName_regex == "false")
                { error_invalid_data = error_invalid_data + " LastName: " + obj.LastName; }
                error_msg = error_msg + " Name should contain alphabates and maximum of three spaces are valid.";

            }
            if (Email_regex == "false")
            {
                error_msg = error_msg + " Email Should be valid without space.";
                error_invalid_data = error_invalid_data + " Email: " + obj.Email;
            }
            if (Password_regex == "false")
            {
                error_msg = error_msg + " Password should be alphanumeric without space and exclude special characters like # , % , - , ; ,/*";
                error_invalid_data = error_invalid_data + " Password: " + obj.Password;
            }
            if (address2_regex == "false" || Street_regex == "false" || HouseNumber_regex == "false")
            {
                string len_house = "";
                if (address2_regex == "false")
                {
                    error_invalid_data = error_invalid_data + " AddressLine2: " + obj.AddressLine2;

                }
                if (Street_regex == "false")
                {
                    error_invalid_data = error_invalid_data + " Street: " + obj.Street;
                }
                if (HouseNumber_regex == "false")
                {
                    error_invalid_data = error_invalid_data + " HouseNumber: " + obj.HouseNumber;
                }
                if (HouseNumber_len == "false")
                {
                    len_house = "House number should be of valid length.";
                    error_invalid_data = error_invalid_data + "  House no length: " + obj.HouseNumber.Length;
                }
                if (AddressLine2_len == "false")
                {
                    len_house = len_house + "Address2 should be of valid length.";
                    error_invalid_data = error_invalid_data + "  Address2 length: " + obj.AddressLine2.Length;
                }
                if (Street_len == "false")
                {
                    len_house = len_house + "Street should be of valid length.";
                    error_invalid_data = error_invalid_data + "  Street length: " + obj.Street.Length;
                }
                error_msg = error_msg + " Address should be alphanumeric with space and special characters like , , , . , ' , ( , ) , { , } , - , @ , & " + len_house;

            }
            if (referral_regex == "false")
            {
                error_msg = error_msg + " Referral Code Should be alphanumeric without space";
                error_invalid_data = error_invalid_data + "  Refcode: " + obj.refcode;

            }
            if (MobileNumber_regex == "false")
            {
                error_msg = error_msg + " Mobile number Should be numeric without space";
                error_invalid_data = error_invalid_data + "  MobileNumber: " + obj.MobileNumber;
            }
            //if (Agent_user_Id_regex == "false")
            //{
            //    error_msg = error_msg + " Mobile number Should be numeric without space";
            //    error_invalid_data = error_invalid_data + "  Agnet User Id: " + obj.Agent_User_ID;
            //}
            if (CompanyName_regex == "false")
            {
                string len_cmp = "";
                error_invalid_data = error_invalid_data + "  CompanyName: " + obj.CompanyName;
                if (company_len == "false")
                {
                    len_cmp = "with valid Length";
                    error_invalid_data = error_invalid_data + "  CompanyName Length: " + obj.CompanyName.Length;
                }
                error_msg = error_msg + " Company name should contain alphabates with space" + len_cmp;
            }
            if (PostCode_regex == "false")
            {
                error_msg = error_msg + " Postcode Should be alphanumeric without space";
                error_invalid_data = error_invalid_data + "  Postcode: " + obj.PostCode;
            }
            if (MobileNumber_regex == "false")
            {
                error_msg = error_msg + " Mobile number Should be numeric without space";
                error_invalid_data = error_invalid_data + "  MobileNumber: " + obj.MobileNumber;
            }
            if (Email_regex == "" && CompanyName_regex == "" && address2_regex == "" && FirstName_regex == "" && Middle_Name_regex == "" && LastName_regex == "" && PostCode_regex == "" && HouseNumber_regex == "" && Street_regex == "" && MobileNumber_regex == "")
            {
                FirstName_regex = "true";
                Middle_Name_regex = "true";
                LastName_regex = "true";
                PostCode_regex = "true";
                HouseNumber_regex = "true";
                Street_regex = "true";
                MobileNumber_regex = "true";
                address2_regex = "true";
                CompanyName_regex = "true";
                Email_regex = "true";
                //Agent_user_Id_regex = "true";
            }

            if (referral_regex != "false" && AddressLine2_len != "false" && HouseNumber_len != "false" && Street_len != "false" && company_len != "false" && Password_regex != "false" && Email_regex != "false" && CompanyName_regex != "false" && address2_regex != "false" && FirstName_regex != "false" && Middle_Name_regex != "false" && LastName_regex != "false" && PostCode_regex != "false" && HouseNumber_regex != "false" && Street_regex != "false" && MobileNumber_regex != "false")
            {
                using (MySqlConnection con = new MySqlConnection(_MTS.WebConnSetting()))
                {
                    con.Open();
                    MySqlTransaction transaction;
                    transaction = con.BeginTransaction(IsolationLevel.ReadCommitted);

                    if(obj.step_flag == 3)
                    {
                        //Check Captcha
                        try
                        {
                            MySqlCommand cmd_captcha = new MySqlCommand("GetPermissions");
                            cmd_captcha.CommandType = CommandType.StoredProcedure;
                            int per102 = 1;
                            cmd_captcha.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                            cmd_captcha.Parameters.AddWithValue("_whereclause", " and PID = 102");
                            DataTable dtperm_status = db_connection.ExecuteQueryDataTableProcedure(cmd_captcha);
                            per102 = Convert.ToInt32(dtperm_status.Rows[0]["Status_ForCustomer"]);

                            if (per102 == 0 && obj.Captcha != "")
                            {
                                Boolean checkCaptcha = true;

                                Service.srvCaptcha srv = new Service.srvCaptcha();
                                checkCaptcha = srv.VerifyCaptcha_withUserName(obj.Email, obj.Captcha);
                                int status = (int)CompanyInfo.InsertActivityLogDetails(" Captcha response value: " + Convert.ToString(checkCaptcha), 0, 0, Convert.ToInt32(0), 1, "IsValidCustomer", Convert.ToInt32(0), Convert.ToInt32(0), "IsValidCustomer", context);
                                if (!checkCaptcha)
                                {
                                    obj.Message = "Invalid Captcha";
                                    return obj;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Model.ErrorLog objError = new Model.ErrorLog();
                            objError.User = new Model.User();
                            objError.Error = "Api : Customer update --" + ex.ToString();
                            objError.Date = DateTime.Now;
                            objError.User_ID = 1;
                            objError.Client_ID = obj.Client_ID;

                            Service.srvErrorLog srvError = new Service.srvErrorLog();
                            srvError.Create(objError, context);
                        }
                    }
                    try
                    {
                        obj.RecordDate = Convert.ToDateTime(CompanyInfo.gettime(obj.Client_ID, "0", obj.Country_Id, context));

                        obj.SecurityKey = srvCommon.SecurityKey();
                        if (obj.professionName != "")
                        {
                            if (obj.professionName.Contains("'"))
                            {
                                obj.professionName = obj.professionName.Replace("'", "");
                            }
                        }

                        obj.SendMoneyFlag = 0;
                        obj.ExceededAmount = 0;

                        string FirstName = CompanyInfo.testInjection(obj.FirstName);
                        string Middle_Name = CompanyInfo.testInjection(obj.Middle_Name);
                        string LastName = CompanyInfo.testInjection(obj.LastName);
                        string Birth_Date1 = CompanyInfo.testInjection(obj.Birth_Date);
                        string city = CompanyInfo.testInjection(Convert.ToString(obj.cityId));
                        string Country = CompanyInfo.testInjection(Convert.ToString(obj.Country_Id));
                        string PostCode = CompanyInfo.testInjection(Convert.ToString(obj.PostCode));
                        string Employement_Status = CompanyInfo.testInjection(Convert.ToString(obj.Employement_Status));
                        string PhoneNumber = CompanyInfo.testInjection(Convert.ToString(obj.PhoneNumber));
                        string MobileNumber = CompanyInfo.testInjection(Convert.ToString(obj.MobileNumber));
                        string HouseNumber = CompanyInfo.testInjection(Convert.ToString(obj.HouseNumber));
                        string Street = CompanyInfo.testInjection(Convert.ToString(obj.Street));
                        string Gender = CompanyInfo.testInjection(Convert.ToString(obj.Gender));
                        string Nationality = CompanyInfo.testInjection(Convert.ToString(obj.Nationality));
                        string WireTransferRefNumber = CompanyInfo.testInjection(Convert.ToString(obj.WireTransferRefNumber));
                        string SendMoneyFlag = CompanyInfo.testInjection(Convert.ToString(obj.SendMoneyFlag));
                        string ExceededAmount = CompanyInfo.testInjection(Convert.ToString(obj.ExceededAmount));
                        string Profession = CompanyInfo.testInjection(Convert.ToString(obj.professionName));
                        string CompanyName = CompanyInfo.testInjection(Convert.ToString(obj.CompanyName));
                        string HeardFromEvent = CompanyInfo.testInjection(Convert.ToString(obj.HeardFromEvent));
                        string SourseOfRegistration = CompanyInfo.testInjection(Convert.ToString(obj.SourseOfRegistration));
                        string Nationality_ID = CompanyInfo.testInjection(Convert.ToString(obj.Nationality_ID));
                        string Remind_Date = CompanyInfo.testInjection(Convert.ToString(obj.Remind_Date));
                        string Branch_ID = CompanyInfo.testInjection(Convert.ToString(obj.Branch_ID));
                        string Title = CompanyInfo.testInjection(Convert.ToString(obj.TitleId));
                        //string HeardFrom = CompanyInfo.testInjection(Convert.ToString(obj.HeardFrom));
                        string HeardFrom = "1";
                        string ProfessionId = CompanyInfo.testInjection(Convert.ToString(obj.professionId));
                        string SecurityKey = CompanyInfo.testInjection(Convert.ToString(obj.SecurityKey));
                        string HeardFromId = CompanyInfo.testInjection(Convert.ToString(obj.HeardFromId));
                        string provience_id = CompanyInfo.testInjection(Convert.ToString(obj.provience_id));
                        string clientids = CompanyInfo.testInjection(Convert.ToString(obj.Client_ID));
                        string Agent_User_ID = CompanyInfo.testInjection(Convert.ToString(obj.Agent_User_ID));
                        string Referred_By_Agent = CompanyInfo.testInjection(Convert.ToString(obj.Referred_By_Agent));

                        if (FirstName == "1" && Middle_Name == "1" && LastName == "1" && Birth_Date1 == "1" && city == "1" && Country == "1" && PostCode == "1" && Employement_Status == "1" && PhoneNumber == "1"
                            && MobileNumber == "1" && HouseNumber == "1" && Street == "1" && Gender == "1"
                            && Nationality == "1" && WireTransferRefNumber == "1" && Profession == "1" && CompanyName == "1"
                            && HeardFromEvent == "1" && SourseOfRegistration == "1" && Nationality_ID == "1" && Remind_Date == "1"
                             && Branch_ID == "1" && Title == "1" && HeardFrom == "1" && ProfessionId == "1"
                            && SecurityKey == "1" && HeardFromId == "1" && clientids == "1"
                            )
                        {

                            //Parth Changes for GBG check status
                            #region GBG check status
                            try
                            {
                                if (obj.step_flag == 3)
                                {
                                    string Bandtext = "", ApiScore = "", ApiCode = "", RiskLevel = "", Apidescription = "", PopUptxt = "", Holdstatus = "";
                                    #region GBG Checks
                                    //Double Total_match = 0;
                                    Total_match = 0;
                                    //int ProbableMatch_Flag = 1, Matched_id = 0;
                                    ProbableMatch_Flag = 1; Matched_id = 0;
                                    int GetGBGChecks = 0; string Comment = "";
                                    MySqlCommand _cmdGBGAlert = new MySqlCommand("GetPermissions");
                                    _cmdGBGAlert.CommandType = CommandType.StoredProcedure;
                                    _cmdGBGAlert.Parameters.AddWithValue("_whereclause", " and PID in (225,226)");
                                    _cmdGBGAlert.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                                    DataTable dt_GBGPermitions = db_connection.ExecuteQueryDataTableProcedure(_cmdGBGAlert);
                                    try
                                    {
                                        if (dt_GBGPermitions.Rows.Count > 0)
                                        {
                                            int EmailVarificationPer = Convert.ToInt32(dt_GBGPermitions.Rows[0]["Status_ForCustomer"]);
                                            int HoldAccountPer = Convert.ToInt32(dt_GBGPermitions.Rows[1]["Status_ForCustomer"]);
                                            try
                                            {
                                                if (EmailVarificationPer == 0)
                                                {
                                                    try
                                                    {
                                                        DataTable GBGtbl = GetGBGCheckStatus(obj, HoldAccountPer);// GBGEmailValidationCheck 
                                                        if (GBGtbl.Rows.Count > 0)
                                                        {
                                                            Bandtext = Convert.ToString(GBGtbl.Rows[0]["BandText"]);
                                                            ApiScore = Convert.ToString(GBGtbl.Rows[0]["ApiScore"]);
                                                            ApiCode = Convert.ToString(GBGtbl.Rows[0]["ResponseCode"]);
                                                            RiskLevel = Convert.ToString(GBGtbl.Rows[0]["RiskLevel"]);
                                                            Apidescription = Convert.ToString(GBGtbl.Rows[0]["Description"]);
                                                            PopUptxt = Convert.ToString(GBGtbl.Rows[0]["PopUpText"]);
                                                            Holdstatus = Convert.ToString(GBGtbl.Rows[0]["HOLD"]);//0 - Cust on hold
                                                            GetGBGChecks = 1;//1 means gbgcheck done , 0 means no gbgcheck 
                                                        }

                                                        else { GetGBGChecks = 0; }

                                                        if (HoldAccountPer == 0 && RiskLevel == "High")
                                                        {//this block for customer save as hold
                                                            if (Bandtext == "PASS")
                                                            {
                                                                Comment = PopUptxt;
                                                                GetGBGChecks = 1;
                                                                obj.Message = "The Email is Not Valid For Registration. Please Login or add another Email ID";
                                                                obj.ExtraGBG = "AccountHoldFromGBG";
                                                            }
                                                            else if (Bandtext == "fail")
                                                            {
                                                                Comment = PopUptxt;
                                                                GetGBGChecks = 1;
                                                                obj.Message = "The Email is Not Valid For Registration. Please Login or add another Email ID";
                                                                obj.ExtraGBG = "AccountHoldFromGBG";
                                                                //return obj;
                                                            }
                                                        }
                                                        else if (RiskLevel == "Low" && Bandtext == "PASS")
                                                        {
                                                            GetGBGChecks = 0;
                                                        }
                                                        else
                                                        {
                                                            GetGBGChecks = 1;
                                                            obj.Message = "The Email is Not Valid For Registration. Please Login or add another Email ID";
                                                            obj.ExtraGBG = "GBGChecksFailed";
                                                            return obj;
                                                        }
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        Model.ActivityLogTracker _objActivityLog = new Model.ActivityLogTracker();
                                                        _objActivityLog.Activity = " Exception Customer update Error: " + ex.ToString() + " ";
                                                        _objActivityLog.FunctionName = "updateCustomer";
                                                        _objActivityLog.Transaction_ID = 0;
                                                        _objActivityLog.WhoAcessed = 1;
                                                        _objActivityLog.Branch_ID = 0;
                                                        _objActivityLog.Client_ID = 0;

                                                        Service.srvActivityLogTracker srvActivityLog = new Service.srvActivityLogTracker();
                                                        CompanyInfo.InsertErrorLogTracker("Exception GBG Check Error: " + ex.ToString(), 0, 0, 0, 0, "customerUpdate", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "", context);

                                                        GetGBGChecks = 1;
                                                        obj.Message = "The Email is Not Valid For Registration. Please Login or add another Email ID";
                                                        obj.ExtraGBG = "GBGChecksFailed";
                                                        return obj;
                                                    }
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                CompanyInfo.InsertErrorLogTracker("Exception GBG Check Error step1: " + ex.ToString(), 0, 0, 0, 0, "customerUpdate", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "", context);
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        CompanyInfo.InsertErrorLogTracker("Exception GBG Check Error step: " + ex.ToString(), 0, 0, 0, 0, "customerUpdate", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "", context);
                                    }
                                    #endregion GBG Checks
                                }
                            }
                            catch (Exception ex)
                            {
                                CompanyInfo.InsertErrorLogTracker("Exception GBG Check Error final: " + ex.ToString(), 0, 0, 0, 0, "customerUpdate", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "", context);
                            }
                            #endregion GBG check status
                            //End Parth Changes for GBG check status


                            //using (MySqlConnector.MySqlCommand cmd = new MySqlConnector.MySqlCommand("sp_update_customer_details", con))
                            using (MySqlConnector.MySqlCommand cmd = new MySqlConnector.MySqlCommand("sp_update_customer_details1", con))
                            {
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Transaction = transaction;

                                notification_icon = "new-cust.jpg";
                                notification_message = "<span class='cls-admin'>Customer record update successfully <strong class='cls-newcust'>registered.</strong></span><span class='cls-customer'></span>";
                                cmd.Parameters.AddWithValue("_Notification", notification_message);
                                cmd.Parameters.AddWithValue("_Notification_Icon", notification_icon);
                                cmd.Parameters.AddWithValue("_provience_id", obj.provience_id);
                                if (obj.Phone_number_code == "0" || obj.Phone_number_code == "" || obj.Phone_number_code == null)
                                {
                                    cmd.Parameters.AddWithValue("_Phone_number_code", null);
                                }
                                else if (obj.Phone_number_code != "0" || obj.Phone_number_code != "" || obj.Phone_number_code != null)
                                {
                                    cmd.Parameters.AddWithValue("_Phone_number_code", obj.Phone_number_code);
                                }

                                if (obj.Mobile_number_code == "0" || obj.Mobile_number_code == "" || obj.Mobile_number_code == null)
                                {
                                    cmd.Parameters.AddWithValue("_Mobile_number_code", null);
                                }
                                else if (obj.Mobile_number_code != "0" || obj.Mobile_number_code != "" || obj.Mobile_number_code != null)
                                {
                                    cmd.Parameters.AddWithValue("_Mobile_number_code", obj.Mobile_number_code);
                                }


                                if (obj.FirstName != null && obj.FirstName != "")
                                {
                                    cmd.Parameters.AddWithValue("_First_Name", obj.FirstName.Replace("'", "''").Trim());
                                }
                                else
                                {
                                    cmd.Parameters.AddWithValue("_First_Name", obj.FirstName.Trim());
                                }
                                if (obj.Middle_Name != "" && obj.Middle_Name != null)
                                {
                                    cmd.Parameters.AddWithValue("_Middle_Name", obj.Middle_Name.Replace("'", "''").Trim());
                                }
                                else
                                {
                                    cmd.Parameters.AddWithValue("_Middle_Name", obj.Middle_Name.Trim());
                                }
                                if (obj.LastName != null && obj.LastName != "")
                                {
                                    cmd.Parameters.AddWithValue("_Last_Name", obj.LastName.Replace("'", "''").Trim());
                                }
                                else
                                {
                                    cmd.Parameters.AddWithValue("_Last_Name", obj.LastName.Trim());
                                }

                                if (obj.Birth_Date != "" && obj.Birth_Date != null)
                                {
                                    DateTime DOB = DateTime.ParseExact(obj.Birth_Date, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                                    string Birth_Date = DOB.ToString("yyyy-MM-dd HH:mm:ss");
                                    cmd.Parameters.AddWithValue("_DOB", Convert.ToDateTime(Birth_Date));
                                }
                                else
                                {
                                    cmd.Parameters.AddWithValue("_DOB", null);
                                }

                                cmd.Parameters.AddWithValue("_City_ID", obj.cityId);
                                cmd.Parameters.AddWithValue("_Country_ID", obj.Country_Id);
                                cmd.Parameters.AddWithValue("_Post_Code", obj.PostCode);
                                cmd.Parameters.AddWithValue("_Employement_Status", obj.Employement_Status);
                                cmd.Parameters.AddWithValue("_Phone_Number", obj.PhoneNumber);

                                cmd.Parameters.AddWithValue("_Mobile_Number", obj.MobileNumber);
                                cmd.Parameters.AddWithValue("_Email_ID", obj.Email);

                                cmd.Parameters.AddWithValue("_Password", obj.Password);
                                cmd.Parameters.AddWithValue("_Security_Question_ID", null);

                                cmd.Parameters.AddWithValue("_Security_Question_Answer", null);
                                cmd.Parameters.AddWithValue("_Record_Insert_DateTime", obj.RecordDate);
                                cmd.Parameters.AddWithValue("_Delete_Status", obj.DeleteStatus);
                                cmd.Parameters.AddWithValue("_RegularCustomer_ID", 0);
                                cmd.Parameters.AddWithValue("_House_Number", obj.HouseNumber);
                                cmd.Parameters.AddWithValue("_Street", obj.Street);

                                cmd.Parameters.AddWithValue("_Gender", obj.Gender);
                                cmd.Parameters.AddWithValue("_Addressline_2", obj.AddressLine2);

                                cmd.Parameters.AddWithValue("_Agent_MappingID", 0);
                                cmd.Parameters.AddWithValue("_Nationality", obj.Nationality);
                                //cmd.Parameters.AddWithValue("_WireTransfer_ReferanceNo", obj.WireTransferRefNumber.Trim());
                                cmd.Parameters.AddWithValue("_Send_Money_flag", obj.SendMoneyFlag);
                                cmd.Parameters.AddWithValue("_Exceeded_Amount", obj.ExceededAmount);

                                cmd.Parameters.AddWithValue("_Inactivate_Date", 0);

                                cmd.Parameters.AddWithValue("_Verification_flag", 0);
                                cmd.Parameters.AddWithValue("_Profession", obj.professionName);

                                cmd.Parameters.AddWithValue("_company_name", obj.CompanyName);
                                cmd.Parameters.AddWithValue("_Cust_Limit", 0);

                                cmd.Parameters.AddWithValue("_Heard_from_Id", obj.HeardFromId);
                                cmd.Parameters.AddWithValue("_Heard_from_event", obj.HeardFromEvent);
                                cmd.Parameters.AddWithValue("_Sourse_of_Registration", obj.SourseOfRegistration);
                                cmd.Parameters.AddWithValue("_Comment", 0);
                                cmd.Parameters.AddWithValue("_Comment_UserId", 0);
                                cmd.Parameters.AddWithValue("_Remind_Me_Flag", 0);
                                cmd.Parameters.AddWithValue("_Nationality_ID", obj.Nationality_ID);
                                cmd.Parameters.AddWithValue("_Remind_Date", obj.Remind_Date);
                                cmd.Parameters.AddWithValue("_Branch_ID", obj.Branch_ID);

                                cmd.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                                cmd.Parameters.AddWithValue("_Title_ID", obj.TitleId);
                                cmd.Parameters.AddWithValue("_heard_from", obj.HeardFrom);

                                cmd.Parameters.AddWithValue("_ProfessionId", obj.professionId);
                                cmd.Parameters.AddWithValue("_SecurityKey", obj.SecurityKey);
                                cmd.Parameters.AddWithValue("_promotional_flag", obj.chk_promo_mail);
                                cmd.Parameters.AddWithValue("_total_match", Total_match);
                                cmd.Parameters.AddWithValue("_ProbableMatch_Flag", ProbableMatch_Flag);// Siddhi changes
                                cmd.Parameters.AddWithValue("_matching_id", Matched_id);// Siddhi changes
                                cmd.Parameters.AddWithValue("_Customer_ID", obj.Customer_ID);
                                cmd.Parameters.AddWithValue("_Place_Of_Birth", obj.birthPlace);
                                cmd.Parameters.AddWithValue("_countryof_birth", obj.Countryof_Birth);
                                cmd.Parameters.AddWithValue("_setp1", obj.step_flag);
                                cmd.Parameters.AddWithValue("_payout_countries", obj.payout_countries);
                                cmd.Parameters.AddWithValue("_Annual_salary_ID", Convert.ToInt32(obj.Annual_salary_ID));//parvej changes

                                if (obj.Agent_User_ID != "0" && obj.Agent_User_ID.ToString() != null && obj.Agent_User_ID.ToString() != "")
                                {
                                    cmd.Parameters.AddWithValue("_Agent_User_ID", obj.Agent_User_ID);
                                }
                                else
                                {
                                    cmd.Parameters.AddWithValue("_Agent_User_ID", null);
                                }
                                if (obj.Referred_By_Agent.ToString() != null)
                                {
                                    cmd.Parameters.AddWithValue("_Referred_By_Agent", obj.Referred_By_Agent);
                                }

                                cmd.Parameters.Add(new MySqlParameter("_existsMobile", MySqlDbType.Int32));
                                cmd.Parameters["_existsMobile"].Direction = ParameterDirection.Output;

                                cmd.Parameters.Add(new MySqlParameter("updatedRecordID", MySqlDbType.Int32));
                                cmd.Parameters["updatedRecordID"].Direction = ParameterDirection.Output;

                                int exquery = cmd.ExecuteNonQuery();


                                int stattusfg = (int)CompanyInfo.InsertActivityLogDetails("Query Update response:" + exquery + " and cid:" + obj.Customer_ID + " and step :" + obj.step_flag, Convert.ToInt32(0), 0, Convert.ToInt32(0), 1, "Create", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "Create", context);

                                try
                                {
                                    obj.ExistMobile = Convert.ToInt32(cmd.Parameters["_existsMobile"].Value);
                                    if (obj.ExistMobile == 1)
                                    {
                                        //return "exist_mobile";
                                        //obj.Message = "exist_mobile";
                                        obj.Message = "Mobile already exist.";
                                        return obj;
                                    }
                                }
                                catch (Exception _x)
                                {

                                }

                                if (exquery == 0)
                                {
                                    obj.Message = "Record updation failed.";
                                    return obj;
                                }
                                if (exquery != 0)
                                {
                                    DateTime today = Convert.ToDateTime(obj.RecordDate.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture), CultureInfo.InvariantCulture);
                                    CompanyInfo.save_notification_compliance(notification_message, notification_icon, obj.Id, Convert.ToDateTime(today), obj.Client_ID, 1, 0, obj.Branch_ID, 0, 0, 1, 0, context);
                                    transaction.Commit();
                                    obj.Message = "success";
                                    obj.step_flag = obj.step_flag;


                                    if (obj.Message == "success" && obj.step_flag.ToString() == "3")
                                    {

                                    }


                                }

                            }




                        }

                    }
                    catch (Exception _x)
                    {
                        try
                        {
                            obj.Message = "Error";
                            transaction.Rollback();
                            Model.ErrorLog objError = new Model.ErrorLog();
                            objError.User = new Model.User();
                            objError.Error = "Api : Customer Update --" + _x.ToString();
                            objError.Date = DateTime.Now;
                            objError.User_ID = 1;
                            objError.Client_ID = obj.Client_ID;

                            Service.srvErrorLog srvError = new Service.srvErrorLog();
                            srvError.Create(objError, context);
                            throw _x;
                        }
                        catch (Exception ex) { }
                    }
                    finally
                    {
                        con.Close();
                    }

                }
            }
            else
            {
                obj.Id = "0";
                Model.Customer _obj = new Model.Customer();
                //_obj = new Model.Customer();
                string msg = "Validation Failed at customer update record " + " <br/>  " + error_invalid_data;
                obj.Comment = "Validation Failed  at customer update record.";
                obj.Message = error_msg;
                int stattus = (int)CompanyInfo.InsertActivityLogDetails(msg, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), 1, "Create", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "Create", context);

            }


            obj.Customer_ID = CompanyInfo.Encrypt(Convert.ToString(obj.Customer_ID), true); obj.Id = "0";
            return obj;
        }

        //Parth Changes for GBG check status
        #region GBG Check Status
        public DataTable GetGBGCheckStatus(Model.Customer obj, int AccHold)
        {
            DataTable dt = new DataTable();// this table is return
            dt.Columns.Add("BandText", typeof(string));
            dt.Columns.Add("ApiScore", typeof(string));
            dt.Columns.Add("ResponseCode", typeof(string));
            dt.Columns.Add("RiskLevel", typeof(string));
            dt.Columns.Add("Description", typeof(string));
            dt.Columns.Add("PopUpText", typeof(string));
            dt.Columns.Add("HOLD", typeof(string));

            int GetGBGChecks = 0; string PopUptxt = "";
            #region check already Exist
            MySqlCommand cmdResponse = new MySqlCommand("GetGBGResponse");
            cmdResponse.CommandType = CommandType.StoredProcedure;
            cmdResponse.Parameters.AddWithValue("_whereclause", "EmailId='" + obj.Email + "'");
            DataTable DtResponse = db_connection.ExecuteQueryDataTableProcedure(cmdResponse);
            if (DtResponse.Rows.Count > 0)
            {
                if (Convert.ToString(DtResponse.Rows[0]["Parameter"]) != null && Convert.ToString(DtResponse.Rows[0]["Parameter"]) != "")
                {
                    DataTable dtExistingValues = ReadGBGRequestResponse(Convert.ToString(DtResponse.Rows[0]["Parameter"]));
                    if (dtExistingValues.Rows.Count > 0)
                    {
                        string ExistBandText = Convert.ToString(dtExistingValues.Rows[0]["BandText"]);
                        string ExistApiScore = Convert.ToString(dtExistingValues.Rows[0]["ApiScore"]);
                        string ExistResponseCode = Convert.ToString(dtExistingValues.Rows[0]["ResponseCode"]);
                        string ExistRiskLevel = Convert.ToString(dtExistingValues.Rows[0]["RiskLevel"]);
                        string ExistDescription = Convert.ToString(dtExistingValues.Rows[0]["Description"]);
                        string ExistPopUpText = Convert.ToString(dtExistingValues.Rows[0]["PopUpText"]);

                        if ((AccHold == 0 || AccHold == 1) && (ExistBandText == "fail" || ExistRiskLevel == "High"))
                        {
                            dt.Clear(); dt.Rows.Add(ExistBandText, ExistApiScore, ExistResponseCode, ExistRiskLevel, ExistDescription, "fail", Convert.ToString(AccHold));
                        }
                        //else if (AccHold == 1 && (ExistBandText == "fail" || ExistRiskLevel == "High"))
                        //{
                        //    dt.Clear(); dt.Rows.Add(ExistBandText, ExistApiScore, ExistResponseCode, ExistRiskLevel, ExistDescription, "fail", Convert.ToString(AccHold));
                        //}
                        else
                        {
                            dt.Clear(); dt.Rows.Add(ExistBandText, ExistApiScore, ExistResponseCode, ExistRiskLevel, ExistDescription, ExistBandText, Convert.ToString(AccHold));
                        }

                        // dt.Rows.Add(ExistBandText, ExistResponseCode, ExistRiskLevel, ExistDescription, ExistPopUpText,Convert.ToString(AccHold));
                        GetGBGChecks = 1; // gbg check already exist
                    }

                }

            }
            else { }
            #endregion
            #region New SCREENING
            if (GetGBGChecks == 0)
            {
                try
                {
                    int API_ID = 13;// API ID
                    MySqlCommand cmd = new MySqlCommand("active_thirdparti_Addressapi");//("active_thirdparti_Addressapi");
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("_whereclause", " API_ID=" + API_ID);
                    DataTable dtApi = db_connection.ExecuteQueryDataTableProcedure(cmd);
                    var _url = Convert.ToString(dtApi.Rows[0]["API_URL"]);
                    var UserName = Convert.ToString(dtApi.Rows[0]["UserName"]);
                    var Password = Convert.ToString(dtApi.Rows[0]["Password"]);
                    var ProfileID = Convert.ToString(dtApi.Rows[0]["ProfileID"]);
                    var _action = "http://www.id3global.com/ID3gWS/2013/04/IGlobalAuthenticate/AuthenticateSP";

                    string Bandtext = "", ApiScore = "", ApiCode = "", RiskLevel = "", Apidescription = ""; int flag1 = 0, flag2 = 0;
                    string description = "";
                    DateTime dateTime = DateTime.Now;
                    obj.Record_Insert_DateTime = Convert.ToString(dateTime);

                    XmlDocument soapEnvelopeXml = new XmlDocument();
                    var iddoc = "";
                    //Country = Country1;
                    string passdoc = "";

                    string soapResult = "";
                    //DateTime birthDate;
                    //int day = 00, month = 00, year = 000;
                    //if (DateTime.TryParseExact(obj.Birth_Date, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out birthDate))
                    //{
                    //    day = birthDate.Day; month = birthDate.Month; year = birthDate.Year;
                    //}

                    //string passdob = "";
                    //string dob = Convert.ToString(obj.Birth_Date);//(i.Sender_DateOfBirth);
                    //if (dob != "" && dob != null)
                    //{
                    //    try
                    //    {
                    //        if (DateTime.TryParseExact(obj.Birth_Date, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out birthDate))
                    //        {
                    //            day = birthDate.Day; month = birthDate.Month; year = birthDate.Year;
                    //        }
                    //        passdob = "<ns:DOBDay>" + day + @"</ns:DOBDay>" +
                    //                  "<ns:DOBMonth>" + month + @"</ns:DOBMonth>" +
                    //                  "<ns:DOBYear>" + year + @"</ns:DOBYear>";
                    //    }
                    //    catch { }
                    //}
                    //Parth changes for fetching Customer Details & titlename, cityname and countryname by it's ID
                    #region fetching Customer Details & titlename, cityname and countryname by it's ID

                    MySqlCommand cmd_cust_details = new MySqlCommand("GetCustDetailsByID");
                    cmd_cust_details.CommandType = CommandType.StoredProcedure;
                    cmd_cust_details.Parameters.AddWithValue("cust_ID", obj.Customer_ID);
                    DataTable dt_cust_details = db_connection.ExecuteQueryDataTableProcedure(cmd_cust_details);
                    int titleid = Convert.ToInt32(dt_cust_details.Rows[0]["Title_ID"]);
                    string firstname = Convert.ToString(dt_cust_details.Rows[0]["First_Name"]);
                    string middlename = Convert.ToString(dt_cust_details.Rows[0]["Middle_Name"]);
                    string lastname = Convert.ToString(dt_cust_details.Rows[0]["Last_Name"]);
                    string gender = Convert.ToString(dt_cust_details.Rows[0]["Gender"]);
                    string dateofbirth = Convert.ToString(dt_cust_details.Rows[0]["DateOf_Birth"]);
                    //string nationality = Convert.ToString(dt_cust_details.Rows[0]["Nationality"]);
                    int countryid = Convert.ToInt32(dt_cust_details.Rows[0]["Country_ID"]);
                    string street = Convert.ToString(dt_cust_details.Rows[0]["Street"]);
                    int cityid = Convert.ToInt32(dt_cust_details.Rows[0]["City_ID"]);
                    string postcode = Convert.ToString(dt_cust_details.Rows[0]["Post_Code"]);
                    string housenumber = Convert.ToString(dt_cust_details.Rows[0]["House_Number"]);
                    string mobilenumber = Convert.ToString(dt_cust_details.Rows[0]["Mobile_Number"]);
                    string email = Convert.ToString(dt_cust_details.Rows[0]["Email_ID"]);

                    MySqlCommand cmd_title = new MySqlCommand("GetTitle");
                    cmd_title.CommandType = CommandType.StoredProcedure;
                    cmd_title.Parameters.AddWithValue("p_Title_ID", titleid);
                    DataTable dt_title = db_connection.ExecuteQueryDataTableProcedure(cmd_title);
                    string title = Convert.ToString(dt_title.Rows[0]["Title"]);

                    MySqlCommand cmd_city = new MySqlCommand("GetCityNameByID");
                    cmd_city.CommandType = CommandType.StoredProcedure;
                    cmd_city.Parameters.AddWithValue("_cityId", cityid);
                    DataTable dt_city = db_connection.ExecuteQueryDataTableProcedure(cmd_city);
                    string city = Convert.ToString(dt_city.Rows[0]["City_Name"]);

                    MySqlCommand cmd_country = new MySqlCommand("GetCountryNameByID");
                    cmd_country.CommandType = CommandType.StoredProcedure;
                    cmd_country.Parameters.AddWithValue("_countryId", countryid);
                    DataTable dt_country = db_connection.ExecuteQueryDataTableProcedure(cmd_country);
                    string country = Convert.ToString(dt_country.Rows[0]["Country_Name"]);

                    DateTime birthDate;
                    int day = 00, month = 00, year = 000;
                    if (DateTime.TryParseExact(dateofbirth, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out birthDate))
                    {
                        day = birthDate.Day; month = birthDate.Month; year = birthDate.Year;
                    }

                    string passdob = "";
                    string dob = Convert.ToString(dateofbirth);
                    if (dob != "" && dob != null)
                    {
                        try
                        {
                            if (DateTime.TryParseExact(dateofbirth, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out birthDate))
                            {
                                day = birthDate.Day; month = birthDate.Month; year = birthDate.Year;
                            }
                            passdob = "<ns:DOBDay>" + day + @"</ns:DOBDay>" +
                                      "<ns:DOBMonth>" + month + @"</ns:DOBMonth>" +
                                      "<ns:DOBYear>" + year + @"</ns:DOBYear>";
                        }
                        catch { }
                    }

                    #endregion fetching Customer Details & titlename, cityname and countryname by it's ID
                    //End Parth changes for fetching Customer Details & titlename, cityname and countryname by it's ID

                    //Screening                        
                    try
                    {
                        //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                        System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
                        ////<ns:MiddleName>A</ns:MiddleName><ns:Title>Mr</ns:Title><ns:Gender>" + Convert.ToString(dc.Rows[0]["Gender"]) + @"</ns:Gender>
                        soapEnvelopeXml.LoadXml(@"<?xml version=""1.0"" encoding=""utf-8""?>
<soapenv:Envelope xmlns:ns=""http://www.id3global.com/ID3gWS/2013/04"" xmlns:soap=""soap"" xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"">
   <soapenv:Header xmlns:wsa=""http://www.w3.org/2005/08/addressing"">
      <wsse:Security soapenv:mustUnderstand=""1"" xmlns:wsse=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd"" xmlns:wsu=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd"">
         <wsse:UsernameToken>
            <wsse:Username>" + UserName + @"</wsse:Username>
            <wsse:Password>" + Password + @"</wsse:Password>
         </wsse:UsernameToken>
      </wsse:Security>
   </soapenv:Header>
   <soapenv:Body>
      <ns:AuthenticateSP>
         <ns:ProfileIDVersion>
            <ns:ID>" + ProfileID + @"</ns:ID>
            <Version>0</Version>
         </ns:ProfileIDVersion>
         <ns:CustomerReference/>
         <ns:InputData>
            <ns:Personal>
               <ns:PersonalDetails>
                  <ns:Title>" + title + @"</ns:Title>
                  <ns:Forename>" + firstname + @"</ns:Forename>
                  <ns:MiddleName>" + middlename + @"</ns:MiddleName>
                  <ns:Surname>" + lastname + @"</ns:Surname>
                  <ns:Gender>" + gender + @"</ns:Gender>
                  <ns:DOBDay>" + day + @"</ns:DOBDay>
                  <ns:DOBMonth>" + month + @"</ns:DOBMonth>
                  <ns:DOBYear>" + year + @"</ns:DOBYear>
               </ns:PersonalDetails>
            </ns:Personal>
            <ns:Addresses>
               <ns:CurrentAddress>
                  <ns:Country>" + country + @"</ns:Country>
                  <ns:Street>" + street + @"</ns:Street>
                  <ns:City>" + city + @"</ns:City>
                  <ns:Region>CityRoad</ns:Region>
                  <ns:ZipPostcode>" + postcode + @"</ns:ZipPostcode>
                  <ns:Building>" + housenumber + @"</ns:Building>
                  <ns:Premise>NewOne</ns:Premise>
               </ns:CurrentAddress>
               <ns:ContactDetails>
                  <ns:MobileTelephone>
                     <ns:Number>" + mobilenumber + @"</ns:Number>
                  </ns:MobileTelephone>
                  <ns:Email>" + email + @"</ns:Email>
               </ns:ContactDetails>
            </ns:Addresses>
         </ns:InputData>
      </ns:AuthenticateSP>
   </soapenv:Body>
</soapenv:Envelope>");

                        string SendTransferReq = soapEnvelopeXml.InnerXml;
                        try//save request
                        {
                            MySqlCommand _cmd = new MySqlCommand("SaveIncompletCustAPIRequestResponce");
                            _cmd.CommandType = CommandType.StoredProcedure;
                            _cmd.Parameters.AddWithValue("_API_ID", API_ID);
                            _cmd.Parameters.AddWithValue("_Client_ID", 1);
                            _cmd.Parameters.AddWithValue("_Customer_ID", 0);
                            _cmd.Parameters.AddWithValue("_RequestResponse_Flag", 0);
                            _cmd.Parameters.AddWithValue("_APICall_From", "Create");
                            _cmd.Parameters.AddWithValue("_Response_Code", 0);
                            _cmd.Parameters.AddWithValue("_Parameter", SendTransferReq.Replace("'", "''"));
                            _cmd.Parameters.AddWithValue("_Record_Insert_Datetime", DateTime.Now);
                            _cmd.Parameters.AddWithValue("_Branch_ID", obj.Branch_ID);
                            _cmd.Parameters.AddWithValue("_EmailId", obj.Email);
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

                        HttpWebRequest webRequest = srvDocument.CreateWebRequest(_url, _action);
                        srvDocument.InsertSoapEnvelopeIntoWebRequest(soapEnvelopeXml, webRequest);
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
                            dt.Clear();
                            dt.Rows.Add("fail", "0", "0", "Low", "NO_GBG_PERMISSION", "", "1");
                            //return dt;
                        }
                        // save responce
                        try
                        {
                            MySqlCommand _cmd = new MySqlCommand("SaveIncompletCustAPIRequestResponce");
                            _cmd.CommandType = CommandType.StoredProcedure;
                            _cmd.Parameters.AddWithValue("_API_ID", API_ID);
                            _cmd.Parameters.AddWithValue("_Client_ID", 1);
                            _cmd.Parameters.AddWithValue("_Customer_ID", 0);
                            _cmd.Parameters.AddWithValue("_RequestResponse_Flag", 1);
                            _cmd.Parameters.AddWithValue("_APICall_From", "Create");
                            _cmd.Parameters.AddWithValue("_Response_Code", 0);
                            _cmd.Parameters.AddWithValue("_Parameter", soapResult.Replace("'", "''"));
                            _cmd.Parameters.AddWithValue("_Record_Insert_Datetime", DateTime.Now);
                            _cmd.Parameters.AddWithValue("_Branch_ID", obj.Branch_ID);
                            _cmd.Parameters.AddWithValue("_EmailId", obj.Email);
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
                    catch (Exception ex)
                    {
                        dt.Clear();
                        dt.Rows.Add("ERROR", "0", "0", "Low", "NO_GBG_PERMISSION", "", "1");
                        return dt;
                    }

                    if (soapResult != null && soapResult != "")
                    {
                        DataTable NewReqValues = ReadGBGRequestResponse(soapResult);
                        if (NewReqValues.Rows.Count > 0)
                        {
                            string NewBandText = Convert.ToString(NewReqValues.Rows[0]["BandText"]);
                            string NewApiScore = Convert.ToString(NewReqValues.Rows[0]["ApiScore"]);
                            string NewResponseCode = Convert.ToString(NewReqValues.Rows[0]["ResponseCode"]);
                            string NewRiskLevel = Convert.ToString(NewReqValues.Rows[0]["RiskLevel"]);
                            string NewDescription = Convert.ToString(NewReqValues.Rows[0]["Description"]);
                            string NewPopUpText = Convert.ToString(NewReqValues.Rows[0]["PopUpText"]);

                            dt.Rows.Add(NewBandText, NewApiScore, NewResponseCode, NewRiskLevel, NewDescription, NewPopUpText, Convert.ToString(AccHold));
                            GetGBGChecks = 1;
                        }
                        else
                        {
                            dt.Rows.Add("fail", "0", "0", "0", "0", "fail", Convert.ToString(AccHold));
                        }
                    }
                }
                catch (Exception ex) { dt.Clear(); dt.Rows.Add("fail", "0", "0", "0", "0", "fail", Convert.ToString(AccHold)); }
            }
            #endregion New SCREENING

            try
            {
                if (dt.Rows.Count > 0)
                {
                    string FinalBandText = Convert.ToString(dt.Rows[0]["BandText"]);
                    string FinalApiScore = Convert.ToString(dt.Rows[0]["ApiScore"]);
                    string FinalResponseCode = Convert.ToString(dt.Rows[0]["ResponseCode"]);
                    string FinalRiskLevel = Convert.ToString(dt.Rows[0]["RiskLevel"]);
                    string FinalDescription = Convert.ToString(dt.Rows[0]["Description"]);
                    string FinalPopUpText = Convert.ToString(dt.Rows[0]["PopUpText"]);
                    string FinalHOLD = Convert.ToString(dt.Rows[0]["HOLD"]);

                    if (AccHold == 0 && (FinalBandText == "fail" || FinalRiskLevel == "High"))
                    {
                        dt.Clear(); dt.Rows.Add(FinalBandText, FinalApiScore, FinalResponseCode, FinalRiskLevel, FinalDescription, "fail", Convert.ToString(AccHold));
                    }
                    else if (AccHold == 1 && (FinalBandText == "fail" || FinalRiskLevel == "High"))
                    {
                        dt.Clear(); dt.Rows.Add(FinalBandText, FinalApiScore, FinalResponseCode, FinalRiskLevel, FinalDescription, "fail", Convert.ToString(AccHold));
                    }
                    else
                    {
                        //dt.Clear(); dt.Rows.Add(FinalBandText, FinalApiScore, FinalResponseCode, FinalRiskLevel, FinalDescription, "PASS", Convert.ToString(AccHold));
                        dt.Clear(); dt.Rows.Add(FinalBandText, FinalApiScore, FinalResponseCode, FinalRiskLevel, FinalDescription, FinalBandText, Convert.ToString(AccHold));
                    }
                }
                else { dt.Clear(); dt.Rows.Add("alert", "0", "0", "0", "0", "alert", Convert.ToString(AccHold)); }
            }
            catch (Exception ex) { dt.Clear(); dt.Rows.Add("fail", "0", "0", "0", "0", "fail", Convert.ToString(AccHold)); }

            return dt;
        }
        public DataTable ReadGBGRequestResponse(string soapResult)
        {
            string Bandtext = "", ApiScore = "", ApiCode = "", description = "", RiskLevel = "", PopUptxt = "", Apidescription = "";

            DataTable dt = new DataTable();// this table is return
            dt.Columns.Add("BandText", typeof(string));
            dt.Columns.Add("ApiScore", typeof(string));
            dt.Columns.Add("ResponseCode", typeof(string));
            dt.Columns.Add("RiskLevel", typeof(string));
            dt.Columns.Add("Description", typeof(string));
            dt.Columns.Add("PopUpText", typeof(string));

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
                        else if (child.Name == "Score")
                        {
                            ApiScore = child.InnerText;
                        }
                        else if (child.Name == "ResultCodes") // Process the ResultCodes node
                        {
                            foreach (XmlNode resultCodeNode in child.ChildNodes)
                            {
                                foreach (XmlNode commentNode in resultCodeNode.ChildNodes)
                                {
                                    if (commentNode.Name == "Match") // Process Comment node
                                    {
                                        foreach (XmlNode globalItemNode in commentNode.ChildNodes)
                                        {
                                            ApiCode = globalItemNode["Code"]?.InnerText;
                                            description = globalItemNode["Description"]?.InnerText;
                                            if (ApiCode == "3400" || ApiCode == "3401" || ApiCode == "3402" || ApiCode == "3403" || ApiCode == "3404" || ApiCode == "3405" || ApiCode == "3515") // Match your desired code
                                            {
                                                RiskLevel = "High";
                                                if (!string.IsNullOrEmpty(description))
                                                {
                                                    if (!string.IsNullOrEmpty(Apidescription))
                                                    {
                                                        PopUptxt = Apidescription;
                                                        Apidescription += ", ";
                                                    }
                                                    Apidescription += description;
                                                }
                                            }
                                            else
                                            {
                                                PopUptxt = description;
                                            }
                                        }
                                    }
                                    else if (commentNode.Name == "Comment") // Process Comment node
                                    {
                                        foreach (XmlNode globalItemNode in commentNode.ChildNodes)
                                        {
                                            RiskLevel = "Low";
                                            ApiCode = globalItemNode["Code"]?.InnerText;
                                            description = globalItemNode["Description"]?.InnerText;
                                            PopUptxt = description;
                                        }
                                    }
                                    else if (commentNode.Name == "Mismatch") // Process Mismatch node
                                    {
                                        foreach (XmlNode globalItemNode in commentNode.ChildNodes)
                                        {
                                            ApiCode = globalItemNode["Code"]?.InnerText;
                                            if (ApiCode == "9502" || ApiCode == "9501") { RiskLevel = "High"; }
                                            description = globalItemNode["Description"]?.InnerText;
                                            PopUptxt = description;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            dt.Rows.Add(Bandtext, ApiScore, ApiCode, RiskLevel, description, PopUptxt);
            return dt;
        }

        #endregion Gbg Check Status
        //End Parth Changes for GBG check status

        public Model.Customer UpdateForgotPassword(Model.Customer obj, HttpContext context)
        {
            string Activity = string.Empty;
            //var context = System.Web.HttpContext.Current;
            string Username = obj.UserName;
            string newPassword = Convert.ToString(obj.newpassword);
            mts_connection _MTS = new mts_connection();
            using (MySqlConnection con = new MySqlConnection(_MTS.WebConnSetting()))
            {
                if (con.State != ConnectionState.Open)
                    con.Open();
                string from_querystring = Convert.ToString(obj.WireTransfer_ReferanceNo);
                string split_string = "", ref_no = "";
                if (from_querystring != "" && from_querystring != null)
                {
                    split_string = from_querystring.Replace("reference=", "");
                    ref_no = CompanyInfo.Decrypt(split_string, Convert.ToBoolean(1));
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
                DateTime dt_cust1 = Convert.ToDateTime(obj.checkparam);
                String Encrypted_time1 = Convert.ToString(CompanyInfo.gettime(obj.Client_ID, obj.Id, obj.Country_Id, context));
                DateTime dt1 = Convert.ToDateTime(Encrypted_time1);
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

                    MySqlCommand cmdn = new MySqlCommand("customer_details_by_param");
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
                            transaction = con.BeginTransaction(IsolationLevel.ReadCommitted);
                            try
                            {
                                obj.CommentUserId = 1;
                                obj.SecurityKey = srvCommon.SecurityKey();
                                //String Encrypted_time1 = Convert.ToString(CompanyInfo.gettime(obj.Client_ID, obj.Id, Convert.ToInt32(dtt.Rows[0]["Country_ID"]), context));
                                using (MySqlConnector.MySqlCommand cmd_cnt = new MySqlConnector.MySqlCommand("Update_password_count", con))
                                {
                                    cmd_cnt.CommandType = CommandType.StoredProcedure;
                                    cmd_cnt.Parameters.AddWithValue("_Customer_ID", obj.Id);
                                    cmd_cnt.Parameters.AddWithValue("_Date_chk", Convert.ToDateTime(Encrypted_time1));

                                    DataTable chck_limit = db_connection.ExecuteQueryDataTableProcedure(cmd_cnt);
                                    if (chck_limit.Rows.Count == 1)
                                    {
                                        if (Convert.ToInt32(chck_limit.Rows[0]["reset_pass_cnt"]) <= 3)
                                        {
                                            using (MySqlConnector.MySqlCommand cmd = new MySqlConnector.MySqlCommand("update_password", con))
                                            {
                                                cmd.CommandType = CommandType.StoredProcedure;
                                                cmd.Transaction = transaction;
                                                cmd.Parameters.AddWithValue("_Password", Convert.ToString(obj.newpassword));
                                                cmd.Parameters.AddWithValue("_Id", obj.Id);
                                                cmd.Parameters.AddWithValue("_Branch_ID", obj.Branch_ID);
                                                cmd.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                                                //cmd.Parameters.AddWithValue("_flag", 0);
                                                cmd.Parameters.AddWithValue("_key", obj.SecurityKey);
                                                int n = cmd.ExecuteNonQuery();

                                                if (check_flag == "SecureFlagOff")
                                                {
                                                    obj.Record_Insert_DateTime = CompanyInfo.gettime(obj.Client_ID, custid, Convert.ToInt32(dtt.Rows[0]["Country_Id"]), context);
                                                    using (MySqlConnector.MySqlCommand cmd2 = new MySqlConnector.MySqlCommand("update_securtity_flag", con))
                                                    {
                                                        cmd2.CommandType = CommandType.StoredProcedure;
                                                        cmd2.Transaction = transaction;
                                                        cmd2.Parameters.AddWithValue("_Customer_ID", obj.Id);
                                                        cmd2.Parameters.AddWithValue("_date", obj.Record_Insert_DateTime);
                                                        cmd2.Parameters.AddWithValue("_clientId", obj.Client_ID);
                                                        cmd2.Parameters.AddWithValue("_branchId", obj.Branch_ID);
                                                        cmd2.Parameters.AddWithValue("_security_flag", 1);

                                                        n = cmd2.ExecuteNonQuery();
                                                    }
                                                }
                                                Model.Customer _ObjCustomer = new Model.Customer();
                                                Service.srvLogin srvLogin = new Service.srvLogin();
                                                DataTable dt = new DataTable();
                                                Model.Login objLogin = new Model.Login();
                                                Model.Customer objcust = new Model.Customer();


                                                objLogin.Client_ID = obj.Client_ID;
                                                MySqlConnector.MySqlCommand cmd1 = new MySqlConnector.MySqlCommand("GetCustDetailsByID", con);
                                                cmd1.CommandType = CommandType.StoredProcedure;
                                                cmd1.Parameters.AddWithValue("cust_ID", obj.Id);
                                                DataTable dt_cust = db_connection.ExecuteQueryDataTableProcedure(cmd1);

                                                objLogin.UserName = Convert.ToString(dt_cust.Rows[0]["Email_ID"]);
                                                dt = srvLogin.IsValidEmail(objLogin);

                                                if (n > 0)
                                                {
                                                    obj.Message = "Success";
                                                    Activity = "<b>" + Username + "</b>" + " Password changed.  </br>";
                                                    int stattus = (int)CompanyInfo.InsertActivityLogDetails(Activity, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(obj.Id), "UpdatePassword", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "Update Password", context);


                                                    string email = obj.Email;
                                                    string body = string.Empty, subject = string.Empty;
                                                    string body1 = string.Empty;
                                                    //string template = "";
                                                    System.Net.HttpWebRequest httpRequest = null, httpRequest1 = null;
                                                    DataTable dtc = CompanyInfo.get(obj.Client_ID, context);
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
                                                        string newsubject = company_name + " - " + subject + Convert.ToString("");
                                                        string msg = (string)CompanyInfo.Send_Mail(dtc, objLogin.UserName, body, newsubject, obj.Client_ID, obj.Branch_ID, "", "", "", context);
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            obj.Message = "Exceeded";
                                            Activity = "<b>" + Username + "</b>" + " Password Try exceeded.  </br>";
                                            int stattus = (int)CompanyInfo.InsertActivityLogDetails(Activity, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(obj.Id), "UpdatePassword", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "Update Password", context);

                                        }
                                    }
                                }
                                transaction.Commit();
                            }
                            catch (Exception _x)
                            {
                                obj.Message = "Failed " + _x;
                                transaction.Rollback();
                                //throw _x;
                            }
                            finally
                            {
                                if (con.State != ConnectionState.Closed)
                                    con.Close();
                            }
                        }
                    }
                }
                return obj;

            }
        }

        public Model.Customer CreateCustomer(Model.Customer obj, HttpContext context)
        {
            string Earn_Activity = string.Empty; DataTable dt_Wallet_config = new DataTable();
            string Activity = string.Empty; string[] Alert_Msg = new string[7]; string sendmsg = string.Empty; string notification_icon = ""; string notification_message = "";
            string Username = context.User.Identity.Name; int per6 = 1;
            string error_invalid_data = ""; string ActivityTracker = string.Empty;
            string error_msg = ""; string Password_regex = "true"; string referral_regex = "true";

            string Email_regex = validation.validate(obj.Email, 1, 1, 1, 1, 0, 1, 1, 1, 1);

            string pwd = Convert.ToString(obj.Password);
            if (Email_regex == "false")
            {
                error_msg = error_msg + " Email Should be valid without space.";
                error_invalid_data = error_invalid_data + " Email: " + obj.Email;
            }
            if (pwd.Contains(" ") || pwd.Contains("-- ") || pwd.Contains(";") || pwd.Contains("/*") || pwd.Contains(" #") || pwd.Contains("# "))
            {
                Password_regex = "false";
                error_msg = error_msg + " Password should be alphanumeric without space and exclude special characters like # , % , - , ; ,/*";
                error_invalid_data = error_invalid_data + " Password: " + obj.Password;
            }

           
            string refcode = "";
            try
            {
                refcode = Convert.ToString(CompanyInfo.Decrypt(obj.refcode, true));
                //obj.refcode = Convert.ToString(CompanyInfo.Decrypt(obj.refcode, true));
                if (refcode != "" && refcode != null)
                {
                    obj.refcode = refcode;

                }

            }
            catch (Exception ex) { }
            if ((obj.refcode).ToString() != null || (obj.refcode).ToString() != "")
            {
                referral_regex = validation.validate((obj.refcode).ToString(), 1, 1, 0, 1, 1, 1, 1, 1, 1);
            }
            if (referral_regex == "false")
            {
                error_msg = error_msg + " Referral Code Should be alphanumeric without space";
                error_invalid_data = error_invalid_data + "  Refcode: " + obj.refcode;
            }


            if (referral_regex != "false" && Password_regex != "false" && Email_regex != "false")
            {
                db_connection _dbConnection = new db_connection();
                using (MySqlConnector.MySqlConnection con = new MySqlConnector.MySqlConnection(_dbConnection.ConnectionStringSection()))
                {
                    con.Open();
                    MySqlConnector.MySqlTransaction transaction;
                    transaction = con.BeginTransaction(IsolationLevel.ReadCommitted);
                    //Check Captcha
                    try
                    {

                        MySqlCommand cmd_captcha = new MySqlCommand("GetPermissions");
                        cmd_captcha.CommandType = CommandType.StoredProcedure;
                        int per102 = 1;
                        cmd_captcha.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                        cmd_captcha.Parameters.AddWithValue("_whereclause", " and PID = 102");
                        DataTable dtperm_status = db_connection.ExecuteQueryDataTableProcedure(cmd_captcha);
                        per102 = Convert.ToInt32(dtperm_status.Rows[0]["Status_ForCustomer"]);

                        if (per102 == 0 && !string.IsNullOrWhiteSpace(obj.Captcha))
                        {
                            Boolean checkCaptcha = true;
                            
                            Service.srvCaptcha srv = new Service.srvCaptcha();
                            checkCaptcha = srv.VerifyCaptcha_withUserName(obj.UserName, obj.Captcha);
                            int status = (int)CompanyInfo.InsertActivityLogDetails(" Captcha response value: " + Convert.ToString(checkCaptcha), 0, 0, Convert.ToInt32(0), 1, "IsValidCustomer", Convert.ToInt32(0), Convert.ToInt32(0), "IsValidCustomer", context);
                            if (!checkCaptcha)
                            {
                                obj.Message = "Invalid Captcha";
                                return obj;
                            }
                        }

                    }
                    catch(Exception ex)
                    {
                        Model.ErrorLog objError = new Model.ErrorLog();
                        objError.User = new Model.User();
                        objError.Error = "Api : Customer add --" + ex.ToString();
                        objError.Date = DateTime.Now;
                        objError.User_ID = 1;
                        objError.Client_ID = obj.Client_ID;

                        Service.srvErrorLog srvError = new Service.srvErrorLog();
                        srvError.Create(objError, context);
                    }
                    try
                    {
                        obj.RecordDate = Convert.ToDateTime(CompanyInfo.gettime(obj.Client_ID, "0", obj.Country_Id, context));
                        obj.SecurityKey = CompanyInfo.SecurityKey();
                        string Email = CompanyInfo.testInjection(Convert.ToString(obj.Email));
                        string Password = CompanyInfo.testInjection(Convert.ToString(obj.Password));
                        string ref_code = CompanyInfo.testInjection(Convert.ToString(obj.refcode));
                        if (Email == "1" && Password == "1" && ref_code == "1")
                        {

                            using (MySqlConnector.MySqlCommand cmd = new MySqlConnector.MySqlCommand("sp_save_customer", con))
                            {

                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Transaction = transaction;

                                notification_icon = "new-cust.jpg";
                                notification_message = "<span class='cls-admin'>New customer successfully <strong class='cls-newcust'>registered.</strong></span><span class='cls-customer'></span>";
                                cmd.Parameters.AddWithValue("_Notification", notification_message);
                                cmd.Parameters.AddWithValue("_Notification_Icon", notification_icon);
                                cmd.Parameters.AddWithValue("_provience_id", 0);
                                if (obj.Phone_number_code == "0" || obj.Phone_number_code == "" || obj.Phone_number_code == null)
                                {
                                    cmd.Parameters.AddWithValue("_Phone_number_code", null);
                                }
                                else if (obj.Phone_number_code != "0" || obj.Phone_number_code != "" || obj.Phone_number_code != null)
                                {
                                    cmd.Parameters.AddWithValue("_Phone_number_code", obj.Phone_number_code);
                                }



                                if (obj.Mobile_number_code == "0" || obj.Mobile_number_code == "" || obj.Mobile_number_code == null)
                                {
                                    cmd.Parameters.AddWithValue("_Mobile_number_code", null);
                                }
                                else if (obj.Mobile_number_code != "0" || obj.Mobile_number_code != "" || obj.Mobile_number_code != null)
                                {
                                    cmd.Parameters.AddWithValue("_Mobile_number_code", obj.Mobile_number_code);
                                }



                                if (obj.FirstName != null && obj.FirstName != "")
                                {
                                    cmd.Parameters.AddWithValue("_First_Name", obj.FirstName.Replace("'", "''").Trim());
                                }
                                else
                                {
                                    cmd.Parameters.AddWithValue("_First_Name", null);
                                }



                                if (obj.Middle_Name != "" && obj.Middle_Name != null)
                                {
                                    cmd.Parameters.AddWithValue("_Middle_Name", obj.Middle_Name.Replace("'", "''").Trim());
                                }
                                else
                                {
                                    cmd.Parameters.AddWithValue("_Middle_Name", null);
                                }
                                if (obj.LastName != null && obj.LastName != "")
                                {
                                    cmd.Parameters.AddWithValue("_Last_Name", obj.LastName.Replace("'", "''").Trim());
                                }
                                else
                                {
                                    cmd.Parameters.AddWithValue("_Last_Name", null);
                                }

                                if (obj.Birth_Date != "" && obj.Birth_Date != null)
                                {
                                    DateTime DOB = DateTime.ParseExact(obj.Birth_Date, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                                    string Birth_Date = DOB.ToString("yyyy-MM-dd HH:mm:ss");
                                    cmd.Parameters.AddWithValue("_DOB", Convert.ToDateTime(Birth_Date));
                                }
                                else
                                {
                                    cmd.Parameters.AddWithValue("_DOB", null);
                                }

                                cmd.Parameters.AddWithValue("_City_ID", 0);
                                cmd.Parameters.AddWithValue("_Country_ID", obj.Country_Id); // country id pass 
                                cmd.Parameters.AddWithValue("_Post_Code", null);
                                cmd.Parameters.AddWithValue("_Employement_Status", 0);
                                cmd.Parameters.AddWithValue("_Phone_Number", null);

                                cmd.Parameters.AddWithValue("_Mobile_Number", obj.MobileNumber);
                                cmd.Parameters.AddWithValue("_Email_ID", obj.Email);

                                cmd.Parameters.AddWithValue("_Password", obj.Password);
                                cmd.Parameters.AddWithValue("_Security_Question_ID", null);

                                cmd.Parameters.AddWithValue("_Security_Question_Answer", null);
                                cmd.Parameters.AddWithValue("_Record_Insert_DateTime", obj.RecordDate);
                                cmd.Parameters.AddWithValue("_Delete_Status", 0);
                                cmd.Parameters.AddWithValue("_RegularCustomer_ID", 0);
                                cmd.Parameters.AddWithValue("_House_Number", null);
                                cmd.Parameters.AddWithValue("_Street", null);

                                cmd.Parameters.AddWithValue("_Gender", null);
                                cmd.Parameters.AddWithValue("_Addressline_2", null);

                                cmd.Parameters.AddWithValue("_Agent_MappingID", 0);
                                cmd.Parameters.AddWithValue("_Nationality", null);
                                cmd.Parameters.AddWithValue("_WireTransfer_ReferanceNo", null);
                                cmd.Parameters.AddWithValue("_Send_Money_flag", 0);
                                cmd.Parameters.AddWithValue("_Exceeded_Amount", 0);

                                cmd.Parameters.AddWithValue("_Inactivate_Date", 0);

                                cmd.Parameters.AddWithValue("_Verification_flag", 0);
                                cmd.Parameters.AddWithValue("_Profession", null);

                                cmd.Parameters.AddWithValue("_company_name", null);
                                cmd.Parameters.AddWithValue("_Cust_Limit", 0);

                                cmd.Parameters.AddWithValue("_Heard_from_Id", 0);
                                cmd.Parameters.AddWithValue("_Heard_from_event", null);
                                cmd.Parameters.AddWithValue("_Sourse_of_Registration", null);
                                cmd.Parameters.AddWithValue("_Comment", 0);
                                cmd.Parameters.AddWithValue("_Comment_UserId", 0);
                                cmd.Parameters.AddWithValue("_Remind_Me_Flag", 0);
                                cmd.Parameters.AddWithValue("_Nationality_ID", 0);
                                cmd.Parameters.AddWithValue("_Remind_Date", obj.Remind_Date);
                                cmd.Parameters.AddWithValue("_Branch_ID", obj.Branch_ID);

                                cmd.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                                cmd.Parameters.AddWithValue("_Title_ID", 0);
                                cmd.Parameters.AddWithValue("_heard_from", 0);

                                cmd.Parameters.AddWithValue("_ProfessionId", 0);

                                cmd.Parameters.AddWithValue("_SecurityKey", obj.SecurityKey);
                                cmd.Parameters.AddWithValue("_promotional_flag", 1);
                                cmd.Parameters.AddWithValue("_total_match", 0);
                                cmd.Parameters.AddWithValue("_ProbableMatch_Flag", 1);// Siddhi changes
                                cmd.Parameters.AddWithValue("_matching_id", 0);// Siddhi changes
                                cmd.Parameters.AddWithValue("_Agent_User_ID", null);
                                cmd.Parameters.AddWithValue("regstepvalue", 0);

                                // Digvijay Changes
                                cmd.Parameters.AddWithValue("_countryof_birth", obj.Countryof_Birth);
                                cmd.Parameters.AddWithValue("_Place_of_birth", "");// Digvijay - add place of birth change

                                if (obj.Referred_By_Agent.ToString() != null)
                                {
                                    cmd.Parameters.AddWithValue("_Referred_By_Agent", obj.Referred_By_Agent);
                                }

                                // New Fields added
                                cmd.Parameters.AddWithValue("_Annual_salary_ID", Convert.ToDouble(obj.Annual_Salary)); //parvej chnages 

                                int stattus_fd = (int)CompanyInfo.InsertActivityLogDetails("Annual_Salary value:" + Convert.ToDouble(obj.Annual_Salary), 0, 0, 0, 0, "customeradd", 2, 1, "customeradd", context);

                                cmd.Parameters.Add(new MySqlConnector.MySqlParameter("_Customer_ID", MySqlConnector.MySqlDbType.Int32));
                                cmd.Parameters["_Customer_ID"].Direction = ParameterDirection.Output;

                                cmd.Parameters.Add(new MySqlConnector.MySqlParameter("_existsMobile", MySqlConnector.MySqlDbType.Int32));
                                cmd.Parameters["_existsMobile"].Direction = ParameterDirection.Output;

                                cmd.Parameters.Add(new MySqlConnector.MySqlParameter("_existEmail", MySqlConnector.MySqlDbType.Int32));
                                cmd.Parameters["_existEmail"].Direction = ParameterDirection.Output;

                                cmd.ExecuteNonQuery();

                                try
                                {
                                    obj.ExistMobile = Convert.ToInt32(cmd.Parameters["_existsMobile"].Value);
                                    if (obj.ExistMobile == 1)
                                    {
                                        //return "exist_mobile";
                                        //obj.Message = "exist_mobile";
                                        obj.Message = "Mobile already exist.";
                                        return obj;
                                    }
                                }
                                catch (Exception _x)
                                {

                                }
                                try
                                {
                                    obj.ExistEmail = Convert.ToInt32(cmd.Parameters["_existEmail"].Value);
                                    if (obj.ExistEmail == 1)
                                    {
                                        //return "exist_email";
                                        obj.Message = "Email already exist. Please login";
                                        return obj;
                                    }
                                }
                                catch (Exception _x)
                                {

                                }



                                if (Convert.ToInt32(obj.Id) == 0) // insert
                                {
                                    try
                                    {
                                        obj.Id = Convert.ToString(cmd.Parameters["_Customer_ID"].Value);

                                    }
                                    catch (Exception)
                                    {

                                        //transaction.Rollback();
                                        //throw new System.InvalidOperationException("This company allready exist ..");
                                    }
                                }

                                if (Convert.ToInt32(obj.Id) == 0)
                                {
                                    obj.Message = "Record failed.";
                                    return obj;
                                }
                                if (Convert.ToInt32(obj.Id) != 0)
                                {
                                    DateTime today = Convert.ToDateTime(obj.RecordDate.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture), CultureInfo.InvariantCulture);
                                    CompanyInfo.save_notification_compliance(notification_message, notification_icon, obj.Id, Convert.ToDateTime(today), obj.Client_ID, 1, 0, obj.Branch_ID, 0, 0, 1, 0, context);

                                    // update customer refernce number
                                    obj.WireTransferRefNumber = srvCommon.Select_Customer_Refernce_Number(obj);

                                    MySqlCommand cmdupdate = new MySqlCommand("sp_update_customer_ref_no", con);
                                    cmdupdate.CommandType = CommandType.StoredProcedure;
                                    cmdupdate.Transaction = transaction;
                                    cmdupdate.Parameters.AddWithValue("_Id", obj.Id);
                                    cmdupdate.Parameters.AddWithValue("_CustomerRefNo", obj.WireTransferRefNumber);
                                    cmdupdate.Parameters.AddWithValue("_Branch_ID", obj.Branch_ID);
                                    cmdupdate.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                                    cmdupdate.Parameters.AddWithValue("_FLAG", "REGISTRATION");
                                    cmdupdate.ExecuteNonQuery();

                                    try
                                    {
                                        //create referral code for 
                                        string referral_code = srvCommon.Create_Customer_Referral_Code(obj);
                                        MySqlCommand cmdupdate1 = new MySqlCommand("sp_update_customer_referral_code", con);
                                        cmdupdate1.CommandType = CommandType.StoredProcedure;
                                        cmdupdate1.Parameters.AddWithValue("_Id", obj.Id);
                                        cmdupdate1.Parameters.AddWithValue("_referral_code", referral_code);
                                        cmdupdate1.Parameters.AddWithValue("_Branch_ID", obj.Branch_ID);
                                        cmdupdate1.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                                        cmdupdate1.ExecuteNonQuery();
                                    }
                                    catch (Exception ex)
                                    {
                                        int stattus_f = (int)CompanyInfo.InsertActivityLogDetails("sp_update_customer_referral_code Error:" + ex.ToString(), 0, 0, 0, 0, "customeradd", 2, 1, "customeradd", context);
                                    }

                                    // company compilance
                                    try
                                    {
                                        MySqlCommand cmd2 = new MySqlCommand("Add_default_limit_App", con);
                                        cmd2.CommandType = CommandType.StoredProcedure;
                                        cmd2.Transaction = transaction;
                                        cmd2.Parameters.AddWithValue("_Customer_id", obj.Id);
                                        cmd2.Parameters.AddWithValue("_Basecurrency_id", obj.base_currency_id);
                                        cmd2.Parameters.AddWithValue("_Client_id", obj.Client_ID);
                                        cmd2.Parameters.AddWithValue("_RecordDate", obj.RecordDate);
                                        cmd2.Parameters.AddWithValue("_branch_id", obj.Branch_ID);

                                        int msgd8 = cmd2.ExecuteNonQuery();

                                        ActivityTracker = "Add_default_limit_App :- " + msgd8 + "+ " + obj.Id + "" + obj.base_currency_id + " " + obj.Client_ID + "" + obj.Branch_ID;
                                        //end aml  limit
                                    }
                                    catch (Exception ex)
                                    {
                                        int stattus_f = (int)CompanyInfo.InsertActivityLogDetails("Add_default_limit_App Error:" + ex.ToString(), 0, 0, 0, 0, "customeradd", 2, 1, "customeradd", context);
                                    }


                                    try
                                    {
                                        MySqlCommand cmd_app = new MySqlCommand("custwise_notification_enteries", con);
                                        cmd_app.CommandType = CommandType.StoredProcedure;
                                        cmd_app.Transaction = transaction;
                                        cmd_app.Parameters.AddWithValue("_Customer_ID", obj.Id);
                                        cmd_app.ExecuteNonQuery();
                                    }
                                    catch (Exception ex)
                                    {
                                    }
                                    try
                                    {
                                        //communication prefrances
                                        MySqlCommand cmds = new MySqlCommand("sp_save_comm_preference_App", con);
                                        cmds.CommandType = CommandType.StoredProcedure;
                                        cmds.Transaction = transaction;
                                        cmds.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                                        cmds.Parameters.AddWithValue("_customer_id", obj.Id);
                                        int msgd = cmds.ExecuteNonQuery();

                                    }
                                    catch { }

                                    string[] check_alert = new string[7];
                                    int stattus1 = (int)CompanyInfo.InsertActivityLogDetails(ActivityTracker + obj.UserName.Trim(), 0, 0, 0, 0, "customeradd", 2, 1, "customeradd", context);
                                    transaction.Commit();


                                    try
                                    {
                                        DataTable dt5 = new DataTable();
                                        MySqlCommand _cmd1 = new MySqlCommand("Get_Cust_Info");
                                        _cmd1.CommandType = CommandType.StoredProcedure;
                                        _cmd1.Transaction = transaction;
                                        if (obj.Agent_User_ID != "0")
                                        {
                                            _cmd1.Parameters.AddWithValue("_User_ID", obj.Agent_User_ID);
                                        }
                                        else
                                        {
                                            _cmd1.Parameters.AddWithValue("_User_ID", null);
                                            //_cmd1.Parameters.AddWithValue("_User_ID",0);
                                        }
                                        _cmd1.Parameters.AddWithValue("_client_ID", obj.Client_ID);
                                        dt5 = db_connection.ExecuteQueryDataTableProcedure(_cmd1);
                                        if (dt5.Rows.Count > 0)
                                        {
                                            if (dt5.Rows[0]["Agent_branch"] != null)
                                            {
                                                obj.Agent_branch = Convert.ToString(dt5.Rows[0]["Agent_branch"]);
                                            }
                                            if (dt5.Rows[0]["Branch_ID"] != null)
                                            {
                                                obj.New_Agent_Branch = Convert.ToString(dt5.Rows[0]["Branch_ID"]);
                                            }
                                        }
                                    }
                                    catch
                                    { }
                                    try
                                    {
                                        DataTable dt_notif = CompanyInfo.set_notification_data(1); //temporary removed
                                        if (dt_notif.Rows.Count > 0)
                                        {
                                            int SMS = Convert.ToInt32(dt_notif.Rows[0]["SMS"]);
                                            int Email1 = Convert.ToInt32(dt_notif.Rows[0]["Email"]);
                                            int Notif_status = Convert.ToInt32(dt_notif.Rows[0]["Notification"]);
                                            string notification_msg = Convert.ToString(dt_notif.Rows[0]["notification_msg"]);
                                            if (notification_msg.Contains("[Ref_no]") == true)
                                            {
                                                notification_msg = notification_msg.Replace("[Ref_no]", obj.WireTransferRefNumber);
                                            }
                                            int i = CompanyInfo.check_notification_perm(Convert.ToString(obj.Id), obj.Client_ID, obj.Branch_ID, 1, 1, Convert.ToDateTime(obj.RecordDate), 0, SMS, Email1, Notif_status, "Registration -1", notification_msg, context);
                                        }
                                    }
                                    catch (Exception ex) { }

                                    #region  First Transfer fee free discount after registration
                                    try
                                    {
                                        DataTable dt = new DataTable();
                                        MySqlCommand cmd1 = new MySqlCommand("Get_CompanyInfo");
                                        cmd1.CommandType = CommandType.StoredProcedure;
                                        cmd1.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                                        cmd1.Parameters.AddWithValue("_SecurityKey", obj.SecurityKey);
                                        cmd1.Parameters.AddWithValue("_Customer_ID", 0);
                                        dt = (DataTable)db_connection.ExecuteQueryDataTableProcedure(cmd1);

                                        //check permissions
                                        MySqlCommand cmd6 = new MySqlCommand("GetPermissions");
                                        cmd6.CommandType = CommandType.StoredProcedure;

                                        cmd6.Parameters.AddWithValue("_Client_ID", obj.Client_ID); //46,105,17
                                        cmd6.Parameters.AddWithValue("_whereclause", " and PID in (16,46,105, 181)");//181 added by anushka 16 for wallet added by siddhi and 105 added by siddhi
                                        DataTable dtperm_status = db_connection.ExecuteQueryDataTableProcedure(cmd6);
                                        obj.per_status = Convert.ToInt32(dtperm_status.Rows[0]["Status_ForCustomer"]);
                                        //cmd6.Dispose();
                                        int per4 = 1;
                                        int perm = 1;
                                        if (dtperm_status.Rows.Count > 0)
                                        {
                                            obj.per_status = Convert.ToInt32(dtperm_status.Rows[0]["Status_ForCustomer"]);
                                            per4 = Convert.ToInt32(dtperm_status.Rows[1]["Status_ForCustomer"]);
                                            perm = Convert.ToInt32(dtperm_status.Rows[2]["Status_ForCustomer"]);
                                            per6 = Convert.ToInt32(dtperm_status.Rows[3]["Status_ForCustomer"]);
                                        }
                                        if (per4 == 0)
                                        {
                                            cmd6 = new MySqlCommand("GetDefaultDiscount_Values");
                                            cmd6.CommandType = CommandType.StoredProcedure;
                                            cmd6.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                                            DataTable dtdisc = db_connection.ExecuteQueryDataTableProcedure(cmd6);
                                            //cmd6.Dispose();
                                            double Minimum_Transfer_Amount = 0, Maximum_Transfer_Amount = 0;
                                            if (dtdisc.Rows.Count > 0)
                                            {
                                                Minimum_Transfer_Amount = Convert.ToDouble(dtdisc.Rows[0]["Min_Amt"]);
                                                Maximum_Transfer_Amount = Convert.ToDouble(dtdisc.Rows[0]["Max_Amt"]);
                                            }
                                            string referrer_discountcode = GenerateDiscountCode(obj.Client_ID);

                                            cmd6 = new MySqlCommand("sp_Save_DiscountDetails", con);
                                            cmd6.CommandType = CommandType.StoredProcedure;
                                            cmd6.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                                            cmd6.Parameters.AddWithValue("_Branch_ID", obj.Branch_ID);
                                            cmd6.Parameters.AddWithValue("_Discount_Code", referrer_discountcode);
                                            cmd6.Parameters.AddWithValue("_discount_type_id", Convert.ToInt32(dtdisc.Rows[0]["Discount_Type_ID"]));
                                            cmd6.Parameters.AddWithValue("_amount_type_id", Convert.ToInt32(dtdisc.Rows[0]["Amount_Type_ID"]));
                                            cmd6.Parameters.AddWithValue("_discount_value", Convert.ToDecimal(dtdisc.Rows[0]["Discount_Value"]));
                                            cmd6.Parameters.AddWithValue("_customer_eligibility_id", Convert.ToInt32(dtdisc.Rows[0]["Customer_Eligibility_ID"]));
                                            cmd6.Parameters.AddWithValue("_usagelimit", Convert.ToInt32(dtdisc.Rows[0]["Usage_Limit"]));
                                            cmd6.Parameters.AddWithValue("_usagelimit_flag", Convert.ToInt32(dtdisc.Rows[0]["Usage_Limit_Flag"]));
                                            DateTime dto = DateTime.Now;
                                            DateTime dto1 = dto.AddMonths(Convert.ToInt32(dtdisc.Rows[0]["Months"]));

                                            cmd6.Parameters.AddWithValue("_start_date", obj.RecordDate);


                                            if (dt.Rows.Count > 0)
                                            {
                                                string timezone = Convert.ToString(dt.Rows[0]["BaseCurrency_Timezone"]);
                                                if (timezone != "" && timezone != null)
                                                {
                                                    var britishZone = TimeZoneInfo.FindSystemTimeZoneById("" + timezone + "");
                                                    dto1 = TimeZoneInfo.ConvertTime(dto1, TimeZoneInfo.Local, britishZone);
                                                }
                                            }
                                            cmd6.Parameters.AddWithValue("_end_date", dto1.ToString("yyyy-MM-dd HH:mm:ss"));
                                            cmd6.Parameters.AddWithValue("_Delete_Status", 0);
                                            cmd6.Parameters.AddWithValue("_Record_Insert_DateTime", obj.RecordDate);
                                            cmd6.Parameters.AddWithValue("_minAmount", Minimum_Transfer_Amount);
                                            cmd6.Parameters.AddWithValue("_maxAmount", Maximum_Transfer_Amount);
                                            int n = db_connection.ExecuteNonQueryProcedure(cmd6);
                                            //int n = cmd6.ExecuteNonQuery();
                                            if (n > 0)//save customer eligibility details
                                            {
                                                cmd6 = new MySqlCommand("CheckDuplicateDiscountCode");
                                                cmd6.CommandType = CommandType.StoredProcedure;
                                                cmd6.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                                                cmd6.Parameters.AddWithValue("_Discount_Code", referrer_discountcode);
                                                DataTable ds = db_connection.ExecuteQueryDataTableProcedure(cmd6);

                                                string disc_id = Convert.ToString(ds.Rows[0]["ID"]);


                                                cmd6 = new MySqlCommand("Save_CustomerEligibilityDetails", con);
                                                cmd6.CommandType = CommandType.StoredProcedure;
                                                cmd6.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                                                cmd6.Parameters.AddWithValue("_Id", Convert.ToInt32(disc_id));
                                                cmd6.Parameters.AddWithValue("_customer_eligibility_id", Convert.ToInt32(dtdisc.Rows[0]["Customer_Eligibility_ID"]));
                                                cmd6.Parameters.AddWithValue("_Customer_ID", obj.Id);
                                                cmd6.Parameters.AddWithValue("Delete_Status", 0);
                                                cmd6.Parameters.AddWithValue("_Branch_ID", obj.Branch_ID);

                                                cmd6.Parameters.AddWithValue("_DeliveryType_Id", 5);
                                                int n1 = db_connection.ExecuteNonQueryProcedure(cmd6);
                                                //int a = db_connection.ExecuteNonQueryProcedure(cmd6);
                                                //cmd6.Dispose();
                                                if (n1 > 0)
                                                {
                                                    notification_icon = "discount.jpg";
                                                    notification_message = "<span class='cls-admin'> new <strong class='cls-reward'>Discount coupon " + referrer_discountcode + "</strong> is added.</span><span class='cls-customer'><strong>Discount coupon " + referrer_discountcode + "</strong><span>A new discount coupon is added to your account.</span></span>";

                                                    CompanyInfo.save_notification(notification_message, notification_icon, Convert.ToInt32(obj.Id), Convert.ToDateTime(obj.RecordDate), Convert.ToInt32(obj.Client_ID), 1, 0, Convert.ToInt32(obj.Branch_ID), 0, 0, 1, 0, context);

                                                    //CompanyInfo.InsertActivityLogDetails("First Transfer Fee Free promo code created for customer " + obj.WireTransferRefNumber + "", Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(obj.Id), "Create", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "register");
                                                    _ = CompanyInfo.InsertActivityLogDetailsasync("First Transfer Fee Free promo code created for customer " + obj.WireTransferRefNumber + "", Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(obj.Id), "Create", Convert.ToInt32(obj.Branch_ID), 1, "register", context);
                                                    try
                                                    {
                                                        DataTable dt_notif = CompanyInfo.set_notification_data(32);
                                                        if (dt_notif.Rows.Count > 0)
                                                        {
                                                            int SMS = Convert.ToInt32(dt_notif.Rows[0]["SMS"]);
                                                            int Email1 = Convert.ToInt32(dt_notif.Rows[0]["Email"]);
                                                            int Notif_status = Convert.ToInt32(dt_notif.Rows[0]["Notification"]);
                                                            string notification_msg = Convert.ToString(dt_notif.Rows[0]["notification_msg"]);
                                                            if (notification_msg.Contains("[discountcode] ") == true)
                                                            {
                                                                notification_msg = notification_msg.Replace("[discountcode] ", Convert.ToString(referrer_discountcode));
                                                            }

                                                            int i1 = CompanyInfo.check_notification_perm(Convert.ToString(obj.Id), obj.Client_ID, obj.Branch_ID, 6, 32, Convert.ToDateTime(obj.RecordDate), 0, SMS, Email1, Notif_status, "Web- Discount Notification - 32", notification_msg, context);
                                                        }
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                    }
                                                }
                                                else
                                                {
                                                    //CompanyInfo.InsertActivityLogDetails("First Transfer Fee Free promo code created but failed while assigning to customer " + obj.WireTransferRefNumber + "", Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(obj.Id), "Create", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "register");
                                                    _ = CompanyInfo.InsertActivityLogDetailsasync("First Transfer Fee Free promo code created but failed while assigning to customer " + obj.WireTransferRefNumber + "", Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(obj.Id), "Create", Convert.ToInt32(obj.Branch_ID), 1, "register", context);
                                                }
                                            }
                                            else
                                            {
                                                //CompanyInfo.InsertActivityLogDetails("Failed while creating First Transfer Fee Free promo code for customer " + obj.WireTransferRefNumber + "", Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(obj.Id), "Create", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "register");
                                                _ = CompanyInfo.InsertActivityLogDetailsasync("Failed while creating First Transfer Fee Free promo code for customer " + obj.WireTransferRefNumber + "", Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(obj.Id), "Create", Convert.ToInt32(obj.Branch_ID), 1, "register", context);
                                            }
                                            //int a = db_connection.ExecuteNonQueryProcedure(cmd6);
                                        }
                                        if (perm == 0)
                                        {
                                            #region Discount for with Everyone Eligibility Criteria


                                            cmd6 = new MySqlCommand("Chkdiscnt_everyoneeligibility");
                                            cmd6.CommandType = CommandType.StoredProcedure;
                                            cmd6.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                                            //cmd.Parameters.AddWithValue("_Discount_Code", referee_discountcode);
                                            DataTable ds_discountduplicate1 = db_connection.ExecuteQueryDataTableProcedure(cmd6);

                                            for (int i = 0; i < ds_discountduplicate1.Rows.Count; i++)
                                            {
                                                string disc_id = Convert.ToString(ds_discountduplicate1.Rows[i]["ID"]);
                                                string discount_code = Convert.ToString(ds_discountduplicate1.Rows[i]["Discount_Code"]);
                                                MySqlCommand cmd9 = new MySqlCommand();
                                                cmd9 = new MySqlCommand("Save_CustomerEligibilityDetails");
                                                cmd9.CommandType = CommandType.StoredProcedure;
                                                cmd9.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                                                cmd9.Parameters.AddWithValue("_Id", Convert.ToInt32(disc_id));
                                                cmd9.Parameters.AddWithValue("_customer_eligibility_id", Convert.ToInt32(ds_discountduplicate1.Rows[0]["Customer_Eligibility_ID"]));
                                                cmd9.Parameters.AddWithValue("_Customer_ID", obj.Id);
                                                cmd9.Parameters.AddWithValue("Delete_Status", 0);
                                                cmd9.Parameters.AddWithValue("_Branch_ID", obj.Branch_ID);

                                                cmd9.Parameters.AddWithValue("_DeliveryType_Id", 5);
                                                int n1 = db_connection.ExecuteNonQueryProcedure(cmd9);
                                                notification_icon = "discount.jpg";
                                                notification_message = "<span class='cls-admin'> new <strong class='cls-reward'>Discount coupon " + discount_code + "</strong> is added.</span><span class='cls-customer'><strong>Discount coupon " + discount_code + "</strong><span>A new discount coupon is added to your account.</span></span>";

                                                CompanyInfo.save_notification(notification_message, notification_icon, Convert.ToInt32(obj.Id), Convert.ToDateTime(obj.RecordDate), Convert.ToInt32(obj.Client_ID), 1, 0, Convert.ToInt32(obj.Branch_ID), 0, 0, 1, 0, context);
                                                try
                                                {
                                                    DataTable dt_notif = CompanyInfo.set_notification_data(32);
                                                    if (dt_notif.Rows.Count > 0)
                                                    {
                                                        int SMS = Convert.ToInt32(dt_notif.Rows[0]["SMS"]);
                                                        int Email1 = Convert.ToInt32(dt_notif.Rows[0]["Email"]);
                                                        int Notif_status = Convert.ToInt32(dt_notif.Rows[0]["Notification"]);
                                                        string notification_msg = Convert.ToString(dt_notif.Rows[0]["notification_msg"]);
                                                        if (notification_msg.Contains("[discountcode] ") == true)
                                                        {
                                                            notification_msg = notification_msg.Replace("[discountcode] ", Convert.ToString(discount_code));
                                                        }

                                                        int i1 = CompanyInfo.check_notification_perm(Convert.ToString(obj.Id), obj.Client_ID, obj.Branch_ID, 6, 32, Convert.ToDateTime(obj.RecordDate), 0, SMS, Email1, Notif_status, "Web- Discount Notification - 32", notification_msg, context);
                                                    }
                                                }
                                                catch (Exception ex)
                                                {
                                                }
                                            }
                                            #endregion
                                        }
                                    }
                                    catch (Exception ae)
                                    {
                                    }
                                    #endregion
                                    #region check password

                                    try
                                    {
                                        MySqlCommand cmd_chk_field = new MySqlCommand("update_watchlist_Registration");
                                        cmd_chk_field.CommandType = CommandType.StoredProcedure;
                                        //cmd_chk_field.Transaction = transaction;
                                        cmd_chk_field.Parameters.AddWithValue("_Customerid", obj.Id);
                                        cmd_chk_field.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                                        cmd_chk_field.Parameters.AddWithValue("_Branch_ID", obj.Branch_ID);
                                        cmd_chk_field.Parameters.AddWithValue("_function_name", "Customer_registration");
                                        cmd_chk_field.Parameters.AddWithValue("_RecordDate", obj.RecordDate);
                                        cmd_chk_field.Parameters.AddWithValue("_Email_id", obj.Email);
                                        cmd_chk_field.Parameters.AddWithValue("_password", obj.Password);
                                        cmd_chk_field.Parameters.AddWithValue("_city_id", 0);

                                        cmd_chk_field.Parameters.Add(new MySqlParameter("flag1", MySqlDbType.Int32));
                                        cmd_chk_field.Parameters["flag1"].Direction = ParameterDirection.Output;

                                        cmd_chk_field.Parameters.Add(new MySqlParameter("flag2", MySqlDbType.Int32));
                                        cmd_chk_field.Parameters["flag2"].Direction = ParameterDirection.Output;

                                        cmd_chk_field.Parameters.Add(new MySqlParameter("flag3", MySqlDbType.Int32));
                                        cmd_chk_field.Parameters["flag3"].Direction = ParameterDirection.Output;

                                        cmd_chk_field.ExecuteNonQuery();
                                        int flag1 = Convert.ToInt32(cmd.Parameters["flag1"].Value);
                                        int flag2 = Convert.ToInt32(cmd.Parameters["flag2"].Value);
                                        int flag3 = Convert.ToInt32(cmd.Parameters["flag3"].Value);

                                        if (flag1 == 1)
                                        {
                                            notification_icon = "beneficiary.jpg";
                                            notification_message = "<span class='cls-admin'>Customer <strong class='cls-new-benf'>Added From suspicious password.</strong></span>";
                                        }
                                        if (flag1 == 2)
                                        {

                                            notification_icon = "beneficiary.jpg";
                                            notification_message = "<span class='cls-admin'>Customer <strong class='cls-new-benf'>Added From suspicious password.</strong></span>";

                                        }
                                        if (flag1 == 3)
                                        {
                                            notification_icon = "beneficiary.jpg";
                                            notification_message = "<span class='cls-admin'>Customer <strong class='cls-new-benf'>Added From suspicious city.</strong></span>";
                                        }
                                        CompanyInfo.save_notification_compliance(notification_message, notification_icon, Convert.ToString(obj.Id), Convert.ToDateTime(obj.RecordDate), obj.Client_ID, 1, 0, obj.Branch_ID, 0, 1, 1, 0, context);

                                    }
                                    catch { }
                                    #endregion

                                    try
                                    {
                                        MySqlCommand customer_map = new MySqlCommand("sp_customer_map", con);
                                        customer_map.CommandType = CommandType.StoredProcedure;
                                        //customer_map.Transaction = transaction;
                                        customer_map.Parameters.AddWithValue("_customer_id", Convert.ToInt32(obj.Id));
                                        customer_map.Parameters.AddWithValue("_Multiple_attempt", "1");
                                        customer_map.Parameters.Add(new MySqlParameter("_ID", MySqlDbType.Int32));
                                        customer_map.Parameters["_ID"].Direction = ParameterDirection.Output;
                                        int dt11 = customer_map.ExecuteNonQuery();
                                    }
                                    catch (Exception ex) { }


                                    try
                                    {
                                        DataTable dt = new DataTable();
                                        MySqlCommand cmd1 = new MySqlCommand("currency_details_byID");
                                        cmd1.CommandType = CommandType.StoredProcedure;
                                        cmd1.Parameters.AddWithValue("_CurrencyID", obj.base_currency_id);
                                        DataTable dt_cur = (DataTable)db_connection.ExecuteQueryDataTableProcedure(cmd1);

                                        //check wallet permission
                                        if (obj.per_status == 0 || per6 == 0)//permission is on
                                        {
                                            MySqlCommand cmd6 = new MySqlCommand("getwalletRefforbasecurrency");
                                            cmd6.CommandType = CommandType.StoredProcedure;
                                            string basecurrency = string.Empty;
                                            int cid = 0;
                                            if (dt_cur.Rows.Count != 0)
                                            {
                                                basecurrency = Convert.ToString(dt_cur.Rows[0]["Currency_Code"]);
                                                obj.Currency_Code = basecurrency;
                                                cid = Convert.ToInt32(dt_cur.Rows[0]["Currency_ID"]);
                                            }
                                            Earn_Activity = Earn_Activity + cid + "1.1";
                                            cmd6.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                                            cmd6.Parameters.AddWithValue("_WireTransfer_ReferanceNo", obj.WireTransferRefNumber);
                                            cmd6.Parameters.AddWithValue("_BaseCurrency", basecurrency);
                                            cmd6.Parameters.AddWithValue("_AgentFlag", 1);
                                            DataTable dtwal = db_connection.ExecuteQueryDataTableProcedure(cmd6);
                                            string wrefNo = string.Empty;
                                            if (dtwal.Rows.Count == 0)//create wallet ref for referee
                                            {
                                                wrefNo = GenerateWalletReference(obj.Client_ID);
                                                Earn_Activity = Earn_Activity + "wallet is created 1.2";
                                                //using (cmd6 = new MySqlCommand("insertinwallet_table", con))
                                                {
                                                    cmd6 = new MySqlCommand("insertinwallet_table", con);
                                                    cmd6.CommandType = CommandType.StoredProcedure;
                                                    cmd6.Parameters.AddWithValue("_WireTransfer_ReferanceNo", obj.WireTransferRefNumber);
                                                    cmd6.Parameters.AddWithValue("_Currency_ID", Convert.ToInt32(cid));
                                                    //iw1.Client_ID = client_id;
                                                    cmd6.Parameters.AddWithValue("_Delete_Status", 0);
                                                    cmd6.Parameters.AddWithValue("_Record_Insert_DateTime", obj.RecordDate);
                                                    cmd6.Parameters.AddWithValue("_wallet_reference", wrefNo);
                                                    cmd6.Parameters.AddWithValue("_Wallet_balance", 0);
                                                    cmd6.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                                                    cmd6.Parameters.AddWithValue("_Branch_ID", obj.Branch_ID);
                                                    cmd6.Parameters.AddWithValue("_Customer_ID", obj.Id);
                                                    cmd6.Parameters.AddWithValue("_AgentFlag", 1);
                                                    cmd6.ExecuteNonQuery();
                                                    //int a = db_connection.ExecuteNonQueryProcedure(cmd6);
                                                    //cmd6.Dispose();
                                                }
                                                Earn_Activity = Earn_Activity + "wallet is created 1.3";
                                            }
                                            try
                                            {
                                                MySqlCommand cmd_6 = new MySqlCommand("getwalletRefforbasecurrency");
                                                cmd_6.CommandType = CommandType.StoredProcedure;
                                                cmd_6.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                                                cmd_6.Parameters.AddWithValue("_WireTransfer_ReferanceNo", obj.WireTransferRefNumber);
                                                cmd_6.Parameters.AddWithValue("_BaseCurrency", basecurrency);
                                                cmd_6.Parameters.AddWithValue("_AgentFlag", 1);
                                                dtwal = db_connection.ExecuteQueryDataTableProcedure(cmd_6);
                                                if (dtwal.Rows.Count > 0)
                                                {
                                                    Earn_Activity = Earn_Activity + "wallet is created 1.4";
                                                    if (per6 == 0)
                                                    {
                                                        Earn_Activity = Earn_Activity + "wallet is created 1.5";
                                                        string query1 = "select * from customer_wallet_usage_limit where 1=1 and delete_status=0 and Currency_Id =" + cid + " and Wallet_apply = 4";
                                                        MySqlCommand _cmd1 = new MySqlCommand("Default_sp");
                                                        _cmd1.CommandType = CommandType.StoredProcedure;
                                                        _cmd1.Parameters.AddWithValue("_Query", query1);
                                                        dt_Wallet_config = db_connection.ExecuteQueryDataTableProcedure(_cmd1);
                                                        Earn_Activity = Earn_Activity + "wallet is created 1.6" + dt_Wallet_config.Rows[0]["Perk_value"];
                                                        MySqlCommand wallet_amt = new MySqlCommand("UpdateWallet_balance");
                                                        wallet_amt.CommandType = CommandType.StoredProcedure;
                                                        wallet_amt.Parameters.AddWithValue("_ReferenceNo", obj.WireTransferRefNumber);
                                                        wallet_amt.Parameters.AddWithValue("_CurrencyID", cid);
                                                        wallet_amt.Parameters.AddWithValue("_CustomerID", obj.Id);
                                                        wallet_amt.Parameters.AddWithValue("_WalletAmount", dt_Wallet_config.Rows[0]["Perk_value"]);

                                                        int wallet_chk = db_connection.ExecuteNonQueryProcedure(wallet_amt);
                                                        Earn_Activity = Earn_Activity + "wallet is created 1.7" + wallet_chk;
                                                        if (wallet_chk != 0)
                                                        {
                                                            _ = (int)CompanyInfo.InsertActivityLogDetails("Wallet added successfully by Register And Earn", Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(obj.Id), "savecustomer", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "savecustomer", context);
                                                            obj.basecur_Amount = Convert.ToString(dt_Wallet_config.Rows[0]["Perk_value"]);
                                                        }
                                                        else
                                                        {
                                                            _ = (int)CompanyInfo.InsertActivityLogDetails("Wallet not added successfully by Register And Earn", Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(obj.Id), "savecustomer", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "savecustomer", context);
                                                        }
                                                    }

                                                }
                                                _ = (int)CompanyInfo.InsertActivityLogDetails(Earn_Activity, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(obj.Id), "activityEarn", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "activityEarn", context);

                                            }
                                            catch (Exception ex)
                                            { }
                                        }

                                        ReferralScheme(obj, context);

                                        string risk_factors = "1,3,9,10";
                                        MySqlCommand cmd_update_rules = new MySqlCommand("Update_Rules_Registration");
                                        cmd_update_rules.CommandType = CommandType.StoredProcedure;
                                        cmd_update_rules.Parameters.AddWithValue("_Customer_id", obj.Id);
                                        cmd_update_rules.Parameters.AddWithValue("_risk_factors", risk_factors);
                                        /*cmd_update_rules.Parameters.AddWithValue("_clientId", obj.Client_ID);
                                        cmd_update_rules.Parameters.AddWithValue("_branchId", obj.Branch_ID);*/
                                        int st = db_connection.ExecuteNonQueryProcedure(cmd_update_rules);
                                        string Activities = "";
                                        if (st != 0)
                                        Activities = "Check Rules Added Successfully";
                                        else
                                        Activities = "Check Rules Not Added.";

                                        // int stattus1 = (int)CompanyInfo.InsertActivityLogDetails(Activities, 0, 0, 0, Convert.ToInt32(obj.Id), "Save_Check_Rules", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "Check_Rule");
                                        _ = CompanyInfo.InsertActivityLogDetailsasync(Activities, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(obj.Id), "create_customer", Convert.ToInt32(obj.Branch_ID), 1, "Check_Rule", context);
                                        // CompanyInfo.add_check_rules(Convert.ToInt32(obj.Id), risk_factors, obj.Branch_ID, obj.Client_ID);
                                    }
                                    catch (Exception ae)
                                    {
                                        Model.ErrorLog objError = new Model.ErrorLog();
                                        objError.User = new Model.User();
                                        objError.Error = "Api : Temp Transaction --" + ae.ToString();
                                        objError.Date = DateTime.Now;
                                        objError.User_ID = 1;
                                        objError.Client_ID = obj.Client_ID;

                                        Service.srvErrorLog srvError = new Service.srvErrorLog();
                                        srvError.Create(objError, context);
                                        throw ae;
                                    }


                                }

                            }

                            obj.Message = "success";
                            Activity = "<b>" + Username + "</b>" + " is on sign up.</br>";
                            //int stattus = (int)CompanyInfo.InsertActivityLogDetails(Activity, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(obj.Id), "create_customer", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "Update Address");
                            _ = CompanyInfo.InsertActivityLogDetailsasync(Activity, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(obj.Id), "create_customer", Convert.ToInt32(obj.Branch_ID), 1, "", context);
                            obj.per_status_verifyMob = 1;
                            string Activity1 = string.Empty;
                            if (obj.chk_promo_mail == 0)
                            {
                                //Activity1 = "<b>" + Username + "</b>" + " checked the decline promotional email checkbox.</br>";
                                Activity1 = "<b>" + Username + "</b>" + " checked the decline promotional email checkbox , Customer Id =" + obj.Id + "</br>";
                            }
                            else if (obj.chk_promo_mail == 1)
                            {

                                Activity1 = "<b>" + Username + "</b>" + " unchecked the decline promotional email checkbox , Customer Id =" + obj.Id + "</br>";
                                //Activity1 = "<b>" + Username + "</b>" + " unchecked the decline promotional email checkbox.</br>";
                            }

                            //int stattus3 = (int)CompanyInfo.InsertActivityLogDetails(Activity1, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(obj.Id), "UpdateAddress", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "Update Address");
                            _ = CompanyInfo.InsertActivityLogDetailsasync(Activity1, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(obj.Id), "Create", Convert.ToInt32(obj.Branch_ID), 1, "", context);
                            try
                            {
                                int cust_id = Convert.ToInt32(obj.Id);
                                obj.Customer_ID = CompanyInfo.Encrypt(Convert.ToString(obj.Id), true);
                                DataTable dt_base = (DataTable)CustomerBaseData(obj);
                                obj.Country_Id = Convert.ToInt32(dt_base.Rows[0]["BaseCountry_ID"]);
                                obj.Currency_Id = Convert.ToInt32(dt_base.Rows[0]["Currency_ID"]);
                                obj.Currency_Code = Convert.ToString(dt_base.Rows[0]["Currency_Code"]);
                                obj.Currency_Sign = Convert.ToString(dt_base.Rows[0]["Currency_Sign"]);
                                obj.Country_Flag = Convert.ToString(dt_base.Rows[0]["Country_Flag"]);
                                obj.Country_Name = Convert.ToString(dt_base.Rows[0]["Country_Name"]);
                            }
                            catch (Exception ex)
                            {

                            }

                        }
                        else
                        {

                            string msg = "SQl Enjection detected";
                            int stattus = (int)CompanyInfo.InsertActivityLogDetailsSecurity(msg, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), 1, "srvcustomer", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "create", 0, context);
                        }

                    }
                    catch (Exception _x)
                    {
                        try
                        {
                            obj.Message = "Error";
                            transaction.Rollback();
                            Model.ErrorLog objError = new Model.ErrorLog();
                            objError.User = new Model.User();
                            objError.Error = "Api : Create customer --" + _x.ToString();
                            objError.Date = DateTime.Now;
                            objError.User_ID = 1;
                            objError.Client_ID = obj.Client_ID;

                            Service.srvErrorLog srvError = new Service.srvErrorLog();
                            srvError.Create(objError, context);
                            throw _x;
                        }
                        catch (Exception ex) { }
                    }
                    finally
                    {
                        con.Close();
                    }

                }
            }
            else
            {
                obj.Id = "0";
                Model.Customer _obj = new Model.Customer();
                //_obj = new Model.Customer();
                string msg = "Validation Failed " + " <br/>  " + error_invalid_data;
                obj.Comment = "Validation Failed";
                obj.Message = error_msg;
                int stattus = (int)CompanyInfo.InsertActivityLogDetails(msg, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), 1, "Create", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "Create", context);
            }

            obj.Customer_ID = CompanyInfo.Encrypt(Convert.ToString(obj.Id), true); obj.Id = "0";
            return obj;
        }


        public List<Model.Customer> Get_Referrer_Name(Model.Customer obj)
        {
            string refcode = "";
            try
            {
                refcode = Convert.ToString(CompanyInfo.Decrypt(obj.refcode, true));
                //obj.refcode = Convert.ToString(CompanyInfo.Decrypt(obj.refcode, true));
                if (refcode != "")
                {
                    obj.refcode = obj.refcode;

                }
            }
            catch (Exception ex)
            {
                refcode = obj.refcode;
                //obj.refcode = obj.refcode;
            }
            List<Model.Customer> _lst = new List<Model.Customer>();
            MySqlCommand _cmd = new MySqlCommand("GetDetailsOnReferrerCode");//("decode_referral_code");
            _cmd.CommandType = CommandType.StoredProcedure;
            _cmd.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
            if (refcode != "")//anushka
            {
                _cmd.Parameters.AddWithValue("_Ref_Code", refcode);
            }
            else
            {
                _cmd.Parameters.AddWithValue("_Ref_Code", obj.refcode);
            }
            DataTable dt = db_connection.ExecuteQueryDataTableProcedure(_cmd);
            Model.Customer _obj = new Model.Customer();
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    _obj = new Model.Customer();

                    if (row["Full_Name"] != DBNull.Value)
                    {
                        _obj.Name = Convert.ToString(row["Full_Name"]);
                    }
                    if (row["ReferenceNo"] != DBNull.Value)
                    {
                        //_obj.refcode = Convert.ToString(row["ReferenceNo"]);
                        _obj.refcode = obj.refcode;
                    }
                    if (row["Referrer_Flag"] != DBNull.Value)
                    {
                        _obj.Referrer_Flag = Convert.ToString(row["Referrer_Flag"]);
                    }
                    if (Convert.ToInt32(row["Referrer_Flag"]) == 3)
                    {
                        if (row["ID"] != DBNull.Value)
                        {
                            _obj.Id = Convert.ToString(row["ID"]);
                        }
                    }
                    else
                    {
                        _obj.Id = "0";
                    }
                    _obj.Id = CompanyInfo.Encrypt(Convert.ToString(_obj.Id), true);
                    _lst.Add(_obj);
                }
            }
            return _lst;
        }

        public string check_Email_Is_Exist(Model.Customer obj)
        {

            using (MySqlConnection con = new MySqlConnection(_MTS.WebConnSetting()))
            {
                con.Open();
                try
                {
                    string chk_mail = validation.validate(obj.Email, 1, 1, 1, 1, 0, 1, 1, 1, 1);
                    if (chk_mail != "false")
                    {
                        using (MySqlCommand cmd = new MySqlCommand("sp_check_email_is_exist", con))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;

                            cmd.Parameters.AddWithValue("_BranchId", obj.Branch_ID);
                            cmd.Parameters.AddWithValue("_ClientId", obj.Client_ID);
                            cmd.Parameters.AddWithValue("_Email", obj.Email);

                            cmd.Parameters.Add(new MySqlParameter("_existsEmail", MySqlDbType.Int32));
                            cmd.Parameters["_existsEmail"].Direction = ParameterDirection.Output;

                            cmd.ExecuteScalar();

                            obj.ExistEmail = Convert.ToInt32(cmd.Parameters["_existsEmail"].Value);
                            if (obj.ExistEmail == 1)
                            {
                                return "exist_email";
                            }
                            else
                            {
                                return "not_exist_email";
                            }
                        }
                    }
                    else
                    {
                        return "validation failed";
                    }

                }
                catch (Exception _x)
                {
                    throw _x;
                }
                finally
                {
                    con.Close();
                }
            }

        }

        public int Email_Counter(Model.Customer obj)
        {
            int Customer_ID = Convert.ToInt32(CompanyInfo.Decrypt(Convert.ToString(obj.Customer_ID), true));
            string Activity = string.Empty;
            MySqlCommand cmdupdate1 = new MySqlCommand("UpdateEmail_Counter");
            cmdupdate1.CommandType = CommandType.StoredProcedure;
            cmdupdate1.Parameters.AddWithValue("_Customer_ID", Customer_ID);
            cmdupdate1.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
            DataTable dt1 = db_connection.ExecuteQueryDataTableProcedure(cmdupdate1);
            obj.per_status = Convert.ToInt32(dt1.Rows[0]["Isvalid_IDScan"]);
            return obj.per_status;
        }

        #region ClickSend
        public class ClickSend
        {
            public string http_code { get; set; }
            public string response_code { get; set; }
            public ClickSendData data { get; set; }
        }
        public class ClickSendData
        {
            public string body { get; set; }
            public string status { get; set; }
        }
        #endregion

        public Model.Customer SendOTP(Model.Customer obj)
        {
            HttpContext context = null;
            int Customer_ID = Convert.ToInt32(CompanyInfo.Decrypt(Convert.ToString(obj.Id), true));
            //var context = System.Web.HttpContext.Current;
            string Username = obj.UserName;// Convert.ToString(context.Request.Form["Username"]);
            string Activity = "<b>" + Username + "</b>";
            try
            {
                Random generator = new Random();
                String r = generator.Next(100000, 999999).ToString("D6");
                var jsonString = String.Empty;
                MySqlCommand cmd = new MySqlCommand("GetAPIDetails");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("_API_ID", 4);//Click Send API ID
                cmd.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                cmd.Parameters.AddWithValue("_status", 0);// API Status
                MySqlCommand cmdn = new MySqlCommand("customer_details_by_param");
                cmdn.CommandType = CommandType.StoredProcedure;
                string where = " and cr.Customer_ID = " + Customer_ID + " and cr.Client_ID=" + obj.Client_ID + "";
                cmdn.Parameters.AddWithValue("_whereclause", where);
                cmdn.Parameters.AddWithValue("_SecurityKey", CompanyInfo.SecurityKey());
                DataTable dttCust = db_connection.ExecuteQueryDataTableProcedure(cmdn);
                DataTable dtt = db_connection.ExecuteQueryDataTableProcedure(cmd);
                DataTable dtc = CompanyInfo.get(obj.Client_ID, context);

                int api_id = 0; string apiurl = "", apiuser = "", apipass = "", accesscode = "";
                string TFN = "";
                if (dtt.Rows.Count > 0)
                {
                    api_id = Convert.ToInt32(dtt.Rows[0]["API_ID"]);
                    apiurl = Convert.ToString(dtt.Rows[0]["API_URL"]);
                    apiuser = Convert.ToString(dtt.Rows[0]["UserName"]);
                    apipass = Convert.ToString(dtt.Rows[0]["Password"]);
                    accesscode = Convert.ToString(dtt.Rows[0]["ProfileID"]);//unique code
                    string API_Fields = Convert.ToString(dtt.Rows[0]["API_Fields"]);
                    if (API_Fields != "" && API_Fields != null)
                    {
                        Newtonsoft.Json.Linq.JObject o = Newtonsoft.Json.Linq.JObject.Parse(API_Fields);
                        TFN = Convert.ToString(o["TFN"]);
                    }
                }
                string send_mobile_no = dtc.Rows[0]["Company_mobile"].ToString().Replace("+", "").Replace(" ", "");
                if (TFN != "")
                    send_mobile_no = TFN;
                var client = new RestClient(apiurl);
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                string encoded = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes("" + apiuser + "" + ":" + "" + apipass + ""));// User Name and Password
                request.AddHeader("Authorization", "Basic " + encoded); //request.AddHeader("Authorization", "Basic c3VwcG9ydEBjYWx5eC1zb2x1dGlvbnMuY29tOkNAbHl4MTIz");

                var body = @"{" + "\n" +
                            @"  ""messages"": [" + "\n" +
                            @"    {" + "\n" +
                            @"      ""body"":""" + dtc.Rows[0]["Company_Name"] + @": Your security code is " + obj.OTP + @". Don't share this code with anyone. Code expires in 3 minutes. ""," + "\n" +
                            @"      ""to"": """ + dttCust.Rows[0]["Country_Code"] + obj.MobileNumber.Replace("+", "") + @"""," + "\n" +
                            @"      ""from"": """ + send_mobile_no + @"""" + "\n" +

                            @"    }" + "\n" +
                            @"  ]" + "\n" +
                            @"} ";
                // Dear Customer, Your OTP for TangoPay is " + obj.OTP + @". Use this Passcode to complete your verification. Thank you.
                request.AddParameter("application/json", body, ParameterType.RequestBody);
                string jsonRequest = Newtonsoft.Json.JsonConvert.SerializeObject(request);
                Activity = "Send SMS Request.  </br> " + jsonRequest + " Message Body" + body;
                int stattus = (int)CompanyInfo.InsertActivityLogDetails(Activity, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(Customer_ID), "UpdatePhone", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "Update Phone", context);
                IRestResponse response = client.Execute(request);
                ClickSend json = Newtonsoft.Json.JsonConvert.DeserializeObject<ClickSend>(response.Content);
                Activity = "Send SMS Response: " + json.response_code + "  </br> " + response.Content.ToString();
                int stattus1 = (int)CompanyInfo.InsertActivityLogDetails(Activity, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(Customer_ID), "UpdatePhone", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "Update Phone", context);
                int stattus12 = (int)CompanyInfo.InsertActivityLogDetails("Security code (" + obj.OTP + ") is sent for Customer :" + dttCust.Rows[0]["WireTransfer_ReferanceNo"] + " ", Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(Customer_ID), "UpdatePhone", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "Update Phone", context);
                if (dttCust.Rows[0]["Mobile_Retry_Flag"] != DBNull.Value)
                    obj.Retry_Cnt = Convert.ToInt32(dttCust.Rows[0]["Mobile_Retry_Flag"]) + 1;
                else
                    obj.Retry_Cnt = 1;
                using (cmd = new MySqlCommand("Retry_otp_count"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("_Id", Customer_ID);
                    cmd.Parameters.AddWithValue("_OTP", obj.OTP);
                    cmd.Parameters.AddWithValue("_Branch_ID", obj.Branch_ID);
                    cmd.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                    int n = db_connection.ExecuteNonQueryProcedure(cmd);
                }
                obj.Message = json.response_code;
            }
            catch (Exception ex)
            {
                Activity = "Send SMS ERROR.  </br> " + ex.ToString();

                int stattus = (int)CompanyInfo.InsertActivityLogDetails(Activity, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(obj.Customer_ID), "SendOTP", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "Update Phone", context);
            }
            return obj;
        }

        public Model.Customer VerifyPhone(Model.Customer obj)
        {
            HttpContext context = null;
            int Customer_ID = Convert.ToInt32(CompanyInfo.Decrypt(Convert.ToString(obj.Id), true));
            string Activity = string.Empty; string notification_icon = ""; string notification_message = "";
            //var context = System.Web.HttpContext.Current;

            string error_msg = ""; string error_invalid_data = "";
            string otp_regex = validation.validate(obj.OTP, 0, 1, 1, 1, 1, 1, 1, 1, 1);

            if (otp_regex == "false")
            {
                error_msg = error_msg + " OTP Should be numeric without space.";
                error_invalid_data = error_invalid_data + " OTP: " + obj.OTP;
            }
            if (otp_regex != "false")
            {
                string otp = "";
                otp = CompanyInfo.testInjection(Convert.ToString(obj.OTP));
                if (otp == "1")
                {
                    obj.RecordDate = Convert.ToDateTime(CompanyInfo.gettime(obj.Client_ID, "0", 0, context));
                    MySqlCommand cmdn = new MySqlCommand("customer_details_by_param");
                    cmdn.CommandType = CommandType.StoredProcedure;
                    string where = " and cr.Customer_ID = " + Customer_ID + " and cr.Client_ID=" + obj.Client_ID + "";
                    cmdn.Parameters.AddWithValue("_whereclause", where);
                    cmdn.Parameters.AddWithValue("_SecurityKey", CompanyInfo.SecurityKey());
                    DataTable dttCust = db_connection.ExecuteQueryDataTableProcedure(cmdn);
                    if (dttCust.Rows[0]["Mobile_OTP"] != DBNull.Value && dttCust.Rows[0]["Mobile_OTP"].ToString() != "")
                    {
                        if (dttCust.Rows[0]["Mobile_OTP"].ToString() != obj.OTP)
                        {
                            obj.Message = "OTPdoesnotmatch";
                            return obj;
                        }
                    }
                    mts_connection _MTS = new mts_connection();
                    using (MySqlConnection con = new MySqlConnection(_MTS.WebConnSetting()))
                    {
                        con.Open();
                        MySqlTransaction transaction;
                        transaction = con.BeginTransaction(IsolationLevel.ReadCommitted);

                        try
                        {
                            using (MySqlCommand cmd = new MySqlCommand("verify_phone_number", con))
                            {
                                cmd.CommandType = CommandType.StoredProcedure;
                                //obj.HouseNumber = "cc";
                                //obj.Street = "23";
                                //cmd.Parameters.AddWithValue("_Phone_Number", obj.PhoneNumber);
                                //cmd.Parameters.AddWithValue("_Mobile_Number", obj.MobileNumber);
                                cmd.Parameters.AddWithValue("_Id", Customer_ID);
                                cmd.Parameters.AddWithValue("_OTP", obj.OTP);
                                cmd.Parameters.AddWithValue("_Branch_ID", obj.Branch_ID);
                                cmd.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                                cmd.Transaction = transaction;
                                int n = cmd.ExecuteNonQuery();
                            }

                            transaction.Commit();
                            int stattus12 = (int)CompanyInfo.InsertActivityLogDetails("Security code (" + obj.OTP + ") is saved for Customer :" + dttCust.Rows[0]["WireTransfer_ReferanceNo"], Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(Customer_ID), "UpdatePhone", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "Update Phone", context);
                            obj.Message = "success";
                            notification_icon = "verify-no.jpg";
                            notification_message = "<span class='cls-admin'><strong class='cls-verify'>Mobile Number</strong> is verified.</span><span class='cls-customer'><strong>Mobile number verified</strong><span>Your mobile number has been verified</span></span>";
                            CompanyInfo.save_notification(notification_message, notification_icon, Convert.ToInt32(Customer_ID), Convert.ToDateTime(obj.RecordDate), Convert.ToInt32(obj.Client_ID), 1, 0, Convert.ToInt32(obj.Branch_ID), 0, 0, 1, 0, context);
                        }
                        catch (Exception _x)
                        {
                            transaction.Rollback();
                            throw _x;
                        }
                        finally
                        {
                            con.Close();
                        }
                        // obj.Message = "success";

                        //}
                    }
                }
                else
                {
                    string msg = "SQl Enjection detected";
                    int stattus = (int)CompanyInfo.InsertActivityLogDetailsSecurity(msg, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), 1, "srvcustomer", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "Update_Phone", 0, context);

                }
            }
            else
            {
                obj.Id = "0";
                string msg = "Validation Failed <br/> " + error_invalid_data;
                obj.Message = "Validation Failed";
                obj.Comment = error_msg;
                int stattus = (int)CompanyInfo.InsertActivityLogDetails(msg, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), 1, "Verify_Mobile", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "Verify_Mobile", context);
            }
            return obj;
        }
        public int Mobile_Counter(Model.Customer obj)
        {
            HttpContext context = null;
            try
            {
                int Customer_ID = Convert.ToInt32(CompanyInfo.Decrypt(Convert.ToString(obj.Id), true));
                string Activity = string.Empty;
                MySqlCommand cmdupdate1 = new MySqlCommand("UpdateMobile_Counter");
                cmdupdate1.CommandType = CommandType.StoredProcedure;
                cmdupdate1.Parameters.AddWithValue("_Customer_ID", Customer_ID);
                cmdupdate1.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                DataTable dt1 = db_connection.ExecuteQueryDataTableProcedure(cmdupdate1);
                obj.per_status = Convert.ToInt32(dt1.Rows[0]["Isvalid_IDScan"]);
                return obj.per_status;
            }
            catch (Exception ex)
            {

                Model.response.WebResponse response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_TECHNICAL_ERROR);
                response.sErrorExceptionText = ex.ToString();
                response.ResponseMessage = "technical error";

                //Error Log Handled
                Model.ErrorLog objError = new Model.ErrorLog();
                objError.User = new Model.User();

                objError.Error = "Api : Mobile_Counter --" + ex.ToString();
                objError.Date = DateTime.Now;
                objError.User_ID = 1;
                objError.Id = 1;
                objError.Function_Name = "Mobile_Counter";
                Service.srvErrorLog srvError = new Service.srvErrorLog();
                srvError.Create(objError, context);
                return obj.per_status;
            }

        }


        public DataTable CustomerBaseData(Model.Customer obj)
        {
            int Customer_ID = 0;
            if (obj.Customer_ID != "" && obj.Customer_ID != null && obj.Customer_ID != "0")
                Customer_ID = Convert.ToInt32(CompanyInfo.Decrypt(Convert.ToString(obj.Customer_ID), true));
            List<Model.Customer> _lst = new List<Model.Customer>();
            MySqlCommand _cmd = new MySqlCommand("sp_CustomerBaseData");
            _cmd.CommandType = CommandType.StoredProcedure;
            _cmd.Parameters.AddWithValue("_ClientId", obj.Client_ID);
            _cmd.Parameters.AddWithValue("_BranchId", obj.Branch_ID);
            _cmd.Parameters.AddWithValue("_Customer_ID", Customer_ID);

            DataTable dt = db_connection.ExecuteQueryDataTableProcedure(_cmd);
            return dt;
        }

        public static string GenerateWalletReference(int client)
        {
            string refNo1 = "";
            try
            {

                int size = 8;
                var rng = new Random(Environment.TickCount);
                MySqlCommand cmd = new MySqlCommand("getWalRef_InitialChar");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("_Client_ID", client);
                string initialchars = Convert.ToString(db_connection.ExecuteScalarProcedure(cmd));
                cmd.Dispose();

                var refNo = initialchars + string.Concat(Enumerable.Range(0, size).Select((index) => rng.Next(10).ToString()));
                refNo1 = Convert.ToString(refNo);
                cmd = new MySqlCommand("CheckDuplicateWalletReference");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("_Client_ID", client);
                cmd.Parameters.AddWithValue("_Discount_Code", refNo1);
                DataTable ds = db_connection.ExecuteQueryDataTableProcedure(cmd);

                if (ds.Rows.Count > 0)
                {
                    refNo1 = null;
                    GenerateWalletReference(client);
                }
                else { }

            }
            catch { }
            return refNo1;
        }


        public string GenerateDiscountCode(int client)
        {
            string refNo1 = "";
            try
            {

                int size = 6;
                var rng = new Random(Environment.TickCount);
                MySqlCommand cmd = new MySqlCommand("getDiscountRef_InitialChar");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("_Client_ID", client);
                string initialchars = Convert.ToString(db_connection.ExecuteScalarProcedure(cmd));
                cmd.Dispose();
                var refNo = initialchars + string.Concat(Enumerable.Range(0, size).Select((index) => rng.Next(10).ToString()));
                refNo1 = Convert.ToString(refNo);
                // t.referee = referee_id;


                cmd = new MySqlCommand("CheckDuplicateDiscountCode");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("_Client_ID", client);
                cmd.Parameters.AddWithValue("_Discount_Code", refNo1);
                DataTable ds = db_connection.ExecuteQueryDataTableProcedure(cmd);
                if (ds.Rows.Count > 0)
                {
                    refNo1 = null;
                    GenerateDiscountCode(client);
                }
                else { }

            }
            catch { }
            return refNo1;
        }

        public void ReferralScheme(Model.Customer obj, HttpContext context)
        {
            using (MySqlConnection con = new MySqlConnection(_MTS.WebConnSetting()))
            {
                con.Open();
                try
                {
                    #region referralscheme
                    //check permission

                    MySqlCommand cmd6 = new MySqlCommand("GetPermissions");
                    cmd6.CommandType = CommandType.StoredProcedure;

                    cmd6.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                    cmd6.Parameters.AddWithValue("_whereclause", " and PID in (4,152) ");
                    DataTable dtperm_status = db_connection.ExecuteQueryDataTableProcedure(cmd6);
                    //cmd6.Dispose();
                    int per4 = 1;
                    if (dtperm_status.Rows.Count > 0)
                    {
                        per4 = Convert.ToInt32(dtperm_status.Rows[0]["Status_ForCustomer"]);
                    }

                    int referrer = 0;
                    string refname = string.Empty;
                    string referrer_refno = string.Empty;

                    string refcode = obj.refcode;
                    if (refcode != "null" && refcode != "" && refcode != null)
                    {
                        //get referrer details
                        cmd6 = new MySqlCommand("GetDetailsOnReferrerCode");
                        cmd6.CommandType = CommandType.StoredProcedure;
                        cmd6.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                        cmd6.Parameters.AddWithValue("_Ref_Code", refcode);
                        DataTable dtcust = db_connection.ExecuteQueryDataTableProcedure(cmd6);
                        //cmd6.Dispose();
                        int Referrer_Flag = 1;//Cust or Agent
                        if (dtcust.Rows.Count > 0)
                        {
                            referrer = Convert.ToInt32(dtcust.Rows[0]["ID"]);
                            //referrer = Convert.ToInt32(dtcust.Rows[0]["Customer_ID"]);
                            refname = Convert.ToString(dtcust.Rows[0]["Full_Name"]);
                            //referrer_refno = Convert.ToString(dtcust.Rows[0]["WireTransfer_ReferanceNo"]);
                            referrer_refno = Convert.ToString(dtcust.Rows[0]["ReferenceNo"]);
                            Referrer_Flag = Convert.ToInt32(dtcust.Rows[0]["Referrer_Flag"]);
                            if (Referrer_Flag == 2)
                            {
                                //Check Agent Referral Perm
                                cmd6 = new MySqlCommand("GetPermissions");
                                cmd6.CommandType = CommandType.StoredProcedure;
                                cmd6.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                                cmd6.Parameters.AddWithValue("_whereclause", " and PID=4 ");
                                DataTable ipdta = db_connection.ExecuteQueryDataTableProcedure(cmd6);

                                if (ipdta.Rows.Count > 0)
                                {
                                    int per4agent = 1;
                                    per4agent = Convert.ToInt32(ipdta.Rows[0]["Status_ForAgent"]);
                                    per4 = per4agent;
                                    // if (per4agent != 0) { return; }
                                }
                            }
                        }
                        DataTable dt = new DataTable();
                        MySqlCommand cmd1 = new MySqlCommand("Get_CompanyInfo");
                        cmd1.CommandType = CommandType.StoredProcedure;
                        cmd1.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                        cmd1.Parameters.AddWithValue("_SecurityKey", obj.SecurityKey);
                        cmd1.Parameters.AddWithValue("_Customer_ID", 0);
                        dt = (DataTable)db_connection.ExecuteQueryDataTableProcedure(cmd1);
                        // get active scheme
                        cmd6 = new MySqlCommand("get_active_scheme");
                        cmd6.CommandType = CommandType.StoredProcedure;
                        cmd6.Parameters.AddWithValue("_Date", obj.RecordDate);
                        cmd6.Parameters.AddWithValue("_whereclause", " and SalesRep_Flag = " + Referrer_Flag);//Agent or Customer 
                        cmd6.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                        cmd6.Parameters.AddWithValue("_Base_Currency_Code", obj.Currency_Code);

                        DataTable dtscheme = db_connection.ExecuteQueryDataTableProcedure(cmd6);
                        //cmd6.Dispose();
                        int maxno = 0;
                        int referernotx = 0;
                        int refereenotx = 0;
                        int scheme_id = 0;
                        int schemetype = 0;
                        int ref_signup_flag = 0;
                        int referee_perk = 0;
                        int referee_discount_type = 0;
                        double referee_value = 0;
                        double mintranamt = 0;
                        int multiplesof = 0;
                        int referer_perk = 0;
                        int referer_discount_type = 0;
                        double referer_value = 0;

                        if (dtscheme.Rows.Count > 0)
                        {   //active scheme details
                            referernotx = Convert.ToInt32(dtscheme.Rows[0]["referrer_nooftx"]);
                            refereenotx = Convert.ToInt32(dtscheme.Rows[0]["referee_nooftx"]);
                            scheme_id = Convert.ToInt32(dtscheme.Rows[0]["scheme_id"]);
                            schemetype = Convert.ToInt32(dtscheme.Rows[0]["scheme_type_id"]);
                            ref_signup_flag = Convert.ToInt32(dtscheme.Rows[0]["referee_signup_flag"]);
                            referee_perk = Convert.ToInt32(dtscheme.Rows[0]["referee_perk_type"]);
                            referee_discount_type = Convert.ToInt32(dtscheme.Rows[0]["referee_discount_type"]);
                            referee_value = Convert.ToDouble(dtscheme.Rows[0]["referee_value"]);
                            mintranamt = Convert.ToDouble(dtscheme.Rows[0]["referee_mintransferamount"]);
                            multiplesof = Convert.ToInt32(dtscheme.Rows[0]["multiples_of"]);
                            referer_perk = Convert.ToInt32(dtscheme.Rows[0]["referrer_perk_type"]);
                            referer_discount_type = Convert.ToInt32(dtscheme.Rows[0]["referrer_discount_type"]);
                            referer_value = Convert.ToDouble(dtscheme.Rows[0]["referrer_value"]);
                            //old code for maxno which not used before
                            //if (schemetype == 1)
                            //{
                            //    maxno = 1;
                            //}
                            //else if (schemetype == 2)
                            //{
                            //    maxno = Convert.ToInt32(dtscheme.Rows[0]["referrer_maxnoofreferee"]);
                            //}
                            //end
                            maxno = Convert.ToInt32(dtscheme.Rows[0]["referrer_maxnoofreferee"]);
                        }
                        //get invite count of referrer
                        cmd6 = new MySqlCommand("getMaxInviteCount");
                        cmd6.CommandType = CommandType.StoredProcedure;
                        cmd6.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                        cmd6.Parameters.AddWithValue("_Referrer", referrer);
                        cmd6.Parameters.AddWithValue("_SchemeId", scheme_id); //Anushka

                        string lastinvitecount = Convert.ToString(db_connection.ExecuteScalarProcedure(cmd6));
                        //cmd6.Dispose();

                        if (lastinvitecount == null || lastinvitecount == "")
                        {
                            lastinvitecount = "0";
                        }
                        int referee_ID = Convert.ToInt32(obj.Id);
                        string referrer_discountcode = GenerateDiscountCode(obj.Client_ID);
                        //using (cmd6 = new MySqlCommand("Sp_insert_referrer_invite", con))
                        {
                            cmd6 = new MySqlCommand("Sp_insert_referrer_invite", con);
                            cmd6.CommandType = CommandType.StoredProcedure;
                            //cmd6 = new MySqlCommand("Sp_insert_referrer_invite");
                            cmd6.Parameters.AddWithValue("_Branch_ID", obj.Branch_ID);
                            cmd6.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                            cmd6.Parameters.AddWithValue("_Referee", obj.Id);
                            cmd6.Parameters.AddWithValue("_Referrer", referrer);
                            cmd6.Parameters.AddWithValue("_referrer_qualify_status", 0);
                            cmd6.Parameters.AddWithValue("_Delete_Status", 0);
                            cmd6.Parameters.AddWithValue("_Date", obj.RecordDate);
                            cmd6.Parameters.AddWithValue("_invitecount", Convert.ToInt32(lastinvitecount) + 1);
                            cmd6.Parameters.AddWithValue("_Ref_Code", refcode);

                            if (referer_perk == 2)
                            {
                                cmd6.Parameters.AddWithValue("_Discount_Code", referrer_discountcode);
                                cmd6.Parameters.AddWithValue("_AllCustomer_Flag", 1);

                            }
                            else if (referer_perk == 1)
                            {
                                cmd6.Parameters.AddWithValue("_Discount_Code", "");
                                cmd6.Parameters.AddWithValue("_AllCustomer_Flag", 2);
                            }
                            else if (referer_perk == 3)
                            {
                                cmd6.Parameters.AddWithValue("_Discount_Code", "");
                                cmd6.Parameters.AddWithValue("_AllCustomer_Flag", 0);
                            }
                            else
                            {
                                cmd6.Parameters.AddWithValue("_Discount_Code", "");
                                cmd6.Parameters.AddWithValue("_AllCustomer_Flag", 0);
                            }
                            cmd6.Parameters.AddWithValue("_Months", 12);
                            cmd6.Parameters.AddWithValue("_Scheme_id", scheme_id);
                            cmd6.Parameters.AddWithValue("_AgentFlag", Referrer_Flag);
                            cmd6.ExecuteNonQuery();
                            //stattus = (int)CompanyInfo.InsertActivityLogDetails("check6", Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(obj.Id), "Referralscheme", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "Referralscheme");
                            // int a = db_connection.ExecuteNonQueryProcedure(cmd6);
                            //cmd6.Dispose();
                        }
                        string referee_discountcode = GenerateDiscountCode(obj.Client_ID);
                        //using (cmd6 = new MySqlCommand("Sp_insert_referee_invite", con))
                        {
                            cmd6 = new MySqlCommand("Sp_insert_referee_invite", con);
                            cmd6.CommandType = CommandType.StoredProcedure;
                            //cmd6 = new MySqlCommand("Sp_insert_referrer_invite");
                            cmd6.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                            cmd6.Parameters.AddWithValue("_Branch_ID", obj.Branch_ID);
                            cmd6.Parameters.AddWithValue("_Referee", obj.Id);
                            cmd6.Parameters.AddWithValue("_Referrer", referrer);

                            cmd6.Parameters.AddWithValue("_signupstatus", 1);
                            cmd6.Parameters.AddWithValue("_transfer_status", 0);
                            cmd6.Parameters.AddWithValue("_transfer_amount", 0);
                            if (referee_perk == 2)
                            {
                                cmd6.Parameters.AddWithValue("_Discount_Code", referee_discountcode);
                                cmd6.Parameters.AddWithValue("_AllCustomer_Flag", "1");

                            }
                            else if (referee_perk == 1)
                            {
                                cmd6.Parameters.AddWithValue("_Discount_Code", "");
                                cmd6.Parameters.AddWithValue("_AllCustomer_Flag", "2");
                            }
                            else if (referee_perk == 3)
                            {
                                cmd6.Parameters.AddWithValue("_Discount_Code", "");
                                cmd6.Parameters.AddWithValue("_AllCustomer_Flag", 0);
                            }
                            else
                            {
                                cmd6.Parameters.AddWithValue("_Discount_Code", "");
                                cmd6.Parameters.AddWithValue("_AllCustomer_Flag", 0);
                            }
                            cmd6.Parameters.AddWithValue("_Delete_Status", 0);
                            cmd6.Parameters.AddWithValue("_Date", obj.RecordDate);

                            cmd6.Parameters.AddWithValue("_Ref_Code", refcode);
                            cmd6.Parameters.AddWithValue("_Months", 12);
                            cmd6.Parameters.AddWithValue("_Scheme_id", scheme_id);
                            cmd6.Parameters.AddWithValue("_AgentFlag", Referrer_Flag);
                            cmd6.ExecuteNonQuery();
                            //stattus = (int)CompanyInfo.InsertActivityLogDetails("check7", Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(obj.Id), "Referralscheme", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "Referralscheme");
                            //int a = db_connection.ExecuteNonQueryProcedure(cmd6);
                            //cmd6.Dispose();
                        }

                        if (per4 == 0 && dtscheme.Rows.Count > 0)
                        {

                            string msgwal1 = string.Empty;
                            string stattus1 = string.Empty;
                            string walletid = string.Empty;
                            cmd6 = new MySqlCommand("GetDefaultDiscount_Values");
                            cmd6.CommandType = CommandType.StoredProcedure;
                            cmd6.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                            DataTable dtdisc = db_connection.ExecuteQueryDataTableProcedure(cmd6);
                            //stattus = (int)CompanyInfo.InsertActivityLogDetails("check8", Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(obj.Id), "Referralscheme", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "Referralscheme");
                            //cmd6.Dispose();
                            double Minimum_Transfer_Amount = 0, Maximum_Transfer_Amount = 0;
                            if (dtdisc.Rows.Count > 0)
                            {
                                Minimum_Transfer_Amount = Convert.ToDouble(dtdisc.Rows[0]["Min_Amt"]);
                                Maximum_Transfer_Amount = Convert.ToDouble(dtdisc.Rows[0]["Max_Amt"]);
                            }
                            #region referee
                            int check_valid_sales = 0;
                            try
                            {


                                if (Referrer_Flag == 2)
                                {

                                    if (dtperm_status.Rows.Count > 0)
                                    {
                                        int per152 = Convert.ToInt32(dtperm_status.Rows[1]["Status_ForCustomer"]);
                                        if (per152 == 0)
                                        {
                                            string Query = "select * from user_master where User_ID = " + Convert.ToString(referrer);

                                            MySqlCommand _cmd = new MySqlCommand("Default_SP");
                                            _cmd.CommandType = CommandType.StoredProcedure;
                                            _cmd.Parameters.AddWithValue("_Query", Query);
                                            DataTable userdt = db_connection.ExecuteQueryDataTableProcedure(_cmd);
                                            if (userdt.Rows.Count > 0)
                                            {
                                                if (Convert.ToInt32(userdt.Rows[0]["referee_perk_flag"]) == 1)
                                                {
                                                    check_valid_sales = 1;
                                                }
                                            }

                                        }
                                    }
                                }
                            }
                            catch { }
                            if (ref_signup_flag == 1 && check_valid_sales == 0)//&& (referee_perk == 1 || referee_discount_type == 1))
                            {
                                if (referee_perk == 2)
                                {
                                    //using (cmd6 = new MySqlCommand("sp_Save_DiscountDetails", con))
                                    {
                                        cmd6 = new MySqlCommand("sp_Save_DiscountDetails", con);
                                        cmd6.CommandType = CommandType.StoredProcedure;
                                        cmd6.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                                        cmd6.Parameters.AddWithValue("_Branch_ID", obj.Branch_ID);
                                        cmd6.Parameters.AddWithValue("_Discount_Code", referee_discountcode);
                                        cmd6.Parameters.AddWithValue("_discount_type_id", 1);
                                        if (referee_discount_type == 1)
                                        {
                                            cmd6.Parameters.AddWithValue("_amount_type_id", 2);
                                        }
                                        else
                                        {
                                            cmd6.Parameters.AddWithValue("_amount_type_id", 1);
                                        }

                                        cmd6.Parameters.AddWithValue("_discount_value", Convert.ToString(referee_value));
                                        cmd6.Parameters.AddWithValue("_customer_eligibility_id", 3);
                                        if (refereenotx == 0)
                                        {
                                            cmd6.Parameters.AddWithValue("_usagelimit", 1);
                                        }
                                        else
                                        {
                                            cmd6.Parameters.AddWithValue("_usagelimit", refereenotx);
                                        }

                                        cmd6.Parameters.AddWithValue("_usagelimit_flag", 5);
                                        DateTime dto = DateTime.Now;
                                        DateTime dto1 = dto.AddMonths(12);

                                        cmd6.Parameters.AddWithValue("_start_date", obj.RecordDate);


                                        if (dt.Rows.Count > 0)
                                        {
                                            string timezone = Convert.ToString(dt.Rows[0]["BaseCurrency_Timezone"]);
                                            if (timezone != "" && timezone != null)
                                            {
                                                var britishZone = TimeZoneInfo.FindSystemTimeZoneById("" + timezone + "");
                                                dto1 = TimeZoneInfo.ConvertTime(dto1, TimeZoneInfo.Local, britishZone);
                                            }
                                        }
                                        cmd6.Parameters.AddWithValue("_end_date", dto1.ToString("yyyy-MM-dd HH:mm:ss"));
                                        cmd6.Parameters.AddWithValue("_Delete_Status", 0);
                                        cmd6.Parameters.AddWithValue("_Record_Insert_DateTime", obj.RecordDate);
                                        cmd6.Parameters.AddWithValue("_minAmount", Minimum_Transfer_Amount);
                                        cmd6.Parameters.AddWithValue("_maxAmount", Maximum_Transfer_Amount);
                                        cmd6.ExecuteNonQuery();
                                        //int a = db_connection.ExecuteNonQueryProcedure(cmd6);
                                        //cmd6.Dispose();
                                    }

                                    //get discount id

                                    cmd6 = new MySqlCommand("CheckDuplicateDiscountCode");
                                    cmd6.CommandType = CommandType.StoredProcedure;
                                    cmd6.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                                    cmd6.Parameters.AddWithValue("_Discount_Code", referee_discountcode);
                                    DataTable ds = db_connection.ExecuteQueryDataTableProcedure(cmd6);

                                    string disc_id = Convert.ToString(ds.Rows[0]["ID"]);

                                    //cmd6.Dispose();
                                    //insert in customereligibility_table
                                    //using (cmd6 = new MySqlCommand("Save_CustomerEligibilityDetails", con))
                                    {
                                        cmd6 = new MySqlCommand("Save_CustomerEligibilityDetails", con);
                                        cmd6.CommandType = CommandType.StoredProcedure;
                                        cmd6.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                                        cmd6.Parameters.AddWithValue("_Id", Convert.ToInt32(disc_id));
                                        cmd6.Parameters.AddWithValue("_customer_eligibility_id", 3);
                                        cmd6.Parameters.AddWithValue("_Customer_ID", referee_ID);
                                        cmd6.Parameters.AddWithValue("Delete_Status", 0);
                                        cmd6.Parameters.AddWithValue("_Branch_ID", obj.Branch_ID);

                                        cmd6.Parameters.AddWithValue("_DeliveryType_Id", 5);
                                        int a_discount = cmd6.ExecuteNonQuery();
                                        string notification_icon = "discount.jpg";
                                        string notification_message = "<span class='cls-admin'> new <strong class='cls-reward'>Discount coupon " + referrer_discountcode + "</strong> is added.</span><span class='cls-customer'><strong>Discount coupon " + referrer_discountcode + "</strong><span>A new discount coupon is added to your account.</span></span>";
                                        if (a_discount == 1)
                                        {
                                            CompanyInfo.save_notification(notification_message, notification_icon, Convert.ToInt32(referee_ID), Convert.ToDateTime(obj.RecordDate), Convert.ToInt32(obj.Client_ID), 1, 0, Convert.ToInt32(obj.Branch_ID), 0, 0, 1, 0, context);
                                            try
                                            {
                                                DataTable dt_notif = CompanyInfo.set_notification_data(60);
                                                if (dt_notif.Rows.Count > 0)
                                                {
                                                    int SMS = Convert.ToInt32(dt_notif.Rows[0]["SMS"]);
                                                    int Email1 = Convert.ToInt32(dt_notif.Rows[0]["Email"]);
                                                    int Notif_status = Convert.ToInt32(dt_notif.Rows[0]["Notification"]);
                                                    string notification_msg = Convert.ToString(dt_notif.Rows[0]["notification_msg"]);
                                                    if (notification_msg.Contains("[discountcode] ") == true)
                                                    {
                                                        notification_msg = notification_msg.Replace("[discountcode] ", Convert.ToString(referrer_discountcode));
                                                    }

                                                    int i1 = CompanyInfo.check_notification_perm(Convert.ToString(referee_ID), obj.Client_ID, obj.Branch_ID, 6, 60, Convert.ToDateTime(obj.RecordDate), 0, SMS, Email1, Notif_status, "Web- Referral Discount Notification - 60", notification_msg, context);
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                            }
                                        }
                                        //int a = db_connection.ExecuteNonQueryProcedure(cmd6);
                                        //cmd6.Dispose();
                                    }

                                }
                                else if (referee_perk == 1)
                                {
                                    cmd6 = new MySqlCommand("getwalletRefforbasecurrency");
                                    cmd6.CommandType = CommandType.StoredProcedure;
                                    string basecurrency = string.Empty;
                                    if (dt.Rows.Count > 0)
                                    {
                                        basecurrency = Convert.ToString(dt.Rows[0]["BaseCurrency_Code"]);
                                    }
                                    cmd6.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                                    cmd6.Parameters.AddWithValue("_WireTransfer_ReferanceNo", obj.WireTransferRefNumber);
                                    cmd6.Parameters.AddWithValue("_BaseCurrency", basecurrency);
                                    cmd6.Parameters.AddWithValue("_AgentFlag", 1);//Get Customer Wallet
                                    DataTable dtwal = db_connection.ExecuteQueryDataTableProcedure(cmd6);

                                    //cmd6.Dispose();
                                    cmd6 = new MySqlCommand("getbasecurrencyid");
                                    cmd6.CommandType = CommandType.StoredProcedure;
                                    cmd6.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                                    cmd6.Parameters.AddWithValue("_BaseCurrency", basecurrency);
                                    string cid = Convert.ToString(db_connection.ExecuteScalarProcedure(cmd6));
                                    //cmd6.Dispose();
                                    string wrefNo = string.Empty;
                                    if (dtwal.Rows.Count == 0)//create wallet ref for referee
                                    {
                                        wrefNo = GenerateWalletReference(obj.Client_ID);

                                        //using (cmd6 = new MySqlCommand("insertinwallet_table", con))
                                        {
                                            cmd6 = new MySqlCommand("insertinwallet_table", con);
                                            cmd6.CommandType = CommandType.StoredProcedure;
                                            cmd6.Parameters.AddWithValue("_WireTransfer_ReferanceNo", obj.WireTransferRefNumber);
                                            cmd6.Parameters.AddWithValue("_Currency_ID", Convert.ToInt32(cid));
                                            //iw1.Client_ID = client_id;
                                            cmd6.Parameters.AddWithValue("_Delete_Status", 0);
                                            cmd6.Parameters.AddWithValue("_Record_Insert_DateTime", obj.RecordDate);
                                            cmd6.Parameters.AddWithValue("_wallet_reference", wrefNo);
                                            cmd6.Parameters.AddWithValue("_Wallet_balance", 0);
                                            cmd6.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                                            cmd6.Parameters.AddWithValue("_Branch_ID", obj.Branch_ID);
                                            cmd6.Parameters.AddWithValue("_Customer_ID", referee_ID);
                                            cmd6.Parameters.AddWithValue("_AgentFlag", 1);//Get Customer Wallet
                                            cmd6.ExecuteNonQuery();
                                            //int a = db_connection.ExecuteNonQueryProcedure(cmd6);
                                            //cmd6.Dispose();
                                        }
                                    }
                                    cmd6 = new MySqlCommand("getwalletid");
                                    cmd6.CommandType = CommandType.StoredProcedure;
                                    cmd6.Parameters.AddWithValue("_WireTransfer_ReferanceNo", obj.WireTransferRefNumber);
                                    cmd6.Parameters.AddWithValue("_Currency_ID", Convert.ToInt32(cid));
                                    cmd6.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                                    walletid = Convert.ToString(db_connection.ExecuteScalarProcedure(cmd6));
                                    //cmd6.Dispose();

                                    cmd6 = new MySqlCommand("checkinwalletbalance");
                                    cmd6.CommandType = CommandType.StoredProcedure;
                                    cmd6.Parameters.AddWithValue("_wallet_id", Convert.ToInt32(walletid));

                                    DataTable dtwbal = db_connection.ExecuteQueryDataTableProcedure(cmd6);
                                    //cmd6.Dispose();

                                    double oldbal = 0, newbal = 0;
                                    if (dtwbal.Rows.Count > 0)
                                    {
                                        oldbal = Convert.ToDouble(dtwbal.Rows[0]["newBalance"]);


                                    }
                                    newbal = oldbal + referee_value;
                                    int paytype = 10, userid = 1;
                                    double exchangerate = 1, trfee = 0;
                                    cmd6 = new MySqlCommand("getPType_ID");
                                    cmd6.CommandType = CommandType.StoredProcedure;

                                    paytype = Convert.ToInt32(db_connection.ExecuteScalarProcedure(cmd6));

                                    //cmd6.Dispose();
                                    //using (cmd6 = new MySqlCommand("insert_wallet_transaction", con))
                                    {
                                        cmd6 = new MySqlCommand("insert_wallet_transaction", con);
                                        cmd6.CommandType = CommandType.StoredProcedure;

                                        cmd6.Parameters.AddWithValue("_wallet_id", Convert.ToInt32(walletid));
                                        cmd6.Parameters.AddWithValue("_transfer_type", 1);
                                        cmd6.Parameters.AddWithValue("_Currency_ID", Convert.ToInt32(cid));
                                        cmd6.Parameters.AddWithValue("_transfer_amount", referee_value);//referee_value;
                                        cmd6.Parameters.AddWithValue("_Wallet_Description", "");
                                        cmd6.Parameters.AddWithValue("_oldwalletbalance", oldbal);
                                        cmd6.Parameters.AddWithValue("_newwalletbalance", newbal);
                                        cmd6.Parameters.AddWithValue("_Record_Insert_DateTime", obj.RecordDate);
                                        cmd6.Parameters.AddWithValue("_Delete_Status", 0);
                                        cmd6.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                                        cmd6.Parameters.AddWithValue("_Branch_ID", obj.Branch_ID);
                                        cmd6.Parameters.AddWithValue("_paytype", paytype);
                                        cmd6.Parameters.AddWithValue("_exchangerate", exchangerate);
                                        cmd6.Parameters.AddWithValue("_Transaction_ID", 0);
                                        cmd6.Parameters.AddWithValue("_fee", trfee);
                                        cmd6.Parameters.AddWithValue("_User_ID", userid);
                                        cmd6.Parameters.AddWithValue("_AgentFlag", 1);
                                        cmd6.Parameters.AddWithValue("_referee_id", referee_ID);
                                        cmd6.Parameters.AddWithValue("_Referral_Flag", 0);
                                        cmd6.ExecuteNonQuery();
                                        //int a = db_connection.ExecuteNonQueryProcedure(cmd6);
                                        //cmd6.Dispose();
                                    }
                                    //using (cmd6 = new MySqlCommand("updateinwallettable", con))
                                    int a_wallet = 0;
                                    {
                                        cmd6 = new MySqlCommand("updateinwallettable", con);
                                        cmd6.CommandType = CommandType.StoredProcedure;

                                        cmd6.Parameters.AddWithValue("_transfer_amount", newbal);
                                        cmd6.Parameters.AddWithValue("_wallet_id", Convert.ToInt32(walletid));
                                        cmd6.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                                        cmd6.Parameters.AddWithValue("_Branch_ID", obj.Branch_ID);
                                        a_wallet = cmd6.ExecuteNonQuery();
                                        //int a = db_connection.ExecuteNonQueryProcedure(cmd6);
                                        //cmd6.Dispose();
                                    }
                                    //using (cmd6 = new MySqlCommand("updatewalletrefereeInvite", con))
                                    {
                                        cmd6 = new MySqlCommand("updatewalletrefereeInvite", con);
                                        cmd6.CommandType = CommandType.StoredProcedure;

                                        cmd6.Parameters.AddWithValue("_referrer", referrer);
                                        cmd6.Parameters.AddWithValue("_referee", referee_ID);
                                        cmd6.Parameters.AddWithValue("_discount_code", wrefNo);
                                        cmd6.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                                        cmd6.ExecuteNonQuery();
                                        //int a = db_connection.ExecuteNonQueryProcedure(cmd6);
                                        //cmd6.Dispose();
                                    }
                                    string notification_icon = "wallet.jpg";
                                    string notification_message = "<span class='cls-admin'> new <strong class='cls-reward'>" + basecurrency + " " + referee_value + "</strong> is added to the wallet.</span><span class='cls-customer'><strong> " + basecurrency + " " + referee_value + " added to your wallet. </strong><span>" + basecurrency + " " + referee_value + " was successfully added to your wallet.</span></span>";
                                    if (a_wallet == 1)
                                    {
                                        CompanyInfo.save_notification(notification_message, notification_icon, Convert.ToInt32(referee_ID), Convert.ToDateTime(obj.RecordDate), Convert.ToInt32(obj.Client_ID), 1, 0, Convert.ToInt32(obj.Branch_ID), 0, 0, 1, 0, context);
                                    }
                                    try
                                    {
                                        DataTable dt_notif = CompanyInfo.set_notification_data(61);
                                        if (dt_notif.Rows.Count > 0)
                                        {
                                            int SMS = Convert.ToInt32(dt_notif.Rows[0]["SMS"]);
                                            int Email = Convert.ToInt32(dt_notif.Rows[0]["Email"]);
                                            int Notif_status = Convert.ToInt32(dt_notif.Rows[0]["Notification"]);
                                            string notification_msg = Convert.ToString(dt_notif.Rows[0]["notification_msg"]);
                                            if (notification_msg.Contains("[wallet_bal]") == true)
                                            {
                                                notification_msg = notification_msg.Replace("[wallet_bal]", Convert.ToString(referee_value));
                                            }
                                            if (notification_msg.Contains("[base_cur]") == true)
                                            {
                                                notification_msg = notification_msg.Replace("[base_cur]", basecurrency);
                                            }
                                            int i = CompanyInfo.check_notification_perm(Convert.ToString(referrer), obj.Client_ID, obj.Branch_ID, 6, 61, Convert.ToDateTime(obj.RecordDate), 0, SMS, Email, Notif_status, "Web-Referral Wallet Notification - 61", notification_msg, context);
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                    }
                                }
                                //using (cmd6 = new MySqlCommand("updatequalifysignupstatusreferee", con))
                                {

                                    cmd6 = new MySqlCommand("updatequalifysignupstatusreferee", con);
                                    cmd6.CommandType = CommandType.StoredProcedure;

                                    cmd6.Parameters.AddWithValue("_referrer", referrer);
                                    cmd6.Parameters.AddWithValue("_referee", referee_ID);
                                    //iw4.transfer_status = 1;
                                    cmd6.Parameters.AddWithValue("_Record_Insert_DateTime", obj.RecordDate);
                                    cmd6.Parameters.AddWithValue("_qualify_status", 1);
                                    cmd6.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                                    cmd6.ExecuteNonQuery();
                                    //int a = db_connection.ExecuteNonQueryProcedure(cmd6);
                                    //cmd6.Dispose();
                                }

                            }
                            #endregion


                            #region referrer
                            if (schemetype == 1 && ref_signup_flag == 1 && mintranamt == 0 && maxno >= Convert.ToInt32(lastinvitecount))
                            {
                                if (referer_perk == 2)// || referer_discount_type == 1)
                                {
                                    //using (cmd6 = new MySqlCommand("sp_Save_DiscountDetails", con))
                                    {
                                        cmd6 = new MySqlCommand("sp_Save_DiscountDetails", con);
                                        cmd6.CommandType = CommandType.StoredProcedure;
                                        cmd6.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                                        cmd6.Parameters.AddWithValue("_Branch_ID", obj.Branch_ID);
                                        cmd6.Parameters.AddWithValue("_Discount_Code", referrer_discountcode);
                                        cmd6.Parameters.AddWithValue("_discount_type_id", 1);
                                        if (referee_discount_type == 1)
                                        {
                                            cmd6.Parameters.AddWithValue("_amount_type_id", 2);
                                        }
                                        else
                                        {
                                            cmd6.Parameters.AddWithValue("_amount_type_id", 1);
                                        }

                                        cmd6.Parameters.AddWithValue("_discount_value", Convert.ToString(referer_value));
                                        cmd6.Parameters.AddWithValue("_customer_eligibility_id", 3);
                                        if (refereenotx == 0)
                                        {
                                            cmd6.Parameters.AddWithValue("_usagelimit", 1);
                                        }
                                        else
                                        {
                                            cmd6.Parameters.AddWithValue("_usagelimit", referernotx);
                                        }

                                        cmd6.Parameters.AddWithValue("_usagelimit_flag", 5);
                                        DateTime dto = DateTime.Now;
                                        DateTime dto1 = dto.AddMonths(12);

                                        cmd6.Parameters.AddWithValue("_start_date", obj.RecordDate);


                                        if (dt.Rows.Count > 0)
                                        {
                                            string timezone = Convert.ToString(dt.Rows[0]["BaseCurrency_Timezone"]);
                                            if (timezone != "" && timezone != null)
                                            {
                                                var britishZone = TimeZoneInfo.FindSystemTimeZoneById("" + timezone + "");
                                                dto1 = TimeZoneInfo.ConvertTime(dto1, TimeZoneInfo.Local, britishZone);
                                            }
                                        }
                                        cmd6.Parameters.AddWithValue("_end_date", dto1.ToString("yyyy-MM-dd HH:mm:ss"));
                                        cmd6.Parameters.AddWithValue("_Delete_Status", 0);
                                        cmd6.Parameters.AddWithValue("_Record_Insert_DateTime", obj.RecordDate);
                                        cmd6.Parameters.AddWithValue("_minAmount", Minimum_Transfer_Amount);
                                        cmd6.Parameters.AddWithValue("_maxAmount", Maximum_Transfer_Amount);
                                        //cmd6.ExecuteNonQuery();
                                        int a = db_connection.ExecuteNonQueryProcedure(cmd6);
                                        //cmd6.Dispose();
                                    }

                                    //get discount id

                                    cmd6 = new MySqlCommand("CheckDuplicateDiscountCode");
                                    cmd6.CommandType = CommandType.StoredProcedure;
                                    cmd6.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                                    cmd6.Parameters.AddWithValue("_Discount_Code", referrer_discountcode);
                                    DataTable ds = db_connection.ExecuteQueryDataTableProcedure(cmd6);

                                    string disc_id = Convert.ToString(ds.Rows[0]["ID"]);

                                    //cmd6.Dispose();
                                    //insert in customereligibility_table
                                    //using (cmd6 = new MySqlCommand("Save_CustomerEligibilityDetails", con))
                                    {
                                        cmd6 = new MySqlCommand("Save_CustomerEligibilityDetails", con);
                                        cmd6.CommandType = CommandType.StoredProcedure;
                                        cmd6.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                                        cmd6.Parameters.AddWithValue("_Id", Convert.ToInt32(disc_id));
                                        cmd6.Parameters.AddWithValue("_customer_eligibility_id", 3);
                                        cmd6.Parameters.AddWithValue("_Customer_ID", referrer);
                                        cmd6.Parameters.AddWithValue("Delete_Status", 0);
                                        cmd6.Parameters.AddWithValue("_Branch_ID", obj.Branch_ID);

                                        cmd6.Parameters.AddWithValue("_DeliveryType_Id", 5);
                                        //cmd6.ExecuteNonQuery();
                                        int a = db_connection.ExecuteNonQueryProcedure(cmd6);
                                        string notification_icon = "discount.jpg";
                                        string notification_message = "<span class='cls-admin'> new <strong class='cls-reward'>Discount coupon " + referrer_discountcode + "</strong> is added.</span><span class='cls-customer'><strong>Discount coupon " + referrer_discountcode + "</strong><span>A new discount coupon is added to your account.</span></span>";
                                        if (a == 1)
                                        {
                                            CompanyInfo.save_notification(notification_message, notification_icon, Convert.ToInt32(referrer), Convert.ToDateTime(obj.RecordDate), Convert.ToInt32(obj.Client_ID), 1, 0, Convert.ToInt32(obj.Branch_ID), 0, 0, 1, 0, context);
                                            try
                                            {
                                                DataTable dt_notif = CompanyInfo.set_notification_data(60);
                                                if (dt_notif.Rows.Count > 0)
                                                {
                                                    int SMS = Convert.ToInt32(dt_notif.Rows[0]["SMS"]);
                                                    int Email1 = Convert.ToInt32(dt_notif.Rows[0]["Email"]);
                                                    int Notif_status = Convert.ToInt32(dt_notif.Rows[0]["Notification"]);
                                                    string notification_msg = Convert.ToString(dt_notif.Rows[0]["notification_msg"]);
                                                    if (notification_msg.Contains("[discountcode] ") == true)
                                                    {
                                                        notification_msg = notification_msg.Replace("[discountcode] ", Convert.ToString(referrer_discountcode));
                                                    }

                                                    int i1 = CompanyInfo.check_notification_perm(Convert.ToString(referrer), obj.Client_ID, obj.Branch_ID, 6, 60, Convert.ToDateTime(obj.RecordDate), 0, SMS, Email1, Notif_status, "Web- Referral Discount Notification - 60", notification_msg, context);
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                            }
                                        }
                                        //cmd6.Dispose();
                                    }

                                }
                                else if (referer_perk == 1)
                                {
                                    cmd6 = new MySqlCommand("getwalletRefforbasecurrency");
                                    cmd6.CommandType = CommandType.StoredProcedure;
                                    string basecurrency = string.Empty;
                                    if (dt.Rows.Count > 0)
                                    {
                                        basecurrency = Convert.ToString(dt.Rows[0]["BaseCurrency_Code"]);
                                    }
                                    cmd6.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                                    cmd6.Parameters.AddWithValue("_WireTransfer_ReferanceNo", referrer_refno);
                                    cmd6.Parameters.AddWithValue("_BaseCurrency", basecurrency);
                                    cmd6.Parameters.AddWithValue("_AgentFlag", Referrer_Flag);
                                    DataTable dtwal = db_connection.ExecuteQueryDataTableProcedure(cmd6);

                                    //cmd6.Dispose();
                                    cmd6 = new MySqlCommand("getbasecurrencyid");
                                    cmd6.CommandType = CommandType.StoredProcedure;
                                    cmd6.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                                    cmd6.Parameters.AddWithValue("_BaseCurrency", basecurrency);
                                    string cid = Convert.ToString(db_connection.ExecuteScalarProcedure(cmd6));
                                    //cmd6.Dispose();
                                    string wrefNo = string.Empty;
                                    if (dtwal.Rows.Count == 0)//create wallet ref for referrer
                                    {
                                        wrefNo = GenerateWalletReference(obj.Client_ID);

                                        //using (cmd6 = new MySqlCommand("insertinwallet_table", con))
                                        {
                                            cmd6 = new MySqlCommand("insertinwallet_table", con);
                                            cmd6.CommandType = CommandType.StoredProcedure;
                                            cmd6.Parameters.AddWithValue("_WireTransfer_ReferanceNo", referrer_refno);
                                            cmd6.Parameters.AddWithValue("_Currency_ID", Convert.ToInt32(cid));
                                            //iw1.Client_ID = client_id;
                                            cmd6.Parameters.AddWithValue("_Delete_Status", 0);
                                            cmd6.Parameters.AddWithValue("_Record_Insert_DateTime", obj.RecordDate);
                                            cmd6.Parameters.AddWithValue("_wallet_reference", wrefNo);
                                            cmd6.Parameters.AddWithValue("_Wallet_balance", 0);
                                            cmd6.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                                            cmd6.Parameters.AddWithValue("_Branch_ID", obj.Branch_ID);
                                            cmd6.Parameters.AddWithValue("_Customer_ID", referrer);
                                            cmd6.Parameters.AddWithValue("_AgentFlag", Referrer_Flag);
                                            //cmd6.ExecuteNonQuery();
                                            int a = db_connection.ExecuteNonQueryProcedure(cmd6);
                                            //cmd6.Dispose();
                                        }
                                    }
                                    cmd6 = new MySqlCommand("getwalletid");
                                    cmd6.CommandType = CommandType.StoredProcedure;
                                    cmd6.Parameters.AddWithValue("_WireTransfer_ReferanceNo", referrer_refno);
                                    cmd6.Parameters.AddWithValue("_Currency_ID", Convert.ToInt32(cid));
                                    cmd6.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                                    walletid = Convert.ToString(db_connection.ExecuteScalarProcedure(cmd6));
                                    //cmd6.Dispose();

                                    cmd6 = new MySqlCommand("checkinwalletbalance");
                                    cmd6.CommandType = CommandType.StoredProcedure;
                                    cmd6.Parameters.AddWithValue("_wallet_id", Convert.ToInt32(walletid));

                                    DataTable dtwbal = db_connection.ExecuteQueryDataTableProcedure(cmd6);
                                    //cmd6.Dispose();

                                    double oldbal = 0, newbal = 0;
                                    if (dtwbal.Rows.Count > 0)
                                    {
                                        oldbal = Convert.ToDouble(dtwbal.Rows[0]["newBalance"]);


                                    }
                                    newbal = oldbal + referer_value;
                                    int paytype = 10, userid = 1;
                                    double exchangerate = 1, trfee = 0;
                                    cmd6 = new MySqlCommand("getPType_ID");
                                    cmd6.CommandType = CommandType.StoredProcedure;

                                    paytype = Convert.ToInt32(db_connection.ExecuteScalarProcedure(cmd6));

                                    //cmd6.Dispose();
                                    //using (cmd6 = new MySqlCommand("insert_wallet_transaction", con))
                                    {
                                        cmd6 = new MySqlCommand("insert_wallet_transaction", con);
                                        cmd6.CommandType = CommandType.StoredProcedure;

                                        cmd6.Parameters.AddWithValue("_wallet_id", Convert.ToInt32(walletid));
                                        cmd6.Parameters.AddWithValue("_transfer_type", 1);
                                        cmd6.Parameters.AddWithValue("_Currency_ID", Convert.ToInt32(cid));
                                        cmd6.Parameters.AddWithValue("_transfer_amount", referer_value);//referer_value;
                                        cmd6.Parameters.AddWithValue("_Wallet_Description", "");
                                        cmd6.Parameters.AddWithValue("_oldwalletbalance", oldbal);
                                        cmd6.Parameters.AddWithValue("_newwalletbalance", newbal);
                                        cmd6.Parameters.AddWithValue("_Record_Insert_DateTime", obj.RecordDate);
                                        cmd6.Parameters.AddWithValue("_Delete_Status", 0);
                                        cmd6.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                                        cmd6.Parameters.AddWithValue("_Branch_ID", obj.Branch_ID);
                                        cmd6.Parameters.AddWithValue("_paytype", paytype);
                                        cmd6.Parameters.AddWithValue("_exchangerate", exchangerate);
                                        cmd6.Parameters.AddWithValue("_Transaction_ID", 0);
                                        cmd6.Parameters.AddWithValue("_fee", trfee);
                                        cmd6.Parameters.AddWithValue("_User_ID", userid);
                                        cmd6.Parameters.AddWithValue("_AgentFlag", Referrer_Flag);
                                        cmd6.Parameters.AddWithValue("_referee_id", referee_ID);
                                        cmd6.Parameters.AddWithValue("_Referral_Flag", 0);
                                        //cmd6.ExecuteNonQuery();
                                        int a = db_connection.ExecuteNonQueryProcedure(cmd6);
                                        //cmd6.Dispose();
                                    }
                                    //using (cmd6 = new MySqlCommand("updateinwallettable", con))
                                    int a_wallet = 0;
                                    {
                                        cmd6 = new MySqlCommand("updateinwallettable", con);
                                        cmd6.CommandType = CommandType.StoredProcedure;

                                        cmd6.Parameters.AddWithValue("_transfer_amount", newbal);
                                        cmd6.Parameters.AddWithValue("_wallet_id", Convert.ToInt32(walletid));
                                        cmd6.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                                        cmd6.Parameters.AddWithValue("_Branch_ID", obj.Branch_ID);
                                        //cmd6.ExecuteNonQuery();
                                        a_wallet = db_connection.ExecuteNonQueryProcedure(cmd6);
                                        //cmd6.Dispose();
                                    }
                                    //using (cmd6 = new MySqlCommand("updatewalletrefererInvite", con))
                                    {
                                        cmd6 = new MySqlCommand("updatewalletrefererInvite", con);
                                        cmd6.CommandType = CommandType.StoredProcedure;

                                        cmd6.Parameters.AddWithValue("_referrer", referrer);
                                        cmd6.Parameters.AddWithValue("_referee", referee_ID);
                                        cmd6.Parameters.AddWithValue("_discount_code", wrefNo);
                                        cmd6.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                                        //cmd6.ExecuteNonQuery();
                                        int a = db_connection.ExecuteNonQueryProcedure(cmd6);
                                        //cmd6.Dispose();
                                    }
                                    string notification_icon = "wallet.jpg";
                                    string notification_message = "<span class='cls-admin'> new <strong class='cls-reward'>" + basecurrency + " " + referee_value + "</strong> is added to the wallet.</span><span class='cls-customer'><strong> " + basecurrency + " " + referee_value + " added to your wallet. </strong><span>" + basecurrency + " " + referee_value + " was successfully added to your wallet.</span></span>";
                                    if (a_wallet == 1)
                                    {
                                        CompanyInfo.save_notification(notification_message, notification_icon, Convert.ToInt32(referrer), Convert.ToDateTime(obj.RecordDate), Convert.ToInt32(obj.Client_ID), 1, 0, Convert.ToInt32(obj.Branch_ID), 0, 0, 1, 0, context);
                                    }
                                    try
                                    {
                                        DataTable dt_notif = CompanyInfo.set_notification_data(61);
                                        if (dt_notif.Rows.Count > 0)
                                        {
                                            int SMS = Convert.ToInt32(dt_notif.Rows[0]["SMS"]);
                                            int Email = Convert.ToInt32(dt_notif.Rows[0]["Email"]);
                                            int Notif_status = Convert.ToInt32(dt_notif.Rows[0]["Notification"]);
                                            string notification_msg = Convert.ToString(dt_notif.Rows[0]["notification_msg"]);
                                            if (notification_msg.Contains("[wallet_bal]") == true)
                                            {
                                                notification_msg = notification_msg.Replace("[wallet_bal]", Convert.ToString(referee_value));
                                            }
                                            if (notification_msg.Contains("[base_cur]") == true)
                                            {
                                                notification_msg = notification_msg.Replace("[base_cur]", basecurrency);
                                            }
                                            int i = CompanyInfo.check_notification_perm(Convert.ToString(referrer), obj.Client_ID, obj.Branch_ID, 6, 61, Convert.ToDateTime(obj.RecordDate), 0, SMS, Email, Notif_status, "Web-Referral Wallet Notification - 61", notification_msg, context);
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                    }

                                }
                                //using (cmd6 = new MySqlCommand("updatequalifysignupstatusreferrer", con))
                                {
                                    cmd6 = new MySqlCommand("updatequalifysignupstatusreferrer", con);
                                    cmd6.CommandType = CommandType.StoredProcedure;

                                    cmd6.Parameters.AddWithValue("_referrer", referrer);
                                    cmd6.Parameters.AddWithValue("_referee", referee_ID);
                                    //iw4.transfer_status = 1;
                                    cmd6.Parameters.AddWithValue("_Record_Insert_DateTime", obj.RecordDate);
                                    cmd6.Parameters.AddWithValue("_qualify_status", 1);
                                    cmd6.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                                    //cmd6.ExecuteNonQuery();
                                    int a = db_connection.ExecuteNonQueryProcedure(cmd6);
                                    //cmd6.Dispose();
                                }
                            }
                            #endregion

                            #region referree2
                            if (schemetype == 2 && mintranamt == 0 && maxno >= Convert.ToInt32(lastinvitecount))
                            {
                                //checking multiples of...if a multiple then add perk to referrer
                                if ((Convert.ToInt32(lastinvitecount) + 1) % multiplesof == 0)
                                {
                                    if (referer_perk == 2)
                                    {
                                        //using (cmd6 = new MySqlCommand("sp_Save_DiscountDetails", con))
                                        {
                                            cmd6 = new MySqlCommand("sp_Save_DiscountDetails", con);
                                            cmd6.CommandType = CommandType.StoredProcedure;
                                            cmd6.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                                            cmd6.Parameters.AddWithValue("_Branch_ID", obj.Branch_ID);
                                            cmd6.Parameters.AddWithValue("_Discount_Code", referrer_discountcode);
                                            cmd6.Parameters.AddWithValue("_discount_type_id", 1);
                                            if (referee_discount_type == 1)
                                            {
                                                cmd6.Parameters.AddWithValue("_amount_type_id", 2);
                                            }
                                            else
                                            {
                                                cmd6.Parameters.AddWithValue("_amount_type_id", 1);
                                            }

                                            cmd6.Parameters.AddWithValue("_discount_value", Convert.ToString(referer_value));
                                            cmd6.Parameters.AddWithValue("_customer_eligibility_id", 3);
                                            if (refereenotx == 0)
                                            {
                                                cmd6.Parameters.AddWithValue("_usagelimit", 1);
                                            }
                                            else
                                            {
                                                cmd6.Parameters.AddWithValue("_usagelimit", referernotx);
                                            }

                                            cmd6.Parameters.AddWithValue("_usagelimit_flag", 5);
                                            DateTime dto = DateTime.Now;
                                            DateTime dto1 = dto.AddMonths(12);

                                            cmd6.Parameters.AddWithValue("_start_date", obj.RecordDate);


                                            if (dt.Rows.Count > 0)
                                            {
                                                string timezone = Convert.ToString(dt.Rows[0]["BaseCurrency_Timezone"]);
                                                if (timezone != "" && timezone != null)
                                                {
                                                    var britishZone = TimeZoneInfo.FindSystemTimeZoneById("" + timezone + "");
                                                    dto1 = TimeZoneInfo.ConvertTime(dto1, TimeZoneInfo.Local, britishZone);
                                                }
                                            }
                                            cmd6.Parameters.AddWithValue("_end_date", dto1.ToString("yyyy-MM-dd HH:mm:ss"));
                                            cmd6.Parameters.AddWithValue("_Delete_Status", 0);
                                            cmd6.Parameters.AddWithValue("_Record_Insert_DateTime", obj.RecordDate);
                                            cmd6.Parameters.AddWithValue("_minAmount", Minimum_Transfer_Amount);
                                            cmd6.Parameters.AddWithValue("_maxAmount", Maximum_Transfer_Amount);
                                            cmd6.ExecuteNonQuery();
                                            //cmd6.Dispose();
                                        }

                                        //get discount id

                                        cmd6 = new MySqlCommand("CheckDuplicateDiscountCode");
                                        cmd6.CommandType = CommandType.StoredProcedure;
                                        cmd6.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                                        cmd6.Parameters.AddWithValue("_Discount_Code", referrer_discountcode);
                                        DataTable ds = db_connection.ExecuteQueryDataTableProcedure(cmd6);

                                        string disc_id = Convert.ToString(ds.Rows[0]["ID"]);

                                        //cmd6.Dispose();
                                        //insert in customereligibility_table
                                        //using (cmd6 = new MySqlCommand("Save_CustomerEligibilityDetails", con))
                                        {
                                            cmd6 = new MySqlCommand("Save_CustomerEligibilityDetails", con);
                                            cmd6.CommandType = CommandType.StoredProcedure;
                                            cmd6.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                                            cmd6.Parameters.AddWithValue("_Id", Convert.ToInt32(disc_id));
                                            cmd6.Parameters.AddWithValue("_customer_eligibility_id", 3);
                                            cmd6.Parameters.AddWithValue("_Customer_ID", referrer);
                                            cmd6.Parameters.AddWithValue("Delete_Status", 0);
                                            cmd6.Parameters.AddWithValue("_Branch_ID", obj.Branch_ID);

                                            cmd6.Parameters.AddWithValue("_DeliveryType_Id", 5);
                                            int a_discount1 = cmd6.ExecuteNonQuery();
                                            string notification_icon = "discount.jpg";
                                            string notification_message = "<span class='cls-admin'> new <strong class='cls-reward'>Discount coupon " + referrer_discountcode + "</strong> is added.</span><span class='cls-customer'><strong>Discount coupon " + referrer_discountcode + "</strong><span>A new discount coupon is added to your account.</span></span>";
                                            if (a_discount1 == 1)
                                            {
                                                CompanyInfo.save_notification(notification_message, notification_icon, Convert.ToInt32(referrer), Convert.ToDateTime(obj.Record_Insert_DateTime), Convert.ToInt32(obj.Client_ID), 1, 0, Convert.ToInt32(obj.Branch_ID), 0, 1, 1, 0, context);
                                                try
                                                {
                                                    DataTable dt_notif = CompanyInfo.set_notification_data(60);
                                                    if (dt_notif.Rows.Count > 0)
                                                    {
                                                        int SMS = Convert.ToInt32(dt_notif.Rows[0]["SMS"]);
                                                        int Email1 = Convert.ToInt32(dt_notif.Rows[0]["Email"]);
                                                        int Notif_status = Convert.ToInt32(dt_notif.Rows[0]["Notification"]);
                                                        string notification_msg = Convert.ToString(dt_notif.Rows[0]["notification_msg"]);
                                                        if (notification_msg.Contains("[discountcode] ") == true)
                                                        {
                                                            notification_msg = notification_msg.Replace("[discountcode] ", Convert.ToString(referrer_discountcode));
                                                        }

                                                        int i1 = CompanyInfo.check_notification_perm(Convert.ToString(referrer), obj.Client_ID, obj.Branch_ID, 6, 60, Convert.ToDateTime(obj.RecordDate), 0, SMS, Email1, Notif_status, "Web- Referral Discount Notification - 60", notification_msg, context);
                                                    }
                                                }
                                                catch (Exception ex)
                                                {
                                                }

                                            }
                                            //cmd6.Dispose();
                                        }

                                    }
                                    else if (referer_perk == 1)
                                    {
                                        cmd6 = new MySqlCommand("getwalletRefforbasecurrency");
                                        cmd6.CommandType = CommandType.StoredProcedure;
                                        string basecurrency = string.Empty;
                                        if (dt.Rows.Count > 0)
                                        {
                                            basecurrency = Convert.ToString(dt.Rows[0]["BaseCurrency_Code"]);
                                        }
                                        cmd6.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                                        cmd6.Parameters.AddWithValue("_WireTransfer_ReferanceNo", referrer_refno);
                                        cmd6.Parameters.AddWithValue("_BaseCurrency", basecurrency);
                                        cmd6.Parameters.AddWithValue("_AgentFlag", Referrer_Flag);
                                        DataTable dtwal = db_connection.ExecuteQueryDataTableProcedure(cmd6);

                                        //cmd6.Dispose();
                                        cmd6 = new MySqlCommand("getbasecurrencyid");
                                        cmd6.CommandType = CommandType.StoredProcedure;
                                        cmd6.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                                        cmd6.Parameters.AddWithValue("_BaseCurrency", basecurrency);
                                        string cid = Convert.ToString(db_connection.ExecuteScalarProcedure(cmd6));
                                        //cmd6.Dispose();
                                        string wrefNo = string.Empty;
                                        if (dtwal.Rows.Count == 0)//create wallet ref for referrer
                                        {
                                            wrefNo = GenerateWalletReference(obj.Client_ID);

                                            //using (cmd6 = new MySqlCommand("insertinwallet_table", con))
                                            {
                                                cmd6 = new MySqlCommand("insertinwallet_table", con);
                                                cmd6.CommandType = CommandType.StoredProcedure;
                                                cmd6.Parameters.AddWithValue("_WireTransfer_ReferanceNo", referrer_refno);
                                                cmd6.Parameters.AddWithValue("_Currency_ID", Convert.ToInt32(cid));
                                                //iw1.Client_ID = client_id;
                                                cmd6.Parameters.AddWithValue("_Delete_Status", 0);
                                                cmd6.Parameters.AddWithValue("_Record_Insert_DateTime", obj.RecordDate);
                                                cmd6.Parameters.AddWithValue("_wallet_reference", wrefNo);
                                                cmd6.Parameters.AddWithValue("_Wallet_balance", 0);
                                                cmd6.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                                                cmd6.Parameters.AddWithValue("_Branch_ID", obj.Branch_ID);
                                                cmd6.Parameters.AddWithValue("_Customer_ID", referrer);
                                                cmd6.Parameters.AddWithValue("_AgentFlag", Referrer_Flag);
                                                cmd6.ExecuteNonQuery();
                                                //cmd6.Dispose();
                                            }
                                        }
                                        cmd6 = new MySqlCommand("getwalletid");
                                        cmd6.CommandType = CommandType.StoredProcedure;
                                        cmd6.Parameters.AddWithValue("_WireTransfer_ReferanceNo", referrer_refno);
                                        cmd6.Parameters.AddWithValue("_Currency_ID", Convert.ToInt32(cid));
                                        cmd6.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                                        walletid = Convert.ToString(db_connection.ExecuteScalarProcedure(cmd6));
                                        //cmd6.Dispose();

                                        cmd6 = new MySqlCommand("checkinwalletbalance");
                                        cmd6.CommandType = CommandType.StoredProcedure;
                                        cmd6.Parameters.AddWithValue("_wallet_id", Convert.ToInt32(walletid));

                                        DataTable dtwbal = db_connection.ExecuteQueryDataTableProcedure(cmd6);
                                        //cmd6.Dispose();

                                        double oldbal = 0, newbal = 0;
                                        if (dtwbal.Rows.Count > 0)
                                        {
                                            oldbal = Convert.ToDouble(dtwbal.Rows[0]["newBalance"]);


                                        }
                                        newbal = oldbal + referer_value;
                                        int paytype = 10, userid = 1;
                                        double exchangerate = 1, trfee = 0;
                                        cmd6 = new MySqlCommand("getPType_ID");
                                        cmd6.CommandType = CommandType.StoredProcedure;

                                        paytype = Convert.ToInt32(db_connection.ExecuteScalarProcedure(cmd6));

                                        //cmd6.Dispose();
                                        //using (cmd6 = new MySqlCommand("insert_wallet_transaction", con))
                                        {
                                            cmd6 = new MySqlCommand("insert_wallet_transaction", con);
                                            cmd6.CommandType = CommandType.StoredProcedure;

                                            cmd6.Parameters.AddWithValue("_wallet_id", Convert.ToInt32(walletid));
                                            cmd6.Parameters.AddWithValue("_transfer_type", 1);
                                            cmd6.Parameters.AddWithValue("_Currency_ID", Convert.ToInt32(cid));
                                            cmd6.Parameters.AddWithValue("_transfer_amount", referer_value);//referer_value;
                                            cmd6.Parameters.AddWithValue("_Wallet_Description", "");
                                            cmd6.Parameters.AddWithValue("_oldwalletbalance", oldbal);
                                            cmd6.Parameters.AddWithValue("_newwalletbalance", newbal);
                                            cmd6.Parameters.AddWithValue("_Record_Insert_DateTime", obj.RecordDate);
                                            cmd6.Parameters.AddWithValue("_Delete_Status", 0);
                                            cmd6.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                                            cmd6.Parameters.AddWithValue("_Branch_ID", obj.Branch_ID);
                                            cmd6.Parameters.AddWithValue("_paytype", paytype);
                                            cmd6.Parameters.AddWithValue("_exchangerate", exchangerate);
                                            cmd6.Parameters.AddWithValue("_Transaction_ID", 0);
                                            cmd6.Parameters.AddWithValue("_fee", trfee);
                                            cmd6.Parameters.AddWithValue("_User_ID", userid);
                                            cmd6.Parameters.AddWithValue("_AgentFlag", Referrer_Flag);
                                            cmd6.Parameters.AddWithValue("_referee_id", referee_ID);
                                            cmd6.Parameters.AddWithValue("_Referral_Flag", 0);
                                            cmd6.ExecuteNonQuery();
                                            //cmd6.Dispose();
                                        }
                                        //using (cmd6 = new MySqlCommand("updateinwallettable", con))
                                        int a_wallet = 0;
                                        {
                                            cmd6 = new MySqlCommand("updateinwallettable", con);
                                            cmd6.CommandType = CommandType.StoredProcedure;

                                            cmd6.Parameters.AddWithValue("_transfer_amount", newbal);
                                            cmd6.Parameters.AddWithValue("_wallet_id", Convert.ToInt32(walletid));
                                            cmd6.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                                            cmd6.Parameters.AddWithValue("_Branch_ID", obj.Branch_ID);
                                            a_wallet = cmd6.ExecuteNonQuery();
                                            //cmd6.Dispose();
                                        }
                                        //using (cmd6 = new MySqlCommand("updatewalletrefererInvite", con))
                                        {
                                            cmd6 = new MySqlCommand("updatewalletrefererInvite", con);
                                            cmd6.CommandType = CommandType.StoredProcedure;

                                            cmd6.Parameters.AddWithValue("_referrer", referrer);
                                            cmd6.Parameters.AddWithValue("_referee", referee_ID);
                                            cmd6.Parameters.AddWithValue("_discount_code", wrefNo);
                                            cmd6.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                                            cmd6.ExecuteNonQuery();
                                            //cmd6.Dispose();
                                        }
                                        string notification_icon = "wallet.jpg";
                                        string notification_message = "<span class='cls-admin'> new <strong class='cls-reward'>" + basecurrency + " " + referee_value + "</strong> is added to the wallet.</span><span class='cls-customer'><strong> " + basecurrency + " " + referee_value + " added to your wallet. </strong><span>" + basecurrency + " " + referee_value + " was successfully added to your wallet.</span></span>";
                                        if (a_wallet == 1)
                                        {
                                            CompanyInfo.save_notification(notification_message, notification_icon, Convert.ToInt32(referrer), Convert.ToDateTime(obj.RecordDate), Convert.ToInt32(obj.Client_ID), 1, 0, Convert.ToInt32(obj.Branch_ID), 0, 0, 1, 0, context);
                                            try
                                            {
                                                DataTable dt_notif = CompanyInfo.set_notification_data(61);
                                                if (dt_notif.Rows.Count > 0)
                                                {
                                                    int SMS = Convert.ToInt32(dt_notif.Rows[0]["SMS"]);
                                                    int Email = Convert.ToInt32(dt_notif.Rows[0]["Email"]);
                                                    int Notif_status = Convert.ToInt32(dt_notif.Rows[0]["Notification"]);
                                                    string notification_msg = Convert.ToString(dt_notif.Rows[0]["notification_msg"]);
                                                    if (notification_msg.Contains("[wallet_bal]") == true)
                                                    {
                                                        notification_msg = notification_msg.Replace("[wallet_bal]", Convert.ToString(referee_value));
                                                    }
                                                    if (notification_msg.Contains("[base_cur]") == true)
                                                    {
                                                        notification_msg = notification_msg.Replace("[base_cur]", basecurrency);
                                                    }
                                                    int i = CompanyInfo.check_notification_perm(Convert.ToString(referrer), obj.Client_ID, obj.Branch_ID, 6, 61, Convert.ToDateTime(obj.RecordDate), 0, SMS, Email, Notif_status, "Web-Referral Wallet Notification - 61", notification_msg, context);
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                            }
                                        }

                                    }
                                    //using (cmd6 = new MySqlCommand("updatequalifysignupstatusreferrer", con))
                                    {
                                        cmd6 = new MySqlCommand("updatequalifysignupstatusreferrer", con);
                                        cmd6.CommandType = CommandType.StoredProcedure;

                                        cmd6.Parameters.AddWithValue("_referrer", referrer);
                                        cmd6.Parameters.AddWithValue("_referee", referee_ID);
                                        //iw4.transfer_status = 1;
                                        cmd6.Parameters.AddWithValue("_Record_Insert_DateTime", obj.RecordDate);
                                        cmd6.Parameters.AddWithValue("_qualify_status", 1);
                                        cmd6.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                                        cmd6.ExecuteNonQuery();
                                        //cmd6.Dispose();
                                    }
                                }
                            }
                            #endregion
                        }

                    }
                    #endregion

                }
                catch (Exception _x)
                {
                    //Error Log Handled
                    Model.ErrorLog objError = new Model.ErrorLog();
                    objError.User = new Model.User();
                    objError.Error = "Api : Referral --" + _x.ToString();
                    objError.Date = DateTime.Now;
                    objError.User_ID = 1;
                    objError.Client_ID = obj.Client_ID;

                    Service.srvErrorLog srvError = new Service.srvErrorLog();
                    srvError.Create(objError, context);
                    throw _x;
                }
                finally
                {
                    con.Close();
                }
            }
        }


        public Model.Customer UpdateAddress(Model.Customer obj, HttpContext context)
        {
            int Customer_ID = Convert.ToInt32(CompanyInfo.Decrypt(Convert.ToString(obj.Id), true));
            string Customer_ID1 = Convert.ToString(CompanyInfo.Decrypt(Convert.ToString(obj.Id), true));
            string Activity = string.Empty; string notification_icon = ""; string notification_message = "";
            string Activity_city = string.Empty; string request = string.Empty;
            //var context = System.Web.HttpContext.Current;
            string Username = "";
            string PostCode_regex = validation.validate(obj.PostCode.Trim(), 1, 1, 0, 1, 1, 1, 1, 0, 0);
            string HouseNumber_regex = validation.validate(obj.HouseNumber, 1, 1, 1, 1, 1, 1, 1, 1, 0);
            string Street_regex = validation.validate(obj.Street, 1, 1, 1, 1, 1, 1, 1, 1, 0);
            string address2_regex = validation.validate(obj.AddressLine2, 1, 1, 1, 1, 1, 1, 1, 1, 0);//
            int address_regex_len = 100; string HouseNumber_len = "true"; string Street_len = "true"; string company_len = "true"; string AddressLine2_len = "true";
            if (obj.HouseNumber.Length >= address_regex_len)
            {
                HouseNumber_len = "false";
            }
            if (obj.Street.Length >= address_regex_len)
            {
                Street_len = "false";
            }
            if (obj.AddressLine2.Length >= address_regex_len)
            {
                AddressLine2_len = "false";
            }//change sqlinjection
            if (PostCode_regex != "false" && HouseNumber_regex != "false" && Street_regex != "false" && address2_regex != "false")
            {
                string user = CompanyInfo.testInjection(obj.UserName);
                string _House = CompanyInfo.testInjection(obj.HouseNumber);
                string _street = CompanyInfo.testInjection(obj.Street);
                string _Addressline_2 = CompanyInfo.testInjection(obj.AddressLine2);
                string _postcode = CompanyInfo.testInjection(obj.PostCode);

                int newTest = (int)CompanyInfo.InsertActivityLogDetailsSecurity("values : user:" + user + " & _House:" + _House + " & _street" + _street + " & _Addressline_2=" + _Addressline_2 + " & _postcode:" + _postcode, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), 1, "srvCustomer", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "UpdateAddress", 0, context);

                if (user == "1" && _House == "1" && _street == "1" && _Addressline_2 == "1" && _postcode == "1")
                {
                    mts_connection _MTS = new mts_connection();
                    using (MySqlConnection con = new MySqlConnection(_MTS.WebConnSetting()))
                    {
                        con.Open();
                        MySqlTransaction transaction;
                        transaction = con.BeginTransaction(IsolationLevel.ReadCommitted);

                        try
                        {
                            using (MySqlCommand cmd = new MySqlCommand("sp_update_address", con))
                            {
                                MySqlCommand cmdupdate1 = new MySqlCommand("Get_Permissions", con);
                                cmdupdate1.CommandType = CommandType.StoredProcedure;
                                cmdupdate1.Parameters.AddWithValue("Per_ID", 42);
                                cmdupdate1.Parameters.AddWithValue("ClientID", obj.Client_ID);

                                //cmdupdate1.ExecuteScalar(); 
                                obj.RecordDate = Convert.ToDateTime(CompanyInfo.gettime(obj.Client_ID, Customer_ID1, obj.Country_Id, context));
                                obj.CommentUserId = 1;
                                // int abcd = Convert.ToInt32(cmdupdate1.Parameters["Status"].Value);
                                DataTable dt1 = db_connection.ExecuteQueryDataTableProcedure(cmdupdate1); //Convert.ToInt32(cmdupdate1.Parameters["Status"].Value);
                                obj.per_status = Convert.ToInt32(dt1.Rows[0]["Status_ForCustomer"]);
                                // var per=0;


                                if (obj.per_status == 1)
                                {

                                    cmd.CommandType = CommandType.StoredProcedure;
                                    //obj.HouseNumber = "cc";
                                    //obj.Street = "23";
                                    cmd.Parameters.AddWithValue("_Country_id", obj.Country_Id);
                                    cmd.Parameters.AddWithValue("_House", obj.HouseNumber);
                                    cmd.Parameters.AddWithValue("_street", obj.Street);
                                    cmd.Parameters.AddWithValue("_Addressline_2", obj.AddressLine2);
                                    cmd.Parameters.AddWithValue("_city", obj.cityId);
                                    cmd.Parameters.AddWithValue("_postcode", obj.PostCode);
                                    cmd.Parameters.AddWithValue("_customerId", Customer_ID);
                                    cmd.Parameters.AddWithValue("_Branch_ID", obj.Branch_ID);
                                    cmd.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                                    cmd.Transaction = transaction;
                                    int n = cmd.ExecuteNonQuery();

                                    Model.Customer _ObjCustomer = new Model.Customer();
                                    // Service.srvCustomer srvCustomer = new Service.srvCustomer();
                                    // response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_SUCCESS);
                                    // _ObjCustomer = srvCustomer.UpdatePhone(obj);
                                    Service.srvLogin srvLogin = new Service.srvLogin();
                                    DataTable dt = new DataTable();
                                    Model.Login objLogin = new Model.Login();
                                    Model.Customer objcust = new Model.Customer();


                                    objLogin.Client_ID = obj.Client_ID;
                                    MySqlCommand cmd1 = new MySqlCommand("GetCustDetailsByID", con);
                                    cmd1.CommandType = CommandType.StoredProcedure;
                                    cmd1.Parameters.AddWithValue("cust_ID", Customer_ID);

                                    DataTable dt_cust = db_connection.ExecuteQueryDataTableProcedure(cmd1);
                                    objLogin.UserName = Convert.ToString(dt_cust.Rows[0]["Email_ID"]);
                                    dt = srvLogin.IsValidEmail(objLogin);



                                    if (n > 0)
                                    {
                                        try
                                        {
                                            DataTable dt_notif = CompanyInfo.set_notification_data(54);
                                            if (dt_notif.Rows.Count > 0)
                                            {
                                                int SMS = Convert.ToInt32(dt_notif.Rows[0]["SMS"]);
                                                int Email = Convert.ToInt32(dt_notif.Rows[0]["Email"]);
                                                int Notif_status = Convert.ToInt32(dt_notif.Rows[0]["Notification"]);
                                                string notification_msg = Convert.ToString(dt_notif.Rows[0]["notification_msg"]);
                                                int i = CompanyInfo.check_notification_perm(Convert.ToString(Customer_ID), obj.Client_ID, obj.Branch_ID, 1, 54, Convert.ToDateTime(obj.RecordDate), 1, SMS, Email, Notif_status, "App - Update Address Notification - 54", notification_msg, context);
                                            }
                                        }
                                        catch (Exception ex) { }
                                        notification_icon = "add-changed.jpg";
                                        notification_message = "<span class='cls-admin'>successully changed <strong class='cls-addr'>Change of address</strong>.</span><span class='cls-customer'><strong>Address change</strong><span>Your have successfully changed your address.</span></span>";
                                        CompanyInfo.save_notification(notification_message, notification_icon, Convert.ToInt32(Customer_ID), Convert.ToDateTime(obj.Record_Insert_DateTime), Convert.ToInt32(obj.Client_ID), 1, 0, Convert.ToInt32(obj.Branch_ID), 0, 0, 1, 0, context);
                                        Activity = "<b>" + Username + "</b>" + "Address changed.  </br>";
                                        int stattus = (int)CompanyInfo.InsertActivityLogDetails(Activity, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(Customer_ID), "UpdateAddress", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "Update Address", context);
                                        string email = obj.Email;
                                        string body = string.Empty, subject = string.Empty;
                                        string body1 = string.Empty;
                                        //string template = "";
                                        System.Net.HttpWebRequest httpRequest = null, httpRequest1 = null;
                                        DataTable dtc = CompanyInfo.get(obj.Client_ID, context);
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
                                            body = body.Replace("[msg]", "Address updated successfully.");

                                            httpRequest1 = (HttpWebRequest)WebRequest.Create(URL + "Email/my-account.txt");
                                            httpRequest1.UserAgent = "Code Sample Web Client";
                                            HttpWebResponse webResponse1 = (HttpWebResponse)httpRequest1.GetResponse();
                                            using (StreamReader reader = new StreamReader(webResponse1.GetResponseStream()))
                                            {
                                                subject = reader.ReadLine();
                                            }
                                            string newsubject = company_name + " - " + subject + Convert.ToString("");
                                            string msg = (string)CompanyInfo.Send_Mail(dtc, objLogin.UserName, body, newsubject, obj.Client_ID, obj.Branch_ID, "", "", "", context);
                                            obj.Message = "success";
                                        }

                                    }
                                }

                                else if (obj.per_status == 0)
                                {
                                    Activity_city = "start step1";
                                    using (MySqlCommand cmd1 = new MySqlCommand("Customer_getallcustdetails", con))
                                    {
                                        Activity_city += "step2";
                                        cmd1.CommandType = CommandType.StoredProcedure;

                                        cmd1.Parameters.AddWithValue("_Id", Customer_ID);

                                        DataTable dt = db_connection.ExecuteQueryDataTableProcedure(cmd1);

                                        if (dt.Rows.Count > 0)
                                        {
                                            Activity_city += "step3 and dt count :- " + dt.Rows.Count;
                                            if (dt.Rows[0]["House_Number"].Equals(obj.HouseNumber))
                                            {
                                                Activity += "step4";
                                            }
                                            else
                                            {
                                                if (obj.HouseNumber != null && obj.HouseNumber != "")
                                                {
                                                    Activity += "step4.1" + request;
                                                    request += " House  Number:" + obj.HouseNumber + ",";
                                                }

                                            }
                                            if (dt.Rows[0]["Street"].Equals(obj.Street))
                                            {
                                                Activity += "step5";
                                            }
                                            else
                                            {
                                                if (obj.Street != null && obj.Street != "")
                                                {
                                                    Activity_city += "step5.1" + request;
                                                    request += "Street:" + obj.Street + ",";
                                                }

                                            }
                                            if (dt.Rows[0]["Addressline_2"].Equals(obj.AddressLine2))
                                            {
                                                Activity_city += "step6";
                                            }
                                            else
                                            {
                                                if (obj.AddressLine2 != null && obj.AddressLine2 != "")
                                                {
                                                    Activity_city += "step6.1" + request;
                                                    request += "Addressline_2:" + obj.AddressLine2 + ",";
                                                }

                                            }

                                            int stattusjj = (int)CompanyInfo.InsertActivityLogDetails("request Value: " + request, Convert.ToInt32(0), 0, Convert.ToInt32(0), Convert.ToInt32(0), "Update Address", Convert.ToInt32(0), Convert.ToInt32(0), "Update Address", context);

                                            if (dt.Rows[0]["City_ID"].Equals(obj.cityId))
                                            {
                                                Activity_city += "step7";
                                                int stattusjjq = (int)CompanyInfo.InsertActivityLogDetails("request Value7: " + obj.cityId, Convert.ToInt32(0), 0, Convert.ToInt32(0), Convert.ToInt32(0), "Update Address", Convert.ToInt32(0), Convert.ToInt32(0), "Update Address", context);
                                            }
                                            else
                                            {
                                                if (obj.cityId != 0)
                                                {
                                                    Activity_city += "step7.1 cityId:-" + obj.cityId;
                                                    // request += "City ID:" + obj.cityId + ",";
                                                    int stattusjjq = (int)CompanyInfo.InsertActivityLogDetails("request Value7: " + obj.cityId, Convert.ToInt32(0), 0, Convert.ToInt32(0), Convert.ToInt32(0), "Update Address", Convert.ToInt32(0), Convert.ToInt32(0), "Update Address", context);
                                                    try
                                                    {
                                                        string cityName = string.Empty;
                                                        int stattusjjl = (int)CompanyInfo.InsertActivityLogDetails("request Value0: " + "", Convert.ToInt32(0), 0, Convert.ToInt32(0), Convert.ToInt32(0), "Update Address", Convert.ToInt32(0), Convert.ToInt32(0), "Update Address", context);
                                                        using (MySqlCommand cmdCity = new MySqlCommand("sp_select_city_detailsbyid", con))
                                                        {
                                                            int stattusjjb = (int)CompanyInfo.InsertActivityLogDetails("request Value0: " + "1start", Convert.ToInt32(0), 0, Convert.ToInt32(0), Convert.ToInt32(0), "Update Address", Convert.ToInt32(0), Convert.ToInt32(0), "Update Address", context);
                                                            cmdCity.CommandType = CommandType.StoredProcedure;
                                                            cmdCity.Parameters.AddWithValue("_whereclause", obj.cityId);
                                                            DataTable dt_cityname = db_connection.ExecuteQueryDataTableProcedure(cmdCity);
                                                            Activity_city += "step7.2 dt_cityname count" + dt_cityname.Rows.Count;
                                                            int stattusjjr = (int)CompanyInfo.InsertActivityLogDetails("request Value8: " + dt_cityname.Rows.Count, Convert.ToInt32(0), 0, Convert.ToInt32(0), Convert.ToInt32(0), "Update Address", Convert.ToInt32(0), Convert.ToInt32(0), "Update Address", context);
                                                            cityName = Convert.ToString(dt_cityname.Rows[0]["City_Name"]);
                                                            Activity_city += "step7.2 cityName :-" + cityName;
                                                            int stattusjjt = (int)CompanyInfo.InsertActivityLogDetails("request Value9: " + cityName, Convert.ToInt32(0), 0, Convert.ToInt32(0), Convert.ToInt32(0), "Update Address", Convert.ToInt32(0), Convert.ToInt32(0), "Update Address", context);
                                                            request += " City Name: " + cityName + ",";
                                                            int stattusjjh = (int)CompanyInfo.InsertActivityLogDetails("request Value10: " + request, Convert.ToInt32(0), 0, Convert.ToInt32(0), Convert.ToInt32(0), "Update Address", Convert.ToInt32(0), Convert.ToInt32(0), "Update Address", context);
                                                        }
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        int stattus = (int)CompanyInfo.InsertActivityLogDetails("Get City Name while Update address error: " + ex.ToString(), Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(Customer_ID), "Update Address", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "Update Address", context);
                                                    }

                                                }

                                            }
                                            if (dt.Rows[0]["Post_Code"].Equals(obj.PostCode))
                                            {
                                            }
                                            else
                                            {
                                                if (obj.PostCode != null && obj.PostCode != "")
                                                {
                                                    request += "Post_Code:" + obj.PostCode + ",";
                                                }

                                            }
                                            if (dt.Rows[0]["Customer_Id"].Equals(Customer_ID))
                                            {
                                            }
                                            else
                                            {
                                                if (Customer_ID != null && Customer_ID != 0)
                                                {
                                                    request += "Customer Id:" + Customer_ID + ",";
                                                }

                                            }
                                            if (dt.Rows[0]["Branch_ID"].Equals(obj.Branch_ID))
                                            {
                                            }
                                            else
                                            {
                                                request += "Branch Id:" + obj.Branch_ID + ",";
                                            }
                                            if (dt.Rows[0]["Client_ID"].Equals(obj.Client_ID))
                                            {
                                            }
                                            else
                                            {
                                                request += "Client Id:" + obj.Client_ID + ",";
                                            }
                                            stattusjj = (int)CompanyInfo.InsertActivityLogDetails("request Value2: " + request, Convert.ToInt32(0), 0, Convert.ToInt32(0), Convert.ToInt32(0), "Update Address", Convert.ToInt32(0), Convert.ToInt32(0), "Update Address", context);
                                            request = request.TrimEnd(',');

                                            string splitter = "<br/>";
                                            string subject_line = "Address Information";
                                            string final_subject_line = " Edit address details request sent.";
                                            obj.whereclause = "Request From Customer App : <b>" + dt.Rows[0]["WireTransfer_ReferanceNo"] + "</b>" + "<br/>Request Details :" + request.TrimEnd();
                                            obj.Comment = final_subject_line;
                                            obj.AllCustomer_Flag = "1";//response flag
                                            obj.Delete_Status = 0;
                                            obj.cashcollection_flag = "1";//active status----default status is pending
                                            obj.Record_Insert_DateTime = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
                                            obj.Flag = 1;
                                            using (MySqlCommand cmd2 = new MySqlCommand("Edit_Request", con))
                                            {
                                                cmd2.CommandType = CommandType.StoredProcedure;
                                                //obj.HouseNumber = "cc";
                                                //obj.Street = "23";

                                                cmd2.Parameters.AddWithValue("_Id", Customer_ID);
                                                cmd2.Parameters.AddWithValue("_Reason", obj.whereclause);
                                                cmd2.Parameters.AddWithValue("_AllCustomer_Flag", obj.AllCustomer_Flag);

                                                cmd2.Parameters.AddWithValue("_Delete_Status", obj.Delete_Status);
                                                cmd2.Parameters.AddWithValue("_cashcollection_flag", obj.cashcollection_flag);
                                                cmd2.Parameters.AddWithValue("_Record_Insert_DateTime", obj.RecordDate);
                                                cmd2.Parameters.AddWithValue("_Branch_ID", obj.Branch_ID);
                                                cmd2.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                                                cmd2.Parameters.AddWithValue("_comments", obj.Comment);
                                                cmd2.Parameters.AddWithValue("_flag", obj.Flag);
                                                cmd2.Transaction = transaction;
                                                int n = cmd2.ExecuteNonQuery();
                                                obj.Message = "Request";
                                                if (n > 0)
                                                {
                                                    try
                                                    {
                                                        DataTable dt_notif = CompanyInfo.set_notification_data(42);
                                                        if (dt_notif.Rows.Count > 0)
                                                        {
                                                            int SMS = Convert.ToInt32(dt_notif.Rows[0]["SMS"]);
                                                            int Email = Convert.ToInt32(dt_notif.Rows[0]["Email"]);
                                                            int Notif_status = Convert.ToInt32(dt_notif.Rows[0]["Notification"]);
                                                            string notification_msg = Convert.ToString(dt_notif.Rows[0]["notification_msg"]);
                                                            int i = CompanyInfo.check_notification_perm(Convert.ToString(Customer_ID), obj.Client_ID, obj.Branch_ID, 1, 42, Convert.ToDateTime(obj.RecordDate), 1, SMS, Email, Notif_status, "App - Update Address Notification - 42", notification_msg, context);
                                                        }
                                                    }
                                                    catch (Exception ex) { }
                                                    obj.Message = "Request";
                                                    notification_icon = "address-change.jpg";
                                                    notification_message = "<span class='cls-admin'>sent Request for <strong class='cls-addr'>Change of address</strong>.</span><span class='cls-customer'><strong>Address change request</strong><span>Your address change request is submitted.</span></span>";
                                                    CompanyInfo.save_notification(notification_message, notification_icon, Convert.ToInt32(Customer_ID), Convert.ToDateTime(obj.RecordDate), Convert.ToInt32(obj.Client_ID), 1, 0, Convert.ToInt32(obj.Branch_ID), 0, 0, 1, 0, context);

                                                    Activity = "<b>" + Username + "</b>" + "changed Address  details." + request + "</br>";
                                                    obj.User_ID = 1;
                                                    var abc = Customer_ID;
                                                    int stattus = (int)CompanyInfo.InsertActivityLogDetails(Activity, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(Customer_ID), "Update Address", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "Update Address", context);

                                                    // int stattus = (int)CompanyInfo.InsertActivityLogDetails(Activity, 1, 0, 1, 1, "UpdatePhone", 1, 1, "Update Phone");
                                                    string email = obj.Email;
                                                    string body = string.Empty, subject = string.Empty;
                                                    string body1 = string.Empty;
                                                    //string template = "";
                                                    System.Net.HttpWebRequest httpRequest = null, httpRequest1 = null;
                                                    DataTable dtc = CompanyInfo.get(obj.Client_ID, context);
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
                                                        obj.Full_Name = Convert.ToString(dt.Rows[0]["Full_name"]);
                                                        body = body.Replace("[name]", Convert.ToString(dt.Rows[0]["First_Name"])); //obj.Full_Name);
                                                        body = body.Replace("[msg]", "Edit address details request sent successfully.");

                                                        //httpRequest1 = (HttpWebRequest)WebRequest.Create(URL + "Email/my-account.txt");
                                                        //httpRequest1.UserAgent = "Code Sample Web Client";
                                                        //HttpWebResponse webResponse1 = (HttpWebResponse)httpRequest1.GetResponse();
                                                        //using (StreamReader reader = new StreamReader(webResponse1.GetResponseStream()))
                                                        //{
                                                        //    subject = reader.ReadLine();
                                                        //}
                                                        subject = "Edit Address Request";
                                                        string newsubject = company_name + " - " + subject + Convert.ToString("");
                                                        string msg = (string)CompanyInfo.Send_Mail(dtc, obj.UserName, body, newsubject, obj.Client_ID, obj.Branch_ID, "", "", "", context);
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            newTest = (int)CompanyInfo.InsertActivityLogDetailsSecurity("values : 0 count found", Convert.ToInt32(0), 0, Convert.ToInt32(0), 1, "srvCustomer", Convert.ToInt32(0), Convert.ToInt32(0), "UpdateAddress", 0, context);
                                            obj.Message = "Record not found";
                                        }
                                    }
                                }
                            }
                            transaction.Commit();
                        }
                        catch (Exception _x)
                        {
                            int stattusNew = (int)CompanyInfo.InsertActivityLogDetailsSecurity("Error Update contact: " + _x.ToString(), Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), 1, "srvCustomer", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "UpdateAddress", 0, context);
                            obj.Message = _x.ToString();
                            transaction.Rollback();
                            throw _x;
                        }
                        finally
                        {
                            int stattus_city = (int)CompanyInfo.InsertActivityLogDetailsSecurity("Get acitivity city: " + Activity_city, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), 1, "srvCustomer", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "UpdateAddress", 0, context);
                            int stattus = (int)CompanyInfo.InsertActivityLogDetails(Activity_city, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(Customer_ID), "UpdateAddress", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "Update Address", context);
                            con.Close();
                        }
                        // obj.Message = "success";

                    }
                }
                else
                {
                    obj.Message = "Validation Failed";

                }
            }
            else
            {
                string msg = "Validation Error PostCode_regex " + PostCode_regex + " HouseNumber_regex-" + HouseNumber_regex + " Street_regex-" + Street_regex + " address2_regex-" + address2_regex;
                int stattus = (int)CompanyInfo.InsertActivityLogDetailsSecurity(msg, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), 1, "srvCustomer", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "UpdateAddress", 0, context);

            }

            return obj;
            //}

        }



        public Model.Customer UpdateEmail(Model.Customer obj, HttpContext context)
        {
            int Customer_ID = Convert.ToInt32(CompanyInfo.Decrypt(Convert.ToString(obj.Id), true));
            string Activity = string.Empty; string error_msg = ""; string error_invalid_data = ""; string enjected_data = "";

            string Username = Convert.ToString(obj.UserName); //Convert.ToString(context.Request.Form["Username"]);
            string Email_regex = validation.validate(obj.UserName, 1, 1, 1, 1, 0, 1, 1, 1, 1);
            if (Email_regex == "false")
            {
                error_msg = "Please enter valid email without space";
                error_invalid_data = "Email: " + obj.Email;
            }
            string Email = CompanyInfo.testInjection(Convert.ToString(obj.Email));
            if (Email != "1")
            {
                enjected_data = "Email: " + obj.Email;
            }
            if (Email_regex != "false")
            {
                if (Email == "1")
                {

                    using (MySqlConnection con = new MySqlConnection(_MTS.WebConnSetting()))
                    {
                        con.Open();
                        try
                        {
                            MySqlCommand cmdupdate1 = new MySqlCommand("Get_Permissions", con);
                            cmdupdate1.CommandType = CommandType.StoredProcedure;
                            cmdupdate1.Parameters.AddWithValue("Per_ID", 42);
                            cmdupdate1.Parameters.AddWithValue("ClientID", obj.Client_ID);
                            //cmdupdate1.ExecuteScalar();
                            obj.CommentUserId = 1;
                            // int abcd = Convert.ToInt32(cmdupdate1.Parameters["Status"].Value);
                            DataTable dt1 = db_connection.ExecuteQueryDataTableProcedure(cmdupdate1); //Convert.ToInt32(cmdupdate1.Parameters["Status"].Value);
                            obj.per_status = Convert.ToInt32(dt1.Rows[0]["Status_ForCustomer"]);
                            if (obj.per_status == 1)
                            {

                                using (MySqlCommand cmd = new MySqlCommand("update_email", con))
                                {
                                    cmd.CommandType = CommandType.StoredProcedure;
                                    cmd.Parameters.AddWithValue("_Id", Customer_ID);
                                    cmd.Parameters.AddWithValue("_Email", obj.UserName);
                                    cmd.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                                    int n = cmd.ExecuteNonQuery();
                                    if (n > 0)
                                    {
                                        obj.Message = "success";

                                    }
                                    else
                                    {
                                        obj.Message = "failed";
                                    }
                                }
                            }
                            else if (obj.per_status == 0)
                            {
                                using (MySqlCommand cmd = new MySqlCommand("Customer_getallcustdetails", con))
                                {
                                    cmd.CommandType = CommandType.StoredProcedure;

                                    cmd.Parameters.AddWithValue("_Id", Customer_ID);
                                    var request = "";
                                    DataTable dt = db_connection.ExecuteQueryDataTableProcedure(cmd);
                                    if (dt.Rows.Count > 0)
                                    {
                                        if (dt.Rows[0]["Email_ID"].Equals(obj.UserName))
                                        {
                                        }
                                        else
                                        {
                                            if (obj.UserName != "" && obj.UserName != null)
                                            {
                                                request += " Email Id : " + obj.UserName + ",";
                                            }

                                        }
                                    }


                                    string splitter = "<br/>";
                                    string subject_line = "Address Information";
                                    string final_subject_line = "Edit email request sent.";
                                    obj.whereclause = "Request From Customer App : Old Email id: <b>" + dt.Rows[0]["Email_ID"] +
                                        "</b>" +
                                        "<br/>Request Details :" + request.TrimEnd();
                                    obj.Comment = final_subject_line;
                                    obj.AllCustomer_Flag = "1";//response flag
                                    obj.Delete_Status = 0;
                                    obj.cashcollection_flag = "1";//active status----default status is pending
                                    //obj.Record_Insert_DateTime = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
                                    obj.Record_Insert_DateTime = Convert.ToString(CompanyInfo.gettime(obj.Client_ID, (CompanyInfo.Decrypt(Convert.ToString(obj.Id), true)), obj.Country_Id, context));
                                    obj.RecordDate = Convert.ToDateTime(CompanyInfo.gettime(obj.Client_ID, (CompanyInfo.Decrypt(Convert.ToString(obj.Id), true)), obj.Country_Id, context));
                                    obj.Flag = 3;//editrequest flag update email 
                                    using (MySqlCommand cmd1 = new MySqlCommand("Edit_Request", con))
                                    {
                                        cmd1.CommandType = CommandType.StoredProcedure;
                                        //obj.HouseNumber = "cc";
                                        //obj.Street = "23";

                                        cmd1.Parameters.AddWithValue("_Id", Customer_ID);
                                        cmd1.Parameters.AddWithValue("_Reason", obj.whereclause);
                                        cmd1.Parameters.AddWithValue("_AllCustomer_Flag", obj.AllCustomer_Flag);

                                        cmd1.Parameters.AddWithValue("_Delete_Status", obj.Delete_Status);
                                        cmd1.Parameters.AddWithValue("_cashcollection_flag", obj.cashcollection_flag);
                                        cmd1.Parameters.AddWithValue("_Record_Insert_DateTime", obj.RecordDate);
                                        cmd1.Parameters.AddWithValue("_Branch_ID", obj.Branch_ID);
                                        cmd1.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                                        cmd1.Parameters.AddWithValue("_comments", obj.Comment);
                                        cmd1.Parameters.AddWithValue("_flag", obj.Flag);
                                        int n = cmd1.ExecuteNonQuery();
                                        obj.Message = "Request";
                                        if (n > 0)
                                        {
                                            Activity = "<b>" + Username + "</b>" + " changed contact details. " + request + "</br>";
                                            obj.User_ID = 1;
                                            var abc = Customer_ID;
                                            int stattus = (int)CompanyInfo.InsertActivityLogDetails(Activity, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(Customer_ID), "UpdatePhone", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "Update Phone", context);
                                            // int stattus = (int)CompanyInfo.InsertActivityLogDetails(Activity, 1, 0, 1, 1, "UpdatePhone", 1, 1, "Update Phone");
                                            string email = obj.Email;
                                            string body = string.Empty, subject = string.Empty;
                                            string body1 = string.Empty;
                                            //string template = "";
                                            System.Net.HttpWebRequest httpRequest = null, httpRequest1 = null;
                                            DataTable dtc = CompanyInfo.get(obj.Client_ID, context);
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
                                                obj.Full_Name = Convert.ToString(dt.Rows[0]["Full_name"]);
                                                body = body.Replace("[name]", Convert.ToString(dt.Rows[0]["First_Name"])); //obj.Full_Name);
                                                body = body.Replace("[msg]", "Edit contact details request sent successfully.");

                                                subject = "Edit Contact Details Request";
                                                string newsubject = company_name + " - " + subject + Convert.ToString("");
                                                string msg = (string)CompanyInfo.Send_Mail(dtc, obj.UserName, body, newsubject, obj.Client_ID, obj.Branch_ID, "", "", "", context);

                                            }
                                        }
                                    }
                                }
                            }

                        }
                        catch (Exception _x)
                        {

                            throw _x;
                        }
                        finally
                        {
                            con.Close();
                        }
                        // obj.Message = "success";
                    }
                }
                else
                {
                    string msg = "SQl Enjection detected";

                    // Activity = "<b>" + Username + "</b>" + " Password changed.  </br>";
                    //int stattus = (int)CompanyInfo.InsertActivityLogDetails(msg, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID),obj.Id, "IsValidCustomer", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "IsValidCustomer");
                    int stattus = (int)CompanyInfo.InsertActivityLogDetailsSecurity(msg + " " + enjected_data, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), 1, "srvcustomer", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "Update_Phone", 0, context);

                }
            }

            else
            {
                obj.Id = "2";
                string msg = "Validation Failed <br/> " + error_invalid_data;
                obj.Message = "Validation Failed";
                obj.Comment = error_msg;
                int stattus = (int)CompanyInfo.InsertActivityLogDetails(msg, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), 1, "Update_Phone", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "Update_Phone", context);
            }
            return obj;
        }

        public DataTable GetTransactionSearch(string txnReferenceNo)
        {
            DataTable dt = new DataTable();
            try
            {
                string whereClause = string.Empty;
                MySqlCommand _cmd = new MySqlCommand();

                if (!string.IsNullOrEmpty(txnReferenceNo))
                {
                    whereClause += " and ReferenceNo like '%" + txnReferenceNo + "%'";
                }

                using (_cmd = new MySqlCommand("View_transfers"))
                {
                    _cmd.CommandType = CommandType.StoredProcedure;
                    _cmd.Parameters.AddWithValue("_whereclause", whereClause);
                    dt = db_connection.ExecuteQueryDataTableProcedure(_cmd);
                }



                if (dt.Rows.Count > 0)
                {
                    DataTable dt_cmp = new DataTable();
                    using (_cmd = new MySqlCommand("Get_CompanyInfo"))
                    {
                        _cmd.CommandType = CommandType.StoredProcedure;
                        _cmd.Parameters.AddWithValue("_Client_ID", dt.Rows[0]["Client_ID"]);
                        _cmd.Parameters.AddWithValue("_Customer_ID", "0");
                        _cmd.Parameters.AddWithValue("_SecurityKey", CompanyInfo.SecurityKey());
                        dt_cmp = db_connection.ExecuteQueryDataTableProcedure(_cmd);

                        if (dt_cmp.Rows.Count > 0)
                        {
                            dt.Columns.Add("Image").SetOrdinal(0);
                            dt.Rows[0]["Image"] = dt_cmp.Rows[0]["Image"];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex; // Consider logging instead of just re-throwing
            }
            dt.Columns.Remove("Customer_ID");
            dt.Columns.Remove("Beneficiary_ID");
            return dt;
        }
        public string updateCustTxnSign(Model.Transaction obj)
        {
            string msg = string.Empty;
            DataTable ds = new DataTable();
            MySqlCommand cmd = new MySqlCommand();
            try
            {
                DataTable dt_cmp = new DataTable();
                using (cmd = new MySqlCommand("Get_CompanyInfo"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                    cmd.Parameters.AddWithValue("_Customer_ID", "0");
                    cmd.Parameters.AddWithValue("_SecurityKey", CompanyInfo.SecurityKey());
                    dt_cmp = db_connection.ExecuteQueryDataTableProcedure(cmd);
                }
                //string encryptData = obj.TransactionReference;
                //string decryptData = CompanyInfo.Decrypt(encryptData, Convert.ToBoolean(1));

                //string ReferenceNo = decryptData.Split(' ')[0];
                string signImg = Convert.ToString(obj.Image);
                string transactionId = Convert.ToString(obj.Transaction_ID);
                string custId = Convert.ToString(obj.Customer_ID);

                string FileName = obj.TransactionReference + "Signature.png";
                string FileNameWithExt = Convert.ToString(dt_cmp.Rows[0]["RootURL"]) + "/assets/Transaction_signs/" + FileName;
                File.WriteAllBytes(FileNameWithExt, Convert.FromBase64String(signImg.Replace("data:image/png;base64,", "")));
                string FileToSave = "assets/Transaction_signs/" + FileName;

                int i = 0;
                using (cmd = new MySqlCommand("updateCustTxnSign"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("_Tran_sign", FileToSave);
                    cmd.Parameters.AddWithValue("_ReferenceNo", obj.TransactionReference);
                    i = db_connection.ExecuteNonQueryProcedure(cmd);
                }
                if (i > 0)
                {
                    msg = "success";
                }
                else
                {
                    msg = "failed";
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return msg;
        }

        public List<Model.TermsAndConditions> GetAllTermsAndConditions()
        {
            List<Model.TermsAndConditions> termsList = new List<Model.TermsAndConditions>();
            try
            {
                using (MySqlConnection con = new MySqlConnection(_MTS.WebConnSetting()))
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand("GetAllTermsAndConditions", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                // Read the blob as bytes
                                byte[] blobData = (byte[])reader["TermsCondition"];

                                // Convert to string if it's meant to be textual data
                                string termsConditionText = Encoding.UTF8.GetString(blobData);

                                // Added by Parth on 16/05/2025 To Replace all occurrences of /' with '
                                termsConditionText = termsConditionText.Replace("/'", "'");

                                termsList.Add(new Model.TermsAndConditions
                                {
                                    Term_ID = reader.GetInt32("Term_ID"),
                                    Term = reader.GetString("Term"),
                                    TermsCondition = termsConditionText
                                });
                            }
                        }
                    }
                    con.Close();
                }
            }
            catch (Exception ex) { }
            finally { }
            return termsList;
        }

        public Model.Customer UpdatePhone(Model.Customer obj, HttpContext context)
        {
            int Customer_ID = Convert.ToInt32(CompanyInfo.Decrypt(Convert.ToString(obj.Id), true));
            string Activity = string.Empty; string error_msg = ""; string error_invalid_data = ""; string enjected_data = ""; string notification_icon = ""; string notification_message = "";
            //var context = System.Web.HttpContext.Current;
            string Username = obj.Email; //Convert.ToString(context.Request.Form["Username"]);
            string MobileNumber_regex = validation.validate(obj.MobileNumber, 1, 1, 1, 0, 1, 1, 1, 1, 1);
            string phoneNumber_regex = validation.validate(obj.PhoneNumber, 1, 1, 1, 0, 1, 1, 1, 1, 1);
            if (phoneNumber_regex != "false" && MobileNumber_regex != "false")
            {
                phoneNumber_regex = "true";
                MobileNumber_regex = "true";

            }

            if (MobileNumber_regex == "false")
            {
                error_msg = error_msg + " Mobile number Should be numeric without space";
                error_invalid_data = "Mobile number: " + obj.MobileNumber;
            }
            if (phoneNumber_regex == "false")
            {
                error_msg = error_msg + " Phone number Should be numeric without space";
                error_invalid_data = error_invalid_data + "Phone number: " + obj.PhoneNumber;
            }


            if (phoneNumber_regex != "false" && MobileNumber_regex != "false")
            {
                string PhoneNumber = CompanyInfo.testInjection(Convert.ToString(obj.PhoneNumber));
                string MobileNumber = CompanyInfo.testInjection(Convert.ToString(obj.MobileNumber));
                if (PhoneNumber == "1" && MobileNumber == "1")
                {
                    MySqlCommand cmdn = new MySqlCommand("customer_details_by_param");
                    cmdn.CommandType = CommandType.StoredProcedure;
                    string where = " and cr.Customer_ID != " + Customer_ID + " and REPLACE(cr.Mobile_Number,'+', '') like '" + obj.MobileNumber.Replace("+", "") + "' and cr.Client_ID=" + obj.Client_ID + "";
                    cmdn.Parameters.AddWithValue("_whereclause", where);
                    cmdn.Parameters.AddWithValue("_SecurityKey", CompanyInfo.SecurityKey());
                    DataTable dtt = db_connection.ExecuteQueryDataTableProcedure(cmdn);
                    if (dtt.Rows.Count > 0)
                    {
                        if (dtt.Rows[0]["Phone_Number"] != null && dtt.Rows[0]["Phone_Number"].ToString() != "")
                        {
                            obj.Comment = "Account with this mobile number already exists. Please Login or add another mobile number.";
                            obj.Message = "exist_mobile";
                            return obj;
                        }
                    }
                    mts_connection _MTS = new mts_connection();
                    using (MySqlConnection con = new MySqlConnection(_MTS.WebConnSetting()))
                    {
                        con.Open();
                        MySqlTransaction transaction;
                        transaction = con.BeginTransaction(IsolationLevel.ReadCommitted);

                        try
                        {
                            MySqlCommand cmdupdate1 = new MySqlCommand("Get_Permissions", con);
                            cmdupdate1.CommandType = CommandType.StoredProcedure;
                            cmdupdate1.Parameters.AddWithValue("Per_ID", 42);
                            cmdupdate1.Parameters.AddWithValue("ClientID", obj.Client_ID);
                            //cmdupdate1.ExecuteScalar();
                            obj.CommentUserId = 1;
                            // int abcd = Convert.ToInt32(cmdupdate1.Parameters["Status"].Value);
                            DataTable dt1 = db_connection.ExecuteQueryDataTableProcedure(cmdupdate1); //Convert.ToInt32(cmdupdate1.Parameters["Status"].Value);
                            obj.per_status = Convert.ToInt32(dt1.Rows[0]["Status_ForCustomer"]);
                            // var per=0;
                            if (obj.per_status == 1)
                            {
                                using (MySqlCommand cmd = new MySqlCommand("update_phone_number", con))
                                {
                                    cmd.CommandType = CommandType.StoredProcedure;
                                    //obj.HouseNumber = "cc";
                                    //obj.Street = "23";
                                    cmd.Parameters.AddWithValue("_Phone_Number", obj.PhoneNumber);
                                    cmd.Parameters.AddWithValue("_Mobile_Number", obj.MobileNumber);
                                    if (obj.Mobile_number_code != "0" && obj.Mobile_number_code != null)
                                    {
                                        cmd.Parameters.AddWithValue("_Mobile_number_code", obj.Mobile_number_code);
                                    }
                                    else
                                    {
                                        cmd.Parameters.AddWithValue("_Mobile_number_code", null);
                                    }


                                    if (obj.Phone_number_code != "0" && obj.Phone_number_code != null)
                                    {
                                        cmd.Parameters.AddWithValue("_Phone_number_code", obj.Phone_number_code);
                                    }
                                    else
                                    {
                                        cmd.Parameters.AddWithValue("_Phone_number_code", null);
                                    }
                                    cmd.Parameters.AddWithValue("_Id", Customer_ID);
                                    cmd.Parameters.AddWithValue("_Branch_ID", obj.Branch_ID);
                                    cmd.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                                    cmd.Transaction = transaction;
                                    int n = cmd.ExecuteNonQuery();
                                    Model.Customer _ObjCustomer = new Model.Customer();
                                    // Service.srvCustomer srvCustomer = new Service.srvCustomer();
                                    // response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_SUCCESS);
                                    // _ObjCustomer = srvCustomer.UpdatePhone(obj);
                                    Service.srvLogin srvLogin = new Service.srvLogin();
                                    DataTable dt = new DataTable();
                                    Model.Login objLogin = new Model.Login();
                                    Model.Customer objcust = new Model.Customer();


                                    objLogin.Client_ID = obj.Client_ID;
                                    MySqlCommand cmd1 = new MySqlCommand("GetCustDetailsByID", con);
                                    cmd1.CommandType = CommandType.StoredProcedure;
                                    cmd1.Parameters.AddWithValue("cust_ID", Customer_ID);

                                    DataTable dt_cust = db_connection.ExecuteQueryDataTableProcedure(cmd1);
                                    objLogin.UserName = Convert.ToString(dt_cust.Rows[0]["Email_ID"]);
                                    dt = srvLogin.IsValidEmail(objLogin);
                                    obj.Email = Convert.ToString(dt_cust.Rows[0]["Email_ID"]);
                                    obj.RecordDate = Convert.ToDateTime(CompanyInfo.gettime(obj.Client_ID, Customer_ID.ToString(), 0, context));
                                    if (n > 0)
                                    {
                                        int submodule_id = 0;
                                        if ((obj.chk_changed).ToLower() == "phone")
                                        {
                                            submodule_id = 55;
                                        }
                                        if ((obj.chk_changed).ToLower() == "mobile")
                                        {
                                            submodule_id = 56;
                                        }
                                        if ((obj.chk_changed).ToLower() == "mobile and phone")
                                        {
                                            submodule_id = 57;
                                        }

                                        try
                                        {
                                            DataTable dt_notif = CompanyInfo.set_notification_data(submodule_id);
                                            if (dt_notif.Rows.Count > 0)
                                            {
                                                int SMS = Convert.ToInt32(dt_notif.Rows[0]["SMS"]);
                                                int Email = Convert.ToInt32(dt_notif.Rows[0]["Email"]);
                                                int Notif_status = Convert.ToInt32(dt_notif.Rows[0]["Notification"]);
                                                string notification_msg = Convert.ToString(dt_notif.Rows[0]["notification_msg"]);
                                                int i = CompanyInfo.check_notification_perm(Convert.ToString(Customer_ID), obj.Client_ID, obj.Branch_ID, 1, 42, Convert.ToDateTime(obj.RecordDate), 1, SMS, Email, Notif_status, "App - Update Phone Notification - " + submodule_id + "", notification_msg, context);
                                            }
                                        }
                                        catch (Exception ex) { }
                                        notification_icon = "mobi-changed.jpg";
                                        //notification_message = "<span class='cls-admin'><strong class='cls-mob'>Phone number</strong> changed successfully.</span><span class='cls-customer'><strong>Phone number changed </strong><span>Your have changed your phone number changed successfully</span>.</span>";
                                        if (obj.chk_changed == "Mobile and Phone")
                                        {
                                            notification_message = "<span class='cls-admin'><strong class='cls-mob'>" + obj.chk_changed + " number</strong> changed successfully.</span><span class='cls-customer'><strong>Contact numbers changed </strong><span>Your have changed your " + (obj.chk_changed).ToLower() + " number changed successfully</span>.</span>";
                                        }
                                        else
                                        {
                                            notification_message = "<span class='cls-admin'><strong class='cls-mob'>" + obj.chk_changed + " number</strong> changed successfully.</span><span class='cls-customer'><strong>" + obj.chk_changed + " number changed </strong><span>Your have changed your " + (obj.chk_changed).ToLower() + " number changed successfully</span>.</span>";
                                        }
                                        CompanyInfo.save_notification(notification_message, notification_icon, Convert.ToInt32(Customer_ID), obj.RecordDate, Convert.ToInt32(obj.Client_ID), 1, 0, Convert.ToInt32(obj.Branch_ID), 0, 0, 1, 0, context);
                                        Activity = "<b>" + Username + "</b>" + "phone number changed.  </br>";
                                        int stattus = (int)CompanyInfo.InsertActivityLogDetails(Activity, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(obj.Customer_ID), "UpdatePhone", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "Update Phone", context);


                                        string email = obj.Email;
                                        string body = string.Empty, subject = string.Empty;
                                        string body1 = string.Empty;
                                        //string template = "";
                                        System.Net.HttpWebRequest httpRequest = null, httpRequest1 = null;
                                        DataTable dtc = CompanyInfo.get(obj.Client_ID, context);
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
                                            body = body.Replace("[msg]", "Phone number updated successfully.");

                                            httpRequest1 = (HttpWebRequest)WebRequest.Create(URL + "Email/my-account.txt");
                                            httpRequest1.UserAgent = "Code Sample Web Client";
                                            HttpWebResponse webResponse1 = (HttpWebResponse)httpRequest1.GetResponse();
                                            using (StreamReader reader = new StreamReader(webResponse1.GetResponseStream()))
                                            {
                                                subject = reader.ReadLine();
                                            }
                                            string newsubject = company_name + " - " + subject + Convert.ToString("");
                                            string msg = (string)CompanyInfo.Send_Mail(dtc, objLogin.UserName, body, newsubject, obj.Client_ID, obj.Branch_ID, "", "", "", context);
                                            obj.Message = "success";
                                            obj.Comment = "Record updated.";
                                        }

                                    }
                                }
                            }
                            else if (obj.per_status == 0)
                            {
                                using (MySqlCommand cmd = new MySqlCommand("Customer_getallcustdetails", con))
                                {
                                    cmd.CommandType = CommandType.StoredProcedure;

                                    cmd.Parameters.AddWithValue("_Id", Customer_ID);
                                    var request = "";
                                    DataTable dt = db_connection.ExecuteQueryDataTableProcedure(cmd);
                                    if (dt.Rows.Count > 0)
                                    {
                                        if (dt.Rows[0]["Phone_Number"].Equals(obj.PhoneNumber))
                                        {
                                        }
                                        else
                                        {
                                            if (obj.PhoneNumber != "" && obj.PhoneNumber != null)
                                            {
                                                request += " Phone Number:" + obj.PhoneNumber + ",";
                                            }

                                        }
                                        if (!dt.Rows[0]["Mobile_Number"].Equals(obj.MobileNumber))
                                        {
                                            if (obj.MobileNumber != "" && obj.MobileNumber != null)
                                            {
                                                request += " Mobile Number:" + obj.MobileNumber;
                                            }

                                        }
                                        if (!dt.Rows[0]["Mobile_number_code"].Equals(obj.Mobile_number_code))
                                        {
                                            if (obj.Mobile_number_code != "0" && obj.Mobile_number_code != null)
                                            {
                                                request += " Mobile Number Country Code:" + obj.Mobile_number_code;
                                            }

                                        }
                                        if (!dt.Rows[0]["Phone_number_code"].Equals(obj.Phone_number_code))
                                        {
                                            if (obj.Phone_number_code != "0" && obj.Phone_number_code != null)
                                            {
                                                request += " Phone Number Country Code:" + obj.Phone_number_code;
                                            }

                                        }

                                    }
                                    string splitter = "<br/>";
                                    string subject_line = "Address Information";
                                    string final_subject_line = "Edit " + (obj.chk_changed).ToLower() + " number request sent.";
                                    obj.whereclause = "Request From Customer App : Old Phone No: <b>" + dt.Rows[0]["Phone_Number"] +
                                        "</b> Old Mobile No: <b>" + dt.Rows[0]["Mobile_Number"] + "</b>" +
                                        "<br/>Request Details :" + request.TrimEnd();
                                    obj.Comment = final_subject_line;
                                    obj.AllCustomer_Flag = "1";//response flag
                                    obj.Delete_Status = 0;
                                    obj.cashcollection_flag = "1";//active status----default status is pending
                                    obj.Record_Insert_DateTime = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
                                    obj.RecordDate = Convert.ToDateTime(CompanyInfo.gettime(obj.Client_ID, Customer_ID.ToString(), 0, context));
                                    obj.Flag = 2;//editrequest flag
                                    using (MySqlCommand cmd1 = new MySqlCommand("Edit_Request", con))
                                    {
                                        cmd1.CommandType = CommandType.StoredProcedure;
                                        //obj.HouseNumber = "cc";
                                        //obj.Street = "23";

                                        cmd1.Parameters.AddWithValue("_Id", Customer_ID);
                                        cmd1.Parameters.AddWithValue("_Reason", obj.whereclause);
                                        cmd1.Parameters.AddWithValue("_AllCustomer_Flag", obj.AllCustomer_Flag);

                                        cmd1.Parameters.AddWithValue("_Delete_Status", obj.Delete_Status);
                                        cmd1.Parameters.AddWithValue("_cashcollection_flag", obj.cashcollection_flag);
                                        cmd1.Parameters.AddWithValue("_Record_Insert_DateTime", obj.RecordDate);
                                        cmd1.Parameters.AddWithValue("_Branch_ID", obj.Branch_ID);
                                        cmd1.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                                        cmd1.Parameters.AddWithValue("_comments", obj.Comment);
                                        cmd1.Parameters.AddWithValue("_flag", obj.Flag);
                                        cmd1.Transaction = transaction;
                                        int n = cmd1.ExecuteNonQuery();
                                        obj.Message = "Request";
                                        if (n > 0)
                                        {
                                            int submodule_id = 0;
                                            if ((obj.chk_changed).ToLower() == "phone")
                                            {
                                                submodule_id = 45;
                                            }
                                            if ((obj.chk_changed).ToLower() == "mobile")
                                            {
                                                submodule_id = 48;
                                            }
                                            if ((obj.chk_changed).ToLower() == "mobile and phone")
                                            {
                                                submodule_id = 51;
                                            }
                                            try
                                            {
                                                DataTable dt_notif = CompanyInfo.set_notification_data(submodule_id);
                                                if (dt_notif.Rows.Count > 0)
                                                {
                                                    int SMS = Convert.ToInt32(dt_notif.Rows[0]["SMS"]);
                                                    int Email = Convert.ToInt32(dt_notif.Rows[0]["Email"]);
                                                    int Notif_status = Convert.ToInt32(dt_notif.Rows[0]["Notification"]);
                                                    string notification_msg = Convert.ToString(dt_notif.Rows[0]["notification_msg"]);
                                                    int i = CompanyInfo.check_notification_perm(Convert.ToString(Customer_ID), obj.Client_ID, obj.Branch_ID, 1, submodule_id, Convert.ToDateTime(obj.Record_Insert_DateTime), 1, SMS, Email, Notif_status, "App - Update Phone Notification - " + submodule_id + "", notification_msg, context);
                                                }
                                            }
                                            catch (Exception ex) { }
                                            notification_icon = "mobile-no-change.jpg";
                                            if (obj.chk_changed == "Mobile and Phone")
                                            {
                                                notification_message = "<span class='cls-admin'>sent Request for <strong class='cls-mob'>Change of " + (obj.chk_changed).ToLower() + " number</strong>. </span><span class='cls-customer'><strong>Contact number change request</strong><span>Your " + (obj.chk_changed) + " number change request sent successfully.</span></span>";
                                            }
                                            else
                                            {
                                                notification_message = "<span class='cls-admin'>sent Request for <strong class='cls-mob'>Change of " + (obj.chk_changed).ToLower() + " number</strong>. </span><span class='cls-customer'><strong>" + (obj.chk_changed) + " number change request</strong><span>Your " + (obj.chk_changed) + " number change request sent successfully.</span></span>";
                                            }
                                            CompanyInfo.save_notification(notification_message, notification_icon, Convert.ToInt32(Customer_ID), Convert.ToDateTime(obj.Record_Insert_DateTime), Convert.ToInt32(obj.Client_ID), 1, 0, Convert.ToInt32(obj.Branch_ID), 0, 0, 1, 0, context);

                                            Activity = "<b>" + Username + "</b>" + " changed contact details. " + request + "</br>";
                                            obj.User_ID = 1;
                                            var abc = Customer_ID;
                                            int stattus = (int)CompanyInfo.InsertActivityLogDetails(Activity, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(Customer_ID), "UpdatePhone", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "Update Phone", context);
                                            // int stattus = (int)CompanyInfo.InsertActivityLogDetails(Activity, 1, 0, 1, 1, "UpdatePhone", 1, 1, "Update Phone");
                                            string email = obj.Email;
                                            string body = string.Empty, subject = string.Empty;
                                            string body1 = string.Empty;
                                            //string template = "";
                                            System.Net.HttpWebRequest httpRequest = null, httpRequest1 = null;
                                            DataTable dtc = CompanyInfo.get(obj.Client_ID, context);
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
                                                obj.Full_Name = Convert.ToString(dt.Rows[0]["Full_name"]);
                                                body = body.Replace("[name]", Convert.ToString(dt.Rows[0]["First_Name"])); //obj.Full_Name);
                                                body = body.Replace("[msg]", "Edit contact details request sent successfully.");

                                                //httpRequest1 = (HttpWebRequest)WebRequest.Create(URL + "Email/my-account.txt");
                                                //httpRequest1.UserAgent = "Code Sample Web Client";
                                                //HttpWebResponse webResponse1 = (HttpWebResponse)httpRequest1.GetResponse();
                                                //using (StreamReader reader = new StreamReader(webResponse1.GetResponseStream()))
                                                //{
                                                //    subject = reader.ReadLine();
                                                //}
                                                subject = "Edit Contact Details Request";
                                                string newsubject = company_name + " - " + subject + Convert.ToString("");
                                                string msg = (string)CompanyInfo.Send_Mail(dtc, obj.UserName, body, newsubject, obj.Client_ID, obj.Branch_ID, "", "", "", context);

                                            }
                                        }
                                    }
                                }
                            }

                            transaction.Commit();
                        }
                        catch (Exception _x)
                        {
                            int stattus = (int)CompanyInfo.InsertActivityLogDetailsSecurity("Update_Phone Error:" + _x.ToString(), Convert.ToInt32(0), 0, Convert.ToInt32(0), 1, "srvcustomer", Convert.ToInt32(0), Convert.ToInt32(0), "Update_Phone", 0, context);
                            CompanyInfo.InsertrequestLogTracker("Update_Phone Error:" + _x.ToString(), 0, 0, 0, 0, "updatecontactdetails", Convert.ToInt32(0), Convert.ToInt32(0), "", context);
                            transaction.Rollback();
                            throw _x;
                        }
                        finally
                        {
                            con.Close();
                        }
                        // obj.Message = "success";

                        //}
                    }
                }
                else
                {
                    string msg = "SQl Enjection detected";

                    // Activity = "<b>" + Username + "</b>" + " Password changed.  </br>";
                    //int stattus = (int)CompanyInfo.InsertActivityLogDetails(msg, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID),obj.Id, "IsValidCustomer", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "IsValidCustomer");
                    int stattus = (int)CompanyInfo.InsertActivityLogDetailsSecurity(msg, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), 1, "srvcustomer", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "Update_Phone", 0, context);

                }
            }
            else
            {
                obj.Id = "0";
                string msg = "Validation Failed <br/> " + error_invalid_data;
                obj.Message = "Validation Failed";
                obj.Comment = error_msg;
                int stattus = (int)CompanyInfo.InsertActivityLogDetails(msg, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), 1, "Update_Phone", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "Update_Phone", context);

            }
            return obj;
        }

        public Model.Customer Verify_Email(Model.Customer obj, HttpContext context)
        {
            string Activity = string.Empty;
            string notification_icon = ""; string notification_message = "";
            string Username = obj.UserName;

            mts_connection _MTS = new mts_connection();
            string from_querystring = Convert.ToString(obj.WireTransfer_ReferanceNo);
            string split_string = "", ref_no = "";
            if (from_querystring != "" && from_querystring != null)
            {
                split_string = from_querystring.Replace("reference=", "");
                //ref_no = CompanyInfo.Decrypt(split_string, Convert.ToBoolean(1));
                ref_no = CompanyInfo.Decrypt1(split_string);
            }
            obj.WireTransfer_ReferanceNo = ref_no;
            obj.Record_Insert_DateTime = CompanyInfo.gettime(obj.Client_ID, "0", 0, context);
            MySqlCommand cmdn = new MySqlCommand("customer_details_by_param");
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
                    try
                    {
                        obj.CommentUserId = 1;
                        obj.SecurityKey = srvCommon.SecurityKey();
                        using (MySqlCommand cmd = new MySqlCommand("verify_email"))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("_Id", obj.Id);
                            cmd.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                            //cmd.Parameters.Add(new MySqlParameter("_verified", MySqlDbType.Int32));
                            //cmd.Parameters["_verified"].Direction = ParameterDirection.Output;
                            int already_verified = db_connection.ExecuteNonQueryProcedure(cmd);
                            //object ExistId = cmd.Parameters["_verified"].Value;
                            //ExistId = (ExistId == DBNull.Value) ? null : ExistId;
                            //int already_verified= Convert.ToInt32(ExistId);
                            if (already_verified == 1)
                            {
                                //return "exist_mobile";
                                obj.Message = "Success";
                                notification_icon = "verify-email.jpg";
                                notification_message = "<span class='cls-admin'><strong class='cls-verify'>Email ID</strong> is verified.</span><span class='cls-customer'><strong>Email ID verified</strong><span>Your email id has been verified</span></span>";
                                CompanyInfo.save_notification(notification_message, notification_icon, Convert.ToInt32(obj.Id), Convert.ToDateTime(obj.Record_Insert_DateTime), Convert.ToInt32(obj.Client_ID), 1, 0, Convert.ToInt32(obj.Branch_ID), 0, 0, 1, 0, context);
                                try
                                {
                                    DataTable dt_notif = CompanyInfo.set_notification_data(2);
                                    if (dt_notif.Rows.Count > 0)
                                    {
                                        int SMS = Convert.ToInt32(dt_notif.Rows[0]["SMS"]);
                                        int Email = Convert.ToInt32(dt_notif.Rows[0]["Email"]);
                                        int Notif_status = Convert.ToInt32(dt_notif.Rows[0]["Notification"]);
                                        string notification_msg = Convert.ToString(dt_notif.Rows[0]["notification_msg"]);
                                        int i = CompanyInfo.check_notification_perm(Convert.ToString(obj.Id), obj.Client_ID, obj.Branch_ID, 1, 2, Convert.ToDateTime(obj.RecordDate), 1, SMS, Email, Notif_status, "App - Email Verification Notification - 2", notification_msg, context);
                                    }
                                }
                                catch (Exception ex) { }

                            }
                            else
                            {
                                obj.Message = "verified";
                            }
                        }
                    }
                    catch (Exception _x)
                    {
                        obj.Message = "Failed " + _x;

                    }

                }
            }
            obj.SecurityKey = "";
            return obj;

        }
        public Model.Customer secure_Account(Model.Customer obj, HttpContext context)
        {
            string Activity = string.Empty;
            string Username = obj.UserName;

            mts_connection _MTS = new mts_connection();
            string from_querystring = Convert.ToString(obj.WireTransfer_ReferanceNo);
            string split_string = "", ref_no = "";
            if (from_querystring != "" && from_querystring != null)
            {
                split_string = from_querystring.Replace("reference=", "");
                //ref_no = CompanyInfo.Decrypt(split_string, Convert.ToBoolean(1));
                ref_no = CompanyInfo.Encrypt(split_string, true);

                ref_no = CompanyInfo.Decrypt(split_string, true);
            }
            obj.WireTransfer_ReferanceNo = ref_no;
            obj.Record_Insert_DateTime = CompanyInfo.gettime(obj.Client_ID, context);
            MySqlCommand cmdn = new MySqlCommand("customer_details_by_param");
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
                    try
                    {
                        obj.CommentUserId = 1;
                        obj.SecurityKey = srvCommon.SecurityKey();
                        using (MySqlCommand cmd = new MySqlCommand("secure_account"))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("_Id", obj.Id);
                            cmd.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                            //cmd.Parameters.Add(new MySqlParameter("_verified", MySqlDbType.Int32));
                            //cmd.Parameters["_verified"].Direction = ParameterDirection.Output;
                            int already_verified = db_connection.ExecuteNonQueryProcedure(cmd);
                            //object ExistId = cmd.Parameters["_verified"].Value;
                            //ExistId = (ExistId == DBNull.Value) ? null : ExistId;
                            //int already_verified= Convert.ToInt32(ExistId);
                            if (already_verified == 1)
                            {
                                //return "exist_mobile";
                                obj.Message = "Success";
                            }
                            else
                            {
                                obj.Message = "verified";
                            }
                        }
                    }
                    catch (Exception _x)
                    {
                        obj.Message = "Failed " + _x;

                    }

                }
            }
            return obj;

        }
    }
}
