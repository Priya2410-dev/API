

namespace Calyx_Solutions.Model
{
    public interface IMaster : ICrud_Master
    {
        int Terms_show { get; set; }
        int gender_vis { get; set; }
        int gender_man { get; set; }
        int denomination { get; set; }
        int Wallet_Min_Length { get; set; }
        int Trans_limit_man { get; set; }
        double Trans_limit_min { get; set; }
        double Trans_limit_max { get; set; }
        double Trans_lmtt_perday_benf { get; set; }
        double Trans_lmt_perday { get; set; }
        double Trans_lmt_peryear_benf { get; set; }

        int range_Branch_Transit { get; set; }
        int branch_transit_vis { get; set; }
        int Range_financial_institution { get; set; }
        int Financial_inst_vis { get; set; }
        int branch_transit_min { get; set; }
        int branch_transit_max { get; set; }
        int Finanical_Institue_min { get; set; }
        int Finanical_Institue_max { get; set; }

        int Micr_code_vis { get; set; }
        int Range_Micr_code { get; set; }
        int Micr_Code_max { get; set; }
        int Micr_Code_min { get; set; }

        int range_check_mob { get; set; }
        int range_check_wallet { get; set; }

        //ID Configuration
        int id_upload_man { get; set; }
        int issue_dt_man { get; set; }
        int id_no_man { get; set; }
        int expiry_dt_man { get; set; }
        int place_man { get; set; }
        int dob_man { get; set; }
        int front_man { get; set; }
        int back_man { get; set; }
        int comment_man { get; set; }
        int range_ID { get; set; }
        int id_min_len { get; set; }
        int id_max_len { get; set; }

        string Branch_label { get; set; }
        int BranchCode_man { get; set; }
        int Agent_Status_transfer { get; set; }
        int Cust_Phone { get; set; }
        int Cust_phone_man { get; set; }
        //provider
        int wallet_provider { get; set; }
        int mobile_provider { get; set; }
        int wallet_id_man { get; set; }
        int wallet_id { get; set; }
        int wallet_id_length { get; set; }
        #region beneficiary configuration
        int range_check_iban { get; set; }
        int iban_min_length { get; set; }
        int range_check_bic { get; set; }
        int bic_min_length { get; set; }

        string Time { get; set; }
        string working_days { get; set; }
        int Custom_Setting_Perm { get; set; }
        int benf_name { get; set; }

        int benf_dob { get; set; }

        int benf_rel { get; set; }

        int benf_mob { get; set; }

        int benf_tel { get; set; }

        int benf_add { get; set; }

        int benf_city { get; set; }

        int benf_country { get; set; }

        int benf_mulbank { get; set; }

        int mobile_min { get; set; }
        int mobile_max { get; set; }
        int telephone_min { get; set; }
        int telephone_max { get; set; }
        #endregion
        int IFSC_man { get; set; }
        int Bank_name { get; set; }
        int IBAN_man { get; set; }
        int BIC_man { get; set; }
        int Account_Min_Length { get; set; }
        int Account_Max_Length { get; set; }
        string Account_Name { get; set; }
        string IFSC_Name { get; set; }
        string IBAN_Name { get; set; }
        string BIC_Name { get; set; }

        string Reason_Text { get; set; }
        int Reason_Id { get; set; }
        int chk_export { get; set; }
        int Cred_Deb { get; set; }
        double amount1 { get; set; }
        double amount2 { get; set; }
        double amount3 { get; set; }
        double amount4 { get; set; }
        int IbanStatus { get; set; }
        int Iban_Length { get; set; }
        int BicStatus { get; set; }
        int BicLength { get; set; }
        int ShowAccount_No { get; set; }
        string Category_Name { get; set; }
        int Category_ID { get; set; }
        string Heard_From_option { get; set; }
        string reason { get; set; }
        int Reason_ID { get; set; }
        string ISO_Code { get; set; }
        int Daily_Limit_Cnt { get; set; }
        int Risk_Level { get; set; }
        //collection type
        int CollectionType_Id { get; set; }
        string Collection_Type { get; set; }
        int ShowOnCustSide { get; set; }
        int ShowOnAdmin { get; set; }
        int PreferredOnAdmin { get; set; }
        int PreferredOnAgent { get; set; }
        int PreferredOnCust { get; set; }

        int SalesRep_Flag { get; set; }
        string SalesRep_RefNo { get; set; }
        int Uppercase { get; set; }
        int Lowercase { get; set; }
        int Digit { get; set; }
        int Isspecial_Char { get; set; }
        string Special_char { get; set; }
        int Minpass_length { get; set; }
        int Maxpass_Length { get; set; }
        //Discount
        string discount_code { get; set; }
        int discount_type_id { get; set; }
        int amount_type_id { get; set; }
        int customer_eligibility_id { get; set; }
        int usagelimit_flag { get; set; }
        decimal usagelimit { get; set; }
        string start_date { get; set; }
        string end_date { get; set; }
        string discount_value { get; set; }
        //company profile
        string company_registration_number { get; set; }
        string Role_In_Company { get; set; }
        string Category { get; set; }
        int Company_Type { get; set; }
        string Company_ReferenceNo { get; set; }
        string website_url { get; set; }
        int auth_flag { get; set; }
        int Customer_ID { get; set; }
        //end company profile

        string whereclause { get; set; }
        string conditionclause { get; set; }
        string selectclause { get; set; }
        string SecurityKey { get; set; }
        string TimeZone { get; set; }
        string Sign { get; set; }
        int Default_Currency { get; set; }
        int User_ID { get; set; }
        string Flag { get; set; }

        int Id { get; set; }

        string Email_ID { get; set; }
        string ContactPhone { get; set; }
        string Mobile_Number { get; set; }

        string City { get; set; }
        // int City_ID { get; set; }
        string Country { get; set; }
        // int Country_ID { get; set; }
        string Nationality { get; set; }
        string Address { get; set; }
        string House_Number { get; set; }
        string Post_Code { get; set; }
        string Street { get; set; }
        string Profession { get; set; }
        string Company { get; set; }
        string sourceofregistration { get; set; }
        string gender { get; set; }
        string WireTransfer_ReferanceNo { get; set; }

        //custom email
        string Email_Template { get; set; }
        string Subject_Line { get; set; }
        string Email_Body { get; set; }
        //end

        int Delete_Status { get; set; } // common use (Used To all )
        int Client_ID { get; set; }
        int preferred_flag { get; set; }

        //Pramod kumbhar
        //Add new employee
        string User_Id { get; set; }
        string Role_ID { get; set; }

        string Title_ID { get; set; }
        string Last_Name { get; set; }
        string First_Name { get; set; }

        string Address_Emp { get; set; }
        string City_ID { get; set; }
        //         int CollCity_ID { get; set; }
        //string Mobile_Number { get; set; }
        //string Email_ID { get; set; }
        string Password_emp { get; set; }
        string Record_Insert_DateTime { get; set; }
        //string Delete_Status { get; set; }
        string Country_ID { get; set; }
        //string Agent_MappingID { get; set; }
        int SendMoney_Flag { get; set; }
        int AllCustomer_Flag { get; set; }
        int CB_ID { get; set; }

        // for add transfer fees
        double Min_Amount { get; set; }
        double Max_Amount { get; set; }
        double Transferfees { get; set; }
        string Transfer_ID { get; set; }
        //End Add transfer fees

        //Customer_TransferAmount
        double Minimum_Transfer_Amount { get; set; }
        double Maximum_Transfer_Amount { get; set; }
        //end Customer_TransferAmount

        //for Customer limit
        string Cust_limit_ID { get; set; }
        int Cust_Days { get; set; }
        double Personal_Trans_Amt_lmt { get; set; }
        double Company_Trans_Amt_lmt { get; set; }
        //End Customer Limit

        //for cashier limit
        string Cashier_limit_ID { get; set; }
        string Cashier_limitsupdatedby { get; set; }
        string Cashier_Pay_In { get; set; }
        string Cashier_Pay_Out { get; set; }
        string Cashier_UserRoll_Id { get; set; }
        //End cashier limit

        //For  Daily transfer limit
        string Cust_Trans_Limit_ID { get; set; }//common to for daily-transfer-limit and Yearly-transfer-limit
        double Daily_Limit { get; set; }
        double Total_TrasferAmt { get; set; }
        string Custmer_ID { get; set; }
        int Updated_By { get; set; }//common to for daily-transfer-limit and Yearly-transfer-limit
                                    // below two var. for userid selection
        string UserName { get; set; }
        string Password { get; set; }
        //End Daily transfer limit


        //Yearly Transfer limit
        double Month_Wise_Transaction_Limit { get; set; }
        string Yearlytrans_Months { get; set; }
        //End Yearly Transfer limit


        //currency master
        int Currency_ID { get; set; }
        string Currency_Name { get; set; }
        string Currency_Code { get; set; }
        double Spot_Rate { get; set; }
        double Lowest_Denomination { get; set; }
        int ShowOnWebsite { get; set; }
        double Currency_Limit { get; set; }
        string ImageName { get; set; }
        //end

        //country master
        string Country_code { get; set; }
        string Country_Currency { get; set; }
        string Country_Flag { get; set; }
        int sending_Flag { get; set; }
        //end

        //payment type master
        int PType_ID { get; set; }
        string PType { get; set; }
        double Max_Amount_Limit { get; set; }
        string Review_Transfer_Message { get; set; }
        int Payment_Type_Src { get; set; }
        //end

        //POC
        int Agent_MappingID { get; set; }
        //end

        //agent mapping table
        double Current_Balance_In_GBP { get; set; }
        double USD_Amount { get; set; }
        double Foreign_Currency_Amount { get; set; }
        double PayIn_Amount { get; set; }
        double PayOut_Amount { get; set; }
        //end

        //id master
        string ID_Name { get; set; }
        int IDType_ID { get; set; }
        //end

        //user role master
        string role_name { get; set; }
        int Module_ID { get; set; }
        int SubModule_ID { get; set; }
        //end

        //delivery type master
        int DeliveryType_Id { get; set; }
        string Delivery_Type { get; set; }
        //end

        //customer transfer limit
        double Transaction_Limit { get; set; }
        int Transaction_Count { get; set; }
        int Customer_Status { get; set; }
        int Months_For_Limit { get; set; }
        //end

        //transfer limit
        double transfer_amount { get; set; }
        int months { get; set; }
        double month_wise_trn_amt { get; set; }
        string update_date { get; set; }
        //end

        //request top up
        int Agent_Id { get; set; }
        double Request_Amount { get; set; }
        int Payment_Method { get; set; }
        double Approved_Amount { get; set; }
        string comments { get; set; }
        int Approved_by { get; set; }
        string Approved_Date { get; set; }
        string Requested_Date { get; set; }
        int Payment_Status { get; set; }
        int Topup_Status { get; set; }
        string Agent_name { get; set; }
        string from_date { get; set; }
        string to_date { get; set; }
        string topup_ref { get; set; }
        decimal GBP_Bal { get; set; }
        decimal USD_Bal { get; set; }
        decimal Foreign_Currency_Bal { get; set; }
        string Last_updated_date { get; set; }
        //end 
        int Bank_Code { get; set; }
        string Branch { get; set; }
        int IFSC { get; set; }
        int Branch_Code { get; set; }
        int Verify_Account_no { get; set; }
        int Acc_no_length { get; set; }
        #region New Code Add for Service Provider
        int CountryId { get; set; }
        int StateId { get; set; }
        int CityId { get; set; }
        int GroupLevel { get; set; }
        #endregion

        //emailconfiguration
        int Email_ID_Config { get; set; }
        int Port_Config { get; set; }
        string Email_Convey_from__Config { get; set; }
        string Host__Config { get; set; }
        string Password__Config { get; set; }
        int Priority__Config { get; set; }

        //change9920

        int Till_ID { get; set; }
        string Till_Name { get; set; }
        string Till_Activity { get; set; }
        int TillMapID { get; set; }
        int TillMapflag { get; set; }
        int Release_Reason_Id { get; set; }

        decimal overshort_amount { get; set; }
        string overshort_Comments { get; set; }
        int overshort_ID { get; set; }
        decimal ActualBalance { get; set; }
        decimal CurrentBalance { get; set; }
        int Client_Stock_Flag { get; set; }

        decimal OpeningBalance { get; set; }
        #region register
        int Cus_email { get; set; }
        int Cus_email_man { get; set; }

        int Cus_con_email { get; set; }
        int Cus_con_email_man { get; set; }

        int Cus_pass { get; set; }
        int Cus_pass_man { get; set; }

        int Cus_con_pass { get; set; }
        int Cus_con_pass_man { get; set; }

        int Ref_code { get; set; }
        int Ref_code_man { get; set; }

        int cust_title { get; set; }
        int cust_title_man { get; set; }

        int cust_first { get; set; }
        int cust_first_man { get; set; }

        int cust_middle { get; set; }
        int cust_middle_man { get; set; }

        int cust_last { get; set; }
        int cust_last_man { get; set; }

        int cust_dob { get; set; }
        int cust_dob_man { get; set; }

        int cust_mobile { get; set; }
        int cust_mobile_man { get; set; }

        int post_code { get; set; }
        int post_code_man { get; set; }

        int house_no { get; set; }
        int house_no_man { get; set; }

        int Add1 { get; set; }
        int Add1_man { get; set; }

        int Add2 { get; set; }
        int Add2_man { get; set; }

        int cust_country { get; set; }
        int cust_country_man { get; set; }

        int cust_city { get; set; }
        int cust_city_man { get; set; }

        int cust_nation { get; set; }
        int cust_nation_man { get; set; }

        int Emp_status { get; set; }
        int Emp_status_man { get; set; }

        int cust_prof { get; set; }
        int cust_prof_man { get; set; }

        int Comp_name { get; set; }
        int Comp_name_man { get; set; }

        int Heard_from { get; set; }
        int Heard_from_man { get; set; }
        #endregion


        //SMS:Cashback Details
        int cashbackid { get; set; }
        int offertypeid { get; set; } //1:Cashback 2: reward
        int cashbackagainsttypeid { get; set; } //1:Fee against 2:Amount Against   (discounttype)
        int cashbackamounttypeid { get; set; } //1: Percentage 2:fixed
        int allow_reward_scheme { get; set; }
        int reward_after_every_N_transaction { get; set; }
        double cashback_value { get; set; }

        int transaction_id { get; set; }


        //int cashbacktype_id { get; set; }//discount_type_id 
        //int amounttype_id { get; set; }//amount_type_id
        //string cashback_value { get; set; }//discount_value
        //int customer_eligibility_id { get; set; }
        //int usagelimit_flag { get; set; }
        //decimal usagelimit { get; set; }
        //string start_date { get; set; }
        //string end_date { get; set; }

        //double Minimum_Transfer_Amount { get; set; }
        //double Maximum_Transfer_Amount { get; set; }

    }
}
