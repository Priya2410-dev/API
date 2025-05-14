using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Data;
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

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Calyx_Solutions.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RelationController : ControllerBase
    {

        [HttpPost]
        [Authorize]
        [Route("relationlist")]
        public JsonResult SelectRelations([FromBody] JsonElement objdata)
        {
            HttpContext context = HttpContext;
            var returnJsonData = (dynamic)null;
            Model.response.WebResponse response = null;
            string json = System.Text.Json.JsonSerializer.Serialize(objdata);
            dynamic data = JObject.Parse(json);

            CompanyInfo.InsertrequestLogTracker("relationlist full request body: " + JObject.Parse(json ), 0, 0, 0, 0, "SelectRelations", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
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
                        returnJsonData = new { response = false, responseCode = "02", data = "Invalid clientID field." };
                        return new JsonResult(returnJsonData);
                    }
                    else if (data.branchID == "" || data.branchID == null)
                    {
                        returnJsonData = new { response = false, responseCode = "02", data = "Invalid branchID field." };
                        return new JsonResult(returnJsonData);
                    }                    
                }
                catch (Exception ex)
                {
                    Model.ActivityLogTracker _objActivityLog = new Model.ActivityLogTracker();
                    _objActivityLog.Activity = " Exception Relation List Error: " + ex.ToString() + " ";
                    _objActivityLog.FunctionName = "SelectRelations";
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

                Beneficiary obj = JsonConvert.DeserializeObject<Beneficiary>(json);
                obj.Client_ID = data.clientID;
                obj.Branch_ID = data.branchID;

                if (!SqlInjectionProtector.ValidateObjectForSqlInjection(obj) || !SqlInjectionProtector.ValidateObjectForScriptSqlInjection(obj))
                {
                    return new JsonResult(returnJsonData) { StatusCode = StatusCodes.Status400BadRequest, Value = "Invalid input detected." };
                }

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
                

                Service.srvBeneficiary srv = new Service.srvBeneficiary(HttpContext);
                DataTable li1 = srv.SelectRelations(obj);                
                if (li1 != null && li1.Rows.Count > 0)
                {
                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_SUCCESS);
                    response.data = "success";
                    response.ObjData = li1;
                    response.ResponseCode = 0;

                    var relationshipData = li1.Rows.OfType<DataRow>()
                .Select(row => li1.Columns.OfType<DataColumn>()
                    .ToDictionary(col => col.ColumnName, c => row[c]));

                    foreach (DataColumn column in li1.Columns)
                    {
                        // Replace "OldColumnName" with the desired new name
                        column.ColumnName = column.ColumnName.Replace("Relation_ID", "relationID");
                        column.ColumnName = column.ColumnName.Replace("Relation", "relation");
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
                objError.Error = "Api : Relation --" + ex.ToString();
                objError.Date = DateTime.Now;
                objError.User_ID = 1;
                 
                objError.Client_ID = data.clientID;
                objError.Function_Name = "SelectRelations";
                Service.srvErrorLog srvError = new Service.srvErrorLog();
                srvError.Create(objError, HttpContext);
                returnJsonData = new { response = false, responseCode = "01", data = "Invalid request." };
                return new JsonResult(returnJsonData);
            }
            return new JsonResult(returnJsonData);
        }


        }
}
