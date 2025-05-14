namespace Calyx_Solutions.Model
{
    public class Services
    {
        public string Services_flag { get; set; }
        public DateTime RecordDate { set; get; }
        public int Client_ID { get; set; }
        public int User_ID { get; set; }

        public int Id { get; set; }


        public string Name { get; set; }

        public int League_Id { get; set; }

        public string League_Name { get; set; }
        public string flag { get; set; }

        public Login Login { set; get; }
        public string Customer_ID { get; set; }

        public int Branch_ID { set; get; }

        public int value { set; get; }

        public string Message { set; get; }

        public string Service_ID { get; set; }
        public string Service_ID1 { get; set; }
        public string Ft_ID { get; set; }
        public string Ft_Name { get; set; }
        public int Team_ID { get; set; }

        //public string selectted_Ftteam { set; get; }                
        public dynamic selectted_Ftteam { set; get; }
        public List<Model.Services> selectted_Ftteam1 { get; set; }
    }
}
