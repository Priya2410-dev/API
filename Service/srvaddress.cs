using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using MySqlConnector;
using Newtonsoft.Json.Linq;

namespace Calyx_Solutions.Service
{
    public class srvaddress
    {       
        public object searchaddress(Model.Customer c , HttpContext context)
        {
            object profile = "";
            MySqlConnector.MySqlCommand cmd = new MySqlConnector.MySqlCommand("GetAPIDetails");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("_API_ID", 2);//Credit Safe API ID
            cmd.Parameters.AddWithValue("_Client_ID", c.Client_ID);
            cmd.Parameters.AddWithValue("_status", 0);// API Status
            DataTable dtApi = db_connection.ExecuteQueryDataTableProcedure(cmd);
            
            if (dtApi.Rows.Count > 0)
            {
                c.Record_Insert_DateTime = (string)CompanyInfo.gettime(c.Client_ID, context);
                string ApiKey = Convert.ToString(dtApi.Rows[0]["UserName"]);
                string url = Convert.ToString(dtApi.Rows[0]["API_URL"]) + c.PostCode + "?api-key=" + ApiKey + "";
                CompanyInfo.InsertrequestLogTracker("searchaddress url : " + url, 0, 0, 0, 0, "searchaddress", Convert.ToInt32(0), Convert.ToInt32(0), "", context);
                try
                {
                    // creates a web request
                    HttpWebRequest webReq = (HttpWebRequest)WebRequest.Create(url);
                    //ServicePointManager.ServerCertificateValidationCallback +=
                    //  (sender, certificate, chain, sslPolicyErrors) => true;
                    //ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12;
                    ////webReq.UseDefaultCredentials = true;             
                    // use default credentials
                    //webReq.Headers.Add("Authorization", authorization);
                    // sets the method as a get
                    webReq.Method = "GET";
                    webReq.ContentType = "application/json";
                    try
                    {
                        MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("SaveAPIRequestResponce");
                        _cmd.CommandType = CommandType.StoredProcedure;
                        _cmd.Parameters.AddWithValue("_API_ID", 2);
                        _cmd.Parameters.AddWithValue("_Client_ID", c.Client_ID);
                        _cmd.Parameters.AddWithValue("_Customer_ID", 0);
                        _cmd.Parameters.AddWithValue("_status", 0);
                        _cmd.Parameters.AddWithValue("_Function_name", "Search Address");
                        _cmd.Parameters.AddWithValue("_Remark", 0);
                        _cmd.Parameters.AddWithValue("_comments", url.Replace("'", "''"));
                        _cmd.Parameters.AddWithValue("_Record_Insert_DateTime", c.Record_Insert_DateTime);
                        _cmd.Parameters.AddWithValue("_Branch_ID", c.Branch_ID);
                        int msg1 = db_connection.ExecuteNonQueryProcedure(_cmd);
                    }
                    catch (Exception ex)
                    {
                        string error = ex.ToString().Replace("\'", "\\'");

                        MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("SaveException");
                        _cmd.CommandType = CommandType.StoredProcedure;
                        _cmd.Parameters.AddWithValue("_Record_Insert_DateTime", c.Record_Insert_DateTime);
                        _cmd.Parameters.AddWithValue("_error", error);
                        int msg1 = db_connection.ExecuteNonQueryProcedure(_cmd);
                    }
                    // performs the request and gets the response            
                    using (HttpWebResponse webResp = (HttpWebResponse)webReq.GetResponse())
                    {
                        Console.WriteLine(webResp.StatusCode);

                        // prints out the server
                        Console.WriteLine(webResp.Server);

                        // create a stream to the response
                        var answer = webResp.GetResponseStream();
                        // read the stream

                        if (answer != null)
                        {
                            StreamReader streamReader = new StreamReader(answer);
                            string result = streamReader.ReadToEnd();
                            profile = result;

                            var dynamicobject = JsonConvert.DeserializeObject<dynamic>(profile.ToString());
                            object historyname = dynamicobject.addresses;
                            profile = historyname;
                            
                            try
                            {
                                int status = 1; string Function_name = "Search Address";
                                string Remark = Convert.ToString(CompanyInfo.getAPIStatus("PASS", c.Client_ID));

                                MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("SaveAPIRequestResponce");
                                _cmd.CommandType = CommandType.StoredProcedure;
                                _cmd.Parameters.AddWithValue("_API_ID", 2);
                                _cmd.Parameters.AddWithValue("_Client_ID", c.Client_ID);
                                _cmd.Parameters.AddWithValue("_Customer_ID", 0);
                                _cmd.Parameters.AddWithValue("_status", status);
                                _cmd.Parameters.AddWithValue("_Function_name", Function_name);
                                _cmd.Parameters.AddWithValue("_Remark", Remark);
                                _cmd.Parameters.AddWithValue("_comments", result.Replace("'", "''"));
                                _cmd.Parameters.AddWithValue("_Record_Insert_DateTime", c.Record_Insert_DateTime);
                                _cmd.Parameters.AddWithValue("_Branch_ID", c.Branch_ID);
                                int msg1 = db_connection.ExecuteNonQueryProcedure(_cmd);

                            }
                            catch (Exception ex)
                            {
                                string error = ex.ToString().Replace("\'", "\\'");

                                MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("SaveException");
                                _cmd.CommandType = CommandType.StoredProcedure;
                                _cmd.Parameters.AddWithValue("_Record_Insert_DateTime", c.Record_Insert_DateTime);
                                _cmd.Parameters.AddWithValue("_error", error);
                                int msg1 = db_connection.ExecuteNonQueryProcedure(_cmd);
                            }
                        }
                        else
                        {
                        }
                    }
                }
                catch (Exception ex) {
                    CompanyInfo.InsertrequestLogTracker("searchaddress Error : " + ex.ToString(), 0, 0, 0, 0, "searchaddress", Convert.ToInt32(0), Convert.ToInt32(0), "", context);
                    throw ex; }
            }
            return profile;
        }
    }
}
