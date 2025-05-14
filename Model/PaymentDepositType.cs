using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Calyx_Solutions.Model
{
    public class PaymentDepositType
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int TransferTypeFlag { get; set; }
        public Client Client { get; set; }
    }
}
