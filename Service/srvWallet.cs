using System.Data;
using System.Net;
using MySqlConnector;
using RestSharp;

namespace Calyx_Solutions.Service
{
    public class srvWallet
    {
        HttpContext context = null;
       
        public DataTable add_customer_email(Model.Transaction obj)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("msg");
            try
            {
                int Customer_ID = Convert.ToInt32(CompanyInfo.Decrypt(obj.Customer_ID, true));
                List<Model.PaymentType> _lst = new List<Model.PaymentType>();
                MySqlCommand _cmd = new MySqlCommand("sp_add_customer_email");
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                _cmd.Parameters.AddWithValue("_Customer_ID", Customer_ID);
                _cmd.Parameters.AddWithValue("_email", obj.CustomerEmail);
                _cmd.Parameters.Add(new MySqlParameter("_email_already_exists", MySqlDbType.Int32));
                _cmd.Parameters["_email_already_exists"].Direction = ParameterDirection.Output;

                int _email_already_exists = db_connection.ExecuteNonQueryProcedure(_cmd);
                if (_email_already_exists == 0)
                {
                    dt.Rows.Add("already_exists");
                }
                else if (_email_already_exists == 1)
                {
                    dt.Rows.Add("success");

                    try
                    {
                        DataTable dt_notif = CompanyInfo.set_notification_data(66);
                        
                        if (dt_notif.Rows.Count > 0)
                        {
                            int SMS = Convert.ToInt32(dt_notif.Rows[0]["SMS"]);
                            int Email = Convert.ToInt32(dt_notif.Rows[0]["Email"]);
                            int NotificationStatus = Convert.ToInt32(dt_notif.Rows[0]["Notification"]);
                            string NotificationMessage = Convert.ToString(dt_notif.Rows[0]["notification_msg"]);
                            if (NotificationMessage.Contains("[Email_Id]") == true)
                            {
                                NotificationMessage = NotificationMessage.Replace("[Email_Id]", obj.CustomerEmail);
                            }

                            int i = CompanyInfo.check_notification_perm(Convert.ToString(Customer_ID), obj.Client_ID, obj.Branch_ID, 1, 66, Convert.ToDateTime(DateTime.Now), 1, SMS, Email, NotificationStatus, "App - linked emails to Wallet Notification - 66", NotificationMessage,context);
                        }

                    }
                    catch (Exception ex)
                    {
                        Model.ErrorLog objError = new Model.ErrorLog();
                        objError.User = new Model.User();
                        objError.Error = "App_linked_emails_to_Wallet_Notification : " + ex.ToString();
                        objError.Date = DateTime.Now;
                        objError.User_ID = 1;
                        objError.Client_ID = 1;
                        objError.Function_Name = "App_linked_emails_to_Wallet_Notification";
                        Service.srvErrorLog srvError = new Service.srvErrorLog();
                        srvError.Create(objError, context);
                    }
                    try
                    {
                        //Digvijay changes - mial send for mail linked for this customer.
                        int client_id = obj.Client_ID;
                        int branch_id = obj.Branch_ID;
                        //Company details
                        DataTable dtc = CompanyInfo.get(obj.Client_ID, context);
                        string company_name = Convert.ToString(dtc.Rows[0]["Company_Name"]);
                        string URL = Convert.ToString(dtc.Rows[0]["Company_URL_Admin"]);
                        string cust_url = Convert.ToString(dtc.Rows[0]["Company_URL_Customer"]);

                        //Customer details
                        MySqlCommand cmd3 = new MySqlCommand("customer_details_by_param");
                        cmd3.CommandType = CommandType.StoredProcedure;
                        string _whereclause = " and cr.Client_ID=" + obj.Client_ID;
                        if (Customer_ID > 0)
                        {
                            _whereclause = " and cr.Customer_ID=" + Customer_ID;
                        }
                        cmd3.Parameters.AddWithValue("_whereclause", _whereclause);
                        cmd3.Parameters.AddWithValue("_SecurityKey", CompanyInfo.SecurityKey());
                        DataTable dt_cust = db_connection.ExecuteQueryDataTableProcedure(cmd3);

                        string full_name = dt_cust.Rows[0]["Full_name"].ToString();
                        string email_id = dt_cust.Rows[0]["Email_ID"].ToString();
                        string referno = Convert.ToString(dt_cust.Rows[0]["WireTransfer_ReferanceNo"]);
                        string link = "";


                        string sendmsg = "Your Email linked successfully updated.";
                        string EmailID = obj.Customer.Email;
                        string subject = string.Empty;
                        string body = string.Empty;

                        HttpWebRequest httpRequest;

                        httpRequest = (HttpWebRequest)WebRequest.Create(URL + "Email/linked-email-to-wallet.html");

                        httpRequest.UserAgent = "Code Sample Web Client";
                        HttpWebResponse webResponse = (HttpWebResponse)httpRequest.GetResponse();
                        using (StreamReader reader = new StreamReader(webResponse.GetResponseStream()))
                        {
                            body = reader.ReadToEnd();
                        }
                        subject = "Your email Is Now Linked to Your Wallet";
                        body = body.Replace("[name]", full_name);
                        body = body.Replace("[Email_ID]", EmailID);


                        CompanyInfo.Send_Mail(dtc, email_id, body, subject, client_id, branch_id, "", "", "", context);
                    }
                    catch (Exception ex)
                    {
                        Model.ErrorLog objError = new Model.ErrorLog();
                        objError.User = new Model.User();
                        objError.Error = "Email_Linked_to_Wallet : " + ex.ToString();
                        objError.Date = DateTime.Now;
                        objError.User_ID = 1;
                        objError.Client_ID = 1;
                        objError.Function_Name = "Email_Linked_to_Wallet";
                        Service.srvErrorLog srvError = new Service.srvErrorLog();
                        srvError.Create(objError, context);
                    }//End
                }
                else
                {
                    dt.Rows.Add("error");
                }

                return dt;
            }
            catch (Exception ex)
            {
                Model.ErrorLog objError = new Model.ErrorLog();
                objError.User = new Model.User();
                objError.Error = "Get_linked_emails : " + ex.ToString();
                objError.Date = DateTime.Now;
                objError.User_ID = 1;
                objError.Client_ID = 1;
                objError.Function_Name = "Get_linked_emails";
                Service.srvErrorLog srvError = new Service.srvErrorLog();
                srvError.Create(objError, context);
                dt.Rows.Add("exception");
                return dt;
            }
        }


        public DataTable delete_customer_email(Model.Transaction obj)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("msg");
            try
            {
                int Customer_ID = Convert.ToInt32(CompanyInfo.Decrypt(obj.Customer_ID, true));
                List<Model.PaymentType> _lst = new List<Model.PaymentType>();
                MySqlCommand _cmd = new MySqlCommand("sp_delete_customer_email");
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                _cmd.Parameters.AddWithValue("_Customer_ID", Customer_ID);
                _cmd.Parameters.AddWithValue("_email", obj.CustomerEmail);
                int msg = db_connection.ExecuteNonQueryProcedure(_cmd);
                if (msg == 0)
                {
                    dt.Rows.Add("error");
                }
                else
                {
                    dt.Rows.Add("deleted");
                }

                return dt;
            }
            catch (Exception ex)
            {
                Model.ErrorLog objError = new Model.ErrorLog();
                objError.User = new Model.User();
                objError.Error = "delete_customer_email : " + ex.ToString();
                objError.Date = DateTime.Now;
                objError.User_ID = 1;
                objError.Client_ID = 1;
                objError.Function_Name = "delete_customer_email";
                Service.srvErrorLog srvError = new Service.srvErrorLog();
                srvError.Create(objError, context);
                dt.Rows.Add("exception");
                return dt;
            }
        }
        public DataTable Get_linked_emails(Model.Transaction obj)
        {
            DataTable dt = new DataTable();
            try
            {
                int Customer_ID = Convert.ToInt32(CompanyInfo.Decrypt(obj.Customer_ID, true));
                List<Model.PaymentType> _lst = new List<Model.PaymentType>();
                MySqlCommand _cmd = new MySqlCommand("SP_Get_linked_emails");
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                _cmd.Parameters.AddWithValue("_Customer_ID", Customer_ID);
                _cmd.Parameters.AddWithValue("_where", "");
                dt = db_connection.ExecuteQueryDataTableProcedure(_cmd);
                return dt;
            }
            catch (Exception ex)
            {
                Model.ErrorLog objError = new Model.ErrorLog();
                objError.User = new Model.User();
                objError.Error = "Get_linked_emails : " + ex.ToString();
                objError.Date = DateTime.Now;
                objError.User_ID = 1;
                objError.Client_ID = 1;
                objError.Function_Name = "Get_linked_emails";
                Service.srvErrorLog srvError = new Service.srvErrorLog();
                srvError.Create(objError, context);
                return dt;
            }
        }

        public DataTable Get_Wallets(Model.Transaction obj)
        {
            int Customer_ID = Convert.ToInt32(CompanyInfo.Decrypt(obj.Customer_ID, true));
            List<Model.PaymentType> _lst = new List<Model.PaymentType>();
           MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("GetWallets");
            _cmd.CommandType = CommandType.StoredProcedure;
            string whereclause = " and w.AgentFlag=1 and c.Client_ID=" + obj.Client_ID + " and w.Client_ID=" + obj.Client_ID + " and w.Customer_ID=" + Customer_ID + " and Currency_Code like '%" + obj.Currency_Code + "%'";
            _cmd.Parameters.AddWithValue("_whereclause", whereclause);
            DataTable dt = db_connection.ExecuteQueryDataTableProcedure(_cmd);

            

            return dt;
        }

        public DataTable Get_WalletDetails(Model.Transaction obj)
        {
            int Customer_ID = Convert.ToInt32(CompanyInfo.Decrypt(obj.Customer_ID, true));
            List<Model.PaymentType> _lst = new List<Model.PaymentType>();
            MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("SP_GetWalletDetails");
            _cmd.CommandType = CommandType.StoredProcedure;
            _cmd.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
            _cmd.Parameters.AddWithValue("_Customer_ID", Customer_ID);
            string where = "";
            if (obj.Currency_Code != null && obj.Currency_Code != "")
            {
                where = " and currency_master.Currency_Code like '%" + obj.Currency_Code + "%'";
            }
            _cmd.Parameters.AddWithValue("_where", " and wallet_table.AgentFlag=1" + where);
            DataTable dt = db_connection.ExecuteQueryDataTableProcedure(_cmd);
            return dt;
        }

        public DataTable Get_WalletDetails_CustomerProfile(Model.Transaction obj)
        {
            int Customer_ID = Convert.ToInt32(CompanyInfo.Decrypt(obj.Customer_ID, true));
            int count = obj.count;
            string count_where = "";
            if (obj.count == 5)
            {
                count_where = " limit 5 ";
            }
            List<Model.PaymentType> _lst = new List<Model.PaymentType>();
            MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("Display_WalletDetails_customerProfile");
            _cmd.CommandType = CommandType.StoredProcedure;
            string whereclause = " and wt.AgentFlag=1 and wtt.Client_ID=" + obj.Client_ID + " and wt.Customer_ID= " + Customer_ID;
            //  string whereclause = " and c.Client_ID=" + obj.Client_ID + " and w.Client_ID=" + obj.Client_ID + " and w.Customer_ID=" + obj.Customer_ID + " and Currency_Code like '%" + obj.Currency_Code + "%'";
            _cmd.Parameters.AddWithValue("_whereclause", whereclause);
            _cmd.Parameters.AddWithValue("_count_where", count_where);
            DataTable dt = db_connection.ExecuteQueryDataTableProcedure(_cmd);
            return dt;
        }


        public string GenerateReferenceNo(int Client_ID, int CB_ID)
        {
            string refNo1 = "";
            try
            {

                DataTable dt1 = CompanyInfo.get(Client_ID,context);
                MySqlCommand cmd = new MySqlCommand("getTxnRef_InitialChar");
                cmd.CommandType = CommandType.StoredProcedure;
                int size = Convert.ToInt32(dt1.Rows[0]["trn_ref_no_length"].ToString());
                var rng = new Random(Environment.TickCount);
                cmd.Parameters.AddWithValue("_Client_ID", Client_ID);
                cmd.Parameters.AddWithValue("_CB_ID", CB_ID);
                string initialchars = Convert.ToString(db_connection.ExecuteScalarProcedure(cmd));
                var refNo = initialchars + string.Concat(Enumerable.Range(0, size).Select((index) => rng.Next(10).ToString()));
                refNo1 = Convert.ToString(refNo);
                string ReferenceNo = refNo1;
                MySqlCommand cmd1 = new MySqlCommand("CheckDuplicateRefNo");
                cmd1.CommandType = CommandType.StoredProcedure;
                cmd1.Parameters.AddWithValue("_Client_ID", Client_ID);
                cmd1.Parameters.AddWithValue("_ReferenceNo", ReferenceNo);
                DataTable dt = (db_connection.ExecuteQueryDataTableProcedure(cmd1));
                if (dt.Rows.Count > 0)
                {
                    refNo1 = null;
                    GenerateReferenceNo(Client_ID, CB_ID);
                }
                else { }

            }
            catch { }
            return refNo1;
        }
        public string DCBankToken(DataTable dtt)
        {
            DCBank json = null;
            try
            {
                int api_id = 0; string apiurl = "", apiuser = "", apipass = "", accesscode = "";
                string host = "";
                if (dtt.Rows.Count > 0)
                {
                    api_id = Convert.ToInt32(dtt.Rows[0]["bank_api_id"]);
                    apiurl = Convert.ToString(dtt.Rows[0]["API_URL"]);
                    apiuser = Convert.ToString(dtt.Rows[0]["UserName"]);
                    apipass = Convert.ToString(dtt.Rows[0]["Password"]);
                    //accesscode = Convert.ToString(dtt.Rows[0]["ProfileID"]);//unique code
                    //string API_Codes = Convert.ToString(dtt.Rows[0]["APIUnique_Codes"]);
                    //Newtonsoft.Json.Linq.JObject o = Newtonsoft.Json.Linq.JObject.Parse(API_Codes);
                    //host = Convert.ToString(o["host"]);
                }
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
                       | SecurityProtocolType.Tls11
                       | SecurityProtocolType.Tls12
                       | SecurityProtocolType.Ssl3;
                //var client = new HttpClient();
                //var request = new HttpRequestMessage(HttpMethod.Post, apiurl + "Authentication/Login");//https://mrcapisandbox.dcbank.ca/integrationapi/v1.0/Authentication/Login
                //var content = new StringContent("{\n    \"UserName\": \"okwy@kobotrade.com\",\n    \"Password\": \"Jan2023!!\"\n}", null, "application/json");
                //request.Content = content;
                //var response = await client.SendAsync(request);
                //response.EnsureSuccessStatusCode();
                //Console.WriteLine(await response.Content.ReadAsStringAsync());
                var client = new RestClient(apiurl + "Authentication/Login");
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                //request.AddHeader("Host", host);
                //request.AddHeader("Content-Length", "69");
                //request.AddHeader("Cache-Control", "no-cache");
                //request.AddHeader("Postman-Token", "91957468-09b6-4ba5-b063-d7ebfc6123b5");
                //request.AddParameter("grant_type", "client_credentials");
                //request.AddParameter("UserName", apiuser);
                //request.AddParameter("Password", apipass);
                var body = @"{" + "\n" +
                @"    ""UserName"": """ + apiuser + "\"," + "\n" +
                @"    ""Password"": """ + apipass + "\"\n" +
                @"}";
                CompanyInfo.InsertActivityLogDetails("Dc bank token request " + body + "", 0, 0, 0, 0, "Generate Reference", 1, 1, "DC bank", context);
                request.AddParameter("application/json", body, ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);
                CompanyInfo.InsertActivityLogDetails("Dc bank token request " + response.Content + "", 0, 0, 0, 0, "Generate Reference", 1, 1, "DC bank", context);
                //Console.WriteLine(response.Content);
                //mtsmethods.InsertActivityLogDetails("MyZeePay parameters: " + response + " Content:" + response.Content + "", t.User_ID, t.Transaction_ID, t.User_ID, t.Customer_ID, "Proceed Transaction", t.CB_ID, t.Client_ID);
                json = Newtonsoft.Json.JsonConvert.DeserializeObject<DCBank>(response.Content);
            }
            catch (Exception ex)
            {
                Model.ErrorLog objError = new Model.ErrorLog();
                objError.User = new Model.User();
                objError.Error = "DCBankToken : Token --" + ex.ToString();
                objError.Date = DateTime.Now;
                objError.User_ID = 1;
                objError.Client_ID = 1;
                objError.Function_Name = "DCBankToken";
                Service.srvErrorLog srvError = new Service.srvErrorLog();
                srvError.Create(objError, context);
                json.DCBankApiAccessToken = "";
            }
            return json.DCBankApiAccessToken;
        }
        public string DCBankApi(Model.Transaction obj, DataTable dtwallets)
        {
            string URL = "";
            try
            {
                MySqlCommand cmd = new MySqlCommand("Get_instantBankAPIDetails");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("_API_ID", 2);//Click Send API ID
                cmd.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                cmd.Parameters.AddWithValue("_status", 0);// API Status
                DataTable dtt = db_connection.ExecuteQueryDataTableProcedure(cmd);
                int api_id = 0; string apiurl = "", apiuser = "", apipass = "", accesscode = "", host = "", destinationAccount = "";
                int Customer_ID = Convert.ToInt32(CompanyInfo.Decrypt(obj.Customer_ID, true));
                if (dtt.Rows.Count > 0)
                {
                    api_id = Convert.ToInt32(dtt.Rows[0]["bank_api_id"]);
                    apiurl = Convert.ToString(dtt.Rows[0]["API_URL"]);
                    apiuser = Convert.ToString(dtt.Rows[0]["UserName"]);
                    apipass = Convert.ToString(dtt.Rows[0]["Password"]);
                    //accesscode = Convert.ToString(dtt.Rows[0]["ProfileID"]);//unique code
                    string API_Codes = Convert.ToString(dtt.Rows[0]["APIUnique_Codes"]);
                    //Newtonsoft.Json.Linq.JObject o = Newtonsoft.Json.Linq.JObject.Parse(API_Codes);
                    //host = Convert.ToString(o["host"]);
                    //destinationAccount = Convert.ToString(o["destinationAccount"]);
                }
                double transfer_cost = obj.TotalAmount;
                if (obj.Wallet_Perm != null && obj.Wallet_Perm != -1)
                {
                    if (Convert.ToString(obj.Wallet_Perm) == "0")
                    {
                        if (Convert.ToString(obj.Transfer_Cost) != "" && Convert.ToString(obj.Transfer_Cost) != null)
                        {
                            transfer_cost = obj.Transfer_Cost;
                        }
                    }
                }

                if (obj.Discount_Perm != null && obj.Discount_Perm != -1)
                {
                    if (Convert.ToString(obj.Discount_Perm) == "0")
                    {
                        if (Convert.ToString(obj.Transfer_Cost) != "" && Convert.ToString(obj.Transfer_Cost) != null)
                        {
                            transfer_cost = obj.Transfer_Cost;
                        }
                    }
                }
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
                       | SecurityProtocolType.Tls11
                       | SecurityProtocolType.Tls12
                       | SecurityProtocolType.Ssl3;
                string token = DCBankToken(dtt);
                //var client = new RestClient(apiurl + "v1/payments");
                var client = new RestClient(apiurl + "Etransfer/CreateEtransferRequestMoney");
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                request.AddHeader("Authorization", "Bearer " + token);
                request.AddHeader("Content-Type", "application/json");
                //@"     ""merchantId"": "+merchant_id+"," + "\n" + 
                //@"     ""amount"": " + obj.AmountInGBP.ToString("0.00") + "," + "\n" +
                //@"     ""destinationAccount"": " + destinationAccount + "," + "\n" +
                //@"     ""customerReference"": ""Ticket" + obj.ReferenceNo + "," + "\n" +
                //@"     ""callbackUrl"": ""https://currencyexchangesoftware.eu/""," + "\n" +
                string ref_no = obj.ReferenceNo;
                var body = @"{
            " + "\n" +
                    @"  ""customerName"": """ + dtwallets.Rows[0]["First_Name"] + " " + dtwallets.Rows[0]["Last_Name"] + @""",
            " + "\n" +
                    @"  ""description"": ""Money Requester"",
            " + "\n" +
                    @"  ""amount"": """ + obj.Wallet_Amount + @""",
            " + "\n" +
                    @"  ""financialAccountId"": 0,
            " + "\n" +
                    @"  ""notificationType"": 2,
            " + "\n" +
                    @"  ""editableFulfillAmount"": false,
            " + "\n" +
                    @"  ""clientReferenceNumber"": """ + ref_no + @"""
            " + "\n" +
                    @"}";
                obj.Worldpay_Response = body;
                obj.Delete_Status = 0;
                SaveRequestResponse(obj);
                CompanyInfo.InsertActivityLogDetails("App - Payvyne Request." + body, obj.User_ID, obj.Transaction_ID, obj.User_ID, Customer_ID, "Send-Insert Transfer" + obj.ReferenceNo, obj.CB_ID, obj.Client_ID, "PayvynePaymentRedirect", context);
                request.AddParameter("application/json", body, ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);
                DCBank json = Newtonsoft.Json.JsonConvert.DeserializeObject<DCBank>(response.Content);
                CompanyInfo.InsertActivityLogDetails("App - Payvyne Response." + response.Content, obj.User_ID, obj.Transaction_ID, obj.User_ID, Customer_ID, "Send-Insert Transfer" + obj.ReferenceNo, obj.CB_ID, obj.Client_ID, "PayvynePaymentRedirect", context);
                obj.Worldpay_Response = response.Content;
                obj.Delete_Status = 1;
                SaveRequestResponse(obj);
                obj.Delete_Status = 0;
                URL = json.Item.GateWayUrl;
                return URL;
            }
            catch (Exception ex)
            {
                Model.ErrorLog objError = new Model.ErrorLog();
                objError.User = new Model.User();
                objError.Error = "DCBankApi : Api --" + ex.ToString();
                objError.Date = DateTime.Now;
                objError.User_ID = 1;
                objError.Client_ID = 1;
                objError.Function_Name = "DCBankApi";
                Service.srvErrorLog srvError = new Service.srvErrorLog();
                srvError.Create(objError, context);
                return URL;
            }
        }
        public int SaveRequestResponse(Model.Transaction t)
        {
            int Customer_ID = Convert.ToInt32(CompanyInfo.Decrypt(t.Customer_ID, true));
            t.Record_Insert_DateTime = CompanyInfo.gettime(t.Client_ID, Customer_ID.ToString(), t.Country_ID, context);
            MySqlCommand cmd1 = new MySqlCommand("Insert_RequestResponse");
            cmd1.CommandType = CommandType.StoredProcedure;
            cmd1.Parameters.AddWithValue("_Customer_ID", Customer_ID);
            cmd1.Parameters.AddWithValue("_ReferenceNo", t.ReferenceNo);
            cmd1.Parameters.AddWithValue("_Remark", t.Worldpay_Response);
            cmd1.Parameters.AddWithValue("_Client_ID", t.Client_ID);
            cmd1.Parameters.AddWithValue("_status", t.Delete_Status);// 0 for request and 1 for response
            cmd1.Parameters.AddWithValue("_CB_ID", t.CB_ID);
            cmd1.Parameters.AddWithValue("_Record_Insert_DateTime", t.Record_Insert_DateTime);
            int msg = db_connection.ExecuteNonQueryProcedure(cmd1);
            return msg;
        }

        public DataTable GetRewardAmountCustomerprofile(Model.Transaction obj)
        {
            int Customer_ID = Convert.ToInt32(CompanyInfo.Decrypt(obj.Customer_ID, true));
            List<Model.PaymentType> _lst = new List<Model.PaymentType>();
            MySqlCommand _cmd = new MySqlCommand("Show_reward_customerprofile");
            _cmd.CommandType = CommandType.StoredProcedure;
            _cmd.Parameters.AddWithValue("_Customer_ID", Customer_ID);
            _cmd.Parameters.AddWithValue("_client_id", obj.Client_ID);
            DataTable dt = db_connection.ExecuteQueryDataTableProcedure(_cmd);
            return dt;
        }

        #region DCBank


        public class DCBank
        {
            public string DCBankApiAccessToken { get; set; }
            public string ErrorCode { get; set; }
            public string ErrorDescription { get; set; }
            public string StackTrace { get; set; }
            public string LastLoginDateTime { get; set; }
            public string ExpireAfterMinutes { get; set; }
            public string IsSucceeded { get; set; }
            //public string[] ErrorList { get; set; }
            //public string[] ParameterList { get; set; }
            public string PaymentType { get; set; }
            public Item Item { get; set; }
        }

        public class Item
        {
            public string TransactionId { get; set; }
            public string TransactionEtransferId { get; set; }
            public string TransactionReferenceNumber { get; set; }
            public string ETransferReferenceNumber { get; set; }
            public string GateWayUrl { get; set; }

        }
        #endregion

    }
}
