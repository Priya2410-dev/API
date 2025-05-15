using Microsoft.AspNetCore.Mvc;
using Auth0.ManagementApi.Models;
using Calyx_Solutions.Model;
using Calyx_Solutions.Service;
using Microsoft.Ajax.Utilities;
using Microsoft.VisualBasic;
using MySql.Data.MySqlClient;
using MySql.Data.MySqlClient.Memcached ;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Text.Json;
using System.Web;
using static Google.Apis.Requests.BatchRequest;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Net.Http.Headers;
using System.Web.Helpers;
using Microsoft.Extensions.Configuration;
using System.Web.Http.Controllers;
using System.ComponentModel.DataAnnotations;
using iTextSharp.text.pdf;
using System.Security.Principal;
using WebGrease.Css.Extensions;
 
using System.Linq;
using System.Text.RegularExpressions;
using Borland.Vcl;
 
using static iTextSharp.text.pdf.AcroFields;
using System;
using iTextSharp.text.pdf.parser;
using MySqlConnector;
using System.Web.Configuration;
using System.Xml.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Calyx_Solutions.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BeneficiaryController : ControllerBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;
        
        public BeneficiaryController(IConfiguration configuration, IHttpContextAccessor httpContextAccessor )
        {
            this._configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            
        }

        [HttpPost]
        [Authorize]
        [Route("configuration")]
     
        public async Task<JsonResult> beneficiaryconfiguration([FromBody] JsonElement objdata)
        {
            var returnJsonData = (dynamic)null;
            Model.response.WebResponse response = null;
            string jsonData = System.Text.Json.JsonSerializer.Serialize(objdata);
            dynamic data = JObject.Parse(jsonData);
            await Task.Run(() => CompanyInfo.InsertrequestLogTracker("configuration full request body: " + JObject.Parse(jsonData), 0, 0, 0, 0, "beneficiaryconfiguration", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext));
            #region validateData
            try
            {
                if (data.clientID == "" || data.clientID == null)
                {
                    returnJsonData = new { response = false, responseCode = "02", data = "Invalid Client ID." };
                    return new JsonResult(returnJsonData);
                }
                else if (data.paymentCollectionTypeID == "" || data.paymentCollectionTypeID == null)
                {
                    returnJsonData = new { response = false, responseCode = "02", data = "Invalid Collection Type." };
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

                Beneficiary obj = JsonConvert.DeserializeObject<Beneficiary>(jsonData);

                obj.Collection_type_Id = data.paymentCollectionTypeID;
                obj.Client_ID = data.clientID;
                obj.Branch_ID = data.branchID;
                obj.Country_ID = data.countryID;
                obj.Beneficiary_Country_ID = obj.Country_ID;

                //if (!SqlInjectionProtector.ReadJsonElementValues(objdata) || !SqlInjectionProtector.ReadJsonElementValuesScript(objdata))
                //{
                //    var errorResponse = new { response = false, responseCode = "02", data = "Invalid input detected." };
                //    return new JsonResult(errorResponse) { StatusCode = 400 };
                //}
                bool sqlCheck = await Task.Run(() => SqlInjectionProtector.ReadJsonElementValues(objdata) && SqlInjectionProtector.ReadJsonElementValuesScript(objdata));

                if (!sqlCheck)
                {
                    var errorResponse = new { response = false, responseCode = "02", data = "Invalid input detected." };
                    return new JsonResult(errorResponse) { StatusCode = 400 };
                }

                using (MySqlConnector.MySqlConnection con = new MySqlConnector.MySqlConnection(_configuration.GetConnectionString("connectionString")))
                {
                    await con.OpenAsync();
                    MySqlConnector.MySqlCommand cmd = new MySqlConnector.MySqlCommand("SP_Beneficiary_Configuration");
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection = con;
                    cmd.Parameters.AddWithValue("_Country_ID", obj.Beneficiary_Country_ID);
                    cmd.Parameters.AddWithValue("_Collection_Type", obj.Collection_type_Id);
                    //db_connection.ExecuteNonQueryProcedure(cmd);
                    //DataTable dtbenfconfig = db_connection.ExecuteQueryDataTableProcedure(cmd);
                    //await Task.Run(() => db_connection.ExecuteNonQueryProcedure(cmd));
                    DataTable dtbenfconfig = await Task.Run(() => db_connection.ExecuteQueryDataTableProcedure(cmd));

                    List<Dictionary<string, object>> confiurations = new List<Dictionary<string, object>>();

                    Dictionary<string, object> confiuration = new Dictionary<string, object>();

                    if (dtbenfconfig != null && dtbenfconfig.Rows.Count > 0)
                    {
                        // Beneficiary Name
                        #region Professional_Details
                        if (Convert.ToString(dtbenfconfig.Rows[0]["Beneficiary_Name"]) == "0")
                        {
                            confiuration["beneficiaryNameVisible"] = true;
                            confiuration["beneficiaryNameMandetory"] = true;
                        }
                        else if (Convert.ToString(dtbenfconfig.Rows[0]["Beneficiary_Name"]) == "1")
                        {
                            confiuration["beneficiaryNameVisible"] = true;
                            confiuration["beneficiaryNameMandetory"] = false;
                        }
                        else if (Convert.ToString(dtbenfconfig.Rows[0]["Beneficiary_Name"]) == "2")
                        {
                            confiuration["beneficiaryNameVisible"] = false;
                            confiuration["beneficiaryNameMandetory"] = false;
                        }

                        // Relation
                        if (Convert.ToString(dtbenfconfig.Rows[0]["Beneficiary_relation"]) == "0")
                        {
                            confiuration["beneficiaryRelationVisible"] = true;
                            confiuration["beneficiaryRelationMandetory"] = true;
                        }
                        else if (Convert.ToString(dtbenfconfig.Rows[0]["Beneficiary_relation"]) == "1")
                        {
                            confiuration["beneficiaryRelationVisible"] = true;
                            confiuration["beneficiaryRelationMandetory"] = false;
                        }
                        else if (Convert.ToString(dtbenfconfig.Rows[0]["Beneficiary_relation"]) == "2")
                        {
                            confiuration["beneficiaryRelationVisible"] = false;
                            confiuration["beneficiaryRelationMandetory"] = false;
                        }

                        // "Beneficiary Address"
                        if (Convert.ToString(dtbenfconfig.Rows[0]["Beneficiary_Address"]) == "0")
                        {
                            confiuration["beneficiaryAddressVisible"] = true;
                            confiuration["beneficiaryAddressMandetory"] = true;

                        }
                        else if (Convert.ToString(dtbenfconfig.Rows[0]["Beneficiary_Address"]) == "1")
                        {
                            confiuration["beneficiaryAddressVisible"] = true;
                            confiuration["beneficiaryAddressMandetory"] = false;
                        }
                        else if (Convert.ToString(dtbenfconfig.Rows[0]["Beneficiary_Address"]) == "2")
                        {
                            confiuration["beneficiaryAddressVisible"] = false;
                            confiuration["beneficiaryAddressMandetory"] = false;
                        }


                        // Beneficiary Postcode
                        try
                        {
                            if (Convert.ToString(dtbenfconfig.Rows[0]["Beneficiary_postcode"]) == "0")
                            {
                                confiuration["beneficiarypostcodeVisible"] = true;
                                confiuration["beneficiarypostcodeMandetory"] = true;

                            }
                            else if (Convert.ToString(dtbenfconfig.Rows[0]["Beneficiary_postcode"]) == "1")
                            {
                                confiuration["beneficiarypostcodeVisible"] = true;
                                confiuration["beneficiarypostcodeMandetory"] = false;
                            }
                            else
                            {
                                confiuration["beneficiarypostcodeVisible"] = false;
                                confiuration["beneficiarypostcodeMandetory"] = false;
                            }
                        }
                        catch (Exception egx) {
                            confiuration["beneficiarypostcodeVisible"] = false;
                            confiuration["beneficiarypostcodeMandetory"] = false;
                        }

                        // Beneficiary State
                        try
                        {
                            if (Convert.ToString(dtbenfconfig.Rows[0]["Beneficiary_state"]) == "0")
                            {
                                confiuration["beneficiarystateVisible"] = true;
                                confiuration["beneficiarystateMandetory"] = true;

                            }
                            else if (Convert.ToString(dtbenfconfig.Rows[0]["Beneficiary_state"]) == "1")
                            {
                                confiuration["beneficiarystateVisible"] = true;
                                confiuration["beneficiarystateMandetory"] = false;
                            }
                            else
                            {
                                confiuration["beneficiarystateVisible"] = false;
                                confiuration["beneficiarystateMandetory"] = false;
                            }
                        }
                        catch (Exception egx)
                        {
                            confiuration["beneficiarystateVisible"] = false;
                            confiuration["beneficiarystateMandetory"] = false;
                        }

                        // Beneficiary City                        
                        if (Convert.ToString(dtbenfconfig.Rows[0]["Beneficiary_City"]) == "0")
                        {
                            confiuration["beneficiaryCityVisible"] = true;
                            confiuration["beneficiaryCityMandetory"] = true;
                        }
                        else if (Convert.ToString(dtbenfconfig.Rows[0]["Beneficiary_City"]) == "1")
                        {
                            confiuration["beneficiaryCityVisible"] = true;
                            confiuration["beneficiaryCityMandetory"] = false;
                        }
                        else if (Convert.ToString(dtbenfconfig.Rows[0]["Beneficiary_City"]) == "2")
                        {
                            confiuration["beneficiaryCityVisible"] = false;
                            confiuration["beneficiaryCityMandetory"] = false;
                        }

                        // Beneficiary Mobile                        
                        if (Convert.ToString(dtbenfconfig.Rows[0]["Beneficiary_Mobile"]) == "0")
                        {
                            confiuration["beneficiaryMobileVisible"] = true;
                            confiuration["beneficiaryMobileMandetory"] = true;
                        }
                        else if (Convert.ToString(dtbenfconfig.Rows[0]["Beneficiary_Mobile"]) == "1")
                        {
                            confiuration["beneficiaryMobileVisible"] = true;
                            confiuration["beneficiaryMobileMandetory"] = false;
                        }
                        else if (Convert.ToString(dtbenfconfig.Rows[0]["Beneficiary_Mobile"]) == "2")
                        {
                            confiuration["beneficiaryMobileVisible"] = false;
                            confiuration["beneficiaryMobileMandetory"] = false;
                        }

                        if (Convert.ToString(dtbenfconfig.Rows[0]["Range_check_mobile"]) == "0" && Convert.ToString(dtbenfconfig.Rows[0]["Beneficiary_Mobile"]) != "2")
                        {   //Fixed                            
                            confiuration["beneficiaryMobileLength"] = Convert.ToInt16(dtbenfconfig.Rows[0]["mobile_max"]);
                        }
                        else if (Convert.ToString(dtbenfconfig.Rows[0]["Range_check_mobile"]) == "1" && Convert.ToString(dtbenfconfig.Rows[0]["Beneficiary_Mobile"]) != "2")
                        {   //Custom
                            confiuration["beneficiaryMobileMinLength"] = Convert.ToInt16(dtbenfconfig.Rows[0]["mobile_min"]);
                            confiuration["beneficiaryMobileMaxLength"] = Convert.ToInt16(dtbenfconfig.Rows[0]["mobile_max"]);
                        }

                        //Beneficiary DOB
                        try
                        {
                            if (Convert.ToString(dtbenfconfig.Rows[0]["DateOf_Birth"]) == "0")
                            {
                                confiuration["beneficiaryDOBVisible"] = true;
                                confiuration["beneficiaryDOBMandetory"] = true;
                            }
                            else if (Convert.ToString(dtbenfconfig.Rows[0]["DateOf_Birth"]) == "1")
                            {
                                confiuration["beneficiaryDOBVisible"] = true;
                                confiuration["beneficiaryDOBMandetory"] = false;
                            }
                            else
                            {
                                confiuration["beneficiaryDOBVisible"] = false;
                                confiuration["beneficiaryDOBMandetory"] = false;
                            }
                        }
                        catch {
                            confiuration["beneficiaryDOBVisible"] = false;
                            confiuration["beneficiaryDOBMandetory"] = false;
                        }

                        //Beneficiary Gender
                        try
                        {
                            if (Convert.ToString(dtbenfconfig.Rows[0]["Beneficiary_Gender"]) == "0")
                            {
                                confiuration["beneficiaryGenderVisible"] = true;
                                confiuration["beneficiaryGenderMandetory"] = true;
                            }
                            else if (Convert.ToString(dtbenfconfig.Rows[0]["Beneficiary_Gender"]) == "1")
                            {
                                confiuration["beneficiaryGenderVisible"] = true;
                                confiuration["beneficiaryGenderMandetory"] = false;
                            }
                            else
                            {
                                confiuration["beneficiaryGenderVisible"] = false;
                                confiuration["beneficiaryGenderMandetory"] = false;
                            }
                        }
                        catch {
                            confiuration["beneficiaryGenderVisible"] = false;
                            confiuration["beneficiaryGenderMandetory"] = false;
                        }

                        #endregion Professional_Details

                        #region Collection_Type_Details  

                        if (obj.Collection_type_Id == 3)
                        {
                            // Mobile Provider
                            if (Convert.ToString(dtbenfconfig.Rows[0]["mobile_provider"]) == "0")
                            {
                                confiuration["beneficiaryMobileProviderVisible"] = true;
                                confiuration["beneficiaryMobileProviderMandetory"] = true;
                            }
                            else if (Convert.ToString(dtbenfconfig.Rows[0]["mobile_provider"]) == "0")
                            {
                                confiuration["beneficiaryMobileProviderVisible"] = false;
                                confiuration["beneficiaryMobileProviderMandetory"] = false;
                            }
                        }
                        #endregion Collection_Type_Details

                        if (obj.Collection_type_Id == 10)
                        {
                            if (Convert.ToString(dtbenfconfig.Rows[0]["wallet_provider"]) == "0")
                            {
                                confiuration["beneficiarywalletProvider"] = true;
                                confiuration["beneficiarywalletProviderMandetory"] = true;
                            }
                            if (Convert.ToString(dtbenfconfig.Rows[0]["wallet_provider"]) == "1")
                            {
                                confiuration["beneficiarywalletProvider"] = false;
                                confiuration["beneficiarywalletProviderMandetory"] = false;
                            }

                            if (Convert.ToString(dtbenfconfig.Rows[0]["Wallet_Id_vis"]) == "1")
                            {
                                confiuration["beneficiarywalletIdVisible"] = false;
                                confiuration["beneficiarywalletIdMandetory"] = false;
                            }
                            if (Convert.ToString(dtbenfconfig.Rows[0]["Wallet_Id_vis"]) == "0")
                            {
                                confiuration["beneficiarywalletIdVisible"] = true;
                                if (Convert.ToString(dtbenfconfig.Rows[0]["Range_check_wallet"]) == "0")
                                {
                                    confiuration["beneficiarywalletIdfixed"] = true;
                                    confiuration["beneficiarywalletlength"] = Convert.ToInt16(dtbenfconfig.Rows[0]["Wallet_Id_length"]);
                                }
                                if (Convert.ToString(dtbenfconfig.Rows[0]["Range_check_wallet"]) == "1")
                                {
                                    confiuration["beneficiarywalletIdfixed"] = false;
                                    confiuration["beneficiarywalletminlength"] = Convert.ToInt16(dtbenfconfig.Rows[0]["Wallet_Min_Length"]);
                                    confiuration["beneficiarywalletmaxlength"] = Convert.ToInt16(dtbenfconfig.Rows[0]["Wallet_Id_length"]);
                                }
                            }


                            if (Convert.ToString(dtbenfconfig.Rows[0]["Wallet_Id_vis"]) == "0" && Convert.ToString(dtbenfconfig.Rows[0]["Wallet_Id_man"]) == "0")
                            {
                                confiuration["beneficiarywalletIdMandetory"] = true;
                            }
                            else if (Convert.ToString(dtbenfconfig.Rows[0]["Wallet_Id_vis"]) == "0" && Convert.ToString(dtbenfconfig.Rows[0]["Wallet_Id_man"]) == "1")
                            {
                                confiuration["beneficiarywalletIdMandetory"] = false;
                            }



                        }


                        #region Bank_Details 

                        if (obj.Collection_type_Id == 1)
                        {

                            //Account Number
                            if (Convert.ToString(dtbenfconfig.Rows[0]["ShowAccount_No"]) == "0")
                            {
                                confiuration["beneficiaryAccountVisible"] = true;
                            }
                            else
                            {
                                confiuration["beneficiaryAccountVisible"] = false;
                            }

                            if (Convert.ToString(dtbenfconfig.Rows[0]["Verify_Account_no"]) == "0")
                            {
                                confiuration["beneficiaryAccountMandetory"] = true;
                            }
                            else
                            {
                                confiuration["beneficiaryAccountMandetory"] = false;
                            }

                            if (Convert.ToString(dtbenfconfig.Rows[0]["Range_check_accno"]) == "1" && Convert.ToString(dtbenfconfig.Rows[0]["ShowAccount_No"]) == "0")
                            {
                                confiuration["beneficiaryAccountFixedLength"] = false;
                                confiuration["beneficiaryAccountMinLength"] = Convert.ToInt16(dtbenfconfig.Rows[0]["Acc_no_min_length"]);
                                confiuration["beneficiaryAccountMaxLength"] = Convert.ToInt16(dtbenfconfig.Rows[0]["Acc_no_length"]);
                            }
                            else if (Convert.ToString(dtbenfconfig.Rows[0]["Range_check_accno"]) == "0" && Convert.ToString(dtbenfconfig.Rows[0]["ShowAccount_No"]) == "0")
                            {
                                confiuration["beneficiaryAccountFixedLength"] = true;
                                confiuration["beneficiaryAccountLength"] = Convert.ToInt16(dtbenfconfig.Rows[0]["Acc_no_length"]);
                            }

                            if (Convert.ToString(dtbenfconfig.Rows[0]["Acc_Lable"]).Trim() != "" && Convert.ToString(dtbenfconfig.Rows[0]["ShowAccount_No"]) == "0")
                            {
                                confiuration["AccountShowLable"] = Convert.ToString(dtbenfconfig.Rows[0]["Acc_Lable"]).Trim();
                            }

                            try
                            {
                                #region Confirm_Account_Number
                                //Confirm Account Number
                                if (Convert.ToString(dtbenfconfig.Rows[0]["ConfirmAccNo_vis"]) == "0")
                                {
                                    confiuration["beneficiaryConfirmAccountVisible"] = true;
                                }
                                else
                                {
                                    confiuration["beneficiaryConfirmAccountVisible"] = false;
                                }

                                if (Convert.ToString(dtbenfconfig.Rows[0]["ConfirmAccNo_man"]) == "0")
                                {
                                    confiuration["beneficiaryConfirmAccountMandetory"] = true;
                                }
                                else
                                {
                                    confiuration["beneficiaryConfirmAccountMandetory"] = false;
                                }
                                if (Convert.ToString(dtbenfconfig.Rows[0]["Range_check_accno"]) == "1" && Convert.ToString(dtbenfconfig.Rows[0]["ConfirmAccNo_vis"]) == "0")
                                {
                                    confiuration["beneficiaryConfirmAccountFixedLength"] = false;
                                    confiuration["beneficiaryConfirmAccountMinLength"] = Convert.ToInt16(dtbenfconfig.Rows[0]["Acc_no_min_length"]);
                                    confiuration["beneficiaryConfirmAccountMaxLength"] = Convert.ToInt16(dtbenfconfig.Rows[0]["Acc_no_length"]);
                                }
                                else if (Convert.ToString(dtbenfconfig.Rows[0]["Range_check_accno"]) == "0" && Convert.ToString(dtbenfconfig.Rows[0]["ConfirmAccNo_vis"]) == "0")
                                {
                                    confiuration["beneficiaryConfirmAccountFixedLength"] = true;
                                    confiuration["beneficiaryConfirmAccountLength"] = Convert.ToInt16(dtbenfconfig.Rows[0]["Acc_no_length"]);
                                }
                                if (Convert.ToString(dtbenfconfig.Rows[0]["ConfirmAccNo_Lable"]).Trim() != "" && Convert.ToString(dtbenfconfig.Rows[0]["ConfirmAccNo_vis"]) == "0")
                                {
                                    confiuration["ConfirmAccountShowLable"] = Convert.ToString(dtbenfconfig.Rows[0]["ConfirmAccNo_Lable"]).Trim();
                                }
                                #endregion Confirm_Account_Number
                            }
                            catch (Exception ex)
                            {
                            }

                            //IFSC Code
                            if (Convert.ToString(dtbenfconfig.Rows[0]["IFSC"]) == "0")
                            {
                                confiuration["beneficiaryIFSCVisible"] = true;
                            }
                            else
                            {
                                confiuration["beneficiaryIFSCVisible"] = false;
                            }

                            if (Convert.ToString(dtbenfconfig.Rows[0]["IFSC_Mandatory"]) == "0")
                            {
                                confiuration["beneficiaryIFSCMandetory"] = true;
                            }
                            else
                            {
                                confiuration["beneficiaryIFSCMandetory"] = false;
                            }

                            if (Convert.ToString(dtbenfconfig.Rows[0]["Range_check_ifsc"]) == "1" && Convert.ToString(dtbenfconfig.Rows[0]["IFSC"]) == "0")
                            {
                                confiuration["beneficiaryIFSCFixedLength"] = false;
                                confiuration["beneficiaryIFSCMinLength"] = Convert.ToInt16(dtbenfconfig.Rows[0]["Ifsc_no_min_length"]);
                                confiuration["beneficiaryIFSCMaxLength"] = Convert.ToInt16(dtbenfconfig.Rows[0]["Ifsc_no_length"]);
                            }
                            else if (Convert.ToString(dtbenfconfig.Rows[0]["Range_check_ifsc"]) == "0" && Convert.ToString(dtbenfconfig.Rows[0]["IFSC"]) == "0")
                            {
                                confiuration["beneficiaryIFSCFixedLength"] = true;
                                confiuration["beneficiaryIFSCLength"] = Convert.ToInt16(dtbenfconfig.Rows[0]["Ifsc_no_length"]);
                            }

                            if (Convert.ToString(dtbenfconfig.Rows[0]["IFSC_Lable"]).Trim() != "" && Convert.ToString(dtbenfconfig.Rows[0]["IFSC"]) == "0")
                            {
                                confiuration["IFSCShowLable"] = Convert.ToString(dtbenfconfig.Rows[0]["IFSC_Lable"]).Trim();
                            }

                            // Bank Name
                            if (Convert.ToString(dtbenfconfig.Rows[0]["Bank_Name"]) == "0")
                            {
                                confiuration["beneficiaryBankNameVisible"] = true;
                                confiuration["beneficiaryBankNameMandetory"] = true;
                            }
                            else
                            {
                                confiuration["beneficiaryBankNameVisible"] = false;
                                confiuration["beneficiaryBankNameMandetory"] = false;
                            }

                            // Bank Code
                            if (Convert.ToString(dtbenfconfig.Rows[0]["Bank_Code"]) == "0")
                            {
                                confiuration["beneficiaryBankCodeVisible"] = true;
                                confiuration["beneficiaryBankCodeMandetory"] = true;
                            }
                            else
                            {
                                confiuration["beneficiaryBankCodeVisible"] = false;
                                confiuration["beneficiaryBankCodeMandetory"] = false;
                            }

                            //Branch Name                            
                            if (Convert.ToString(dtbenfconfig.Rows[0]["Branch"]) == "0")
                            {
                                confiuration["beneficiaryBranchVisible"] = true;
                                confiuration["beneficiaryBranchMandetory"] = true;
                            }
                            else
                            {
                                confiuration["beneficiaryBranchVisible"] = false;
                                confiuration["beneficiaryBranchMandetory"] = false;
                            }

                            //Branch Code                            
                            if (Convert.ToString(dtbenfconfig.Rows[0]["Branch_code_man"]) == "0")
                            {
                                confiuration["beneficiaryBranchCodeMandetory"] = true;
                            }
                            else
                            {
                                confiuration["beneficiaryBranchCodeMandetory"] = false;
                            }

                            if (Convert.ToString(dtbenfconfig.Rows[0]["Branch_Code"]) == "0")
                            {
                                confiuration["beneficiaryBranchCodeVisible"] = true;
                            }
                            else
                            {
                                confiuration["beneficiaryBranchCodeVisible"] = false;
                            }

                            if (Convert.ToString(dtbenfconfig.Rows[0]["Branch_code_name"]) != "")
                            {
                                confiuration["beneficiaryBranchCodeShowLable"] = Convert.ToString(dtbenfconfig.Rows[0]["Branch_code_name"]);
                            }
                            else
                            {
                                confiuration["beneficiaryBranchCodeShowLable"] = "Branch Code";
                            }

                            //IBAN Code                            
                            if (Convert.ToString(dtbenfconfig.Rows[0]["IBAN_Mandatory"]) == "0")
                            {
                                confiuration["beneficiaryIBANMandetory"] = true;
                            }
                            else
                            {
                                confiuration["beneficiaryIBANMandetory"] = false;
                            }

                            if (Convert.ToString(dtbenfconfig.Rows[0]["IBAN_Status"]) == "0")
                            {
                                confiuration["beneficiaryIBANVisible"] = true;
                            }
                            else
                            {
                                confiuration["beneficiaryIBANVisible"] = false;
                            }

                            if (Convert.ToString(dtbenfconfig.Rows[0]["Range_check_Iban"]) == "1" && Convert.ToString(dtbenfconfig.Rows[0]["IBAN_Status"]) == "0")
                            {
                                confiuration["beneficiaryIBANFixedLength"] = false;
                                confiuration["beneficiaryIBANMinLength"] = Convert.ToInt16(dtbenfconfig.Rows[0]["IBAN_Min_Length"]);
                                confiuration["beneficiaryIBANMaxLength"] = Convert.ToInt16(dtbenfconfig.Rows[0]["IBAN_Length"]);
                            }
                            else if (Convert.ToString(dtbenfconfig.Rows[0]["Range_check_Iban"]) == "0" && Convert.ToString(dtbenfconfig.Rows[0]["IBAN_Status"]) == "0")
                            {
                                confiuration["beneficiaryIBANFixedLength"] = true;
                                confiuration["beneficiaryIBANLength"] = Convert.ToInt16(dtbenfconfig.Rows[0]["IBAN_Length"]);
                            }
                            //beneficiaryConfig["fieldID"] = "branchcode";
                            if (Convert.ToString(dtbenfconfig.Rows[0]["IBAN_Label"]).Trim() != "" && Convert.ToString(dtbenfconfig.Rows[0]["IBAN_Status"]) == "0")
                            {
                                confiuration["IBANShowLable"] = Convert.ToString(dtbenfconfig.Rows[0]["IBAN_Label"]).Trim();
                            }

                            //BIC Code
                            if (Convert.ToString(dtbenfconfig.Rows[0]["BIC_Status"]) == "0")
                            {
                                confiuration["beneficiaryBICCodeVisible"] = true;
                            }
                            else
                            {
                                confiuration["beneficiaryBICCodeVisible"] = false;
                            }

                            if (Convert.ToString(dtbenfconfig.Rows[0]["Validate_BIC"]) == "0")
                            {
                                confiuration["beneficiaryBICCodeMandetory"] = true;
                            }
                            else
                            {
                                confiuration["beneficiaryBICCodeMandetory"] = false;
                            }

                            if (Convert.ToString(dtbenfconfig.Rows[0]["Range_check_BIC"]) == "1" && Convert.ToString(dtbenfconfig.Rows[0]["BIC_Status"]) == "0")
                            {
                                confiuration["beneficiaryBICCodeFixedLength"] = false;
                                confiuration["beneficiaryBICCodeMinLength"] = Convert.ToInt16(dtbenfconfig.Rows[0]["BIC_Min_Length"]);
                                confiuration["beneficiaryBICCodeMaxLength"] = Convert.ToInt16(dtbenfconfig.Rows[0]["BIC_Length"]);
                            }
                            else if (Convert.ToString(dtbenfconfig.Rows[0]["Range_check_BIC"]) == "0" && Convert.ToString(dtbenfconfig.Rows[0]["BIC_Status"]) == "0")
                            {
                                confiuration["beneficiaryBICCodeFixedLength"] = true;
                                confiuration["beneficiaryBICCodeLength"] = Convert.ToInt16(dtbenfconfig.Rows[0]["BIC_Length"]);
                            }

                            if (Convert.ToString(dtbenfconfig.Rows[0]["BIC_Lable"]).Trim() != "" && Convert.ToString(dtbenfconfig.Rows[0]["BIC_Status"]) == "0")
                            {
                                confiuration["BICCodeShowLable"] = Convert.ToString(dtbenfconfig.Rows[0]["BIC_Lable"]).Trim();
                            }
                        }

                        #endregion Bank_Details

                        #region Transactions_Configuration

                        if (Convert.ToString(dtbenfconfig.Rows[0]["Trans_limit_man"]) == "0")
                        {
                            confiuration["beneficiaryTransactionLimitSetting"] = true;
                            confiuration["beneficiaryMinTransactionLimit"] = dtbenfconfig.Rows[0]["Trans_limit_min"];
                            confiuration["beneficiaryMaxTransactionLimit"] = dtbenfconfig.Rows[0]["Trans_limit_max"];
                            confiuration["beneficiaryTransactionLimitPerDay"] = dtbenfconfig.Rows[0]["Trans_lmt_perday"];
                            confiuration["beneficiaryTransactionLimitPerDayPerBeneficiary"] = dtbenfconfig.Rows[0]["Trans_lmtt_perday_benf"];
                            confiuration["beneficiaryTransactionLimitPeryearPerBeneficiary"] = dtbenfconfig.Rows[0]["Trans_lmt_peryear_benf"];
                            confiuration["beneficiaryDenomantionAmount"] = dtbenfconfig.Rows[0]["denomination_rate"];
                        }
                        else if (Convert.ToString(dtbenfconfig.Rows[0]["Trans_limit_man"]) == "1")
                        {
                            confiuration["beneficiaryTransactionLimitSetting"] = false;
                        }

                        #endregion Transactions_Configuration

                        #region Custom_Settings

                        if (data.deliveryTypeID > 0)
                        {
                            if (true)
                            {
                                Transaction objT = JsonConvert.DeserializeObject<Transaction>(jsonData);
                                objT.Client_ID = data.clientID;
                                objT.Country_ID = data.countryID;
                                objT.CollectionPoint_ID = data.paymentCollectionTypeID;
                                objT.DeliveryType_Id = data.deliveryTypeID;
                                Service.srvDeliveryType srv = new Service.srvDeliveryType();
                                //DataTable liDeliveryMap = srv.GetDeliveryMappingSettings(objT);
                                // Call database asynchronously using Task.Run()
                                DataTable liDeliveryMap = await Task.Run(() => srv.GetDeliveryMappingSettings(objT));
                                if (liDeliveryMap.Rows.Count > 0)
                                {
                                    confiuration["beneficiaryCustomSettings"] = true;
                                    confiuration["beneficiaryCustomSettingsTime"] = liDeliveryMap.Rows[0]["Time"];
                                    confiuration["beneficiaryCustomSettingsMinAmount"] = liDeliveryMap.Rows[0]["Min_Amount"];
                                    confiuration["beneficiaryCustomSettingsMaxAmount"] = liDeliveryMap.Rows[0]["Max_Amount"];
                                    confiuration["beneficiaryCustomSettingsWorkingDays"] = liDeliveryMap.Rows[0]["Woking_Days"];
                                    confiuration["beneficiaryCustomSettingsDeliveryTypeId"] = liDeliveryMap.Rows[0]["DeliveryType_ID"];
                                }
                                else
                                {
                                    confiuration["beneficiaryCustomSettings"] = false;
                                }

                            }
                        }
                        else
                        {
                            confiuration["beneficiaryCustomSettings"] = false;
                        }
                        #endregion Custom_Settings

                        #region IDUploadConfiguration

                        //ID Upload
                        if (Convert.ToString(dtbenfconfig.Rows[0]["Id_Upload_man"]) == "0")
                        {
                            confiuration["beneficiaryIdUploadMandatory"] = true;
                        }
                        else
                        {
                            confiuration["beneficiaryIdUploadMandatory"] = false;
                        }

                        //ID Number

                        if (Convert.ToString(dtbenfconfig.Rows[0]["ID_number_vis"]) == "0")
                        {
                            confiuration["beneficiaryIdNoVisibile"] = true;
                            confiuration["beneficiaryIdNoMandatory"] = true;
                        }
                        else if (Convert.ToString(dtbenfconfig.Rows[0]["ID_number_vis"]) == "1")
                        {
                            confiuration["beneficiaryIdNoVisibile"] = true;
                            confiuration["beneficiaryIdNoMandatory"] = false;
                        }
                        else if (Convert.ToString(dtbenfconfig.Rows[0]["ID_number_vis"]) == "2")
                        {
                            confiuration["beneficiaryIdNoVisibile"] = false;
                            confiuration["beneficiaryIdNoMandatory"] = false;
                        }

                        if (Convert.ToString(dtbenfconfig.Rows[0]["Range_ID_number"]) == "0")
                        {
                            confiuration["beneficiaryfixedID"] = true;
                            confiuration["beneficiaryCustomID"] = false;
                            confiuration["beneficiaryIDNumberminlength"] = Convert.ToInt32(dtbenfconfig.Rows[0]["ID_Number_max"]);
                            confiuration["beneficiaryIDNumbermaxlength"] = Convert.ToInt32(dtbenfconfig.Rows[0]["ID_Number_max"]);
                        }
                        else if (Convert.ToString(dtbenfconfig.Rows[0]["Range_ID_number"]) == "1")
                        {
                            confiuration["beneficiaryfixedID"] = false;
                            confiuration["beneficiaryCustomID"] = true;
                            confiuration["beneficiaryIDNumberminlength"] = Convert.ToInt32(dtbenfconfig.Rows[0]["ID_Number_min"]);
                            confiuration["beneficiaryIDNumbermaxlength"] = Convert.ToInt32(dtbenfconfig.Rows[0]["ID_Number_max"]);
                        }


                        //Issue date
                        if (Convert.ToString(dtbenfconfig.Rows[0]["Issue_date_vis"]) == "0")
                        {
                            confiuration["beneficiaryIssueDateVisibilty"] = true;
                            confiuration["beneficiaryIssueDateMandatory"] = true;

                        }
                        else if (Convert.ToString(dtbenfconfig.Rows[0]["Issue_date_vis"]) == "1")
                        {
                            confiuration["beneficiaryIssueDateVisibilty"] = true;
                            confiuration["beneficiaryIssueDateMandatory"] = false;

                        }
                        else if (Convert.ToString(dtbenfconfig.Rows[0]["Issue_date_vis"]) == "2")
                        {
                            confiuration["beneficiaryIssueDateVisibilty"] = false;
                            confiuration["beneficiaryIssueDateMandatory"] = false;

                        }

                        //Expiry Date
                        if (Convert.ToString(dtbenfconfig.Rows[0]["Expiry_date_vis"]) == "0")
                        {
                            confiuration["beneficiaryExpiryDateVisibilty"] = true;
                            confiuration["beneficiaryExpiryDateMandatory"] = true;

                        }
                        else if (Convert.ToString(dtbenfconfig.Rows[0]["Expiry_date_vis"]) == "1")
                        {
                            confiuration["beneficiaryExpiryDateVisibilty"] = true;
                            confiuration["beneficiaryExpiryDateMandatory"] = false;

                        }
                        else if (Convert.ToString(dtbenfconfig.Rows[0]["Expiry_date_vis"]) == "2")
                        {
                            confiuration["beneficiaryExpiryDateVisibilty"] = false;
                            confiuration["beneficiaryExpiryDateMandatory"] = false;

                        }

                        //Place of Issue
                        if (Convert.ToString(dtbenfconfig.Rows[0]["Plcae_of_Issue_vis"]) == "0")
                        {
                            confiuration["beneficiaryPlcaeOfIssueVisibilty"] = true;
                            confiuration["beneficiaryPlcaeOfIssueMandatory"] = true;

                        }
                        else if (Convert.ToString(dtbenfconfig.Rows[0]["Plcae_of_Issue_vis"]) == "1")
                        {
                            confiuration["beneficiaryPlcaeOfIssueVisibilty"] = true;
                            confiuration["beneficiaryPlcaeOfIssueMandatory"] = false;

                        }
                        else if (Convert.ToString(dtbenfconfig.Rows[0]["Plcae_of_Issue_vis"]) == "2")
                        {
                            confiuration["beneficiaryPlcaeOfIssueVisibilty"] = false;
                            confiuration["beneficiaryPlcaeOfIssueMandatory"] = false;

                        }

                        //Date of Birth
                        if (Convert.ToString(dtbenfconfig.Rows[0]["Date_of_Birth_vis"]) == "0")
                        {
                            confiuration["beneficiaryBirthDateVisibilty"] = true;
                            confiuration["beneficiaryBirthDateMandatory"] = true;

                        }
                        else if (Convert.ToString(dtbenfconfig.Rows[0]["Date_of_Birth_vis"]) == "1")
                        {
                            confiuration["beneficiaryBirthDateVisibilty"] = true;
                            confiuration["beneficiaryBirthDateMandatory"] = false;

                        }
                        else if (Convert.ToString(dtbenfconfig.Rows[0]["Date_of_Birth_vis"]) == "2")
                        {
                            confiuration["beneficiaryBirthDateVisibilty"] = false;
                            confiuration["beneficiaryBirthDateMandatory"] = false;

                        }

                        // Front Page of ID
                        if (Convert.ToString(dtbenfconfig.Rows[0]["Front_ID_vis"]) == "0")
                        {
                            confiuration["beneficiaryFrontPageIdVisibilty"] = true;
                            confiuration["beneficiaryFrontPageIdMandatory"] = true;

                        }
                        else if (Convert.ToString(dtbenfconfig.Rows[0]["Front_ID_vis"]) == "1")
                        {
                            confiuration["beneficiaryFrontPageIdVisibilty"] = true;
                            confiuration["beneficiaryFrontPageIdMandatory"] = false;

                        }
                        else if (Convert.ToString(dtbenfconfig.Rows[0]["Front_ID_vis"]) == "2")
                        {
                            confiuration["beneficiaryFrontPageIdVisibilty"] = false;
                            confiuration["beneficiaryFrontPageIdMandatory"] = false;

                        }



                        // Back Page of ID
                        if (Convert.ToString(dtbenfconfig.Rows[0]["Back_ID_vis"]) == "0")
                        {
                            confiuration["beneficiaryBackIdVisibilty"] = true;
                            confiuration["beneficiaryBackIdMandatory"] = true;

                        }
                        else if (Convert.ToString(dtbenfconfig.Rows[0]["Back_ID_vis"]) == "1")
                        {
                            confiuration["beneficiaryBackIdVisibilty"] = true;
                            confiuration["beneficiaryBackIdMandatory"] = false;

                        }
                        else if (Convert.ToString(dtbenfconfig.Rows[0]["Back_ID_vis"]) == "2")
                        {
                            confiuration["beneficiaryBackIdVisibilty"] = false;
                            confiuration["beneficiaryBackIdMandatory"] = false;

                        }

                        // Comment
                        if (Convert.ToString(dtbenfconfig.Rows[0]["Comment_vis"]) == "0")
                        {
                            confiuration["beneficiaryCommentVisibilty"] = true;
                            confiuration["beneficiaryCommentMandatory"] = true;

                        }
                        else if (Convert.ToString(dtbenfconfig.Rows[0]["Comment_vis"]) == "1")
                        {
                            confiuration["beneficiaryCommentVisibilty"] = true;
                            confiuration["beneficiaryCommentMandatory"] = false;

                        }
                        else if (Convert.ToString(dtbenfconfig.Rows[0]["Comment_vis"]) == "2")
                        {
                            confiuration["beneficiaryCommentVisibilty"] = false;
                            confiuration["beneficiaryCommentMandatory"] = false;

                        }

                        #endregion IDUploadConfiguration


                        confiurations.Add(confiuration);

                        
                    }
                    await con.CloseAsync();
                    returnJsonData = new { response = true, responseCode = "00", data = confiurations };
                    return new JsonResult(returnJsonData);
                }

            }
            catch (Exception ex)
            {
                Model.ActivityLogTracker _objActivityLog = new Model.ActivityLogTracker();
                _objActivityLog.Activity = " Exception Beneficiary Configuration Error: " + ex.ToString() + " ";
                _objActivityLog.FunctionName = "beneficiaryconfiguration";
                _objActivityLog.Transaction_ID = 0;
                _objActivityLog.WhoAcessed = 1;
                _objActivityLog.Branch_ID = 0;
                _objActivityLog.Client_ID = 0;

                Service.srvActivityLogTracker srvActivityLog = new Service.srvActivityLogTracker();
                int _i = srvActivityLog.Create(_objActivityLog, HttpContext);
                //CompanyInfo.InsertErrorLogTracker(" Exception Beneficiary Configuration Error: " + ex.ToString(), 0, 0, 0, 0, "beneficiaryconfiguration", Convert.ToInt32(data.Branch_ID), Convert.ToInt32(data.Client_ID), "", HttpContext);
                await Task.Run(() => CompanyInfo.InsertErrorLogTracker(
                "Exception Beneficiary Configuration Error: " + ex.ToString(),
                0, 0, 0, 0, "beneficiaryconfiguration", Convert.ToInt32(data.Branch_ID), Convert.ToInt32(data.Client_ID), "", HttpContext));
                returnJsonData = new { response = false, responseCode = "01", data = "Field is missing." };
                return new JsonResult(returnJsonData);
            }
            #endregion
            return returnJsonData;
        }


        [HttpPost]
        [Route("getbenefmobilecount")]
        public IActionResult getbenefmobilecount([FromBody] JsonElement objNew)
        {
            HttpContext context = HttpContext;
            string json = System.Text.Json.JsonSerializer.Serialize(objNew);
            CompanyInfo.InsertrequestLogTracker("SaveBeneficiaryMedicare full request body: " + JObject.Parse(json), 0, 0, 0, 0, "savebeneficiarymedicare", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
            dynamic data = JObject.Parse(json); string Status = string.Empty;
            Model.response.WebResponse response = null;
            DataTable dt_info = new DataTable();
            Beneficiary obj = new Beneficiary();
            var validateJsonData = (dynamic)null;
            // var cm = li.SingleOrDefault();

            int number;
            if (!int.TryParse(Convert.ToString(data.Client_ID), out number))
            {
                validateJsonData = new { response = false, responseCode = "02", data = "Invalid ClientID." };
                return new JsonResult(validateJsonData);
            }
            if (!int.TryParse(Convert.ToString(data.Branch_ID), out number))
            {
                validateJsonData = new { response = false, responseCode = "02", data = "Invalid branchID." };
                return new JsonResult(validateJsonData);
            }
            if (data.Beneficiary_ID == "" || data.Beneficiary_ID == null)
            {
                validateJsonData = new { response = false, responseCode = "02", data = "Invalid BeneficiaryID." };
                return new JsonResult(validateJsonData);
            }
            if (data.Country_Code == "" || data.Country_Code == null)
            {
                validateJsonData = new { response = false, responseCode = "02", data = "Invalid Country Code." };
                return new JsonResult(validateJsonData);
            }
            if (data.Beneficiary_Mobile == "" || data.Beneficiary_Mobile == null)
            {
                validateJsonData = new { response = false, responseCode = "02", data = "Invalid Beneficiary Mobile Number." };
                return new JsonResult(validateJsonData);
            }

            try
            {
                obj = JsonConvert.DeserializeObject<Beneficiary>(json);
                Service.srvBeneficiary srv3 = new Service.srvBeneficiary(HttpContext);
                DataTable li1 = srv3.get_benf_mobile_count(obj);

                DataTable dtc = CompanyInfo.get(obj.Client_ID, HttpContext);


                if (dtc.Rows.Count > 0 && li1 != null && li1.Rows.Count > 0)
                {
                    dt_info.Columns.Add("Company_Cnt", typeof(int));
                    dt_info.Columns.Add("found_cnt", typeof(int));
                    var Company_cnt = Convert.ToInt16(dtc.Rows[0]["Count_Duplicate_Beneficiary_Mobile"]);
                    var found_cnt = Convert.ToInt16(li1.Rows[0]["count"]);
                    dt_info.Rows.Add(Company_cnt, found_cnt); // add values in table

                    var relationshipData = dt_info.Rows.OfType<DataRow>()
               .Select(row => dt_info.Columns.OfType<DataColumn>()
                   .ToDictionary(col => col.ColumnName, c => row[c].ToString()));

                    if (Convert.ToInt16(dtc.Rows[0]["Count_Duplicate_Beneficiary_Mobile"]) > Convert.ToInt16(li1.Rows[0]["count"]))
                    {
                        response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_SUCCESS);
                        response.data = "Success";
                        response.ObjData = dt_info;
                        response.ResponseCode = 0;
                        validateJsonData = new { response = false, responseCode = "00", data = response.data };
                        return new JsonResult(validateJsonData);
                    }
                    else
                    {
                        response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_SUCCESS);
                        response.data = "Already";
                        response.ObjData = dt_info;
                        response.ResponseCode = 2;
                        validateJsonData = new { response = false, responseCode = "02", data = response.data };
                        return new JsonResult(validateJsonData);
                    }

                }
                else
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "Something went wrong. Please try again later." };
                    return new JsonResult(validateJsonData);

                }


            }
            catch (Exception ex)
            {
                string Activity = "Api getbenefmobilecount: " + ex.ToString() + " ";
                validateJsonData = new { response = false, responseCode = "01", data = "Invalid Request" };
                CompanyInfo.InsertErrorLogTracker(Activity.ToString(), 0, 0, 0, 0, "getbenefmobilecount", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "", context);
                return new JsonResult(validateJsonData);
            }
        }



        [HttpPost]
        [Authorize]
        [Route("configuration_")]
        public JsonResult beneficiaryconfiguration_([FromBody] JsonElement objdata)
        {
            
            var returnJsonData = (dynamic)null;
            Model.response.WebResponse response = null;
            string jsonData = System.Text.Json.JsonSerializer.Serialize(objdata);
            dynamic data = JObject.Parse(jsonData);
            CompanyInfo.InsertrequestLogTracker("configuration full request body: " + JObject.Parse(jsonData), 0, 0, 0, 0, "beneficiaryconfiguration", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
            #region validateData
            try
            {
                if (data.clientID == "" || data.clientID == null)
                {
                    returnJsonData = new { response = false, responseCode = "02", data = "Invalid Client ID." };
                    return new JsonResult(returnJsonData);
                }
                else if (data.paymentCollectionTypeID == "" || data.paymentCollectionTypeID == null)
                {
                    returnJsonData = new { response = false, responseCode = "02", data = "Invalid Collection Type." };
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

                Beneficiary obj = JsonConvert.DeserializeObject<Beneficiary>(jsonData);
                
                obj.Collection_type_Id = data.paymentCollectionTypeID;
                obj.Client_ID = data.clientID;
                obj.Branch_ID = data.branchID;
                obj.Country_ID = data.countryID;
                obj.Beneficiary_Country_ID = obj.Country_ID;

                if (!SqlInjectionProtector.ReadJsonElementValues(objdata) || !SqlInjectionProtector.ReadJsonElementValuesScript(objdata))
                {
                    var errorResponse = new { response = false, responseCode = "02", data = "Invalid input detected." };
                    return new JsonResult(errorResponse) { StatusCode = 400 };
                }

                using (MySqlConnector.MySqlConnection con = new MySqlConnector.MySqlConnection(_configuration.GetConnectionString("connectionString")))
                {
                    con.Open();
                    MySqlConnector.MySqlCommand cmd = new MySqlConnector.MySqlCommand("SP_Beneficiary_Configuration");
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection = con;
                    cmd.Parameters.AddWithValue("_Country_ID", obj.Beneficiary_Country_ID);
                    cmd.Parameters.AddWithValue("_Collection_Type", obj.Collection_type_Id);
                    db_connection.ExecuteNonQueryProcedure(cmd);
                    DataTable dtbenfconfig = db_connection.ExecuteQueryDataTableProcedure(cmd);

                    List<Dictionary<string, object>> confiurations = new List<Dictionary<string, object>>();

                    Dictionary<string, object> confiuration = new Dictionary<string, object>();
                    Dictionary<string, object> configuration2 = new Dictionary<string, object>();

                    List<Dictionary<string, object>> beneficiaryConfigureList = new List<Dictionary<string, object>>();
                    Dictionary<string, object> beneficiaryConfig = new Dictionary<string, object>();


                    if (dtbenfconfig != null && dtbenfconfig.Rows.Count > 0)
                    {
                        // Beneficiary Name
                        #region Professional_Details
                        if ( Convert.ToString(dtbenfconfig.Rows[0]["Beneficiary_Name"]) == "0" )
                         {
                             confiuration["beneficiaryNameVisible"] = true ;
                             confiuration["beneficiaryNameMandetory"] = true;

                            beneficiaryConfig["fieldID"] = "recipient_name";
                            beneficiaryConfig["fieldName"] = "Recipient Name";
                            beneficiaryConfig["fieldPlaceHolder"] = "Recipient Name";
                            beneficiaryConfig["visible"] = true;
                            beneficiaryConfig["mandetory"] = true;
                            beneficiaryConfig["type"] = "text";
                            beneficiaryConfigureList.Add(beneficiaryConfig);
                        }
                        else if (Convert.ToString(dtbenfconfig.Rows[0]["Beneficiary_Name"]) == "1")
                        {
                            confiuration["beneficiaryNameVisible"] = true;
                            confiuration["beneficiaryNameMandetory"] = false;

                            beneficiaryConfig["fieldID"] = "recipient_name";
                            beneficiaryConfig["fieldName"] = "Recipient Name";
                            beneficiaryConfig["fieldPlaceHolder"] = "Recipient Name";
                            beneficiaryConfig["visible"] = true;
                            beneficiaryConfig["mandetory"] = false;
                            beneficiaryConfig["type"] = "text";
                            beneficiaryConfigureList.Add(beneficiaryConfig);
                        }
                        else if (Convert.ToString(dtbenfconfig.Rows[0]["Beneficiary_Name"]) == "2")
                        {
                            confiuration["beneficiaryNameVisible"] = false;
                            confiuration["beneficiaryNameMandetory"] = false;

                            beneficiaryConfig["fieldID"] = "recipient_name";
                            beneficiaryConfig["fieldName"] = "Recipient Name";
                            beneficiaryConfig["fieldPlaceHolder"] = "Recipient Name";
                            beneficiaryConfig["visible"] = false;
                            beneficiaryConfig["mandetory"] = false;
                            beneficiaryConfig["type"] = "text";
                            beneficiaryConfigureList.Add(beneficiaryConfig);
                        }

                        // Relation
                        if (Convert.ToString(dtbenfconfig.Rows[0]["Beneficiary_relation"]) == "0")
                        {
                            confiuration["beneficiaryRelationVisible"] = true;
                            confiuration["beneficiaryRelationMandetory"] = true;

                            beneficiaryConfig["fieldID"] = "relationship";
                            beneficiaryConfig["fieldName"] = "Relationship";
                            beneficiaryConfig["fieldPlaceHolder"] = "Relationship";
                            beneficiaryConfig["visible"] = true;
                            beneficiaryConfig["mandetory"] = true;
                            beneficiaryConfig["type"] = "int";
                            beneficiaryConfigureList.Add(beneficiaryConfig);

                        }
                        else if (Convert.ToString(dtbenfconfig.Rows[0]["Beneficiary_relation"]) == "1")
                        {
                            confiuration["beneficiaryRelationVisible"] = true;
                            confiuration["beneficiaryRelationMandetory"] = false;

                            beneficiaryConfig["fieldID"] = "relationship";
                            beneficiaryConfig["fieldName"] = "Relationship";
                            beneficiaryConfig["fieldPlaceHolder"] = "Relationship";
                            beneficiaryConfig["visible"] = true;
                            beneficiaryConfig["mandetory"] = false;
                            beneficiaryConfig["type"] = "int";
                            beneficiaryConfigureList.Add(beneficiaryConfig);
                        }
                        else if (Convert.ToString(dtbenfconfig.Rows[0]["Beneficiary_relation"]) == "2")
                        {
                            confiuration["beneficiaryRelationVisible"] = false;
                            confiuration["beneficiaryRelationMandetory"] = false;

                            beneficiaryConfig["fieldID"] = "relationship";
                            beneficiaryConfig["fieldName"] = "Relationship";
                            beneficiaryConfig["fieldPlaceHolder"] = "Relationship";
                            beneficiaryConfig["visible"] = false;
                            beneficiaryConfig["mandetory"] = false;
                            beneficiaryConfig["type"] = "int";
                            beneficiaryConfigureList.Add(beneficiaryConfig);
                        }

                        // "Beneficiary Address"
                        if (Convert.ToString(dtbenfconfig.Rows[0]["Beneficiary_Address"]) == "0")
                        {
                            confiuration["beneficiaryAddressVisible"] = true;
                            confiuration["beneficiaryAddressMandetory"] = true;

                            beneficiaryConfig["fieldID"] = "address";
                            beneficiaryConfig["fieldName"] = "Address";
                            beneficiaryConfig["fieldPlaceHolder"] = "Address";
                            beneficiaryConfig["visible"] = true;
                            beneficiaryConfig["mandetory"] = true;
                            beneficiaryConfig["type"] = "text";
                            beneficiaryConfigureList.Add(beneficiaryConfig);

                        }
                        else if (Convert.ToString(dtbenfconfig.Rows[0]["Beneficiary_Address"]) == "1")
                        {
                            confiuration["beneficiaryAddressVisible"] = true;
                            confiuration["beneficiaryAddressMandetory"] = false;

                            beneficiaryConfig["fieldID"] = "address";
                            beneficiaryConfig["fieldName"] = "Address";
                            beneficiaryConfig["fieldPlaceHolder"] = "Address";
                            beneficiaryConfig["visible"] = true;
                            beneficiaryConfig["mandetory"] = false;
                            beneficiaryConfig["type"] = "text";
                            beneficiaryConfigureList.Add(beneficiaryConfig);
                        }
                        else if (Convert.ToString(dtbenfconfig.Rows[0]["Beneficiary_Address"]) == "2")
                        {
                            confiuration["beneficiaryAddressVisible"] = false;
                            confiuration["beneficiaryAddressMandetory"] = false;

                            beneficiaryConfig["fieldID"] = "address";
                            beneficiaryConfig["fieldName"] = "Address";
                            beneficiaryConfig["fieldPlaceHolder"] = "Address";
                            beneficiaryConfig["visible"] = false;
                            beneficiaryConfig["mandetory"] = false;
                            beneficiaryConfig["type"] = "text";
                            beneficiaryConfigureList.Add(beneficiaryConfig);
                        }

                        // Beneficiary City                        
                        if (Convert.ToString(dtbenfconfig.Rows[0]["Beneficiary_City"]) == "0")
                        {
                            confiuration["beneficiaryCityVisible"] = true;
                            confiuration["beneficiaryCityMandetory"] = true;

                            beneficiaryConfig["fieldID"] = "city";
                            beneficiaryConfig["fieldName"] = "City";
                            beneficiaryConfig["fieldPlaceHolder"] = "City";
                            beneficiaryConfig["visible"] = true;
                            beneficiaryConfig["mandetory"] = true;
                            beneficiaryConfig["type"] = "int";
                            beneficiaryConfigureList.Add(beneficiaryConfig);
                        }
                        else if (Convert.ToString(dtbenfconfig.Rows[0]["Beneficiary_City"]) == "1")
                        {
                            confiuration["beneficiaryCityVisible"] = true;
                            confiuration["beneficiaryCityMandetory"] = false;

                            beneficiaryConfig["fieldID"] = "city";
                            beneficiaryConfig["fieldName"] = "City";
                            beneficiaryConfig["fieldPlaceHolder"] = "City";
                            beneficiaryConfig["visible"] = true;
                            beneficiaryConfig["mandetory"] = false;
                            beneficiaryConfig["type"] = "int";
                            beneficiaryConfigureList.Add(beneficiaryConfig);
                        }
                        else if (Convert.ToString(dtbenfconfig.Rows[0]["Beneficiary_City"]) == "2")
                        {
                            confiuration["beneficiaryCityVisible"] = false;
                            confiuration["beneficiaryCityMandetory"] = false;

                            beneficiaryConfig["fieldID"] = "city";
                            beneficiaryConfig["fieldName"] = "City";
                            beneficiaryConfig["fieldPlaceHolder"] = "City";
                            beneficiaryConfig["visible"] = false;
                            beneficiaryConfig["mandetory"] = false;
                            beneficiaryConfig["type"] = "int";
                            beneficiaryConfigureList.Add(beneficiaryConfig);
                        }

                        // Beneficiary Mobile                        
                        if (Convert.ToString(dtbenfconfig.Rows[0]["Beneficiary_Mobile"]) == "0")
                        {
                            confiuration["beneficiaryMobileVisible"] = true;
                            confiuration["beneficiaryMobileMandetory"] = true;

                            beneficiaryConfig["fieldID"] = "mobile";
                            beneficiaryConfig["fieldName"] = "Mobile";
                            beneficiaryConfig["fieldPlaceHolder"] = "Mobile";
                            beneficiaryConfig["visible"] = true;
                            beneficiaryConfig["mandetory"] = true;
                            beneficiaryConfig["type"] = "number";
                            
                        }
                        else if (Convert.ToString(dtbenfconfig.Rows[0]["Beneficiary_Mobile"]) == "1")
                        {
                            confiuration["beneficiaryMobileVisible"] = true;
                            confiuration["beneficiaryMobileMandetory"] = false;

                            beneficiaryConfig["fieldID"] = "mobile";
                            beneficiaryConfig["fieldName"] = "Mobile";
                            beneficiaryConfig["fieldPlaceHolder"] = "Mobile";
                            beneficiaryConfig["visible"] = true;
                            beneficiaryConfig["mandetory"] = false;
                            beneficiaryConfig["type"] = "number";
                            
                        }
                        else if (Convert.ToString(dtbenfconfig.Rows[0]["Beneficiary_Mobile"]) == "2")
                        {
                            confiuration["beneficiaryMobileVisible"] = false;
                            confiuration["beneficiaryMobileMandetory"] = false;

                            beneficiaryConfig["fieldID"] = "mobile";
                            beneficiaryConfig["fieldName"] = "Mobile";
                            beneficiaryConfig["fieldPlaceHolder"] = "Mobile";
                            beneficiaryConfig["visible"] = false;
                            beneficiaryConfig["mandetory"] = false;
                            beneficiaryConfig["type"] = "number";
                            beneficiaryConfigureList.Add(beneficiaryConfig);
                        }

                        if (Convert.ToString(dtbenfconfig.Rows[0]["Range_check_mobile"]) == "0" && Convert.ToString(dtbenfconfig.Rows[0]["Beneficiary_Mobile"]) != "2")
                        {   //Fixed                            
                            confiuration["beneficiaryMobileLength"] = Convert.ToInt16(dtbenfconfig.Rows[0]["mobile_max"]);

                            beneficiaryConfig["minlength"] = Convert.ToInt16(dtbenfconfig.Rows[0]["mobile_max"]);
                            beneficiaryConfig["maxlength"] = Convert.ToInt16(dtbenfconfig.Rows[0]["mobile_max"]);
                            beneficiaryConfigureList.Add(beneficiaryConfig);
                        }
                        else if (Convert.ToString(dtbenfconfig.Rows[0]["Range_check_mobile"]) == "1" && Convert.ToString(dtbenfconfig.Rows[0]["Beneficiary_Mobile"]) != "2")
                        {   //Custom
                            confiuration["beneficiaryMobileMinLength"] = Convert.ToInt16(dtbenfconfig.Rows[0]["mobile_min"]);
                            confiuration["beneficiaryMobileMaxLength"] = Convert.ToInt16(dtbenfconfig.Rows[0]["mobile_max"]);

                            beneficiaryConfig["minlength"] = Convert.ToInt16(dtbenfconfig.Rows[0]["mobile_min"]); 
                            beneficiaryConfig["maxlength"] = Convert.ToInt16(dtbenfconfig.Rows[0]["mobile_max"]);
                            beneficiaryConfigureList.Add(beneficiaryConfig);
                        }

                        //Beneficiary DOB
                        try
                        {
                            if (Convert.ToString(dtbenfconfig.Rows[0]["DateOf_Birth"]) == "0")
                            {
                                confiuration["beneficiaryDOBVisible"] = true;
                                confiuration["beneficiaryDOBMandetory"] = true;
                            }
                            else if (Convert.ToString(dtbenfconfig.Rows[0]["DateOf_Birth"]) == "1")
                            {
                                confiuration["beneficiaryDOBVisible"] = true;
                                confiuration["beneficiaryDOBMandetory"] = false;
                            }
                            else
                            {
                                confiuration["beneficiaryDOBVisible"] = false;
                                confiuration["beneficiaryDOBMandetory"] = false;
                            }
                        }
                        catch { }

                        //Beneficiary Gender
                        try
                        {
                            if (Convert.ToString(dtbenfconfig.Rows[0]["Beneficiary_Gender"]) == "0")
                            {
                                confiuration["beneficiaryGenderVisible"] = true;
                                confiuration["beneficiaryGenderMandetory"] = true;
                            }
                            else if (Convert.ToString(dtbenfconfig.Rows[0]["DateOf_Birth"]) == "1")
                            {
                                confiuration["beneficiaryGenderVisible"] = true;
                                confiuration["beneficiaryGenderMandetory"] = false;
                            }
                            else
                            {
                                confiuration["beneficiaryGenderVisible"] = false;
                                confiuration["beneficiaryGenderMandetory"] = false;
                            }
                        }
                        catch { }

                        #endregion Professional_Details

                        #region Collection_Type_Details  

                        if (obj.Collection_type_Id == 3)
                        {
                            // Mobile Provider
                            if (Convert.ToString(dtbenfconfig.Rows[0]["mobile_provider"]) == "0")
                            {
                                confiuration["beneficiaryMobileProviderVisible"] = true;
                                confiuration["beneficiaryMobileProviderMandetory"] = true;

                                beneficiaryConfig["fieldID"] = "mobileprovider";
                                beneficiaryConfig["fieldName"] = "Select Mobile Money Provider";
                                beneficiaryConfig["fieldPlaceHolder"] = "Select Mobile Money Provider";
                                beneficiaryConfig["visible"] = true;
                                beneficiaryConfig["mandetory"] = true;
                                beneficiaryConfig["type"] = "int";
                                beneficiaryConfigureList.Add(beneficiaryConfig);
                            }
                            else if (Convert.ToString(dtbenfconfig.Rows[0]["mobile_provider"]) == "0")
                            {
                                confiuration["beneficiaryMobileProviderVisible"] = false;
                                confiuration["beneficiaryMobileProviderMandetory"] = false;

                                beneficiaryConfig["fieldID"] = "mobileprovider";
                                beneficiaryConfig["fieldName"] = "Select Mobile Money Provider";
                                beneficiaryConfig["fieldPlaceHolder"] = "Select Mobile Money Provider";
                                beneficiaryConfig["visible"] = false;
                                beneficiaryConfig["mandetory"] = false;
                                beneficiaryConfig["type"] = "int";
                                beneficiaryConfigureList.Add(beneficiaryConfig);
                            }
                        }
                        #endregion Collection_Type_Details

                        if (obj.Collection_type_Id == 10)
                        {
                            if (Convert.ToString(dtbenfconfig.Rows[0]["wallet_provider"]) == "0")
                            {
                                confiuration["beneficiarywalletProvider"] = true;
                                confiuration["beneficiarywalletProviderMandetory"] = true;
                                beneficiaryConfigureList.Add(beneficiaryConfig);
                            }
                            if (Convert.ToString(dtbenfconfig.Rows[0]["wallet_provider"]) == "1")
                            {
                                confiuration["beneficiarywalletProvider"] = false;
                                confiuration["beneficiarywalletProviderMandetory"] = false;
                                beneficiaryConfigureList.Add(beneficiaryConfig);
                            }

                            if (Convert.ToString(dtbenfconfig.Rows[0]["Wallet_Id_vis"]) == "1")
                            {
                                confiuration["beneficiarywalletIdVisible"] = false;
                                confiuration["beneficiarywalletIdMandetory"] = false;

                                beneficiaryConfigureList.Add(beneficiaryConfig);
                            }
                            if (Convert.ToString(dtbenfconfig.Rows[0]["Wallet_Id_vis"]) == "0")
                            {
                                confiuration["beneficiarywalletIdVisible"] = true;
                                if (Convert.ToString(dtbenfconfig.Rows[0]["Range_check_wallet"]) == "0")
                                {
                                    confiuration["beneficiarywalletIdfixed"] = true;                                    
                                    confiuration["beneficiarywalletlength"] = Convert.ToInt16(dtbenfconfig.Rows[0]["Wallet_Id_length"]); 
                                }
                                if (Convert.ToString(dtbenfconfig.Rows[0]["Range_check_wallet"]) == "1")
                                {
                                    confiuration["beneficiarywalletIdfixed"] = false;
                                    confiuration["beneficiarywalletminlength"] = Convert.ToInt16(dtbenfconfig.Rows[0]["Wallet_Min_Length"]); 
                                    confiuration["beneficiarywalletmaxlength"] = Convert.ToInt16(dtbenfconfig.Rows[0]["Wallet_Id_length"]); 
                                }
                                beneficiaryConfigureList.Add(beneficiaryConfig);
                            }


                            if (Convert.ToString(dtbenfconfig.Rows[0]["Wallet_Id_vis"]) == "0" && Convert.ToString(dtbenfconfig.Rows[0]["Wallet_Id_man"]) == "0")
                            {                                
                                confiuration["beneficiarywalletIdMandetory"] = true;
                                beneficiaryConfigureList.Add(beneficiaryConfig);
                            }
                            else if (Convert.ToString(dtbenfconfig.Rows[0]["Wallet_Id_vis"]) == "0" && Convert.ToString(dtbenfconfig.Rows[0]["Wallet_Id_man"]) == "1")
                            {                                
                                confiuration["beneficiarywalletIdMandetory"] = false;
                                beneficiaryConfigureList.Add(beneficiaryConfig);
                            }



                        }


                            #region Bank_Details 

                            if (obj.Collection_type_Id == 1) 
                            {

                            //Account Number
                            if (Convert.ToString(dtbenfconfig.Rows[0]["ShowAccount_No"]) == "0")
                            {
                                confiuration["beneficiaryAccountVisible"] = true;
                                beneficiaryConfig["visible"] = true;
                            }
                            else
                            {
                                confiuration["beneficiaryAccountVisible"] = false;
                                beneficiaryConfig["visible"] = false;
                            }

                            if (Convert.ToString(dtbenfconfig.Rows[0]["Verify_Account_no"]) == "0")
                            {
                                confiuration["beneficiaryAccountMandetory"] = true;
                                beneficiaryConfig["mandetory"] = true;
                            }
                            else
                            {
                                confiuration["beneficiaryAccountMandetory"] = false;
                                beneficiaryConfig["mandetory"] = false;
                            }

                            if (Convert.ToString(dtbenfconfig.Rows[0]["Range_check_accno"]) == "1" && Convert.ToString(dtbenfconfig.Rows[0]["ShowAccount_No"]) == "0")
                            {
                                confiuration["beneficiaryAccountFixedLength"] = false;
                                confiuration["beneficiaryAccountMinLength"] = Convert.ToInt16(dtbenfconfig.Rows[0]["Acc_no_min_length"]);
                                confiuration["beneficiaryAccountMaxLength"] = Convert.ToInt16(dtbenfconfig.Rows[0]["Acc_no_length"]);

                                beneficiaryConfig["minLength"] = Convert.ToInt16(dtbenfconfig.Rows[0]["Acc_no_min_length"]);
                                beneficiaryConfig["maxLength"] = Convert.ToInt16(dtbenfconfig.Rows[0]["Acc_no_length"]);                                 
                            }
                            else if (Convert.ToString(dtbenfconfig.Rows[0]["Range_check_accno"]) == "0" && Convert.ToString(dtbenfconfig.Rows[0]["ShowAccount_No"]) == "0")
                            {
                                confiuration["beneficiaryAccountFixedLength"] = true;
                                confiuration["beneficiaryAccountLength"] = Convert.ToInt16(dtbenfconfig.Rows[0]["Acc_no_length"]);

                                beneficiaryConfig["minLength"] = Convert.ToInt16(dtbenfconfig.Rows[0]["Acc_no_length"]);
                                beneficiaryConfig["maxLength"] = Convert.ToInt16(dtbenfconfig.Rows[0]["Acc_no_length"]);                                 
                            }

                            if (Convert.ToString(dtbenfconfig.Rows[0]["Acc_Lable"]).Trim() != "" && Convert.ToString(dtbenfconfig.Rows[0]["ShowAccount_No"]) == "0")
                            {
                                confiuration["AccountShowLable"] = Convert.ToString(dtbenfconfig.Rows[0]["Acc_Lable"]).Trim();

                                beneficiaryConfig["fieldID"] = "accountnumber";
                                beneficiaryConfig["fieldName"] = confiuration["AccountShowLable"];
                                beneficiaryConfig["fieldPlaceHolder"] = confiuration["AccountShowLable"];
                                beneficiaryConfig["type"] = "text";
                            }
                            beneficiaryConfigureList.Add(beneficiaryConfig);

                            try { 
                            #region Confirm_Account_Number
                            //Confirm Account Number
                            if (Convert.ToString(dtbenfconfig.Rows[0]["ConfirmAccNo_vis"]) == "0")
                            {
                                confiuration["beneficiaryConfirmAccountVisible"] = true;
                                beneficiaryConfig["visible"] = true;
                            }
                            else
                            {
                                confiuration["beneficiaryConfirmAccountVisible"] = false;
                                beneficiaryConfig["visible"] = false;
                            }

                            if (Convert.ToString(dtbenfconfig.Rows[0]["ConfirmAccNo_man"]) == "0")
                            {
                                confiuration["beneficiaryConfirmAccountMandetory"] = true;
                                beneficiaryConfig["mandetory"] = true;
                            }
                            else
                            {
                                confiuration["beneficiaryConfirmAccountMandetory"] = false;
                                beneficiaryConfig["mandetory"] = false;
                            }
                            if (Convert.ToString(dtbenfconfig.Rows[0]["Range_check_accno"]) == "1" && Convert.ToString(dtbenfconfig.Rows[0]["ConfirmAccNo_vis"]) == "0")
                            {
                                confiuration["beneficiaryConfirmAccountFixedLength"] = false;
                                confiuration["beneficiaryConfirmAccountMinLength"] = Convert.ToInt16(dtbenfconfig.Rows[0]["Acc_no_min_length"]);
                                confiuration["beneficiaryConfirmAccountMaxLength"] = Convert.ToInt16(dtbenfconfig.Rows[0]["Acc_no_length"]);

                                beneficiaryConfig["minLength"] = Convert.ToInt16(dtbenfconfig.Rows[0]["Acc_no_min_length"]);
                                beneficiaryConfig["maxLength"] = Convert.ToInt16(dtbenfconfig.Rows[0]["Acc_no_length"]);
                            }
                            else if (Convert.ToString(dtbenfconfig.Rows[0]["Range_check_accno"]) == "0" && Convert.ToString(dtbenfconfig.Rows[0]["ConfirmAccNo_vis"]) == "0")
                            {
                                confiuration["beneficiaryConfirmAccountFixedLength"] = true;
                                confiuration["beneficiaryConfirmAccountLength"] = Convert.ToInt16(dtbenfconfig.Rows[0]["Acc_no_length"]);

                                beneficiaryConfig["minLength"] = Convert.ToInt16(dtbenfconfig.Rows[0]["Acc_no_length"]);
                                beneficiaryConfig["maxLength"] = Convert.ToInt16(dtbenfconfig.Rows[0]["Acc_no_length"]);
                            }
                            if (Convert.ToString(dtbenfconfig.Rows[0]["ConfirmAccNo_Lable"]).Trim() != "" && Convert.ToString(dtbenfconfig.Rows[0]["ConfirmAccNo_vis"]) == "0")
                            {
                                confiuration["ConfirmAccountShowLable"] = Convert.ToString(dtbenfconfig.Rows[0]["ConfirmAccNo_Lable"]).Trim();

                                beneficiaryConfig["fieldID"] = "confirmaccountnumber";
                                beneficiaryConfig["fieldName"] = confiuration["ConfirmAccountShowLable"];
                                beneficiaryConfig["fieldPlaceHolder"] = confiuration["ConfirmAccountShowLable"];
                                beneficiaryConfig["type"] = "text";
                            }
                            beneficiaryConfigureList.Add(beneficiaryConfig);
                                #endregion Confirm_Account_Number
                            }
                            catch(Exception ex)
                            {                                
                            }

                            //IFSC Code
                            if (Convert.ToString(dtbenfconfig.Rows[0]["IFSC"]) == "0")
                            {
                                confiuration["beneficiaryIFSCVisible"] = true;
                                beneficiaryConfig["visible"] = true;
                            }
                            else
                            {
                                confiuration["beneficiaryIFSCVisible"] = false;
                                beneficiaryConfig["visible"] = false;
                            }

                            if (Convert.ToString(dtbenfconfig.Rows[0]["IFSC_Mandatory"]) == "0")
                            {
                                confiuration["beneficiaryIFSCMandetory"] = true;
                                beneficiaryConfig["mandetory"] = true;
                            }
                            else
                            {
                                confiuration["beneficiaryIFSCMandetory"] = false;
                                beneficiaryConfig["mandetory"] = false;
                            }

                            if (Convert.ToString(dtbenfconfig.Rows[0]["Range_check_ifsc"]) == "1" && Convert.ToString(dtbenfconfig.Rows[0]["IFSC"]) == "0")
                            {
                                confiuration["beneficiaryIFSCFixedLength"] = false;
                                confiuration["beneficiaryIFSCMinLength"] = Convert.ToInt16(dtbenfconfig.Rows[0]["Ifsc_no_min_length"]);
                                confiuration["beneficiaryIFSCMaxLength"] = Convert.ToInt16(dtbenfconfig.Rows[0]["Ifsc_no_length"]);

                                beneficiaryConfig["minLength"] = Convert.ToInt16(dtbenfconfig.Rows[0]["Ifsc_no_min_length"]);
                                beneficiaryConfig["maxLength"] = Convert.ToInt16(dtbenfconfig.Rows[0]["Ifsc_no_length"]);
                            }
                            else if (Convert.ToString(dtbenfconfig.Rows[0]["Range_check_ifsc"]) == "0" && Convert.ToString(dtbenfconfig.Rows[0]["IFSC"]) == "0")
                            {
                                confiuration["beneficiaryIFSCFixedLength"] = true;
                                confiuration["beneficiaryIFSCLength"] = Convert.ToInt16(dtbenfconfig.Rows[0]["Ifsc_no_length"]);

                                beneficiaryConfig["minLength"] = Convert.ToInt16(dtbenfconfig.Rows[0]["Ifsc_no_length"]);
                                beneficiaryConfig["maxLength"] = Convert.ToInt16(dtbenfconfig.Rows[0]["Ifsc_no_length"]);
                            }

                            if (Convert.ToString(dtbenfconfig.Rows[0]["IFSC_Lable"]).Trim() != "" && Convert.ToString(dtbenfconfig.Rows[0]["IFSC"]) == "0")
                            {
                                confiuration["IFSCShowLable"] = Convert.ToString(dtbenfconfig.Rows[0]["IFSC_Lable"]).Trim();

                                beneficiaryConfig["fieldID"] = "ifsccode";
                                beneficiaryConfig["fieldName"] = Convert.ToString(dtbenfconfig.Rows[0]["IFSC_Lable"]).Trim();
                                beneficiaryConfig["fieldPlaceHolder"] = Convert.ToString(dtbenfconfig.Rows[0]["IFSC_Lable"]).Trim();
                                beneficiaryConfig["type"] = "text";
                            }
                            beneficiaryConfigureList.Add(beneficiaryConfig);

                            // Bank Name
                            if (Convert.ToString(dtbenfconfig.Rows[0]["Bank_Name"]) == "0")
                            {
                                confiuration["beneficiaryBankNameVisible"] = true;
                                confiuration["beneficiaryBankNameMandetory"] = true;

                                beneficiaryConfig["fieldID"] = "bankname";
                                beneficiaryConfig["fieldName"] = "Bank Name";
                                beneficiaryConfig["fieldPlaceHolder"] = "Bank Name";
                                beneficiaryConfig["visible"] = true;
                                beneficiaryConfig["mandetory"] = true;
                                beneficiaryConfig["type"] = "text";
                                beneficiaryConfigureList.Add(beneficiaryConfig);
                            }
                            else
                            {
                                confiuration["beneficiaryBankNameVisible"] = false;
                                confiuration["beneficiaryBankNameMandetory"] = false;

                                beneficiaryConfig["fieldID"] = "bankname";
                                beneficiaryConfig["fieldName"] = "Bank Name";
                                beneficiaryConfig["fieldPlaceHolder"] = "Bank Name";
                                beneficiaryConfig["visible"] = false;
                                beneficiaryConfig["mandetory"] = false;
                                beneficiaryConfig["type"] = "text";
                                beneficiaryConfigureList.Add(beneficiaryConfig);
                            }

                            // Bank Code
                            if (Convert.ToString(dtbenfconfig.Rows[0]["Bank_Code"]) == "0")
                            {
                                confiuration["beneficiaryBankCodeVisible"] = true;
                                confiuration["beneficiaryBankCodeMandetory"] = true;

                                beneficiaryConfig["fieldID"] = "bankcode";
                                beneficiaryConfig["fieldName"] = "Bank Code";
                                beneficiaryConfig["fieldPlaceHolder"] = "Bank Code";
                                beneficiaryConfig["visible"] = true;
                                beneficiaryConfig["mandetory"] = true;
                                beneficiaryConfig["type"] = "text";
                                beneficiaryConfigureList.Add(beneficiaryConfig);
                            }
                            else
                            {
                                confiuration["beneficiaryBankCodeVisible"] = false;
                                confiuration["beneficiaryBankCodeMandetory"] = false;

                                beneficiaryConfig["fieldID"] = "bankcode";
                                beneficiaryConfig["fieldName"] = "Bank Code";
                                beneficiaryConfig["fieldPlaceHolder"] = "Bank Code";
                                beneficiaryConfig["visible"] = false;
                                beneficiaryConfig["mandetory"] = false;
                                beneficiaryConfig["type"] = "text";
                                beneficiaryConfigureList.Add(beneficiaryConfig);
                            }

                            //Branch Name                            
                            if (Convert.ToString(dtbenfconfig.Rows[0]["Branch"]) == "0")
                            {
                                confiuration["beneficiaryBranchVisible"] = true;
                                confiuration["beneficiaryBranchMandetory"] = true;

                                beneficiaryConfig["fieldID"] = "branch";
                                beneficiaryConfig["fieldName"] = "Branch";
                                beneficiaryConfig["fieldPlaceHolder"] = "Branch";
                                beneficiaryConfig["visible"] = true;
                                beneficiaryConfig["mandetory"] = true;
                                beneficiaryConfig["type"] = "text";
                                beneficiaryConfigureList.Add(beneficiaryConfig);
                            }
                            else
                            {
                                confiuration["beneficiaryBranchVisible"] = false;
                                confiuration["beneficiaryBranchMandetory"] = false;

                                beneficiaryConfig["fieldID"] = "branch";
                                beneficiaryConfig["fieldName"] = "Branch";
                                beneficiaryConfig["fieldPlaceHolder"] = "Branch";
                                beneficiaryConfig["visible"] = false;
                                beneficiaryConfig["mandetory"] = false;
                                beneficiaryConfig["type"] = "text";
                                beneficiaryConfigureList.Add(beneficiaryConfig);
                            }

                            //Branch Code                            
                            if (Convert.ToString(dtbenfconfig.Rows[0]["Branch_code_man"]) == "0")
                            {
                                confiuration["beneficiaryBranchCodeMandetory"] = true;

                                beneficiaryConfig["mandetory"] = true;                                                                
                            }
                            else
                            {
                                confiuration["beneficiaryBranchCodeMandetory"] = false;

                                beneficiaryConfig["mandetory"] = false;
                            }

                            if (Convert.ToString(dtbenfconfig.Rows[0]["Branch_Code"]) == "0")
                            {
                                confiuration["beneficiaryBranchCodeVisible"] = true;

                                beneficiaryConfig["visible"] = true;
                            }
                            else
                            {
                                confiuration["beneficiaryBranchCodeVisible"] = false;

                                beneficiaryConfig["visible"] = false;
                            }

                            if (Convert.ToString(dtbenfconfig.Rows[0]["Branch_code_name"]) != "")
                            {
                                confiuration["beneficiaryBranchCodeShowLable"] = Convert.ToString(dtbenfconfig.Rows[0]["Branch_code_name"]);                                
                            }
                            else
                            {
                                confiuration["beneficiaryBranchCodeShowLable"] = "Branch Code";
                            }
                            beneficiaryConfig["fieldID"] = "branchcode";
                            beneficiaryConfig["fieldName"] = "Branch Code";
                            beneficiaryConfig["fieldPlaceHolder"] = "Branch Code";
                            beneficiaryConfig["type"] = "text";
                            beneficiaryConfigureList.Add(beneficiaryConfig);

                            //IBAN Code                            
                            if (Convert.ToString(dtbenfconfig.Rows[0]["IBAN_Mandatory"]) == "0")
                            {
                                confiuration["beneficiaryIBANMandetory"] = true;
                                beneficiaryConfig["mandetory"] = true;
                            }
                            else
                            {
                                confiuration["beneficiaryIBANMandetory"] = false;
                                beneficiaryConfig["mandetory"] = false;
                            }

                            if (Convert.ToString(dtbenfconfig.Rows[0]["IBAN_Status"]) == "0")
                            {
                                confiuration["beneficiaryIBANVisible"] = true;
                                beneficiaryConfig["visible"] = true;
                            }
                            else
                            {
                                confiuration["beneficiaryIBANVisible"] = false;
                                beneficiaryConfig["visible"] = false;
                            }

                            if (Convert.ToString(dtbenfconfig.Rows[0]["Range_check_Iban"]) == "1" && Convert.ToString(dtbenfconfig.Rows[0]["IBAN_Status"]) == "0")
                            {
                                confiuration["beneficiaryIBANFixedLength"] = false;
                                confiuration["beneficiaryIBANMinLength"] = Convert.ToInt16(dtbenfconfig.Rows[0]["IBAN_Min_Length"]);
                                confiuration["beneficiaryIBANMaxLength"] = Convert.ToInt16(dtbenfconfig.Rows[0]["IBAN_Length"]);

                                beneficiaryConfig["minLength"] = Convert.ToInt16(dtbenfconfig.Rows[0]["IBAN_Min_Length"]);
                                beneficiaryConfig["maxLength"] = Convert.ToInt16(dtbenfconfig.Rows[0]["IBAN_Length"]);
                            }
                            else if (Convert.ToString(dtbenfconfig.Rows[0]["Range_check_Iban"]) == "0" && Convert.ToString(dtbenfconfig.Rows[0]["IBAN_Status"]) == "0")
                            {
                                confiuration["beneficiaryIBANFixedLength"] = true;
                                confiuration["beneficiaryIBANLength"] = Convert.ToInt16(dtbenfconfig.Rows[0]["IBAN_Length"]);

                                beneficiaryConfig["minLength"] = Convert.ToInt16(dtbenfconfig.Rows[0]["IBAN_Length"]);
                                beneficiaryConfig["maxLength"] = Convert.ToInt16(dtbenfconfig.Rows[0]["IBAN_Length"]);
                            }
                            beneficiaryConfig["fieldID"] = "branchcode";
                            if (Convert.ToString(dtbenfconfig.Rows[0]["IBAN_Label"]).Trim() != "" && Convert.ToString(dtbenfconfig.Rows[0]["IBAN_Status"]) == "0")
                            {
                                confiuration["IBANShowLable"] = Convert.ToString(dtbenfconfig.Rows[0]["IBAN_Label"]).Trim();

                                beneficiaryConfig["fieldName"] = confiuration["IBANShowLable"];
                                beneficiaryConfig["fieldPlaceHolder"] = confiuration["IBANShowLable"];
                            }                                                        
                            beneficiaryConfig["type"] = "text";
                            beneficiaryConfigureList.Add(beneficiaryConfig);

                            //BIC Code
                            if (Convert.ToString(dtbenfconfig.Rows[0]["BIC_Status"]) == "0")
                            {
                                confiuration["beneficiaryBICCodeVisible"] = true;
                                beneficiaryConfig["visible"] = true;
                            }
                            else
                            {
                                confiuration["beneficiaryBICCodeVisible"] = false;
                                beneficiaryConfig["visible"] = false;
                            }

                            if (Convert.ToString(dtbenfconfig.Rows[0]["Validate_BIC"]) == "0")
                            {
                                confiuration["beneficiaryBICCodeMandetory"] = true;
                                beneficiaryConfig["mandetory"] = true;
                            }
                            else
                            {
                                confiuration["beneficiaryBICCodeMandetory"] = false;
                                beneficiaryConfig["mandetory"] = false;
                            }

                            if (Convert.ToString(dtbenfconfig.Rows[0]["Range_check_BIC"]) == "1" && Convert.ToString(dtbenfconfig.Rows[0]["BIC_Status"]) == "0")
                            {
                                confiuration["beneficiaryBICCodeFixedLength"] = false;
                                confiuration["beneficiaryBICCodeMinLength"] = Convert.ToInt16(dtbenfconfig.Rows[0]["BIC_Min_Length"]);
                                confiuration["beneficiaryBICCodeMaxLength"] = Convert.ToInt16(dtbenfconfig.Rows[0]["BIC_Length"]);

                                beneficiaryConfig["minLength"] = Convert.ToInt16(dtbenfconfig.Rows[0]["BIC_Min_Length"]);
                                beneficiaryConfig["maxLength"] = Convert.ToInt16(dtbenfconfig.Rows[0]["BIC_Length"]);
                            }
                            else if (Convert.ToString(dtbenfconfig.Rows[0]["Range_check_BIC"]) == "0" && Convert.ToString(dtbenfconfig.Rows[0]["BIC_Status"]) == "0")
                            {
                                confiuration["beneficiaryBICCodeFixedLength"] = true;
                                confiuration["beneficiaryBICCodeLength"] = Convert.ToInt16(dtbenfconfig.Rows[0]["BIC_Length"]);

                                beneficiaryConfig["minLength"] = Convert.ToInt16(dtbenfconfig.Rows[0]["BIC_Min_Length"]);
                                beneficiaryConfig["maxLength"] = Convert.ToInt16(dtbenfconfig.Rows[0]["BIC_Length"]);
                            }

                            if (Convert.ToString(dtbenfconfig.Rows[0]["BIC_Lable"]).Trim() != "" && Convert.ToString(dtbenfconfig.Rows[0]["BIC_Status"]) == "0")
                            {
                                confiuration["BICCodeShowLable"] = Convert.ToString(dtbenfconfig.Rows[0]["BIC_Lable"]).Trim();

                                beneficiaryConfig["fieldName"] = confiuration["BICCodeShowLable"];
                                beneficiaryConfig["fieldPlaceHolder"] = confiuration["BICCodeShowLable"];
                            }
                            beneficiaryConfig["type"] = "text";
                            beneficiaryConfigureList.Add(beneficiaryConfig);

                        }

                        #endregion Bank_Details

                        #region Transactions_Configuration

                        if (Convert.ToString(dtbenfconfig.Rows[0]["Trans_limit_man"]) == "0")
                        {
                            confiuration["beneficiaryTransactionLimitSetting"] = true;
                            confiuration["beneficiaryMinTransactionLimit"] = dtbenfconfig.Rows[0]["Trans_limit_min"];
                            confiuration["beneficiaryMaxTransactionLimit"] = dtbenfconfig.Rows[0]["Trans_limit_max"];
                            confiuration["beneficiaryTransactionLimitPerDay"] = dtbenfconfig.Rows[0]["Trans_lmt_perday"];
                            confiuration["beneficiaryTransactionLimitPerDayPerBeneficiary"] = dtbenfconfig.Rows[0]["Trans_lmtt_perday_benf"];
                            confiuration["beneficiaryTransactionLimitPeryearPerBeneficiary"] = dtbenfconfig.Rows[0]["Trans_lmt_peryear_benf"];
                            confiuration["beneficiaryDenomantionAmount"] = dtbenfconfig.Rows[0]["denomination_rate"];
                        }
                        else if (Convert.ToString(dtbenfconfig.Rows[0]["Trans_limit_man"]) == "1")
                        {
                            confiuration["beneficiaryTransactionLimitSetting"] = false;
                        }

                        #endregion Transactions_Configuration

                        #region Custom_Settings

                        if (data.deliveryTypeID > 0)
                        {
                            if (true)
                            {
                                Transaction objT = JsonConvert.DeserializeObject<Transaction>(jsonData);
                                objT.Client_ID = data.clientID;
                                objT.Country_ID = data.countryID;
                                objT.CollectionPoint_ID = data.paymentCollectionTypeID;
                                objT.DeliveryType_Id = data.deliveryTypeID;
                                Service.srvDeliveryType srv = new Service.srvDeliveryType();
                                DataTable liDeliveryMap = srv.GetDeliveryMappingSettings(objT);
                                if (liDeliveryMap.Rows.Count > 0)
                                {
                                    confiuration["beneficiaryCustomSettings"] = true;
                                    confiuration["beneficiaryCustomSettingsTime"] = liDeliveryMap.Rows[0]["Time"];
                                    confiuration["beneficiaryCustomSettingsMinAmount"] = liDeliveryMap.Rows[0]["Min_Amount"];
                                    confiuration["beneficiaryCustomSettingsMaxAmount"] = liDeliveryMap.Rows[0]["Max_Amount"];
                                    confiuration["beneficiaryCustomSettingsWorkingDays"] = liDeliveryMap.Rows[0]["Woking_Days"];
                                    confiuration["beneficiaryCustomSettingsDeliveryTypeId"] = liDeliveryMap.Rows[0]["DeliveryType_ID"];
                                }
                                else
                                {
                                    confiuration["beneficiaryCustomSettings"] = false;
                                }
                                
                            }
                        }
                        #endregion Custom_Settings

                        #region IDUploadConfiguration

                        //ID Upload
                        if (Convert.ToString(dtbenfconfig.Rows[0]["Id_Upload_man"]) == "0")
                        {
                            confiuration["beneficiaryIdUploadMandatory"] = true;

                            beneficiaryConfig["fieldName"] = "Id Upload";
                            beneficiaryConfig["fieldID"] = "iduploadmandetory";
                            beneficiaryConfig["mandetory"] = true;
                            beneficiaryConfigureList.Add(beneficiaryConfig);
                        }
                        else
                        {
                            confiuration["beneficiaryIdUploadMandatory"] = false;

                            beneficiaryConfig["fieldName"] = "Id Upload";
                            beneficiaryConfig["fieldID"] = "iduploadmandetory";
                            beneficiaryConfig["mandetory"] = false;
                            beneficiaryConfigureList.Add(beneficiaryConfig);
                        }

                        //ID Number

                        if (Convert.ToString(dtbenfconfig.Rows[0]["ID_number_vis"]) == "0")
                        {
                            confiuration["beneficiaryIdNoVisibile"] = true;
                            confiuration["beneficiaryIdNoMandatory"] = true;
                        }
                        else if (Convert.ToString(dtbenfconfig.Rows[0]["ID_number_vis"]) == "1")
                        {
                            confiuration["beneficiaryIdNoVisibile"] = true;
                            confiuration["beneficiaryIdNoMandatory"] = false;
                        }
                        else if (Convert.ToString(dtbenfconfig.Rows[0]["ID_number_vis"]) == "2")
                        {
                            confiuration["beneficiaryIdNoVisibile"] = false;
                            confiuration["beneficiaryIdNoMandatory"] = false;
                        }

                        if (Convert.ToString(dtbenfconfig.Rows[0]["Range_ID_number"]) == "0")
                        {
                            confiuration["beneficiaryfixedID"] = true;
                            confiuration["beneficiaryCustomID"] = false;
                            confiuration["beneficiaryIDNumberminlength"] = Convert.ToInt32(dtbenfconfig.Rows[0]["ID_Number_max"]);
                            confiuration["beneficiaryIDNumbermaxlength"] = Convert.ToInt32(dtbenfconfig.Rows[0]["ID_Number_max"]);
                        }
                        else if (Convert.ToString(dtbenfconfig.Rows[0]["Range_ID_number"]) == "1")
                        {
                            confiuration["beneficiaryfixedID"] = false;
                            confiuration["beneficiaryCustomID"] = true;
                            confiuration["beneficiaryIDNumberminlength"] = Convert.ToInt32(dtbenfconfig.Rows[0]["ID_Number_min"]);
                            confiuration["beneficiaryIDNumbermaxlength"] = Convert.ToInt32(dtbenfconfig.Rows[0]["ID_Number_max"]);
                        }


                        //Issue date
                        if (Convert.ToString(dtbenfconfig.Rows[0]["Issue_date_vis"]) == "0")
                        {
                            confiuration["beneficiaryIssueDateVisibilty"] = true;
                            confiuration["beneficiaryIssueDateMandatory"] = true;

                        }
                        else if (Convert.ToString(dtbenfconfig.Rows[0]["Issue_date_vis"]) == "1")
                        {
                            confiuration["beneficiaryIssueDateVisibilty"] = true;
                            confiuration["beneficiaryIssueDateMandatory"] = false;

                        }
                        else if (Convert.ToString(dtbenfconfig.Rows[0]["Issue_date_vis"]) == "2")
                        {
                            confiuration["beneficiaryIssueDateVisibilty"] = false;
                            confiuration["beneficiaryIssueDateMandatory"] = false;

                        }

                        //Expiry Date
                        if (Convert.ToString(dtbenfconfig.Rows[0]["Expiry_date_vis"]) == "0")
                        {
                            confiuration["beneficiaryExpiryDateVisibilty"] = true;
                            confiuration["beneficiaryExpiryDateMandatory"] = true;

                        }
                        else if (Convert.ToString(dtbenfconfig.Rows[0]["Expiry_date_vis"]) == "1")
                        {
                            confiuration["beneficiaryExpiryDateVisibilty"] = true;
                            confiuration["beneficiaryExpiryDateMandatory"] = false;

                        }
                        else if (Convert.ToString(dtbenfconfig.Rows[0]["Expiry_date_vis"]) == "2")
                        {
                            confiuration["beneficiaryExpiryDateVisibilty"] = false;
                            confiuration["beneficiaryExpiryDateMandatory"] = false;

                        }

                        //Place of Issue
                        if (Convert.ToString(dtbenfconfig.Rows[0]["Plcae_of_Issue_vis"]) == "0")
                        {
                            confiuration["beneficiaryPlcaeOfIssueVisibilty"] = true;
                            confiuration["beneficiaryPlcaeOfIssueMandatory"] = true;

                        }
                        else if (Convert.ToString(dtbenfconfig.Rows[0]["Plcae_of_Issue_vis"]) == "1")
                        {
                            confiuration["beneficiaryPlcaeOfIssueVisibilty"] = true;
                            confiuration["beneficiaryPlcaeOfIssueMandatory"] = false;

                        }
                        else if (Convert.ToString(dtbenfconfig.Rows[0]["Plcae_of_Issue_vis"]) == "2")
                        {
                            confiuration["beneficiaryPlcaeOfIssueVisibilty"] = false;
                            confiuration["beneficiaryPlcaeOfIssueMandatory"] = false;

                        }

                        //Date of Birth
                        if (Convert.ToString(dtbenfconfig.Rows[0]["Date_of_Birth_vis"]) == "0")
                        {
                            confiuration["beneficiaryBirthDateVisibilty"] = true;
                            confiuration["beneficiaryBirthDateMandatory"] = true;

                        }
                        else if (Convert.ToString(dtbenfconfig.Rows[0]["Date_of_Birth_vis"]) == "1")
                        {
                            confiuration["beneficiaryBirthDateVisibilty"] = true;
                            confiuration["beneficiaryBirthDateMandatory"] = false;

                        }
                        else if (Convert.ToString(dtbenfconfig.Rows[0]["Date_of_Birth_vis"]) == "2")
                        {
                            confiuration["beneficiaryBirthDateVisibilty"] = false;
                            confiuration["beneficiaryBirthDateMandatory"] = false;

                        }

                        // Front Page of ID
                        if (Convert.ToString(dtbenfconfig.Rows[0]["Front_ID_vis"]) == "0")
                        {
                            confiuration["beneficiaryFrontPageIdVisibilty"] = true;
                            confiuration["beneficiaryFrontPageIdMandatory"] = true;

                        }
                        else if (Convert.ToString(dtbenfconfig.Rows[0]["Front_ID_vis"]) == "1")
                        {
                            confiuration["beneficiaryFrontPageIdVisibilty"] = true;
                            confiuration["beneficiaryFrontPageIdMandatory"] = false;

                        }
                        else if (Convert.ToString(dtbenfconfig.Rows[0]["Front_ID_vis"]) == "2")
                        {
                            confiuration["beneficiaryFrontPageIdVisibilty"] = false;
                            confiuration["beneficiaryFrontPageIdMandatory"] = false;

                        }



                        // Back Page of ID
                        if (Convert.ToString(dtbenfconfig.Rows[0]["Back_ID_vis"]) == "0")
                        {
                            confiuration["beneficiaryBackIdVisibilty"] = true;
                            confiuration["beneficiaryBackIdMandatory"] = true;

                        }
                        else if (Convert.ToString(dtbenfconfig.Rows[0]["Back_ID_vis"]) == "1")
                        {
                            confiuration["beneficiaryBackIdVisibilty"] = true;
                            confiuration["beneficiaryBackIdMandatory"] = false;

                        }
                        else if (Convert.ToString(dtbenfconfig.Rows[0]["Back_ID_vis"]) == "2")
                        {
                            confiuration["beneficiaryBackIdVisibilty"] = false;
                            confiuration["beneficiaryBackIdMandatory"] = false;

                        }

                        // Comment
                        if (Convert.ToString(dtbenfconfig.Rows[0]["Comment_vis"]) == "0")
                        {
                            confiuration["beneficiaryCommentVisibilty"] = true;
                            confiuration["beneficiaryCommentMandatory"] = true;

                        }
                        else if (Convert.ToString(dtbenfconfig.Rows[0]["Back_ID_vis"]) == "1")
                        {
                            confiuration["beneficiaryCommentVisibilty"] = true;
                            confiuration["beneficiaryCommentMandatory"] = false;

                        }
                        else if (Convert.ToString(dtbenfconfig.Rows[0]["Back_ID_vis"]) == "2")
                        {
                            confiuration["beneficiaryCommentVisibilty"] = false;
                            confiuration["beneficiaryCommentMandatory"] = false;

                        }


                        #endregion IDUploadConfiguration


                            confiurations.Add(confiuration);
                      //  confiurations.Add(configuration2);

                        var mobileProviderDataf = dtbenfconfig.Rows.OfType<DataRow>()
                   .Select(row => dtbenfconfig.Columns.OfType<DataColumn>()
                       .ToDictionary(col => col.ColumnName, c => row[c]));

                        returnJsonData = new { response = true, responseCode = "00", data = confiurations };
                        return new JsonResult(returnJsonData);                                             
                    }

                }

            }
            catch (Exception ex)
            {
                Model.ActivityLogTracker _objActivityLog = new Model.ActivityLogTracker();
                _objActivityLog.Activity = " Exception Beneficiary Configuration Error: " + ex.ToString() + " ";
                _objActivityLog.FunctionName = "beneficiaryconfiguration";
                _objActivityLog.Transaction_ID = 0;
                _objActivityLog.WhoAcessed = 1;
                _objActivityLog.Branch_ID = 0;
                _objActivityLog.Client_ID = 0;

                Service.srvActivityLogTracker srvActivityLog = new Service.srvActivityLogTracker();
                int _i = srvActivityLog.Create(_objActivityLog, HttpContext);
                CompanyInfo.InsertErrorLogTracker(" Exception Beneficiary Configuration Error: " + ex.ToString(), 0, 0, 0, 0, "beneficiaryconfiguration", Convert.ToInt32(data.Branch_ID), Convert.ToInt32(data.Client_ID), "", HttpContext);
                returnJsonData = new { response = false, responseCode = "01", data = "Field is missing." };
                return new JsonResult(returnJsonData);
            }
            #endregion
            return returnJsonData;
        }



        [HttpPost]
        [Route("searchbeneficiaryholdername")]
        public IActionResult SearchBeneficiaryHolderName([FromBody] JsonElement obj1)
        {
            HttpContext context = HttpContext;
            string json = System.Text.Json.JsonSerializer.Serialize(obj1);
            dynamic data = JObject.Parse(json); string Status = string.Empty;
            Model.response.WebResponse response = null;
            CompanyInfo.InsertrequestLogTracker("searchbeneficiaryholdername full request body: " + JObject.Parse(json), 0, 0, 0, 0, "SearchBeneficiaryHolderName", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
            Beneficiary obj = new Beneficiary();

            DataTable dt = new DataTable();

            var validateJsonData = (dynamic)null;
            // var cm = li.SingleOrDefault();

            int number;
            if (!int.TryParse(Convert.ToString(data.clientID), out number))
            {
                validateJsonData = new { response = false, responseCode = "02", data = "Invalid Client ID." };
                return new JsonResult(validateJsonData);
            }
            if (!int.TryParse(Convert.ToString(data.branchID), out number))
            {
                validateJsonData = new { response = false, responseCode = "02", data = "Invalid branch ID field." };
                return new JsonResult(validateJsonData);
            }
            if (data.beneficiaryCountryID == "" || data.beneficiaryCountryID == null)
            {
                validateJsonData = new { response = false, responseCode = "02", data = "Invalid Beneficiary Country ID." };
                return new JsonResult(validateJsonData);
            }


            try
            {
                try { obj.Client_ID = data.clientID; }catch(Exception ex) { obj.Client_ID = 0; }
                try { obj.Branch_ID = data.branchID;}catch(Exception ex) { obj.Branch_ID = 0; }
                 try {obj.Account_Number = data.accountNumber; } catch (Exception ex) { obj.Account_Number = "0"; }
                 try {obj.Beneficiary_Country_ID = data.beneficiaryCountryID; } catch (Exception ex) { obj.Beneficiary_Country_ID = 0; }
                 try {obj.BBank_ID = data.bankID; } catch (Exception ex) { obj.BBank_ID = 0; }
                 try {obj.Customer_ID = data.customerID; } catch (Exception ex) { obj.Customer_ID = "0"; }

                try { obj.Beneficiary_Mobile = data.Beneficiary_Mobile; } catch (Exception egx) { obj.Beneficiary_Mobile = ""; }
                try { obj.Mobile_provider = data.Mobile_provider; } catch (Exception egx) { obj.Mobile_provider = 0; }
                try { obj.Collection_type_Id = data.Collection_type_Id; } catch (Exception egx) { obj.Collection_type_Id = 0; }

                if (!SqlInjectionProtector.ReadJsonElementValues(obj1) || !SqlInjectionProtector.ReadJsonElementValuesScript(obj1))
                {
                    var errorResponse = new { response = false, responseCode = "02", data = "Invalid input detected." };
                    return new JsonResult(errorResponse) { StatusCode = 400 };
                }

                Service.srvBeneficiary srvBeneficiary = new Service.srvBeneficiary(context);

                obj = srvBeneficiary.AccHolderNameSearch(obj, context);
                if (obj.AccountHolderName != "" && obj.AccountHolderName != null)
                {
                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_SUCCESS);
                    response.ResponseCode = 0;
                    response.data = obj.AccountHolderName;

                    if(obj.AccountHolderName == "error") {
                        validateJsonData = new { response = false, responseCode = "02", data = "" };
                        return new JsonResult(validateJsonData);
                    }

                    validateJsonData = new { response = true, responseCode = "00", data = response.data };
                    return new JsonResult(validateJsonData);
                }
                if (obj.AccountHolderName == "" || obj.AccountHolderName != null)
                {
                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_SUCCESS);
                    response.ResponseCode = 0;
                    response.data = "";

                    validateJsonData = new { response = false, responseCode = "02", data = response.data };
                    return new JsonResult(validateJsonData);

                }
                //string activity = "Get account details : AccountHolderName :" + obj.AccountHolderName + "  ResponseCode: " + response.ResponseCode;
                string activity = "Get account details : AccountHolderName :" + obj.AccountHolderName + "    "  ;
                CompanyInfo.InsertActivityLogDetails("Get account details response <br>:" + activity, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(CompanyInfo.Decrypt((string)obj.Customer_ID, true)), "SearchBeneficiaryHolderName", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "Get Beneficiary Bank Account Details", context);


                validateJsonData = new { response = false, responseCode = "00", data = response.data };
                return new JsonResult(validateJsonData);
            }
            catch (Exception ex)
            {
                string Activity = "Api SearchBeneficiaryHolderName: " + ex.ToString() + " ";
                validateJsonData = new { response = false, responseCode = "01", data = "Invalid Request" };
                CompanyInfo.InsertErrorLogTracker(Activity.ToString(), 0, 0, 0, 0, "SearchBeneficiaryHolderName", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "", context);
                return new JsonResult(validateJsonData);
            }

        }

        [ApiExplorerSettings(IgnoreApi = true)]       
        static bool IsAllDigitsSame(string accountNumber)
        {
            if (string.IsNullOrEmpty(accountNumber)) return false;

            // Check if all digits are the same
            if (accountNumber.All(digit => digit == accountNumber[0]))
                return true;

            // Check if digits are in sequential order (either ascending or descending)
            bool isAscending = true, isDescending = true;
            for (int i = 1; i < accountNumber.Length; i++)
            {
                if (accountNumber[i] != accountNumber[i - 1] + 1)
                    isAscending = false;
                if (accountNumber[i] != accountNumber[i - 1] - 1)
                    isDescending = false;
            }

            return isAscending || isDescending;
        }
        //Start Parth added for validating first 5 digits is same i.e. zeroes
        #region validating first 5 digits is same i.e. zeroes
        static bool IsFirst5DigitsSame(string Number)
        {
            // Check if the first 4 digits are all '0'
            if (Number.Length >= 5 && Number.Substring(0, 5).All(digit => digit == '0'))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion validating first 5 digits is same i.e. zeroes
        //End Parth added for validating first 5 digits is same i.e. zeroes
        [HttpPost]
        [Authorize]
        [Route("beneficiaryadd")]
        public JsonResult saveBeneficiary([FromBody] JsonElement objdata)
        {
            var returnJsonData = (dynamic)null;
            Model.response.WebResponse response = null;
            DataTable dt_info = new DataTable();
            string jsonData = System.Text.Json.JsonSerializer.Serialize(objdata);
            dynamic data = JObject.Parse(jsonData);
            var claimsIdentity = User.Identity as ClaimsIdentity;//sanket
            var authHeader = "";
            try { authHeader = HttpContext.Request.Headers["Authorization"].ToString(); } catch (Exception egx) { authHeader = ""; }

            bool auth = AuthController.checkAuth(claimsIdentity, objdata, Convert.ToString(authHeader));
            if (!auth)
            {
                var errorResponse = new { response = false, responseCode = "403", data = "Access Forbidden" };
                return new JsonResult(errorResponse) { StatusCode = (int)HttpStatusCode.Forbidden };
            }
            CompanyInfo.InsertrequestLogTracker("saveBeneficiary full request body: " + JObject.Parse(jsonData), 0, 0, 0, 0, "saveBeneficiary", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
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
                        "AccountHolderName" => "Invalid input detected for Account Holder Name",
                        "Account_Number" => "Invalid input detected for Account Number",
                        "BankCode" => "Invalid input detected for Bank Code",
                        "beneficiaryAddress" => "Invalid input detected for Address",
                        "Beneficiary_Address1" => "Invalid input detected for Address",
                        "Beneficiary_CityName" => "Invalid input detected for City Name",
                        "Beneficiary_CountryName" => "Invalid input detected for Country Name",
                        "beneficiaryMobile" => "Invalid input detected for Mobile",
                        "beneficiaryName" => "Invalid input detected for Name",
                        "Beneficiary_PostCode" => "Invalid input detected for Post Code",
                        "Beneficiary_Telephone" => "Invalid input detected for Telephone",
                        "Benf_BIC" => "Invalid input detected for BIC",
                        "Benf_Iban" => "Invalid input detected for Iban",
                        "Birth_Date" => "Invalid input detected for Birth Date",
                        "Branch" => "Invalid input detected for Branch",
                        "BranchCode" => "Invalid input detected for Branch Code",
                        "First_Name" => "Invalid input detected for First Name",
                        "Ifsc_Code" => "Invalid input detected for Ifsc Code",
                        "Last_Name" => "Invalid input detected for Last Name",
                        "Middle_Name" => "Invalid input detected for Middle Name",
                        "Wallet_provider" => "Invalid input detected for Wallet Provider",
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

               

                string responseMessage = "";

                if (!int.TryParse(Convert.ToString(data.clientID), out int number))
                {
                    returnJsonData = new { response = false, responseCode = "02", data = "Invalid Client ID." };
                    return new JsonResult(returnJsonData);
                }
                if (data.clientID == "" || data.clientID == null)
                {
                    returnJsonData = new { response = false, responseCode = "02", data = "Invalid Client ID." };
                    return new JsonResult(returnJsonData);
                }
                else if (data.paymentCollectionTypeID == "" || data.paymentCollectionTypeID == null)
                {
                    returnJsonData = new { response = false, responseCode = "02", data = "Invalid Collection Type." };
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
                else if (data.customerID == null || data.customerID == "")
                {
                    returnJsonData = new { response = false, responseCode = "02", data = "Invalid Customer ID." };
                    return new JsonResult(returnJsonData);
                }

            }
            catch (Exception ex)
            {
                Model.ActivityLogTracker _objActivityLog = new Model.ActivityLogTracker();
                _objActivityLog.Activity = " Exception Beneficiary Create Error: " + ex.ToString() + " ";
                _objActivityLog.FunctionName = "saveBeneficiary";
                _objActivityLog.Transaction_ID = 0;
                _objActivityLog.WhoAcessed = 1;
                _objActivityLog.Branch_ID = 0;
                _objActivityLog.Client_ID = 0;

                Service.srvActivityLogTracker srvActivityLog = new Service.srvActivityLogTracker();
                int _i = srvActivityLog.Create(_objActivityLog, HttpContext);
                CompanyInfo.InsertErrorLogTracker(" Exception saveBeneficiary Error: " + ex.ToString(), 0, 0, 0, 0, "saveBeneficiary", Convert.ToInt32(data.Branch_ID), Convert.ToInt32(data.Client_ID), "", HttpContext);
                returnJsonData = new { response = false, responseCode = "01", data = "Field missing or Invalid field data." };
                return new JsonResult(returnJsonData);
            }
            #endregion

            
            try
            {
                string userAgent = Request.Headers[HeaderNames.UserAgent];
                //Beneficiary obj = JsonConvert.DeserializeObject<Beneficiary>(jsonData);
                Beneficiary obj = new Beneficiary();
                obj.Collection_type_Id = data.paymentCollectionTypeID;
                obj.Country_Code = data.Country_Code;
                obj.Client_ID = data.clientID;
                obj.Branch_ID = data.branchID;
                obj.Country_ID = data.countryID;
                obj.Beneficiary_Country_ID = obj.Country_ID;
                obj.userAgent = userAgent;
                obj.Customer_ID = data.customerID;

                // Personal Details
                obj.Beneficiary_Name = data.beneficiaryName;
                string[] nameParts = obj.Beneficiary_Name.Split(' ');
                obj.First_Name = "";
                obj.Middle_Name = "";
                obj.Last_Name = "";
                if (nameParts.Length >= 1)
                {
                    // First name is the first element
                    obj.First_Name = nameParts[0];

                    // Check if there is a middle name
                    if (nameParts.Length >= 3)
                    {
                        // Middle name is the second element
                        obj.Middle_Name = nameParts[1];

                        // Last name is the third element and beyond
                        obj.Last_Name = string.Join(" ", nameParts, 2, nameParts.Length - 2);
                    }
                    else if (nameParts.Length == 2)
                    {
                        // If there are only two name parts, consider the second as the last name
                        obj.Last_Name = nameParts[1];
                    }
                    // If there's only one name part, it will be considered as the first name
                }


                obj.Beneficiary_Address = data.beneficiaryAddress;

                if (Convert.ToString(data.relationID).Trim() == "" || Convert.ToString(data.relationID).Trim() == null) { data.relationID = "0"; }

                if (Convert.ToString(data.providerID).Trim() == "" || Convert.ToString(data.providerID).Trim() == null) { data.providerID = "0"; }

                obj.Relation_ID = data.relationID;
                obj.Beneficiary_Mobile = data.beneficiaryMobile;

                try { obj.Beneficiary_City_ID = data.cityID; } catch (Exception ex) { obj.Beneficiary_City_ID = 0; }


                obj.Beneficiary_Address1 = "";
                obj.Beneficiary_Telephone = "";
                
                
                obj.Delete_Status = 0;
                obj.Created_By_User_ID = 1;
                obj.Agent_MappingID = 0;
                obj.cashcollection_flag = obj.Collection_type_Id;

                if (data.paymentCollectionTypeID == 1)
                {   // Bank Beneficiary
                    try
                    {
                        obj.BankBranch_ID = data.BankBranch_ID;
                    }
                    catch (Exception egx) { obj.BankBranch_ID = 0; }

                    obj.Account_Number = data.Account_Number;

                    // Parth Changes for Confirm Account Number
                    try
                    {
                        obj.Confirm_Account_Number = data.Confirm_Account_Number;
                        if (obj.Confirm_Account_Number == null) { obj.Confirm_Account_Number = ""; }
                    }
                    catch (Exception ex) { obj.Confirm_Account_Number = ""; }
                    //End Parth Changes for Confirm Account Number

                    obj.AccountHolderName = data.AccountHolderName;
                    obj.BankCode = data.BankCode;
                    obj.Branch = data.Branch;
                    obj.Ifsc_Code = data.Ifsc_Code;
                    obj.BranchCode = data.BranchCode;
                    obj.Benf_Iban = data.Benf_Iban;
                    obj.Benf_BIC = data.Benf_BIC;
                    try
                    {
                        obj.BBank_ID = data.BBank_ID;
                    }
                    catch (Exception ex) { obj.BBank_ID = 0; }
                }
                else if (data.paymentCollectionTypeID == 3)
                {   // Mobile wallet Beneficiary
                    obj.Mobile_provider = data.providerID;
                    obj.Wallet_Id = "";
                    obj.Wallet_provider = 0;
                }

                try
                {
                    obj.Beneficiary_Gender = Convert.ToInt32( data.Beneficiary_Gender);
                }
                catch(Exception ex) { obj.Beneficiary_Gender = 0; }

                try
                {
                    obj.Beneficiary_PostCode = Convert.ToString(data.Beneficiary_PostCode);
                }
                catch (Exception ex) { obj.Beneficiary_PostCode = ""; }
                try
                {
                    obj.Beneficiary_State = Convert.ToString(data.Beneficiary_State).Trim();
                }
                catch (Exception ex) { obj.Beneficiary_State = ""; }

                try
                {
                    obj.Birth_Date = Convert.ToString(data.Birth_Date);
                }
                catch (Exception ex) { obj.Birth_Date = null; }

                int Customer_ID = Convert.ToInt32(CompanyInfo.Decrypt(obj.Customer_ID, true));

                #region requiredValidation
                string validbeneficiaryName = valideBeneficiaryData(obj, "Beneficiary_Name");
                if (validbeneficiaryName == "T") // For Beneficiary_Name
                {
                    returnJsonData = new { response = false, responseCode = "02", data = "Beneficiary Name is required." };
                    return new JsonResult(returnJsonData);
                }

                string validbeneficiaryRelation = valideBeneficiaryData(obj, "Beneficiary_relation");
                if (validbeneficiaryRelation == "T") // For Beneficiary_relation
                {
                    returnJsonData = new { response = false, responseCode = "02", data = "Beneficiary Relation is required." };
                    return new JsonResult(returnJsonData);
                }

                string validbeneficiaryAddress = valideBeneficiaryData(obj, "Beneficiary_Address");
                if (validbeneficiaryAddress == "T") // For Beneficiary_Address
                {
                    returnJsonData = new { response = false, responseCode = "02", data = "Beneficiary Address is required." };
                    return new JsonResult(returnJsonData);
                }

                string validbeneficiaryMobile = valideBeneficiaryData(obj, "Beneficiary_Mobile");
                if (validbeneficiaryMobile == "T") // For Beneficiary_Mobile
                {
                    returnJsonData = new { response = false, responseCode = "02", data = "Beneficiary Mobile is required." };
                    return new JsonResult(returnJsonData);
                }

                string validbeneficiaryMobileRange = valideBeneficiaryRangeDataData(obj, "Beneficiary_Mobile");
                if (validbeneficiaryMobileRange != "") // For Beneficiary_Mobile
                {
                    returnJsonData = new { response = false, responseCode = "02", data = validbeneficiaryMobileRange };
                    return new JsonResult(returnJsonData);
                }

                string validbeneficiaryCity = valideBeneficiaryData(obj, "Beneficiary_City");
                if (validbeneficiaryCity == "T") // For Beneficiary_City
                {
                    returnJsonData = new { response = false, responseCode = "02", data = "Beneficiary City is required." };
                    return new JsonResult(returnJsonData);
                }

                if (obj.Collection_type_Id == 3) // Mobile wallet Validate
                {
                    //For Mobile Wallet
                    string validbeneficiaryMobileProvider = valideBeneficiaryData(obj, "mobile_provider");
                    if (validbeneficiaryMobileProvider == "T") // For mobile_provider
                    {
                        returnJsonData = new { response = false, responseCode = "02", data = "Mobile Operator is required." };
                        return new JsonResult(returnJsonData);
                    }
                }

                if (obj.Collection_type_Id == 1) // Bank Account Validate
                {
                    //For Bank transfer
                    string validbeneficiaryAccNumber = valideBeneficiaryBankData(obj, "Verify_Account_no");
                    if (validbeneficiaryAccNumber == "T") // For Verify_Account_no
                    {
                        returnJsonData = new { response = false, responseCode = "02", data = "Account Number is required." };
                        return new JsonResult(returnJsonData);
                    }

                    string validbeneficiaryAccNumberMinRange = valideBeneficiaryRangeDataData(obj, "Range_check_accno");
                    if (validbeneficiaryAccNumberMinRange != "") // For Range_check_accno
                    {
                        returnJsonData = new { response = false, responseCode = "02", data = validbeneficiaryAccNumberMinRange };
                        return new JsonResult(returnJsonData);
                    }


                    // Parth Changes for Confirm Account number
                    try
                    {
                        string validbeneficiaryConfirmAccNumber = valideBeneficiaryBankData(obj, "ConfirmAccNo_man"); // For empty check and mandetory
                        if (validbeneficiaryConfirmAccNumber == "T") // For Verify_Confirm_Account_no
                        {
                            returnJsonData = new { response = false, responseCode = "02", data = "Confirm Account Number is required." };
                            return new JsonResult(returnJsonData);
                        }
                    }
                    catch (Exception egx) { }

                    string validbeneficiaryConfAccNumberMinRange = valideConfAccNoRangeData(obj, "Range_check_accno");
                    if (validbeneficiaryConfAccNumberMinRange != "") // For Range_check_confaccno
                    {
                        returnJsonData = new { response = false, responseCode = "02", data = validbeneficiaryConfAccNumberMinRange };
                        return new JsonResult(returnJsonData);
                    }

                    string validbeneficiaryConfirmAccountNumber = valideBeneficiaryAccNo(obj, "Confirm_Account_Number");
                    if (validbeneficiaryConfirmAccountNumber == "T") // For Confirm_Account_no
                    {
                        returnJsonData = new { response = false, responseCode = "02", data = " Confirm Account Number does not matches with Account Number." };
                        return new JsonResult(returnJsonData);
                    }

                    // End Parth Changes for Confirm Account number

                    string validbeneficiaryBIC_CodeNumber = valideBeneficiaryRangeDataData(obj, "Validate_BIC");
                    if (validbeneficiaryBIC_CodeNumber != "") // For Validate_BIC
                    {
                        returnJsonData = new { response = false, responseCode = "02", data = validbeneficiaryBIC_CodeNumber };
                        return new JsonResult(returnJsonData);
                    }

                    string validbeneficiaryIBAN_CodeNumber = valideBeneficiaryRangeDataData(obj, "IBAN_Mandatory");
                    if (validbeneficiaryIBAN_CodeNumber != "") // For IBAN_Mandatory
                    {
                        returnJsonData = new { response = false, responseCode = "02", data = validbeneficiaryIBAN_CodeNumber };
                        return new JsonResult(returnJsonData);
                    }

                    string validbeneficiaryBranch_Code = valideBeneficiaryRangeDataData(obj, "Branch_Code");
                    if (validbeneficiaryBranch_Code != "") // For Branch_Code
                    {
                        returnJsonData = new { response = false, responseCode = "02", data = validbeneficiaryBranch_Code };
                        return new JsonResult(returnJsonData);
                    }

                    string validbeneficiaryIFSC = valideBeneficiaryRangeDataData(obj, "IFSC_Mandatory");
                    if (validbeneficiaryIFSC != "") // For IFSC_Mandatory
                    {
                        returnJsonData = new { response = false, responseCode = "02", data = validbeneficiaryIFSC };
                        return new JsonResult(returnJsonData);
                    }

                    string validbeneficiaryBranch = valideBeneficiaryRangeDataData(obj, "Branch");
                    if (validbeneficiaryBranch != "") // For Branch
                    {
                        returnJsonData = new { response = false, responseCode = "02", data = validbeneficiaryBranch };
                        return new JsonResult(returnJsonData);
                    }

                    string validbeneficiaryBankCode = valideBeneficiaryRangeDataData(obj, "BankCode");
                    if (validbeneficiaryBankCode != "") // For Bankcode
                    {
                        returnJsonData = new { response = false, responseCode = "02", data = validbeneficiaryBankCode };
                        return new JsonResult(returnJsonData);
                    }

                    string validbeneficiaryBank_Name = valideBeneficiaryRangeDataData(obj, "Bank_Name");
                    if (validbeneficiaryBank_Name != "") // For Bank_Name
                    {
                        returnJsonData = new { response = false, responseCode = "02", data = validbeneficiaryBank_Name };
                        return new JsonResult(returnJsonData);
                    }
                }

                #endregion

                Model.Beneficiary _ObjCustomer = new Model.Beneficiary();
                List<Model.CompanyDetails> li = new List<Model.CompanyDetails>();
                var cm = li.SingleOrDefault();

                if(obj.Account_Number !="" && obj.Account_Number != null && data.paymentCollectionTypeID == 1)
                {
                   bool validateAccountNumber =  IsAllDigitsSame(obj.Account_Number);
                    if (validateAccountNumber)
                    {
                        returnJsonData = new { response = false, responseCode = "02", data = "Invalid account number." };
                        return new JsonResult(returnJsonData);
                    }
                }
                //Start Parth Added for validating first 5 digits is same i.e. zeroes
                #region validating all digits same
                try
                {
                    bool validateIfscCode = IsFirst5DigitsSame(obj.Ifsc_Code);
                    if (validateIfscCode)
                    {
                        returnJsonData = new { response = false, responseCode = "02", data = "Invalid IFSC Code." };
                        return new JsonResult(returnJsonData);
                    }
                    bool validateBranchCode= IsFirst5DigitsSame(obj.BranchCode);
                    if (validateBranchCode)
                    {
                        returnJsonData = new { response = false, responseCode = "02", data = "Invalid Branch Code." };
                        return new JsonResult(returnJsonData);
                    }
                    bool validateIbanCode = IsFirst5DigitsSame(obj.Benf_Iban);
                    if (validateIbanCode)
                    {
                        returnJsonData = new { response = false, responseCode = "02", data = "Invalid IBAN Code." };
                        return new JsonResult(returnJsonData);
                    }
                }
                catch (Exception ex)
                {
                    _ = Task.Run(() => CompanyInfo.InsertErrorLogTracker(" Error: " + ex.ToString(), 0, 0, 0, 0, "saveBeneficiary", Convert.ToInt32(data.Branch_ID), Convert.ToInt32(data.Client_ID), "", HttpContext));
                }
                #endregion validating all digits same
                //End Parth Added for validating first 5 digits is same i.e. zeroes
                Service.srvBeneficiary srvBenf = new Service.srvBeneficiary(HttpContext);
                response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_SUCCESS);

                int testingpurpose = 0;
                try
                {
                    testingpurpose = data.testingpurpose;
                }
                catch (Exception eg) { testingpurpose = 0; }

                if (obj.Beneficiary_Mobile != "" && 1 == 2 )
                {

                    Service.srvBeneficiary srv3 = new Service.srvBeneficiary(HttpContext);
                    DataTable li1 = srv3.get_benf_mobile_count(obj);

                    DataTable dtc = CompanyInfo.get(obj.Client_ID, HttpContext);


                    if (dtc.Rows.Count > 0 && li1 != null && li1.Rows.Count > 0)
                    {
                        dt_info.Columns.Add("Company_Cnt", typeof(int));
                        dt_info.Columns.Add("found_cnt", typeof(int));
                        var Company_cnt = Convert.ToInt16(dtc.Rows[0]["Count_Duplicate_Beneficiary_Mobile"]);
                        var found_cnt = Convert.ToInt16(li1.Rows[0]["count"]);
                        dt_info.Rows.Add(Company_cnt, found_cnt); // add values in table

                        var relationshipData = dt_info.Rows.OfType<DataRow>()
                       .Select(row => dt_info.Columns.OfType<DataColumn>()
                       .ToDictionary(col => col.ColumnName, c => row[c].ToString()));

                        if (Convert.ToInt16(dtc.Rows[0]["Count_Duplicate_Beneficiary_Mobile"]) > Convert.ToInt16(li1.Rows[0]["count"]))
                        {
                            response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_SUCCESS);
                            response.data = "Success";

                        }
                        else
                        {
                            response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_SUCCESS);
                            response.data = "Beneficiary Mobile Number Already Exist";
                            response.ObjData = dt_info;
                            response.ResponseCode = 2;
                            returnJsonData = new { response = false, responseCode = "02", data = response.data };
                            return new JsonResult(returnJsonData);
                        }



                    }

                }
                
                if (testingpurpose == 0)
                _ObjCustomer = srvBenf.Create(obj, HttpContext);
                else
                _ObjCustomer = srvBenf.Create_new(obj, HttpContext);


                response.ObjData = _ObjCustomer;
                if (_ObjCustomer.Id == 0 && _ObjCustomer.whereclause == "Validation Failed")
                {
                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_TECHNICAL_ERROR);
                    response.data = "Validation Failed";
                    response.ObjData = (_ObjCustomer.Message).ToString();
                    response.ResponseCode = 6;
                    returnJsonData = new { response = false, responseCode = "02", data = _ObjCustomer.Message };
                    return new JsonResult(returnJsonData);
                }
                else if (_ObjCustomer.Id == -1)
                {
                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_TECHNICAL_ERROR);
                    response.ResponseMessage = cm.ResponseMessage;
                    response.ResponseCode = 5;
                    //return response;
                    returnJsonData = new { response = false, responseCode = "02", data = cm.ResponseMessage };
                    return new JsonResult(returnJsonData);
                }
                else if (_ObjCustomer.Message == "success")
                {

                    List<Dictionary<string, object>> beneficiaryData = new List<Dictionary<string, object>>();
                    Dictionary<string, object> beneficiary = new Dictionary<string, object>();

                    beneficiary["beneficiaryID"] = _ObjCustomer.Beneficiary_ID;
                    beneficiaryData.Add(beneficiary);

                    returnJsonData = new { response = true, responseCode = "00", data = beneficiaryData };
                    return new JsonResult(returnJsonData);
                }
                else if (_ObjCustomer.Message == "exist_Beneficiary")
                {
                    returnJsonData = new { response = false, responseCode = "02", data = "Beneficiary record already exist." };
                    return new JsonResult(returnJsonData);
                }

            }
            catch (Exception ex)
            {
                try
                {
                    CompanyInfo.InsertErrorLogTracker(" Error: " + ex.ToString(), 0, 0, 0, 0, "saveBeneficiary", Convert.ToInt32(data.Branch_ID), Convert.ToInt32(data.Client_ID), "", HttpContext);
                }
                catch { }

                returnJsonData = new { response = false, responseCode = "01", data = "Invalid request." };
                return new JsonResult(returnJsonData);
            }
            return new JsonResult(returnJsonData);
        }

        [HttpPost]
        [Authorize]
        [Route("select_biccodes")]
        public IActionResult select_biccodes([FromBody] JsonElement objd)
        {
            var returnJsonData = (dynamic)null;
            HttpContext context = HttpContext;
            string json = System.Text.Json.JsonSerializer.Serialize(objd);
            var validateJsonData = (dynamic)null;
            dynamic data = JObject.Parse(json);
            var claimsIdentity = User.Identity as ClaimsIdentity;//sanket
            var authHeader = "";
            try { authHeader = HttpContext.Request.Headers["Authorization"].ToString(); } catch (Exception egx) { authHeader = ""; }

            bool auth = AuthController.checkAuth(claimsIdentity, objd, Convert.ToString(authHeader));
            if (!auth)
            {
                var errorResponse = new { response = false, responseCode = "403", data = "Access Forbidden" };
                return new JsonResult(errorResponse) { StatusCode = (int)HttpStatusCode.Forbidden };
            }
            Beneficiary obj1 = new Beneficiary();
            Model.response.WebResponse response = null;
            DataTable dt = new DataTable();
            try
            {
                Bank obj = new Bank();
                obj.Id = Convert.ToInt32(data.bankID);
                List<Model.Bank> _lst = new List<Model.Bank>();

                if (!SqlInjectionProtector.ReadJsonElementValues(objd) || !SqlInjectionProtector.ReadJsonElementValuesScript(objd))
                {
                    var errorResponse = new { response = false, responseCode = "02", data = "Invalid input detected." };
                    return new JsonResult(errorResponse) { StatusCode = 400 };
                }

                Service.srvBank srv = new Service.srvBank();
                _lst = srv.GetBICCodes_ByBank(obj);
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

                    returnJsonData = new { response = true, responseCode = "00", data = relationshipData };
                    return new JsonResult(returnJsonData);
                }
                else
                {
                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_TECHNICAL_ERROR);
                    response.ResponseMessage = "No Records Found.";
                    response.ResponseCode = 0;
                    returnJsonData = new { response = false, responseCode = "02", data = response.ResponseMessage };
                    return returnJsonData;
                }


            }
            catch (Exception ex)
            {
                Model.ErrorLog objError = new Model.ErrorLog();
                objError.User = new Model.User();
                objError.Branch = new Model.Branch();
                objError.Client = new Model.Client();

                objError.Error = "Api : Select_Bank --" + ex.ToString();
                objError.Branch.Id = 1;
                objError.Date = DateTime.Now;
                objError.User.Id = 1;
                objError.Id = 1;
                objError.Function_Name = "Select_BICCodes";
                Service.srvErrorLog srvError = new Service.srvErrorLog();
                srvError.Create(objError, context);
                returnJsonData = new { response = false, responseCode = "01", data = "Invalid Response" };
                return returnJsonData;
            }
        }


        #region for merging 3 api's getdeliverytype, getpaytype, collectiontypelist
        //Parth added for merging 3 api's getdeliverytype, getpaytype, collectiontypelist
        [HttpPost]
        [Route("collection_delivery_paytypes")]
        /*public JsonResult MergedApi([FromBody] JsonElement objdata)
        {
            var returnJsonData = (dynamic)null;
            Model.response.WebResponse response = null;
            string json = System.Text.Json.JsonSerializer.Serialize(objdata);
            dynamic data = JObject.Parse(json);
            CompanyInfo.InsertrequestLogTracker("Merged API full request body: " + JObject.Parse(json), 0, 0, 0, 0, "collection_delivery_paytypes", 0, 0, "", HttpContext);

            try
            {
                if (!SqlInjectionProtector.ReadJsonElementValues(objdata) || !SqlInjectionProtector.ReadJsonElementValuesScript(objdata))
                {
                    return new JsonResult(new { response = false, responseCode = "02", data = "Invalid input detected." }) { StatusCode = 400 };
                }
               
                Transaction obj = JsonConvert.DeserializeObject<Transaction>(json);
             
                string webstring = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("ConnectionStrings")["WEB_DB_CONN"];
                webstring = webstring.ToLower();
                if (webstring.IndexOf("kobo", 0, StringComparison.OrdinalIgnoreCase) != -1)
                {
                    obj.Basecurr_Country_ID = 0;  
                }

             
                Service.srvPaymentDepositType srv = new Service.srvPaymentDepositType();
                DataTable collectionData = srv.GetPayDepositTypes(obj);
                var collectionList = collectionData?.Rows.OfType<DataRow>()
                    .Select(row => collectionData.Columns.OfType<DataColumn>()
                    .ToDictionary(col => col.ColumnName, c => row[c].ToString()));

               
                MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("GetPaymentTypes");
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                int SourceID = 1;
                string Whereclause = "";
                if (obj.Is_App == 0)
                { SourceID = 3; }
                else if (obj.Branch_ID == 2) { SourceID = 2; }
                _cmd.Parameters.AddWithValue("_Source_ID", SourceID);
                if (obj.Basecurr_Country_ID != 0)
                { Whereclause = Whereclause + " And Base_Country_Id =" + obj.Basecurr_Country_ID; }
                _cmd.Parameters.AddWithValue("_whereclause", Whereclause);
                DataTable payTypeData = db_connection.ExecuteQueryDataTableProcedure(_cmd);
                var payTypeList = payTypeData?.Rows.OfType<DataRow>()
                    .Select(row => payTypeData.Columns.OfType<DataColumn>()
                    .ToDictionary(col => col.ColumnName, c => row[c].ToString()));

              
                MySqlConnector.MySqlCommand _cmdDelivery = new MySqlConnector.MySqlCommand("GetDeliveryTypes");
                _cmdDelivery.CommandType = CommandType.StoredProcedure;
                _cmdDelivery.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                _cmdDelivery.Parameters.AddWithValue("_Country_ID", obj.Country_ID);
                DataTable deliveryData = db_connection.ExecuteQueryDataTableProcedure(_cmdDelivery);
                var deliveryList = deliveryData?.Rows.OfType<DataRow>()
                    .Select(row => deliveryData.Columns.OfType<DataColumn>()
                    .ToDictionary(col => col.ColumnName, c => row[c].ToString()));

                return new JsonResult(new { response = true, responseCode = "00", data = new { collectionTypes = collectionList, paymentTypes = payTypeList, deliveryTypes = deliveryList } });
            }
            catch (Exception ex)
            {
                CompanyInfo.InsertErrorLogTracker(ex.ToString(), 0, 0, 0, 0, "MergedApi", 0, 0, "", HttpContext);
                return new JsonResult(new { response = false, responseCode = "01", data = "Invalid request." });
            }
        }
        */
        public async Task<JsonResult> MergedApiAsync([FromBody] JsonElement objdata)
        {
            var returnJsonData = (dynamic)null;
            Model.response.WebResponse response = null;
            string json = System.Text.Json.JsonSerializer.Serialize(objdata);
            dynamic data = JObject.Parse(json);
            var claimsIdentity = User.Identity as ClaimsIdentity;//sanket
            var authHeader = "";
            try { authHeader = HttpContext.Request.Headers["Authorization"].ToString(); } catch (Exception egx) { authHeader = ""; }

            bool auth = AuthController.checkAuth(claimsIdentity, objdata, Convert.ToString(authHeader));
            if (!auth)
            {
                var errorResponse = new { response = false, responseCode = "403", data = "Access Forbidden" };
                return new JsonResult(errorResponse) { StatusCode = (int)HttpStatusCode.Forbidden };
            }
            // Log request asynchronously
            _ = Task.Run(() => CompanyInfo.InsertrequestLogTracker("MergedApiAsync API full request body: " + JObject.Parse(json), 0, 0, 0, 0, "collection_delivery_paytypes1", 0, 0, "", HttpContext));

            try
            {
                if (!SqlInjectionProtector.ReadJsonElementValues(objdata) || !SqlInjectionProtector.ReadJsonElementValuesScript(objdata))
                {
                    return new JsonResult(new { response = false, responseCode = "02", data = "Invalid input detected." }) { StatusCode = 400 };
                }

                Transaction obj = JsonConvert.DeserializeObject<Transaction>(json);

                string webstring = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("ConnectionStrings")["WEB_DB_CONN"];
                webstring = webstring.ToLower();
                if (webstring.IndexOf("kobo", 0, StringComparison.OrdinalIgnoreCase) != -1)
                {
                    obj.Basecurr_Country_ID = 0; // Remove This after kobo. This is for just Kobo
                }

                // Fetch Collection Types Asynchronously 
                Task<DataTable> collectionTask = Task.Run(() =>
                {
                    Service.srvPaymentDepositType srv = new Service.srvPaymentDepositType();
                    return srv.GetPayDepositTypes(obj);
                });

                // Fetch Payment Types Asynchronously 
                Task<DataTable> paymentTask = Task.Run(() =>
                {
                    MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("GetPaymentTypes");
                    _cmd.CommandType = CommandType.StoredProcedure;
                    _cmd.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                    //int SourceID = obj.Is_App == 0 ? 3 : obj.Branch_ID == 2 ? 2 : 1;
                    //_cmd.Parameters.AddWithValue("_Source_ID", SourceID);
                    //string Whereclause = obj.Basecurr_Country_ID != 0 ? $" And Base_Country_Id ={obj.Basecurr_Country_ID}" : "";
                    int SourceID = 1;
                    string Whereclause = "";
                    if (obj.Is_App == 0)
                    { SourceID = 3; }
                    else if (obj.Branch_ID == 2) { SourceID = 2; }
                    _cmd.Parameters.AddWithValue("_Source_ID", SourceID);
                    if (obj.Basecurr_Country_ID != 0)
                    { Whereclause = Whereclause + " And Base_Country_Id =" + obj.Basecurr_Country_ID; }
                    _cmd.Parameters.AddWithValue("_whereclause", Whereclause);
                    //return db_connection.ExecuteQueryDataTableProcedure(_cmd);
                    DataTable dt = db_connection.ExecuteQueryDataTableProcedure(_cmd);
                    foreach (DataRow row in dt.Rows)
                    {
                        row["Review_Transfer_Message"] = ConvertAsciiToSpecialChars(Convert.ToString(row["Review_Transfer_Message"]));
                    }
                    return dt;

                });

                // Fetch Delivery Types Asynchronously 
                Task<DataTable> deliveryTask = Task.Run(() =>
                {
                    MySqlConnector.MySqlCommand _cmdDelivery = new MySqlConnector.MySqlCommand("GetDeliveryTypes");
                    _cmdDelivery.CommandType = CommandType.StoredProcedure;
                    _cmdDelivery.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                    _cmdDelivery.Parameters.AddWithValue("_Country_ID", obj.Country_ID);
                    return db_connection.ExecuteQueryDataTableProcedure(_cmdDelivery);
                });

                
                //Fetch Currency List
                Task<DataTable> currencyTask = Task.Run(() =>
                {                    
                    MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("getMultiCurrency");
                    string whereclause = "";
                    if (obj.Country_ID > 0)
                    {
                        whereclause = " and c.Country_ID=" + obj.Country_ID + "";
                    }
                    _cmd.CommandType = CommandType.StoredProcedure;
                    _cmd.Parameters.AddWithValue("_whereclause", whereclause);
                    _cmd.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                    return db_connection.ExecuteQueryDataTableProcedure(_cmd);                    
                });

                

                // Wait for all tasks to complete
                await Task.WhenAll(collectionTask, paymentTask, deliveryTask, currencyTask);

                // Convert DataTables to List
                var collectionList = collectionTask.Result?.Rows.OfType<DataRow>()
                    .Select(row => collectionTask.Result.Columns.OfType<DataColumn>()
                    .ToDictionary(col => col.ColumnName, c => row[c].ToString()));

                var payTypeList = paymentTask.Result?.Rows.OfType<DataRow>()
                    .Select(row => paymentTask.Result.Columns.OfType<DataColumn>()
                    .ToDictionary(col => col.ColumnName, c => row[c].ToString()));

                var deliveryList = deliveryTask.Result?.Rows.OfType<DataRow>()
                    .Select(row => deliveryTask.Result.Columns.OfType<DataColumn>()
                    .ToDictionary(col => col.ColumnName, c => row[c].ToString()));

                var currencyList = currencyTask.Result?.Rows.OfType<DataRow>()
                    .Select(row => currencyTask.Result.Columns.OfType<DataColumn>()
                    .ToDictionary(col => col.ColumnName, c => row[c].ToString()));


                return new JsonResult(new
                {
                    response = true,
                    responseCode = "00",
                    data = new
                    {
                        collectionTypes = collectionList,
                        paymentTypes = payTypeList,
                        deliveryTypes = deliveryList,
                        currencyLists = currencyList
                    }
                });
            }
            catch (Exception ex)
            {
                // Log error asynchronously
                _ = Task.Run(() => CompanyInfo.InsertErrorLogTracker("MergedApiAsync Error "+ex.ToString(), 0, 0, 0, 0, "MergedApiAsync", 0, 0, "", HttpContext));

                return new JsonResult(new { response = false, responseCode = "01", data = "Invalid request." });
            }
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        static string ConvertAsciiToSpecialChars(string inputStr)
        {
            string tempStr = inputStr;
            while (true)
            {
                int startIdx = tempStr.IndexOf('`');
                int endIdx = tempStr.IndexOf('~');

                if (startIdx == -1 || endIdx == -1 || startIdx + 1 >= endIdx)
                    break;

                string asciiStr = tempStr.Substring(startIdx + 1, endIdx - startIdx - 1);

                if (string.IsNullOrEmpty(asciiStr))
                    break;

                if (!int.TryParse(asciiStr, out int asciiValue))
                    return "dont"; // Invalid ASCII number

                char specialChar = (char)asciiValue;
                tempStr = ReplaceBetween(tempStr, startIdx, endIdx + 1, specialChar.ToString());
            }

            return tempStr;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        static string ReplaceBetween(string original, int start, int end, string replacement)
        {
            return original.Substring(0, start) + replacement + original.Substring(end);
        }


        [HttpGet]
        [Route("collection_delivery_paytypes_get")]
        public async Task<JsonResult> MergedApi_get([FromBody] JsonElement objdata)
        {
            var returnJsonData = (dynamic)null;
            Model.response.WebResponse response = null;
            string json = System.Text.Json.JsonSerializer.Serialize(objdata);
            dynamic data = JObject.Parse(json);

            // Log request asynchronously
            _ = Task.Run(() => CompanyInfo.InsertrequestLogTracker("collection_delivery_paytypes_get API full request body: " + JObject.Parse(json), 0, 0, 0, 0, "collection_delivery_paytypes1", 0, 0, "", HttpContext));

            try
            {
                if (!SqlInjectionProtector.ReadJsonElementValues(objdata) || !SqlInjectionProtector.ReadJsonElementValuesScript(objdata))
                {
                    return new JsonResult(new { response = false, responseCode = "02", data = "Invalid input detected." }) { StatusCode = 400 };
                }

                Transaction obj = JsonConvert.DeserializeObject<Transaction>(json);

                string webstring = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("ConnectionStrings")["WEB_DB_CONN"];
                webstring = webstring.ToLower();
                if (webstring.IndexOf("kobo", 0, StringComparison.OrdinalIgnoreCase) != -1)
                {
                    obj.Basecurr_Country_ID = 0; // Remove This after kobo. This is for just Kobo
                }

                // Fetch Collection Types Asynchronously 
                Task<DataTable> collectionTask = Task.Run(() =>
                {
                    Service.srvPaymentDepositType srv = new Service.srvPaymentDepositType();
                    return srv.GetPayDepositTypes(obj);
                });

                // Fetch Payment Types Asynchronously 
                Task<DataTable> paymentTask = Task.Run(() =>
                {
                    MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("GetPaymentTypes");
                    _cmd.CommandType = CommandType.StoredProcedure;
                    _cmd.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                    //int SourceID = obj.Is_App == 0 ? 3 : obj.Branch_ID == 2 ? 2 : 1;
                    //_cmd.Parameters.AddWithValue("_Source_ID", SourceID);
                    //string Whereclause = obj.Basecurr_Country_ID != 0 ? $" And Base_Country_Id ={obj.Basecurr_Country_ID}" : "";
                    int SourceID = 1;
                    string Whereclause = "";
                    if (obj.Is_App == 0)
                    { SourceID = 3; }
                    else if (obj.Branch_ID == 2) { SourceID = 2; }
                    _cmd.Parameters.AddWithValue("_Source_ID", SourceID);
                    if (obj.Basecurr_Country_ID != 0)
                    { Whereclause = Whereclause + " And Base_Country_Id =" + obj.Basecurr_Country_ID; }
                    _cmd.Parameters.AddWithValue("_whereclause", Whereclause);
                    return db_connection.ExecuteQueryDataTableProcedure(_cmd);
                });

                // Fetch Delivery Types Asynchronously 
                Task<DataTable> deliveryTask = Task.Run(() =>
                {
                    MySqlConnector.MySqlCommand _cmdDelivery = new MySqlConnector.MySqlCommand("GetDeliveryTypes");
                    _cmdDelivery.CommandType = CommandType.StoredProcedure;
                    _cmdDelivery.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                    _cmdDelivery.Parameters.AddWithValue("_Country_ID", obj.Country_ID);
                    return db_connection.ExecuteQueryDataTableProcedure(_cmdDelivery);
                });


                //Fetch Currency List
                Task<DataTable> currencyTask = Task.Run(() =>
                {
                    MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("getMultiCurrency");
                    string whereclause = "";
                    if (obj.Country_ID > 0)
                    {
                        whereclause = " and c.Country_ID=" + obj.Country_ID + "";
                    }
                    _cmd.CommandType = CommandType.StoredProcedure;
                    _cmd.Parameters.AddWithValue("_whereclause", whereclause);
                    _cmd.Parameters.AddWithValue("_Client_ID", obj.Client_ID);
                    return db_connection.ExecuteQueryDataTableProcedure(_cmd);
                });



                // Wait for all tasks to complete
                await Task.WhenAll(collectionTask, paymentTask, deliveryTask, currencyTask);

                // Convert DataTables to List
                var collectionList = collectionTask.Result?.Rows.OfType<DataRow>()
                    .Select(row => collectionTask.Result.Columns.OfType<DataColumn>()
                    .ToDictionary(col => col.ColumnName, c => row[c].ToString()));

                var payTypeList = paymentTask.Result?.Rows.OfType<DataRow>()
                    .Select(row => paymentTask.Result.Columns.OfType<DataColumn>()
                    .ToDictionary(col => col.ColumnName, c => row[c].ToString()));

                var deliveryList = deliveryTask.Result?.Rows.OfType<DataRow>()
                    .Select(row => deliveryTask.Result.Columns.OfType<DataColumn>()
                    .ToDictionary(col => col.ColumnName, c => row[c].ToString()));

                var currencyList = currencyTask.Result?.Rows.OfType<DataRow>()
                    .Select(row => currencyTask.Result.Columns.OfType<DataColumn>()
                    .ToDictionary(col => col.ColumnName, c => row[c].ToString()));


                return new JsonResult(new
                {
                    response = true,
                    responseCode = "00",
                    data = new
                    {
                        collectionTypes = collectionList,
                        paymentTypes = payTypeList,
                        deliveryTypes = deliveryList,
                        currencyLists = currencyList
                    }
                });
            }
            catch (Exception ex)
            {
                // Log error asynchronously
                _ = Task.Run(() => CompanyInfo.InsertErrorLogTracker("MergedApiAsync Error " + ex.ToString(), 0, 0, 0, 0, "MergedApiAsync", 0, 0, "", HttpContext));

                return new JsonResult(new { response = false, responseCode = "01", data = "Invalid request." });
            }
        }

        //End Parth added for merging 3 api's getdeliverytype, getpaytype, collectiontypelist
        #endregion for merging 3 api's getdeliverytype, getpaytype, collectiontypelist

        [HttpPost]
        [Authorize]
        [Route("select_bankcodes")]
        public IActionResult select_bankcodes([FromBody] JsonElement objd)
        {
            var returnJsonData = (dynamic)null;
            HttpContext context = HttpContext;
            string json = System.Text.Json.JsonSerializer.Serialize(objd);
            var validateJsonData = (dynamic)null;
            dynamic data = JObject.Parse(json);
            var claimsIdentity = User.Identity as ClaimsIdentity;//sanket
            var authHeader = "";
            try { authHeader = HttpContext.Request.Headers["Authorization"].ToString(); } catch (Exception egx) { authHeader = ""; }

            bool auth = AuthController.checkAuth(claimsIdentity, objd, Convert.ToString(authHeader));
            if (!auth)
            {
                var errorResponse = new { response = false, responseCode = "403", data = "Access Forbidden" };
                return new JsonResult(errorResponse) { StatusCode = (int)HttpStatusCode.Forbidden };
            }
            Beneficiary obj1 = new Beneficiary();
            Model.response.WebResponse response = null;
            DataTable dt = new DataTable();

            try
            {
                Bank obj = new Bank();
                obj.Id = Convert.ToInt32(data.bankID);
                List<Model.Bank> _lst = new List<Model.Bank>();

                if (!SqlInjectionProtector.ReadJsonElementValues(objd) || !SqlInjectionProtector.ReadJsonElementValuesScript(objd))
                {
                    var errorResponse = new { response = false, responseCode = "02", data = "Invalid input detected." };
                    return new JsonResult(errorResponse) { StatusCode = 400 };
                }

                Service.srvBank srv = new Service.srvBank();
                _lst = srv.GetBankCodes_ByBank(obj);
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
                   .ToDictionary(col => col.ColumnName, c => row[c]));

                    returnJsonData = new { response = true, responseCode = "00", data = relationshipData };
                    return new JsonResult(returnJsonData);
                }
                else
                {
                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_TECHNICAL_ERROR);
                    response.ResponseMessage = "No Records Found.";
                    response.ResponseCode = 0;
                    returnJsonData = new { response = false, responseCode = "02", data = response.ResponseMessage };
                    return returnJsonData;
                }

            }
            catch (Exception ex)
            {
                Model.ErrorLog objError = new Model.ErrorLog();
                objError.User = new Model.User();
                objError.Branch = new Model.Branch();
                objError.Client = new Model.Client();

                objError.Error = "Api : Select_Bank --" + ex.ToString();
                objError.Branch.Id = 1;
                objError.Date = DateTime.Now;
                objError.User.Id = 1;
                objError.Id = 1;
                objError.Function_Name = "Select_BankCodes";
                Service.srvErrorLog srvError = new Service.srvErrorLog();
                srvError.Create(objError, context);
                returnJsonData = new { response = false, responseCode = "01", data = "Invalid Request" };
                return returnJsonData;
            }
        }


            [HttpPost]
        [Authorize]
        [Route("select_bankbranches")]
        public IActionResult select_bankbranches([FromBody] JsonElement objd)
        {
            var returnJsonData = (dynamic)null;
            HttpContext context = HttpContext;
            string json = System.Text.Json.JsonSerializer.Serialize(objd);
            var validateJsonData = (dynamic)null;
            dynamic data = JObject.Parse(json);
            Beneficiary obj1 = new Beneficiary();
            Model.response.WebResponse response = null;
            DataTable dt = new DataTable();
            try
            {
                Bank obj = new Bank();
                obj.Id =  Convert.ToInt32( data.bankID);

                if (!SqlInjectionProtector.ReadJsonElementValues(objd) || !SqlInjectionProtector.ReadJsonElementValuesScript(objd))
                {
                    var errorResponse = new { response = false, responseCode = "02", data = "Invalid input detected." };
                    return new JsonResult(errorResponse) { StatusCode = 400 };
                }

                List<Model.Bank> _lst = new List<Model.Bank>();
                Service.srvBank srv = new Service.srvBank();
                _lst = srv.Select_BankBranch_Master(obj);
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
                        column.ColumnName = column.ColumnName.Replace("Id", "branchId");
                        column.ColumnName = column.ColumnName.Replace("Name", "branchName");
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
                    return returnJsonData;
                }
            }
            catch (Exception ex)
            {
                Model.ErrorLog objError = new Model.ErrorLog();
                objError.User = new Model.User();
                objError.Branch = new Model.Branch();
                objError.Client = new Model.Client();

                objError.Error = "Api : Select_Bank --" + ex.ToString();
                objError.Branch.Id = 1;
                objError.Date = DateTime.Now;
                objError.User.Id = 1;
                objError.Id = 1;
                objError.Function_Name = "Select_Banks";
                Service.srvErrorLog srvError = new Service.srvErrorLog();
                srvError.Create(objError, context);
                returnJsonData = new { response = false, responseCode = "01", data = "Invalid Response" };
                return returnJsonData;
            }

        }


        [HttpPost]
        [Authorize]
        [Route("getbeneficiarydetailstest")]
        public IActionResult BeneficiaryInfotest([FromBody] JsonElement obj)
        {
            HttpContext context = HttpContext;
            string json = System.Text.Json.JsonSerializer.Serialize(obj);
            var validateJsonData = (dynamic)null;
            dynamic data = JObject.Parse(json);
            var claimsIdentity = User.Identity as ClaimsIdentity;//sanket
            var authHeader = "";
            try { authHeader = HttpContext.Request.Headers["Authorization"].ToString(); } catch (Exception egx) { authHeader = ""; }

            bool auth = AuthController.checkAuth(claimsIdentity, obj, Convert.ToString(authHeader));
            if (!auth)
            {
                var errorResponse = new { response = false, responseCode = "403", data = "Access Forbidden" };
                return new JsonResult(errorResponse) { StatusCode = (int)HttpStatusCode.Forbidden };
            }
            CompanyInfo.InsertrequestLogTracker("getbeneficiarydetailstest full request body: " + JObject.Parse(json), 0, 0, 0, 0, "BeneficiaryInfotest", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);

            Beneficiary obj1 = new Beneficiary();
            Model.response.WebResponse response = null;
            DataTable dt = new DataTable();
            try
            {
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
                if (data.deleteStatus == "" || data.deleteStatus == null)
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "Invalid Delete Status." };
                    return new JsonResult(validateJsonData);
                }


                obj1.Client_ID = data.clientID;
                obj1.Customer_ID = data.customerID;
                obj1.Branch_ID = data.branchID;
                obj1.Delete_Status = data.deleteStatus;
                obj1.Beneficiary_Name = data.beneficiaryName;
                obj1.Beneficiary_Country_ID = Convert.ToInt32(data.beneficiaryCountryID);
                obj1.Sending_Flag = data.sendingFlag;

                if (Convert.ToString(data.beneficiaryID).Trim() == "")
                    data.beneficiaryID = 0;


                obj1.Beneficiary_ID = Convert.ToInt32(data.beneficiaryID);

                if (!SqlInjectionProtector.ReadJsonElementValues(obj) || !SqlInjectionProtector.ReadJsonElementValuesScript(obj))
                {
                    var errorResponse = new { response = false, responseCode = "02", data = "Invalid input detected." };
                    return new JsonResult(errorResponse) { StatusCode = 400 };
                }

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
                int Customer_ID = Convert.ToInt32(CompanyInfo.Decrypt(Convert.ToString(obj1.Customer_ID), true));




                List<Model.Beneficiary> _lst = new List<Model.Beneficiary>();
                DataTable dtperm = (DataTable)CompanyInfo.getEmailPermission(obj1.Client_ID, 47);
                MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("Beneficiary_Search");
                _cmd.CommandType = CommandType.StoredProcedure;
                string whereclause = " ";
                if (obj1.Beneficiary_ID > 0)
                {
                    whereclause = whereclause + " and bb.Beneficiary_ID=" + obj1.Beneficiary_ID + "";
                }
                if (obj1.Customer != null)
                {
                    if (obj1.Customer.WireTransfer_ReferanceNo != "" && obj1.Customer.WireTransfer_ReferanceNo != null)
                    {
                        whereclause = whereclause + " and WireTransfer_ReferanceNo  LIKE '%" + obj1.Customer.WireTransfer_ReferanceNo + "%'";
                    }
                    if (obj1.Customer.Full_Name != "" && obj1.Customer.Full_Name != null)
                    {
                        whereclause = whereclause + " and concat(first_Name,' ',Last_Name) LIKE '%" + obj1.Customer.Full_Name + "%'";
                    }
                }
                if (obj1.Beneficiary_Name != null && obj1.Beneficiary_Name != "")
                {
                    whereclause = whereclause + " and bb.Beneficiary_Name LIKE '%" + obj1.Beneficiary_Name + "%'";
                }
                if (obj1.Delete_Status != null && obj1.Delete_Status != -1)
                {
                    whereclause = whereclause + " and bb.delete_status=" + obj1.Delete_Status + "";
                }
                if (Customer_ID != null && Customer_ID > 0)
                {
                    whereclause = whereclause + " and bb.Customer_ID=" + Customer_ID + "";
                }
                if (obj1.Beneficiary_Country_ID != null && obj1.Beneficiary_Country_ID > 0)
                {
                    whereclause = whereclause + " and bb.beneficiary_country_id=" + obj1.Beneficiary_Country_ID + "";
                }
                if (obj1.Sending_Flag == 1 || obj1.Sending_Flag == 0)
                {
                    whereclause = whereclause + " and dd.Sending_Flag=" + obj1.Sending_Flag + "";
                }

                whereclause = whereclause + " and bb.Client_ID=" + obj1.Client_ID + "";

                _cmd.Parameters.AddWithValue("_whereclause", whereclause);

                dt = db_connection.ExecuteQueryDataTableProcedure(_cmd);
                dt.Columns.Add("EditPerm", typeof(int));
                if (dtperm.Rows.Count > 0 && dt.Rows.Count > 0)
                {
                    dt.Rows[0]["EditPerm"] = dtperm.Rows[0]["Status_ForCustomer"];
                }



                if (dt != null && dt.Rows.Count > 0)
                {
                    DataRow[] rowsToDelete = dt.Select("Sending_Flag = 1");
                    Array.ForEach(rowsToDelete, row => row.Delete());
                    dt.AcceptChanges();

                    dt.Columns["AccountHolderName"].ColumnName = "accountHolderName";                    
                     dt.Columns["Account_Number"].ColumnName = "accountNumber";
                     dt.Columns["BBank_ID"].ColumnName = "bbankID";
                     dt.Columns["BIC_Code"].ColumnName = "bicCode";
                     dt.Columns["BackID_Document"].ColumnName = "backIdDocument";
                     dt.Columns["BankCode"].ColumnName = "bankCode";
                     dt.Columns["Bank_Name"].ColumnName = "bankName";
                     dt.Columns["Beneficiary_Address"].ColumnName = "beneficiaryAddress";
                     dt.Columns["Beneficiary_City_ID"].ColumnName = "beneficiaryCityID";
                     dt.Columns["Beneficiary_Country_ID"].ColumnName = "beneficiaryCountryID";
                     dt.Columns["Beneficiary_ID"].ColumnName = "beneficiaryID";
                     dt.Columns["Beneficiary_ID1"].ColumnName = "beneficiaryID1";
                     dt.Columns["Beneficiary_Mobile"].ColumnName = "beneficiaryMobile";
                     dt.Columns["Beneficiary_Name"].ColumnName = "beneficiaryName";
                     dt.Columns["Beneficiary_Telephone"].ColumnName = "beneficiaryTelephone";
                     dt.Columns["Black_Listed"].ColumnName = "blackListed";
                     dt.Columns["Branch"].ColumnName = "branch";
                     dt.Columns["BranchCode"].ColumnName = "branchCode";
                     dt.Columns["City_Name"].ColumnName = "cityName";
                     dt.Columns["Collection_type_Id"].ColumnName = "collectionTypeID";
                     dt.Columns["Country_Name"].ColumnName = "countryName";
                     dt.Columns["Customer_ID"].ColumnName = "customerID";
                     dt.Columns["DateOf_Birth"].ColumnName = "dateOfBirth";
                     dt.Columns["Delete_Status"].ColumnName = "deleteStatus";
                     dt.Columns["Deposite_Type_Country_ID"].ColumnName = "depositeTypeCountryID";
                     dt.Columns["Deposite_Type_Status"].ColumnName = "depositeTypeStatus";
                     dt.Columns["EditPerm"].ColumnName = "editPerm";
                     dt.Columns["FileNameWithExt"].ColumnName = "fileNameWithExt";
                     dt.Columns["Full_name"].ColumnName = "fullName";
                     dt.Columns["Iban_ID"].ColumnName = "ibanID";
                     dt.Columns["Ifsc_Code"].ColumnName = "ifscCode";
                     dt.Columns["Issue_Date"].ColumnName = "issueDate";
                     dt.Columns["Mobile_provider"].ColumnName = "mobileProvider";
                     dt.Columns["Provider_name"].ColumnName = "providerName";
                     dt.Columns["Relation"].ColumnName = "relation";
                     dt.Columns["Relation_ID"].ColumnName = "relationID";
                     dt.Columns["SenderID_ExpiryDate"].ColumnName = "senderIDExpiryDate";
                     dt.Columns["SenderID_ID"].ColumnName = "senderidID";
                     dt.Columns["SenderID_Number"].ColumnName = "senderidNumber";
                     dt.Columns["SenderID_PlaceOfIssue"].ColumnName = "senderIdPlaceOfIssue";
                     dt.Columns["Sender_DateOfBirth"].ColumnName = "senderDateOfBirth";
                     dt.Columns["Sending_Flag"].ColumnName = "sendingFlag";
                     dt.Columns["ShowOnCustSide"].ColumnName = "showOnCustSide";
                     dt.Columns["TransCount"].ColumnName = "transCount";
                     dt.Columns["TransCount_scam"].ColumnName = "transCountScam";
                     dt.Columns["TransferTypeFlag"].ColumnName = "transferTypeFlag";
                     dt.Columns["Type_Name"].ColumnName = "typeName";
                     dt.Columns["Wallet_Id"].ColumnName = "walletID";
                     dt.Columns["Wallet_Id1"].ColumnName = "walletID_ID";
                     dt.Columns["Wallet_provider"].ColumnName = "walletProvider";
                     dt.Columns["WireTransfer_ReferanceNo"].ColumnName = "wireTransferReferanceNo";
                     dt.Columns["cashcollection_flag"].ColumnName = "cashcollectionFlag";
                     dt.Columns["collection_type_status"].ColumnName = "collectionTypeStatus";
                     dt.Columns["comments"].ColumnName = "comments";
                     dt.Columns["country_status"].ColumnName = "countryStatus";

                     var relationshipData = dt.Rows.OfType<DataRow>()
                .Select(row => dt.Columns.OfType<DataColumn>()
                    .ToDictionary(col => col.ColumnName, c => row[c].ToString()));
                     validateJsonData = new { response = true, responseCode = "00", data = relationshipData };
                     return new JsonResult(validateJsonData);

                    List<Dictionary<string, object>> beneficiary = new List<Dictionary<string, object>>();
                    foreach (DataRow dr in dt.Rows)
                    {
                        if (Convert.ToInt64(dr["Sending_Flag"]) == 1) { continue; }

                        Dictionary<string, object> beneficiary_details = new Dictionary<string, object>();
                        beneficiary_details["accountHolderName"] = (dr["AccountHolderName"] == DBNull.Value) ? "" : dr["AccountHolderName"];
                        beneficiary_details["accountNumber"] = dr["Account_Number"];
                        beneficiary_details["bbankID"] = dr["BBank_ID"];
                        beneficiary_details["bicCode"] = dr["BIC_Code"];
                        beneficiary_details["backIdDocument"] = dr["BackID_Document"];
                        beneficiary_details["bankCode"] = dr["BankCode"];
                        beneficiary_details["bankName"] = dr["Bank_Name"];
                        beneficiary_details["beneficiaryAddress"] = dr["Beneficiary_Address"];
                        beneficiary_details["beneficiaryCityID"] = dr["Beneficiary_City_ID"];
                        beneficiary_details["beneficiaryCountryID"] = dr["Beneficiary_Country_ID"];
                        beneficiary_details["beneficiaryID"] = dr["Beneficiary_ID"];
                        beneficiary_details["beneficiaryID1"] = dr["Beneficiary_ID1"];
                        beneficiary_details["beneficiaryMobile"] = dr["Beneficiary_Mobile"];
                        beneficiary_details["beneficiaryName"] = dr["Beneficiary_Name"];
                        beneficiary_details["beneficiaryTelephone"] = dr["Beneficiary_Telephone"];
                        beneficiary_details["blackListed"] = dr["Black_Listed"];
                        beneficiary_details["branch"] = dr["Branch"];
                        beneficiary_details["branchCode"] = dr["BranchCode"];
                        beneficiary_details["cityName"] = dr["City_Name"];
                        beneficiary_details["collectionTypeID"] = dr["Collection_type_Id"];
                        beneficiary_details["countryName"] = dr["Country_Name"];
                        beneficiary_details["customerID"] = dr["Customer_ID"];
                        beneficiary_details["dateOfBirth"] = dr["DateOf_Birth"];
                        beneficiary_details["deleteStatus"] = dr["Delete_Status"];
                        beneficiary_details["depositeTypeCountryID"] = dr["Deposite_Type_Country_ID"];
                        beneficiary_details["depositeTypeStatus"] = dr["Deposite_Type_Status"];
                        beneficiary_details["editPerm"] = dr["EditPerm"];
                        beneficiary_details["fileNameWithExt"] = dr["FileNameWithExt"];
                        beneficiary_details["fullName"] = dr["Full_name"];
                        beneficiary_details["ibanID"] = dr["Iban_ID"];
                        beneficiary_details["ifscCode"] = dr["Ifsc_Code"];
                        beneficiary_details["issueDate"] = dr["Issue_Date"];

                        beneficiary_details["mobileProvider"] = dr["Mobile_provider"];
                        beneficiary_details["providerName"] = dr["Provider_name"];
                        beneficiary_details["relation"] = dr["Relation"];
                        beneficiary_details["relationID"] = dr["Relation_ID"];
                        beneficiary_details["senderIDExpiryDate"] = dr["SenderID_ExpiryDate"];
                        beneficiary_details["senderidID"] = (dr["SenderID_ID"] == DBNull.Value) ? "" : dr["SenderID_ID"];
                        beneficiary_details["senderidNumber"] = dr["SenderID_Number"];
                        beneficiary_details["senderIdPlaceOfIssue"] = dr["SenderID_PlaceOfIssue"];
                        beneficiary_details["senderDateOfBirth"] = dr["Sender_DateOfBirth"];
                        beneficiary_details["sendingFlag"] = dr["Sending_Flag"];

                        beneficiary_details["showOnCustSide"] = dr["ShowOnCustSide"];
                        beneficiary_details["transCount"] = dr["TransCount"];
                        beneficiary_details["transCountScam"] = dr["TransCount_scam"];
                        beneficiary_details["transferTypeFlag"] = dr["TransferTypeFlag"];
                        beneficiary_details["typeName"] = dr["Type_Name"];
                        beneficiary_details["walletID"] = dr["Wallet_Id"];
                        beneficiary_details["walletID_ID"] = dr["Wallet_Id1"];
                        beneficiary_details["walletProvider"] = dr["Wallet_provider"];
                        beneficiary_details["wireTransferReferanceNo"] = dr["WireTransfer_ReferanceNo"];
                        beneficiary_details["cashcollectionFlag"] = dr["cashcollection_flag"];
                        beneficiary_details["collectionTypeStatus"] = dr["collection_type_status"];
                        beneficiary_details["comments"] = dr["comments"];
                        beneficiary_details["countryStatus"] = dr["country_status"];
                        beneficiary.Add(beneficiary_details);
                    }

                    validateJsonData = new { response = true, responseCode = "00", data = beneficiary };
                    return new JsonResult(validateJsonData);
                }
                else
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "No Records Found." };
                    return new JsonResult(validateJsonData);
                }
            }
            catch (Exception ex)
            {
                string Activity = "Api:- getbeneficiarydetailstest" + ex.ToString();
                validateJsonData = new { response = false, responseCode = "01", data = "Invalid Request" };
                CompanyInfo.InsertErrorLogTracker(Activity.ToString(), 0, 0, 0, 0, "getbeneficiarydetailstest", Convert.ToInt32(obj1.Branch_ID), Convert.ToInt32(obj1.Client_ID), "", context);
                return new JsonResult(validateJsonData);

            }
        }



        [HttpPost]
        [Authorize]
        [Route("getbeneficiarydetails")]

        public IActionResult BeneficiaryInfo([FromBody] JsonElement obj)
        {
            HttpContext context = HttpContext;
            string json = System.Text.Json.JsonSerializer.Serialize(obj);
            var validateJsonData = (dynamic)null;
            dynamic data = JObject.Parse(json);
            var claimsIdentity = User.Identity as ClaimsIdentity;//sanket
            var authHeader = "";
            try { authHeader = HttpContext.Request.Headers["Authorization"].ToString(); } catch (Exception egx) { authHeader = ""; }

            bool auth = AuthController.checkAuth(claimsIdentity, obj, Convert.ToString(authHeader));
            if (!auth)
            {
                var errorResponse = new { response = false, responseCode = "403", data = "Access Forbidden" };
                return new JsonResult(errorResponse) { StatusCode = (int)HttpStatusCode.Forbidden };
            }
            CompanyInfo.InsertrequestLogTracker("getbeneficiarydetails full request body: " + JObject.Parse(json), 0, 0, 0, 0, "BeneficiaryInfo", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);

            Beneficiary obj1 = new Beneficiary();
            Model.response.WebResponse response = null;
            DataTable dt = new DataTable();
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
                if (data.deleteStatus == "" || data.deleteStatus == null)
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "Invalid Delete Status." };
                    return new JsonResult(validateJsonData);
                }


                obj1.Client_ID = data.clientID;
                obj1.Customer_ID = data.customerID;
                obj1.Branch_ID = data.branchID;
                obj1.Delete_Status = data.deleteStatus;
                obj1.Beneficiary_Name = data.beneficiaryName;
                obj1.Beneficiary_Country_ID = Convert.ToInt32(data.beneficiaryCountryID);
                obj1.Sending_Flag = data.sendingFlag;
                try { obj1.Benf_BankDetails_ID = data.Benf_BankDetails_ID; } catch { obj1.Benf_BankDetails_ID = 0; }
               
                if (Convert.ToString(data.beneficiaryID).Trim() == "")
                    data.beneficiaryID = 0;


                obj1.Beneficiary_ID = Convert.ToInt32(data.beneficiaryID);

                int Customer_ID = Convert.ToInt32(CompanyInfo.Decrypt(Convert.ToString(obj1.Customer_ID), true));

                


                List<Model.Beneficiary> _lst = new List<Model.Beneficiary>();
                DataTable dtperm = (DataTable)CompanyInfo.getEmailPermission(obj1.Client_ID, 47);
                DataTable dtperm_hideBenficiary = (DataTable)CompanyInfo.getEmailPermission(obj1.Client_ID, 151);
                string sp_name = string.Empty;
                if(obj1.Benf_BankDetails_ID > 0)
                {
                    sp_name = "Beneficiary_Search_Details";
                }
                else
                {
                    sp_name = "Beneficiary_Search";
                }
                MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand(sp_name);
                _cmd.CommandType = CommandType.StoredProcedure;
                string whereclause = " ";
                if (obj1.Beneficiary_ID > 0)
                {
                    whereclause = whereclause + " and bb.Beneficiary_ID=" + obj1.Beneficiary_ID + "";
                }
                if (obj1.Customer != null)
                {
                    if (obj1.Customer.WireTransfer_ReferanceNo != "" && obj1.Customer.WireTransfer_ReferanceNo != null)
                    {
                        whereclause = whereclause + " and WireTransfer_ReferanceNo  LIKE '%" + obj1.Customer.WireTransfer_ReferanceNo + "%'";
                    }
                    if (obj1.Customer.Full_Name != "" && obj1.Customer.Full_Name != null)
                    {
                        whereclause = whereclause + " and concat(first_Name,' ',Last_Name) LIKE '%" + obj1.Customer.Full_Name + "%'";
                    }
                }
                if (obj1.Beneficiary_Name != null && obj1.Beneficiary_Name != "")
                {
                    whereclause = whereclause + " and bb.Beneficiary_Name LIKE '%" + obj1.Beneficiary_Name + "%'";
                }
                if (obj1.Delete_Status != null && obj1.Delete_Status != -1)
                {
                    try
                    {
                        if (dtperm_hideBenficiary.Rows.Count > 0 && Convert.ToInt32(dtperm_hideBenficiary.Rows[0]["Status_ForCustomer"]) == 0)
                        {
                            //Hide deleted Benficiaries
                            whereclause += " and bb.delete_status = 0";
                        }
                        else
                        {
                            // Show both 0 and 1, but prioritize 0
                            whereclause += " and (bb.delete_status = 0 OR bb.delete_status = 1)";
                        }
                    }
                    catch(Exception ex)
                    {
                        whereclause = whereclause + " and bb.delete_status=" + obj1.Delete_Status + "";
                    }
                }
                if (Customer_ID != null && Customer_ID > 0)
                {
                    whereclause = whereclause + " and bb.Customer_ID=" + Customer_ID + "";
                }
                if (obj1.Beneficiary_Country_ID != null && obj1.Beneficiary_Country_ID > 0)
                {
                    whereclause = whereclause + " and bb.beneficiary_country_id=" + obj1.Beneficiary_Country_ID + "";
                }
                if (obj1.Sending_Flag == 1 || obj1.Sending_Flag == 0)
                {
                    whereclause = whereclause + " and dd.Sending_Flag=" + obj1.Sending_Flag + "";
                }
                if (obj1.Benf_BankDetails_ID != 0)
                {
                    if (obj1.Benf_BankDetails_ID != 0) //vyankatesh 25-11-24
                    {
                        whereclause = whereclause + " and ee.BBDetails_ID='" + obj1.Benf_BankDetails_ID + "'";
                        _cmd.Parameters.AddWithValue("_conditionclause", "");
                    }
                    else
                    {
                        _cmd.Parameters.AddWithValue("_conditionclause", " and ee.BBDetails_ID=tmm.Benf_BankDetails_ID");
                    }
                }

                whereclause = whereclause + " and bb.Client_ID=" + obj1.Client_ID + "";

                _cmd.Parameters.AddWithValue("_whereclause", whereclause);
                

                dt = db_connection.ExecuteQueryDataTableProcedure(_cmd);


                

                try
                {
                    if (dt.Columns.Contains("DateOf_Birth"))
                    {
                        // Create a new string column
                        dt.Columns.Add("DateOf_Birth_Str", typeof(string));

                        foreach (DataRow row in dt.Rows)
                        {
                            if (row["DateOf_Birth"] != DBNull.Value && DateTime.TryParse(row["DateOf_Birth"].ToString(), out DateTime dob))
                            {
                                row["DateOf_Birth_Str"] = dob.ToString("dd-MM-yyyy");
                            }
                            else
                            {
                                row["DateOf_Birth_Str"] = "";
                            }
                        }

                        // Remove old DateOf_Birth column
                        dt.Columns.Remove("DateOf_Birth");

                        // Rename new column to expected name
                        dt.Columns["DateOf_Birth_Str"].ColumnName = "dateOfBirth";
                    }
                }
                catch (Exception ex)
                {
                    string Activity = "Api:- BeneficiaryInfo" + ex.ToString();
                    _ = Task.Run(() => CompanyInfo.InsertErrorLogTracker(Activity.ToString(), 0, 0, 0, 0, "BeneficiaryInfo", Convert.ToInt32(obj1.Branch_ID), Convert.ToInt32(obj1.Client_ID), "", context));
                }

                dt.Columns.Add("EditPerm", typeof(int));
                if (dtperm.Rows.Count > 0 && dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        row["EditPerm"] = dtperm.Rows[0]["Status_ForCustomer"];
                    }
                }

                if (dt != null && dt.Rows.Count > 0)
                {
                   
                    DataRow[] rowsToDelete = dt.Select("Sending_Flag = 1");
                    Array.ForEach(rowsToDelete, row => row.Delete());
                    dt.AcceptChanges();
                    
                    dt.Columns["AccountHolderName"].ColumnName = "accountHolderName";
                    dt.Columns["Account_Number"].ColumnName = "accountNumber";
                    dt.Columns["BBank_ID"].ColumnName = "bbankID";
                    dt.Columns["BIC_Code"].ColumnName = "bicCode";
                    dt.Columns["BackID_Document"].ColumnName = "backIdDocument";
                    dt.Columns["BankCode"].ColumnName = "bankCode";
                    try
                    {
                        dt.Columns["BenfBankCode"].ColumnName = "benfbankCode";
                    }
                    catch (Exception ex) { }
                    try { dt.Columns["BankBranch_ID"].ColumnName = "bankBranchID"; } catch (Exception ex) {}
                    dt.Columns["Bank_Name"].ColumnName = "bankName";
                    dt.Columns["Beneficiary_Address"].ColumnName = "beneficiaryAddress";
                    dt.Columns["Beneficiary_City_ID"].ColumnName = "beneficiaryCityID";
                    dt.Columns["Beneficiary_Country_ID"].ColumnName = "beneficiaryCountryID";
                    dt.Columns["Beneficiary_ID"].ColumnName = "beneficiaryID";
                    dt.Columns["Beneficiary_ID1"].ColumnName = "beneficiaryID1";
                    dt.Columns["Beneficiary_Mobile"].ColumnName = "beneficiaryMobile";
                    dt.Columns["Beneficiary_Name"].ColumnName = "beneficiaryName";
                    dt.Columns["Beneficiary_Telephone"].ColumnName = "beneficiaryTelephone";
                    dt.Columns["Black_Listed"].ColumnName = "blackListed";
                    dt.Columns["Branch"].ColumnName = "branch";
                    dt.Columns["BranchCode"].ColumnName = "branchCode";
                    dt.Columns["City_Name"].ColumnName = "cityName";
                    dt.Columns["Collection_type_Id"].ColumnName = "collectionTypeID";
                    dt.Columns["Country_Name"].ColumnName = "countryName";
                    dt.Columns["Customer_ID"].ColumnName = "customerID";
                    //dt.Columns["DateOf_Birth"].ColumnName = "dateOfBirth";
                    dt.Columns["Delete_Status"].ColumnName = "deleteStatus";
                    dt.Columns["Deposite_Type_Country_ID"].ColumnName = "depositeTypeCountryID";
                    dt.Columns["Deposite_Type_Status"].ColumnName = "depositeTypeStatus";
                    dt.Columns["EditPerm"].ColumnName = "editPerm";
                    dt.Columns["FileNameWithExt"].ColumnName = "fileNameWithExt";
                    dt.Columns["Full_name"].ColumnName = "fullName";
                    dt.Columns["Iban_ID"].ColumnName = "ibanID";
                    dt.Columns["Ifsc_Code"].ColumnName = "ifscCode";
                    dt.Columns["Issue_Date"].ColumnName = "issueDate";
                    dt.Columns["Mobile_provider"].ColumnName = "mobileProvider";
                    dt.Columns["Provider_name"].ColumnName = "providerName";
                    dt.Columns["Relation"].ColumnName = "relation";
                    dt.Columns["Relation_ID"].ColumnName = "relationID";
                    dt.Columns["SenderID_ExpiryDate"].ColumnName = "senderIDExpiryDate";
                    dt.Columns["SenderID_ID"].ColumnName = "senderidID";
                    dt.Columns["SenderID_Number"].ColumnName = "senderidNumber";
                    dt.Columns["SenderID_PlaceOfIssue"].ColumnName = "senderIdPlaceOfIssue";
                    dt.Columns["Sender_DateOfBirth"].ColumnName = "senderDateOfBirth";
                    dt.Columns["Sending_Flag"].ColumnName = "sendingFlag";
                    dt.Columns["ShowOnCustSide"].ColumnName = "showOnCustSide";
                    dt.Columns["TransCount"].ColumnName = "transCount";
                    dt.Columns["TransCount_scam"].ColumnName = "transCountScam";
                    dt.Columns["TransferTypeFlag"].ColumnName = "transferTypeFlag";
                    dt.Columns["Type_Name"].ColumnName = "typeName";
                    dt.Columns["Wallet_Id"].ColumnName = "walletID";
                    dt.Columns["Wallet_Id1"].ColumnName = "walletID_ID";
                    dt.Columns["Wallet_provider"].ColumnName = "walletProvider";
                    dt.Columns["WireTransfer_ReferanceNo"].ColumnName = "wireTransferReferanceNo";
                    dt.Columns["cashcollection_flag"].ColumnName = "cashcollectionFlag";
                    dt.Columns["collection_type_status"].ColumnName = "collectionTypeStatus";
                    dt.Columns["comments"].ColumnName = "comments";
                    dt.Columns["country_status"].ColumnName = "countryStatus";
                   

                    var relationshipData = dt.Rows.OfType<DataRow>()
               .Select(row => dt.Columns.OfType<DataColumn>()
                   .ToDictionary(col => col.ColumnName, c => row[c].ToString()));
                    validateJsonData = new { response = true, responseCode = "00", data = relationshipData };
                    return new JsonResult(validateJsonData);


                    List<Dictionary<string, object>> beneficiary = new List<Dictionary<string, object>>();
                    foreach (DataRow dr in dt.Rows)
                    {
                        if (Convert.ToInt64(dr["Sending_Flag"]) == 1) { continue; }

                        Dictionary<string, object> beneficiary_details = new Dictionary<string, object>();
                        beneficiary_details["accountHolderName"] = (dr["AccountHolderName"] == DBNull.Value) ? "" : dr["AccountHolderName"];
                        beneficiary_details["accountNumber"] = dr["Account_Number"];
                        beneficiary_details["bbankID"] = dr["BBank_ID"];
                        beneficiary_details["bicCode"] = dr["BIC_Code"];
                        beneficiary_details["backIdDocument"] = dr["BackID_Document"];
                        beneficiary_details["bankCode"] = dr["BankCode"];
                        beneficiary_details["bankName"] = dr["Bank_Name"];
                        beneficiary_details["beneficiaryAddress"] = dr["Beneficiary_Address"];
                        beneficiary_details["beneficiaryCityID"] = dr["Beneficiary_City_ID"];
                        beneficiary_details["beneficiaryCountryID"] = dr["Beneficiary_Country_ID"];
                        beneficiary_details["beneficiaryID"] = dr["Beneficiary_ID"];
                        beneficiary_details["beneficiaryID1"] = dr["Beneficiary_ID1"];
                        beneficiary_details["beneficiaryMobile"] = dr["Beneficiary_Mobile"];
                        beneficiary_details["beneficiaryName"] = dr["Beneficiary_Name"];
                        beneficiary_details["beneficiaryTelephone"] = dr["Beneficiary_Telephone"];
                        beneficiary_details["blackListed"] = dr["Black_Listed"];
                        beneficiary_details["branch"] = dr["Branch"];
                        beneficiary_details["branchCode"] = dr["BranchCode"];
                        beneficiary_details["cityName"] = dr["City_Name"];
                        beneficiary_details["collectionTypeID"] = dr["Collection_type_Id"];
                        beneficiary_details["countryName"] = dr["Country_Name"];
                        beneficiary_details["customerID"] = dr["Customer_ID"];
                        beneficiary_details["dateOfBirth"] = dr["DateOf_Birth"];
                        beneficiary_details["deleteStatus"] = dr["Delete_Status"];
                        beneficiary_details["depositeTypeCountryID"] = dr["Deposite_Type_Country_ID"];
                        beneficiary_details["depositeTypeStatus"] = dr["Deposite_Type_Status"];
                        beneficiary_details["editPerm"] = dr["EditPerm"];
                        beneficiary_details["fileNameWithExt"] = dr["FileNameWithExt"];
                        beneficiary_details["fullName"] = dr["Full_name"];
                        beneficiary_details["ibanID"] = dr["Iban_ID"];
                        beneficiary_details["ifscCode"] = dr["Ifsc_Code"];
                        beneficiary_details["issueDate"] = dr["Issue_Date"];

                        beneficiary_details["mobileProvider"] = dr["Mobile_provider"];
                        beneficiary_details["providerName"] = dr["Provider_name"];
                        beneficiary_details["relation"] = dr["Relation"];
                        beneficiary_details["relationID"] = dr["Relation_ID"];
                        beneficiary_details["senderIDExpiryDate"] = dr["SenderID_ExpiryDate"];
                        beneficiary_details["senderidID"] = ( dr["SenderID_ID"] == DBNull.Value) ? "" : dr["SenderID_ID"];
                        beneficiary_details["senderidNumber"] = dr["SenderID_Number"];
                        beneficiary_details["senderIdPlaceOfIssue"] = dr["SenderID_PlaceOfIssue"];
                        beneficiary_details["senderDateOfBirth"] = dr["Sender_DateOfBirth"];
                        beneficiary_details["sendingFlag"] = dr["Sending_Flag"];

                        beneficiary_details["showOnCustSide"] = dr["ShowOnCustSide"];
                        beneficiary_details["transCount"] = dr["TransCount"];
                        beneficiary_details["transCountScam"] = dr["TransCount_scam"];
                        beneficiary_details["transferTypeFlag"] = dr["TransferTypeFlag"];
                        beneficiary_details["typeName"] = dr["Type_Name"];
                        beneficiary_details["walletID"] = dr["Wallet_Id"];
                        beneficiary_details["walletID_ID"] = dr["Wallet_Id1"];
                        beneficiary_details["walletProvider"] = dr["Wallet_provider"];
                        beneficiary_details["wireTransferReferanceNo"] = dr["WireTransfer_ReferanceNo"];
                        beneficiary_details["cashcollectionFlag"] = dr["cashcollection_flag"];
                        beneficiary_details["collectionTypeStatus"] = dr["collection_type_status"];
                        beneficiary_details["comments"] = dr["comments"];
                        beneficiary_details["countryStatus"] = dr["country_status"];
                        beneficiary.Add(beneficiary_details);
                    }

                    validateJsonData = new { response = true, responseCode = "00", data = beneficiary };
                    return new JsonResult(validateJsonData);
                }
                else
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "No Records Found." };
                    return new JsonResult(validateJsonData);
                }
            }
            catch (Exception ex)
            {
                string Activity = "Api:- BeneficiaryInfo" + ex.ToString();
                validateJsonData = new { response = false, responseCode = "01", data = "Invalid Request" };
                CompanyInfo.InsertErrorLogTracker(Activity.ToString(), 0, 0, 0, 0, "BeneficiaryInfo", Convert.ToInt32(obj1.Branch_ID), Convert.ToInt32(obj1.Client_ID), "", context);
                return new JsonResult(validateJsonData);

            }
        }

        

        [HttpPost]
        [Authorize]
        [Route("deletebeneficiary")]

        public JsonResult Delete_Beneficiary([FromBody] JsonElement obj1)
        {

            string result = string.Empty;
            HttpContext context = HttpContext;
            Model.response.WebResponse response = null;
            string json = System.Text.Json.JsonSerializer.Serialize(obj1);
            dynamic data = JObject.Parse(json);
            var claimsIdentity = User.Identity as ClaimsIdentity;//sanket
            var authHeader = "";
            try { authHeader = HttpContext.Request.Headers["Authorization"].ToString(); } catch (Exception egx) { authHeader = ""; }

            bool auth = AuthController.checkAuth(claimsIdentity, obj1, Convert.ToString(authHeader));
            if (!auth)
            {
                var errorResponse = new { response = false, responseCode = "403", data = "Access Forbidden" };
                return new JsonResult(errorResponse) { StatusCode = (int)HttpStatusCode.Forbidden };
            }
            CompanyInfo.InsertrequestLogTracker("deletebeneficiary full request body: " + JObject.Parse(json), 0, 0, 0, 0, "Delete_Beneficiary", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
            var validateJsonData = (dynamic)null;
            Beneficiary obj = new Beneficiary();
            try
            {
                if (!SqlInjectionProtector.ReadJsonElementValues(obj1) || !SqlInjectionProtector.ReadJsonElementValuesScript(obj1))
                {
                    var errorResponse = new { response = false, responseCode = "02", data = "Invalid input detected." };
                    return new JsonResult(errorResponse) { StatusCode = 400 };
                }

                obj.Id = data.beneficiaryID;
                obj.status = data.status;
                obj.whereclause = data.whereclause;
                obj.Customer_ID = data.customerID;
                obj.Branch_ID = data.branchID;
                obj.Client_ID = data.clientID;
                MySqlConnector.MySqlCommand cmd = new MySqlConnector.MySqlCommand("sp_update_status");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("_Id", obj.Id);
                cmd.Parameters.AddWithValue("_status", obj.status);
                cmd.Parameters.AddWithValue("_Flag", obj.whereclause);//flag name
                cmd.Parameters.Add(new MySqlConnector.MySqlParameter("_Name", MySqlConnector.MySqlDbType.String));
                cmd.Parameters["_Name"].Direction = ParameterDirection.Output;

                int n = db_connection.ExecuteNonQueryProcedure(cmd);
                if (n > 0)//success
                {
                    if (obj.status == 1)
                    {
                        string name = Convert.ToString(cmd.Parameters["_Name"].Value);
                        int Customer_ID = Convert.ToInt32(CompanyInfo.Decrypt(Convert.ToString(obj.Customer_ID), true));
                        CompanyInfo.InsertActivityLogDetails("Beneficiary Id " + obj.Id + " deleted this beneficiary.", 0, 0, 0, Convert.ToInt32(Customer_ID), "UpdateStatus", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "Delete_Beneficiary", context);
                    }
                    validateJsonData = new { response = true, responseCode = "00", data = "Beneficiary Record Deleted Successfully." };
                    return new JsonResult(validateJsonData);
                }
                else//failed
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "Record Deletion Failed." };
                    return new JsonResult(validateJsonData);
                }

            }
            catch (Exception _x)
            {
                string Activity = "Api:- deletebeneficiary" + _x.ToString();
                validateJsonData = new { response = false, responseCode = "01", data = "Invalid Request" };
                CompanyInfo.InsertErrorLogTracker(Activity.ToString(), 0, 0, 0, 0, "deletebeneficiary", Convert.ToInt32(0), Convert.ToInt32(0), "", context);
                return new JsonResult(validateJsonData);
            }            
        }



        [HttpPost]
        [Authorize]
        [Route("getidinfo")]

        public IActionResult getIDInfo([FromBody] JsonElement obj1)
        {

            HttpContext context = HttpContext;
            string json = System.Text.Json.JsonSerializer.Serialize(obj1);
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
            CompanyInfo.InsertrequestLogTracker("getidinfo full request body: " + JObject.Parse(json), 0, 0, 0, 0, "getIDInfo", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
            Model.response.WebResponse response = null;
            Document obj = new Document();

            if (!SqlInjectionProtector.ReadJsonElementValues(obj1) || !SqlInjectionProtector.ReadJsonElementValuesScript(obj1))
            {
                var errorResponse = new { response = false, responseCode = "02", data = "Invalid input detected." };
                return new JsonResult(errorResponse) { StatusCode = 400 };
            }

            DataTable dt = new DataTable();


            obj.Client_ID = data.clientID;
            obj.Branch_ID = data.branchID;
            obj.Comments = data.commentFlag;
            obj.Beneficiary_ID = data.beneficiaryID;
            obj.Customer_ID = data.customerID;

            var validateJsonData = (dynamic)null;


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
            if (data.beneficiaryID == "" || data.beneficiaryID == null)
            {
                validateJsonData = new { response = false, responseCode = "02", data = "Invalid Beneficiary ID." };
                return new JsonResult(validateJsonData);
            }
            if (data.customerID == "" || data.customerID == null)
            {
                validateJsonData = new { response = false, responseCode = "02", data = "Invalid Customer ID." };
                return new JsonResult(validateJsonData);
            }
            try
            {
                Service.srvDocument srv = new Service.srvDocument();
                DataTable li1 = srv.Select_Document_List(obj, context);

                List<Dictionary<string, object>> IdDocument = new List<Dictionary<string, object>>();
                if (li1 != null && li1.Rows.Count > 0)
                {
                    foreach (DataRow dr in li1.Rows)
                    {
                        Dictionary<string, object> Id_Document = new Dictionary<string, object>();
                        Id_Document["backIDocument"] = dr["BackID_Document"];
                        Id_Document["expired"] = dr["Expired"];
                        Id_Document["fileNameWithExt"] = dr["FileNameWithExt"];
                        Id_Document["idNameID"] = dr["IDName_ID"];
                        Id_Document["idTypeID"] = dr["IDType_ID"];
                        Id_Document["idName"] = dr["ID_Name"];
                        Id_Document["idType"] = dr["ID_Type"];
                        Id_Document["issueDate"] = dr["Issue_Date"];
                        Id_Document["recordInsertDateTime"] = dr["Record_Insert_DateTime"];
                        Id_Document["senderIDExpiryDate"] = dr["SenderID_ExpiryDate"];
                        Id_Document["senderidID"] = dr["SenderID_ID"];
                        Id_Document["senderidNumber"] = dr["SenderID_Number"];
                        Id_Document["senderidPlaceOfIssue"] = dr["SenderID_PlaceOfIssue"];
                        Id_Document["senderNameOnID"] = dr["SenderNameOnID"];
                        Id_Document["senderDateOfBirth"] = dr["Sender_DateOfBirth"];
                        Id_Document["verifiedBy"] = dr["Verified_By"];
                        Id_Document["verifiedDate"] = dr["Verified_Date"];
                        Id_Document["comments"] = dr["comments"];
                        IdDocument.Add(Id_Document);
                    }
                    validateJsonData = new { response = true, responseCode = "00", data = IdDocument };
                    return new JsonResult(validateJsonData);
                }
                else
                {
                    validateJsonData = new { response = false, responseCode = "02", data = IdDocument };
                    return new JsonResult(validateJsonData);
                }
            }
            catch (Exception ex)
            {
                string Activity = "Api getIDInfo: " + ex.ToString() + " ";
                validateJsonData = new { response = false, responseCode = "01", data = "Invalid Request" };
                CompanyInfo.InsertErrorLogTracker(Activity.ToString(), 0, 0, 0, 0, "getIDInfo", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "", context);
                return new JsonResult(validateJsonData);
            }
        }

        [HttpPost]
        [Route("getbenfconfig")]
        public IActionResult GetBenfConfig([FromBody] JsonElement obj1)
        {
            var validateJsonData = (dynamic)null;
            HttpContext context = HttpContext;
            string json = System.Text.Json.JsonSerializer.Serialize(obj1);
            dynamic data = JObject.Parse(json);
            Model.response.WebResponse response = null;
            CompanyInfo.InsertrequestLogTracker("getbenfconfig full request body: " + JObject.Parse(json), 0, 0, 0, 0, "GetBenfConfig", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
            
            try
            {
                if (!SqlInjectionProtector.ReadJsonElementValues(obj1) || !SqlInjectionProtector.ReadJsonElementValuesScript(obj1))
                {
                    var errorResponse = new { response = false, responseCode = "02", data = "Invalid input detected." };
                    return new JsonResult(errorResponse) { StatusCode = 400 };
                }

                Beneficiary obj = JsonConvert.DeserializeObject<Beneficiary>(json);
                Service.srvBeneficiary srv = new Service.srvBeneficiary(context);
                DataTable[] dt = new DataTable[1];
                dt[0] = srv.GetConfig(obj);
                dt[0].TableName = "Beneficary_Config";
                if (dt[0] != null)
                {
                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_SUCCESS);
                    response.data = "success";
                    response.ObjData = dt;
                    response.ResponseCode = 0;
                  
                    var relationshipData = dt[0].Rows.OfType<DataRow>()
               .Select(row => dt[0].Columns.OfType<DataColumn>()
                   .ToDictionary(col => col.ColumnName, c => row[c].ToString()));

                    validateJsonData = new { response = true, responseCode = "00", data = relationshipData };
                    return new JsonResult(validateJsonData);
                }
                else
                {
                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_TECHNICAL_ERROR);
                    response.ResponseMessage = "No Records Found.";
                    response.ResponseCode = 0;
                    validateJsonData = new { response = false, responseCode = "01", data = response.ResponseMessage };
                    return new JsonResult(validateJsonData);
                }

            }
            catch (Exception ex)
            {
                string Activity = "Api getbenfconfig: " + ex.ToString() + " ";
                validateJsonData = new { response = false, responseCode = "01", data = "Invalid Request" };
                CompanyInfo.InsertErrorLogTracker(Activity.ToString(), 0, 0, 0, 0, "GetBenfConfig", Convert.ToInt32(0), Convert.ToInt32(0), "", context);
                return new JsonResult(validateJsonData);
            }
        }

        [HttpPost]
        [Authorize]
        [Route("editbenficiary")]
        public IActionResult updatebeneficiary([FromBody] JsonElement obj1)
        {
            HttpContext context = HttpContext;
            string json = System.Text.Json.JsonSerializer.Serialize(obj1);
            dynamic data = JObject.Parse(json); string Status = string.Empty;
            Model.response.WebResponse response = null;
            var claimsIdentity = User.Identity as ClaimsIdentity;//sanket
            var authHeader = "";
            try { authHeader = HttpContext.Request.Headers["Authorization"].ToString(); } catch (Exception egx) { authHeader = ""; }

            bool auth = AuthController.checkAuth(claimsIdentity, obj1, Convert.ToString(authHeader));
            if (!auth)
            {
                var errorResponse = new { response = false, responseCode = "403", data = "Access Forbidden" };
                return new JsonResult(errorResponse) { StatusCode = (int)HttpStatusCode.Forbidden };
            }
            CompanyInfo.InsertrequestLogTracker("editbenficiary full request body: " + JObject.Parse(json), 0, 0, 0, 0, "updatebeneficiary", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
            Beneficiary obj = new Beneficiary();

            //Start Parth added for proper fields validation
            #region proper fields validation
            if (!SqlInjectionProtector.ReadJsonElementValues_SingleElement(obj1).isValid)
            {
                string fieldName = string.Empty;

                var sqlValidationResult = SqlInjectionProtector.ReadJsonElementValues_SingleElement(obj1);   // Checked SQL injection validation
                if (!sqlValidationResult.isValid)
                {
                    fieldName = sqlValidationResult.fieldName;  // Captured the field name from the validation
                }

                string errorMessage = fieldName switch
                {
                    "accountHolderName" => "Invalid input detected for Account Holder Name",
                    "accountNumber" => "Invalid input detected for Account Number",
                    "bankCode" => "Invalid input detected for Bank Code",
                    "bankName" => "Invalid input detected for Bank Name",
                    "beneficiaryAddress" => "Invalid input detected for Address",
                    "beneficiaryAddress1" => "Invalid input detected for Address",
                    "beneficiaryCityName" => "Invalid input detected for City Name",
                    "beneficiaryCountryName" => "Invalid input detected for Country Name",
                    "beneficiaryMobile" => "Invalid input detected for Mobile",
                    "beneficiaryName" => "Invalid input detected for Name",
                    "beneficiaryPostCode" => "Invalid input detected for Post Code",
                    "beneficiaryTelephone" => "Invalid input detected for Telephone",
                    "benfBIC" => "Invalid input detected for BIC",
                    "benfIban" => "Invalid input detected for Iban",
                    "birthDate" => "Invalid input detected for Birth Date",
                    "branch" => "Invalid input detected for Branch",
                    "branchCode" => "Invalid input detected for Branch Code",
                    "firstName" => "Invalid input detected for First Name",
                    "ifscCode" => "Invalid input detected for Ifsc Code",
                    "lastName" => "Invalid input detected for Last Name",
                    "middleName" => "Invalid input detected for Middle Name",
                    "walletProvider" => "Invalid input detected for Wallet Provider",
                    "sectionvalue" => "Invalid input detected for Section Value",
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

           

            DataTable dt = new DataTable();


            obj.AccountHolderName = data.accountHolderName;
            obj.Account_Number = data.accountNumber;

            //Parth changes for confirm account number
            #region Confirm Account Number
            try
            {
                obj.Confirm_Account_Number = Convert.ToString(data.confirmAccountNumber);
                if (obj.Confirm_Account_Number == null) { obj.Confirm_Account_Number = ""; }

            }
            catch (Exception ex)
            {
                obj.Confirm_Account_Number = "";
            }
            #endregion
            //End Parth changes for confirm account number

            obj.Agent_MappingID = data.agentMappingID;

            obj.BBank_ID = data.bBankID;

            obj.BankCode = data.bankCode;

            obj.Bank_Name = data.bankName;

            obj.Beneficiary_Address = data.beneficiaryAddress;

            obj.Beneficiary_Address1 = data.beneficiaryAddress1;

            try
            {
                obj.BBDetails_ID = data.BBDetails_ID;
            }
            catch (Exception ex) { obj.BBDetails_ID = 0; }

            obj.City_Name = data.beneficiaryCityName;

            obj.Beneficiary_City_ID = data.beneficiaryCityID;

            obj.Country_Name = data.beneficiaryCountryName;

obj.Beneficiary_Country_ID = data.beneficiaryCountryID;
            obj.Beneficiary_ID = data.beneficiaryID;

            obj.Beneficiary_Mobile = data.beneficiaryMobile;

            obj.Beneficiary_Name = data.beneficiaryName;

            obj.Beneficiary_PostCode = data.beneficiaryPostCode;

            obj.Beneficiary_Telephone = data.beneficiaryTelephone;

            obj.Benf_BIC = data.benfBIC;

            obj.Benf_Iban = data.benfIban;

            obj.Birth_Date = data.birthDate;

            obj.Branch = data.branch;

            obj.BranchCode = data.branchCode;

            obj.Branch_ID = data.branchID;

            obj.Client_ID = data.clientID;

            obj.Collection_type_Id = data.collectionTypeId;

            try { obj.BankBranch_ID = Convert.ToInt32(data.BankBranch_ID); } catch (Exception ex) { obj.BankBranch_ID = 0; }

            obj.Created_By_User_ID = data.createdByUserID;

            obj.Customer_ID = data.customerID;

            obj.Delete_Status = data.deleteStatus;

    obj.First_Name = data.firstName;

    obj.Ifsc_Code = data.ifscCode;

    obj.Last_Name = data.lastName;


    obj.Middle_Name = data.middleName;

    obj.Mobile_provider = data.mobileProvider;

//obj.Record_Insert_DateTime = data.recordInsertDateTime;

    obj.Relation_ID = data.relationID;

    obj.Wallet_Id = data.walletId;

    obj.Wallet_provider = data.walletProvider;

    obj.blacklisted = data.blackListed;

            obj.cashcollection_flag = data.cashcollectionFlag;

            if (obj.Collection_type_Id == 2)
            {
                obj.cashcollection_flag = 0;
            }
            if (obj.Collection_type_Id == 1)
            {
                obj.cashcollection_flag = 1;
            }


            obj.sectionvalue = data.sectionvalue;

            try
            {
                obj.Beneficiary_Gender = Convert.ToInt32( data.Beneficiary_Gender);
            }
            catch (Exception ex) { obj.Beneficiary_Gender = 0; }

            try
            {
                obj.Beneficiary_State = Convert.ToString(data.Beneficiary_State);
                if (obj.Beneficiary_State == null) { obj.Beneficiary_State = ""; }
            }
            catch (Exception ex) { obj.Beneficiary_State = ""; }

            var validateJsonData = (dynamic)null;
           // var cm = li.SingleOrDefault();

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
            if (data.beneficiaryID == "" || data.beneficiaryID == null)
            {
                validateJsonData = new { response = false, responseCode = "02", data = "Invalid Beneficiary ID." };
                return new JsonResult(validateJsonData);
            }
            if (data.customerID == "" || data.customerID == null)
            {
                validateJsonData = new { response = false, responseCode = "02", data = "Invalid Customer ID." };
                return new JsonResult(validateJsonData);
            }

            //Parth changes for confirm account number
            #region Confirm Account Number Validation
            if (obj.Collection_type_Id == 1)
            {
                var returnJsonData = (dynamic)null;

                try
                {
                    string validbeneficiaryConfirmAccNumber = valideBeneficiaryBankData(obj, "ConfirmAccNo_man");
                    if (validbeneficiaryConfirmAccNumber == "T") // For Verify_Confirm_Account_no
                    {
                        returnJsonData = new { response = false, responseCode = "02", data = "Confirm Account Number is required." };
                        return new JsonResult(returnJsonData);
                    }
                }
                catch(Exception egx) { }

                string validbeneficiaryConfAccNumberMinRange = valideConfAccNoRangeData(obj, "Range_check_accno");
                if (validbeneficiaryConfAccNumberMinRange != "") // For Range_check_confaccno
                {
                    returnJsonData = new { response = false, responseCode = "02", data = validbeneficiaryConfAccNumberMinRange };
                    return new JsonResult(returnJsonData);
                }

                string validbeneficiaryConfirmAccountNumber = valideBeneficiaryAccNo(obj, "Confirm_Account_Number");
                if (validbeneficiaryConfirmAccountNumber == "T") // For Confirm_Account_no
                {
                    returnJsonData = new { response = false, responseCode = "02", data = " Confirm Account Number does not matches with Account Number." };
                    return new JsonResult(returnJsonData);
                }
            }
            #endregion
            //End Parth changes for confirm account number
            //Start Parth Added for validating first 5 digits is same i.e. zeroes
            #region validating all digits same
            try
            {
                var returnJsonData = (dynamic)null;

                bool validateIfscCode = IsFirst5DigitsSame(obj.Ifsc_Code);
                if (validateIfscCode)
                {
                    returnJsonData = new { response = false, responseCode = "02", data = "Invalid IFSC Code." };
                    return new JsonResult(returnJsonData);
                }
                bool validateBranchCode = IsFirst5DigitsSame(obj.BranchCode);
                if (validateBranchCode)
                {
                    returnJsonData = new { response = false, responseCode = "02", data = "Invalid Branch Code." };
                    return new JsonResult(returnJsonData);
                }
                bool validateIbanCode = IsFirst5DigitsSame(obj.Benf_Iban);
                if (validateIbanCode)
                {
                    returnJsonData = new { response = false, responseCode = "02", data = "Invalid IBAN Code." };
                    return new JsonResult(returnJsonData);
                }
            }
            catch (Exception ex)
            {
                _ = Task.Run(() => CompanyInfo.InsertErrorLogTracker(" Error: " + ex.ToString(), 0, 0, 0, 0, "saveBeneficiary", Convert.ToInt32(data.Branch_ID), Convert.ToInt32(data.Client_ID), "", HttpContext));
            }
            #endregion validating all digits same
            //End Parth Added for validating first 5 digits is same i.e. zeroes
            try
            {
                Model.Beneficiary _ObjCustomer = new Model.Beneficiary();
                Service.srvBeneficiary srvBenf = new Service.srvBeneficiary(HttpContext);
                
                _ObjCustomer = srvBenf.Update(obj , context);

                //response.ObjData = _ObjCustomer;
                if (_ObjCustomer.Id == 1)
                {
                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_TECHNICAL_ERROR);
                    response.data = "Validation Failed";
                    response.ObjData = (_ObjCustomer.Message).ToString();
                    response.ResponseCode = 1;
                    validateJsonData = new { response = false, responseCode = "02", data = _ObjCustomer.Message };
                    return new JsonResult(validateJsonData);
                }
                if (_ObjCustomer.Id == -1)
                {
                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_TECHNICAL_ERROR);
                    //response.ResponseMessage = cm.ResponseMessage;
                    response.data = "Failed to update beneficiary details!";
                    response.ResponseCode = 5;
                    validateJsonData = new { response = false, responseCode = "02", data = response.data };
                    return new JsonResult(validateJsonData);
                    //return response;
                }
                validateJsonData = new { response = true, responseCode = "00", data = "Beneficiary Updated Successfully." };
                return new JsonResult(validateJsonData);
            }
            catch (Exception ex)
            {
                string Activity = "Api editbenficiary: " + ex.ToString() + " ";
                validateJsonData = new { response = false, responseCode = "01", data = "Invalid Request" };
                CompanyInfo.InsertErrorLogTracker(Activity.ToString(), 0, 0, 0, 0, "editbenficiary", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "", context);
                return new JsonResult(validateJsonData);
            }

        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public string valideConfAccNoRangeData(Beneficiary obj, string fieldName)
        {
            string required = "";
            try
            {
                using (MySqlConnector.MySqlConnection con = new MySqlConnector.MySqlConnection(_configuration.GetConnectionString("connectionString")))
                {
                    con.Open();
                    MySqlConnector.MySqlCommand cmd_update1 = new MySqlConnector.MySqlCommand("SP_Beneficiary_Configuration");
                    cmd_update1.CommandType = CommandType.StoredProcedure;
                    cmd_update1.Connection = con;
                    cmd_update1.Parameters.AddWithValue("_Country_ID", obj.Beneficiary_Country_ID);
                    cmd_update1.Parameters.AddWithValue("_Collection_Type", obj.Collection_type_Id);
                    db_connection.ExecuteNonQueryProcedure(cmd_update1);
                    DataTable dtbenfconfig = db_connection.ExecuteQueryDataTableProcedure(cmd_update1);

                    if (fieldName == "Range_check_accno" && Convert.ToString(dtbenfconfig.Rows[0]["Range_check_accno"]) == "1" && Convert.ToString(dtbenfconfig.Rows[0]["ConfirmAccNo_man"]) == "0" || Convert.ToString(obj.Confirm_Account_Number).Trim() != "" && Convert.ToString(dtbenfconfig.Rows[0]["Range_check_accno"]) == "1")
                    {
                        if (Convert.ToString(dtbenfconfig.Rows[0]["ConfirmAccNo_vis"]) == "0" && Convert.ToString(obj.Confirm_Account_Number).Trim().Length < Convert.ToInt16(dtbenfconfig.Rows[0]["Acc_no_min_length"]))
                        {
                            required = "Confirm Account Number length should be minimum " + Convert.ToInt16(dtbenfconfig.Rows[0]["Acc_no_min_length"]) + " digit long.";
                        }
                        else if (Convert.ToString(dtbenfconfig.Rows[0]["ConfirmAccNo_vis"]) == "0" && Convert.ToString(obj.Confirm_Account_Number).Trim().Length > Convert.ToInt16(dtbenfconfig.Rows[0]["Acc_no_length"]))
                        {
                            required = "Confirm Account Number length should be maximum " + Convert.ToInt16(dtbenfconfig.Rows[0]["Acc_no_length"]) + " digit long.";
                        }
                    }
                    else if (fieldName == "Range_check_accno" && Convert.ToString(dtbenfconfig.Rows[0]["Range_check_accno"]) == "0" && Convert.ToString(dtbenfconfig.Rows[0]["ConfirmAccNo_man"]) == "0" || Convert.ToString(obj.Confirm_Account_Number).Trim() != "" && Convert.ToString(dtbenfconfig.Rows[0]["Range_check_accno"]) == "0")
                    {
                        if (Convert.ToString(dtbenfconfig.Rows[0]["ConfirmAccNo_vis"]) == "0" && Convert.ToString(obj.Confirm_Account_Number).Trim().Length != Convert.ToInt16(dtbenfconfig.Rows[0]["Acc_no_length"]))
                        {
                            required = "Confirm Account Number length must be " + Convert.ToInt16(dtbenfconfig.Rows[0]["Acc_no_length"]) + " digit long";
                        }
                    }
                }
            }
            catch (Exception egx) { }
            return required;
        }

        [HttpPost]
        [Route("updaterelationdetails")]
        public IActionResult UpdateRelationDetails([FromBody] JsonElement obj1)
        {
            HttpContext context = HttpContext;
            string json = System.Text.Json.JsonSerializer.Serialize(obj1);
            var claimsIdentity = User.Identity as ClaimsIdentity;//sanket
            var authHeader = "";
            try { authHeader = HttpContext.Request.Headers["Authorization"].ToString(); } catch (Exception egx) { authHeader = ""; }

            bool auth = AuthController.checkAuth(claimsIdentity, obj1, Convert.ToString(authHeader));
            if (!auth)
            {
                var errorResponse = new { response = false, responseCode = "403", data = "Access Forbidden" };
                return new JsonResult(errorResponse) { StatusCode = (int)HttpStatusCode.Forbidden };
            }
            CompanyInfo.InsertrequestLogTracker("UpdateRelationDetails full request body: " + JObject.Parse(json), 0, 0, 0, 0, "updaterelationdetails", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
            dynamic data = JObject.Parse(json);
            Model.response.WebResponse response = null;
            var validateJsonData = (dynamic)null;
            Beneficiary obj = new Beneficiary();

            try
            {
                Model.Beneficiary _ObjCustomer = new Model.Beneficiary();
                Service.srvBeneficiary srvBenf = new Service.srvBeneficiary(HttpContext);
                _ObjCustomer.Client_ID = data.clientID;
                _ObjCustomer.Customer_ID = data.customerID;
                _ObjCustomer.Branch_ID = data.branchID;
                _ObjCustomer.Beneficiary_ID = data.beneficiaryID;
                _ObjCustomer.Relation_ID = data.relationID;

                Model.Beneficiary result = srvBenf.Update_RelationDetails(_ObjCustomer, context);

                if (result != null && result.Message == "success")
                {
                    validateJsonData = new { response = true, responseCode = "00", data = result };
                }
                else
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "Failed to update" };
                }
            }
            catch (Exception ex)
            {
                string Activity = "Api:- updaterelationdetails" + ex.ToString();
                validateJsonData = new { response = false, responseCode = "02", data = "Failed to update" };
                CompanyInfo.InsertErrorLogTracker(Activity.ToString(), 0, 0, 0, 0, "updaterelationdetails", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "", context);
            }
            return new JsonResult(validateJsonData);
        }

        [HttpPost]
        [Route("updatemobiledetails")]
        public IActionResult Update_MobileDetails([FromBody] JsonElement obj1)
        {
            HttpContext context = HttpContext;
            string json = System.Text.Json.JsonSerializer.Serialize(obj1);
            var claimsIdentity = User.Identity as ClaimsIdentity;//sanket
            var authHeader = "";
            try { authHeader = HttpContext.Request.Headers["Authorization"].ToString(); } catch (Exception egx) { authHeader = ""; }

            bool auth = AuthController.checkAuth(claimsIdentity, obj1, Convert.ToString(authHeader));
            if (!auth)
            {
                var errorResponse = new { response = false, responseCode = "403", data = "Access Forbidden" };
                return new JsonResult(errorResponse) { StatusCode = (int)HttpStatusCode.Forbidden };
            }
            CompanyInfo.InsertrequestLogTracker("Update_MobileDetails full request body: " + JObject.Parse(json), 0, 0, 0, 0, "updatemobiledetails", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
            dynamic data = JObject.Parse(json);
            Model.response.WebResponse response = null;
            var validateJsonData = (dynamic)null;
            Beneficiary obj = new Beneficiary();

            try
            {
                Model.Beneficiary _ObjCustomer = new Model.Beneficiary();
                Service.srvBeneficiary srvBenf = new Service.srvBeneficiary(HttpContext);
                _ObjCustomer.Client_ID = data.clientID;
                _ObjCustomer.Customer_ID = data.customerID;
                _ObjCustomer.Branch_ID = data.branchID;
                _ObjCustomer.Beneficiary_ID = data.beneficiaryID;
                _ObjCustomer.Beneficiary_Mobile = data.beneficiaryMobile;

                Model.Beneficiary result = srvBenf.Update_MobileDetails(_ObjCustomer, context);

                if (result != null && result.Message == "success")
                {
                    validateJsonData = new { response = true, responseCode = "00", data = result };
                }
                else
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "Failed to update record." };
                }
            }
            catch (Exception ex)
            {
                string Activity = "Api:- updatemobiledetails" + ex.ToString();
                validateJsonData = new { response = false, responseCode = "02", data = "Failed to update record." };
                CompanyInfo.InsertErrorLogTracker(Activity.ToString(), 0, 0, 0, 0, "updatemobiledetails", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "", context);
            }
            return new JsonResult(validateJsonData);
        }


        [ApiExplorerSettings(IgnoreApi = true)]
        public string valideBeneficiaryRangeDataData(Beneficiary obj, string fieldName)
        {
            string required = "";

            using (MySqlConnector.MySqlConnection con = new MySqlConnector.MySqlConnection(_configuration.GetConnectionString("connectionString")))
            {
                con.Open();
                MySqlConnector.MySqlCommand cmd_update1 = new MySqlConnector.MySqlCommand("SP_Beneficiary_Configuration");
                cmd_update1.CommandType = CommandType.StoredProcedure;
                cmd_update1.Connection = con;
                cmd_update1.Parameters.AddWithValue("_Country_ID", obj.Beneficiary_Country_ID);
                cmd_update1.Parameters.AddWithValue("_Collection_Type", obj.Collection_type_Id);
                db_connection.ExecuteNonQueryProcedure(cmd_update1);
                DataTable dtbenfconfig = db_connection.ExecuteQueryDataTableProcedure(cmd_update1);

                if (fieldName == "Range_check_accno" && Convert.ToString(dtbenfconfig.Rows[0]["Range_check_accno"]) == "1" && Convert.ToString(dtbenfconfig.Rows[0]["ShowAccount_No"]) != "1")
                {
                    if (Convert.ToString(obj.Account_Number).Trim().Length < Convert.ToInt16(dtbenfconfig.Rows[0]["Acc_no_min_length"]))
                    {
                        required = "Account length min. require " + Convert.ToInt16(dtbenfconfig.Rows[0]["Acc_no_min_length"]) + " digit long.";
                    }
                    else if (Convert.ToString(obj.Account_Number).Trim().Length > Convert.ToInt16(dtbenfconfig.Rows[0]["Acc_no_length"]))
                    {
                        required = "Account length max. require " + Convert.ToInt16(dtbenfconfig.Rows[0]["Acc_no_length"]) + " digit long.";
                    }
                }
                else if (fieldName == "Range_check_accno" && Convert.ToString(dtbenfconfig.Rows[0]["Range_check_accno"]) == "0" && Convert.ToString(dtbenfconfig.Rows[0]["ShowAccount_No"]) != "1")
                {
                    if (Convert.ToString(obj.Account_Number).Trim().Length != Convert.ToInt16(dtbenfconfig.Rows[0]["Acc_no_length"]))
                    {
                        required = "Account length must be " + Convert.ToInt16(dtbenfconfig.Rows[0]["Acc_no_length"]) + " digit long";
                    }
                }
                else if (fieldName == "Beneficiary_Mobile" && Convert.ToString(dtbenfconfig.Rows[0]["Range_check_mobile"]) == "0" && Convert.ToString(dtbenfconfig.Rows[0]["Beneficiary_Mobile"]) != "2")
                {   //Fixed
                    if (Convert.ToInt16(dtbenfconfig.Rows[0]["Beneficiary_Mobile"]) == 0)
                    {   // Mobile Mandetory
                        if (Convert.ToString(obj.Beneficiary_Mobile).Trim().Length != Convert.ToInt16(dtbenfconfig.Rows[0]["mobile_max"]))
                        {
                            required = "Mobile length must be " + Convert.ToInt16(dtbenfconfig.Rows[0]["mobile_max"]) + " digit long";
                        }
                    }
                    else if (Convert.ToInt16(dtbenfconfig.Rows[0]["Beneficiary_Mobile"]) == 1)
                    {   // Mobile Optional
                        if (Convert.ToString(obj.Beneficiary_Mobile).Trim().Length > 0 && Convert.ToString(obj.Beneficiary_Mobile).Trim().Length != Convert.ToInt16(dtbenfconfig.Rows[0]["mobile_max"]))
                        {
                            required = "Mobile length must be " + Convert.ToInt16(dtbenfconfig.Rows[0]["mobile_max"]) + " digit long";
                        }
                    }

                }
                else if (fieldName == "Beneficiary_Mobile" && Convert.ToString(dtbenfconfig.Rows[0]["Range_check_mobile"]) == "1" && Convert.ToString(dtbenfconfig.Rows[0]["Beneficiary_Mobile"]) != "2")
                {   //Custom
                    if (Convert.ToInt16(dtbenfconfig.Rows[0]["Beneficiary_Mobile"]) == 0)
                    {   // Mobile Mandetory
                        if (Convert.ToString(obj.Beneficiary_Mobile).Trim().Length < Convert.ToInt16(dtbenfconfig.Rows[0]["mobile_min"]) && Convert.ToInt16(dtbenfconfig.Rows[0]["mobile_min"]) > 0)
                        {
                            required = "Mobile length min. require " + Convert.ToInt16(dtbenfconfig.Rows[0]["mobile_min"]) + " digit long.";
                        }
                        else if (Convert.ToString(obj.Beneficiary_Mobile).Trim().Length > Convert.ToInt16(dtbenfconfig.Rows[0]["mobile_max"]))
                        {
                            required = "Mobile length max. require " + Convert.ToInt16(dtbenfconfig.Rows[0]["mobile_max"]) + " digit long.";
                        }
                    }
                    else if (Convert.ToInt16(dtbenfconfig.Rows[0]["Beneficiary_Mobile"]) == 1)
                    {   // Mobile Optional
                        if (Convert.ToString(obj.Beneficiary_Mobile).Trim().Length > 0 && Convert.ToString(obj.Beneficiary_Mobile).Trim().Length < Convert.ToInt16(dtbenfconfig.Rows[0]["mobile_min"]) && Convert.ToInt16(dtbenfconfig.Rows[0]["mobile_min"]) > 0)
                        {
                            required = "Mobile length min. require " + Convert.ToInt16(dtbenfconfig.Rows[0]["mobile_min"]) + " digit long.";
                        }
                        else if (Convert.ToString(obj.Beneficiary_Mobile).Trim().Length > 0 && Convert.ToString(obj.Beneficiary_Mobile).Trim().Length > Convert.ToInt16(dtbenfconfig.Rows[0]["mobile_max"]))
                        {
                            required = "Mobile length max. require " + Convert.ToInt16(dtbenfconfig.Rows[0]["mobile_max"]) + " digit long.";
                        }
                    }
                }
                else if (fieldName == "Validate_BIC" && (Convert.ToString(dtbenfconfig.Rows[0]["Validate_BIC"]) == "0" || Convert.ToString(dtbenfconfig.Rows[0]["BIC_Status"]) == "0"))
                {   // BIC Code
                    if ( (Convert.ToString(obj.Benf_BIC).Trim() == "" || Convert.ToString(obj.Benf_BIC).Trim() == null) && Convert.ToString(dtbenfconfig.Rows[0]["Validate_BIC"]) == "0")
                    {
                        required = Convert.ToString(dtbenfconfig.Rows[0]["BIC_Lable"]).Trim() + " is required.";
                    }
                    else {
                        if (Convert.ToInt16(dtbenfconfig.Rows[0]["Range_check_BIC"]) == 1)
                        {   // Custom BIC Code 

                            if (Convert.ToInt16(dtbenfconfig.Rows[0]["Validate_BIC"]) == 0)
                            {   // Mandetory BIC
                                if (Convert.ToString(obj.Benf_BIC).Trim().Length == 0)
                                {
                                    required = Convert.ToString(dtbenfconfig.Rows[0]["BIC_Lable"]).Trim() + " length min. require " + Convert.ToInt16(dtbenfconfig.Rows[0]["BIC_Min_Length"]) + " digit long.";
                                }
                                else if (Convert.ToString(obj.Benf_BIC).Trim().Length < Convert.ToInt16(dtbenfconfig.Rows[0]["BIC_Min_Length"]) && Convert.ToString(obj.Benf_BIC).Trim().Length > 0)
                                {
                                    required = Convert.ToString(dtbenfconfig.Rows[0]["BIC_Lable"]).Trim() + " length min. require " + Convert.ToInt16(dtbenfconfig.Rows[0]["BIC_Min_Length"]) + " digit long.";
                                }
                                else if (Convert.ToString(obj.Benf_BIC).Trim().Length > Convert.ToInt16(dtbenfconfig.Rows[0]["BIC_Length"]))
                                {
                                    required = Convert.ToString(dtbenfconfig.Rows[0]["BIC_Lable"]).Trim() + " length max. require " + Convert.ToInt16(dtbenfconfig.Rows[0]["BIC_Length"]) + " digit long.";
                                }
                            }
                            else
                            {   // Optional BIC Code
                                if (Convert.ToString(obj.Benf_BIC).Trim().Length > 0)
                                {
                                    if (Convert.ToString(obj.Benf_BIC).Trim().Length < Convert.ToInt16(dtbenfconfig.Rows[0]["BIC_Min_Length"]) )
                                    {
                                        required = Convert.ToString(dtbenfconfig.Rows[0]["BIC_Lable"]).Trim() + " length min. require " + Convert.ToInt16(dtbenfconfig.Rows[0]["BIC_Min_Length"]) + " digit long.";
                                    }
                                    else if (Convert.ToString(obj.Benf_BIC).Trim().Length > Convert.ToInt16(dtbenfconfig.Rows[0]["BIC_Length"]))
                                    {
                                        required = Convert.ToString(dtbenfconfig.Rows[0]["BIC_Lable"]).Trim() + " length max. require " + Convert.ToInt16(dtbenfconfig.Rows[0]["BIC_Length"]) + " digit long.";
                                    }
                                }
                            }
                        }
                        if (Convert.ToInt16(dtbenfconfig.Rows[0]["Range_check_BIC"]) == 0   )
                        {   // Fixed BIC Code
                            if (Convert.ToInt16(dtbenfconfig.Rows[0]["Validate_BIC"]) == 0)
                            {   // Mandetory BIC
                                if (Convert.ToString(obj.Benf_BIC).Trim().Length != Convert.ToInt16(dtbenfconfig.Rows[0]["BIC_Length"]))
                                {
                                    required = Convert.ToString(dtbenfconfig.Rows[0]["BIC_Lable"]).Trim() + " length must be " + Convert.ToInt16(dtbenfconfig.Rows[0]["BIC_Length"]) + " digit long.";
                                }
                            }
                            else
                            {   // Optional BIC Code
                                if (Convert.ToString(obj.Benf_BIC).Trim().Length != Convert.ToInt16(dtbenfconfig.Rows[0]["BIC_Length"]) && Convert.ToString(obj.Benf_BIC).Trim().Length > 0)
                                {
                                    required = Convert.ToString(dtbenfconfig.Rows[0]["BIC_Lable"]).Trim() + " length must be " + Convert.ToInt16(dtbenfconfig.Rows[0]["BIC_Length"]) + " digit long.";
                                }
                            }
                        }

                    }
                }
                else if (fieldName == "IBAN_Mandatory" && (Convert.ToString(dtbenfconfig.Rows[0]["IBAN_Mandatory"]) == "0" || Convert.ToString(dtbenfconfig.Rows[0]["IBAN_Status"]) == "0"))
                {   // IBAN Code
                    if ( (Convert.ToString(obj.Benf_Iban).Trim() == "" || Convert.ToString(obj.Benf_Iban).Trim() == null) && Convert.ToString(dtbenfconfig.Rows[0]["IBAN_Mandatory"]) == "0")
                    {
                        required = Convert.ToString(dtbenfconfig.Rows[0]["IBAN_Label"]).Trim() + " is required.";
                    }
                    else
                    {
                        if (Convert.ToInt16(dtbenfconfig.Rows[0]["Range_check_Iban"]) == 1)
                        {   // Custom IBAN Code
                            if (Convert.ToString(dtbenfconfig.Rows[0]["IBAN_Mandatory"]) == "1")
                            {
                                if (Convert.ToString(obj.Benf_Iban).Trim().Length != 0)
                                {
                                    required = Convert.ToString(dtbenfconfig.Rows[0]["IBAN_Label"]).Trim() + " length max. require " + Convert.ToInt16(dtbenfconfig.Rows[0]["IBAN_Length"]) + " digit long.";
                                }
                            }
                            else if (Convert.ToString(obj.Benf_Iban).Trim().Length < Convert.ToInt16(dtbenfconfig.Rows[0]["IBAN_Min_Length"]))
                            {
                                required = Convert.ToString(dtbenfconfig.Rows[0]["IBAN_Label"]).Trim() + " length min. require " + Convert.ToInt16(dtbenfconfig.Rows[0]["IBAN_Min_Length"]) + " digit long.";
                            }
                            else if (Convert.ToString(obj.Benf_Iban).Trim().Length > Convert.ToInt16(dtbenfconfig.Rows[0]["IBAN_Length"]))
                            {
                                required = Convert.ToString(dtbenfconfig.Rows[0]["IBAN_Label"]).Trim() + " length max. require " + Convert.ToInt16(dtbenfconfig.Rows[0]["IBAN_Length"]) + " digit long.";
                            }
                        }
                        if (Convert.ToInt16(dtbenfconfig.Rows[0]["Range_check_Iban"]) == 0)
                        {// Fixed IBAN Code
                            if(Convert.ToString(dtbenfconfig.Rows[0]["IBAN_Mandatory"]) == "1")
                            {
                                if(Convert.ToString(obj.Benf_Iban).Trim().Length == 0)
                                {

                                }
                                else if (Convert.ToString(obj.Benf_Iban).Trim().Length != Convert.ToInt16(dtbenfconfig.Rows[0]["IBAN_Length"]))
                                {
                                    required = Convert.ToString(dtbenfconfig.Rows[0]["IBAN_Label"]).Trim() + " length must be " + Convert.ToInt16(dtbenfconfig.Rows[0]["IBAN_Length"]) + " digit long.";
                                }
                            }
                            else if (Convert.ToString(obj.Benf_Iban).Trim().Length != Convert.ToInt16(dtbenfconfig.Rows[0]["IBAN_Length"]))
                            {
                                required = Convert.ToString(dtbenfconfig.Rows[0]["IBAN_Label"]).Trim() + " length must be " + Convert.ToInt16(dtbenfconfig.Rows[0]["IBAN_Length"]) + " digit long.";
                            }
                        }

                    }
                }
                else if (fieldName == "Branch_Code" && Convert.ToString(dtbenfconfig.Rows[0]["Branch_code_man"]) == "0")
                {   // Branch Code
                    if (Convert.ToString(obj.BranchCode).Trim() == "" || Convert.ToString(obj.BranchCode).Trim() == null)
                    {
                        required = Convert.ToString(dtbenfconfig.Rows[0]["Branch_code_name"]).Trim() + " is required.";
                    }
                }
                else if (fieldName == "IFSC_Mandatory" && (Convert.ToString(dtbenfconfig.Rows[0]["IFSC_Mandatory"]) == "0" || Convert.ToString(dtbenfconfig.Rows[0]["IFSC"]) == "0"))
                {   // IFSC Code
                    if ((Convert.ToString(obj.Ifsc_Code).Trim() == "" || Convert.ToString(obj.Ifsc_Code).Trim() == null) && Convert.ToString(dtbenfconfig.Rows[0]["IFSC_Mandatory"]) == "0")
                    {
                        required = Convert.ToString(dtbenfconfig.Rows[0]["IFSC_Lable"]).Trim() + " is required.";
                    }
                    else
                    {
                        if (Convert.ToInt16(dtbenfconfig.Rows[0]["Range_check_ifsc"]) == 1)
                        {   // Custom IFSC Code 
                            if (Convert.ToInt16(dtbenfconfig.Rows[0]["IFSC_Mandatory"]) == 0)
                            {   // IFSC mandetory
                                if (Convert.ToString(obj.Ifsc_Code).Trim().Length < Convert.ToInt16(dtbenfconfig.Rows[0]["Ifsc_no_min_length"]))
                                {
                                    required = Convert.ToString(dtbenfconfig.Rows[0]["IFSC_Lable"]).Trim() + " length min. require " + Convert.ToInt16(dtbenfconfig.Rows[0]["Ifsc_no_min_length"]) + " digit long.";
                                }
                                else if (Convert.ToString(obj.Ifsc_Code).Trim().Length > Convert.ToInt16(dtbenfconfig.Rows[0]["Ifsc_no_length"]))
                                {
                                    required = Convert.ToString(dtbenfconfig.Rows[0]["IFSC_Lable"]).Trim() + " length max. require " + Convert.ToInt16(dtbenfconfig.Rows[0]["Ifsc_no_length"]) + " digit long.";
                                }
                            }
                            else
                            {
                                if (Convert.ToString(obj.Ifsc_Code).Trim().Length < Convert.ToInt16(dtbenfconfig.Rows[0]["Ifsc_no_min_length"]) && Convert.ToString(obj.Ifsc_Code).Trim().Length > 0)
                                {
                                    required = Convert.ToString(dtbenfconfig.Rows[0]["IFSC_Lable"]).Trim() + " length min. require " + Convert.ToInt16(dtbenfconfig.Rows[0]["Ifsc_no_min_length"]) + " digit long.";
                                }
                                else if (Convert.ToString(obj.Ifsc_Code).Trim().Length > Convert.ToInt16(dtbenfconfig.Rows[0]["Ifsc_no_length"]) && Convert.ToString(obj.Ifsc_Code).Trim().Length > 0)
                                {
                                    required = Convert.ToString(dtbenfconfig.Rows[0]["IFSC_Lable"]).Trim() + " length max. require " + Convert.ToInt16(dtbenfconfig.Rows[0]["Ifsc_no_length"]) + " digit long.";
                                }
                            }
                        }
                        if (Convert.ToInt16(dtbenfconfig.Rows[0]["Range_check_ifsc"]) == 0)
                        {// Fixed IFSC Code

                            if (Convert.ToInt16(dtbenfconfig.Rows[0]["IFSC_Mandatory"]) == 0)
                            {   // IFSC mandetory
                                if (Convert.ToString(obj.Ifsc_Code).Trim().Length != Convert.ToInt16(dtbenfconfig.Rows[0]["Ifsc_no_length"]))
                                {
                                    required = Convert.ToString(dtbenfconfig.Rows[0]["IFSC_Lable"]).Trim() + " length must be " + Convert.ToInt16(dtbenfconfig.Rows[0]["Ifsc_no_length"]) + " digit long.";
                                }
                            }
                            else
                            {   // IFSC Option
                                if (Convert.ToString(obj.Ifsc_Code).Trim().Length != Convert.ToInt16(dtbenfconfig.Rows[0]["Ifsc_no_length"]) && Convert.ToString(obj.Ifsc_Code).Trim().Length > 0)
                                {
                                    required = Convert.ToString(dtbenfconfig.Rows[0]["IFSC_Lable"]).Trim() + " length must be " + Convert.ToInt16(dtbenfconfig.Rows[0]["Ifsc_no_length"]) + " digit long.";
                                }
                            }
                            
                        }

                    }
                }

                else if (fieldName == "Branch" && Convert.ToString(dtbenfconfig.Rows[0]["Branch"]) == "0")
                {   // Branch Name
                    if (Convert.ToString(obj.Branch) == "" || Convert.ToString(obj.Branch) == null)
                    {
                        required =  "Branch is required.";
                    }
                }
                else if (fieldName == "BankCode" && Convert.ToString(dtbenfconfig.Rows[0]["Bank_Code"]) == "0")
                {   // Bank Code
                    if (Convert.ToString(obj.BankCode) == "" || Convert.ToString(obj.BankCode) == null)
                    {
                        required = "Bank Code is required.";
                    }
                }
                else if (fieldName == "Bank_Name" && Convert.ToString(dtbenfconfig.Rows[0]["Bank_Name"]) == "0")
                {   // Bank Name
                    if (Convert.ToString(obj.BBank_ID) == "" || Convert.ToString(obj.BBank_ID) == null)
                    {
                        required = "Bank Name is required.";
                    }
                }
                



            }

            return required;
        }

        [HttpPost]
        [Route("benefcollectionmapping")]
        public IActionResult GetBenf_CollType_Mapping_Details([FromBody] JsonElement obj)
        {
            HttpContext context = HttpContext;
            string json = System.Text.Json.JsonSerializer.Serialize(obj);
            dynamic data = JObject.Parse(json); string Status = string.Empty;
            CompanyInfo.InsertrequestLogTracker("benefcollectionmapping full request body: " + JObject.Parse(json), 0, 0, 0, 0, "GetBenf_CollType_Mapping_Details", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
            Model.response.WebResponse response = null;


            if (!SqlInjectionProtector.ReadJsonElementValues(obj) || !SqlInjectionProtector.ReadJsonElementValuesScript(obj))
            {
                var errorResponse = new { response = false, responseCode = "02", data = "Invalid input detected." };
                return new JsonResult(errorResponse) { StatusCode = 400 };
            }

            Transaction obj1 = JsonConvert.DeserializeObject<Transaction>(json);
            var validateJsonData = (dynamic)null;


            int number;
            if (!int.TryParse(Convert.ToString(data.Client_ID), out number))
            {
                validateJsonData = new { response = false, responseCode = "02", data = "Invalid Client ID." };
                return new JsonResult(validateJsonData);
            }
            if (!int.TryParse(Convert.ToString(data.Branch_ID), out number))
            {
                validateJsonData = new { response = false, responseCode = "02", data = "Invalid branchID field." };
                return new JsonResult(validateJsonData);
            }
            if (data.Beneficiary_ID == "" || data.Beneficiary_ID == null)
            {
                validateJsonData = new { response = false, responseCode = "02", data = "Invalid Beneficiary ID." };
                return new JsonResult(validateJsonData);
            }
            try
            {
                Service.srvPaymentDepositType srv = new Service.srvPaymentDepositType();
                DataTable dt_collection_mapping = srv.GetBenf_CollType_Mapping_Details(obj1);

                if (dt_collection_mapping != null && dt_collection_mapping.Rows.Count > 0)
                {
                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_SUCCESS);
                    response.data = "success";
                    response.ObjData = dt_collection_mapping;
                    response.ResponseCode = 0;

                    var relationshipData = dt_collection_mapping.Rows.OfType<DataRow>()
               .Select(row => dt_collection_mapping.Columns.OfType<DataColumn>()
                   .ToDictionary(col => col.ColumnName, c => row[c].ToString()));
                    validateJsonData = new { response = true, responseCode = "00", data = relationshipData };
                    return new JsonResult(validateJsonData);
                }
                else
                {
                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_TECHNICAL_ERROR);
                    response.ResponseMessage = "No Records Found.";
                    response.ResponseCode = 0;
                    validateJsonData = new { response = false, responseCode = "02", data = "No Records Found." };
                    return new JsonResult(validateJsonData);
                }
            }
            catch (Exception ex)
            {
                string Activity = "Api benefcollectionmapping: " + ex.ToString() + " ";
                validateJsonData = new { response = false, responseCode = "01", data = "Invalid Request" };
                CompanyInfo.InsertErrorLogTracker(Activity.ToString(), 0, 0, 0, 0, "benefcollectionmapping", Convert.ToInt32(obj1.Branch_ID), Convert.ToInt32(obj1.Client_ID), "", context);
                return new JsonResult(validateJsonData);
            }

        }

        [HttpPost]
        [Route("savebeneficiarybank")]

        public IActionResult SaveBeneficiaryBank([FromBody] JsonElement obj)
        {
            HttpContext context = HttpContext;
            string json = System.Text.Json.JsonSerializer.Serialize(obj);
            dynamic data = JObject.Parse(json); string Status = string.Empty;
            var claimsIdentity = User.Identity as ClaimsIdentity;//sanket
            var authHeader = "";
            try { authHeader = HttpContext.Request.Headers["Authorization"].ToString(); } catch (Exception egx) { authHeader = ""; }

            bool auth = AuthController.checkAuth(claimsIdentity, obj, Convert.ToString(authHeader));
            if (!auth)
            {
                var errorResponse = new { response = false, responseCode = "403", data = "Access Forbidden" };
                return new JsonResult(errorResponse) { StatusCode = (int)HttpStatusCode.Forbidden };
            }
            CompanyInfo.InsertrequestLogTracker("savebeneficiarybank full request body: " + JObject.Parse(json), 0, 0, 0, 0, "savebeneficiarybank", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
            Model.response.WebResponse response = null;

            if (!SqlInjectionProtector.ReadJsonElementValues(obj) || !SqlInjectionProtector.ReadJsonElementValuesScript(obj))
            {
                var errorResponse = new { response = false, responseCode = "02", data = "Invalid input detected." };
                return new JsonResult(errorResponse) { StatusCode = 400 };
            }

            Beneficiary obj1 = JsonConvert.DeserializeObject<Beneficiary>(json);
            var validateJsonData = (dynamic)null;

            try
            {

                int number;
                if (!int.TryParse(Convert.ToString(data.Client_ID), out number))
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "Invalid Client ID." };
                    return new JsonResult(validateJsonData);
                }
                if (!int.TryParse(Convert.ToString(data.Branch_ID), out number))
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "Invalid branchID field." };
                    return new JsonResult(validateJsonData);
                }
                if (data.Beneficiary_ID == "" || data.Beneficiary_ID == null)
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "Invalid Beneficiary ID." };
                    return new JsonResult(validateJsonData);
                }

                Model.Beneficiary _ObjCustomer = new Model.Beneficiary();
                Service.srvBeneficiary srvBenf = new Service.srvBeneficiary(context);
                response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_SUCCESS);
                _ObjCustomer = srvBenf.AddBenfBankAccount(obj1, context);
                response.ObjData = _ObjCustomer;


                if (_ObjCustomer.Id == 0 && _ObjCustomer.whereclause == "Validation Failed")
                {
                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_TECHNICAL_ERROR);
                    response.data = "Validation Failed";
                    response.ObjData = (_ObjCustomer.Message).ToString();
                    response.ResponseCode = 6;
                    validateJsonData = new { response = false, responseCode = "02", data = "Validation Failed" };

                }
                else if (_ObjCustomer.Id == -1)
                {
                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_TECHNICAL_ERROR);
                    response.ResponseCode = 5;
                    validateJsonData = new { response = false, responseCode = "02", data = "Validation Failed" };

                }
                else if (_ObjCustomer.Message == "success")
                {
                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_SUCCESS);
                    response.ObjData = _ObjCustomer;
                    response.ResponseCode = 0;
                    validateJsonData = new { response = true, responseCode = "00", data = _ObjCustomer.Message };


                }
                else if (_ObjCustomer.Message == "Notsucess")
                {
                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_SUCCESS);
                    response.ObjData = _ObjCustomer;
                    response.ResponseCode = 0;
                    validateJsonData = new { response = false, responseCode = "02", data = _ObjCustomer.Message };


                }
                return new JsonResult(validateJsonData);
            }
            catch (Exception ex)
            {
                string Activity = "Api SaveBeneficiaryBank: " + ex.ToString() + " ";
                validateJsonData = new { response = false, responseCode = "01", data = "Invalid Request" };
                CompanyInfo.InsertErrorLogTracker(Activity.ToString(), 0, 0, 0, 0, "SaveBeneficiaryBank", Convert.ToInt32(obj1.Branch_ID), Convert.ToInt32(obj1.Client_ID), "", context);
                return new JsonResult(validateJsonData);

            }
        }

        [HttpPost]
        [Route("updatebeneficiarybank")]

        public IActionResult updatebeneficiarybank([FromBody] JsonElement obj)
        {
            HttpContext context = HttpContext;
            string json = System.Text.Json.JsonSerializer.Serialize(obj);
            dynamic data = JObject.Parse(json); string Status = string.Empty;
            var claimsIdentity = User.Identity as ClaimsIdentity;//sanket
            //var authHeader = "";
            //try { authHeader = HttpContext.Request.Headers["Authorization"].ToString(); } catch (Exception egx) { authHeader = ""; }

            //bool auth = AuthController.checkAuth(claimsIdentity, obj, Convert.ToString(authHeader));
            //if (!auth)
            //{
            //    var errorResponse = new { response = false, responseCode = "403", data = "Access Forbidden" };
            //    return new JsonResult(errorResponse) { StatusCode = (int)HttpStatusCode.Forbidden };
            //}
            CompanyInfo.InsertrequestLogTracker("updatebeneficiarybank full request body: " + JObject.Parse(json), 0, 0, 0, 0, "updatebeneficiarybank", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
            Model.response.WebResponse response = null;

            if (!SqlInjectionProtector.ReadJsonElementValues(obj) || !SqlInjectionProtector.ReadJsonElementValuesScript(obj))
            {
                var errorResponse = new { response = false, responseCode = "02", data = "Invalid input detected." };
                return new JsonResult(errorResponse) { StatusCode = 400 };
            }

            Beneficiary obj1 = JsonConvert.DeserializeObject<Beneficiary>(json);
            var validateJsonData = (dynamic)null;

            try
            {

                int number;
                if (!int.TryParse(Convert.ToString(data.Client_ID), out number))
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "Invalid Client ID." };
                    return new JsonResult(validateJsonData);
                }
                if (!int.TryParse(Convert.ToString(data.Branch_ID), out number))
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "Invalid branchID field." };
                    return new JsonResult(validateJsonData);
                }
                if (data.Beneficiary_ID == "" || data.Beneficiary_ID == null)
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "Invalid Beneficiary ID." };
                    return new JsonResult(validateJsonData);
                }

                Model.Beneficiary _ObjCustomer = new Model.Beneficiary();
                Service.srvBeneficiary srvBenf = new Service.srvBeneficiary(context);
                response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_SUCCESS);
                _ObjCustomer = srvBenf.UpdateBank(obj1, context);
                response.ObjData = _ObjCustomer;


                if (_ObjCustomer.Id == 1)
                {
                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_TECHNICAL_ERROR);
                    response.data = "Validation Failed";
                    response.ObjData = (_ObjCustomer.Message).ToString();
                    response.ResponseCode = 6;
                    validateJsonData = new { response = false, responseCode = "02", data = "Validation Failed" };

                }
                else if (_ObjCustomer.Id == -1)
                {
                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_TECHNICAL_ERROR);
                    response.ResponseCode = 5;
                    validateJsonData = new { response = false, responseCode = "02", data = "Validation Failed" };

                }
                else if (_ObjCustomer.Id == 0 && _ObjCustomer.Message == "success")
                {
                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_TECHNICAL_ERROR);
                    response.ResponseCode = 5;
                    validateJsonData = new { response = true, responseCode = "00", data = _ObjCustomer.Message };

                }
                else if (_ObjCustomer.Message == "Notsucess")
                {
                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_TECHNICAL_ERROR);
                    response.ResponseCode = 5;
                    validateJsonData = new { response = false, responseCode = "02", data = _ObjCustomer.Message };

                }
                else if (_ObjCustomer.Id == 2)//Digvijay chanegs for beneficiary name validation for restricted keywords
                {
                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_SUCCESS);
                    response.ObjData = _ObjCustomer;
                    response.ResponseCode = 0;
                    validateJsonData = new { response = true, responseCode = "00", data = _ObjCustomer.Message };


                }
                else if (_ObjCustomer.Id == 3)//Parvej added for bank already exist
                {
                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_SUCCESS);
                    response.ObjData = _ObjCustomer;
                    response.ResponseCode = 0;
                    validateJsonData = new { response = false, responseCode = "02", data = _ObjCustomer.Message };


                }
                return new JsonResult(validateJsonData);
            }
            catch (Exception ex)
            {
                string Activity = "Api updatebeneficiarybank: " + ex.ToString() + " ";
                validateJsonData = new { response = false, responseCode = "01", data = "Invalid Request" };
                CompanyInfo.InsertErrorLogTracker(Activity.ToString(), 0, 0, 0, 0, "updatebeneficiarybank", Convert.ToInt32(obj1.Branch_ID), Convert.ToInt32(obj1.Client_ID), "", context);
                return new JsonResult(validateJsonData);

            }
        }

        [HttpPost]
        [Route("gettrasanctionstatus")]

        public IActionResult gettrasanctionstatus([FromBody] JsonElement obj)
        {
            HttpContext context = HttpContext;
            string json = System.Text.Json.JsonSerializer.Serialize(obj);
            dynamic data = JObject.Parse(json); string Status = string.Empty;
            var claimsIdentity = User.Identity as ClaimsIdentity;//sanket
            var authHeader = "";
            try { authHeader = HttpContext.Request.Headers["Authorization"].ToString(); } catch (Exception egx) { authHeader = ""; }

            bool auth = AuthController.checkAuth(claimsIdentity, obj, Convert.ToString(authHeader));
            if (!auth)
            {
                var errorResponse = new { response = false, responseCode = "403", data = "Access Forbidden" };
                return new JsonResult(errorResponse) { StatusCode = (int)HttpStatusCode.Forbidden };
            }
            CompanyInfo.InsertrequestLogTracker("gettrasanctionstatus full request body: " + JObject.Parse(json), 0, 0, 0, 0, "gettrasanctionstatus", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
            Model.response.WebResponse response = null;


            if (!SqlInjectionProtector.ReadJsonElementValues(obj) || !SqlInjectionProtector.ReadJsonElementValuesScript(obj))
            {
                var errorResponse = new { response = false, responseCode = "02", data = "Invalid input detected." };
                return new JsonResult(errorResponse) { StatusCode = 400 };
            }

            Beneficiary obj1 = JsonConvert.DeserializeObject<Beneficiary>(json);
            var validateJsonData = (dynamic)null;


            int number;
            if (!int.TryParse(Convert.ToString(data.Client_ID), out number))
            {
                validateJsonData = new { response = false, responseCode = "02", data = "Invalid Client ID." };
                return new JsonResult(validateJsonData);
            }
            if (!int.TryParse(Convert.ToString(data.Branch_ID), out number))
            {
                validateJsonData = new { response = false, responseCode = "02", data = "Invalid branchID field." };
                return new JsonResult(validateJsonData);
            }
            if (data.Beneficiary_ID == "" || data.Beneficiary_ID == null)
            {
                validateJsonData = new { response = false, responseCode = "02", data = "Invalid Beneficiary ID." };
                return new JsonResult(validateJsonData);
            }
            try
            {
                Service.srvBeneficiary srv = new Service.srvBeneficiary(context);
                DataTable li1 = srv.GetTxnStatus(obj1, context);


                if (li1 != null && li1.Rows.Count > 0)
                {
                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_SUCCESS);
                    response.data = "success";
                    response.ObjData = li1;
                    response.ResponseCode = 0;

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
                    validateJsonData = new { response = false, responseCode = "02", data = "No Records Found." };
                    return new JsonResult(validateJsonData);
                }
            }
            catch (Exception ex)
            {
                string Activity = "Api gettrasanctionstatus: " + ex.ToString() + " ";
                validateJsonData = new { response = false, responseCode = "01", data = "Invalid Request" };
                CompanyInfo.InsertErrorLogTracker(Activity.ToString(), 0, 0, 0, 0, "gettrasanctionstatus", Convert.ToInt32(obj1.Branch_ID), Convert.ToInt32(obj1.Client_ID), "", context);
                return new JsonResult(validateJsonData);
            }

        }


        [ApiExplorerSettings(IgnoreApi = true)]
        public string valideBeneficiaryAccNo(Beneficiary obj, string fieldname)
        {
            string required = "F";
            try
            {
                using (MySqlConnector.MySqlConnection con = new MySqlConnector.MySqlConnection(_configuration.GetConnectionString("connectionString")))
                {
                    con.Open();
                    MySqlConnector.MySqlCommand cmd_update1 = new MySqlConnector.MySqlCommand("SP_Beneficiary_Configuration");
                    cmd_update1.CommandType = CommandType.StoredProcedure;
                    cmd_update1.Connection = con;
                    cmd_update1.Parameters.AddWithValue("_Country_ID", obj.Beneficiary_Country_ID);
                    cmd_update1.Parameters.AddWithValue("_Collection_Type", obj.Collection_type_Id);
                    db_connection.ExecuteNonQueryProcedure(cmd_update1);
                    DataTable dtbenfconfig = db_connection.ExecuteQueryDataTableProcedure(cmd_update1);

                    if (Convert.ToString(dtbenfconfig.Rows[0]["ConfirmAccNo_man"]) == "0" || Convert.ToString(obj.Confirm_Account_Number).Trim() != "")
                    {
                        if (Convert.ToString(dtbenfconfig.Rows[0]["ConfirmAccNo_vis"]) == "0" && obj.Account_Number != obj.Confirm_Account_Number)
                        {
                            required = "T";
                        }
                    }
                }
            }
            catch (Exception egx) { }
            return required;
        }


        [ApiExplorerSettings(IgnoreApi = true)]
        public string valideBeneficiaryBankData(Beneficiary obj, string fieldName)
        {
            string required = "F";

            using (MySqlConnector.MySqlConnection con = new MySqlConnector.MySqlConnection(_configuration.GetConnectionString("connectionString")))
            {
                con.Open();
                MySqlConnector.MySqlCommand cmd_update1 = new MySqlConnector.MySqlCommand("SP_Beneficiary_Configuration");
                cmd_update1.CommandType = CommandType.StoredProcedure;
                cmd_update1.Connection = con;
                cmd_update1.Parameters.AddWithValue("_Country_ID", obj.Beneficiary_Country_ID);
                cmd_update1.Parameters.AddWithValue("_Collection_Type", obj.Collection_type_Id);
                db_connection.ExecuteNonQueryProcedure(cmd_update1);
                DataTable dtbenfconfig = db_connection.ExecuteQueryDataTableProcedure(cmd_update1);

                if (fieldName == "Verify_Account_no" && Convert.ToString(dtbenfconfig.Rows[0]["Verify_Account_no"]) == "0" && Convert.ToString(obj.Verify_Account_no).Trim() == "")
                {
                    required = "T";
                }
                try
                {
                    if (fieldName == "ConfirmAccNo_man" && Convert.ToString(dtbenfconfig.Rows[0]["ConfirmAccNo_man"]) == "0" && Convert.ToString(obj.Confirm_Account_Number).Trim() == "")
                    {
                        required = "T";
                    }
                }
                catch (Exception eg) {  }

            }

                return required;
        }

        [HttpPost]
        [Authorize]
        [Route("checksanctionconfigbeneficiary")]
        public IActionResult checksanctionconfigbeneficiary([FromBody] JsonElement obj)
        {
            HttpContext context = HttpContext;
            string json = System.Text.Json.JsonSerializer.Serialize(obj);
            var validateJsonData = (dynamic)null;
            dynamic data = JObject.Parse(json);
            var claimsIdentity = User.Identity as ClaimsIdentity;//sanket
            var authHeader = "";
            try { authHeader = HttpContext.Request.Headers["Authorization"].ToString(); } catch (Exception egx) { authHeader = ""; }

            bool auth = AuthController.checkAuth(claimsIdentity, obj, Convert.ToString(authHeader));
            if (!auth)
            {
                var errorResponse = new { response = false, responseCode = "403", data = "Access Forbidden" };
                return new JsonResult(errorResponse) { StatusCode = (int)HttpStatusCode.Forbidden };
            }
            CompanyInfo.InsertrequestLogTracker("checksanctionconfigbeneficiary full request body: " + JObject.Parse(json), 0, 0, 0, 0, "checksanctionconfigbeneficiary", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
            Beneficiary obj1 = new Beneficiary();
            Model.response.WebResponse response = null;
            DataTable dt = new DataTable();
            try
            {
                if (!SqlInjectionProtector.ReadJsonElementValues(obj) || !SqlInjectionProtector.ReadJsonElementValuesScript(obj))
                {
                    var errorResponse = new { response = false, responseCode = "02", data = "Invalid input detected." };
                    return new JsonResult(errorResponse) { StatusCode = 400 };
                }

                if (!int.TryParse(Convert.ToString(data.clientID), out int number))
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "Invalid Client ID." };
                    return new JsonResult(validateJsonData);
                }
                if (!int.TryParse(Convert.ToString(data.branchID), out int number2))
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "Invalid Branch ID." };
                    return new JsonResult(validateJsonData);
                }
                if (data.customerID == "" || data.customerID == null)
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "Invalid Customer ID." };
                    return new JsonResult(validateJsonData);
                }

                Model.Beneficiary _ObjCustomer = new Model.Beneficiary();
                Service.srvBeneficiary srvBenf = new Service.srvBeneficiary(HttpContext);
                _ObjCustomer.Client_ID = data.clientID;
                _ObjCustomer.Customer_ID = data.customerID;
                _ObjCustomer.Branch_ID = data.branchID;

                dt = srvBenf.Check_sanction_config_beneficiary(_ObjCustomer, context);

                if (dt != null && dt.Rows.Count > 0)
                {
                    List<Dictionary<string, object>> beneficiary = new List<Dictionary<string, object>>();

                    foreach (DataRow dr in dt.Rows)
                    {
                        Dictionary<string, object> beneficiary_details = new Dictionary<string, object>();
                        beneficiary_details["status"] = dr["Status"];
                        beneficiary_details["showalert"] = dr["showalert"];

                        beneficiary.Add(beneficiary_details);
                    }

                    validateJsonData = new { response = true, responseCode = "00", data = beneficiary };
                    return new JsonResult(validateJsonData);
                }
                else
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "No Records Foound." };
                    return new JsonResult(validateJsonData);
                }
               
            }
            catch (Exception ex)
            {
                string Activity = "Api:- checksanctionconfigbeneficiary" + ex.ToString();
                validateJsonData = new { response = false, responseCode = "02", data = "Invalid Response" };
                CompanyInfo.InsertErrorLogTracker(Activity.ToString(), 0, 0, 0, 0, "checksanctionconfigbeneficiary", Convert.ToInt32(obj1.Branch_ID), Convert.ToInt32(obj1.Client_ID), "", context);
                return new JsonResult(validateJsonData);
            }
        }


        [HttpPost]
        [Authorize]
        [Route("checksanctionconfigcustomer")]
        public IActionResult checksanctionconfigcustomer([FromBody] JsonElement obj)
        {
            HttpContext context = HttpContext;
            string json = System.Text.Json.JsonSerializer.Serialize(obj);
            var validateJsonData = (dynamic)null;
            dynamic data = JObject.Parse(json);
            var claimsIdentity = User.Identity as ClaimsIdentity;//sanket
            var authHeader = "";
            try { authHeader = HttpContext.Request.Headers["Authorization"].ToString(); } catch (Exception egx) { authHeader = ""; }

            bool auth = AuthController.checkAuth(claimsIdentity, obj, Convert.ToString(authHeader));
            if (!auth)
            {
                var errorResponse = new { response = false, responseCode = "403", data = "Access Forbidden" };
                return new JsonResult(errorResponse) { StatusCode = (int)HttpStatusCode.Forbidden };
            }
            CompanyInfo.InsertrequestLogTracker("checksanctionconfigcustomer full request body: " + JObject.Parse(json), 0, 0, 0, 0, "checksanctionconfigcustomer", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
            Beneficiary obj1 = new Beneficiary();
            Model.response.WebResponse response = null;
            DataTable dt = new DataTable();
            try
            {
                if (!SqlInjectionProtector.ReadJsonElementValues(obj) || !SqlInjectionProtector.ReadJsonElementValuesScript(obj))
                {
                    var errorResponse = new { response = false, responseCode = "02", data = "Invalid input detected." };
                    return new JsonResult(errorResponse) { StatusCode = 400 };
                }

                if (!int.TryParse(Convert.ToString(data.clientID), out int number))
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "Invalid Client ID." };
                    return new JsonResult(validateJsonData);
                }
                if (!int.TryParse(Convert.ToString(data.branchID), out int number2))
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "Invalid Branch ID." };
                    return new JsonResult(validateJsonData);
                }                 
                if (data.customerID == "" || data.customerID == null)
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "Invalid Customer ID." };
                    return new JsonResult(validateJsonData);
                }

                Model.Beneficiary _ObjCustomer = new Model.Beneficiary();
                Service.srvBeneficiary srvBenf = new Service.srvBeneficiary(HttpContext);
                _ObjCustomer.Client_ID = data.clientID;
                _ObjCustomer.Customer_ID = data.customerID;
                _ObjCustomer.Branch_ID = data.branchID;

                dt = srvBenf.Check_sanction_config_customer(_ObjCustomer, context);

                if (dt != null && dt.Rows.Count > 0)
                {
                    List<Dictionary<string, object>> beneficiary = new List<Dictionary<string, object>>();

                    foreach (DataRow dr in dt.Rows)
                    {
                        Dictionary<string, object> beneficiary_details = new Dictionary<string, object>();
                        beneficiary_details["status"] = dr["Status"];
                        beneficiary_details["showalert"] = dr["showalert"];
                               
                        beneficiary.Add(beneficiary_details);
                    }

                    validateJsonData = new { response = true, responseCode = "00", data = beneficiary };
                    return new JsonResult(validateJsonData);
                }
                else
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "No Records Foound." };
                    return new JsonResult(validateJsonData);
                }


                return new JsonResult(dt);
            }
            catch (Exception ex)
            {
                string Activity = "Api:- checksanctionconfigcustomer" + ex.ToString();
                validateJsonData = new { response = false, responseCode = "02", data = "Invalid Response" };
                CompanyInfo.InsertErrorLogTracker(Activity.ToString(), 0, 0, 0, 0, "checksanctionconfigcustomer", Convert.ToInt32(obj1.Branch_ID), Convert.ToInt32(obj1.Client_ID), "", context);
                return new JsonResult(validateJsonData);
            }
        }

        [HttpPost]        
        [Route("check_rekyc_customer")]
        public IActionResult check_rekyc_customer([FromBody] JsonElement objn)
        {
            HttpContext context = HttpContext;
            string json = System.Text.Json.JsonSerializer.Serialize(objn);
            var validateJsonData = (dynamic)null;
            dynamic data = JObject.Parse(json);
            var claimsIdentity = User.Identity as ClaimsIdentity;//sanket
            var authHeader = "";
            try { authHeader = HttpContext.Request.Headers["Authorization"].ToString(); } catch (Exception egx) { authHeader = ""; }

            bool auth = AuthController.checkAuth(claimsIdentity, objn, Convert.ToString(authHeader));
            if (!auth)
            {
                var errorResponse = new { response = false, responseCode = "403", data = "Access Forbidden" };
                return new JsonResult(errorResponse) { StatusCode = (int)HttpStatusCode.Forbidden };
            }
            CompanyInfo.InsertrequestLogTracker("check_rekyc_customer full request body: " + JObject.Parse(json), 0, 0, 0, 0, "check_rekyc_customer", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
            Beneficiary obj1 = new Beneficiary();
            Model.response.WebResponse response = null;
            DataTable dt = new DataTable();
            try
            {
                if (!SqlInjectionProtector.ReadJsonElementValues(objn) || !SqlInjectionProtector.ReadJsonElementValuesScript(objn))
                {
                    var errorResponse = new { response = false, responseCode = "02", data = "Invalid input detected." };
                    return new JsonResult(errorResponse) { StatusCode = 400 };
                }

                Model.Beneficiary obj = new Model.Beneficiary();

                obj.Client_ID = data.clientID;
                obj.Customer_ID = data.customerID;
                obj.Branch_ID = data.branchID;


                srvBeneficiary srv3 = new srvBeneficiary(context);
                DataTable li1 = srv3.check_Rekyc_customer(obj, context);
                Model.Dashboard _Obj = new Model.Dashboard();
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
                    response.ResponseMessage = "Something went wrong. Please try again later.";
                    response.ResponseCode = 1;
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
                response.ResponseMessage = "technical error";
                response.ResponseCode = 3;
                //Error Log Handled
                Model.ErrorLog objError = new Model.ErrorLog();
                objError.User = new Model.User();
                objError.Error = "Api : RecipientController --" + ex.ToString();
                objError.Date = DateTime.Now;
                objError.User_ID = 1;
                objError.Client_ID = obj.Client_ID;
                objError.Function_Name = "check_Rekyc_customer";
                Service.srvErrorLog srvError = new Service.srvErrorLog();
                srvError.Create(objError,context);
                validateJsonData = new { response = false, responseCode = "01", data = "Invalid Response" };
                return new JsonResult(validateJsonData);

            }

        }


        [ApiExplorerSettings(IgnoreApi = true)]
        public string valideBeneficiaryData(Beneficiary obj, string fieldName)
        {
            string required = "F";
            using (MySqlConnector.MySqlConnection con = new MySqlConnector.MySqlConnection(_configuration.GetConnectionString("connectionString")))
            {
                con.Open();
                MySqlConnector.MySqlCommand cmd_update1 = new MySqlConnector.MySqlCommand("SP_Beneficiary_Configuration");
                cmd_update1.CommandType = CommandType.StoredProcedure;                               
                cmd_update1.Connection = con;
                cmd_update1.Parameters.AddWithValue("_Country_ID", obj.Beneficiary_Country_ID);
                cmd_update1.Parameters.AddWithValue("_Collection_Type", obj.Collection_type_Id);
                db_connection.ExecuteNonQueryProcedure(cmd_update1);
                DataTable dtbenfconfig = db_connection.ExecuteQueryDataTableProcedure(cmd_update1);
                 

                if(fieldName == "Beneficiary_Name" && Convert.ToString(dtbenfconfig.Rows[0]["Beneficiary_Name"]) == "0" && Convert.ToString(obj.Beneficiary_Name).Trim() == "")
                {
                    required = "T";
                }
                else if (fieldName == "Beneficiary_Address" && Convert.ToString(dtbenfconfig.Rows[0]["Beneficiary_Address"]) == "0" && Convert.ToString(obj.Beneficiary_Address).Trim() == "")
                {
                    required = "T";
                }
                else if (fieldName == "Beneficiary_relation" && Convert.ToString(dtbenfconfig.Rows[0]["Beneficiary_relation"]) == "0" && Convert.ToString(obj.Relation_ID).Trim() == "0")
                {
                    required = "T";
                }
                else if (fieldName == "Beneficiary_Mobile" && Convert.ToString(dtbenfconfig.Rows[0]["Beneficiary_Mobile"]) == "0" && Convert.ToString(obj.Beneficiary_Mobile).Trim() == "")
                {
                    required = "T";
                }                
                else if (fieldName == "Beneficiary_City" && Convert.ToString(dtbenfconfig.Rows[0]["Beneficiary_City"]) == "0" && Convert.ToString(obj.Beneficiary_City_ID).Trim() == "0")
                {
                    required = "T";
                }
                else if (fieldName == "mobile_provider" && Convert.ToString(dtbenfconfig.Rows[0]["mobile_provider"]) == "0" && Convert.ToString(obj.Mobile_provider).Trim() == "")
                {
                    required = "T";
                }

            }

            return required;
        }

        [HttpPost]
        [Authorize]
        [Route("savebeneficiarymedicare")]
        public IActionResult SaveBeneficiaryMedicare([FromBody] JsonElement obj1)
        {
            HttpContext context = HttpContext;
            string json = System.Text.Json.JsonSerializer.Serialize(obj1);
            var claimsIdentity = User.Identity as ClaimsIdentity;//sanket
            var authHeader = "";
            try { authHeader = HttpContext.Request.Headers["Authorization"].ToString(); } catch (Exception egx) { authHeader = ""; }

            bool auth = AuthController.checkAuth(claimsIdentity, obj1, Convert.ToString(authHeader));
            if (!auth)
            {
                var errorResponse = new { response = false, responseCode = "403", data = "Access Forbidden" };
                return new JsonResult(errorResponse) { StatusCode = (int)HttpStatusCode.Forbidden };
            }
            CompanyInfo.InsertrequestLogTracker("SaveBeneficiaryMedicare full request body: " + JObject.Parse(json), 0, 0, 0, 0, "savebeneficiarymedicare", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
            dynamic data = JObject.Parse(json); string Status = string.Empty;
            Model.response.WebResponse response = null;

            Beneficiary obj = new Beneficiary();
            var validateJsonData = (dynamic)null;
            // var cm = li.SingleOrDefault();

            int number;
            if (!int.TryParse(Convert.ToString(data.Client_ID), out number))
            {
                validateJsonData = new { response = false, responseCode = "02", data = "Invalid ClientID." };
                return new JsonResult(validateJsonData);
            }
            if (!int.TryParse(Convert.ToString(data.Branch_ID), out number))
            {
                validateJsonData = new { response = false, responseCode = "02", data = "Invalid branchID." };
                return new JsonResult(validateJsonData);
            }
            if (data.Beneficiary_ID == "" || data.Beneficiary_ID == null)
            {
                validateJsonData = new { response = false, responseCode = "02", data = "Invalid BeneficiaryID." };
                return new JsonResult(validateJsonData);
            }
            if (data.Customer_ID == "" || data.Customer_ID == null)
            {
                validateJsonData = new { response = false, responseCode = "02", data = "Invalid CustomerID." };
                return new JsonResult(validateJsonData);
            }

            try
            {
                obj = JsonConvert.DeserializeObject<Beneficiary>(json);
                Model.Beneficiary _ObjBenef = new Model.Beneficiary();
                Service.srvBeneficiary srvBenf = new Service.srvBeneficiary(HttpContext);

                _ObjBenef = srvBenf.CreateMedicare(obj, context);

                string benef = "Beneficiary";
                if (obj.Beneficiary_ID == 0)
                {
                    benef = "Customer";
                }
                List<Dictionary<string, object>> customerData = new List<Dictionary<string, object>>();
                Dictionary<string, object> customer = new Dictionary<string, object>();
                if (_ObjBenef.Message == "success")
                {
                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_SUCCESS);
                    response.data = "success";
                    response.ResponseCode = 0;

                    customer["status"] = "success";
                    customer["message"] = benef + " added successfully to KMoney Health Care.";
                    customerData.Add(customer);

                    validateJsonData = new { response = true, responseCode = "00", data = customerData };
                    return new JsonResult(validateJsonData);
                }
                else if (_ObjBenef.Message == "Validation Failed")
                {
                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_TECHNICAL_ERROR);
                    response.data = "Validation Failed";
                    response.ObjData = (_ObjBenef.Message).ToString();
                    response.ResponseCode = 1;

                    customer["status"] = "failed";
                    customer["message"] = _ObjBenef.Message;
                }
                else if (_ObjBenef.Message == "limit_exceed")
                {
                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_SUCCESS);
                    response.data = "limit_exceed";
                    response.ObjData = (_ObjBenef.Message).ToString();
                    response.ResponseCode = 2;
                    customer["status"] = "failed";
                    customer["message"] = "KMoney Health Care " + benef + " Limit Exceeded";
                }
                else if (_ObjBenef.Message == "no_enough_points")
                {
                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_SUCCESS);
                    response.data = "no_enough_points";
                    response.ObjData = (_ObjBenef.Message).ToString();
                    response.ResponseCode = 4;
                    customer["status"] = "failed";
                    customer["message"] = "No Enough Points in Wallet";
                }
                else if (_ObjBenef.Message == "wallet_inactive")
                {
                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_SUCCESS);
                    response.data = "wallet_inactive";
                    response.ObjData = (_ObjBenef.Message).ToString();
                    response.ResponseCode = 9;
                    customer["status"] = "failed";
                    customer["message"] = "Your Points Wallet is inactive. Please make a transfer to activate your wallet and redeem the benefits.";
                }
                else if (_ObjBenef.Message == "Failed")
                {
                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_SUCCESS);
                    response.data = "Failed";
                    response.ObjData = (_ObjBenef.Message).ToString();
                    response.ResponseCode = 7;
                    customer["status"] = "failed";
                    customer["message"] = "Failed to add " + benef + " to KMoney Health Care";
                }
                else if (_ObjBenef.Message == "already_exist")
                {
                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_SUCCESS);
                    response.data = "already_exist";
                    response.ObjData = (_ObjBenef.Message).ToString();
                    response.ResponseCode = 8;
                    customer["status"] = "failed";
                    customer["message"] = benef + " already exists in KMoney Health Care";
                }
                else if (_ObjBenef.Message == "duplicate")
                {
                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_SUCCESS);
                    response.data = "duplicate";
                    response.ObjData = (_ObjBenef.Message).ToString();
                    response.ResponseCode = 10;
                    customer["status"] = "failed";
                    customer["message"] = benef + " with this Mobile number already exists in KMoney Health Care.";
                }
                else
                {
                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_SUCCESS);
                    response.data = "duplicate";
                    response.ObjData = (_ObjBenef.Message).ToString();
                    response.ResponseCode = 3;
                    customer["status"] = "failed";
                    customer["message"] = "Oop's.. Error occured";
                }
                customerData.Add(customer);
                validateJsonData = new { response = false, responseCode = "02", data = customerData };
                return new JsonResult(validateJsonData);
            }
            catch (Exception ex)
            {
                string Activity = "Api SaveBeneficiaryMedicare: " + ex.ToString() + " ";
                validateJsonData = new { response = false, responseCode = "01", data = "Invalid Request" };
                CompanyInfo.InsertErrorLogTracker(Activity.ToString(), 0, 0, 0, 0, "SaveBeneficiaryMedicare", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "", context);
                return new JsonResult(validateJsonData);
            }

        }

        [HttpPost]
        [Authorize]
        [Route("removebeneficiarymedicare")]
        public IActionResult RemoveBeneficiaryMedicare([FromBody] JsonElement obj1)
        {
            HttpContext context = HttpContext;
            string json = System.Text.Json.JsonSerializer.Serialize(obj1);
            var claimsIdentity = User.Identity as ClaimsIdentity;//sanket
            var authHeader = "";
            try { authHeader = HttpContext.Request.Headers["Authorization"].ToString(); } catch (Exception egx) { authHeader = ""; }

            bool auth = AuthController.checkAuth(claimsIdentity, obj1, Convert.ToString(authHeader));
            if (!auth)
            {
                var errorResponse = new { response = false, responseCode = "403", data = "Access Forbidden" };
                return new JsonResult(errorResponse) { StatusCode = (int)HttpStatusCode.Forbidden };
            }
            CompanyInfo.InsertrequestLogTracker("SaveBeneficiaryMedicare full request body: " + JObject.Parse(json), 0, 0, 0, 0, "savebeneficiarymedicare", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
            dynamic data = JObject.Parse(json); string Status = string.Empty;
            Model.response.WebResponse response = null;

            Beneficiary obj = new Beneficiary();
            var validateJsonData = (dynamic)null;
            // var cm = li.SingleOrDefault();

            int number;
            if (!int.TryParse(Convert.ToString(data.Client_ID), out number))
            {
                validateJsonData = new { response = false, responseCode = "02", data = "Invalid ClientID." };
                return new JsonResult(validateJsonData);
            }
            if (!int.TryParse(Convert.ToString(data.Branch_ID), out number))
            {
                validateJsonData = new { response = false, responseCode = "02", data = "Invalid branchID." };
                return new JsonResult(validateJsonData);
            }
            if (data.Beneficiary_ID == "" || data.Beneficiary_ID == null)
            {
                validateJsonData = new { response = false, responseCode = "02", data = "Invalid BeneficiaryID." };
                return new JsonResult(validateJsonData);
            }
            if (data.Customer_ID == "" || data.Customer_ID == null)
            {
                validateJsonData = new { response = false, responseCode = "02", data = "Invalid CustomerID." };
                return new JsonResult(validateJsonData);
            }

            try
            {
                obj = JsonConvert.DeserializeObject<Beneficiary>(json);
                Model.Beneficiary _ObjBenef = new Model.Beneficiary();
                Service.srvBeneficiary srvBenf = new Service.srvBeneficiary(HttpContext);

                _ObjBenef = srvBenf.RemoveMedicare(obj, context);

                string benef = "Beneficiary";
                //if (obj.Beneficiary_ID == 0)
                //{
                //    benef = "Customer";
                //}
                List<Dictionary<string, object>> customerData = new List<Dictionary<string, object>>();
                Dictionary<string, object> customer = new Dictionary<string, object>();
                if (_ObjBenef.Message == "success")
                {
                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_SUCCESS);
                    response.data = "success";
                    response.ResponseCode = 0;

                    customer["status"] = "success";
                    customer["message"] = benef + " KMoney Health Care removed successfully";
                    customerData.Add(customer);

                    validateJsonData = new { response = true, responseCode = "00", data = customerData };
                    return new JsonResult(validateJsonData);
                }
                else if (_ObjBenef.Message == "failed")
                {
                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_SUCCESS);
                    response.data = "Failed";
                    response.ObjData = (_ObjBenef.Message).ToString();
                    response.ResponseCode = 2;
                    customer["status"] = "failed";
                    customer["message"] = "Failed to remove KMoney Health Care for " + benef;
                }
                else
                {
                    response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_SUCCESS);
                    response.data = "duplicate";
                    response.ObjData = (_ObjBenef.Message).ToString();
                    response.ResponseCode = 3;
                    customer["status"] = "failed";
                    customer["message"] = "Oop's.. Error occured";
                }
                customerData.Add(customer);
                validateJsonData = new { response = false, responseCode = "02", data = customerData };
                return new JsonResult(validateJsonData);
            }
            catch (Exception ex)
            {
                string Activity = "Api SaveBeneficiaryMedicare: " + ex.ToString() + " ";
                validateJsonData = new { response = false, responseCode = "01", data = "Invalid Request" };
                CompanyInfo.InsertErrorLogTracker(Activity.ToString(), 0, 0, 0, 0, "SaveBeneficiaryMedicare", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "", context);
                return new JsonResult(validateJsonData);
            }

        }


        [HttpPost]
        [Authorize]
        [Route("benfBankInfo")]
        public IActionResult BenfBankInfo([FromBody] JsonElement obj1)
        {
            HttpContext context = HttpContext;
            string json = System.Text.Json.JsonSerializer.Serialize(obj1);
            var claimsIdentity = User.Identity as ClaimsIdentity;//sanket
            var authHeader = "";
            try { authHeader = HttpContext.Request.Headers["Authorization"].ToString(); } catch (Exception egx) { authHeader = ""; }

            bool auth = AuthController.checkAuth(claimsIdentity, obj1, Convert.ToString(authHeader));
            if (!auth)
            {
                var errorResponse = new { response = false, responseCode = "403", data = "Access Forbidden" };
                return new JsonResult(errorResponse) { StatusCode = (int)HttpStatusCode.Forbidden };
            }
            CompanyInfo.InsertrequestLogTracker("BenfBankInfo full request body: " + JObject.Parse(json), 0, 0, 0, 0, "BenfBankInfo", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
            dynamic data = JObject.Parse(json); string Status = string.Empty;
            Model.response.WebResponse response = null;

            Beneficiary obj = new Beneficiary();
            var validateJsonData = (dynamic)null;
            // var cm = li.SingleOrDefault();

            int number;
            if (!int.TryParse(Convert.ToString(data.Client_ID), out number))
            {
                validateJsonData = new { response = false, responseCode = "02", data = "Invalid ClientID." };
                return new JsonResult(validateJsonData);
            }
            if (data.Beneficiary_ID == "" || data.Beneficiary_ID == null)
            {
                validateJsonData = new { response = false, responseCode = "02", data = "Invalid BeneficiaryID." };
                return new JsonResult(validateJsonData);
            }
            if (data.Customer_ID == "" || data.Customer_ID == null)
            {
                validateJsonData = new { response = false, responseCode = "02", data = "Invalid CustomerID." };
                return new JsonResult(validateJsonData);
            }

            try
            {
                obj = JsonConvert.DeserializeObject<Beneficiary>(json);
                Service.srvBank srv = new Service.srvBank();
                response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_SUCCESS);
                DataTable li1 = srv.BenfBankInfo(obj , context);

                if(li1.Rows.Count != 0 && li1 != null)
                {
                    var relationshipData = li1.Rows.OfType<DataRow>()
                    .Select(row =>
                    {
                        var dict = new Dictionary<string, string>();

                        foreach (DataColumn col in li1.Columns)
                        {
                            var value = row[col];
                            if (col.ColumnName == "Customer_ID")
                            {
                                dict[col.ColumnName] = CompanyInfo.Encrypt(Convert.ToString(value), true);
                            }
                            else
                            {
                                dict[col.ColumnName] = Convert.ToString(value);
                            }
                        }

                        return dict;
                    });


                    validateJsonData = new { response = true, responseCode = "00", data = relationshipData };
                    return new JsonResult(validateJsonData);

                }
                else
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "No Records found." };
                    return new JsonResult(validateJsonData);
                }


                
                
            }
            catch (Exception ex)
            {
                string Activity = "Api BenfBankInfo: " + ex.ToString() + " ";
                validateJsonData = new { response = false, responseCode = "01", data = "Invalid Request" };
                CompanyInfo.InsertErrorLogTracker(Activity.ToString(), 0, 0, 0, 0, "BenfBankInfo", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "", context);
                return new JsonResult(validateJsonData);
            }

            return new JsonResult(validateJsonData);

        }


        [HttpPost]
        [Authorize]
        [Route("updatebankstatus")]
        public IActionResult UpdateBankStatus([FromBody] JsonElement Obj) 
        {
            HttpContext context = HttpContext;

            string json = System.Text.Json.JsonSerializer.Serialize(Obj);
            _ = Task.Run(() => CompanyInfo.InsertrequestLogTracker("updatebankstatus full request body: " + JObject.Parse(json), 0, 0, 0, 0, "savebeneficiarymedicare", Convert.ToInt32(0), Convert.ToInt32(0), "", context));
            Model.response.WebResponse response = null;
            Beneficiary obj = new Beneficiary();
            var validateJsonData = (dynamic)null;
            try
            {
                obj = JsonConvert.DeserializeObject<Beneficiary>(json);
                Service.srvCommon srv = new Service.srvCommon();
                var result = srv.UpdateBankStatus(obj, context);

                //Activity Log
                Model.ActivityLogTracker _ActivityObj = new Model.ActivityLogTracker();
                _ActivityObj.User = new Model.User();
                _ActivityObj.Customer = new Model.Customer();
                _ActivityObj.Branch = new Model.Branch();
                _ActivityObj.Client = new Model.Client();
                _ActivityObj.Client.Id = 1;
                //   _ActivityObj.Employee = new Model.Employee();
                _ActivityObj.User.Id = 1;
                _ActivityObj.FunctionName = "CommonApiController -> UpdateBankStatus";
                if (result == "success")
                {
                    _ActivityObj.Activity = "Updated Bank status for the " + obj.Sending_Flag + ": <b>" + obj.Sending_Flag + " </b></br>";
                }
                else
                {
                    _ActivityObj.Activity = "Failed to update Bank status for the " + obj.Sending_Flag + ": <b>" + obj.Sending_Flag + " </b></br>";
                }
                validateJsonData = new { response = true, responseCode = "00", data = result };

            }
            catch (Exception ex)
            {
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
                srvError.Create(objError, context);

                validateJsonData = new { response = false, responseCode = "02", data = "Invalid Request" };
            }
            return new JsonResult(validateJsonData);
        }



        [HttpPost]
        [Authorize]
        [Route("getInfo")]

        public IActionResult GetInfo([FromBody] JsonElement Obj)
        {
            HttpContext context = HttpContext;
            string json = System.Text.Json.JsonSerializer.Serialize(Obj);
            var claimsIdentity = User.Identity as ClaimsIdentity;//sanket
            var authHeader = "";
            try { authHeader = HttpContext.Request.Headers["Authorization"].ToString(); } catch (Exception egx) { authHeader = ""; }

            bool auth = AuthController.checkAuth(claimsIdentity, Obj, Convert.ToString(authHeader));
            if (!auth)
            {
                var errorResponse = new { response = false, responseCode = "403", data = "Access Forbidden" };
                return new JsonResult(errorResponse) { StatusCode = (int)HttpStatusCode.Forbidden };
            }
            CompanyInfo.InsertrequestLogTracker("BenfBankInfo full request body: " + JObject.Parse(json), 0, 0, 0, 0, "BenfBankInfo", Convert.ToInt32(0), Convert.ToInt32(0), "", HttpContext);
            dynamic data = JObject.Parse(json); string Status = string.Empty;
            Model.response.WebResponse response = null;

            Beneficiary obj = new Beneficiary();
            var validateJsonData = (dynamic)null;
            // var cm = li.SingleOrDefault();

            int number;
            if (!int.TryParse(Convert.ToString(data.Client_ID), out number))
            {
                validateJsonData = new { response = false, responseCode = "02", data = "Invalid ClientID." };
                return new JsonResult(validateJsonData);
            }
            if (data.Beneficiary_ID == "" || data.Beneficiary_ID == null)
            {
                validateJsonData = new { response = false, responseCode = "02", data = "Invalid BeneficiaryID." };
                return new JsonResult(validateJsonData);
            }
            if (data.Customer_ID == "" || data.Customer_ID == null)
            {
                validateJsonData = new { response = false, responseCode = "02", data = "Invalid CustomerID." };
                return new JsonResult(validateJsonData);
            }

            try
            {
                obj = JsonConvert.DeserializeObject<Beneficiary>(json);
                Service.srvBeneficiary srv = new Service.srvBeneficiary(HttpContext);
                response = new Model.response.WebResponse(Model.response.WebResponse.RESPONSE_STATUS_TYPE_SUCCESS);
                DataTable li1 = srv.GetInfo(obj, context);

                if (li1.Rows.Count != 0 && li1 != null)
                {
                    var relationshipData = li1.Rows.OfType<DataRow>()
              .Select(row => li1.Columns.OfType<DataColumn>()
                  .ToDictionary(col => col.ColumnName, c => row[c].ToString()));
                    validateJsonData = new { response = true, responseCode = "00", data = relationshipData };
                    return new JsonResult(validateJsonData);

                }
                else
                {
                    validateJsonData = new { response = false, responseCode = "02", data = "No Records found." };
                    return new JsonResult(validateJsonData);
                }




            }
            catch (Exception ex)
            {
                string Activity = "Api GetInfo: " + ex.ToString() + " ";
                validateJsonData = new { response = false, responseCode = "01", data = "Invalid Request" };
                CompanyInfo.InsertErrorLogTracker(Activity.ToString(), 0, 0, 0, 0, "GetInfo", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "", context);
                return new JsonResult(validateJsonData);
            }

            return new JsonResult(validateJsonData);


        }

    }


}
