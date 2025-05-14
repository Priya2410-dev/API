namespace Calyx_Solutions.Model
{
    public class Communication_Prefer
    {
        public Int32 CP_ID { set; get; }
        public Int32 CP_ID_Email { set; get; }
        public Int32 CP_ID_SMS { set; get; }
        public Int32 CP_ID_Phone { set; get; }
        public string Customer_ID { set; get; }
        public Int32 Comm_preference_ID { set; get; }
        public Int32 Comm_Preference_Status { set; get; }
        public Int32 Comm_preference_ID_Email { set; get; }
        public Int32 Comm_Preference_Status_Email { set; get; }
        public Int32 Comm_preference_ID_SMS { set; get; }
        public Int32 Comm_Preference_Status_SMS { set; get; }
        public Int32 Comm_preference_ID_Phone { set; get; }
        public Int32 Comm_Preference_Status_Phone { set; get; }
        public DateTime Record_Insert_DateTime { set; get; }
        public Int32 Client_ID { set; get; }
        public Int32 Branch_ID { set; get; }

        public List<Model.Communication_Prefer> _lstPreferences = new List<Model.Communication_Prefer>();
        public List<Model.Communication_Prefer> _lstcheckedPreferences = new List<Model.Communication_Prefer>();
        public List<Model.Communication_Prefer> _lstcheckedPreferences1 = new List<Model.Communication_Prefer>();

        public int CpID { get; set; }
        public int PreferenceID { get; set; }
        public int PreferenceStatus { get; set; }
        public List<CommunicationPreference> CommunicationPreference { get; set; }
        public string CustomerID { get; set; }


    }

    public class CommunicationPreference
    {
        public int CpID { get; set; }
        public int PreferenceID { get; set; }
        public int PreferenceStatus { get; set; }
    }

    public class CommunicationPrefer
    {
        public List<CommunicationPreference> CommunicationPreference { get; set; }
        public string CustomerID { get; set; }
    }
}
