using System.Data;
using MySqlConnector;

namespace Calyx_Solutions.Service
{
    public class srvPermission
    {
        public DataTable Get_Permissions(Model.Permission obj)
        {
            List<Model.Permission> _lst = new List<Model.Permission>();
            MySqlCommand _cmd = new MySqlCommand("GetPermissions");
            _cmd.CommandType = CommandType.StoredProcedure;
            if (obj.whereclause == "" || obj.whereclause == null)
            {
                _cmd.Parameters.AddWithValue("_whereclause", "");
            }
            else
            {
                _cmd.Parameters.AddWithValue("_whereclause", obj.whereclause);
            }
            _cmd.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
            DataTable dt = db_connection.ExecuteQueryDataTableProcedure(_cmd);
            return dt;
        }
    }
}
