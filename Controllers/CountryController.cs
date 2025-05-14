
using Microsoft.AspNetCore.Mvc;
using static Google.Apis.Requests.BatchRequest;
using System.Web.DynamicData;
using System.Text.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Web.Helpers;
using Calyx_Solutions.Model;
using System.Data;
using Microsoft.AspNetCore.Authorization;
using Calyx_Solutions.Service;
using static Calyx_Solutions.mtsIntegrationmethods;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Calyx_Solutions.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountryController : ControllerBase
    {
        [HttpPost]        
        [Route("countrylist")]
        public JsonResult countrylist([FromBody] JsonElement objdata)
        {
            var returnJsonData = (dynamic)null;
            Model.response.WebResponse response = null;
            string jsonData = System.Text.Json.JsonSerializer.Serialize(objdata);
            dynamic data = JObject.Parse(jsonData);
            CompanyInfo.InsertrequestLogTracker("countrylist full request body: " + JObject.Parse(jsonData), 0, 0, 0, 0, "countrylist", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
            List<Model.Beneficiary> _lst = new List<Model.Beneficiary>();
            try
            {
                if (!SqlInjectionProtector.ReadJsonElementValues(objdata) || !SqlInjectionProtector.ReadJsonElementValuesScript(objdata))
                {
                    var errorResponse = new { response = false, responseCode = "02", data = "Invalid input detected." };
                    return new JsonResult(errorResponse) { StatusCode = 400 };
                }

                #region validateData
                try
                {
                    int number;
                    if (!int.TryParse(Convert.ToString(data.clientID), out number))
                    {
                        returnJsonData = new { response = false, responseCode = "02", data = "Invalid Client ID." };
                        return new JsonResult(returnJsonData);
                    }
                    else if (!int.TryParse(Convert.ToString(data.branchID), out number))
                    {
                        returnJsonData = new { response = false, responseCode = "02", data = "Invalid Branch ID." };
                        return new JsonResult(returnJsonData);
                    }


                    object o = JsonConvert.DeserializeObject(jsonData);
                    bool checkinjection = CompanyInfo.checksqlinjectiondata(o);
                    if (!checkinjection)
                    {
                        response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_TECHNICAL_ERROR);
                        response.ResponseMessage = "Invalid Field Values.";
                        response.ResponseCode = 6;
                        returnJsonData = new { response = false, responseCode = "02", data = response.ResponseMessage };
                        return new JsonResult(returnJsonData);
                    }
                }
                catch (Exception ex)
                {
                    Model.ActivityLogTracker _objActivityLog = new Model.ActivityLogTracker();
                    _objActivityLog.Activity = " Exception Found Coutry Error: " + ex.ToString() + " ";
                    _objActivityLog.FunctionName = "countrylist";
                    _objActivityLog.Transaction_ID = 0;
                    _objActivityLog.WhoAcessed = 1;
                    _objActivityLog.Branch_ID = 0;
                    _objActivityLog.Client_ID = 0;

                    Service.srvActivityLogTracker srvActivityLog = new Service.srvActivityLogTracker();
                    int _i = srvActivityLog.Create(_objActivityLog, HttpContext);

                    returnJsonData = new { response = false, responseCode = "01", data = "Field is missing." };
                    return new JsonResult(returnJsonData);
                }
                #endregion

                Beneficiary obj = JsonConvert.DeserializeObject<Beneficiary>(jsonData);
                obj.Client_ID= data.clientID;
                obj.Branch_ID= data.branchID;

                if (data.countryID == "" || data.countryID == null)
                {
                    obj.Country_ID = 0;
                }
                else
                {
                    obj.Country_ID = data.countryID;
                }
                try { obj.Customer_ID = CompanyInfo.Decrypt(Convert.ToString(data.customerID), true); } catch (Exception ex) { obj.Customer_ID = ""; }
                Service.srvBeneficiary srv = new Service.srvBeneficiary(HttpContext);
                //    
                DataTable dt= srv.Select_BeneficiaryCountries(obj);
                /// Fetch Payout Countries only if Customer_ID is not null or empty
                List<int> payoutCountryList = new List<int>();
                if (!string.IsNullOrEmpty(obj.Customer_ID))
                {
                    string payoutCountries = srv.GetPayoutCountries(obj.Customer_ID);
                    if (!string.IsNullOrEmpty(payoutCountries))
                    {
                        payoutCountryList = payoutCountries?.Split(',').Select(int.Parse).ToList() ?? new List<int>();
                    }                    
                }

                /*   
                   DataTable[] dt = new DataTable[3];
                    dt[0] = srv.Select_BeneficiaryCountries(obj);
                   dt[0].TableName = "Beneficary";
                   if (obj.Country_ID == null || obj.Country_ID == 0)
                   {
                       dt[1] = srv.GetConfig(obj);
                       dt[1].TableName = "Beneficary_Config";
                   }

                   dt[2] = srv.CollectionTypeConfig(obj);
                   dt[2].TableName = "CollectionTypeConfig";
               */
                if (dt != null && dt.Rows.Count > 0)
                {
                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_SUCCESS);
                    response.data = "success";
                    response.ObjData = dt;
                    response.ResponseCode = 0;
                    try
                    {
                        dt.Columns.Add("sort_order", typeof(int));
                        DataRow[] payoutRows = dt.Select("Country_ID IN (" + string.Join(",", payoutCountryList) + ")");

                        foreach (DataRow row in dt.Rows)
                        {
                            int countryID = Convert.ToInt32(row["Country_ID"]);
                            row["sort_order"] = payoutCountryList.Contains(countryID) ? 0 : 1;
                        }

                        dt.DefaultView.Sort = "sort_order, Popular_country, preferred_flag, Country_Name";
                        dt = dt.DefaultView.ToTable();
                    }
                    catch(Exception ex)
                    {
                        Model.ErrorLog objError = new Model.ErrorLog();
                        objError.User = new Model.User();
                        objError.Branch = new Model.Branch();
                        objError.Client = new Model.Client();

                        objError.Error = "Api : countrylist --" + ex.ToString();
                        objError.Branch_ID = 1;
                        objError.Date = DateTime.Now;
                        objError.User_ID = 1;
                        objError.Id = 1;
                        objError.Function_Name = "countrylist";
                        Service.srvErrorLog srvError = new Service.srvErrorLog();
                        srvError.Create(objError, HttpContext);
                    }

                    DataRow[] result = dt.Select("Country_ID <> "+ obj.Country_ID);

                    if (data.countryID == "" || data.countryID == null)
                    {
                        foreach (DataRow row in result)
                        {
                           // if (row["Country_ID"].ToString().Trim().ToUpper().Equals(Convert.ToString(obj.Country_ID))) { } else { dt.Rows.Remove(row); }
                        }
                    }
                    else
                    {
                        foreach (DataRow row in result)
                        {
                            if (row["Country_ID"].ToString().Trim().ToUpper().Equals(Convert.ToString(obj.Country_ID))) { } else { dt.Rows.Remove(row); }
                        }
                    }

                    var rateData = dt.Rows.OfType<DataRow>()
                    .Select(row => dt.Columns.OfType<DataColumn>()
                        .ToDictionary(col => col.ColumnName, c => row[c].ToString()));

                    foreach (DataColumn column in dt.Columns)
                    {
                        // Replace "OldColumnName" with the desired new name
                        column.ColumnName = column.ColumnName.Replace("Popular_country", "popularCountry");
                        column.ColumnName = column.ColumnName.Replace("flag", "flag");
                        column.ColumnName = column.ColumnName.Replace("Country_Flag", "countryFlag");
                        column.ColumnName = column.ColumnName.Replace("Country_Code", "countryCode");
                        column.ColumnName = column.ColumnName.Replace("ISO_Code", "ISOCode");
                        column.ColumnName = column.ColumnName.Replace("Country_ID", "countryID");
                        column.ColumnName = column.ColumnName.Replace("Country_Name", "countryName");
                        column.ColumnName = column.ColumnName.Replace("Country_Currency", "countryCurrency");
                        column.ColumnName = column.ColumnName.Replace("preferred_flag", "preferredFlag");                       
                    }

                    returnJsonData = new { response = true, responseCode = "00", data = rateData };
                    return new JsonResult(returnJsonData);
                }
                else
                {
                        response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_TECHNICAL_ERROR);
                        response.ResponseMessage = "No Records Found.";
                        response.ResponseCode = 0;
                        returnJsonData = new { response = false, responseCode = "02", data = response.ResponseMessage };
                        return new JsonResult(returnJsonData);
                }
            }
            catch (Exception ex)
            {
                Model.ErrorLog objError = new Model.ErrorLog();
                objError.User = new Model.User();
                objError.Branch = new Model.Branch();
                objError.Client = new Model.Client();

                objError.Error = "Api : countrylist --" + ex.ToString();
                objError.Branch_ID = 1;
                objError.Date = DateTime.Now;
                objError.User_ID = 1;
                objError.Id = 1;
                objError.Function_Name = "countrylist";
                Service.srvErrorLog srvError = new Service.srvErrorLog();
                srvError.Create(objError, HttpContext);
                returnJsonData = new { response = false, responseCode = "01", data = "Invalid request." };
                return new JsonResult(returnJsonData);
            }
            return new JsonResult(returnJsonData);
        }

        [HttpGet]
        [Route("basecountrycodemaster")]
        public JsonResult countrycodemaster(int clientID)
        {
            var returnJsonData = (dynamic)null;
            Model.response.WebResponse response = null;

            List<Model.Country> _lst = new List<Model.Country>();
            try
            {
                Country obj = new Country() ;
                obj.Client_ID = clientID;

                int number;
                if (!int.TryParse(Convert.ToString(clientID), out number))
                {
                    returnJsonData = new { response = false, responseCode = "02", data = "Invalid Client ID." };
                    return new JsonResult(returnJsonData);
                }

                Service.srvCountry srv = new Service.srvCountry();
                _lst = srv.Select_base_Country_Master(obj);
                if (_lst != null)
                {
                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_SUCCESS);
                    response.data = "success";
                    response.ObjData = _lst;
                    response.ResponseCode = 0;

                    Service.ListtoDataTableConverter converter = new Service.ListtoDataTableConverter();
                    DataTable li1 = converter.ToDataTable(_lst);

                    string[] ColumnsToBeDeleted = { "provience_id", "login", "Client_ID", "id", "currency", "deleteStatus", "sendingFlag", "client", "base_country", "chkpostcode_search", "chkbasecountry_mb", "base_currency_id", "base_currency_code" };
                    foreach (string ColName in ColumnsToBeDeleted)
                    {
                        if (li1.Columns.Contains(ColName))
                            li1.Columns.Remove(ColName);
                    }

                    var listData = li1.Rows.OfType<DataRow>()
                .Select(row => li1.Columns.OfType<DataColumn>()
                    .ToDictionary(col => col.ColumnName, c => row[c].ToString()));

                    foreach (DataColumn column in li1.Columns)
                    {
                        // Replace "OldColumnName" with the desired new name
                        column.ColumnName = column.ColumnName.Replace("Code", "code");
                        column.ColumnName = column.ColumnName.Replace("Name", "countryName");
                        column.ColumnName = column.ColumnName.Replace("CurrencyCode", "currencyCode");
                    }

                    returnJsonData = new { response = true, responseCode = "00", data = listData };
                    return new JsonResult(returnJsonData);               
                }
                else
                {
                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_TECHNICAL_ERROR);
                    response.ResponseMessage = "No Records Found.";
                    response.ResponseCode = 0;
                    returnJsonData = new { response = false, responseCode = "02", data = response.ResponseMessage };
                    return new JsonResult(returnJsonData);
                }                 
            }
            catch (Exception ex)
            {
                Model.ErrorLog objError = new Model.ErrorLog();
                objError.User = new Model.User();
                objError.Branch = new Model.Branch();
                objError.Client = new Model.Client();

                objError.Error = "Api : countrycodemaster --" + ex.ToString();
                objError.Branch_ID = 1;
                objError.Date = DateTime.Now;
                objError.User_ID = 1;
                objError.Id = 1;
                objError.Function_Name = "countrycodemaster";
                Service.srvErrorLog srvError = new Service.srvErrorLog();
                srvError.Create(objError, HttpContext);
                returnJsonData = new { response = false, responseCode = "01", data = "Invalid request." };
                CompanyInfo.InsertErrorLogTracker(" Exception countrycodemaster Error: " + ex.ToString(), 0, 0, 0, 0, "countrycodemaster", Convert.ToInt32(0), Convert.ToInt32(clientID), "", HttpContext);
                return new JsonResult(returnJsonData);
            }
            return new JsonResult(returnJsonData);
        }


        [HttpPost]
        [Route("provience")]
        public JsonResult provience([FromBody] JsonElement objdata)
        {
            var returnJsonData = (dynamic)null;
            Model.response.WebResponse response = null;
            string jsonData = System.Text.Json.JsonSerializer.Serialize(objdata);
            dynamic data = JObject.Parse(jsonData);
            CompanyInfo.InsertrequestLogTracker("provience full request body: " + JObject.Parse(jsonData), 0, 0, 0, 0, "provience", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
            List<Model.Country> _lst = new List<Model.Country>();
            try
            {
                if (!SqlInjectionProtector.ReadJsonElementValues(objdata) || !SqlInjectionProtector.ReadJsonElementValuesScript(objdata))
                {
                    var errorResponse = new { response = false, responseCode = "02", data = "Invalid input detected." };
                    return new JsonResult(errorResponse) { StatusCode = 400 };
                }

                int number;
                if (!int.TryParse(Convert.ToString(data.clientID), out number))
                {
                    returnJsonData = new { response = false, responseCode = "02", data = "Invalid Client ID." };
                    return new JsonResult(returnJsonData);
                }
                else if (!int.TryParse(Convert.ToString(data.branchID), out number))
                {
                    returnJsonData = new { response = false, responseCode = "02", data = "Invalid Branch ID." };
                    return new JsonResult(returnJsonData);
                }
                else if (!int.TryParse(Convert.ToString(data.countryID), out number))
                {
                    returnJsonData = new { response = false, responseCode = "02", data = "Invalid countryID." };
                    return new JsonResult(returnJsonData);
                }
                

                /*Country obj = new Country();
                obj.Client_ID = clientID;*/

                Country obj = JsonConvert.DeserializeObject<Country>(jsonData);
                obj.Client_ID = data.clientID;
                obj.base_country = data.countryID;
                Service.srvCountry srv = new Service.srvCountry();
                DataTable li1 = srv.select_provience(obj);

                if (li1 != null && li1.Rows.Count > 0)
                {
                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_SUCCESS);
                    response.data = "success";
                    response.ObjData = li1;
                    response.ResponseCode = 0;

                    string[] ColumnsToBeDeleted = { "country", "client_id", "status" };
                    foreach (string ColName in ColumnsToBeDeleted)
                    {
                        if (li1.Columns.Contains(ColName))
                            li1.Columns.Remove(ColName);
                    }

                    var rateData = li1.Rows.OfType<DataRow>()
                    .Select(row => li1.Columns.OfType<DataColumn>()
                        .ToDictionary(col => col.ColumnName, c => row[c]));

                    foreach (DataColumn column in li1.Columns)
                    {
                        // Replace "OldColumnName" with the desired new name
                        column.ColumnName = column.ColumnName.Replace("provience", "provience");
                        column.ColumnName = column.ColumnName.Replace("code", "code");
                        column.ColumnName = column.ColumnName.Replace("ID", "id");
                    }

                    returnJsonData = new { response = true, responseCode = "00", data = rateData };
                    return new JsonResult(returnJsonData);
                }
                else
                {
                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_TECHNICAL_ERROR);
                    response.ResponseMessage = "No Records Found.";
                    response.ResponseCode = 0;
                    returnJsonData = new { response = false, responseCode = "02", data = response.ResponseMessage };
                    return new JsonResult(returnJsonData);
                }               
            }
            catch (Exception ex)
            {
                Model.ErrorLog objError = new Model.ErrorLog();
                objError.User = new Model.User();
                objError.Branch = new Model.Branch();
                objError.Client = new Model.Client();

                objError.Error = "Api : provience --" + ex.ToString();
                objError.Branch_ID = 1;
                objError.Date = DateTime.Now;
                objError.User_ID = 1;
                objError.Id = 1;
                objError.Function_Name = "provience";
                Service.srvErrorLog srvError = new Service.srvErrorLog();
                srvError.Create(objError, HttpContext);
                returnJsonData = new { response = false, responseCode = "01", data = "Invalid request." };
                CompanyInfo.InsertErrorLogTracker(" Exception provience Error: " + ex.ToString(), 0, 0, 0, 0, "provience", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
                return new JsonResult(returnJsonData);
            }
            return new JsonResult(returnJsonData);
        }


        [HttpPost]
        [Route("lga")]
        public JsonResult Select_Lgas_details([FromBody] JsonElement objdata)
        {
            var returnJsonData = (dynamic)null;
            Model.response.WebResponse response = null;
            string jsonData = System.Text.Json.JsonSerializer.Serialize(objdata);
            dynamic data = JObject.Parse(jsonData);
            CompanyInfo.InsertrequestLogTracker("lga full request body: " + JObject.Parse(jsonData), 0, 0, 0, 0, "Select_Lgas_details", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
            List<Model.Country> _lst = new List<Model.Country>();
            try
            {
                int number;
                if (!int.TryParse(Convert.ToString(data.Client_ID), out number))
                {
                    returnJsonData = new { response = false, responseCode = "02", data = "Invalid Client Id." };
                    return new JsonResult(returnJsonData);
                }
                else if (!int.TryParse(Convert.ToString(data.Branch_ID), out number))
                {
                    returnJsonData = new { response = false, responseCode = "02", data = "Invalid Branch Id." };
                    return new JsonResult(returnJsonData);
                }
                else if (!int.TryParse(Convert.ToString(data.provience_id), out number))
                {
                    returnJsonData = new { response = false, responseCode = "02", data = "Invalid Provience Id." };
                    return new JsonResult(returnJsonData);
                }


                /*Country obj = new Country();
                obj.Client_ID = clientID;*/

                Country obj = JsonConvert.DeserializeObject<Country>(jsonData);
                //obj.Client_ID = data.clientID;
                //obj.base_country = data.countryID;
                Service.srvCountry srv = new Service.srvCountry();
                DataTable li1 = srv.select_LGA_details(obj);

                if (li1 != null && li1.Rows.Count > 0)
                {
                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_SUCCESS);
                    response.data = "success";
                    response.ObjData = li1;
                    response.ResponseCode = 0;

                    string[] ColumnsToBeDeleted = { "country", "client_id", "status" };
                    foreach (string ColName in ColumnsToBeDeleted)
                    {
                        if (li1.Columns.Contains(ColName))
                            li1.Columns.Remove(ColName);
                    }

                    var rateData = li1.Rows.OfType<DataRow>()
                    .Select(row => li1.Columns.OfType<DataColumn>()
                        .ToDictionary(col => col.ColumnName, c => row[c]));

                    foreach (DataColumn column in li1.Columns)
                    {
                        // Replace "OldColumnName" with the desired new name
                        column.ColumnName = column.ColumnName.Replace("provience", "provience");
                        column.ColumnName = column.ColumnName.Replace("code", "code");
                        column.ColumnName = column.ColumnName.Replace("ID", "id");
                    }

                    returnJsonData = new { response = true, responseCode = "00", data = rateData };
                    return new JsonResult(returnJsonData);
                }
                else
                {
                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_TECHNICAL_ERROR);
                    response.ResponseMessage = "No Records Found.";
                    response.ResponseCode = 0;
                    returnJsonData = new { response = false, responseCode = "02", data = response.ResponseMessage };
                    return new JsonResult(returnJsonData);
                }
            }
            catch (Exception ex)
            {
                Model.ErrorLog objError = new Model.ErrorLog();
                objError.User = new Model.User();
                objError.Branch = new Model.Branch();
                objError.Client = new Model.Client();

                objError.Error = "Api : lga --" + ex.ToString();
                objError.Branch_ID = 1;
                objError.Date = DateTime.Now;
                objError.User_ID = 1;
                objError.Id = 1;
                objError.Function_Name = "Select_Lgas_details";
                Service.srvErrorLog srvError = new Service.srvErrorLog();
                srvError.Create(objError, HttpContext);
                returnJsonData = new { response = false, responseCode = "01", data = "Invalid request." };
                CompanyInfo.InsertErrorLogTracker(" Exception lga Error: " + ex.ToString(), 0, 0, 0, 0, "lga", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
                return new JsonResult(returnJsonData);
            }
            return new JsonResult(returnJsonData);
        }
    }
}
