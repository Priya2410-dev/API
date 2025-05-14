using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 
using System.Data;
using System.Configuration;
using MySqlConnector;
namespace Calyx_Solutions.Service
{
    public class srvCommon
    {
        public static string SecurityKey()
        {
           
            return CompanyInfo.SecurityKey();
        }

        public static string Select_Customer_Refernce_Number(Model.Customer obj)
        {
            try
            {
                string _SecurityKey = SecurityKey();

                MySqlCommand cmd = new MySqlCommand("sp_select_company_details_by_param");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("_securityKey", _SecurityKey);
                cmd.Parameters.AddWithValue("_clientId", obj.Client_ID);

                DataTable dtIntial = db_connection.ExecuteQueryDataTableProcedure(cmd);

                string initialchars = dtIntial.Rows[0]["CustRef_InitialChar"].ToString();

                string dt = DateTime.Now.ToString("dd/MM/yy");
                string fpart = dt.Substring(3, 2);
                string spart = dt.Substring(6, 2);
                string lpart = "";
                if (Convert.ToString(obj.Id).Length < 4)
                {
                    lpart = obj.Id.ToString().PadLeft(4, '0');
                }
                else
                {
                    lpart = Convert.ToString(obj.Id);
                }
                string no = initialchars + fpart + spart + lpart;

                return no;
            }


            catch (Exception _x)
            {
                throw _x;
            }


        }

        public static string Create_Customer_Referral_Code(Model.Customer obj)//sanket
        {
            try
            {
                string fpart = obj.FirstName.Replace(" ", "").Trim() + obj.LastName.Replace(" ", "").Trim();
                fpart = fpart.Substring(0, Math.Min(fpart.Length, 4)).ToUpper();
                if (fpart.Length < 4)
                {
                    fpart = fpart.PadRight(4, '0');
                }
                //string spart = obj.LastName.Replace(" ", "").Trim();
                //spart = spart.Substring(0, Math.Min(spart.Length, 4));
                string lpart = "";
                if (Convert.ToString(obj.Id).Length < 4)
                {
                    lpart = obj.Id.ToString().PadLeft(4, '0');
                }
                else
                {
                    lpart = Convert.ToString(obj.Id);
                }
                string code = fpart + lpart;



                return code;
            }
            catch (Exception _x)
            {
                throw _x;
            }


        }

        public List<Model.HeardFrom> Select_Heard_From()
        {
            List<Model.HeardFrom> _lst = new List<Model.HeardFrom>();

            MySqlCommand _cmd = new MySqlCommand("sp_select_heard_from");
            _cmd.CommandType = CommandType.StoredProcedure;

            DataTable dt = db_connection.ExecuteQueryDataTableProcedure(_cmd);
            Model.HeardFrom _obj = new Model.HeardFrom();

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    _obj = new Model.HeardFrom();

                    if (row["Heard_From_Id"] != DBNull.Value)
                    {
                        _obj.Id = Convert.ToInt32(row["Heard_From_Id"].ToString());
                    }
                    if (row["Heard_From_Option"] != DBNull.Value)
                    {
                        _obj.HeardFromOption = (row["Heard_From_Option"].ToString());
                    }

                    _lst.Add(_obj);
                }
            }
            return _lst;
        }

        public DataTable GetAppTexts(Model.Customer obj, HttpContext context)
        {
            int Customer_ID = 0;
            DataTable dt = new DataTable();
            try
            {
                List<Model.Customer> _lst = new List<Model.Customer>();
                string Client_ID_regex = validation.validate(Convert.ToString(obj.Client_ID), 0, 1, 1, 1, 1, 1, 1, 1, 1);
                string Id_regex = validation.validate(Convert.ToString(Customer_ID), 0, 1, 1, 1, 1, 1, 1, 1, 1);
                if (Client_ID_regex != "false" && Id_regex != "false")
                {
                    string whereclause = "";
                    MySqlCommand _cmd = new MySqlCommand("get_app_text");
                    _cmd.CommandType = CommandType.StoredProcedure;
                    _cmd.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                    _cmd.Parameters.AddWithValue("_whereclause", whereclause);
                    dt = db_connection.ExecuteQueryDataTableProcedure(_cmd);
                }
                else
                {
                    string msg = "Validation Error Client_ID_regex-" + Client_ID_regex + "Id_regex-" + Id_regex;
                    int stattus = (int)CompanyInfo.InsertActivityLogDetails(msg, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Customer_ID, "GetAppTexts", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "GetAppTexts", context);
                }
            }
            catch (Exception ex)
            {
                string msg = "Api GetAppTexts: " + ex.ToString() + " ";
                CompanyInfo.InsertErrorLogTracker(msg, 0, 0, 0, 0, "GetAppTexts", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "", context);
            }
            return dt;
        }

        public string UpdateBankStatus(Model.Beneficiary obj, HttpContext context)  //Parvej Added
        {
            string result = string.Empty;
            try
            {
                MySqlCommand cmd = new MySqlCommand("Update_benf_bank_status");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("_Id", obj.Id);
                cmd.Parameters.AddWithValue("_BBDetails_ID", obj.BBDetails_ID);
                cmd.Parameters.AddWithValue("_status", obj.status);
                //cmd.Parameters.AddWithValue("_Flag", obj.whereclause);//flag name
                cmd.Parameters.Add(new MySqlParameter("_Name", MySqlDbType.String));
                cmd.Parameters["_Name"].Direction = ParameterDirection.Output;

                cmd.Parameters.Add(new MySqlParameter("_Bank", MySqlDbType.String));
                cmd.Parameters["_Bank"].Direction = ParameterDirection.Output;

                int n = db_connection.ExecuteNonQueryProcedure(cmd);
                if (n > 0)//success
                {
                    string name = Convert.ToString(cmd.Parameters["_Name"].Value);
                    string bank = Convert.ToString(cmd.Parameters["_Bank"].Value);
                    if (obj.status == 1)
                    {

                        int Customer_ID = Convert.ToInt32(CompanyInfo.Decrypt(Convert.ToString(obj.Customer_ID), true));
                        CompanyInfo.InsertActivityLogDetails("Beneficiary Name " + name + " Inactivated the bank :  " + bank + "", 0, 0, 0, Convert.ToInt32(Customer_ID), "UpdateBankStatus", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "SrvCommon", context);
                    }
                    if (obj.status == 0)
                    {
                        int Customer_ID = Convert.ToInt32(CompanyInfo.Decrypt(Convert.ToString(obj.Customer_ID), true));
                        CompanyInfo.InsertActivityLogDetails("Beneficiary Name " + name + "  Activated the bank : " + bank + "", 0, 0, 0, Convert.ToInt32(Customer_ID), "UpdateBankStatus", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "SrvCommon", context);
                    }
                    result = "success";
                }
                else//failed
                {
                    result = "notsuccess";
                }

            }
            catch (Exception _x)
            {
                //  throw _x;
            }
            return result;
        }

    }
}
