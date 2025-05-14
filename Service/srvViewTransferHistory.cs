using System.Data;
using System.Globalization;
using System.Net;
using MySql.Data.MySqlClient;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Text.Json;


namespace Calyx_Solutions.Service
{
    public class srvViewTransferHistory
    {
        public DataTable ViewTransfer(Model.Transaction obj)
        {
            HttpContext context = null;
            int Customer_ID = Convert.ToInt32(CompanyInfo.Decrypt(obj.Customer_ID, true));
            List<Model.Transaction> _lst = new List<Model.Transaction>();
            string Beneficiary_ID_regex = validation.validate(Convert.ToString(obj.Beneficiary_ID), 0, 1, 1, 1, 1, 1, 1, 1, 1);
            //string WireTransfer_ReferanceNo_regex = validation.validate(Convert.ToString(obj.Customer.WireTransfer_ReferanceNo), 1, 1, 1, 1, 1, 1, 1, 1, 1);
            //string Full_Name_regex= validation.validate(Convert.ToString(obj.Customer.Full_Name), 1, 1, 1, 1, 1, 1, 0, 1, 1);
            string Transaction_ID_regex = validation.validate(Convert.ToString(obj.Transaction_ID), 0, 1, 1, 1, 1, 1, 1, 1, 1);
            string Customer_ID_regex = validation.validate(Convert.ToString(Customer_ID), 0, 1, 1, 1, 1, 1, 1, 1, 1);
            //string ReferenceNo_regex = validation.validate(Convert.ToString(obj.ReferenceNo), 1, 1, 1, 1, 1, 1, 1, 1, 1);
            //string Beneficiary_Name_regex = validation.validate(Convert.ToString(obj.Beneficiary_Name), 1, 1, 1, 1, 1, 1, 0, 1, 1);
            string Country_ID_regex = validation.validate(Convert.ToString(obj.Beneficiary_Name), 1, 1, 1, 1, 1, 1, 1, 1, 1);
            DataTable dt = new DataTable();
            if (Beneficiary_ID_regex != "false" && Transaction_ID_regex != "false" && Customer_ID_regex != "false"
             && Country_ID_regex != "false")
            {
                MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("Transaction_Search");
                _cmd.CommandType = CommandType.StoredProcedure;
                string _query = " ";
                if (obj.Beneficiary_ID > 0)
                {
                    _query = _query + " and aa.Beneficiary_ID=" + obj.Beneficiary_ID + "";
                }
                if (obj.Customer != null)
                {
                    if (obj.CustomerWireTransfer_ReferanceNo != "" && obj.CustomerWireTransfer_ReferanceNo != null)
                    {
                        _query = _query + " and WireTransfer_ReferanceNo  LIKE '%" + obj.CustomerWireTransfer_ReferanceNo + "%'";
                    }
                    if (obj.CustomerFull_Name != "" && obj.CustomerFull_Name != null)
                    {
                        _query = _query + " and concat(cc.first_Name,' ',cc.Last_Name) LIKE '%" + obj.CustomerFull_Name + "%'";
                    }
                }

                if (obj.Transaction_ID > 0)
                {
                    _query += " and aa.Transaction_ID=" + obj.Transaction_ID + "";
                }
                if (Customer_ID > 0)
                {
                    _query += " and  aa.customer_Id='" + Customer_ID + "'";
                }
                if (obj.ReferenceNo != "" && obj.ReferenceNo != null)
                {
                    _query += " and aa.ReferenceNo like '%" + obj.ReferenceNo + "%'";
                }
                if (obj.Beneficiary_Name != "" && obj.Beneficiary_Name != null)
                {
                    _query += " and beneficiary_name  like '%" + obj.Beneficiary_Name.Replace("'", "''") + "%'";
                }
                if (obj.FromDate != null && obj.ToDate != null && obj.FromDate != "" && obj.ToDate != "")
                {
                    DateTime txtdate = DateTime.ParseExact(obj.FromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    DateTime txtdate1 = DateTime.ParseExact(obj.ToDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    _query += "  and  Date(aa.Record_Insert_DateTime) between '" + txtdate.ToString("yyyy-MM-dd") + "' and '" + txtdate1.ToString("yyyy-MM-dd") + "' ";
                }
                if (obj.Country_ID > 0)
                {
                    _query += " and aa.Country_ID=" + obj.Country_ID;
                }
                string _limit = " limit 1000";
                if (obj.PartPay_Flag > 0)
                {
                    _limit = " limit " + obj.PartPay_Flag;
                }
                _query += " and aa.Client_ID=" + obj.Client_ID;
                _cmd.Parameters.AddWithValue("_whereclause", _query);
                _cmd.Parameters.AddWithValue("_limit", _limit);
                dt = db_connection.ExecuteQueryDataTableProcedure(_cmd);
                if (dt.Rows.Count > 0)//amruta
                {
                    //var context = System.Web.HttpContext.Current;
                    DataTable ds1 = CompanyInfo.get(obj.Client_ID, context);
                    string Company_URL_Admin = "", Company_Name = "";
                    if (ds1.Rows.Count > 0)
                    {
                        Company_URL_Admin = Convert.ToString(ds1.Rows[0]["Company_URL_Admin"]);                       
                    }

                    foreach (DataRow row in dt.Rows)
                    {
                        if (row["Document_Name"] != DBNull.Value && row["Document_Name"].ToString() != "")
                        {
                            string image_link = Company_URL_Admin + row["Document_Name"].ToString();
                            string base64str = CompanyInfo.ConvertImageLinkToBase64(image_link);
                            row["Document_Name"] = base64str;
                        }
                    }

                }
            }
            else
            {
                string msg = "Validation Error Beneficiary_ID_regex- " + Beneficiary_ID_regex + "Transaction_ID_regex-" + Transaction_ID_regex + "Customer_ID_regex-" + Customer_ID_regex + "Country_ID_regex- " + Country_ID_regex;
                int stattus = (int)CompanyInfo.InsertActivityLogDetailsSecurity(msg, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), 1, "srvViewTransferHistory", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "ViewTransfer", 0, context);
            }
            return dt;
        }
        public DataTable Cancellation_Request(Model.Transaction obj , HttpContext context)
        {

            string BCC = string.Empty; string CC = string.Empty; string send_mail = string.Empty; string URL = string.Empty;
            HttpWebRequest httpRequest; string URL_Cust = string.Empty;
            HttpWebRequest httpRequest1 = null;
            string Activity = string.Empty;
            string errMessage = string.Empty;
            int msg = 0;
            DataTable ds = new DataTable(); string get_flag = string.Empty;
            string coll_flag = string.Empty;
            
            ds.Columns.Add("Status", typeof(int));
            ds.Columns.Add("Message", typeof(string));
            try
            {
                string Proceed_Reason_regex = validation.validate(Convert.ToString(obj.Proceed_Reason), 1, 1, 1, 1, 1, 1, 1, 1, 1);
                if (Proceed_Reason_regex != "false")
                {
                    var jsonString = String.Empty;
                    // context = HttpContext.Current;

                  MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("SP_Send_CancellationRequest");
                    _cmd.CommandType = CommandType.StoredProcedure;
                    _cmd.Parameters.AddWithValue("_transaction_id", obj.Transaction_ID);
                    _cmd.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                    _cmd.Parameters.AddWithValue("_Cancellation_Reason", obj.Proceed_Reason);
                    msg = db_connection.ExecuteNonQueryProcedure(_cmd);

                    if (msg > 0)
                    {

                        string EmailID = string.Empty;
                        DataTable dt_admin_Email_list = new DataTable();
                        dt_admin_Email_list = (DataTable)CompanyInfo.getAdminEmailList(obj.Client_ID, obj.Branch_ID);
                        if (dt_admin_Email_list.Rows.Count > 0)
                        {
                            for (int a = 0; a < dt_admin_Email_list.Rows.Count; a++)
                            {
                                if (Convert.ToString(dt_admin_Email_list.Rows[a]["Email_ID"]) != "" && Convert.ToString(dt_admin_Email_list.Rows[a]["Email_ID"]) != null)
                                {
                                    string AdminEmailID = Convert.ToString(dt_admin_Email_list.Rows[a]["Email_ID"]) + ",";
                                    EmailID += AdminEmailID;
                                }
                            }
                        }
                        DataTable dtc = CompanyInfo.get(obj.Client_ID , context);
                        MySqlConnector.MySqlCommand _cmd1 = new MySqlConnector.MySqlCommand("Transaction_Search");
                        _cmd1.CommandType = CommandType.StoredProcedure;
                        string _query = " ";
                        if (obj.Transaction_ID > 0)
                        {
                            _query = _query + " and aa.Transaction_ID=" + obj.Transaction_ID + "";
                        }
                        _query += " and aa.Client_ID=" + obj.Client_ID;
                        _cmd1.Parameters.AddWithValue("_whereclause", _query);
                        _cmd1.Parameters.AddWithValue("_limit", " limit 1");
                        DataTable dt_transaction = db_connection.ExecuteQueryDataTableProcedure(_cmd1);
                        string subject = string.Empty;
                        string body = string.Empty;

                        httpRequest = (HttpWebRequest)WebRequest.Create("" + Convert.ToString(dtc.Rows[0]["Company_URL_Admin"]) + "Email/CancellationRequest.html");

                        httpRequest.UserAgent = "Code Sample Web Client";
                        HttpWebResponse webResponse = (HttpWebResponse)httpRequest.GetResponse();
                        using (StreamReader reader = new StreamReader(webResponse.GetResponseStream()))
                        {
                            body = reader.ReadToEnd();
                        }
                        body = body.Replace("[CUSTOMER_REF]", Convert.ToString(dt_transaction.Rows[0]["WireTransfer_ReferanceNo"]));

                        body = body.Replace("[TRANSFER_REF]", Convert.ToString(dt_transaction.Rows[0]["ReferenceNo"]));

                        subject = "" + dtc.Rows[0]["Company_Name"] + " -  Transfer Cancellation Request " + dt_transaction.Rows[0]["WireTransfer_ReferanceNo"];
                        string mail_send = (string)CompanyInfo.Send_Mail(dtc, EmailID, body, subject, obj.Client_ID, obj.Branch_ID, "cancellation request", "", "" , context);
                        if (mail_send == "Success")
                        {
                            ds.Rows.Add(0, "Success");
                            Activity = "<b>" + obj.Customer + "</b>" + " sent cancellation request";
                            int stattus = (int)CompanyInfo.InsertActivityLogDetails(Activity, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), 0, "InsertPrimaryDocumentID", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "ID Upload" , context);
                        }
                        else
                        {
                            ds.Rows.Add(0, "Success");
                            Activity = "<b>" + obj.Customer + "</b>" + " sent cancellation request but email sending failed";
                            int stattus = (int)CompanyInfo.InsertActivityLogDetails(Activity, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), 0, "InsertPrimaryDocumentID", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "ID Upload" , context);
                        }
                        //string madetxn = Convert.ToString(context.Request.Form["Flag_Status"]);
                        //if (madetxn != null && madetxn != "")
                        //{
                        //    c.MadeThisTransfer_Flag = Convert.ToInt32(madetxn);
                        //}
                        //c.MadeThisTransfer_Label = Convert.ToString(context.Request.Form["Flag"]);
                        //string cust_status = Convert.ToString(c.MadeThisTransfer_Label);//i have made the transfer
                        ////    c.Id = Convert.ToInt32(dictObjMain["Email_Template_Id"]);
                        //string subject = string.Empty;
                        //DataTable dt_cust = (DataTable)CompanyInfo.getCustomerDetails(c.Client_ID, c.Customer_ID);
                        //string full_name = dt_cust.Rows[0]["Full_name"].ToString();
                        //string Email_ID = dt_cust.Rows[0]["Email_ID"].ToString();
                        //string WireTransfer_ReferanceNo = dt_cust.Rows[0]["WireTransfer_ReferanceNo"].ToString();

                        //int flag = 0;
                        //if (c.MadeThisTransfer_Flag == 0)//status
                        //{
                        //    flag = 1;
                        //}
                        //if (c.MadeThisTransfer_Flag == 1)
                        //{
                        //    flag = 0;
                        //}

                        //int Update_Flag_status = Update_Flag(c.Transaction_ID, c.Client_ID, flag);

                        //if (c.Transaction_ID != 0 && Update_Flag_status > 0)
                        //{
                        //    ds.Rows.Add(0, "Success");
                        //    CompanyInfo.InsertActivityLogDetails("App - Bank Account payment from customer -" + WireTransfer_ReferanceNo + ". ", c.User_ID, c.Transaction_ID, c.User_ID, c.Customer_ID, "Upload_Receipt", c.CB_ID, c.Client_ID, "View Transfer");
                        //    string msg1 = "Success";
                        //    mts_connection _MTS = new mts_connection();
                        //    MySqlConnection con = new MySqlConnection(_MTS.WebConnSetting());
                        //    con.Open();

                        //    MySqlCommand cmdupdate1 = new MySqlCommand("SP_Get_Email_Permission", con);
                        //    cmdupdate1.CommandType = CommandType.StoredProcedure;
                        //    cmdupdate1.Parameters.AddWithValue("_ID", 5);
                        //    cmdupdate1.Parameters.AddWithValue("_Client_ID", c.Client_ID);
                        //    string permission_status = string.Empty;
                        //    DataTable dt1 = db_connection.ExecuteQueryDataTableProcedure(cmdupdate1);
                        //    if (dt1.Rows.Count > 0)
                        //    {
                        //        permission_status = Convert.ToString(dt1.Rows[0]["Status_ForCustomer"]);
                        //    }
                        //    if (permission_status == "0")//send mail
                        //    {
                        //        send_mail = "true";
                        //    }
                        //    else
                        //    {
                        //        send_mail = "false";
                        //    }
                        //    if (send_mail == "true")
                        //    {
                        //        string email = Email_ID;
                        //        string body = string.Empty;
                        //        string body1 = string.Empty;
                        //        string template = "";

                        //        DataTable dt_transaction = (DataTable)getTransactionDetails(c.Transaction_ID, c.Client_ID);

                        //        URL_Cust = Convert.ToString(company_email_details.Rows[0]["Company_URL_Customer"]) + "Email";

                        //        httpRequest1 = (HttpWebRequest)WebRequest.Create(URL_Cust + "/AdminPayementApp.htm");

                        //        httpRequest1.UserAgent = "Code Sample Web Client";
                        //        HttpWebResponse webResponse = (HttpWebResponse)httpRequest1.GetResponse();
                        //        using (StreamReader reader = new StreamReader(webResponse.GetResponseStream()))
                        //        {
                        //            body = reader.ReadToEnd();
                        //        }

                        //        if (dt_transaction.Rows.Count > 0)
                        //        {
                        //            double total = Convert.ToDouble(dt_transaction.Rows[0]["AmountInGBP"].ToString()) + Convert.ToDouble(dt_transaction.Rows[0]["Transfer_Fees"].ToString());
                        //            body = body.Replace("[name]", full_name);
                        //            body = body.Replace("[payment_status]", cust_status);
                        //            body = body.Replace("[Refer]", dt_transaction.Rows[0]["ReferenceNo"].ToString());
                        //            body = body.Replace("[Date]", dt_transaction.Rows[0]["Transfer_date"].ToString());
                        //            body = body.Replace("[amountingbp]", dt_transaction.Rows[0]["AmountINGBP"].ToString());
                        //            body = body.Replace("[exchangerate]", dt_transaction.Rows[0]["Exchange_Rate"].ToString());
                        //            body = body.Replace("[amountinforcurr]", dt_transaction.Rows[0]["Amount_in_other_cur"].ToString());
                        //            body = body.Replace("[transferfess]", dt_transaction.Rows[0]["Transfer_Fees"].ToString());
                        //            body = body.Replace("[totalamt]", Convert.ToString(total));

                        //        }
                        //        body = body.Replace("[cust_reference_no]", WireTransfer_ReferanceNo);
                        //        if (c.MadeThisTransfer_Flag == 1)
                        //            body = body.Replace("[complete]", "complete");
                        //        else
                        //            body = body.Replace("[complete]", "is not completed");

                        //        subject = "[company_name] - Bank Account payment from customer -" + WireTransfer_ReferanceNo;
                        //        string activity = string.Empty;
                        //        string mail_send = (string)CompanyInfo.Send_Mail(company_email_details, email, body, subject, c.Client_ID, c.Branch_ID, "", CC, BCC);

                        //        if (mail_send == "Success")
                        //        {
                        //            CompanyInfo.InsertActivityLogDetails("App - Bank Account payment email sent successfully. ", c.User_ID, c.Transaction_ID, c.User_ID, c.Customer_ID, "Upload_Receipt", c.CB_ID, c.Client_ID, "View Transfer");
                        //        }
                        //        else//not success
                        //        {
                        //            CompanyInfo.InsertActivityLogDetails("App - Email sending failed. " + mail_send + " ", c.User_ID, c.Transaction_ID, c.User_ID, c.Customer_ID, "Upload_Receipt", c.CB_ID, c.Client_ID, "View Transfer");
                        //            mail_send = "NotSuccess";
                        //        }
                        //    }
                        //}
                        //else
                        //{
                        //    ds.Rows.Add(1, "Failed");
                        //    msg = 0;
                        //}
                    }
                    else
                    {
                        ds.Rows.Add(2, "failed to send cancellation request");
                    }
                }
                else
                {

                    string msg1 = "Validation Error Proceed_Reason_regex- " + Proceed_Reason_regex;
                    int stattus = (int)CompanyInfo.InsertActivityLogDetailsSecurity(msg1, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), 1, "srvViewTransferHistory", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "Cancellation_Request", 0 , context);
                }
            }
            catch (Exception ex)
            {
                CompanyInfo.InsertActivityLogDetails("Cancellation transaction error : " + ex.ToString(), 0, 0, 0, 0, "srvViewTransferHistory", 0, 0, "", context);
                ds.Rows.Add(3, ex.ToString());
                throw ex;
                //string stattus = (string)mtsmethods.InsertErrorLogDetails(ex.Message, user_id, "Upload_Reciept", branch_id, client_id);
            }
            return ds;
        }


        public DataTable ViewRewardDetails(Model.Transaction obj)
        {
            List<Model.Transaction> _lst = new List<Model.Transaction>();
           MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("SP_Get_Reward_Details");
            _cmd.CommandType = CommandType.StoredProcedure;
            string _query = " ";
            if (obj.Transaction_ID > 0)
            {
                _query = _query + " and  aa.Transaction_ID=" + obj.Transaction_ID + "";
            }
            if (obj.Client_ID != null && obj.Client_ID != -1)
            {
                _query = _query + " and  aa.Client_ID=" + obj.Client_ID + "";
            }
            _cmd.Parameters.AddWithValue("_whereclause", _query);
            DataTable dt = db_connection.ExecuteQueryDataTableProcedure(_cmd);
            return dt;
        }


        public  async Task<DataTable> Upload_Reciept(Model.TransactionReceipt c, HttpContext context, IFormFile? f_file)
        {
            //Model.TransactionReceipt c = new Model.TransactionReceipt();
            string BCC = string.Empty; string CC = string.Empty; string send_mail = string.Empty; string URL = string.Empty;
            HttpWebRequest httpRequest; string URL_Cust = string.Empty;
            HttpWebRequest httpRequest1 = null;
            string Activity = string.Empty;
            string errMessage = string.Empty;
            int msg = 0;
            DataTable ds = new DataTable(); string get_flag = string.Empty;
            string coll_flag = string.Empty;

            ds.Columns.Add("Status", typeof(int));
            ds.Columns.Add("Message", typeof(string));
            string recipt_activity = "";
            try
            {
                var jsonString = String.Empty;
                //var context = HttpContext.Current;
                recipt_activity += " clinentid," + context.Request.Form["clientId"] + " branchId," + context.Request.Form["branchId"] + " customerId," + context.Request.Form["customerId"] + " transactionId," + context.Request.Form["transactionId"];
                c.Client_ID = Convert.ToInt32(context.Request.Form["clientId"]);
                c.Branch_ID = Convert.ToInt32(context.Request.Form["branchId"]);
                int Customer_ID = Convert.ToInt32(CompanyInfo.Decrypt(context.Request.Form["customerId"], true));
                c.Customer_ID = Convert.ToString(Customer_ID);
                c.CB_ID = Convert.ToInt32(context.Request.Form["branchId"]);
                c.Transaction_ID = Convert.ToInt32(context.Request.Form["transactionId"]);
                //string SendMoney_Flag = Convert.ToString(context.Request.Form["is_uploaded"]);
                string SendMoney_Flag = "";
                send_mail = "true"; //Convert.ToString(context.Request.Form["send_mail"]);

                c.Record_Insert_DateTime = (string)CompanyInfo.gettime(c.Client_ID, context);
                c.Delete_Status = 0;


                c.ReceiptNameWithExt = "";
                IFormFile postedFile = null;
                DataTable company_email_details = (DataTable)CompanyInfo.get(c.Client_ID, context);
                if (context.Request.Form.Files.Count > 0)
                {
                    recipt_activity += " = audit 01((y)";
                    SendMoney_Flag = "Y";
                }
                else
                {
                    recipt_activity += " = audit 02(n)";
                    SendMoney_Flag = "N";
                }
                if (SendMoney_Flag == "Y")
                {
                    recipt_activity += "03 : inside in Y = audit ";
                    //if (context.Request.Files.Count > 0)
                    var files = context.Request.Form.Files;
                    if (files.Count > 0)
                    {

                        recipt_activity += "04 : inside in file count";
                        //HttpPostedFile postedFile = context.Request.Files[0];
                        postedFile = files[0];
                        recipt_activity += "05 : inside in posted file count "+ postedFile.FileName;
                        //Set the Folder Path.
                        string fileName = string.Empty;
                        string filen;

                        //          string folderPath = "F://poojaT//12_11_2020//MTS_App-Api_2//MTS-App-Api//assets/Uploads";
                        //  string folderPath=   context.Server.MapPath("~/assets/Uploads/");
                        //  string folderPath = "~/assets/Receipt_Upload/";
                        string folderPath = Convert.ToString(company_email_details.Rows[0]["RootURL"]) + "assets/Receipt_Upload/";
                        string ext = Path.GetExtension(postedFile.FileName);
                        recipt_activity += "Extension " + ext;
                        bool checkvalidext = CompanyInfo.chkValidExtensionforall(ext);
                        if (!checkvalidext)
                        {
                            recipt_activity += "06 : Invalid Receipt " + checkvalidext;
                            ds.Rows.Add(4, "Invalid Receipt");
                            return ds;
                        }
                        filen = c.Record_Insert_DateTime;

                        string path = filen.Replace(":", "") + "" + ext + "";
                        //postedFile.SaveAs(folderPath + path);
                        var filePath = Path.Combine(folderPath, path);
                        await using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            recipt_activity += "07 : insidefilestrm " + checkvalidext;
                           await   postedFile.CopyToAsync(fileStream);
                        }
                        c.ReceiptNameWithExt = "assets/Receipt_Upload/" + path;
                    }
                    //Save the File in Folder.

                }
                else if (SendMoney_Flag == "N")
                {
                }
                recipt_activity += "08 : Upload_Reciept" ;
                MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("Upload_Reciept");
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.Parameters.AddWithValue("_Transaction_ID", c.Transaction_ID);
                _cmd.Parameters.AddWithValue("_Client_ID", c.Client_ID);
                _cmd.Parameters.AddWithValue("_CB_ID", c.Branch_ID);
                _cmd.Parameters.AddWithValue("_ReceiptNameWithExt", c.ReceiptNameWithExt);
                _cmd.Parameters.AddWithValue("_Record_Insert_DateTime", c.Record_Insert_DateTime);
                _cmd.Parameters.AddWithValue("_Delete_Status", 0);
                msg = db_connection.ExecuteNonQueryProcedure(_cmd);

                if (msg > 0)
                {
                    recipt_activity += "09 : Upload_Reciept record update";
                    var MadeThisTransfer_Flag = 1;
                    string btn = Convert.ToString(context.Request.Form["btnValue"]);
                    if (btn == "I made this payment")
                    {
                        recipt_activity += "10 : record"+ btn;
                        MadeThisTransfer_Flag = 0;
                    }
                    if (btn == "I haven't made this payment")
                    {
                        recipt_activity += "11 : record" + btn;
                        MadeThisTransfer_Flag = 1;
                    }



                    //string madetxn = Convert.ToString(context.Request.Form["flagStatus"]);
                    string madetxn = MadeThisTransfer_Flag.ToString();
                    if (madetxn != null && madetxn != "")
                    {
                        c.MadeThisTransfer_Flag = Convert.ToInt32(madetxn);
                    }
                    c.MadeThisTransfer_Label = Convert.ToString(context.Request.Form["Flag"]);
                    string cust_status = Convert.ToString(c.MadeThisTransfer_Label);//i have made the transfer
                                                                                    //    c.Id = Convert.ToInt32(dictObjMain["Email_Template_Id"]);
                    string subject = string.Empty;
                    DataTable dt_cust = (DataTable)CompanyInfo.getCustomerDetails(c.Client_ID, Customer_ID);
                    string full_name = dt_cust.Rows[0]["Full_name"].ToString();
                    string Email_ID = dt_cust.Rows[0]["Email_ID"].ToString();
                    string WireTransfer_ReferanceNo = dt_cust.Rows[0]["WireTransfer_ReferanceNo"].ToString();

                    int flag = 0;
                    if (c.MadeThisTransfer_Flag == 0)//status
                    {
                        flag = 1;
                    }
                    if (c.MadeThisTransfer_Flag == 1)
                    {
                        flag = 0;
                    }

                    int Update_Flag_status = Update_Flag(c.Transaction_ID, c.Client_ID, flag);

                    if (c.Transaction_ID != 0 && Update_Flag_status > 0)
                    {
                        recipt_activity += "11 : updatesuccess";
                        ds.Rows.Add(0, "Success");
                        if (c.MadeThisTransfer_Flag == 0)//status
                        {
                            CompanyInfo.InsertTrackingLogDetails(2, c.Transaction_ID, 0, c.Client_ID, c.CB_ID, context);
                        }
                        CompanyInfo.InsertActivityLogDetails("App - Bank Account payment from customer -" + WireTransfer_ReferanceNo + ". ", c.User_ID, c.Transaction_ID, c.User_ID, Customer_ID, "Upload_Receipt", c.CB_ID, c.Client_ID, "View Transfer", context);
                        string msg1 = "Success";
                        mts_connection _MTS = new mts_connection();
                        MySqlConnector.MySqlConnection con = new MySqlConnector.MySqlConnection(_MTS.WebConnSetting());
                        con.Open();

                        MySqlConnector.MySqlCommand cmdupdate1 = new MySqlConnector.MySqlCommand("SP_Get_Email_Permission", con);
                        cmdupdate1.CommandType = CommandType.StoredProcedure;
                        cmdupdate1.Parameters.AddWithValue("_ID", 5);
                        cmdupdate1.Parameters.AddWithValue("_Client_ID", c.Client_ID);
                        string permission_status = string.Empty;
                        DataTable dt1 = db_connection.ExecuteQueryDataTableProcedure(cmdupdate1);
                        if (dt1.Rows.Count > 0)
                        {
                            permission_status = Convert.ToString(dt1.Rows[0]["Status_ForCustomer"]);
                        }
                        if (permission_status == "0")//send mail
                        {
                            send_mail = "true";
                        }
                        else
                        {
                            send_mail = "false";
                        }
                        if (send_mail == "true")
                        {
                            string email = Email_ID;
                            string body = string.Empty;
                            string body1 = string.Empty;
                            string template = "";

                            DataTable dt_transaction = (DataTable)getTransactionDetails(c.Transaction_ID, c.Client_ID, context);

                            URL_Cust = Convert.ToString(company_email_details.Rows[0]["Company_URL_Customer"]) + "Email";

                            httpRequest1 = (HttpWebRequest)WebRequest.Create(URL_Cust + "/AdminPayementApp.htm");

                            httpRequest1.UserAgent = "Code Sample Web Client";
                            HttpWebResponse webResponse = (HttpWebResponse)httpRequest1.GetResponse();
                            using (StreamReader reader = new StreamReader(webResponse.GetResponseStream()))
                            {
                                body = reader.ReadToEnd();
                            }

                            if (dt_transaction.Rows.Count > 0)
                            {
                                double total = Convert.ToDouble(dt_transaction.Rows[0]["AmountInGBP"].ToString()) + Convert.ToDouble(dt_transaction.Rows[0]["Transfer_Fees"].ToString());
                                body = body.Replace("[name]", Convert.ToString(dt_cust.Rows[0]["First_Name"])); //full_name);
                                body = body.Replace("[payment_status]", cust_status);
                                body = body.Replace("[Refer]", dt_transaction.Rows[0]["ReferenceNo"].ToString());
                                body = body.Replace("[Date]", dt_transaction.Rows[0]["Transfer_date"].ToString());
                                body = body.Replace("[amountingbp]", dt_transaction.Rows[0]["AmountINGBP"].ToString());
                                body = body.Replace("[exchangerate]", dt_transaction.Rows[0]["Exchange_Rate"].ToString());
                                body = body.Replace("[amountinforcurr]", dt_transaction.Rows[0]["Amount_in_other_cur"].ToString());
                                body = body.Replace("[transferfess]", dt_transaction.Rows[0]["Transfer_Fees"].ToString());
                                body = body.Replace("[totalamt]", Convert.ToString(total));

                            }
                            body = body.Replace("[cust_reference_no]", WireTransfer_ReferanceNo);
                            if (c.MadeThisTransfer_Flag == 1)
                                body = body.Replace("[complete]", "complete");
                            else
                                body = body.Replace("[complete]", "is not completed");

                            subject = "[company_name] - Bank Account payment from customer -" + WireTransfer_ReferanceNo;
                            string activity = string.Empty;
                            string mail_send = (string)CompanyInfo.Send_Mail(company_email_details, email, body, subject, c.Client_ID, c.Branch_ID, "", CC, BCC, context);

                            if (mail_send == "Success")
                            {
                                recipt_activity += "10 : mail success";
                                CompanyInfo.InsertActivityLogDetails("App - Bank Account payment email sent successfully. ", c.User_ID, c.Transaction_ID, c.User_ID, Customer_ID, "Upload_Receipt", c.CB_ID, c.Client_ID, "View Transfer", context);
                            }
                            else//not success
                            {
                                recipt_activity += "11 : mail Failed";
                                CompanyInfo.InsertActivityLogDetails("App - Email sending failed. " + mail_send + " ", c.User_ID, c.Transaction_ID, c.User_ID, Customer_ID, "Upload_Receipt", c.CB_ID, c.Client_ID, "View Transfer", context);
                                mail_send = "NotSuccess";
                            }
                        }
                    }
                    else
                    {

                        recipt_activity += "12 : Failed";
                        ds.Rows.Add(1, "Failed");
                        msg = 0;
                    }
                }
                else
                {
                    recipt_activity += "14 : Upload receipt Failed";
                    ds.Rows.Add(2, "Upload receipt Failed");
                }
            }
            catch (Exception ex)
            {
                recipt_activity += "15 : Failed" + ex;
                ds.Rows.Add(3, ex.ToString());
                throw ex;
                //string stattus = (string)mtsmethods.InsertErrorLogDetails(ex.Message, user_id, "Upload_Reciept", branch_id, client_id);
            }
            finally
            {
                Model.ErrorLog objError = new Model.ErrorLog();
                objError.User = new Model.User();
                objError.Branch = new Model.Branch();
                objError.Client = new Model.Client();

                objError.Error = "Api : audit uploadreceipt --" + recipt_activity;
                objError.Branch_ID = 1;
                objError.Date = DateTime.Now;
                objError.User_ID = 1;
                objError.Id = 1;
                objError.Function_Name = "Upload";
                Service.srvErrorLog srvError = new Service.srvErrorLog();
                srvError.Create(objError, context);
            }
            return ds;
        }

        public int Update_Flag(int Trn_ID, int Client_ID, int flag)
        {
            int result = 0;
            try
            {
                MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("Update_Transfer_Flag_Status");
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.Parameters.AddWithValue("_Transaction_ID", Trn_ID);
                _cmd.Parameters.AddWithValue("_Client_ID", Client_ID);
                _cmd.Parameters.AddWithValue("_status", flag);
                result = db_connection.ExecuteNonQueryProcedure(_cmd);
            }
            catch (Exception ae)
            {

            }
            return result;
        }

        public DataTable getTransactionDetails(int Trn_ID, int Client_ID, HttpContext context)
        {
            DataTable dt = new DataTable();
            try
            {
                MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("Made_this_Transfer_Details");
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.Parameters.AddWithValue("_Transaction_ID", Trn_ID);
                _cmd.Parameters.AddWithValue("_Client_ID", Client_ID);
                dt = db_connection.ExecuteQueryDataTableProcedure(_cmd);
            }
            catch (Exception ae)
            {

            }
            return dt;
        }

        public DataTable SelectBankDetails(Model.Transaction obj, HttpContext context) // Anushka
        {
            MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("selectbankdetails");
            _cmd.CommandType = CommandType.StoredProcedure;
            _cmd.Parameters.AddWithValue("_currencyCode", obj.basecurrency);
            _cmd.Parameters.AddWithValue("_deliveryTypeID", obj.DeliveryType_Id);
            _cmd.Parameters.AddWithValue("_collectionTypeID", obj.PaymentDepositType_ID);

            string acitivity = "basecurrency:-" + obj.basecurrency + "deliveryType:- " + obj.DeliveryType_Id + "collectionType :- " + obj.PaymentDepositType_ID + "";



            DataTable dt = db_connection.ExecuteQueryDataTableProcedure(_cmd);
            Int32 table = dt.Rows.Count;
            CompanyInfo.InsertActivityLogDetails("App - SelectBankDetails. " + acitivity + " ", 0, 0, 0, 0, "bankdetails :-" + table, 2, 1, "View Transfer", context);
            return dt;
        }

        public DataTable MadeThisTransferFlag_Mail(Model.Transaction c, HttpContext context)//I haven't made this transfer
        {
            int Customer_ID = Convert.ToInt32(CompanyInfo.Decrypt(c.Customer_ID, true));
            string activity = string.Empty; string URL = string.Empty; string URL_Cust = string.Empty;
            HttpWebRequest httpRequest;
            HttpWebRequest httpRequest1 = null;
            DataTable ds = new DataTable();
            ds.Columns.Add("Status", typeof(int));
            ds.Columns.Add("Message", typeof(string));
            try
            {
                var jsonString = String.Empty;

                //c.Customer_ID = Convert.ToInt32(dictObjMain["Customer_ID"]);
                //c.Transaction_ID = Convert.ToInt32(dictObjMain["Trn_ID"]);
                //c.Client_ID = Convert.ToInt32(dictObjMain["Client_ID"]);
                //URL = Convert.ToString(HttpContext.Current.Session["Company_URL"]);
                //URL_Cust = Convert.ToString(HttpContext.Current.Session["Customer_URL"]);
                ////URL = Convert.ToString(dictObjMain["URL"]); URL_Cust = Convert.ToString(dictObjMain["URL_Cust"]);
                //context.Session.Add("Client_ID", c.Client_ID);
                if (c.MadeThisTransfer_Label == "I haven not made this payment")
                {
                    c.MadeThisTransfer_Label = c.MadeThisTransfer_Label.Replace("*", "'");
                }

                string cust_status = Convert.ToString(c.MadeThisTransfer_Label);//i have made the transfer
                //c.status = Convert.ToInt32(dictObjMain["Flag_Status"]);//0 or 1
                ////    c.Id = Convert.ToInt32(dictObjMain["Email_Template_Id"]);
                string subject = string.Empty;
                DataTable dt_cust = (DataTable)CompanyInfo.getCustomerDetails(c.Client_ID, Customer_ID);
                string full_name = dt_cust.Rows[0]["Full_name"].ToString();
                string Email_ID = dt_cust.Rows[0]["Email_ID"].ToString();
                string WireTransfer_ReferanceNo = dt_cust.Rows[0]["WireTransfer_ReferanceNo"].ToString();
                //client_id = Convert.ToInt32(dictObjMain["Client_ID"]);
                int flag = 0;
                if (c.MadeThisTransfer_Flag == 0)//status
                {
                    flag = 1;
                }
                if (c.MadeThisTransfer_Flag == 1)
                {
                    flag = 0;
                }

                int Update_Flag_status = Update_Flag(c.Transaction_ID, c.Client_ID, flag);

                if (c.Transaction_ID != 0 && Update_Flag_status > 0)
                {
                    ds.Rows.Add(0, "Success");
                    try
                    {

                        mts_connection _MTS = new mts_connection();
                        MySqlConnector.MySqlConnection con = new MySqlConnector.MySqlConnection(_MTS.WebConnSetting());
                        con.Open();

                        MySqlConnector.MySqlCommand cmdupdate1 = new MySqlConnector.MySqlCommand("SP_Get_Email_Permission", con);
                        cmdupdate1.CommandType = CommandType.StoredProcedure;
                        cmdupdate1.Parameters.AddWithValue("_ID", 5);
                        cmdupdate1.Parameters.AddWithValue("_Client_ID", c.Client_ID);
                        //cmdupdate1.ExecuteScalar();
                        // obj.CommentUserId = 1;
                        // int abcd = Convert.ToInt32(cmdupdate1.Parameters["Status"].Value);
                        string permission_status = string.Empty;
                        DataTable dt1 = db_connection.ExecuteQueryDataTableProcedure(cmdupdate1);
                        if (dt1.Rows.Count > 0)
                        {
                            permission_status = Convert.ToString(dt1.Rows[0]["Status_ForCustomer"]);
                        }
                        if (permission_status == "0")//send mail
                        {
                            string email = Email_ID;
                            //string subject = string.Empty;
                            string body = string.Empty;
                            string body1 = string.Empty;
                            string template = "";
                            DataTable dt_transaction = (DataTable)getTransactionDetails(c.Transaction_ID, c.Client_ID, context);

                            DataTable company_email_details = (DataTable)CompanyInfo.get(c.Client_ID, context);
                            URL_Cust = Convert.ToString(company_email_details.Rows[0]["Company_URL_Admin"]) + "/Email";//URL_Cust = Convert.ToString(company_email_details.Rows[0]["Company_URL_Customer"]) + "/Email";

                            httpRequest1 = (HttpWebRequest)WebRequest.Create(URL_Cust + "/AdminPayementApp.htm");
                            httpRequest1.UserAgent = "Code Sample Web Client";
                            HttpWebResponse webResponse = (HttpWebResponse)httpRequest1.GetResponse();
                            using (StreamReader reader = new StreamReader(webResponse.GetResponseStream()))
                            {
                                body = reader.ReadToEnd();
                            }

                            if (dt_transaction.Rows.Count > 0)
                            {
                                double total = Convert.ToDouble(dt_transaction.Rows[0]["AmountInGBP"].ToString()) + Convert.ToDouble(dt_transaction.Rows[0]["Transfer_Fees"].ToString());
                                body = body.Replace("[name]", Convert.ToString(dt_cust.Rows[0]["First_Name"])); //full_name);
                                body = body.Replace("[payment_status]", cust_status);
                                body = body.Replace("[Refer]", dt_transaction.Rows[0]["ReferenceNo"].ToString());
                                body = body.Replace("[Date]", dt_transaction.Rows[0]["Transfer_date"].ToString());
                                body = body.Replace("[amountingbp]", dt_transaction.Rows[0]["AmountINGBP"].ToString());
                                body = body.Replace("[exchangerate]", dt_transaction.Rows[0]["Exchange_Rate"].ToString());
                                body = body.Replace("[amountinforcurr]", dt_transaction.Rows[0]["Amount_in_other_cur"].ToString());
                                body = body.Replace("[transferfess]", dt_transaction.Rows[0]["Transfer_Fees"].ToString());
                                body = body.Replace("[totalamt]", Convert.ToString(total));

                            }
                            subject = Convert.ToString(company_email_details.Rows[0]["Company_Name"]) + " - Bank Account payment " + WireTransfer_ReferanceNo + "";
                            body = body.Replace("[cust_reference_no]", WireTransfer_ReferanceNo);
                            if (c.MadeThisTransfer_Flag == 1)
                                body = body.Replace("[complete]", "complete");
                            else
                                body = body.Replace("[complete]", "is not completed");

                            CompanyInfo.Send_Mail(company_email_details, email, body, subject, c.Client_ID, c.Branch_ID, "", "", "", context);
                            //using (MailMessage mailMessage = new MailMessage())
                            //{
                            //    string from = company_email_details.Rows[0]["Email_Convey_from"].ToString(),

                            //    password = company_email_details.Rows[0]["Password"].ToString();
                            //    body = body.Replace("[company_website]", Convert.ToString(company_email_details.Rows[0]["company_website"]));
                            //    body = body.Replace("[company_name]", Convert.ToString(company_email_details.Rows[0]["Company_Name"]));

                            //    body = body.Replace("[contact_no]", Convert.ToString(company_email_details.Rows[0]["Company_mobile"]));
                            //    body = body.Replace("[email_id]", Convert.ToString(company_email_details.Rows[0]["Company_Email"]));
                            //    string send_mail = "mailto:" + Convert.ToString(company_email_details.Rows[0]["Company_Email"]);
                            //    body = body.Replace("[email_id1]", send_mail);
                            //    body = body.Replace("[privacy_policy]", Convert.ToString(company_email_details.Rows[0]["privacy_policy"]));
                            //    body = body.Replace("[company_logo]", Convert.ToString(company_email_details.Rows[0]["Image"]));
                            //    body = body.Replace("[company_website]", Convert.ToString(company_email_details.Rows[0]["Company_website"]));

                            //    mailMessage.From = new MailAddress(from);
                            //    mailMessage.Body = body;
                            //    mailMessage.Subject = subject;
                            //    mailMessage.BodyEncoding = System.Text.Encoding.UTF8;
                            //    mailMessage.Priority = MailPriority.High;
                            //    mailMessage.IsBodyHtml = true;

                            //    if (email == "" || email == null)
                            //    {

                            //    }
                            //    else
                            //    {
                            //        mailMessage.To.Add(new MailAddress(email));
                            //    }
                            //    SmtpClient smtp = new SmtpClient();

                            //    smtp.Host = company_email_details.Rows[0]["Host"].ToString();

                            //    smtp.EnableSsl = true;
                            //    System.Net.NetworkCredential NetworkCred = new System.Net.NetworkCredential(from, password);
                            //    smtp.UseDefaultCredentials = true;
                            //    smtp.Credentials = NetworkCred;
                            //    smtp.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
                            //    smtp.Port = Convert.ToInt32(company_email_details.Rows[0]["Port"].ToString());
                            //    try
                            //    {
                            //        smtp.Send(mailMessage);
                            //        CompanyInfo.InsertActivityLogDetails("App- Bank Account payment email sent successfully. ", c.User_ID, c.Transaction_ID, c.User_ID, c.Customer_ID, "Upload_Receipt", c.CB_ID, c.Client_ID, "View Transfer");
                            //    }
                            //    catch (Exception ex)
                            //    {
                            //        CompanyInfo.InsertActivityLogDetails("App- Email sending failed. " + ex.ToString() + " ", c.User_ID, c.Transaction_ID, c.User_ID, c.Customer_ID, "Upload_Receipt", c.CB_ID, c.Client_ID, "View Transfer");
                            //    }
                            //}
                        }


                    }
                    catch (Exception ex)
                    {
                        throw;
                        //  string stattus = (string)mtsmethods.InsertErrorLogDetails(ex.Message, user_id, "Send_Custom_Email_By_Template", branch_id, client_id);
                    }
                }
            }
            catch (Exception ex)
            {
                ds.Rows.Add(0, ex.ToString());
                //string stattus = (string)CompanyInfo.InsertErrorLogDetails(ex.Message, user_id, "Send_Cusstom_Email_By_Template", branch_id, client_id);
            }
            return ds;
        }

        public DataTable SelectBasecurrency(Model.Transaction obj, HttpContext context)
        {
            MySqlConnector.MySqlCommand _cmdbC = new MySqlConnector.MySqlCommand("SelectBankDetails_Settings");
            _cmdbC.CommandType = CommandType.StoredProcedure;
            _cmdbC.Parameters.AddWithValue("_clientID", obj.Client_ID);
            _cmdbC.Parameters.AddWithValue("_flag", "baseCurrency");
            DataTable dt = db_connection.ExecuteQueryDataTableProcedure(_cmdbC);
            Int32 activity1 = dt.Rows.Count;

            _ = Task.Run(() => CompanyInfo.InsertActivityLogDetails("App - SelectBankDetails. " + activity1 + " ", 0, 0, 0, 0, "bankdetails", 2, 1, "View Transfer", context));
            return dt;
        }// Anushka

        public DataTable SelectDeliveryType(Model.Transaction obj, HttpContext context)
        {
            MySqlConnector.MySqlCommand _cmddT = new MySqlConnector.MySqlCommand("SelectBankDetails_Settings");
            _cmddT.CommandType = CommandType.StoredProcedure;
            _cmddT.Parameters.AddWithValue("_clientID", obj.Client_ID);
            _cmddT.Parameters.AddWithValue("_flag", "deliveryType");
            DataTable dt = db_connection.ExecuteQueryDataTableProcedure(_cmddT);
            Int32 activity1 = dt.Rows.Count;

            _ = Task.Run(() => CompanyInfo.InsertActivityLogDetails("App - SelectBankDetails. " + activity1 + " ", 0, 0, 0, 0, "bankdetails", 2, 1, "View Transfer", context));
            return dt;
        }// Anushka

        public DataTable SelectcollectionType(Model.Transaction obj, HttpContext context)
        {
            MySqlConnector.MySqlCommand _cmdcT = new MySqlConnector.MySqlCommand("SelectBankDetails_Settings");
            _cmdcT.CommandType = CommandType.StoredProcedure;
            _cmdcT.Parameters.AddWithValue("_clientID", obj.Client_ID);
            _cmdcT.Parameters.AddWithValue("_flag", "collectionType");
            DataTable dt = db_connection.ExecuteQueryDataTableProcedure(_cmdcT);

            Int32 activity1 = dt.Rows.Count;

            _ = Task.Run(() => CompanyInfo.InsertActivityLogDetails("App - SelectBankDetails. " + activity1 + " ", 0, 0, 0, 0, "bankdetails", 2, 1, "View Transfer", context));

            return dt;
        }// Anushka
        public DataTable BindReceiptConfiguration(Model.Transaction obj)//Digvijay chnages for get receipt configuration for footernote add
        {

            mts_connection _MTS = new mts_connection();
            DataTable dt = new DataTable();
            using (MySqlConnection con = new MySqlConnection(_MTS.WebConnSetting()))
            {
                try
                {
                    /*using (MySqlCommand _cmd = new MySqlCommand("Get_ReceiptConfiguration", con))
                    {
                        _cmd.CommandType = CommandType.StoredProcedure;
                        _cmd.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                        if (con.State != System.Data.ConnectionState.Open)
                        {
                            con.Open();
                        }
                        dt = db_connection.ExecuteQueryDataTableProcedure(_cmd);

                    }*/

                    MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("Get_ReceiptConfiguration");
                    _cmd.CommandType = CommandType.StoredProcedure;
                    _cmd.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                    dt = db_connection.ExecuteQueryDataTableProcedure(_cmd);

                }
                catch (Exception _x)
                {
                    CompanyInfo.InsertErrorLogDetails(_x.ToString(), 0, "BindReceiptConfiguration", obj.Branch_ID, obj.Client_ID);
                }
                finally
                {
                    if (con.State == System.Data.ConnectionState.Open)
                    {
                        con.Close();
                    }
                }

                return dt;
            }
        }

    }
}
