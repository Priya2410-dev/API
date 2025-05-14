using MySqlConnector;
using System.Data;

namespace Calyx_Solutions.Service
{
    public class srvServices //for Kmoney
    {
        private readonly HttpContext _srvServicesHttpContext;
        private readonly IDbConnection _dbConnection;
        public srvServices(HttpContext context)
        {
            this._srvServicesHttpContext = context;
        }

        public DataTable Select_football_league(Model.Customer obj, HttpContext context)
        {
            //List<Model.Services> _lst = new List<Model.Services>();

            MySqlCommand _cmd = new MySqlCommand("select_football_league");
            _cmd.CommandType = CommandType.StoredProcedure;
            _cmd.Parameters.AddWithValue("_ClientId", obj.Client_ID);
            DataTable dt = db_connection.ExecuteQueryDataTableProcedure(_cmd);

            //Model.Services _obj = new Model.Services();

            if (dt != null && dt.Rows.Count > 0)
            {
                //foreach (DataRow row in dt.Rows)
                //{
                //    _obj = new Model.Services();


                //    if (row["S_ID"] != DBNull.Value)
                //    {
                //        _obj.Id = Convert.ToInt32(row["S_ID"].ToString());
                //    }
                //    if (row["Services_Name"] != DBNull.Value)
                //    {
                //        _obj.Name = (row["Services_Name"].ToString());
                //    }
                //    if (row["Services_Flag"] != DBNull.Value)
                //    {
                //        _obj.Services_flag = (row["Services_Flag"].ToString());
                //    }

                //    _lst.Add(_obj);
                //}

            }

            //return _lst;
            return dt;
        }

        public DataTable Select_FtCountry(Model.Services obj, HttpContext context)
        {
            //List<Model.Services> _lst = new List<Model.Services>();

            MySqlCommand _cmd = new MySqlCommand("Football_services");
            _cmd.CommandType = CommandType.StoredProcedure;
            _cmd.Parameters.AddWithValue("_FL_ID", obj.League_Id);
            DataTable dt = db_connection.ExecuteQueryDataTableProcedure(_cmd);

            //Model.Services _obj = new Model.Services();

            if (dt != null && dt.Rows.Count > 0)
            {
                //foreach (DataRow row in dt.Rows)
                //{
                //    _obj = new Model.Services();


                //    if (row["S_ID"] != DBNull.Value)
                //    {
                //        _obj.Id = Convert.ToInt32(row["S_ID"].ToString());
                //    }
                //    if (row["Services_Name"] != DBNull.Value)
                //    {
                //        _obj.Name = (row["Services_Name"].ToString());
                //    }
                //    if (row["Services_Flag"] != DBNull.Value)
                //    {
                //        _obj.Services_flag = (row["Services_Flag"].ToString());
                //    }

                //    _lst.Add(_obj);
                //}

            }

            //return _lst;
            return dt;
        }

        public Model.Services Save_services(Model.Services obj, HttpContext context)
        {
            string Activity = string.Empty;
            //string Username = Convert.ToString(context.Request.Form["Username"]);
            mts_connection _MTS = new mts_connection();
            int Customer_ID = Convert.ToInt32(CompanyInfo.Decrypt(obj.Customer_ID, true));
            using (MySqlConnection con = new MySqlConnection(_MTS.WebConnSetting()))
            {
                con.Open();

                MySqlCommand cmd = new MySqlCommand("update_services_flag");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = con;
                cmd.Parameters.AddWithValue("_Customer_Id", Customer_ID);
                cmd.Parameters.AddWithValue("_getval", obj.value);
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
                //object result = db_connection.ExecuteScalarProcedure(cmd);

                //result = (result == DBNull.Value) ? null : result;
                //obj.Beneficiary_ID = Convert.ToInt32(result);
                cmd.Dispose();

                Activity = "Customer services flag updated.  </br>";
                int stattus = (int)CompanyInfo.InsertActivityLogDetails(Activity, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(0), Convert.ToInt32(Customer_ID), "Update", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "Update Beneficiary Mobile", context);
            }
            return obj;
        }
        public DataTable Selected_Football_Team(Model.Services obj, HttpContext context)
        {
            //List<Model.Services> _lst = new List<Model.Services>();
            int Customer_ID = Convert.ToInt32(CompanyInfo.Decrypt(Convert.ToString(obj.Customer_ID), true));
            MySqlCommand _cmd = new MySqlCommand("Selected_Football_teams");
            _cmd.CommandType = CommandType.StoredProcedure;
            _cmd.Parameters.AddWithValue("_Customer_ID", Customer_ID);

            DataTable dt = db_connection.ExecuteQueryDataTableProcedure(_cmd);

            //Model.Services _obj = new Model.Services();

            if (dt != null && dt.Rows.Count > 0)
            {
                //foreach (DataRow row in dt.Rows)
                //{
                //    _obj = new Model.Services();


                //    if (row["S_ID"] != DBNull.Value)
                //    {
                //        _obj.Id = Convert.ToInt32(row["S_ID"].ToString());
                //    }
                //    if (row["Services_Name"] != DBNull.Value)
                //    {
                //        _obj.Name = (row["Services_Name"].ToString());
                //    }
                //    if (row["Services_Flag"] != DBNull.Value)
                //    {
                //        _obj.Services_flag = (row["Services_Flag"].ToString());
                //    }

                //    _lst.Add(_obj);
                //}

            }

            //return _lst;
            return dt;
        }
    }
}
