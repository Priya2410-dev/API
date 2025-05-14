using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Calyx_Solutions.Model;
using MySqlConnector;
namespace Calyx_Solutions.Service
{
    public class srvActivityLogTracker
    {

        public int Create(ActivityLogTracker obj, HttpContext context)
        {
            try
            {
                obj.RecordInsertDate = Convert.ToDateTime(CompanyInfo.gettime(obj.Client_ID, context));

                MySqlConnector.MySqlCommand cmd = new MySqlConnector.MySqlCommand("sp_save_activity_log_tracker");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("_activity", obj.Activity.Trim());
                cmd.Parameters.AddWithValue("_whoAccessed", obj.WhoAcessed);
                cmd.Parameters.AddWithValue("_recordInsertDate", obj.RecordInsertDate);
                cmd.Parameters.AddWithValue("_deleteStatus", obj.DeleteStatus);
                cmd.Parameters.AddWithValue("_transactionId", obj.Transaction_ID);

                cmd.Parameters.AddWithValue("_userId", obj.User_ID);

                cmd.Parameters.AddWithValue("_custId", obj.Customer_ID);
                cmd.Parameters.AddWithValue("_functionName", obj.FunctionName);

                cmd.Parameters.AddWithValue("_branchId", obj.Branch_ID);
                cmd.Parameters.AddWithValue("_clientId", obj.Client_ID);

                object result = db_connection.ExecuteScalarProcedure(cmd);

                result = result == DBNull.Value ? null : result;
                int _id = Convert.ToInt32(result);
                obj.Id = _id;
                cmd.Dispose();
            }
            catch { }
            return obj.Id;
        }

        public int Create_activity_security(ActivityLogTracker obj, HttpContext context)
        {
            try
            {
                obj.RecordInsertDate = Convert.ToDateTime(CompanyInfo.gettime(obj.Client_ID, context));

                MySqlConnector.MySqlCommand cmd = new MySqlConnector.MySqlCommand("sp_save_activity_log_tracker_security");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("_activity", obj.Activity.Trim());
                cmd.Parameters.AddWithValue("_whoAccessed", obj.WhoAcessed);
                cmd.Parameters.AddWithValue("_recordInsertDate", obj.RecordInsertDate);
                cmd.Parameters.AddWithValue("_deleteStatus", obj.DeleteStatus);
                cmd.Parameters.AddWithValue("_transactionId", obj.Transaction_ID);

                cmd.Parameters.AddWithValue("_userId", obj.User_ID);

                cmd.Parameters.AddWithValue("_custId", obj.Customer_ID);
                cmd.Parameters.AddWithValue("_functionName", obj.FunctionName);

                cmd.Parameters.AddWithValue("_branchId", obj.Branch_ID);
                cmd.Parameters.AddWithValue("_clientId", obj.Client_ID);
                cmd.Parameters.AddWithValue("_securityflag", obj.Security_flag);
                object result = db_connection.ExecuteScalarProcedure(cmd);

                result = result == DBNull.Value ? null : result;
                int _id = Convert.ToInt32(result);
                obj.Id = _id;
                cmd.Dispose();
            }
            catch(Exception ehx) {
                CompanyInfo.InsertrequestLogTracker("QueryErrorSave : " + ehx.ToString(), 0, 0, 0, 0, "QueryErrorSave", 0, 0, "", context);
            }
            return obj.Id;
        }

     

    }
}
