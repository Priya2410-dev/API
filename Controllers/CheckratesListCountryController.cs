/*using System.Data;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System.Web;
using System;
using System.Threading.Tasks;
using MySqlConnector;
 
using Microsoft.AspNetCore.Mvc;
using static Google.Apis.Requests.BatchRequest;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Text.Json;
using MySqlCommand = MySql.Data.MySqlClient.MySqlCommand;
using Microsoft.IdentityModel.Tokens;
using Calyx_Solutions.Model;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using Calyx_Solutions.Service;
using System.Text.Json.Nodes;
using Microsoft.Ajax.Utilities;
using System.Web.Helpers;*/
using System.Data;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;

using System;
using System.Threading.Tasks;
using MySqlConnector;
using Calyx_Solutions.Model;
using Calyx_Solutions.Service;

using static Google.Apis.Requests.BatchRequest;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using RestSharp;
using Microsoft.Net.Http.Headers;
using Auth0.ManagementApi.Models;
using System.Net;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNetCore.Http.Features;



using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Http.Extensions;
using Borland.Vcl;
using System.Linq;
using System.Web.Http.Results;
using System.Diagnostics;
 

namespace Calyx_Solutions.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CheckratesListCountryController : ControllerBase
    {
        private string RateData;

        /* private readonly object obj;*/

        

        [HttpPost]
        [Route("checklistcountry")]
        public IActionResult checkListCountry([FromBody] JsonObject obj)
        {
            Model.response.WebResponse response = null;
            string json = System.Text.Json.JsonSerializer.Serialize(obj);
            dynamic data = JObject.Parse(json);
            CompanyInfo.InsertrequestLogTracker("checklistcountry full request body: " + JObject.Parse(json), 0, 0, 0, 0, "checkListCountry", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
            var validateJsonData = (dynamic)null;
            Customer Obj = JsonConvert.DeserializeObject<Customer>(json);
            try
            {
                int number;
                if (!int.TryParse(Convert.ToString(data.clientID), out number) || data.clientID == "" || data.clientID == null)
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "Invalid Client ID." };
                    return new JsonResult(validateJsonData);
                }

            Obj.Client_ID = data.clientID;

            if (!SqlInjectionProtector.ValidateObjectForSqlInjection(Obj) || !SqlInjectionProtector.ValidateObjectForScriptSqlInjection(Obj))
            {
                var errorResponse = new { response = false, responseCode = "02", data = "Invalid input detected." };
                return new JsonResult(errorResponse) { StatusCode = 400 };
            }

                object o = JsonConvert.DeserializeObject(json);
            bool checkinjection = CompanyInfo.checksqlinjectiondata(o);
            if (!checkinjection)
            {
                response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_TECHNICAL_ERROR);
                response.ResponseMessage = "Invalid Field Values.";
                response.ResponseCode = 6;
                validateJsonData = new { response = false, responseCode = "02", data = response.ResponseMessage };
                return new JsonResult(validateJsonData);
            }
             
                DataTable dt = new DataTable();
                MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("getMultiCurrency");
                string whereclause = "";
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.Parameters.AddWithValue("_whereclause", whereclause);
                _cmd.Parameters.AddWithValue("_Client_ID", Obj.Client_ID);
                dt = db_connection.ExecuteQueryDataTableProcedure(_cmd);
                List<Dictionary<string, object>> countries = new List<Dictionary<string, object>>();

                RemoveDuplicates(dt, "Country_ID");

                foreach (DataRow dr in dt.Rows)
                {
                    Dictionary<string, object> country = new Dictionary<string, object>();
                    country["countryID"] = dr["Country_ID"];
                    country["countryFlag"] = dr["flag"];
                    country["countryName"] = dr["Country_Name"];
                    countries.Add(country);
                }
                validateJsonData = new { response = true, responseCode = "00", data = countries };
                return new JsonResult(validateJsonData);
            }
            catch (Exception ex)
            {
                validateJsonData = new { response = false, responseCode = "01", data = "Invalid Request" };
                CompanyInfo.InsertErrorLogTracker(ex.ToString(), 0, 0, 0, 0, "checkListCountry", 0, Convert.ToInt32(Obj.Client_ID), "", HttpContext);
                return new JsonResult(validateJsonData);
            }
   
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        static void RemoveDuplicates(DataTable table, string columnName)
        {
            HashSet<object> uniqueValues = new HashSet<object>();
            List<DataRow> duplicates = new List<DataRow>();

            foreach (DataRow row in table.Rows)
            {
                object value = row[columnName];
                if (uniqueValues.Contains(value))
                {
                    // Duplicate found, add it to the list for removal
                    duplicates.Add(row);
                }
                else
                {
                    // Unique value, add it to the HashSet
                    uniqueValues.Add(value);
                }
            }

            // Remove duplicate rows
            foreach (DataRow duplicateRow in duplicates)
            {
                table.Rows.Remove(duplicateRow);
            }
        }



        [HttpPost]
        [Route("checkrateslistcountry")]
        /*   25-3-24
        public IActionResult checkratesListCountry([FromBody] JsonElement obj1)
        {
            try
            {
                string msg = "";
                HttpContext context = HttpContext;
                string json = System.Text.Json.JsonSerializer.Serialize(obj1);
                dynamic data = JObject.Parse(json);

                var validateJsonData = (dynamic)null;
                Model.response.WebResponse response = null;
                Transaction obj = new Transaction();



                if (data.clientID == "" || data.clientID == null)
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "Invalid Client Id." };

                }
                //if (data.countryID == "" || data.countryID == null )
                //{
                //    validateJsonData = new { response = false, responseCode = "02", data = "Invalid Country Id." };

                //}
                if (data.paymentDepositTypeID == "" || data.paymentDepositTypeID == null)
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "Invalid Payment DepositType ID." };
                    return new JsonResult(validateJsonData);
                }
                if (data.deliveryTypeID == "" || data.deliveryTypeID == null)
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "Invalid Delivery Type Id." };
                    return new JsonResult(validateJsonData);
                }
                if (data.paymentTypeID == "" || data.paymentTypeID == null)
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "Invalid Payment Type ID." };
                    return new JsonResult(validateJsonData);
                }

                obj.Country_ID = data.countryID;

                obj.Client_ID = data.clientID;
                obj.PaymentDepositType_ID = data.paymentDepositTypeID;
                obj.DeliveryType_Id = data.deliveryTypeID;
                obj.PaymentType_ID = data.paymentTypeID;
                obj.Currency_Code = data.currencyCode;
                obj.TransferAmount = data.transferAmount;
                obj.CB_ID = data.branchID;


                DataTable dt = new DataTable();
                MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("getMultiCurrency");
                string whereclause = " ";

                if (obj.Country_ID > 0)
                {
                    whereclause = " and c.Country_ID=" + obj.Country_ID + "";
                }

                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.Parameters.AddWithValue("_whereclause", whereclause);
                _cmd.Parameters.AddWithValue("_Client_ID", Convert.ToString(obj.Client_ID));
                dt = db_connection.ExecuteQueryDataTableProcedure(_cmd);
                List<Dictionary<string, object>> countries1 = new List<Dictionary<string, object>>();
                Dictionary<string, object> country1 = new Dictionary<string, object>();
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        if (obj.Currency_Code == dr["Currency_Code"].ToString())
                        {
                            obj.Currency_ID = (int)dr["Currency_ID"];
                            countries1.Add(country1);
                        }
                    }
                }

                if (countries1.Count > 0)
                {
                    //List<Model.Currency> _lst = new List<Model.Currency>();
                    //_cmd = new MySqlConnector.MySqlCommand("Rates_Search");
                    //_cmd.CommandType = CommandType.StoredProcedure;

                    //if (obj.Country_ID > 0)
                    //{
                    //    whereclause = whereclause + " and t.Country_ID = " + obj.Country_ID + "";
                    //}
                    //if (obj.Currency_ID > 0)
                    //{
                    //    whereclause = whereclause + " and t.Currency_ID = " + obj.Currency_ID + "";
                    //}
                    //if (obj.PaymentDepositType_ID > 0)
                    //{
                    //    whereclause = whereclause + " and t.PayDepositType_ID = " + obj.PaymentDepositType_ID + "";
                    //}
                    //if (obj.DeliveryType_Id > 0)
                    //{
                    //    whereclause = whereclause + " and t.DeliveryType_ID = " + obj.DeliveryType_Id + "";
                    //}
                    //if (obj.PaymentType_ID > 0)
                    //{
                    //    whereclause = whereclause + " and t.PType_ID = " + obj.PaymentType_ID + "";
                    //}
                    //if (obj.AmountInGBP > 0)
                    //{
                    //    whereclause = whereclause + " and " + obj.AmountInGBP + " between t.Min_Amount and t.Max_Amount group by t.PType_ID , t.DeliveryType_ID , t.Country_ID ";
                    //}

                    //_cmd.Parameters.AddWithValue("_Client_ID", Convert.ToString(obj.Client_ID));
                    //_cmd.Parameters.AddWithValue("_whereclause", whereclause);


                    List<Model.Currency> _lst = new List<Model.Currency>();
                    MySqlConnector.MySqlCommand cmd = new MySqlConnector.MySqlCommand("Rates_Search");
                    cmd.CommandType = CommandType.StoredProcedure;
                    //string whereclause = " ";
                    if (obj.Country_ID > 0)
                    {
                        whereclause = whereclause + " and t.Country_ID = " + obj.Country_ID + "";
                    }
                    if (obj.Currency_ID > 0)
                    {
                        whereclause = whereclause + " and t.Currency_ID = " + obj.Currency_ID + "";
                    }
                    if (obj.CB_ID > 0)
                    {
                        whereclause = whereclause + " and t.CB_ID = " + obj.CB_ID + "";
                    }
                    if (obj.PType_ID > 0)
                    {
                        whereclause = whereclause + " and t.PType_ID = " + obj.PType_ID + "";
                    }
                    if (obj.PaymentDepositType_ID > 0)
                    {
                        whereclause = whereclause + " and t.PayDepositType_ID = " + obj.PaymentDepositType_ID + "";
                    }
                    if (obj.DeliveryType_Id > 0)
                    {
                        whereclause = whereclause + " and t.DeliveryType_ID = " + obj.DeliveryType_Id + "";
                    }
                    obj.AmountInGBP = Convert.ToDouble(obj.TransferAmount);
                    if (obj.AmountInGBP != null && obj.AmountInGBP != 0)
                    {
                        whereclause = whereclause + " and t.Min_Amount <= " + obj.AmountInGBP + " and t.Max_Amount >= " + obj.AmountInGBP + "";
                    }
                    obj.AmountInPKR = Convert.ToDouble(obj.TransferForeignAmount);
                    if (obj.AmountInPKR != null && obj.AmountInPKR != 0)
                    {
                        whereclause = whereclause + " and t.Foreign_Currency_Min_Amount <= " + obj.AmountInPKR + " and t.Foreign_Currency_Max_Amount >= " + obj.AmountInPKR + "";
                    }

                    cmd.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                    cmd.Parameters.AddWithValue("_whereclause", whereclause);
                    dt = db_connection.ExecuteQueryDataTableProcedure(cmd);
                    if (dt.Rows.Count > 0)
                    {
                        List<Dictionary<string, object>> countries = new List<Dictionary<string, object>>();
                        foreach (DataRow dr in dt.Rows)
                        {
                            Dictionary<string, object> country = new Dictionary<string, object>();
                            country["baseCurrencyCodeCompany"] = dr["CompanyBaseCode"];
                            country["baseCurrencyFlag"] = dr["Base_Country_Flag"];
                            country["countryName"] = dr["Country_Name"];
                            country["currencyCode"] = dr["Currency_Code"];
                            country["countryFlag"] = dr["Country_Flag"];
                            country["minAmount"] = dr["Min_Amount"];
                            country["maxAmount"] = dr["Max_Amount"];
                            country["foreignCurrencyMinAmount"] = dr["Foreign_Currency_Min_Amount"];
                            country["foreignCurrencyMaxAmount"] = dr["Foreign_Currency_Max_Amount"];
                            country["transferFeesGBP"] = dr["Transfer_Fees"];
                            country["rate"] = dr["Rate"];
                            countries.Add(country);

                        }
                        var jsonData1 = new { response = true, responseCode = "00", data = countries };
                        return new JsonResult(jsonData1);
                    }
                    var jsonData2 = new { response = false, responseCode = "02", data = "Rates and fees are not available" };
                    return new JsonResult(jsonData2);
                }
                var jsonData = new { response = false, responseCode = "02", data = "Invalid Request" };
                return new JsonResult(jsonData);

            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                var jsonData2 = new { response = false, responseCode = "02", data = msg };
                return new JsonResult(jsonData2);
            }
        }
        */

        public IActionResult checkratesListCountry([FromBody] JsonElement obj1)
        {
            try
            {
                string msg = "";
                HttpContext context = HttpContext;
                string json = System.Text.Json.JsonSerializer.Serialize(obj1);
                dynamic data = JObject.Parse(json);
                CompanyInfo.InsertrequestLogTracker("checkratesListCountry full request body: " + JObject.Parse(json), 0, 0, 0, 0, "checkratesListCountry", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
                var validateJsonData = (dynamic)null;
                Model.response.WebResponse response = null;
                Transaction obj = new Transaction();

                if (!SqlInjectionProtector.ReadJsonElementValues(obj1) || !SqlInjectionProtector.ReadJsonElementValuesScript(obj1))
                {
                    var errorResponse = new { response = false, responseCode = "02", data = "Invalid input detected." };
                    return new JsonResult(errorResponse) { StatusCode = 400 };
                }


                if (data.clientID == "" || data.clientID == null)
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "Invalid Client Id." };

                }
                //if (data.countryID == "" || data.countryID == null )
                //{
                //    validateJsonData = new { response = false, responseCode = "02", data = "Invalid Country Id." };

                //}
                if (data.paymentDepositTypeID == "" || data.paymentDepositTypeID == null)
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "Invalid Payment DepositType ID." };
                    return new JsonResult(validateJsonData);
                }
                if (data.deliveryTypeID == "" || data.deliveryTypeID == null)
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "Invalid Delivery Type Id." };
                    return new JsonResult(validateJsonData);
                }
                if (data.paymentTypeID == "" || data.paymentTypeID == null)
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "Invalid Payment Type ID." };
                    return new JsonResult(validateJsonData);
                }

                obj.Country_ID = data.countryID;

                obj.Client_ID = data.clientID;
                obj.PaymentDepositType_ID = data.paymentDepositTypeID;
                obj.DeliveryType_Id = data.deliveryTypeID;
                obj.PaymentType_ID = data.paymentTypeID;
                obj.Currency_Code = data.currencyCode;
                obj.TransferAmount = data.transferAmount;
                obj.CB_ID = data.branchID;
                obj.Basecurr_Country_ID = data.BaseCurrencyID;


                DataTable dt = new DataTable();
                MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("getMultiCurrency");
                string whereclause = " ";

                if (obj.Country_ID > 0)
                {
                    whereclause = " and c.Country_ID=" + obj.Country_ID + "";
                }

                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.Parameters.AddWithValue("_whereclause", whereclause);
                _cmd.Parameters.AddWithValue("_Client_ID", Convert.ToString(obj.Client_ID));
                dt = db_connection.ExecuteQueryDataTableProcedure(_cmd);
                List<Dictionary<string, object>> countries1 = new List<Dictionary<string, object>>();
                Dictionary<string, object> country1 = new Dictionary<string, object>();
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        if (obj.Currency_Code == dr["Currency_Code"].ToString())
                        {
                            obj.Currency_ID = (int)dr["Currency_ID"];
                            countries1.Add(country1);
                        }
                    }
                }

                if (countries1.Count > 0)
                {
                
                    List<Model.Currency> _lst = new List<Model.Currency>();
                    MySqlConnector.MySqlCommand cmd = new MySqlConnector.MySqlCommand("Rates_Search");
                    cmd.CommandType = CommandType.StoredProcedure;
                     whereclause = " ";
                    if (obj.Country_ID > 0)
                    {
                        whereclause = whereclause + " and t.Country_ID = " + obj.Country_ID + "";
                    }
                    if (obj.Currency_ID > 0)
                    {
                        whereclause = whereclause + " and t.Currency_ID = " + obj.Currency_ID + "";
                    }
                    if (obj.CB_ID > 0)
                    {
                        whereclause = whereclause + " and t.CB_ID = " + obj.CB_ID + "";
                    }
                    if (obj.PType_ID > 0)
                    {
                        whereclause = whereclause + " and t.PType_ID = " + obj.PType_ID + "";
                    }
                    if (obj.PaymentDepositType_ID > 0)
                    {
                        whereclause = whereclause + " and t.PayDepositType_ID = " + obj.PaymentDepositType_ID + "";
                    }
                    if (obj.DeliveryType_Id > 0)
                    {
                        whereclause = whereclause + " and t.DeliveryType_ID = " + obj.DeliveryType_Id + "";
                    }
                    if (obj.Basecurr_Country_ID != 0 && obj.Basecurr_Country_ID != null)
                    {
                        whereclause = whereclause + " and t.BaseCurrency_ID = " + obj.Basecurr_Country_ID + "";
                    }
                    obj.AmountInGBP = Convert.ToDouble(obj.TransferAmount);
                    if (obj.AmountInGBP != null && obj.AmountInGBP != 0)
                    {
                        whereclause = whereclause + " and t.Min_Amount <= " + obj.AmountInGBP + " and t.Max_Amount >= " + obj.AmountInGBP + "";
                    }
                    obj.AmountInPKR = Convert.ToDouble(obj.TransferForeignAmount);
                    if (obj.AmountInPKR != null && obj.AmountInPKR != 0)
                    {
                        whereclause = whereclause + " and t.Foreign_Currency_Min_Amount <= " + obj.AmountInPKR + " and t.Foreign_Currency_Max_Amount >= " + obj.AmountInPKR + "";
                    }

                    CompanyInfo.InsertrequestLogTracker("checkratesListCountry where clause: "+ whereclause  , 0, 0, 0, 0, "checkratesListCountry", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);

                    cmd.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                    cmd.Parameters.AddWithValue("_whereclause", whereclause);
                    dt = db_connection.ExecuteQueryDataTableProcedure(cmd);
                    if (dt.Rows.Count > 0)
                    {
                        List<Dictionary<string, object>> countries = new List<Dictionary<string, object>>();
                        foreach (DataRow dr in dt.Rows)
                        {
                            Dictionary<string, object> country = new Dictionary<string, object>();
                            // country["baseCurrencyCodeCompany"] = dr["CompanyBaseCode"];
                            // country["baseCurrencyFlag"] = dr["Base_Country_Flag"];
                            country["countryName"] = dr["Country_Name"];
                            country["currencyCode"] = dr["Currency_Code"];
                            country["countryFlag"] = dr["Country_Flag"];
                            country["minAmount"] = dr["Min_Amount"];
                            country["maxAmount"] = dr["Max_Amount"];
                            country["foreignCurrencyMinAmount"] = dr["Foreign_Currency_Min_Amount"];
                            country["foreignCurrencyMaxAmount"] = dr["Foreign_Currency_Max_Amount"];
                            country["transferFeesGBP"] = dr["Transfer_Fees"];
                            country["rate"] = dr["Rate"];
                            countries.Add(country);

                        }
                        var jsonData1 = new { response = true, responseCode = "00", data = countries };
                        return new JsonResult(jsonData1);
                    }
                    var jsonData2 = new { response = false, responseCode = "02", data = "Rates and fees are not available" };
                    return new JsonResult(jsonData2);
                }
                var jsonData = new { response = false, responseCode = "02", data = "Invalid Request" };
                return new JsonResult(jsonData);

            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                var jsonData2 = new { response = false, responseCode = "02", data = msg };
                return new JsonResult(jsonData2);
            }
        }


        [HttpPost]
        [Route("basecurrencylist")]

        public IActionResult basecurrencylist([FromBody] JsonElement Obj)
        {
            try
            {
                string msg = "";
                HttpContext context = HttpContext;
                string json = System.Text.Json.JsonSerializer.Serialize(Obj);
                dynamic data = JObject.Parse(json);
                CompanyInfo.InsertrequestLogTracker("basecurrencylist full request body: " + JObject.Parse(json), 0, 0, 0, 0, "basecurrencylist", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
                var validateJsonData = (dynamic)null;
                Model.response.WebResponse response = null;
                Transaction obj = new Transaction();

                if (!SqlInjectionProtector.ReadJsonElementValues(Obj) || !SqlInjectionProtector.ReadJsonElementValuesScript(Obj))
                {
                    var errorResponse = new { response = false, responseCode = "02", data = "Invalid input detected." };
                    return new JsonResult(errorResponse) { StatusCode = 400 };
                }

                int number;
                if (!int.TryParse(Convert.ToString(data.clientID), out number) || data.clientID == "" || data.clientID == null)
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "Invalid Client ID." };
                    return new JsonResult(validateJsonData);
                }
                if (!int.TryParse(Convert.ToString(data.branchID), out number) || data.branchID == "" || data.branchID == null)
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "Invalid branch ID ." };
                    return new JsonResult(validateJsonData);
                }

                obj.Branch_ID = data.branchID;
                obj.Client_ID = data.clientID;

                DataTable dt = new DataTable();
                MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("Get_Basecurrency_Details");
                string whereclause = "";
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.Parameters.AddWithValue("_whereclause", whereclause);
                dt = db_connection.ExecuteQueryDataTableProcedure(_cmd);


                Dictionary<string, string> jsonDictionary = new Dictionary<string, string>();
                List<Dictionary<string, object>> countries = new List<Dictionary<string, object>>();
                if (dt.Rows.Count > 0)
                {

                    foreach (DataRow dr in dt.Rows)
                    {
                        Dictionary<string, object> country = new Dictionary<string, object>();
                        country["countryFlag"] = dr["Country_Flag"];
                        country["imageName"] = dr["ImageName"];                        
                        country["mobileFlag"] = Convert.ToString(dr["chkmobile"]);
                        country["postcodeFlag"] = dr["chkpostcode"];
                        country["countryCode"] = dr["Country_Code"];
                        country["flag"] = dr["flag"];
                        country["isoCode"] = dr["ISO_Code"];
                        country["countryID"] = dr["Country_ID"];
                        country["countryName"] = dr["Country_Name"];
                        country["countryCurrency"] = dr["Country_Currency"];
                        country["baseCurrencyID"] = dr["base_currency_id"];
                        country["baseCurrencyCode"] = dr["BaseCurrency_Code"];
                        country["baseCurrencyStatus"] = dr["basecountry_status"];
                        country["ShowOnRegistration"] = dr["show_on_Registration"];
                        countries.Add(country);

                    }
                    var jsonData1 = new { response = true, responseCode = "00", data = countries };
                    return new JsonResult(jsonData1);
                }
                else
                {
                    var jsonData2 = new { response = false, responseCode = "00", data = countries };
                    return new JsonResult(jsonData2);
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                CompanyInfo.InsertrequestLogTracker("Error basecurrencylist full request body: " + ex.Message, 0, 0, 0, 0, "basecurrencylist", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
                var jsonData2 = new { response = false, responseCode = "01", data = "Invalid Request." };
                return new JsonResult(jsonData2);
            }
        }



        public static bool IsValidInt(int value)
        {
            return value >= int.MinValue && value <= int.MaxValue;
        }

        private List<T> ConvertDataTableToList<T>(DataTable dt)
        {
            throw new NotImplementedException();
        }

    }

}
