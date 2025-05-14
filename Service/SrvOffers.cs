using gudusoft.gsqlparser.Units;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using Microsoft.Ajax.Utilities;
using System.Net;

namespace Calyx_Solutions.Service
{
    public class SrvOffers
    {
        mts_connection _MTS = new mts_connection();
        //bind_spin_config
        public DataTable Get_offers(Model.Offer obj, HttpContext context)
        {
            string activity = "Get_offers: Step1";
            string Client_ID_regex = validation.validate(Convert.ToString(obj.Client_ID), 0, 1, 1, 1, 1, 1, 1, 1, 1);
            string Customer_Id = validation.validate(Convert.ToString(obj.Customer_Id), 0, 1, 1, 1, 1, 1, 1, 1, 1);
            DataTable dt = new DataTable();
            string Customer_Id1 = Convert.ToString(CompanyInfo.Decrypt1(Convert.ToString(obj.Customer_Id)));
            int Customer_Id11 = Convert.ToInt32(Customer_Id1);
            DateTime Date = new DateTime();
            DateTime current_date = Convert.ToDateTime(CompanyInfo.gettime(obj.Client_ID, Customer_Id1, 0, context));
            activity = activity + "Step2";
            try
            {
                activity = activity + "Step3";
                if (Client_ID_regex != "false" && Customer_Id1 != "false")
                {
                    activity = activity + "Step4";
                    MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("Get_offers");
                    _cmd.CommandType = CommandType.StoredProcedure;
                    _cmd.Parameters.AddWithValue("_transaction_id", obj.Transaction_ID);
                    _cmd.Parameters.AddWithValue("_base_currency_id", obj.Base_currency_id);
                    _cmd.Parameters.AddWithValue("_customer_id", Convert.ToInt32(Customer_Id1));
                    _cmd.Parameters.AddWithValue("_Client_id", obj.Client_ID);
                    _cmd.Parameters.AddWithValue("_Date", current_date);
                    dt = db_connection.ExecuteQueryDataTableProcedure(_cmd);
                    activity = activity + "Step5";
                    if (dt.Rows.Count > 0)
                    {
                        if (Convert.ToString(dt.Rows[0]["eligibility"]) != "Noteligible")
                        {

                            if (Convert.ToString(dt.Rows[0]["Offer_ID"]) == null || Convert.ToString(dt.Rows[0]["Offer_ID"]) == "" || Convert.ToString(dt.Rows[0]["Offer_ID"]) == "null" && Convert.ToString(dt.Rows[0]["Offer_ID"]) == "0")
                            {
                                // DataColumnCollection columns = dt.Columns; if (!columns.Contains("eligibility")) { dt.Columns.Add("eligibility"); DataRow dt_row = dt.NewRow(); dt_row["eligibility"] = "Noteligible"; }
                                dt = null;
                                activity = activity + "Step6";

                            }
                        }
                        if (Convert.ToString(dt.Rows[0]["eligibility"]) == "offer_available")
                        {
                            activity = activity + "Step7";
                        }
                        if (Convert.ToString(dt.Rows[0]["eligibility"]) == "Inactive")
                        {
                            dt.Rows[0]["eligibility"] = "Noteligible";
                            CompanyInfo.InsertActivityLogDetailsSecurity("Spin offer Permission is inactive.", Convert.ToInt32(obj.User_ID), (obj.Transaction_ID), Convert.ToInt32(obj.User_ID), (Customer_Id11), "srvOffers", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "Getoffers", 0, context);
                            activity = activity + "Step8";
                        }
                        else if (Convert.ToString(dt.Rows[0]["eligibility"]) == "eligible")
                        {
                            activity = activity + "Step9";
                            if (Convert.ToInt32(dt.Rows[0]["Currency_type"]) == 1)
                            {
                                activity = activity + "Step10";
                                if ((Convert.ToDouble(dt.Rows[0]["Qualify_amt"]) != -1 && Convert.ToDouble(dt.Rows[0]["AmountInGBP"]) < Convert.ToDouble(dt.Rows[0]["Qualify_amt"])) || (Convert.ToInt32(dt.Rows[0]["Transfer_count"]) != -1 && Convert.ToInt32(dt.Rows[0]["Transfer_count"]) >= Convert.ToInt32(dt.Rows[0]["count"])))
                                {
                                    if ((Convert.ToDouble(dt.Rows[0]["Qualify_amt"]) != -1 && Convert.ToDouble(dt.Rows[0]["AmountInGBP"]) < Convert.ToDouble(dt.Rows[0]["Qualify_amt"])))
                                    {
                                        CompanyInfo.InsertActivityLogDetailsSecurity("Customer is unable reach to qualify Base transaction amount.", Convert.ToInt32(obj.User_ID), (obj.Transaction_ID), Convert.ToInt32(obj.User_ID), (Customer_Id11), "srvOffers", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "Getoffers", 0, context);
                                    }
                                    if ((Convert.ToInt32(dt.Rows[0]["Transfer_count"]) != -1 && Convert.ToInt32(dt.Rows[0]["Transfer_count"]) >= Convert.ToInt32(dt.Rows[0]["count"])))
                                    {
                                        CompanyInfo.InsertActivityLogDetailsSecurity("Customer is unable reach to qualify transaction count.", Convert.ToInt32(obj.User_ID), (obj.Transaction_ID), Convert.ToInt32(obj.User_ID), (Customer_Id11), "srvOffers", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "Getoffers", 0, context);
                                    }
                                    DataColumnCollection columns = dt.Columns; if (!columns.Contains("eligibility")) { dt.Columns.Add("eligibility"); DataRow dt_row = dt.NewRow(); dt_row["eligibility"] = "Noteligible"; }
                                }
                            }
                            else if (Convert.ToInt32(dt.Rows[0]["Currency_type"]) == 2)
                            {
                                activity = activity + "Step11";
                                if ((Convert.ToDouble(dt.Rows[0]["Qualify_amt"]) != -1 && Convert.ToDouble(dt.Rows[0]["AmountInPKR"]) < Convert.ToDouble(dt.Rows[0]["Qualify_amt"])) || (Convert.ToInt32(dt.Rows[0]["Transfer_count"]) != -1 && Convert.ToInt32(dt.Rows[0]["Transfer_count"]) >= Convert.ToInt32(dt.Rows[0]["count"])))
                                    activity = activity + "Step12";
                                if ((Convert.ToDouble(dt.Rows[0]["Qualify_amt"]) != -1 && Convert.ToDouble(dt.Rows[0]["AmountInPKR"]) < Convert.ToDouble(dt.Rows[0]["Qualify_amt"])))
                                {
                                    activity = activity + "Step13";
                                    CompanyInfo.InsertActivityLogDetailsSecurity("Customer is unable reach to qualify payout transaction amount.", Convert.ToInt32(obj.User_ID), (obj.Transaction_ID), Convert.ToInt32(obj.User_ID), (Customer_Id11), "srvOffers", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "Getoffers", 0, context);
                                }
                                if ((Convert.ToInt32(dt.Rows[0]["Transfer_count"]) != -1 && Convert.ToInt32(dt.Rows[0]["Transfer_count"]) >= Convert.ToInt32(dt.Rows[0]["count"])))
                                {
                                    activity = activity + "Step14";
                                    CompanyInfo.InsertActivityLogDetailsSecurity("Customer is unable reach to qualify transaction count.", Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), 1, "srvOffers", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "Get_offers", 0, context);
                                }
                                activity = activity + "Step15";
                                { DataColumnCollection columns = dt.Columns; if (!columns.Contains("eligibility")) { dt.Columns.Add("eligibility"); DataRow dt_row = dt.NewRow(); dt_row["eligibility"] = "Noteligible"; activity = activity + "Step17"; } }
                                activity = activity + "Step18";
                            }
                        }

                        else
                        {
                            activity = activity + "Step19";
                            DataColumnCollection columns = dt.Columns; if (!columns.Contains("eligibility")) { dt.Columns.Add("eligibility"); DataRow dt_row = dt.NewRow(); dt_row["eligibility"] = "Noteligible"; activity = activity + "Step21"; }
                        }

                        if (Convert.ToString(dt.Rows[0]["eligibility"]) == "eligible")
                        {
                            activity = activity + "Step22";
                            MySqlConnector.MySqlCommand cmd = new MySqlConnector.MySqlCommand("Update_customer_Offer");
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("_Offer_amount", 0.00);
                            cmd.Parameters.AddWithValue("_Customer_ID", Customer_Id1);
                            cmd.Parameters.AddWithValue("_Record_date", current_date);
                            cmd.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                            cmd.Parameters.AddWithValue("_Offer_ID", Convert.ToInt32(dt.Rows[0]["Offer_ID"]));
                            cmd.Parameters.AddWithValue("_Transaction_ID", obj.Transaction_ID);
                            cmd.Parameters.AddWithValue("_Amount_type", 0);
                            cmd.Parameters.AddWithValue("_Currency_ID", Convert.ToString(dt.Rows[0]["Currency_Id"]));
                            cmd.Parameters.AddWithValue("_Currency_Type", Convert.ToString(dt.Rows[0]["Currency_type"]));
                            cmd.Parameters.AddWithValue("_flag1", 1);
                            cmd.Parameters.Add(new MySqlParameter("_Flag", MySqlDbType.Int32));
                            cmd.Parameters["_Flag"].Direction = ParameterDirection.Output;
                            db_connection.ExecuteNonQueryProcedure(cmd);
                            try
                            {
                                activity = activity + "Step22_00";
                                activity = activity + "Currency_type" + Convert.ToString(dt.Rows[0]["Currency_type"]) + "Currency_id" + Convert.ToString(dt.Rows[0]["Currency_Id"]) + "Transaction_ID" + obj.Transaction_ID + "Offer_ID" + Convert.ToInt32(dt.Rows[0]["Offer_ID"]) + "Client_ID" + obj.Client_ID + "Customer_Id1" + Customer_Id1 + "current_date" + current_date;
                            }
                            catch (Exception ex)
                            {
                                activity = activity + "Step22_01";
                            }
                            activity = activity + "Step23";
                            int flag = Convert.ToInt32(cmd.Parameters["_Flag"].Value);
                            if (flag >= 0) // insert
                            {
                                activity = activity + "Step24";
                                if (flag == 2)
                                {
                                    activity = activity + "Step25";
                                    int stattus = (int)CompanyInfo.InsertActivityLogDetailsSecurity("Offer against customer updated.", Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), 1, "srvOffers", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "GetOffers", 0, context);
                                }
                                if (flag == 3)
                                {
                                    activity = activity + "Step26";
                                    int stattus = (int)CompanyInfo.InsertActivityLogDetailsSecurity("Offer against customer added.", Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), 1, "srvOffers", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "GetOffers", 0, context);
                                }
                                if (flag != 2 && flag != 3)
                                {
                                    activity = activity + "Step27";
                                }

                            }
                        }

                    }
                }
                else
                {
                    activity = activity + "Step28";
                    string msg = "Validation Error Client_ID_regex- +" + Client_ID_regex;
                    int stattus1 = (int)CompanyInfo.InsertActivityLogDetailsSecurity(msg, Convert.ToInt32(obj.User_ID), (obj.Transaction_ID), Convert.ToInt32(obj.User_ID), (Customer_Id11), "srvOffers", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "Getoffers", 0, context);

                }
            }
            catch (Exception Ex)
            {
                activity = activity + "Step29";
                CompanyInfo.InsertErrorLogDetails(Ex.Message.Replace("\'", "\\'"), 0, "Getoffers", obj.Branch_ID, obj.Client_ID);
            }
            finally
            {
                activity = activity + "Step30";
                int stattus = (int)CompanyInfo.InsertActivityLogDetailsSecurity(activity, Convert.ToInt32(obj.User_ID), (obj.Transaction_ID), Convert.ToInt32(obj.User_ID), (Customer_Id11), "srvOffers", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "Getoffers", 0, context);

            }
            return dt;
        }

        public DataTable bind_spin_config(Model.Offer obj , HttpContext context)
        {
            string Client_ID_regex = validation.validate(Convert.ToString(obj.Client_ID), 0, 1, 1, 1, 1, 1, 1, 1, 1);
           // string Customer_Id = validation.validate(Convert.ToString(obj.Customer_Id), 0, 1, 1, 1, 1, 1, 1, 1, 1);
            DataTable dt = new DataTable();
            //string Customer_Id1 = Convert.ToString(CompanyInfo.Decrypt1(Convert.ToString(obj.Customer_Id)));
            if (Client_ID_regex != "false")
            {
                string _wherclause = "1=1";
                if (obj.Base_currency_id > 0) {
                    _wherclause = _wherclause + " and offer_map.Currency_ID = "+ obj.Base_currency_id;
                }
                MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("bind_spin_config");
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.Parameters.AddWithValue("_offer_id",obj.Offer_ID);
                _cmd.Parameters.AddWithValue("_wherclause", _wherclause);

                dt = db_connection.ExecuteQueryDataTableProcedure(_cmd);
                // select Sum(AmountInGBP) , count(Transaction_ID) from transaction_table;

            }
            else
            {
                string msg = "Validation Error Client_ID_regex- +" + Client_ID_regex;
                int stattus = (int)CompanyInfo.InsertActivityLogDetailsSecurity(msg, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), 1, "srvOffers", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "CollectionTypeConfig", 0 , context );
            }
            return dt;
        }
        
        public DataTable Get_wallet_configuration(Model.Offer obj , HttpContext context)
        {
            string Client_ID_regex = validation.validate(Convert.ToString(obj.Client_ID), 0, 1, 1, 1, 1, 1, 1, 1, 1);
           // string Customer_Id = validation.validate(Convert.ToString(obj.Customer_Id), 0, 1, 1, 1, 1, 1, 1, 1, 1);
            
            
            DataTable dt = new DataTable();
            DataTable dt_wallet = new DataTable();
            //string Customer_Id1 = Convert.ToString(CompanyInfo.Decrypt1(Convert.ToString(obj.Customer_Id)));
            if (Client_ID_regex != "false")
            {
                string currency_ids = obj.Base_currency_code;
                string where = "";
               
                DataTable dt_wallet_cur = new DataTable();
                if (obj.Transaction_ID>0)
                {
                    String query2 = "SELECT cm1.currency_id as Base_currency_id ,cm2.currency_id as Foreign_currency_id  FROM  transaction_table tt LEFT JOIN currency_master cm1 ON tt.FromCurrency_Code = cm1.currency_code LEFT JOIN currency_master cm2 ON tt.Currency_Code = cm2.currency_code";
                    MySqlConnector.MySqlCommand _cmd2 = new MySqlConnector.MySqlCommand("Default_sp");
                    _cmd2.CommandType = CommandType.StoredProcedure;
                    _cmd2.Parameters.AddWithValue("_Query", query2);
                    dt_wallet_cur = db_connection.ExecuteQueryDataTableProcedure(_cmd2);
                    if (dt_wallet_cur.Rows.Count > 0)
                    {
                        obj.Foreign_currency_id = Convert.ToInt32(dt_wallet_cur.Rows[0]["Foreign_currency_id"]);
                        obj.Base_currency_id = Convert.ToInt32(dt_wallet_cur.Rows[0]["Base_currency_id"]);
                    }
                }
                if (obj.Base_currency_id > 0)
                {
                    where = where + " and  (Currency_Id = " + Convert.ToInt32(obj.Base_currency_id) + " AND Currency_type = 1)";
                }
                if (obj.Foreign_currency_id > 0)
                {
                    if (where == "" || where == null)
                    {
                        where = where + " and  (Currency_Id = " + Convert.ToInt32(obj.Foreign_currency_id) + " AND Currency_type = 2)";
                    }
                    else if (where != "" && where != null)
                    {
                        where = where + " or  (Currency_Id = " + Convert.ToInt32(obj.Foreign_currency_id) + " AND Currency_type = 2)";
                    }
                }

                string query1 = "select * from customer_wallet_usage_limit where delete_status=0 "+ where;
                MySqlConnector.MySqlCommand _cmd1 = new MySqlConnector.MySqlCommand("Default_sp");
                _cmd1.CommandType = CommandType.StoredProcedure;
                _cmd1.Parameters.AddWithValue("_Query", query1);
                dt_wallet = db_connection.ExecuteQueryDataTableProcedure(_cmd1);
                
            }
            else
            {
                string msg = "Validation Error Client_ID_regex- +" + Client_ID_regex;
                int stattus = (int)CompanyInfo.InsertActivityLogDetailsSecurity(msg, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), 1, "srvOffers", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "CollectionTypeConfig", 0 , context);
            }
            return dt_wallet;
        }

        public Model.Offer check_spin_limit(Model.Offer obj , HttpContext context)
        {
            string activity = "Spin the wheel :- step1";
            string Customer_Id12 = Convert.ToString(CompanyInfo.Decrypt1(Convert.ToString(obj.Customer_Id)));
            try
            {

                string Client_ID_regex = validation.validate(Convert.ToString(obj.Client_ID), 0, 1, 1, 1, 1, 1, 1, 1, 1);
                // string Customer_Id = validation.validate(Convert.ToString(obj.Customer_Id), 0, 1, 1, 1, 1, 1, 1, 1, 1);
                DataTable dt = new DataTable();

                string Customer_Id1 = Convert.ToString(CompanyInfo.Decrypt1(Convert.ToString(obj.Customer_Id)));
                DateTime current_date = Convert.ToDateTime(CompanyInfo.gettime(obj.Client_ID, Customer_Id1, 0, context));
                if (Client_ID_regex != "false")
                {
                    activity = activity + " step2";
                    DateTime Date = new DateTime();
                    //Check transaction offer added or not against txn
                    MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("Default_sp");
                    _cmd.CommandType = CommandType.StoredProcedure;
                    _cmd.Parameters.AddWithValue("_Query", "select STR_TO_DATE(offer_customer_mapping_table.Record_date,'%Y-%m-%d') as Record_date,offer_customer_mapping_table.*,omt.*,  DATE(omt.Record_date) + INTERVAL omt.Spin_wheel_validity DAY AS spin_validity_date from offer_customer_mapping_table join offer_mapping_table omt where omt.Currency_ID=" + obj.Base_currency_id + " and transaction_id = " + obj.Transaction_ID);
                    DataTable dt_cust_map = db_connection.ExecuteQueryDataTableProcedure(_cmd);
                    activity = activity + " step3";
                    DataTable dt_txn = new DataTable();
                    if (dt_cust_map.Rows.Count > 0) //if yes then proceed
                    {
                        activity = activity + " step4";
                        DateTime spin_validity_date = Convert.ToDateTime(dt_cust_map.Rows[0]["Record_date"]).AddDays(Convert.ToInt32(dt_cust_map.Rows[0]["Spin_wheel_validity"]));

                        if (Convert.ToInt32(dt_cust_map.Rows[0]["Usage_flag"]) > 0)// check wheather already txn spinned
                        {
                            //check customer's spin lmt 
                            MySqlConnector.MySqlCommand _cmd_lmt = new MySqlConnector.MySqlCommand("check_customer_spin_lmt");
                            _cmd_lmt.CommandType = CommandType.StoredProcedure;
                            _cmd_lmt.Parameters.AddWithValue("_Transaction_ID", obj.Transaction_ID);
                            _cmd_lmt.Parameters.AddWithValue("_Customer_ID", Convert.ToInt32(Customer_Id1));
                            _cmd_lmt.Parameters.AddWithValue("_todays_Date", current_date.ToString("yyyy-MM-dd HH:mm:ss"));
                            _cmd_lmt.Parameters.AddWithValue("_Offer_ID", obj.Offer_ID);
                            _cmd_lmt.Parameters.AddWithValue("_Base_Currency_id", obj.Base_currency_id);
                            _cmd_lmt.Parameters.AddWithValue("_Foreign_currency_id", obj.Currency_ID);
                            DataTable dt_spin_check = db_connection.ExecuteQueryDataTableProcedure(_cmd_lmt);
                            //check how many time customer is winner within specific days limit 
                            if (dt_spin_check.Rows.Count > 0)
                            {
                                //if yes then show try again
                                if (Convert.ToInt32(dt_spin_check.Rows[0]["No_time_winner1"]) <= Convert.ToInt32(dt_spin_check.Rows.Count))

                                {
                                    activity = activity + " step0";

                                    obj.Spin_amount = 0.00;
                                    obj.Spin_Id = 0;
                                    obj.Spin_lable = "Try Again";
                                    obj.Spin_result = "lose";
                                    CompanyInfo.InsertActivityLogDetails("Customer already won offer amount for the today. Spin result: " + obj.Spin_result, obj.User_ID, obj.Transaction_ID, obj.User_ID, Convert.ToInt32(Customer_Id12), "App-check_spin_limit", 2, 1, "check_spin_limit", context);
                                    defaul_try_staus(obj, context);
                                    activity = activity + " step01";
                                    return obj;
                                }
                            }

                            //check spin use validity days if it grater than current date then proceed
                            if (spin_validity_date.Date > (current_date.Date))
                            {
                                activity = activity + " step5";
                                //bind offermapping data
                                MySqlConnector.MySqlCommand _cmd1 = new MySqlConnector.MySqlCommand("check_spin_limit");
                                _cmd1.CommandType = CommandType.StoredProcedure;
                                _cmd1.Parameters.AddWithValue("_offer_id", obj.Offer_ID);
                                _cmd1.Parameters.AddWithValue("_flag", 1);
                                _cmd1.Parameters.AddWithValue("_Date", current_date);
                                _cmd1.Parameters.AddWithValue("_Currency_id", obj.Base_currency_id);
                                dt = db_connection.ExecuteQueryDataTableProcedure(_cmd1);
                                // select Sum(AmountInGBP) , count(Transaction_ID) from transaction_table;
                                activity = activity + " step6";
                                if (dt.Rows.Count > 0)
                                {
                                    //bind offermapping data
                                    activity = activity + " step7";
                                    //check daily and jackpot limit 
                                    if (Convert.ToDouble(dt.Rows[0]["Daily_Amount"]) + Convert.ToDouble(dt.Rows[0]["Jackpot_price"]) >= Convert.ToDouble(dt.Rows[0]["Total_offer_amount"]))
                                    {
                                        activity = activity + " step8";
                                        if (Convert.ToInt32(dt.Rows[0]["Currency_type"]) == 1)// if cur type = 1 then check for base currency
                                        {
                                            activity = activity + " step9";


                                        }
                                        else if (Convert.ToInt32(dt.Rows[0]["Currency_type"]) == 2)// if cur type = 2 then check for payout currency
                                        {
                                            activity = activity + " step10";
                                            string whereclause = "1=1";
                                            if (obj.Base_currency_id > 0)
                                            {
                                                activity = activity + " step10_00";

                                                whereclause = whereclause + " and offer_map.Currency_Id = " + obj.Base_currency_id;
                                            }
                                            double remaining_amount = (Convert.ToDouble(dt.Rows[0]["Daily_Amount"]) + Convert.ToDouble(dt.Rows[0]["Jackpot_price"])) - Convert.ToDouble(dt.Rows[0]["Total_offer_amount"]);
                                            activity = activity + " step10_01";
                                            //check daily and jackpot limit with total customer's winning amount for current date 
                                            if (remaining_amount > 0) // if it is grater then check
                                            {
                                                activity = activity + " step10_02";
                                                if (remaining_amount > 0)
                                                {
                                                    activity = activity + " step10_03";
                                                    //check daily amount limit with amt type =1(regular winner) total customer's winning amount for current date  
                                                    if (Convert.ToDouble(dt.Rows[0]["Daily_Amount"]) > Convert.ToDouble(dt.Rows[0]["Daily_cust_amount_limit"]))
                                                    {
                                                        activity = activity + " step10_04";
                                                        remaining_amount = Convert.ToDouble(dt.Rows[0]["Daily_Amount"]) - Convert.ToDouble(dt.Rows[0]["Daily_cust_amount_limit"]);
                                                        whereclause = whereclause + " and  (Amount <= " + remaining_amount + " or Amount_type = 2)  and Amount!=0";
                                                    }
                                                    else
                                                    {
                                                        //check jackpot
                                                        activity = activity + " step10_05";
                                                        whereclause = whereclause + " and  ((Amount_type = 2)  or Amount=0)";
                                                    }

                                                }
                                                activity = activity + " step10_05" + whereclause;
                                                //bind spin data
                                                MySqlConnector.MySqlCommand _cmd3 = new MySqlConnector.MySqlCommand("bind_spin_config");
                                                _cmd3.CommandType = CommandType.StoredProcedure;
                                                _cmd3.Parameters.AddWithValue("_offer_id", obj.Offer_ID);
                                                _cmd3.Parameters.AddWithValue("_wherclause", whereclause);
                                                dt = db_connection.ExecuteQueryDataTableProcedure(_cmd3);
                                                activity = activity + " step11";
                                                int randomNumber = 0;
                                                if (dt.Rows.Count > 0)
                                                {
                                                    //create random number
                                                    activity = activity + " step12";
                                                    obj.Spin_currency_code = Convert.ToString(dt.Rows[0]["Currency_Code"]);
                                                    Random random = new Random();
                                                    // Define the range
                                                    int minNumber = Convert.ToInt32(1);
                                                    int maxNumber = Convert.ToInt32(dt.Rows.Count - 1);
                                                    // Generate a random number
                                                    if (obj.Old_picked.Contains(randomNumber))
                                                    {
                                                        randomNumber = random.Next(minNumber, maxNumber);
                                                    }
                                                    else
                                                    {
                                                        randomNumber = random.Next(minNumber, maxNumber);
                                                    }
                                                    int spin_id = Convert.ToInt32(dt.Rows[randomNumber]["Id"]);
                                                    double spin_amount = Convert.ToDouble(dt.Rows[randomNumber]["Amount"]);
                                                    string spin_lable = Convert.ToString(dt.Rows[randomNumber]["lable"]);
                                                    string spin_type = Convert.ToString(dt.Rows[randomNumber]["Amount_type"]);
                                                    activity = activity + " step13";
                                                    if (spin_type == "2")// check if its jackpot
                                                    {
                                                        //check jackpot already winner or not within week
                                                        activity = activity + " step14";
                                                        _cmd1 = new MySqlConnector.MySqlCommand("check_spin_limit");
                                                        _cmd1.CommandType = CommandType.StoredProcedure;
                                                        _cmd1.Parameters.AddWithValue("_offer_id", obj.Offer_ID);
                                                        _cmd1.Parameters.AddWithValue("_flag", 2);
                                                        _cmd1.Parameters.AddWithValue("_Date", DateTime.Now);
                                                        _cmd1.Parameters.AddWithValue("_Currency_id", obj.Base_currency_id);
                                                        DataTable dt_jackpot = db_connection.ExecuteQueryDataTableProcedure(_cmd1);
                                                        activity = activity + " step15";
                                                        if (dt_jackpot.Rows.Count > 0) // if yes then check within different spin sections
                                                        {
                                                            CompanyInfo.InsertActivityLogDetails("Already jackpot winner for the current week.", obj.User_ID, obj.Transaction_ID, obj.User_ID, Convert.ToInt32(Customer_Id12), "App-check_spin_limit", 2, 1, "check_spin_limit", context);
                                                            activity = activity + " step16";
                                                            DataRow[] rows = dt.Select("Id = " + spin_id);
                                                            activity = activity + " step17";
                                                            if (rows.Length > 1)
                                                            {
                                                                activity = activity + " step18";
                                                                rows[0].Delete();  // Mark the row for deletion
                                                                dt.AcceptChanges();  // Commit the deletion
                                                                minNumber = Convert.ToInt32(1);
                                                                maxNumber = Convert.ToInt32(dt.Rows.Count - 1);
                                                                // Generate a random number
                                                                if (obj.Old_picked.Contains(randomNumber))
                                                                {
                                                                    randomNumber = random.Next(minNumber, maxNumber);
                                                                }
                                                                else
                                                                {
                                                                    randomNumber = random.Next(minNumber, maxNumber);
                                                                }
                                                                activity = activity + " step19";
                                                                spin_id = Convert.ToInt32(dt.Rows[randomNumber]["Id"]);
                                                                spin_amount = Convert.ToDouble(dt.Rows[randomNumber]["Amount"]);
                                                                spin_lable = Convert.ToString(dt.Rows[randomNumber]["lable"]);
                                                                spin_type = Convert.ToString(dt.Rows[randomNumber]["Amount_type"]);
                                                                activity = activity + " step20";
                                                            }

                                                            else
                                                            {
                                                                spin_id = Convert.ToInt32(dt.Rows[0]["Id"]);
                                                                spin_amount = Convert.ToDouble(dt.Rows[0]["Amount"]);
                                                                spin_lable = Convert.ToString(dt.Rows[0]["lable"]);
                                                                spin_type = Convert.ToString(dt.Rows[0]["Amount_type"]);
                                                                activity = activity + " step20";
                                                            }
                                                        }
                                                    }
                                                    if (spin_type == "1" && (spin_amount == 0 || spin_amount == 0.00))
                                                    {
                                                        obj.Spin_result = "lose";
                                                    }
                                                    if (spin_type == "1" && spin_amount > 0)
                                                    {
                                                        obj.Spin_result = "win";
                                                    }
                                                    if (spin_type == "2" && spin_amount > 0)
                                                    {
                                                        obj.Spin_result = "jackpot";
                                                    }
                                                    activity = activity + " step21";
                                                    string spin_result = Convert.ToString(dt.Rows[randomNumber]["lable"]);
                                                    activity = activity + " step22";
                                                    using (MySqlConnection con = new MySqlConnection(_MTS.WebConnSetting()))
                                                    {
                                                        activity = activity + " step23";
                                                        MySqlTransaction transaction;
                                                        if (con.State == ConnectionState.Closed)
                                                        {
                                                            con.Open();
                                                        }
                                                        transaction = con.BeginTransaction(IsolationLevel.ReadCommitted);
                                                        try
                                                        {
                                                            activity = activity + " step25";
                                                            //update offer against transaction id
                                                            MySqlCommand cmd = new MySqlCommand("Update_customer_Offer", con);
                                                            cmd.CommandType = CommandType.StoredProcedure;
                                                            cmd.Parameters.AddWithValue("_Offer_amount", spin_amount);
                                                            cmd.Parameters.AddWithValue("_Customer_ID", Customer_Id1);
                                                            cmd.Parameters.AddWithValue("_Record_date", current_date);
                                                            cmd.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                                                            cmd.Parameters.AddWithValue("_Offer_ID", obj.Offer_ID);
                                                            cmd.Parameters.AddWithValue("_Transaction_ID", obj.Transaction_ID);
                                                            cmd.Parameters.AddWithValue("_Amount_type", spin_type);
                                                            cmd.Parameters.AddWithValue("_Currency_ID", obj.Base_currency_id);
                                                            cmd.Parameters.AddWithValue("_Currency_Type", obj.Wallet_currency_type);
                                                            cmd.Parameters.AddWithValue("_flag1", 0);
                                                            cmd.Parameters.Add(new MySqlParameter("_Flag", MySqlDbType.Int32));
                                                            cmd.Parameters["_Flag"].Direction = ParameterDirection.Output;
                                                            cmd.ExecuteNonQuery();
                                                            cmd.Dispose();
                                                            transaction.Commit();
                                                            try
                                                            {
                                                                activity = activity + "Wallet_currency_type" + obj.Wallet_currency_type + "Base cure" + obj.Base_currency_id + "spin_type" + spin_type + "Offer_ID" + obj.Offer_ID + "spin_amount" + spin_amount + "Customer_Id1" + Customer_Id1 + " current_date" + current_date;
                                                            }
                                                            catch (Exception ex) { activity = activity + "exception" + ex.ToString(); }
                                                            int flag = Convert.ToInt32(cmd.Parameters["_Flag"].Value);
                                                            activity = activity + " step26" + flag;
                                                            if (flag == 1) // insert
                                                            {
                                                                activity = activity + " step27";
                                                                //CompanyInfo.InsertActivityLogDetails("Spin Amount " + spin_amount + " successfully added for customer for offer id " + obj.Offer_ID, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(obj.Customer_Id), "update_customer_offer", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "update_customer_offer");
                                                                obj.Spin_Id = spin_id;
                                                                obj.Spin_amount = spin_amount;
                                                                obj.Spin_lable = spin_lable;
                                                                CompanyInfo.InsertActivityLogDetails("Spin offer result: " + obj.Spin_result + " customer's offer amount is " + obj.Spin_lable, obj.User_ID, obj.Transaction_ID, obj.User_ID, Convert.ToInt32(Customer_Id12), "App-check_spin_limit", 2, 1, "check_spin_limit", context);
                                                                if (obj.Spin_amount > 0)
                                                                {
                                                                    obj.Wallet_on_deposit = Offer_wallet_reward(obj, context);
                                                                }
                                                            }
                                                            else
                                                            {
                                                                activity = activity + " step27_00";
                                                                CompanyInfo.InsertActivityLogDetails("unable to update spin offer result: " + obj.Spin_result + " customer's offer amount is " + obj.Spin_lable, obj.User_ID, obj.Transaction_ID, obj.User_ID, Convert.ToInt32(Customer_Id12), "App-check_spin_limit", 2, 1, "check_spin_limit", context);
                                                                obj.Spin_amount = 0.00;
                                                                obj.Spin_Id = 0;
                                                                obj.Spin_lable = "Try Again";
                                                                obj.Spin_result = "lose";
                                                                activity = activity + " step27_01";
                                                                defaul_try_staus(obj, context);
                                                                CompanyInfo.InsertActivityLogDetails("New spin offer result: " + obj.Spin_result + " customer's offer amount is " + obj.Spin_lable, obj.User_ID, obj.Transaction_ID, obj.User_ID, Convert.ToInt32(Customer_Id12), "App-check_spin_limit", 2, 1, "check_spin_limit", context);
                                                                activity = activity + " step27_02";
                                                            }

                                                        }
                                                        catch (Exception ex)
                                                        {
                                                            transaction.Rollback();
                                                            activity = activity + " step28";
                                                            CompanyInfo.InsertErrorLogDetails(ex.Message.Replace("\'", "\\'"), 0, "check_spin_limit", obj.Branch_ID, obj.Client_ID);
                                                            obj.Spin_amount = 0.00;
                                                            obj.Spin_Id = 0;
                                                            obj.Spin_lable = "Try Again";
                                                            obj.Spin_result = "lose";
                                                            activity = activity + " step02";
                                                            defaul_try_staus(obj, context);
                                                            activity = activity + " step03";
                                                            CompanyInfo.InsertActivityLogDetails("Something went wrong and the spin result: " + obj.Spin_result, obj.User_ID, obj.Transaction_ID, obj.User_ID, Convert.ToInt32(Customer_Id12), "App-check_spin_limit", 2, 1, "check_spin_limit", context);
                                                        }
                                                    }
                                                }
                                                else
                                                {

                                                    activity = activity + " step29";
                                                    obj.Spin_amount = 0.00;
                                                    obj.Spin_Id = 0;
                                                    obj.Spin_lable = "Try Again";
                                                    obj.Spin_result = "lose";
                                                    activity = activity + " step04";
                                                    defaul_try_staus(obj, context);
                                                    activity = activity + " step05";
                                                    string msg = "Failed to bind spin amounts so unable to add wallet to customer " + obj.Customer_Id + " for Transaction Id " + obj.Transaction_ID;
                                                    CompanyInfo.InsertActivityLogDetails(msg, obj.User_ID, obj.Transaction_ID, obj.User_ID, Convert.ToInt32(Customer_Id12), "App-check_spin_limit", 2, 1, "check_spin_limit", context);
                                                }
                                            }
                                            else
                                            {

                                                activity = activity + " step29";
                                                obj.Spin_amount = 0.00;
                                                obj.Spin_Id = 0;
                                                obj.Spin_lable = "Try Again";
                                                obj.Spin_result = "lose";
                                                activity = activity + " step04";
                                                defaul_try_staus(obj, context);
                                                activity = activity + " step05";
                                                string msg = "Failed to bind spin amounts so unable to add wallet to customer " + obj.Customer_Id + " for Transaction Id " + obj.Transaction_ID;
                                                CompanyInfo.InsertActivityLogDetails(msg, obj.User_ID, obj.Transaction_ID, obj.User_ID, Convert.ToInt32(Customer_Id12), "App-check_spin_limit", 2, 1, "check_spin_limit", context);
                                            }
                                        }

                                    }
                                    else
                                    {
                                        activity = activity + " step30";
                                        obj.Spin_amount = 0.00;
                                        obj.Spin_Id = 0;
                                        obj.Spin_lable = "Try Again";
                                        obj.Spin_result = "lose";
                                        activity = activity + " step06";
                                        defaul_try_staus(obj, context);
                                        activity = activity + " step07";
                                        string msg = "Offer Daily limit Exceeded so unable to add wallet to customer " + obj.Customer_Id + " for Transaction Id " + obj.Transaction_ID;
                                        CompanyInfo.InsertActivityLogDetails(msg, obj.User_ID, obj.Transaction_ID, obj.User_ID, Convert.ToInt32(Customer_Id12), "App-check_spin_limit", 2, 1, "check_spin_limit", context);

                                    }
                                }
                            }
                            else
                            {
                                activity = activity + " step34";
                                obj.Spin_amount = 0.00;
                                obj.Spin_Id = 0;
                                obj.Spin_lable = "Try Again";
                                obj.Spin_result = "lose";
                                activity = activity + " step07";
                                defaul_try_staus(obj, context);
                                activity = activity + " step08";
                                CompanyInfo.InsertActivityLogDetails("Spin validity date exceeds. Spin result: " + obj.Spin_result, obj.User_ID, obj.Transaction_ID, obj.User_ID, Convert.ToInt32(Customer_Id12), "App-check_spin_limit", 2, 1, "check_spin_limit", context);

                            }
                        }
                        else
                        {
                            activity = activity + " step31";
                            string whereclause = " Amount= " + dt_cust_map.Rows[0]["Offer_amont"];
                            if (obj.Base_currency_id > 0)
                            {
                                whereclause = whereclause + " and offer_map.Currency_Id = " + obj.Base_currency_id;
                            }
                            MySqlConnector.MySqlCommand _cmd3 = new MySqlConnector.MySqlCommand("bind_spin_config");
                            _cmd3.CommandType = CommandType.StoredProcedure;
                            _cmd3.Parameters.AddWithValue("_offer_id", obj.Offer_ID);
                            _cmd3.Parameters.AddWithValue("_wherclause", whereclause);
                            dt = db_connection.ExecuteQueryDataTableProcedure(_cmd3);
                            if (dt.Rows.Count > 0)
                            {
                                activity = activity + " step32";
                                obj.Spin_Id = Convert.ToInt32(dt.Rows[0]["Id"]);
                                obj.Spin_amount = Convert.ToDouble(dt.Rows[0]["Amount"]);
                                obj.Spin_lable = Convert.ToString(dt.Rows[0]["lable"]);
                                string spin_type = Convert.ToString(dt.Rows[0]["Amount_type"]);
                                if (spin_type == "1" && (obj.Spin_amount == 0 || obj.Spin_amount == 0.00))
                                {
                                    obj.Spin_result = "lose";
                                }
                                if (spin_type == "1" && obj.Spin_amount > 0)
                                {
                                    obj.Spin_result = "win";
                                }
                                if (spin_type == "2" && obj.Spin_amount > 0)
                                {
                                    obj.Spin_result = "jackpot";
                                }
                                activity = activity + " step33";
                                CompanyInfo.InsertActivityLogDetails("Wheel already spinned and the old result is Spin result: " + obj.Spin_result, obj.User_ID, obj.Transaction_ID, obj.User_ID, Convert.ToInt32(Customer_Id12), "App-check_spin_limit", 2, 1, "check_spin_limit", context);

                            }
                            else
                            {
                                activity = activity + " step34";
                                obj.Spin_amount = 0.00;
                                obj.Spin_Id = 0;
                                obj.Spin_lable = "Try Again";
                                obj.Spin_result = "lose";
                                activity = activity + " step35";
                                activity = activity + " step09";
                                defaul_try_staus(obj, context);
                                activity = activity + " step010";
                            }
                            string msg = "Spin offer Amount already added for this transaction so unable to add wallet to customer " + obj.Customer_Id + " for Transaction Id " + obj.Transaction_ID;
                            CompanyInfo.InsertActivityLogDetails(msg, obj.User_ID, obj.Transaction_ID, obj.User_ID, Convert.ToInt32(Customer_Id12), "App-check_spin_limit", 2, 1, "check_spin_limit", context);

                        }
                    }
                    else
                    {
                        activity = activity + " step36";
                        obj.Spin_amount = 0.00;
                        obj.Spin_Id = 0;
                        obj.Spin_lable = "Try Again";
                        obj.Spin_result = "lose";

                        CompanyInfo.InsertActivityLogDetails("No records found against customer and transaction", obj.User_ID, obj.Transaction_ID, obj.User_ID, Convert.ToInt32(Customer_Id12), "App-check_spin_limit", 2, 1, "check_spin_limit", context);

                    }
                }
                else
                {
                    activity = activity + " step37";
                    string msg = "Validation Error Client_ID_regex- +" + Client_ID_regex;
                    CompanyInfo.InsertActivityLogDetails(msg, obj.User_ID, obj.Transaction_ID, obj.User_ID, Convert.ToInt32(Customer_Id12), "App-check_spin_limit", 2, 1, "check_spin_limit", context);
                }

            }
            catch (Exception e)
            {
                activity = activity + " step38";
                CompanyInfo.InsertErrorLogDetails(e.ToString(), 0, "check_spin_limit", obj.Branch_ID, obj.Client_ID);
            }
            finally
            {

                activity = activity + " step39";
                CompanyInfo.InsertActivityLogDetails(activity, obj.User_ID, obj.Transaction_ID, obj.User_ID, Convert.ToInt32(Customer_Id12), "App-check_spin_limit", 2, 1, "check_spin_limit", context);
            }
            return obj;
        }

        public DataTable Check_offer(Model.Offer obj)
        {
            DataTable dt_spin_offer = new DataTable();
            HttpContext context = null;
            Model.Offer _obj = new Model.Offer();

            string Customer_Id12 = Convert.ToString(CompanyInfo.Decrypt1(Convert.ToString(obj.Customer_Id)));
            try
            {
                string Client_ID_regex = validation.validate(Convert.ToString(obj.Client_ID), 0, 1, 1, 1, 1, 1, 1, 1, 1);
                // string Customer_Id = validation.validate(Convert.ToString(obj.Customer_Id), 0, 1, 1, 1, 1, 1, 1, 1, 1);
                DataTable dt = new DataTable();

                string Customer_Id1 = Convert.ToString(CompanyInfo.Decrypt1(Convert.ToString(obj.Customer_Id)));
                DateTime current_date = Convert.ToDateTime(CompanyInfo.gettime(obj.Client_ID, Customer_Id1, 0, context));
                if (Client_ID_regex != "false")
                {
                    MySqlConnector.MySqlCommand _cmd_off = new MySqlConnector.MySqlCommand("Check_offer_result");
                    _cmd_off.CommandType = CommandType.StoredProcedure;
                    _cmd_off.Parameters.AddWithValue("_transaction_id", obj.Transaction_ID);
                    dt_spin_offer = db_connection.ExecuteQueryDataTableProcedure(_cmd_off);
                }

            }
            catch (Exception e)
            {
                CompanyInfo.InsertErrorLogDetails(e.ToString(), 0, "check_spin_limit", obj.Branch_ID, obj.Client_ID);
            }
            finally
            {
                // CompanyInfo.InsertActivityLogDetails(activity, obj.User_ID, obj.Transaction_ID, obj.User_ID, Convert.ToInt32(""), "App-check_spin_limit", 2, 1, "check_spin_limit");
            }
            return dt_spin_offer;
        }

        protected string defaul_try_staus(Model.Offer obj , HttpContext context) {
            string status = "";
            string Customer_Id12 = Convert.ToString(CompanyInfo.Decrypt1(Convert.ToString(obj.Customer_Id)));
            try
            {
                MySqlConnector.MySqlCommand _cmd_reset = new MySqlConnector.MySqlCommand("Default_sp");
                _cmd_reset.CommandType = CommandType.StoredProcedure;
                _cmd_reset.Parameters.AddWithValue("_Query", "update offer_customer_mapping_table omt inner join transaction_mapping_table tmt on tmt.transaction_id= omt.transaction_id set Spin_offer_status= 0, Amount_Type=1, Usage_flag=0, Offer_amont= "+obj.Spin_amount+" where omt.transaction_id ="+obj.Transaction_ID+";");
                string ii = Convert.ToString(db_connection.ExecuteNonQueryProcedure(_cmd_reset));
                CompanyInfo.InsertActivityLogDetails("Customer already winner for the day", obj.User_ID, obj.Transaction_ID, obj.User_ID, Convert.ToInt32(Customer_Id12), "App-check_spin_limit", 2, 1, "check_spin_limit" , context);

            }
            catch (Exception e_set)
            {
                CompanyInfo.InsertErrorLogDetails(e_set.ToString(), 0, "check_spin_limit", obj.Branch_ID, obj.Client_ID);

            }
            return status;
        }

        protected int Offer_wallet_reward(Model.Offer obj, HttpContext context)
        {
            int offer_val = 0;
            string whereclause = "App-Offer_wallet_reward start:";
            DataTable dt = new DataTable();
            //double amountAfterOffer = 0;
            whereclause = whereclause + " step1 ";
            var activity = "";
            int transactionID = obj.Transaction_ID;
            int customerID = Convert.ToInt32(CompanyInfo.Decrypt1(Convert.ToString(obj.Customer_Id)));
            int Branch_ID = obj.Branch_ID;
            int Client_ID = obj.Client_ID;

            try
            {
                whereclause = whereclause + " step2 ";
                int Currency_id = 0;
                int Base_crrency_id = 0;
                #region add offer to Wallet
                //add amount to wallet

                whereclause = whereclause + " step3 ";
                MySqlConnector.MySqlCommand cmd1 = new MySqlConnector.MySqlCommand("select tt.TransactionStatus_ID, tt.ReferenceNo,cr.First_Name,tt.FromCurrency_Code,tt.Currency_Code,cr.WireTransfer_ReferanceNo,cr.Customer_Id,cr.Email_ID,tt.Customer_ID,tt.Transaction_ID from transaction_table tt join customer_registration cr on cr.Customer_id=tt.Customer_ID where TransactionStatus_ID=3 and tt.Transaction_ID=" + obj.Transaction_ID);
                cmd1.CommandType = CommandType.Text;
                DataTable get_Details = new DataTable();
                get_Details = (DataTable)db_connection.ExecuteQueryDataTableProcedure(cmd1);
                if (get_Details.Rows.Count > 0)
                {
                    if (Convert.ToInt32(get_Details.Rows[0]["TransactionStatus_ID"]) == 3)
                    {
                        //get_Details.Load(cmd1.ExecuteReader());//(DataTable)(db_connection.ExecuteScalarProcedure(cmd1));

                        whereclause = whereclause + " step4 ";
                        //if (Convert.ToInt32(get_Details.Rows[0]["Spin_offer_status"]) == 0) {
                        MySqlConnector.MySqlCommand cmd = new MySqlConnector.MySqlCommand("Default_sp");
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("_Query", "select cust_om.*,om.*,tmt.Spin_offer_status from offer_customer_mapping_table  cust_om inner join offer_master om on om.Offer_id=cust_om.Offer_ID left join transaction_mapping_table tmt on tmt.Transaction_id=cust_om.Transaction_id and cust_om.delete_Status=0 where tmt.Transaction_id=" + transactionID);
                        DataTable dtt = db_connection.ExecuteQueryDataTableProcedure(cmd);

                        if (dtt.Rows.Count > 0)
                        {
                            whereclause = whereclause + " step5 ";
                            string txn_ref = Convert.ToString(get_Details.Rows[0]["ReferenceNo"]);
                            string Customer_ID = Convert.ToString(dtt.Rows[0]["Customer_ID"]);
                            string Transaction_ID = Convert.ToString(dtt.Rows[0]["Transaction_ID"]);
                            string First_Name = Convert.ToString(get_Details.Rows[0]["First_Name"]);
                            if (Convert.ToInt32(dtt.Rows[0]["Spin_offer_status"]) == 0)
                            {
                                whereclause = whereclause + " step6 ";
                                if (Convert.ToDouble(dtt.Rows[0]["Offer_amont"]) > 0)
                                {
                                    whereclause = whereclause + " step7 ";
                                    MySqlConnector.MySqlCommand cmd6 = new MySqlConnector.MySqlCommand("getwalletRefforbasecurrency");
                                    cmd6.CommandType = CommandType.StoredProcedure;
                                    string basecurrency = string.Empty;
                                    whereclause = whereclause + " step8 ";
                                    if (Convert.ToInt32(dtt.Rows[0]["Currency_type"]) == 1)
                                    {
                                        whereclause = whereclause + " step9";
                                        basecurrency = Convert.ToString(get_Details.Rows[0]["FromCurrency_Code"]);
                                        // Currency_id = Convert.ToInt32(dtt.Rows[0]["Currency_Id"]);
                                    }
                                    else if (Convert.ToInt32(dtt.Rows[0]["Currency_type"]) == 2)
                                    {
                                        whereclause = whereclause + " step10";

                                        basecurrency = Convert.ToString(get_Details.Rows[0]["Currency_Code"]);
                                        //Currency_id = Convert.ToInt32(dtt.Rows[0]["Currency_Id"]);
                                    }
                                    cmd6.Parameters.AddWithValue("_Client_ID", 1);
                                    cmd6.Parameters.AddWithValue("_WireTransfer_ReferanceNo", get_Details.Rows[0]["WireTransfer_ReferanceNo"]);
                                    cmd6.Parameters.AddWithValue("_BaseCurrency", basecurrency);

                                    cmd6.Parameters.AddWithValue("_AgentFlag", 1);//Get Customer Wallet
                                    DataTable dtwal = db_connection.ExecuteQueryDataTableProcedure(cmd6);
                                    whereclause = whereclause + " step10";

                                    cmd6 = new MySqlConnector.MySqlCommand("getbasecurrencyid");
                                    cmd6.CommandType = CommandType.StoredProcedure;
                                    cmd6.Parameters.AddWithValue("_Client_ID", Client_ID);
                                    cmd6.Parameters.AddWithValue("_BaseCurrency", basecurrency);
                                    Currency_id = Convert.ToInt32(db_connection.ExecuteScalarProcedure(cmd6));
                                    whereclause = whereclause + " step11";

                                    if (dtwal.Rows.Count == 0)
                                    {
                                        whereclause = whereclause + " step12";

                                        string wrefNo = GenerateWalletReference(Client_ID);
                                        whereclause = whereclause + " step13";

                                        //using (cmd6 = new MySqlCommand("insertinwallet_table", con))
                                        {
                                            whereclause = whereclause + " step14";

                                            cmd6 = new MySqlConnector.MySqlCommand("insertinwallet_table");
                                            cmd6.CommandType = CommandType.StoredProcedure;
                                            cmd6.Parameters.AddWithValue("_WireTransfer_ReferanceNo", get_Details.Rows[0]["WireTransfer_ReferanceNo"]);
                                            cmd6.Parameters.AddWithValue("_Currency_ID", Currency_id);
                                            //iw1.Client_ID = client_id;
                                            cmd6.Parameters.AddWithValue("_Delete_Status", 0);
                                            cmd6.Parameters.AddWithValue("_Record_Insert_DateTime", DateTime.Now);
                                            cmd6.Parameters.AddWithValue("_wallet_reference", wrefNo);
                                            cmd6.Parameters.AddWithValue("_Wallet_balance", 0);
                                            cmd6.Parameters.AddWithValue("_Client_ID", 1);
                                            cmd6.Parameters.AddWithValue("_Branch_ID", 1);
                                            cmd6.Parameters.AddWithValue("_Customer_ID", Convert.ToInt32(get_Details.Rows[0]["Customer_Id"]));
                                            cmd6.Parameters.AddWithValue("_AgentFlag", 1);//Get Customer Wallet
                                            db_connection.ExecuteNonQueryProcedure(cmd6);

                                            //int a = db_connection.ExecuteNonQueryProcedure(cmd6);
                                            //cmd6.Dispose();
                                        }
                                    }
                                    whereclause = whereclause + " step15";

                                    cmd6 = new MySqlConnector.MySqlCommand("getwalletid");
                                    cmd6.CommandType = CommandType.StoredProcedure;
                                    cmd6.Parameters.AddWithValue("_WireTransfer_ReferanceNo", get_Details.Rows[0]["WireTransfer_ReferanceNo"]);
                                    cmd6.Parameters.AddWithValue("_Currency_ID", Convert.ToInt32(Currency_id));
                                    cmd6.Parameters.AddWithValue("_Client_ID", 1);
                                    string walletid = Convert.ToString(db_connection.ExecuteScalarProcedure(cmd6));
                                    //cmd6.Dispose();
                                    whereclause = whereclause + " step16";

                                    cmd6 = new MySqlConnector.MySqlCommand("checkinwalletbalance");
                                    cmd6.CommandType = CommandType.StoredProcedure;
                                    cmd6.Parameters.AddWithValue("_wallet_id", Convert.ToInt32(walletid));

                                    DataTable dtwbal = db_connection.ExecuteQueryDataTableProcedure(cmd6);
                                    //cmd6.Dispose();
                                    whereclause = whereclause + " step17";

                                    double oldbal = 0, newbal = 0;
                                    if (dtwbal.Rows.Count > 0)
                                    {
                                        oldbal = Convert.ToDouble(dtwbal.Rows[0]["newBalance"]);


                                    }
                                    whereclause = whereclause + " step18";

                                    newbal = oldbal + Convert.ToDouble(dtt.Rows[0]["Offer_amont"]);
                                    int paytype = 10, userid = 1;
                                    double exchangerate = 1, trfee = 0;
                                    cmd6 = new MySqlConnector.MySqlCommand("getPType_ID");
                                    cmd6.CommandType = CommandType.StoredProcedure;

                                    paytype = Convert.ToInt32(db_connection.ExecuteScalarProcedure(cmd6));
                                    whereclause = whereclause + " step19";

                                    //cmd6.Dispose();
                                    //using (cmd6 = new MySqlCommand("insert_wallet_transaction", con))
                                    activity = "Offer Wallet amount added to customer is " + basecurrency + Convert.ToDouble(dtt.Rows[0]["Offer_amont"]) + " for transaction reference " + txn_ref;
                                    {
                                        whereclause = whereclause + " step20";

                                        cmd6 = new MySqlConnector.MySqlCommand("insert_wallet_transaction");
                                        cmd6.CommandType = CommandType.StoredProcedure;

                                        cmd6.Parameters.AddWithValue("_wallet_id", Convert.ToInt32(walletid));
                                        cmd6.Parameters.AddWithValue("_transfer_type", 1);
                                        cmd6.Parameters.AddWithValue("_Currency_ID", Convert.ToInt32(Currency_id));
                                        cmd6.Parameters.AddWithValue("_transfer_amount", Convert.ToDouble(dtt.Rows[0]["Offer_amont"]));//referee_value;
                                        cmd6.Parameters.AddWithValue("_Wallet_Description", activity);
                                        cmd6.Parameters.AddWithValue("_oldwalletbalance", oldbal);
                                        cmd6.Parameters.AddWithValue("_newwalletbalance", newbal);
                                        cmd6.Parameters.AddWithValue("_Record_Insert_DateTime", DateTime.Now);
                                        cmd6.Parameters.AddWithValue("_Delete_Status", 0);
                                        cmd6.Parameters.AddWithValue("_Client_ID", 1);
                                        cmd6.Parameters.AddWithValue("_Branch_ID", 1);
                                        cmd6.Parameters.AddWithValue("_paytype", paytype);
                                        cmd6.Parameters.AddWithValue("_exchangerate", exchangerate);
                                        cmd6.Parameters.AddWithValue("_Transaction_ID", 0);
                                        cmd6.Parameters.AddWithValue("_fee", trfee);
                                        cmd6.Parameters.AddWithValue("_User_ID", userid);
                                        cmd6.Parameters.AddWithValue("_AgentFlag", 1);
                                        cmd6.Parameters.AddWithValue("_referee_id", 0);
                                        cmd6.Parameters.AddWithValue("_Referral_Flag", 1);
                                        db_connection.ExecuteNonQueryProcedure(cmd6);
                                        //int a = db_connection.ExecuteNonQueryProcedure(cmd6);
                                        //cmd6.Dispose();
                                        whereclause = whereclause + " step21";

                                    }
                                    int a_wallet = 0;
                                    {
                                        whereclause = whereclause + " step22";
                                        cmd6 = new MySqlConnector.MySqlCommand("updateinwallettable");
                                        cmd6.CommandType = CommandType.StoredProcedure;

                                        cmd6.Parameters.AddWithValue("_transfer_amount", newbal);
                                        cmd6.Parameters.AddWithValue("_wallet_id", Convert.ToInt32(walletid));
                                        cmd6.Parameters.AddWithValue("_Client_ID", Client_ID);
                                        cmd6.Parameters.AddWithValue("_Branch_ID", 1);
                                        // a_wallet = cmd6.ExecuteNonQuery();
                                        int a = db_connection.ExecuteNonQueryProcedure(cmd6);
                                        if (a > 0)
                                        {
                                            offer_val = 1;
                                        }
                                        whereclause = whereclause + "offer_val" + offer_val + " step23";
                                        try
                                        {
                                            whereclause = whereclause + " step24";
                                            CompanyInfo.InsertActivityLogDetails(activity, 1, Convert.ToInt32(Transaction_ID), 1, Convert.ToInt32(Customer_ID), "offer_wallet_Reward", 1, 1, "Offer", context);

                                            DataTable dt_notif = CompanyInfo.set_notification_data(170);

                                            if (dt_notif.Rows.Count > 0)
                                            {

                                                int SMS = Convert.ToInt32(dt_notif.Rows[0]["SMS"]);
                                                int Email = Convert.ToInt32(dt_notif.Rows[0]["Email"]);
                                                int Notif_status = Convert.ToInt32(dt_notif.Rows[0]["Notification"]);
                                                string notification_msg = Convert.ToString(dt_notif.Rows[0]["notification_msg"]);
                                                if (notification_msg.Contains("[Txn_Ref] ") == true)
                                                {
                                                    notification_msg = notification_msg.Replace("[Txn_Ref] ", Convert.ToString(txn_ref));
                                                }

                                                if (notification_msg.Contains("[Wallet_Amount] ") == true)
                                                {
                                                    string wallet_amt = Convert.ToString(Convert.ToDouble(dtt.Rows[0]["Offer_amont"])) + basecurrency;
                                                    notification_msg = notification_msg.Replace("[Wallet_Amount] ", wallet_amt);
                                                }

                                                int i1 = CompanyInfo.check_notification_perm(Convert.ToString(Customer_ID), 1, 1, 170, 9, Convert.ToDateTime(DateTime.Now), 1, SMS, Email, Notif_status, "Backoffice - Spin the wheel offer notification - 170", notification_msg, Convert.ToInt32(Transaction_ID), context);

                                            }

                                        }
                                        catch (Exception ex)
                                        {
                                            CompanyInfo.InsertErrorLogDetails(ex.ToString(), 1, "Spin the offer on deposit", 1, 1);

                                        }
                                        try
                                        {
                                            whereclause = whereclause + " step25";
                                            string EmailID = "";
                                           
                                            string email = Convert.ToString(get_Details.Rows[0]["Email_ID"]);
                                            string subject = string.Empty;
                                            string body = string.Empty;

                                            DataTable ds1 = CompanyInfo.get(obj.Client_ID,context);
                                            string Company_URL_Admin = "", Company_Name = "";
                                            if (ds1.Rows.Count > 0)
                                            {
                                                Company_URL_Admin = Convert.ToString(ds1.Rows[0]["Company_URL_Admin"]);
                                                Company_Name = Convert.ToString(ds1.Rows[0]["Company_Name"]);
                                                /*HttpContext.Current.Session["BaseCurrency_Code"] = Convert.ToString(ds1.Rows[0]["BaseCurrency_Code"]);
                                                HttpContext.Current.Session["BaseCurrency_TimeZone"] = Convert.ToString(ds1.Rows[0]["BaseCurrency_TimeZone"]);
                                                HttpContext.Current.Session["BaseCurrency_Sign"] = Convert.ToString(ds1.Rows[0]["BaseCurrency_Sign"]);
                                                HttpContext.Current.Session["BaseCurrency_Country"] = Convert.ToString(ds1.Rows[0]["BaseCurrency_Country"]);
                                                HttpContext.Current.Session["BaseCountry_ID"] = Convert.ToString(ds1.Rows[0]["BaseCountry_ID"]);
                                                HttpContext.Current.Session["EmailUrl"] = Convert.ToString(ds1.Rows[0]["Company_URL_Admin"]);
                                                HttpContext.Current.Session["CustomerURL"] = Convert.ToString(ds1.Rows[0]["Company_URL_Customer"]);
                                                HttpContext.Current.Session["Company_Name"] = Convert.ToString(ds1.Rows[0]["Company_Name"]);
                                                HttpContext.Current.Session["Cancel_Transaction_Hours"] = Convert.ToString(ds1.Rows[0]["Cancel_Transaction_Hours"]);*/
                                            }
                                            HttpWebRequest httpRequest;

                                            httpRequest = (HttpWebRequest)WebRequest.Create("" + Company_URL_Admin + "Email/offeremail.html");

                                            httpRequest.UserAgent = "Code Sample Web Client";
                                            HttpWebResponse webResponse = (HttpWebResponse)httpRequest.GetResponse();
                                            using (StreamReader reader = new StreamReader(webResponse.GetResponseStream()))
                                            {
                                                body = reader.ReadToEnd();
                                            }
                                            body = body.Replace("[name]", First_Name);
                                            string message = "You won offer amount " + Convert.ToDouble(dtt.Rows[0]["Offer_amont"]) + Convert.ToString(basecurrency) + " to your wallet for spin the wheel for Transaction reference " + txn_ref + ".";
                                            body = body.Replace("[msg]", message);
                                            body = body.Replace("[wallet_amt]", Convert.ToDouble(dtt.Rows[0]["Offer_amont"]) + Convert.ToString(basecurrency));
                                            body = body.Replace("[txn_ref]", txn_ref);
                                            subject = "" + Company_Name + " - Congratulations! You received a reward." + get_Details.Rows[0]["WireTransfer_ReferanceNo"];

                                            string send_mail = (string)CompanyInfo.Send_Mail(ds1, email, body, subject, obj.Client_ID, obj.Branch_ID, "", "", "", context);
                                           
                                            whereclause = whereclause + " step26";

                                        }
                                        catch (Exception ex)
                                        {
                                            CompanyInfo.InsertErrorLogDetails(ex.ToString().Replace("\'", "\\'"), 0, "App-Offer_wallet_Reward", 0, 0);
                                        }
                                        //cmd6.Dispose();
                                    }
                                }

                            }
                        }
                        #endregion adding cashback to wallet ends
                    }
                }
            }
            catch (Exception ex11)
            {
                CompanyInfo.InsertErrorLogDetails(ex11.ToString().Replace("\'", "\\'"), 0, "App-Offer_wallet_Reward", 0, 0);

            }
            finally
            {
                CompanyInfo.InsertActivityLogDetails(whereclause, 1, Convert.ToInt32(transactionID), 1, Convert.ToInt32(customerID), "App-Offer_wallet_Reward", 1, 1, "Offer", context);

            }
            return offer_val;
        }

        public static string GenerateWalletReference(int client)
        {
            string refNo1 = "";
            try
            {

                int size = 8;
                var rng = new Random(Environment.TickCount);
                MySqlConnector.MySqlCommand cmd = new MySqlConnector.MySqlCommand("getWalRef_InitialChar");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("_Client_ID", client);
                string initialchars = Convert.ToString(db_connection.ExecuteScalarProcedure(cmd));
                cmd.Dispose();

                var refNo = initialchars + string.Concat(Enumerable.Range(0, size).Select((index) => rng.Next(10).ToString()));
                refNo1 = Convert.ToString(refNo);
                cmd = new MySqlConnector.MySqlCommand("CheckDuplicateWalletReference");
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
    }
}

