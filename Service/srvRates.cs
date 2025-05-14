using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Net;
using RestSharp;
using Newtonsoft.Json;
using ListToDataTable;
using System.Xml;


namespace Calyx_Solutions.Service
{
    public class srvRates
    {

        private readonly HttpContext _srvRatesHttpContext;
        public srvRates(HttpContext context)
        {
            this._srvRatesHttpContext = context;
        }
        public DataTable GetRates(Model.Transaction obj)
        {
            List<Model.Currency> _lst = new List<Model.Currency>();
            string Country_ID_regex = validation.validate(Convert.ToString(obj.Client_ID), 0, 1, 1, 1, 1, 1, 1, 1, 1);
            string Currency_ID_regex = validation.validate(Convert.ToString(obj.Currency_ID), 0, 1, 1, 1, 1, 1, 1, 1, 1);
            string CB_ID_regex = validation.validate(Convert.ToString(obj.Currency_ID), 0, 1, 1, 1, 1, 1, 1, 1, 1);
            string PType_ID_regex = validation.validate(Convert.ToString(obj.PType_ID), 0, 1, 1, 1, 1, 1, 1, 1, 1);
            string PaymentDepositType_ID_regex = validation.validate(Convert.ToString(obj.PaymentDepositType_ID), 0, 1, 1, 1, 1, 1, 1, 1, 1);
            string DeliveryType_Id_regex = validation.validate(Convert.ToString(obj.DeliveryType_Id), 0, 1, 1, 1, 1, 1, 1, 1, 1);
            string TransferAmount_regex = validation.validate(Convert.ToString(obj.TransferAmount), 1, 1, 1, 1, 1, 1, 1, 1, 1);
            string TransferForeignAmount_regex = validation.validate(Convert.ToString(obj.TransferAmount), 1, 1, 1, 1, 1, 1, 1, 1, 1);
            DataTable dt = new DataTable();
            if (Country_ID_regex != "false" && Currency_ID_regex != "false" && CB_ID_regex != "false" && PType_ID_regex != "false" && PaymentDepositType_ID_regex != "false" &&
                DeliveryType_Id_regex != "false" && TransferAmount_regex != "false" && TransferForeignAmount_regex != "false")
            {
                int Assigned_Branch_ID = 0;
                int ratePerm = -1;
                try
                {
                    //apply assigned branch rates permission
                    MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("GetPermissions");//sanket 310125
                    _cmd.CommandType = CommandType.StoredProcedure;
                    _cmd.Parameters.AddWithValue("_whereclause", " and PID = 251");
                    _cmd.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                    DataTable dtperm = db_connection.ExecuteQueryDataTableProcedure(_cmd);
                    if (dtperm.Rows.Count > 0)
                    {
                        ratePerm = Convert.ToInt32(dtperm.Rows[0]["Status_ForCustomer"]);
                    }
                    if (ratePerm == 0)
                    {
                        int Customer_ID = Convert.ToInt32(CompanyInfo.Decrypt(obj.Customer_ID, true));
                        _cmd = new MySqlConnector.MySqlCommand("GetCustDetailsByID");
                        _cmd.CommandType = CommandType.StoredProcedure;
                        _cmd.Parameters.AddWithValue("cust_ID", Customer_ID);
                        DataTable dt1 = db_connection.ExecuteQueryDataTableProcedure(_cmd);
                        if (dt1.Rows.Count > 0)
                        {
                            Assigned_Branch_ID = Convert.ToInt32(dt1.Rows[0]["Assigned_Branch_ID"]);
                        }
                    }
                }
                catch(Exception egc) {
                  int h=  (int)CompanyInfo.InsertActivityLogDetailsSecurity("Errror GetRates:" + egc.ToString(), Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), 1, "srvRates", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "GetRates", 0, _srvRatesHttpContext);
                }

                CompanyInfo.InsertrequestLogTracker("GetRates Assigned_Branch_ID: " + Assigned_Branch_ID, 0, 0, 0, 0, "GetData", Convert.ToInt32(0), Convert.ToInt32(0), "", _srvRatesHttpContext);

                MySqlConnector.MySqlCommand cmd = new MySqlConnector.MySqlCommand("Rates_Search");
                cmd.CommandType = CommandType.StoredProcedure;
                string whereclause = " ";
                if (obj.Country_ID > 0)
                {
                    whereclause = whereclause + " and t.Country_ID = " + obj.Country_ID + "";
                }
                if (obj.Currency_ID > 0)
                {
                    whereclause = whereclause + " and t.Currency_ID = " + obj.Currency_ID + "";
                }
                if (ratePerm == 0 && Assigned_Branch_ID > 0)//sanket 310125
                {
                    whereclause = whereclause + " and t.CB_ID = " + Assigned_Branch_ID + "";
                }
                //if (obj.new_agent_branch != "null" && obj.new_agent_branch != "0" && obj.new_agent_branch != null)

                else if (obj.new_agent_id != "1" && obj.new_agent_id != "" && obj.new_agent_id != "null" && obj.new_agent_id != null)
                {
                    whereclause = whereclause + " and t.CB_ID = " + obj.new_agent_branch + "";
                    
                }
                else
                {
                    whereclause = whereclause + " and t.CB_ID = " + obj.CB_ID + "";

                }
                if (obj.PType_ID > 0)
                {
                    whereclause = whereclause + " and t.PType_ID = " + obj.PType_ID + "";
                }
                if (obj.PaymentDepositType_ID > 0)
                {
                    whereclause = whereclause + " and t.PayDepositType_ID = " + obj.PaymentDepositType_ID + "";
                }
                if (obj.DeliveryType_Id > 0)
                {
                    whereclause = whereclause + " and t.DeliveryType_ID = " + obj.DeliveryType_Id + "";
                }
                obj.AmountInGBP = Convert.ToDouble(obj.TransferAmount);
                if (obj.AmountInGBP != null && obj.AmountInGBP != 0)
                {
                    whereclause = whereclause + " and t.Min_Amount <= " + obj.AmountInGBP + " and t.Max_Amount >= " + obj.AmountInGBP + "";
                }
                obj.AmountInPKR = Convert.ToDouble(obj.TransferForeignAmount);
                if (obj.AmountInPKR != null && obj.AmountInPKR != 0)
                {
                    whereclause = whereclause + " and t.Foreign_Currency_Min_Amount <= " + obj.AmountInPKR + " and t.Foreign_Currency_Max_Amount >= " + obj.AmountInPKR + "";
                }

                if(obj.FromCurrency_CodeId == 0)
                whereclause = whereclause + " and t.BaseCurrency_ID = " + obj.FromCurrency_Code + "";//mandatory
                else
                whereclause = whereclause + " and t.BaseCurrency_ID = " + obj.FromCurrency_CodeId + "";//mandatory

                try
                {
                    if (obj.offer_rate_flag != -1 || obj.offer_rate_flag == 0 || obj.offer_rate_flag == 1)
                    { //vyankatesh

                        whereclause = whereclause + " and t.offer_rate_flag = " + obj.offer_rate_flag;
                    }
                    else
                    {
                        whereclause = whereclause + " and t.offer_rate_flag = 1";
                    }
                }
                catch(Exception ex) { }

                cmd.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                cmd.Parameters.AddWithValue("_whereclause", whereclause);
                dt = db_connection.ExecuteQueryDataTableProcedure(cmd);

                int stattus = (int)CompanyInfo.InsertActivityLogDetailsSecurity(whereclause, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), 1, "srvRates", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "GetRates", 0, _srvRatesHttpContext);
                try
                {
                    CompanyInfo.InsertrequestLogTracker("GetRates whereclause : " + whereclause + " and improved_rate_flag value :"+ obj.improved_rate_flag, 0, 0, 0, 0, "GetData", Convert.ToInt32(0), Convert.ToInt32(0), "", _srvRatesHttpContext);
                }
                catch (Exception egx) {
                    CompanyInfo.InsertrequestLogTracker("GetRates whereclause : " + whereclause, 0, 0, 0, 0, "GetData", Convert.ToInt32(0), Convert.ToInt32(0), "", _srvRatesHttpContext);
                }


                try
                {
                    //code to verify improved rates
                    if (obj.improved_rate_flag == 0)
                    {

                        MySqlConnector.MySqlCommand cmd1 = new MySqlConnector.MySqlCommand("verify_improved_rate");
                        cmd1.CommandType = CommandType.StoredProcedure;
                        cmd1.Parameters.AddWithValue("_cust_id", Convert.ToInt32(CompanyInfo.Decrypt(obj.Customer_ID, true)));
                        cmd1.Parameters.Add("_is_valid", MySqlConnector.MySqlDbType.Bit).Direction = ParameterDirection.Output;
                        db_connection.ExecuteNonQueryProcedure(cmd1); cmd1.Dispose();


                        bool status1 = Convert.ToBoolean(cmd1.Parameters["_is_valid"].Value);
                        string activity = "<b>" + obj.UserName + "</b> is on verify Improved Rate.<br/>";
                        stattus = (int)CompanyInfo.InsertActivityLogDetailsSecurity(activity, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), 1, "srvRates", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "GetRates", 0, _srvRatesHttpContext);

                        dt.Columns.Add("status", typeof(string));
                        foreach (DataRow row in dt.Rows)
                        {
                            row["status"] = status1;
                        }
                    }
                }
                catch (Exception egx) { }
            }
            else
            {
                string msg = "Validation Error Country_ID_regex- +" + Country_ID_regex + "Currency_ID_regex- " + Currency_ID_regex + " CB_ID_regex- +" + CB_ID_regex + " +PType_ID_regex- " + PType_ID_regex + "PaymentDepositType_ID_regex- " + PaymentDepositType_ID_regex + "DeliveryType_Id_regex-" + DeliveryType_Id_regex + "TransferAmount_regex- +" + TransferAmount_regex + " + TransferForeignAmount_regex-" + TransferForeignAmount_regex;
                int stattus = (int)CompanyInfo.InsertActivityLogDetailsSecurity(msg, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), 1, "srvRates", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "GetRates", 0, _srvRatesHttpContext);
            }
            return dt;
        }
        public DataTable Dashboard_CountryWiseRates(Model.Transaction obj)
        {
            List<Model.Currency> _lst = new List<Model.Currency>();
            string Client_ID_regex = validation.validate(Convert.ToString(obj.Client_ID), 0, 1, 1, 1, 1, 1, 1, 1, 1);
            string Country_ID_regex = validation.validate(Convert.ToString(obj.Country_ID), 0, 1, 1, 1, 1, 1, 1, 1, 1);
            DataTable dt = new DataTable();
            if (Client_ID_regex != "false" && Country_ID_regex != "false")
            {
               MySqlConnector.MySqlCommand cmd = new MySqlConnector.MySqlCommand("Dashboard_CountrywiseRates");
                cmd.CommandType = CommandType.StoredProcedure;
                int SourceID = 1;
                if (obj.Is_App == 0)
                { SourceID = 3; }
                else if (obj.Branch_ID == 2)
                { SourceID = 2; }
                cmd.Parameters.AddWithValue("_Source_ID", SourceID);
                cmd.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                cmd.Parameters.AddWithValue("_Country_ID", obj.Country_ID);
                dt = db_connection.ExecuteQueryDataTableProcedure(cmd);
            }
            else
            {
                string msg = "Validation Error Client_ID_regex- " + Client_ID_regex + "Country_ID_regex- " + Country_ID_regex;
                int stattus = (int)CompanyInfo.InsertActivityLogDetailsSecurity(msg, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), 1, "srvRates", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "Dashboard_CountryWiseRates", 0, _srvRatesHttpContext);
            }
            return dt;
        }
        //and t.country_ID=45 and t.CB_ID=2
        public DataTable Dashboard_Rates(Model.Transaction obj)
        {
            List<Model.Currency> _lst = new List<Model.Currency>();
            string Country_ID_regex = validation.validate(Convert.ToString(obj.Country_ID), 0, 1, 1, 1, 1, 1, 1, 1, 1);
            string Currency_ID_regex = validation.validate(Convert.ToString(obj.Currency_ID), 0, 1, 1, 1, 1, 1, 1, 1, 1);
            string CB_ID_regex = validation.validate(Convert.ToString(obj.CB_ID), 0, 1, 1, 1, 1, 1, 1, 1, 1);
            string PType_ID = validation.validate(Convert.ToString(obj.PType_ID), 0, 1, 1, 1, 1, 1, 1, 1, 1);
            string PaymentDepositType_ID = validation.validate(Convert.ToString(obj.PaymentDepositType_ID), 0, 1, 1, 1, 1, 1, 1, 1, 1);
            string DeliveryType_Id = validation.validate(Convert.ToString(obj.PaymentDepositType_ID), 0, 1, 1, 1, 1, 1, 1, 1, 1);
            DataTable dt = new DataTable();
            if (Country_ID_regex != "false" && Currency_ID_regex != "false" && CB_ID_regex != "false" && PType_ID != "false" && PaymentDepositType_ID != "false" && DeliveryType_Id != "false")
            {
                int Customer_IDNew = 0;
                int Assigned_Branch_ID = 0;
                int ratePerm = -1;//apply assigned branch rates permission
                try { 
                MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("GetPermissions");//sanket 310125
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.Parameters.AddWithValue("_whereclause", " and PID = 251");
                _cmd.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                DataTable dtperm = db_connection.ExecuteQueryDataTableProcedure(_cmd);
                if (dtperm.Rows.Count > 0)
                {
                    ratePerm = Convert.ToInt32(dtperm.Rows[0]["Status_ForCustomer"]);
                }
                if (ratePerm == 0)
                {
                    int Customer_ID = Convert.ToInt32(CompanyInfo.Decrypt(obj.Customer_ID, true));
                        Customer_IDNew = Customer_ID;
                    _cmd = new MySqlConnector.MySqlCommand("GetCustDetailsByID");
                    _cmd.CommandType = CommandType.StoredProcedure;
                    _cmd.Parameters.AddWithValue("cust_ID", Customer_ID);
                    DataTable dt1 = db_connection.ExecuteQueryDataTableProcedure(_cmd);
                    if (dt1.Rows.Count > 0)
                    {
                        Assigned_Branch_ID = Convert.ToInt32(dt1.Rows[0]["Assigned_Branch_ID"]);
                    }
                }
                }
                catch (Exception egc)
                {
                    int h = (int)CompanyInfo.InsertActivityLogDetailsSecurity("Errror Dashboard_Rates:" + egc.ToString(), Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), 1, "Dashboard_Rates", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "GetRates", 0, _srvRatesHttpContext);
                }

                string GetFlag = obj.Transaction_From_Flag; string NewGetF = "";

                if (GetFlag == "sendmoneyflag")
                {
                    NewGetF = "Dashboard_Rates_SendMoney";
                }
                else
                {
                    NewGetF = "Dashboard_Rates";
                }

                MySqlConnector.MySqlCommand cmd = new MySqlConnector.MySqlCommand(NewGetF);
                cmd.CommandType = CommandType.StoredProcedure;
                string whereclause = " ";
                if (obj.Country_ID > 0)
                {
                    whereclause = whereclause + " and t.Country_ID = " + obj.Country_ID + "";
                }
                if (obj.Currency_ID > 0)
                {
                    whereclause = whereclause + " and t.Currency_ID = " + obj.Currency_ID + "";
                }
                if (ratePerm == 0 && Assigned_Branch_ID > 0)//sanket 310125
                {
                    whereclause = whereclause + " and t.CB_ID = " + Assigned_Branch_ID + "";
                }
                //if (obj.new_agent_branch != "null" && obj.new_agent_branch!="0" &&  obj.new_agent_branch != null)
                else if (obj.new_agent_id != "1" && obj.new_agent_id != "null" && obj.new_agent_id != null)
                {
                    whereclause = whereclause + " and t.CB_ID = " + obj.new_agent_branch + "";

                }
                else
                {
                    whereclause = whereclause + " and t.CB_ID = " + obj.CB_ID + "";

                }
                if (obj.PType_ID > 0)
                {
                    whereclause = whereclause + " and t.PType_ID = " + obj.PType_ID + "";
                }
                if (obj.PaymentDepositType_ID > 0)
                {
                    whereclause = whereclause + " and t.PayDepositType_ID = " + obj.PaymentDepositType_ID + "";
                }
                if (obj.DeliveryType_Id > 0)
                {
                    whereclause = whereclause + " and t.DeliveryType_ID = " + obj.DeliveryType_Id + "";
                }
                //obj.AmountInGBP = Convert.ToDouble(obj.TransferAmount);
                //if (obj.AmountInGBP != null && obj.AmountInGBP != 0)
                //{
                //    whereclause = whereclause + " and t.Min_Amount <= " + obj.AmountInGBP + " and t.Max_Amount >= " + obj.AmountInGBP + "";
                //}
                //obj.AmountInPKR = Convert.ToDouble(obj.TransferForeignAmount);
                //if (obj.AmountInPKR != null && obj.AmountInPKR != 0)
                //{
                //    whereclause = whereclause + " and t.Foreign_Currency_Min_Amount <= " + obj.AmountInPKR + " and t.Foreign_Currency_Max_Amount >= " + obj.AmountInPKR + "";
                //}

                CompanyInfo.InsertrequestLogTracker("Dashboard_Rates Assigned_Branch_ID: " + Assigned_Branch_ID + " and ratePerm: "+ ratePerm+ " and Customer_ID: "+ Customer_IDNew, 0, 0, 0, 0, "Dashboard_Rates", Convert.ToInt32(0), Convert.ToInt32(0), "", _srvRatesHttpContext);


                whereclause = whereclause + " and t.BaseCurrency_ID = " + obj.FromCurrency_Code + "";//mandatory
                cmd.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                cmd.Parameters.AddWithValue("_whereclause", whereclause);
                dt = db_connection.ExecuteQueryDataTableProcedure(cmd);
            }
            else
            {

                string msg = "Validation Error Country_ID_regex-" + Country_ID_regex + "Currency_ID_regex- " + Currency_ID_regex + "CB_ID_regex- " + CB_ID_regex + "PType_ID-" + PType_ID + "PaymentDepositType_ID-" + PaymentDepositType_ID + "DeliveryType_Id " + DeliveryType_Id;
                int stattus = (int)CompanyInfo.InsertActivityLogDetailsSecurity(msg, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), 1, "srvRates", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "Dashboard_Rates", 0,_srvRatesHttpContext);

            }
            return dt;
        }
        public DataTable GetActiveCountries(Model.Transaction obj)
        {
            DataTable dt = new DataTable();
            MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("GetPermissions");
            _cmd.CommandType = CommandType.StoredProcedure;
            _cmd.Parameters.AddWithValue("_whereclause", " and PID=34");
            _cmd.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
            DataTable dt1 = db_connection.ExecuteQueryDataTableProcedure(_cmd);
            int perm = 1;
            if (dt1.Rows.Count > 0)
            {
                perm = Convert.ToInt32(dt1.Rows[0]["Status_ForCustomer"]);
            }
            if (perm != 0)
            {
                _cmd = new MySqlConnector.MySqlCommand("Rates_bindActiveCountry");
                _cmd.CommandType = CommandType.StoredProcedure;
                string whereclause = " and cm.Client_ID=" + obj.Client_ID;
                if (obj.Country_ID > 0)
                {
                    whereclause = whereclause + " and c.Country_ID=" + obj.Country_ID + "";
                }
                _cmd.Parameters.AddWithValue("_whereclause", whereclause);
                dt = db_connection.ExecuteQueryDataTableProcedure(_cmd);
            }
            else
            {
                string whereclause = "";
                if (obj.Country_ID > 0)
                {
                    whereclause = " and c.Country_ID=" + obj.Country_ID + "";
                }
                _cmd = new MySqlConnector.MySqlCommand("getMultiCurrency");
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.Parameters.AddWithValue("_whereclause", whereclause);
                _cmd.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                dt = db_connection.ExecuteQueryDataTableProcedure(_cmd);
            }
            return dt;
        }
        public DataTable CollectionPoints(Model.Transaction obj)
        {
            List<Model.Currency> _lst = new List<Model.Currency>();
            MySqlConnector.MySqlCommand cmd = new MySqlConnector.MySqlCommand("CollectionPoint_Search");
            cmd.CommandType = CommandType.StoredProcedure;
            string whereclause = " ";
            DataTable dtperm = (DataTable)CompanyInfo.getEmailPermission(obj.Client_ID, 50);
            int cityperm = 1;
            if (dtperm.Rows.Count > 0)
            {
                cityperm = Convert.ToInt32(dtperm.Rows[0]["Status_ForCustomer"]);
            }
            if (obj.Country_ID > 0)
            {
                whereclause = whereclause + " and collection_point_table.Country_ID = " + obj.Country_ID + "";
            }
            if (obj.City_ID > 0 && cityperm == 0)
            {
                whereclause = whereclause + " and collection_point_table.City_ID = " + obj.City_ID + "";
            }
            if (obj.Client_ID > 0)
            {
                whereclause = whereclause + " and collection_point_table.Client_ID = " + obj.Client_ID + "";
            }
            if (obj.CollectionPoint != "" && obj.CollectionPoint != null)
            {
                whereclause = whereclause + " and concat(collection_point_table.Location_Name,' ',collection_point_table.Address) like '%" + obj.CollectionPoint + "%'";
            }
            if (obj.CollectionPoint_ID > 0)
            {
                whereclause = whereclause + " and ID = " + obj.CollectionPoint_ID + "";
            }
            cmd.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
            cmd.Parameters.AddWithValue("_whereclause", whereclause);
            DataTable dt = db_connection.ExecuteQueryDataTableProcedure(cmd);

            if (dt.Rows.Count == 0 && obj.City_ID > 0 && cityperm == 0)
            {
                //without city
                cmd = new MySqlConnector.MySqlCommand("CollectionPoint_Search");
                cmd.CommandType = CommandType.StoredProcedure;
                whereclause = " ";
                if (obj.Country_ID > 0)
                {
                    whereclause = whereclause + " and collection_point_table.Country_ID = " + obj.Country_ID + "";
                }
                if (obj.Client_ID > 0)
                {
                    whereclause = whereclause + " and collection_point_table.Client_ID = " + obj.Client_ID + "";
                }
                if (obj.CollectionPoint != "" && obj.CollectionPoint != null)
                {
                    whereclause = whereclause + " and concat(collection_point_table.Location_Name,' ',collection_point_table.Address) like '%" + obj.CollectionPoint + "%'";
                }
                if (obj.CollectionPoint_ID > 0)
                {
                    whereclause = whereclause + " and ID = " + obj.CollectionPoint_ID + "";
                }
                cmd.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                cmd.Parameters.AddWithValue("_whereclause", whereclause);
                dt = db_connection.ExecuteQueryDataTableProcedure(cmd);
            }
            return dt;
        }
        public DataTable Get_BKLBranchList(Model.Transaction c)
        {
            DataTable ds = new DataTable();
            try
            {
                MySqlConnector.MySqlCommand cmd = new MySqlConnector.MySqlCommand("Get_APIDetails");
                cmd.CommandType = CommandType.StoredProcedure;
                string whereclause = " and a.Client_ID=" + c.Client_ID + " and ac.Country_ID=" + c.Country_ID + "";
                cmd.Parameters.AddWithValue("_whereclause", whereclause);
                cmd.Parameters.AddWithValue("_security_key", CompanyInfo.SecurityKey());
                DataTable dt = db_connection.ExecuteQueryDataTableProcedure(cmd);
                if (dt.Rows.Count > 0)
                {
                    string countrycode = "", country = "";
                    if (c.Country_ID > 0)
                    {
                        cmd.Dispose();
                        cmd = new MySqlConnector.MySqlCommand("Country_Search");
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("_Client_ID", c.Client_ID);
                        cmd.Parameters.AddWithValue("_whereclause", " and Country_ID=" + c.Country_ID + ""); // Get ISO Aplha 2 letter code
                        DataTable dtc = db_connection.ExecuteQueryDataTableProcedure(cmd);
                        if (dt.Rows.Count > 0)
                        {
                            countrycode = Convert.ToString(dtc.Rows[0]["ISO_Code"]); country = Convert.ToString(dtc.Rows[0]["Country_Name"]);
                        }
                    }
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        int api_id = Convert.ToInt32(dt.Rows[i]["ID"]);
                        string apiurl = Convert.ToString(dt.Rows[i]["API_URL"]);
                        string apiuser = Convert.ToString(dt.Rows[i]["APIUser_ID"]);
                        string apipass = Convert.ToString(dt.Rows[i]["Password"]);
                        string accesscode = Convert.ToString(dt.Rows[i]["APIAccess_Code"]);//unique code
                        string apicompany_id = Convert.ToString(dt.Rows[i]["APICompany_ID"]);
                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                        if (api_id == 2)
                        {   // Get collection Points by GCC
                            DataColumnCollection columns = ds.Columns;
                            if (!columns.Contains("Branchcode"))
                            {
                                ds.Columns.Add("Branchcode", typeof(string)); ds.Columns.Add("City", typeof(string));
                                ds.Columns.Add("Country", typeof(string)); ds.Columns.Add("Address", typeof(string)); ds.Columns.Add("API_ID", typeof(string));
                            }
                            try
                            {
                                string paymentmode = "";
                                if (c.PaymentDepositType_ID == 1) // BANK
                                {
                                    paymentmode = "BANK";
                                }
                                else if (c.PaymentDepositType_ID == 2) // CASH
                                {
                                    paymentmode = "CASH";
                                }
                                else if (c.PaymentDepositType_ID == 3) // Mobile wallet
                                {
                                    paymentmode = "BANK";
                                }

                                // Country Code Check Here
                                if (countrycode == "FR" || countrycode == "DE" || countrycode == "BE" || countrycode == "BG" || countrycode == "DK" || countrycode == "CH"
                          || countrycode == "SE" || countrycode == "ES" || countrycode == "ES" || countrycode == "SI" || countrycode == "NL" || countrycode == "IT"
                          || countrycode == "LV" || countrycode == "LI" || countrycode == "LT" || countrycode == "LU" || countrycode == "MT" || countrycode == "MC" || countrycode == "NL" || countrycode == "NO"
                          || countrycode == "PL" || countrycode == "PT" || countrycode == "RO" || countrycode == "SM" || countrycode == "SK" || countrycode == "SI" || countrycode == "ES" || countrycode == "SE"
                          || countrycode == "CZ" || countrycode == "FR" || countrycode == "AT" || countrycode == "HR" || countrycode == "CY" || countrycode == "EE" || countrycode == "FI" || countrycode == "GR"
                          || countrycode == "HU" || countrycode == "IS" || countrycode == "IE")
                                {
                                    countrycode = "EU";
                                }

                                // Gat PaymentMode Here New Update from GCC team
                                string countryCurrency = "";
                                if ("EU" == countrycode) { countryCurrency = "EUR"; }

                                if ("EU" != countrycode)
                                {
                                    try { countryCurrency = Convert.ToString(c.Currency_Code).Trim(); } catch (Exception egx) { }
                                }


                                var clientPaymentMode = new RestClient(apiurl);
                                clientPaymentMode.Timeout = -1;
                                var requestPaymentMode = new RestRequest(Method.POST);
                                requestPaymentMode.AddHeader("Content-Type", "text/xml; charset=utf-8");
                                requestPaymentMode.AddHeader("SOAPAction", "http://tempuri.org/ISendAPI/GetPaymentModeList");
                                var bodyPaymentMode = @"<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:tem=""http://tempuri.org/"" xmlns:grem=""http://schemas.datacontract.org/2004/07/GRemitWCFService.Send"">
                                    " + "\n" +
                                            @"   <soapenv:Header/>
                                    " + "\n" +
                                            @"   <soapenv:Body>
                                    " + "\n" +
                                            @"      <tem:GetPaymentModeList>       
                                    " + "\n" +
                                            @"         <tem:req>      
                                    " + "\n" +
                                            @"            <grem:CountryCode>" + countrycode + "</grem:CountryCode>" + "\n" +
                                            @"            <grem:CurrencyCode>" + countryCurrency + "</grem:CurrencyCode>" + "\n" +
                                            @"            <grem:Password>" + apipass + "</grem:Password>" + "\n" +
                                            @"            <grem:SecurityKey>" + accesscode + "</grem:SecurityKey>" + "\n" +
                                            @"            <grem:UniqueID>" + apiuser + "</grem:UniqueID>" + "\n" +
                                            @"         </tem:req>
                                    " + "\n" +
                                            @"      </tem:GetPaymentModeList>
                                    " + "\n" +
                                            @"   </soapenv:Body>
                                    " + "\n" +
                                            @"</soapenv:Envelope>";
                                requestPaymentMode.AddParameter("text/xml; charset=utf-8", bodyPaymentMode, ParameterType.RequestBody);
                                IRestResponse responsePaymentMode = clientPaymentMode.Execute(requestPaymentMode);

                                XmlDocument xmlDocPaymentMode = new XmlDocument();

                                xmlDocPaymentMode.LoadXml(responsePaymentMode.Content);
                                XmlNodeList nodeListPaymentMode = xmlDocPaymentMode.GetElementsByTagName("Table");
                                string Country = "", paymentMode = "", branchCode = "", bankNM = "", branchAddress = "";
                                foreach (XmlNode node1 in nodeListPaymentMode)
                                {
                                    string json = Newtonsoft.Json.JsonConvert.SerializeXmlNode(node1);
                                    var obj1 = Newtonsoft.Json.Linq.JObject.Parse(json);
                                    string PaymentModeName = Convert.ToString(obj1["Table"]["PaymentModeName"]).ToUpper().Trim();
                                    string PaymentModeCode = Convert.ToString(obj1["Table"]["PaymentModeCode"]).Trim();
                                    string PaymentModePrefix = Convert.ToString(obj1["Table"]["PaymentModePrefix"]).Trim();
                                    string PaymentModeType = Convert.ToString(obj1["Table"]["PaymentModeType"]).Trim();
                                    // In Calyx system 1 for Bank, 2 for Cash , 3 for MW available
                                    if (c.PaymentDepositType_ID == 1) // BANK
                                    {
                                        if (PaymentModeName.Contains("MOBILE") || PaymentModeName.Contains("CASH"))
                                        {
                                            continue;
                                        }
                                    }
                                    else if (c.PaymentDepositType_ID == 2) // CASH
                                    {
                                        if (PaymentModeName.Contains("MOBILE") || PaymentModeName.Contains("CREDIT") || PaymentModeName.Contains("IMPS") || PaymentModeName.Contains("RTGS"))
                                        {
                                            continue;
                                        }
                                    }
                                    else if (c.PaymentDepositType_ID == 3) // Mobile wallet
                                    {
                                        if (PaymentModeName.Contains("Home") || PaymentModeName.Contains("Pickup") || PaymentModeName.Contains("CREDIT") || PaymentModeName.Contains("IMPS") || PaymentModeName.Contains("RTGS"))
                                        {
                                            continue;
                                        }
                                    }

                                    clientPaymentMode = new RestClient(apiurl);
                                    clientPaymentMode.Timeout = -1;
                                    requestPaymentMode = new RestRequest(Method.POST);
                                    requestPaymentMode.AddHeader("Content-Type", "text/xml; charset=utf-8");
                                    requestPaymentMode.AddHeader("SOAPAction", "http://tempuri.org/ISendAPI/GetBranchList");
                                    bodyPaymentMode = @"<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:tem=""http://tempuri.org/"" xmlns:grem=""http://schemas.datacontract.org/2004/07/GRemitWCFService.Send"">
                                        " + "\n" +
                                                @"   <soapenv:Header/>
                                        " + "\n" +
                                                @"   <soapenv:Body>
                                        " + "\n" +
                                                @"      <tem:GetBranchList>         
                                        " + "\n" +
                                                @"         <tem:req>            
                                        " + "\n" +
                                                @"            <grem:CityCode>0</grem:CityCode>            
                                        " + "\n" +
                                                @"            <grem:CountryCode>" + countrycode + "</grem:CountryCode>" + "\n" +
                                                @"            <grem:CurrencyCode>" + countryCurrency + "</grem:CurrencyCode>" + "\n" +
                                                @"            <grem:Password>" + apipass + "</grem:Password> " + "\n" +
                                                @"            <grem:PaymentModeCode>" + PaymentModeCode + "</grem:PaymentModeCode>" + "\n" +
                                                @"            <grem:Search></grem:Search>            
                                        " + "\n" +
                                                @"            <grem:SecurityKey>" + accesscode + "</grem:SecurityKey> " + "\n" +
                                                @"            <grem:UniqueID>" + apiuser + "</grem:UniqueID> " + "\n" +
                                                @"         </tem:req>
                                        " + "\n" +
                                                @"      </tem:GetBranchList>
                                        " + "\n" +
                                                @"   </soapenv:Body>
                                        " + "\n" +
                                                @"</soapenv:Envelope>";
                                    requestPaymentMode.AddParameter("text/xml; charset=utf-8", bodyPaymentMode, ParameterType.RequestBody);
                                    responsePaymentMode = clientPaymentMode.Execute(requestPaymentMode);

                                    xmlDocPaymentMode = new XmlDocument();

                                    xmlDocPaymentMode.LoadXml(responsePaymentMode.Content);
                                    XmlNodeList nodeList = xmlDocPaymentMode.GetElementsByTagName("Table");
                                    foreach (XmlNode node2 in nodeList)
                                    {
                                        string json2 = Newtonsoft.Json.JsonConvert.SerializeXmlNode(node2);
                                        var obj22 = Newtonsoft.Json.Linq.JObject.Parse(json2);
                                        Country = Convert.ToString(obj22["Table"]["CountryName"]).Trim();
                                        paymentMode = Convert.ToString(obj22["Table"]["PaymentModeName"]).Trim();
                                        branchCode = Convert.ToString(obj22["Table"]["BranchCode"]).Trim();
                                        bankNM = Convert.ToString(obj22["Table"]["BranchName"]).Trim();
                                        branchAddress = Convert.ToString(obj22["Table"]["BranchAddress"]).Trim().ToLower();

                                        //ds.Rows.Add(branchCode, "", custCountryName, "(" + bankNM + " : " + paymentMode + " : " + branchAddress + ")", api_id);
                                        ds.Rows.Add(branchCode, "", Country, "(" + bankNM + " : " + paymentMode + " : " + branchAddress + ")", api_id);
                                    }

                                }


                                /*
                            var client = new RestClient(apiurl);
                            client.Timeout = -1;
                            var request = new RestRequest(Method.POST);
                            request.AddHeader("Content-Type", "text/xml; charset=utf-8");
                            request.AddHeader("SOAPAction", "http://tempuri.org/ISendAPI/GetPayoutBranchList");
                            var body = @"<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:tem=""http://tempuri.org/"" xmlns:grem=""http://schemas.datacontract.org/2004/07/GRemitWCFService.Send"">" + "\n" +
                            @"   <soapenv:Header/>" + "\n" +
                            @"   <soapenv:Body>" + "\n" +
                            @"      <tem:GetPayoutBranchList>" + "\n" +
                            @"         <!--Optional:-->" + "\n" +
                            @"         <tem:req>" + "\n" +
                            @"            <!--Optional:-->" + "\n" +
                            @"            <grem:CountryCode>" + countrycode + "</grem:CountryCode>" + "\n" +
                            @"            <!--Optional:-->" + "\n" +
                            @"            <grem:Password>" + apipass + "</grem:Password>" + "\n" +
                            @"            <!--Optional:-->" + "\n" +
                            @"            <grem:PaymentModeType>" + paymentmode + "</grem:PaymentModeType>" + "\n" +
                            @"            <!--Optional:-->" + "\n" +
                            @"            <grem:SecurityKey>" + accesscode + "</grem:SecurityKey>" + "\n" +
                            @"            <!--Optional:-->" + "\n" +
                            @"            <grem:UniqueID>" + apiuser + "</grem:UniqueID>" + "\n" +
                            @"         </tem:req>" + "\n" +
                            @"      </tem:GetPayoutBranchList>" + "\n" +
                            @"   </soapenv:Body>" + "\n" +
                            @"</soapenv:Envelope>";
                            request.AddParameter("text/xml; charset=utf-8", body, ParameterType.RequestBody);

                            IRestResponse response = client.Execute(request);
                            Console.WriteLine(response.Content);

                            XmlDocument xmlDoc = new XmlDocument();
                            xmlDoc = new XmlDocument();
                            xmlDoc.LoadXml(response.Content);
                            XmlNodeList nodeList = xmlDoc.GetElementsByTagName("Table");
                            string Country = "", paymentMode = "", branchCode = "", bankNM = "", branchAddress = "";
                            foreach (XmlNode node1 in nodeList)
                            {
                                string json = Newtonsoft.Json.JsonConvert.SerializeXmlNode(node1);
                                var obj1 = Newtonsoft.Json.Linq.JObject.Parse(json);
                                Country = Convert.ToString(obj1["Table"]["Country"]).Trim();
                                paymentMode = Convert.ToString(obj1["Table"]["PaymentMode"]).Trim();
                                branchCode = Convert.ToString(obj1["Table"]["BranchCode"]).Trim();
                                bankNM = Convert.ToString(obj1["Table"]["BranchName"]).Trim();
                                branchAddress = Convert.ToString(obj1["Table"]["BranchAddress"]).Trim().ToLower();

                                if (c.PaymentDepositType_ID == 1 && paymentMode == "Mobile Wallet / UPI") // BANK and Check PaymentMode
                                {
                                    continue;
                                }
                                if (c.PaymentDepositType_ID == 3 && paymentMode != "Mobile Wallet / UPI") // BANK and Check PaymentMode for Wallet
                                {
                                    continue;
                                }
                                ds.Rows.Add(branchCode, "", Country, "(" + bankNM + " : " + paymentMode + " : " + branchAddress + ")", api_id);
                            }
                            */

                            }
                            catch { }

                        }
                        if (api_id == 3)
                        {
                            try
                            {
                                var client = new RestClient("" + apiurl + "Token");
                                client.Timeout = -1;
                                var request = new RestRequest(Method.GET);
                                request.AddHeader("Content-Type", "application/json");
                                request.AddHeader("Api-Version", "6.0.0.0");
                                string encoded = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes("" + apiuser + "" + ":" + "" + apipass + ""));// User Name and Password
                                request.AddHeader("Authorization", "Basic " + encoded);
                                IRestResponse response = client.Execute(request);
                                Newtonsoft.Json.Linq.JObject obj = Newtonsoft.Json.Linq.JObject.Parse(response.Content);
                                if (response.Content.Contains("Token"))
                                {
                                    string token = Convert.ToString(obj["Token"]);
                                    client = new RestClient("" + apiurl + "BranchList");
                                    client.Timeout = -1;
                                    request = new RestRequest(Method.POST);
                                    request.AddHeader("Content-Type", "application/json");
                                    request.AddHeader("Api-Version", "6.0.0.0");
                                    request.AddHeader("Authorization", "Basic " + encoded);
                                    var para = CompanyInfo.Encrypt("{UserId:'" + apiuser + "',CompanyID:'" + apicompany_id + "',CountryCode:'" + countrycode + "',City:''}");
                                    request.AddParameter("application/json", "{\"jsonstring\": \"" + para + "\"}", ParameterType.RequestBody);
                                    request.AddHeader("HoryalSandbox", token); // Extra Parameter for DataField
                                    IRestResponse response1 = client.Execute(request);
                                    var obj1 = Newtonsoft.Json.Linq.JObject.Parse(response1.Content);
                                    var arr = obj1["Branch_List"];
                                    List<Branch_List> UserList = JsonConvert.DeserializeObject<List<Branch_List>>(Convert.ToString(obj1["Branch_List"]));
                                    ds = UserList.ToDataTable();
                                    ds.Columns.Add("API_ID", typeof(string));
                                    for (int j = 0; j < ds.Rows.Count; j++)
                                        ds.Rows[j]["API_ID"] = "API" + api_id;
                                }
                            }
                            catch { }
                        }
                        if (api_id == 4)//GLOREMIT
                        {
                            try
                            {
                                DataColumnCollection columns = ds.Columns;
                                if (!columns.Contains("Branchcode"))
                                {
                                    ds.Columns.Add("Branchcode", typeof(string)); ds.Columns.Add("City", typeof(string));
                                    ds.Columns.Add("Country", typeof(string)); ds.Columns.Add("Address", typeof(string)); ds.Columns.Add("API_ID", typeof(string));
                                }
                                if (c.PaymentDepositType_ID == 1)
                                {
                                    var client = new RestClient(apiurl + "supportedentities/BANK?ListName=BANK");
                                    client.Timeout = -1;
                                    var request = new RestRequest(Method.GET);
                                    string rspsign = "";
                                    request.AddHeader("Authorization", apicompany_id + " " + apiuser + ":" + apipass + ":" + rspsign);
                                    //request.AddHeader("Content-Type", "application/json");
                                    IRestResponse response = client.Execute(request);
                                    if (response.StatusCode.ToString() == "OK")
                                    {
                                        var obj1 = Newtonsoft.Json.Linq.JObject.Parse(response.Content);
                                        string list = Convert.ToString(obj1["List"]);
                                        if (list != null && list != "")
                                        {
                                            string[] Split = list.Split(',');
                                            foreach (string item in Split)
                                            {
                                                if (item != null && item != "")
                                                {
                                                    string[] arr = item.Split('|');
                                                    //arr = 0-Country Code,1-Code(issuer code,biller code,province code,regency code,etc),2-Name/Description
                                                    if (Convert.ToString(arr[0]) == countrycode)
                                                        ds.Rows.Add(arr[0] + "|" + arr[1], "", country, arr[2], api_id);
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    var client = new RestClient(apiurl + "supportedentities/CASH_PICKUP?ListName=CASH_PICKUP");
                                    client.Timeout = -1;
                                    var request = new RestRequest(Method.GET);
                                    string rspsign = "";
                                    request.AddHeader("Authorization", apicompany_id + " " + apiuser + ":" + apipass + ":" + rspsign);
                                    //request.AddHeader("Content-Type", "application/json");
                                    IRestResponse response = client.Execute(request);
                                    CompanyInfo.InsertActivityLogDetailsSecurity("App Tranglo Collection Points: " + response.StatusCode + response.ErrorException + " " + response.ErrorMessage, Convert.ToInt32(c.User_ID), 0, Convert.ToInt32(c.User_ID), 0, "srvRates", Convert.ToInt32(c.Branch_ID), Convert.ToInt32(c.Client_ID), "Get_BKLBranch", 0,_srvRatesHttpContext);
                                    if (response.StatusCode.ToString() == "OK")
                                    {
                                        var obj1 = Newtonsoft.Json.Linq.JObject.Parse(response.Content);
                                        string list = Convert.ToString(obj1["List"]);
                                        if (list != null && list != "")
                                        {
                                            string[] Split = list.Split(',');
                                            foreach (string item in Split)
                                            {
                                                if (item != null && item != "")
                                                {
                                                    string[] arr = item.Split('|');
                                                    //arr = 0-Country Code,1-Code(issuer code,biller code,province code,regency code,etc),2-Name/Description
                                                    if (Convert.ToString(arr[0]) == countrycode)
                                                        ds.Rows.Add(arr[0] + "|" + arr[1], "", country, arr[2], api_id);
                                                }
                                            }
                                        }
                                    }
                                }

                            }
                            catch { }
                        }
                        if (api_id == 5)//HKPinoy
                        {
                            DataColumnCollection columns = ds.Columns;
                            if (!columns.Contains("Branchcode"))
                            {
                                ds.Columns.Add("Branchcode", typeof(string)); ds.Columns.Add("City", typeof(string));
                                ds.Columns.Add("Country", typeof(string)); ds.Columns.Add("Address", typeof(string)); ds.Columns.Add("API_ID", typeof(string));
                            }
                            try
                            {
                                var client = new RestClient(apiurl + "collectionPoint/getCollectionPoints");
                                client.Timeout = -1;
                                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                                var request = new RestRequest(Method.POST);
                                request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                                request.AddHeader("Cookie", "PHPSESSID=c35lbvc8r9cjbe80n78g5932se; authchallenge=4d9cf560a62ceb64e39022be07068c6b");
                                request.AddParameter("username", apiuser);
                                request.AddParameter("password", apipass);
                                request.AddParameter("pin", accesscode);
                                request.AddParameter("destination_country_code", countrycode);
                                IRestResponse response = client.Execute(request);
                                if (response.StatusCode.ToString() == "OK")
                                {
                                    XmlDocument xmlDoc = new XmlDocument();
                                    xmlDoc.LoadXml(response.Content);

                                    XmlNodeList nodeList = xmlDoc.GetElementsByTagName("collection_points");
                                    foreach (XmlNode node1 in nodeList)
                                    {
                                        foreach (XmlNode child1 in node1.ChildNodes)
                                        {
                                            if (child1.Name == "collection_point")
                                            {
                                                string collid = "", collcode = "", collname = "", bankid = "", bank = "", address = "", city = "", state = "", telephone = "", country_id = "";
                                                foreach (XmlNode child in child1.ChildNodes)
                                                {
                                                    if (child.Name == "collection_id") { collid = child.InnerText; }
                                                    if (child.Name == "code") { collcode = child.InnerText; }
                                                    if (child.Name == "name") { collname = child.InnerText; }
                                                    if (child.Name == "bank") { bank = child.InnerText; }
                                                    if (child.Name == "delivery_bank") { bankid = child.InnerText; }
                                                    if (child.Name == "address") { address = child.InnerText; }
                                                    if (child.Name == "city") { city = child.InnerText; }
                                                    if (child.Name == "state") { state = child.InnerText; }
                                                    if (child.Name == "telephone") { telephone = child.InnerText; }
                                                    //if (child.Name == "collection_pin_prefix") { collection_pin = child.InnerText; }
                                                    if (child.Name == "country_id") { country_id = child.InnerText; }
                                                }
                                                string collect_details = collid + "|" + collcode + "|" + bankid + "|" + collname + "," + bank + "," + address;
                                                if (collid != "" && collid != null)
                                                    ds.Rows.Add(collect_details, city, country, address, api_id);
                                            }
                                        }
                                    }
                                }
                            }
                            catch { }
                        }
                        if (api_id == 6)//Esewa
                        {
                            DataColumnCollection columns = ds.Columns;
                            if (!columns.Contains("Branchcode"))
                            {
                                ds.Columns.Add("Branchcode", typeof(string)); ds.Columns.Add("City", typeof(string));
                                ds.Columns.Add("Country", typeof(string)); ds.Columns.Add("Address", typeof(string)); ds.Columns.Add("API_ID", typeof(string));
                            }
                            if (country.ToLower() == "nepal")
                            {
                                ds.Rows.Add("Esewa", "", country, "", api_id);
                            }
                        }
                        if (api_id == 7)//HNB
                        {
                            DataColumnCollection columns = ds.Columns;
                            if (!columns.Contains("Branchcode"))
                            {
                                ds.Columns.Add("Branchcode", typeof(string)); ds.Columns.Add("City", typeof(string));
                                ds.Columns.Add("Country", typeof(string)); ds.Columns.Add("Address", typeof(string)); ds.Columns.Add("API_ID", typeof(string));
                            }
                            //if (country.ToLower() == "sri lanka")
                            //{
                            ds.Rows.Add("HNB", "", country, "", api_id);
                            //}
                        }
                        else if (api_id == 8)
                        {
                            DataColumnCollection columns = ds.Columns;
                            if (!columns.Contains("Branchcode"))
                            {
                                ds.Columns.Add("Branchcode", typeof(string)); ds.Columns.Add("City", typeof(string));
                                ds.Columns.Add("Country", typeof(string)); ds.Columns.Add("Address", typeof(string)); ds.Columns.Add("API_ID", typeof(string));
                            }
                            ds.Rows.Add("MyZeePay", "", country, "", api_id);
                        }
                        else if (api_id == 9)
                        {
                            DataColumnCollection columns = ds.Columns;
                            if (!columns.Contains("Branchcode"))
                            {
                                ds.Columns.Add("Branchcode", typeof(string)); ds.Columns.Add("City", typeof(string));
                                ds.Columns.Add("Country", typeof(string)); ds.Columns.Add("Address", typeof(string)); ds.Columns.Add("API_ID", typeof(string));
                            }
                            ds.Rows.Add("AZA", "", country, "", api_id);
                        }
                        else if (api_id == 10)
                        {
                            DataColumnCollection columns = ds.Columns;
                            if (!columns.Contains("Branchcode"))
                            {
                                ds.Columns.Add("Branchcode", typeof(string)); ds.Columns.Add("City", typeof(string));
                                ds.Columns.Add("Country", typeof(string)); ds.Columns.Add("Address", typeof(string)); ds.Columns.Add("API_ID", typeof(string));
                            }
                            ds.Rows.Add("INDUSIND", "", country, "", api_id);
                        }
                        else if (api_id == 1)
                        {
                            DataColumnCollection columns = ds.Columns;
                            if (!columns.Contains("Branchcode"))
                            {
                                ds.Columns.Add("Branchcode", typeof(string)); ds.Columns.Add("City", typeof(string));
                                ds.Columns.Add("Country", typeof(string)); ds.Columns.Add("Address", typeof(string)); ds.Columns.Add("API_ID", typeof(string));
                            }
                            ds.Rows.Add("Sampath", "", country, "", api_id);
                        }
                        else if (api_id == 15)
                        {
                            #region AmalGetCity
                            DataColumnCollection columns = ds.Columns;
                            if (!columns.Contains("Branchcode"))
                            {
                                ds.Columns.Add("Branchcode", typeof(string)); ds.Columns.Add("City", typeof(string));
                                ds.Columns.Add("Country", typeof(string)); ds.Columns.Add("Address", typeof(string)); ds.Columns.Add("API_ID", typeof(string));
                            }

                            if (c.PaymentDepositType_ID == 1) // Just for Bank Accounts
                            {
                                ds.Rows.Add("Head Office", "", country, "", api_id);
                            }
                            else
                            {
                                MySqlConnector.MySqlCommand cmd_select = new MySqlConnector.MySqlCommand("select Country_Code from api_country_codes where country_id = " + c.Country_ID + " and api_id=" + api_id);
                                DataTable dt_country = db_connection.ExecuteQueryDataTableProcedure(cmd_select);
                                string countryCodeAmal = dt_country.Rows[0]["Country_Code"].ToString();

                                cmd_select = new MySqlConnector.MySqlCommand("select a.ID,a.Bank_Name,a.api_Fields,cast(AES_DECRYPT(UNHEX(API_URL), '" + CompanyInfo.SecurityKey() + "') as  char(500)) as API_URL,cast(AES_DECRYPT(UNHEX(APIUser_ID), '" + CompanyInfo.SecurityKey() + "') as  char(500)) as APIUser_ID,cast(AES_DECRYPT(UNHEX(APIAccess_Code),'" + CompanyInfo.SecurityKey() + "') as  char(500)) as APIAccess_Code,APICompany_ID,cast(AES_DECRYPT(UNHEX(Password), '" + CompanyInfo.SecurityKey() + "') as  char(500)) as Password from api_master a where ID = " + api_id);
                                //cmd_select = new MySqlCommand("select * from api_master where ID = " + api_id);
                                dt_country = db_connection.ExecuteQueryDataTableProcedure(cmd_select);
                                string userId = dt_country.Rows[0]["APIUser_ID"].ToString();
                                string password = dt_country.Rows[0]["Password"].ToString();
                                string apiURL = dt_country.Rows[0]["API_URL"].ToString();
                                string cred = Convert.ToBase64String(Encoding.Default.GetBytes(userId + ":" + password));
                                string token = tokengeneratation(apiURL, cred);

                                string api_fields = ""; string password_api = ""; string clientkey_api = ""; string SourceBranchkey_api = "";
                                api_fields = dt_country.Rows[0]["api_Fields"].ToString();
                                if (api_fields != "" && api_fields != null)
                                {
                                    Newtonsoft.Json.Linq.JObject objforkey = Newtonsoft.Json.Linq.JObject.Parse(api_fields);
                                    password_api = Convert.ToString(objforkey["password"]);
                                    clientkey_api = Convert.ToString(objforkey["clientkey"]);
                                    SourceBranchkey_api = Convert.ToString(objforkey["SourceBranchkey"]);
                                }

                                Newtonsoft.Json.Linq.JObject json = getCities(userId, apiURL, password_api, clientkey_api, SourceBranchkey_api, token, countryCodeAmal);
                                var arr1 = (json["response"]["result"]["Cities"]["City"]);
                                foreach (Newtonsoft.Json.Linq.JObject item in arr1)
                                {
                                    string CityId = item.GetValue("CityId").ToString().Trim();
                                    string CityName = item.GetValue("CityName").ToString().ToUpper().Trim();
                                    //if (obj.City_ID.ToString() != CityId)
                                    //ds.Rows.Add(CityName, CityId);
                                    ds.Rows.Add(CityName, "", country, CityId, api_id);
                                }
                            }
                            #endregion
                        }
                        else if (api_id == 18)  // Collection point Crosspay
                        {
                            #region Crosspay_Collection_Points_list
                            DataColumnCollection columns = ds.Columns;
                            if (!columns.Contains("Branchcode"))
                            {
                                ds.Columns.Add("Branchcode", typeof(string)); ds.Columns.Add("City", typeof(string));
                                ds.Columns.Add("Country", typeof(string)); ds.Columns.Add("Address", typeof(string)); ds.Columns.Add("API_ID", typeof(string));
                            }
                            string origin = "";
                            try
                            {
                                string api_fieldsVal = Convert.ToString(dt.Rows[i]["api_Fields"]);

                                if (api_fieldsVal != "" && api_fieldsVal != null)
                                {
                                    Newtonsoft.Json.Linq.JObject obj = Newtonsoft.Json.Linq.JObject.Parse(api_fieldsVal);
                                    origin = Convert.ToString(obj["origin"]);
                                }
                            }
                            catch (Exception ex) { }
                            try
                            {
                                var client = new RestClient(apiurl);
                                if (c.PaymentDepositType_ID == 2) // If CASH Pickup transaction
                                {
                                    client = new RestClient(apiurl + "services/agents");
                                }
                                if (c.PaymentDepositType_ID == 3) // If Mobile Wallet transaction
                                {
                                    client = new RestClient(apiurl + "services/wallets");
                                }
                                if (c.PaymentDepositType_ID == 1) // If Bank transaction
                                {
                                    client = new RestClient(apiurl + "services/banks");
                                }

                                client.Timeout = -1;
                                string countryCode = "\"" + countrycode + "\"";
                                var request = new RestRequest(Method.POST);
                                request.AddHeader("X-API-KEY", accesscode);
                                request.AddHeader("origin", origin);
                                request.AddHeader("Content-Type", "application/json");
                                var body = @"{" + "\n" +
                                @"    ""destinationCountryCode"" :    " + countryCode + " }";

                                request.AddParameter("application/json", body, ParameterType.RequestBody);
                                IRestResponse response = client.Execute(request);
                                Console.WriteLine(response.Content);


                                dynamic dynJson = JsonConvert.DeserializeObject(response.Content);
                                foreach (var item in dynJson)
                                {
                                    if (c.PaymentDepositType_ID == 2) // If CASH Pickup transaction                            
                                        ds.Rows.Add(item.agentName, item.agentBranchName, country, "", api_id);
                                    else if (c.PaymentDepositType_ID == 1) // If Bank transfer transaction  
                                        ds.Rows.Add(item.bankName, item.sortCode, country, "", api_id);
                                    else if (c.PaymentDepositType_ID == 3) // If Mobile Wallet transfer transaction  
                                        ds.Rows.Add(item.name, item.providerCode, country, "", api_id);
                                }

                            }
                            catch (Exception ex)
                            {
                                // mtsmethods.InsertActivityLogDetails("Get Crosspay Collection Points List Error : <br/>" + ex.ToString() + "", user_id, tId, user_id, BankPOCUser_ID, "Get BAHL BANK List", branch_id, client_id);
                            }
                            #endregion
                        }
                        else if (api_id == 26)  // Collection point Bakaal
                        {   // Bakaal Cash pickup collection points

                            #region Bakaal_Collection_Points_list
                            DataColumnCollection columns = ds.Columns;
                            if (!columns.Contains("Branchcode"))
                            {
                                ds.Columns.Add("Branchcode", typeof(string)); ds.Columns.Add("City", typeof(string));
                                ds.Columns.Add("Country", typeof(string)); ds.Columns.Add("Address", typeof(string)); ds.Columns.Add("API_ID", typeof(string));
                            }

                            try
                            {
                                string ServiceType = "";
                                if (c.PaymentDepositType_ID == 1)
                                {   // If Bank transfer transaction 
                                    ServiceType = "BANK_DEPOSIT";
                                }
                                else if (c.PaymentDepositType_ID == 2)
                                {   // If CASH Pickup transaction  
                                    ServiceType = "REMITTANCE";
                                }
                                else if (c.PaymentDepositType_ID == 3)
                                {   // If Mobile Wallet transfer transaction  
                                    ServiceType = "MOBILE_MONEY";
                                }

                                // Token generation API Call
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
                                IRestResponse response = client.Execute(request);
                                dynamic json = Newtonsoft.Json.JsonConvert.DeserializeObject(response.Content);
                                string token = json.token;

                                // Collection Point API Call
                                client = new RestClient(apiurl + "ServiceCatalog");
                                client.Timeout = -1;
                                request = new RestRequest(Method.POST);
                                request.AddHeader("Authorization", token);
                                request.AddHeader("Content-Type", "application/json");
                                body = @"{
" + "\n" +
                                @"""ServiceType"": """ + ServiceType + @""",
" + "\n" +
                                @"""CountryCode"": """ + countrycode + @""",
" + "\n" +
                                @"""UserId"": """ + apiuser + @"""
" + "\n" +
                                @"}";
                                request.AddParameter("application/json", body, ParameterType.RequestBody);
                                response = client.Execute(request);
                                json = Newtonsoft.Json.JsonConvert.DeserializeObject(response.Content);
                                dynamic result = json["data"];

                                foreach (var item in result)
                                {
                                    if (c.PaymentDepositType_ID == 1)
                                    {   // Bank
                                        ds.Rows.Add(item.serviceDesc, item.serviceId, country, "", api_id);
                                    }
                                    else if (c.PaymentDepositType_ID == 2)
                                    {   // Cashpickup
                                        ds.Rows.Add(item.serviceDesc, item.serviceId, country, "", api_id);
                                    }
                                    else if (c.PaymentDepositType_ID == 3)
                                    {   // Mobile wallet
                                        ds.Rows.Add(item.serviceDesc, item.serviceId, country, "", api_id);
                                    }
                                }

                            }
                            catch (Exception ex)
                            {
                            }
                            #endregion

                        }


                    }
                }
                return ds;
            }
            catch (Exception ex)
            {
                int stattus = (int)CompanyInfo.InsertActivityLogDetailsSecurity(ex.ToString(), Convert.ToInt32(c.User_ID), 0, Convert.ToInt32(c.User_ID), 0, "srvRates", Convert.ToInt32(c.Branch_ID), Convert.ToInt32(c.Client_ID), "Get_BKLBranch", 0,_srvRatesHttpContext);
                //string stattus = (string)CompanyInfo.InsertErrorLogDetails(ex.ToString().Replace("\'", "\\'"), c.User_ID, "Get_APIDetails", c.Branch_ID, c.Client_ID);
                return ds;
            }
        }

        public string tokengeneratation(string apiURL, string cred)
        {
            var client = new RestClient(apiURL + "/Auth/Token");
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            request.AddHeader("Authorization", "Basic " + cred);

            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            System.Net.ServicePointManager.ServerCertificateValidationCallback += delegate { return true; };
            IRestResponse response = client.Execute(request);
            Console.WriteLine(response.Content);
            dynamic stuff = JsonConvert.DeserializeObject(response.Content);

            string token = stuff.response.result.token;
            return token;
        }
        public Newtonsoft.Json.Linq.JObject getCities(string Username_api, string url, string password_api, string clientkey_api, string SourceBranchkey_api, string token, string country_id)
        {
            Newtonsoft.Json.Linq.JObject json = new Newtonsoft.Json.Linq.JObject();
            try
            {

                var client = new RestClient(url + "/Services/GetCitiesByCountryId");
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                request.AddHeader("Authorization", "Bearer " + token);
                request.AddHeader("Content-Type", "application/json");
                var body = @"{
                    " + "\n" +
                        @"  ""username"": """ + Username_api + @""",
                    " + "\n" +
                        @"	""password"": """ + password_api + @""",
                    " + "\n" +
                        @"	""clientkey"": """ + clientkey_api + @""",
                    " + "\n" +
                        @"    ""countryId"": " + country_id + @",
                    " + "\n" +
                        @"  ""sourceBranchkey"": """ + SourceBranchkey_api + @"""
                    " + "\n" +
                        @"
                    " + "\n" +
                        @" 
                    " + "\n" +
                        @"}";
                request.AddParameter("application/json", body, ParameterType.RequestBody);
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                System.Net.ServicePointManager.ServerCertificateValidationCallback += delegate { return true; };


                IRestResponse response = client.Execute(request);
                json = Newtonsoft.Json.Linq.JObject.Parse(response.Content);

                Console.WriteLine(response.Content);
                return json;
            }
            catch (Exception ex)
            {
                return json;
            }
        }

        class Branch_List
        {
            public string Branchcode { set; get; }
            public string City { set; get; }
            public string Country { set; get; }
            public string Address { set; get; }
        }
        public DataTable GetRates_calculator(Model.Transaction obj)
        {
            List<Model.Currency> _lst = new List<Model.Currency>();
            MySqlConnector.MySqlCommand cmd = new MySqlConnector.MySqlCommand("Rates_Search");
            cmd.CommandType = CommandType.StoredProcedure;
            string whereclause = " ";
            if (obj.Country_ID > 0)
            {
                whereclause = whereclause + " and t.Country_ID = " + obj.Country_ID + "";
            }
            if (obj.Currency_ID > 0)
            {
                whereclause = whereclause + " and t.Currency_ID = " + obj.Currency_ID + "";
            }
            if (obj.CB_ID > 0)
            {
                whereclause = whereclause + " and t.CB_ID = " + obj.CB_ID + "";
            }
            if (obj.PType_ID > 0)
            {
                whereclause = whereclause + " and t.PType_ID = " + obj.PType_ID + "";
            }
            if (obj.PaymentDepositType_ID > 0)
            {
                whereclause = whereclause + " and t.PayDepositType_ID = " + obj.PaymentDepositType_ID + "";
            }
            if (obj.DeliveryType_Id > 0)
            {
                whereclause = whereclause + " and t.DeliveryType_ID = " + obj.DeliveryType_Id + "";
            }
            obj.AmountInGBP = Convert.ToDouble(obj.TransferAmount);
            if (obj.AmountInGBP != null && obj.AmountInGBP != 0)
            {
                whereclause = whereclause + " and t.Min_Amount <= " + obj.AmountInGBP + " and t.Max_Amount >= " + obj.AmountInGBP + "";
            }
            obj.AmountInPKR = Convert.ToDouble(obj.TransferForeignAmount);
            if (obj.AmountInPKR != null && obj.AmountInPKR != 0)
            {
                whereclause = whereclause + " and t.Foreign_Currency_Min_Amount <= " + obj.AmountInPKR + " and t.Foreign_Currency_Max_Amount >= " + obj.AmountInPKR + "";
            }

            cmd.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
            cmd.Parameters.AddWithValue("_whereclause", whereclause);
            DataTable dt = db_connection.ExecuteQueryDataTableProcedure(cmd);
            return dt;
        }
    }
}
