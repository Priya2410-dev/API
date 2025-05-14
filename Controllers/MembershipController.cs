using Calyx_Solutions.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data;
using System.Net;
using System.Security.Claims;
using System.Text.Json;
using System.Web.Http.Results;
using static Google.Apis.Requests.BatchRequest;

namespace Calyx_Solutions.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MembershipController : ControllerBase  //for Kmoney
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;

        public MembershipController(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            this._configuration = configuration;
            _httpContextAccessor = httpContextAccessor;

        }

        [HttpPost]
        [Authorize]
        [Route("getmembershippoints")]
        public IActionResult Get_memberships_points([FromBody] JsonElement obj1)
        {
            HttpContext context = HttpContext;
            string json = System.Text.Json.JsonSerializer.Serialize(obj1);
            try
            {
                CompanyInfo.InsertrequestLogTracker("Get_memberships_points full request body: " + JObject.Parse(json), 0, 0, 0, 0, "getmembershippoints", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
            }
            catch (Exception ex) { }
            dynamic data = JObject.Parse(json); string Status = string.Empty;
            var claimsIdentity = User.Identity as ClaimsIdentity;//sanket
            var authHeader = "";
            try { authHeader = HttpContext.Request.Headers["Authorization"].ToString(); } catch (Exception egx) { authHeader = ""; }

            bool auth = AuthController.checkAuth(claimsIdentity, obj1, Convert.ToString(authHeader));
            if (!auth)
            {
                var errorResponse = new { response = false, responseCode = "403", data = "Access Forbidden" };
                return new JsonResult(errorResponse) { StatusCode = (int)HttpStatusCode.Forbidden };
            }
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
                //obj.Client_ID = data.clientID;
                //obj.Branch_ID = data.branchID;
                //obj.User_ID = data.usedID;
                //obj.Customer_ID = data.customerID;
                obj = JsonConvert.DeserializeObject<Customer>(json);

                Service.srvMembership srvMembership = new Service.srvMembership(context);
                dt = srvMembership.Membership_point_min_max(obj, context);
                if (dt != null)
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
                string Activity = "Api Get_memberships_points: " + ex.ToString() + " ";
                validateJsonData = new { response = false, responseCode = "01", data = "Invalid Request" };
                CompanyInfo.InsertErrorLogTracker(Activity.ToString(), 0, 0, 0, 0, "Get_memberships_points", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "", context);
                return new JsonResult(validateJsonData);
            }

            return new JsonResult(jsonData);
        }

        [HttpPost]
        [Authorize]
        [Route("getmembershipbenefitsdata")]
        public IActionResult get_membership_benefits_data([FromBody] JsonElement obj1)
        {
            HttpContext context = HttpContext;
            string json = System.Text.Json.JsonSerializer.Serialize(obj1);
            CompanyInfo.InsertrequestLogTracker("get_membership_benefits_data full request body: " + JObject.Parse(json), 0, 0, 0, 0, "getmembershipbenefitsdata", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
            dynamic data = JObject.Parse(json); string Status = string.Empty;
            var claimsIdentity = User.Identity as ClaimsIdentity;//sanket
            var authHeader = "";
            try { authHeader = HttpContext.Request.Headers["Authorization"].ToString(); } catch (Exception egx) { authHeader = ""; }

            bool auth = AuthController.checkAuth(claimsIdentity, obj1, Convert.ToString(authHeader));
            if (!auth)
            {
                var errorResponse = new { response = false, responseCode = "403", data = "Access Forbidden" };
                return new JsonResult(errorResponse) { StatusCode = (int)HttpStatusCode.Forbidden };
            }
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
                //obj.Client_ID = data.clientID;
                //obj.Branch_ID = data.branchID;
                //obj.User_ID = data.usedID;
                //obj.Customer_ID = data.customerID;
                obj = JsonConvert.DeserializeObject<Customer>(json);

                Service.srvMembership srvMembership = new Service.srvMembership(context);
                dt = srvMembership.get_membership_benefits_data(obj, context);
                if (dt != null)
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
                string Activity = "Api get_membership_benefits_data: " + ex.ToString() + " ";
                validateJsonData = new { response = false, responseCode = "01", data = "Invalid Request" };
                CompanyInfo.InsertErrorLogTracker(Activity.ToString(), 0, 0, 0, 0, "get_membership_benefits_data", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "", context);
                return new JsonResult(validateJsonData);
            }

            return new JsonResult(jsonData);
        }

        [HttpPost]
        [Authorize]
        [Route("getcurrentmembershipbenefitsdata")]
        public IActionResult get_current_membership_benefits_data([FromBody] JsonElement obj1)
        {
            HttpContext context = HttpContext;
            string json = System.Text.Json.JsonSerializer.Serialize(obj1);
            CompanyInfo.InsertrequestLogTracker("get_current_membership_benefits_data full request body: " + JObject.Parse(json), 0, 0, 0, 0, "getcurrentmembershipbenefitsdata", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
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
                //obj.Client_ID = data.clientID;
                //obj.Branch_ID = data.branchID;
                //obj.User_ID = data.usedID;
                //obj.Customer_ID = data.customerID;
                obj = JsonConvert.DeserializeObject<Customer>(json);

                Service.srvMembership srvMembership = new Service.srvMembership(context);
                dt = srvMembership.get_current_membership_benefits_data(obj, context);
                if (dt != null)
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
                string Activity = "Api Get_memberships_points: " + ex.ToString() + " ";
                validateJsonData = new { response = false, responseCode = "01", data = "Invalid Request" };
                CompanyInfo.InsertErrorLogTracker(Activity.ToString(), 0, 0, 0, 0, "Get_memberships_points", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "", context);
                return new JsonResult(validateJsonData);
            }

            return new JsonResult(jsonData);
        }

        [HttpPost]
        [Authorize]
        [Route("redeembenefits")]
        public IActionResult redeem_benefits([FromBody] JsonElement obj1)
        {
            HttpContext context = HttpContext;
            string json = System.Text.Json.JsonSerializer.Serialize(obj1);
            CompanyInfo.InsertrequestLogTracker("get_current_membership_benefits_data full request body: " + JObject.Parse(json), 0, 0, 0, 0, "getcurrentmembershipbenefitsdata", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
            dynamic data = JObject.Parse(json); string Status = string.Empty;
            var claimsIdentity = User.Identity as ClaimsIdentity;//sanket
            var authHeader = "";
            try { authHeader = HttpContext.Request.Headers["Authorization"].ToString(); } catch (Exception egx) { authHeader = ""; }

            bool auth = AuthController.checkAuth(claimsIdentity, obj1, Convert.ToString(authHeader));
            if (!auth)
            {
                var errorResponse = new { response = false, responseCode = "403", data = "Access Forbidden" };
                return new JsonResult(errorResponse) { StatusCode = (int)HttpStatusCode.Forbidden };
            }
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

                Service.srvMembership srvMembership = new Service.srvMembership(context);
                Customer _obj = srvMembership.redeem_benefits(obj, context);

                List<Dictionary<string, object>> customerData = new List<Dictionary<string, object>>();
                Dictionary<string, object> customer = new Dictionary<string, object>();
                if (_obj != null)
                {
                    if (_obj.Message == "success")
                    {
                        response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_SUCCESS);
                        response.ResponseCode = 0;
                        response.ObjData = dt;
                        customer["status"] = "success";
                        customer["message"] = obj.Benefit_Name + " Benefit Redeemed successfully."+ _obj.Comment;
                        customerData.Add(customer);

                        validateJsonData = new { response = true, responseCode = "00", data = customerData };
                        return new JsonResult(validateJsonData);
                    }
                    else if (_obj.Message == "unavailable")
                    {
                        customer["status"] = "failed";
                        customer["message"] = "This benefit is not available for you at the moment";
                    }
                    else if (_obj.Message == "wallet_inactive")
                    {
                        customer["status"] = "failed";
                        customer["message"] = "Your Points Wallet is inactive. Please make a transfer to activate your wallet and redeem the benefits.";
                    }
                    else if (_obj.Message == "no_enough_points")
                    {
                        customer["status"] = "failed";
                        customer["message"] = "No Enough Points in Wallet";
                    }
                    else if (_obj.Message == "already_redeemed")
                    {
                        customer["status"] = "failed";
                        customer["message"] = "The benefit is already redeemed";
                    }
                    else if (_obj.Message == "invalid_date")
                    {
                        customer["status"] = "failed";
                        customer["message"] = "You can redeem this benefit before 14 to 30 days of your birthday only";
                    }
                    else if (_obj.Message == "error")
                    {
                        customer["status"] = "failed";
                        customer["message"] = "Something went wrong";
                    }
                    //else if (_obj.Message == "Validation_Failed")
                    //{
                    //    customer["status"] = "failed";
                    //    customer["message"] = "Failed to redeem the benefit";
                    //}
                    else if (_obj.Message == "month_limit_exceed")
                    {
                        customer["status"] = "failed";
                        customer["message"] = "You can redeem benefit once a month only";
                    }
                    else
                    {
                        customer["status"] = "failed";
                        customer["message"] = "Failed to redeem the benefit";
                    }
                    customerData.Add(customer);

                    validateJsonData = new { response = false, responseCode = "02", data = customerData };
                    return new JsonResult(validateJsonData);
                }
                else
                {
                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_TECHNICAL_ERROR);
                    response.ResponseMessage = "Failed";
                    response.ResponseCode = 0;

                    customer["status"] = "failed";
                    customer["message"] = "Failed to redeem the benefit";
                    customerData.Add(customer);
                    validateJsonData = new { response = false, responseCode = "02", data = response.ResponseMessage };
                    return new JsonResult(validateJsonData);
                }
            }
            catch (Exception ex)
            {
                string Activity = "Api Get_memberships_points: " + ex.ToString() + " ";
                validateJsonData = new { response = false, responseCode = "01", data = "Invalid Request" };
                CompanyInfo.InsertErrorLogTracker(Activity.ToString(), 0, 0, 0, 0, "Get_memberships_points", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "", context);
                return new JsonResult(validateJsonData);
            }

            return new JsonResult(jsonData);
        }

        [HttpPost]
        [Authorize]
        [Route("redeemhealthcare")]
        public IActionResult redeem_health_care([FromBody] JsonElement obj1)
        {
            HttpContext context = HttpContext;
            string json = System.Text.Json.JsonSerializer.Serialize(obj1);
            CompanyInfo.InsertrequestLogTracker("get_current_membership_benefits_data full request body: " + JObject.Parse(json), 0, 0, 0, 0, "getcurrentmembershipbenefitsdata", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
            dynamic data = JObject.Parse(json); string Status = string.Empty;
            var claimsIdentity = User.Identity as ClaimsIdentity;//sanket
            var authHeader = "";
            try { authHeader = HttpContext.Request.Headers["Authorization"].ToString(); } catch (Exception egx) { authHeader = ""; }

            bool auth = AuthController.checkAuth(claimsIdentity, obj1, Convert.ToString(authHeader));
            if (!auth)
            {
                var errorResponse = new { response = false, responseCode = "403", data = "Access Forbidden" };
                return new JsonResult(errorResponse) { StatusCode = (int)HttpStatusCode.Forbidden };
            }
            Model.response.WebResponse response = null;
            var jsonData = new { };
            Customer obj = new Customer();

            var validateJsonData = (dynamic)null;
            // var cm = li.SingleOrDefault();
            DataTable dt = new DataTable();

            int number;
            if (!int.TryParse(Convert.ToString(data.Client_ID), out number))
            {
                validateJsonData = new { response = false, responseCode = "02", data = "Invalid clientID." };
                return new JsonResult(validateJsonData);
            }
            if (!int.TryParse(Convert.ToString(data.Branch_ID), out number))
            {
                validateJsonData = new { response = false, responseCode = "02", data = "Invalid branchID." };
                return new JsonResult(validateJsonData);
            }
            if (!int.TryParse(Convert.ToString(data.User_ID), out number))
            {
                validateJsonData = new { response = false, responseCode = "02", data = "Invalid usedID." };
                return new JsonResult(validateJsonData);
            }
            if (!int.TryParse(Convert.ToString(data.Customer_ID), out number))
            {
                validateJsonData = new { response = false, responseCode = "02", data = "Invalid Customer_ID." };
                return new JsonResult(validateJsonData);
            }
            if (!int.TryParse(Convert.ToString(data.Benefit_ID), out number))
            {
                validateJsonData = new { response = false, responseCode = "02", data = "Invalid Benefit_ID." };
                return new JsonResult(validateJsonData);
            }
            if (!int.TryParse(Convert.ToString(data.Benefit_Name), out number))
            {
                validateJsonData = new { response = false, responseCode = "02", data = "Invalid Benefit_Name." };
                return new JsonResult(validateJsonData);
            }
            if (!int.TryParse(Convert.ToString(data.Email), out number))
            {
                validateJsonData = new { response = false, responseCode = "02", data = "Invalid Email." };
                return new JsonResult(validateJsonData);
            }
            if (!int.TryParse(Convert.ToString(data.Email), out number))
            {
                validateJsonData = new { response = false, responseCode = "02", data = "Invalid Name." };
                return new JsonResult(validateJsonData);
            }

            try
            {
                obj = JsonConvert.DeserializeObject<Customer>(json);

                Service.srvMembership srvMembership = new Service.srvMembership(context);
                string status = srvMembership.redeem_health_care(obj, context);

                List<Dictionary<string, object>> customerData = new List<Dictionary<string, object>>();
                Dictionary<string, object> customer = new Dictionary<string, object>();
                if (status == "success")
                {
                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_SUCCESS);
                    response.ResponseCode = 0;
                    response.ResponseMessage = "success";
                    //customer["customerID"] = _obj.Customer_ID;
                    //customer["customerUsername"] = _obj.UserName;
                    customer["status"] = "success";
                    customer["message"] = "KMoney Health Care Redeemed successfully. You can add beneficiaries to Health Care from Beneficiary page.";
                    customerData.Add(customer);

                    validateJsonData = new { response = true, responseCode = "00", data = customerData };
                    return new JsonResult(validateJsonData);
                }
                else if (status == "wallet_inactive")
                {
                    customer["ststus"] = "failed";
                    customer["message"] = "Your Points Wallet is inactive. Please make a transfer to activate your wallet and redeem the benefits.";
                    customerData.Add(customer);

                    validateJsonData = new { response = false, responseCode = "02", data = customerData };
                    return new JsonResult(validateJsonData);
                }
                else if (status == "error")
                {
                    customer["ststus"] = "failed";
                    customer["message"] = "Something went wrong";
                    customerData.Add(customer);

                    validateJsonData = new { response = false, responseCode = "02", data = customerData };
                    return new JsonResult(validateJsonData);
                }
                else
                {
                    customer["ststus"] = "failed";
                    customer["message"] = "Failed to Redeem.";
                    customerData.Add(customer);

                    validateJsonData = new { response = false, responseCode = "02", data = customerData };
                    return new JsonResult(validateJsonData);
                }

            }
            catch (Exception ex)
            {
                string Activity = "Api Get_memberships_points: " + ex.ToString() + " ";
                validateJsonData = new { response = false, responseCode = "01", data = "Invalid Request" };
                CompanyInfo.InsertErrorLogTracker(Activity.ToString(), 0, 0, 0, 0, "Get_memberships_points", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "", context);
                return new JsonResult(validateJsonData);
            }

            return new JsonResult(jsonData);
        }

        [HttpPost]
        [Authorize]
        [Route("remainingmedicarecount")]
        public IActionResult RemainingMedicareCount([FromBody] JsonElement obj1)
        {
            HttpContext context = HttpContext;
            string json = System.Text.Json.JsonSerializer.Serialize(obj1);
            CompanyInfo.InsertrequestLogTracker("RemainingMedicareCount full request body: " + JObject.Parse(json), 0, 0, 0, 0, "remainingmedicarecount", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
            dynamic data = JObject.Parse(json); string Status = string.Empty;
            var claimsIdentity = User.Identity as ClaimsIdentity;//sanket
            var authHeader = "";
            try { authHeader = HttpContext.Request.Headers["Authorization"].ToString(); } catch (Exception egx) { authHeader = ""; }

            bool auth = AuthController.checkAuth(claimsIdentity, obj1, Convert.ToString(authHeader));
            if (!auth)
            {
                var errorResponse = new { response = false, responseCode = "403", data = "Access Forbidden" };
                return new JsonResult(errorResponse) { StatusCode = (int)HttpStatusCode.Forbidden };
            }
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

                Service.srvMembership srvMembership = new Service.srvMembership(context);
                dt = srvMembership.RemainingMedicareCount(obj, context);
                if (dt != null)
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
                string Activity = "Api Get_memberships_points: " + ex.ToString() + " ";
                validateJsonData = new { response = false, responseCode = "01", data = "Invalid Request" };
                CompanyInfo.InsertErrorLogTracker(Activity.ToString(), 0, 0, 0, 0, "Get_memberships_points", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "", context);
                return new JsonResult(validateJsonData);
            }

            return new JsonResult(jsonData);
        }

        [HttpPost]
        [Authorize]
        [Route("getcakeflavours")]
        public IActionResult GetCakeFlavours([FromBody] JsonElement obj1)
        {
            HttpContext context = HttpContext;
            string json = System.Text.Json.JsonSerializer.Serialize(obj1);
            CompanyInfo.InsertrequestLogTracker("GetCakeFlavours full request body: " + JObject.Parse(json), 0, 0, 0, 0, "getcakeflavours", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
            dynamic data = JObject.Parse(json); string Status = string.Empty;
            var claimsIdentity = User.Identity as ClaimsIdentity;//sanket
            var authHeader = "";
            try { authHeader = HttpContext.Request.Headers["Authorization"].ToString(); } catch (Exception egx) { authHeader = ""; }

            bool auth = AuthController.checkAuth(claimsIdentity, obj1, Convert.ToString(authHeader));
            if (!auth)
            {
                var errorResponse = new { response = false, responseCode = "403", data = "Access Forbidden" };
                return new JsonResult(errorResponse) { StatusCode = (int)HttpStatusCode.Forbidden };
            }
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

                Service.srvMembership srvMembership = new Service.srvMembership(context);
                dt = srvMembership.GetCakeFlavours(obj, context);
                if (dt != null)
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
                string Activity = "Api GetCakeFlavours: " + ex.ToString() + " ";
                validateJsonData = new { response = false, responseCode = "01", data = "Invalid Request" };
                CompanyInfo.InsertErrorLogTracker(Activity.ToString(), 0, 0, 0, 0, "GetCakeFlavours", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "", context);
                return new JsonResult(validateJsonData);
            }

            return new JsonResult(jsonData);
        }

        [HttpPost]
        [Authorize]
        [Route("getcakediet")]
        public IActionResult GetCakeDiet([FromBody] JsonElement obj1)
        {
            HttpContext context = HttpContext;
            string json = System.Text.Json.JsonSerializer.Serialize(obj1);
            CompanyInfo.InsertrequestLogTracker("GetCakeDiet full request body: " + JObject.Parse(json), 0, 0, 0, 0, "getcakediet", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
            dynamic data = JObject.Parse(json); string Status = string.Empty;
            var claimsIdentity = User.Identity as ClaimsIdentity;//sanket
            var authHeader = "";
            try { authHeader = HttpContext.Request.Headers["Authorization"].ToString(); } catch (Exception egx) { authHeader = ""; }

            bool auth = AuthController.checkAuth(claimsIdentity, obj1, Convert.ToString(authHeader));
            if (!auth)
            {
                var errorResponse = new { response = false, responseCode = "403", data = "Access Forbidden" };
                return new JsonResult(errorResponse) { StatusCode = (int)HttpStatusCode.Forbidden };
            }
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

                Service.srvMembership srvMembership = new Service.srvMembership(context);
                dt = srvMembership.GetCakeDiet(obj, context);
                if (dt != null)
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
                string Activity = "Api GetCakeDiet: " + ex.ToString() + " ";
                validateJsonData = new { response = false, responseCode = "01", data = "Invalid Request" };
                CompanyInfo.InsertErrorLogTracker(Activity.ToString(), 0, 0, 0, 0, "GetCakeDiet", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "", context);
                return new JsonResult(validateJsonData);
            }

            return new JsonResult(jsonData);
        }

        [HttpPost]
        [Authorize]
        [Route("getsuperstores")]
        public IActionResult GetSuperStores([FromBody] JsonElement obj1)
        {
            HttpContext context = HttpContext;
            string json = System.Text.Json.JsonSerializer.Serialize(obj1);
            CompanyInfo.InsertrequestLogTracker("GetSuperStores full request body: " + JObject.Parse(json), 0, 0, 0, 0, "getsuperstores", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
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

                Service.srvMembership srvMembership = new Service.srvMembership(context);
                dt = srvMembership.GetSuperStores(obj, context);
                if (dt != null)
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
                string Activity = "Api GetSuperStores: " + ex.ToString() + " ";
                validateJsonData = new { response = false, responseCode = "01", data = "Invalid Request" };
                CompanyInfo.InsertErrorLogTracker(Activity.ToString(), 0, 0, 0, 0, "GetSuperStores", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "", context);
                return new JsonResult(validateJsonData);
            }

            return new JsonResult(jsonData);
        }

        [HttpPost]
        [Authorize]
        [Route("getentertainmentvouchers")]
        public IActionResult GetEntertainmentVouchers([FromBody] JsonElement obj1)
        {
            HttpContext context = HttpContext;
            string json = System.Text.Json.JsonSerializer.Serialize(obj1);
            CompanyInfo.InsertrequestLogTracker("GetEntertainmentVouchers full request body: " + JObject.Parse(json), 0, 0, 0, 0, "getentertainmentvouchers", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
            dynamic data = JObject.Parse(json); string Status = string.Empty;
            var claimsIdentity = User.Identity as ClaimsIdentity;//sanket
            var authHeader = "";
            try { authHeader = HttpContext.Request.Headers["Authorization"].ToString(); } catch (Exception egx) { authHeader = ""; }

            bool auth = AuthController.checkAuth(claimsIdentity, obj1, Convert.ToString(authHeader));
            if (!auth)
            {
                var errorResponse = new { response = false, responseCode = "403", data = "Access Forbidden" };
                return new JsonResult(errorResponse) { StatusCode = (int)HttpStatusCode.Forbidden };
            }
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

                Service.srvMembership srvMembership = new Service.srvMembership(context);
                dt = srvMembership.GetEntertainmentVouchers(obj, context);
                if (dt != null)
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
                string Activity = "Api GetEntertainmentVouchers: " + ex.ToString() + " ";
                validateJsonData = new { response = false, responseCode = "01", data = "Invalid Request" };
                CompanyInfo.InsertErrorLogTracker(Activity.ToString(), 0, 0, 0, 0, "GetEntertainmentVouchers", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "", context);
                return new JsonResult(validateJsonData);
            }

            return new JsonResult(jsonData);
        }


        [HttpPost]
        [Authorize]
        [Route("customerdetailsforhealthcare")]
        public IActionResult CustomerDetailsForHealthcare([FromBody] JsonElement obj1)
        {
            HttpContext context = HttpContext;
            string json = System.Text.Json.JsonSerializer.Serialize(obj1);
            CompanyInfo.InsertrequestLogTracker("CustomerDetailsForHealthcare full request body: " + JObject.Parse(json), 0, 0, 0, 0, "customerdetailsforhealthcare", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
            dynamic data = JObject.Parse(json); string Status = string.Empty;
            var claimsIdentity = User.Identity as ClaimsIdentity;//sanket
            var authHeader = "";
            try { authHeader = HttpContext.Request.Headers["Authorization"].ToString(); } catch (Exception egx) { authHeader = ""; }

            bool auth = AuthController.checkAuth(claimsIdentity, obj1, Convert.ToString(authHeader));
            if (!auth)
            {
                var errorResponse = new { response = false, responseCode = "403", data = "Access Forbidden" };
                return new JsonResult(errorResponse) { StatusCode = (int)HttpStatusCode.Forbidden };
            }
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

                Service.srvMembership srvMembership = new Service.srvMembership(context);
                dt = srvMembership.CustomerDetailsForHealthcare(obj, context);
                if (dt != null)
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
                string Activity = "Api CustomerDetailsForHealthcare: " + ex.ToString() + " ";
                validateJsonData = new { response = false, responseCode = "01", data = "Invalid Request" };
                CompanyInfo.InsertErrorLogTracker(Activity.ToString(), 0, 0, 0, 0, "CustomerDetailsForHealthcare", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "", context);
                return new JsonResult(validateJsonData);
            }

            return new JsonResult(jsonData);
        }

        [HttpPost]
        [Authorize]
        [Route("getpointswallethistory")]
        public IActionResult Get_Points_Wallet_History([FromBody] JsonElement obj1)
        {
            HttpContext context = HttpContext;
            string json = System.Text.Json.JsonSerializer.Serialize(obj1);
            CompanyInfo.InsertrequestLogTracker("Get_Points_Wallet_History full request body: " + JObject.Parse(json), 0, 0, 0, 0, "getpointswallethistory", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
            dynamic data = JObject.Parse(json); string Status = string.Empty;
            var claimsIdentity = User.Identity as ClaimsIdentity;//sanket
            var authHeader = "";
            try { authHeader = HttpContext.Request.Headers["Authorization"].ToString(); } catch (Exception egx) { authHeader = ""; }

            bool auth = AuthController.checkAuth(claimsIdentity, obj1, Convert.ToString(authHeader));
            if (!auth)
            {
                var errorResponse = new { response = false, responseCode = "403", data = "Access Forbidden" };
                return new JsonResult(errorResponse) { StatusCode = (int)HttpStatusCode.Forbidden };
            }
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

                Service.srvMembership srvMembership = new Service.srvMembership(context);
                dt = srvMembership.Get_Points_Wallet_History(obj, context);
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
                string Activity = "Api Get_Points_Wallet_History: " + ex.ToString() + " ";
                validateJsonData = new { response = false, responseCode = "01", data = "Invalid Request" };
                CompanyInfo.InsertErrorLogTracker(Activity.ToString(), 0, 0, 0, 0, "Get_Points_Wallet_History", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "", context);
                return new JsonResult(validateJsonData);
            }

            return new JsonResult(jsonData);
        }
        
        [HttpPost]
        [Authorize]
        [Route("getbenefitsredemptionhistory")]
        public IActionResult GetBenefitsRedemptionHistory([FromBody] JsonElement obj1)
        {
            HttpContext context = HttpContext;
            string json = System.Text.Json.JsonSerializer.Serialize(obj1);
            CompanyInfo.InsertrequestLogTracker("GetBenefitsRedemptionHistory full request body: " + JObject.Parse(json), 0, 0, 0, 0, "getbenefitsredemptionhistory", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
            dynamic data = JObject.Parse(json); string Status = string.Empty;
            var claimsIdentity = User.Identity as ClaimsIdentity;//sanket
            var authHeader = "";
            try { authHeader = HttpContext.Request.Headers["Authorization"].ToString(); } catch (Exception egx) { authHeader = ""; }

            bool auth = AuthController.checkAuth(claimsIdentity, obj1, Convert.ToString(authHeader));
            if (!auth)
            {
                var errorResponse = new { response = false, responseCode = "403", data = "Access Forbidden" };
                return new JsonResult(errorResponse) { StatusCode = (int)HttpStatusCode.Forbidden };
            }
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

                Service.srvMembership srvMembership = new Service.srvMembership(context);
                dt = srvMembership.GetBenefitsRedemptionHistory(obj, context);
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
                string Activity = "Api GetBenefitsRedemptionHistory: " + ex.ToString() + " ";
                validateJsonData = new { response = false, responseCode = "01", data = "Invalid Request" };
                CompanyInfo.InsertErrorLogTracker(Activity.ToString(), 0, 0, 0, 0, "GetBenefitsRedemptionHistory", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "", context);
                return new JsonResult(validateJsonData);
            }

            return new JsonResult(jsonData);
        }


    }
}
