using System.Data;
using Calyx_Solutions.Model;

namespace Calyx_Solutions.Service
{
    public class srvDeliveryType
    {
        public DataTable getDeliveryTypes(Transaction obj)
        {
            List<DeliveryType> _lst = new List<DeliveryType>();

            MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("GetDeliveryTypes");
            _cmd.CommandType = CommandType.StoredProcedure;
            _cmd.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
            _cmd.Parameters.AddWithValue("_Country_ID", obj.Country_ID);

            DataTable dt = db_connection.ExecuteQueryDataTableProcedure(_cmd);
            return dt;
        }
        /*public DataTable GetDeliveryMapping(Transaction obj)
        {
            List<DeliveryType> _lst = new List<DeliveryType>();

            MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("GetDeliveryMapping");
            _cmd.CommandType = CommandType.StoredProcedure;
            _cmd.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
            _cmd.Parameters.AddWithValue("_Country_ID", obj.Country_ID);

            DataTable dt = db_connection.ExecuteQueryDataTableProcedure(_cmd);
            return dt;
        }*/

        public DataTable GetDeliveryMapping(Transaction obj)
        {
            List<DeliveryType> _lst = new List<DeliveryType>();

            MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("GetDeliveryMapping");
            _cmd.CommandType = CommandType.StoredProcedure;
            _cmd.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
            _cmd.Parameters.AddWithValue("_Country_ID", obj.Country_ID);
            DataTable filteredDt = new DataTable();
            DataTable dt = db_connection.ExecuteQueryDataTableProcedure(_cmd);
            int bankID = 0;
            try
            {
                bankID = obj.Benf_BankDetails_ID;  //New
            }
            catch (Exception ex) { bankID = 0; }

            if (bankID != 0)
            {
                DataRow[] dr1 = dt.Select($"Sendmoney_Banks LIKE '%,{bankID},%' OR Sendmoney_Banks LIKE '{bankID},%' OR Sendmoney_Banks LIKE '%,{bankID}' OR Sendmoney_Banks = '{bankID}'");
                filteredDt = dt.Clone();
                foreach (DataRow row in dr1)
                {
                    filteredDt.ImportRow(row);
                }
                return filteredDt; //End new
            }
            //DataRow[] dr1 = dt.Select("Sendmoney_Banks= ",'"+"'159");
            return dt;
        }

        public DataTable GetDeliveryMappingSettings(Transaction obj)
        {
            List<DeliveryType> _lst = new List<DeliveryType>();

            MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("GetDeliveryMappingSettings");
            _cmd.CommandType = CommandType.StoredProcedure;
            _cmd.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
            _cmd.Parameters.AddWithValue("_Country_ID", obj.Country_ID);
            _cmd.Parameters.AddWithValue("_DeliveryType_Id", obj.DeliveryType_Id);
            _cmd.Parameters.AddWithValue("_CollectionType_Id", obj.CollectionPoint_ID);

            DataTable dt = db_connection.ExecuteQueryDataTableProcedure(_cmd);
            return dt;
        }
    }
}
 
