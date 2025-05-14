using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calyx_Solutions.Model
{
    public class DocumentDetails
    {
        public int Id { get; set; }
        public int SenderId { get; set; }
        public string SenderName { get; set; }
        //public IDType IDType { get; set; }
        public string SenderNumber { get; set; }
        public DateTime SenderExpiryDate { get; set; }
        public string SenderPlaceOfIssue { get; set; }
        public string SecondaryFileNameWithExt { get; set; }
        public DateTime SenderDateOfBirth { get; set; }
        //public Customer Customer { get; set; }
        //public IDName IDName { get; set; }
        public int SecondayIdNameId { get; set; }
        public string SenderIdNumber { get; set; }
        public string SenderIdNumber2 { get; set; }

        
        public DateTime RecordInsert { get; set; }
        public int DeleteStatus { get; set; }
        public int CreateByUserId { get; set; }
        public int DocumentDetailsId { get; set; }
        //public Branch Branch { get; set; }
        public string FileNameWithExt { get; set; }
        public string VerifiedBy { get; set; }
        public DateTime VerifiedDate { get; set; }
        public int Verified { get; set; }
        //public Client Client { get; set; }
        public string OtherDocumentName { get; set; }
        public string Comments { get; set; }
    }
}
