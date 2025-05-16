using Calyx_Solutions.Model;
using Calyx_Solutions.Service;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Text.Json;

using static Google.Apis.Requests.BatchRequest;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Calyx_Solutions.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        [HttpPost]
        [Authorize]
        [Route("viewtransfer")]
        public IActionResult ViewTransfer([FromBody] JsonElement obj)
        {
            HttpContext context = HttpContext;
            string json = System.Text.Json.JsonSerializer.Serialize(obj);
            dynamic data = JObject.Parse(json);
            CompanyInfo.InsertrequestLogTracker("viewtransfer full request body: " + JObject.Parse(json), 0, 0, 0, 0, "ViewTransfer", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
            var validateJsonData = (dynamic)null;
            Model.response.WebResponse response = null;
            Transaction obj1 = new Transaction();
            // data used for validation purpose

            try
            {
                if (!SqlInjectionProtector.ReadJsonElementValues(obj) || !SqlInjectionProtector.ReadJsonElementValuesScript(obj))
                {
                    var errorResponse = new { response = false, responseCode = "02", data = "Invalid input detected." };
                    return new JsonResult(errorResponse) { StatusCode = 400 };
                }

                int number;
                if (!int.TryParse(Convert.ToString(data.clientID), out number))
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "Invalid Client Id." };
                    return new JsonResult(validateJsonData);
                }
                if (!int.TryParse(Convert.ToString(data.branchID), out number))
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "Invalid Branch Id." };
                    return new JsonResult(validateJsonData);
                }
                if (data.customerID == "" || data.customerID == null)
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "Invalid Customer ID." };
                    return new JsonResult(validateJsonData);
                }
                if (data.limitFlag == "" || data.limitFlag == null)
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "Invalid partypayFlag." };
                    return new JsonResult(validateJsonData);
                }


                obj1.Client_ID = data.clientID;
                obj1.Customer_ID = data.customerID;
                obj1.Branch_ID = data.branchID;
                obj1.PartPay_Flag = data.limitFlag;
                obj1.offset_flag = data.offsetFlag;

                if (data.transactionStatusID != 0 && data.transactionStatusID != "" && data.transactionStatusID != null && data.transactionStatusID != "null")
                {
                    obj1.TransactionStatus_ID = data.transactionStatusID;
                }

                if (data.transactionID != 0 && data.transactionID != "" && data.transactionID != null && data.transactionID != "null")
                {
                    obj1.Transaction_ID = data.transactionID;
                }
                int Customer_ID = Convert.ToInt32(CompanyInfo.Decrypt(Convert.ToString(obj1.Customer_ID), true));
                List<Model.Transaction> _lst = new List<Model.Transaction>();

                DataTable dt = new DataTable();

                MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("Transaction_Search");
                _cmd.CommandType = CommandType.StoredProcedure;
                string _query = " ";
                if (obj1.Beneficiary_ID > 0)
                {
                    _query = _query + " and aa.Beneficiary_ID=" + obj1.Beneficiary_ID + "";
                }
                if (obj1.Customer != null)
                {
                    if (obj1.Customer.WireTransfer_ReferanceNo != "" && obj1.Customer.WireTransfer_ReferanceNo != null)
                    {
                        _query = _query + " and WireTransfer_ReferanceNo  LIKE '%" + obj1.Customer.WireTransfer_ReferanceNo + "%'";
                    }
                    if (obj1.Customer.Full_Name != "" && obj1.Customer.Full_Name != null)
                    {
                        _query = _query + " and concat(cc.first_Name,' ',cc.Last_Name) LIKE '%" + obj1.Customer.Full_Name + "%'";
                    }
                }

                if (obj1.Transaction_ID > 0)
                {
                    _query += " and aa.Transaction_ID=" + obj1.Transaction_ID + "";
                }
                if (Customer_ID > 0)
                {
                    _query += " and  aa.customer_Id='" + Customer_ID + "'";
                }
                if (obj1.TransactionStatus_ID != 0)
                {
                    _query += "and aa.TransactionStatus_ID =" + obj1.TransactionStatus_ID + "";
                }
                if (obj1.ReferenceNo != "" && obj1.ReferenceNo != null)
                {
                    _query += " and ReferenceNo like '%" + obj1.ReferenceNo + "%'";
                }
                if (obj1.Beneficiary_Name != "" && obj1.Beneficiary_Name != null)
                {
                    _query += " and beneficiary_name  like '%" + obj1.Beneficiary_Name.Replace("'", "''") + "%'";
                }
                if (obj1.FromDate != null && obj1.ToDate != null && obj1.FromDate != "" && obj1.ToDate != "")
                {
                    DateTime txtdate = DateTime.ParseExact(obj1.FromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    DateTime txtdate1 = DateTime.ParseExact(obj1.ToDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    _query += "  and  Date(aa.Record_Insert_DateTime) between '" + txtdate.ToString("yyyy-MM-dd") + "' and '" + txtdate1.ToString("yyyy-MM-dd") + "' ";
                }
                if (obj1.Country_ID > 0)
                {
                    _query += " and aa.Country_ID=" + obj1.Country_ID;
                }
                string _limit = " limit 1000"; int _limit2 = 0;
                if (obj1.Transaction_ID == 0 || obj1.Transaction_ID.ToString() == "")
                {
                    if (obj1.PartPay_Flag > 0)
                    {
                        _limit = " limit " + obj1.PartPay_Flag + " offset " + obj1.offset_flag;

                        _limit2 = obj1.offset_flag + obj1.PartPay_Flag;

                    }
                }

                List<Dictionary<string, object>> Transaction_count = new List<Dictionary<string, object>>();
                _cmd.Parameters.AddWithValue("_whereclause", _query);
                _cmd.Parameters.AddWithValue("_limit", _limit);
                dt = db_connection.ExecuteQueryDataTableProcedure(_cmd);
                if (dt.Rows.Count > 0)//amruta
                {


                    foreach (DataRow row in dt.Rows)
                    {
                        if (row["Document_Name"] != DBNull.Value && row["Document_Name"].ToString() != "")
                        {
                            DataTable dtc = CompanyInfo.get(obj1.Client_ID, context);
                            string image_link = (dtc.Rows[0]["Company_URL_Admin"]).ToString() + row["Document_Name"].ToString();
                            string base64str = CompanyInfo.ConvertImageLinkToBase64(image_link);
                            row["Document_Name"] = base64str;
                        }
                    }
                }
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        try
                        {
                            Dictionary<string, object> Transactioncount = new Dictionary<string, object>();
                            var TStatus_ID = dr["TransactionStatus_ID"].ToString();
                            var pstatus = dr["paymentReceived_ID"].ToString();
                            var PtypeID = dr["PaymentType_ID"].ToString();
                            var madeTransFlag = dr["MadeThisTransfer_Flag"].ToString();
                            Transactioncount["apiID"] = dr["API_ID"];
                            string localAgentAddressDetails = null;
                            try
                            {
                                
                                if (Convert.ToInt64(dr["API_ID"]) == 0 && Convert.ToInt64(dr["CollectionPoint_ID"]) != 0)
                                {
                                    MySqlConnector.MySqlCommand cmd = new MySqlConnector.MySqlCommand("CollectionPoint_Search");
                                    cmd.CommandType = CommandType.StoredProcedure;
                                    string whereclause = " and ID = " + Convert.ToInt64(dr["CollectionPoint_ID"]) + "";
                                    cmd.Parameters.AddWithValue("_whereclause", whereclause);
                                    DataTable dtLocalAgent = db_connection.ExecuteQueryDataTableProcedure(cmd);
                                   
                                        foreach (DataRow drLocalAgent in dtLocalAgent.Rows)
                                        {

                                            localAgentAddressDetails = Convert.ToString(drLocalAgent["Address"]);
                                            if (Convert.ToString(drLocalAgent["Phone_No"])  != "" && Convert.ToString(drLocalAgent["Phone_No"]) != null)
                                            {
                                                localAgentAddressDetails += ", Phone:" + drLocalAgent["Phone_No"];
                                            }
                                            if (Convert.ToString(drLocalAgent["Mobile_No"])  != "" && Convert.ToString(drLocalAgent["Mobile_No"])  != null)
                                            {
                                                localAgentAddressDetails += ", Mobile:" + drLocalAgent["Mobile_No"];
                                            }
                                        }
                                  
                                }
                            }
                            catch { }
                            //extraFees added on 15/05/2025 by parth for showing in response, also done changes (aa.Extra_fees) in sp --> Transaction_Search
                            try { Transactioncount["extraFees"] = dr["Extra_fees"].ToString(); }
                            catch (Exception ex)
                            {
                                _ = Task.Run(() => CompanyInfo.InsertErrorLogTracker(ex.ToString(), 0, 0, 0, 0, "ViewTransfer --> added extraFees in response exception", Convert.ToInt32(obj1.Branch_ID), Convert.ToInt32(obj1.Client_ID), "", context));
                            }
                            Transactioncount["apiTransactionID"] = dr["APITransaction_ID"];
                            Transactioncount["apiBranchDetails"] = dr["API_BranchDetails"]+" " +localAgentAddressDetails;
                            Transactioncount["reciever"] = dr["Reciever"];
                            Transactioncount["relation"] = dr["Relation"].ToString();
                            Transactioncount["transactionID"] = dr["Transaction_ID"];
                            Transactioncount["referenceNo"] = dr["ReferenceNo"];
                            Transactioncount["customerID"] = dr["Customer_ID"];
                            Transactioncount["beneficiaryID"] = dr["Beneficiary_ID"];
                            Transactioncount["transactionTypeID"] = dr["TransactionType_ID"];
                            Transactioncount["exchangeRate"] = dr["Exchange_Rate"];
                            Transactioncount["fromCurrencyCode"] = dr["FromCurrency_Code"];
                            Transactioncount["currencyCode"] = dr["Currency_Code"];
                            Transactioncount["countryID"] = dr["Country_ID"];
                            Transactioncount["purposeID"] = dr["Purpose_ID"];
                            Transactioncount["sourceofFunds"] = dr["SourceofFunds"].ToString();
                            Transactioncount["transferFees"] = dr["Transfer_Fees"];
                            Transactioncount["creditStatus"] = dr["Credit_Status"];
                            Transactioncount["recordInsertDateTime"] = dr["Record_Insert_DateTime"];
                            Transactioncount["deleteStatus"] = dr["Delete_Status"];
                            Transactioncount["paymentDepositTypeID"] = dr["PaymentDepositType_ID"];
                            Transactioncount["agentID"] = dr["Agent_ID"];
                            Transactioncount["collectionPointID"] = dr["CollectionPoint_ID"];
                            Transactioncount["deliveryTypeID"] = dr["DeliveryType_Id"];
                            Transactioncount["cbID"] = dr["CB_ID"];
                            Transactioncount["userID"] = dr["User_ID"];
                            Transactioncount["agentMappingID"] = dr["Agent_MappingID"];
                            Transactioncount["transactionthroughID"] = dr["TransactionThrough_ID"];
                            Transactioncount["spotOnAppSendMoneyFlag"] = dr["SpotOnApp_SendMoneyFlag"];
                            Transactioncount["proceedReason"] = dr["Proceed_Reason"];
                            Transactioncount["transactionFromFlag"] = dr["Transaction_From_Flag"];
                            Transactioncount["remainingPartialAmount"] = dr["RemainingPartial_Amount"];
                            Transactioncount["depositePartialAmount"] = dr["DepositePartial_Amount"];
                            Transactioncount["partialDepositFlag"] = dr["PartialDeposit_Flag"];
                            Transactioncount["authCode"] = dr["auth_code"];
                            Transactioncount["actualExchangeRate"] = dr["Actual_ExchangeRate"];
                            Transactioncount["manualRateChangedFlag"] = dr["ManualRateChangedFlag"];
                            Transactioncount["rateUpdateReasonID"] = dr["RateUpdateReason_ID"];
                            Transactioncount["msgToAgent"] = dr["MsgToAgent"];
                            Transactioncount["flagforcancellation"] = dr["Flag_for_cancellation"];
                            Transactioncount["noteFromCashierforTxCancellationFlag"] = dr["Note_From_Cashier_for_TxCancellation_Flag"];
                            Transactioncount["cancellationflagByUserId"] = dr["cancellation_flag_By_UserId"];
                            Transactioncount["gCCTransactionNo"] = dr["GCCTransactionNo"];
                            Transactioncount["gCCPayoutBranchID"] = dr["GCCPayoutBranch_ID"];
                            Transactioncount["gccRate"] = dr["GCCRate"];
                            Transactioncount["gccAmountInGBP"] = dr["GCCAmountInGBP"];
                            Transactioncount["gccAmountInPKR"] = dr["GCCAmountInPKR"];
                            Transactioncount["partPayFlag"] = dr["PartPay_Flag"];
                            Transactioncount["gccFees"] = dr["GCCFees"];
                            Transactioncount["gccCustomerRate"] = dr["GccCustomerRate"];
                            Transactioncount["custgccAmountInGBP"] = dr["CustGCCAmountInGBP"];
                            Transactioncount["custGCCAmountInPKR"] = dr["CustGCCAmountInPKR"];
                            Transactioncount["transactionSource"] = dr["Transaction_Source"];
                            Transactioncount["worldpayResponse"] = dr["Worldpay_Response"];
                            Transactioncount["sourceCommentFlag"] = dr["SourceComment_Flag"];
                            Transactioncount["sourceComment"] = dr["SourceComment"];
                            Transactioncount["otherPurpose"] = dr["Other_Purpose"];
                            Transactioncount["hDeliveryFlag"] = dr["HDelivery_Flag"];
                            Transactioncount["hDeliveryAddress"] = dr["HDelivery_Address"];
                            Transactioncount["clientID"] = dr["Client_ID"];
                            Transactioncount["payByCardID"] = dr["PayByCard_ID"];
                            Transactioncount["paymentGatewayID"] = dr["PaymentGateway_ID"];
                            Transactioncount["walletID"] = dr["Wallet_ID"];
                            Transactioncount["walletAmount"] = dr["Wallet_Amount"];
                            Transactioncount["discountID"] = dr["Discount_ID"];
                            Transactioncount["discountAmount"] = dr["Discount_Amount"];
                            Transactioncount["cancellationRequest"] = dr["Cancellation_Request"];
                            Transactioncount["cancellationReason"] = dr["Cancellation_Reason"];
                            Transactioncount["tillID"] = dr["Till_ID"];
                            Transactioncount["rateReasonDescription"] = dr["Rate_Reason_Description"];
                            Transactioncount["txnProcessingStatus"] = dr["Txn_ProcessingStatus"];
                            Transactioncount["comment"] = dr["Comment"];
                            Transactioncount["tranSign"] = dr["Tran_sign"];
                            Transactioncount["branchAddress"] = dr["Branch_Address"].ToString();
                            Transactioncount["branchPhoneOne"] = dr["Branch_PhoneOne"];
                            Transactioncount["totalAmount"] = dr["Totalamount"];
                            Transactioncount["Purpose"] = dr["Purpose"];
                            Transactioncount["wireTransferReferanceNo"] = dr["WireTransfer_ReferanceNo"];
                            Transactioncount["Sender"] = dr["Sender"];
                            Transactioncount["beneficiaryMobile"] = dr["Beneficiary_Mobile"];
                            Transactioncount["amountinOthercur"] = dr["Amount_in_other_cur"];
                            Transactioncount["foreignAmount"] = dr["Foreign_Amount"];
                            Transactioncount["paymentReceivedName"] = dr["paymentReceived_Name"];
                            Transactioncount["benef_status"] = dr["benef_status"];

                            string extractedDateFromDB = ""; string formattedDate = "";
                            extractedDateFromDB = dr["Date1"].ToString();
                            if (DateTime.TryParse(extractedDateFromDB, out DateTime parsedDate))
                            {
                                formattedDate = parsedDate.ToString("dd MMM yyyy");
                                Transactioncount["Date"] = formattedDate;
                            }
                            try
                            {
                                Transactioncount["senderMobile"] = dr["senderMobile"];
                            }
                            catch (Exception efg) { Transactioncount["senderMobile"] = ""; }

                            Transactioncount["User"] = dr["User"];
                            Transactioncount["typeName"] = dr["Type_Name"];
                            Transactioncount["deliveryType"] = dr["Delivery_Type"];
                            Transactioncount["dateFormat"] = dr["DateFormat"];
                            Transactioncount["recordInsertDateTimeFormat"] = dr["Record_Insert_DateTime_Format"];
                            Transactioncount["countryName"] = dr["Country_Name"];
                            Transactioncount["branch"] = dr["Branch"];
                            Transactioncount["countryName"] = dr["Country_Name"];
                            Transactioncount["Ptype"] = dr["Ptype"];
                            Transactioncount["reviewTransferMessage"] = dr["Review_Transfer_Message"];
                            Transactioncount["paymentReceivedId1"] = dr["paymentReceived_Id1"];
                            Transactioncount["senderaddress"] = dr["sender_address"];
                            Transactioncount["senderCc"] = dr["sender_cc"];
                            Transactioncount["recverAddress"] = dr["recver_Address"].ToString();
                            Transactioncount["recvCc"] = dr["recv_cc"];
                            Transactioncount["bankName"] = dr["Bank_Name"].ToString();
                            Transactioncount["accountNumber"] = dr["Account_Number"].ToString();
                            Transactioncount["documentName"] = dr["Document_Name"].ToString();
                            Transactioncount["showonapp"] = dr["show_on_app"].ToString();
                            Transactioncount["Branch"] = dr["Branch1"].ToString();
                            

                            if (dr["Tx_Status"].ToString() == "Hold")
                            {
                                Transactioncount["txStatus"] = "Pending";
                            }
                            else
                            {
                                Transactioncount["txStatus"] = dr["Tx_Status"];
                            }

                            Transactioncount["dateFormat"] = dr["DateFormat"];
                            Transactioncount["amountInGBP"] = dr["AmountInGBP"];
                            Transactioncount["amountInForeginCurrency"] = dr["AmountInPKR"];
                            //Transactioncount["tStatusName"] = dr["TStatus_Name"];
                            
                            if(dr["TransactionStatus_ID"].ToString() == "6")
                            Transactioncount["transactionStatusID"] = "1";
                            else
                            Transactioncount["transactionStatusID"] = dr["TransactionStatus_ID"];
                            Transactioncount["paymentReceivedID"] = dr["paymentReceived_ID"];
                            Transactioncount["paymentTypeID"] = dr["PaymentType_ID"];
                            Transactioncount["madeThisTransferFlag"] = dr["MadeThisTransfer_Flag"];
                            Transactioncount["image"] = "assets/img/sample/brand/clock-alert.png";
                            if (dr["TransactionStatus_ID"].ToString() == "1" || dr["TransactionStatus_ID"].ToString() == "6")
                            {
                                Transactioncount["image"] = "assets/img/sample/brand/clock-alert.png";
                            }
                            else if (dr["TransactionStatus_ID"].ToString() == "2")
                            {
                                Transactioncount["image"] = "assets/img/sample/brand/progress.png";
                            }
                            else if (dr["TransactionStatus_ID"].ToString() == "3")
                            {
                                Transactioncount["image"] = "assets/img/sample/brand/checkmark.png";
                            }
                            else if (dr["TransactionStatus_ID"].ToString() == "4")
                            {
                                Transactioncount["image"] = "assets/img/sample/brand/cancel.png";
                            }
                            else
                            {
                                Transactioncount["image"] = "assets/img/sample/brand/clock-alert.png";
                            }

                            if ((TStatus_ID == "1" || TStatus_ID == "6") && pstatus == "2" && madeTransFlag == "0" && PtypeID == "2")
                            {
                                Transactioncount["message"] = "I made this payment";
                                Transactioncount["newmessage"] = "if you have made a bank transfer of your remittance amount to our bank account then tap on this button.";
                            }
                            else if ((TStatus_ID == "1" || TStatus_ID == "6") && pstatus == "2" && madeTransFlag == "1" && PtypeID == "2")
                            {
                                Transactioncount["message"] = "I haven't made this payment";
                                Transactioncount["newmessage"] = "Payment seems pending as the money would be still with your bank. Once we receive the payment we will change the status to received. <br/>If you have still not paid and tapped the button in mistake, not a problem, you can tap on the provided option to update payment status.";

                            }
                            else if (TStatus_ID == "2")
                            {
                                Transactioncount["message"] = "Transfer Initiated";
                                Transactioncount["newmessage"] = "We have received your payment and initiated the transfer process.";

                            }
                            else if ((TStatus_ID == "1" || TStatus_ID == "6") && pstatus == "1")
                            {
                                Transactioncount["message"] = "Payment received";
                                Transactioncount["newmessage"] = "We have received your payment and we will initiate your transfer soon.";
                            }
                            else if ((TStatus_ID == "1" || TStatus_ID == "6") && pstatus == "2" && madeTransFlag == "1")
                            {
                                Transactioncount["message"] = "Awaiting payment";
                                Transactioncount["newmessage"] = "We will be checking our bank account for your money. This usually takes 2 hours to 2 days depending on the Bank.";
                            }
                            Transaction_count.Add(Transactioncount);
                        }
                        catch(Exception ex) {
                            CompanyInfo.InsertErrorLogTracker("Error getting element ViewTransfer:" + ex.ToString(), 0, 0, 0, 0, "ViewTransfer", Convert.ToInt32(0), Convert.ToInt32(0), "", context);
                        }
                    }

                   

                    var jsonData = new { response = true, responseCode = "00", data = Transaction_count, offset_value = _limit2 };
                    return new JsonResult(jsonData);
                }
                else
                {
                    var jsonData = new { response = true, responseCode = "00", data = Transaction_count };
                    return new JsonResult(jsonData);
                }

            }
            catch (Exception ex)
            {
                string Activity = "Api: viewtransfer " + ex.ToString();
                var jsonData = new { response = false, responseCode = "01", data = "Invalid Request" };
                CompanyInfo.InsertErrorLogTracker(Activity.ToString(), 0, 0, 0, 0, "ViewTransfer", Convert.ToInt32(obj1.Branch_ID), Convert.ToInt32(obj1.Client_ID), "", context);
                return new JsonResult(jsonData);

            }
        }

        [HttpPost]
        [Authorize]
        [Route("trackingdetails")]
        public IActionResult GetTrackingDetails([FromBody] JsonElement obj)
        {
            HttpContext context = HttpContext;
            string json = System.Text.Json.JsonSerializer.Serialize(obj);
            dynamic data = JObject.Parse(json);
            CompanyInfo.InsertrequestLogTracker("trackingdetails full request body: " + JObject.Parse(json), 0, 0, 0, 0, "GetTrackingDetails", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
            var validateJsonData = (dynamic)null;
            Model.response.WebResponse response = null;
            Beneficiary obj1 = new Beneficiary();
            // data used for validation purpose

            try
            {
                if (!SqlInjectionProtector.ReadJsonElementValues(obj) || !SqlInjectionProtector.ReadJsonElementValuesScript(obj))
                {
                    var errorResponse = new { response = false, responseCode = "02", data = "Invalid input detected." };
                    return new JsonResult(errorResponse) { StatusCode = 400 };
                }

                if (data.clientID == "" || data.clientID == null)
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "Invalid Client Id." };
                    return new JsonResult(validateJsonData);
                }
                if (data.branchID == "" || data.branchID == null)
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "Invalid Branch Id." };
                    return new JsonResult(validateJsonData);
                }
                if (data.transactionID == "" || data.transactionID == null)
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "Invalid Transaction Id." };
                    return new JsonResult(validateJsonData);
                }
                
               
                obj1.Client_ID = data.clientID;
                obj1.Id = data.transactionID;
                obj1.Branch_ID = data.branchID;

                object o = JsonConvert.DeserializeObject(json);
                bool checkinjection = CompanyInfo.checksqlinjectiondata(o);
                if (!checkinjection)
                {
                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_TECHNICAL_ERROR);
                    response.ResponseMessage = "Invalid Field Values.";
                    response.ResponseCode = 6;
                    validateJsonData = new { response = false, responseCode = "01", data = response.ResponseMessage };
                    return new JsonResult(validateJsonData);
                }

                List<Model.Beneficiary> _lst = new List<Model.Beneficiary>();

               

                MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("Get_Track_Details");
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.Parameters.AddWithValue("_Client_ID", obj1.Client_ID);
                _cmd.Parameters.AddWithValue("_Transaction_ID", obj1.Id);
                DataTable dt = db_connection.ExecuteQueryDataTableProcedure(_cmd);

                if (dt != null && dt.Rows.Count > 0)
                {

                    List<Dictionary<string, object>> trasaction = new List<Dictionary<string, object>>();

                    foreach (DataRow dr in dt.Rows)
                    {
                        Dictionary<string, object> transaction_history = new Dictionary<string, object>();
                        transaction_history["date"] = dr["DATE"];
                        transaction_history["trackId"] = dr["Track_ID"];
                        trasaction.Add(transaction_history);
                    }

                    validateJsonData = new { response = true, responseCode = "00", data = trasaction };
                    return new JsonResult(validateJsonData);
                }
                else
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "No Records Found." };
                    return new JsonResult(validateJsonData);

                }

            }
            catch (Exception ex)
            {
                validateJsonData = new { response = false, responseCode = "01", data = "Invalid Request" };
                CompanyInfo.InsertErrorLogTracker(ex.ToString(), 0, 0, 0, 0, "gettrackingdetails", Convert.ToInt32(obj1.Branch_ID), Convert.ToInt32(obj1.Client_ID), "", context);
                return new JsonResult(validateJsonData);

            }

        }


        [HttpPost]
        [Route("cancellationrequest")]
        public IActionResult Cancellation_Request([FromBody] JsonElement obj)
        {
            HttpContext context = HttpContext;
            string json = System.Text.Json.JsonSerializer.Serialize(obj);
            dynamic data = JObject.Parse(json);
            CompanyInfo.InsertrequestLogTracker("cancellationrequest full request body: " + JObject.Parse(json), 0, 0, 0, 0, "Cancellation_Request", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
            var validateJsonData = (dynamic)null;
            Model.response.WebResponse response = null;
            Transaction obj1 = new Transaction();
            // data used for validation purpose

            try
            {
                if (!SqlInjectionProtector.ReadJsonElementValues(obj) || !SqlInjectionProtector.ReadJsonElementValuesScript(obj))
                {
                    var errorResponse = new { response = false, responseCode = "02", data = "Invalid input detected." };
                    return new JsonResult(errorResponse) { StatusCode = 400 };
                }

                if (data.clientID == "" || data.clientID == null)
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "Invalid Client Id." };
                    return new JsonResult(validateJsonData);
                }
                if (data.branchID == "" || data.branchID == null)
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "Invalid Branch Id." };
                    return new JsonResult(validateJsonData);
                }
                if (data.transactionID == "" || data.transactionID == null)
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "Invalid Transaction Id." };
                    return new JsonResult(validateJsonData);
                }

                obj1.Client_ID = data.clientID;
                obj1.Branch_ID = data.branchID;
                obj1.Transaction_ID = data.transactionID;
                obj1.Proceed_Reason = data.proceedReason;


                object o = JsonConvert.DeserializeObject(json);
                bool checkinjection = CompanyInfo.checksqlinjectiondata(o);
                if (!checkinjection)
                {
                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_TECHNICAL_ERROR);
                    response.ResponseMessage = "Invalid Field Values.";
                    response.ResponseCode = 6;
                    validateJsonData = new { response = false, responseCode = "01", data = response.ResponseMessage };
                    return new JsonResult(validateJsonData);
                }

                Service.srvViewTransferHistory srv = new Service.srvViewTransferHistory();
                DataTable li1 = srv.Cancellation_Request(obj1, context);
                Model.Dashboard _Obj = new Model.Dashboard();
                response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_SUCCESS);
                response.data = "success";
                response.ObjData = li1;
                response.ResponseCode = 0;

                string response_data = Convert.ToString(li1.Rows[0]["Status"]);

                if(response_data == "0")
                {
                    response_data = "Transaction cancellation request sent successfully";
                }
                else
                {
                    response_data = "Failed to send cancellation request";
                }


                /*var returndata = li1.Rows.OfType<DataRow>()
                    .Select(row => li1.Columns.OfType<DataColumn>()
                        .ToDictionary(col => col.ColumnName, c => row[c]));*/

                validateJsonData = new { response = true, responseCode = "00", data = response_data };
                return new JsonResult(validateJsonData);

            }
            catch (Exception ex)
            {
                validateJsonData = new { response = false, responseCode = "01", data = "Invalid Request" };
                CompanyInfo.InsertErrorLogTracker(ex.ToString(), 0, 0, 0, 0, "Cancellation_Request", Convert.ToInt32(obj1.Branch_ID), Convert.ToInt32(obj1.Client_ID), "", context);
                return new JsonResult(validateJsonData);

            }

        }


        [HttpPost]
        [Authorize]
        [Route("trackingnames")]
        public IActionResult GetTrackingNames([FromBody] JsonElement obj)
        {
            HttpContext context = HttpContext;
            string json = System.Text.Json.JsonSerializer.Serialize(obj);
            dynamic data = JObject.Parse(json);
            CompanyInfo.InsertrequestLogTracker("trackingnames full request body: " + JObject.Parse(json), 0, 0, 0, 0, "GetTrackingNames", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
            var validateJsonData = (dynamic)null;
            Model.response.WebResponse response = null;
            Beneficiary obj1 = new Beneficiary();
            // data used for validation purpose

            try
            {
                if (!SqlInjectionProtector.ReadJsonElementValues(obj) || !SqlInjectionProtector.ReadJsonElementValuesScript(obj))
                {
                    var errorResponse = new { response = false, responseCode = "02", data = "Invalid input detected." };
                    return new JsonResult(errorResponse) { StatusCode = 400 };
                }

                if (data.clientID == "" || data.clientID == null)
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "Invalid Client Id." };
                    return new JsonResult(validateJsonData);
                }
                if (data.branchID == "" || data.branchID == null)
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "Invalid Branch Id." };
                    return new JsonResult(validateJsonData);
                }

                obj1.Client_ID = data.clientID;
               
                obj1.Branch_ID = data.branchID  ;

                object o = JsonConvert.DeserializeObject(json);
                bool checkinjection = CompanyInfo.checksqlinjectiondata(o);
                if (!checkinjection)
                {
                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_TECHNICAL_ERROR);
                    response.ResponseMessage = "Invalid Field Values.";
                    response.ResponseCode = 6;
                    validateJsonData = new { response = false, responseCode = "01", data = response.ResponseMessage };
                    return new JsonResult(validateJsonData);
                }

                List<Model.Beneficiary> _lst = new List<Model.Beneficiary>();

               MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("Get_Track_Names");
                _cmd.CommandType = CommandType.StoredProcedure;
                DataTable dt = db_connection.ExecuteQueryDataTableProcedure(_cmd);
                

                if (dt != null && dt.Rows.Count > 0)
                {

                    List<Dictionary<string, object>> trasaction = new List<Dictionary<string, object>>();

                    foreach (DataRow dr in dt.Rows)
                    {
                        Dictionary<string, object> transaction_history = new Dictionary<string, object>();
                        transaction_history["id"] = dr["ID"];
                        transaction_history["Trackingname"] = dr["Tracking_Name"];
                        trasaction.Add(transaction_history);
                    }

                    validateJsonData = new { response = true, responseCode = "00", data = trasaction };
                    return new JsonResult(validateJsonData);
                }
                else
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "No Records Found." };
                    return new JsonResult(validateJsonData);

                }

            }
            catch (Exception ex)
            {
                validateJsonData = new { response = false, responseCode = "01", data = "Invalid Request" };
                CompanyInfo.InsertErrorLogTracker(ex.ToString(), 0, 0, 0, 0, "gettrackingnames", Convert.ToInt32(obj1.Branch_ID), Convert.ToInt32(obj1.Client_ID), "", context);
                return new JsonResult(validateJsonData);

            }

        }

        [HttpPost]
        [Authorize]
        [Route("viewtransferconfiguration")]
        public IActionResult configuration([FromBody] JsonElement objd)
        {
            HttpContext context = HttpContext;
            string json = System.Text.Json.JsonSerializer.Serialize(objd);
            dynamic data = JObject.Parse(json);
            CompanyInfo.InsertrequestLogTracker("viewtransferconfiguration full request body: " + JObject.Parse(json), 0, 0, 0, 0, "configuration", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
            var returnJsonData = (dynamic)null;
            var validateJsonData = (dynamic)null;
            Model.response.WebResponse response = null;

            try
            {
                if (!SqlInjectionProtector.ReadJsonElementValues(objd) || !SqlInjectionProtector.ReadJsonElementValuesScript(objd))
                {
                    var errorResponse = new { response = false, responseCode = "02", data = "Invalid input detected." };
                    return new JsonResult(errorResponse) { StatusCode = 400 };
                }

                int number;
                if (!int.TryParse(Convert.ToString(data.clientID), out number))
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "Invalid Client Id." };
                    return new JsonResult(validateJsonData);
                }
                if (!int.TryParse(Convert.ToString(data.branchID), out number))
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "Invalid Branch Id." };
                    return new JsonResult(validateJsonData);
                }

                Beneficiary obj = JsonConvert.DeserializeObject<Beneficiary>(json);
                obj.Client_ID = data.clientID;
                obj.Branch_ID = data.branchID;

                Service.srvBeneficiary srv = new Service.srvBeneficiary(HttpContext);
                DataTable li1 = srv.GetReceiptConfig(obj);
                Model.Dashboard _Obj = new Model.Dashboard();
                if (li1 != null && li1.Rows.Count > 0)
                {
                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_SUCCESS);
                    response.data = "success";
                    response.ObjData = li1;
                    response.ResponseCode = 0;

                    var transferConfiguration = li1.Rows.OfType<DataRow>()
                    .Select(row => li1.Columns.OfType<DataColumn>()
                        .ToDictionary(col => col.ColumnName, c => row[c]));

                    foreach (DataColumn column in li1.Columns)
                    {
                        // Replace "OldColumnName" with the desired new name
                        column.ColumnName = column.ColumnName.Replace("status_for_customer", "status" );
                        column.ColumnName = column.ColumnName.Replace( "ID", "id");
                        column.ColumnName = column.ColumnName.Replace("permission_name" , "permission");
                    }

                    returnJsonData = new { response = true, responseCode = "00", data = transferConfiguration };
                    return new JsonResult(returnJsonData);
                }
                else
                {
                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_TECHNICAL_ERROR);
                    response.ResponseMessage = "No Records Found.";
                    response.ResponseCode = 0;
                    returnJsonData = new { response = false, responseCode = "02", data = response.ResponseMessage };
                    return new JsonResult(returnJsonData);
                }

            }
            catch (Exception ex)
            {
                string Activity = "Api: configuration " + ex.ToString();
                var jsonData = new { response = false, responseCode = "01", data = "Invalid Request" };
                CompanyInfo.InsertErrorLogTracker(Activity.ToString(), 0, 0, 0, 0, "configuration", Convert.ToInt32(0), Convert.ToInt32(0), "", context);
                returnJsonData = new { response = false, responseCode = "01", data = "Invalid Request." };
                return new JsonResult(returnJsonData);

            }


        }



        }
}
