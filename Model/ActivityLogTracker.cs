using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Auth0.ManagementApi.Models;

namespace Calyx_Solutions.Model
{
    public class ActivityLogTracker
    {
        public int Security_flag { get; set; }
        public int Customer_ID { set; get; }
        public int User_ID { set; get; }
        public int Client_ID { set; get; }
        public int Branch_ID { set; get; }

        public int Id { set; get; }
        public string Activity { set; get; }
        public int WhoAcessed { set; get; }
        public DateTime RecordInsertDate { set; get; }
        public int DeleteStatus { set; get; }
        public int Transaction_ID { set; get; }
        public string FunctionName { set; get; }
        public User User { set; get; }
        public Customer Customer { set; get; }
        public Branch Branch { set; get; }
        public Client Client { set; get; }
    }
}
