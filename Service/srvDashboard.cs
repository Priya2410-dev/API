using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using MySqlConnector;
using System.Globalization;
using System.Xml;
using Newtonsoft.Json.Linq;

namespace Calyx_Solutions.Service
{
    public class srvDashboard
    {
        HttpContext context = null;

        public DataTable Info(Model.Dashboard obj)
        {
            int Customer_ID = Convert.ToInt32(CompanyInfo.Decrypt(obj.Customer_ID, true));
            string Client_ID_regex = validation.validate(Convert.ToString(obj.Client_ID), 0, 1, 1, 1, 1, 1, 1, 1, 1);
            string Customer_ID_regex = validation.validate(Convert.ToString(Customer_ID), 0, 1, 1, 1, 1, 1, 1, 1, 1);
            DataTable dt = new DataTable();
            if (Client_ID_regex != "false" && Customer_ID_regex != "false")
            {
                List<Model.Dashboard> _lst = new List<Model.Dashboard>();
                //var context =  System.Web.HttpContext.Current;
                //string Username = Convert.ToString(context.Request.Form["Branch_ID"]);
                string Username = "";//Convert.ToString(context.Request.Form["Branch_ID"]);
                MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("Dashboard_Details");//"select    cast(AES_DECRYPT(UNHEX(u.Password), '" + obj.SecurityKey.Trim() + "' ) as  char(500)) as Password,u.First_Name,u.Last_Name,u.Customer_ID,U.WireTransfer_ReferanceNo,u.Email_ID,u.Branch_ID,u.Client_ID from customer_registration u where    Email_ID = '" + obj.Name.Trim() + "' and Password=HEX(AES_ENCRYPT('" + obj.Password.Trim() + "','" + obj.SecurityKey.Trim() + "'))  and u.delete_status=0 ;");
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.Parameters.AddWithValue("_Customer_ID", Customer_ID);
                _cmd.Parameters.AddWithValue("_Client_ID", obj.Client_ID);

                dt = db_connection.ExecuteQueryDataTableProcedure(_cmd);
                //Model.Dashboard _obj = new Model.Dashboard();
                //if (dt != null && dt.Rows.Count > 0)
                //{
                //    _lst = CompanyInfo.ConvertDataTable<Model.Dashboard>(dt);
                //}
            }
            else
            {
                string msg = "Validation Error Client_ID_regex- +" + Client_ID_regex + "Customer_ID_regex- +" + Customer_ID_regex;
                int stattus = (int)CompanyInfo.InsertActivityLogDetailsSecurity(msg, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), 1, "srvDiscount", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "create", 0, context);
            }
            return dt;
        }
        public DataTable Viewnotifications(Model.Dashboard obj)
        {
            int Customer_ID = Convert.ToInt32(CompanyInfo.Decrypt(obj.Customer_ID, true));
            List<Model.Dashboard> _lst = new List<Model.Dashboard>();
            MySqlConnector.MySqlCommand cmd = new MySqlConnector.MySqlCommand("Get_DashboardNotifications");
            cmd.CommandType = CommandType.StoredProcedure;
            string _whereclause = "";
            string Customer_ID_regex = validation.validate(Convert.ToString(Customer_ID), 0, 1, 1, 1, 1, 1, 1, 1, 1);
            string Client_ID_regex = validation.validate(Convert.ToString(obj.Client_ID), 0, 1, 1, 1, 1, 1, 1, 1, 1);
            string Branch_ID_regex = validation.validate(Convert.ToString(obj.Client_ID), 0, 1, 1, 1, 1, 1, 1, 1, 1);
            DataTable ds = new DataTable();
            if (Customer_ID_regex != "false" && Client_ID_regex != "false" && Branch_ID_regex != "false")
            {
                //_whereclause = _whereclause + " date(Record_Insert_DateTime) between '" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "'  and  '" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "' ";
                if (obj.flag == 0)
                {
                    DateTime GetFrom_Date = DateTime.ParseExact(obj.date, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    string date = GetFrom_Date.ToString("yyyy-MM-dd");
                    _whereclause = _whereclause + " notification.Customer_ID=" + Customer_ID + " and notification.client_id= " + obj.Client_ID + " and show_notificationfor_customer=0 and date(notification.Record_Insert_DateTime) between '" + date + "' and '" + DateTime.Now.ToString("yyyy-MM-dd") + "' and show_notificationfor_customer=0 and notification.Read_Count_Customer=1  order by notification.Record_Insert_DateTime desc limit 5";
                }
                else
                {

                    DateTime GetFrom_Date = DateTime.ParseExact(obj.date, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    string date = GetFrom_Date.ToString("yyyy-MM-dd");

                    _whereclause = _whereclause + " notification.Customer_ID=" + Customer_ID + " and  notification.client_id= " + obj.Client_ID + " and show_notificationfor_customer=0 and date(notification.Record_Insert_DateTime) between '" + date + "' and '" + DateTime.Now.ToString("yyyy-MM-dd") + "'  order by notification.Record_Insert_DateTime desc,ID";
                }
                cmd.Parameters.AddWithValue("_Branch_ID", obj.Branch_ID);

                cmd.Parameters.AddWithValue("_whereclause", _whereclause);
                ds = db_connection.ExecuteQueryDataTableProcedure(cmd);
            }
            else
            {
                string msg = "Validation Error Customer_ID_regex- " + Customer_ID_regex + " Client_ID_regex- " + Client_ID_regex + "Branch_ID_regex- " + Branch_ID_regex;
                int stattus = (int)CompanyInfo.InsertActivityLogDetailsSecurity(msg, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), 1, "srvDashboard", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "Viewnotifications", 0, context);
            }
            return ds;
        }

        public DataTable Update_dashboard_readcount(Model.Dashboard obj)
        {
            string msg = string.Empty;
            DataTable ds = new DataTable();

            ds.Columns.Add("status", typeof(string));
            List<Model.Dashboard> _lst = new List<Model.Dashboard>();
            MySqlConnector.MySqlCommand cmd = new MySqlConnector.MySqlCommand("Update_dashboard_readcount_customer");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("_Id", obj.flag);

            int dss = db_connection.ExecuteNonQueryProcedure(cmd);
            if (dss > 0)
            {
                msg = "Success";
                ds.Rows.Add(msg);
            }

            return ds;

        }

        public DataTable View_push_notifications_old(Model.Dashboard obj)
        {
            HttpContext contextg = null;
            int stattusfilef = (int)CompanyInfo.InsertActivityLogDetailsSecurity("ReplaceLetters start", Convert.ToInt32(0), 0, Convert.ToInt32(0), 1, "srvDashboard", Convert.ToInt32(0), Convert.ToInt32(0), "Viewnotifications", 0, contextg);
            
            int Customer_ID = Convert.ToInt32(CompanyInfo.Decrypt(obj.Customer_ID, true));
            List<Model.Dashboard> _lst = new List<Model.Dashboard>();
            MySqlConnector.MySqlCommand cmd = new MySqlConnector.MySqlCommand("FetchNotifications");
            cmd.CommandType = CommandType.StoredProcedure;
            
            string Customer_ID_regex = validation.validate(Convert.ToString(Customer_ID), 0, 1, 1, 1, 1, 1, 1, 1, 1);
            string Client_ID_regex = validation.validate(Convert.ToString(obj.Client_ID), 0, 1, 1, 1, 1, 1, 1, 1, 1);
            string Branch_ID_regex = validation.validate(Convert.ToString(obj.Client_ID), 0, 1, 1, 1, 1, 1, 1, 1, 1);
            DataTable ds = new DataTable();
            stattusfilef = (int)CompanyInfo.InsertActivityLogDetailsSecurity("ReplaceLetters file: step 0", Convert.ToInt32(0), 0, Convert.ToInt32(0), 1, "srvDashboard", Convert.ToInt32(0), Convert.ToInt32(0), "Viewnotifications", 0, contextg);
            if (Customer_ID_regex != "false" && Client_ID_regex != "false" && Branch_ID_regex != "false")
            {
                ////_whereclause = _whereclause + " date(Record_Insert_DateTime) between '" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "'  and  '" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "' ";
                //if (obj.flag == 0)
                //{
                //    DateTime GetFrom_Date = DateTime.ParseExact(obj.date, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                //    string date = GetFrom_Date.ToString("yyyy-MM-dd");
                //    _whereclause = _whereclause + " notification.Customer_ID=" + Customer_ID + " and notification.client_id= " + obj.Client_ID + " and show_notificationfor_customer=0 and date(notification.Record_Insert_DateTime) between '" + date + "' and '" + DateTime.Now.ToString("yyyy-MM-dd") + "' and show_notificationfor_customer=0 and notification.Read_Count_Customer=1  order by notification.Record_Insert_DateTime desc limit 5";
                //}
                //else
                //{

                //    DateTime GetFrom_Date = DateTime.ParseExact(obj.date, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                //    string date = GetFrom_Date.ToString("yyyy-MM-dd");

                //    _whereclause = _whereclause + " notification.Customer_ID=" + Customer_ID + " and  notification.client_id= " + obj.Client_ID + " and show_notificationfor_customer=0 and date(notification.Record_Insert_DateTime) between '" + date + "' and '" + DateTime.Now.ToString("yyyy-MM-dd") + "'  order by notification.Record_Insert_DateTime desc,ID";
                //}
                //cmd.Parameters.AddWithValue("_Branch_ID", obj.Branch_ID);

                //cmd.Parameters.AddWithValue("_whereclause", _whereclause);
                  stattusfilef = (int)CompanyInfo.InsertActivityLogDetailsSecurity("ReplaceLetters file: step 1" , Convert.ToInt32(0), 0, Convert.ToInt32(0), 1, "srvDashboard", Convert.ToInt32(0), Convert.ToInt32(0), "Viewnotifications", 0, contextg);

                cmd.Parameters.AddWithValue("_customer_id", Customer_ID);
                ds = db_connection.ExecuteQueryDataTableProcedure(cmd);
                DataTable dtc = CompanyInfo.get(obj.Client_ID, context);
                DataTable xmldataTable = new DataTable();
                stattusfilef = (int)CompanyInfo.InsertActivityLogDetailsSecurity("ReplaceLetters file: step 2", Convert.ToInt32(0), 0, Convert.ToInt32(0), 1, "srvDashboard", Convert.ToInt32(0), Convert.ToInt32(0), "Viewnotifications", 0, contextg);
                string xmlFilePath = Convert.ToString(dtc.Rows[0]["RootURL"]) + "assets/Notification_key/ReplaceLetters.xml";
                int stattusfile = (int)CompanyInfo.InsertActivityLogDetailsSecurity("ReplaceLetters file:" + xmlFilePath, Convert.ToInt32(0), 0, Convert.ToInt32(0), 1, "srvDashboard", Convert.ToInt32(0), Convert.ToInt32(0), "Viewnotifications", 0, context);
                CompanyInfo.InsertrequestLogTracker("ReplaceLetters file:" + xmlFilePath, 0, 0, 0, 0, "View_push_notifications", Convert.ToInt32(0), Convert.ToInt32(0), "", contextg);
                if (File.Exists(xmlFilePath))
                {
                    CompanyInfo.InsertrequestLogTracker("ReplaceLetters file stp 2:" + xmlFilePath, 0, 0, 0, 0, "View_push_notifications", Convert.ToInt32(0), Convert.ToInt32(0), "", contextg);
                    stattusfile = (int)CompanyInfo.InsertActivityLogDetailsSecurity("ReplaceLetters :" + xmlFilePath, Convert.ToInt32(0), 0, Convert.ToInt32(0), 1, "srvDashboard", Convert.ToInt32(0), Convert.ToInt32(0), "Viewnotifications", 0, context);
                    //test_act += "step 2";

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

                    List<string> columnValues = new List<string>();
                    List<FirebaseAdmin.Messaging.Message> messages = new List<FirebaseAdmin.Messaging.Message>();
                    foreach (DataRow row in ds.Rows)
                    {
                        //string token = row["Token"].ToString();

                        string title = CompanyInfo.ConvertMessage(Convert.ToString(row["notification_title"]), xmldataTable, row);
                        string message1 = CompanyInfo.ConvertMessage(Convert.ToString(row["notification_msg"]), xmldataTable, row);

                        //var fcmMessage = new FirebaseAdmin.Messaging.Message
                        //{
                        //    //Token = token,
                        //    Notification = new Notification
                        //    {
                        //        Title = title,
                        //        Body = message1
                        //    }
                        //};

                        //messages.Add(fcmMessage);
                        row["notification_title"] = title;

                        int index = message1.IndexOf("countryid_");
                        if (index >= 0)
                            message1 = message1.Substring(0, index);

                        row["notification_msg"] = message1;                        
                    }
                }
            }
            else
            {
                string msg = "Validation Error Customer_ID_regex- " + Customer_ID_regex + " Client_ID_regex- " + Client_ID_regex + "Branch_ID_regex- " + Branch_ID_regex;
                int stattus = (int)CompanyInfo.InsertActivityLogDetailsSecurity(msg, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), 1, "srvDashboard", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "Viewnotifications", 0, context);
            }
            return ds;
        }

        public DataTable View_push_notifications(Model.Dashboard obj)
        {
            HttpContext contextg = null;
            DataTable ds = new DataTable();
            string Activity = "Inside the View_push_notifications";

            try
            {
                int Customer_ID = Convert.ToInt32(CompanyInfo.Decrypt(obj.Customer_ID, true));
                List<Model.Dashboard> _lst = new List<Model.Dashboard>();
                MySqlConnector.MySqlCommand cmd = new MySqlConnector.MySqlCommand("FetchNotifications");
                cmd.CommandType = CommandType.StoredProcedure;
                string _whereclause = "";
                string Customer_ID_regex = validation.validate(Convert.ToString(Customer_ID), 0, 1, 1, 1, 1, 1, 1, 1, 1);
                string Client_ID_regex = validation.validate(Convert.ToString(obj.Client_ID), 0, 1, 1, 1, 1, 1, 1, 1, 1);
                string Branch_ID_regex = validation.validate(Convert.ToString(obj.Client_ID), 0, 1, 1, 1, 1, 1, 1, 1, 1);
                if (Customer_ID_regex != "false" && Client_ID_regex != "false" && Branch_ID_regex != "false")
                {
                    Activity += "step 1";
                    ////_whereclause = _whereclause + " date(Record_Insert_DateTime) between '" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "'  and  '" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "' ";
                    //if (obj.flag == 0)
                    //{
                    //    DateTime GetFrom_Date = DateTime.ParseExact(obj.date, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    //    string date = GetFrom_Date.ToString("yyyy-MM-dd");
                    //    _whereclause = _whereclause + " notification.Customer_ID=" + Customer_ID + " and notification.client_id= " + obj.Client_ID + " and show_notificationfor_customer=0 and date(notification.Record_Insert_DateTime) between '" + date + "' and '" + DateTime.Now.ToString("yyyy-MM-dd") + "' and show_notificationfor_customer=0 and notification.Read_Count_Customer=1  order by notification.Record_Insert_DateTime desc limit 5";
                    //}
                    //else
                    //{

                    //    DateTime GetFrom_Date = DateTime.ParseExact(obj.date, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    //    string date = GetFrom_Date.ToString("yyyy-MM-dd");

                    //    _whereclause = _whereclause + " notification.Customer_ID=" + Customer_ID + " and  notification.client_id= " + obj.Client_ID + " and show_notificationfor_customer=0 and date(notification.Record_Insert_DateTime) between '" + date + "' and '" + DateTime.Now.ToString("yyyy-MM-dd") + "'  order by notification.Record_Insert_DateTime desc,ID";
                    //}
                    //cmd.Parameters.AddWithValue("_Branch_ID", obj.Branch_ID);

                    /*String dd = CompanyInfo.gettime(1, Convert.ToString(Customer_ID), 0,context);
                    cmd.Parameters.AddWithValue("_NowDateTime", dd);*/
                    String dd = CompanyInfo.gettime(1, context);
                    cmd.Parameters.AddWithValue("_NowDateTime", dd);


                    cmd.Parameters.AddWithValue("_customer_id", Customer_ID);
                    ds = db_connection.ExecuteQueryDataTableProcedure(cmd);
                    Activity += "step 2 ds " + ds.Rows.Count;
                    DataTable dtc = CompanyInfo.get(obj.Client_ID, contextg);
                    DataTable xmldataTable = new DataTable();
                    string xmlFilePath = Convert.ToString(dtc.Rows[0]["RootURL"]) + "assets/Notification_key/ReplaceLetters.xml";
                    if (File.Exists(xmlFilePath))
                    {
                        Activity += "step 3 ";
                        //test_act += "step 2";

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

                        List<string> columnValues = new List<string>();
                        List<FirebaseAdmin.Messaging.Message> messages = new List<FirebaseAdmin.Messaging.Message>();
                        foreach (DataRow row in ds.Rows)
                        {
                            //string token = row["Token"].ToString();

                            string title = CompanyInfo.ConvertMessage(Convert.ToString(row["notification_title"]), xmldataTable, row);
                            string message1 = CompanyInfo.ConvertMessage(Convert.ToString(row["notification_msg"]), xmldataTable, row);

                            //var fcmMessage = new FirebaseAdmin.Messaging.Message
                            //{
                            //    //Token = token,
                            //    Notification = new Notification
                            //    {
                            //        Title = title,
                            //        Body = message1
                            //    }
                            //};

                            //messages.Add(fcmMessage);
                            row["notification_title"] = title;
                            row["notification_msg"] = message1;
                        }
                    }
                }
                else
                {
                    string msg = "Validation Error Customer_ID_regex- " + Customer_ID_regex + " Client_ID_regex- " + Client_ID_regex + "Branch_ID_regex- " + Branch_ID_regex;
                    int stattus = (int)CompanyInfo.InsertActivityLogDetailsSecurity(msg, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), 1, "srvDashboard", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "Viewnotifications", 0,context);
                }
            }
            catch (Exception ex)
            {
                Activity += "step 4 " + ex.ToString();
            }
            finally
            {
                Activity += "step 5 ";
                int stattus = (int)CompanyInfo.InsertActivityLogDetails(Activity, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), 0, "View_push_notifications", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "View_push_notifications", context);

            }
            return ds;
        }


        public string UpdateNotificationStatus(Model.Dashboard obj)
        {
            string result = string.Empty;
            string Activity = string.Empty;
            try
            {
                int Customer_ID = Convert.ToInt32(CompanyInfo.Decrypt(obj.Customer_ID, true));
                MySqlConnector.MySqlCommand cmd = new MySqlConnector.MySqlCommand("sp_update_notification_status");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("_Notification_Id", obj.Id);
                cmd.Parameters.AddWithValue("_status", obj.status);
                cmd.Parameters.AddWithValue("_Notification_Flag", obj.Notification_Flag);

                //niranjan changes for delete system notification 22/11/2024 
                cmd.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                cmd.Parameters.AddWithValue("_Customer_ID", Customer_ID);
                cmd.Parameters.AddWithValue("_User_ID", obj.User_ID);
                cmd.Parameters.AddWithValue("_Deleted_By", obj.Deleted_By);

                DateTime record_Insert_Date = Convert.ToDateTime(DateTime.Now);
                cmd.Parameters.AddWithValue("_record_insert_date", record_Insert_Date);
                //upto here

                int n = db_connection.ExecuteNonQueryProcedure(cmd);
                if (n > 0)//success
                {
                    result = "success";
                    Activity = "Notification deleted successfully. Notification ID : " + obj.Id + " " + " Customer ID : " + Customer_ID + "";
                    int stattus = (int)CompanyInfo.InsertActivityLogDetails(Activity, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Customer_ID, "UpdateNotificationStatus", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "UpdateNotificationStatus", context);

                }
                else//failed
                {
                    result = "notsuccess";
                    Activity = "Failed to delete notification. Notification ID : " + obj.Id + " " + " Customer ID : " + Customer_ID + "";
                    int stattus = (int)CompanyInfo.InsertActivityLogDetails(Activity, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Customer_ID, "UpdateNotificationStatus", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "UpdateNotificationStatus", context);

                }

            }
            catch (Exception _x)
            {
                Model.ErrorLog objError = new Model.ErrorLog();
                objError.User = new Model.User();
                objError.Error = "UpdateNotificationStatus : " + _x.ToString();
                objError.Date = DateTime.Now;
                objError.User_ID = 1;
                objError.Client_ID = 1;
                objError.Function_Name = "UpdateNotificationStatus";
                Service.srvErrorLog srvError = new Service.srvErrorLog();
                srvError.Create(objError, context);
                result = "notsuccess";
                return result;
            }
            return result;
        }
    }
}
