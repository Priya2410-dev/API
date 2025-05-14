using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calyx_Solutions.Model
{
    public class ErrorLog
    {
        public int Customer_ID { set; get; }
        public int Client_ID { set; get; }
        public int Branch_ID { set; get; }
        public int User_ID { set; get; }
        public int Id { set; get; }
        public string Error { set; get; }
        public DateTime Date { set; get; }
        public User User { set; get; }
        public int DeleteStatus { set; get; }
        public Branch Branch { set; get; }
        public Client Client { set; get; }
        public string Function_Name { get; set; }
    }
}
