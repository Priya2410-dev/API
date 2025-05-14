using Microsoft.Extensions.Diagnostics.HealthChecks;
using Newtonsoft.Json.Linq;

namespace Calyx_Solutions.Model
{
    public class Currency
    {
        /*
        public int Client_ID { get;  set; }
        public int Country_ID { get;  set; }
        public int Currency_ID { get; set; }
        public int PaymentDepositType_ID { get;  set; }
        public int DeliveryType_Id { get;  set; }
        public int PType_ID { get;  set; }
        public int Amount { get;  set; }
        public string? Currency_Code { get; set; }
        */

        public int Client_ID { get; set; }
        public int Country_ID { get; set; }
        public int Branch_ID { get; set; }
        public int Currency_ID { get; set; }
        public int Flag { get; set; }
        public int PaymentDepositType_ID { get; set; }
        public int DeliveryType_Id { get; set; }
        public int PType_ID { get; set; }
        public int Amount { get; set; }
        public string? Currency_Code { get; set; }
    }

}