using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calyx_Solutions.Model
{
    public class Offer
    {
        public int Transaction_ID { get; set;}
        public string Customer_Id { get; set; }
        public string Spin_currency_code { get; set; }
        public int Client_ID { get; set; }
        public int User_ID { get; set; }
        public int Offer_ID { get; set; }
        public int Branch_ID { get; set; }
        public Branch Branch { set; get; }
        public Client Client { set; get; }
        public Login Login { set; get; }
        public Int32 DeleteStatus { set; get; }
        public DateTime RecordInsertDate { set; get; }
        public DateTime Expiry_Date { set; get; }
        public double Spin_amount { set; get; }
        public String Spin_lable { set; get; }
        public String Spin_result { set; get; }
        public int Spin_Id { set; get; }
        public int[] Old_picked { get; set; }
        public string Foreign_currency_code { get; set; }
        public string Base_currency_code { get; set; } 
        public int Foreign_currency_id { get; set; }
        public int Base_currency_id{ get; set; }
        public int Wallet_currency_type{ get; set; }
        public int Currency_ID { get; set; }
        public int Wallet_on_deposit { get; set; }

    }
}
