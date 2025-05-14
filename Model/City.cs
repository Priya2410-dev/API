namespace Calyx_Solutions.Model
{
    public class City
    {
        public Int32 Id { set; get; }
        public string Name { set; get; }
        public Country Country { get; set; }
        public int Client_ID { get; set; }        
        public int Country_ID { get; set; }
        public int cityBranchId { get; set; }
        public Login Login { set; get; }
    }
}
