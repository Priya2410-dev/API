using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Configuration;
using Microsoft.Extensions.Configuration;

namespace Calyx_Solutions.Service
{
    public class mts_connection
    {
        private readonly IConfiguration _configuration;
        public string WebConnSetting()
        {
            db_connection _dbConnection = new db_connection();

            //var connectionString = _configuration.GetConnectionString("WEB_DB_CONN"); //ConfigurationManager.AppSettings["WEB_DB_CONN"];
            return _dbConnection.ConnectionStringSection();
        }
        /*public string CentralConnSetting()
        {
            var connectionString = ConfigurationManager.AppSettings["CENTRAL_DB_CONN"];
            return connectionString;
        }*/
    }
}
