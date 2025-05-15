using System.Data;
using MySqlConnector;

namespace Calyx_Solutions.Service
{
    public class srvCountry
    {
        public List<Model.Country> Select_All_Countries(Model.Country obj)
        {
            List<Model.Country> _lst = new List<Model.Country>();

            MySqlCommand _cmd = new MySqlCommand("SP_Select_All_Countries");
            _cmd.CommandType = CommandType.StoredProcedure;
            _cmd.Parameters.AddWithValue("_Client_ID", obj.Client_ID);

            DataTable dt = db_connection.ExecuteQueryDataTableProcedure(_cmd);

            Model.Country _obj = new Model.Country();

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    _obj = new Model.Country();
                    _obj.Currency = new Model.Currency();

                    if (row["Country_ID"] != DBNull.Value)
                    {
                        _obj.Id = Convert.ToInt32(row["Country_ID"].ToString());
                    }
                    if (row["Country_Name"] != DBNull.Value)
                    {
                        _obj.Name = (row["Country_Name"].ToString());
                    }
                    if (row["Country_Code"] != DBNull.Value)
                    {
                        _obj.Code = (row["Country_Code"].ToString());
                    }

                    if (row["Country_Currency"] != DBNull.Value)
                    {
                        _obj.CurrencyCode = (row["Country_Currency"].ToString());
                    }
                    _lst.Add(_obj);
                }
            }

            return _lst;
        }
        public List<Model.Country> Select_Country_Master(Model.Country obj)
        {
            List<Model.Country> _lst = new List<Model.Country>();

            MySqlCommand _cmd = new MySqlCommand("sp_select_country_master");
            _cmd.CommandType = CommandType.StoredProcedure;
            _cmd.Parameters.AddWithValue("_ClientId", obj.Client_ID);

            DataTable dt = db_connection.ExecuteQueryDataTableProcedure(_cmd);

            Model.Country _obj = new Model.Country();

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    _obj = new Model.Country();
                    _obj.Currency = new Model.Currency();

                    if (row["Country_ID"] != DBNull.Value)
                    {
                        _obj.Id = Convert.ToInt32(row["Country_ID"].ToString());
                    }
                    if (row["Country_Name"] != DBNull.Value)
                    {
                        _obj.Name = (row["Country_Name"].ToString());
                    }
                    if (row["Country_Code"] != DBNull.Value)
                    {
                        _obj.Code = (row["Country_Code"].ToString());
                    }

                    if (row["Country_Currency"] != DBNull.Value)
                    {
                        _obj.CurrencyCode = (row["Country_Currency"].ToString());
                    }
                    _lst.Add(_obj);
                }
            }

            return _lst;
        }

        public List<Model.Country> Select_base_Country_Master(Model.Country obj)
        {
            List<Model.Country> _lst = new List<Model.Country>();

            MySqlCommand _cmd = new MySqlCommand("sp_select_base_country");
            if(obj.From_Send_Money == 1)//sanket 010525
            {
                _cmd = new MySqlCommand("sp_select_base_country_send_money");
            }
            _cmd.CommandType = CommandType.StoredProcedure;
            _cmd.Parameters.AddWithValue("_ClientId", obj.Client_ID);
            _cmd.Parameters.AddWithValue("_basecountry", obj.base_country);

            DataTable dt = db_connection.ExecuteQueryDataTableProcedure(_cmd);

            Model.Country _obj = new Model.Country();

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    _obj = new Model.Country();
                    _obj.Currency = new Model.Currency();

                    if (row["Country_ID"] != DBNull.Value)
                    {
                        _obj.Id = Convert.ToInt32(row["Country_ID"].ToString());
                    }
                    if (row["Country_Name"] != DBNull.Value)
                    {
                        _obj.Name = (row["Country_Name"].ToString());
                    }
                    if (row["Country_Code"] != DBNull.Value)
                    {
                        _obj.Code = (row["Country_Code"].ToString());
                    }

                    if (row["Country_Currency"] != DBNull.Value)
                    {
                        _obj.CurrencyCode = (row["Country_Currency"].ToString());
                    }
                    if (row["chkpostcode"] != DBNull.Value)
                    {
                        _obj.chkpostcode_search = Convert.ToInt32(row["chkpostcode"].ToString());
                    }
                    if (row["chkmobile"] != DBNull.Value)
                    {
                        _obj.chkbasecountry_mb = Convert.ToString(row["chkmobile"].ToString());
                    }
                    if (row["base_currency_id"] != DBNull.Value)
                    {
                        _obj.base_currency_id = Convert.ToInt32(row["base_currency_id"].ToString());
                    }
                    if (row["Country_Flag"] != DBNull.Value)
                    {
                        _obj.Country_Flag = (row["Country_Flag"].ToString());
                    }
                    if (row["flag"] != DBNull.Value)
                    {
                        _obj.flag = (row["flag"].ToString());
                    }
                    if (dt.Columns.Contains("Currency_Sign") && row["Currency_Sign"] != DBNull.Value)//sanket 010525
                    {
                        _obj.Currency.Sign = (row["Currency_Sign"].ToString());
                    }
                    _lst.Add(_obj);
                }
            }

            return _lst;
        }

        public List<Model.Country> Bind_Currencywise_Annual_Salary(Model.Country obj)
        {
            List<Model.Country> _lst = new List<Model.Country>();
            string whereclause = "";
            if (obj.Currency.Currency_Code != "" && obj.Client_ID != 0)
            {
                whereclause += "and asr.Currency_Code='" + obj.Currency.Currency_Code + "' and asr.Client_ID=" + obj.Client_ID + "";
            }
            MySqlCommand _cmd = new MySqlCommand("Get_annualSalary_by_currencyCode");
            _cmd.CommandType = CommandType.StoredProcedure;
            _cmd.Parameters.AddWithValue("_Whereclause", whereclause);

            DataTable dt = db_connection.ExecuteQueryDataTableProcedure(_cmd);

            Model.Country _obj = new Model.Country();

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    _obj = new Model.Country();
                    _obj.Currency = new Model.Currency();

                    if (row["ID"] != DBNull.Value)
                    {
                        _obj.Id = Convert.ToInt32(row["ID"].ToString());
                    }
                    if (row["BaseCurrency_Code"] != DBNull.Value)
                    {
                        _obj.Currency.Currency_Code = (row["BaseCurrency_Code"].ToString());
                    }
                    if (row["base_currency_id"] != DBNull.Value)
                    {
                        _obj.Currency.Currency_ID = Convert.ToInt32(row["base_currency_id"]);
                    }
                    if (row["Salary_Range"] != DBNull.Value)
                    {
                        _obj.Salary_Range = (row["Salary_Range"]).ToString();
                    }


                    _lst.Add(_obj);
                }
            }

            return _lst;
        }

        public DataTable select_provience(Model.Country obj)
        {

            List<Model.PaymentType> _lst = new List<Model.PaymentType>();
            MySqlCommand _cmd = new MySqlCommand("select_provience");
            _cmd.CommandType = CommandType.StoredProcedure;
            _cmd.Parameters.AddWithValue("_country_id", obj.base_country);
            _cmd.Parameters.AddWithValue("_client_id", obj.Client_ID);
            DataTable dt = db_connection.ExecuteQueryDataTableProcedure(_cmd);
            return dt;
        }
        public DataTable select_LGA_details(Model.Country obj)
        {

            List<Model.PaymentType> _lst = new List<Model.PaymentType>();
            MySqlCommand _cmd = new MySqlCommand("select_LGA_details");
            _cmd.CommandType = CommandType.StoredProcedure;
            _cmd.Parameters.AddWithValue("_Provience_Id", obj.provience_id);
            _cmd.Parameters.AddWithValue("_client_id", obj.Client_ID);
            DataTable dt = db_connection.ExecuteQueryDataTableProcedure(_cmd);
            return dt;
        }
    }
}
