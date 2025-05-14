using Calyx_Solutions.Service;
using System.Data;

namespace Calyx_Solutions.Model
{
    public class MasterDAC
    {
        public static object InsertToDatabase(IMaster iobjCustomer)
        {
            DBHelper objDBHelper = new DBHelper();
            try
            {
                string strResult = string.Empty;

                if (iobjCustomer.Is_Procedure == "PROCEDURE")
                {
                    string strProcedure;
                    object[] arrparams = setProcParams(iobjCustomer, out strProcedure);
                    int iResult = objDBHelper.ExecuteNonQuery(objDBHelper.Get_SqlConnstring(), strProcedure, arrparams);
                    
                }
                else if (iobjCustomer.Is_Procedure.ToString().ToUpper() == "QUERY")
                {
                    string Query = GetQuery_With_Param(iobjCustomer);
                    int iResult = objDBHelper.ExecuteNonQuery(objDBHelper.Get_SqlConnstring(), CommandType.Text, Query);
                    if (iResult > 0)
                    {
                        strResult = "Success";
                    }
                    else
                    {
                        strResult = "NotSuccess";
                    }

                }
                return strResult;

                //  return "OK";
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static object UpdateToDatabase(IMaster iobjCustomer)
        {
            DBHelper objDBHelper = new DBHelper();
            try
            {
                string strResult = string.Empty;

                if (iobjCustomer.Is_Procedure == "PROCEDURE")
                {
                    string strProcedure;
                    object[] arrParams = setProcParams(iobjCustomer, out strProcedure);
                    int iResult = objDBHelper.ExecuteNonQuery(objDBHelper.Get_SqlConnstring(), strProcedure, arrParams);

                    //if (iResult > 0)
                    //{
                    //    strResult = "Success";
                    //}
                    //else
                    //{
                    //    strResult = "NotSuccess";
                    //}
                }
                else if (iobjCustomer.Is_Procedure.ToString().ToUpper() == "QUERY")/*Added for update data*/
                {
                    string Query = GetQuery_With_Param(iobjCustomer);
                    int iResult = objDBHelper.ExecuteNonQuery(objDBHelper.Get_SqlConnstring(), CommandType.Text, Query);
                    if (iResult > 0)
                    {
                        strResult = "Success";
                    }
                    else
                    {
                        strResult = "NotSuccess";
                    }

                }

                return strResult;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static object Check_Exists(IMaster iobjCustomer)
        {
            string flag = "0";
            DataTable dtData = new DataTable();
            DBHelper objDBHelper = new DBHelper();
            /*if (iobjCustomer.Is_Procedure.ToString().ToUpper() == "QUERY")
            {
                string Query = GetQuery_With_Param(iobjCustomer);
                MySqlDataReader rdr = objDBHelper.ExecuteReader(objDBHelper.Get_SqlConnstring(), CommandType.Text, Query);
                if (rdr.Read())
                {
                    flag = "1";
                }
                else
                {
                    flag = "0";
                }

            }*/
            return flag;
        }

        public static object ReadFromDatabase(IMaster iobjCustomer)
        {
            DataTable dtData = new DataTable();
            DBHelper objDBHelper = new DBHelper();
            try
            {
                string strResult = string.Empty;

                if (iobjCustomer.Is_Procedure == "PROCEDURE")
                {
                    string strProcedure;
                    object[] arrparams = setProcParams(iobjCustomer, out strProcedure);
                    dtData = objDBHelper.ExecuteDataset(objDBHelper.Get_SqlConnstring(), strProcedure, arrparams).Tables[0];
                }
                else if (iobjCustomer.Is_Procedure.ToString().ToUpper() == "QUERY")
                {
                    string Query = GetQuery_With_Param(iobjCustomer);
                    dtData = objDBHelper.ExecuteDataset(objDBHelper.Get_SqlConnstring(), CommandType.Text, Query).Tables[0];
                }

                return dtData;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static object ReadSingleValueFromDatabase(IMaster iobjCustomer)
        {
            object objData = new object();
            DBHelper objDBHelper = new DBHelper();
            try
            {
                if (iobjCustomer.Is_Procedure.ToUpper() == "PROCEDURE")
                {
                    string strProcedureName;
                    object[] arrParams = setProcParams(iobjCustomer, out strProcedureName);
                    objData = objDBHelper.ExecuteScalar(objDBHelper.Get_SqlConnstring(), strProcedureName, arrParams);
                }
                else if (iobjCustomer.Is_Procedure.ToString().ToUpper() == "QUERY")
                {
                    string Query = GetQuery_With_Param(iobjCustomer);
                    objData = objDBHelper.ExecuteScalar(objDBHelper.Get_SqlConnstring(), CommandType.Text, Query);
                }
                return objData;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static object DeleteFromDatabase(IMaster iobjCustomer)
        {
            DBHelper objDBHelper = new DBHelper();
            try
            {
                string strResult = string.Empty;

                if (iobjCustomer.Is_Procedure == "PROCEDURE")
                {
                    string strProcedure;
                    object[] arrParams = setProcParams(iobjCustomer, out strProcedure);
                    int iResult = objDBHelper.ExecuteNonQuery(objDBHelper.Get_SqlConnstring(), strProcedure, arrParams);
                    if (iResult > 0)
                    {
                        strResult = "Success";
                    }
                    else
                    {
                        strResult = "NotSuccess";
                    }
                }

                return strResult;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        static object[] setProcParams(IMaster iobjCustomer, out string strPrcoName)
        {
            List<object> lstParams = new List<object>();

            switch (iobjCustomer.Operation_Name.Trim())
            {
                case "Customer_getallcustdetails":
                    strPrcoName = "spSearchCustomerDynamicSQL";
                    lstParams.Add(iobjCustomer.WireTransfer_ReferanceNo);
                    lstParams.Add(iobjCustomer.Post_Code);
                    // lstParams.Add(iobjCustomer.CustomerName);
                    lstParams.Add(iobjCustomer.Email_ID);
                    lstParams.Add(iobjCustomer.Mobile_Number);
                    lstParams.Add(iobjCustomer.sourceofregistration);
                    //lstParams.Add(iobjCustomer.cust_status);
                    //iobjCustomer.Operation_Name = "@Duplicate";
                    //lstParams.Add(iobjCustomer.Operation_Name);
                    break;

                case "Get_Customers_siteGroup":
                    strPrcoName = "WebRAM_prcGetCustomers";
                    lstParams.Add("Multiple_site_Group");
                    lstParams.Add(0);
                    // lstParams.Add(iobjCustomer.SPId);
                    lstParams.Add(null);
                    break;

                default:
                    strPrcoName = "";
                    break;
            }

            if (lstParams.Count > 0)
            {
                object[] arrParams = new object[lstParams.Count];
                for (int kk = 0; kk < lstParams.Count; kk++)
                {
                    arrParams[kk] = lstParams[kk];
                }
                return arrParams;
            }
            else
            {
                return (object[])null;
            }
        }

        private static string GetQuery_With_Param(IMaster iRepository)
        {

            string Query = string.Empty;
            switch (iRepository.Operation_Name.Trim())
            {
                case "Cashback_CheckDuplicate"://for duplicate_cashback
                    Query = "select * from cashbackdetails_table where " + iRepository.whereclause + " < (End_Date) and offertypeid = 1";
                    break;

            }

            return Query;
        }

            }
}
