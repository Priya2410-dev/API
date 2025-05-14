namespace Calyx_Solutions.Model
{
    public class Doc
    {
        public int? Place_Of_ID { get; set; }
        public string? DocumentName { get; set; }
        public string? contains_file { get; set; }
        public string? contains_backfile { get; set; }
        public string? documentname { get; set; }
        public string? Comments { get; set; }
        public string? comments { get; set; }
        public string? Sender_DateOfBirth { get; set; }
        public string? senderdateofbirth { get; set; }
        public int? IDName_ID { get; set; }
        public int? nameID { get; set; }

        public int? IDType_ID { get; set; }
        public int? typeID { get; set; }

        public string? tokenValue { get; set; }
        public string? SenderNameOnID { get; set; }
        public string? sendernameonid { get; set; }

        public string? SenderID_Number { get; set; }
        public string? senderidnumber { get; set; }


        public string? SenderID_ExpiryDate { get; set; }
        public string? senderidexpirydate { get; set; }

        public string? Customer_ID { get; set; }
        public string? customerid { get; set; }



        public string? shufti_status { set; get; }
        public string? background_checks { set; get; }
        public int? Collection_type { get; set; }

        public string? UserName { set; get; }  //Used for encrypted customer id -ID Scan
        public string? JourneyID { get; set; }
        public int Country_ID { get; set; }
        public int countryID { get; set; }
       // public int? Beneficiary_ID { get; set; }

        public int Beneficiary_ID { get; set; }

        public int BenfID { get; set; }
        public string? Issue_date { get; set; }
        public string? issuedate { get; set; }
        //public Login Login { set; get; }
        public int Client_ID { set; get; }
        public int clientID { set; get; }
        public int Branch_ID { set; get; }
        public int branchID { set; get; }

        public int? SenderID_ID { set; get; }

        public string? ID_Type { set; get; }

        public string? ID_Name { set; get; }
        public string? Verified_By { set; get; }
        public DateTime Verified_Date { set; get; }
        public string? SenderID_PlaceOfIssue { set; get; }
        public string? senderidplaceofissue { set; get; }


        public string? Record_Insert_DateTime { set; get; }
        public string? FileNameWithExt { set; get; }
        public string? secondaryFileNameWithExt { set; get; }


        public string? Path { set; get; }
        public DateTime RecordDate { set; get; }
        // public Company Company { set; get; }
        //public Customer Customer { set; get; }
        public int? Delete_Status { get; set; }

        
        public int User_ID { get; set; }
        public int userID { get; set; }

        public string? MRZ_number { get; set; }
        public string? MRZ_number_Second { get; set; }
        

        public int? ExistId { get; set; }
        public string? Message { get; set; }
        public int? Permission_status { get; set; }


        public string? ShuftiId { set; get; }
        public string? shuftipro_face { set; get; }
        public string? shuftipro_address { set; get; }
        public string? shuftipro_frontdoc { set; get; }
        public string? shuftipro_backdoc { set; get; }
        public string? shuftipro_document_proof_dob { set; get; }
        public string? shuftipro_document_proof_gender { set; get; }
        public string? shuftipro_document_proof_full_name { set; get; }
        public string? shuftipro_document_first_name { set; get; }
        public string? shuftipro_document_middle_name { set; get; }
        public string? shuftipro_document_last_name { set; get; }
        public string? shuftipro_document_proof_document_country { set; get; }
        public string? shuftipro_document_proof_document_number { set; get; }
        public string? shuftipro_document_proof_document_country_code { set; get; }
        public string? shuftipro_document_proof_document_official_name { set; get; }
        public string? shuftipro_doc_age { set; get; }
        public string? shuftipro_doc_full_address { set; get; }
        public string? shuftipro_doc_gender { set; get; }
        public string? shuftipro_doc_selected_type { set; get; }
        public string? shuftipro_doc_face_match_confidence { set; get; }
        public string? shuftipro_doc_issue_date { set; get; }
        public string? shuftipro_doc_expiry_date { set; get; }
        public string? shuftipro_face_on_document_matched { set; get; }
        public string? shuftipro_face_available { set; get; }
        public string? shuftipro_frontdoc_available { set; get; }
        public string? shuftipro_frontdoc_expired { set; get; }
        public string? Custmer_Ref { set; get; }

        public DateTime From_Date { get; set; }
        public DateTime To_Date { get; set; }

    }



}