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

namespace Calyx_Solutions.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MobileProviderController : ControllerBase
    {
        [HttpPost]
        [Authorize]
        [Route("providerlist")]
        public JsonResult mobileproviderlist([FromBody] JsonElement objdata)
        {
            var returnJsonData = (dynamic)null;
            Model.response.WebResponse response = null;
            string jsonData = System.Text.Json.JsonSerializer.Serialize(objdata);
            dynamic data = JObject.Parse(jsonData);

            try
            {
                CompanyInfo.InsertrequestLogTracker("providerlist full request body: " + JObject.Parse(jsonData), 0, 0, 0, 0, "mobileproviderlist", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
                if (!SqlInjectionProtector.ReadJsonElementValues(objdata) || !SqlInjectionProtector.ReadJsonElementValuesScript(objdata))
                {
                    var errorResponse = new { response = false, responseCode = "02", data = "Invalid input detected." };
                    return new JsonResult(errorResponse) { StatusCode = 400 };
                }

                #region validateData
                try
                {
                    if (data.clientID == "" || data.clientID == null)
                    {
                        returnJsonData = new { response = false, responseCode = "02", data = "Invalid Client ID." };
                        return new JsonResult(returnJsonData);
                    }
                    else if (data.branchID == "" || data.branchID == null)
                    {
                        returnJsonData = new { response = false, responseCode = "02", data = "Invalid Branch ID." };
                        return new JsonResult(returnJsonData);
                    }
                    else if (data.countryID == "" || data.countryID == null)
                    {
                        returnJsonData = new { response = false, responseCode = "02", data = "Invalid Country ID." };
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
                    _objActivityLog.Activity = " Exception Found Mobile Provider Error: " + ex.ToString() + " ";
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
                obj.Client_ID = data.clientID;
                obj.Branch_ID = data.branchID;
                obj.Country_ID = data.countryID;

                Service.srvBeneficiary srv3 = new Service.srvBeneficiary(HttpContext);
               
                DataTable li1 = srv3.GetProviders(obj);
                
                if (li1 != null && li1.Rows.Count > 0)
                {
                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_SUCCESS);
                    response.data = "success";
                    response.ObjData = li1;
                    response.ResponseCode = 0;

                    string[] ColumnsToBeDeleted = {   "Delete_Status", "Client_ID", "Mapping_ID", "mobileprovider_ID", "Country_ID", "Delete_Status1", "api_id_list", "Client_ID1" };
                    foreach (string ColName in ColumnsToBeDeleted)
                    {
                        if (li1.Columns.Contains(ColName))
                            li1.Columns.Remove(ColName);
                    }

                    var mobileProviderData = li1.Rows.OfType<DataRow>()
                    .Select(row => li1.Columns.OfType<DataColumn>()
                        .ToDictionary(col => col.ColumnName, c => row[c]));

                    foreach (DataColumn column in li1.Columns)
                    {
                        // Replace "OldColumnName" with the desired new name
                        column.ColumnName = column.ColumnName.Replace("Mobile_flag", "mobileflag");
                        column.ColumnName = column.ColumnName.Replace("Provider_Id", "providerID");
                        column.ColumnName = column.ColumnName.Replace("Provider_name", "providerName");                    
                    }

                    returnJsonData = new { response = true, responseCode = "00", data = mobileProviderData };
                    return new JsonResult(returnJsonData);
                }
                else
                {
                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_TECHNICAL_ERROR);
                    response.ResponseMessage = "Record not found.";
                    response.ResponseCode = 1;
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

                objError.Error = "Api : mobileproviderlist --" + ex.ToString();
                objError.Branch_ID = 1;
                objError.Date = DateTime.Now;
                objError.User_ID = 1;
                objError.Id = 1;
                objError.Function_Name = "mobileproviderlist";
                Service.srvErrorLog srvError = new Service.srvErrorLog();
                srvError.Create(objError, HttpContext);
                returnJsonData = new { response = false, responseCode = "01", data = "Invalid request." };
                return new JsonResult(returnJsonData);
            }
            return new JsonResult(returnJsonData);

        }


       

    }
}
