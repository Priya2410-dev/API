using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calyx_Solutions.Model
{
    public class Bank
    {
        public Login Login { set; get; }
        public Int32 Id { set; get; }
        public string Code { set; get; }
        public string IFSCCode { set; get; }
        public string Name { set; get; }
        public string BranchCode { set; get; }
        public Branch Branch { set; get; }
        public int Client_ID { set; get; }
        public int Branch_ID { set; get; }
        public string BICcodes { get; set; }
    }
}
