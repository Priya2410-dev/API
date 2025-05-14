using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Net;
using System.IO;
using System.Net.Mail;
using Microsoft.AspNetCore.Mvc;

namespace Calyx_Solutions.Service
{
    public class srvPreference
    {
        public List<Model.Communication_Prefer> Select_Preference(Model.Communication_Prefer obj)
        {
            List<Model.Communication_Prefer> _lst = new List<Model.Communication_Prefer>();
            try
            {

                int Customer_ID = Convert.ToInt32(CompanyInfo.Decrypt(Convert.ToString(obj.Customer_ID), true));
               MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("select * from customer_prferences where Customer_ID=" + Customer_ID + " and Client_ID=" + obj.Client_ID + "");
                _cmd.CommandType = CommandType.Text;
                DataTable dt = db_connection.ExecuteQueryDataTableProcedure(_cmd);

                Model.Communication_Prefer _obj = new Model.Communication_Prefer();
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        _obj = new Model.Communication_Prefer();
                        if (row["CP_ID"] != DBNull.Value)
                        {
                            _obj.CP_ID = Convert.ToInt32(row["CP_ID"].ToString());
                        }
                        if (row["Customer_ID"] != DBNull.Value)
                        {
                            _obj.Customer_ID = Convert.ToString( Customer_ID);
                        }
                        if (row["Comm_preference_ID"] != DBNull.Value)
                        {
                            _obj.Comm_preference_ID = Convert.ToInt32(row["Comm_preference_ID"].ToString());
                        }
                        if (row["Comm_Preference_Status"] != DBNull.Value)
                        {
                            _obj.Comm_Preference_Status = Convert.ToInt32(row["Comm_Preference_Status"].ToString());
                        }
                        if (row["Client_ID"] != DBNull.Value)
                        {
                            _obj.Client_ID = Convert.ToInt32(row["Client_ID"]);
                        }
                        _lst.Add(_obj);
                    }
                }

            }
            catch (Exception _x)
            {
                //throw _x;
            }

            return _lst;
        }

        public string Update_Comm_Preference_new(Model.Communication_Prefer obj, HttpContext context)
        {
            int Customer_ID = Convert.ToInt32(CompanyInfo.Decrypt(Convert.ToString(obj.Customer_ID), true));
            string msg = ""; string notification_icon = ""; string notification_message = ""; string notification_icon2 = ""; string notification_message2 = ""; string notification_icon3 = ""; string notification_message3 = "";
            List<Model.Communication_Prefer> _lst = new List<Model.Communication_Prefer>();
            Model.Communication_Prefer _obj = new Model.Communication_Prefer();
            obj.Record_Insert_DateTime = Convert.ToDateTime(CompanyInfo.gettime(obj.Client_ID, context));
            List<Model.Communication_Prefer> _lstprefer = new List<Model.Communication_Prefer>();
            List<Model.Communication_Prefer> _lstPreferences = new List<Model.Communication_Prefer>();
            _lstprefer = Select_Preference(obj);
            string stastus = "";
            try
            {
                obj.Record_Insert_DateTime = Convert.ToDateTime(CompanyInfo.gettime(obj.Client_ID, context));
                MySqlConnector.MySqlCommand cmd = new MySqlConnector.MySqlCommand("sp_update_commPreference");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("_CP_ID", obj.Comm_preference_ID_Email);
                cmd.Parameters.AddWithValue("_Customer_ID", Customer_ID);
                cmd.Parameters.AddWithValue("_Comm_preference_ID", obj.CP_ID_Email);
                cmd.Parameters.AddWithValue("_Comm_Preference_Status", obj.Comm_Preference_Status_Email);
                cmd.Parameters.AddWithValue("_ClientID", obj.Client_ID);
                int x = db_connection.ExecuteNonQueryProcedure(cmd);
                if (x == 1)
                {
                    msg = "success";
                    string type = "";
                    notification_icon = "";
                    //obj.Comm_preference_ID_Email = obj.CommunicationPreference[0].PreferenceID;
                    //obj.Comm_Preference_Status_Email = obj.CommunicationPreference[0].PreferenceStatus;
                    int i = obj.Comm_preference_ID_Email;

                    type = "E-mail";

                    if (obj.Comm_Preference_Status_Email == 0)
                    {
                        notification_icon = "subscription.jpg";
                        notification_message = "<span class='cls-admin'>has <strong class='cls-subscription'>Subscribed</strong> for " + type + " notification.</span><span class='cls-customer'> <strong>Communication Subscription change</strong><span>You have subscribed to " + type + " notifications.</span></span>";
                    }
                    else
                    {
                        notification_icon = "unsubscription.jpg";
                        notification_message = "<span class='cls-admin'>has <strong class='cls-subscription'>Unubscribed</strong> for " + type + " notification.</span><span class='cls-customer'> <strong>Communication Unsubscription change</strong><span>You have unsubscribed to " + type + " notifications.</span></span>";
                    }

                }

                MySqlConnector.MySqlCommand cmd1 = new MySqlConnector.MySqlCommand("sp_update_commPreference");
                cmd1.CommandType = CommandType.StoredProcedure;
                cmd1.Parameters.AddWithValue("_CP_ID", obj.Comm_preference_ID_SMS);
                cmd1.Parameters.AddWithValue("_Customer_ID", Customer_ID);
                cmd1.Parameters.AddWithValue("_Comm_preference_ID", obj.CP_ID_SMS);
                cmd1.Parameters.AddWithValue("_Comm_Preference_Status", obj.Comm_Preference_Status_SMS);
                cmd1.Parameters.AddWithValue("_ClientID", obj.Client_ID);
                x = db_connection.ExecuteNonQueryProcedure(cmd1);
                if (x == 1)
                {
                    msg = "success";
                    string type = "";
                    notification_icon2 = "";

                    //obj.Comm_preference_ID_SMS = obj.CommunicationPreference[1].PreferenceID;
                    //obj.Comm_Preference_Status_SMS = obj.CommunicationPreference[1].PreferenceStatus;
                    int i = obj.Comm_preference_ID_SMS;

                    type = "SMS";

                    if (obj.Comm_Preference_Status_SMS == 0)
                    {
                        notification_icon2 = "subscription.jpg";
                        notification_message2 = "<span class='cls-admin'>has <strong class='cls-subscription'>Subscribed</strong> for " + type + " notification.</span><span class='cls-customer'> <strong>Communication Subscription change</strong><span>You have subscribed to " + type + " notifications.</span></span>";
                    }
                    else
                    {
                        notification_icon2 = "unsubscription.jpg";
                        notification_message2 = "<span class='cls-admin'>has <strong class='cls-subscription'>Unubscribed</strong> for " + type + " notification.</span><span class='cls-customer'> <strong>Communication Unsubscription change</strong><span>You have unsubscribed to " + type + " notifications.</span></span>";
                    }
                }
                stastus = notification_message;

                MySqlConnector.MySqlCommand cmd2 = new MySqlConnector.MySqlCommand("sp_update_commPreference");
                cmd2.CommandType = CommandType.StoredProcedure;
                cmd2.Parameters.AddWithValue("_CP_ID", obj.Comm_preference_ID_Phone);
                cmd2.Parameters.AddWithValue("_Customer_ID", Customer_ID);
                cmd2.Parameters.AddWithValue("_Comm_preference_ID", obj.CP_ID_Phone);
                cmd2.Parameters.AddWithValue("_Comm_Preference_Status", obj.Comm_Preference_Status_Phone);
                cmd2.Parameters.AddWithValue("_ClientID", obj.Client_ID);
                x = db_connection.ExecuteNonQueryProcedure(cmd2);
                if (x == 1)
                {
                    msg = "success";
                    string type = "";
                    notification_icon3 = "";
                    //obj.Comm_preference_ID_Phone = obj.CommunicationPreference[2].PreferenceID;
                    //obj.Comm_Preference_Status_Phone = obj.CommunicationPreference[2].PreferenceStatus;
                    int i = obj.Comm_preference_ID_Phone;

                    type = "Phone";

                    if (obj.Comm_Preference_Status_Phone == 0)
                    {
                        notification_icon3 = "subscription.jpg";
                        notification_message3 = "<span class='cls-admin'>has <strong class='cls-subscription'>Subscribed</strong> for " + type + " notification.</span><span class='cls-customer'> <strong>Communication Subscription change</strong><span>You have subscribed to " + type + " notifications.</span></span>";
                    }
                    else
                    {
                        notification_icon3 = "unsubscription.jpg";
                        notification_message3 = "<span class='cls-admin'>has <strong class='cls-subscription'>Unubscribed</strong> for " + type + " notification.</span><span class='cls-customer'> <strong>Communication Unsubscription change</strong><span>You have unsubscribed to " + type + " notifications.</span></span>";
                    }
                }

            }
            catch (Exception _x)
            {
                return _x.ToString();
            }
            string email_message = "Email: " + notification_message;
            string sms_message = "SMS: " + notification_message2;
            string phone_message = "Phone: " + notification_message3;

            // Concatenate the messages with new lines and spaces
            string full_message = $"{email_message}\n\n{sms_message}\n\n{phone_message}";


            // Return the full message as a JSON response
            return full_message;



        }

        public string Update_Comm_Preference(Model.Communication_Prefer obj,HttpContext context)
        {
            int Customer_ID = Convert.ToInt32(CompanyInfo.Decrypt(Convert.ToString(obj.Customer_ID), true));
            string msg = ""; string notification_icon = ""; string notification_message = ""; string notification_icon2 = ""; string notification_message2 = ""; string notification_icon3 = ""; string notification_message3 = "";
            List<Model.Communication_Prefer> _lst = new List<Model.Communication_Prefer>();
            Model.Communication_Prefer _obj = new Model.Communication_Prefer();
            obj.Record_Insert_DateTime = Convert.ToDateTime(CompanyInfo.gettime(obj.Client_ID, context));
            List<Model.Communication_Prefer> _lstprefer = new List<Model.Communication_Prefer>();
            List<Model.Communication_Prefer> _lstPreferences = new List<Model.Communication_Prefer>();
            _lstprefer = Select_Preference(obj);
            string stastus = "";
            try
            {
                    obj.Record_Insert_DateTime = Convert.ToDateTime(CompanyInfo.gettime(obj.Client_ID, context));
                    MySqlConnector.MySqlCommand cmd = new MySqlConnector.MySqlCommand("sp_update_commPreference");
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("_CP_ID", obj.CommunicationPreference[0].CpID);
                    cmd.Parameters.AddWithValue("_Customer_ID", Customer_ID);
                    cmd.Parameters.AddWithValue("_Comm_preference_ID", obj.CommunicationPreference[0].PreferenceID);
                    cmd.Parameters.AddWithValue("_Comm_Preference_Status", obj.CommunicationPreference[0].PreferenceStatus);
                    cmd.Parameters.AddWithValue("_ClientID", obj.Client_ID);
                    int x = db_connection.ExecuteNonQueryProcedure(cmd);
                    if (x == 1)
                    {
                        msg = "success";
                        string type = "";
                        notification_icon = "";
                        obj.Comm_preference_ID_Email = obj.CommunicationPreference[0].PreferenceID;
                        obj.Comm_Preference_Status_Email = obj.CommunicationPreference[0].PreferenceStatus;
                        int i = obj.Comm_preference_ID_Email;

                            type = "E-mail";

                            if (obj.Comm_Preference_Status_Email == 0)
                            {
                                notification_icon = "subscription.jpg";
                                notification_message = "<span class='cls-admin'>has <strong class='cls-subscription'>Subscribed</strong> for " + type + " notification.</span><span class='cls-customer'> <strong>Communication Subscription change</strong><span>You have subscribed to " + type + " notifications.</span></span>";
                            }
                            else
                            {
                                notification_icon = "unsubscription.jpg";
                                notification_message = "<span class='cls-admin'>has <strong class='cls-subscription'>Unubscribed</strong> for " + type + " notification.</span><span class='cls-customer'> <strong>Communication Unsubscription change</strong><span>You have unsubscribed to " + type + " notifications.</span></span>";
                            }
                        
                    }

                    MySqlConnector.MySqlCommand cmd1 = new MySqlConnector.MySqlCommand("sp_update_commPreference");
                    cmd1.CommandType = CommandType.StoredProcedure;
                    cmd1.Parameters.AddWithValue("_CP_ID", obj.CommunicationPreference[1].CpID);
                    cmd1.Parameters.AddWithValue("_Customer_ID", Customer_ID);
                    cmd1.Parameters.AddWithValue("_Comm_preference_ID", obj.CommunicationPreference[1].PreferenceID);
                    cmd1.Parameters.AddWithValue("_Comm_Preference_Status", obj.CommunicationPreference[1].PreferenceStatus);
                    cmd1.Parameters.AddWithValue("_ClientID", obj.Client_ID);
                     x = db_connection.ExecuteNonQueryProcedure(cmd1);
                    if (x == 1)
                    {
                        msg = "success";
                        string type = "";
                        notification_icon2 = "";

                    obj.Comm_preference_ID_SMS = obj.CommunicationPreference[1].PreferenceID;
                    obj.Comm_Preference_Status_SMS = obj.CommunicationPreference[1].PreferenceStatus;
                        int i = obj.Comm_preference_ID_SMS;
                       
                            type = "SMS";

                            if (obj.Comm_Preference_Status_SMS == 0)
                            {
                                notification_icon2 = "subscription.jpg";
                                notification_message2 = "<span class='cls-admin'>has <strong class='cls-subscription'>Subscribed</strong> for " + type + " notification.</span><span class='cls-customer'> <strong>Communication Subscription change</strong><span>You have subscribed to " + type + " notifications.</span></span>";
                            }
                            else
                            {
                                notification_icon2 = "unsubscription.jpg";
                                notification_message2 = "<span class='cls-admin'>has <strong class='cls-subscription'>Unubscribed</strong> for " + type + " notification.</span><span class='cls-customer'> <strong>Communication Unsubscription change</strong><span>You have unsubscribed to " + type + " notifications.</span></span>";
                            }
                    }
                stastus = notification_message;

                MySqlConnector.MySqlCommand cmd2 = new MySqlConnector.MySqlCommand("sp_update_commPreference");
                    cmd2.CommandType = CommandType.StoredProcedure;
                    cmd2.Parameters.AddWithValue("_CP_ID", obj.CommunicationPreference[2].CpID);
                    cmd2.Parameters.AddWithValue("_Customer_ID", Customer_ID);
                    cmd2.Parameters.AddWithValue("_Comm_preference_ID", obj.CommunicationPreference[2].PreferenceID);
                    cmd2.Parameters.AddWithValue("_Comm_Preference_Status", obj.CommunicationPreference[2].PreferenceStatus);
                    cmd2.Parameters.AddWithValue("_ClientID", obj.Client_ID);
                    x = db_connection.ExecuteNonQueryProcedure(cmd2);
                    if (x == 1)
                    {
                        msg = "success";
                        string type = "";
                        notification_icon3 = "";
                    obj.Comm_preference_ID_Phone = obj.CommunicationPreference[2].PreferenceID;
                    obj.Comm_Preference_Status_Phone = obj.CommunicationPreference[2].PreferenceStatus;
                        int i = obj.Comm_preference_ID_Phone;
                       
                            type = "Phone";

                            if (obj.Comm_Preference_Status_Phone == 0)
                            {
                                notification_icon3 = "subscription.jpg";
                                notification_message3 = "<span class='cls-admin'>has <strong class='cls-subscription'>Subscribed</strong> for " + type + " notification.</span><span class='cls-customer'> <strong>Communication Subscription change</strong><span>You have subscribed to " + type + " notifications.</span></span>";
                            }
                            else
                            {
                                notification_icon3 = "unsubscription.jpg";
                                notification_message3 = "<span class='cls-admin'>has <strong class='cls-subscription'>Unubscribed</strong> for " + type + " notification.</span><span class='cls-customer'> <strong>Communication Unsubscription change</strong><span>You have unsubscribed to " + type + " notifications.</span></span>";
                            }
                        }
                
            }
            catch (Exception _x)
            {
                return _x.ToString();
            }
            string email_message = "Email: " + notification_message;
            string sms_message = "SMS: " + notification_message2;
            string phone_message = "Phone: " + notification_message3;

            // Concatenate the messages with new lines and spaces
            string full_message = $"{email_message}\n\n{sms_message}\n\n{phone_message}";


            // Return the full message as a JSON response
            return full_message;



        }
    }
}
