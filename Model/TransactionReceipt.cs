namespace Calyx_Solutions.Model
{
    public class TransactionReceipt
    {
        public int User_ID { get; set; }
        public int Branch_ID { get; set; }
        public int branchID { get; set; }
        public int CB_ID { get; set; }
        public int Transaction_ID { get; set; }
        public int Client_ID { set; get; }
        public int clientID { set; get; }
        //public int? Client_ID { get; set; }
        //public int? Branch_ID { get; set; }
        public string? customerid { get; set; }
        public string? is_uploaded { get; set; }

        //public int? Transaction_ID { get; set; }
        public string? Customer_ID { get; set; }
        public int? MadeThisTransfer_Flag { get; set; }
        //public int? CB_ID { get; set; }

        public string? Record_Insert_DateTime { get; set; }
        public int? Delete_Status { get; set; }

        public string? ReceiptNameWithExt { get; set; }
        public string? MadeThisTransfer_Label { get; set; }
        public int? transactionID { get; set; }
        public string? btnValue { get; set; }
    }



}