using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calyx_Solutions.Model
{
    public class CompanyDetails
    {
        public string APIUser_ID { set; get; }
        public string APIAccess_Code { set; get; }
        public string APIAccessCode { set; get; }
        public int ResponseCode { set; get; }
        public string ResponseMessage { set; get; }

        public Int32 Id { set; get; }
        public string Company_Name { set; get; }
        public string Company_Address { set; get; }
        public string Company_URL_Admin { set; get; }
        public string Company_URL_Customer { set; get; }
        public string HouseNumber { set; get; }
        public string Address { set; get; }
        public string CompanyPhone_One { set; get; }
        public string CompanyPhone_two { set; get; }
        public string Company_mobile { set; get; }
        public string Company_Email { set; get; }
        public string Company_Postode { get; set; }
        public string CompanyHouse_Number { get; set; } //public string BaseCurrency_Country { get; set; }

        public string PostCode { set; get; }
        public string Fax { set; get; }
        public string Email { set; get; }
        public string NearestTubeStation { set; get; }
        public string OtherComments { set; get; }
        public string Image { set; get; }
        public DateTime RecordInsertDate { set; get; }
        public DateTime RecordUpdateDate { set; get; }
        public Int32 RecordUpdateById { set; get; }
        public Int32 DeleteStatus { set; get; }
        public int ExpiryMonths { get; set; }

        public string BankName { set; get; }
        public string AccountHolderName { set; get; }
        public DateTime ExtraThree { set; get; }
        public string AccountNumber { set; get; }
        public string Sort_ID { set; get; }
        public string AdminMail { set; get; }
        public string Branch { set; get; }
        public string IBAN { set; get; }

        public string BIC { set; get; }
        public string CustRefIntialChar { set; get; }
        public string TransferRefIntialChar { set; get; }
        public string Company_website { set; get; }
        public string Password { set; get; }
        public string Port { set; get; }
        public string Host { set; get; }
        //public Currency Currency { set; get; }
        public string Mobile { set; get; }
        public string BrandColor { set; get; }
        public string SecondColor { set; get; }
        public string Whats_app_no { set; get; }//public string WhatsNo { set; get; }

        public string Twitter { set; get; }
        public string Facebook { set; get; }
        public string Gmail { set; get; }

        //public Client Client { set; get; }
        public string EmailConveyFrom { set; get; }
        public string privacy_policy { set; get; }
        public string Terms_Condtions { get; set; }
        //Base Currency Details
        public string BaseCurrency_Timezone { get; set; }
        public string BaseCurrency_Sign { get; set; }
        public string BaseCurrency_Country { get; set; }
        public string BaseCountry_ID { get; set; }
        public string BaseCurrency_Code { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
    }
}
