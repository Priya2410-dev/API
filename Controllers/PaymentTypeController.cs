using Calyx_Solutions.Model;
using Calyx_Solutions.Service;
using Microsoft.Ajax.Utilities;

using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data;
using System.Globalization;
using System.Text.Json;

using static Google.Apis.Requests.BatchRequest;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Calyx_Solutions.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentTypeController : ControllerBase
    {
        // POST api/<PaymentTypeController>
        [HttpPost]       
        [Route("getpaytypes")]
        /*public IActionResult GetPayTypes([FromBody] JsonElement obj)
        {
            HttpContext context = HttpContext;
            string json = System.Text.Json.JsonSerializer.Serialize(obj);
            var validateJsonData = (dynamic)null;
            dynamic data = JObject.Parse(json);
            Transaction obj1 = new Transaction();
            Model.response.WebResponse response = null;
            try
            {
                if (data.clientID == "" || data.clientID == null)
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "Invalid Client ID." };
                    return new JsonResult(validateJsonData);
                }
                obj1.Client_ID = data.clientID;

                MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("GetPaymentType");
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.Parameters.AddWithValue("_Client_ID", obj1.Client_ID);
                // _cmd.Parameters.AddWithValue("_whereclause", "");
                DataTable dt = db_connection.ExecuteQueryDataTableProcedure(_cmd);

                if (dt != null && dt.Rows.Count > 0)
                {
                    List<Dictionary<string, object>> Paytype = new List<Dictionary<string, object>>();

                    foreach (DataRow dr in dt.Rows)
                    {
                        Dictionary<string, object> Pay_Type = new Dictionary<string, object>();
                        Pay_Type["baseCountryId"] = dr["Base_Country_Id"];
                        Pay_Type["baseCurrencyId"] = dr["Base_Currency_Id"];
                        Pay_Type["clientID"] = dr["Client_ID"];
                        Pay_Type["countryActiveStatus"] = dr["Country_active_status"];
                        Pay_Type["deleteStatus"] = dr["Delete_Status"];
                        Pay_Type["deleteStatus"] = dr["Delete_Status1"];
                        Pay_Type["mappingID"] = dr["Mapping_ID"];
                        Pay_Type["maxAmountLimit"] = dr["Max_Amount_Limit"];
                        Pay_Type["payType"] = dr["PType"];
                        Pay_Type["payTypeDescription"] = dr["PType_Description"];
                        Pay_Type["payTypeID"] = dr["PType_ID"];
                        Pay_Type["paymentTypeSrc"] = dr["Payment_Type_Src"];
                        Pay_Type["paymentTypeSrcID"] = dr["Payment_Type_Src_ID"];
                        Pay_Type["reviewTransferMessage"] = dr["Review_Transfer_Message"];
                        Pay_Type["preferredFlag"] = dr["preferred_flag"];
                        Pay_Type["preferredFlag"] = dr["preferred_flag1"];
                        Paytype.Add(Pay_Type);
                    }

                    validateJsonData = new { response = true, responseCode = "00", data = Paytype };
                    return new JsonResult(validateJsonData);
                }
                else
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "No Records Found." };
                    return new JsonResult(validateJsonData);
                }
            }
            catch (Exception ex)
            {
                string Activity = ex.ToString();
                validateJsonData = new { response = false, responseCode = "01", data = "Invalid Request." };
                CompanyInfo.InsertErrorLogTracker(Activity.ToString(), 0, 0, 0, 0, "GetPayTypes", Convert.ToInt32(obj1.Branch_ID), Convert.ToInt32(obj1.Client_ID), "", context);
                return new JsonResult(validateJsonData);
            }

        }
        */


        public IActionResult GetPayTypes([FromBody] JsonElement obj)
        {
            HttpContext context = HttpContext;
            string json = System.Text.Json.JsonSerializer.Serialize(obj);
            var validateJsonData = (dynamic)null;
            dynamic data = JObject.Parse(json);
            CompanyInfo.InsertrequestLogTracker("GetPayTypes full request body: " + JObject.Parse(json), 0, 0, 0, 0, "GetPayTypes", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
            Transaction obj1 = new Transaction();
            Model.response.WebResponse response = null;
            try
            {
                if (!SqlInjectionProtector.ReadJsonElementValues(obj) || !SqlInjectionProtector.ReadJsonElementValuesScript(obj))
                {
                    var errorResponse = new { response = false, responseCode = "02", data = "Invalid input detected." };
                    return new JsonResult(errorResponse) { StatusCode = 400 };
                }

                if (data.clientID == "" || data.clientID == null)
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "Invalid Client ID." };
                    return new JsonResult(validateJsonData);
                }
                if (data.branchID == "" || data.branchID == null)
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "Invalid Branch ID." };
                    return new JsonResult(validateJsonData);
                }
                obj1.Client_ID = data.clientID;
                obj1.Branch_ID = data.branchID;
                obj1.Is_App = data.isApp;
                obj1.Basecurr_Country_ID = data.baseCountryID;


                string webstring = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("ConnectionStrings")["WEB_DB_CONN"];
                webstring = webstring.ToLower();
                if (webstring.IndexOf("kobo", 0, StringComparison.OrdinalIgnoreCase) != -1)
                {
                    obj1.Basecurr_Country_ID = 0; // Remove This after kobo. This is for just Kobo
                }
                

                MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("GetPaymentTypes");
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.Parameters.AddWithValue("_Client_ID", obj1.Client_ID);
                int SourceID = 1;
                string Whereclause = "";
                if (obj1.Is_App == 0) 
                { SourceID = 3; }
                else if (obj1.Branch_ID == 2) { SourceID = 2; }
                _cmd.Parameters.AddWithValue("_Source_ID", SourceID);
                if (obj1.Basecurr_Country_ID != 0)
                { Whereclause = Whereclause + " And Base_Country_Id =" + obj1.Basecurr_Country_ID; }
                _cmd.Parameters.AddWithValue("_whereclause", Whereclause);
                DataTable dt = db_connection.ExecuteQueryDataTableProcedure(_cmd);

                if (dt != null && dt.Rows.Count > 0)
                {
                    List<Dictionary<string, object>> Paytype = new List<Dictionary<string, object>>();

                   /* var dtData = dt.Rows.OfType<DataRow>()
               .Select(row => dt.Columns.OfType<DataColumn>()
                   .ToDictionary(col => col.ColumnName, c => row[c].ToString()));
                    validateJsonData = new { response = true, responseCode = "00", data = dtData };
                    return new JsonResult(validateJsonData);*/
                    
                    foreach (DataRow dr in dt.Rows)
                    {
                        string result = ConvertAsciiToSpecialChars(Convert.ToString(dr["Review_Transfer_Message"]));

                        Dictionary<string, object> Pay_Type = new Dictionary<string, object>();
                        Pay_Type["baseCountryId"] = dr["Base_Country_Id"];
                        Pay_Type["baseCurrencyId"] = dr["Base_Currency_Id"];
                        Pay_Type["clientID"] = dr["Client_ID"];
                        Pay_Type["countryActiveStatus"] = dr["Country_active_status"];
                        Pay_Type["deleteStatus"] = dr["Delete_Status"];
                        Pay_Type["deleteStatus"] = dr["Delete_Status1"];
                        Pay_Type["mappingID"] = dr["Mapping_ID"];
                        Pay_Type["maxAmountLimit"] = dr["Max_Amount_Limit"];
                        Pay_Type["payType"] = dr["PType"];
                        Pay_Type["payTypeDescription"] = Convert.ToString(dr["PType_Description"]);
                        Pay_Type["payTypeID"] = dr["PType_ID"];
                        Pay_Type["paymentTypeSrc"] = dr["Payment_Type_Src"];
                        Pay_Type["paymentTypeSrcID"] = dr["Payment_Type_Src_ID"];
                        Pay_Type["reviewTransferMessage"] = result; 
                        Pay_Type["preferredFlag"] = dr["preferred_flag"];
                        Pay_Type["preferredFlag"] = dr["preferred_flag1"];
                        Paytype.Add(Pay_Type);
                    }

                    validateJsonData = new { response = true, responseCode = "00", data = Paytype };
                    return new JsonResult(validateJsonData);
                }
                else
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "No Records Found." };
                    return new JsonResult(validateJsonData);
                }
            }
            catch (Exception ex)
            {
                string Activity = ex.ToString();
                validateJsonData = new { response = false, responseCode = "01", data = "Invalid Request." };
                CompanyInfo.InsertErrorLogTracker(Activity.ToString(), 0, 0, 0, 0, "GetPayTypes", Convert.ToInt32(obj1.Branch_ID), Convert.ToInt32(obj1.Client_ID), "", context);
                return new JsonResult(validateJsonData);
            }

        }

        static string ConvertAsciiToSpecialChars(string inputStr)
        {
            string tempStr = inputStr;
            while (true)
            {
                int startIdx = tempStr.IndexOf('`');
                int endIdx = tempStr.IndexOf('~');

                if (startIdx == -1 || endIdx == -1 || startIdx + 1 >= endIdx)
                    break;

                string asciiStr = tempStr.Substring(startIdx + 1, endIdx - startIdx - 1);

                if (string.IsNullOrEmpty(asciiStr))
                    break;

                if (!int.TryParse(asciiStr, out int asciiValue))
                    return "dont"; // Invalid ASCII number

                char specialChar = (char)asciiValue;
                tempStr = ReplaceBetween(tempStr, startIdx, endIdx + 1, specialChar.ToString());
            }

            return tempStr;
        }

        static string ReplaceBetween(string original, int start, int end, string replacement)
        {
            return original.Substring(0, start) + replacement + original.Substring(end);
        }

        //Get_payment_mapping_details customerfor payby card & bank transfer
        [HttpPost]
        [Route("Getpaymentmappingdetails")]

        public IActionResult Getpaymentmappingdetails([FromBody] JsonElement obj)
        {
            HttpContext context = HttpContext;
            string json = System.Text.Json.JsonSerializer.Serialize(obj);
            var validateJsonData = (dynamic)null;
            dynamic data = JObject.Parse(json);
            CompanyInfo.InsertrequestLogTracker("GetPayTypes full request body: " + JObject.Parse(json), 0, 0, 0, 0, "Get_payment_mapping_details", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);

            Transaction obj1 = new Transaction();
            Model.response.WebResponse response = null;
            try
            {
                // Check for SQL injection in the input
                if (!SqlInjectionProtector.ReadJsonElementValues(obj) || !SqlInjectionProtector.ReadJsonElementValuesScript(obj))
                {
                    var errorResponse = new { response = false, responseCode = "02", data = "Invalid input detected." };
                    return new JsonResult(errorResponse) { StatusCode = 400 };
                }

                // Validate required fields from the request body
                if (data.clientID == "" || data.clientID == null)
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "Invalid Client ID." };
                    return new JsonResult(validateJsonData);
                }
                if (data.branchID == "" || data.branchID == null)
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "Invalid Branch ID." };
                    return new JsonResult(validateJsonData);
                }

                obj1.Client_ID = data.clientID;
                obj1.Branch_ID = data.branchID;
                //obj1.Is_App = data.isApp;
                obj1.Basecurr_Country_ID = data.baseCountryID;
                obj1.PaymentType_ID = data.PaymentType_ID;

                // Get connection string for the database
                string webstring = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("ConnectionStrings")["WEB_DB_CONN"];
                webstring = webstring.ToLower();
                if (webstring.IndexOf("kobo", 0, StringComparison.OrdinalIgnoreCase) != -1)
                {
                    obj1.Basecurr_Country_ID = 0; // Special handling for Kobo
                }

                // Get current date
                DateTime date = Convert.ToDateTime(CompanyInfo.gettime(obj1.Client_ID, "0", obj1.Branch_ID, context));
                string Todays_date = date.ToString("yyyy-MM-dd");

                // Prepare the MySQL command to call the stored procedure
                MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("sp_get_paymentgetway_mapping_list");
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.Parameters.AddWithValue("_Base_Country_Id", obj1.Basecurr_Country_ID);
                _cmd.Parameters.AddWithValue("_PType_ID", obj1.PaymentType_ID);
                _cmd.Parameters.AddWithValue("_Date", Todays_date);
                DataTable dt = db_connection.ExecuteQueryDataTableProcedure(_cmd);
                if (dt != null && dt.Rows.Count > 0)
                {
                    List<Dictionary<string, object>> Paytype = new List<Dictionary<string, object>>();

                    /* var dtData = dt.Rows.OfType<DataRow>()
                .Select(row => dt.Columns.OfType<DataColumn>()
                    .ToDictionary(col => col.ColumnName, c => row[c].ToString()));
                     validateJsonData = new { response = true, responseCode = "00", data = dtData };
                     return new JsonResult(validateJsonData);*/

                    foreach (DataRow dr in dt.Rows)
                    {
                        //  string result = ConvertAsciiToSpecialChars(Convert.ToString(dr["Review_Transfer_Message"]));

                        Dictionary<string, object> Pay_Type = new Dictionary<string, object>();
                        Pay_Type["Payment_getway_id"] = dr["Payment_getway_id"];
                        Pay_Type["Preference_No"] = dr["Preference_No"];
                        Pay_Type["Admin_Status"] = dr["Admin_Status"];
                        Pay_Type["Customer_Status"] = dr["Customer_Status"];
                        Pay_Type["todays_tran_Count"] = dr["todays_tran_Count"];
                        Pay_Type["todays_total_AmountInGBP"] = dr["todays_total_AmountInGBP"];
                        Pay_Type["Custom_Setting_ID"] = dr["Custom_Setting_ID"];
                        Pay_Type["PType_ID"] = dr["PType_ID"];
                        Pay_Type["Base_Country_Id"] = dr["Base_Country_Id"];
                        Pay_Type["PaymentGateway_Name"] = Convert.ToString(dr["PaymentGateway_Name"]);//
                        Pay_Type["Delete_Status"] = dr["Delete_Status"];
                        Pay_Type["Min_Amount"] = dr["Min_Amount"];
                        Pay_Type["Max_Amount"] = dr["Max_Amount"];
                        Pay_Type["Start_Time"] = Convert.ToString(dr["Start_Time"]);
                        Pay_Type["End_Time"] = Convert.ToString(dr["End_Time"]);
                        Pay_Type["Custom_Setting_Perm"] = dr["Custom_Setting_Perm"];
                        Pay_Type["Woking_Days"] = Convert.ToString(dr["Woking_Days"]);
                        Pay_Type["Transction_cnt"] = dr["Transction_cnt"];
                        Pay_Type["Total_Amt_perday"] = dr["Total_Amt_perday"];
                        Pay_Type["Per_chk_type_id"] = dr["Per_chk_type_id"];
                        Pay_Type["ID"] = dr["ID"];

                        Paytype.Add(Pay_Type);
                    }

                    validateJsonData = new { response = true, responseCode = "00", data = Paytype };
                    return new JsonResult(validateJsonData);
                }
                else
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "No Records Found." };
                    return new JsonResult(validateJsonData);
                }
            }
            catch (Exception ex)
            {
                string Activity = ex.ToString();
                validateJsonData = new { response = false, responseCode = "02", data = "No Records Found." };
                CompanyInfo.InsertErrorLogTracker(Activity.ToString(), 0, 0, 0, 0, "Get_payment_mapping_details", Convert.ToInt32(obj1.Branch_ID), Convert.ToInt32(obj1.Client_ID), "", context);
                return new JsonResult(validateJsonData);
            }

        }

    }
}
