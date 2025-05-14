using System.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Data;
namespace Calyx_Solutions.Service
{
    public class srvErrorLog
    {
        public string Create(Model.ErrorLog obj, HttpContext context)
        {
            string msg = string.Empty;
           /* mts_connection _MTS = new mts_connection();
           MySqlConnection con = new MySqlConnection(_MTS.WebConnSetting());*/

            db_connection aController = new db_connection();
            string conneObj = aController.WebConnSetting();

            MySqlConnector.MySqlConnection con = new MySqlConnector.MySqlConnection(  conneObj );
            MySqlConnector.MySqlCommand cmd = new MySqlConnector.MySqlCommand("sp_save_error_log");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = con;
            con.Open();
            cmd.Parameters.AddWithValue("_Error", obj.Error);
            cmd.Parameters.AddWithValue("_Record_insert_Date_time", CompanyInfo.gettime(obj.Client_ID, context));
            cmd.Parameters.AddWithValue("_User_ID", obj.User_ID);
            cmd.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
            cmd.Parameters.AddWithValue("_Branch_ID", obj.Branch_ID);
            cmd.Parameters.AddWithValue("_Delete_Status", obj.DeleteStatus);
            cmd.Parameters.AddWithValue("_Function_Name", obj.Function_Name);
            int n = cmd.ExecuteNonQuery();
            if (n > 0)//success
            {
                msg = "success";
            }
            else//not success
            {
                msg = "notsuccess";
            }
            cmd.Dispose();
            return msg;
        }
    }
}
