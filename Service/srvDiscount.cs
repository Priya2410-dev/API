using System.Data;
using Calyx_Solutions.Model;
using Calyx_Solutions.Service;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System.Globalization;
using System.Text.Json;

using static Google.Apis.Requests.BatchRequest;

namespace Calyx_Solutions.Service
{
    public class srvDiscount
    {
        private string context;

        public DataTable getavailableDiscDetailsreferee(Model.Transaction obj , HttpContext context)
        {            
            int Customer_ID = Convert.ToInt32(CompanyInfo.Decrypt(obj.Customer_ID, true));
            List<Model.PaymentType> _lst = new List<Model.PaymentType>();
           MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("getavailableDiscDetailsreferee");
            _cmd.CommandType = CommandType.StoredProcedure;
            _cmd.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
            _cmd.Parameters.AddWithValue("_Customer_ID", Customer_ID);
            _cmd.Parameters.AddWithValue("_to_date", CompanyInfo.gettime(obj.Client_ID, context));
            DataTable dt = db_connection.ExecuteQueryDataTableProcedure(_cmd);
            try
            {
                foreach (DataRow row in dt.Select("Delete_Status = 1"))
                {
                    row.Delete();
                }
                dt.AcceptChanges();
            }
            catch(Exception egx) { }

            MySqlConnector.MySqlCommand _cmd1 = new MySqlConnector.MySqlCommand("getavailableDiscDetailsreferrer");
            _cmd1.CommandType = CommandType.StoredProcedure;
            _cmd1.Parameters.AddWithValue("_Client_ID", obj.Client_ID); 
            _cmd1.Parameters.AddWithValue("_Customer_ID", Customer_ID);
            _cmd1.Parameters.AddWithValue("_to_date", CompanyInfo.gettime(obj.Client_ID ,  context));
            DataTable dt1 = db_connection.ExecuteQueryDataTableProcedure(_cmd1);

            try
            {
                foreach (DataRow row in dt1.Select("Delete_Status = 1"))
            {
                row.Delete();
            }
            dt1.AcceptChanges();
            }
            catch (Exception egx) { }

            DataTable dt2 = null;
            if (Convert.ToString(obj.SourceComment) == "true" && obj.SourceComment != "" && obj.SourceComment != null)
            {
                _cmd1 = new MySqlConnector.MySqlCommand("getavailableDiscVoucherDetails");
                _cmd1.CommandType = CommandType.StoredProcedure;
                _cmd1.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                _cmd1.Parameters.AddWithValue("_Customer_ID", Customer_ID);
                _cmd1.Parameters.AddWithValue("_to_date", CompanyInfo.gettime(obj.Client_ID,  context));
                dt2 = db_connection.ExecuteQueryDataTableProcedure(_cmd1);

                foreach (DataRow row in dt2.Select("Delete_Status = 1"))
                {
                    row.Delete();
                }
                dt2.AcceptChanges();
            }

            dt.AcceptChanges();
            dt.Merge(dt1);
            if (dt2 != null)
            {
                dt.Merge(dt2, true, MissingSchemaAction.Ignore);
            }
            return dt;
        }


        public DataTable getusedDiscDetailsreferee(Model.Transaction obj , HttpContext context)
        {
            int Customer_ID = Convert.ToInt32(CompanyInfo.Decrypt(obj.Customer_ID, true));
            List<Model.PaymentType> _lst = new List<Model.PaymentType>();
           MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("getusedDiscDetailsreferee");
            _cmd.CommandType = CommandType.StoredProcedure;
            _cmd.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
            _cmd.Parameters.AddWithValue("_Customer_ID", Customer_ID);
            _cmd.Parameters.AddWithValue("_to_date", CompanyInfo.gettime(obj.Client_ID ,context ));
            DataTable dt = db_connection.ExecuteQueryDataTableProcedure(_cmd);

            MySqlConnector.MySqlCommand _cmd1 = new MySqlConnector.MySqlCommand("getusedDiscDetailsreferrer");
            _cmd1.CommandType = CommandType.StoredProcedure;
            _cmd1.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
            _cmd1.Parameters.AddWithValue("_Customer_ID", Customer_ID);
            _cmd1.Parameters.AddWithValue("_to_date", CompanyInfo.gettime(obj.Client_ID , context));
            DataTable dt1 = db_connection.ExecuteQueryDataTableProcedure(_cmd1);

            dt.AcceptChanges();
            dt.Merge(dt1);
            return dt;
        }


        public DataTable getexpDiscDetailsreferee(Model.Transaction obj , HttpContext context)
        {
            int Customer_ID = Convert.ToInt32(CompanyInfo.Decrypt(obj.Customer_ID, true));
            List<Model.PaymentType> _lst = new List<Model.PaymentType>();
           MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("getexpDiscDetailsreferee");
            _cmd.CommandType = CommandType.StoredProcedure;
            _cmd.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
            _cmd.Parameters.AddWithValue("_Customer_ID", Customer_ID);
            _cmd.Parameters.AddWithValue("_to_date", CompanyInfo.gettime(obj.Client_ID, context));
            DataTable dt = db_connection.ExecuteQueryDataTableProcedure(_cmd);

            MySqlConnector.MySqlCommand _cmd1 = new MySqlConnector.MySqlCommand("getexpDiscDetailsreferrer");
            _cmd1.CommandType = CommandType.StoredProcedure;
            _cmd1.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
            _cmd1.Parameters.AddWithValue("_Customer_ID", Customer_ID);
            _cmd1.Parameters.AddWithValue("_to_date", CompanyInfo.gettime(obj.Client_ID, context));
            DataTable dt1 = db_connection.ExecuteQueryDataTableProcedure(_cmd1);

            dt.AcceptChanges();
            dt.Merge(dt1);
            return dt;
        }

        public List<Model.Transaction> GetInviteDetails(Model.Transaction obj)
        {
            HttpContext context = null;
            mts_connection _MTS = new mts_connection();
            List<Model.Transaction> _lst = new List<Model.Transaction>();
            string scheme_type_id_regex = validation.validate(Convert.ToString(obj.scheme_type_id), 0, 1, 1, 1, 1, 1, 1, 1, 1);
            string referrer_maxnoofreferee_regex = validation.validate(Convert.ToString(obj.referrer_maxnoofreferee), 0, 1, 1, 1, 1, 1, 1, 1, 1);
            string multiples_of_regex = validation.validate(Convert.ToString(obj.multiples_of), 0, 1, 1, 1, 1, 1, 1, 1, 1);
            string referrer_perk_type_regex = validation.validate(Convert.ToString(obj.referrer_perk_type), 0, 1, 1, 1, 1, 1, 1, 1, 1);
            string referee_perk_type_regex = validation.validate(Convert.ToString(obj.referee_perk_type), 0, 1, 1, 1, 1, 1, 1, 1, 1);
            string referrer_discount_type_regex = validation.validate(Convert.ToString(obj.referrer_discount_type), 0, 1, 1, 1, 1, 1, 1, 1, 1);
            string referee_discount_type_regex = validation.validate(Convert.ToString(obj.referee_discount_type), 0, 1, 1, 1, 1, 1, 1, 1, 1);
            //string referrer_value_regex = validation.validate(Convert.ToString(obj.referrer_value), 0, 1, 1, 1, 1, 1, 1, 1, 1);
            //string referee_value = validation.validate(Convert.ToString(obj.referee_value), 0, 1, 1, 1, 1, 1, 1, 1, 1);
            //string referee_mintransferamount= validation.validate(Convert.ToString(obj.referrer_value), 0, 1, 1, 1, 1, 1, 1, 1, 1);
            string description = validation.validate(Convert.ToString(obj.referrer_value), 1, 1, 1, 1, 1, 1, 1, 1, 1);
            if (scheme_type_id_regex != "false" && referrer_maxnoofreferee_regex != "false" && multiples_of_regex != "false" && referrer_perk_type_regex != "false" && referee_perk_type_regex != "false" && referrer_discount_type_regex != "false" && referee_discount_type_regex != "false")
            {
                MySqlConnector.MySqlConnection con = new MySqlConnector.MySqlConnection(_MTS.WebConnSetting());
                con.Open();
                MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("SP_GetSchemeDetails");
                _cmd.Connection = con;
                _cmd.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                //   _cmd.Parameters.Add(new MySqlParameter("_perm", MySqlDbType.Int32));
                //       _cmd.Parameters["_perm"].Direction = ParameterDirection.Output;
                _cmd.CommandType = CommandType.StoredProcedure;
                Model.Transaction _obj = new Model.Transaction();
                DataTable dt = db_connection.ExecuteQueryDataTableProcedure(_cmd);
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        _obj = new Model.Transaction();

                        int SalesRep_Flag = Convert.ToInt32(row["SalesRep_Flag"].ToString());
                        if (SalesRep_Flag == 1 || SalesRep_Flag == 3) //for customer only
                        {
                            if (row["scheme_type_id"] != DBNull.Value)
                            {
                                _obj.scheme_type_id = Convert.ToInt32(row["scheme_type_id"].ToString());
                            }
                            if (row["referrer_maxnoofreferee"] != DBNull.Value)
                            {
                                _obj.referrer_maxnoofreferee = Convert.ToInt32(row["referrer_maxnoofreferee"].ToString());
                            }
                            if (row["multiples_of"] != DBNull.Value)
                            {
                                _obj.multiples_of = Convert.ToInt32(row["multiples_of"].ToString());
                            }
                            if (row["referrer_perk_type"] != DBNull.Value)
                            {
                                _obj.referrer_perk_type = Convert.ToInt32(row["referrer_perk_type"].ToString());
                            }
                            if (row["referee_perk_type"] != DBNull.Value)
                            {
                                _obj.referee_perk_type = Convert.ToInt32(row["referee_perk_type"].ToString());
                            }
                            if (row["referrer_discount_type"] != DBNull.Value)
                            {
                                _obj.referrer_discount_type = Convert.ToInt32(row["referrer_discount_type"].ToString());
                            }
                            if (row["referee_discount_type"] != DBNull.Value)
                            {
                                _obj.referee_discount_type = Convert.ToInt32(row["referee_discount_type"].ToString());
                            }
                            if (row["referrer_value"] != DBNull.Value)
                            {
                                _obj.referrer_value = Convert.ToDecimal(row["referrer_value"].ToString());
                            }
                            if (row["referee_value"] != DBNull.Value)
                            {
                                _obj.referee_value = Convert.ToDecimal(row["referee_value"].ToString());
                            }
                            if (row["referee_mintransferamount"] != DBNull.Value)
                            {
                                _obj.referee_mintransferamount = Convert.ToDecimal(row["referee_mintransferamount"].ToString());
                            }
                            if (row["description"] != DBNull.Value)
                            {
                                _obj.description = Convert.ToString(row["description"].ToString());
                            }
                            if (row["referee_applicablefor"] != DBNull.Value)//Digvijay changes for reward info show transfer type wise
                            {
                                _obj.referee_applicablefor = Convert.ToInt32(row["referee_applicablefor"].ToString());
                            }
                            _lst.Add(_obj);
                        }
                    }
                }


                con.Close();
                // DataTable dt = db_connection.ExecuteQueryDataTableProcedure(_cmd);

                //Model.Customer _obj = new Model.Customer();
                //if (dt != null && dt.Rows.Count > 0)
                //{
                //    foreach (DataRow row in dt.Rows)
                //    {
                //        _obj = new Model.Customer();

                //        if (row["_perm"] != DBNull.Value)
                //        {
                //            _obj.Id = Convert.ToInt32(row["_perm"].ToString());
                //        }
                //        _lst.Add(_obj);
                //    }
                //}
            }
            else
            {
                string msg = "Validation Error scheme_type_id_regex- " + scheme_type_id_regex + "referrer_maxnoofreferee_regex-" + multiples_of_regex + "multiples_of_regex- " + multiples_of_regex + "referrer_perk_type_regex- " + referee_perk_type_regex + "referrer_discount_type_regex- " + referrer_discount_type_regex +
                    "referee_discount_type_regex-" + referee_discount_type_regex;
                int stattus = (int)CompanyInfo.InsertActivityLogDetailsSecurity(msg, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), 1, "srvDiscount", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "GetInviteDetails", 0, context);
            }
            return _lst;
        }

        public List<Model.Customer> Check_ReferalScheme(Model.Customer obj)
        {
            HttpContext context = null;
            mts_connection _MTS = new mts_connection();
            List<Model.Customer> _lst = new List<Model.Customer>();
            string Client_ID_regex = validation.validate(Convert.ToString(obj.Client_ID), 0, 1, 1, 1, 1, 1, 1, 1, 1); ;
            string Message_regex = validation.validate(Convert.ToString(obj.Message), 1, 1, 1, 1, 1, 1, 1, 1, 1);
            if (Client_ID_regex != "false" && Message_regex != "false")
            {
                MySqlConnector.MySqlConnection con = new MySqlConnector.MySqlConnection(_MTS.WebConnSetting());
                con.Open();
                MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("SP_Check_ReferralScheme");
                _cmd.Connection = con;
                _cmd.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                _cmd.Parameters.AddWithValue("_PID", 4);
                //   _cmd.Parameters.Add(new MySqlParameter("_perm", MySqlDbType.Int32));
                //       _cmd.Parameters["_perm"].Direction = ParameterDirection.Output;
                _cmd.CommandType = CommandType.StoredProcedure;
                //_cmd.ExecuteScalar();
                //int res = Convert.ToInt32(_cmd.Parameters["_perm"].Value);
                //if (res == 1)//expired
                //{
                //    //return "exist_mobile";
                //    obj.Message = "expired";
                //}
                //else//not expired
                //{
                //    obj.Message = "active";
                //}
                DataTable dt = db_connection.ExecuteQueryDataTableProcedure(_cmd);
                if (dt.Rows.Count > 0)
                {
                    if (Convert.ToString(dt.Rows[0]["Perm"]) == "0")//permission on
                    {
                        if (Convert.ToString(dt.Rows[0]["expired"]) == "0")//active
                        {
                            obj.Message = "active";
                        }
                        if (Convert.ToString(dt.Rows[0]["expired"]) == "1")//expired
                        {
                            obj.Message = "expired";
                        }
                    }
                    else
                    {
                        obj.Message = "expired";
                    }
                }
                else
                {
                    obj.Message = "expired";
                }
                _lst.Add(obj);
                con.Close();
                // DataTable dt = db_connection.ExecuteQueryDataTableProcedure(_cmd);

                //Model.Customer _obj = new Model.Customer();
                //if (dt != null && dt.Rows.Count > 0)
                //{
                //    foreach (DataRow row in dt.Rows)
                //    {
                //        _obj = new Model.Customer();

                //        if (row["_perm"] != DBNull.Value)
                //        {
                //            _obj.Id = Convert.ToInt32(row["_perm"].ToString());
                //        }
                //        _lst.Add(_obj);
                //    }
                //}
            }
            else
            {

                string msg = "Validation Error Client_ID_regex- " + Client_ID_regex + "Message_regex- " + Message_regex;
                int stattus = (int)CompanyInfo.InsertActivityLogDetailsSecurity(msg, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), 1, "srvDiscount", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "Check_ReferalScheme", 0, context);

            }
            return _lst;
        }


    }
}
