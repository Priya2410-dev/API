using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Calyx_Solutions.Model
{
    public class Beneficiary
    {
        public int blacklisted { get; set; }
        public int Benf_BankDetails_ID { get; set; }
        public int sectionvalue { get; set; }
        public string Wallet_Id { get; set; }
        public int Collection_type_Id { get; set; }
        public int Mobile_provider { get; set; }
        public int Wallet_provider { get; set; }
        public string Beneficiary_State { set; get; }        
        
        public int BankBranch_ID { get; set; }
        public string First_Name { get; set; }
        public  int Beneficiary_Gender { get; set; }

        public string Middle_Name { get; set; }
        public string Last_Name { get; set; }

        public int Relation_ID { get; set; }
        public int CommentUserId { get; set; }
        public int per_status { get; set; }
        public int status { get; set; }
        public int Id1 { get; set; }
        public int DeleteStatus { get; set; }
        public DateTime RecordDate { get; set; }
        public string whereclause { get; set; }
        public int ExistBeneficiary { set; get; }
        public int Id { get; set; }

        //Other Files
        public Login Login { set; get; }
        public Customer Customer { set; get; }
        public int Pid { set; get; }
        public string Full_name { set; get; }
        public string WireTransfer_ReferanceNo { set; get; }
        public int Sending_Flag { set; get; }

        //Beneficiary Bank Details
        public string AccountHolderName { set; get; }
        public int BBank_ID { set; get; }
        public string Account_Number { set; get; }
        public string Bank_Name { set; get; }
        public string Branch { set; get; }
        public string Confirm_Account_Number { set; get; }
        public string BankCode { set; get; }
        public string Ifsc_Code { set; get; }
        public string BranchCode { set; get; }
        public string City_Name { set; get; }
        public string Country_Name { set; get; }

        //Beneficiary Personal Details
        public int Beneficiary_ID { set; get; }

        [Required]
        public string Beneficiary_Name { set; get; }
        public string Beneficiary_Address { set; get; }
        public int Beneficiary_City_ID { set; get; }
        public int Beneficiary_Country_ID { set; get; }
        public string Beneficiary_Telephone { set; get; }
        public string Beneficiary_Mobile { set; get; }
        public DateTime Record_Insert_DateTime { set; get; }
        public int Delete_Status { set; get; }
        public int Created_By_User_ID { set; get; }
        public string Customer_ID { set; get; }
        public string Beneficiary_Address1 { set; get; }
        public string Beneficiary_PostCode { set; get; }
        public int cashcollection_flag { set; get; }
        public int Agent_MappingID { set; get; }
        public int Branch_ID { set; get; }
        public int Client_ID { set; get; }

        //Beneficiary Bank Validations
        public int Country_ID { set; get; }
        public int Bank_Code { set; get; }
        public int IFSC { set; get; }
        public int Branch_Code { set; get; }
        public int Verify_Account_no { set; get; }
        public int Acc_no_length { set; get; }

        public string Message { get; set; }

        public int User_ID { get; set; }

        public string Benf_Iban { get; set; }
        public string Benf_BIC { get; set; }

        public string Birth_Date { get; set; }
        public string userAgent { get; set; }

        //******************
        public string beneficiaryCountry { get; set; }
        public string beneficiaryCollectionType { get; set; }
        public string foreignCurrencyCode { get; set; }
        public int BBDetails_ID { get; set; } //Parvej changes for update bank

public string Beneficiary_Email { get; set; }//sanket 091124
        public DateTime Medicare_Expiry_Date { get; set; }
        public int Provience_id { get; set; }
        public int LGA_Id { get; set; }
        public int Address_Flag { get; set; }
        public int Medicare_Benef { get; set; }
        public string Country_Code { get; set; }
    }
}
