using System.Data;

namespace Calyx_Solutions.Service
{
    public class srvPurpose
    {
        public DataTable GetPurposes(Model.Transaction obj) //Digvijay changes for relation wise purpose show on send money details 24-12-24
        {
            List<Model.PaymentType> _lst = new List<Model.PaymentType>();
            MySqlCommand cmd = new MySqlCommand("GetPermissions");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
            cmd.Parameters.AddWithValue("_whereclause", " and PID=186 ");
            DataTable dtperm_status = db_connection.ExecuteQueryDataTableProcedure(cmd);
            int perm_purpose = 1;
            if (dtperm_status.Rows.Count > 0)
            {
                perm_purpose = Convert.ToInt32(dtperm_status.Rows[0]["Status_ForCustomer"]);
            }
            if (perm_purpose == 0)
            {
                MySqlCommand _cmd = new MySqlCommand("Get_Purpose_List");
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                _cmd.Parameters.AddWithValue("_Relation_ID", obj.Relation_ID);
                DataTable dt = db_connection.ExecuteQueryDataTableProcedure(_cmd);
                return dt;
            }
            else
            {
                MySqlCommand _cmd = new MySqlCommand("Get_Purpose_List");
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                _cmd.Parameters.AddWithValue("_Relation_ID", 0);
                DataTable dt = db_connection.ExecuteQueryDataTableProcedure(_cmd);
                return dt;
            }

        }
    }
}
