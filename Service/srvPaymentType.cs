using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Data;

namespace Calyx_Solutions.Service
{
    public class srvPaymentType
    {

        public DataTable GetPaymentTypes(Model.Transaction obj)
        {
            MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("GetPaymentTypes");
            _cmd.CommandType = CommandType.StoredProcedure;
            _cmd.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
            int SourceID = 1;
            string Whereclause = "";
            if (obj.Is_App == 0) { SourceID = 3; }
            else if (obj.Branch_ID == 2) { SourceID = 2; }
            _cmd.Parameters.AddWithValue("_Source_ID", SourceID);
            if (obj.Basecurr_Country_ID != 0)
            { Whereclause = Whereclause + " And Base_Country_Id =" + obj.Basecurr_Country_ID; }
            _cmd.Parameters.AddWithValue("_whereclause", Whereclause);
            DataTable dt = db_connection.ExecuteQueryDataTableProcedure(_cmd);
            return dt;
        }
        public DataTable GetPayTypes(Model.Transaction obj)//vishvesh 250523
        {
            MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("GetPaymentType");
            _cmd.CommandType = CommandType.StoredProcedure;
            _cmd.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
            // _cmd.Parameters.AddWithValue("_whereclause", "");
            DataTable dt = db_connection.ExecuteQueryDataTableProcedure(_cmd);
            return dt;
        }
        public DataTable GetPaybyCardTypes(Model.Transaction obj)
        {
           MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("Get_PaymentByCardsTypes");
            _cmd.CommandType = CommandType.StoredProcedure;
            _cmd.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
            _cmd.Parameters.AddWithValue("_whereclause", "");
            DataTable dt = db_connection.ExecuteQueryDataTableProcedure(_cmd);
            return dt;
        }


        public DataTable GetPayGateway(Model.Transaction obj)
        {
           MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("Get_PaymentGateway");
            _cmd.CommandType = CommandType.StoredProcedure;
            _cmd.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
            _cmd.Parameters.AddWithValue("_whereclause", "");
            DataTable dt = db_connection.ExecuteQueryDataTableProcedure(_cmd);
            return dt;
        }


        public DataTable GetinstantBankAPIDetails(Model.Transaction obj)
        {
            string Query = "SELECT * FROM instantbank_transfer_api_details WHERE Delete_Status=0 AND Client_ID=" + obj.Client_ID + " AND API_Status=0";
           MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("Default_SP");
            _cmd.CommandType = CommandType.StoredProcedure;
            _cmd.Parameters.AddWithValue("_Query", Query);
            DataTable active_instantbnkapi = db_connection.ExecuteQueryDataTableProcedure(_cmd);
            return active_instantbnkapi;
        }
    }
}
