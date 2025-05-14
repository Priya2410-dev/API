using MySqlConnector;
using System.Data;
using System.Diagnostics;
using System.Net;

namespace Calyx_Solutions.Service
{
    public class srvMembership //for Kmoney
    {
        private readonly HttpContext _srvMembershipHttpContext;
        private readonly IDbConnection _dbConnection;
        public srvMembership(HttpContext context)
        {
            this._srvMembershipHttpContext = context;
        }

        public DataTable Membership_point_min_max(Model.Customer obj, HttpContext context)
        {
            int Customer_ID = Convert.ToInt32(CompanyInfo.Decrypt(Convert.ToString(obj.Customer_ID), true));
            DataTable dt = new DataTable();
            try
            {
                List<Model.Customer> _lst = new List<Model.Customer>();
                string Client_ID_regex = validation.validate(Convert.ToString(obj.Client_ID), 0, 1, 1, 1, 1, 1, 1, 1, 1);
                string Id_regex = validation.validate(Convert.ToString(Customer_ID), 0, 1, 1, 1, 1, 1, 1, 1, 1);
                if (Client_ID_regex != "false" && Id_regex != "false")
                {
                    MySqlCommand _cmd = new MySqlCommand("Get_memberships_points");
                    _cmd.CommandType = CommandType.StoredProcedure;
                    _cmd.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                    //_cmd.Parameters.AddWithValue("_whereclause", _whereclause);
                    dt = db_connection.ExecuteQueryDataTableProcedure(_cmd);
                }
                else
                {
                    string msg = "Validation Error Client_ID_regex-" + Client_ID_regex + "Id_regex-" + Id_regex;
                    int stattus = (int)CompanyInfo.InsertActivityLogDetails(msg, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Customer_ID, "Membership_point_min_max", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "Membership", context);
                }
            }
            catch (Exception ex) {
                string msg = "Api Membership_point_min_max: " + ex.ToString() + " ";
                CompanyInfo.InsertErrorLogTracker(msg, 0, 0, 0, 0, "Membership_point_min_max", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "", context);
            }
            return dt;
        }

        public DataTable get_membership_benefits_data(Model.Customer obj, HttpContext context)
        {
            int Customer_ID = Convert.ToInt32(CompanyInfo.Decrypt(Convert.ToString(obj.Customer_ID), true));
            DataTable dt = new DataTable();
            try
            {
                List<Model.Customer> _lst = new List<Model.Customer>();
                string Client_ID_regex = validation.validate(Convert.ToString(obj.Client_ID), 0, 1, 1, 1, 1, 1, 1, 1, 1);
                string Id_regex = validation.validate(Convert.ToString(Customer_ID), 0, 1, 1, 1, 1, 1, 1, 1, 1);
                if (Client_ID_regex != "false" && Id_regex != "false")
                {
                    MySqlCommand _cmds = new MySqlCommand("sp_get_membership_benefits_deta");
                    _cmds.CommandType = CommandType.StoredProcedure;
                    _cmds.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                    _cmds.Parameters.AddWithValue("_Membership_ID", obj.Membership_id);
                    dt = db_connection.ExecuteQueryDataTableProcedure(_cmds);
                }
                else
                {
                    string msg = "Validation Error Client_ID_regex-" + Client_ID_regex + "Id_regex-" + Id_regex;
                    int stattus = (int)CompanyInfo.InsertActivityLogDetails(msg, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Customer_ID, "get_membership_benefits_data", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "Membership", context);
                }
            }
            catch (Exception ex) {
                string msg = "Api get_membership_benefits_data: " + ex.ToString() + " ";
                CompanyInfo.InsertErrorLogTracker(msg, 0, 0, 0, 0, "get_membership_benefits_data", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "", context);
            }
            return dt;
        }

        public DataTable get_current_membership_benefits_data(Model.Customer obj, HttpContext context)
        {
            int Customer_ID = Convert.ToInt32(CompanyInfo.Decrypt(Convert.ToString(obj.Customer_ID), true));
            DataTable dt = new DataTable();
            try
            {
                List<Model.Customer> _lst = new List<Model.Customer>();
                string Client_ID_regex = validation.validate(Convert.ToString(obj.Client_ID), 0, 1, 1, 1, 1, 1, 1, 1, 1);
                string Id_regex = validation.validate(Convert.ToString(Customer_ID), 0, 1, 1, 1, 1, 1, 1, 1, 1);
                if (Client_ID_regex != "false" && Id_regex != "false")
                {
                    string whereclause = " and cmmt.Customer_ID=" + Customer_ID + " and cmmt.Client_ID=" + obj.Client_ID + " and mbmt.Delete_Status=0 group by bm.Benefit_ID order by bm.Preference,bm.Benefit_ID";
                    MySqlCommand _cmd = new MySqlCommand("get_customer_membership_mapping");
                    _cmd.CommandType = CommandType.StoredProcedure;
                    _cmd.Parameters.AddWithValue("_Whereclause", whereclause);
                    DataTable dta = db_connection.ExecuteQueryDataTableProcedure(_cmd);
                }
                else
                {
                    string msg = "Validation Error Client_ID_regex-" + Client_ID_regex + "Id_regex-" + Id_regex;
                    int stattus = (int)CompanyInfo.InsertActivityLogDetails(msg, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Customer_ID, "get_current_membership_benefits_data", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "Membership", context);
                }
            }
            catch (Exception ex) {
                string msg = "Api get_current_membership_benefits_data: " + ex.ToString() + " ";
                CompanyInfo.InsertErrorLogTracker(msg, 0, 0, 0, 0, "get_current_membership_benefits_data", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "", context);
            }
            return dt;
        }
        
        public Model.Customer redeem_benefits(Model.Customer obj, HttpContext context)
        {
            int Customer_ID = Convert.ToInt32(CompanyInfo.Decrypt(Convert.ToString(obj.Customer_ID), true));
            DataTable dt = new DataTable();
            Model.Customer _Obj = new Model.Customer();
            try
            {
                string error_invalid_data = ""; string error_msg = "";
                List<Model.Customer> _lst = new List<Model.Customer>();
                string Client_ID_regex = validation.validate(Convert.ToString(obj.Client_ID), 0, 1, 1, 1, 1, 1, 1, 1, 1);
                string Id_regex = validation.validate(Convert.ToString(Customer_ID), 0, 1, 1, 1, 1, 1, 1, 1, 1);
                string Benefit_ID_regex = validation.validate(Convert.ToString(obj.Benefit_ID), 0, 1, 1, 1, 1, 1, 1, 1, 1);
                string Delivery_Address_regex = validation.validate(Convert.ToString(obj.Delivery_Address), 1, 1, 1, 1, 1, 1, 1, 1, 0);
                string Cake_Flavour_ID_regex = validation.validate(Convert.ToString(obj.Cake_Flavour_ID), 0, 1, 1, 1, 1, 1, 1, 1, 1);
                string Cake_Diet_ID_regex = validation.validate(Convert.ToString(obj.Cake_Diet_ID), 0, 1, 1, 1, 1, 1, 1, 1, 1);
                if (Delivery_Address_regex == "false")
                {
                    error_msg = error_msg + " Delivery Address should be alphanumeric with space and special characters like , , , . , ' , ( , ) , { , } , - , @ , &.";
                    error_invalid_data = error_invalid_data + " Delivery_Address: " + obj.Delivery_Address;
                }
                if (Cake_Flavour_ID_regex == "false")
                {
                    error_msg = error_msg + " Cake_Flavour_ID should be numeric value.";
                    error_invalid_data = error_invalid_data + " Cake_Flavour_ID: " + obj.Cake_Flavour_ID;
                }
                if (Cake_Diet_ID_regex == "false")
                {
                    error_msg = error_msg + " Delivery Address should be numeric value.";
                    error_invalid_data = error_invalid_data + " Cake_Diet_ID: " + obj.Cake_Diet_ID;
                }

                if (Client_ID_regex != "false" && Id_regex != "false" && Benefit_ID_regex != "false" && Delivery_Address_regex != "false" && Cake_Flavour_ID_regex != "false" && Cake_Diet_ID_regex != "false")
                {
                    obj.Record_Insert_DateTime = CompanyInfo.gettime(obj.Client_ID, context);
                    DateTime del_date = new DateTime();

                    string whereclause = " and cmmt.Customer_ID=" + Customer_ID + " and cmmt.Client_ID=" + obj.Client_ID + " and mbmt.Benefit_ID=" + obj.Benefit_ID;
                    MySqlCommand _cmds = new MySqlCommand("get_customer_membership_mapping");
                    _cmds.CommandType = CommandType.StoredProcedure;
                    _cmds.Parameters.AddWithValue("_Whereclause", whereclause);
                    DataTable dt_data = db_connection.ExecuteQueryDataTableProcedure(_cmds);
                    if (dt_data != null && dt_data.Rows.Count > 0)
                    {
                        DateTime Last_Redemption_date = new DateTime();
                        if (dt_data.Rows[0]["Last_Redemption_Date"] != null && dt_data.Rows[0]["Last_Redemption_Date"] != DBNull.Value) //checking monthly redemption limit
                        {
                            Last_Redemption_date = Convert.ToDateTime(dt_data.Rows[0]["Last_Redemption_Date"]);
                            int last_access_month = Last_Redemption_date.Month;
                            int current_month = Convert.ToDateTime(obj.Record_Insert_DateTime).Month;

                            if (last_access_month == current_month)
                            {
                                _Obj.Message = "month_limit_exceed";
                                string Activities = obj.Benefit_Name + " monthly redeem limit exceed for Customer Id= " + Customer_ID + "";
                                int stattus = (int)CompanyInfo.InsertActivityLogDetails(Activities, 0, 0, 0, Convert.ToInt32(Customer_ID), "redeem_benefits", Convert.ToInt32("2"), obj.Client_ID, "", context);
                                return _Obj;
                            }
                        }

                        obj.Birth_Date = Convert.ToString(dt_data.Rows[0]["DateOf_Birth"]);
                        if (obj.Delivery_Date != null && obj.Delivery_Date != "") //cake delivery date between 15 to 30 days
                        {
                            if (obj.Benefit_ID == 3)
                            {
                                DateTime min_date = Convert.ToDateTime(obj.Record_Insert_DateTime).AddDays(14).Date;
                                DateTime max_date = Convert.ToDateTime(obj.Record_Insert_DateTime).AddDays(30).Date;
                                string min_date1 = min_date.ToString("dd/MM");
                                string max_date1 = max_date.ToString("dd/MM");
                                min_date = DateTime.ParseExact(min_date1, "dd/MM", null);
                                max_date = DateTime.ParseExact(max_date1, "dd/MM", null);
                                string bdate = Convert.ToDateTime(obj.Birth_Date).ToString("dd/MM");
                                del_date = DateTime.ParseExact(bdate, "dd/MM", null); // change obj.Delivery_Date to Birthday date
                                if (del_date < min_date || max_date < del_date)
                                {
                                    _Obj.Message = "invalid_date";
                                    string Activities = "Invalid Delivery_Date= " + obj.Delivery_Date + "";
                                    int stattus = (int)CompanyInfo.InsertActivityLogDetails(Activities, 0, 0, 0, Convert.ToInt32(Customer_ID), "redeem_benefits", Convert.ToInt32("2"), obj.Client_ID, "", context);
                                    return _Obj;
                                }
                                obj.Delivery_Date = del_date.ToString("yyyy-MM-dd HH:mm:ss");
                            }
                            else if (obj.Benefit_ID == 4)
                            {
                                del_date = Convert.ToDateTime(obj.Record_Insert_DateTime).AddDays(14).Date;
                                obj.Delivery_Date = del_date.ToString("yyyy-MM-dd HH:mm:ss");
                                obj.Delivery_Address = null;
                            }
                            else
                            {
                                obj.Delivery_Date = null;
                                obj.Delivery_Address = null;
                            }
                        }
                        else if (obj.Benefit_ID == 13)
                        {
                            DateTime min_date = Convert.ToDateTime(obj.Record_Insert_DateTime).AddDays(14).Date;
                            DateTime max_date = Convert.ToDateTime(obj.Record_Insert_DateTime).AddDays(30).Date;
                            string min_date1 = min_date.ToString("dd/MM");
                            string max_date1 = max_date.ToString("dd/MM");
                            min_date = DateTime.ParseExact(min_date1, "dd/MM", null);
                            max_date = DateTime.ParseExact(max_date1, "dd/MM", null);
                            string bdate = Convert.ToDateTime(obj.Birth_Date).ToString("dd/MM");
                            del_date = DateTime.ParseExact(bdate, "dd/MM", null); // change obj.Delivery_Date to Birthday date
                            if (del_date < min_date || max_date < del_date)
                            {
                                _Obj.Message = "invalid_date";
                                string Activities = "Invalid Delivery_Date= " + obj.Delivery_Date + "";
                                int stattus = (int)CompanyInfo.InsertActivityLogDetails(Activities, 0, 0, 0, Convert.ToInt32(Customer_ID), "redeem_benefits", Convert.ToInt32("2"), obj.Client_ID, "", context);
                                return _Obj;
                            }
                            obj.Delivery_Date = del_date.ToString("yyyy-MM-dd HH:mm:ss");
                        }
                        else
                        {
                            obj.Delivery_Date = null;
                            obj.Delivery_Address = null;
                        }

                        mts_connection _MTS = new mts_connection();
                        if (obj.Benefit_ID != 1)
                        {
                            using (MySqlConnection con = new MySqlConnection(_MTS.WebConnSetting()))
                            {
                                con.Open();
                                MySqlTransaction transaction;
                                transaction = con.BeginTransaction(IsolationLevel.ReadCommitted);
                                try
                                {
                                    //if (dt_data != null && dt_data.Rows.Count > 0)
                                    //{
                                    int Membership_ID = Convert.ToInt32(dt_data.Rows[0]["Membership_ID"]);
                                    string Membership = Convert.ToString(dt_data.Rows[0]["Membership"]);
                                    int Benefit_Active_Status = Convert.ToInt32(dt_data.Rows[0]["Benefit_Active_Status"]);
                                    double benefit_points = 0;
                                    if (dt_data.Rows[0]["Points"] != null && Convert.ToString(dt_data.Rows[0]["Points"]) != "")
                                    {
                                        benefit_points = Convert.ToDouble(dt_data.Rows[0]["Points"]);
                                    }

                                    int total_members = Convert.ToInt32(dt_data.Rows[0]["Total_Members"]);
                                    //if (dt_data.Rows[0]["Redemption_Flag"] != null && Convert.ToInt32(dt_data.Rows[0]["Redemption_Flag"]) != 0)
                                    //{
                                    if (Benefit_Active_Status == 0)
                                    {
                                        MySqlCommand _cmd1 = new MySqlCommand("get_points_wallet_details");
                                        _cmd1.CommandType = CommandType.StoredProcedure;
                                        _cmd1.Parameters.AddWithValue("_Customer_ID", Customer_ID);
                                        _cmd1.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                                        DataTable dtwallets = db_connection.ExecuteQueryDataTableProcedure(_cmd1);

                                        if (dtwallets != null && dtwallets.Rows.Count > 0)
                                        {
                                            int wallet_active = Convert.ToInt32(dtwallets.Rows[0]["Delete_Status"]);
                                            int points_wallet_id = Convert.ToInt32(dtwallets.Rows[0]["Points_Wallet_ID"]);
                                            string wallet_reference = Convert.ToString(dtwallets.Rows[0]["Wallet_Ref_No"]);
                                            double oldwalletbalance = Convert.ToDouble(dtwallets.Rows[0]["Total_Points"]);
                                            double newwalletbalance = Math.Round(oldwalletbalance - benefit_points, MidpointRounding.AwayFromZero);

                                            if (wallet_active == 0)
                                            {
                                                if (oldwalletbalance >= benefit_points)
                                                {

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
                                                        string notification_icon1 = "points-debited.png";
                                                        string notification_message1 = "<span class='cls-admin'><strong class='cls-reward'>" + benefit_points + "</strong> Points debited from Customer's Points Wallet.</span><span class='cls-customer'><strong></strong><span>" + benefit_points + " Points debited from your Points Wallet.</span></span>";
                                                        CompanyInfo.save_notification(notification_message1, notification_icon1, Customer_ID, Convert.ToDateTime(obj.Record_Insert_DateTime), Convert.ToInt32(obj.Client_ID), 1, 0, Convert.ToInt32(obj.Branch_ID), 1, 0, 1, 0, context);
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
                                                        _cmd5.Parameters.AddWithValue("_Comments", "Debited against Benefit '" + obj.Benefit_Name + "' for " + Membership + " membership");
                                                        _cmd5.Parameters.AddWithValue("_Benefit_ID", obj.Benefit_ID);
                                                        _cmd5.Parameters.AddWithValue("_Client_ID", obj.Client_ID);

                                                        _cmd5.Parameters.Add(new MySqlParameter("_points_mapping_id", MySqlDbType.Int32));
                                                        _cmd5.Parameters["_points_mapping_id"].Direction = ParameterDirection.Output;
                                                        //int pmt = db_connection.ExecuteNonQueryProcedure(_cmd5);
                                                        int pmt = _cmd5.ExecuteNonQuery();

                                                        int points_mapping_id = 0;
                                                        if (_cmd5.Parameters["_points_mapping_id"].Value != DBNull.Value)
                                                        {
                                                            points_mapping_id = Convert.ToInt32(_cmd5.Parameters["_points_mapping_id"].Value);
                                                        }


                                                        if (obj.Delivery_Address == "" || obj.Delivery_Address == null)
                                                        {
                                                            obj.Delivery_Address = null;
                                                        }
                                                        if (obj.Cake_Flavour_ID == "" || obj.Cake_Flavour_ID == null)
                                                        {
                                                            obj.Cake_Flavour_ID = null;
                                                        }
                                                        if (obj.Cake_Diet_ID == "" || obj.Cake_Diet_ID == null)
                                                        {
                                                            obj.Cake_Diet_ID = null;
                                                        }
                                                        if (obj.SuperStore_ID == "" || obj.SuperStore_ID == null)
                                                        {
                                                            obj.SuperStore_ID = null;
                                                        }
                                                        if (obj.EntertainmentVoucher_ID == "" || obj.EntertainmentVoucher_ID == null)
                                                        {
                                                            obj.EntertainmentVoucher_ID = null;
                                                        }

                                                        string whereclause1 = " and Customer_Id=" + Customer_ID + " and Benefit_ID=" + obj.Benefit_ID + " and Delete_Status=0"; // and Membership_ID=" + Membership_ID + "
                                                                                                                                                                                //string myarray = "(" + Customer_ID + "," + obj.Benefit_ID + "," + Membership_ID + "," + "0" + ",\'" + obj.Record_Insert_DateTime + "\'," + obj.Client_ID + ",\'" + obj.Delivery_Date + "\',\'" + obj.Delivery_Address + "\'," + obj.Cake_Flavour_ID + "," + obj.Cake_Diet_ID + ")";
                                                        MySqlCommand cmd2 = new MySqlCommand("save_benefit_redemption_mapping", con);
                                                        cmd2.CommandType = CommandType.StoredProcedure;
                                                        cmd2.Transaction = transaction;
                                                        cmd2.Parameters.AddWithValue("_points_mapping_id", points_mapping_id);
                                                        cmd2.Parameters.AddWithValue("_Customer_ID", Customer_ID);
                                                        cmd2.Parameters.AddWithValue("_Benefit_ID", obj.Benefit_ID);
                                                        cmd2.Parameters.AddWithValue("_Membership_ID", Membership_ID);
                                                        cmd2.Parameters.AddWithValue("_Redemption_Flag", 0);
                                                        cmd2.Parameters.AddWithValue("_Record_Insert_DateTime", obj.Record_Insert_DateTime);
                                                        cmd2.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                                                        cmd2.Parameters.AddWithValue("_Delivery_Date", obj.Delivery_Date);
                                                        cmd2.Parameters.AddWithValue("_Delivery_Address", obj.Delivery_Address);
                                                        cmd2.Parameters.AddWithValue("_Cake_Flavour_ID", obj.Cake_Flavour_ID);
                                                        cmd2.Parameters.AddWithValue("_Cake_Diet_ID", obj.Cake_Diet_ID);
                                                        cmd2.Parameters.AddWithValue("_SuperStore_ID", obj.SuperStore_ID);
                                                        cmd2.Parameters.AddWithValue("_EntertainmentVoucher_ID", obj.EntertainmentVoucher_ID);

                                                        cmd2.Parameters.AddWithValue("_whereclause", whereclause1);
                                                        //DataTable dta2 = db_connection.ExecuteQueryDataTableProcedure(cmd2);
                                                        DataTable dta2 = new DataTable();
                                                        MySqlDataAdapter da = new MySqlDataAdapter(cmd2);
                                                        da.Fill(dta2);

                                                        if (dta2.Rows.Count > 0)
                                                        {
                                                            if (Convert.ToInt32(dta2.Rows[0]["RowsInserted"]) > 0)
                                                            {
                                                                transaction.Commit();
                                                                _Obj.Message = "success";
                                                                _Obj.Comment = benefit_points + " Points deducted from Points wallet. Remaining points: " + newwalletbalance;
                                                                string Activities = "Benefit_ID= " + obj.Benefit_ID + " redeemed Successfully againts Membership_ID= " + Membership_ID;
                                                                int stattus = (int)CompanyInfo.InsertActivityLogDetails(Activities, 0, 0, 0, Convert.ToInt32(Customer_ID), "redeem_benefits", Convert.ToInt32("2"), obj.Client_ID, "", context);

                                                                string msgg = "";
                                                                if (benefit_points > 0)
                                                                {
                                                                    msgg = _Obj.Comment;
                                                                }

                                                                string notification_icon = "benefit-redeemed.png";
                                                                string notification_message = "<span class='cls-admin'><strong class='cls-reward'>" + obj.Benefit_Name + "</strong> Benefit Redeemed successfully. " + msgg + "</span><span class='cls-customer'><strong>" + obj.Benefit_Name + "</strong> Benefit Redeemed successfully. " + msgg + "</span>";
                                                                CompanyInfo.save_notification(notification_message, notification_icon, Customer_ID, Convert.ToDateTime(obj.Record_Insert_DateTime), Convert.ToInt32(obj.Client_ID), 1, 0, Convert.ToInt32(obj.Branch_ID), 1, 0, 1, 0, context);

                                                                try
                                                                {
                                                                    DataTable dt_notif = CompanyInfo.set_notification_data(73);
                                                                    if (dt_notif.Rows.Count > 0)
                                                                    {
                                                                        int SMS = Convert.ToInt32(dt_notif.Rows[0]["SMS"]);
                                                                        int Email1 = Convert.ToInt32(dt_notif.Rows[0]["Email"]);
                                                                        int Notif_status = Convert.ToInt32(dt_notif.Rows[0]["Notification"]);
                                                                        string notification_msg = Convert.ToString(dt_notif.Rows[0]["notification_msg"]);
                                                                        if (notification_msg.Contains("[benefit_name]") == true)
                                                                        {
                                                                            notification_msg = notification_msg.Replace("[benefit_name]", obj.Benefit_Name);
                                                                        }
                                                                        if (notification_msg.Contains("[msg]") == true)
                                                                        {
                                                                            notification_msg = notification_msg.Replace("[msg]", msgg);
                                                                        }

                                                                        int i1 = CompanyInfo.check_notification_perm(Convert.ToString(Customer_ID), obj.Client_ID, obj.Branch_ID, 1, 73, Convert.ToDateTime(obj.Record_Insert_DateTime), 1, SMS, Email1, Notif_status, "App- Benefit Redeemed - 73", notification_msg, context);
                                                                    }
                                                                }
                                                                catch (Exception ex)
                                                                {
                                                                }

                                                                #region send mail to Customer for Benefit redemption
                                                                try
                                                                {
                                                                    string email = obj.Email;
                                                                    string Cust_Name = obj.Name;

                                                                    string subject = string.Empty;
                                                                    string body = string.Empty;

                                                                    DataTable dtc = CompanyInfo.get(obj.Client_ID, context);
                                                                    string company_name = Convert.ToString(dtc.Rows[0]["Company_Name"]);
                                                                    string URL = Convert.ToString(dtc.Rows[0]["Company_URL_Admin"]);
                                                                    string cust_url = Convert.ToString(dtc.Rows[0]["Company_URL_Customer"]);

                                                                    HttpWebRequest httpRequest;
                                                                    httpRequest = (HttpWebRequest)WebRequest.Create(URL + "Email/benefit-redemption.html");
                                                                    //httpRequest = (HttpWebRequest)WebRequest.Create("http://localhost:51078/Email/benefit-redemption.html");

                                                                    httpRequest.UserAgent = "Code Sample Web Client";
                                                                    HttpWebResponse webResponse = (HttpWebResponse)httpRequest.GetResponse();
                                                                    using (StreamReader reader = new StreamReader(webResponse.GetResponseStream()))
                                                                    {
                                                                        body = reader.ReadToEnd();
                                                                    }

                                                                    string style_health = "display: none;";
                                                                    string style_raffle = "display: none;";
                                                                    string style_birthday = "display: none;";
                                                                    string style_grocery = "display: none;";
                                                                    string style_entertainment = "display: none;";
                                                                    string style_bdayTreat = "display: none;";
                                                                    string style_mystery_gift = "display: none;";
                                                                    string div_points = "";

                                                                    if (obj.Benefit_ID == 2)
                                                                    {
                                                                        style_raffle = "";
                                                                    }
                                                                    if (obj.Benefit_ID == 3)
                                                                    {
                                                                        style_birthday = "";
                                                                    }
                                                                    if (obj.Benefit_ID == 4)
                                                                    {
                                                                        style_grocery = "";
                                                                    }
                                                                    if (obj.Benefit_ID == 5)
                                                                    {
                                                                        style_entertainment = "";
                                                                    }
                                                                    if (obj.Benefit_ID == 12)
                                                                    {
                                                                        style_mystery_gift = "";
                                                                    }
                                                                    if (obj.Benefit_ID == 13)
                                                                    {
                                                                        style_bdayTreat = "";
                                                                    }

                                                                    body = body.Replace("[style_health]", style_health);
                                                                    body = body.Replace("[style_raffle]", style_raffle);
                                                                    body = body.Replace("[style_birthday]", style_birthday);
                                                                    body = body.Replace("[style_grocery]", style_grocery);
                                                                    body = body.Replace("[style_entertainment]", style_entertainment);
                                                                    body = body.Replace("[style_bdayTreat]", style_bdayTreat);
                                                                    body = body.Replace("[style_mystery_gift]", style_mystery_gift);

                                                                    body = body.Replace("[name]", Cust_Name);
                                                                    body = body.Replace("[benefit_name]", obj.Benefit_Name);
                                                                    body = body.Replace("[Membership]", Membership);
                                                                    body = body.Replace("[benefit_points]", Convert.ToString(benefit_points));
                                                                    body = body.Replace("[balance_points]", Convert.ToString(newwalletbalance));

                                                                    body = body.Replace("[div_points]", div_points);


                                                                    DataTable d1 = (DataTable)CompanyInfo.getCustomerDetails(obj.Client_ID, Customer_ID);

                                                                    //subject = company_name + " - Benefit Redeemed - " + d1.Rows[0]["WireTransfer_ReferanceNo"];

                                                                    HttpWebRequest httpRequest1;
                                                                    httpRequest1 = (HttpWebRequest)WebRequest.Create(URL + "Email/benefit-redeemed.txt");
                                                                    //httpRequest1 = (HttpWebRequest)WebRequest.Create("http://localhost:51078/Email/benefit-redeemed.txt");//subject

                                                                    httpRequest1.UserAgent = "Code Sample Web Client";
                                                                    HttpWebResponse webResponse1 = (HttpWebResponse)httpRequest1.GetResponse();
                                                                    using (StreamReader reader = new StreamReader(webResponse1.GetResponseStream()))
                                                                    {
                                                                        subject = company_name + " " + reader.ReadLine() + " - " + d1.Rows[0]["WireTransfer_ReferanceNo"];
                                                                    }

                                                                    string send_mail = (string)CompanyInfo.Send_Mail(dtc, email, body, subject, obj.Client_ID, obj.Branch_ID, "", "", "", context);
                                                                }
                                                                catch (Exception ex)
                                                                {

                                                                }
                                                                #endregion
                                                            }
                                                            else
                                                            {
                                                                transaction.Rollback();
                                                                _Obj.Message = "failed";
                                                                string Activities = "Failed to redeem Benefit_ID= " + obj.Benefit_ID + " againts Membership_ID= " + Membership_ID;
                                                                int stattus = (int)CompanyInfo.InsertActivityLogDetails(Activities, 0, 0, 0, Convert.ToInt32(Customer_ID), "redeem_benefits", Convert.ToInt32("2"), obj.Client_ID, "", context);
                                                            }
                                                        }
                                                        else
                                                        {
                                                            transaction.Rollback();
                                                            _Obj.Message = "failed";
                                                            string Activities = "Failed to redeem Benefit_ID= " + obj.Benefit_ID + " againts Membership_ID= " + Membership_ID;
                                                            int stattus = (int)CompanyInfo.InsertActivityLogDetails(Activities, 0, 0, 0, Convert.ToInt32(Customer_ID), "redeem_benefits", Convert.ToInt32("2"), obj.Client_ID, "", context);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        transaction.Rollback();
                                                        _Obj.Message = "failed";
                                                        string Activities = "Failed to redeem update points wallet for Benefit_ID= " + obj.Benefit_ID + " againts Membership_ID= " + Membership_ID;
                                                        int stattus = (int)CompanyInfo.InsertActivityLogDetails(Activities, 0, 0, 0, Convert.ToInt32(Customer_ID), "redeem_benefits", Convert.ToInt32("2"), obj.Client_ID, "", context);
                                                    }
                                                }
                                                else
                                                {
                                                    _Obj.Message = "no_enough_points";
                                                    string Activities = "No Enought Points. Failed to Debit " + benefit_points + " Points against Benefit_ID= " + obj.Benefit_ID + ". Old Wallet Points: " + oldwalletbalance + " ";
                                                    int stattus = (int)CompanyInfo.InsertActivityLogDetails(Activities, 0, 0, 0, Convert.ToInt32(Customer_ID), "redeem_benefits", Convert.ToInt32("2"), obj.Client_ID, "", context);
                                                }
                                            }
                                            else
                                            {
                                                _Obj.Message = "wallet_inactive";
                                                string Activities = "Points wallet is inactive. Debit " + benefit_points + " Points against Benefit_ID= " + obj.Benefit_ID;
                                                int stattus = (int)CompanyInfo.InsertActivityLogDetails(Activities, 0, 0, 0, Convert.ToInt32(Customer_ID), "redeem_benefits", Convert.ToInt32("2"), obj.Client_ID, "", context);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        _Obj.Message = "unavailable";
                                        string Activities = "Benefit_ID= " + obj.Benefit_ID + " not available for Membership_ID= " + Membership_ID;
                                        int stattus = (int)CompanyInfo.InsertActivityLogDetails(Activities, 0, 0, 0, Convert.ToInt32(Customer_ID), "redeem_benefits", Convert.ToInt32("2"), obj.Client_ID, "", context);
                                    }
                                    //}
                                    //else
                                    //{
                                    //    _Obj.Message = "already_redeemed";
                                    //    string Activities = "Benefit_ID= " + obj.Benefit_ID + " is already redeemed for Membership_ID= " + Membership_ID;
                                    //    int stattus = (int)CompanyInfo.InsertActivityLogDetails(Activities, 0, 0, 0, Convert.ToInt32(Customer_ID), "redeem_benefits", Convert.ToInt32("2"), obj.Client_ID, "");
                                    //}
                                    //}
                                }
                                catch (Exception ex)
                                {
                                    transaction.Rollback();
                                    _Obj.Message = "error";
                                    //Error Log Handled
                                    Model.ErrorLog objError = new Model.ErrorLog();
                                    objError.User = new Model.User();
                                    objError.Error = "srvMembership : redeem_benefits ---" + ex.ToString();
                                    objError.Date = DateTime.Now;
                                    objError.User_ID = 1;
                                    objError.Client_ID = obj.Client_ID;
                                    objError.Function_Name = "redeem_benefits";
                                    Service.srvErrorLog srvError = new Service.srvErrorLog();
                                    srvError.Create(objError, context);
                                }
                                finally
                                {
                                    con.Close();
                                }
                            }
                        }
                        else
                        {
                            _Obj.Message = "failed";
                            string Activities = "Invalid Benefit_ID= " + obj.Benefit_ID + "";
                            int stattus = (int)CompanyInfo.InsertActivityLogDetails(Activities, 0, 0, 0, Convert.ToInt32(Customer_ID), "redeem_benefits", Convert.ToInt32("2"), obj.Client_ID, "Membership", context);
                        }
                    }
                }
                else
                {
                    string msg = "Validation Error Client_ID_regex-" + Client_ID_regex + "Id_regex-" + Id_regex;
                    int stattus = (int)CompanyInfo.InsertActivityLogDetails(msg, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Customer_ID, "get_current_membership_benefits_data", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "Membership", context);
                }
            }
            catch (Exception ex) {
                string msg = "Api get_current_membership_benefits_data: " + ex.ToString() + " ";
                CompanyInfo.InsertErrorLogTracker(msg, 0, 0, 0, 0, "get_current_membership_benefits_data", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "", context);
            }
            return _Obj;
        }

        public string redeem_health_care(Model.Customer obj, HttpContext context)
        {
            string status = "";
            try
            {
                int Customer_ID = Convert.ToInt32(CompanyInfo.Decrypt(Convert.ToString(obj.Customer_ID), true));
                string Client_ID_regex = validation.validate(Convert.ToString(obj.Client_ID), 0, 1, 1, 1, 1, 1, 1, 1, 1);
                string Benefit_ID_regex = validation.validate(Convert.ToString(obj.Benefit_ID), 0, 1, 1, 1, 1, 1, 1, 1, 1);

                if (Client_ID_regex != "false" && Benefit_ID_regex != "false")
                {
                    obj.Record_Insert_DateTime = CompanyInfo.gettime(obj.Client_ID, context);
                    string whereclause = " and cmmt.Customer_ID=" + Customer_ID + " and cmmt.Client_ID=" + obj.Client_ID + " and mbmt.Benefit_ID=" + obj.Benefit_ID;
                    MySqlCommand _cmds = new MySqlCommand("get_customer_membership_mapping");
                    _cmds.CommandType = CommandType.StoredProcedure;
                    _cmds.Parameters.AddWithValue("_Whereclause", whereclause);
                    DataTable dt_data = db_connection.ExecuteQueryDataTableProcedure(_cmds);
                    if (dt_data != null && dt_data.Rows.Count > 0)
                    {
                        int Membership_ID = Convert.ToInt32(dt_data.Rows[0]["Membership_ID"]);
                        string Membership = Convert.ToString(dt_data.Rows[0]["Membership"]);
                        int Benefit_Active_Status = Convert.ToInt32(dt_data.Rows[0]["Benefit_Active_Status"]);
                        double benefit_points = 0;

                        if (Benefit_Active_Status == 0)
                        {

                            MySqlCommand _cmd1 = new MySqlCommand("get_points_wallet_details");
                            _cmd1.CommandType = CommandType.StoredProcedure;
                            _cmd1.Parameters.AddWithValue("_Customer_ID", Customer_ID);
                            _cmd1.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                            DataTable dtwallets = db_connection.ExecuteQueryDataTableProcedure(_cmd1);
                            if (dtwallets != null && dtwallets.Rows.Count > 0)
                            {
                                int wallet_active = Convert.ToInt32(dtwallets.Rows[0]["Delete_Status"]);

                                if (wallet_active == 0)
                                {
                                    if (obj.Benefit_ID == 1)
                                    {
                                        MySqlCommand _cmd4 = new MySqlCommand("update_medicare_flag");
                                        _cmd4.CommandType = CommandType.StoredProcedure;
                                        _cmd4.Parameters.AddWithValue("_Medicare_Flag", 0);
                                        _cmd4.Parameters.AddWithValue("_Customer_ID", Customer_ID);
                                        _cmd4.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                                        int ipw = db_connection.ExecuteNonQueryProcedure(_cmd4);

                                        if (ipw > 0)
                                        {
                                            try
                                            {
                                                string whereclause1 = " and Customer_Id=" + Customer_ID + " and Benefit_ID=" + obj.Benefit_ID + " and Delete_Status=0"; // and Membership_ID=" + Membership_ID + "
                                                                                                                                                                        //string myarray = "(" + Customer_ID + "," + obj.Benefit_ID + "," + Membership_ID + "," + "0" + ",\'" + obj.Record_Insert_DateTime + "\'," + obj.Client_ID + ",\'" + obj.Delivery_Date + "\',\'" + obj.Delivery_Address + "\'," + obj.Cake_Flavour_ID + "," + obj.Cake_Diet_ID + ")";
                                                MySqlCommand cmd2 = new MySqlCommand("save_benefit_redemption_mapping");
                                                cmd2.CommandType = CommandType.StoredProcedure;
                                                cmd2.Parameters.AddWithValue("_points_mapping_id", 0);
                                                cmd2.Parameters.AddWithValue("_Customer_ID", Customer_ID);
                                                cmd2.Parameters.AddWithValue("_Benefit_ID", obj.Benefit_ID);
                                                cmd2.Parameters.AddWithValue("_Membership_ID", Membership_ID);
                                                cmd2.Parameters.AddWithValue("_Redemption_Flag", 0);
                                                cmd2.Parameters.AddWithValue("_Record_Insert_DateTime", obj.Record_Insert_DateTime);
                                                cmd2.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                                                cmd2.Parameters.AddWithValue("_Delivery_Date", null);
                                                cmd2.Parameters.AddWithValue("_Delivery_Address", null);
                                                cmd2.Parameters.AddWithValue("_Cake_Flavour_ID", null);
                                                cmd2.Parameters.AddWithValue("_Cake_Diet_ID", null);
                                                cmd2.Parameters.AddWithValue("_SuperStore_ID", null);
                                                cmd2.Parameters.AddWithValue("_EntertainmentVoucher_ID", null);

                                                cmd2.Parameters.AddWithValue("_whereclause", whereclause1);
                                                DataTable dta2 = db_connection.ExecuteQueryDataTableProcedure(cmd2);
                                                //DataTable dta2 = new DataTable();
                                                //MySqlDataAdapter da = new MySqlDataAdapter(cmd2);
                                                //da.Fill(dta2);
                                            }
                                            catch (Exception ex) { }

                                            status = "success";

                                            string Activities = "Medicare flag enabled for Customer_ID= " + Customer_ID;
                                            int stattus = (int)CompanyInfo.InsertActivityLogDetails(Activities, 0, 0, 0, Convert.ToInt32(Customer_ID), "redeem_health_care", Convert.ToInt32("2"), Convert.ToInt32("1"), "Membership", context);

                                            string msgg = "";
                                            string notification_icon = "benefit-redeemed.png";
                                            string notification_message = "<span class='cls-admin'><strong class='cls-reward'>" + obj.Benefit_Name + "</strong> Benefit Redeemed successfully. " + msgg + "</span><span class='cls-customer'><strong>" + obj.Benefit_Name + "</strong> Benefit Redeemed successfully. " + msgg + "</span>";
                                            CompanyInfo.save_notification(notification_message, notification_icon, Customer_ID, Convert.ToDateTime(obj.Record_Insert_DateTime), Convert.ToInt32(obj.Client_ID), 1, 0, Convert.ToInt32(obj.Branch_ID), 1, 0, 1, 0, context);

                                            try
                                            {
                                                DataTable dt_notif = CompanyInfo.set_notification_data(73);
                                                if (dt_notif.Rows.Count > 0)
                                                {
                                                    int SMS = Convert.ToInt32(dt_notif.Rows[0]["SMS"]);
                                                    int Email1 = Convert.ToInt32(dt_notif.Rows[0]["Email"]);
                                                    int Notif_status = Convert.ToInt32(dt_notif.Rows[0]["Notification"]);
                                                    string notification_msg = Convert.ToString(dt_notif.Rows[0]["notification_msg"]);
                                                    if (notification_msg.Contains("[benefit_name]") == true)
                                                    {
                                                        notification_msg = notification_msg.Replace("[benefit_name]", obj.Benefit_Name);
                                                    }
                                                    if (notification_msg.Contains("[msg]") == true)
                                                    {
                                                        notification_msg = notification_msg.Replace("[msg]", msgg);
                                                    }

                                                    int i1 = CompanyInfo.check_notification_perm(Convert.ToString(Customer_ID), obj.Client_ID, obj.Branch_ID, 1, 73, Convert.ToDateTime(obj.Record_Insert_DateTime), 1, SMS, Email1, Notif_status, "App- Benefit Redeemed - 73", notification_msg, context);
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                            }

                                            #region send mail to Customer for Benefit redemption
                                            try
                                            {
                                                string email = obj.Email;
                                                string Cust_Name = obj.Name;

                                                string subject = string.Empty;
                                                string body = string.Empty;

                                                DataTable dtc = CompanyInfo.get(obj.Client_ID, context);
                                                string company_name = Convert.ToString(dtc.Rows[0]["Company_Name"]);
                                                string URL = Convert.ToString(dtc.Rows[0]["Company_URL_Admin"]);
                                                string cust_url = Convert.ToString(dtc.Rows[0]["Company_URL_Customer"]);

                                                HttpWebRequest httpRequest;
                                                //httpRequest = (HttpWebRequest)WebRequest.Create(URL + "Email/benefit-redemption.html");
                                                httpRequest = (HttpWebRequest)WebRequest.Create("http://localhost:51078/Email/benefit-redemption.html");

                                                httpRequest.UserAgent = "Code Sample Web Client";
                                                HttpWebResponse webResponse = (HttpWebResponse)httpRequest.GetResponse();
                                                using (StreamReader reader = new StreamReader(webResponse.GetResponseStream()))
                                                {
                                                    body = reader.ReadToEnd();
                                                }

                                                string style_health = "display: none;";
                                                string style_raffle = "display: none;";
                                                string style_birthday = "display: none;";
                                                string style_grocery = "display: none;";
                                                string style_entertainment = "display: none;";
                                                string style_bdayTreat = "display: none;";
                                                string style_mystery_gift = "display: none;";
                                                string div_points = "display: none;";

                                                if (obj.Benefit_ID == 1)
                                                {
                                                    style_health = "";
                                                }
                                                if (benefit_points > 0)
                                                {
                                                    div_points = "";
                                                }

                                                body = body.Replace("[style_health]", style_health);
                                                body = body.Replace("[style_raffle]", style_raffle);
                                                body = body.Replace("[style_birthday]", style_birthday);
                                                body = body.Replace("[style_grocery]", style_grocery);
                                                body = body.Replace("[style_entertainment]", style_entertainment);
                                                body = body.Replace("[style_bdayTreat]", style_bdayTreat);
                                                body = body.Replace("[style_mystery_gift]", style_mystery_gift);

                                                body = body.Replace("[name]", Cust_Name);
                                                body = body.Replace("[benefit_name]", obj.Benefit_Name);
                                                body = body.Replace("[Membership]", Membership);
                                                body = body.Replace("[benefit_points]", "");
                                                body = body.Replace("[balance_points]", "");

                                                body = body.Replace("[div_points]", div_points);


                                                DataTable d1 = (DataTable)CompanyInfo.getCustomerDetails(obj.Client_ID, Customer_ID);

                                                //subject = company_name + " - Benefit Redeemed - " + d1.Rows[0]["WireTransfer_ReferanceNo"];

                                                HttpWebRequest httpRequest1;
                                                //httpRequest1 = (HttpWebRequest)WebRequest.Create(URL + "Email/benefit-redeemed.txt");
                                                httpRequest1 = (HttpWebRequest)WebRequest.Create("http://localhost:51078/Email/benefit-redeemed.txt");//subject

                                                httpRequest1.UserAgent = "Code Sample Web Client";
                                                HttpWebResponse webResponse1 = (HttpWebResponse)httpRequest1.GetResponse();
                                                using (StreamReader reader = new StreamReader(webResponse1.GetResponseStream()))
                                                {
                                                    //subject = reader.ReadLine();
                                                    subject = company_name + " " + reader.ReadLine() + " - " + d1.Rows[0]["WireTransfer_ReferanceNo"];
                                                }

                                                string send_mail = (string)CompanyInfo.Send_Mail(dtc, email, body, subject, obj.Client_ID, obj.Branch_ID, "", "", "", context);
                                            }
                                            catch (Exception ex)
                                            {

                                            }
                                            #endregion
                                        }
                                        else
                                        {
                                            status = "failed";

                                            string Activities = "Failed to enable Medicare flag for Customer_ID= " + Customer_ID;
                                            int stattus = (int)CompanyInfo.InsertActivityLogDetails(Activities, 0, 0, 0, Convert.ToInt32(Customer_ID), "redeem_health_care", Convert.ToInt32("2"), Convert.ToInt32("1"), "Membership", context);
                                        }
                                    }
                                }
                                else
                                {
                                    status = "wallet_inactive";

                                    string act = "Points wallet is inactive. Failed to enable Medicare flag for Customer_ID= " + Customer_ID;
                                    int stattus = (int)CompanyInfo.InsertActivityLogDetails(act, 0, 0, 0, Convert.ToInt32(Customer_ID), "redeem_health_care", Convert.ToInt32("2"), Convert.ToInt32("1"), "Membership", context);

                                }

                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                status = "error";
                //Error Log Handled
                Model.ErrorLog objError = new Model.ErrorLog();
                objError.User = new Model.User();
                objError.Error = "srvMembership : redeem_health_care --" + ex.ToString();
                objError.Date = DateTime.Now;
                objError.User_ID = 1;
                objError.Client_ID = obj.Client_ID;
                objError.Function_Name = "redeem_health_care";
                Service.srvErrorLog srvError = new Service.srvErrorLog();
                srvError.Create(objError, context);
            }
            return status;
        }

        public DataTable RemainingMedicareCount(Model.Customer obj, HttpContext context)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("referral_remaining", typeof(string));
            dt.Columns.Add("membership_remaining", typeof(int));
            dt.Columns.Add("referral_used", typeof(int));
            dt.Columns.Add("membership_used", typeof(int));
            dt.Columns.Add("customer_used", typeof(int));
            try
            {
                int Customer_ID = Convert.ToInt32(CompanyInfo.Decrypt(Convert.ToString(obj.Customer_ID), true));
                string Activity = string.Empty;
                int referral_remaining = 0; int membership_remaining = 0; int total_members = 0;
                int used_referral_count = 0; int used_membership_count = 0; int used_customer_slots = 0;

                obj.Record_Insert_DateTime = CompanyInfo.gettime(obj.Client_ID, context);


                string where_clause = " Customer_ID=" + Customer_ID + " and Client_ID=" + obj.Client_ID;
                MySqlCommand _cmda = new MySqlCommand("get_referral_count");
                _cmda.CommandType = CommandType.StoredProcedure;
                _cmda.Parameters.AddWithValue("_Whereclause", where_clause);
                DataTable dt_ref = db_connection.ExecuteQueryDataTableProcedure(_cmda);
                if (dt_ref != null && dt_ref.Rows.Count > 0)
                {
                    if (dt_ref.Rows[0]["referral_medicare_count"] != null && dt_ref.Rows[0]["referral_medicare_count"].ToString() != "")
                    {
                        referral_remaining = Convert.ToInt32(dt_ref.Rows[0]["referral_medicare_count"]);
                    }
                }
                string whereclause = " and cmmt.Customer_ID=" + Customer_ID + " and cmmt.Client_ID=" + obj.Client_ID + " and mbmt.Benefit_ID=1";
                MySqlCommand _cmds = new MySqlCommand("get_customer_membership_mapping");
                _cmds.CommandType = CommandType.StoredProcedure;
                _cmds.Parameters.AddWithValue("_Whereclause", whereclause);
                DataTable dt_data = db_connection.ExecuteQueryDataTableProcedure(_cmds);

                if (dt_data != null && dt_data.Rows.Count > 0)
                {
                    double benefit_points = Convert.ToDouble(dt_data.Rows[0]["Points"]);
                    total_members = Convert.ToInt32(dt_data.Rows[0]["Total_Members"]);
                    int Membership_ID = Convert.ToInt32(dt_data.Rows[0]["Membership_ID"]);
                }

                MySqlCommand _cmd = new MySqlCommand("get_medicare_used_counts");
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.Parameters.AddWithValue("_Customer_ID", Customer_ID);
                _cmd.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                DataTable dt_med = db_connection.ExecuteQueryDataTableProcedure(_cmd);
                if (dt_data.Rows.Count > 0 && dt_med != null && dt_med.Rows.Count > 0)
                {
                    if (dt_med.Rows[0]["used_membership_count"] != null)
                    {
                        used_membership_count = Convert.ToInt32(dt_med.Rows[0]["used_membership_count"]);
                        membership_remaining = total_members - used_membership_count;
                    }
                    if (dt_med.Rows[0]["used_referral_count"] != null)
                    {
                        used_referral_count = Convert.ToInt32(dt_med.Rows[0]["used_referral_count"]);
                    }
                    if (dt_med.Rows[0]["used_customer_slots"] != null)
                    {
                        used_customer_slots = Convert.ToInt32(dt_med.Rows[0]["used_customer_slots"]);
                    }
                    if (membership_remaining <= 0)
                    {
                        membership_remaining = 0;
                    }
                }

                dt.Rows.Add(referral_remaining, membership_remaining, used_referral_count, used_membership_count, used_customer_slots);
            }
            catch (Exception ex)
            {
                string msg = "Api get_current_membership_benefits_data: " + ex.ToString() + " ";
                CompanyInfo.InsertErrorLogTracker(msg, 0, 0, 0, 0, "get_current_membership_benefits_data", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "", context);
            }
            return dt;
        }
        public DataTable GetCakeFlavours(Model.Customer obj, HttpContext context)
        {
            int Customer_ID = Convert.ToInt32(CompanyInfo.Decrypt(Convert.ToString(obj.Customer_ID), true));
            DataTable dt = new DataTable();
            try
            {
                List<Model.Customer> _lst = new List<Model.Customer>();
                string Client_ID_regex = validation.validate(Convert.ToString(obj.Client_ID), 0, 1, 1, 1, 1, 1, 1, 1, 1);
                string Id_regex = validation.validate(Convert.ToString(Customer_ID), 0, 1, 1, 1, 1, 1, 1, 1, 1);
                if (Client_ID_regex != "false" && Id_regex != "false")
                {
                    MySqlCommand _cmd = new MySqlCommand("get_cake_flavours");
                    _cmd.CommandType = CommandType.StoredProcedure;
                    _cmd.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                    dt = db_connection.ExecuteQueryDataTableProcedure(_cmd);
                }
                else
                {
                    string msg = "Validation Error Client_ID_regex-" + Client_ID_regex + "Id_regex-" + Id_regex;
                    int stattus = (int)CompanyInfo.InsertActivityLogDetails(msg, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Customer_ID, "GetCakeFlavours", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "Membership", context);
                }
            }
            catch (Exception ex)
            {
                string msg = "Api GetCakeFlavours: " + ex.ToString() + " ";
                CompanyInfo.InsertErrorLogTracker(msg, 0, 0, 0, 0, "GetCakeFlavours", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "", context);
            }
            return dt;
        }
        public DataTable GetCakeDiet(Model.Customer obj, HttpContext context)
        {
            int Customer_ID = Convert.ToInt32(CompanyInfo.Decrypt(Convert.ToString(obj.Customer_ID), true));
            DataTable dt = new DataTable();
            try
            {
                List<Model.Customer> _lst = new List<Model.Customer>();
                string Client_ID_regex = validation.validate(Convert.ToString(obj.Client_ID), 0, 1, 1, 1, 1, 1, 1, 1, 1);
                string Id_regex = validation.validate(Convert.ToString(Customer_ID), 0, 1, 1, 1, 1, 1, 1, 1, 1);
                if (Client_ID_regex != "false" && Id_regex != "false")
                {
                    MySqlCommand _cmd = new MySqlCommand("get_cake_diet");
                    _cmd.CommandType = CommandType.StoredProcedure;
                    _cmd.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                    dt = db_connection.ExecuteQueryDataTableProcedure(_cmd);
                }
                else
                {
                    string msg = "Validation Error Client_ID_regex-" + Client_ID_regex + "Id_regex-" + Id_regex;
                    int stattus = (int)CompanyInfo.InsertActivityLogDetails(msg, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Customer_ID, "GetCakeFlavours", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "Membership", context);
                }
            }
            catch (Exception ex)
            {
                string msg = "Api GetCakeFlavours: " + ex.ToString() + " ";
                CompanyInfo.InsertErrorLogTracker(msg, 0, 0, 0, 0, "GetCakeFlavours", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "", context);
            }
            return dt;
        }
        public DataTable GetSuperStores(Model.Customer obj, HttpContext context)
        {
            int Customer_ID = Convert.ToInt32(CompanyInfo.Decrypt(Convert.ToString(obj.Customer_ID), true));
            DataTable dt = new DataTable();
            try
            {
                List<Model.Customer> _lst = new List<Model.Customer>();
                string Client_ID_regex = validation.validate(Convert.ToString(obj.Client_ID), 0, 1, 1, 1, 1, 1, 1, 1, 1);
                string Id_regex = validation.validate(Convert.ToString(Customer_ID), 0, 1, 1, 1, 1, 1, 1, 1, 1);
                if (Client_ID_regex != "false" && Id_regex != "false")
                {
                    MySqlCommand _cmd = new MySqlCommand("get_superstore_list");
                    _cmd.CommandType = CommandType.StoredProcedure;
                    _cmd.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                    dt = db_connection.ExecuteQueryDataTableProcedure(_cmd);
                }
                else
                {
                    string msg = "Validation Error Client_ID_regex-" + Client_ID_regex + "Id_regex-" + Id_regex;
                    int stattus = (int)CompanyInfo.InsertActivityLogDetails(msg, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Customer_ID, "GetSuperStores", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "Membership", context);
                }
            }
            catch (Exception ex)
            {
                string msg = "Api GetSuperStores: " + ex.ToString() + " ";
                CompanyInfo.InsertErrorLogTracker(msg, 0, 0, 0, 0, "GetSuperStores", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "", context);
            }
            return dt;
        }
        
        public DataTable GetEntertainmentVouchers(Model.Customer obj, HttpContext context)
        {
            int Customer_ID = Convert.ToInt32(CompanyInfo.Decrypt(Convert.ToString(obj.Customer_ID), true));
            DataTable dt = new DataTable();
            try
            {
                List<Model.Customer> _lst = new List<Model.Customer>();
                string Client_ID_regex = validation.validate(Convert.ToString(obj.Client_ID), 0, 1, 1, 1, 1, 1, 1, 1, 1);
                string Id_regex = validation.validate(Convert.ToString(Customer_ID), 0, 1, 1, 1, 1, 1, 1, 1, 1);
                if (Client_ID_regex != "false" && Id_regex != "false")
                {
                    MySqlCommand _cmd = new MySqlCommand("get_entertainment_voucher_list");
                    _cmd.CommandType = CommandType.StoredProcedure;
                    _cmd.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                    dt = db_connection.ExecuteQueryDataTableProcedure(_cmd);
                }
                else
                {
                    string msg = "Validation Error Client_ID_regex-" + Client_ID_regex + "Id_regex-" + Id_regex;
                    int stattus = (int)CompanyInfo.InsertActivityLogDetails(msg, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Customer_ID, "GetEntertainmentVouchers", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "Membership", context);
                }
            }
            catch (Exception ex)
            {
                string msg = "Api GetEntertainmentVouchers: " + ex.ToString() + " ";
                CompanyInfo.InsertErrorLogTracker(msg, 0, 0, 0, 0, "GetEntertainmentVouchers", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "", context);
            }
            return dt;
        }
        
        public DataTable CustomerDetailsForHealthcare(Model.Customer obj, HttpContext context)
        {
            int Customer_ID = Convert.ToInt32(CompanyInfo.Decrypt(Convert.ToString(obj.Customer_ID), true));
            DataTable dt = new DataTable();
            try
            {
                List<Model.Customer> _lst = new List<Model.Customer>();
                string Client_ID_regex = validation.validate(Convert.ToString(obj.Client_ID), 0, 1, 1, 1, 1, 1, 1, 1, 1);
                string Id_regex = validation.validate(Convert.ToString(Customer_ID), 0, 1, 1, 1, 1, 1, 1, 1, 1);
                if (Client_ID_regex != "false" && Id_regex != "false")
                {
                    MySqlCommand _cmd = new MySqlCommand("getCustDetailsForHealthcare");
                    _cmd.CommandType = CommandType.StoredProcedure;
                    string _whereclause = " and cr.Client_ID=" + obj.Client_ID;
                    _whereclause = _whereclause + " and cr.Customer_ID=" + Customer_ID;

                    _cmd.Parameters.AddWithValue("_whereclause", _whereclause);
                    dt = db_connection.ExecuteQueryDataTableProcedure(_cmd);
                    if (dt.Rows.Count > 0)
                    {
                        //var context = System.Web.HttpContext.Current;
                        string URL = "";
                        DataTable dtc = CompanyInfo.get(obj.Client_ID, context);
                        if (dtc.Rows.Count > 0)
                        {
                            URL = Convert.ToString(dtc.Rows[0]["Company_URL_Admin"]);
                        }
                        foreach (DataRow row in dt.Rows)
                        {
                            if (row["Profile_Image"] != DBNull.Value && row["Profile_Image"].ToString() != "")
                            {
                                //string image_link = (context.Session["Company_URL_Admin"]).ToString() + row["Profile_Image"].ToString();
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
                    int stattus = (int)CompanyInfo.InsertActivityLogDetails(msg, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Customer_ID, "GetEntertainmentVouchers", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "Membership", context);
                }
            }
            catch (Exception ex)
            {
                string msg = "Api GetEntertainmentVouchers: " + ex.ToString() + " ";
                CompanyInfo.InsertErrorLogTracker(msg, 0, 0, 0, 0, "GetEntertainmentVouchers", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "", context);
            }
            return dt;
        }

        public DataTable Get_Points_Wallet_History(Model.Customer obj, HttpContext context)
        {
            DataTable dt_wal = new DataTable();
            try
            {
                int Customer_ID = Convert.ToInt32(CompanyInfo.Decrypt(Convert.ToString(obj.Customer_ID), true));

                string whereclause = " and pmt.Customer_ID=" + Customer_ID + " and pmt.Client_ID=" + obj.Client_ID;


                MySqlCommand _cmd = new MySqlCommand("Display_Point_Wallet_History");
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.Parameters.AddWithValue("_whereclause", whereclause);
                dt_wal = db_connection.ExecuteQueryDataTableProcedure(_cmd);

            }
            catch (Exception ex)
            {
                Model.ErrorLog objError = new Model.ErrorLog();
                objError.User = new Model.User();
                objError.Error = "srvMembership : Get_Points_Wallet_History ---" + ex.ToString();
                objError.Date = DateTime.Now;
                objError.User_ID = 1;
                objError.Client_ID = obj.Client_ID;
                objError.Function_Name = "Get_Points_Wallet_History ";
                Service.srvErrorLog srvError = new Service.srvErrorLog();
                srvError.Create(objError, context);
            }
            return dt_wal;
        }

        public DataTable GetBenefitsRedemptionHistory(Model.Customer obj, HttpContext context)
        {
            DataTable ds = new DataTable();
            try
            {
                int Customer_ID = Convert.ToInt32(CompanyInfo.Decrypt(Convert.ToString(obj.Customer_ID), true));

                string whereclause = " and brm.Customer_ID=" + Customer_ID;


                MySqlCommand _cmd = new MySqlCommand("sp_get_membership_data");
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.Parameters.AddWithValue("_whereclause", whereclause);
                _cmd.Parameters.AddWithValue("_flag", 0);
                ds = db_connection.ExecuteQueryDataTableProcedure(_cmd);

            }
            catch (Exception ex)
            {
                Model.ErrorLog objError = new Model.ErrorLog();
                objError.User = new Model.User();
                objError.Error = "srvMembership : GetBenefitsRedemptionHistory ---" + ex.ToString();
                objError.Date = DateTime.Now;
                objError.User_ID = 1;
                objError.Client_ID = obj.Client_ID;
                objError.Function_Name = "GetBenefitsRedemptionHistory ";
                Service.srvErrorLog srvError = new Service.srvErrorLog();
                srvError.Create(objError, context);
            }
            return ds;
        }
    }
}
