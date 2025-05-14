namespace Calyx_Solutions.Model
{
    public class Dashboard
    {
        public int flag { get; set; }
        public string date { get; set; }
        public double Sending_Amount { set; get; }
        public int Transaction_count { set; get; }
        public int Benf_count { set; get; }
        public string Customer_ID { set; get; }
        public int User_ID { set; get; }
        public int Client_ID { set; get; }
        public int Branch_ID { set; get; }       
        public int Id { set; get; } //Digvijay changes for delete notification
        public int status { set; get; }
        public int Notification_Flag { set; get; }

        public string Deleted_By { set; get; }  //niranjan changes for delete system notification 22/11/2024 
    }
}
