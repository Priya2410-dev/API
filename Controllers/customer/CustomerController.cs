using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.Json;
using System.Data;
using Calyx_Solutions.Model;
using System.Net;
using MySqlConnector;
using Calyx_Solutions.Service;
using Microsoft.Extensions.Configuration;
using static iTextSharp.text.pdf.codec.TiffWriter;
using System.Web.Helpers;
using System;
using Borland.Vcl;
using System.Collections;
using System.Numerics;
using System.Text.Json.Nodes;
using System.Diagnostics.Metrics;
using static Calyx_Solutions.mtsIntegrationmethods;
using Microsoft.Ajax.Utilities;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Calyx_Solutions.Controllers.customer
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        [HttpPost]
        [Route("addcustomeremail")]
        public JsonResult add_customer_email([FromBody] JsonElement objdata)
        {
            var returnJsonData = (dynamic)null;
            Model.response.WebResponse response = null;
            string jsonData = System.Text.Json.JsonSerializer.Serialize(objdata);
            dynamic data = JObject.Parse(jsonData);
            CompanyInfo.InsertrequestLogTracker("addcustomeremail full request body: " + JObject.Parse(jsonData), 0, 0, 0, 0, "add_customer_email", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);

            List<Model.Customer> _lst = new List<Model.Customer>();
            Transaction obj  = new Transaction();
            try
            {
                if (!SqlInjectionProtector.ReadJsonElementValues(objdata) || !SqlInjectionProtector.ReadJsonElementValuesScript(objdata))
                {
                    var errorResponse = new { response = false, responseCode = "02", data = "Invalid input detected." };
                    return new JsonResult(errorResponse) { StatusCode = 400 };
                }

                obj.Client_ID = data.Client_ID;
                obj.Customer_ID = data.Customer_ID;
                obj.CustomerEmail = data.CustomerEmail;

                if( obj.CustomerEmail  == null || obj.CustomerEmail == "")
                {
                    returnJsonData = new { response = false, responseCode = "02", data = "Invalid email address." };
                    return new JsonResult(returnJsonData);
                }

                Service.srvWallet srv = new Service.srvWallet();
                DataTable li1 = srv.add_customer_email(obj);
                //Model.Dashboard _Obj = new Model.Dashboard();
                response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_SUCCESS);
                response.data = "success";
                response.ObjData = li1;
                response.ResponseCode = 0;
                
                var returndata = li1.Rows.OfType<DataRow>()
                    .Select(row => li1.Columns.OfType<DataColumn>()
                        .ToDictionary(col => col.ColumnName, c => row[c]));

                returnJsonData = new { response = true, responseCode = "00", data = returndata };
                return new JsonResult(returnJsonData);
            }
            catch (Exception ex)
            {
                response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_TECHNICAL_ERROR);
                response.sErrorExceptionText = ex.ToString();
                response.ResponseMessage = "Technical error";
                response.ResponseCode = 3;
                //Error Log Handled
                Model.ErrorLog objError = new Model.ErrorLog();
                objError.User = new Model.User();
                objError.Error = "Api : add_customer_email --" + ex.ToString();
                objError.Date = DateTime.Now;
                objError.User_ID = 1;
                objError.Client_ID = data.Client_ID;
                objError.Function_Name = "add_customer_email";
                CompanyInfo.InsertErrorLogTracker(" Exception add_customer_email Error: " + ex.ToString(), 0, 0, 0, 0, "add_customer_email", Convert.ToInt32(data.Branch_ID), Convert.ToInt32(data.Client_ID), "add_customer_email", HttpContext);
                Service.srvErrorLog srvError = new Service.srvErrorLog();
                srvError.Create(objError, HttpContext );                
                returnJsonData = new { response = false, responseCode = "01", data = "Invalid request." };
                return new JsonResult(returnJsonData);                
            }

        }


        [HttpPost]
        [Route("deletecustomeremail")]
        public JsonResult delete_customer_email([FromBody] JsonElement objdata)
        {
            var returnJsonData = (dynamic)null;
            Model.response.WebResponse response = null;
            string jsonData = System.Text.Json.JsonSerializer.Serialize(objdata);
            dynamic data = JObject.Parse(jsonData);
            CompanyInfo.InsertrequestLogTracker("deletecustomeremail full request body: " + JObject.Parse(jsonData), 0, 0, 0, 0, "delete_customer_email", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
            Transaction obj = new Transaction();
            try
            {
                if (!SqlInjectionProtector.ReadJsonElementValues(objdata) || !SqlInjectionProtector.ReadJsonElementValuesScript(objdata))
                {
                    var errorResponse = new { response = false, responseCode = "02", data = "Invalid input detected." };
                    return new JsonResult(errorResponse) { StatusCode = 400 };
                }

                Service.srvWallet srv = new Service.srvWallet();
                obj.Client_ID = data.Client_ID;
                obj.Customer_ID = data.Customer_ID;
                obj.CustomerEmail = data.CustomerEmail;

                DataTable li1 = srv.delete_customer_email(obj);

                //Model.Dashboard _Obj = new Model.Dashboard();
                response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_SUCCESS);
                response.data = "success";
                response.ObjData = li1;
                response.ResponseCode = 0;

                var returndata = li1.Rows.OfType<DataRow>()
                    .Select(row => li1.Columns.OfType<DataColumn>()
                        .ToDictionary(col => col.ColumnName, c => row[c]));

                returnJsonData = new { response = true, responseCode = "00", data = returndata };
                return new JsonResult(returnJsonData);

            }
            catch (Exception ex)
            {
                response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_TECHNICAL_ERROR);
                response.sErrorExceptionText = ex.ToString();
                response.ResponseMessage = "Technical error";
                response.ResponseCode = 3;
                //Error Log Handled
                Model.ErrorLog objError = new Model.ErrorLog();
                objError.User = new Model.User();
                objError.Error = "Api : deletecustomeremail --" + ex.ToString();
                objError.Date = DateTime.Now;
                objError.User_ID = 1;
                objError.Client_ID = data.Client_ID;
                objError.Function_Name = "deletecustomeremail";
                CompanyInfo.InsertErrorLogTracker(" Exception add_customer_email Error: " + ex.ToString(), 0, 0, 0, 0, "delete_customer_email", Convert.ToInt32(data.Branch_ID), Convert.ToInt32(data.Client_ID), "delete_customer_email", HttpContext);
                Service.srvErrorLog srvError = new Service.srvErrorLog();
                srvError.Create(objError, HttpContext);
                returnJsonData = new { response = false, responseCode = "01", data = "Invalid request." };
                return new JsonResult(returnJsonData);

            }
        }


            [HttpPost]
        [Route("customerconfiguration")]
        public JsonResult customerconfiguration([FromBody] JsonElement objdata)
        {
            var returnJsonData = (dynamic)null;
            Model.response.WebResponse response = null;
            string jsonData = System.Text.Json.JsonSerializer.Serialize(objdata);
            dynamic data = JObject.Parse(jsonData);
            CompanyInfo.InsertrequestLogTracker("customerconfiguration full request body: " + JObject.Parse(jsonData), 0, 0, 0, 0, "customerconfiguration", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
            #region validateData
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
                _objActivityLog.Activity = " Exception Customer Configuration Error: " + ex.ToString() + " ";
                _objActivityLog.FunctionName = "customerconfiguration";
                _objActivityLog.Transaction_ID = 0;
                _objActivityLog.WhoAcessed = 1;
                _objActivityLog.Branch_ID = 0;
                _objActivityLog.Client_ID = 0;

                Service.srvActivityLogTracker srvActivityLog = new Service.srvActivityLogTracker();
                int _i = srvActivityLog.Create(_objActivityLog, HttpContext);
                CompanyInfo.InsertErrorLogTracker(" Exception Customer Configuration Error: " + ex.ToString(), 0, 0, 0, 0, "customerconfiguration", Convert.ToInt32(data.Branch_ID), Convert.ToInt32(data.Client_ID), "", HttpContext);
                returnJsonData = new { response = false, responseCode = "01", data = "Invalid request." };
                return new JsonResult(returnJsonData);
            }
            #endregion validateData

            try
            {
                Service.srvCustomer srv = new Service.srvCustomer();
                DataTable li1 = srv.dataValidations(Convert.ToInt32(data.clientID), Convert.ToInt32(data.branchID));

                if (li1 != null)
                {
                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_SUCCESS);
                    response.data = "success";
                    response.ObjData = li1;
                    response.ResponseCode = 0;

                    List<Dictionary<string, object>> confiurations = new List<Dictionary<string, object>>();
                    Dictionary<string, object> confiuration = new Dictionary<string, object>();

                    if (li1 != null && li1.Rows.Count > 0)
                    {
                        #region Step_1
                        // Email
                        if (Convert.ToString(li1.Rows[0]["Customer_email"]) == "0")
                        {
                            confiuration["customerEmailVisible"] = true;
                        }
                        else
                        {
                            confiuration["customerEmailVisible"] = false;
                        }

                        if (Convert.ToString(li1.Rows[0]["Customer_email_man"]) == "0")
                        {
                            confiuration["customerEmailMandetory"] = true;
                        }
                        else
                        {
                            confiuration["customerEmailMandetory"] = false;
                        }

                        // Confirm Email
                        if (Convert.ToString(li1.Rows[0]["Customer_con_email"]) == "0")
                        {
                            confiuration["customerConfirmEmailVisible"] = true;
                        }
                        else
                        {
                            confiuration["customerConfirmEmailVisible"] = false;
                        }

                        if (Convert.ToString(li1.Rows[0]["Customer_con_email_man"]) == "0")
                        {
                            confiuration["customerConfirmEmailMandetory"] = true;
                        }
                        else
                        {
                            confiuration["customerConfirmEmailMandetory"] = false;
                        }

                        // Password
                        if (Convert.ToString(li1.Rows[0]["Customer_password"]) == "0")
                        {
                            confiuration["customerPasswordVisible"] = true;
                        }
                        else
                        {
                            confiuration["customerPasswordVisible"] = false;
                        }

                        if (Convert.ToString(li1.Rows[0]["Customer_password_man"]) == "0")
                        {
                            confiuration["customerPasswordMandetory"] = true;
                        }
                        else
                        {
                            confiuration["customerPasswordMandetory"] = false;
                        }

                        // Confirm Password
                        if (Convert.ToString(li1.Rows[0]["Customer_con_password"]) == "0")
                        {
                            confiuration["customerConfirmpasswordVisible"] = true;
                        }
                        else
                        {
                            confiuration["customerConfirmpasswordVisible"] = false;
                        }

                        if (Convert.ToString(li1.Rows[0]["Customer_con_password_man"]) == "0")
                        {
                            confiuration["customerConfirmpasswordMandetory"] = true;
                        }
                        else
                        {
                            confiuration["customerConfirmpasswordMandetory"] = false;
                        }
                        #endregion Step_1

                        #region Step_2
                        // Customer Title
                        if (Convert.ToString(li1.Rows[0]["Customer_title"]) == "0")
                        {
                            confiuration["customerTitleVisible"] = true;
                        }
                        else
                        {
                            confiuration["customerTitleVisible"] = false;
                        }

                        if (Convert.ToString(li1.Rows[0]["Customer_title_man"]) == "0")
                        {
                            confiuration["customerTitleMandetory"] = true;
                        }
                        else
                        {
                            confiuration["customerTitleMandetory"] = false;
                        }

                        try
                        {
                            if (Convert.ToString(li1.Rows[0]["Set_default_title"]) == "0")
                            {
                                confiuration["customerDefaultTitle"] = true;
                            }
                            else
                            {
                                confiuration["customerDefaultTitle"] = false;
                            }
                        }
                        catch(Exception egx) { confiuration["customerDefaultTitle"] = false; }


                        // Gender
                        if (Convert.ToString(li1.Rows[0]["Gender_show"]) == "0")
                        {
                            confiuration["customerGenderVisible"] = true;
                        }
                        else
                        {
                            confiuration["customerGenderVisible"] = false;
                        }

                        if (Convert.ToString(li1.Rows[0]["Gender_man"]) == "0")
                        {
                            confiuration["customerGenderMandetory"] = true;
                        }
                        else
                        {
                            confiuration["customerGenderMandetory"] = false;
                        }

                        // Customer First Name
                        if (Convert.ToString(li1.Rows[0]["First_name"]) == "0")
                        {
                            confiuration["customerFirstNameVisible"] = true;
                        }
                        else
                        {
                            confiuration["customerFirstNameVisible"] = false;
                        }

                        if (Convert.ToString(li1.Rows[0]["First_name_man"]) == "0")
                        {
                            confiuration["customerFirstNameMandetory"] = true;
                        }
                        else
                        {
                            confiuration["customerFirstNameMandetory"] = false;
                        }

                        // Customer Middle Name
                        if (Convert.ToString(li1.Rows[0]["Middle_name"]) == "0")
                        {
                            confiuration["customerMiddleNameVisible"] = true;
                        }
                        else
                        {
                            confiuration["customerMiddleNameVisible"] = false;
                        }

                        if (Convert.ToString(li1.Rows[0]["Middle_name_man"]) == "0")
                        {
                            confiuration["customerMiddleNameMandetory"] = true;
                        }
                        else
                        {
                            confiuration["customerMiddleNameMandetory"] = false;
                        }

                        // Customer Last Name
                        if (Convert.ToString(li1.Rows[0]["Last_name"]) == "0")
                        {
                            confiuration["customerLastNameVisible"] = true;
                        }
                        else
                        {
                            confiuration["customerLastNameVisible"] = false;
                        }

                        if (Convert.ToString(li1.Rows[0]["Last_name_man"]) == "0")
                        {
                            confiuration["customerLastNameMandetory"] = true;
                        }
                        else
                        {
                            confiuration["customerLastNameMandetory"] = false;
                        }

                        // Date Of Birth
                        if (Convert.ToString(li1.Rows[0]["Date_of_Birth"]) == "0")
                        {
                            confiuration["customerDOBVisible"] = true;
                        }
                        else
                        {
                            confiuration["customerDOBVisible"] = false;
                        }

                        if (Convert.ToString(li1.Rows[0]["Date_of_Birth_man"]) == "0")
                        {
                            confiuration["customerDOBMandetory"] = true;
                        }
                        else
                        {
                            confiuration["customerDOBMandetory"] = false;
                        }

                        if (Convert.ToString(li1.Rows[0]["Set_default_dob"]) == "0")
                        {
                            confiuration["customerDOBDefaultValue"] = true;
                        }
                        else
                        {
                            confiuration["customerDOBDefaultValue"] = false;
                        }

                        // Mobile Number
                        if (Convert.ToString(li1.Rows[0]["Customer_Mobile_number"]) == "0")
                        {
                            confiuration["customerMobNumberVisible"] = true;
                        }
                        else
                        {
                            confiuration["customerMobNumberVisible"] = false;
                        }

                        if (Convert.ToString(li1.Rows[0]["Customer_Mobile_number_man"]) == "0")
                        {
                            confiuration["customerMobNumberMandetory"] = true;
                        }
                        else
                        {
                            confiuration["customerMobNumberMandetory"] = false;
                        }

                        // Phone Number
                        if (Convert.ToString(li1.Rows[0]["customer_phone_number"]) == "0")
                        {
                            confiuration["customerPhoneVisible"] = true;
                        }
                        else
                        {
                            confiuration["customerPhoneVisible"] = false;
                        }

                        if (Convert.ToString(li1.Rows[0]["customer_phone_man"]) == "0")
                        {
                            confiuration["customerPhoneMandetory"] = true;
                        }
                        else
                        {
                            confiuration["customerPhoneMandetory"] = false;
                        }

                        #endregion Step_2

                        #region Step_3
                        // Post Code
                        if (Convert.ToString(li1.Rows[0]["Customer_post_code"]) == "0")
                        {
                            confiuration["customerPostcodeVisible"] = true;
                        }
                        else
                        {
                            confiuration["customerPostcodeVisible"] = false;
                        }

                        if (Convert.ToString(li1.Rows[0]["Customer_post_code_man"]) == "0")
                        {
                            confiuration["customerPostcodeMandetory"] = true;
                        }
                        else
                        {
                            confiuration["customerPostcodeMandetory"] = false;
                        }

                        // House Number
                        if (Convert.ToString(li1.Rows[0]["Customer_house_number"]) == "0")
                        {
                            confiuration["customerHouseNumberVisible"] = true;
                        }
                        else
                        {
                            confiuration["customerHouseNumberVisible"] = false;
                        }

                        if (Convert.ToString(li1.Rows[0]["Customer_house_number_man"]) == "0")
                        {
                            confiuration["customerHouseNumberMandetory"] = true;
                        }
                        else
                        {
                            confiuration["customerHouseNumberMandetory"] = false;
                        }

                        // Address 1
                        if (Convert.ToString(li1.Rows[0]["Customer_address1"]) == "0")
                        {
                            confiuration["customerAddress1Visible"] = true;
                        }
                        else
                        {
                            confiuration["customerAddress1Visible"] = false;
                        }

                        if (Convert.ToString(li1.Rows[0]["Customer_address1_man"]) == "0")
                        {
                            confiuration["customerAddress1Mandetory"] = true;
                        }
                        else
                        {
                            confiuration["customerAddress1Mandetory"] = false;
                        }

                        // Address 2 
                        if (Convert.ToString(li1.Rows[0]["Customer_address2"]) == "0")
                        {
                            confiuration["customerAddress2Visible"] = true;
                        }
                        else
                        {
                            confiuration["customerAddress2Visible"] = false;
                        }

                        if (Convert.ToString(li1.Rows[0]["Customer_address2_man"]) == "0")
                        {
                            confiuration["customerAddress2Mandetory"] = true;
                        }
                        else
                        {
                            confiuration["customerAddress2Mandetory"] = false;
                        }

                        // Country 
                        if (Convert.ToString(li1.Rows[0]["Customer_country"]) == "0")
                        {
                            confiuration["customerCountryVisible"] = true;
                        }
                        else
                        {
                            confiuration["customerCountryVisible"] = false;
                        }

                        if (Convert.ToString(li1.Rows[0]["Customer_country_man"]) == "0")
                        {
                            confiuration["customerCountryMandetory"] = true;
                        }
                        else
                        {
                            confiuration["customerCountryMandetory"] = false;
                        }

                        // Provience/State
                        if (Convert.ToString(li1.Rows[0]["provience_man"]) == "0")
                        {
                            confiuration["customerProvienceVisible"] = true;
                            confiuration["customerProvienceMandetory"] = true;
                        }
                        else if (Convert.ToString(li1.Rows[0]["provience_man"]) == "1")
                        {
                            confiuration["customerProvienceVisible"] = true;
                            confiuration["customerProvienceMandetory"] = false;
                        }
                        else
                        {
                            confiuration["customerProvienceVisible"] = false;
                            confiuration["customerProvienceMandetory"] = false;
                        }

                        // City
                        if (Convert.ToString(li1.Rows[0]["Customer_city"]) == "0")
                        {
                            confiuration["customerCityVisible"] = true;
                        }
                        else
                        {
                            confiuration["customerCityVisible"] = false;
                        }

                        if (Convert.ToString(li1.Rows[0]["Customer_city_man"]) == "0")
                        {
                            confiuration["customerCityMandetory"] = true;
                        }
                        else
                        {
                            confiuration["customerCityMandetory"] = false;
                        }

                        // Birthplace
                        try
                        {
                            if (Convert.ToString(li1.Rows[0]["Customer_birthplace"]) == "0")
                            {
                                confiuration["customerBirthPlaceVisible"] = true;
                                confiuration["customerBirthPlaceMandetory"] = true;
                            }
                            else if (Convert.ToString(li1.Rows[0]["Customer_birthplace"]) == "1")
                            {
                                confiuration["customerBirthPlaceVisible"] = true;
                                confiuration["customerBirthPlaceMandetory"] = false;
                            }
                            else
                            {
                                confiuration["customerBirthPlaceVisible"] = false;
                                confiuration["customerBirthPlaceMandetory"] = false;
                            }
                        }
                        catch {
                            confiuration["customerBirthPlaceVisible"] = false;
                            confiuration["customerBirthPlaceMandetory"] = false;
                        }

                        try
                        {
                            // Country of Birth
                            if (Convert.ToString(li1.Rows[0]["Countryof_Birth"]) == "0")
                        {
                            confiuration["countryOfBirthVisible"] = true;
                            confiuration["countryOfBirthMandetory"] = true;

                        }
                        else if (Convert.ToString(li1.Rows[0]["Countryof_Birth"]) == "1")
                        {
                            confiuration["countryOfBirthVisible"] = true;
                            confiuration["countryOfBirthMandetory"] = false;
                        }
                        else if (Convert.ToString(li1.Rows[0]["Countryof_Birth"]) == "2")
                        {
                            confiuration["countryOfBirthVisible"] = false;
                            confiuration["countryOfBirthMandetory"] = false;
                        }
                        }
                        catch
                        {
                            confiuration["countryOfBirthVisible"] = false;
                            confiuration["countryOfBirthMandetory"] = false;
                        }

                        try
                        {
                            //Payout Countries
                            if (Convert.ToString(li1.Rows[0]["Payout_Countries"]) == "0")
                            {
                                confiuration["payoutCountriesVisible"] = true;
                            }
                            else
                            {
                                confiuration["payoutCountriesVisible"] = false;
                            }
                            if (Convert.ToString(li1.Rows[0]["Payout_Countries_man"]) == "0")
                            {
                                confiuration["payoutCountriesMandetory"] = true;
                            }
                            else
                            {
                                confiuration["payoutCountriesMandetory"] = false;
                            }
                        }
                        catch
                        {
                            confiuration["payoutCountriesMandetory"] = false;
                            confiuration["payoutCountriesVisible"] = false;
                        }

                        // Nationality
                        if (Convert.ToString(li1.Rows[0]["Customer_nationality"]) == "0")
                        {
                            confiuration["customerNationalityVisible"] = true;
                        }
                        else
                        {
                            confiuration["customerNationalityVisible"] = false;
                        }

                        if (Convert.ToString(li1.Rows[0]["Customer_nationality_man"]) == "0")
                        {
                            confiuration["customerNationalityMandetory"] = true;
                        }
                        else
                        {
                            confiuration["customerNationalityMandetory"] = false;
                        }

                        try
                        {
                            if (Convert.ToString(li1.Rows[0]["Set_default_nationality"]) == "0")
                            {
                                confiuration["customerNationalityDefaultValueAvailable"] = true;
                                confiuration["customerNationalityDefaultValue"] = li1.Rows[0]["Selected_default_nationality"];
                            }
                            else
                            {
                                confiuration["customerNationalityDefaultValueAvailable"] = false;
                            }
                        }
                        catch(Exception eg) { confiuration["customerNationalityDefaultValueAvailable"] = false; }

                        //  Employement Status
                        if (Convert.ToString(li1.Rows[0]["Employement_status"]) == "0")
                        {
                            confiuration["customerEmployementStatusVisible"] = true;
                        }
                        else
                        {
                            confiuration["customerEmployementStatusVisible"] = false;
                        }

                        if (Convert.ToString(li1.Rows[0]["Employement_status_man"]) == "0")
                        {
                            confiuration["customerEmployementStatusMandetory"] = true;
                        }
                        else
                        {
                            confiuration["customerEmployementStatusMandetory"] = false;
                        }

                        // Profession 
                        if (Convert.ToString(li1.Rows[0]["Customer_profession"]) == "0")
                        {
                            confiuration["customerProfessionVisible"] = true;
                        }
                        else
                        {
                            confiuration["customerProfessionVisible"] = false;
                        }

                        if (Convert.ToString(li1.Rows[0]["Customer_profession_man"]) == "0")
                        {
                            confiuration["customerProfessionMandetory"] = true;
                        }
                        else
                        {
                            confiuration["customerProfessionMandetory"] = false;
                        }

                        // Company Name
                        if (Convert.ToString(li1.Rows[0]["Company_name"]) == "0")
                        {
                            confiuration["customerCompanyNameVisible"] = true;
                        }
                        else
                        {
                            confiuration["customerCompanyNameVisible"] = false;
                        }

                        if (Convert.ToString(li1.Rows[0]["Company_name_man"]) == "0")
                        {
                            confiuration["customerCompanyNameMandetory"] = true;
                        }
                        else
                        {
                            confiuration["customerCompanyNameMandetory"] = false;
                        }

                        // Where Did You Hear About Us?
                        if (Convert.ToString(li1.Rows[0]["Heard_from"]) == "0")
                        {
                            confiuration["customerHeardfromVisible"] = true;
                        }
                        else
                        {
                            confiuration["customerHeardfromVisible"] = false;
                        }

                        if (Convert.ToString(li1.Rows[0]["Heard_from_man"]) == "0")
                        {
                            confiuration["customerHeardfromMandetory"] = true;
                        }
                        else
                        {
                            confiuration["customerHeardfromMandetory"] = false;
                        }

                        // Terms and Conditions 
                        if (Convert.ToString(li1.Rows[0]["Terms"]) == "0")
                        {
                            confiuration["customerTermsMandetory"] = true;
                        }
                        else
                        {
                            confiuration["customerTermsMandetory"] = false;
                        }

                        // Promotional Email Checkbox
                        if (Convert.ToString(li1.Rows[0]["Promo_mail"]) == "0")
                        {
                            confiuration["customerPromoMail"] = true;
                        }
                        else
                        {
                            confiuration["customerPromoMail"] = false;
                        }

                        try
                        {
                            // Annual Salary
                            if (Convert.ToString(li1.Rows[0]["Annual_Salary"]) == "0")
                            {
                                confiuration["annualSalaryVisible"] = true;
                            }
                            else
                            {
                                confiuration["annualSalaryVisible"] = false;
                            }
                            if (Convert.ToString(li1.Rows[0]["Annual_Salary_man"]) == "0")
                            {
                                confiuration["annualSalaryMandetory"] = true;
                            }
                            else
                            {
                                confiuration["annualSalaryMandetory"] = false;
                            }
                        }
                        catch (Exception egx) {
                            confiuration["annualSalaryVisible"] = false;
                            confiuration["annualSalaryMandetory"] = false;
                        }

                        #endregion Step_3
                    }
                    int countryIDForMobileValidate = 0;
                    try
                    {
                        countryIDForMobileValidate = Convert.ToInt32(data.countryID);
                    }
                    catch (Exception eg) { }
                    if(countryIDForMobileValidate > 0 || true )
                    {
                        MySqlConnector.MySqlCommand cmd = new MySqlConnector.MySqlCommand("Country_Search");
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("_Client_ID", Convert.ToInt32(data.clientID));
                        cmd.Parameters.AddWithValue("_whereclause", " and Country_ID=" + countryIDForMobileValidate + ""); // Get ISO Aplha 2 letter code
                        DataTable dtc = db_connection.ExecuteQueryDataTableProcedure(cmd);
                        if (dtc.Rows.Count > 0)
                        {                            
                            if (Convert.ToString(dtc.Rows[0]["chkpostcode"]) == "0")
                            {
                                confiuration["searchpostcodeVisible"] = true;
                            }
                            else
                            {
                                confiuration["searchpostcodeVisible"] = false;
                            }

                            try { 
                            if (Convert.ToString(dtc.Rows[0]["chkmobile"])  != "")
                            {
                                confiuration["mobileInitials"] = Convert.ToString(dtc.Rows[0]["chkmobile"]) ;
                                confiuration["mobileinitialactive"] = true;
                            }
                            else
                            {
                                confiuration["mobileInitials"] = "";
                                confiuration["mobileinitialactive"] = false;
                            }
                            }
                            catch (Exception ex) { }
                            }

                        MySqlCommand _cmd = new MySqlCommand("get_Password_Permission");
                        _cmd.CommandType = CommandType.StoredProcedure;
                        _cmd.Parameters.AddWithValue("_Client_ID", Convert.ToInt32(data.clientID));
                        DataTable dt = db_connection.ExecuteQueryDataTableProcedure(_cmd);
                        try
                        {
                            if (dt != null && dt.Rows.Count > 0)
                            {
                                foreach (DataRow row in dt.Rows)
                                {
                                    if (row["Uppercase"] != DBNull.Value)
                                    {
                                        confiuration["Passworduppercase"] = Convert.ToInt32(row["Uppercase"]);
                                    }
                                    if (row["Lowercase"] != DBNull.Value)
                                    {
                                        confiuration["Passwordlowercase"] = Convert.ToDecimal(row["Lowercase"]);
                                    }
                                    if (row["Digit"] != DBNull.Value)
                                    {
                                        confiuration["PasswordDigit"] = Convert.ToDecimal(row["Digit"]);
                                    }

                                    if (row["Isspecial_Char"] != DBNull.Value)
                                    {
                                        confiuration["Passwordisspecialcharacter"] = Convert.ToInt32(row["Isspecial_Char"]);
                                    }
                                    if (row["Special_char"] != DBNull.Value)
                                    {
                                        confiuration["Passwordspecialcharacter"] = Convert.ToString(row["Special_char"]);
                                    }
                                    if (row["Minpass_length"] != DBNull.Value)
                                    {
                                        confiuration["Passwordminlength"] = Convert.ToInt32(row["Minpass_length"]);
                                    }
                                    if (row["Maxpass_Length"] != DBNull.Value)
                                    {
                                        confiuration["Passwordmaxlength"] = Convert.ToInt32(row["Maxpass_Length"]);
                                    }
                                }
                            }
                        }
                        catch (Exception ex) { }

                    }



                    confiurations.Add(confiuration);


                    returnJsonData = new { response = true, responseCode = "00", data = confiurations };
                    return new JsonResult(returnJsonData);
                }
                else
                {
                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_TECHNICAL_ERROR);
                    response.ResponseMessage = "Something went wrong. Please try again later.";
                    response.ResponseCode = 1;
                    returnJsonData = new { response = false, responseCode = "02", data = response.ResponseMessage };
                    return new JsonResult(returnJsonData);
                }
            }
            catch (Exception ex)
            {
                Model.ActivityLogTracker _objActivityLog = new Model.ActivityLogTracker();
                _objActivityLog.Activity = " Exception Customer Configuration App Error: " + ex.ToString() + " ";
                _objActivityLog.FunctionName = "customerconfiguration";
                _objActivityLog.Transaction_ID = 0;
                _objActivityLog.WhoAcessed = 1;
                _objActivityLog.Branch_ID = 0;
                _objActivityLog.Client_ID = 0;

                Service.srvActivityLogTracker srvActivityLog = new Service.srvActivityLogTracker();
                int _i = srvActivityLog.Create(_objActivityLog, HttpContext);
                CompanyInfo.InsertErrorLogTracker(" Exception Customer Configuration App Error: " + ex.ToString(), 0, 0, 0, 0, "customerconfiguration", Convert.ToInt32(data.Branch_ID), Convert.ToInt32(data.Client_ID), "", HttpContext);
                returnJsonData = new { response = false, responseCode = "01", data = "Field is missing." };
                return new JsonResult(returnJsonData);
            }

            return returnJsonData;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public bool validateCustomerRegistration(string fieldName, JsonElement objdata)
        {
            bool result = false;
            try
            {
                string jsonData = System.Text.Json.JsonSerializer.Serialize(objdata);
                dynamic data = JObject.Parse(jsonData);

                Customer obj = JsonConvert.DeserializeObject<Customer>(jsonData);
                obj.Client_ID = Convert.ToInt32(data.clientID);

                Service.srvCustomer srv_ = new Service.srvCustomer();
                DataTable li1 = srv_.dataValidations(obj.Client_ID, Convert.ToInt32(data.branchID));
                if (li1 != null && li1.Rows.Count > 0)
                {
                    if (Convert.ToString(li1.Rows[0][fieldName]) == "0")
                    {
                        result = true;
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                CompanyInfo.InsertErrorLogTracker(" Exception Validation Customer App Error: " + ex.ToString(), 0, 0, 0, 0, "customerconfiguration", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
            }
            return result;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public string validateCustomerRegistrationUpdate(string fieldName, JsonElement objdata)
        {
            string result = "";

            try
            {
                string jsonData = System.Text.Json.JsonSerializer.Serialize(objdata);
                dynamic data = JObject.Parse(jsonData);

                Customer obj = JsonConvert.DeserializeObject<Customer>(jsonData);
                obj.Client_ID = Convert.ToInt32(data.clientID);

                Service.srvCustomer srv_ = new Service.srvCustomer();
                DataTable li1 = srv_.dataValidations(obj.Client_ID, Convert.ToInt32(data.branchID));
                if (li1 != null && li1.Rows.Count > 0)
                {
                    if (Convert.ToString(li1.Rows[0][fieldName]) == "0")
                    {
                        #region personaldetails
                        if (fieldName == "Customer_title_man")
                        {
                            result = "Please select title.";
                        }
                        else if (fieldName == "Gender_man")
                        {
                            result = "Gender is required.";
                        }
                        else if (fieldName == "First_name_man")
                        {
                            result = "First name is required.";
                        }
                        else if (fieldName == "Middle_name_man")
                        {
                            result = "Middle name is required.";
                        }
                        else if (fieldName == "Last_name_man")
                        {
                            result = "Last name is required.";
                        }
                        else if (fieldName == "Date_of_Birth_man")
                        {
                            result = "DOB is required.";
                        }
                        else if (fieldName == "Customer_Mobile_number_man")
                        {
                            result = "Mobile number is required.";
                        }
                        else if (fieldName == "customer_phone_man")
                        {
                            result = "Phone is required.";
                        }
                        #endregion personaldetails

                        #region addressdetails

                        else if (fieldName == "Customer_post_code_man")
                        {
                            result = "Post code is required.";
                        }
                        else if (fieldName == "Customer_house_number_man")
                        {
                            result = "Customer house number is required.";
                        }
                        else if (fieldName == "Customer_address1_man")
                        {
                            result = "Address1 is required.";
                        }
                        else if (fieldName == "Customer_address2_man")
                        {
                            result = "Address2 is required.";
                        }
                        else if (fieldName == "Customer_country_man")
                        {
                            result = "Country is required.";
                        }
                        else if (fieldName == "provience_man")
                        {
                            result = "State is required.";
                             
                        }
                        else if (fieldName == "Customer_city_man")
                        {
                            result = "City is required.";
                        }
                        else if (fieldName == "Customer_birthplace")
                        {
                            result = "Birth place is required.";
                        }
                        else if (fieldName == "Countryof_Birth")
                        {
                            result = "Country of Birth is required.";
                        }
                        else if (fieldName == "Employement_status_man")
                        {
                            result = "Employement Status is required.";
                        }
                        else if (fieldName == "Customer_profession_man")
                        {
                            result = "Profession is required.";
                        }
                        else if (fieldName == "Company_name_man")
                        {
                            result = "Company name is required.";
                        }
                        else if (fieldName == "Heard_from_man")
                        {
                            result = "Heard from is required.";
                        }
                        else if (fieldName == "Terms")
                        {
                            result = "Terms and Condition must be required.";
                        }
                        else if (fieldName == "Customer_nationality_man")
                        {
                            result = "Nationality is required.";
                        }

                        #endregion addressdetails


                    }

                }

                return result;
            }
            catch (Exception ex)
            {
                CompanyInfo.InsertErrorLogTracker(" Exception Validation Customer App Error: " + ex.ToString(), 0, 0, 0, 0, "customerconfiguration", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
            }

            return result;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        static bool PasswordContainsEmail(string email, string password)
        {
            string inputString = password;

            // Create an ArrayList
            ArrayList arrayList = new ArrayList();

            // Loop through the string and add characters at even indices to the ArrayList
            for (int i = 0; i < password.Length; i += 1)
            {

                for (int j = 0; j <= i; j++)
                {
                    if (i == j)
                    {
                        string firstTwoLetters = inputString.Substring(0, i + 1);
                        arrayList.Add(firstTwoLetters);
                    }
                }
            }
            int matchCount = 0;
            foreach (string word in arrayList)
            {
                if (email.ToLower().Contains(word.ToLower()))
                {
                    matchCount += 1; if (matchCount >= 3) { return true; }
                }
            }

            if (matchCount >= 3) { return true; }

            return false;
        }


        [HttpPost]
        [Route("searchaddress")]
        public IActionResult search_address([FromBody] JsonObject obj)
        {
            Model.response.WebResponse response = null;
            string json = System.Text.Json.JsonSerializer.Serialize(obj);
            dynamic data = JObject.Parse(json);
            CompanyInfo.InsertrequestLogTracker("searchaddress full request body: " + JObject.Parse(json), 0, 0, 0, 0, "search_address", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);

            var validateJsonData = (dynamic)null;
            HttpContext context = HttpContext;
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
                    validateJsonData = new { response = false, responseCode = "02", data = "Invalid branch ID ." };
                    return new JsonResult(validateJsonData);
                }

                Obj.Client_ID = data.clientID;
                Obj.Branch_ID = data.branchID;
                Obj.PostCode = data.postCode;
                object o = JsonConvert.DeserializeObject(json);
                bool checkinjection = CompanyInfo.checksqlinjectiondata(o);
                if (!checkinjection)
                {
                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_TECHNICAL_ERROR);
                    response.ResponseMessage = "Invalid Field Values.";
                    response.ResponseCode = 6;
                    validateJsonData = new { response = false, responseCode = "01", data = response.ResponseMessage };
                    return new JsonResult(validateJsonData);
                }

                Service.srvaddress srv = new Service.srvaddress();
                object address = srv.searchaddress(Obj, context);
                response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_SUCCESS);
                response.data = "success";
                response.ObjData = address;
                response.ResponseCode = 0;





                //string jsonString = response.ObjData.ToString();

                //string cleanedAddressData = jsonString.Replace("\r\n", "").Trim('[', ']', ' ', '\r', '\n');

                ////string[] addressArray = cleanedAddressData.Split(new[] { '\"' }, StringSplitOptions.RemoveEmptyEntries);
                //string[] addressArray = cleanedAddressData.Split(new[] { '\"' });

                //// Create a list and add the cleaned addresses
                //List<string> addressList = new List<string>();
                //foreach (var address1 in addressArray)
                //{
                //    if (!string.IsNullOrWhiteSpace(address1))
                //    {
                //        string cleanedAddress = address1.Replace(",  ", " ");
                //        addressList.Add(cleanedAddress);
                //    }
                //}

                string jsonString = response.ObjData.ToString();

                // Remove unnecessary characters and whitespace
                string cleanedAddressData = jsonString.Replace("\r\n", "").Trim('[', ']', ' ', '\r', '\n');

                // Split the addresses based on quotes
                string[] addressArray = cleanedAddressData.Split(new[] { '\"' }, StringSplitOptions.RemoveEmptyEntries);

                // Create a list and add the cleaned addresses in the desired format
                List<string> addressList = new List<string>();
                foreach (var address1 in addressArray)
                {
                    // Check if the address is not null, not empty, and does not consist only of spaces after trimming
                    string trimmedAddress = address1.Trim();
                    if (!string.IsNullOrWhiteSpace(trimmedAddress) && trimmedAddress != ",")
                    {
                        string cleanedAddress = trimmedAddress.Replace(",  ", " ");
                        addressList.Add($"[\"{cleanedAddress}\"]");
                    }
                }

                // Convert the list to a single string with comma separation
                string formattedAddresses = string.Join(",", addressList);

                //DataTable dt = new DataTable((string?)response.ObjData);

                //List<Dictionary<string, object>> Titles = new List<Dictionary<string, object>>();
                //foreach (DataRow dr in dt.Rows)
                //{
                //    Dictionary<string, object> Title = new Dictionary<string, object>();
                //    Title["employeeTitleId"] = dr["Title_ID"];
                //    Title["employeeTitle"] = dr["Title"];
                //    Titles.Add(Title);
                //}
                validateJsonData = new { response = true, responseCode = "00", data = addressList };
                return new JsonResult(validateJsonData);
            }
            catch (Exception ex)
            {
                validateJsonData = new { response = false, responseCode = "01", data = "Invalid Request" };
                CompanyInfo.InsertErrorLogTracker(ex.ToString(), 0, 0, 0, 0, "search_address", 0, Convert.ToInt32(Obj.Client_ID), "", HttpContext);
                return new JsonResult(validateJsonData);
            }
        }

        [HttpGet]
        [Route("getalltermsandconditions")]
        public IActionResult GetAllTermsAndConditions()
        {
            Service.srvCustomer srv = new srvCustomer();
            List<TermsAndConditions> terms = srv.GetAllTermsAndConditions();

            return Ok(new
            {
                response = true,
                responseCode = "00",
                data = terms
            });
        }

        [HttpPost]
        [Route("add")]
        public IActionResult createCustomer([FromBody] JsonElement objdata)
        {
            var returnJsonData = (dynamic)null;
            Model.response.WebResponse response = null;
            string jsonData = System.Text.Json.JsonSerializer.Serialize(objdata);
            dynamic data = JObject.Parse(jsonData);
            CompanyInfo.InsertrequestLogTracker("createCustomer full request body: " + JObject.Parse(jsonData), 0, 0, 0, 0, "add", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
            
            string Activity_login1 = "";
            #region validateData
            try
            {

                //Start Parth added for proper fields validation
                #region proper fields validation
                if (!SqlInjectionProtector.ReadJsonElementValues_SingleElement(objdata).isValid)
                {
                    string fieldName = string.Empty;

                    var sqlValidationResult = SqlInjectionProtector.ReadJsonElementValues_SingleElement(objdata);   // Checked SQL injection validation
                    if (!sqlValidationResult.isValid)
                    {
                        fieldName = sqlValidationResult.fieldName;  // Captured the field name from the validation
                    }

                    string errorMessage = fieldName switch
                    {
                        "userName" => "Invalid input detected for Email",
                        "confirmUserName" => "Invalid input detected for Confirm Email",
                        "password" => "Invalid input detected for Password",
                        "confirmPassword" => "Invalid input detected for Confirm Password",
                        "mobileCode" => "Invalid input detected for Mobile Code",
                        "mobileNumber" => "Invalid input detected for Mobile Number",
                        "referenceCode" => "Invalid input detected for Reference Code",
                        _ => "Invalid input detected for " + fieldName
                    };

                    if (errorMessage != null)
                    {
                        var errorResponse = new
                        {
                            response = false,
                            responseCode = "02",
                            data = errorMessage
                        };
                        return new JsonResult(errorResponse) { StatusCode = 400 };
                    }
                }
                #endregion proper fields validation
                //End Parth added for proper fields validation

                /*if (!SqlInjectionProtector.ReadJsonElementValues(objdata) || !SqlInjectionProtector.ReadJsonElementValuesScript(objdata))
                {
                    var errorResponse = new { response = false, responseCode = "02", data = "Invalid input detected." };
                    return new JsonResult(errorResponse) { StatusCode = 400 };
                }*/

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
                else if (data.userName == "" || data.userName == null)
                {
                    returnJsonData = new { response = false, responseCode = "02", data = "Invalid UserName." };
                    return new JsonResult(returnJsonData);
                }
                else if (data.password == "" || data.password == null)
                {
                    returnJsonData = new { response = false, responseCode = "02", data = "Invalid Password." };
                    return new JsonResult(returnJsonData);
                }



                bool returnresult = false;
                returnresult = validateCustomerRegistration("Customer_con_email_man", objdata);
                if (returnresult)
                {
                    if (Convert.ToString(data.confirmUserName).Trim() != Convert.ToString(data.userName).Trim())
                    {
                        returnJsonData = new { response = false, responseCode = "02", data = "Username did not match." };
                        return new JsonResult(returnJsonData);
                    }
                }

                returnresult = validateCustomerRegistration("customerConfirmEmailVisible", objdata);
                if (returnresult)
                {
                    if (Convert.ToString(data.confirmUserName).Trim() != Convert.ToString(data.userName).Trim())
                    {
                        returnJsonData = new { response = false, responseCode = "02", data = "Username did not match." };
                        return new JsonResult(returnJsonData);
                    }
                }

                bool returnresultPass = validateCustomerRegistration("Customer_con_password", objdata);
                bool returnresultconfPass = validateCustomerRegistration("Customer_con_password_man", objdata);
                if (returnresultPass || returnresultconfPass)
                {
                    if (returnresultconfPass)
                    {
                        if (Convert.ToString(data.confirmPassword).Trim() == ""  || Convert.ToString(data.confirmPassword).Trim() == null )
                        {
                            returnJsonData = new { response = false, responseCode = "02", data = "Confirm Password field required." };
                            return new JsonResult(returnJsonData);
                        }
                    }



                    if (Convert.ToString(data.password).Trim() != Convert.ToString(data.confirmPassword).Trim() && returnresultconfPass 
                        && ( Convert.ToString(data.confirmPassword).Trim() != "" || Convert.ToString(data.confirmPassword).Trim() != null ) )
                    {
                        returnJsonData = new { response = false, responseCode = "02", data = "Password did not match." };
                        return new JsonResult(returnJsonData);
                    }
                    else if( !returnresultconfPass  && (Convert.ToString(data.confirmPassword).Trim() == "" || Convert.ToString(data.confirmPassword).Trim() == null))
                    {
                        // If confirm Password not mandetory and empty passed value
                    }
                    else if (!returnresultconfPass && (Convert.ToString(data.confirmPassword).Trim() != "" || Convert.ToString(data.confirmPassword).Trim() != null))
                    {
                        // If confirm Password not mandetory and not empty passed value
                        if ( Convert.ToString(data.password).Trim() != Convert.ToString(data.confirmPassword).Trim() )
                        {
                            returnJsonData = new { response = false, responseCode = "02", data = "Password did not match." };
                            return new JsonResult(returnJsonData);
                        }
                    }
                }

                /*object o = JsonConvert.DeserializeObject(jsonData);
                bool checkinjection = CompanyInfo.checksqlinjectiondata(o);
                if (!checkinjection)
                {
                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_TECHNICAL_ERROR);
                    response.ResponseMessage = "Invalid Field Values.";
                    response.ResponseCode = 6;
                    returnJsonData = new { response = false, responseCode = "02", data = response.ResponseMessage };
                    return new JsonResult(returnJsonData);
                }
                */

            }
            catch (Exception ex)
            {
                Model.ActivityLogTracker _objActivityLog = new Model.ActivityLogTracker();
                _objActivityLog.Activity = " Exception Customer create Error: " + ex.ToString() + " ";
                _objActivityLog.FunctionName = "createCustomer";
                _objActivityLog.Transaction_ID = 0;
                _objActivityLog.WhoAcessed = 1;
                _objActivityLog.Branch_ID = 0;
                _objActivityLog.Client_ID = 0;

                Service.srvActivityLogTracker srvActivityLog = new Service.srvActivityLogTracker();
                int _i = srvActivityLog.Create(_objActivityLog, HttpContext);
                CompanyInfo.InsertErrorLogTracker(" Exception Customer create Error: " + ex.ToString(), 0, 0, 0, 0, "createCustomer", Convert.ToInt32(data.Branch_ID), Convert.ToInt32(data.Client_ID), "createCustomer", HttpContext);
                returnJsonData = new { response = false, responseCode = "01", data = "Field is missing." };
                return new JsonResult(returnJsonData);
            }
            #endregion validateData

            #region registerCustomer
            // Register customer
            try
            {
                Customer obj = JsonConvert.DeserializeObject<Customer>(jsonData);
                obj.Email = Convert.ToString(data.userName).ToLower();
                obj.Password = Convert.ToString(data.password);
                obj.refcode = Convert.ToString(data.referenceCode);
                obj.MobileNumber = Convert.ToString(data.mobileNumber);
                obj.Mobile_number_code = Convert.ToString(data.mobileCode);
                obj.Country_Id = Convert.ToInt32(data.countryID);
                obj.Client_ID = Convert.ToInt32(data.clientID);
                obj.Branch_ID = Convert.ToInt32(data.branchID);
                obj.base_currency_id = Convert.ToInt32(data.baseCurrencyID);
                obj.Referred_By_Agent = Convert.ToInt32(data.referredByAgent);

                try { obj.Captcha = data.Captcha; } catch (Exception ex) { obj.Captcha = ""; }

                try { obj.Countryof_Birth = Convert.ToString(data.Countryof_Birth); } catch(Exception ex) { obj.Countryof_Birth = ""; }

                try
                {
                    if (Convert.ToString(data.Annual_Salary) == "") { data.Annual_Salary = "0"; }
                    obj.Annual_Salary = Convert.ToString(data.Annual_Salary);
                }
                catch(Exception ex) { obj.Annual_Salary = "0";  }

                if (PasswordContainsEmail(obj.Email, obj.Password))
                {
                    returnJsonData = new { response = false, responseCode = "02", data = "Password is not valid.Do not use password same as username." };
                    return new JsonResult(returnJsonData);
                }
try
                {
                    if (obj.MobileNumber != "" && obj.MobileNumber != null)
                    {
                        bool validateMobileNumber = IsFirst4DigitsSame(obj.MobileNumber);
                        if (validateMobileNumber)
                        {
                            returnJsonData = new { response = false, responseCode = "02", data = "Invalid mobile number." };
                            return new JsonResult(returnJsonData);
                        }
                    }
                }
                catch (Exception ex) { }
                Model.Customer _ObjCustomer = new Model.Customer();
                Service.srvCustomer srvCustomer = new Service.srvCustomer();
                response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_SUCCESS);

                _ObjCustomer = srvCustomer.CreateCustomer(obj, HttpContext);
                response.ObjData = _ObjCustomer;


                // send mail
                if (_ObjCustomer.Message == "success")
                {
                    #region "Send Mail"
                    Service.srvLogin srvLogin = new Service.srvLogin();
                    DataTable dt = new DataTable();
                    Model.Login objLogin = new Model.Login();
                    objLogin.UserName = obj.Email;
                    objLogin.Client_ID = obj.Client_ID;
                    DataTable dtc = CompanyInfo.get(objLogin.Client_ID, HttpContext);
                    dt = srvLogin.IsValidEmail(objLogin);
                    try
                    {
                        if (dt.Rows.Count > 0)
                        {
                            string email = objLogin.UserName;
                            string body = string.Empty, subject = string.Empty;
                            string body1 = string.Empty;
                            //string template = "";
                            HttpWebRequest httpRequest = null, httpRequest1 = null;
                            //DataTable dtc = CompanyInfo.get(objLogin.Client_ID);
                            if (dtc.Rows.Count > 0)
                            {
                                string company_name = Convert.ToString(dtc.Rows[0]["Company_Name"]);
                                string URL = Convert.ToString(dtc.Rows[0]["Company_URL_Admin"]);
                                string cust_url = Convert.ToString(dtc.Rows[0]["Company_URL_Customer"]);
                                //httpRequest = (HttpWebRequest)WebRequest.Create(URL + "Email/Customer-Registered-Success-Details.htm");
                                httpRequest = (HttpWebRequest)WebRequest.Create(URL + "Email/customer-registration-first-step.html");
                                httpRequest.UserAgent = "Code Sample Web Client";
                                HttpWebResponse webResponse = (HttpWebResponse)httpRequest.GetResponse();
                                using (StreamReader reader = new StreamReader(webResponse.GetResponseStream()))
                                {
                                    body = reader.ReadToEnd();
                                }
                                body = body.Replace("[name]", obj.FirstName);//+ " " + obj.LastName);
                                                                             // body = body.Replace("[name]", dt.Rows[0]["full_name"].ToString());
                                                                             //string ref_no = Convert.ToString(dt.Rows[0]["WireTransfer_ReferanceNo"]);
                                                                             //int bool_check = 1;
                                                                             //bool b1 = Convert.ToBoolean(bool_check);
                                                                             //string s2 = Convert.ToString(dt.Rows[0]["WireTransfer_ReferanceNo"]);
                                                                             //string str_de2 = Convert.ToString(CompanyInfo.Encrypt(s2, b1));
                                                                             //body = body.Replace("[link]", cust_url + "new-password.aspx?reference=" + str_de2 + "");
                                                                             // body = body.Replace("[insert body]", body1);
                                httpRequest1 = (HttpWebRequest)WebRequest.Create(URL + "Email/RegisteredSuccessMail.txt");
                                httpRequest1.UserAgent = "Code Sample Web Client";
                                HttpWebResponse webResponse1 = (HttpWebResponse)httpRequest1.GetResponse();
                                using (StreamReader reader = new StreamReader(webResponse1.GetResponseStream()))
                                {
                                    subject = reader.ReadLine();
                                }
                                string newsubject = company_name + " - " + subject + " - " + Convert.ToString(dt.Rows[0]["WireTransfer_ReferanceNo"]);
                                newsubject = "Welcome to "+ company_name+" — Let’s Get You Set Up!";

                                //string msg = (string)CompanyInfo.Send_Mail(dtc, objLogin.UserName, body, newsubject, obj.Client_ID, obj.Branch_ID, "", "", "");


                                // Check zoho email activate or not
                                MySqlCommand cmdp_activethirdpartyAPI = new MySqlCommand("active_thirdparti_api");
                                cmdp_activethirdpartyAPI.CommandType = CommandType.StoredProcedure;
                                DataTable dtApi_activethirdpartyAPI = db_connection.ExecuteQueryDataTableProcedure(cmdp_activethirdpartyAPI);
                                string activeZohoAPI = "F"; string msg = "";

                                if (dtApi_activethirdpartyAPI.Rows.Count > 0)
                                {
                                    for (int ita = 0; ita < dtApi_activethirdpartyAPI.Rows.Count; ita++)
                                    {
                                        if (Convert.ToInt32(dtApi_activethirdpartyAPI.Rows[ita]["API_ID"]) == 6) { activeZohoAPI = "T"; break; }
                                    }
                                }
                                if (activeZohoAPI == "T")
                                {
                                    int zohoAPIID = 6;
                                    string zohoToken = CompanyInfo.generateZohoToken(zohoAPIID, HttpContext);
                                    string description = "";
                                    DataTable dtuserData = new DataTable();
                                    dtuserData.Columns.Add("firstname", typeof(string));
                                    dtuserData.Columns.Add("lastname", typeof(string));
                                    dtuserData.Columns.Add("emailaddress", typeof(string));
                                    dtuserData.Columns.Add("description", typeof(string));
                                    dtuserData.Columns.Add("street", typeof(string));
                                    dtuserData.Columns.Add("state", typeof(string));
                                    dtuserData.Columns.Add("city", typeof(string));
                                    DataRow dr = dtuserData.NewRow();
                                    dr["firstname"] = Convert.ToString(obj.FirstName).Trim();
                                    dr["lastname"] = Convert.ToString(obj.LastName).Trim();
                                    dr["emailaddress"] = Convert.ToString(obj.Email).Trim();
                                    dr["description"] = Convert.ToString(description).Trim();
                                    dr["street"] = Convert.ToString("").Trim(); // coupon_code value get here
                                    dr["state"] = ""; // expires_date value get here
                                    dr["city"] = Convert.ToString("").Trim();
                                    dtuserData.Rows.Add(dr);
                                    string leadId = ""; string updateRecordStatus = "F";

                                    leadId = CompanyInfo.getZohoLeadIdDetails(zohoToken, zohoAPIID, dtuserData, HttpContext);
                                    bool digitsOnly = leadId.All(char.IsDigit);

                                    if (digitsOnly)
                                    {
                                        updateRecordStatus = CompanyInfo.updateZohoLeadDetails(zohoToken, zohoAPIID, dtuserData, HttpContext);
                                    }
                                    if (updateRecordStatus == "F")
                                    {
                                        leadId = CompanyInfo.generateZohoLead(zohoToken, zohoAPIID, dtuserData, HttpContext);
                                    }

                                    DataTable dttemplateData = new DataTable();
                                    dttemplateData.Columns.Add("zohoid", typeof(int));
                                    dttemplateData.Columns.Add("leadid", typeof(string));
                                    dttemplateData.Columns.Add("zohoToken", typeof(string));
                                    dttemplateData.Columns.Add("templateName", typeof(string));
                                    DataRow drtem = dttemplateData.NewRow();
                                    drtem["zohoid"] = zohoAPIID;
                                    drtem["leadid"] = Convert.ToString(leadId).Trim();
                                    drtem["zohoToken"] = Convert.ToString(zohoToken).Trim();
                                    drtem["templateName"] = Convert.ToString("custregistrationtempid").Trim();

                                    dttemplateData.Rows.Add(drtem);
                                    msg = (string)CompanyInfo.Send_Mail_usingZoho(email, body, newsubject, obj.Client_ID, Convert.ToInt32(obj.Branch_ID), "discount Customer Mail", "", "", dttemplateData, HttpContext);
                                    if (msg == "NOT_ALLOWED")
                                    {
                                        msg = (string)CompanyInfo.Send_Mail(dtc, objLogin.UserName, body, newsubject, obj.Client_ID, obj.Branch_ID, "", "", "", HttpContext);
                                    }

                                }
                                else
                                {
                                    msg = (string)CompanyInfo.Send_Mail(dtc, objLogin.UserName, body, newsubject, obj.Client_ID, obj.Branch_ID, "", "", "", HttpContext);
                                }



                                response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_SUCCESS);
                                response.data = "success"; response.ObjData = _ObjCustomer;
                                response.ResponseCode = 0;


                                if (msg == "Success")
                                {
                                    Activity_login1 = "Successful registration email sent to " + objLogin.UserName + " ";
                                    //stattus61 = (int)CompanyInfo.InsertActivityLogDetails(Activity_login1, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), 0, "Register_controller", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "Customer Registration");
                                    _ = CompanyInfo.InsertActivityLogDetailsasync(Activity_login1, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), 0, "Register_controller", Convert.ToInt32(obj.Branch_ID), 1, "Customer Registration", HttpContext);
                                }
                                else
                                {
                                    Activity_login1 = "Successful registration email not sent to " + objLogin.UserName + " " + msg;
                                    //stattus61 = (int)CompanyInfo.InsertActivityLogDetails(Activity_login1, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), 0, "Register_controller", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "Customer Registration");
                                    _ = CompanyInfo.InsertActivityLogDetailsasync(Activity_login1, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), 0, "Register_controller", Convert.ToInt32(obj.Branch_ID), 1, "Customer Registration", HttpContext);
                                }


                            }

                            else
                            {
                                Model.Customer obj1 = new Model.Customer(); obj1.Message = "Failed";
                                response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_TECHNICAL_ERROR);
                                response.ResponseMessage = "Something went wrong. Please try again later.";
                                response.ResponseCode = 2; response.ObjData = obj1;
                            }
                        }
                        else
                        {
                            Model.Customer obj1 = new Model.Customer(); obj1.Message = "Failed";
                            response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_TECHNICAL_ERROR);
                            response.ResponseMessage = "This Email is not registered with us.";
                            response.ResponseCode = 3; response.ObjData = obj1;

                        }
                    }
                    catch (Exception e)
                    { }
                    #endregion

                    #region Verification Mail
                    try //Send Verification mail
                    {

                        if (obj.Perm_status == 0)
                        {
                            string msg1 = "";//CompanyInfo.Send_verify_mail(obj, HttpContext);

                            //Model.ActivityLogTracker _objActivityLog = new Model.ActivityLogTracker();
                            try
                            {

                                if (msg1 == "Success")
                                {
                                    Activity_login1 = "Verification email sent to " + objLogin.UserName + " " + msg1;
                                    //stattus61 = (int)CompanyInfo.InsertActivityLogDetails(Activity_login1, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), 0, "Register_controller", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "CreateCustomer");
                                    _ = CompanyInfo.InsertActivityLogDetailsasync(Activity_login1, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), 0, "Register_controller", Convert.ToInt32(obj.Branch_ID), 1, "CreateCustomer", HttpContext);
                                }
                            }
                            catch (Exception ex)
                            {
                                Activity_login1 = "Verification email not sent to " + objLogin.UserName + " " + msg1;
                                //stattus61 = (int)CompanyInfo.InsertActivityLogDetails(Activity_login1, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), 0, "Register_controller", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "CreateCustomer");
                                _ = CompanyInfo.InsertActivityLogDetailsasync(Activity_login1, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), 0, "Register_controller", Convert.ToInt32(obj.Branch_ID), 1, "CreateCustomer", HttpContext);
                            }
                        }
                    }
                    catch (Exception ex) { }
                    #endregion

                    List<Dictionary<string, object>> customerData = new List<Dictionary<string, object>>();
                    Dictionary<string, object> customer = new Dictionary<string, object>();


                    if (response.ResponseCode == 0)
                    {
                        customer["customerID"] = _ObjCustomer.Customer_ID;
                        customer["customerUsername"] = _ObjCustomer.UserName;
                        customer["status"] = "Great start! Let’s move to the next step.";
                        customerData.Add(customer);

                        returnJsonData = new { response = true, responseCode = "00", data = customerData };
                        return new JsonResult(returnJsonData);
                    }
                    else if (response.ResponseCode == 2)
                    {
                        customer["customerID"] = _ObjCustomer.Customer_ID;
                        customer["customerUsername"] = _ObjCustomer.UserName;
                        customer["status"] = response.ResponseMessage;
                        customerData.Add(customer);
                        returnJsonData = new { response = true, responseCode = "02", data = customerData };
                        return new JsonResult(returnJsonData);
                    }
                    else if (response.ResponseCode == 3)
                    {
                        customer["customerID"] = _ObjCustomer.Customer_ID;
                        customer["customerUsername"] = _ObjCustomer.UserName;
                        customer["status"] = response.ResponseMessage;
                        customerData.Add(customer);
                        returnJsonData = new { response = true, responseCode = "02", data = customerData };
                        return new JsonResult(returnJsonData);
                    }

                }
                else if(_ObjCustomer.Message == "Invalid Captcha")
                {
                    returnJsonData = new { response = false, responseCode = "02", data = "Invalid Captcha" };
                    return new JsonResult(returnJsonData);
                }
                else
                {
                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_SUCCESS);
                    response.data = "Validation Failed";
                    response.ObjData = (_ObjCustomer.Message).ToString();
                    response.ResponseCode = 0;
                    returnJsonData = new { response = false, responseCode = "02", data = (_ObjCustomer.Message).ToString() };
                    return new JsonResult(returnJsonData);
                }

            }
            catch (Exception ex)
            {
                Model.ActivityLogTracker _objActivityLog = new Model.ActivityLogTracker();
                _objActivityLog.Activity = " Exception Customer create Error: " + ex.ToString() + " ";
                _objActivityLog.FunctionName = "createCustomer";
                _objActivityLog.Transaction_ID = 0;
                _objActivityLog.WhoAcessed = 1;
                _objActivityLog.Branch_ID = 0;
                _objActivityLog.Client_ID = 0;

                Service.srvActivityLogTracker srvActivityLog = new Service.srvActivityLogTracker();
                int _i = srvActivityLog.Create(_objActivityLog, HttpContext);
                CompanyInfo.InsertErrorLogTracker(" Exception Customer create Error: " + ex.ToString(), 0, 0, 0, 0, "createCustomer", Convert.ToInt32(data.Branch_ID), Convert.ToInt32(data.Client_ID), "", HttpContext);
                returnJsonData = new { response = false, responseCode = "01", data = "Invalid request." };
                return new JsonResult(returnJsonData);
            }
            #endregion registerCustomer

            return returnJsonData;
        }
static bool IsFirst4DigitsSame(string Number)
        {
            // Check if the first 4 digits are all '0'
            if (Number.Length >= 4 && Number.Substring(0, 4).All(digit => digit == '0'))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        [HttpPost]
        [Route("referrername")]
        public IActionResult GetReferrerName([FromBody] JsonElement objdata)
        {
            var returnJsonData = (dynamic)null;
            Model.response.WebResponse response = null;
            string jsonData = System.Text.Json.JsonSerializer.Serialize(objdata);
            dynamic data = JObject.Parse(jsonData);
            CompanyInfo.InsertrequestLogTracker("referrername full request body: " + JObject.Parse(jsonData), 0, 0, 0, 0, "GetReferrerName", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
            List<Model.Customer> _lst = new List<Model.Customer>();
            Customer obj1 = new Customer();

            if (!SqlInjectionProtector.ReadJsonElementValues(objdata) || !SqlInjectionProtector.ReadJsonElementValuesScript(objdata))
            {
                var errorResponse = new { response = false, responseCode = "02", data = "Invalid input detected." };
                return new JsonResult(errorResponse) { StatusCode = 400 };
            }

            if (data.clientID == "" || data.clientID == null)
            {
                returnJsonData = new { response = false, responseCode = "02", data = "Invalid Client ID." };
                return new JsonResult(returnJsonData);
            }
            obj1.refcode = data.refCode;
            obj1.Client_ID = data.clientID;



            Service.srvCustomer srv = new Service.srvCustomer();
            Dictionary<string, object> titleList = new Dictionary<string, object>();
            _lst = srv.Get_Referrer_Name(obj1);

            if (_lst != null && _lst.Count > 0)
            {


                foreach (var item in _lst)
                {

                    titleList["ReferrerFlag"] = item.Referrer_Flag;
                    titleList["ID"] = item.Id;
                    titleList["Full_Name"] = item.Name;
                    titleList["ReferenceNo"] = item.refcode;
                }


                response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_SUCCESS);
                returnJsonData = new { response = true, responseCode = "00", data = titleList };
                return new JsonResult(returnJsonData);
            }
            else
            {
                response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_TECHNICAL_ERROR);
                response.ResponseMessage = "No Records Found.";
                response.ResponseCode = 0;
            }

            returnJsonData = new { response = false, responseCode = "02", data = "You have entered invalid referral code. Do you still want to continue?" };
            return new JsonResult(returnJsonData);
        }


        [HttpPost]
        [Route("checkemail")]
        public IActionResult check_Email_Is_Exist([FromBody] JsonElement objdata)
        {
            var returnJsonData = (dynamic)null;
            Model.response.WebResponse response = null;
            string jsonData = System.Text.Json.JsonSerializer.Serialize(objdata);
            dynamic data = JObject.Parse(jsonData);
            CompanyInfo.InsertrequestLogTracker("checkemail full request body: " + JObject.Parse(jsonData), 0, 0, 0, 0, "check_Email_Is_Exist", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
            List<Model.Customer> _lst = new List<Model.Customer>();
            Customer obj1 = new Customer();

            if (!SqlInjectionProtector.ReadJsonElementValues(objdata) || !SqlInjectionProtector.ReadJsonElementValuesScript(objdata))
            {
                var errorResponse = new { response = false, responseCode = "02", data = "Invalid input detected." };
                return new JsonResult(errorResponse) { StatusCode = 400 };
            }

            obj1.Client_ID = data.clientID;
            obj1.Branch_ID = data.branchID;
            obj1.Email = data.emailID;


            Service.srvCustomer srvCustomer = new Service.srvCustomer();
            response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_SUCCESS);
            response.data = srvCustomer.check_Email_Is_Exist(obj1);
            if (response.data != null)
            {
                if (response.data == "exist_email")
                {
                    response.ObjData = response.data;
                    response.ResponseCode = 00;
                    returnJsonData = new { response = true, responseCode = "00", data = true };
                }
                else if (response.data == "not_exist_email")
                {
                    response.ObjData = response.data;
                    response.ResponseCode = 02;
                    returnJsonData = new { response = false, responseCode = "02", data = false };
                }
            }
            else
            {
                response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_TECHNICAL_ERROR);
                response.ResponseMessage = "No Records Found.";
                response.ResponseCode = 0;
            }

            return new JsonResult(returnJsonData);
        }

        [HttpPost]
        [Route("getcustomerinfo")]
        public IActionResult getcustomerinfo([FromBody] JsonElement objdata)
        {
            var returnJsonData = (dynamic)null;
            try
            {
                Model.response.WebResponse response = null;
                string jsonData = System.Text.Json.JsonSerializer.Serialize(objdata);
                dynamic data = JObject.Parse(jsonData);
                CompanyInfo.InsertrequestLogTracker("getcustomerinfo full request body: " + JObject.Parse(jsonData), 0, 0, 0, 0, "getcustomerinfo", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);

                List<Model.Customer> _lst = new List<Model.Customer>();
                Customer obj1 = new Customer();

                if (!SqlInjectionProtector.ReadJsonElementValues(objdata) || !SqlInjectionProtector.ReadJsonElementValuesScript(objdata))
                {
                    var errorResponse = new { response = false, responseCode = "02", data = "Invalid input detected." };
                    return new JsonResult(errorResponse) { StatusCode = 400 };
                }

                obj1.Client_ID = data.clientID;
                obj1.Branch_ID = data.branchID;
                obj1.Id = data.customerID;

                Service.srvCustomer srv = new Service.srvCustomer();
                DataTable li1 = srv.Customer_Details(obj1);
                //Model.Dashboard _Obj = new Model.Dashboard();
                if (li1 != null && li1.Rows.Count > 0)
                {
                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_SUCCESS);
                    response.data = "success";
                    response.ObjData = li1;
                    response.ResponseCode = 0;

                    string dateFormat = "%d-%m-%Y"; // default format if DB doesn't return one

                    try
                    {
                        MySqlCommand cmd = new MySqlCommand("sp_company_details");
                        cmd.CommandType = CommandType.StoredProcedure;

                        DataTable dtFormat = db_connection.ExecuteQueryDataTableProcedure(cmd);

                        if (dtFormat != null && dtFormat.Rows.Count > 0)
                        {
                            string dbFormat = dtFormat.Rows[0]["Date_Format"].ToString();

                            // Convert MySQL-style date format to .NET compatible format
                            dateFormat = CompanyInfo.ConvertMySQLDateFormatToDotNet(dbFormat);
                        }
                    }
                    catch (Exception ex)
                    {
                        _ = Task.Run(() => CompanyInfo.InsertErrorLogTracker(ex.ToString(), 0, 0, 0, 0, "getcustomerinfo", 0, 0, "", HttpContext));
                    }

                    string currentDate = DateTime.Now.ToString(dateFormat);

                    List<Dictionary<string, string>> returndata;
                    try
                    {
                        returndata = li1.Rows.OfType<DataRow>()
                            .Select(row => li1.Columns.OfType<DataColumn>()
                                .ToDictionary(col => col.ColumnName, col => row[col].ToString()))
                            .ToList();

                        if (returndata.Count > 0)
                        {
                            returndata[0]["CurrentDate"] = currentDate;
                        }
                    }
                    catch (Exception ex)
                    {
                        _ = Task.Run(() => CompanyInfo.InsertErrorLogTracker(ex.ToString(), 0, 0, 0, 0, "getcustomerinfo", 0, Convert.ToInt32(0), "", HttpContext));

                        returndata = new List<Dictionary<string, string>>();
                    }

                    returnJsonData = new { response = true, responseCode = "00", data = returndata };
                    return new JsonResult(returnJsonData);
                }
                else
                {
                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_TECHNICAL_ERROR);
                    response.ResponseMessage = "No Records Found.";
                    response.ResponseCode = 0;
                   
                    returnJsonData = new { response = false, responseCode = "02", data = "No record found" };
                    return new JsonResult(returnJsonData);
                }
            }
            catch (Exception ex)
            {
                returnJsonData = new { response = false, responseCode = "01", data = "Invalid Request" };
                CompanyInfo.InsertErrorLogTracker(ex.ToString(), 0, 0, 0, 0, "getcustomerinfo", 0, Convert.ToInt32(0), "", HttpContext);
                return new JsonResult(returnJsonData);
            }
        }

        [HttpPost]
        [Route("email_verify")]
        public IActionResult email_verify([FromBody] JsonElement objdata)
        {
            var returnJsonData = (dynamic)null;
            try
            {
                Model.response.WebResponse response = null;
                string jsonData = System.Text.Json.JsonSerializer.Serialize(objdata);
                dynamic data = JObject.Parse(jsonData);
                CompanyInfo.InsertrequestLogTracker("email_verify full request body: " + JObject.Parse(jsonData), 0, 0, 0, 0, "email_verify", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
                Customer obj1 = new Customer();

                if (!SqlInjectionProtector.ReadJsonElementValues(objdata) || !SqlInjectionProtector.ReadJsonElementValuesScript(objdata))
                {
                    var errorResponse = new { response = false, responseCode = "02", data = "Invalid input detected." };
                    return new JsonResult(errorResponse) { StatusCode = 400 };
                }

                obj1.Client_ID = data.clientID;
                obj1.Branch_ID = data.branchID;
                obj1.Customer_ID = data.customerID;
                obj1.UserName = data.username;
                try
                {
                    string status = Send_verify_mail(obj1);
                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_SUCCESS);
                    response.ResponseMessage = status;

                    if("failed" == status)
                    {
                        returnJsonData = new { response = false, responseCode = "02", data = status };
                        return new JsonResult(returnJsonData);
                    }

                    returnJsonData = new { response = true, responseCode = "00", data = status };
                    return new JsonResult(returnJsonData);
                }
                catch (Exception ex)
                {
                    CompanyInfo.InsertErrorLogTracker("email_verify Send_verify_mail error" + ex.ToString(), 0, 0, 0, 0, "email_verify", 0, Convert.ToInt32(0), "", HttpContext);
                }


                returnJsonData = new { response = false, responseCode = "02", data = "" };
                return new JsonResult(returnJsonData);
            }
            catch (Exception ex)
            {
                returnJsonData = new { response = false, responseCode = "01", data = "Invalid Request" };
                CompanyInfo.InsertErrorLogTracker("email_verify error"+ex.ToString(), 0, 0, 0, 0, "email_verify", 0, Convert.ToInt32(0), "", HttpContext);
                return new JsonResult(returnJsonData);
            }
        }


            [HttpPost]
        [Route("update")]
        public IActionResult customerUpdate([FromBody] JsonElement objdata)
        {
            var returnJsonData = (dynamic)null;
            HttpContext context = HttpContext;
            Model.response.WebResponse response = null;
            Document obj123 = new Document();
            string jsonData = System.Text.Json.JsonSerializer.Serialize(objdata);
            dynamic data = JObject.Parse(jsonData);
            CompanyInfo.InsertrequestLogTracker("customerUpdate full request body: " + JObject.Parse(jsonData), 0, 0, 0, 0, "update", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);

            string Activity_login1 = "";
            string stepNumber = "0";
            #region validateData
            try
            {

                //Start Parth added for proper fields validation
                #region proper fields validation
                if (!SqlInjectionProtector.ReadJsonElementValues_SingleElement(objdata).isValid)
                {
                    string fieldName = string.Empty;

                    var sqlValidationResult = SqlInjectionProtector.ReadJsonElementValues_SingleElement(objdata);   // Checked SQL injection validation
                    if (!sqlValidationResult.isValid)
                    {
                        fieldName = sqlValidationResult.fieldName;  // Captured the field name from the validation
                    }

                    string errorMessage = fieldName switch
                    {
                        "emailID" => "Invalid input detected for Email ID",
                        "gender" => "Invalid input detected for Gender",
                        "firstName" => "Invalid input detected for First Name",
                        "middleName" => "Invalid input detected for Middle Name",
                        "lastName" => "Invalid input detected for Last Name",
                        "dob" => "Invalid input detected for Date Of Birth",
                        "mobileCode" => "Invalid input detected for Mobile Code",
                        "mobile" => "Invalid input detected for Mobile Number",
                        "phoneCode" => "Invalid input detected for Phone Code",
                        "phone" => "Invalid input detected for Phone Number",
                        "postCode" => "Invalid input detected for Post Code",
                        "houseNumber" => "Invalid input detected for House Number",
                        "address1" => "Invalid input detected for Street",
                        "address2" => "Invalid input detected for Address Line 2",
                        "provience" => "Invalid input detected for Provience",
                        "birthPlace" => "Invalid input detected for Birth Place",
                        "professionName" => "Invalid input detected for Profession Name",
                        "companyName" => "Invalid input detected for Company Name",
                        "heardFromEvent" => "Invalid input detected for Heard From Event",
                        "nationalityName" => "Invalid input detected for Nationality",
                        "sourseOfRegistration" => "Invalid input detected for Source Of Registration",
                        _ => "Invalid input detected for " + fieldName
                    };

                    if (errorMessage != null)
                    {
                        var errorResponse = new
                        {
                            response = false,
                            responseCode = "02",
                            data = errorMessage
                        };
                        return new JsonResult(errorResponse) { StatusCode = 400 };
                    }
                }
                #endregion proper fields validation
                //End Parth added for proper fields validation

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
                else if (Convert.ToString(data.emailID).Trim() == "" || Convert.ToString(data.emailID).Trim() == null)
                {
                    returnJsonData = new { response = false, responseCode = "02", data = "Invalid Email field." };
                    return new JsonResult(returnJsonData);
                }
                else if (Convert.ToString(data.customerID).Trim() == "" || Convert.ToString(data.customerID).Trim() == null)
                {
                    returnJsonData = new { response = false, responseCode = "02", data = "customerID Not found." };
                    return new JsonResult(returnJsonData);
                }

                string returnresult = "";
                

                try { stepNumber = Convert.ToString(data.step); }catch(Exception) {  }


                if (stepNumber == "1")
                {
                    returnresult = validateCustomerRegistrationUpdate("Customer_title_man", objdata);
                    if (returnresult != "")
                    {
                        try
                        {
                            if (Convert.ToString(data.title).Trim() == "" || Convert.ToString(data.title).Trim() == "0")
                            {
                                returnJsonData = new { response = false, responseCode = "02", data = returnresult };
                                return new JsonResult(returnJsonData);
                            }
                        }
                        catch
                        {
                            returnJsonData = new { response = false, responseCode = "02", data = "title field missing" };
                            return new JsonResult(returnJsonData);
                        }
                    }

                    returnresult = validateCustomerRegistrationUpdate("Gender_man", objdata);
                    if (returnresult != "")
                    {
                        try
                        {
                            if (Convert.ToString(data.gender).Trim() == "")
                            {
                                returnJsonData = new { response = false, responseCode = "02", data = returnresult };
                                return new JsonResult(returnJsonData);
                            }
                        }
                        catch
                        {
                            returnJsonData = new { response = false, responseCode = "02", data = "gender field missing" };
                            return new JsonResult(returnJsonData);
                        }
                    }

                    returnresult = validateCustomerRegistrationUpdate("First_name_man", objdata);
                    if (returnresult != "")
                    {
                        try
                        {
                            if (Convert.ToString(data.firstName).Trim() == "")
                            {
                                returnJsonData = new { response = false, responseCode = "02", data = returnresult };
                                return new JsonResult(returnJsonData);
                            }
                        }
                        catch
                        {
                            returnJsonData = new { response = false, responseCode = "02", data = "firstName field missing" };
                            return new JsonResult(returnJsonData);
                        }
                    }

                    returnresult = validateCustomerRegistrationUpdate("Middle_name_man", objdata);
                    if (returnresult != "")
                    {
                        try
                        {
                            if (Convert.ToString(data.middleName).Trim() == "")
                            {
                                returnJsonData = new { response = false, responseCode = "02", data = returnresult };
                                return new JsonResult(returnJsonData);
                            }
                        }
                        catch
                        {
                            returnJsonData = new { response = false, responseCode = "02", data = "middleName field missing" };
                            return new JsonResult(returnJsonData);
                        }
                    }

                    returnresult = validateCustomerRegistrationUpdate("Last_name_man", objdata);
                    if (returnresult != "")
                    {
                        try
                        {
                            if (Convert.ToString(data.lastName).Trim() == "")
                            {
                                returnJsonData = new { response = false, responseCode = "02", data = returnresult };
                                return new JsonResult(returnJsonData);
                            }
                        }
                        catch
                        {
                            returnJsonData = new { response = false, responseCode = "02", data = "lastName field missing" };
                            return new JsonResult(returnJsonData);
                        }
                    }

                    returnresult = validateCustomerRegistrationUpdate("Date_of_Birth_man", objdata);
                    if (returnresult != "")
                    {
                        try
                        {
                            if (Convert.ToString(data.dob).Trim() == "")
                            {
                                returnJsonData = new { response = false, responseCode = "02", data = returnresult };
                                return new JsonResult(returnJsonData);
                            }
                        }
                        catch
                        {
                            returnJsonData = new { response = false, responseCode = "02", data = "dob field missing" };
                            return new JsonResult(returnJsonData);
                        }
                    }

                    /*returnresult = validateCustomerRegistrationUpdate("Customer_Mobile_number_man", objdata);
                    if (returnresult != "")
                    {
                        try
                        {
                            if (Convert.ToString(data.mobile).Trim() == "")
                            {
                                returnJsonData = new { response = false, responseCode = "02", data = returnresult };
                                return new JsonResult(returnJsonData);
                            }
                        }
                        catch
                        {
                            returnJsonData = new { response = false, responseCode = "02", data = "mobile field missing" };
                            return new JsonResult(returnJsonData);
                        }
                    }*/
                    returnresult = validateCustomerRegistrationUpdate("Customer_birthplace", objdata);
                    if (returnresult != "")
                    {
                        try
                        {
                            if (Convert.ToString(data.birthPlace).Trim() == "")
                            {
                                returnJsonData = new { response = false, responseCode = "02", data = returnresult };
                                return new JsonResult(returnJsonData);
                            }
                        }
                        catch
                        {
                            returnJsonData = new { response = false, responseCode = "02", data = "birthPlace field missing" };
                            return new JsonResult(returnJsonData);
                        }
                    }

                    

                    returnresult = validateCustomerRegistrationUpdate("customer_phone_man", objdata);
                    if (returnresult != "")
                    {
                        try
                        {
                            if (Convert.ToString(data.phone).Trim() == "")
                            {
                                returnJsonData = new { response = false, responseCode = "02", data = returnresult };
                                return new JsonResult(returnJsonData);
                            }
                        }
                        catch
                        {
                            returnJsonData = new { response = false, responseCode = "02", data = "phone field missing" };
                            return new JsonResult(returnJsonData);
                        }
                    }
                }

                if (stepNumber == "2")
                {
                    try
                    {
                        obj123.Client_ID = data.clientID;
                        obj123.Branch_ID = data.branchID;
                        obj123.Customer_ID = data.customerID;
                        Service.srvDocument srv = new Service.srvDocument();
                        DataTable li1 = srv.GetToken(obj123, context);

                    }
                    catch { }
                    returnresult = validateCustomerRegistrationUpdate("Customer_post_code_man", objdata);
                    if (returnresult != "")
                    {
                        try
                        {
                            if (Convert.ToString(data.postCode).Trim() == "")
                            {
                                returnJsonData = new { response = false, responseCode = "02", data = returnresult };
                                return new JsonResult(returnJsonData);
                            }
                        }
                        catch
                        {
                            returnJsonData = new { response = false, responseCode = "02", data = "postCode field missing" };
                            return new JsonResult(returnJsonData);
                        }
                    }

                    returnresult = validateCustomerRegistrationUpdate("Customer_house_number_man", objdata);
                    if (returnresult != "")
                    {
                        try
                        {
                            if (Convert.ToString(data.houseNumber).Trim() == "")
                            {
                                returnJsonData = new { response = false, responseCode = "02", data = returnresult };
                                return new JsonResult(returnJsonData);
                            }
                        }
                        catch
                        {
                            returnJsonData = new { response = false, responseCode = "02", data = "houseNumber field missing" };
                            return new JsonResult(returnJsonData);
                        }
                    }

                    returnresult = validateCustomerRegistrationUpdate("Customer_address1_man", objdata);
                    if (returnresult != "")
                    {
                        try
                        {
                            if (Convert.ToString(data.address1).Trim() == "")
                            {
                                returnJsonData = new { response = false, responseCode = "02", data = returnresult };
                                return new JsonResult(returnJsonData);
                            }
                        }
                        catch
                        {
                            returnJsonData = new { response = false, responseCode = "02", data = "address1 field missing" };
                            return new JsonResult(returnJsonData);
                        }
                    }

                    returnresult = validateCustomerRegistrationUpdate("Customer_address2_man", objdata);
                    if (returnresult != "")
                    {
                        try
                        {
                            if (Convert.ToString(data.address2).Trim() == "")
                            {
                                returnJsonData = new { response = false, responseCode = "02", data = returnresult };
                                return new JsonResult(returnJsonData);
                            }
                        }
                        catch
                        {
                            returnJsonData = new { response = false, responseCode = "02", data = "address2 field missing" };
                            return new JsonResult(returnJsonData);
                        }
                    }

                    /*returnresult = validateCustomerRegistrationUpdate("Customer_country_man", objdata);
                    if (returnresult != "")
                    {
                        try
                        {
                            if (Convert.ToString(data.countryID).Trim() == "")
                            {
                                returnJsonData = new { response = false, responseCode = "02", data = returnresult };
                                return new JsonResult(returnJsonData);
                            }
                        }
                        catch
                        {
                            returnJsonData = new { response = false, responseCode = "02", data = "countryID field missing" };
                            return new JsonResult(returnJsonData);
                        }
                    }*/
                    returnresult = validateCustomerRegistrationUpdate("Customer_city_man", objdata);
                    if (returnresult != "")
                    {
                        try
                        {
                            if (Convert.ToString(data.cityID).Trim() == "")
                            {
                                returnJsonData = new { response = false, responseCode = "02", data = returnresult };
                                return new JsonResult(returnJsonData);
                            }
                        }
                        catch
                        {
                            returnJsonData = new { response = false, responseCode = "02", data = "cityID field missing" };
                            return new JsonResult(returnJsonData);
                        }
                    }

                    returnresult = validateCustomerRegistrationUpdate("provience_man", objdata);
                    if (returnresult != "")
                    {
                        try
                        {
                            if (Convert.ToString(data.provience).Trim() == "")
                            {
                                returnJsonData = new { response = false, responseCode = "02", data = returnresult };
                                return new JsonResult(returnJsonData);
                            }
                        }
                        catch
                        {
                            returnJsonData = new { response = false, responseCode = "02", data = "provience field missing" };
                            return new JsonResult(returnJsonData);
                        }
                    }

                    returnresult = validateCustomerRegistrationUpdate("Customer_nationality_man", objdata);
                    if (returnresult != "")
                    {
                        try
                        {
                            if (Convert.ToString(data.nationalityName).Trim() == "")
                            {
                                returnJsonData = new { response = false, responseCode = "02", data = returnresult };
                                return new JsonResult(returnJsonData);
                            }
                        }
                        catch
                        {
                            returnJsonData = new { response = false, responseCode = "02", data = "Nationality field missing" };
                            return new JsonResult(returnJsonData);
                        }
                    }
                } // Second step validation

                if (stepNumber == "3")
                {
                    
                    returnresult = validateCustomerRegistrationUpdate("Employement_status_man", objdata);
                    if (returnresult != "")
                    {
                        try
                        {
                            if (Convert.ToString(data.employementStatus).Trim() == "")
                            {
                                returnJsonData = new { response = false, responseCode = "02", data = returnresult };
                                return new JsonResult(returnJsonData);
                            }
                        }
                        catch
                        {
                            returnJsonData = new { response = false, responseCode = "02", data = "employementStatus field missing" };
                            return new JsonResult(returnJsonData);
                        }
                    }

                    returnresult = validateCustomerRegistrationUpdate("Customer_profession_man", objdata);
                    if (returnresult != "")
                    {
                        try
                        {
                            if (Convert.ToString(data.professionID).Trim() == "")
                            {
                                returnJsonData = new { response = false, responseCode = "02", data = returnresult };
                                return new JsonResult(returnJsonData);
                            }
                        }
                        catch
                        {
                            returnJsonData = new { response = false, responseCode = "02", data = "professionName field missing" };
                            return new JsonResult(returnJsonData);
                        }
                    }

                    returnresult = validateCustomerRegistrationUpdate("Company_name_man", objdata);
                    if (returnresult != "")
                    {
                        try
                        {
                            if (Convert.ToString(data.companyName).Trim() == "")
                            {
                                returnJsonData = new { response = false, responseCode = "02", data = returnresult };
                                return new JsonResult(returnJsonData);
                            }
                        }
                        catch
                        {
                            returnJsonData = new { response = false, responseCode = "02", data = "companyName field missing" };
                            return new JsonResult(returnJsonData);
                        }
                    }

                    // Parth changes for country of birth
                    returnresult = validateCustomerRegistrationUpdate("Countryof_Birth", objdata);
                    if (returnresult != "")
                    {
                        try
                        {
                            if (Convert.ToString(data.Countryof_Birth).Trim() == "")
                            {
                                returnJsonData = new { response = false, responseCode = "02", data = returnresult };
                                return new JsonResult(returnJsonData);
                            }
                        }
                        catch
                        {
                            data.Countryof_Birth = "";
                        }
                    }
                    //End Parth changes for country of birth 

                    returnresult = validateCustomerRegistrationUpdate("Heard_from_man", objdata);
                    if (returnresult != "")
                    {
                            try
                            {
                                if (Convert.ToString(data.heardFromEvent).Trim() == "")
                                {
                                    returnJsonData = new { response = false, responseCode = "02", data = returnresult };
                                    return new JsonResult(returnJsonData);
                                }
                            }
                            catch
                            {
                                returnJsonData = new { response = false, responseCode = "02", data = "heardFromEvent field missing" };
                                return new JsonResult(returnJsonData);
                           }
                    }

                    returnresult = validateCustomerRegistrationUpdate("Terms", objdata);
                    if (returnresult != "")
                    {
                        try
                        {
                            if (Convert.ToString(data.terms).Trim() == "")
                            {
                                returnJsonData = new { response = false, responseCode = "02", data = returnresult };
                                return new JsonResult(returnJsonData);
                            }
                        }
                        catch
                        {
                            returnJsonData = new { response = false, responseCode = "02", data = "terms field missing" };
                            return new JsonResult(returnJsonData);
                        }
                    }
                }


              

            }
            catch (Exception ex)
            {
                Model.ActivityLogTracker _objActivityLog = new Model.ActivityLogTracker();
                _objActivityLog.Activity = " Exception Customer update Error: " + ex.ToString() + " ";
                _objActivityLog.FunctionName = "updateCustomer";
                _objActivityLog.Transaction_ID = 0;
                _objActivityLog.WhoAcessed = 1;
                _objActivityLog.Branch_ID = 0;
                _objActivityLog.Client_ID = 0;

                Service.srvActivityLogTracker srvActivityLog = new Service.srvActivityLogTracker();
                int _i = srvActivityLog.Create(_objActivityLog, HttpContext);
                CompanyInfo.InsertErrorLogTracker(" Exception Customer update Error: " + ex.ToString(), 0, 0, 0, 0, "updateCustomer", Convert.ToInt32(data.Branch_ID), Convert.ToInt32(data.Client_ID), "updateCustomer", HttpContext);
                returnJsonData = new { response = false, responseCode = "01", data = "Field is missing." };
                return new JsonResult(returnJsonData);
            }
            #endregion

            #region registerCustomer
            // Register customer
            try
            {
                if (!SqlInjectionProtector.ReadJsonElementValues(objdata) || !SqlInjectionProtector.ReadJsonElementValuesScript(objdata))
                {
                    var errorResponse = new { response = false, responseCode = "02", data = "Invalid input detected." };
                    return new JsonResult(errorResponse) { StatusCode = 400 };
                }

                Customer obj = JsonConvert.DeserializeObject<Customer>(jsonData);
                obj.step_flag = Convert.ToInt32(data.step);
                
                obj.payout_countries = "";
                try
                {
                    obj.payout_countries = Convert.ToString(data.payout_countries);
                    if(obj.payout_countries !=  "")
                    {
                        if (obj.payout_countries.EndsWith(","))
                        {
                            obj.payout_countries = obj.payout_countries.Substring(0, obj.payout_countries.Length - 1);
                        }
                    }

                }
                catch(Exception ex) { }

                obj.Email = Convert.ToString(data.emailID).Trim();
                obj.Client_ID = Convert.ToInt32(data.clientID);
                //obj.Branch_ID = Convert.ToInt32(data.branchID);
                obj.Customer_ID = Convert.ToString(data.customerID).Trim();


                // Personal Details
                try  { obj.TitleId = Convert.ToInt32(data.title); } catch (Exception ex) { obj.TitleId = 0; }

                try { obj.Annual_salary_ID = Convert.ToInt32(data.Annual_salary_ID); } catch (Exception ex) { obj.Annual_salary_ID = 0; }

                try  { obj.Gender = Convert.ToString(data.gender).Trim(); } catch (Exception ex) { obj.Gender = ""; }
                try  { obj.FirstName = Convert.ToString(data.firstName).Trim(); } catch (Exception ex) { obj.FirstName = ""; }
                try  { obj.Middle_Name = Convert.ToString(data.middleName).Trim(); } catch (Exception ex) { obj.Middle_Name = ""; }
                try  { obj.LastName = Convert.ToString(data.lastName).Trim(); } catch (Exception ex) { obj.LastName = ""; }
                try  { obj.Birth_Date = Convert.ToString(data.dob).Trim(); } catch (Exception ex) { obj.Birth_Date = ""; }
                try { obj.birthPlace = Convert.ToString(data.birthPlace).trim(); } catch(Exception ex) { obj.birthPlace = ""; }
                //obj.Mobile_number_code = Convert.ToString(data.mobileCode).Trim();
                obj.Mobile_number_code = "";
                    obj.MobileNumber = "";
                    //obj.MobileNumber = Convert.ToString(data.mobile).Trim();
                    try  { obj.Phone_number_code = Convert.ToString(data.phoneCode).Trim(); } catch (Exception ex) { obj.Phone_number_code = ""; }
                 try  { obj.PhoneNumber = Convert.ToString(data.phone).Trim(); } catch (Exception ex) { obj.PhoneNumber = ""; }


 try
                {
                    if (obj.PhoneNumber != "" && obj.PhoneNumber != null)
                    {
                        bool validatePhoneNumber = IsFirst4DigitsSame(obj.PhoneNumber);
                        if (validatePhoneNumber)
                        {
                            returnJsonData = new { response = false, responseCode = "02", data = "Invalid phone number." };
                            return new JsonResult(returnJsonData);
                        }
                    }
                }
                catch (Exception ex) { }
                // Address Details
               try  {  obj.PostCode = Convert.ToString(data.postCode).Trim(); } catch (Exception ex) { obj.PostCode = ""; }
                try { obj.HouseNumber = Convert.ToString(data.houseNumber).Trim();} catch (Exception ex) { obj.HouseNumber = ""; }
                try { obj.AddressLine2 = Convert.ToString(data.address2).Trim();} catch (Exception ex) { obj.AddressLine2 = ""; }
                try { obj.Street = Convert.ToString(data.address1).Trim();} catch (Exception ex) { obj.Street = ""; }

                try {
                    obj.Country_Id = Convert.ToInt32(data.countryID);
                }
                catch (Exception ex)
                {
                    obj.Country_Id = 0;
                }
                try { obj.provience_id = Convert.ToInt32(data.provience); } catch (Exception ex) { obj.provience_id = 0; }
                try { obj.cityId = Convert.ToInt32(data.cityID); } catch (Exception ex) { obj.cityId = 0; }
                try { obj.birthPlace = Convert.ToString(data.birthPlace).Trim(); } catch (Exception ex) { obj.birthPlace = ""; }
                try
                {
                    obj.Employement_Status = Convert.ToInt32(data.employementStatus);
                }
                catch (Exception ex) { obj.Employement_Status = 0; }

                try { obj.professionId = Convert.ToInt32(data.professionId); } catch (Exception ex) { obj.professionId = 0; }

                try { obj.professionName = Convert.ToString(data.professionName).Trim();} catch (Exception ex) { obj.professionName = ""; }
                try { obj.CompanyName = Convert.ToString(data.companyName).Trim();} catch (Exception ex) { obj.CompanyName = ""; }
                try { obj.HeardFromId = Convert.ToInt32(data.heardFromID); } catch (Exception ex) { obj.HeardFromId = 0; }
                obj.HeardFrom = Convert.ToInt32("0");
                try { obj.HeardFromEvent = Convert.ToString(data.heardFromEvent).Trim(); } catch (Exception ex) { obj.HeardFromEvent = ""; }
                try { obj.Nationality_ID = Convert.ToInt32(data.nationalityID); } catch (Exception ex) { obj.Nationality_ID = 0; }
                try { obj.Nationality = Convert.ToString(data.nationalityName); } catch (Exception ex) { obj.Nationality = ""; }

                try {obj.chk_promo_mail = Convert.ToInt32(data.Chkpromomail); } catch (Exception ex) { obj.chk_promo_mail = 0; }
                try {obj.Agent_User_ID = Convert.ToString(data.agentUserID).Trim(); } catch (Exception ex) { obj.Agent_User_ID = "0"; }
                    try { obj.Referred_By_Agent = Convert.ToInt32(data.referredByAgent); } catch (Exception ex) { obj.Referred_By_Agent = 1; }
                try {obj.SourseOfRegistration = Convert.ToString(data.sourseOfRegistration).Trim(); } catch (Exception ex) { obj.SourseOfRegistration = ""; }


                            try
                {
                    obj.Customer_ID = Convert.ToInt32(CompanyInfo.Decrypt(Convert.ToString(obj.Customer_ID), true));
                }
                catch (Exception ex) {
                    returnJsonData = new { response = false, responseCode = "02", data = "Invalid Customer ID." };
                    return new JsonResult(returnJsonData);
                }
                try { obj.Captcha = data.Captcha; } catch (Exception ex) { obj.Captcha = ""; }

                try { obj.Countryof_Birth = Convert.ToString(data.Countryof_Birth); } catch (Exception ex) { obj.Countryof_Birth = ""; }

                if (!SqlInjectionProtector.ValidateObjectForSqlInjection(obj) || !SqlInjectionProtector.ValidateObjectForScriptSqlInjection(obj))
                {
                    return new JsonResult(returnJsonData) { StatusCode = StatusCodes.Status400BadRequest, Value = "Invalid input detected." };
                }

                Model.Customer _ObjCustomer = new Model.Customer();
                Service.srvCustomer srvCustomer = new Service.srvCustomer();
                response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_SUCCESS);
                _ObjCustomer = srvCustomer.updatecustomerdetails(obj, HttpContext);
                response.ObjData = _ObjCustomer;


                // send mail
                if (_ObjCustomer.Message == "success")
                {
                    #region "Send Mail"
                    Service.srvLogin srvLogin = new Service.srvLogin();
                    DataTable dt = new DataTable();
                    Model.Login objLogin = new Model.Login();
                    objLogin.UserName = obj.Email;
                    objLogin.Client_ID = obj.Client_ID;
                    DataTable dtc = CompanyInfo.get(objLogin.Client_ID, HttpContext);
                    dt = srvLogin.IsValidEmail(objLogin);
                    if (dt.Rows.Count > 0)
                    {
                        try
                        {
                            if (obj.step_flag == 3)
                            {
                                string email = objLogin.UserName;
                            string body = string.Empty, subject = string.Empty;
                            string body1 = string.Empty;
                            //string template = "";
                            HttpWebRequest httpRequest = null, httpRequest1 = null;
                            //DataTable dtc = CompanyInfo.get(objLogin.Client_ID);
                            if (dtc.Rows.Count > 0)
                            {
                                string company_name = Convert.ToString(dtc.Rows[0]["Company_Name"]);
                                string URL = Convert.ToString(dtc.Rows[0]["Company_URL_Admin"]);
                                string cust_url = Convert.ToString(dtc.Rows[0]["Company_URL_Customer"]);
                                //httpRequest = (HttpWebRequest)WebRequest.Create(URL + "Email/Customer-Registered-Success-Details.htm");
								httpRequest = (HttpWebRequest)WebRequest.Create(URL + "Email/customer-registration-last-step.html");
                                httpRequest.UserAgent = "Code Sample Web Client";
                                HttpWebResponse webResponse = (HttpWebResponse)httpRequest.GetResponse();
                                using (StreamReader reader = new StreamReader(webResponse.GetResponseStream()))
                                {
                                    body = reader.ReadToEnd();
                                }
                                body = body.Replace("[name]", obj.FirstName);
                                httpRequest1 = (HttpWebRequest)WebRequest.Create(URL + "Email/RegisteredSuccessMail.txt");
                                httpRequest1.UserAgent = "Code Sample Web Client";
                                HttpWebResponse webResponse1 = (HttpWebResponse)httpRequest1.GetResponse();
                                using (StreamReader reader = new StreamReader(webResponse1.GetResponseStream()))
                                {
                                    subject = reader.ReadLine();
                                }
                                string newsubject = company_name + " - " + subject + " - " + Convert.ToString(dt.Rows[0]["WireTransfer_ReferanceNo"]);
								newsubject = "Your " + company_name + " Account is Ready — Start Sending Money Today!";
                                //string msg = (string)CompanyInfo.Send_Mail(dtc, objLogin.UserName, body, newsubject, obj.Client_ID, obj.Branch_ID, "", "", "");


                                // Check zoho email activate or not
                                MySqlCommand cmdp_activethirdpartyAPI = new MySqlCommand("active_thirdparti_api");
                                cmdp_activethirdpartyAPI.CommandType = CommandType.StoredProcedure;
                                DataTable dtApi_activethirdpartyAPI = db_connection.ExecuteQueryDataTableProcedure(cmdp_activethirdpartyAPI);
                                string activeZohoAPI = "F"; string msg = "";

                                if (dtApi_activethirdpartyAPI.Rows.Count > 0)
                                {
                                    for (int ita = 0; ita < dtApi_activethirdpartyAPI.Rows.Count; ita++)
                                    {
                                        if (Convert.ToInt32(dtApi_activethirdpartyAPI.Rows[ita]["API_ID"]) == 6) { activeZohoAPI = "T"; break; }
                                    }
                                }
                                if (activeZohoAPI == "T")
                                {
                                    int zohoAPIID = 6;
                                    string zohoToken = CompanyInfo.generateZohoToken(zohoAPIID, HttpContext);
                                    string description = "";
                                    DataTable dtuserData = new DataTable();
                                    dtuserData.Columns.Add("firstname", typeof(string));
                                    dtuserData.Columns.Add("lastname", typeof(string));
                                    dtuserData.Columns.Add("emailaddress", typeof(string));
                                    dtuserData.Columns.Add("description", typeof(string));
                                    dtuserData.Columns.Add("street", typeof(string));
                                    dtuserData.Columns.Add("state", typeof(string));
                                    dtuserData.Columns.Add("city", typeof(string));
                                    DataRow dr = dtuserData.NewRow();
                                    dr["firstname"] = Convert.ToString(obj.FirstName).Trim();
                                    dr["lastname"] = Convert.ToString(obj.LastName).Trim();
                                    dr["emailaddress"] = Convert.ToString(obj.Email).Trim();
                                    dr["description"] = Convert.ToString(description).Trim();
                                    dr["street"] = Convert.ToString("").Trim(); // coupon_code value get here
                                    dr["state"] = ""; // expires_date value get here
                                    dr["city"] = Convert.ToString("").Trim();
                                    dtuserData.Rows.Add(dr);
                                    string leadId = ""; string updateRecordStatus = "F";

                                    leadId = CompanyInfo.getZohoLeadIdDetails(zohoToken, zohoAPIID, dtuserData, HttpContext);
                                    bool digitsOnly = leadId.All(char.IsDigit);

                                    if (digitsOnly)
                                    {
                                        updateRecordStatus = CompanyInfo.updateZohoLeadDetails(zohoToken, zohoAPIID, dtuserData, HttpContext);
                                    }
                                    if (updateRecordStatus == "F")
                                    {
                                        leadId = CompanyInfo.generateZohoLead(zohoToken, zohoAPIID, dtuserData, HttpContext);
                                    }

                                    DataTable dttemplateData = new DataTable();
                                    dttemplateData.Columns.Add("zohoid", typeof(int));
                                    dttemplateData.Columns.Add("leadid", typeof(string));
                                    dttemplateData.Columns.Add("zohoToken", typeof(string));
                                    dttemplateData.Columns.Add("templateName", typeof(string));
                                    DataRow drtem = dttemplateData.NewRow();
                                    drtem["zohoid"] = zohoAPIID;
                                    drtem["leadid"] = Convert.ToString(leadId).Trim();
                                    drtem["zohoToken"] = Convert.ToString(zohoToken).Trim();
                                    drtem["templateName"] = Convert.ToString("custregistrationtempid").Trim();

                                    dttemplateData.Rows.Add(drtem);
                                    msg = (string)CompanyInfo.Send_Mail_usingZoho(email, body, newsubject, obj.Client_ID, Convert.ToInt32(obj.Branch_ID), "discount Customer Mail", "", "", dttemplateData, HttpContext);
                                    if (msg == "NOT_ALLOWED")
                                    {
                                        msg = (string)CompanyInfo.Send_Mail(dtc, objLogin.UserName, body, newsubject, obj.Client_ID, obj.Branch_ID, "", "", "", HttpContext);
                                    }

                                }
                                else
                                {
                                    try
                                    {   // 7-10-2024  Email send changes
                                       // msg = (string)CompanyInfo.Send_Mail(dtc, objLogin.UserName, body, newsubject, obj.Client_ID, obj.Branch_ID, "", "", "", HttpContext);
                                    }
                                    catch (Exception ex) { }
                                }


                                response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_SUCCESS);
                                response.data = "success"; response.ObjData = _ObjCustomer;
                                response.ResponseCode = 0;
                                /*if (msg == "Success")
                                {
                                    Activity_login1 = "Successful registration email sent to " + objLogin.UserName + " ";                                    
                                    _ = CompanyInfo.InsertActivityLogDetailsasync(Activity_login1, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), 0, "Register_controller", Convert.ToInt32(obj.Branch_ID), 1, "Customer Registration", HttpContext);
                                }
                                else
                                {
                                    Activity_login1 = "Successful registration email not sent to " + objLogin.UserName + " " + msg;
                                    
                                    _ = CompanyInfo.InsertActivityLogDetailsasync(Activity_login1, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), 0, "Register_controller", Convert.ToInt32(obj.Branch_ID), 1, "Customer Registration", HttpContext);
                                }
                                */

                            }
                            else
                            {
                                Model.Customer obj1 = new Model.Customer(); obj1.Message = "Failed";
                                response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_TECHNICAL_ERROR);
                                response.ResponseMessage = "Something went wrong. Please try again later.";
                                response.ResponseCode = 2; response.ObjData = obj1;
                            }
                        }
                        }
                        catch (Exception ex)
                        {
                            CompanyInfo.InsertErrorLogTracker("Exception Customer while sendmail Error : " + ex.ToString(), 0, 0, 0, 0, "updateCustomer", Convert.ToInt32(data.Branch_ID), Convert.ToInt32(data.Client_ID), "", HttpContext);
                        }
                    }
                    else
                    {
                        Model.Customer obj1 = new Model.Customer(); obj1.Message = "Failed";
                        response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_TECHNICAL_ERROR);
                        response.ResponseMessage = "This Email is not registered with us.";
                        response.ResponseCode = 3; response.ObjData = obj1;

                    }
                    #endregion

                    #region Verification Mail
                    try //Send Verification mail
                    {

                        if (obj.Perm_status == 0)
                        {                            
                            string msg1 = "";
                            if (obj.step_flag == 3 && _ObjCustomer.Message == "success") {
                                obj.UserName = Convert.ToString(data.emailID).Trim();
                                try
                                {
                                    string StepVerification = CompanyInfo.Send_RegistrationCompleted_mail(obj, HttpContext);
                                }
                                catch { }
                                
                                msg1 = CompanyInfo.Send_verify_mail(obj, HttpContext);
                            }
                            //Model.ActivityLogTracker _objActivityLog = new Model.ActivityLogTracker();
                            try
                            {

                                if (msg1 == "Success")
                                {
                                    Activity_login1 = "Verification email sent to " + objLogin.UserName + " " + msg1;
                                    //stattus61 = (int)CompanyInfo.InsertActivityLogDetails(Activity_login1, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), 0, "Register_controller", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "CreateCustomer");
                                    _ = CompanyInfo.InsertActivityLogDetailsasync(Activity_login1, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), 0, "Register_controller", Convert.ToInt32(obj.Branch_ID), 1, "CreateCustomer", HttpContext);
                                }
                            }
                            catch (Exception ex)
                            {
                                Activity_login1 = "Verification email not sent to " + objLogin.UserName + " " + msg1;
                                //stattus61 = (int)CompanyInfo.InsertActivityLogDetails(Activity_login1, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), 0, "Register_controller", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "CreateCustomer");
                                _ = CompanyInfo.InsertActivityLogDetailsasync(Activity_login1, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), 0, "Register_controller", Convert.ToInt32(obj.Branch_ID), 1, "CreateCustomer", HttpContext);
                            }
                        }
                    }
                    catch (Exception ex) { }
                    #endregion

                    List<Dictionary<string, object>> customerData = new List<Dictionary<string, object>>();
                    Dictionary<string, object> customer = new Dictionary<string, object>();


                    if (response.ResponseCode == 0)
                    {
                        customer["customerID"] = _ObjCustomer.Customer_ID;
                        customer["customerUsername"] = _ObjCustomer.Email;
                        if (obj.step_flag == 3)
                            customer["status"] = "Thank you. We will now start with your ID Verification.";
                        else if (obj.step_flag == 2)
                            customer["status"] = "Address details submitted! Keep going, few more taps.";
                        else if (obj.step_flag == 1)
                            customer["status"] = "Thanks for submitting your details. Now, let’s complete your address details.";


                        customerData.Add(customer);

                        returnJsonData = new { response = true, responseCode = "00", data = customerData };
                        return new JsonResult(returnJsonData);
                    }
                    else if (response.ResponseCode == 2)
                    {
                        customer["customerID"] = "";
                        customer["customerUsername"] = "";
                        customer["status"] = response.ResponseMessage;
                        customerData.Add(customer);
                        returnJsonData = new { response = false, responseCode = "02", data = customerData };
                        return new JsonResult(returnJsonData);
                    }
                    else if (response.ResponseCode == 3)
                    {
                        customer["customerID"] = "";
                        customer["customerUsername"] = "";
                        customer["status"] = response.ResponseMessage;
                        customerData.Add(customer);
                        returnJsonData = new { response = false, responseCode = "02", data = customerData };
                        return new JsonResult(returnJsonData);
                    }

                }
                // Pradip Code
                else if(_ObjCustomer.ExtraGBG == "AccountHoldFromGBG")// add by pradip HoldFromApiResultCode
                {
                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_SUCCESS);                    
                    response.data = "AccountHoldFromGBG";
                    response.ObjData = (_ObjCustomer.Message).ToString();
                    response.ResponseCode = 0;
                    returnJsonData = new { response = false, responseCode = "02", data = (_ObjCustomer.Message).ToString() };
                    return new JsonResult(returnJsonData);
                }
                else if(_ObjCustomer.ExtraGBG == "GBGChecksFailed")// add by pradip HoldFromApiResultCode NotYetSave
                {
                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_SUCCESS);                    
                    response.data = "GBGChecksFailed";
                    response.ObjData = (_ObjCustomer.Message).ToString();
                    response.ResponseCode = 0;
                    returnJsonData = new { response = false, responseCode = "02", data = (_ObjCustomer.Message).ToString() };
                    return new JsonResult(returnJsonData);
                }
                //End Pradip Code
                else if (_ObjCustomer.Message == "Invalid Captcha")
                {
                    returnJsonData = new { response = false, responseCode = "02", data = "Invalid Captcha" };
                    return new JsonResult(returnJsonData);
                }
                else
                {
                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_SUCCESS);
                    response.data = "Validation Failed";
                    response.ObjData = (_ObjCustomer.Message).ToString();
                    response.ResponseCode = 0;
                    returnJsonData = new { response = false, responseCode = "02", data = (_ObjCustomer.Message).ToString() };
                    return new JsonResult(returnJsonData);
                }

            }
            catch (Exception ex)
            {
                Model.ActivityLogTracker _objActivityLog = new Model.ActivityLogTracker();
                _objActivityLog.Activity = " Exception Customer update Error: " + ex.ToString() + " ";
                _objActivityLog.FunctionName = "updateCustomer";
                _objActivityLog.Transaction_ID = 0;
                _objActivityLog.WhoAcessed = 1;
                _objActivityLog.Branch_ID = 0;
                _objActivityLog.Client_ID = 0;

                Service.srvActivityLogTracker srvActivityLog = new Service.srvActivityLogTracker();
                int _i = srvActivityLog.Create(_objActivityLog, HttpContext);
                CompanyInfo.InsertErrorLogTracker("Exception Customer update Error: " + ex.ToString(), 0, 0, 0, 0, "updateCustomer", Convert.ToInt32(data.Branch_ID), Convert.ToInt32(data.Client_ID), "", HttpContext);
                returnJsonData = new { response = false, responseCode = "01", data = "Invalid request." };
                return new JsonResult(returnJsonData);
            }
            #endregion registerCustomer


            return returnJsonData;
        }

        [HttpGet]
        [Route("heard")]
        public IActionResult heardFrom(int clientID)
        {
            var returnJsonData = (dynamic)null;
            List<Model.HeardFrom> _lst = new List<Model.HeardFrom>();
            Model.response.WebResponse response = null;
            string Activity_login1 = "";
            CompanyInfo.InsertrequestLogTracker("heard full request body: " + clientID, 0, 0, 0, 0, "heardFrom", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
            try
            {
                Service.srvCommon srv = new Service.srvCommon();
                _lst = srv.Select_Heard_From();
                if (_lst != null)
                {
                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_SUCCESS);
                    response.data = "success";
                    response.ObjData = _lst;
                    response.ResponseCode = 0;

                    Service.ListtoDataTableConverter converter = new Service.ListtoDataTableConverter();
                    DataTable li1 = converter.ToDataTable(_lst);

                    string[] ColumnsToBeDeleted = { "deleteStatus", "login", "client_ID" };
                    foreach (string ColName in ColumnsToBeDeleted)
                    {
                        if (li1.Columns.Contains(ColName))
                            li1.Columns.Remove(ColName);
                    }

                    var heardFromData = li1.Rows.OfType<DataRow>()
                .Select(row => li1.Columns.OfType<DataColumn>()
                    .ToDictionary(col => col.ColumnName, c => row[c]));

                    foreach (DataColumn column in li1.Columns)
                    {
                        // Replace "OldColumnName" with the desired new name
                        column.ColumnName = column.ColumnName.Replace("Id", "heardFrom");
                        column.ColumnName = column.ColumnName.Replace("HeardFromOption", "heardFromOption");
                    }

                    returnJsonData = new { response = true, responseCode = "00", data = heardFromData };
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

                objError.Error = "Api : Select_Heard_From --" + ex.ToString();
                objError.Branch_ID = 1;
                objError.Date = DateTime.Now;
                objError.User_ID = 1;
                objError.Id = 1;
                objError.Function_Name = "heardFrom";
                Service.srvErrorLog srvError = new Service.srvErrorLog();
                srvError.Create(objError, HttpContext);
                CompanyInfo.InsertErrorLogTracker(" Exception Customer Configuration Error: " + ex.ToString(), 0, 0, 0, 0, "heardFrom", Convert.ToInt32(0), Convert.ToInt32(clientID), "", HttpContext);
            }

            returnJsonData = new { response = false, responseCode = "02", data = "Something goes wrong." };
            return new JsonResult(returnJsonData);
        }

        [HttpPost]
        [Route("geteditrequestcontact")]
        public IActionResult geteditrequestcontact([FromBody] JsonElement objdata)
        {
            HttpContext context = HttpContext;
            var validateJsonData = (dynamic)null;
            Model.response.WebResponse response = null;
            string jsonData = System.Text.Json.JsonSerializer.Serialize(objdata);
            dynamic data = JObject.Parse(jsonData);
            CompanyInfo.InsertrequestLogTracker("geteditrequestcontact full request body: " + JObject.Parse(jsonData), 0, 0, 0, 0, "geteditrequestcontact", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
            Model.Customer obj = new Model.Customer();
            try
            {
                if (!SqlInjectionProtector.ReadJsonElementValues(objdata) || !SqlInjectionProtector.ReadJsonElementValuesScript(objdata))
                {
                    var errorResponse = new { response = false, responseCode = "02", data = "Invalid input detected." };
                    return new JsonResult(errorResponse) { StatusCode = 400 };
                }

                DataTable li1 = new DataTable();
                obj.Client_ID = data.clientID;
                obj.Id = data.customerID;
                obj.Branch_ID = data.branchID;
                obj.Flag = data.flag;
                Service.srvCustomer srv = new Service.srvCustomer();
                List<Model.Customer> _lst = new List<Model.Customer>();
                _lst = srv.Geteditrequest(obj);

                validateJsonData = new { response = true, responseCode = "00", data = _lst };
                return new JsonResult(validateJsonData);

            }
            catch (Exception ex)
            {
                validateJsonData = new { response = false, responseCode = "01", data = "Invalid Request" };
                CompanyInfo.InsertErrorLogTracker(ex.ToString(), 0, 0, 0, 0, "geteditrequestcontact", 0, Convert.ToInt32(obj.Client_ID), "", HttpContext);
                return new JsonResult(validateJsonData);

            }

        }

        [HttpPost]
        [Route("updatecontactdetails")]
        public IActionResult updatecontactdetails([FromBody] JsonElement objdata)
        {
            HttpContext context = HttpContext;
            var returnJsonData = (dynamic)null;

            Model.response.WebResponse response = null;
            string jsonData = System.Text.Json.JsonSerializer.Serialize(objdata);
            dynamic data = JObject.Parse(jsonData);
            CompanyInfo.InsertrequestLogTracker("updatecontactdetails full request body: " + JObject.Parse(jsonData), 0, 0, 0, 0, "updatecontactdetails", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
            Customer obj1 = new Customer();
            try
            {
                if (!SqlInjectionProtector.ReadJsonElementValues(objdata) || !SqlInjectionProtector.ReadJsonElementValuesScript(objdata))
                {
                    var errorResponse = new { response = false, responseCode = "02", data = "Invalid input detected." };
                    return new JsonResult(errorResponse) { StatusCode = 400 };
                }

                List<Model.Customer> _lst = new List<Model.Customer>();

                obj1.Id = data.customerID;
                obj1.Phone_number_code = data.phoneNumberCode;
                obj1.Mobile_number_code = data.mobileNumberCode;
                obj1.Delete_Status = data.deleteStatus;
                obj1.cityId = data.cityID;
                obj1.Country_Id = data.countryID;
                obj1.PhoneNumber = data.phoneNumber;
                obj1.MobileNumber = data.mobileNumber;
                obj1.MobileNumber = data.mobileNumber;
                obj1.Email = data.Email;
                obj1.Client_ID = data.clientID;
                obj1.Branch_ID = data.branchID;
                obj1.chk_changed = data.checkStatus;

                // Register customer
                Model.Customer _ObjCustomer = new Model.Customer();
                Service.srvCustomer srvCustomer = new Service.srvCustomer();
                response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_SUCCESS);
                _ObjCustomer = srvCustomer.UpdatePhone(obj1, context);
                response.ObjData = _ObjCustomer;

                returnJsonData = new { response = true, responseCode = "00", data = response.ObjData };
                return new JsonResult(returnJsonData);
            }
            catch (Exception ex)
            {
                returnJsonData = new { response = false, responseCode = "01", data = "Invalid Request" };
                CompanyInfo.InsertErrorLogTracker("updatecontactdetails error: "+ex.ToString(), 0, 0, 0, 0, "updatecontactdetails", 0, Convert.ToInt32(obj1.Client_ID), "", HttpContext);
                return new JsonResult(returnJsonData);
            }

        }


        [HttpPost]
        [Route("updateaddress")]
        public IActionResult UpdateAddress([FromBody] JsonElement objdata)
        {
            HttpContext context = HttpContext;
            var returnJsonData = (dynamic)null;
            Model.response.WebResponse response = null;
            string jsonData = System.Text.Json.JsonSerializer.Serialize(objdata);
            dynamic data = JObject.Parse(jsonData);

            CompanyInfo.InsertrequestLogTracker("UpdateAddress full request body: " + JObject.Parse(jsonData), 0, 0, 0, 0, "updateaddress", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);

            if (!SqlInjectionProtector.ReadJsonElementValues(objdata) || !SqlInjectionProtector.ReadJsonElementValuesScript(objdata))
            {
                var errorResponse = new { response = false, responseCode = "02", data = "Invalid input detected." };
                return new JsonResult(errorResponse) { StatusCode = 400 };
            }

            List<Model.Customer> _lst = new List<Model.Customer>();
            Customer obj1 = new Customer();
            obj1.Id = data.ID;
            obj1.Email = data.emailID;
            obj1.DeleteStatus = data.deleteStatus;
            obj1.cityId = data.cityID;
            obj1.Country_Id = data.countryID;
            obj1.PostCode = data.postCode;
            obj1.AddressLine2 = data.addressLine;
            obj1.HouseNumber = data.houseNumber;
            obj1.Street = data.street;
            obj1.Client_ID = data.clientID;
            obj1.Branch_ID = data.branchID;

            Model.Customer _ObjCustomer = new Model.Customer();
            Service.srvCustomer srvCustomer = new Service.srvCustomer();
            response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_SUCCESS);
            _ObjCustomer = srvCustomer.UpdateAddress(obj1, context);
            response.ObjData = _ObjCustomer;

            if (response.ObjData != null)
            {
                if (_ObjCustomer.Message == "Request")
                {
                    response.ObjData = response.data;
                    response.ResponseCode = 00;
                    returnJsonData = new { response = true, responseCode = "00", data = "Update address request sent successfully." };
                }
                else if (_ObjCustomer.Message == "success")
                {
                    response.ObjData = response.data;
                    response.ResponseCode = 02;
                    returnJsonData = new { response = true, responseCode = "00", data = "Address Updated successfully." };
                }
                else
                {
                    response.ObjData = response.data;
                    response.ResponseCode = 02;
                    returnJsonData = new { response = false, responseCode = "02", data = "No changes detected." };
                }
            }
            else
            {
               response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_TECHNICAL_ERROR);
                response.ResponseMessage = "No Records Found.";
                response.ResponseCode = 0;
                response.ResponseCode = 02;
                returnJsonData = new { response = false, responseCode = "02", data = "No record updated." };
            }


            return new JsonResult(returnJsonData);
        }


        [HttpPost]
        [Route("geteditrequest")]
        public IActionResult geteditrequest([FromBody] JsonElement objdata)
        {
            HttpContext context = HttpContext;
            var returnJsonData = (dynamic)null; var validateJsonData = (dynamic)null;
            Model.response.WebResponse response = null;
            string jsonData = System.Text.Json.JsonSerializer.Serialize(objdata);
            dynamic data = JObject.Parse(jsonData);
            CompanyInfo.InsertrequestLogTracker("geteditrequest full request body: " + JObject.Parse(jsonData), 0, 0, 0, 0, "geteditrequest", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
            try
            {
                if (!SqlInjectionProtector.ReadJsonElementValues(objdata) || !SqlInjectionProtector.ReadJsonElementValuesScript(objdata))
                {
                    var errorResponse = new { response = false, responseCode = "02", data = "Invalid input detected." };
                    return new JsonResult(errorResponse) { StatusCode = 400 };
                }

                Model.Customer obj = new Model.Customer();

                obj.Client_ID = data.clientID;
                obj.Id = data.customerID;
                obj.Branch_ID = data.branchID;
                obj.Flag = data.flag;
                Service.srvCustomer srv = new Service.srvCustomer();
                DataTable li1 = srv.GeteditrequestALL(obj);
                //Model.Dashboard _Obj = new Model.Dashboard();
                if (li1 != null && li1.Rows.Count > 0)
                {
                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_SUCCESS);
                    response.data = "success";
                    response.ObjData = li1;
                    response.ResponseCode = 0;
                    var relationshipData = li1.Rows.OfType<DataRow>()
                .Select(row => li1.Columns.OfType<DataColumn>()
                    .ToDictionary(col => col.ColumnName, c => row[c]));

                    validateJsonData = new { response = true, responseCode = "00", data = relationshipData };
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
                Model.Beneficiary obj = new Model.Beneficiary();
                obj.Client_ID = data.clientID;
                response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_TECHNICAL_ERROR);
                response.sErrorExceptionText = ex.ToString();
                response.ResponseMessage = "Technical error";
                response.ResponseCode = 3;
                //Error Log Handled
                Model.ErrorLog objError = new Model.ErrorLog();
                objError.User = new Model.User();
                objError.Error = "Api : GeteditrequestALL --" + ex.ToString();
                objError.Date = DateTime.Now;
                objError.User_ID = 1;
                objError.Client_ID = obj.Client_ID;
                objError.Function_Name = "GeteditrequestAll";
                Service.srvErrorLog srvError = new Service.srvErrorLog();
                srvError.Create(objError, context);
                validateJsonData = new { response = false, responseCode = "02", data = "Invalid Request" };
                return new JsonResult(validateJsonData);
            }

        }

        [HttpPost]
        [Route("updateemail")]
        public IActionResult Updateemail([FromBody] JsonElement objdata)
        {
            HttpContext context = HttpContext;
            var returnJsonData = (dynamic)null;
            Model.response.WebResponse response = null;
            string jsonData = System.Text.Json.JsonSerializer.Serialize(objdata);
            dynamic data = JObject.Parse(jsonData);
            try { 
            
            CompanyInfo.InsertrequestLogTracker("Updateemail full request body: " + JObject.Parse(jsonData), 0, 0, 0, 0, "updateemail", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);

            if (!SqlInjectionProtector.ReadJsonElementValues(objdata) || !SqlInjectionProtector.ReadJsonElementValuesScript(objdata))
            {
                var errorResponse = new { response = false, responseCode = "02", data = "Invalid input detected." };
                return new JsonResult(errorResponse) { StatusCode = 400 };
            }

                List<Model.Customer> _lst = new List<Model.Customer>();
            Customer obj1 = new Customer();
            obj1.Id = data.ID;
            obj1.UserName = data.emailid;
            obj1.Client_ID = data.clientID;
            obj1.Branch_ID = data.branchID;
            obj1.Country_Id = data.countryId;

            Model.Customer _ObjCustomer = new Model.Customer();
            Service.srvCustomer srvCustomer = new Service.srvCustomer();
            response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_SUCCESS);
            _ObjCustomer = srvCustomer.UpdateEmail(obj1, context);
            response.ObjData = _ObjCustomer;
            if (_ObjCustomer.Message == "Validation Failed")
            {
                response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_SUCCESS);
                response.data = "Validation Failed";
                response.ObjData = (_ObjCustomer.Comment).ToString();
                //response.hasError = (_ObjCustomer.Message).ToString();
                //response.ObjData = response.data;
                response.ResponseCode = 0;
                returnJsonData = new { response = false, responseCode = "02", data = "Failed to update the email" };
            }
            else if (_ObjCustomer.Message == "success")
            {
                response.data = "success";
                try
                {
                    if (obj1.Perm_status == 0)
                    {
                        obj1.Customer_ID = obj1.Id;
                        Send_verify_mail(obj1);
                    }
                }
                catch (Exception ex)
                {
                }
                response.ResponseCode = 0;
                returnJsonData = new { response = true, responseCode = "00", data = "Email updated successfully" };
                }
            else
                if (_ObjCustomer.Message == "Request")
            {
                response.data = "Request";
                response.ResponseCode = 0;
                    returnJsonData = new { response = true, responseCode = "00", data = "Email update request sent successfully." };
                }
            else
            {
                response.ResponseMessage = "Something went wrong. Please try again later.";
                response.ResponseCode = 1;
                response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_SUCCESS);
                response.ResponseCode = 0;
                    returnJsonData = new { response = false, responseCode = "02", data = "Failed to update the email" };
                     
            }


            return new JsonResult(returnJsonData);
            }
            catch (Exception ex)
            {
                Model.Beneficiary obj = new Model.Beneficiary();
                obj.Client_ID = Convert.ToInt32( data.clientID);
                response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_TECHNICAL_ERROR);
                response.sErrorExceptionText = ex.ToString();
                response.ResponseMessage = "Technical error";
                response.ResponseCode = 3;
                //Error Log Handled
                Model.ErrorLog objError = new Model.ErrorLog();
                objError.User = new Model.User();
                objError.Error = "Api : Updateemail --" + ex.ToString();
                objError.Date = DateTime.Now;
                objError.User_ID = 1;
                objError.Client_ID = obj.Client_ID;
                objError.Function_Name = "Updateemail";
                Service.srvErrorLog srvError = new Service.srvErrorLog();
                srvError.Create(objError, context);
                returnJsonData = new { response = false, responseCode = "02", data = "Failed to update the email" };
                return new JsonResult(returnJsonData);
            }

        }

        [HttpPost]
        [Authorize]
        [Route("verifyphone")]
        public IActionResult VerifyPhone([FromBody] JsonElement objdata)
        {
            HttpContext context = HttpContext;
            var returnJsonData = (dynamic)null;
            Model.response.WebResponse response = null;
            string jsonData = System.Text.Json.JsonSerializer.Serialize(objdata);
            dynamic data = JObject.Parse(jsonData);
            CompanyInfo.InsertrequestLogTracker("verifyphone full request body: " + JObject.Parse(jsonData), 0, 0, 0, 0, "VerifyPhone", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
            try
            {
                if (!SqlInjectionProtector.ReadJsonElementValues(objdata) || !SqlInjectionProtector.ReadJsonElementValuesScript(objdata))
                {
                    var errorResponse = new { response = false, responseCode = "02", data = "Invalid input detected." };
                    return new JsonResult(errorResponse) { StatusCode = 400 };
                }

                Model.Customer obj = new Model.Customer();

                obj.Id = data.customerID;
                obj.OTP = data.otp;
                obj.DeleteStatus = data.deleteStatus;
                obj.Client_ID = data.clientID;
                obj.Branch_ID = data.branchID;                                
                obj.PhoneNumber = data.phone;
                obj.MobileNumber = data.mobileNumber;
                obj.UserName = data.userName;

                if (data.countryId == "") { data.countryId = 0; }
                if (data.cityId == "") { data.cityId = 0; }
                obj.Country_Id = data.countryId;
                obj.cityId  = data.cityId;

                Model.Customer _ObjCustomer = new Model.Customer();
                Service.srvCustomer srvCustomer = new Service.srvCustomer();
                response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_SUCCESS);
                _ObjCustomer = srvCustomer.VerifyPhone(obj);
                response.ObjData = _ObjCustomer;
                
                // Response messages
                response.ResponseMessage = "";
                if (_ObjCustomer.Message == null || _ObjCustomer.Message == "" || _ObjCustomer.Message == "Validation Failed") { 
                    response.ResponseMessage = "Validation Failed";
                    returnJsonData = new { response = false, responseCode = "02", data = response.ResponseMessage };
                }
                else if ( _ObjCustomer.Message == "Request") { response.ResponseMessage = "Verify Phone Number Request Sent successfully.";
                    returnJsonData = new { response = true, responseCode = "00", data = response.ResponseMessage };
                }
                else if (_ObjCustomer.Message == "OTPdoesnotmatch") { response.ResponseMessage = "Security Code is not valid.";
                    returnJsonData = new { response = false, responseCode = "02", data = response.ResponseMessage };
                }
                else if (_ObjCustomer.Message == "success") { response.ResponseMessage = "Mobile Number verified successfully.";
                    returnJsonData = new { response = true, responseCode = "00", data = response.ResponseMessage };
                }
                                
                return new JsonResult(returnJsonData);
            }
            catch (Exception ex)
            {
                Model.Beneficiary obj = new Model.Beneficiary();
                obj.Client_ID = Convert.ToInt32(data.clientID);
                response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_TECHNICAL_ERROR);
                response.sErrorExceptionText = ex.ToString();
                response.ResponseMessage = "Technical error";
                response.ResponseCode = 3;                
                Model.ErrorLog objError = new Model.ErrorLog();
                objError.User = new Model.User();
                objError.Error = "Api : verifyphone --" + ex.ToString();
                objError.Date = DateTime.Now;
                objError.User_ID = 1;
                objError.Client_ID = obj.Client_ID;
                objError.Function_Name = "VerifyPhone";
                Service.srvErrorLog srvError = new Service.srvErrorLog();
                srvError.Create(objError, context);
                returnJsonData = new { response = false, responseCode = "01", data = "Invalid Request" };
                return new JsonResult(returnJsonData);
            }
        }



        [HttpPost]
        [Authorize]
        [Route("sendcode")]
        public IActionResult sendcode([FromBody] JsonElement objdata)
        {
            HttpContext context = HttpContext;
            var returnJsonData = (dynamic)null;
            Model.response.WebResponse response = null;
            string jsonData = System.Text.Json.JsonSerializer.Serialize(objdata);
            dynamic data = JObject.Parse(jsonData);
            CompanyInfo.InsertrequestLogTracker("sendcode full request body: " + JObject.Parse(jsonData), 0, 0, 0, 0, "sendcode", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
            try
            {
                if (!SqlInjectionProtector.ReadJsonElementValues(objdata) || !SqlInjectionProtector.ReadJsonElementValuesScript(objdata))
                {
                    var errorResponse = new { response = false, responseCode = "02", data = "Invalid input detected." };
                    return new JsonResult(errorResponse) { StatusCode = 400 };
                }

                Model.Customer obj = new Model.Customer();

                obj.Client_ID = data.clientID;
                obj.Branch_ID = data.branchID;
                obj.Id = data.customerID;

                Random generator = new Random();
                String r = generator.Next(0, 1000000).ToString("D6");

                obj.OTP = r; //data.otp;
                obj.DeleteStatus = data.deleteStatus;
                obj.PhoneNumber = data.phone;
                obj.MobileNumber= data.mobileNumber;
                obj.UserName = data.userName;

                Service.srvCustomer srv = new Service.srvCustomer();
                int a = srv.Mobile_Counter(obj);
                if (a == 1)
                {
                    Model.Customer _ObjCustomer = new Model.Customer();
                    Service.srvCustomer srvCustomer = new Service.srvCustomer();
                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_SUCCESS);
                    _ObjCustomer = srvCustomer.SendOTP(obj);
                    response.ObjData = _ObjCustomer;
                    response.ResponseMessage = _ObjCustomer.Message;

                    if(response.ResponseMessage == null) { response.ResponseMessage = "OTP sending failed"; }

                    returnJsonData = new { response = true, responseCode = "00", data = response.ResponseMessage };
                    return new JsonResult(returnJsonData);
                }
                else
                {
                    Model.Customer _ObjCustomer = new Model.Customer();
                    Service.srvCustomer srvCustomer = new Service.srvCustomer();
                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_SUCCESS);
                    response.ObjData = _ObjCustomer;
                    response.ResponseMessage = "Exceed";
                    response.ResponseMessage = "You have reached maximum number of Send requests for the day. Please contact Support Team for any assistance required.";
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

                objError.Error = "Api : sendcode --" + ex.ToString();
                objError.Branch.Id = 1;
                objError.Date = DateTime.Now;
                objError.User.Id = 1;
                objError.Id = 1;
                objError.Function_Name = "sendcode";
                Service.srvErrorLog srvError = new Service.srvErrorLog();
                srvError.Create(objError, context);
                returnJsonData = new { response = false, responseCode = "01", data = "Invalid request" };
                return new JsonResult(returnJsonData);
            }

        }

        [HttpPost]
        [Authorize]
        [Route("getbasecountrytime")]
        public IActionResult Get_basecountry_time([FromBody] JsonElement obj)
        {
            Model.response.WebResponse response = null;
            HttpContext context = HttpContext;
            var returnJsonData = (dynamic)null;
            string jsonData = System.Text.Json.JsonSerializer.Serialize(obj);
            dynamic data = JObject.Parse(jsonData);
            _ = Task.Run(() => CompanyInfo.InsertrequestLogTracker("getbasecountrytime full request body: " + JObject.Parse(jsonData), 0, 0, 0, 0, "Get_basecountry_time", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext));
            try
            {
                if (!SqlInjectionProtector.ReadJsonElementValues(obj) || !SqlInjectionProtector.ReadJsonElementValuesScript(obj))
                {
                    var errorResponse = new { response = false, responseCode = "02", data = "Invalid input detected." };
                    return new JsonResult(errorResponse) { StatusCode = 400 };
                }



                Customer objdata = JsonConvert.DeserializeObject<Customer>(jsonData);
                DataTable dt = new DataTable();
                dt.Columns.Add("Datetime", typeof(DateTime));
                DateTime date = Convert.ToDateTime(CompanyInfo.gettime(objdata.Client_ID, "0", objdata.Country_Id, context));
                dt.Rows.Add(date);
                string new_Datetime = Convert.ToString(date);

                response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_SUCCESS);
                response.data = new_Datetime;
                response.ObjData = dt;
                var dtData = dt.Rows.OfType<DataRow>()
                    .Select(row => dt.Columns.OfType<DataColumn>()
                    .ToDictionary(col => col.ColumnName, c => row[c].ToString()));
                response.ResponseCode = 0;
                returnJsonData = new { response = true, responseCode = "00", data = dtData };
            }
            catch (Exception ex)
            {
                response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_TECHNICAL_ERROR);
                response.sErrorExceptionText = ex.ToString();
                response.ResponseMessage = "Technical error";
                response.ResponseCode = 3;
                //Error Log Handled
                Model.ErrorLog objError = new Model.ErrorLog();
                objError.User = new Model.User();
                objError.Error = "Api : Get_basecountry_time --" + ex.ToString();
                objError.Date = DateTime.Now;
                objError.User_ID = 1;
                objError.Client_ID = 1;
                objError.Function_Name = "Get_basecountry_time";
                Service.srvErrorLog srvError = new Service.srvErrorLog();
                srvError.Create(objError, context);

                returnJsonData = new { response = false, responseCode = "01", data = "Invalid request" };
            }
            return new JsonResult(returnJsonData);
        }

        [HttpPost]
        [Authorize]
        [Route("getdeleteacc")]
        public IActionResult getdeleteacc([FromBody] JsonElement objdata)
        {
            List<Model.Customer> _lst = new List<Model.Customer>();
            HttpContext context = HttpContext;
            var returnJsonData = (dynamic)null; var validateJsonData = (dynamic)null;
            Model.response.WebResponse response = null;
            string jsonData = System.Text.Json.JsonSerializer.Serialize(objdata);
            dynamic data = JObject.Parse(jsonData);
            CompanyInfo.InsertrequestLogTracker("getdeleteacc full request body: " + JObject.Parse(jsonData), 0, 0, 0, 0, "getdeleteacc", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);

            try
                {
                if (!SqlInjectionProtector.ReadJsonElementValues(objdata) || !SqlInjectionProtector.ReadJsonElementValuesScript(objdata))
                {
                    var errorResponse = new { response = false, responseCode = "02", data = "Invalid input detected." };
                    return new JsonResult(errorResponse) { StatusCode = 400 };
                }

                Model.Customer obj = new Model.Customer();

                obj.Client_ID = data.clientID;                
                obj.Branch_ID = data.branchID;
                obj.Id = data.customerID;
                obj.whereclause = data.whereclause;
                Service.srvCustomer srv = new Service.srvCustomer();
                    _lst = srv.Getdeleteacc(obj);
                //returnJsonData = new { response = true, responseCode = "00", data = _lst };
                returnJsonData = new { response = true, responseCode = "00", data = "Account Delete Request Send Successfully" };
                return new JsonResult(returnJsonData);
            }
                catch (Exception ex)
                {
                    Model.ErrorLog objError = new Model.ErrorLog();
                    objError.User = new Model.User();
                    objError.Branch = new Model.Branch();
                    objError.Client = new Model.Client();

                    objError.Error = "Api : getdeleteacc --" + ex.ToString();
                    objError.Branch.Id = 1;
                    objError.Date = DateTime.Now;
                    objError.User.Id = 1;
                    objError.Id = 1;
                    objError.Function_Name = "getdeleteacc";
                    Service.srvErrorLog srvError = new Service.srvErrorLog();
                    srvError.Create(objError, context);
                returnJsonData = new { response = false, responseCode = "01", data = "Invalid request" };
                return new JsonResult(returnJsonData);
            }
                
            }

        [HttpPost]
        
        [Route("checkscheme")]
        public IActionResult CheckScheme([FromBody] JsonElement objdata)
        {
            List<Model.Customer> _lst = new List<Model.Customer>();
            HttpContext context = HttpContext;
            var returnJsonData = (dynamic)null; var validateJsonData = (dynamic)null;
            Model.response.WebResponse response = null;
            string jsonData = System.Text.Json.JsonSerializer.Serialize(objdata);
            dynamic data = JObject.Parse(jsonData);
            CompanyInfo.InsertrequestLogTracker("checkscheme full request body: " + JObject.Parse(jsonData), 0, 0, 0, 0, "CheckScheme", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);

            try
            {
                if (!SqlInjectionProtector.ReadJsonElementValues(objdata) || !SqlInjectionProtector.ReadJsonElementValuesScript(objdata))
                {
                    var errorResponse = new { response = false, responseCode = "02", data = "Invalid input detected." };
                    return new JsonResult(errorResponse) { StatusCode = 400 };
                }

                Customer obj = JsonConvert.DeserializeObject<Customer>(jsonData);
               
                Service.srvDiscount srv = new Service.srvDiscount();
                _lst = srv.Check_ReferalScheme(obj);
                if (_lst != null)
                {
                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_SUCCESS);
                    response.data = "success";
                    response.ObjData = _lst;
                    response.ResponseCode = 0;

                    Service.ListtoDataTableConverter converter = new Service.ListtoDataTableConverter();
                    DataTable li1 = converter.ToDataTable(_lst);

                    var relationshipData = li1.Rows.OfType<DataRow>()
               .Select(row => li1.Columns.OfType<DataColumn>()
                   .ToDictionary(col => col.ColumnName, c => row[c].ToString()));

                    validateJsonData = new { response = true, responseCode = "00", data = relationshipData };
                    return new JsonResult(validateJsonData);
                }
                else
                {
                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_TECHNICAL_ERROR);
                    response.ResponseMessage = "No Records Found.";
                    response.ResponseCode = 0;
                    returnJsonData = new { response = false, responseCode = "02", data = "No Records Found." };
                    return new JsonResult(returnJsonData);
                }
            }
            catch (Exception ex)
            {
                Model.ErrorLog objError = new Model.ErrorLog();
                objError.User = new Model.User();
                objError.Branch = new Model.Branch();
                objError.Client = new Model.Client();

                objError.Error = "Api : checkscheme --" + ex.ToString();
                objError.Branch.Id = 1;
                objError.Date = DateTime.Now;
                objError.User.Id = 1;
                objError.Id = 1;
                objError.Function_Name = "CheckScheme";
                Service.srvErrorLog srvError = new Service.srvErrorLog();
                srvError.Create(objError, context);
                returnJsonData = new { response = false, responseCode = "01", data = "Invalid request" };
                return new JsonResult(returnJsonData);
            }
        }

        [HttpPost]
        [Route("gettransactionsearch")]

        public IActionResult GetTransactionSearch([FromBody] JsonElement obj1)
        {
            HttpContext context = HttpContext;
            string json = System.Text.Json.JsonSerializer.Serialize(obj1);
            CompanyInfo.InsertrequestLogTracker("GetTransactionSearch full request body: " + JObject.Parse(json), 0, 0, 0, 0, "gettransactionsearch", 0, 0, "", HttpContext);

            dynamic data = JObject.Parse(json);
            DataTable dt = new DataTable();
            object responseData;

            try
            {
                string encryptData = data.txnReferenceNo;
                string decryptedData = CompanyInfo.Decrypt(encryptData, true);

                string txnReferenceNo = decryptedData.Split(' ')[0];
                string dateStr = decryptedData.Substring(txnReferenceNo.Length + 1);
                int Client_ID = data.ClientId;
                DateTime requestDateTime = DateTime.ParseExact(dateStr, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                DateTime expiryTime = requestDateTime.AddMinutes(15);
                DateTime currentTime = Convert.ToDateTime(CompanyInfo.gettime(Client_ID, context)); // get current time from server

                if (currentTime > expiryTime)
                {
                    responseData = new { response = false, responseCode = "03", data = "This link has expired" };
                    return new JsonResult(responseData);
                }

                // Fetch transaction data
                Service.srvCustomer srvTxn = new Service.srvCustomer();
                dt = srvTxn.GetTransactionSearch(txnReferenceNo);

                if (dt != null && dt.Rows.Count > 0)
                {
                    string tranSign = dt.Rows[0]["Tran_sign"].ToString();
                    if (!string.IsNullOrEmpty(tranSign))
                    {
                        responseData = new { response = false, responseCode = "02", data = "Transaction sign already uploaded against this customer's transaction" };
                        return new JsonResult(responseData);
                    }

                    var resultData = dt.Rows.OfType<DataRow>()
                        .Select(row => dt.Columns.OfType<DataColumn>()
                            .ToDictionary(col => col.ColumnName, col => row[col]?.ToString()));

                    responseData = new { response = true, responseCode = "00", data = resultData };
                }
                else
                {
                    responseData = new { response = false, responseCode = "02", data = "No transactions found." };
                }
            }
            catch (Exception ex)
            {
                string activity = $"Api:- gettransactionsearch Exception: {ex}";
                CompanyInfo.InsertErrorLogTracker(activity, 0, 0, 0, 0, "gettransactionsearch", 0, 0, "", context);

                responseData = new { response = false, responseCode = "99", data = "Technical error occurred." };
            }

            return new JsonResult(responseData);
        }





        [HttpPost]
        [Route("updatecusttxnsign")]
        public IActionResult UpdateCustTxnSign([FromBody] JsonElement objdata)
        {
            HttpContext context = HttpContext;
            var returnJsonData = (dynamic)null;
            var validateJsonData = (dynamic)null;
            string jsonData = System.Text.Json.JsonSerializer.Serialize(objdata);
            dynamic data = JObject.Parse(jsonData);

            CompanyInfo.InsertrequestLogTracker("updatecusttxnsign full request body: " + JObject.Parse(jsonData), 0, 0, 0, 0, "UpdateCustTxnSign", 0, 0, "", context);

            try
            {
                // SQL Injection Prevention
                var filteredObj = new JsonObject();

                // Populate filteredObj with all properties except 'Image'
                foreach (var prop in objdata.EnumerateObject())
                {
                    if (!prop.NameEquals("Image"))
                    {
                        filteredObj[prop.Name] = JsonValue.Create(prop.Value.ToString());
                    }
                }

                // Validate the filtered object for SQL injection
                JsonElement filteredJsonElement = System.Text.Json.JsonSerializer.SerializeToElement(filteredObj);

                // Perform SQL injection checks
                if (!SqlInjectionProtector.ReadJsonElementValues(filteredJsonElement) ||
                    !SqlInjectionProtector.ReadJsonElementValuesScript(filteredJsonElement))
                {
                    return new JsonResult(new
                    {
                        response = false,
                        responseCode = "02",
                        data = "Invalid input detected."
                    })
                    { StatusCode = 400 };
                }

                // Deserialize JSON to Model.Transaction
                Model.Transaction objTransaction = JsonConvert.DeserializeObject<Model.Transaction>(jsonData);

                // Decrypt TransactionReference
                string encryptedData = objTransaction.TransactionReference;
                string decryptedData = CompanyInfo.Decrypt(encryptedData, true);

                string txnReferenceNo = decryptedData.Split(' ')[0];
                string dateStr = decryptedData.Substring(txnReferenceNo.Length + 1);

                DateTime requestTime = DateTime.ParseExact(dateStr, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                DateTime expiryTime = requestTime.AddMinutes(15);
                int Client_ID = data.ClientId;
                DateTime currentTime = Convert.ToDateTime(CompanyInfo.gettime(Client_ID, context));

                if (currentTime > expiryTime)
                {
                    return new JsonResult(new
                    {
                        response = false,
                        responseCode = "03",
                        data = "This link has expired"
                    });
                }

                // Call update service
                objTransaction.TransactionReference = txnReferenceNo;
                string msg = string.Empty;
                Service.srvCustomer srvTxn = new Service.srvCustomer();
                msg = srvTxn.updateCustTxnSign(objTransaction);

                if (!string.IsNullOrWhiteSpace(msg))
                {
                    validateJsonData = new
                    {
                        response = true,
                        responseCode = "00",
                        data = "Transaction signature updated successfully."
                    };
                }
                else
                {
                    validateJsonData = new
                    {
                        response = false,
                        responseCode = "02",
                        data = "Failed to update transaction signature."
                    };
                }

                return new JsonResult(validateJsonData);
            }
            catch (Exception ex)
            {
                // Log the exception
                Model.ErrorLog objError = new Model.ErrorLog();
                objError.User = new Model.User();
                objError.Branch = new Model.Branch();
                objError.Client = new Model.Client();

                objError.Error = "Api : updatecusttxnsign --" + ex.ToString();
                objError.Branch.Id = 1;
                objError.Date = DateTime.Now;
                objError.User.Id = 1;
                objError.Id = 1;
                objError.Function_Name = "UpdateCustTxnSign";
                Service.srvErrorLog srvError = new Service.srvErrorLog();
                srvError.Create(objError, context);

                returnJsonData = new { response = false, responseCode = "01", data = "Invalid request" };
                return new JsonResult(returnJsonData);
            }
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public string Send_verify_mail(Model.Customer obj)
        {
            try
            {
                Service.srvCustomer srv = new Service.srvCustomer();
                HttpContext context = HttpContext;
                //obj.Customer_ID = obj.Id;
                int a = srv.Email_Counter(obj);
                if (a == 1)
                {
                    Service.srvLogin srvLogin = new Service.srvLogin();
                    DataTable dt = new DataTable();
                    Model.Login obj1 = new Model.Login();
                    obj1.UserName = obj.UserName;
                    obj1.Client_ID = obj.Client_ID;

                    CompanyInfo.InsertrequestLogTracker("Send_verify_mail values: " + Convert.ToString(obj1.UserName) + " and obj1.Client_ID:" + obj1.Client_ID, 0, 0, 0, 0, "beneficiaryconfiguration", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);

                    dt = srvLogin.IsValidEmail(obj1);

                    CompanyInfo.InsertrequestLogTracker("Send_verify_mail count dt values: " + dt.Rows.Count, 0, 0, 0, 0, "beneficiaryconfiguration", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);

                    Model.response.WebResponse response = null;
                    string email = obj.UserName;
                    obj.UserName = email;
                    //string subject = string.Empty;
                    string msg = string.Empty;
                    string body = string.Empty, subject = string.Empty;
                    string body1 = string.Empty;
                    string template = "";
                    HttpWebRequest httpRequest = null, httpRequest1 = null;
                    DataTable dtc = CompanyInfo.get(obj.Client_ID, context);

                    CompanyInfo.InsertrequestLogTracker("Send_verify_mail count values: " + dtc.Rows.Count, 0, 0, 0, 0, "beneficiaryconfiguration", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);


                    if (dtc.Rows.Count > 0)
                    {

                        string URL = Convert.ToString(dtc.Rows[0]["Company_URL_Admin"]);
                        string cust_url = Convert.ToString(dtc.Rows[0]["Company_URL_Customer"]);
                        httpRequest = (HttpWebRequest)WebRequest.Create(URL + "Email/verify-email.html");
                        httpRequest.UserAgent = "Code Sample Web Client";
                        HttpWebResponse webResponse = (HttpWebResponse)httpRequest.GetResponse();
                        using (StreamReader reader = new StreamReader(webResponse.GetResponseStream()))
                        {
                            body = reader.ReadToEnd();
                        }
                        body = body.Replace("[name]", Convert.ToString(dt.Rows[0]["First_Name"])); //Convert.ToString(dt.Rows[0]["full_name"]));
                        string ref_no = Convert.ToString(dt.Rows[0]["WireTransfer_ReferanceNo"]);
                        string str = CompanyInfo.Encrypt1(ref_no);
                        body = body.Replace("[link]", cust_url + "verify-email?reference=" + str + ""); //body = body.Replace("[link]", cust_url + "verify-email.html?reference=" + ref_no + "");
                        httpRequest1 = (HttpWebRequest)WebRequest.Create(URL + "Email/verify-email.txt");
                        httpRequest1.UserAgent = "Code Sample Web Client";
                        HttpWebResponse webResponse1 = (HttpWebResponse)httpRequest1.GetResponse();
                        using (StreamReader reader = new StreamReader(webResponse1.GetResponseStream()))
                        {
                            subject = reader.ReadLine();
                        }
                        msg = (string)CompanyInfo.Send_Mail(dtc, obj.UserName, body, subject, obj.Client_ID, obj.Branch_ID, "", "", "", HttpContext);
                        if (msg == "Success")
                        {
                            response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_SUCCESS);
                            response.data = "success";
                            response.ResponseCode = 0;
                            Model.ActivityLogTracker _objActivityLog = new Model.ActivityLogTracker();
                            _objActivityLog.Activity = "Email Verify mail sent to " + obj.UserName + " ";
                            _objActivityLog.FunctionName = "Send_verify_mail";
                            _objActivityLog.Transaction_ID = 0;
                            _objActivityLog.WhoAcessed = 0;
                            _objActivityLog.Branch_ID = obj.Branch_ID;
                            _objActivityLog.Client_ID = obj.Client_ID;
                            Service.srvActivityLogTracker srvActivityLog = new Service.srvActivityLogTracker();
                            //int _i = srvActivityLog.Create(_objActivityLog);
                        }
                        else
                        {
                            response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_TECHNICAL_ERROR);
                            response.ResponseMessage = "Something went wrong. Please try again later.";
                            response.ResponseCode = 1;
                            response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_SUCCESS);
                            response.data = "success";
                            response.ResponseCode = 0;
                            Model.ActivityLogTracker _objActivityLog = new Model.ActivityLogTracker();
                            _objActivityLog.Activity = "Email Verify mail not sent to " + obj.UserName + " ";
                            _objActivityLog.FunctionName = "Send_verify_mail";
                            _objActivityLog.Transaction_ID = 0;
                            _objActivityLog.WhoAcessed = 0;
                            _objActivityLog.Branch_ID = obj.Branch_ID;
                            _objActivityLog.Client_ID = obj.Client_ID;
                            Service.srvActivityLogTracker srvActivityLog = new Service.srvActivityLogTracker();
                            // int _i = srvActivityLog.Create(_objActivityLog);
                        }
                    }
                    else
                    {
                        response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_TECHNICAL_ERROR);
                        response.ResponseMessage = "Something went wrong. Please try again later.";
                        response.ResponseCode = 2;
                    }

                    return msg;
                }
                else
                {
                    return "failed";
                }
            }
            catch(Exception egc)
            {
                CompanyInfo.InsertrequestLogTracker("Send_verify_mail Error: " + egc.ToString(), 0, 0, 0, 0, "Send_verify_mail", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
                return "failed";
            }
        }


       

    }


   }

