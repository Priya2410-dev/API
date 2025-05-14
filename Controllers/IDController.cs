using Microsoft.AspNetCore.Mvc;
using static Google.Apis.Requests.BatchRequest;
using System.Data;
using System.Text.Json.Nodes;
using Newtonsoft.Json.Linq;
using Calyx_Solutions.Model;
using Microsoft.AspNetCore.Authorization;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Web.Helpers;
using System.Web.DynamicData;
using System.Transactions;
using Calyx_Solutions.Service;
using Microsoft.Ajax.Utilities;
using MySqlConnector;
using Microsoft.Net.Http.Headers;
using OfficeOpenXml.FormulaParsing.LexicalAnalysis;
using System.Net;
using System.Security.Claims;
//using Microsoft.Ajax.Utilities;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Calyx_Solutions.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IDController : ControllerBase
    {
        [HttpPost]
        [Route("getiddocuments")]
        public IActionResult GetIDdocuments([FromBody] JsonObject obj)
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
            CompanyInfo.InsertrequestLogTracker("getiddocuments full request body: " + JObject.Parse(json), 0, 0, 0, 0, "GetIDdocuments", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
            var validateJsonData = (dynamic)null;
            Document Obj = JsonConvert.DeserializeObject<Document>(json);
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
                else if (data.customerID == "" || data.customerID == null)
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "Invalid customer ID." };
                    return new JsonResult(validateJsonData);
                }
                else if (!int.TryParse(Convert.ToString(data.branchID), out number) || data.branchID == "" || data.branchID == null)
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "Invalid branch ID." };
                    return new JsonResult(validateJsonData);
                }

                else if (data.idType == "" || data.idType == null)
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "Invalid Id Type." };
                    return new JsonResult(validateJsonData);
                }
                Obj.Client_ID = data.clientID;
                Obj.Branch_ID = data.branchID;
                Obj.Customer_ID = data.customerID;
                Obj.IDType_ID = data.idType;
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
                MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("IDDoc_list_app");
                _cmd.CommandType = CommandType.StoredProcedure;
                int benf_id = Convert.ToInt32(data.beneficiaryID);
                DataTable dt = new DataTable();
                int Customer_ID = Convert.ToInt32(CompanyInfo.Decrypt(Obj.Customer_ID, true));
                _cmd.Parameters.AddWithValue("_Client_ID", Obj.Client_ID);
                string _whereclause = "";
                string get_flag = Obj.Comments;
                if (Obj.SenderID_ID > 0)
                {
                    _whereclause = " and SenderID_ID =" + Obj.SenderID_ID;
                }
                if (Obj.IDType_ID != 0 && Obj.IDType_ID != null)
                {
                    _whereclause = _whereclause + " and documents_details.IDType_ID=" + Obj.IDType_ID;
                }

                if (Obj.ID_Name != "" && Obj.ID_Name != null)
                {
                    _whereclause = _whereclause + " and ID_Name LIKE '%" + Obj.ID_Name + "%'";
                }
                if (benf_id != -1 && benf_id != null && benf_id != 0)
                {
                    _whereclause = _whereclause + " and Beneficiary_ID= " + benf_id + "";
                    get_flag = "from_benf_id_documents";
                }

                if (benf_id == null || benf_id <= 0)
                {
                    get_flag = "from_id_documents";
                    if (get_flag == "from_id_documents")
                    {
                        _whereclause = _whereclause + " and ( Beneficiary_ID is null  OR Beneficiary_ID=0) ";
                    }
                }

                if (Customer_ID != -1 && Customer_ID != 0)
                {

                    _whereclause = _whereclause + " and customer_id= " + Customer_ID + "";

                    _cmd.Parameters.AddWithValue("_whereclause", _whereclause);
                    dt = db_connection.ExecuteQueryDataTableProcedure(_cmd);
                    List<Dictionary<string, object>> iddocs = new List<Dictionary<string, object>>();
                    foreach (DataRow dr in dt.Rows)
                    {
                        Dictionary<string, object> id = new Dictionary<string, object>();

                        id["documentTypeID"] = dr["IDType_ID"];
                        id["idName"] = dr["ID_Name"];
                        id["idType"] = dr["ID_Type"];
                        id["senderIDExpiryDate"] = dr["SenderID_ExpiryDate"];
                        id["senderIDNumber"] = dr["Sender_ID_Number"];
                        id["senderNameOnID"] = dr["SenderNameOnID"];
                        id["expired"] = dr["Expired"];
                        iddocs.Add(id);
                    }
                    validateJsonData = new { response = true, responseCode = "00", data = iddocs };
                    //CompanyInfo.InsertActivityLogDetails("Something went wrong. Please try again later.", 0, 0, 0, 0, "GetIDdocuments", Obj.Client_ID, Obj.Branch_ID, "", HttpContext);
                    return new JsonResult(validateJsonData);

                }
                validateJsonData = new { response = false, responseCode = "02", data = "No record found." };
                return new JsonResult(validateJsonData);
            }
            catch (Exception ex)
            {
                validateJsonData = new { response = false, responseCode = "01", data = "Invalid Request." };
                CompanyInfo.InsertErrorLogTracker(ex.ToString(), 0, 0, 0, 0, "GetIDdocuments", Convert.ToInt32(Obj.Branch_ID), Convert.ToInt32(Obj.Client_ID), "", HttpContext);
                return new JsonResult(validateJsonData);
            }
        }


        // POST api/<IDController>
        [HttpPost]
        [Authorize]
        [Route("getiddetails")]
        public IActionResult GetIDDetails([FromBody] JsonElement Obj )
        {
            HttpContext context = HttpContext;
            Model.response.WebResponse response = null;
            string json1 = System.Text.Json.JsonSerializer.Serialize(Obj);
            dynamic data = JObject.Parse(json1);
            CompanyInfo.InsertrequestLogTracker("getiddetails full request body: " + JObject.Parse(json1), 0, 0, 0, 0, "GetIDDetails", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
            Document obj = new Document();
            obj.Client_ID = data.clientID;
            obj.Branch_ID = data.branchID;


            if (!SqlInjectionProtector.ReadJsonElementValues(Obj) || !SqlInjectionProtector.ReadJsonElementValuesScript(Obj))
            {
                var errorResponse = new { response = false, responseCode = "02", data = "Invalid input detected." };
                return new JsonResult(errorResponse) { StatusCode = 400 };
            }

            try
            {
                obj.Country_ID = data.countryID;
            }
            catch { obj.Country_ID = 0; }
            var validateJsonData = (dynamic)null;

            int number;
            if (!int.TryParse(Convert.ToString(data.clientID), out number)) { 
                validateJsonData = new { response = false, responseCode = "02", data = "Invalid Client ID." };
                return new JsonResult(validateJsonData);
        }
            else if (!int.TryParse(Convert.ToString(data.branchID), out number))            
            {
                validateJsonData = new { response = false, responseCode = "02", data = "Invalid Branch ID." };
                return new JsonResult(validateJsonData);
            }
            else if (!int.TryParse(Convert.ToString(obj.Country_ID), out number))
            {
                validateJsonData = new { response = false, responseCode = "02", data = "Invalid Country ID." };
                return new JsonResult(validateJsonData);
            }

            try
            {

                Service.srvDocument srv = new Service.srvDocument();
                DataTable li1 = srv.GetIDTypes(obj, context);
                DataTable li2 = srv.GetIDNames(obj, context);
                DataTable[] dt = new DataTable[2];
                dt[0] = li1; dt[0].TableName = "IDTypes";
                dt[1] = li2; dt[1].TableName = "IDNames";
                
                if (li1 != null && li1.Rows.Count > 0)
                {
                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_SUCCESS);
                    response.data = "success";
                    response.ObjData = li1;
                    response.ResponseCode = 0;

                    var relationshipData = li1.Rows.OfType<DataRow>()
                .Select(row => li1.Columns.OfType<DataColumn>()
                    .ToDictionary(col => col.ColumnName, c => row[c].ToString()));

                    foreach (DataColumn column in li1.Columns)
                    {
                        // Replace "OldColumnName" with the desired new name
                        column.ColumnName = column.ColumnName.Replace("IDType_ID", "typeID");
                        column.ColumnName = column.ColumnName.Replace("ID_Type", "typeName");
                        column.ColumnName = column.ColumnName.Replace("Delete_Status", "activeStatus");
                    }

                    validateJsonData = new { response = true, responseCode = "00", data = relationshipData };
                    return new JsonResult(validateJsonData);
                }
                else
                {
                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_TECHNICAL_ERROR);
                    response.ResponseMessage = "No record found";
                    response.ResponseCode = 1;
                    
                    validateJsonData = new { response = false, responseCode = "02", data = response.ResponseMessage };
                    return new JsonResult(validateJsonData);
                }
                
               /*
                Dictionary<string, string> jsonDictionary = new Dictionary<string, string>();
                for (int i = 0; i < dt.Length; i++)
                {
                    string jsonName = ""; 
                    if (i == 0)
                    {
                        jsonName = "IDTypes";
                    }
                    if (i == 1)
                    {
                        jsonName = "IDNames";
                    }

                    
                    var json = CompanyInfo.DataTableToJSONWithNewtonsoftJson(dt[i]);
                    json = CompanyInfo.UnescapeJson(json); // Remove escape characters
                    jsonDictionary.Add(jsonName, json);
                }

                List<Dictionary<string, object>> trasaction1 = new List<Dictionary<string, object>>();
                validateJsonData = new { response = true, responseCode = "00", data = jsonDictionary };
                return new JsonResult(validateJsonData);
               */
            }
            catch(Exception ex)
            {
                validateJsonData = new { response = false, responseCode = "01", data = "Invalid Request" };
                string Acitivy = "Api: GetIDDetails " + ex.ToString();
                CompanyInfo.InsertErrorLogTracker(Acitivy.ToString(), 0, 0, 0, 0, "GetIDDetails", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "", context);
                return new JsonResult(validateJsonData);
            }

        }


        [HttpGet]
        [Route("documentlist")]
        public IActionResult DocumentList()
        {
            try
            {
                var DocumentType = new[]
                {
            new { typeID = 1, typeName = "Document Scan" },
            new { typeID = 2, typeName = "Manual Upload" },

            };

                var jsonData = new { response = true, responseCode = "00", data = DocumentType };
                return new JsonResult(jsonData);
            }
            catch (Exception ex)
            {
                var jsonData = new { response = false, responseCode = "01", data = "Invalid Request" };
                return new JsonResult(jsonData);
            }
        }

        [HttpPost]
        [Route("documentlist")]
        public IActionResult DocumentListpost()
        {
            try
            {
                var DocumentType = new[]
                {
            new { typeID = 1, typeName = "Document Scan" },
            new { typeID = 2, typeName = "Manual Upload" },

            };

                var jsonData = new { response = true, responseCode = "00", data = DocumentType };
                return new JsonResult(jsonData);
            }
            catch (Exception ex)
            {
                var jsonData = new { response = false, responseCode = "01", data = "Invalid Request" };
                return new JsonResult(jsonData);
            }
        }


        [HttpPost]
        [Authorize]
        [Route("getidtypename")]
        public IActionResult GetIDTypenameDetails([FromBody] JsonElement Obj)
        {
            HttpContext context = HttpContext;
            Model.response.WebResponse response = null;
            string json1 = System.Text.Json.JsonSerializer.Serialize(Obj);
            dynamic data = JObject.Parse(json1);
            CompanyInfo.InsertrequestLogTracker("getidtypename full request body: " + JObject.Parse(json1), 0, 0, 0, 0, "GetIDTypenameDetails", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
            Document obj = new Document();
            obj.Client_ID = data.clientID;
            obj.Branch_ID = data.branchID;
            obj.Country_ID = data.countryID;
            obj.IDType_ID  = data.typeID;
            var validateJsonData = (dynamic)null;

            if (!SqlInjectionProtector.ReadJsonElementValues(Obj) || !SqlInjectionProtector.ReadJsonElementValuesScript(Obj))
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
            
            else if (!int.TryParse(Convert.ToString(data.branchID), out number))            
            {
                validateJsonData = new { response = false, responseCode = "02", data = "Invalid Branch ID." };
                return new JsonResult(validateJsonData);
            }             
            else if (!int.TryParse(Convert.ToString(data.countryID), out number))
            {
                validateJsonData = new { response = false, responseCode = "02", data = "Invalid Country ID." };
                return new JsonResult(validateJsonData);
            }
            else if (!int.TryParse(Convert.ToString(data.typeID), out number))
            {
                validateJsonData = new { response = false, responseCode = "02", data = "Invalid Documenttype ID." };
                return new JsonResult(validateJsonData);
            }

            try
            {

                Service.srvDocument srv = new Service.srvDocument();
                
                DataTable li1 = srv.GetIDNames(obj, context);
                DataTable[] dt = new DataTable[2];
                 
                dt[1] = li1; dt[1].TableName = "IDNames";

                if (li1 != null && li1.Rows.Count > 0)
                {
                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_SUCCESS);
                    response.data = "success";
                    response.ObjData = li1;
                    response.ResponseCode = 0;

                    var relationshipData = li1.Rows.OfType<DataRow>()
                .Select(row => li1.Columns.OfType<DataColumn>()
                    .ToDictionary(col => col.ColumnName, c => row[c].ToString()));

                    foreach (DataColumn column in li1.Columns)
                    {
                        // Replace "OldColumnName" with the desired new name
                        column.ColumnName = column.ColumnName.Replace("IDName_ID", "nameID");
                        column.ColumnName = column.ColumnName.Replace("ID_Name", "idName");
                    }

                    validateJsonData = new { response = true, responseCode = "00", data = relationshipData };
                    return new JsonResult(validateJsonData);
                }
                else
                { 
                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_TECHNICAL_ERROR);
                    response.ResponseMessage = "No record found";
                    response.ResponseCode = 1;
                    validateJsonData = new { response = false, responseCode = "02", data = response.ResponseMessage };
                    return new JsonResult(validateJsonData);
                }

                
            }
            catch (Exception ex)
            {
                validateJsonData = new { response = false, responseCode = "01", data = "Invalid Request" };
                string Acitivy = "Api: GetIDTypenameDetails " + ex.ToString();
                CompanyInfo.InsertErrorLogTracker(Acitivy.ToString(), 0, 0, 0, 0, "GetIDTypenameDetails", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "", context);
                return new JsonResult(validateJsonData);
            }

        }


        //public IActionResult upload([FromBody] JsonElement obj)
        [HttpPost]
        [Authorize]
        [Consumes("multipart/form-data")]              
        [Route("Upload")]      
          public async Task<IActionResult> Upload([FromForm] Doc obj1, [FromForm] IFormFile? f_file, [FromForm] IFormFile? b_file)
          {
            var returnJsonData = (dynamic)null;
            HttpContext context = HttpContext;
            Model.response.WebResponse response = null;
            string json1 = System.Text.Json.JsonSerializer.Serialize(obj1);
            dynamic data = JObject.Parse(json1);
            JsonElement jsonElement = System.Text.Json.JsonSerializer.SerializeToElement(obj1);//sanket // Converted JsonObject to JsonElement
            var claimsIdentity = User.Identity as ClaimsIdentity;//sanket
            var authHeader = "";
            try { authHeader = HttpContext.Request.Headers["Authorization"].ToString(); } catch (Exception egx) { authHeader = ""; }

            bool auth = AuthController.checkAuth(claimsIdentity, jsonElement, Convert.ToString(authHeader));
            if (!auth)
            {
                var errorResponse = new { response = false, responseCode = "403", data = "Access Forbidden" };
                return new JsonResult(errorResponse) { StatusCode = (int)HttpStatusCode.Forbidden };
            }
            _ = Task.Run(() => CompanyInfo.InsertrequestLogTracker("Upload full request body: " + JObject.Parse(json1), 0, 0, 0, 0, "Upload", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext));

            Doc obj = new Doc();
            Document doo = new Document();

            // Populate obj1 properties if they exist in the data
            try
            {
            obj.Client_ID = data.clientID;
            obj.Branch_ID = data.branchID;
            obj.SenderNameOnID = data.sendernameonid;
            obj.SenderID_Number = data.senderidnumber;
            obj.IDType_ID = data.typeID;
            obj.IDName_ID = data.nameID;
            obj.SenderID_ExpiryDate = data.senderidexpirydate;
            obj.Sender_DateOfBirth = data.senderdateofbirth;
            obj.Customer_ID = data.customerid;
            obj.Comments = data.comments;
            obj.DocumentName = data.documentname;
            obj.SenderID_PlaceOfIssue = data.senderidplaceofissue;
            obj.Country_ID = data.countryID;
            obj.User_ID = data.userID;
            obj.Beneficiary_ID = data.BenfID;
            obj.Issue_date = data.issuedate;
            obj.Place_Of_ID = data.placeOfID;
            try { obj.MRZ_number = data.mrznumber; } catch (Exception ex) { obj.MRZ_number = ""; }
            obj.tokenValue = Request.Headers[HeaderNames.Authorization];
                try { obj.MRZ_number_Second = data.mrznumbersecond; } catch (Exception ex) { obj.MRZ_number_Second = ""; }

                string  readTokenValue = obj.tokenValue;
                readTokenValue = readTokenValue.Replace("Bearer ", "");

                /*if (!SqlInjectionProtector.ValidateObjectForSqlInjection(obj) || !SqlInjectionProtector.ValidateObjectForScriptSqlInjection(obj))
                {
                    var errorResponse = new { response = false, responseCode = "02", data = "Invalid input detected." };
                    return new JsonResult(errorResponse) { StatusCode = 400 };
                }*/

                var validateJsonData = (dynamic)null;

            Model.Beneficiary _ObjCustomer = new Model.Beneficiary();
            Service.srvDocument srv = new Service.srvDocument();

            DataTable li1 = await srv.Upload(obj, context, f_file , b_file);

            response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_SUCCESS);
            string exist = li1.Rows[0]["status"].ToString();
            if (exist == "exist_Id")
            {
                response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_TECHNICAL_ERROR);
                response.data = "exist_Id";
                    response.data = "Already exist.";
                    response.ObjData = li1;
                response.ResponseCode = 1;
                returnJsonData = new { response = false, responseCode = "02", data = response.data };
                return new JsonResult(returnJsonData);
            }
            else if (exist == "Validation Failed")
            {
                response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_TECHNICAL_ERROR);
                response.data = "Validation Failed.";
                response.ObjData = li1;
                response.ResponseCode = 1;
                returnJsonData = new { response = false, responseCode = "02", data = response.data };
                return new JsonResult(returnJsonData);
            }
            else
            {
                response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_TECHNICAL_ERROR);
                response.data = "success";
                    response.data = "Record update successfully.";
                    response.ObjData = li1;
                response.ResponseCode = 0;
                returnJsonData = new { response = true, responseCode = "00", data = response.data };
                return new JsonResult(returnJsonData);
            }

             
            }
            catch (Exception ex)
            {
                returnJsonData = new { response = false, responseCode = "01", data = "Invalid Request" };
                string Acitivy = "Api: Upload " + ex.ToString();
                CompanyInfo.InsertErrorLogTracker(Acitivy.ToString(), 0, 0, 0, 0, "Upload", Convert.ToInt32(data.branchID), Convert.ToInt32(data.clientID), "", context);
                return new JsonResult(returnJsonData);
            }
        }
        

        [HttpPost]
        [Authorize]
        [Route("customeridconfig")]

        public IActionResult getIDconfig([FromBody] JsonElement obj1)
        {
            HttpContext context = HttpContext;
            string json = System.Text.Json.JsonSerializer.Serialize(obj1);
            dynamic data = JObject.Parse(json); string Status = string.Empty;
            CompanyInfo.InsertrequestLogTracker("customeridconfig full request body: " + JObject.Parse(json), 0, 0, 0, 0, "getIDconfig", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
            Model.response.WebResponse response = null;
            Document obj = new Document();

            if (!SqlInjectionProtector.ReadJsonElementValues(obj1) || !SqlInjectionProtector.ReadJsonElementValuesScript(obj1))
            {
                var errorResponse = new { response = false, responseCode = "02", data = "Invalid input detected." };
                return new JsonResult(errorResponse) { StatusCode = 400 };
            }

            //Customer obj_1 = new Customer();

            DataTable dt = new DataTable();


            obj.Client_ID = data.clientID;
            obj.Branch_ID = data.branchID;
            obj.IDType_ID = data.idTypeID;

            var validateJsonData = (dynamic)null;


            int number;
            if (!int.TryParse(Convert.ToString(data.clientID), out number))
            {
                validateJsonData = new { response = false, responseCode = "02", data = "Invalid Client ID." };
                return new JsonResult(validateJsonData);
            }
            if (!int.TryParse(Convert.ToString(data.branchID), out number))
            {
                validateJsonData = new { response = false, responseCode = "02", data = "Invalid branchID." };
                return new JsonResult(validateJsonData);
            }
            if (!int.TryParse(Convert.ToString(data.idTypeID), out number))
            {
                validateJsonData = new { response = false, responseCode = "02", data = "Invalid IDType." };
                return new JsonResult(validateJsonData);
            }

            try
            {
                Service.srvDocument srv = new Service.srvDocument();
                DataTable li1 = srv.select_idconfig(obj, context);

                List<Dictionary<string, object>> IdDocument = new List<Dictionary<string, object>>();
                if (li1 != null && li1.Rows.Count > 0)
                {
                    foreach (DataRow dr in li1.Rows)
                    {
                        Dictionary<string, object> Id_Document = new Dictionary<string, object>();
                        Id_Document["branchID"] = dr["Branch_ID"];
                        Id_Document["clientID"] = dr["Client_ID"];
                        Id_Document["deleteStatus"] = dr["Delete_Status"];
                        Id_Document["id"] = dr["ID"];
                        Id_Document["idConfigNo"] = dr["ID_Config_No"];
                        Id_Document["idType"] = dr["ID_Type"];
                        Id_Document["labelName"] = dr["Label_Name"];
                        Id_Document["mandatory"] = dr["Mandatory"];
                        Id_Document["setdefaultvalues"] = dr["Set_Default_Values"];
                        Id_Document["visibility"] = dr["Visibility"];
                        IdDocument.Add(Id_Document);
                    }
                    validateJsonData = new { response = true, responseCode = "00", data = IdDocument };
                    return new JsonResult(validateJsonData);
                }
                else
                {
                    validateJsonData = new { response = true, responseCode = "00", data = IdDocument };
                    return new JsonResult(validateJsonData);
                }
            }
            catch (Exception ex)
            {
                string Activity = "Api customeridconfig: " + ex.ToString() + " ";
                validateJsonData = new { response = false, responseCode = "01", data = "Invalid Request" };
                CompanyInfo.InsertErrorLogTracker(Activity.ToString(), 0, 0, 0, 0, "getIDInfo", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "", context);
                return new JsonResult(validateJsonData);
            }
        }

       
        [HttpPost]
        [Authorize]
        [Route("getjourney")]
        public IActionResult GetJourney([FromBody] JsonElement obj)
        {
            var validateJsonData = (dynamic)null;
            HttpContext context = HttpContext;
            Model.response.WebResponse response = null;
            string json1 = System.Text.Json.JsonSerializer.Serialize(obj);
            dynamic data = JObject.Parse(json1);
            var claimsIdentity = User.Identity as ClaimsIdentity;//sanket
            var authHeader = "";
            try { authHeader = HttpContext.Request.Headers["Authorization"].ToString(); } catch (Exception egx) { authHeader = ""; }

            bool auth = AuthController.checkAuth(claimsIdentity, obj, Convert.ToString(authHeader));
            if (!auth)
            {
                var errorResponse = new { response = false, responseCode = "403", data = "Access Forbidden" };
                return new JsonResult(errorResponse) { StatusCode = (int)HttpStatusCode.Forbidden };
            }
            CompanyInfo.InsertrequestLogTracker("getjourney full request body: " + JObject.Parse(json1), 0, 0, 0, 0, "GetJourney", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);

            if (!SqlInjectionProtector.ReadJsonElementValues(obj) || !SqlInjectionProtector.ReadJsonElementValuesScript(obj))
            {
                var errorResponse = new { response = false, responseCode = "02", data = "Invalid input detected." };
                return new JsonResult(errorResponse) { StatusCode = 400 };
            }

            Document obj1 = new Document();
            obj1.Client_ID = data.clientID;
            obj1.Branch_ID = data.branchID;
            obj1.Customer_ID = data.customerID;
            obj1.UserName = data.UserName;
            obj1.IDType_ID = data.IDType_ID;
            obj1.scanstatus = data.scanstatus;
            obj1.CustomerName = data.Name;
            obj1.JourneyID = data.JourneyID;

            try { obj1.MRZ_number = data.MRZ_number;
            if(obj1.MRZ_number== null) { obj1.MRZ_number = ""; }
            } catch(Exception ex) { obj1.MRZ_number = ""; }

            try
            {
                obj1.Custmer_Ref = data.Custmer_Ref;
                obj1.post_code = data.post_code;
                obj1.Adderess = data.Adderess;
                obj1.strete = data.strete;
                obj1.city = data.city;
                obj1.country_code = data.country_code;
                obj1.customer_email = data.customer_email;
                obj1.customer_mobile = data.customer_mobile;
                obj1.applicant_id = data.applicant_id;
            }
            catch (Exception ex) { }

            try
            {
            int Customer_ID = Convert.ToInt32(CompanyInfo.Decrypt(obj1.Customer_ID, true));
            if (Customer_ID > 0)
            {
                // context.Session["Customer_ID"] = Customer_ID;
                HttpContext.Session.SetInt32("Customer_ID", Customer_ID);
            
            }
            Service.srvDocument srv = new Service.srvDocument();
           // DataTable li1 = srv.GetJourney(obj1 , context);
            DataTable li1 = srv.GetJourney(obj1, context);

            if (li1 != null && li1.Rows.Count > 0)
            {
                response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_SUCCESS);
                response.data = "success";
                response.ObjData = li1;
                response.ResponseCode = 0;

                var relationshipData = li1.Rows.OfType<DataRow>()
            .Select(row => li1.Columns.OfType<DataColumn>()
                .ToDictionary(col => col.ColumnName, c => row[c].ToString()));

                foreach (DataColumn column in li1.Columns)
                {
                    // Replace "OldColumnName" with the desired new name
                    column.ColumnName = column.ColumnName.Replace("Status", "status");
                    column.ColumnName = column.ColumnName.Replace("ResponseMessage", "message");
                    column.ColumnName = column.ColumnName.Replace("ResultID", "resultID");
                }

                validateJsonData = new { response = true, responseCode = "00", data = relationshipData };
                return new JsonResult(validateJsonData);
            }
            else
            {
                response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_TECHNICAL_ERROR);
                response.ResponseMessage = "No record found.";
                response.ResponseCode = 1;

                validateJsonData = new { response = false, responseCode = "02", data = response.ResponseMessage };
                return new JsonResult(validateJsonData);
            }


            }
            catch (Exception ex)
            {
                string Activity = "Api GetJourney error: " + ex.ToString() + " ";
                validateJsonData = new { response = false, responseCode = "01", data = "Invalid Request." };
                CompanyInfo.InsertErrorLogTracker(Activity.ToString(), 0, 0, 0, 0, "GetJourney", Convert.ToInt32(obj1.Branch_ID), Convert.ToInt32(obj1.Client_ID), "", context);
                return new JsonResult(validateJsonData);
            }
        }

        [HttpPost]
        [Authorize]
        [Route("getgbgperm")]
        public IActionResult GetGBGperm([FromBody] JsonElement obj)
        {
            HttpContext context = HttpContext;
            Model.response.WebResponse response = null;
            string json1 = System.Text.Json.JsonSerializer.Serialize(obj);
            dynamic data = JObject.Parse(json1);
            var claimsIdentity = User.Identity as ClaimsIdentity;//sanket
            var authHeader = "";
            try { authHeader = HttpContext.Request.Headers["Authorization"].ToString(); } catch (Exception egx) { authHeader = ""; }

            bool auth = AuthController.checkAuth(claimsIdentity, obj, Convert.ToString(authHeader));
            if (!auth)
            {
                var errorResponse = new { response = false, responseCode = "403", data = "Access Forbidden" };
                return new JsonResult(errorResponse) { StatusCode = (int)HttpStatusCode.Forbidden };
            }
            CompanyInfo.InsertrequestLogTracker("getgbgperm full request body: " + JObject.Parse(json1), 0, 0, 0, 0, "GetGBGperm", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
            Document obj1 = new Document();

            if (!SqlInjectionProtector.ReadJsonElementValues(obj) || !SqlInjectionProtector.ReadJsonElementValuesScript(obj))
            {
                var errorResponse = new { response = false, responseCode = "02", data = "Invalid input detected." };
                return new JsonResult(errorResponse) { StatusCode = 400 };
            }

            obj1.Client_ID = data.clientID;
            obj1.Customer_ID = data.customerID;
            var validateJsonData = (dynamic)null;
            try
            {
                Service.srvDocument srv = new Service.srvDocument();
                DataTable li1 = srv.CheckIDScanPerm(obj1, context);
                Dictionary<string, string> jsonDictionary = new Dictionary<string, string>();
                if (li1 != null && li1.Rows.Count > 0)
                {
                    var responseData = li1.Rows.OfType<DataRow>()
               .Select(row => li1.Columns.OfType<DataColumn>()
                   .ToDictionary(col => col.ColumnName, c => row[c] ));

                    validateJsonData = new { response = true, responseCode = "00", data = responseData };
                    return new JsonResult(validateJsonData);
                }
                else
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "No Record Found." };
                    return new JsonResult(validateJsonData);
                }
            }
            catch(Exception ex)
            {
                string Activity = "Api getgbgperm: " + ex.ToString() + " ";
                validateJsonData = new { response = false, responseCode = "01", data = "Invalid Request." };
                CompanyInfo.InsertErrorLogTracker(Activity.ToString(), 0, 0, 0, 0, "getgbgperm", Convert.ToInt32(obj1.Branch_ID), Convert.ToInt32(obj1.Client_ID), "", context);
                return new JsonResult(validateJsonData);
            }
            //List<Dictionary<string, object>> trasaction1 = new List<Dictionary<string, object>>();
            
        }

        [HttpPost]
        [Route("getjourneyid")]
        public IActionResult getjourneyid([FromBody] JsonElement obj)
        {
            HttpContext context = HttpContext;
            Model.response.WebResponse response = null;
            string json1 = System.Text.Json.JsonSerializer.Serialize(obj);
            dynamic data = JObject.Parse(json1); var validateJsonData = (dynamic)null;
            var claimsIdentity = User.Identity as ClaimsIdentity;//sanket
            var authHeader = "";
            try { authHeader = HttpContext.Request.Headers["Authorization"].ToString(); } catch (Exception egx) { authHeader = ""; }

            bool auth = AuthController.checkAuth(claimsIdentity, obj, Convert.ToString(authHeader));
            if (!auth)
            {
                var errorResponse = new { response = false, responseCode = "403", data = "Access Forbidden" };
                return new JsonResult(errorResponse) { StatusCode = (int)HttpStatusCode.Forbidden };
            }
            CompanyInfo.InsertrequestLogTracker("getjourneyid full request body: " + JObject.Parse(json1), 0, 0, 0, 0, "getjourneyid", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
            Document obj1 = new Document();
            try
            {
                if (!SqlInjectionProtector.ReadJsonElementValues(obj) || !SqlInjectionProtector.ReadJsonElementValuesScript(obj))
                {
                    var errorResponse = new { response = false, responseCode = "02", data = "Invalid input detected." };
                    return new JsonResult(errorResponse) { StatusCode = 400 };
                }

                obj1.Customer_ID = data.customerID;
                obj1.uniqueId = data.uniqueID;

                if (!SqlInjectionProtector.ValidateObjectForSqlInjection(obj) || !SqlInjectionProtector.ValidateObjectForScriptSqlInjection(obj))
                {
                    return new JsonResult(validateJsonData) { StatusCode = StatusCodes.Status400BadRequest, Value = "Invalid input detected." };
                }

                int Customer_ID = Convert.ToInt32(CompanyInfo.Decrypt(obj1.Customer_ID, true));

                string Query = "select * from thirdparty_journeystatus_table where journeystatusfinished=0 and api_id=3 " +
                    " and customerid='" + Customer_ID + "' and uniqueid= '" + obj1.uniqueId + "'  order by 1 desc limit 1";
                MySqlCommand cmd5 = new MySqlCommand(Query);
                DataTable dtt = db_connection.ExecuteQueryDataTableProcedure(cmd5);

                string journeyuniqueid = "", journeystatus = "" ;
                if (dtt.Rows.Count > 0)
                {
                    journeyuniqueid = Convert.ToString(dtt.Rows[0]["journeyuniqueid"]);
                    journeystatus = Convert.ToString(dtt.Rows[0]["journeystatus"]);
                }

                var dataList = new Dictionary<string, object>()
                                {
                                    { "journeyuniqueid", journeyuniqueid },
                                    { "journeystatus",journeystatus }                                     
                                };

                validateJsonData = new { response = true, responseCode = "00", data = dataList };
                return new JsonResult(validateJsonData);
            }
            catch (Exception ex)
            {
                string Activity = "Api getjourneyid: " + ex.ToString() + " ";
                validateJsonData = new { response = false, responseCode = "01", data = "Invalid Request." };
                CompanyInfo.InsertErrorLogTracker(Activity.ToString(), 0, 0, 0, 0, "getjourneyid", Convert.ToInt32(0), Convert.ToInt32(0), "", context);
                return new JsonResult(validateJsonData);
            }
        }

        [HttpGet]
        [Route("postsearch")]
        public IActionResult PostSearch()
        {
            var returnJsonData = (dynamic)null;
            int apiStatus_2 = 1, deleteStatus_2 = 1;
            int apiStatus_12 = 1, deleteStatus_12 = 1;
            string whereClause = " API_ID=2 OR API_ID=12 ";
            try
            {
                MySqlCommand cmd_postSearch = new MySqlCommand("active_thirdparti_Addressapi");
                cmd_postSearch.CommandType = CommandType.StoredProcedure;
                cmd_postSearch.Parameters.AddWithValue("_whereclause", whereClause);
                DataTable dtApi_postSearch = db_connection.ExecuteQueryDataTableProcedure(cmd_postSearch);
                foreach (DataRow row in dtApi_postSearch.Rows)
                {
                    int apiId = Convert.ToInt32(row["API_ID"]);

                    if (apiId == 2)
                    {
                        apiStatus_2 = Convert.ToInt32(row["API_Status"]);
                        deleteStatus_2 = Convert.ToInt32(row["Delete_Status"]);
                        break;
                    }
                    else if (apiId == 12)
                    {
                        apiStatus_12 = Convert.ToInt32(row["API_Status"]);
                        deleteStatus_12 = Convert.ToInt32(row["Delete_Status"]);
                        break;
                    }
                }

                if (apiStatus_2 == 0 && deleteStatus_2 == 0 || apiStatus_12 == 0 && deleteStatus_12 == 0)
                {
                    returnJsonData = new { response = true, responseCode = "00", data = "T" };
                }
                else
                {
                    returnJsonData = new { response = true, responseCode = "00", data = "F" };
                }
            }
            catch (Exception ex)
            {
                returnJsonData = new { response = true, responseCode = "00", data = "F" };
            }

            return new JsonResult(returnJsonData);
        }


        [HttpPost]
        [Authorize]
        [Route("startonboard")]
        public IActionResult StartOnBoard([FromBody] JsonElement obj)
        {
            HttpContext context = HttpContext;
            Model.response.WebResponse response = null;
            string json1 = System.Text.Json.JsonSerializer.Serialize(obj);
            dynamic data = JObject.Parse(json1);
            var claimsIdentity = User.Identity as ClaimsIdentity;//sanket
            var authHeader = "";
            try { authHeader = HttpContext.Request.Headers["Authorization"].ToString(); } catch (Exception egx) { authHeader = ""; }

            bool auth = AuthController.checkAuth(claimsIdentity, obj, Convert.ToString(authHeader));
            if (!auth)
            {
                var errorResponse = new { response = false, responseCode = "403", data = "Access Forbidden" };
                return new JsonResult(errorResponse) { StatusCode = (int)HttpStatusCode.Forbidden };
            }
            Document obj1 = new Document();
            var validateJsonData = (dynamic)null;
            CompanyInfo.InsertrequestLogTracker("startonboard full request body: " + JObject.Parse(json1), 0, 0, 0, 0, "StartOnBoard", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);

            try
            {
                if (!SqlInjectionProtector.ReadJsonElementValues(obj) || !SqlInjectionProtector.ReadJsonElementValuesScript(obj))
                {
                    var errorResponse = new { response = false, responseCode = "02", data = "Invalid input detected." };
                    return new JsonResult(errorResponse) { StatusCode = 400 };
                }

                if (data.clientID == "" || data.clientID == null)
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "Invalid Client ID." };
                    return new JsonResult(validateJsonData);
                }
                if (data.branchID == "" || data.branchID == null)
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "Invalid Branch ID." };
                    return new JsonResult(validateJsonData);
                }
                if (data.customerID == "" || data.customerID == null)
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "Invalid Customer ID." };
                    return new JsonResult(validateJsonData);
                }

                obj1.Client_ID = data.clientID;
                obj1.Branch_ID = data.branchID;
                obj1.Customer_ID = data.customerID;
                var host = $"{context.Request.Scheme}://{context.Request.Host}";
                obj1.Path = host;

                try
                {
                    obj1.Custmer_Ref = data.Custmer_Ref;
                    obj1.post_code = data.post_code;
                    obj1.Adderess = data.Adderess;
                    obj1.strete = data.strete;
                    obj1.city = data.city;
                    obj1.country_code = data.country_code;
                    obj1.customer_email = data.customer_email;
                    obj1.customer_mobile = data.customer_mobile;
                }
                catch (Exception ex) { 
                    obj1.Custmer_Ref = obj1.post_code = obj1.Adderess = obj1.strete = obj1.city = obj1.country_code = obj1.customer_email = obj1.customer_mobile = ""; 
                }

                


                Service.srvDocument srv = new Service.srvDocument();
                DataTable li1 = srv.GetToken(obj1, context);
                Dictionary<string, string> jsonDictionary = new Dictionary<string, string>();
                if (li1 != null && li1.Rows.Count > 0)
                {
                    
                    string json = CompanyInfo.DataTableToJSONWithNewtonsoftJson(li1).Replace(@"\", " ");
                    jsonDictionary.Add("GetToken", json);

                    List<Dictionary<string, object>> gbgTokenresponse = new List<Dictionary<string, object>>();

                    foreach (DataRow dr in li1.Rows)
                    {
                        Dictionary<string, object> transaction_history = new Dictionary<string, object>();
                        transaction_history["status"] = dr["Status"];
                        transaction_history["redirectURL"] = dr["RedirectURL"];

                        try
                        {
                            transaction_history["journeyid"] = dr["journeyid"];
                            transaction_history["redirectgbgurl"] = dr["redirecturl"];
                            transaction_history["qrcode"] = dr["qrcode"]; 
                            transaction_history["gbgtoken"] = dr["gbgtoken"];
                            transaction_history["uniquevalue"] = dr["uniquevalue"];
                            transaction_history["redirectsuccessurl"] = dr["redirectsuccessurl"];
                        }
                        catch (Exception ex)
                        {
                        }

                        gbgTokenresponse.Add(transaction_history);
                    }

                    validateJsonData = new { response = true, responseCode = "00", data = gbgTokenresponse };
                    return new JsonResult(validateJsonData);
                }
                else
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "No Record Found." };
                    return new JsonResult(validateJsonData);
                }
                
            }
            catch (Exception ex) 
            {
                string Activity = "Api StartOnBoard: " + ex.ToString() + " ";
                validateJsonData = new { response = false, responseCode = "01", data = "Invalid Request." };
                CompanyInfo.InsertErrorLogTracker(Activity.ToString(), 0, 0, 0, 0, "StartOnBoard", Convert.ToInt32(obj1.Branch_ID), Convert.ToInt32(obj1.Client_ID), "", context);
                return new JsonResult(validateJsonData);
            }
        }

        [HttpPost]
        //Authorize]
        [Route("idscandocument")]
        public IActionResult IDScanDocument([FromBody] JsonElement obj)
        {
            HttpContext context = HttpContext;
            Model.response.WebResponse response = null;
            string json = System.Text.Json.JsonSerializer.Serialize(obj);
            var validateJsonData = (dynamic)null;
            _ = Task.Run(()=>CompanyInfo.InsertrequestLogTracker("IDScanDocument full request body: " + JObject.Parse(json), 0, 0, 0, 0, "IDScanDocument", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext));

            try
            {
                if (!SqlInjectionProtector.ReadJsonElementValues(obj) || !SqlInjectionProtector.ReadJsonElementValuesScript(obj))
                {
                    var errorResponse = new { response = false, responseCode = "02", data = "Invalid input detected." };
                    return new JsonResult(errorResponse) { StatusCode = 400 };
                }
                Document Obj = JsonConvert.DeserializeObject<Document>(json);
                Service.srvDocument srv = new Service.srvDocument();
                DataTable li1 = srv.QRGetIDScanToken(Obj, context);
                var idscanDoc = li1.Rows.OfType<DataRow>()
                .Select(row => li1.Columns.OfType<DataColumn>()
                .ToDictionary(col => col.ColumnName, c => row[c].ToString()));
                validateJsonData = new { response = true, responseCode = "00", data = idscanDoc };
            }
            catch (Exception ex)
            {
                //Error Log Handled
                Model.ErrorLog objError = new Model.ErrorLog();
                objError.User = new Model.User();
                objError.Error = "Api : Get Token --" + ex.ToString();
                objError.Date = DateTime.Now;
                objError.User_ID = 1;
                objError.Client_ID = 1;
                objError.Function_Name = "IDScanDocument";
                Service.srvErrorLog srvError = new Service.srvErrorLog();
                srvError.Create(objError, context);
                validateJsonData = new { response = false, responseCode = "02", data = "Invalid Request." };
            }
            return new JsonResult(validateJsonData);
        }

        [HttpPost]
        //Authorize]
        [Route("chkcustidscanparams")]
        public IActionResult ChkCustIDScanParams([FromBody] JsonElement obj)
        {
            HttpContext context = HttpContext;
            Model.response.WebResponse response = null;
            string json = System.Text.Json.JsonSerializer.Serialize(obj);
            var validateJsonData = (dynamic)null;
            _ = Task.Run(() => CompanyInfo.InsertrequestLogTracker("ChkCustIDScanParams full request body: " + JObject.Parse(json), 0, 0, 0, 0, "ChkCustIDScanParams", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext));

            try
            {
                if (!SqlInjectionProtector.ReadJsonElementValues(obj) || !SqlInjectionProtector.ReadJsonElementValuesScript(obj))
                {
                    var errorResponse = new { response = false, responseCode = "02", data = "Invalid input detected." };
                    return new JsonResult(errorResponse) { StatusCode = 400 };
                }
                Document Obj = JsonConvert.DeserializeObject<Document>(json);
                string check_exp = "";
                if (Obj.checkparam != null && Obj.checkparam != "" && Obj.checkparam != "0")
                {
                    string checktime = Convert.ToString(Obj.checkparam);

                    string check_param = CompanyInfo.Decrypt(checktime, Convert.ToBoolean(1));

                    DateTime dtchktime = Convert.ToDateTime(check_param);
                    DateTime dtnowdate = DateTime.Now;

                    if (dtchktime >= dtnowdate)
                    {
                        check_exp = "Not expired";
                        validateJsonData = new { response = true, responseCode = "00", data = "Not expired" };
                    }
                    else
                    {
                        check_exp = "expired";
                        validateJsonData = new { response = true, responseCode = "00", data = "expired" };
                    }
                }
                else
                {
                    check_exp = "Not expired";
                    validateJsonData = new { response = true, responseCode = "00", data = "Not expired" };
                }
                if (check_exp != "expired")
                {

                    Service.srvDocument srv = new Service.srvDocument();
                    DataTable li1 = srv.ChkCustIDParams(Obj, context);
                    var idscanDoc = new List<Dictionary<string, object>>();

                    if (li1 != null && li1.Columns.Count > 0 && li1.Rows.Count > 0)
                    {
                        for (int i = 0; i < li1.Rows.Count; i++)
                        {
                            var rowDict = new Dictionary<string, object>();
                            for (int j = 0; j < li1.Columns.Count; j++)
                            {
                                string columnName = li1.Columns[j].ColumnName;
                                object value = li1.Rows[i][j];

                                rowDict[columnName] = value == DBNull.Value ? null : value;
                            }

                            idscanDoc.Add(rowDict);
                        }
                    }
                    validateJsonData = new { response = true, responseCode = "00", data = idscanDoc };
                }
                else
                {
                    validateJsonData = new { response = true, responseCode = "00", data = "expired" };
                }
            }
            catch (Exception ex)
            {
                //Error Log Handled
                Model.ErrorLog objError = new Model.ErrorLog();
                objError.User = new Model.User();
                objError.Error = "Api : Get Token --" + ex.ToString();
                objError.Date = DateTime.Now;
                objError.User_ID = 1;
                objError.Client_ID = 1;
                objError.Function_Name = "ChkCustIDScanParams";
                Service.srvErrorLog srvError = new Service.srvErrorLog();
                srvError.Create(objError, context);

                validateJsonData = new { response = false, responseCode = "02", data = "Invalid Request" };
            }
            return new JsonResult(validateJsonData);
        }

    }
}
