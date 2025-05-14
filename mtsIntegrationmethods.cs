using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Net;
using Microsoft.Ajax.Utilities;


namespace Calyx_Solutions
{
    class mtsIntegrationmethods
    {

        public class MyZeepay
        {
            public string code { get; set; }
            public string zeepay_id { get; set; }
            public string amount { get; set; }
            public string message { get; set; }
            public string token_type { get; set; }
            public string expires_in { get; set; }
            public string access_token { get; set; }
        }
/*
        public static string getAccountHolderDetails(  DataTable dtt, string token, Model.Beneficiary obj , HttpContext context)
        {
            
            mtsIntegrationmethods obj2 = new mtsIntegrationmethods();
            string accHolderName = "";
            int api_id = 0;
            try
            {
                
                for (int i = 0; i < dtt.Rows.Count; i++)
                {
                     api_id = Convert.ToInt32(dtt.Rows[i]["ID"]);
                    //if (api_id != 14)
                    //{
                    //    continue;
                    //}

                    string apiurl = Convert.ToString(dtt.Rows[i]["API_URL"]);
                    string apiuser = Convert.ToString(dtt.Rows[i]["APIUser_ID"]);
                    string apipass = Convert.ToString(dtt.Rows[i]["Password"]);
                    string accesscode = Convert.ToString(dtt.Rows[i]["APIAccess_Code"]);
                    string bankCode = Convert.ToString(obj.BBank_ID);
                    string Beneficiary_Country_ID =  Convert.ToString(obj.Beneficiary_Country_ID);

                   MySqlConnector.MySqlCommand cmd = new MySqlConnector.MySqlCommand("SearchHolderName");
                    cmd.CommandType = CommandType.StoredProcedure;
                    string wherecondition = "   bank_id=" + bankCode + " and api_id = " + api_id;
                    cmd.Parameters.AddWithValue("_security_key", CompanyInfo.SecurityKey());
                    cmd.Parameters.AddWithValue("wherecondition", wherecondition);
                    DataTable dtofKMOBankCode = db_connection.ExecuteQueryDataTableProcedure(cmd);


                    if (api_id == 14)
                    {
                        try
                        {
                            string accNumber = Convert.ToString(obj.Account_Number).Trim();
                            //bankCode = Convert.ToString(dtofKMOBankCode.Rows[0]["bank_code"]);
                            string gettoken = "", getclientID = "";
                            string api_fields = Convert.ToString(dtt.Rows[i]["api_Fields"]);
                            if (api_fields != "" && api_fields != null)
                            {
                                Newtonsoft.Json.Linq.JObject objAPI = Newtonsoft.Json.Linq.JObject.Parse(api_fields);
                                gettoken = Convert.ToString(objAPI["gettoken"]);
                                getclientID = Convert.ToString(objAPI["clientid"]);
                            }

                            if (accNumber != null || accNumber == "")
                            {
                                string authbaseurl = gettoken;
                                string awsClientId = Convert.ToString(dtt.Rows[i]["APIUser_ID"]).Trim();
                                string awsSecret = Convert.ToString(dtt.Rows[i]["APIAccess_Code"]).Trim();
                                var plainTextBytes = Encoding.UTF8.GetBytes($"{awsClientId}:{awsSecret}");
                                string authorizationheader = Convert.ToBase64String(plainTextBytes);
                                var client = new RestClient(authbaseurl);
                                var request = new RestRequest(Method.POST);
                                request.AddHeader("Content-type", "application/x-www-form-urlencoded");
                                request.AddHeader("Authorization", $"Basic {authorizationheader}");
                                //  request.AddParameter("scope", "nairapayoutmerchant-resource-server/merchant");
                                request.Resource = "/oauth2/token";
                                request.AddParameter("client_id", awsClientId, ParameterType.GetOrPost);
                                request.AddParameter("grant_type", "client_credentials", ParameterType.GetOrPost);

                                var response = client.Execute(request);
                                if (response.StatusCode == System.Net.HttpStatusCode.OK || response.StatusCode == System.Net.HttpStatusCode.BadRequest || response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                                {
                                    CompanyInfo.InsertActivityLogDetails("Payceler Token Generated.", Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(obj.Customer_ID), "getAccountHolderDetails", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "Get Beneficiary Bank Account Details", context);
                                    Console.WriteLine(response.Content);
                                    kmoney json = Newtonsoft.Json.JsonConvert.DeserializeObject<kmoney>(response.Content);
                                    token = json.access_token;
                                }

                                dynamic Json = null;
                                response = null;
                                try
                                {
                                    client = new RestClient(Convert.ToString(dtt.Rows[i]["API_URL"]) + "/client/getAPIClientGatewayBalanceList/" + Convert.ToString(getclientID) + "");
                                    client.Timeout = -1;
                                    request = new RestRequest(Method.GET);
                                    request.AddHeader("Authorization", token);
                                    response = client.Execute(request);
                                    Json = Newtonsoft.Json.JsonConvert.DeserializeObject(response.Content);
                                    CompanyInfo.InsertActivityLogDetails("Payceler Balance and Gateway response: <br>:" + response.Content, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(obj.Customer_ID), "getAccountHolderDetails", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "Get Beneficiary Bank Account Details", context);
                                }
                                catch (Exception ex)
                                {
                                    CompanyInfo.InsertActivityLogDetails("Payceler Balance and Gateway error : <br>:" + ex.ToString().Replace("'", "\""), Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(obj.Customer_ID), "getAccountHolderDetails", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "Get Beneficiary Bank Account Details", context);
                                }


                                foreach (var gtwayNumber in Json)
                                {
                                    string gatewayId = Convert.ToString(gtwayNumber.gatewayId);
                                    string acccountValid = "F"; string bnkId = "";
                                    try
                                    {
                                        cmd = new MySqlConnector.MySqlCommand("SearchHolderName");
                                        cmd.CommandType = CommandType.StoredProcedure;
                                        wherecondition = "   bank_id=" + bankCode + " and api_id = " + api_id + " and bank_name like '%" + gatewayId + "%' ";
                                        cmd.Parameters.AddWithValue("_security_key", CompanyInfo.SecurityKey());
                                        cmd.Parameters.AddWithValue("wherecondition", wherecondition);
                                        DataTable dtofKMOBankCodePayceller = db_connection.ExecuteQueryDataTableProcedure(cmd);
                                        try
                                        {
                                            bnkId = Convert.ToString(dtofKMOBankCodePayceller.Rows[0]["bank_code"]);
                                        }
                                        catch (Exception ex)
                                        {
                                        }

                                        client = new RestClient(Convert.ToString(dtt.Rows[i]["API_URL"]));
                                        request = new RestRequest(Method.GET);
                                        request.AddHeader("Content-type", "application/json");
                                        request.AddHeader("Authorization", token);
                                        request.Resource = $"/client/nameEnquiry/" + gatewayId + "/" + Convert.ToString(bnkId).Trim() + "/" + Convert.ToString(accNumber).Trim();
                                        string urlR = "/client/nameEnquiry/" + gatewayId + "/" + Convert.ToString(bnkId).Trim() + "/" + Convert.ToString(accNumber).Trim();
                                        response = client.Execute(request);
                                        CompanyInfo.InsertActivityLogDetails("Payceler Validate Request : <br/>" + urlR, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(obj.Customer_ID), "getAccountHolderDetails", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "validateAccountNumber", context);
                                        if (response.StatusCode == System.Net.HttpStatusCode.OK)
                                        {
                                            Console.WriteLine("Success Is Ok");
                                            accHolderName = Convert.ToString(response.Content);
                                            accHolderName = accHolderName.Trim().Replace(@"\", "").Replace(@"""", "");
                                            acccountValid = "T"; break;
                                        }
                                        else
                                        {
                                            Console.WriteLine("Bad Request " + response.StatusDescription);
                                            acccountValid = "F";
                                        }
                                        Console.WriteLine(response.Content);
                                        CompanyInfo.InsertActivityLogDetails("App Payceler Validate Account response: <br/>" + response.Content, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(obj.Customer_ID), "getAccountHolderDetails", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "validateAccountNumber", context);
                                    }
                                    catch (Exception ex)
                                    {
                                        acccountValid = "F";
                                        CompanyInfo.InsertActivityLogDetails("App Payceler Validate Account error: <br/>" + ex.ToString().Replace("'", "\""), Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(obj.Customer_ID), "getAccountHolderDetails", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "validateAccountNumber", context);
                                    }
                                }
                            }
                            else
                            {
                                Console.Write("alert(Account Number or Account Holder Name is Empty)");
                            }
                        }
                        catch (Exception ex)
                        {
                            CompanyInfo.InsertActivityLogDetails("Error Get account details response <br>:" + api_id + " " + ex.ToString(), Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(obj.Customer_ID), "getAccountHolderDetails", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "Get Beneficiary Bank Account Details", context);
                        }
                    }
                    else if (api_id == 20)
                    {
                        bankCode = Convert.ToString(dtofKMOBankCode.Rows[0]["bank_code"]);
                        var client = new RestClient(apiurl + "account/lookup");
                        client.Timeout = -1;
                        var request = new RestRequest(Method.POST);
                        request.AddHeader("Accept", "application/json");
                        request.AddHeader("Authorization", "Bearer " + token);
                        request.AddHeader("Content-Type", "application/json");
                        var body = @"{
                        " + "\n" +
                        @"    ""bank"":""" + bankCode + @""",
                        " + "\n" +
                        @"    ""account"": """ + obj.Account_Number + @"""
                        " + "\n" +
                        @"}";
                        request.AddParameter("application/json", body, ParameterType.RequestBody);
                        //string activity = " Get Ac Details URL: " + Convert.ToString(dtt.Rows[i]["API_URL"]) + "/NameEnquiry/" + Convert.ToString(bankCode) + "/" + obj.Account_Number;
                        CompanyInfo.InsertActivityLogDetails("Get TierMoney account details request <br>:" + body, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(obj.Customer_ID), "getAccountHolderDetails TierMoney", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "Get Beneficiary Bank Account Details" , context);
                        IRestResponse response = client.Execute(request);
                        CompanyInfo.InsertActivityLogDetails("Get TierMoney account details response <br>:" + response.Content, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(obj.Customer_ID), "getAccountHolderDetails TierMoney", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "Get Beneficiary Bank Account Details" , context);
                        kmoney json = Newtonsoft.Json.JsonConvert.DeserializeObject<kmoney>(response.Content);
                        accHolderName = json.data.accountName;
                        //Console.WriteLine(response.Content);
                    }
                    else if (api_id == 23)//Kbt
                    {
                        bankCode = Convert.ToString(dtofKMOBankCode.Rows[0]["bank_code"]);
                        var client = new RestClient(apiurl + "getBeneficiaryAccountName/100102/NGR/" + bankCode + "/" + obj.Account_Number);
                        client.Timeout = -1;
                        var request = new RestRequest(Method.GET);
                        // CompanyInfo.InsertActivityLogDetails("Get KoboTrade account details request <br>:" + body, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(obj.Customer_ID), "getAccountHolderDetails Kobo", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "Get Beneficiary Bank Account Details");
                        IRestResponse response = client.Execute(request);
                        //CompanyInfo.InsertActivityLogDetails("Get KoboTrade account details response <br>:" + response.Content, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(obj.Customer_ID), "getAccountHolderDetails KObo", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "Get Beneficiary Bank Account Details");
                        dynamic json = Newtonsoft.Json.JsonConvert.DeserializeObject(response.Content);
                        accHolderName = json.data;
                    }
                    else if (api_id == 31)
                    {
                        try
                        {
                            string requestLink = "", bnkId = ""; string responseCode = "";
                            string api_fields = Convert.ToString(dtt.Rows[i]["api_Fields"]);
                            if (api_fields != "" && api_fields != null)
                            {
                                Newtonsoft.Json.Linq.JObject objAPI = Newtonsoft.Json.Linq.JObject.Parse(api_fields);
                                requestLink = Convert.ToString(objAPI["requestlink"]);
                            }

                            cmd = new MySqlConnector.MySqlCommand("SearchHolderName");
                            cmd.CommandType = CommandType.StoredProcedure;
                            wherecondition = "   bank_id=" + bankCode + " and api_id = " + api_id + "   ";
                            cmd.Parameters.AddWithValue("_security_key", CompanyInfo.SecurityKey());
                            cmd.Parameters.AddWithValue("wherecondition", wherecondition);
                            DataTable dtofKMOBankCodePayceller = db_connection.ExecuteQueryDataTableProcedure(cmd);
                            try
                            {
                                bnkId = Convert.ToString(dtofKMOBankCodePayceller.Rows[0]["bank_code"]);
                            }
                            catch (Exception ex)
                            {
                            }

                            ServicePointManager.Expect100Continue = true;
                            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
                                   | SecurityProtocolType.Tls11
                               | SecurityProtocolType.Tls12;

                            string accNumber = Convert.ToString(obj.Account_Number).Trim();
                            string APIUser_ID = Convert.ToString(dtt.Rows[i]["APIUser_ID"]).Trim();
                            string Password = Convert.ToString(dtt.Rows[i]["Password"]).Trim();


                            var client = new RestClient(Convert.ToString(dtt.Rows[i]["API_URL"]));
                            client.Timeout = -1;
                            var request = new RestRequest(requestLink);
                            request.Method = Method.POST;
                            request.AddHeader("Content-Type", "text/xml; charset=utf-8");
                            var body = @"<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:prov=""http://providus.com/"">
                                " + "\n" +
                                @"   <soapenv:Header/>
                                " + "\n" +
                                @"   <soapenv:Body>
                                " + "\n" +
                                @"      <prov:GetNIPAccount>
                                " + "\n" +
                                @"         <!--Optional:-->
                                " + "\n" +
                                @"         <account_number>" + accNumber + "</account_number>" + "\n" +
                                @"         <!--Optional:-->
                                " + "\n" +
                                @"         <bank_code>" + bnkId + "</bank_code>" + "\n" +
                                @"         <!--Optional:-->
                                " + "\n" +
                                @"         <username>" + APIUser_ID + "</username>" + "\n" +
                                @"         <!--Optional:-->
                                " + "\n" +
                                @"         <password>" + Password + "</password>" + "\n" +
                                @"      </prov:GetNIPAccount>
                                " + "\n" +
                                @"   </soapenv:Body>
                                " + "\n" +
                                @"</soapenv:Envelope>";

                            request.AddParameter("text/xml; charset=utf-8", body, ParameterType.RequestBody);
                            CompanyInfo.InsertActivityLogDetails("App Providus request parameters GetNIPAccount  : <br/>" + body, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(obj.Customer_ID), "getAccountHolderDetails", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "validateAccountNumber", context);
                            IRestResponse response1 = client.Execute(request);

                            CompanyInfo.InsertActivityLogDetails("App Providus response parameters GetNIPAccount  : <br/>" + response1.Content, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(obj.Customer_ID), "getAccountHolderDetails", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "validateAccountNumber", context);
                            XmlDocument doc = new XmlDocument();
                            doc.LoadXml(response1.Content);
                            XmlNodeList nodeList = doc.GetElementsByTagName("return");
                            string accountName = "";
                            foreach (XmlNode node in nodeList)
                            {
                                string jsonContent = node.InnerText;

                                try
                                {
                                    JObject jsonObject = JObject.Parse(jsonContent);

                                    responseCode = (string)jsonObject["responseCode"];

                                    if (responseCode == "00")
                                    { accountName = (string)jsonObject["accountName"]; break; }
                                }
                                catch (JsonException ex)
                                {
                                    Console.WriteLine("Error parsing JSON: " + ex.Message);
                                }
                            }
                            accHolderName = accountName;

                        }
                        catch (Exception ex)
                        {
                            CompanyInfo.InsertActivityLogDetails("App Error Get account details Providus response <br>:" + ex.ToString() + " " + ex.ToString(), Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(obj.Customer_ID), "getAccountHolderDetails", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "Get Beneficiary Bank Account Details", context);
                        }
                    }
                    if (api_id == 28)
                    {
                        try
                        {
                            bankCode = Convert.ToString(dtofKMOBankCode.Rows[0]["bank_code"]);
                            var client = new RestClient(apiurl + "accounts/resolve");
                            //var request = new RestRequest(Method.POST);
                            var request = new RestRequest(Method.POST);
                            //request.Method = Method.POST;
                            request.AddHeader("Authorization", accesscode);
                            request.AddHeader("Content-Type", "application/json");
                            var body = @"{
" + "\n" +
                            @"    ""account_number"": """ + obj.Account_Number + @""",
" + "\n" +
                            @"    ""account_bank"": """ + bankCode + @"""
" + "\n" +
                            @"}";
                            //request.AddStringBody(body, DataFormat.Json);
                            //RestResponse response = await client.ExecuteAsync(request);
                            request.AddParameter("application/json", body, ParameterType.RequestBody);
                            CompanyInfo.InsertActivityLogDetails("Flutterwave bank validate api request: " + body, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(obj.Customer_ID), "getAccountHolderDetails", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "Get Beneficiary Bank Account Details", context);
                            var response = client.Execute(request);
                            CompanyInfo.InsertActivityLogDetails("Flutterwave bank validate api response: " + response.Content, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(obj.Customer_ID), "getAccountHolderDetails", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "Get Beneficiary Bank Account Details", context);
                            //CompanyInfo.InsertActivityLogDetails("Flutterwave bank validate api response: " + response.ErrorMessage, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(obj.Customer_ID), "getAccountHolderDetails", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "Get Beneficiary Bank Account Details");
                            //CompanyInfo.InsertActivityLogDetails("Flutterwave bank validate api response: " + response.ErrorException, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(obj.Customer_ID), "getAccountHolderDetails", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "Get Beneficiary Bank Account Details");
                            dynamic Json = null;
                            Json = Newtonsoft.Json.JsonConvert.DeserializeObject(response.Content);
                            try
                            {
                                accHolderName = Json.data.account_name;
                            }
                            catch (Exception ex) { }
                            //Console.WriteLine(response.Content);
                            return accHolderName;
                        }
                        catch (Exception ex)
                        {
                            CompanyInfo.InsertActivityLogDetails("Flutterwave api error." + ex.ToString(), Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(obj.Customer_ID), "getAccountHolderDetails", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "Get Beneficiary Bank Account Details", context);
                        }
                    }
                }
                
            }
            catch (Exception ex)
            {
                CompanyInfo.InsertActivityLogDetails("Error Get account details response <br>:" + api_id +" " + ex.ToString(), Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), Convert.ToInt32(obj.Customer_ID), "getAccountHolderDetails", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "Get Beneficiary Bank Account Details", context);
            }
            return accHolderName;  
        }
        */
       

        public static string  TokenGenration(DataTable dt , HttpContext context)
        {
            try
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {

                    int api_id = Convert.ToInt32(dt.Rows[i]["ID"]);
                    //if (api_id != 14)
                    //{
                    //    continue;
                    //}

                    string apiurl = Convert.ToString(dt.Rows[i]["API_URL"]);
                    string apiuser = Convert.ToString(dt.Rows[i]["APIUser_ID"]);
                    string apipass = Convert.ToString(dt.Rows[i]["Password"]);
                    string accesscode = Convert.ToString(dt.Rows[i]["APIAccess_Code"]);//unique code
                    string apicompany_id = Convert.ToString(dt.Rows[i]["APICompany_ID"]);
                    string apiField = Convert.ToString(dt.Rows[i]["api_Fields"]);                    
                    if (api_id == 14)
                    {
                        string api_fields = ""; string gettoken = "";
                        api_fields = Convert.ToString(dt.Rows[i]["api_Fields"]);
                        if (api_fields != "" && api_fields != null)
                        {
                            Newtonsoft.Json.Linq.JObject obj = Newtonsoft.Json.Linq.JObject.Parse(api_fields);
                            gettoken = Convert.ToString(obj["gettoken"]);
                        }
                        var client = new RestClient(gettoken);
                        client.Timeout = -1;
                        var request = new RestRequest(Method.POST);
                        request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                        request.AddParameter("grant_type", "client_credentials");
                        request.AddParameter("client_id", Convert.ToString(dt.Rows[i]["APIUser_ID"]));
                        request.AddParameter("client_secret", Convert.ToString(dt.Rows[i]["APIAccess_Code"]));
                        IRestResponse response = client.Execute(request);
                        Console.WriteLine(response.Content);
                        string activity = "client_id :" + Convert.ToString(dt.Rows[i]["APIUser_ID"]) + "   client_secret :" + Convert.ToString(dt.Rows[i]["APIAccess_Code"]);
                        CompanyInfo.InsertActivityLogDetails("Generate TokenGenration Kmoney request <br>:" + activity, 0, 0, 0, 0, "Search", 0, 0, "Generate TokenGenration Kmoney request", context);
                        CompanyInfo.InsertActivityLogDetails("Generate TokenGenration Kmoney response <br>:" + response.Content, 0, 0, 0, 0, "Search", 0, 0, "Generate TokenGenration Kmoney response", context);
                        kmoney json = Newtonsoft.Json.JsonConvert.DeserializeObject<kmoney>(response.Content);
                        return json.access_token;                        
                    }
                    else if (api_id == 20) {
                        var client = new RestClient(apiurl+"token");
                        client.Timeout = -1;
                        var request = new RestRequest(Method.POST);
                        request.AddHeader("Accept", "application/json");
                        request.AddHeader("Content-Type", "application/json");
                        var body = @"{
    " + "\n" +
                        @"    ""email"": """ + Convert.ToString(dt.Rows[i]["APIUser_ID"]) + @""",
    " + "\n" +
                        @"    ""password"" : """ + Convert.ToString(dt.Rows[i]["Password"]) + @""" 
    " + "\n" +
                        @"}";
                        request.AddParameter("application/json", body, ParameterType.RequestBody);
                        CompanyInfo.InsertActivityLogDetails("Get TierMoney account details request <br>:" + body, 0, 0, 0, 0, "getAccountHolderDetails TierMoney", 1, 1, "Get Beneficiary Bank Account Details", context);
                        IRestResponse response = client.Execute(request);
                        CompanyInfo.InsertActivityLogDetails("Get TierMoney account details response <br>:" + response.Content,0, 0, 0, 0, "getAccountHolderDetails TierMoney", 1, 1, "Get Beneficiary Bank Account Details", context);
                        kmoney json = Newtonsoft.Json.JsonConvert.DeserializeObject<kmoney>(response.Content);
                        return json.data.token;
                    }
                }
            }
            catch(Exception ex)
            {
                CompanyInfo.InsertActivityLogDetails("Generate TokenGenration Kmoney response Error :<br>:" + ex.ToString(), 0, 0, 0, 0, "Search", 0, 0, "Generate TokenGenration Kmoney response error", context); 
            }
            return "";
        }
     
        public class kmoney
        {
            public string code { get; set; }
            public string zeepay_id { get; set; }
            public string amount { get; set; }
            public string message { get; set; }
            public string token_type { get; set; }
            public string expires_in { get; set; }
            public string access_token { get; set; }
            public string balance { get; set; }
            public data data { get; set; }
            public string traceid { get; set; }
            public string description { get; set; }
        }

        public class data
        {
            public string balance { get; set; }
            public string lasttransdate { get; set; }
            public string token { get; set; }
            public string success { get; set; }
            public string expires_at { get; set; }
            public string accountName { get; set; }
            public string bankName { get; set; }
            public string bvn { get; set; }
            //{"data":{"accountName":"SEGUN   ADEMOLA","bankName":"Zenith Bank","bvn":"22238416831","success":true}}
        }

       
    }
}
