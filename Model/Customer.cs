using System.Diagnostics.Metrics;

namespace Calyx_Solutions.Model
{
    public class Customer
    {
        public string Agent_branch { get; set; }
        public string New_Agent_Branch { get; set; }
        public string Referrer_Flag { get; set; }
        public int Referred_By_Agent { get; set; }
        public string Agent_User_ID { get; set; }
        public string payout_countries { get; set; }

        public string customer_profileimage { get; set; }
        public string Annual_Salary { set; get; }
        public int step_flag { get; set; }
        public string Countryof_Birth { set; get; }
        public string RatingsArray { get; set; }
        public string basecur_Amount { get; set; }
        public string basecur_purpose { get; set; }
        public string ErrorFlag { get; set; }
        public string checkparam { get; set; }
        public int chk_promo_mail { get; set; }
        public int base_currency_id { get; set; }
        public int base_currency_code { get; set; }
        public string Phone_number_code { get; set; }
        public string Mobile_number_code { get; set; }
        public string chk_changed { get; set; }
        public int Perm_status { get; set; }
        public string Birth_Date { get; set; }
        public int Employement_Status { get; set; }
        public string AllCustomer_Flag { get; set; }
        public string OTP { get; set; }

        public int Delete_Status { get; set; }

        public string cashcollection_flag { get; set; }

        public string Record_Insert_DateTime { get; set; }

        public object newpassword { get; set; }

        public string oldpassword { get; set; }
        public string Captcha { get; set; }
        
        public int customerRegStep { set; get; }
        public int Client_ID { set; get; }
        public int Branch_ID { set; get; }
        public int TitleId { set; get; }
        public int Annual_salary_ID { set; get; }

      

        public string Id { set; get; }
        public string FirstName { set; get; }
        public string Middle_Name { set; get; }
        public string LastName { set; get; }
        public string Full_Name { set; get; }
        public string Name { set; get; }
     
        public string PostCode { set; get; }
        public string PhoneNumber { set; get; }
        public string MobileNumber { set; get; }
        public string Email { set; get; }
        public string Password { set; get; }
        public int SecurityQuestionId { set; get; }
        public string SecurityQuestionAnswer { set; get; }
        public Int32 DeleteStatus { set; get; }
        public int block_login_flag { get; set; }
        public int provience_id { set; get; }
        public int professionId { set; get; }
        public string professionName { set; get; }
        public int cityId { set; get; }
        
        public Int32 RegularCustomerId { set; get; }
        public string HouseNumber { set; get; }
        public string Street { set; get; }
        public string Gender { set; get; }

        public string AddressLine2 { set; get; }
        public string birthPlace { set; get; }
        
        public Int32 AgentMappingId { set; get; }
        public string Nationality { set; get; }
        public Int32 Nationality_ID { set; get; }
        public string WireTransfer_ReferanceNo { set; get; }
        public string WireTransferRefNumber { set; get; }
        public Int32 SendMoneyFlag { set; get; }
        public Int32 VerificationFlag { set; get; }

        public Decimal ExceededAmount { set; get; }

        public DateTime InactivateDate { set; get; }

     
        public string CompanyName { set; get; }
        public float CustLimit { set; get; }
        public int HeardFromId { set; get; }
        public string HeardFromEvent { set; get; }
        public string SourseOfRegistration { set; get; }
        public string Comment { set; get; }

        public int CommentUserId { set; get; }
        public int RemindMeFlag { set; get; }
        public DateTime Remind_Date { set; get; }

        public DateTime RecordDate { set; get; }
 

        public Int32 HeardFrom { set; get; }

        public Int32 DocumentUploadCount { set; get; }
        public int ExistMobile { get; set; }
        public int ExistEmail { get; set; }

        public int IncompleteRegistrationId { get; set; }
        public string SecurityKey { get; set; }
        public string APIAccess_Code { get; set; }
        public string APIUser_ID { get; set; }

        public int per_status { get; set; }
        public int per_status_verifyMob { get; set; }

        public int Id1 { get; set; }

        public int status { get; set; }
        public string Message { get; set; }
        public string ExtraGBG { get; set; }
        public string whereclause { get; set; }

        public string refcode { get; set; }

        public object Customer_ID { get; set; }

        public int Uppercase { get; set; }

        public decimal Lowercase { get; set; }

        public decimal Digit { get; set; }

        public int Maxpass_Length { get; set; }

        public int Minpass_length { get; set; }

        public string Special_char { get; set; }

        public int Isspecial_Char { get; set; }

        public string UserName { get; set; }

        public int Flag { get; set; }

        public int User_ID { get; set; }

        public int Retry_Cnt { get; set; }

        public int Mobile_Verification_flag { get; set; }

        //281122
        public int Country_Id { get; set; }
        public int Currency_Id { get; set; }
        public string Currency_Code { get; set; }
        public string Country_Flag { get; set; }
        public string Currency_Sign { get; set; }
        public string Country_Name { get; set; }
        //281122
        public int passcode_flag { get; set; }

        //sanket 071124
        public int Membership_id { get; set; }
        public int Benefit_ID { get; set; }
        public string Benefit_Name { get; set; }
        public string Delivery_Date { get; set; }
        public string Delivery_Address { get; set; }
        public string Cake_Flavour_ID { get; set; }
        public string Cake_Diet_ID { get; set; }
        public string SuperStore_ID { get; set; }
        public string EntertainmentVoucher_ID { get; set; }

        public int popup_read_flag { get; set; }
        //sanket 071124
    }
}
