namespace Calyx_Solutions.Model
{
    public class clsMaster : IMaster
    {
        int _Terms_show; public int Terms_show { get { return _Terms_show; } set { _Terms_show = value; } }
        int _gender_vis; public int gender_vis { get { return _gender_vis; } set { _gender_vis = value; } }
        int _gender_man; public int gender_man { get { return _gender_man; } set { _gender_man = value; } }
        int _denomination; public int denomination { get { return _denomination; } set { _denomination = value; } }
        int _Wallet_Min_Length; public int Wallet_Min_Length { get { return _Wallet_Min_Length; } set { _Wallet_Min_Length = value; } }
        int _Trans_limit_man; public int Trans_limit_man { get { return _Trans_limit_man; } set { _Trans_limit_man = value; } }
        double _Trans_limit_min; public double Trans_limit_min { get { return _Trans_limit_min; } set { _Trans_limit_min = value; } }
        double _Trans_limit_max; public double Trans_limit_max { get { return _Trans_limit_max; } set { _Trans_limit_max = value; } }
        double _Trans_lmtt_perday_benf; public double Trans_lmtt_perday_benf { get { return _Trans_lmtt_perday_benf; } set { _Trans_lmtt_perday_benf = value; } }
        double _Trans_lmt_perday; public double Trans_lmt_perday { get { return _Trans_lmt_perday; } set { _Trans_lmt_perday = value; } }
        double _Trans_lmt_peryear_benf; public double Trans_lmt_peryear_benf { get { return _Trans_lmt_peryear_benf; } set { _Trans_lmt_peryear_benf = value; } }

        int _Range_Micr_code; public int Range_Micr_code { get { return _Range_Micr_code; } set { _Range_Micr_code = value; } }
        int _Micr_code_vis; public int Micr_code_vis { get { return _Micr_code_vis; } set { _Micr_code_vis = value; } }
        int _Micr_Code_min; public int Micr_Code_min { get { return _Micr_Code_min; } set { _Micr_Code_min = value; } }
        int _Micr_Code_max; public int Micr_Code_max { get { return _Micr_Code_max; } set { _Micr_Code_max = value; } }
        int _Finanical_Institue_min; public int Finanical_Institue_min { get { return _Finanical_Institue_min; } set { _Finanical_Institue_min = value; } }
        int _Finanical_Institue_max; public int Finanical_Institue_max { get { return _Finanical_Institue_max; } set { _Finanical_Institue_max = value; } }

        int _branch_transit_min; public int branch_transit_min { get { return _branch_transit_min; } set { _branch_transit_min = value; } }
        int _branch_transit_max; public int branch_transit_max { get { return _branch_transit_max; } set { _branch_transit_max = value; } }
        int _Financial_inst_vis; public int Financial_inst_vis { get { return _Financial_inst_vis; } set { _Financial_inst_vis = value; } }
        int _Range_financial_institution; public int Range_financial_institution { get { return _Range_financial_institution; } set { _Range_financial_institution = value; } }
        int _branch_transit_vis; public int branch_transit_vis { get { return _branch_transit_vis; } set { _branch_transit_vis = value; } }
        int _range_Branch_Transit; public int range_Branch_Transit { get { return _range_Branch_Transit; } set { _range_Branch_Transit = value; } }

        int _range_check_mob; public int range_check_mob { get { return _range_check_mob; } set { _range_check_mob = value; } }
        int _range_check_wallet; public int range_check_wallet { get { return _range_check_wallet; } set { _range_check_wallet = value; } }

        //ID Configuration

        int _id_upload_man; public int id_upload_man { get { return _id_upload_man; } set { _id_upload_man = value; } }
        int _id_no_man; public int id_no_man { get { return _id_no_man; } set { _id_no_man = value; } }
        int _issue_dt_man; public int issue_dt_man { get { return _issue_dt_man; } set { _issue_dt_man = value; } }
        int _expiry_dt_man; public int expiry_dt_man { get { return _expiry_dt_man; } set { _expiry_dt_man = value; } }
        int _place_man; public int place_man { get { return _place_man; } set { _place_man = value; } }
        int _dob_man; public int dob_man { get { return _dob_man; } set { _dob_man = value; } }
        int _front_man; public int front_man { get { return _front_man; } set { _front_man = value; } }
        int _back_man; public int back_man { get { return _back_man; } set { _back_man = value; } }
        int _comment_man; public int comment_man { get { return _comment_man; } set { _comment_man = value; } }
        int _range_ID; public int range_ID { get { return _range_ID; } set { _range_ID = value; } }
        int _id_min_len; public int id_min_len { get { return _id_min_len; } set { _id_min_len = value; } }
        int _id_max_len; public int id_max_len { get { return _id_max_len; } set { _id_max_len = value; } }
        string _Branch_label; public string Branch_label { get { return _Branch_label; } set { _Branch_label = value; } }

        int _BranchCode_man; public int BranchCode_man { get { return _BranchCode_man; } set { _BranchCode_man = value; } }

        int _Agent_Status_transfer;
        public int Agent_Status_transfer { get { return _Agent_Status_transfer; } set { _Agent_Status_transfer = value; } }

        int _Cust_phone_man; public int Cust_phone_man { get { return _Cust_phone_man; } set { _Cust_phone_man = value; } }
        int _Cust_Phone; public int Cust_Phone { get { return _Cust_Phone; } set { _Cust_Phone = value; } }

        int _wallet_provider; public int wallet_provider { get { return _wallet_provider; } set { _wallet_provider = value; } }
        int _mobile_provider; public int mobile_provider { get { return _mobile_provider; } set { _mobile_provider = value; } }
        int _wallet_id_man; public int wallet_id_man { get { return _wallet_id_man; } set { _wallet_id_man = value; } }
        int _wallet_id_length; public int wallet_id_length { get { return _wallet_id_length; } set { _wallet_id_length = value; } }
        int _wallet_id; public int wallet_id { get { return _wallet_id; } set { _wallet_id = value; } }
        #region beneficiary configuration
        int _range_check_iban; public int range_check_iban { get { return _range_check_iban; } set { _range_check_iban = value; } }
        int _iban_min_length; public int iban_min_length { get { return _iban_min_length; } set { _iban_min_length = value; } }
        int _range_check_bic; public int range_check_bic { get { return _range_check_bic; } set { _range_check_bic = value; } }
        int _bic_min_length; public int bic_min_length { get { return _bic_min_length; } set { _bic_min_length = value; } }

        string _Time;
        public string Time
        {
            get { return _Time; }
            set { _Time = value; }
        }
        string _working_days;
        public string working_days
        {
            get { return _working_days; }
            set { _working_days = value; }
        }

        int _Custom_Setting_Perm;
        public int Custom_Setting_Perm
        {
            get { return _Custom_Setting_Perm; }
            set { _Custom_Setting_Perm = value; }
        }
        int _benf_name; public int benf_name { get { return _benf_name; } set { _benf_name = value; } }

        int _benf_dob; public int benf_dob { get { return _benf_dob; } set { _benf_dob = value; } }

        int _benf_rel; public int benf_rel { get { return _benf_rel; } set { _benf_rel = value; } }

        int _benf_mob; public int benf_mob { get { return _benf_mob; } set { _benf_mob = value; } }

        int _benf_tel; public int benf_tel { get { return _benf_tel; } set { _benf_tel = value; } }

        int _benf_add; public int benf_add { get { return _benf_add; } set { _benf_add = value; } }

        int _benf_city; public int benf_city { get { return _benf_city; } set { _benf_city = value; } }

        int _benf_country; public int benf_country { get { return _benf_country; } set { _benf_country = value; } }

        int _benf_mulbank; public int benf_mulbank { get { return _benf_mulbank; } set { _benf_mulbank = value; } }

        int _mobile_min; public int mobile_min { get { return _mobile_min; } set { _mobile_min = value; } }

        int _mobile_max; public int mobile_max { get { return _mobile_max; } set { _mobile_max = value; } }

        int _telephone_min; public int telephone_min { get { return _telephone_min; } set { _telephone_min = value; } }

        int _telephone_max; public int telephone_max { get { return _telephone_max; } set { _telephone_max = value; } }
        #endregion
        int _IFSC_man; public int IFSC_man { get { return _IFSC_man; } set { _IFSC_man = value; } }
        int _Bank_name; public int Bank_name { get { return _Bank_name; } set { _Bank_name = value; } }
        int _IBAN_man; public int IBAN_man { get { return _IBAN_man; } set { _IBAN_man = value; } }
        int _BIC_man; public int BIC_man { get { return _BIC_man; } set { _BIC_man = value; } }
        int _Account_Min_Length; public int Account_Min_Length { get { return _Account_Min_Length; } set { _Account_Min_Length = value; } }
        int _Account_Max_Length; public int Account_Max_Length { get { return _Account_Max_Length; } set { _Account_Max_Length = value; } }
        string _Account_Name; public string Account_Name { get { return _Account_Name; } set { _Account_Name = value; } }
        string _IFSC_Name; public string IFSC_Name { get { return _IFSC_Name; } set { _IFSC_Name = value; } }
        string _IBAN_Name; public string IBAN_Name { get { return _IBAN_Name; } set { _IBAN_Name = value; } }
        string _BIC_Name; public string BIC_Name { get { return _BIC_Name; } set { _BIC_Name = value; } }

        int _Reason_Id; public int Reason_Id { get { return _Reason_Id; } set { _Reason_Id = value; } }
        string _Reason_Text; public string Reason_Text { get { return _Reason_Text; } set { _Reason_Text = value; } }
        int _chk_export; public int chk_export { get { return _chk_export; } set { _chk_export = value; } }
        int _Cred_Deb; public int Cred_Deb { get { return _Cred_Deb; } set { _Cred_Deb = value; } }
        double _amount1; public double amount1 { get { return _amount1; } set { _amount1 = value; } }
        double _amount2; public double amount2 { get { return _amount2; } set { _amount2 = value; } }
        double _amount3; public double amount3 { get { return _amount3; } set { _amount3 = value; } }
        double _amount4; public double amount4 { get { return _amount4; } set { _amount4 = value; } }

        int _Iban_Length; public int Iban_Length { get { return _Iban_Length; } set { _Iban_Length = value; } }
        int _IbanStatus; public int IbanStatus { get { return _IbanStatus; } set { _IbanStatus = value; } }
        int _BicStatus; public int BicStatus { get { return _BicStatus; } set { _BicStatus = value; } }
        int _BicLength; public int BicLength { get { return _BicLength; } set { _BicLength = value; } }
        int _ShowAccount_No; public int ShowAccount_No { get { return _ShowAccount_No; } set { _ShowAccount_No = value; } }

        string _Category_Name; public string Category_Name { get { return _Category_Name; } set { _Category_Name = value; } }
        int _Category_ID; public int Category_ID { get { return _Category_ID; } set { _Category_ID = value; } }
        string _Heard_From_option; public string Heard_From_option { get { return _Heard_From_option; } set { _Heard_From_option = value; } }
        int _Reason_ID; public int Reason_ID { get { return _Reason_ID; } set { _Reason_ID = value; } }
        string _reason; public string reason { get { return _reason; } set { _reason = value; } }
        int _overshort_ID; public int overshort_ID { get { return _overshort_ID; } set { _overshort_ID = value; } }
        decimal _CurrentBalance; public decimal CurrentBalance { get { return _CurrentBalance; } set { _CurrentBalance = value; } }
        decimal _ActualBalance; public decimal ActualBalance { get { return _ActualBalance; } set { _ActualBalance = value; } }
        string _overshort_Comments; public string overshort_Comments { get { return _overshort_Comments; } set { _overshort_Comments = value; } }
        int _Client_Stock_Flag; public int Client_Stock_Flag { get { return _Client_Stock_Flag; } set { _Client_Stock_Flag = value; } }
        decimal _overshort_amount; public decimal overshort_amount { get { return _overshort_amount; } set { _overshort_amount = value; } }
        decimal _OpeningBalance; public decimal OpeningBalance { get { return _OpeningBalance; } set { _OpeningBalance = value; } }
        int _Till_ID; public int Till_ID { get { return _Till_ID; } set { _Till_ID = value; } }
        string _Till_Name; public string Till_Name { get { return _Till_Name; } set { _Till_Name = value; } }
        int _TillMapID; public int TillMapID { get { return _TillMapID; } set { _TillMapID = value; } }
        int _TillMapflag; public int TillMapflag { get { return _TillMapflag; } set { _TillMapflag = value; } }
        int _Release_Reason_Id; public int Release_Reason_Id { get { return _Release_Reason_Id; } set { _Release_Reason_Id = value; } }
        string _Till_Activity; public string Till_Activity { get { return _Till_Activity; } set { _Till_Activity = value; } }

        string _ISO_Code; public string ISO_Code { get { return _ISO_Code; } set { _ISO_Code = value; } }
        int _Daily_Limit_Cnt; public int Daily_Limit_Cnt { get { return _Daily_Limit_Cnt; } set { _Daily_Limit_Cnt = value; } }
        string _selectclause; public string selectclause { get { return _selectclause; } set { _selectclause = value; } }
        string _conditionclause; public string conditionclause { get { return _conditionclause; } set { _conditionclause = value; } }
        int _PreferredOnAdmin; public int PreferredOnAdmin { get { return _PreferredOnAdmin; } set { _PreferredOnAdmin = value; } }
        int _PreferredOnCust; public int PreferredOnCust { get { return _PreferredOnCust; } set { _PreferredOnCust = value; } }
        int _PreferredOnAgent; public int PreferredOnAgent { get { return _PreferredOnAgent; } set { _PreferredOnAgent = value; } }

        int _Risk_Level; public int Risk_Level { get { return _Risk_Level; } set { _Risk_Level = value; } }

        int _CollectionType_Id; public int CollectionType_Id { get { return _CollectionType_Id; } set { _CollectionType_Id = value; } }
        string _Collection_Type; public string Collection_Type { get { return _Collection_Type; } set { _Collection_Type = value; } }

        int _ShowOnCustSide; public int ShowOnCustSide { get { return _ShowOnCustSide; } set { _ShowOnCustSide = value; } }
        int _ShowOnAdmin; public int ShowOnAdmin { get { return _ShowOnAdmin; } set { _ShowOnAdmin = value; } }

        int _SalesRep_Flag; public int SalesRep_Flag { get { return _SalesRep_Flag; } set { _SalesRep_Flag = value; } }
        string _SalesRep_RefNo; public string SalesRep_RefNo { get { return _SalesRep_RefNo; } set { _SalesRep_RefNo = value; } }

        int _Uppercase; public int Uppercase { get { return _Uppercase; } set { _Uppercase = value; } }
        int _Lowercase; public int Lowercase { get { return _Lowercase; } set { _Lowercase = value; } }
        int _Digit; public int Digit { get { return _Digit; } set { _Digit = value; } }
        int _Isspecial_Char; public int Isspecial_Char { get { return _Isspecial_Char; } set { _Isspecial_Char = value; } }
        string _Special_char; public string Special_char { get { return _Special_char; } set { _Special_char = value; } }
        int _Minpass_length; public int Minpass_length { get { return _Minpass_length; } set { _Minpass_length = value; } }
        int _Maxpass_Length; public int Maxpass_Length { get { return _Maxpass_Length; } set { _Maxpass_Length = value; } }

        //Discount
        string _discount_code; public string discount_code { get { return _discount_code; } set { _discount_code = value; } }
        int _discount_type_id; public int discount_type_id { get { return _discount_type_id; } set { _discount_type_id = value; } }
        int _amount_type_id; public int amount_type_id { get { return _amount_type_id; } set { _amount_type_id = value; } }
        int _customer_eligibility_id; public int customer_eligibility_id { get { return _customer_eligibility_id; } set { _customer_eligibility_id = value; } }
        int _usagelimit_flag; public int usagelimit_flag { get { return _usagelimit_flag; } set { _usagelimit_flag = value; } }
        decimal _usagelimit; public decimal usagelimit { get { return _usagelimit; } set { _usagelimit = value; } }
        string _start_date; public string start_date { get { return _start_date; } set { _start_date = value; } }
        string _end_date; public string end_date { get { return _end_date; } set { _end_date = value; } }
        string _discount_value; public string discount_value { get { return _discount_value; } set { _discount_value = value; } }
        //company profile

        string _company_registration_number; public string company_registration_number { get { return _company_registration_number; } set { _company_registration_number = value; } }
        string _Role_In_Company; public string Role_In_Company { get { return _Role_In_Company; } set { _Role_In_Company = value; } }
        string _Category; public string Category { get { return _Category; } set { _Category = value; } }
        string _Company_ReferenceNo; public string Company_ReferenceNo { get { return _Company_ReferenceNo; } set { _Company_ReferenceNo = value; } }
        int _Company_Type; public int Company_Type { get { return _Company_Type; } set { _Company_Type = value; } }
        string _website_url; public string website_url { get { return _website_url; } set { _website_url = value; } }
        int _auth_flag; public int auth_flag { get { return _auth_flag; } set { _auth_flag = value; } }

        //end company profile
        string _TimeZone; public string TimeZone { get { return _TimeZone; } set { _TimeZone = value; } }
        string _Sign; public string Sign { get { return _Sign; } set { _Sign = value; } }

        string _SecurityKey; public string SecurityKey { get { return _SecurityKey; } set { _SecurityKey = value; } }
        string _whereclause;
        public string whereclause { get { return _whereclause; } set { _whereclause = value; } }

        string _Flag;
        public string Flag { get { return _Flag; } set { _Flag = value; } }

        string _BaseCurrency;
        public string BaseCurrency { get { return _BaseCurrency; } set { _BaseCurrency = value; } }

        int _User_ID; public int User_ID { get { return _User_ID; } set { _User_ID = value; } }
        int _Currency_ID; public int Currency_ID { get { return _Currency_ID; } set { _Currency_ID = value; } }
        int _Default_Currency; public int Default_Currency { get { return _Default_Currency; } set { _Default_Currency = value; } }

        string _UserName;
        public string UserName { get { return _UserName; } set { _UserName = value; } }

        int _userroleid;
        public int userroleid { get { return _userroleid; } set { _userroleid = value; } }

        string _LoginName;
        public string LoginName { get { return _LoginName; } set { _LoginName = value; } }

        string _Password;
        public string Password { get { return _Password; } set { _Password = value; } }

        string _WireTransfer_ReferanceNo;
        public string WireTransfer_ReferanceNo { get { return _WireTransfer_ReferanceNo; } set { _WireTransfer_ReferanceNo = value; } }

        string _Email_ID;
        public string Email_ID { get { return _Email_ID; } set { _Email_ID = value; } }

        string _Mobile_Number;
        public string Mobile_Number { get { return _Mobile_Number; } set { _Mobile_Number = value; } }

        string _sourceofregistration;
        public string sourceofregistration { get { return _sourceofregistration; } set { _sourceofregistration = value; } }

        string _cust_status;
        public string cust_status { get { return _cust_status; } set { _cust_status = value; } }

        int _Id;
        public int Id { get { return _Id; } set { _Id = value; } }

        int _SPId;
        public int SPId { get { return _SPId; } set { _SPId = value; } }

        string _CustomerName;
        public string CustomerName { get { return _CustomerName; } set { _CustomerName = value; } }

        string _First_Name;
        public string First_Name { get { return _First_Name; } set { _First_Name = value; } }

        string _Last_Name;
        public string Last_Name { get { return _Last_Name; } set { _Last_Name = value; } }

        string _ContactPerson;
        public string ContactPerson { get { return _ContactPerson; } set { _ContactPerson = value; } }

        string _ContactPhone;
        public string ContactPhone { get { return _ContactPhone; } set { _ContactPhone = value; } }

        string _Address;
        public string Address { get { return _Address; } set { _Address = value; } }

        string _City; public string City { get { return _City; } set { _City = value; } }
        int _CollCity_ID; public int CollCity_ID { get { return _CollCity_ID; } set { _CollCity_ID = value; } }
        //custom email
        string _Email_Template;
        public string Email_Template { get { return _Email_Template; } set { _Email_Template = value; } }
        string _Subject_Line;
        public string Subject_Line { get { return _Subject_Line; } set { _Subject_Line = value; } }
        string _Email_Body;
        public string Email_Body { get { return _Email_Body; } set { _Email_Body = value; } }
        //end email

        string _Street;
        public string Street { get { return _Street; } set { _Street = value; } }

        string _House_Number;
        public string House_Number { get { return _House_Number; } set { _House_Number = value; } }

        string _Nationality;
        public string Nationality { get { return _Nationality; } set { _Nationality = value; } }

        string _Country;
        public string Country { get { return _Country; } set { _Country = value; } }

        string _country_code;
        public string country_code { get { return _country_code; } set { _country_code = value; } }

        //int _Country_ID;
        //public int Country_ID { get { return _Country_ID; } set { _Country_ID = value; } }

        string _Post_Code;
        public string Post_Code { get { return _Post_Code; } set { _Post_Code = value; } }

        string _gender;
        public string gender { get { return _gender; } set { _gender = value; } }

        string _Profession;
        public string Profession { get { return _Profession; } set { _Profession = value; } }

        string _Company;
        public string Company { get { return _Company; } set { _Company = value; } }

        string _Operation_Name;
        public string Operation_Name { get { return _Operation_Name; } set { _Operation_Name = value; } }

        private string _Is_Procedure;
        public string Is_Procedure { get { return _Is_Procedure; } set { _Is_Procedure = value; } }

        string _Remark;
        public string Remark { get { return _Remark; } set { _Remark = value; } }

        string _Designation;
        public string Designation { get { return _Designation; } set { _Designation = value; } }

        string _Bank_Name;
        public string Bank_Name { get { return _Bank_Name; } set { _Bank_Name = value; } }

        string _Bank_code;
        public string Bank_code { get { return _Bank_code; } set { _Bank_code = value; } }

        string _Bank_ID;
        public string Bank_ID { get { return _Bank_ID; } set { _Bank_ID = value; } }

        int _status;
        public int status { get { return _status; } set { _status = value; } }
        int _preferred_flag;
        public int preferred_flag { get { return _preferred_flag; } set { _preferred_flag = value; } }

        public object InsertToDatabase()
        {
            try
            {
                return MasterDAC.InsertToDatabase(this);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public object UpdateToDatabase()
        {
            try
            {
                return MasterDAC.UpdateToDatabase(this);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public object ReadFromDatabase()
        {
            try
            {
                return MasterDAC.ReadFromDatabase(this);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public object DeleteFromDatabase()
        {
            try
            {
                return MasterDAC.DeleteFromDatabase(this);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public object ReadSingleValueFromDatabase()
        {
            return MasterDAC.ReadSingleValueFromDatabase(this);
        }
        public object Check_Exists()
        {
            return MasterDAC.Check_Exists(this);
        }
       
        string _OS_Type;
        public string OS_Type
        {
            get { return _OS_Type; }
            set { _OS_Type = value; }
        }

        int _Bank_Code;
        public int Bank_Code { get { return _Bank_Code; } set { _Bank_Code = value; } }

        int _IFSC;
        public int IFSC { get { return _IFSC; } set { _IFSC = value; } }

        int _Branch_Code;
        public int Branch_Code { get { return _Branch_Code; } set { _Branch_Code = value; } }

        int _Acc_no_length;
        public int Acc_no_length { get { return _Acc_no_length; } set { _Acc_no_length = value; } }

        int _Verify_Account_no;
        public int Verify_Account_no { get { return _Verify_Account_no; } set { _Verify_Account_no = value; } }

        int _CountryId;
        public int CountryId
        {
            get { return _CountryId; }
            set { _CountryId = value; }
        }

        int _StateId;
        public int StateId
        {
            get { return _StateId; }
            set { _StateId = value; }
        }

        int _CityId;
        public int CityId
        {
            get { return _CityId; }
            set { _CityId = value; }
        }

        int _GroupLevel;
        public int GroupLevel
        {
            get { return _GroupLevel; }
            set { _GroupLevel = value; }
        }

        //Benf
        int _Beneficiary_ID;
        public int Beneficiary_ID
        {
            get { return _Beneficiary_ID; }
            set { _Beneficiary_ID = value; }
        }

        string _Beneficiary_Name;
        public string Beneficiary_Name
        {
            get { return _Beneficiary_Name; }
            set { _Beneficiary_Name = value; }
        }

        string _Beneficiary_Address;
        public string Beneficiary_Address
        {
            get { return _Beneficiary_Address; }
            set { _Beneficiary_Address = value; }
        }
        int _Beneficiary_City_ID;
        public int Beneficiary_City_ID
        {
            get { return _Beneficiary_City_ID; }
            set { _Beneficiary_City_ID = value; }
        }

        int _Beneficiary_Country_ID;
        public int Beneficiary_Country_ID
        {
            get { return _Beneficiary_Country_ID; }
            set { _Beneficiary_Country_ID = value; }
        }
        string _Beneficiary_Telephone;
        public string Beneficiary_Telephone
        {
            get { return _Beneficiary_Telephone; }
            set { _Beneficiary_Telephone = value; }
        }
        string _Beneficiary_Mobile;
        public string Beneficiary_Mobile
        {
            get { return _Beneficiary_Mobile; }
            set { _Beneficiary_Mobile = value; }
        }
        string _Record_Insert_DateTime;
        public string Record_Insert_DateTime
        {
            get { return _Record_Insert_DateTime; }
            set { _Record_Insert_DateTime = value; }
        }

        int _Created_By_User_ID;
        public int Created_By_User_ID
        {
            get { return _Created_By_User_ID; }
            set { _Created_By_User_ID = value; }
        }
        int _Customer_ID;
        public int Customer_ID
        {
            get { return _Customer_ID; }
            set { _Customer_ID = value; }
        }
        string _Beneficiary_Address1;
        public string Beneficiary_Address1
        {
            get { return _Beneficiary_Address1; }
            set { _Beneficiary_Address1 = value; }
        }
        string _Beneficiary_PostCode;
        public string Beneficiary_PostCode
        {
            get { return _Beneficiary_PostCode; }
            set { _Beneficiary_PostCode = value; }
        }
        string _cashcollection_flag;
        public string cashcollection_flag
        {
            get { return _cashcollection_flag; }
            set { _cashcollection_flag = value; }
        }
        //int _Agent_MappingID;
        //public int Agent_MappingID
        //{
        //    get { return _Agent_MappingID; }
        //    set { _Agent_MappingID = value; }
        //}
        string _AccountHolderName;
        public string AccountHolderName
        {
            get { return _AccountHolderName; }
            set { _AccountHolderName = value; }
        }
        int _BBank_ID;
        public int BBank_ID
        {
            get { return _BBank_ID; }
            set { _BBank_ID = value; }
        }
        string _Account_Number;
        public string Account_Number
        {
            get { return _Account_Number; }
            set { _Account_Number = value; }
        }
        string _BankCode;
        public string BankCode
        {
            get { return _BankCode; }
            set { _BankCode = value; }
        }
        string _Branch;
        public string Branch
        {
            get { return _Branch; }
            set { _Branch = value; }
        }
        string _Ifsc_Code;
        public string Ifsc_Code
        {
            get { return _Ifsc_Code; }
            set { _Ifsc_Code = value; }
        }
        string _BranchCode;
        public string BranchCode
        {
            get { return _BranchCode; }
            set { _BranchCode = value; }
        }

        double _Min_Amount;
        public double Min_Amount
        {
            get { return _Min_Amount; }
            set { _Min_Amount = value; }
        }
        double _Max_Amount;
        public double Max_Amount
        {
            get { return _Max_Amount; }
            set { _Max_Amount = value; }
        }
        double _Transferfees;
        public double Transferfees
        {
            get { return _Transferfees; }
            set { _Transferfees = value; }
        }
        string _Transfer_ID;
        public string Transfer_ID
        {
            get { return _Transfer_ID; }
            set { _Transfer_ID = value; }
        }

        // for Customer limit

        string _Cust_limit_ID;
        public string Cust_limit_ID
        {
            get { return _Cust_limit_ID; }
            set { _Cust_limit_ID = value; }
        }

        int _Cust_Days;
        public int Cust_Days
        {
            get { return _Cust_Days; }
            set { _Cust_Days = value; }
        }
        double _Personal_Trans_Amt_lmt;
        public double Personal_Trans_Amt_lmt
        {
            get { return _Personal_Trans_Amt_lmt; }
            set { _Personal_Trans_Amt_lmt = value; }
        }

        double _Company_Trans_Amt_lmt;
        public double Company_Trans_Amt_lmt
        {
            get { return _Company_Trans_Amt_lmt; }
            set { _Company_Trans_Amt_lmt = value; }
        }

        //for Cashier limit 
        string _Cashier_limit_ID;
        public string Cashier_limit_ID
        {
            get { return _Cashier_limit_ID; }
            set { _Cashier_limit_ID = value; }
        }
        string _Cashier_limitsupdatedby;
        public string Cashier_limitsupdatedby
        {
            get { return _Cashier_limitsupdatedby; }
            set { _Cashier_limitsupdatedby = value; }
        }
        string _Cashier_Pay_In;
        public string Cashier_Pay_In
        {
            get { return _Cashier_Pay_In; }
            set { _Cashier_Pay_In = value; }
        }
        string _Cashier_Pay_Out;
        public string Cashier_Pay_Out
        {
            get { return _Cashier_Pay_Out; }
            set { _Cashier_Pay_Out = value; }
        }
        string _Cashier_UserRoll_Id;
        public string Cashier_UserRoll_Id
        {
            get { return _Cashier_UserRoll_Id; }
            set { _Cashier_UserRoll_Id = value; }
        }
        //End Cashier Limit




        //int _StateId;
        //public int StateId
        //{
        //    get { return _StateId; }
        //    set { _StateId = value; }
        //}

        //int _CityId;
        //public int CityId
        //{
        //    get { return _CityId; }
        //    set { _CityId = value; }
        //}

        //Add New Employee

        string _User_Id;
        public string User_Id
        {
            get { return _User_Id; }
            set { _User_Id = value; }
        }

        string _Role_ID;
        public string Role_ID
        {
            get { return _Role_ID; }
            set { _Role_ID = value; }
        }
        string _Title_ID;
        public string Title_ID
        {
            get { return _Title_ID; }
            set { _Title_ID = value; }
        }

        string _Address_Emp;
        public string Address_Emp
        {
            get { return _Address_Emp; }
            set { _Address_Emp = value; }
        }

        string _Password_emp;
        public string Password_emp
        {
            get { return _Password_emp; }
            set { _Password_emp = value; }
        }

        int _Delete_Status;
        public int Delete_Status
        {
            get { return _Delete_Status; }
            set { _Delete_Status = value; }
        }

        //string _Record_Insert_DateTime;
        //public string Record_Insert_DateTime
        //{
        //    get { return _Record_Insert_DateTime; }
        //    set { _Record_Insert_DateTime = value; }
        //}

        int _Client_ID;
        public int Client_ID
        {
            get { return _Client_ID; }
            set { _Client_ID = value; }
        }
        int _Agent_MappingID;
        public int Agent_MappingID
        {
            get { return _Agent_MappingID; }
            set { _Agent_MappingID = value; }
        }
        int _SendMoney_Flag;
        public int SendMoney_Flag
        {
            get { return _SendMoney_Flag; }
            set { _SendMoney_Flag = value; }
        }
        int _AllCustomer_Flag;
        public int AllCustomer_Flag
        {
            get { return _AllCustomer_Flag; }
            set { _AllCustomer_Flag = value; }
        }
        int _CB_ID;
        public int CB_ID
        {
            get { return _CB_ID; }
            set { _CB_ID = value; }
        }
        string _City_ID;
        public string City_ID
        {
            get { return _City_ID; }
            set { _City_ID = value; }
        }
        string _Country_ID;
        public string Country_ID
        {
            get { return _Country_ID; }
            set { _Country_ID = value; }
        }

        //Customer transfer limit 
        string _Cust_Trans_Limit_ID; //common to for daily-transfer-limit and Yearly-transfer-limit
        public string Cust_Trans_Limit_ID
        {
            get { return _Cust_Trans_Limit_ID; }
            set { _Cust_Trans_Limit_ID = value; }
        }
        double _Daily_Limit;
        public double Daily_Limit
        {
            get { return _Daily_Limit; }
            set { _Daily_Limit = value; }
        }

        int _Updated_By;   //common to for daily-transfer-limit and Yearly-transfer-limit


        public int Updated_By
        {
            get { return _Updated_By; }
            set { _Updated_By = value; }
        }

        double _Total_TrasferAmt;
        public double Total_TrasferAmt
        {
            get { return _Total_TrasferAmt; }
            set { _Total_TrasferAmt = value; }

        }

        string _Custmer_ID;
        public string Custmer_ID
        {
            get { return _Custmer_ID; }
            set { _Custmer_ID = value; }
        }

        //End Customer transfer limit 

        //Yearly Transfer limit
        double _Month_Wise_Transaction_Limit;
        public double Month_Wise_Transaction_Limit
        {
            get { return _Month_Wise_Transaction_Limit; }
            set { _Month_Wise_Transaction_Limit = value; }
        }
        string _Yearlytrans_Months;
        public string Yearlytrans_Months
        {
            get { return _Yearlytrans_Months; }
            set { _Yearlytrans_Months = value; }
        }
        //End Yearly Transfer limit


        //currency master
        string _Currency_Name;
        public string Currency_Name { get { return _Currency_Name; } set { _Currency_Name = value; } }
        string _Currency_Code;
        public string Currency_Code { get { return _Currency_Code; } set { _Currency_Code = value; } }
        double _Spot_Rate;
        public double Spot_Rate { get { return _Spot_Rate; } set { _Spot_Rate = value; } }
        double _Lowest_Denomination;
        public double Lowest_Denomination { get { return _Lowest_Denomination; } set { _Lowest_Denomination = value; } }
        int _ShowOnWebsite;
        public int ShowOnWebsite { get { return _ShowOnWebsite; } set { _ShowOnWebsite = value; } }
        double _Currency_Limit;
        public double Currency_Limit { get { return _Currency_Limit; } set { _Currency_Limit = value; } }
        string _ImageName;
        public string ImageName { get { return _ImageName; } set { _ImageName = value; } }
        //end


        //country master
        string _Country_code;
        public string Country_code { get { return _Country_code; } set { _Country_code = value; } }
        string _Country_Currency;
        public string Country_Currency { get { return _Country_Currency; } set { _Country_Currency = value; } }
        string _Country_Flag;
        public string Country_Flag { get { return _Country_Flag; } set { _Country_Flag = value; } }
        int _sending_Flag;
        public int sending_Flag { get { return _sending_Flag; } set { _sending_Flag = value; } }
        //end

        //paymenttype master
        int _PType_ID;
        public int PType_ID { get { return _PType_ID; } set { _PType_ID = value; } }
        string _PType;
        public string PType { get { return _PType; } set { _PType = value; } }
        double _Max_Amount_Limit;
        public double Max_Amount_Limit { get { return _Max_Amount_Limit; } set { _Max_Amount_Limit = value; } }
        string _Review_Transfer_Message;
        public string Review_Transfer_Message { get { return _Review_Transfer_Message; } set { _Review_Transfer_Message = value; } }
        int _Payment_Type_Src;
        public int Payment_Type_Src { get { return _Payment_Type_Src; } set { _Payment_Type_Src = value; } }
        //end

        //agent mapping
        double _Current_Balance_In_GBP;
        public double Current_Balance_In_GBP { get { return _Current_Balance_In_GBP; } set { _Current_Balance_In_GBP = value; } }
        double _USD_Amount;
        public double USD_Amount { get { return _USD_Amount; } set { _USD_Amount = value; } }
        double _Foreign_Currency_Amount;
        public double Foreign_Currency_Amount { get { return _Foreign_Currency_Amount; } set { _Foreign_Currency_Amount = value; } }
        double _PayIn_Amount;
        public double PayIn_Amount { get { return _PayIn_Amount; } set { _PayIn_Amount = value; } }
        double _PayOut_Amount;
        public double PayOut_Amount { get { return _PayOut_Amount; } set { _PayOut_Amount = value; } }
        //end

        //id master
        string _ID_Name;
        public string ID_Name { get { return _ID_Name; } set { _ID_Name = value; } }
        int _IDType_ID;
        public int IDType_ID { get { return _IDType_ID; } set { _IDType_ID = value; } }
        //end


        //user role add
        string _role_name;
        public string role_name { get { return _role_name; } set { _role_name = value; } }
        int _Module_ID;
        public int Module_ID
        {
            get { return _Module_ID; }
            set { _Module_ID = value; }
        }
        int _SubModule_ID;
        public int SubModule_ID
        {
            get { return _SubModule_ID; }
            set { _SubModule_ID = value; }
        }
        //end

        //delivery type master
        int _DeliveryType_Id;
        public int DeliveryType_Id { get { return _DeliveryType_Id; } set { _DeliveryType_Id = value; } }
        string _Delivery_Type;
        public string Delivery_Type { get { return _Delivery_Type; } set { _Delivery_Type = value; } }
        //end

        //customerwise_limit
        double _Minimum_Transfer_Amount;
        public double Minimum_Transfer_Amount { get { return _Minimum_Transfer_Amount; } set { _Minimum_Transfer_Amount = value; } }

        double _Maximum_Transfer_Amount;
        public double Maximum_Transfer_Amount { get { return _Maximum_Transfer_Amount; } set { _Maximum_Transfer_Amount = value; } }
        //end


        //customer tranfer limit
        double _Transaction_Limit;
        public double Transaction_Limit { get { return _Transaction_Limit; } set { _Transaction_Limit = value; } }
        int _Transaction_Count;
        public int Transaction_Count { get { return _Transaction_Count; } set { _Transaction_Count = value; } }
        int _Customer_Status;
        public int Customer_Status { get { return _Customer_Status; } set { _Customer_Status = value; } }
        int _Months_For_Limit;
        public int Months_For_Limit { get { return _Months_For_Limit; } set { _Months_For_Limit = value; } }
        //end

        //transfer limit
        double _transfer_amount;
        public double transfer_amount { get { return _transfer_amount; } set { _transfer_amount = value; } }

        int _months;
        public int months
        {
            get { return _months; }
            set { _months = value; }
        }
        double _month_wise_trn_amt;
        public double month_wise_trn_amt { get { return _month_wise_trn_amt; } set { _month_wise_trn_amt = value; } }
        string _update_date;
        public string update_date { get { return _update_date; } set { _update_date = value; } }
        //end

        //request top up
        int _Agent_Id;
        public int Agent_Id { get { return _Agent_Id; } set { _Agent_Id = value; } }
        double _Request_Amount;
        public double Request_Amount { get { return _Request_Amount; } set { _Request_Amount = value; } }
        int _Payment_Method;
        public int Payment_Method { get { return _Payment_Method; } set { _Payment_Method = value; } }
        double _Approved_Amount;
        public double Approved_Amount { get { return _Approved_Amount; } set { _Approved_Amount = value; } }
        string _comments;
        public string comments { get { return _comments; } set { _comments = value; } }
        int _Approved_by;
        public int Approved_by { get { return _Approved_by; } set { _Approved_by = value; } }
        string _Approved_Date;
        public string Approved_Date { get { return _Approved_Date; } set { _Approved_Date = value; } }
        string _Requested_Date;
        public string Requested_Date { get { return _Requested_Date; } set { _Requested_Date = value; } }
        int _Payment_Status;
        public int Payment_Status { get { return _Payment_Status; } set { _Payment_Status = value; } }
        int _Topup_Status;
        public int Topup_Status { get { return _Topup_Status; } set { _Topup_Status = value; } }
        string _Agent_name;
        public string Agent_name { get { return _Agent_name; } set { _Agent_name = value; } }
        string _from_date;
        public string from_date { get { return _from_date; } set { _from_date = value; } }
        string _to_date;
        public string to_date { get { return _to_date; } set { _to_date = value; } }
        string _topup_ref;
        public string topup_ref { get { return _topup_ref; } set { _topup_ref = value; } }
        decimal _GBP_Bal;
        public decimal GBP_Bal { get { return _GBP_Bal; } set { _GBP_Bal = value; } }
        decimal _USD_Bal;
        public decimal USD_Bal { get { return _USD_Bal; } set { _USD_Bal = value; } }
        decimal _Foreign_Currency_Bal;
        public decimal Foreign_Currency_Bal { get { return _Foreign_Currency_Bal; } set { _Foreign_Currency_Bal = value; } }
        string _Last_updated_date;
        public string Last_updated_date { get { return _Last_updated_date; } set { _Last_updated_date = value; } }
        //end 
        //email configuraation 
        int _Email_ID_Config; public int Email_ID_Config { get { return _Email_ID_Config; } set { _Email_ID_Config = value; } }
        int _Port_Config; public int Port_Config { get { return _Port_Config; } set { _Port_Config = value; } }
        string _Email_Convey_from__Config; public string Email_Convey_from__Config { get { return _Email_Convey_from__Config; } set { _Email_Convey_from__Config = value; } }
        string _Host__Config; public string Host__Config { get { return _Host__Config; } set { _Host__Config = value; } }
        int _Priority__Config; public int Priority__Config { get { return _Priority__Config; } set { _Priority__Config = value; } }
        string _Password__Config; public string Password__Config { get { return _Password__Config; } set { _Password__Config = value; } }

        #region registration

        int _Cus_email; public int Cus_email { get { return _Cus_email; } set { _Cus_email = value; } }
        int _Cus_email_man; public int Cus_email_man { get { return _Cus_email_man; } set { _Cus_email_man = value; } }

        int _Cus_con_email; public int Cus_con_email { get { return _Cus_con_email; } set { _Cus_con_email = value; } }
        int _Cus_con_email_man; public int Cus_con_email_man { get { return _Cus_con_email_man; } set { _Cus_con_email_man = value; } }

        int _Cus_pass; public int Cus_pass { get { return _Cus_pass; } set { _Cus_pass = value; } }
        int _Cus_pass_man; public int Cus_pass_man { get { return _Cus_pass_man; } set { _Cus_pass_man = value; } }

        int _Cus_con_pass; public int Cus_con_pass { get { return _Cus_con_pass; } set { _Cus_con_pass = value; } }
        int _Cus_con_pass_man; public int Cus_con_pass_man { get { return _Cus_con_pass_man; } set { _Cus_con_pass_man = value; } }

        int _Ref_code; public int Ref_code { get { return _Ref_code; } set { _Ref_code = value; } }
        int _Ref_code_man; public int Ref_code_man { get { return _Ref_code_man; } set { _Ref_code_man = value; } }

        int _cust_title; public int cust_title { get { return _cust_title; } set { _cust_title = value; } }
        int _cust_title_man; public int cust_title_man { get { return _cust_title_man; } set { _cust_title_man = value; } }

        int _cust_first; public int cust_first { get { return _cust_first; } set { _cust_first = value; } }
        int _cust_first_man; public int cust_first_man { get { return _cust_first_man; } set { _cust_first_man = value; } }

        int _cust_middle; public int cust_middle { get { return _cust_middle; } set { _cust_middle = value; } }
        int _cust_middle_man; public int cust_middle_man { get { return _cust_middle_man; } set { _cust_middle_man = value; } }

        int _cust_last; public int cust_last { get { return _cust_last; } set { _cust_last = value; } }
        int _cust_last_man; public int cust_last_man { get { return _cust_last_man; } set { _cust_last_man = value; } }

        int _cust_dob; public int cust_dob { get { return _cust_dob; } set { _cust_dob = value; } }
        int _cust_dob_man; public int cust_dob_man { get { return _cust_dob_man; } set { _cust_dob_man = value; } }

        int _cust_mobile; public int cust_mobile { get { return _cust_mobile; } set { _cust_mobile = value; } }
        int _cust_mobile_man; public int cust_mobile_man { get { return _cust_mobile_man; } set { _cust_mobile_man = value; } }

        int _post_code; public int post_code { get { return _post_code; } set { _post_code = value; } }
        int _post_code_man; public int post_code_man { get { return _post_code_man; } set { _post_code_man = value; } }

        int _house_no; public int house_no { get { return _house_no; } set { _house_no = value; } }
        int _house_no_man; public int house_no_man { get { return _house_no_man; } set { _house_no_man = value; } }

        int _Add1; public int Add1 { get { return _Add1; } set { _Add1 = value; } }
        int _Add1_man; public int Add1_man { get { return _Add1_man; } set { _Add1_man = value; } }

        int _Add2; public int Add2 { get { return _Add2; } set { _Add2 = value; } }
        int _Add2_man; public int Add2_man { get { return _Add2_man; } set { _Add2_man = value; } }

        int _cust_country; public int cust_country { get { return _cust_country; } set { _cust_country = value; } }
        int _cust_country_man; public int cust_country_man { get { return _cust_country_man; } set { _cust_country_man = value; } }

        int _cust_city; public int cust_city { get { return _cust_city; } set { _cust_city = value; } }
        int _cust_city_man; public int cust_city_man { get { return _cust_city_man; } set { _cust_city_man = value; } }

        int _cust_nation; public int cust_nation { get { return _cust_nation; } set { _cust_nation = value; } }
        int _cust_nation_man; public int cust_nation_man { get { return _cust_nation_man; } set { _cust_nation_man = value; } }

        int _Emp_status; public int Emp_status { get { return _Emp_status; } set { _Emp_status = value; } }
        int _Emp_status_man; public int Emp_status_man { get { return _Emp_status_man; } set { _Emp_status_man = value; } }

        int _cust_prof; public int cust_prof { get { return _cust_prof; } set { _cust_prof = value; } }
        int _cust_prof_man; public int cust_prof_man { get { return _cust_prof_man; } set { _cust_prof_man = value; } }

        int _Comp_name; public int Comp_name { get { return _Comp_name; } set { _Comp_name = value; } }
        int _Comp_name_man; public int Comp_name_man { get { return _Comp_name_man; } set { _Comp_name_man = value; } }

        int _Heard_from; public int Heard_from { get { return _Heard_from; } set { _Heard_from = value; } }
        int _Heard_from_man; public int Heard_from_man { get { return _Heard_from_man; } set { _Heard_from_man = value; } }

        #endregion

        //SMS:
        int _cashbackid; public int cashbackid { get { return _cashbackid; } set { _cashbackid = value; } }
        int _offertypeid; public int offertypeid { get { return _offertypeid; } set { _offertypeid = value; } } //1:Cashback 2: reward
        int _cashbackagainsttypeid; public int cashbackagainsttypeid { get { return _cashbackagainsttypeid; } set { _cashbackagainsttypeid = value; } }  //1:Fee against 2:Amount Against   (discounttype)
        int _cashbackamounttypeid; public int cashbackamounttypeid { get { return _cashbackamounttypeid; } set { _cashbackamounttypeid = value; } }  //1: Percentage 2:fixed
        int _allow_reward_scheme; public int allow_reward_scheme { get { return _allow_reward_scheme; } set { _allow_reward_scheme = value; } }
        int _reward_after_every_N_transaction; public int reward_after_every_N_transaction { get { return _reward_after_every_N_transaction; } set { _reward_after_every_N_transaction = value; } }
        double _cashback_value; public double cashback_value { get { return _cashback_value; } set { _cashback_value = value; } }
        int _transactionid; public int transaction_id { get { return _transactionid; } set { _transactionid = value; } }



    }
}
