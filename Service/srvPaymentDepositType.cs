using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Data;


namespace Calyx_Solutions.Service
{
    public class srvPaymentDepositType
    {       
        public DataTable GetPayDepositTypes(Model.Transaction obj)
        {
            List<Model.PaymentDepositType> _lst = new List<Model.PaymentDepositType>();
            MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("GetPayDepositTypes");
            _cmd.CommandType = CommandType.StoredProcedure;
            string where = "";
            if (obj.PaymentDepositType_ID > 0)
            {
                where = " and PaymentDepositType_ID=" + obj.PaymentDepositType_ID;
            }
            _cmd.Parameters.AddWithValue("_where", " and ShowOnCustSide=0 and Country_ID=" + obj.Country_ID + "" + where);
            _cmd.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
            _cmd.Parameters.AddWithValue("_TransferTypeFlag", 0);
            DataTable dt = db_connection.ExecuteQueryDataTableProcedure(_cmd);
            return dt;
        }
        public DataTable GetBenf_CollType_Mapping_Details(Model.Transaction obj)
        {
            List<Model.PaymentDepositType> _lst = new List<Model.PaymentDepositType>();

            string whereclause = "";
            DataTable dt = new DataTable();
            if (obj.Beneficiary_ID > 0)
            {
                whereclause = whereclause + " and bmt.Beneficiary_ID=" + obj.Beneficiary_ID;

                if (obj.PaymentDepositType_ID > 0)
                {
                    whereclause = whereclause + " and bmt.PaymentDepositType_ID=" + obj.PaymentDepositType_ID;
                }
                MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("Get_benef_colltype_mapping");
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.Parameters.AddWithValue("_whereclause", whereclause);

                dt = db_connection.ExecuteQueryDataTableProcedure(_cmd);
            }
            return dt;
        }
    }
}
