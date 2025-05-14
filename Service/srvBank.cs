using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Data;

namespace Calyx_Solutions.Service
{
    public class srvBank
    {

        public List<Model.Bank> Select_BankBranch_Master(Model.Bank _BankObj)
        {
            List<Model.Bank> _lst = new List<Model.Bank>();

            MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("sp_select_bankbranch_master");
            _cmd.CommandType = CommandType.StoredProcedure;
            _cmd.Parameters.AddWithValue("_BankId", _BankObj.Id);
            DataTable dt = db_connection.ExecuteQueryDataTableProcedure(_cmd);
            Model.Bank _obj = new Model.Bank();

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    _obj = new Model.Bank();
                    // _obj.Country = new Model.Country();

                    if (row["BankBranch_ID"] != DBNull.Value)
                    {
                        _obj.Id = Convert.ToInt32(row["BankBranch_ID"].ToString());
                    }
                    if (row["Branch_Name"] != DBNull.Value && row["Branch_code"] != DBNull.Value)
                    {
                        _obj.Name = (row["Branch_Name"].ToString() + " (" + row["Branch_code"].ToString()) + ")";
                    }

                    _lst.Add(_obj);
                }
            }
            return _lst;
        }
        public List<Model.Bank> Select_Bank_Master(Model.Country _CountryObj)
        {
            List<Model.Bank> _lst = new List<Model.Bank>();

            MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("sp_select_bank_master");
            _cmd.CommandType = CommandType.StoredProcedure;
            _cmd.Parameters.AddWithValue("_CountryId", _CountryObj.Id);
            DataTable dt = db_connection.ExecuteQueryDataTableProcedure(_cmd);
            Model.Bank _obj = new Model.Bank();

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    _obj = new Model.Bank();
                    // _obj.Country = new Model.Country();

                    if (row["Bank_ID"] != DBNull.Value)
                    {
                        _obj.Id = Convert.ToInt32(row["Bank_ID"].ToString());
                    }
                    if (row["Bank_Name"] != DBNull.Value)
                    {
                        _obj.Name = (row["Bank_Name"].ToString());
                    }
                    if (row["BIC_Codes"] != DBNull.Value)
                    {
                        _obj.BICcodes = (row["BIC_Codes"].ToString());
                    }

                    _lst.Add(_obj);
                }
            }
            return _lst;
        }
        public List<Model.Bank> GetBankCodes_ByBank(Model.Bank _Obj)
        {
            List<Model.Bank> _lst = new List<Model.Bank>();

            MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("SP_BankcodesBy_BankID");
            _cmd.CommandType = CommandType.StoredProcedure;
            _cmd.Parameters.AddWithValue("_Bank_ID", _Obj.Id);

            DataTable dt = db_connection.ExecuteQueryDataTableProcedure(_cmd);

            Model.Bank _obj = new Model.Bank();

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    _obj = new Model.Bank();
                    // _obj.Country = new Model.Country();

                    if (row["Bankcode"] != DBNull.Value)
                    {
                        _obj.Code = Convert.ToString(row["Bankcode"].ToString());
                    }
                    //if (row["City_Name"] != DBNull.Value)
                    //{
                    //    _obj.Name = (row["City_Name"].ToString());
                    //}

                    _lst.Add(_obj);
                }
            }
            return _lst;
        }
        public List<Model.Bank> GetBICCodes_ByBank(Model.Bank _Obj)
        {
            List<Model.Bank> _lst = new List<Model.Bank>();

            MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("SP_BICcodesBy_BankID");
            _cmd.CommandType = CommandType.StoredProcedure;
            _cmd.Parameters.AddWithValue("_Bank_ID", _Obj.Id);

            DataTable dt = db_connection.ExecuteQueryDataTableProcedure(_cmd);

            Model.Bank _obj = new Model.Bank();

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    _obj = new Model.Bank();
                    // _obj.Country = new Model.Country();

                    if (row["BIC_Codes"] != DBNull.Value)
                    {
                        _obj.Code = Convert.ToString(row["BIC_Codes"].ToString());
                    }
                    //if (row["City_Name"] != DBNull.Value)
                    //{
                    //    _obj.Name = (row["City_Name"].ToString());
                    //}

                    _lst.Add(_obj);
                }
            }
            return _lst;
        }


        public DataTable BenfBankInfo(Model.Beneficiary obj , HttpContext context)
        {
            int Customer_ID = Convert.ToInt32(CompanyInfo.Decrypt(obj.Customer_ID, true));
            string Activity = string.Empty; string[] Alert_Msg = new string[7]; string sendmsg = string.Empty; string notification_icon = ""; string notification_message = "";
            
            string Username = "";
            string error_invalid_data = "";
            string error_msg = "";
            string Client_ID_regex = validation.validate(Convert.ToString(obj.Client_ID), 0, 1, 1, 1, 1, 1, 1, 1, 1);
            string Beneficiary_ID_regex = validation.validate(Convert.ToString(obj.Beneficiary_ID), 0, 1, 1, 1, 1, 1, 1, 1, 1);
            string Delete_Status_regex = validation.validate(Convert.ToString(obj.Delete_Status), 0, 1, 1, 1, 1, 1, 1, 1, 1);
            string Customer_ID_regex = validation.validate(Convert.ToString(Customer_ID), 0, 1, 1, 1, 1, 1, 0, 1, 1);
            if (Client_ID_regex == "" && Beneficiary_ID_regex == "" && Delete_Status_regex == "" && Customer_ID_regex == "")
            {
                Client_ID_regex = "true";
                Beneficiary_ID_regex = "true";
                Delete_Status_regex = "true";
                Customer_ID_regex = "true";
               
            }
            DataTable dt = new DataTable();
           
            if (Client_ID_regex != "false" && Beneficiary_ID_regex != "false" && Delete_Status_regex != "false" && Customer_ID_regex != "false")
            {
                List<Model.Beneficiary> _lst = new List<Model.Beneficiary>();
               
              MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("Get_Beneficiary_Banks");
                _cmd.CommandType = CommandType.StoredProcedure;
                string whereclause = " ";
                if (obj.Beneficiary_ID > 0)
                {
                    whereclause = whereclause + " and bb.Beneficiary_ID=" + obj.Beneficiary_ID + "";
                }
                if (Customer_ID != null && Customer_ID > 0)
                {
                    whereclause = whereclause + " and bb.Customer_ID=" + Customer_ID + "";
                }
                
                _cmd.Parameters.AddWithValue("_whereclause", whereclause);
                dt = db_connection.ExecuteQueryDataTableProcedure(_cmd);
            }
            else
            {
                string msg = "Validation Error Client_ID_regex- +" + Client_ID_regex + "Beneficiary_ID_regex- " + Beneficiary_ID_regex + " Delete_Status_regex- +" + Delete_Status_regex + " +Customer_ID_regex- " + Customer_ID_regex;
                int stattus = (int)CompanyInfo.InsertActivityLogDetailsSecurity(msg, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), 1, "srvDiscount", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "create", 0, context);
            }
            return dt;
        }
    }
}
