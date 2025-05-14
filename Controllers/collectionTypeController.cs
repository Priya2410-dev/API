using Calyx_Solutions.Model;
using System.Data;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using static Google.Apis.Requests.BatchRequest;
using Microsoft.AspNetCore.Authorization;
using System.Web.DynamicData;


namespace Calyx_Solutions.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class collectionTypeController : ControllerBase
    {
        [HttpPost]       
        [Route("collectiontypelist")]
        public JsonResult collectiontypelist([FromBody] JsonElement objdata)
        {
            var returnJsonData = (dynamic)null;
            Model.response.WebResponse response = null;
            string json = System.Text.Json.JsonSerializer.Serialize(objdata);
            dynamic data = JObject.Parse(json);
            CompanyInfo.InsertrequestLogTracker("collectiontypelist full request body: " + JObject.Parse(json ), 0, 0, 0, 0, "collectiontypelist", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
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
                                       
                }
                catch (Exception ex)
                {
                    Model.ActivityLogTracker _objActivityLog = new Model.ActivityLogTracker();
                    _objActivityLog.Activity = " Exception Found Rate Error: " + ex.ToString() + " ";
                    _objActivityLog.FunctionName = "collectiontypelist";
                    _objActivityLog.Transaction_ID = 0;
                    _objActivityLog.WhoAcessed = 1;
                    _objActivityLog.Branch_ID = 0;
                    _objActivityLog.Client_ID = 0;

                    Service.srvActivityLogTracker srvActivityLog = new Service.srvActivityLogTracker();
                    int _i = srvActivityLog.Create(_objActivityLog, HttpContext);

                    returnJsonData = new { response = false, responseCode = "02", data = "Field is missing." };
                    return new JsonResult(returnJsonData);
                }
                #endregion

                Transaction obj = JsonConvert.DeserializeObject<Transaction>(json);

                obj.Client_ID = data.clientID;
               
                obj.Country_ID = data.countryID;
                obj.CB_ID = data.branchID;
                obj.Branch_ID = data.branchID;

                object o = JsonConvert.DeserializeObject(json);
                bool checkinjection = CompanyInfo.checksqlinjectiondata(o);
                if (!checkinjection)
                {
                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_TECHNICAL_ERROR);
                    response.ResponseMessage = "Invalid Field Values.";
                    response.ResponseCode = 6;
                    returnJsonData = new { response = false, responseCode = "02", data = response.ResponseMessage };
                    return new JsonResult(returnJsonData);
                }

                Service.srvCompanyDetails cmp = new Service.srvCompanyDetails(HttpContext);
                List<Model.CompanyDetails> li = new List<Model.CompanyDetails>();
                

                Service.srvPaymentDepositType srv = new Service.srvPaymentDepositType();
                DataTable li1 = srv.GetPayDepositTypes(obj);
                if (li1 != null && li1.Rows.Count > 0)
                {
                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_SUCCESS);
                    response.data = "success";
                    response.ObjData = li1;
                    response.ResponseCode = 0;

                    #region removeColumn
                    if (li1.Columns.Contains("Delete_Status"))
                    {
                        li1.Columns.Remove("Delete_Status");
                    }
                    if (li1.Columns.Contains("TransferTypeFlag"))
                    {
                        li1.Columns.Remove("TransferTypeFlag");
                    }
                    if (li1.Columns.Contains("Client_ID"))
                    {
                        li1.Columns.Remove("Client_ID");
                    }
                    if (li1.Columns.Contains("ShowOnWebsite"))
                    {
                        li1.Columns.Remove("ShowOnWebsite");
                    }
                    if (li1.Columns.Contains("ShowOnCustSide"))
                    {
                        li1.Columns.Remove("ShowOnCustSide");
                    }
                    if (li1.Columns.Contains("ShowOnAdmin"))
                    {
                        li1.Columns.Remove("ShowOnAdmin");
                    }
                    if (li1.Columns.Contains("preferred_flag"))
                    {
                        li1.Columns.Remove("preferred_flag");
                    }
                    if (li1.Columns.Contains("preferred_customer"))
                    {
                        li1.Columns.Remove("preferred_customer");
                    }
                    if (li1.Columns.Contains("preferred_agent"))
                    {
                        li1.Columns.Remove("preferred_agent");
                    }
                    #endregion

                    var relationshipData = li1.Rows.OfType<DataRow>()
                .Select(row => li1.Columns.OfType<DataColumn>()
                    .ToDictionary(col => col.ColumnName, c => row[c].ToString()));

                    foreach (DataColumn column in li1.Columns)
                    {                        
                        // Replace "OldColumnName" with the desired new name
                        column.ColumnName = column.ColumnName.Replace("PaymentDepositType_ID", "paymentCollectionTypeID");
                        column.ColumnName = column.ColumnName.Replace("Type_Name", "typeName");
                        column.ColumnName = column.ColumnName.Replace("Max_Amount", "maxAmount");
                    }

                    returnJsonData = new { response = true, responseCode = "00", data = relationshipData };
                    return new JsonResult(returnJsonData);
                }
                else
                {
                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_TECHNICAL_ERROR);
                    response.ResponseMessage = "No record found";
                    response.ResponseCode = 1;
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
                objError.Error = "Api : Collection type --" + ex.ToString();
                objError.Date = DateTime.Now;
                objError.User_ID = 1;

                objError.Client_ID = data.clientID;
                objError.Function_Name = "collectiontypelist";
                Service.srvErrorLog srvError = new Service.srvErrorLog();
                srvError.Create(objError, HttpContext);
                returnJsonData = new { response = false, responseCode = "01", data = "Invalid request." };
                return new JsonResult(returnJsonData);
            }
            return new JsonResult(returnJsonData);
        }



    }
}
