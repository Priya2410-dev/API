namespace Calyx_Solutions.Model
{
    public class HeardFrom
    {
        public int Id { get; set; }
        public string HeardFromOption { get; set; }
        public int DeleteStatus { get; set; }
        public Login Login { set; get; }
        public int Client_ID { get; set; }
    }
}
