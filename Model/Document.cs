namespace Calyx_Solutions.Model
{
    public class Document
    {
        public string shufti_status { set; get; }
        public string background_checks { set; get; }
        public int Collection_type { get; set; }
        public string checkparam { set; get; }
        public string UserName { set; get; }  //Used for encrypted customer id -ID Scan
        public string JourneyID { get; set; }
        public int Country_ID { get; set; }
        public int Beneficiary_ID { get; set; }
        public string Issue_date { get; set; }
        public Login Login { set; get; }
        public int Client_ID { set; get; }
        public int Branch_ID { set; get; }

        public int SenderID_ID { set; get; }
        public string SenderNameOnID { set; get; }
        public string SenderID_Number { set; get; }
        public int IDType_ID { set; get; }
        public string ID_Type { set; get; }
        public int IDName_ID { set; get; }
        public string ID_Name { set; get; }
        public string Verified_By { set; get; }
        public DateTime Verified_Date { set; get; }
        public string SenderID_PlaceOfIssue { set; get; }
        public string Sender_DateOfBirth { set; get; }
        public string MRZ_number { get; set; }
        public string MRZ_number_Second { get; set; }
        public string SenderID_ExpiryDate { set; get; }
        public string Record_Insert_DateTime { set; get; }
        public string FileNameWithExt { set; get; }
        public string secondaryFileNameWithExt { set; get; }

        public string DocumentName { set; get; }
        public string Path { set; get; }
        public DateTime RecordDate { set; get; }
        public CompanyDetails Company { set; get; }
        public Customer Customer { set; get; }
        public int Delete_Status { get; set; }
        public string Customer_ID { get; set; }
        public int User_ID { get; set; }
        public string Comments { get; set; }

        public int ExistId { get; set; }
        public string Message { get; set; }
        public int Permission_status { get; set; }


        public string ShuftiId { set; get; }
        public string shuftipro_face { set; get; }
        public string shuftipro_address { set; get; }
        public string shuftipro_frontdoc { set; get; }
        public string shuftipro_backdoc { set; get; }
        public string shuftipro_document_proof_dob { set; get; }
        public string shuftipro_document_proof_gender { set; get; }
        public string shuftipro_document_proof_full_name { set; get; }
        public string shuftipro_document_first_name { set; get; }
        public string shuftipro_document_middle_name { set; get; }
        public string shuftipro_document_last_name { set; get; }
        public string shuftipro_document_proof_document_country { set; get; }
        public string shuftipro_document_proof_document_number { set; get; }
        public string shuftipro_document_proof_document_country_code { set; get; }
        public string shuftipro_document_proof_document_official_name { set; get; }
        public string shuftipro_doc_age { set; get; }
        public string shuftipro_doc_full_address { set; get; }
        public string shuftipro_doc_gender { set; get; }
        public string shuftipro_doc_selected_type { set; get; }
        public string shuftipro_doc_face_match_confidence { set; get; }
        public string shuftipro_doc_issue_date { set; get; }
        public string shuftipro_doc_expiry_date { set; get; }
        public string shuftipro_face_on_document_matched { set; get; }
        public string shuftipro_face_available { set; get; }
        public string shuftipro_frontdoc_available { set; get; }
        public string shuftipro_frontdoc_expired { set; get; }

        public string scanstatus { set; get; }
        public string CustomerName { set; get; }
        public string uniqueId { set; get; }


        public string applicant_id { get; set; }
        public string Custmer_Ref { get; set; }
        public int Place_Of_ID { get; set; }
        public string post_code { get; set; }
        public string Adderess { get; set; }
        public string strete { get; set; }
        public string city { get; set; }
        public string country_code { get; set; }
        public string customer_email { get; set; }
        public string customer_mobile { get; set; }

        public DateTime From_Date { get; set; }
        public DateTime To_Date { get; set; }
    }
}
