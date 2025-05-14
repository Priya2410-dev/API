using Calyx_Solutions.Model;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Data;
using System.Globalization;
using System.Web.Helpers;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Authorization;
using System.Text.RegularExpressions;
using System.Net;
using System.Security.Claims;
using System.Text.Json;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Calyx_Solutions.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {

        [HttpPost]
        [Route("viewpushnotifications")]
        public IActionResult View_push_notifications([FromBody] JsonObject obj)
        {
            Model.response.WebResponse response = null;
            string json = System.Text.Json.JsonSerializer.Serialize(obj);
            dynamic data = JObject.Parse(json);
            var validateJsonData = (dynamic)null;
            CompanyInfo.InsertrequestLogTracker("viewpushnotifications full request body: " + JObject.Parse(json), 0, 0, 0, 0, "View_push_notifications", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
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
            Customer Obj = JsonConvert.DeserializeObject<Customer>(json);
            try
            {
                if (!SqlInjectionProtector.ReadJsonObjectValues(obj) || !SqlInjectionProtector.ReadJsonObjectValuesScript(obj))
                {
                    var errorResponse = new { response = false, responseCode = "02", data = "Invalid input detected." };
                    return new JsonResult(errorResponse) { StatusCode = 400 };
                }

                int number;
                if (!int.TryParse(Convert.ToString(data.clientID), out number))
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "Invalid Client ID." };
                    return new JsonResult(validateJsonData);
                }
                if (!int.TryParse(Convert.ToString(data.branchID), out number))
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "Invalid branchID field." };
                    return new JsonResult(validateJsonData);
                }
                if (data.customerID == "" || data.customerID == null)
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "Invalid customer ID ." };
                    return new JsonResult(validateJsonData);
                }
                Obj.Client_ID = data.clientID;
                Obj.Branch_ID = data.branchID;
                Obj.Id = data.customerID;
                Obj.RemindMeFlag = data.flag;
               
                // Updated code for notification start
                Model.Dashboard d = new Model.Dashboard();
                d.Client_ID = data.clientID;
                d.Branch_ID = data.branchID;
                d.flag = data.flag;
                d.date = data.date;
                d.Customer_ID = data.customerID;
                Service.srvDashboard srv = new Service.srvDashboard();
                DataTable li1s = srv.View_push_notifications(d);
                if (li1s != null && li1s.Rows.Count > 0)
                {
                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_SUCCESS);
                    response.data = "success";
                    response.ObjData = li1s;
                    response.ResponseCode = 0;

                    List<Dictionary<string, object>> notifications = new List<Dictionary<string, object>>();

                    foreach (DataRow dr in li1s.Rows)
                    {
                        Dictionary<string, object> notification = new Dictionary<string, object>();
                        notification["notificationIcon"] = Convert.ToString(dr["Notification_Icon"]);
                        notification["notification"] = dr["notification_title"] +"<br>"+ dr["notification_msg"];
                        notification["ID"] = dr["Notification_Id"];
                        notification["time"] = dr["Time"];
                        notification["recordInsertedTime"] = dr["start_datetime"];
                        notification["Read_Count_Customer"] = dr["Read_Count_Customer"];
                        
                        notifications.Add(notification);
                    }
                    validateJsonData = new { response = true, responseCode = "00", data = notifications };
                    return new JsonResult(validateJsonData);

                    /*var returndata = li1s.Rows.OfType<DataRow>()
                 .Select(row => li1s.Columns.OfType<DataColumn>()
                     .ToDictionary(col => col.ColumnName, c => row[c]));
                    validateJsonData = new { response = true, responseCode = "00", data = returndata };
                    return new JsonResult(validateJsonData);*/

                }
                else
                {
                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_TECHNICAL_ERROR);
                    response.ResponseMessage = "No Records Found.";
                    response.ResponseCode = 0;
                    validateJsonData = new { response = false, responseCode = "02", data = response.ResponseMessage };
                    return new JsonResult(validateJsonData);
                }
                // Updated code for notification end

            }
            catch (Exception ex)
            {

                validateJsonData = new { response = false, responseCode = "01", data = "Invalid Request" };
                CompanyInfo.InsertErrorLogTracker(ex.ToString(), 0, 0, 0, 0, "viewpushnotifications", Convert.ToInt32(Obj.Branch_ID), Convert.ToInt32(Obj.Client_ID), "", HttpContext);
                return new JsonResult(validateJsonData);

            }
        }

        
        [HttpPost]        
        [Route("notificationupdatereadcount")]
        public IActionResult Notification_update_readcount([FromBody] JsonObject obj)
        {
            Model.response.WebResponse response = null;
            string json = System.Text.Json.JsonSerializer.Serialize(obj);
            dynamic data = JObject.Parse(json);
            var validateJsonData = (dynamic)null;
            CompanyInfo.InsertrequestLogTracker("notificationupdatereadcount full request body: " + JObject.Parse(json), 0, 0, 0, 0, "Notification_update_readcount", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
            Customer Obj = JsonConvert.DeserializeObject<Customer>(json);
            try
            {
                if (!SqlInjectionProtector.ReadJsonObjectValues(obj) || !SqlInjectionProtector.ReadJsonObjectValuesScript(obj))
                {
                    var errorResponse = new { response = false, responseCode = "02", data = "Invalid input detected." };
                    return new JsonResult(errorResponse) { StatusCode = 400 };
                }

                Service.srvDashboard srv = new Service.srvDashboard();

                List<Model.Dashboard> li1 = new List<Model.Dashboard>();
                Model.Dashboard d = new Model.Dashboard();
                d.Client_ID = data.clientID;
                d.Branch_ID = data.branchID;
                d.flag = data.notificationId;
                DataTable li1s = srv.Update_dashboard_readcount(d);
                Model.Dashboard _Obj = new Model.Dashboard();
                if (li1s != null && li1s.Rows.Count > 0)
                {
                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_SUCCESS);
                    response.data = "success";
                    response.ObjData = li1s;
                    response.ResponseCode = 0;

                    List<Dictionary<string, object>> notifications = new List<Dictionary<string, object>>();

                    var returndata = li1s.Rows.OfType<DataRow>()
                .Select(row => li1s.Columns.OfType<DataColumn>()
                    .ToDictionary(col => col.ColumnName, c => row[c]));
                    validateJsonData = new { response = true, responseCode = "00", data = returndata };
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

                validateJsonData = new { response = false, responseCode = "01", data = "Invalid Request" };
                CompanyInfo.InsertErrorLogTracker(ex.ToString(), 0, 0, 0, 0, "notificationupdatereadcount", Convert.ToInt32(Obj.Branch_ID), Convert.ToInt32(Obj.Client_ID), "", HttpContext);
                return new JsonResult(validateJsonData);
            }
        }

        private string RemoveExtraInfo(string message)
        {
            if (string.IsNullOrEmpty(message))
                return message;

            return message.Split("countryid")[0].Trim(); // Take only the first part before the countryid
        }

        [HttpPost]
        [Authorize]
        [Route("notification")]
        public IActionResult Viewnotifications([FromBody] JsonObject obj)
        {
            Model.response.WebResponse response = null;
            string json = System.Text.Json.JsonSerializer.Serialize(obj);
            dynamic data = JObject.Parse(json);
            var validateJsonData = (dynamic)null;
            CompanyInfo.InsertrequestLogTracker("notification full request body: " + JObject.Parse(json), 0, 0, 0, 0, "Viewnotifications", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
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
            Customer Obj = JsonConvert.DeserializeObject<Customer>(json);
            try
            {
                if (!SqlInjectionProtector.ReadJsonObjectValues(obj) || !SqlInjectionProtector.ReadJsonObjectValuesScript(obj))
                {
                    var errorResponse = new { response = false, responseCode = "02", data = "Invalid input detected." };
                    return new JsonResult(errorResponse) { StatusCode = 400 };
                }

                int number;
                if (!int.TryParse(Convert.ToString(data.clientID), out number)  )
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "Invalid Client ID." };
                    return new JsonResult(validateJsonData);
                }
                if (!int.TryParse(Convert.ToString(data.branchID), out number))
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "Invalid branchID field." };
                    return new JsonResult(validateJsonData);
                }
                if (data.customerID == "" || data.customerID == null)
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "Invalid customer ID ." };
                    return new JsonResult(validateJsonData);
                }
                Obj.Client_ID = data.clientID;
                Obj.Branch_ID = data.branchID;
                Obj.Id = data.customerID;
                Obj.RemindMeFlag = data.flag;

                /*if (!SqlInjectionProtector.ValidateObjectForSqlInjection(obj) || !SqlInjectionProtector.ValidateObjectForScriptSqlInjection(obj))
                {
                    return new JsonResult(validateJsonData) { StatusCode = StatusCodes.Status400BadRequest, Value = "Invalid input detected." };
                }*/



                /*object o = JsonConvert.DeserializeObject(json);
                bool checkinjection = CompanyInfo.checksqlinjectiondata(o);
                if (!checkinjection)
                {
                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_TECHNICAL_ERROR);
                    response.ResponseMessage = "Invalid Field Values.";
                    response.ResponseCode = 6;
                    validateJsonData = new { response = false, responseCode = "02", data = response.ResponseMessage };
                    return new JsonResult(validateJsonData);
                }*/

                // Updated code for notification start
                Model.Dashboard d = new Model.Dashboard();
                d.Client_ID = data.clientID;
                d.Branch_ID = data.branchID;
                d.flag = data.flag;
                d.date = data.date;
                d.Customer_ID = data.customerID;
                Service.srvDashboard srv = new Service.srvDashboard();
                DataTable li1s = srv.View_push_notifications(d);
                if (li1s != null && li1s.Rows.Count > 0)
                {
                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_SUCCESS);
                    response.data = "success";
                    response.ObjData = li1s;
                    response.ResponseCode = 0;

                    List<Dictionary<string, object>> notifications = new List<Dictionary<string, object>>();
                    int unreadCount = 0; // <<== Parth added to track unread count
                    foreach (DataRow dr in li1s.Rows)
                    {
                        Dictionary<string, object> notification = new Dictionary<string, object>();
                        notification["notificationIcon"] = Convert.ToString(dr["Notification_Icon"]);
                        //notification["notification"] = dr["notification_title"] + "<br>" + dr["notification_msg"];
                        notification["notification"] = dr["notification_title"] + "<br>" + RemoveExtraInfo(Convert.ToString(dr["notification_msg"]));

                        notification["ID"] = dr["Notification_Id"];
                        notification["time"] = dr["Time"];
                        notification["recordInsertedTime"] = dr["start_datetime"];
                        notification["Read_Count_Customer"] = dr["Read_Count_Customer"];
                        notification["source_flag"] = dr["source_flag"];
                        notification["Notification_Id"] = dr["Notification_Id"];
                        notification["Submodule_Id"] = dr["Submodule_Id"];
                        notification["Notif_Id"] = Convert.ToString(dr["Notif_Id"]);
                        notification["InApp_Notif_Id"] = Convert.ToString(dr["InApp_Notif_Id"]);
                        //Start Parth added to get total count of unread notifications
                        #region get total count of unread notifications
                        try
                        {
                            // Count unread where Read_Count_Customer = 1
                            if (Convert.ToInt32(dr["Read_Count_Customer"]) == 1 && (!string.IsNullOrEmpty(Convert.ToString(dr["Notif_Id"]))))
                            {
                                unreadCount++;
                            }
                        }
                        catch(Exception ex)
                        {
                            _ = Task.Run (() => CompanyInfo.InsertErrorLogTracker(ex.ToString(), 0, 0, 0, 0, "Viewnotifications", Convert.ToInt32(Obj.Branch_ID), Convert.ToInt32(Obj.Client_ID), "", HttpContext));
                        }
                        #endregion get total count of unread notifications
                        //Start Parth added to get total count of unread notifications
                        notifications.Add(notification);
                    }
                    validateJsonData = new { response = true, responseCode = "00", data = notifications, totalUnreadNotifications = unreadCount };
                    return new JsonResult(validateJsonData);

                    /*var returndata = li1s.Rows.OfType<DataRow>()
                 .Select(row => li1s.Columns.OfType<DataColumn>()
                     .ToDictionary(col => col.ColumnName, c => row[c]));
                    validateJsonData = new { response = true, responseCode = "00", data = returndata };
                    return new JsonResult(validateJsonData);*/

                }
                else
                {
                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_TECHNICAL_ERROR);
                    response.ResponseMessage = "No Records Found.";
                    response.ResponseCode = 0;
                    validateJsonData = new { response = false, responseCode = "02", data = response.ResponseMessage };
                    return new JsonResult(validateJsonData);
                }
                // Updated code for notification end



                int Customer_ID = Convert.ToInt32(CompanyInfo.Decrypt(Obj.Id, true));
                List<Model.Customer> _lst = new List<Model.Customer>();
                MySqlConnector.MySqlCommand cmd = new MySqlConnector.MySqlCommand("Get_DashboardNotifications");
                cmd.CommandType = CommandType.StoredProcedure;
                string _whereclause = "";

                DataTable ds = new DataTable();

                if (Obj.RemindMeFlag == 0)
                {
                    DateTime GetFrom_Date = DateTime.ParseExact(Convert.ToString(data.date), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    string date = GetFrom_Date.ToString("yyyy-MM-dd");
                    _whereclause = _whereclause + " notification.Customer_ID=" + Customer_ID + " and notification.client_id= " + Obj.Client_ID + " and show_notificationfor_customer=0 and date(notification.Record_Insert_DateTime) between '" + date + "' and '" + DateTime.Now.ToString("yyyy-MM-dd") + "' and show_notificationfor_customer=0 and notification.Read_Count_Customer=1  order by notification.Record_Insert_DateTime desc limit 5";
                }
                else
                {

                    DateTime GetFrom_Date = DateTime.ParseExact(Convert.ToString(data.date), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    string date = GetFrom_Date.ToString("yyyy-MM-dd");

                    _whereclause = _whereclause + " notification.Customer_ID=" + Customer_ID + " and  notification.client_id= " + Obj.Client_ID + " and show_notificationfor_customer=0 and date(notification.Record_Insert_DateTime) between '" + date + "' and '" + DateTime.Now.ToString("yyyy-MM-dd") + "'  order by notification.Record_Insert_DateTime desc,ID";
                }
                cmd.Parameters.AddWithValue("_Branch_ID", Obj.Branch_ID);

                cmd.Parameters.AddWithValue("_whereclause", _whereclause);
                ds = db_connection.ExecuteQueryDataTableProcedure(cmd);
                if (ds.Rows.Count > 0)
                {
                    List<Dictionary<string, object>> notifications = new List<Dictionary<string, object>>();

                    foreach (DataRow dr in ds.Rows)
                    {
                        Dictionary<string, object> notification = new Dictionary<string, object>();
                        notification["notificationIcon"] = dr["Notification_Icon"];
                        notification["notification"] = dr["Notification"];
                        notification["ID"] = dr["ID"];
                        notification["time"] = dr["Time"];
                        notification["recordInsertedTime"] = dr["Record_Insert_datetime"];
                        notifications.Add(notification);
                    }
                    validateJsonData = new { response = true, responseCode = "00", data = notifications };
                    return new JsonResult(validateJsonData);
                }
                validateJsonData = new { response = false, responseCode = "02", data = "Invalid Request" };
                return new JsonResult(validateJsonData);

            }
            catch (Exception ex)
            {

                validateJsonData = new { response = false, responseCode = "01", data = "Invalid Request" };
                CompanyInfo.InsertErrorLogTracker(ex.ToString(), 0, 0, 0, 0, "Viewnotifications", Convert.ToInt32(Obj.Branch_ID), Convert.ToInt32(Obj.Client_ID), "", HttpContext);
                return new JsonResult(validateJsonData);

            }
        }

        [HttpPost]
        [Authorize]
        [Route("notificationmodule")]
        public IActionResult NotificationModule([FromBody] JsonObject obj)
        {
            Model.response.WebResponse response = null;
            string json = System.Text.Json.JsonSerializer.Serialize(obj);
            dynamic data = JObject.Parse(json);
            CompanyInfo.InsertrequestLogTracker("notificationmodule full request body: " + JObject.Parse(json), 0, 0, 0, 0, "NotificationModule", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
            var validateJsonData = (dynamic)null;
            Customer Obj = JsonConvert.DeserializeObject<Customer>(json);
            try
            {
                if (!SqlInjectionProtector.ReadJsonObjectValues(obj) || !SqlInjectionProtector.ReadJsonObjectValuesScript(obj))
                {
                    var errorResponse = new { response = false, responseCode = "02", data = "Invalid input detected." };
                    return new JsonResult(errorResponse) { StatusCode = 400 };
                }

                int number;
                if (!int.TryParse(Convert.ToString(data.clientID), out number) || data.clientID == "" || data.clientID == null)
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "Invalid Client ID." };
                    return new JsonResult(validateJsonData);
                }
                if (!int.TryParse(Convert.ToString(data.branchID), out number) || data.branchID == "" || data.branchID == null)
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "Invalid branchID field." };
                    return new JsonResult(validateJsonData);
                }
                if (data.customerID == "" || data.customerID == null)
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "Invalid customer ID ." };
                    return new JsonResult(validateJsonData);
                }
                Obj.Client_ID = data.clientID;
                Obj.Branch_ID = data.branchID;
                Obj.Id = data.customerID;
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

                int Customer_Id = Convert.ToInt32(CompanyInfo.Decrypt(Convert.ToString(Obj.Id), true));
                MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("Get_Notification_config");
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.Parameters.AddWithValue("_Client_ID", Obj.Client_ID);
                _cmd.Parameters.AddWithValue("_Customer_ID", Customer_Id);
                _cmd.Parameters.AddWithValue("_Branch_ID", Obj.Branch_ID);
                DataTable dt = db_connection.ExecuteQueryDataTableProcedure(_cmd);

                if (dt.Rows.Count > 0)
                {
                    List<Dictionary<string, object>> notifications = new List<Dictionary<string, object>>();

                    var relationshipData = dt.Rows.OfType<DataRow>()
             .Select(row => dt.Columns.OfType<DataColumn>()
                 .ToDictionary(col => col.ColumnName, c => row[c].ToString()));
                    validateJsonData = new { response = true, responseCode = "00", data = relationshipData };
                    return new JsonResult(validateJsonData);

                    /*foreach (DataRow dr in dt.Rows)
                    {
                        Dictionary<string, object> notification = new Dictionary<string, object>();
                        notification["notificationModule"] = dr["Notification_module"];
                        notification["notificationID"] = dr["Notification_ID"];
                        notification["customerID"] = dr["Customer_Id"];
                        notification["moduleStatus"] = dr["module_status"];
                        notifications.Add(notification);
                    }
                    validateJsonData = new { response = true, responseCode = "00", data = notifications };
                    return new JsonResult(validateJsonData);*/
                }
                validateJsonData = new { response = false, responseCode = "02", data = "No record found." };
                return new JsonResult(validateJsonData);
            }
            catch (Exception ex)
            {
                validateJsonData = new { response = false, responseCode = "01", data = "Invalid Request" };
                CompanyInfo.InsertErrorLogTracker(ex.ToString(), 0, 0, 0, 0, "notificationmodule", Convert.ToInt32(Obj.Branch_ID), Convert.ToInt32(Obj.Client_ID), "", HttpContext);
                return new JsonResult(validateJsonData);
            }
        }

        [HttpPost]
        [Authorize]
        [Route("updatenotificationmodule")]
        public IActionResult Update_Notification_config([FromBody] JsonObject obj)
        {
            Model.response.WebResponse response = null;
            string json = System.Text.Json.JsonSerializer.Serialize(obj);
            dynamic data = JObject.Parse(json);
            CompanyInfo.InsertrequestLogTracker("updatenotificationmodule full request body: " + JObject.Parse(json), 0, 0, 0, 0, "Update_Notification_config", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
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
            var validateJsonData = (dynamic)null;
            Notification Obj = JsonConvert.DeserializeObject<Notification>(json);
            try
            {
                if (!SqlInjectionProtector.ReadJsonObjectValues(obj) || !SqlInjectionProtector.ReadJsonObjectValuesScript(obj))
                {
                    var errorResponse = new { response = false, responseCode = "02", data = "Invalid input detected." };
                    return new JsonResult(errorResponse) { StatusCode = 400 };
                }

                int number;
                if (!int.TryParse(Convert.ToString(data.clientID), out number) || data.clientID == "" || data.clientID == null)
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "Invalid Client ID." };
                    return new JsonResult(validateJsonData);
                }
                if (!int.TryParse(Convert.ToString(data.branchID), out number) || data.branchID == "" || data.branchID == null)
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "Invalid branchID field." };
                    return new JsonResult(validateJsonData);
                }
                if (data.moduleID == "" || data.moduleID == null)
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "Invalid module ID ." };
                    return new JsonResult(validateJsonData);
                }
                if (data.customerID == "" || data.customerID == null)
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "Invalid customer ID ." };
                    return new JsonResult(validateJsonData);
                }
                if (data.countryID == "" || data.countryID == null)
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "Invalid country ID ." };
                    return new JsonResult(validateJsonData);
                }
                if (!int.TryParse(Convert.ToString(data.deleteStatus), out number) || data.deleteStatus == "" || data.deleteStatus == null)
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "Invalid delete Status." };
                    return new JsonResult(validateJsonData);
                }

                Obj.Client_Id = data.clientID;
                Obj.Branch_Id = data.branchID;
                Obj.Tocheck = data.countryID;
                Obj.Module_Id = data.moduleID;
                Obj.Delete_Status = data.deleteStatus;
                Obj.Customer_Id = data.customerID;
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

                string msg = "";
                int Customer_Id = Convert.ToInt32(CompanyInfo.Decrypt(Convert.ToString(Obj.Customer_Id), true));
                Regex regex = new Regex(@"^(\d +[,]\d*|\d*[,]\d +)$");
                string error = "true";
                if (regex.IsMatch(Obj.Tocheck) == false)
                { error = "false"; }
                if (error == "false")
                {
                    string chk_param = ""; string ii = "";
                    if (Obj.Module_Id == 6)
                    {
                        if (Obj.Tocheck.Contains("2"))
                        { //Cashback
                            if (chk_param != "")
                            {
                                chk_param = chk_param + "," + "21,22,23,24";
                            }
                            else
                            {
                                chk_param = "21,22,23,24";
                            }

                        }
                        if (Obj.Tocheck.Contains("1")) //Discount
                        {
                            if (chk_param != "")
                            {
                                chk_param = chk_param + "," + "28,32";
                            }
                            else
                            {
                                chk_param = "28,32";
                            }
                        }
                        if (Obj.Tocheck.Contains("3")) //Referral
                        {
                            if (chk_param != "")
                            {
                                chk_param = chk_param + "," + "25,26,60,61";
                            }
                            else
                            {
                                chk_param = "25,26,60,61";
                            }
                        }
                        string whereclause = "";
                        whereclause = "and sn.SubNotification_ID IN(" + chk_param + ") and nat.Branch_id=2";
                        MySqlConnector.MySqlCommand cmd = new MySqlConnector.MySqlCommand("Get_Subnotifications");
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("_Client_ID", Obj.Client_Id);
                        cmd.Parameters.AddWithValue("_whereclause", whereclause);
                        DataTable dt = db_connection.ExecuteQueryDataTableProcedure(cmd);

                        for (int i1 = 0; i1 < dt.Rows.Count; i1++)
                        {
                            if (ii != "")
                            {
                                ii = ii + "," + Convert.ToString(dt.Rows[i1]["Sub_Notification_ID"]);
                            }
                            else
                            {
                                ii = Convert.ToString(dt.Rows[i1]["Sub_Notification_ID"]);
                            }
                        }
                    }
                    else
                    {
                        ii = Obj.Tocheck;
                    }
                    MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("Update_Notification_config");
                    _cmd.CommandType = CommandType.StoredProcedure;
                    _cmd.Parameters.AddWithValue("_Client_ID", Obj.Client_Id);
                    _cmd.Parameters.AddWithValue("_Customer_Id", Customer_Id);
                    _cmd.Parameters.AddWithValue("_Check_list", ii);
                    _cmd.Parameters.AddWithValue("_Module_Id", Obj.Module_Id);
                    _cmd.Parameters.AddWithValue("_Delete_Status", Obj.Delete_Status);
                    int i = db_connection.ExecuteNonQueryProcedure(_cmd);
                    if (i > 0)
                    {
                        validateJsonData = new { response = true, responseCode = "00", data = "Module ID changed successfully" };
                        return new JsonResult(validateJsonData);
                    }
                    validateJsonData = new { response = false, responseCode = "02", data = "Invalid Request" };
                    return new JsonResult(validateJsonData);                
            }
                else
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "Invalid Request" };
                    return new JsonResult(validateJsonData);
                }
            }
            catch (Exception ex)
            {
                validateJsonData = new { response = false, responseCode = "01", data = "Invalid Request" };
                CompanyInfo.InsertErrorLogTracker(ex.ToString(), 0, 0, 0, 0, "Update_Notification_config", Convert.ToInt32(Obj.Branch_Id), Convert.ToInt32(Obj.Client_Id), "", HttpContext);
                return new JsonResult(validateJsonData);
            }
        }

        [HttpPost]
        [Route("updatenotificationstatus")]
        public IActionResult UpdateNotificationStatus([FromBody] JsonObject objjson)
        {
            Model.response.WebResponse response = null;
            string json = System.Text.Json.JsonSerializer.Serialize(objjson);
            dynamic data = JObject.Parse(json);
            CompanyInfo.InsertrequestLogTracker("updatenotificationstatus full request body: " + JObject.Parse(json), 0, 0, 0, 0, "UpdateNotificationStatus", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
            var validateJsonData = (dynamic)null;
            try
            {
                Dashboard obj  = JsonConvert.DeserializeObject<Dashboard>(json);
                Service.srvDashboard srv = new Service.srvDashboard();
                response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_SUCCESS);
                response.data = srv.UpdateNotificationStatus(obj);
                
                Model.ActivityLogTracker _ActivityObj = new Model.ActivityLogTracker();
               
                if (response.data == "success")
                {
                    _ActivityObj.Activity = "Updated notification status. " + obj.Id + ": <b>" + obj.status + " </b></br>";
                    _ActivityObj.Activity = "Notification deleted successfully";
                    validateJsonData = new { response = true, responseCode = "00", data = _ActivityObj.Activity };
                }
                else
                {
                    _ActivityObj.Activity = "Failed to notification status. " + obj.Id + ": <b>" + obj.status + " </b></br>";
                    _ActivityObj.Activity = "Failed to delete Notification";
                    validateJsonData = new { response = false, responseCode = "02", data = _ActivityObj.Activity };
                }
                return new JsonResult(validateJsonData);
            }
            catch (Exception ex)
            {
                validateJsonData = new { response = false, responseCode = "01", data = "Failed to delete Notification" };               
                response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_TECHNICAL_ERROR);
                response.sErrorExceptionText = ex.ToString();
                response.sErrorExceptionText = "technical error";

                //Error Log Handled
                Model.ErrorLog objError = new Model.ErrorLog();
                objError.Error = "Api : Update Status --" + ex.ToString();
                objError.Date = DateTime.Now;
                objError.User.Id = 1;
                objError.Id = 1;
                Service.srvErrorLog srvError = new Service.srvErrorLog();
                srvError.Create(objError, HttpContext);
                return new JsonResult(validateJsonData);
            }

        }

    }
}
           

