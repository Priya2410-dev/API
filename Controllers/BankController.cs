using Microsoft.AspNetCore.Mvc;
using static Google.Apis.Requests.BatchRequest;
using Microsoft.AspNetCore.Authorization;
using System.Web.DynamicData;
using System.Text.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Web.Helpers;
using Calyx_Solutions.Model;
using System.Data;
using Microsoft.Ajax.Utilities;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Calyx_Solutions.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BankController : ControllerBase
    {
        [HttpPost]
        [Authorize]
        [Route("banklist")]
        public JsonResult banklist([FromBody] JsonElement objdata)
        {
            var returnJsonData = (dynamic)null;
            Model.response.WebResponse response = null;
            string jsonData = System.Text.Json.JsonSerializer.Serialize(objdata);
            dynamic data = JObject.Parse(jsonData);
            CompanyInfo.InsertrequestLogTracker("banklist full request body: " + JObject.Parse(jsonData), 0, 0, 0, 0, "banklist", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
            try
            {
                #region validateData
                try
                {
                    int number;
                    if (!int.TryParse(Convert.ToString(data.clientID), out number))
                    {
                        var errorResponse = new { response = false, responseCode = "02", data = "Invalid input detected." };
                        return new JsonResult(errorResponse) { StatusCode = 400 };
                    }
                    else if (!int.TryParse(Convert.ToString(data.branchID), out number))
                    {
                        var errorResponse = new { response = false, responseCode = "02", data = "Invalid input detected." };
                        return new JsonResult(errorResponse) { StatusCode = 400 };
                    }
                    else if (!int.TryParse(Convert.ToString(data.countryID), out number))
                    {
                        var errorResponse = new { response = false, responseCode = "02", data = "Invalid input detected." };
                        return new JsonResult(errorResponse) { StatusCode = 400 };
                    }                    
                }
                catch (Exception ex)
                {
                    Model.ActivityLogTracker _objActivityLog = new Model.ActivityLogTracker();
                    _objActivityLog.Activity = " Exception Found Bank Error: " + ex.ToString() + " ";
                    _objActivityLog.FunctionName = "banklist";
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

                Country obj = JsonConvert.DeserializeObject<Country>(jsonData);
                obj.Id = data.countryID;
                obj.Client_ID = data.clientID;

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

                List<Model.Bank> _lst = new List<Model.Bank>();
                Service.srvBank srv = new Service.srvBank();
                _lst = srv.Select_Bank_Master(obj);

                if (_lst != null)
                {
                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_SUCCESS);
                    response.data = "success";
                    response.ObjData = _lst;
                    response.ResponseCode = 0;

                    Service.ListtoDataTableConverter converter = new Service.ListtoDataTableConverter();
                    DataTable li1 = converter.ToDataTable(_lst);

                    string[] ColumnsToBeDeleted = { "login", "Code", "IFSCCode", "BranchCode", "Branch", "Client_ID", "Branch_ID", "BICcodes" };
                    foreach (string ColName in ColumnsToBeDeleted)
                    {
                        if (li1.Columns.Contains(ColName))
                            li1.Columns.Remove(ColName);
                    }

                    var relationshipData = li1.Rows.OfType<DataRow>()
                .Select(row => li1.Columns.OfType<DataColumn>()
                    .ToDictionary(col => col.ColumnName, c => row[c]));

                    foreach (DataColumn column in li1.Columns)
                    {
                        // Replace "OldColumnName" with the desired new name
                        column.ColumnName = column.ColumnName.Replace("Id", "bankId");
                        column.ColumnName = column.ColumnName.Replace("Name", "bankName");
                    }

                    returnJsonData = new { response = true, responseCode = "00", data = relationshipData };
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
                response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_TECHNICAL_ERROR);
                response.sErrorExceptionText = ex.ToString();
                response.ResponseMessage = "technical error";
                response.ResponseCode = 3;
                //Error Log Handled
                Model.ErrorLog objError = new Model.ErrorLog();
                objError.User = new Model.User();
                objError.Error = "Api : bank list --" + ex.ToString();
                objError.Date = DateTime.Now;
                objError.User_ID = 1;

                objError.Client_ID = data.clientID;
                objError.Function_Name = "banklist";
                Service.srvErrorLog srvError = new Service.srvErrorLog();
                srvError.Create(objError, HttpContext);
                returnJsonData = new { response = false, responseCode = "01", data = "Invalid request." };
                return new JsonResult(returnJsonData);
            }
            return new JsonResult(returnJsonData);


        }
        }
}
