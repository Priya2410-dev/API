using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
//using System.Data.SqlClient;
using System.Configuration;
using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration;
using Calyx_Solutions.Controllers;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Calyx_Solutions
{
    public class db_connection
    {
        private IConfiguration _configuration;
        static string connectionString;
        private string conneObj;
        public db_connection(IConfiguration configuration)
        {
            _configuration = configuration;
            connectionString = _configuration.GetConnectionString("ConnectionString");
        }





        //static ConnectionStringSettings con = ConfigurationManager.ConnectionStrings["ConnectionString"];

        static MySqlConnection cn = new MySqlConnection(connectionString);
        static string conn = connectionString;
        ///readonly string conn;
        public db_connection()
        {
        }



        public string ConnectionStringSection()
        {
            var objBuilder = new ConfigurationBuilder()
          .SetBasePath(Directory.GetCurrentDirectory())
          .AddJsonFile("appSettings.json", optional: false, reloadOnChange: true);
            IConfiguration conManager = objBuilder.Build();

            var conn = conManager.GetConnectionString("connectionString");

            if (conn == null)
            {
                conn = "";
            }

            return conn.ToString();
        }

        public string WebConnSetting()
        {
            var objBuilder = new ConfigurationBuilder()
          .SetBasePath(Directory.GetCurrentDirectory())
          .AddJsonFile("appSettings.json", optional: false, reloadOnChange: true);
            IConfiguration conManager = objBuilder.Build();

            var conn = conManager.GetConnectionString("WEB_DB_CONN");

            if (conn == null)
            {
                conn = "";
            }

            return conn.ToString();
        }

        static public void EnsureMySqlConnection()
        {
            if (null == cn)
                cn = new MySqlConnection(conn);//ConfigurationManager.ConnectionStrings["ConnectionString"].ToString());

            if (cn.State != ConnectionState.Open)
                cn.Open();
        }
        static public int ExecuteNonQueryProcedure(MySqlConnector.MySqlCommand sqlcmd)
        {
            int retVal = 0;
            try
            {
                db_connection aController = new db_connection();
                string conneObj = aController.ConnectionStringSection();

                using (MySqlConnector.MySqlConnection connect = new MySqlConnector.MySqlConnection(conneObj))
                {
                    connect.Open();
                    MySqlConnector.MySqlCommand cmd = new MySqlConnector.MySqlCommand();
                    cmd = sqlcmd; cmd.Connection = connect; //cmd.CommandTimeout = 0;
                    retVal = sqlcmd.ExecuteNonQuery(); connect.Close();
                }
            }
            catch (Exception ex)
            {
                //throw ex;
            }
            finally
            {
                cn.Close();
            }
            return retVal;
        }

        static public object ExecuteScalarProcedure(MySqlConnector.MySqlCommand sqlcmd)
        {

            object retVal = 0;
            try
            {
                db_connection aController = new db_connection();
                string conneObj = aController.ConnectionStringSection();

                using (MySqlConnector.MySqlConnection connect = new MySqlConnector.MySqlConnection(conneObj))
                {
                    connect.Open();
                    MySqlConnector.MySqlCommand cmd = new MySqlConnector.MySqlCommand();
                    cmd = sqlcmd; cmd.Connection = connect; //cmd.CommandTimeout = 0;
                    retVal = cmd.ExecuteScalar();
                    connect.Close();
                }
            }
            finally
            {
                cn.Close();
            }
            return retVal;


        }

        public static async Task<object> ExecuteScalarProcedureAsync(MySqlConnector.MySqlCommand sqlcmd)
        {
            object retVal = 0;
            try
            {
                db_connection aController = new db_connection();
                string conneObj = aController.ConnectionStringSection();

                using (MySqlConnector.MySqlConnection connect = new MySqlConnector.MySqlConnection(conneObj))
                {
                    await connect.OpenAsync(); // Open connection asynchronously

                    using (MySqlConnector.MySqlCommand cmd = sqlcmd)
                    {
                        cmd.Connection = connect;
                        retVal = await cmd.ExecuteScalarAsync(); // Execute asynchronously
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in ExecuteScalarProcedureAsync: {ex.Message}");
            }

            return retVal;
        }


        //static public MySqlDataReader ExecuteQueryReaderProcedure(MySqlCommand sqlcmd)
        //{
        //    MySqlDataReader reader = null;
        //    try
        //    {
        //        EnsureMySqlConnection();
        //        sqlcmd.Connection = cn;
        //        reader = sqlcmd.ExecuteReader();
        //    }
        //    finally
        //    {
        //        cn.Close();
        //    }
        //    return reader;
        //}

        static public DataTable ExecuteQueryDataTableProcedure(MySqlConnector.MySqlCommand sqlcmd)
        {
            try
            {
                db_connection aController = new db_connection();
                string conneObj = aController.ConnectionStringSection();

                DataTable table = new DataTable();
                using (MySqlConnector.MySqlConnection connect = new MySqlConnector.MySqlConnection(conneObj))
                {
                    connect.Open();
                    MySqlConnector.MySqlCommand cmd = new MySqlConnector.MySqlCommand();//sqlcmd.CommandText.Trim(), connect);
                    cmd = sqlcmd; cmd.Connection = connect;
                    //cmd.CommandTimeout = 0;
                    MySqlConnector.MySqlDataAdapter da = new MySqlConnector.MySqlDataAdapter(cmd);
                    da.Fill(table);
                    connect.Close();
                }
                //dataAdapter.Fill(table);
                return table;
            }
            finally
            {
                cn.Close();
                sqlcmd.Dispose();
            }
        }

        static public DataSet ExecuteQueryDataSetProcedure(MySqlConnector.MySqlCommand sqlcmd)
        {
            try
            {
                //EnsureMySqlConnection();
                //sqlcmd.Connection = cn;
                //MySqlDataAdapter dataAdapter = new MySqlDataAdapter(sqlcmd);
                //DataSet set = new DataSet();
                //dataAdapter.Fill(set);
                //return set;
                DataSet dsresult = new DataSet();
                using (MySqlConnector.MySqlConnection connect = new MySqlConnector.MySqlConnection(cn.ConnectionString))
                {
                    connect.Open();
                    MySqlConnector.MySqlCommand cmd = new MySqlConnector.MySqlCommand();
                    cmd = sqlcmd; cmd.Connection = connect; //cmd.CommandTimeout = 0;
                    MySqlConnector.MySqlDataAdapter da = new MySqlConnector.MySqlDataAdapter(cmd);
                    da.Fill(dsresult); connect.Close();
                    return dsresult;
                }
            }
            finally
            {
                cn.Close();
                sqlcmd.Dispose();
            }
        }

        public static string ConnectionStringStatic()
        {
            var objBuilder = new ConfigurationBuilder()
          .SetBasePath(Directory.GetCurrentDirectory())
          .AddJsonFile("appSettings.json", optional: false, reloadOnChange: true);
            IConfiguration conManager = objBuilder.Build();

            var conn = conManager.GetConnectionString("connectionString");

            if (conn == null)
            {
                conn = "";
            }

            return conn.ToString();
        }

    }

}
