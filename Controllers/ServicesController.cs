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
    public class ServicesController : ControllerBase //for Kmoney
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;

        public ServicesController(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            this._configuration = configuration;
            _httpContextAccessor = httpContextAccessor;

        }

        [HttpPost]
        [Authorize]
        [Route("selectfootballleague")]
        public IActionResult Select_football_league([FromBody] JsonElement obj1)
        {
            HttpContext context = HttpContext;
            string json = System.Text.Json.JsonSerializer.Serialize(obj1);
            CompanyInfo.InsertrequestLogTracker("Select_football_league full request body: " + JObject.Parse(json), 0, 0, 0, 0, "selectfootballleague", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
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
                validateJsonData = new { response = false, responseCode = "02", data = "Invalid clientID field." };
                return new JsonResult(validateJsonData);
            }
            if (!int.TryParse(Convert.ToString(data.Branch_ID), out number))
            {
                validateJsonData = new { response = false, responseCode = "02", data = "Invalid branchID field." };
                return new JsonResult(validateJsonData);
            }
            if (!int.TryParse(Convert.ToString(data.User_ID), out number))
            {
                validateJsonData = new { response = false, responseCode = "02", data = "Invalid usedID field." };
                return new JsonResult(validateJsonData);
            }

            try
            {
                obj = JsonConvert.DeserializeObject<Customer>(json);

                Service.srvServices srvServices = new Service.srvServices(context);
                dt = srvServices.Select_football_league(obj, context);
                if (dt != null)
                {
                    //string JSONresult = JsonConvert.SerializeObject(dt);
                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_SUCCESS);
                    response.ResponseCode = 0;
                    response.ObjData = dt;
                    var dtData = dt.Rows.OfType<DataRow>()
                    .Select(row => dt.Columns.OfType<DataColumn>()
                    .ToDictionary(col => col.ColumnName, c => row[c].ToString()));
                    foreach (DataColumn column in dt.Columns)
                    {
                        // Replace "OldColumnName" with the desired new name
                        column.ColumnName = column.ColumnName.Replace("S_ID", "League_ID");
                        column.ColumnName = column.ColumnName.Replace("Services_Name", "League_Name");
                    }
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
                string Activity = "Api Select_football_league: " + ex.ToString() + " ";
                validateJsonData = new { response = false, responseCode = "01", data = "Invalid Request" };
                CompanyInfo.InsertErrorLogTracker(Activity.ToString(), 0, 0, 0, 0, "Select_football_league", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "", context);
                return new JsonResult(validateJsonData);
            }

            return new JsonResult(jsonData);
        }

        [HttpPost]
        [Authorize]
        [Route("selectfootballCountry")]
        public IActionResult Select_FtCountry([FromBody] JsonElement obj1)
        {
            HttpContext context = HttpContext;
            string json = System.Text.Json.JsonSerializer.Serialize(obj1);
            CompanyInfo.InsertrequestLogTracker("Select_football_league full request body: " + JObject.Parse(json), 0, 0, 0, 0, "selectfootballleague", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
            dynamic data = JObject.Parse(json); string Status = string.Empty;
            Model.response.WebResponse response = null;
            var jsonData = new { };
            Services obj = new Services();

            var validateJsonData = (dynamic)null;
            // var cm = li.SingleOrDefault();
            DataTable dt = new DataTable();

            int number;
            if (!int.TryParse(Convert.ToString(data.Client_ID), out number))
            {
                validateJsonData = new { response = false, responseCode = "02", data = "Invalid Client_ID field." };
                return new JsonResult(validateJsonData);
            }
            if (!int.TryParse(Convert.ToString(data.Branch_ID), out number))
            {
                validateJsonData = new { response = false, responseCode = "02", data = "Invalid Branch_ID field." };
                return new JsonResult(validateJsonData);
            }
            if (!int.TryParse(Convert.ToString(data.User_ID), out number))
            {
                validateJsonData = new { response = false, responseCode = "02", data = "Invalid User_ID field." };
                return new JsonResult(validateJsonData);
            }

            try
            {
                obj = JsonConvert.DeserializeObject<Services>(json);

                Service.srvServices srvServices = new Service.srvServices(context);
                dt = srvServices.Select_FtCountry(obj, context);
                if (dt != null)
                {
                    //string JSONresult = JsonConvert.SerializeObject(dt);
                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_SUCCESS);
                    response.ResponseCode = 0;
                    response.ObjData = dt;
                    var dtData = dt.Rows.OfType<DataRow>()
                    .Select(row => dt.Columns.OfType<DataColumn>()
                    .ToDictionary(col => col.ColumnName, c => row[c].ToString()));
                    foreach (DataColumn column in dt.Columns)
                    {
                        // Replace "OldColumnName" with the desired new name
                        column.ColumnName = column.ColumnName.Replace("S_ID", "League_ID");
                        column.ColumnName = column.ColumnName.Replace("Services_Name", "League_Name");
                    }
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
                string Activity = "Api Select_football_league: " + ex.ToString() + " ";
                validateJsonData = new { response = false, responseCode = "01", data = "Invalid Request" };
                CompanyInfo.InsertErrorLogTracker(Activity.ToString(), 0, 0, 0, 0, "Select_football_league", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "", context);
                return new JsonResult(validateJsonData);
            }

            return new JsonResult(jsonData);
        }
        
        [HttpPost]
        [Authorize]
        [Route("saveservices")]
        public IActionResult Save_services([FromBody] JsonElement obj1)
        {
            HttpContext context = HttpContext;
            string json = System.Text.Json.JsonSerializer.Serialize(obj1);
            CompanyInfo.InsertrequestLogTracker("Select_football_league full request body: " + JObject.Parse(json), 0, 0, 0, 0, "selectfootballleague", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
            dynamic data = JObject.Parse(json); string Status = string.Empty;
            Model.response.WebResponse response = null;
            var jsonData = new { };
            Services obj = new Services();

            var validateJsonData = (dynamic)null;
            // var cm = li.SingleOrDefault();
            DataTable dt = new DataTable();

            int number;
            if (!int.TryParse(Convert.ToString(data.Client_ID), out number))
            {
                validateJsonData = new { response = false, responseCode = "02", data = "Invalid ClientID." };
                return new JsonResult(validateJsonData);
            }
            if (!int.TryParse(Convert.ToString(data.Branch_ID), out number))
            {
                validateJsonData = new { response = false, responseCode = "02", data = "Invalid BranchID." };
                return new JsonResult(validateJsonData);
            }
            if (!int.TryParse(Convert.ToString(data.User_ID), out number))
            {
                validateJsonData = new { response = false, responseCode = "02", data = "Invalid UserID." };
                return new JsonResult(validateJsonData);
            }
            if (data.Customer_ID == "" || data.Customer_ID == null)
            {
                validateJsonData = new { response = false, responseCode = "02", data = "Invalid CustomerID." };
                return new JsonResult(validateJsonData);
            }
            if (!int.TryParse(Convert.ToString(data.value), out number))
            {
                validateJsonData = new { response = false, responseCode = "02", data = "Invalid value." };
                return new JsonResult(validateJsonData);
            }

            try
            {
                obj = JsonConvert.DeserializeObject<Services>(json);
                Model.Services _Obj = new Model.Services();
                Service.srvServices srvServices = new Service.srvServices(context);
                _Obj = srvServices.Save_services(obj, context);
                List<Dictionary<string, object>> customerData = new List<Dictionary<string, object>>();
                Dictionary<string, object> customer = new Dictionary<string, object>();
                if (_Obj.Message == "success")
                {
                    //string JSONresult = JsonConvert.SerializeObject(dt);
                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_SUCCESS);
                    response.ResponseCode = 0;
                    response.ObjData = dt;
                    customer["status"] = "success";
                    customerData.Add(customer);

                    validateJsonData = new { response = true, responseCode = "00", data = customerData };
                    return new JsonResult(validateJsonData);
                }
                else
                {
                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_TECHNICAL_ERROR);
                    response.ResponseMessage = "No Records Found.";
                    response.ResponseCode = 0;
                    customer["status"] = "failed";
                    customerData.Add(customer);
                    validateJsonData = new { response = false, responseCode = "02", data = customerData };
                    return new JsonResult(validateJsonData);
                }
            }
            catch (Exception ex)
            {
                string Activity = "Api Select_football_league: " + ex.ToString() + " ";
                validateJsonData = new { response = false, responseCode = "01", data = "Invalid Request" };
                CompanyInfo.InsertErrorLogTracker(Activity.ToString(), 0, 0, 0, 0, "Select_football_league", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "", context);
                return new JsonResult(validateJsonData);
            }

            return new JsonResult(jsonData);
        }

        [HttpPost]
        [Authorize]
        [Route("selectedfootballteams")]
        public IActionResult Selected_football_team([FromBody] JsonElement obj1)
        {
            HttpContext context = HttpContext;
            string json = System.Text.Json.JsonSerializer.Serialize(obj1);
            CompanyInfo.InsertrequestLogTracker("Selected_football_team full request body: " + JObject.Parse(json), 0, 0, 0, 0, "selectedfootballteams", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
            dynamic data = JObject.Parse(json); string Status = string.Empty;
            Model.response.WebResponse response = null;
            var jsonData = new { };
            Services obj = new Services();

            var validateJsonData = (dynamic)null;
            // var cm = li.SingleOrDefault();
            DataTable dt = new DataTable();

            int number;
            if (!int.TryParse(Convert.ToString(data.Client_ID), out number))
            {
                validateJsonData = new { response = false, responseCode = "02", data = "Invalid Client_ID field." };
                return new JsonResult(validateJsonData);
            }
            if (!int.TryParse(Convert.ToString(data.Branch_ID), out number))
            {
                validateJsonData = new { response = false, responseCode = "02", data = "Invalid Branch_ID field." };
                return new JsonResult(validateJsonData);
            }
            if (!int.TryParse(Convert.ToString(data.User_ID), out number))
            {
                validateJsonData = new { response = false, responseCode = "02", data = "Invalid User_ID field." };
                return new JsonResult(validateJsonData);
            }

            try
            {
                obj = JsonConvert.DeserializeObject<Services>(json);

                Service.srvServices srvServices = new Service.srvServices(context);
                dt = srvServices.Selected_Football_Team(obj, context);
                if (dt != null && dt.Rows.Count > 0)
                {
                    //string JSONresult = JsonConvert.SerializeObject(dt);
                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_SUCCESS);
                    response.ResponseCode = 0;
                    response.ObjData = dt;
                    var dtData = dt.Rows.OfType<DataRow>()
                    .Select(row => dt.Columns.OfType<DataColumn>()
                    .ToDictionary(col => col.ColumnName, c => row[c].ToString()));
                    foreach (DataColumn column in dt.Columns)
                    {
                        // Replace "OldColumnName" with the desired new name
                        column.ColumnName = column.ColumnName.Replace("S_ID", "League_ID");
                        column.ColumnName = column.ColumnName.Replace("Services_Name", "League_Name");
                    }
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
                string Activity = "Api Selected_football_team: " + ex.ToString() + " ";
                validateJsonData = new { response = false, responseCode = "01", data = "Invalid Request" };
                CompanyInfo.InsertErrorLogTracker(Activity.ToString(), 0, 0, 0, 0, "Selected_football_team", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "", context);
                return new JsonResult(validateJsonData);
            }

            return new JsonResult(jsonData);
        }
    }
}
