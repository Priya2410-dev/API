using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calyx_Solutions.Model
{
    public class Country
    {
        public int provience_id { set; get; }
        public Login Login { set; get; }
        public int Client_ID { get; set; }
        public int Id { set; get; }
        public string Salary_Range { get; set; } //parvej

        public string Name { set; get; }
        public string Code { set; get; }
        public Currency Currency { set; get; }
        public int DeleteStatus { set; get; }
        public string CurrencyCode { set; get; }
        public int SendingFlag { set; get; }
        public Client Client { set; get; }
        public int base_country { set; get; }
        public int chkpostcode_search { set; get; }
        public string chkbasecountry_mb { set; get; }

        public int base_currency_id { get; set; }
        public int base_currency_code { get; set; }
    }
}
