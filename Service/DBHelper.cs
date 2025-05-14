using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.IO;
using System.Xml;
using System.Net;
using MySql.Data.MySqlClient;
using System.Collections;
using Microsoft;
using System.Web;
using System.Web.UI;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;

namespace Calyx_Solutions.Service
{
    public class DBHelper
    {
        MySqlConnection con;
        private readonly IConfiguration _configuration;

        public MySqlConnection OpenConnection()
        {
            try
            {
                //New Code strat
                string myConnection = string.Empty;
                myConnection = "ConnectionString";

                //New Code end

                //Old Code strat
                //string myConnection = Convert.ToString(System.Web.HttpContext.Current.Session["CurrentFQDN"]);
                //Old Code end

                //string strConnection = System.Configuration.ConfigurationManager.AppSettings["ConnectionString"];
                string strConnection = Get_SqlConnstring(myConnection);
                con = new MySqlConnection(strConnection);

                //if (con.State == ConnectionState.Closed || con.State == ConnectionState.Broken)
                //{
                //    con.Open();
                //}

                return con;
            }
            catch (MySqlException exm)
            {
                throw exm;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void CloseConnection()
        {
            try
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                    MySqlConnection.ClearAllPools();
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DBHelper()
        {

        }

        public string Get_SqlConnstring()
        {
            try
            {


                //DBHelper objDBHelper = new DBHelper();
                //string connStr = objDBHelper.FQDNString;
                //int i = 0;
                //if (System.Web.HttpContext.Current.Session["XYZ"] == null)
                //{
                //    i = 1;
                //    System.Web.HttpContext.Current.Session["XYZ"] = i;
                //}
                //else
                //{
                //    i = (int)System.Web.HttpContext.Current.Session["XYZ"];
                //    i = i + 1;
                //    System.Web.HttpContext.Current.Session["XYZ"] = i;
                //}
                //writeToFile(i.ToString());

                //New Code start
                string strConnection = string.Empty;
                strConnection = _configuration.GetConnectionString("connectionString");
                //if ((System.Web.HttpContext.Current != null) && (!string.IsNullOrWhiteSpace(Convert.ToString(System.Web.HttpContext.Current.Request.Url.Host))) && System.Web.HttpContext.Current.Request.Url.Host.ToLower() != "localhost")
                //    strConnection = System.Configuration.ConfigurationManager.AppSettings[System.Web.HttpContext.Current.Request.Url.Host];
                //else if ((System.Web.HttpContext.Current != null) && (!string.IsNullOrWhiteSpace(Convert.ToString(System.Web.HttpContext.Current.Session["CurrentFQDN"]))) && Convert.ToString(System.Web.HttpContext.Current.Session["CurrentFQDN"]) != "localhost")//.ToLower()
                //    strConnection = System.Configuration.ConfigurationManager.AppSettings[Convert.ToString(System.Web.HttpContext.Current.Session["CurrentFQDN"])];
                // else
                //     strConnection = _configuration.GetConnectionString("connectionString");// System.Configuration.ConfigurationManager.AppSettings["ConnectionString"];
                //New Code end

                //Old Code strat
                //string myConnection = Convert.ToString(System.Web.HttpContext.Current.Session["CurrentFQDN"]);
                //string strConnection = System.Configuration.ConfigurationManager.AppSettings[myConnection];
                //
                if (string.IsNullOrEmpty(strConnection))
                {
                    strConnection = _configuration.GetConnectionString("connectionString");// System.Configuration.ConfigurationManager.AppSettings["ConnectionString"].ToString();
                }
                else if (string.IsNullOrWhiteSpace(strConnection))
                {
                    strConnection = _configuration.GetConnectionString("connectionString");// System.Configuration.ConfigurationManager.AppSettings["ConnectionString"].ToString();
                }
                //Old Code end

                //string strConnection = "Data Source=192.168.1.14,1433;Network=DBMSSOCN;Initial Catalog=WEB_RAM_28MAY12;User ID=sa;Password=123";
                return strConnection;
            }
            catch (Exception ex)
            {
                string strConnection = _configuration.GetConnectionString("connectionString"); //System.Configuration.ConfigurationManager.AppSettings["ConnectionString"].ToString();
                return strConnection;
            }
            finally
            {

            }


        }
        public string Get_SqlConnstring(string strFQDN)
        {
            try
            {
                string strConnection = _configuration.GetConnectionString("connectionString");// System.Configuration.ConfigurationManager.AppSettings[strFQDN];
                if (string.IsNullOrWhiteSpace(strConnection))
                { strConnection = _configuration.GetConnectionString("connectionString"); }// System.Configuration.ConfigurationManager.AppSettings["ConnectionString"]; }
                //string strConnection = "Data Source=192.168.1.14,1433;Network=DBMSSOCN;Initial Catalog=WEB_RAM_28MAY12;User ID=sa;Password=123";
                return strConnection;
            }
            catch (Exception ex)
            {
                return Get_SqlConnstring();
            }
            finally
            {

            }


        }
        public static DataTable GetLastUpdatedate(string sanctionlistname)
        {
            string key = "1CEBS1"; string UserId = "calyxkolhapur"; string ServerName = "Picktalks";
            string password = "c@lyx123"; string Application_Name = "democebs";
            DataTable dt = new DataTable();

            //dt.Columns.Add("Designation", typeof(string));
            dt.Columns.Add("RecordUpdatedDate", typeof(string));
            //dt.Columns.Add("Sanctionlist_Name", typeof(string));

            try
            {
                TimeZoneInfo timeZoneInfo;
                DateTime dateTime;
                //Set the time zone information to US Mountain Standard Time 
                timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time");
                //Get date and time in US Mountain Standard Time 
                dateTime = TimeZoneInfo.ConvertTime(DateTime.Now, timeZoneInfo);
                //Print out the date and time
                string DateLondon = dateTime.ToString("dd-MM-yyyy HH:mm tt");

                var _url = "https://currencygenie.co.uk/services/CheckSanctionList.asmx";
                var _action = "https://currencygenie.co.uk/GetLastDateSaction";
                XmlDocument soapEnvelopeXml = new XmlDocument();
                soapEnvelopeXml.LoadXml(@"<?xml version=""1.0"" encoding=""utf-8""?>  
        <soap:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">      
        <soap:Header/>
        <soap:Body>  
    <GetLastDateSaction xmlns=""https://currencygenie.co.uk"">

            <ServerName>" + ServerName + @"</ServerName>
            <Application_Name>" + Application_Name + @"</Application_Name>
            <key>" + key + @"</key>
            <UserId>" + UserId + @"</UserId>
            <password>" + password + @"</password>
            <prefixText>" + sanctionlistname + @"</prefixText>            
        </GetLastDateSaction>
        </soap:Body>  
        </soap:Envelope >");
                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(_url);
                webRequest.Headers.Add("SOAPAction", _action);
                //webRequest.Headers.Add(@"SOAP:Action");
                webRequest.ContentType = "text/xml;charset=\"utf-8\"";
                webRequest.Accept = "text/xml";
                webRequest.Method = "POST";
                //  webRequest.KeepAlive = false;
                // HttpContext.Current.Session["CheckWebExc"] = "";
                //     Microsoft.Office.Interop.Word.Page page = HttpContext.Current.CurrentHandler as Microsoft.Office.Interop.Word.Page;
                try
                {
                    using (Stream stream = webRequest.GetRequestStream())
                    {
                        soapEnvelopeXml.Save(stream);
                    }
                }
                catch (Exception ex)
                {
                    //   HttpContext.Current.Session["CheckWebExc"] = ex;
                }
                // begin async call to web request.
                IAsyncResult asyncResult = webRequest.BeginGetResponse(null, null);

                // suspend this thread until call is complete. You might want to
                // do something usefull here like update your UI.
                asyncResult.AsyncWaitHandle.WaitOne();
                string json = "";
                // get the response from the completed web request.
                string soapResult;
                using (WebResponse webResponse = webRequest.EndGetResponse(asyncResult))
                {
                    using (StreamReader rd = new StreamReader(webResponse.GetResponseStream()))
                    {
                        soapResult = rd.ReadToEnd();
                        XmlDocument xmlDoc = new XmlDocument();
                        xmlDoc.LoadXml(soapResult);
                        StringReader theReader = new StringReader(soapResult);
                        DataSet theDataSet = new DataSet();
                        XmlNamespaceManager xmlnsManager = new System.Xml.XmlNamespaceManager(xmlDoc.NameTable);
                        XmlNodeList timeslot = xmlDoc.GetElementsByTagName("SanctionList");
                        int i = 0;
                        string ResponseCode = "1";
                        foreach (XmlNode node in timeslot)
                        {
                            //if (i == 0)
                            //{
                            //    ResponseCode = node["ResponseCode"].InnerText;
                            //}
                            //else if (ResponseCode == "00")
                            //{
                            //foreach (XmlNode c in node.ChildNodes)                       
                            dt.Rows.Add(node["RecordUpdatedDate"].InnerText);//, node["Passport_Details"].InnerText, node["DOB"].InnerText, node["Town_of_Birth"].InnerText);
                            //dt.Rows.Add(node["Nationality"].InnerText);
                            //dt.Rows.Add(node["Passport_Details"].InnerText);
                            //dt.Rows.Add(node["DOB"].InnerText);
                            //dt.Rows.Add(node["Town_of_Birth"].InnerText);
                            //}
                            //i++;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //MySqlCommand insert_log = new MySqlCommand("Insert into Error_LogTable (Error,Date,UserID) values(@Error,@Date,@UserID)");
                //insert_log.Parameters.AddWithValue("@Error", ex);
                //insert_log.Parameters.AddWithValue("@Date", DateTime.Now);
                //insert_log.Parameters.AddWithValue("@UserID", Convert.ToInt32(HttpContext.Current.Session["User_ID"]));
            }
            return dt;
        }
        public static DataTable GetEUSanctionList(string prefix)
        {
            //string key = "centraldbconnection"; string UserId = "root"; string ServerName = "localhost";
            //string password = "root"; string Application_Name = "cebsgeniedb";
            string key = "1CEBS1"; string UserId = "calyxkolhapur"; string ServerName = "Picktalks";
            string password = "c@lyx123"; string Application_Name = "democebs";
            DataTable dt = new DataTable();
            dt.Columns.Add("Name", typeof(string));
            //dt.Columns.Add("birthdate", typeof(string));
            dt.Columns.Add("_euReferenceNumber", typeof(string));
            dt.Columns.Add("citizenship_countryDescription", typeof(string));

            dt.Columns.Add("remark", typeof(string));
            try
            {
                TimeZoneInfo timeZoneInfo;
                DateTime dateTime;
                //Set the time zone information to US Mountain Standard Time 
                timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time");
                //Get date and time in US Mountain Standard Time 
                dateTime = TimeZoneInfo.ConvertTime(DateTime.Now, timeZoneInfo);
                //Print out the date and time
                string DateLondon = dateTime.ToString("dd-MM-yyyy HH:mm tt");

                var _url = "https://currencygenie.co.uk/services/CheckSanctionList.asmx";
                var _action = "https://currencygenie.co.uk/GetEUSaction";

                XmlDocument soapEnvelopeXml = new XmlDocument();
                soapEnvelopeXml.LoadXml(@"<?xml version=""1.0"" encoding=""utf-8""?>  
        <soap:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">      
        <soap:Header/>
        <soap:Body>  
        <GetEUSaction xmlns=""https://currencygenie.co.uk/"">
            <ServerName>" + ServerName + @"</ServerName>
            <Application_Name>" + Application_Name + @"</Application_Name>
            <key>" + key + @"</key>
            <UserId>" + UserId + @"</UserId>
            <password>" + password + @"</password>
            <prefixText>" + prefix + @"</prefixText>            
        </GetEUSaction>
        </soap:Body>  
        </soap:Envelope >");
                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(_url);
                webRequest.Headers.Add("SOAPAction", _action);
                //webRequest.Headers.Add(@"SOAP:Action");
                webRequest.ContentType = "text/xml;charset=\"utf-8\"";
                webRequest.Accept = "text/xml";
                webRequest.Method = "POST";
                //  webRequest.KeepAlive = false;
                //    HttpContext.Current.Session["CheckWebExc"] = "";
                // Microsoft.Office.Interop.Word.Page page = HttpContext.Current.CurrentHandler as Microsoft.Office.Interop.Word.Page;
                try
                {
                    using (Stream stream = webRequest.GetRequestStream())
                    {
                        soapEnvelopeXml.Save(stream);
                    }
                }
                catch (Exception ex)
                {
                    //  HttpContext.Current.Session["CheckWebExc"] = ex;
                }
                // begin async call to web request.
                IAsyncResult asyncResult = webRequest.BeginGetResponse(null, null);

                // suspend this thread until call is complete. You might want to
                // do something usefull here like update your UI.
                asyncResult.AsyncWaitHandle.WaitOne();
                string json = "";
                // get the response from the completed web request.
                string soapResult;
                using (WebResponse webResponse = webRequest.EndGetResponse(asyncResult))
                {
                    using (StreamReader rd = new StreamReader(webResponse.GetResponseStream()))
                    {
                        soapResult = rd.ReadToEnd();
                        XmlDocument xmlDoc = new XmlDocument();
                        xmlDoc.LoadXml(soapResult);
                        StringReader theReader = new StringReader(soapResult);
                        DataSet theDataSet = new DataSet();
                        XmlNamespaceManager xmlnsManager = new System.Xml.XmlNamespaceManager(xmlDoc.NameTable);
                        XmlNodeList timeslot = xmlDoc.GetElementsByTagName("SanctionList");
                        int i = 0;
                        string ResponseCode = "1";
                        foreach (XmlNode node in timeslot)
                        {
                            //if (i == 0)
                            //{
                            //    ResponseCode = node["ResponseCode"].InnerText;
                            //}
                            //else if (ResponseCode == "00")
                            //{
                            //foreach (XmlNode c in node.ChildNodes)                       
                            dt.Rows.Add(node["Name"].InnerText, node["_euReferenceNumber"].InnerText, node["citizenship_countryDescription"].InnerText, node["remark"].InnerText);
                            //dt.Rows.Add(node["birthdate"].InnerText);
                            //dt.Rows.Add(node["_euReferenceNumber"].InnerText);
                            //dt.Rows.Add(node["citizenship_countryDescription"].InnerText);
                            //dt.Rows.Add(node["remark"].InnerText);
                            ////dt.Rows.Add(node["INDIVIDUAL_DATE_OF_BIRTH"].InnerText);
                            ////dt.Rows.Add(node["COMMENTS1"].InnerText);


                            //}
                            //i++;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //MySqlCommand insert_log = new MySqlCommand("Insert into Error_LogTable (Error,Date,UserID) values(@Error,@Date,@UserID)");
                //insert_log.Parameters.AddWithValue("@Error", ex);
                //insert_log.Parameters.AddWithValue("@Date", DateTime.Now);
                //insert_log.Parameters.AddWithValue("@UserID", Convert.ToInt32(HttpContext.Current.Session["User_ID"]));
            }
            return dt;
        }
        public static DataTable GetUNSanctionList(string prefix)
        {
            string key = "1CEBS1"; string UserId = "calyxkolhapur"; string ServerName = "Picktalks";
            string password = "c@lyx123"; string Application_Name = "democebs";
            DataTable dt = new DataTable();
            dt.Columns.Add("Name", typeof(string));
            dt.Columns.Add("REFERENCE_NUMBER", typeof(string));
            dt.Columns.Add("NATIONALITY", typeof(string));
            dt.Columns.Add("INDIVIDUAL_DOCUMENT", typeof(string));
            dt.Columns.Add("INDIVIDUAL_DOCUMENT_Number", typeof(string));
            dt.Columns.Add("INDIVIDUAL_DATE_OF_BIRTH", typeof(string));
            dt.Columns.Add("COMMENTS1", typeof(string));
            try
            {
                TimeZoneInfo timeZoneInfo;
                DateTime dateTime;
                //Set the time zone information to US Mountain Standard Time 
                timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time");
                //Get date and time in US Mountain Standard Time 
                dateTime = TimeZoneInfo.ConvertTime(DateTime.Now, timeZoneInfo);
                //Print out the date and time
                string DateLondon = dateTime.ToString("dd-MM-yyyy HH:mm tt");

                var _url = "https://currencygenie.co.uk/services/CheckSanctionList.asmx";
                var _action = "https://currencygenie.co.uk/GetUNSaction";

                XmlDocument soapEnvelopeXml = new XmlDocument();
                soapEnvelopeXml.LoadXml(@"<?xml version=""1.0"" encoding=""utf-8""?>  
        <soap:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">      
        <soap:Header/>
        <soap:Body>  
        <GetUNSaction xmlns=""https://currencygenie.co.uk/"">
            <ServerName>" + ServerName + @"</ServerName>
            <Application_Name>" + Application_Name + @"</Application_Name>
            <key>" + key + @"</key>
            <UserId>" + UserId + @"</UserId>
            <password>" + password + @"</password>
            <prefixText>" + prefix + @"</prefixText>            
        </GetUNSaction>
        </soap:Body>  
        </soap:Envelope >");
                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(_url);
                webRequest.Headers.Add("SOAPAction", _action);
                //webRequest.Headers.Add(@"SOAP:Action");
                webRequest.ContentType = "text/xml;charset=\"utf-8\"";
                webRequest.Accept = "text/xml";
                webRequest.Method = "POST";
                //  webRequest.KeepAlive = false;
                //     HttpContext.Current.Session["CheckWebExc"] = "";
                //    Microsoft.Office.Interop.Word.Page page = HttpContext.Current.CurrentHandler as Microsoft.Office.Interop.Word.Page;
                try
                {
                    using (Stream stream = webRequest.GetRequestStream())
                    {
                        soapEnvelopeXml.Save(stream);
                    }
                }
                catch (Exception ex)
                {
                    //  HttpContext.Current.Session["CheckWebExc"] = ex;
                }
                // begin async call to web request.
                IAsyncResult asyncResult = webRequest.BeginGetResponse(null, null);

                // suspend this thread until call is complete. You might want to
                // do something usefull here like update your UI.
                asyncResult.AsyncWaitHandle.WaitOne();
                string json = "";
                // get the response from the completed web request.
                string soapResult;
                using (WebResponse webResponse = webRequest.EndGetResponse(asyncResult))
                {
                    using (StreamReader rd = new StreamReader(webResponse.GetResponseStream()))
                    {
                        soapResult = rd.ReadToEnd();
                        XmlDocument xmlDoc = new XmlDocument();
                        xmlDoc.LoadXml(soapResult);
                        StringReader theReader = new StringReader(soapResult);
                        DataSet theDataSet = new DataSet();
                        XmlNamespaceManager xmlnsManager = new System.Xml.XmlNamespaceManager(xmlDoc.NameTable);
                        XmlNodeList timeslot = xmlDoc.GetElementsByTagName("SanctionList");
                        int i = 0;
                        string ResponseCode = "1";
                        foreach (XmlNode node in timeslot)
                        {
                            //if (i == 0)
                            //{
                            //    ResponseCode = node["ResponseCode"].InnerText;
                            //}
                            //else if (ResponseCode == "00")
                            //{
                            //foreach (XmlNode c in node.ChildNodes)                       
                            dt.Rows.Add(node["Name"].InnerText, node["REFERENCE_NUMBER"].InnerText, node["NATIONALITY"].InnerText, node["INDIVIDUAL_DOCUMENT"].InnerText, node["INDIVIDUAL_DOCUMENT_Number"].InnerText, node["INDIVIDUAL_DATE_OF_BIRTH"].InnerText, node["COMMENTS1"].InnerText);
                            //dt.Rows.Add(node["REFERENCE_NUMBER"].InnerText),
                            //dt.Rows.Add(node["NATIONALITY"].InnerText),
                            //dt.Rows.Add(node["INDIVIDUAL_DOCUMENT"].InnerText);
                            //dt.Rows.Add(node["INDIVIDUAL_DOCUMENT_Number"].InnerText);
                            //dt.Rows.Add(node["INDIVIDUAL_DATE_OF_BIRTH"].InnerText);
                            //dt.Rows.Add(node["COMMENTS1"].InnerText);

                            //}
                            //i++;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //MySqlCommand insert_log = new MySqlCommand("Insert into Error_LogTable (Error,Date,UserID) values(@Error,@Date,@UserID)");
                //insert_log.Parameters.AddWithValue("@Error", ex);
                //insert_log.Parameters.AddWithValue("@Date", DateTime.Now);
                //insert_log.Parameters.AddWithValue("@UserID", Convert.ToInt32(HttpContext.Current.Session["User_ID"]));
            }
            return dt;
        }
        public static DataTable GetUSASanctionList(string prefix)
        {
            //string key = "centraldbconnection"; string UserId = "root"; string ServerName = "localhost";
            //string password = "root"; string Application_Name = "cebsgeniedb";
            string key = "1CEBS1"; string UserId = "calyxkolhapur"; string ServerName = "Picktalks";
            string password = "c@lyx123"; string Application_Name = "democebs";
            DataTable dt = new DataTable();
            dt.Columns.Add("Name", typeof(string));
            //dt.Columns.Add("DOB", typeof(string));
            //dt.Columns.Add("Last_Updated", typeof(string));
            try
            {
                TimeZoneInfo timeZoneInfo;
                DateTime dateTime;
                //Set the time zone information to US Mountain Standard Time 
                timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time");
                //Get date and time in US Mountain Standard Time 
                dateTime = TimeZoneInfo.ConvertTime(DateTime.Now, timeZoneInfo);
                //Print out the date and time
                string DateLondon = dateTime.ToString("dd-MM-yyyy HH:mm tt");

                var _url = "https://currencygenie.co.uk/services/CheckSanctionList.asmx";
                var _action = "https://currencygenie.co.uk/GetUSASaction";

                XmlDocument soapEnvelopeXml = new XmlDocument();
                soapEnvelopeXml.LoadXml(@"<?xml version=""1.0"" encoding=""utf-8""?>  
        <soap:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">      
        <soap:Header/>
        <soap:Body>  
        <GetUSASaction xmlns=""https://currencygenie.co.uk/"">
            <ServerName>" + ServerName + @"</ServerName>
            <Application_Name>" + Application_Name + @"</Application_Name>
            <key>" + key + @"</key>
            <UserId>" + UserId + @"</UserId>
            <password>" + password + @"</password>
            <prefixText>" + prefix + @"</prefixText>            
        </GetUSASaction>
        </soap:Body>  
        </soap:Envelope >");
                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(_url);
                webRequest.Headers.Add("SOAPAction", _action);
                //webRequest.Headers.Add(@"SOAP:Action");
                webRequest.ContentType = "text/xml;charset=\"utf-8\"";
                webRequest.Accept = "text/xml";
                webRequest.Method = "POST";
                //  webRequest.KeepAlive = false;
                //  HttpContext.Current.Session["CheckWebExc"] = "";
                //    Microsoft.Office.Interop.Word.Page page = HttpContext.Current.CurrentHandler as Microsoft.Office.Interop.Word.Page;
                try
                {
                    using (Stream stream = webRequest.GetRequestStream())
                    {
                        soapEnvelopeXml.Save(stream);
                    }
                }
                catch (Exception ex)
                {
                    //HttpContext.Current.Session["CheckWebExc"] = ex;
                }
                // begin async call to web request.
                IAsyncResult asyncResult = webRequest.BeginGetResponse(null, null);

                // suspend this thread until call is complete. You might want to
                // do something usefull here like update your UI.
                asyncResult.AsyncWaitHandle.WaitOne();
                string json = "";
                // get the response from the completed web request.
                string soapResult;
                using (WebResponse webResponse = webRequest.EndGetResponse(asyncResult))
                {
                    using (StreamReader rd = new StreamReader(webResponse.GetResponseStream()))
                    {
                        soapResult = rd.ReadToEnd();
                        XmlDocument xmlDoc = new XmlDocument();
                        xmlDoc.LoadXml(soapResult);
                        StringReader theReader = new StringReader(soapResult);
                        DataSet theDataSet = new DataSet();
                        XmlNamespaceManager xmlnsManager = new System.Xml.XmlNamespaceManager(xmlDoc.NameTable);
                        XmlNodeList timeslot = xmlDoc.GetElementsByTagName("SanctionList");
                        int i = 0;
                        string ResponseCode = "1";
                        foreach (XmlNode node in timeslot)
                        {
                            //if (i == 0)
                            //{
                            //    ResponseCode = node["ResponseCode"].InnerText;
                            //}
                            //else if (ResponseCode == "00")
                            //{
                            //foreach (XmlNode c in node.ChildNodes)                       
                            dt.Rows.Add(node["Name"].InnerText);//node["DOB"].InnerText);

                            //}
                            //i++;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //MySqlCommand insert_log = new MySqlCommand("Insert into Error_LogTable (Error,Date,UserID) values(@Error,@Date,@UserID)");
                //insert_log.Parameters.AddWithValue("@Error", ex);
                //insert_log.Parameters.AddWithValue("@Date", DateTime.Now);
                //insert_log.Parameters.AddWithValue("@UserID", Convert.ToInt32(HttpContext.Current.Session["User_ID"]));
            }
            return dt;
        }
        public static DataTable GetSanctionList(string prefix)
        {
            string key = "1CEBS1"; string UserId = "calyxkolhapur"; string ServerName = "Picktalks";
            string password = "c@lyx123"; string Application_Name = "democebs";
            DataTable dt = new DataTable();
            dt.Columns.Add("name", typeof(string));
            dt.Columns.Add("Nationality", typeof(string));
            dt.Columns.Add("Passport_Details", typeof(string));
            dt.Columns.Add("DOB", typeof(string));
            dt.Columns.Add("Town_of_Birth", typeof(string));
            try
            {
                TimeZoneInfo timeZoneInfo;
                DateTime dateTime;
                //Set the time zone information to US Mountain Standard Time 
                timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time");
                //Get date and time in US Mountain Standard Time 
                dateTime = TimeZoneInfo.ConvertTime(DateTime.Now, timeZoneInfo);
                //Print out the date and time
                string DateLondon = dateTime.ToString("dd-MM-yyyy HH:mm tt");

                var _url = "https://currencygenie.co.uk/services/CheckSanctionList.asmx";//"https://currencygenie.co.uk/services/CheckSanctionList.asmx";//"https://currencygenie.co.uk/Services/GetRatesfromCentraldb.asmx";
                var _action = "https://currencygenie.co.uk/GetUKSaction";

                XmlDocument soapEnvelopeXml = new XmlDocument();
                soapEnvelopeXml.LoadXml(@"<?xml version=""1.0"" encoding=""utf-8""?>  
        <soap:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">      
        <soap:Header/>
        <soap:Body>  
        <GetUKSaction xmlns=""https://currencygenie.co.uk/"">
            <ServerName>" + ServerName + @"</ServerName>
            <Application_Name>" + Application_Name + @"</Application_Name>
            <key>" + key + @"</key>
            <UserId>" + UserId + @"</UserId>
            <password>" + password + @"</password>
            <prefixText>" + prefix + @"</prefixText>            
        </GetUKSaction>
        </soap:Body>  
        </soap:Envelope >");
                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(_url);
                webRequest.Headers.Add("SOAPAction", _action);
                //webRequest.Headers.Add(@"SOAP:Action");
                webRequest.ContentType = "text/xml;charset=\"utf-8\"";
                webRequest.Accept = "text/xml";
                webRequest.Method = "POST";
                //  webRequest.KeepAlive = false;
                //      HttpContext.Current.Session["CheckWebExc"] = "";
                //     Microsoft.Office.Interop.Word.Page page = HttpContext.Current.CurrentHandler as Microsoft.Office.Interop.Word.Page;
                try
                {
                    using (Stream stream = webRequest.GetRequestStream())
                    {
                        soapEnvelopeXml.Save(stream);
                    }
                }
                catch (Exception ex)
                {
                    //  HttpContext.Current.Session["CheckWebExc"] = ex;
                }
                // begin async call to web request.
                IAsyncResult asyncResult = webRequest.BeginGetResponse(null, null);

                // suspend this thread until call is complete. You might want to
                // do something usefull here like update your UI.

                asyncResult.AsyncWaitHandle.WaitOne();
                string json = "";
                // get the response from the completed web request.
                string soapResult;
                using (WebResponse webResponse = webRequest.EndGetResponse(asyncResult))
                {
                    using (StreamReader rd = new StreamReader(webResponse.GetResponseStream()))
                    {
                        soapResult = rd.ReadToEnd();
                        XmlDocument xmlDoc = new XmlDocument();
                        xmlDoc.LoadXml(soapResult);
                        StringReader theReader = new StringReader(soapResult);
                        DataSet theDataSet = new DataSet();
                        XmlNamespaceManager xmlnsManager = new System.Xml.XmlNamespaceManager(xmlDoc.NameTable);
                        XmlNodeList timeslot = xmlDoc.GetElementsByTagName("SanctionList");
                        int i = 0;
                        string ResponseCode = "1";
                        foreach (XmlNode node in timeslot)
                        {
                            //if (i == 0)
                            //{
                            //    ResponseCode = node["ResponseCode"].InnerText;
                            //}
                            //else if (ResponseCode == "00")
                            //{
                            //foreach (XmlNode c in node.ChildNodes)                       
                            dt.Rows.Add(node["name"].InnerText, node["Nationality"].InnerText, node["Passport_Details"].InnerText, node["DOB"].InnerText, node["Town_of_Birth"].InnerText);
                            //dt.Rows.Add(node["Nationality"].InnerText);
                            //dt.Rows.Add(node["Passport_Details"].InnerText);
                            //dt.Rows.Add(node["DOB"].InnerText);
                            //dt.Rows.Add(node["Town_of_Birth"].InnerText);
                            //}
                            //i++;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //MySqlCommand insert_log = new MySqlCommand("Insert into Error_LogTable (Error,Date,UserID) values(@Error,@Date,@UserID)");
                //insert_log.Parameters.AddWithValue("@Error", ex);
                //insert_log.Parameters.AddWithValue("@Date", DateTime.Now);
                //insert_log.Parameters.AddWithValue("@UserID", Convert.ToInt32(HttpContext.Current.Session["User_ID"]));
            }
            return dt;
        }
        public static DataTable GetAUDSanctionList(string prefix)
        {
            string key = "1CEBS1"; string UserId = "calyxkolhapur"; string ServerName = "Picktalks";
            string password = "c@lyx123"; string Application_Name = "democebs";
            DataTable dt = new DataTable();
            dt.Columns.Add("name", typeof(string));
            dt.Columns.Add("Nationality", typeof(string));
            dt.Columns.Add("Passport_Details", typeof(string));
            dt.Columns.Add("DOB", typeof(string));
            dt.Columns.Add("Town_of_Birth", typeof(string));
            try
            {
                TimeZoneInfo timeZoneInfo;
                DateTime dateTime;
                //Set the time zone information to US Mountain Standard Time 
                timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time");
                //Get date and time in US Mountain Standard Time 
                dateTime = TimeZoneInfo.ConvertTime(DateTime.Now, timeZoneInfo);
                //Print out the date and time
                string DateLondon = dateTime.ToString("dd-MM-yyyy HH:mm tt");

                var _url = "https://currencygenie.co.uk/services/CheckSanctionList.asmx";//"http://localhost:10353/democebs/CheckSanctionList.asmx";//"https://currencygenie.co.uk/services/CheckSanctionList.asmx";//"https://currencygenie.co.uk/Services/GetRatesfromCentraldb.asmx";
                var _action = "https://currencygenie.co.uk/GetAUDSaction";

                XmlDocument soapEnvelopeXml = new XmlDocument();
                soapEnvelopeXml.LoadXml(@"<?xml version=""1.0"" encoding=""utf-8""?>  
        <soap:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">      
        <soap:Header/>
        <soap:Body>  
        <GetAUDSaction xmlns=""https://currencygenie.co.uk/"">
            <ServerName>" + ServerName + @"</ServerName>
            <Application_Name>" + Application_Name + @"</Application_Name>
            <key>" + key + @"</key>
            <UserId>" + UserId + @"</UserId>
            <password>" + password + @"</password>
            <prefixText>" + prefix + @"</prefixText>            
        </GetAUDSaction>
        </soap:Body>  
        </soap:Envelope >");
                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(_url);
                webRequest.Headers.Add("SOAPAction", _action);
                //webRequest.Headers.Add(@"SOAP:Action");
                webRequest.ContentType = "text/xml;charset=\"utf-8\"";
                webRequest.Accept = "text/xml";
                webRequest.Method = "POST";
                //  webRequest.KeepAlive = false;
                //HttpContext.Current.Session["CheckWebExc"] = "";
                // Microsoft.Office.Interop.Word.Page page = HttpContext.Current.CurrentHandler as Microsoft.Office.Interop.Word.Page;
                try
                {
                    using (Stream stream = webRequest.GetRequestStream())
                    {
                        soapEnvelopeXml.Save(stream);
                    }
                }
                catch (Exception ex)
                {
                    // HttpContext.Current.Session["CheckWebExc"] = ex;
                }
                // begin async call to web request.
                IAsyncResult asyncResult = webRequest.BeginGetResponse(null, null);

                // suspend this thread until call is complete. You might want to
                // do something usefull here like update your UI.
                asyncResult.AsyncWaitHandle.WaitOne();
                string json = "";
                // get the response from the completed web request.
                string soapResult;
                using (WebResponse webResponse = webRequest.EndGetResponse(asyncResult))
                {
                    using (StreamReader rd = new StreamReader(webResponse.GetResponseStream()))
                    {
                        soapResult = rd.ReadToEnd();
                        XmlDocument xmlDoc = new XmlDocument();
                        xmlDoc.LoadXml(soapResult);
                        StringReader theReader = new StringReader(soapResult);
                        DataSet theDataSet = new DataSet();
                        XmlNamespaceManager xmlnsManager = new System.Xml.XmlNamespaceManager(xmlDoc.NameTable);
                        XmlNodeList timeslot = xmlDoc.GetElementsByTagName("SanctionList");
                        int i = 0;
                        string ResponseCode = "1";
                        foreach (XmlNode node in timeslot)
                        {
                            //if (i == 0)
                            //{
                            //    ResponseCode = node["ResponseCode"].InnerText;
                            //}
                            //else if (ResponseCode == "00")
                            //{
                            //foreach (XmlNode c in node.ChildNodes)                       
                            dt.Rows.Add(node["name"].InnerText, node["Nationality"].InnerText, node["Passport_Details"].InnerText, node["DOB"].InnerText, node["Town_of_Birth"].InnerText);
                            //dt.Rows.Add(node["Nationality"].InnerText);
                            //dt.Rows.Add(node["Passport_Details"].InnerText);
                            //dt.Rows.Add(node["DOB"].InnerText);
                            //dt.Rows.Add(node["Town_of_Birth"].InnerText);
                            //}
                            //i++;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MySqlCommand insert_log = new MySqlCommand("Insert into Error_LogTable (Error,Date,UserID) values(@Error,@Date,@UserID)");
                insert_log.Parameters.AddWithValue("@Error", ex);
                insert_log.Parameters.AddWithValue("@Date", DateTime.Now);
                //insert_log.Parameters.AddWithValue("@UserID", Convert.ToInt32(HttpContext.Current.Session["User_ID"]));
                insert_log.Parameters.AddWithValue("@UserID", Convert.ToInt32(0));
            }
            return dt;
        }

        //        public static DataTable GetAUDSanctionList(string prefix)
        //        {
        //            string key = "1CEBS1"; string UserId = "calyxkolhapur"; string ServerName = "Picktalks";
        //            string password = "c@lyx123"; string Application_Name = "democebs";
        //            DataTable dt = new DataTable();
        //            dt.Columns.Add("name", typeof(string));
        //            dt.Columns.Add("Nationality", typeof(string));
        //            dt.Columns.Add("Passport_Details", typeof(string));
        //            dt.Columns.Add("DOB", typeof(string));
        //            dt.Columns.Add("Town_of_Birth", typeof(string));
        //            try
        //            {
        //                TimeZoneInfo timeZoneInfo;
        //                DateTime dateTime;
        //                //Set the time zone information to US Mountain Standard Time 
        //                timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time");
        //                //Get date and time in US Mountain Standard Time 
        //                dateTime = TimeZoneInfo.ConvertTime(DateTime.Now, timeZoneInfo);
        //                //Print out the date and time
        //                string DateLondon = dateTime.ToString("dd-MM-yyyy HH:mm tt");

        //                var _url = "https://currencygenie.co.uk/services/CheckSanctionList.asmx";
        //                var _action = "https://currencygenie.co.uk/GetAUDSaction";

        //                XmlDocument soapEnvelopeXml = new XmlDocument();
        //                soapEnvelopeXml.LoadXml(@"<?xml version=""1.0"" encoding=""utf-8""?>  
        //        <soap:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">      
        //        <soap:Header/>
        //        <soap:Body>  
        //        <GetAUDSaction xmlns=""https://currencygenie.co.uk/"">
        //            <ServerName>" + ServerName + @"</ServerName>
        //            <Application_Name>" + Application_Name + @"</Application_Name>
        //            <key>" + key + @"</key>
        //            <UserId>" + UserId + @"</UserId>
        //            <password>" + password + @"</password>
        //            <prefixText>" + prefix + @"</prefixText>            
        //        </GetAUDSaction>
        //        </soap:Body>  
        //        </soap:Envelope >");
        //                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(_url);
        //                webRequest.Headers.Add("SOAPAction", _action);
        //                //webRequest.Headers.Add(@"SOAP:Action");
        //                webRequest.ContentType = "text/xml;charset=\"utf-8\"";
        //                webRequest.Accept = "text/xml";
        //                webRequest.Method = "POST";
        //                //  webRequest.KeepAlive = false;
        //                HttpContext.Current.Session["CheckWebExc"] = "";
        //                // Microsoft.Office.Interop.Word.Page page = HttpContext.Current.CurrentHandler as Microsoft.Office.Interop.Word.Page;
        //                try
        //                {
        //                    using (Stream stream = webRequest.GetRequestStream())
        //                    {
        //                        soapEnvelopeXml.Save(stream);
        //                    }
        //                }
        //                catch (Exception ex)
        //                {
        //                    HttpContext.Current.Session["CheckWebExc"] = ex;
        //                }
        //                // begin async call to web request.
        //                IAsyncResult asyncResult = webRequest.BeginGetResponse(null, null);

        //                // suspend this thread until call is complete. You might want to
        //                // do something usefull here like update your UI.
        //                asyncResult.AsyncWaitHandle.WaitOne();
        //                string json = "";
        //                // get the response from the completed web request.
        //                string soapResult;
        //                using (WebResponse webResponse = webRequest.EndGetResponse(asyncResult))
        //                {
        //                    using (StreamReader rd = new StreamReader(webResponse.GetResponseStream()))
        //                    {
        //                        soapResult = rd.ReadToEnd();
        //                        XmlDocument xmlDoc = new XmlDocument();
        //                        xmlDoc.LoadXml(soapResult);
        //                        StringReader theReader = new StringReader(soapResult);
        //                        DataSet theDataSet = new DataSet();
        //                        XmlNamespaceManager xmlnsManager = new System.Xml.XmlNamespaceManager(xmlDoc.NameTable);
        //                        XmlNodeList timeslot = xmlDoc.GetElementsByTagName("SanctionList");
        //                        int i = 0;
        //                        string ResponseCode = "1";
        //                        foreach (XmlNode node in timeslot)
        //                        {
        //                            //if (i == 0)
        //                            //{
        //                            //    ResponseCode = node["ResponseCode"].InnerText;
        //                            //}
        //                            //else if (ResponseCode == "00")
        //                            //{
        //                            //foreach (XmlNode c in node.ChildNodes)                       
        //                            dt.Rows.Add(node["name"].InnerText, node["Nationality"].InnerText, node["Passport_Details"].InnerText, node["DOB"].InnerText, node["Town_of_Birth"].InnerText);
        //                            //dt.Rows.Add(node["Nationality"].InnerText);
        //                            //dt.Rows.Add(node["Passport_Details"].InnerText);
        //                            //dt.Rows.Add(node["DOB"].InnerText);
        //                            //dt.Rows.Add(node["Town_of_Birth"].InnerText);
        //                            //}
        //                            //i++;
        //                        }
        //                    }
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                MySqlCommand insert_log = new MySqlCommand("Insert into Error_LogTable (Error,Date,UserID) values(@Error,@Date,@UserID)");
        //                insert_log.Parameters.AddWithValue("@Error", ex);
        //                insert_log.Parameters.AddWithValue("@Date", DateTime.Now);
        //                insert_log.Parameters.AddWithValue("@UserID", Convert.ToInt32(HttpContext.Current.Session["User_ID"]));
        //            }
        //            return dt;
        //        }
        //        public static DataTable GetPEPSanctionList(string prefix)
        //        {
        //            string key = "1CEBS1"; string UserId = "calyxkolhapur"; string ServerName = "Picktalks";
        //            string password = "c@lyx123"; string Application_Name = "democebs";
        //            DataTable dt = new DataTable();

        //            //dt.Columns.Add("Designation", typeof(string));
        //            dt.Columns.Add("Name", typeof(string));
        //            try
        //            {
        //                TimeZoneInfo timeZoneInfo;
        //                DateTime dateTime;
        //                //Set the time zone information to US Mountain Standard Time 
        //                timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time");
        //                //Get date and time in US Mountain Standard Time 
        //                dateTime = TimeZoneInfo.ConvertTime(DateTime.Now, timeZoneInfo);
        //                //Print out the date and time
        //                string DateLondon = dateTime.ToString("dd-MM-yyyy HH:mm tt");

        //                var _url = "https://currencygenie.co.uk/services/CheckSanctionList.asmx";//"https://currencygenie.co.uk/Services/GetRatesfromCentraldb.asmx";
        //                var _action = "https://currencygenie.co.uk/GetPEPSaction";

        //                XmlDocument soapEnvelopeXml = new XmlDocument();
        //                soapEnvelopeXml.LoadXml(@"<?xml version=""1.0"" encoding=""utf-8""?>  
        //        <soap:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">      
        //        <soap:Header/>
        //        <soap:Body>  
        //        <GetPEPSaction xmlns=""https://currencygenie.co.uk/"">
        //            <ServerName>" + ServerName + @"</ServerName>
        //            <Application_Name>" + Application_Name + @"</Application_Name>
        //            <key>" + key + @"</key>
        //            <UserId>" + UserId + @"</UserId>
        //            <password>" + password + @"</password>
        //            <prefixText>" + prefix + @"</prefixText>            
        //        </GetPEPSaction>
        //        </soap:Body>  
        //        </soap:Envelope >");
        //                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(_url);
        //                webRequest.Headers.Add("SOAPAction", _action);
        //                //webRequest.Headers.Add(@"SOAP:Action");
        //                webRequest.ContentType = "text/xml;charset=\"utf-8\"";
        //                webRequest.Accept = "text/xml";
        //                webRequest.Method = "POST";
        //                //  webRequest.KeepAlive = false;
        //                HttpContext.Current.Session["CheckWebExc"] = "";
        //                // Microsoft.Office.Interop.Word.Page page = HttpContext.Current.CurrentHandler as Microsoft.Office.Interop.Word.Page;
        //                try
        //                {
        //                    using (Stream stream = webRequest.GetRequestStream())
        //                    {
        //                        soapEnvelopeXml.Save(stream);
        //                    }
        //                }
        //                catch (Exception ex)
        //                {
        //                    HttpContext.Current.Session["CheckWebExc"] = ex;
        //                }
        //                // begin async call to web request.
        //                IAsyncResult asyncResult = webRequest.BeginGetResponse(null, null);

        //                // suspend this thread until call is complete. You might want to
        //                // do something usefull here like update your UI.
        //                asyncResult.AsyncWaitHandle.WaitOne();
        //                string json = "";
        //                // get the response from the completed web request.
        //                string soapResult;
        //                using (WebResponse webResponse = webRequest.EndGetResponse(asyncResult))
        //                {
        //                    using (StreamReader rd = new StreamReader(webResponse.GetResponseStream()))
        //                    {
        //                        soapResult = rd.ReadToEnd();
        //                        XmlDocument xmlDoc = new XmlDocument();
        //                        xmlDoc.LoadXml(soapResult);
        //                        StringReader theReader = new StringReader(soapResult);
        //                        DataSet theDataSet = new DataSet();
        //                        XmlNamespaceManager xmlnsManager = new System.Xml.XmlNamespaceManager(xmlDoc.NameTable);
        //                        XmlNodeList timeslot = xmlDoc.GetElementsByTagName("SanctionList");
        //                        int i = 0;
        //                        string ResponseCode = "1";
        //                        foreach (XmlNode node in timeslot)
        //                        {
        //                            //if (i == 0)
        //                            //{
        //                            //    ResponseCode = node["ResponseCode"].InnerText;
        //                            //}
        //                            //else if (ResponseCode == "00")
        //                            //{
        //                            //foreach (XmlNode c in node.ChildNodes)                       
        //                            dt.Rows.Add(node["Name"].InnerText);//, node["Passport_Details"].InnerText, node["DOB"].InnerText, node["Town_of_Birth"].InnerText);
        //                            //dt.Rows.Add(node["Nationality"].InnerText);
        //                            //dt.Rows.Add(node["Passport_Details"].InnerText);
        //                            //dt.Rows.Add(node["DOB"].InnerText);
        //                            //dt.Rows.Add(node["Town_of_Birth"].InnerText);
        //                            //}
        //                            //i++;
        //                        }
        //                    }
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                MySqlCommand insert_log = new MySqlCommand("Insert into Error_LogTable (Error,Date,UserID) values(@Error,@Date,@UserID)");
        //                insert_log.Parameters.AddWithValue("@Error", ex);
        //                insert_log.Parameters.AddWithValue("@Date", DateTime.Now);
        //                insert_log.Parameters.AddWithValue("@UserID", Convert.ToInt32(HttpContext.Current.Session["User_ID"]));
        //            }
        //            return dt;
        //        }
        public static DataTable GetPEPSanctionList(string prefix)
        {
            string key = "1CEBS1"; string UserId = "calyxkolhapur"; string ServerName = "Picktalks";
            string password = "c@lyx123"; string Application_Name = "democebs";
            DataTable dt = new DataTable();

            //dt.Columns.Add("Designation", typeof(string));
            dt.Columns.Add("Name", typeof(string));
            try
            {
                TimeZoneInfo timeZoneInfo;
                DateTime dateTime;
                //Set the time zone information to US Mountain Standard Time 
                timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time");
                //Get date and time in US Mountain Standard Time 
                dateTime = TimeZoneInfo.ConvertTime(DateTime.Now, timeZoneInfo);
                //Print out the date and time
                string DateLondon = dateTime.ToString("dd-MM-yyyy HH:mm tt");

                var _url = "https://currencygenie.co.uk/services/CheckSanctionList.asmx";//"https://currencygenie.co.uk/Services/GetRatesfromCentraldb.asmx";
                var _action = "https://currencygenie.co.uk/GetPEPSaction";

                XmlDocument soapEnvelopeXml = new XmlDocument();
                soapEnvelopeXml.LoadXml(@"<?xml version=""1.0"" encoding=""utf-8""?>  
        <soap:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">      
        <soap:Header/>
        <soap:Body>  
        <GetPEPSaction xmlns=""https://currencygenie.co.uk/"">
            <ServerName>" + ServerName + @"</ServerName>
            <Application_Name>" + Application_Name + @"</Application_Name>
            <key>" + key + @"</key>
            <UserId>" + UserId + @"</UserId>
            <password>" + password + @"</password>
            <prefixText>" + prefix + @"</prefixText>            
        </GetPEPSaction>
        </soap:Body>  
        </soap:Envelope >");
                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(_url);
                webRequest.Headers.Add("SOAPAction", _action);
                //webRequest.Headers.Add(@"SOAP:Action");
                webRequest.ContentType = "text/xml;charset=\"utf-8\"";
                webRequest.Accept = "text/xml";
                webRequest.Method = "POST";
                //  webRequest.KeepAlive = false;
                //HttpContext.Current.Session["CheckWebExc"] = "";
                // Microsoft.Office.Interop.Word.Page page = HttpContext.Current.CurrentHandler as Microsoft.Office.Interop.Word.Page;
                try
                {
                    using (Stream stream = webRequest.GetRequestStream())
                    {
                        soapEnvelopeXml.Save(stream);
                    }
                }
                catch (Exception ex)
                {
                    //HttpContext.Current.Session["CheckWebExc"] = ex;
                }
                // begin async call to web request.
                IAsyncResult asyncResult = webRequest.BeginGetResponse(null, null);

                // suspend this thread until call is complete. You might want to
                // do something usefull here like update your UI.
                asyncResult.AsyncWaitHandle.WaitOne();
                string json = "";
                // get the response from the completed web request.
                string soapResult;
                using (WebResponse webResponse = webRequest.EndGetResponse(asyncResult))
                {
                    using (StreamReader rd = new StreamReader(webResponse.GetResponseStream()))
                    {
                        soapResult = rd.ReadToEnd();
                        XmlDocument xmlDoc = new XmlDocument();
                        xmlDoc.LoadXml(soapResult);
                        StringReader theReader = new StringReader(soapResult);
                        DataSet theDataSet = new DataSet();
                        XmlNamespaceManager xmlnsManager = new System.Xml.XmlNamespaceManager(xmlDoc.NameTable);
                        XmlNodeList timeslot = xmlDoc.GetElementsByTagName("SanctionList");
                        int i = 0;
                        string ResponseCode = "1";
                        foreach (XmlNode node in timeslot)
                        {
                            //if (i == 0)
                            //{
                            //    ResponseCode = node["ResponseCode"].InnerText;
                            //}
                            //else if (ResponseCode == "00")
                            //{
                            //foreach (XmlNode c in node.ChildNodes)                       
                            dt.Rows.Add(node["Name"].InnerText);//, node["Passport_Details"].InnerText, node["DOB"].InnerText, node["Town_of_Birth"].InnerText);
                            //dt.Rows.Add(node["Nationality"].InnerText);
                            //dt.Rows.Add(node["Passport_Details"].InnerText);
                            //dt.Rows.Add(node["DOB"].InnerText);
                            //dt.Rows.Add(node["Town_of_Birth"].InnerText);
                            //}
                            //i++;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MySqlCommand insert_log = new MySqlCommand("Insert into Error_LogTable (Error,Date,UserID) values(@Error,@Date,@UserID)");
                insert_log.Parameters.AddWithValue("@Error", ex);
                insert_log.Parameters.AddWithValue("@Date", DateTime.Now);
                insert_log.Parameters.AddWithValue("@UserID", Convert.ToInt32(0));
                //insert_log.Parameters.AddWithValue("@UserID", Convert.ToInt32(HttpContext.Current.Session["User_ID"]));
            }
            return dt;
        }
        public static DataTable GetCompanyDetails_fromHouseRegNo(string Reg_no)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("company_name", typeof(string));
            //dt.Columns.Add("birthdate", typeof(string));
            dt.Columns.Add("company_number", typeof(string));
            dt.Columns.Add("address_line_1", typeof(string));
            dt.Columns.Add("locality", typeof(string));
            dt.Columns.Add("postal_code", typeof(string));
            try
            {

                string soapResult;
                var _url = "https://calyx-solutions.com/Company.asmx?WSDL";
                var _action = "https://calyx-solutions.com/GetCompanyFromNumber";
                XmlDocument soapEnvelopeXml = new XmlDocument();
                soapEnvelopeXml.LoadXml(@"<?xml version=""1.0"" encoding=""utf-8""?>
           <soap:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
           <soap:Header/>
               <soap:Body>
                   <GetCompanyFromNumber xmlns=""https://calyx-solutions.com/"">
                       <CompanyNumber>" + Reg_no + @"</CompanyNumber>
                   </GetCompanyFromNumber>
               </soap:Body>
           </soap:Envelope>");
                HttpWebRequest webRequest = CreateWebRequest(_url, _action);

                //  webRequest.KeepAlive = false;
                InsertSoapEnvelopeIntoWebRequest(soapEnvelopeXml, webRequest);

                // begin async call to web request.
                IAsyncResult asyncResult = webRequest.BeginGetResponse(null, null);

                // suspend this thread until call is complete. You might want to
                // do something usefull here like update your UI.
                asyncResult.AsyncWaitHandle.WaitOne();

                // get the response from the completed web request.                
                using (WebResponse webResponse = webRequest.EndGetResponse(asyncResult))
                {
                    using (StreamReader rd = new StreamReader(webResponse.GetResponseStream()))
                    {
                        soapResult = rd.ReadToEnd();
                        //Session["CompanyResp"] = soapResult;
                    }
                }

                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(soapResult);
                XmlNodeList nodeList = xmlDoc.GetElementsByTagName("GetCompanyFromNumberResponse");
                string json = "";
                foreach (XmlNode node in nodeList)
                {
                    json = node.InnerText;

                    try
                    {
                        CompanyInfo item = JsonConvert.DeserializeObject<CompanyInfo>(json);
                        dt.Rows.Add(item.company_name, item.company_number, item.registered_office_address.address_line_1, item.registered_office_address.locality, item.registered_office_address.postal_code);
                    }
                    catch
                    {
                        //The underlying connection was closed: An unexpected error occurred on a send.
                        //div_error.Style.Add("display", "block");
                        //errorlog.InnerHtml = json;
                    }
                }

            }
            catch (Exception exc)
            {

            }
            return dt;
        }
        private static HttpWebRequest CreateWebRequest(string url, string action)
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.Headers.Add("SOAPAction", action);
            webRequest.ContentType = "text/xml;charset=\"utf-8\"";
            webRequest.Accept = "text/xml";
            webRequest.Method = "POST";
            return webRequest;
        }
        private static void InsertSoapEnvelopeIntoWebRequest(XmlDocument soapEnvelopeXml, HttpWebRequest webRequest)
        {
            /* Page page = HttpContext.Current.CurrentHandler as Page;
             try
             {
                 using (Stream stream = webRequest.GetRequestStream())
                 {
                     soapEnvelopeXml.Save(stream);
                 }
             }
             catch (Exception ex)
             {
                 HttpContext.Current.Session["excweb"] = ex;               
             }*/
        }
        public class registered_office_address
        {
            public string address_line_1 { get; set; }
            public string address_line_2 { get; set; }
            public string postal_code { get; set; }
            public string region { get; set; }
            public string locality { get; set; }
        }

        public class foreign_company_details
        {
            public String company_type;
            public String registration_number;
            public String company_number;
        }
        public class CompanyInfo
        {
            public string company_name;
            public String company_number;
            public registered_office_address registered_office_address;
        }
        #region Comment
        //#region Execute Scalar

        ///// <summary>
        ///// Get single column Value with parameters.
        ///// </summary>
        ///// <param name="query"></param>
        ///// <param name="MySqlParameter"></param>
        ///// <returns></returns>
        //public object executeScalarWithQuery(String query, MySqlParameter[] MySqlParameter)
        //{
        //    MySqlCommand cmd = new MySqlCommand();
        //    try
        //    {
        //        cmd.Connection = OpenConnection();
        //        cmd.Parameters.AddRange(MySqlParameter);
        //        cmd.CommandText = query;
        //        //myAdapter.InsertCommand = myCommand;
        //        object objColoumVal = cmd.ExecuteScalar();
        //        return objColoumVal;
        //        //cmd.ExecuteNonQuery();
        //    }
        //    catch (Exception ex)
        //    {
        //        //Console.Write("Error - Connection.executeInsertQuery - Query: " + _query + " \nException: \n" + e.StackTrace.ToString());
        //        return false;
        //    }
        //    finally
        //    {
        //        if (cmd != null)
        //        {
        //            cmd.Dispose(); cmd = null;
        //        }
        //        CloseConnection();
        //    }

        //}

        ///// <summary>
        ///// Get single column Value without parameters.
        ///// </summary>
        ///// <param name="query"></param>
        ///// <param name="MySqlParameter"></param>
        ///// <returns></returns>
        //public object executeScalarWithQuery(String query)
        //{
        //    MySqlCommand cmd = new MySqlCommand();
        //    try
        //    {
        //        cmd.Connection = OpenConnection();
        //        cmd.CommandText = query;
        //        //cmd.Parameters.AddRange(MySqlParameter);
        //        //myAdapter.InsertCommand = myCommand;
        //        object objColoumVal = cmd.ExecuteScalar();
        //        return objColoumVal;
        //        //cmd.ExecuteNonQuery();
        //    }
        //    catch (Exception e)
        //    {
        //        //Console.Write("Error - Connection.executeInsertQuery - Query: " + _query + " \nException: \n" + e.StackTrace.ToString());
        //        return false;
        //    }
        //    finally
        //    {
        //        if (cmd != null)
        //        {
        //            cmd.Dispose(); cmd = null;
        //        }
        //        CloseConnection();
        //    }

        //}



        ///// <summary>
        ///// Get single procedure Value with parameters.
        ///// </summary>
        ///// <param name="query"></param>
        ///// <param name="MySqlParameter"></param>
        ///// <returns></returns>
        //public object executeScalarWithProcedure(String prcName, MySqlParameter[] MySqlParameter)
        //{
        //    MySqlCommand cmd = new MySqlCommand();
        //    try
        //    {
        //        cmd.Connection = OpenConnection();
        //        cmd.Parameters.AddRange(MySqlParameter);
        //        cmd.CommandText = prcName;

        //        object objColoumVal = cmd.ExecuteScalar();
        //        return objColoumVal;

        //    }
        //    catch (Exception ex)
        //    {

        //        return false;
        //    }
        //    finally
        //    {
        //        if (cmd != null)
        //        {
        //            cmd.Dispose(); cmd = null;
        //        }
        //        CloseConnection();
        //    }

        //}

        ///// <summary>
        ///// Get single column Value without parameters.
        ///// </summary>
        ///// <param name="query"></param>
        ///// <param name="MySqlParameter"></param>
        ///// <returns></returns>
        //public object executeScalarWithProcedure(String prcName)
        //{
        //    MySqlCommand cmd = new MySqlCommand();
        //    try
        //    {
        //        cmd.Connection = OpenConnection();
        //        cmd.CommandText = prcName;
        //        object objColoumVal = cmd.ExecuteScalar();
        //        return objColoumVal;

        //    }
        //    catch (Exception e)
        //    {

        //        return false;
        //    }
        //    finally
        //    {
        //        if (cmd != null)
        //        {
        //            cmd.Dispose(); cmd = null;
        //        }
        //        CloseConnection();
        //    }

        //}

        //#endregion


        //#region for Get Data from Table

        ///// <summary>
        ///// Return databable
        ///// </summary>
        ///// <param name="query"></param>
        ///// <param name="MySqlParameter"></param>
        ///// <returns></returns>
        //public DataTable GetDataTableWithQuery(String query, MySqlParameter[] MySqlParameter)
        //{
        //    MySqlCommand cmd = new MySqlCommand();

        //    MySqlDataReader adpt = new MySqlDataReader();
        //    DataTable DataTable = new DataTable();
        //    //dataTable = null;
        //    DataSet ds = new DataSet();
        //    try
        //    {
        //        cmd.Connection = OpenConnection();
        //        cmd.Parameters.AddRange(MySqlParameter);
        //        // query += " and ud.login_name='" + MySqlParameter[0].Value + "' and ud.pwd='" + MySqlParameter[1].Value + "'";
        //        cmd.CommandText = query;
        //        cmd.ExecuteNonQuery();
        //        adpt.SelectCommand = cmd;
        //        adpt.Fill(ds);
        //        DataTable = ds.Tables[0];
        //        return DataTable;
        //    }
        //    catch (SqlException exm)
        //    {
        //        throw exm;
        //    }
        //    catch (Exception e)
        //    {
        //        //Console.Write("Error - Connection.executeSelectQuery - Query: " + _query + " \nException: " + e.StackTrace.ToString());                
        //        return null;
        //    }
        //    finally
        //    {
        //        if (cmd != null)
        //        {
        //            cmd.Dispose(); cmd = null;
        //        }

        //        if (adpt != null)
        //        {
        //            adpt.Dispose(); adpt = null;
        //        }

        //        if (ds != null)
        //        {
        //            ds.Dispose(); ds = null;
        //        }

        //        CloseConnection();

        //    }

        //}

        ///// <summary>
        ///// Return databable
        ///// </summary>
        ///// <param name="query"></param>
        ///// <param name="MySqlParameter"></param>
        ///// <returns></returns>
        //public DataTable GetDataTableWithQuery(String query)
        //{
        //    MySqlCommand cmd = new MySqlCommand();
        //    MySqlDataReader adpt = new MySqlDataReader();
        //    DataTable DataTable = new DataTable();
        //    //dataTable = null;
        //    DataSet ds = new DataSet();
        //    try
        //    {

        //        cmd.Connection = OpenConnection();
        //        cmd.CommandText = query;
        //        //cmd.Parameters.AddRange(MySqlParameter);
        //        cmd.ExecuteNonQuery();
        //        adpt.SelectCommand = cmd;
        //        adpt.Fill(ds);
        //        DataTable = ds.Tables[0];
        //        return DataTable;
        //    }
        //    catch (Exception e)
        //    {
        //        //Console.Write("Error - Connection.executeSelectQuery - Query: " + _query + " \nException: " + e.StackTrace.ToString());
        //        return null;
        //    }
        //    finally
        //    {
        //        if (cmd != null)
        //        {
        //            cmd.Dispose(); cmd = null;
        //        }

        //        if (adpt != null)
        //        {
        //            adpt.Dispose(); adpt = null;
        //        }

        //        if (ds != null)
        //        {
        //            ds.Dispose(); ds = null;
        //        }

        //        CloseConnection();

        //    }

        //}

        ///// <summary>
        ///// Return Datatable
        ///// </summary>
        ///// <param name="prcName"></param>
        ///// <param name="MySqlParameter"></param>
        ///// <returns></returns>
        //public DataTable GetDataTableWithProcedure(String prcName, MySqlParameter[] MySqlParameter)
        //{
        //    MySqlCommand cmd = new MySqlCommand();
        //    MySqlDataReader adpt = new MySqlDataReader();
        //    DataTable DataTable = new DataTable();
        //    //dataTable = null;
        //    DataSet ds = new DataSet();
        //    try
        //    {

        //        cmd.Connection = OpenConnection();
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.CommandText = prcName;
        //        cmd.Parameters.AddRange(MySqlParameter);
        //        cmd.ExecuteNonQuery();
        //        adpt.SelectCommand = cmd;
        //        adpt.Fill(ds);
        //        DataTable = ds.Tables[0];
        //        return DataTable;
        //    }
        //    catch (Exception ex)
        //    {
        //        //Console.Write("Error - Connection.executeSelectQuery - Query: " + _query + " \nException: " + e.StackTrace.ToString());
        //        throw ex;
        //    }
        //    finally
        //    {
        //        if (cmd != null)
        //        {
        //            cmd.Dispose(); cmd = null;
        //        }

        //        if (adpt != null)
        //        {
        //            adpt.Dispose(); adpt = null;
        //        }

        //        if (ds != null)
        //        {
        //            ds.Dispose(); ds = null;
        //        }

        //        CloseConnection();

        //    }

        //}

        ///// <summary>
        ///// Return Datatable
        ///// </summary>
        ///// <param name="prcName"></param>
        ///// <returns></returns>
        //public DataTable GetDataTableWithProcedure(String prcName)
        //{
        //    MySqlCommand cmd = new MySqlCommand();
        //    MySqlDataReader adpt = new MySqlDataReader();
        //    DataTable DataTable = new DataTable();
        //    //dataTable = null;
        //    DataSet ds = new DataSet();
        //    try
        //    {

        //        cmd.Connection = OpenConnection();
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.CommandText = prcName;
        //        //cmd.Parameters.AddRange(MySqlParameter);
        //        cmd.ExecuteNonQuery();
        //        adpt.SelectCommand = cmd;
        //        adpt.Fill(ds);
        //        DataTable = ds.Tables[0];
        //        return DataTable;
        //    }
        //    catch (Exception e)
        //    {
        //        //Console.Write("Error - Connection.executeSelectQuery - Query: " + _query + " \nException: " + e.StackTrace.ToString());
        //        return null;
        //    }
        //    finally
        //    {
        //        if (cmd != null)
        //        {
        //            cmd.Dispose(); cmd = null;
        //        }

        //        if (adpt != null)
        //        {
        //            adpt.Dispose(); adpt = null;
        //        }

        //        if (ds != null)
        //        {
        //            ds.Dispose(); ds = null;
        //        }

        //        CloseConnection();

        //    }

        //}

        //#endregion



        //#region Get Data set 
        ///// <summary>
        ///// Return Data Set
        ///// </summary>
        ///// <param name="query"></param>
        ///// <param name="MySqlParameter"></param>
        ///// <returns></returns>
        //public DataSet GetDatasetWithQuery(String query, MySqlParameter[] MySqlParameter)
        //{
        //    MySqlCommand cmd = new MySqlCommand();

        //    MySqlDataReader adpt = new MySqlDataReader();


        //    DataSet ds = new DataSet();
        //    try
        //    {
        //        cmd.Connection = OpenConnection();
        //        cmd.Parameters.AddRange(MySqlParameter);
        //        // query += " and ud.login_name='" + MySqlParameter[0].Value + "' and ud.pwd='" + MySqlParameter[1].Value + "'";
        //        cmd.CommandText = query;
        //        cmd.ExecuteNonQuery();
        //        adpt.SelectCommand = cmd;
        //        adpt.Fill(ds);

        //        return ds;
        //    }
        //    catch (SqlException exm)
        //    {
        //        throw exm;
        //    }
        //    catch (Exception e)
        //    {

        //        return null;
        //    }
        //    finally
        //    {
        //        if (cmd != null)
        //        {
        //            cmd.Dispose(); cmd = null;
        //        }

        //        if (adpt != null)
        //        {
        //            adpt.Dispose(); adpt = null;
        //        }

        //        if (ds != null)
        //        {
        //            ds.Dispose(); ds = null;
        //        }

        //        CloseConnection();

        //    }

        //}

        ///// <summary>
        ///// Return Data Set
        ///// </summary>
        ///// <param name="query"></param>
        ///// <param name="MySqlParameter"></param>
        ///// <returns></returns>
        //public DataSet GetDatasetWithQuery(String query)
        //{
        //    MySqlCommand cmd = new MySqlCommand();
        //    MySqlDataReader adpt = new MySqlDataReader();

        //    //dataTable = null;
        //    DataSet ds = new DataSet();
        //    try
        //    {

        //        cmd.Connection = OpenConnection();
        //        cmd.CommandText = query;
        //        //cmd.Parameters.AddRange(MySqlParameter);
        //        cmd.ExecuteNonQuery();
        //        adpt.SelectCommand = cmd;
        //        adpt.Fill(ds);

        //        return ds;
        //    }
        //    catch (Exception ex)
        //    {

        //        return null;
        //    }
        //    finally
        //    {
        //        if (cmd != null)
        //        {
        //            cmd.Dispose(); cmd = null;
        //        }

        //        if (adpt != null)
        //        {
        //            adpt.Dispose(); adpt = null;
        //        }

        //        if (ds != null)
        //        {
        //            ds.Dispose(); ds = null;
        //        }

        //        CloseConnection();

        //    }

        //}

        ///// <summary>
        ///// Return Data Set
        ///// </summary>
        ///// <param name="prcName"></param>
        ///// <param name="MySqlParameter"></param>
        ///// <returns></returns>
        //public DataSet GetDatasetWithProcedure(String prcName, MySqlParameter[] MySqlParameter)
        //{
        //    MySqlCommand cmd = new MySqlCommand();
        //    MySqlDataReader adpt = new MySqlDataReader();

        //    DataSet ds = new DataSet();
        //    try
        //    {

        //        cmd.Connection = OpenConnection();
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.CommandText = prcName;
        //        cmd.Parameters.AddRange(MySqlParameter);
        //        cmd.ExecuteNonQuery();
        //        adpt.SelectCommand = cmd;
        //        adpt.Fill(ds);

        //        return ds;
        //    }
        //    catch (Exception ex)
        //    {
        //        return null;
        //    }
        //    finally
        //    {
        //        if (cmd != null)
        //        {
        //            cmd.Dispose(); cmd = null;
        //        }

        //        if (adpt != null)
        //        {
        //            adpt.Dispose(); adpt = null;
        //        }

        //        if (ds != null)
        //        {
        //            ds.Dispose(); ds = null;
        //        }

        //        CloseConnection();

        //    }

        //}

        ///// <summary>
        ///// Return Data Set
        ///// </summary>
        ///// <param name="prcName"></param>
        ///// <returns></returns>
        //public DataSet GetDatasetWithProcedure(String prcName)
        //{
        //    MySqlCommand cmd = new MySqlCommand();
        //    MySqlDataReader adpt = new MySqlDataReader();

        //    DataSet ds = new DataSet();
        //    try
        //    {

        //        cmd.Connection = OpenConnection();
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.CommandText = prcName;

        //        cmd.ExecuteNonQuery();
        //        adpt.SelectCommand = cmd;
        //        adpt.Fill(ds);

        //        return ds;
        //    }
        //    catch (Exception e)
        //    {

        //        return null;
        //    }
        //    finally
        //    {
        //        if (cmd != null)
        //        {
        //            cmd.Dispose(); cmd = null;
        //        }

        //        if (adpt != null)
        //        {
        //            adpt.Dispose(); adpt = null;
        //        }

        //        if (ds != null)
        //        {
        //            ds.Dispose(); ds = null;
        //        }

        //        CloseConnection();

        //    }

        //}

        //#endregion



        //#region For Insert update Delete Data Table



        ///// <summary>
        ///// Execute insert query.
        ///// </summary>
        ///// <param name="query"></param>
        ///// <param name="MySqlParameter"></param>
        ///// <returns></returns>
        //public bool executeQuery(String query, MySqlParameter[] MySqlParameter)
        //{
        //    MySqlCommand cmd = new MySqlCommand();
        //    try
        //    {
        //        cmd.Connection = OpenConnection();
        //        cmd.CommandText = query;
        //        cmd.Parameters.AddRange(MySqlParameter);
        //        //myAdapter.InsertCommand = myCommand;
        //        cmd.ExecuteNonQuery();
        //        return true;
        //        //cmd.ExecuteNonQuery();
        //    }
        //    catch (Exception e)
        //    {
        //        //Console.Write("Error - Connection.executeInsertQuery - Query: " + _query + " \nException: \n" + e.StackTrace.ToString());
        //        return false;
        //    }
        //    finally
        //    {
        //        if (cmd != null)
        //        {
        //            cmd.Dispose(); cmd = null;
        //        }
        //        CloseConnection();
        //    }

        //}



        ///// <summary>
        ///// Execute insert query.
        ///// </summary>
        ///// <param name="query"></param>
        ///// <param name="MySqlParameter"></param>
        ///// <returns></returns>
        //public bool executeQuery(String query)
        //{
        //    MySqlCommand cmd = new MySqlCommand();
        //    try
        //    {
        //        cmd.Connection = OpenConnection();
        //        cmd.CommandText = query;

        //        cmd.ExecuteNonQuery();
        //        return true;

        //    }
        //    catch (Exception e)
        //    {

        //        return false;
        //    }
        //    finally
        //    {
        //        if (cmd != null)
        //        {
        //            cmd.Dispose(); cmd = null;
        //        }
        //        CloseConnection();
        //    }

        //}




        ///// <summary>
        ///// Execute Insert procedure.
        ///// </summary>
        ///// <param name="prcName"></param>
        ///// <param name="MySqlParameter"></param>
        ///// <returns></returns>
        //public bool executeProcedure(String prcName, MySqlParameter[] MySqlParameter)
        //{
        //    MySqlCommand cmd = new MySqlCommand();
        //    try
        //    {
        //        cmd.Connection = OpenConnection();
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.CommandText = prcName;
        //        cmd.Parameters.AddRange(MySqlParameter);

        //        cmd.ExecuteNonQuery();
        //        return true;

        //    }
        //    catch (Exception e)
        //    {

        //        return false;
        //    }
        //    finally
        //    {
        //        if (cmd != null)
        //        {
        //            cmd.Dispose(); cmd = null;
        //        }
        //        CloseConnection();
        //    }
        //}


        ///// <summary>
        ///// Execute Insert procedure.
        ///// </summary>
        ///// <param name="prcName"></param>
        ///// <param name="MySqlParameter"></param>
        ///// <returns></returns>
        //public bool executeProcedure(String prcName)
        //{
        //    MySqlCommand cmd = new MySqlCommand();
        //    try
        //    {
        //        cmd.Connection = OpenConnection();
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.CommandText = prcName;

        //        cmd.ExecuteNonQuery();
        //        return true;

        //    }
        //    catch (Exception e)
        //    {

        //        return false;
        //    }
        //    finally
        //    {
        //        if (cmd != null)
        //        {
        //            cmd.Dispose(); cmd = null;
        //        }
        //        CloseConnection();
        //    }
        //}
        //#endregion 
        #endregion


        #region private utility methods & constructors

        // Since this class provides only static methods, make the default constructor private to prevent 
        // instances from being created with "new SqlHelper()"


        /// <summary>
        /// This method is used to attach array of SqlParameters to a MySqlCommand.
        /// 
        /// This method will assign a value of DbNull to any parameter with a direction of
        /// InputOutput and a value of null.  
        /// 
        /// This behavior will prevent default values from being used, but
        /// this will be the less common case than an intended pure output parameter (derived as InputOutput)
        /// where the user provided no input value.
        /// </summary>
        /// <param name="command">The command to which the parameters will be added</param>
        /// <param name="commandParameters">An array of SqlParameters to be added to command</param>
        private static void AttachParameters(MySqlCommand command, MySqlParameter[] commandParameters)
        {
            if (command == null) throw new ArgumentNullException("command");
            if (commandParameters != null)
            {
                foreach (MySqlParameter p in commandParameters)
                {
                    if (p != null)
                    {
                        // Check for derived output value with no value assigned
                        if ((p.Direction == ParameterDirection.InputOutput ||
                            p.Direction == ParameterDirection.Input || p.Direction == ParameterDirection.Output) &&
                            (p.Value == null))
                        {
                            p.Value = DBNull.Value;
                        }
                        if (p.Direction == ParameterDirection.InputOutput)
                        {
                            p.Value = DBNull.Value;
                        }
                        command.Parameters.Add(p);
                    }
                }
            }
        }

        /// <summary>
        /// This method assigns dataRow column values to an array of SqlParameters
        /// </summary>
        /// <param name="commandParameters">Array of SqlParameters to be assigned values</param>
        /// <param name="dataRow">The dataRow used to hold the stored procedure's parameter values</param>
        private static void AssignParameterValues(MySqlParameter[] commandParameters, DataRow dataRow)
        {
            if ((commandParameters == null) || (dataRow == null))
            {
                // Do nothing if we get no data
                return;
            }

            int i = 0;
            // Set the parameters values
            foreach (MySqlParameter commandParameter in commandParameters)
            {
                // Check the parameter name
                if (commandParameter.ParameterName == null ||
                    commandParameter.ParameterName.Length <= 1)
                    throw new Exception(
                        string.Format(
                            "Please provide a valid parameter name on the parameter #{0}, the ParameterName property has the following value: '{1}'.",
                            i, commandParameter.ParameterName));
                if (dataRow.Table.Columns.IndexOf(commandParameter.ParameterName.Substring(1)) != -1)
                    commandParameter.Value = dataRow[commandParameter.ParameterName.Substring(1)];
                i++;
            }
        }

        /// <summary>
        /// This method assigns an array of values to an array of SqlParameters
        /// </summary>
        /// <param name="commandParameters">Array of SqlParameters to be assigned values</param>
        /// <param name="parameterValues">Array of objects holding the values to be assigned</param>
        private static void AssignParameterValues(MySqlParameter[] commandParameters, object[] parameterValues)
        {
            if ((commandParameters == null) || (parameterValues == null))
            {
                // Do nothing if we get no data
                return;
            }

            // We must have the same number of values as we pave parameters to put them in
            if (commandParameters.Length != parameterValues.Length)
            {
                throw new ArgumentException("Parameter count does not match Parameter Value count.");
            }

            // Iterate through the SqlParameters, assigning the values from the corresponding position in the 
            // value array
            for (int i = 0, j = commandParameters.Length; i < j; i++)
            {
                // If the current array value derives from IDbDataParameter, then assign its Value property
                if (parameterValues[i] is IDbDataParameter)
                {
                    IDbDataParameter paramInstance = (IDbDataParameter)parameterValues[i];
                    if (paramInstance.Value == null)
                    {
                        commandParameters[i].Value = DBNull.Value;
                    }
                    else
                    {
                        commandParameters[i].Value = paramInstance.Value;
                    }
                }
                else if (parameterValues[i] == null)
                {
                    commandParameters[i].Value = DBNull.Value;
                }
                else
                {
                    commandParameters[i].Value = parameterValues[i];
                }
            }
        }

        /// <summary>
        /// This method opens (if necessary) and assigns a connection, transaction, command type and parameters 
        /// to the provided command
        /// </summary>
        /// <param name="command">The MySqlCommand to be prepared</param>
        /// <param name="connection">A valid MySqlConnection, on which to execute this command</param>
        /// <param name="transaction">A valid MySqlTransaction, or 'null'</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">The stored procedure name or T-SQL command</param>
        /// <param name="commandParameters">An array of SqlParameters to be associated with the command or 'null' if no parameters are required</param>
        /// <param name="mustCloseConnection"><c>true</c> if the connection was opened by the method, otherwose is false.</param>
        private static void PrepareCommand(MySqlCommand command, MySqlConnection connection, MySqlTransaction transaction, CommandType commandType, string commandText, MySqlParameter[] commandParameters, out bool mustCloseConnection)
        {
            if (command == null) throw new ArgumentNullException("command");
            if (commandText == null || commandText.Length == 0) throw new ArgumentNullException("commandText");

            // If the provided connection is not open, we will open it
            if (connection.State != ConnectionState.Open)
            {
                mustCloseConnection = true;
                connection.Open();
            }
            else
            {
                mustCloseConnection = true;
            }

            // Associate the connection with the command
            command.Connection = connection;

            // Set the command text (stored procedure name or SQL statement)
            command.CommandText = commandText;

            // If we were provided a transaction, assign it
            if (transaction != null)
            {
                if (transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
                command.Transaction = transaction;
            }

            // Set the command type
            command.CommandType = commandType;

            // Attach the command parameters if they are provided
            if (commandParameters != null)
            {
                AttachParameters(command, commandParameters);
            }
            return;
        }

        #endregion private utility methods & constructors






        #region ExecuteNonQuery

        /// <summary>
        /// Execute a MySqlCommand (that returns no resultset and takes no parameters) against the database specified in 
        /// the connection string
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  int result = ExecuteNonQuery(connString, CommandType.StoredProcedure, "PublishOrders");
        /// </remarks>
        /// <param name="connectionString">A valid connection string for a MySqlConnection</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">The stored procedure name or T-SQL command</param>
        /// <returns>An int representing the number of rows affected by the command</returns>
        public int ExecuteNonQuery(string connectionString, CommandType commandType, string commandText)
        {
            // Pass through the call providing null for the set of SqlParameters
            return ExecuteNonQuery(connectionString, commandType, commandText, (MySqlParameter[])null);
        }

        /// <summary>
        /// Execute a MySqlCommand (that returns no resultset) against the database specified in the connection string 
        /// using the provided parameters
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  int result = ExecuteNonQuery(connString, CommandType.StoredProcedure, "PublishOrders", new MySqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connectionString">A valid connection string for a MySqlConnection</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">The stored procedure name or T-SQL command</param>
        /// <param name="commandParameters">An array of SqlParamters used to execute the command</param>
        /// <returns>An int representing the number of rows affected by the command</returns>
        public int ExecuteNonQuery(string connectionString, CommandType commandType, string commandText, params MySqlParameter[] commandParameters)
        {
            if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");

            // Create & open a MySqlConnection, and dispose of it after we are done
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                // Call the overload that takes a connection in place of the connection string
                return ExecuteNonQuery(connection, commandType, commandText, commandParameters);
            }
        }

        /// <summary>
        /// Execute a stored procedure via a MySqlCommand (that returns no resultset) against the database specified in 
        /// the connection string using the provided parameter values.  This method will query the database to discover the parameters for the 
        /// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
        /// </summary>
        /// <remarks>
        /// This method provides no access to output parameters or the stored procedure's return value parameter.
        /// 
        /// e.g.:  
        ///  int result = ExecuteNonQuery(connString, "PublishOrders", 24, 36);
        /// </remarks>
        /// <param name="connectionString">A valid connection string for a MySqlConnection</param>
        /// <param name="spName">The name of the stored prcedure</param>
        /// <param name="parameterValues">An array of objects to be assigned as the input values of the stored procedure</param>
        /// <returns>An int representing the number of rows affected by the command</returns>
        public int ExecuteNonQuery(string connectionString, string spName, params object[] parameterValues)
        {
            if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

            // If we receive parameter values, we need to figure out where they go
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                // Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                MySqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connectionString, spName);

                // Assign the provided values to these parameters based on parameter order
                AssignParameterValues(commandParameters, parameterValues);

                // Call the overload that takes an array of SqlParameters
                return ExecuteNonQuery(connectionString, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                // Otherwise we can just call the SP without params
                return ExecuteNonQuery(connectionString, CommandType.StoredProcedure, spName);
            }
        }



        /// <summary>
        /// Execute a MySqlCommand (that returns no resultset and takes no parameters) against the provided MySqlConnection. 
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  int result = ExecuteNonQuery(conn, CommandType.StoredProcedure, "PublishOrders");
        /// </remarks>
        /// <param name="connection">A valid MySqlConnection</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">The stored procedure name or T-SQL command</param>
        /// <returns>An int representing the number of rows affected by the command</returns>
        private int ExecuteNonQuery(MySqlConnection connection, CommandType commandType, string commandText)
        {
            // Pass through the call providing null for the set of SqlParameters
            return ExecuteNonQuery(connection, commandType, commandText, (MySqlParameter[])null);
        }

        /// <summary>
        /// Execute a MySqlCommand (that returns no resultset) against the specified MySqlConnection 
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  int result = ExecuteNonQuery(conn, CommandType.StoredProcedure, "PublishOrders", new MySqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connection">A valid MySqlConnection</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">The stored procedure name or T-SQL command</param>
        /// <param name="commandParameters">An array of SqlParamters used to execute the command</param>
        /// <returns>An int representing the number of rows affected by the command</returns>
        private int ExecuteNonQuery(MySqlConnection connection, CommandType commandType, string commandText, params MySqlParameter[] commandParameters)
        {
            if (connection == null) throw new ArgumentNullException("connection");
            int retval = 0;
            // Create a command and prepare it for execution
            MySqlCommand cmd = new MySqlCommand();
            bool mustCloseConnection = false;
            try
            {
                PrepareCommand(cmd, connection, (MySqlTransaction)null, commandType, commandText, commandParameters, out mustCloseConnection);

                // Finally, execute the command
                retval = cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                mustCloseConnection = true;
            }
            finally { mustCloseConnection = true; }

            // Detach the SqlParameters from the command object, so they can be used again
            cmd.Parameters.Clear();
            if (mustCloseConnection)
            {
                connection.Close();
                // MySqlConnection.ClearAllPools();
            }
            return retval;
        }

        private string ExecuteNonQuery(MySqlConnection connection, CommandType commandType, string commandText, string strIPAddress, params MySqlParameter[] commandParameters)
        {
            if (connection == null) throw new ArgumentNullException("connection");

            // Create a command and prepare it for execution
            MySqlCommand cmd = new MySqlCommand();
            bool mustCloseConnection = false;
            try
            {
                PrepareCommand(cmd, connection, (MySqlTransaction)null, commandType, commandText, commandParameters, out mustCloseConnection);

                // Finally, execute the command
                cmd.ExecuteNonQuery();
                strIPAddress = cmd.Parameters["@IPAddress"].Value.ToString();
            }
            catch (Exception ex)
            {
                mustCloseConnection = true;
            }
            finally { mustCloseConnection = true; }
            // Detach the SqlParameters from the command object, so they can be used again
            cmd.Parameters.Clear();
            if (mustCloseConnection)
            {
                connection.Close();
                //  MySqlConnection.ClearAllPools();
            }
            return strIPAddress;
        }

        #region Added by Supriya on 25 May 2012
        private string ExecuteNonQuery(MySqlConnection connection, CommandType commandType, string commandText, string strOutputVal, string output, params MySqlParameter[] commandParameters)
        {
            if (connection == null) throw new ArgumentNullException("connection");

            // Create a command and prepare it for execution
            MySqlCommand cmd = new MySqlCommand();
            bool mustCloseConnection = false;
            try
            {
                PrepareCommand(cmd, connection, (MySqlTransaction)null, commandType, commandText, commandParameters, out mustCloseConnection);

                // Finally, execute the command
                cmd.ExecuteNonQuery();
                strOutputVal = cmd.Parameters["" + output + ""].Value.ToString();
            }
            catch (Exception ex)
            {
                mustCloseConnection = true;
            }
            finally { mustCloseConnection = true; }
            // Detach the SqlParameters from the command object, so they can be used again
            cmd.Parameters.Clear();
            if (mustCloseConnection)
            {
                connection.Close();
                //  MySqlConnection.ClearAllPools();
            }
            return strOutputVal;
        }
        #endregion

        /// <summary>
        /// Execute a stored procedure via a MySqlCommand (that returns no resultset) against the specified MySqlConnection 
        /// using the provided parameter values.  This method will query the database to discover the parameters for the 
        /// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
        /// </summary>
        /// <remarks>
        /// This method provides no access to output parameters or the stored procedure's return value parameter.
        /// 
        /// e.g.:  
        ///  int result = ExecuteNonQuery(conn, "PublishOrders", 24, 36);
        /// </remarks>
        /// <param name="connection">A valid MySqlConnection</param>
        /// <param name="spName">The name of the stored procedure</param>
        /// <param name="parameterValues">An array of objects to be assigned as the input values of the stored procedure</param>
        /// <returns>An int representing the number of rows affected by the command</returns>
        private int ExecuteNonQuery(MySqlConnection connection, string spName, params object[] parameterValues)
        {
            if (connection == null) throw new ArgumentNullException("connection");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

            // If we receive parameter values, we need to figure out where they go
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                // Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                MySqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connection, spName);

                // Assign the provided values to these parameters based on parameter order
                AssignParameterValues(commandParameters, parameterValues);

                // Call the overload that takes an array of SqlParameters
                return ExecuteNonQuery(connection, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                // Otherwise we can just call the SP without params
                return ExecuteNonQuery(connection, CommandType.StoredProcedure, spName);
            }
        }

        public string ExecuteNonQuery(MySqlConnection connection, string spName, string strLANWAN, params object[] parameterValues)
        {
            if (connection == null) throw new ArgumentNullException("connection");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

            // If we receive parameter values, we need to figure out where they go
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                // Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                MySqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connection, spName);

                // Assign the provided values to these parameters based on parameter order
                AssignParameterValues(commandParameters, parameterValues);

                // Call the overload that takes an array of SqlParameters
                return ExecuteNonQuery(connection, CommandType.StoredProcedure, spName, strLANWAN, commandParameters);
            }
            return "Fail";

        }


        #region Added by Supriya on 25 May 2012
        /// <summary>
        ///Execute procedure with output parameter
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="spName"></param>
        /// <param name="strOutputVal"></param>
        /// <param name="output">name of output parameter</param>
        /// <param name="parameterValues"></param>
        /// <returns></returns>
        public string ExecuteNonQuery(MySqlConnection connection, string spName, string strOutputVal, string stroutput, params object[] parameterValues)
        {
            if (connection == null) throw new ArgumentNullException("connection");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

            // If we receive parameter values, we need to figure out where they go
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                // Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                MySqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connection, spName);

                // Assign the provided values to these parameters based on parameter order
                AssignParameterValues(commandParameters, parameterValues);

                // Call the overload that takes an array of SqlParameters
                return ExecuteNonQuery(connection, CommandType.StoredProcedure, spName, strOutputVal, stroutput, commandParameters);
            }
            return "Fail";

        }
        #endregion


        /// <summary>
        /// Execute a MySqlCommand (that returns no resultset and takes no parameters) against the provided MySqlTransaction. 
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  int result = ExecuteNonQuery(trans, CommandType.StoredProcedure, "PublishOrders");
        /// </remarks>
        /// <param name="transaction">A valid MySqlTransaction</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">The stored procedure name or T-SQL command</param>
        /// <returns>An int representing the number of rows affected by the command</returns>
        public int ExecuteNonQuery(MySqlTransaction transaction, CommandType commandType, string commandText)
        {
            // Pass through the call providing null for the set of SqlParameters
            return ExecuteNonQuery(transaction, commandType, commandText, (MySqlParameter[])null);
        }

        /// <summary>
        /// Execute a MySqlCommand (that returns no resultset) against the specified MySqlTransaction
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  int result = ExecuteNonQuery(trans, CommandType.StoredProcedure, "GetOrders", new MySqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="transaction">A valid MySqlTransaction</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">The stored procedure name or T-SQL command</param>
        /// <param name="commandParameters">An array of SqlParamters used to execute the command</param>
        /// <returns>An int representing the number of rows affected by the command</returns>
        public int ExecuteNonQuery(MySqlTransaction transaction, CommandType commandType, string commandText, params MySqlParameter[] commandParameters)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            if (transaction != null && transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
            int retval = 0;
            // Create a command and prepare it for execution
            MySqlCommand cmd = new MySqlCommand();
            bool mustCloseConnection = false;
            //try
            //{
            PrepareCommand(cmd, transaction.Connection, transaction, commandType, commandText, commandParameters, out mustCloseConnection);

            // Finally, execute the command
            retval = cmd.ExecuteNonQuery();
            //}
            //catch (Exception ex)
            //{
            //    mustCloseConnection = true;
            //}
            // finally { mustCloseConnection = true; }
            // Detach the SqlParameters from the command object, so they can be used again
            cmd.Parameters.Clear();
            return retval;
        }

        /// <summary>
        /// Execute a stored procedure via a MySqlCommand (that returns no resultset) against the specified 
        /// MySqlTransaction using the provided parameter values.  This method will query the database to discover the parameters for the 
        /// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
        /// </summary>
        /// <remarks>
        /// This method provides no access to output parameters or the stored procedure's return value parameter.
        /// 
        /// e.g.:  
        ///  int result = ExecuteNonQuery(conn, trans, "PublishOrders", 24, 36);
        /// </remarks>
        /// <param name="transaction">A valid MySqlTransaction</param>
        /// <param name="spName">The name of the stored procedure</param>
        /// <param name="parameterValues">An array of objects to be assigned as the input values of the stored procedure</param>
        /// <returns>An int representing the number of rows affected by the command</returns>
        public int ExecuteNonQuery(MySqlTransaction transaction, string spName, params object[] parameterValues)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            if (transaction != null && transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

            // If we receive parameter values, we need to figure out where they go
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                // Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                MySqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(transaction.Connection, spName);

                // Assign the provided values to these parameters based on parameter order
                AssignParameterValues(commandParameters, parameterValues);

                // Call the overload that takes an array of SqlParameters
                return ExecuteNonQuery(transaction, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                // Otherwise we can just call the SP without params
                return ExecuteNonQuery(transaction, CommandType.StoredProcedure, spName);
            }
        }

        #endregion ExecuteNonQuery




        #region ExecuteDataset




        /// <summary>
        /// Execute a MySqlCommand (that returns a resultset and takes no parameters) against the database specified in 
        /// the connection string. 
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  DataSet ds = ExecuteDataset(connString, CommandType.StoredProcedure, "GetOrders");
        /// </remarks>
        /// <param name="connectionString">A valid connection string for a MySqlConnection</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">The stored procedure name or T-SQL command</param>
        /// <returns>A dataset containing the resultset generated by the command</returns>
        public DataSet ExecuteDataset(string connectionString, CommandType commandType, string commandText)
        {
            // Pass through the call providing null for the set of SqlParameters
            return ExecuteDataset(connectionString, commandType, commandText, (MySqlParameter[])null);
        }

        /// <summary>
        /// AK
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="commandType"></param>
        /// <param name="commandText"></param>
        /// <param name="commandParameters"></param>
        /// <param name="startIndex"></param>
        /// <param name="maxRecords"></param>
        /// <returns></returns>
        public DataSet ExecuteDataset(string connectionString, CommandType commandType, string commandText, int startIndex, int maxRecords, params MySqlParameter[] commandParameters)
        {
            if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");

            // Create & open a MySqlConnection, and dispose of it after we are done
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                // Call the overload that takes a connection in place of the connection string
                return ExecuteDataset(connection, commandType, commandText, commandParameters, startIndex, maxRecords);
            }
        }

        /// <summary>
        /// Execute a MySqlCommand (that returns a resultset) against the database specified in the connection string 
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  DataSet ds = ExecuteDataset(connString, CommandType.StoredProcedure, "GetOrders", new MySqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connectionString">A valid connection string for a MySqlConnection</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">The stored procedure name or T-SQL command</param>
        /// <param name="commandParameters">An array of SqlParamters used to execute the command</param>
        /// <returns>A dataset containing the resultset generated by the command</returns>
        public DataSet ExecuteDataset(string connectionString, CommandType commandType, string commandText, params MySqlParameter[] commandParameters)
        {
            if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");

            // Create & open a MySqlConnection, and dispose of it after we are done
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                // Call the overload that takes a connection in place of the connection string
                return ExecuteDataset(connection, commandType, commandText, commandParameters);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="spName"></param>
        /// <param name="parameterValues"></param>
        /// <returns></returns>
        public DataSet ExecuteDataset(string connectionString, string spName, int startIndex, int maxRecords, params object[] parameterValues)
        {
            if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

            // If we receive parameter values, we need to figure out where they go
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                // Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                MySqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connectionString, spName);

                // Assign the provided values to these parameters based on parameter order
                AssignParameterValues(commandParameters, parameterValues);

                // Call the overload that takes an array of SqlParameters
                return ExecuteDataset(connectionString, CommandType.StoredProcedure, spName, startIndex, maxRecords, commandParameters);
            }
            else
            {
                // Otherwise we can just call the SP without params
                return ExecuteDataset(connectionString, CommandType.StoredProcedure, spName);
            }
        }


        /// <summary>
        /// Execute a stored procedure via a MySqlCommand (that returns a resultset) against the database specified in 
        /// the connection string using the provided parameter values.  This method will query the database to discover the parameters for the 
        /// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
        /// </summary>
        /// <remarks>
        /// This method provides no access to output parameters or the stored procedure's return value parameter.
        /// 
        /// e.g.:  
        ///  DataSet ds = ExecuteDataset(connString, "GetOrders", 24, 36);
        /// </remarks>
        /// <param name="connectionString">A valid connection string for a MySqlConnection</param>
        /// <param name="spName">The name of the stored procedure</param>
        /// <param name="parameterValues">An array of objects to be assigned as the input values of the stored procedure</param>
        /// <returns>A dataset containing the resultset generated by the command</returns>
        public DataSet ExecuteDataset(string connectionString, string spName, params object[] parameterValues)
        {
            if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

            // If we receive parameter values, we need to figure out where they go
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                // Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                MySqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connectionString, spName);

                // Assign the provided values to these parameters based on parameter order
                AssignParameterValues(commandParameters, parameterValues);

                // Call the overload that takes an array of SqlParameters
                return ExecuteDataset(connectionString, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                // Otherwise we can just call the SP without params
                return ExecuteDataset(connectionString, CommandType.StoredProcedure, spName);
            }
        }





        /// <summary>
        /// Execute a MySqlCommand (that returns a resultset and takes no parameters) against the provided MySqlConnection. 
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  DataSet ds = ExecuteDataset(conn, CommandType.StoredProcedure, "GetOrders");
        /// </remarks>
        /// <param name="connection">A valid MySqlConnection</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">The stored procedure name or T-SQL command</param>
        /// <returns>A dataset containing the resultset generated by the command</returns>
        private DataSet ExecuteDataset(MySqlConnection connection, CommandType commandType, string commandText)
        {
            // Pass through the call providing null for the set of SqlParameters
            return ExecuteDataset(connection, commandType, commandText, (MySqlParameter[])null);
        }


        /// <summary>
        /// AK
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="commandType"></param>
        /// <param name="commandText"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        private DataSet ExecuteDataset(MySqlConnection connection, CommandType commandType, string commandText, MySqlParameter[] commandParameters, int startIndex, int maxRecords)
        {
            if (connection == null) throw new ArgumentNullException("connection");

            // Create a command and prepare it for execution
            MySqlCommand cmd = new MySqlCommand();
            bool mustCloseConnection = false;
            PrepareCommand(cmd, connection, (MySqlTransaction)null, commandType, commandText, commandParameters, out mustCloseConnection);

            // Create the DataAdapter & DataSet
            using (MySqlDataAdapter da = new MySqlDataAdapter(cmd))
            {
                DataSet ds = new DataSet();

                // Fill the DataSet using default values for DataTable names, etc
                da.Fill(ds, startIndex, maxRecords, "DataTable");

                // Detach the SqlParameters from the command object, so they can be used again
                cmd.Parameters.Clear();

                if (mustCloseConnection)
                {
                    connection.Close();
                    //  MySqlConnection.ClearAllPools();
                }

                // Return the dataset
                return ds;
            }
        }

        /// <summary>
        /// Execute a MySqlCommand (that returns a resultset) against the specified MySqlConnection 
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  DataSet ds = ExecuteDataset(conn, CommandType.StoredProcedure, "GetOrders", new MySqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connection">A valid MySqlConnection</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">The stored procedure name or T-SQL command</param>
        /// <param name="commandParameters">An array of SqlParamters used to execute the command</param>
        /// <returns>A dataset containing the resultset generated by the command</returns>
        private DataSet ExecuteDataset(MySqlConnection connection, CommandType commandType, string commandText, params MySqlParameter[] commandParameters)
        {
            if (connection == null) throw new ArgumentNullException("connection");

            // Create a command and prepare it for execution
            MySqlCommand cmd = new MySqlCommand();
            bool mustCloseConnection = false;
            PrepareCommand(cmd, connection, (MySqlTransaction)null, commandType, commandText, commandParameters, out mustCloseConnection);

            // Create the DataAdapter & DataSet
            using (MySqlDataAdapter da = new MySqlDataAdapter(cmd))
            {
                DataSet ds = new DataSet();

                // Fill the DataSet using default values for DataTable names, etc
                da.Fill(ds);

                // Detach the SqlParameters from the command object, so they can be used again
                cmd.Parameters.Clear();

                if (mustCloseConnection)
                {
                    connection.Close();
                    //   MySqlConnection.ClearAllPools();
                }

                // Return the dataset
                return ds;
            }
        }

        /// <summary>
        /// Execute a stored procedure via a MySqlCommand (that returns a resultset) against the specified MySqlConnection 
        /// using the provided parameter values.  This method will query the database to discover the parameters for the 
        /// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
        /// </summary>
        /// <remarks>
        /// This method provides no access to output parameters or the stored procedure's return value parameter.
        /// 
        /// e.g.:  
        ///  DataSet ds = ExecuteDataset(conn, "GetOrders", 24, 36);
        /// </remarks>
        /// <param name="connection">A valid MySqlConnection</param>
        /// <param name="spName">The name of the stored procedure</param>
        /// <param name="parameterValues">An array of objects to be assigned as the input values of the stored procedure</param>
        /// <returns>A dataset containing the resultset generated by the command</returns>
        private DataSet ExecuteDataset(MySqlConnection connection, string spName, params object[] parameterValues)
        {
            if (connection == null) throw new ArgumentNullException("connection");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

            // If we receive parameter values, we need to figure out where they go
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                // Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                MySqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connection, spName);

                // Assign the provided values to these parameters based on parameter order
                AssignParameterValues(commandParameters, parameterValues);

                // Call the overload that takes an array of SqlParameters
                return ExecuteDataset(connection, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                // Otherwise we can just call the SP without params
                return ExecuteDataset(connection, CommandType.StoredProcedure, spName);
            }
        }






        /// <summary>
        /// Execute a MySqlCommand (that returns a resultset and takes no parameters) against the provided MySqlTransaction. 
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  DataSet ds = ExecuteDataset(trans, CommandType.StoredProcedure, "GetOrders");
        /// </remarks>
        /// <param name="transaction">A valid MySqlTransaction</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">The stored procedure name or T-SQL command</param>
        /// <returns>A dataset containing the resultset generated by the command</returns>
        public DataSet ExecuteDataset(MySqlTransaction transaction, CommandType commandType, string commandText)
        {
            // Pass through the call providing null for the set of SqlParameters
            return ExecuteDataset(transaction, commandType, commandText, (MySqlParameter[])null);
        }

        /// <summary>
        /// Execute a MySqlCommand (that returns a resultset) against the specified MySqlTransaction
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  DataSet ds = ExecuteDataset(trans, CommandType.StoredProcedure, "GetOrders", new MySqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="transaction">A valid MySqlTransaction</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">The stored procedure name or T-SQL command</param>
        /// <param name="commandParameters">An array of SqlParamters used to execute the command</param>
        /// <returns>A dataset containing the resultset generated by the command</returns>
        public DataSet ExecuteDataset(MySqlTransaction transaction, CommandType commandType, string commandText, params MySqlParameter[] commandParameters)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            if (transaction != null && transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");

            // Create a command and prepare it for execution
            MySqlCommand cmd = new MySqlCommand();
            bool mustCloseConnection = false;
            PrepareCommand(cmd, transaction.Connection, transaction, commandType, commandText, commandParameters, out mustCloseConnection);

            // Create the DataAdapter & DataSet
            using (MySqlDataAdapter da = new MySqlDataAdapter(cmd))
            {
                DataSet ds = new DataSet();

                // Fill the DataSet using default values for DataTable names, etc
                da.Fill(ds);

                // Detach the SqlParameters from the command object, so they can be used again
                cmd.Parameters.Clear();

                // Return the dataset
                return ds;
            }
        }

        /// <summary>
        /// Execute a stored procedure via a MySqlCommand (that returns a resultset) against the specified 
        /// MySqlTransaction using the provided parameter values.  This method will query the database to discover the parameters for the 
        /// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
        /// </summary>
        /// <remarks>
        /// This method provides no access to output parameters or the stored procedure's return value parameter.
        /// 
        /// e.g.:  
        ///  DataSet ds = ExecuteDataset(trans, "GetOrders", 24, 36);
        /// </remarks>
        /// <param name="transaction">A valid MySqlTransaction</param>
        /// <param name="spName">The name of the stored procedure</param>
        /// <param name="parameterValues">An array of objects to be assigned as the input values of the stored procedure</param>
        /// <returns>A dataset containing the resultset generated by the command</returns>
        public DataSet ExecuteDataset(MySqlTransaction transaction, string spName, params object[] parameterValues)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            if (transaction != null && transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

            // If we receive parameter values, we need to figure out where they go
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                // Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                MySqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(transaction.Connection, spName);

                // Assign the provided values to these parameters based on parameter order
                AssignParameterValues(commandParameters, parameterValues);

                // Call the overload that takes an array of SqlParameters
                return ExecuteDataset(transaction, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                // Otherwise we can just call the SP without params
                return ExecuteDataset(transaction, CommandType.StoredProcedure, spName);
            }
        }

        #endregion ExecuteDataset





        #region ExecuteDatasetWithSchemaAction




        /// <summary>
        /// Execute a MySqlCommand (that returns a resultset and takes no parameters) against the database specified in 
        /// the connection string. 
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  DataSet ds = ExecuteDataset(connString, CommandType.StoredProcedure, "GetOrders");
        /// </remarks>
        /// <param name="connectionString">A valid connection string for a MySqlConnection</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">The stored procedure name or T-SQL command</param>
        /// <returns>A dataset containing the resultset generated by the command</returns>
        public DataSet ExecuteDatasetWithSchemaAction(string connectionString, CommandType commandType, string commandText, MissingSchemaAction missingSchemaAction)
        {
            // Pass through the call providing null for the set of SqlParameters
            return ExecuteDatasetWithSchemaAction(connectionString, commandType, commandText, missingSchemaAction, (MySqlParameter[])null);
        }

        /// <summary>
        /// Anil Chavan
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="commandType"></param>
        /// <param name="commandText"></param>
        /// <param name="commandParameters"></param>
        /// <param name="startIndex"></param>
        /// <param name="maxRecords"></param>
        /// <returns></returns>
        public DataSet ExecuteDatasetWithSchemaAction(string connectionString, CommandType commandType, string commandText, int startIndex, int maxRecords, MissingSchemaAction missingSchemaAction, params MySqlParameter[] commandParameters)
        {
            if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");

            // Create & open a MySqlConnection, and dispose of it after we are done
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                // Call the overload that takes a connection in place of the connection string
                return ExecuteDatasetWithSchemaAction(connection, commandType, commandText, missingSchemaAction, commandParameters, startIndex, maxRecords);
            }
        }

        /// <summary>
        /// Execute a MySqlCommand (that returns a resultset) against the database specified in the connection string 
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  DataSet ds = ExecuteDataset(connString, CommandType.StoredProcedure, "GetOrders", new MySqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connectionString">A valid connection string for a MySqlConnection</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">The stored procedure name or T-SQL command</param>
        /// <param name="commandParameters">An array of SqlParamters used to execute the command</param>
        /// <returns>A dataset containing the resultset generated by the command</returns>
        public DataSet ExecuteDatasetWithSchemaAction(string connectionString, CommandType commandType, string commandText, MissingSchemaAction missingSchemaAction, params MySqlParameter[] commandParameters)
        {
            if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");

            // Create & open a MySqlConnection, and dispose of it after we are done
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                // Call the overload that takes a connection in place of the connection string
                return ExecuteDatasetWithSchemaAction(connection, commandType, commandText, missingSchemaAction, commandParameters);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="spName"></param>
        /// <param name="parameterValues"></param>
        /// <returns></returns>
        public DataSet ExecuteDatasetWithSchemaAction(string connectionString, string spName, int startIndex, int maxRecords, MissingSchemaAction missingSchemaAction, object[] parameterValues)
        {
            if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

            // If we receive parameter values, we need to figure out where they go
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                // Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                MySqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connectionString, spName);

                // Assign the provided values to these parameters based on parameter order
                AssignParameterValues(commandParameters, parameterValues);

                // Call the overload that takes an array of SqlParameters
                return ExecuteDatasetWithSchemaAction(connectionString, CommandType.StoredProcedure, spName, startIndex, maxRecords, missingSchemaAction, commandParameters);
            }
            else
            {
                // Otherwise we can just call the SP without params
                return ExecuteDatasetWithSchemaAction(connectionString, CommandType.StoredProcedure, spName, missingSchemaAction);
            }
        }


        /// <summary>
        /// Execute a stored procedure via a MySqlCommand (that returns a resultset) against the database specified in 
        /// the connection string using the provided parameter values.  This method will query the database to discover the parameters for the 
        /// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
        /// </summary>
        /// <remarks>
        /// This method provides no access to output parameters or the stored procedure's return value parameter.
        /// 
        /// e.g.:  
        ///  DataSet ds = ExecuteDataset(connString, "GetOrders", 24, 36);
        /// </remarks>
        /// <param name="connectionString">A valid connection string for a MySqlConnection</param>
        /// <param name="spName">The name of the stored procedure</param>
        /// <param name="parameterValues">An array of objects to be assigned as the input values of the stored procedure</param>
        /// <returns>A dataset containing the resultset generated by the command</returns>
        public DataSet ExecuteDatasetWithSchemaAction(string connectionString, string spName, MissingSchemaAction missingSchemaAction, params object[] parameterValues)
        {
            if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

            // If we receive parameter values, we need to figure out where they go
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                // Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                MySqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connectionString, spName);

                // Assign the provided values to these parameters based on parameter order
                AssignParameterValues(commandParameters, parameterValues);

                // Call the overload that takes an array of SqlParameters
                return ExecuteDatasetWithSchemaAction(connectionString, CommandType.StoredProcedure, spName, missingSchemaAction, commandParameters);
            }
            else
            {
                // Otherwise we can just call the SP without params
                return ExecuteDatasetWithSchemaAction(connectionString, CommandType.StoredProcedure, spName, missingSchemaAction);
            }
        }





        /// <summary>
        /// Execute a MySqlCommand (that returns a resultset and takes no parameters) against the provided MySqlConnection. 
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  DataSet ds = ExecuteDataset(conn, CommandType.StoredProcedure, "GetOrders");
        /// </remarks>
        /// <param name="connection">A valid MySqlConnection</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">The stored procedure name or T-SQL command</param>
        /// <returns>A dataset containing the resultset generated by the command</returns>
        private DataSet ExecuteDatasetWithSchemaAction(MySqlConnection connection, CommandType commandType, string commandText, MissingSchemaAction missingSchemaAction)
        {
            // Pass through the call providing null for the set of SqlParameters
            return ExecuteDatasetWithSchemaAction(connection, commandType, commandText, missingSchemaAction, (MySqlParameter[])null);
        }


        /// <summary>
        /// Anil Chavan
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="commandType"></param>
        /// <param name="commandText"></param>
        /// <param name="missingSchemaAction"</param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        private DataSet ExecuteDatasetWithSchemaAction(MySqlConnection connection, CommandType commandType, string commandText, MissingSchemaAction missingSchemaAction, MySqlParameter[] commandParameters, int startIndex, int maxRecords)
        {
            if (connection == null) throw new ArgumentNullException("connection");

            // Create a command and prepare it for execution
            MySqlCommand cmd = new MySqlCommand();
            bool mustCloseConnection = false;
            PrepareCommand(cmd, connection, (MySqlTransaction)null, commandType, commandText, commandParameters, out mustCloseConnection);

            // Create the DataAdapter & DataSet
            using (MySqlDataAdapter da = new MySqlDataAdapter(cmd))
            {
                DataSet ds = new DataSet();

                // Fill the DataSet using default values for DataTable names, etc
                da.MissingSchemaAction = missingSchemaAction;
                da.Fill(ds, startIndex, maxRecords, "DataTable");

                // Detach the SqlParameters from the command object, so they can be used again
                cmd.Parameters.Clear();

                if (mustCloseConnection)
                {
                    connection.Close();
                    // MySqlConnection.ClearAllPools();
                }

                // Return the dataset
                return ds;
            }
        }

        /// <summary>
        /// Execute a MySqlCommand (that returns a resultset) against the specified MySqlConnection 
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  DataSet ds = ExecuteDataset(conn, CommandType.StoredProcedure, "GetOrders", new MySqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connection">A valid MySqlConnection</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">The stored procedure name or T-SQL command</param>
        /// <param name="commandParameters">An array of SqlParamters used to execute the command</param>
        /// <returns>A dataset containing the resultset generated by the command</returns>
        private DataSet ExecuteDatasetWithSchemaAction(MySqlConnection connection, CommandType commandType, string commandText, MissingSchemaAction missingSchemaAction, params MySqlParameter[] commandParameters)
        {
            if (connection == null) throw new ArgumentNullException("connection");

            // Create a command and prepare it for execution
            MySqlCommand cmd = new MySqlCommand();
            bool mustCloseConnection = false;
            PrepareCommand(cmd, connection, (MySqlTransaction)null, commandType, commandText, commandParameters, out mustCloseConnection);

            // Create the DataAdapter & DataSet
            using (MySqlDataAdapter da = new MySqlDataAdapter(cmd))
            {
                DataSet ds = new DataSet();

                // Fill the DataSet using default values for DataTable names, etc
                da.MissingSchemaAction = missingSchemaAction;
                da.Fill(ds);

                // Detach the SqlParameters from the command object, so they can be used again
                cmd.Parameters.Clear();

                if (mustCloseConnection)
                {
                    connection.Close();
                    //  MySqlConnection.ClearAllPools();
                }

                // Return the dataset
                return ds;
            }
        }

        /// <summary>
        /// Execute a stored procedure via a MySqlCommand (that returns a resultset) against the specified MySqlConnection 
        /// using the provided parameter values.  This method will query the database to discover the parameters for the 
        /// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
        /// </summary>
        /// <remarks>
        /// This method provides no access to output parameters or the stored procedure's return value parameter.
        /// 
        /// e.g.:  
        ///  DataSet ds = ExecuteDataset(conn, "GetOrders", 24, 36);
        /// </remarks>
        /// <param name="connection">A valid MySqlConnection</param>
        /// <param name="spName">The name of the stored procedure</param>
        /// <param name="parameterValues">An array of objects to be assigned as the input values of the stored procedure</param>
        /// <returns>A dataset containing the resultset generated by the command</returns>
        private DataSet ExecuteDatasetWithSchemaAction(MySqlConnection connection, string spName, MissingSchemaAction missingSchemaAction, params object[] parameterValues)
        {
            if (connection == null) throw new ArgumentNullException("connection");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

            // If we receive parameter values, we need to figure out where they go
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                // Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                MySqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connection, spName);

                // Assign the provided values to these parameters based on parameter order
                AssignParameterValues(commandParameters, parameterValues);

                // Call the overload that takes an array of SqlParameters
                return ExecuteDatasetWithSchemaAction(connection, CommandType.StoredProcedure, spName, missingSchemaAction, commandParameters);
            }
            else
            {
                // Otherwise we can just call the SP without params
                return ExecuteDatasetWithSchemaAction(connection, CommandType.StoredProcedure, spName, missingSchemaAction);
            }
        }






        /// <summary>
        /// Execute a MySqlCommand (that returns a resultset and takes no parameters) against the provided MySqlTransaction. 
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  DataSet ds = ExecuteDataset(trans, CommandType.StoredProcedure, "GetOrders");
        /// </remarks>
        /// <param name="transaction">A valid MySqlTransaction</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">The stored procedure name or T-SQL command</param>
        /// <returns>A dataset containing the resultset generated by the command</returns>
        public DataSet ExecuteDatasetWithSchemaAction(MySqlTransaction transaction, CommandType commandType, string commandText, MissingSchemaAction missingSchemaAction)
        {
            // Pass through the call providing null for the set of SqlParameters
            return ExecuteDatasetWithSchemaAction(transaction, commandType, commandText, missingSchemaAction, (MySqlParameter[])null);
        }

        /// <summary>
        /// Execute a MySqlCommand (that returns a resultset) against the specified MySqlTransaction
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  DataSet ds = ExecuteDataset(trans, CommandType.StoredProcedure, "GetOrders", new MySqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="transaction">A valid MySqlTransaction</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">The stored procedure name or T-SQL command</param>
        /// <param name="commandParameters">An array of SqlParamters used to execute the command</param>
        /// <returns>A dataset containing the resultset generated by the command</returns>
        public DataSet ExecuteDatasetWithSchemaAction(MySqlTransaction transaction, CommandType commandType, string commandText, MissingSchemaAction missingSchemaAction, params MySqlParameter[] commandParameters)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            if (transaction != null && transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");

            // Create a command and prepare it for execution
            MySqlCommand cmd = new MySqlCommand();
            bool mustCloseConnection = false;
            PrepareCommand(cmd, transaction.Connection, transaction, commandType, commandText, commandParameters, out mustCloseConnection);

            // Create the DataAdapter & DataSet
            using (MySqlDataAdapter da = new MySqlDataAdapter(cmd))
            {
                DataSet ds = new DataSet();

                // Fill the DataSet using default values for DataTable names, etc
                da.MissingSchemaAction = missingSchemaAction;
                da.Fill(ds);

                // Detach the SqlParameters from the command object, so they can be used again
                cmd.Parameters.Clear();

                // Return the dataset
                return ds;
            }
        }

        /// <summary>
        /// Execute a stored procedure via a MySqlCommand (that returns a resultset) against the specified 
        /// MySqlTransaction using the provided parameter values.  This method will query the database to discover the parameters for the 
        /// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
        /// </summary>
        /// <remarks>
        /// This method provides no access to output parameters or the stored procedure's return value parameter.
        /// 
        /// e.g.:  
        ///  DataSet ds = ExecuteDataset(trans, "GetOrders", 24, 36);
        /// </remarks>
        /// <param name="transaction">A valid MySqlTransaction</param>
        /// <param name="spName">The name of the stored procedure</param>
        /// <param name="parameterValues">An array of objects to be assigned as the input values of the stored procedure</param>
        /// <returns>A dataset containing the resultset generated by the command</returns>
        public DataSet ExecuteDatasetWithSchemaAction(MySqlTransaction transaction, string spName, MissingSchemaAction missingSchemaAction, params object[] parameterValues)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            if (transaction != null && transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

            // If we receive parameter values, we need to figure out where they go
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                // Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                MySqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(transaction.Connection, spName);

                // Assign the provided values to these parameters based on parameter order
                AssignParameterValues(commandParameters, parameterValues);

                // Call the overload that takes an array of SqlParameters
                return ExecuteDatasetWithSchemaAction(transaction, CommandType.StoredProcedure, spName, missingSchemaAction, commandParameters);
            }
            else
            {
                // Otherwise we can just call the SP without params
                return ExecuteDatasetWithSchemaAction(transaction, CommandType.StoredProcedure, spName, missingSchemaAction);
            }
        }

        #endregion ExecuteDatasetWithSchemaAction






        #region ExecuteReader

        /// <summary>
        /// This enum is used to indicate whether the connection was provided by the caller, or created by SqlHelper, so that
        /// we can set the appropriate CommandBehavior when calling ExecuteReader()
        /// </summary>
        private enum SqlConnectionOwnership
        {
            /// <summary>Connection is owned and managed by SqlHelper</summary>
            Internal,
            /// <summary>Connection is owned and managed by the caller</summary>
            External
        }

        /// <summary>
        /// Create and prepare a MySqlCommand, and call ExecuteReader with the appropriate CommandBehavior.
        /// </summary>
        /// <remarks>
        /// If we created and opened the connection, we want the connection to be closed when the DataReader is closed.
        /// 
        /// If the caller provided the connection, we want to leave it to them to manage.
        /// </remarks>
        /// <param name="connection">A valid MySqlConnection, on which to execute this command</param>
        /// <param name="transaction">A valid MySqlTransaction, or 'null'</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">The stored procedure name or T-SQL command</param>
        /// <param name="commandParameters">An array of SqlParameters to be associated with the command or 'null' if no parameters are required</param>
        /// <param name="connectionOwnership">Indicates whether the connection parameter was provided by the caller, or created by SqlHelper</param>
        /// <returns>MySqlDataReader containing the results of the command</returns>
        private MySqlDataReader ExecuteReader(MySqlConnection connection, MySqlTransaction transaction, CommandType commandType, string commandText, MySqlParameter[] commandParameters, SqlConnectionOwnership connectionOwnership)
        {
            if (connection == null) throw new ArgumentNullException("connection");

            bool mustCloseConnection = false;
            // Create a command and prepare it for execution
            MySqlCommand cmd = new MySqlCommand();
            try
            {
                PrepareCommand(cmd, connection, transaction, commandType, commandText, commandParameters, out mustCloseConnection);

                // Create a reader
                MySqlDataReader dataReader;

                // Call ExecuteReader with the appropriate CommandBehavior
                if (connectionOwnership == SqlConnectionOwnership.External)
                {
                    dataReader = cmd.ExecuteReader();
                }
                else
                {
                    dataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                }

                // Detach the SqlParameters from the command object, so they can be used again.
                // HACK: There is a problem here, the output parameter values are fletched 
                // when the reader is closed, so if the parameters are detached from the command
                // then the SqlReader can´t set its values. 
                // When this happen, the parameters can´t be used again in other command.
                bool canClear = true;
                foreach (MySqlParameter commandParameter in cmd.Parameters)
                {
                    if (commandParameter.Direction != ParameterDirection.Input)
                        canClear = false;
                }

                if (canClear)
                {
                    cmd.Parameters.Clear();
                }

                return dataReader;
            }
            catch
            {
                if (mustCloseConnection)
                {
                    connection.Close();
                    // MySqlConnection.ClearAllPools();
                }
                throw;
            }
        }

        /// <summary>
        /// Execute a MySqlCommand (that returns a resultset and takes no parameters) against the database specified in 
        /// the connection string. 
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  MySqlDataReader dr = ExecuteReader(connString, CommandType.StoredProcedure, "GetOrders");
        /// </remarks>
        /// <param name="connectionString">A valid connection string for a MySqlConnection</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">The stored procedure name or T-SQL command</param>
        /// <returns>A MySqlDataReader containing the resultset generated by the command</returns>
        public MySqlDataReader ExecuteReader(string connectionString, CommandType commandType, string commandText)
        {
            // Pass through the call providing null for the set of SqlParameters
            return ExecuteReader(connectionString, commandType, commandText, (MySqlParameter[])null);
        }

        /// <summary>
        /// Execute a MySqlCommand (that returns a resultset) against the database specified in the connection string 
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  MySqlDataReader dr = ExecuteReader(connString, CommandType.StoredProcedure, "GetOrders", new MySqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connectionString">A valid connection string for a MySqlConnection</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">The stored procedure name or T-SQL command</param>
        /// <param name="commandParameters">An array of SqlParamters used to execute the command</param>
        /// <returns>A MySqlDataReader containing the resultset generated by the command</returns>
        public MySqlDataReader ExecuteReader(string connectionString, CommandType commandType, string commandText, params MySqlParameter[] commandParameters)
        {
            if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");
            MySqlConnection connection = null;
            try
            {
                connection = new MySqlConnection(connectionString);
                connection.Open();

                // Call the private overload that takes an internally owned connection in place of the connection string
                return ExecuteReader(connection, null, commandType, commandText, commandParameters, SqlConnectionOwnership.Internal);
            }
            catch
            {
                // If we fail to return the SqlDatReader, we need to close the connection ourselves
                if (connection != null)
                {
                    connection.Close();
                    // MySqlConnection.ClearAllPools();
                }
                throw;
            }

        }

        /// <summary>
        /// Execute a stored procedure via a MySqlCommand (that returns a resultset) against the database specified in 
        /// the connection string using the provided parameter values.  This method will query the database to discover the parameters for the 
        /// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
        /// </summary>
        /// <remarks>
        /// This method provides no access to output parameters or the stored procedure's return value parameter.
        /// 
        /// e.g.:  
        ///  MySqlDataReader dr = ExecuteReader(connString, "GetOrders", 24, 36);
        /// </remarks>
        /// <param name="connectionString">A valid connection string for a MySqlConnection</param>
        /// <param name="spName">The name of the stored procedure</param>
        /// <param name="parameterValues">An array of objects to be assigned as the input values of the stored procedure</param>
        /// <returns>A MySqlDataReader containing the resultset generated by the command</returns>
        public MySqlDataReader ExecuteReader(string connectionString, string spName, params object[] parameterValues)
        {
            if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

            // If we receive parameter values, we need to figure out where they go
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                MySqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connectionString, spName);

                AssignParameterValues(commandParameters, parameterValues);

                return ExecuteReader(connectionString, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                // Otherwise we can just call the SP without params
                return ExecuteReader(connectionString, CommandType.StoredProcedure, spName);
            }
        }




        /// <summary>
        /// Execute a MySqlCommand (that returns a resultset and takes no parameters) against the provided MySqlConnection. 
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  MySqlDataReader dr = ExecuteReader(conn, CommandType.StoredProcedure, "GetOrders");
        /// </remarks>
        /// <param name="connection">A valid MySqlConnection</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">The stored procedure name or T-SQL command</param>
        /// <returns>A MySqlDataReader containing the resultset generated by the command</returns>
        private MySqlDataReader ExecuteReader(MySqlConnection connection, CommandType commandType, string commandText)
        {
            // Pass through the call providing null for the set of SqlParameters
            return ExecuteReader(connection, commandType, commandText, (MySqlParameter[])null);
        }

        /// <summary>
        /// Execute a MySqlCommand (that returns a resultset) against the specified MySqlConnection 
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  MySqlDataReader dr = ExecuteReader(conn, CommandType.StoredProcedure, "GetOrders", new MySqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connection">A valid MySqlConnection</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">The stored procedure name or T-SQL command</param>
        /// <param name="commandParameters">An array of SqlParamters used to execute the command</param>
        /// <returns>A MySqlDataReader containing the resultset generated by the command</returns>
        private MySqlDataReader ExecuteReader(MySqlConnection connection, CommandType commandType, string commandText, params MySqlParameter[] commandParameters)
        {
            // Pass through the call to the private overload using a null transaction value and an externally owned connection
            return ExecuteReader(connection, (MySqlTransaction)null, commandType, commandText, commandParameters, SqlConnectionOwnership.External);
        }

        /// <summary>
        /// Execute a stored procedure via a MySqlCommand (that returns a resultset) against the specified MySqlConnection 
        /// using the provided parameter values.  This method will query the database to discover the parameters for the 
        /// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
        /// </summary>
        /// <remarks>
        /// This method provides no access to output parameters or the stored procedure's return value parameter.
        /// 
        /// e.g.:  
        ///  MySqlDataReader dr = ExecuteReader(conn, "GetOrders", 24, 36);
        /// </remarks>
        /// <param name="connection">A valid MySqlConnection</param>
        /// <param name="spName">The name of the stored procedure</param>
        /// <param name="parameterValues">An array of objects to be assigned as the input values of the stored procedure</param>
        /// <returns>A MySqlDataReader containing the resultset generated by the command</returns>
        private MySqlDataReader ExecuteReader(MySqlConnection connection, string spName, params object[] parameterValues)
        {
            if (connection == null) throw new ArgumentNullException("connection");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

            // If we receive parameter values, we need to figure out where they go
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                MySqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connection, spName);

                AssignParameterValues(commandParameters, parameterValues);

                return ExecuteReader(connection, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                // Otherwise we can just call the SP without params
                return ExecuteReader(connection, CommandType.StoredProcedure, spName);
            }
        }




        /// <summary>
        /// Execute a MySqlCommand (that returns a resultset and takes no parameters) against the provided MySqlTransaction. 
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  MySqlDataReader dr = ExecuteReader(trans, CommandType.StoredProcedure, "GetOrders");
        /// </remarks>
        /// <param name="transaction">A valid MySqlTransaction</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">The stored procedure name or T-SQL command</param>
        /// <returns>A MySqlDataReader containing the resultset generated by the command</returns>
        public MySqlDataReader ExecuteReader(MySqlTransaction transaction, CommandType commandType, string commandText)
        {
            // Pass through the call providing null for the set of SqlParameters
            return ExecuteReader(transaction, commandType, commandText, (MySqlParameter[])null);
        }

        /// <summary>
        /// Execute a MySqlCommand (that returns a resultset) against the specified MySqlTransaction
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///   MySqlDataReader dr = ExecuteReader(trans, CommandType.StoredProcedure, "GetOrders", new MySqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="transaction">A valid MySqlTransaction</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">The stored procedure name or T-SQL command</param>
        /// <param name="commandParameters">An array of SqlParamters used to execute the command</param>
        /// <returns>A MySqlDataReader containing the resultset generated by the command</returns>
        public MySqlDataReader ExecuteReader(MySqlTransaction transaction, CommandType commandType, string commandText, params MySqlParameter[] commandParameters)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            if (transaction != null && transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");

            // Pass through to private overload, indicating that the connection is owned by the caller
            return ExecuteReader(transaction.Connection, transaction, commandType, commandText, commandParameters, SqlConnectionOwnership.External);
        }

        /// <summary>
        /// Execute a stored procedure via a MySqlCommand (that returns a resultset) against the specified
        /// MySqlTransaction using the provided parameter values.  This method will query the database to discover the parameters for the 
        /// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
        /// </summary>
        /// <remarks>
        /// This method provides no access to output parameters or the stored procedure's return value parameter.
        /// 
        /// e.g.:  
        ///  MySqlDataReader dr = ExecuteReader(trans, "GetOrders", 24, 36);
        /// </remarks>
        /// <param name="transaction">A valid MySqlTransaction</param>
        /// <param name="spName">The name of the stored procedure</param>
        /// <param name="parameterValues">An array of objects to be assigned as the input values of the stored procedure</param>
        /// <returns>A MySqlDataReader containing the resultset generated by the command</returns>
        public MySqlDataReader ExecuteReader(MySqlTransaction transaction, string spName, params object[] parameterValues)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            if (transaction != null && transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

            // If we receive parameter values, we need to figure out where they go
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                MySqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(transaction.Connection, spName);

                AssignParameterValues(commandParameters, parameterValues);

                return ExecuteReader(transaction, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                // Otherwise we can just call the SP without params
                return ExecuteReader(transaction, CommandType.StoredProcedure, spName);
            }
        }

        #endregion ExecuteReader









        #region ExecuteScalar

        /// <summary>
        /// Execute a MySqlCommand (that returns a 1x1 resultset and takes no parameters) against the database specified in 
        /// the connection string. 
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  int orderCount = (int)ExecuteScalar(connString, CommandType.StoredProcedure, "GetOrderCount");
        /// </remarks>
        /// <param name="connectionString">A valid connection string for a MySqlConnection</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">The stored procedure name or T-SQL command</param>
        /// <returns>An object containing the value in the 1x1 resultset generated by the command</returns>
        public object ExecuteScalar(string connectionString, CommandType commandType, string commandText)
        {
            // Pass through the call providing null for the set of SqlParameters
            return ExecuteScalar(connectionString, commandType, commandText, (MySqlParameter[])null);
        }

        /// <summary>
        /// Execute a MySqlCommand (that returns a 1x1 resultset) against the database specified in the connection string 
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  int orderCount = (int)ExecuteScalar(connString, CommandType.StoredProcedure, "GetOrderCount", new MySqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connectionString">A valid connection string for a MySqlConnection</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">The stored procedure name or T-SQL command</param>
        /// <param name="commandParameters">An array of SqlParamters used to execute the command</param>
        /// <returns>An object containing the value in the 1x1 resultset generated by the command</returns>
        public object ExecuteScalar(string connectionString, CommandType commandType, string commandText, params MySqlParameter[] commandParameters)
        {
            if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");
            // Create & open a MySqlConnection, and dispose of it after we are done
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                // Call the overload that takes a connection in place of the connection string
                return ExecuteScalar(connection, commandType, commandText, commandParameters);
            }
        }

        /// <summary>
        /// Execute a stored procedure via a MySqlCommand (that returns a 1x1 resultset) against the database specified in 
        /// the connection string using the provided parameter values.  This method will query the database to discover the parameters for the 
        /// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
        /// </summary>
        /// <remarks>
        /// This method provides no access to output parameters or the stored procedure's return value parameter.
        /// 
        /// e.g.:  
        ///  int orderCount = (int)ExecuteScalar(connString, "GetOrderCount", 24, 36);
        /// </remarks>
        /// <param name="connectionString">A valid connection string for a MySqlConnection</param>
        /// <param name="spName">The name of the stored procedure</param>
        /// <param name="parameterValues">An array of objects to be assigned as the input values of the stored procedure</param>
        /// <returns>An object containing the value in the 1x1 resultset generated by the command</returns>
        public object ExecuteScalar(string connectionString, string spName, params object[] parameterValues)
        {
            if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

            // If we receive parameter values, we need to figure out where they go
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                // Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                MySqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connectionString, spName);

                // Assign the provided values to these parameters based on parameter order
                AssignParameterValues(commandParameters, parameterValues);

                // Call the overload that takes an array of SqlParameters
                return ExecuteScalar(connectionString, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                // Otherwise we can just call the SP without params
                return ExecuteScalar(connectionString, CommandType.StoredProcedure, spName);
            }
        }





        /// <summary>
        /// Execute a MySqlCommand (that returns a 1x1 resultset and takes no parameters) against the provided MySqlConnection. 
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  int orderCount = (int)ExecuteScalar(conn, CommandType.StoredProcedure, "GetOrderCount");
        /// </remarks>
        /// <param name="connection">A valid MySqlConnection</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">The stored procedure name or T-SQL command</param>
        /// <returns>An object containing the value in the 1x1 resultset generated by the command</returns>
        private object ExecuteScalar(MySqlConnection connection, CommandType commandType, string commandText)
        {
            // Pass through the call providing null for the set of SqlParameters
            return ExecuteScalar(connection, commandType, commandText, (MySqlParameter[])null);
        }

        /// <summary>
        /// Execute a MySqlCommand (that returns a 1x1 resultset) against the specified MySqlConnection 
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  int orderCount = (int)ExecuteScalar(conn, CommandType.StoredProcedure, "GetOrderCount", new MySqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connection">A valid MySqlConnection</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">The stored procedure name or T-SQL command</param>
        /// <param name="commandParameters">An array of SqlParamters used to execute the command</param>
        /// <returns>An object containing the value in the 1x1 resultset generated by the command</returns>
        private object ExecuteScalar(MySqlConnection connection, CommandType commandType, string commandText, params MySqlParameter[] commandParameters)
        {
            if (connection == null) throw new ArgumentNullException("connection");

            // Create a command and prepare it for execution
            MySqlCommand cmd = new MySqlCommand();

            bool mustCloseConnection = false;
            PrepareCommand(cmd, connection, (MySqlTransaction)null, commandType, commandText, commandParameters, out mustCloseConnection);

            // Execute the command & return the results
            object retval = cmd.ExecuteScalar();

            // Detach the SqlParameters from the command object, so they can be used again
            cmd.Parameters.Clear();

            if (mustCloseConnection)
            {
                connection.Close();
                // MySqlConnection.ClearAllPools();
            }

            return retval;
        }

        /// <summary>
        /// Execute a stored procedure via a MySqlCommand (that returns a 1x1 resultset) against the specified MySqlConnection 
        /// using the provided parameter values.  This method will query the database to discover the parameters for the 
        /// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
        /// </summary>
        /// <remarks>
        /// This method provides no access to output parameters or the stored procedure's return value parameter.
        /// 
        /// e.g.:  
        ///  int orderCount = (int)ExecuteScalar(conn, "GetOrderCount", 24, 36);
        /// </remarks>
        /// <param name="connection">A valid MySqlConnection</param>
        /// <param name="spName">The name of the stored procedure</param>
        /// <param name="parameterValues">An array of objects to be assigned as the input values of the stored procedure</param>
        /// <returns>An object containing the value in the 1x1 resultset generated by the command</returns>
        private object ExecuteScalar(MySqlConnection connection, string spName, params object[] parameterValues)
        {
            if (connection == null) throw new ArgumentNullException("connection");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

            // If we receive parameter values, we need to figure out where they go
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                // Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                MySqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connection, spName);

                // Assign the provided values to these parameters based on parameter order
                AssignParameterValues(commandParameters, parameterValues);

                // Call the overload that takes an array of SqlParameters
                return ExecuteScalar(connection, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                // Otherwise we can just call the SP without params
                return ExecuteScalar(connection, CommandType.StoredProcedure, spName);
            }
        }






        /// <summary>
        /// Execute a MySqlCommand (that returns a 1x1 resultset and takes no parameters) against the provided MySqlTransaction. 
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  int orderCount = (int)ExecuteScalar(trans, CommandType.StoredProcedure, "GetOrderCount");
        /// </remarks>
        /// <param name="transaction">A valid MySqlTransaction</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">The stored procedure name or T-SQL command</param>
        /// <returns>An object containing the value in the 1x1 resultset generated by the command</returns>
        public object ExecuteScalar(MySqlTransaction transaction, CommandType commandType, string commandText)
        {
            // Pass through the call providing null for the set of SqlParameters
            return ExecuteScalar(transaction, commandType, commandText, (MySqlParameter[])null);
        }

        /// <summary>
        /// Execute a MySqlCommand (that returns a 1x1 resultset) against the specified MySqlTransaction
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  int orderCount = (int)ExecuteScalar(trans, CommandType.StoredProcedure, "GetOrderCount", new MySqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="transaction">A valid MySqlTransaction</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">The stored procedure name or T-SQL command</param>
        /// <param name="commandParameters">An array of SqlParamters used to execute the command</param>
        /// <returns>An object containing the value in the 1x1 resultset generated by the command</returns>
        public object ExecuteScalar(MySqlTransaction transaction, CommandType commandType, string commandText, params MySqlParameter[] commandParameters)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            if (transaction != null && transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");

            // Create a command and prepare it for execution
            MySqlCommand cmd = new MySqlCommand();
            bool mustCloseConnection = false;
            PrepareCommand(cmd, transaction.Connection, transaction, commandType, commandText, commandParameters, out mustCloseConnection);

            // Execute the command & return the results
            object retval = cmd.ExecuteScalar();

            // Detach the SqlParameters from the command object, so they can be used again
            cmd.Parameters.Clear();
            return retval;
        }

        /// <summary>
        /// Execute a stored procedure via a MySqlCommand (that returns a 1x1 resultset) against the specified
        /// MySqlTransaction using the provided parameter values.  This method will query the database to discover the parameters for the 
        /// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
        /// </summary>
        /// <remarks>
        /// This method provides no access to output parameters or the stored procedure's return value parameter.
        /// 
        /// e.g.:  
        ///  int orderCount = (int)ExecuteScalar(trans, "GetOrderCount", 24, 36);
        /// </remarks>
        /// <param name="transaction">A valid MySqlTransaction</param>
        /// <param name="spName">The name of the stored procedure</param>
        /// <param name="parameterValues">An array of objects to be assigned as the input values of the stored procedure</param>
        /// <returns>An object containing the value in the 1x1 resultset generated by the command</returns>
        public object ExecuteScalar(MySqlTransaction transaction, string spName, params object[] parameterValues)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            if (transaction != null && transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

            // If we receive parameter values, we need to figure out where they go
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                // PPull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                MySqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(transaction.Connection, spName);

                // Assign the provided values to these parameters based on parameter order
                AssignParameterValues(commandParameters, parameterValues);

                // Call the overload that takes an array of SqlParameters
                return ExecuteScalar(transaction, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                // Otherwise we can just call the SP without params
                return ExecuteScalar(transaction, CommandType.StoredProcedure, spName);
            }
        }

        #endregion ExecuteScalar







        #region ExecuteXmlReader





        /// <summary>
        /// Execute a MySqlCommand (that returns a resultset and takes no parameters) against the provided MySqlConnection. 
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  XmlReader r = ExecuteXmlReader(conn, CommandType.StoredProcedure, "GetOrders");
        /// </remarks>
        /// <param name="connection">A valid MySqlConnection</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">The stored procedure name or T-SQL command using "FOR XML AUTO"</param>
        /// <returns>An XmlReader containing the resultset generated by the command</returns>
        public XmlReader ExecuteXmlReader(string connectionString, CommandType commandType, string commandText)
        {
            // Pass through the call providing null for the set of SqlParameters
            return ExecuteXmlReader(connectionString, commandType, commandText, (MySqlParameter[])null);
        }

        /// <summary>
        /// Execute a MySqlCommand (that returns a resultset) against the specified MySqlConnection 
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  XmlReader r = ExecuteXmlReader(conn, CommandType.StoredProcedure, "GetOrders", new MySqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connection">A valid MySqlConnection</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">The stored procedure name or T-SQL command using "FOR XML AUTO"</param>
        /// <param name="commandParameters">An array of SqlParamters used to execute the command</param>
        /// <returns>An XmlReader containing the resultset generated by the command</returns>
        public XmlReader ExecuteXmlReader(string connectionString, CommandType commandType, string commandText, params MySqlParameter[] commandParameters)
        {
            if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");


            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                // Call the overload that takes a connection in place of the connection string
                return ExecuteXmlReader(connection, commandType, commandText, commandParameters);
            }



        }

        /// <summary>
        /// Execute a stored procedure via a MySqlCommand (that returns a resultset) against the specified MySqlConnection 
        /// using the provided parameter values.  This method will query the database to discover the parameters for the 
        /// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
        /// </summary>
        /// <remarks>
        /// This method provides no access to output parameters or the stored procedure's return value parameter.
        /// 
        /// e.g.:  
        ///  XmlReader r = ExecuteXmlReader(conn, "GetOrders", 24, 36);
        /// </remarks>
        /// <param name="connection">A valid MySqlConnection</param>
        /// <param name="spName">The name of the stored procedure using "FOR XML AUTO"</param>
        /// <param name="parameterValues">An array of objects to be assigned as the input values of the stored procedure</param>
        /// <returns>An XmlReader containing the resultset generated by the command</returns>
        public XmlReader ExecuteXmlReader(string connectionString, string spName, params object[] parameterValues)
        {
            if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

            // If we receive parameter values, we need to figure out where they go
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                // Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                MySqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connectionString, spName);

                // Assign the provided values to these parameters based on parameter order
                AssignParameterValues(commandParameters, parameterValues);

                // Call the overload that takes an array of SqlParameters
                return ExecuteXmlReader(connectionString, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                // Otherwise we can just call the SP without params
                return ExecuteXmlReader(connectionString, CommandType.StoredProcedure, spName);
            }
        }










        /// <summary>
        /// Execute a MySqlCommand (that returns a resultset and takes no parameters) against the provided MySqlConnection. 
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  XmlReader r = ExecuteXmlReader(conn, CommandType.StoredProcedure, "GetOrders");
        /// </remarks>
        /// <param name="connection">A valid MySqlConnection</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">The stored procedure name or T-SQL command using "FOR XML AUTO"</param>
        /// <returns>An XmlReader containing the resultset generated by the command</returns>
        private XmlReader ExecuteXmlReader(MySqlConnection connection, CommandType commandType, string commandText)
        {
            // Pass through the call providing null for the set of SqlParameters
            return ExecuteXmlReader(connection, commandType, commandText, (MySqlParameter[])null);
        }

        /// <summary>
        /// Execute a MySqlCommand (that returns a resultset) against the specified MySqlConnection 
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  XmlReader r = ExecuteXmlReader(conn, CommandType.StoredProcedure, "GetOrders", new MySqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connection">A valid MySqlConnection</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">The stored procedure name or T-SQL command using "FOR XML AUTO"</param>
        /// <param name="commandParameters">An array of SqlParamters used to execute the command</param>
        /// <returns>An XmlReader containing the resultset generated by the command</returns>
        private XmlReader ExecuteXmlReader(MySqlConnection connection, CommandType commandType, string commandText, params MySqlParameter[] commandParameters)
        {
            //if (connection == null) throw new ArgumentNullException("connection");

            //bool mustCloseConnection = false;
            //// Create a command and prepare it for execution
            //MySqlCommand cmd = new MySqlCommand();
            //try
            //{
            //    PrepareCommand(cmd, connection, (MySqlTransaction)null, commandType, commandText, commandParameters, out mustCloseConnection);

            //    // Create the DataAdapter & DataSet
            //    XmlReader retval = cmd.ExecuteXmlReader();

            //    // Detach the SqlParameters from the command object, so they can be used again
            //    cmd.Parameters.Clear();

            //    return retval;
            //}
            //catch
            //{
            //    if (mustCloseConnection)
            //    {
            //        connection.Close();
            //     //   MySqlConnection.ClearAllPools();
            //    }
            //    throw;
            //}
            return null;
        }

        /// <summary>
        /// Execute a stored procedure via a MySqlCommand (that returns a resultset) against the specified MySqlConnection 
        /// using the provided parameter values.  This method will query the database to discover the parameters for the 
        /// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
        /// </summary>
        /// <remarks>
        /// This method provides no access to output parameters or the stored procedure's return value parameter.
        /// 
        /// e.g.:  
        ///  XmlReader r = ExecuteXmlReader(conn, "GetOrders", 24, 36);
        /// </remarks>
        /// <param name="connection">A valid MySqlConnection</param>
        /// <param name="spName">The name of the stored procedure using "FOR XML AUTO"</param>
        /// <param name="parameterValues">An array of objects to be assigned as the input values of the stored procedure</param>
        /// <returns>An XmlReader containing the resultset generated by the command</returns>
        private XmlReader ExecuteXmlReader(MySqlConnection connection, string spName, params object[] parameterValues)
        {
            if (connection == null) throw new ArgumentNullException("connection");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

            // If we receive parameter values, we need to figure out where they go
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                // Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                MySqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connection, spName);

                // Assign the provided values to these parameters based on parameter order
                AssignParameterValues(commandParameters, parameterValues);

                // Call the overload that takes an array of SqlParameters
                return ExecuteXmlReader(connection, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                // Otherwise we can just call the SP without params
                return ExecuteXmlReader(connection, CommandType.StoredProcedure, spName);
            }
        }





        /// <summary>
        /// Execute a MySqlCommand (that returns a resultset and takes no parameters) against the provided MySqlTransaction. 
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  XmlReader r = ExecuteXmlReader(trans, CommandType.StoredProcedure, "GetOrders");
        /// </remarks>
        /// <param name="transaction">A valid MySqlTransaction</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">The stored procedure name or T-SQL command using "FOR XML AUTO"</param>
        /// <returns>An XmlReader containing the resultset generated by the command</returns>
        public XmlReader ExecuteXmlReader(MySqlTransaction transaction, CommandType commandType, string commandText)
        {
            // Pass through the call providing null for the set of SqlParameters
            return ExecuteXmlReader(transaction, commandType, commandText, (MySqlParameter[])null);
        }

        /// <summary>
        /// Execute a MySqlCommand (that returns a resultset) against the specified MySqlTransaction
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  XmlReader r = ExecuteXmlReader(trans, CommandType.StoredProcedure, "GetOrders", new MySqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="transaction">A valid MySqlTransaction</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">The stored procedure name or T-SQL command using "FOR XML AUTO"</param>
        /// <param name="commandParameters">An array of SqlParamters used to execute the command</param>
        /// <returns>An XmlReader containing the resultset generated by the command</returns>
        public XmlReader ExecuteXmlReader(MySqlTransaction transaction, CommandType commandType, string commandText, params MySqlParameter[] commandParameters)
        {
            //if (transaction == null) throw new ArgumentNullException("transaction");
            //if (transaction != null && transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");

            //// Create a command and prepare it for execution
            //MySqlCommand cmd = new MySqlCommand();
            //bool mustCloseConnection = false;
            //PrepareCommand(cmd, transaction.Connection, transaction, commandType, commandText, commandParameters, out mustCloseConnection);

            //// Create the DataAdapter & DataSet
            //XmlReader retval = cmd.ExecuteXmlReader();

            //// Detach the SqlParameters from the command object, so they can be used again
            //cmd.Parameters.Clear();
            //return retval;
            return null;
        }

        /// <summary>
        /// Execute a stored procedure via a MySqlCommand (that returns a resultset) against the specified 
        /// MySqlTransaction using the provided parameter values.  This method will query the database to discover the parameters for the 
        /// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
        /// </summary>
        /// <remarks>
        /// This method provides no access to output parameters or the stored procedure's return value parameter.
        /// 
        /// e.g.:  
        ///  XmlReader r = ExecuteXmlReader(trans, "GetOrders", 24, 36);
        /// </remarks>
        /// <param name="transaction">A valid MySqlTransaction</param>
        /// <param name="spName">The name of the stored procedure</param>
        /// <param name="parameterValues">An array of objects to be assigned as the input values of the stored procedure</param>
        /// <returns>A dataset containing the resultset generated by the command</returns>
        public XmlReader ExecuteXmlReader(MySqlTransaction transaction, string spName, params object[] parameterValues)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            if (transaction != null && transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

            // If we receive parameter values, we need to figure out where they go
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                // Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                MySqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(transaction.Connection, spName);

                // Assign the provided values to these parameters based on parameter order
                AssignParameterValues(commandParameters, parameterValues);

                // Call the overload that takes an array of SqlParameters
                return ExecuteXmlReader(transaction, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                // Otherwise we can just call the SP without params
                return ExecuteXmlReader(transaction, CommandType.StoredProcedure, spName);
            }
        }




        #endregion ExecuteXmlReader








        #region FillDataset
        /// <summary>
        /// Execute a MySqlCommand (that returns a resultset and takes no parameters) against the database specified in 
        /// the connection string. 
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  FillDataset(connString, CommandType.StoredProcedure, "GetOrders", ds, new string[] {"orders"});
        /// </remarks>
        /// <param name="connectionString">A valid connection string for a MySqlConnection</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">The stored procedure name or T-SQL command</param>
        /// <param name="dataSet">A dataset wich will contain the resultset generated by the command</param>
        /// <param name="tableNames">This array will be used to create table mappings allowing the DataTables to be referenced
        /// by a user defined name (probably the actual table name)</param>
        public void FillDataset(string connectionString, CommandType commandType, string commandText, DataSet dataSet, string[] tableNames)
        {
            if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");
            if (dataSet == null) throw new ArgumentNullException("dataSet");

            // Create & open a MySqlConnection, and dispose of it after we are done
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                // Call the overload that takes a connection in place of the connection string
                FillDataset(connection, commandType, commandText, dataSet, tableNames);
            }
        }

        /// <summary>
        /// Execute a MySqlCommand (that returns a resultset) against the database specified in the connection string 
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  FillDataset(connString, CommandType.StoredProcedure, "GetOrders", ds, new string[] {"orders"}, new MySqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connectionString">A valid connection string for a MySqlConnection</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">The stored procedure name or T-SQL command</param>
        /// <param name="commandParameters">An array of SqlParamters used to execute the command</param>
        /// <param name="dataSet">A dataset wich will contain the resultset generated by the command</param>
        /// <param name="tableNames">This array will be used to create table mappings allowing the DataTables to be referenced
        /// by a user defined name (probably the actual table name)
        /// </param>
        public void FillDataset(string connectionString, CommandType commandType,
            string commandText, DataSet dataSet, string[] tableNames,
            params MySqlParameter[] commandParameters)
        {
            if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");
            if (dataSet == null) throw new ArgumentNullException("dataSet");
            // Create & open a MySqlConnection, and dispose of it after we are done
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                // Call the overload that takes a connection in place of the connection string
                FillDataset(connection, commandType, commandText, dataSet, tableNames, commandParameters);
            }
        }

        /// <summary>
        /// Execute a stored procedure via a MySqlCommand (that returns a resultset) against the database specified in 
        /// the connection string using the provided parameter values.  This method will query the database to discover the parameters for the 
        /// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
        /// </summary>
        /// <remarks>
        /// This method provides no access to output parameters or the stored procedure's return value parameter.
        /// 
        /// e.g.:  
        ///  FillDataset(connString, CommandType.StoredProcedure, "GetOrders", ds, new string[] {"orders"}, 24);
        /// </remarks>
        /// <param name="connectionString">A valid connection string for a MySqlConnection</param>
        /// <param name="spName">The name of the stored procedure</param>
        /// <param name="dataSet">A dataset wich will contain the resultset generated by the command</param>
        /// <param name="tableNames">This array will be used to create table mappings allowing the DataTables to be referenced
        /// by a user defined name (probably the actual table name)
        /// </param>    
        /// <param name="parameterValues">An array of objects to be assigned as the input values of the stored procedure</param>
        public void FillDataset(string connectionString, string spName,
            DataSet dataSet, string[] tableNames,
            params object[] parameterValues)
        {
            if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");
            if (dataSet == null) throw new ArgumentNullException("dataSet");
            // Create & open a MySqlConnection, and dispose of it after we are done
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                // Call the overload that takes a connection in place of the connection string
                FillDataset(connection, spName, dataSet, tableNames, parameterValues);
            }
        }





        /// <summary>
        /// Execute a MySqlCommand (that returns a resultset and takes no parameters) against the provided MySqlConnection. 
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  FillDataset(conn, CommandType.StoredProcedure, "GetOrders", ds, new string[] {"orders"});
        /// </remarks>
        /// <param name="connection">A valid MySqlConnection</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">The stored procedure name or T-SQL command</param>
        /// <param name="dataSet">A dataset wich will contain the resultset generated by the command</param>
        /// <param name="tableNames">This array will be used to create table mappings allowing the DataTables to be referenced
        /// by a user defined name (probably the actual table name)
        /// </param>    
        private void FillDataset(MySqlConnection connection, CommandType commandType,
            string commandText, DataSet dataSet, string[] tableNames)
        {
            FillDataset(connection, commandType, commandText, dataSet, tableNames, null);
        }

        /// <summary>
        /// Execute a MySqlCommand (that returns a resultset) against the specified MySqlConnection 
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  FillDataset(conn, CommandType.StoredProcedure, "GetOrders", ds, new string[] {"orders"}, new MySqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connection">A valid MySqlConnection</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">The stored procedure name or T-SQL command</param>
        /// <param name="dataSet">A dataset wich will contain the resultset generated by the command</param>
        /// <param name="tableNames">This array will be used to create table mappings allowing the DataTables to be referenced
        /// by a user defined name (probably the actual table name)
        /// </param>
        /// <param name="commandParameters">An array of SqlParamters used to execute the command</param>
        private void FillDataset(MySqlConnection connection, CommandType commandType,
            string commandText, DataSet dataSet, string[] tableNames,
            params MySqlParameter[] commandParameters)
        {
            FillDataset(connection, null, commandType, commandText, dataSet, tableNames, commandParameters);
        }

        /// <summary>
        /// Execute a stored procedure via a MySqlCommand (that returns a resultset) against the specified MySqlConnection 
        /// using the provided parameter values.  This method will query the database to discover the parameters for the 
        /// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
        /// </summary>
        /// <remarks>
        /// This method provides no access to output parameters or the stored procedure's return value parameter.
        /// 
        /// e.g.:  
        ///  FillDataset(conn, "GetOrders", ds, new string[] {"orders"}, 24, 36);
        /// </remarks>
        /// <param name="connection">A valid MySqlConnection</param>
        /// <param name="spName">The name of the stored procedure</param>
        /// <param name="dataSet">A dataset wich will contain the resultset generated by the command</param>
        /// <param name="tableNames">This array will be used to create table mappings allowing the DataTables to be referenced
        /// by a user defined name (probably the actual table name)
        /// </param>
        /// <param name="parameterValues">An array of objects to be assigned as the input values of the stored procedure</param>
        private void FillDataset(MySqlConnection connection, string spName,
            DataSet dataSet, string[] tableNames,
            params object[] parameterValues)
        {
            if (connection == null) throw new ArgumentNullException("connection");
            if (dataSet == null) throw new ArgumentNullException("dataSet");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

            // If we receive parameter values, we need to figure out where they go
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                // Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                MySqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connection, spName);

                // Assign the provided values to these parameters based on parameter order
                AssignParameterValues(commandParameters, parameterValues);

                // Call the overload that takes an array of SqlParameters
                FillDataset(connection, CommandType.StoredProcedure, spName, dataSet, tableNames, commandParameters);
            }
            else
            {
                // Otherwise we can just call the SP without params
                FillDataset(connection, CommandType.StoredProcedure, spName, dataSet, tableNames);
            }
        }





        /// <summary>
        /// Execute a MySqlCommand (that returns a resultset and takes no parameters) against the provided MySqlTransaction. 
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  FillDataset(trans, CommandType.StoredProcedure, "GetOrders", ds, new string[] {"orders"});
        /// </remarks>
        /// <param name="transaction">A valid MySqlTransaction</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">The stored procedure name or T-SQL command</param>
        /// <param name="dataSet">A dataset wich will contain the resultset generated by the command</param>
        /// <param name="tableNames">This array will be used to create table mappings allowing the DataTables to be referenced
        /// by a user defined name (probably the actual table name)
        /// </param>
        public void FillDataset(MySqlTransaction transaction, CommandType commandType,
            string commandText,
            DataSet dataSet, string[] tableNames)
        {
            FillDataset(transaction, commandType, commandText, dataSet, tableNames, null);
        }

        /// <summary>
        /// Execute a MySqlCommand (that returns a resultset) against the specified MySqlTransaction
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  FillDataset(trans, CommandType.StoredProcedure, "GetOrders", ds, new string[] {"orders"}, new MySqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="transaction">A valid MySqlTransaction</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">The stored procedure name or T-SQL command</param>
        /// <param name="dataSet">A dataset wich will contain the resultset generated by the command</param>
        /// <param name="tableNames">This array will be used to create table mappings allowing the DataTables to be referenced
        /// by a user defined name (probably the actual table name)
        /// </param>
        /// <param name="commandParameters">An array of SqlParamters used to execute the command</param>
        public void FillDataset(MySqlTransaction transaction, CommandType commandType,
            string commandText, DataSet dataSet, string[] tableNames,
            params MySqlParameter[] commandParameters)
        {
            FillDataset(transaction.Connection, transaction, commandType, commandText, dataSet, tableNames, commandParameters);
        }

        /// <summary>
        /// Execute a stored procedure via a MySqlCommand (that returns a resultset) against the specified 
        /// MySqlTransaction using the provided parameter values.  This method will query the database to discover the parameters for the 
        /// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
        /// </summary>
        /// <remarks>
        /// This method provides no access to output parameters or the stored procedure's return value parameter.
        /// 
        /// e.g.:  
        ///  FillDataset(trans, "GetOrders", ds, new string[]{"orders"}, 24, 36);
        /// </remarks>
        /// <param name="transaction">A valid MySqlTransaction</param>
        /// <param name="spName">The name of the stored procedure</param>
        /// <param name="dataSet">A dataset wich will contain the resultset generated by the command</param>
        /// <param name="tableNames">This array will be used to create table mappings allowing the DataTables to be referenced
        /// by a user defined name (probably the actual table name)
        /// </param>
        /// <param name="parameterValues">An array of objects to be assigned as the input values of the stored procedure</param>
        public void FillDataset(MySqlTransaction transaction, string spName,
            DataSet dataSet, string[] tableNames,
            params object[] parameterValues)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            if (transaction != null && transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
            if (dataSet == null) throw new ArgumentNullException("dataSet");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

            // If we receive parameter values, we need to figure out where they go
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                // Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                MySqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(transaction.Connection, spName);

                // Assign the provided values to these parameters based on parameter order
                AssignParameterValues(commandParameters, parameterValues);

                // Call the overload that takes an array of SqlParameters
                FillDataset(transaction, CommandType.StoredProcedure, spName, dataSet, tableNames, commandParameters);
            }
            else
            {
                // Otherwise we can just call the SP without params
                FillDataset(transaction, CommandType.StoredProcedure, spName, dataSet, tableNames);
            }
        }

        /// <summary>
        /// Private helper method that execute a MySqlCommand (that returns a resultset) against the specified MySqlTransaction and MySqlConnection
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  FillDataset(conn, trans, CommandType.StoredProcedure, "GetOrders", ds, new string[] {"orders"}, new MySqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connection">A valid MySqlConnection</param>
        /// <param name="transaction">A valid MySqlTransaction</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">The stored procedure name or T-SQL command</param>
        /// <param name="dataSet">A dataset wich will contain the resultset generated by the command</param>
        /// <param name="tableNames">This array will be used to create table mappings allowing the DataTables to be referenced
        /// by a user defined name (probably the actual table name)
        /// </param>
        /// <param name="commandParameters">An array of SqlParamters used to execute the command</param>
        private void FillDataset(MySqlConnection connection, MySqlTransaction transaction, CommandType commandType,
            string commandText, DataSet dataSet, string[] tableNames,
            params MySqlParameter[] commandParameters)
        {
            if (connection == null) throw new ArgumentNullException("connection");
            if (dataSet == null) throw new ArgumentNullException("dataSet");

            // Create a command and prepare it for execution
            MySqlCommand command = new MySqlCommand();
            bool mustCloseConnection = false;
            PrepareCommand(command, connection, transaction, commandType, commandText, commandParameters, out mustCloseConnection);

            // Create the DataAdapter & DataSet
            using (MySqlDataAdapter dataAdapter = new MySqlDataAdapter(command))
            {

                // Add the table mappings specified by the user
                if (tableNames != null && tableNames.Length > 0)
                {
                    string tableName = "Table";
                    for (int index = 0; index < tableNames.Length; index++)
                    {
                        if (tableNames[index] == null || tableNames[index].Length == 0) throw new ArgumentException("The tableNames parameter must contain a list of tables, a value was provided as null or empty string.", "tableNames");
                        dataAdapter.TableMappings.Add(tableName, tableNames[index]);
                        tableName += (index + 1).ToString();
                    }
                }

                // Fill the DataSet using default values for DataTable names, etc
                dataAdapter.Fill(dataSet);

                // Detach the SqlParameters from the command object, so they can be used again
                command.Parameters.Clear();
            }

            if (mustCloseConnection)
            {
                connection.Close();
                //  MySqlConnection.ClearAllPools();
            }
        }
        #endregion








        #region UpdateDataset
        /// <summary>
        /// Executes the respective command for each inserted, updated, or deleted row in the DataSet.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  UpdateDataset(conn, insertCommand, deleteCommand, updateCommand, dataSet, "Order");
        /// </remarks>
        /// <param name="insertCommand">A valid transact-SQL statement or stored procedure to insert new records into the data source</param>
        /// <param name="deleteCommand">A valid transact-SQL statement or stored procedure to delete records from the data source</param>
        /// <param name="updateCommand">A valid transact-SQL statement or stored procedure used to update records in the data source</param>
        /// <param name="dataSet">The DataSet used to update the data source</param>
        /// <param name="tableName">The DataTable used to update the data source.</param>
        public void UpdateDataset(MySqlCommand insertCommand, MySqlCommand deleteCommand, MySqlCommand updateCommand, DataSet dataSet, string tableName)
        {
            if (insertCommand == null) throw new ArgumentNullException("insertCommand");
            if (deleteCommand == null) throw new ArgumentNullException("deleteCommand");
            if (updateCommand == null) throw new ArgumentNullException("updateCommand");
            if (tableName == null || tableName.Length == 0) throw new ArgumentNullException("tableName");

            // Create a MySqlDataReader, and dispose of it after we are done
            using (MySqlDataAdapter dataAdapter = new MySqlDataAdapter())
            {
                // Set the data adapter commands

                dataAdapter.UpdateCommand = updateCommand;
                dataAdapter.InsertCommand = insertCommand;
                dataAdapter.DeleteCommand = deleteCommand;

                // Update the dataset changes in the data source
                dataAdapter.Update(dataSet, tableName);

                // Commit all the changes made to the DataSet
                dataSet.AcceptChanges();
            }
        }
        #endregion






        #region CreateCommand
        /// <summary>
        /// Simplify the creation of a Sql command object by allowing
        /// a stored procedure and optional parameters to be provided
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  MySqlCommand command = CreateCommand(conn, "AddCustomer", "CustomerID", "CustomerName");
        /// </remarks>
        /// <param name="connection">A valid MySqlConnection object</param>
        /// <param name="spName">The name of the stored procedure</param>
        /// <param name="sourceColumns">An array of string to be assigned as the source columns of the stored procedure parameters</param>
        /// <returns>A valid MySqlCommand object</returns>
        public MySqlCommand CreateCommand(MySqlConnection connection, string spName, params string[] sourceColumns)
        {
            if (connection == null) throw new ArgumentNullException("connection");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

            // Create a MySqlCommand
            MySqlCommand cmd = new MySqlCommand(spName, connection);
            cmd.CommandType = CommandType.StoredProcedure;

            // If we receive parameter values, we need to figure out where they go
            if ((sourceColumns != null) && (sourceColumns.Length > 0))
            {
                // Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                MySqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connection, spName);

                // Assign the provided source columns to these parameters based on parameter order
                for (int index = 0; index < sourceColumns.Length; index++)
                    commandParameters[index].SourceColumn = sourceColumns[index];

                // Attach the discovered parameters to the MySqlCommand object
                AttachParameters(cmd, commandParameters);
            }

            return cmd;
        }
        #endregion







        #region ExecuteNonQueryTypedParams
        /// <summary>
        /// Execute a stored procedure via a MySqlCommand (that returns no resultset) against the database specified in 
        /// the connection string using the dataRow column values as the stored procedure's parameters values.
        /// This method will query the database to discover the parameters for the 
        /// stored procedure (the first time each stored procedure is called), and assign the values based on row values.
        /// </summary>
        /// <param name="connectionString">A valid connection string for a MySqlConnection</param>
        /// <param name="spName">The name of the stored procedure</param>
        /// <param name="dataRow">The dataRow used to hold the stored procedure's parameter values.</param>
        /// <returns>An int representing the number of rows affected by the command</returns>
        public int ExecuteNonQueryTypedParams(String connectionString, String spName, DataRow dataRow)
        {
            if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

            // If the row has values, the store procedure parameters must be initialized
            if (dataRow != null && dataRow.ItemArray.Length > 0)
            {
                // Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                MySqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connectionString, spName);

                // Set the parameters values
                AssignParameterValues(commandParameters, dataRow);

                return ExecuteNonQuery(connectionString, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                return ExecuteNonQuery(connectionString, CommandType.StoredProcedure, spName);
            }
        }




        /// <summary>
        /// Execute a stored procedure via a MySqlCommand (that returns no resultset) against the specified MySqlConnection 
        /// using the dataRow column values as the stored procedure's parameters values.  
        /// This method will query the database to discover the parameters for the 
        /// stored procedure (the first time each stored procedure is called), and assign the values based on row values.
        /// </summary>
        /// <param name="connection">A valid MySqlConnection object</param>
        /// <param name="spName">The name of the stored procedure</param>
        /// <param name="dataRow">The dataRow used to hold the stored procedure's parameter values.</param>
        /// <returns>An int representing the number of rows affected by the command</returns>
        private int ExecuteNonQueryTypedParams(MySqlConnection connection, String spName, DataRow dataRow)
        {
            if (connection == null) throw new ArgumentNullException("connection");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

            // If the row has values, the store procedure parameters must be initialized
            if (dataRow != null && dataRow.ItemArray.Length > 0)
            {
                // Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                MySqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connection, spName);

                // Set the parameters values
                AssignParameterValues(commandParameters, dataRow);

                return ExecuteNonQuery(connection, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                return ExecuteNonQuery(connection, CommandType.StoredProcedure, spName);
            }
        }





        /// <summary>
        /// Execute a stored procedure via a MySqlCommand (that returns no resultset) against the specified
        /// MySqlTransaction using the dataRow column values as the stored procedure's parameters values.
        /// This method will query the database to discover the parameters for the 
        /// stored procedure (the first time each stored procedure is called), and assign the values based on row values.
        /// </summary>
        /// <param name="transaction">A valid MySqlTransaction object</param>
        /// <param name="spName">The name of the stored procedure</param>
        /// <param name="dataRow">The dataRow used to hold the stored procedure's parameter values.</param>
        /// <returns>An int representing the number of rows affected by the command</returns>
        public int ExecuteNonQueryTypedParams(MySqlTransaction transaction, String spName, DataRow dataRow)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            if (transaction != null && transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

            // Sf the row has values, the store procedure parameters must be initialized
            if (dataRow != null && dataRow.ItemArray.Length > 0)
            {
                // Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                MySqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(transaction.Connection, spName);

                // Set the parameters values
                AssignParameterValues(commandParameters, dataRow);

                return ExecuteNonQuery(transaction, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                return ExecuteNonQuery(transaction, CommandType.StoredProcedure, spName);
            }
        }
        #endregion







        #region ExecuteDatasetTypedParams
        /// <summary>
        /// Execute a stored procedure via a MySqlCommand (that returns a resultset) against the database specified in 
        /// the connection string using the dataRow column values as the stored procedure's parameters values.
        /// This method will query the database to discover the parameters for the 
        /// stored procedure (the first time each stored procedure is called), and assign the values based on row values.
        /// </summary>
        /// <param name="connectionString">A valid connection string for a MySqlConnection</param>
        /// <param name="spName">The name of the stored procedure</param>
        /// <param name="dataRow">The dataRow used to hold the stored procedure's parameter values.</param>
        /// <returns>A dataset containing the resultset generated by the command</returns>
        public DataSet ExecuteDatasetTypedParams(string connectionString, String spName, DataRow dataRow)
        {
            if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

            //If the row has values, the store procedure parameters must be initialized
            if (dataRow != null && dataRow.ItemArray.Length > 0)
            {
                // Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                MySqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connectionString, spName);

                // Set the parameters values
                AssignParameterValues(commandParameters, dataRow);

                return ExecuteDataset(connectionString, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                return ExecuteDataset(connectionString, CommandType.StoredProcedure, spName);
            }
        }




        /// <summary>
        /// Execute a stored procedure via a MySqlCommand (that returns a resultset) against the specified MySqlConnection 
        /// using the dataRow column values as the store procedure's parameters values.
        /// This method will query the database to discover the parameters for the 
        /// stored procedure (the first time each stored procedure is called), and assign the values based on row values.
        /// </summary>
        /// <param name="connection">A valid MySqlConnection object</param>
        /// <param name="spName">The name of the stored procedure</param>
        /// <param name="dataRow">The dataRow used to hold the stored procedure's parameter values.</param>
        /// <returns>A dataset containing the resultset generated by the command</returns>
        private DataSet ExecuteDatasetTypedParams(MySqlConnection connection, String spName, DataRow dataRow)
        {
            if (connection == null) throw new ArgumentNullException("connection");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

            // If the row has values, the store procedure parameters must be initialized
            if (dataRow != null && dataRow.ItemArray.Length > 0)
            {
                // Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                MySqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connection, spName);

                // Set the parameters values
                AssignParameterValues(commandParameters, dataRow);

                return ExecuteDataset(connection, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                return ExecuteDataset(connection, CommandType.StoredProcedure, spName);
            }
        }




        /// <summary>
        /// Execute a stored procedure via a MySqlCommand (that returns a resultset) against the specified MySqlTransaction 
        /// using the dataRow column values as the stored procedure's parameters values.
        /// This method will query the database to discover the parameters for the 
        /// stored procedure (the first time each stored procedure is called), and assign the values based on row values.
        /// </summary>
        /// <param name="transaction">A valid MySqlTransaction object</param>
        /// <param name="spName">The name of the stored procedure</param>
        /// <param name="dataRow">The dataRow used to hold the stored procedure's parameter values.</param>
        /// <returns>A dataset containing the resultset generated by the command</returns>
        public DataSet ExecuteDatasetTypedParams(MySqlTransaction transaction, String spName, DataRow dataRow)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            if (transaction != null && transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

            // If the row has values, the store procedure parameters must be initialized
            if (dataRow != null && dataRow.ItemArray.Length > 0)
            {
                // Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                MySqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(transaction.Connection, spName);

                // Set the parameters values
                AssignParameterValues(commandParameters, dataRow);

                return ExecuteDataset(transaction, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                return ExecuteDataset(transaction, CommandType.StoredProcedure, spName);
            }
        }

        #endregion







        #region ExecuteReaderTypedParams
        /// <summary>
        /// Execute a stored procedure via a MySqlCommand (that returns a resultset) against the database specified in 
        /// the connection string using the dataRow column values as the stored procedure's parameters values.
        /// This method will query the database to discover the parameters for the 
        /// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
        /// </summary>
        /// <param name="connectionString">A valid connection string for a MySqlConnection</param>
        /// <param name="spName">The name of the stored procedure</param>
        /// <param name="dataRow">The dataRow used to hold the stored procedure's parameter values.</param>
        /// <returns>A MySqlDataReader containing the resultset generated by the command</returns>
        public MySqlDataReader ExecuteReaderTypedParams(String connectionString, String spName, DataRow dataRow)
        {
            if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

            // If the row has values, the store procedure parameters must be initialized
            if (dataRow != null && dataRow.ItemArray.Length > 0)
            {
                // Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                MySqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connectionString, spName);

                // Set the parameters values
                AssignParameterValues(commandParameters, dataRow);

                return ExecuteReader(connectionString, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                return ExecuteReader(connectionString, CommandType.StoredProcedure, spName);
            }
        }




        /// <summary>
        /// Execute a stored procedure via a MySqlCommand (that returns a resultset) against the specified MySqlConnection 
        /// using the dataRow column values as the stored procedure's parameters values.
        /// This method will query the database to discover the parameters for the 
        /// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
        /// </summary>
        /// <param name="connection">A valid MySqlConnection object</param>
        /// <param name="spName">The name of the stored procedure</param>
        /// <param name="dataRow">The dataRow used to hold the stored procedure's parameter values.</param>
        /// <returns>A MySqlDataReader containing the resultset generated by the command</returns>
        private MySqlDataReader ExecuteReaderTypedParams(MySqlConnection connection, String spName, DataRow dataRow)
        {
            if (connection == null) throw new ArgumentNullException("connection");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

            // If the row has values, the store procedure parameters must be initialized
            if (dataRow != null && dataRow.ItemArray.Length > 0)
            {
                // Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                MySqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connection, spName);

                // Set the parameters values
                AssignParameterValues(commandParameters, dataRow);

                return ExecuteReader(connection, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                return ExecuteReader(connection, CommandType.StoredProcedure, spName);
            }
        }



        /// <summary>
        /// Execute a stored procedure via a MySqlCommand (that returns a resultset) against the specified MySqlTransaction 
        /// using the dataRow column values as the stored procedure's parameters values.
        /// This method will query the database to discover the parameters for the 
        /// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
        /// </summary>
        /// <param name="transaction">A valid MySqlTransaction object</param>
        /// <param name="spName">The name of the stored procedure</param>
        /// <param name="dataRow">The dataRow used to hold the stored procedure's parameter values.</param>
        /// <returns>A MySqlDataReader containing the resultset generated by the command</returns>
        public MySqlDataReader ExecuteReaderTypedParams(MySqlTransaction transaction, String spName, DataRow dataRow)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            if (transaction != null && transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

            // If the row has values, the store procedure parameters must be initialized
            if (dataRow != null && dataRow.ItemArray.Length > 0)
            {
                // Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                MySqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(transaction.Connection, spName);

                // Set the parameters values
                AssignParameterValues(commandParameters, dataRow);

                return ExecuteReader(transaction, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                return ExecuteReader(transaction, CommandType.StoredProcedure, spName);
            }
        }
        #endregion







        #region ExecuteScalarTypedParams
        /// <summary>
        /// Execute a stored procedure via a MySqlCommand (that returns a 1x1 resultset) against the database specified in 
        /// the connection string using the dataRow column values as the stored procedure's parameters values.
        /// This method will query the database to discover the parameters for the 
        /// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
        /// </summary>
        /// <param name="connectionString">A valid connection string for a MySqlConnection</param>
        /// <param name="spName">The name of the stored procedure</param>
        /// <param name="dataRow">The dataRow used to hold the stored procedure's parameter values.</param>
        /// <returns>An object containing the value in the 1x1 resultset generated by the command</returns>
        public object ExecuteScalarTypedParams(String connectionString, String spName, DataRow dataRow)
        {
            if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

            // If the row has values, the store procedure parameters must be initialized
            if (dataRow != null && dataRow.ItemArray.Length > 0)
            {
                // Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                MySqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connectionString, spName);

                // Set the parameters values
                AssignParameterValues(commandParameters, dataRow);

                return ExecuteScalar(connectionString, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                return ExecuteScalar(connectionString, CommandType.StoredProcedure, spName);
            }
        }

        /// <summary>
        /// Execute a stored procedure via a MySqlCommand (that returns a 1x1 resultset) against the specified MySqlConnection 
        /// using the dataRow column values as the stored procedure's parameters values.
        /// This method will query the database to discover the parameters for the 
        /// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
        /// </summary>
        /// <param name="connection">A valid MySqlConnection object</param>
        /// <param name="spName">The name of the stored procedure</param>
        /// <param name="dataRow">The dataRow used to hold the stored procedure's parameter values.</param>
        /// <returns>An object containing the value in the 1x1 resultset generated by the command</returns>
        private object ExecuteScalarTypedParams(MySqlConnection connection, String spName, DataRow dataRow)
        {
            if (connection == null) throw new ArgumentNullException("connection");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

            // If the row has values, the store procedure parameters must be initialized
            if (dataRow != null && dataRow.ItemArray.Length > 0)
            {
                // Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                MySqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connection, spName);

                // Set the parameters values
                AssignParameterValues(commandParameters, dataRow);

                return ExecuteScalar(connection, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                return ExecuteScalar(connection, CommandType.StoredProcedure, spName);
            }
        }

        /// <summary>
        /// Execute a stored procedure via a MySqlCommand (that returns a 1x1 resultset) against the specified MySqlTransaction
        /// using the dataRow column values as the stored procedure's parameters values.
        /// This method will query the database to discover the parameters for the 
        /// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
        /// </summary>
        /// <param name="transaction">A valid MySqlTransaction object</param>
        /// <param name="spName">The name of the stored procedure</param>
        /// <param name="dataRow">The dataRow used to hold the stored procedure's parameter values.</param>
        /// <returns>An object containing the value in the 1x1 resultset generated by the command</returns>
        public object ExecuteScalarTypedParams(MySqlTransaction transaction, String spName, DataRow dataRow)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            if (transaction != null && transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

            // If the row has values, the store procedure parameters must be initialized
            if (dataRow != null && dataRow.ItemArray.Length > 0)
            {
                // Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                MySqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(transaction.Connection, spName);

                // Set the parameters values
                AssignParameterValues(commandParameters, dataRow);

                return ExecuteScalar(transaction, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                return ExecuteScalar(transaction, CommandType.StoredProcedure, spName);
            }
        }
        #endregion







        #region ExecuteXmlReaderTypedParams
        /// <summary>
        /// Execute a stored procedure via a MySqlCommand (that returns a resultset) against the specified MySqlConnection 
        /// using the dataRow column values as the stored procedure's parameters values.
        /// This method will query the database to discover the parameters for the 
        /// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
        /// </summary>
        /// <param name="connection">A valid MySqlConnection object</param>
        /// <param name="spName">The name of the stored procedure</param>
        /// <param name="dataRow">The dataRow used to hold the stored procedure's parameter values.</param>
        /// <returns>An XmlReader containing the resultset generated by the command</returns>
        public XmlReader ExecuteXmlReaderTypedParams(MySqlConnection connection, String spName, DataRow dataRow)
        {
            if (connection == null) throw new ArgumentNullException("connection");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

            // If the row has values, the store procedure parameters must be initialized
            if (dataRow != null && dataRow.ItemArray.Length > 0)
            {
                // Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                MySqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connection, spName);

                // Set the parameters values
                AssignParameterValues(commandParameters, dataRow);

                return ExecuteXmlReader(connection, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                return ExecuteXmlReader(connection, CommandType.StoredProcedure, spName);
            }
        }

        /// <summary>
        /// Execute a stored procedure via a MySqlCommand (that returns a resultset) against the specified MySqlTransaction 
        /// using the dataRow column values as the stored procedure's parameters values.
        /// This method will query the database to discover the parameters for the 
        /// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
        /// </summary>
        /// <param name="transaction">A valid MySqlTransaction object</param>
        /// <param name="spName">The name of the stored procedure</param>
        /// <param name="dataRow">The dataRow used to hold the stored procedure's parameter values.</param>
        /// <returns>An XmlReader containing the resultset generated by the command</returns>
        public XmlReader ExecuteXmlReaderTypedParams(MySqlTransaction transaction, String spName, DataRow dataRow)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            if (transaction != null && transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

            // If the row has values, the store procedure parameters must be initialized
            if (dataRow != null && dataRow.ItemArray.Length > 0)
            {
                // Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                MySqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(transaction.Connection, spName);

                // Set the parameters values
                AssignParameterValues(commandParameters, dataRow);

                return ExecuteXmlReader(transaction, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                return ExecuteXmlReader(transaction, CommandType.StoredProcedure, spName);
            }
        }
        #endregion







    }

    public sealed class SqlHelperParameterCache
    {
        #region private methods, variables, and constructors

        //Since this class provides only static methods, make the default constructor private to prevent 
        //instances from being created with "new SqlHelperParameterCache()"
        private SqlHelperParameterCache() { }

        private static Hashtable paramCache = Hashtable.Synchronized(new Hashtable());

        /// <summary>
        /// Resolve at run time the appropriate set of SqlParameters for a stored procedure
        /// </summary>
        /// <param name="connection">A valid MySqlConnection object</param>
        /// <param name="spName">The name of the stored procedure</param>
        /// <param name="includeReturnValueParameter">Whether or not to include their return value parameter</param>
        /// <returns>The parameter array discovered.</returns>
        private static MySqlParameter[] DiscoverSpParameterSet(MySqlConnection connection, string spName, bool includeReturnValueParameter)
        {
            if (connection == null) throw new ArgumentNullException("connection");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

            MySqlCommand cmd = new MySqlCommand(spName, connection);
            cmd.CommandType = CommandType.StoredProcedure;

            connection.Open();
            MySqlCommandBuilder.DeriveParameters(cmd);
            connection.Close();

            if (!includeReturnValueParameter)
            {
                cmd.Parameters.RemoveAt(0);
            }

            MySqlParameter[] discoveredParameters = new MySqlParameter[cmd.Parameters.Count];

            cmd.Parameters.CopyTo(discoveredParameters, 0);

            // Init the parameters with a DBNull value
            foreach (MySqlParameter discoveredParameter in discoveredParameters)
            {
                discoveredParameter.Value = DBNull.Value;
            }
            return discoveredParameters;
        }

        /// <summary>
        /// Deep copy of cached MySqlParameter array
        /// </summary>
        /// <param name="originalParameters"></param>
        /// <returns></returns>
        private static MySqlParameter[] CloneParameters(MySqlParameter[] originalParameters)
        {
            MySqlParameter[] clonedParameters = new MySqlParameter[originalParameters.Length];

            for (int i = 0, j = originalParameters.Length; i < j; i++)
            {
                clonedParameters[i] = (MySqlParameter)((ICloneable)originalParameters[i]).Clone();
            }

            return clonedParameters;
        }

        #endregion private methods, variables, and constructors

        #region caching functions

        /// <summary>
        /// Add parameter array to the cache
        /// </summary>
        /// <param name="connectionString">A valid connection string for a MySqlConnection</param>
        /// <param name="commandText">The stored procedure name or T-SQL command</param>
        /// <param name="commandParameters">An array of SqlParamters to be cached</param>
        public static void CacheParameterSet(string connectionString, string commandText, params MySqlParameter[] commandParameters)
        {
            if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");
            if (commandText == null || commandText.Length == 0) throw new ArgumentNullException("commandText");

            string hashKey = connectionString + ":" + commandText;

            paramCache[hashKey] = commandParameters;
        }

        /// <summary>
        /// Retrieve a parameter array from the cache
        /// </summary>
        /// <param name="connectionString">A valid connection string for a MySqlConnection</param>
        /// <param name="commandText">The stored procedure name or T-SQL command</param>
        /// <returns>An array of SqlParamters</returns>
        public static MySqlParameter[] GetCachedParameterSet(string connectionString, string commandText)
        {
            if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");
            if (commandText == null || commandText.Length == 0) throw new ArgumentNullException("commandText");

            string hashKey = connectionString + ":" + commandText;

            MySqlParameter[] cachedParameters = paramCache[hashKey] as MySqlParameter[];
            if (cachedParameters == null)
            {
                return null;
            }
            else
            {
                return CloneParameters(cachedParameters);
            }
        }

        #endregion caching functions

        #region Parameter Discovery Functions

        /// <summary>
        /// Retrieves the set of SqlParameters appropriate for the stored procedure
        /// </summary>
        /// <remarks>
        /// This method will query the database for this information, and then store it in a cache for future requests.
        /// </remarks>
        /// <param name="connectionString">A valid connection string for a MySqlConnection</param>
        /// <param name="spName">The name of the stored procedure</param>
        /// <returns>An array of SqlParameters</returns>
        public static MySqlParameter[] GetSpParameterSet(string connectionString, string spName)
        {
            return GetSpParameterSet(connectionString, spName, false);
        }

        /// <summary>
        /// Retrieves the set of SqlParameters appropriate for the stored procedure
        /// </summary>
        /// <remarks>
        /// This method will query the database for this information, and then store it in a cache for future requests.
        /// </remarks>
        /// <param name="connectionString">A valid connection string for a MySqlConnection</param>
        /// <param name="spName">The name of the stored procedure</param>
        /// <param name="includeReturnValueParameter">A bool value indicating whether the return value parameter should be included in the results</param>
        /// <returns>An array of SqlParameters</returns>
        public static MySqlParameter[] GetSpParameterSet(string connectionString, string spName, bool includeReturnValueParameter)
        {
            if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                return GetSpParameterSetInternal(connection, spName, includeReturnValueParameter);
            }
        }

        /// <summary>
        /// Retrieves the set of SqlParameters appropriate for the stored procedure
        /// </summary>
        /// <remarks>
        /// This method will query the database for this information, and then store it in a cache for future requests.
        /// </remarks>
        /// <param name="connection">A valid MySqlConnection object</param>
        /// <param name="spName">The name of the stored procedure</param>
        /// <returns>An array of SqlParameters</returns>
        internal static MySqlParameter[] GetSpParameterSet(MySqlConnection connection, string spName)
        {
            return GetSpParameterSet(connection, spName, false);
        }

        /// <summary>
        /// Retrieves the set of SqlParameters appropriate for the stored procedure
        /// </summary>
        /// <remarks>
        /// This method will query the database for this information, and then store it in a cache for future requests.
        /// </remarks>
        /// <param name="connection">A valid MySqlConnection object</param>
        /// <param name="spName">The name of the stored procedure</param>
        /// <param name="includeReturnValueParameter">A bool value indicating whether the return value parameter should be included in the results</param>
        /// <returns>An array of SqlParameters</returns>
        internal static MySqlParameter[] GetSpParameterSet(MySqlConnection connection, string spName, bool includeReturnValueParameter)
        {
            if (connection == null) throw new ArgumentNullException("connection");
            using (MySqlConnection clonedConnection = (MySqlConnection)((ICloneable)connection).Clone())
            {
                return GetSpParameterSetInternal(clonedConnection, spName, includeReturnValueParameter);
            }
        }

        /// <summary>
        /// Retrieves the set of SqlParameters appropriate for the stored procedure
        /// </summary>
        /// <param name="connection">A valid MySqlConnection object</param>
        /// <param name="spName">The name of the stored procedure</param>
        /// <param name="includeReturnValueParameter">A bool value indicating whether the return value parameter should be included in the results</param>
        /// <returns>An array of SqlParameters</returns>
        private static MySqlParameter[] GetSpParameterSetInternal(MySqlConnection connection, string spName, bool includeReturnValueParameter)
        {
            if (connection == null) throw new ArgumentNullException("connection");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

            string hashKey = connection.ConnectionString + ":" + spName + (includeReturnValueParameter ? ":include ReturnValue Parameter" : "");

            MySqlParameter[] cachedParameters;

            cachedParameters = paramCache[hashKey] as MySqlParameter[];
            if (cachedParameters == null)
            {
                MySqlParameter[] spParameters = DiscoverSpParameterSet(connection, spName, includeReturnValueParameter);
                paramCache[hashKey] = spParameters;
                cachedParameters = spParameters;
            }

            return CloneParameters(cachedParameters);
        }

        #endregion Parameter Discovery Functions

    }
}
