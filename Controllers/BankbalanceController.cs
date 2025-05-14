using Calyx_Solutions.Model;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Text.Json.Nodes;
using System.Data;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNetCore.Authorization;
using System.Net;
using System.Security.Claims;
using System.Text.Json;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Calyx_Solutions.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BankbalanceController : ControllerBase
    {
       
        [HttpPost]
        [Authorize]
        [Route("bankaccountdetails")]
        public IActionResult BankAccountDetails([FromBody] JsonObject obj)
        {
            Model.response.WebResponse response = null;
            string json = System.Text.Json.JsonSerializer.Serialize(obj);
            dynamic data = JObject.Parse(json);
            JsonElement jsonElement = System.Text.Json.JsonSerializer.SerializeToElement(obj);//sanket // Converted JsonObject to JsonElement
            var claimsIdentity = User.Identity as ClaimsIdentity;//sanket
            var authHeader = "";
            try { authHeader = HttpContext.Request.Headers["Authorization"].ToString(); } catch (Exception egx) { authHeader = ""; }

            bool auth = AuthController.checkAuth(claimsIdentity, jsonElement, Convert.ToString(authHeader));
            if (!auth)
            {
                var errorResponse = new { response = false, responseCode = "403", data = "Access Forbidden" };
                return new JsonResult(errorResponse) { StatusCode = (int)HttpStatusCode.Forbidden };
            }
            CompanyInfo.InsertrequestLogTracker("bankaccountdetails full request body: " + JObject.Parse(json), 0, 0, 0, 0, "BankAccountDetails", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
            var validateJsonData = (dynamic)null;
            Customer Obj = JsonConvert.DeserializeObject<Customer>(json);
            try
            {
            if (data.clientID == "" || data.clientID == null)
            {
                validateJsonData = new { response = false, responseCode = "02", data = "Invalid clientID field." };
                return new JsonResult(validateJsonData);
            }
            if (data.branchID == "" || data.branchID == null)
            {
                validateJsonData = new { response = false, responseCode = "02", data = "Invalid branchID field." };
                return new JsonResult(validateJsonData);
            }
            if (data.baseCurrency == "" || data.baseCurrency == null)
            {
                validateJsonData = new { response = false, responseCode = "02", data = "Invalid baseCurrency field." };
                return new JsonResult(validateJsonData);
            }

            Obj.Client_ID = data.clientID;
            Obj.Branch_ID = data.branchID;
            string Base_currency = data.baseCurrency;

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
                string _wherecluase = "1=1 ";
                DataTable db = new DataTable();
                string strResult = string.Empty;
                if (Obj.Client_ID != 0 && Obj.Client_ID != null)
                {
                    _wherecluase = _wherecluase + " and cbd.company_id=" + Obj.Client_ID;
                }
                if (Base_currency != "" && Base_currency != null && Base_currency != "null")
                {
                    _wherecluase = _wherecluase + " and base_currency_code ='" + Base_currency + "'";
                }
                else
                {
                    _wherecluase = _wherecluase + " limit 1";
                }

                MySqlConnector.MySqlCommand cmd1 = new MySqlConnector.MySqlCommand("sp_select_company_bank_deatils");
                cmd1.CommandType = CommandType.StoredProcedure;
                cmd1.Parameters.AddWithValue("_wherecluase", _wherecluase);
                cmd1.Parameters.AddWithValue("_Timezone_Date", (string)CompanyInfo.gettime(Obj.Client_ID, HttpContext));
                db = (DataTable)db_connection.ExecuteQueryDataTableProcedure(cmd1);
                _wherecluase = " 1=1";
                if (db.Rows.Count == 0)
                {
                    if (Obj.Client_ID != 0 && Obj.Client_ID != null)
                    {
                        _wherecluase = _wherecluase + " and cbd.company_id=" + Obj.Client_ID;
                    }
                    _wherecluase = _wherecluase + " limit 1";
                    cmd1 = new MySqlConnector.MySqlCommand("sp_select_company_bank_deatils");
                    cmd1.CommandType = CommandType.StoredProcedure;
                    cmd1.Parameters.AddWithValue("_wherecluase", _wherecluase);
                    cmd1.Parameters.AddWithValue("_Timezone_Date", (string)CompanyInfo.gettime(Obj.Client_ID, HttpContext));
                    db = (DataTable)db_connection.ExecuteQueryDataTableProcedure(cmd1);
                }
                List<Dictionary<string, object>> Bankaccount = new List<Dictionary<string, object>>();

                foreach (DataRow dr in db.Rows)
                {
                    Dictionary<string, object> bank = new Dictionary<string, object>();
                    bank["companyBankName"] = dr["BankName"];
                    bank["companyAccountHolderName"] = dr["AccountHolderName"];
                    bank["companyAccountNumber"] = dr["AccountNumber"];
                    bank["bankBranch"] = dr["Branch"];
                    bank["bankSortCode"] = dr["Sort_ID"];
                    bank["bankIBAN"] = dr["IBAN"];
                    Bankaccount.Add(bank);
                }
                validateJsonData = new { response = true, responseCode = "00", data = Bankaccount };
                return new JsonResult(validateJsonData);
            }
            catch (Exception ex)
            {
                validateJsonData = new { response = false, responseCode = "01", data = "Invalid Request" };
                CompanyInfo.InsertErrorLogTracker(ex.ToString(), 0, 0, 0, 0, "BankAccountDetails", Convert.ToInt32(Obj.Branch_ID), Convert.ToInt32(Obj.Client_ID), "", HttpContext);
                return new JsonResult(validateJsonData);
            }
        }

    }
}
