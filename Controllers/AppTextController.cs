using Calyx_Solutions.Model;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Data;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;

namespace Calyx_Solutions.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppTextController : ControllerBase
    {
        [HttpPost]
        //[Authorize]
        [Route("getapptext")]
        public IActionResult Get_App_Texts([FromBody] JsonElement obj1)
        {
            HttpContext context = HttpContext;
            string json = System.Text.Json.JsonSerializer.Serialize(obj1);
            CompanyInfo.InsertrequestLogTracker("Get_App_Texts full request body: " + JObject.Parse(json), 0, 0, 0, 0, "getapptext", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
            dynamic data = JObject.Parse(json); string Status = string.Empty;
            Model.response.WebResponse response = null;
            var jsonData = new { };
            Customer obj = new Customer();

            var validateJsonData = (dynamic)null;
            // var cm = li.SingleOrDefault();
            DataTable dt = new DataTable();

            int number;
            if (!int.TryParse(Convert.ToString(data.Client_ID), out number))
            {
                validateJsonData = new { response = false, responseCode = "02", data = "Invalid Client ID." };
                return new JsonResult(validateJsonData);
            }
            if (!int.TryParse(Convert.ToString(data.Branch_ID), out number))
            {
                validateJsonData = new { response = false, responseCode = "02", data = "Invalid Branch ID." };
                return new JsonResult(validateJsonData);
            }

            try
            {
                obj = JsonConvert.DeserializeObject<Customer>(json);

                if (!SqlInjectionProtector.ValidateObjectForSqlInjection(obj) || !SqlInjectionProtector.ValidateObjectForScriptSqlInjection(obj))
                {                    
                    var errorResponse = new { response = false, responseCode = "02", data = "Invalid input detected." };
                    return new JsonResult(errorResponse) { StatusCode = 400 };
                }

                Service.srvCommon srv = new Service.srvCommon();
                dt = srv.GetAppTexts(obj, context);
                if (dt != null && dt.Rows.Count > 0)
                {
                    //string JSONresult = JsonConvert.SerializeObject(dt);
                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_SUCCESS);
                    response.ResponseCode = 0;
                    response.ObjData = dt;
                    var dtData = dt.Rows.OfType<DataRow>()
                    .Select(row => dt.Columns.OfType<DataColumn>()
                    .ToDictionary(col => col.ColumnName, c => row[c].ToString()));

                    validateJsonData = new { response = true, responseCode = "00", data = dtData };
                    return new JsonResult(validateJsonData);
                }
                else
                {
                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_TECHNICAL_ERROR);
                    response.ResponseMessage = "No Records Found.";
                    response.ResponseCode = 0;

                    validateJsonData = new { response = false, responseCode = "02", data = response.ResponseMessage };
                    return new JsonResult(validateJsonData);
                }
            }
            catch (Exception ex)
            {
                string Activity = "Api Get_App_Texts: " + ex.ToString() + " ";
                validateJsonData = new { response = false, responseCode = "01", data = "Invalid Request" };
                CompanyInfo.InsertErrorLogTracker(Activity.ToString(), 0, 0, 0, 0, "Get_App_Texts", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "", context);
                return new JsonResult(validateJsonData);
            }

            return new JsonResult(jsonData);
        }
    }
}
