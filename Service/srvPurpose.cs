using System.Data;

namespace Calyx_Solutions.Service
{
    public class srvPurpose
    {
        public DataTable GetPurposes(Model.Transaction obj)
        {
            List<Model.PaymentType> _lst = new List<Model.PaymentType>();
           MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("Purpose_List");
            _cmd.CommandType = CommandType.StoredProcedure;
            _cmd.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
            DataTable dt = db_connection.ExecuteQueryDataTableProcedure(_cmd);
            return dt;
        }
    }
}
