namespace Calyx_Solutions.Model
{
    public class Transaction
    {
        
        public string ipAddress { get; set; }
        public string Store_Adder { get; set; }
        public string BarcodeDetails { get; set; }
        public string StoreDetails { get; set; }
        public int Basecurr_Country_ID { get; set; }
        public string basecurrency { get; set; }
        public string Trans_ID { get; set; }
        public string Toatal_amount { get; set; }
        public string curency { get; set; }
        public string payment_token { get; set; }
        public string bank_api_id { get; set; }
        public string TransactionID { get; set; }
        public string Company_Name { get; set; }
        public string API_URL { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string API_Status { get; set; }
        public string paysafekey { get; set; }
        public string curency_code { get; set; }
        public string merchent_num { get; set; }
        public string MSG { get; set; }
        public string Refence_num { get; set; }

        public string consumerId { get; set; }
        public string paymentHandleToken { get; set; }
        public string total_amount { get; set; }
        public int sanction_responce_bene_aml { get; set; }
        public int sanction_responce_bene_kyc { get; set; }
        public int sanction_responce_cust_aml { get; set; }
        public int sanction_responce_cust_kyc { get; set; }
        public string new_agent_id { get; set; }
        public int new_agent_branch { get; set; }
        public string bankGateway { get; set; }
        public int count { get; set; }
        public string Comment { get; set; }
        public string Username { get; set; }
        public string NameOnCard { get; set; }
        public int Till_ID { get; set; }
        public int BranchListAPI_ID { get; set; }
        public string APIBranch_Details { get; set; }
        public string tokenValue { get; set; }
       
        public int City_ID { get; set; }
        public int Perm_ID { get; set; }
        //invite details
        public int scheme_type_id { get; set; }
        public int referrer_maxnoofreferee { get; set; }
        public int multiples_of { get; set; }
        public int referrer_perk_type { get; set; }
        public int referee_perk_type { get; set; }
        public int referrer_discount_type { get; set; }
        public int referee_discount_type { get; set; }
        public decimal referrer_value { get; set; }
        public decimal referee_value { get; set; }
        public decimal referee_mintransferamount { get; set; }
        public string description { get; set; }
        public string CustomerWireTransfer_ReferanceNo { set; get; }
        public string CustomerFull_Name { set; get; }
        //Other Files
        public Login Login { set; get; }
        public Customer Customer { set; get; }
        public string Image { get; set; }
        public int Transaction_ID { get; set; }
        public string Customer_ID { get; set; }
        public int Beneficiary_ID { get; set; }
        public string Beneficiary_Name { get; set; }
        public int TransactionType_ID { get; set; }

        public int PaymentType_ID { get; set; }
        public int TransactionStatus_ID { get; set; }
        public double AmountInGBP { get; set; }
        public string FromCurrency_Code { get; set; }
        public int FromCurrency_CodeId { get; set; }
        public string Currency_Code { get; set; }

        public double Exchange_Rate { get; set; }
        public double AmountInPKR { get; set; }
        public int Purpose_ID { get; set; }
        public string SourceOfFunds { get; set; }
        public double Transfer_Fees { get; set; }
        public int Credit_Status { get; set; }
        public string Record_Insert_DateTime { get; set; }
        public int Delete_Status { get; set; }

        public int CollectionPoint_ID { get; set; }
        public string CollectionPoint { get; set; }
        public int Agent_ID { get; set; }
        public int PaymentDepositType_ID { get; set; }
        public int CB_ID { get; set; }
        public int Branch_ID { get; set; }
        public int Is_App { get; set; }
        public string ReferenceNo { get; set; }
        public int PaymentReceived_ID { get; set; }
        public int improved_rate_flag { get; set; }
        public int improved_rate_used_flag { get; set; }
        public int User_ID { get; set; }
        public string TransactionThrough_ID { get; set; }
        public int Agent_MappingID { get; set; }
        public int SpotOnApp_SendMoneyFlag { get; set; }


        public int MadeThisTransfer_Flag { get; set; }
        public int Country_ID { get; set; }
        public int Currency_ID { get; set; }
        public string Proceed_Reason { get; set; }
        public string Transaction_From_Flag { get; set; }
        public double RemainingPartial_Amount { get; set; }
        public double DepositePartial_Amount { get; set; }
        public int PartialDeposit_Flag { get; set; }
        public string auth_code { get; set; }
        public int DeliveryType_Id { get; set; }

   
        
        public string Actual_ExchangeRate { get; set; }
        public int ManualRateChangedFlag { get; set; }
        public int RateUpdateReason_ID { get; set; }
        public string MsgToAgent { get; set; }
        public int Flag_for_cancellation { get; set; }
        public string Note_From_Cashier_for_TxCancellation_Flag { get; set; }
        public int cancellation_flag_By_UserId { get; set; }
        public string GCCTransactionNo { get; set; }

        public int GCCPayoutBranch_ID { get; set; }
        public string GCCRate { get; set; }
        public string GCCAmountInGBP { get; set; }
        public string GCCAmountInPKR { get; set; }
        public int PartPay_Flag { get; set; }
        public string GCCFees { get; set; }
        public string GCCCustomerRate { get; set; }

        public string CustGCCAmountInGBP { get; set; }
        public string CustGCCAmountInPKR { get; set; }
        public string Transaction_Source { get; set; }
        public string Other_Purpose { get; set; }
        public int HDelivery_Flag { get; set; }
        public string HDelivery_Address { get; set; }
        public string Worldpay_Response { get; set; }
        public int Client_ID { get; set; }
        public int SourceComment_Flag { get; set; }
        public string SourceComment { get; set; }
        public string CustomerEmail { get; set; }
        public int PayByCard_ID { get; set; }
        public int PaymentGateway_ID { get; set; }
        public int Wallet_ID { get; set; }
        public double Wallet_Amount { get; set; }
        public int Discount_ID { get; set; }
        public double Discount_Amount { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string ForeignAmount { get; set; }
        public int offset_flag { get; set; }
        public string TransferAmount { get; set; }

        public string TransferForeignAmount { get; set; }

        public int PType_ID { get; set; }
        public int pTypeID { get; set; }
        
        public object TransferTypeFlag { get; set; }

        public string Purpose { get; set; }

        public int transfer_type { get; set; }

        public int paytype { get; set; }

        public int exchangerate { get; set; }

        public int fee { get; set; }

        public double newwalletbalance { get; set; }

        public double oldwalletbalance { get; set; }

        public double Transfer_Cost { get; set; }

        public string Wallet_Description { get; set; }

        public string Wallet_Currency { get; set; }

        public string DiscountType { get; set; }

        public string Discount_Code { get; set; }

        public int SOFID { get; set; }

        public string PaymentType { get; set; }

        public string TransferType { get; set; }

        public int Discount_Perm { get; set; }

        public int Wallet_Perm { get; set; }

        public string MadeThisTransfer_Label { get; set; }

        public string ReceiptNameWithExt { get; set; }

        public string CallBackURL { get; set; }
        public string payvyne_trans_id { get; set; }
        public double TotalAmount { get; set; }


        public string Customer_Reference { get; set; }

        public string GuavapayRefNo { get; set; }
        public string GuavapayorderId { get; set; }
        public string userAgent { get; set; }

        public int status { get; set; }
        public string referenceNumber { get; set; }
        public string customerReferenceNumber { get; set; }
        public int transactionID { get; set; }
        public string gccPinNumber { get; set; }

        
        public string LoginUserName { get; set; }
        public int payWithBankGatewayId { get; set; }

        public Double ExtraTransfer_Fees { set; get; }
        public int offer_rate_flag { get; set; }

        public int Wallet_Currency_type { get; set; }
        public Offer Offer { set; get; }
      
        public Double transfer_cost_foreign { set; get; }

     
        public string From_page { get; set; }
        public int Mapping_Data { get; set; }

        public int referee_applicablefor { set; get; } //Digvijay changes for reward info show transfer tyep wise

        public int Benf_BankDetails_ID { set; get; } // Vyankatesh change for multiple bank beneficiary details

        public string readTokenValue { set; get; }

        public int checklimit_step {  set; get; }
        public string TransactionReference { get; set; }//Added By Rushikesh

        public int testingpurpose { set; get; }
        
    }
}