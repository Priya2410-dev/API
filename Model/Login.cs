using System.ComponentModel.DataAnnotations;

namespace Calyx_Solutions.Model
{    
    public class Login
    {
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string deviceId { get; set; }
        public string ipAddress { get; set; }
        public string flutterval { get; set; }
        

        public string CredentialFlag { get; set; }
        public string Resend_otp { get; set; }
        public string New_Agent_Branch { get; set; }
        public string Agent_branch { get; set; }
        public string Captcha { get; set; }
        public string token { get; set; }

        
        public string Base_Currency_code { get; set; }
        public string Agent_User_ID { get; set; }
        public int flag { get; set; }
        public string date { get; set; }

        public string APIUser_ID { set; get; }
        public string APIAccess_Code { set; get; }
        public string WireTransferRefNumber { set; get; }
        public string Customer_ID { set; get; }
        public int Client_ID { set; get; }
        public int Branch_ID { set; get; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string UserName { get; set; }

        public string Passcode { get; set; }
        public string SecurityKey { get; set; }
        public Customer Customer { get; set; }
         
        public int DocUploadCount { get; set; }
        public string FullName { get; set; }

        [Range(1, 100)]
        [Required]
        public int User_ID { get; set; }
        public string Location { get; set; }
        public string IP_Address { get; set; }
        public string Login_DateTime { get; set; }
        public string Login_Time { get; set; }
        public int Mobile_Verification_flag { get; set; }

        //281122
        public int Currency_Id { get; set; }
        public string Currency_Code { get; set; }
        public string Country_Flag { get; set; }
        public string Currency_Sign { get; set; }
        public string Country_Name { get; set; }

        public int Country_Id { get; set; }
        public string OTP { get; set; }



        //31-1-24
        public string tokenValue { get; set; }
        public string tokenRequest { get; set; }
        public string userAgent { get; set; }
    }
}
