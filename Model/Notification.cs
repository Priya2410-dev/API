namespace Calyx_Solutions.Model
{
    public class Notification
    {
        public string Message { set; get; }
        public string Customer_Id { set; get; }
        public int Module_Id { set; get; }
        public string Tocheck { set; get; }
        public string Delete_Status { set; get; }
        public DateTime Records_Insert_Date { set; get; }

        public int Client_Id { set; get; }
        public int Branch_Id { set; get; }
        public Login Login { set; get; }
    }
}
