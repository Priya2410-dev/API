using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calyx_Solutions.Model
{
   public class CustomerLimit
    {
        public Int32 Id { set; get; }
        public Int32 Days { set; get; }
        public Decimal PersonalTransAmtLmt { set; get; }
        public Decimal CompanyTransAmtLmt { set; get; }
        public DateTime RecordInsertDateTime { set; get; }
        public Int32 DeleteStatus { set; get; }
        public Branch Branch { set; get; }
        public Client Client { set; get; }
    
    }
}
